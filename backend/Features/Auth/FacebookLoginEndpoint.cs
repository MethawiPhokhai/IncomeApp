using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.IdentityModel.Tokens;
using backend.Features.Auth.Services;

namespace backend.Features.Auth;

// Request Model
public record FacebookLoginRequest(string AccessToken);

// Response Model
public record AuthResponse(bool Success, string Message, string? Token = null, UserDto? User = null);

public record UserDto(string Id, string Email, string Name, string PictureUrl);

// Internal DTO for Facebook Graph API response
public class FacebookUserDto
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("email")]
    public string? Email { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("picture")]
    public FacebookPictureData? Picture { get; set; }
}

public class FacebookPictureData
{
    [JsonPropertyName("data")]
    public FacebookPicture? Data { get; set; }
}

public class FacebookPicture
{
    [JsonPropertyName("url")]
    public string? Url { get; set; }
}

public static class FacebookLoginEndpoint
{
    private static readonly HttpClient HttpClient = new();

    public static void MapFacebookLoginEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/auth/facebook", async (
            FacebookLoginRequest request, 
            IConfiguration configuration,
            IFacebookLoginService supabaseService) =>
        {
            if (string.IsNullOrWhiteSpace(request.AccessToken))
            {
                return Results.BadRequest(new AuthResponse(false, "Access token is required"));
            }

            try
            {
                // 1. Verify Token with Facebook Graph API
                var fbResponse = await HttpClient.GetAsync(
                    $"https://graph.facebook.com/me?fields=email,name,picture&access_token={request.AccessToken}");

                if (!fbResponse.IsSuccessStatusCode)
                {
                    return Results.BadRequest(new AuthResponse(false, "Invalid Facebook Token"));
                }

                var fbContent = await fbResponse.Content.ReadAsStringAsync();
                var fbUser = System.Text.Json.JsonSerializer.Deserialize<FacebookUserDto>(fbContent);

                if (fbUser == null || string.IsNullOrEmpty(fbUser.Id))
                {
                    return Results.BadRequest(new AuthResponse(false, "Could not retrieve user data from Facebook"));
                }

                // 2. Find or Create User with Supabase
                var existingUser = await supabaseService.FindUserByProviderAsync("facebook", fbUser.Id);

                Models.User dbUser;
                if (existingUser != null)
                {
                    // User already exists
                    dbUser = existingUser;
                }
                else
                {
                    // Create new user
                    var newUser = new Models.User
                    {
                        Email = fbUser.Email ?? $"fb_{fbUser.Id}@placeholder.com",
                        Name = fbUser.Name ?? "Facebook User",
                        PictureUrl = fbUser.Picture?.Data?.Url ?? ""
                    };

                    var newProvider = new Models.UserProvider
                    {
                        ProviderName = "facebook",
                        ProviderUserId = fbUser.Id,
                        ProviderEmail = fbUser.Email,
                        ProviderData = new Dictionary<string, object>
                        {
                            { "name", fbUser.Name ?? "" },
                            { "picture_url", fbUser.Picture?.Data?.Url ?? "" }
                        }
                    };

                    dbUser = await supabaseService.CreateUserWithProviderAsync(newUser, newProvider);
                }

                // 3. Generate JWT
                var jwtKey = configuration["Jwt:Key"] ?? "super_secret_key_for_development_purpose_only_very_long_string";
                var jwtIssuer = configuration["Jwt:Issuer"] ?? "http://localhost:5098";
                var jwtAudience = configuration["Jwt:Audience"] ?? "http://localhost:5173";

                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                var claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, dbUser.Id.ToString()),
                    new Claim(JwtRegisteredClaimNames.Email, dbUser.Email),
                    new Claim(JwtRegisteredClaimNames.Name, dbUser.Name ?? ""),
                    new Claim("facebook_id", fbUser.Id)
                };

                var token = new JwtSecurityToken(
                    issuer: jwtIssuer,
                    audience: jwtAudience,
                    claims: claims,
                    expires: DateTime.Now.AddDays(7),
                    signingCredentials: credentials);

                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

                // 4. Return response with user data from database
                var userDto = new UserDto(
                    Id: dbUser.Id.ToString(),
                    Email: dbUser.Email,
                    Name: dbUser.Name ?? "",
                    PictureUrl: dbUser.PictureUrl ?? ""
                );

                return Results.Ok(new AuthResponse(
                    Success: true,
                    Message: "Login successful",
                    Token: tokenString,
                    User: userDto
                ));
            }
            catch (Exception ex)
            {
                return Results.Problem($"Authentication failed: {ex.Message}");
            }
        })
        .WithName("FacebookLogin")
        .WithOpenApi()
        .WithTags("Authentication");
    }
}
