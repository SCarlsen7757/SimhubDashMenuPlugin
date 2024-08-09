using DashMenu.Extensions;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DashMenu.Settings.DisplayedFields
{
    internal class CarFields : INotifyPropertyChanged
    {
        //TODO: Add INotifyPropertyChanged
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
        public ObservableCollection<string> DisplayedFields { get; set; }
        public CarFields() { }
        public CarFields(string carId, string carModel, List<string> fields)
        {
            CarId = carId;
            CarModel = carModel;
            DisplayedFields = fields.ToObservableCollection();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    }
}
