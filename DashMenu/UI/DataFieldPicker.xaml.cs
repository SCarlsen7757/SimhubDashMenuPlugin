using SimHub.Plugins.UI;
using System.Collections.Generic;
using System.Windows.Markup;

namespace DashMenu.UI
{
    /// <summary>
    /// Interaction logic for DataFieldPicker.xaml
    /// </summary>
    public partial class FieldPicker : SHDialogContentBase, IComponentConnector
    {
        public FieldPicker(IList<string> fields)
        {
            InitializeComponent();
            ListBoxFields.ItemsSource = fields;
        }

        public string SelectedDataField { get; private set; }

        private void Select_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty((string)ListBoxFields.SelectedItem))
            {
                SelectedDataField = (string)ListBoxFields.SelectedItem;
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
