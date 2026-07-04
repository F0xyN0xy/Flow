using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Flow.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Windows.Input;

namespace Flow.App.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    private readonly ILogger<MainWindowViewModel> _logger;
    private readonly ISettingsService _settingsService;
    private readonly IOverlayService _overlayService;
    private readonly IRocketLeagueService _rocketLeagueService;

    [ObservableProperty]
    private string _statusText = "Ready";

    [ObservableProperty]
    private bool _isOverlayVisible;

    public ICommand ToggleOverlayCommand { get; }

    public MainWindowViewModel(
        ILogger<MainWindowViewModel> logger,
        ISettingsService settingsService,
        IOverlayService overlayService,
        IRocketLeagueService rocketLeagueService)
    {
        _logger = logger;
        _settingsService = settingsService;
        _overlayService = overlayService;
        _rocketLeagueService = rocketLeagueService;

        ToggleOverlayCommand = new RelayCommand(ToggleOverlay);

        _overlayService.VisibilityChanged += OnOverlayVisibilityChanged;
        _rocketLeagueService.ConnectionStateChanged += OnRocketLeagueConnectionChanged;

        // The RocketLeagueService may have already detected the game before this
        // ViewModel finished constructing (hosted services start earlier than the
        // window graph). Sync to whatever the current state already is.
        if (_rocketLeagueService.IsConnected)
        {
            OnRocketLeagueConnectionChanged(this, true);
        }

        _ = InitializeAsync();
    }

    private async Task InitializeAsync()
    {
        try
        {
            await _settingsService.LoadAsync();
            if (!_rocketLeagueService.IsConnected)
            {
                StatusText = "Waiting for Rocket League...";
            }

            _logger.LogInformation("Main window initialized.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize main window.");
            StatusText = "Error loading settings";
        }
    }

    [RelayCommand]
    private void OpenSettings()
    {
        _logger.LogInformation("Open settings requested.");

        if (App.Services is null)
        {
            _logger.LogWarning("Service provider is not available.");
            return;
        }

        var settingsWindow = App.Services.GetRequiredService<Views.SettingsWindow>();
        settingsWindow.Show();
    }

    private void ToggleOverlay()
    {
        _ = _overlayService.ToggleAsync();
    }

    private void OnOverlayVisibilityChanged(object? sender, EventArgs e)
    {
        Dispatcher.UIThread.Post(() =>
        {
            IsOverlayVisible = _overlayService.IsVisible;
            _logger.LogInformation("Overlay visibility changed. Visible: {Visible}", IsOverlayVisible);
        });
    }

    private void OnRocketLeagueConnectionChanged(object? sender, bool isConnected)
    {
        Dispatcher.UIThread.Post(() =>
        {
            StatusText = isConnected
                ? $"Rocket League running (started {_rocketLeagueService.StartedAtUtc:HH:mm:ss} UTC)"
                : "Waiting for Rocket League...";
        });
    }
}
