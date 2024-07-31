using DashMenu.Data;

namespace DashMenu.Settings
{
    internal class ColorSchemePropertyOverride : PropertyOverride<ColorScheme>
    {
        public ColorSchemePropertyOverride()
        {
            DefaultValue = new ColorScheme();
            OverrideValue = new ColorScheme();
        }
    }
}
