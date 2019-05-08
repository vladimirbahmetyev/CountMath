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
            defaultIntegralsFunction = new Func<double, double>[6];
            
            defaultIntegralsFunction[0] = x =>
                1.5 * Math.Pow(x - 1.5, 2.0 / 3);
            
            defaultIntegralsFunction[1] = x =>
                0.6 * Math.Pow(x - 1.5, 2.0 / 3)*(x + 2.25);

            defaultIntegralsFunction[2] = x =>
                0.375 * Math.Pow(x - 1.5, 2.0 / 3)*(x * x + 1.8 * x + 4.05);
            
            defaultIntegralsFunction[3] = x =>
                0.2727 * Math.Pow(x - 1.5, 2.0 / 3)*(x*x*x + 1.6875 * x * x + 3.0375 * x + 6.834438);

            defaultIntegralsFunction[4] = x =>
                0.214286 * Math.Pow(x - 1.5, 2.0 / 3) *
                (x * x * x * x + 1.63633 * x * x * x + 2.76136 * x * x + 4.97045 * x + 11.1835);

            defaultIntegralsFunction[5] = x =>
                0.176471 * Math.Pow(x - 1.5, 2.0 / 3) * 
                (x * x * x * x * x + 1.60714 * x * x * x * x +2.62987 * x * x * x + 4.43791 * x * x + 7.98823 * x + 17.9735);
            
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