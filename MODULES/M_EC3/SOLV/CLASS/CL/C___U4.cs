using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using M_BASE;

namespace M_EC3
{
    class C___U4 : CL_EF
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

        // Flanges
        // Upper Flange
        float fSigma_1_fu,
              fSigma_2_fu,
              fPsi_fu,
              fk_Sigma_fu,
              fLambda_rel_p_fu,
              fRho_fu,
              fb_eff_fu,
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
              fb_red_fb,
              fb_em_fb;

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

        public C___U4(CCrSc objCrSc, C_GEO___U objGeo, C_IFO objIFO, C_STR___U objStr, C___U objC__U, C_MAT___U objMat, C_ADD___U objAdd)
        {


        float fA_eff_f,
              fz_eff_c_f,
              fy_eff_c_f,
              fI_eff_y_f,
              fI_eff_z_f;

        // Web
        float fz_eff_c,
              fy_eff_c;

            if (objC__U.m_iClass_f == 4)
            {
             // EN 1993-1-5, Table 4.2
                GetEff_OUT(objStr.m_fSigma_fuA,
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
                                    fb_red_fu,
                                    fb_em_fu,
                                    objMat.BStainlessS);

            // EN 1993-1-5, Table 4.2
                GetEff_OUT(objStr.m_fSigma_fbA,
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
                                    fb_red_fb,
                                    fb_em_fb,
                                    objMat.BStainlessS);

        
            fA_eff_f   = objCrSc.m_fA - (fb_red_fu + fb_red_fb) * objGeo.m_ft_f;

            fz_eff_c_f = (objCrSc.m_fA * objGeo.m_fh / 2.0f -                                           
                          fb_red_fu * objGeo.m_ft_f * objGeo.m_ft_f / 2.0f  -                          
                          fb_red_fb * objGeo.m_ft_f * (objGeo.m_fh - objGeo.m_ft_f / 2.0f)                      
                         ) / fA_eff_f;

            fy_eff_c_f = (objCrSc.m_fA * objCrSc.m_fy_S -                                                
                          fb_red_fu * objGeo.m_ft_f * (objGeo.m_fb - fb_em_fu) -                       
                          fb_red_fb * objGeo.m_ft_f * (objGeo.m_fb - fb_em_fb)                         
                          ) / fA_eff_f;

          
            fI_eff_y_f = objCrSc.m_fI_y +                                                      
                         objCrSc.m_fA * sqr(fz_eff_c_f - objGeo.m_fh / 2.0f) -                          
                         fb_red_fu * (float)Math.Pow(objGeo.m_ft_f, 3) / 12.0f -                          
                         fb_red_fu * objGeo.m_ft_f * sqr(fz_eff_c_f - objGeo.m_ft_f / 2.0f) -          
                         fb_red_fb * (float)Math.Pow(objGeo.m_ft_f, 3) / 12.0f -                          
                         fb_red_fb * objGeo.m_ft_f * sqr(objGeo.m_fh - fz_eff_c_f - objGeo.m_ft_f / 2.0f);      

            fI_eff_z_f = objCrSc.m_fI_z +
                         objCrSc.m_fA * sqr(fy_eff_c_f - objCrSc.m_fy_S) -                               
                         (float)Math.Pow(fb_red_fu, 3) * objGeo.m_ft_f / 12.0f -                          
                         fb_red_fu * objGeo.m_ft_f * sqr(objGeo.m_fb - fy_eff_c_f - fb_em_fu) -        
                         (float)Math.Pow(fb_red_fb, 3) * objGeo.m_ft_f / 12.0f -                          
                         fb_red_fb * objGeo.m_ft_f * sqr(objGeo.m_fb - fy_eff_c_f - fb_em_fb);         
          }
          else    
          {
            fb_red_fu  = 0.0f;
            fb_red_fb  = 0.0f;

            fb_em_fu   = 0.0f;
            fb_em_fb   = 0.0f;
            
            fA_eff_f   = objCrSc.m_fA;

            fz_eff_c_f = objGeo.m_fh / 2.0f;
            fy_eff_c_f = objCrSc.m_fy_S;

            fI_eff_y_f = objCrSc.m_fI_y;
            fI_eff_z_f = objCrSc.m_fI_z;
          }

          if(objC__U.m_iClass_w == 4)
          {
           
            float fSigma_N     = objIFO.FN_Ed / fA_eff_f;
            float fSigma_My_wo = objIFO.FM_y_Ed / fI_eff_y_f * (fz_eff_c_f - objGeo.m_ft_f - objGeo.m_fr);
            float fSigma_My_wu = objIFO.FM_y_Ed / fI_eff_y_f * (objGeo.m_fh - fz_eff_c_f - objGeo.m_ft_f - objGeo.m_fr);
            float fSigma_Mz_wa = objIFO.FM_z_Ed / fI_eff_z_f *  fy_eff_c_f;                 
            float fSigma_Mz_wi = objIFO.FM_z_Ed / fI_eff_z_f * (fy_eff_c_f - objGeo.m_ft_w);         

           
            objStr.m_fSigma_wA = fSigma_N - fSigma_My_wo + Math.Min(fSigma_Mz_wa, fSigma_Mz_wi);  
            objStr.m_fSigma_wB = fSigma_N + fSigma_My_wu + Math.Min(fSigma_Mz_wa, fSigma_Mz_wi);

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
                        fb_red_fu * objGeo.m_ft_f * objGeo.m_ft_f / 2.0f  -                             
                        fb_red_fb * objGeo.m_ft_f * (objGeo.m_fh - objGeo.m_ft_f / 2.0f) -                       
                        fb_red_w  * objGeo.m_ft_w * (fb_em_w + objGeo.m_fr + objGeo.m_ft_f)                      
                       ) / m_fA_eff;  

            fy_eff_c = (objCrSc.m_fA * objCrSc.m_fy_S -                                                   
                        fb_red_fu * objGeo.m_ft_f * (objGeo.m_fb - fb_em_fu) -                          
                        fb_red_fb * objGeo.m_ft_f * (objGeo.m_fb - fb_em_fb) -                          
                        fb_red_w  * objGeo.m_ft_w * objGeo.m_ft_w / 2.0f                                
                        ) / m_fA_eff;

            m_fI_eff_y = objCrSc.m_fI_y +                                                         
                       objCrSc.m_fA * sqr(fz_eff_c - objGeo.m_fh / 2.0f) -                               
                       fb_red_fu * (float)Math.Pow(objGeo.m_ft_f, 3) / 12.0f -                             
                       fb_red_fu * objGeo.m_ft_f * sqr(fz_eff_c - objGeo.m_ft_f / 2.0f) -               
                       fb_red_fb * (float)Math.Pow(objGeo.m_ft_f, 3) / 12.0f -                             
                       fb_red_fb * objGeo.m_ft_f * sqr(objGeo.m_fh - fz_eff_c - objGeo.m_ft_f / 2.0f) -          
                       (float)Math.Pow(fb_red_w, 3) * objGeo.m_ft_w / 12.0f -                              
                       fb_red_w * objGeo.m_ft_w * sqr(fz_eff_c - objGeo.m_ft_f - fb_em_w - objGeo.m_fr);         

            m_fI_eff_z = objCrSc.m_fI_z +                                                         
                       objCrSc.m_fA * sqr(fy_eff_c - objCrSc.m_fy_S) -                                    
                       (float)Math.Pow(fb_red_fu, 3) * objGeo.m_ft_f / 12.0f -                             
                       fb_red_fu * objGeo.m_ft_f * sqr(objGeo.m_fb - fy_eff_c - fb_em_fu) -             
                       (float)Math.Pow(fb_red_fb, 3) * objGeo.m_ft_f / 12.0f -                             
                       fb_red_fb * objGeo.m_ft_f * sqr(objGeo.m_fb - fy_eff_c - fb_em_fb) -             
                       (float)Math.Pow(objGeo.m_ft_w, 3) * fb_red_w / 12.0f -                              
                       objGeo.m_ft_w * fb_red_w * sqr(fy_eff_c - objGeo.m_ft_w / 2.0f);                 
          }
          else   
          {
            fb_red_w = 0.0f;

            m_fA_eff   = fA_eff_f;

            fz_eff_c = fz_eff_c_f;
            fy_eff_c = fy_eff_c_f;

            m_fI_eff_y = fI_eff_y_f;
            m_fI_eff_z = fI_eff_z_f;
          }
  
           float      fA_f_eff  = objAdd.m_fA_f - (fb_red_fu + fb_red_fb) * objGeo.m_ft_f;

           float fA_w_eff = objAdd.m_fA_w - fb_red_w * objGeo.m_ft_w;


           m_fN_pl_eff = fA_f_eff * objMat.m_ff_y_f + fA_w_eff * objMat.m_ff_y_w;

       
          m_fe_Ny = fz_eff_c - objGeo.m_fh / 2.0f;
          m_fe_Nz = fy_eff_c - objCrSc.m_fy_S;
          
     
          m_fW_eff_y_min = m_fI_eff_y / Math.Max(fz_eff_c, objGeo.m_fh - fz_eff_c);
          m_fW_eff_z_min = m_fI_eff_z / Math.Max(fy_eff_c, objGeo.m_fb - Math.Min(fb_red_fu, fb_red_fb) - fy_eff_c);
        }
    }
}
