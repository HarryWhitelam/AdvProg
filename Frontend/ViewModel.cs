using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Backend;
using System.Windows.Media;
using System.Windows.Documents;

namespace AdvProg
{
    public class ViewModel
    {
        public int CountRichLines(RichTextBox box)
        {
            TextRange boxText = new TextRange(box.Document.ContentStart, box.Document.ContentEnd);

            string[] splitLines = boxText.Text.Split(new[] {Environment.NewLine}, StringSplitOptions.None);
            return splitLines.Length;
        }

        public void PrintResult(string result, string prompt)
        {
            TextBox inputWindow = (TextBox)Application.Current.MainWindow.FindName("inputWindow");
            TextBox cursorWindow = (TextBox)Application.Current.MainWindow.FindName("cursorWindow");
            RichTextBox printWindow = (RichTextBox)Application.Current.MainWindow.FindName("printWindow");

            string resultString = new string(new TextRange(printWindow.Document.ContentStart, printWindow.Document.ContentEnd).Text + prompt + "\n    " + result);
            Console.Write("PRINT STARTS HERE: " + resultString);
            printWindow.AppendText(resultString);

            int lineCount = CountRichLines(printWindow);
            inputWindow.Text = "";
            for (int i = 0; i < lineCount; i++)
            {
                inputWindow.AppendText("\n");
            }
            inputWindow.SelectionStart = inputWindow.Text.Length;
            inputWindow.SelectionLength = 0;

            cursorWindow.AppendText("\n\n\n>>");
            cursorWindow.ScrollToEnd();
        }

        public void PrintError(string error)
        {
            TextBox inputWindow = (TextBox)Application.Current.MainWindow.FindName("inputWindow");
            TextBox cursorWindow = (TextBox)Application.Current.MainWindow.FindName("cursorWindow");
            RichTextBox printWindow = (RichTextBox)Application.Current.MainWindow.FindName("printWindow");


            FlowDocument flowDoc = new FlowDocument();
            Run resRun = new Run(new TextRange(printWindow.Document.ContentStart, printWindow.Document.ContentEnd).Text + inputWindow.Text);
            Run errRun = new Run("\n   " + error + "\n\n");
            Paragraph resPara = new Paragraph();
            errRun.Foreground = Brushes.Red;
            resPara.Inlines.Add(resRun);
            resPara.Inlines.Add(errRun);
            flowDoc.Blocks.Add(resPara);
            printWindow.Document = flowDoc;

            int lineCount = CountRichLines(printWindow);
            inputWindow.Text = "";
            for (int i = 0; i <= lineCount; i++)
            {
                inputWindow.AppendText("\n");
            }
            inputWindow.SelectionStart = inputWindow.Text.Length;
            inputWindow.SelectionLength = 0;

            cursorWindow.AppendText("\n\n\n>>");
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
                        PrintError(input);
                    }
                    else
                    {
                        try
                        {
                            PrintResult(Interpreter.interpret(input), input);
                        }
                        catch (Exception ex)
                        {
                            PrintError(ex.Message);
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
