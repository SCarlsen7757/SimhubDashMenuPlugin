using SimHub.Plugins;
using System.Windows;
using System.Windows.Controls;

namespace DashMenu.UI
{
    /// <summary>
    /// Interaction logic for SettingsControl.xaml
    /// </summary>
    public partial class SettingsControl : UserControl
    {
        internal SettingsControl(Settings.Settings settings)
        {
            InitializeComponent();
            DataContext = settings;
            FirstCarDataFieldSetting.ItemsSource = settings.GameSettings[PluginManager.GetInstance().GameName].CarFields;
        }
        private void ButtonForgetAllCars_Click(object sender, RoutedEventArgs e)
        {
            var settings = (Settings.Settings)this.DataContext;
            settings.RemoveAllDisplayedFields(PluginManager.GetInstance().GameName);
        }
    }
}
