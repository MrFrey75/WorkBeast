namespace WorkBeast.Core.Services;

/// <summary>
/// Interface for initializing the application environment
/// </summary>
public interface IApplicationInitializer
{
    /// <summary>
    /// Initializes the application by creating necessary directories and files
    /// </summary>
    /// <returns>Task representing the async operation</returns>
    Task InitializeAsync();

    /// <summary>
    /// Gets the application data directory path
    /// </summary>
    string GetApplicationDataPath();

    /// <summary>
    /// Gets the logs directory path
    /// </summary>
    string GetLogsPath();

    /// <summary>
    /// Gets the backups directory path
    /// </summary>
    string GetBackupsPath();
}
