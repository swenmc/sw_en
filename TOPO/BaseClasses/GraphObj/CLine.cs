using System;
using System.Windows;

namespace BaseClasses.GraphObj
{
    [Serializable]
    public class CLine : CEntity
    {
        public int[] m_iPointsCollection; // List / Collection of points IDs

        public CLine()
        {
        
        }

        // Constructor 2
        public CLine(int iLine_ID, int[] iPCollection, int fTime)
        {
            ID = iLine_ID;
            m_iPointsCollection = iPCollection;
            FTime = fTime;
        }
    }
}
