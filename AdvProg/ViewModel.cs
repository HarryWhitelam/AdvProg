using Microsoft.Xaml.Behaviors.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace AdvProg
{
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
            
            if (varList is ListBox)

        }
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;
    }

    public class MyDataContext
    {
        ICommand _returnCommand = new ReturnCommand();

        public ICommand ReturnCommand
        {
            get { return _returnCommand; }
        }
    }
}
