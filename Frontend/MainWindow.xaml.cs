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

        private void ButtonDelVar_Click(object sender, RoutedEventArgs e)
        {
            if ((varNames.SelectedIndex == -1) && (varValues.SelectedIndex == -1))
            {
                MessageBox.Show("Error: please select a variable for deletion.");
            }
            else if ((varNames.SelectedItems.Count > 1) || (varValues.SelectedItems.Count > 1))
            {
                MessageBox.Show("Error: multi-deletion not implemented; please select one variable");
            }
            else
            {
                int index = varNames.SelectedIndex;
                // ensures vars can be deleted from either column
                if (index == -1)
                {
                    index = varValues.SelectedIndex;
                }
                varNames.Items.Remove(varNames.Items.GetItemAt(index));
                varValues.Items.Remove(varValues.Items.GetItemAt(index));
            }
//             Frontend.GraphWindow graphWindow = new Frontend.GraphWindow();
//             graphWindow.Show();
        }

        private void RootShortcut_Click(object sender, RoutedEventArgs e)
        {
            RootPopUp rootPopUp = new RootPopUp();
            rootPopUp.Show();
        }
    }
}
