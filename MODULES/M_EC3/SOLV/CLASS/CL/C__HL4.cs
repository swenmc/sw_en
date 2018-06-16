using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using M_BASE;

namespace M_EC3
{
    class C__HL4 : CL_EF
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

        public C__HL4(CCrSc objCrSc, C_GEO__HL objGeo, C_IFO objIFO, C_STR__HL objStr, C__HL objC_HL, C_MAT__HL objMat, C_ADD__HL objAdd, ECrScPrType1 eProd)
        {
            if (eProd != ECrScPrType1.eCrSc_wld && eProd != ECrScPrType1.eCrSc_wldnorm)
                GetCrSc4_HLR(objCrSc, objGeo, objIFO, objStr, objC_HL, objMat, objAdd);
            else
                GetCrSc4_HLW(objCrSc, objGeo, objIFO, objStr, objC_HL, objMat, objAdd);
        }

        // Flanges
        // Upper Flange
        float fSigma_1_fu,
              fSigma_2_fu,
              fPsi_fu,
              fk_Sigma_fu,
              fLambda_rel_p_fu,
              fRho_fu,
              fb_eff_fu,
              fb_e1_fu,
              fb_e2_fu,
              fb_red_fu,
              fb_em_fu,

            // Bottom Flange

              fSigma_1_fb,
              fSigma_2_fb,
              fPsi_fb,
              fk_Sigma_fb,
              fLambda_rel_p_fb,
              fRho_fb,
              fb_eff_fb,
              fb_e1_fb,
              fb_e2_fb,
              fb_red_fb,
              fb_em_fb;

   
        
        // Webs
        // Left Web
        float fSigma_1_wl,
              fSigma_2_wl,
              fPsi_wl,
              fk_Sigma_wl,
              fLambda_rel_p_wl,
              fRho_wl,
              fb_eff_wl,
              fb_e1_wl,
              fb_e2_wl,
              fb_red_wl,
              fb_em_wl;

        // Right Web

        float fSigma_1_wr,
      fSigma_2_wr,
      fPsi_wr,
      fk_Sigma_wr,
      fLambda_rel_p_wr,
      fRho_wr,
      fb_eff_wr,
      fb_e1_wr,
      fb_e2_wr,
      fb_red_wr,
      fb_em_wr;

        float fA_eff_f,
              fz_eff_c_f,
              fy_eff_c_f,
              fI_eff_y_f,
              fI_eff_z_f;

        // Web
        float fz_eff_c,
              fy_eff_c;

