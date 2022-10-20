using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AdvProg
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new ViewModel();
        }

        private void ButtonAddVar_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(varName.Text) && !varList.Items.Contains(varName.Text))
            {
                varList.Items.Add(varName.Text);
                varName.Clear();
            }
        }
        private void ButtonDelVar_Click(object sender, RoutedEventArgs e)
        {
            if (varList.SelectedIndex == -1)
            {
                MessageBox.Show("Error: please select a variable for deletion.");
            }
            else
            {
                if (varList.SelectedItems.Count > 1)
                {
                    MessageBox.Show("Error: multi-deletion not implemented; please select one variable");
                }
                else
                {
                    varList.Items.Remove(varList.SelectedItem);
                }
            }
        }
    }
}
