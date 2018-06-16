using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using M_BASE;

namespace M_EC3
{
    class C___I4 : CL_EF
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
        public float m_fW_eff_y_min;
        public float m_fW_eff_z_min;

        public C___I4(CCrSc objCrSc, C_GEO___I objGeo, C_IFO objIFO, C_STR___I objStr, C___I objC__I, C_MAT___I objMat, C_ADD___I objAdd, ECrScSymmetry1 eSym)
        {
            if (eSym == ECrScSymmetry1.eDS)
                GetCrSc4_ID(objCrSc, objGeo, objIFO, objStr, objC__I, objMat, objAdd);
            else
                GetCrSc4_IM(objCrSc, objGeo, objIFO, objStr, objC__I, objMat, objAdd);
        }

        // Flanges
        // Upper Flange
        float fSigma_1_ful,
              fSigma_2_ful,
              fPsi_ful,
              fk_Sigma_ful,
              fLambda_rel_p_ful,
              fRho_ful,
              fb_eff_ful,
              fb_red_ful,
              fb_em_ful,

              fSigma_1_fur,
              fSigma_2_fur,
              fPsi_fur,
              fk_Sigma_fur,
              fLambda_rel_p_fur,
              fRho_fur,
              fb_eff_fur,
              fb_red_fur,
              fb_em_fur,

              // Bottom Flange

              fSigma_1_fbl,
              fSigma_2_fbl,
              fPsi_fbl,
              fk_Sigma_fbl,
              fLambda_rel_p_fbl,
              fRho_fbl,
              fb_eff_fbl,
              fb_red_fbl,
              fb_em_bl,

              fSigma_1_fbr,
              fSigma_2_fbr,
              fPsi_fbr,
              fk_Sigma_fbr,
              fLambda_rel_p_fbr,
              fRho_fbr,
              fb_eff_fbr,
              fb_red_fbr,
              fb_em_br;
        // Web
        float fSigma_1_w,
              fSigma_2_w,
              fPsi_w,
              fk_Sigma_w,
              fLambda_rel_p_w,
              fRho_w,
              fb_eff_w,
              fb_e1_w,
              fb_e2_w,
              fb_red_w,
              fb_em_w;

        float fA_eff_f,
              fz_eff_c_f,
              fy_eff_c_f,
              fI_eff_y_f,
              fI_eff_z_f;

        // Web
        float fz_eff_c,
              fy_eff_c;

