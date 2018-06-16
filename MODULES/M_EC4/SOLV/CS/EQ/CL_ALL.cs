using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MATH;

namespace M_EC4
{
    public class CL_ALL
    {
        public float Eq_61______(float fM_plaRd, float fM_plRd, float fN_c, float fN_cf)
        {
            if (fM_plRd >= fM_plaRd && (fN_cf > 0f && fN_cf >= fN_c))
                return fM_plaRd + (fM_plRd - fM_plaRd) * (fN_c / fN_cf); // Eq. (6.1) fM_Rd
            else
                return 0f;
        }
        public float Eq_62______(float fM_Ed, float fM_elRd, float fM_aEd, float fN_c, float fN_cel)
        {
            if (fM_elRd >= fM_aEd && (fN_cel > 0f && fN_cel >= fN_c))
                return fM_aEd + (fM_elRd - fM_aEd) * (fN_c / fN_cel); // Eq. (6.2) fM_Rd
            else
                return 0f;
        }
        public float Eq_63______(float fM_elRd, float fM_plRd, float fN_cf, float fN_c, float fN_cel)
        {
            if (fM_plRd >= fM_elRd && (fN_cf >= fN_c && fN_c >= fN_cel && fN_cf - fN_cel > 0f))
                return fM_elRd + (fM_plRd - fM_elRd) * ((fN_c - fN_cel)/ (fN_cf - fN_cel)); // Eq. (6.3) fM_Rd
            else
                return 0f;
        }
        public float Eq_64______(float fM_aEd, float fk, float fM_cEd)
        {
            return fM_aEd + fk * fM_cEd; // Eq. (6.4) fM_elRd

        }

//----------------------------------------------------------Jan D.---------------------------------------

