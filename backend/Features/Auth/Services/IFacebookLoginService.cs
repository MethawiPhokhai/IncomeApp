using backend.Features.Auth.Models;

namespace backend.Features.Auth.Services;

public interface IFacebookLoginService
{
    /// <summary>
    /// Find user by OAuth provider (e.g., 'facebook') and provider user ID
    /// </summary>
    Task<User?> FindUserByProviderAsync(string providerName, string providerUserId);

    /// <summary>
    /// Create a new user with associated provider information
    /// </summary>
    Task<User> CreateUserWithProviderAsync(User user, UserProvider provider);
}
