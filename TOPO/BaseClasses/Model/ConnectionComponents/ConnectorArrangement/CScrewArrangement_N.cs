using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BaseClasses
{
    public class CScrewArrangement_N : CScrewArrangement
    {
        public CScrewArrangement_N() { }

        public CScrewArrangement_N(int iScrewsNumber_temp, CScrew referenceScrew_temp)
        {
            IHolesNumber = iScrewsNumber_temp;
            referenceScrew = referenceScrew_temp;
        }

        public void Calc_HolesCentersCoord2D(float fbX1, float fbX2, float fbX3, float fhY)
        {
            // 3 x 4 screws = 12 screws in the plate (square arrangement 4 screws in group)

            float fx_edge = 0.020f;
            float fy_edge = 0.020f;

            if (IHolesNumber > 0)
            {
                HolesCentersPoints2D = new Point[IHolesNumber];

                // Left back
                HolesCentersPoints2D[0] = new Point(fx_edge, fy_edge);

                HolesCentersPoints2D[1] = new Point(fbX1 - fx_edge, HolesCentersPoints2D[0].Y);

                HolesCentersPoints2D[2] = new Point(HolesCentersPoints2D[0].X, fhY - fy_edge);

                HolesCentersPoints2D[3] = new Point(HolesCentersPoints2D[1].X, HolesCentersPoints2D[2].Y);

                // Right back
                HolesCentersPoints2D[4] = new Point(fbX1 + 2 * fbX2 + fbX3 + fx_edge, HolesCentersPoints2D[0].Y);

                HolesCentersPoints2D[5] = new Point(2 * fbX1 + 2 * fbX2 + fbX3 - fx_edge, HolesCentersPoints2D[4].Y);

                HolesCentersPoints2D[6] = new Point(HolesCentersPoints2D[4].X, HolesCentersPoints2D[2].Y);

                HolesCentersPoints2D[7] = new Point(HolesCentersPoints2D[5].X, HolesCentersPoints2D[2].Y);

                // Middle front
                HolesCentersPoints2D[8] = new Point(fbX1 + fbX2 + fx_edge, HolesCentersPoints2D[0].Y);

                HolesCentersPoints2D[9] = new Point(fbX1 + fbX2 + fbX3 - fx_edge, HolesCentersPoints2D[0].Y);

                HolesCentersPoints2D[10] = new Point(HolesCentersPoints2D[8].X, HolesCentersPoints2D[2].Y);

                HolesCentersPoints2D[11] = new Point(HolesCentersPoints2D[9].X, HolesCentersPoints2D[2].Y);
            }
        }
    }
}
