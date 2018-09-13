using MATH;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace CRSC
{
    // Test cross-section class
    // Temporary Class - includes array of drawing points of cross-section in its coordinate system (LCS-for 2D yz)
    public class CCrSc_0_54 : CCrSc
    {
        // Welded Angle section

        /*




        1  _  2              ____|/
          | |                   /|
          | |                    |
          | |                    |
          | |                    |
  t_a=t_w | |                 h  |
          | |                    |
          | |                    |
          | | 3  *  t_b=t_f      |
          | |___________  4      |
          |_____________|    ____|/
        6                 5     /|
                  b
          |/____________|/
         /|            /|

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


        //----------------------------------------------------------------------------
        private float m_fh;   // Height / Vyska / Leg A
        private float m_fb;   // Width  / Sirka / Leg B
        private float m_ft_f; // Flange Thickness / Hrubka pasnice / Leg B
        private float m_ft_w; // Web Thickness  / Hrubka steny/stojiny / Leg A
        private float m_fy_c; // Centroid coordinate / Suradnica tažiska / Absolute value
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
            get { return m_fy_c; }
            set { m_fy_c = value; }
        }
        public float Fz_c
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
        public CCrSc_0_54()  {   }
        public CCrSc_0_54(float fh, float fb, float ft_f, float ft_w, float fy_c, float fz_c)
        {
            IsShapeSolid = true;
            ITotNoPoints = 6;
            m_fh = fh;
            m_fb = fb;
            m_ft_f = ft_f;
            m_ft_w = ft_w;
            m_fy_c = Math.Abs(fy_c); // Absolute value
            m_fz_c = Math.Abs(fz_c); // Absolute value

            // Create Array - allocate memory
            CrScPointsOut = new float [ITotNoPoints,2];
            //CrScPointsOut = new List<Point>(ITotNoPoints);
            // Fill Array Data
            CalcCrSc_Coord();

            // Fill list of indices for drawing of surface - triangles edges
            loadCrScIndices();
        }

        //----------------------------------------------------------------------------
        void CalcCrSc_Coord()
        {
            // Fill Point Array Data in LCS (Local Coordinate System of Cross-Section, horizontal y, vertical - z)

            // Point No. 1
            CrScPointsOut[0, 0] = -m_fy_c;           // y
            CrScPointsOut[0, 1] = m_fh - m_fz_c;     // z

            // Point No. 2
            CrScPointsOut[1, 0] = CrScPointsOut[0, 0] + m_ft_w;    // y
            CrScPointsOut[1, 1] = CrScPointsOut[0, 1];             // z

            // Point No. 3
            CrScPointsOut[2, 0] = CrScPointsOut[1, 0];    // y
            CrScPointsOut[2, 1] = -m_fz_c + m_ft_f;     // z

            // Point No. 4
            CrScPointsOut[3, 0] = CrScPointsOut[0, 0] + m_fb;     // y
            CrScPointsOut[3, 1] = CrScPointsOut[2, 1];            // z

            // Point No. 5
            CrScPointsOut[4, 0] = CrScPointsOut[3, 0];  // y
            CrScPointsOut[4, 1] = -m_fz_c;            // z

            // Point No. 6
            CrScPointsOut[5, 0] = CrScPointsOut[0, 0];     // y
            CrScPointsOut[5, 1] = CrScPointsOut[4, 1];     // z
        }
        //void CalcCrSc_Coord()
        //{
        //    // Fill Point Array Data in LCS (Local Coordinate System of Cross-Section, horizontal y, vertical - z)

        //    // Point No. 1            
        //    CrScPointsOut.Add(new Point(-m_fy_c, m_fh - m_fz_c));

        //    // Point No. 2            
        //    CrScPointsOut.Add(new Point(CrScPointsOut[0].X + m_ft_w, CrScPointsOut[0].Y));

        //    // Point No. 3            
        //    CrScPointsOut.Add(new Point(CrScPointsOut[1].X, -m_fz_c + m_ft_f));

        //    // Point No. 4            
        //    CrScPointsOut.Add(new Point(CrScPointsOut[0].X + m_fb, CrScPointsOut[2].Y));

        //    // Point No. 5            
        //    CrScPointsOut.Add(new Point(CrScPointsOut[3].X, -m_fz_c));

        //    // Point No. 6            
        //    CrScPointsOut.Add(new Point(CrScPointsOut[0].X, CrScPointsOut[4].Y));
        //}


        //----------------------------------------------------------------------------
        // Cross-section properties
        //----------------------------------------------------------------------------

        // Rolled steel
        // Monosymmetrical (equal legs)

        float m_fU, m_fA,
                m_fS_y, m_fI_y, m_fW_y_el, m_fW_y_pl, m_ff_y_plel,
                m_fS_z, m_fI_z, m_fW_z_el, m_fW_z_pl, m_ff_z_plel,
                m_fW_t_el, m_fI_t, m_fi_t, m_fW_t_pl, m_ff_t_plel,
                m_fEta_y_v, m_fA_y_v_el, m_fA_y_v_pl, m_ff_y_v_plel,
                m_fEta_z_v, m_fA_z_v_el, m_fA_z_v_pl, m_ff_z_v_plel;

        float m_ft,
            m_fr_1,   // Radius between faces of web and flange
            m_fr_2,
            m_fe_y,
            m_fe_z,
            m_fe_v,
            m_fz_u,
            m_fz_v,
            m_fI_w; // Section warping constant

        // Basic dimmensions
        void Calc_BasicDimension()
        {
            if (m_fr_1 < 0.0f)
                m_fr_1 = m_ft;

            if (m_fr_2 < 0.0f)
                m_fr_2 = m_ft;

            m_fe_v = m_fe_y * MathF.fSqrt2;
            m_fz_u = m_fh / MathF.fSqrt2;
            m_fz_v = (m_fh + m_ft - 2 * m_fr_2) / MathF.fSqrt2 + m_fr_2 - m_fe_v;
        }

        // Perimeter of section
        void Calc_U()
        {
            m_fU = 4 * m_fh - (4- MathF.fPI) * (m_fr_2 + m_fr_1 /2.0f);
        }
        // Section area
        void Calc_A()
        {
            m_fA = (2.0f * m_fh - m_ft) * m_ft + (1.0f - MathF.fPI / 4.0f) * (MathF.Pow2(m_fr_1) - 2.0f * MathF.Pow2(m_fr_2));
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
            m_fW_z_el = m_fI_z / (m_fb - m_fe_z);
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
            m_fI_t = 2 * (m_fh - m_ft) * MathF.Pow3(m_ft) / 3.0f;
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
            m_fW_t_pl = m_fh * MathF.Pow2(m_ft) + 2.0f * MathF.Pow3(m_ft) / 3.0f;
        }
        // Torsional shape factor plastic/elastic
        void Calc_f_t_plel()
        {
            m_ff_t_plel = m_fW_t_pl / m_fW_t_el;
        }
        // Section warping constant
        void Calc_I_w()
        {
            m_fI_w = (MathF.Pow3(m_fh - m_ft) + MathF.Pow3(m_fb - m_ft)) * MathF.Pow3(m_ft) / 36.0f;
        }


        // Shear factor
        void Calc_Eta_y_v()
        {
            m_fEta_y_v = (m_fA / MathF.Pow2(m_fI_y)) /*  Syi^2 / byi *dz */ ;  // Temp
        }
        // Shear effective area - elastic
        void Calc_A_y_v_el()
        {
            m_fA_y_v_el = m_ft * m_fI_y / m_fS_y;
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
            m_fA_z_v_el = m_ft * m_fI_z / m_fS_z;
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


        // Unequal legs

        float m_fe_u;
        float m_fy_C, m_fz_C;

        //// Basic dimmensions
        //void Calc_BasicDimension()
        //{
        //    if (m_fr_1 < 0.0f)
        //        m_fr_1 = m_ft;

        //    if (m_fr_2 < 0.0f)
        //        m_fr_2 = m_ft;

        //    m_fAlpha = (float)Math.Atan(2.0f * m_fI_yz / (m_fI_y - m_fI_z)) / 2.0f;

        //    m_fe_u = (m_fb - m_fe_z) * (float)Math.Sin(m_fAlpha) + m_fe_y * (float)Math.Cos(m_fAlpha);
        //    m_fe_v = m_fe_z * (float)Math.Cos(m_fAlpha) + m_fe_y * (float) Math.Sin(m_fAlpha);

        //    m_fz_u = (m_fh - m_fe_y) * (float)Math.Cos(m_fAlpha) + m_fe_z * (float)Math.Sin(m_fAlpha);
        //    m_fz_v = (m_fb - m_fz_C - m_fr_2) * (float)Math.Cos(m_fAlpha) + (m_ft - m_fr_2 - m_fe_y) * (float)Math.Sin(m_fAlpha) + m_fr_2;
        //}

        //// Perimeter of section
        //void Calc_U()
        //{
        //    m_fU = 2 * (m_fh + m_fb) - (4 - MathF.fPI) * (m_fr_2 + m_fr_1 / 2.0f);
        //}
        //// Section area
        //void Calc_A()
        //{
        //    m_fA = (m_fb + m_fh - m_ft) * m_ft + (1.0f - MathF.fPI / 4.0f) * (MathF.Pow2(m_fr_1) - 2.0f * MathF.Pow2(m_fr_2));
        //}


        //// First moment o area
        //void Calc_S_y()
        //{
        //    // m_fS_y;
        //}
        //// Second moment of area
        //void Calc_I_y()
        //{
        //    // m_fI_y;
        //}
        //// Section modulus - elastic
        //void Calc_W_y_el()
        //{
        //    m_fW_y_el = m_fI_y / (m_fh - m_fe_y);
        //}
        //// Section modulus - plastic
        //void Calc_W_y_pl()
        //{
        //    // m_fW_y_pl;
        //}
        //// Shape factor - plastic/elastic
        //void Calc_f_y_plel()
        //{
        //    m_ff_y_plel = m_fW_y_pl / m_fW_y_el;
        //}


        //// First moment o area
        //void Calc_S_z()
        //{
        //    // m_fS_z;
        //}
        //// Second moment of area
        //void Calc_I_z()
        //{
        //    // m_fI_z;
        //}
        //// Section modulus - elastic
        //void Calc_W_z_el()
        //{
        //    m_fW_z_el = m_fI_z / (m_fb - m_fe_z);
        //}
        //// Section modulus - plastic
        //void Calc_W_z_pl()
        //{
        //    // m_fW_z_pl;
        //}
        //// Shape factor - plastic/elastic
        //void Calc_f_z_plel()
        //{
        //    m_ff_z_plel = m_fW_z_pl / m_fW_z_el;
        //}


        //// Torsional inertia constant
        //void Calc_I_t()
        //{
        //    m_fI_t = (m_fh + m_fb - 2 * m_ft) * MathF.Pow3(m_ft) / 3.0f;
        //}
        //// Torsional radius of gyration
        //void Calc_i_t()
        //{
        //    m_fi_t = MathF.Sqrt(m_fI_t / m_fA);
        //}
        //// Torsional section modulus - elastic
        //void Calc_W_t_el()
        //{
        //    m_fW_t_el = m_fI_t / m_ft;
        //}
        //// Torsional section modulus - plastic
        //void Calc_W_t_pl()
        //{
        //    m_fW_t_pl = (m_fh + m_fb) * MathF.Pow2(m_ft) / 2.0f + 2.0f * MathF.Pow3(m_ft) / 3.0f;
        //}
        //// Torsional shape factor plastic/elastic
        //void Calc_f_t_plel()
        //{
        //    m_ff_t_plel = m_fW_t_pl / m_fW_t_el;
        //}
        //// Section warping constant
        //void Calc_I_w()
        //{
        //    m_fI_w = (MathF.Pow3(m_fh - m_ft) + MathF.Pow3(m_fb - m_ft)) * MathF.Pow3(m_ft) / 36.0f;
        //}


        //// Shear factor
        //void Calc_Eta_y_v()
        //{
        //    m_fEta_y_v = (m_fA / MathF.Pow2(m_fI_y)) /*  Syi^2 / byi *dz */ ;  // Temp
        //}
        //// Shear effective area - elastic
        //void Calc_A_y_v_el()
        //{
        //    m_fA_y_v_el = m_ft * m_fI_y / m_fS_y;
        //}
        //// Shape factor for shear - plastic/elastic
        //void Calc_f_y_v_plel()
        //{
        //    m_ff_y_v_plel = 1.00f; // Temp
        //}
        //// Shear effective area - plastic
        //void Calc_A_y_v_pl()
        //{
        //    m_fA_y_v_pl = m_ff_y_v_plel * m_fA_y_v_el; // Temp
        //}


        //// Shear factor
        //void Calc_Eta_z_v()
        //{
        //    m_fEta_z_v = (m_fA / MathF.Pow2(m_fI_z)) /*  Szi^2 / bzi *dy */ ;  // Temp
        //}
        //// Shear effective area - elastic
        //void Calc_A_z_v_el()
        //{
        //    m_fA_z_v_el = m_ft * m_fI_z / m_fS_z;
        //}
        //// Shape factor for shear - plastic/elastic
        //void Calc_f_z_v_plel()
        //{
        //    m_ff_z_v_plel = 1.00f; // Temp
        //}
        //// Shear effective area - plastic
        //void Calc_A_z_v_pl()
        //{
        //    m_fA_z_v_pl = m_ff_z_v_plel * m_fA_z_v_el; // Temp
        //}

        float m_fI_u, m_fI_v, m_fW_u_el, m_fW_v_el, m_fAlpha, m_fI_yz;

        // Second moment of area
        void Calc_I_u()
        {
            m_fI_u = m_fI_y * MathF.Pow2((float)Math.Cos(m_fAlpha)) + m_fI_z * MathF.Pow2((float)Math.Sin(m_fAlpha)) + m_fI_yz * (float)Math.Sin(2.0f * m_fAlpha);
        }
        // Second moment of area
        void Calc_I_v()
        {
            m_fI_v = m_fI_z * MathF.Pow2((float)Math.Cos(m_fAlpha)) + m_fI_y * MathF.Pow2((float)Math.Sin(m_fAlpha)) - m_fI_yz * (float)Math.Sin(2.0f * m_fAlpha);
        }
        // Section modulus - elastic
        void Calc_W_u_el()
        {
            m_fW_u_el = m_fI_u / Math.Max(m_fe_u, m_fz_u);
        }
        // Section modulus - elastic
        void Calc_W_v_el()
        {
            m_fW_v_el = m_fI_v / Math.Max(m_fe_v, m_fz_v);
        }

		protected override void loadCrScIndices()
        {
            // const int secNum = 6;  // Number of points in section (2D)
            TriangleIndices = new Int32Collection(24 + 6*6);

            // Front Side / Forehead
            AddRectangleIndices_CW_1234(TriangleIndices, 0, 1, 2, 5);
            AddRectangleIndices_CW_1234(TriangleIndices, 2, 3, 4, 5);

            // Back Side 
            AddRectangleIndices_CW_1234(TriangleIndices, 7, 6, 11, 8);
            AddRectangleIndices_CW_1234(TriangleIndices, 9, 8, 11, 10);

            // Shell Surface
            DrawCaraLaterals(6, TriangleIndices);
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
