using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MATH;

namespace M_EC3.SOLV.CODE
{
    public class EN1993_1_3
    {
        //------------------------------------------------CSN_EN_1993_1_3_A-------------------------------------------

        public float Eq_31_____(float ff_yb, float ff_u, float fk, float fn, float ft, float fA_g)
        {
            return ff_yb + ((ff_u - ff_yb) * ((fk * fn * MathF.Pow2(ft)) / fA_g)); // Eq. (3.1) f_ya
        }
        public float Eq_51a____(float fA_g_sh, float fDelta)
        {
            return fA_g_sh * (1f - fDelta); // Eq. (5.1a) A_g
        }
        public float Eq_51b____(float fI_g_sh, float fDelta)
        {
            return fI_g_sh * (1f - (2f * fDelta)); // Eq. (5.1b) I_g
        }
        public float Eq_51c____(float fI_w_sh, float fDelta)
        {
            return fI_w_sh * (1f - (4f * fDelta)); // Eq. (5.1c) I_w
        }
        public float Eq_53a____(float fSigma_a, float fb_s, float fE, float ft, float fz)
        {
            if (!MathF.d_equal(fE, 0f) && !MathF.d_equal(ft, 0f) && !MathF.d_equal(fz, 0f))
                return 2f * (MathF.Pow2(fSigma_a) / MathF.Pow2(fE)) * (MathF.Pow2(fb_s) / (MathF.Pow2(ft) * fz)); // Eq. (5.3a) u
            else
                return 0f;
        }
        public float Eq_53b____(float fSigma_a, float fb_s, float fE, float ft, float fr)
        {
            if (!MathF.d_equal(fE, 0f) && !MathF.d_equal(ft, 0f) && !MathF.d_equal(fr, 0f))
                return 2f * (fSigma_a / fE) * (MathF.Pow4(fb_s) / (MathF.Pow2(ft) * fr)); // Eq. (5.3b)
            else
                return 0f;
        }
        public float Eq_59_____(float fu, float fDelta)
        {
            return fu / fDelta; // Eq. (5.9) K
        }
        public float Eq_510a___(float fTheta, float fb_p, float fu, float fv, float fE, float ft)
        {
            return (fTheta * fb_p) + ((fu * (MathF.Pow3(fb_p)) / 3f) * (12f * (1f - MathF.Pow2(fv)) / (fE * MathF.Pow3(ft)))); // Eq. (5.10a) Delta
        }
        public float Eq_510b___(float fE, float ft, float fv, float fb_1, float fh_w, float fb_2, float fk_f)
        {
            if (!MathF.d_equal(1f - MathF.Pow2(fv), 0f))
                return (fE * MathF.Pow3(ft) / (4f * (1f - MathF.Pow2(fv)))) * (1f / ((MathF.Pow2(fb_1) * fh_w)) + MathF.Pow3(fb_1) + (0.5f * fb_1 * fb_2 * fh_w * fk_f)); // Eq. (5.10b) K_1
            else
                return 0f;
        }
        public float Eq_511____(float fu, float fb_1, float fb_2, float fv, float fE, float ft)
        {
            if (!MathF.d_equal(3 * (fb_1 + fb_2), 0f) && !MathF.d_equal(fE * MathF.Pow3(ft), 0f))
                return ((fu * MathF.Pow2(fb_1) * MathF.Pow2(fb_2)) / (3f * (fb_1 + fb_2))) * (12f * (1f - MathF.Pow2(fv)) / (fE * MathF.Pow3(ft))); // Eq. (5.11) Delta
            else
                return 0f;
        }
        //--------------------------------------------------10--------------------------------------------------
        public float Eq_512b___(float ff_yb, float fSigma_cr_s)
        {
            if (!MathF.d_equal(fSigma_cr_s, 0f))
                return MathF.Sqrt(ff_yb / fSigma_cr_s); // Eq. (5.12b) Lambda_d
            else
                return 0f;
        }
        public float Eq_513a___(float fRho, float fb_p_c)
        {
            return fRho * fb_p_c; // Eq. (5.13a) c_eff
        }
        public float Eq_513e___(float fRho, float fb_p_d)
        {
            return fRho * fb_p_d; // Eq. (5.13e) d_eff
        }
        public float Eq_515____(float fK, float fE, float fI_s, float fA_s)
        {
            if (!MathF.d_equal(fA_s, 0f))
                return (2f * MathF.Sqrt(fK * fE * fI_s)) / fA_s; // Eq. (5.15) Sigma_cr_s
            else
                return 0f;
        }
        public float Eq_516____(float fLambda_p, float fChi_d)
        {
            return fLambda_p * MathF.Sqrt(fChi_d); // Eq. (5.16) Lambda_p_red
        }
        public float Eq_517____(float fChi_d, float fA_s, float ff_yb, float fGamma_M0, float fSigma_com_Ed)
        {
            if (!MathF.d_equal(fSigma_com_Ed, 0f))
                return fChi_d * fA_s * ((ff_yb / fGamma_M0) / fSigma_com_Ed); // Eq. (5.17) A_s_red
            else
                return 0f;
        }
        public float Eq_518____(float ft, float fb_1_e2, float fb_2_e1, float fb_s)
        {
            return ft * (fb_1_e2 + fb_2_e1 + fb_s); // Eq. (5.18)
        }
        public float Eq_519____(float fK, float fE, float fI_s, float fA_s)
        {
            if (!MathF.d_equal(fA_s, 0f))
                return (2f * MathF.Sqrt(fK * fE * fI_s)) / fA_s; // Eq. (5.19) Sigma_cr_s
            else
                return 0f;
        }
        public float Eq_520____(float fLambda_p, float fChi_d)
        {
            return fLambda_p * MathF.Sqrt(fChi_d); // Eq. (5.20) Lambda_p_red
        }
        public float Eq_521____(float fChi_d, float fA_s, float ff_yb, float fGamma_M0, float fSigma_com_Ed)
        {
            if (!MathF.d_equal(fSigma_com_Ed, 0f))
                return fChi_d * fA_s * ((ff_yb / fGamma_M0) / fSigma_com_Ed); // Eq. (5.20) A_s_red
            else
                return 0f;
        }
        //-----------------------------------------------20----------------------------------------------------------
        public float Eq_522____(float fk_w, float fE, float fI_s, float ft, float fA_s, float fb_p, float fb_s)
        {
            if (!MathF.d_equal(fA_s, 0f) && !MathF.d_equal(4f * MathF.Pow2(fb_p) * ((2f * fb_p) + (3f * fb_s)), 0f))
                return (4.2f * fk_w * fE) / fA_s * ((fI_s * MathF.Pow3(ft)) / (MathF.Sqrt((4f * MathF.Pow2(fb_p) * ((2f * fb_p) + (3f * fb_s)))))); // Eq. (5.22) Sigma_cr_s
            else
                return 0f;
        }
        public float Eq_523a___(float fk_w, float fE, float fA_s, float fI_s, float ft, float fb_1, float fb_e)
        {
            if (!MathF.d_equal(fA_s, 0f) && !MathF.d_equal(8f * MathF.Pow2(fb_1) * ((3f * fb_e) - (4f * fb_1)), 0f))
                return (4.2f * fk_w * fE) / fA_s * ((fI_s * MathF.Pow3(ft)) / (MathF.Sqrt((8f * MathF.Pow2(fb_1) * ((3f * fb_e) - (4f * fb_1)))))); // Eq. (5.23a) Sigma_cr_s
            else
                return 0f;
        }
        public float Eq_523b___(float fRho, float fb_e, float ft)
        {
            return fRho * fb_e * ft; // Eq. (5.23b) A_eff
        }
        public float Eq_523c___(float fE, float fI_s, float ft, float fb_o, float fb_e)
        {
            if (!MathF.d_equal(fb_o, 0f) && !MathF.d_equal(fb_e, 0f))
                return 1.8f * fE * MathF.Sqrt((fI_s * ft) / (MathF.Pow2(fb_o) * MathF.Pow3(fb_e))) + (3.6f * (fE * MathF.Pow2(ft)) / MathF.Pow2(fb_o)); // Eq. (5.23c) Sigma_cr_s
            else
                return 0f;
        }
        public float Eq_524b___(float fk_wo, float fl_b, float fs_w)
        {
            return fk_wo - ((fk_wo - 1f) * (((2f * fl_b) / fs_w) - (MathF.Pow2(fl_b / fs_w)))); //  Eq. (5.24b) k_w
        }
        public float Eq_526____(float fs_w, float fb_d)
        {
            if (!MathF.d_equal((fs_w + (0.5 * fb_d)), 0f))
                return MathF.Sqrt(fs_w + (2f * fb_d) / (fs_w + (0.5f * fb_d))); // Eq. (5.26) k_wo
            else
                return 0f;
        }
        public float Eq_528____(float fb_e, float fs_w, float fb_1)
        {
            return MathF.Sqrt(((2f * fb_e) + fs_w) * ((3f * fb_e) - (4f * fb_1)) / (fb_1 * ((4f * fb_e) - (6f * fb_1)) + (fs_w * ((3f * fb_e) - (4f * fb_1))))); // Eq. (5.28) k_wo
        }
        public float Eq_529____(float fChi_d, float fA_s, float ff_yb, float fGamma_M0, float fSigma_com_ser)
        {
            if (!MathF.d_equal(fSigma_com_ser, 0f))
                return fChi_d * fA_s * ((ff_yb / fGamma_M0) / fSigma_com_ser); // Eq. (5.29) A_s_red
            else
                return 0f;
        }
        public float Eq_530____(float ft, float fs_eff_2, float fs_eff_3, float fs_sa)
        {
            return ft * (fs_eff_2 + fs_eff_3 + fs_sa); // Eq. (5.30) A_sa
        }
        public float Eq_531____(float ft, float fs_eff_4, float fs_eff_5, float fs_sb)
        {
            return ft * (fs_eff_4 + fs_eff_5 + fs_sb); // Eq. (5.31) A_sb
        }
        //------------------------------------------------30--------------------------------------------------------------
        public float Eq_532____(float ft, float fE, float fGamma_M0, float fSigma_com_Ed)
        {
            if (!MathF.d_equal((fGamma_M0 * fSigma_com_Ed), 0f))
                return 0.76f * ft * MathF.Sqrt(fE / (fGamma_M0 * fSigma_com_Ed)); // Eq. (5.32) s_eff_0
            else
                return 0f;
        }
        public float Eq_533b___(float fh_a, float fe_c, float fs_eff_0)
        {
            return (1f + (0.5f * fh_a / fe_c)) * fs_eff_0; // Eq. (5.33b) s_eff_2
        }
        public float Eq_533f___(float fs_eff_0)
        {
            return 1.5f * fs_eff_0; // Eq. (5.33f) s_eff_n
        }
        public float Eq_534a___(float fs_n)
        {
            return 0.4f * fs_n; // Eq. (5.34a) s_eff_1
        }
        public float Eq_534b___(float fs_n)
        {
            return 0.6f * fs_n; // Eq. (5.34b) s_eff_n
        }
        public float Eq_535b___(float fs_a, float fh_a, float fe_c)
        {
            return fs_a * (1f + (0.5f * (fh_a / fe_c)) / (2f + (0.5f * (fh_a / fe_c)))); // Eq. (5.35b) s_eff_2
        }
        public float Eq_536a___(float fs_n, float fh_a, float fh_sa, float fe_c)
        {
            return fs_n * ((1f + ((0.5f * (fh_a + fh_sa)) / fe_c)) / (2.5f + (0.5f * ((fh_a + fh_sa) / fe_c)))); // Eq. (5.36a) s_eff_3
        }
        public float Eq_536b___(float fs_n, float fh_a, float fh_sa, float fe_c)
        {
            return (1.5f * fs_n) / (2.5f + ((0.5f * (fh_a + fh_sa)) / fe_c)); // Eq. (5.36b) s_eff_n
        }
        public float Eq_537a___(float fs_b, float fh_a, float fh_sa, float fe_c, float fh_b)
        {
            return fs_b * ((1f + (0.5f * ((fh_a + fh_sa) / fe_c))) / (2f + (0.5f * ((fh_a + fh_sa + fh_b) / fe_c)))); // Eq. (5.37a) s_eff_3
        }
        public float Eq_537b___(float fs_b, float fh_b, float fe_c, float fh_a, float fh_sa)
        {
            return fs_b * ((1f + (0.5f * ((fh_a + fh_sa) / fe_c))) / (2f + (0.5f * ((fh_a + fh_sa + fh_b) / fe_c)))); // Eq. (5.37b) s_eff_4
        }
        //--------------------------------------------------40----------------------------------------------------------
        public float Eq_538a___(float fs_n, float fh_b, float fh_sb, float fe_c)
        {
            return fs_n * ((1f + (0.5f * ((fh_b + fh_sb) / fe_c))) / (2.5f + (0.5f * ((fh_b + fh_sb) / fe_c)))); // Eq. (5.38a) s_eff_5
        }
        public float Eq_538b___(float fs_n, float fh_b, float fh_sb, float fe_c)
        {
            return (1.5f * fs_n) / (2.5f + (0.5f * ((fh_b + fh_sb) / fe_c))); // Eq. (5.38b) s_eff_n
        }
        public float Eq_539b___(float fs_a, float fs_sa, float fs_c)
        {
            return 0.9f * (fs_a + fs_sa + fs_c); // Eq. (5.39b) s_1
        }
        public float Eq_539c___(float fs_a, float fs_sa, float fs_b, float fs_sb, float fs_c)
        {
            return fs_a + fs_sa + fs_b + (0.5f * (fs_sb + fs_c)); // Eq. (5.39c) s_1
        }
        public float Eq_539d___(float fs_1, float fs_a, float fs_sa)
        {
            return fs_1 - fs_a - (0.5f * fs_sa); // Eq. (5.39d) s_2
        }
        public float Eq_540____(float fChi_d, float fA_sa, float fh_a, float fh_sa, float fe_c)
        {
            return (fChi_d * fA_sa) / (1f - ((fh_a + (0.5f * fh_sa)) / fe_c)); // Eq. (5.40) A_sa_red
        }
        public float Eq_541____(float ft, float fE, float fGamma_M0, float fSigma_com_Ed)
        {
            if (!MathF.d_equal((fGamma_M0 * fSigma_com_Ed), 0f))
                return 0.95f * MathF.Sqrt(fE / (fGamma_M0 * fSigma_com_Ed)); // Eq. (5.41) s_eff_0
            else
                return 0f;
        }
        public float Eq_61_____(float ff_ya, float fA_g, float fGamma_M0)
        {
            if (!MathF.d_equal(fGamma_M0, 0f))
                return (ff_ya * fA_g) / fGamma_M0; // Eq. (6.1) N_t_Rd
            else
                return 0f;
        }
        public float Eq_62_____(float fA_eff, float ff_yb, float fGamma_M0)
        {
            if (!MathF.d_equal(fGamma_M0, 0f))
                return (fA_eff * ff_yb) / fGamma_M0; // Eq. (6.2) N_c_Rd
            else
                return 0f;
        }
        public float Eq_64_____(float fW_eff, float ff_yb, float fGamma_M0)
        {
            if (!MathF.d_equal(fGamma_M0, 0f))
                return (fW_eff * ff_yb) / fGamma_M0; // Eq. (6.4) M_c_Rd
            else
                return 0f;
        }
        //---------------------------------------------------50------------------------------------------------
        public float Eq_66_____(float fW_el, float ff_ya, float fGamma_M0)
        {
            if (!MathF.d_equal(fGamma_M0, 0f))
                return (fW_el * ff_ya) / fGamma_M0; // Eq. (6.6) M_c_Rd
            else
                return 0f;
        }
        public float Eq_67_____(float fM_y_Ed, float fM_cy_Rd, float fM_z_Ed, float fM_cz_Rd)
        {
            return ((fM_y_Ed / fM_cy_Rd) + (fM_z_Ed / fM_cz_Rd)) / 1f; // Eq. (6.7) Design Ratio
        }
        public float Eq_68_____(float fh_w, float ft, float ff_bv, float fPhi, float fGamma_M0)
        {
            if (!MathF.d_equal(fGamma_M0, 0f))
                return ((fh_w / (float)Math.Sin(fPhi)) * ft * ff_bv) / fGamma_M0; // Eq. (6.8) V_b_Rd
            else
                return 0f;
        }
        public float Eq_610a___(float fs_w, float ft, float ff_yb, float fE)
        {
            if (!MathF.d_equal(ft, 0f) && !MathF.d_equal(fE, 0f))
                return 0.346f * (fs_w / ft) * MathF.Sqrt(ff_yb / fE); // Eq. (6.10a) Lambda_w
            else
                return 0f;
        }
        public float Eq_610b___(float fs_d, float ff_yb, float ft, float fk_Tau, float fE)
        {
            if (!MathF.d_equal(ft, 0f) && !MathF.d_equal(fk_Tau, 0f) && !MathF.d_equal(fE, 0f))
                return 0.346f * (fs_d / ft) * MathF.Sqrt((5.34f / fk_Tau) * (ff_yb * fE)); // Eq. (6.10b) Lambda_w
            else
                return 0f;
        }
        public float Eq_611a___(float fSigma_tot_Ed, float ff_ya, float fGamma_M0)
        {
            if (!MathF.d_equal(fGamma_M0, 0f))
                return fSigma_tot_Ed / (ff_ya / fGamma_M0); // Eq. (6.11a) Design Ratio
            else
                return 0f;
        }
        public float Eq_611b___(float fTau_tot_Ed, float ff_ya, float fGamma_M0)
        {
            if (!MathF.d_equal(fGamma_M0, 0f))
                return fTau_tot_Ed / ((ff_ya / MathF.Sqrt(3f)) / fGamma_M0); // Eq. (6.11b) Design Ratio
            else
                return 0f;
        }
        public float Eq_612a___(float fSigma_N_Ed, float fSigma_My_Ed, float fSigma_Mz_Ed, float fSigma_w_Ed)
        {
            return fSigma_N_Ed + fSigma_My_Ed + fSigma_Mz_Ed + fSigma_w_Ed; // Eq. (6.12a) Sigma_tot_Ed
        }
        public float Eq_612b___(float fTau_Vy_Ed, float fTau_Vz_Ed, float fTau_t_Ed, float fTau_w_Ed)
        {
            return fTau_Vy_Ed + fTau_Vz_Ed + fTau_t_Ed + fTau_w_Ed; // Eq. (6.12b) Tau_tot_Ed
        }
        public float Eq_613____(float fF_Ed, float fR_w_Rd)
        {
            return fF_Ed / fR_w_Rd; // Eq. (6.13) Design Ratio
        }
        //-------------------------------------------------------60---------------------------------------------
        public float Eq_622____(float fe_max, float ft)
        {
            return 1.45f - (0.05f * fe_max / ft); // Eq. (6.22) k_a_s
        }
        public float Eq_623____(float fN_Ed, float fN_t_Rd, float fM_y_Ed, float fM_cy_Rd_ten, float fM_z_Ed, float fM_cz_Rd_ten)
        {
            return ((fN_Ed / fN_t_Rd) + (fM_y_Ed / fM_cy_Rd_ten) + (fM_z_Ed / fM_cz_Rd_ten)) / 1f; // Eq. (6.23) Design Ratio
        }
        public float Eq_624____(float fM_y_Ed, float fM_cy_Rd_com, float fM_z_Ed, float fM_cz_Rd_com, float fN_Ed, float fN_t_Rd)
        {
            return ((fM_y_Ed / fM_cy_Rd_com) + (fM_z_Ed / fM_cz_Rd_com) - (fN_Ed / fN_t_Rd)) / 1f; // Eq. (6.24) Design Ratio
        }
        public float Eq_628a___(float fM_Ed, float fM_c_Rd)
        {
            return (fM_Ed / fM_c_Rd) / 1f; // Eq. (6.28a) Design Ratio
        }
        public float Eq_628b___(float fF_Ed, float fR_w_Rd)
        {
            return (fF_Ed / fR_w_Rd) / 1f; // Eq. (6.28b) Design Ratio
        }
        public float Eq_628c___(float fM_Ed, float fM_c_Rd, float fF_Ed, float fR_w_Rd)
        {
            return ((fM_Ed / fM_c_Rd) + (fF_Ed / fR_w_Rd)) / 1f; // Eq. (6.28c)
        }
        public float Eq_633b___(float fi_y, float fi_z, float fy_o, float fz_o)
        {
            return MathF.Pow2(fi_y) + MathF.Pow2(fi_z) + MathF.Pow2(fy_o) + MathF.Pow2(fz_o); // Eq. (6.33b) i_o^2
        }
        public float Eq_81a____(float fN_Ed, float fChi, float fW_eff, float fA_eff, float fa, float fl)
        {
            if (!MathF.d_equal(fA_eff, 0f) && !MathF.d_equal(fl, 0f))
                return fN_Ed * ((1f / fChi) - 1f) * (fW_eff / fA_eff) * (float)Math.Sin((MathF.fPI * fa) / fl); // Eq. (8.1a) Delta M_Ed
            else
                return 0f;
        }
        public float Eq_81b____(float fN_Ed, float fl, float fChi, float fW_eff, float fA_eff)
        {
            if (!MathF.d_equal(fl, 0f) && !MathF.d_equal(fA_eff, 0f))
                return ((MathF.fPI * fN_Ed) / fl) * ((1f / fChi) - 1f) * (fW_eff / fA_eff); // Eq. (8.1b)
            else
                return 0f;
        }
        public float Eq_82_____(float fF_t_Ed, float fF_v_Ed, float fF_p_Rd, float fF_o_Rd, float fF_b_Rd, float fF_n_Rd)
        {
            return ((fF_t_Ed / Math.Min(fF_p_Rd, fF_o_Rd)) + (fF_v_Ed / Math.Min(fF_b_Rd, fF_n_Rd))) / 1f; // Eq. (8.2) Design Ratio
        }
        //---------------------------------------------------70--------------------------------------------------------
        public float Eq_83b____(float ft)
        {
            return 5f * MathF.Sqrt(ft); // Eq. (8.3b) d_s
        }
        public float Eq_84a____(float ft, float fL_w_s, float fb, float ff_u, float fGamma_M2)
        {
            if (!MathF.d_equal(fGamma_M2, 0f))
                return ft * fL_w_s * (0.9f - (0.45f * fL_w_s / fb)) * ff_u / fGamma_M2; // Eq. (8.4a) F_w_Rd
            else
                return 0f;
        }
        public float Eq_84b____(float ft, float fb, float ff_u, float fGamma_M2)
        {
            if (!MathF.d_equal(fGamma_M2, 0f) && !MathF.d_equal(fb, 0f))
                return 0.45f * ft * fb * ff_u / fGamma_M2; // Eq. (8.4b) F_W_Rd
            else
                return 0f;
        }
        public float Eq_84c____(float ft, float fL_w_e, float fb, float ff_u, float fGamma_M2)
        {
            if (!MathF.d_equal(fb, 0f) && !MathF.d_equal(fGamma_M2, 0f))
                return ft * fL_w_e * (1f - (0.3f * fL_w_e / fb)) * ff_u / fGamma_M2; // Eq. (8.4c) F_w_Rd
            else
                return 0f;
        }
        public float Eq_85a____(float fd_s, float ff_uw, float fGamma_M2)
        {
            if (!MathF.d_equal(fGamma_M2, 0f))
                return (MathF.fPI / 4f) * MathF.Pow2(fd_s) * 0.625f * ff_uw / fGamma_M2; // Eq. (8.5a) F_w_Rd
            else
                return 0f;
        }
        public float Eq_87a____(float fd_w, float ft)
        {
            return fd_w - ft; // Eq. (8.7a) d_p
        }
        public float Eq_88a____(float fd_s, float fL_w, float ff_uw, float fGamma_M2)
        {
            if (!MathF.d_equal(fGamma_M2, 0f))
                return (((MathF.fPI / 4f) * MathF.Pow2(fd_s) + (fL_w * fd_s)) * 0.625f * ff_uw) / fGamma_M2; // Eq. (8.8a) F_w_Rd
            else
                return 0f;
        }
    }
}
