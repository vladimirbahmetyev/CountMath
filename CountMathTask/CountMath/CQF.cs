using System;

namespace CountMath
{
    public class Cqf
    {
        private readonly Iqf _iqfHelper;

        public Cqf(Iqf baseIqf)
        {
            _iqfHelper = baseIqf;
        }

        public double CalcIntegral(double start, double end, int parts)
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
        
        public double CalcIntegralWithAccuracy(double start, double end, double accuracy, int startCountOfSteps)
        {
            var steps = startCountOfSteps;
            var arrayWithSteps = new[] {startCountOfSteps};
            var integralValues = new double[1];
            integralValues[0] = CalcIntegral(start, end, steps);
            
            var currentAccuracy = CalcAccuracyRichardson(integralValues, arrayWithSteps, start, end);
            
            while (currentAccuracy > accuracy)
            {
                var newIntegralValue = CalcIntegral(start, end, ++steps);

                integralValues = AddValueToArray(integralValues, newIntegralValue);

                arrayWithSteps = AddValueToArray(arrayWithSteps, steps);
                
                currentAccuracy = CalcAccuracyRichardson(integralValues,arrayWithSteps, start, end);
            }
            
            return integralValues[integralValues.Length - 1];
        }
        
        private double CalcAccuracyRichardson(double[] integralsValues, int[] steps, double start, double end)
        {
            if (integralsValues.Length == 1)
                return double.MaxValue;
            
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
            
            var expectedIntegralValue = lupHelper.LesSol(negativeIntegralValues);
            
            return integralsValues[integralsValues.Length - 1] - expectedIntegralValue[integralsValues.Length - 1];
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
            
            var sh1 = integralsResult[integralsResult.Length - 3];
            var sh2 = integralsResult[integralsResult.Length - 2];
            var sh3 = integralsResult[integralsResult.Length - 1];
            
            var stepsRelation = (double)steps[steps.Length - 1] / steps[steps.Length - 2];
            
            var aitkinConstant = -Math.Log(Math.Abs((sh3 - sh2) / (sh2 - sh1))) / Math.Log(stepsRelation);

            return (int)aitkinConstant;
        }
        
        //Default grid with 1, 2 end 4 steps
        public int CalcOptStep(double start, double end, double accuracy)
        {
            var defaultIntegralsValue = new[] {CalcIntegral(start, end, 1), CalcIntegral(start, end, 2), CalcIntegral(start, end, 4)};
            var aitkinConstant = CalcAitkinConstant(defaultIntegralsValue, new []{1,2,4});
            var defaultStepsRelation = 2;
            var optimalStepSize = 1.0/2 * Math.Pow(accuracy * (1 - Math.Pow(defaultStepsRelation, -aitkinConstant)) / Math.Abs(defaultIntegralsValue[2] - defaultIntegralsValue[1]), (double)1 / aitkinConstant);
            return (int)(1.0 / optimalStepSize);
        }
    }
}