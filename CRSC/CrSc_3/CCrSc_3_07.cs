using MATH;
using System.Windows.Media;
using System.Collections.Generic;
using System.Windows;

namespace CRSC
{
    // Test cross-section class
    // Temporary Class - includes array of drawing points of cross-section in its coordinate system (LCS-for 2D yz)
    public class CCrSc_3_07 : CSO
    {
        // Rolled / Cold-formed rectangular / square hollow section

        //----------------------------------------------------------------------------
        private short m_sShape;       // Section shape

        // Section shapes types
        // 0 - Two radii, coincident centre points (4 auxialiary points)
        // 1 - Two radii, incoincident centre points (8 auxialiary points)
        // 2 - Outside radius = 0 (4 auxialiary points)
        // 3 - Inside radius = 0, coincident centre points (4 auxialiary points)
        // 4 - Inside radius = 0, incoincident centre points // Temp - not implemented
        // 5 - Both radii = 0 (0 auxialiary points)

        //private float Fh;                 // Depth - Height / Vyska
        //private float Fb;                 // Width  / Sirka
        private float m_ft;                 // Thickness / Hrubka
        private float m_fr_out;             // Radius outside / Polomer vonkajsi
        private float m_fr_in;              // Radius inside / Polomer vnutorny
        private short m_iNumOfArcSegment;   // Number of Arc Segments
        private short m_iNumOfArcPoints;    // Number of Arc Points
        private short m_iNumOfAuxPoints;    // Number of Auxialiary Points
        private short m_iNoPoints;          // Number of Cross-section Points for
        //public float[,] CrScPointsOut;     // Array of Outside Points and values in 2D
        //public float[,] CrScPointsIn;      // Array of Inside Points and values in 2D

        //----------------------------------------------------------------------------

        public short SShape
        {
            get { return m_sShape; }
            set { m_sShape = value; }
        }

        public float Ft
        {
            get { return m_ft; }
            set { m_ft = value; }
        }
        public float Fr_out
        {
            get { return m_fr_out; }
            set { m_fr_out = value; }
        }
        public float Fr_in
        {
            get { return m_fr_in; }
            set { m_fr_in = value; }
        }
        public short INoPoints
        {
            get { return m_iNoPoints; }
            set { m_iNoPoints = value; }
        }

        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        public CCrSc_3_07()  {   }
        public CCrSc_3_07(short sShape, float fh, float fb, float ft, float fr)
        {
            // 0 - Two radii, same centre point (4 auxialiary points)
            // 2 - Outside radius = 0
            // 4 - Inside radius = 0, incoincident centre points

            m_iNumOfArcSegment = 4;
            m_iNumOfArcPoints = (short)(m_iNumOfArcSegment + 1); // Each arc is defined by number of segments + 1 point points
            m_iNoPoints = (short)(4 * (short)m_iNumOfArcPoints + 4);

            m_sShape = sShape;
            h = fh;
            b = fb;
            m_ft = ft;

            if (m_sShape == 0)
            {
                m_fr_out = fr;
                m_fr_in = m_fr_out - ft;
            }
            else if (m_sShape == 2)
            {
               m_fr_out = 0f;
               m_fr_in = fr;
            }
            else if (m_sShape == 4)
            {
                m_fr_out = fr;
                m_fr_in = 0f;
            }

            INoPointsOut = INoPointsIn = INoPoints;
            // Create Array - allocate memory
            CrScPointsOut = new float[INoPoints, 2];
            CrScPointsIn = new float[INoPoints, 2];

            //CrScPointsOut = new List<Point>(INoPointsOut);
            //CrScPointsIn = new List<Point>(INoPointsIn);
            // Fill Array Data

            if (m_sShape == 0)       // Both radii, coincident centres
                CalcCrSc_Coord_0();
            else if (m_sShape == 2)  // Outside radius = 0
                CalcCrSc_Coord_2();
            else if (m_sShape == 4)  // Inside radius = 0
                CalcCrSc_Coord_4();

            // Fill list of indices for drawing of surface - triangles edges
            loadCrScIndices();
        }

        public CCrSc_3_07(short sShape, float fh, float fb, float ft, float fr_out, float fr_in)
        {
            // 1 - Two radii, diff centre point (8 auxialiary points)

            m_iNumOfArcSegment = 8;
            m_iNumOfArcPoints = (short)(m_iNumOfArcSegment + 1); // Each arc is defined by number of segments + 1 point points
            m_iNoPoints = (short)(4 * (short)m_iNumOfArcPoints + 4);

            m_sShape = sShape;
            h = fh;
            b = fb;
            m_ft = ft;
            m_fr_out = fr_out;
            m_fr_in = fr_in;

            INoPointsOut = INoPointsIn = INoPoints;
            // Create Array - allocate memory
            CrScPointsOut = new float[INoPoints, 2];
            CrScPointsIn = new float[INoPoints, 2];

            // Fill Array Data
            CalcCrSc_Coord_0();

            // Fill list of indices for drawing of surface - triangles edges
            loadCrScIndices();
        }

        public CCrSc_3_07(short sShape, float fh, float fb, float ft)
        {
            // 0 - Two radii, same centre point (4 auxialiary points)
            // 2 - Outside radius = 0  (4 auxialiary points)
            // 3 - Inside radius = 0, coincident centre points (no auxiliary points)
            // 5 - No radii (no auxiliary points)

            SShape = sShape;
            h = fh;
            b = fb;
            m_ft = ft;

            if (m_sShape == 0)
            {
                m_iNumOfArcSegment = 4;
                m_iNumOfArcPoints = (short)(m_iNumOfArcSegment + 1); // Each arc is defined by number of segments + 1 point points
                m_iNoPoints = (short)(4 * (short)m_iNumOfArcPoints + 4);

                m_fr_out = 2 * m_ft;
                m_fr_in = m_ft;
            }
            else if (m_sShape == 2)
            {
                m_iNumOfArcSegment = 4;
                m_iNumOfArcPoints = (short)(m_iNumOfArcSegment + 1); // Each arc is defined by number of segments + 1 point points
                m_iNoPoints = (short)(4 * (short)m_iNumOfArcPoints + 4);

                m_fr_out = 0f;
                m_fr_in = ft;
            }
            else if (m_sShape == 3)
            {
                m_iNumOfArcSegment = 4;
                m_iNumOfArcPoints = (short)(m_iNumOfArcSegment + 1); // Each arc is defined by number of segments + 1 point points
                m_iNoPoints = (short)(4 * (short)m_iNumOfArcPoints + 4);

                m_fr_out = m_ft; // Radius is not needed
                m_fr_in = 0f;
            }
            else if (m_sShape == 5)
            {
                m_iNumOfArcSegment = 0;
                m_iNumOfArcPoints = 0;
                m_iNoPoints = 8;

                m_fr_out = 0f;
                m_fr_in = 0f;
            }

            INoPointsOut = INoPointsIn = INoPoints;

            // Create Array - allocate memory
            CrScPointsOut = new float[INoPoints, 2];
            CrScPointsIn = new float[INoPoints, 2];

            // Fill Array Data
            if (m_sShape == 0) // Two radii with coincident centres - radii are defined only by thickness
            {
                // r_out = 2 * t
                // r_in = t

                m_iNumOfAuxPoints = 4;
                CalcCrSc_Coord_0();
            }
            else if (m_sShape == 2) // Outside radius = 0
            {
                // r_in = t

                m_iNumOfAuxPoints = 4;
                CalcCrSc_Coord_2();
            }
            else if (m_sShape == 3) // Inside radius = 0
            {
                // r_out = t

                // No inside points - set empty
                CrScPointsIn = null; // No Inside surface points (auxialiary points of outside surface - centre of radii are identical to inside points)

                m_iNumOfAuxPoints = 4;
                CalcCrSc_Coord_3();
            }
            else
            {
                m_iNumOfAuxPoints = 0;
                // No inside points - set empty
                CrScPointsIn = null; // No Inside surface points (auxialiary points of outside surface - centre of radii are identical to inside points)

                CCrSc_0_25 objTemp = new CCrSc_0_25((float)h, (float)b, m_ft, m_ft);
                CrScPointsOut = objTemp.CrScPointsOut;
            }

            // Fill list of indices for drawing of surface - triangles edges
            loadCrScIndices();
        }

