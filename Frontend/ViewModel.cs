using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Backend;
using System.Windows.Documents;
using System.Diagnostics;
using System.Windows.Media;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using static Microsoft.FSharp.Core.LanguagePrimitives;
using System.Text;

namespace AdvProg
{
    /// <summary>
    /// Class <c>ViewModel</c> is used to control various elements of the GUI
    /// </summary>
    public class ViewModel
    {
        public string[] inputHistory = new string[1];
        public int historyIndex = -1;
        public string inputSave = "";

        /// <summary>
        /// Method <c>CountRichLines</c> is used to count the number of lines the print will require
        /// </summary>
        /// <param name="resultString"></param> the string to be measured
        /// <returns>Number of lines</returns>
        public int CountRichLines(String resultString)
        {
            string[] splitLines = resultString.Split(new[] {'\n', '\r'}, StringSplitOptions.RemoveEmptyEntries);
            Debug.WriteLine("LINES: " + splitLines.Length);
            return splitLines.Length;
        }

        /// <summary>
        /// Method <c>GetPriorChar</c> returns the character in the textbox one index before the cursor. 
        /// </summary>
        /// <param name="textbox"></param> the textbox to be addressed
        /// <param name="curIndex"></param> the current index of the cursor/selection
        /// <returns></returns>
        public char GetPriorChar(TextBox textbox, int curIndex)
        {
            if (curIndex > 0)
            {
                textbox.Select(curIndex - 1, 1);        // manual selection to ensure only one character is selected
                string res = textbox.SelectedText;
                textbox.Select(curIndex, 0);
                if (res.Length == 1)                    // error checking for conversion
                {
                    return Convert.ToChar(res);
                }
                else
                {
                    return '\n';
                }
            }
            else
                return '\n';
        }

        /// <summary>
        /// Method <c>RemoveCurrentLineText</c> removes the current entered line text in a given TextBox
        /// </summary>
        /// <param name="textbox"></param> the textbox that will be affected
        public void RemoveCurrentLineText(TextBox textbox)
        {
            if (GetPrompt(textbox) != "")
            {
                textbox.Text = textbox.Text.Remove(textbox.Text.IndexOf(GetPrompt(textbox)));
            }
        }

        public string GetPrompt(TextBox textbox)
        {
            string[] lines = textbox.Text.Split(new char[] { '\n' });
            return lines[lines.Length - 1];
        }

        /// <summary>
        /// Method <c>PrintResults</c> takes the input string and moves it to the PrintWindow along with the result
        /// </summary>
        /// <param name="result"></param> the result string to be printed
        /// <param name="prompt"></param> the prompt which the user entered
        public void PrintResult(string result, string prompt)
        {
            TextBox inputWindow = (TextBox)Application.Current.MainWindow.FindName("inputWindow");
            TextBox cursorWindow = (TextBox)Application.Current.MainWindow.FindName("cursorWindow");
            RichTextBox printWindow = (RichTextBox)Application.Current.MainWindow.FindName("printWindow");

            string resultString = prompt + '\r' + "    " + result + Environment.NewLine;
            TextRange errorRange = new TextRange(printWindow.Document.ContentEnd, printWindow.Document.ContentEnd);
            errorRange.Text = resultString;

            RemoveCurrentLineText(inputWindow);

            int lineCount = CountRichLines(resultString);
            for (int i = 0; i <= lineCount; i++)
            {
                inputWindow.AppendText("\n");
                cursorWindow.AppendText("\n");
            }
            inputWindow.Select(inputWindow.Text.Length, 0);

            cursorWindow.AppendText(">>");
            cursorWindow.ScrollToEnd();
            printWindow.ScrollToEnd();
        }