        void GetCrSc4_HLR(CCrSc objCrSc, C_GEO__HL objGeo, C_IFO objIFO, C_STR__HL objStr, C__HL objC_HL, C_MAT__HL objMat, C_ADD__HL objAdd)
        {
            if (objC_HL.m_iClass_fu == 4 || objC_HL.m_iClass_fb == 4)
            {
                // EN 1993-1-5,Table 4.1
                GetEff_INT(objStr.m_fSigma_fuA,
                                         objStr.m_fSigma_fuB,
                                         objGeo.m_fc_f,
                                         objGeo.m_ft_f,
                                         objMat.m_fEps_f,
                                         fSigma_1_fu,
                                         fSigma_2_fu,
                                         fPsi_fu,
                                         fk_Sigma_fu,
                                         fLambda_rel_p_fu,
                                         fRho_fu,
                                         fb_eff_fu,
                                         fb_e1_fu,
                                         fb_e2_fu,
                                         fb_red_fu,
                                         fb_em_fu,
                                         objMat.BStainlessS);

                // EN 1993-1-5,Table 4.1
                GetEff_INT(objStr.m_fSigma_fbA,
                                         objStr.m_fSigma_fbB,
                                         objGeo.m_fc_f,
                                         objGeo.m_ft_f,
                                         objMat.m_fEps_f,
                                         fSigma_1_fb,
                                         fSigma_2_fb,
                                         fPsi_fb,
                                         fk_Sigma_fb,
                                         fLambda_rel_p_fb,
                                         fRho_fb,
                                         fb_eff_fb,
                                         fb_e1_fb,
                                         fb_e2_fb,
                                         fb_red_fb,
                                         fb_em_fb,
                                         objMat.BStainlessS);


                fA_eff_f = objCrSc.m_fA - (fb_red_fu + fb_red_fb) * objGeo.m_ft_f;


                fz_eff_c_f = (objCrSc.m_fA * objGeo.m_fh / 2.0f -
                              fb_red_fu * objGeo.m_ft_f * objGeo.m_ft_f / 2.0f -
                              fb_red_fb * objGeo.m_ft_f * (objGeo.m_fh - objGeo.m_ft_f / 2.0f)
                             ) / fA_eff_f;

                fy_eff_c_f = (objCrSc.m_fA * objGeo.m_fb / 2.0f -
                              fb_red_fu * objGeo.m_ft_f * (objGeo.m_ft_w + objGeo.m_fr + fb_em_fu) -
                              fb_red_fb * objGeo.m_ft_f * (objGeo.m_ft_w + objGeo.m_fr + fb_em_fb)
                              ) / fA_eff_f;


                fI_eff_y_f = objCrSc.m_fI_y +
                             objCrSc.m_fA * sqr(fz_eff_c_f - objGeo.m_fh / 2.0f) -
                             fb_red_fu * (float)Math.Pow(objGeo.m_ft_f, 3) / 12.0f -
                             fb_red_fu * objGeo.m_ft_f * sqr(fz_eff_c_f - objGeo.m_ft_f / 2.0f) -
                             fb_red_fb * (float)Math.Pow(objGeo.m_ft_f, 3) / 12.0f -
                             fb_red_fb * objGeo.m_ft_f * sqr(objGeo.m_fh - fz_eff_c_f - objGeo.m_ft_f / 2.0f);

                fI_eff_z_f = objCrSc.m_fI_z +
                             objCrSc.m_fA * sqr(fy_eff_c_f - objGeo.m_fb / 2.0f) -
                             (float)Math.Pow(fb_red_fu, 3) * objGeo.m_ft_f / 12.0f -
                             fb_red_fu * objGeo.m_ft_f * sqr(fy_eff_c_f - fb_em_fu - objGeo.m_ft_w - objGeo.m_fr) -
                             (float)Math.Pow(fb_red_fb, 3) * objGeo.m_ft_f / 12.0f -
                             fb_red_fb * objGeo.m_ft_f * sqr(fy_eff_c_f - fb_em_fb - objGeo.m_ft_w - objGeo.m_fr);
            }
            else
            {
                fb_red_fu = 0.0f;
                fb_red_fb = 0.0f;

                fb_em_fu = 0.0f;
                fb_em_fb = 0.0f;

                fA_eff_f = objCrSc.m_fA;

                fz_eff_c_f = objGeo.m_fh / 2.0f;
                fy_eff_c_f = objGeo.m_fb / 2.0f;
                fI_eff_y_f = objCrSc.m_fI_y;
                fI_eff_z_f = objCrSc.m_fI_z;
            }

            if (objC_HL.m_iClass_w == 4)
            {

                float fSigma_N = objIFO.FN_Ed / fA_eff_f;
                float fSigma_My_wo = objIFO.FM_y_Ed / fI_eff_y_f * (fz_eff_c_f - objGeo.m_ft_f - objGeo.m_fr);
                float fSigma_My_wu = objIFO.FM_y_Ed / fI_eff_y_f * (objGeo.m_fh - fz_eff_c_f - objGeo.m_ft_f - objGeo.m_fr);
                float fSigma_Mz_wl = objIFO.FM_z_Ed / fI_eff_z_f * (fy_eff_c_f - objGeo.m_ft_w - objGeo.m_fr);
                float fSigma_Mz_wr = objIFO.FM_z_Ed / fI_eff_z_f * (objGeo.m_fb - fy_eff_c_f - objGeo.m_ft_w - objGeo.m_fr);

                objStr.m_fSigma_wlA = fSigma_N - fSigma_My_wo + fSigma_Mz_wl;
                objStr.m_fSigma_wlB = fSigma_N + fSigma_My_wu + fSigma_Mz_wl;

                // EN 1993-1-5,Table 4.1
                GetEff_INT(objStr.m_fSigma_wlA,
                                         objStr.m_fSigma_wlB,
                                         objGeo.m_fc_w,
                                         objGeo.m_ft_w,
                                         objMat.m_fEps_w,
                                         fSigma_1_wl,
                                         fSigma_2_wl,
                                         fPsi_wl,
                                         fk_Sigma_wl,
                                         fLambda_rel_p_wl,
                                         fRho_wl,
                                         fb_eff_wl,
                                         fb_e1_wl,
                                         fb_e2_wl,
                                         fb_red_wl,
                                         fb_em_wl,
                                         objMat.BStainlessS);

                objStr.m_fSigma_wrA = fSigma_N - fSigma_My_wo - fSigma_Mz_wl;
                objStr.m_fSigma_wrB = fSigma_N + fSigma_My_wu - fSigma_Mz_wl;

                // EN 1993-1-5,Table 4.1
                GetEff_INT(objStr.m_fSigma_wrA,
                                         objStr.m_fSigma_wrB,
                                         objGeo.m_fc_w,
                                         objGeo.m_ft_w,
                                         objMat.m_fEps_w,
                                         fSigma_1_wr,
                                         fSigma_2_wr,
                                         fPsi_wr,
                                         fk_Sigma_wr,
                                         fLambda_rel_p_wr,
                                         fRho_wr,
                                         fb_eff_wr,
                                         fb_e1_wr,
                                         fb_e2_wr,
                                         fb_red_wr,
                                         fb_em_wr,
                                         objMat.BStainlessS);

                m_fA_eff = fA_eff_f - (fb_red_wl + fb_red_wr) * objGeo.m_ft_w;

                fz_eff_c = (objCrSc.m_fA * objGeo.m_fh / 2.0f -
                           fb_red_fu * objGeo.m_ft_f * objGeo.m_ft_f / 2.0f -
                           fb_red_fb * objGeo.m_ft_f * (objGeo.m_fh - objGeo.m_ft_f / 2.0f) -
                           fb_red_wl * objGeo.m_ft_w * (fb_em_wl + objGeo.m_ft_f + objGeo.m_fr) -
                           fb_red_wr * objGeo.m_ft_w * (fb_em_wr + objGeo.m_ft_f + objGeo.m_fr)
                          ) / m_fA_eff;

                fy_eff_c = (objCrSc.m_fA * objGeo.m_fb / 2.0f -
                            fb_red_fu * objGeo.m_ft_f * (fb_em_fu + objGeo.m_ft_w + objGeo.m_fr) -
                            fb_red_fb * objGeo.m_ft_f * (fb_em_fb + objGeo.m_ft_w + objGeo.m_fr) -
                            fb_red_wl * objGeo.m_ft_w * objGeo.m_ft_w / 2.0f -
                            fb_red_wr * objGeo.m_ft_w * (objGeo.m_fb - objGeo.m_ft_w / 2.0f)
                           ) / m_fA_eff;


                m_fI_eff_y = objCrSc.m_fI_y +
                           objCrSc.m_fA * sqr(fz_eff_c - objGeo.m_fh / 2.0f) -
                           fb_red_fu * (float)Math.Pow(objGeo.m_ft_f, 3) / 12.0f -
                           fb_red_fu * objGeo.m_ft_f * sqr(fz_eff_c - objGeo.m_ft_f / 2.0f) -
                           fb_red_fb * (float)Math.Pow(objGeo.m_ft_f, 3) / 12.0f -
                           fb_red_fb * objGeo.m_ft_f * sqr(objGeo.m_fh - fz_eff_c - objGeo.m_ft_f / 2.0f) -
                           (float)Math.Pow(fb_red_wl, 3) * objGeo.m_ft_w / 12.0f -
                           fb_red_wl * objGeo.m_ft_w * sqr(fz_eff_c - fb_em_wl - objGeo.m_ft_f - objGeo.m_fr) -
                           (float)Math.Pow(fb_red_wr, 3) * objGeo.m_ft_w / 12.0f -
                           fb_red_wr * objGeo.m_ft_w * sqr(fz_eff_c - fb_em_wr - objGeo.m_ft_f - objGeo.m_fr);

                m_fI_eff_z = objCrSc.m_fI_z +
                            objCrSc.m_fA * sqr(fy_eff_c - objGeo.m_fb / 2.0f) -
                           (float)Math.Pow(fb_red_fu, 3) * objGeo.m_ft_f / 12.0f -
                           fb_red_fu * objGeo.m_ft_f * sqr(fy_eff_c - fb_em_fu - objGeo.m_ft_w - objGeo.m_fr) -
                            (float)Math.Pow(fb_red_fb, 3) * objGeo.m_ft_f / 12.0f -
                           fb_red_fb * objGeo.m_ft_f * sqr(fy_eff_c - fb_em_fb - objGeo.m_ft_w - objGeo.m_fr) -
                           (float)Math.Pow(objGeo.m_ft_w, 3) * fb_red_wl / 12.0f -
                           objGeo.m_ft_w * fb_red_wl * sqr(fy_eff_c - objGeo.m_ft_w / 2.0f) -
                           (float)Math.Pow(objGeo.m_ft_w, 3) * fb_red_wr / 12.0f -
                           objGeo.m_ft_w * fb_red_wr * sqr(objGeo.m_fb - fy_eff_c - objGeo.m_ft_w / 2.0f);
            }
            else
            {
                fb_red_wl = 0.0f;
                fb_red_wr = 0.0f;

                m_fA_eff = fA_eff_f;

                fz_eff_c = fz_eff_c_f;
                fy_eff_c = fy_eff_c_f;
                m_fI_eff_y = fI_eff_y_f;
                m_fI_eff_z = fI_eff_z_f;
            }


            m_fA_eff = objCrSc.m_fA - (fb_red_fu + fb_red_fb + fb_red_wl + fb_red_wr) *objGeo.m_ft;


            m_fN_pl_eff = m_fA_eff * objMat.Ff_y;


            m_fe_Ny = fz_eff_c - objGeo.m_fh / 2.0f;
            m_fe_Nz = fy_eff_c - objGeo.m_fb / 2.0f;


            m_fW_eff_y_min = m_fI_eff_y / Math.Max(fz_eff_c, objGeo.m_fh - fz_eff_c);
            m_fW_eff_z_min = m_fI_eff_z / Math.Max(fy_eff_c, objGeo.m_fb - fy_eff_c);


        }

