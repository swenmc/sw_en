using System.Windows.Media;
using System.Collections.Generic;
using System.Windows;

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

        public CCrSc_3_Z() { }
        public CCrSc_3_Z(int iID_temp, float fh, float fb_flange_temp, float fc_lip, float ft, Color color_temp)
        {
            ID = iID_temp;

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
            CrScPointsOut = new List<Point>(ITotNoPoints);
            // Fill Array Data
            CalcCrSc_Coord();

            // SOLID MODEL
            // Fill list of indices for drawing of surface - triangles edges
            // Particular indices - distinguished colors of member surfaces
            loadCrScIndicesFrontSide();
            loadCrScIndicesShell();
            loadCrScIndicesBackSide();

            // Complex indices - one color of member
            loadCrScIndices();

            // WIREFRAME MODEL
            // Complex indices
            loadCrScWireFrameIndices();
        }

        public void CalcCrSc_Coord()
        {
            // Fill Point Array Data in LCS (Local Coordinate System of Cross-Section, horizontal y, vertical - z)

            // Point No. 1
            CrScPointsOut.Add(new Point(-Fb_flange + m_ft_f + m_ft_f / 2.0, h / 2.0 - m_fc_lip));

            // Point No. 2
            CrScPointsOut.Add(new Point(CrScPointsOut[0].X, CrScPointsOut[0].Y + (m_fc_lip - m_ft_w)));
            // Point No. 3
            CrScPointsOut.Add(new Point(-m_ft_f / 2.0, CrScPointsOut[1].Y));

            // Point No. 4
            CrScPointsOut.Add(new Point(CrScPointsOut[2].X, -h / 2.0));

            // Point No. 5
            CrScPointsOut.Add(new Point(Fb_flange - m_ft_w / 2.0, CrScPointsOut[3].Y));

            // Point No. 6
            CrScPointsOut.Add(new Point(CrScPointsOut[4].X, -CrScPointsOut[0].Y));

            // Point No. 7
            CrScPointsOut.Add(new Point(-CrScPointsOut[0].X, -CrScPointsOut[0].Y));

            // Point No. 8
            CrScPointsOut.Add(new Point(-CrScPointsOut[1].X, -CrScPointsOut[1].Y));

            // Point No. 9
            CrScPointsOut.Add(new Point(-CrScPointsOut[2].X, -CrScPointsOut[2].Y));

            // Point No. 10
            CrScPointsOut.Add(new Point(CrScPointsOut[8].X, h / 2.0));

            // Point No. 11
            CrScPointsOut.Add(new Point(-Fb_flange + m_ft_w / 2.0, CrScPointsOut[9].Y));

            // Point No. 12
            CrScPointsOut.Add(new Point(CrScPointsOut[10].X, CrScPointsOut[0].Y));

            // Suradnice su definovane CCW, ale ma to byt CW, bolo by dobre prepracovat suradnice P1-12
            List<Point>CrScPointsOutCorrect = new List<Point>(ITotNoPoints);

            for (int i = 0; i < CrScPointsOut.Count; i++)
                CrScPointsOutCorrect.Add(CrScPointsOut[CrScPointsOut.Count - 1 - i]);

            CrScPointsOut = CrScPointsOutCorrect; // Nastavit upravene hodnoty do povodneho zoznamu (zadanie bodov je teda CW ako u ostatnych prierezov)
        }
    }
}