using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using M_BASE;

namespace M_EC3
{
    // Additional Data of Cross section / resistances, specific settings, etc.
    class C_ADD__HL : CBase
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
                     m_fA_k,
                     m_fM_pl_y,
                     m_fM_pl_z,
                     m_fEta,
                     m_fLambda_rel_LT_0,
                     m_fBeta_LT;

        bool m_bQuerschn_fuer_InterVerf_2_moeglich;

        public C_ADD__HL(C_NAD objNAD, C_GEO__HL objGeo, C_MAT__HL objMat, CCrSc objCrSc, ECrScPrType1 eProd)
        {
            if (eProd != ECrScPrType1.eCrSc_wld && eProd != ECrScPrType1.eCrSc_wldnorm)
            {
                m_ft_V_z = objGeo.m_ft_w;        // (6.20)
                m_ft_V_y = objGeo.m_ft_f;
                m_fN_pl = objCrSc.m_fA * objMat.Ff_y;
                m_fN_u = objCrSc.m_fA * objMat.Ff_u;

                // 6.2.6(3) f)
                m_fA_v_z = objCrSc.m_fA * objGeo.m_fh / (objGeo.m_fb + objGeo.m_fh);
                m_fA_v_y = objCrSc.m_fA * objGeo.m_fb / (objGeo.m_fb + objGeo.m_fh);

                // 6.2.8
                float rW_pl_y_w = 2.0f * sqr(objGeo.m_fh - objGeo.m_ft) * objGeo.m_ft / 4.0f;
                float rW_pl_z_f = 2.0f * sqr(objGeo.m_fb - objGeo.m_ft) * objGeo.m_ft / 4.0f;

                m_fM_pl_y = objCrSc.m_fW_pl_y * objMat.Ff_y;
                m_fM_pl_z = objCrSc.m_fW_pl_z * objMat.Ff_y;


                //bGeschlQuerschnitt = TRUE;

                m_fA_k = (objGeo.m_fb - objGeo.m_ft) * (objGeo.m_fh - objGeo.m_ft);

                // bQuerschn_fuer_InterVerf_2_moeglich = TRUE;
            }
            else
            {
                m_ft_V_z = objGeo.m_ft_w; // (6.20)
                m_ft_V_y = Math.Min(objGeo.m_ft_fu, objGeo.m_ft_fb);
                m_fA_fu = objGeo.m_fb * objGeo.m_ft_fu;
                m_fA_fb = objGeo.m_fb * objGeo.m_ft_fb;
                m_fA_w = 2.0f * objGeo.m_fh_w * objGeo.m_ft_w;
                m_fN_pl = m_fA_fu * objMat.m_ff_y_fu + m_fA_fb * objMat.m_ff_y_fb + m_fA_w * objMat.m_ff_y_w;
                m_fN_u = m_fA_fu * objMat.m_ff_u_fu + m_fA_fb * objMat.m_ff_u_fb + m_fA_w * objMat.m_ff_u_w;

                // 6.2.6(3) d) und e)
                m_fA_v_z = m_fEta * 2.0f * objGeo.m_fh_w * objGeo.m_ft_w;
                m_fA_v_y = objCrSc.m_fA - 2.0f * objGeo.m_fh_w * objGeo.m_ft_w;

                float  rz_pl_0 = 0.5f * (-objGeo.m_fb * objGeo.m_ft_fu * objMat.m_ff_y_fu + objGeo.m_fh_w * 2.0f * objGeo.m_ft_w * objMat.m_ff_y_w + objGeo.m_fb * objGeo.m_ft_fb * objMat.m_ff_y_fb) / (2.0f * objGeo.m_ft_w * objMat.m_ff_y_w) + objGeo.m_ft_fu;
                float rW_pl_y_wh;

                if (rz_pl_0 >=objGeo.m_ft_fu && rz_pl_0 <= objGeo.m_fh - objGeo.m_ft_fb)
                {
                    rW_pl_y_wh = sqr(rz_pl_0 -objGeo.m_ft_fu) * objGeo.m_ft_w + sqr(objGeo.m_fh - rz_pl_0 - objGeo.m_ft_fb) * objGeo.m_ft_w;
                    m_fM_pl_y = objGeo.m_fb *objGeo.m_ft_fu * (rz_pl_0 -objGeo.m_ft_fu / 2.0f) * objMat.m_ff_y_fu +
                              rW_pl_y_wh * objMat.m_ff_y_w +
                              objGeo.m_fb * objGeo.m_ft_fb * (objGeo.m_fh - rz_pl_0 - objGeo.m_ft_fb / 2.0f) * objMat.m_ff_y_fb;
                }
                else if (rz_pl_0 <objGeo.m_ft_fu)
                {
                    rz_pl_0 = 0.5f * (objGeo.m_fb * objGeo.m_ft_fb * objMat.m_ff_y_fb + objGeo.m_fb *objGeo.m_ft_fu * objMat.m_ff_y_fu + objGeo.m_fh_w * 2.0f * objGeo.m_ft_w * objMat.m_ff_y_w) / (objGeo.m_fb * objMat.m_ff_y_fu);
                    rW_pl_y_wh = objGeo.m_fh_w * 2.0f * objGeo.m_ft_w * (objGeo.m_fh_w / 2.0f +objGeo.m_ft_fu - rz_pl_0);
                    m_fM_pl_y = objGeo.m_fb * (sqr(rz_pl_0) / 2.0f + sqr(objGeo.m_ft_fu - rz_pl_0) / 2.0f) * objMat.m_ff_y_fu +
                              rW_pl_y_wh * objMat.m_ff_y_w +
                              objGeo.m_fb * objGeo.m_ft_fb * (objGeo.m_fh - rz_pl_0 - objGeo.m_ft_fb / 2.0f) * objMat.m_ff_y_fb;
                }
                else
                {
                    rz_pl_0 = -0.5f * (objGeo.m_fb *objGeo.m_ft_fu * objMat.m_ff_y_fu + objGeo.m_fh_w * 2.0f * objGeo.m_ft_w * objMat.m_ff_y_w - objGeo.m_fb * objMat.m_ff_y_fb *objGeo.m_ft_fu - objGeo.m_fb * objMat.m_ff_y_fb * objGeo.m_fh_w - objGeo.m_fh * objGeo.m_fb * objMat.m_ff_y_fb) / (objGeo.m_fb * objMat.m_ff_y_fb);
                    rW_pl_y_wh = objGeo.m_fh_w * 2.0f * objGeo.m_ft_w * (rz_pl_0 -objGeo.m_ft_fu - objGeo.m_fh_w / 2.0f);
                    m_fM_pl_y = objGeo.m_fb *objGeo.m_ft_fu * (rz_pl_0 -objGeo.m_ft_fu / 2.0f) * objMat.m_ff_y_fu +
                              rW_pl_y_wh * objMat.m_ff_y_w +
                              objGeo.m_fb * objGeo.m_ft_fb * (sqr(objGeo.m_fh - rz_pl_0) / 2.0f + sqr(rz_pl_0 - objGeo.m_fh_w -objGeo.m_ft_fu) / 2.0f) * objMat.m_ff_y_fb;
                }

                // 6.2.8
                float rW_pl_y_w = m_fEta * rW_pl_y_wh;                                                        // 6.2.6(3) d)

                // 6.2.8
                float rW_pl_z_fo = 2.0f * (objGeo.m_fb / 2.0f * objGeo.m_ft_fu * objGeo.m_fb / 4.0f);
                float rW_pl_z_fu = 2.0f * (objGeo.m_fb / 2.0f * objGeo.m_ft_fb * objGeo.m_fb / 4.0f);
                float rW_pl_z_f = rW_pl_z_fo + rW_pl_z_fu;                                                 // 6.2.6(3) e)

                m_fM_pl_z = objCrSc.m_fW_pl_z * objMat.m_ff_y_w + rW_pl_z_fo * (objMat.m_ff_y_fu - objMat.m_ff_y_w) + rW_pl_z_fu * (objMat.m_ff_y_fb - objMat.m_ff_y_w);

                //bGeschlQuerschnitt = TRUE;

                m_fA_k = (objGeo.m_fb - (objGeo.m_ft_wl + objGeo.m_ft_wr) / 2.0f) * (objGeo.m_fh - (objGeo.m_ft_fu + objGeo.m_ft_fb) / 2.0f);

                // Symetriu urcovat uy po nacitani roymerov a priereyovych charakteristik
                /*
                if (Math.Abs(m_fz_S - objGeo.m_fh / 2.0f) > objGeo.m_fh * 1.0e - 3f)
                    bDoppeltSymmQuerschnitt = FALSE;
                 */

                //bQuerschn_fuer_InterVerf_2_moeglich = TRUE;
            }
        }
    }
}
