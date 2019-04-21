using System;

namespace CountMath
{
    class Program
    {
        static void Main()
        {
            const double q = 5.5;

            var myGen = new MatrixGen(5, q);

            var b = myGen.CreateB();

            var kek = myGen.CreateMatrix();

            var myYakobi = new Yakobi(kek,b);
            var myZel = new GaussZel(kek, b);
            int itCount;
            var nani = myYakobi.GetSolutionWithAccuracy(0.000001, out itCount);
            Console.WriteLine(itCount);
            var nani2 = myZel.GetSolutionWithAccuracy(0.0000001, out itCount);
            foreach (var c in nani2)
            {
                Console.Write(c.ToString("0.000000") + " ");
            }
            Console.WriteLine();
            foreach (var c in nani)
            {
                Console.Write(c.ToString("0.000000") + " ");
            }
            Console.WriteLine();
            Console.WriteLine(itCount);
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
