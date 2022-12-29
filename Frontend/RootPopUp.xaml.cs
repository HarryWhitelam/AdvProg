using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AdvProg
{
    public partial class RootPopUp : Window
    {
        // 1 - root
        // 2 - power
        // 3 - log
        int type;
        public RootPopUp(int type)
        {
            InitializeComponent();
            DataContext = new ViewModel();
            this.type = type;
            ConstructFields();
        }

        public void ConstructFields()
        {
            switch (type)
            {
                case 1:
                    // root
                    TextRow.Text = "Root Function:";
                    popUpImg.Source = new BitmapImage(new Uri("resources/root.png", UriKind.Relative));
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
                    popUpImg.Source = new BitmapImage();
                    input2.Margin = new Thickness(187, 87, 117, 147);
                    input1.Margin = new Thickness(150, 122, 150, 95);
                    break;
                case 3:
                    // log
                    TextRow.Text = "Logarithm Function:";
                    popUpImg.Source = new BitmapImage(new Uri("resources/log.png", UriKind.Relative));
                    popUpImg.Margin = new Thickness(-93, 97, 93, 97);
                    input2.Margin = new Thickness(185, 107, 75, 127);
                    input1.Margin = new Thickness(135, 142, 165, 90);
                    break;
            }
        }

        public IEnumerable<T> FindInputs<T>(DependencyObject obj) where T:DependencyObject
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

        public void ProcessShortcut(double[] values)
        {
            TextBox inputWindow = (TextBox) Application.Current.MainWindow.FindName("inputWindow");

            switch (type)
            {
                // ROOT
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

        private void ShortCutSubmit_Click(object sender, RoutedEventArgs e)
        {
            bool errorBool = false;
            int numInputs = 2;
            double doubleInput;
            double[] inputs = new double[numInputs];

            IEnumerable<TextBox> tbList = FindInputs<TextBox>(this);

            int count = 0;
            foreach (TextBox textBox in FindInputs<TextBox>(this))
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
