using Flow.Core.Services;
using Flow.Storage.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Flow.Storage;

/// <summary>
/// Extension methods for registering Flow.Storage services.
/// </summary>
public static class StorageServiceExtensions
{
    /// <summary>
    /// Adds storage services to the dependency injection container.
    /// </summary>
    public static IServiceCollection AddFlowStorage(this IServiceCollection services)
    {
        services.AddSingleton<ISettingsService, SettingsService>();
        return services;
    }
}
