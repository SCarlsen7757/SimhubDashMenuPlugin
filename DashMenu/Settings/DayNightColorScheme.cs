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
            DayModeColor.DefaultValue = defaultColor.Clone();
            DayModeColor.OverrideValue = defaultColor.Clone();
            NightModeColor.DefaultValue = defaultColor.Clone();
            NightModeColor.OverrideValue = defaultColor.Clone();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
