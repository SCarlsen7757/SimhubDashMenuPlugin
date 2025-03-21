﻿using DashMenu.Settings;
using DashMenu.UI.Popup;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

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

        /// <summary>
        /// Ref to the settings object.
        /// </summary>
        readonly Settings.GameSettings settings;

        internal SettingsControl(Settings.GameSettings settings)
        {
            this.settings = settings;

            InitializeComponent();
            DataContext = settings;

            settings.DefaultDataFields.CollectionChanged += DefaultDataFields_CollectionChanged;
            DefaultDataFields_CollectionChanged();

            settings.DefaultGaugeFields.CollectionChanged += DefaultGaugeFields_CollectionChanged;
            DefaultGaugeFields_CollectionChanged();

            foreach (var dataField in settings.DataFields.Settings.Values)
            {
                dataField.PropertyChanged += DataFieldPropertyChanged;
            }
            RefreshDataFieldList();

            foreach (var alert in settings.Alerts.Values)
            {
                alert.PropertyChanged += AlertPropertyChanged;
            }
            RefreshAlertList();

            foreach (var gaugeField in settings.GaugeFields.Settings.Values)
            {
                gaugeField.PropertyChanged += GaugeFieldPropertyChanged;
            }
            RefreshGaugeFieldList();

            CarFieldSettingItem.Settings = settings;
        }

        private void DefaultDataFields_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            DefaultDataFields_CollectionChanged();
        }

        private void DefaultDataFields_CollectionChanged()
        {
            DefaultFieldButtonBehavior(settings.DefaultDataFields, ButtonMakeDefaultDataFields, ButtonForgetDefaultDataFields);
            ItemsControlDefaultDataField.ItemsSource = FieldInformation.ItemsControlSource(settings.DefaultDataFields, settings.DataFields);
        }

        private void DefaultGaugeFields_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            DefaultGaugeFields_CollectionChanged();
        }

        private void DefaultGaugeFields_CollectionChanged()
        {
            DefaultFieldButtonBehavior(settings.DefaultGaugeFields, ButtonMakeDefaultGaugeFields, ButtonForgetDefaultGaugeFields);
            ItemsControlDefaultGaugeField.ItemsSource = FieldInformation.ItemsControlSource(settings.DefaultGaugeFields, settings.GaugeFields);
        }

        private static void DefaultFieldButtonBehavior(IList<string> fields, UIElement makeDefaultButton, UIElement forgetDefaultButton)
        {
            makeDefaultButton.Visibility = fields.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
            forgetDefaultButton.Visibility = fields.Count != 0 ? Visibility.Visible : Visibility.Collapsed;
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
            settings.RemoveCar();
        }

        private void ButtonForgetDefaultDataFields_Click(object sender, RoutedEventArgs e)
        {
            settings.DefaultDataFields.Clear();
        }

        private void ButtonMakeDefaultDataFields_Click(object sender, RoutedEventArgs e)
        {
            settings.DefaultDataFields = new ObservableCollection<string>(settings.DefaultDataFieldsList());
        }

        private void ButtonMakeDefaultGaugeFields_Click(object sender, RoutedEventArgs e)
        {
            settings.DefaultGaugeFields = new ObservableCollection<string>(settings.DefaultGaugeFieldsList());
        }

        private void ButtonForgetDefaultGaugeFields_Click(object sender, RoutedEventArgs e)
        {
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

            dataFieldView = CollectionViewSource.GetDefaultView(settings.DataFields.Settings.Values.ToList());
            dataFieldView.Filter = item => ContainsFilter(DataFieldFilter.Text, DataFieldHide.IsChecked ?? false, item as IBasicSettings);
            dataFieldView.GroupDescriptions.Add(new PropertyGroupDescription(nameof(Settings.IBasicSettings.Namespace)));
            dataFieldView.SortDescriptions.Add(new SortDescription(nameof(Settings.IBasicSettings.Namespace), ListSortDirection.Ascending));
            dataFieldView.SortDescriptions.Add(new SortDescription(nameof(Settings.IBasicSettings.Name), ListSortDirection.Ascending));

            FieldDataSettings.ItemsSource = dataFieldView;
        }

        private void RefreshAlertList()
        {
            if (DesignerProperties.GetIsInDesignMode(this)) return;

            alertView = CollectionViewSource.GetDefaultView(settings.Alerts.Values.ToList());
            alertView.Filter = item => ContainsFilter(AlertFilter.Text, AlertHide.IsChecked ?? false, item as IBasicSettings);
            alertView.GroupDescriptions.Add(new PropertyGroupDescription(nameof(Settings.IBasicSettings.Namespace)));
            alertView.SortDescriptions.Add(new SortDescription(nameof(Settings.IBasicSettings.Namespace), ListSortDirection.Ascending));
            alertView.SortDescriptions.Add(new SortDescription(nameof(Settings.IBasicSettings.Name), ListSortDirection.Ascending));

            AlertSettings.ItemsSource = alertView;
        }

        private void RefreshGaugeFieldList()
        {
            if (DesignerProperties.GetIsInDesignMode(this)) return;

            gaugeFieldView = CollectionViewSource.GetDefaultView(settings.GaugeFields.Settings.Values.ToList());
            gaugeFieldView.Filter = item => ContainsFilter(GaugeFieldFilter.Text, GaugeFieldHide.IsChecked ?? false, item as IBasicSettings);
            gaugeFieldView.GroupDescriptions.Add(new PropertyGroupDescription(nameof(Settings.IBasicSettings.Namespace)));
            gaugeFieldView.SortDescriptions.Add(new SortDescription(nameof(Settings.IBasicSettings.Namespace), ListSortDirection.Ascending));
            gaugeFieldView.SortDescriptions.Add(new SortDescription(nameof(Settings.IBasicSettings.Name), ListSortDirection.Ascending));

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

        private Point dataFieldDragStartPoint;

        private void DataFieldOrderListBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is ListBox)) return;
            dataFieldDragStartPoint = e.GetPosition(null);
        }

        private void DataFieldOrderListBox_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed) return;

            Point mousePos = e.GetPosition(null);
            Vector diff = dataFieldDragStartPoint - mousePos;

            if (Math.Abs(diff.X) <= SystemParameters.MinimumHorizontalDragDistance &&
                 Math.Abs(diff.Y) <= SystemParameters.MinimumVerticalDragDistance) return;

            if (!(sender is ListBox listBox) || listBox.SelectedItem == null) return;

            DragDrop.DoDragDrop(listBox, listBox.SelectedItem, DragDropEffects.Move);
        }

        private void FieldOrderListBox_Drop(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Move;
        }

        private void FieldOrderListBox_DragOver(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(typeof(string)) || !(sender is ListBox listBox)) return;
            if (!(e.Data.GetData(typeof(string)) is string droppedData) || !(((FrameworkElement)e.OriginalSource).DataContext is string target) || ReferenceEquals(droppedData, target)) return;

            ObservableCollection<string> items = listBox.ItemsSource as ObservableCollection<string>;
            int removedIdx = items.IndexOf(droppedData);
            int targetIdx = items.IndexOf(target);

            if (removedIdx == -1 || targetIdx == -1) return;
            items.Move(removedIdx, targetIdx);
        }

        private Point gaugeFieldDragStartPoint;

        private void GaugeFieldOrderListBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is ListBox)) return;
            gaugeFieldDragStartPoint = e.GetPosition(null);
        }

        private void GaugeFieldOrderListBox_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed) return;

            Point mousePos = e.GetPosition(null);
            Vector diff = gaugeFieldDragStartPoint - mousePos;

            if (Math.Abs(diff.X) <= SystemParameters.MinimumHorizontalDragDistance &&
                 Math.Abs(diff.Y) <= SystemParameters.MinimumVerticalDragDistance) return;

            if (!(sender is ListBox listBox) || listBox.SelectedItem == null) return;

            DragDrop.DoDragDrop(listBox, listBox.SelectedItem, DragDropEffects.Move);
        }

        private async void DefaultDataFieldItem_Click(object sender, RoutedEventArgs e)
        {
            FieldPicker dialog = new FieldPicker(settings.DataFields.EnabledFieldsSortedByOrder()) { Title = "Select Data field" };
            if (await dialog.ShowDialogAsync(this, SimHub.Plugins.UI.DialogOptions.CenterPrimaryScreen) != System.Windows.Forms.DialogResult.OK) return;

            // Get the MenuItem that was clicked
            var menuItem = sender as MenuItem;
            var info = menuItem?.DataContext as FieldInformation;
            if (menuItem == null && info == null) return;

            settings.DefaultDataFields[info.Index] = dialog.SelectedDataField;
        }

        private async void DefaultGaugeFieldItem_Click(object sender, RoutedEventArgs e)
        {
            FieldPicker dialog = new FieldPicker(settings.GaugeFields.EnabledFieldsSortedByOrder()) { Title = "Select Gauge field" };
            if (await dialog.ShowDialogAsync(this, SimHub.Plugins.UI.DialogOptions.CenterPrimaryScreen) != System.Windows.Forms.DialogResult.OK) return;

            var menuItem = sender as MenuItem;
            var info = menuItem?.DataContext as FieldInformation;
            if (menuItem == null && info == null) return;

            settings.DefaultGaugeFields[info.Index] = dialog.SelectedDataField;
        }
    }
}
