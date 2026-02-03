using Microsoft.Extensions.DependencyInjection;
using WorkBeast.Core.Services;
using WorkBeast.Data.Services;

namespace WorkBeast.Data;

/// <summary>
/// Extension methods for registering Data layer services
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds WorkBeast Data services to the dependency injection container
    /// </summary>
    public static IServiceCollection AddWorkBeastData(this IServiceCollection services)
    {
        services.AddScoped<IDataSeeder, DataSeeder>();

        return services;
    }
}
