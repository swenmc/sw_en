using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using MATH;
using BaseClasses;

namespace CRSC
{
    public class CCrSc_2_00_AAC_Roof_Panel : CCrSc_2_00
    {
        // Solid AAC roof panel - grooved

        bool bIndicesCW = true; // Clockwise or counter-clockwise system

        public CCrSc_2_00_AAC_Roof_Panel()
        {
        }
        public CCrSc_2_00_AAC_Roof_Panel(float fh, float fb)
        {
            IsShapeSolid = true;

            //ITotNoPoints = 12;
            ITotNoPoints = 12;
            INoAuxPoints = 0;
            h = fh;
            b = fb;

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

            // Outside Points Coordinates

            float xc = 0.03f; // 30 mm ???

            float ya = 0.3f * (float)h;
            float yb = 0.4f * (float)h;

            CrScPointsOut[0, 0] = 0.0f;
            CrScPointsOut[0, 1] = (float)h;
            CrScPointsOut[1, 0] = (float)b;
            CrScPointsOut[1, 1] = (float)h;
            CrScPointsOut[2, 0] = (float)b;
            CrScPointsOut[2, 1] = (float)h - ya;
            CrScPointsOut[3, 0] = (float)b + xc;
            CrScPointsOut[3, 1] = (float)h - yb;
            CrScPointsOut[4, 0] = (float)b + xc;
            CrScPointsOut[4, 1] = yb;
            CrScPointsOut[5, 0] = (float)b;
            CrScPointsOut[5, 1] = ya;
            CrScPointsOut[6, 0] = (float)b;
            CrScPointsOut[6, 1] = 0;
            CrScPointsOut[7, 0] = 0;
            CrScPointsOut[7, 1] = 0;
            CrScPointsOut[8, 0] = 0;
            CrScPointsOut[8, 1] = ya;
            CrScPointsOut[9, 0] = xc;
            CrScPointsOut[9, 1] = yb;
            CrScPointsOut[10, 0] = xc;
            CrScPointsOut[10, 1] = (float)h - yb;
            CrScPointsOut[11, 0] = 0;
            CrScPointsOut[11, 1] = (float)h - ya;
        }

        protected override void loadCrScIndicesFrontSide()
        {
            TriangleIndicesFrontSide = new Int32Collection(30);

            if (bIndicesCW)
            {
                AddTriangleIndices_CW_123(TriangleIndicesFrontSide, 0, 1, 2);
                AddTriangleIndices_CW_123(TriangleIndicesFrontSide, 0, 2, 11);
                AddTriangleIndices_CW_123(TriangleIndicesFrontSide, 11, 3, 10);
                AddTriangleIndices_CW_123(TriangleIndicesFrontSide, 11, 2, 3);
                AddTriangleIndices_CW_123(TriangleIndicesFrontSide, 10, 3, 4);
                AddTriangleIndices_CW_123(TriangleIndicesFrontSide, 10, 4, 9);
                AddTriangleIndices_CW_123(TriangleIndicesFrontSide, 9, 4, 5);
                AddTriangleIndices_CW_123(TriangleIndicesFrontSide, 9, 5, 8);
                AddTriangleIndices_CW_123(TriangleIndicesFrontSide, 8, 5, 6);
                AddTriangleIndices_CW_123(TriangleIndicesFrontSide, 8, 6, 7);

            }
            else
            {
                AddTriangleIndices_CCW_123(TriangleIndicesFrontSide, 0, 1, 2);
                AddTriangleIndices_CCW_123(TriangleIndicesFrontSide, 0, 2, 11);
                AddTriangleIndices_CCW_123(TriangleIndicesFrontSide, 11, 3, 10);
                AddTriangleIndices_CCW_123(TriangleIndicesFrontSide, 11, 2, 3);
                AddTriangleIndices_CCW_123(TriangleIndicesFrontSide, 10, 3, 4);
                AddTriangleIndices_CCW_123(TriangleIndicesFrontSide, 10, 4, 9);
                AddTriangleIndices_CCW_123(TriangleIndicesFrontSide, 9, 4, 5);
                AddTriangleIndices_CCW_123(TriangleIndicesFrontSide, 9, 5, 8);
                AddTriangleIndices_CCW_123(TriangleIndicesFrontSide, 8, 5, 6);
                AddTriangleIndices_CCW_123(TriangleIndicesFrontSide, 8, 6, 7);
            }

        }

