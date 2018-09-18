using MATH;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace CRSC
{
    public class CCrSc_3_63020_BOX : CSC
    {
        // Thin-walled box section - monosymmetrical or double symmetrical

        private float m_ft_f; // Flange Thickness / Hrubka pasnice
        private float m_ft_w; // Web Thickness  / Hrubka steny/stojiny
        private float m_fd;

        private float fz_1stif1_out;
        private float fy_1stif1;
        private float fy_1stif2;
        private float fr_1_in;
        private float fr_1_out;

        private float fy_2stif1_out;
        private float fz_2stif1;
        private float fz_2stif2;

        private float fy_3stif1_out;
        private float fz_3stif1;
        private float fz_3stif2;

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

        public CCrSc_3_63020_BOX(float fh, float fb, float ft, float ft_flange, Color color_temp)
        {
            Name = "Box " + (fh * 1000).ToString() + (ft * 1000 * 10).ToString(); // Original Description
            Name = "Box " + (fh * 1000).ToString() + (20).ToString(); // Formsteel Description
            NameDatabase = (fh * 1000).ToString() + (20).ToString();

            // Temporary  identify stiffeners acc. to flange thickness
            if (0.003f < ft_flange && ft_flange < 0.005)
            {
                Name = "Box " + (fh * 1000).ToString() + (20).ToString() + " single stiffener"; // Formsteel Description
                NameDatabase = (fh * 1000).ToString() + (20).ToString() + "s1";
            }
            else if (ft_flange > 0.005)
            {
                Name = "Box " + (fh * 1000).ToString() + (20).ToString() + " double stiffener"; // Formsteel Description
                NameDatabase = (fh * 1000).ToString() + (20).ToString() + "s2";
            }

            CSColor = color_temp;  // Set cross-section color

            //ITotNoPoints = 40;
            IsShapeSolid = false;
            INoPointsIn = INoPointsOut = 40; // vykreslujeme ako n-uholnik, pocet bodov n
            ITotNoPoints = INoPointsIn + INoPointsOut;

            h = fh;
            b = fb;
            m_ft_f = ft_flange;
            m_ft_w = ft;

            m_fd = fh - 2 * ft;

            fz_1stif1_out = 0.015f;
            fy_1stif1 = 0.026f;
            fy_1stif2 = 0.026f;
            fr_1_in = 0.023f;
            fr_1_out = fr_1_in + ft;

            fy_2stif1_out = 0.010f;
            fz_2stif1 = 0.010f;
            fz_2stif2 = 0.020f;

            fy_3stif1_out = 0.030f;
            fz_3stif1 = 0.0400f;
            fz_3stif2 = 0.100f;

            fz_23stif = 0.100f; // Straight part

            b_in = b - 2 * MathF.Max(fy_2stif1_out, fy_3stif1_out) - 2 * m_ft_w;
            h_in = h - 2 * fz_1stif1_out - 2 * m_ft_f;

            // Create Array - allocate memory
            //CrScPointsOut = new float[INoPointsOut, 2];
            //CrScPointsIn = new float[INoPointsIn, 2];

            CrScPointsOut = new List<Point>(INoPointsOut);
            CrScPointsIn = new List<Point>(INoPointsIn);

            // Fill Array Data
            CalcCrSc_Coord();

            ChangeCoordToCentroid(); // Temp - TODO doriesit zadavanie bodov (CW, CCW), osove systemy, orientaciu os a zjednotit zadanie pre vsetky prierezy

            // Fill list of indices for drawing of surface - triangles edges

            // Particular indices - distinguished colors of member surfaces
            loadCrScIndicesFrontSide();
            loadCrScIndicesShell();
            loadCrScIndicesBackSide();

            // Complex indices - one color or member
            loadCrScIndices();

            // Wireframe Indices
            loadCrScWireFrameIndicesFrontSide();
            loadCrScWireFrameIndicesBackSide();
            loadCrScWireFrameIndicesLaterals();

            FillCrScPropertiesByTableData();
        }

        //public void CalcCrSc_Coord()
        //{
        //    // Fill Point Array Data in LCS (Local Coordinate System of Cross-Section, horizontal y, vertical - z)

        //    // Point No. 1
        //    CrScPointsOut[0, 0] = fy_1stif1 / 2f;                          // y
        //    CrScPointsOut[0, 1] = (float)h / 2f - fz_1stif1_out;           // z

        //    // Point No. 2
        //    CrScPointsOut[1, 0] = CrScPointsOut[0, 0] + fy_1stif2;         // y
        //    CrScPointsOut[1, 1] = (float)h / 2f;                           // z

        //    // Point No. 3
        //    CrScPointsOut[2, 0] = 0.5f * (float)b - fr_1_out;              // y
        //    CrScPointsOut[2, 1] = CrScPointsOut[1, 1];                     // z

        //    // Point No. 4
        //    CrScPointsOut[3, 0] = 0.5f * (float)b;                         // y
        //    CrScPointsOut[3, 1] = CrScPointsOut[2, 1] - fr_1_out;          // z

        //    // Point No. 5
        //    CrScPointsOut[4, 0] = CrScPointsOut[3, 0];                     // y
        //    CrScPointsOut[4, 1] = CrScPointsOut[3, 1] - (0.5f * (float)h - fr_1_out - 2f * fz_2stif1 - fz_2stif2 - fz_23stif - fz_3stif1 - 0.5f * fz_3stif2);                     // z

        //    // Point No. 6
        //    CrScPointsOut[5, 0] = CrScPointsOut[4, 0] - fy_2stif1_out;     // y
        //    CrScPointsOut[5, 1] = CrScPointsOut[4, 1] - fz_2stif1;         // z

        //    // Point No. 7
        //    CrScPointsOut[6, 0] = CrScPointsOut[5, 0];                     // y
        //    CrScPointsOut[6, 1] = CrScPointsOut[5, 1] - fz_2stif2;         // z

        //    // Point No. 8
        //    CrScPointsOut[7, 0] = CrScPointsOut[4, 0];                     // y
        //    CrScPointsOut[7, 1] = CrScPointsOut[6, 1] - fz_2stif1;         // z

        //    // Point No. 9
        //    CrScPointsOut[8, 0] = CrScPointsOut[7, 0];                     // y
        //    CrScPointsOut[8, 1] = CrScPointsOut[7, 1] - fz_23stif;         // z

        //    // Point No. 10
        //    CrScPointsOut[9, 0] = CrScPointsOut[8, 0] - fy_3stif1_out;     // y
        //    CrScPointsOut[9, 1] = CrScPointsOut[8, 1] - fz_3stif1;         // z

        //    // Internal

        //    // Point No. 1
        //    CrScPointsIn[0, 0] = fy_1stif1 / 2f;                          // y
        //    CrScPointsIn[0, 1] = (float)h / 2f - fz_1stif1_out - m_ft_f;  // z

        //    // Point No. 2
        //    CrScPointsIn[1, 0] = CrScPointsIn[0, 0] + fy_1stif2;         // y
        //    CrScPointsIn[1, 1] = (float)h / 2f - m_ft_f;                 // z

        //    // Point No. 3
        //    CrScPointsIn[2, 0] = 0.5f * (float)b - fr_1_out;              // y
        //    CrScPointsIn[2, 1] = CrScPointsIn[1, 1];                     // z

        //    // Point No. 4
        //    CrScPointsIn[3, 0] = 0.5f * (float)b - m_ft_w;               // y
        //    CrScPointsIn[3, 1] = CrScPointsOut[3, 1];                    // z

        //    // Point No. 5
        //    CrScPointsIn[4, 0] = CrScPointsIn[3, 0];                     // y
        //    CrScPointsIn[4, 1] = CrScPointsOut[3, 1] - (0.5f * (float)h - fr_1_out - 2f * fz_2stif1 - fz_2stif2 - fz_23stif - fz_3stif1 - 0.5f * fz_3stif2);                     // z

        //    // Point No. 6
        //    CrScPointsIn[5, 0] = CrScPointsIn[4, 0] - fy_2stif1_out;     // y
        //    CrScPointsIn[5, 1] = CrScPointsIn[4, 1] - fz_2stif1;         // z

        //    // Point No. 7
        //    CrScPointsIn[6, 0] = CrScPointsIn[5, 0];                     // y
        //    CrScPointsIn[6, 1] = CrScPointsIn[5, 1] - fz_2stif2;         // z

        //    // Point No. 8
        //    CrScPointsIn[7, 0] = CrScPointsIn[4, 0];                     // y
        //    CrScPointsIn[7, 1] = CrScPointsIn[6, 1] - fz_2stif1;         // z

        //    // Point No. 9
        //    CrScPointsIn[8, 0] = CrScPointsIn[7, 0];                     // y
        //    CrScPointsIn[8, 1] = CrScPointsIn[7, 1] - fz_23stif;         // z

        //    // Point No. 10
        //    CrScPointsIn[9, 0] = CrScPointsIn[8, 0] - fy_3stif1_out;     // y
        //    CrScPointsIn[9, 1] = CrScPointsIn[8, 1] - fz_3stif1;         // z

        //    // Mirror about y-y
        //    for (int i = 0; i < INoPointsOut / 4; i++)
        //    {
        //        CrScPointsOut[INoPointsOut / 2 - i - 1, 0] = CrScPointsOut[i, 0];                  // Outside
        //        CrScPointsOut[INoPointsOut / 2 - i - 1, 1] = -CrScPointsOut[i, 1];                 // Outside

        //        CrScPointsIn[INoPointsIn / 2 - i - 1, 0] = CrScPointsIn[i, 0];                     // Inside
        //        CrScPointsIn[INoPointsIn / 2 - i - 1, 1] = -CrScPointsIn[i, 1];                    // Inside
        //    }

        //    // Mirror about z-z
        //    for (int i = 0; i < INoPointsOut / 2; i++)
        //    {
        //        CrScPointsOut[INoPointsOut - i - 1, 0] = -CrScPointsOut[i, 0];                     // Outside
        //        CrScPointsOut[INoPointsOut - i - 1, 1] = CrScPointsOut[i, 1];                      // Outside

        //        CrScPointsIn[INoPointsIn - i - 1, 0] = -CrScPointsIn[i, 0];                        // Inside
        //        CrScPointsIn[INoPointsIn - i - 1, 1] = CrScPointsIn[i, 1];                         // Inside
        //    }
        //}

        public void CalcCrSc_Coord()
        {
            // Fill Point Array Data in LCS (Local Coordinate System of Cross-Section, horizontal y, vertical - z)
            float[,] CrScPointsOutArr = new float[INoPointsOut, 2];
            float[,] CrScPointsInArr = new float[INoPointsIn, 2];
            // Point No. 1
            CrScPointsOutArr[0, 0] = fy_1stif1 / 2f;                          // y
            CrScPointsOutArr[0, 1] = (float)h / 2f - fz_1stif1_out;           // z

            // Point No. 2
            CrScPointsOutArr[1, 0] = CrScPointsOutArr[0, 0] + fy_1stif2;         // y
            CrScPointsOutArr[1, 1] = (float)h / 2f;                           // z

            // Point No. 3
            CrScPointsOutArr[2, 0] = 0.5f * (float)b - fr_1_out;              // y
            CrScPointsOutArr[2, 1] = CrScPointsOutArr[1, 1];                     // z

            // Point No. 4
            CrScPointsOutArr[3, 0] = 0.5f * (float)b;                         // y
            CrScPointsOutArr[3, 1] = CrScPointsOutArr[2, 1] - fr_1_out;          // z

            // Point No. 5
            CrScPointsOutArr[4, 0] = CrScPointsOutArr[3, 0];                     // y
            CrScPointsOutArr[4, 1] = CrScPointsOutArr[3, 1] - (0.5f * (float)h - fr_1_out - 2f * fz_2stif1 - fz_2stif2 - fz_23stif - fz_3stif1 - 0.5f * fz_3stif2);                     // z

            // Point No. 6
            CrScPointsOutArr[5, 0] = CrScPointsOutArr[4, 0] - fy_2stif1_out;     // y
            CrScPointsOutArr[5, 1] = CrScPointsOutArr[4, 1] - fz_2stif1;         // z

            // Point No. 7
            CrScPointsOutArr[6, 0] = CrScPointsOutArr[5, 0];                     // y
            CrScPointsOutArr[6, 1] = CrScPointsOutArr[5, 1] - fz_2stif2;         // z

            // Point No. 8
            CrScPointsOutArr[7, 0] = CrScPointsOutArr[4, 0];                     // y
            CrScPointsOutArr[7, 1] = CrScPointsOutArr[6, 1] - fz_2stif1;         // z

            // Point No. 9
            CrScPointsOutArr[8, 0] = CrScPointsOutArr[7, 0];                     // y
            CrScPointsOutArr[8, 1] = CrScPointsOutArr[7, 1] - fz_23stif;         // z

            // Point No. 10
            CrScPointsOutArr[9, 0] = CrScPointsOutArr[8, 0] - fy_3stif1_out;     // y
            CrScPointsOutArr[9, 1] = CrScPointsOutArr[8, 1] - fz_3stif1;         // z

            // Internal

            // Point No. 1
            CrScPointsInArr[0, 0] = fy_1stif1 / 2f;                          // y
            CrScPointsInArr[0, 1] = (float)h / 2f - fz_1stif1_out - m_ft_f;  // z

            // Point No. 2
            CrScPointsInArr[1, 0] = CrScPointsInArr[0, 0] + fy_1stif2;         // y
            CrScPointsInArr[1, 1] = (float)h / 2f - m_ft_f;                 // z

            // Point No. 3
            CrScPointsInArr[2, 0] = 0.5f * (float)b - fr_1_out;              // y
            CrScPointsInArr[2, 1] = CrScPointsInArr[1, 1];                     // z

            // Point No. 4
            CrScPointsInArr[3, 0] = 0.5f * (float)b - m_ft_w;               // y
            CrScPointsInArr[3, 1] = CrScPointsOutArr[3, 1];                    // z

            // Point No. 5
            CrScPointsInArr[4, 0] = CrScPointsInArr[3, 0];                     // y
            CrScPointsInArr[4, 1] = CrScPointsOutArr[3, 1] - (0.5f * (float)h - fr_1_out - 2f * fz_2stif1 - fz_2stif2 - fz_23stif - fz_3stif1 - 0.5f * fz_3stif2);                     // z

            // Point No. 6
            CrScPointsInArr[5, 0] = CrScPointsInArr[4, 0] - fy_2stif1_out;     // y
            CrScPointsInArr[5, 1] = CrScPointsInArr[4, 1] - fz_2stif1;         // z

            // Point No. 7
            CrScPointsInArr[6, 0] = CrScPointsInArr[5, 0];                     // y
            CrScPointsInArr[6, 1] = CrScPointsInArr[5, 1] - fz_2stif2;         // z

            // Point No. 8
            CrScPointsInArr[7, 0] = CrScPointsInArr[4, 0];                     // y
            CrScPointsInArr[7, 1] = CrScPointsInArr[6, 1] - fz_2stif1;         // z

            // Point No. 9
            CrScPointsInArr[8, 0] = CrScPointsInArr[7, 0];                     // y
            CrScPointsInArr[8, 1] = CrScPointsInArr[7, 1] - fz_23stif;         // z

            // Point No. 10
            CrScPointsInArr[9, 0] = CrScPointsInArr[8, 0] - fy_3stif1_out;     // y
            CrScPointsInArr[9, 1] = CrScPointsInArr[8, 1] - fz_3stif1;         // z

            // Mirror about y-y
            for (int i = 0; i < INoPointsOut / 4; i++)
            {
                CrScPointsOutArr[INoPointsOut / 2 - i - 1, 0] = CrScPointsOutArr[i, 0];                  // Outside
                CrScPointsOutArr[INoPointsOut / 2 - i - 1, 1] = -CrScPointsOutArr[i, 1];                 // Outside

                CrScPointsInArr[INoPointsIn / 2 - i - 1, 0] = CrScPointsInArr[i, 0];                     // Inside
                CrScPointsInArr[INoPointsIn / 2 - i - 1, 1] = -CrScPointsInArr[i, 1];                    // Inside
            }

            // Mirror about z-z
            for (int i = 0; i < INoPointsOut / 2; i++)
            {
                CrScPointsOutArr[INoPointsOut - i - 1, 0] = -CrScPointsOutArr[i, 0];                     // Outside
                CrScPointsOutArr[INoPointsOut - i - 1, 1] = CrScPointsOutArr[i, 1];                      // Outside

                CrScPointsInArr[INoPointsIn - i - 1, 0] = -CrScPointsInArr[i, 0];                        // Inside
                CrScPointsInArr[INoPointsIn - i - 1, 1] = CrScPointsInArr[i, 1];                         // Inside
            }

            for (int i = 0; i < CrScPointsOutArr.GetLength(0); i++)
            {
                CrScPointsOut.Add(new Point(CrScPointsOutArr[i, 0], CrScPointsOutArr[i, 1]));
            }
            for (int i = 0; i < CrScPointsInArr.GetLength(0); i++)
            {
                CrScPointsIn.Add(new Point(CrScPointsInArr[i, 0], CrScPointsInArr[i, 1]));
            }
        }

        //public void ChangeCoordToCentroid() // Prepocita suradnice outline podla suradnic taziska
        //{
        //    // Temporary - odstranit po implementacii vypoctu
        //    D_y_gc = 0; // Temporary - TODO
        //    D_z_gc = 0;

        //    y_min = -b / 2;
        //    y_max = b / 2;
        //    z_min = -h / 2;
        //    z_max = h / 2;

        //    for (int i = 0; i < INoPointsOut; i++)
        //    {
        //        CrScPointsOut[i, 0] += (float)D_y_gc;
        //        CrScPointsOut[i, 1] += (float)D_z_gc;
        //    }

        //    for (int i = 0; i < INoPointsIn; i++)
        //    {
        //        CrScPointsIn[i, 0] += (float)D_y_gc;
        //        CrScPointsIn[i, 1] += (float)D_z_gc;
        //    }
        //}

        public void ChangeCoordToCentroid() // Prepocita suradnice outline podla suradnic taziska
        {
            // Temporary - odstranit po implementacii vypoctu
            D_y_gc = 0; // Temporary - TODO
            D_z_gc = 0;

            y_min = -b / 2;
            y_max = b / 2;
            z_min = -h / 2;
            z_max = h / 2;

            for (int i = 0; i < INoPointsOut; i++)
            {
                Point p = CrScPointsOut[i];
                p.X += D_y_gc;
                p.Y += D_z_gc;
                CrScPointsOut[i] = p;
            }

            for (int i = 0; i < INoPointsIn; i++)
            {
                Point p = CrScPointsIn[i];
                p.X += D_y_gc;
                p.Y += D_z_gc;
                CrScPointsIn[i] = p;
            }
        }

        public void FillCrScPropertiesByTableData()
        {
            // Do not calculate but set table data
            A_g = 3343 / 1e+6;
        }
    }
}
