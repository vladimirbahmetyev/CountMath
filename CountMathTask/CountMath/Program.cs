using System;
using System.Linq.Expressions;

namespace CountMath
{
    static class Program
    {
        public static void Main()
        {
            var funcSystem = new Func<double[], double>[10];
            funcSystem[0] = FuncFirst;
            funcSystem[1] = FuncSecond;
            funcSystem[2] = FuncThird;
            funcSystem[3] = FuncFourth;
            funcSystem[4] = FuncFifth;
            funcSystem[5] = FuncSixth;
            funcSystem[6] = FuncSeventh;
            funcSystem[7] = FuncEighth;
            funcSystem[8] = FuncNinth;
            funcSystem[9] = FuncTenth;

            var startVector = new[]{0.5, 0.5, 1.5, -1.0, -0.5, 1.5, 0.5, -0.5, 1.5, -1.5 };
            
            var newtonTest = new Newton(funcSystem);

            var testSol = newtonTest.GetSolutionWithAccuracy(startVector, 1e-6);            
        }

        //static double[][] MatrixMult(double[][] first, double[][] second)
        //{
        //    double[][] result = new double[first.Length][];
        //    for (int i = 0; i < first.Length; i++)
        //        result[i] = new double[first.Length];

        //    for (int i = 0; i < first.Length; i++)
        //        for (int j = 0; j < first.Length; j++)
        //        {
        //            double temple = 0;
        //            for (int k = 0; k < first.Length; k++)
        //            {
        //                temple += first[i][k] * second[k][j];
        //            }
        //            result[i][j] = temple;
        //        }
        //    return result;
        //}
        //static void printMatrix(double[][] matrix)
        //{
        //    foreach (var row in matrix)
        //    {
        //        foreach (var value in row)
        //            Console.Write(value.ToString("0.000") + " ");
        //        Console.WriteLine();
        //    }
        //}
        
        
        public static double FuncFirst(double[] xVector) =>        
            Math.Cos(xVector[0] * xVector[1]) - Math.Exp(-3 * xVector[2]) + xVector[3] * xVector[4] * xVector[4] -
                   xVector[5] - Math.Sinh(2 * xVector[7]) * xVector[8] + 2 * xVector[9]  + 2.0004339741653854440;

        public static double FuncSecond(double[] xVector) =>
            Math.Sin(xVector[0] * xVector[1]) + xVector[2] * xVector[6] * xVector[8] -
            Math.Exp(-xVector[9] + xVector[5])
            + 3 * xVector[4] * xVector[4] - xVector[5] * (xVector[7] + 1) + 10.886272036407019994;

        public static double FuncThird(double[] xVector) =>
            xVector[0] - xVector[1] + xVector[2] - xVector[3] + xVector[4] - xVector[5] + xVector[6] - xVector[7] +
            xVector[8] - xVector[9] - 3.1361904761904761904;

        public static double FuncFourth(double[] xVector) =>
            2 * Math.Cos(-xVector[8] + xVector[3]) + xVector[4] / (xVector[2] + xVector[0]) -
            Math.Sin(xVector[1] * xVector[1])
            + Math.Cos(xVector[6] * xVector[9]) * Math.Cos(xVector[6] * xVector[9]) - xVector[7] -  0.170747270502230475;

        public static double FuncFifth(double[] xVector) =>
            Math.Sin(xVector[4]) + 2 * xVector[7] * (xVector[2] + xVector[0]) -
            Math.Exp(-xVector[6] * (-xVector[9]) + xVector[5])
            + 2 * Math.Cos(xVector[1]) - 1 / (xVector[3] - xVector[8]) -  0.368589627310127786;

        public static double FuncSixth(double[] xVector) =>
            Math.Exp(xVector[0] - xVector[3] - xVector[9]) + xVector[4] * xVector[4] / xVector[7] +
            Math.Cos(3 * xVector[9] * xVector[1]) / 2 - xVector[5] * xVector[2] + 2.049108601677187511;

        public static double FuncSeventh(double[] xVector) =>
            xVector[1] * xVector[1] * xVector[1] * xVector[6] - Math.Sin(xVector[9] / xVector[4] + xVector[7]) +
            (xVector[0] - xVector[5]) *
            Math.Sin(xVector[3]) + xVector[2] -  0.738043007620279801;

        public static double FuncEighth(double[] xVector) =>
            xVector[4] * (xVector[0] - xVector[5] * 2) * (xVector[0] - xVector[5] * 2) -
            2 * Math.Sin(-xVector[8] + xVector[2]) +
            1.5 * xVector[3] - Math.Exp(xVector[1] * xVector[6] + xVector[9]) + 3.566832198969380904;

        public static double FuncNinth(double[] xVector) =>
            7 / xVector[5] + Math.Exp(xVector[4] + xVector[3]) - 2 * xVector[1] * xVector[7] * xVector[9] * xVector[6] +
            3 * xVector[8] - 3 * xVector[0] -  8.439473450838325749;

        public static double FuncTenth(double[] xVector) =>
            xVector[9] * xVector[0] + xVector[8] * xVector[1] - xVector[7] * xVector[2] +
            Math.Sin(xVector[3] + xVector[4] + xVector[5]) * xVector[6] - 0.7823809523809523809;

    }


    class MatrixGen
    {
        private readonly int _size;
        private Random _random;
        private double _q;
        public MatrixGen(int inputSize, double q)
        {
            _size = inputSize;
            _random = new Random(_size);
            _q = q;
        }

        public double[][] CreateMatrix()
        {
            var matrix = new double[_size][];
            for (var i = 0; i < _size; i++)
                matrix[i] = new double[_size];

            for (var i = 0; i < _size; i++)
                for (var j = 0; j < _size; j++)
                    matrix[i][j] = _random.Next(10);
            for (var i = 0; i < _size; i++)
            {
                matrix[i][i] = 0;
                for (var j = 0; j < _size; j++)
                {
                    if (i == j)
                        continue;
                    matrix[i][i] += matrix[i][j] * _q;
                }
            }

            return matrix;
        }

        public double[] CreateB()
        {
            var b = new double[_size];
            for (var i = 0; i < _size; i++)
                b[i] = _random.Next(10);
            return b;
        }
    }
    
}