        void GetCrSc4_ID(CCrSc objCrSc, C_GEO___I objGeo, C_IFO objIFO, C_STR___I objStr, C___I objC__I, C_MAT___I objMat, C_ADD___I objAdd)
        {
            if (objC__I.m_iClass_f == 4)
            {
                // EN 1993-1-5, Table 4.2
                GetEff_OUT(objStr.m_fSigma_fulA,
                                      objStr.m_fSigma_fulB,
                                      objGeo.m_fc_f,
                                      objGeo.m_ft_f,
                                      objMat.m_fEps_f,
                                      fSigma_1_ful,
                                      fSigma_2_ful,
                                      fPsi_ful,
                                      fk_Sigma_ful,
                                      fLambda_rel_p_ful,
                                      fRho_ful,
                                      fb_eff_ful,
                                      fb_red_ful,
                                      fb_em_ful,
                                      objMat.BStainlessS);

                // EN 1993-1-5, Table 4.2
                GetEff_OUT(objStr.m_fSigma_furA,
                                      objStr.m_fSigma_furB,
                                      objGeo.m_fc_f,
                                      objGeo.m_ft_f,
                                      objMat.m_fEps_f,
                                      fSigma_1_fur,
                                      fSigma_2_fur,
                                      fPsi_fur,
                                      fk_Sigma_fur,
                                      fLambda_rel_p_fur,
                                      fRho_fur,
                                      fb_eff_fur,
                                      fb_red_fur,
                                      fb_em_fur,
                                      objMat.BStainlessS);

                // EN 1993-1-5, Table 4.2
                GetEff_OUT(objStr.m_fSigma_fblA,
                                      objStr.m_fSigma_fblB,
                                      objGeo.m_fc_f,
                                      objGeo.m_ft_f,
                                      objMat.m_fEps_f,
                                      fSigma_1_fbl,
                                      fSigma_2_fbl,
                                      fPsi_fbl,
                                      fk_Sigma_fbl,
                                      fLambda_rel_p_fbl,
                                      fRho_fbl,
                                      fb_eff_fbl,
                                      fb_red_fbl,
                                      fb_em_bl,
                                      objMat.BStainlessS);

                // EN 1993-1-5, Table 4.2
                GetEff_OUT(objStr.m_fSigma_fbrA,
                                      objStr.m_fSigma_fbrB,
                                      objGeo.m_fc_f,
                                      objGeo.m_ft_f,
                                      objMat.m_fEps_f,
                                      fSigma_1_fbr,
                                      fSigma_2_fbr,
                                      fPsi_fbr,
                                      fk_Sigma_fbr,
                                      fLambda_rel_p_fbr,
                                      fRho_fbr,
                                      fb_eff_fbr,
                                      fb_red_fbr,
                                      fb_em_br,
                                      objMat.BStainlessS);

                fA_eff_f = objCrSc.m_fA - (fb_red_ful + fb_red_fur + fb_red_fbl + fb_red_fbr) * objGeo.m_ft_f;

                fz_eff_c_f = (objCrSc.m_fA * objGeo.m_fh / 2.0f -
                            fb_red_ful * objGeo.m_ft_f * objGeo.m_ft_f / 2.0f -
                            fb_red_fur * objGeo.m_ft_f * objGeo.m_ft_f / 2.0f -
                            fb_red_fbl * objGeo.m_ft_f * (objGeo.m_fh - objGeo.m_ft_f / 2.0f) -
                            fb_red_fbr * objGeo.m_ft_f * (objGeo.m_fh - objGeo.m_ft_f / 2.0f)
                           ) / fA_eff_f;

                fy_eff_c_f = (objCrSc.m_fA * objGeo.m_fb / 2.0f -
                            fb_red_ful * objGeo.m_ft_f * fb_em_ful -
                            fb_red_fur * objGeo.m_ft_f * (objGeo.m_fb - fb_em_fur) -
                            fb_red_fbl * objGeo.m_ft_f * fb_em_bl -
                            fb_red_fbr * objGeo.m_ft_f * (objGeo.m_fb - fb_em_br)
                            ) / fA_eff_f;

                fI_eff_y_f = objCrSc.m_fI_y +
                           objCrSc.m_fA * sqr(fz_eff_c_f - objGeo.m_fh / 2.0f) -
                           fb_red_ful * (float)Math.Pow(objGeo.m_ft_f, 3) / 12.0f -
                           fb_red_ful * objGeo.m_ft_f * sqr(fz_eff_c_f - objGeo.m_ft_f / 2.0f) -
                           fb_red_fur * (float)Math.Pow(objGeo.m_ft_f, 3) / 12.0f -
                           fb_red_fur * objGeo.m_ft_f * sqr(fz_eff_c_f - objGeo.m_ft_f / 2.0f) -
                           fb_red_fbl * (float)Math.Pow(objGeo.m_ft_f, 3) / 12.0f -
                           fb_red_fbl * objGeo.m_ft_f * sqr(objGeo.m_fh - fz_eff_c_f - objGeo.m_ft_f / 2.0f) -
                           fb_red_fbr * (float)Math.Pow(objGeo.m_ft_f, 3) / 12.0f -
                           fb_red_fbr * objGeo.m_ft_f * sqr(objGeo.m_fh - fz_eff_c_f - objGeo.m_ft_f / 2.0f);

                fI_eff_z_f = objCrSc.m_fI_z +
                           objCrSc.m_fA * sqr(fy_eff_c_f - objGeo.m_fb / 2.0f) -
                           (float)Math.Pow(fb_red_ful, 3) * objGeo.m_ft_f / 12.0f -
                           fb_red_ful * objGeo.m_ft_f * sqr(fy_eff_c_f - fb_em_ful) -
                           (float)Math.Pow(fb_red_fur, 3) * objGeo.m_ft_f / 12.0f -
                           fb_red_fur * objGeo.m_ft_f * sqr(objGeo.m_fb - fy_eff_c_f - fb_em_fur) -
                           (float)Math.Pow(fb_red_fbl, 3) * objGeo.m_ft_f / 12.0f -
                           fb_red_fbl * objGeo.m_ft_f * sqr(fy_eff_c_f - fb_em_bl) -
                           (float)Math.Pow(fb_red_fbr, 3) * objGeo.m_ft_f / 12.0f -
                           fb_red_fbr * objGeo.m_ft_f * sqr(objGeo.m_fb - fy_eff_c_f - fb_em_br);
            }
            else
            {
                fb_red_ful = 0.0f;
                fb_red_fur = 0.0f;
                fb_red_fbl = 0.0f;
                fb_red_fbr = 0.0f;

                fb_em_ful = 0.0f;
                fb_em_fur = 0.0f;
                fb_em_bl = 0.0f;
                fb_em_br = 0.0f;

                fA_eff_f = objCrSc.m_fA;

                fz_eff_c_f = objGeo.m_fh / 2.0f;
                fy_eff_c_f = objGeo.m_fb / 2.0f;
                fI_eff_y_f = objCrSc.m_fI_y;
                fI_eff_z_f = objCrSc.m_fI_z;
            }

            if (objC__I.m_iClass_w == 4)
            {

                float rSigma_N = objIFO.FN_Ed / fA_eff_f;
                float fSigma_My_wu = objIFO.FM_y_Ed / fI_eff_y_f * (fz_eff_c_f - objGeo.m_ft_f - objGeo.m_fr);
                float fSigma_My_wb = objIFO.FM_y_Ed / fI_eff_y_f * (objGeo.m_fh - fz_eff_c_f - objGeo.m_ft_f - objGeo.m_fr);
                float rSigma_Mz = objIFO.FM_z_Ed / fI_eff_z_f * (fy_eff_c_f - objGeo.m_fb / 2.0f);

                objStr.m_fSigma_wA = rSigma_N - fSigma_My_wu + rSigma_Mz;
                objStr.m_fSigma_wB = rSigma_N + fSigma_My_wb + rSigma_Mz;

                // EN 1993-1-5, Table 4.1
                GetEff_INT(objStr.m_fSigma_wA,
                                         objStr.m_fSigma_wB,
                                         objGeo.m_fc_w,
                                         objGeo.m_ft_w,
                                         objMat.m_fEps_w,
                                         fSigma_1_w,
                                         fSigma_2_w,
                                         fPsi_w,
                                         fk_Sigma_w,
                                         fLambda_rel_p_w,
                                         fRho_w,
                                         fb_eff_w,
                                         fb_e1_w,
                                         fb_e2_w,
                                         fb_red_w,
                                         fb_em_w,
                                         objMat.BStainlessS);

                m_fA_eff = fA_eff_f - fb_red_w * objGeo.m_ft_w;

                fz_eff_c = (objCrSc.m_fA * objGeo.m_fh / 2.0f -
                            fb_red_ful * objGeo.m_ft_f * objGeo.m_ft_f / 2.0f -
                            fb_red_fur * objGeo.m_ft_f * objGeo.m_ft_f / 2.0f -
                            fb_red_fbl * objGeo.m_ft_f * (objGeo.m_fh - objGeo.m_ft_f / 2.0f) -
                            fb_red_fbr * objGeo.m_ft_f * (objGeo.m_fh - objGeo.m_ft_f / 2.0f) -
                            fb_red_w * objGeo.m_ft_w * (fb_em_w + objGeo.m_fr + objGeo.m_ft_f)
                           ) / m_fA_eff;

                fy_eff_c = (objCrSc.m_fA * objGeo.m_fb / 2.0f -
                            fb_red_ful * objGeo.m_ft_f * fb_em_ful -
                            fb_red_fur * objGeo.m_ft_f * (objGeo.m_fb - fb_em_fur) -
                            fb_red_fbl * objGeo.m_ft_f * fb_em_bl -
                            fb_red_fbr * objGeo.m_ft_f * (objGeo.m_fb - fb_em_br) -
                            fb_red_w * objGeo.m_ft_w * objGeo.m_fb / 2.0f
                           ) / m_fA_eff;

                m_fI_eff_y = objCrSc.m_fI_y +
                           objCrSc.m_fA * sqr(fz_eff_c - objGeo.m_fh / 2.0f) -
                           fb_red_ful * (float)Math.Pow(objGeo.m_ft_f, 3) / 12.0f -
                           fb_red_ful * objGeo.m_ft_f * sqr(fz_eff_c - objGeo.m_ft_f / 2.0f) -
                           fb_red_fur * (float)Math.Pow(objGeo.m_ft_f, 3) / 12.0f -
                           fb_red_fur * objGeo.m_ft_f * sqr(fz_eff_c - objGeo.m_ft_f / 2.0f) -
                           fb_red_fbl * (float)Math.Pow(objGeo.m_ft_f, 3) / 12.0f -
                           fb_red_fbl * objGeo.m_ft_f * sqr(objGeo.m_fh - fz_eff_c - objGeo.m_ft_f / 2.0f) -
                           fb_red_fbr * (float)Math.Pow(objGeo.m_ft_f, 3) / 12.0f -
                           fb_red_fbr * objGeo.m_ft_f * sqr(objGeo.m_fh - fz_eff_c - objGeo.m_ft_f / 2.0f) -
                           (float)Math.Pow(fb_red_w, 3) * objGeo.m_ft_w / 12.0f -
                           fb_red_w * objGeo.m_ft_w * sqr(fz_eff_c - objGeo.m_ft_f - fb_em_w - objGeo.m_fr);

                m_fI_eff_z = objCrSc.m_fI_z +
                           objCrSc.m_fA * sqr(fy_eff_c - objGeo.m_fb / 2.0f) -
                           (float)Math.Pow(fb_red_ful, 3) * objGeo.m_ft_f / 12.0f -
                           fb_red_ful * objGeo.m_ft_f * sqr(fy_eff_c - fb_em_ful) -
                           (float)Math.Pow(fb_red_fur, 3) * objGeo.m_ft_f / 12.0f -
                           fb_red_fur * objGeo.m_ft_f * sqr(objGeo.m_fb - fy_eff_c - fb_em_fur) -
                           (float)Math.Pow(fb_red_fbl, 3) * objGeo.m_ft_f / 12.0f -
                           fb_red_fbl * objGeo.m_ft_f * sqr(fy_eff_c - fb_em_bl) -
                           (float)Math.Pow(fb_red_fbr, 3) * objGeo.m_ft_f / 12.0f -
                           fb_red_fbr * objGeo.m_ft_f * sqr(objGeo.m_fb - fy_eff_c - fb_em_br) -
                           (float)Math.Pow(objGeo.m_ft_w, 3) * fb_red_w / 12.0f -
                           objGeo.m_ft_w * fb_red_w * sqr(fy_eff_c - objGeo.m_fb / 2.0f);
            }
            else
            {
                fb_red_w = 0.0f;

                m_fA_eff = fA_eff_f;

                fz_eff_c = fz_eff_c_f;
                fy_eff_c = fy_eff_c_f;
                m_fI_eff_y = fI_eff_y_f;
                m_fI_eff_z = fI_eff_z_f;
            }

            float fA_f_eff = objAdd.m_fA_f - (fb_red_ful + fb_red_fur + fb_red_fbl + fb_red_fbr) * objGeo.m_ft_f;
            float fA_w_eff = objAdd.m_fA_w - fb_red_w * objGeo.m_ft_w;
            m_fN_pl_eff = fA_f_eff * objMat.m_ff_y_f + fA_w_eff * objMat.m_ff_y_w;
            m_fe_Ny = fz_eff_c - objGeo.m_fh / 2.0f;
            m_fe_Nz = fy_eff_c - objGeo.m_fb / 2.0f;
            m_fW_eff_y_min = m_fI_eff_y / Math.Max(fz_eff_c, objGeo.m_fh - fz_eff_c);
            m_fW_eff_z_min = m_fI_eff_z / Math.Max(fy_eff_c - Math.Min(fb_red_ful, fb_red_fbl), objGeo.m_fb - fy_eff_c - Math.Min(fb_red_fur, fb_red_fbr));
        }

