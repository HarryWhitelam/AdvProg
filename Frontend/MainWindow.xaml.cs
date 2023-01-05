using Backend;
using Frontend;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;

namespace AdvProg
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new ViewModel();
            PrepSettings();
        }

        public static Theme Theme { get; set; }

        public void PrepSettings()
        {
            UserSettings us = Settings.GetSettings();
            switch (us.settings[0])
            {
                case "Light":
                    Theme = Theme.Light;
                    break;
                case "Dark":
                    Theme = Theme.Dark;
                    break;
                case "High Contrast":
                    Theme = Theme.HighContrast;
                    break;
            }
            Application.Current.MainWindow.FontSize = Convert.ToInt32(us.settings[1]);
            FontFamily ff = new FontFamily(Convert.ToString(us.settings[2]));
            Debug.WriteLine("FONT FAMILY: " + ff.Source);
            Application.Current.MainWindow.FontFamily = ff;
            this.Resources.MergedDictionaries[0].Source =
                new Uri($"/resources/themes/{Theme}.xaml", UriKind.Relative);
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
                Interpreter.removeVarStore((string)varNames.Items[index]);
                varNames.Items.Remove(varNames.Items.GetItemAt(index));
                varValues.Items.Remove(varValues.Items.GetItemAt(index));
            }
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            Settings settings = new Settings(Theme);
            settings.Show();
        }

        private void HelpButton_Click(object sender, RoutedEventArgs e)
        {

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
            PopUp PopUp = new PopUp(type, Theme);
            PopUp.Show();
        }
    }
}
