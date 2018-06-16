using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using M_BASE;

namespace M_EC3
{
    class C_STR___I
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

        public float m_fSigma_com_Ed;


        public C_STR___I(C_IFO objIFO, C_GEO___I objGeo, CCrSc objCrSc, ECrScSymmetry1 eSym)
        {
            m_fSigma_N = objIFO.FN_Ed / objCrSc.m_fA;

            if (eSym == ECrScSymmetry1.eDS)
            {
                float fSigma_My_f = objIFO.FM_y_Ed / objCrSc.m_fI_y * objGeo.m_fh / 2.0f;
                float fSigma_My_w = objIFO.FM_y_Ed / objCrSc.m_fI_y * objGeo.m_fc_w / 2.0f;

                float fSigma_Mz_fA = objIFO.FM_z_Ed / objCrSc.FI_z * (objGeo.m_ft_w / 2.0f + objGeo.m_fr);
                float fSigma_Mz_fB = objIFO.FM_z_Ed / objCrSc.FI_z * objGeo.m_fb / 2.0f;

                m_fSigma_fulA = m_fSigma_N - fSigma_My_f + fSigma_Mz_fA;
                m_fSigma_fulB = m_fSigma_N - fSigma_My_f + fSigma_Mz_fB;
                m_fSigma_furA = m_fSigma_N - fSigma_My_f - fSigma_Mz_fA;
                m_fSigma_furB = m_fSigma_N - fSigma_My_f - fSigma_Mz_fB;

                m_fSigma_fblA = m_fSigma_N + fSigma_My_f + fSigma_Mz_fA;
                m_fSigma_fblB = m_fSigma_N + fSigma_My_f + fSigma_Mz_fB;
                m_fSigma_fbrA = m_fSigma_N + fSigma_My_f - fSigma_Mz_fA;
                m_fSigma_fbrB = m_fSigma_N + fSigma_My_f - fSigma_Mz_fB;

                float fSigma_fuA = Math.Min(m_fSigma_fulA, m_fSigma_furA);
                float fSigma_fuB = Math.Min(m_fSigma_fulB, m_fSigma_furB);

                float fSigma_fbA = Math.Min(m_fSigma_fblA, m_fSigma_fbrA);
                float fSigma_fbB = Math.Min(m_fSigma_fblB, m_fSigma_fbrB);

                // Flange
                m_fSigma_fA = Math.Min(fSigma_fuA, fSigma_fbA);
                m_fSigma_fB = Math.Min(fSigma_fuB, fSigma_fbB);

                // Web
                m_fSigma_wA = m_fSigma_N - fSigma_My_f;
                m_fSigma_wB = m_fSigma_N + fSigma_My_f;
            }
            else
            {
                float fSigma_My_fu = objIFO.FM_y_Ed / objCrSc.m_fI_y * objCrSc.m_fz_S;
                float fSigma_My_fb = objIFO.FM_y_Ed / objCrSc.m_fI_y * (objGeo.m_fh - objCrSc.m_fz_S);

                float fSigma_My_wu = objIFO.FM_y_Ed / objCrSc.m_fI_y * (objCrSc.m_fz_S - objGeo.m_ft_fu - objGeo.m_fr_su);
                float fSigma_My_wb = objIFO.FM_y_Ed / objCrSc.m_fI_y * (objGeo.m_fh - objCrSc.m_fz_S - objGeo.m_ft_fb - objGeo.m_fr_sb);

                float fSigma_Mz_fuA = objIFO.FM_z_Ed / objCrSc.FI_z * (objGeo.m_ft_w / 2.0f + objGeo.m_fr_su);
                float fSigma_Mz_fuB = objIFO.FM_z_Ed / objCrSc.FI_z * objGeo.m_fb_fu / 2.0f;

                float fSigma_Mz_fbA = objIFO.FM_z_Ed / objCrSc.FI_z * (objGeo.m_ft_w / 2.0f + objGeo.m_fr_sb);
                float fSigma_Mz_fbB = objIFO.FM_z_Ed / objCrSc.FI_z * objGeo.m_fb_fb / 2.0f;

                // Upper Flange
                m_fSigma_fulA = m_fSigma_N - fSigma_My_fu + fSigma_Mz_fuA;
                m_fSigma_fulB = m_fSigma_N - fSigma_My_fu + fSigma_Mz_fuB;
                m_fSigma_furA = m_fSigma_N - fSigma_My_fu - fSigma_Mz_fuA;
                m_fSigma_furB = m_fSigma_N - fSigma_My_fu - fSigma_Mz_fuB;
                m_fSigma_fuA = Math.Min(m_fSigma_fulA, m_fSigma_furA);
                m_fSigma_fuB = Math.Min(m_fSigma_fulB, m_fSigma_furB);

                // Bottom Flange
                m_fSigma_fblA = m_fSigma_N + fSigma_My_fb + fSigma_Mz_fbA;
                m_fSigma_fblB = m_fSigma_N + fSigma_My_fb + fSigma_Mz_fbB;
                m_fSigma_fbrA = m_fSigma_N + fSigma_My_fb - fSigma_Mz_fbA;
                m_fSigma_fbrB = m_fSigma_N + fSigma_My_fb - fSigma_Mz_fbB;
                m_fSigma_fbA = Math.Min(m_fSigma_fblA, m_fSigma_fbrA);
                m_fSigma_fbB = Math.Min(m_fSigma_fblB, m_fSigma_fbrB);

                // Web
                m_fSigma_wA = m_fSigma_N - fSigma_My_fb;
                m_fSigma_wB = m_fSigma_N + fSigma_My_fb;
            }

            // 5.5.2(9)
            m_fSigma_com_Ed = Math.Max(Math.Abs(Math.Min(m_fSigma_fuB, m_fSigma_fbB)), 0.0f);
        }
    }
}
