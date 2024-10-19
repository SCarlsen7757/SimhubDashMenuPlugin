using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DashMenu.Settings
{
    internal class Alert : IAlert, INotifyPropertyChanged
    {
        [JsonIgnore]
        public string Description { get; internal set; }
        [JsonIgnore]
        public string FullName { get; internal set; }
        [JsonIgnore]
        public string Name { get; internal set; }
        [JsonIgnore]
        public string Namespace { get; internal set; }

        private bool enabled = true;
        public bool Enabled
        {
            get => enabled;
            set
            {
                if (enabled == value) return;
                enabled = value;
                OnPropertyChanged();
            }
        }

        private TimeSpan time = TimeSpan.FromMilliseconds(700);

        public TimeSpan ShowTimeDuration
        {
            get => time;
            set
            {
                if (time == value) return;
                time = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    }
}
