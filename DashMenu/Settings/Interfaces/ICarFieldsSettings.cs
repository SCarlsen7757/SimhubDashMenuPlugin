using System.Collections.ObjectModel;

namespace DashMenu.Settings.Interfaces
{
    internal interface ICarFieldsSettings
    {
        string CarId { get; set; }
        string CarModel { get; set; }
        ObservableCollection<string> DisplayedDataFields { get; set; }
        ObservableCollection<string> DisplayedGaugeFields { get; set; }
        bool IsActive { get; }
    }
}