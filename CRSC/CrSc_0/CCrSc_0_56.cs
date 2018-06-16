using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using MATH;
using System.Windows.Media;

namespace CRSC
{
    // Test cross-section class
    // Temporary Class - includes array of drawing points of cross-section in its coordinate system (LCS-for 2D yz)
    public class CCrSc_0_56 : CCrSc
    {
        // Welded monosymmetric T section

        /*




        1  _____________  2       ____|/
          |_____   _____|    t_f     /|
        8     7 | | 4     3           |
                |*|         ____|/    |
                | |            /|     |
           t_w  | |             |  h  |
                | |         z_c |     |
                | |             |     |
             6  |_|  5      ____|/____|/
                               /|    /|
                 b
          |/____________|/
         /|            /|


         Centroid [0,0]

        z
        /|\
         |
         |
         |_____________\  y
                       /
         */


        //----------------------------------------------------------------------------
        private float m_fh;   // Height / Vyska
        private float m_fb;   // Width  / Sirka
        private float m_ft_f; // Flange Thickness / Hrubka pasnice
        private float m_ft_w; // Web Thickness  / Hrubka steny/stojiny
        private float m_fz_c; // Centroid coordinate / Suradnica tažiska / Absolute value
        //private short ITotNoPoints; // Total Number of Cross-section Points for Drawing
        //public  float[,] CrScPointsOut; // Array of Points and values in 2D
        //----------------------------------------------------------------------------

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
        public float Fy_c
        {
          get { return m_fz_c; }
          set { m_fz_c = value; }
        }
        /*public short ITotNoPoints
        {
            get { return ITotNoPoints; }
            set { ITotNoPoints = value; }
        }*/
        /*
        public float[,] CrScPoint
        {
            get { return CrScPointsOut; }
            set { CrScPointsOut = value; }
        }
        */

        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        public CCrSc_0_56()  {   }
        public CCrSc_0_56(float fh, float fb, float ft_f, float ft_w, float fz_c)
        {
            IsShapeSolid = true;
            ITotNoPoints = 8;
            m_fh = fh;
            m_fb = fb;
            m_ft_f = ft_f;
            m_ft_w = ft_w;
            m_fz_c = Math.Abs(fz_c); // Absolute value

            // Create Array - allocate memory
            CrScPointsOut = new float [ITotNoPoints,2];
            // Fill Array Data
            CalcCrSc_Coord_T_MS();

            // Fill list of indices for drawing of surface - triangles edges
            loadCrScIndices();
        }

        //----------------------------------------------------------------------------
        void CalcCrSc_Coord_T_MS()
        {
            // Fill Point Array Data in LCS (Local Coordinate System of Cross-Section, horizontal y, vertical - z)

            // Point No. 1
            CrScPointsOut[0, 0] = -m_fb / 2f;       // y
            CrScPointsOut[0, 1] = m_fh - m_fz_c;    // z

            // Point No. 2
            CrScPointsOut[1, 0] = -CrScPointsOut[0, 0];  // y
            CrScPointsOut[1, 1] = CrScPointsOut[0, 1];   // z

            // Point No. 3
            CrScPointsOut[2, 0] = CrScPointsOut[1, 0];             // y
            CrScPointsOut[2, 1] = CrScPointsOut[0, 1] - m_ft_f;    // z

            // Point No. 4
            CrScPointsOut[3, 0] = CrScPointsOut[2, 0] - ((m_fb - m_ft_w) / 2f);    // y
            CrScPointsOut[3, 1] = CrScPointsOut[2, 1];                             // z

            // Point No. 5
            CrScPointsOut[4, 0] = CrScPointsOut[3, 0];    // y
            CrScPointsOut[4, 1] = -m_fz_c;              // z

            // Point No. 6
            CrScPointsOut[5, 0] = -CrScPointsOut[4, 0];    // y
            CrScPointsOut[5, 1] = CrScPointsOut[4, 1];     // z

            // Point No. 7
            CrScPointsOut[6, 0] = -CrScPointsOut[3, 0];    // y
            CrScPointsOut[6, 1] = CrScPointsOut[3, 1];     // z

            // Point No. 8
            CrScPointsOut[7, 0] = -CrScPointsOut[2, 0];    // y
            CrScPointsOut[7, 1] = CrScPointsOut[2, 1];     // z
        }

        // Welded aymmetric T section

        /*




        1  _________________  2       ____|/
          |_____   _________|    t_f     /|
        8     7 | | 4     3               |
                | |    *    ____|/        |
                | |            /|         |
           t_w  | |             |      h  |
                | |         z_c |         |
                | |             |         |
             6  |_|  5      ____|/________|/
                               /|        /|
          |     |      |
          |     |      |
          | b_l |  y_c |
          |/____|/_____|/
         /|    /|     /|

                   b
          |/________________|/
         /|                /|


         Centroid [0,0]

        z
        /|\
         |
         |
         |_____________\  y
                       /
         */

        private float m_fb_l;   // Width of Free Left Part Flange  / Sirka volnej lavej strany pasnice
        private float m_fy_c; // Centroid coordinate / Suradnica tažiska

