namespace Flow.Core.Services;

/// <summary>
/// Manages the transparent overlay window lifecycle and visibility.
/// </summary>
public interface IOverlayService
{
    /// <summary>
    /// Gets a value indicating whether the overlay is currently visible.
    /// </summary>
    bool IsVisible { get; }

    /// <summary>
    /// Occurs when <see cref="IsVisible"/> changes as a result of <see cref="ShowAsync"/>,
    /// <see cref="HideAsync"/>, or <see cref="ToggleAsync"/>.
    /// </summary>
    event EventHandler? VisibilityChanged;

    /// <summary>
    /// Shows the overlay window asynchronously.
    /// </summary>
    Task ShowAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Hides the overlay window asynchronously.
    /// </summary>
    Task HideAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Toggles the overlay visibility.
    /// </summary>
    Task ToggleAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates overlay properties (opacity, scale, position) from settings.
    /// </summary>
    Task UpdatePropertiesAsync(CancellationToken cancellationToken = default);
}
