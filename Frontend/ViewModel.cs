using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Backend;
using System.Windows.Documents;
using System.Diagnostics;
using System.Windows.Media;

namespace AdvProg
{
    public class ViewModel
    {        
        public int CountRichLines(String resultString)
        {
            string[] splitLines = resultString.Split(new[] {'\r'}, StringSplitOptions.None);
            return splitLines.Length;
        }

        public void PrintResult(string result, string prompt)
        {
            TextBox inputWindow = (TextBox)Application.Current.MainWindow.FindName("inputWindow");
            TextBox cursorWindow = (TextBox)Application.Current.MainWindow.FindName("cursorWindow");
            RichTextBox printWindow = (RichTextBox)Application.Current.MainWindow.FindName("printWindow");

            string resultString = prompt + '\r' + "    " + result + Environment.NewLine;
            printWindow.AppendText(resultString);

            int nlIndex = inputWindow.Text.LastIndexOf('\n');
            if (nlIndex < 0)
            {
                nlIndex = 0;
            }
            inputWindow.Text = inputWindow.Text.Remove(nlIndex);

            int lineCount = CountRichLines(resultString);
            for (int i = 0; i < lineCount; i++)
            {
                inputWindow.AppendText(Environment.NewLine);
                cursorWindow.AppendText(Environment.NewLine);
            }
            inputWindow.SelectionStart = inputWindow.Text.Length;
            inputWindow.SelectionLength = 0;

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
            
            int nlIndex = inputWindow.Text.LastIndexOf('\n');
            if (nlIndex < 0)
            {
                nlIndex = 0;
            }
            inputWindow.Text = inputWindow.Text.Remove(nlIndex);

            int lineCount = CountRichLines(resultString + errorString);
            for (int i = 0; i < lineCount; i++)
            {
                inputWindow.AppendText(Environment.NewLine);
                cursorWindow.AppendText(Environment.NewLine);
            }
            inputWindow.SelectionStart = inputWindow.Text.Length;
            inputWindow.SelectionLength = 0;

            cursorWindow.AppendText(">>");
            cursorWindow.ScrollToEnd();
        }

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
                    
                    String input = inputWindow.GetLineText(inputWindow.LineCount-1);
                    if (input.Contains("plot"))
                    {

                    }
                    else if (input.Contains("error"))
                    {
                        PrintError("THIS IS AN ERROR BTW", input);
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