        protected override void loadCrScIndicesShell()
        {
            TriangleIndicesShell = new Int32Collection((ITotNoPoints - 1) * 6);

            // Shell Surface OutSide
            for (int i = 0; i < ITotNoPoints - 1; i++)
            {
                if (i < ITotNoPoints - 2)
                {
                    if (bIndicesCW)
                        AddRectangleIndices_CW_1234(TriangleIndicesShell, i, ITotNoPoints + i, ITotNoPoints + i + 1, i + 1);
                    else
                        AddRectangleIndices_CCW_1234(TriangleIndicesShell, i, ITotNoPoints + i, ITotNoPoints + i + 1, i + 1);
                }
                else
                {
                    if (bIndicesCW)
                        AddRectangleIndices_CW_1234(TriangleIndicesShell, i, ITotNoPoints + i, ITotNoPoints, 0); // Last Element
                    else
                        AddRectangleIndices_CCW_1234(TriangleIndicesShell, i, ITotNoPoints + i, ITotNoPoints, 0); // Last Element
                }
            }
        }

        protected override void loadCrScIndicesBackSide()
        {
            TriangleIndicesBackSide = new Int32Collection(30);

            if (bIndicesCW)
            {
                AddTriangleIndices_CCW_123(TriangleIndicesBackSide, ITotNoPoints, ITotNoPoints + 1, ITotNoPoints + 2);
                AddTriangleIndices_CCW_123(TriangleIndicesBackSide, ITotNoPoints, ITotNoPoints + 2, ITotNoPoints + 11);
                AddTriangleIndices_CCW_123(TriangleIndicesBackSide, ITotNoPoints + 11, ITotNoPoints + 3, ITotNoPoints + 10);
                AddTriangleIndices_CCW_123(TriangleIndicesBackSide, ITotNoPoints + 11, ITotNoPoints + 2, ITotNoPoints + 3);
                AddTriangleIndices_CCW_123(TriangleIndicesBackSide, ITotNoPoints + 10, ITotNoPoints + 3, ITotNoPoints + 4);
                AddTriangleIndices_CCW_123(TriangleIndicesBackSide, ITotNoPoints + 10, ITotNoPoints + 4, ITotNoPoints + 9);
                AddTriangleIndices_CCW_123(TriangleIndicesBackSide, ITotNoPoints + 9, ITotNoPoints + 4, ITotNoPoints + 5);
                AddTriangleIndices_CCW_123(TriangleIndicesBackSide, ITotNoPoints + 9, ITotNoPoints + 5, ITotNoPoints + 8);
                AddTriangleIndices_CCW_123(TriangleIndicesBackSide, ITotNoPoints + 8, ITotNoPoints + 5, ITotNoPoints + 6);
                AddTriangleIndices_CCW_123(TriangleIndicesBackSide, ITotNoPoints + 8, ITotNoPoints + 6, ITotNoPoints + 7);
            }
            else
            {
                AddTriangleIndices_CW_123(TriangleIndicesBackSide, ITotNoPoints, ITotNoPoints + 1, ITotNoPoints + 2);
                AddTriangleIndices_CW_123(TriangleIndicesBackSide, ITotNoPoints, ITotNoPoints + 2, ITotNoPoints + 11);
                AddTriangleIndices_CW_123(TriangleIndicesBackSide, ITotNoPoints + 11, ITotNoPoints + 3, ITotNoPoints + 10);
                AddTriangleIndices_CW_123(TriangleIndicesBackSide, ITotNoPoints + 11, ITotNoPoints + 2, ITotNoPoints + 3);
                AddTriangleIndices_CW_123(TriangleIndicesBackSide, ITotNoPoints + 10, ITotNoPoints + 3, ITotNoPoints + 4);
                AddTriangleIndices_CW_123(TriangleIndicesBackSide, ITotNoPoints + 10, ITotNoPoints + 4, ITotNoPoints + 9);
                AddTriangleIndices_CW_123(TriangleIndicesBackSide, ITotNoPoints + 9, ITotNoPoints + 4, ITotNoPoints + 5);
                AddTriangleIndices_CW_123(TriangleIndicesBackSide, ITotNoPoints + 9, ITotNoPoints + 5, ITotNoPoints + 8);
                AddTriangleIndices_CW_123(TriangleIndicesBackSide, ITotNoPoints + 8, ITotNoPoints + 5, ITotNoPoints + 6);
                AddTriangleIndices_CW_123(TriangleIndicesBackSide, ITotNoPoints + 8, ITotNoPoints + 6, ITotNoPoints + 7);
            }
        }
    }
}
