namespace DashMenu.Settings
{
    internal interface IDataField : IBasicSettings
    {
        bool Enabled { get; set; }
        bool GameSupported { get; }
        bool IsDecimal { get; set; }
        DataField.OverrideProperties Override { get; set; }
        string SupportedGames { get; }
    }
}