using System;
using System.Linq;

namespace CountMath.LES
{
    internal class Yakobi
    {
        private readonly double[][] _matrix;
        private readonly double[] _b;
        private double[] _currentSol;
        private double _currentAccuracy;

        public Yakobi(double[][] matrix, double[] b)
        {
            _currentAccuracy = double.MaxValue;
            _matrix = matrix;
            _b = b;
            _currentSol = new double[b.Length];
            for (var i = 0; i < b.Length; i++)
                _currentSol[i] = _b[i];
        }

        private void NextSolution()
        {
            var newSol = new double[_b.Length];

            for(var i = 0; i < _b.Length; i++)
            {
                newSol[i] = _b[i];
                for(var j = 0; j< _b.Length;j++)
                {
                    if (i == j)
                        continue;
                    newSol[i] -= _matrix[i][j] * _currentSol[j];
                }
                newSol[i] /= _matrix[i][i];
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
