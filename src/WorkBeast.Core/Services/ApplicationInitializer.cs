using Microsoft.Extensions.Logging;

namespace WorkBeast.Core.Services;

/// <summary>
/// Service for initializing the application environment
/// </summary>
public class ApplicationInitializer : IApplicationInitializer
{
    private readonly ILogger<ApplicationInitializer> _logger;
    private readonly string _appDataPath;
    private readonly string _logsPath;
    private readonly string _backupsPath;
    private readonly string _exportsPath;
    private readonly string _importsPath;

    public ApplicationInitializer(ILogger<ApplicationInitializer> logger)
    {
        _logger = logger;

        // Determine base application data path based on OS
        string baseDataPath;
        if (OperatingSystem.IsWindows())
        {
            baseDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        }
        else if (OperatingSystem.IsLinux() || OperatingSystem.IsMacOS())
        {
            var home = Environment.GetEnvironmentVariable("HOME") 
                ?? Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            baseDataPath = Path.Combine(home, ".local", "share");
        }
        else
        {
            baseDataPath = Environment.CurrentDirectory;
        }

        _appDataPath = Path.Combine(baseDataPath, "WorkBeast");
        _logsPath = Path.Combine(_appDataPath, "Logs");
        _backupsPath = Path.Combine(_appDataPath, "Backups");
        _exportsPath = Path.Combine(_appDataPath, "Exports");
        _importsPath = Path.Combine(_appDataPath, "Imports");
    }

    /// <summary>
    /// Initializes the application by creating necessary directories and files
    /// </summary>
    public async Task InitializeAsync()
    {
        _logger.LogInformation("Initializing WorkBeast application environment...");

        try
        {
            // Create main application directory
            CreateDirectoryIfNotExists(_appDataPath, "Application data");

            // Create subdirectories
            CreateDirectoryIfNotExists(_logsPath, "Logs");
            CreateDirectoryIfNotExists(_backupsPath, "Database backups");
            CreateDirectoryIfNotExists(_exportsPath, "Data exports");
            CreateDirectoryIfNotExists(_importsPath, "Data imports");

            // Create a README file in the app data directory
            await CreateReadMeFileAsync();

            _logger.LogInformation("Application environment initialized successfully at: {AppDataPath}", _appDataPath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize application environment");
            throw;
        }
    }

    /// <summary>
    /// Gets the application data directory path
    /// </summary>
    public string GetApplicationDataPath() => _appDataPath;

    /// <summary>
    /// Gets the logs directory path
    /// </summary>
    public string GetLogsPath() => _logsPath;

    /// <summary>
    /// Gets the backups directory path
    /// </summary>
    public string GetBackupsPath() => _backupsPath;

    /// <summary>
    /// Gets the exports directory path
    /// </summary>
    public string GetExportsPath() => _exportsPath;

    /// <summary>
    /// Gets the imports directory path
    /// </summary>
    public string GetImportsPath() => _importsPath;

    private void CreateDirectoryIfNotExists(string path, string description)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
            _logger.LogInformation("Created directory for {Description}: {Path}", description, path);
        }
        else
        {
            _logger.LogDebug("Directory already exists for {Description}: {Path}", description, path);
        }
    }

    private async Task CreateReadMeFileAsync()
    {
        var readmePath = Path.Combine(_appDataPath, "README.txt");
        
        if (File.Exists(readmePath))
        {
            return;
        }

        var readmeContent = $@"WorkBeast Application Data Directory
=====================================

This directory contains data and configuration files for WorkBeast.
Created: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC

Directory Structure:
--------------------
- Logs/                    : Application log files
- Backups/                 : Automatic database backups
- Exports/                 : Exported workout data (JSON, CSV, etc.)
- Imports/                 : Place files here to import workout data
- config.encrypted.json    : Encrypted application configuration
- .encryption.key          : Encryption key (DO NOT DELETE OR SHARE)
- workbeast.db             : SQLite database

IMPORTANT:
----------
- Do not manually modify encrypted configuration files
- NEVER share or delete the .encryption.key file
- Database backups are stored in the Backups/ folder
- Log files older than 30 days are automatically deleted
- You can safely delete the Exports/ folder contents
- Use the API to manage configuration settings

Security:
---------
- Configuration is encrypted using AES-256
- API keys and sensitive data are protected
- Keep .encryption.key file secure and backed up
- If .encryption.key is lost, encrypted configuration cannot be recovered

For more information, visit: https://github.com/MrFrey75/WorkBeast
";

        await File.WriteAllTextAsync(readmePath, readmeContent);
        _logger.LogInformation("Created README file: {ReadmePath}", readmePath);
    }
}
