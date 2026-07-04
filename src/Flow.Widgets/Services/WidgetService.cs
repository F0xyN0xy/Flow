using System.Collections.ObjectModel;
using Flow.Core.Services;
using Flow.Core.Widgets;
using Microsoft.Extensions.Logging;

namespace Flow.Widgets.Services;

/// <summary>
/// Implements <see cref="IWidgetService"/> managing the lifecycle of all widgets.
/// </summary>
public sealed class WidgetService : IWidgetService
{
    private readonly ILogger<WidgetService> _logger;
    private readonly List<IWidget> _widgets = [];

    /// <inheritdoc />
    public IReadOnlyCollection<IWidget> Widgets => new ReadOnlyCollection<IWidget>(_widgets);

    /// <summary>
    /// Initializes a new instance of the <see cref="WidgetService"/> class.
    /// </summary>
    public WidgetService(ILogger<WidgetService> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public void RegisterWidget(IWidget widget)
    {
        ArgumentNullException.ThrowIfNull(widget);

        if (_widgets.Any(w => w.Name.Equals(widget.Name, StringComparison.OrdinalIgnoreCase)))
        {
            _logger.LogWarning("Widget '{Name}' is already registered.", widget.Name);
            return;
        }

        _widgets.Add(widget);
        _logger.LogInformation("Widget '{Name}' registered.", widget.Name);
    }

    /// <inheritdoc />
    public void UnregisterWidget(IWidget widget)
    {
        ArgumentNullException.ThrowIfNull(widget);

        if (_widgets.Remove(widget))
        {
            _logger.LogInformation("Widget '{Name}' unregistered.", widget.Name);
        }
    }

    /// <inheritdoc />
    public async Task UpdateAllAsync(CancellationToken cancellationToken = default)
    {
        foreach (var widget in _widgets.Where(w => w.IsEnabled))
        {
            try
            {
                await widget.UpdateAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update widget '{Name}'.", widget.Name);
            }
        }
    }
}
