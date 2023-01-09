using Frontend;
using NUnit.Framework;
using System.Linq;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;

namespace UnitTests
{
    class MainWindowTest
    {
        [SetUp]
        public void Setup()
        {
            if (Application.Current == null)
            {
                //TestContext.Out.WriteLine("Restarting app");
                new Application { ShutdownMode = ShutdownMode.OnExplicitShutdown };
            }
            new MainWindow(true);
        }

        [TearDown]
        public void TearDown()
        {
            Application.Current.Shutdown();
        }

        //[TestCase(ExpectedResult = true)]
        //public bool TestSettingsButton_Click()
        //{
        //    Button sb = (Button)Application.Current.MainWindow.FindName("settingsButton");
        //    sb.Command.Execute(null);
        //    //ButtonAutomationPeer peer = new ButtonAutomationPeer(sb);
        //    //IInvokeProvider provider = peer.GetPattern(PatternInterface.Invoke) as IInvokeProvider;
        //    //provider.Invoke();

        //    foreach (Window window in Application.Current.Windows.OfType<Window>())
        //    {
        //        TestContext.Out.WriteLine(window);
        //    }
        //    //if (Application.Current.Windows.OfType<Window>().Any(w => w.Name.Equals("Settings")))
        //    //{
        //    //    return true;
        //    //}
        //    return true;
        //}
    }
}