        void GetCrSc4_HLW(CCrSc objCrSc, C_GEO__HL objGeo, C_IFO objIFO, C_STR__HL objStr, C__HL objC_HL, C_MAT__HL objMat, C_ADD__HL objAdd)
        {
            if (objC_HL.m_iClass_fu == 4 || objC_HL.m_iClass_fb == 4)
            {
                // EN 1993-1-5,Table 4.1
                GetEff_INT(objStr.m_fSigma_fuA,
                                         objStr.m_fSigma_fuB,
                                         objGeo.m_fc_f,
                                         objGeo.m_ft_fu,
                                         objMat.m_fEps_fu,
                                         fSigma_1_fu,
                                         fSigma_2_fu,
                                         fPsi_fu,
                                         fk_Sigma_fu,
                                         fLambda_rel_p_fu,
                                         fRho_fu,
                                         fb_eff_fu,
                                         fb_e1_fu,
                                         fb_e2_fu,
                                         fb_red_fu,
                                         fb_em_fu,
                                         objMat.BStainlessS);

                //EN 1993-1-5,Table 4.1
                GetEff_INT(objStr.m_fSigma_fbA,
                                         objStr.m_fSigma_fbB,
                                         objGeo.m_fc_f,
                                         objGeo.m_ft_fb,
                                         objMat.m_fEps_fb,
                                         fSigma_1_fb,
                                         fSigma_2_fb,
                                         fPsi_fb,
                                         fk_Sigma_fb,
                                         fLambda_rel_p_fb,
                                         fRho_fb,
                                         fb_eff_fb,
                                         fb_e1_fb,
                                         fb_e2_fb,
                                         fb_red_fb,
                                         fb_em_fb,
                                         objMat.BStainlessS);


                fA_eff_f = objCrSc.m_fA - (fb_red_fu * objGeo.m_ft_fu + fb_red_fb * objGeo.m_ft_fb);

                fz_eff_c_f = (objCrSc.m_fA * objGeo.m_fh / 2.0f -
                              fb_red_fu * objGeo.m_ft_fu * objGeo.m_ft_fu / 2.0f -
                              fb_red_fb * objGeo.m_ft_fb * (objGeo.m_fh - objGeo.m_ft_fb / 2.0f)
                             ) / fA_eff_f;

                fy_eff_c_f = (objCrSc.m_fA * objGeo.m_fb / 2.0f -
                              fb_red_fu * objGeo.m_ft_fu * (objGeo.m_ft_w + fb_em_fu) -
                              fb_red_fb * objGeo.m_ft_fb * (objGeo.m_ft_w + fb_em_fb)
                              ) / fA_eff_f;

                fI_eff_y_f = objCrSc.m_fI_y +
                             objCrSc.m_fA * sqr(fz_eff_c_f - objGeo.m_fh / 2.0f) -
                             fb_red_fu * (float)Math.Pow(objGeo.m_ft_fu, 3) / 12.0f -
                             fb_red_fu * objGeo.m_ft_fu * sqr(fz_eff_c_f - objGeo.m_ft_fu / 2.0f) -
                             fb_red_fb * (float)Math.Pow(objGeo.m_ft_fb, 3) / 12.0f -
                             fb_red_fb * objGeo.m_ft_fb * sqr(objGeo.m_fh - fz_eff_c_f - objGeo.m_ft_fb / 2.0f);

                fI_eff_z_f = objCrSc.m_fI_z +
                             objCrSc.m_fA * sqr(fy_eff_c_f - objGeo.m_fb / 2.0f) -
                             (float)Math.Pow(fb_red_fu, 3) * objGeo.m_ft_fu / 12.0f -
                             fb_red_fu * objGeo.m_ft_fu * sqr(fy_eff_c_f - fb_em_fu - objGeo.m_ft_w) -
                             (float)Math.Pow(fb_red_fb, 3) * objGeo.m_ft_fb / 12.0f -
                             fb_red_fb * objGeo.m_ft_fb * sqr(fy_eff_c_f - fb_em_fb - objGeo.m_ft_w);
            }
            else
            {
                fb_red_fu = 0.0f;
                fb_red_fb = 0.0f;

                fb_em_fu = 0.0f;
                fb_em_fb = 0.0f;

                fA_eff_f = objCrSc.m_fA;

                fz_eff_c_f = objGeo.m_fh / 2.0f;
                fy_eff_c_f = objGeo.m_fb / 2.0f;
                fI_eff_y_f = objCrSc.m_fI_y;
                fI_eff_z_f = objCrSc.m_fI_z;
            }

            if (objC_HL.m_iClass_w == 4)
            {

                float fSigma_N = objIFO.FN_Ed / fA_eff_f;
                float fSigma_My_wo = objIFO.FM_y_Ed / fI_eff_y_f * (fz_eff_c_f - objGeo.m_ft_fu);
                float fSigma_My_wu = objIFO.FM_y_Ed / fI_eff_y_f * (objGeo.m_fh - fz_eff_c_f - objGeo.m_ft_fb);
                float fSigma_Mz_wl = objIFO.FM_z_Ed / fI_eff_z_f * (fy_eff_c_f - objGeo.m_ft_w);
                float fSigma_Mz_wr = objIFO.FM_z_Ed / fI_eff_z_f * (objGeo.m_fb - fy_eff_c_f - objGeo.m_ft_w);


                objStr.m_fSigma_wlA = fSigma_N - fSigma_My_wo + fSigma_Mz_wl;
                objStr.m_fSigma_wlB = fSigma_N + fSigma_My_wu + fSigma_Mz_wl;

                // EN 1993-1-5,Table 4.1
                GetEff_INT(objStr.m_fSigma_wlA,
                                         objStr.m_fSigma_wlB,
                                         objGeo.m_fc_w,
                                         objGeo.m_ft_w,
                                         objMat.m_fEps_w,
                                         fSigma_1_wl,
                                         fSigma_2_wl,
                                         fPsi_wl,
                                         fk_Sigma_wl,
                                         fLambda_rel_p_wl,
                                         fRho_wl,
                                         fb_eff_wl,
                                         fb_e1_wl,
                                         fb_e2_wl,
                                         fb_red_wl,
                                         fb_em_wl,
                                         objMat.BStainlessS);


                objStr.m_fSigma_wrA = fSigma_N - fSigma_My_wo - fSigma_Mz_wl;
                objStr.m_fSigma_wrB = fSigma_N + fSigma_My_wu - fSigma_Mz_wl;

                // EN 1993-1-5,Table 4.1
                GetEff_INT(objStr.m_fSigma_wrA,
                                         objStr.m_fSigma_wrB,
                                         objGeo.m_fc_w,
                                         objGeo.m_ft_w,
                                         objMat.m_fEps_w,
                                         fSigma_1_wr,
                                         fSigma_2_wr,
                                         fPsi_wr,
                                         fk_Sigma_wr,
                                         fLambda_rel_p_wr,
                                         fRho_wr,
                                         fb_eff_wr,
                                         fb_e1_wr,
                                         fb_e2_wr,
                                         fb_red_wr,
                                         fb_em_wr,
                                         objMat.BStainlessS);

                m_fA_eff = fA_eff_f - (fb_red_wl + fb_red_wr) * objGeo.m_ft_w;


                fz_eff_c = (objCrSc.m_fA * objGeo.m_fh / 2.0f -
                            fb_red_fu * objGeo.m_ft_fu * objGeo.m_ft_fu / 2.0f -
                            fb_red_fb * objGeo.m_ft_fb * (objGeo.m_fh - objGeo.m_ft_fb / 2.0f) -
                            fb_red_wl * objGeo.m_ft_w * (fb_em_wl + objGeo.m_ft_fu) -
                            fb_red_wr * objGeo.m_ft_w * (fb_em_wr + objGeo.m_ft_fu)
                           ) / m_fA_eff;

                fy_eff_c = (objCrSc.m_fA * objGeo.m_fb / 2.0f -
                            fb_red_fu * objGeo.m_ft_fu * (fb_em_fu + objGeo.m_ft_w) -
                            fb_red_fb * objGeo.m_ft_fb * (fb_em_fb + objGeo.m_ft_w) -
                            fb_red_wl * objGeo.m_ft_w * objGeo.m_ft_w / 2.0f -
                            fb_red_wr * objGeo.m_ft_w * (objGeo.m_fb - objGeo.m_ft_w / 2.0f)
                           ) / m_fA_eff;

                m_fI_eff_y = objCrSc.m_fI_y +
                   objCrSc.m_fA * sqr(fz_eff_c - objGeo.m_fh / 2.0f) -
                   fb_red_fu * (float)Math.Pow(objGeo.m_ft_fu, 3) / 12.0f -
                   fb_red_fu * objGeo.m_ft_fu * sqr(fz_eff_c - objGeo.m_ft_fu / 2.0f) -
                   fb_red_fb * (float)Math.Pow(objGeo.m_ft_fb, 3) / 12.0f -
                   fb_red_fb * objGeo.m_ft_fb * sqr(objGeo.m_fh - fz_eff_c - objGeo.m_ft_fb / 2.0f) -
                   (float)Math.Pow(fb_red_wl, 3) * objGeo.m_ft_w / 12.0f -
                   fb_red_wl * objGeo.m_ft_w * sqr(fz_eff_c - fb_em_wl - objGeo.m_ft_fu) -
                   (float)Math.Pow(fb_red_wr, 3) * objGeo.m_ft_w / 12.0f -
                   fb_red_wr * objGeo.m_ft_w * sqr(fz_eff_c - fb_em_wr - objGeo.m_ft_fu);

                m_fI_eff_z = objCrSc.m_fI_z +
                            objCrSc.m_fA * sqr(fy_eff_c - objGeo.m_fb / 2.0f) -
                           (float)Math.Pow(fb_red_fu, 3) * objGeo.m_ft_fu / 12.0f -
                           fb_red_fu * objGeo.m_ft_fu * sqr(fy_eff_c - fb_em_fu - objGeo.m_ft_w) -
                            (float)Math.Pow(fb_red_fb, 3) * objGeo.m_ft_fb / 12.0f -
                           fb_red_fb * objGeo.m_ft_fb * sqr(fy_eff_c - fb_em_fb - objGeo.m_ft_w) -
                           (float)Math.Pow(objGeo.m_ft_w, 3) * fb_red_wl / 12.0f -
                           objGeo.m_ft_w * fb_red_wl * sqr(fy_eff_c - objGeo.m_ft_w / 2.0f) -
                           (float)Math.Pow(objGeo.m_ft_w, 3) * fb_red_wr / 12.0f -
                           objGeo.m_ft_w * fb_red_wr * sqr(objGeo.m_fb - fy_eff_c - objGeo.m_ft_w / 2.0f);
            }
            else
            {
                fb_red_wl = 0.0f;
                fb_red_wr = 0.0f;

                m_fA_eff = fA_eff_f;

                fz_eff_c = fz_eff_c_f;
                fy_eff_c = fy_eff_c_f;
                m_fI_eff_y = fI_eff_y_f;
                m_fI_eff_z = fI_eff_z_f;
            }


            m_fA_eff = objCrSc.m_fA - (fb_red_fu * objGeo.m_ft_fu + fb_red_fb * objGeo.m_ft_fb + fb_red_wl * objGeo.m_ft_w + fb_red_wr * objGeo.m_ft_w);

            float fA_fu = objGeo.m_fb * objGeo.m_ft_fu;
            float fA_fb = objGeo.m_fb * objGeo.m_ft_fb;
            objAdd.m_fA_w = 2.0f * objGeo.m_fh_w * objGeo.m_ft_w;

            float fA_fu_eff = fA_fu - fb_red_fu * objGeo.m_ft_fu;
            float fA_fb_eff = fA_fb - fb_red_fb * objGeo.m_ft_fb;

            float fA_w_eff = objAdd.m_fA_w - (fb_red_wl * objGeo.m_ft_w + fb_red_wr * objGeo.m_ft_w);

            m_fN_pl_eff = fA_fu_eff * objMat.m_ff_y_fu + fA_fb_eff * objMat.m_ff_y_fb + fA_w_eff * objMat.m_ff_y_w;

            m_fe_Ny = fz_eff_c - objGeo.m_fh / 2.0f;
            m_fe_Nz = fy_eff_c - objGeo.m_fb / 2.0f;

            m_fW_eff_y_min = m_fI_eff_y / Math.Max(fz_eff_c, objGeo.m_fh - fz_eff_c);
            m_fW_eff_z_min = m_fI_eff_z / Math.Max(fy_eff_c, objGeo.m_fb - fy_eff_c);


        }
    }
}
