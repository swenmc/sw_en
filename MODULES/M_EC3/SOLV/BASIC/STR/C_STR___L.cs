using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using M_BASE;

namespace M_EC3
{
    class C_STR___L
    {
        // Axial Stress due to Axial Force;
        //public float m_fSigma_N;

        // Moment of Area - Statical Moment

        public float[] m_fSP_y_par = new float[7];
        public float[] m_fSP_z_par = new float[7];

        public float[] m_fSP_y = new float[7];
        public float[] m_fSP_z = new float[7];

        // Array of Axial Stresses in Stress Points
        public float [] m_fSP_Sigma_x_Ed = new float [7];

        // Minimum Axial Stress in Section
        public float m_fSigma_x_Ed_min;

        public C_STR___L(C_IFO objIFO, C_GEO___L objGeo, CCrSc objCrSc, ECrScSymmetry1 eSym)
        {
            m_fSP_y_par[0] = -objCrSc.m_fy_S;
            m_fSP_z_par[0] = objCrSc.m_fz_S - objGeo.m_fh;
            m_fSP_y_par[1] = -objCrSc.m_fy_S + objGeo.m_ft_a;
            m_fSP_z_par[1] = objCrSc.m_fz_S - objGeo.m_fh;
            m_fSP_y_par[2] = -objCrSc.m_fy_S;
            m_fSP_z_par[2] = objCrSc.m_fz_S - objGeo.m_ft_b - objGeo.m_fr;

            m_fSP_y_par[3] = -objCrSc.m_fy_S;
            m_fSP_z_par[3] = objCrSc.m_fz_S;

            m_fSP_y_par[4] = -objCrSc.m_fy_S + objGeo.m_ft_a + objGeo.m_fr;
            m_fSP_z_par[4] = objCrSc.m_fz_S;
            m_fSP_y_par[5] = -objCrSc.m_fy_S + objGeo.m_fb;
            m_fSP_z_par[5] = objCrSc.m_fz_S;
            m_fSP_y_par[6] = -objCrSc.m_fy_S + objGeo.m_fb;
            m_fSP_z_par[6] = objCrSc.m_fz_S - objGeo.m_ft_b;

            float m_fSigma_x_N_Ed = objIFO.FN_Ed / objCrSc.m_fA;

            float m_fSigma_x_Ed_min = float.MaxValue;

            for (int iSP = 0; iSP < 7; iSP++)
            {

                m_fSP_y[iSP] = m_fSP_y_par[iSP] * (float)Math.Cos(objGeo.m_fAlpha_Axis) + m_fSP_z_par[iSP] * (float)Math.Sin(objGeo.m_fAlpha_Axis);
                m_fSP_z[iSP] = -m_fSP_y_par[iSP] * (float)Math.Sin(objGeo.m_fAlpha_Axis) + m_fSP_z_par[iSP] * (float)Math.Cos(objGeo.m_fAlpha_Axis);


                float m_fSP_Sigma_x_My_Ed = objIFO.FM_y_Ed / objCrSc.m_fI_y * m_fSP_z[iSP];
                float m_fSP_Sigma_x_Mz_Ed = -objIFO.FM_z_Ed / objCrSc.m_fI_z * m_fSP_y[iSP];

                m_fSP_Sigma_x_Ed[iSP] = m_fSigma_x_N_Ed + m_fSP_Sigma_x_My_Ed + m_fSP_Sigma_x_Mz_Ed;

                m_fSigma_x_Ed_min = Math.Min(m_fSigma_x_Ed_min, m_fSP_Sigma_x_Ed[iSP]);
            }
        }
    }
}
