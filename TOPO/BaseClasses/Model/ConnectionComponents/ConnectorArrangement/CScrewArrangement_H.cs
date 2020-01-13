using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BaseClasses
{
    [Serializable]
    public class CScrewArrangement_H : CScrewArrangement
    {
        // TODO - prerobit na properties
        // Zadavat v konstruktore
        public int iRows_BottomLeg = 7;
        public int iColumns_BottomLeg = 7;
        public float fx_edge_BottomLeg = 0.04f;
        public float fy_edge_BottomLeg = 0.05f;
        public float fx_spacing_BottomLeg = 0.07f;
        public float fy_spacing_BottomLeg = 0.05f;

        public int iRows_TopLeg = 3;
        public int iColumns_TopLeg = 3;
        public float fx_edge_TopLeg = 0.04f;
        public float fy_edge_TopLeg = 0.05f;
        public float fx_spacing_TopLeg = 0.03f; //0.07f;
        public float fy_spacing_TopLeg = 0.03f; //0.05f;

        public int iNumberOfScrews_BottomLeg = 49;
        public int iNumberOfScrews_TopLeg = 9;

        public CScrewArrangement_H() { }

        public CScrewArrangement_H(/*int iScrewsNumber_temp,*/ CScrew referenceScrew_temp)
        {
            //IHolesNumber = iScrewsNumber_temp;
            //IHolesNumber = iRows_BottomLeg * iColumns_BottomLeg + iRows_TopLeg * iColumns_TopLeg;
            referenceScrew = referenceScrew_temp;
        }

        public void Calc_HolesCentersCoord2D(float fbX, float fhY1, float fhY2, float fMainMemberWidth)
        {
            iColumns_BottomLeg = GetNumberOfEquallySpacedConnectors(fbX, fx_spacing_BottomLeg);
            iRows_BottomLeg = GetNumberOfEquallySpacedConnectors(fhY1, fy_spacing_BottomLeg);

            fx_edge_BottomLeg = GetEdgeDistanceOfEquallySpacedConnectors(fbX, fx_spacing_BottomLeg, iColumns_BottomLeg);
            fy_edge_BottomLeg = GetEdgeDistanceOfEquallySpacedConnectors(fhY1, fy_spacing_BottomLeg, iRows_BottomLeg);

            iColumns_TopLeg = GetNumberOfEquallySpacedConnectors(fMainMemberWidth, fx_spacing_TopLeg);
            iRows_TopLeg = GetNumberOfEquallySpacedConnectors(fhY2 - fhY1, fy_spacing_TopLeg);

            fx_edge_TopLeg = GetEdgeDistanceOfEquallySpacedConnectors(fMainMemberWidth, fx_spacing_TopLeg, iColumns_TopLeg);
            fy_edge_TopLeg = GetEdgeDistanceOfEquallySpacedConnectors(fhY2 - fhY1, fy_spacing_TopLeg, iRows_TopLeg);

            iNumberOfScrews_BottomLeg = iRows_BottomLeg * iColumns_BottomLeg;
            iNumberOfScrews_TopLeg = iRows_TopLeg * iColumns_TopLeg;

            IHolesNumber = iNumberOfScrews_BottomLeg + iNumberOfScrews_TopLeg;

            if (IHolesNumber > 0)
            {
                HolesCentersPoints2D = new Point[IHolesNumber];

                // Bottom leg
                Point[] pointsBottomLeg = GetRegularArrayOfPointsInCartesianCoordinates(new Point(fx_edge_BottomLeg, fy_edge_BottomLeg), iColumns_BottomLeg, iRows_BottomLeg, fx_spacing_BottomLeg, fy_spacing_BottomLeg);

                // Top Leg
                Point[] pointsTopLeg = GetRegularArrayOfPointsInCartesianCoordinates(new Point(fx_edge_TopLeg, fhY1 + fy_edge_TopLeg), iColumns_TopLeg, iRows_TopLeg, fx_spacing_TopLeg, fy_spacing_TopLeg);

                // Merge arrays
                HolesCentersPoints2D = pointsBottomLeg.Concat(pointsTopLeg).ToArray();
            }
            else
            {
                // Not defined expected number of holes for G plate
            }
        }
    }
}
