using MATH;
using System;
using System.Windows.Media;

namespace CRSC
{
    public class CCrSc_3_00 : CSO
    {
        // Rolled doubly symmetric I section - parallel or tapered flanges

        //----------------------------------------------------------------------------
		private short m_sShape;       // Section shape

		public short ShapeType
		{
			get { return m_sShape; }
		}

        // Section shapes types
        // 0 - Eight radii, tapered or parallel flanges (12 auxiliary points)
        // 1 - Four radii at flanges tips, tapered or parallel flanges (8 auxiliary points), r1 = 0
        // 2 - Four radii at flanges roots, tapered or parallel flanges (4 auxiliary points), r2 = 0

        private float m_ft_f;               // Flange Thickness / Hrubka pasnice
        private float m_ft_w;               // Web Thickness  / Hrubka steny/stojiny
        private float m_fr_1;               // Radius
        private float m_fr_2;               // Radius - flange edge
        private float m_fd;                 // Web depth - straigth part
        private float m_fSlopeTaper;        // Slope of Taper
        private short m_iNumOfArcSegment;   // Number of Arc Segments
        private short m_iNumOfArcPoints;    // Number of Arc Points
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

        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------

        // Temp
        public CCrSc_3_00()
        { }


        // Shape 0
        public CCrSc_3_00(short sShapeType, short iNumOfArcSegment, float fh, float fb, float ft_f, float ft_w, float fr_1, float fr_2, float fd)
		{
            IsShapeSolid = true;
            m_sShape = sShapeType;
            m_iNumOfArcSegment = iNumOfArcSegment; //8;
		    m_iNumOfArcPoints = (short)(m_iNumOfArcSegment + 1); // Each arc is defined by number of segments + 1 point points
		    ITotNoPoints = (short)(8 * (short)m_iNumOfArcPoints + (8 + 4));

		    h = fh;
		    b = fb;
		    m_ft_f = ft_f;
		    m_ft_w = ft_w;
		    m_fr_1 = fr_1;
		    m_fr_2 = fr_2;
		    m_fd = fd;

		    m_fSlopeTaper = (((float)h - m_fd - 2*( m_fr_1 + m_fr_2)) / 2.0f) / (((float)b - m_ft_w - 2*(m_fr_1 + m_fr_2)) / 2.0f);
          
		    //  m_fSlopeTaper = 0.08f; // Default

		    // Create Array - allocate memory
            CrScPointsOut = new float[ITotNoPoints, 2];
		    // Fill Array Data
		    CalcCrSc_Coord_I_DS_0();

            // Fill list of indices for drawing of surface - triangles edges
            loadCrScIndices();
		}

        // Shape 1 and 2
        public CCrSc_3_00(short sShapeType, short iNumOfArcSegment, float fh, float fb, float ft_f, float ft_w, float fr, float fd)
		{
            IsShapeSolid = true;
            m_sShape = sShapeType;
            m_iNumOfArcSegment = iNumOfArcSegment; // 8;
		    m_iNumOfArcPoints = (short)(m_iNumOfArcSegment + 1); // Each arc is defined by number of segments + 1 point points

		    h = fh;
		    b = fb;
		    m_ft_f = ft_f;
		    m_ft_w = ft_w;
		    m_fd = fd;

		    // Number of points per section
		    if (m_sShape == 1)       // 1 - Four radii at flanges tips, tapered or parallel flanges (8 auxiliary points)
		    {
		        ITotNoPoints = (short)(4 * (short)m_iNumOfArcPoints + (4 + 4 + 4));
		        m_fr_1 = 0.0f;
		        m_fr_2 = fr;
		        m_fSlopeTaper = (((float)h - m_fd - 2 * m_fr_2) / 2.0f) / (((float)b - m_ft_w - 2 * m_fr_2) / 2.0f);
		    }
		    else if (m_sShape == 2)  // 2 -  Four radii at flanges roots, tapered or parallel flanges (4 auxiliary points)
		    {
		        ITotNoPoints = (short)(4 * (short)m_iNumOfArcPoints + (4 + 6 + 6));
		        m_fr_1 = fr;
		        m_fr_2 = 0.0f;
		        m_fSlopeTaper = (2 * (((float)h - m_fd - 2 * m_fr_1)/ 2.0f - m_ft_f)) / (((float)b - m_ft_w - 2 * m_fr_1) / 2.0f);
		    }
		    else // Exception
		    { }

		    //  m_fSlopeTaper = 0.08f; // Default

		    // Create Array - allocate memory
		    CrScPointsOut = new float[ITotNoPoints, 2];
		    // Fill Array Data

		    if (m_sShape == 1)       // 1 - Four radii at flanges tips, tapered or parallel flanges (8 auxiliary points)
		        CalcCrSc_Coord_I_DS_1();
		    else if (m_sShape == 2)  // 2 - Four radii at flanges roots, tapered or parallel flanges (4 auxiliary points)
		        CalcCrSc_Coord_I_DS_2();
		    else // Exception
		    { }

            // Fill list of indices for drawing of surface - triangles edges
            loadCrScIndices();
		}

