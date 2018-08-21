using MATH;
using System;
using System.Windows.Media;

namespace CRSC
{
    // Test cross-section class
    // Temporary Class - includes array of drawing points of cross-section in its coordinate system (LCS-for 2D yz)
    public class CCrSc_0_25 : CCrSc
    {
        // Welded hollow section - doubly symmetrical

        /*




        1  ________________  2     ____|/
          |  ____________  |  t_f     /|
          | | 5        6 | |           |
          | |            | |           |
          | |            | |           |
      t_w | |            | |        h  |
          | |      *     | |           |
          | |            | |           |
          | | 8        7 | |           |
          | |____________| |           |
          |________________|       ____|/
        4                    3        /|
                  b
          |/_______________|/
         /|               /|


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
        public CCrSc_0_25()  {   }
        public CCrSc_0_25(float fh, float fb, float ft_f, float ft_w)
        {
            IsShapeSolid = false;

            INoPointsIn = INoPointsOut = 4; // 4+4
            m_fh = fh;
            m_fb = fb;
            m_ft_f = ft_f;
            m_ft_w = ft_w;

            // Create Array - allocate memory
            CrScPointsOut = new float[INoPointsOut, 2];
            CrScPointsIn = new float[INoPointsIn, 2];

            // Fill Array Data
            CalcCrSc_Coord();

            // Fill list of indices for drawing of surface - triangles edges
            loadCrScIndices();
        }

        //----------------------------------------------------------------------------
        public void CalcCrSc_Coord()
        {
            // Fill Point Array Data in LCS (Local Coordinate System of Cross-Section, horizontal y, vertical - z)

            // Outside

            // Point No. 1
            CrScPointsOut[0,0] = -m_fb / 2f;    // y
            CrScPointsOut[0,1] = m_fh / 2f;     // z

            // Point No. 2
            CrScPointsOut[1,0] = -CrScPointsOut[0,0];    // y
            CrScPointsOut[1,1] = CrScPointsOut[0,1];     // z

            // Point No. 3
            CrScPointsOut[2,0] = -CrScPointsOut[0,0];    // y
            CrScPointsOut[2,1] = -CrScPointsOut[1,1];    // z

            // Point No. 4
            CrScPointsOut[3,0] = CrScPointsOut[0,0];     // y
            CrScPointsOut[3,1] = CrScPointsOut[2,1];     // z

            // Inside

            // Point No. 5
            CrScPointsIn[0,0] = CrScPointsOut[0,0] + m_ft_w;  // y
            CrScPointsIn[0,1] = CrScPointsOut[0, 1] - m_ft_f; // z

            // Point No. 6
            CrScPointsIn[1,0] = -CrScPointsIn[0,0];    // y
            CrScPointsIn[1,1] = CrScPointsIn[0,1];     // z

            // Point No. 7
            CrScPointsIn[2,0] = CrScPointsIn[1,0];     // y
            CrScPointsIn[2,1] = -CrScPointsIn[1,1];    // z

            // Point No. 8
            CrScPointsIn[3,0] = CrScPointsIn[0,0];     // y
            CrScPointsIn[3,1] = -CrScPointsIn[0,1];     // z
        }


        // Temporary for hollow rectanngular and square section - steel !!!

        // Use for shape 3_07  


        // SH(S)S - square hollow (structural) section

        //----------------------------------------------------------------------------
        // Cross-section properties
        //----------------------------------------------------------------------------

        float m_ft, m_fr; // Temporary

        // default
        // m_fr = 2*m_ft;

        float m_fU, m_fA,
            m_fU_m, m_fA_m,
            m_fW_t_el, m_fI_t, m_fW_t_pl, m_ff_t_plel,
            m_fS, m_fI, m_fW_el, m_fW_pl, m_ff_plel,
            m_fEta_v, m_fA_v_el, m_fA_v_pl, m_ff_v_plel;

        // Mid-thickness perimeter of section
        void Calc_U_m()
        {
            m_fU_m = 2 * (2* m_fh - 2* m_ft + (m_fr - m_ft /2f) * (MathF.fPI - 4));
        }
        // Mid-thickness area of section
        void Calc_A_m()
        {
            m_fA_m = MathF.Pow2(m_fh - m_ft) - MathF.Pow2(m_fr - m_ft / 2f) * (4 - MathF.fPI);
        }
        // Perimeter of section
        void Calc_U()
        {
            m_fU = 2 * (2 * m_fh + m_fr * (MathF.fPI - 4));
        }
        // Section area
        void Calc_A()
        {
            m_fA = m_ft * m_fU_m;
        }
        // Torsional inertia constant
        void Calc_I_t()
        {
            m_fI_t = m_fU_m * MathF.Pow3(m_ft) / 3f + 4* m_ft * MathF.Pow2(m_fA_m) / m_fU_m;
        }
        // Torsional section modulus - elastic
        void Calc_W_t_el()
        {
            m_fW_t_el = m_fI_t / (m_ft + 2f* m_fA_m / m_fU_m);
        }
        // Torsional section modulus - plastic
        void Calc_W_t_pl()
        {
            m_fW_t_pl = m_fW_t_el; // TEMP
        }
        // Torsional shape factor plastic/elastic
        void Calc_f_t_plel()
        {
            m_ff_t_plel = m_fW_t_pl / m_fW_t_el;
        }
        // First moment o area
        void Calc_S()
        {
            /// ????
        }
        // Second moment of area
        void Calc_I()
        {
            /// ????
        }
        // Section modulus - elastic
        void Calc_W_el()
        {
            m_fW_el = 2 * m_fI  / m_fh;
        }
        // Section modulus - plastic
        void Calc_W_pl()
        {
            /// ????
        }
        // Shape factor - plastic/elastic
        void Calc_f_plel()
        {
            m_ff_plel = m_fW_pl / m_fW_el;
        }
        // Shear factor
        void Calc_Eta_v()
        {
            /// ????
            // m_fEta_v = (m_fA / MathF.Pow2(m_fI) * Integral m_fSyi^2 / byi dz;
        }
        // Shear effective area - elastic
        void Calc_A_v_el()
        {
            /// ????
            // m_fA_v_el = m_fI * Math.Min(byi / Syi);
        }
        // Shape factor for shear - plastic/elastic
        void Calc_f_v_plel()
        {
            /// ????
            m_ff_v_plel = 1.00f;
        }
        // Shear effective area - plastic
        void Calc_A_v_pl()
        {
            // ???? 
            m_fA_v_pl = m_ff_v_plel * m_fA_v_el;
        }





        // RH(S)S - rectangular hollow (structural) section

        
        // Note - it is better to create two classes , one for SHS and the other one for RHS
        // just for steel and aluminium rolled profiles !!!!

      

         float  m_fS_y, m_fI_y, m_fW_y_el, m_fW_y_pl, m_ff_y_plel,
                m_fS_z, m_fI_z, m_fW_z_el, m_fW_z_pl, m_ff_z_plel,
                m_fEta_y_v, m_fA_y_v_el, m_fA_y_v_pl, m_ff_y_v_plel,
                m_fEta_z_v, m_fA_z_v_el, m_fA_z_v_pl, m_ff_z_v_plel;

        //// Mid-thickness perimeter of section
        //void Calc_U_m()
        //{
        //    m_fU_m = 2 * (m_fb * m_fh - 2 * m_ft + (m_fr - m_ft / 2f) * (MathF.fPI - 4));
        //}
        //// Mid-thickness area of section
        //void Calc_A_m()
        //{
        //    m_fA_m = (m_fh - m_ft) * (m_fb - m_ft) - MathF.Pow2(m_fr - m_ft / 2f) * (4 - MathF.fPI);
        //}
        //// Perimeter of section
        //void Calc_U()
        //{
        //    m_fU = 2 * (m_fb + m_fh + m_fr * (MathF.fPI - 4));
        //}
        //// Section area
        //void Calc_A()
        //{
        //    m_fA = m_ft * m_fU_m;
        //}
        //// Torsional inertia constant
        //void Calc_I_t()
        //{
        //    m_fI_t = m_fU_m * MathF.Pow3(m_ft) / 3f + 4 * m_ft * MathF.Pow2(m_fA_m) / m_fU_m;
        //}
        //// Torsional section modulus - elastic
        //void Calc_W_t_el()
        //{
        //    m_fW_t_el = m_fI_t / (m_ft + 2f * m_fA_m / m_fU_m);
        //}
        //// Torsional section modulus - plastic
        //void Calc_W_t_pl()
        //{
        //    m_fW_t_pl = m_fW_t_el; // TEMP
        //}
        //// Torsional shape factor plastic/elastic
        //void Calc_f_t_plel()
        //{
        //    m_ff_t_plel = m_fW_t_pl / m_fW_t_el;
        //}


        // First moment o area
        void Calc_S_y()
        {
            /// ????
        }
        // Second moment of area
        void Calc_I_y()
        {
            /// ????
        }
        // Section modulus - elastic
        void Calc_W_y_el()
        {
            m_fW_y_el = 2 * m_fI_y / m_fh;
        }
        // Section modulus - plastic
        void Calc_W_y_pl()
        {
            /// ????
        }
        // Shape factor - plastic/elastic
        void Calc_f_y_plel()
        {
            m_ff_y_plel = m_fW_y_pl / m_fW_y_el;
        }
        // Shear factor
        void Calc_Eta_y_v()
        {
            /// ????
            // m_fEta_y_v = (m_fA / MathF.Pow2(m_fI_y) * Integral m_fSyi^2 / byi dz;
        }
        // Shear effective area - elastic
        void Calc_A_y_v_el()
        {
            /// ????
            // m_fA_y_v_el = m_fI_y * Math.Min(byi / Syi);
        }
        // Shape factor for shear - plastic/elastic
        void Calc_f_y_v_plel()
        {
            /// ????
            m_ff_y_v_plel = 1.00f;
        }
        // Shear effective area - plastic
        void Calc_A_y_v_pl()
        {
            // ???? 
            m_fA_y_v_pl = m_ff_y_v_plel * m_fA_y_v_el;
        }


        // First moment o area
        void Calc_S_z()
        {
            /// ????
        }

        /*
         * 
         * 
         *
         *  for z-axis
         * 
         * 
         */

