using backend.Features.Auth;
using backend.Features.Auth.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Supabase;

// // Load .env file
// Using env from railway instead of this
// DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);


// ── Config / Environment Variables ──────────────────────────────────────────
// From environment variables (.env)
var supabaseUrl = Environment.GetEnvironmentVariable("SUPABASE_URL")
    ?? throw new InvalidOperationException("SUPABASE_URL not found in environment variables");
var supabaseKey = Environment.GetEnvironmentVariable("SUPABASE_SERVICE_KEY")
    ?? throw new InvalidOperationException("SUPABASE_SERVICE_KEY not found in environment variables");

// From appsettings.json
var jwtKey = builder.Configuration["Jwt:Key"] ?? "super_secret_key_for_development_purpose_only_very_long_string";
var backendUrl = builder.Configuration["Cors:BackendUrl"] ?? "http://localhost:5098";
var frontendUrls = (builder.Configuration["Cors:FrontendUrl"] ?? "http://localhost:5173")
    .Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
    .Select(url => url.Trim().TrimEnd('/'))
    .ToArray();
// ────────────────────────────────────────────────────────────────────────────

// Configure Supabase
var supabaseOptions = new SupabaseOptions
{
    AutoConnectRealtime = false // We don't need realtime for this use case
};

builder.Services.AddSingleton(provider => new Supabase.Client(supabaseUrl, supabaseKey, supabaseOptions));

builder.Services.AddScoped<IFacebookLoginService, FacebookLoginService>();
builder.Services.AddScoped<IncomeApp.Features.Financial.Services.IFinancialService, IncomeApp.Features.Financial.Services.FinancialService>();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = backendUrl,
            ValidAudiences = frontendUrls,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();

// Add CORS for frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(frontendUrls)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Disable HTTPS redirection in production on Railway as it terminates SSL at the edge
// app.UseHttpsRedirection();
app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

// Map feature endpoints using vertical slice architecture
app.MapFacebookLoginEndpoint();
app.MapControllers();

app.Run();

