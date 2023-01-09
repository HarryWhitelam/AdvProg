using Frontend;
using NUnit.Framework;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

// requires single thread apartments to support WPF implementations
[assembly: Apartment(ApartmentState.STA)]

namespace UnitTests
{
    public class ViewModelTest
    {
        private ViewModel _viewModel;

        [SetUp]
        public void Setup()
        {
            _viewModel = new ViewModel();
            if (Application.Current == null)
            {
                new Application { ShutdownMode = ShutdownMode.OnExplicitShutdown };
            }
            new MainWindow(true);
        }

        [TearDown]
        public void TearDown()
        {
            Application.Current.MainWindow.Close();
            Application.Current.Shutdown();
        }

        [TestCase("Test1", ExpectedResult = 1)]
        [TestCase("\r\n\r\n\r\nTest2\r\n\r\n", ExpectedResult = 1)]     // should remove empty entries
        [TestCase("This \r\n is a \r\n test string \r\n for test3", ExpectedResult = 4)]
        public int TestCountRichLines(string testValue)
        {
            return _viewModel.CountRichLines(testValue);
        }

        [TestCase("test1", 2, ExpectedResult = 'e')]
        [TestCase("test2", 0, ExpectedResult = '\n')]
        [TestCase("test3\r\n\r\nplaceholder", 9, ExpectedResult = 'p')]
        [TestCase("test4\r\n\r\nplaceholder", 8, ExpectedResult = '\n')]
        [TestCase("test5\r\n\r\nplaceholder", 20, ExpectedResult = 'r')]
        public char TestGetPriorChar(string tbText, int curIndex)
        {
            TextBox tb = new();
            tb.Text = tbText;
            return _viewModel.GetPriorChar(tb, curIndex);
        }

        [TestCase("test1", ExpectedResult = "")]
        [TestCase("test2\r\nremoveThisText", ExpectedResult = "test2\r\n")]
        [TestCase("test3\r\n\r\nplaceholder\r\n\r\nremoveThisText", ExpectedResult = "test3\r\n\r\nplaceholder\r\n\r\n")]
        [TestCase("test4\r\n\r\n\r\n\r\n", ExpectedResult = "test4\r\n\r\n\r\n\r\n")]
        public string TestRemoveCurrentLineText(string tbText)
        {
            TextBox tb = new();
            tb.Text = tbText;
            _viewModel.RemoveCurrentLineText(tb);

            return tb.Text;
        }

        [TestCase("test1", ExpectedResult = "test1")]
        [TestCase("test2\r\nplaceholder\r\ngetThisText", ExpectedResult = "getThisText")]
        [TestCase("test2\r\nplaceholder\r\nnotThisText\r\n", ExpectedResult = "")]
        public string TestGetPrompt(string tbText)
        {
            TextBox tb = new();
            tb.Text = tbText;
            return _viewModel.GetPrompt(tb);
        }

        [TestCase("10", "5+5", ExpectedResult = new string[] { "\r\n\r\n\r\n", ">>\r\n\r\n\r\n>>", "5+5\r    10\r\n\r\n" })]
        [TestCase("x:=5", "x:=5", ExpectedResult = new string[] { "\r\n\r\n\r\n", ">>\r\n\r\n\r\n>>", "x:=5\r    x:=5\r\n\r\n" })]
        [TestCase("", "", ExpectedResult = new string[] { "\r\n\r\n", ">>\r\n\r\n>>", "\r\n\r\n" })]
        public string[] TestPrintResult(string result, string prompt)
        {
            TextBox iw = (TextBox)Application.Current.MainWindow.FindName("inputWindow");
            TextBox cw = (TextBox)Application.Current.MainWindow.FindName("cursorWindow");
            RichTextBox pw = (RichTextBox)Application.Current.MainWindow.FindName("printWindow");

            iw.AppendText(prompt);
            _viewModel.PrintResult(result, prompt);
            string pwText = new TextRange(pw.Document.ContentStart, pw.Document.ContentEnd).Text;
            return new string[] { iw.Text, cw.Text, pwText };
        }

        [TestCase("Expected number, variable, or closing bracket here: \r   5+\r     ^", "5+", ExpectedResult = new string[] { "\r\n\r\n\r\n\r\n\r\n", ">>\r\n\r\n\r\n\r\n\r\n>>", "5+\rError: Expected number, variable, or closing bracket here: \r   5+\r     ^\r\n\r\n" })]
        public string[] TestPrintError(string error, string prompt)
        {
            TextBox iw = (TextBox)Application.Current.MainWindow.FindName("inputWindow");
            TextBox cw = (TextBox)Application.Current.MainWindow.FindName("cursorWindow");
            RichTextBox pw = (RichTextBox)Application.Current.MainWindow.FindName("printWindow");

            iw.AppendText(prompt);
            _viewModel.PrintError(error, prompt);
            string pwText = new TextRange(pw.Document.ContentStart, pw.Document.ContentEnd).Text;
            return new string[] { iw.Text, cw.Text, pwText };
        }

