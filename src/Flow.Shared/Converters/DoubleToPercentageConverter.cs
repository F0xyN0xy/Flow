using System.Globalization;

namespace Flow.Shared.Converters;

/// <summary>
/// Converts a double value to a percentage string and back.
/// </summary>
public sealed class DoubleToPercentageConverter : Avalonia.Data.Converters.IValueConverter
{
    /// <inheritdoc />
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is double d)
        {
            return $"{d:P0}";
        }
        return value;
    }

    /// <inheritdoc />
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string s && double.TryParse(s.Replace("%", "").Trim(), out var result))
        {
            return result / 100.0;
        }
        return value;
    }
}
