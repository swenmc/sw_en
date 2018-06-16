using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MATH;

namespace M_EC2.SOLV.CS
{
    class CL_62  // Clause 6.2 Shear
    {
        //float Eq_61______(float fV_Rd_s, float fV_ccd, float fV_td)
        //{
        //    return fV_Rd_s + fV_ccd + fV_td; // Eq. (6.1) fV_Rd
        //}
        //float Eq_62a_____(float fC_Rd_c, float fk, float frho_1, float ff_ck, float fk_1, float fsigma_cp, float fb_w, float fd)
        //{
        //    return (fC_Rd_c * fk * (float)Math.Pow((100f * frho_1 * ff_ck), 1f / 3f) + fk_1 * fsigma_cp) * fb_w * fd; // Eq. (6.2a) fV_Rd_c
        //}
        //double Eq_62b_____(double fv_min, double fk_1, double fsigma_cp, double fb_w, double fd)
        //{
        //    return (fv_min + fk_1 * fsigma_cp) * fb_w * fd; // Eq. (6.2b) fV_Rd_c
        //}
        //double Eq_k_______(double fd)
        //{
        //    if (fd > 0f)
        //        return Math.Min((1f + Math.Sqrt(200f / (fd * 1000f))), 2f); // Eq. k, dd - in [mm]
        //    else
        //        return 0f;
        //}
        //float Eq_rho_1___(float fA_s1, float fb_w, float fd)
        //{
        //    if (fb_w > 0f && fd > 0f)
        //        return Math.Min(fA_s1 / (fb_w * fd), 0.02f); // Eq. rho_1
        //    else
        //        return 0f;
        //}
        //float Eq_sig_cp_62(float fN_Ed, float fA_c, float ff_cd)
        //{
        //    if (fA_c > 0f && ff_cd > 0f)
        //        return Math.Min(Math.Abs(fN_Ed) / fA_c, 0.2f * ff_cd); // Eq. sigma_cp for (6.2)
        //    else
        //        return 0f;
        //}
        //float Eq_64______(float fI, float fb_w, float fS, float ff_ctd, float falpha_l, float fsigma_cp)
        //{
        //    if (fS > 0f && fb_w > 0f && ff_ctd > 0f)
        //        return ((fI * fb_w) / fS) * (float)Math.Sqrt(Math.Pow(ff_ctd, 2f)) + falpha_l * fsigma_cp * ff_ctd; // Eq. V_Rd_c
        //    else
        //        return 0f;
        //}
        //float Eq_aplha_l__(float dI_x, float dI_pt2, bool b_pressed_r)
        //{
        //    if (b_pressed_r)
        //    {
        //        if (dI_pt2 > 0f)
        //            return Math.Min(dI_x / dI_pt2, 1f); // Eq. alpha_l
        //        else
        //            return 0f;
        //    }
        //    else
        //        return 1f; // not presressed reinforcemet
        //}
        //float Eq_sig_cp_64(float dN_Ed, float dA_c)
        //{
        //    if (dA_c > 0f)
        //        return Math.Abs(dN_Ed) / dA_c; // Eq. sigma_cp for (6.4)
        //    else
        //        return 0f;
        //}
        //float Eq_65______(float fV_Ed, float fb_w, float fd, float fnu, float ff_cd)
        //{
        //    if (fb_w > 0f && fd > 0f && fnu > 0f && ff_cd > 0f)
        //        return fV_Ed / (0.5f * fb_w * fd * fnu * ff_cd); // Eq. (6.5) ratio 
        //    else
        //        return 0f;
        //}
        //float Eq_66N_____(float fnu, float ff_ck)
        //{
        //    return 0.6f * (1f - ff_ck / 250e+6f); // Eq. (6.5) nu 
        //}
        //float Eq_67N_____(float f_cot_phi, float f_cot_phi_min /*= 1*/, float f_cot_phi_max /*=2.5*/)
        //{
        //    if (f_cot_phi_min <= f_cot_phi && f_cot_phi <= f_cot_phi_max)
        //        return f_cot_phi; // Eq. (6.7) cot phi
        //    else
        //        return Math.Max(f_cot_phi_min, Math.Min(f_cot_phi, f_cot_phi_max));
        //}







