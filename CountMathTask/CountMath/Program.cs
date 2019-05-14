using System;
using CountMath.Integrals;

namespace CountMath
{
    public static class Program
    {
        public static void Main()
        {
            var testCqf = new Cqf(MainFunc);
            var testNewton = testCqf.CalcIntegralWithAccuracy(1.5, 3.3, 1e-6, TypeOfCqf.Iqf);
            var testGauss= testCqf.CalcIntegralWithAccuracy(1.5, 3.3, 1e-6,TypeOfCqf.Gqf);
            
            Console.WriteLine(testNewton);
            Console.WriteLine(testGauss);
        }

        private static double MainFunc(double x) =>
            2 * Math.Cos(2.5 * x) * Math.Exp(x / 3) + 4 * Math.Sin(3.5 * x) * Math.Exp(-3 * x) + x;
    }
}
