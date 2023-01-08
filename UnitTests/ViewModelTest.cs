using Frontend;
using NUnit.Framework;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Documents;

[assembly: Apartment(ApartmentState.STA)]

namespace UnitTests
{
    public class Tests
    {
        private App _app;
        private MainWindow _mainWindow;
        private ViewModel _viewModel;

        [SetUp]
        public void Setup()
        {
            _mainWindow = new MainWindow(true);
            _viewModel = new();
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

        [TestCase("5+5", "10", ExpectedResult = "fail")]
        public string TestPrintResult(string result, string prompt)
        {
            TextBox iw = (TextBox)_mainWindow.FindName("inputWindow");
            iw.AppendText(prompt);
            _viewModel.PrintResult(result, prompt);
            return iw.Text;
        }
    }
}