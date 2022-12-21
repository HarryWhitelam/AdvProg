using System.Windows;

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
            int type = 0;
            if (sender == rootButton)
            {
                type = 1;
            }
            else if (sender == powerButton)
            {
                type = 2;
            }
            RootPopUp rootPopUp = new RootPopUp(type);
            rootPopUp.Show();
        }
    }
}
