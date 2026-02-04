using Spectre.Console;
using WorkBeast.Core.Services;

namespace WorkBeast.AdminConsole.Commands;

public class ConfigurationCommands
{
    private readonly IConfigurationService _configService;

    public ConfigurationCommands(IConfigurationService configService)
    {
        _configService = configService;
    }

    public async Task ExecuteAsync()
    {
        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[yellow]Configuration Management:[/]")
                .AddChoices(new[]
                {
                    "View Current Configuration",
                    "View Configuration File Path",
                    "Reset Configuration to Defaults",
                    "Update Application Name",
                    "Update Application Environment",
                    "Toggle Auto Backup",
                    "Back to Main Menu"
                }));

        switch (choice)
        {
            case "View Current Configuration":
                await ViewConfiguration();
                break;
            case "View Configuration File Path":
                ViewConfigurationPath();
                break;
            case "Reset Configuration to Defaults":
                await ResetConfiguration();
                break;
            case "Update Application Name":
                await UpdateApplicationName();
                break;
            case "Update Application Environment":
                await UpdateEnvironment();
                break;
            case "Toggle Auto Backup":
                await ToggleAutoBackup();
                break;
        }
    }

    private async Task ViewConfiguration()
    {
        await AnsiConsole.Status()
            .StartAsync("Loading configuration...", async ctx =>
            {
                try
                {
                    var config = await _configService.LoadAsync();

                    var tree = new Tree("[yellow]Application Configuration[/]");

                    var appNode = tree.AddNode("[blue]Application[/]");
                    appNode.AddNode($"Name: {config.Application.Name}");
                    appNode.AddNode($"Version: {config.Application.Version}");
                    appNode.AddNode($"Theme: {config.Application.Theme}");
                    appNode.AddNode($"Language: {config.Application.Language}");
                    appNode.AddNode($"Environment: {config.Application.Environment}");

                    var dbNode = tree.AddNode("[blue]Database[/]");
                    dbNode.AddNode($"Auto Backup: {config.Database.AutoBackup}");
                    dbNode.AddNode($"Backup Interval (Days): {config.Database.BackupIntervalDays}");
                    dbNode.AddNode($"Max Backup Count: {config.Database.MaxBackupCount}");
                    dbNode.AddNode($"Auto Migrate: {config.Database.AutoMigrate}");

                    var logNode = tree.AddNode("[blue]Logging[/]");
                    logNode.AddNode($"Enabled: {config.Logging.Enabled}");
                    logNode.AddNode($"Level: {config.Logging.Level}");
                    logNode.AddNode($"Retention (Days): {config.Logging.RetentionDays}");
                    logNode.AddNode($"Log to File: {config.Logging.LogToFile}");
                    logNode.AddNode($"Log to Console: {config.Logging.LogToConsole}");

                    var secNode = tree.AddNode("[blue]Security[/]");
                    secNode.AddNode($"JWT Expiration (Days): {config.Security.JwtExpirationDays}");
                    secNode.AddNode($"Password Min Length: {config.Security.PasswordMinLength}");
                    secNode.AddNode($"Require Digit: {config.Security.RequireDigit}");
                    secNode.AddNode($"Require Uppercase: {config.Security.RequireUppercase}");
                    secNode.AddNode($"Require Lowercase: {config.Security.RequireLowercase}");
                    secNode.AddNode($"Require Non-Alphanumeric: {config.Security.RequireNonAlphanumeric}");
                    secNode.AddNode($"Max Failed Access Attempts: {config.Security.MaxFailedAccessAttempts}");
                    secNode.AddNode($"Lockout (Minutes): {config.Security.LockoutMinutes}");

                    AnsiConsole.Write(tree);
                }
                catch (Exception ex)
                {
                    AnsiConsole.MarkupLine($"[red]Error loading configuration: {ex.Message}[/]");
                }
            });
    }

    private void ViewConfigurationPath()
    {
        var path = _configService.GetConfigurationFilePath();
        var exists = _configService.ConfigurationExists();

        var panel = new Panel($"[yellow]Configuration File Path:[/]\n{path}\n\n[grey]Exists: {(exists ? "[green]Yes[/]" : "[red]No[/]")}[/]")
        {
            Border = BoxBorder.Rounded,
            Padding = new Padding(2, 1)
        };

        AnsiConsole.Write(panel);
    }

    private async Task ResetConfiguration()
    {
        var confirm = AnsiConsole.Confirm("Reset configuration to default values?");
        if (!confirm) return;

        await AnsiConsole.Status()
            .StartAsync("Resetting configuration...", async ctx =>
            {
                try
                {
                    var configPath = _configService.GetConfigurationFilePath();
                    if (File.Exists(configPath))
                    {
                        File.Delete(configPath);
                    }

                    await _configService.InitializeAsync();
                    AnsiConsole.MarkupLine("[green]Configuration reset to defaults![/]");
                }
                catch (Exception ex)
                {
                    AnsiConsole.MarkupLine($"[red]Error resetting configuration: {ex.Message}[/]");
                }
            });
    }

    private async Task UpdateApplicationName()
    {
        var newName = AnsiConsole.Ask<string>("Enter new application name:");

        await AnsiConsole.Status()
            .StartAsync("Updating configuration...", async ctx =>
            {
                try
                {
                    await _configService.SetValueAsync("Application.Name", newName);
                    AnsiConsole.MarkupLine("[green]Application name updated successfully![/]");
                }
                catch (Exception ex)
                {
                    AnsiConsole.MarkupLine($"[red]Error updating configuration: {ex.Message}[/]");
                }
            });
    }

    private async Task UpdateEnvironment()
    {
        var environment = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Select environment:")
                .AddChoices(new[] { "Development", "Staging", "Production" }));

        await AnsiConsole.Status()
            .StartAsync("Updating configuration...", async ctx =>
            {
                try
                {
                    await _configService.SetValueAsync("Application.Environment", environment);
                    AnsiConsole.MarkupLine($"[green]Environment updated to {environment}![/]");
                }
                catch (Exception ex)
                {
                    AnsiConsole.MarkupLine($"[red]Error updating configuration: {ex.Message}[/]");
                }
            });
    }

    private async Task ToggleAutoBackup()
    {
        await AnsiConsole.Status()
            .StartAsync("Toggling auto backup...", async ctx =>
            {
                try
                {
                    var currentValue = await _configService.GetValueAsync<bool>("Database.AutoBackup");
                    await _configService.SetValueAsync("Database.AutoBackup", !currentValue);
                    AnsiConsole.MarkupLine($"[green]Auto backup {(!currentValue ? "enabled" : "disabled")}![/]");
                }
                catch (Exception ex)
                {
                    AnsiConsole.MarkupLine($"[red]Error toggling auto backup: {ex.Message}[/]");
                }
            });
    }

    // Non-interactive mode support for testing
    public async Task ExecuteDirectAsync(string command)
    {
        switch (command)
        {
            case "View Current Configuration":
                await ViewConfiguration();
                break;
            default:
                AnsiConsole.MarkupLine($"[red]Unknown command: {command}[/]");
}
