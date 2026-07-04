using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Flow.API;
using Flow.App.Services;
using Flow.App.ViewModels;
using Flow.App.Views;
using Flow.Core.Services;
using Flow.Input;
using Flow.Overlay;
using Flow.Storage;
using Flow.Widgets;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Flow.App;

/// <summary>
/// Application entry point.
/// </summary>
public class Program
{
    private static IHost? _host;

    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    public static void Main(string[] args)
    {
        Directory.SetCurrentDirectory(AppContext.BaseDirectory);
    
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .WriteTo.File("logs/flow-.log", rollingInterval: RollingInterval.Day)
            .CreateLogger();

        try
        {
            Log.Information("Flow application starting...");
            BuildAvaloniaApp()
                .StartWithClassicDesktopLifetime(args);
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application terminated unexpectedly.");
        }
        finally
        {
            _host?.StopAsync().GetAwaiter().GetResult();
            _host?.Dispose();
            Log.Information("Flow application shutting down...");
            Log.CloseAndFlush();
        }
    }

    /// <summary>
    /// Configures and builds the Avalonia application.
    /// </summary>
    public static AppBuilder BuildAvaloniaApp()
    {
        _host = CreateHostBuilder().Build();
        App.Services = _host.Services;

        var builder = AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .LogToTrace()
            .WithInterFont();

        builder.AfterSetup(_ =>
        {
            _host.StartAsync().GetAwaiter().GetResult();
        });

        return builder;
    }

    /// <summary>
    /// Configures the dependency injection container and hosted services.
    /// </summary>
    private static IHostBuilder CreateHostBuilder()
    {
        return Host.CreateDefaultBuilder()
            .UseSerilog()
            .ConfigureServices((context, services) =>
            {
                // Core services
                services.AddFlowStorage();
                services.AddFlowApi();
                services.AddFlowOverlay();
                services.AddFlowInput();
                services.AddFlowWidgets();

                // Placeholder services
                services.AddSingleton<ISessionService, SessionService>();
                services.AddSingleton<INotificationService, NotificationService>();

                // ViewModels
                services.AddSingleton<MainWindowViewModel>();
                services.AddSingleton<SettingsWindowViewModel>();

                // Views
                services.AddSingleton<MainWindow>();
                services.AddSingleton<OverlayWindow>();
                services.AddTransient<SettingsWindow>();
            });
    }
}
