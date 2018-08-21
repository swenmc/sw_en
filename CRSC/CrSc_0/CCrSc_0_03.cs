using MATH;
using System;

namespace CRSC
{
    // Test cross-section class
    // Temporary Class - includes array of drawing points of cross-section in its coordinate system (LCS-for 2D yz)
    public class CCrSc_0_03 : CCrSc
    {
        // Solid Ellipse

        //----------------------------------------------------------------------------
        private float m_fa;           // Major Axis Dimension (2x Length of Semimajor Axis)
        private float m_fb;           // Minor Axis Dimension (2x Length of Semiminor Axis)
        private float m_fa_semi;      // Semimajor Axis Dimension
        private float m_fb_semi;      // Semiminor Axis Dimension
        //private float m_fr_1;         // Radius max
        //private float m_fr_2;         // Radius min
        private float m_fAngle;       // Angle of Rotation
        //private short m_iTotNoPoints; // Total Number of Cross-section Points for Drawing (withCentroid Point)
        //public float[,] m_CrScPoint;  // Array of Points and values in 2D
        //----------------------------------------------------------------------------

        public float Fa
        {
            get { return m_fa; }
            set { m_fa = value; }
        }

        /*public short ITotNoPoints
        {
            get { return m_iTotNoPoints; }
            set { m_iTotNoPoints = value; }
        }*/

        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        public CCrSc_0_03()  {   }
        public CCrSc_0_03(float fa, float fb, short iTotNoPoints)
        {
            IsShapeSolid = true;
            // m_iTotNoPoints = 72+1; // vykreslujeme ako plny n-uholnik + 1 stredovy bod
            m_fa = fa;
            m_fb = fb;

            m_fa_semi = 0.5f * m_fa;
            m_fb_semi = 0.5f * m_fb;

            ITotNoPoints = iTotNoPoints; // auxialiary node in centroid / stredovy bod v tazisku

            m_fAngle = 0;

            if (iTotNoPoints < 2 || fa <= 0f || fb <= 0f)
                return;

            // Create Array - allocate memory
            CrScPointsOut = new float[ITotNoPoints, 2];
            // Fill Array Data
            CalcCrSc_Coord();

            // Fill list of indices for drawing of surface - triangles edges
            loadCrScIndices();
        }
        public CCrSc_0_03(float fa, float fb)
        {
            IsShapeSolid = true;
            // ITotNoPoints = 72+1; // vykreslujeme ako plny n-uholnik + 1 stredovy bod
            m_fa = fa;
            m_fb = fb;
            ITotNoPoints = 73; // 1 auxialiary node in centroid / stredovy bod v tazisku

            m_fAngle = 0;

            if (fa <= 0f || fb <= 0f)
                return;

            // Create Array - allocate memory
            CrScPointsOut = new float[ITotNoPoints, 2];
            // Fill Array Data
            CalcCrSc_Coord();

            // Fill list of indices for drawing of surface - triangles edges
            loadCrScIndices();
        }

        //----------------------------------------------------------------------------
        void CalcCrSc_Coord()
        {
            // Basic Ellipse Function
            // Zbytocne vytvaram nove pole !!!!
            float [,] arrtemp = new float [ITotNoPoints - 1,2];
            arrtemp = Geom2D.GetEllipsePointCoord(0.5f * m_fa, 0.5f * m_fb, m_fAngle, (short)((int)ITotNoPoints - 1));
            // Fill Point Array Data in LCS (Local Coordinate System of Cross-Section, horizontal y, vertical - z)
            // Outside Points Coordinates
            for (int i = 0; i < ITotNoPoints-1; i++)
            {
                CrScPointsOut[i, 0] = arrtemp[i, 0];  // y
                CrScPointsOut[i, 1] = arrtemp[i, 1];  // z
            }

            // Centroid
            CrScPointsOut[ITotNoPoints-1, 0] = 0f;
            CrScPointsOut[ITotNoPoints-1, 1] = 0f;
        }


        //----------------------------------------------------------------------------
        // Cross-section properties
        //----------------------------------------------------------------------------

        float m_fU, m_fA,
                m_fS_y, m_fI_y, m_fW_y_el, m_fW_y_pl, m_ff_y_plel,
                m_fS_z, m_fI_z, m_fW_z_el, m_fW_z_pl, m_ff_z_plel,
                m_fW_t_el, m_fI_t, m_fi_t, m_fq_t, m_fW_t_pl, m_ff_t_plel,
                m_fEta_y_v, m_fA_y_v_el, m_fA_y_v_pl, m_ff_y_v_plel,
                m_fEta_z_v, m_fA_z_v_el, m_fA_z_v_pl, m_ff_z_v_plel;

        // Perimeter of section
        void Calc_U()
        {
            m_fU = MathF.fPI * MathF.Sqrt(2 * (MathF.Pow2(m_fa_semi) + MathF.Pow2(m_fb_semi)) - MathF.Pow2(m_fa_semi - m_fb_semi) / 2.2f);
        }
        // Section area
        void Calc_A()
        {
            m_fA = MathF.fPI * m_fa_semi * m_fb_semi;
        }


