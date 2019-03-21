using System.Windows.Media;
using System.Collections.Generic;
using System.Windows;

namespace CRSC
{
    public class CCrSc_3_50020_C : CSO
    {
        // Thin-walled mono-symmetrical C-section with lips

        private float m_ft_f; // Flange Thickness / Hrubka pasnice
        private float m_ft_w; // Web Thickness  / Hrubka steny/stojiny
        private float m_fd;
        private float m_fc_lip1;
        private float m_fc_lip2;
        private float fr_1_out;
        private float fr_1_in;
        private float fz_stif;
        private float fy_stif;

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
        public float FC_lip1
        {
            get { return m_fc_lip1; }
            set { m_fc_lip1 = value; }
        }
        public float FC_lip2
        {
            get { return m_fc_lip2; }
            set { m_fc_lip2 = value; }
        }

        public CCrSc_3_50020_C(int iID_temp, float fh, float fb, float ft, Color color_temp)
        {
            ID = iID_temp;

            Name = "C " + (fh * 1000).ToString() + (ft * 1000 * 10).ToString(); // Original Description
            Name = "C " + (fh * 1000).ToString() + (20).ToString(); // FS system Description
            NameDatabase = (fh * 1000).ToString() + (20).ToString();

            CSColor = color_temp;  // Set cross-section color

            //ITotNoPoints = 34;
            IsShapeSolid = true;
            ITotNoPoints = INoPointsOut = 34;

            h = fh;
            b = fb;
            m_ft_f = ft;
            m_ft_w = ft;

            CSColor = color_temp;

            m_fd = fh - 2 * ft;
            m_fc_lip1 = 0.012f; // Horizontal
            m_fc_lip2 = 0.034f; // Vertical

            fr_1_in = 0.006f;
            fr_1_out = fr_1_in + m_ft_f;

            fz_stif = 0.13f;
            fy_stif = 0.04f;

            // Create Array - allocate memory
            //CrScPointsOut = new float[ITotNoPoints, 2];
            CrScPointsOut = new List<Point>(ITotNoPoints);

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
            loadCrScWireFrameIndices();

            FillCrScPropertiesByTableData();
        }

