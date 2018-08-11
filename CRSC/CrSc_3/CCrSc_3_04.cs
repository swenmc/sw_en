using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Media;
using MATH;
using BaseClasses;

namespace CRSC
{
    public class CCrSc_3_04 : CSO
    {
        // Rolled L section (angle with unequal legs)

        //----------------------------------------------------------------------------
        private float m_fh;                 // Depth / Vyska
        private float m_fb;                 // Width  / Sirka
        private float m_ft;                 // Leg Thickness / Hrubka ramena
        private float m_fr_1;               // Radius
        private float m_fr_2;               // Radius - flange edge
        private float m_fy_c;               // Centroid coordinate / Suradnica tažiska / Absolute value
        private float m_fz_c;               // Centroid coordinate / Suradnica tažiska / Absolute value
        private short m_iNumOfArcSegment;   // Number of Arc Segments
        private short m_iNumOfArcPoints;    // Number of Arc Points
        private short m_iNumOfAuxPoints;    // Number of Auxialiary Points
        //private short m_iTotNoPoints;       // Total Number of Cross-section Points for Drawing
        //public float[,] m_CrScPoint;        // Array of Points and values in 2D
        //----------------------------------------------------------------------------

        public float Ft
        {
            get { return m_ft; }
            set { m_ft = value; }
        }
        public float Fr_1
        {
            get { return m_fr_1; }
            set { m_fr_1 = value; }
        }
        public float Fr_2
        {
            get { return m_fr_2; }
            set { m_fr_2 = value; }
        }
        public float Fy_c
        {
          get { return m_fy_c; }
          set { m_fy_c = value; }
        }
        public float Fz_c
        {
          get { return m_fz_c; }
          set { m_fz_c = value; }
        }
        /*public short ITotNoPoints
        {
            get { return m_iTotNoPoints; }
            set { m_iTotNoPoints = value; }
        }*/

        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        public CCrSc_3_04()  {   }
        public CCrSc_3_04(float fh, float fb, float ft, float fr_1, float fr_2, float fy_c, float fz_c)
        {
            IsShapeSolid = true;
            m_iNumOfAuxPoints = 3;
            m_iNumOfArcSegment = 8;
            m_iNumOfArcPoints = (short)(m_iNumOfArcSegment + 1); // Each arc is defined by number of segments + 1 point points
            ITotNoPoints = (short)(3 * (short)m_iNumOfArcPoints + 3 + 1);

            m_fh = fh;
            m_fb = fb;
            m_ft = ft;
            m_fr_1 = fr_1;
            m_fr_2 = fr_2;
            m_fy_c = fy_c;
            m_fz_c = fz_c;

            // Create Array - allocate memory
            CrScPointsOut = new float [ITotNoPoints,2];
            // Fill Array Data
            CalcCrSc_Coord_L();

            // Fill list of indices for drawing of surface - triangles edges
            loadCrScIndices();
        }

        //----------------------------------------------------------------------------
        void CalcCrSc_Coord_L()
        {
            // Fill Point Array Data in LCS (Local Coordinate System of Cross-Section, horizontal y, vertical - z)

            // Auxialiary nodes

            //short iNumberAux = 3;

            // Point No. 1
            CrScPointsOut[0, 0] = -m_fy_c;                         // y
            CrScPointsOut[0, 1] = m_fh - m_fz_c- m_fr_2;           // z

            // Point No. 2
            CrScPointsOut[1, 0] = -m_fy_c + m_ft;                  // y
            CrScPointsOut[1, 1] = -m_fz_c + m_ft;                  // z

            // Point No. 3
            CrScPointsOut[2, 0] = m_fb - m_fy_c - m_fr_2; ;        // y
            CrScPointsOut[2, 1] = -m_fz_c;                         // z

            // Surface points

            int iRadiusAngle = 90; // Radius Angle

            // 1st radius - centre "1" (270-360 degrees)
            for (short i = 0; i < m_iNumOfArcPoints; i++)
            {
                CrScPointsOut[m_iNumOfAuxPoints+i, 0] = CrScPointsOut[0, 0] + Geom2D.GetPositionX(m_fr_2, 270 + i * iRadiusAngle / m_iNumOfArcSegment);     // y
                CrScPointsOut[m_iNumOfAuxPoints+i, 1] = CrScPointsOut[0, 1] + Geom2D.GetPositionY_CW(m_fr_2, 270 + i * iRadiusAngle / m_iNumOfArcSegment);  // z
            }

            // 2nd radius - centre "2" (180-270 degrees)
            for (short i = 0; i < m_iNumOfArcPoints; i++)
            {
                CrScPointsOut[m_iNumOfAuxPoints + m_iNumOfArcPoints + i, 0] = CrScPointsOut[1, 0] + m_fr_1 + Geom2D.GetPositionX(m_fr_1, 180 + i * iRadiusAngle / m_iNumOfArcSegment);     // y
                CrScPointsOut[m_iNumOfAuxPoints + m_iNumOfArcPoints + i, 1] = CrScPointsOut[1, 1] + m_fr_1 + Geom2D.GetPositionY_CCW(m_fr_1, 180 + i * iRadiusAngle / m_iNumOfArcSegment); // z
            }

            // 3rd radius - centre "3" (270-360 degrees)
            for (short i = 0; i < m_iNumOfArcPoints; i++)
            {
                CrScPointsOut[m_iNumOfAuxPoints + 2 * m_iNumOfArcPoints + i, 0] = CrScPointsOut[2, 0] + Geom2D.GetPositionX(m_fr_2, 270 + i * iRadiusAngle / m_iNumOfArcSegment);     // y
                CrScPointsOut[m_iNumOfAuxPoints + 2 * m_iNumOfArcPoints + i, 1] = CrScPointsOut[2, 1] + Geom2D.GetPositionY_CW(m_fr_2, 270 + i * iRadiusAngle / m_iNumOfArcSegment); // z
            }

            // Point No.  - Last edge point - bottom left
            CrScPointsOut[m_iNumOfAuxPoints + 3 * m_iNumOfArcPoints, 0] = -m_fy_c;              // y
            CrScPointsOut[m_iNumOfAuxPoints + 3 * m_iNumOfArcPoints, 1] = -m_fz_c;              // z
        }

        protected override void loadCrScIndices()
        {
            CCrSc_3_03 oTemp = new CCrSc_3_03();
            oTemp.load_3_03_04_TriangelIndices(m_iNumOfAuxPoints, m_iNumOfArcSegment);
            TriangleIndices = new Int32Collection();
            TriangleIndices = oTemp.TriangleIndices;
        }

        protected override void loadCrScIndicesFrontSide()
        {
        }

        protected override void loadCrScIndicesShell()
        {
        }

        protected override void loadCrScIndicesBackSide()
        {
        }
	}
}
