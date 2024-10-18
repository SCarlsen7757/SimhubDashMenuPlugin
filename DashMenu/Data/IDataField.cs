using System.ComponentModel;

namespace DashMenu.Data
{
    public interface IDataField : INotifyPropertyChanged
    {
        ColorScheme Color { get; set; }
        int Decimal { get; set; }
        bool IsDecimalNumber { get; set; }
        string Name { get; set; }
        string Unit { get; set; }
        string Value { get; set; }
    }
}