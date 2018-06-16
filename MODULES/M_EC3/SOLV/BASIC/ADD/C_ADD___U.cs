using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using M_BASE;

namespace M_EC3
{
    // Additional Data of Cross section / resistances, specific settings, etc.
    class C_ADD___U:CBase
    {
        public float m_ft_V_z,
                     m_ft_V_y,
                     m_fA_w,
                     m_fA_f,
                     //m_fA_fu,
                     //m_fA_fb,
                     m_fN_pl,
                     m_fN_u,
                     m_fA_v_z,
                     m_fA_v_y,
                     m_fM_pl_y,
                     m_fM_pl_z,
                     //m_fEta,
                     m_fLambda_rel_LT_0,
                     m_fBeta_LT;

                  bool m_bQuerschn_fber_InterVerf_2_moeglich;

                  public C_ADD___U(C_NAD objNAD, C_GEO___U objGeo, C_MAT___U objMat, CCrSc objCrSc, ECrScPrType1 eProd)
        {
        m_ft_V_z = objGeo.m_ft_w;            // (6.20)
        m_ft_V_y = objGeo.m_ft_f;

        m_fA_w = objCrSc.m_fA - 2f * objGeo.m_fb * objGeo.m_ft_f;
        if (eProd != ECrScPrType1.eCrSc_wld && eProd != ECrScPrType1.eCrSc_wldnorm)
          m_fA_w += (objGeo.m_ft_w + objGeo.m_fr) * objGeo.m_ft_f;

        m_fA_f = objCrSc.m_fA - m_fA_w;

        m_fN_pl = m_fA_f * objMat.m_ff_y_f + m_fA_w * objMat.m_ff_y_w;
        m_fN_u  = m_fA_f * objMat.m_ff_u_f + m_fA_w * objMat.m_ff_u_w;

        if (eProd != ECrScPrType1.eCrSc_wld && eProd != ECrScPrType1.eCrSc_wldnorm)
            m_fA_v_z = Math.Max(m_fA_v_z, objCrSc.m_fA - 2f * objGeo.m_fb * objGeo.m_ft_f + (objGeo.m_ft_w + objGeo.m_fr) * objGeo.m_ft_f); //6.2.6(3)a) 

         // 6.2.8
        float rW_pl_y_w;
        if (eProd != ECrScPrType1.eCrSc_wld && eProd != ECrScPrType1.eCrSc_wldnorm)   
          rW_pl_y_w = 2.0f * sqr(0.5f * objGeo.m_fh - 0.5f * objGeo.m_ft_f) * objGeo.m_ft_w / 4.0f +                // 6.2.6(3) b)
                    objGeo.m_fr * 0.5f * objGeo.m_ft_f * (0.5f * objGeo.m_fh - 0.75f * objGeo.m_ft_f) +
                    0.214602f * sqr(objGeo.m_fr) * (0.5f * objGeo.m_fh - objGeo.m_ft_f - 0.77663f * objGeo.m_fr);
        else
        rW_pl_y_w = sqr(objGeo.m_fh - 2.0f * objGeo.m_ft_f) * objGeo.m_ft_w / 4.0f;

        // 6.2.8
        float rW_pl_z_f = 2.0f * sqr(objGeo.m_fb) * objGeo.m_ft_f / 4.0f;

        float rW_pl_y_wh = sqr(objGeo.m_fh_w) * objGeo.m_ft_w / 4.0f;   

            m_fM_pl_y = objCrSc.m_fW_pl_y * objMat.m_ff_y_f + rW_pl_y_wh * (objMat.m_ff_y_w - objMat.m_ff_y_f);

        m_fM_pl_z = objCrSc.m_fW_pl_z * objMat.Ff_y;

        if (objCrSc.m_fz_S == 0.0f)
            objCrSc.m_fz_S = objGeo.m_fh / 2.0f;

        // 6.3.2.3(1)
        if (eProd != ECrScPrType1.eCrSc_wld && eProd != ECrScPrType1.eCrSc_wldnorm)  
        {
            m_fLambda_rel_LT_0 = objNAD.m_fLambda_rel_LT_0_IR;
            m_fBeta_LT = objNAD.m_fBeta_LT_IR;
        }
        else  
        {
            m_fLambda_rel_LT_0 = objNAD.m_fLambda_rel_LT_0_IS;
            m_fBeta_LT = objNAD.m_fBeta_LT_IS;
        }

       // bDoppeltSymmQuerschnitt = FALSE;
      }
    }
}
