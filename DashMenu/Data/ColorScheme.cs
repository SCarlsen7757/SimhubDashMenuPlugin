namespace DashMenu.Data
{
    public class ColorScheme
    {
        public ColorScheme(string primary, string accent)
        {
            Primary = primary;
            Accent = accent;
        }
        private string primary = "#FFFFFF";
        /// <summary>
        /// Primary color.
        /// </summary>
        public string Primary { get => primary; set => primary = value.ToUpper(); }
        private string accent = "#000000";
        /// <summary>
        /// Accent color.
        /// </summary>
        public string Accent { get => accent; set => accent = value.ToUpper(); }
    }
}
