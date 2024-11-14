using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace DashMenu.UI
{
    /// <summary>
    /// Interaction logic for SettingsControl.xaml
    /// </summary>
    public partial class SettingsControl : UserControl
    {

        ICollectionView dataFieldView;
        ICollectionView alertView;
        ICollectionView gaugeFieldView;

        internal SettingsControl(Settings.GameSettings settings)
        {
            InitializeComponent();
            DataContext = settings;

            foreach (var dataField in settings.DataFields.Values)
            {
                dataField.PropertyChanged += DataFieldPropertyChanged;
            }
            RefreshDataFieldList();

            foreach (var alert in settings.Alerts.Values)
            {
                alert.PropertyChanged += AlertPropertyChanged;
            }
            RefreshAlertList();

            foreach (var gaugeField in settings.GaugeFields.Values)
            {
                gaugeField.PropertyChanged += GaugeFieldPropertyChanged;
            }
            RefreshGaugeFieldList();
        }

        private void DataFieldPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is Settings.DataField)
            {
                Settings.DataField dataField;
                if (e.PropertyName == nameof(dataField.Hide))
                {
                    RefreshDataFieldList();
                }
            }
        }

        private void AlertPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is Settings.Alert)
            {
                Settings.Alert dataField;
                if (e.PropertyName == nameof(dataField.Hide))
                {
                    RefreshAlertList();
                }
            }
        }

        private void GaugeFieldPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is Settings.GaugeField)
            {
                Settings.GaugeField dataField;
                if (e.PropertyName == nameof(dataField.Hide))
                {
                    RefreshGaugeFieldList();
                }
            }
        }

        private void ButtonForgetAllCars_Click(object sender, RoutedEventArgs e)
        {
            var settings = (Settings.GameSettings)DataContext;
            settings.RemoveCar();
        }

        private void ButtonForgetDefaultDataFields_Click(object sender, RoutedEventArgs e)
        {
            var settings = (Settings.GameSettings)DataContext;
            settings.DefaultDataFields.Clear();
        }
        private void ButtonForgetDefaultGaugeFields_Click(object sender, RoutedEventArgs e)
        {
            var settings = (Settings.GameSettings)DataContext;
            settings.DefaultGaugeFields.Clear();
        }

        private void RefreshDataFieldList()
        {
            if (DesignerProperties.GetIsInDesignMode(this)) return;

            var settings = (Settings.GameSettings)DataContext;
            var fields = (DataFieldHide.IsChecked ?? false)
                ? settings.DataFields.Values.ToList()
                : settings.DataFields.Values.Where(x => !x.Hide).ToList();

            dataFieldView = CollectionViewSource.GetDefaultView(fields);
            dataFieldView.GroupDescriptions.Add(new PropertyGroupDescription("Namespace"));
            dataFieldView.SortDescriptions.Add(new SortDescription("Namespace", ListSortDirection.Ascending));
            dataFieldView.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));

            FieldDataSettings.ItemsSource = dataFieldView;
        }

        private void RefreshAlertList()
        {
            if (DesignerProperties.GetIsInDesignMode(this)) return;

            var settings = (Settings.GameSettings)DataContext;
            var alerts = (AlertHide.IsChecked ?? false)
                ? settings.Alerts.Values.ToList()
                : settings.Alerts.Values.Where(x => !x.Hide).ToList();

            alertView = CollectionViewSource.GetDefaultView(alerts);
            alertView.GroupDescriptions.Add(new PropertyGroupDescription("Namespace"));
            alertView.SortDescriptions.Add(new SortDescription("Namespace", ListSortDirection.Ascending));
            alertView.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));

            AlertSettings.ItemsSource = alertView;
        }

        private void RefreshGaugeFieldList()
        {
            if (DesignerProperties.GetIsInDesignMode(this)) return;

            var settings = (Settings.GameSettings)DataContext;
            var fields = (GaugeFieldHide.IsChecked ?? false)
                ? settings.GaugeFields.Values.ToList()
                : settings.GaugeFields.Values.Where(x => !x.Hide).ToList();

            gaugeFieldView = CollectionViewSource.GetDefaultView(fields);
            gaugeFieldView.GroupDescriptions.Add(new PropertyGroupDescription("Namespace"));
            gaugeFieldView.SortDescriptions.Add(new SortDescription("Namespace", ListSortDirection.Ascending));
            gaugeFieldView.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));

            FieldGaugeSettings.ItemsSource = gaugeFieldView;
        }

        private void DataFieldHide_CheckedChanged(object sender, RoutedEventArgs e)
        {
            RefreshDataFieldList();
        }

        private void AlertHide_CheckedChanged(object sender, RoutedEventArgs e)
        {
            RefreshAlertList();
        }

        private void GaugeFieldHide_CheckedChanged(object sender, RoutedEventArgs e)
        {
            RefreshGaugeFieldList();
        }
    }
}
