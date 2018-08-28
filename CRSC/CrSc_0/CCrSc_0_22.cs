using MATH;
using System;

namespace CRSC
{
    // Test cross-section class
    // Temporary Class - includes array of drawing points of cross-section in its coordinate system (LCS-for 2D yz)
    public class CCrSc_0_22 : CCrSc
    {
        // Tube / Ring / Annulus / Rura

        //----------------------------------------------------------------------------
        private float m_fd;   // Diameter/ Priemer
        private float m_ft;   // Thickness/ Hrubka
        //private short m_iNoPoints; // Number of Cross-section Points for Drawing in One Circle
        //----------------------------------------------------------------------------

        public float Fd
        {
            get { return m_fd; }
            set { m_fd = value; }
        }

        public float Ft
        {
            get { return m_ft; }
            set { m_ft = value; }
        }

        /*
        public short INoPoints
        {
            get { return m_iNoPoints; }
            set { m_iNoPoints = value; }
        }
        */

        // Auxiliary variables

        float m_fd_in;

        public float Fd_in
        {
            get { return m_fd_in; }
            set { m_fd_in = value; }
        }
        float m_fr_out;

        public float Fr_out
        {
            get { return m_fr_out; }
            set { m_fr_out = value; }
        }
        float m_fr_in;

        public float Fr_in
        {
            get { return m_fr_in; }
            set { m_fr_in = value; }
        }

        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        public CCrSc_0_22()  {   }

        public CCrSc_0_22(float fd, float ft, short iNoPoints)
        {
            INoPointsIn = INoPointsOut = iNoPoints; // vykreslujeme ako n-uholnik, pocet bodov n
            m_fd = fd;
            m_ft = ft;

            m_fd_in = m_fd - 2 * m_ft;

            // Radii
            m_fr_out = m_fd / 2f;
            m_fr_in = m_fd_in / 2f;

            if (iNoPoints < 2 || m_fr_in == m_fr_out)
                return;

            // Create Array - allocate memory
            CrScPointsOut = new float[INoPointsOut, 2];
            CrScPointsIn = new float[INoPointsIn, 2];

            // Fill Array Data
            CalcCrSc_Coord();

            // Fill list of indices for drawing of surface - triangles edges
            loadCrScIndices();
        }
        public CCrSc_0_22(float fd, float ft)
        {
            INoPointsIn = INoPointsOut = 72; // vykreslujeme ako n-uholnik, pocet bodov n
            m_fd = fd;
            m_ft = ft;

            m_fd_in = m_fd - 2 * m_ft;

            // Radii
            m_fr_out = m_fd / 2f;
            m_fr_in = m_fd_in / 2f;

            if (m_fr_in == m_fr_out)
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

            // Outside Points Coordinates
            CrScPointsOut = Geom2D.GetCirclePointCoord_CW(m_fr_out, INoPointsOut);

            // Inside Points
            CrScPointsIn = Geom2D.GetCirclePointCoord_CW(m_fr_in, INoPointsIn);
        }

                
        //----------------------------------------------------------------------------
        // Cross-section properties
        //----------------------------------------------------------------------------

        float m_fU, m_fA,
            m_fW_t_el, m_fI_t, m_fW_t_pl, m_ff_t_plel,
            m_fS, m_fI, m_fW_el, m_fW_pl, m_ff_plel,
            m_fEta_v, m_fA_v_el, m_fA_v_pl, m_ff_v_plel;

        // Perimeter of section
        void Calc_U()
        {
            m_fU = MathF.fPI * m_fd;
        }
        // Section area
        void Calc_A()
        {
            m_fA = MathF.fPI * (MathF.Pow2(m_fd) - MathF.Pow2(m_fd - 2*m_ft)) / 4f;
        }
        // Torsional inertia constant
        void Calc_I_t()
        {
            m_fI_t = 2  * (MathF.fPI * (MathF.Pow4(m_fd) - MathF.Pow4(m_fd - 2 * m_ft)) / 64f);
        }
        // Torsional section modulus - elastic
        void Calc_W_t_el()
        {
            m_fW_t_el = 2 * (MathF.fPI * (MathF.Pow3(m_fd) - MathF.Pow3(m_fd - 2 * m_ft)) / 32f);
        }
        // Torsional section modulus - plastic
        void Calc_W_t_pl()
        {
            m_fW_t_pl = MathF.fPI * MathF.Pow3(m_fd) / 12f;
        }
        // Torsional shape factor plastic/elastic
        void Calc_f_t_plel()
        {
            m_ff_t_plel = m_fW_t_pl / m_fW_t_el;
        }
        // First moment o area
        void Calc_S()
        {
            m_fS = (MathF.Pow3(m_fd) - MathF.Pow3(m_fd - 2 * m_ft)) / 12f;
        }
        // Second moment of area
        void Calc_I()
        {
            m_fI = MathF.fPI * (MathF.Pow4(m_fd) - MathF.Pow4(m_fd-2*m_ft)) / 64f;
        }
        // Section modulus - elastic
        void Calc_W_el()
        {
            m_fW_el = MathF.fPI * (MathF.Pow3(m_fd) - MathF.Pow3(m_fd - 2 * m_ft)) / 32f;
        }
        // Section modulus - plastic
        void Calc_W_pl()
        {
            m_fW_pl = MathF.Pow3(m_fd) / 6f;
        }
        // Shape factor - plastic/elastic
        void Calc_f_plel()
        {
            m_ff_plel = m_fW_pl / m_fW_el;
        }
        // Shear factor
        void Calc_Eta_v()
        {
            float fx = (m_fd - 2 * m_ft) / m_fd;
            m_fEta_v = -1.9235f* MathF.Pow3(fx) + 2.6215f*MathF.Pow2(fx) + 0.1109f * fx + 1.1773f;
        }
        // Shear effective area - elastic
        void Calc_A_v_el()
        {
            m_fA_v_el = 2 * m_fA / MathF.fPI;
        }
        // Shape factor for shear - plastic/elastic
        void Calc_f_v_plel()
        {
            m_ff_v_plel = 1.00f;
        }
        // Shear effective area - plastic
        void Calc_A_v_pl()
        {
            //?????  
            m_fA_v_pl = m_ff_v_plel * m_fA_v_el;
        }

        protected override void loadCrScIndices()
        {
            // Triangle Indices CRSC No 26 and 28
            CCrSc_0_26 oTemp = new CCrSc_0_26();
            oTemp.loadCrScIndices_26_28(INoPointsOut,0);            
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