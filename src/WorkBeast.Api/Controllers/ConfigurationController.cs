using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkBeast.Core.Models;
using WorkBeast.Core.Services;

namespace WorkBeast.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class ConfigurationController : ControllerBase
{
    private readonly IConfigurationService _configService;
    private readonly ILogger<ConfigurationController> _logger;

    public ConfigurationController(
        IConfigurationService configService,
        ILogger<ConfigurationController> logger)
    {
        _configService = configService;
        _logger = logger;
    }

    /// <summary>
    /// Get complete application configuration (Admin only)
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<AppConfiguration>> GetConfiguration()
    {
        var config = await _configService.LoadAsync();
        
        // Mask sensitive data in response
        var safeConfig = MaskSensitiveData(config);
        return Ok(safeConfig);
    }

    /// <summary>
    /// Get a specific configuration value (Admin only)
    /// </summary>
    [HttpGet("{key}")]
    public async Task<ActionResult<object>> GetConfigValue(string key)
    {
        try
        {
            var value = await _configService.GetValueAsync<object>(key);
            if (value == null)
            {
                return NotFound(new { message = $"Configuration key '{key}' not found" });
            }

            return Ok(new { key, value });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get configuration value for key: {Key}", key);
            return BadRequest(new { message = "Invalid configuration key" });
        }
    }

