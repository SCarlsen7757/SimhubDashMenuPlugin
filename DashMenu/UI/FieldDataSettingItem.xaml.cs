using System.Windows.Controls;

namespace DashMenu.UI
{
    /// <summary>
    /// Interaction logic for FieldDataSetting.xaml
    /// </summary>
    public partial class FieldDataSettingItem : UserControl
    {
        private readonly bool contentInitialized = false;
        public FieldDataSettingItem()
        {
            InitializeComponent();
            contentInitialized = true;
        }

        private void UserControl_DataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            if (!contentInitialized) return;
            if (!(DataContext is Settings.DataFields field)) return;
            //Can't disable empty field plugin.
            EnabledCheckBox.IsEnabled = !(field.FullName == typeof(EmptyDataField).FullName);
        }
    }
}
