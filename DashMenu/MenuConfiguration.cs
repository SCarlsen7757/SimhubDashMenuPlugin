namespace DashMenu
{
    internal class MenuConfiguration
    {
        /// <summary>
        /// When confiugration mode is true, it's possible to change the displayed field configuration.
        /// </summary>
        internal bool ConfigurationMode { get; set; } = false;
        /// <summary>
        /// Active field in the dash menu configuration window.
        /// </summary>
        internal int ActiveField { get; set; } = 0;
        /// <summary>
        /// Max data fields to be shown.
        /// </summary>
        internal int MaxFields { get; set; } = 5;
    }
}