    /// <summary>
    /// Update application settings (Admin only)
    /// </summary>
    [HttpPut("application")]
    public async Task<IActionResult> UpdateApplicationSettings([FromBody] ApplicationSettings settings)
    {
        try
        {
            var config = await _configService.LoadAsync();
            config.Application = settings;
            await _configService.SaveAsync(config);

            _logger.LogInformation("Application settings updated by admin");
            return Ok(new { message = "Application settings updated successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update application settings");
            return StatusCode(500, new { message = "Failed to update settings" });
        }
    }

    /// <summary>
    /// Update database settings (Admin only)
    /// </summary>
    [HttpPut("database")]
    public async Task<IActionResult> UpdateDatabaseSettings([FromBody] DatabaseSettings settings)
    {
        try
        {
            var config = await _configService.LoadAsync();
            config.Database = settings;
            await _configService.SaveAsync(config);

            _logger.LogInformation("Database settings updated by admin");
            return Ok(new { message = "Database settings updated successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update database settings");
            return StatusCode(500, new { message = "Failed to update settings" });
        }
    }

    /// <summary>
    /// Update logging settings (Admin only)
    /// </summary>
    [HttpPut("logging")]
    public async Task<IActionResult> UpdateLoggingSettings([FromBody] LoggingSettings settings)
    {
        try
        {
            var config = await _configService.LoadAsync();
            config.Logging = settings;
            await _configService.SaveAsync(config);

            _logger.LogInformation("Logging settings updated by admin");
            return Ok(new { message = "Logging settings updated successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update logging settings");
            return StatusCode(500, new { message = "Failed to update settings" });
        }
    }

    /// <summary>
    /// Update security settings (Admin only)
    /// </summary>
    [HttpPut("security")]
    public async Task<IActionResult> UpdateSecuritySettings([FromBody] SecuritySettings settings)
    {
        try
        {
            var config = await _configService.LoadAsync();
            config.Security = settings;
            await _configService.SaveAsync(config);

            _logger.LogWarning("Security settings updated by admin - application restart may be required");
            return Ok(new { message = "Security settings updated successfully. Application restart may be required." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update security settings");
            return StatusCode(500, new { message = "Failed to update settings" });
        }
    }

    /// <summary>
    /// Set an external API key (Admin only)
    /// </summary>
    [HttpPost("apikey")]
    public async Task<IActionResult> SetApiKey([FromBody] ApiKeyRequest request)
    {
        try
        {
            var config = await _configService.LoadAsync();

            switch (request.Provider.ToLower())
            {
                case "openai":
                    config.ExternalApis.OpenAiApiKey = request.ApiKey;
                    break;
                case "stripe":
                    config.ExternalApis.StripeApiKey = request.ApiKey;
                    break;
                case "sendgrid":
                    config.ExternalApis.SendGridApiKey = request.ApiKey;
                    break;
                case "googlemaps":
                    config.ExternalApis.GoogleMapsApiKey = request.ApiKey;
                    break;
                default:
                    config.ExternalApis.CustomApiKeys[request.Provider] = request.ApiKey;
                    break;
            }

            await _configService.SaveAsync(config);

            _logger.LogInformation("API key set for provider: {Provider}", request.Provider);
            return Ok(new { message = $"API key for '{request.Provider}' saved successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to set API key for provider: {Provider}", request.Provider);
            return StatusCode(500, new { message = "Failed to save API key" });
        }
    }

    /// <summary>
    /// Delete an API key (Admin only)
    /// </summary>
    [HttpDelete("apikey/{provider}")]
    public async Task<IActionResult> DeleteApiKey(string provider)
    {
        try
        {
            var config = await _configService.LoadAsync();

            switch (provider.ToLower())
            {
                case "openai":
                    config.ExternalApis.OpenAiApiKey = null;
                    break;
                case "stripe":
                    config.ExternalApis.StripeApiKey = null;
                    break;
                case "sendgrid":
                    config.ExternalApis.SendGridApiKey = null;
                    break;
                case "googlemaps":
                    config.ExternalApis.GoogleMapsApiKey = null;
                    break;
                default:
                    config.ExternalApis.CustomApiKeys.Remove(provider);
                    break;
            }

            await _configService.SaveAsync(config);

            _logger.LogInformation("API key deleted for provider: {Provider}", provider);
            return Ok(new { message = $"API key for '{provider}' deleted successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete API key for provider: {Provider}", provider);
            return StatusCode(500, new { message = "Failed to delete API key" });
        }
    }

    /// <summary>
    /// Get configuration file path (Admin only)
    /// </summary>
    [HttpGet("path")]
    public ActionResult<object> GetConfigPath()
    {
        var path = _configService.GetConfigurationFilePath();
        return Ok(new { configurationPath = path, exists = _configService.ConfigurationExists() });
    }

    private AppConfiguration MaskSensitiveData(AppConfiguration config)
    {
        var masked = new AppConfiguration
        {
            Application = config.Application,
            Database = config.Database,
            Logging = config.Logging,
            Security = new SecuritySettings
            {
                JwtSecretKey = MaskString(config.Security.JwtSecretKey),
                JwtExpirationDays = config.Security.JwtExpirationDays,
                PasswordMinLength = config.Security.PasswordMinLength,
                RequireDigit = config.Security.RequireDigit,
                RequireUppercase = config.Security.RequireUppercase,
                RequireLowercase = config.Security.RequireLowercase,
                RequireNonAlphanumeric = config.Security.RequireNonAlphanumeric,
                MaxFailedAccessAttempts = config.Security.MaxFailedAccessAttempts,
                LockoutMinutes = config.Security.LockoutMinutes
            },
            ExternalApis = new ExternalApiSettings
            {
                OpenAiApiKey = MaskString(config.ExternalApis.OpenAiApiKey),
                StripeApiKey = MaskString(config.ExternalApis.StripeApiKey),
                SendGridApiKey = MaskString(config.ExternalApis.SendGridApiKey),
                GoogleMapsApiKey = MaskString(config.ExternalApis.GoogleMapsApiKey),
                CustomApiKeys = config.ExternalApis.CustomApiKeys.ToDictionary(
                    kvp => kvp.Key,
                    kvp => MaskString(kvp.Value))
            }
        };

        return masked;
    }

    private string MaskString(string? value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return string.Empty;
        }

        if (value.Length <= 8)
        {
            return "****";
        }

        return $"{value[..4]}...{value[^4..]}";
    }
}

public class ApiKeyRequest
{
    public string Provider { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
}
