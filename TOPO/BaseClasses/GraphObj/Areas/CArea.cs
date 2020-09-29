using System;

namespace BaseClasses.GraphObj
{
    [Serializable]
    public class CArea : CEntity3D
    {
        public CArea()
        {
        
        }

        // Constructor 2
        public CArea(int iArea_ID, int fTime)
        {
            ID = iArea_ID;
            FTime = fTime;
        }
    }
}