        public CCrSc_0_56(float fh, float fb, float fb_l, float ft_f, float ft_w, float fy_c, float fz_c)
        {
            IsShapeSolid = true;
            ITotNoPoints = 8;
            m_fh = fh;
            m_fb = fb;
            m_fb_l = fb_l;
            m_ft_f = ft_f;
            m_ft_w = ft_w;
            m_fy_c = fy_c;
            m_fz_c = Math.Abs(fz_c); // Absolute value

            // Create Array - allocate memory
            CrScPointsOut = new float[ITotNoPoints, 2];
            // Fill Array Data
            CalcCrSc_Coord_T_AS();

            // Fill list of indices for drawing of surface - triangles edges
            loadCrScIndices();
        }

        //----------------------------------------------------------------------------
        void CalcCrSc_Coord_T_AS()
        {
            // Fill Point Array Data in LCS (Local Coordinate System of Cross-Section, horizontal y, vertical - z)

            // Point No. 1
            CrScPointsOut[0, 0] = -m_fb_l - m_fy_c;       // y
            CrScPointsOut[0, 1] = m_fh - m_fz_c;         // z

            // Point No. 2
            CrScPointsOut[1, 0] = -CrScPointsOut[0, 0];  // y
            CrScPointsOut[1, 1] = CrScPointsOut[0, 1];   // z

            // Point No. 3
            CrScPointsOut[2, 0] = CrScPointsOut[1, 0];             // y
            CrScPointsOut[2, 1] = CrScPointsOut[0, 1] - m_ft_f;    // z

            // Point No. 4
            CrScPointsOut[3, 0] = CrScPointsOut[2, 0] - (m_fb - m_fb_l - m_ft_w);    // y
            CrScPointsOut[3, 1] = CrScPointsOut[2, 1];                               // z

            // Point No. 5
            CrScPointsOut[4, 0] = CrScPointsOut[3, 0];    // y
            CrScPointsOut[4, 1] = -m_fz_c;              // z

            // Point No. 6
            CrScPointsOut[5, 0] = CrScPointsOut[4, 0] - m_ft_w;    // y
            CrScPointsOut[5, 1] = CrScPointsOut[4, 1];             // z

            // Point No. 7
            CrScPointsOut[6, 0] = CrScPointsOut[5, 0];     // y
            CrScPointsOut[6, 1] = CrScPointsOut[3, 1];     // z

            // Point No. 8
            CrScPointsOut[7, 0] = CrScPointsOut[0, 0];     // y
            CrScPointsOut[7, 1] = CrScPointsOut[2, 1];     // z
        }


        //----------------------------------------------------------------------------
        // Cross-section properties
        //----------------------------------------------------------------------------

        // Rolled steel
        // Tapered flanges
        // Monosymmetrical

        float m_fU, m_fA,
                m_fS_y, m_fI_y, m_fW_y_el, m_fW_y_pl, m_ff_y_plel,
                m_fS_z, m_fI_z, m_fW_z_el, m_fW_z_pl, m_ff_z_plel,
                m_fW_t_el, m_fI_t, m_fi_t, m_fW_t_pl, m_ff_t_plel,
                m_fEta_y_v, m_fA_y_v_el, m_fA_y_v_pl, m_ff_y_v_plel,
                m_fEta_z_v, m_fA_z_v_el, m_fA_z_v_pl, m_ff_z_v_plel;

        float m_fh_i, // Web Depth (distance between flange faces)
              m_fd,   // Web Depth (straight part)
              m_fr,   // Radius between faces of web and flange
              m_fI_w; // Section warping constant

        float m_fe_y;

        // Web Depth
        void Calc_h_i()
        {
            m_fh_i = m_fh - m_ft_f;
        }

        // Web Depth (straight part)
        void Calc_d()
        {
            m_fd = m_fh - m_ft_f - m_fr; // temp
        }

        // Perimeter of section
        void Calc_U()
        {
            m_fU = 2 * m_fb + 2 * m_fh - m_ft_w + (MathF.fPI - 4) * m_fr;
        }
        // Section area
        void Calc_A()
        {
            // m_fA;
        }


        // First moment o area
        void Calc_S_y()
        {
            // m_fS_y;
        }
        // Second moment of area
        void Calc_I_y()
        {
            // m_fI_y;
        }
        // Section modulus - elastic
        void Calc_W_y_el()
        {
            m_fW_y_el = m_fI_y / (m_fh - m_fe_y);
        }
        // Section modulus - plastic
        void Calc_W_y_pl()
        {
           // m_fW_y_pl;
        }
        // Shape factor - plastic/elastic
        void Calc_f_y_plel()
        {
            m_ff_y_plel = m_fW_y_pl / m_fW_y_el;
        }


