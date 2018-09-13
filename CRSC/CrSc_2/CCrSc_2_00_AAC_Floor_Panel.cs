using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace CRSC
{
    public class CCrSc_2_00_AAC_Floor_Panel : CCrSc_2_00
    {
        //  Solid AAC floor panel - grooved

        bool bIndicesCW = true; // Clockwise or counter-clockwise system

        public CCrSc_2_00_AAC_Floor_Panel()
        {
        }
        public CCrSc_2_00_AAC_Floor_Panel(float fh, float fb)
        {
            IsShapeSolid = true;

            //ITotNoPoints = 10;
            ITotNoPoints = 10;
            INoAuxPoints = 0;
            h = fh;
            b = fb;

            // Create Array - allocate memory
            CrScPointsOut = new float[ITotNoPoints, 2];
            //CrScPointsOut = new List<Point>(ITotNoPoints);

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

            float xb = 0.18f * (float)b;
            float xa = 0.15f * (float)b;
            float xc = 0.03f; // 30 mm ???

            float ya = 0.5f * (float)h;
            float yb = 0.7f * (float)h;

            CrScPointsOut[0, 0] = 0.0f;
            CrScPointsOut[0, 1] = (float)h;
            CrScPointsOut[1, 0] = (float)b - xa;
            CrScPointsOut[1, 1] = (float)h;
            CrScPointsOut[2, 0] = (float)b - xb;
            CrScPointsOut[2, 1] = yb;
            CrScPointsOut[3, 0] = (float)b;
            CrScPointsOut[3, 1] = ya;
            CrScPointsOut[4, 0] = (float)b;
            CrScPointsOut[4, 1] = 0;
            CrScPointsOut[5, 0] = 0;
            CrScPointsOut[5, 1] = 0;
            CrScPointsOut[6, 0] = 0;
            CrScPointsOut[6, 1] = ya;
            CrScPointsOut[7, 0] = xc;
            CrScPointsOut[7, 1] = ya + 0.5f * xc;
            CrScPointsOut[8, 0] = xc;
            CrScPointsOut[8, 1] = yb;
            CrScPointsOut[9, 0] = 0;
            CrScPointsOut[9, 1] = yb + 0.5f * xc;
        }
        //public new void CalcCrSc_Coord()
        //{
        //    // Fill Point Array Data in LCS (Local Coordinate System of Cross-Section, horizontal y, vertical - z)

        //    // Outside Points Coordinates
        //    double xb = 0.18f * b;
        //    double xa = 0.15f * b;
        //    double xc = 0.03f; // 30 mm ???
        //    double ya = 0.5f * h;
        //    double yb = 0.7f * h;
            
        //    CrScPointsOut.Add(new Point(0, h));
        //    CrScPointsOut.Add(new Point(b - xa, h));
        //    CrScPointsOut.Add(new Point(b - xb, yb));            
        //    CrScPointsOut.Add(new Point(b, ya));            
        //    CrScPointsOut.Add(new Point(b, 0));            
        //    CrScPointsOut.Add(new Point(0, 0));
        //    CrScPointsOut.Add(new Point(0, ya));
        //    CrScPointsOut.Add(new Point(xc, ya + 0.5f * xc));
        //    CrScPointsOut.Add(new Point(xc, yb));
        //    CrScPointsOut.Add(new Point(0, yb + 0.5f * xc));
        //}

        protected override void loadCrScIndicesFrontSide()
        {
            TriangleIndicesFrontSide = new Int32Collection(24);

            if (bIndicesCW)
            {
                AddTriangleIndices_CW_123(TriangleIndicesFrontSide, 0, 2, 9);
                AddTriangleIndices_CW_123(TriangleIndicesFrontSide, 0, 1, 2);
                AddTriangleIndices_CW_123(TriangleIndicesFrontSide, 9, 2, 8);
                AddTriangleIndices_CW_123(TriangleIndicesFrontSide, 8, 2, 7);
                AddTriangleIndices_CW_123(TriangleIndicesFrontSide, 7, 2, 3);
                AddTriangleIndices_CW_123(TriangleIndicesFrontSide, 7, 3, 6);
                AddTriangleIndices_CW_123(TriangleIndicesFrontSide, 6, 4, 5);
                AddTriangleIndices_CW_123(TriangleIndicesFrontSide, 6, 3, 4);

            }
            else
            {
                AddTriangleIndices_CCW_123(TriangleIndicesFrontSide, 0, 2, 9);
                AddTriangleIndices_CCW_123(TriangleIndicesFrontSide, 0, 1, 2);
                AddTriangleIndices_CCW_123(TriangleIndicesFrontSide, 9, 2, 8);
                AddTriangleIndices_CCW_123(TriangleIndicesFrontSide, 8, 2, 7);
                AddTriangleIndices_CCW_123(TriangleIndicesFrontSide, 7, 2, 3);
                AddTriangleIndices_CCW_123(TriangleIndicesFrontSide, 7, 3, 6);
                AddTriangleIndices_CCW_123(TriangleIndicesFrontSide, 6, 4, 5);
                AddTriangleIndices_CCW_123(TriangleIndicesFrontSide, 6, 3, 4);
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
            TriangleIndicesBackSide = new Int32Collection(24);

            if (bIndicesCW)
            {
                AddTriangleIndices_CCW_123(TriangleIndicesBackSide, ITotNoPoints, ITotNoPoints + 1, ITotNoPoints + 2);
                AddTriangleIndices_CCW_123(TriangleIndicesBackSide, ITotNoPoints, ITotNoPoints + 2, ITotNoPoints + 9);
                AddTriangleIndices_CCW_123(TriangleIndicesBackSide, ITotNoPoints + 9, ITotNoPoints + 8, ITotNoPoints + 2);
                AddTriangleIndices_CCW_123(TriangleIndicesBackSide, ITotNoPoints + 8, ITotNoPoints + 7, ITotNoPoints + 2);
                AddTriangleIndices_CCW_123(TriangleIndicesBackSide, ITotNoPoints + 7, ITotNoPoints + 3, ITotNoPoints + 2);
                AddTriangleIndices_CCW_123(TriangleIndicesBackSide, ITotNoPoints + 7, ITotNoPoints + 6, ITotNoPoints + 3);
                AddTriangleIndices_CCW_123(TriangleIndicesBackSide, ITotNoPoints + 6, ITotNoPoints + 5, ITotNoPoints + 3);
                AddTriangleIndices_CCW_123(TriangleIndicesBackSide, ITotNoPoints + 6, ITotNoPoints + 3, ITotNoPoints + 4);
            }
            else
            {
                AddTriangleIndices_CW_123(TriangleIndicesBackSide, ITotNoPoints, ITotNoPoints + 1, ITotNoPoints + 2);
                AddTriangleIndices_CW_123(TriangleIndicesBackSide, ITotNoPoints, ITotNoPoints + 2, ITotNoPoints + 9);
                AddTriangleIndices_CW_123(TriangleIndicesBackSide, ITotNoPoints + 9, ITotNoPoints + 8, ITotNoPoints + 2);
                AddTriangleIndices_CW_123(TriangleIndicesBackSide, ITotNoPoints + 8, ITotNoPoints + 7, ITotNoPoints + 2);
                AddTriangleIndices_CW_123(TriangleIndicesBackSide, ITotNoPoints + 7, ITotNoPoints + 3, ITotNoPoints + 2);
                AddTriangleIndices_CW_123(TriangleIndicesBackSide, ITotNoPoints + 7, ITotNoPoints + 6, ITotNoPoints + 3);
                AddTriangleIndices_CW_123(TriangleIndicesBackSide, ITotNoPoints + 6, ITotNoPoints + 5, ITotNoPoints + 3);
                AddTriangleIndices_CW_123(TriangleIndicesBackSide, ITotNoPoints + 6, ITotNoPoints + 3, ITotNoPoints + 4);
            }
        }
    }
}
