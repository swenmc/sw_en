using System;
using System.Windows.Media.Media3D;
using System.Collections.Generic;

namespace BaseClasses.GraphObj
{
    [Serializable]
    public class CAreaRectangular : CEntity3D
    {
        public CAreaRectangular()
        {
        
        }

        // Constructor 2
        public CAreaRectangular(int iArea_ID, int fTime)
        {
            ID = iArea_ID;
            FTime = fTime;
        }
    }
}
