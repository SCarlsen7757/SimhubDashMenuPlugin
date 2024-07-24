using DashMenu.Settings.DisplayedFields;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace DashMenu.Settings
{
    internal class Settings : INotifyPropertyChanged
    {
        private int defaultMaxFields = 5;
        /// <summary>
        /// Max amount of fields that can be displayed.
        /// </summary>
        public int DefaultAmountOfFields
        {
            get => defaultMaxFields; set
            {
                if (value == defaultMaxFields) return;
                defaultMaxFields = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// Fields displayed.
        /// </summary>
        public Dictionary<string, DisplayedFields.GameSettings> GameSettings { get; set; } = new Dictionary<string, DisplayedFields.GameSettings>();
        private List<string> DefaultFieldData()
        {
            return Enumerable.Repeat(EmptyField.FullName, DefaultAmountOfFields).ToList();
        }
        /// <summary>
        /// Add or update displayed fields settings for the car.
        /// </summary>
        /// <param name="gameName">Name of the game.</param>
        /// <param name="carId">Id of the car</param>
        /// <param name="carModel">Name of the car.</param>
        /// <param name="displayedFields">Displayed fields settings. Can be null then it create default displayed fields settings.</param>
        internal void UpdateDisplayedField(string gameName, string carId, string carModel, List<string> displayedFields = null)
        {
            if (displayedFields == null)
            {
                displayedFields = DefaultFieldData();
            }
            if (GameSettings.TryGetValue(gameName, out var gameSettings))
            {
                if (gameSettings.CarSettings.TryGetValue(carId, out var carSettings))
                {
                    carSettings.DisplayedFields = displayedFields;
                }
                else
                {
                    carSettings = new CarSettings(carId, carModel, displayedFields);
                    gameSettings.CarSettings.Add(carId, carSettings);
                }
            }
            else
            {
                gameSettings = new GameSettings();
                GameSettings.Add(gameName, gameSettings);
                UpdateDisplayedField(gameName, carId, carModel, displayedFields);
            }
        }
        /// <summary>
        /// Get displayed fields settings for the car.
        /// </summary>
        /// <param name="gameName">Game of the game.</param>
        /// <param name="carId">ID of the car.</param>
        /// <returns></returns>
        internal List<string> GetDisplayedField(string gameName, string carId)
        {

            if (GameSettings.TryGetValue(gameName, out var gameSettings))
            {
                if (gameSettings.CarSettings.TryGetValue(carId, out var carSettings))
                {
                    return carSettings.DisplayedFields;
                }
                else
                {
                    return DefaultFieldData();
                }
            }
            else
            {
                gameSettings = new GameSettings();
                GameSettings.Add(gameName, gameSettings);
                return GetDisplayedField(gameName, carId);
            }
        }
        /// <summary>
        /// Remove specified displayed field settings.
        /// </summary>
        /// <param name="gameName">Name of the game</param>
        /// <param name="carId">ID of the car</param>
        internal void RemoveDisplayedField(string gameName, string carId)
        {
            if (GameSettings.TryGetValue(gameName, out var gameSettings))
            {
                if (gameSettings.CarSettings.TryGetValue(carId, out var carSettings))
                {
                    gameSettings.CarSettings.Remove(carId);
                }
            }
        }
        internal void RemoveAllDisplayedFields(string gameName)
        {
            if (GameSettings.TryGetValue(gameName, out var gameSettings))
            {
                GameSettings.Remove(gameName);
            }
        }
        /// <summary>
        /// All fields. Used for enabling and disabling the fields to be able to select them.
        /// </summary> 
        public ObservableCollection<Fields> Fields { get; set; } = new ObservableCollection<Fields>();

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
