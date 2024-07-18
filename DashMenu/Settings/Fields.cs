using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DashMenu.Settings
{
    internal class Fields : INotifyPropertyChanged
    {
        private string fullName = string.Empty;
        /// <summary>
        /// Full name of the field class with namespace.
        /// </summary>
        public string FullName
        {
            get => fullName; set
            {
                if (value == fullName) return;
                fullName = value;
                OnPropertyChanged();
            }
        }
        private bool enabled = true;
        /// <summary>
        /// Is the field enabled.
        /// </summary>
        public bool Enabled
        {
            get => enabled; set
            {
                if (value == enabled) return;
                enabled = value;
                OnPropertyChanged();
            }
        }

        public PropertyOverride<string> NameOverride { get; set; } = new PropertyOverride<string>();
        public PropertyOverride<int> DecimalOverride { get; set; } = new PropertyOverride<int>();
        public DayNightColorScheme DayNightColorScheme { get; set; } = new DayNightColorScheme();

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    }
}