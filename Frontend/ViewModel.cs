using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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
                    MessageBox.Show("WORKING");

                    Object varNames = Application.Current.MainWindow.FindName("varNames");
                    Object varValues = Application.Current.MainWindow.FindName("varValues");
                    Object inputWindow = Application.Current.MainWindow.FindName("inputWindow");

                    if ((varNames is ListBox) && (varValues is ListBox) && (inputWindow is TextBox))
                    {
                        ListBox names = (ListBox)varNames;
                        ListBox values = (ListBox)varValues;
                        TextBox iw = (TextBox)inputWindow;

                        String txt = iw.GetLineText(iw.LineCount-1);
                        txt = txt.Substring(1);

                        Microsoft.FSharp.Collections.FSharpList<Token> tokens = Lexer.lex(txt);
                        tokens = Parser.parse(tokens);

                        MessageBox.Show(Interpreter.interpret(txt).ToString());

                        
                        
                        
                        //MessageBox.Show(Lexer.lex(txt).ToString());


                        //if (txt.Contains("="))
                        //{
                        //    String[] txtSplit = txt.Split("=");
                        //    if (!string.IsNullOrWhiteSpace(iw.Text) && !names.Items.Contains(txtSplit[0]))
                        //    {
                        //        names.Items.Add(txtSplit[0].Substring(1).Trim());
                        //        values.Items.Add(txtSplit[1].Trim());

                        //        iw.AppendText("\n>");
                        //        iw.SelectionStart = iw.Text.Length;
                        //        iw.SelectionLength = 0;
                        //    }
                        //}
                        //else
                        //{
                        //    iw.AppendText("\n>");
                        //    iw.SelectionStart = iw.Text.Length;
                        //    iw.SelectionLength = 0;
                        //}
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
    }
}
