using System.ComponentModel;

namespace DashMenu
{
    internal class GaugeFieldComponent : FieldComponentBase, INotifyPropertyChanged
    {
        public GaugeFieldComponent() : base() { }
        public GaugeFieldComponent(Data.IGaugeFieldComponent fieldComponent)
        {
            FieldComponent = fieldComponent;
            FullName = FieldComponent.GetType().FullName;
        }
        public Data.IGaugeFieldComponent FieldComponent { get; set; }

    }
}
