using DashMenu.Data;

namespace DashMenu.Settings
{
    internal class Fields : IFields
    {
        /// <summary>
        /// Full name of the field class with namespace.
        /// </summary>
        public string FullName { get; set; }
        /// <summary>
        /// Is the field enabled.
        /// </summary>
        public bool Enabled { get; set; } = true;

        public PropertyOverride<string> NameOverride { get; set; } = new PropertyOverride<string>();
        public PropertyOverride<int> DecimalOverride { get; set; } = new PropertyOverride<int>();
        public PropertyOverride<ColorScheme> ColorSchemeOverride { get; set; } = new PropertyOverride<ColorScheme>();
    }
}