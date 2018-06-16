using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MATH;

namespace M_EC1.EC_1_4
{
    public class EC_1_4
    {
        public float Eq_41______(float fc_dir, float fc_season, float fv_b_0)
        {
            return fc_dir * fc_season * fv_b_0; // Eq. (4.1) // fv_b
        }
        public float Eq_42______(float fK, float fp, float fn)
        {
            return (float)Math.Pow((1f - fK * MathF.Ln(-MathF.Ln(1f - fp))) / (1f - fK * MathF.Ln(-MathF.Ln(0.98f))), fn); // Eq. (4.2) // fc_prob
        }
        public float Eq_43______(float fc_r_z, float fc_0_z, float fv_b)
        {
            return fc_r_z * fc_0_z * fv_b; // Eq. (4.3) // fv_m
        }
        public float Eq_44______(float fz_min, float fz_max, float fz, float fz_0, float fk_r, float fc_r_z_min)
        {
            // Eq. (4.4) // fc_r_z

            if (fz_min <= fz && fz <= fz_max)
                return fk_r * MathF.Ln(fz / fz_0);
            else if (fz < fz_min)
                return fc_r_z_min;
            else
                return 0f;
        }
        public float Eq_45______(float fz_0, float fz_0_II)
        {
            return 0.19f * (float)Math.Pow((fz_0 / fz_0_II), 0.07f); // Eq. (4.5) // fk_r 
        }
        public float Eq_46______(float fk_r, float fv_b, float fk_l)
        {
            return fk_r * fv_b * fk_l; // Eq. (4.6) //  fSigma_v
        }
        public float Eq_47______(float fk_l, float fc_0_z_min, float fz_min, float fz_0)
        {
            return fk_l / (fc_0_z_min * MathF.Ln(fz_min / fz_0)); // Eq. (4.7) // fl_v_z_min
        }
        public float Eq_48______(float fl_v_z, float fRho_air, float fv_m)
        {
            return (1f + 7f * fl_v_z) * 0.5f * fRho_air * (float)MathF.Pow2(fv_m); // Eq. (4.8) // fq_p_z
        }
        public float Eq_49______(float fq_p_z, float fq_b)
        {
            return fq_p_z / fq_b; // Eq. (4.9) // fC_e_z
        }
        public float Eq_410_____(float fRho_air, float fv_b)
        {
            return 0.5f * fRho_air * MathF.Pow2(fv_b); // Eq. (4.10) // fq_b
        }
        public float Eq_51________(float fq_p, float fc_pe)
        {
            return fq_p * fc_pe; // Eq (5.1) // fw_e
        }
        public float Eq_52________(float fq_p, float fc_pi)
        {
            return fq_p * fc_pi; // Eq (5.2) // fw_i
        }
        public float Eq_53________(float fc_s, float fc_d, float fc_f, float fq_p, float fA_ref)
        {
            return fc_s * fc_d * fc_f * fq_p * fA_ref; //Eq (5.3) // fF_w 
        }
        public float Eq_54________(float fc_s, float fc_d, float fSigma_c_f, float fq_p, float fA_ref)
        {
            return fc_s * fc_d * fSigma_c_f * fq_p * fA_ref; // Eq (5.4) // fF_w
        }
        public float Eq_55________(float fc_s, float fc_d, float fSigma_w_e, float fA_ref)
        {
            return fc_s * fc_d * fSigma_w_e * fA_ref; // Eq (5.5) // fF_w_e
        }
        public float Eq_56________(float fSigma_w_i, float fA_ref)
        {
            return fSigma_w_i * fA_ref; // Eq (5.6) // fF_w_i
        }
        public float Eq_57________(float fc_fr, float fq_p, float fA_f)
        {
            return fc_fr * fq_p * fA_f; // Eq (5.7) // fF_fr 
        }
        public float Eq_61________(float fk_p, float fl_v, float fB_2, float fR_2)
        {
            return (1f + 2f * fk_p * fl_v * MathF.Sqrt(fB_2 + fR_2)) / 1f + 7f * fl_v; // Eq (6.1) // fc_s_c_d 
        }
        public float Eq_62________(float fk_p, float fl_v, float fB_2)
        {
            return (1f + 2f * fk_p * fl_v * MathF.Sqrt(fB_2)) / (1f + 7f * fl_v); // Eq (6.2) // fc_s
        }
        public float Eq_63________(float fk_v, float fl_v, float fB_2, float fR_2)
        {
            return (1f + 2f * fk_v * fl_v * MathF.Sqrt(fB_2 + fR_2)) / (1f + 7f * fl_v * MathF.Sqrt(fB_2)); // Eq (6.3) // fc_d 
        }
        public float Eq_71________(float fc_pe)
        {
            return 0.75f * fc_pe; // Eq (7.1) // fc_pi
        }
        public float Eq_72________(float fc_pe)
        {
            return 0.90f * fc_pe; // Eq (7.2) // fc_pi
        }
        public float Eq_76________(float fPsi_s, float fc_p_net)
        {
            return fPsi_s * fc_p_net; // Eq (7.6) // fc_p_net_s
        }
        public float Eq_79________(float fc_f_0, float fPsi_r, float fPsi_Lambda)
        {
            return fc_f_0 * fPsi_r * fPsi_Lambda; // Eq (7.9) // fc_f
        }
        public float Eq_710________(float fl, float fb)
        {
            return fl * fb; // Eq (7.10) // fA_ref
        }
        public float Eq_711________(float fc_f_0, float fPsi_Lambda)
        {
            return fc_f_0 * fPsi_Lambda; // Eq (7.11) // fc_f
        }
        public float Eq_712________(float fl, float fb)
        {
            return fl * fb; // Eq (7.12) // fA_ref_x
        }
        public float Eq_713________(float fc_f_0, float fPsi_Lambda)
        {
            return fc_f_0 * fPsi_Lambda; // Eq (7.13) // fc_f
        }
        public float Eq_714________(float fl, float fb)
        {
            return fl * fb; // Eq (7.14) // fA_ref
        }
        public float Eq_715________(float fb, float fv, float fNu)
        {
            return fb * fv / fNu; // Eq (7.15) // fRe
        }
        public float Eq_716_________(float fc_p_0, float fPsi_Lambda_Alpha)
        {
            return fc_p_0 * fPsi_Lambda_Alpha; // Eq (7.16) // fc_pe 
        }
        public float Eq_718________(float fl, float fb)
        {
            return fl * fb; // Eq (7.18) // fA_ref
        }
        public float Eq_719_________(float fc_f_0, float fPsi_Lambda)
        {
            return fc_f_0 * fPsi_Lambda; // Eq (7.19) // fc_f
        }
        public float Eq_720________(float fl, float fb)
        {
            return fl * fb; // Eq (7.20) // fA_ref
        }
        public float Eq_721________(float fc_f_0, float fPsi_Lambda, float fK)
        {
            return fc_f_0 * fPsi_Lambda * fK;  // Eq (7.21) // fc_f
        }
        public float Eq_723________(float fPi, float fb)
        {
            return fPi * MathF.Pow2(fb) / 4f; // Eq (7.23) // fA_ref 
        }
        public float Eq_724________(float fz_g, float fb)
        {
            return fz_g + fb / 2f; // (7.24) // fz_e 
        }
        public float Eq_725_______(float fc_f_0, float fPsi_Lambda)
        {
            return fc_f_0 * fPsi_Lambda; // Eq (7.25) // fc_f 
        }
        public float Eq_726________(float fA, float fA_c)
        {
            return fA / fA_c; // Eq (7.26) // fPhi 
        }
        public float Eq_728________(float fA, float fA_c)
        {
            return fA / fA_c; // Eq (7.28) // fPhi 
        }
        public float Eq_82________(float fRho, float fv_b, float fc, float fA_ref_x)
        {
            return 0.5f * fRho * MathF.Pow2(fv_b) * fc * fA_ref_x; // Eq (8.2) // fF_w
        }
        public float Eq_83________(float fb, float fl)
        {
            return fb * fl; // Eq (8.3) // fA_ref_z
        }
        public float Eq_A5________(float fz, float fL_e)
        {
            return 0.1552f * MathF.Pow4(fz / fL_e) - 0.8575f * MathF.Pow3(fz / fL_e) + 1.8133f * MathF.Pow2(fz / fL_e) - 1.9115f * (fz / fL_e) + 1.0124f; // Eq (A.5) // fA
        }
        public float Eq_A6________(float fz, float fL_e)
        {
            return 0.3542f * MathF.Pow2(fz / fL_e) - 1.0577f * (fz / fL_e) + 2.6456f; // Eq (A.6) // fB
        }
        public float Eq_A8________(float fz, float fL_e)
        {
            return -1.3420f * MathF.Pow3((float)(float)Math.Log10(fz / fL_e)) - 0.8222f * MathF.Pow2((float)Math.Log10(fz / fL_e)) + 0.4609f * (float)Math.Log10(fz / fL_e) - 0.0791f; // Eq (A.8) // fA
        }
        public float Eq_A9________(float fz, float fL_e)
        {
            return -1.0196f * MathF.Pow3((float)(float)Math.Log10(fz / fL_e)) - 0.8910f * MathF.Pow2((float)Math.Log10(fz / fL_e)) + 0.5343f * (float)(float)Math.Log10(fz / fL_e) - 0.1156f; // Eq (A.9) // fB
        }
        public float Eq_A10_______(float fz, float fL_e)
        {
            return 0.8030f * MathF.Pow3((float)Math.Log10(fz / fL_e)) + 0.4236f * MathF.Pow2((float)Math.Log10(fz / fL_e)) - 0.5738f * (float)Math.Log10(fz / fL_e) + 0.1606f; // Eq (A.10) // fC
        }
        public float Eq_A12_______(float fz, float fL_e)
        {
            return 0.1552f * MathF.Pow4(fz / fL_e) - 0.8575f * MathF.Pow3(fz / fL_e) + 1.8133f * MathF.Pow2(fz / fL_e) - 1.9115f * (fz / fL_e) + 1.0124f; // Eq (A.12) // fA
        }
        public float Eq_A13_______(float fz, float fL_e)
        {
            return -0.3056f * MathF.Pow2(fz / fL_e) + 1.0212f * (fz / fL_e) - 1.7637f; // Eq (A.13) // fB
        }
        public float Eq_B1________(float fz, float fz_min, float fL, float fL_t, float fz_t, float fL_z_min)
        {
            // Eq (B.1) // fL_z
            if (fz >= fz_min)
                return fL_t * (fz / fz_t);
            else /*if (fz < fz_min)*/
                return fL_z_min;
        }
        public float Eq_B2________(float fn, float fS_V, float fSigma_V)
        {
            if (MathF.d_equal(fSigma_V, 0f))
                return fn * fS_V / MathF.Pow2(fSigma_V);
            else
                return 0f;
        }
        public float Eq_B2________(float ff_L)
        {
            return 6.8f * ff_L / (1f + 10.2f * (float)Math.Pow(ff_L, 5f / 3f));
        }
        public float Eq_B3________(float fb, float fh, float fL)
        {
            return 1f / (1f + 0.9f * (float)Math.Pow((fb + fh) / fL, 0.63f)); // Eq (B.3) // fB
        }
        //public float Eq_B4
        public float Eq_B5________(float fn_1_x, float fR, float fB, float fv)
        {
            return Math.Max(fn_1_x * MathF.Sqrt(MathF.Pow2(fR / fB + fR)), 0.08f); // Eq (B.5) // fv 
        }
        public float Eq_B6________(float fS_L, float fR_h, float fR_b, float fDelta)
        {
            return (MathF.fPI / (2f * fDelta)) * fS_L * fR_h * fR_b; // Eq (B.6) // fR
        }
        public float Eq_B9________(float fN_g)
        {
            return 0.7f * MathF.Pow2((float)Math.Log10(fN_g)) - 17.4f * (float)Math.Log10(fN_g) + 100f; // Eq (B.9) // fDelta_S / fS_k
        }
        public float Eq_B10_______(float fc_f, float fRho, float fb, float fl_v, float fv_m, float fR, float fK_x, float fPhi_1_x, float fm_1_x)
        {
            return ((fc_f * fRho * fb * fl_v * MathF.Pow2(fv_m)) / fm_1_x) * fR * fK_x * fPhi_1_x; // Eq (B.10) // fSigma_a_x
        }
        public float Eq_C1________(float fb, float fL, float fh)
        {
            return 1f / (1f + 1.5f * MathF.Sqrt(MathF.Pow2(fb / fL) * MathF.Pow2(fh / fL) + MathF.Pow2((fb / fL) * (fh / fL))));
        }
        public float Eq_C2________(float fS_L, float fK_s, float fDelta)
        {
            return (MathF.Pow2(MathF.fPI) / (2f * fDelta)) * fS_L * fK_s; // Eq (C.2) // fR  
        }
        public float Eq_C3________(float fG_y, float fPhi_y, float fG_z, float fPhi_z)
        {
            return 1 / (1 + (float)MathF.Sqrt(MathF.Pow2(fG_y * fPhi_y) + MathF.Pow2(fG_z * fPhi_z) + MathF.Pow2((2f / MathF.fPI) * fG_y * fPhi_y * fG_z * fPhi_z)));  // Eq (C.3) // fK_s 
        }
        public float Eq_C4________(float fc_f, float fRho, float fl_v, float fv_m, float fR, float fK_y, float fK_z, float fPhi, float fMu_ref, float fPhi_max)
        {
            return fc_f * fRho * fl_v * MathF.Pow2(fv_m) * fR * (fK_y * fK_z * fPhi / fMu_ref * fPhi_max); // Eq (C.4) // fSigma_a_x
        }
        public float Eq_E2________(float fb, float fn_i_y, float fSt)
        {
            return (fb * fn_i_y) / fSt; // Eq (E.2) // fv_crit_i
        }
        public float Eq_E3________(float fb, float fn_i_0, float fS_t)
        {
            return (fb * fn_i_0) / (2f * fS_t); // Eq (E.3) // fv_crit_i 
        }
        public float Eq_E4________(float fDelta_s, float fm_i_e, float fRho, float fb)
        {
            return (2f * fDelta_s * fm_i_e) / (fRho * MathF.Pow2(fb)); // Eq (E.4) // fS_c
        }
        public float Eq_E5________(float fb, float fv_crit_i, float fv)
        {
            return fb * fv_crit_i / fv; // Eq (E.5) // fR_e
        }
        public float Eq_E6________(float fm, float fn_i_y, float fPhi_i_y, float fy_F_max)
        {
            return fm * (MathF.Pow2(2f * MathF.fPI * fn_i_y)) * fPhi_i_y * fy_F_max; // Eq (E.6) // fF_w
        }
        public float Eq_E7________(float fS_t, float fS_c, float fK, float fK_w, float fc_lat)
        {
            return (1f / MathF.Pow2(fS_t)) * (1f / fS_c) * fK * fK_w * fc_lat; // Eq (E.7) // fy_F_max/fb
        }
        public float Eq_E10_______(float fT, float fn_y, float fEpsilon_0, float fv_crit, float fv_0, float fexp)
        {
            return 2f * fT * fn_y * fEpsilon_0 * (MathF.Pow2(fv_crit / fv_0)) * (float)Math.Exp(-MathF.Pow2(fv_crit / fv_0)); // Eq (E.10) // fN
        }
        public float Eq_E14________(float fS_t, float fC_c, float fRho, float fb, float fm_e, float fh, float fS_c, float fK_a, float fSigma_y, float fa_L)
        {
            return (1f / fS_t) * (fC_c / MathF.Sqrt(fS_c / 4f * MathF.fPI - fK_a * (1 - MathF.Pow2(fSigma_y / fb * fa_L)))) * (MathF.Sqrt(fRho * MathF.Pow2(fb) / fm_e)) * (MathF.Sqrt(fb / fh)); // Eq (E.14) //  fSigma_y / fb
        }
        public float Eq_E15_______(float fc_1, float fc_2)
        {
            return fc_1 + (MathF.Sqrt(MathF.Pow2(fc_1)) + fc_2); // Eq (E.15) // fSigma_y / fb
        }
        public float Eq_E16_______(float fRho, float fb, float fm_e, float fa_L, float fK_a, float fC_c, float fS_t, float fh)
        {
            return ((fRho * MathF.Pow2(fb)) / fm_e) * (MathF.Pow2(fa_L) / fK_a) * (MathF.Pow2(fC_c) / MathF.Pow4(fS_t)) * (fb / fh); // Eq (E.16) // fc_2
        }
        public float Eq_E16_______(float fa_L, float fS_c, float fK_a)
        {
            return (MathF.Pow2(fa_L) / 2f) * (1 - (fS_c / 4f * MathF.fPI * fK_a)); // Eq (E.16) // fc_1 
        }
        public float Eq_E17_______(float farctg, float fS_c, float fK_a)
        {
            return MathF.Sqrt(2f) * (1f + 1.2f * farctg * (0.75f * fS_c / (4f * MathF.fPI * fK_a))); // Eq (E.17) // fk_p
        }
        public float Eq_E18_______(float fSc, float fa_G, float fn_1_y, float fb)
        {
            return (2f * fSc / fa_G) * fn_1_y * fb; // Eq (E.18) // fv_CG 
        }
        public float Eq_E23_______(float fn_1_y, float fb, float fa, float fS_c, float fa_lG)
        {
            return 3.5f * fn_1_y * fb * MathF.Sqrt((fa / fb) * fS_c / fa_lG); // Eq (E.23) // fv_ClG
        }
        public float Eq_E24_______(float fk_Theta, float fRho, float fd, float fdc_M, float fd_Theta)
        {
            return MathF.Sqrt((2f * fk_Theta) / ((fRho * MathF.Pow2(fd)) * (fdc_M / fd_Theta))); // Eq (E.24) // fv_div
        }
        public float Eq_E25_______(float fM, float fRho, float fv, float fd)
        {
            return fM / (0.5f * fRho * MathF.Pow2(fv) * MathF.Pow2(fd)); // Eq (E.25) // fc_M
        }
        public float Eq_F1________(float fg, float fx_1)
        {
            return (1f / 2f * MathF.fPI) * MathF.Sqrt(fg / fx_1); // Eq (F.1) // fn_1
        }
        public float Eq_F3________(float fEpsilon_1, float fb, float fh_eff, float fW_s, float fW_t, float fH_z)
        {
            return (fEpsilon_1 * fb / MathF.Pow2(fh_eff)) * MathF.Sqrt(fW_s / fW_t) * (fH_z); // Eq (F.3) // fn_1
        }
        public float Eq_F4________(float fh_1, float fh_2)
        {
            return fh_1 + (fh_2 / 3f); // Eq (F.4) // fh_eff
        }
        public float Eq_F5________(float ft, float fE, float fMu_s, float fNu, float fb)
        {
            return 0.492f * MathF.Sqrt(MathF.Pow3(ft) * fE / (fMu_s * (1f - MathF.Pow2(fNu) * MathF.Pow4(fb))));
        }
        public float Eq_F6________(float fK, float fL, float fE, float fl_b, float fm)
        {
            return MathF.Pow2(fK) / (2f * MathF.fPI * MathF.Pow2(fL)) * MathF.Sqrt(fE * fl_b / fm); // Eq (F.6) // fn_1_B
        }
        public float Eq_F7________(float fn_1_B, float fP_1, float fP_2, float fP_3)
        {
            return fn_1_B * MathF.Sqrt(fP_1 * (fP_2 + fP_3)); // Eq (F.7) // fn_1_T
        }
        public float Eq_F8________(float fm, float fb, float fl_p)
        {
            return fm * MathF.Pow2(fm) / fl_p; // Eq (F.8) // fP_1
        }
        public float Eq_F9________(float fSigma, float fr_j, float fl_j, float fb, float fl_p)
        {
            return (fSigma * MathF.Pow2(fr_j) * fl_j) / (MathF.Pow2(fb) * fl_p); // Eq (F.9) // fP_2
        }
        public float Eq_F10________(float fL, float fSigma, float fJ_j, float fK_2, float fb, float fl_p, float fv)
        {
            return (MathF.Pow2(fL) * fSigma * fJ_j) / (2f * fK_2 * MathF.Pow2(fb) * fl_p * (1f + fv)); // Eq (F.10) // fP_3
        }
        public float Eq_F11_______(float fm_d, float fb, float fSigma, float fl_pj, float fm_j, float fr_j)
        {
            return (fm_d * MathF.Pow2(fb) / 12f) + fSigma * (fl_pj + fm_j * MathF.Pow2(fr_j)); // Eq (F.11) // fl_p
        }
        public float Eq_F16_______(float fRho, float fc_f, float fv_m, float fn_1, float fMu_e)
        {
            return (fRho * fc_f * fv_m) / (2f * fn_1 * fMu_e); // Eq (F.16) // fDelta_a 
        }
        public float Eq_F18_______(float fc_f, float fRho, float fb, float fv_m, float fn_1, float fm_e)
        {
            return (fc_f * fRho * fb * fv_m) / (2f * fn_1 * fm_e); // Eq (F.18) // fDelta_a
        }
    }
}