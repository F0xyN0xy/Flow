namespace Flow.Shared.Helpers;

/// <summary>
/// Provides common guard and validation utilities.
/// </summary>
public static class Ensure
{
    /// <summary>
    /// Throws <see cref="ArgumentNullException"/> if <paramref name="value"/> is null.
    /// </summary>
    public static void NotNull(object? value, string paramName)
    {
        ArgumentNullException.ThrowIfNull(value, paramName);
    }

    /// <summary>
    /// Throws <see cref="ArgumentException"/> if <paramref name="value"/> is null or whitespace.
    /// </summary>
    public static void NotNullOrWhiteSpace(string? value, string paramName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Value cannot be null or whitespace.", paramName);
        }
    }
}
