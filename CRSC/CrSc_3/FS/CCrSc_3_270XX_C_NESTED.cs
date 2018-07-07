using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using MATH;
using BaseClasses;

namespace CRSC
{
    public class CCrSc_3_270XX_C_NESTED : CCrSc_TW
    {
        // Thin-walled box section - monosymmetrical or double symmetrical

        private float m_ft_f; // Flange Thickness / Hrubka pasnice
        private float m_ft_w; // Web Thickness  / Hrubka steny/stojiny
        private float m_fd;

        private float fr_1_in;
        private float fr_1_out;

        private float fz_1stif1_out;
        private float fy_1stif1;
        private float fy_flange;
        private float fy_2stif1_out;
        private float fz_2stif1;
        private float fz_2stif2;

        private float fz_23stif;

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
        public float Fd
        {
            get { return m_fd; }
            set { m_fd = value; }
        }

        public CCrSc_3_270XX_C_NESTED(float fh, float fb, float ft, Color color_temp)
        {
            CSColor = color_temp;  // Set cross-section color

            //ITotNoPoints = 32;
            IsShapeSolid = false;
            INoPointsIn = INoPointsOut = 32; // vykreslujeme ako n-uholnik, pocet bodov n

            h = fh;
            b = fb;
            m_ft_f = ft;
            m_ft_w = ft;

            CSColor = color_temp;

            m_fd = fh - 2 * ft;

            fr_1_in = 0.007f;
            fr_1_out = fr_1_in + ft;

            fz_1stif1_out = 0.004f;
            fy_1stif1 = 0.02f;

            fy_flange = 0.017f;

            fy_2stif1_out = 0.009f;
            fz_2stif1 = 0.018f;
            fz_2stif2 = 0.020f;

            fz_23stif = 0.060f; // Straight part

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
            CrScPointsOut[0, 0] = 0.001f;                                          // y
            CrScPointsOut[0, 1] = 0.5f * (float)h - fz_1stif1_out;                 // z

            // Point No. 2
            CrScPointsOut[1, 0] = CrScPointsOut[0, 0] + 0.5f * fy_1stif1;          // y
            CrScPointsOut[1, 1] = 0.5f * (float)h;                             // z

            // Point No. 3
            CrScPointsOut[2, 0] = 0.5f * (float)b - fr_1_out;                      // y
            CrScPointsOut[2, 1] = 0.5f * (float)h;                                 // z

            // Point No. 4
            CrScPointsOut[3, 0] = 0.5f * (float)b;                                 // y
            CrScPointsOut[3, 1] = CrScPointsOut[2, 1] - fr_1_out;                  // z

            // Point No. 5
            CrScPointsOut[4, 0] = CrScPointsOut[3, 0];                     // y
            CrScPointsOut[4, 1] = CrScPointsOut[3, 1] - (0.5f * (float)h - fr_1_out - 2f * fz_2stif1 - fz_2stif2 - 0.5f * fz_23stif);                     // z

            // Point No. 6
            CrScPointsOut[5, 0] = CrScPointsOut[4, 0] - fy_2stif1_out;     // y
            CrScPointsOut[5, 1] = CrScPointsOut[4, 1] - fz_2stif1;         // z

            // Point No. 7
            CrScPointsOut[6, 0] = CrScPointsOut[5, 0];                     // y
            CrScPointsOut[6, 1] = CrScPointsOut[5, 1] - fz_2stif2;         // z

            // Point No. 8
            CrScPointsOut[7, 0] = CrScPointsOut[4, 0];                     // y
            CrScPointsOut[7, 1] = CrScPointsOut[6, 1] - fz_2stif1;         // z

            // Internal

            // Point No. 1
            CrScPointsIn[0, 0] = 0.001f;                                          // y
            CrScPointsIn[0, 1] = 0.5f * (float)h - fz_1stif1_out - m_ft_f;        // z

            // Point No. 2
            CrScPointsIn[1, 0] = CrScPointsIn[0, 0] + 0.5f * fy_1stif1;          // y
            CrScPointsIn[1, 1] = 0.5f * (float)h - m_ft_f;                             // z

            // Point No. 3
            CrScPointsIn[2, 0] = 0.5f * (float)b - fr_1_out;              // y
            CrScPointsIn[2, 1] = 0.5f * (float)h - m_ft_f;                // z

            // Point No. 4
            CrScPointsIn[3, 0] = 0.5f * (float)b - m_ft_w;               // y
            CrScPointsIn[3, 1] = CrScPointsOut[3, 1];                    // z

            // Point No. 5
            CrScPointsIn[4, 0] = CrScPointsIn[3, 0];                     // y
            CrScPointsIn[4, 1] = CrScPointsIn[3, 1] - (0.5f * (float)h - fr_1_out - 2f * fz_2stif1 - fz_2stif2 - 0.5f * fz_23stif);                     // z

            // Point No. 6
            CrScPointsIn[5, 0] = CrScPointsIn[4, 0] - fy_2stif1_out;     // y
            CrScPointsIn[5, 1] = CrScPointsIn[4, 1] - fz_2stif1;         // z

            // Point No. 7
            CrScPointsIn[6, 0] = CrScPointsIn[5, 0];                     // y
            CrScPointsIn[6, 1] = CrScPointsIn[5, 1] - fz_2stif2;         // z

            // Point No. 8
            CrScPointsIn[7, 0] = CrScPointsIn[4, 0];                     // y
            CrScPointsIn[7, 1] = CrScPointsIn[6, 1] - fz_2stif1;         // z

            // Mirror about y-y
            for (int i = 0; i < INoPointsOut / 4; i++)
            {
                CrScPointsOut[INoPointsOut / 2 - i - 1, 0] = CrScPointsOut[i, 0];                  // Outside
                CrScPointsOut[INoPointsOut / 2 - i - 1, 1] = -CrScPointsOut[i, 1];                 // Outside

                CrScPointsIn[INoPointsIn / 2 - i - 1, 0] = CrScPointsIn[i, 0];                     // Inside
                CrScPointsIn[INoPointsIn / 2 - i - 1, 1] = -CrScPointsIn[i, 1];                    // Inside
            }

            // Mirror about z-z
            for (int i = 0; i < INoPointsOut / 2; i++)
            {
                CrScPointsOut[INoPointsOut - i - 1, 0] = -CrScPointsOut[i, 0];                     // Outside
                CrScPointsOut[INoPointsOut - i - 1, 1] = CrScPointsOut[i, 1];                      // Outside

                CrScPointsIn[INoPointsIn - i - 1, 0] = -CrScPointsIn[i, 0];                        // Inside
                CrScPointsIn[INoPointsIn - i - 1, 1] = CrScPointsIn[i, 1];                         // Inside
            }
        }
    }
}
