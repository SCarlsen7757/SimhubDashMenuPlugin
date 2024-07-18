using DashMenu.Settings.DisplayedFields;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DashMenu.Settings
{
    internal class Settings : INotifyPropertyChanged
    {
        private int defaultMaxFields = 5;
        /// <summary>
        /// Max amount of fields that can be displayed.
        /// </summary>
        public int DefaultMaxFields
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
        /// <summary>
        /// Add or update displayed fields settings for the car.
        /// </summary>
        /// <param name="gameName">Name of the game.</param>
        /// <param name="carName">Name of the car.</param>
        /// <param name="displayedFields">Displayed fields settings. Can be null then it create default displayed fields settings.</param>
        internal void UpdateDisplayedField(string gameName, string carName, string[] displayedFields = null)
        {
            if (displayedFields == null)
            {
                for (int i = 0; i < displayedFields.Length; i++)
                {
                    displayedFields[i] = EmptyField.FullName;
                }
            }
            if (GameSettings.TryGetValue(gameName, out var gameSettings))
            {
                if (gameSettings.CarSettings.TryGetValue(carName, out var carSettings))
                {
                    carSettings.DisplayedFields = displayedFields;
                }
                else
                {
                    carSettings = new CarSettings
                    {
                        DisplayedFields = displayedFields
                    };
                    gameSettings.CarSettings.Add(carName, carSettings);
                }
            }
            else
            {
                gameSettings = new GameSettings();
                GameSettings.Add(gameName, gameSettings);
                UpdateDisplayedField(gameName, carName, displayedFields);
            }
        }
        /// <summary>
        /// Get displayed fields settings for the car.
        /// </summary>
        /// <param name="gameName">Game of the game.</param>
        /// <param name="carName">Game of the car.</param>
        /// <returns></returns>
        internal string[] GetDisplayedField(string gameName, string carName)
        {

            if (GameSettings.TryGetValue(gameName, out var gameSettings))
            {
                if (gameSettings.CarSettings.TryGetValue(carName, out var carSettings))
                {
                    return carSettings.DisplayedFields;
                }
                else
                {
                    var displayedFields = new string[DefaultMaxFields];
                    for (int i = 0; i < displayedFields.Length; i++)
                    {
                        displayedFields[i] = EmptyField.FullName;
                    }
                    return displayedFields;
                }
            }
            else
            {
                gameSettings = new GameSettings();
                GameSettings.Add(gameName, gameSettings);
                return GetDisplayedField(gameName, carName);
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