        [TestCase("x:=1", "x", "1", false, ExpectedResult = new bool[] { true, true, true })]
        [TestCase("x:=1", "y", "5", true, ExpectedResult = new bool[] { true, true, true })]
        public bool[] UpdateWorkstation(string testInput, string name, string value, bool changeValue)
        {
            TextBox iw = (TextBox)Application.Current.MainWindow.FindName("inputWindow");
            ListBox varNames = (ListBox)Application.Current.MainWindow.FindName("varNames");
            ListBox varValues = (ListBox)Application.Current.MainWindow.FindName("varValues");

            iw.AppendText(testInput);
            _viewModel.ReturnCommand.Execute(null);     //UpdateWorkstation called within ReturnCommand

            // Check for if updating values is also functioning correctly
            if (changeValue)
            {
                iw.AppendText(name + ":=10");
                _viewModel.ReturnCommand.Execute(null);
                value = "10";
            }

            bool indexTest = varNames.Items.IndexOf(name) == varValues.Items.IndexOf(value);

            return new bool[] { varNames.Items.Contains(name), varValues.Items.Contains(value), indexTest };
        }

        [TestCase(ExpectedResult = true)]
        public bool TestReturnCommand()
        {
            return _viewModel.ReturnCommand.CanExecute(null);
        }

        [TestCase("", 0, ExpectedResult = "")]
        [TestCase("\r\n", 0, ExpectedResult = "")]
        [TestCase("5+5", 0, ExpectedResult = "5+")]
        [TestCase("5+5", 2, ExpectedResult = "+5")]
        [TestCase("5+5", 3, ExpectedResult = "5+5")]
        public string TestBackCommand(string inputString, int offset)
        {
            TextBox iw = (TextBox)Application.Current.MainWindow.FindName("inputWindow");
            iw.AppendText(inputString);
            int index = iw.Text.Length - offset;
            if (index >= 0)
            {
                iw.SelectionStart = index;
            }
            else
                iw.SelectionStart = 0;
            _viewModel.BackCommand.Execute(null);
            return _viewModel.GetPrompt(iw);
        }

        [TestCase("", 0, ExpectedResult = 0)]
        [TestCase("\r\n\r\n\r\n", 0, ExpectedResult = 6)]
        [TestCase("\r\n\r\n\r\n5+5", 0, ExpectedResult = 8)]
        [TestCase("\r\n\r\n\r\n5+5", 3, ExpectedResult = 6)]
        public int TestLeftCommand(string inputString, int offset)
        {
            TextBox iw = (TextBox)Application.Current.MainWindow.FindName("inputWindow");
            iw.AppendText(inputString);
            iw.Select(iw.Text.Length - offset, 0);
            _viewModel.LeftCommand.Execute(null);
            return iw.SelectionStart;
        }

        [TestCase("x:=5", "x", "5", ExpectedResult = new bool[] { true, true })]
        [TestCase("y:=100", "y", "100", ExpectedResult = new bool[] { true, true })]
        public bool[] TestDelVarCommand(string testInput, string name, string value)
        {
            TextBox iw = (TextBox)Application.Current.MainWindow.FindName("inputWindow");
            ListBox varNames = (ListBox)Application.Current.MainWindow.FindName("varNames");
            ListBox varValues = (ListBox)Application.Current.MainWindow.FindName("varValues");
            bool[] results = new bool[] { false, false };

            //populate in order to remove
            iw.AppendText(testInput);
            _viewModel.ReturnCommand.Execute(null);
            // check both column selections are valid
            varNames.SelectedIndex = varNames.Items.IndexOf(name);
            _viewModel.DelVarCommand.Execute(null);
            if (!varNames.Items.Contains(name) && !varValues.Items.Contains(value))
                results[0] = true;

            // repeat for second columnn selection
            iw.AppendText(testInput);
            _viewModel.ReturnCommand.Execute(null);
            // check both column selections are valid
            varValues.SelectedIndex = varValues.Items.IndexOf(value);
            _viewModel.DelVarCommand.Execute(null);
            if (!varNames.Items.Contains(name) && !varValues.Items.Contains(value))
                results[1] = true;

            return results;
        }

        [TestCase(new string[] { "1+1", "" }, "1", ExpectedResult = "1+1")]
        [TestCase(new string[] { "1+1", "2+2", "3+3", "4+4", "5+5", "" }, "5", ExpectedResult = "5+5")]
        [TestCase(new string[] { "1+1", "2+2", "3+3", "4+4", "5+5", "6+6", "" }, "9", ExpectedResult = "")]
        public string TestUpHistoryCommand(string[] historyArray, int recursions)
        {
            TextBox iw = (TextBox)Application.Current.MainWindow.FindName("inputWindow");
            _viewModel.inputHistory = historyArray;

            for (int i=0; i < recursions; i++)  { _viewModel.UpHistoryCommand.Execute(null); }
            return _viewModel.GetPrompt(iw);
        }

        [TestCase(new string[] { "1+1", "2+2", "3+3", "4+4", "5+5", "" }, 2, 4, "", ExpectedResult = "2+2")]
        [TestCase(new string[] { "1+1", "2+2", "3+3", "4+4", "5+5", "" }, 2, 10, "", ExpectedResult = "4+4")]
        [TestCase(new string[] { "1+1", "2+2", "3+3", "4+4", "5+5", "" }, 4, 4, "6+6", ExpectedResult = "6+6")]
        public string TestDownHistoryCommand(string[] historyArray, int recursions, int upRecursions, string inputSave)
        {
            TextBox iw = (TextBox)Application.Current.MainWindow.FindName("inputWindow");
            _viewModel.inputSave = inputSave;
            _viewModel.inputHistory = historyArray;

            for (int i = 0; i < upRecursions; i++) { _viewModel.UpHistoryCommand.Execute(null); }
            for (int i = 0; i < recursions; i++) { _viewModel.DownHistoryCommand.Execute(null); }

            return _viewModel.GetPrompt(iw);
        }
    }
}