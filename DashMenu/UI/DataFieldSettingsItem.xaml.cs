using System.Windows.Controls;

namespace DashMenu.UI
{
    /// <summary>
    /// Interaction logic for FieldDataSetting.xaml
    /// </summary>
    public partial class DataFieldSettingsItem : UserControl
    {
        private readonly bool contentInitialized = false;
        public DataFieldSettingsItem()
        {
            InitializeComponent();
            contentInitialized = true;
        }

        private void UserControl_DataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            if (!contentInitialized) return;
            if (DataContext is Settings.GaugeField gaugeField)
            {
                //Can't disable empty gauge field extension.
                EnabledCheckBox.IsEnabled = !(gaugeField.FullName == EmptyGaugeField.FullName);
            }
            else if (DataContext is Settings.DataField dataField)
            {
                //Can't disable empty data field extension.
                EnabledCheckBox.IsEnabled = !(dataField.FullName == EmptyDataField.FullName);
            }
        }
    }
}
