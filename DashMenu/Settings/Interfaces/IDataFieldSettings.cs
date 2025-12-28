namespace DashMenu.Settings.Interfaces
{
    internal interface IDataFieldSettings : IBasicSettings
    {
        bool IsDecimal { get; set; }
        DataField.OverrideProperties Override { get; set; }
    }
}