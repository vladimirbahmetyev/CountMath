using System;
using System.Linq;

namespace CountMath
{
    public class Iqf
    {
        private readonly Func<double, double> _mainFunction;
        private readonly Func<double, double> _weightFunction;
        private readonly double _paramA;
        private readonly double _paramB;

        private const int DefaultStep = 100000;

        public Iqf(Func<double, double> mainFunction, Func<double, double> weightFunction, double paramA, double paramB)
        {
            _mainFunction = mainFunction;
            _weightFunction = weightFunction;
            _paramA = paramA;
            _paramB = paramB;
        }

        public double CalcIntegral(double start, double end, double[] nodes)
        {
            var a = GetIqf(start, end, nodes);
            
            var result = nodes.Select((t, i) => a[i] * _mainFunction(t)).Sum();

            return result;
        }


        public double[] GetIqf(double start, double end, double[] nodes)
        {
            var moments = new double[nodes.Length];
            
            for (var i = 0; i < nodes.Length; i++)
                moments[i] = CalcWeightFunctionMoment(start, end, DefaultStep, i);
            
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
        
        
        private double CalcWeightFunctionMoment(double start, double end, int step, int numberOfMoment) => 
            CalcIntegral((x) =>(_weightFunction(x) * Math.Pow(x, numberOfMoment)), start, end, step);

        private static double CalcIntegral(Func<double, double> function, double start, double end, int step)
        {
            var h = (end - start) / step;
            var result = 0.0;
            for (var i = 1; i <= step; i++)
            {
                result += function(start + (i - 1.0/2) * h);
            }
            return result * h;
        }
    }
}