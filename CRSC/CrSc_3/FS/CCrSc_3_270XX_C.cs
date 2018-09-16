using MATH;
using System.Windows.Media;
using System.Collections.Generic;
using System.Windows;

namespace CRSC
{
    public class CCrSc_3_270XX_C : CSO
    {
        // Thin-walled mono-symmetrical C-section with lips

        private float m_ft_f; // Flange Thickness / Hrubka pasnice
        private float m_ft_w; // Web Thickness  / Hrubka steny/stojiny
        private float m_fd;

        private float fr_1_in;
        private float fr_1_out;

        private float fr_2_in;
        private float fr_2_out;

        private float fc_lip1;
        private float fc_lip2;

        private float fz_1stif1_out;
        private float fy_1stif1;

        private float fy_flange;

        private float fz_web;

        private float fy_2stif1_out;
        private float fz_2stif1;
        private float fz_2stif2;

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

        public CCrSc_3_270XX_C(float fh, float fb, float ft, Color color_temp)
        {
            Name = "C " + (fh * 1000).ToString() + (ft * 1000 * 100).ToString();
            NameDatabase = (fh * 1000).ToString() + (ft * 1000 * 100).ToString();

            CSColor = color_temp;  // Set cross-section color
            //ITotNoPoints = 56;
            IsShapeSolid = true;
            ITotNoPoints = INoPointsOut = 56;

            h = fh;
            b = fb;
            m_ft_f = ft;
            m_ft_w = ft;
            m_fd = fh - 2 * ft;

            fr_1_in = 0.004f;
            fr_1_out = fr_1_in + ft;

            fr_2_in = 0.007f;
            fr_2_out = fr_2_in + ft;

            fc_lip1 = 0.005f;
            fc_lip2 = 0.007f;

            fz_1stif1_out = 0.004f;
            fy_1stif1 = 0.02f;

            fy_flange = 0.017f;

            fz_web = 0.040f;

            fy_2stif1_out = 0.009f;
            fz_2stif1 = 0.018f;
            fz_2stif2 = 0.020f;

            // Create Array - allocate memory
            CrScPointsOut = new float[ITotNoPoints, 2];
            //CrScPointsOut = new List<Point>(ITotNoPoints);
            
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
            CrScPointsOut[0, 0] = (float)b - fr_1_out - fc_lip1;                    // y
            CrScPointsOut[0, 1] = (float)h / 2f - fr_2_out - fc_lip2 - fr_1_in;     // z

            // Point No. 2
            CrScPointsOut[1, 0] = CrScPointsOut[0, 0] + fc_lip1;           // y
            CrScPointsOut[1, 1] = CrScPointsOut[0, 1];                     // z

            // Point No. 3
            CrScPointsOut[2, 0] = CrScPointsOut[1, 0] + fr_1_in;           // y
            CrScPointsOut[2, 1] = CrScPointsOut[1, 1] + fr_1_in;           // z

            // Point No. 4
            CrScPointsOut[3, 0] = CrScPointsOut[2, 0];                     // y
            CrScPointsOut[3, 1] = CrScPointsOut[2, 1] + fc_lip2;           // z

            // Point No. 5
            CrScPointsOut[4, 0] = CrScPointsOut[3, 0] - fr_2_in;           // y
            CrScPointsOut[4, 1] = CrScPointsOut[3, 1] + fr_2_in;           // z

            // Point No. 6
            CrScPointsOut[5, 0] = CrScPointsOut[4, 0] - fy_flange;          // y
            CrScPointsOut[5, 1] = CrScPointsOut[4, 1];                      // z

            // Point No. 7
            CrScPointsOut[6, 0] = CrScPointsOut[5, 0] - 0.5f * fy_1stif1;   // y
            CrScPointsOut[6, 1] = CrScPointsOut[5, 1] - fz_1stif1_out;      // z

            // Point No. 8
            CrScPointsOut[7, 0] = CrScPointsOut[6, 0] - 0.5f * fy_1stif1;   // y
            CrScPointsOut[7, 1] = CrScPointsOut[5, 1];                      // z

            // Point No. 9
            CrScPointsOut[8, 0] = CrScPointsOut[7, 0] - ((float)b - 2 * fr_2_out - fy_flange - fy_1stif1); // y
            CrScPointsOut[8, 1] = CrScPointsOut[7, 1];                     // z

            // Point No. 10
            CrScPointsOut[9, 0] = m_ft_w;                                  // y
            CrScPointsOut[9, 1] = (float)h / 2f - fr_2_out;                // z

            // Point No. 11
            CrScPointsOut[10, 0] = CrScPointsOut[9, 0];                    // y
            CrScPointsOut[10, 1] = CrScPointsOut[9, 1] - fz_web;           // z

            // Point No. 12
            CrScPointsOut[11, 0] = CrScPointsOut[10, 0] + fy_2stif1_out;   // y
            CrScPointsOut[11, 1] = CrScPointsOut[10, 1] - fz_2stif1;       // z

            // Point No. 13
            CrScPointsOut[12, 0] = CrScPointsOut[11, 0];                   // y
            CrScPointsOut[12, 1] = CrScPointsOut[11, 1] - fz_2stif2;       // z

            // Point No. 14
            CrScPointsOut[13, 0] = CrScPointsOut[10, 0];                   // y
            CrScPointsOut[13, 1] = CrScPointsOut[12, 1] - fz_2stif1;       // z



            // Point No. 39
            CrScPointsOut[42, 0] = 0;                        // y
            CrScPointsOut[42, 1] = CrScPointsOut[13, 1];     // z

            // Point No. 40
            CrScPointsOut[43, 0] = CrScPointsOut[12, 0] - m_ft_w;           // y
            CrScPointsOut[43, 1] = CrScPointsOut[12, 1];                    // z

            // Point No. 41
            CrScPointsOut[44, 0] = CrScPointsOut[11, 0] - m_ft_w;           // y
            CrScPointsOut[44, 1] = CrScPointsOut[11, 1];                    // z

            // Point No. 42
            CrScPointsOut[45, 0] = CrScPointsOut[10, 0] - m_ft_w;           // y
            CrScPointsOut[45, 1] = CrScPointsOut[10, 1];                    // z

            // Point No. 43
            CrScPointsOut[46, 0] = CrScPointsOut[9, 0] - m_ft_w;           // y
            CrScPointsOut[46, 1] = CrScPointsOut[9, 1];                    // z

            // Point No. 44
            CrScPointsOut[47, 0] = CrScPointsOut[8, 0];                    // y
            CrScPointsOut[47, 1] = CrScPointsOut[8, 1] + m_ft_f;           // z

            // Point No. 45
            CrScPointsOut[48, 0] = CrScPointsOut[7, 0];                    // y
            CrScPointsOut[48, 1] = CrScPointsOut[7, 1] + m_ft_f;           // z

            // Point No. 46
            CrScPointsOut[49, 0] = CrScPointsOut[6, 0];                    // y
            CrScPointsOut[49, 1] = CrScPointsOut[6, 1] + m_ft_f;           // z

            // Point No. 47
            CrScPointsOut[50, 0] = CrScPointsOut[5, 0];                    // y
            CrScPointsOut[50, 1] = CrScPointsOut[5, 1] + m_ft_f;           // z

            // Point No. 48
            CrScPointsOut[51, 0] = CrScPointsOut[4, 0];                    // y
            CrScPointsOut[51, 1] = CrScPointsOut[4, 1] + m_ft_f;           // z

            // Point No. 49
            CrScPointsOut[52, 0] = CrScPointsOut[3, 0] + m_ft_f;           // y
            CrScPointsOut[52, 1] = CrScPointsOut[3, 1];                    // z

            // Point No. 50
            CrScPointsOut[53, 0] = CrScPointsOut[2, 0] + m_ft_f;           // y
            CrScPointsOut[53, 1] = CrScPointsOut[2, 1];                    // z

            // Point No. 51
            CrScPointsOut[54, 0] = CrScPointsOut[1, 0];                   // y
            CrScPointsOut[54, 1] = CrScPointsOut[1, 1] - m_ft_f;          // z

            // Point No. 52
            CrScPointsOut[55, 0] = CrScPointsOut[0, 0];                   // y
            CrScPointsOut[55, 1] = CrScPointsOut[0, 1] - m_ft_f;          // z

            // Mirror about y-y
            for (int i = 0; i < ITotNoPoints / 4; i++)
            {
                CrScPointsOut[ITotNoPoints / 2 - i - 1, 0] = CrScPointsOut[i, 0];
                CrScPointsOut[ITotNoPoints / 2 - i - 1, 1] = -CrScPointsOut[i, 1];
            }

            for (int i = 0; i < ITotNoPoints / 4; i++)
            {
                CrScPointsOut[ITotNoPoints / 2 + i, 0] = CrScPointsOut[ITotNoPoints - i - 1, 0];
                CrScPointsOut[ITotNoPoints / 2 + i, 1] = -CrScPointsOut[ITotNoPoints - i - 1, 1];
            }
        }