		//----------------------------------------------------------------------------
		void CalcCrSc_Coord_I_DS_0() // 0 - Eight radii, tapered or parallel flanges (12 auxiliary points)
		{
		    // Fill Point Array Data in LCS (Local Coordinate System of Cross-Section, horizontal y, vertical - z)

		    // Auxialiary nodes

		    short iNumberAux = 8 + 4;

		    // Point No. 1
		    CrScPointsOut[0,0] = -(float)b / 2f + m_fr_2;    // y
            CrScPointsOut[0, 1] = (float)h / 2f;              // z

		    // Point No. 2
		    CrScPointsOut[1, 0] = -m_ft_w / 2.0f - m_fr_1;   // y
		    CrScPointsOut[1,1] = CrScPointsOut[0,1];           // z

		    // Point No. 3
		    CrScPointsOut[2,0] = -CrScPointsOut[1,0];      // y
		    CrScPointsOut[2,1] = CrScPointsOut[0,1];       // z

		    // Point No. 4
		    CrScPointsOut[3,0] = -CrScPointsOut[0,0];      // y
		    CrScPointsOut[3,1] = CrScPointsOut[0,1];       // z

		    // Point No. 5
		    CrScPointsOut[4,0] = m_ft_w / 2.0f;         // y
            CrScPointsOut[4, 1] = CrScPointsOut[3, 1] - m_fr_2 - (((float)h - m_fd - 2 * (m_fr_1 + m_fr_2)) / 2.0f);     // z

		    // Point No. 6
		    CrScPointsOut[5,0] = CrScPointsOut[4,0];      // y
		    CrScPointsOut[5,1] = -CrScPointsOut[4,1];     // z

		    // Point No. 7
		    CrScPointsOut[6,0] = CrScPointsOut[3,0];      // y
		    CrScPointsOut[6,1] = -CrScPointsOut[3,1];     // z

		    // Point No. 8
		    CrScPointsOut[7,0] = CrScPointsOut[2,0];      // y
		    CrScPointsOut[7,1] = -CrScPointsOut[2,1];     // z

		    // Point No. 9
		    CrScPointsOut[8,0] = CrScPointsOut[1,0];      // y
		    CrScPointsOut[8,1] = -CrScPointsOut[1,1];     // z

		    // Point No. 10
		    CrScPointsOut[9,0] = CrScPointsOut[0,0];      // y
		    CrScPointsOut[9,1] = -CrScPointsOut[0,1];     // z

		    // Point No. 11
		    CrScPointsOut[10,0] = -CrScPointsOut[5,0];    // y
		    CrScPointsOut[10,1] = CrScPointsOut[5,1];     // z

		    // Point No. 12
		    CrScPointsOut[11, 0] = -CrScPointsOut[4, 0];  // y
		    CrScPointsOut[11, 1] = CrScPointsOut[4, 1];   // z

		    // Surface points

		    // Point No. 13 - 1st Edge point - upper left
            CrScPointsOut[iNumberAux, 0] = -(float)b / 2.0f;     // y
            CrScPointsOut[iNumberAux, 1] = (float)h / 2.0f;      // z

		    int iRadiusAngle = 90; // Radius Angle

		    // 2nd radius - centre "3" (0-90 degrees)
		    for (short i = 0; i < m_iNumOfArcPoints; i++)
		    {
		        CrScPointsOut[iNumberAux + i + 1, 0] = CrScPointsOut[3, 0] + Geom2D.GetPositionX(m_fr_2, 0 + i * iRadiusAngle / m_iNumOfArcSegment);     // y
		        CrScPointsOut[iNumberAux + i + 1, 1] = CrScPointsOut[3, 1] + Geom2D.GetPositionY_CW(m_fr_2, 0 + i * iRadiusAngle / m_iNumOfArcSegment);  // z
		    }

		    // 3rd radius - centre "4" (90-180 degrees)
		    for (short i = 0; i < m_iNumOfArcPoints; i++)
		    {
		        CrScPointsOut[iNumberAux + m_iNumOfArcPoints + i + 1, 0] = CrScPointsOut[4, 0] + m_fr_1 + Geom2D.GetPositionX(m_fr_1, 90 + i * iRadiusAngle / m_iNumOfArcSegment);     // y
		        CrScPointsOut[iNumberAux + m_iNumOfArcPoints + i + 1, 1] = CrScPointsOut[4, 1] - m_fr_1 + Geom2D.GetPositionY_CCW(m_fr_1, 90 + i * iRadiusAngle / m_iNumOfArcSegment); // z
		    }

		    // 4th radius - centre "5" (180-270 degrees)
		    for (short i = 0; i < m_iNumOfArcPoints; i++)
		    {
		        CrScPointsOut[iNumberAux + 2 * m_iNumOfArcPoints + i + 1, 0] = CrScPointsOut[5, 0] + m_fr_1 + Geom2D.GetPositionX(m_fr_1, 180 + i * iRadiusAngle / m_iNumOfArcSegment);     // y
		        CrScPointsOut[iNumberAux + 2 * m_iNumOfArcPoints + i + 1, 1] = CrScPointsOut[5, 1] + m_fr_1 + Geom2D.GetPositionY_CCW(m_fr_1, 180 + i * iRadiusAngle / m_iNumOfArcSegment); // z
		    }

		    // 5th radius - centre "6" (270-360 degrees)
		    for (short i = 0; i < m_iNumOfArcPoints; i++)
		    {
		        CrScPointsOut[iNumberAux + 3 * m_iNumOfArcPoints + i + 1, 0] = CrScPointsOut[6, 0] + Geom2D.GetPositionX(m_fr_2, 270 + i * iRadiusAngle / m_iNumOfArcSegment);     // y
		        CrScPointsOut[iNumberAux + 3 * m_iNumOfArcPoints + i + 1, 1] = CrScPointsOut[6, 1] + Geom2D.GetPositionY_CW(m_fr_2, 270 + i * iRadiusAngle / m_iNumOfArcSegment);  // z
		    }

		    // 6th radius - centre "9" (180-270 degrees)
		    for (short i = 0; i < m_iNumOfArcPoints; i++)
		    {
		        CrScPointsOut[iNumberAux + 4 * m_iNumOfArcPoints + i + 1, 0] = CrScPointsOut[9, 0] + Geom2D.GetPositionX(m_fr_2, 180 + i * iRadiusAngle / m_iNumOfArcSegment);     // y
		        CrScPointsOut[iNumberAux + 4 * m_iNumOfArcPoints + i + 1, 1] = CrScPointsOut[9, 1] + Geom2D.GetPositionY_CW(m_fr_2, 180 + i * iRadiusAngle / m_iNumOfArcSegment);  // z
		    }

		    // 7th radius - centre "10" (270-360 degrees)
		    for (short i = 0; i < m_iNumOfArcPoints; i++)
		    {
		        CrScPointsOut[iNumberAux + 5 * m_iNumOfArcPoints + i + 1, 0] = CrScPointsOut[10, 0] - m_fr_1 + Geom2D.GetPositionX(m_fr_1, 270 + i * iRadiusAngle / m_iNumOfArcSegment);     // y
		        CrScPointsOut[iNumberAux + 5 * m_iNumOfArcPoints + i + 1, 1] = CrScPointsOut[10, 1] + m_fr_1 + Geom2D.GetPositionY_CCW(m_fr_1, 270 + i * iRadiusAngle / m_iNumOfArcSegment); // z
		    }

		    // 8th radius - centre "11" (0-90 degrees)
		    for (short i = 0; i < m_iNumOfArcPoints; i++)
		    {
		        CrScPointsOut[iNumberAux + 6 * m_iNumOfArcPoints + i + 1, 0] = CrScPointsOut[11, 0] - m_fr_1 + Geom2D.GetPositionX(m_fr_1, 0 + i * iRadiusAngle / m_iNumOfArcSegment);     // y
		        CrScPointsOut[iNumberAux + 6 * m_iNumOfArcPoints + i + 1, 1] = CrScPointsOut[11, 1] - m_fr_1 + Geom2D.GetPositionY_CCW(m_fr_1, 0 + i * iRadiusAngle / m_iNumOfArcSegment); // z
		    }

		    // 1st radius - centre "0" (90-180 degrees)
		    // Do not create last point of segment - it is already defined
		    for (short i = 0; i < m_iNumOfArcPoints - 1; i++)
		    {
		        CrScPointsOut[iNumberAux + 7 * m_iNumOfArcPoints + i + 1, 0] = CrScPointsOut[0, 0] + Geom2D.GetPositionX(m_fr_2, 90 + i * iRadiusAngle / m_iNumOfArcSegment);      // y
		        CrScPointsOut[iNumberAux + 7 * m_iNumOfArcPoints + i + 1, 1] = CrScPointsOut[0, 1] + Geom2D.GetPositionY_CW(m_fr_2, 90 + i * iRadiusAngle / m_iNumOfArcSegment);    // z
		    }
		}