        //----------------------------------------------------------------------------
        void CalcCrSc_Coord_0()
        {
            // Fill Point Array Data in LCS (Local Coordinate System of Cross-Section, horizontal y, vertical - z)

            //m_iNumberAux = 4;

            // Outside points
            // Auxialiary nodes per section

            // Point No. 1
            CrScPointsOut[0, 0] = (float)-b / 2f + m_fr_out;    // y
            CrScPointsOut[0, 1] = (float)h / 2f - m_fr_out;     // z

            // Point No. 2
            CrScPointsOut[1, 0] = -CrScPointsOut[0, 0];        // y
            CrScPointsOut[1, 1] = CrScPointsOut[0, 1];         // z

            // Point No. 3
            CrScPointsOut[2, 0] = -CrScPointsOut[0, 0];        // y
            CrScPointsOut[2, 1] = -CrScPointsOut[1, 1];        // z

            // Point No. 4
            CrScPointsOut[3, 0] = CrScPointsOut[0, 0];         // y
            CrScPointsOut[3, 1] = CrScPointsOut[2, 1];         // z

            // Surface points

            int iRadiusAngle = 90; // Radius Angle

            // 1st radius - centre "1" (180-270 degrees)
            for (short i = 0; i < m_iNumOfArcPoints; i++)
            {
                CrScPointsOut[m_iNumOfAuxPoints + i, 0] = CrScPointsOut[0, 0] + Geom2D.GetPositionX_deg(m_fr_out, 180 + i * iRadiusAngle / m_iNumOfArcSegment);     // y
                CrScPointsOut[m_iNumOfAuxPoints + i, 1] = CrScPointsOut[0, 1] + Geom2D.GetPositionY_CW_deg(m_fr_out, 180 + i * iRadiusAngle / m_iNumOfArcSegment);  // z
            }

            // 2nd radius - centre "2" (270-360 degrees)
            for (short i = 0; i < m_iNumOfArcPoints; i++)
            {
                CrScPointsOut[m_iNumOfAuxPoints + m_iNumOfArcPoints + i, 0] = CrScPointsOut[1, 0] + Geom2D.GetPositionX_deg(m_fr_out, 270 + i * iRadiusAngle / m_iNumOfArcSegment);     // y
                CrScPointsOut[m_iNumOfAuxPoints + m_iNumOfArcPoints + i, 1] = CrScPointsOut[1, 1] + Geom2D.GetPositionY_CW_deg(m_fr_out, 270 + i * iRadiusAngle / m_iNumOfArcSegment); // z
            }

            // 3rd radius - centre "3" (0-90 degrees)
            for (short i = 0; i < m_iNumOfArcPoints; i++)
            {
                CrScPointsOut[m_iNumOfAuxPoints + 2 * m_iNumOfArcPoints + i, 0] = CrScPointsOut[2, 0] + Geom2D.GetPositionX_deg(m_fr_out, 0 + i * iRadiusAngle / m_iNumOfArcSegment);     // y
                CrScPointsOut[m_iNumOfAuxPoints + 2 * m_iNumOfArcPoints + i, 1] = CrScPointsOut[2, 1] + Geom2D.GetPositionY_CW_deg(m_fr_out, 0 + i * iRadiusAngle / m_iNumOfArcSegment); // z
            }

            // 4th radius - centre "4" (90-180 degrees)
            for (short i = 0; i < m_iNumOfArcPoints; i++)
            {
                CrScPointsOut[m_iNumOfAuxPoints + 3 * m_iNumOfArcPoints + i, 0] = CrScPointsOut[3, 0] + Geom2D.GetPositionX_deg(m_fr_out, 90 + i * iRadiusAngle / m_iNumOfArcSegment);     // y
                CrScPointsOut[m_iNumOfAuxPoints + 3 * m_iNumOfArcPoints + i, 1] = CrScPointsOut[3, 1] + Geom2D.GetPositionY_CW_deg(m_fr_out, 90 + i * iRadiusAngle / m_iNumOfArcSegment); // z
            }

            // Inside points
            // Auxialiary nodes per section

            // Point No. 1
            CrScPointsIn[0, 0] = (float)-b / 2f + m_fr_in + m_ft;    // y
            CrScPointsIn[0, 1] = (float)h / 2f - m_fr_in - m_ft;     // z

            // Point No. 2
            CrScPointsIn[1, 0] = -CrScPointsIn[0, 0];        // y
            CrScPointsIn[1, 1] = CrScPointsIn[0, 1];         // z

            // Point No. 3
            CrScPointsIn[2, 0] = -CrScPointsIn[0, 0];        // y
            CrScPointsIn[2, 1] = -CrScPointsIn[1, 1];        // z

            // Point No. 4
            CrScPointsIn[3, 0] = CrScPointsIn[0, 0];         // y
            CrScPointsIn[3, 1] = CrScPointsIn[2, 1];         // z

            // Surface points

            // 1st radius - centre "1" (180-270 degrees)
            for (short i = 0; i < m_iNumOfArcPoints; i++)
            {
                CrScPointsIn[m_iNumOfAuxPoints + i, 0] = CrScPointsIn[0, 0] + Geom2D.GetPositionX_deg(m_fr_in, 180 + i * iRadiusAngle / m_iNumOfArcSegment);     // y
                CrScPointsIn[m_iNumOfAuxPoints + i, 1] = CrScPointsIn[0, 1] + Geom2D.GetPositionY_CW_deg(m_fr_in, 180 + i * iRadiusAngle / m_iNumOfArcSegment);  // z
            }

            // 2nd radius - centre "2" (270-360 degrees)
            for (short i = 0; i < m_iNumOfArcPoints; i++)
            {
                CrScPointsIn[m_iNumOfAuxPoints + m_iNumOfArcPoints + i, 0] = CrScPointsIn[1, 0] + Geom2D.GetPositionX_deg(m_fr_in, 270 + i * iRadiusAngle / m_iNumOfArcSegment);     // y
                CrScPointsIn[m_iNumOfAuxPoints + m_iNumOfArcPoints + i, 1] = CrScPointsIn[1, 1] + Geom2D.GetPositionY_CW_deg(m_fr_in, 270 + i * iRadiusAngle / m_iNumOfArcSegment);  // z
            }

            // 3rd radius - centre "3" (0-90 degrees)
            for (short i = 0; i < m_iNumOfArcPoints; i++)
            {
                CrScPointsIn[m_iNumOfAuxPoints + 2 * m_iNumOfArcPoints + i, 0] = CrScPointsIn[2, 0] + Geom2D.GetPositionX_deg(m_fr_in, 0 + i * iRadiusAngle / m_iNumOfArcSegment);     // y
                CrScPointsIn[m_iNumOfAuxPoints + 2 * m_iNumOfArcPoints + i, 1] = CrScPointsIn[2, 1] + Geom2D.GetPositionY_CW_deg(m_fr_in, 0 + i * iRadiusAngle / m_iNumOfArcSegment);  // z
            }

            // 4th radius - centre "4" (90-180 degrees)
            for (short i = 0; i < m_iNumOfArcPoints; i++)
            {
                CrScPointsIn[m_iNumOfAuxPoints + 3 * m_iNumOfArcPoints + i, 0] = CrScPointsIn[3, 0] + Geom2D.GetPositionX_deg(m_fr_in, 90 + i * iRadiusAngle / m_iNumOfArcSegment);     // y
                CrScPointsIn[m_iNumOfAuxPoints + 3 * m_iNumOfArcPoints + i, 1] = CrScPointsIn[3, 1] + Geom2D.GetPositionY_CW_deg(m_fr_in, 90 + i * iRadiusAngle / m_iNumOfArcSegment);  // z
            }
        }