        // First moment o area
        void Calc_S_y()
        {
            m_fS_y = 2* m_fb_semi * MathF.Pow2(m_fa_semi) / 3f;
        }
        // Second moment of area
        void Calc_I_y()
        {
            m_fI_y = MathF.fPI * m_fb_semi * MathF.Pow3(m_fa_semi) / 4f;
        }
        // Section modulus - elastic
        void Calc_W_y_el()
        {
            m_fW_y_el = MathF.fPI * m_fb_semi * MathF.Pow2(m_fa_semi) / 4f;
        }
        // Section modulus - plastic
        void Calc_W_y_pl()
        {
            m_fW_y_pl = 4 * m_fb_semi * MathF.Pow2(m_fa_semi) / 3f;
        }
        // Shape factor - plastic/elastic
        void Calc_f_y_plel()
        {
            m_ff_y_plel = m_fW_y_pl / m_fW_y_el;
        }


        // First moment o area
        void Calc_S_z()
        {
            m_fS_z = 2 * m_fa_semi * MathF.Pow2(m_fb_semi) / 3f;
        }
        // Second moment of area
        void Calc_I_z()
        {
            m_fI_z = MathF.fPI * m_fa_semi * MathF.Pow3(m_fb_semi) / 4f;
        }
        // Section modulus - elastic
        void Calc_W_z_el()
        {
            m_fW_z_el = MathF.fPI * m_fa_semi * MathF.Pow2(m_fb_semi) / 4f;
        }
        // Section modulus - plastic
        void Calc_W_z_pl()
        {
            m_fW_z_pl = 4 * m_fa_semi * MathF.Pow2(m_fb_semi) / 3f;
        }
        // Shape factor - plastic/elastic
        void Calc_f_z_plel()
        {
            m_ff_z_plel = m_fW_z_pl / m_fW_z_el;
        }


        // Torsional inertia constant
        void Calc_I_t()
        {
            m_fI_t = (MathF.fPI * m_fb_semi * MathF.Pow3(m_fa_semi) / 4f)  +  (MathF.fPI * m_fa_semi * MathF.Pow3(m_fb_semi) / 4f); // I_y + I_z
        }
        // Torsional radius of gyration
        void Calc_i_t()
        {
            m_fi_t = MathF.Sqrt(m_fI_t / m_fA);
        }
        // Torsion factor
        void Calc_q_t()
        {
            m_fq_t = m_fI_t * (MathF.Pow2(m_fa_semi) + MathF.Pow2(m_fb_semi)) / ( MathF.fPI * MathF.Pow3(m_fa_semi) * MathF.Pow3(m_fb_semi));
        }
        // Torsional section modulus - elastic
        void Calc_W_t_el()
        {
            m_fW_t_el = MathF.fPI * m_fa_semi * MathF.Pow2(m_fb_semi) / 2f;
        }
        // Torsional section modulus - plastic
        void Calc_W_t_pl()
        {
            m_fW_t_pl = (2 * MathF.fPI * m_fa_semi * MathF.Pow2(m_fb_semi) / 3f) * (1.046f + 0.314f * m_fb_semi / m_fa_semi - 1.38f * (MathF.Pow2(m_fb_semi) / MathF.Pow2(m_fa_semi)) + 1.762f * (MathF.Pow3(m_fb_semi) / MathF.Pow3(m_fa_semi)) - 0.741f* (MathF.Pow4(m_fb_semi) / MathF.Pow4(m_fa_semi)));
        }
        // Torsional shape factor plastic/elastic
        void Calc_f_t_plel()
        {
            m_ff_t_plel = m_fW_t_pl / m_fW_t_el;
        }


        // Shear factor
        void Calc_Eta_y_v()
        {
            m_fEta_y_v = 10f/9f + 2f/27f * MathF.Pow2(m_fb_semi) / MathF.Pow2(m_fa_semi);
        }
        // Shear effective area - elastic
        void Calc_A_y_v_el()
        {
            m_fA_y_v_el = 0.75f * m_fA;
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
            m_fEta_z_v = 10f / 9f + 2f / 27f * MathF.Pow2(m_fa_semi) / MathF.Pow2(m_fb_semi);
        }
        // Shear effective area - elastic
        void Calc_A_z_v_el()
        {
            m_fA_z_v_el = 0.75f * m_fA;
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


        // Auxialiary
        float m_fF_distance;

        // Distance between focuses 
        void Calc_F_distance()
        {
            m_fF_distance = 2* MathF.Sqrt(MathF.Pow2(m_fa_semi) - MathF.Pow2(m_fb_semi));
        }

		protected override void loadCrScIndices()
        {
            CCrSc_0_02 oTemp = new CCrSc_0_02();
            oTemp.loadCrScIndices_02_03(ITotNoPoints);            
            TriangleIndices = oTemp.TriangleIndices;
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