		//----------------------------------------------------------------------------
		void CalcCrSc_Coord_I_DS_1() // 1 - Four radii at flanges tips, tapered or parallel flanges (8 auxiliary points)
		{
		    // Fill Point Array Data in LCS (Local Coordinate System of Cross-Section, horizontal y, vertical - z)

		    // Auxialiary nodes

		    short iNumberAux = 4 + 4;

		    // Point No. 1
		    CrScPointsOut[0, 0] = -(float)b / 2f + m_fr_2;    // y
		    CrScPointsOut[0, 1] = (float)h / 2f;              // z

		    // Point No. 2
		    CrScPointsOut[1, 0] = -m_ft_w / 2.0f;         // y
		    CrScPointsOut[1, 1] = CrScPointsOut[0, 1];           // z

		    // Point No. 3
		    CrScPointsOut[2, 0] = -CrScPointsOut[1, 0];      // y
		    CrScPointsOut[2, 1] = CrScPointsOut[0, 1];       // z

		    // Point No. 4
		    CrScPointsOut[3, 0] = -CrScPointsOut[0, 0];      // y
		    CrScPointsOut[3, 1] = CrScPointsOut[0, 1];     // z

		    // Point No. 5
		    CrScPointsOut[4, 0] = CrScPointsOut[3, 0];      // y
		    CrScPointsOut[4, 1] = -CrScPointsOut[3, 1];     // z

		    // Point No. 6
		    CrScPointsOut[5, 0] = CrScPointsOut[2, 0];      // y
		    CrScPointsOut[5, 1] = -CrScPointsOut[2, 1];     // z

		    // Point No. 7
		    CrScPointsOut[6, 0] = CrScPointsOut[1, 0];    // y
		    CrScPointsOut[6, 1] = -CrScPointsOut[1, 1];     // z

		    // Point No. 8
		    CrScPointsOut[7, 0] = CrScPointsOut[0, 0];  // y
		    CrScPointsOut[7, 1] = -CrScPointsOut[0, 1];   // z

		    // Surface points

		    // Point No. 13 - 1st Edge point - upper left
		    CrScPointsOut[iNumberAux, 0] = -(float)b / 2.0f;     // y
		    CrScPointsOut[iNumberAux, 1] = (float)h / 2.0f;      // z

		    int iRadiusAngle = 90; // Radius Angle

		    // 2nd radius - centre "3" (0-90 degrees)
		    for (short i = 0; i < m_iNumOfArcPoints; i++)
		    {
		        CrScPointsOut[iNumberAux + i + 1, 0] = CrScPointsOut[3, 0] + Geom2D.GetPositionX(m_fr_2, 0 + i * iRadiusAngle / m_iNumOfArcSegment);     // y
		        CrScPointsOut[iNumberAux + i + 1, 1] = CrScPointsOut[3, 1] + Geom2D.GetPositionY_CW(m_fr_2, 0 + i * iRadiusAngle / m_iNumOfArcSegment);  // z
		    }

		    // Point No. XX
		    CrScPointsOut[iNumberAux + m_iNumOfArcPoints + 1, 0] = m_ft_w / 2.0f;      // y
		    CrScPointsOut[iNumberAux + m_iNumOfArcPoints + 1, 1] = CrScPointsOut[0, 1] - (((float)h - m_fd - 2 * +m_fr_2) / 2.0f); // z

		    // Point No. XX
		    CrScPointsOut[iNumberAux + m_iNumOfArcPoints + 2, 0] = m_ft_w / 2.0f;         // y
		    CrScPointsOut[iNumberAux + m_iNumOfArcPoints + 2, 1] = -CrScPointsOut[iNumberAux + m_iNumOfArcPoints + 1, 1];     // z

		    // 3rd radius - centre "4" (270-360 degrees)
		    for (short i = 0; i < m_iNumOfArcPoints; i++)
		    {
		        CrScPointsOut[iNumberAux + m_iNumOfArcPoints + i + 3, 0] = CrScPointsOut[4, 0] + Geom2D.GetPositionX(m_fr_2, 270 + i * iRadiusAngle / m_iNumOfArcSegment);     // y
		        CrScPointsOut[iNumberAux + m_iNumOfArcPoints + i + 3, 1] = CrScPointsOut[4, 1] + Geom2D.GetPositionY_CW(m_fr_2, 270 + i * iRadiusAngle / m_iNumOfArcSegment); // z
		    }

		    // 4th radius - centre "7" (180-270 degrees)
		    for (short i = 0; i < m_iNumOfArcPoints; i++)
		    {
		        CrScPointsOut[iNumberAux + 2 * m_iNumOfArcPoints + i + 3, 0] = CrScPointsOut[7, 0] + Geom2D.GetPositionX(m_fr_2, 180 + i * iRadiusAngle / m_iNumOfArcSegment);     // y
		        CrScPointsOut[iNumberAux + 2 * m_iNumOfArcPoints + i + 3, 1] = CrScPointsOut[7, 1] + Geom2D.GetPositionY_CW(m_fr_2, 180 + i * iRadiusAngle / m_iNumOfArcSegment); // z
		    }

		    // Point No. XX
		    CrScPointsOut[iNumberAux + 2 * m_iNumOfArcPoints + 8, 0] = - m_ft_w / 2.0f;         // y
		    CrScPointsOut[iNumberAux + 2 * m_iNumOfArcPoints + 8, 1] = CrScPointsOut[iNumberAux + m_iNumOfArcPoints + 2, 1];     // z

		    // Point No. XX
		    CrScPointsOut[iNumberAux + 2 * m_iNumOfArcPoints + 9, 0] = - m_ft_w / 2.0f;         // y
		    CrScPointsOut[iNumberAux + 2 * m_iNumOfArcPoints + 9, 1] = CrScPointsOut[iNumberAux + m_iNumOfArcPoints + 1, 1];     // z

		    // 1st radius - centre "0" (90-180 degrees)
		    // Do not create last point of segment - it is already defined
		    for (short i = 0; i < m_iNumOfArcPoints - 1; i++)
		    {
		        CrScPointsOut[iNumberAux + 2 * m_iNumOfArcPoints + i + 10, 0] = CrScPointsOut[0, 0] + Geom2D.GetPositionX(m_fr_2, 90 + i * iRadiusAngle / m_iNumOfArcSegment);      // y
		        CrScPointsOut[iNumberAux + 2 * m_iNumOfArcPoints + i + 10, 1] = CrScPointsOut[0, 1] + Geom2D.GetPositionY_CW(m_fr_2, 90 + i * iRadiusAngle / m_iNumOfArcSegment);    // z
		    }
		}

