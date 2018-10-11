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
            CrScPointsOut = new List<Point>(INoPointsOut);
            CrScPointsIn = new List<Point>(INoPointsIn);

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
            float[,] CrScPointsOutArr = new float[INoPointsOut, 2];
            float[,] CrScPointsInArr = new float[INoPointsIn, 2];
            // Point No. 1
            CrScPointsOutArr[0, 0] = -(float)b / 2f;                // y
            CrScPointsOutArr[0, 1] = (float)h / 2f;                 // z

            // Point No. 2
            CrScPointsOutArr[1, 0] = -fy_stif1 / 2f;                // y
            CrScPointsOutArr[1, 1] = CrScPointsOutArr[0, 1];           // z

            // Point No. 3
            CrScPointsOutArr[2, 0] = 0;                             // y
            CrScPointsOutArr[2, 1] = CrScPointsOutArr[0, 1] - fz_stif1 + m_ft_f; // z

            // Point No. 4
            CrScPointsOutArr[3, 0] = -CrScPointsOutArr[1, 0];         // y
            CrScPointsOutArr[3, 1] = CrScPointsOutArr[0, 1];           // z

            // Point No. 5
            CrScPointsOutArr[4, 0] = -CrScPointsOutArr[0, 0];          // y
            CrScPointsOutArr[4, 1] = CrScPointsOutArr[0, 1];           // z

            // Point No. 6
            CrScPointsOutArr[5, 0] = -CrScPointsOutArr[0, 0];          // y
            CrScPointsOutArr[5, 1] = fz_stif2 / 2f;                // z

            // Point No. 7
            CrScPointsOutArr[6, 0] = -CrScPointsOutArr[0, 0] - fy_stif2 + m_ft_w; // y
            CrScPointsOutArr[6, 1] = 0;                             // z

            // Point No. 8
            CrScPointsOutArr[7, 0] = CrScPointsOutArr[5, 0];           // y
            CrScPointsOutArr[7, 1] = -CrScPointsOutArr[5, 1];          // z

            // Point No. 9
            CrScPointsOutArr[8, 0] = -CrScPointsOutArr[0, 0];          // y
            CrScPointsOutArr[8, 1] = -CrScPointsOutArr[0, 1];          // z

            // Point No. 10
            CrScPointsOutArr[9, 0] = CrScPointsOutArr[3, 0];           // y
            CrScPointsOutArr[9, 1] = -CrScPointsOutArr[3, 1];          // z

            // Point No. 11
            CrScPointsOutArr[10, 0] = CrScPointsOutArr[2, 0];          // y
            CrScPointsOutArr[10, 1] = -CrScPointsOutArr[2, 1];         // z

            // Point No. 12
            CrScPointsOutArr[11, 0] = CrScPointsOutArr[1, 0];          // y
            CrScPointsOutArr[11, 1] = -CrScPointsOutArr[1, 1];         // z

            // Point No. 13
            CrScPointsOutArr[12, 0] = CrScPointsOutArr[0, 0];          // y
            CrScPointsOutArr[12, 1] = -CrScPointsOutArr[0, 1];         // z

            // Point No. 14
            CrScPointsOutArr[13, 0] = -CrScPointsOutArr[7, 0];         // y
            CrScPointsOutArr[13, 1] = CrScPointsOutArr[7, 1];          // z

            // Point No. 15
            CrScPointsOutArr[14, 0] = -CrScPointsOutArr[6, 0];         // y
            CrScPointsOutArr[14, 1] = CrScPointsOutArr[6, 1];          // z

            // Point No. 16
            CrScPointsOutArr[15, 0] = -CrScPointsOutArr[5, 0];         // y
            CrScPointsOutArr[15, 1] = CrScPointsOutArr[5, 1];          // z

            // Internal

            // Point No. 1
            CrScPointsInArr[0, 0] = CrScPointsOutArr[0, 0] + m_ft_w;   // y
            CrScPointsInArr[0, 1] = CrScPointsOutArr[0, 1] - m_ft_f;   // z

            // Point No. 2
            CrScPointsInArr[1, 0] = -fy_stif1 / 2f;                 // y
            CrScPointsInArr[1, 1] = CrScPointsInArr[0, 1];             // z

            // Point No. 3
            CrScPointsInArr[2, 0] = 0;                             // y
            CrScPointsInArr[2, 1] = CrScPointsOutArr[0, 1] - fz_stif1; // z

            // Point No. 4
            CrScPointsInArr[3, 0] = -CrScPointsInArr[1, 0];         // y
            CrScPointsInArr[3, 1] = CrScPointsInArr[0, 1];           // z

            // Point No. 5
            CrScPointsInArr[4, 0] = -CrScPointsInArr[0, 0];          // y
            CrScPointsInArr[4, 1] = CrScPointsInArr[0, 1];           // z

            // Point No. 6
            CrScPointsInArr[5, 0] = CrScPointsOutArr[5, 0] - m_ft_w;  // y
            CrScPointsInArr[5, 1] = CrScPointsOutArr[5, 1];         // z

            // Point No. 7
            CrScPointsInArr[6, 0] = CrScPointsOutArr[5, 0] - fy_stif2; // y
            CrScPointsInArr[6, 1] = 0;                             // z

            // Point No. 8
            CrScPointsInArr[7, 0] = CrScPointsInArr[5, 0];           // y
            CrScPointsInArr[7, 1] = -CrScPointsInArr[5, 1];          // z

            // Point No. 9
            CrScPointsInArr[8, 0] = -CrScPointsInArr[0, 0];          // y
            CrScPointsInArr[8, 1] = -CrScPointsInArr[0, 1];          // z

            // Point No. 10
            CrScPointsInArr[9, 0] = CrScPointsInArr[3, 0];           // y
            CrScPointsInArr[9, 1] = -CrScPointsInArr[3, 1];          // z

            // Point No. 11
            CrScPointsInArr[10, 0] = CrScPointsInArr[2, 0];          // y
            CrScPointsInArr[10, 1] = -CrScPointsInArr[2, 1];         // z

            // Point No. 12
            CrScPointsInArr[11, 0] = CrScPointsInArr[1, 0];          // y
            CrScPointsInArr[11, 1] = -CrScPointsInArr[1, 1];         // z

            // Point No. 13
            CrScPointsInArr[12, 0] = CrScPointsInArr[0, 0];          // y
            CrScPointsInArr[12, 1] = -CrScPointsInArr[0, 1];         // z

            // Point No. 14
            CrScPointsInArr[13, 0] = -CrScPointsInArr[7, 0];         // y
            CrScPointsInArr[13, 1] = CrScPointsInArr[7, 1];          // z

            // Point No. 15
            CrScPointsInArr[14, 0] = -CrScPointsInArr[6, 0];         // y
            CrScPointsInArr[14, 1] = CrScPointsInArr[6, 1];          // z

            // Point No. 16
            CrScPointsInArr[15, 0] = -CrScPointsInArr[5, 0];         // y
            CrScPointsInArr[15, 1] = CrScPointsInArr[5, 1];          // z


            for (int i = 0; i < CrScPointsOutArr.GetLength(0); i++)
            {
                CrScPointsOut.Add(new Point(CrScPointsOutArr[i, 0], CrScPointsOutArr[i, 1]));
            }
            for (int i = 0; i < CrScPointsInArr.GetLength(0); i++)
            {
                CrScPointsIn.Add(new Point(CrScPointsInArr[i, 0], CrScPointsInArr[i, 1]));
            }
        }
    }
}
