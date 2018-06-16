using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using MATH;

namespace CRSC
{
    public class CCrSc_3_51_C_LIP2_FS50020 : CCrSc_TW
    {
        // Thin-walled mono-symmetrical C-section with lips

        private float m_ft_f; // Flange Thickness / Hrubka pasnice
        private float m_ft_w; // Web Thickness  / Hrubka steny/stojiny
        private float m_fd;
        private float m_fc_lip1;
        private float m_fc_lip2;
        private float fr_1;
        private float fr_2;
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

        public CCrSc_3_51_C_LIP2_FS50020(float fh, float fb, float fc_lip1, float fc_lip2, float ft, Color color_temp)
      {
            //ITotNoPoints = 22;
            IsShapeSolid = true;
            ITotNoPoints = 22;

            h = fh;
            b = fb;
            m_ft_f = ft;
            m_ft_w = ft;

            CSColor = color_temp;

            m_fd = fh - 2 * ft;
            m_fc_lip1 = fc_lip1;
            m_fc_lip2 = fc_lip2;

            fz_stif = 0.12f;
            fy_stif = 0.04f;

            // Create Array - allocate memory
            CrScPointsOut = new float[ITotNoPoints, 2];
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
              CrScPointsOut[0, 0] = (float)b - m_fc_lip1;          // y
              CrScPointsOut[0, 1] = - (float)h / 2f + m_fc_lip2;   // z

              // Point No. 2
              CrScPointsOut[1, 0] = (float)b;                      // y
              CrScPointsOut[1, 1] = CrScPointsOut[0, 1];           // z

              // Point No. 3
              CrScPointsOut[2, 0] = (float)b;                      // y
              CrScPointsOut[2, 1] = -(float)h / 2f;                // z

              // Point No. 4
              CrScPointsOut[3, 0] = 0;                             // y
              CrScPointsOut[3, 1] = CrScPointsOut[2, 1];           // z

              // Point No. 5
              CrScPointsOut[4, 0] = 0;                             // y
              CrScPointsOut[4, 1] = -(float)(0.5f * fz_stif);      // z

              // Point No. 6
              CrScPointsOut[5, 0] = fy_stif - m_ft_f;              // y
              CrScPointsOut[5, 1] = 0;                             // z

              for(int i = 6; i < ITotNoPoints / 2; i++)
              {
                  CrScPointsOut[i, 0] = CrScPointsOut[(ITotNoPoints / 2 - 1) - i, 0];        // y
                  CrScPointsOut[i, 1] = -CrScPointsOut[(ITotNoPoints / 2 - 1) - i, 1];       // z
              }

              // Point No. 17
              CrScPointsOut[16, 0] = fy_stif;                       // y
              CrScPointsOut[16, 1] = 0;                             // z

              // Point No. 18
              CrScPointsOut[17, 0] = m_ft_w;                        // y
              CrScPointsOut[17, 1] = -(float)(0.5f * fz_stif);      // z

              // Point No. 19
              CrScPointsOut[18, 0] = m_ft_w;                        // y
              CrScPointsOut[18, 1] = - (float)h / 2.0f + m_ft_w;    // z

              // Point No. 20
              CrScPointsOut[19, 0] = (float)b - m_ft_w;             // y
              CrScPointsOut[19, 1] = CrScPointsOut[18, 1];          // z

              // Point No. 21
              CrScPointsOut[20, 0] = (float)b - m_ft_w;             // y
              CrScPointsOut[20, 1] = -(float)h / 2.0f + m_fc_lip2 - m_ft_f; // z

              // Point No. 22
              CrScPointsOut[21, 0] = CrScPointsOut[0, 0];           // y
              CrScPointsOut[21, 1] = CrScPointsOut[20, 1];          // z

              for (int i = ITotNoPoints / 2; i < ITotNoPoints / 2 + 5; i++)
              {
                  CrScPointsOut[i, 0] = CrScPointsOut[(ITotNoPoints / 2 - 1) + (ITotNoPoints - i), 0];        // y
                  CrScPointsOut[i, 1] = -CrScPointsOut[(ITotNoPoints / 2 - 1) + (ITotNoPoints - i), 1];       // z
              }
        }
    }
}
