using Flow.API.Services;
using Flow.Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Flow.API;

/// <summary>
/// Extension methods for registering Flow.API services.
/// </summary>
public static class ApiServiceExtensions
{
    /// <summary>
    /// Adds API services to the dependency injection container.
    /// </summary>
    public static IServiceCollection AddFlowApi(this IServiceCollection services)
    {
        services.AddSingleton<IRocketLeagueService, RocketLeagueService>();
        services.AddHostedService(provider => (RocketLeagueService)provider.GetRequiredService<IRocketLeagueService>());
        return services;
    }
}
