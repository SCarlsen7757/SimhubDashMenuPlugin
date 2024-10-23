using System.Windows;
using System.Windows.Controls;

namespace DashMenu.UI
{
    /// <summary>
    /// Interaction logic for SettingsControl.xaml
    /// </summary>
    public partial class SettingsControl : UserControl
    {
        internal SettingsControl(Settings.GameSettings settings)
        {
            InitializeComponent();
            DataContext = settings;
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
    }
}
