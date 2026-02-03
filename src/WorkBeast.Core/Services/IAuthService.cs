using WorkBeast.Core.DTOs;

namespace WorkBeast.Core.Services;

/// <summary>
/// Interface for authentication and user management
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Registers a new user
    /// </summary>
    Task<AuthResponse?> RegisterAsync(RegisterRequest request);

    /// <summary>
    /// Authenticates a user and returns JWT token
    /// </summary>
    Task<AuthResponse?> LoginAsync(LoginRequest request);

    /// <summary>
    /// Gets user information by ID
    /// </summary>
    Task<UserResponse?> GetUserByIdAsync(int userId);

    /// <summary>
    /// Gets all users
    /// </summary>
    Task<List<UserResponse>> GetAllUsersAsync();

    /// <summary>
    /// Assigns a role to a user
    /// </summary>
    Task<bool> AssignRoleAsync(int userId, string roleName);

    /// <summary>
    /// Removes a role from a user
    /// </summary>
    Task<bool> RemoveRoleAsync(int userId, string roleName);

    /// <summary>
    /// Changes user password
    /// </summary>
    Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword);

    /// <summary>
    /// Deactivates a user account
    /// </summary>
    Task<bool> DeactivateUserAsync(int userId);

    /// <summary>
    /// Activates a user account
    /// </summary>
    Task<bool> ActivateUserAsync(int userId);
}