        /// <summary>
        /// Method <c>PrintError</c> is used when an error is printed, this is done in red with some formatting
        /// </summary>
        /// <param name="error"></param> the error message to be printed
        /// <param name="prompt"></param> the prompt which the user entered
        public void PrintError(string error, string prompt)
        {
            TextBox inputWindow = (TextBox)Application.Current.MainWindow.FindName("inputWindow");
            TextBox cursorWindow = (TextBox)Application.Current.MainWindow.FindName("cursorWindow");
            RichTextBox printWindow = (RichTextBox)Application.Current.MainWindow.FindName("printWindow");

            string resultString = prompt + '\r';
            string errorString = "Error: " + error + Environment.NewLine;
            printWindow.AppendText(resultString);
            TextRange errorRange = new TextRange(printWindow.Document.ContentEnd, printWindow.Document.ContentEnd);
            errorRange.Text = errorString;
            errorRange.ApplyPropertyValue(TextElement.ForegroundProperty, (SolidColorBrush)Application.Current.MainWindow.Resources.MergedDictionaries[0]["ErrorBrush"]);

            RemoveCurrentLineText(inputWindow);

            int lineCount = CountRichLines(resultString + errorString);
            for (int i = 0; i <= lineCount; i++)
            {
                inputWindow.AppendText("\n");
                cursorWindow.AppendText("\n");
            }
            inputWindow.Select(inputWindow.Text.Length, 0);

            cursorWindow.AppendText(">>");
            cursorWindow.ScrollToEnd();
            printWindow.ScrollToEnd();
        }

        /// <summary>
        /// Method <c>UpdateWorkstation</c> is used to synchronise the GUI workstation with the backend VariableStore
        /// </summary>
        public void UpdateWorkstation()
        {
            ListBox varNames = (ListBox)Application.Current.MainWindow.FindName("varNames");
            ListBox varValues = (ListBox)Application.Current.MainWindow.FindName("varValues");

            //Checks each pair to ensure all are present
            foreach (KeyValuePair<string, string> entry in Interpreter.getVarStore())
            {
                if (!varNames.Items.Contains(entry.Key))
                {
                    varNames.Items.Add(entry.Key);
                    varValues.Items.Add(entry.Value);
                }
                else //replaces already assigned keys
                {
                    varValues.Items[varNames.Items.IndexOf(entry.Key)] = entry.Value;
                }
            }
        }

        private ICommand returnCommand;
        /// <summary>
        /// Method <c>ReturnCommand</c> is used with the return keybind to send prompts to the backend
        /// </summary>
        public ICommand ReturnCommand
        {
            get
            {
                return returnCommand ??= new ActionCommand(() =>
                {
                    ListBox varNames = (ListBox) Application.Current.MainWindow.FindName("varNames");
                    ListBox varValues = (ListBox) Application.Current.MainWindow.FindName("varValues");
                    TextBox inputWindow = (TextBox) Application.Current.MainWindow.FindName("inputWindow");

                    int lc = inputWindow.LineCount;
                    string input = GetPrompt(inputWindow);

                    if (inputHistory[0] != input)
                    {
                        inputHistory = inputHistory.Prepend(input).ToArray();
                    }
                    if (input.Contains("plot"))
                    {
                        // creating a Regex expression to pick up the y=mx+c pattern
                        string strtlinePattern = @"plot y=\dx\+\d";
                        string polynomialPattern = @"\dx\^[0-9]+.*\dx.*\d";
                        Match strtline = Regex.Match(input, strtlinePattern, RegexOptions.IgnoreCase);
                        Match polynomial = Regex.Match(input, polynomialPattern, RegexOptions.IgnoreCase);

                        // if the pattern matches the input for a straight line
                        if (strtline.Success)
                        {
                            //Splitting the equation from the plot
                            string[] inputArray = input.Split(" ");
                            // split the "plot" from the "y=mx+c"
                            for (int i = 1; i < inputArray.Length; i++)
                            {
                                string output = inputArray[i];
                                // printing the test 
                                PrintResult(output, output);
                            }
                            string leng = Convert.ToString(inputArray.Length);
                            PrintResult(leng, leng);
                        }
                        else if (polynomial.Success)
                        {
                            string splitpattern = @"plot|y|=|\+|-";
                            string[] inputArray = Regex.Split(input, splitpattern);
                            for (int i = 0; i < inputArray.Length; i++)
                            {
                                string output = inputArray[i];
                                // printing the test 
                                PrintResult(output, output);
                            }
                            string result = string.Concat(inputArray);
                            PrintResult(result, result);
                            string leng = Convert.ToString(inputArray.Length);
                            PrintResult(leng, leng);
                        }
                        Frontend.OxyplotGraphWindow OPGW = new Frontend.OxyplotGraphWindow();
                        OPGW.Show();
                    }
                    else
                    {
                        try
                        {
                            var variableStore = Interpreter.getVarStore();
                            var result = Interpreter.interpret(input);
                            PrintResult(result, input);
                            if (result != null && result.Contains(":="))
                            {
                                UpdateWorkstation();
                            }
                        }
                        catch (Exception ex)
                        {
                            PrintError(ex.Message[(ex.Message.IndexOf("\"") + 1)..ex.Message.Length], input);
                        }
                    }
                    inputSave = "";
                    historyIndex = -1;
                });
            }
        }

