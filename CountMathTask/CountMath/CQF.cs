using System;
using System.ComponentModel;

namespace CountMath
{
    public class Cqf
    {
        private readonly Func<double, double> _mainFunction;
        private readonly Func<double, double> _weightFunction;

        private readonly Iqf _iqfHelper;

        public Cqf(Func<double, double> mainFunction, Func<double, double> weightFunction, double paramA, double paramB)
        {
            _mainFunction = mainFunction;
            _weightFunction = weightFunction;
            _iqfHelper = new Iqf(mainFunction, weightFunction, paramA, paramB);
        }

        public double CalcIntegral(double start, double end, int parts)
        {
            var result = 0.0;
            var partStep = (end - start) / parts;
            for (var i = 0; i < parts; i++)
            {
                var nodes = new double[3];
                
                nodes[0] = start + partStep * i;
                nodes[1] = start + partStep * (i + 1.0 / 2);
                nodes[2] = start + partStep * (i + 1);

                result += _iqfHelper.CalcIntegral(nodes[0], nodes[2], nodes); 
            }
            return result;
        }
    }
}