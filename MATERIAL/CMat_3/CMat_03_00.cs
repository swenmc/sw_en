using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BaseClasses;

namespace MATERIAL
{
    // Default steel material class
    public class CMat_03_00:CMat
    {
        // Default - steel
        // General material properties

        // Design properties
        public float m_ff_yk_0;
        public float m_ff_u_0;

        // Constructor
        public CMat_03_00()
        {
            Name = "Steel S235"; // Default Name
            m_sMatType = 3;
            m_fE = 2.1e11f;   // Unit [Pa]
            m_fNu = 0.3f;    // Unit [-]
            m_fG = m_fE / (2f * (1f + m_fNu)); // Unit [Pa]
            m_fAlpha_T = 1.2e-5f; // Unit [1/Celsius degree]
            m_ff_yk_0 = 2.35e+8f;
            m_ff_u_0 = 3.60e+8f;
        }

        public CMat_03_00(string name_temp, float fy, float fu)
        {
            Name = name_temp; // Default Name
            m_sMatType = 3;
            m_fE = 2.1e11f;   // Unit [Pa]
            m_fNu = 0.3f;    // Unit [-]
            m_fG = m_fE / (2f * (1f + m_fNu)); // Unit [Pa]
            m_fAlpha_T = 1.2e-5f; // Unit [1/Celsius degree]
            m_ff_yk_0 = fy;
            m_ff_u_0 = fu;
        }

    }
}
