using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Spectre.Console;
using WorkBeast.Core.Models;
using WorkBeast.Core.Services;

namespace WorkBeast.AdminConsole.Commands;

public class UserCommands
{
    private readonly IAuthService _authService;
    private readonly UserManager<ApplicationUser> _userManager;

    public UserCommands(IAuthService authService, UserManager<ApplicationUser> userManager)
    {
        _authService = authService;
        _userManager = userManager;
    }

    public async Task ExecuteAsync()
    {
        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[yellow]User Management:[/]")
                .AddChoices(new[]
                {
                    "List All Users",
                    "View User Details",
                    "Create New User",
                    "Assign Role to User",
                    "Remove Role from User",
                    "Activate User",
                    "Deactivate User",
                    "Reset User Password",
                    "Delete User",
                    "Back to Main Menu"
                }));

        switch (choice)
        {
            case "List All Users":
                await ListAllUsers();
                break;
            case "View User Details":
                await ViewUserDetails();
                break;
            case "Create New User":
                await CreateNewUser();
                break;
            case "Assign Role to User":
                await AssignRole();
                break;
            case "Remove Role from User":
                await RemoveRole();
                break;
            case "Activate User":
                await ActivateUser();
                break;
            case "Deactivate User":
                await DeactivateUser();
                break;
            case "Reset User Password":
                await ResetPassword();
                break;
            case "Delete User":
                await DeleteUser();
                break;
        }
    }

    private async Task ListAllUsers()
    {
        await AnsiConsole.Status()
            .StartAsync("Loading users...", async ctx =>
            {
                var users = await _authService.GetAllUsersAsync();

                var table = new Table();
                table.AddColumn("ID");
                table.AddColumn("Email");
                table.AddColumn("Name");
                table.AddColumn("Active");
                table.AddColumn("Roles");
                table.AddColumn("Last Login");

                foreach (var user in users)
                {
                    table.AddRow(
                        user.Id.ToString(),
                        user.Email,
                        user.FullName,
                        user.IsActive ? "[green]Yes[/]" : "[red]No[/]",
                        string.Join(", ", user.Roles),
                        user.LastLoginAt?.ToString("yyyy-MM-dd HH:mm") ?? "Never"
                    );
                }

                AnsiConsole.Write(table);
            });
    }

    private async Task ViewUserDetails()
    {
        var userId = AnsiConsole.Ask<int>("Enter user ID:");

        await AnsiConsole.Status()
            .StartAsync("Loading user details...", async ctx =>
            {
                var user = await _authService.GetUserByIdAsync(userId);
                if (user == null)
                {
                    AnsiConsole.MarkupLine("[red]User not found![/]");
                    return;
                }

                var table = new Table();
                table.AddColumn("Property");
                table.AddColumn("Value");

                table.AddRow("ID", user.Id.ToString());
                table.AddRow("Email", user.Email);
                table.AddRow("First Name", user.FirstName);
                table.AddRow("Last Name", user.LastName);
                table.AddRow("Full Name", user.FullName);
                table.AddRow("Active", user.IsActive ? "[green]Yes[/]" : "[red]No[/]");
                table.AddRow("Created At", user.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"));
                table.AddRow("Last Login", user.LastLoginAt?.ToString("yyyy-MM-dd HH:mm:ss") ?? "Never");
                table.AddRow("Roles", string.Join(", ", user.Roles));

                AnsiConsole.Write(table);
            });
    }

    private async Task CreateNewUser()
    {
        var email = AnsiConsole.Ask<string>("Enter email:");
        var firstName = AnsiConsole.Ask<string>("Enter first name:");
        var lastName = AnsiConsole.Ask<string>("Enter last name:");
        var password = AnsiConsole.Prompt(
            new TextPrompt<string>("Enter password:")
                .PromptStyle("red")
                .Secret());

        await AnsiConsole.Status()
            .StartAsync("Creating user...", async ctx =>
            {
                var user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    FirstName = firstName,
                    LastName = lastName,
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "User");
                    AnsiConsole.MarkupLine($"[green]User created successfully! ID: {user.Id}[/]");
                }
                else
                {
                    AnsiConsole.MarkupLine("[red]Failed to create user:[/]");
                    foreach (var error in result.Errors)
                    {
                        AnsiConsole.MarkupLine($"  - {error.Description}");
                    }
                }
            });
    }

    private async Task AssignRole()
    {
        var userId = AnsiConsole.Ask<int>("Enter user ID:");
        var role = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Select role to assign:")
                .AddChoices(new[] { "Admin", "Trainer", "User" }));

        var success = await _authService.AssignRoleAsync(userId, role);
        if (success)
        {
            AnsiConsole.MarkupLine($"[green]Role '{role}' assigned successfully![/]");
        }
        else
        {
            AnsiConsole.MarkupLine("[red]Failed to assign role![/]");
        }
    }

    private async Task RemoveRole()
    {
        var userId = AnsiConsole.Ask<int>("Enter user ID:");
        var role = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Select role to remove:")
                .AddChoices(new[] { "Admin", "Trainer", "User" }));

        var success = await _authService.RemoveRoleAsync(userId, role);
        if (success)
        {
            AnsiConsole.MarkupLine($"[green]Role '{role}' removed successfully![/]");
        }
        else
        {
            AnsiConsole.MarkupLine("[red]Failed to remove role![/]");
        }
    }

    private async Task ActivateUser()
    {
        var userId = AnsiConsole.Ask<int>("Enter user ID:");
        var success = await _authService.ActivateUserAsync(userId);
        if (success)
        {
            AnsiConsole.MarkupLine("[green]User activated successfully![/]");
        }
        else
        {
            AnsiConsole.MarkupLine("[red]Failed to activate user![/]");
        }
    }

    private async Task DeactivateUser()
    {
        var userId = AnsiConsole.Ask<int>("Enter user ID:");
        var success = await _authService.DeactivateUserAsync(userId);
        if (success)
        {
            AnsiConsole.MarkupLine("[green]User deactivated successfully![/]");
        }
        else
        {
            AnsiConsole.MarkupLine("[red]Failed to deactivate user![/]");
        }
    }

    private async Task ResetPassword()
    {
        var userId = AnsiConsole.Ask<int>("Enter user ID:");
        var newPassword = AnsiConsole.Prompt(
            new TextPrompt<string>("Enter new password:")
                .PromptStyle("red")
                .Secret());

        await AnsiConsole.Status()
            .StartAsync("Resetting password...", async ctx =>
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());
                if (user == null)
                {
                    AnsiConsole.MarkupLine("[red]User not found![/]");
                    return;
                }

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

                if (result.Succeeded)
                {
                    AnsiConsole.MarkupLine("[green]Password reset successfully![/]");
                }
                else
                {
                    AnsiConsole.MarkupLine("[red]Failed to reset password:[/]");
                    foreach (var error in result.Errors)
                    {
                        AnsiConsole.MarkupLine($"  - {error.Description}");
                    }
                }
            });
    }

    private async Task DeleteUser()
    {
        var userId = AnsiConsole.Ask<int>("Enter user ID:");

        var confirm = AnsiConsole.Confirm($"Are you sure you want to delete user {userId}?");
        if (!confirm) return;

        await AnsiConsole.Status()
            .StartAsync("Deleting user...", async ctx =>
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());
                if (user == null)
                {
                    AnsiConsole.MarkupLine("[red]User not found![/]");
                    return;
                }

                if (user.IsSystem)
                {
                    AnsiConsole.MarkupLine("[red]System users cannot be deleted![/]");
                    return;
                }

                var result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    AnsiConsole.MarkupLine("[green]User deleted successfully![/]");
                }
                else
                {
                    AnsiConsole.MarkupLine("[red]Failed to delete user:[/]");
                    foreach (var error in result.Errors)
                    {
                        AnsiConsole.MarkupLine($"  - {error.Description}");
                    }
                }
            });
    }

    // Non-interactive mode support for testing
    public async Task ExecuteDirectAsync(string command)
    {
        switch (command)
        {
            case "List All Users":
                await ListAllUsers();
                break;
            default:
                AnsiConsole.MarkupLine($"[red]Unknown command: {command}[/]");
                break;
        }
    }
