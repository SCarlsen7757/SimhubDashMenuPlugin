using Newtonsoft.Json;
using System.ComponentModel;


namespace DashMenu.Settings
{
    internal class GaugeField : DataField
    {
        public GaugeField() : base() { }
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

        new public class OverrideProperties : DataField.OverrideProperties
        {
            public OverrideProperties() : base()
            {
                Maximum.PropertyChanged += Maximum_PropertyChanged;
                Minimum.PropertyChanged += Minimum_PropertyChanged;
                Step.PropertyChanged += Step_PropertyChanged;
            }
            public PropertyOverride<string> Maximum { get; set; } = new PropertyOverride<string>();
            public PropertyOverride<string> Minimum { get; set; } = new PropertyOverride<string>();
            public PropertyOverride<string> Step { get; set; } = new PropertyOverride<string>();
            public event PropertyChangedEventHandler MaximumPropertyChanged;
            public event PropertyChangedEventHandler MinimumPropertyChanged;
            public event PropertyChangedEventHandler StepPropertyChanged;
            private void Maximum_PropertyChanged(object sender, PropertyChangedEventArgs e) => MaximumPropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Maximum)));
            private void Minimum_PropertyChanged(object sender, PropertyChangedEventArgs e) => MinimumPropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Minimum)));
            private void Step_PropertyChanged(object sender, PropertyChangedEventArgs e) => StepPropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Override.Step)));
        }
        new public OverrideProperties Override { get; set; } = new OverrideProperties();
    }
}
