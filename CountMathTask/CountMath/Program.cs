using System;

namespace CountMath
{
    public static class Program
    {
        public static void Main()
        {
            var baseIqf = new Iqf(MainFunc);

            var baseGqf = new Gqf(MainFunc, baseIqf);
//            var testVal = testGqf.CalcIntegral(1.5, 3.3);
//            Console.WriteLine(testVal);
            
            
            var testCqf = new Cqf(baseIqf, baseGqf);
            for (var i = 0; i < 3; i++)
            {
                var kek = testCqf.CalcIntegralGqf(1.5, 3.3, i + 1);
                Console.WriteLine(kek);
            }

//            var optimalStepsCount = testCqf.CalcOptStep(1.5, 2.4, 1e-6);
//            var testRich = testCqf.CalcIntegralWithAccuracy(1.5, 3.3, 1e-6, optimalStepsCount);
//            
//            Console.WriteLine(testRich);
        }

        private static double MainFunc(double x) =>
            2 * Math.Cos(2.5 * x) * Math.Exp(x / 3) + 4 * Math.Sin(3.5 * x) * Math.Exp(-3 * x) + x;
    }
//
//
//    public class MatrixGen
//    {
//        private readonly int _size;
//        private readonly Random _random;
//        private readonly double _q;
//        public MatrixGen(int inputSize, double q)
//        {
//            _size = inputSize;
//            _random = new Random(_size);
//            _q = q;
//        }
//
//        public double[][] CreateMatrix()
//        {
//            var matrix = new double[_size][];
//            for (var i = 0; i < _size; i++)
//                matrix[i] = new double[_size];
//
//            for (var i = 0; i < _size; i++)
//                for (var j = 0; j < _size; j++)
//                    matrix[i][j] = _random.Next(10);
//            for (var i = 0; i < _size; i++)
//            {
//                matrix[i][i] = 0;
//                for (var j = 0; j < _size; j++)
//                {
//                    if (i == j)
//                        continue;
//                    matrix[i][i] += matrix[i][j] * _q;
//                }
//            }
//
//            return matrix;
//        }
//
//        public double[] CreateB()
//        {
//            var b = new double[_size];
//            for (var i = 0; i < _size; i++)
//                b[i] = _random.Next(10);
//            return b;
//        }
//    }
//        private static double FuncFirst(double[] x) =>
//                    -(Math.Cos(x[0] * x[1]) - Math.Exp(-3 * x[2]) + x[3] * Math.Pow(x[4], 2) - x[5] - Math.Sinh(2 * x[7]) * x[8] + 2 * x[9] + 2.0004339741653854440);
//
//        private static double FuncSecond(double[] x) =>
//                    -(Math.Sin(x[0] * x[1]) + x[2] * x[8] * x[6] - Math.Exp(-x[9] + x[5]) + 3 * Math.Pow(x[4], 2) - x[5] * (x[7] + 1) + 10.886272036407019994);
//
//        private static double FuncThird(double[] x) =>
//                    -(x[0] - x[1] + x[2] - x[3] + x[4] - x[5] + x[6] - x[7] + x[8] - x[9] - 3.1361904761904761904);
//
//        private static double FuncFourth(double[] x) =>
//                    -(2 * Math.Cos(-x[8] + x[3]) + x[4] / (x[2] + x[0]) - Math.Sin(x[1] * x[1]) + Math.Pow(Math.Cos(x[6] * x[9]), 2) - x[7] - 0.1707472705022304757);
//
//        private static double FuncFifth(double[] x) =>
//                    -(Math.Sin(x[4]) + 2 * x[7] * (x[2] + x[0]) - Math.Exp(-x[6] * (-x[9] + x[5])) + 2 * Math.Cos(x[1]) - 1 / (x[3] - x[8]) - 0.368589627310127786);
//
//        private static double FuncSixth(double[] x) =>
//                    -(Math.Exp(x[0] - x[3] - x[8]) + x[4] * x[4] / x[7] + 0.5 * Math.Cos(3 * x[9] * x[1]) - x[5] * x[2] + 2.049108601677187511);
//
//        private static double FuncSeventh(double[] x) =>
//                    -x[1] * x[1] * x[1] * x[6] - (-Math.Sin(x[9] / x[4] + x[7]) + (x[0] - x[5]) * Math.Cos(x[3]) + x[2] - 0.738043007620279801);
//
//        private static double FuncEighth(double[] x) =>
//                    -(x[4] * Math.Pow(x[0] - 2 * x[5], 2) - 2 * Math.Sin(-x[8] + x[2]) + 1.5 * x[3] - Math.Exp(x[1] * x[6] + x[9]) + 3.566832198969380904);
//
//        private static double FuncNinth(double[] x) =>
//                    -(7 / x[5] + Math.Exp(x[4] + x[3]) - 2 * x[1] * x[7] * x[9] * x[6] + 3 * x[8] - 3 * x[0] - 8.439473450838325749);
//
//        private static double FuncTenth(double[] x) =>
//                    -(x[9] * x[0] + x[8] * x[1] - x[7] * x[2] + Math.Sin(x[3] + x[4] + x[5]) * x[6] - 0.7823809523809523809);
//            var funcSystem = new Func<double[], double>[10];
//            funcSystem[0] = FuncFirst;
//            funcSystem[1] = FuncSecond;
//            funcSystem[2] = FuncThird;
//            funcSystem[3] = FuncFourth;
//            funcSystem[4] = FuncFifth;
//            funcSystem[5] = FuncSixth;
//            funcSystem[6] = FuncSeventh;
//            funcSystem[7] = FuncEighth;
//            funcSystem[8] = FuncNinth;
//            funcSystem[9] = FuncTenth;
//
//            var startVector = new[]{0.5, 0.5, 1.5, -1.0, -0.5, 1.5, 0.5, -0.5, 1.5, -1.5 };
//            
//            var newtonTest = new Newton(funcSystem);
//
//            var timer = new Stopwatch();
//            timer.Start();
//            var testSol = newtonTest.GetSolutionWithAccuracyHybrid(startVector, 1e-6, 2);
//            timer.Stop();
//
//            for (var i = 0; i < 10; i++)
//            {
//                Console.WriteLine(funcSystem[i](testSol));
//            }
//            
//            Console.WriteLine(newtonTest.CountOfIterationInLastSolution);
//            Console.WriteLine(timer.ElapsedMilliseconds);
}
