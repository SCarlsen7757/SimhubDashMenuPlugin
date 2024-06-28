using System.Windows.Controls;

namespace DashMenu.UI
{
    /// <summary>
    /// Interaction logic for FieldDataSetting.xaml
    /// </summary>
    public partial class FieldDataSetting : UserControl
    {
        internal FieldDataSetting(Settings.Fields fields)
        {
            this.Content = fields;
            InitializeComponent();
        }
    }
}
