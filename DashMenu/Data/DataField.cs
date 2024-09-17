namespace DashMenu.Data
{
    /// <summary>
    /// Data type to be used in the dash. 
    /// </summary>
    public class DataField : IDataField
    {
        /// <summary>
        /// Name to be shown in the data field.
        /// </summary>
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// Value to be shown in the data field.
        /// </summary>
        public string Value { get; set; } = "-";
        /// <summary>
        /// Unit of the value.
        /// </summary>
        public string Unit { get; set; } = string.Empty;
        /// <summary>
        /// Is value a Decimal number
        /// </summary>
        public bool IsDecimalNumber { get; set; } = false;
        /// <summary>
        /// Number of decimal of the value is a decimal number.
        /// </summary>
        public int Decimal { get; set; } = 0;
        /// <summary>
        /// Color to be shown in the data field.
        /// </summary>
        public ColorScheme Color { get; set; } = new ColorScheme();
    }
}