        private ICommand backCommand;
        /// <summary>
        /// Method <c>BackCommand</c> is used with the backspace keybind to handle character deletion. This ensures the user cannot delete previous prompts/move lines
        /// </summary>
        public ICommand BackCommand
        {
            get
            {
                return backCommand ??= new ActionCommand(() =>
                {
                    TextBox inputWindow = (TextBox)Application.Current.MainWindow.FindName("inputWindow");
                    string currentText = inputWindow.GetLineText(inputWindow.LineCount - 1);

                    if (currentText != "")
                    {
                        RemoveCurrentLineText(inputWindow);
                        inputWindow.AppendText(currentText.Remove(currentText.Length - 1));

                        inputWindow.SelectionStart = inputWindow.Text.Length;
                        inputWindow.SelectionLength = 0;
                    }
                });
            }
        }

        private ICommand leftCommand;
        /// <summary>
        /// Method <c>LeftCommand</c> is used with the left-arrow keybind to handle line-selection. This ensures the user cannot ascend/descend lines incorrectly. 
        /// </summary>
        public ICommand LeftCommand
        {
            get
            {
                return leftCommand ??= new ActionCommand(() =>
                {
                    TextBox inputWindow = (TextBox)Application.Current.MainWindow.FindName("inputWindow");
                    int currentSelection = inputWindow.SelectionStart;
                    char priorChar = GetPriorChar(inputWindow, currentSelection);

                    if (priorChar != '\n')
                    {
                        inputWindow.Select(currentSelection - 1, 0);
                    }
                });
            }
        }

        private ICommand delVarCommand;
        /// <summary>
        /// Method <c>DelVarCommand</c> is used to delete a variable from the Workstation and hence the VariableStore
        /// </summary>
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
                        Interpreter.removeVarStore((string)varNames.Items[index]);
                        varNames.Items.Remove(varNames.Items.GetItemAt(index));
                        varValues.Items.Remove(varValues.Items.GetItemAt(index));
                    }
                });
            }
        }

        private ICommand upHistoryCommand;
        /// <summary>
        /// Method <c>UpHistoryCommand</c> is used with the up-arrow keybind to handle history viewing (ascends through the history array). 
        /// </summary>
        public ICommand UpHistoryCommand
        {
            get
            {
                return upHistoryCommand ??= new ActionCommand(() =>
                {
                    TextBox inputWindow = (TextBox)Application.Current.MainWindow.FindName("inputWindow");
                    if (inputSave == "" && historyIndex == -1)
                    {
                        inputSave = inputWindow.GetLineText(inputWindow.LineCount - 1);
                    }
                    if (inputHistory[0] != null)
                    {
                        if (historyIndex < 9 && historyIndex < inputHistory.Length - 1)
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
        /// <summary>
        /// Method <c>DownHistoryCommand</c> is used to descend through the history array using the down-arrow keybind. 
        /// </summary>
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
                            inputSave = "";
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
