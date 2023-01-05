using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Frontend;
using AdvProg;
using System.Collections;

namespace Frontend
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        UserSettings userSettings;
        string settingsFile = "../../../resources/UserSettings.xml";
        static string staticFile = "../../../resources/UserSettings.xml";
        public Settings(Theme theme)
        {
            InitializeComponent();
            SetTheme(theme);
            LoadSettings();
        }

        public Theme Theme { get; set; }

        public void SetTheme(Theme newTheme)
        {
            this.Theme = newTheme;
            this.Resources.MergedDictionaries[0].Source =
                new Uri($"/resources/themes/{Theme}.xaml", UriKind.Relative);
            MainWindow.Theme = newTheme;
            Application.Current.MainWindow.Resources.MergedDictionaries[0].Source =
                new Uri($"/resources/themes/{Theme}.xaml", UriKind.Relative);
        }

        public void LoadSettings()
        {
            userSettings = UserSettings.Read(this.settingsFile);
            switch (userSettings.settings[0])
            {
                case "Light":
                    lightRadio.IsChecked = true;
                    break;
                case "Dark":
                    darkRadio.IsChecked = true;
                    break;
                case "High Contrast":
                    hcRadio.IsChecked = true;
                    break;
            }
            FontSizeBox.Text = userSettings.settings[1];
            FontSizeSlider.Value = Convert.ToDouble(userSettings.settings[1]);
            FontComboBox.SelectedValue = userSettings.settings[2];
        }

        public static UserSettings GetSettings()
        {
            return UserSettings.Read(staticFile);
        }

        public void SaveSettings_Click(object sender, RoutedEventArgs e)
        {
            List<string> settingsList = new List<string>();
            settingsList.Add(FindInputs<RadioButton>(this).FirstOrDefault(n => (bool)n.IsChecked).Content.ToString());
            settingsList.Add(FontSizeSlider.Value.ToString());
            settingsList.Add(FontComboBox.SelectedValue.ToString());

            UserSettings userSettings = new UserSettings();
            userSettings.settings = settingsList;
            userSettings.Save(this.settingsFile);

            switch (userSettings.settings[0])
            {
                case "Light":
                    SetTheme(Theme.Light);
                    break;
                case "Dark":
                    SetTheme(Theme.Dark);
                    break;
                case "High Contrast":
                    SetTheme(Theme.HighContrast);
                    break;
            }
            Application.Current.MainWindow.FontSize = Convert.ToInt32(userSettings.settings[1]);
            Application.Current.MainWindow.FontFamily = new FontFamily(Convert.ToString(userSettings.settings[2]));

            this.Close();
        }

        public IEnumerable<T> FindInputs<T>(DependencyObject obj) where T : DependencyObject
        {
            if (obj == null) yield return (T)Enumerable.Empty<T>();
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child == null) continue;
                if (child is T t) yield return t;
                foreach (T childOfChild in FindInputs<T>(child)) yield return childOfChild;
            }
        }
    }
}