        protected override void loadCrScIndices()
        {
            loadCrScIndices_25();
        }
        public void loadCrScIndices_25()
        {
            // const int secNum = 8;  // Number of points in section (2D)
            TriangleIndices = new Int32Collection(16*6);

            // Front Side / Forehead
            AddRectangleIndices_CW_1234(TriangleIndices, 0, 1, 5, 4);
            AddRectangleIndices_CW_1234(TriangleIndices, 1, 2, 6, 5);
            AddRectangleIndices_CW_1234(TriangleIndices, 7, 6, 2, 3);
            AddRectangleIndices_CW_1234(TriangleIndices, 0, 4, 7, 3);

            // Back Side
            AddRectangleIndices_CW_1234(TriangleIndices, 9, 8, 12, 13);
            AddRectangleIndices_CW_1234(TriangleIndices, 9, 13, 14, 10);
            AddRectangleIndices_CW_1234(TriangleIndices, 14, 15, 11, 10);
            AddRectangleIndices_CW_1234(TriangleIndices, 8, 11, 15, 12);

            // Shell Surface
            // Outside
            AddRectangleIndices_CW_1234(TriangleIndices, 0, 8, 9, 1);
            AddRectangleIndices_CW_1234(TriangleIndices, 1, 9, 10, 2);
            AddRectangleIndices_CW_1234(TriangleIndices, 2, 10, 11, 3);
            AddRectangleIndices_CW_1234(TriangleIndices, 8, 0, 3, 11);
            // Inside
            AddRectangleIndices_CW_1234(TriangleIndices, 13, 5, 6, 14);
            AddRectangleIndices_CW_1234(TriangleIndices, 7, 15, 14, 6);
            AddRectangleIndices_CW_1234(TriangleIndices, 4, 12, 15, 7);
            AddRectangleIndices_CW_1234(TriangleIndices, 12, 4, 5, 13);
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
