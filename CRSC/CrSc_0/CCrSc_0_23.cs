using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using MATH;
using BaseClasses;

namespace CRSC
{
    // Test cross-section class
    // Temporary Class - includes array of drawing points of cross-section in its coordinate system (LCS-for 2D yz)
    public class CCrSc_0_23 : CCrSc
    {
        // Elliptical Hollow Section / Elipsa

        //----------------------------------------------------------------------------
        private float m_fa;   // Major Axis Dimension (2x Length of Semimajor Axis)
        private float m_fb;   // Minor Axis Dimension (2x Length of Semiminor Axis)
        private float m_ft;   // Thickness
        private float m_fAngle; // Angle of Rotation
        //private short m_iNoPoints; // Number of Cross-section Points for Drawing in One Ellipse (36)
        //public float[,] m_CrScPointOut; // Array of Outside Points and values in 2D
        //public float[,] m_CrScPointIn; // Array of Inside Points and values in 2D
        //----------------------------------------------------------------------------

        public float Fa
        {
            get { return m_fa; }
            set { m_fa = value; }
        }

        public float Fb
        {
            get { return m_fb; }
            set { m_fb = value; }
        }

        public float Ft
        {
            get { return m_ft; }
            set { m_ft = value; }
        }

        public float FAngle
        {
            get { return m_fAngle; }
            set { m_fAngle = value; }
        }

        /*public short INoPoints
        {
            get { return m_iNoPoints; }
            set { m_iNoPoints = value; }
        }*/
        
        // Auxiliary variables
        float m_fr_out_major;

        public float Fr_out_major
        {
            get { return m_fr_out_major; }
            set { m_fr_out_major = value; }
        }
        float m_fr_in_major;

        public float Fr_in_major
        {
            get { return m_fr_in_major; }
            set { m_fr_in_major = value; }
        }
        float m_fr_out_minor;

        public float Fr_out_minor
        {
            get { return m_fr_out_minor; }
            set { m_fr_out_minor = value; }
        }
        float m_fr_in_minor;

        public float Fr_in_minor
        {
            get { return m_fr_in_minor; }
            set { m_fr_in_minor = value; }
        }

        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        public CCrSc_0_23() { }
        public CCrSc_0_23(float fa, float fb, float ft, short iNoPoints)
        {
            INoPointsIn = INoPointsOut = iNoPoints; // vykreslujeme ako n-uholnik, pocet bodov n
            m_fa = fa;
            m_fb = fb;
            m_ft = ft;

            m_fAngle = 90f;

            // Radii
            m_fr_out_major = m_fa / 2f;
            m_fr_in_major = m_fa / 2f - m_ft;

            m_fr_out_minor = m_fb / 2f;
            m_fr_in_minor = m_fb / 2f - m_ft;

            if (iNoPoints < 2 || m_fr_in_major == m_fr_out_major || m_fr_in_minor == m_fr_out_minor)
                return;


            // Create Array - allocate memory
            CrScPointsOut = new float[INoPointsOut, 2];
            CrScPointsIn = new float[INoPointsIn, 2];

            // Fill Array Data
            CalcCrSc_Coord();

            // Fill list of indices for drawing of surface - triangles edges
            loadCrScIndices();
        }
        public CCrSc_0_23(float fa, float fb, float ft)
        {
            INoPointsIn = INoPointsOut = 72; // vykreslujeme ako n-uholnik, pocet bodov n
            m_fa = fa;
            m_fb = fb;
            m_ft = ft;

            m_fAngle = 90f;

            // Radii
            m_fr_out_major = m_fa / 2f;
            m_fr_in_major = m_fa / 2f - m_ft;

            m_fr_out_minor = m_fb / 2f;
            m_fr_in_minor = m_fb / 2f - m_ft;

            if (m_fr_in_major == m_fr_out_major || m_fr_in_minor == m_fr_out_minor)
                return;

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
            // INoPoints = 72; // vykreslujeme ako n-uholnik

            // Outside Points Coordinates
            CrScPointsOut = Geom2D.GetEllipsePointCoord(m_fr_out_major, m_fr_out_minor, m_fAngle, INoPointsOut);

            // Inside Points
            CrScPointsIn = Geom2D.GetEllipsePointCoord(m_fr_in_major, m_fr_in_minor, m_fAngle, INoPointsIn);
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
            m_fU = MathF.fPI * (MathF.Sqrt(2 * (MathF.Pow2(m_fr_out_major) + MathF.Pow2(m_fr_out_minor)) - MathF.Pow2(m_fr_out_major - m_fr_out_minor) / 2.2f)
                + MathF.Sqrt(2 * (MathF.Pow2(m_fr_in_major) + MathF.Pow2(m_fr_in_minor)) - MathF.Pow2(m_fr_in_major - m_fr_in_minor) / 2.2f));
        }
        // Section area
        void Calc_A()
        {
            m_fA = MathF.fPI * m_ft * (m_fr_out_major + m_fr_out_minor - m_ft);
        }
        
