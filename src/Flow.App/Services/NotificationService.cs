using Flow.Core.Services;
using Microsoft.Extensions.Logging;

namespace Flow.App.Services;

/// <summary>
/// Placeholder implementation of <see cref="INotificationService"/>.
/// </summary>
public sealed class NotificationService : INotificationService
{
    private readonly ILogger<NotificationService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="NotificationService"/> class.
    /// </summary>
    public NotificationService(ILogger<NotificationService> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public Task ShowInfoAsync(string message, string? title = null, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("[Notification] {Title}: {Message}", title ?? "Info", message);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task ShowWarningAsync(string message, string? title = null, CancellationToken cancellationToken = default)
    {
        _logger.LogWarning("[Notification] {Title}: {Message}", title ?? "Warning", message);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task ShowErrorAsync(string message, string? title = null, CancellationToken cancellationToken = default)
    {
        _logger.LogError("[Notification] {Title}: {Message}", title ?? "Error", message);
        return Task.CompletedTask;
    }
}
