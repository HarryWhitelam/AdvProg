using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Axes;
using System.Text.RegularExpressions;

namespace Frontend
{
    public class PlotViewModel
    {
        public PlotViewModel()
        {

            /* modelnum 1 = polynomial
             * modelnum 2 = cos(x)
             * modelnum 3 = sin(x)
             * modelnum 4 = tan(x) */
            double min = -10;
            double max = 10;

            int modelnum = 4;

            if (modelnum == 1)
            {
                //string[] equation = { "5x^4", "4x^3", "3x^2", "2x", "1" };
                //string[] equation = { "5x^3", "4x^2", "3x", "2" };
                string[] equation = { "6x", "3" };
                int lengthEq = equation.Length;
                Func<double, double> i = (x) =>
                {
                    string splitpattern = @"x\^|x";
                    double[] calcValues = new double[lengthEq];
                    for (int j = 0; j < lengthEq; j++)
                    {
                        if (equation[j].Contains("^"))
                        {
                            string[] inputSplit = Regex.Split(equation[j], splitpattern);
                            double ax = Math.Pow(x, Double.Parse(inputSplit[1]));
                            double a2 = (Double.Parse(inputSplit[0]) * ax);
                            calcValues[j] = a2;
                        }
                        else if (equation[j].Contains("x"))
                        {
                            string[] inputSplit = Regex.Split(equation[j], splitpattern);
                            double d = (Double.Parse(inputSplit[0]) * x);
                            calcValues[j] = d;

                        }
                        else
                        {
                            calcValues[j] = Double.Parse(equation[j]);
                        }

                    }
                    double y = 0;
                    int numTerms = calcValues.Length;
                    for (int k = 0; k < numTerms; k++)
                    {
                        y += calcValues[k];
                    }
                    return y;
                };
                this.GraphModel = new PlotModel
                {
                    //Title = "y = 5x^4 + 4x^3 + 3x^2 + 2x + 1",
                    Title = "y = 6x + 3",
                    TitleColor = OxyColor.Parse("#036ffc")
                };
                this.GraphModel.Series.Add(new FunctionSeries(i, min, max, 0.1, "y = 6x + 3"));

                this.GraphModel.Axes.Add(new LinearAxis
                {
                    Position = AxisPosition.Bottom,
                    Minimum = -5,
                    Maximum = 5,
                    MajorGridlineStyle = LineStyle.Dot
                });
                this.GraphModel.Axes.Add(new LinearAxis
                {
                    Position = AxisPosition.Left,
                    Minimum = -5,
                    Maximum = 5,
                    MajorGridlineStyle = LineStyle.Dot
                });
            } else if (modelnum == 2)
            {
                Func<double, double> f = (x) =>
                {
                    double maths = Math.Cos(x);
                    return maths;
                };
                this.GraphModel = new PlotModel
                {
                    Title = "Cos(x) Graph"
                };
                this.GraphModel.Series.Add(new FunctionSeries(f, min, max, 0.1, "cos(x)"));

                this.GraphModel.Axes.Add(new LinearAxis
                {
                    Position = AxisPosition.Bottom,
                    Minimum = -5,
                    Maximum = 5,
                    MajorGridlineStyle = LineStyle.Dot
                });
                this.GraphModel.Axes.Add(new LinearAxis
                {
                    Position = AxisPosition.Left,
                    Minimum = -5,
                    Maximum = 5,
                    MajorGridlineStyle = LineStyle.Dot
                });
            } else if (modelnum == 3)
            {
                Func<double, double> g = (x) =>
                {
                    double maths = Math.Sin(x);
                    return maths;
                };
                this.GraphModel = new PlotModel
                {
                    Title = "Sin(x) Graph"
                };
                this.GraphModel.Series.Add(new FunctionSeries(g, min, max, 0.1, "Sin(x)"));

                this.GraphModel.Axes.Add(new LinearAxis
                {
                    Position = AxisPosition.Bottom,
                    Minimum = -5,
                    Maximum = 5,
                    MajorGridlineStyle = LineStyle.Dot
                });
                this.GraphModel.Axes.Add(new LinearAxis
                {
                    Position = AxisPosition.Left,
                    Minimum = -5,
                    Maximum = 5,
                    MajorGridlineStyle = LineStyle.Dot
                });
            } else if(modelnum == 4)
            {
                Func<double, double> h = (x) =>
                {
                    double maths = Math.Tan(x);
                    return maths;
                };
                this.GraphModel = new PlotModel
                {
                    Title = "Tan(x) Graph"
                };
                this.GraphModel.Series.Add(new FunctionSeries(h, min, max, 0.1, "Tan(x)"));

                this.GraphModel.Axes.Add(new LinearAxis
                {
                    Position = AxisPosition.Bottom,
                    Minimum = -5,
                    Maximum = 5,
                    MajorGridlineStyle = LineStyle.Dot
                });
                this.GraphModel.Axes.Add(new LinearAxis
                {
                    Position = AxisPosition.Left,
                    Minimum = -5,
                    Maximum = 5,
                    MajorGridlineStyle = LineStyle.Dot
                });
            }
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

        public PlotModel GraphModel
        {
            get;
            private set;
        }

    }
}