        //----------------------------------------------------------------------------
        void CalcCrSc_Coord_2()
        {
            // Fill Point Array Data in LCS (Local Coordinate System of Cross-Section, horizontal y, vertical - z)

            //m_iNumOfAuxPoints = 4;

            // Number of segments should be even-numbered

            // Outside points
            // Auxialiary nodes per section

            // Point No. 1
            CrScPointsOut[0, 0] = (float)-b / 2f + m_fr_in + m_ft;    // y
            CrScPointsOut[0, 1] = (float)h / 2f - m_fr_in - m_ft;     // z

            // Point No. 2
            CrScPointsOut[1, 0] = -CrScPointsOut[0, 0];        // y
            CrScPointsOut[1, 1] = CrScPointsOut[0, 1];         // z

            // Point No. 3
            CrScPointsOut[2, 0] = -CrScPointsOut[0, 0];        // y
            CrScPointsOut[2, 1] = -CrScPointsOut[1, 1];        // z

            // Point No. 4
            CrScPointsOut[3, 0] = CrScPointsOut[0, 0];         // y
            CrScPointsOut[3, 1] = CrScPointsOut[2, 1];         // z

            // Surface points

            float fOutSegmLength = 2* (m_ft + m_fr_in) / m_iNumOfArcSegment;

            // 1st radius - centre "1"
            for (short i = 0; i < m_iNumOfArcPoints; i++)
            {
                if (i <= m_iNumOfArcPoints / 2) // Vertical
                {
                    CrScPointsOut[m_iNumOfAuxPoints + i, 0] = (float)-b / 2f;                                             // y
                    CrScPointsOut[m_iNumOfAuxPoints + i, 1] = (float)h / 2f - (m_iNumOfArcSegment/2 - i) * fOutSegmLength;  // z
                }
                else // Horizontal
                {
                    CrScPointsOut[m_iNumOfAuxPoints + i, 0] = (float)-b / 2f + (i - m_iNumOfArcSegment/2) * fOutSegmLength; // y      
                    CrScPointsOut[m_iNumOfAuxPoints + i, 1] = (float)h / 2f;                                              // z                
                }
            }

            // 2nd radius - centre "2"
            for (short i = 0; i < m_iNumOfArcPoints; i++)
            {
                if (i <= m_iNumOfArcPoints / 2) // Horizontal
                {
                    CrScPointsOut[m_iNumOfAuxPoints + m_iNumOfArcPoints + i, 0] = (float)b / 2f - (m_iNumOfArcSegment/2 - i) * fOutSegmLength; // y          
                    CrScPointsOut[m_iNumOfAuxPoints + m_iNumOfArcPoints + i, 1] = (float)h / 2f;                                             // z 
                }
                else // Vertical
                {
                    CrScPointsOut[m_iNumOfAuxPoints + m_iNumOfArcPoints + i, 0] = (float)b / 2f;                                             // y
                    CrScPointsOut[m_iNumOfAuxPoints + m_iNumOfArcPoints + i, 1] = (float)h / 2f - i/2 * fOutSegmLength;                        // z
                }
            }

            // 3rd radius - centre "3"
            for (short i = 0; i < m_iNumOfArcPoints; i++)
            {
                if (i <= m_iNumOfArcPoints / 2) // Vertical
                {
                    CrScPointsOut[m_iNumOfAuxPoints + 2*m_iNumOfArcPoints + i, 0] = (float)b / 2f;                                               // y
                    CrScPointsOut[m_iNumOfAuxPoints + 2 * m_iNumOfArcPoints + i, 1] = (float)-h / 2f + (m_iNumOfArcSegment/2 - i) * fOutSegmLength;  // z
                }
                else // Horizontal
                {
                    CrScPointsOut[m_iNumOfAuxPoints + 2 * m_iNumOfArcPoints + i, 0] = (float)b / 2f - i/2 * fOutSegmLength;                         // y
                    CrScPointsOut[m_iNumOfAuxPoints + 2 * m_iNumOfArcPoints + i, 1] = (float)-h / 2f;                                              // z                
                }
            }

            // 4th radius - centre "4"
            for (short i = 0; i < m_iNumOfArcPoints; i++)
            {
                if (i <= m_iNumOfArcPoints / 2) // Horizontal
                {
                    CrScPointsOut[m_iNumOfAuxPoints + 3 * m_iNumOfArcPoints + i, 0] = (float)-b / 2f + (m_iNumOfArcSegment/2 - i) * fOutSegmLength; // y                                    
                    CrScPointsOut[m_iNumOfAuxPoints + 3 * m_iNumOfArcPoints + i, 1] = (float)-h / 2f;                                             // z 
                }
                else // Vertical
                {
                    CrScPointsOut[m_iNumOfAuxPoints + 3 * m_iNumOfArcPoints + i, 0] = (float)-b / 2f;                                             // y
                    CrScPointsOut[m_iNumOfAuxPoints + 3 * m_iNumOfArcPoints + i, 1] = (float)-h / 2f + i/2 * fOutSegmLength;                       // z
                }
            }


            // Inside points
            // Auxialiary nodes per section

            // Point No. 1
            CrScPointsIn[0, 0] = (float)-b / 2f + m_fr_in + m_ft;    // y
            CrScPointsIn[0, 1] = (float)h / 2f - m_fr_in - m_ft;     // z

            // Point No. 2
            CrScPointsIn[1, 0] = -CrScPointsIn[0, 0];        // y
            CrScPointsIn[1, 1] = CrScPointsIn[0, 1];         // z

            // Point No. 3
            CrScPointsIn[2, 0] = -CrScPointsIn[0, 0];        // y
            CrScPointsIn[2, 1] = -CrScPointsIn[1, 1];        // z

            // Point No. 4
            CrScPointsIn[3, 0] = CrScPointsIn[0, 0];         // y
            CrScPointsIn[3, 1] = CrScPointsIn[2, 1];         // z

            // Surface points

            int iRadiusAngle = 90; // Radius Angle

            // 1st radius - centre "1" (180-270 degrees)
            for (short i = 0; i < m_iNumOfArcPoints; i++)
            {
                CrScPointsIn[m_iNumOfAuxPoints + i, 0] = CrScPointsIn[0, 0] + Geom2D.GetPositionX_deg(m_fr_in, 180 + i * iRadiusAngle / m_iNumOfArcSegment);     // y
                CrScPointsIn[m_iNumOfAuxPoints + i, 1] = CrScPointsIn[0, 1] + Geom2D.GetPositionY_CW_deg(m_fr_in, 180 + i * iRadiusAngle / m_iNumOfArcSegment);  // z
            }

            // 2nd radius - centre "2" (270-360 degrees)
            for (short i = 0; i < m_iNumOfArcPoints; i++)
            {
                CrScPointsIn[m_iNumOfAuxPoints + m_iNumOfArcPoints + i, 0] = CrScPointsIn[1, 0] + Geom2D.GetPositionX_deg(m_fr_in, 270 + i * iRadiusAngle / m_iNumOfArcSegment);     // y
                CrScPointsIn[m_iNumOfAuxPoints + m_iNumOfArcPoints + i, 1] = CrScPointsIn[1, 1] + Geom2D.GetPositionY_CW_deg(m_fr_in, 270 + i * iRadiusAngle / m_iNumOfArcSegment);  // z
            }

            // 3rd radius - centre "3" (0-90 degrees)
            for (short i = 0; i < m_iNumOfArcPoints; i++)
            {
                CrScPointsIn[m_iNumOfAuxPoints + 2 * m_iNumOfArcPoints + i, 0] = CrScPointsIn[2, 0] + Geom2D.GetPositionX_deg(m_fr_in, 0 + i * iRadiusAngle / m_iNumOfArcSegment);     // y
                CrScPointsIn[m_iNumOfAuxPoints + 2 * m_iNumOfArcPoints + i, 1] = CrScPointsIn[2, 1] + Geom2D.GetPositionY_CW_deg(m_fr_in, 0 + i * iRadiusAngle / m_iNumOfArcSegment);  // z
            }

            // 4th radius - centre "4" (90-180 degrees)
            for (short i = 0; i < m_iNumOfArcPoints; i++)
            {
                CrScPointsIn[m_iNumOfAuxPoints + 3 * m_iNumOfArcPoints + i, 0] = CrScPointsIn[3, 0] + Geom2D.GetPositionX_deg(m_fr_in, 90 + i * iRadiusAngle / m_iNumOfArcSegment);     // y
                CrScPointsIn[m_iNumOfAuxPoints + 3 * m_iNumOfArcPoints + i, 1] = CrScPointsIn[3, 1] + Geom2D.GetPositionY_CW_deg(m_fr_in, 90 + i * iRadiusAngle / m_iNumOfArcSegment);  // z
            }
        }

