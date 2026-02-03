namespace WorkBeast.Core.Models;

/// <summary>
/// Application configuration model
/// </summary>
public class AppConfiguration
{
    public ApplicationSettings Application { get; set; } = new();
    public DatabaseSettings Database { get; set; } = new();
    public LoggingSettings Logging { get; set; } = new();
    public SecuritySettings Security { get; set; } = new();
    public ExternalApiSettings ExternalApis { get; set; } = new();
}

/// <summary>
/// General application settings
/// </summary>
public class ApplicationSettings
{
    public string Name { get; set; } = "WorkBeast";
    public string Version { get; set; } = "1.0.0";
    public string Theme { get; set; } = "Light";
    public string Language { get; set; } = "en-US";
    public string Environment { get; set; } = "Development";
}

/// <summary>
/// Database configuration settings
/// </summary>
public class DatabaseSettings
{
    public bool AutoBackup { get; set; } = true;
    public int BackupIntervalDays { get; set; } = 7;
    public int MaxBackupCount { get; set; } = 10;
    public bool AutoMigrate { get; set; } = true;
}

/// <summary>
/// Logging configuration settings
/// </summary>
public class LoggingSettings
{
    public bool Enabled { get; set; } = true;
    public string Level { get; set; } = "Information";
    public int RetentionDays { get; set; } = 30;
    public bool LogToFile { get; set; } = true;
    public bool LogToConsole { get; set; } = true;
}

/// <summary>
/// Security and authentication settings
/// </summary>
public class SecuritySettings
{
    public string JwtSecretKey { get; set; } = string.Empty;
    public int JwtExpirationDays { get; set; } = 7;
    public int PasswordMinLength { get; set; } = 6;
    public bool RequireDigit { get; set; } = true;
    public bool RequireUppercase { get; set; } = true;
    public bool RequireLowercase { get; set; } = true;
    public bool RequireNonAlphanumeric { get; set; } = true;
    public int MaxFailedAccessAttempts { get; set; } = 5;
    public int LockoutMinutes { get; set; } = 15;
}

/// <summary>
/// External API keys and settings
/// </summary>
public class ExternalApiSettings
{
    public string? OpenAiApiKey { get; set; }
    public string? StripeApiKey { get; set; }
    public string? SendGridApiKey { get; set; }
    public string? GoogleMapsApiKey { get; set; }
    public Dictionary<string, string> CustomApiKeys { get; set; } = new();
}
