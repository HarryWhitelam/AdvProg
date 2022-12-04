using System.Windows;

namespace AdvProg
{
    public partial class RootPopUp : Window
    {
        public RootPopUp()
        {
            InitializeComponent();
        }

        private void ShortCutSubmit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
