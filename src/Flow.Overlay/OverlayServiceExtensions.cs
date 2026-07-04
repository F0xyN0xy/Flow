using Flow.Core.Services;
using Flow.Overlay.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Flow.Overlay;

/// <summary>
/// Extension methods for registering Flow.Overlay services.
/// </summary>
public static class OverlayServiceExtensions
{
    /// <summary>
    /// Adds overlay services to the dependency injection container.
    /// </summary>
    public static IServiceCollection AddFlowOverlay(this IServiceCollection services)
    {
        services.AddSingleton<IOverlayService, OverlayService>();
        return services;
    }
}
