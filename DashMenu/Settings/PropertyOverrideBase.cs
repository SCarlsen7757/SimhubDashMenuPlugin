using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DashMenu.Settings
{
    /// <summary>
    /// Class to override property.
    /// </summary>
    /// <typeparam name="T">Type of the property value.</typeparam>
    internal class PropertyOverride<T> : INotifyPropertyChanged
    {
        private T defaultValue = default;
        /// <summary>
        /// Default value of the property.
        /// </summary>
        public T DefaultValue
        {
            get => defaultValue;
            set
            {
                if (EqualityComparer<T>.Default.Equals(defaultValue, value)) return;
                defaultValue = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// Override the property.
        /// </summary>
        public bool Override { get; set; } = false;
        /// <summary>
        /// Override value of the property.
        /// </summary>
        public T OverrideValue { get; set; } = default;
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    }
}