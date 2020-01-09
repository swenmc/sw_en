using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BaseClasses
{
    [Serializable]
    public class CScrewArrangement_G : CScrewArrangement
    {
        public CScrewArrangement_G() { }

        public CScrewArrangement_G(int iScrewsNumber_temp, CScrew referenceScrew_temp)
        {
            IHolesNumber = iScrewsNumber_temp;
            referenceScrew = referenceScrew_temp;
        }

        public void Calc_HolesCentersCoord2D(float fbX1, float fbX2, float fhY, float flZ)
        {
            float fx_edge = flZ * 0.5f; // Middle of left leg
            float fy_edge1 = 0.200f;
            float fy_edge2 = 0.050f;

            if (IHolesNumber > 0)
            {
                HolesCentersPoints2D = new Point[IHolesNumber];

                // Left Leg
                // Bottom
                HolesCentersPoints2D[0] = new Point(fx_edge, fy_edge1);
                HolesCentersPoints2D[1] = new Point(HolesCentersPoints2D[0].X, HolesCentersPoints2D[0].Y + fy_edge2);

                // TODO
            }
            else
            {
                // Not defined expected number of holes for G plate
            }
        }
    }
}
