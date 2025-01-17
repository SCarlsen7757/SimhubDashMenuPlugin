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
            DataFields.Settings.CollectionChanged += DataFields_CollectionChanged;
            GaugeFields.Settings.CollectionChanged += GaugeFields_CollectionChanged;
            Alerts.CollectionChanged += Alerts_CollectionChanged;
            PropertyChanged += DefaultAmountOfFields_PropertyChanged;
        }


        private void DefaultAmountOfFields_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(DefaultAmountOfDataFields))
            {
                ChangeDefaultField(DefaultAmountOfDataFields, DefaultDataFields);
            }
            else if (e.PropertyName == nameof(DefaultAmountOfGaugeFields))
            {
                ChangeDefaultField(DefaultAmountOfGaugeFields, DefaultGaugeFields);
            }
        }

        private static void ChangeDefaultField(int amount, IList<string> fields)
        {
            if (amount == fields.Count) return;
            do
            {
                if (fields.Count < amount)
                {
                    fields.Add(EmptyField.FullName);
                }
                else
                {
                    fields.RemoveAt(fields.Count - 1);
                }
            } while (fields.Count != amount);
        }

        #region Default amount of fields
        private int defaultAmountOfDataFields = 5;

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

        internal IList<string> DefaultDataFieldsList()
        {
            return DefaultDataFieldsList(DefaultAmountOfDataFields);
        }

        private static IList<string> DefaultDataFieldsList(int amountOfFields)
        {
            return new List<string>(Enumerable.Repeat(EmptyField.FullName, amountOfFields));
        }

        internal IList<string> DefaultGaugeFieldsList()
        {
            return DefaultGaugeFieldsList(DefaultAmountOfGaugeFields);
        }

        private IList<string> DefaultGaugeFieldsList(int amountOfFields)
        {
            return new List<string>(Enumerable.Repeat(EmptyField.FullName, amountOfFields));
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

        #region Field and Alert settings
        public delegate void DataFieldSettingsChangedEventHandler(IDataField dataField, PropertyChangedEventArgs e);

        public delegate void GaugeFieldSettingsChangedEventHandler(IGaugeField gaugeField, PropertyChangedEventArgs e);

        public delegate void AlertSettingsChangedEventHandler(IDataField dataField, IAlert alert, PropertyChangedEventArgs e);

        public event DataFieldSettingsChangedEventHandler DataFieldSettingsChanged;

        public event GaugeFieldSettingsChangedEventHandler GaugeFieldSettingsChanged;

        public event AlertSettingsChangedEventHandler AlertSettingsChanged;

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public FieldSettings<DataField> DataFields { get; set; } = new FieldSettings<DataField>();

        public FieldSettings<GaugeField> GaugeFields { get; set; } = new FieldSettings<GaugeField>();

        public ObservableDictionary<string, Alert> Alerts { get; set; } = new ObservableDictionary<string, Alert>();

        private void DataField_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is DataField.OverrideProperties overrideProperties)
            {
                DataFieldSettingsChanged?.Invoke(overrideProperties.parent, e);
            }
            else if (sender is DataField field)
            {
                DataFieldSettingsChanged?.Invoke(field, e);
                if (Alerts.TryGetValue(field.FullName, out Alert alert)) AlertSettingsChanged?.Invoke(field, alert, e);
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
                GaugeFieldSettingsChanged?.Invoke(overrideProperties.parent, e);
            }
            else if (sender is GaugeField field)
            {
                GaugeFieldSettingsChanged?.Invoke(field, e);
            }
            else
            {
                throw new NotImplementedException(e.PropertyName);
            }
        }

        private void Alert_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is Alert alert)
            {
                AlertSettingsChanged?.Invoke(DataFields.Settings[alert.FullName], alert, e);
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
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    foreach (KeyValuePair<string, DataField> item in e.OldItems)
                    {
                        item.Value.PropertyChanged -= DataField_PropertyChanged;
                        item.Value.Override.NamePropertyChanged -= DataField_PropertyChanged;
                        item.Value.Override.DecimalPropertyChanged -= DataField_PropertyChanged;
                        item.Value.Override.ColorSchemePropertyChanged -= DataField_PropertyChanged;
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
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    foreach (KeyValuePair<string, GaugeField> item in e.OldItems)
                    {
                        item.Value.PropertyChanged -= GaugeField_PropertyChanged;
                        item.Value.Override.NamePropertyChanged -= GaugeField_PropertyChanged;
                        item.Value.Override.DecimalPropertyChanged -= GaugeField_PropertyChanged;
                        item.Value.Override.ColorSchemePropertyChanged -= GaugeField_PropertyChanged;
                        item.Value.Override.MaximumPropertyChanged -= GaugeField_PropertyChanged;
                        item.Value.Override.MinimumPropertyChanged -= GaugeField_PropertyChanged;
                        item.Value.Override.StepPropertyChanged -= GaugeField_PropertyChanged;
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private void Alerts_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (!(sender is ObservableDictionary<string, Alert> alerts)) return;
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    foreach (KeyValuePair<string, Alert> item in e.NewItems)
                    {
                        item.Value.PropertyChanged += Alert_PropertyChanged;
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    foreach (KeyValuePair<string, Alert> item in e.OldItems)
                    {
                        item.Value.PropertyChanged -= Alert_PropertyChanged;
                    }
                    break;
                default:
                    break;
            }
        }
        #endregion

        /// <summary>
        /// Remove the specific car, except if it's the current car.
        /// </summary>
        /// <param name="carId"></param>
        internal void RemoveCar(string carId)
        {
            if (!CarFields.ContainsKey(carId)) return;
            CarFields.Remove(carId);
        }

        /// <summary>
        /// Remove all cars, except the current car.
        /// </summary>
        internal void RemoveCar()
        {
            if (CurrentCarId == null)
            {
                CarFields.Clear();
                return;
            }

            var carsToRemove = CarFields.Keys.Where(key => key != CurrentCarId).ToList();
            foreach (var carId in carsToRemove)
            {
                CarFields.Remove(carId);
            }
        }
    }
}
