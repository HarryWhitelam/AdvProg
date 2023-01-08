using Frontend;
using NUnit.Framework;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

[assembly: Apartment(ApartmentState.STA)]

namespace UnitTests
{
    public class Tests
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

        [TestCase("This \n is a \n test string \n for testing", ExpectedResult = 4)]
        [TestCase("Test2", ExpectedResult = 1)]
        public int TestCountRichLines(string testValue)
        {
            return _viewModel.CountRichLines(testValue);
        }

        [TestCase("test1", 2, ExpectedResult = 'e')]
        [TestCase("test2", 0, ExpectedResult = '\n')]
        public char TestGetPriorChar(string tbText, int curIndex)
        {
            TextBox tb = new();
            tb.Text = tbText;
            return _viewModel.GetPriorChar(tb, curIndex);
        }

        [TestCase("test1", ExpectedResult = "")]
        [TestCase("test2\nremoveThisText", ExpectedResult = "test2\n")]
        [TestCase("test3\n\nplaceholder\n\nremoveThisText", ExpectedResult = "test3\n\nplaceholder\n\n")]
        [TestCase("test4\n\n\n\n", ExpectedResult = "test4\n\n\n\n")]
        public string TestRemoveCurrentLineText(string tbText)
        {
            TextBox tb = new();
            tb.Text = tbText;
            _viewModel.RemoveCurrentLineText(tb);

            return tb.Text;
        }

        [TestCase("test1", ExpectedResult = "test1")]
        [TestCase("test2\n\nplaceholder\n\ngetThisText", ExpectedResult = "getThisText")]
        public string TestGetPrompt(string tbText)
        {
            TextBox tb = new();
            tb.Text = tbText;
            return _viewModel.GetPrompt(tb);
        }

        [TestCase("10", "5+5", ExpectedResult = new string[] { "\r\n\r\n\r\n", ">>\r\n\r\n\r\n>>", "5+5\r    10\r\n\r\n" })]
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

        [TestCase("Expected number, variable, or closing bracket here: \r   5+\r     ^", "5+", ExpectedResult = new string[] { "\n\n\n\n\n", ">>\n\n\n\n\n>>", "5+\rError: Expected number, variable, or closing bracket here: \r   5+\r     ^\r\n\r\n" })]
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
            _viewModel.ReturnCommand.Execute(null);

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

        [TestCase("5+5", "0", ExpectedResult = "5+")]
        [TestCase("", "0", ExpectedResult = "")]
        [TestCase("\r\n", "0", ExpectedResult = "")]
        [TestCase("5+5", "2", ExpectedResult = "+5")]
        [TestCase("5+5", "3", ExpectedResult = "5+5")]
        public string TestBackCommand(string inputString, int offset)
        {
            TextBox iw = (TextBox)Application.Current.MainWindow.FindName("inputWindow");
            iw.AppendText(inputString);
            iw.Select(iw.Text.Length - (offset+1), 0);
            _viewModel.BackCommand.Execute(null);
            return _viewModel.GetPrompt(iw);
        }
    }
}