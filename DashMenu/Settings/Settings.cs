using DashMenu.UI;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;


namespace DashMenu.Settings
{
    internal class Settings : INotifyPropertyChanged
    {
        public Settings()
        {
        }
        /// <summary>
        /// Fields displayed. Per game and car.
        /// </summary>
        public ObservableDictionary<string, GameSettings> GameSettings { get; set; } = new ObservableDictionary<string, GameSettings>();

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        private void PropertyOverride_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(sender.GetType().Name); // Raise PropertyChanged for the PropertyOverride property
        }
        private void Fields_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (!(sender is Fields fields)) return;
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    fields.PropertyChanged += PropertyOverride_PropertyChanged;
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    fields.PropertyChanged -= PropertyOverride_PropertyChanged;
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                    throw new NotImplementedException();
                case System.Collections.Specialized.NotifyCollectionChangedAction.Move:
                    throw new NotImplementedException();
                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                    throw new NotImplementedException();
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
