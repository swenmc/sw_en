using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace BaseClasses.GraphObj
{
    [Serializable]
    public class CPoint : CEntity
    {
        public double X, Y, Z;

        public CPoint()
        {

        }
        public CPoint(int iPoint_ID, double dX, double dY, double dZ, int fTime)
        {
            ID = iPoint_ID;
            X = dX;
            Y = dY;
            Z = dZ;
            FTime = fTime;
        }

        //----------------------------------------------------------------------------------------------------------------
        public void Create()
        { }

        public void Delete()
        { }

        public void Edit()
        { }

        public void Draw()
        { }
    }

    public class CCompare_PointID : IComparer
    {
        // x<y - zaporne cislo; x=y - nula; x>y - kladne cislo
        public int Compare(object x, object y)
        {
            return ((CPoint)x).ID - ((CPoint)y).ID;
        }
    }
}
