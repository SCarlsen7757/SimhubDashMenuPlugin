using DashMenu.Settings;
using DashMenu.UI.Popup;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace DashMenu.UI
{
    /// <summary>
    /// Interaction logic for CarFieldSettingItem.xaml
    /// </summary>
    public partial class CarFieldSettingItem : UserControl
    {
        private readonly bool contentInitialized = false;

        private GameSettings settings;
        internal GameSettings Settings
        {
            set
            {
                if (settings != null) return;
                settings = value;
            }
        }

        private string oldCarId;

        public CarFieldSettingItem()
        {
            InitializeComponent();
            contentInitialized = true;
        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!contentInitialized || DataContext == null || settings == null) return;
            if (!(e.NewValue is CarFields carFields)) throw new InvalidOperationException($"{nameof(DataContext)} is not of type {typeof(CarFields).FullName}.");
            if (string.IsNullOrWhiteSpace(carFields.CarId)) return;

            if (!string.IsNullOrWhiteSpace(oldCarId))
            {
                settings.CarFields[oldCarId].DisplayedDataFields.CollectionChanged -= DisplayedDataFields_CollectionChanged;
                settings.CarFields[oldCarId].DisplayedGaugeFields.CollectionChanged -= DisplayedGaugeFields_CollectionChanged;
            }
            oldCarId = carFields.CarId;

            settings.CarFields[carFields.CarId].DisplayedDataFields.CollectionChanged += DisplayedDataFields_CollectionChanged;
            settings.CarFields[carFields.CarId].DisplayedGaugeFields.CollectionChanged += DisplayedGaugeFields_CollectionChanged;

            ItemsControlDataFields.ItemsSource = FieldInformation.ItemsControlSource(carFields.DisplayedDataFields, settings.DataFields);
            ItemsControlGaugeFields.ItemsSource = FieldInformation.ItemsControlSource(carFields.DisplayedGaugeFields, settings.GaugeFields);
        }

        private void DisplayedDataFields_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (!(DataContext is CarFields carFields)) throw new InvalidOperationException($"{nameof(DataContext)} is not of type {typeof(CarFields).FullName}.");
            ItemsControlDataFields.ItemsSource = FieldInformation.ItemsControlSource(carFields.DisplayedDataFields, settings.DataFields);
        }

        private void DisplayedGaugeFields_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (!(DataContext is CarFields carFields)) throw new InvalidOperationException($"{nameof(DataContext)} is not of type {typeof(CarFields).FullName}.");
            ItemsControlGaugeFields.ItemsSource = FieldInformation.ItemsControlSource(carFields.DisplayedGaugeFields, settings.GaugeFields);
        }

        private void ForgetCar_Click(object sender, RoutedEventArgs e)
        {
            if (!(DataContext is CarFields carFields)) throw new InvalidOperationException($"{nameof(DataContext)} is not of type {typeof(CarFields).FullName}.");
            settings.RemoveCar(carFields.CarId);
        }
        private void MakeDefaultDataFields_Click(object sender, RoutedEventArgs e)
        {
            if (!(DataContext is CarFields carFields)) throw new InvalidOperationException($"{nameof(DataContext)} is not of type {typeof(CarFields).FullName}.");
            settings.DefaultDataFields = carFields.DisplayedDataFields;
        }

        private void UseDefaultDataFields_Click(object sender, RoutedEventArgs e)
        {
            if (!(DataContext is CarFields carFields)) throw new InvalidOperationException($"{nameof(DataContext)} is not of type {typeof(CarFields).FullName}.");
            carFields.DisplayedDataFields = settings.DefaultDataFields;
        }

        private void MakeDefaultGaugeFields_Click(object sender, RoutedEventArgs e)
        {
            if (!(DataContext is CarFields carFields)) throw new InvalidOperationException($"{nameof(DataContext)} is not of type {typeof(CarFields).FullName}.");
            settings.DefaultGaugeFields = carFields.DisplayedGaugeFields;
        }

        private void UseDefaultGaugeFields_Click(object sender, RoutedEventArgs e)
        {
            if (!(DataContext is CarFields carFields)) throw new InvalidOperationException($"{nameof(DataContext)} is not of type {typeof(CarFields).FullName}.");
            carFields.DisplayedGaugeFields = settings.DefaultGaugeFields;
        }

        private async void UseDataFieldsFromOtherCar_Click(object sender, RoutedEventArgs e)
        {
            if (!(DataContext is CarFields carFields)) throw new InvalidOperationException($"{nameof(DataContext)} is not of type {typeof(CarFields).FullName}.");

            CarPickerWithFields dialog = new CarPickerWithFields(settings.CarFields.Keys.Where(x => x != carFields.CarId)) { Title = "Select car" };
            if (await dialog.ShowDialogAsync(this, SimHub.Plugins.UI.DialogOptions.CenterPrimaryScreen) != System.Windows.Forms.DialogResult.OK) return;

            settings.CarFields[carFields.CarId].DisplayedDataFields = settings.CarFields[dialog.SelectedCar].DisplayedDataFields;
        }

        private async void UseGaugeFieldsFromOtherCar_Click(object sender, RoutedEventArgs e)
        {
            if (!(DataContext is CarFields carFields)) throw new InvalidOperationException($"{nameof(DataContext)} is not of type {typeof(CarFields).FullName}.");

            CarPickerWithFields dialog = new CarPickerWithFields(settings.CarFields.Keys.Where(x => x != carFields.CarId)) { Title = "Select car" };
            if (await dialog.ShowDialogAsync(this, SimHub.Plugins.UI.DialogOptions.CenterPrimaryScreen) != System.Windows.Forms.DialogResult.OK) return;

            settings.CarFields[carFields.CarId].DisplayedGaugeFields = settings.CarFields[dialog.SelectedCar].DisplayedGaugeFields;
        }

        private async void DataFieldItem_Click(object sender, RoutedEventArgs e)
        {
            FieldPicker dialog = new FieldPicker(settings.DataFields.EnabledFieldsSortedByOrder()) { Title = "Select Data field" };
            if (await dialog.ShowDialogAsync(this, SimHub.Plugins.UI.DialogOptions.CenterPrimaryScreen) != System.Windows.Forms.DialogResult.OK) return;

            if (!(sender is MenuItem menuItem)) throw new InvalidOperationException($"{nameof(sender)} is not of type {typeof(MenuItem).FullName}.");
            if (!(menuItem.DataContext is FieldInformation info)) throw new InvalidOperationException($"{nameof(menuItem.DataContext)} is not of type {typeof(FieldInformation).FullName}.");
            if (!(DataContext is CarFields carFields)) throw new InvalidOperationException($"{nameof(DataContext)} is not of type {typeof(CarFields).FullName}.");

            settings.CarFields[carFields.CarId].DisplayedDataFields[info.Index] = dialog.SelectedDataField;
        }

        private void RemoveDataFieldItem_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is MenuItem menuItem)) throw new InvalidOperationException($"{nameof(sender)} is not of type {typeof(MenuItem).FullName}.");
            if (!(menuItem.DataContext is FieldInformation info)) throw new InvalidOperationException($"{nameof(menuItem.DataContext)} is not of type {typeof(FieldInformation).FullName}.");
            if (!(DataContext is CarFields carFields)) throw new InvalidOperationException($"{nameof(DataContext)} is not of type {typeof(CarFields).FullName}.");

            settings.CarFields[carFields.CarId].DisplayedDataFields.RemoveAt(info.Index);
        }

        private async void GaugeFieldItem_Click(object sender, RoutedEventArgs e)
        {
            FieldPicker dialog = new FieldPicker(settings.GaugeFields.EnabledFieldsSortedByOrder()) { Title = "Select Gauge field" };
            if (await dialog.ShowDialogAsync(this, SimHub.Plugins.UI.DialogOptions.CenterPrimaryScreen) != System.Windows.Forms.DialogResult.OK) return;

            if (!(sender is MenuItem menuItem)) throw new InvalidOperationException($"{nameof(sender)} is not of type {typeof(MenuItem).FullName}.");
            if (!(menuItem.DataContext is FieldInformation info)) throw new InvalidOperationException($"{nameof(menuItem.DataContext)} is not of type {typeof(FieldInformation).FullName}.");
            if (!(DataContext is CarFields carFields)) throw new InvalidOperationException($"{nameof(DataContext)} is not of type {typeof(CarFields).FullName}.");

            settings.CarFields[carFields.CarId].DisplayedGaugeFields[info.Index] = dialog.SelectedDataField;
        }

        private void RemoveGaugeFieldItem_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is MenuItem menuItem)) throw new InvalidOperationException($"{nameof(sender)} is not of type {typeof(MenuItem).FullName}.");
            if (!(menuItem.DataContext is FieldInformation info)) throw new InvalidOperationException($"{nameof(menuItem.DataContext)} is not of type {typeof(FieldInformation).FullName}.");
            if (!(DataContext is CarFields carFields)) throw new InvalidOperationException($"{nameof(DataContext)} is not of type {typeof(CarFields).FullName}.");

            settings.CarFields[carFields.CarId].DisplayedGaugeFields.RemoveAt(info.Index);
        }

    }
}
