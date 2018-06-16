using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using M_BASE;

namespace M_EC3
{
    class C__HL : CL_55
    {
        // Flange
        public float  
            m_fct_f,
       m_fAlpha_f,
       m_fSigma_fyd_f_1,
       m_fSigma_fyd_f_2,
       m_fPsi_f,
       m_fLambda_f_1,
       m_fLambda_f_2,
       m_fLambda_f_3;
        public int m_iClass_f;

        // Upper Flange
        public float 
       m_fct_fu,
       m_fAlpha_fu,
       m_fSigma_fyd_fu_1,
       m_fSigma_fyd_fu_2,
       m_fPsi_fu,
       m_fLambda_fu_1,
       m_fLambda_fu_2,
       m_fLambda_fu_3;
        public int m_iClass_fu;

        // Bottom Flange
        public float m_fct_fb,
                m_fAlpha_fb,
                m_fSigma_fyd_fb_1,
                m_fSigma_fyd_fb_2,
                m_fPsi_fb,
                m_fLambda_fb_1,
                m_fLambda_fb_2,
                m_fLambda_fb_3;
        public int m_iClass_fb;

        // Web
        public int m_iKlassif_Psi_Fix_Sigma_N;
        public bool m_bKlassif_Eps_Kl3_Sigma_com;
        public float m_fct_w,
                        m_fAlpha_w,
                        m_fSigma_fyd_w_1,
                        m_fSigma_fyd_w_2,
                        m_fPsi_w,
                        m_fLambda_w_1,
                        m_fLambda_w_2,
                        m_fLambda_w_3;
        public int m_iClass_w;
        public float m_fSigma_com_Ed;

        // Whole Cross-section
        public int m_iClass;


