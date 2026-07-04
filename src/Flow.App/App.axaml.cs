using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Flow.App.ViewModels;
using Flow.App.Views;
using Microsoft.Extensions.DependencyInjection;

namespace Flow.App;

/// <summary>
/// The main Avalonia application class.
/// </summary>
public partial class App : Application
{
    /// <summary>
    /// Gets the service provider for the application.
    /// </summary>
    public static IServiceProvider? Services { get; internal set; }

    /// <inheritdoc />
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    /// <inheritdoc />
    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var mainWindow = Services?.GetRequiredService<MainWindow>();
            if (mainWindow is not null)
            {
                desktop.MainWindow = mainWindow;
            }

            // Resolve the overlay window eagerly (but don't show it) so it's already
            // subscribed to IOverlayService/IRocketLeagueService events and ready to
            // appear the moment Rocket League is detected.
            Services?.GetRequiredService<OverlayWindow>();
        }

        base.OnFrameworkInitializationCompleted();
    }
}