        // First moment o area
        void Calc_S_z()
        {
           // m_fS_z;
        }
        // Second moment of area
        void Calc_I_z()
        {
            // m_fI_z;
        }
        // Section modulus - elastic
        void Calc_W_z_el()
        {
            m_fW_z_el = 2.0f * m_fI_z / m_fb;
        }
        // Section modulus - plastic
        void Calc_W_z_pl()
        {
           // m_fW_z_pl;
        }
        // Shape factor - plastic/elastic
        void Calc_f_z_plel()
        {
            m_ff_z_plel = m_fW_z_pl / m_fW_z_el;
        }


        // Torsional inertia constant
        void Calc_I_t()
        {
            m_fI_t = (m_fb * MathF.Pow3(m_ft_f) + (m_fh - m_ft_f / 2.0f) * MathF.Pow3(m_ft_w)) / 3.0f;
        }
        // Torsional radius of gyration
        void Calc_i_t()
        {
            m_fi_t = MathF.Sqrt(m_fI_t / m_fA);
        }
        // Torsional section modulus - elastic
        void Calc_W_t_el()
        {
            m_fW_t_el = m_fI_t / Math.Max(m_ft_w, m_ft_f); // Min or Max Thickness
        }
        // Torsional section modulus - plastic
        void Calc_W_t_pl()
        {
            m_fW_t_pl = (m_fb - m_ft_w - 2 * m_fr - m_ft_f / 3.0f) * MathF.Pow2(m_ft_f) / 2.0f + (m_fh - m_ft_f - m_fr - m_ft_w / 6.0f) * MathF.Pow2(m_ft_w) / 2.0f /* + */;  // Temp
        }
        // Torsional shape factor plastic/elastic
        void Calc_f_t_plel()
        {
            m_ff_t_plel = m_fW_t_pl / m_fW_t_el;
        }
        // Section warping constant
        void Calc_I_w()
        {
            m_fI_w = MathF.Pow3(m_fb) * MathF.Pow3(m_ft_f) / 144.0f + MathF.Pow3(m_fh - m_ft_f / 2.0f) * MathF.Pow3(m_ft_w) / 36.0f;
        }


        // Shear factor
        void Calc_Eta_y_v()
        {
            m_fEta_y_v = (m_fA / MathF.Pow2(m_fI_y)) /*  Syi^2 / byi *dz */ ;  // Temp
        }
        // Shear effective area - elastic
        void Calc_A_y_v_el()
        {
            m_fA_y_v_el = 2 * m_ft_w / m_fW_y_pl;
        }
        // Shape factor for shear - plastic/elastic
        void Calc_f_y_v_plel()
        {
            m_ff_y_v_plel = 1.00f; // Temp
        }
        // Shear effective area - plastic
        void Calc_A_y_v_pl()
        {
            m_fA_y_v_pl = m_ff_y_v_plel * m_fA_y_v_el; // Temp
        }


        // Shear factor
        void Calc_Eta_z_v()
        {
            m_fEta_z_v = (m_fA / MathF.Pow2(m_fI_z)) /*  Szi^2 / bzi *dy */ ;  // Temp
        }
        // Shear effective area - elastic
        void Calc_A_z_v_el()
        {
            m_fA_z_v_el = m_fI_z /* * Math.Min (bzi / Szi)*/;
        }
        // Shape factor for shear - plastic/elastic
        void Calc_f_z_v_plel()
        {
            m_ff_z_v_plel = 1.00f; // Temp
        }
        // Shear effective area - plastic
        void Calc_A_z_v_pl()
        {
            m_fA_z_v_pl = m_ff_z_v_plel * m_fA_z_v_el; // Temp
        }

        // Rolled steel
        // Parallel-faced flanges (Cut IPE, HE sections, ...)
        // Monosymmetrical


        // Torsional inertia constant
        //void Calc_I_t()
        //{
        //    m_fI_t = (m_fb - 0.63f * m_ft_f) * MathF.Pow3(m_ft_f) / 3.0f + (m_fh - m_ft_f - 0.315f * m_ft_w) * MathF.Pow3(m_ft_w) / 3.0f + (m_ft_w / m_ft_f) * (0.15f + 0.1f * m_fr / m_ft_f) * MathF.Pow4((MathF.Pow2(m_fr + m_ft_w / 2.0f) + MathF.Pow2(m_fr + m_ft_f) - MathF.Pow2(m_fr) / (2 * m_fr + m_ft_f)));
        //}




		protected override void loadCrScIndices()
        {
            // const int secNum = 8;  // Number of points in section (2D)
            TriangleIndices = new Int32Collection();

            // Front Side / Forehead
            AddRectangleIndices_CW_1234(TriangleIndices, 0, 1, 2, 7);
            AddRectangleIndices_CW_1234(TriangleIndices, 6, 3, 4, 5);

            // Back Side 
            AddRectangleIndices_CW_1234(TriangleIndices, 9, 8, 15, 10);
            AddRectangleIndices_CW_1234(TriangleIndices, 11, 14, 13, 12);

            // Shell Surface
            DrawCaraLaterals(8, TriangleIndices);
        }

        protected override void loadCrScIndicesFrontSide()
        {
        }

        protected override void loadCrScIndicesShell()
        {
        }

        protected override void loadCrScIndicesBackSide()
        {
        }
	}
}
