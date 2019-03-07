using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MATH.ARRAY
{
    // Array operations
    public static class ArrayF
    {
        public static float[,] fGetSum(float[,] mat1, float[] mat2)
        {
            if ((int)Math.Sqrt(mat1.Length) == mat2.Length)
            {
                float[,] newMatrix = new float[(int)Math.Sqrt(mat1.Length), (int)Math.Sqrt(mat2.Length)];

                for (int x = 0; x < (int)Math.Sqrt(mat1.Length); x++)
                    for (int y = 0; y < mat2.Length; y++)
                        newMatrix[x, y] = mat1[x, y] + mat2[x];

                return newMatrix;
            }
            else
            {
                //Error - exception
                return null;
            }
        }

        // Interpolation
        // Interpolation - aby fungovalo obecne, je potrebne doriesit kladne a zaporne hodnoty
        public static float GetBilinearInterpolationValuePositive(float[,] array, float[] arrayHeaderColumnValues, float a_row, float b_column)
        {
            float result = 0;

            int iNumberOfColumns = arrayHeaderColumnValues.Length + 1;
            int iNumberOfRows = array.Length / iNumberOfColumns;

            float[] arrayRowSmaller = new float[iNumberOfColumns];
            float[] arrayRowHigher = new float[iNumberOfColumns];
            float[] arrayRowInterpolated = new float[iNumberOfColumns];

            bool bPerformInterpolationBetweenRows = true;

            // Find smaller and higher value in row (defined by first column in array)

            for (int i = 0; i < iNumberOfRows; i++)
            {
                bool bIsAscending = array[iNumberOfRows - 1, 0] - array[0, 0] > 0;

                // Same as row value
                // or if value a_row is less than first value in table column [0] and values are ascending
                // or if value a_row is larger than first value in table column [0] and values are descending
                if (a_row == array[i, 0] ||
                    (a_row < array[0, 0] && bIsAscending) ||
                    (a_row > array[0, 0] && !bIsAscending))
                {
                    for (int j = 0; j < iNumberOfColumns; j++) // Fill arrays of rows
                    {
                        // We dont need first interpolation between row values
                        arrayRowInterpolated[j] = array[i, j];
                    }

                    bPerformInterpolationBetweenRows = false; // It is not necessary to interpolate between row values

                    break;
                }

                if (bIsAscending && (a_row - array[i+1, 0]) < 0) // Between row values - ascending
                {
                    for (int j = 0; j < iNumberOfColumns; j++) // Fill arrays of rows
                    {
                        arrayRowSmaller[j] = array[i, j];
                        arrayRowHigher[j] = array[i + 1, j];
                    }

                    break;
                }

                if(!bIsAscending && (a_row - array[i, 0]) < 0) // Between row values - descending
                {
                    for (int j = 0; j < iNumberOfColumns; j++) // Fill arrays of rows
                    {
                        arrayRowSmaller[j] = array[i + 1, j];
                        arrayRowHigher[j] = array[i, j];
                    }

                    break;
                }
            }

            // First linear Interpolation - Get one Row corresponding to a_row value
            // TODO - Pridat podmienku ze nie je potrebne interpolovat ak je pole uz naplnene

            if (bPerformInterpolationBetweenRows)
            {
                if (arrayRowSmaller.Length != arrayRowHigher.Length)
                {
                    throw new ArgumentException("Arrays must be of equal length.");
                }
                else
                {
                    for (int k = 0; k < arrayRowSmaller.Length; k++)
                    {
                        arrayRowInterpolated[k] = GetLinearInterpolationValuePositive(arrayRowSmaller[0], arrayRowHigher[0], arrayRowSmaller[k], arrayRowHigher[k], a_row);
                    }
                }
            }

            // Second linear interpolation - Between Column values
            float fColumnSmaller = 0;
            float fColumnHigher = 0;
            float fColumnInterpolated = 0;

            for (int i = 0; i < arrayHeaderColumnValues.Length; i++)
            {
                bool bIsAscending = arrayHeaderColumnValues[arrayHeaderColumnValues.Length - 1] - arrayHeaderColumnValues[0] > 0;

                if (b_column == arrayHeaderColumnValues[i] ||
                   (b_column < arrayHeaderColumnValues[0] && bIsAscending) ||
                   (b_column > arrayHeaderColumnValues[0] && !bIsAscending)) // Same as column header value
                {
                    // We dont need interpolation between column values
                    fColumnInterpolated = arrayHeaderColumnValues[i];
                    result = arrayRowInterpolated[i + 1]; // Array of rows has one more item (identificator of row)

                    break;
                }

                if (bIsAscending && (b_column - arrayHeaderColumnValues[i + 1]) < 0) // Between column values - ascending
                {
                    fColumnSmaller = arrayHeaderColumnValues[i];
                    fColumnHigher = arrayHeaderColumnValues[i + 1];

                    result = GetLinearInterpolationValuePositive(fColumnSmaller, fColumnHigher, arrayRowInterpolated[i + 1], arrayRowInterpolated[i + 2], b_column);

                    break;
                }

                if (!bIsAscending && (b_column - arrayHeaderColumnValues[i]) < 0) // Between column values - descending
                {
                    fColumnSmaller = arrayHeaderColumnValues[i + 1];
                    fColumnHigher = arrayHeaderColumnValues[i];

                    result = GetLinearInterpolationValuePositive(fColumnSmaller, fColumnHigher, arrayRowInterpolated[i + 1], arrayRowInterpolated[i + 2], b_column);

                    break;
                }
            }

            return result;
        }

        // Interpolation - aby fungovalo obecne, je potrebne doriesit kladne a zaporne hodnoty
        public static float GetLinearInterpolationValuePositive(float a1, float a2, float b1, float b2, float a)
        {
            return b1 + (((b2 - b1) / (a2 - a1)) * (a - a1));
        }

        // Interpolation - aby fungovalo obecne, je potrebne doriesit kladne a zaporne hodnoty
        public static float GetLinearInterpolationValuePositive(float x, float[] xd, float[] yd)
        {
            // Convert arrays items from float to double

            double x_val = x;
            double[] xd_val;
            double[] yd_val;

            if (xd.Length != yd.Length)
            {
                throw new ArgumentException("Arrays must be of equal length.");
            }
            else
            {
                xd_val = new double[xd.Length];
                yd_val = new double[yd.Length];

                for (int i = 0; i< xd.Length;i++)
                {
                    xd_val[i] = xd[i];
                    yd_val[i] = yd[i];
                }
            }

            return (float)GetLinearInterpolationValuePositive(x_val, xd_val, yd_val); // Return result as double
        }

        public static double GetLinearInterpolationValuePositive(double x, double[] xd, double[] yd)
        {
            if (xd.Length != yd.Length)
            {
                throw new ArgumentException("Arrays must be of equal length.");
            }
            else
            {
                for (int i = 0; i < xd.Length; i++)
                {
                    if (x < xd[0])
                        return yd[0]; // Minimum
                    else if(x > xd[xd.Length - 1])
                        return yd[xd.Length - 1]; // Maximum
                    else
                    {
                        // Find nearest higher value
                        if (x - xd[i] == 0)
                        {
                            return yd[i];
                        }
                        else if (x - xd[i] < 0) // xi > xi-1
                        {
                            if (yd[i] - yd[i - 1] > 0) // yi > yi-1
                            {
                                return yd[i - 1] + ((x - xd[i - 1]) * ((yd[i] - yd[i - 1]) / (xd[i] - xd[i - 1])));
                            }
                            else  // yi < yi-1
                                return yd[i] + (xd[i] - x) * ((yd[i - 1] - yd[i]) / (xd[i] - xd[i - 1]));
                        }

                    }
                }

                return 0; // Error
            }
        }

        public static float[] GetSpecificIndexColumnValuesFromArray2D(float[,] arr, int iColumnIndex)
        {
            int iNumberOfRows = arr.GetLength(0);
            int iNumberOfColumns = arr.GetLength(1);

            float[] outputArray = new float[iNumberOfRows];

            for (int i = 0; i < iNumberOfRows; i++)
            {
                outputArray[i] = arr[i, iColumnIndex];
            }

            return outputArray;
        }

        public static void ShowArrayInfo(Array arr)
        {
            Console.WriteLine("Length of Array:      {0,3}", arr.Length);
            Console.WriteLine("Number of Dimensions: {0,3}", arr.Rank);

            // For multidimensional arrays, show number of elements in each dimension.
            if (arr.Rank > 1)
            {
                for (int dimension = 1; dimension <= arr.Rank; dimension++)
                    Console.WriteLine("   Dimension {0}: {1,3}", dimension, arr.GetUpperBound(dimension - 1) + 1);
            }
            Console.WriteLine();
        }

        public static List<int> GetMultiArrayDimensionList(Array arr)
        {
            List<int> listDimensions = new List<int>();
            //Console.WriteLine("Length of Array:      {0,3}", arr.Length);
            //Console.WriteLine("Number of Dimensions: {0,3}", arr.Rank);

            // For multidimensional arrays, show number of elements in each dimension.
            if (arr.Rank > 1)
            {
                for (int dimension = 1; dimension <= arr.Rank; dimension++)
                {
                    //Console.WriteLine("   Dimension {0}: {1,3}", dimension, arr.GetUpperBound(dimension - 1) + 1);
                    listDimensions.Add(arr.GetUpperBound(dimension - 1) + 1);
                }
            }
            //Console.WriteLine();

            return listDimensions;
        }

        public static void MultiplyMultiFloatArrayItems(float factor, ref float[,] arr)
        {
            // TODO - Toto by bolo super zobecnit pre rozny pocet rozmerov pola a rozny datovy typ (int, float, double)
            List<int> arrayDimensionsList = GetMultiArrayDimensionList(arr);

            for (int i = 0; i < arrayDimensionsList[0]; i++)
            {
                for (int j = 0; j < arrayDimensionsList[1]; j++)
                {
                    arr[i,j] *= -1f;
                }
            }
        }
    }
}
