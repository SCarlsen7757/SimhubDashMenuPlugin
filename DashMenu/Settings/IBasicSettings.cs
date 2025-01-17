namespace DashMenu.Settings
{
    internal interface IBasicSettings
    {
        string FullName { get; }
        string Namespace { get; }
        string Name { get; }
        string Description { get; }
        bool Enabled { get; set; }
        bool GameSupported { get; }
        string SupportedGames { get; }
        bool Hide { get; }
    }
}