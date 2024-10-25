using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;

namespace DashMenu.Data
{
    public class ColorScheme
    {
        public ColorScheme() { }
        public ColorScheme(string primary)
        {
            Primary = primary;
        }
        public ColorScheme(string primary, string accent) : this(primary)
        {
            Accent = accent;
        }
        // Copy constructor for deep copying
        public ColorScheme(ColorScheme other)
        {
            Primary = other.Primary;
            Accent = other.Accent;
        }
        private string primary = "#808080";
        /// <summary>
        /// Primary color.
        /// </summary>
        public string Primary
        {
            get => primary;
            set
            {
                string color = ColorStringValid(value) ? value : defaultColor.ToString();
                if (color == primary) return;
                primary = color;
                OnPropertyChanged();
            }
        }
        private string accent = "#ffffff";
        /// <summary>
        /// Accent color.
        /// </summary>
        public string Accent
        {
            get => accent;
            set
            {
                string color = ColorStringValid(value) ? value : defaultColor.ToString();
                if (color == accent) return;
                accent = color;
                OnPropertyChanged();
            }
        }
        public ColorScheme Clone()
        {
            return new ColorScheme(this); // Use the copy constructor
        }

        private static Color defaultColor = Color.FromRgb(255, 255, 255);

        private static bool ColorStringValid(string value)
        {
            try
            {
                ColorConverter.ConvertFromString(value);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    }
}
