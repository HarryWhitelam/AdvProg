using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

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

        public void ProcessShortcut(int type, double[] values)
        {
            TextBox inputWindow = (TextBox) Application.Current.MainWindow.FindName("inputWindow");

            switch (type)
            {
                // ROOT
                case 1:
                    inputWindow.AppendText("root(" + values[1] + ", " + values[0] + ")");
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
                ProcessShortcut(1, inputs);
                this.Close();
            }
        }
    }
}
