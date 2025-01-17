using System;
using System.ComponentModel;

namespace DashMenu.Settings
{
    internal class Alert : BasicSettings, IAlert, INotifyPropertyChanged
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
