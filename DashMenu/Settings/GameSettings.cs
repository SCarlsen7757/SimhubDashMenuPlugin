using DashMenu.UI;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;


namespace DashMenu.Settings
{
    internal class GameSettings : INotifyPropertyChanged
    {
        public GameSettings()
        {
            DataFields.CollectionChanged += Fields_CollectionChanged;
            CarFields = new ObservableDictionary<string, CarFields>();
        }

        private int defaultAmountOfDataFields = 5;
        /// <summary>
        /// Max amount of fields that can be displayed.
        /// </summary>
        public int DefaultAmountOfDataFields
        {
            get => defaultAmountOfDataFields;
            set
            {
                if (value == defaultAmountOfDataFields) return;
                defaultAmountOfDataFields = value;
                OnPropertyChanged();
            }
        }
        private readonly object collectionDataFieldLock = new object();
        private readonly ObservableCollection<string> defaultDataFields = new ObservableCollection<string>();
        public ObservableCollection<string> DefaultDataFields
        {
            get
            {
                return Application.Current.Dispatcher.Invoke(() =>
                {
                    lock (collectionDataFieldLock)
                    {
                        return defaultDataFields;
                    }
                });
            }
            set
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    lock (collectionDataFieldLock)
                    {
                        // Only clear and repopulate if the incoming value is different
                        if (value != null && !defaultDataFields.SequenceEqual(value))
                        {
                            defaultDataFields.Clear();
                            foreach (var field in value)
                            {
                                defaultDataFields.Add(field);
                            }
                            OnPropertyChanged();
                        }
                    }
                });
            }
        }
        private ObservableCollection<string> DefaultDataFieldsList()

        {
            return new ObservableCollection<string>(Enumerable.Repeat(EmptyDataField.FullName, DefaultAmountOfDataFields));
        }
        public ObservableDictionary<string, CarFields> CarFields { get; set; }
        /// <summary>
        /// All fields. Used for enabling and disabling the fields to be able to select them.
        /// </summary> 
        public ObservableDictionary<string, DataFields> DataFields { get; set; } = new ObservableDictionary<string, DataFields>();

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        private void PropertyOverride_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(sender.GetType().Name); // Raise PropertyChanged for the PropertyOverride property
        }
        private void Fields_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (!(sender is DataFields fields)) return;
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    fields.PropertyChanged += PropertyOverride_PropertyChanged;
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    fields.PropertyChanged -= PropertyOverride_PropertyChanged;
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
#if DEBUG
                    throw new NotImplementedException();
#endif
                case System.Collections.Specialized.NotifyCollectionChangedAction.Move:
#if DEBUG
                    throw new NotImplementedException();
#endif
                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
#if DEBUG
                    throw new NotImplementedException();
#endif
                default:
#if DEBUG
                    throw new NotImplementedException();
#endif
            }
        }

        /// <summary>
        /// Add or update displayed fields settings for the car.
        /// </summary>
        /// <param name="gameName">Name of the game.</param>
        /// <param name="carId">Id of the car</param>
        /// <param name="carModel">Name of the car.</param>
        /// <param name="displayedFields">Displayed fields settings. Can be null then it create default displayed fields settings.</param>
        internal void UpdateDisplayedField(string carId, string carModel, ObservableCollection<string> displayedFields = null)
        {
            if (displayedFields == null)
            {
                displayedFields = DefaultDataFields;
                if (displayedFields.Count == 0)
                {
                    displayedFields = DefaultDataFieldsList();
                }
            }
            if (CarFields.TryGetValue(carId, out var carSettings))
            {
                carSettings.DisplayedDataFields = displayedFields;
            }
            else
            {
                carSettings = new CarFields(carId, carModel, displayedFields);
                CarFields.Add(carId, carSettings);
            }
        }
        /// <summary>
        /// Get displayed fields settings for the car.
        /// </summary>
        /// <param name="gameName">Game of the game.</param>
        /// <param name="carId">ID of the car.</param>
        /// <returns></returns>
        internal ObservableCollection<string> GetDisplayedField(string carId)
        {
            if (CarFields.TryGetValue(carId, out var carSettings))
            {
                return carSettings.DisplayedDataFields;
            }
            else
            {
                var displayedFields = DefaultDataFields;
                if (displayedFields.Count == 0)
                {
                    displayedFields = DefaultDataFieldsList();
                }
                return displayedFields;
            }
        }
        /// <summary>
        /// Remove specified displayed field settings.
        /// </summary>
        /// <param name="gameName">Name of the game</param>
        /// <param name="carId">ID of the car</param>
        internal void RemoveDisplayedField(string gameName, string carId)
        {
            CarFields.Remove(carId);
        }
        internal void RemoveAllDisplayedFields()
        {
            CarFields.Clear();
        }
    }
}
