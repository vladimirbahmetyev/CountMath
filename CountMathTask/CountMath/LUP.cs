using System;

namespace CountMath
{
    class Lup
    {
        private readonly double[][] _l;
        private readonly double[][] _u;
        private readonly double[][] _p;
        private readonly double[][] _matrix;

        public Lup(double[][] matrix)
        {
            _matrix = new double[matrix.Length][];

            for (var i = 0; i < matrix.Length; i++)
            {
                _matrix[i] = new double[matrix.Length];
                for (var j = 0; j < matrix.Length; j++)
                    _matrix[i][j] = matrix[i][j];
            }

            _p = new double[matrix.Length][];

            for (var i = 0; i < matrix.Length; i++)
            {
                _p[i] = new double[matrix.Length];
                _p[i][i] = 1;
            }

            _l = new double[matrix.Length][];
            for (var i = 0; i < matrix.Length; i++)
            {
                _l[i] = new double[matrix.Length];
                _l[i][i] = 1;
            }

            _u = new double[matrix.Length][];
            for (var i = 0; i < matrix.Length; i++)
                _u[i] = new double[matrix.Length];

            for (var i = 0; i < _matrix.Length; i++)
            {
                var maxValNumber = FindMaxAbsValInColoumn(i);
                SwapRows(_matrix, i, maxValNumber);
                SwapRows(_p, i, maxValNumber);

                for (var j = i + 1; j < _matrix.Length; j++)
                    _matrix[j][i] /= _matrix[i][i];

                for (var j = i + 1; j < _matrix.Length; j++)
                    for (var k = i + 1; k < _matrix.Length; k++)
                        _matrix[j][k] -= (_matrix[j][i] * _matrix[i][k]);
            }

            for (var i = 0; i < _matrix.Length; i++)
                for (var j = 0; j < _matrix.Length; j++)
                {
                    if (j >= i)
                        _u[i][j] = _matrix[i][j];
                    else
                        _l[i][j] = _matrix[i][j];
                }
        }

        public void PrintL()
        {
            foreach (var row in _l)
            {
                foreach (var value in row)
                    Console.Write(value + " ");
                Console.WriteLine();
            }
        }

        public void PrintU()
        {
            foreach (var row in _u)
            {
                foreach (var value in row)
                    Console.Write(value + " ");
                Console.WriteLine();
            }
        }

        public void PrintP()
        {
            foreach (var row in _p)
            {
                foreach (var value in row)
                    Console.Write(value + " ");
                Console.WriteLine();
            }
        }

        public double DetA()
        {
            double det = 1;
            for (var i = 0; i < _u.Length; i++)
                det *= _u[i][i];
            return det;
        }

        public double[] LesSol(double[] b)
        {
            var newB = new double[b.Length];
            for (var i = 0; i < b.Length; i++)
            {
                for (var j = 0; j < b.Length; j++)
                {
                    newB[i] += b[j] * _p[i][j];
                }
            }

            b = newB;

            var templeSol = new double[_matrix.Length];

            templeSol[0] = b[0] / _l[0][0];

            for (var i = 1; i < _matrix.Length; i++)
            {
                var currentB = b[i];

                for (var j = 0; j < i; j++)
                    currentB -= _l[i][j] * templeSol[j];
                templeSol[i] = currentB / _l[i][i];
            }

            var finalSol = new double[_matrix.Length];

            finalSol[templeSol.Length - 1] = templeSol[templeSol.Length - 1] / _u[templeSol.Length - 1][templeSol.Length - 1];

            for (var i = templeSol.Length - 2; i > -1; i--)
            {
                var currentB = templeSol[i];

                for (var j = templeSol.Length - 1; j > i; j--)
                    currentB -= _u[i][j] * finalSol[j];
                finalSol[i] = currentB / _u[i][i];
            }

            return finalSol;
        }

        public double[][] GetReverse()
        {
            var reverse = new double[_matrix.Length][];

            for (var i = 0; i < _matrix.Length; i++)
            {
                var row = new double[_matrix.Length];
                row[i] = 1.0;
                reverse[i] = LesSol(row);
            }

            for (var i = 0; i < _matrix.Length; i++)
                for (var j = i; j < _matrix.Length; j++)
                {
                    var templeValue = reverse[i][j];
                    reverse[i][j] = reverse[j][i];
                    reverse[j][i] = templeValue;
                }

            return reverse;
        }

        private int FindMaxAbsValInColoumn(int numberOfColoumn)
        {
            double max = 0;
            var numberOfMax = -1;
            for (var i = numberOfColoumn; i < _matrix.Length; i++)
            {
                if (Math.Abs(_matrix[i][numberOfColoumn]) > max)
                {
                    numberOfMax = i;
                    max = Math.Abs(_matrix[i][numberOfColoumn]);
                }
            }
            if (numberOfMax == -1)
                throw new InvalidOperationException("Матрица имеет нулевой определитель");
            return numberOfMax;
        }

        private void SwapRows(double[][] matr, int numberOfFirst, int numberOfSecond)
        {
            var temple = matr[numberOfFirst];
            matr[numberOfFirst] = matr[numberOfSecond];
            matr[numberOfSecond] = temple;
        }
    }
}
