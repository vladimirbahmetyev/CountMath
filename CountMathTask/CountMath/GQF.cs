using System;
using System.Linq;

namespace CountMath
{
    public class Gqf
    {
        private Func<double, double>[] defaultIntegralsFunction;
        private Iqf _iqfHelper;

        private readonly Func<double, double> _mainFunction;
        private const int CountOfNodes = 3;
        
        public Gqf(Func<double, double> mainFunction, Iqf iqfHelper)
        {
            var d = 1.0 / 3;
            var a = 1.5;
            
            defaultIntegralsFunction = new Func<double, double>[6];
            
            defaultIntegralsFunction[0] = x =>
                Math.Pow(x - a, 1-d) / (1-d);

            defaultIntegralsFunction[1] = x =>
                Math.Pow(x - a, 1-d) / (d-2) * (a -d * x + x) / (d-1);
            defaultIntegralsFunction[2] = x =>
                -Math.Pow(x - a, 1 - d) * (2 * a*a - 3 * (d - 1) * x + (d*d - 3*d + 2) * x * x) / (d - 3) /
                (d - 2) / (d - 1);
            defaultIntegralsFunction[3] = x =>
                Math.Pow(x - a, 1 - d) * (6 * Math.Pow(a, 3) - 6 * Math.Pow(a, 2) * (d - 1) * x +
                                              3 * a * (d * d - 3 * d + 2) * Math.Pow(x, 2) -
                                              (d * d * d - 6 * d * d + 11*d - 6) * Math.Pow(x, 3))/(d-4)/(d-3)/(d-2)/(d-1);
            defaultIntegralsFunction[4] = x =>
                -(Math.Pow(x - a, 1 - d) * (24 * Math.Pow(a, 4) - 24 * Math.Pow(a, 3) * (d - 1) * x +
                                            12 * a * a * (d * d - 3*d + 2) * x * x -
                                            4 * a * (d * d * d - 6 * d * d + 11 * d - 6) * x * x * x +
                                            (d * d * d * d - 10 * d * d * d + 35 * d * d - 50 * d + 24) *
                                            Math.Pow(x, 4))) / (d - 5) / (d - 4) / (d - 3) / (d - 2) / (d - 1);
            defaultIntegralsFunction[5] = x =>
                Math.Pow(x - a, 1 - d) * (120 * Math.Pow(a, 5) - 120 * Math.Pow(a, 4) * (d - 1) * x +
                                                 60 * Math.Pow(a, 3) * (d * d - 3 * d + 2) * x * x -
                                                 20 * a * a * (d * d * d - 6 * d * d + 11 * d - 6) * x * x * x + 5 *
                                                 a *
                                                 (Math.Pow(d, 4) - 10 * d * d * d + 35 * d * d - 50 * d + 24) *
                                                 Math.Pow(x, 4) -
                                          (Math.Pow(d, 5) - 15 * Math.Pow(d, 4) + 85 * d * d * d - 225 * d * d +
                                           274 * d - 120) *
                                          Math.Pow(x, 5)) / (d - 6)/(d - 5) / (d - 4) / (d - 3) / (d - 2) / (d - 1);
                _mainFunction = mainFunction;
            _iqfHelper = iqfHelper;
        }
        
        public double CalcIntegral(double start, double end)
        {
            var gqfСoefficients = GetGqf(start, end, out var nodes);
            
            var integralValue = nodes.Select((t, i) => 
                gqfСoefficients[i] * _mainFunction(t)).Sum();

            return integralValue;
        }

        public double[] GetGqf(double start, double end, out double[] nodes)
        {
            var moments = new double[CountOfNodes * 2];
            var bMoments = new double[CountOfNodes];
            var matrixWithMoments = new double[CountOfNodes][];
            
            for (var i = 0; i < CountOfNodes * 2; i++)
                moments[i] = CalcWeightFunctionMomentAnalytics(start, end, i);

            for (var i = 0; i < CountOfNodes; i++)
            {
                matrixWithMoments[i] = new double[CountOfNodes];
                for (var j = 0; j < CountOfNodes; j++)
                {
                    matrixWithMoments[i][j] = moments[i + j];
                }

                bMoments[i] = -moments[i + CountOfNodes];
            }
            
            var lupHelper = new Lup(matrixWithMoments);

            var aCoeff = lupHelper.LesSol(bMoments);

            nodes = GetPolRoot(aCoeff);

            var result = _iqfHelper.GetIqf(start, end, nodes);

            return result;
        }
        
        private double[] GetPolRoot(double[] aCoeff)
        {
            var b = aCoeff[2];
            var c = aCoeff[1];
            var d = aCoeff[0];

            var q = (b*b - 3 * c) / 9;
            var r = (2*b*b*b - 9 * b*c + 27 * d) / 54;

            var s = Math.Pow(q, 3) - Math.Pow(r, 2);
            
            if(s <= 0)
                throw new ArgumentException("Комплексные корни полинома!");

            var phi = Math.Acos(r / Math.Pow(q, 3.0/2)) / 3;

            var nodes = new double[3];

            nodes[0] = -2 * Math.Sqrt(q) * Math.Cos(phi) - b / 3;
            nodes[1] = -2 * Math.Sqrt(q) * Math.Cos(phi + 2 * Math.PI / 3) - b / 3;
            nodes[2] = -2 * Math.Sqrt(q) * Math.Cos(phi - 2 * Math.PI/3) - b / 3;
            
            return nodes;
        }

        private double CalcWeightFunctionMomentAnalytics(double start, double end, int numberOfMoment) =>
            defaultIntegralsFunction[numberOfMoment](end) - defaultIntegralsFunction[numberOfMoment](start);
    }
}