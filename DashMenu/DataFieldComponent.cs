using DashMenu.Data;
using System.ComponentModel;

namespace DashMenu
{
    internal class DataFieldComponent : FieldComponentBase<IDataFieldExtension>, INotifyPropertyChanged
    {
        public DataFieldComponent() { }
        public DataFieldComponent(IDataFieldExtension fieldExtension)
        {
            FieldExtension = fieldExtension;
        }
    }
}