using Frontend;
using NUnit.Framework;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

[assembly: Apartment(ApartmentState.STA)]

namespace UnitTests
{
    public class Tests
    {
        private MainWindow _mainWindow;
        private ViewModel _viewModel;

        [SetUp]
        public void Setup()
        {
            _viewModel = new();
            if (Application.Current == null)
                new Application { ShutdownMode = ShutdownMode.OnExplicitShutdown };
            if (Application.Current.MainWindow == null)
                new MainWindow(true);
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
    }
}