using System;
using System.Linq;

namespace CountMath
{
    public class Gqf
    {
        private readonly Func<double, double> _mainFunction;
        private readonly Func<double, double> _weightFunction;
        private readonly double _paramA;
        private readonly double _paramB;
        private const int CountOfNodes = 3;

        private const int DefaultStep = 100000;

        public Gqf(Func<double, double> mainFunction, Func<double, double> weightFunction, double paramA, double paramB)
        {
            _mainFunction = mainFunction;
            _weightFunction = weightFunction;
            _paramA = paramA;
            _paramB = paramB;
        }

        public double[] GetGqf(double start, double end)
        {
            var moments = new double[CountOfNodes * 2];
            var bMoments = new double[CountOfNodes];
            var matrixWithMoments = new double[CountOfNodes][];
            
            for (var i = 0; i < CountOfNodes * 2; i++)
                moments[i] = CalcWeightFunctionMoment(start, end, DefaultStep, i);

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

            GetPolRoot(aCoeff);
            
            return null;
        }
        
        //Что делать с комплексными корнями?
        private double[] GetPolRoot(double[] aCoeff)
        {
            var p = (aCoeff[1] * 3 - aCoeff[2] * aCoeff[2]) / 3;
            var q = (2 * aCoeff[2] * aCoeff[2] - 9 * aCoeff[1] * aCoeff[2] + 27 * aCoeff[0]) / 27;

            var checkValue = Math.Pow(p / 3, 3) + Math.Pow(q / 2, 2);
        
            return null;
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