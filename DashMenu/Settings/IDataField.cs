namespace DashMenu.Settings
{
    internal interface IDataField : IBasicSettings
    {
        bool IsDecimal { get; set; }
        DataField.OverrideProperties Override { get; set; }
    }
}