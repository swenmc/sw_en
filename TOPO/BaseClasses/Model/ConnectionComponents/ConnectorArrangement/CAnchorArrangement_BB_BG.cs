using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Media3D;
using System.Globalization;
using MATH;

namespace BaseClasses
{
    [Serializable]
    public class CAnchorArrangement_BB_BG : CAnchorArrangement_Rectangular
    {
        private float m_fDistanceBetweenHoles;

        public float DistanceBetweenHoles
        {
            get
            {
                return m_fDistanceBetweenHoles;
            }

            set
            {
                m_fDistanceBetweenHoles = value;
            }
        }

        public CAnchorArrangement_BB_BG() { }

        public CAnchorArrangement_BB_BG(CAnchor referenceAnchor_temp)
        {
            IHolesNumber = 2; // 2 Otvory
            NumberOfAnchorsInYDirection = 1;
            NumberOfAnchorsInZDirection = 2;
            referenceAnchor = referenceAnchor_temp;
            HoleRadius = 0.5f * referenceAnchor.Diameter_thread; // Anchor diameter
            RadiusAngle = 360; // Circle total angle to generate holes
        }

        public CAnchorArrangement_BB_BG(float fDistanceBetweenHoles_temp, CAnchor referenceAnchor_temp)
        {
            IHolesNumber = 2; // 2 Otvory
            NumberOfAnchorsInYDirection = 1;
            NumberOfAnchorsInZDirection = 2;
            DistanceBetweenHoles = fDistanceBetweenHoles_temp;
            referenceAnchor = referenceAnchor_temp;
            HoleRadius = 0.5f * referenceAnchor.Diameter_thread; // Anchor diameter
            RadiusAngle = 360; // Circle total angle to generate holes
        }

        public void Calc_HolesCentersCoord2D(float fbX, float fhY, float flZ)
        {
            DistanceBetweenHoles = 0.5f * fhY; // Default

            HolesCentersPoints2D = new Point[IHolesNumber];
            HolesCentersPoints2D[0] = new Point(flZ + 0.5f * fbX, 0.5f * fhY - 0.5f * DistanceBetweenHoles);
            HolesCentersPoints2D[1] = new Point(HolesCentersPoints2D[0].X, 0.5f * fhY + 0.5f * DistanceBetweenHoles);
        }

        public void Calc_HolesCentersCoord3D(float fbX, float fhY, float flZ)
        {
            holesCentersPointsfor3D = new Point[IHolesNumber];
            holesCentersPointsfor3D[0] = new Point(0.5f * fbX, 0.5f * fhY - 0.5f * DistanceBetweenHoles);
            holesCentersPointsfor3D[1] = new Point(0.5f * fbX, 0.5f * fhY + 0.5f * DistanceBetweenHoles);
        }
    }
}
