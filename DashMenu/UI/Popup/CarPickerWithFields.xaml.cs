using SimHub.Plugins.UI;
using System.Collections.Generic;
using System.Windows.Markup;

namespace DashMenu.UI.Popup
{
    /// <summary>
    /// Interaction logic for DataFieldPicker.xaml
    /// </summary>
    public partial class CarPickerWithFields : SHDialogContentBase, IComponentConnector
    {
        public CarPickerWithFields(IEnumerable<string> fields)
        {
            InitializeComponent();
            ListBoxFields.ItemsSource = fields;
        }

        public string SelectedCar { get; private set; }

        private void Select_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty((string)ListBoxFields.SelectedItem))
            {
                SelectedCar = (string)ListBoxFields.SelectedItem;
                Close(System.Windows.Forms.DialogResult.OK);
            }
            else
            {
                Close(System.Windows.Forms.DialogResult.Cancel);
            }
        }

        private void Close_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Close(System.Windows.Forms.DialogResult.Cancel);
        }
    }
}
