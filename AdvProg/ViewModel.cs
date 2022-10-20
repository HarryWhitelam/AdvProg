using Microsoft.Xaml.Behaviors.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AdvProg
{
    public class ViewModel
    {
        private ICommand testCommand;
        public ICommand TestCommand
        {
            get
            {
                return testCommand
                    ?? (testCommand = new ActionCommand(() =>
                    {
                        MessageBox.Show("TEST SUCCESS!");
                    }));
            }
        }

        private ICommand returnCommand;
        public ICommand ReturnCommand
        {
            get
            {
                return returnCommand
                    ?? (returnCommand = new ActionCommand(() =>
                    {
                        Object varList = Application.Current.MainWindow.FindName("varList");
                        Object txt = Application.Current.MainWindow.FindName("varName");

                        if ((varList is ListBox) && (txt is TextBox))
                        {
                            ListBox lb = (ListBox)varList;
                            TextBox tb = (TextBox)txt;

                            if (!string.IsNullOrWhiteSpace(tb.Text) && !lb.Items.Contains(tb.Text))
                            {
                                lb.Items.Add(tb.Text);
                                tb.Clear();
                            }
                        }
                    }));
            }
        }
    }
}
