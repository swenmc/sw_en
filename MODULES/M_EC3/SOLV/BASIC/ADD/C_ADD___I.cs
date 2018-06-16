using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using M_BASE;

namespace M_EC3
{
    // Additional Data of Cross section / resistances, specific settings, etc.
    class C_ADD___I:CBase
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

        public C_ADD___I(C_NAD objNAD, C_GEO___I objGeo, C_MAT___I objMat, CCrSc objCrSc, ECrScSymmetry1 eSym, ECrScPrType1 eProd)
        {
            if (eSym == ECrScSymmetry1.eDS)
            {
                m_ft_V_z = objGeo.m_ft_w;        //    (6.20)
                m_ft_V_y = objGeo.m_ft_f;

                m_fA_w = objCrSc.m_fA - 2.0f * objGeo.m_fb * objGeo.m_ft_f;
                if (eProd == ECrScPrType1.eCrSc_rold)
                    m_fA_w += (objGeo.m_ft_w + 2.0f * objGeo.m_fr) * objGeo.m_ft_f;

                m_fA_f = objCrSc.m_fA - m_fA_w;

                m_fN_pl = m_fA_f * objMat.m_ff_y_f + m_fA_w * objMat.m_ff_y_w;
                m_fN_u = m_fA_f * objMat.m_ff_u_f + m_fA_w * objMat.m_ff_u_w;

                if (eProd == ECrScPrType1.eCrSc_rold)  
                {
                    m_fA_v_z = Math.Max(m_fA_v_z, m_fEta * objGeo.m_fh_w * objGeo.m_ft_w); //6.2.6(3)a) 
                }
                else                                             
                {
                    m_fA_v_z = m_fEta * objGeo.m_fh_w * objGeo.m_ft_w;           // 6.2.6(3)d)
                }

                //6.2.8
                float rW_pl_y_w;
                if (eProd == ECrScPrType1.eCrSc_rold)    
                    rW_pl_y_w = 2.0f * sqr(0.5f * objGeo.m_fh - 0.5f * objGeo.m_ft_f) * objGeo.m_ft_w / 4.0f +                  // acc. to 6.2.6(3) a)
                                2.0f * objGeo.m_fr * 0.5f * objGeo.m_ft_f * (0.5f * objGeo.m_fh - 0.75f * objGeo.m_ft_f) +
                                2.0f * 0.214602f * sqr(objGeo.m_fr) * (0.5f * objGeo.m_fh - objGeo.m_ft_f - 0.77663f * objGeo.m_fr);
                else
                    rW_pl_y_w = sqr(objGeo.m_fh - 2.0f * objGeo.m_ft_f) * objGeo.m_ft_w / 4.0f;                   // acc. to 6.2.6(3) d)

                // 6.2.8
                float rW_pl_z_f = 2.0f * sqr(objGeo.m_fb) * objGeo.m_ft_f / 4.0f;

                
                float rW_pl_y_wh = sqr(objGeo.m_fh_w) * objGeo.m_ft_w / 4.0f;


                m_fM_pl_y = objCrSc.m_fW_pl_y * objMat.m_ff_y_f + rW_pl_y_wh * (objMat.m_ff_y_w - objMat.m_ff_y_f);

                m_fM_pl_z = objCrSc.m_fW_pl_z * objMat.m_ff_y_w + rW_pl_z_f * (objMat.m_ff_y_f - objMat.m_ff_y_w);

                // 6.3.2.3(1)
                if (eProd == ECrScPrType1.eCrSc_rold)  
                {
                    m_fLambda_rel_LT_0 = objNAD.m_fLambda_rel_LT_0_IR;
                    m_fBeta_LT = objNAD.m_fBeta_LT_IR;
                }
                else                                        
                {
                    m_fLambda_rel_LT_0 = objNAD.m_fLambda_rel_LT_0_IS;
                    m_fBeta_LT = objNAD.m_fBeta_LT_IS;
                }

                if (objCrSc.m_fz_S == 0.0f)
                    objCrSc.m_fz_S = objGeo.m_fh / 2.0f;

                m_bQuerschn_fuer_InterVerf_2_moeglich = true;      
            }
            else
            {
                m_ft_V_z = objGeo.m_ft_w; // (6.20)
                m_ft_V_y = Math.Min(objGeo.m_ft_fu, objGeo.m_ft_fb);

                m_fA_fu = objGeo.m_fb_fu * objGeo.m_ft_fu;
                m_fA_fb = objGeo.m_fb_fb * objGeo.m_ft_fb;
                m_fA_f = Math.Min(m_fA_fu, m_fA_fb); // Temporary
                m_fA_w = objGeo.m_fh_w * objGeo.m_ft_w;

                m_fN_pl = m_fA_fu * objMat.m_ff_y_fu + m_fA_fb * objMat.m_ff_y_fb + m_fA_w * objMat.m_ff_y_w;
                m_fN_u = m_fA_fu * objMat.m_ff_u_fu + m_fA_fb * objMat.m_ff_u_fb + m_fA_w * objMat.m_ff_u_w;

                // 6.2.6(3) d) and e)
                m_fA_v_z = m_fEta * objGeo.m_fh_w * objGeo.m_ft_w;
                m_fA_v_y = objCrSc.m_fA - objGeo.m_fh_w * objGeo.m_ft_w;

                float rz_pl_0 = 0.5f * (-objGeo.m_fb_fu * objGeo.m_ft_fu * objMat.m_ff_y_fu + objGeo.m_fh_w * objGeo.m_ft_w * objMat.m_ff_y_w + objGeo.m_fb_fb * objGeo.m_ft_fb * objMat.m_ff_y_fb) / (objGeo.m_ft_w * objMat.m_ff_y_w) + objGeo.m_ft_fu;
                float rW_pl_y_wh;

                if (rz_pl_0 >= objGeo.m_ft_fu && rz_pl_0 <= objGeo.m_fh - objGeo.m_ft_fb) 
                {
                    rW_pl_y_wh = sqr(rz_pl_0 - objGeo.m_ft_fu) * objGeo.m_ft_w / 2.0f + sqr(objGeo.m_fh - rz_pl_0 - objGeo.m_ft_fb) * objGeo.m_ft_w / 2.0f;
                    m_fM_pl_y = objGeo.m_fb_fu * objGeo.m_ft_fu * (rz_pl_0 - objGeo.m_ft_fu / 2.0f) * objMat.m_ff_y_fu +                           
                              rW_pl_y_wh * objMat.m_ff_y_w +                                                          
                              objGeo.m_fb_fb * objGeo.m_ft_fb * (objGeo.m_fh - rz_pl_0 - objGeo.m_ft_fb / 2.0f) * objMat.m_ff_y_fb;                       
                }
                else if (rz_pl_0 < objGeo.m_ft_fu)                 
                {
                    rz_pl_0 = 0.5f * (objGeo.m_fb_fb * objGeo.m_ft_fb * objMat.m_ff_y_fb + objGeo.m_fb_fu * objGeo.m_ft_fu * objMat.m_ff_y_fu + objGeo.m_fh_w * objGeo.m_ft_w * objMat.m_ff_y_w) / (objGeo.m_fb_fu * objMat.m_ff_y_fu);
                    rW_pl_y_wh = objGeo.m_fh_w * objGeo.m_ft_w * (objGeo.m_fh_w / 2.0f + objGeo.m_ft_fu - rz_pl_0);
                    m_fM_pl_y = objGeo.m_fb_fu * (sqr(rz_pl_0) / 2.0f + sqr(objGeo.m_ft_fu - rz_pl_0) / 2.0f) * objMat.m_ff_y_fu +           
                              rW_pl_y_wh * objMat.m_ff_y_w +                                                             
                              objGeo.m_fb_fb * objGeo.m_ft_fb * (objGeo.m_fh - rz_pl_0 - objGeo.m_ft_fb / 2.0f) * objMat.m_ff_y_fb;                          
                }
                else                               
                {
                    rz_pl_0 = -0.5f * (objGeo.m_fb_fu * objGeo.m_ft_fu * objMat.m_ff_y_fu + objGeo.m_fh_w * objGeo.m_ft_w * objMat.m_ff_y_w - objGeo.m_fb_fb * objMat.m_ff_y_fb * objGeo.m_ft_fu - objGeo.m_fb_fb * objMat.m_ff_y_fb * objGeo.m_fh_w - objGeo.m_fh * objGeo.m_fb_fb * objMat.m_ff_y_fb) / (objGeo.m_fb_fb * objMat.m_ff_y_fb);
                    rW_pl_y_wh = objGeo.m_fh_w * objGeo.m_ft_w * (rz_pl_0 - objGeo.m_ft_fu - objGeo.m_fh_w / 2.0f);
                    m_fM_pl_y = objGeo.m_fb_fu * objGeo.m_ft_fu * (rz_pl_0 - objGeo.m_ft_fu / 2.0f) * objMat.m_ff_y_fu +                                       
                              rW_pl_y_wh * objMat.m_ff_y_w +                                                                      
                              objGeo.m_fb_fb * objGeo.m_ft_fb * (sqr(objGeo.m_fh - rz_pl_0) / 2.0f + sqr(rz_pl_0 - objGeo.m_fh_w - objGeo.m_ft_fu) / 2.0f) * objMat.m_ff_y_fb;
                }

                float rW_pl_y_w = m_fEta * rW_pl_y_wh;                                                        // Acc. to 6.2.6(3) d)


                float rW_pl_z_fo = 2.0f * (objGeo.m_fb_fu / 2.0f * objGeo.m_ft_fu * objGeo.m_fb_fu / 4.0f);
                float rW_pl_z_fu = 2.0f * (objGeo.m_fb_fb / 2.0f * objGeo.m_ft_fb * objGeo.m_fb_fb / 4.0f);
                float rW_pl_z_f = rW_pl_z_fo + rW_pl_z_fu;                                                 // Acc. to 6.2.6(3) e)


                m_fM_pl_z = objCrSc.m_fW_pl_z * objMat.m_ff_y_w + rW_pl_z_fo * (objMat.m_ff_y_fu - objMat.m_ff_y_w) + rW_pl_z_fu * (objMat.m_ff_y_fb - objMat.m_ff_y_w);

                // 6.3.2.3(1)
                m_fLambda_rel_LT_0 = objNAD.m_fLambda_rel_LT_0_IS;
                m_fBeta_LT = objNAD.m_fBeta_LT_IS;

                objGeo.m_fb = Math.Min(objGeo.m_fb_fu, objGeo.m_fb_fb);                           // Tab. 6.4 resp. 6.5

                // Symmetriu musim urcovat skor - uz pri vytvoreni tried Geo a CrSc
                /*
                if (Math.Abs(objCrSc.m_fz_S - objGeo.m_fh / 2.0f) > objGeo.m_fh * 1.0e-3f)
                    bDoppeltSymmQuerschnitt = FALSE; 
                 */

                m_bQuerschn_fuer_InterVerf_2_moeglich = true;
            }
        }
    }
}
