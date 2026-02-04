using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Spectre.Console;
using WorkBeast.AdminConsole.Commands;
using WorkBeast.Core.Models;
using WorkBeast.Core.Services;
using WorkBeast.Data;
using WorkBeast.Data.Context;
using WorkBeast.Data.Services;

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/admin-console-.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

try
{
    var builder = Host.CreateApplicationBuilder(args);

    // Add configuration
    builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

    // Add logging
    builder.Services.AddSerilog();

    // Configure database
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    if (string.IsNullOrEmpty(connectionString) || connectionString.Contains("workbeast.db"))
    {
        var dbPath = PathHelper.GetDatabasePath();
        connectionString = $"Data Source={dbPath}";
    }

    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlite(connectionString));

    // Configure Identity stores
    builder.Services.AddIdentityCore<ApplicationUser>(options =>
    {
        // Password settings
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = true;
        options.Password.RequiredLength = 6;

        // User settings
        options.User.RequireUniqueEmail = true;

        // Lockout settings
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
        options.Lockout.MaxFailedAccessAttempts = 5;
    })
    .AddRoles<ApplicationRole>()
    .AddEntityFrameworkStores<AppDbContext>();

    // Add RoleManager and UserManager
    builder.Services.AddScoped<UserManager<ApplicationUser>>();
    builder.Services.AddScoped<RoleManager<ApplicationRole>>();

    // Add RoleSeeder
    builder.Services.AddScoped<RoleSeeder>();

    // Add WorkBeast services
    builder.Services.AddWorkBeastCore();
    builder.Services.AddWorkBeastData();

    // Register admin commands
    builder.Services.AddTransient<DatabaseCommands>();
    builder.Services.AddTransient<UserCommands>();
    builder.Services.AddTransient<DataCommands>();
    builder.Services.AddTransient<ConfigurationCommands>();

    var host = builder.Build();

    // Display banner
    AnsiConsole.Write(
        new FigletText("WorkBeast Admin")
            .LeftJustified()
            .Color(Color.Green));

    AnsiConsole.MarkupLine("[grey]Administrative Console for WorkBeast[/]\n");

    // Support non-interactive mode for testing
    if (args.Length > 0)
    {
        // Non-interactive test mode
        var command = args[0].ToLower();
        switch (command)
        {
            case "db-stats":
                await ExecuteDatabaseCommandsDirect(host.Services, "View Database Statistics");
                break;
            case "db-migrate":
                await ExecuteDatabaseCommandsDirect(host.Services, "Apply Pending Migrations");
                break;
            case "db-seed-system":
                await ExecuteDatabaseCommandsDirect(host.Services, "Seed System Data");
                break;
            case "db-seed-roles":
                await ExecuteDatabaseCommandsDirect(host.Services, "Seed Roles and Admin User");
                break;
            case "db-backup":
                await ExecuteDatabaseCommandsDirect(host.Services, "Backup Database");
                break;
            case "db-verify":
                await ExecuteDatabaseCommandsDirect(host.Services, "Verify Database Integrity");
                break;
            case "users-list":
                await ExecuteUserCommandsDirect(host.Services, "List All Users");
                break;
            case "data-exercises":
                await ExecuteDataCommandsDirect(host.Services, "List Exercises");
                break;
            case "data-bodyparts":
                await ExecuteDataCommandsDirect(host.Services, "List Body Parts");
                break;
            case "data-workouts":
                await ExecuteDataCommandsDirect(host.Services, "View Workout Sessions");
                break;
            case "config-view":
                await ExecuteConfigCommandsDirect(host.Services, "View Current Configuration");
                break;
            case "sysinfo":
                DisplaySystemInformation(host.Services);
                break;
            case "help":
                DisplayHelp();
                break;
            default:
                AnsiConsole.MarkupLine($"[red]Unknown command: {command}[/]");
                DisplayHelp();
                return 1;
        }
        return 0;
    }

    // Main menu loop - Interactive mode
    var exit = false;
    while (!exit)
    {
        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[green]Select an administrative task:[/]")
                .PageSize(10)
                .AddChoices(new[]
                {
                    "Database Management",
                    "User Management",
                    "Data Management",
                    "Configuration Management",
                    "System Information",
                    "Exit"
                }));

        switch (choice)
        {
            case "Database Management":
                await ExecuteDatabaseCommands(host.Services);
                break;
            case "User Management":
                await ExecuteUserCommands(host.Services);
                break;
            case "Data Management":
                await ExecuteDataCommands(host.Services);
                break;
            case "Configuration Management":
                await ExecuteConfigurationCommands(host.Services);
                break;
            case "System Information":
                DisplaySystemInformation(host.Services);
                break;
            case "Exit":
                exit = true;
                break;
        }

        if (!exit)
        {
            AnsiConsole.MarkupLine("\n[grey]Press any key to continue...[/]");
            Console.ReadKey(true);
            AnsiConsole.Clear();
        }
    }

    AnsiConsole.MarkupLine("\n[green]Goodbye![/]");
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
    AnsiConsole.WriteException(ex);
    return 1;
}
finally
{
    Log.CloseAndFlush();
}

