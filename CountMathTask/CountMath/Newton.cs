using System;
using System.Linq;

namespace CountMath
{
    public class Newton
    {
        private readonly Func<double[], double>[] _funcSystem;

        private const double Eps = 0.0000000001;

        private readonly double[] _prevSolution = new double[10];

        private double[] _currentSolution = new double[10];

        public int CountOfIterationInLastSolution { get; private set; }

        public Newton(Func<double[], double>[] funcSystem)
        {
            _funcSystem = funcSystem;
            CountOfIterationInLastSolution = 0;
        }

        public double[] GetSolutionWithAccuracy(double[] startVector, double accuracy)
        {
            CountOfIterationInLastSolution = 0;
            for (var i = 0; i < startVector.Length; i++)
            {
                _currentSolution[i] = startVector[i];
            }

            do
            {
                Iterate();
                
                
            } while (CountCurrentAccuracy() > accuracy);

            return _currentSolution;
        }

        private void Iterate()
        {
            for (var i = 0; i < _currentSolution.Length; i++)
            {
                _prevSolution[i] = _currentSolution[i];
            }

            var jacobi = new double[_funcSystem.Length][];
            var templeB = new double[_funcSystem.Length];
            
            for (var i = 0; i < _funcSystem.Length; i++)
            {
                 jacobi[i] = new double[_funcSystem.Length];
                 for (var j = 0; j < _funcSystem.Length; j++)
                    jacobi[i][j] = GetPartialDerivativeInPoint(_funcSystem[i], _currentSolution, j);
                 
                 templeB[i] = -_funcSystem[i](_currentSolution);
            }
            
            var lupHelper = new Lup(jacobi);

            _currentSolution = lupHelper.LesSol(templeB);
         
            for (var i = 0; i < _funcSystem.Length; i++)
                _currentSolution[i] += _prevSolution[i];

            CountOfIterationInLastSolution++;
        }
        
        public double[] GetSolutionWithAccuracyModified(double[] startVector, double accuracy)
        {
            CountOfIterationInLastSolution = 0;

            for (var i = 0; i < startVector.Length; i++)
            {
                _currentSolution[i] = startVector[i];
            }
            
            var jacobi = new double[_funcSystem.Length][];
            
            for (var i = 0; i < _funcSystem.Length; i++)
            {
                jacobi[i] = new double[_funcSystem.Length];
                for (var j = 0; j < _funcSystem.Length; j++)
                    jacobi[i][j] = GetPartialDerivativeInPoint(_funcSystem[i], _currentSolution, j);
            }

            var lupHelper = new Lup(jacobi);

            do
            {
                IterateModified(lupHelper);
                
            } while (CountCurrentAccuracy() > accuracy);

            return _currentSolution;
        }

        private void IterateModified(Lup lupHelper)
        {
            for (var i = 0; i < _currentSolution.Length; i++)
            {
                _prevSolution[i] = _currentSolution[i];
            }
           
            var templeB = new double[_funcSystem.Length];

            for (var i = 0; i < _funcSystem.Length; i++)
            {
                templeB[i] = -_funcSystem[i](_prevSolution);
            }

            _currentSolution = lupHelper.LesSol(templeB);
         
            for (var i = 0; i < _funcSystem.Length; i++)
                _currentSolution[i] += _prevSolution[i];

            CountOfIterationInLastSolution++;
        }
        public double[] GetSolutionWithAccuracySuperModified(double[] startVector, double accuracy, int iterationTrigger)
        {
            
            CountOfIterationInLastSolution = 0;
            
            for (var i = 0; i < startVector.Length; i++)
            {
                _currentSolution[i] = startVector[i];
            }

            Iterate();
            
            while (CountCurrentAccuracy() > accuracy)
            {
                if (iterationTrigger > CountOfIterationInLastSolution)
                {
                    Iterate();                
                }
                else
                {
                    var jacobi = new double[_funcSystem.Length][];
            
                    for (var i = 0; i < _funcSystem.Length; i++)
                    {
                        jacobi[i] = new double[_funcSystem.Length];
                        for (var j = 0; j < _funcSystem.Length; j++)
                            jacobi[i][j] = GetPartialDerivativeInPoint(_funcSystem[i], _currentSolution, j);
                    }

                    var lupHelper = new Lup(jacobi);
                    
                    while (CountCurrentAccuracy() > accuracy)
                    {
                        IterateModified(lupHelper);
                        
                    }                    
                }
            }

            return _currentSolution;
        }
        private double CountCurrentAccuracy() =>
            _prevSolution.Select((t, i) => Math.Abs(_currentSolution[i] - t)).Concat(new[] {double.MinValue}).Max();
        
        private static double GetPartialDerivativeInPoint(Func<double[], double> func, double[] point, int numberOfPartial)
        {
            var deltaPoint = new double[10];
            for (var i = 0; i < point.Length; i++)
            {
                deltaPoint[i] = i != numberOfPartial ? point[i] : point[i] + Eps;
            }

            return (func(deltaPoint) - func(point)) / Eps;
        }
    }
}