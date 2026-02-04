using Microsoft.EntityFrameworkCore;
using Spectre.Console;
using WorkBeast.Core.Services;
using WorkBeast.Data.Context;
using WorkBeast.Data.Services;

namespace WorkBeast.AdminConsole.Commands;

public class DatabaseCommands
{
    private readonly AppDbContext _context;
    private readonly IDataSeeder _dataSeeder;
    private readonly RoleSeeder _roleSeeder;

    public DatabaseCommands(AppDbContext context, IDataSeeder dataSeeder, RoleSeeder roleSeeder)
    {
        _context = context;
        _dataSeeder = dataSeeder;
        _roleSeeder = roleSeeder;
    }

    public async Task ExecuteAsync()
    {
        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[yellow]Database Management:[/]")
                .AddChoices(new[]
                {
                    "View Database Statistics",
                    "Apply Pending Migrations",
                    "Seed System Data",
                    "Seed Roles and Admin User",
                    "Backup Database",
                    "Verify Database Integrity",
                    "Clear All Data (Dangerous)",
                    "Back to Main Menu"
                }));

        switch (choice)
        {
            case "View Database Statistics":
                await ViewDatabaseStatistics();
                break;
            case "Apply Pending Migrations":
                await ApplyMigrations();
                break;
            case "Seed System Data":
                await SeedSystemData();
                break;
            case "Seed Roles and Admin User":
                await SeedRolesAndAdmin();
                break;
            case "Backup Database":
                await BackupDatabase();
                break;
            case "Verify Database Integrity":
                await VerifyDatabaseIntegrity();
                break;
            case "Clear All Data (Dangerous)":
                await ClearAllData();
                break;
        }
    }

    private async Task ViewDatabaseStatistics()
    {
        await AnsiConsole.Status()
            .StartAsync("Loading statistics...", async ctx =>
            {
                var exerciseCount = await _context.Exercises.CountAsync();
                var bodyPartCount = await _context.BodyParts.CountAsync();
                var workoutCount = await _context.WorkoutSessions.CountAsync();
                var userCount = await _context.Users.CountAsync();
                var roleCount = await _context.Roles.CountAsync();

                var table = new Table();
                table.AddColumn("Entity");
                table.AddColumn("Count");

                table.AddRow("Exercises", exerciseCount.ToString());
                table.AddRow("Body Parts", bodyPartCount.ToString());
                table.AddRow("Workout Sessions", workoutCount.ToString());
                table.AddRow("Users", userCount.ToString());
                table.AddRow("Roles", roleCount.ToString());

                AnsiConsole.Write(table);
            });
    }

    private async Task ApplyMigrations()
    {
        var confirm = AnsiConsole.Confirm("Apply pending migrations to the database?");
        if (!confirm) return;

        await AnsiConsole.Status()
            .StartAsync("Applying migrations...", async ctx =>
            {
                try
                {
                    await _context.Database.MigrateAsync();
                    AnsiConsole.MarkupLine("[green]Migrations applied successfully![/]");
                }
                catch (Exception ex)
                {
                    AnsiConsole.MarkupLine($"[red]Error applying migrations: {ex.Message}[/]");
                }
            });
    }

    private async Task SeedSystemData()
    {
        var confirm = AnsiConsole.Confirm("Seed system data (exercises and body parts)?");
        if (!confirm) return;

        await AnsiConsole.Status()
            .StartAsync("Seeding system data...", async ctx =>
            {
                try
                {
                    await _dataSeeder.SeedAsync();
                    AnsiConsole.MarkupLine("[green]System data seeded successfully![/]");
                }
                catch (Exception ex)
                {
                    AnsiConsole.MarkupLine($"[red]Error seeding data: {ex.Message}[/]");
                }
            });
    }

    private async Task SeedRolesAndAdmin()
    {
        var confirm = AnsiConsole.Confirm("Seed roles and create admin user?");
        if (!confirm) return;

        await AnsiConsole.Status()
            .StartAsync("Seeding roles and admin user...", async ctx =>
            {
                try
                {
                    await _roleSeeder.SeedAsync();
                    AnsiConsole.MarkupLine("[green]Roles and admin user created successfully![/]");
                    AnsiConsole.MarkupLine("[yellow]Default admin credentials:[/]");
                    AnsiConsole.MarkupLine("  Email: admin@workbeast.com");
                    AnsiConsole.MarkupLine("  Password: Admin123!");
                }
                catch (Exception ex)
                {
                    AnsiConsole.MarkupLine($"[red]Error seeding roles: {ex.Message}[/]");
                }
            });
    }

    private async Task BackupDatabase()
    {
        var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        var backupPath = $"workbeast_backup_{timestamp}.db";

        var confirm = AnsiConsole.Confirm($"Create database backup at '{backupPath}'?");
        if (!confirm) return;

        await AnsiConsole.Status()
            .StartAsync("Creating backup...", async ctx =>
            {
                try
                {
                    var sourcePath = _context.Database.GetConnectionString()?.Replace("Data Source=", "");
                    if (!string.IsNullOrEmpty(sourcePath) && File.Exists(sourcePath))
                    {
                        await Task.Run(() => File.Copy(sourcePath, backupPath, true));
                        AnsiConsole.MarkupLine($"[green]Backup created successfully at: {backupPath}[/]");
                    }
                    else
                    {
                        AnsiConsole.MarkupLine("[red]Could not locate database file![/]");
                    }
                }
                catch (Exception ex)
                {
                    AnsiConsole.MarkupLine($"[red]Error creating backup: {ex.Message}[/]");
                }
            });
    }

    private async Task VerifyDatabaseIntegrity()
    {
        await AnsiConsole.Status()
            .StartAsync("Verifying database integrity...", async ctx =>
            {
                try
                {
                    await _context.Database.CanConnectAsync();
                    var pendingMigrations = await _context.Database.GetPendingMigrationsAsync();
                    
                    if (pendingMigrations.Any())
                    {
                        AnsiConsole.MarkupLine($"[yellow]Warning: {pendingMigrations.Count()} pending migrations found[/]");
                        foreach (var migration in pendingMigrations)
                        {
                            AnsiConsole.MarkupLine($"  - {migration}");
                        }
                    }
                    else
                    {
                        AnsiConsole.MarkupLine("[green]Database is up to date![/]");
                    }
                }
                catch (Exception ex)
                {
                    AnsiConsole.MarkupLine($"[red]Database integrity check failed: {ex.Message}[/]");
                }
            });
    }

    private async Task ClearAllData()
    {
        AnsiConsole.MarkupLine("[red]WARNING: This will delete ALL data from the database![/]");
        var confirm1 = AnsiConsole.Confirm("Are you absolutely sure?");
        if (!confirm1) return;

        var confirm2 = AnsiConsole.Confirm("[red]This action cannot be undone. Continue?[/]");
        if (!confirm2) return;

        await AnsiConsole.Status()
            .StartAsync("Clearing all data...", async ctx =>
            {
                try
                {
                    await _context.Database.EnsureDeletedAsync();
                    await _context.Database.EnsureCreatedAsync();
                    await _context.Database.MigrateAsync();
                    AnsiConsole.MarkupLine("[green]All data cleared and database recreated![/]");
                }
                catch (Exception ex)
                {
                    AnsiConsole.MarkupLine($"[red]Error clearing data: {ex.Message}[/]");
                }
            });
    }

    // Non-interactive mode support for testing
    public async Task ExecuteDirectAsync(string command)
    {
        switch (command)
        {
            case "View Database Statistics":
                await ViewDatabaseStatistics();
                break;
            case "Apply Pending Migrations":
                await ApplyMigrations();
                break;
            case "Seed System Data":
                await SeedSystemData();
                break;
            case "Seed Roles and Admin User":
                await SeedRolesAndAdmin();
                break;
            case "Backup Database":
                await BackupDatabase();
                break;
            case "Verify Database Integrity":
                await VerifyDatabaseIntegrity();
                break;
            default:
                AnsiConsole.MarkupLine($"[red]Unknown command: {command}[/]");
                break;
        }
    }
}

```
