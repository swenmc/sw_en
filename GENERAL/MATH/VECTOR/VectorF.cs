using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MATH
{
    // Static class - vector operations
    public static class VectorF
    {
        // Return result of matrix and vector multiplying
        public static CVector fMultiplyMatrVectr(CMatrix fM1, CVector fM2)
        {
            // Number of Matrix M1 rows and columns
            int iM1_iRowsMax = (int)Math.Sqrt(fM1.m_fArrMembers.Length); // square
            int iM1_jColsMax = (int)Math.Sqrt(fM1.m_fArrMembers.Length);

            // Number of Matrix M2 rows and columns
            int iM2_iRowsMax = (int)fM2.FVectorItems.Length;

            // Number of columns of the first one must be equal to number of rows of the second
            // Number of rows of the first one must be equal to number of columns of the second

            if (iM1_jColsMax != iM2_iRowsMax)
            {
                throw new ArgumentException();
            }
            // Output Matrix
            CVector fM = new CVector(iM1_iRowsMax);

            for (int i = 0; i < iM1_iRowsMax; ++i)
            {
                float sum = 0;
                for (int it = 0; it < iM1_jColsMax; ++it)
                {
                    sum += fM1.m_fArrMembers[i, it] * fM2.FVectorItems[it];
                }
                fM.FVectorItems[i] = sum;
            }
            return fM;
        }

        // Return result of vectors multiplying
        public static CVector fMultiplyVectors(CVector fM1, CVector fM2)
        {
            // Number of Matrix M1 columns
            int iM1_jColsMax = fM1.FVectorItems.Length;

            // Number of Matrix M2 rows
            int iM2_iRowsMax = fM2.FVectorItems.Length;

            if (iM1_jColsMax != iM2_iRowsMax)
            {
                throw new ArgumentException();
            }
            // Output Matrix
            CVector fM = new CVector(iM1_jColsMax);

            for (int i = 0; i < iM1_jColsMax; ++i)
                fM.FVectorItems[i] = fM1.FVectorItems[i] * fM2.FVectorItems[i];

            return fM;
        }

        public static CVector fGetSum(CVector mat1, CVector mat2)
        {
            if (mat1.FVectorItems.Length == mat2.FVectorItems.Length)
            {
                CVector newMatrix = new CVector(mat1.FVectorItems.Length);

                for (int x = 0; x < mat1.FVectorItems.Length; x++)
                    newMatrix.FVectorItems[x] = mat1.FVectorItems[x] + mat2.FVectorItems[x];

                return newMatrix;
            }
            else
            {
                //Error - exception
                return null;
            }
        }

        public static CVector fGetSum(CMatrix mat1, CVector mat2)
        {
            // Skontrolovat !!!!!!
            if ((int)Math.Sqrt(mat1.m_fArrMembers.Length) == mat2.FVectorItems.Length)
            {
                CVector newMatrix = new CVector(mat2.FVectorItems.Length);

                for (int x = 0; x < (int)Math.Sqrt(mat1.m_fArrMembers.Length); x++)
                    for (int y = 0; y < mat2.FVectorItems.Length; y++)
                        newMatrix.FVectorItems[x] = mat1.m_fArrMembers[x, y] + mat2.FVectorItems[x];

                return newMatrix;
            }
            else
            {
                //Error - exception
                return null;
            }
        }

        public static CVector fGetSum(CVector mat2, CMatrix mat1)
        {
           return fGetSum(mat1, mat2);
        }

        public static string Print1DVector(float[] fV)
        {
            string sOutput = null;
            foreach (float f in fV)
            {
                sOutput += f.ToString();
                sOutput += "\n";
            }

            return sOutput;
        }

        public static string Print1DVector(CVector fV)
        {
            string sOutput = null;
            foreach (float f in fV.FVectorItems)
            {
                sOutput += f.ToString();
                sOutput += "\n";
            }

            return sOutput;
        }
    }
}
