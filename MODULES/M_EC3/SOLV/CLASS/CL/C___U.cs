using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace M_EC3
{
    class C___U : CL_55
    {

        // Flange
        public float m_fct_f,
                     m_fLambda_f_1,
                     m_fLambda_f_2,
                     m_fLambda_f_3;
        public int m_iClass_f;

        // Upper Flange
        public int m_iClass_fu;
        
        // Bottom Flange
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

        public C___U(C_GEO___U objGeo, C_MAT___U objMat, C_IFO objIFO, C_NAD objNAD, C_STR___U objStr)
        {
            // Table 5.2
            GetClassTab52_OUT(objStr.m_fSigma_fuA,
                            objStr.m_fSigma_fuB,
                            objGeo.m_fc_f,
                            objGeo.m_ft_f,
                            objMat.m_fEps_f,
                            m_fct_f,
                            m_fLambda_f_1,
                            m_fLambda_f_2,
                            m_fLambda_f_3,
                            m_iClass_fu,
                            objMat.BStainlessS);

            GetClassTab52_OUT(objStr.m_fSigma_fbA,
                            objStr.m_fSigma_fbB,
                            objGeo.m_fc_f,
                            objGeo.m_ft_f,
                            objMat.m_fEps_f,
                            m_fct_f,
                            m_fLambda_f_1,
                            m_fLambda_f_2,
                            m_fLambda_f_3,
                            m_iClass_fb,
                            objMat.BStainlessS);

            m_iClass_f = Math.Max(m_iClass_fu, m_iClass_fb);

            float fF_f = objGeo.m_fb * objGeo.m_ft_f * objMat.m_ff_y_f / objNAD.FGamma_M0;
            float fF_w = objGeo.m_fc_w * objGeo.m_ft_w * objMat.m_ff_y_w / objNAD.FGamma_M0;

            // Table 5.2
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
                             objStr.m_fSigma_com_Ed,
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
    }
}
