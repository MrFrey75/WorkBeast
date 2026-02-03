using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using WorkBeast.Core.Models;

namespace WorkBeast.Data.Services;

/// <summary>
/// Service for seeding default roles and admin user
/// </summary>
public class RoleSeeder
{
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<RoleSeeder> _logger;

    public RoleSeeder(
        RoleManager<ApplicationRole> roleManager,
        UserManager<ApplicationUser> userManager,
        ILogger<RoleSeeder> logger)
    {
        _roleManager = roleManager;
        _userManager = userManager;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        await SeedRolesAsync();
        await SeedAdminUserAsync();
        await SeedBasicUserAsync();
    }

    private async Task SeedRolesAsync()
    {
        var roles = new[]
        {
            new ApplicationRole { Name = "Admin", Description = "Full system access", IsSystem = true},
            new ApplicationRole { Name = "Trainer", Description = "Can manage workout plans and exercises" },
            new ApplicationRole { Name = "User", Description = "Standard user access", IsSystem = true}
        };

        foreach (var role in roles)
        {
            if (!await _roleManager.RoleExistsAsync(role.Name!))
            {
                var result = await _roleManager.CreateAsync(role);
                if (result.Succeeded)
                {
                    _logger.LogInformation("Created role: {RoleName}", role.Name);
                }
                else
                {
                    _logger.LogError("Failed to create role: {RoleName}", role.Name);
                }
            }
        }
    }

    private async Task SeedAdminUserAsync()
    {
        const string adminEmail = "admin@workbeast.com";
        const string adminPassword = "Admin@123";

        var adminUser = await _userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FirstName = "System",
                LastName = "Administrator",
                EmailConfirmed = true,
                IsActive = true,
                IsSystem = true
            };

            var result = await _userManager.CreateAsync(adminUser, adminPassword);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(adminUser, "Admin");
                _logger.LogInformation("Created default admin user: {Email}", adminEmail);
                _logger.LogWarning("IMPORTANT: Change the default admin password after first login!");
            }
            else
            {
                _logger.LogError("Failed to create admin user");
            }
        }
    }

    private async Task SeedBasicUserAsync()
    {
        const string userEmail = "user@workbeast.com";
        const string userPassword = "User@123";

        var basicUser = await _userManager.FindByEmailAsync(userEmail);
        if (basicUser == null)
        {
            basicUser = new ApplicationUser
            {
                UserName = userEmail,
                Email = userEmail,
                FirstName = "Basic",
                LastName = "User",
                EmailConfirmed = true,
                IsActive = true,
                IsSystem = true
            };

            var result = await _userManager.CreateAsync(basicUser, userPassword);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(basicUser, "User");
                _logger.LogInformation("Created default basic user: {Email}", userEmail);
                _logger.LogWarning("IMPORTANT: Change the default basic user password after first login!");
            }
            else
            {
                _logger.LogError("Failed to create basic user");
            }
        }
    }
}