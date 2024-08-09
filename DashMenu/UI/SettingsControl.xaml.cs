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
        private PluginManager PluginManager { get; set; }
        internal SettingsControl(PluginManager pluginManager, Settings.Settings settings)
        {
            PluginManager = pluginManager;
            InitializeComponent();
            DataContext = settings;
            FirstCarDataFieldSetting.ItemsSource = settings.GameSettings[pluginManager.GameName].CarFields;
        }
        private void ButtonForgetAllCars_Click(object sender, RoutedEventArgs e)
        {
            var settings = (Settings.Settings)this.DataContext;
            settings.RemoveAllDisplayedFields(PluginManager.GameName);
        }
    }
}
