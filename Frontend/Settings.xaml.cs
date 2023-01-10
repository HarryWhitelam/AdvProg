using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Frontend;
using System.Collections;
using System.Windows.Shapes;

namespace Frontend
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        // attributes definition
        UserSettings userSettings;
        string settingsFile = "../../../../Frontend/resources/UserSettings.xml";
        static string staticFile = "../../../../Frontend/resources/UserSettings.xml";

        /// <summary>
        /// Constructor <c>Settings</c> instantiates a Settings window with the relevant theme and prefills
        /// </summary>
        /// <param name="theme"><c>theme</c> the current theme of the system</param>
        public Settings(Theme theme)
        {
            InitializeComponent();
            SetTheme(theme);
            LoadSettings();
        }

        public Theme Theme { get; set; }

        /// <summary>
        /// Method <c>SetTheme</c> changes the current theme to the theme specified by the parameter given
        /// </summary>
        /// <param name="newTheme"><c>newTheme</c> the new theme to be applied</param>
        public void SetTheme(Theme newTheme)
        {
            this.Theme = newTheme;
            this.Resources.MergedDictionaries[0].Source =
                new Uri($"/resources/themes/{Theme}.xaml", UriKind.Relative);
            MainWindow.Theme = newTheme;
            Application.Current.MainWindow.Resources.MergedDictionaries[0].Source =
                new Uri($"/resources/themes/{Theme}.xaml", UriKind.Relative);
        }

        /// <summary>
        /// Method <c>LoadSettings</c> imports the user's saved settings from an external file. <see href="resources/UserSettings.xml" />
        /// </summary>
        public void LoadSettings()
        {
            userSettings = UserSettings.Read(this.settingsFile);
            Ellipse ellipse = null;
            switch (userSettings.settings[0])
            {
                case "Light":
                    lightRadio.IsChecked = true;
                    ellipse = FindInputs<Ellipse>((RadioButton)lightRadio).FirstOrDefault();
                    break;
                case "Dark":
                    darkRadio.IsChecked = true;
                    ellipse = FindInputs<Ellipse>((RadioButton)darkRadio).FirstOrDefault();
                    break;
                case "High Contrast":
                    hcRadio.IsChecked = true;
                    ellipse = FindInputs<Ellipse>((RadioButton)hcRadio).FirstOrDefault();
                    break;
            }
            if (ellipse != null)
                ellipse.SetResourceReference(Shape.FillProperty, "CaretBrush");

            FontSizeBox.Text = userSettings.settings[1];
            FontSizeSlider.Value = Convert.ToDouble(userSettings.settings[1]);
            FontComboBox.SelectedValue = userSettings.settings[2];
        }

        /// <summary>
        /// Method <c>GetSettings</c> is a static method for getting the current settings. <see href="resources/UserSettings.xml" />
        /// </summary>
        /// <returns>An instance of UserSettings, populated by the XML file</returns>
        public static UserSettings GetSettings()
        {
            return UserSettings.Read(staticFile);
        }

        /// <summary>
        /// Method <c>SaveSettings_Click</c> saves any changes the user has made to their settings file. <see href="resources/UserSettings.xml" />
        /// </summary>
        /// <param name="sender"><c>sender</c> provides information about the sender button</param>
        /// <param name="e"><c>e</c> provides event arguments</param>
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

        public void RadioButton_Checked(object sender, RoutedEventArgs e)
        {            
            Ellipse ellipse = FindInputs<Ellipse>((RadioButton)sender).FirstOrDefault();
            if (ellipse != null)
                ellipse.SetResourceReference(Shape.FillProperty, "CaretBrush");

        }

        /// <summary>
        /// Method <c>FindElements</c> returns a list of the chosen elements found on the popup
        /// </summary>
        /// <typeparam name="T">TypeParameter <c>T</c> providees the type of element to search for</typeparam>
        /// <param name="obj"><c>obj</c> provides the object to search</param>
        /// <returns>An Enumerable of the requested elements</returns>
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
