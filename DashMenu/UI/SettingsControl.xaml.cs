using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace DashMenu.UI
{
    /// <summary>
    /// Interaction logic for SettingsControl.xaml
    /// </summary>
    public partial class SettingsControl : UserControl
    {
        internal SettingsControl(Settings.Settings settings, ObservableCollection<FieldComponent> allFieldData)
        {
            InitializeComponent();

            foreach (FieldComponent field in allFieldData)
            {
                this.AllFields.Children.Add(new FieldDataSetting(field));
            }
        }
    }
}