        //----------------------------------------------------------------------------
        void CalcCrSc_Coord_3()
        {
            // Fill Point Array Data in LCS (Local Coordinate System of Cross-Section, horizontal y, vertical - z)

            //m_iNumOfAuxPoints = 4;

            // Points
            // Auxialiary nodes for out side points and internal points

            // Point No. 1
            CrScPointsOut[0, 0] = (float)-b / 2f + m_fr_out;    // y
            CrScPointsOut[0, 1] = (float)h / 2f - m_fr_out;     // z

            // Point No. 2
            CrScPointsOut[1, 0] = -CrScPointsOut[0, 0];        // y
            CrScPointsOut[1, 1] = CrScPointsOut[0, 1];         // z

            // Point No. 3
            CrScPointsOut[2, 0] = -CrScPointsOut[0, 0];        // y
            CrScPointsOut[2, 1] = -CrScPointsOut[1, 1];        // z

            // Point No. 4
            CrScPointsOut[3, 0] = CrScPointsOut[0, 0];         // y
            CrScPointsOut[3, 1] = CrScPointsOut[2, 1];         // z

            // Surface points

            int iRadiusAngle = 90; // Radius Angle

            // 1st radius - centre "1" (180-270 degrees)
            for (short i = 0; i < m_iNumOfArcPoints; i++)
            {
                CrScPointsOut[m_iNumOfAuxPoints + i, 0] = CrScPointsOut[0, 0] + Geom2D.GetPositionX_deg(m_fr_out, 180 + i * iRadiusAngle / m_iNumOfArcSegment);     // y
                CrScPointsOut[m_iNumOfAuxPoints + i, 1] = CrScPointsOut[0, 1] + Geom2D.GetPositionY_CW_deg(m_fr_out, 180 + i * iRadiusAngle / m_iNumOfArcSegment);  // z
            }

            // 2nd radius - centre "2" (270-360 degrees)
            for (short i = 0; i < m_iNumOfArcPoints; i++)
            {
                CrScPointsOut[m_iNumOfAuxPoints + m_iNumOfArcPoints + i, 0] = CrScPointsOut[1, 0] + Geom2D.GetPositionX_deg(m_fr_out, 270 + i * iRadiusAngle / m_iNumOfArcSegment);     // y
                CrScPointsOut[m_iNumOfAuxPoints + m_iNumOfArcPoints + i, 1] = CrScPointsOut[1, 1] + Geom2D.GetPositionY_CW_deg(m_fr_out, 270 + i * iRadiusAngle / m_iNumOfArcSegment); // z
            }

            // 3rd radius - centre "3" (0-90 degrees)
            for (short i = 0; i < m_iNumOfArcPoints; i++)
            {
                CrScPointsOut[m_iNumOfAuxPoints + 2 * m_iNumOfArcPoints + i, 0] = CrScPointsOut[2, 0] + Geom2D.GetPositionX_deg(m_fr_out, 0 + i * iRadiusAngle / m_iNumOfArcSegment);     // y
                CrScPointsOut[m_iNumOfAuxPoints + 2 * m_iNumOfArcPoints + i, 1] = CrScPointsOut[2, 1] + Geom2D.GetPositionY_CW_deg(m_fr_out, 0 + i * iRadiusAngle / m_iNumOfArcSegment); // z
            }

            // 4th radius - centre "4" (90-180 degrees)
            for (short i = 0; i < m_iNumOfArcPoints; i++)
            {
                CrScPointsOut[m_iNumOfAuxPoints + 3 * m_iNumOfArcPoints + i, 0] = CrScPointsOut[3, 0] + Geom2D.GetPositionX_deg(m_fr_out, 90 + i * iRadiusAngle / m_iNumOfArcSegment);     // y
                CrScPointsOut[m_iNumOfAuxPoints + 3 * m_iNumOfArcPoints + i, 1] = CrScPointsOut[3, 1] + Geom2D.GetPositionY_CW_deg(m_fr_out, 90 + i * iRadiusAngle / m_iNumOfArcSegment); // z
            }
        }

        //void CalcCrSc_Coord_0()
        //{
        //    // Fill Point Array Data in LCS (Local Coordinate System of Cross-Section, horizontal y, vertical - z)
        //    float[,] CrScPointsOutArr = new float[ITotNoPoints, 2];
        //    //m_iNumberAux = 4;

        //    // Outside points
        //    // Auxialiary nodes per section

