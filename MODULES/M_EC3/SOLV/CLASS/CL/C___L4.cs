using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using M_BASE;

namespace M_EC3
{
    class C___L4 : CL_EF
    {
        public float m_fA_eff;
        //public float m_fA_f_eff;
        //public float m_fA_w_eff;
        public float m_fN_pl_eff;
        //public float m_fN_u_eff;
        public float m_fe_Ny;
        public float m_fe_Nz;
        public float m_fI_eff_y;
        public float m_fI_eff_z;
        public float m_fI_eff_yy;
        public float m_fI_eff_zz;
        public float m_fI_eff_yz;
        public float m_fW_eff_y_min;
        public float m_fW_eff_z_min;


        // Leg a - vertical for asymmetrical cross-section
        float fSigma_1_a,
              fSigma_2_a,
              fPsi_a,
              fk_Sigma_a,
              fLambda_rel_p_a,
              fRho_a,
              fb_eff_a,
              fb_red_a,
              fb_em_a;

        // Leg b - horizontal for asymmetrical cross-section
        float fSigma_1_b,
              fSigma_2_b,
              fPsi_b,
              fk_Sigma_b,
              fLambda_rel_p_b,
              fRho_b,
              fb_eff_b,
              fb_red_b,
              fb_em_b;

        float fz_eff_c,
              fy_eff_c;


