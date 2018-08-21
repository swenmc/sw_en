using MATH;
using System.Windows.Media;

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
