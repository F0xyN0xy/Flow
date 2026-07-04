namespace Flow.Core.Services;

/// <summary>
/// Monitors and communicates with the Rocket League Stats API.
/// </summary>
public interface IRocketLeagueService
{
    /// <summary>
    /// Gets a value indicating whether a connection to Rocket League is active.
    /// </summary>
    bool IsConnected { get; }

    /// <summary>
    /// Gets the UTC time at which the currently running Rocket League process was started,
    /// or <see langword="null"/> if Rocket League is not currently running.
    /// </summary>
    DateTimeOffset? StartedAtUtc { get; }

    /// <summary>
    /// Occurs when the connection state changes.
    /// </summary>
    event EventHandler<bool>? ConnectionStateChanged;

    /// <summary>
    /// Starts monitoring for Rocket League and connects to its API asynchronously.
    /// </summary>
    Task StartAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Disconnects from the Rocket League API and stops monitoring.
    /// </summary>
    Task StopAsync(CancellationToken cancellationToken = default);
}
