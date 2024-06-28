using System.Windows.Controls;

namespace DashMenu.UI
{
    /// <summary>
    /// Interaction logic for SettingsControl.xaml
    /// </summary>
    public partial class SettingsControl : UserControl
    {
        public SettingsControl(Settings.Settings settings)
        {
            InitializeComponent();

            foreach (Settings.Fields field in settings.Fields)
            {
                this.AllFields.Children.Add(new FieldDataSetting(field));
            }
        }
    }
}
