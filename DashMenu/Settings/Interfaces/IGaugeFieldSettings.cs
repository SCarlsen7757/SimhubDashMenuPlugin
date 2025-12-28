namespace DashMenu.Settings.Interfaces
{
    internal interface IGaugeFieldSettings : IDataFieldSettings
    {
        bool IsRangeLocked { get; set; }
        bool IsStepLocked { get; set; }
        new GaugeField.OverrideProperties Override { get; set; }
    }
}