        //public void CalcCrSc_Coord()
        //{
        //    // Fill Point Array Data in LCS (Local Coordinate System of Cross-Section, horizontal y, vertical - z)
        //    float[,] CrScPointsOutArr = new float[ITotNoPoints, 2];
        //    // Point No. 1
        //    CrScPointsOutArr[0, 0] = (float)b - fr_1_out - fc_lip1;                    // y
        //    CrScPointsOutArr[0, 1] = (float)h / 2f - fr_2_out - fc_lip2 - fr_1_in;     // z

        //    // Point No. 2
        //    CrScPointsOutArr[1, 0] = CrScPointsOutArr[0, 0] + fc_lip1;           // y
        //    CrScPointsOutArr[1, 1] = CrScPointsOutArr[0, 1];                     // z

        //    // Point No. 3
        //    CrScPointsOutArr[2, 0] = CrScPointsOutArr[1, 0] + fr_1_in;           // y
        //    CrScPointsOutArr[2, 1] = CrScPointsOutArr[1, 1] + fr_1_in;           // z

        //    // Point No. 4
        //    CrScPointsOutArr[3, 0] = CrScPointsOutArr[2, 0];                     // y
        //    CrScPointsOutArr[3, 1] = CrScPointsOutArr[2, 1] + fc_lip2;           // z