        // First moment o area
        void Calc_S_y()
        {
            m_fS_y = 2 * (m_fr_out_minor * MathF.Pow2(m_fr_out_major) - m_fr_in_minor * MathF.Pow2(m_fr_in_major)) / 3f;
        }
        // Second moment of area
        void Calc_I_y()
        {
            m_fI_y = MathF.fPI * (m_fr_out_minor * MathF.Pow3(m_fr_out_major) - m_fr_in_minor * MathF.Pow3(m_fr_in_major))  / 4f;
        }
        // Section modulus - elastic
        void Calc_W_y_el()
        {
            m_fW_y_el = m_fI_y / m_fr_out_major;
        }
        // Section modulus - plastic
        void Calc_W_y_pl()
        {
            m_fW_y_pl = 4 * (m_fr_out_minor * MathF.Pow2(m_fr_out_major) - m_fr_in_minor * MathF.Pow2(m_fr_in_major)) / 3f;
        }
        // Shape factor - plastic/elastic
        void Calc_f_y_plel()
        {
            m_ff_y_plel = m_fW_y_pl / m_fW_y_el;
        }


        // First moment o area
        void Calc_S_z()
        {
            m_fS_z = 2 * (m_fr_out_major * MathF.Pow2(m_fr_out_minor) - m_fr_in_major * MathF.Pow2(m_fr_in_minor)) / 3f;
        }
        // Second moment of area
        void Calc_I_z()
        {
            m_fI_z = MathF.fPI * (m_fr_out_major * MathF.Pow3(m_fr_out_minor) - m_fr_in_major * MathF.Pow3(m_fr_in_minor)) / 4f;
        }
        // Section modulus - elastic
        void Calc_W_z_el()
        {
            m_fW_z_el = m_fI_z / m_fr_out_minor;
        }
        // Section modulus - plastic
        void Calc_W_z_pl()
        {
            m_fW_z_pl = 4 * (m_fr_out_major * MathF.Pow2(m_fr_out_minor) - m_fr_in_major * MathF.Pow2(m_fr_in_minor)) / 3f;
        }
        // Shape factor - plastic/elastic
        void Calc_f_z_plel()
        {
            m_ff_z_plel = m_fW_z_pl / m_fW_z_el;
        }


        // Torsional inertia constant
        void Calc_I_t()
        {
            m_fI_t = 2 * MathF.Pow2(m_fW_t_el) / (m_ft * m_fU);
        }
        // Torsional radius of gyration
        void Calc_i_t()
        {
            m_fi_t = MathF.Sqrt(m_fI_t / m_fA);
        }
        // Torsion factor
        void Calc_q_t()
        {
           // m_fq_t
        }
        // Torsional section modulus - elastic
        void Calc_W_t_el()
        {
            m_fW_t_el = 2 * MathF.fPI * m_ft * (m_fr_out_major - m_ft/2.0f) * (m_fr_out_minor - m_ft/2.0f);
        }
        // Torsional section modulus - plastic
        void Calc_W_t_pl()
        {
            //m_fW_t_pl
        }
        // Torsional shape factor plastic/elastic
        void Calc_f_t_plel()
        {
            m_ff_t_plel = m_fW_t_pl / m_fW_t_el;
        }


        // Shear factor
        void Calc_Eta_y_v()
        {
            m_fEta_y_v = 1.00f; // Temp
        }
        // Shear effective area - elastic
        void Calc_A_y_v_el()
        {
            m_fA_y_v_el = 2 * m_ft * m_fI_y / m_fS_y;
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
            m_fEta_z_v = 1.00f; // Temp
        }
        // Shear effective area - elastic
        void Calc_A_z_v_el()
        {
            m_fA_z_v_el = 2 * m_ft * m_fI_z / m_fS_z;
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

		protected override void loadCrScIndices()
		{
            CCrSc_0_26 oTemp = new CCrSc_0_26();
            oTemp.loadCrScIndices_26_28(INoPointsOut, 0);            
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