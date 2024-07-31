using DashMenu.Data;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DashMenu.Settings
{
    internal class DayNightColorScheme : INotifyPropertyChanged
    {
        public DayNightColorScheme()
        {
            dayModeColor.PropertyChanged += Color_PropertyChanged;
            nightModeColor.PropertyChanged += Color_PropertyChanged;
        }

        private void Color_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(e.PropertyName);
        }

        private ColorSchemePropertyOverride dayModeColor = new ColorSchemePropertyOverride();
        public ColorSchemePropertyOverride DayModeColor
        {
            get => dayModeColor;
            set
            {
                if (dayModeColor == value) return;
                dayModeColor = value;
                OnPropertyChanged();
            }
        }
        private ColorSchemePropertyOverride nightModeColor = new ColorSchemePropertyOverride();
        public ColorSchemePropertyOverride NightModeColor
        {
            get => nightModeColor;
            set
            {
                if (nightModeColor == value) return;
                nightModeColor = value;
                OnPropertyChanged();
            }
        }
        public DayNightColorScheme(ColorScheme defaultColor)
        {
            DayModeColor.DefaultValue = defaultColor;
            DayModeColor.OverrideValue = defaultColor;
            NightModeColor.DefaultValue = defaultColor;
            NightModeColor.OverrideValue = defaultColor;
        }
        public DayNightColorScheme(ColorScheme defaultDayColor, ColorScheme defaultNightColor)
        {
            DayModeColor.DefaultValue = defaultDayColor;
            DayModeColor.OverrideValue = defaultDayColor;
            NightModeColor.DefaultValue = defaultNightColor;
            NightModeColor.OverrideValue = defaultNightColor;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
