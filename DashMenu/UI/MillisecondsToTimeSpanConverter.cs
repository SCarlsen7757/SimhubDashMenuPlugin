using System;
using System.Globalization;
using System.Windows.Data;

namespace DashMenu.UI
{
    public class MillisecondsToTimeSpanConverter : IValueConverter
    {
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double milliseconds)
            {
                return TimeSpan.FromMilliseconds(milliseconds);
            }
            return TimeSpan.Zero; // Default value if the input is not valid
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TimeSpan timeSpan)
            {
                return timeSpan.TotalMilliseconds; // Convert TimeSpan back to milliseconds
            }
            return 0; // Default value if the input is not valid
        }
    }
}
