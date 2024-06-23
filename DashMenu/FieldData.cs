namespace DashMenu
{
    /// <summary>
    /// Data type to be used in the dash. 
    /// </summary>
    public class FieldData
    {
        //TODO : Make it possible to override the Name and Color values from UI.

        public class ColorScheme
        {
            public ColorScheme(string primary, string accent)
            {
                Primary = primary;
                Accent = accent;
            }

            /// <summary>
            /// Primary color.
            /// </summary>
            public string Primary { get; set; } = "#ffffff";
            /// <summary>
            /// Accent color.
            /// </summary>
            public string Accent { get; set; } = "#000000";
        }

        /// <summary>
        /// Name to be shown in the data field.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Value to be shown in the data field.
        /// </summary>
        public string Value { get; set; } = "-";
        /// <summary>
        /// Unit of the value.
        /// </summary>
        public string Unit { get; set; } = string.Empty;
        /// <summary>
        /// Number of decimal of the value is a decimal number.
        /// </summary>
        public int Decimal { get; set; } = 0;
        /// <summary>
        /// Color to be shown in the data field.
        /// </summary>
        public ColorScheme Color { get; set; }
    }
}
