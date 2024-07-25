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

        private void UserControl_DataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            var field = DataContext as DashMenu.Settings.Fields;
            if (field == null) return;
            //Can't disable empty field plugin.

            EnabledCheckBox.IsEnabled = !(field.FullName == typeof(EmptyField).FullName);
        }
    }
}
