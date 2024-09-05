namespace DashMenu.Data
{
    /// <summary>
    /// Interface for field data.
    /// </summary>
    public interface IDataField
    {
        ColorScheme Color { get; set; }
        int Decimal { get; set; }
        bool IsDecimalNumber { get; set; }
        string Name { get; set; }
        string Unit { get; set; }
        string Value { get; set; }
    }
}