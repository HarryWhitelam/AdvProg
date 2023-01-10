using Backend;
using Frontend;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml;
using System.Xml.Linq;

namespace Frontend
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private XDocument methods;
        /// <summary>
        /// Constructor <c>MainWindow</c> instantiates the main display, specifying Data Contexts (controllers) and preparing the user's settings
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new ViewModel();
            HelpMenu.DataContext = this;
            PrepSettings();
            methods = ReadHelpDocs();
        }

        private XDocument ReadHelpDocs()
        {
            return XDocument.Load("../../../../Frontend/resources/HelpDocs.xml");
        }

        private string search;
        /// <summary>
        /// Search get and set methods for the help menu
        /// </summary>
        public string Search
        {
            get
            {
                return search;
            }
            set
            {
                search = value;
                OnPropertyChanged("SearchText");
                OnPropertyChanged("SearchResults");
            }
        }

        public List<string> SearchList { get; set; }
        /// <summary>
        /// Method <c>SearchResults</c> returns an Enumerable of the filtered results from the SearchList
        /// </summary>
        public IEnumerable<string> SearchResults
        {
            get
            {
                if (Search == null) return methods.Descendants("method").Select(x => (string)x.Element("opName")).ToList();

                return methods.Descendants("method").Where(x => x.Attribute("tags").ToString().Contains(search)).Select(x => (string)x.Element("opName")).ToList();
            }
        }

        public static Theme Theme { get; set; }

        /// <summary>
        /// Method <c>PrepSettings</c> loads and establishes the user's settings from a saved file
        /// </summary>
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
            Application.Current.MainWindow.FontFamily = new FontFamily(Convert.ToString(us.settings[2]));
            this.Resources.MergedDictionaries[0].Source =
                new Uri($"/resources/themes/{Theme}.xaml", UriKind.Relative);
        }

        /// <summary>
        /// Method <c>SettingsButton_Click</c> opens the settings menu popup
        /// </summary>
        /// <param name="sender"><c>sender</c> provides information about the sender button</param>
        /// <param name="e"><c>e</c> provides event arguments</param>
        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            Settings settings = new Settings(Theme);
            settings.Show();
        }

        /// <summary>
        /// Method <c>HelpButton_Click</c> opens the help menu or closes it
        /// </summary>
        /// <param name="sender"><c>sender</c> provides information about the sender button</param>
        /// <param name="e"><c>e</c> provides event arguments</param>
        private void HelpButton_Click(object sender, RoutedEventArgs e)
        {
            if (HelpMenu.Visibility == Visibility.Collapsed)
            {
                WorkstationTextBlock.Text = "Help Menu";
                HelpMenu.Visibility = Visibility.Visible;
            }
            else
            {
                WorkstationTextBlock.Text = "Workstation";
                HelpMenu.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Method <c>ShortCut_Click</c> opens the relevant maths visualiser popup 
        /// </summary>
        /// <param name="sender"><c>sender</c> provides information about the sender button</param>
        /// <param name="e"><c>e</c> provides event arguments</param>
        private void Shortcut_Click(object sender, RoutedEventArgs e)
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

        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// Method <c>OnPropertyChanged</c> triggered when search help menu is changed
        /// </summary>
        /// <param name="element"><c>element</c> gives thee name of the xaml element which has changed</param>
        void OnPropertyChanged(string element)
        {
            Debug.WriteLine("Value: " + element);
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(element));
        }

        public void input_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (sender == inputWindow)
            {
                printWindow.ScrollToVerticalOffset(e.VerticalOffset);
                cursorWindow.ScrollToVerticalOffset(e.VerticalOffset);
            }
            else if (sender == printWindow)
            {
                inputWindow.ScrollToVerticalOffset(e.VerticalOffset);
                cursorWindow.ScrollToVerticalOffset(e.VerticalOffset);
            }
            else
            {
                inputWindow.ScrollToVerticalOffset(e.VerticalOffset);
                printWindow.ScrollToVerticalOffset(e.VerticalOffset);
            }
        }
    }
}
