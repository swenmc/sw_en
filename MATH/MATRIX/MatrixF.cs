using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MATH
{
    // Matrix opertations

    public static class MatrixF
    {
        // Return transformed matrix - change rows and columns
        public static CMatrix GetTransMatrix(CMatrix fM)
        {
            // Number of Matrix M rows and columns
            int iM_iRowsMax = (int)Math.Sqrt(fM.m_fArrMembers.Length); // square
            int iM_jColsMax = (int)Math.Sqrt(fM.m_fArrMembers.Length);

            // Output Matrix
            CMatrix fM_T = new CMatrix(iM_jColsMax, iM_iRowsMax);

            for (int i = 0; i < iM_jColsMax; i++)
                for (int j = 0; j < iM_iRowsMax; j++)
                    fM_T.m_fArrMembers[i, j] = fM.m_fArrMembers[j, i];
            return fM_T;
        }

        // Return result of matrix multiplying
        public static CMatrix fMultiplyMatr(CMatrix fM1, CMatrix fM2)
        {
            if (fM1.m_fArrMembers == null || fM2.m_fArrMembers == null) // Empty matrix
            {
                throw new ArgumentException();
            }

            // Number of Matrix M1 rows and columns
            int iM1_iRowsMax = (int)Math.Sqrt(fM1.m_fArrMembers.Length); // square
            int iM1_jColsMax = (int)Math.Sqrt(fM1.m_fArrMembers.Length);

            // Number of Matrix M2 rows and columns
            int iM2_iRowsMax = (int)Math.Sqrt(fM2.m_fArrMembers.Length);
            int iM2_jColsMax = (int)Math.Sqrt(fM2.m_fArrMembers.Length);

            // Number of columns of the first one must be equal to number of rows of the second
            // Number of rows of the first one must be equal to number of columns of the second

            if (iM1_jColsMax != iM2_iRowsMax)
            {
                throw new ArgumentException();
            }
            // Output Matrix
            CMatrix fM = new CMatrix(iM1_iRowsMax, iM2_jColsMax);

            for (int i = 0; i < iM1_iRowsMax; ++i)
            {
                for (int j = 0; j < iM2_jColsMax; ++j)
                {
                    float sum = 0;
                    for (int it = 0; it < iM1_jColsMax; ++it)
                    {
                        sum += fM1.m_fArrMembers[i, it] * fM2.m_fArrMembers[it, j];
                    }
                    fM.m_fArrMembers[i, j] = sum;
                }
            }
            return fM;
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Change matrix sign
        ///// ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public static CMatrix fChangeSignMatr(CMatrix fM)
        {
            for (int i = 0; i < (int)Math.Sqrt(fM.m_fArrMembers.Length); i++)
            {
                for (int j = 0; j < (int)Math.Sqrt(fM.m_fArrMembers.Length); j++)
                    fM.m_fArrMembers[i, j] = -fM.m_fArrMembers[i, j];
            }
            return fM;
        }
    }
}
