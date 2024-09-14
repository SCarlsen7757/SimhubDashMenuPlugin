using DashMenu.Settings;
using System.Windows.Controls;

namespace DashMenu.UI
{
    /// <summary>
    /// Interaction logic for GaugeFieldSettingsItem.xaml
    /// </summary>
    public partial class GaugeFieldSettingsItem : UserControl
    {
        private readonly bool contentInitialized = false;
        public GaugeFieldSettingsItem()
        {
            InitializeComponent();
            contentInitialized = true;
        }
        private void UserControl_DataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            if (!contentInitialized) return;
            if (!(DataContext is GaugeField data)) return;

            Visibility = data.IsRangeLocked && data.IsStepLocked ? System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible;

            MaximumSection.Visibility = data.IsRangeLocked ? System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible;
            MinimumSection.Visibility = data.IsRangeLocked ? System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible;

            StepSection.Visibility = data.IsStepLocked ? System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible;
        }
    }
}
