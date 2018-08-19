using MATH;
using System.Windows.Media;

namespace BaseClasses.CRSC
{
    public class CCrSc_3_08 : CSO
    {
        // Rolled mono-symmetric T section

        //----------------------------------------------------------------------------

        private short m_sShape;       // Section shape

        // Section shapes types
        // 0 - Five radii, tapered flanges, optional tapered web (5+2 auxiliary points)
        // 1 - Four radii, tapered or parallel flanges, optional tapered web (4+2 auxiliary points)
        // 2 - Two radii at flanges tips, tapered or parallel flanges, optional tapered web (4 auxiliary points)
        // 3 - Two radii at flanges roots, tapered or parallel flanges, optional tapered web (2 auxiliary points)

        private float m_fh;                 // Height-depth / Vyska
        private float m_fb;                 // Width  / Sirka
        private float m_ft_f;               // Flange Thickness / Hrubka pasnice
        private float m_ft_w;               // Web Thickness  / Hrubka steny/stojiny
        private float m_fz_c;               // Centroid coordinate / Suradnica tažiska / Absolute value
        private float m_fr_1;               // Radius - web to flange face
        private float m_fr_2;               // Radius - flange edge
        private float m_fr_3;               // Radius - web edge - optional
        private float m_fd;                 // Web depth - straigth part - perpendicular distance
        private float m_fSlopeTaperFlange;  // Slope of Tapered Flange
        private short m_iNumOfArcSegment;   // Number of Arc Segments
        private short m_iNumOfArcPoints;    // Number of Arc Points
        private short m_iNumOfAuxPoints;    // Number of Auxialiary Points
        //private short m_iTotNoPoints;       // Total Number of Cross-section Points for Drawing
        //public float[,] m_CrScPoint;        // Array of Points and values in 2D
        //----------------------------------------------------------------------------

