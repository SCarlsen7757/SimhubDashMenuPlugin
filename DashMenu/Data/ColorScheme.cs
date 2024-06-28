namespace DashMenu.Data
{
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
}
