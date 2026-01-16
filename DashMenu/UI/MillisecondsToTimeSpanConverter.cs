using System;
using System.Globalization;
using System.Windows.Data;

namespace DashMenu.UI
{
    public sealed class MillisecondsToTimeSpanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TimeSpan timeSpan)
            {
                return timeSpan.TotalMilliseconds; // Convert TimeSpan to total milliseconds
            }
            return 0.0; // Default value if the input is not valid
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double milliseconds)
            {
                return TimeSpan.FromMilliseconds(milliseconds); // Convert milliseconds to TimeSpan
            }
            return TimeSpan.Zero; // Default value if the input is not valid
        }
    }
}
