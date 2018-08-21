using System.Windows.Media;

namespace CRSC
{
    public class CCrSc_3_Z : CSO
    {
        // Thin-walled centrally (point) - symmetrical Z-section with lips

        private float m_ft_f; // Flange Thickness / Hrubka pasnice
        private float m_ft_w; // Web Thickness  / Hrubka steny/stojiny
        private float m_fd;
        private float m_fc_lip;
        private float m_fb_flange;

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
        public float Fc_lip
        {
            get { return m_fc_lip; }
            set { m_fc_lip = value; }
        }
        public float Fb_flange
        {
            get { return m_fb_flange; }
            set { m_fb_flange = value; }
        }

        public CCrSc_3_Z(float fh, float fb_flange_temp, float fc_lip, float ft, Color color_temp)
        {
            CSColor = color_temp;  // Set cross-section color

            IsShapeSolid = true;
            ITotNoPoints = INoPointsOut = 12;

            h = fh;
            m_ft_f = ft;
            m_ft_w = ft;
            Fb_flange = fb_flange_temp;
            m_fc_lip = fc_lip;

            b = 2 * Fb_flange - m_ft_w;

            CSColor = color_temp;
            m_fd = fh - 2 * ft;


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
            CrScPointsOut[0, 0] = -Fb_flange + m_ft_f + m_ft_f / 2f;                          // y
            CrScPointsOut[0, 1] = (float)h / 2f - m_fc_lip;                         // z

            // Point No. 2
            CrScPointsOut[1, 0] = CrScPointsOut[0, 0];                              // y
            CrScPointsOut[1, 1] = CrScPointsOut[0, 1] + (m_fc_lip - m_ft_w);        // z

            // Point No. 3
            CrScPointsOut[2, 0] = - m_ft_f / 2f;                                    // y
            CrScPointsOut[2, 1] = CrScPointsOut[1, 1];                              // z

            // Point No. 4
            CrScPointsOut[3, 0] = CrScPointsOut[2, 0];                              // y
            CrScPointsOut[3, 1] = -(float)h / 2f;                                   // z

            // Point No. 5
            CrScPointsOut[4, 0] = (float)Fb_flange - m_ft_w / 2f;                   // y
            CrScPointsOut[4, 1] = CrScPointsOut[3, 1];                              // z

            // Point No. 6
            CrScPointsOut[5, 0] = CrScPointsOut[4, 0];                              // y
            CrScPointsOut[5, 1] = -CrScPointsOut[0, 1];                              // z

            // Point No. 7
            CrScPointsOut[6, 0] = -CrScPointsOut[0, 0];                             // y
            CrScPointsOut[6, 1] = -CrScPointsOut[0, 1];                             // z

            // Point No. 8
            CrScPointsOut[7, 0] = -CrScPointsOut[1, 0];                             // y
            CrScPointsOut[7, 1] = -CrScPointsOut[1, 1];                             // z

            // Point No. 9
            CrScPointsOut[8, 0] = -CrScPointsOut[2, 0];                             // y
            CrScPointsOut[8, 1] = -CrScPointsOut[2, 1];                             // z

            // Point No. 10
            CrScPointsOut[9, 0] = CrScPointsOut[8, 0];                              // y
            CrScPointsOut[9, 1] = (float)h / 2f;                                    // z

            // Point No. 11
            CrScPointsOut[10, 0] = -(float)Fb_flange + m_ft_w / 2f;                 // y
            CrScPointsOut[10, 1] = CrScPointsOut[9, 1];                             // z

            // Point No. 12
            CrScPointsOut[11, 0] = CrScPointsOut[10, 0];                            // y
            CrScPointsOut[11, 1] = CrScPointsOut[0, 1];                             // z
        }
    }
}


