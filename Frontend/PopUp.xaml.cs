using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Frontend;

namespace Frontend
{
    public partial class PopUp : Window
    {
        // type: the variant of popup window (which operation it facilitates)
        // 1 - root
        // 2 - power
        // 3 - log
        int type;

        /// <summary>
        /// Constructor <c>PopUp</c> establishes themes, data contexts and the layout of the popup
        /// </summary>
        /// <param name="type"><c>type</c> the variant of popup window (which operation it facilitates)</param>
        /// <param name="theme"><c>theme</c> the current theme of the system</param>
        public PopUp(int type, Theme theme)
        {
            InitializeComponent();
            DataContext = new ViewModel();
            SetTheme(theme);
            this.type = type;
            ConstructFields();
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
        }

        /// <summary>
        /// Method <c>ConstructFields</c> adjusts the layout to suit the type of the popup
        /// </summary>
        public void ConstructFields()
        {
            switch (type)
            {
                case 1:
                    // root
                    TextRow.Text = "Root Function:";
                    if (this.Theme == Theme.Light)
                        popUpImg.Source = new BitmapImage(new Uri("/resources/root.png", UriKind.Relative));
                    else
                        popUpImg.Source = new BitmapImage(new Uri("/resources/rootInvert.png", UriKind.Relative));
                    input2.Width = 50;
                    input2.Height = 22;
                    input2.Margin = new Thickness(89, 102, 186, 142);
                    input1.Width = 75;
                    input1.Height = 22;
                    input1.Margin = new Thickness(193, 117, 57, 117);
                    break;
                case 2:
                    // power
                    TextRow.Text = "Power Function:";
                    input2.Margin = new Thickness(187, 87, 117, 147);
                    input1.Margin = new Thickness(150, 122, 150, 95);
                    break;
                case 3:
                    // log
                    TextRow.Text = "Logarithm Function:";
                    if (this.Theme == Theme.Light)
                        popUpImg.Source = new BitmapImage(new Uri("/resources/log.png", UriKind.Relative));
                    else
                        popUpImg.Source = new BitmapImage(new Uri("/resources/logInvert.png", UriKind.Relative));
                    popUpImg.Margin = new Thickness(-93, 97, 93, 97);
                    input2.Margin = new Thickness(185, 107, 75, 127);
                    input1.Margin = new Thickness(135, 142, 165, 90);
                    break;
            }
        }

        /// <summary>
        /// Method <c>FindElements</c> returns a list of the chosen elements found on the popup
        /// </summary>
        /// <typeparam name="T">TypeParameter <c>T</c> providees the type of element to search for</typeparam>
        /// <param name="obj"><c>obj</c> provides the object to search</param>
        /// <returns>An Enumerable of the requested elements</returns>
        public IEnumerable<T> FindElements<T>(DependencyObject obj) where T:DependencyObject
        {
            if (obj == null) yield return (T)Enumerable.Empty<T>();
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child == null) continue;
                if (child is T t) yield return t;
                foreach (T childOfChild in FindElements<T>(child)) yield return childOfChild;
            }
        }

        /// <summary>
        /// Method <c>ProcessShortcut</c> converts the values from the popup into a function call for the interpreter
        /// </summary>
        /// <param name="values"><c>values</c> is an array of the values taken form the inputs</param> 
        public void ProcessShortcut(double[] values)
        {
            TextBox inputWindow = (TextBox) Application.Current.MainWindow.FindName("inputWindow");

            switch (type)
            {
                case 1:
                    inputWindow.AppendText("root(" + values[1] + ", " + values[0] + ")");
                    break;
                case 2:
                    inputWindow.AppendText(values[1] + "^" + values[0]);
                    break;
                case 3:
                    inputWindow.AppendText("log(" + values[1] + ", " + values[0] + ")");
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Method <c>ShortCutSubmit_Click</c> gathers the users inputs and passes them to a list. <see cref="ProcessShortcut"/>
        /// </summary>
        /// <param name="sender"><c>sender</c> provides information about the sender button</param>
        /// <param name="e"><c>e</c> provides event arguments</param>
        private void ShortCutSubmit_Click(object sender, RoutedEventArgs e)
        {
            bool errorBool = false;
            int numInputs = 2;
            double doubleInput;
            double[] inputs = new double[numInputs];

            IEnumerable<TextBox> tbList = FindElements<TextBox>(this);

            int count = 0;
            foreach (TextBox textBox in FindElements<TextBox>(this))
            {
                if ((double.TryParse(textBox.Text, out doubleInput)) && (doubleInput != 0))
                {
                    inputs[count] = doubleInput;
                    count++;
                }
                else errorBool = true;
            }

            if (errorBool)
            {
                InputWarning.Visibility = Visibility.Visible;
            }
            else
            {
                ProcessShortcut(inputs);
                this.Close();
            }
        }
    }
}
