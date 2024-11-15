using DashMenu.Settings;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
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

        private static bool ContainsFilter(string text, bool showHiddenItems, IBasicSettings settings)
        {
            if (!showHiddenItems && settings.Hide) return false;
            if (string.IsNullOrEmpty(text)) return true;
            string str1 = text;
            char[] chArray = new char[1] { ';' };
            foreach (string str2 in str1.Split(chArray))
            {
                if (LikeOperator.LikeString(settings.FullName.ToLower(), "*" + str2.Trim().ToLower() + "*", CompareMethod.Binary))
                    return true;
            }
            return false;
        }

        private void RefreshDataFieldList()
        {
            if (DesignerProperties.GetIsInDesignMode(this)) return;

            var settings = (Settings.GameSettings)DataContext;

            dataFieldView = CollectionViewSource.GetDefaultView(settings.DataFields.Values.ToList());
            dataFieldView.Filter = item => ContainsFilter(DataFieldFilter.Text, DataFieldHide.IsChecked ?? false, item as IBasicSettings);
            dataFieldView.GroupDescriptions.Add(new PropertyGroupDescription("Namespace"));
            dataFieldView.SortDescriptions.Add(new SortDescription("Namespace", ListSortDirection.Ascending));
            dataFieldView.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));

            FieldDataSettings.ItemsSource = dataFieldView;
        }

        private void RefreshAlertList()
        {
            if (DesignerProperties.GetIsInDesignMode(this)) return;

            var settings = (Settings.GameSettings)DataContext;

            alertView = CollectionViewSource.GetDefaultView(settings.Alerts.Values.ToList());
            alertView.GroupDescriptions.Add(new PropertyGroupDescription("Namespace"));
            alertView.Filter = item => ContainsFilter(AlertFilter.Text, AlertHide.IsChecked ?? false, item as IBasicSettings);
            alertView.SortDescriptions.Add(new SortDescription("Namespace", ListSortDirection.Ascending));
            alertView.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));

            AlertSettings.ItemsSource = alertView;
        }

        private void RefreshGaugeFieldList()
        {
            if (DesignerProperties.GetIsInDesignMode(this)) return;

            var settings = (Settings.GameSettings)DataContext;

            gaugeFieldView = CollectionViewSource.GetDefaultView(settings.GaugeFields.Values.ToList());
            gaugeFieldView.Filter = item => ContainsFilter(GaugeFieldFilter.Text, GaugeFieldHide.IsChecked ?? false, item as IBasicSettings);
            gaugeFieldView.GroupDescriptions.Add(new PropertyGroupDescription("Namespace"));
            gaugeFieldView.SortDescriptions.Add(new SortDescription("Namespace", ListSortDirection.Ascending));
            gaugeFieldView.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));

            FieldGaugeSettings.ItemsSource = gaugeFieldView;
        }

        private void DataFieldHide_CheckedChanged(object sender, RoutedEventArgs e)
        {
            RefreshDataFieldList();
        }

        private void DataFieldFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            RefreshDataFieldList();
        }

        private void AlertHide_CheckedChanged(object sender, RoutedEventArgs e)
        {
            RefreshAlertList();
        }

        private void AlertFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            RefreshAlertList();
        }

        private void GaugeFieldHide_CheckedChanged(object sender, RoutedEventArgs e)
        {
            RefreshGaugeFieldList();
        }
        private void GaugeFieldFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            RefreshGaugeFieldList();
        }
    }
}