		//----------------------------------------------------------------------------
		void CalcCrSc_Coord_I_DS_2() // 2 - Four radii at flanges roots, tapered or parallel flanges (4 auxiliary points)
		{
		    // Fill Point Array Data in LCS (Local Coordinate System of Cross-Section, horizontal y, vertical - z)

		    // Auxialiary nodes

		    short iNumberAux = 4;

		    // Point No. 1
		    CrScPointsOut[0, 0] = -m_ft_w / 2f;           // y
		    CrScPointsOut[0, 1] = m_fd  /2.0f + m_fr_1;   // z

		    // Point No. 2
		    CrScPointsOut[1, 0] = -CrScPointsOut[0, 0];     // y
		    CrScPointsOut[1, 1] = CrScPointsOut[0, 1];      // z

		    // Point No. 3
		    CrScPointsOut[2, 0] = CrScPointsOut[1, 0];      // y
		    CrScPointsOut[2, 1] = -CrScPointsOut[0, 1];     // z

		    // Point No. 4
		    CrScPointsOut[3, 0] = CrScPointsOut[0, 0];      // y
		    CrScPointsOut[3, 1] = -CrScPointsOut[0, 1];     // z

		    // Surface points

		    // Point No. 5
		    CrScPointsOut[4, 0] = (float)-b / 2.0f;           // y
		    CrScPointsOut[4, 1] = (float)h / 2.0f;           // z

		    // Point No. 6
		    CrScPointsOut[5, 0] = -m_ft_w / 2.0f - m_fr_1; ;  // y
		    CrScPointsOut[5, 1] = CrScPointsOut[4, 1];         // z

		    // Point No. 7
		    CrScPointsOut[6, 0] = -CrScPointsOut[5, 0];    // y
		    CrScPointsOut[6, 1] = CrScPointsOut[5, 1];     // z

		    // Point No. 8
		    CrScPointsOut[7, 0] = -CrScPointsOut[4, 0];     // y
		    CrScPointsOut[7, 1] = CrScPointsOut[4, 1];     // z

		    // Point No. 9
		    CrScPointsOut[8, 0] = -CrScPointsOut[4, 0];      // y
		    CrScPointsOut[8, 1] = m_fd / 2 + m_fr_1 +  (2 * (((float)h - m_fd - 2 * m_fr_1) / 2.0f - m_ft_f));     // z

		    int iRadiusAngle = 90; // Radius Angle

		    // 1st radius - centre "1" (90-180 degrees)
		    for (short i = 0; i < m_iNumOfArcPoints; i++)
		    {
		        CrScPointsOut[iNumberAux + i + 4 + 1, 0] = CrScPointsOut[1, 0] + m_fr_1 + Geom2D.GetPositionX(m_fr_1, 90 + i * iRadiusAngle / m_iNumOfArcSegment);     // y
		        CrScPointsOut[iNumberAux + i + 4 + 1, 1] = CrScPointsOut[1, 1] - m_fr_1 + Geom2D.GetPositionY_CCW(m_fr_1, 90 + i * iRadiusAngle / m_iNumOfArcSegment); // z
		    }

		    // 2nd radius - centre "2" (180-270 degrees)
		    for (short i = 0; i < m_iNumOfArcPoints; i++)
		    {
		        CrScPointsOut[iNumberAux + m_iNumOfArcPoints + i + 4 + 1, 0] = CrScPointsOut[2, 0] + m_fr_1 + Geom2D.GetPositionX(m_fr_1, 180 + i * iRadiusAngle / m_iNumOfArcSegment);     // y
		        CrScPointsOut[iNumberAux + m_iNumOfArcPoints + i + 4 + 1, 1] = CrScPointsOut[2, 1] + m_fr_1 + Geom2D.GetPositionY_CCW(m_fr_1, 180 + i * iRadiusAngle / m_iNumOfArcSegment); // z
		    }

		    // Point No. XX
		    CrScPointsOut[iNumberAux + 2 * m_iNumOfArcPoints + 5, 0] = CrScPointsOut[8, 0];         // y
		    CrScPointsOut[iNumberAux + 2 * m_iNumOfArcPoints + 5, 1] = -CrScPointsOut[8, 1];        // z

		    // Point No. XX
		    CrScPointsOut[iNumberAux + 2 * m_iNumOfArcPoints + 6, 0] = CrScPointsOut[7, 0];         // y
		    CrScPointsOut[iNumberAux + 2 * m_iNumOfArcPoints + 6, 1] = -CrScPointsOut[7, 1];        // z

		    // Point No. XX
		    CrScPointsOut[iNumberAux + 2 * m_iNumOfArcPoints + 7, 0] = CrScPointsOut[6, 0];         // y
		    CrScPointsOut[iNumberAux + 2 * m_iNumOfArcPoints + 7, 1] = -CrScPointsOut[6, 1];        // z

		    // Point No. XX
		    CrScPointsOut[iNumberAux + 2 * m_iNumOfArcPoints + 8, 0] = CrScPointsOut[5, 0];         // y
		    CrScPointsOut[iNumberAux + 2 * m_iNumOfArcPoints + 8, 1] = -CrScPointsOut[5, 1];        // z

		    // Point No. XX
		    CrScPointsOut[iNumberAux + 2 * m_iNumOfArcPoints + 9, 0] = CrScPointsOut[4, 0];         // y
		    CrScPointsOut[iNumberAux + 2 * m_iNumOfArcPoints + 9, 1] = -CrScPointsOut[4, 1];        // z

		    // Point No. XX
		    CrScPointsOut[iNumberAux + 2 * m_iNumOfArcPoints + 10, 0] = CrScPointsOut[4, 0];       // y
		    CrScPointsOut[iNumberAux + 2 * m_iNumOfArcPoints + 10, 1] = -CrScPointsOut[8, 1];      // z

		    // 3rd radius - centre "3" (270-360 degrees)
		    for (short i = 0; i < m_iNumOfArcPoints; i++)
		    {
		        CrScPointsOut[iNumberAux + 2 * m_iNumOfArcPoints + i + 11, 0] = CrScPointsOut[3, 0] - m_fr_1 + Geom2D.GetPositionX(m_fr_1, 270 + i * iRadiusAngle / m_iNumOfArcSegment);     // y
		        CrScPointsOut[iNumberAux + 2 * m_iNumOfArcPoints + i + 11, 1] = CrScPointsOut[3, 1] + m_fr_1 + Geom2D.GetPositionY_CCW(m_fr_1, 270 + i * iRadiusAngle / m_iNumOfArcSegment); // z
		    }

		    // 4th radius - centre "0" (0-90 degrees)
		    for (short i = 0; i < m_iNumOfArcPoints; i++)
		    {
		        CrScPointsOut[iNumberAux + 3 * m_iNumOfArcPoints + i + 11, 0] = CrScPointsOut[0, 0] - m_fr_1 + Geom2D.GetPositionX(m_fr_1, 0 + i * iRadiusAngle / m_iNumOfArcSegment);     // y
		        CrScPointsOut[iNumberAux + 3 * m_iNumOfArcPoints + i + 11, 1] = CrScPointsOut[0, 1] - m_fr_1 + Geom2D.GetPositionY_CCW(m_fr_1, 0 + i * iRadiusAngle / m_iNumOfArcSegment); // z
		    }

		    // Point No. XX
		    CrScPointsOut[iNumberAux + 4 * m_iNumOfArcPoints + 11, 0] = CrScPointsOut[4, 0];       // y
		    CrScPointsOut[iNumberAux + 4 * m_iNumOfArcPoints + 11, 1] = CrScPointsOut[8, 1];        // z

		}

