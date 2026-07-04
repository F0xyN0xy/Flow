namespace Flow.Core.Services;

/// <summary>
/// Handles global hotkeys and controller input for the application.
/// </summary>
public interface IInputService
{
    /// <summary>
    /// Occurs when the configured overlay toggle hotkey is pressed.
    /// </summary>
    event EventHandler? ToggleOverlayRequested;

    /// <summary>
    /// Initializes the input hooks and begins listening asynchronously.
    /// </summary>
    Task StartAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Releases input hooks and stops listening asynchronously.
    /// </summary>
    Task StopAsync(CancellationToken cancellationToken = default);
}