        //    // Point No. 1
        //    CrScPointsOutArr[0, 0] = (float)-b / 2f + m_fr_out;    // y
        //    CrScPointsOutArr[0, 1] = (float)h / 2f - m_fr_out;     // z

        //    // Point No. 2
        //    CrScPointsOutArr[1, 0] = -CrScPointsOutArr[0, 0];        // y
        //    CrScPointsOutArr[1, 1] = CrScPointsOutArr[0, 1];         // z

        //    // Point No. 3
        //    CrScPointsOutArr[2, 0] = -CrScPointsOutArr[0, 0];        // y
        //    CrScPointsOutArr[2, 1] = -CrScPointsOutArr[1, 1];        // z

        //    // Point No. 4
        //    CrScPointsOutArr[3, 0] = CrScPointsOutArr[0, 0];         // y
        //    CrScPointsOutArr[3, 1] = CrScPointsOutArr[2, 1];         // z

        //    // Surface points

        //    int iRadiusAngle = 90; // Radius Angle

        //    // 1st radius - centre "1" (180-270 degrees)
        //    for (short i = 0; i < m_iNumOfArcPoints; i++)
        //    {
        //        CrScPointsOutArr[m_iNumOfAuxPoints + i, 0] = CrScPointsOutArr[0, 0] + Geom2D.GetPositionX_deg(m_fr_out, 180 + i * iRadiusAngle / m_iNumOfArcSegment);     // y
        //        CrScPointsOutArr[m_iNumOfAuxPoints + i, 1] = CrScPointsOutArr[0, 1] + Geom2D.GetPositionY_CW_deg(m_fr_out, 180 + i * iRadiusAngle / m_iNumOfArcSegment);  // z
        //    }

        //    // 2nd radius - centre "2" (270-360 degrees)
        //    for (short i = 0; i < m_iNumOfArcPoints; i++)
        //    {
        //        CrScPointsOutArr[m_iNumOfAuxPoints + m_iNumOfArcPoints + i, 0] = CrScPointsOutArr[1, 0] + Geom2D.GetPositionX_deg(m_fr_out, 270 + i * iRadiusAngle / m_iNumOfArcSegment);     // y
        //        CrScPointsOutArr[m_iNumOfAuxPoints + m_iNumOfArcPoints + i, 1] = CrScPointsOutArr[1, 1] + Geom2D.GetPositionY_CW_deg(m_fr_out, 270 + i * iRadiusAngle / m_iNumOfArcSegment); // z
        //    }

        //    // 3rd radius - centre "3" (0-90 degrees)
        //    for (short i = 0; i < m_iNumOfArcPoints; i++)
        //    {
        //        CrScPointsOutArr[m_iNumOfAuxPoints + 2 * m_iNumOfArcPoints + i, 0] = CrScPointsOutArr[2, 0] + Geom2D.GetPositionX_deg(m_fr_out, 0 + i * iRadiusAngle / m_iNumOfArcSegment);     // y
        //        CrScPointsOutArr[m_iNumOfAuxPoints + 2 * m_iNumOfArcPoints + i, 1] = CrScPointsOutArr[2, 1] + Geom2D.GetPositionY_CW_deg(m_fr_out, 0 + i * iRadiusAngle / m_iNumOfArcSegment); // z
        //    }

        //    // 4th radius - centre "4" (90-180 degrees)
        //    for (short i = 0; i < m_iNumOfArcPoints; i++)
        //    {
        //        CrScPointsOutArr[m_iNumOfAuxPoints + 3 * m_iNumOfArcPoints + i, 0] = CrScPointsOutArr[3, 0] + Geom2D.GetPositionX_deg(m_fr_out, 90 + i * iRadiusAngle / m_iNumOfArcSegment);     // y
        //        CrScPointsOutArr[m_iNumOfAuxPoints + 3 * m_iNumOfArcPoints + i, 1] = CrScPointsOutArr[3, 1] + Geom2D.GetPositionY_CW_deg(m_fr_out, 90 + i * iRadiusAngle / m_iNumOfArcSegment); // z
        //    }

        //    // Inside points
        //    // Auxialiary nodes per section

        //    // Point No. 1
        //    CrScPointsIn[0, 0] = (float)-b / 2f + m_fr_in + m_ft;    // y
        //    CrScPointsIn[0, 1] = (float)h / 2f - m_fr_in - m_ft;     // z

        //    // Point No. 2
        //    CrScPointsIn[1, 0] = -CrScPointsIn[0, 0];        // y
        //    CrScPointsIn[1, 1] = CrScPointsIn[0, 1];         // z

        //    // Point No. 3
        //    CrScPointsIn[2, 0] = -CrScPointsIn[0, 0];        // y
        //    CrScPointsIn[2, 1] = -CrScPointsIn[1, 1];        // z

        //    // Point No. 4
        //    CrScPointsIn[3, 0] = CrScPointsIn[0, 0];         // y
        //    CrScPointsIn[3, 1] = CrScPointsIn[2, 1];         // z

        //    // Surface points

        //    // 1st radius - centre "1" (180-270 degrees)
        //    for (short i = 0; i < m_iNumOfArcPoints; i++)
        //    {
        //        CrScPointsIn[m_iNumOfAuxPoints + i, 0] = CrScPointsIn[0, 0] + Geom2D.GetPositionX_deg(m_fr_in, 180 + i * iRadiusAngle / m_iNumOfArcSegment);     // y
        //        CrScPointsIn[m_iNumOfAuxPoints + i, 1] = CrScPointsIn[0, 1] + Geom2D.GetPositionY_CW_deg(m_fr_in, 180 + i * iRadiusAngle / m_iNumOfArcSegment);  // z
        //    }

        //    // 2nd radius - centre "2" (270-360 degrees)
        //    for (short i = 0; i < m_iNumOfArcPoints; i++)
        //    {
        //        CrScPointsIn[m_iNumOfAuxPoints + m_iNumOfArcPoints + i, 0] = CrScPointsIn[1, 0] + Geom2D.GetPositionX_deg(m_fr_in, 270 + i * iRadiusAngle / m_iNumOfArcSegment);     // y
        //        CrScPointsIn[m_iNumOfAuxPoints + m_iNumOfArcPoints + i, 1] = CrScPointsIn[1, 1] + Geom2D.GetPositionY_CW_deg(m_fr_in, 270 + i * iRadiusAngle / m_iNumOfArcSegment);  // z
        //    }

        //    // 3rd radius - centre "3" (0-90 degrees)
        //    for (short i = 0; i < m_iNumOfArcPoints; i++)
        //    {
        //        CrScPointsIn[m_iNumOfAuxPoints + 2 * m_iNumOfArcPoints + i, 0] = CrScPointsIn[2, 0] + Geom2D.GetPositionX_deg(m_fr_in, 0 + i * iRadiusAngle / m_iNumOfArcSegment);     // y
        //        CrScPointsIn[m_iNumOfAuxPoints + 2 * m_iNumOfArcPoints + i, 1] = CrScPointsIn[2, 1] + Geom2D.GetPositionY_CW_deg(m_fr_in, 0 + i * iRadiusAngle / m_iNumOfArcSegment);  // z
        //    }

