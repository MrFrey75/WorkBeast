using Microsoft.AspNetCore.Identity;

namespace WorkBeast.Core.Models;

/// <summary>
/// Application user entity with Identity integration
/// </summary>
public class ApplicationUser : IdentityUser<int>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastLoginAt { get; set; }
    public bool IsActive { get; set; } = true;

    public string FullName => $"{FirstName} {LastName}".Trim();
}

/// <summary>
/// Application role entity
/// </summary>
public class ApplicationRole : IdentityRole<int>
{
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
