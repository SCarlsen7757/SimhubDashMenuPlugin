﻿using DashMenu.Settings.DisplayedFields;
using System.Windows.Controls;

namespace DashMenu.UI
{
    /// <summary>
    /// Interaction logic for CarFieldSettingItem.xaml
    /// </summary>
    public partial class CarFieldSettingItem : UserControl
    {
        private readonly bool contentInitialized = false;

        public CarFieldSettingItem()
        {
            InitializeComponent();
            contentInitialized = true;
        }

        private void UserControl_DataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            if (!contentInitialized) return;
        }

        private void ForgetCar_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            // Retrieve the CarFields object from the DataContext of the current UserControl
            CarFields carFields = (CarFields)DataContext;

            // Get the current UserControl
            UserControl currentControl = this;

            // Get the parent panel (assuming it's a Panel)
            Panel parent = (Panel)currentControl.Parent;

            // Retrieve the settings from the parent's DataContext
            Settings.Settings settings = (Settings.Settings)parent.DataContext;

            // Retrieve the ObservableDictionary from the settings using the game name
            ObservableDictionary<string, CarFields> carFieldsDictionary = settings.GameSettings[SimHub.Plugins.PluginManager.GetInstance().GameName].CarFields;

            // Remove the car from the dictionary using the CarId key
            if (carFieldsDictionary.ContainsKey(carFields.CarId))
            {
                carFieldsDictionary.Remove(carFields.CarId);
            }
        }
    }
}