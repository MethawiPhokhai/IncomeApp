using backend.Features.Auth.Models;
using Supabase;

namespace backend.Features.Auth.Services;

public class FacebookLoginService : IFacebookLoginService
{
    private readonly Client _supabase;

    public FacebookLoginService(Client supabase)
    {
        _supabase = supabase;
    }

    public async Task<User?> FindUserByProviderAsync(string providerName, string providerUserId)
    {
        try
        {
            // Query user_providers table to find matching provider
            var providerResult = await _supabase
                .From<UserProvider>()
                .Where(p => p.ProviderName == providerName && p.ProviderUserId == providerUserId)
                .Single();

            if (providerResult == null)
            {
                return null;
            }

            // Get the associated user
            var userResult = await _supabase
                .From<User>()
                .Where(u => u.Id == providerResult.UserId)
                .Single();

            return userResult;
        }
        catch (Exception)
        {
            // User not found or error occurred
            return null;
        }
    }

    public async Task<User> CreateUserWithProviderAsync(User user, UserProvider provider)
    {
        // Set timestamps
        var now = DateTime.UtcNow;
        user.Id = Guid.NewGuid();
        user.CreatedAt = now;
        user.UpdatedAt = now;

        // Insert user
        var userResult = await _supabase
            .From<User>()
            .Insert(user);

        var createdUser = userResult.Models.FirstOrDefault();
        if (createdUser == null)
        {
            throw new Exception("Failed to create user");
        }

        // Set provider timestamps and link to user
        provider.Id = Guid.NewGuid();
        provider.UserId = createdUser.Id;
        provider.CreatedAt = now;
        provider.UpdatedAt = now;

        // Insert provider
        await _supabase
            .From<UserProvider>()
            .Insert(provider);

        return createdUser;
    }
}
