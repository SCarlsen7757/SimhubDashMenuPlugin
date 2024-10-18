namespace DashMenu.Settings
{
    internal interface IBasicSettings
    {
        string Description { get; }
        string FullName { get; }
        string Name { get; }
        string Namespace { get; }
    }
}