        public float Fh
        {
            get { return m_fh; }
            set { m_fh = value; }
        }
        public float Fb
        {
            get { return m_fb; }
            set { m_fb = value; }
        }
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
        public float Fz_c
        {
            get { return m_fz_c; }
            set { m_fz_c = value; }
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
        public float Fr_3
        {
            get { return m_fr_3; }
            set { m_fr_3 = value; }
        }

        public float Fd
        {
            get { return m_fd; }
            set { m_fd = value; }
        }

        /*public short ITotNoPoints
        {
            get { return m_iTotNoPoints; }
            set { m_iTotNoPoints = value; }
        }*/

        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        public CCrSc_3_08()  {   }
        public CCrSc_3_08(short sShape, float fh, float fb, float ft_f, float ft_w, float fz_c, float fr_1, float fr_2, float fr_3, float fd)
        {
            // Section shapes types
            // 0 - Five radii, tapered flanges, optional tapered web (5+2 auxiliary points)
            // 1 - Four radii, tapered or parallel flanges, optional tapered web (4+2 auxiliary points) 2*r3 - thickness of web at bottom edge

            IsShapeSolid = true;
            m_iNumOfArcSegment = 4;
            m_iNumOfArcPoints = (short)(m_iNumOfArcSegment + 1); // Each arc is defined by number of segments + 1 point points

            m_sShape = sShape;
            m_fh = fh;
            m_fb = fb;
            m_ft_f = ft_f;
            m_ft_w = ft_w;
            m_fz_c = fz_c;
            m_fr_1 = fr_1;
            m_fr_2 = fr_2;
            m_fr_3 = fr_3;
            m_fd = fd;

            // Number of points per section
            if (m_sShape == 0)       // 0 - Five radii, tapered flanges, optional tapered web (5+2 auxiliary points)
            {
                m_iNumOfAuxPoints = 7; // ??
                ITotNoPoints = (short)(4 * (short)m_iNumOfArcPoints + (2 * m_iNumOfArcPoints - 1) + (5 + 2));
                m_fSlopeTaperFlange = (m_fh - m_fd - (m_fr_1 + m_fr_2 + m_fr_3)) / ((m_fb - (2 * m_fr_3 + 4 * (m_ft_w - 2 * m_fr_3)) - 2 * (m_fr_1 + m_fr_2)) / 2.0f);
            }
            else if (m_sShape == 1)  // 1 - Four radii, tapered or paralel flanges, optional tapered web (4+2 auxiliary points)
            {
                m_iNumOfAuxPoints = 6;
                ITotNoPoints = (short)((4 * (short)m_iNumOfArcPoints) + 2 + (4 + 2));
                m_fSlopeTaperFlange = (m_fh - m_fd - (m_fr_1 + m_fr_2)) / ((m_fb - (2 * m_fr_3 + 4 * (m_ft_w - 2 * m_fr_3)) - 2 * (m_fr_1 + m_fr_2)) / 2.0f);
            }

            // Create Array - allocate memory
            CrScPointsOut = new float [ITotNoPoints,2];
            // Fill Array Data

            if (m_sShape == 0)       // 0 - Five radii, tapered flanges, optional tapered web (5+2 auxiliary points)
                CalcCrSc_Coord_T_MS_0();
            else if (m_sShape == 1)  // 1 - Four radii, tapered or parallel flanges, optional tapered web (4+2 auxiliary points)
                CalcCrSc_Coord_T_MS_1();

            // Fill list of indices for drawing of surface - triangles edges
            loadCrScIndices();
        }

        public CCrSc_3_08(short sShape, float fh, float fb, float ft_f, float ft_w, float fz_c, float fr, float fr_3, float fd)
        {
            // 2 - Two radii at flanges tips, tapered or parallel flanges, optional tapered web (4 auxiliary points) 2*r3 - thickness of web at bottom edge, r1 = 0
            // 3 - Two radii at flanges roots, tapered or parallel flanges, optional tapered web (2 auxiliary points) 2*r3 - thickness of web at bottom edge, r2 = 0

            IsShapeSolid = true;
            m_iNumOfArcSegment = 4;
            m_iNumOfArcPoints = (short)(m_iNumOfArcSegment + 1); // Each arc is defined by number of segments + 1 point points

            m_sShape = sShape;
            m_fh = fh;
            m_fb = fb;
            m_ft_f = ft_f;
            m_ft_w = ft_w;
            m_fz_c = fz_c;
            m_fr_3 = fr_3;
            m_fd = fd;

            // Number of points per section
            if (m_sShape == 2)  // 2 - Two radii at flanges tips, tapered or parallel flanges, optional tapered web (4 auxiliary points)
            {
                m_fr_1 = 0.0f;
                m_fr_2 = fr;
                m_iNumOfAuxPoints = 4;

                ITotNoPoints = (short)((2 * (short)m_iNumOfArcPoints) + 4 + 4);
                m_fSlopeTaperFlange = (m_fh - m_fd - m_fr_2) / ((m_fb - (2 * m_fr_3 + 4 * (m_ft_w - 2 * m_fr_3)) - 2 * m_fr_2) / 2.0f);
            }
            else if (m_sShape == 3)  // 3 - Two radii at flanges roots, tapered or parallel flanges, optional tapered web (2 auxiliary points)
            {
                m_fr_1 = fr;
                m_fr_2 = 0.0f;
                m_iNumOfAuxPoints = 2;

                ITotNoPoints = (short)((2 * (short)m_iNumOfArcPoints) + 2 + 8);
                m_fSlopeTaperFlange = (2 * (m_fh - m_fd - m_fr_1 -m_ft_f)) / ((m_fb - (2 * m_fr_3 + 4 * (m_ft_w - 2 * m_fr_3)) - 2 * m_fr_1) / 2.0f);
            }
            else // Exception
            { }

            // Create Array - allocate memory
            CrScPointsOut = new float[ITotNoPoints, 2];
            // Fill Array Data

            if (m_sShape == 2)       // 2 - Two radii at flanges tips, tapered or parallel flanges, optional tapered web (4 auxiliary points)
                CalcCrSc_Coord_T_MS_2();
            else if (m_sShape == 3)       // 3 - Two radii at flanges roots, tapered or parallel flanges, optional tapered web (2 auxiliary points)
                CalcCrSc_Coord_T_MS_3();
            else // Exception
            { }

            // Fill list of indices for drawing of surface - triangles edges
            loadCrScIndices();
        }


        //----------------------------------------------------------------------------
        // 0 - Five radii, tapered flanges, optional tapered web (5+2 auxiliary points)

        void CalcCrSc_Coord_T_MS_0()
        {
            // Fill Point Array Data in LCS (Local Coordinate System of Cross-Section, horizontal y, vertical - z)

            // auxiliary nodes

            //short m_iNumOfAuxPoints = 5 + 2;

            // Point No. 1
            CrScPointsOut[0,0] = -m_fb / 2f + m_fr_2;    // y
            CrScPointsOut[0,1] = m_fh - m_fz_c;              // z

            // Point No. 2
            CrScPointsOut[1, 0] = -m_fb / 2f + m_fr_2 + (m_fb - (2 * m_fr_3 + 4 * (m_ft_w - 2 * m_fr_3)) - 2 * (m_fr_1 + m_fr_2)) / 2.0f;   // y
            CrScPointsOut[1,1] = CrScPointsOut[0,1];           // z

            // Point No. 3
            CrScPointsOut[2,0] = -CrScPointsOut[1,0];      // y
            CrScPointsOut[2,1] = CrScPointsOut[0,1];       // z

            // Point No. 4
            CrScPointsOut[3,0] = -CrScPointsOut[0,0];      // y
            CrScPointsOut[3,1] = CrScPointsOut[0,1];       // z

            // Point No. 5
            CrScPointsOut[4, 0] = (m_ft_w + (m_ft_w - 2*m_fr_3))/2f;         // y
            CrScPointsOut[4, 1] = -m_fz_c + m_fr_3 + m_fd + m_fr_1;          // z

            // Point No. 6
            CrScPointsOut[5,0] = 0;         // y
            CrScPointsOut[5,1] = - m_fz_c + m_fr_3;     // z

            // Point No. 7
            CrScPointsOut[6,0] = -CrScPointsOut[4,0];      // y
            CrScPointsOut[6,1] = CrScPointsOut[4,1];     // z

            // Surface points

            // Point No. 8 - 1st Edge point - upper left
            CrScPointsOut[m_iNumOfAuxPoints, 0] = -m_fb / 2.0f;     // y
            CrScPointsOut[m_iNumOfAuxPoints, 1] = m_fh  - m_fz_c;   // z

            int iRadiusAngle = 90; // Radius Angle

            // 2nd radius - centre "3" (0-90 degrees)
            for (short i = 0; i < m_iNumOfArcPoints; i++)
            {
                CrScPointsOut[m_iNumOfAuxPoints + i + 1, 0] = CrScPointsOut[3, 0] + Geom2D.GetPositionX(m_fr_2, 0 + i * iRadiusAngle / m_iNumOfArcSegment);     // y
                CrScPointsOut[m_iNumOfAuxPoints + i + 1, 1] = CrScPointsOut[3, 1] + Geom2D.GetPositionY_CW(m_fr_2, 0 + i * iRadiusAngle / m_iNumOfArcSegment);  // z
            }

            // 3rd radius - centre "4" (90-180 degrees)
            for (short i = 0; i < m_iNumOfArcPoints; i++)
            {
                CrScPointsOut[m_iNumOfAuxPoints + m_iNumOfArcPoints + i + 1, 0] = CrScPointsOut[4, 0] + m_fr_1 + Geom2D.GetPositionX(m_fr_1, 90 + i * iRadiusAngle / m_iNumOfArcSegment);     // y
                CrScPointsOut[m_iNumOfAuxPoints + m_iNumOfArcPoints + i + 1, 1] = CrScPointsOut[4, 1] - m_fr_1 + Geom2D.GetPositionY_CCW(m_fr_1, 90 + i * iRadiusAngle / m_iNumOfArcSegment); // z
            }

            // 4th radius - centre "5" (0-180 degrees)
            for (short i = 0; i < 2 * m_iNumOfArcPoints-1; i++)
            {
                CrScPointsOut[m_iNumOfAuxPoints + 2 * m_iNumOfArcPoints + i + 1, 0] = CrScPointsOut[5, 0] + Geom2D.GetPositionX(m_fr_3, 0 + i * iRadiusAngle / m_iNumOfArcSegment);     // y
                CrScPointsOut[m_iNumOfAuxPoints + 2 * m_iNumOfArcPoints + i + 1, 1] = CrScPointsOut[5, 1] + Geom2D.GetPositionY_CW(m_fr_3, 0 + i * iRadiusAngle / m_iNumOfArcSegment); // z
            }

            // 5th radius - centre "6" (180-270 degrees)
            for (short i = 0; i < m_iNumOfArcPoints; i++)
            {
                CrScPointsOut[m_iNumOfAuxPoints + 2 * m_iNumOfArcPoints + 2 * m_iNumOfArcPoints + i, 0] = CrScPointsOut[6, 0] - m_fr_1 + Geom2D.GetPositionX(m_fr_1, 0 + i * iRadiusAngle / m_iNumOfArcSegment);     // y
                CrScPointsOut[m_iNumOfAuxPoints + 2 * m_iNumOfArcPoints + 2 * m_iNumOfArcPoints + i, 1] = CrScPointsOut[6, 1] - m_fr_1 + Geom2D.GetPositionY_CCW(m_fr_1, 0 + i * iRadiusAngle / m_iNumOfArcSegment); // z
            }

            // 1st radius - centre "0" (90-180 degrees)
            // Do not create last point of segment - it is already defined
            for (short i = 0; i < m_iNumOfArcPoints - 1; i++)
            {
                CrScPointsOut[m_iNumOfAuxPoints + 3 * m_iNumOfArcPoints + 2 * m_iNumOfArcPoints + i, 0] = CrScPointsOut[0, 0] + Geom2D.GetPositionX(m_fr_2, 90 + i * iRadiusAngle / m_iNumOfArcSegment);      // y
                CrScPointsOut[m_iNumOfAuxPoints + 3 * m_iNumOfArcPoints + 2 * m_iNumOfArcPoints + i, 1] = CrScPointsOut[0, 1] + Geom2D.GetPositionY_CW(m_fr_2, 90 + i * iRadiusAngle / m_iNumOfArcSegment);    // z
            }
        }

        //----------------------------------------------------------------------------
        // 1 - Four radii, tapered or parallel flanges, optional tapered web (4+2 auxiliary points)

        void CalcCrSc_Coord_T_MS_1()
        {
            // Fill Point Array Data in LCS (Local Coordinate System of Cross-Section, horizontal y, vertical - z)

            // auxiliary nodes

            //short m_iNumOfAuxPoints = 4 + 2;

            // Point No. 1
            CrScPointsOut[0, 0] = -m_fb / 2f + m_fr_2;    // y
            CrScPointsOut[0, 1] = m_fh - m_fz_c;          // z

            // Point No. 2
            CrScPointsOut[1, 0] = -m_fb / 2f + m_fr_2 + (m_fb - (2 * m_fr_3 + 4 * (m_ft_w - 2 * m_fr_3)) - 2 * (m_fr_1 + m_fr_2)) / 2.0f;   // y
            CrScPointsOut[1, 1] = CrScPointsOut[0, 1];           // z

            // Point No. 3
            CrScPointsOut[2, 0] = -CrScPointsOut[1, 0];      // y
            CrScPointsOut[2, 1] = CrScPointsOut[0, 1];       // z

            // Point No. 4
            CrScPointsOut[3, 0] = -CrScPointsOut[0, 0];      // y
            CrScPointsOut[3, 1] = CrScPointsOut[0, 1];       // z

            // Point No. 5
            CrScPointsOut[4, 0] = (m_ft_w + (m_ft_w - 2 * m_fr_3)) / 2f;         // y
            CrScPointsOut[4, 1] = -m_fz_c + m_fd + m_fr_1;          // z

            // Point No. 6
            CrScPointsOut[5, 0] = -CrScPointsOut[4, 0];      // y
            CrScPointsOut[5, 1] = CrScPointsOut[4, 1];     // z

            // Surface points

            // Point No. 7 - 1st Edge point - upper left
            CrScPointsOut[m_iNumOfAuxPoints, 0] = -m_fb / 2.0f;     // y
            CrScPointsOut[m_iNumOfAuxPoints, 1] = m_fh - m_fz_c;   // z

            int iRadiusAngle = 90; // Radius Angle

            // 2nd radius - centre "3" (0-90 degrees)
            for (short i = 0; i < m_iNumOfArcPoints; i++)
            {
                CrScPointsOut[m_iNumOfAuxPoints + i + 1, 0] = CrScPointsOut[3, 0] + Geom2D.GetPositionX(m_fr_2, 0 + i * iRadiusAngle / m_iNumOfArcSegment);     // y
                CrScPointsOut[m_iNumOfAuxPoints + i + 1, 1] = CrScPointsOut[3, 1] + Geom2D.GetPositionY_CW(m_fr_2, 0 + i * iRadiusAngle / m_iNumOfArcSegment);  // z
            }

            // 3rd radius - centre "4" (90-180 degrees)
            for (short i = 0; i < m_iNumOfArcPoints; i++)
            {
                CrScPointsOut[m_iNumOfAuxPoints + m_iNumOfArcPoints + i + 1, 0] = CrScPointsOut[4, 0] + m_fr_1 + Geom2D.GetPositionX(m_fr_1, 90 + i * iRadiusAngle / m_iNumOfArcSegment);     // y
                CrScPointsOut[m_iNumOfAuxPoints + m_iNumOfArcPoints + i + 1, 1] = CrScPointsOut[4, 1] - m_fr_1 + Geom2D.GetPositionY_CCW(m_fr_1, 90 + i * iRadiusAngle / m_iNumOfArcSegment); // z
            }

            // Point No. XX - Bottom Right Edge point
            CrScPointsOut[m_iNumOfAuxPoints + 2 * m_iNumOfArcPoints +1, 0] = m_fr_3;      // y
            CrScPointsOut[m_iNumOfAuxPoints + 2 * m_iNumOfArcPoints+1, 1] = -m_fz_c;     // z

            // Point No. XX - Bottom Left Edge point
            CrScPointsOut[m_iNumOfAuxPoints + 2 * m_iNumOfArcPoints + 2, 0] = -m_fr_3;     // y
            CrScPointsOut[m_iNumOfAuxPoints + 2 * m_iNumOfArcPoints + 2, 1] = -m_fz_c;     // z

            // 4th radius - centre "5" (180-270 degrees)
            for (short i = 0; i < m_iNumOfArcPoints; i++)
            {
                CrScPointsOut[m_iNumOfAuxPoints + 2 * m_iNumOfArcPoints + 3 + i, 0] = CrScPointsOut[5, 0] - m_fr_1 + Geom2D.GetPositionX(m_fr_1, 0 + i * iRadiusAngle / m_iNumOfArcSegment);     // y
                CrScPointsOut[m_iNumOfAuxPoints + 2 * m_iNumOfArcPoints + 3 + i, 1] = CrScPointsOut[5, 1] - m_fr_1 + Geom2D.GetPositionY_CCW(m_fr_1, 0 + i * iRadiusAngle / m_iNumOfArcSegment); // z
            }

            // 1st radius - centre "0" (90-180 degrees)
            // Do not create last point of segment - it is already defined
            for (short i = 0; i < m_iNumOfArcPoints - 1; i++)
            {
                CrScPointsOut[m_iNumOfAuxPoints + 3 * m_iNumOfArcPoints + 3 + i, 0] = CrScPointsOut[0, 0] + Geom2D.GetPositionX(m_fr_2, 90 + i * iRadiusAngle / m_iNumOfArcSegment);       // y
                CrScPointsOut[m_iNumOfAuxPoints + 3 * m_iNumOfArcPoints + 3 + i, 1] = CrScPointsOut[0, 1] + Geom2D.GetPositionY_CW(m_fr_2, 90 + i * iRadiusAngle / m_iNumOfArcSegment);    // z
            }
        }

        //----------------------------------------------------------------------------
        // 2 - Two radii at flanges tips, tapered or parallel flanges, optional tapered web (4 auxiliary points)

        void CalcCrSc_Coord_T_MS_2()
        {
            // Fill Point Array Data in LCS (Local Coordinate System of Cross-Section, horizontal y, vertical - z)

            // auxiliary nodes

            //short m_iNumOfAuxPoints = 4;

            // Point No. 1
            CrScPointsOut[0, 0] = -m_fb / 2f + m_fr_2;    // y
            CrScPointsOut[0, 1] = m_fh - m_fz_c;          // z

            // Point No. 2
            CrScPointsOut[1, 0] = -m_fb / 2f + m_fr_2 + (m_fb - (2 * m_fr_3 + 4 * (m_ft_w - 2 * m_fr_3)) - 2 * m_fr_2) / 2.0f;   // y
            CrScPointsOut[1, 1] = CrScPointsOut[0, 1];       // z

            // Point No. 3
            CrScPointsOut[2, 0] = -CrScPointsOut[1, 0];      // y
            CrScPointsOut[2, 1] = CrScPointsOut[0, 1];       // z

            // Point No. 4
            CrScPointsOut[3, 0] = -CrScPointsOut[0, 0];      // y
            CrScPointsOut[3, 1] = CrScPointsOut[0, 1];       // z

            // Surface points

            // Point No. 5 - 1st Edge point - upper left
            CrScPointsOut[m_iNumOfAuxPoints, 0] = -m_fb / 2.0f;    // y
            CrScPointsOut[m_iNumOfAuxPoints, 1] = m_fh - m_fz_c;   // z

            int iRadiusAngle = 90; // Radius Angle

            // 2nd radius - centre "3" (0-90 degrees)
            for (short i = 0; i < m_iNumOfArcPoints; i++)
            {
                CrScPointsOut[m_iNumOfAuxPoints + i + 1, 0] = CrScPointsOut[3, 0] + Geom2D.GetPositionX(m_fr_2, 0 + i * iRadiusAngle / m_iNumOfArcSegment);     // y
                CrScPointsOut[m_iNumOfAuxPoints + i + 1, 1] = CrScPointsOut[3, 1] + Geom2D.GetPositionY_CW(m_fr_2, 0 + i * iRadiusAngle / m_iNumOfArcSegment);  // z
            }

            // Point No. XX - Upper right edge web point
            CrScPointsOut[m_iNumOfAuxPoints + m_iNumOfArcPoints + 1, 0] = CrScPointsOut[2, 0];      // y
            CrScPointsOut[m_iNumOfAuxPoints + m_iNumOfArcPoints + 1, 1] = -m_fz_c + m_fd;         // z

            // Point No. XX - Bottom right edge web point
            CrScPointsOut[m_iNumOfAuxPoints + m_iNumOfArcPoints + 2, 0] = m_fr_3;      // y
            CrScPointsOut[m_iNumOfAuxPoints + m_iNumOfArcPoints + 2, 1] = -m_fz_c;     // z

            // Point No. XX - Bottom left edge web point
            CrScPointsOut[m_iNumOfAuxPoints + m_iNumOfArcPoints + 3, 0] = -m_fr_3;     // y
            CrScPointsOut[m_iNumOfAuxPoints + m_iNumOfArcPoints + 3, 1] = -m_fz_c;     // z

            // Point No. XX - Upper left edge web point
            CrScPointsOut[m_iNumOfAuxPoints + m_iNumOfArcPoints + 4, 0] = CrScPointsOut[1, 0];      // y
            CrScPointsOut[m_iNumOfAuxPoints + m_iNumOfArcPoints + 4, 1] = -m_fz_c + m_fd;         // z

            // 1st radius - centre "0" (90-180 degrees)
            // Do not create last point of segment - it is already defined
            for (short i = 0; i < m_iNumOfArcPoints - 1; i++)
            {
                CrScPointsOut[m_iNumOfAuxPoints + m_iNumOfArcPoints + 5 + i, 0] = CrScPointsOut[0, 0] + Geom2D.GetPositionX(m_fr_2, 90 + i * iRadiusAngle / m_iNumOfArcSegment);       // y
                CrScPointsOut[m_iNumOfAuxPoints + m_iNumOfArcPoints + 5 + i, 1] = CrScPointsOut[0, 1] + Geom2D.GetPositionY_CW(m_fr_2, 90 + i * iRadiusAngle / m_iNumOfArcSegment);    // z
            }
        }

        //----------------------------------------------------------------------------
        // 3 - Two radii at flanges roots, tapered or parallel flanges, optional tapered web (2 auxiliary points)

        void CalcCrSc_Coord_T_MS_3()
        {
            // Fill Point Array Data in LCS (Local Coordinate System of Cross-Section, horizontal y, vertical - z)

            // auxiliary nodes

            //short m_iNumOfAuxPoints = 2;

            // Point No. 1
            CrScPointsOut[0, 0] = -m_fb / 2f + (m_fb - (2 * m_fr_3 + 4 * (m_ft_w - 2 * m_fr_3))) / 2.0f;   // y
            CrScPointsOut[0, 1] = - m_fz_c + m_fd + m_fr_1;                                                         // z

            // Point No. 2
            CrScPointsOut[1, 0] = -CrScPointsOut[0, 0];          // y
            CrScPointsOut[1, 1] = CrScPointsOut[0, 1];           // z

            // Surface points

            // Point No. 3
            CrScPointsOut[2, 0] = -m_fb / 2f;                  // y
            CrScPointsOut[2, 1] = m_fh - m_fz_c;               // z

            // Point No. 4
            CrScPointsOut[3, 0] = -m_fb / 2f + (m_fb - (2 * m_fr_3 + 4 * (m_ft_w - 2 * m_fr_3)) - 2 * m_fr_1) / 2.0f;      // y
            CrScPointsOut[3, 1] = CrScPointsOut[2, 1];                                                      // z

            // Point No. 5
            CrScPointsOut[4, 0] = -CrScPointsOut[3, 0];          // y
            CrScPointsOut[4, 1] = CrScPointsOut[2, 1];           // z

            // Point No. 6
            CrScPointsOut[5, 0] = -CrScPointsOut[2, 0];          // y
            CrScPointsOut[5, 1] = CrScPointsOut[2, 1];           // z

            // Point No. 7
            CrScPointsOut[6, 0] = CrScPointsOut[5, 0];           // y
            CrScPointsOut[6, 1] = -m_fz_c + m_fd + m_fr_1 + m_fSlopeTaperFlange * ((m_fb - (2 * m_fr_3 + 4 * (m_ft_w - 2 * m_fr_3)) - 2 * m_fr_1) / 2.0f);   // z

            int iRadiusAngle = 90; // Radius Angle

            // 1st radius - centre "1" (90-180 degrees) - right side
            for (short i = 0; i < m_iNumOfArcPoints; i++)
            {
                CrScPointsOut[m_iNumOfAuxPoints + i + 5, 0] = CrScPointsOut[1, 0] + m_fr_1 + Geom2D.GetPositionX(m_fr_1, 90 + i * iRadiusAngle / m_iNumOfArcSegment);     // y
                CrScPointsOut[m_iNumOfAuxPoints + i + 5, 1] = CrScPointsOut[1, 1] - m_fr_1 + Geom2D.GetPositionY_CCW(m_fr_1, 90 + i * iRadiusAngle / m_iNumOfArcSegment); // z
            }

            // Point No. XX - Bottom Right Edge point
            CrScPointsOut[m_iNumOfAuxPoints + m_iNumOfArcPoints + 5, 0] = m_fr_3;      // y
            CrScPointsOut[m_iNumOfAuxPoints + m_iNumOfArcPoints + 5, 1] = -m_fz_c;     // z

            // Point No. XX - Bottom Left Edge point
            CrScPointsOut[m_iNumOfAuxPoints + m_iNumOfArcPoints + 6, 0] = -m_fr_3;     // y
            CrScPointsOut[m_iNumOfAuxPoints + m_iNumOfArcPoints + 6, 1] = -m_fz_c;     // z

            // 2nd radius - centre "0" (180-270 degrees) - left side
            for (short i = 0; i < m_iNumOfArcPoints; i++)
            {
                CrScPointsOut[m_iNumOfAuxPoints + m_iNumOfArcPoints + 7 + i, 0] = CrScPointsOut[0, 0] - m_fr_1 + Geom2D.GetPositionX(m_fr_1, 0 + i * iRadiusAngle / m_iNumOfArcSegment);     // y
                CrScPointsOut[m_iNumOfAuxPoints + m_iNumOfArcPoints + 7 + i, 1] = CrScPointsOut[0, 1] - m_fr_1 + Geom2D.GetPositionY_CCW(m_fr_1, 0 + i * iRadiusAngle / m_iNumOfArcSegment); // z
            }

            // Last point
            // Point No. XX
            CrScPointsOut[m_iNumOfAuxPoints + 2 * m_iNumOfArcPoints + 7, 0] = CrScPointsOut[2, 0];     // y
            CrScPointsOut[m_iNumOfAuxPoints + 2 * m_iNumOfArcPoints + 7, 1] = CrScPointsOut[6, 1];     // z
        }

        protected override void loadCrScIndices()
        {
            load_3_08_TriangelIndices(m_sShape, m_iNumOfAuxPoints, m_iNumOfArcSegment);
        }

        private void load_3_08_TriangelIndices(short sShape, int iAux, int iRadiusSegment)
        {
            int iRadiusPoints = iRadiusSegment + 1;

            TriangleIndices = new Int32Collection(100);

            if (sShape == 0) // 0 - Five radii, tapered flanges, optional tapered web (5+2 auxiliary points)
            {
                // Front Side / Forehead
                AddRectangleIndices_CW_1234(TriangleIndices, 0, 1, iAux + 5 * iRadiusPoints - 1, iAux + 5 * iRadiusPoints);
                AddRectangleIndices_CW_1234(TriangleIndices, 1, 2, iAux + 2 + iRadiusSegment, iAux + 5 * iRadiusPoints - 1);
                AddRectangleIndices_CW_1234(TriangleIndices, 2, 3, iAux + 1 + iRadiusSegment, iAux + 2 + iRadiusSegment);
                AddRectangleIndices_CW_1234(TriangleIndices, 6, 4, iAux + 2 * iRadiusPoints, iAux + 4 * iRadiusPoints);
                AddRectangleIndices_CW_1234(TriangleIndices, iAux + 4 * iRadiusPoints, iAux + 2 * iRadiusPoints, iAux + 2 * iRadiusPoints + 1, iAux + 4 * iRadiusPoints - 1);

                // Arc sectors
                // 1st SolidCircleSector
                AddSolidCircleSectorIndices(3, iAux + 1, iRadiusSegment, TriangleIndices, false);
                // 2nd SolidCircleSector
                AddSolidCircleSectorIndices(4, iAux + 1 + iRadiusPoints, iRadiusSegment, TriangleIndices, false);
                // 3rd SolidCircleSector
                AddSolidCircleSectorIndices(5, iAux + 1 + 2 * iRadiusPoints, 2 * iRadiusSegment, TriangleIndices, false);
                // 4th SolidCircleSector
                AddSolidCircleSectorIndices(6, iAux + 4 * iRadiusPoints, iRadiusSegment, TriangleIndices, false);
                // 5th SolidCircleSector
                AddSolidCircleSectorIndices(0, iAux + 5 * iRadiusPoints, iRadiusSegment - 1, TriangleIndices, false); // Segments number = iRadiusSegment-1

                // Last Triangle
                TriangleIndices.Add(0); // 1st Point
                TriangleIndices.Add(iAux); // 1st Point of Radii (1st after auxiliary)
                TriangleIndices.Add(iAux + 6 * iRadiusPoints - 2); // Last Point

                // Back Side

                int iPointNumbersOffset = iAux + 6 * iRadiusPoints - 1; // Number of nodes per section - Nodes offset

                AddRectangleIndices_CW_1234(TriangleIndices, iPointNumbersOffset + 0, iPointNumbersOffset + iAux + 5 * iRadiusPoints, iPointNumbersOffset + iAux + 5 * iRadiusPoints - 1, iPointNumbersOffset + 1);
                AddRectangleIndices_CW_1234(TriangleIndices, iPointNumbersOffset + 1, iPointNumbersOffset + iAux + 5 * iRadiusPoints - 1, iPointNumbersOffset + iAux + 2 + iRadiusSegment, iPointNumbersOffset + 2);
                AddRectangleIndices_CW_1234(TriangleIndices, iPointNumbersOffset + 2, iPointNumbersOffset + iAux + 2 + iRadiusSegment, iPointNumbersOffset + iAux + 1 + iRadiusSegment, iPointNumbersOffset + 3);
                AddRectangleIndices_CW_1234(TriangleIndices, iPointNumbersOffset + 6, iPointNumbersOffset + iAux + 4 * iRadiusPoints, iPointNumbersOffset + iAux + 2 * iRadiusPoints, iPointNumbersOffset + 4);
                AddRectangleIndices_CW_1234(TriangleIndices, iPointNumbersOffset + iAux + 4 * iRadiusPoints, iPointNumbersOffset + iAux + 4 * iRadiusPoints - 1, iPointNumbersOffset + iAux + 2 * iRadiusPoints + 1, iPointNumbersOffset + iAux + 2 * iRadiusPoints);

                // Arc sectors
                // 1st SolidCircleSector
                AddSolidCircleSectorIndices(iPointNumbersOffset + 3, iPointNumbersOffset + iAux + 1, iRadiusSegment, TriangleIndices, true);
                // 2nd SolidCircleSector
                AddSolidCircleSectorIndices(iPointNumbersOffset + 4, iPointNumbersOffset + iAux + 1 + iRadiusPoints, iRadiusSegment, TriangleIndices, true);
                // 3rd SolidCircleSector
                AddSolidCircleSectorIndices(iPointNumbersOffset + 5, iPointNumbersOffset + iAux + 1 + 2 * iRadiusPoints, 2 * iRadiusSegment, TriangleIndices, true);
                // 4th SolidCircleSector
                AddSolidCircleSectorIndices(iPointNumbersOffset + 6, iPointNumbersOffset + iAux + 4 * iRadiusPoints, iRadiusSegment, TriangleIndices, true);
                // 5th SolidCircleSector
                AddSolidCircleSectorIndices(iPointNumbersOffset + 0, iPointNumbersOffset + iAux + 5 * iRadiusPoints, iRadiusSegment - 1, TriangleIndices, true); // Segments number = iRadiusSegment-1

                // Last Triangle 
                TriangleIndices.Add(iPointNumbersOffset + 0); // 1st Point
                TriangleIndices.Add(iPointNumbersOffset + iAux + 6 * iRadiusPoints - 2); // Last Point
                TriangleIndices.Add(iPointNumbersOffset + iAux); // 1st Point of Radii (1st after auxiliary)

                // Shell
                DrawCaraLaterals(iAux, 6 * iRadiusPoints - 1, TriangleIndices);
            }
            else if (sShape == 1) // 1 - Four radii, tapered or parallel flanges, optional tapered web (4+2 auxiliary points)
            {
                // Front Side / Forehead
                AddRectangleIndices_CW_1234(TriangleIndices, 0, 1, iAux + 3 * iRadiusPoints + 2, iAux + 3 * iRadiusPoints + 3);
                AddRectangleIndices_CW_1234(TriangleIndices, 1, 2, iAux + 1 * iRadiusPoints + 1, iAux + 3 * iRadiusPoints + 2);
                AddRectangleIndices_CW_1234(TriangleIndices, 2, 3, iAux + 1 * iRadiusPoints, iAux + 1 * iRadiusPoints + 1);
                AddRectangleIndices_CW_1234(TriangleIndices, 5, 4, iAux + 2 * iRadiusPoints, iAux + 2 * iRadiusPoints + 3);
                AddRectangleIndices_CW_1234(TriangleIndices, iAux + 2 * iRadiusPoints + 3, iAux + 2 * iRadiusPoints, iAux + 2 * iRadiusPoints + 1, iAux + 2 * iRadiusPoints + 2);

                // Arc sectors
                // 1st SolidCircleSector
                AddSolidCircleSectorIndices(3, iAux + 1, iRadiusSegment, TriangleIndices, false);
                // 2nd SolidCircleSector
                AddSolidCircleSectorIndices(4, iAux + 1 + iRadiusPoints, iRadiusSegment, TriangleIndices, false);
                // 3rd SolidCircleSector
                AddSolidCircleSectorIndices(5, iAux + 2 * iRadiusPoints + 3, iRadiusSegment, TriangleIndices, false);
                // 4th SolidCircleSector
                AddSolidCircleSectorIndices(0, iAux + 3 * iRadiusPoints + 3, iRadiusSegment - 1, TriangleIndices, false); // Segments number = iRadiusSegment-1

                // Last Triangle 
                TriangleIndices.Add(0); // 1st Point
                TriangleIndices.Add(iAux); // 1st Point of Radii (1st after auxiliary)
                TriangleIndices.Add(iAux + 4 * iRadiusPoints + 1); // Last Point

                // Back Side

                int iPointNumbersOffset = iAux + 4 * iRadiusPoints + 2; // Number of nodes per section - Nodes offset

                // Changed orientation of triangles
                AddRectangleIndices_CCW_1234(TriangleIndices, iPointNumbersOffset + 0, iPointNumbersOffset + 1, iPointNumbersOffset + iAux + 3 * iRadiusPoints + 2, iPointNumbersOffset + iAux + 3 * iRadiusPoints + 3);
                AddRectangleIndices_CCW_1234(TriangleIndices, iPointNumbersOffset + 1, iPointNumbersOffset + 2, iPointNumbersOffset + iAux + 1 * iRadiusPoints + 1, iPointNumbersOffset + iAux + 3 * iRadiusPoints + 2);
                AddRectangleIndices_CCW_1234(TriangleIndices, iPointNumbersOffset + 2, iPointNumbersOffset + 3, iPointNumbersOffset + iAux + 1 * iRadiusPoints, iPointNumbersOffset + iAux + 1 * iRadiusPoints + 1);
                AddRectangleIndices_CCW_1234(TriangleIndices, iPointNumbersOffset + 5, iPointNumbersOffset + 4, iPointNumbersOffset + iAux + 2 * iRadiusPoints, iPointNumbersOffset + iAux + 2 * iRadiusPoints + 3);
                AddRectangleIndices_CCW_1234(TriangleIndices, iPointNumbersOffset + iAux + 2 * iRadiusPoints + 3, iPointNumbersOffset + iAux + 2 * iRadiusPoints, iPointNumbersOffset + iAux + 2 * iRadiusPoints + 1, iPointNumbersOffset + iAux + 2 * iRadiusPoints + 2);

                // Arc sectors
                // 1st SolidCircleSector
                AddSolidCircleSectorIndices(iPointNumbersOffset + 3, iPointNumbersOffset + iAux + 1, iRadiusSegment, TriangleIndices, true);
                // 2nd SolidCircleSector
                AddSolidCircleSectorIndices(iPointNumbersOffset + 4, iPointNumbersOffset + iAux + 1 + iRadiusPoints, iRadiusSegment, TriangleIndices, true);
                // 3rd SolidCircleSector
                AddSolidCircleSectorIndices(iPointNumbersOffset + 5, iPointNumbersOffset + iAux + 2 * iRadiusPoints + 3, iRadiusSegment, TriangleIndices, true);
                // 4th SolidCircleSector
                AddSolidCircleSectorIndices(iPointNumbersOffset + 0, iPointNumbersOffset + iAux + 3 * iRadiusPoints + 3, iRadiusSegment - 1, TriangleIndices, true); // Segments number = iRadiusSegment-1

                // Last Triangle 
                TriangleIndices.Add(iPointNumbersOffset + 0); // 1st Point
                TriangleIndices.Add(iPointNumbersOffset + iAux + 4 * iRadiusPoints + 1); // Last Point
                TriangleIndices.Add(iPointNumbersOffset + iAux); // 1st Point of Radii (1st after auxiliary)

                // Shell
                DrawCaraLaterals(iAux, 4 * iRadiusPoints + 2, TriangleIndices);
            }
            else if (sShape == 2) // 2 - Two radii at flanges tips, tapered or parallel flanges, optional tapered web (4 auxiliary points)
            {
                // Front Side / Forehead
                AddRectangleIndices_CW_1234(TriangleIndices, 0, 1, iAux + iRadiusPoints + 4, iAux + iRadiusPoints + 5);
                AddRectangleIndices_CW_1234(TriangleIndices, 1, 2, iAux + iRadiusPoints + 1, iAux + iRadiusPoints + 4);
                AddRectangleIndices_CW_1234(TriangleIndices, 2, 3, iAux + iRadiusPoints + 0, iAux + iRadiusPoints + 1);
                AddRectangleIndices_CW_1234(TriangleIndices, iAux + iRadiusPoints + 4, iAux + iRadiusPoints + 1, iAux + iRadiusPoints + 2, iAux + iRadiusPoints + 3);

                // Arc sectors
                // 1st SolidCircleSector
                AddSolidCircleSectorIndices(3, iAux + 1, iRadiusSegment, TriangleIndices, false);
                // 2nd SolidCircleSector
                AddSolidCircleSectorIndices(0, iAux + iRadiusPoints + 5, iRadiusSegment - 1, TriangleIndices, false); // Segments number = iRadiusSegment-1

                // Last Triangle 
                TriangleIndices.Add(0); // 1st Point
                TriangleIndices.Add(iAux); // 1st Point of Radii (1st after auxiliary)
                TriangleIndices.Add(iAux + 2 * iRadiusPoints + 3); // Last Point

                // Back Side

                int iPointNumbersOffset = iAux + 2 * iRadiusPoints + 4; // Number of nodes per section - Nodes offset

                // Changed orientation of triangles
                AddRectangleIndices_CCW_1234(TriangleIndices, iPointNumbersOffset + 0, iPointNumbersOffset + 1, iPointNumbersOffset + iAux + iRadiusPoints + 4, iPointNumbersOffset + iAux + iRadiusPoints + 5);
                AddRectangleIndices_CCW_1234(TriangleIndices, iPointNumbersOffset + 1, iPointNumbersOffset + 2, iPointNumbersOffset + iAux + iRadiusPoints + 1, iPointNumbersOffset + iAux + iRadiusPoints + 4);
                AddRectangleIndices_CCW_1234(TriangleIndices, iPointNumbersOffset + 2, iPointNumbersOffset + 3, iPointNumbersOffset + iAux + iRadiusPoints + 0, iPointNumbersOffset + iAux + iRadiusPoints + 1);
                AddRectangleIndices_CCW_1234(TriangleIndices, iPointNumbersOffset + iAux + iRadiusPoints + 4, iPointNumbersOffset + iAux + iRadiusPoints + 1, iPointNumbersOffset + iAux + iRadiusPoints + 2, iPointNumbersOffset + iAux + iRadiusPoints + 3);

                // Arc sectors
                // 1st SolidCircleSector
                AddSolidCircleSectorIndices(iPointNumbersOffset + 3, iPointNumbersOffset + iAux + 1, iRadiusSegment, TriangleIndices, true);
                // 2nd SolidCircleSector
                AddSolidCircleSectorIndices(iPointNumbersOffset + 0, iPointNumbersOffset + iAux + iRadiusPoints + 5, iRadiusSegment - 1, TriangleIndices, true); // Segments number = iRadiusSegment-1

                // Last Triangle
                TriangleIndices.Add(iPointNumbersOffset + 0); // 1st Point
                TriangleIndices.Add(iPointNumbersOffset + iAux + 2 * iRadiusPoints + 3); // Last Point
                TriangleIndices.Add(iPointNumbersOffset + iAux); // 1st Point of Radii (1st after auxiliary)

                // Shell
                DrawCaraLaterals(iAux, 2 * iRadiusPoints + 4, TriangleIndices);
            }
            else if (sShape == 3) // 3 - Two radii at flanges roots, tapered or parallel flanges, optional tapered web (2 auxiliary points)
            {
                // Front Side / Forehead
                AddRectangleIndices_CW_1234(TriangleIndices, 2, 3, iAux + 2 * iRadiusPoints + 6, iAux + 2 * iRadiusPoints + 7);
                AddRectangleIndices_CW_1234(TriangleIndices, 3, 4, iAux + iRadiusPoints, iAux + 2 * iRadiusPoints + 6);
                AddRectangleIndices_CW_1234(TriangleIndices, 4, 5, 6, 7);
                AddRectangleIndices_CW_1234(TriangleIndices, 0, 1, iAux + iRadiusPoints + 4, iAux + 2 * iRadiusPoints + 2);
                AddRectangleIndices_CW_1234(TriangleIndices, iAux + 2 * iRadiusPoints + 2, iAux + iRadiusPoints + 4, iAux + iRadiusPoints + 5, iAux + iRadiusPoints + 6);

                // Arc sectors
                // 1st SolidCircleSector
                AddSolidCircleSectorIndices(1, iAux + 5, iRadiusSegment, TriangleIndices, false);
                // 2nd SolidCircleSector
                AddSolidCircleSectorIndices(0, iAux + 2 * iRadiusPoints + 2, iRadiusSegment, TriangleIndices, false);

                // Back Side

                int iPointNumbersOffset = iAux + 2 * iRadiusPoints + 8; // Number of nodes per section - Nodes offset

                // Changed orientation of triangles
                AddRectangleIndices_CCW_1234(TriangleIndices, iPointNumbersOffset + 2, iPointNumbersOffset + 3, iPointNumbersOffset + iAux + 2 * iRadiusPoints + 6, iPointNumbersOffset + iAux + 2 * iRadiusPoints + 7);
                AddRectangleIndices_CCW_1234(TriangleIndices, iPointNumbersOffset + 3, iPointNumbersOffset + 4, iPointNumbersOffset + iAux + iRadiusPoints, iPointNumbersOffset + iAux + 2 * iRadiusPoints + 6);
                AddRectangleIndices_CCW_1234(TriangleIndices, iPointNumbersOffset + 4, iPointNumbersOffset + 5, iPointNumbersOffset + 6, iPointNumbersOffset + 7);
                AddRectangleIndices_CCW_1234(TriangleIndices, iPointNumbersOffset + 0, iPointNumbersOffset + 1, iPointNumbersOffset + iAux + iRadiusPoints + 4, iPointNumbersOffset + iAux + 2 * iRadiusPoints + 2);
                AddRectangleIndices_CCW_1234(TriangleIndices, iPointNumbersOffset + iAux + 2 * iRadiusPoints + 2, iPointNumbersOffset + iAux + iRadiusPoints + 4, iPointNumbersOffset + iAux + iRadiusPoints + 5, iPointNumbersOffset + iAux + iRadiusPoints + 6);

                // Arc sectors
                // 1st SolidCircleSector
                AddSolidCircleSectorIndices(iPointNumbersOffset + 1, iPointNumbersOffset + iAux + 5, iRadiusSegment, TriangleIndices, true);
                // 2nd SolidCircleSector
                AddSolidCircleSectorIndices(iPointNumbersOffset + 0, iPointNumbersOffset + iAux + 2 * iRadiusPoints + 2, iRadiusSegment, TriangleIndices, true);

                // Shell
                DrawCaraLaterals(iAux, 2 * iRadiusPoints + 8, TriangleIndices);
            }
            else //Exception
            { }
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
