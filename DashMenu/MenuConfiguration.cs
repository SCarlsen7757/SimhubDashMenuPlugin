namespace DashMenu
{
    public class MenuConfiguration
    {
        /// <summary>
        /// When confiugration mode is true, it's possible to change the displayed field configuration.
        /// </summary>
        public bool ConfigurationMode { get; set; } = false;
        /// <summary>
        /// Active field in the dash menu configuration window.
        /// </summary>
        public int ActiveField { get; set; } = 0;
        /// <summary>
        /// Max data fields to be shown.
        /// </summary>
        public int MaxFields { get; set; } = 5;
    }
}