        /*
        // Flanges
        float fSigma_1_ful,
              fSigma_2_ful,
              fPsi_ful,
              fk_Sigma_ful,
              fLambda_rel_p_ful,
              fRho_ful,
              fb_eff_ful,
              fb_red_ful,
              fb_em_ful,

              fSigma_1_fur,
              fSigma_2_fur,
              fPsi_fur,
              fk_Sigma_fur,
              fLambda_rel_p_fur,
              fRho_fur,
              fb_eff_fur,
              fb_red_fur,
              fb_em_fur,

              fSigma_1_fbl,
              fSigma_2_fbl,
              fPsi_fbl,
              fk_Sigma_fbl,
              fLambda_rel_p_fbl,
              fRho_fbl,
              fb_eff_fbl,
              fb_red_fbl,
              fb_em_bl,

              fSigma_1_fbr,
              fSigma_2_fbr,
              fPsi_fbr,
              fk_Sigma_fbr,
              fLambda_rel_p_fbr,
              fRho_fbr,
              fb_eff_fbr,
              fb_red_fbr,
              fb_em_br;



        // Web
        float fSigma_1_w,
              fSigma_2_w,
              fPsi_w,
              fk_Sigma_w,
              fLambda_rel_p_w,
              fRho_w,
              fb_eff_w,
              fb_e1_w,
              fb_e2_w,
              fb_red_w,
              fb_em_w;

        float fA_eff_f,
              fz_eff_c_f,
              fy_eff_c_f,
              fI_eff_y_f,
              fI_eff_z_f;


        // Web
        float fz_eff_c,
        fy_eff_c;
        */

