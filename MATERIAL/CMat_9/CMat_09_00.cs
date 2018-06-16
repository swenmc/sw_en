using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MATERIAL
{
    // Default aluminium material class
    public class CMat_09_00:CMat_00
    {
        // Default - aluminium
        // General material properties

        // Design properties
        public float m_ff_o_0;
        public float m_ff_u_0;

        // Constructor
        public CMat_09_00()
        {
            m_sMatType = 9;
            m_fE = 0.7e5f;   // Unit [Pa]
            m_fNu = 0.3f;    // Unit [-]
            m_fG = m_fE / (2f * (1f + m_fNu)); // Unit [Pa]
            m_fAlpha_T = 2.3e-5f; // Unit [1/Celsius degree]
            m_fRho = 2700f;       // Unit [kg/m^3]
        }
    }
}
