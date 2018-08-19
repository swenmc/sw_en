using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MATH
{
    public class CVector
    {
        private int m_iRows;

        public int IRows
        {
            get { return m_iRows; }
            set { m_iRows = value; }
        }

        private float[] m_fVectorItems;

        public float[] FVectorItems
        {
            get { return m_fVectorItems; }
            set { m_fVectorItems = value; }
        }

        public CVector()
        { 
        
        
        }
        public CVector(int i)
        {
            m_iRows = i;
            m_fVectorItems = new float [i];
        }
        public CVector(int i, params float[] fV)
        {
            m_iRows = i;
            m_fVectorItems = new float[i];
        }



        //// GENERAL VECTOR OPERATIONS

        // Return sum of vectors / 1D arrays
        public float[] GetVectorSum(int iV_jRowsMax, params float[]fV)
        {
            m_iRows = iV_jRowsMax;

             float[] fVa = new float[iV_jRowsMax];

            // Output Vector
            for(int i = 0 ; i < fV.Length; i++)  // for each vector
            {
                for (int j = 0; j < iV_jRowsMax; j++)  // for each vector member
                    {
                    // j-vector of i-vector 
                    //   fV[i];
                    }
            }
            return fVa;
        }

        public void Print1DVector()
        {
            int precision = 5;
            int largestEntryWidth = 1;

            // Find largest entity

            for (int j = 0; j < m_fVectorItems.Length; j++)
            {
                String text = String.Format(String.Empty
                    + '{' + '0' + ':' + 'g' + precision + '}', m_fVectorItems[j]);
                if (text.Length > largestEntryWidth)
                {
                    largestEntryWidth = text.Length;
                }
            }

            if (m_fVectorItems != null)
            {
                System.Console.Write('[');

                for (int i = 0; i < m_fVectorItems.Length; i++)
                {
                    System.Console.Write(' ');
                    String text = String.Format(String.Empty
                    + '{' + '0' + ',' + largestEntryWidth + ':' + 'g'
                    + precision + '}', m_fVectorItems[i]);

                    System.Console.Write(text);

                    if (i < m_fVectorItems.Length - 1)
                    {
                        System.Console.Write(';');
                    }
                    else
                    {
                        System.Console.Write(' ');
                    }
                }
                System.Console.WriteLine(']');
            }
            else
            {
                System.Console.WriteLine("Vector is empty!");
            }
        }
    }
}