        void GetCrSc4_IM(CCrSc objCrSc, C_GEO___I objGeo, C_IFO objIFO, C_STR___I objStr, C___I objC__I, C_MAT___I objMat, C_ADD___I objAdd)
        {
            if (objC__I.m_iClass_fu == 4 || objC__I.m_iClass_fb == 4)
            {
                // EN 1993-1-5, Table 4.2
                GetEff_OUT(objStr.m_fSigma_fulA,
                                        objStr.m_fSigma_fulB,
                                        objGeo.m_fc_fu,
                                        objGeo.m_ft_fu,
                                        objMat.m_fEps_fu,
                                        fSigma_1_ful,
                                        fSigma_2_ful,
                                        fPsi_ful,
                                        fk_Sigma_ful,
                                        fLambda_rel_p_ful,
                                        fRho_ful,
                                        fb_eff_ful,
                                        fb_red_ful,
                                        fb_em_ful,
                                        objMat.BStainlessS);
                                     
                // EN 1993-1-5, Tabl e 4.2
                GetEff_OUT(objStr.m_fSigma_furA,
                                        objStr.m_fSigma_furB,
                                        objGeo.m_fc_fu,
                                        objGeo.m_ft_fu,
                                        objMat.m_fEps_fu,
                                        fSigma_1_fur,
                                        fSigma_2_fur,
                                        fPsi_fur,
                                        fk_Sigma_fur,
                                        fLambda_rel_p_fur,
                                        fRho_fur,
                                        fb_eff_fur,
                                        fb_red_fur,
                                        fb_em_fur,
                                        objMat.BStainlessS);

                // EN 1993-1-5, Table 4.2
                GetEff_OUT(objStr.m_fSigma_fblA,
                                        objStr.m_fSigma_fblB,
                                        objGeo.m_fc_fb,
                                        objGeo.m_ft_fb,
                                        objMat.m_fEps_fb,
                                        fSigma_1_fbl,
                                        fSigma_2_fbl,
                                        fPsi_fbl,
                                        fk_Sigma_fbl,
                                        fLambda_rel_p_fbl,
                                        fRho_fbl,
                                        fb_eff_fbl,
                                        fb_red_fbl,
                                        fb_em_bl,
                                        objMat.BStainlessS);

                // EN 1993-1-5, Table 4.2
                GetEff_OUT(objStr.m_fSigma_fbrA,
                                        objStr.m_fSigma_fbrB,
                                        objGeo.m_fc_fb,
                                        objGeo.m_ft_fb,
                                        objMat.m_fEps_fb,
                                        fSigma_1_fbr,
                                        fSigma_2_fbr,
                                        fPsi_fbr,
                                        fk_Sigma_fbr,
                                        fLambda_rel_p_fbr,
                                        fRho_fbr,
                                        fb_eff_fbr,
                                        fb_red_fbr,
                                        fb_em_br,
                                        objMat.BStainlessS);

                fA_eff_f = objCrSc.m_fA - (fb_red_ful + fb_red_fur) * objGeo.m_ft_fu - (fb_red_fbl + fb_red_fbr) * objGeo.m_ft_fb;

                fz_eff_c_f = (objCrSc.m_fA * objCrSc.m_fz_S -
                              fb_red_ful * objGeo.m_ft_fu * objGeo.m_ft_fu / 2.0f -
                              fb_red_fur * objGeo.m_ft_fu * objGeo.m_ft_fu / 2.0f -
                              fb_red_fbl * objGeo.m_ft_fb * (objGeo.m_fh - objGeo.m_ft_fb / 2.0f) -
                              fb_red_fbr * objGeo.m_ft_fb * (objGeo.m_fh - objGeo.m_ft_fb / 2.0f)
                             ) / fA_eff_f;

                float rb_d = (objGeo.m_fb_fu - objGeo.m_fb_fb) / 2.0f;

                fy_eff_c_f = (objCrSc.m_fA * objGeo.m_fb_fu / 2.0f -
                              fb_red_ful * objGeo.m_ft_fu * fb_em_ful -
                              fb_red_fur * objGeo.m_ft_fu * (objGeo.m_fb_fu - fb_em_fur) -
                              fb_red_fbl * objGeo.m_ft_fb * (rb_d + fb_em_bl) -
                              fb_red_fbr * objGeo.m_ft_fb * (objGeo.m_fb_fu - rb_d - fb_em_br)
                              ) / fA_eff_f;

                fI_eff_y_f = objCrSc.m_fI_y +
                             objCrSc.m_fA * sqr(fz_eff_c_f - objCrSc.m_fz_S) -
                             fb_red_ful * (float)Math.Pow(objGeo.m_ft_fu, 3) / 12.0f -
                             fb_red_ful * objGeo.m_ft_fu * sqr(fz_eff_c_f - objGeo.m_ft_fu / 2.0f) -
                             fb_red_fur * (float)Math.Pow(objGeo.m_ft_fu, 3) / 12.0f -
                             fb_red_fur * objGeo.m_ft_fu * sqr(fz_eff_c_f - objGeo.m_ft_fu / 2.0f) -
                             fb_red_fbl * (float)Math.Pow(objGeo.m_ft_fb, 3) / 12.0f -
                             fb_red_fbl * objGeo.m_ft_fb * sqr(objGeo.m_fh - fz_eff_c_f - objGeo.m_ft_fb / 2.0f) -
                             fb_red_fbr * (float)Math.Pow(objGeo.m_ft_fb, 3) / 12.0f -
                             fb_red_fbr * objGeo.m_ft_fb * sqr(objGeo.m_fh - fz_eff_c_f - objGeo.m_ft_fb / 2.0f);

                fI_eff_z_f = objCrSc.m_fI_z +
                             objCrSc.m_fA * sqr(fy_eff_c_f - objGeo.m_fb_fu / 2.0f) -
                             (float)Math.Pow(fb_red_ful, 3) * objGeo.m_ft_fu / 12.0f -
                             fb_red_ful * objGeo.m_ft_fu * sqr(fy_eff_c_f - fb_em_ful) -
                             (float)Math.Pow(fb_red_fur, 3) * objGeo.m_ft_fu / 12.0f -
                             fb_red_fur * objGeo.m_ft_fu * sqr(objGeo.m_fb_fu - fy_eff_c_f - fb_em_fur) -
                             (float)Math.Pow(fb_red_fbl, 3) * objGeo.m_ft_fb / 12.0f -
                             fb_red_fbl * objGeo.m_ft_fb * sqr(fy_eff_c_f - rb_d - fb_em_bl) -
                             (float)Math.Pow(fb_red_fbr, 3) * objGeo.m_ft_fb / 12.0f -
                             fb_red_fbr * objGeo.m_ft_fb * sqr(objGeo.m_fb_fb + rb_d - fy_eff_c_f - fb_em_br);
            }
            else
            {
                fb_red_ful = 0.0f;
                fb_red_fur = 0.0f;
                fb_red_fbl = 0.0f;
                fb_red_fbr = 0.0f;

                fb_em_ful = 0.0f;
                fb_em_fur = 0.0f;
                fb_em_bl = 0.0f;
                fb_em_br = 0.0f;

                fA_eff_f = objCrSc.m_fA;

                fz_eff_c_f = objCrSc.m_fz_S;
                fy_eff_c_f = objGeo.m_fb_fu / 2.0f;

                fI_eff_y_f = objCrSc.m_fI_y;
                fI_eff_z_f = objCrSc.m_fI_z;
            }

            if (objC__I.m_iClass_w == 4)
            {
                float rSigma_N = objIFO.FN_Ed / fA_eff_f;
                float fSigma_My_wu = objIFO.FM_y_Ed / fI_eff_y_f * (fz_eff_c_f - objGeo.m_ft_fu - objGeo.m_fr_su);
                float fSigma_My_wb = objIFO.FM_y_Ed / fI_eff_y_f * (objGeo.m_fh - fz_eff_c_f - objGeo.m_ft_fb - objGeo.m_fr_sb);
                float rSigma_Mz = objIFO.FM_z_Ed / fI_eff_z_f * (fy_eff_c_f - objGeo.m_fb_fu / 2.0f);


                objStr.m_fSigma_wA = rSigma_N - fSigma_My_wu + rSigma_Mz;
                objStr.m_fSigma_wB = rSigma_N + fSigma_My_wb + rSigma_Mz;

                // EN 1993-1-5, Table 4.1
                GetEff_INT(objStr.m_fSigma_wA,
                                         objStr.m_fSigma_wB,
                                         objGeo.m_fc_w,
                                         objGeo.m_ft_w,
                                         objMat.m_fEps_w,
                                         fSigma_1_w,
                                         fSigma_2_w,
                                         fPsi_w,
                                         fk_Sigma_w,
                                         fLambda_rel_p_w,
                                         fRho_w,
                                         fb_eff_w,
                                         fb_e1_w,
                                         fb_e2_w,
                                         fb_red_w,
                                         fb_em_w,
                                         objMat.BStainlessS);

                m_fA_eff = fA_eff_f - fb_red_w * objGeo.m_ft_w;

                fz_eff_c = (objCrSc.m_fA * objCrSc.m_fz_S -
                            fb_red_ful * objGeo.m_ft_fu * objGeo.m_ft_fu / 2.0f -
                            fb_red_fur * objGeo.m_ft_fu * objGeo.m_ft_fu / 2.0f -
                            fb_red_fbl * objGeo.m_ft_fb * (objGeo.m_fh - objGeo.m_ft_fb / 2.0f) -
                            fb_red_fbr * objGeo.m_ft_fb * (objGeo.m_fh - objGeo.m_ft_fb / 2.0f) -
                            fb_red_w * objGeo.m_ft_w * (fb_em_w + objGeo.m_fr_su + objGeo.m_ft_fu)
                           ) / m_fA_eff;

                float rb_d = (objGeo.m_fb_fu - objGeo.m_fb_fb) / 2.0f;

                fy_eff_c = (objCrSc.m_fA * objGeo.m_fb_fu / 2.0f -
                            fb_red_ful * objGeo.m_ft_fu * fb_em_ful -
                            fb_red_fur * objGeo.m_ft_fu * (objGeo.m_fb_fu - fb_em_fur) -
                            fb_red_fbl * objGeo.m_ft_fb * (rb_d + fb_em_bl) -
                            fb_red_fbr * objGeo.m_ft_fb * (objGeo.m_fb_fu - rb_d - fb_em_br) -
                            fb_red_w * objGeo.m_ft_w * objGeo.m_fb_fu / 2.0f
                           ) / m_fA_eff;

                // Trägheitsmomente des wirksamen relschnitts
                m_fI_eff_y = objCrSc.m_fI_y +
                           objCrSc.m_fA * sqr(fz_eff_c - objCrSc.m_fz_S) -
                           fb_red_ful * (float)Math.Pow(objGeo.m_ft_fu, 3) / 12.0f -
                           fb_red_ful * objGeo.m_ft_fu * sqr(fz_eff_c - objGeo.m_ft_fu / 2.0f) -
                           fb_red_fur * (float)Math.Pow(objGeo.m_ft_fu, 3) / 12.0f -
                           fb_red_fur * objGeo.m_ft_fu * sqr(fz_eff_c - objGeo.m_ft_fu / 2.0f) -
                           fb_red_fbl * (float)Math.Pow(objGeo.m_ft_fb, 3) / 12.0f -
                           fb_red_fbl * objGeo.m_ft_fb * sqr(objGeo.m_fh - fz_eff_c - objGeo.m_ft_fb / 2.0f) -
                           fb_red_fbr * (float)Math.Pow(objGeo.m_ft_fb, 3) / 12.0f -
                           fb_red_fbr * objGeo.m_ft_fb * sqr(objGeo.m_fh - fz_eff_c - objGeo.m_ft_fb / 2.0f) -
                           (float)Math.Pow(fb_red_w, 3) * objGeo.m_ft_w / 12.0f -
                           fb_red_w * objGeo.m_ft_w * sqr(fz_eff_c - objGeo.m_ft_fu - fb_em_w - objGeo.m_fr_su);

                m_fI_eff_z = objCrSc.m_fI_z +
                           objCrSc.m_fA * sqr(fy_eff_c - objGeo.m_fb_fu / 2.0f) -
                           (float)Math.Pow(fb_red_ful, 3) * objGeo.m_ft_fu / 12.0f -
                           fb_red_ful * objGeo.m_ft_fu * sqr(fy_eff_c - fb_em_ful) -
                           (float)Math.Pow(fb_red_fur, 3) * objGeo.m_ft_fu / 12.0f -
                           fb_red_fur * objGeo.m_ft_fu * sqr(objGeo.m_fb_fu - fy_eff_c - fb_em_fur) -
                           (float)Math.Pow(fb_red_fbl, 3) * objGeo.m_ft_fb / 12.0f -
                           fb_red_fbl * objGeo.m_ft_fb * sqr(fy_eff_c_f - rb_d - fb_em_bl) -
                           (float)Math.Pow(fb_red_fbr, 3) * objGeo.m_ft_fb / 12.0f -
                           fb_red_fbr * objGeo.m_ft_fb * sqr(objGeo.m_fb_fb + rb_d - fy_eff_c_f - fb_em_br) -
                           (float)Math.Pow(objGeo.m_ft_w, 3) * fb_red_w / 12.0f -
                           objGeo.m_ft_w * fb_red_w * sqr(fy_eff_c - objGeo.m_fb_fu / 2.0f);
            }
            else
            {
                fb_red_w = 0.0f;

                m_fA_eff = fA_eff_f;

                fz_eff_c = fz_eff_c_f;
                fy_eff_c = fy_eff_c_f;

                m_fI_eff_y = fI_eff_y_f;
                m_fI_eff_z = fI_eff_z_f;
            }

            float fA_fu_eff = objAdd.m_fA_fu - (fb_red_ful + fb_red_fur) * objGeo.m_ft_fu;
            float fA_fb_eff = objAdd.m_fA_fb - (fb_red_fbl + fb_red_fbr) * objGeo.m_ft_fb;
            float fA_w_eff = objAdd.m_fA_w - fb_red_w * objGeo.m_ft_w;
            m_fN_pl_eff = fA_fu_eff * objMat.m_ff_y_fu + fA_fb_eff * objMat.m_ff_y_fb + fA_w_eff * objMat.m_ff_y_w;
            m_fe_Ny = fz_eff_c - objCrSc.m_fz_S;
            m_fe_Nz = fy_eff_c - objGeo.m_fb_fu / 2.0f;

            float fW_eff_y_fu = m_fI_eff_y / fz_eff_c;
            float fW_eff_y_fb = m_fI_eff_y / (objGeo.m_fh - fz_eff_c);
            m_fW_eff_y_min = Math.Min(fW_eff_y_fu, fW_eff_y_fb);
            float fW_eff_z_fu = m_fI_eff_z / Math.Max(fy_eff_c - fb_red_ful, objGeo.m_fb_fu - fy_eff_c - fb_red_fur);
            float fW_eff_z_fb = m_fI_eff_z / Math.Max(fy_eff_c + ((objGeo.m_fb_fb - objGeo.m_fb_fu) / 2.0f) - fb_red_fbl, objGeo.m_fb_fu + (objGeo.m_fb_fb - objGeo.m_fb_fu) / 2.0f - fy_eff_c - fb_red_fbr);
            m_fW_eff_z_min = Math.Min(fW_eff_z_fu, fW_eff_z_fb);
        }
    }
}