        //    // 4th radius - centre "4" (90-180 degrees)
        //    for (short i = 0; i < m_iNumOfArcPoints; i++)
        //    {
        //        CrScPointsIn[m_iNumOfAuxPoints + 3 * m_iNumOfArcPoints + i, 0] = CrScPointsIn[3, 0] + Geom2D.GetPositionX_deg(m_fr_in, 90 + i * iRadiusAngle / m_iNumOfArcSegment);     // y
        //        CrScPointsIn[m_iNumOfAuxPoints + 3 * m_iNumOfArcPoints + i, 1] = CrScPointsIn[3, 1] + Geom2D.GetPositionY_CW_deg(m_fr_in, 90 + i * iRadiusAngle / m_iNumOfArcSegment);  // z
        //    }

        //    for (int i = 0; i < CrScPointsOutArr.GetLength(0); i++)
        //    {
        //        CrScPointsOut.Add(new Point(CrScPointsOutArr[i, 0], CrScPointsOutArr[i, 1]));
        //    }
        //}

        ////----------------------------------------------------------------------------
        //void CalcCrSc_Coord_2()
        //{
        //    // Fill Point Array Data in LCS (Local Coordinate System of Cross-Section, horizontal y, vertical - z)
        //    float[,] CrScPointsOutArr = new float[ITotNoPoints, 2];
        //    //m_iNumOfAuxPoints = 4;

        //    // Number of segments should be even-numbered

        //    // Outside points
        //    // Auxialiary nodes per section

        //    // Point No. 1
        //    CrScPointsOutArr[0, 0] = (float)-b / 2f + m_fr_in + m_ft;    // y
        //    CrScPointsOutArr[0, 1] = (float)h / 2f - m_fr_in - m_ft;     // z

        //    // Point No. 2
        //    CrScPointsOutArr[1, 0] = -CrScPointsOutArr[0, 0];        // y
        //    CrScPointsOutArr[1, 1] = CrScPointsOutArr[0, 1];         // z

        //    // Point No. 3
        //    CrScPointsOutArr[2, 0] = -CrScPointsOutArr[0, 0];        // y
        //    CrScPointsOutArr[2, 1] = -CrScPointsOutArr[1, 1];        // z

        //    // Point No. 4
        //    CrScPointsOutArr[3, 0] = CrScPointsOutArr[0, 0];         // y
        //    CrScPointsOutArr[3, 1] = CrScPointsOutArr[2, 1];         // z

        //    // Surface points

        //    float fOutSegmLength = 2 * (m_ft + m_fr_in) / m_iNumOfArcSegment;

        //    // 1st radius - centre "1"
        //    for (short i = 0; i < m_iNumOfArcPoints; i++)
        //    {
        //        if (i <= m_iNumOfArcPoints / 2) // Vertical
        //        {
        //            CrScPointsOutArr[m_iNumOfAuxPoints + i, 0] = (float)-b / 2f;                                             // y
        //            CrScPointsOutArr[m_iNumOfAuxPoints + i, 1] = (float)h / 2f - (m_iNumOfArcSegment / 2 - i) * fOutSegmLength;  // z
        //        }
        //        else // Horizontal
        //        {
        //            CrScPointsOutArr[m_iNumOfAuxPoints + i, 0] = (float)-b / 2f + (i - m_iNumOfArcSegment / 2) * fOutSegmLength; // y      
        //            CrScPointsOutArr[m_iNumOfAuxPoints + i, 1] = (float)h / 2f;                                              // z                
        //        }
        //    }

        //    // 2nd radius - centre "2"
        //    for (short i = 0; i < m_iNumOfArcPoints; i++)
        //    {
        //        if (i <= m_iNumOfArcPoints / 2) // Horizontal
        //        {
        //            CrScPointsOutArr[m_iNumOfAuxPoints + m_iNumOfArcPoints + i, 0] = (float)b / 2f - (m_iNumOfArcSegment / 2 - i) * fOutSegmLength; // y          
        //            CrScPointsOutArr[m_iNumOfAuxPoints + m_iNumOfArcPoints + i, 1] = (float)h / 2f;                                             // z 
        //        }
        //        else // Vertical
        //        {
        //            CrScPointsOutArr[m_iNumOfAuxPoints + m_iNumOfArcPoints + i, 0] = (float)b / 2f;                                             // y
        //            CrScPointsOutArr[m_iNumOfAuxPoints + m_iNumOfArcPoints + i, 1] = (float)h / 2f - i / 2 * fOutSegmLength;                        // z
        //        }
        //    }

        //    // 3rd radius - centre "3"
        //    for (short i = 0; i < m_iNumOfArcPoints; i++)
        //    {
        //        if (i <= m_iNumOfArcPoints / 2) // Vertical
        //        {
        //            CrScPointsOutArr[m_iNumOfAuxPoints + 2 * m_iNumOfArcPoints + i, 0] = (float)b / 2f;                                               // y
        //            CrScPointsOutArr[m_iNumOfAuxPoints + 2 * m_iNumOfArcPoints + i, 1] = (float)-h / 2f + (m_iNumOfArcSegment / 2 - i) * fOutSegmLength;  // z
        //        }
        //        else // Horizontal
        //        {
        //            CrScPointsOutArr[m_iNumOfAuxPoints + 2 * m_iNumOfArcPoints + i, 0] = (float)b / 2f - i / 2 * fOutSegmLength;                         // y
        //            CrScPointsOutArr[m_iNumOfAuxPoints + 2 * m_iNumOfArcPoints + i, 1] = (float)-h / 2f;                                              // z                
        //        }
        //    }

        //    // 4th radius - centre "4"
        //    for (short i = 0; i < m_iNumOfArcPoints; i++)
        //    {
        //        if (i <= m_iNumOfArcPoints / 2) // Horizontal
        //        {
        //            CrScPointsOutArr[m_iNumOfAuxPoints + 3 * m_iNumOfArcPoints + i, 0] = (float)-b / 2f + (m_iNumOfArcSegment / 2 - i) * fOutSegmLength; // y                                    
        //            CrScPointsOutArr[m_iNumOfAuxPoints + 3 * m_iNumOfArcPoints + i, 1] = (float)-h / 2f;                                             // z 
        //        }
        //        else // Vertical
        //        {
        //            CrScPointsOutArr[m_iNumOfAuxPoints + 3 * m_iNumOfArcPoints + i, 0] = (float)-b / 2f;                                             // y
        //            CrScPointsOutArr[m_iNumOfAuxPoints + 3 * m_iNumOfArcPoints + i, 1] = (float)-h / 2f + i / 2 * fOutSegmLength;                       // z
        //        }
        //    }


        //    // Inside points
        //    // Auxialiary nodes per section

        //    // Point No. 1
        //    CrScPointsIn[0, 0] = (float)-b / 2f + m_fr_in + m_ft;    // y
        //    CrScPointsIn[0, 1] = (float)h / 2f - m_fr_in - m_ft;     // z

        //    // Point No. 2
        //    CrScPointsIn[1, 0] = -CrScPointsIn[0, 0];        // y
        //    CrScPointsIn[1, 1] = CrScPointsIn[0, 1];         // z

        //    // Point No. 3
        //    CrScPointsIn[2, 0] = -CrScPointsIn[0, 0];        // y
        //    CrScPointsIn[2, 1] = -CrScPointsIn[1, 1];        // z

        //    // Point No. 4
        //    CrScPointsIn[3, 0] = CrScPointsIn[0, 0];         // y
        //    CrScPointsIn[3, 1] = CrScPointsIn[2, 1];         // z

