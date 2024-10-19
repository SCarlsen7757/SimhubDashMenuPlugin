namespace DashMenu.Settings
{
    internal interface IGaugeField : IDataField
    {
        bool IsRangeLocked { get; set; }
        bool IsStepLocked { get; set; }
        new GaugeField.OverrideProperties Override { get; set; }
    }
}