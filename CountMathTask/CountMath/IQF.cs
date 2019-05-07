using System;
using System.Linq;
using System.Threading;

namespace CountMath
{
    public class Iqf
    {
        private readonly Func<double, double> _mainFunction;

        private Func<double, double>[] defaultIntegralsFunction;

        public Iqf(Func<double, double> mainFunction)
        {
            defaultIntegralsFunction = new Func<double, double>[3];
            
            defaultIntegralsFunction[0] = x =>
                1.5 * Math.Pow(x - 1.5, 2.0 / 3);
            
            defaultIntegralsFunction[1] = x =>
                0.6 * Math.Pow(x - 1.5, 2.0 / 3)*(x + 2.25);

            defaultIntegralsFunction[2] = x =>
                0.375 * Math.Pow(x - 1.5, 2.0 / 3)*(x * x + 1.8 * x + 4.05);

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
            defaultIntegralsFunction[numberOfMoment](end) - defaultIntegralsFunction[numberOfMoment](start);
    }
}