        public C__HL(C_GEO__HL objGeo, C_MAT__HL objMat, C_IFO objIFO, C_NAD objNAD, C_STR__HL objStr, ECrScSymmetry1 eSym, ECrScPrType1 eProd)
        {
            if (eProd != ECrScPrType1.eCrSc_wld && eProd != ECrScPrType1.eCrSc_wldnorm)
            {
                float fF_f = 2.0f * objGeo.m_fc_f * objGeo.m_ft * objMat.Ff_y / objNAD.FGamma_M0;
                float fF_w = objGeo.m_fh * objGeo.m_ft * objMat.Ff_y / objNAD.FGamma_M0;

                //Table 5.2
                GetClassTab52_INT(objStr.m_fSigma_fuA,
                                 objStr.m_fSigma_fuB,
                                 objStr.m_fSigma_N,
                                 fF_w,
                                 fF_f,
                                 fF_w,
                                 objIFO.FN_Ed,
                                 objGeo.m_fc_f,
                                 objGeo.m_ft_fu,
                                 objMat.m_ff_y_fu / objNAD.FGamma_M0,
                                 objMat.m_fEps_f,
                                 m_iKlassif_Psi_Fix_Sigma_N,
                                 m_bKlassif_Eps_Kl3_Sigma_com,
                                 objStr.m_fSigma_com_Ed,
                                 m_fct_fu,
                                 m_fAlpha_fu,
                                 m_fSigma_fyd_fu_1,
                                 m_fSigma_fyd_fu_2,
                                 m_fPsi_fu,
                                 m_fLambda_fu_1,
                                 m_fLambda_fu_2,
                                 m_fLambda_fu_3,
                                 m_iClass_fu,
                                 objMat.BStainlessS);

                //Table 5.2
                GetClassTab52_INT(objStr.m_fSigma_fbA,
                                 objStr.m_fSigma_fbB,
                                 objStr.m_fSigma_N,
                                 fF_w,
                                 fF_f,
                                 fF_w,
                                 objIFO.FN_Ed,
                                 objGeo.m_fc_f,
                                 objGeo.m_ft_fb,
                                 objMat.m_ff_y_fb / objNAD.FGamma_M0,
                                 objMat.m_fEps_f,
                                 m_iKlassif_Psi_Fix_Sigma_N,
                                 m_bKlassif_Eps_Kl3_Sigma_com,
                                 objStr.m_fSigma_com_Ed,
                                 m_fct_fb,
                                 m_fAlpha_fb,
                                 m_fSigma_fyd_fb_1,
                                 m_fSigma_fyd_fb_2,
                                 m_fPsi_fb,
                                 m_fLambda_fb_1,
                                 m_fLambda_fb_2,
                                 m_fLambda_fb_3,
                                 m_iClass_fb,
                                 objMat.BStainlessS);



                fF_f = objGeo.m_fb * objGeo.m_ft * objMat.Ff_y / objNAD.FGamma_M0;
                fF_w = 2.0f * objGeo.m_fc_w * objGeo.m_ft * objMat.Ff_y / objNAD.FGamma_M0;

                //Table 5.2
                GetClassTab52_INT(objStr.m_fSigma_wA,
                                 objStr.m_fSigma_wB,
                                 objStr.m_fSigma_N,
                                 fF_f,
                                 fF_w,
                                 fF_f,
                                 objIFO.FN_Ed,
                                 objGeo.m_fc_w,
                                 objGeo.m_ft,
                                 objMat.m_ff_y_w / objNAD.FGamma_M0,
                                 objMat.m_fEps_w,
                                 m_iKlassif_Psi_Fix_Sigma_N,
                                 m_bKlassif_Eps_Kl3_Sigma_com,
                                 objStr.m_fSigma_com_Ed,
                                 m_fct_w,
                                 m_fAlpha_w,
                                 m_fSigma_fyd_w_1,
                                 m_fSigma_fyd_w_2,
                                 m_fPsi_w,
                                 m_fLambda_w_1,
                                 m_fLambda_w_2,
                                 m_fLambda_w_3,
                                 m_iClass_w,
                                 objMat.BStainlessS);

                m_iClass_f = Math.Max(m_iClass_fu, m_iClass_fb);
                m_iClass = Math.Max(m_iClass_f, m_iClass_w);

                if (m_iClass_fu >= m_iClass_fb)
                {
                    objStr.m_fSigma_fA = objStr.m_fSigma_fuA;
                    objStr.m_fSigma_fB = objStr.m_fSigma_fuB;
                    m_fAlpha_f = m_fAlpha_fu;
                    m_fSigma_fyd_f_1 = m_fSigma_fyd_fu_1;
                    m_fSigma_fyd_f_2 = m_fSigma_fyd_fu_2;
                    m_fPsi_f = m_fPsi_fu;
                    objMat.m_fEps_f = objMat.m_fEps_fu;
                    m_fLambda_f_1 = m_fLambda_fu_1;
                    m_fLambda_f_2 = m_fLambda_fu_2;
                    m_fLambda_f_3 = m_fLambda_fu_3;
                    m_fct_f = m_fct_fu;
                    m_iClass_f = m_iClass_fu;
                }
                else
                {
                    objStr.m_fSigma_fA = objStr.m_fSigma_fbA;
                    objStr.m_fSigma_fB = objStr.m_fSigma_fbB;
                    m_fAlpha_f = m_fAlpha_fb;
                    m_fSigma_fyd_f_1 = m_fSigma_fyd_fb_1;
                    m_fSigma_fyd_f_2 = m_fSigma_fyd_fb_2;
                    m_fPsi_f = m_fPsi_fb;
                    objMat.m_fEps_f = objMat.m_fEps_fb;
                    m_fLambda_f_1 = m_fLambda_fb_1;
                    m_fLambda_f_2 = m_fLambda_fb_2;
                    m_fLambda_f_3 = m_fLambda_fb_3;
                    m_fct_f = m_fct_fb;
                    m_iClass_f = m_iClass_fb;
                }
            }
            else
            {
                float fF_f = objGeo.m_fc_f * (objGeo.m_ft_fu * objMat.m_ff_y_fu + objGeo.m_ft_fb * objMat.m_ff_y_fb) / objNAD.FGamma_M0;
                float fF_w = objGeo.m_fh * objGeo.m_ft_w * objMat.m_ff_y_w / objNAD.FGamma_M0;

                //Table 5.2
                GetClassTab52_INT(objStr.m_fSigma_fuA,
                                 objStr.m_fSigma_fuB,
                                 objStr.m_fSigma_N,
                                 fF_w,
                                 fF_f,
                                 fF_w,
                                 objIFO.FN_Ed,
                                 objGeo.m_fc_f,
                                 objGeo.m_ft_fu,
                                 objMat.m_ff_y_fu / objNAD.FGamma_M0,
                                 objMat.m_fEps_fu,
                                 m_iKlassif_Psi_Fix_Sigma_N,
                                 m_bKlassif_Eps_Kl3_Sigma_com,
                                 objStr.m_fSigma_com_Ed,
                                 m_fct_fu,
                                 m_fAlpha_fu,
                                 m_fSigma_fyd_fu_1,
                                 m_fSigma_fyd_fu_2,
                                 m_fPsi_fu,
                                 m_fLambda_fu_1,
                                 m_fLambda_fu_2,
                                 m_fLambda_fu_3,
                                 m_iClass_fu,
                                 objMat.BStainlessS);

                //Table 5.2
                GetClassTab52_INT(objStr.m_fSigma_fbA,
                                 objStr.m_fSigma_fbB,
                                 objStr.m_fSigma_N,
                                 fF_w,
                                 fF_f,
                                 fF_w,
                                 objIFO.FN_Ed,
                                 objGeo.m_fc_fb,
                                 objGeo.m_ft_fb,
                                 objMat.m_ff_y_fb / objNAD.FGamma_M0,
                                 objMat.m_fEps_fb,
                                 m_iKlassif_Psi_Fix_Sigma_N,
                                 m_bKlassif_Eps_Kl3_Sigma_com,
                                 objStr.m_fSigma_com_Ed,
                                 m_fct_fb,
                                 m_fAlpha_fb,
                                 m_fSigma_fyd_fb_1,
                                 m_fSigma_fyd_fb_2,
                                 m_fPsi_fb,
                                 m_fLambda_fb_1,
                                 m_fLambda_fb_2,
                                 m_fLambda_fb_3,
                                 m_iClass_fb,
                                 objMat.BStainlessS);

                float fF_fu = objGeo.m_fb * objGeo.m_ft_fu * objMat.m_ff_y_fu / objNAD.FGamma_M0;
                fF_w = 2.0f * objGeo.m_fc_w * objGeo.m_ft_w * objMat.m_ff_y_w / objNAD.FGamma_M0;
                float fF_fb = objGeo.m_fb * objGeo.m_ft_fb * objMat.m_ff_y_fb / objNAD.FGamma_M0;

                float fF_f_com, fF_f_ten;

                if (objIFO.FM_y_Ed >= 0.0f)
                {
                    fF_f_com = fF_fu;
                    fF_f_ten = fF_fb;
                }
                else
                {
                    fF_f_com = fF_fb;
                    fF_f_ten = fF_fu;
                }

                //Table 5.2
                GetClassTab52_INT(objStr.m_fSigma_wA,
                                 objStr.m_fSigma_wB,
                                 objStr.m_fSigma_N,
                                 fF_f_com,
                                 fF_w,
                                 fF_f_ten,
                                 objIFO.FN_Ed,
                                 objGeo.m_fc_w,
                                 objGeo.m_ft_w,
                                 objMat.m_ff_y_w / objNAD.FGamma_M0,
                                 objMat.m_fEps_w,
                                 m_iKlassif_Psi_Fix_Sigma_N,
                                 m_bKlassif_Eps_Kl3_Sigma_com,
                                 objStr.m_fSigma_com_Ed,
                                 m_fct_w,
                                 m_fAlpha_w,
                                 m_fSigma_fyd_w_1,
                                 m_fSigma_fyd_w_2,
                                 m_fPsi_w,
                                 m_fLambda_w_1,
                                 m_fLambda_w_2,
                                 m_fLambda_w_3,
                                 m_iClass_w,
                                 objMat.BStainlessS);

                m_iClass = Math.Max(m_iClass_fu, m_iClass_fb);
                m_iClass = Math.Max(m_iClass, m_iClass_w);
            }
        }
    }
}