        //    // Point No. 5
        //    CrScPointsOutArr[4, 0] = CrScPointsOutArr[3, 0] - fr_2_in;           // y
        //    CrScPointsOutArr[4, 1] = CrScPointsOutArr[3, 1] + fr_2_in;           // z

        //    // Point No. 6
        //    CrScPointsOutArr[5, 0] = CrScPointsOutArr[4, 0] - fy_flange;          // y
        //    CrScPointsOutArr[5, 1] = CrScPointsOutArr[4, 1];                      // z

        //    // Point No. 7
        //    CrScPointsOutArr[6, 0] = CrScPointsOutArr[5, 0] - 0.5f * fy_1stif1;   // y
        //    CrScPointsOutArr[6, 1] = CrScPointsOutArr[5, 1] - fz_1stif1_out;      // z

        //    // Point No. 8
        //    CrScPointsOutArr[7, 0] = CrScPointsOutArr[6, 0] - 0.5f * fy_1stif1;   // y
        //    CrScPointsOutArr[7, 1] = CrScPointsOutArr[5, 1];                      // z

        //    // Point No. 9
        //    CrScPointsOutArr[8, 0] = CrScPointsOutArr[7, 0] - ((float)b - 2 * fr_2_out - fy_flange - fy_1stif1); // y
        //    CrScPointsOutArr[8, 1] = CrScPointsOutArr[7, 1];                     // z

        //    // Point No. 10
        //    CrScPointsOutArr[9, 0] = m_ft_w;                                  // y
        //    CrScPointsOutArr[9, 1] = (float)h / 2f - fr_2_out;                // z

        //    // Point No. 11
        //    CrScPointsOutArr[10, 0] = CrScPointsOutArr[9, 0];                    // y
        //    CrScPointsOutArr[10, 1] = CrScPointsOutArr[9, 1] - fz_web;           // z

        //    // Point No. 12
        //    CrScPointsOutArr[11, 0] = CrScPointsOutArr[10, 0] + fy_2stif1_out;   // y
        //    CrScPointsOutArr[11, 1] = CrScPointsOutArr[10, 1] - fz_2stif1;       // z

        //    // Point No. 13
        //    CrScPointsOutArr[12, 0] = CrScPointsOutArr[11, 0];                   // y
        //    CrScPointsOutArr[12, 1] = CrScPointsOutArr[11, 1] - fz_2stif2;       // z

