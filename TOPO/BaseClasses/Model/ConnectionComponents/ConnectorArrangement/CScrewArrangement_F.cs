using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BaseClasses
{
    [Serializable]
    public class CScrewArrangement_F : CScrewArrangement
    {
        public CScrewArrangement_F() { }

        public CScrewArrangement_F(int iScrewsNumber_temp, CScrew referenceScrew_temp)
        {
            IHolesNumber = iScrewsNumber_temp;
            referenceScrew = referenceScrew_temp;
        }

        public void Calc_HolesCentersCoord2D(float fbX1, float fbX2, float fhY, float flZ)
        {
            float fx_edge = flZ * 0.5f; // Middle of left leg
            float fy_edge1 = 0.010f;
            float fy_edge2 = 0.030f;
            float fx_edge1 = 0.010f;
            // float fx_edge2 = 0.030f;

            float fConnectedSectionDepth = 0.27f;
            float fx_edge21 = (fbX2 - 2 * fx_edge1) / 3f; // Todo - v rade su 4 skrutky a 3 medzery, bolo by super urobit to dynamicke
            float fb_temp_rightBottomRow = fbX1 + ((fbX2 - fbX1) / fhY * (fhY - fConnectedSectionDepth + fy_edge1));
            float fx_edge22 = (fb_temp_rightBottomRow - 2 * fx_edge1) / 1f; // Todo - v rade su 2 skrutky a 1 medzera, bolo by super urobit to dynamicke

            // TODO nahradit enumom a switchom

            if (IHolesNumber > 0)
            {
                HolesCentersPoints2D = new Point[IHolesNumber];

                // Left Leg
                // Bottom
                HolesCentersPoints2D[0] = new Point(fx_edge, fy_edge1);

                HolesCentersPoints2D[1] = new Point(HolesCentersPoints2D[0].X, HolesCentersPoints2D[0].Y + fy_edge2);

                HolesCentersPoints2D[2] = new Point(HolesCentersPoints2D[0].X, HolesCentersPoints2D[1].Y + fy_edge2);

                HolesCentersPoints2D[3] = new Point(HolesCentersPoints2D[0].X, HolesCentersPoints2D[2].Y + fy_edge2);

                // Top
                HolesCentersPoints2D[4] = new Point(HolesCentersPoints2D[0].X, fhY - fy_edge1);

                HolesCentersPoints2D[5] = new Point(HolesCentersPoints2D[0].X, HolesCentersPoints2D[4].Y - fy_edge2);

                HolesCentersPoints2D[6] = new Point(HolesCentersPoints2D[0].X, HolesCentersPoints2D[5].Y - fy_edge2);

                HolesCentersPoints2D[7] = new Point(HolesCentersPoints2D[0].X, HolesCentersPoints2D[6].Y - fy_edge2);

                // Right Leg
                // Bottom
                HolesCentersPoints2D[8] = new Point(flZ + fx_edge1, fhY - fConnectedSectionDepth + fy_edge1);

                HolesCentersPoints2D[9] = new Point(HolesCentersPoints2D[8].X + fx_edge22, HolesCentersPoints2D[8].Y);

                // Top
                HolesCentersPoints2D[10] = new Point(flZ + fx_edge1, fhY - fy_edge1);

                HolesCentersPoints2D[11] = new Point(HolesCentersPoints2D[10].X + fx_edge21, HolesCentersPoints2D[10].Y);

                HolesCentersPoints2D[12] = new Point(HolesCentersPoints2D[11].X + fx_edge21, HolesCentersPoints2D[10].Y);

                HolesCentersPoints2D[13] = new Point(HolesCentersPoints2D[12].X + fx_edge21, HolesCentersPoints2D[10].Y);
            }
            else
            {
                // Not defined expected number of holes for L or F plate
            }
        }
    }
}
