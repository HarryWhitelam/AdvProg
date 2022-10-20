using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AdvProg
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ButtonAddVar_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(varName.Text) && !varList.Items.Contains(varName.Text))
            {
                varList.Items.Add(varName.Text);
                varName.Clear();
            }
        }
    }
    class MessageCommand : ICommand
    {
        public void Execute(object parameter)
        {
            string message;

            if (parameter == null)
                message = "COMMAND!";
            else
                message = parameter.ToString();

            MessageBox.Show(message);
        }
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;
    }

    class ReturnCommand : ICommand
    {
        public void Execute(object parameter)
        {
            MessageBox.Show(Application.Current.MainWindow.FindName("varList").ToString());
            Object varList = Application.Current.MainWindow.FindName("varList");
            Object txt = Application.Current.MainWindow.FindName("varName");

            if ((varList is ListBox) && (txt is TextBox))
            {
                ListBox lb = (ListBox) varList;
                TextBox tb = (TextBox)txt;

                if (!string.IsNullOrWhiteSpace(tb.Text) && !lb.Items.Contains(tb.Text))
                {
                    lb.Items.Add(tb.Text);
                    tb.Clear();
                }
            }

        }
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;
    }
}
