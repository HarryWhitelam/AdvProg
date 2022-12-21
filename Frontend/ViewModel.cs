using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Backend;
using System.Windows.Documents;
using System.Diagnostics;
using System.Windows.Media;
using System.Collections.Generic;
using System.Linq;

namespace AdvProg
{
    public class ViewModel
    {
        public string[] inputHistory = new string[1];
        public int historyIndex = -1;
        public string inputSave = "";

        public int CountRichLines(String resultString)
        {
            string[] splitLines = resultString.Split(new[] {'\r'}, StringSplitOptions.None);
            return splitLines.Length;
        }

        public void RemoveCurrentLineText(TextBox window)
        {
            if (window.GetLineText(window.LineCount - 1) != "")
            {
                window.Text = window.Text.Remove(window.Text.IndexOf(window.GetLineText(window.LineCount - 1)));
            }
        }

        public void PrintResult(string result, string prompt)
        {
            TextBox inputWindow = (TextBox)Application.Current.MainWindow.FindName("inputWindow");
            TextBox cursorWindow = (TextBox)Application.Current.MainWindow.FindName("cursorWindow");
            RichTextBox printWindow = (RichTextBox)Application.Current.MainWindow.FindName("printWindow");

            string resultString = prompt + '\r' + "    " + result + Environment.NewLine;
            printWindow.AppendText(resultString);

            RemoveCurrentLineText(inputWindow);

            int lineCount = CountRichLines(resultString);
            for (int i = 0; i < lineCount; i++)
            {
                inputWindow.AppendText(Environment.NewLine);
                cursorWindow.AppendText(Environment.NewLine);
            }
            inputWindow.ScrollToLine(inputWindow.LineCount-1);
            inputWindow.Select(inputWindow.Text.Length, 0);

            cursorWindow.AppendText(">>");
            cursorWindow.ScrollToEnd();
        }

        public void PrintError(string error, string prompt)
        {
            TextBox inputWindow = (TextBox)Application.Current.MainWindow.FindName("inputWindow");
            TextBox cursorWindow = (TextBox)Application.Current.MainWindow.FindName("cursorWindow");
            RichTextBox printWindow = (RichTextBox)Application.Current.MainWindow.FindName("printWindow");

            string resultString = prompt + '\r';
            string errorString = "    " + error + Environment.NewLine;
            printWindow.AppendText(resultString);
            TextRange errorRange = new TextRange(printWindow.Document.ContentEnd, printWindow.Document.ContentEnd);
            errorRange.Text = errorString;
            errorRange.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Red);

            RemoveCurrentLineText(inputWindow);

            int lineCount = CountRichLines(resultString + errorString);
            for (int i = 0; i < lineCount; i++)
            {
                inputWindow.AppendText(Environment.NewLine);
                cursorWindow.AppendText(Environment.NewLine);
            }
            inputWindow.ScrollToEnd();

            cursorWindow.AppendText(">>");
            cursorWindow.ScrollToEnd();
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
                    
                    String input = inputWindow.GetLineText(inputWindow.LineCount-1);
                    if (inputHistory[0] != input)
                    {
                        inputHistory = inputHistory.Prepend(input).ToArray();
                    }

                    if (input.Contains("plot"))
                    {

                    }
                    else if (input.Contains("error"))
                    {
                        PrintError("ERROR TEST <-- this text should display in red", input);
                    }
                    else
                    {
                        try
                        {
                            PrintResult(Interpreter.interpret(input), input);
                        }
                        catch (Exception ex)
                        {
                            PrintError(ex.Message, input);
                        }
                    }
                    inputSave = "";
                    historyIndex = -1;
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

        private ICommand upHistoryCommand;
        public ICommand UpHistoryCommand
        {
            get
            {
                return upHistoryCommand ??= new ActionCommand(() =>
                {
                    TextBox inputWindow = (TextBox)Application.Current.MainWindow.FindName("inputWindow");
                    if (inputSave == "")
                    {
                        inputSave = inputWindow.GetLineText(inputWindow.LineCount - 1);
                    }
                        if (inputHistory.Length > 0)
                        {
                            if (historyIndex < 9 && historyIndex < inputHistory.Count())
                            {
                                RemoveCurrentLineText(inputWindow);
                                historyIndex++;
                                inputWindow.AppendText(inputHistory[historyIndex]);
                            }
                        
                            inputWindow.SelectionStart = inputWindow.Text.Length;
                            inputWindow.SelectionLength = 0;
                        }
                });
            }
        }

        private ICommand downHistoryCommand;
        public ICommand DownHistoryCommand
        {
            get
            {
                return downHistoryCommand ??= new ActionCommand(() =>
                {
                    TextBox inputWindow = (TextBox)Application.Current.MainWindow.FindName("inputWindow");
                    if (inputHistory.Length > 0)
                    {
                        if (historyIndex == 0)
                        {
                            RemoveCurrentLineText(inputWindow);
                            historyIndex--;
                            inputWindow.AppendText(inputSave);
                        }
                        else if (historyIndex == -1)
                        {
                            RemoveCurrentLineText(inputWindow);
                        }
                        else if (historyIndex <= 9)
                        {
                            RemoveCurrentLineText(inputWindow);
                            historyIndex--;
                            inputWindow.AppendText(inputHistory[historyIndex]);
                        }
                        
                        inputWindow.SelectionStart = inputWindow.Text.Length;
                        inputWindow.SelectionLength = 0;
                    }
                });
            }
        }
    }
}
