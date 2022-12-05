using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Frontend
{
    /// <summary>
    /// Interaction logic for GraphWindow.xaml
    /// </summary>
    public partial class GraphWindow : Window
    {
        public GraphWindow()
        {
            InitializeComponent();
            TestPlot();
        }

        private void TestPlot()
        {
            //Console.WriteLine("Enter a value for m: ");
            //int m = Convert.ToInt32(Console.ReadLine());
            //Console.WriteLine("Enter a value for c: ");
            //int c = Convert.ToInt32(Console.ReadLine());
            //Console.WriteLine("Your equation is y = " + m + "x + " + c);

            //double[] xCoords = new double[] { -10, -9, -8, -7, -6, -5, -4, -3, -2, -1, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            //double[] yCoords = new double[] { -46, -41, -36, -31, -26, -21, -16, -11, -6, -1, 4, 9, 14, 19, 24, 29, 34, 39, 44, 49, 54};

            int m = 5;
            int c = 4;

            //double[] xCoords = new double[20];
            var xCoords = Enumerable.Range(-100, 200).ToArray();
            //List<double> xCoords = new List<double>();
            //double[] yCoords = new double[20];
            List<double> yCoords = new List<double>();

            foreach (int x in Enumerable.Range(-100, 200))
            {
                double y = (m * x) + c;
                yCoords.Add(y);
            }

            var line1 = new InteractiveDataDisplay.WPF.LineGraph
            {
                Stroke = new SolidColorBrush(Colors.RoyalBlue),
                Description = "Line 1",
                StrokeThickness = 3
            };

            line1.Plot(xCoords, yCoords);
            myGrid.Children.Clear();
            myGrid.Children.Add(line1);

            myChart.Title = $"Line plot for y = 5x + 4 for a range of -100 -> 100";
            myChart.IsAutoFitEnabled = true;
            myChart.LegendVisibility = Visibility.Visible;
        }
    }
}
