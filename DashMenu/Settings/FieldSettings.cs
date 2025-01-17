using DashMenu.UI;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace DashMenu.Settings
{
    internal class FieldSettings<FieldType> where FieldType : IBasicSettings, new()
    {
        public ObservableDictionary<string, FieldType> Settings { get; set; } = new ObservableDictionary<string, FieldType>();
        public ObservableCollection<string> Order { get; set; } = new ObservableCollection<string>();

        public IEnumerable<string> EnabledFieldsSortedByOrder()
        {
            return Order.Where(key => Settings.ContainsKey(key) && Settings[key].Enabled);
        }
    }
}
