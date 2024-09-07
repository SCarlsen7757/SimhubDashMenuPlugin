using System.ComponentModel;

namespace DashMenu
{
    internal class DataFieldComponent : FieldComponentBase, INotifyPropertyChanged
    {
        public DataFieldComponent() { }
        public DataFieldComponent(Data.IDataFieldComponent fieldComponent)
        {
            FieldComponent = fieldComponent;
            FullName = FieldComponent.GetType().FullName;
        }
        public Data.IDataFieldComponent FieldComponent { get; set; }

    }
}