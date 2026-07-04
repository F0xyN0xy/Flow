using Flow.Core.Services;
using Microsoft.Extensions.Logging;

namespace Flow.Overlay.Services;

/// <summary>
/// Manages the transparent overlay window lifecycle and visibility.
/// This service is UI-framework agnostic: it tracks state and raises events;
/// the actual window (in Flow.App) subscribes to <see cref="VisibilityChanged"/>
/// to show/hide itself.
/// </summary>
public sealed class OverlayService : IOverlayService
{
    private readonly ILogger<OverlayService> _logger;

    /// <inheritdoc />
    public bool IsVisible { get; private set; }

    /// <inheritdoc />
    public event EventHandler? VisibilityChanged;

    /// <summary>
    /// Initializes a new instance of the <see cref="OverlayService"/> class.
    /// </summary>
    public OverlayService(ILogger<OverlayService> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public Task ShowAsync(CancellationToken cancellationToken = default)
    {
        if (IsVisible)
        {
            return Task.CompletedTask;
        }

        _logger.LogInformation("Overlay show requested.");
        IsVisible = true;
        VisibilityChanged?.Invoke(this, EventArgs.Empty);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task HideAsync(CancellationToken cancellationToken = default)
    {
        if (!IsVisible)
        {
            return Task.CompletedTask;
        }

        _logger.LogInformation("Overlay hide requested.");
        IsVisible = false;
        VisibilityChanged?.Invoke(this, EventArgs.Empty);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task ToggleAsync(CancellationToken cancellationToken = default)
    {
        if (IsVisible)
            await HideAsync(cancellationToken);
        else
            await ShowAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task UpdatePropertiesAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Overlay properties update requested.");
        return Task.CompletedTask;
    }
}
