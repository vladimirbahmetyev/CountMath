using System;

namespace CountMath
{
    public class Cqf
    {
        private readonly Iqf _iqfHelper;

        public Cqf(Func<double, double> mainFunction, Func<double, double> weightFunction, double paramA, double paramB)
        {
            _iqfHelper = new Iqf(mainFunction, weightFunction, paramA, paramB);
        }

        public double CalcIntegral(double start, double end, int parts)
        {
            var result = 0.0;
            var partStep = (end - start) / parts;
            for (var i = 0; i < parts; i++)
            {
                var nodes = new double[3];
                
                nodes[0] = start + partStep * i;
                nodes[1] = start + partStep * (i + 1.0 / 2);
                nodes[2] = start + partStep * (i + 1);

                result += _iqfHelper.CalcIntegral(nodes[0], nodes[2], nodes); 
            }
            return result;
        }
        
        public double CalcIntegralWithAccuracy(double start, double end, double accuracy)
        {
            var parts = 1;
            var resultList = new double[1];
            resultList[0] = CalcIntegral(start, end, parts);
            var currentAccuracy = CalcAccuracyRichardson(resultList, start, end);
            
            while (currentAccuracy > accuracy)
            {
                var newValue = CalcIntegral(start, end, ++parts);
                
                resultList = AddValueToArray(resultList, newValue);
                
                currentAccuracy = CalcAccuracyRichardson(resultList, start, end);
                
                if(resultList.Length > 3)
                    Console.WriteLine(CalcOptStep(resultList, accuracy));
            }
            //Console.WriteLine(resultList.Length);
            
            return resultList[resultList.Length - 1];
        }
        
        private double CalcAccuracyRichardson(double[] integralsValue, double start, double end)
        {
            if (integralsValue.Length == 1)
                return double.MaxValue;
            
            var m = (int)CalcEytkin(integralsValue);
            var countOfSteps = 1;
            var lesRich = new double[integralsValue.Length][];
            for (var i = 0; i < integralsValue.Length; i++)
            {
                lesRich[i] = new double[integralsValue.Length];
                for (var j = 0; j < integralsValue.Length - 1; j++)
                {
                    lesRich[i][j] = Math.Pow((end - start) / countOfSteps, m + j);
                }

                lesRich[i][integralsValue.Length - 1] = -1;

                countOfSteps++;
            }

            var bVector = new double[integralsValue.Length];
            for (var i = 0; i < integralsValue.Length; i++)
            {
                bVector[i] = -integralsValue[i];
            }
            
            var lupHelper = new Lup(lesRich);
            
            var solution = lupHelper.LesSol(bVector);
            
            return integralsValue[integralsValue.Length - 1] - solution[integralsValue.Length - 1];
        }

        private double[] AddValueToArray(double[] array, double newValue)
        {
            var newArray = new double[array.Length + 1];
            for (int i = 0; i < array.Length; i++)
            {
                newArray[i] = array[i];
            }
            newArray[array.Length] = newValue;
            return newArray;
        }

        private double CalcEytkin(double[] integralsResult)
        {
            if (integralsResult.Length < 3)
                return 2;
            var sh1 = integralsResult[integralsResult.Length - 3];
            var sh2 = integralsResult[integralsResult.Length - 2];
            var sh3 = integralsResult[integralsResult.Length - 1];
            double l = (double)integralsResult.Length / (integralsResult.Length - 1);
            
            var m = -Math.Log(Math.Abs((sh3 - sh2) / (sh2 - sh1))) / Math.Log(l);

            return m;
        }

        private int CalcOptStep(double[] integralsResult, double accuracy)
        {
            var m = CalcEytkin(integralsResult);
            var l = (double)integralsResult.Length / (integralsResult.Length - 1);
            var sh1 = integralsResult[integralsResult.Length - 2];
            var sh2 = integralsResult[integralsResult.Length - 1];
            return (int) (integralsResult.Length * Math.Pow(accuracy*(1 - Math.Pow(l, -m))/Math.Abs(sh2 - sh1), 1/m));
        }
    }
}