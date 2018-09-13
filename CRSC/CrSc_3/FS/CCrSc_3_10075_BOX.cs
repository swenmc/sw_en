using System.Windows.Media;
using System.Collections.Generic;
using System.Windows;

namespace CRSC
{
    public class CCrSc_3_10075_BOX : CSC
    {
        // Thin-walled box section - monosymmetrical or double symmetrical

        private float m_ft_f; // Flange Thickness / Hrubka pasnice
        private float m_ft_w; // Web Thickness  / Hrubka steny/stojiny
        private float m_fd;

        private float fz_stif1_out;
        private float fy_stif1;
        private float fy_stif2;
        private float fy_stif1_out;
        private float fz_stif1;
        private float fz_stif2;

        private float fr_1_in;
        private float fr_1_out;

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

        public CCrSc_3_10075_BOX(float fh, float fb, float ft, Color color_temp)
        {
            Name = "Box " + (fh * 1000).ToString() + (ft * 1000 * 100).ToString();
            NameDatabase = (fh * 1000).ToString() + (ft * 1000 * 100).ToString();

            CSColor = color_temp;  // Set cross-section color

            //ITotNoPoints = 40;
            IsShapeSolid = false;
            INoPointsIn = INoPointsOut = 40; // vykreslujeme ako n-uholnik, pocet bodov n
            ITotNoPoints = INoPointsIn + INoPointsOut;

            h = fh;
            b = fb;
            m_ft_f = ft;
            m_ft_w = ft;

            CSColor = color_temp;

            m_fd = fh - 2 * ft;

            fz_stif1_out = 0.00285f;
            fy_stif1 = 0.0045f;
            fy_stif2 = 0.0140f;
            fr_1_in = 0.00525f;
            fr_1_out = fr_1_in + ft;

            fy_stif1_out = 0.00285f;
            fz_stif1 = 0.0045f;
            fz_stif2 = 0.0140f;

            b_in = b - 2 * fz_stif1_out - 2 * m_ft_w;
            h_in = h - 2 * fz_stif1_out - 2 * m_ft_f;

            // Create Array - allocate memory
            CrScPointsOut = new float[INoPointsOut, 2];
            CrScPointsIn = new float[INoPointsIn, 2];
            //CrScPointsOut = new List<Point>(INoPointsOut);
            //CrScPointsIn = new List<Point>(INoPointsIn);

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

        public void CalcCrSc_Coord()
        {
            // Fill Point Array Data in LCS (Local Coordinate System of Cross-Section, horizontal y, vertical - z)

            // Point No. 1
            CrScPointsOut[0, 0] = fy_stif2 / 2f;                           // y
            CrScPointsOut[0, 1] = (float)h / 2f;                           // z

            // Point No. 2
            CrScPointsOut[1, 0] = CrScPointsOut[0, 0] + fy_stif1;          // y
            CrScPointsOut[1, 1] = CrScPointsOut[0, 1] - fz_stif1_out;      // z

            // Point No. 3
            CrScPointsOut[2, 0] = CrScPointsOut[1, 0] + fy_stif2;          // y
            CrScPointsOut[2, 1] = CrScPointsOut[1, 1];                     // z

            // Point No. 4
            CrScPointsOut[3, 0] = CrScPointsOut[2, 0] + fy_stif1;          // y
            CrScPointsOut[3, 1] = CrScPointsOut[0, 1];                     // z

            // Point No. 5
            CrScPointsOut[4, 0] = 0.5f * (float)b - fr_1_out;              // y
            CrScPointsOut[4, 1] = CrScPointsOut[0, 1];                     // z

            // Point No. 6
            CrScPointsOut[5, 0] = 0.5f * (float)b;                         // y
            CrScPointsOut[5, 1] = 0.5f * (float)h - fr_1_out;              // z

            // Point No. 7
            CrScPointsOut[6, 0] = CrScPointsOut[5, 0];                                                                    // y
            CrScPointsOut[6, 1] = CrScPointsOut[5, 1] - (0.5f * (float)h - fr_1_out - 1.5f * fz_stif2 - 2f * fz_stif1);   // z

            // Point No. 8
            CrScPointsOut[7, 0] = CrScPointsOut[6, 0] - fy_stif1_out;      // y
            CrScPointsOut[7, 1] = CrScPointsOut[6, 1] - fz_stif1;          // z

            // Point No. 9
            CrScPointsOut[8, 0] = CrScPointsOut[7, 0];                     // y
            CrScPointsOut[8, 1] = CrScPointsOut[7, 1] - fz_stif2;          // z

            // Point No. 10
            CrScPointsOut[9, 0] = CrScPointsOut[6, 0];                     // y
            CrScPointsOut[9, 1] = CrScPointsOut[8, 1] - fz_stif1;          // z

            // Internal

            // Point No. 1
            CrScPointsIn[0, 0] = fy_stif2 / 2f;                           // y
            CrScPointsIn[0, 1] = (float)h / 2f - m_ft_f;                  // z

            // Point No. 2
            CrScPointsIn[1, 0] = CrScPointsIn[0, 0] + fy_stif1;           // y
            CrScPointsIn[1, 1] = CrScPointsIn[0, 1] - fz_stif1_out;       // z

            // Point No. 3
            CrScPointsIn[2, 0] = CrScPointsIn[1, 0] + fy_stif2;           // y
            CrScPointsIn[2, 1] = CrScPointsIn[1, 1];                      // z

            // Point No. 4
            CrScPointsIn[3, 0] = CrScPointsIn[2, 0] + fy_stif1;           // y
            CrScPointsIn[3, 1] = CrScPointsIn[0, 1];                      // z

            // Point No. 5
            CrScPointsIn[4, 0] = 0.5f * (float)b - fr_1_out;              // y
            CrScPointsIn[4, 1] = CrScPointsIn[0, 1];                      // z

            // Point No. 6
            CrScPointsIn[5, 0] = 0.5f * (float)b - m_ft_w;                // y
            CrScPointsIn[5, 1] = 0.5f * (float)h - fr_1_out;              // z

            // Point No. 7
            CrScPointsIn[6, 0] = CrScPointsIn[5, 0];                                                                    // y
            CrScPointsIn[6, 1] = CrScPointsIn[5, 1] - (0.5f * (float)h - fr_1_out - 1.5f * fz_stif2 - 2f * fz_stif1);   // z

            // Point No. 8
            CrScPointsIn[7, 0] = CrScPointsIn[6, 0] - fy_stif1_out;      // y
            CrScPointsIn[7, 1] = CrScPointsIn[6, 1] - fz_stif1;          // z

            // Point No. 9
            CrScPointsIn[8, 0] = CrScPointsIn[7, 0];                     // y
            CrScPointsIn[8, 1] = CrScPointsIn[7, 1] - fz_stif2;          // z

            // Point No. 10
            CrScPointsIn[9, 0] = CrScPointsIn[6, 0];                     // y
            CrScPointsIn[9, 1] = CrScPointsIn[8, 1] - fz_stif1;          // z

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

        //public void CalcCrSc_Coord()
        //{
        //    // Fill Point Array Data in LCS (Local Coordinate System of Cross-Section, horizontal y, vertical - z)

        //    // Point No. 1
        //    CrScPointsOut.Add(new Point(fy_stif2 / 2.0, h / 2.0));
            
        //    // Point No. 2
        //    CrScPointsOut.Add(new Point(CrScPointsOut[0].X + fy_stif1, CrScPointsOut[0].Y - fz_stif1_out));

        //    // Point No. 3
        //    CrScPointsOut.Add(new Point(CrScPointsOut[1].X + fy_stif2, CrScPointsOut[1].Y));

        //    // Point No. 4
        //    CrScPointsOut.Add(new Point(CrScPointsOut[2].X + fy_stif1, CrScPointsOut[0].Y));

        //    // Point No. 5
        //    CrScPointsOut.Add(new Point(0.5f * b - fr_1_out, CrScPointsOut[0].Y));

        //    // Point No. 6
        //    CrScPointsOut.Add(new Point(0.5f * b, 0.5f * h - fr_1_out));

        //    // Point No. 7
        //    CrScPointsOut.Add(new Point(CrScPointsOut[5].X, CrScPointsOut[5, 1] - (0.5f * h - fr_1_out - 1.5f * fz_stif2 - 2f * fz_stif1)));

        //    // Point No. 8
        //    CrScPointsOut.Add(new Point(CrScPointsOut[6].X - fy_stif1_out, CrScPointsOut[6].Y - fz_stif1));

        //    // Point No. 9
        //    CrScPointsOut.Add(new Point(CrScPointsOut[7].X, CrScPointsOut[7].Y - fz_stif2));

        //    // Point No. 10
        //    CrScPointsOut.Add(new Point(CrScPointsOut[6].X, CrScPointsOut[8].Y - fz_stif1));

        //    // Internal

        //    // Point No. 1
        //    CrScPointsin.Add(new Point(fy_stif2 / 2f, h / 2f - m_ft_f));

        //    // Point No. 2
        //    CrScPointsIn.Add(new Point(fy_stif2 / 2f, h / 2f - m_ft_f));

        //    // Point No. 3
        //    CrScPointsIn.Add(new Point(CrScPointsIn[1].X + fy_stif2, CrScPointsIn[1].Y));

        //    // Point No. 4
        //    CrScPointsIn.Add(new Point(CrScPointsIn[2].X + fy_stif1, CrScPointsIn[0].Y));

        //    // Point No. 5
        //    CrScPointsIn.Add(new Point(0.5f * b - fr_1_out, CrScPointsIn[0].Y));

        //    // Point No. 6
        //    CrScPointsIn.Add(new Point(0.5f * b - m_ft_w, 0.5f * h - fr_1_out));

        //    // Point No. 7
        //    CrScPointsIn.Add(new Point(CrScPointsIn[5].X, CrScPointsIn[5].Y - (0.5f * h - fr_1_out - 1.5f * fz_stif2 - 2f * fz_stif1)));

        //    // Point No. 8
        //    CrScPointsIn.Add(new Point(CrScPointsIn[6].X - fy_stif1_out, CrScPointsIn[6].Y - fz_stif1));

        //    // Point No. 9
        //    CrScPointsIn.Add(new Point(CrScPointsIn[7].X, CrScPointsIn[7].Y - fz_stif2));

        //    // Point No. 10
        //    CrScPointsIn.Add(new Point(CrScPointsIn[6].X, CrScPointsIn[8].Y - fz_stif1));

        //    // Mirror about y-y
        //    for (int i = 0; i < INoPointsOut / 4; i++)
        //    {
        //        CrScPointsOut[INoPointsOut / 2 - i - 1, 0] = CrScPointsOut[i, 0];                  // Outside
        //        CrScPointsOut[INoPointsOut / 2 - i - 1, 1] = -CrScPointsOut[i, 1];                 // Outside

        //        CrScPointsIn[INoPointsIn / 2 - i - 1, 0] = CrScPointsIn[i, 0];                     // Inside
        //        CrScPointsIn[INoPointsIn / 2 - i - 1, 1] = -CrScPointsIn[i, 1];                    // Inside

        //        //???
        //        //CrScPointsIn.Add(new Point(CrScPointsIn[1].X + fy_stif2, CrScPointsIn[1].Y));
        //    }

        //    // Mirror about z-z
        //    for (int i = 0; i < INoPointsOut / 2; i++)
        //    {
        //        CrScPointsOut[INoPointsOut - i - 1, 0] = -CrScPointsOut[i, 0];                     // Outside
        //        CrScPointsOut[INoPointsOut - i - 1, 1] = CrScPointsOut[i, 1];                      // Outside

        //        CrScPointsIn[INoPointsIn - i - 1, 0] = -CrScPointsIn[i, 0];                        // Inside
        //        CrScPointsIn[INoPointsIn - i - 1, 1] = CrScPointsIn[i, 1];                         // Inside

        //        //???
        //        //CrScPointsIn.Add(new Point(CrScPointsIn[1].X + fy_stif2, CrScPointsIn[1].Y));
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
                CrScPointsOut[i, 0] += (float)D_y_gc;
                CrScPointsOut[i, 1] += (float)D_z_gc;
            }

            for (int i = 0; i < INoPointsIn; i++)
            {
                CrScPointsIn[i, 0] += (float)D_y_gc;
                CrScPointsIn[i, 1] += (float)D_z_gc;
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
        //        Point p = CrScPointsOut[i];
        //        p.X += D_y_gc;
        //        p.Y += D_z_gc;
        //    }

        //    for (int i = 0; i < INoPointsIn; i++)
        //    {
        //        Point p = CrScPointsIn[i];
        //        p.X += D_y_gc;
        //        p.Y += D_z_gc;
        //    }
        //}

        public void FillCrScPropertiesByTableData()
        {
            // Do not calculate but set table data
            A_g = 306.8 / 1e+6;
            I_y = 4.711E+05;
            I_z = 4.711E+05;
            W_y_el = 9493;
            W_z_el = 9493;
            I_yz = 0.0;
            I_epsilon = 4.711E+05;
            I_mikro = 4.711E+05;
            W_y_el = 9493;
            W_z_el = 9493;
            I_t = 6.302E+05;
            I_w = 1.390E+06;
            D_y_gc = 0;
            D_z_gc = 0;
            D_y_sc = 0;
            D_z_sc = 0;
            D_y_s = 0;
            D_z_s = 0;
            Beta_y = 0;
            Beta_z = 0;
            Alpha_rad = 0;
        }
    }
}
