using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MATH
{
    public class CArray
    {

        public CArray()
        {


        }

        // Array size conversion

        // Transformation of 1D matrix array/matrix to 2D square array/matrix 
        public float[,] ArrTranf1Dto2D(float[] fArray1D)
        {
            // Check number of input elements
            if ((((decimal)Math.Sqrt(fArray1D.Length)) % 2) != 0)
            { 
            // Exception   
                
            };

            float[,] fArray2D = new float[(int)Math.Sqrt(fArray1D.Length), (int)Math.Sqrt(fArray1D.Length)];

            for (int i = 0; i < (int)Math.Sqrt(fArray1D.Length); i++)
            {
                for (int j = 0; j < (int)Math.Sqrt(fArray1D.Length); j++)
                    fArray2D[i, j] = fArray1D[i * (int)Math.Sqrt(fArray1D.Length) + j];
            }

            return fArray2D;
        }



        // Transformation of 2D square array/matrix to 1D 
        public float[] ArrTranf2Dto1D(float[,] fArray2D)
        {
            float[] fArray1D = new float[fArray2D.Length];

            for (int i = 0; i < (int)Math.Sqrt(fArray2D.Length); i++)
            {
                for (int j = 0; j < (int)Math.Sqrt(fArray2D.Length); j++)
                    fArray1D[i * (int)Math.Sqrt(fArray2D.Length) + j] = fArray2D[i, j];
            }

            return fArray1D;
        }

        // Transformation of 2D rectangular array/matrix to 1D
        public float[] ArrTranf2Dto1D(float[,] fArray2D, int iRow, int iCol)
        {
            float[] fArray1D = new float[iRow * iCol];

            for (int i = 0; i < iRow; i++)
            {
                for (int j = 0; j < iCol; j++)
                    fArray1D[i * (int)Math.Sqrt(fArray2D.Length) + j] = fArray2D[i, j];
            }

            return fArray1D;
        }





        // Type Conversion
        public double[] ArrConverFloatToDouble1D(float[] fArrayFloat)
        {
            double[] fArrayDouble = new double[fArrayFloat.Length];

            for (int i = 0; i < fArrayDouble.Length; i++)
            {
                fArrayDouble[i] = fArrayFloat[i];
            }

            return fArrayDouble;
        }

        public float[] ArrConverDoubleToFloat1D(double[] fArrayDouble)
        {
            float[] fArrayFloat = new float[fArrayDouble.Length];

            for (int i = 0; i < fArrayFloat.Length; i++)
            {
                fArrayFloat[i] = (float)fArrayDouble[i];
            }

            return fArrayFloat;
        }


        // MatrixF64 fo float Array 1D
        public float[] ArrConverMatrixF64ToFloat1D(MatrixF64 M)
        {
            float[] fArrayFloat = new float[M.Rows() * M.Columns()];

            for (int i = 0; i < fArrayFloat.Length; i++)
            {
                fArrayFloat[i] = (float)M.Entries[i];
            }

            return fArrayFloat;
        }
    }
}
