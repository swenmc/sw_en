using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using M_BASE;

namespace M_EC3
{
    // Additional Data of Cross section / resistances, specific settings, etc.
    class C_ADD___L:CBase
    {
        public float m_ft_V_z,
                     m_ft_V_y,
                     m_fA_w,
                     m_fA_f,
                     m_fA_fu,
                     m_fA_fb,
                     m_fN_pl,
                     m_fN_u,
                     m_fA_v_z,
                     m_fA_v_y,
                     m_fM_pl_y,
                     m_fM_pl_z,
                     m_fEta,
                     m_fLambda_rel_LT_0,
                     m_fBeta_LT;

                  bool m_bQuerschn_fuer_InterVerf_2_moeglich;

                  public C_ADD___L(C_NAD objNAD, C_GEO___L objGeo, C_MAT___L objMat, CCrSc objCrSc, ECrScSymmetry1 eSym, ECrScPrType1 eProd)
        {

        m_ft_V_z = objGeo.m_ft_a;   // ???????
        m_ft_V_y = objGeo.m_ft_b;   // ???????

        m_fN_pl = objCrSc.m_fA * objMat.Ff_y;
        m_fN_u = objCrSc.m_fA * objMat.Ff_u;
        m_fM_pl_y = objCrSc.m_fW_pl_y * objMat.Ff_y;
        m_fM_pl_z = objCrSc.m_fW_pl_z * objMat.Ff_y;

        if (eProd == ECrScPrType1.eCrSc_rold || eProd == ECrScPrType1.eCrSc_cldfrm) 
        {
            m_fLambda_rel_LT_0 = objNAD.m_fLambda_rel_LT_0_IR;
            m_fBeta_LT = objNAD.m_fBeta_LT_IR;
        }
        else                                         
        {
            m_fLambda_rel_LT_0 = objNAD.m_fLambda_rel_LT_0_IS;
            m_fBeta_LT = objNAD.m_fBeta_LT_IS;
        }

        /*
        bSchubbeulen = FALSE;

        bDoppeltSymmQuerschnitt = FALSE;   
        bEinfachSymmQuerschnitt = FALSE;
        */
        }
    }
}
