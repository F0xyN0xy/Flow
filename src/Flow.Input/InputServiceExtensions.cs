using Flow.Core.Services;
using Flow.Input.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Flow.Input;

/// <summary>
/// Extension methods for registering Flow.Input services.
/// </summary>
public static class InputServiceExtensions
{
    /// <summary>
    /// Adds input services to the dependency injection container.
    /// </summary>
    public static IServiceCollection AddFlowInput(this IServiceCollection services)
    {
        services.AddSingleton<IInputService, InputService>();
        services.AddHostedService(provider => (InputService)provider.GetRequiredService<IInputService>());
        return services;
    }
}
