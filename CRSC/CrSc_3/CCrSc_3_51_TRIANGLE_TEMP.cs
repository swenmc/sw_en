using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using MATH;

namespace CRSC
{
    public class CCrSc_3_51_TRIANGLE_TEMP : CCrSc_TW
    {
        // Thin-walled triangle

        private float m_ft; 

        public float Ft
        {
            get { return m_ft; }
            set { m_ft = value; }
        }

        public CCrSc_3_51_TRIANGLE_TEMP(float fh = 0.866025f * 0.5f, float fb = 0.5f, float ft = 0.002f)
        {
            CSColor = Colors.DarkGreen;

            //ITotNoPoints = 3;
            IsShapeSolid = false;
            INoPointsIn = INoPointsOut = 3; // vykreslujeme ako n-uholnik, pocet bodov n

            h = fh;
            b = fb;
            m_ft = ft;

            // Create Array - allocate memory
            CrScPointsOut = new float[INoPointsOut, 2];
            CrScPointsIn = new float[INoPointsIn, 2];

            // Fill Array Data
            CalcCrSc_Coord();

            // Fill list of indices for drawing of surface - triangles edges

            // Particular indices Rozpracovane pre vykreslovanie cela prutu inou farbou
            loadCrScIndicesFrontSide();
            loadCrScIndicesShell();
            loadCrScIndicesBackSide();
        }

        public void CalcCrSc_Coord()
        {
            // Fill Point Array Data in LCS (Local Coordinate System of Cross-Section, horizontal y, vertical - z)

            // Point No. 1
            CrScPointsOut[0, 0] = 0;                                    // y
            CrScPointsOut[0, 1] = (float)h * (2f / 3f);                 // z

            // Point No. 2
            CrScPointsOut[1, 0] = (float)b / 2f;                        // y
            CrScPointsOut[1, 1] = -(float)h * (1f / 3f);                // z

            // Point No. 3
            CrScPointsOut[2, 0] = -CrScPointsOut[1, 0];                 // y
            CrScPointsOut[2, 1] = CrScPointsOut[1, 1];                  // z

            float fAlphaDeg = 30f;

            // Internal

            // Point No. 1
            CrScPointsIn[0, 0] = CrScPointsOut[0, 0];   // y
            CrScPointsIn[0, 1] = CrScPointsOut[0, 1] - m_ft / (float)Math.Sin(fAlphaDeg * MathF.fPI / 180f);   // z

            // Point No. 2
            CrScPointsIn[1, 0] = CrScPointsOut[1, 0] - m_ft / (float)Math.Tan(fAlphaDeg * MathF.fPI / 180f);   // y
            CrScPointsIn[1, 1] = CrScPointsOut[1, 1] + m_ft;   // z

            // Point No. 3
            CrScPointsIn[2, 0] = -CrScPointsIn[1, 0];  // y
            CrScPointsIn[2, 1] = CrScPointsIn[1, 1];   // z
        }
    }
}
