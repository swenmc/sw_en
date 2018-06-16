using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using M_BASE;

namespace M_EC3
{
    class C___I : CL_55
    {
        // Flange
        public float m_fct_f,
                     m_fLambda_f_1,
                     m_fLambda_f_2,
                     m_fLambda_f_3;
        public int m_iClass_f;

        // Upper Flange
        public float m_fct_fu,
       m_fLambda_fu_1,
       m_fLambda_fu_2,
       m_fLambda_fu_3;
        public int m_iClass_fu;

        // Bottom Flange
        public float
                m_fct_fb,
                m_fLambda_fb_1,
                m_fLambda_fb_2,
                m_fLambda_fb_3;
        public int m_iClass_fb;

        // Web
        public int m_iKlassif_Psi_Fix_Sigma_N;
        public bool m_bKlassif_Eps_Kl3_Sigma_com;
        public float m_fSigma_com_Ed,
                     m_fct_w,
                     m_fAlpha_w,
                     m_fSigma_fyd_1,
                     m_fSigma_fyd_2,
                     m_fPsi_w,
                     m_fLambda_w_1,
                     m_fLambda_w_2,
                     m_fLambda_w_3;
        public int m_iClass_w;

        // Whole Cross-section
        public int m_iClass;


        public C___I(C_GEO___I objGeo, C_MAT___I objMat, C_IFO objIFO, C_NAD objNAD, C_STR___I objStr, ECrScSymmetry1 eSym, ECrScPrType1 eProd)
        {
            if (eSym == ECrScSymmetry1.eDS)
            {
                // Flange
                GetClassTab52_OUT(objStr.m_fSigma_fA,
                                objStr.m_fSigma_fB,
                                objGeo.m_fc_f,
                                objGeo.m_ft_f,
                                objMat.m_fEps_f,
                                m_fct_f,
                                m_fLambda_f_1,
                                m_fLambda_f_2,
                                m_fLambda_f_3,
                                m_iClass_f,
                                objMat.BStainlessS);

                float fF_f = objGeo.m_fb * objGeo.m_ft_f * objMat.m_ff_y_f / objNAD.FGamma_M0;
                float fF_w = objGeo.m_fc_w * objGeo.m_ft_w * objMat.m_ff_y_w / objNAD.FGamma_M0;

                // Web
                GetClassTab52_INT(objStr.m_fSigma_wA,
                                  objStr.m_fSigma_wB,
                                  objStr.m_fSigma_N,
                                  fF_f,
                                  fF_w,
                                  fF_f,
                                  objIFO.FN_Ed,
                                  objGeo.m_fc_w,
                                  objGeo.m_ft_w,
                                  objMat.m_ff_y_w / objNAD.FGamma_M0,
                                  objMat.m_fEps_w,
                                  m_iKlassif_Psi_Fix_Sigma_N,
                                  m_bKlassif_Eps_Kl3_Sigma_com,
                                  m_fSigma_com_Ed,
                                  m_fct_w,
                                  m_fAlpha_w,
                                  m_fSigma_fyd_1,
                                  m_fSigma_fyd_2,
                                  m_fPsi_w,
                                  m_fLambda_w_1,
                                  m_fLambda_w_2,
                                  m_fLambda_w_3,
                                  m_iClass_w,
                                  objMat.BStainlessS);

                m_iClass = Math.Max(m_iClass_f, m_iClass_w);
            }
            else
            {
               // Upper Flange
                GetClassTab52_OUT(objStr.m_fSigma_fuA,
                                objStr.m_fSigma_fuB,
                                objGeo.m_fc_fu,
                                objGeo.m_ft_fu,
                                 objMat.m_fEps_fu,
                                 m_fct_fu,
                                 m_fLambda_fu_1,
                                 m_fLambda_fu_2,
                                 m_fLambda_fu_3,
                                 m_iClass_fu,
                                objMat.BStainlessS);
                // Bottom Flange
                GetClassTab52_OUT(objStr.m_fSigma_fbA,
                objStr.m_fSigma_fbB,
                objGeo.m_fc_fb,
                objGeo.m_ft_fb,
                objMat.m_fEps_fb,
                m_fct_fb,
                m_fLambda_fb_1,
                m_fLambda_fb_2,
                m_fLambda_fb_3,
                m_iClass_fb,
                objMat.BStainlessS);

                float fF_fu = objGeo.m_fb_fu * objGeo.m_ft_fu * objMat.m_ff_y_fu / objNAD.FGamma_M0 + objGeo.m_fr_su * objGeo.m_ft_w * objMat.m_ff_y_w / objNAD.FGamma_M0;
                float fF_w = objGeo.m_fc_w * objGeo.m_ft_w * objMat.m_ff_y_w / objNAD.FGamma_M0;
                float fF_fb = objGeo.m_fb_fb * objGeo.m_ft_fb * objMat.m_ff_y_fb / objNAD.FGamma_M0 + objGeo.m_fr_sb * objGeo.m_ft_w * objMat.m_ff_y_w / objNAD.FGamma_M0;

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

                // Web
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
                                 m_fSigma_com_Ed,
                                 m_fct_w,
                                 m_fAlpha_w,
                                 m_fSigma_fyd_1,
                                 m_fSigma_fyd_2,
                                 m_fPsi_w,
                                 m_fLambda_w_1,
                                 m_fLambda_w_2,
                                 m_fLambda_w_3,
                                 m_iClass_w,
                                 objMat.BStainlessS);

                m_iClass = Math.Max(Math.Max(m_iClass_fu, m_iClass_fb), m_iClass_w);
            }
        }
    }
}
