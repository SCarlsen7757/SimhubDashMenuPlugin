using DashMenu.Data;

namespace DashMenu.Settings
{
    internal class DayNightColorScheme
    {
        public PropertyOverride<ColorScheme> DayModeColor { get; set; } = default;
        public PropertyOverride<ColorScheme> NightModeColor { get; set; } = default;
        public DayNightColorScheme() { }
        public DayNightColorScheme(ColorScheme defaultColor)
        {
            DayModeColor.DefaultValue = defaultColor;
            NightModeColor.DefaultValue = defaultColor;
        }
        public DayNightColorScheme(ColorScheme defaultDayColor, ColorScheme defaultNightColor)
        {
            DayModeColor.DefaultValue = defaultDayColor;
            NightModeColor.DefaultValue = defaultNightColor;
        }
    }
}
