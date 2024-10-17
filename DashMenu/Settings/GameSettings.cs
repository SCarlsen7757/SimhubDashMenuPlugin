using DashMenu.Extensions;
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
        }

        #region Default amount of fields
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
        #endregion

        #region Default fields
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
        private IList<string> DefaultDataFieldsList()
        {
            return new List<string>(Enumerable.Repeat(EmptyDataField.FullName, DefaultAmountOfDataFields));
        }
        private IList<string> DefaultGaugeFieldsList()
        {
            return new List<string>(Enumerable.Repeat(EmptyGaugeField.FullName, DefaultAmountOfGaugeFields));
        }
        #endregion

        #region Car
        internal string CurrentCarId { get; private set; } = null;
        internal string CurrentCarModel { get; private set; } = null;

        public delegate void CarFieldChangedEventHandler(ICarFields carFields);
        public event CarFieldChangedEventHandler CurrentCarFieldChanged;
        internal void CarChanged(object sender, EventArgs e)
        {
            CarChanged();
        }
        internal void CarChanged()
        {
            //Old car
            if (CurrentCarId != null && CarFields.TryGetValue(CurrentCarId, out CarFields oldCar))
            {
                oldCar.IsActive = false;
            }

            //New car
            var pluginManager = SimHub.Plugins.PluginManager.GetInstance();
            CurrentCarId = pluginManager.LastCarId;
            CurrentCarModel = pluginManager.GameManager.CarManager.LastCarSettings.CarModel;

            if (!(CarFields.ContainsKey(CurrentCarId)))
            {
                var defualtDataField = DefaultDataFields.Count > 0 ? DefaultDataFields : DefaultDataFieldsList();
                var defaultGaugeField = DefaultGaugeFields.Count > 0 ? DefaultGaugeFields : DefaultGaugeFieldsList();

                CarFields newCar = new CarFields(CurrentCarId, CurrentCarModel, defualtDataField, defaultGaugeField);
                CarFields.Add(CurrentCarId, newCar);
            }

            CarFields[CurrentCarId].IsActive = true;
            CurrentCarFieldChanged?.Invoke(CarFields[CurrentCarId]);
        }

        public void UpdateDisplayedDataFields(IList<string> fields)
        {
            CarFields[CurrentCarId].DisplayedDataFields = fields.ToObservableCollection();
        }

        public void UpdateDisplayedGaugeFields(IList<string> fields)
        {
            CarFields[CurrentCarId].DisplayedGaugeFields = fields.ToObservableCollection();
        }

        public ObservableDictionary<string, CarFields> CarFields { get; set; } = new ObservableDictionary<string, CarFields>();
        #endregion

        #region Field settings
        public delegate void DataFieldSettingsChangedEventHandler(DataField dataField);
        public delegate void GaugeFieldSettingsChangedEventHandler(GaugeField gaugeField);

        public event DataFieldSettingsChangedEventHandler DataFieldSettingsChanged;
        public event DataFieldSettingsChangedEventHandler DataFieldOverrideNameSettingsChanged;
        public event DataFieldSettingsChangedEventHandler DataFieldOverrideDecimalSettingsChanged;
        public event DataFieldSettingsChangedEventHandler DataFieldOverrideColorSchemeSettingsChanged;

        public event GaugeFieldSettingsChangedEventHandler GaugeFieldSettingsChanged;
        public event GaugeFieldSettingsChangedEventHandler GaugeFieldOverrideNameSettingsChanged;
        public event GaugeFieldSettingsChangedEventHandler GaugeFieldOverrideDecimalSettingsChanged;
        public event GaugeFieldSettingsChangedEventHandler GaugeFieldOverrideColorSchemeSettingsChanged;
        public event GaugeFieldSettingsChangedEventHandler GaugeFieldOverrideRangeSettingsChanged;
        public event GaugeFieldSettingsChangedEventHandler GaugeFieldOverrideStepSettingsChanged;

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
        private void DataField_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is DataField.OverrideProperties overrideProperties)
            {
                switch (e.PropertyName)
                {
                    case nameof(overrideProperties.Name):
                        DataFieldOverrideNameSettingsChanged?.Invoke(overrideProperties.parent);
                        return;
                    case nameof(overrideProperties.Decimal):
                        DataFieldOverrideDecimalSettingsChanged?.Invoke(overrideProperties.parent);
                        return;
                    case nameof(overrideProperties.DayNightColorScheme):
                        DataFieldOverrideColorSchemeSettingsChanged?.Invoke(overrideProperties.parent);
                        return;
                    default:
                        throw new NotImplementedException(e.PropertyName);
                }
            }
            else if (sender is DataField field)
            {
                DataFieldSettingsChanged?.Invoke(field);
            }
            else
            {
                throw new NotImplementedException(e.PropertyName);
            }

        }
        private void GaugeField_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is GaugeField.OverrideProperties overrideProperties)
            {
                switch (e.PropertyName)
                {
                    case nameof(overrideProperties.Name):
                        GaugeFieldOverrideNameSettingsChanged?.Invoke(overrideProperties.parent);
                        return;
                    case nameof(overrideProperties.Decimal):
                        GaugeFieldOverrideDecimalSettingsChanged?.Invoke(overrideProperties.parent);
                        return;
                    case nameof(overrideProperties.DayNightColorScheme):
                        GaugeFieldOverrideColorSchemeSettingsChanged?.Invoke(overrideProperties.parent);
                        return;
                    case nameof(overrideProperties.Maximum):
                    case nameof(overrideProperties.Minimum):
                        GaugeFieldOverrideRangeSettingsChanged?.Invoke(overrideProperties.parent);
                        return;
                    case nameof(overrideProperties.Step):
                        GaugeFieldOverrideStepSettingsChanged?.Invoke(overrideProperties.parent);
                        return;
                    default:
                        throw new NotImplementedException(e.PropertyName);
                }
            }
            else if (sender is GaugeField field)
            {
                GaugeFieldSettingsChanged?.Invoke(field);
            }
            else
            {
                throw new NotImplementedException(e.PropertyName);
            }

        }
        private void DataFields_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (!(sender is ObservableDictionary<string, DataField> fields)) return;
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    foreach (KeyValuePair<string, DataField> item in e.NewItems)
                    {
                        item.Value.PropertyChanged += DataField_PropertyChanged;
                        item.Value.Override.NamePropertyChanged += DataField_PropertyChanged;
                        item.Value.Override.DecimalPropertyChanged += DataField_PropertyChanged;
                        item.Value.Override.ColorSchemePropertyChanged += DataField_PropertyChanged;
                    }
                    break;
                default:
                    throw new NotImplementedException();

            }
        }
        private void GaugeFields_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (!(sender is ObservableDictionary<string, GaugeField> fields)) return;
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    foreach (KeyValuePair<string, GaugeField> item in e.NewItems)
                    {
                        item.Value.PropertyChanged += GaugeField_PropertyChanged;
                        item.Value.Override.NamePropertyChanged += GaugeField_PropertyChanged;
                        item.Value.Override.DecimalPropertyChanged += GaugeField_PropertyChanged;
                        item.Value.Override.ColorSchemePropertyChanged += GaugeField_PropertyChanged;
                        item.Value.Override.MaximumPropertyChanged += GaugeField_PropertyChanged;
                        item.Value.Override.MinimumPropertyChanged += GaugeField_PropertyChanged;
                        item.Value.Override.StepPropertyChanged += GaugeField_PropertyChanged;
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
        #endregion

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
        internal IList<string> GetDisplayedDataField()
        {
            if (CarFields.TryGetValue(CurrentCarId, out var carSettings))
            {
                return carSettings.DisplayedDataFields;
            }
            else
            {
                if (DefaultDataFields.Count == 0)
                {
                    DefaultDataFields = DefaultDataFieldsList().ToObservableCollection();
                }
                return DefaultDataFields;
            }
        }
        internal IList<string> GetDisplayedGaugeField()
        {
            if (CarFields.TryGetValue(CurrentCarId, out var carSettings))
            {
                return carSettings.DisplayedGaugeFields;
            }
            else
            {
                if (DefaultGaugeFields.Count == 0)
                {
                    DefaultGaugeFields = DefaultGaugeFieldsList().ToObservableCollection();
                }
                return DefaultGaugeFields;
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
