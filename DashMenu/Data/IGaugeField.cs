namespace DashMenu.Data
{
    public interface IGaugeField : IDataField
    {
        bool IsRangeLocked { get; set; }
        string Maximum { get; set; }
        string Minimum { get; set; }
        bool IsStepLocked { get; set; }
        string Step { get; set; }
    }
}