        public C___L4(CCrSc objCrSc, C_GEO___L objGeo, C_IFO objIFO, C_STR___L objStr, C___L objC__L, C_MAT___L objMat, C_ADD___L objAdd, ECrScSymmetry1 eSym)
        {
            //  EN 1993-1-5,Table 4.2
            GetEff_OUT(objStr.m_fSP_Sigma_x_Ed[2],
                                    Math.Min(objStr.m_fSP_Sigma_x_Ed[0], objStr.m_fSP_Sigma_x_Ed[1]),
                                    objGeo.m_fc_a,
                                    objGeo.m_ft_a,
                                    objMat.FEps,
                                    fSigma_1_a,
                                    fSigma_2_a,
                                    fPsi_a,
                                    fk_Sigma_a,
                                    fLambda_rel_p_a,
                                    fRho_a,
                                    fb_eff_a,
                                    fb_red_a,
                                    fb_em_a,
                                    objMat.BStainlessS);

            // EN 1993-1-5,Table 4.2
            GetEff_OUT(objStr.m_fSP_Sigma_x_Ed[4],
                                    Math.Min(objStr.m_fSP_Sigma_x_Ed[5], objStr.m_fSP_Sigma_x_Ed[6]),
                                    objGeo.m_fc_b,
                                    objGeo.m_ft_b,
                                    objMat.FEps,
                                    fSigma_1_b,
                                    fSigma_2_b,
                                    fPsi_b,
                                    fk_Sigma_b,
                                    fLambda_rel_p_b,
                                    fRho_b,
                                    fb_eff_b,
                                    fb_red_b,
                                    fb_em_b,
                                    objMat.BStainlessS);

            if (fb_red_a > 0.0f || fb_red_b > 0.0f)
            {
                m_fA_eff = objCrSc.m_fA - fb_red_a * objGeo.m_ft_a - fb_red_b * objGeo.m_ft_b;
                fz_eff_c = (objCrSc.m_fA * objCrSc.m_fz_S -
                            fb_red_a * objGeo.m_ft_a * (objGeo.m_fh - fb_em_a) -
                            fb_red_b * objGeo.m_ft_b * objGeo.m_ft_b / 2.0f
                           ) / m_fA_eff;

                fy_eff_c = (objCrSc.m_fA * objCrSc.m_fy_S -
                            fb_red_a * objGeo.m_ft_a * objGeo.m_ft_a / 2.0f -
                            fb_red_b * objGeo.m_ft_b * (objGeo.m_fb - fb_em_b)
                           ) / m_fA_eff;

                m_fI_eff_yy = objCrSc.m_fI_yy +
                            objCrSc.m_fA * sqr(fz_eff_c - objCrSc.m_fz_S) -
                            objGeo.m_ft_a * (float)Math.Pow(fb_red_a, 3) / 12.0f -
                            fb_red_a * objGeo.m_ft_a * sqr(objGeo.m_fh - fb_em_a - fz_eff_c) -
                            fb_red_b * (float)Math.Pow(objGeo.m_ft_b, 3) / 12.0f -
                            fb_red_b * objGeo.m_ft_b * sqr(fz_eff_c - objGeo.m_ft_b / 2.0f);

                m_fI_eff_zz = objCrSc.m_fI_zz +
                            objCrSc.m_fA * sqr(fy_eff_c - objCrSc.m_fy_S) -
                            (float)Math.Pow(objGeo.m_ft_a, 3) * fb_red_a / 12.0f -
                            fb_red_a * objGeo.m_ft_a * sqr(fy_eff_c - objGeo.m_ft_a / 2.0f) -
                            (float)Math.Pow(fb_red_b, 3) * objGeo.m_ft_b / 12.0f -
                            fb_red_b * objGeo.m_ft_b * sqr(objGeo.m_fb - fb_em_b - fy_eff_c);

                m_fI_eff_yz = objCrSc.m_fI_yz + objCrSc.m_fA * (objCrSc.m_fy_S - fy_eff_c) * (fz_eff_c - objCrSc.m_fz_S) -
                            fb_red_a * objGeo.m_ft_a * (fy_eff_c - objGeo.m_ft_a / 2.0f) * (objGeo.m_fh - fb_em_a - fz_eff_c) -
                            fb_red_b * objGeo.m_ft_b * (fz_eff_c - objGeo.m_ft_b / 2.0f) * (objGeo.m_fb - fb_em_b - fy_eff_c);

                m_fI_eff_y = (m_fI_eff_yy + m_fI_eff_zz) / 2.0f + (float)Math.Sqrt(sqr((m_fI_eff_yy - m_fI_eff_zz) / 2.0f) + sqr(m_fI_eff_yz));
                m_fI_eff_z = (m_fI_eff_yy + m_fI_eff_zz) / 2.0f - (float)Math.Sqrt(sqr((m_fI_eff_yy - m_fI_eff_zz) / 2.0f) + sqr(m_fI_eff_yz));

                if (m_fI_eff_yy != m_fI_eff_zz)
                    objGeo.m_fAlpha_Axis = 0.5f * (float)Math.Atan((2.0f * m_fI_eff_yz) / (m_fI_eff_zz - m_fI_eff_yy));
                else
                    objGeo.m_fAlpha_Axis = -0.785398163f; // = -45?

                float m_fe_Ny_zz = fz_eff_c - objCrSc.m_fz_S;
                float m_fe_Nz_yy = fy_eff_c - objCrSc.m_fy_S;
                m_fe_Ny = m_fe_Nz_yy * (float)Math.Cos(objGeo.m_fAlpha_Axis) + m_fe_Ny_zz * (float)Math.Sin(objGeo.m_fAlpha_Axis);
                m_fe_Nz = -m_fe_Nz_yy * (float)Math.Sin(objGeo.m_fAlpha_Axis) + m_fe_Ny_zz * (float)Math.Cos(objGeo.m_fAlpha_Axis);

                objStr.m_fSP_y_par[0] = -fy_eff_c;
                objStr.m_fSP_z_par[0] = fz_eff_c - objGeo.m_fh + fb_red_a;
                objStr.m_fSP_y_par[1] = -fy_eff_c + objGeo.m_ft_a;
                objStr.m_fSP_z_par[1] = fz_eff_c - objGeo.m_fh + fb_red_a;
                objStr.m_fSP_y_par[2] = -fy_eff_c;
                objStr.m_fSP_z_par[2] = fz_eff_c - objGeo.m_ft_b - objGeo.m_fr;

                objStr.m_fSP_y_par[3] = -fy_eff_c;
                objStr.m_fSP_z_par[3] = fz_eff_c;

                objStr.m_fSP_y_par[4] = -fy_eff_c + objGeo.m_ft_a + objGeo.m_fr;
                objStr.m_fSP_z_par[4] = fz_eff_c;
                objStr.m_fSP_y_par[5] = -fy_eff_c + objGeo.m_fb - fb_red_b;
                objStr.m_fSP_z_par[5] = fz_eff_c;
                objStr.m_fSP_y_par[6] = -fy_eff_c + objGeo.m_fb - fb_red_b;
                objStr.m_fSP_z_par[6] = fz_eff_c - objGeo.m_ft_b;

                float rz_max = 0.0f;
                float ry_max = 0.0f;

                for (int iSP = 0; iSP < 7; iSP++)
                {
                    objStr.m_fSP_y[iSP] = objStr.m_fSP_y_par[iSP] * (float)Math.Cos(objGeo.m_fAlpha_Axis) + objStr.m_fSP_z_par[iSP] * (float)Math.Sin(objGeo.m_fAlpha_Axis);
                    objStr.m_fSP_z[iSP] = -objStr.m_fSP_y_par[iSP] * (float)Math.Sin(objGeo.m_fAlpha_Axis) + objStr.m_fSP_z_par[iSP] * (float)Math.Cos(objGeo.m_fAlpha_Axis);

                    rz_max = Math.Max(rz_max, Math.Abs(objStr.m_fSP_z[iSP]));
                    ry_max = Math.Max(ry_max, Math.Abs(objStr.m_fSP_y[iSP]));
                }

                m_fW_eff_y_min = m_fI_eff_y / rz_max;
                m_fW_eff_z_min = m_fI_eff_z / ry_max;
            }
            else
            {
                m_fA_eff = objCrSc.m_fA;

                fz_eff_c = objCrSc.m_fz_S;
                fy_eff_c = objCrSc.m_fy_S;

                m_fI_eff_y = objCrSc.m_fI_y;
                m_fI_eff_z = objCrSc.m_fI_z;

                m_fW_eff_y_min = objCrSc.m_fW_el_y_min;
                m_fW_eff_z_min = objCrSc.m_fW_el_z_min;

                m_fe_Ny = 0.0f;
                m_fe_Nz = 0.0f;
            }

            m_fN_pl_eff = m_fA_eff * objMat.Ff_y;
        }
    }
}
