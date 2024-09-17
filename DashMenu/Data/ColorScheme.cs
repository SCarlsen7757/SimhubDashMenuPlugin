using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DashMenu.Data
{
    public class ColorScheme
    {
        public ColorScheme()
        {
            Primary = "808080";
            Accent = "000000";
        }
        public ColorScheme(string primary)
        {
            Primary = primary;
            Accent = "#000000";
        }
        public ColorScheme(string primary, string accent)
        {
            Primary = primary;
            Accent = accent;
        }
        // Copy constructor for deep copying
        public ColorScheme(ColorScheme other)
        {
            Primary = other.Primary;
            Accent = other.Accent;
        }
        private string primary = "#FFFFFF";
        /// <summary>
        /// Primary color.
        /// </summary>
        public string Primary
        {
            get => primary;
            set
            {
                var color = value.ToUpper();
                if (color == primary) return;
                primary = color;
                OnPropertyChanged();
            }
        }
        private string accent = "#000000";
        /// <summary>
        /// Accent color.
        /// </summary>
        public string Accent
        {
            get => accent;
            set
            {
                var color = value.ToUpper();
                if (color == accent) return;
                accent = color;
                OnPropertyChanged();
            }
        }
        public ColorScheme Clone()
        {
            return new ColorScheme(this); // Use the copy constructor
        }
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    }
}
