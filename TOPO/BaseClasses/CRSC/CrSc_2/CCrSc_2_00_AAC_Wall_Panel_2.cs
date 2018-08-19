using MATH;
using System.Windows.Media;

namespace BaseClasses.CRSC
{
    public class CCrSc_2_00_AAC_Wall_Panel_2 : CCrSc_2_00
    {
        // Solid AAC vertical wall panel - grooved

        //bool bIndicesCW = true; // Clockwise or counter-clockwise system

        public int m_iNumOfArcSegment;
        public int m_iNumOfArcPoints;

        public float m_fr_1;

        public CCrSc_2_00_AAC_Wall_Panel_2()
        {
        }
        public CCrSc_2_00_AAC_Wall_Panel_2(float fh, float fb)
        {
            IsShapeSolid = true;

            //m_iNumOfArcSegment = iNumOfArcSegment; // 8;
            m_iNumOfArcSegment = 18; // !!! between 0 and 180 degrees, have to be an even number
            m_iNumOfArcPoints = (short)(m_iNumOfArcSegment + 1); // Each arc is defined by number of segments + 1 point points // have to be an odd number

            INoAuxPoints = 4;
            ITotNoPoints = (short)(2 * (short)m_iNumOfArcPoints + (INoAuxPoints + 4));

            h = fh;
            b = fb;
            m_fr_1 = 0.05f;

            // Create Array - allocate memory
            CrScPointsOut = new float[ITotNoPoints, 2];

            // Fill Array Data
            CalcCrSc_Coord();

            // Particular indices Rozpracovane pre vykreslovanie cela prutu inou farbou
            loadCrScIndicesFrontSide();
            loadCrScIndicesShell();
            loadCrScIndicesBackSide();

            // All indices together
            //loadCrScIndices();
        }

        public new void CalcCrSc_Coord()
        {
            // Fill Point Array Data in LCS (Local Coordinate System of Cross-Section, horizontal y, vertical - z)

            // Point No. 1
            CrScPointsOut[0, 0] = m_fr_1;                   // y
            CrScPointsOut[0, 1] = 0.5f * (float)h + m_fr_1;       // z

            // Point No. 2
            CrScPointsOut[1, 0] = (float)b - m_fr_1;              // y
            CrScPointsOut[1, 1] = CrScPointsOut[0, 1];      // z

            // Point No. 3
            CrScPointsOut[2, 0] = CrScPointsOut[1, 0];      // y
            CrScPointsOut[2, 1] = 0.5f * (float)h - m_fr_1;       // z

            // Point No. 4
            CrScPointsOut[3, 0] = CrScPointsOut[0, 0];      // y
            CrScPointsOut[3, 1] = CrScPointsOut[2, 1];      // z

            // Surface points

            // Point No. 5
            CrScPointsOut[4, 0] = 0;                        // y
            CrScPointsOut[4, 1] = (float)h;                       // z

            // Point No. 6
            CrScPointsOut[5, 0] = (float)b;                       // y
            CrScPointsOut[5, 1] = CrScPointsOut[4, 1];      // z

            int iRadiusAngle = 180; // Radius Angle

            // 1st radius - centre "1" (90-270 degrees)
            for (short i = 0; i < m_iNumOfArcPoints; i++)
            {
                CrScPointsOut[INoAuxPoints + i + 2, 0] = CrScPointsOut[1, 0] + m_fr_1 + Geom2D.GetPositionX(m_fr_1, 90 + i * iRadiusAngle / m_iNumOfArcSegment);     // y
                CrScPointsOut[INoAuxPoints + i + 2, 1] = CrScPointsOut[1, 1] - m_fr_1 + Geom2D.GetPositionY_CCW(m_fr_1, 90 + i * iRadiusAngle / m_iNumOfArcSegment); // z
            }

            // Point No. 7
            CrScPointsOut[INoAuxPoints + m_iNumOfArcPoints + 2, 0] = CrScPointsOut[5, 0];      // y
            CrScPointsOut[INoAuxPoints + m_iNumOfArcPoints + 2, 1] = 0;                        // z

            // Point No. 8
            CrScPointsOut[INoAuxPoints + m_iNumOfArcPoints + 3, 0] = 0;                        // y
            CrScPointsOut[INoAuxPoints + m_iNumOfArcPoints + 3, 1] = 0;                        // z

            // 2nd radius - centre "2" (180-270 degrees)
            for (short i = 0; i < m_iNumOfArcPoints; i++)
            {
                CrScPointsOut[INoAuxPoints + m_iNumOfArcPoints + i + 4, 0] = CrScPointsOut[0, 0] - m_fr_1 + Geom2D.GetPositionX(m_fr_1, 270 + i * iRadiusAngle / m_iNumOfArcSegment);     // y
                CrScPointsOut[INoAuxPoints + m_iNumOfArcPoints + i + 4, 1] = CrScPointsOut[0, 1] - m_fr_1 + Geom2D.GetPositionY_CCW(m_fr_1, 270 + i * iRadiusAngle / m_iNumOfArcSegment); // z
            }
        }

