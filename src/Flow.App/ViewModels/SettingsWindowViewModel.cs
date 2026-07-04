using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Flow.Core.Models;
using Flow.Core.Services;
using Microsoft.Extensions.Logging;

namespace Flow.App.ViewModels;

/// <summary>
/// ViewModel for the settings window.
/// </summary>
public partial class SettingsWindowViewModel : ObservableObject
{
    private readonly ILogger<SettingsWindowViewModel> _logger;
    private readonly ISettingsService _settingsService;

    /// <summary>
    /// Gets the current application settings.
    /// </summary>
    public AppSettings Settings => _settingsService.Current;

    /// <summary>
    /// Initializes a new instance of the <see cref="SettingsWindowViewModel"/> class.
    /// </summary>
    public SettingsWindowViewModel(
        ILogger<SettingsWindowViewModel> logger,
        ISettingsService settingsService)
    {
        _logger = logger;
        _settingsService = settingsService;
    }

    /// <summary>
    /// Saves the current settings.
    /// </summary>
    [RelayCommand]
    private async Task Save()
    {
        try
        {
            await _settingsService.SaveAsync();
            _logger.LogInformation("Settings saved successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save settings.");
        }
    }

    /// <summary>
    /// Resets settings to their default values.
    /// </summary>
    [RelayCommand]
    private void ResetToDefaults()
    {
        _settingsService.ResetToDefaults();
        OnPropertyChanged(string.Empty);
        _logger.LogInformation("Settings reset to defaults.");
    }
}
