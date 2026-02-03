using WorkBeast.Core.Models;

namespace WorkBeast.Core.Services;

/// <summary>
/// Interface for configuration management
/// </summary>
public interface IConfigurationService
{
    /// <summary>
    /// Initializes the configuration file with default values
    /// </summary>
    Task InitializeAsync();

    /// <summary>
    /// Loads configuration from file
    /// </summary>
    Task<AppConfiguration> LoadAsync();

    /// <summary>
    /// Saves configuration to file
    /// </summary>
    Task SaveAsync(AppConfiguration configuration);

    /// <summary>
    /// Gets a specific configuration value
    /// </summary>
    Task<T?> GetValueAsync<T>(string key);

    /// <summary>
    /// Sets a specific configuration value
    /// </summary>
    Task SetValueAsync<T>(string key, T value);

    /// <summary>
    /// Gets the configuration file path
    /// </summary>
    string GetConfigurationFilePath();

    /// <summary>
    /// Checks if configuration file exists
    /// </summary>
    bool ConfigurationExists();
}
