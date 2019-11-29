using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BaseClasses
{
    [Serializable]
    public class CScrewArrangement_M:CScrewArrangement
    {
        public CScrewArrangement_M() { }

        public CScrewArrangement_M(int iScrewsNumber_temp, CScrew referenceScrew_temp)
        {
            IHolesNumber = iScrewsNumber_temp;
            referenceScrew = referenceScrew_temp;
        }

        public void Calc_HolesCentersCoord2D(float ft, float fbX1, float fbX2, float fbX3, float fhY)
        {
            float fx_edge = 0.015f;  // x-direction
            float fy_edge = 0.015f;  // y-direction

            if (IHolesNumber > 0)
            {
                HolesCentersPoints2D = new Point[IHolesNumber];

                HolesCentersPoints2D[0] = new Point(fx_edge, fy_edge);

                HolesCentersPoints2D[1] = new Point(fx_edge, fhY - fy_edge);

                HolesCentersPoints2D[2] = new Point(fbX1 + 0.5f * fbX2, fy_edge);

                HolesCentersPoints2D[3] = new Point(fbX1 + 0.5f * fbX2, fhY - fy_edge);

                HolesCentersPoints2D[4] = new Point(fbX1 + fbX2 + fbX3 - fx_edge, fy_edge);

                HolesCentersPoints2D[5] = new Point(fbX1 + fbX2 + fbX3 - fx_edge, fhY - fy_edge);
            }
        }
    }
}
