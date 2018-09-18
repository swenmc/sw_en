using MATH;
using System.Collections.Generic;
using System.Windows;

namespace CRSC
{
    public class CCrSc_3_01 : CSO
    {
        // Rolled mono-symmetric I section - parallel or tapered flanges

        //----------------------------------------------------------------------------
        private short m_sShape;       // Section shape

        // Section shapes types
        // 0 - Eight radii, tapered or parallel flanges (12 auxiliary points)
        // 1 - Four radii at flanges tips, tapered or parallel flanges (8 auxiliary points), r1 = 0
        // 2 - Four radii at flanges roots, tapered or parallel flanges (4 auxiliary points), r2 = 0

        private float m_fh;                 // Height / Vyska
        private float m_fb_1;               // Width  / Sirka
        private float m_fb_2;               // Width  / Sirka
        private float m_ft_f_1;             // Flange Thickness / Hrubka pasnice
        private float m_ft_f_2;             // Flange Thickness / Hrubka pasnice
        private float m_ft_w;               // Web Thickness  / Hrubka steny/stojiny
        private float m_fr_1;               // Radius
        private float m_fr_2;               // Radius - flange edge
        private float m_fd;                 // Web depth - straigth part
        private float m_fz_c;               // Centroid coordinate / Suradnica tažiska / Absolute value
        private float m_fSlopeTaper_1;      // Slope of Taper
        private float m_fSlopeTaper_2;      // Slope of Taper
        private short m_iNumOfArcSegment;   // Number of Arc Segments
        private short m_iNumOfArcPoints;    // Number of Arc Points
        //private short m_iTotNoPoints;       // Total Number of Cross-section Points for Drawing
        //public float[,] m_CrScPoint;        // Array of Points and values in 2D
        //----------------------------------------------------------------------------

