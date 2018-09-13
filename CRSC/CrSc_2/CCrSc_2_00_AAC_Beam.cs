using System.Collections.Generic;
using System.Windows;

namespace CRSC
{
    public class CCrSc_2_00_AAC_Beam : CCrSc_2_00
    {
        // Solid rectangle beam

        public CCrSc_2_00_AAC_Beam()
        {
        }
        public CCrSc_2_00_AAC_Beam(float fh, float fb/*, float ft*/)
        {
            //ITotNoPoints = 4;
            IsShapeSolid = true;
            ITotNoPoints = 4;
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

            // Fill list of indices for drawing of surface - triangles edges
            loadCrScIndices();
        }

        public new void CalcCrSc_Coord()
        {
            // Point No. 1
            CrScPointsOut[0, 0] = 0f;                          // y
            CrScPointsOut[0, 1] = (float)h;                          // z

            // Point No. 2
            CrScPointsOut[1, 0] = (float)b;                          // y
            CrScPointsOut[1, 1] = CrScPointsOut[0, 1];         // z

            // Point No. 3
            CrScPointsOut[2, 0] = CrScPointsOut[1, 0];         // y
            CrScPointsOut[2, 1] = 0;                           // z

            // Point No. 4
            CrScPointsOut[3, 0] = CrScPointsOut[0, 0];         // y
            CrScPointsOut[3, 1] = CrScPointsOut[2, 1];         // z
        }

        //public new void CalcCrSc_Coord()
        //{
        //    // Point No. 1            
        //    CrScPointsOut.Add(new Point(0, h));

        //    // Point No. 2            
        //    CrScPointsOut.Add(new Point(b, CrScPointsOut[0].Y));

        //    // Point No. 3
        //    CrScPointsOut.Add(new Point(CrScPointsOut[1].X, 0));

        //    // Point No. 4
        //    CrScPointsOut.Add(new Point(CrScPointsOut[0].X, CrScPointsOut[2].Y));
        //}
    }
}
