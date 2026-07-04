using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Threading;
using Flow.Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Flow.App.Views;

public partial class OverlayWindow : Window
{
    private readonly IOverlayService? _overlayService;
    private readonly ISettingsService? _settingsService;
    private readonly IRocketLeagueService? _rocketLeagueService;
    private readonly IInputService? _inputService;

    private bool _isDragging;
    private Point _dragStartPointerPosition;
    private PixelPoint _dragStartWindowPosition;

    public OverlayWindow()
    {
        AvaloniaXamlLoader.Load(this);

        _overlayService = App.Services?.GetRequiredService<IOverlayService>();
        _settingsService = App.Services?.GetRequiredService<ISettingsService>();
        _rocketLeagueService = App.Services?.GetRequiredService<IRocketLeagueService>();
        _inputService = App.Services?.GetRequiredService<IInputService>();

        if (_overlayService is not null)
        {
            _overlayService.VisibilityChanged += OnOverlayVisibilityChanged;
        }

        if (_rocketLeagueService is not null)
        {
            _rocketLeagueService.ConnectionStateChanged += OnRocketLeagueConnectionChanged;
        }

        if (_inputService is not null)
        {
            _inputService.ToggleOverlayRequested += OnToggleOverlayRequested;
        }

        Opened += OnOpened;
        PointerPressed += OnPointerPressed;
        PointerMoved += OnPointerMoved;
        PointerReleased += OnPointerReleased;

        // OverlayWindow is resolved from DI after MainWindow/MainWindowViewModel, so Rocket
        // League may already have been detected (or the overlay already made visible) before
        // this constructor ran and subscribed. Sync to the current state directly instead of
        // only reacting to future events, and drive auto-show from here rather than relying
        // on another class to have already called ShowAsync() first.
        SyncVisibility();
        if (_rocketLeagueService?.IsConnected == true)
        {
            OnRocketLeagueConnectionChanged(this, true);
        }
    }

    private void OnOpened(object? sender, EventArgs e)
    {
        ApplyPosition();
        ApplyAppearance();
        ApplyStatusText(_rocketLeagueService?.IsConnected ?? false);

        var settings = _settingsService?.Current;
        if (settings is not null)
        {
            settings.PropertyChanged += OnSettingsPropertyChanged;
        }
    }

    private void OnSettingsPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(Core.Models.AppSettings.OverlayOpacity)
            or nameof(Core.Models.AppSettings.OverlayScale))
        {
            Dispatcher.UIThread.Post(ApplyAppearance);
        }
    }

    private void ApplyPosition()
    {
        var settings = _settingsService?.Current;
        if (settings is null)
        {
            return;
        }

        Position = new PixelPoint((int)settings.OverlayPositionX, (int)settings.OverlayPositionY);
    }

    private void ApplyAppearance()
    {
        var settings = _settingsService?.Current;
        if (settings is null)
        {
            return;
        }

        Opacity = settings.OverlayOpacity;
        RenderTransform = new ScaleTransform(settings.OverlayScale, settings.OverlayScale);
    }

    private void ApplyStatusText(bool isConnected)
    {
        if (StatusTextBlock is null)
        {
            return;
        }

        StatusTextBlock.Text = isConnected
            ? "Rocket League detected"
            : "Waiting for Rocket League...";
    }

    private void OnOverlayVisibilityChanged(object? sender, EventArgs e)
    {
        Dispatcher.UIThread.Post(SyncVisibility);
    }

    private void SyncVisibility()
    {
        if (_overlayService is null)
        {
            return;
        }

        if (_overlayService.IsVisible)
        {
            ApplyPosition();
            ApplyAppearance();
            Show();
        }
        else
        {
            Hide();
        }
    }

    private void OnRocketLeagueConnectionChanged(object? sender, bool isConnected)
    {
        Dispatcher.UIThread.Post(() =>
        {
            ApplyStatusText(isConnected);

            if (_overlayService is null || _settingsService?.Current.AutoDetectRocketLeague != true)
            {
                return;
            }

            _ = isConnected ? _overlayService.ShowAsync() : _overlayService.HideAsync();
        });
    }

    private void OnToggleOverlayRequested(object? sender, EventArgs e)
    {
        Dispatcher.UIThread.Post(() => _ = _overlayService?.ToggleAsync());
    }

    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (!e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            return;
        }

        _isDragging = true;
        _dragStartPointerPosition = e.GetPosition(this);
        _dragStartWindowPosition = Position;
        e.Pointer.Capture(this);
    }

    private void OnPointerMoved(object? sender, PointerEventArgs e)
    {
        if (!_isDragging)
        {
            return;
        }

        var currentPointerPosition = e.GetPosition(this);
        var deltaX = currentPointerPosition.X - _dragStartPointerPosition.X;
        var deltaY = currentPointerPosition.Y - _dragStartPointerPosition.Y;

        Position = new PixelPoint(
            _dragStartWindowPosition.X + (int)deltaX,
            _dragStartWindowPosition.Y + (int)deltaY);
    }

    private async void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (!_isDragging)
        {
            return;
        }

        _isDragging = false;
        e.Pointer.Capture(null);

        if (_settingsService is null)
        {
            return;
        }

        _settingsService.Current.OverlayPositionX = Position.X;
        _settingsService.Current.OverlayPositionY = Position.Y;
        await _settingsService.SaveAsync();
    }
}
