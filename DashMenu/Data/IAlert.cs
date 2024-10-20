using System;

namespace DashMenu.Data
{
    public interface IAlert
    {
        bool Show { get; }
        TimeSpan ShowTimeDuration { get; set; }
        IDataField Data { get; set; }
        DateTime EndTime { get; }
    }
}
