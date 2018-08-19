using MATH;
using System;
using System.Windows.Media;

namespace BaseClasses.CRSC
{
    // Test cross-section class
    // Temporary Class - includes array of drawing points of cross-section in its coordinate system (LCS-for 2D yz)
    public class CCrSc_0_52 : CCrSc
    {
        // Welded monosymmetric U/C section

        /*




        1  _______  2       ____|/
          | ______|    t_f     /|
          | | 4       3         |
          | |                   |
          | |                   |
   t_w    | |                h  |
          | | *                 |
          | |                   |
          | | 5       6         |
          | |_____              |
          |_______|         ____|/
        8            7         /|
              b
          |/______|/
         /|      /|


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
        private float m_fy_c; // Centroid coordinate / Suradnica tažiska / Absolute value
        //private short m_iTotNoPoints; // Total Number of Cross-section Points for Drawing
        //public  float[,] m_CrScPoint; // Array of Points and values in 2D
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
          get { return m_fy_c; }
          set { m_fy_c = value; }
        }
        /*public short ITotNoPoints
        {
            get { return m_iTotNoPoints; }
            set { m_iTotNoPoints = value; }
        }*/
        /*
        public float[,] CrScPoint
        {
            get { return m_CrScPoint; }
            set { m_CrScPoint = value; }
        }
        */

        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        public CCrSc_0_52()  {   }
        public CCrSc_0_52(float fh, float fb, float ft_f, float ft_w, float fy_c)
        {
            IsShapeSolid = true;
            ITotNoPoints = 8;
            m_fh = fh;
            m_fb = fb;
            m_ft_f = ft_f;
            m_ft_w = ft_w;
            m_fy_c = Math.Abs(fy_c); // Absolute value

            // Create Array - allocate memory
            CrScPointsOut = new float [ITotNoPoints,2];
            // Fill Array Data
            CalcCrSc_Coord_U_MS();

            // Fill list of indices for drawing of surface - triangles edges
            loadCrScIndices();
        }

        //----------------------------------------------------------------------------
        void CalcCrSc_Coord_U_MS()
        {
            // Fill Point Array Data in LCS (Local Coordinate System of Cross-Section, horizontal y, vertical - z)

            // Point No. 1
            CrScPointsOut[0,0] = -m_fy_c;       // y
            CrScPointsOut[0,1] = m_fh / 2f;     // z

            // Point No. 2
            CrScPointsOut[1, 0] = -m_fy_c + m_fb;      // y
            CrScPointsOut[1,1] = CrScPointsOut[0,1];     // z

            // Point No. 3
            CrScPointsOut[2,0] = CrScPointsOut[1,0];             // y
            CrScPointsOut[2,1] = CrScPointsOut[0,1] - m_ft_f;    // z

            // Point No. 4
            CrScPointsOut[3,0] = CrScPointsOut[2,0] - m_fb + m_ft_w;    // y
            CrScPointsOut[3,1] = CrScPointsOut[2,1];                    // z

            // Point No. 5
            CrScPointsOut[4,0] = CrScPointsOut[3,0];      // y
            CrScPointsOut[4,1] = -CrScPointsOut[3,1];     // z

            // Point No. 6
            CrScPointsOut[5,0] = CrScPointsOut[2,0];      // y
            CrScPointsOut[5,1] = -CrScPointsOut[2,1];     // z

            // Point No. 7
            CrScPointsOut[6,0] = CrScPointsOut[1,0];      // y
            CrScPointsOut[6,1] = -CrScPointsOut[1,1];     // z

            // Point No. 8
            CrScPointsOut[7,0] = CrScPointsOut[0,0];      // y
            CrScPointsOut[7,1] = -CrScPointsOut[0,1];     // z
        }

        // Welded asymmetric U/C section

        /*

             b_fu
          |/______|/
         /|      /|

        1  _______  2              ____|/
          | ______|    t_fu           /|
          | | 4     3                  |
          | |                          |
          | |                          |
   t_w    | |                       h  |
          | |                          |
          | |   *                      |
          | | 5       6                |
          | |________                  |
          |__________|   t_fb      ____|/
        8            7                /|
              b_fb
          |/_________|/
         /|         /|


         y_c - from left
         z_c - from bottom

         Centroid [0,0]

        z
        /|\
         |
         |
         |_____________\  y
                       /
         */

        private float m_fb_fu;   // Width of Upper Flange / Sirka hornej pasnice
        private float m_ft_fu;   // Upper Flange Thickness / Hrubka hornej pasnice
        private float m_fb_fb;   // Width of Bottom Flange / Sirka spodnej pasnice
        private float m_ft_fb;   // Bottom Flange Thickness / Hrubka spodnej pasnice
        private float m_fz_c;    // Centroid coordinate / Suradnica tažiska / Absolute value

