using System;
using System.ComponentModel;

namespace DashMenu.Data
{
    public interface IAlert<T>
        where T : IDataField, INotifyPropertyChanged
    {
        bool Show { get; }
        TimeSpan ShowTimeDuration { get; set; }
        T Data { get; set; }
        DateTime EndTime { get; }
    }
}
