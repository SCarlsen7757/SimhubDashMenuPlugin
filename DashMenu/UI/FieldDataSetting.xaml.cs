using System.Windows.Controls;

namespace DashMenu.UI
{
    /// <summary>
    /// Interaction logic for FieldDataSetting.xaml
    /// </summary>
    public partial class FieldDataSetting : UserControl
    {
        internal FieldDataSetting(DashMenu.Settings.Fields fields)
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
