using System.Text.Json;
using Flow.Core.Models;
using Flow.Core.Services;
using Microsoft.Extensions.Logging;

namespace Flow.Storage.Services;

/// <summary>
/// Implements <see cref="ISettingsService"/> using JSON file persistence.
/// </summary>
public sealed class SettingsService : ISettingsService
{
    private readonly ILogger<SettingsService> _logger;
    private readonly string _settingsPath;
    private readonly JsonSerializerOptions _jsonOptions;

    /// <inheritdoc />
    /// <remarks>
    /// This reference never changes after construction. Other services (the overlay
    /// window, the input service) subscribe to <c>Current.PropertyChanged</c> once and
    /// rely on that subscription staying valid for the app's lifetime, so <see cref="LoadAsync"/>
    /// and <see cref="ResetToDefaults"/> copy values into this instance instead of replacing it.
    /// </remarks>
    public AppSettings Current { get; } = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="SettingsService"/> class.
    /// </summary>
    public SettingsService(ILogger<SettingsService> logger)
    {
        _logger = logger;

        var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var flowPath = Path.Combine(appData, "Flow");
        Directory.CreateDirectory(flowPath);
        _settingsPath = Path.Combine(flowPath, "settings.json");

        _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    /// <inheritdoc />
    public async Task LoadAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            if (!File.Exists(_settingsPath))
            {
                _logger.LogInformation("Settings file not found at {Path}. Using defaults.", _settingsPath);
                return;
            }

            await using var stream = File.OpenRead(_settingsPath);
            var loaded = await JsonSerializer.DeserializeAsync<AppSettings>(stream, _jsonOptions, cancellationToken);

            if (loaded is not null)
            {
                CopyInto(Current, loaded);
            }

            _logger.LogInformation("Settings loaded successfully from {Path}.", _settingsPath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load settings. Using defaults.");
        }
    }

    /// <inheritdoc />
    public async Task SaveAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await using var stream = File.Create(_settingsPath);
            await JsonSerializer.SerializeAsync(stream, Current, _jsonOptions, cancellationToken);
            _logger.LogInformation("Settings saved successfully to {Path}.", _settingsPath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save settings to {Path}.", _settingsPath);
        }
    }

    /// <inheritdoc />
    public void ResetToDefaults()
    {
        CopyInto(Current, new AppSettings());
        _logger.LogInformation("Settings reset to defaults.");
    }

    /// <summary>
    /// Copies every setting from <paramref name="source"/> onto <paramref name="target"/>
    /// property-by-property, so <paramref name="target"/>'s identity (and anyone subscribed
    /// to its <see cref="AppSettings.PropertyChanged"/>) is preserved.
    /// </summary>
    private static void CopyInto(AppSettings target, AppSettings source)
    {
        target.Theme = source.Theme;
        target.AccentColor = source.AccentColor;
        target.OverlayOpacity = source.OverlayOpacity;
        target.OverlayScale = source.OverlayScale;
        target.OverlayPositionX = source.OverlayPositionX;
        target.OverlayPositionY = source.OverlayPositionY;
        target.ControllerButton = source.ControllerButton;
        target.Hotkey = source.Hotkey;
        target.EnabledWidgets = source.EnabledWidgets;
        target.StartWithWindows = source.StartWithWindows;
        target.MinimizeToTray = source.MinimizeToTray;
        target.AutoDetectRocketLeague = source.AutoDetectRocketLeague;
    }
}
