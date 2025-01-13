using DashMenu.UI;
using System.Collections.ObjectModel;

namespace DashMenu.Settings
{
    internal class FieldSettings<FieldType> where FieldType : IBasicSettings, new()
    {
        public ObservableDictionary<string, FieldType> Settings { get; set; } = new ObservableDictionary<string, FieldType>();
        public ObservableCollection<string> Order { get; set; } = new ObservableCollection<string>();
    }
}
