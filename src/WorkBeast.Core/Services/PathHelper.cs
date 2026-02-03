namespace WorkBeast.Core.Services;

/// <summary>
/// Helper class for getting application paths
/// </summary>
public static class PathHelper
{
    /// <summary>
    /// Gets the full path for the database file
    /// </summary>
    public static string GetDatabasePath()
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
        
        // Ensure directory exists
        if (!Directory.Exists(appDataPath))
        {
            Directory.CreateDirectory(appDataPath);
        }

        return Path.Combine(appDataPath, "workbeast.db");
    }
}
