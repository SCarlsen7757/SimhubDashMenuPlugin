using Newtonsoft.Json;
using System.ComponentModel;

namespace DashMenu.Settings
{
    internal class DataField : BasicSettings, INotifyPropertyChanged, IDataField
    {
        public DataField()
        {
            Override = new OverrideProperties(this);
        }

        private bool isDecimal = false;

        [JsonIgnore]
        public bool IsDecimal
        {
            get => isDecimal;
            set
            {
                if (isDecimal == value) return;
                isDecimal = value;
            }
        }

        public OverrideProperties Override { get; set; }

        public class OverrideProperties
        {
            public OverrideProperties()
            {
                Name.PropertyChanged += Name_PropertyChanged;
                Decimal.PropertyChanged += Decimal_PropertyChanged;

                DayNightColorScheme.DayModeColor.PropertyChanged += ColorScheme_PropertyChanged;
                DayNightColorScheme.DayModeColor.OverrideValue.PropertyChanged += ColorScheme_PropertyChanged;
                DayNightColorScheme.NightModeColor.PropertyChanged += ColorScheme_PropertyChanged;
                DayNightColorScheme.NightModeColor.OverrideValue.PropertyChanged += ColorScheme_PropertyChanged;
            }

            public OverrideProperties(DataField parent) : this()
            {
                this.parent = parent;
            }

            [JsonIgnore]
            internal readonly DataField parent;

            public PropertyOverride<string> Name { get; } = new PropertyOverride<string>();

            public PropertyOverride<int> Decimal { get; } = new PropertyOverride<int>();

            public DayNightColorScheme DayNightColorScheme { get; } = new DayNightColorScheme();

            public event PropertyChangedEventHandler NamePropertyChanged;
            public event PropertyChangedEventHandler DecimalPropertyChanged;
            public event PropertyChangedEventHandler ColorSchemePropertyChanged;

            private void Name_PropertyChanged(object sender, PropertyChangedEventArgs e) => NamePropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Name)));

            private void Decimal_PropertyChanged(object sender, PropertyChangedEventArgs e) => DecimalPropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Decimal)));

            private void ColorScheme_PropertyChanged(object sender, PropertyChangedEventArgs e) => ColorSchemePropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DayNightColorScheme)));
        }
    }
}