        public CCrSc_0_52(float fh, float fb_fu, float fb_fb, float ft_fu, float ft_fb, float ft_w, float fy_c, float fz_c)
        {
            IsShapeSolid = true;
            ITotNoPoints = 8;
            m_fh    = fh;
            m_fb_fu = fb_fu;
            m_fb_fb = fb_fb;
            m_ft_fu = ft_fu;
            m_ft_fb = ft_fb;
            m_ft_w = ft_w;
            m_fy_c = Math.Abs(fy_c); // Absolute value
            m_fz_c = Math.Abs(fz_c); // Absolute value

            // Create Array - allocate memory
            CrScPointsOut = new float[ITotNoPoints, 2];
            // Fill Array Data
            CalcCrSc_Coord_U_AS();

            // Fill list of indices for drawing of surface - triangles edges
            loadCrScIndices();
        }
        //----------------------------------------------------------------------------
        void CalcCrSc_Coord_U_AS()
        {
            // Fill Point Array Data in LCS (Local Coordinate System of Cross-Section, horizontal y, vertical - z)

            // Point No. 1
            CrScPointsOut[0, 0] = -m_fy_c;           // y
            CrScPointsOut[0, 1] = m_fh - m_fz_c;     // z

            // Point No. 2
            CrScPointsOut[1, 0] = -m_fy_c + m_fb_fu;     // y
            CrScPointsOut[1, 1] = CrScPointsOut[0, 1];     // z

            // Point No. 3
            CrScPointsOut[2, 0] = CrScPointsOut[1, 0];              // y
            CrScPointsOut[2, 1] = CrScPointsOut[0, 1] - m_ft_fu;    // z

            // Point No. 4
            CrScPointsOut[3, 0] = CrScPointsOut[2, 0] - m_fb_fu + m_ft_w;    // y
            CrScPointsOut[3, 1] = CrScPointsOut[2, 1];                      // z

            // Point No. 5
            CrScPointsOut[4, 0] = CrScPointsOut[3, 0];        // y
            CrScPointsOut[4, 1] = -m_fz_c + m_ft_fb;        // z

            // Point No. 6
            CrScPointsOut[5, 0] = -m_fy_c + m_fb_fb;        // y
            CrScPointsOut[5, 1] = CrScPointsOut[4, 1];        // z

            // Point No. 7
            CrScPointsOut[6, 0] = CrScPointsOut[5, 0];        // y
            CrScPointsOut[6, 1] = -m_fz_c;                  // z

            // Point No. 8
            CrScPointsOut[7, 0] = CrScPointsOut[0, 0];        // y
            CrScPointsOut[7, 1] = CrScPointsOut[6, 1];        // z
        }


        //----------------------------------------------------------------------------
        // Cross-section properties
        //----------------------------------------------------------------------------

        // Rolled steel
        // Parallel-faced flanges (flat)
        // Mono symmetrical

        float m_fU, m_fA,
                m_fS_y, m_fI_y, m_fW_y_el, m_fW_y_pl, m_ff_y_plel,
                m_fS_z, m_fI_z, m_fW_z_el, m_fW_z_pl, m_ff_z_plel,
                m_fW_t_el, m_fI_t, m_fi_t, m_fW_t_pl, m_ff_t_plel,
                m_fEta_y_v, m_fA_y_v_el, m_fA_y_v_pl, m_ff_y_v_plel,
                m_fEta_z_v, m_fA_z_v_el, m_fA_z_v_pl, m_ff_z_v_plel;

        float m_fh_i, // Web Depth (distance between flange faces)
              m_fd,   // Web Depth (straight part)
              m_fr,   // Radius between faces of web and flange
              m_fI_w, // Section warping constant
              m_fy_M, // Shear Center
              m_fy_C; // Centroid

        // Web Depth (distance between flange faces)
        void Calc_h_i()
        {
            m_fh_i = m_fh - 2 * m_ft_f;
        }

        // Web Depth (straight part)
        void Calc_d()
        {
            m_fd = m_fh - 2 * m_ft_f - 2 * m_fr;
        }

        // 
        void Calc_y_M()
        {
            m_fy_M = (3 * MathF.Pow2(m_fb - m_ft_w / 2.0f) / (m_fh - m_ft_f + 6.0f *(m_fb - m_ft_w/2.0f))-m_ft_w / 2.0f);
        }

        // 
        void Calc_y_C()
        {
            // m_fy_C = bzi * y * dy / m_fA;
        }


        // Perimeter of section
        void Calc_U()
        {
            m_fU = 2 * m_fh + 4 * m_fb - 2 * m_ft_w + (MathF.fPI - 4) * m_fr;
        }
        // Section area
        void Calc_A()
        {
            // m_fA =  byi * dz;
        }


        // First moment o area
        void Calc_S_y()
        {
            // m_fS_y;
        }
        // Second moment of area
        void Calc_I_y()
        {
            // m_fI_y = byi * z^2 * dz;
        }
        // Section modulus - elastic
        void Calc_W_y_el()
        {
            m_fW_y_el = 2 * m_fI_y / m_fh;
        }
        // Section modulus - plastic
        void Calc_W_y_pl()
        {
           // m_fW_y_pl = byi * z * dz;
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
            m_fW_z_el = 2 * m_fI_z / (m_fb - m_fy_C);
        }
        // Section modulus - plastic
        void Calc_W_z_pl()
        {
            // m_fW_z_pl 
        }
        // Shape factor - plastic/elastic
        void Calc_f_z_plel()
        {
            m_ff_z_plel = m_fW_z_pl / m_fW_z_el;
        }


