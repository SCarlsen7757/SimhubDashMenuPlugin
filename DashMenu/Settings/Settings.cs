using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DashMenu.Settings
{
    internal class Settings : INotifyPropertyChanged
    {
        //TODO: Add INotifyPropertyChanged to settings class, to make the UI work properly.
        private int maxFields = 5;
        /// <summary>
        /// Max amount of fields that can be displayed.
        /// </summary>
        internal int MaxFields
        {
            get => maxFields; set
            {
                if (value == maxFields) return;
                maxFields = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// Fields displayed.
        /// </summary>
        internal string[] DisplayedFields { get; set; }
        /// <summary>
        /// All fields. Used for enabling and disabling the fields to be able to select them.
        /// </summary>
        internal List<IFields> Fields { get; set; } = new List<IFields>();

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    }
}
