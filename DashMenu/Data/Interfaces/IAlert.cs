using GameReaderCommon;
using SimHub.Plugins;
using System;

namespace DashMenu.Data.Interfaces
{
    public interface IAlert
    {
        bool Show { get; }
        TimeSpan ShowTimeDuration { get; set; }
        IDataField Data { get; set; }
        DateTime EndTime { get; }
        void Update(PluginManager pluginManager, ref GameData data);
    }
}
