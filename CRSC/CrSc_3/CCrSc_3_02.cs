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
    public class CCrSc_3_02 : CSO
    {
        // Rolled monosymmetric U section (channel) - tapered or paralel flanges

        //----------------------------------------------------------------------------
        private short m_sShape;       // Section shape

        // Section shapes types
        // 0 - Four radii, tapered or parallel flanges (6 auxiliary points)
        // 1 - Two radii at flanges tips, tapered or parallel flanges (0 auxiliary points), r1 = 0
        // 2 - Two radii at flanges roots, tapered or parallel flanges (2 auxiliary points), r2 = 0

        private float m_fh;                 // Height / Vyska
        private float m_fb;                 // Width  / Sirka
        private float m_ft_f;               // Flange Thickness / Hrubka pasnice
        private float m_ft_w;               // Web Thickness  / Hrubka steny/stojiny
        private float m_fr_1;               // Radius
        private float m_fr_2;               // Radius - flange edge
        private float m_fd;                 // Web depth - straigth part
        private float m_fy_c;               // Centroid coordinate / Suradnica tažiska / Absolute value
        private float m_fSlopeTaper;        // Slope of Taper
        private short m_iNumOfArcSegment;   // Number of Arc Segments
        private short m_iNumOfArcPoints;    // Number of Arc Points
        private short m_iNumOfAuxPoints;    // Number of Auxialiary Points
        //private short m_iTotNoPoints;       // Total Number of Cross-section Points for Drawing
        //public float[,] CrScPointsOut;        // Array of Points and values in 2D
        //----------------------------------------------------------------------------

        public float Ft_f
        {
            get { return m_ft_f; }
            set { m_ft_f = value; }
        }
        public float Ft_w
        {
            get { return m_ft_w; }
            set { m_ft_w = value; }
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
        public float Fd
        {
            get { return m_fd; }
            set { m_fd = value; }
        }
                public float Fy_c
        {
          get { return m_fy_c; }
          set { m_fy_c = value; }
        }
        /*public short ITotNoPoints
        {
            get { return m_iTotNoPoints; }
            set { m_iTotNoPoints = value; }
        }*/

        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        public CCrSc_3_02()  {   }
        // 0 - Four radii, tapered or parallel flanges (6 auxiliary points)
        public CCrSc_3_02(short sShape, float fh, float fb, float ft_f, float ft_w, float fr_1, float fr_2, float fd, float fy_c)
        {
            IsShapeSolid = true;
            m_iNumOfArcSegment = 8;
            m_iNumOfArcPoints = (short)(m_iNumOfArcSegment + 1); // Each arc is defined by number of segments + 1 point points
            ITotNoPoints = (short)(4 * (short)m_iNumOfArcPoints + 6 + 2);

            m_sShape = sShape;
            m_fh = fh;
            m_fb = fb;
            m_ft_f = ft_f;
            m_ft_w = ft_w;
            m_fr_1 = fr_1;
            m_fr_2 = fr_2;
            m_fd = fd;
            m_fy_c = fy_c;

            //if()
              m_fSlopeTaper = ((m_fh - m_fd - 2*( m_fr_1 + m_fr_2)) / 2.0f) / (m_fb - m_ft_w - m_fr_1 - m_fr_2);
            //else
            //  m_fSlopeTaper = 0.08f; // Default

            // Create Array - allocate memory
            CrScPointsOut = new float [ITotNoPoints,2];
            // Fill Array Data
            // Auxialiary points
            m_iNumOfAuxPoints = 6;
            CalcCrSc_Coord_U_MS_0();

            // Fill list of indices for drawing of surface - triangles edges
            loadCrScIndices();
        }

        public CCrSc_3_02(short sShape, float fh, float fb, float ft_f, float ft_w, float fr, float fd, float fy_c)
        {
            IsShapeSolid = true;
            m_iNumOfArcSegment = 8;
            m_iNumOfArcPoints = (short)(m_iNumOfArcSegment + 1); // Each arc is defined by number of segments + 1 point points

            m_sShape = sShape;
            m_fh = fh;
            m_fb = fb;
            m_ft_f = ft_f;
            m_ft_w = ft_w;
            m_fd = fd;
            m_fy_c = fy_c;
            
            //  m_fSlopeTaper = 0.08f; // Default
            // Number of points per section
            if (m_sShape == 1)       // 1 - Two radii at flanges tips, tapered or parallel flanges (0 auxiliary points), r1 = 0
            {
                m_fr_1 = 0.0f;
                m_fr_2 = fr;
            }
            else if (m_sShape == 2)  // 2 - Two radii at flanges roots, tapered or parallel flanges (2 auxiliary points), r2 = 0
            {
                ITotNoPoints = (short)(2 * (short)m_iNumOfArcPoints + 8 + 2);
                m_fr_1 = fr;
                m_fr_2 = 0.0f;
                m_fSlopeTaper = (2*((m_fh - m_fd - 2 * m_fr_1) / 2.0f - m_ft_f)) / (m_fb - m_ft_w - m_fr_1);
            }
            else // Exception
            { }

            // Create Array - allocate memory
            CrScPointsOut = new float[ITotNoPoints, 2];
            // Fill Array Data

            if (m_sShape == 1)       // 1 - Two radii at flanges tips, tapered or parallel flanges (0 auxiliary points), r1 = 0
            {
                // Auxialiary points
                m_iNumOfAuxPoints = 0;
                CalcCrSc_Coord_U_MS_1();
            }
            else if (m_sShape == 2)  // 2 - Two radii at flanges roots, tapered or parallel flanges (2 auxiliary points), r2 = 0
            {
                // Auxialiary points
                m_iNumOfAuxPoints = 2;
                CalcCrSc_Coord_U_MS_2();
            }
            else // Exception
            { }

            // Fill list of indices for drawing of surface - triangles edges
            loadCrScIndices();
        }

        //----------------------------------------------------------------------------
        void CalcCrSc_Coord_U_MS_0()
        {
            // Fill Point Array Data in LCS (Local Coordinate System of Cross-Section, horizontal y, vertical - z)

            // Auxialiary nodes
            //short iNumberAux = 6;

            // Point No. 1
            CrScPointsOut[0, 0] = -m_fy_c + m_ft_w + m_fr_1;       // y
            CrScPointsOut[0, 1] = m_fh / 2f;                       // z

            // Point No. 2
            CrScPointsOut[1, 0] = - m_fy_c + m_fb- m_fr_2;         // y
            CrScPointsOut[1, 1] = CrScPointsOut[0, 1];               // z

            // Point No. 3
            CrScPointsOut[2, 0] = -m_fy_c + m_ft_w;                                                                // y
            CrScPointsOut[2, 1] = CrScPointsOut[0, 1]  - m_fr_2 - ((m_fh - m_fd - 2 * (m_fr_1 + m_fr_2)) / 2.0f);    // z

            // Point No. 4
            CrScPointsOut[3, 0] = CrScPointsOut[2, 0];               // y
            CrScPointsOut[3, 1] = -CrScPointsOut[2, 1];              // z

            // Point No. 5
            CrScPointsOut[4, 0] = CrScPointsOut[1, 0];               // y
            CrScPointsOut[4, 1] = -CrScPointsOut[1, 1];              // z

            // Point No. 6
            CrScPointsOut[5,0] = CrScPointsOut[0,0];                 // y
            CrScPointsOut[5,1] = -CrScPointsOut[0,1];                // z

            // Surface points

            // Point No. 7 - 1st Edge point - upper left
            CrScPointsOut[m_iNumOfAuxPoints, 0] = -m_fy_c;                // y
            CrScPointsOut[m_iNumOfAuxPoints, 1] = m_fh / 2.0f;            // z


            int iRadiusAngle = 90; // Radius Angle

            // 1st radius - centre "1" (0-90 degrees)
            for (short i = 0; i < m_iNumOfArcPoints; i++)
            {
                CrScPointsOut[m_iNumOfAuxPoints + i + 1, 0] = CrScPointsOut[1, 0] + Geom2D.GetPositionX(m_fr_2, 0 + i * iRadiusAngle / m_iNumOfArcSegment);     // y
                CrScPointsOut[m_iNumOfAuxPoints + i + 1, 1] = CrScPointsOut[1, 1] + Geom2D.GetPositionY_CW(m_fr_2, 0 + i * iRadiusAngle / m_iNumOfArcSegment);  // z
            }

            // 2nd radius - centre "2" (90-180 degrees)
            for (short i = 0; i < m_iNumOfArcPoints; i++)
            {
                CrScPointsOut[m_iNumOfAuxPoints + m_iNumOfArcPoints + i + 1, 0] = CrScPointsOut[2, 0] + m_fr_1 + Geom2D.GetPositionX(m_fr_1, 90 + i * iRadiusAngle / m_iNumOfArcSegment);     // y
                CrScPointsOut[m_iNumOfAuxPoints + m_iNumOfArcPoints + i + 1, 1] = CrScPointsOut[2, 1] - m_fr_1 + Geom2D.GetPositionY_CCW(m_fr_1, 90 + i * iRadiusAngle / m_iNumOfArcSegment); // z
            }

            // 3rd radius - centre "3" (180-270 degrees)
            for (short i = 0; i < m_iNumOfArcPoints; i++)
            {
                CrScPointsOut[m_iNumOfAuxPoints + 2 * m_iNumOfArcPoints + i + 1, 0] = CrScPointsOut[3, 0] + m_fr_1 + Geom2D.GetPositionX(m_fr_1, 180 + i * iRadiusAngle / m_iNumOfArcSegment);     // y
                CrScPointsOut[m_iNumOfAuxPoints + 2 * m_iNumOfArcPoints + i + 1, 1] = CrScPointsOut[3, 1] + m_fr_1 + Geom2D.GetPositionY_CCW(m_fr_1, 180 + i * iRadiusAngle / m_iNumOfArcSegment); // z
            }

            // 4th radius - centre "4" (270-360 degrees)
            for (short i = 0; i < m_iNumOfArcPoints; i++)
            {
                CrScPointsOut[m_iNumOfAuxPoints + 3 * m_iNumOfArcPoints + i + 1, 0] = CrScPointsOut[4, 0] + Geom2D.GetPositionX(m_fr_2, 270 + i * iRadiusAngle / m_iNumOfArcSegment);     // y
                CrScPointsOut[m_iNumOfAuxPoints + 3 * m_iNumOfArcPoints + i + 1, 1] = CrScPointsOut[4, 1] + Geom2D.GetPositionY_CW(m_fr_2, 270 + i * iRadiusAngle / m_iNumOfArcSegment);  // z
            }

            // Point No.  - Last edge point - bottom left
            CrScPointsOut[m_iNumOfAuxPoints + 2+4 * m_iNumOfArcPoints -1, 0] = -m_fy_c;                  // y
            CrScPointsOut[m_iNumOfAuxPoints + 2+4 * m_iNumOfArcPoints -1, 1] = -m_fh / 2.0f;             // z
        }

        void CalcCrSc_Coord_U_MS_1()
        {
            // Fill Point Array Data in LCS (Local Coordinate System of Cross-Section, horizontal y, vertical - z)

            // Auxialiary nodes

            //short iNumberAux = 0;
        }

        void CalcCrSc_Coord_U_MS_2()
        {
            // Fill Point Array Data in LCS (Local Coordinate System of Cross-Section, horizontal y, vertical - z)

            // Auxialiary nodes
            //short iNumberAux = 2;

            // Point No. 1
            CrScPointsOut[0, 0] = -m_fy_c + m_ft_w;                 // y                                                             // y
            CrScPointsOut[0, 1] = (m_fd + 2 * m_fr_1) / 2.0f;       // z

            // Point No. 2
            CrScPointsOut[1, 0] = CrScPointsOut[0, 0];                // y
            CrScPointsOut[1, 1] = -CrScPointsOut[0, 1];               // z

            // Surface points

            // Point No. 3
            CrScPointsOut[2, 0] = -m_fy_c;                          // y
            CrScPointsOut[2, 1] = m_fh / 2.0f;                      // z

            // Point No. 4
            CrScPointsOut[3, 0] = -m_fy_c + m_ft_w + m_fr_1;        // y
            CrScPointsOut[3, 1] = CrScPointsOut[2, 1];                // z

            // Point No. 5
            CrScPointsOut[4, 0] = -m_fy_c + m_fb;                    // y
            CrScPointsOut[4, 1] = CrScPointsOut[2, 1];                 // z

            // Point No. 6
            CrScPointsOut[5, 0] = CrScPointsOut[4, 0];                                             // y
            CrScPointsOut[5, 1] = CrScPointsOut[0, 1] + m_fSlopeTaper * (m_fb - m_ft_w - m_fr_1);  // z

            int iRadiusAngle = 90; // Radius Angle

            // 1st radius - centre "0" (90-180 degrees)
            for (short i = 0; i < m_iNumOfArcPoints; i++)
            {
                CrScPointsOut[m_iNumOfAuxPoints + i + 4, 0] = CrScPointsOut[0, 0] + m_fr_1 + Geom2D.GetPositionX(m_fr_1, 90 + i * iRadiusAngle / m_iNumOfArcSegment);     // y
                CrScPointsOut[m_iNumOfAuxPoints + i + 4, 1] = CrScPointsOut[0, 1] - m_fr_1 + Geom2D.GetPositionY_CCW(m_fr_1, 90 + i * iRadiusAngle / m_iNumOfArcSegment); // z
            }

            // 2nd radius - centre "1" (180-270 degrees)
            for (short i = 0; i < m_iNumOfArcPoints; i++)
            {
                CrScPointsOut[m_iNumOfAuxPoints + m_iNumOfArcPoints + i + 4, 0] = CrScPointsOut[1, 0] + m_fr_1 + Geom2D.GetPositionX(m_fr_1, 180 + i * iRadiusAngle / m_iNumOfArcSegment);     // y
                CrScPointsOut[m_iNumOfAuxPoints + m_iNumOfArcPoints + i + 4, 1] = CrScPointsOut[1, 1] + m_fr_1 + Geom2D.GetPositionY_CCW(m_fr_1, 180 + i * iRadiusAngle / m_iNumOfArcSegment); // z
            }

            // Point No. XX
            CrScPointsOut[m_iNumOfAuxPoints + 2 * m_iNumOfArcPoints + 4, 0] = CrScPointsOut[5, 0];              // y
            CrScPointsOut[m_iNumOfAuxPoints + 2 * m_iNumOfArcPoints + 4, 1] = -CrScPointsOut[5, 1];             // z

            // Point No. XX
            CrScPointsOut[m_iNumOfAuxPoints + 2 * m_iNumOfArcPoints + 5, 0] = CrScPointsOut[4, 0];              // y
            CrScPointsOut[m_iNumOfAuxPoints + 2 * m_iNumOfArcPoints + 5, 1] = -CrScPointsOut[4, 1];             // z

            // Point No. XX
            CrScPointsOut[m_iNumOfAuxPoints + 2 * m_iNumOfArcPoints + 6, 0] = CrScPointsOut[3, 0];              // y
            CrScPointsOut[m_iNumOfAuxPoints + 2 * m_iNumOfArcPoints + 6, 1] = -CrScPointsOut[3, 1];             // z

            // Point No. XX  - Last edge point - bottom left
            CrScPointsOut[m_iNumOfAuxPoints + 2 * m_iNumOfArcPoints + 7, 0] = CrScPointsOut[2, 0];              // y
            CrScPointsOut[m_iNumOfAuxPoints + 2 * m_iNumOfArcPoints + 7, 1] = -CrScPointsOut[2, 1];             // z
        }

        protected override void loadCrScIndices()
        {
            load_3_02_TriangelIndices(m_sShape, m_iNumOfAuxPoints, m_iNumOfArcSegment);
        }

        private void load_3_02_TriangelIndices(short sShape, int iAux, int iRadiusSegment)
        {
            // const int secNum = iAux + iRadiusPoints * 4;  // Number of points in section (2D)
            int iRadiusPoints = iRadiusSegment + 1;

            TriangleIndices = new Int32Collection(60);

            if (sShape == 0)
            {
                // Front Side / Forehead
                // Points order 1,2,3,4

                AddRectangleIndices_CW_1234(TriangleIndices, 0, 1, iAux + 1 + iRadiusSegment, iAux + 2 + iRadiusSegment);
                AddRectangleIndices_CW_1234(TriangleIndices, 0, iAux + 2 + iRadiusSegment, 2, 6);

                AddRectangleIndices_CW_1234(TriangleIndices, 2, 3, iAux + 2 + 4 * iRadiusPoints - 1, 6);

                AddRectangleIndices_CW_1234(TriangleIndices, 3, iAux + 3 * iRadiusPoints, 5, iAux + 1 + 4 * iRadiusPoints);
                AddRectangleIndices_CW_1234(TriangleIndices, iAux + 3 * iRadiusPoints, iAux + 3 * iRadiusPoints + 1, 4, 5);

                // Arc sectors
                // 1st SolidCircleSector
                AddSolidCircleSectorIndices(1, iAux + 1, iRadiusSegment, TriangleIndices, false);
                // 2nd SolidCircleSector
                AddSolidCircleSectorIndices(2, iAux + 1 + iRadiusPoints, iRadiusSegment, TriangleIndices, false);
                // 3rd SolidCircleSector
                AddSolidCircleSectorIndices(3, iAux + 1 + 2 * iRadiusPoints, iRadiusSegment, TriangleIndices, false);
                // 4th SolidCircleSector
                AddSolidCircleSectorIndices(4, iAux + 1 + 3 * iRadiusPoints, iRadiusSegment, TriangleIndices, false);

                // Back Side
                // Points order 1,4,3,2

                int iPointNumbersOffset = iAux + 2 + 4 * iRadiusPoints; // Number of nodes per section - Nodes offset

                AddRectangleIndices_CW_1234(TriangleIndices, iPointNumbersOffset + 0, iPointNumbersOffset + iAux + 2 + iRadiusSegment, iPointNumbersOffset + iAux + 1 + iRadiusSegment, iPointNumbersOffset + 1);
                AddRectangleIndices_CW_1234(TriangleIndices, iPointNumbersOffset + 0, iPointNumbersOffset + 6, iPointNumbersOffset + 2, iPointNumbersOffset + iAux + 2 + iRadiusSegment);

                AddRectangleIndices_CW_1234(TriangleIndices, iPointNumbersOffset + 2, iPointNumbersOffset + 6, iPointNumbersOffset + iAux + 2 + 4 * iRadiusPoints - 1, iPointNumbersOffset + 3);

                AddRectangleIndices_CW_1234(TriangleIndices, iPointNumbersOffset + 3, iPointNumbersOffset + iAux + 1 + 4 * iRadiusPoints, iPointNumbersOffset + 5, iPointNumbersOffset + iAux + 3 * iRadiusPoints);
                AddRectangleIndices_CW_1234(TriangleIndices, iPointNumbersOffset + iAux + 3 * iRadiusPoints, iPointNumbersOffset + 5, iPointNumbersOffset + 4, iPointNumbersOffset + iAux + 3 * iRadiusPoints + 1);

                // Arc sectors
                // 1st SolidCircleSector
                AddSolidCircleSectorIndices(iPointNumbersOffset + 1, iPointNumbersOffset + iAux + 1, iRadiusSegment, TriangleIndices, true);
                // 2nd SolidCircleSector
                AddSolidCircleSectorIndices(iPointNumbersOffset + 2, iPointNumbersOffset + iAux + 1 + iRadiusPoints, iRadiusSegment, TriangleIndices, true);
                // 3rd SolidCircleSector
                AddSolidCircleSectorIndices(iPointNumbersOffset + 3, iPointNumbersOffset + iAux + 1 + 2 * iRadiusPoints, iRadiusSegment, TriangleIndices, true);
                // 4th SolidCircleSector
                AddSolidCircleSectorIndices(iPointNumbersOffset + 4, iPointNumbersOffset + iAux + 1 + 3 * iRadiusPoints, iRadiusSegment, TriangleIndices, true);

                // Shell
                DrawCaraLaterals(iAux, 2 + 4 * iRadiusPoints, TriangleIndices);
            }
            else if (sShape == 1)
            {
                // Not implemented
            }
            else if (sShape == 2)
            {
                // Front Side / Forehead
                // Points order 1,2,3,4

                AddRectangleIndices_CW_1234(TriangleIndices, 0, 2, 3, 6);
                AddRectangleIndices_CW_1234(TriangleIndices, 3, 4, 5, 6);

                AddRectangleIndices_CW_1234(TriangleIndices, 0, 1, iAux + 7 + 2 * iRadiusPoints, 2);

                AddRectangleIndices_CW_1234(TriangleIndices, 1, iAux + 2 * iRadiusPoints + 3, iAux + 2 * iRadiusPoints + 6, iAux + 2 * iRadiusPoints + 7);
                AddRectangleIndices_CW_1234(TriangleIndices, iAux + 2 * iRadiusPoints + 3, iAux + 2 * iRadiusPoints + 4, iAux + 2 * iRadiusPoints + 5, iAux + 2 * iRadiusPoints + 6);

                // Arc sectors
                // 1st SolidCircleSector
                AddSolidCircleSectorIndices(0, iAux + 4, iRadiusSegment, TriangleIndices, false);
                // 2nd SolidCircleSector
                AddSolidCircleSectorIndices(1, iAux + 4 + iRadiusPoints, iRadiusSegment, TriangleIndices, false);

                // Back Side
                // Points order 1,4,3,2

                int iPointNumbersOffset = iAux + 8 + 2 * iRadiusPoints; // Number of nodes per section - Nodes offset

                AddRectangleIndices_CCW_1234(TriangleIndices, iPointNumbersOffset + 0, iPointNumbersOffset + 2, iPointNumbersOffset + 3, iPointNumbersOffset + 6);
                AddRectangleIndices_CCW_1234(TriangleIndices, iPointNumbersOffset + 3, iPointNumbersOffset + 4, iPointNumbersOffset + 5, iPointNumbersOffset + 6);

                AddRectangleIndices_CCW_1234(TriangleIndices, iPointNumbersOffset + 0, iPointNumbersOffset + 1, iPointNumbersOffset + iAux + 7 + 2 * iRadiusPoints, iPointNumbersOffset + 2);

                AddRectangleIndices_CCW_1234(TriangleIndices, iPointNumbersOffset + 1, iPointNumbersOffset + iAux + 2 * iRadiusPoints + 3, iPointNumbersOffset + iAux + 2 * iRadiusPoints + 6, iPointNumbersOffset + iAux + 2 * iRadiusPoints + 7);
                AddRectangleIndices_CCW_1234(TriangleIndices, iPointNumbersOffset + iAux + 2 * iRadiusPoints + 3, iPointNumbersOffset + iAux + 2 * iRadiusPoints + 4, iPointNumbersOffset + iAux + 2 * iRadiusPoints + 5, iPointNumbersOffset + iAux + 2 * iRadiusPoints + 6);

                // Arc sectors
                // 1st SolidCircleSector
                AddSolidCircleSectorIndices(iPointNumbersOffset + 0, iPointNumbersOffset + iAux + 4, iRadiusSegment, TriangleIndices, true);
                // 2nd SolidCircleSector
                AddSolidCircleSectorIndices(iPointNumbersOffset + 1, iPointNumbersOffset + iAux + 4 + iRadiusPoints, iRadiusSegment, TriangleIndices, true);

                // Shell
                DrawCaraLaterals(iAux, 8 + 2 * iRadiusPoints, TriangleIndices);
            }
            else // Exception
            {

            }
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
