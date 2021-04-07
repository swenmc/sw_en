namespace MATERIAL
{
    // Default plastic / polymere / fibreglass material class
    public class CMat_10_00: CMat
    {
        //// Default - plastic / polymere / fibreglass
        //// General material properties

        private float m_fE_11; // Young's modulus in longitudinal (fibre) direction

        public float E_11
        {
            get { return m_fE_11; }
            set { m_fE_11 = value; }
        }

        private float m_fE_22; // Young's modulus in transverse direction

        public float E_22
        {
            get { return m_fE_22; }
            set { m_fE_22 = value; }
        }

        private float m_fG_12; // In-plane shear modulus

        public float G_12
        {
            get { return m_fG_12; }
            set { m_fG_12 = value; }
        }

        private float m_fG_23; // Out-of-plane shear modulus

        public float G_23
        {
            get { return m_fG_23; }
            set { m_fG_23 = value; }
        }

        private float m_fNu_21; // Minor Poisson ratio

        public float Nu_21
        {
            get { return m_fNu_21; }
            set { m_fNu_21 = value; }
        }

        private float m_fSigma_00_t; // Longitudinal tensile strength

        public float Sigma_00_t
        {
            get { return m_fSigma_00_t; }
            set { m_fSigma_00_t = value; }
        }

        private float m_fSigma_00_c; // Longitudinal compressive strength

        public float Sigma_00_c
        {
            get { return m_fSigma_00_c; }
            set { m_fSigma_00_c = value; }
        }

        private float m_fSigma_90_t; // Transverse tensile strength

        public float Sigma_90_t
        {
            get { return m_fSigma_90_t; }
            set { m_fSigma_90_t = value; }
        }

        private float m_fSigma_90_c; // Transverse compressive strength

        public float Sigma_90_c
        {
            get { return m_fSigma_90_c; }
            set { m_fSigma_90_c = value; }
        }

        private float m_fTau; // Shear strength

        public float Tau
        {
            get { return m_fTau; }
            set { m_fTau = value; }
        }

        // Constructor
        public CMat_10_00(string name_temp)
        {
            //Standard = "";
            Name = name_temp; // Default Name
            m_sMatType = 10;

            m_fE = m_fE_11 =   30.90e+9f; // 30.90 [GPa]
            m_fE_22 =          8.30e+9f; //   8.30 [GPa]
            m_fG = m_fG_12 =   2.80e+9f; //   2.80 [GPa]
            m_fG_23 =          3.00e+9f; //   3.00 [GPa]
            m_fNu = m_fNu_21 = 0.0866f; // 0.0866 [-]
            m_fSigma_00_t =  798e+6f; // 798 [MPa]
            m_fSigma_00_c =  480e+6f; // 480 [MPa]
            m_fSigma_90_t =   40e+6f; //  40 [MPa]
            m_fSigma_90_c =  140e+6f; // 140 [MPa]
            m_fTau =          70e+6f; //  70 [MPa]

            m_fRho = 1600; // Default density [kg /m^3]
        }
    }
}
