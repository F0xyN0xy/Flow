using Flow.Core.Services;
using Flow.Widgets.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Flow.Widgets;

/// <summary>
/// Extension methods for registering Flow.Widgets services.
/// </summary>
public static class WidgetServiceExtensions
{
    /// <summary>
    /// Adds widget services to the dependency injection container.
    /// </summary>
    public static IServiceCollection AddFlowWidgets(this IServiceCollection services)
    {
        services.AddSingleton<IWidgetService, WidgetService>();
        return services;
    }
}
