using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using M_BASE;

namespace M_EC3
{
    class C_STR__HL
    {
        // Axial Stress due to Axial Force;
        public float m_fSigma_N;

        // Axial Stresses at Flange
        public float m_fSigma_fA;
        public float m_fSigma_fB;

        // Axial Stresses at Upper Flange
        public float m_fSigma_fulA;
        public float m_fSigma_fulB;
        public float m_fSigma_furA;
        public float m_fSigma_furB;
        public float m_fSigma_fuA;
        public float m_fSigma_fuB;

        // Axial Stresses at Bottom Flange
        public float m_fSigma_fblA;
        public float m_fSigma_fblB;
        public float m_fSigma_fbrA;
        public float m_fSigma_fbrB;
        public float m_fSigma_fbA;
        public float m_fSigma_fbB;

        // Axial Stresses at Web
        public float m_fSigma_wA;
        public float m_fSigma_wB;
        public float m_fSigma_wlA;
        public float m_fSigma_wlB;
        public float m_fSigma_wrA;
        public float m_fSigma_wrB;


        public float m_fSigma_com_Ed;


        public C_STR__HL(C_IFO objIFO, C_GEO__HL objGeo, CCrSc objCrSc, ECrScPrType1 eProd)
        {
            m_fSigma_N = objIFO.FN_Ed / objCrSc.m_fA;

            if (eProd != ECrScPrType1.eCrSc_wld && eProd != ECrScPrType1.eCrSc_wldnorm)
            {
                float fSigma_My_f = objIFO.FM_y_Ed / objCrSc.m_fI_y * objGeo.m_fh / 2.0f;
                float fSigma_Mz_f = objIFO.FM_z_Ed / objCrSc.m_fI_z * objGeo.m_fc_f / 2.0f;
                float fSigma_My_w = objIFO.FM_y_Ed / objCrSc.m_fI_y * objGeo.m_fc_w / 2.0f;
                float fSigma_Mz_w = objIFO.FM_z_Ed / objCrSc.m_fI_z * objGeo.m_fb / 2.0f;

                // Upper Flange
                m_fSigma_fuA = m_fSigma_N - fSigma_My_f + fSigma_Mz_f;
                m_fSigma_fuB = m_fSigma_N - fSigma_My_f - fSigma_Mz_f;

                // Bottom Flange
                m_fSigma_fbA = m_fSigma_N + fSigma_My_f + fSigma_Mz_f;
                m_fSigma_fbB = m_fSigma_N + fSigma_My_f - fSigma_Mz_f;

                // Flanges
                m_fSigma_fA = Math.Min(m_fSigma_fuA, m_fSigma_fbA);
                m_fSigma_fB = Math.Min(m_fSigma_fuB, m_fSigma_fbB);

                // Left Web
                m_fSigma_wlA = m_fSigma_N - fSigma_My_w + fSigma_Mz_w;
                m_fSigma_wlB = m_fSigma_N + fSigma_My_w + fSigma_Mz_w;

                // Right Web
                m_fSigma_wrA = m_fSigma_N - fSigma_My_w - fSigma_Mz_w;
                m_fSigma_wrB = m_fSigma_N + fSigma_My_w - fSigma_Mz_w;

                // Webs
                m_fSigma_wA = Math.Min(m_fSigma_wlA, m_fSigma_wrA);
                m_fSigma_wB = Math.Min(m_fSigma_wlB, m_fSigma_wrB);
            }
            else
            {
                float fSigma_My_fu = objIFO.FM_y_Ed / objCrSc.m_fI_y * objCrSc.m_fz_S;
                float fSigma_My_fb = objIFO.FM_y_Ed / objCrSc.m_fI_y * (objGeo.m_fh - objCrSc.m_fz_S);
                float fSigma_My_wu = objIFO.FM_y_Ed / objCrSc.m_fI_y * (objCrSc.m_fz_S - objGeo.m_ft_fu);
                float fSigma_My_wb = objIFO.FM_y_Ed / objCrSc.m_fI_y * (objGeo.m_fh - objCrSc.m_fz_S - objGeo.m_ft_fb);
                float fSigma_Mz_w = objIFO.FM_z_Ed / objCrSc.m_fI_z * objGeo.m_fb / 2.0f;
                float fSigma_Mz_f = objIFO.FM_z_Ed / objCrSc.m_fI_z * (objGeo.m_fb / 2.0f - objGeo.m_ft_w);

                // Upper Flange
                m_fSigma_fuA = m_fSigma_N - fSigma_My_fu + fSigma_Mz_f;
                m_fSigma_fuB = m_fSigma_N - fSigma_My_fu - fSigma_Mz_f;

                // Bottom Flange
                m_fSigma_fbA = m_fSigma_N + fSigma_My_fb + fSigma_Mz_f;
                m_fSigma_fbB = m_fSigma_N + fSigma_My_fb - fSigma_Mz_f;

                // Flanges
                m_fSigma_fA = Math.Min(m_fSigma_fuA, m_fSigma_fbA);
                m_fSigma_fB = Math.Min(m_fSigma_fuB, m_fSigma_fbB);

                // Left Web
                m_fSigma_wlA = m_fSigma_N - fSigma_My_wu + fSigma_Mz_w;
                m_fSigma_wlB = m_fSigma_N + fSigma_My_wb + fSigma_Mz_w;

                // Right Web
                m_fSigma_wrA = m_fSigma_N - fSigma_My_wu - fSigma_Mz_w;
                m_fSigma_wrB = m_fSigma_N + fSigma_My_wb - fSigma_Mz_w;

                // Webs
                m_fSigma_wA = Math.Min(m_fSigma_wlA, m_fSigma_wrA);
                m_fSigma_wB = Math.Min(m_fSigma_wlB, m_fSigma_wrB);
            }

            // 5.5.2(9)
            m_fSigma_com_Ed = Math.Max(Math.Abs(Math.Min(m_fSigma_fA, m_fSigma_fB)), 0.0f);
        }
    }
}
