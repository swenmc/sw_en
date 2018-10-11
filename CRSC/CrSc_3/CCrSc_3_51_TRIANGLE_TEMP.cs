using MATH;
using System;
using System.Windows.Media;
using System.Collections.Generic;
using System.Windows;

namespace CRSC
{
    public class CCrSc_3_51_TRIANGLE_TEMP : CSO
    {
        // Thin-walled triangle

        private float m_ft; 

        public float Ft
        {
            get { return m_ft; }
            set { m_ft = value; }
        }

        bool bIsDefinedCCW = false; // Pomocny bool

        public CCrSc_3_51_TRIANGLE_TEMP(float fh = 0.866025f * 0.5f, float fb = 0.5f, float ft = 0.002f)
        {
            CSColor = Colors.DarkGreen;

            //ITotNoPoints = 3;
            IsShapeSolid = false;
            INoPointsIn = INoPointsOut = 3; // vykreslujeme ako n-uholnik, pocet bodov n
            ITotNoPoints = INoPointsIn + INoPointsOut;

            h = fh;
            b = fb;
            m_ft = ft;

            // Create Array - allocate memory
            CrScPointsOut = new List<Point>(INoPointsOut);
            CrScPointsIn = new List<Point>(INoPointsIn);

            // Fill Array Data
            CalcCrSc_Coord();

            // Fill list of indices for drawing of surface - triangles edges

            // Particular indices - distinguished colors of member surfaces
            loadCrScIndicesFrontSide();
            loadCrScIndicesShell();
            loadCrScIndicesBackSide();

            // Complex indices - one color of member
            loadCrScIndices();

            // Wireframe Indices
            loadCrScWireFrameIndicesFrontSide();
            loadCrScWireFrameIndicesBackSide();
            loadCrScWireFrameIndicesLaterals();

            // Complex indices
            loadCrScWireFrameIndices();
        }

        public CCrSc_3_51_TRIANGLE_TEMP(float fh, float fb, float ft, Color color_temp)
        {
            CSColor = color_temp;

            //ITotNoPoints = 3;
            IsShapeSolid = false;
            INoPointsIn = INoPointsOut = 3; // vykreslujeme ako n-uholnik, pocet bodov n
            ITotNoPoints = INoPointsIn + INoPointsOut;

            h = fh;
            b = fb;
            m_ft = ft;

            // Create Array - allocate memory
            CrScPointsOut = new List<Point>(INoPointsOut);
            CrScPointsIn = new List<Point>(INoPointsIn);

            // Fill Array Data
            CalcCrSc_Coord();

            // Fill list of indices for drawing of surface - triangles edges

            // Particular indices - distinguished colors of member surfaces
            loadCrScIndicesFrontSide();
            loadCrScIndicesShell();
            loadCrScIndicesBackSide();

            // Complex indices - one color of member
            loadCrScIndices();

            // Wireframe Indices
            loadCrScWireFrameIndicesFrontSide();
            loadCrScWireFrameIndicesBackSide();
            loadCrScWireFrameIndicesLaterals();

            // Complex indices
            loadCrScWireFrameIndices();
        }