        //    // Point No. 14
        //    CrScPointsOutArr[13, 0] = CrScPointsOutArr[10, 0];                   // y
        //    CrScPointsOutArr[13, 1] = CrScPointsOutArr[12, 1] - fz_2stif1;       // z



        //    // Point No. 39
        //    CrScPointsOutArr[42, 0] = 0;                        // y
        //    CrScPointsOutArr[42, 1] = CrScPointsOutArr[13, 1];     // z

        //    // Point No. 40
        //    CrScPointsOutArr[43, 0] = CrScPointsOutArr[12, 0] - m_ft_w;           // y
        //    CrScPointsOutArr[43, 1] = CrScPointsOutArr[12, 1];                    // z

        //    // Point No. 41
        //    CrScPointsOutArr[44, 0] = CrScPointsOutArr[11, 0] - m_ft_w;           // y
        //    CrScPointsOutArr[44, 1] = CrScPointsOutArr[11, 1];                    // z

        //    // Point No. 42
        //    CrScPointsOutArr[45, 0] = CrScPointsOutArr[10, 0] - m_ft_w;           // y
        //    CrScPointsOutArr[45, 1] = CrScPointsOutArr[10, 1];                    // z

        //    // Point No. 43
        //    CrScPointsOutArr[46, 0] = CrScPointsOutArr[9, 0] - m_ft_w;           // y
        //    CrScPointsOutArr[46, 1] = CrScPointsOutArr[9, 1];                    // z

        //    // Point No. 44
        //    CrScPointsOutArr[47, 0] = CrScPointsOutArr[8, 0];                    // y
        //    CrScPointsOutArr[47, 1] = CrScPointsOutArr[8, 1] + m_ft_f;           // z

        //    // Point No. 45
        //    CrScPointsOutArr[48, 0] = CrScPointsOutArr[7, 0];                    // y
        //    CrScPointsOutArr[48, 1] = CrScPointsOutArr[7, 1] + m_ft_f;           // z

        //    // Point No. 46
        //    CrScPointsOutArr[49, 0] = CrScPointsOutArr[6, 0];                    // y
        //    CrScPointsOutArr[49, 1] = CrScPointsOutArr[6, 1] + m_ft_f;           // z

        //    // Point No. 47
        //    CrScPointsOutArr[50, 0] = CrScPointsOutArr[5, 0];                    // y
        //    CrScPointsOutArr[50, 1] = CrScPointsOutArr[5, 1] + m_ft_f;           // z

        //    // Point No. 48
        //    CrScPointsOutArr[51, 0] = CrScPointsOutArr[4, 0];                    // y
        //    CrScPointsOutArr[51, 1] = CrScPointsOutArr[4, 1] + m_ft_f;           // z

        //    // Point No. 49
        //    CrScPointsOutArr[52, 0] = CrScPointsOutArr[3, 0] + m_ft_f;           // y
        //    CrScPointsOutArr[52, 1] = CrScPointsOutArr[3, 1];                    // z

        //    // Point No. 50
        //    CrScPointsOutArr[53, 0] = CrScPointsOutArr[2, 0] + m_ft_f;           // y
        //    CrScPointsOutArr[53, 1] = CrScPointsOutArr[2, 1];                    // z

        //    // Point No. 51
        //    CrScPointsOutArr[54, 0] = CrScPointsOutArr[1, 0];                   // y
        //    CrScPointsOutArr[54, 1] = CrScPointsOutArr[1, 1] - m_ft_f;          // z

        //    // Point No. 52
        //    CrScPointsOutArr[55, 0] = CrScPointsOutArr[0, 0];                   // y
        //    CrScPointsOutArr[55, 1] = CrScPointsOutArr[0, 1] - m_ft_f;          // z

        //    // Mirror about y-y
        //    for (int i = 0; i < ITotNoPoints / 4; i++)
        //    {
        //        CrScPointsOutArr[ITotNoPoints / 2 - i - 1, 0] = CrScPointsOutArr[i, 0];
        //        CrScPointsOutArr[ITotNoPoints / 2 - i - 1, 1] = -CrScPointsOutArr[i, 1];
        //    }

