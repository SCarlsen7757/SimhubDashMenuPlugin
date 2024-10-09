using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
namespace DashMenu.UI
{
    public class ColorStringToColorConverter : IValueConverter
    {
        private static Color defaultColor = Color.FromRgb(255, 255, 255);
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Color color)
            {
                // Convert Color to Hex string
                return color.ToString();
            }
            return defaultColor.ToString();
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string colorString && ColorConverter.ConvertFromString(colorString) is Color color)
            {
                // Convert Hex string back to Color
                return color;
            }
            return defaultColor;
        }
    }
}