        public void CalcCrSc_Coord()
        {
            // Fill Point Array Data in LCS (Local Coordinate System of Cross-Section, horizontal y, vertical - z)

            if (bIsDefinedCCW)
            {
                // Defined Counter-clockwise

                // Point No. 1
                CrScPointsOut.Add(new Point(0, h * (2.0 / 3.0)));
                // Point No. 2
                CrScPointsOut.Add(new Point(-b / 2.0, -h * (1.0 / 3.0)));
                // Point No. 3
                CrScPointsOut.Add(new Point(-CrScPointsOut[1].X, CrScPointsOut[1].Y));

                float fAlphaDeg = 30f;

                // Internal

                // Point No. 1
                CrScPointsIn.Add(new Point(CrScPointsOut[0].X, CrScPointsOut[0].Y - m_ft / Math.Sin(fAlphaDeg * MathF.fPI / 180f)));
                // Point No. 2
                CrScPointsIn.Add(new Point(CrScPointsOut[1].X + m_ft / Math.Tan(fAlphaDeg * MathF.fPI / 180f), CrScPointsOut[1].Y + m_ft));
                // Point No. 3
                CrScPointsIn.Add(new Point(-CrScPointsIn[1].X, CrScPointsIn[1].Y));
            }
            else
            {
                // Point No. 1
                CrScPointsOut.Add(new Point(0, h * (2.0 / 3.0)));
                // Point No. 2
                CrScPointsOut.Add(new Point(b / 2.0, -h * (1.0 / 3.0)));
                // Point No. 3
                CrScPointsOut.Add(new Point(-CrScPointsOut[1].X, CrScPointsOut[1].Y));

                float fAlphaDeg = 30f;

                // Internal

                // Point No. 1
                CrScPointsIn.Add(new Point(CrScPointsOut[0].X, CrScPointsOut[0].Y - m_ft / Math.Sin(fAlphaDeg * MathF.fPI / 180f)));
                // Point No. 2
                CrScPointsIn.Add(new Point(CrScPointsOut[1].X - m_ft / Math.Tan(fAlphaDeg * MathF.fPI / 180f), CrScPointsOut[1].Y + m_ft));
                // Point No. 3
                CrScPointsIn.Add(new Point(-CrScPointsIn[1].X, CrScPointsIn[1].Y));
            }
        }

        protected override void loadCrScIndicesFrontSide()
        {
            TriangleIndicesFrontSide = new Int32Collection(3 * 6);

            if (bIsDefinedCCW)
            {
                AddRectangleIndices_CW_1234(TriangleIndicesFrontSide, 0, 1, 4, 3);
                AddRectangleIndices_CW_1234(TriangleIndicesFrontSide, 1, 2, 5, 4);
                AddRectangleIndices_CW_1234(TriangleIndicesFrontSide, 2, 0, 3, 5);
            }
            else
            {
                AddRectangleIndices_CCW_1234(TriangleIndicesFrontSide, 0, 1, 4, 3);
                AddRectangleIndices_CCW_1234(TriangleIndicesFrontSide, 1, 2, 5, 4);
                AddRectangleIndices_CCW_1234(TriangleIndicesFrontSide, 2, 0, 3, 5);
            }
        }

        protected override void loadCrScIndicesBackSide()
        {
            TriangleIndicesBackSide = new Int32Collection(3 * 6);

            if (bIsDefinedCCW)
            {
                AddRectangleIndices_CCW_1234(TriangleIndicesBackSide, ITotNoPoints + 0, ITotNoPoints + 1, ITotNoPoints + 4, ITotNoPoints + 3);
                AddRectangleIndices_CCW_1234(TriangleIndicesBackSide, ITotNoPoints + 1, ITotNoPoints + 2, ITotNoPoints + 5, ITotNoPoints + 4);
                AddRectangleIndices_CCW_1234(TriangleIndicesBackSide, ITotNoPoints + 2, ITotNoPoints + 0, ITotNoPoints + 3, ITotNoPoints + 5);
            }
            else
            {
                AddRectangleIndices_CW_1234(TriangleIndicesBackSide, ITotNoPoints + 0, ITotNoPoints + 1, ITotNoPoints + 4, ITotNoPoints + 3);
                AddRectangleIndices_CW_1234(TriangleIndicesBackSide, ITotNoPoints + 1, ITotNoPoints + 2, ITotNoPoints + 5, ITotNoPoints + 4);
                AddRectangleIndices_CW_1234(TriangleIndicesBackSide, ITotNoPoints + 2, ITotNoPoints + 0, ITotNoPoints + 3, ITotNoPoints + 5);
            }
        }
    }
}