        // Torsional inertia constant
        void Calc_I_t()
        {
            m_fI_t =  ((m_fh - m_ft_f) * MathF.Pow3(m_ft_w) + 2.0f * (m_fb - m_ft_w / 2.0f) * MathF.Pow3(m_ft_f)) / 3.0f;
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
            m_fW_t_pl = (m_fb - m_ft_w - m_fr - m_ft_f / 3.0f) * MathF.Pow2(m_ft_f) + m_fd * MathF.Pow2(m_ft_w) / 2.0f /*+   doplnit   */;  // Temp
        }
        // Torsional shape factor plastic/elastic
        void Calc_f_t_plel()
        {
            m_ff_t_plel = m_fW_t_pl / m_fW_t_el;
        }
        // Section warping constant
        void Calc_I_w()
        {
            m_fI_w = MathF.Pow2(m_fh - m_ft_f) * MathF.Pow3(m_fb- m_ft_w/2.0f) * m_ft_f * (1/6.0f - 1/(8.0f + 4.0f * (m_fh - m_ft_f) / m_ft_w / (m_fb-m_ft_w / 2.0f) / m_ft_f));
        }


        // Shear factor
        void Calc_Eta_y_v()
        {
            m_fEta_y_v = (m_fA / MathF.Pow2(m_fI_y)) /*  Syi^2 / byi *dz */ ;  // Temp
        }
        // Shear effective area - elastic
        void Calc_A_y_v_el()
        {
            m_fA_y_v_el = m_fI_y /** MathF.Min (byi / Syi)*/;
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
            m_fA_z_v_el = m_fI_z /** Math.Min (bzi / Szi)*/;
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


        // Tapered flanges
        // Monosymmetrical

        float m_fAlpha_Taper, // Pocitat s % (8) alebo s podielom (0,08)
             m_fr_1, m_fr_2, m_fu;

        // t - stredna hrubka

        // Basic dimmensions
        void Calc_BasicDimension()
        {
            m_fu = (m_fb - m_ft_w) / 2.0f;

            if (m_fr_1 < 0.0f)
                m_fr_1 = m_ft_f;

            if (m_fr_2 < 0.0f)
                m_fr_2 = m_ft_f - m_fu * m_fAlpha_Taper;
        }


        //// Web Depth (straight part)
        //void Calc_d()
        //{
        //    m_fd = m_fh - 2 * (m_ft_f + m_fr + m_fAlpha_Taper * (m_fb - m_fu - m_ft_w));
        //}
        //// Perimeter of section
        //void Calc_U()
        //{
        //    m_fU = 2 * (m_fh + 2 * m_fb - m_ft_w - (4.0f - MathF.fPI) * m_fr);
        //}
        //// Torsional inertia constant
        //void Calc_I_t()
        //{
        //    m_fI_t = ((m_fh - m_ft_f) * MathF.Pow3(m_ft_w) + 2* (m_fb - m_ft_w / 2.0f) * MathF.Pow3(m_ft_f)) / 3.0f;
        //}
        //// Torsional section modulus - plastic
        //void Calc_W_t_pl()
        //{
        //    m_fW_t_pl = (m_fh - m_ft_f) * MathF.Pow2(m_ft_w) / 2.0f + (m_fb - m_ft_w / 2.0f)* MathF.Pow2(m_ft_f);  
        //}

		protected override void loadCrScIndices()
        {
            // const int secNum = 8;  // Number of points in section (2D)
            TriangleIndices = new Int32Collection(36 + 8*6);

            // Front Side / Forehead
            AddRectangleIndices_CW_1234(TriangleIndices, 0, 1, 2, 3);
            AddRectangleIndices_CW_1234(TriangleIndices, 0, 3, 4, 7);
            AddRectangleIndices_CW_1234(TriangleIndices, 4, 5, 6, 7);

            // Back Side 
            AddRectangleIndices_CW_1234(TriangleIndices, 9, 8, 11, 10);
            AddRectangleIndices_CW_1234(TriangleIndices, 11, 8, 15, 12);
            AddRectangleIndices_CW_1234(TriangleIndices, 13, 12, 15, 14);

            // Shell Surface 
            DrawCaraLaterals(8, TriangleIndices);
        }

        protected override void loadCrScIndicesFrontSide()
        {
            throw new NotImplementedException();
        }

        protected override void loadCrScIndicesShell()
        {
            throw new NotImplementedException();
        }

        protected override void loadCrScIndicesBackSide()
        {
            throw new NotImplementedException();
        }

        public override void CalculateSectionProperties()
        {
            throw new NotImplementedException();
        }
    }
}
