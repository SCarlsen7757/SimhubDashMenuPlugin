using DashMenu.Data;
using DashMenu.Settings;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DashMenu
{
    internal class FieldComponent : Settings.IFields, INotifyPropertyChanged
    {
        public FieldComponent() { }
        public FieldComponent(Data.IFieldDataComponent fieldData)
        {
            FieldData = fieldData;
            FullName = FieldData.GetType().FullName;
            NameOverride.DefaultValue = FieldData.Data.Name;
            ColorSchemeOverride.DefaultValue = FieldData.Data.Color;
            DecimalOverride.DefaultValue = FieldData.Data.Decimal;
        }
        private string fullName = string.Empty;
        public string FullName
        {
            get => fullName;
            set
            {
                if (fullName == value) return;
                fullName = value;
                OnPropertyChanged();
            }
        }
        public bool enabled = true;
        public bool Enabled
        {
            get => enabled; set
            {
                if (enabled == value) return;
                enabled = value;
                OnPropertyChanged();
            }
        }
        public Data.IFieldDataComponent FieldData { get; set; }
        public PropertyOverride<string> NameOverride { get; set; } = new PropertyOverride<string>();
        PropertyOverride<string> IFields.NameOverride { get => NameOverride; set { NameOverride = value; } }
        public PropertyOverride<ColorScheme> ColorSchemeOverride { get; set; } = new PropertyOverride<ColorScheme>();
        PropertyOverride<ColorScheme> IFields.ColorSchemeOverride { get => ColorSchemeOverride; set { ColorSchemeOverride = value; } }
        public PropertyOverride<int> DecimalOverride { get; set; } = new PropertyOverride<int>();
        PropertyOverride<int> IFields.DecimalOverride { get => DecimalOverride; set { DecimalOverride = value; } }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    }
}