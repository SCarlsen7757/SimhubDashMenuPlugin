using System.Windows.Controls;

namespace DashMenu.UI
{
    /// <summary>
    /// Interaction logic for FieldDataSetting.xaml
    /// </summary>
    public partial class FieldDataSettingItem : UserControl
    {
        public FieldDataSettingItem()
        {
            InitializeComponent();
        }
        internal FieldDataSettingItem(DashMenu.Settings.Fields fields)
        {
            InitializeComponent();
            DataContext = fields;
            //Can't disable empty field plugin.
            if (fields.FullName == typeof(EmptyField).FullName)
            {
                EnabledCheckBox.IsEnabled = false;
            }
        }
    }
}
