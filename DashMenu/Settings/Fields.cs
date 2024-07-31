using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DashMenu.Settings
{
    internal class Fields : INotifyPropertyChanged
    {
        public Fields()
        {
            DayNightColorScheme = new DayNightColorScheme();
            NameOverride.PropertyChanged += NameOverride_PropertyChanged;
            DecimalOverride.PropertyChanged += DecimalOverride_PropertyChanged;
            DayNightColorScheme.DayModeColor.PropertyChanged += ColorSchemeOverride_PropertyChanged;
            DayNightColorScheme.DayModeColor.OverrideValue.PropertyChanged += ColorSchemeOverride_PropertyChanged;
            DayNightColorScheme.NightModeColor.PropertyChanged += ColorSchemeOverride_PropertyChanged;
            DayNightColorScheme.NightModeColor.OverrideValue.PropertyChanged += ColorSchemeOverride_PropertyChanged;
        }
        private string fullName = string.Empty;
        /// <summary>
        /// Full name of the field class with namespace.
        /// </summary>
        public string FullName
        {
            get => fullName;
            set
            {
                if (value == fullName) return;
                fullName = value;
                OnPropertyChanged();
            }
        }
        private bool enabled = true;
        /// <summary>
        /// Is the field enabled.
        /// </summary>
        public bool Enabled
        {
            get => enabled;
            set
            {
                if (value == enabled) return;
                enabled = value;
                OnPropertyChanged();
            }
        }
        private bool isDecimal = false;
        /// <summary>
        /// Is field value a decimal number
        /// </summary>
        public bool IsDecimal
        {
            get => isDecimal;
            set
            {
                if (isDecimal == value) return;
                isDecimal = value;
                OnPropertyChanged();
            }
        }
        public PropertyOverride<string> NameOverride { get; set; } = new PropertyOverride<string>();
        public PropertyOverride<int> DecimalOverride { get; set; } = new PropertyOverride<int>();
        public DayNightColorScheme DayNightColorScheme { get; set; } = new DayNightColorScheme();

        public event PropertyChangedEventHandler PropertyChanged;
        public event PropertyChangedEventHandler NameOverridePropertyChanged;
        public event PropertyChangedEventHandler DecimalOverridePropertyChanged;
        public event PropertyChangedEventHandler ColorSchemeOverridePropertyChanged;

        internal void OnPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        private void OnNameOverridePropertyChanged(string propertyName) => NameOverridePropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        private void OnDecimalOveridePropertyChanged(string propertyName) => DecimalOverridePropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        private void OnColorSchemeOverridePropertyChanged(string propertyName) => ColorSchemeOverridePropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        private void NameOverride_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnNameOverridePropertyChanged(nameof(NameOverride));
        }
        private void DecimalOverride_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnDecimalOveridePropertyChanged(nameof(DecimalOverride));
        }
        private void ColorSchemeOverride_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnColorSchemeOverridePropertyChanged(nameof(DayNightColorScheme));
        }
    }
}