        //    for (int i = 0; i < ITotNoPoints / 4; i++)
        //    {
        //        CrScPointsOutArr[ITotNoPoints / 2 + i, 0] = CrScPointsOutArr[ITotNoPoints - i - 1, 0];
        //        CrScPointsOutArr[ITotNoPoints / 2 + i, 1] = -CrScPointsOutArr[ITotNoPoints - i - 1, 1];
        //    }
        //    for (int i = 0; i < CrScPointsOutArr.GetLength(0); i++)
        //    {
        //        CrScPointsOut.Add(new Point(CrScPointsOutArr[i, 0], CrScPointsOutArr[i, 1]));
        //    }
        //}

        //public void CalcCrSc_Coord()
        //{
        //    // Fill Point Array Data in LCS (Local Coordinate System of Cross-Section, horizontal y, vertical - z)

        //    // Point No. 1
        //    CrScPointsOut.Add(new Point(b - fr_1_out - fc_lip1, h / 2f - fr_2_out - fc_lip2 - fr_1_in));

        //    // Point No. 2
        //    CrScPointsOut.Add(new Point(CrScPointsOut[0].X + fc_lip1, CrScPointsOut[0].Y));

        //    // Point No. 3
        //    CrScPointsOut.Add(new Point(CrScPointsOut[1].X + fr_1_in, CrScPointsOut[1].Y + fr_1_in));

        //    // Point No. 4
        //    CrScPointsOut.Add(new Point(CrScPointsOut[2].X, CrScPointsOut[2].Y + fc_lip2));

        //    // Point No. 5
        //    CrScPointsOut.Add(new Point(CrScPointsOut[3].X - fr_2_in, CrScPointsOut[3].Y + fr_2_in));

        //    // Point No. 6
        //    CrScPointsOut.Add(new Point(CrScPointsOut[4].X - fy_flange, CrScPointsOut[4].Y));

        //    // Point No. 7
        //    CrScPointsOut.Add(new Point(CrScPointsOut[5].X - 0.5f * fy_1stif1, CrScPointsOut[5].Y - fz_1stif1_out));

        //    // Point No. 8
        //    CrScPointsOut.Add(new Point(CrScPointsOut[6].X - 0.5f * fy_1stif1, CrScPointsOut[5].Y));

        //    // Point No. 9
        //    CrScPointsOut.Add(new Point(CrScPointsOut[7].X - (b - 2 * fr_2_out - fy_flange - fy_1stif1), CrScPointsOut[7].Y));

        //    // Point No. 10
        //    CrScPointsOut.Add(new Point(m_ft_w, h / 2f - fr_2_out));

        //    // Point No. 11
        //    CrScPointsOut.Add(new Point(CrScPointsOut[9].X, CrScPointsOut[9].Y - fz_web));

        //    // Point No. 12
        //    CrScPointsOut.Add(new Point(CrScPointsOut[10].X + fy_2stif1_out, CrScPointsOut[10].Y - fz_2stif1));

        //    // Point No. 13
        //    CrScPointsOut.Add(new Point(CrScPointsOut[11].X, CrScPointsOut[11].Y - fz_2stif2));

        //    // Point No. 14
        //    CrScPointsOut.Add(new Point(CrScPointsOut[10].X, CrScPointsOut[12].Y - fz_2stif1));



        //    // Point No. 39
        //    CrScPointsOut.Add(new Point(0, CrScPointsOut[13].Y));

        //    // Point No. 40
        //    CrScPointsOut.Add(new Point(CrScPointsOut[12].X - m_ft_w, CrScPointsOut[12].Y));

        //    // Point No. 41
        //    CrScPointsOut.Add(new Point(CrScPointsOut[11].X - m_ft_w, CrScPointsOut[11].Y));

        //    // Point No. 42
        //    CrScPointsOut.Add(new Point(CrScPointsOut[10].X - m_ft_w, CrScPointsOut[10].Y));

        //    // Point No. 43
        //    CrScPointsOut.Add(new Point(CrScPointsOut[9].X - m_ft_w, CrScPointsOut[9].Y));

        //    // Point No. 44
        //    CrScPointsOut.Add(new Point(CrScPointsOut[8].X, CrScPointsOut[8].Y + m_ft_f));

