using DashMenu.Data;
using System.ComponentModel;

namespace DashMenu
{
    internal class GaugeFieldComponent : FieldComponentBase<IGaugeFieldExtension>, INotifyPropertyChanged
    {
        public GaugeFieldComponent() : base() { }
        public GaugeFieldComponent(IGaugeFieldExtension fieldExtension)
        {
            FieldExtension = fieldExtension;
        }
    }
}
