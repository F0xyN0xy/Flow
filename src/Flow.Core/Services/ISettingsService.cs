namespace Flow.Core.Services;

/// <summary>
/// Provides access to application settings with load/save capabilities.
/// </summary>
public interface ISettingsService
{
    /// <summary>
    /// Gets the current application settings.
    /// </summary>
    Models.AppSettings Current { get; }

    /// <summary>
    /// Loads settings from persistent storage asynchronously.
    /// </summary>
    Task LoadAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Saves the current settings to persistent storage asynchronously.
    /// </summary>
    Task SaveAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Resets settings to their default values.
    /// </summary>
    void ResetToDefaults();
}