        //    // Surface points

        //    int iRadiusAngle = 90; // Radius Angle

        //    // 1st radius - centre "1" (180-270 degrees)
        //    for (short i = 0; i < m_iNumOfArcPoints; i++)
        //    {
        //        CrScPointsIn[m_iNumOfAuxPoints + i, 0] = CrScPointsIn[0, 0] + Geom2D.GetPositionX_deg(m_fr_in, 180 + i * iRadiusAngle / m_iNumOfArcSegment);     // y
        //        CrScPointsIn[m_iNumOfAuxPoints + i, 1] = CrScPointsIn[0, 1] + Geom2D.GetPositionY_CW_deg(m_fr_in, 180 + i * iRadiusAngle / m_iNumOfArcSegment);  // z
        //    }

        //    // 2nd radius - centre "2" (270-360 degrees)
        //    for (short i = 0; i < m_iNumOfArcPoints; i++)
        //    {
        //        CrScPointsIn[m_iNumOfAuxPoints + m_iNumOfArcPoints + i, 0] = CrScPointsIn[1, 0] + Geom2D.GetPositionX_deg(m_fr_in, 270 + i * iRadiusAngle / m_iNumOfArcSegment);     // y
        //        CrScPointsIn[m_iNumOfAuxPoints + m_iNumOfArcPoints + i, 1] = CrScPointsIn[1, 1] + Geom2D.GetPositionY_CW_deg(m_fr_in, 270 + i * iRadiusAngle / m_iNumOfArcSegment);  // z
        //    }

        //    // 3rd radius - centre "3" (0-90 degrees)
        //    for (short i = 0; i < m_iNumOfArcPoints; i++)
        //    {
        //        CrScPointsIn[m_iNumOfAuxPoints + 2 * m_iNumOfArcPoints + i, 0] = CrScPointsIn[2, 0] + Geom2D.GetPositionX_deg(m_fr_in, 0 + i * iRadiusAngle / m_iNumOfArcSegment);     // y
        //        CrScPointsIn[m_iNumOfAuxPoints + 2 * m_iNumOfArcPoints + i, 1] = CrScPointsIn[2, 1] + Geom2D.GetPositionY_CW_deg(m_fr_in, 0 + i * iRadiusAngle / m_iNumOfArcSegment);  // z
        //    }

        //    // 4th radius - centre "4" (90-180 degrees)
        //    for (short i = 0; i < m_iNumOfArcPoints; i++)
        //    {
        //        CrScPointsIn[m_iNumOfAuxPoints + 3 * m_iNumOfArcPoints + i, 0] = CrScPointsIn[3, 0] + Geom2D.GetPositionX_deg(m_fr_in, 90 + i * iRadiusAngle / m_iNumOfArcSegment);     // y
        //        CrScPointsIn[m_iNumOfAuxPoints + 3 * m_iNumOfArcPoints + i, 1] = CrScPointsIn[3, 1] + Geom2D.GetPositionY_CW_deg(m_fr_in, 90 + i * iRadiusAngle / m_iNumOfArcSegment);  // z
        //    }

        //    for (int i = 0; i < CrScPointsOutArr.GetLength(0); i++)
        //    {
        //        CrScPointsOut.Add(new Point(CrScPointsOutArr[i, 0], CrScPointsOutArr[i, 1]));
        //    }
        //}

        ////----------------------------------------------------------------------------
        //void CalcCrSc_Coord_3()
        //{
        //    // Fill Point Array Data in LCS (Local Coordinate System of Cross-Section, horizontal y, vertical - z)
        //    float[,] CrScPointsOutArr = new float[ITotNoPoints, 2];
        //    //m_iNumOfAuxPoints = 4;

        //    // Points
        //    // Auxialiary nodes for out side points and internal points

        //    // Point No. 1
        //    CrScPointsOutArr[0, 0] = (float)-b / 2f + m_fr_out;    // y
        //    CrScPointsOutArr[0, 1] = (float)h / 2f - m_fr_out;     // z

        //    // Point No. 2
        //    CrScPointsOutArr[1, 0] = -CrScPointsOutArr[0, 0];        // y
        //    CrScPointsOutArr[1, 1] = CrScPointsOutArr[0, 1];         // z

        //    // Point No. 3
        //    CrScPointsOutArr[2, 0] = -CrScPointsOutArr[0, 0];        // y
        //    CrScPointsOutArr[2, 1] = -CrScPointsOutArr[1, 1];        // z

        //    // Point No. 4
        //    CrScPointsOutArr[3, 0] = CrScPointsOutArr[0, 0];         // y
        //    CrScPointsOutArr[3, 1] = CrScPointsOutArr[2, 1];         // z

        //    // Surface points

        //    int iRadiusAngle = 90; // Radius Angle

        //    // 1st radius - centre "1" (180-270 degrees)
        //    for (short i = 0; i < m_iNumOfArcPoints; i++)
        //    {
        //        CrScPointsOutArr[m_iNumOfAuxPoints + i, 0] = CrScPointsOutArr[0, 0] + Geom2D.GetPositionX_deg(m_fr_out, 180 + i * iRadiusAngle / m_iNumOfArcSegment);     // y
        //        CrScPointsOutArr[m_iNumOfAuxPoints + i, 1] = CrScPointsOutArr[0, 1] + Geom2D.GetPositionY_CW_deg(m_fr_out, 180 + i * iRadiusAngle / m_iNumOfArcSegment);  // z
        //    }

        //    // 2nd radius - centre "2" (270-360 degrees)
        //    for (short i = 0; i < m_iNumOfArcPoints; i++)
        //    {
        //        CrScPointsOutArr[m_iNumOfAuxPoints + m_iNumOfArcPoints + i, 0] = CrScPointsOutArr[1, 0] + Geom2D.GetPositionX_deg(m_fr_out, 270 + i * iRadiusAngle / m_iNumOfArcSegment);     // y
        //        CrScPointsOutArr[m_iNumOfAuxPoints + m_iNumOfArcPoints + i, 1] = CrScPointsOutArr[1, 1] + Geom2D.GetPositionY_CW_deg(m_fr_out, 270 + i * iRadiusAngle / m_iNumOfArcSegment); // z
        //    }

        //    // 3rd radius - centre "3" (0-90 degrees)
        //    for (short i = 0; i < m_iNumOfArcPoints; i++)
        //    {
        //        CrScPointsOutArr[m_iNumOfAuxPoints + 2 * m_iNumOfArcPoints + i, 0] = CrScPointsOutArr[2, 0] + Geom2D.GetPositionX_deg(m_fr_out, 0 + i * iRadiusAngle / m_iNumOfArcSegment);     // y
        //        CrScPointsOutArr[m_iNumOfAuxPoints + 2 * m_iNumOfArcPoints + i, 1] = CrScPointsOutArr[2, 1] + Geom2D.GetPositionY_CW_deg(m_fr_out, 0 + i * iRadiusAngle / m_iNumOfArcSegment); // z
        //    }

