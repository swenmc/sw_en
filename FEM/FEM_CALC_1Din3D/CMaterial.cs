using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FEM_CALC_1Din3D
{
    public class CMaterial
    {
        public float m_fE  = 2.1e5f;  // Unit [Pa]
        public float m_fnu = 0.3f;    // Unit [-]
        public float m_fG;            // Unit [Pa]
        public float m_fAlpha_T = 1.2e-5f; // Unit 

        public CMaterial()
        {
            m_fG = m_fE / (2f * (1f + m_fnu));
        }
    }
}
