using System;
using System.Linq;

namespace CountMath
{
    class GaussZel
    {
        private readonly double[][] _matrix;
        private readonly double[] _b;
        private double[] _currentSol;
        private double _currentAccuracy;
        private readonly double[][] _c;
        private readonly double[] _d;

        public GaussZel(double[][] matrix, double[] b)
        {
            _currentAccuracy = double.MaxValue;
            _matrix = matrix;
            _b = b;
            _currentSol = new double[b.Length];
            for (var i = 0; i < b.Length; i++)
                _currentSol[i] = _b[i];

            _c = new double[matrix.Length][];
            for(var i = 0; i < matrix.Length;i++)
                _c[i] = new double[matrix.Length];

            _d = new double[matrix.Length];

            for(var i = 0; i < matrix.Length;i++)
                for(var j = 0; j < matrix.Length;j++)
                    _c[i][j] = i == j? 0 : -matrix[i][j] / matrix[i][i];

            for (var i = 0; i < matrix.Length; i++)
                _d[i] = b[i] / matrix[i][i];
        }

        private void NextSolution()
        {
            var newSol = new double[_b.Length];
            for (var i = 0; i < _b.Length; i++)
            {
                for(var j = 0; j < i; j++)
                    newSol[i] += _c[i][j] * newSol[j];

                for (var j = i; j < _b.Length; j++)
                    newSol[i] += _c[i][j] * _currentSol[j];

                newSol[i] += _d[i];
            }

            _currentAccuracy = GetCurrentAccuracy(_currentSol, newSol);

            _currentSol = newSol;
        }

        public double[] GetSolutionWithAccuracy(double accuracy, out int iterationCount)
        {
            iterationCount = 0;
            while (_currentAccuracy >= accuracy)
            {
                iterationCount++;
                NextSolution();
            }

            return _currentSol;
        }

        private double GetCurrentAccuracy(double[] currSol, double[] newSol)
        {
            var distance = currSol.Select((t, i) => (t - newSol[i]) * (t - newSol[i])).Sum();

            distance = Math.Sqrt(distance);

            var q = _matrix.Max(row => row.Sum(x => Math.Abs(x)));

            return Math.Abs(q / (1 - q) * distance);
        }
    }
}
