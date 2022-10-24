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
                return returnCommand ??= new ActionCommand(() =>
                {
                    Object varNames = Application.Current.MainWindow.FindName("varNames");
                    Object varValues = Application.Current.MainWindow.FindName("varValues");
                    Object inputWindow = Application.Current.MainWindow.FindName("inputWindow");

                    if ((varNames is ListBox) && (varValues is ListBox) && (inputWindow is TextBox))
                    {
                        ListBox names = (ListBox)varNames;
                        ListBox values = (ListBox)varValues;
                        TextBox iw = (TextBox)inputWindow;

                        String txt = iw.GetLineText(0);

                        if (txt.Contains("="))
                        {
                            String[] txtSplit = txt.Split("=");
                            if (!string.IsNullOrWhiteSpace(iw.Text) && !names.Items.Contains(txtSplit[0]))
                            {
                                names.Items.Add(txtSplit[0].Trim());
                                values.Items.Add(txtSplit[1].Trim());
                                iw.AppendText("\n");
                            }
                        }
                        else
                        {
                            iw.AppendText("\n");
                        }
                    }
                });
            }
        }

        private ICommand delVarCommand;
        public ICommand DelVarCommand
        {
            get
            {
                return delVarCommand ??= new ActionCommand(() =>
                {
                    ListBox varList = (ListBox) Application.Current.MainWindow.FindName("varList");

                    if (varList.SelectedIndex == -1)
                    {
                        MessageBox.Show("Error: please select a variable for deletion.");
                    }
                    else
                    {
                        if (varList.SelectedItems.Count > 1)
                        {
                            MessageBox.Show("Error: multi-deletion not implemented; please select one variable");
                        }
                        else
                        {
                            varList.Items.Remove(varList.SelectedItem);
                        }
                    }
                });
            }
        }
    }
}
