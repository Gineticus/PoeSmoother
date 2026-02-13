using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace PoeSmoother;

public class HexToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string hex && !string.IsNullOrEmpty(hex))
        {
            try
            {
                // Remove '#' if present
                hex = hex.TrimStart('#');

                // Parse ARGB if 8 chars, else RGB with full alpha
                byte a = 255;
                byte r, g, b;

                if (hex.Length == 8)
                {
                    a = byte.Parse(hex.Substring(0, 2), NumberStyles.HexNumber);
                    r = byte.Parse(hex.Substring(2, 2), NumberStyles.HexNumber);
                    g = byte.Parse(hex.Substring(4, 2), NumberStyles.HexNumber);
                    b = byte.Parse(hex.Substring(6, 2), NumberStyles.HexNumber);
                }
                else if (hex.Length == 6)
                {
                    r = byte.Parse(hex.Substring(0, 2), NumberStyles.HexNumber);
                    g = byte.Parse(hex.Substring(2, 2), NumberStyles.HexNumber);
                    b = byte.Parse(hex.Substring(4, 2), NumberStyles.HexNumber);
                }
                else
                {
                    return Colors.Transparent;
                }

                return System.Windows.Media.Color.FromArgb(a, r, g, b);
            }
            catch
            {
                return Colors.Transparent;
            }
        }
        return Colors.Transparent;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is System.Windows.Media.Color color)
        {
            return $"#{color.A:X2}{color.R:X2}{color.G:X2}{color.B:X2}";
        }
        return string.Empty;
    }
}