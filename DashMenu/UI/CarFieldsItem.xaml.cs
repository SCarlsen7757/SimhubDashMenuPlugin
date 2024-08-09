using System.Windows.Controls;

namespace DashMenu.UI
{
    /// <summary>
    /// Interaction logic for CarFieldSettingItem.xaml
    /// </summary>
    public partial class CarFieldSettingItem : UserControl
    {
        private readonly bool contentInitialized = false;

        public CarFieldSettingItem()
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
