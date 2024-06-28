using DashMenu.Data;

namespace DashMenu.Settings
{
    internal interface IFields
    {
        string FullName { get; set; }
        bool Enabled { get; set; }
        PropertyOverride<string> NameOverride { get; set; }
        PropertyOverride<ColorScheme> ColorSchemeOverride { get; set; }
        PropertyOverride<int> DecimalOverride { get; set; }
    }
}