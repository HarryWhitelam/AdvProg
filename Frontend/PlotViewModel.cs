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
using Backend;

namespace Frontend
{
    public class PlotViewModel
    {
        public PlotViewModel()
        {

            /* modelnum 1 = polynomial
             * modelnum 2 = cos(x)
             * modelnum 3 = sin(x)
             * modelnum 4 = tan(x) 
             * modelnum 5 = circle graphs */
            double min = -10;
            double max = 10;

            int modelnum = 1;

            if (modelnum == 1)
            {
                //string[] equation = { "5x^4", "4x^3", "3x^2", "2x", "1" };
                string[] equation = { "5x^3", "+", "4x^2", "-", "3x", "+", "2", "+", "6x^4"};
                //string[] equation = { "6x", "-" ,"3" };
                int totalLength = equation.Length;
                bool plus;
                bool neg;
                int numOfTerms = (totalLength + 1) / 2;
                int numOperations = numOfTerms - 1;
                int ops = 0;
                string[] eqOperands = new string[numOperations];
                string[] newEq = new string[numOfTerms];
                for ( int terms = 0; terms < numOfTerms; terms++)
                {
                    if (equation[terms].Contains("+"))
                    {
                        plus = true;
                        neg = false;
                        equation = equation.Where((source, index) => index != terms).ToArray();
                        System.Diagnostics.Debug.WriteLine(equation);
                        if (plus == true)
                        {
                            eqOperands[ops] = "+";
                            ops++;
                        }
                        else if (neg == true)
                        {
                            eqOperands[ops] = "-";
                            ops++;
                        }
                    }
                    else if (equation[terms].Contains("-"))
                    {
                        plus = false;
                        neg = true;
                        equation = equation.Where((source, index) => index != terms).ToArray();
                        System.Diagnostics.Debug.WriteLine(equation);
                        if (plus == true)
                        {
                            eqOperands[ops] = "+";
                            ops++;
                        }
                        else if (neg == true)
                        {
                            eqOperands[ops] = "-";
                            ops++;
                        }
                    }
                    
                }

                Array.Copy(equation, newEq, numOfTerms);
                
                Func<double, double> i = (x) =>
                {
                    string splitpattern = @"x\^|x";
                    double[] calcValues = new double[numOfTerms];
                    /*string[] operands = new string[numOperations];
                    //I think I can re-write this better
                    for (int k = 0; k < numOperations; k++)
                    {
                        
                    }*/
                    for (int j = 0; j < numOfTerms; j++)
                    {
                        if (newEq[j].Contains("^"))
                        {
                            //Have re-written this usign references to the backend but left in old code incase things break later 
                            string[] inputSplit = Regex.Split(newEq[j], splitpattern);
                            //double ax = Math.Pow(x, Double.Parse(inputSplit[1]));
                            //double a2 = (Double.Parse(inputSplit[0]) * ax);
                            double power = Double.Parse(inputSplit[1]);
                            string xpower = String.Format("({0})^({1})", Convert.ToDecimal(x), power);
                            double ax = Double.Parse(Interpreter.interpret(xpower));
                            decimal convAX = Convert.ToDecimal(ax);
                            string multiplyByPower = String.Format("({0})*({1})", inputSplit[1], convAX);
                            double a2 = Double.Parse(Interpreter.interpret(multiplyByPower));
                            calcValues[j] = a2;
                        }
                        else if (newEq[j].Contains("x"))
                        {
                            string[] inputSplit = Regex.Split(newEq[j], splitpattern);
                            string dx = String.Format("({0})*({1})", inputSplit[0], Convert.ToDecimal(x));
                            double d = Double.Parse(Interpreter.interpret(dx));
                            //double d = (Double.Parse(inputSplit[0]) * x);
                            calcValues[j] = d;

                        }
                        else
                        {
                            calcValues[j] = Double.Parse(newEq[j]);
                        }

                    }

                    double add(double a, double b)
                    {
                        return a + b;
                    }
                    double subtract(double a, double b)
                    {
                        return a - (b);
                    }


                    double y = 0;
                    y = calcValues[0];
                    System.Diagnostics.Debug.WriteLine(calcValues.ToString());
                    System.Diagnostics.Debug.WriteLine(eqOperands.ToString());
                    for(int m = 1; m < numOfTerms; m++)
                    {
                        if (eqOperands[m-1].Contains("+"))
                        {
                            y = add(y, calcValues[m]);
                        }
                        else if (eqOperands[m-1].Contains("-"))
                        {
                            y = subtract(y, calcValues[m]);
                        }
                    }
                    return y;
                };
                this.GraphModel = new PlotModel
                {
                    //Title = "y = 5x^4 + 4x^3 + 3x^2 + 2x + 1",
                    Title = "y = 5x^3 + 4x^2 - 3x + 2 + 6x^4",
                    //Title = "y = 6x - 3",
                    TitleColor = OxyColor.Parse("#036ffc")
                };
                this.GraphModel.Series.Add(new FunctionSeries(i, min, max, 0.1, "y = 5x^3 + 4x^2 - 3x + 2 + 6x^4"));

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
                    //TODO CHANGE TO USE BACKEND
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
                    //TODO CHANGE TO USE BACKEND
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
                    //TODO CHANGE TO USE BACKEND
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
            else if(modelnum == 5)
            {
                //THIS DOES NOT WORK, TODO IF TIME
                string[] equation = { "x^2", "+", "y^2", "=", "70" };
                //string[] equation = { "y^2", "=", "70", "-", "x^2" };
                string checkFormat = @"x\^.*\+.*y\^.*=.*\d";
                string equationStr = string.Join(" ", equation);
                string[] rearrangedEq = new string[equation.Length];
                if (Regex.IsMatch(equationStr, checkFormat))
                {
                    Array.Copy(equation, 2, rearrangedEq, 0, 3);
                    rearrangedEq[4] = equation[0];
                    if (equation[1].Contains("+"))
                    {
                        rearrangedEq[3] = "-";
                    }
                    else if (equation[1].Contains("-"))
                    {
                        rearrangedEq[3] = "-";
                    }
                }
                else
                {
                    rearrangedEq = (string[])equation.Clone();
                }
                System.Diagnostics.Debug.WriteLine(rearrangedEq);

                Func<double, double> c = (x) =>
                {
                    int newLength = rearrangedEq.Length - 2;
                    string[] AfterEqualsEq = new string[newLength];
                    Array.Copy(rearrangedEq, 2, AfterEqualsEq, 0, newLength);
                    int numOfTerms = (newLength + 1) / 2;
                    int numOperations = numOfTerms - 1;
                    double[] calcValues = new double[numOfTerms];
                    string splitpattern = @"x\^|x";
                    string[] eqOperands = new string[numOperations];
                    double powerValue = 0;
                    int op = 0;
                    int val = 0;
                    for (int j = 0; j < newLength; j++)
                    {
                        if (AfterEqualsEq[j].Contains("^"))
                        {
                            string[] inputSplit = Regex.Split(AfterEqualsEq[j], splitpattern);
                            powerValue = Double.Parse(inputSplit[1]);
                            string xpower = String.Format("({0})^({1})", Convert.ToDecimal(x), powerValue);
                            double ax = Double.Parse(Interpreter.interpret(xpower));
                            calcValues[val] = ax;
                            val++;
                        }
                        else if (AfterEqualsEq[j].Contains("+") | AfterEqualsEq[j].Contains("-"))
                        {
                            eqOperands[op] = AfterEqualsEq[j];
                            op++;
                        }
                        else
                        {
                            calcValues[val] = Double.Parse(AfterEqualsEq[j]);
                            val++;
                        }

                    }

                    System.Diagnostics.Debug.WriteLine(calcValues);

                    double add(double a, double b)
                    {
                        return a + b;
                    }
                    double subtract(double a, double b)
                    {
                        return a - (b);
                    }


                    double y2 = 0;
                    y2 = calcValues[0];
                    System.Diagnostics.Debug.WriteLine(calcValues.ToString());
                    System.Diagnostics.Debug.WriteLine(eqOperands.ToString());
                    for (int m = 1; m < numOfTerms; m++)
                    {
                        if (eqOperands[m - 1].Contains("+"))
                        {
                            y2 = add(y2, calcValues[m]);
                        }
                        else if (eqOperands[m - 1].Contains("-"))
                        {
                            y2 = subtract(y2, calcValues[m]);
                        }
                    }
                    string yPower = String.Format("nroot(({0}),({1}))", Convert.ToDecimal(y2), powerValue);
                    double y = Double.Parse(Interpreter.interpret(yPower));
                    return y;
                };
                this.GraphModel = new PlotModel
                {
                    Title = "y^2 + x^2 = 70",
                    TitleColor = OxyColor.Parse("#036ffc")
                };
                this.GraphModel.Series.Add(new FunctionSeries(c, min, max, 0.0001, "y^2 + x^2 = 70"));
                //this.GraphModel.Series.Add(new FunctionSeries(d, -20, 20, 0.1, "y^2 + x^2 = 70"));

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
