using System;

namespace BaseClasses.GraphObj
{
    [Serializable]
    public class CArea : CEntity3D
    {
        //public int m_iPointsCollection = new Int32Collection(); // List / Collection of points IDs

        public int[] m_iPointsCollection; // List / Collection of points IDs

        public CArea()
        {
        
        }

        // Constructor 2
        public CArea(int iArea_ID, int[] iPCollection, int fTime)
        {
            ID = iArea_ID;
            m_iPointsCollection = iPCollection;
            FTime = fTime;
        }
    }
}
