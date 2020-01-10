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
        // TODO - prerobit na properties
        // Zadavat v konstruktore
        public int iRows_LeftLeg = 3;
        public int iColumns_LeftLeg = 1;
        public float fx_edge_LeftLeg /*= 0.05f*/;
        public float fy_edge_LeftLeg = 0.6f;
        public float fx_spacing_LeftLeg = 0.00f;
        public float fy_spacing_LeftLeg = 0.07f;

        public int iRows_RightLeg = 5;
        public int iColumns_RightLeg = 4;
        public float fx_edge_RightLeg = 0.03f;
        public float fy_edge_RightLeg = 0.05f;
        public float fx_spacing_RightLeg = 0.08f;
        public float fy_spacing_RightLeg = 0.05f;

        public CScrewArrangement_G() { }

        public CScrewArrangement_G(/*int iScrewsNumber_temp,*/ CScrew referenceScrew_temp)
        {
            //IHolesNumber = iScrewsNumber_temp;
            IHolesNumber = iRows_LeftLeg * iColumns_LeftLeg + iRows_RightLeg * iColumns_RightLeg;
            referenceScrew = referenceScrew_temp;
        }

        public void Calc_HolesCentersCoord2D(float fbX1, float fbX2, float fhY, float flZ)
        {
            fx_edge_LeftLeg = flZ * 0.5f; // Middle of left leg

            if (IHolesNumber > 0)
            {
                HolesCentersPoints2D = new Point[IHolesNumber];

                // Left leg
                Point[] pointsLeftLeg = GetRegularArrayOfPointsInCartesianCoordinates(new Point(fx_edge_LeftLeg, fy_edge_LeftLeg), iColumns_LeftLeg, iRows_LeftLeg, fx_spacing_LeftLeg, fy_spacing_LeftLeg);

                // Right Leg
                Point[] pointsRightLeg = GetRegularArrayOfPointsInCartesianCoordinates(new Point(flZ + fx_edge_RightLeg, fy_edge_RightLeg), iColumns_RightLeg, iRows_RightLeg, fx_spacing_RightLeg, fy_spacing_RightLeg);

                // Merge arrays
                HolesCentersPoints2D = pointsLeftLeg.Concat(pointsRightLeg).ToArray();
            }
            else
            {
                // Not defined expected number of holes for G plate
            }
        }
    }
}
