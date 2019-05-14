using System;
using System.Linq;
using CountMath.LES;

namespace CountMath.Integrals
{
    public class Iqf
    {
        private readonly Func<double, double> _mainFunction;

        private readonly Func<double, double>[] _defaultIntegralsFunction;

        public Iqf(Func<double, double> mainFunction)
        {
            var a = 1.5;
            var d = 1.0 / 3;
            
            _defaultIntegralsFunction = new Func<double, double>[3];
            
            _defaultIntegralsFunction[0] = x =>
                Math.Pow(x - a, 1-d) / (1-d);

            _defaultIntegralsFunction[1] = x =>
                Math.Pow(x - a, 1-d) / (d-2) * (a -d * x + x) / (d-1);
            _defaultIntegralsFunction[2] = x =>
                -Math.Pow(x - a, 1 - d) * (2 * a*a - 3 * (d - 1) * x + (d*d - 3*d + 2) * x * x) / (d - 3) /
                (d - 2) / (d - 1);

            _mainFunction = mainFunction;
        }

        public double CalcIntegral(double start, double end, double[] nodes)
        {
            var iqfСoefficients = GetIqf(start, end, nodes);
            
            var integralValue = nodes.Select((t, i) => 
                iqfСoefficients[i] * _mainFunction(t)).Sum();

            return integralValue;
        }

        public double[] GetIqf(double start, double end, double[] nodes)
        {
            var moments = new double[nodes.Length];
            
            for (var i = 0; i < nodes.Length; i++)
                moments[i] = CalcWeightFunctionMomentAnalytics(start, end,  i);
            
            var aMatrix = new double[nodes.Length][];
            
            for (var i = 0; i < nodes.Length; i++)
            {
                aMatrix[i] = new double[nodes.Length];
                for (var j = 0; j < nodes.Length; j++)
                {
                    aMatrix[i][j] = Math.Pow(nodes[j], i);
                }
            }
            
            var lupHelper = new Lup(aMatrix);

            var result = lupHelper.LesSol(moments);

            return result;
        }

        private double CalcWeightFunctionMomentAnalytics(double start, double end, int numberOfMoment) =>
            _defaultIntegralsFunction[numberOfMoment](end) - _defaultIntegralsFunction[numberOfMoment](start);
    }
}