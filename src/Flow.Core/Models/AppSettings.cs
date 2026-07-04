using System.ComponentModel;
using System.Text.Json.Serialization;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Flow.Core.Models;

/// <summary>
/// Represents the user-configurable application settings.
/// </summary>
public partial class AppSettings : ObservableObject
{
    [ObservableProperty]
    [JsonPropertyName("theme")]
    [DefaultValue(AppTheme.Dark)]
    private AppTheme _theme = AppTheme.Dark;

    [ObservableProperty]
    [JsonPropertyName("accentColor")]
    [DefaultValue("#FF6B35")]
    private string _accentColor = "#FF6B35";

    [ObservableProperty]
    [JsonPropertyName("overlayOpacity")]
    [DefaultValue(0.85)]
    private double _overlayOpacity = 0.85;

    [ObservableProperty]
    [JsonPropertyName("overlayScale")]
    [DefaultValue(1.0)]
    private double _overlayScale = 1.0;

    [ObservableProperty]
    [JsonPropertyName("overlayPositionX")]
    [DefaultValue(100)]
    private double _overlayPositionX = 100;

    [ObservableProperty]
    [JsonPropertyName("overlayPositionY")]
    [DefaultValue(100)]
    private double _overlayPositionY = 100;

    [ObservableProperty]
    [JsonPropertyName("controllerButton")]
    [DefaultValue("RightStick")]
    private string _controllerButton = "RightStick";

    [ObservableProperty]
    [JsonPropertyName("hotkey")]
    [DefaultValue("F9")]
    private string _hotkey = "F9";

    [ObservableProperty]
    [JsonPropertyName("enabledWidgets")]
    private List<string> _enabledWidgets = [];

    [ObservableProperty]
    [JsonPropertyName("startWithWindows")]
    [DefaultValue(false)]
    private bool _startWithWindows;

    [ObservableProperty]
    [JsonPropertyName("minimizeToTray")]
    [DefaultValue(true)]
    private bool _minimizeToTray = true;

    [ObservableProperty]
    [JsonPropertyName("autoDetectRocketLeague")]
    [DefaultValue(true)]
    private bool _autoDetectRocketLeague = true;
}
