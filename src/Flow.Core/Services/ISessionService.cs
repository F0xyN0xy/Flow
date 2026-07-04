namespace Flow.Core.Services;

/// <summary>
/// Tracks the current Rocket League session and aggregates statistics.
/// </summary>
public interface ISessionService
{
    /// <summary>
    /// Gets a value indicating whether a session is currently active.
    /// </summary>
    bool IsSessionActive { get; }

    /// <summary>
    /// Starts a new session asynchronously.
    /// </summary>
    Task StartSessionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Ends the current session asynchronously.
    /// </summary>
    Task EndSessionAsync(CancellationToken cancellationToken = default);
}
