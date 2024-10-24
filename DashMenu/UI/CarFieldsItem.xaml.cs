﻿using DashMenu.Settings;
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
            GameSettings gameSettings = (GameSettings)parent.DataContext;
            gameSettings.RemoveCar(carFields.CarId);
        }
        private void DefaultDataFields_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            CarFields carFields = (CarFields)DataContext;
            UserControl currentControl = this;
            Panel parent = (Panel)currentControl.Parent;
            GameSettings gameSettings = (GameSettings)parent.DataContext;

            gameSettings.DefaultDataFields = carFields.DisplayedDataFields;
        }
        private void DefaultGaugeFields_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            CarFields carFields = (CarFields)DataContext;
            UserControl currentControl = this;
            Panel parent = (Panel)currentControl.Parent;
            GameSettings gameSettings = (GameSettings)parent.DataContext;

            gameSettings.DefaultGaugeFields = carFields.DisplayedGaugeFields;
        }
    }
}
