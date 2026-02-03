using Microsoft.Extensions.Logging;
using Moq;
using WorkBeast.Core.Services;
using Xunit;

namespace WorkBeast.Tests.Services;

[Collection("FileSystem")]
public class ApplicationInitializerTests : IDisposable
{
    private readonly Mock<ILogger<ApplicationInitializer>> _loggerMock;
    private readonly ApplicationInitializer _service;
    private readonly string _testAppDataPath;
    private static readonly object _lockObj = new object();

    public ApplicationInitializerTests()
    {
        _loggerMock = new Mock<ILogger<ApplicationInitializer>>();
        _service = new ApplicationInitializer(_loggerMock.Object);
        _testAppDataPath = _service.GetApplicationDataPath();
        
        // Ensure clean state before test
        lock (_lockObj)
        {
            if (Directory.Exists(_testAppDataPath))
            {
                try
                {
                    Directory.Delete(_testAppDataPath, true);
                    Thread.Sleep(100);
                }
                catch
                {
                    // Ignore cleanup errors
                }
            }
        }
    }

    public void Dispose()
    {
        // Clean up test directories after test completes
        lock (_lockObj)
        {
            if (Directory.Exists(_testAppDataPath))
            {
                try
                {
                    Directory.Delete(_testAppDataPath, true);
                }
                catch
                {
                    // Ignore cleanup errors
                }
            }
        }
    }

    [Fact]
    public void GetApplicationDataPath_ReturnsValidPath()
    {
        // Act
        var path = _service.GetApplicationDataPath();

        // Assert
        Assert.NotNull(path);
        Assert.NotEmpty(path);
        Assert.Contains("WorkBeast", path);
    }

    [Fact]
    public void GetLogsPath_ReturnsValidPath()
    {
        // Act
        var path = _service.GetLogsPath();

        // Assert
        Assert.NotNull(path);
        Assert.Contains("Logs", path);
        Assert.Contains("WorkBeast", path);
    }

    [Fact]
    public void GetBackupsPath_ReturnsValidPath()
    {
        // Act
        var path = _service.GetBackupsPath();

        // Assert
        Assert.NotNull(path);
        Assert.Contains("Backups", path);
        Assert.Contains("WorkBeast", path);
    }

    [Fact]
    public void GetExportsPath_ReturnsValidPath()
    {
        // Act
        var path = _service.GetExportsPath();

        // Assert
        Assert.NotNull(path);
        Assert.Contains("Exports", path);
        Assert.Contains("WorkBeast", path);
    }

    [Fact]
    public void GetImportsPath_ReturnsValidPath()
    {
        // Act
        var path = _service.GetImportsPath();

        // Assert
        Assert.NotNull(path);
        Assert.Contains("Imports", path);
        Assert.Contains("WorkBeast", path);
    }

    [Fact]
    public async Task InitializeAsync_CreatesRequiredDirectories()
    {
        // Act
        await _service.InitializeAsync();

        // Assert
        Assert.True(Directory.Exists(_service.GetApplicationDataPath()));
        Assert.True(Directory.Exists(_service.GetLogsPath()));
        Assert.True(Directory.Exists(_service.GetBackupsPath()));
        Assert.True(Directory.Exists(_service.GetExportsPath()));
        Assert.True(Directory.Exists(_service.GetImportsPath()));
    }

    [Fact]
    public async Task InitializeAsync_CreatesReadmeFile()
    {
        // Act
        await _service.InitializeAsync();

        // Assert
        var readmePath = Path.Combine(_service.GetApplicationDataPath(), "README.txt");
        Assert.True(File.Exists(readmePath));

        var content = await File.ReadAllTextAsync(readmePath);
        Assert.Contains("WorkBeast Application Data Directory", content);
        Assert.Contains("Security:", content);
        Assert.Contains("AES-256", content);
    }

    [Fact]
    public async Task InitializeAsync_RunsMultipleTimesWithoutError()
    {
        // Act
        await _service.InitializeAsync();
        await _service.InitializeAsync(); // Second call should not fail

        // Assert
        Assert.True(Directory.Exists(_service.GetApplicationDataPath()));
    }
}
