using Flow.Core.Services;
using Microsoft.Extensions.Logging;

namespace Flow.App.Services;

/// <summary>
/// Placeholder implementation of <see cref="ISessionService"/>.
/// </summary>
public sealed class SessionService : ISessionService
{
    private readonly ILogger<SessionService> _logger;

    /// <inheritdoc />
    public bool IsSessionActive { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SessionService"/> class.
    /// </summary>
    public SessionService(ILogger<SessionService> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public Task StartSessionAsync(CancellationToken cancellationToken = default)
    {
        IsSessionActive = true;
        _logger.LogInformation("Session started.");
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task EndSessionAsync(CancellationToken cancellationToken = default)
    {
        IsSessionActive = false;
        _logger.LogInformation("Session ended.");
        return Task.CompletedTask;
    }
}