        //    // 4th radius - centre "4" (90-180 degrees)
        //    for (short i = 0; i < m_iNumOfArcPoints; i++)
        //    {
        //        CrScPointsOutArr[m_iNumOfAuxPoints + 3 * m_iNumOfArcPoints + i, 0] = CrScPointsOutArr[3, 0] + Geom2D.GetPositionX_deg(m_fr_out, 90 + i * iRadiusAngle / m_iNumOfArcSegment);     // y
        //        CrScPointsOutArr[m_iNumOfAuxPoints + 3 * m_iNumOfArcPoints + i, 1] = CrScPointsOutArr[3, 1] + Geom2D.GetPositionY_CW_deg(m_fr_out, 90 + i * iRadiusAngle / m_iNumOfArcSegment); // z
        //    }

        //    for (int i = 0; i < CrScPointsOutArr.GetLength(0); i++)
        //    {
        //        CrScPointsOut.Add(new Point(CrScPointsOutArr[i, 0], CrScPointsOutArr[i, 1]));
        //    }
        //}

        //----------------------------------------------------------------------------
        void CalcCrSc_Coord_4()
        {


        }

		protected override void loadCrScIndices()
		{
            int secNum = 4 * (m_iNumOfArcSegment + 1); // Number of points to draw in one section inside or outside surface

                if (SShape == 0 || SShape == 1 || SShape == 2)
                {
                    CCrSc_0_26 o26 = new CCrSc_0_26();
                    o26.loadCrScIndices_26_28(secNum,m_iNumOfAuxPoints);
                    TriangleIndices = o26.TriangleIndices;
                    //load_0_26_28_TriangelIndices(iNumberAux, secNum);
                }
                else if (SShape == 3)
                {
                    // const int secNum = iNumberAux + iRadiusPoints * 4;  // Number of points in section (2D)
                    int iRadiusPoints = m_iNumOfArcSegment + 1;

                    TriangleIndices = new Int32Collection(12 * 6 + m_iNumOfArcSegment * 2 * 8 + (4 * iRadiusPoints) * 6);

                    // Front Side / Forehead
                    // Points order 1,2,3,4

                    AddRectangleIndices_CW_1234(TriangleIndices, 0, m_iNumOfAuxPoints + m_iNumOfArcSegment, m_iNumOfAuxPoints + 1 + m_iNumOfArcSegment, 1);
                    AddRectangleIndices_CW_1234(TriangleIndices, 1, m_iNumOfAuxPoints + 2 * m_iNumOfArcSegment + 1, m_iNumOfAuxPoints + 2 * m_iNumOfArcSegment + 2, 2);
                    AddRectangleIndices_CW_1234(TriangleIndices, 3, 2, m_iNumOfAuxPoints + 3 * m_iNumOfArcSegment + 2, m_iNumOfAuxPoints + 3 * m_iNumOfArcSegment + 3);
                    AddRectangleIndices_CW_1234(TriangleIndices, m_iNumOfAuxPoints, 0, 3, m_iNumOfAuxPoints + 4 * m_iNumOfArcSegment + 3);

                    // Arc sectors
                    // 1st SolidCircleSector
                    AddSolidCircleSectorIndices(0, m_iNumOfAuxPoints, m_iNumOfArcSegment, TriangleIndices, false);
                    // 2nd SolidCircleSector
                    AddSolidCircleSectorIndices(1, m_iNumOfAuxPoints + iRadiusPoints, m_iNumOfArcSegment, TriangleIndices, false);
                    // 3rd SolidCircleSector
                    AddSolidCircleSectorIndices(2, m_iNumOfAuxPoints + 2 * iRadiusPoints, m_iNumOfArcSegment, TriangleIndices, false);
                    // 4th SolidCircleSector
                    AddSolidCircleSectorIndices(3, m_iNumOfAuxPoints + 3 * iRadiusPoints, m_iNumOfArcSegment, TriangleIndices, false);

                    // Back Side
                    // Points order 1,4,3,2

                    int iPointNumbersOffset = m_iNumOfAuxPoints + 4 * iRadiusPoints; // Number of nodes per section - Nodes offset

                    AddRectangleIndices_CW_1234(TriangleIndices, iPointNumbersOffset + 0, iPointNumbersOffset + 1, iPointNumbersOffset + m_iNumOfAuxPoints + 1 + m_iNumOfArcSegment, iPointNumbersOffset + m_iNumOfAuxPoints + m_iNumOfArcSegment);
                    AddRectangleIndices_CW_1234(TriangleIndices, iPointNumbersOffset + 1, iPointNumbersOffset + 2, iPointNumbersOffset + m_iNumOfAuxPoints + 2 * m_iNumOfArcSegment + 2, iPointNumbersOffset + m_iNumOfAuxPoints + 2 * m_iNumOfArcSegment + 1);
                    AddRectangleIndices_CW_1234(TriangleIndices, iPointNumbersOffset + 3, iPointNumbersOffset + m_iNumOfAuxPoints + 3 * m_iNumOfArcSegment + 3, iPointNumbersOffset + m_iNumOfAuxPoints + 3 * m_iNumOfArcSegment + 2, iPointNumbersOffset + 2);
                    AddRectangleIndices_CW_1234(TriangleIndices, iPointNumbersOffset + m_iNumOfAuxPoints, iPointNumbersOffset + m_iNumOfAuxPoints + 4 * m_iNumOfArcSegment + 3, iPointNumbersOffset + 3, iPointNumbersOffset + 0);

                    // Arc sectors
                    // 1st SolidCircleSector
                    AddSolidCircleSectorIndices(iPointNumbersOffset + 0, iPointNumbersOffset + m_iNumOfAuxPoints, m_iNumOfArcSegment, TriangleIndices, true);
                    // 2nd SolidCircleSector
                    AddSolidCircleSectorIndices(iPointNumbersOffset + 1, iPointNumbersOffset + m_iNumOfAuxPoints + iRadiusPoints, m_iNumOfArcSegment, TriangleIndices, true);
                    // 3rd SolidCircleSector
                    AddSolidCircleSectorIndices(iPointNumbersOffset + 2, iPointNumbersOffset + m_iNumOfAuxPoints + 2 * iRadiusPoints, m_iNumOfArcSegment, TriangleIndices, true);
                    // 4th SolidCircleSector
                    AddSolidCircleSectorIndices(iPointNumbersOffset + 3, iPointNumbersOffset + m_iNumOfAuxPoints + 3 * iRadiusPoints, m_iNumOfArcSegment, TriangleIndices, true);

                    // Shell - outside
                    DrawCaraLaterals(m_iNumOfAuxPoints, 4 * iRadiusPoints, TriangleIndices);
                    // Shell - inside
                    AddRectangleIndices_CW_1234(TriangleIndices, 0, iPointNumbersOffset + 0, iPointNumbersOffset + 3, 3);
                    AddRectangleIndices_CW_1234(TriangleIndices, 1, iPointNumbersOffset + 1, iPointNumbersOffset + 0, 0);
                    AddRectangleIndices_CW_1234(TriangleIndices, 2, iPointNumbersOffset + 2, iPointNumbersOffset + 1, 1);
                    AddRectangleIndices_CW_1234(TriangleIndices, 3, iPointNumbersOffset + 3, iPointNumbersOffset + 2, 2);
                }
                else if (SShape == 4)
                {

                }
                else
                {
                    CCrSc_0_25 o25 = new CCrSc_0_25();
                    o25.loadCrScIndices_25();
                    TriangleIndices = o25.TriangleIndices;
                    //load_0_25_TriangelIndices();
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
