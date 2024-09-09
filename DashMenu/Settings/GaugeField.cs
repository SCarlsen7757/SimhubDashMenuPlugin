using Newtonsoft.Json;
using System.ComponentModel;


namespace DashMenu.Settings
{
    internal class GaugeField : DataField
    {
        public GaugeField() : base()
        {
            MaximumOverride = new PropertyOverride<string>();
            MaximumOverride.PropertyChanged += Maximum_PropertyChanged;

            MinimumOverride = new PropertyOverride<string>();
            MinimumOverride.PropertyChanged += Minimum_PropertyChanged;

            StepOverride = new PropertyOverride<string>();
            StepOverride.PropertyChanged += Step_PropertyChanged;
        }
        private bool isRangeLocked = true;
        /// <summary>
        /// Is the value range, maximum and minimum locked. Determined by the extension.
        /// </summary>
        [JsonIgnore]
        public bool IsRangeLocked
        {
            get => isRangeLocked;
            set
            {
                if (isRangeLocked == value) return;
                isRangeLocked = value;
                OnPropertyChanged();
            }
        }
        public PropertyOverride<string> MaximumOverride { get; set; }
        public PropertyOverride<string> MinimumOverride { get; set; }
        private bool isStepLocked = true;
        /// <summary>
        /// Is the step value locked. Determined by the extension.
        /// </summary>
        [JsonIgnore]
        public bool IsStepLocked
        {
            get => isStepLocked;
            set
            {
                if (isStepLocked == value) return;
                isStepLocked = value;
                OnPropertyChanged();
            }
        }
        public PropertyOverride<string> StepOverride { get; set; }

        public event PropertyChangedEventHandler MaximumOverridePropertyChanged;
        public event PropertyChangedEventHandler MinimumOverridePropertyChanged;
        public event PropertyChangedEventHandler StepOverridePropertyChanged;

        private void Maximum_PropertyChanged(object sender, PropertyChangedEventArgs e) => MaximumOverridePropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MaximumOverride)));
        private void Minimum_PropertyChanged(object sender, PropertyChangedEventArgs e) => MinimumOverridePropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MinimumOverride)));
        private void Step_PropertyChanged(object sender, PropertyChangedEventArgs e) => StepOverridePropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(StepOverride)));
    }
}
