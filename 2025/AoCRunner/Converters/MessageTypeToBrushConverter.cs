using System;
using System.Globalization;
using AoCRunner.Models;
using Avalonia;
using Avalonia.Data.Converters;
using Avalonia.Media;
using Helpers.Logging;

namespace AoCRunner.Converters;

public class MessageTypeToBrushConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not MessageType type)
        {
            return AvaloniaProperty.UnsetValue;
        }

        var app = Application.Current!;
        var key = type switch
        {
            MessageType.Error => "ErrorBrush",
            MessageType.Warning => "WarningBrush",
            _ => "TextBrush"
        };

        if (app.TryGetResource(key, app.ActualThemeVariant, out var res) && res is IBrush brush)
        {
            return brush;
        }

        throw new InvalidOperationException($"Did not find brush with key {key}");
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}