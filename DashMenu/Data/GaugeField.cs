namespace DashMenu.Data
{
    public class GaugeField : DataField, IGaugeField
    {
        public string Maximum { get; set; } = 100.ToString();
        public string Minimum { get; set; } = 0.ToString();
        public string Step { get; set; } = 0.ToString();
    }
}
