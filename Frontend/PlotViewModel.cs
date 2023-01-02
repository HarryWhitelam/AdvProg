using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Axes;

namespace Frontend
{
    public class PlotViewModel
    {
        public PlotViewModel()
        {
            /*this.Title = "Test Plot";
            this.Points = new List<DataPoint>
                                {
                                    new DataPoint(0,4),
                                    new DataPoint(10,13),
                                    new DataPoint(20,15),
                                    new DataPoint(30,16),
                                    new DataPoint(40,12),
                                    new DataPoint(50,12)
                                };*/

            //Testing out Cos functions
            double z = 5;
            double radiansX = (z * (Math.PI)) / 180;
            Func<double, double> f = (x) =>
            {
                double maths = Math.Cos(z);
                return maths;
            };

            int max = 200;
            int min = -200;
            int m = 4;
            int c = 4;
            //Testing out y=mx+c functions
            Func<double, double> g = (x) =>
            {
                double y = 0;
                y = (m * x) + c;
                return y;
            };

            this.MyModel = new PlotModel
            {
                Title = "Cos Graph"
            };
            this.MyModel.Series.Add(new FunctionSeries(g, -10, 10, 0.1, "y=4x+c"));
            this.MyModel.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Bottom,
                Minimum = 0,
                Maximum = 5
            });
            this.MyModel.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Left,
                Minimum = 0,
                Maximum = 5
            });
        }

        public string Title
        {
            get;
            private set;
        }

        public IList<DataPoint> Points
        {
            get;
            private set;
        }

        public PlotModel MyModel
        {
            get;
            private set;
        }
    }
}
