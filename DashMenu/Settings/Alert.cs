using DashMenu.Settings.Interfaces;
using System;
using System.ComponentModel;

namespace DashMenu.Settings
{
    internal sealed class Alert : BasicSettings, IAlertSettings, INotifyPropertyChanged
    {
        private TimeSpan time = TimeSpan.FromMilliseconds(700);

        public TimeSpan ShowTimeDuration
        {
            get => time;
            set
            {
                if (time == value) return;
                time = value;
                OnPropertyChanged();
            }
        }
    }
}
