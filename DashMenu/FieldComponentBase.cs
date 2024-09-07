using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DashMenu
{
    internal abstract class FieldComponentBase
    {
        public bool enabled = true;
        private string fullName = string.Empty;
        public bool Enabled
        {
            get => enabled; set
            {
                if (enabled == value) return;
                enabled = value;
                OnPropertyChanged();
            }
        }
        public string FullName
        {
            get => fullName;
            set
            {
                if (fullName == value) return;
                fullName = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}