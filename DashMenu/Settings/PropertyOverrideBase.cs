namespace DashMenu.Settings
{
    /// <summary>
    /// Class to override property.
    /// </summary>
    /// <typeparam name="T">Type of the property value.</typeparam>
    internal class PropertyOverride<T>
    {
        /// <summary>
        /// Default value of the property.
        /// </summary>
        public T DefaultValue { get; set; } = default(T);
        /// <summary>
        /// Override the property.
        /// </summary>
        public bool Override { get; set; } = false;
        /// <summary>
        /// Override value of the property.
        /// </summary>
        public T OverrideValue { get; set; } = default(T);
    }
}