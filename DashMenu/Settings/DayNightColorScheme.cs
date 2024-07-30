using DashMenu.Data;

namespace DashMenu.Settings
{
    internal class DayNightColorScheme
    {
        public ColorSchemePropertyOverride DayModeColor { get; set; } = new ColorSchemePropertyOverride();
        public ColorSchemePropertyOverride NightModeColor { get; set; } = new ColorSchemePropertyOverride();
        public DayNightColorScheme() { }
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
    }
}
