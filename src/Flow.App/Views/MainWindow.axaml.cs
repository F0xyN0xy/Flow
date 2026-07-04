using System;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Platform;
using Flow.App.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace Flow.App.Views;

/// <summary>
/// The main application window.
/// </summary>
public partial class MainWindow : Window
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindow"/> class.
    /// </summary>
    public MainWindow()
    {
        AvaloniaXamlLoader.Load(this);
        DataContext = App.Services?.GetRequiredService<MainWindowViewModel>();

        // Set window icon from the embedded AvaloniaResource, not a loose file on disk.
        try
        {
            using var iconStream = AssetLoader.Open(new Uri("avares://Flow/Assets/flow.ico"));
            Icon = new WindowIcon(iconStream);
        }
        catch (Exception)
        {
            // Don't let a missing/bad icon resource crash the whole app.
        }
    }
}