        //    // Point No. 45
        //    CrScPointsOut.Add(new Point(CrScPointsOut[7].X, CrScPointsOut[7].Y + m_ft_f));

        //    // Point No. 46
        //    CrScPointsOut.Add(new Point(CrScPointsOut[6].X, CrScPointsOut[6].Y + m_ft_f));

        //    // Point No. 47
        //    CrScPointsOut.Add(new Point(CrScPointsOut[5].X, CrScPointsOut[5].Y + m_ft_f));

        //    // Point No. 48
        //    CrScPointsOut.Add(new Point(CrScPointsOut[4].X, CrScPointsOut[4].Y + m_ft_f));

        //    // Point No. 49
        //    CrScPointsOut.Add(new Point(CrScPointsOut[3].X + m_ft_f, CrScPointsOut[3].Y));

        //    // Point No. 50
        //    CrScPointsOut.Add(new Point(CrScPointsOut[2].X + m_ft_f, CrScPointsOut[2].Y));

        //    // Point No. 51
        //    CrScPointsOut.Add(new Point(CrScPointsOut[1].X, CrScPointsOut[1].Y - m_ft_f));

        //    // Point No. 52
        //    CrScPointsOut.Add(new Point(CrScPointsOut[0].X, CrScPointsOut[0].Y - m_ft_f));

        //    // Mirror about y-y
        //    for (int i = 0; i < ITotNoPoints / 4; i++)
        //    {
        //        CrScPointsOut[ITotNoPoints / 2 - i - 1, 0] = CrScPointsOut[i, 0];
        //        CrScPointsOut[ITotNoPoints / 2 - i - 1, 1] = -CrScPointsOut[i, 1];

        //        //???
        //        CrScPointsOut.Add(new Point(CrScPointsOut[i].X, -CrScPointsOut[i].Y));
        //    }

        //    for (int i = 0; i < ITotNoPoints / 4; i++)
        //    {
        //        CrScPointsOut[ITotNoPoints / 2 + i, 0] = CrScPointsOut[ITotNoPoints - i - 1, 0];
        //        CrScPointsOut[ITotNoPoints / 2 + i, 1] = -CrScPointsOut[ITotNoPoints - i - 1, 1];

        //        //???
        //        CrScPointsOut.Add(new Point(CrScPointsOut[ITotNoPoints - i - 1].X, -CrScPointsOut[ITotNoPoints - i - 1].Y));
        //    }
        //}

        public void ChangeCoordToCentroid() // Prepocita suradnice outline podla suradnic taziska
        {
            // Temporary - odstranit po implementacii vypoctu

            D_y_gc = -0.044; // Temporary - TODO
            y_min = D_y_gc;
            y_max = b + y_min;

            z_min = -h / 2;
            z_max = h / 2;

            D_z_gc = 0;

            for (int i = 0; i < ITotNoPoints; i++)
            {
                CrScPointsOut[i, 0] += (float)D_y_gc;
                CrScPointsOut[i, 1] += (float)D_z_gc;
            }
        }
        //public void ChangeCoordToCentroid() // Prepocita suradnice outline podla suradnic taziska
        //{
        //    // Temporary - odstranit po implementacii vypoctu

        //    D_y_gc = -0.044; // Temporary - TODO
        //    y_min = D_y_gc;
        //    y_max = b + y_min;

        //    z_min = -h / 2;
        //    z_max = h / 2;

        //    D_z_gc = 0;

        //    for (int i = 0; i < ITotNoPoints; i++)
        //    {
        //        Point p = CrScPointsOut[i];
        //        p.X += D_y_gc;
        //        p.Y += D_z_gc;
        //    }
        //}

        public void FillCrScPropertiesByTableData()
        {
            // Do not calculate but set table data
            if (MathF.d_equal(Ft_w, 0.00055))
                A_g = 0.00055 / 0.00095 * 436.8 / 1e+6; // TODO Temp - zistit plochu
            if (MathF.d_equal(Ft_w, 0.00095))
                A_g = 436.8 / 1e+6;
            else if (MathF.d_equal(Ft_w, 0.00115))
                A_g = 527.0 / 1e+6; 
            else
                A_g = Ft_w / 0.00095 * 436.8 / 1e+6; // Priblizne - nespecifikovana hrubka
        }
    }
}
