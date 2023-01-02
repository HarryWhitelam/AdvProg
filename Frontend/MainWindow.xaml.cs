using Frontend;
using System;
using System.Windows;
using System.Windows.Navigation;

namespace AdvProg
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new ViewModel();
            this.Theme = Theme.HighContrast; // HARD CODED IN BOTH MAINWINDOW FILES
        }

        public Theme Theme { get; set; }

        public void ChangeTheme(Theme newTheme)
        {
            this.Theme = newTheme;
            this.Resources.MergedDictionaries[0].Source =
                new Uri($"/resources/themes/{Theme}.xaml", UriKind.Relative);
        }

        private void ButtonDelVar_Click(object sender, RoutedEventArgs e)
        {
            if (Theme is Theme.Dark)
                ChangeTheme(Theme.Light);
            else if (Theme is Theme.Light)
                ChangeTheme(Theme.HighContrast);
            else if (Theme is Theme.HighContrast)
                ChangeTheme(Theme.Dark);

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
            else if (sender == logButton)
            {
                type = 3;
            }
            RootPopUp rootPopUp = new RootPopUp(type, this.Theme);
            rootPopUp.Show();
        }
    }
}