        float Eq_61______(float fV_Rd_s, float fV_ccd, float fV_td)
        {
            return fV_Rd_s + fV_ccd + fV_td; // Eq. (6.1) fV_Rd
        }
        float Eq_62a_____(float fC_Rd_c, float fk, float frho_1, float ff_ck, float fk_1, float fsigma_cp, float fb_w, float fd)
        {
            return (fC_Rd_c * fk * (float)Math.Pow((100f * frho_1 * ff_ck), 1f / 3f) + fk_1 * fsigma_cp) * fb_w * fd; // Eq. (6.2a) fV_Rd_c
        }
        double Eq_62b_____(double fv_min, double fk_1, double fsigma_cp, double fb_w, double fd)
        {
            return (fv_min + fk_1 * fsigma_cp) * fb_w * fd; // Eq. (6.2b) fV_Rd_c
        }
        double Eq_k_______(double fd)
        {
            if (fd > 0f)
                return Math.Min((1f + Math.Sqrt(200f / (fd * 1000f))), 2f); // Eq. k, dd - in [mm]
            else
                return 0f;
        }
        float Eq_rho_1___(float fA_s1, float fb_w, float fd)
        {
            if (fb_w > 0f && fd > 0f)
                return Math.Min(fA_s1 / (fb_w * fd), 0.02f); // Eq. rho_1
            else
                return 0f;
        }
        float Eq_sig_cp_62(float fN_Ed, float fA_c, float ff_cd)
        {
            if (fA_c > 0f && ff_cd > 0f)
                return Math.Min(Math.Abs(fN_Ed) / fA_c, 0.2f * ff_cd); // Eq. sigma_cp for (6.2)
            else
                return 0f;
        }
        float Eq_64______(float fI, float fb_w, float fS, float ff_ctd, float falpha_l, float fsigma_cp)
        {
            if (fS > 0f && fb_w > 0f && ff_ctd > 0f)
                return ((fI * fb_w) / fS) * (float)Math.Sqrt(Math.Pow(ff_ctd, 2f)) + falpha_l * fsigma_cp * ff_ctd; // Eq. V_Rd_c
            else
                return 0f;
        }
        float Eq_aplha_l__(float dI_x, float dI_pt2, bool b_pressed_r)
        {
            if (b_pressed_r)
            {
                if (dI_pt2 > 0f)
                    return Math.Min(dI_x / dI_pt2, 1f); // Eq. alpha_l
                else
                    return 0f;
            }
            else
                return 1f; // not presressed reinforcemet
        }
        float Eq_sig_cp_64(float dN_Ed, float dA_c)
        {
            if (dA_c > 0f)
                return Math.Abs(dN_Ed) / dA_c; // Eq. sigma_cp for (6.4)
            else
                return 0f;
        }
        float Eq_65______(float fV_Ed, float fb_w, float fd, float fnu, float ff_cd)
        {
            if (fb_w > 0f && fd > 0f && fnu > 0f && ff_cd > 0f)
                return fV_Ed / (0.5f * fb_w * fd * fnu * ff_cd); // Eq. (6.5) ratio 
            else
                return 0f;
        }
        float Eq_66N_____(float fnu, float ff_ck)
        {
            return 0.6f * (1f - ff_ck / 250e+6f); // Eq. (6.5) nu 
        }
        float Eq_67N_____(float f_cot_phi, float f_cot_phi_min /*= 1*/, float f_cot_phi_max /*=2.5*/)
        {
            if (f_cot_phi_min <= f_cot_phi && f_cot_phi <= f_cot_phi_max)
                return f_cot_phi; // Eq. (6.7) cot phi
            else
                return Math.Max(f_cot_phi_min, Math.Min(f_cot_phi, f_cot_phi_max));
        }
        float Eq_672_____(float fE_cd_max_equ, float fR_equ)
        {
            return (fE_cd_max_equ + 0.43f * (float)Math.Sqrt(1f - fR_equ)) / 1f;
        }
        //float Eq_673_____(float fE_cd_min_equ, float fE_ce_max_equ)
        //{
        //    return fE_cd_min_equ / fE_ce_max_equ; // fR_equ
        //}
        //float Eq_674_____(float fSigma_cd_min_equ, float ff_cd_fat)
        //{
        //    return fSigma_cd_min_equ / ff_cd_fat; // fE_cd_min_equ
        //}
        //float Eq_675_____(float fSigma_cd_max_equ, float ff_cd_fat)
        //{
        //    return fSigma_cd_max_equ / ff_cd_fat; // fE_cd_max_equ
        //}
        float Eq_676_____(float fk_1, float fBeta_cc, float ft_0, float ff_cd, float ff_ck)
        {
            return fk_1 * fBeta_cc * ft_0 * ff_cd * (1f - (ff_ck / 250f)); // ff_cd_fat
        }

