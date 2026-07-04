namespace Flow.Core.Widgets;

/// <summary>
/// Defines the contract for a Flow overlay widget.
/// </summary>
public interface IWidget
{
    /// <summary>
    /// Gets the unique name of the widget.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets or sets a value indicating whether the widget is enabled.
    /// </summary>
    bool IsEnabled { get; set; }

    /// <summary>
    /// Gets or sets the X position of the widget on the overlay.
    /// </summary>
    double PositionX { get; set; }

    /// <summary>
    /// Gets or sets the Y position of the widget on the overlay.
    /// </summary>
    double PositionY { get; set; }

    /// <summary>
    /// Gets or sets the width of the widget.
    /// </summary>
    double Width { get; set; }

    /// <summary>
    /// Gets or sets the height of the widget.
    /// </summary>
    double Height { get; set; }

    /// <summary>
    /// Updates the widget state with the latest data asynchronously.
    /// </summary>
    Task UpdateAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Renders the widget content. Implementation deferred to concrete types.
    /// </summary>
    void Render();
}
