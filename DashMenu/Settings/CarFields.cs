﻿using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DashMenu.Settings
{
    internal class CarFields : INotifyPropertyChanged
    {
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
        public ObservableCollection<string> DisplayedDataFields { get; set; }
        public ObservableCollection<string> DisplayedGaugeFields { get; set; }

        public CarFields()
        {
            DisplayedDataFields = new ObservableCollection<string>();
            DisplayedGaugeFields = new ObservableCollection<string>();
        }
        public CarFields(string carId, string carModel, ObservableCollection<string> dataFields, ObservableCollection<string> gaugeFields)
        {
            CarId = carId;
            CarModel = carModel;
            DisplayedDataFields = dataFields;
            DisplayedGaugeFields = gaugeFields;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    }
}