        public void CalcCrSc_Coord()
        {
            // Fill Point Array Data in LCS (Local Coordinate System of Cross-Section, horizontal y, vertical - z)
            float[,] CrScPointsOutArr = new float[ITotNoPoints, 2];
            // Point No. 1
            CrScPointsOutArr[0, 0] = (float)b - m_fc_lip1 - fr_1_out;                  // y
            CrScPointsOutArr[0, 1] = (float)h / 2f - m_fc_lip2 - fr_1_out - fr_1_in;   // z

            // Point No. 2
            CrScPointsOutArr[1, 0] = CrScPointsOutArr[0, 0] + m_fc_lip1;                  // y
            CrScPointsOutArr[1, 1] = CrScPointsOutArr[0, 1];                              // z

            // Point No. 3
            CrScPointsOutArr[2, 0] = (float)b - m_ft_f;                                // y
            CrScPointsOutArr[2, 1] = (float)h / 2f - fr_1_out - m_fc_lip2;             // z

            // Point No. 4
            CrScPointsOutArr[3, 0] = CrScPointsOutArr[2, 0];                              // y
            CrScPointsOutArr[3, 1] = CrScPointsOutArr[2, 1] + m_fc_lip2;                  // z

            // Point No. 5
            CrScPointsOutArr[4, 0] = (float)b - fr_1_out;                              // y
            CrScPointsOutArr[4, 1] = (float)h / 2f - m_ft_f;                           // z

            // Point No. 6
            CrScPointsOutArr[5, 0] = fr_1_out;                                        // y
            CrScPointsOutArr[5, 1] = CrScPointsOutArr[4, 1];                             // z

            // Point No. 7
            CrScPointsOutArr[6, 0] = m_ft_w;                                          // y
            CrScPointsOutArr[6, 1] = CrScPointsOutArr[5, 1] - fr_1_out;                  // z

            // Point No. 8
            CrScPointsOutArr[7, 0] = CrScPointsOutArr[6, 0];                             // y
            CrScPointsOutArr[7, 1] = fz_stif / 2f;                                    // z

            // Point No. 9
            CrScPointsOutArr[8, 0] = CrScPointsOutArr[7, 0] + fy_stif;                   // y
            CrScPointsOutArr[8, 1] = 0;                                               // z


            // Point No. 34
            CrScPointsOutArr[33, 0] = (float)b - m_fc_lip1 - fr_1_out;                // y
            CrScPointsOutArr[33, 1] = (float)h / 2f - m_fc_lip2 - 2 * fr_1_out;       // z

            // Point No. 33
            CrScPointsOutArr[32, 0] = CrScPointsOutArr[33, 0] + m_fc_lip1;               // y
            CrScPointsOutArr[32, 1] = CrScPointsOutArr[33, 1];                           // z

            // Point No. 32
            CrScPointsOutArr[31, 0] = (float)b;                                       // y
            CrScPointsOutArr[31, 1] = CrScPointsOutArr[2, 1];                            // z

            // Point No. 31
            CrScPointsOutArr[30, 0] = CrScPointsOutArr[31, 0];                           // y
            CrScPointsOutArr[30, 1] = CrScPointsOutArr[3, 1];                            // z

            // Point No. 30
            CrScPointsOutArr[29, 0] = CrScPointsOutArr[4, 0];                            // y
            CrScPointsOutArr[29, 1] = (float)h / 2f;                                  // z

            // Point No. 29
            CrScPointsOutArr[28, 0] = CrScPointsOutArr[5, 0];                            // y
            CrScPointsOutArr[28, 1] = CrScPointsOutArr[29, 1];                           // z

            // Point No. 28
            CrScPointsOutArr[27, 0] = 0;                                              // y
            CrScPointsOutArr[27, 1] = CrScPointsOutArr[6, 1];                            // z

            // Point No. 27
            CrScPointsOutArr[26, 0] = CrScPointsOutArr[27, 0];                           // y
            CrScPointsOutArr[26, 1] = fz_stif / 2f;                                   // z

            // Point No. 26
            CrScPointsOutArr[25, 0] = fy_stif;                                        // y
            CrScPointsOutArr[25, 1] = 0f;                                             // z

            // Mirror about y-y
            for (int i = 0; i < 8; i++)
            {
                CrScPointsOutArr[ITotNoPoints / 2 - i - 1, 0] = CrScPointsOutArr[i, 0];
                CrScPointsOutArr[ITotNoPoints / 2 - i - 1, 1] = -CrScPointsOutArr[i, 1];
            }

            for (int i = 0; i < 8; i++)
            {
                CrScPointsOutArr[ITotNoPoints / 2 + i, 0] = CrScPointsOutArr[ITotNoPoints - i - 1, 0];
                CrScPointsOutArr[ITotNoPoints / 2 + i, 1] = -CrScPointsOutArr[ITotNoPoints - i - 1, 1];
            }

            for (int i = 0; i < CrScPointsOutArr.GetLength(0); i++)
            {
                CrScPointsOut.Add(new Point(CrScPointsOutArr[i, 0], CrScPointsOutArr[i, 1]));
            }
        }
 
        public void ChangeCoordToCentroid() // Prepocita suradnice outline podla suradnic taziska
        {
            y_min = -D_y_gc;
            y_max = b + y_min;

            z_min = -h / 2;
            z_max = h / 2;

            for (int i = 0; i < ITotNoPoints; i++)
            {
                Point p = CrScPointsOut[i];
                p.X += y_min;
                p.Y += 0;
                CrScPointsOut[i] = p;
            }
        }
    }
}
