using System;

namespace DashMenu.Settings.Interfaces
{
    internal interface IAlertSettings : IBasicSettings
    {
        TimeSpan ShowTimeDuration { get; set; }
    }
}