        public float Eq_21______(float ff_ck, float fGamma_C)
        {
            if (!MathF.d_equal(fGamma_C, 0f))
                return ff_ck / fGamma_C; // Eq. (2.1) f_cd
            else
                return 0f;
        }
        public float Eq_51______(float fAlpha_cr)
        {
            return fAlpha_cr / 10f; // Eq. (5.1) Design Ratio
        }
        public float Eq_52______(float fLambda, float fN_pl_Rk, float fN_Ed)
        {
            if (!MathF.d_equal(fN_Ed, 0f))
                return fLambda / (0.5f * (float)MathF.Sqrt(fN_pl_Rk / fN_Ed)); // Eq. (5.2) Design Ratio
            else
                return 0f;
        }
        public float Eq_56______(float fn_0, float fPsy_L, float fPhi_t)
        {
            return fn_0 * (1f + (fPsy_L * fPhi_t)); // Eq. (5.6) n_L
        }
        public float Eq_57______(float fA_s, float fRho_s, float fA_c)
        {
            return fA_s / (fRho_s * fA_c); // Eq. (5.7) Design Ratio
        }
        public float Eq_58______(float fDelta, float ff_y, float ff_ctm, float ff_sk, float fk_c)
        {
            if (!MathF.d_equal(ff_sk, 0f))
                return fDelta * (ff_y / 235f) * (ff_ctm / ff_sk) * (float)MathF.Sqrt(fk_c); // Eq. (5.8) Rho_s
            else
                return 0f;
        }
        //  public float Eq_61______(float fM_pl_a_Rd, float fM_pl_Rd, float fN_c, float fN_cf)
        //  {
        //      if (!MathF.d_equal(fN_cf, 0f))
        //          return fM_pl_a_Rd + (fM_pl_Rd - fM_pl_a_Rd) * (fN_c / fN_cf); // Eq. (6.1) M_Rd
        //      else
        //          return 0f;
        //  }
        public float Eq_62______(float fM_a_Ed, float fM_el_Rd, float fN_c, float fN_c_el)
        {
            if (!MathF.d_equal(fN_c_el, 0f))
                return fM_a_Ed + (fM_el_Rd - fM_a_Ed) * (fN_c / fN_c_el); // Eq. (6.2) M_Rd
            else
                return 0f;
        }
        //  public float Eq_63______(float fM_el_Rd, float fM_pl_Rd, float fN_c, float fN_c_el, float fN_c_f)
        //  {
        //      if (!MathF.d_equal((fN_c_f - fN_c_el), 0f))
        //          return fM_el_Rd + (fM_pl_Rd - fM_el_Rd) * ((fN_c - fN_c_el) / (fN_c_f - fN_c_el)); // Eq. (6.3) M_Rd
        //      else
        //          return 0f;
        //  }
        //public float Eq_64______(float fM_a_Ed, float fk, float fM_c_Ed)
        //{
        //    return fM_a_Ed + (fk * fM_c_Ed); // Eq. (6.4) M_el_Rd
        //}
        //-------------------------------------------------10--------------------------------------------------------
        public float Eq_65______(float fV_Ed, float fV_Rd)
        {
            return (float)MathF.Pow2((2f * (fV_Ed / fV_Rd)) - 1f); // Eq. (6.5) Rho
        }
        public float Eq_66______(float fChi_LT, float fM_Rd)
        {
            return fChi_LT * fM_Rd; // Eq. (6.6) M_b_Rd
        }
        public float Eq_67______(float fM_Rk, float fM_cr)
        {
            if (!MathF.d_equal(fM_cr, 0f))
                return (float)MathF.Sqrt(fM_Rk / fM_cr); // Eq. (6.7) Lambda
            else
                return 0f;
        }
        public float Eq_68______(float fk_1, float fk_2)
        {
            if (!MathF.d_equal((fk_1 + fk_2), 0f))
                return (fk_1 * fk_2) / (fk_1 + fk_2); // Eq. (6.8) k_s
            else
                return 0f;
        }
        public float Eq_610_____(float fE_a, float ft_w, float fv_a, float fh_s)
        {
            if (!MathF.d_equal(fh_s, 0f))
                return (fE_a * (float)MathF.Pow3(ft_w)) / (4f * (1f - (float)MathF.Pow2(fv_a)) * fh_s); // Eq. (6.10) k_2
            else
                return 0f;
        }
        public float Eq_611_____(float fE_a, float ft_w, float fb_c, float fh_s, float fn)
        {
            if(!MathF.d_equal(fh_s,0f))
                return (fE_a*ft_w*(float)MathF.Pow2(fb_c))/(16f*fh_s*(1f+((4f*fn*ft_w)/fb_c))); // Eq. (6.11) k_2
            else
                return 0f;
        }
        public float Eq_618_____(float ff_u, float fd, float fGamma_V)
        {
            if (!MathF.d_equal(fGamma_V, 0f))
                return (0.8f * ff_u * (float)Math.PI * (float)MathF.Pow2(fd) / 4f) / fGamma_V; // Eq. (6.18) P_Rd
            else
                return 0f;
        }
        public float Eq_619_____(float fAlpha, float fd, float ff_ck, float fE_cm, float fGamma_V)
        {
            if (!MathF.d_equal(fGamma_V, 0f))
                return (0.29f * fAlpha * (float)MathF.Pow2(fd) * (float)MathF.Sqrt(ff_ck * fE_cm)) / fGamma_V; // Eq. (6.19) P_Rd
            else
                return 0f;
        }
        public float Eq_620_____(float fh_sc, float fd)
        {
            return 0.2f * ((fh_sc / fd) + 1f); // Eq. (6.20) Alpha
        }
        public float Eq_623_____(float fb_0, float fh_sc, float fh_p, float fn_r)
        {
            if (!MathF.d_equal(fn_r, 0f) && !MathF.d_equal(fh_p, 0f))
                return (0.7f / (float)MathF.Sqrt(fn_r)) * (fb_0 / fh_p) * ((fh_sc / fh_p) - 1f); // Eq. (6.23) k_t
            else
                return 0f;
        }
//-----------------------------------------------------20--------------------------------------------------------
        public float Eq_624_____(float fF_l, float fF_t, float fP_l_Rd, float fP_t_Rd)
        {
            return (((float)MathF.Pow2(fF_l) / (float)MathF.Pow2(fP_l_Rd)) + ((float)MathF.Pow2(fF_t) / (float)MathF.Pow2(fP_t_Rd))) / 1f; // Eq. (6.24) Design Ratio
        }
        public float Eq_628_____(float fLambda)
        {
            return fLambda / 2.0f; // Eq. (6.28) Design ratio
        }
        public float Eq_629_____(float fh)
        {
            return 0.3f * fh; // Eq. (6.29) c_z
        }
        public float Eq_630_____(float fA_a, float ff_yd, float fA_c, float ff_cd, float fA_s, float ff_sd)
        {
            return (fA_a * ff_yd) + (0.85f * fA_c * ff_cd) + (fA_s * ff_sd); // Eq. (6.30) N_pl_Rd
        }
        public float Eq_631_____(float fV_Ed, float fM_pl_a_Rd, float fM_pl_Rd)
        {
            if (!MathF.d_equal(fM_pl_Rd, 0f))
                return fV_Ed * (fM_pl_a_Rd / fM_pl_Rd); // Eq. (6.31) V_a_Ed
            else
                return 0f;
        }
        public float Eq_632_____(float fV_Ed, float fV_a_Ed)
        {
            return fV_Ed - fV_a_Ed; // Eq. (6.32) V_c_Ed
        }
        public float Eq_633_____(float fEta_a, float fA_a, float ff_yd, float fA_c, float ff_cd, float fEta_c, float ft, float ff_y, float fd, float ff_ck, float fA_s, float ff_sd)
        {
            return (fEta_a * fA_a * ff_yd) + ((fA_c * ff_cd) * (1f + ((fEta_c * (ft / fd)) * (ff_y / ff_ck))) + (fA_s * ff_sd)); // Eq. (6.33) N_pl_Rd
        }
        public float Eq_634_____(float fLambda)
        {
            return 0.25f * (3f + (2f * fLambda)); // Eq. (6.34) Eta_a_o
        }
        public float Eq_635_____(float fLambda)
        {
            return 4.9f - (18.5f * fLambda) + (17f * (float)MathF.Pow2(fLambda)); // Eq. (6.35) Eta_c_o
        }
        public float Eq_636_____(float fEta_a_o, float fe, float fd)
        {
            if (!MathF.d_equal(fd, 0f))
                return fEta_a_o + (1f - fEta_a_o) * (10f * fe / fd); // Eq. (6.36) Eta_a
            else
                return 0f;
        }
//--------------------------------------------------------30----------------------------------------------------
        public float Eq_637_____(float fEta_c_o, float fe, float fd)
        {
            return fEta_c_o * (1f - (10f * fe / fd)); // Eq. (6.37) Eta_c
        }
        public float Eq_638_____(float fA_a, float ff_yd, float fN_pl_Rd)
        {
            if (!MathF.d_equal(fN_pl_Rd, 0f))
                return (fA_a * ff_yd) / fN_pl_Rd; // Eq. (6.38) Delta
            else
                return 0f;
        }
        public float Eq_639_____(float fN_pl_Rd, float fN_cr)
        {
            if (!MathF.d_equal(fN_cr, 0f))
                return (float)MathF.Sqrt(fN_pl_Rd / fN_cr); // Eq. (6.39) Lambda
            else
                return 0f;
        }
        public float Eq_640_____(float fE_a, float fl_a, float fE_s, float fl_s, float fK_e, float fE_cm, float fl_c)
        {
            return (fE_a * fl_a) + (fE_s * fl_s) + (fK_e * fE_cm * fl_c); // Eq. (6.40) (El)_eff
        }
        public float Eq_641_____(float fE_cm, float fN_G_Ed, float fN_Ed, float fPhi_t)
        {
            return fE_cm * (1f / (1f + (fN_G_Ed / fN_Ed) * fPhi_t)); // Eq. (6.41) E_c_eff
        }
        public float Eq_643_____(float fBeta, float fN_Ed, float fN_cr_eff)
        {
            return fBeta / (1f - (fN_Ed / fN_cr_eff)); // Eq. (6.43) k
        }
        public float Eq_644_____(float fN_Ed, float fChi, float fN_pl_Rd)
        {
            return (fN_Ed / (fChi * fN_pl_Rd)) / 1.0f; // Eq. (6.44) Design Ratio
        }
        public float Eq_645_____(float fM_Ed, float fMu_d, float fM_pl_Rd, float fAlpha_M)
        {
            return (fM_Ed / (fMu_d * fM_pl_Rd)) / fAlpha_M; // Eq. (6.45) Design Ratio
        }
        public float Eq_646_____(float fM_y_Ed, float fMu_dy, float fM_pl_y_Rd, float fAlpha_M_y)
        {
            return (fM_y_Ed / (fMu_dy * fM_pl_y_Rd)) / fAlpha_M_y; // Eq. (6.46) Design Ratio
        }
        public float Eq_647_____(float fM_y_Ed, float fM_z_Ed, float fMu_dy, float fM_pl_y_Rd, float fMu_dz, float fM_pl_z_Rd)
        {
            return ((fM_y_Ed / (fMu_dy * fM_pl_y_Rd)) + (fM_z_Ed / (fMu_dz * fM_pl_z_Rd))) / 1.0f; // Eq. (6.47) Design Ratio
        }
//--------------------------------------------------------40-----------------------------------------------------
        public float Eq_649_____(float fc_z, float fc_z_min)
        {
            return (1f + (0.002f * fc_z * (1f - (fc_z_min / fc_z)))) / 2.5f; // Eq. (6.49) Design Ratio
        }
        public float Eq_651_____(float fSigma_s_max_f, float fM_Ed_min_f, float fM_Ed_max_f)
        {
            if (!MathF.d_equal(fM_Ed_max_f, 0f))
                return fSigma_s_max_f * (fM_Ed_min_f / fM_Ed_max_f); // Eq. (6.51) Sigma_s_min_f
            else
                return 0f;
        }
        public float Eq_625_____(float fLambda, float fPhi, float fSigma_max_f, float fSigma_min_f)
        {
            return fLambda * fPhi * Math.Abs(fSigma_max_f - fSigma_min_f); // Eq. (6.25) Delta Sigma_E , vo vzorci je velkost |Sigma_max_f-Sigma_min_f|
        }
        public float Eq_653_____(float fLambda_glob, float fPhi_glob, float fDelta, float fSigma_E_glob, float fLambda_loc, float fPhi_loc, float fSigma_E_loc)
        {
            return (fLambda_glob * fPhi_glob * fDelta * fSigma_E_glob) + (fLambda_loc * fPhi_loc * fDelta * fSigma_E_loc); // Eq. (6.53) Delta Sigma_E
        }
        public float Eq_654_____(float fLambda_v, float fDelta, float fTau)
        {
            return fLambda_v * fDelta * fTau; // Eq. (6.54) Delta Tau_E_2
        }
        public float Eq_71______(float fk_s, float fk_c, float fk, float ff_ct_eff, float fA_ct, float fSigma_s)
        {
            if (!MathF.d_equal(fSigma_s, 0f))
                return fk_s * fk_c * fk * ff_ct_eff * fA_ct / fSigma_s; // Eq. (7.1) A_s
            else
                return 0f;
        }
        public float Eq_73______(float fPhi, float ff_ct_eff, float ff_ct_0)
        {
            if (!MathF.d_equal(ff_ct_0, 0f))
                return fPhi * ff_ct_eff / ff_ct_0; // Eq. (7.3) Phi .... vo vzorci ma Phi horny index *
            else
                return 0f;
        }
        public float Eq_74______(float fSigma_s_o, float fDelta, float fSigma_s)
        {
            return fSigma_s_o + (fDelta * fSigma_s); // Eq. (7.4) Sigma_s
        }
        public float Eq_75______(float ff_ctm, float fAlpha_st, float fRho_s)
        {
            if (!MathF.d_equal(fAlpha_st * fRho_s, 0f))
                return (0.4f * ff_ctm) / (fAlpha_st * fRho_s); // Eq. (7.5) Delta Sigma_s
            else
                return 0f;
        }
        public float Eq_76______(float fA, float fl, float fA_a, float fl_a)
        {
            if (!MathF.d_equal(fA_a, 0f) && !MathF.d_equal(fl_a, 0f))
                return (fA * fl) / (fA_a * fl_a); // Eq. (7.6) Alpha_st
            else
                return 0f;
        }
//----------------------------------------------------50------------------------------------------------------
        public float Eq_81______(float fv, float fA_c, float ff_cd, float fTheta)
        {
            return 0.85f * fv * fA_c * ff_cd * (float)Math.Sin(fTheta); // Eq. (8.1) V_wp_c_Rd
        }
        public float Eq_82______(float fb_c, float ft_w, float fh, float ft_f, float fTheta)
        {
            return 0.8f * (fb_c - ft_w) * (fh - (2f * ft_f)) * (float)Math.Cos(fTheta); // Eq. (8.2) A_c
        }
        public float Eq_83______(float fh, float ft_f, float fz)
        {
            return (float)Math.Atan((fh - (2f * ft_f)) / fz); // Eq. (8.3) Theta
        }
        public float Eq_85______(float fk_wc_c, float ft_eff_c, float fb_c, float ft_w, float ff_cd)
        {
            return 0.85f * fk_wc_c * ft_eff_c * (fb_c - ft_w) * ff_cd; // Eq. (8.5) F_c_wc_c_Rd
        }
        public float Eq_91______(float fb_p, float fh_c, float fh_f)
        {
            return fb_p + (2f * (fh_c + fh_f)); // Eq. (9.1) b_m
        }
        public float Eq_95______(float fh, float fh_c, float fe_p, float fe, float fN_cf, float fA_pe, float ff_yp_d)
        {
            if (!MathF.d_equal((fA_pe * ff_yp_d), 0f))
                return fh - (0.5f * fh_c) - fe_p + (fe_p - fe) * (fN_cf / (fA_pe * ff_yp_d)); // Eq. (9.5) z
            else
                return 0f;
        }
        public float Eq_97______(float fb, float fd_p, float fm, float fA_p, float fk, float fY_VS, float fL_s)
        {
            return ((fb * fd_p / fY_VS) * (fm * fA_p / fb * fL_s)) + fk; // Eq. (9.7) V_1_Rd  ... vo vzorci a v popise vo worde je rozdiel: Y_VS alebo Gamma_Vs ?
        }
        public float Eq_98______(float fTau_u_Rd, float fb, float fL_x, float fN_cf)
        {
            return (fTau_u_Rd * fb * fL_x) / fN_cf; // Eq. (9.8) N_c
        }
        public float Eq_99______(float fh, float fx_pl, float fe_p, float fe, float fN_c, float fA_pe, float ff_yp_d)
        {
            return fh - (0.5f * fx_pl) - fe_p + (fe_p - fe) * (fN_c / fA_pe * ff_yp_d); // Eq. (9.9) z
        }
        public float Eq_910_____(float fk_Phi, float fd_do, float ft, float ff_yp_d)
        {
            return fk_Phi * fd_do * ft * ff_yp_d; // Eq. (9.10) P_pb_Rd
        }
//---------------------------------------------------60-----------------------------------------------------
        public float Eq_911_____(float fa, float fd_do)
        {
            return (1f + (fa / fd_do)) / 6.0f; // Eq. (9.11) k_Phi
        }
        public float Eq_A1______(float fb_eff_c_wc, float ft_wc, float fd_c)
        {
            if (!MathF.d_equal(fd_c, 0f))
                return (0.2f * fb_eff_c_wc * ft_wc) / fd_c; // Eq. (A.1) k_2
            else
                return 0f;
        }
        public float Eq_A2______(float fE_cm, float fb_c, float fh_c, float fE_a, float fBeta, float fz)
        {
            if (!MathF.d_equal(fE_a, 0f) && !MathF.d_equal(fBeta, 0f) && !MathF.d_equal(fz, 0f))
                return 0.06f * (fE_cm / fE_a) * ((fb_c * fh_c) / (fBeta * fz)); // Eq. (A.2) k_1_c
            else
                return 0f;
        }
        public float Eq_A3______(float fE_cm, float fE_a, float ft_eff_c, float fb_c, float fh_c)
        {
            if (!MathF.d_equal(fE_a, 0f) && !MathF.d_equal(fh_c, 0f))
                return 0.13f * (fE_cm / fE_a) * ((ft_eff_c * fb_c) / fh_c); // Eq. (A.3) k_2_c
            else
                return 0f;
        }
        public float Eq_A4______(float fE_cm, float fE_a, float ft_eff_c, float fb_c, float fh_c)
        {
            if (!MathF.d_equal(fE_a, 0f) && !MathF.d_equal(fh_c, 0f))
                return 0.5f * (fE_cm / fE_a) * ((ft_eff_c * fb_c) / fh_c); // Eq. (A.4) k_2_c
            else
                return 0f;
        }
        public float Eq_A5______(float fE_s, float fk_s_r, float fK_sc)
        {
            return 1 / (1f + ((fE_s * fk_s_r) / fK_sc)); // Eq. (A.5) k_slip
        }
        public float Eq_A6______(float fN, float fk_sc, float fv, float fXi, float fh_s, float fd_s)
        {
            return (fN*fk_sc)/(fv-(((fv-1f)/(1f+fXi))*(fh_s/fd_s))); // Eq. (A.6) K_sc
        }
        public float Eq_A7______(float fN, float fXi, float fk_sc, float fl, float fd_s, float fE_a, float fl_a)
        {
            if (!MathF.d_equal(fE_a, 0f) && !MathF.d_equal(fl_a, 0f))
                return (float)MathF.Sqrt(((1f + fXi) * fN * fk_sc * fl * (float)MathF.Pow2(fd_s)) / (fE_a * fl_a)); // Eq. (A.7) v
            else
                return 0f;
        }
        public float Eq_A8______(float fE_a, float fl_a, float fd_s, float fE_s, float fA_s)
        {
            if (!MathF.d_equal(fd_s, 0f) && !MathF.d_equal(fE_s, 0f) && !MathF.d_equal(fA_s, 0f))
                return (fE_a * fl_a) / ((float)MathF.Pow2(fd_s) * fE_s * fA_s); // Eq. (A.8) Xi
            else
                return 0f;
        }
        public float Eq_B1______(float ff_u, float ff_ut, float fP_Rk, float fGamma_V)
        {
            if (!MathF.d_equal(ff_ut, 0f) && !MathF.d_equal(fGamma_V, 0f))
                return ((ff_u / ff_ut) * (fP_Rk / fGamma_V)) / (fP_Rk / fGamma_V); // Eq. (B.1) P_Rd
            else
                return 0f;
        }
//--------------------------------------------------70------------------------------------------------------------------
        public float Eq_B2______(float fEta, float fN_cf, float fb, float fL_s, float fL_o)
        {
            if (!MathF.d_equal(fb, 0f))
                return (fEta * fN_cf) / (fb * (fL_s + fL_o)); // Eq. (B.2) Tau_u
            else
                return 0f;
        }
        public float Eq_B3______(float fEta, float fN_cf, float fMu, float fV_t, float fb, float fL_s, float fL_o)
        {
            if (!MathF.d_equal(fb, 0f))
                return ((fEta * fN_cf) - (fMu * fV_t)) / (fb * (fL_s + fL_o)); // Eq. (B.3) Tau_u
            else
                return 0f;
        }
    }
}
