using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using WorkBeast.Core.Models;

namespace WorkBeast.Core.Services;

/// <summary>
/// Service for managing application configuration stored as encrypted JSON
/// </summary>
public class ConfigurationService : IConfigurationService
{
    private readonly ILogger<ConfigurationService> _logger;
    private readonly string _configFilePath;
    private readonly string _encryptionKey;
    private AppConfiguration? _cachedConfig;

    public ConfigurationService(ILogger<ConfigurationService> logger)
    {
        _logger = logger;
        _configFilePath = GetConfigurationFilePath();
        _encryptionKey = GetOrCreateEncryptionKey();
    }

    /// <summary>
    /// Initializes the configuration file if it doesn't exist
    /// </summary>
    public async Task InitializeAsync()
    {
        if (ConfigurationExists())
        {
            _logger.LogInformation("Configuration file already exists at: {Path}", _configFilePath);
            return;
        }

        _logger.LogInformation("Initializing configuration file at: {Path}", _configFilePath);

        var defaultConfig = CreateDefaultConfiguration();
        await SaveAsync(defaultConfig);

        _logger.LogInformation("Configuration initialized successfully");
    }

    /// <summary>
    /// Loads the application configuration from encrypted file
    /// </summary>
    /// <returns>The application configuration</returns>
    public async Task<AppConfiguration> LoadAsync()
    {
        if (_cachedConfig != null)
        {
            return _cachedConfig;
        }

        if (!ConfigurationExists())
        {
            _logger.LogWarning("Configuration file not found, creating default");
            await InitializeAsync();
        }

        try
        {
            var encryptedJson = await File.ReadAllTextAsync(_configFilePath);
            var json = DecryptString(encryptedJson);
            
            _cachedConfig = JsonSerializer.Deserialize<AppConfiguration>(json) 
                ?? CreateDefaultConfiguration();

            _logger.LogDebug("Configuration loaded successfully");
            return _cachedConfig;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load configuration, using defaults");
            return CreateDefaultConfiguration();
        }
    }

    /// <summary>
    /// Saves the application configuration to encrypted file
    /// </summary>
    /// <param name="configuration">The configuration to save</param>
    public async Task SaveAsync(AppConfiguration configuration)
    {
        try
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var json = JsonSerializer.Serialize(configuration, options);
            var encryptedJson = EncryptString(json);

            var directory = Path.GetDirectoryName(_configFilePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            await File.WriteAllTextAsync(_configFilePath, encryptedJson);
            _cachedConfig = configuration;

            _logger.LogInformation("Configuration saved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save configuration");
            throw;
        }
    }

    /// <summary>
    /// Gets a specific configuration value by key path (e.g., "Application.Name")
    /// </summary>
    /// <typeparam name="T">The type of the value to retrieve</typeparam>
    /// <param name="key">The dot-separated key path</param>
    /// <returns>The configuration value, or default if not found</returns>
    public async Task<T?> GetValueAsync<T>(string key)
    {
        var config = await LoadAsync();
        var properties = key.Split('.');
        
        object? current = config;
        foreach (var prop in properties)
        {
            if (current == null) return default;
            
            var propInfo = current.GetType().GetProperty(prop);
            if (propInfo == null) return default;
            
            current = propInfo.GetValue(current);
        }

        if (current is T value)
        {
            return value;
        }

        return default;
    }

    /// <summary>
    /// Sets a specific configuration value by key path (e.g., "Application.Name")
    /// </summary>
    /// <typeparam name="T">The type of the value to set</typeparam>
    /// <param name="key">The dot-separated key path</param>
    /// <param name="value">The value to set</param>
    public async Task SetValueAsync<T>(string key, T value)
    {
        var config = await LoadAsync();
        var properties = key.Split('.');
        
        object current = config;
        for (int i = 0; i < properties.Length - 1; i++)
        {
            var propInfo = current.GetType().GetProperty(properties[i]);
            if (propInfo == null)
            {
                throw new ArgumentException($"Property '{properties[i]}' not found");
            }
            
            var nextValue = propInfo.GetValue(current);
            if (nextValue == null)
            {
                throw new ArgumentException($"Property '{properties[i]}' is null");
            }
            
            current = nextValue;
        }

        var finalProp = current.GetType().GetProperty(properties[^1]);
        if (finalProp == null)
        {
            throw new ArgumentException($"Property '{properties[^1]}' not found");
        }

        finalProp.SetValue(current, value);
        await SaveAsync(config);
    }

