using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Flow.App.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace Flow.App.Views;

/// <summary>
/// The settings configuration window.
/// </summary>
public partial class SettingsWindow : Window
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SettingsWindow"/> class.
    /// </summary>
    public SettingsWindow()
    {
        AvaloniaXamlLoader.Load(this);
        DataContext = App.Services?.GetRequiredService<SettingsWindowViewModel>();
    }
}
