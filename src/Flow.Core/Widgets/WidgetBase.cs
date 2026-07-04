using CommunityToolkit.Mvvm.ComponentModel;

namespace Flow.Core.Widgets;

/// <summary>
/// Abstract base class for all Flow widgets providing common properties.
/// </summary>
public abstract partial class WidgetBase : ObservableObject, IWidget
{
    [ObservableProperty]
    private bool _isEnabled = true;

    [ObservableProperty]
    private double _positionX;

    [ObservableProperty]
    private double _positionY;

    [ObservableProperty]
    private double _width = 200;

    [ObservableProperty]
    private double _height = 100;

    /// <inheritdoc />
    public abstract string Name { get; }

    /// <inheritdoc />
    public virtual Task UpdateAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public abstract void Render();
}
