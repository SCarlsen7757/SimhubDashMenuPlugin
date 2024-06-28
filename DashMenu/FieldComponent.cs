using DashMenu.Data;
using DashMenu.Settings;

namespace DashMenu
{
    internal class FieldComponent : Settings.IFields
    {
        public FieldComponent(Data.IFieldDataComponent fieldData)
        {
            FieldData = fieldData;
            FullName = FieldData.GetType().FullName;
        }
        public string FullName { get; set; } = string.Empty;
        public bool Enabled { get; set; } = true;
        public Data.IFieldDataComponent FieldData { get; set; }
        private PropertyOverride<string> NameOverride { get; set; } = new PropertyOverride<string>();
        PropertyOverride<string> IFields.NameOverride { get => NameOverride; set { NameOverride = value; } }
        private PropertyOverride<ColorScheme> ColorSchemeOverride { get; set; } = new PropertyOverride<ColorScheme>();
        PropertyOverride<ColorScheme> IFields.ColorSchemeOverride { get => ColorSchemeOverride; set { ColorSchemeOverride = value; } }
        private PropertyOverride<int> DecimalOverride { get; set; } = new PropertyOverride<int>();
        PropertyOverride<int> IFields.DecimalOverride { get => DecimalOverride; set { DecimalOverride = value; } }
    }
}