        //--------------------------------------------------------------------------------
        public float Eq_33______(float ff_ct_sp)
        {
            return 0.9f * ff_ct_sp; // Eq. (3.3) f_ct
        }
        public float Eq_38______(float fEpsilon_cd, float fEpsilon_ca)
        {
            return fEpsilon_cd + fEpsilon_ca; // Eq. (3.8) Epsilon_cs
        }
        public float Eq_310_____(float ft, float ft_s, float fh_0)
        {
            return (ft - ft_s) / ((ft - ft_s) + 0.04f * (float)Math.Sqrt((float)Math.Pow(fh_0, 3f))); // Eq. (3.10) Beta_ds(t,t_s)
        }
        public float Eq_312_____(float ff_ck)
        {
            return 2.5f * (ff_ck - 10f) * (float)Math.Pow(10f, -6f); // Eq. (3.12) Epsilon_ca
        }
        public float Eq_314_____(float fk, float fEta)
        {
            return ((fk * fEta - (float)Math.Pow(fEta, 2f)) / 1f + (fk - 2f) * fEta); // Eq. (3.14) Sigma_c/f_cm
        }
        public float Eq_315_____(float fAlpha_cc, float ff_ck, float fGamma_C)
        {
            if (!Math.Equals(fGamma_C, 0f))
                return fAlpha_cc * ff_ck / fGamma_C; // Eq. (3.15) f_cd
            else
                return 0f;
        }
        public float Eq_316_____(float fAlpha_ct, float ff_ctk_0_05, float fGamma_C)
        {
            if (!Math.Equals(fGamma_C, 0f))
                return fAlpha_ct * ff_ctk_0_05 / fGamma_C; // Eq. (3.16) f_ctd
            else
                return 0f;
        }
        public float Eq_323_____(float fh, float ff_ctm)
        {
            return Math.Max((1.6f - (fh / 1000f)) * ff_ctm, ff_ctm); // Eq. (3.23) f_ctm_fl
        }
        public float Eq_324_____(float ff_ck, float fSigma_2)
        {
            return ff_ck * (1.000f + (5.0f * fSigma_2 / ff_ck)); // Eq. (3.24) f_ck_c
        }
        public float Eq_326_____(float fEpsilon_c2, float ff_ck_c, float ff_ck)
        {
            if (!Math.Equals(ff_ck, 0f))
                return fEpsilon_c2 * (float)Math.Pow((ff_ck_c / ff_ck), 2f); // Eq. (3.26) Epsilon_c2_c
            else
                return 0f;
        }
        //----------------------------------------------10---------------------------------------------------------------
        public float Eq_327_____(float fEpsilon_cu2, float fSigma_2, float ff_ck)
        {
            return fEpsilon_cu2 + (0.2f * fSigma_2 / ff_ck); // Eq. (3.27) Epsilon_cu2_c
        }
        public float Eq_41______(float fc_min, float fDelta, float fc_dev)
        {
            return fc_min + (fDelta * fc_dev); // Eq. (4.1) c_nom
        }
        public float Eq_51______(float fTheta_0, float fAlpha_h, float fAlpha_m)
        {
            return fTheta_0 * fAlpha_h * fAlpha_m; // Eq. (5.1) Theta_l
        }
        public float Eq_52______(float fTheta_i, float fl_0)
        {
            return fTheta_i * fl_0 / 2f; // Eq. (5.2) e_i
        }
        public float Eq_54______(float fTheta_i, float fN_b, float fN_a)
        {
            return fTheta_i * (fN_b - fN_a); // Eq. (5.4) H_i
        }
        public float Eq_55______(float fTheta_i, float fN_b, float fN_a)
        {
            return fTheta_i * (fN_b + fN_a) / 2f; // Eq. (5.5) H_i
        }
        public float Eq_56______(float fTheta_i, float fN_a)
        {
            return fTheta_i * fN_a; // Eq. (5.6) H_i
        }
        public float Eq_58______(float fl_n, float fa_1, float fa_2)
        {
            return fl_n + fa_1 + fa_2; // Eq. (5.8) l_eff
        }
        public float Eq_59______(float fF_Ed_sup, float ft)
        {
            return fF_Ed_sup * ft / 8f; // Eq. (5.9) Delta M_Ed
        }
        public float Eq_511N____(float fLambda)
        {
            return (float)Math.Sqrt(fLambda / 3f); // Eq. (5.11N) k_Lambda
        }
        //----------------------------------------------------20----------------------------------------
        public float Eq_512N____(float fM_Sd, float fV_Sd, float fd)
        {
            return fM_Sd / (fV_Sd * fd); // Eq. (5.12N) Lambda
        }
        public float Eq_513N____(float fA, float fB, float fC, float fn)
        {
            return 20f * fA * fB * fC / (float)Math.Sqrt(fn); // Eq. (5.13N) Lambda_lim
        }
        public float Eq_514_____(float fl_0, float fi)
        {
            return fl_0 / fi; // Eq. (5.14) Lambda
        }
        public float Eq_520_____(float fE_cm, float fGamma_cE)
        {
            return fE_cm / fGamma_cE; // Eq. (5.20) E_cd
        }
        public float Eq_521_____(float fK_c, float fE_cd, float fI_c, float fK_s, float fE_s, float fI_s)
        {
            return (fK_c * fE_cd * fI_c) + (fK_s * fE_s * fI_s); // Eq. (5.21) EI
        }
        public float Eq_526_____(float fPhi_ef)
        {
            return 0.3f / (1f + (0.5f * fPhi_ef)); // Eq. (5.26) K_c
        }
        public float Eq_527_____(float fE_cd, float fPhi_ef)
        {
            return fE_cd / (1f + fPhi_ef); // Eq. (5.27) E_cd_eff
        }
        public float Eq_528_____(float fM_0Ed, float fBeta, float fN_B, float fN_Ed)
        {
            return fM_0Ed * (1f + (fBeta / (fN_B / fN_Ed) - 1f)); // Eq. (5.28) M_ed
        }
        public float Eq_529_____(float fc_0)
        {
            if (!Math.Equals(fc_0, 0f))
                return (float)Math.Pow((float)Math.PI, 2f) / fc_0; // Eq. (5.29) Beta
            else
                return 0f;
        }
        public float Eq_530_____(float fM_0Ed, float fN_Ed, float fN_B)
        {
            return fM_0Ed / (1f - (fN_Ed / fN_B)); // Eq. (5.30) M_Ed
        }
        //-------------------------------------------------30-----------------------------------------
        public float Eq_531_____(float fM_0Ed, float fM_2)
        {
            return fM_0Ed + fM_2; // Eq. (5.31) M_Ed
        }
        public float Eq_534_____(float fK_r, float fK_Phi, float fr_0)
        {
            if (!Math.Equals(fr_0, 0f))
                return fK_r * fK_Phi * 1f / fr_0; // Eq. (5.34) 1/r
            else
                return 0f;
        }
        public float Eq_535_____(float fh, float fi_s)
        {
            return (fh / 2f) + fi_s; // Eq. (5.35) d
        }
        public float Eq_541_____(float fAlpha_p, float fSigma_p_max)
        {
            return fAlpha_p * fSigma_p_max; // Eq. (5.41) P_max
        }
        public float Eq_619_____(float fV_Ed, float fA_sw, float ff_ywd, float fAlpha)
        {
            return fV_Ed / (fA_sw * ff_ywd * (float)Math.Sin(fAlpha)); // Eq. (6.19) Design ratio
        }
        public float Eq_624_____(float fBeta, float fV_Ed, float fz, float fb_i)
        {
            return fBeta * fV_Ed / (fz * fb_i); // Eq. (6.24) V_Edi
        }
        public float Eq_626_____(float fT_Ed, float fA_k)
        {
            return fT_Ed / (2f * fA_k); // Eq. (6.26) Tau_t_i t_ef_i
        }
        public float Eq_627_____(float fTau_t_i, float ft_ef_i, float fz_i)
        {
            return fTau_t_i * ft_ef_i * fz_i; // Eq. (5.27) V_Ed_i
        }
        public float Eq_630_____(float fNu, float fAlpha_cw, float ff_cd, float fA_k, float ft_ef_i, float fTheta)
        {
            return 2f * fNu * fAlpha_cw * ff_cd * fA_k * (float)Math.Sin(fTheta) * (float)Math.Cos(fTheta); // Eq. (6.30) T_Rd_max
        }
        public float Eq_632_____(float fd_y, float fd_z)
        {
            return (fd_y + fd_z) / 2f; // Eq. (6.32) d_eff
        }
        //---------------------------------------------------40-----------------------------------------------------
        public float Eq_633_____(float fd, float fJota_H, float fc)
        {
            return (2f * fd) + fJota_H + (0.5f * fc); // Eq. (6.33) r_cont
        }
        public float Eq_637_____(float fd, float fh_H, float fc)
        {
            return (2f * (fd + fh_H)) + (0.5f * fc); // Eq. (6.37) r_cont_int
        }
        public float Eq_638_____(float fBeta, float fV_Ed, float fu_i, float fd)
        {
            if (!Math.Equals(fu_i, 0f) && !Math.Equals(fd, 0f))
                return fBeta * (fV_Ed / (fu_i * fd)); // Eq. (6.38) v_Ed
            else
                return 0f;
        }
        public float Eq_639_____(float fk, float fM_Ed, float fV_Ed, float fu_1, float fW_1)
        {
            return 1f + (fk * (fM_Ed / fV_Ed) * (fu_1 / fW_1)); // Eq. (3.9) Beta
        }
        public float Eq_641_____(float fc_1, float fc_2, float fd)
        {
            return ((float)Math.Pow(fc_1, 2f) / 2f) + (fc_1 * fc_2) + (4f * fc_2 * fd) + (16f * fd) + (2f * (float)Math.PI * fd * fc_1); // Eq. (6.41)
        }
        public float Eq_642_____(float fe, float fD, float fd)
        {
            return 1f + (0.6f * (float)Math.PI * (fe / (fD + (4f * fd)))); // Eq. (6.41) Beta
        }
        public float Eq_643_____(float fe_y, float fe_z, float fb_z, float fb_y)
        {
            return 1f + (1.8f * (float)Math.Sqrt((float)Math.Pow((fe_y / fb_z), 2f) + (float)Math.Pow((fe_z / fb_y), 2f))); // Eq. (6.43) Beta
        }
        public float Eq_645_____(float fc_2, float fc_1, float fd)
        {
            return ((float)Math.Pow(fc_2, 2f) / 4f) + (fc_1 * fc_2) + (4f * fc_1 * fd) + (8f * (float)Math.Pow(fd, 2f)) + ((float)Math.PI * fd * fc_2); // Eq. (6.45) W_1
        }
        public float Eq_649_____(float fV_Ed_red, float fu, float fd)
        {
            return fV_Ed_red / (fu * fd); // Eq. (6.49) v_Ed
        }
        public float Eq_651_____(float fV_Ed_red, float fu, float fd, float fk, float fM_Ed, float fW)
        {
            if (!Math.Equals(fu, 0f) && !Math.Equals(fd, 0f))
                return (fV_Ed_red / fu * fd) * (1f + (fk * ((fM_Ed * fu) / (fV_Ed_red * fW)))); // Eq. (6.51) v_Ed
            else
                return 0f;
        }
        //-----------------------------------------------50----------------------------------------------------------
        public float Eq_654_____(float fBeta, float fV_Ed, float fv_Rd_c, float fd)
        {
            return fBeta * fV_Ed / (fv_Rd_c * fd); // Eq. (6.54) u_out_ef
        }
        public float Eq_656_____(float ff_cd, float fNu)
        {
            return 0.6f * fNu * ff_cd; // Eq. (6.56) Sigma_Rd_max
        }
        public float Eq_657N____(float ff_ck)
        {
            return 1f - (ff_ck / 250f); // Eq. (6.57N) Nu
        }
        public float Eq_658_____(float fb, float fa, float fF)
        {
            if (!Math.Equals(fb, 0f))
                return (1f / 4f) * ((fb - fa) / fb) * fF; // Eq. (6.58) T
            else
                return 0f;
        }
        public float Eq_659_____(float fa, float fh, float fF)
        {
            return (1f / 4f) * (1f - (0.7f * (fa / fh))) * fF; // Eq. (6.59) T
        }
        public float Eq_660_____(float fk_1, float fNu, float ff_cd)
        {
            return fk_1 * fNu * ff_cd; // Eq. (6.60) Sigma_Rd_max
        }
        public float Eq_661_____(float fk_2, float fNu, float ff_cd)
        {
            return fk_2 * fNu * ff_cd; // Eq. (6.61) Sigma_Rd_max
        }
        public float Eq_662_____(float fk_3, float fNu, float ff_cd)
        {
            return fk_3 * fNu * ff_cd; // Eq. (6.62) Sigma_Rd_max
        }
        public float Eq_664_____(float fA_S, float fA_P, float fXi, float fPhi_S, float fPhi_P)
        {
            return (fA_S + fA_P) / (fA_S + (fA_P * (float)Math.Sqrt(fXi * (fPhi_S / fPhi_P)))); // Eq. (6.64) Eta
        }
        public float Eq_673_____(float fE_cd_min_equ, float fE_cd_max_equ)
        {
            if (!Math.Equals(fE_cd_max_equ, 0f))
                return fE_cd_min_equ / fE_cd_max_equ; // Eq. (6.73) R_equ
            else
                return 0f;
        }
        //-----------------------------------------------------60---------------------------------------------
        public float Eq_674_____(float fSigma_cd_min_equ, float ff_cd_fat)
        {
            if (!Math.Equals(ff_cd_fat, 0f))
                return fSigma_cd_min_equ / ff_cd_fat; // Eq. (6.74) E_cd_min_equ
            else
                return 0f;
        }
        public float Eq_675_____(float fSigma_cd_max_equ, float ff_cd_fat)
        {
            if (!Math.Equals(ff_cd_fat, 0f))
                return fSigma_cd_max_equ / ff_cd_fat; // Eq. (6.75) E_cd_max_equ
            else
                return 0f;
        }
        public float Eq_677_____(float fSigma_c_max, float ff_cd_fat, float fSigma_c_min)
        {
            return (fSigma_c_max / ff_cd_fat) / (0.5f + (0.45f * (fSigma_c_min / ff_cd_fat))); // Eq. (6.77) Design Ratio
        }
        public float Eq_71______(float fk_c, float fk, float ff_ct_eff, float fA_ct)
        {
            return fk_c * fk * ff_ct_eff * fA_ct; // Eq. (7.1) A_s_minSigma_s
        }
        public float Eq_74______(float fN_Ed, float fb, float fh)
        {
            if (!Math.Equals(fb, 0f) && !Math.Equals(fh, 0f))
                return fN_Ed / (fb * fh); // Eq. (7.4) Sigma_c
            else
                return 0f;
        }
        public float Eq_75______(float fXi, float fPhi_s, float fPhi_p)
        {
            if (!Math.Equals(fPhi_p, 0f))
                return (float)Math.Sqrt(fXi * (fPhi_s / fPhi_p)); // Eq. (7.5) 
            else
                return 0f;
        }
        public float Eq_78______(float fs_r_max, float fEpsilon_sm, float fEpsilon_cm)
        {
            return fs_r_max * (fEpsilon_sm - fEpsilon_cm); // Eq. (7.8) w_k
        }
        public float Eq_711_____(float fk_3, float fc, float fk_1, float fk_2, float fk_4, float fPhi, float fRho_p_eff)
        {
            if (!Math.Equals(fRho_p_eff, 0f))
                return (fk_3 * fc) + ((fk_1 * fk_2 * fk_4 * fPhi) / fRho_p_eff); // Eq. (7.11) s_r_max
            else
                return 0f;
        }
        public float Eq_712____(float fn_1, float fPhi_1, float fn_2, float fPhi_2)
        {
            return ((fn_1 * (float)Math.Pow(fPhi_1, 2f)) + ((fn_2 * (float)Math.Pow(fPhi_2, 2f))) / ((fn_1 * fPhi_1) + (fn_2 * fPhi_2))); // Eq. (7.12) Phi_eq
        }
        public float Eq_713____(float fEpsilon_1, float fEpsilon_2)
        {
            if (!Math.Equals(fEpsilon_1, 0f))
                return (fEpsilon_1 + fEpsilon_2) / 2f * fEpsilon_1; // Eq. (7.13) k_!2
            else
                return 0f;
        }
        //---------------------------------------------------70------------------------------------------------------
        public float Eq_714_____(float fh, float fx)
        {
            return 1.3f * (fh - fx); // Eq. (7.14) s_r_max
        }
        public float Eq_715_____(float fTheta, float fs_r_max_y, float fs_r_max_z)
        {
            return 1f / (((float)Math.Cos(fTheta) / fs_r_max_y) + ((float)Math.Sin(fTheta) / fs_r_max_z)); // Eq. (7.15) s_r_max
        }
        public float Eq_717_____(float ff_yk, float fA_s_req, float fA_s_prov)
        {
            return 500f / (ff_yk * fA_s_req / fA_s_prov); // Eq. (7.17) 310/Sigma_s
        }
        public float Eq_719_____(float fBeta, float fSigma_sr, float fSigma_s)
        {
            return 1f - (fBeta * (float)Math.Pow((fSigma_sr / fSigma_s), 2f)); // Eq. (7.19) Zeta
        }
        public float Eq_721_____(float fEpsilon_cs, float fAlpha_e, float fS, float fI)
        {
            if (!Math.Equals(fI, 0f))
                return fEpsilon_cs * fAlpha_e * (fS / fI); // Eq. (7.21) 1/r_cs
            else
                return 0f;
        }
        public float Eq_81______(float fPhi_m_min, float fF_bt, float fa_b, float fPhi, float ff_cd)
        {
            return fPhi_m_min / ((fF_bt * ((1f / fa_b) + (1f / (2f * fPhi)) / ff_cd))); // Eq. (8.1) Design ratio
        }
        public float Eq_82______(float fEta_1, float fEta_2, float ff_ctd)
        {
            return 2.25f * fEta_1 * fEta_2 * ff_ctd; // Eq. (8.2) f_bd
        }
        public float Eq_83______(float fPhi, float fSigma_sd, float ff_bd)
        {
            if (!Math.Equals(ff_bd, 0f))
                return (fPhi / 4f) * (fSigma_sd / ff_bd); // Eq. (8.3) l_b_rqd
            else
                return 0f;
        }
        public float Eq_812_____(float fA_s, float fn_1)
        {
            return 0.25f * fA_s * fn_1; // Eq. (8.12) A_sh
        }
        public float Eq_813_____(float fA_s, float fn_2)
        {
            return 0.25f * fA_s * fn_2; // Eq. (8.13) A_sv
        }
        //--------------------------------------------------80-------------------------------------------------
        public float Eq_816_____(float fAlpha_1, float fAlpha_2, float fPhi, float fSigma_pm0, float ff_bpt)
        {
            if (!Math.Equals(ff_bpt, 0f))
                return (fAlpha_1 * fAlpha_2 * fPhi * fSigma_pm0) / ff_bpt; // Eq. (8.16) l_pt
            else
                return 0f;
        }
        public float Eq_817_____(float fl_pt)
        {
            return 0.8f * fl_pt; // Eq. (8.17) l_pt1
        }
        public float Eq_818_____(float fl_pt2)
        {
            return 1.2f * fl_pt2; // Eq. (8.18) l_pt2
        }
        public float Eq_819_____(float fl_pt, float fd)
        {
            return (float)Math.Sqrt((float)Math.Pow(fl_pt, 2f) + (float)Math.Pow(fd, 2f)); // Eq. (8.19) l_disp
        }
        public float Eq_820_____(float fEta_p2, float fEta_1, float ff_ctd)
        {
            return fEta_p2 * fEta_1 * ff_ctd; // Eq. (8.20) f_bpd
        }
        public float Eq_91N_____(float ff_ctm, float ff_yk, float fb_t, float fd)
        {
            if (!Math.Equals(ff_yk, 0f))
                return 0.26f * (ff_ctm / ff_yk) * fb_t * fd; // Eq. (9.1N) A_s_min
            else
                return 0f;
        }
        public float Eq_94______(float fA_sw, float fs, float fb_w, float fAlpha)
        {
            if (!Math.Equals(fs, 0f) && !Math.Equals(fb_w, 0f))
                return fA_sw / (fs * fb_w * (float)Math.Sin(fAlpha)); // Eq. (9.4) Rgo_w
            else
                return 0f;
        }
        public float Eq_95N_____(float ff_ck, float ff_yk)
        {
            if (!Math.Equals(ff_yk, 0f))
                return (0.08f * (float)Math.Sqrt(ff_ck)) / ff_yk; // Eq. (9.5N) Rho_w_min
            else
                return 0f;
        }
        public float Eq_912N____(float fN_Ed, float ff_yd)
        {
            if (!Math.Equals(ff_yd, 0f))
                return (0.10f * fN_Ed) / ff_yd; // Eq. (9.12N) A_s_min
            else
                return 0f;
        }
        public float Eq_913_____(float fR, float fz_e, float fz_i)
        {
            if (!Math.Equals(fz_i, 0f))
                return (fR * fz_e) / fz_i; // Eq. (9.13) F_s
            else
                return 0f;
        }
        //--------------------------------------------------90-------------------------------------------
        public float Eq_914_____(float fc, float fh, float fN_Ed)
        {
            return 0.25f * (1f - (fc / fh)) * fN_Ed; // Eq. (9.14) F_s
        }
        public float Eq_104_____(float fq_Ed, float fb_e)
        {
            return fq_Ed * fb_e / 3f; // Eq. (10.4) v_Ed
        }
        public float Eq_105_____(float ft, float fh, float fF_Ed, float ff_yd)
        {
            if (!Math.Equals(fh, 0f) && !Math.Equals(ff_yd, 0f))
                return 0.25f * (ft / fh) * fF_Ed / ff_yd; // Eq. (10.5) A_s
            else
                return 0f;
        }
        public float Eq_111_____(float fRho)
        {
            return 0.40f + (0.60f * fRho) / 2200f; // Eq. (11.1) Eta_1
        }
        public float Eq_112_____(float fRho)
        {
            return (float)Math.Pow((fRho / 2200f), 2f); // Eq. (11.2) Eta_E
        }
        public float Eq_11315___(float fAlpha_lcc, float ff_lck, float fGamma_c)
        {
            if (!Math.Equals(fGamma_c, 0f))
                return fAlpha_lcc * ff_lck / fGamma_c; // Eq. (11.3.15) f_lcd
            else
                return 0f;
        }
        public float Eq_11316___(float fAlpha_lct, float ff_lctk, float fGamma_c)
        {
            if (!Math.Equals(fGamma_c, 0f))
                return fAlpha_lct * ff_lctk / fGamma_c; // Eq. (11.3.16) f_lctd
            else
                return 0f;
        }
        public float Eq_11324___(float ff_lck, float fk, float fSigma_2)
        {
            return ff_lck * (1.0f + (fk * fSigma_2 / ff_lck)); // Eq. (11.3.24) f_lck_c
        }
        public float Eq_1166N___(float fEta_1, float ff_lck)
        {
            return 0.5f * fEta_1 * (1f - (ff_lck / 250f)); // Eq. (11.6.6N) Nu_1
        }
        public float Eq_121_____(float fAlpha_ct, float ff_ctk_0_05, float fGamma_c)
        {
            if (!Math.Equals(fGamma_c, 0f))
                return fAlpha_ct * ff_ctk_0_05 / fGamma_c; // Eq. (12.1) f_ctd
            else
                return 0f;
        }
        //------------------------------------------------------100-----------------------------------------------
        public float Eq_123_____(float fN_Ed, float fA_cc)
        {
            if (!Math.Equals(fA_cc, 0f))
                return fN_Ed / fA_cc; // Eq. (12.3) Sigma_cp
            else
                return 0f;
        }
        public float Eq_124_____(float fk, float fV_Ed, float fA_cc)
        {
            if (!Math.Equals(fA_cc, 0f))
                return fk * fV_Ed / fA_cc; // Eq. (12.4) Tau_cp
            else
                return 0f;
        }
        public float Eq_129_____(float fBeta, float fl_w)
        {
            return fBeta * fl_w; // Eq. (12.9) l_0
        }
        public float Eq_1210____(float fb, float fh_w, float ff_cd, float fPhi)
        {
            return fb * fh_w * ff_cd * fPhi; // Eq. (12.10) N_Rd
        }
        public float Eq_1212____(float fe_o, float fe_i)
        {
            return fe_o + fe_i; // Eq. (12.12) e_tot
        }
        public float Eq_B4______(float ff_cm)
        {
            if (!Math.Equals(ff_cm, 0f))
                return 16.8f / (float)Math.Sqrt(ff_cm); // Eq. (B.4)
            else
                return 0f;
        }
        public float Eq_B6______(float fA_c, float fu)
        {
            if (!Math.Equals(fu, 0f))
                return (2f * fA_c) / fu; // Eq. (B.6)
            else
                return 0f;
        }
        public float Eq_B12_____(float fR, float fH, float fH_0)
        {
            return 1.55f * (1f - (float)Math.Pow(((fR * fH) / (fR * fH_0)), 3f)); // Eq. (B.12) Beta_RH
        }
        public float Eq_C3______(float fM, float fC_v, float fa)
        {
            return fM / (fC_v + fa); // Eq. (C.3) Design ratio
        }
        public float Eq_H1______(float fF_V_Ed, float fF_V_BB)
        {
            return fF_V_Ed / (0.1f * fF_V_BB); // Eq. (H.1) Design ratio
        }
        //-----------------------------------------------------110----------------------------------------------------
        public float Eq_H4______(float fn_s, float fk)
        {
            return 7.8f * (fn_s / (fn_s + 1.6f)) * (1f / (1f + (0.7f * fk))); // Eq. (H.4) Xi
        }
        public float Eq_H7_____(float fF_H_0Ed, float fF_V_Ed, float fF_V_B)
        {
            return fF_H_0Ed / (1f - (fF_V_Ed / fF_V_B)); // Eq. (H.7) F_H_Ed
        }
        public float Eq_H8______(float fF_H_0Ed, float fF_H_1Ed)
        {
            return fF_H_0Ed / (1f - (fF_H_1Ed / fF_H_0Ed)); // Eq. (H.8) F_H_Ed
        }

        // vypis znakov greckej abecedy - v rovniciach pouzivat anglicky nazov namiesto symbolu
        string[,] aGreekAlphabet = 
{
    {"Α","α","Alpha     "},	
    {"Β","β","Beta	    "},
    {"Γ","γ","Gamma	    "},
    {"Δ","δ","Delta	    "},
    {"Ε","ε","Epsilon   "},	
    {"Ζ","ζ","Zeta	    "},
    {"Η","η","Eta	    "},
    {"Θ","θ","Theta	    "},
    {"Ι","ι","Iota	    "},
    {"Κ","κ","Kappa	    "},
    {"Λ","λ","Lambda	"},
    {"Μ","μ","Mu	    "},
    {"Ν","ν","Nu	    "},
    {"Ξ","ξ","Xi	    "},
    {"Ο","ο","Omicron	"},
    {"Π","π","Pi	    "},
    {"Ρ","ρ","Rho  	    "},
    {"Σ","σ","Sigma	    "},
    {"Τ","τ","Tau	    "},
    {"Υ","υ","Upsilon	"},
    {"Φ","φ","Phi	    "},
    {"Χ","χ","Chi	    "},
    {"Ψ","ψ","Psi	    "},
    {"Ω","ω","Omega     "}
};


    }
}
