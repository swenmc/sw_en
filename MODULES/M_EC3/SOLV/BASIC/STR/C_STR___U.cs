using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using M_BASE;

namespace M_EC3
{
    class C_STR___U
    {
        // Axial Stress due to Axial Force;
        public float m_fSigma_N;

        // Axial Stresses at Flange
        public float m_fSigma_fA;
        public float m_fSigma_fB;

        // Axial Stresses at Upper Flange
        public float m_fSigma_fuA;
        public float m_fSigma_fuB;

        // Axial Stresses at Bottom Flange
        public float m_fSigma_fbA;
        public float m_fSigma_fbB;

        // Axial Stresses at Web
        public float m_fSigma_wA;
        public float m_fSigma_wB;

        public float m_fSigma_com_Ed;


        public C_STR___U(C_IFO objIFO, C_GEO___U objGeo, CCrSc objCrSc)
        {
            m_fSigma_N = objIFO.FN_Ed / objCrSc.m_fA;

            float fSigma_My_f = objIFO.FM_y_Ed / objCrSc.m_fI_y * objGeo.m_fh / 2.0f;
            float fSigma_My_w = objIFO.FM_y_Ed / objCrSc.m_fI_y * objGeo.m_fc_w / 2.0f;
            float fSigma_Mz_fA = objIFO.FM_z_Ed / objCrSc.m_fI_z * (objCrSc.m_fy_S - objGeo.m_ft_w - objGeo.m_fr);
            float fSigma_Mz_fB = objIFO.FM_z_Ed / objCrSc.m_fI_z * (objCrSc.m_fy_S - objGeo.m_fb);
            float fSigma_Mz_wa = objIFO.FM_z_Ed / objCrSc.m_fI_z * objCrSc.m_fy_S;
            float fSigma_Mz_wi = objIFO.FM_z_Ed / objCrSc.m_fI_z * (objCrSc.m_fy_S - objGeo.m_ft_w);

            // Flanges
            m_fSigma_fuA = m_fSigma_N - fSigma_My_f + fSigma_Mz_fA;
            m_fSigma_fuB = m_fSigma_N - fSigma_My_f + fSigma_Mz_fB;
            m_fSigma_fbA = m_fSigma_N + fSigma_My_f + fSigma_Mz_fA;
            m_fSigma_fbB = m_fSigma_N + fSigma_My_f + fSigma_Mz_fB;

            m_fSigma_fA = Math.Min(m_fSigma_fuA, m_fSigma_fbA);
            m_fSigma_fB = Math.Min(m_fSigma_fuB, m_fSigma_fbB);

            // Web
            m_fSigma_wA = m_fSigma_N - fSigma_My_w + Math.Min(fSigma_Mz_wa, fSigma_Mz_wi);
            m_fSigma_wB = m_fSigma_N + fSigma_My_w + Math.Min(fSigma_Mz_wa, fSigma_Mz_wi);

            // 5.5.2(9)
            m_fSigma_com_Ed = Math.Max(Math.Abs(Math.Min(m_fSigma_fA, m_fSigma_fB)), 0.0f);
        }
    }
}
