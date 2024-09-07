namespace DashMenu.Data
{
    public class GaugeField : DataField, IGaugeField
    {
        public GaugeField() : base() { }
        public bool IsRangeLocked { get; set; } = false;
        public string Maximum { get; set; } = 100.ToString();
        public string Minimum { get; set; } = 0.ToString();
        public bool IsStepLocked { get; set; } = false;
        public string Step { get; set; } = 0.ToString();
    }
}