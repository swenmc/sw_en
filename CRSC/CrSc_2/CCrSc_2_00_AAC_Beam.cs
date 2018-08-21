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
    }
}
