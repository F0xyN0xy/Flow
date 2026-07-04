using System.Diagnostics;
using Flow.Core.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Flow.API.Services;

/// <summary>
/// Detects whether Rocket League is running by polling for its process, and exposes
/// when it started. A future iteration can layer the WebSocket Stats API connection
/// on top of this once a game session is confirmed to be active.
/// </summary>
public sealed class RocketLeagueService : IRocketLeagueService, IHostedService, IDisposable
{
    private const string ProcessName = "RocketLeague";
    private static readonly TimeSpan PollInterval = TimeSpan.FromSeconds(2);

    private readonly ILogger<RocketLeagueService> _logger;
    private Timer? _pollTimer;

    /// <inheritdoc />
    public bool IsConnected { get; private set; }

    /// <inheritdoc />
    public DateTimeOffset? StartedAtUtc { get; private set; }

    /// <inheritdoc />
    public event EventHandler<bool>? ConnectionStateChanged;

    /// <summary>
    /// Initializes a new instance of the <see cref="RocketLeagueService"/> class.
    /// </summary>
    public RocketLeagueService(ILogger<RocketLeagueService> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public Task StartAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Rocket League service starting. Polling for '{ProcessName}' every {Interval}.", ProcessName, PollInterval);
        _pollTimer = new Timer(_ => PollProcess(), null, TimeSpan.Zero, PollInterval);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task StopAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Rocket League service stopping...");
        _pollTimer?.Dispose();
        _pollTimer = null;

        if (IsConnected)
        {
            IsConnected = false;
            StartedAtUtc = null;
            ConnectionStateChanged?.Invoke(this, false);
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Checks whether the Rocket League process is currently running and raises
    /// <see cref="ConnectionStateChanged"/> if the state has changed since the last poll.
    /// </summary>
    private void PollProcess()
    {
        Process[] processes;
        try
        {
            processes = Process.GetProcessesByName(ProcessName);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to query running processes for '{ProcessName}'.", ProcessName);
            return;
        }

        try
        {
            var isRunning = processes.Length > 0;

            // No state change since the last poll: nothing to do.
            if (isRunning == IsConnected)
            {
                return;
            }

            IsConnected = isRunning;

            if (isRunning)
            {
                try
                {
                    // Process.StartTime is in local time; normalize to UTC for consumers.
                    StartedAtUtc = processes[0].StartTime.ToUniversalTime();
                }
                catch (Exception ex)
                {
                    // Can fail due to access restrictions (e.g. elevated process). Fall back to "now".
                    _logger.LogWarning(ex, "Could not read Rocket League process start time; using current time instead.");
                    StartedAtUtc = DateTimeOffset.UtcNow;
                }

                _logger.LogInformation("Rocket League detected. Started at {StartedAtUtc}.", StartedAtUtc);
            }
            else
            {
                StartedAtUtc = null;
                _logger.LogInformation("Rocket League is no longer running.");
            }

            ConnectionStateChanged?.Invoke(this, isRunning);
        }
        finally
        {
            foreach (var process in processes)
            {
                process.Dispose();
            }
        }
    }

    Task IHostedService.StartAsync(CancellationToken cancellationToken)
    {
        return StartAsync(cancellationToken);
    }

    Task IHostedService.StopAsync(CancellationToken cancellationToken)
    {
        return StopAsync(cancellationToken);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _pollTimer?.Dispose();
    }
}
