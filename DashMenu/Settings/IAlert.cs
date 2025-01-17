using System;

namespace DashMenu.Settings
{
    internal interface IAlert : IBasicSettings
    {
        TimeSpan ShowTimeDuration { get; set; }
    }
}