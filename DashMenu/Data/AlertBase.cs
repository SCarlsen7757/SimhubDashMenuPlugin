using System;
using System.ComponentModel;

namespace DashMenu.Data
{
    public abstract class AlertBase : FieldExtensionBase<IDataField>, IAlert
    {
        protected AlertBase(string gameName) : base(gameName)
        {
        }

        protected virtual void DataAlert_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!(sender is IDataField)) return;
            switch (e.PropertyName)
            {
                case nameof(IDataField.Value):
                    EndTime = DateTime.Now + ShowTimeDuration;
                    break;
                default:
                    break;
            }
        }

        public bool Show { get => DateTime.Now < EndTime; }

        public TimeSpan ShowTimeDuration { get; set; } = TimeSpan.Zero;

        public DateTime EndTime { get; protected set; } = DateTime.Now;
    }
}
