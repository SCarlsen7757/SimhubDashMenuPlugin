using Newtonsoft.Json;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DashMenu.Settings
{
    internal class DataField : INotifyPropertyChanged
    {
        public DataField()
        {
            NameOverride = new PropertyOverride<string>();
            NameOverride.PropertyChanged += NameOverride_PropertyChanged;

            DecimalOverride = new PropertyOverride<int>();
            DecimalOverride.PropertyChanged += DecimalOverride_PropertyChanged;

            DayNightColorScheme = new DayNightColorScheme();
            DayNightColorScheme.DayModeColor.PropertyChanged += ColorSchemeOverride_PropertyChanged;
            DayNightColorScheme.DayModeColor.OverrideValue.PropertyChanged += ColorSchemeOverride_PropertyChanged;
            DayNightColorScheme.NightModeColor.PropertyChanged += ColorSchemeOverride_PropertyChanged;
            DayNightColorScheme.NightModeColor.OverrideValue.PropertyChanged += ColorSchemeOverride_PropertyChanged;
        }
        [JsonIgnore]
        public string Namespace { get; internal set; }
        [JsonIgnore]
        public string Name { get; internal set; }
        [JsonIgnore]
        public string FullName { get; internal set; }
        [JsonIgnore]
        public bool GameSupported { get; internal set; }
        [JsonIgnore]
        public string SupportedGames { get; internal set; }
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
        [JsonIgnore]
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
        public PropertyOverride<string> NameOverride { get; set; }
        public PropertyOverride<int> DecimalOverride { get; set; }
        public DayNightColorScheme DayNightColorScheme { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        public event PropertyChangedEventHandler NameOverridePropertyChanged;
        public event PropertyChangedEventHandler DecimalOverridePropertyChanged;
        public event PropertyChangedEventHandler ColorSchemeOverridePropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        private void NameOverride_PropertyChanged(object sender, PropertyChangedEventArgs e) => NameOverridePropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(NameOverride)));
        private void DecimalOverride_PropertyChanged(object sender, PropertyChangedEventArgs e) => DecimalOverridePropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DecimalOverride)));
        private void ColorSchemeOverride_PropertyChanged(object sender, PropertyChangedEventArgs e) => ColorSchemeOverridePropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DayNightColorScheme)));
    }
}