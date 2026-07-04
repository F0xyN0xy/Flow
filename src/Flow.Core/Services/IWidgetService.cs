namespace Flow.Core.Services;

/// <summary>
/// Manages the lifecycle and state of all registered widgets.
/// </summary>
public interface IWidgetService
{
    /// <summary>
    /// Gets the collection of registered widgets.
    /// </summary>
    IReadOnlyCollection<Widgets.IWidget> Widgets { get; }

    /// <summary>
    /// Registers a widget with the service.
    /// </summary>
    void RegisterWidget(Widgets.IWidget widget);

    /// <summary>
    /// Unregisters a widget from the service.
    /// </summary>
    void UnregisterWidget(Widgets.IWidget widget);

    /// <summary>
    /// Updates all enabled widgets asynchronously.
    /// </summary>
    Task UpdateAllAsync(CancellationToken cancellationToken = default);
}
