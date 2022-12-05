using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Diagnostics;
using Backend;

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
                    ListBox varNames = (ListBox) Application.Current.MainWindow.FindName("varNames");
                    ListBox varValues = (ListBox) Application.Current.MainWindow.FindName("varValues");
                    TextBox inputWindow = (TextBox) Application.Current.MainWindow.FindName("inputWindow");
                    TextBox cursorWindow = (TextBox) Application.Current.MainWindow.FindName("cursorWindow");
                    
                    String input = inputWindow.GetLineText(inputWindow.LineCount-1);
                    if (input.Contains("plot"))
                    {

                    }
                    else
                    {
                        String answer;
                        try
                        {
                            answer = Interpreter.interpret(input);
                        }
                        catch (Exception ex)
                        {
                            answer = ex.Message;
                        }
                        inputWindow.AppendText("\n");
                        inputWindow.AppendText("   " + answer + "\n\n");
                        inputWindow.SelectionStart = inputWindow.Text.Length;
                        inputWindow.SelectionLength = 0;

                        cursorWindow.AppendText("\n\n\n>>");
                        cursorWindow.ScrollToEnd();
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
                    ListBox varNames = (ListBox) Application.Current.MainWindow.FindName("varNames");
                    ListBox varValues = (ListBox) Application.Current.MainWindow.FindName("varValues");

                    if ((varNames.SelectedIndex == -1) && (varValues.SelectedIndex == -1))
                    {
                        MessageBox.Show("Error: please select a variable for deletion.");
                    }
                    else if ((varNames.SelectedItems.Count > 1) || (varValues.SelectedItems.Count > 1))
                    {
                        MessageBox.Show("Error: multi-deletion not implemented; please select one variable");
                    }
                    else
                    {
                        int index = varNames.SelectedIndex;
                        // ensures vars can be deleted from either column
                        if (index == -1)
                        {
                            index = varValues.SelectedIndex;
                        }
                        varNames.Items.Remove(varNames.Items.GetItemAt(index));
                        varValues.Items.Remove(varValues.Items.GetItemAt(index));
                    }
                });
            }
        }

        public void ProcessShortcut(int type, double[] values)
        {
            
        }
    }
}