        protected override void loadCrScIndicesFrontSide()
        {
            int iRadiusPoints = m_iNumOfArcSegment + 1;
            int iRadiusPoints_14 = m_iNumOfArcSegment / 2 + 1;
            int iNumOfArcSegment_14 = m_iNumOfArcSegment / 2;

            TriangleIndicesFrontSide = new Int32Collection(iNumOfArcSegment_14 * 2 * 4 + 18);

            // Front Side / Forehead
            AddRectangleIndices_CW_1234(TriangleIndicesFrontSide, 4, 5, INoAuxPoints + 2, INoAuxPoints + 3 + 2 * iRadiusPoints);
            AddRectangleIndices_CW_1234(TriangleIndicesFrontSide, 0, 1, 2, 3);
            AddRectangleIndices_CW_1234(TriangleIndicesFrontSide, INoAuxPoints + 4 + iRadiusPoints, INoAuxPoints + 1 + iRadiusPoints, INoAuxPoints + 2 + iRadiusPoints, INoAuxPoints + 3 + iRadiusPoints);
            
            // Arc sectors
            // 1st SolidCircleSector
            AddSolidCircleSectorIndices(1, INoAuxPoints + 2, iNumOfArcSegment_14, TriangleIndicesFrontSide, false);
            // 2nd SolidCircleSector
            AddSolidCircleSectorIndices(2, INoAuxPoints + 2 + iNumOfArcSegment_14, iNumOfArcSegment_14, TriangleIndicesFrontSide, false);
            // 3rd SolidCircleSector
            AddSolidCircleSectorIndices(3, INoAuxPoints + 4 + 2 * iNumOfArcSegment_14 + 1, iNumOfArcSegment_14, TriangleIndicesFrontSide, false);
            // 4th SolidCircleSector
            AddSolidCircleSectorIndices(0, INoAuxPoints + 4 + 3 * iNumOfArcSegment_14 + 1, iNumOfArcSegment_14, TriangleIndicesFrontSide, false);
        }

        protected override void loadCrScIndicesShell()
        {
            // Shell
            int iRadiusPoints = m_iNumOfArcSegment + 1;
            TriangleIndicesShell = new Int32Collection((2 * iRadiusPoints + 4) * 6);

            DrawCaraLaterals(INoAuxPoints, 2 * iRadiusPoints + 4, TriangleIndicesShell);
        }

        protected override void loadCrScIndicesBackSide()
        {
            int iRadiusPoints = m_iNumOfArcSegment + 1;
            int iRadiusPoints_14 = m_iNumOfArcSegment / 2 + 1;
            int iNumOfArcSegment_14 = m_iNumOfArcSegment / 2;
            int iPointNumbersOffset = INoAuxPoints + 2 * iRadiusPoints + 4; // Number of nodes per section - Nodes offset (aux points + 2 *radius points + 4 * edge points)

            TriangleIndicesBackSide = new Int32Collection(iNumOfArcSegment_14 * 2 * 4 + 18);

            // Front Side / Forehead
            AddRectangleIndices_CCW_1234(TriangleIndicesBackSide, iPointNumbersOffset + 4, iPointNumbersOffset + 5, iPointNumbersOffset + INoAuxPoints + 2, iPointNumbersOffset + INoAuxPoints + 3 + 2 * iRadiusPoints);
            AddRectangleIndices_CCW_1234(TriangleIndicesBackSide, iPointNumbersOffset + 0, iPointNumbersOffset + 1, iPointNumbersOffset + 2, iPointNumbersOffset + 3);
            AddRectangleIndices_CCW_1234(TriangleIndicesBackSide, iPointNumbersOffset + INoAuxPoints + 4 + iRadiusPoints, iPointNumbersOffset + INoAuxPoints + 1 + iRadiusPoints, iPointNumbersOffset + INoAuxPoints + 2 + iRadiusPoints, iPointNumbersOffset + INoAuxPoints + 3 + iRadiusPoints);

            // Arc sectors
            // 1st SolidCircleSector
            AddSolidCircleSectorIndices(iPointNumbersOffset + 1, iPointNumbersOffset + INoAuxPoints + 2, iNumOfArcSegment_14, TriangleIndicesBackSide, true);
            // 2nd SolidCircleSector
            AddSolidCircleSectorIndices(iPointNumbersOffset + 2, iPointNumbersOffset + INoAuxPoints + 2 + iNumOfArcSegment_14, iNumOfArcSegment_14, TriangleIndicesBackSide, true);
            // 3rd SolidCircleSector
            AddSolidCircleSectorIndices(iPointNumbersOffset + 3, iPointNumbersOffset + INoAuxPoints + 4 + 2 * iNumOfArcSegment_14 + 1, iNumOfArcSegment_14, TriangleIndicesBackSide, true);
            // 4th SolidCircleSector
            AddSolidCircleSectorIndices(iPointNumbersOffset + 0, iPointNumbersOffset + INoAuxPoints + 4 + 3 * iNumOfArcSegment_14 + 1, iNumOfArcSegment_14, TriangleIndicesBackSide, true);
        }
    }
}
