using Microsoft.Extensions.DependencyInjection;

namespace WorkBeast.Core.Services;

/// <summary>
/// Extension methods for registering Core services
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds WorkBeast Core services to the dependency injection container
    /// </summary>
    public static IServiceCollection AddWorkBeastCore(this IServiceCollection services)
    {
        services.AddSingleton<IApplicationInitializer, ApplicationInitializer>();
        services.AddScoped<IAuthService, AuthService>();

        return services;
    }
}
