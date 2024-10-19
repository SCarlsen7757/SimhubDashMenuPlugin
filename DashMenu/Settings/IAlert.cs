using System;

namespace DashMenu.Settings
{
    internal interface IAlert : IBasicSettings
    {
        bool Enabled { get; set; }
        TimeSpan ShowTimeDuration { get; set; }
    }
}