        public float Fh
        {
            get { return m_fh; }
            set { m_fh = value; }
        }
        public float Fb_1
        {
            get { return m_fb_1; }
            set { m_fb_1 = value; }
        }
        public float Fb_2
        {
            get { return m_fb_2; }
            set { m_fb_2 = value; }
        }
        public float Ft_f_1
        {
            get { return m_ft_f_1; }
            set { m_ft_f_1 = value; }
        }
        public float Ft_f_2
        {
            get { return m_ft_f_2; }
            set { m_ft_f_2 = value; }
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
        public CCrSc_3_01()  {   }
        public CCrSc_3_01(short sShape, float fh, float fb_1, float fb_2, float ft_f_1, float ft_f_2, float ft_w, float fr_1, float fr_2, float fd)
        {
            IsShapeSolid = true;
            m_sShape = sShape;
            m_iNumOfArcSegment = 8;
            m_iNumOfArcPoints = (short)(m_iNumOfArcSegment + 1); // Each arc is defined by number of segments + 1 point points
            ITotNoPoints = (short)(8 * (short)m_iNumOfArcPoints + (8 + 4));

            m_fh = fh;
            m_fb_1 = fb_1;
            m_ft_f_1 = ft_f_1;
            m_fb_2 = fb_2;
            m_ft_f_2 = ft_f_2;
            m_ft_w = ft_w;
            m_fr_1 = fr_1;
            m_fr_2 = fr_2;
            m_fd = fd;

            m_fSlopeTaper_1 = (m_fh - m_fd - 2*( m_fr_1 + m_fr_2) / 2.0f) / ((m_fb_1 - m_ft_w - 2*(m_fr_1 + m_fr_2)) / 2.0f);
            m_fSlopeTaper_2 = (m_fh - m_fd - 2 * (m_fr_1 + m_fr_2) / 2.0f) / ((m_fb_2 - m_ft_w - 2 * (m_fr_1 + m_fr_2)) / 2.0f);

            //  m_fSlopeTaper = 0.08f; // Default

            // Create Array - allocate memory
            //CrScPointsOut = new float [ITotNoPoints,2];
            CrScPointsOut = new List<Point>(ITotNoPoints);
            // Fill Array Data
            CalcCrSc_Coord_I_MS_0();

            // Fill list of indices for drawing of surface - triangles edges
            loadCrScIndices();
        }

        public CCrSc_3_01(short sShape, float fh, float fb_1, float fb_2, float ft_f_1, float ft_f_2, float ft_w, float fr, float fz_c)
        {
            IsShapeSolid = true;
            m_sShape = sShape;

            m_iNumOfArcSegment = 4;
            m_iNumOfArcPoints = (short)(m_iNumOfArcSegment + 1); // Each arc is defined by number of segments + 1 point points

            m_fh = fh;
            m_fb_1 = fb_1;
            m_ft_f_1 = ft_f_1;
            m_fb_2 = fb_2;
            m_ft_f_2 = ft_f_2;
            m_ft_w = ft_w;
            m_fz_c = fz_c;

            // Number of points per section
            // 1 - Four radii at flanges tips, tapered or parallel flanges (8 auxiliary points)

            ITotNoPoints = (short)(4 * (short)m_iNumOfArcPoints + (4 + 4 + 4));
            m_fr_1 = 0.0f;
            m_fr_2 = fr;
            m_fSlopeTaper_1 = (2.0f * (m_ft_f_1 - m_fr_2)) / ((m_fb_1 - m_ft_w - 2 * m_fr_2) / 2.0f);
            m_fSlopeTaper_2 = (2.0f * (m_ft_f_2 - m_fr_2)) / ((m_fb_2 - m_ft_w - 2 * m_fr_2) / 2.0f);
            m_fd = m_fh - 2.0f * m_fr_2 - 2.0f * (m_ft_f_1 - m_fr_2) - 2.0f * (m_ft_f_2 - m_fr_2); 

            //  m_fSlopeTaper = 0.08f; // Default

            // Create Array - allocate memory
            //CrScPointsOut = new float[ITotNoPoints, 2];
            // Fill Array Data

            // 1 - Four radii at flanges tips, tapered or parallel flanges (8 auxiliary points)
            CalcCrSc_Coord_I_MS_1();

            // Fill list of indices for drawing of surface - triangles edges
            loadCrScIndices();
        }

        public CCrSc_3_01(short sShape, float fh, float fb_1, float fb_2, float ft_f_1, float ft_f_1_tip, float ft_f_2, float ft_f_2_tip, float ft_w, float fr, float fz_c)
        {
            IsShapeSolid = true;
            m_sShape = sShape;

            m_iNumOfArcSegment = 4;
            m_iNumOfArcPoints = (short)(m_iNumOfArcSegment + 1); // Each arc is defined by number of segments + 1 point points

            m_fh     = fh;
            m_fb_1   = fb_1;
            m_ft_f_1 = ft_f_1;
            m_fb_2   = fb_2;
            m_ft_f_2 = ft_f_2;
            m_ft_w   = ft_w;
            m_fz_c   = fz_c;

            // Number of points per section
            // 2 - Four radii at flanges roots, tapered or parallel flanges (4 auxiliary points)

            ITotNoPoints = (short)(4 * (short)m_iNumOfArcPoints + (4 + 6 + 6));
            m_fr_1 = fr;
            m_fr_2 = 0.0f;
            m_fSlopeTaper_1 = (2.0f * (m_ft_f_1 - ft_f_1_tip)) / ((m_fb_1 - m_ft_w - 2 * m_fr_1) / 2.0f);
            m_fSlopeTaper_2 = (2.0f * (m_ft_f_2 - ft_f_2_tip)) / ((m_fb_2 - m_ft_w - 2 * m_fr_1) / 2.0f);
            m_fd = m_fh - 2.0f * m_fr_1 - 2.0f * (m_ft_f_1 - ft_f_1_tip) - ft_f_1_tip - 2.0f * (m_ft_f_2 - ft_f_2_tip) - ft_f_2_tip; 

            // Create Array - allocate memory
			//CrScPointsOut= new float[ITotNoPoints, 2];
            CrScPointsOut = new List<Point>(ITotNoPoints);
            // Fill Array Data

            // 2 - Four radii at flanges roots, tapered or parallel flanges (4 auxiliary points)
            CalcCrSc_Coord_I_MS_2(ft_f_1_tip);

            // Fill list of indices for drawing of surface - triangles edges
            loadCrScIndices();
        }

        //----------------------------------------------------------------------------
        //void CalcCrSc_Coord_I_MS_0() // 0 - Eight radii, tapered or parallel flanges (12 auxiliary points)
        //{
        //    // Fill Point Array Data in LCS (Local Coordinate System of Cross-Section, horizontal y, vertical - z)

        //    // Auxialiary nodes

        //    short iNumberAux = 8 + 4;

        //    // Point No. 1
        //    CrScPointsOut[0,0] = -m_fb_1 / 2f + m_fr_2;    // y
        //    CrScPointsOut[0,1] = m_fh / 2f;              // z

        //    // Point No. 2
        //    CrScPointsOut[1, 0] = -m_ft_w / 2.0f - m_fr_1;   // y
        //    CrScPointsOut[1,1] = CrScPointsOut[0,1];           // z

        //    // Point No. 3
        //    CrScPointsOut[2,0] = -CrScPointsOut[1,0];      // y
        //    CrScPointsOut[2,1] = CrScPointsOut[0,1];       // z

        //    // Point No. 4
        //    CrScPointsOut[3,0] = -CrScPointsOut[0,0];      // y
        //    CrScPointsOut[3,1] = CrScPointsOut[0,1];       // z

        //    // Point No. 5
        //    CrScPointsOut[4,0] = m_ft_w / 2.0f;         // y
        //    CrScPointsOut[4,1] = CrScPointsOut[3, 1] - m_fr_2 - ((m_fh - m_fd - 2 * (m_fr_1 + m_fr_2)) / 2.0f);     // z

        //    // Point No. 6
        //    CrScPointsOut[5,0] = CrScPointsOut[4,0];      // y
        //    CrScPointsOut[5,1] = -CrScPointsOut[4,1];     // z

        //    // Point No. 7
        //    CrScPointsOut[6, 0] = m_fb_2 / 2f - m_fr_2;    // y
        //    CrScPointsOut[6,1] = -CrScPointsOut[3,1];     // z

        //    // Point No. 8
        //    CrScPointsOut[7,0] = CrScPointsOut[2,0];      // y
        //    CrScPointsOut[7,1] = -CrScPointsOut[2,1];     // z

        //    // Point No. 9
        //    CrScPointsOut[8,0] = CrScPointsOut[1,0];      // y
        //    CrScPointsOut[8,1] = -CrScPointsOut[1,1];     // z

        //    // Point No. 10
        //    CrScPointsOut[9,0] = -CrScPointsOut[6,0];      // y
        //    CrScPointsOut[9,1] = -CrScPointsOut[0,1];     // z

        //    // Point No. 11
        //    CrScPointsOut[10,0] = -CrScPointsOut[5,0];    // y
        //    CrScPointsOut[10,1] = CrScPointsOut[5,1];     // z

        //    // Point No. 12
        //    CrScPointsOut[11, 0] = -CrScPointsOut[4, 0];  // y
        //    CrScPointsOut[11, 1] = CrScPointsOut[4, 1];   // z

        //    // Surface points

        //    // Point No. 13 - 1st Edge point - upper left
        //    CrScPointsOut[iNumberAux, 0] = -m_fb_1 / 2.0f;     // y
        //    CrScPointsOut[iNumberAux, 1] = m_fh / 2.0f;      // z

        //    int iRadiusAngle = 90; // Radius Angle

        //    // 2nd radius - centre "3" (0-90 degrees)
        //    for (short i = 0; i < m_iNumOfArcPoints; i++)
        //    {
        //        CrScPointsOut[iNumberAux + i + 1, 0] = CrScPointsOut[3, 0] + Geom2D.GetPositionX_deg(m_fr_2, 0 + i * iRadiusAngle / m_iNumOfArcSegment);     // y
        //        CrScPointsOut[iNumberAux + i + 1, 1] = CrScPointsOut[3, 1] + Geom2D.GetPositionY_CW_deg(m_fr_2, 0 + i * iRadiusAngle / m_iNumOfArcSegment);  // z
        //    }

        //    // 3rd radius - centre "4" (90-180 degrees)
        //    for (short i = 0; i < m_iNumOfArcPoints; i++)
        //    {
        //        CrScPointsOut[iNumberAux + m_iNumOfArcPoints + i + 1, 0] = CrScPointsOut[4, 0] + m_fr_1 + Geom2D.GetPositionX_deg(m_fr_1, 90 + i * iRadiusAngle / m_iNumOfArcSegment);     // y
        //        CrScPointsOut[iNumberAux + m_iNumOfArcPoints + i + 1, 1] = CrScPointsOut[4, 1] - m_fr_1 + Geom2D.GetPositionY_CCW_deg(m_fr_1, 90 + i * iRadiusAngle / m_iNumOfArcSegment); // z
        //    }

        //    // 4th radius - centre "5" (180-270 degrees)
        //    for (short i = 0; i < m_iNumOfArcPoints; i++)
        //    {
        //        CrScPointsOut[iNumberAux + 2 * m_iNumOfArcPoints + i + 1, 0] = CrScPointsOut[5, 0] + m_fr_1 + Geom2D.GetPositionX_deg(m_fr_1, 180 + i * iRadiusAngle / m_iNumOfArcSegment);     // y
        //        CrScPointsOut[iNumberAux + 2 * m_iNumOfArcPoints + i + 1, 1] = CrScPointsOut[5, 1] + m_fr_1 + Geom2D.GetPositionY_CCW_deg(m_fr_1, 180 + i * iRadiusAngle / m_iNumOfArcSegment); // z
        //    }

        //    // 5th radius - centre "6" (270-360 degrees)
        //    for (short i = 0; i < m_iNumOfArcPoints; i++)
        //    {
        //        CrScPointsOut[iNumberAux + 3 * m_iNumOfArcPoints + i + 1, 0] = CrScPointsOut[6, 0] + Geom2D.GetPositionX_deg(m_fr_2, 270 + i * iRadiusAngle / m_iNumOfArcSegment);     // y
        //        CrScPointsOut[iNumberAux + 3 * m_iNumOfArcPoints + i + 1, 1] = CrScPointsOut[6, 1] + Geom2D.GetPositionY_CW_deg(m_fr_2, 270 + i * iRadiusAngle / m_iNumOfArcSegment);  // z
        //    }

        //    // 6th radius - centre "9" (180-270 degrees)
        //    for (short i = 0; i < m_iNumOfArcPoints; i++)
        //    {
        //        CrScPointsOut[iNumberAux + 4 * m_iNumOfArcPoints + i + 1, 0] = CrScPointsOut[9, 0] + Geom2D.GetPositionX_deg(m_fr_2, 180 + i * iRadiusAngle / m_iNumOfArcSegment);     // y
        //        CrScPointsOut[iNumberAux + 4 * m_iNumOfArcPoints + i + 1, 1] = CrScPointsOut[9, 1] + Geom2D.GetPositionY_CW_deg(m_fr_2, 180 + i * iRadiusAngle / m_iNumOfArcSegment);  // z
        //    }

        //    // 7th radius - centre "10" (270-360 degrees)
        //    for (short i = 0; i < m_iNumOfArcPoints; i++)
        //    {
        //        CrScPointsOut[iNumberAux + 5 * m_iNumOfArcPoints + i + 1, 0] = CrScPointsOut[10, 0] - m_fr_1 + Geom2D.GetPositionX_deg(m_fr_1, 270 + i * iRadiusAngle / m_iNumOfArcSegment);     // y
        //        CrScPointsOut[iNumberAux + 5 * m_iNumOfArcPoints + i + 1, 1] = CrScPointsOut[10, 1] + m_fr_1 + Geom2D.GetPositionY_CCW_deg(m_fr_1, 270 + i * iRadiusAngle / m_iNumOfArcSegment); // z
        //    }

        //    // 8th radius - centre "11" (0-90 degrees)
        //    for (short i = 0; i < m_iNumOfArcPoints; i++)
        //    {
        //        CrScPointsOut[iNumberAux + 6 * m_iNumOfArcPoints + i + 1, 0] = CrScPointsOut[11, 0] - m_fr_1 + Geom2D.GetPositionX_deg(m_fr_1, 0 + i * iRadiusAngle / m_iNumOfArcSegment);     // y
        //        CrScPointsOut[iNumberAux + 6 * m_iNumOfArcPoints + i + 1, 1] = CrScPointsOut[11, 1] - m_fr_1 + Geom2D.GetPositionY_CCW_deg(m_fr_1, 0 + i * iRadiusAngle / m_iNumOfArcSegment); // z
        //    }

        //    // 1st radius - centre "0" (90-180 degrees)
        //    // Do not create last point of segment - it is already defined
        //    for (short i = 0; i < m_iNumOfArcPoints - 1; i++)
        //    {
        //        CrScPointsOut[iNumberAux + 7 * m_iNumOfArcPoints + i + 1, 0] = CrScPointsOut[0, 0] + Geom2D.GetPositionX_deg(m_fr_2, 90 + i * iRadiusAngle / m_iNumOfArcSegment);      // y
        //        CrScPointsOut[iNumberAux + 7 * m_iNumOfArcPoints + i + 1, 1] = CrScPointsOut[0, 1] + Geom2D.GetPositionY_CW_deg(m_fr_2, 90 + i * iRadiusAngle / m_iNumOfArcSegment);    // z
        //    }
        //}

        ////----------------------------------------------------------------------------
        //void CalcCrSc_Coord_I_MS_1() // 1 - Four radii at flanges tips, tapered or parallel flanges (8 auxiliary points)
        //{
        //    // Fill Point Array Data in LCS (Local Coordinate System of Cross-Section, horizontal y, vertical - z)

        //    // Auxialiary nodes

        //    short iNumberAux = 4 + 4;

        //    // Point No. 1
        //    CrScPointsOut[0, 0] = -m_fb_1 / 2.0f + m_fr_2;    // y
        //    CrScPointsOut[0, 1] = m_fh - m_fz_c;       // z

        //    // Point No. 2
        //    CrScPointsOut[1, 0] = -m_ft_w / 2.0f;        // y
        //    CrScPointsOut[1, 1] = CrScPointsOut[0, 1];     // z

        //    // Point No. 3
        //    CrScPointsOut[2, 0] = -CrScPointsOut[1, 0];    // y
        //    CrScPointsOut[2, 1] = CrScPointsOut[0, 1];     // z

        //    // Point No. 4
        //    CrScPointsOut[3, 0] = -CrScPointsOut[0, 0];    // y
        //    CrScPointsOut[3, 1] = CrScPointsOut[0, 1];     // z

        //    // Point No. 5
        //    CrScPointsOut[4, 0] = m_fb_2 / 2.0f - m_fr_2;// y
        //    CrScPointsOut[4, 1] = -m_fz_c;               // z

        //    // Point No. 6
        //    CrScPointsOut[5, 0] = CrScPointsOut[2, 0];     // y
        //    CrScPointsOut[5, 1] = CrScPointsOut[4, 1];     // z

        //    // Point No. 7
        //    CrScPointsOut[6, 0] = CrScPointsOut[1, 0];     // y
        //    CrScPointsOut[6, 1] = CrScPointsOut[4, 1];     // z

        //    // Point No. 8
        //    CrScPointsOut[7, 0] = -CrScPointsOut[4, 0];    // y
        //    CrScPointsOut[7, 1] = CrScPointsOut[4, 1];     // z

        //    // Surface points

        //    // Point No. 8 - 1st Edge point - upper left
        //    CrScPointsOut[iNumberAux, 0] = -m_fb_1 / 2.0f;     // y
        //    CrScPointsOut[iNumberAux, 1] = CrScPointsOut[0, 1];  // z

        //    int iRadiusAngle = 90; // Radius Angle

        //    // 2nd radius - centre "3" (0-90 degrees)
        //    for (short i = 0; i < m_iNumOfArcPoints; i++)
        //    {
        //        CrScPointsOut[iNumberAux + i + 1, 0] = CrScPointsOut[3, 0] + Geom2D.GetPositionX_deg(m_fr_2, 0 + i * iRadiusAngle / m_iNumOfArcSegment);     // y
        //        CrScPointsOut[iNumberAux + i + 1, 1] = CrScPointsOut[3, 1] + Geom2D.GetPositionY_CW_deg(m_fr_2, 0 + i * iRadiusAngle / m_iNumOfArcSegment);  // z
        //    }

        //    // Point No. XX
        //    CrScPointsOut[iNumberAux + m_iNumOfArcPoints + 1, 0] = m_ft_w / 2.0f;      // y
        //    CrScPointsOut[iNumberAux + m_iNumOfArcPoints + 1, 1] = CrScPointsOut[0, 1] - m_fr_2 - m_fSlopeTaper_1 * ((m_fb_1 - m_ft_w - 2 * m_fr_2) / 2.0f); // z

        //    // Point No. XX
        //    CrScPointsOut[iNumberAux + m_iNumOfArcPoints + 2, 0] = m_ft_w / 2.0f;         // y
        //    CrScPointsOut[iNumberAux + m_iNumOfArcPoints + 2, 1] = CrScPointsOut[4, 1] + m_fr_2 + m_fSlopeTaper_2 * ((m_fb_2 - m_ft_w - 2 * m_fr_2) / 2.0f);     // z

        //    // 3rd radius - centre "4" (270-360 degrees)
        //    for (short i = 0; i < m_iNumOfArcPoints; i++)
        //    {
        //        CrScPointsOut[iNumberAux + m_iNumOfArcPoints + i + 3, 0] = CrScPointsOut[4, 0] + Geom2D.GetPositionX_deg(m_fr_2, 270 + i * iRadiusAngle / m_iNumOfArcSegment);     // y
        //        CrScPointsOut[iNumberAux + m_iNumOfArcPoints + i + 3, 1] = CrScPointsOut[4, 1] + Geom2D.GetPositionY_CW_deg(m_fr_2, 270 + i * iRadiusAngle / m_iNumOfArcSegment); // z
        //    }

        //    // 4th radius - centre "7" (180-270 degrees)
        //    for (short i = 0; i < m_iNumOfArcPoints; i++)
        //    {
        //        CrScPointsOut[iNumberAux + 2 * m_iNumOfArcPoints + i + 3, 0] = CrScPointsOut[7, 0] + Geom2D.GetPositionX_deg(m_fr_2, 180 + i * iRadiusAngle / m_iNumOfArcSegment);     // y
        //        CrScPointsOut[iNumberAux + 2 * m_iNumOfArcPoints + i + 3, 1] = CrScPointsOut[7, 1] + Geom2D.GetPositionY_CW_deg(m_fr_2, 180 + i * iRadiusAngle / m_iNumOfArcSegment); // z
        //    }

        //    // Point No. XX
        //    CrScPointsOut[iNumberAux + 3 * m_iNumOfArcPoints + 3, 0] = - m_ft_w / 2.0f;         // y
        //    CrScPointsOut[iNumberAux + 3 * m_iNumOfArcPoints + 3, 1] = CrScPointsOut[iNumberAux + m_iNumOfArcPoints + 2, 1];     // z

        //    // Point No. XX
        //    CrScPointsOut[iNumberAux + 3 * m_iNumOfArcPoints + 4, 0] = - m_ft_w / 2.0f;         // y
        //    CrScPointsOut[iNumberAux + 3 * m_iNumOfArcPoints + 4, 1] = CrScPointsOut[iNumberAux + m_iNumOfArcPoints + 1, 1];     // z

        //    // 1st radius - centre "0" (90-180 degrees)
        //    // Do not create last point of segment - it is already defined
        //    for (short i = 0; i < m_iNumOfArcPoints - 1; i++)
        //    {
        //        CrScPointsOut[iNumberAux + 3 * m_iNumOfArcPoints + i + 5, 0] = CrScPointsOut[0, 0] + Geom2D.GetPositionX_deg(m_fr_2, 90 + i * iRadiusAngle / m_iNumOfArcSegment);      // y
        //        CrScPointsOut[iNumberAux + 3 * m_iNumOfArcPoints + i + 5, 1] = CrScPointsOut[0, 1] + Geom2D.GetPositionY_CW_deg(m_fr_2, 90 + i * iRadiusAngle / m_iNumOfArcSegment);    // z
        //    }
        //}

        ////----------------------------------------------------------------------------
        //void CalcCrSc_Coord_I_MS_2(float ft_f_1_tip) // 2 - Four radii at flanges roots, tapered or parallel flanges (4 auxiliary points)
        //{
        //    // Fill Point Array Data in LCS (Local Coordinate System of Cross-Section, horizontal y, vertical - z)

        //    // Auxialiary nodes

        //    short iNumberAux = 4;

        //    // Point No. 1
        //    CrScPointsOut[0, 0] = -m_ft_w / 2f;           // y
        //    CrScPointsOut[0, 1] = m_fh - m_fz_c - ft_f_1_tip - (m_fSlopeTaper_1 * ((m_fb_1 - m_ft_w - 2 * m_fr_1) / 2.0f));   // z

        //    // Point No. 2
        //    CrScPointsOut[1, 0] = -CrScPointsOut[0, 0];     // y
        //    CrScPointsOut[1, 1] = CrScPointsOut[0, 1];      // z

        //    // Point No. 3
        //    CrScPointsOut[2, 0] = CrScPointsOut[1, 0];      // y
        //    CrScPointsOut[2, 1] = CrScPointsOut[0, 1] - 2.0f * m_fr_1 - m_fd;     // z

        //    // Point No. 4
        //    CrScPointsOut[3, 0] = CrScPointsOut[0, 0];      // y
        //    CrScPointsOut[3, 1] = CrScPointsOut[2, 1];     // z

        //    // Surface points

        //    // Point No. 5
        //    CrScPointsOut[4, 0] = -m_fb_1 / 2.0f;           // y
        //    CrScPointsOut[4, 1] = m_fh - m_fz_c;           // z

        //    // Point No. 6
        //    CrScPointsOut[5, 0] = -m_ft_w / 2.0f - m_fr_1; ;  // y
        //    CrScPointsOut[5, 1] = CrScPointsOut[4, 1];         // z

        //    // Point No. 7
        //    CrScPointsOut[6, 0] = -CrScPointsOut[5, 0];    // y
        //    CrScPointsOut[6, 1] = CrScPointsOut[5, 1];     // z

        //    // Point No. 8
        //    CrScPointsOut[7, 0] = -CrScPointsOut[4, 0];     // y
        //    CrScPointsOut[7, 1] = CrScPointsOut[4, 1];     // z

        //    // Point No. 9
        //    CrScPointsOut[8, 0] = -CrScPointsOut[4, 0];      // y
        //    CrScPointsOut[8, 1] = CrScPointsOut[4, 1] - ft_f_1_tip;     // z

        //    int iRadiusAngle = 90; // Radius Angle

        //    // 1st radius - centre "1" (90-180 degrees)
        //    for (short i = 0; i < m_iNumOfArcPoints; i++)
        //    {
        //        CrScPointsOut[iNumberAux + i + 4 + 1, 0] = CrScPointsOut[1, 0] + m_fr_1 + Geom2D.GetPositionX_deg(m_fr_1, 90 + i * iRadiusAngle / m_iNumOfArcSegment);     // y
        //        CrScPointsOut[iNumberAux + i + 4 + 1, 1] = CrScPointsOut[1, 1] - m_fr_1 + Geom2D.GetPositionY_CCW_deg(m_fr_1, 90 + i * iRadiusAngle / m_iNumOfArcSegment); // z
        //    }

        //    // 2nd radius - centre "2" (180-270 degrees)
        //    for (short i = 0; i < m_iNumOfArcPoints; i++)
        //    {
        //        CrScPointsOut[iNumberAux + m_iNumOfArcPoints + i + 4 + 1, 0] = CrScPointsOut[2, 0] + m_fr_1 + Geom2D.GetPositionX_deg(m_fr_1, 180 + i * iRadiusAngle / m_iNumOfArcSegment);     // y
        //        CrScPointsOut[iNumberAux + m_iNumOfArcPoints + i + 4 + 1, 1] = CrScPointsOut[2, 1] + m_fr_1 + Geom2D.GetPositionY_CCW_deg(m_fr_1, 180 + i * iRadiusAngle / m_iNumOfArcSegment); // z
        //    }

        //    // Point No. XX
        //    CrScPointsOut[iNumberAux + 2 * m_iNumOfArcPoints + 5, 0] = m_fb_2 / 2.0f;         // y
        //    CrScPointsOut[iNumberAux + 2 * m_iNumOfArcPoints + 5, 1] = -m_fz_c + (m_fh - ft_f_1_tip - (m_fSlopeTaper_1 * ((m_fb_1 - m_ft_w - 2 * m_fr_1) / 2.0f)) - 2 * m_fr_1 - m_fd - (m_fSlopeTaper_2 * ((m_fb_2 - m_ft_w - 2 * m_fr_1) / 2.0f)));        // z

        //    // Point No. XX
        //    CrScPointsOut[iNumberAux + 2 * m_iNumOfArcPoints + 6, 0] = CrScPointsOut[iNumberAux + 2 * m_iNumOfArcPoints + 5, 0];   // y
        //    CrScPointsOut[iNumberAux + 2 * m_iNumOfArcPoints + 6, 1] = -m_fz_c;        // z

        //    // Point No. XX
        //    CrScPointsOut[iNumberAux + 2 * m_iNumOfArcPoints + 7, 0] = CrScPointsOut[6, 0];         // y
        //    CrScPointsOut[iNumberAux + 2 * m_iNumOfArcPoints + 7, 1] = -m_fz_c;        // z

        //    // Point No. XX
        //    CrScPointsOut[iNumberAux + 2 * m_iNumOfArcPoints + 8, 0] = CrScPointsOut[5, 0];         // y
        //    CrScPointsOut[iNumberAux + 2 * m_iNumOfArcPoints + 8, 1] = -m_fz_c;        // z

        //    // Point No. XX
        //    CrScPointsOut[iNumberAux + 2 * m_iNumOfArcPoints + 9, 0] = -CrScPointsOut[iNumberAux + 2 * m_iNumOfArcPoints + 5, 0];         // y
        //    CrScPointsOut[iNumberAux + 2 * m_iNumOfArcPoints + 9, 1] = -m_fz_c;        // z

        //    // Point No. XX
        //    CrScPointsOut[iNumberAux + 2 * m_iNumOfArcPoints + 10, 0] = - CrScPointsOut[iNumberAux + 2 * m_iNumOfArcPoints + 5, 0];       // y
        //    CrScPointsOut[iNumberAux + 2 * m_iNumOfArcPoints + 10, 1] = CrScPointsOut[iNumberAux + 2 * m_iNumOfArcPoints + 5, 1];        // z

        //    // 3rd radius - centre "3" (270-360 degrees)
        //    for (short i = 0; i < m_iNumOfArcPoints; i++)
        //    {
        //        CrScPointsOut[iNumberAux + 2 * m_iNumOfArcPoints + i + 11, 0] = CrScPointsOut[3, 0] - m_fr_1 + Geom2D.GetPositionX_deg(m_fr_1, 270 + i * iRadiusAngle / m_iNumOfArcSegment);     // y
        //        CrScPointsOut[iNumberAux + 2 * m_iNumOfArcPoints + i + 11, 1] = CrScPointsOut[3, 1] + m_fr_1 + Geom2D.GetPositionY_CCW_deg(m_fr_1, 270 + i * iRadiusAngle / m_iNumOfArcSegment); // z
        //    }

        //    // 4th radius - centre "0" (0-90 degrees)
        //    for (short i = 0; i < m_iNumOfArcPoints; i++)
        //    {
        //        CrScPointsOut[iNumberAux + 3 * m_iNumOfArcPoints + i + 11, 0] = CrScPointsOut[0, 0] - m_fr_1 + Geom2D.GetPositionX_deg(m_fr_1, 0 + i * iRadiusAngle / m_iNumOfArcSegment);     // y
        //        CrScPointsOut[iNumberAux + 3 * m_iNumOfArcPoints + i + 11, 1] = CrScPointsOut[0, 1] - m_fr_1 + Geom2D.GetPositionY_CCW_deg(m_fr_1, 0 + i * iRadiusAngle / m_iNumOfArcSegment); // z
        //    }

        //    // Point No. XX
        //    CrScPointsOut[iNumberAux + 4 * m_iNumOfArcPoints + 11, 0] = CrScPointsOut[4, 0];       // y
        //    CrScPointsOut[iNumberAux + 4 * m_iNumOfArcPoints + 11, 1] = CrScPointsOut[8, 1];        // z

        //}


        void CalcCrSc_Coord_I_MS_0() // 0 - Eight radii, tapered or parallel flanges (12 auxiliary points)
        {
            // Fill Point Array Data in LCS (Local Coordinate System of Cross-Section, horizontal y, vertical - z)
            float[,] CrScPointsOutArr = new float[ITotNoPoints, 2];
            // Auxialiary nodes

            short iNumberAux = 8 + 4;

            // Point No. 1
            CrScPointsOutArr[0, 0] = -m_fb_1 / 2f + m_fr_2;    // y
            CrScPointsOutArr[0, 1] = m_fh / 2f;              // z

            // Point No. 2
            CrScPointsOutArr[1, 0] = -m_ft_w / 2.0f - m_fr_1;   // y
            CrScPointsOutArr[1, 1] = CrScPointsOutArr[0, 1];           // z

            // Point No. 3
            CrScPointsOutArr[2, 0] = -CrScPointsOutArr[1, 0];      // y
            CrScPointsOutArr[2, 1] = CrScPointsOutArr[0, 1];       // z

            // Point No. 4
            CrScPointsOutArr[3, 0] = -CrScPointsOutArr[0, 0];      // y
            CrScPointsOutArr[3, 1] = CrScPointsOutArr[0, 1];       // z

            // Point No. 5
            CrScPointsOutArr[4, 0] = m_ft_w / 2.0f;         // y
            CrScPointsOutArr[4, 1] = CrScPointsOutArr[3, 1] - m_fr_2 - ((m_fh - m_fd - 2 * (m_fr_1 + m_fr_2)) / 2.0f);     // z

            // Point No. 6
            CrScPointsOutArr[5, 0] = CrScPointsOutArr[4, 0];      // y
            CrScPointsOutArr[5, 1] = -CrScPointsOutArr[4, 1];     // z

            // Point No. 7
            CrScPointsOutArr[6, 0] = m_fb_2 / 2f - m_fr_2;    // y
            CrScPointsOutArr[6, 1] = -CrScPointsOutArr[3, 1];     // z

            // Point No. 8
            CrScPointsOutArr[7, 0] = CrScPointsOutArr[2, 0];      // y
            CrScPointsOutArr[7, 1] = -CrScPointsOutArr[2, 1];     // z

            // Point No. 9
            CrScPointsOutArr[8, 0] = CrScPointsOutArr[1, 0];      // y
            CrScPointsOutArr[8, 1] = -CrScPointsOutArr[1, 1];     // z

            // Point No. 10
            CrScPointsOutArr[9, 0] = -CrScPointsOutArr[6, 0];      // y
            CrScPointsOutArr[9, 1] = -CrScPointsOutArr[0, 1];     // z

            // Point No. 11
            CrScPointsOutArr[10, 0] = -CrScPointsOutArr[5, 0];    // y
            CrScPointsOutArr[10, 1] = CrScPointsOutArr[5, 1];     // z

            // Point No. 12
            CrScPointsOutArr[11, 0] = -CrScPointsOutArr[4, 0];  // y
            CrScPointsOutArr[11, 1] = CrScPointsOutArr[4, 1];   // z

            // Surface points

            // Point No. 13 - 1st Edge point - upper left
            CrScPointsOutArr[iNumberAux, 0] = -m_fb_1 / 2.0f;     // y
            CrScPointsOutArr[iNumberAux, 1] = m_fh / 2.0f;      // z

            int iRadiusAngle = 90; // Radius Angle

            // 2nd radius - centre "3" (0-90 degrees)
            for (short i = 0; i < m_iNumOfArcPoints; i++)
            {
                CrScPointsOutArr[iNumberAux + i + 1, 0] = CrScPointsOutArr[3, 0] + Geom2D.GetPositionX_deg(m_fr_2, 0 + i * iRadiusAngle / m_iNumOfArcSegment);     // y
                CrScPointsOutArr[iNumberAux + i + 1, 1] = CrScPointsOutArr[3, 1] + Geom2D.GetPositionY_CW_deg(m_fr_2, 0 + i * iRadiusAngle / m_iNumOfArcSegment);  // z
            }

            // 3rd radius - centre "4" (90-180 degrees)
            for (short i = 0; i < m_iNumOfArcPoints; i++)
            {
                CrScPointsOutArr[iNumberAux + m_iNumOfArcPoints + i + 1, 0] = CrScPointsOutArr[4, 0] + m_fr_1 + Geom2D.GetPositionX_deg(m_fr_1, 90 + i * iRadiusAngle / m_iNumOfArcSegment);     // y
                CrScPointsOutArr[iNumberAux + m_iNumOfArcPoints + i + 1, 1] = CrScPointsOutArr[4, 1] - m_fr_1 + Geom2D.GetPositionY_CCW_deg(m_fr_1, 90 + i * iRadiusAngle / m_iNumOfArcSegment); // z
            }

            // 4th radius - centre "5" (180-270 degrees)
            for (short i = 0; i < m_iNumOfArcPoints; i++)
            {
                CrScPointsOutArr[iNumberAux + 2 * m_iNumOfArcPoints + i + 1, 0] = CrScPointsOutArr[5, 0] + m_fr_1 + Geom2D.GetPositionX_deg(m_fr_1, 180 + i * iRadiusAngle / m_iNumOfArcSegment);     // y
                CrScPointsOutArr[iNumberAux + 2 * m_iNumOfArcPoints + i + 1, 1] = CrScPointsOutArr[5, 1] + m_fr_1 + Geom2D.GetPositionY_CCW_deg(m_fr_1, 180 + i * iRadiusAngle / m_iNumOfArcSegment); // z
            }

            // 5th radius - centre "6" (270-360 degrees)
            for (short i = 0; i < m_iNumOfArcPoints; i++)
            {
                CrScPointsOutArr[iNumberAux + 3 * m_iNumOfArcPoints + i + 1, 0] = CrScPointsOutArr[6, 0] + Geom2D.GetPositionX_deg(m_fr_2, 270 + i * iRadiusAngle / m_iNumOfArcSegment);     // y
                CrScPointsOutArr[iNumberAux + 3 * m_iNumOfArcPoints + i + 1, 1] = CrScPointsOutArr[6, 1] + Geom2D.GetPositionY_CW_deg(m_fr_2, 270 + i * iRadiusAngle / m_iNumOfArcSegment);  // z
            }

            // 6th radius - centre "9" (180-270 degrees)
            for (short i = 0; i < m_iNumOfArcPoints; i++)
            {
                CrScPointsOutArr[iNumberAux + 4 * m_iNumOfArcPoints + i + 1, 0] = CrScPointsOutArr[9, 0] + Geom2D.GetPositionX_deg(m_fr_2, 180 + i * iRadiusAngle / m_iNumOfArcSegment);     // y
                CrScPointsOutArr[iNumberAux + 4 * m_iNumOfArcPoints + i + 1, 1] = CrScPointsOutArr[9, 1] + Geom2D.GetPositionY_CW_deg(m_fr_2, 180 + i * iRadiusAngle / m_iNumOfArcSegment);  // z
            }

            // 7th radius - centre "10" (270-360 degrees)
            for (short i = 0; i < m_iNumOfArcPoints; i++)
            {
                CrScPointsOutArr[iNumberAux + 5 * m_iNumOfArcPoints + i + 1, 0] = CrScPointsOutArr[10, 0] - m_fr_1 + Geom2D.GetPositionX_deg(m_fr_1, 270 + i * iRadiusAngle / m_iNumOfArcSegment);     // y
                CrScPointsOutArr[iNumberAux + 5 * m_iNumOfArcPoints + i + 1, 1] = CrScPointsOutArr[10, 1] + m_fr_1 + Geom2D.GetPositionY_CCW_deg(m_fr_1, 270 + i * iRadiusAngle / m_iNumOfArcSegment); // z
            }

            // 8th radius - centre "11" (0-90 degrees)
            for (short i = 0; i < m_iNumOfArcPoints; i++)
            {
                CrScPointsOutArr[iNumberAux + 6 * m_iNumOfArcPoints + i + 1, 0] = CrScPointsOutArr[11, 0] - m_fr_1 + Geom2D.GetPositionX_deg(m_fr_1, 0 + i * iRadiusAngle / m_iNumOfArcSegment);     // y
                CrScPointsOutArr[iNumberAux + 6 * m_iNumOfArcPoints + i + 1, 1] = CrScPointsOutArr[11, 1] - m_fr_1 + Geom2D.GetPositionY_CCW_deg(m_fr_1, 0 + i * iRadiusAngle / m_iNumOfArcSegment); // z
            }

            // 1st radius - centre "0" (90-180 degrees)
            // Do not create last point of segment - it is already defined
            for (short i = 0; i < m_iNumOfArcPoints - 1; i++)
            {
                CrScPointsOutArr[iNumberAux + 7 * m_iNumOfArcPoints + i + 1, 0] = CrScPointsOutArr[0, 0] + Geom2D.GetPositionX_deg(m_fr_2, 90 + i * iRadiusAngle / m_iNumOfArcSegment);      // y
                CrScPointsOutArr[iNumberAux + 7 * m_iNumOfArcPoints + i + 1, 1] = CrScPointsOutArr[0, 1] + Geom2D.GetPositionY_CW_deg(m_fr_2, 90 + i * iRadiusAngle / m_iNumOfArcSegment);    // z
            }

            for (int i = 0; i < CrScPointsOutArr.GetLength(0); i++)
            {
                CrScPointsOut.Add(new Point(CrScPointsOutArr[i, 0], CrScPointsOutArr[i, 1]));
            }
        }

        //----------------------------------------------------------------------------
        void CalcCrSc_Coord_I_MS_1() // 1 - Four radii at flanges tips, tapered or parallel flanges (8 auxiliary points)
        {
            // Fill Point Array Data in LCS (Local Coordinate System of Cross-Section, horizontal y, vertical - z)
            float[,] CrScPointsOutArr = new float[ITotNoPoints, 2];
            // Auxialiary nodes

            short iNumberAux = 4 + 4;

            // Point No. 1
            CrScPointsOutArr[0, 0] = -m_fb_1 / 2.0f + m_fr_2;    // y
            CrScPointsOutArr[0, 1] = m_fh - m_fz_c;       // z

            // Point No. 2
            CrScPointsOutArr[1, 0] = -m_ft_w / 2.0f;        // y
            CrScPointsOutArr[1, 1] = CrScPointsOutArr[0, 1];     // z

            // Point No. 3
            CrScPointsOutArr[2, 0] = -CrScPointsOutArr[1, 0];    // y
            CrScPointsOutArr[2, 1] = CrScPointsOutArr[0, 1];     // z

            // Point No. 4
            CrScPointsOutArr[3, 0] = -CrScPointsOutArr[0, 0];    // y
            CrScPointsOutArr[3, 1] = CrScPointsOutArr[0, 1];     // z

            // Point No. 5
            CrScPointsOutArr[4, 0] = m_fb_2 / 2.0f - m_fr_2;// y
            CrScPointsOutArr[4, 1] = -m_fz_c;               // z

            // Point No. 6
            CrScPointsOutArr[5, 0] = CrScPointsOutArr[2, 0];     // y
            CrScPointsOutArr[5, 1] = CrScPointsOutArr[4, 1];     // z

            // Point No. 7
            CrScPointsOutArr[6, 0] = CrScPointsOutArr[1, 0];     // y
            CrScPointsOutArr[6, 1] = CrScPointsOutArr[4, 1];     // z

            // Point No. 8
            CrScPointsOutArr[7, 0] = -CrScPointsOutArr[4, 0];    // y
            CrScPointsOutArr[7, 1] = CrScPointsOutArr[4, 1];     // z

            // Surface points

            // Point No. 8 - 1st Edge point - upper left
            CrScPointsOutArr[iNumberAux, 0] = -m_fb_1 / 2.0f;     // y
            CrScPointsOutArr[iNumberAux, 1] = CrScPointsOutArr[0, 1];  // z

            int iRadiusAngle = 90; // Radius Angle

            // 2nd radius - centre "3" (0-90 degrees)
            for (short i = 0; i < m_iNumOfArcPoints; i++)
            {
                CrScPointsOutArr[iNumberAux + i + 1, 0] = CrScPointsOutArr[3, 0] + Geom2D.GetPositionX_deg(m_fr_2, 0 + i * iRadiusAngle / m_iNumOfArcSegment);     // y
                CrScPointsOutArr[iNumberAux + i + 1, 1] = CrScPointsOutArr[3, 1] + Geom2D.GetPositionY_CW_deg(m_fr_2, 0 + i * iRadiusAngle / m_iNumOfArcSegment);  // z
            }

            // Point No. XX
            CrScPointsOutArr[iNumberAux + m_iNumOfArcPoints + 1, 0] = m_ft_w / 2.0f;      // y
            CrScPointsOutArr[iNumberAux + m_iNumOfArcPoints + 1, 1] = CrScPointsOutArr[0, 1] - m_fr_2 - m_fSlopeTaper_1 * ((m_fb_1 - m_ft_w - 2 * m_fr_2) / 2.0f); // z

            // Point No. XX
            CrScPointsOutArr[iNumberAux + m_iNumOfArcPoints + 2, 0] = m_ft_w / 2.0f;         // y
            CrScPointsOutArr[iNumberAux + m_iNumOfArcPoints + 2, 1] = CrScPointsOutArr[4, 1] + m_fr_2 + m_fSlopeTaper_2 * ((m_fb_2 - m_ft_w - 2 * m_fr_2) / 2.0f);     // z

            // 3rd radius - centre "4" (270-360 degrees)
            for (short i = 0; i < m_iNumOfArcPoints; i++)
            {
                CrScPointsOutArr[iNumberAux + m_iNumOfArcPoints + i + 3, 0] = CrScPointsOutArr[4, 0] + Geom2D.GetPositionX_deg(m_fr_2, 270 + i * iRadiusAngle / m_iNumOfArcSegment);     // y
                CrScPointsOutArr[iNumberAux + m_iNumOfArcPoints + i + 3, 1] = CrScPointsOutArr[4, 1] + Geom2D.GetPositionY_CW_deg(m_fr_2, 270 + i * iRadiusAngle / m_iNumOfArcSegment); // z
            }

            // 4th radius - centre "7" (180-270 degrees)
            for (short i = 0; i < m_iNumOfArcPoints; i++)
            {
                CrScPointsOutArr[iNumberAux + 2 * m_iNumOfArcPoints + i + 3, 0] = CrScPointsOutArr[7, 0] + Geom2D.GetPositionX_deg(m_fr_2, 180 + i * iRadiusAngle / m_iNumOfArcSegment);     // y
                CrScPointsOutArr[iNumberAux + 2 * m_iNumOfArcPoints + i + 3, 1] = CrScPointsOutArr[7, 1] + Geom2D.GetPositionY_CW_deg(m_fr_2, 180 + i * iRadiusAngle / m_iNumOfArcSegment); // z
            }

            // Point No. XX
            CrScPointsOutArr[iNumberAux + 3 * m_iNumOfArcPoints + 3, 0] = -m_ft_w / 2.0f;         // y
            CrScPointsOutArr[iNumberAux + 3 * m_iNumOfArcPoints + 3, 1] = CrScPointsOutArr[iNumberAux + m_iNumOfArcPoints + 2, 1];     // z

            // Point No. XX
            CrScPointsOutArr[iNumberAux + 3 * m_iNumOfArcPoints + 4, 0] = -m_ft_w / 2.0f;         // y
            CrScPointsOutArr[iNumberAux + 3 * m_iNumOfArcPoints + 4, 1] = CrScPointsOutArr[iNumberAux + m_iNumOfArcPoints + 1, 1];     // z

            // 1st radius - centre "0" (90-180 degrees)
            // Do not create last point of segment - it is already defined
            for (short i = 0; i < m_iNumOfArcPoints - 1; i++)
            {
                CrScPointsOutArr[iNumberAux + 3 * m_iNumOfArcPoints + i + 5, 0] = CrScPointsOutArr[0, 0] + Geom2D.GetPositionX_deg(m_fr_2, 90 + i * iRadiusAngle / m_iNumOfArcSegment);      // y
                CrScPointsOutArr[iNumberAux + 3 * m_iNumOfArcPoints + i + 5, 1] = CrScPointsOutArr[0, 1] + Geom2D.GetPositionY_CW_deg(m_fr_2, 90 + i * iRadiusAngle / m_iNumOfArcSegment);    // z
            }

            for (int i = 0; i < CrScPointsOutArr.GetLength(0); i++)
            {
                CrScPointsOut.Add(new Point(CrScPointsOutArr[i, 0], CrScPointsOutArr[i, 1]));
            }
        }

        //----------------------------------------------------------------------------
        void CalcCrSc_Coord_I_MS_2(float ft_f_1_tip) // 2 - Four radii at flanges roots, tapered or parallel flanges (4 auxiliary points)
        {
            // Fill Point Array Data in LCS (Local Coordinate System of Cross-Section, horizontal y, vertical - z)
            float[,] CrScPointsOutArr = new float[ITotNoPoints, 2];
            // Auxialiary nodes

            short iNumberAux = 4;

            // Point No. 1
            CrScPointsOutArr[0, 0] = -m_ft_w / 2f;           // y
            CrScPointsOutArr[0, 1] = m_fh - m_fz_c - ft_f_1_tip - (m_fSlopeTaper_1 * ((m_fb_1 - m_ft_w - 2 * m_fr_1) / 2.0f));   // z

            // Point No. 2
            CrScPointsOutArr[1, 0] = -CrScPointsOutArr[0, 0];     // y
            CrScPointsOutArr[1, 1] = CrScPointsOutArr[0, 1];      // z

            // Point No. 3
            CrScPointsOutArr[2, 0] = CrScPointsOutArr[1, 0];      // y
            CrScPointsOutArr[2, 1] = CrScPointsOutArr[0, 1] - 2.0f * m_fr_1 - m_fd;     // z

            // Point No. 4
            CrScPointsOutArr[3, 0] = CrScPointsOutArr[0, 0];      // y
            CrScPointsOutArr[3, 1] = CrScPointsOutArr[2, 1];     // z

            // Surface points

            // Point No. 5
            CrScPointsOutArr[4, 0] = -m_fb_1 / 2.0f;           // y
            CrScPointsOutArr[4, 1] = m_fh - m_fz_c;           // z

            // Point No. 6
            CrScPointsOutArr[5, 0] = -m_ft_w / 2.0f - m_fr_1; ;  // y
            CrScPointsOutArr[5, 1] = CrScPointsOutArr[4, 1];         // z

            // Point No. 7
            CrScPointsOutArr[6, 0] = -CrScPointsOutArr[5, 0];    // y
            CrScPointsOutArr[6, 1] = CrScPointsOutArr[5, 1];     // z

            // Point No. 8
            CrScPointsOutArr[7, 0] = -CrScPointsOutArr[4, 0];     // y
            CrScPointsOutArr[7, 1] = CrScPointsOutArr[4, 1];     // z

            // Point No. 9
            CrScPointsOutArr[8, 0] = -CrScPointsOutArr[4, 0];      // y
            CrScPointsOutArr[8, 1] = CrScPointsOutArr[4, 1] - ft_f_1_tip;     // z

            int iRadiusAngle = 90; // Radius Angle

            // 1st radius - centre "1" (90-180 degrees)
            for (short i = 0; i < m_iNumOfArcPoints; i++)
            {
                CrScPointsOutArr[iNumberAux + i + 4 + 1, 0] = CrScPointsOutArr[1, 0] + m_fr_1 + Geom2D.GetPositionX_deg(m_fr_1, 90 + i * iRadiusAngle / m_iNumOfArcSegment);     // y
                CrScPointsOutArr[iNumberAux + i + 4 + 1, 1] = CrScPointsOutArr[1, 1] - m_fr_1 + Geom2D.GetPositionY_CCW_deg(m_fr_1, 90 + i * iRadiusAngle / m_iNumOfArcSegment); // z
            }

            // 2nd radius - centre "2" (180-270 degrees)
            for (short i = 0; i < m_iNumOfArcPoints; i++)
            {
                CrScPointsOutArr[iNumberAux + m_iNumOfArcPoints + i + 4 + 1, 0] = CrScPointsOutArr[2, 0] + m_fr_1 + Geom2D.GetPositionX_deg(m_fr_1, 180 + i * iRadiusAngle / m_iNumOfArcSegment);     // y
                CrScPointsOutArr[iNumberAux + m_iNumOfArcPoints + i + 4 + 1, 1] = CrScPointsOutArr[2, 1] + m_fr_1 + Geom2D.GetPositionY_CCW_deg(m_fr_1, 180 + i * iRadiusAngle / m_iNumOfArcSegment); // z
            }

            // Point No. XX
            CrScPointsOutArr[iNumberAux + 2 * m_iNumOfArcPoints + 5, 0] = m_fb_2 / 2.0f;         // y
            CrScPointsOutArr[iNumberAux + 2 * m_iNumOfArcPoints + 5, 1] = -m_fz_c + (m_fh - ft_f_1_tip - (m_fSlopeTaper_1 * ((m_fb_1 - m_ft_w - 2 * m_fr_1) / 2.0f)) - 2 * m_fr_1 - m_fd - (m_fSlopeTaper_2 * ((m_fb_2 - m_ft_w - 2 * m_fr_1) / 2.0f)));        // z

            // Point No. XX
            CrScPointsOutArr[iNumberAux + 2 * m_iNumOfArcPoints + 6, 0] = CrScPointsOutArr[iNumberAux + 2 * m_iNumOfArcPoints + 5, 0];   // y
            CrScPointsOutArr[iNumberAux + 2 * m_iNumOfArcPoints + 6, 1] = -m_fz_c;        // z

            // Point No. XX
            CrScPointsOutArr[iNumberAux + 2 * m_iNumOfArcPoints + 7, 0] = CrScPointsOutArr[6, 0];         // y
            CrScPointsOutArr[iNumberAux + 2 * m_iNumOfArcPoints + 7, 1] = -m_fz_c;        // z

            // Point No. XX
            CrScPointsOutArr[iNumberAux + 2 * m_iNumOfArcPoints + 8, 0] = CrScPointsOutArr[5, 0];         // y
            CrScPointsOutArr[iNumberAux + 2 * m_iNumOfArcPoints + 8, 1] = -m_fz_c;        // z

            // Point No. XX
            CrScPointsOutArr[iNumberAux + 2 * m_iNumOfArcPoints + 9, 0] = -CrScPointsOutArr[iNumberAux + 2 * m_iNumOfArcPoints + 5, 0];         // y
            CrScPointsOutArr[iNumberAux + 2 * m_iNumOfArcPoints + 9, 1] = -m_fz_c;        // z

            // Point No. XX
            CrScPointsOutArr[iNumberAux + 2 * m_iNumOfArcPoints + 10, 0] = -CrScPointsOutArr[iNumberAux + 2 * m_iNumOfArcPoints + 5, 0];       // y
            CrScPointsOutArr[iNumberAux + 2 * m_iNumOfArcPoints + 10, 1] = CrScPointsOutArr[iNumberAux + 2 * m_iNumOfArcPoints + 5, 1];        // z

            // 3rd radius - centre "3" (270-360 degrees)
            for (short i = 0; i < m_iNumOfArcPoints; i++)
            {
                CrScPointsOutArr[iNumberAux + 2 * m_iNumOfArcPoints + i + 11, 0] = CrScPointsOutArr[3, 0] - m_fr_1 + Geom2D.GetPositionX_deg(m_fr_1, 270 + i * iRadiusAngle / m_iNumOfArcSegment);     // y
                CrScPointsOutArr[iNumberAux + 2 * m_iNumOfArcPoints + i + 11, 1] = CrScPointsOutArr[3, 1] + m_fr_1 + Geom2D.GetPositionY_CCW_deg(m_fr_1, 270 + i * iRadiusAngle / m_iNumOfArcSegment); // z
            }

            // 4th radius - centre "0" (0-90 degrees)
            for (short i = 0; i < m_iNumOfArcPoints; i++)
            {
                CrScPointsOutArr[iNumberAux + 3 * m_iNumOfArcPoints + i + 11, 0] = CrScPointsOutArr[0, 0] - m_fr_1 + Geom2D.GetPositionX_deg(m_fr_1, 0 + i * iRadiusAngle / m_iNumOfArcSegment);     // y
                CrScPointsOutArr[iNumberAux + 3 * m_iNumOfArcPoints + i + 11, 1] = CrScPointsOutArr[0, 1] - m_fr_1 + Geom2D.GetPositionY_CCW_deg(m_fr_1, 0 + i * iRadiusAngle / m_iNumOfArcSegment); // z
            }

            // Point No. XX
            CrScPointsOutArr[iNumberAux + 4 * m_iNumOfArcPoints + 11, 0] = CrScPointsOutArr[4, 0];       // y
            CrScPointsOutArr[iNumberAux + 4 * m_iNumOfArcPoints + 11, 1] = CrScPointsOutArr[8, 1];        // z

            for (int i = 0; i < CrScPointsOutArr.GetLength(0); i++)
            {
                CrScPointsOut.Add(new Point(CrScPointsOutArr[i, 0], CrScPointsOutArr[i, 1]));
            }
        }

        protected override void loadCrScIndices()
        {
            CCrSc_3_00 oTemp = new CCrSc_3_00();
            oTemp.loadCrScIndices_00_01(m_sShape, 0, m_iNumOfArcSegment);            
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
