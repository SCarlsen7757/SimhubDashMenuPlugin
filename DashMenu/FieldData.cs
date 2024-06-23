namespace DashMenu
{
    /// <summary>
    /// Data type to be used in the dash. 
    /// </summary>
    public class FieldData
    {
        //TODO : Make it possible to override the Name and Color values from UI.

        /// <summary>
        /// Name to be shown in the data field.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Value to be shown in the data field.
        /// </summary>
        public string Value { get; set; }
        /// <summary>
        /// Color to be shown in the data field.
        /// </summary>
        public string Color { get; set; }
    }
}
