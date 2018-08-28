using System;

namespace MATERIAL
{
    [Serializable]
    public class CMat
    {
        // Predecessor of materials
        // Predok pre jednotlive materialy
        private int m_iMat_ID;

        public int IMat_ID
        {
            get { return m_iMat_ID; }
            set { m_iMat_ID = value; }
        }

        // Name of material
        private string m_sName;

        public string Name
        {
            get { return m_sName; }
            set { m_sName = value; }
        }

        // Default material - steel
        public short m_sMatType = 3;
        public float m_fE = 2.1e11f;       // Unit [Pa]
        public float m_fNu = 0.3f;         // Unit [-]
        public float m_fG;                 // Unit [Pa]
        public float m_fAlpha_T = 1.2e-5f; // Unit [1/Celsius degree]
        public float m_fRho = 7850f;       // Unit [kg/m^3]

        //--------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------------------
        // Constructor
        public CMat()
        {
            Name = "Steel S500"; // Temporary default
            m_fG = m_fE / (2f * (1f + m_fNu));
        }

        // User defined material
        public CMat(float fE, float fNu, float fAlpha_T, float fRho)
        {
            Name = "Steel S500"; // Temporary default
            m_fE = fE;
            m_fG = m_fE / (2f * (1f + m_fNu));
            m_fNu = fNu;
            m_fAlpha_T = fAlpha_T;
            m_fRho = fRho;
        }

        public CMat(short sMatType, float fE, float fNu, float fAlpha_T, float fRho)
        {
            Name = "Steel S500"; // Temporary default
            m_sMatType = sMatType;
            m_fE = fE;
            m_fG = m_fE / (2f * (1f + m_fNu));
            m_fNu = fNu;
            m_fAlpha_T = fAlpha_T;
            m_fRho = fRho;
        }
    }
}
