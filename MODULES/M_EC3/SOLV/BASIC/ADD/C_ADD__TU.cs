using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using M_BASE;

namespace M_EC3
{
    // Additional Data of Cross section / resistances, specific settings, etc.
    class C_ADD__TU:CBase
    {
        public float m_ft_V_z,
                     m_ft_V_y,
                     m_fA_w,
                     m_fA_f,
                     m_fN_pl,
                     m_fN_u,
                     m_fA_v_z,
                     m_fA_v_y,
                     m_fM_pl_y,
                     m_fM_pl_z,
                     m_fA_k;

        bool m_bQuerschn_fuer_InterVerf_2_moeglich;

        public C_ADD__TU(C_NAD objNAD, C_GEO__TU objGeo, C_MAT__TU objMat, CCrSc objCrSc, ECrScPrType1 eProd)
        {
            m_ft_V_z = objGeo.m_ft;            // (6.20)
            m_ft_V_y = objGeo.m_ft;

            m_fN_pl = objCrSc.m_fA * objMat.Ff_y;
            m_fN_u = objCrSc.m_fA * objMat.Ff_u;

            // 6.2.6(3) g)
            m_fA_v_z = 2.0f * objCrSc.m_fA / (float)Math.PI;
            m_fA_w = m_fA_f = m_fA_v_y = m_fA_v_z;

            // Plastische Momente
            m_fM_pl_y = objCrSc.m_fW_pl_y * objMat.Ff_y;
            m_fM_pl_z = objCrSc.m_fW_pl_z * objMat.Ff_y;

            /*
            bGeschlQuerschnitt = TRUE;

            bSchubbeulen = FALSE;
             */

            m_fA_k = (float)Math.PI * sqr((objGeo.m_fd - objGeo.m_ft) / 2.0f);
        }
    }
}
