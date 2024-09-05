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
            var settings = (Settings.GameSettings)this.DataContext;
            settings.RemoveAllDisplayedFields();
        }

        private void ButtonForgetDefaultFields_Click(object sender, RoutedEventArgs e)
        {
            var settings = (Settings.GameSettings)this.DataContext;
            settings.DefaultDataFields.Clear();
        }
    }
}
