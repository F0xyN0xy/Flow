namespace Flow.Core.Services;

/// <summary>
/// Displays user-facing notifications and toast messages.
/// </summary>
public interface INotificationService
{
    /// <summary>
    /// Shows an informational notification asynchronously.
    /// </summary>
    Task ShowInfoAsync(string message, string? title = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Shows a warning notification asynchronously.
    /// </summary>
    Task ShowWarningAsync(string message, string? title = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Shows an error notification asynchronously.
    /// </summary>
    Task ShowErrorAsync(string message, string? title = null, CancellationToken cancellationToken = default);
}
