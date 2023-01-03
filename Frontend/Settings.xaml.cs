using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Frontend;

namespace Frontend
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        UserSettings userSettings;
        string settingsFile = "./resources/UserSettings.xml";
        string settingsFile1 = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "resources/UserSettings.xml"));
        string settingsFile2 = AppDomain.CurrentDomain.BaseDirectory;
        public Settings()
        {
            InitializeComponent();
            LoadSettings();
        }
        
        public void LoadSettings()
        {
            userSettings = UserSettings.Read(settingsFile);
            Debug.WriteLine(userSettings.ToString());
        }

        public void SaveSettings_Click(object sender, RoutedEventArgs e)
        {
            List<string> settingsList = new List<string>();
            settingsList.Add(FindInputs<RadioButton>(this).FirstOrDefault(n => (bool)n.IsChecked).Content.ToString());
            settingsList.Add(FontSizeSlider.Value.ToString());
            settingsList.Add(FontComboBox.SelectedItem.ToString());

            UserSettings userSettings = new UserSettings(settingsList);
            userSettings.Save(settingsFile);
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
