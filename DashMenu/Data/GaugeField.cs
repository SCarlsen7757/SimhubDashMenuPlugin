namespace DashMenu.Data
{
    public class GaugeField : DataField, IGaugeField
    {
        public GaugeField() : base() { }
        public bool IsRangeLocked { get; set; } = false;

        private string maximum = 100.ToString();

        public string Maximum
        {
            get => maximum; set
            {
                if (maximum == value) return;
                maximum = value;
                OnPropertyChanged();
            }
        }

        private string minimum = 0.ToString();
        public string Minimum
        {
            get => minimum; set
            {
                if (minimum == value) return;
                minimum = value;
                OnPropertyChanged();
            }
        }

        public bool IsStepLocked { get; set; } = false;

        private string step = 0.ToString();
        public string Step
        {
            get => step; set
            {
                if (step == value) return;
                step = value;
                OnPropertyChanged();
            }
        }
    }
}