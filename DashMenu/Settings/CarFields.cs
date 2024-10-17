using DashMenu.Extensions;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;

namespace DashMenu.Settings
{
    internal class CarFields : INotifyPropertyChanged, ICarFields
    {
        public CarFields()
        {
        }
        public CarFields(string carId, string carModel, IList<string> dataFields, IList<string> gaugeFields)
        {
            CarId = carId;
            CarModel = carModel;
            DisplayedDataFields = dataFields.ToObservableCollection();
            DisplayedGaugeFields = gaugeFields.ToObservableCollection();
        }

        private bool isActive = false;
        /// <summary>
        /// Car model active
        /// </summary>
        [JsonIgnore]
        public bool IsActive
        {
            get => isActive;
            internal set
            {
                if (isActive == value) return;
                isActive = value;
                OnPropertyChanged();
            }
        }
        private string carId = string.Empty;
        /// <summary>
        /// Car ID
        /// </summary>
        public string CarId
        {
            get => carId;
            set
            {
                if (carId == value) return;
                carId = value;
                OnPropertyChanged();
            }
        }
        private string carModel = string.Empty;
        /// <summary>
        /// Car Model
        /// </summary>
        public string CarModel
        {
            get => carModel;
            set
            {
                if (carModel == value) return;
                carModel = value;
                OnPropertyChanged();
            }
        }
        private readonly ObservableCollection<string> displayedDataFields = new ObservableCollection<string>();
        private readonly object collectionDisplayedDataFieldsLock = new object();

        public ObservableCollection<string> DisplayedDataFields
        {
            get
            {
                return Application.Current.Dispatcher.Invoke(() =>
                {
                    lock (collectionDisplayedDataFieldsLock)
                    {
                        return displayedDataFields;
                    }
                });
            }
            set => Application.Current.Dispatcher.Invoke(() =>
            {
                lock (collectionDisplayedDataFieldsLock)
                {
                    // Only clear and repopulate if the incoming value is different
                    if (value != null && !displayedDataFields.SequenceEqual(value))
                    {
                        displayedDataFields.Clear();
                        foreach (string field in value)
                        {
                            displayedDataFields.Add(field);
                        }
                        OnPropertyChanged();
                    }
                }
            });
        }

        private readonly ObservableCollection<string> displayedGaugeFields = new ObservableCollection<string>();
        private readonly object collectionDisplayedGaugeFieldsLock = new object();

        public ObservableCollection<string> DisplayedGaugeFields
        {
            get
            {
                return Application.Current.Dispatcher.Invoke(() =>
                {
                    lock (collectionDisplayedGaugeFieldsLock)
                    {
                        return displayedGaugeFields;
                    }
                });
            }
            set => Application.Current.Dispatcher.Invoke(() =>
            {
                lock (collectionDisplayedGaugeFieldsLock)
                {
                    // Only clear and repopulate if the incoming value is different
                    if (value != null && !displayedGaugeFields.SequenceEqual(value))
                    {
                        displayedGaugeFields.Clear();
                        foreach (string field in value)
                        {
                            displayedGaugeFields.Add(field);
                        }
                        OnPropertyChanged();
                    }
                }
            });
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    }
}
