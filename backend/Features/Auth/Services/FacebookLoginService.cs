using backend.Features.Auth.Models;
using Microsoft.Extensions.Logging;
using Supabase;

namespace backend.Features.Auth.Services;

public class FacebookLoginService : IFacebookLoginService
{
    private readonly Client _supabase;
    private readonly ILogger<FacebookLoginService> _logger;

    public FacebookLoginService(Client supabase, ILogger<FacebookLoginService> logger)
    {
        _supabase = supabase;
        _logger = logger;
    }

    public async Task<User?> FindUserByProviderAsync(string providerName, string providerUserId)
    {
        try
        {
            var providerResult = await _supabase
                .From<UserProvider>()
                .Where(p => p.ProviderName == providerName && p.ProviderUserId == providerUserId)
                .Single();

            if (providerResult == null)
            {
                return null;
            }

            var userResult = await _supabase
                .From<User>()
                .Where(u => u.Id == providerResult.UserId)
                .Single();

            return userResult;
        }
        catch (Exception ex)
        {
            // May be normal if user does not exist yet — log at Warning for visibility
            _logger.LogWarning(ex, "Provider lookup failed for {ProviderName} — user may not exist yet", providerName);
            return null;
        }
    }

    public async Task<User> CreateUserWithProviderAsync(User user, UserProvider provider)
    {
        var now = DateTime.UtcNow;
        user.Id = Guid.NewGuid();
        user.CreatedAt = now;
        user.UpdatedAt = now;

        var userResult = await _supabase
            .From<User>()
            .Insert(user);

        var createdUser = userResult.Models.FirstOrDefault();
        if (createdUser == null)
        {
            throw new Exception("Failed to create user");
        }

        provider.Id = Guid.NewGuid();
        provider.UserId = createdUser.Id;
        provider.CreatedAt = now;
        provider.UpdatedAt = now;

        await _supabase
            .From<UserProvider>()
            .Insert(provider);

        _logger.LogInformation("New user registered via {ProviderName}. UserId={UserId}", provider.ProviderName, createdUser.Id);

        return createdUser;
    }
}
