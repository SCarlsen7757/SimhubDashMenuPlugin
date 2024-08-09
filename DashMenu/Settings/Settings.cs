using DashMenu.Extensions;
using DashMenu.Settings.DisplayedFields;
using DashMenu.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;


namespace DashMenu.Settings
{
    internal class Settings : INotifyPropertyChanged
    {
        public Settings()
        {
            Fields.CollectionChanged += Fields_CollectionChanged;
        }
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
        /// Fields displayed. Per game and car.
        /// </summary>
        public ObservableDictionary<string, GameSettings> GameSettings { get; set; } = new ObservableDictionary<string, GameSettings>();
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
                if (gameSettings.CarFields.TryGetValue(carId, out var carSettings))
                {
                    carSettings.DisplayedFields = displayedFields.ToObservableCollection();
                }
                else
                {
                    carSettings = new CarFields(carId, carModel, displayedFields);
                    gameSettings.CarFields.Add(carId, carSettings);
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
                if (gameSettings.CarFields.TryGetValue(carId, out var carSettings))
                {
                    return carSettings.DisplayedFields.ToList();
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
                if (gameSettings.CarFields.TryGetValue(carId, out var carSettings))
                {
                    gameSettings.CarFields.Remove(carId);
                }
            }
        }
        internal void RemoveAllDisplayedFields(string gameName)
        {
            if (GameSettings.TryGetValue(gameName, out var gameSettings))
            {
                gameSettings.CarFields.Clear();
            }
        }
        /// <summary>
        /// All fields. Used for enabling and disabling the fields to be able to select them.
        /// </summary> 
        public ObservableDictionary<string, Fields> Fields { get; set; } = new ObservableDictionary<string, Fields>();

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
