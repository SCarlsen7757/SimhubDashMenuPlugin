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

        }
    }
}
