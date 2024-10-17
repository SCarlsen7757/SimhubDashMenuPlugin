using DashMenu.UI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;


namespace DashMenu.Settings
{
    internal class Settings : INotifyPropertyChanged
    {
        public Settings()
        {
            GameName = SimHub.Plugins.PluginManager.GetInstance().GameName;
            GameSettings.CollectionChanged += GameSettings_CollectionChanged;
        }

        private void GameSettings_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    foreach (var item in e.NewItems)
                    {
                        if (!(item is KeyValuePair<string, GameSettings> addedGameSettings)) return;
                        if (addedGameSettings.Key != GameName) return;

                        var value = addedGameSettings.Value;
                        value.DataFields.CollectionChanged += DataFields_CollectionChanged;
                        foreach (var field in value.DataFields.Values)
                        {
                            field.PropertyChanged += Field_PropertyChanged;
                            field.Override.NamePropertyChanged += Name_PropertyChanged;
                            field.Override.DecimalPropertyChanged += Decimal_PropertyChanged;
                            field.Override.ColorSchemePropertyChanged += ColorScheme_PropertyChanged;
                        }
                    }

                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Move:
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                    break;
                default:
                    break;
            }
        }

        private void DataFields_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    foreach (var item in e.NewItems)
                    {
                        if (!(item is KeyValuePair<string, DataField> dataField)) return;
                        var field = dataField.Value;
                        field.PropertyChanged += Field_PropertyChanged;
                        field.Override.NamePropertyChanged += Name_PropertyChanged;
                        field.Override.DecimalPropertyChanged += Decimal_PropertyChanged;
                        field.Override.ColorSchemePropertyChanged += ColorScheme_PropertyChanged;
                    }
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Move:
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                    break;
                default:
                    break;
            }
        }

        private void Field_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
        }

        private void ColorScheme_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
        }

        private void Decimal_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
        }

        private void Name_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
        }


        [JsonIgnore]
        internal string GameName { get; private set; }
        /// <summary>
        /// Fields displayed. Per game and car.
        /// </summary>
        public ObservableDictionary<string, GameSettings> GameSettings { get; set; } = new ObservableDictionary<string, GameSettings>();

        internal GameSettings GetCurrentGameSettings()
        {
            if (!GameSettings.ContainsKey(GameName))
            {
                GameSettings.Add(GameName, new GameSettings());
            }

            return GameSettings[GameName];
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        private void PropertyOverride_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(sender.GetType().Name); // Raise PropertyChanged for the PropertyOverride property
        }
        private void Fields_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
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
