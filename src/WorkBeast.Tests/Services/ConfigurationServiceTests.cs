using Microsoft.Extensions.Logging;
using Moq;
using WorkBeast.Core.Models;
using WorkBeast.Core.Services;
using Xunit;

namespace WorkBeast.Tests.Services;

public class ConfigurationServiceTests : IDisposable
{
    private readonly Mock<ILogger<ConfigurationService>> _loggerMock;
    private readonly ConfigurationService _service;
    private readonly string _configPath;

    public ConfigurationServiceTests()
    {
        _loggerMock = new Mock<ILogger<ConfigurationService>>();
        _service = new ConfigurationService(_loggerMock.Object);
        _configPath = _service.GetConfigurationFilePath();
    }

    public void Dispose()
    {
        // Clean up test files
        var directory = Path.GetDirectoryName(_configPath);
        if (!string.IsNullOrEmpty(directory) && Directory.Exists(directory))
        {
            try
            {
                Directory.Delete(directory, true);
            }
            catch
            {
                // Ignore cleanup errors
            }
        }
    }

    [Fact]
    public void GetConfigurationFilePath_ReturnsValidPath()
    {
        // Act
        var path = _service.GetConfigurationFilePath();

        // Assert
        Assert.NotNull(path);
        Assert.Contains("WorkBeast", path);
        Assert.Contains("config.encrypted.json", path);
    }

    [Fact]
    public void ConfigurationExists_ReturnsFalseWhenFileDoesNotExist()
    {
        // Act
        var exists = _service.ConfigurationExists();

        // Assert
        Assert.False(exists);
    }

    [Fact]
    public async Task InitializeAsync_CreatesConfigurationFile()
    {
        // Act
        await _service.InitializeAsync();

        // Assert
        Assert.True(_service.ConfigurationExists());
    }

    [Fact]
    public void Constructor_CreatesEncryptionKey()
    {
        // The encryption key should be created during construction
        var directory = Path.GetDirectoryName(_configPath);
        Assert.NotNull(directory);
        
        var keyPath = Path.Combine(directory, ".encryption.key");
        
        // Key should exist after constructor is called
        Assert.True(File.Exists(keyPath), $"Encryption key file not found at: {keyPath}");
        
        // Key should not be empty
        var keyContent = File.ReadAllText(keyPath);
        Assert.NotEmpty(keyContent);
    }

    [Fact]
    public async Task LoadAsync_ReturnsDefaultConfiguration()
    {
        // Act
        var config = await _service.LoadAsync();

        // Assert
        Assert.NotNull(config);
        Assert.NotNull(config.Application);
        Assert.NotNull(config.Database);
        Assert.NotNull(config.Logging);
        Assert.NotNull(config.Security);
        Assert.NotNull(config.ExternalApis);
        Assert.Equal("WorkBeast", config.Application.Name);
    }

    [Fact]
    public async Task SaveAsync_PersistsConfiguration()
    {
        // Arrange
        var config = new AppConfiguration
        {
            Application = new ApplicationSettings { Name = "TestApp", Version = "2.0.0" }
        };

        // Act
        await _service.SaveAsync(config);
        var loaded = await _service.LoadAsync();

        // Assert
        Assert.Equal("TestApp", loaded.Application.Name);
        Assert.Equal("2.0.0", loaded.Application.Version);
    }

    [Fact]
    public async Task GetValueAsync_ReturnsCorrectValue()
    {
        // Arrange
        await _service.InitializeAsync();

        // Act
        var appName = await _service.GetValueAsync<string>("Application.Name");

        // Assert
        Assert.Equal("WorkBeast", appName);
    }

    [Fact]
    public async Task SetValueAsync_UpdatesValue()
    {
        // Arrange
        await _service.InitializeAsync();

        // Act
        await _service.SetValueAsync("Application.Name", "NewName");
        var value = await _service.GetValueAsync<string>("Application.Name");

        // Assert
        Assert.Equal("NewName", value);
    }

    [Fact]
    public async Task SetValueAsync_UpdatesNestedValue()
    {
        // Arrange
        await _service.InitializeAsync();

        // Act
        await _service.SetValueAsync("Database.BackupIntervalDays", 14);
        var value = await _service.GetValueAsync<int>("Database.BackupIntervalDays");

        // Assert
        Assert.Equal(14, value);
    }

    [Fact]
    public async Task LoadAsync_DecryptsDataCorrectly()
    {
        // Arrange
        var originalConfig = new AppConfiguration
        {
            Security = new SecuritySettings { JwtSecretKey = "SuperSecret123!" },
            ExternalApis = new ExternalApiSettings { OpenAiApiKey = "sk-test-key" }
        };

        // Act
        await _service.SaveAsync(originalConfig);
        var loadedConfig = await _service.LoadAsync();

        // Assert
        Assert.Equal("SuperSecret123!", loadedConfig.Security.JwtSecretKey);
        Assert.Equal("sk-test-key", loadedConfig.ExternalApis.OpenAiApiKey);
    }

    [Fact]
    public async Task SaveAsync_CreatesEncryptedFile()
    {
        // Arrange
        var config = new AppConfiguration();

        // Act
        await _service.SaveAsync(config);

        // Assert
        var encryptedContent = await File.ReadAllTextAsync(_configPath);
        // Encrypted content should not contain plain text JSON
        Assert.DoesNotContain("WorkBeast", encryptedContent);
        Assert.DoesNotContain("Application", encryptedContent);
    }

    [Fact]
    public async Task InitializeAsync_RunsMultipleTimesWithoutError()
    {
        // Act
        await _service.InitializeAsync();
        await _service.InitializeAsync();
        await _service.InitializeAsync();

        // Assert
        Assert.True(_service.ConfigurationExists());
    }
}
