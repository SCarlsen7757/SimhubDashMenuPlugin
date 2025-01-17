using Newtonsoft.Json;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DashMenu.Settings
{
    internal class BasicSettings : IBasicSettings
    {
        [JsonIgnore]
        public string Namespace { get; internal set; }

        [JsonIgnore]
        public string Name { get; internal set; }

        [JsonIgnore]
        public string FullName { get; internal set; }

        [JsonIgnore]
        public bool GameSupported { get; internal set; }

        [JsonIgnore]
        public string SupportedGames { get; internal set; }

        [JsonIgnore]
        public string Description { get; internal set; }

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

        private bool hide = false;

        public bool Hide
        {
            get => hide;
            set
            {
                if (hide == value) return;
                hide = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
