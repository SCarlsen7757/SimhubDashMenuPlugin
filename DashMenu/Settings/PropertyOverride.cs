using Newtonsoft.Json;
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
        public PropertyOverride() { }
        public PropertyOverride(T defaultValue)
        {
            DefaultValue = defaultValue;
            OverrideValue = defaultValue;
        }
        private T defaultValue = default;
        /// <summary>
        /// Default value of the property.
        /// </summary>
        [JsonIgnore]
        public T DefaultValue
        {
            get => defaultValue;
            set
            {
                if (EqualityComparer<T>.Default.Equals(defaultValue, value)) return;
                defaultValue = value;
            }
        }
        private bool @override = false;
        /// <summary>
        /// Override the property.
        /// </summary>
        public bool Override
        {
            get => @override;
            set
            {
                if (@override == value) return;
                @override = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// Override value of the property.
        /// </summary>
        private T overrideValue = default;
        public T OverrideValue
        {
            get => overrideValue;
            set
            {
                if (EqualityComparer<T>.Default.Equals(overrideValue, value)) return;
                overrideValue = value;
                OnPropertyChanged();
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    }
}