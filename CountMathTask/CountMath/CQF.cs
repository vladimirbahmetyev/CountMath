using System;
using System.Linq;
using static System.Double;

namespace CountMath
{
    public class Cqf
    {
        private readonly Iqf _iqfHelper;
        private readonly Gqf _gqfHelper;

        public Cqf(Iqf baseIqf, Gqf gqfHelper)
        {
            _iqfHelper = baseIqf;
            _gqfHelper = gqfHelper;
        }

        public double CalcIntegralIqf(double start, double end, int parts)
        {
            var integralValue = 0.0;
            
            var sizeOfStep = (end - start) / parts;
            
            for (var i = 0; i < parts; i++)
            {
                var nodes = new double[3];
                
                nodes[0] = start + sizeOfStep * i;
                nodes[1] = start + sizeOfStep * (i + 1.0 / 2);
                nodes[2] = start + sizeOfStep * (i + 1);

                integralValue += _iqfHelper.CalcIntegral(nodes[0], nodes[2], nodes); 
            }
            return integralValue;
        }
        
        public double CalcIntegralGqf(double start, double end, int parts)
        {
            var integralValue = 0.0;
            
            var sizeOfStep = (end - start) / parts;
            
            for (var i = 0; i < parts; i++)
            {
                var nodes = new double[2];
                
                nodes[0] = start + sizeOfStep * i;
                nodes[1] = start + sizeOfStep * (i + 1);

                integralValue += _gqfHelper.CalcIntegral(nodes[0], nodes[1]); 
            }
            return integralValue;
        }

        public double CalcIntegralIqfWithAccuracy(double start, double end, double accuracy)
        {
            var steps = CalcOptStepIqf(start, end, accuracy);
            var arrayWithSteps = new[] {steps};
            var integralValues = new[] {CalcIntegralIqf(start, end, steps)};
          
            var currentAccuracy = MaxValue;
            
            while (currentAccuracy > accuracy)
            {
                steps *= 2;

                var newIntegralValue = CalcIntegralIqf(start, end, steps);

                integralValues = AddValueToArray(integralValues, newIntegralValue);

                arrayWithSteps = AddValueToArray(arrayWithSteps, steps);
                
                currentAccuracy = CalcAccuracyRichardson(integralValues,arrayWithSteps, start, end);
            }
            
            return integralValues.Last();
        }
        public double CalcIntegralGqfWithAccuracy(double start, double end, double accuracy)
        {
            var steps = CalcOptStepGqf(start,end,accuracy);
            var arrayWithSteps = new[] {steps};
            var integralValues = new[] {CalcIntegralGqf(start, end, steps)};

            var currentAccuracy = MaxValue;
            
            while (currentAccuracy > accuracy)
            {
                steps *= 2;

                var newIntegralValue = CalcIntegralGqf(start, end, steps);
                
                integralValues = AddValueToArray(integralValues, newIntegralValue);

                arrayWithSteps = AddValueToArray(arrayWithSteps, steps);
                
                currentAccuracy = CalcAccuracyRichardson(integralValues, arrayWithSteps, start, end);
            }
            
            return integralValues.Last();
        }
        
        private double CalcAccuracyRichardson(double[] integralsValues, int[] steps, double start, double end)
        {
            
            var aitkinConstant = CalcAitkinConstant(integralsValues, steps);
            var lesRichardson = new double[integralsValues.Length][];
            
            for (var i = 0; i < integralsValues.Length; i++)
            {
                lesRichardson[i] = new double[integralsValues.Length];
                
                for (var j = 0; j < integralsValues.Length - 1; j++)
                {
                    lesRichardson[i][j] = Math.Pow((end - start) / steps[i], aitkinConstant + j);
                }

                lesRichardson[i][integralsValues.Length - 1] = -1;
            }

            var negativeIntegralValues = new double[integralsValues.Length];
            
            for (var i = 0; i < integralsValues.Length; i++)
                negativeIntegralValues[i] = -integralsValues[i];
            
            
            var lupHelper = new Lup(lesRichardson);

            var expectedError = lupHelper.LesSol(negativeIntegralValues);
            return integralsValues.Select((integralValue, i) => Math.Abs(integralValue - expectedError[integralsValues.Length - 1])).Min();
        }

        private T[] AddValueToArray<T>(T[] array, T newValue)
        {
            var newArray = new T[array.Length + 1];
            
            for (var i = 0; i < array.Length; i++)
                newArray[i] = array[i];
            
            newArray[array.Length] = newValue;
            
            return newArray;
        }

        private int CalcAitkinConstant(double[] integralsResult, int[] steps)
        {
            if (integralsResult.Length < 3)
                return 1;
            
            var sh1 = integralsResult[0];
            var sh2 = integralsResult[1];
            var sh3 = integralsResult[2];
            
            var stepsRelation = (double)steps[1] / steps[0];
            
            var aitkinConstant = -Math.Log(Math.Abs((sh3 - sh2) / (sh2 - sh1))) / Math.Log(stepsRelation);

            return (int)aitkinConstant;
        }
        
        //Default grid with 1, 2 end 4 steps
        public int CalcOptStepIqf(double start, double end, double accuracy)
        {
            var defaultIntegralsValue = new[] {CalcIntegralIqf(start, end, 1), CalcIntegralIqf(start, end, 2), CalcIntegralIqf(start, end, 4)};
            var aitkinConstant = CalcAitkinConstant(defaultIntegralsValue, new []{1,2,4});
            var defaultStepsRelation = 2;
            var optimalStepSize = 1.0/2 * Math.Pow(accuracy * (1 - Math.Pow(defaultStepsRelation, -aitkinConstant)) / Math.Abs(defaultIntegralsValue[2] - defaultIntegralsValue[1]), (double)1 / aitkinConstant);
            return (int)(1.0 / optimalStepSize);
        } 
        public int CalcOptStepGqf(double start, double end, double accuracy)
        {
            var defaultIntegralsValue = new[] {CalcIntegralGqf(start, end, 1), CalcIntegralGqf(start, end, 2), CalcIntegralGqf(start, end, 4)};
            var aitkinConstant = CalcAitkinConstant(defaultIntegralsValue, new []{1,2,4});
            var defaultStepsRelation = 2;
            var optimalStepSize = 1.0/2 * Math.Pow(accuracy * (1 - Math.Pow(defaultStepsRelation, -aitkinConstant)) / Math.Abs(defaultIntegralsValue[2] - defaultIntegralsValue[1]), (double)1 / aitkinConstant);
            return (int)(1.0 / optimalStepSize);
        }
    }
}