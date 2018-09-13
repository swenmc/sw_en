using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace CRSC
{
    public class CCrSc_3_51_BOX_TEMP : CSC
    {
        // Thin-walled box section - monosymmetrical or double symmetrical

        private float m_ft_f; // Flange Thickness / Hrubka pasnice
        private float m_ft_w; // Web Thickness  / Hrubka steny/stojiny
        private float m_fd;

        private float fz_stif1;
        private float fy_stif1;
        private float fz_stif2;
        private float fy_stif2;

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

        public CCrSc_3_51_BOX_TEMP(float fh, float fb, float ft, Color color_temp)
        {
            //ITotNoPoints = 16;
            IsShapeSolid = false;
            INoPointsIn = INoPointsOut = 16; // vykreslujeme ako n-uholnik, pocet bodov n

            h = fh;
            b = fb;
            m_ft_f = ft;
            m_ft_w = ft;

            CSColor = color_temp;

            m_fd = fh - 2 * ft;

            fz_stif1 = 0.004f;
            fy_stif1 = 0.012f;

            fz_stif2 = 0.020f;
            fy_stif2 = 0.006f;

            // Create Array - allocate memory
            CrScPointsOut = new float[INoPointsOut, 2];
            CrScPointsIn = new float[INoPointsIn, 2];

            //CrScPointsOut = new List<Point>(INoPointsOut);
            //CrScPointsIn = new List<Point>(INoPointsIn);

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
            CrScPointsOut[0, 0] = -(float)b / 2f;                // y
            CrScPointsOut[0, 1] = (float)h / 2f;                 // z

            // Point No. 2
            CrScPointsOut[1, 0] = -fy_stif1 / 2f;                // y
            CrScPointsOut[1, 1] = CrScPointsOut[0, 1];           // z

            // Point No. 3
            CrScPointsOut[2, 0] = 0;                             // y
            CrScPointsOut[2, 1] = CrScPointsOut[0, 1] - fz_stif1 + m_ft_f; // z

            // Point No. 4
            CrScPointsOut[3, 0] = - CrScPointsOut[1, 0];         // y
            CrScPointsOut[3, 1] = CrScPointsOut[0, 1];           // z

            // Point No. 5
            CrScPointsOut[4, 0] = -CrScPointsOut[0, 0];          // y
            CrScPointsOut[4, 1] = CrScPointsOut[0, 1];           // z

            // Point No. 6
            CrScPointsOut[5, 0] = -CrScPointsOut[0, 0];          // y
            CrScPointsOut[5, 1] = fz_stif2 / 2f;                // z

            // Point No. 7
            CrScPointsOut[6, 0] = -CrScPointsOut[0, 0] - fy_stif2 + m_ft_w; // y
            CrScPointsOut[6, 1] = 0;                             // z

            // Point No. 8
            CrScPointsOut[7, 0] = CrScPointsOut[5, 0];           // y
            CrScPointsOut[7, 1] = -CrScPointsOut[5, 1];          // z

            // Point No. 9
            CrScPointsOut[8, 0] = -CrScPointsOut[0, 0];          // y
            CrScPointsOut[8, 1] = -CrScPointsOut[0, 1];          // z

            // Point No. 10
            CrScPointsOut[9, 0] = CrScPointsOut[3, 0];           // y
            CrScPointsOut[9, 1] = -CrScPointsOut[3, 1];          // z

            // Point No. 11
            CrScPointsOut[10, 0] = CrScPointsOut[2, 0];          // y
            CrScPointsOut[10, 1] = -CrScPointsOut[2, 1];         // z

            // Point No. 12
            CrScPointsOut[11, 0] = CrScPointsOut[1, 0];          // y
            CrScPointsOut[11, 1] = -CrScPointsOut[1, 1];         // z

            // Point No. 13
            CrScPointsOut[12, 0] = CrScPointsOut[0, 0];          // y
            CrScPointsOut[12, 1] = -CrScPointsOut[0, 1];         // z

            // Point No. 14
            CrScPointsOut[13, 0] = -CrScPointsOut[7, 0];         // y
            CrScPointsOut[13, 1] = CrScPointsOut[7, 1];          // z

            // Point No. 15
            CrScPointsOut[14, 0] = -CrScPointsOut[6, 0];         // y
            CrScPointsOut[14, 1] = CrScPointsOut[6, 1];          // z

            // Point No. 16
            CrScPointsOut[15, 0] = -CrScPointsOut[5, 0];         // y
            CrScPointsOut[15, 1] = CrScPointsOut[5, 1];          // z

            // Internal

            // Point No. 1
            CrScPointsIn[0, 0] = CrScPointsOut[0, 0] + m_ft_w;   // y
            CrScPointsIn[0, 1] = CrScPointsOut[0, 1] - m_ft_f;   // z

            // Point No. 2
            CrScPointsIn[1, 0] = -fy_stif1 / 2f;                 // y
            CrScPointsIn[1, 1] = CrScPointsIn[0, 1];             // z

            // Point No. 3
            CrScPointsIn[2, 0] = 0;                             // y
            CrScPointsIn[2, 1] = CrScPointsOut[0, 1] - fz_stif1; // z

            // Point No. 4
            CrScPointsIn[3, 0] = -CrScPointsIn[1, 0];         // y
            CrScPointsIn[3, 1] = CrScPointsIn[0, 1];           // z

            // Point No. 5
            CrScPointsIn[4, 0] = -CrScPointsIn[0, 0];          // y
            CrScPointsIn[4, 1] = CrScPointsIn[0, 1];           // z

            // Point No. 6
            CrScPointsIn[5, 0] = CrScPointsOut[5, 0] - m_ft_w;  // y
            CrScPointsIn[5, 1] = CrScPointsOut[5, 1];         // z

            // Point No. 7
            CrScPointsIn[6, 0] = CrScPointsOut[5, 0] - fy_stif2; // y
            CrScPointsIn[6, 1] = 0;                             // z

            // Point No. 8
            CrScPointsIn[7, 0] = CrScPointsIn[5, 0];           // y
            CrScPointsIn[7, 1] = -CrScPointsIn[5, 1];          // z

            // Point No. 9
            CrScPointsIn[8, 0] = -CrScPointsIn[0, 0];          // y
            CrScPointsIn[8, 1] = -CrScPointsIn[0, 1];          // z

            // Point No. 10
            CrScPointsIn[9, 0] = CrScPointsIn[3, 0];           // y
            CrScPointsIn[9, 1] = -CrScPointsIn[3, 1];          // z

            // Point No. 11
            CrScPointsIn[10, 0] = CrScPointsIn[2, 0];          // y
            CrScPointsIn[10, 1] = -CrScPointsIn[2, 1];         // z

            // Point No. 12
            CrScPointsIn[11, 0] = CrScPointsIn[1, 0];          // y
            CrScPointsIn[11, 1] = -CrScPointsIn[1, 1];         // z

            // Point No. 13
            CrScPointsIn[12, 0] = CrScPointsIn[0, 0];          // y
            CrScPointsIn[12, 1] = -CrScPointsIn[0, 1];         // z

            // Point No. 14
            CrScPointsIn[13, 0] = -CrScPointsIn[7, 0];         // y
            CrScPointsIn[13, 1] = CrScPointsIn[7, 1];          // z

            // Point No. 15
            CrScPointsIn[14, 0] = -CrScPointsIn[6, 0];         // y
            CrScPointsIn[14, 1] = CrScPointsIn[6, 1];          // z

            // Point No. 16
            CrScPointsIn[15, 0] = -CrScPointsIn[5, 0];         // y
            CrScPointsIn[15, 1] = CrScPointsIn[5, 1];          // z
        }
    }
}