        protected override void loadCrScIndices()
        {
            int iAux = 0;
            loadCrScIndices_00_01(m_sShape, iAux, m_iNumOfArcSegment);
        }

        public void loadCrScIndices_00_01(short sShapeID, int iAux, int iNumOfArcSegment)
        {
            // List
            // const int secNum = iAux + iRadiusPoints * 8;  // Number of points in section (2D)
            // load_3_00_TriangelIndices(0, 12, 8); // Shape ID, number of auxiliary points , number of segments of arc
            // load_3_00_TriangelIndices(1, 8, 4); // Shape ID, number of auxiliary points , number of segments of arc
            // load_3_00_TriangelIndices(2, 4, 4); // Shape ID, number of auxiliary points , number of segments of arc


            int iRadiusPoints = m_iNumOfArcSegment + 1;

            TriangleIndices = new Int32Collection(120);

            if (sShapeID == 0)
            {
                iAux = 12;

                // Front Side / Forehead
                AddRectangleIndices_CW_1234(TriangleIndices, 0, 1, iAux + 7 + 7 * iNumOfArcSegment, iAux + 8 + 7 * iNumOfArcSegment);
                AddRectangleIndices_CW_1234(TriangleIndices, 1, 2, iAux + 2 + iNumOfArcSegment, iAux + 7 + 7 * iNumOfArcSegment);
                AddRectangleIndices_CW_1234(TriangleIndices, 2, 3, iAux + 1 + iNumOfArcSegment, iAux + 2 + iNumOfArcSegment);

                AddRectangleIndices_CW_1234(TriangleIndices, 4, 5, 10, 11);

                AddRectangleIndices_CW_1234(TriangleIndices, 6, 7, iAux + 3 + 3 * iNumOfArcSegment, iAux + 4 + 3 * iNumOfArcSegment);
                AddRectangleIndices_CW_1234(TriangleIndices, 7, 8, iAux + 6 + 5 * iNumOfArcSegment, iAux + 3 + 3 * iNumOfArcSegment);
                AddRectangleIndices_CW_1234(TriangleIndices, 8, 9, iAux + 5 + 5 * iNumOfArcSegment, iAux + 6 + 5 * iNumOfArcSegment);

                // Arc sectors
                // 1st SolidCircleSector
                AddSolidCircleSectorIndices(3, iAux + 1, iNumOfArcSegment, TriangleIndices, false);
                // 2nd SolidCircleSector
                AddSolidCircleSectorIndices(4, iAux + 1 + iRadiusPoints, iNumOfArcSegment, TriangleIndices, false);
                // 3rd SolidCircleSector
                AddSolidCircleSectorIndices(5, iAux + 1 + 2 * iRadiusPoints, iNumOfArcSegment, TriangleIndices, false);
                // 4th SolidCircleSector
                AddSolidCircleSectorIndices(6, iAux + 1 + 3 * iRadiusPoints, iNumOfArcSegment, TriangleIndices, false);
                // 5th SolidCircleSector
                AddSolidCircleSectorIndices(9, iAux + 1 + 4 * iRadiusPoints, iNumOfArcSegment, TriangleIndices, false);
                // 6th SolidCircleSector
                AddSolidCircleSectorIndices(10, iAux + 1 + 5 * iRadiusPoints, iNumOfArcSegment, TriangleIndices, false);
                // 7th SolidCircleSector
                AddSolidCircleSectorIndices(11, iAux + 1 + 6 * iRadiusPoints, iNumOfArcSegment, TriangleIndices, false);
                // 8th SolidCircleSector
                AddSolidCircleSectorIndices(0, iAux + 1 + 7 * iRadiusPoints, iNumOfArcSegment - 1, TriangleIndices, false); // Segments number = iNumOfArcSegment-1

                // Last Triangle 
                TriangleIndices.Add(0); // 1st Point
                TriangleIndices.Add(iAux); // 1st Point of Radii (1st after auxiliary)
                TriangleIndices.Add(iAux + 8 * iRadiusPoints - 1); // Last Point


                // Back Side 

                int iPointNumbersOffset = iAux + 8 * iRadiusPoints; // Number of nodes per section - Nodes offset

                AddRectangleIndices_CW_1234(TriangleIndices, iPointNumbersOffset + 0, iPointNumbersOffset + iAux + 8 + 7 * iNumOfArcSegment, iPointNumbersOffset + iAux + 7 + 7 * iNumOfArcSegment, iPointNumbersOffset + 1);
                AddRectangleIndices_CW_1234(TriangleIndices, iPointNumbersOffset + 1, iPointNumbersOffset + iAux + 7 + 7 * iNumOfArcSegment, iPointNumbersOffset + iAux + 2 + iNumOfArcSegment, iPointNumbersOffset + 2);
                AddRectangleIndices_CW_1234(TriangleIndices, iPointNumbersOffset + 2, iPointNumbersOffset + iAux + 2 + iNumOfArcSegment, iPointNumbersOffset + iAux + 1 + iNumOfArcSegment, iPointNumbersOffset + 3);

                AddRectangleIndices_CW_1234(TriangleIndices, iPointNumbersOffset + 4, iPointNumbersOffset + 11, iPointNumbersOffset + 10, iPointNumbersOffset + 5);

                AddRectangleIndices_CW_1234(TriangleIndices, iPointNumbersOffset + 6, iPointNumbersOffset + iAux + 4 + 3 * iNumOfArcSegment, iPointNumbersOffset + iAux + 3 + 3 * iNumOfArcSegment, iPointNumbersOffset + 7);
                AddRectangleIndices_CW_1234(TriangleIndices, iPointNumbersOffset + 7, iPointNumbersOffset + iAux + 3 + 3 * iNumOfArcSegment, iPointNumbersOffset + iAux + 6 + 5 * iNumOfArcSegment, iPointNumbersOffset + 8);
                AddRectangleIndices_CW_1234(TriangleIndices, iPointNumbersOffset + 8, iPointNumbersOffset + iAux + 6 + 5 * iNumOfArcSegment, iPointNumbersOffset + iAux + 5 + 5 * iNumOfArcSegment, iPointNumbersOffset + 9);

                // Arc sectors
                // 1st SolidCircleSector
                AddSolidCircleSectorIndices(iPointNumbersOffset + 3, iPointNumbersOffset + iAux + 1, iNumOfArcSegment, TriangleIndices, true);
                // 2nd SolidCircleSector
                AddSolidCircleSectorIndices(iPointNumbersOffset + 4, iPointNumbersOffset + iAux + 1 + iRadiusPoints, iNumOfArcSegment, TriangleIndices, true);
                // 3rd SolidCircleSector
                AddSolidCircleSectorIndices(iPointNumbersOffset + 5, iPointNumbersOffset + iAux + 1 + 2 * iRadiusPoints, iNumOfArcSegment, TriangleIndices, true);
                // 4th SolidCircleSector
                AddSolidCircleSectorIndices(iPointNumbersOffset + 6, iPointNumbersOffset + iAux + 1 + 3 * iRadiusPoints, iNumOfArcSegment, TriangleIndices, true);
                // 5th SolidCircleSector
                AddSolidCircleSectorIndices(iPointNumbersOffset + 9, iPointNumbersOffset + iAux + 1 + 4 * iRadiusPoints, iNumOfArcSegment, TriangleIndices, true);
                // 6th SolidCircleSector
                AddSolidCircleSectorIndices(iPointNumbersOffset + 10, iPointNumbersOffset + iAux + 1 + 5 * iRadiusPoints, iNumOfArcSegment, TriangleIndices, true);
                // 7th SolidCircleSector
                AddSolidCircleSectorIndices(iPointNumbersOffset + 11, iPointNumbersOffset + iAux + 1 + 6 * iRadiusPoints, iNumOfArcSegment, TriangleIndices, true);
                // 8th SolidCircleSector
                AddSolidCircleSectorIndices(iPointNumbersOffset + 0, iPointNumbersOffset + iAux + 1 + 7 * iRadiusPoints, iNumOfArcSegment - 1, TriangleIndices, true); // Segments number = iNumOfArcSegment-1

                // Last Triangle 
                TriangleIndices.Add(iPointNumbersOffset + 0); // 1st Point
                TriangleIndices.Add(iPointNumbersOffset + iAux + 8 * iRadiusPoints - 1); // Last Point
                TriangleIndices.Add(iPointNumbersOffset + iAux); // 1st Point of Radii (1st after auxiliary)


                // Shell
                DrawCaraLaterals(iAux, 8 * iRadiusPoints, TriangleIndices);
            }
            else if (sShapeID == 1)
            {
                iAux = 8;

                // Front Side / Forehead
                AddRectangleIndices_CW_1234(TriangleIndices, 0, 1, iAux + 4 + 3 * iRadiusPoints, iAux + 5 + 3 * iRadiusPoints);
                AddRectangleIndices_CW_1234(TriangleIndices, 1, 2, iAux + iRadiusPoints + 1, iAux + 4 + 3 * iRadiusPoints);
                AddRectangleIndices_CW_1234(TriangleIndices, 2, 3, iAux + iRadiusPoints, iAux + 1 + iRadiusPoints);

                AddRectangleIndices_CW_1234(TriangleIndices, iAux + 4 + 3 * iRadiusPoints, iAux + 1 + iRadiusPoints, iAux + 2 + iRadiusPoints, iAux + 3 + 3 * iRadiusPoints);

                AddRectangleIndices_CW_1234(TriangleIndices, 4, 5, iAux + 2 + iRadiusPoints, iAux + 3 + iRadiusPoints);
                AddRectangleIndices_CW_1234(TriangleIndices, 5, 6, iAux + 3 + 3 * iRadiusPoints, iAux + 2 + iRadiusPoints);
                AddRectangleIndices_CW_1234(TriangleIndices, 6, 7, iAux + 2 + 3 * iRadiusPoints, iAux + 3 + 3 * iRadiusPoints);

                // Arc sectors
                // 1st SolidCircleSector
                AddSolidCircleSectorIndices(3, iAux + 1, iNumOfArcSegment, TriangleIndices, false);
                // 2nd SolidCircleSector
                AddSolidCircleSectorIndices(4, iAux + 3 + iRadiusPoints, iNumOfArcSegment, TriangleIndices, false);
                // 3rd SolidCircleSector
                AddSolidCircleSectorIndices(7, iAux + 3 + 2 * iRadiusPoints, iNumOfArcSegment, TriangleIndices, false);
                // 4th SolidCircleSector
                AddSolidCircleSectorIndices(0, iAux + 5 + 3 * iRadiusPoints, iNumOfArcSegment - 1, TriangleIndices, false); // Segments number = iNumOfArcSegment-1

                // Last Triangle 
                TriangleIndices.Add(0); // 1st Point
                TriangleIndices.Add(iAux); // 1st Point of Radii (1st after auxiliary)
                TriangleIndices.Add(iAux + 4 * iRadiusPoints + 3); // Last Point

                // Back Side 

                int iPointNumbersOffset = iAux + 4 * iRadiusPoints + 4; // Number of nodes per section - Nodes offset

                AddRectangleIndices_CCW_1234(TriangleIndices, iPointNumbersOffset + 0, iPointNumbersOffset + 1, iPointNumbersOffset + iAux + 4 + 3 * iRadiusPoints, iPointNumbersOffset + iAux + 5 + 3 * iRadiusPoints);
                AddRectangleIndices_CCW_1234(TriangleIndices, iPointNumbersOffset + 1, iPointNumbersOffset + 2, iPointNumbersOffset + iAux + iRadiusPoints + 1, iPointNumbersOffset + iAux + 4 + 3 * iRadiusPoints);
                AddRectangleIndices_CCW_1234(TriangleIndices, iPointNumbersOffset + 2, iPointNumbersOffset + 3, iPointNumbersOffset + iAux + iRadiusPoints, iPointNumbersOffset + iAux + 1 + iRadiusPoints);

                AddRectangleIndices_CCW_1234(TriangleIndices, iPointNumbersOffset + iAux + 4 + 3 * iRadiusPoints, iPointNumbersOffset + iAux + 1 + iRadiusPoints, iPointNumbersOffset + iAux + 2 + iRadiusPoints, iPointNumbersOffset + iAux + 3 + 3 * iRadiusPoints);

                AddRectangleIndices_CCW_1234(TriangleIndices, iPointNumbersOffset + 4, iPointNumbersOffset + 5, iPointNumbersOffset + iAux + 2 + iRadiusPoints, iPointNumbersOffset + iAux + 3 + iRadiusPoints);
                AddRectangleIndices_CCW_1234(TriangleIndices, iPointNumbersOffset + 5, iPointNumbersOffset + 6, iPointNumbersOffset + iAux + 3 + 3 * iRadiusPoints, iPointNumbersOffset + iAux + 2 + iRadiusPoints);
                AddRectangleIndices_CCW_1234(TriangleIndices, iPointNumbersOffset + 6, iPointNumbersOffset + 7, iPointNumbersOffset + iAux + 2 + 3 * iRadiusPoints, iPointNumbersOffset + iAux + 3 + 3 * iRadiusPoints);

                // Arc sectors
                // 1st SolidCircleSector
                AddSolidCircleSectorIndices(iPointNumbersOffset + 3, iPointNumbersOffset + iAux + 1, iNumOfArcSegment, TriangleIndices, true);
                // 2nd SolidCircleSector
                AddSolidCircleSectorIndices(iPointNumbersOffset + 4, iPointNumbersOffset + iAux + 3 + iRadiusPoints, iNumOfArcSegment, TriangleIndices, true);
                // 3rd SolidCircleSector
                AddSolidCircleSectorIndices(iPointNumbersOffset + 7, iPointNumbersOffset + iAux + 3 + 2 * iRadiusPoints, iNumOfArcSegment, TriangleIndices, true);
                // 4th SolidCircleSector
                AddSolidCircleSectorIndices(iPointNumbersOffset + 0, iPointNumbersOffset + iAux + 5 + 3 * iRadiusPoints, iNumOfArcSegment - 1, TriangleIndices, true); // Segments number = iNumOfArcSegment-1

                // Last Triangle 
                TriangleIndices.Add(iPointNumbersOffset + 0); // 1st Point
                TriangleIndices.Add(iPointNumbersOffset + 4 * iRadiusPoints + 2 * 4 + 3); // Last Point 
                TriangleIndices.Add(iPointNumbersOffset + iAux); // 1st Point of Radii (1st after auxiliary)

                // Shell
                DrawCaraLaterals(iAux, 4 * iRadiusPoints + 4, TriangleIndices);
            }
            else if (sShapeID == 2)
            {
                iAux = 4;

                // Front Side / Forehead
                AddRectangleIndices_CW_1234(TriangleIndices, 4, 5, iAux + 10 + 4 * iRadiusPoints, iAux + 11 + 4 * iRadiusPoints);
                AddRectangleIndices_CW_1234(TriangleIndices, 5, 6, iAux + 5, iAux + 10 + 4 * iRadiusPoints);
                AddRectangleIndices_CW_1234(TriangleIndices, 6, 7, 8, 9);

                AddRectangleIndices_CW_1234(TriangleIndices, 0, 1, 2, 3);

                AddRectangleIndices_CW_1234(TriangleIndices, iAux + 4 + 2 * iRadiusPoints, iAux + 5 + 2 * iRadiusPoints, iAux + 6 + 2 * iRadiusPoints, iAux + 7 + 2 * iRadiusPoints);
                AddRectangleIndices_CW_1234(TriangleIndices, iAux + 11 + 2 * iRadiusPoints, iAux + 4 + 2 * iRadiusPoints, iAux + 7 + 2 * iRadiusPoints, iAux + 8 + 2 * iRadiusPoints);
                AddRectangleIndices_CW_1234(TriangleIndices, iAux + 8 + 2 * iRadiusPoints, iAux + 9 + 2 * iRadiusPoints, iAux + 10 + 2 * iRadiusPoints, iAux + 11 + 2 * iRadiusPoints);

                // Arc sectors
                // 1st SolidCircleSector
                AddSolidCircleSectorIndices(1, iAux + 5, iNumOfArcSegment, TriangleIndices, false);
                // 2nd SolidCircleSector
                AddSolidCircleSectorIndices(2, iAux + 5 + iRadiusPoints, iNumOfArcSegment, TriangleIndices, false);
                // 3rd SolidCircleSector
                AddSolidCircleSectorIndices(3, iAux + 11 + 2 * iRadiusPoints, iNumOfArcSegment, TriangleIndices, false);
                // 4th SolidCircleSector
                AddSolidCircleSectorIndices(0, iAux + 11 + 3 * iRadiusPoints, iNumOfArcSegment, TriangleIndices, false);

                // Back Side 

                int iPointNumbersOffset = iAux + 4 * iRadiusPoints + 6 + 6; // Number of nodes per section - Nodes offset

                AddRectangleIndices_CCW_1234(TriangleIndices, iPointNumbersOffset + 4, iPointNumbersOffset + 5, iPointNumbersOffset + iAux + 10 + 4 * iRadiusPoints, iPointNumbersOffset + iAux + 11 + 4 * iRadiusPoints);
                AddRectangleIndices_CCW_1234(TriangleIndices, iPointNumbersOffset + 5, iPointNumbersOffset + 6, iPointNumbersOffset + iAux + 5, iPointNumbersOffset + iAux + 10 + 4 * iRadiusPoints);
                AddRectangleIndices_CCW_1234(TriangleIndices, iPointNumbersOffset + 6, iPointNumbersOffset + 7, iPointNumbersOffset + 8, iPointNumbersOffset + 9);

                AddRectangleIndices_CCW_1234(TriangleIndices, iPointNumbersOffset + 0, iPointNumbersOffset + 1, iPointNumbersOffset + 2, iPointNumbersOffset + 3);

                AddRectangleIndices_CCW_1234(TriangleIndices, iPointNumbersOffset + iAux + 4 + 2 * iRadiusPoints, iPointNumbersOffset + iAux + 5 + 2 * iRadiusPoints, iPointNumbersOffset + iAux + 6 + 2 * iRadiusPoints, iPointNumbersOffset + iAux + 7 + 2 * iRadiusPoints);
                AddRectangleIndices_CCW_1234(TriangleIndices, iPointNumbersOffset + iAux + 11 + 2 * iRadiusPoints, iPointNumbersOffset + iAux + 4 + 2 * iRadiusPoints, iPointNumbersOffset + iAux + 7 + 2 * iRadiusPoints, iPointNumbersOffset + iAux + 8 + 2 * iRadiusPoints);
                AddRectangleIndices_CCW_1234(TriangleIndices, iPointNumbersOffset + iAux + 8 + 2 * iRadiusPoints, iPointNumbersOffset + iAux + 9 + 2 * iRadiusPoints, iPointNumbersOffset + iAux + 10 + 2 * iRadiusPoints, iPointNumbersOffset + iAux + 11 + 2 * iRadiusPoints);

                // Arc sectors
                // 1st SolidCircleSector
                AddSolidCircleSectorIndices(iPointNumbersOffset + 1, iPointNumbersOffset + iAux + 5, iNumOfArcSegment, TriangleIndices, true);
                // 2nd SolidCircleSector
                AddSolidCircleSectorIndices(iPointNumbersOffset + 2, iPointNumbersOffset + iAux + 5 + iRadiusPoints, iNumOfArcSegment, TriangleIndices, true);
                // 3rd SolidCircleSector
                AddSolidCircleSectorIndices(iPointNumbersOffset + 3, iPointNumbersOffset + iAux + 11 + 2 * iRadiusPoints, iNumOfArcSegment, TriangleIndices, true);
                // 4th SolidCircleSector
                AddSolidCircleSectorIndices(iPointNumbersOffset + 0, iPointNumbersOffset + iAux + 11 + 3 * iRadiusPoints, iNumOfArcSegment, TriangleIndices, true);

                // Shell
                DrawCaraLaterals(iAux, 4 * iRadiusPoints + 6 + 6, TriangleIndices);
            }
            else
            {
                throw new NotSupportedException(string.Format("Shape not supported: [{0}]", sShapeID));
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