return 0;

static async Task ExecuteDatabaseCommands(IServiceProvider services)
{
    var commands = services.GetRequiredService<DatabaseCommands>();
    await commands.ExecuteAsync();
}

static async Task ExecuteUserCommands(IServiceProvider services)
{
    var commands = services.GetRequiredService<UserCommands>();
    await commands.ExecuteAsync();
}

static async Task ExecuteDataCommands(IServiceProvider services)
{
    var commands = services.GetRequiredService<DataCommands>();
    await commands.ExecuteAsync();
}

static async Task ExecuteConfigurationCommands(IServiceProvider services)
{
    var commands = services.GetRequiredService<ConfigurationCommands>();
    await commands.ExecuteAsync();
}

static void DisplaySystemInformation(IServiceProvider services)
{
    var table = new Table();
    table.AddColumn("Property");
    table.AddColumn("Value");

    table.AddRow("OS", Environment.OSVersion.ToString());
    table.AddRow(".NET Version", Environment.Version.ToString());
    table.AddRow("Database Path", PathHelper.GetDatabasePath());
    table.AddRow("App Data Path", PathHelper.GetAppDataPath());
    table.AddRow("Machine Name", Environment.MachineName);
    table.AddRow("User Name", Environment.UserName);

    AnsiConsole.Write(table);
}

static void DisplayHelp()
{
    AnsiConsole.MarkupLine("[yellow]Available Commands:[/]");
    AnsiConsole.MarkupLine("  [green]Database Commands:[/]");
    AnsiConsole.MarkupLine("    db-stats              - View database statistics");
    AnsiConsole.MarkupLine("    db-migrate            - Apply pending migrations");
    AnsiConsole.MarkupLine("    db-seed-system        - Seed system data");
    AnsiConsole.MarkupLine("    db-seed-roles         - Seed roles and admin user");
    AnsiConsole.MarkupLine("    db-backup             - Backup database");
    AnsiConsole.MarkupLine("    db-verify             - Verify database integrity");
    AnsiConsole.MarkupLine("  [green]User Commands:[/]");
    AnsiConsole.MarkupLine("    users-list            - List all users");
    AnsiConsole.MarkupLine("  [green]Data Commands:[/]");
    AnsiConsole.MarkupLine("    data-exercises        - List exercises");
    AnsiConsole.MarkupLine("    data-bodyparts        - List body parts");
    AnsiConsole.MarkupLine("    data-workouts         - View workout sessions");
    AnsiConsole.MarkupLine("  [green]Configuration Commands:[/]");
    AnsiConsole.MarkupLine("    config-view           - View configuration");
    AnsiConsole.MarkupLine("  [green]System Commands:[/]");
    AnsiConsole.MarkupLine("    sysinfo               - Display system information");
    AnsiConsole.MarkupLine("    help                  - Display this help message");
}

static async Task ExecuteDatabaseCommandsDirect(IServiceProvider services, string subCommand)
{
    var commands = services.GetRequiredService<DatabaseCommands>();
    await commands.ExecuteDirectAsync(subCommand);
}

static async Task ExecuteUserCommandsDirect(IServiceProvider services, string subCommand)
{
    var commands = services.GetRequiredService<UserCommands>();
    await commands.ExecuteDirectAsync(subCommand);
}

static async Task ExecuteDataCommandsDirect(IServiceProvider services, string subCommand)
{
    var commands = services.GetRequiredService<DataCommands>();
    await commands.ExecuteDirectAsync(subCommand);
}

static async Task ExecuteConfigCommandsDirect(IServiceProvider services, string subCommand)
{
    var commands = services.GetRequiredService<ConfigurationCommands>();
    await commands.ExecuteDirectAsync(subCommand);
}
