using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DashMenu.Data
{
    public class ColorScheme
    {
        public ColorScheme() { }
        public ColorScheme(string primary, string accent)
        {
            Primary = primary;
            Accent = accent;
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
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    }
}
