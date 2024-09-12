using DashMenu.UI;
using System;
using System.Collections.Generic;
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
            DataFields.CollectionChanged += DataFields_CollectionChanged;
            GaugeFields.CollectionChanged += GaugeFields_CollectionChanged;
            CarFields = new ObservableDictionary<string, CarFields>();
        }

        private int defaultAmountOfDataFields = 5;
        /// <summary>
        /// Max amount of data fields that can be displayed.
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
            set => Application.Current.Dispatcher.Invoke(() =>
                                {
                                    lock (collectionDataFieldLock)
                                    {
                                        // Only clear and repopulate if the incoming value is different
                                        if (value != null && !defaultDataFields.SequenceEqual(value))
                                        {
                                            defaultDataFields.Clear();
                                            foreach (string field in value)
                                            {
                                                defaultDataFields.Add(field);
                                            }
                                            OnPropertyChanged();
                                        }
                                    }
                                });
        }
        private int defaultAmountOfGaugeFields = 2;
        public int DefaultAmountOfGaugeFields
        {
            get => defaultAmountOfGaugeFields;
            set
            {
                if (value == defaultAmountOfGaugeFields) return;
                defaultAmountOfGaugeFields = value;
                OnPropertyChanged();
            }
        }
        private readonly object collectionGaugeFieldLock = new object();
        private readonly ObservableCollection<string> defaultGaugeFields = new ObservableCollection<string>();
        public ObservableCollection<string> DefaultGaugeFields
        {
            get
            {
                return Application.Current.Dispatcher.Invoke(() =>
                {
                    lock (collectionGaugeFieldLock)
                    {
                        return defaultGaugeFields;
                    }
                });
            }
            set => Application.Current.Dispatcher.Invoke(() =>
                                {
                                    lock (collectionGaugeFieldLock)
                                    {
                                        // Only clear and repopulate if the incoming value is different
                                        if (value != null && !defaultGaugeFields.SequenceEqual(value))
                                        {
                                            defaultGaugeFields.Clear();
                                            foreach (string field in value)
                                            {
                                                defaultGaugeFields.Add(field);
                                            }
                                            OnPropertyChanged();
                                        }
                                    }
                                });
        }
        private ObservableCollection<string> DefaultDataFieldsList()
        {
            return new ObservableCollection<string>(Enumerable.Repeat(EmptyDataField.FullName, DefaultAmountOfDataFields));
        }
        private ObservableCollection<string> DefaultGaugeFieldsList()
        {
            return new ObservableCollection<string>(Enumerable.Repeat(EmptyGaugeField.FullName, DefaultAmountOfGaugeFields));
        }
        public ObservableDictionary<string, CarFields> CarFields { get; set; }
        /// <summary>
        /// All data fields. Used for enabling and disabling the fields to be able to select them.
        /// </summary> 
        public ObservableDictionary<string, DataField> DataFields { get; set; } = new ObservableDictionary<string, DataField>();
        /// <summary>
        /// All gauge fields. Used for enabling and disabling the fields to be able to select them.
        /// </summary>
        public ObservableDictionary<string, GaugeField> GaugeFields { get; set; } = new ObservableDictionary<string, GaugeField>();

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        private void PropertyOverride_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(sender.GetType().Name); // Raise PropertyChanged for the PropertyOverride property
        }
        private void DataFields_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (!(sender is DataField fields)) return;
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    fields.PropertyChanged += PropertyOverride_PropertyChanged;
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    fields.PropertyChanged -= PropertyOverride_PropertyChanged;
                    break;
                default:
#if DEBUG
                    throw new NotImplementedException();
#else
                    break;
#endif
            }
        }
        private void GaugeFields_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (!(sender is GaugeField fields)) return;
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    fields.PropertyChanged += PropertyOverride_PropertyChanged;
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    fields.PropertyChanged -= PropertyOverride_PropertyChanged;
                    break;
                default:
#if DEBUG
                    throw new NotImplementedException();
#else
                    break;
#endif
            }
        }

        /// <summary>
        /// Add or update displayed fields settings for the car.
        /// </summary>
        /// <param name="gameName">Name of the game.</param>
        /// <param name="carId">Id of the car</param>
        /// <param name="carModel">Name of the car.</param>
        /// <param name="displayedDataFields">Displayed data field settings.</param>
        /// <param name="displayedGaugeFields">Displayed gauge field settings</param>
        internal void UpdateDisplayedFields(string carId, string carModel, List<string> displayedDataFields, List<string> displayedGaugeFields)
        {
            if (CarFields.TryGetValue(carId, out var carSettings))
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    carSettings.DisplayedDataFields.Clear();
                    foreach (string field in displayedDataFields)
                    {
                        carSettings.DisplayedDataFields.Add(field);
                    }
                    carSettings.DisplayedGaugeFields.Clear();
                    foreach (string field in displayedGaugeFields)
                    {
                        carSettings.DisplayedGaugeFields.Add(field);
                    }
                });
            }
            else
            {
                carSettings = new CarFields(carId,
                    carModel,
                    new ObservableCollection<string>(displayedDataFields),
                    new ObservableCollection<string>(displayedGaugeFields));
                CarFields.Add(carId, carSettings);
            }
        }
        /// <summary>
        /// Get displayed fields settings for the car.
        /// </summary>
        /// <param name="gameName">Game of the game.</param>
        /// <param name="carId">ID of the car.</param>
        /// <returns></returns>
        internal ObservableCollection<string> GetDisplayedDataField(string carId)
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
        internal ObservableCollection<string> GetDisplayedGaugeField(string carId)
        {
            if (CarFields.TryGetValue(carId, out var carSettings))
            {
                return carSettings.DisplayedGaugeFields;
            }
            else
            {
                var displayedFields = DefaultGaugeFields;
                if (displayedFields.Count == 0)
                {
                    displayedFields = DefaultGaugeFieldsList();
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
