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
           
            double z = 5;
            double radiansX = (z * (Math.PI)) / 180;
            Func<double, double> f = (x) =>
            {
                double maths = Math.Cos(z);
                return maths;
            };

            /*
            this.MyModel = new PlotModel
            {
                Title = "Cos Graph"
            };
            this.MyModel.Series.Add(new FunctionSeries(f, -200, 200, 0.1, "cos(x)"));
           */

            int max = 10;
            int min = -10;
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
                Title = "y=4x+4"
            };
            this.MyModel.Series.Add(new FunctionSeries(g, min, max, 0.1, "y=4x+c"));
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
