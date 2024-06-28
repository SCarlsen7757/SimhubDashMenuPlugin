using DashMenu.Data;

namespace DashMenu.Settings
{
    public class Fields
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

        /// <summary>
        /// Class to override property.
        /// </summary>
        /// <typeparam name="T">Type of the property value.</typeparam>
        public class PropertyOverride<T>
        {
            /// <summary>
            /// Override the property.
            /// </summary>
            public bool Override { get; set; } = false;
            /// <summary>
            /// Default value of the property.
            /// </summary>
            public T DefaultValue { get; set; } = default(T);
            /// <summary>
            /// Override value of the property.
            /// </summary>
            public T OverrideValue { get; set; } = default(T);
        }
    }
}