    /// <summary>
    /// Gets the full path to the configuration file
    /// </summary>
    /// <returns>The configuration file path</returns>
    public string GetConfigurationFilePath()
    {
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

        var appDataPath = Path.Combine(baseDataPath, "WorkBeast");
        return Path.Combine(appDataPath, "config.encrypted.json");
    }

    /// <summary>
    /// Checks if the configuration file exists
    /// </summary>
    /// <returns>True if configuration file exists, false otherwise</returns>
    public bool ConfigurationExists()
    {
        return File.Exists(_configFilePath);
    }

    private AppConfiguration CreateDefaultConfiguration()
    {
        return new AppConfiguration
        {
            Application = new ApplicationSettings
            {
                Name = "WorkBeast",
                Version = "1.0.0",
                Theme = "Light",
                Language = "en-US",
                Environment = "Development"
            },
            Database = new DatabaseSettings
            {
                AutoBackup = true,
                BackupIntervalDays = 7,
                MaxBackupCount = 10,
                AutoMigrate = true
            },
            Logging = new LoggingSettings
            {
                Enabled = true,
                Level = "Information",
                RetentionDays = 30,
                LogToFile = true,
                LogToConsole = true
            },
            Security = new SecuritySettings
            {
                JwtSecretKey = GenerateSecureKey(),
                JwtExpirationDays = 7,
                PasswordMinLength = 6,
                RequireDigit = true,
                RequireUppercase = true,
                RequireLowercase = true,
                RequireNonAlphanumeric = true,
                MaxFailedAccessAttempts = 5,
                LockoutMinutes = 15
            },
            ExternalApis = new ExternalApiSettings
            {
                CustomApiKeys = new Dictionary<string, string>()
            }
        };
    }

    private string GetOrCreateEncryptionKey()
    {
        var keyPath = Path.Combine(
            Path.GetDirectoryName(_configFilePath) ?? string.Empty,
            ".encryption.key"
        );

        if (File.Exists(keyPath))
        {
            return File.ReadAllText(keyPath);
        }

        var key = GenerateSecureKey();
        
        var directory = Path.GetDirectoryName(keyPath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        File.WriteAllText(keyPath, key);
        
        // Set file as hidden on Windows
        if (OperatingSystem.IsWindows())
        {
            File.SetAttributes(keyPath, FileAttributes.Hidden);
        }

        _logger.LogInformation("Created encryption key at: {Path}", keyPath);
        return key;
    }

    private string GenerateSecureKey()
    {
        var bytes = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(bytes);
        }
        return Convert.ToBase64String(bytes);
    }

    private string EncryptString(string plainText)
    {
        try
        {
            using var aes = Aes.Create();
            var keyBytes = Convert.FromBase64String(_encryptionKey);
            aes.Key = keyBytes;
            aes.GenerateIV();

            using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            using var msEncrypt = new MemoryStream();
            
            // Write IV first
            msEncrypt.Write(aes.IV, 0, aes.IV.Length);
            
            using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
            using (var swEncrypt = new StreamWriter(csEncrypt))
            {
                swEncrypt.Write(plainText);
            }

            return Convert.ToBase64String(msEncrypt.ToArray());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Encryption failed");
            throw;
        }
    }

    private string DecryptString(string cipherText)
    {
        try
        {
            var fullCipher = Convert.FromBase64String(cipherText);
            
            using var aes = Aes.Create();
            var keyBytes = Convert.FromBase64String(_encryptionKey);
            aes.Key = keyBytes;

            // Extract IV
            var iv = new byte[aes.BlockSize / 8];
            var cipher = new byte[fullCipher.Length - iv.Length];
            
            Array.Copy(fullCipher, iv, iv.Length);
            Array.Copy(fullCipher, iv.Length, cipher, 0, cipher.Length);
            
            aes.IV = iv;

            using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            using var msDecrypt = new MemoryStream(cipher);
            using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
            using var srDecrypt = new StreamReader(csDecrypt);
            
            return srDecrypt.ReadToEnd();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Decryption failed");
            throw;
        }
    }
}
