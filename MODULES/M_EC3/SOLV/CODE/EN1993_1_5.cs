using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MATH;

namespace M_EC3.SOLV.CODE
{
    public class EN1993_1_5
    {
        //-------------------------------------------EN_1993_1_5--------------------------------------------------------------

        public float Eq_11_____(float fr, float fa, float ft)
        {
            if (!MathF.d_equal(ft, 0f))
                return fr / (MathF.Pow2(fa) / ft); // Eq. (1.1) Design Ratio
            else
                return 0f;
        }
        public float Eq_31_____(float fBeta, float fb_0)
        {
            return fBeta * fb_0; // Eq. (3.1) b_eff
        }
        public float Eq_32_____(float fF_Ed, float fb_eff, float ft_w, float fa_st_l)
        {
            if (!MathF.d_equal(fb_eff * (ft_w + fa_st_l), 0f))
                return fF_Ed / (fb_eff * (ft_w + fa_st_l)); // Eq. (3.2) Sigma_z_Ed
            else
                return 0f;
        }
        public float Eq_33_____(float fA_c_eff, float fBeta_ult)
        {
            return fA_c_eff * fBeta_ult; // Eq. (3.3) A_eff
        }
        public float Eq_34_____(float fA_c_eff, float fb_0, float ft_f)
        {
            if (!MathF.d_equal((fb_0 * ft_f), 0f))
                return MathF.Sqrt(fA_c_eff / (fb_0 * ft_f)); // Eq. (3.4) Alpha*_0
            else
                return 0f;
        }
        public float Eq_41_____(float fRho, float fA_c)
        {
            return fRho * fA_c; // Eq. (4.1) A_c_eff
        }
        public float Eq_44_____(float fSigma_com_Ed, float fLambda_p, float ff_y, float fGamma_M0)
        {
            if (!MathF.d_equal((ff_y / fGamma_M0), 0f))
                return fLambda_p * (MathF.Sqrt(fSigma_com_Ed / (ff_y / fGamma_M0))); // Eq. (4.4) Lambda_p_red
            else
                return 0f;
        }
        public float Eq_47_____(float fBeta_A_c, float ff_y, float fSigma_cr_p)
        {
            if (!MathF.d_equal(fSigma_cr_p, 0f))
                return MathF.Sqrt((fBeta_A_c * ff_y) / fSigma_cr_p); // Eq. (4.7) Lambda_p
            else
                return 0f;
        }
        public float Eq_48_____(float fE, float ft, float fv, float fa)
        {
            if (!MathF.d_equal(fa, 0f))
                return ((MathF.Pow2(MathF.fPI) * fE * MathF.Pow2(ft)) / (12f * (1f - MathF.Pow2(fv) * MathF.Pow2(fa)))); // Eq. (4.8) Sigma_cr_c
            else
                return 0f;
        }
        public float Eq_49_____(float fE, float fI_sl_1, float fA_sl_1, float fa)
        {
            if (!MathF.d_equal(fA_sl_1 * MathF.Pow2(fa), 0f))
                return (MathF.Pow2(MathF.fPI) * fE * fI_sl_1) / (fA_sl_1 * MathF.Pow2(fa)); // Eq. (4.9) Sigma_cr_sl
            else
                return 0f;
        }
        //--------------------------------------------------------10----------------------------------------------------------------
        public float Eq_410____(float ff_y, float fSigma_cr_c)
        {
            if (!MathF.d_equal(fSigma_cr_c, 0f))
                return MathF.Sqrt(ff_y / fSigma_cr_c); // Eq. (4.10) Lambda_c
            else
                return 0f;
        }
        public float Eq_411____(float fBeta_A_c, float ff_y, float fSigma_cr_c)
        {
            if (!MathF.d_equal(fSigma_cr_c, 0f))
                return MathF.Sqrt((fBeta_A_c * ff_y) / fSigma_cr_c); // Eq. (4.11) Lambda_c
            else
                return 0f;
        }
        public float Eq_412____(float fAlpha, float fi, float fe)
        {
            return fAlpha + (0.09f / (fi / fe)); // Eq. (4.12) Alpha_e
        }
        public float Eq_413____(float fRho, float fXi, float fChi_c)
        {
            return (fRho - fChi_c) * fXi * (2f - fXi) + fChi_c; // Eq. (4.13) Rho_c
        }
        public float Eq_52_____(float fChi_w, float ff_yw, float fh_w, float ft, float fGamma_M1)
        {
            if (!MathF.d_equal(fGamma_M1, 0f))
                return (fChi_w * ff_yw * fh_w * ft) / (MathF.Sqrt(3) * fGamma_M1); // Eq. (5.2) V_bw_Rd
            else
                return 0f;
        }
        public float Eq_53_____(float ff_yw, float fTau_cr)
        {
            if (!MathF.d_equal(fTau_cr, 0f))
                return 0.76f * MathF.Sqrt(ff_yw / fTau_cr); // Eq. (5.3) Lambda_w
            else
                return 0f;
        }
        public float Eq_54_____(float fk_Tau, float fSigma_E)
        {
            return fk_Tau * fSigma_E; // Eq. (5.4) Tau_cr
        }
        public float Eq_55_____(float fh_w, float ft, float fEpsilon)
        {
            if (!MathF.d_equal(ft * fEpsilon, 0f))
                return fh_w / (86.4f * ft * fEpsilon); // Eq. (5.5) Lambda_w
            else
                return 0f;
        }
        public float Eq_56_____(float fh_w, float ft, float fEpsilon, float fk_Tau)
        {
            if (!MathF.d_equal(ft * fEpsilon * MathF.Sqrt(fk_Tau), 0f))
                return fh_w / (37.4f * ft * fEpsilon * MathF.Sqrt(fk_Tau)); // Eq. (5.6) Lambda_w
            else
                return 0f;
        }
        public float Eq_57_____(float fh_wi, float ft, float fEpsilon, float fk_Taui)
        {
            if (!MathF.d_equal(ft * fEpsilon * MathF.Sqrt(fk_Taui), 0f))
                return fh_wi / (37.4f * ft * fEpsilon * MathF.Sqrt(fk_Taui)); // Eq. (5.7) Lambda_w
            else
                return 0f;
        }
        //----------------------------------------------------20----------------------------------------------------------------
        public float Eq_58_____(float fb_f, float ft_f, float ff_yf, float fM_Ed, float fM_f_Rd, float fc, float fGamma_M1)
        {
            if (!MathF.d_equal(fc * fGamma_M1, 0f))
                return ((fb_f * MathF.Pow2(ft_f) * ff_yf) / (fc * fGamma_M1)) * (1f - (MathF.Pow2(fM_Ed / fM_f_Rd))); // Eq. (5.8) V_bf_Rd
            else
                return 0f;
        }
        public float Eq_61_____(float ff_yw, float fL_eff, float ft_w, float fGamma_M1)
        {
            if (!MathF.d_equal(fGamma_M1, 0f))
                return (ff_yw * fL_eff * ft_w) / fGamma_M1; // Eq. (6.1) F_Rd
            else
                return 0f;
        }
        public float Eq_62_____(float fChi_F, float fl_y)
        {
            return fChi_F * fl_y; // Eq. (6.2) L_eff
        }
        public float Eq_64_____(float fl_y, float ft_w, float ff_yw, float fF_cr)
        {
            if (!MathF.d_equal(fF_cr, 0f))
                return MathF.Sqrt((fl_y * ft_w * ff_yw) / fF_cr); // Eq. (6.4) Lambda_F
            else
                return 0f;
        }
        public float Eq_65_____(float fk_F, float fE, float ft_w, float fh_w)
        {
            if (!MathF.d_equal(fh_w, 0f))
                return 0.9f * fk_F * fE * (MathF.Pow3(ft_w) / fh_w); // Eq. (6.5) F_cr
            else
                return 0f;
        }
        public float Eq_66_____(float fh_w, float fa, float fb_1, float fGamma_s)
        {
            return 6f + (2f * MathF.Pow2((fh_w / fa) / fa)) + ((5.44f * (fb_1 / fa)) - 0.21f) * MathF.Sqrt(fGamma_s); // Eq. (6.6) k_F
        }
        public float Eq_68_____(float ff_yf, float fb_f, float ft_w, float ff_yw)
        {
            if (!MathF.d_equal(ff_yw * ft_w, 0f))
                return (ff_yf * fb_f) / (ff_yw * ft_w); // Eq. (6.8) m_1
            else
                return 0f;
        }
        public float Eq_69_____(float fLambda_F, float fh_w, float ft_f)
        {
            if (fLambda_F > 0.5f)
                return 0.02f * MathF.Pow2(fh_w / ft_f); // Eq. (6.9) m_2
            else
                return 0f;
        }
        public float Eq_610____(float fs_s, float ft_f, float fm_1, float fm_2)
        {
            return fs_s + ((2f * ft_f) * (1f + MathF.Sqrt(fm_1 + fm_2))); // Eq. (6.10) l_y
        }
        public float Eq_611____(float fl_e, float ft_f, float fm_1, float fm_2)
        {
            return fl_e + (ft_f * MathF.Sqrt((fm_1 / 2f) + MathF.Pow2(fl_e / ft_f) + fm_2)); // Eq. (6.11) l_y
        }
        //----------------------------------------------------30-----------------------------------------------------------------------
        public float Eq_612____(float fl_e, float ft_f, float fm_1, float fm_2)
        {
            return fl_e + (ft_f * MathF.Sqrt(fm_1 + fm_2)); // Eq. (6.12) l_y
        }
        public float Eq_91_____(float fSigma_m, float fb, float fw_0, float fu, float fE)
        {
            if (!MathF.d_equal(fE, 0f))
                return (fSigma_m / fE) * MathF.Pow2(fb / MathF.fPI) * (1f + (fw_0 * (300f / fb) * fu)); // Eq. (9.1) I_st
            else
                return 0f;
        }
        public float Eq_92_____(float fSigma_m, float fw_0, float fw_el)
        {
            return (MathF.fPI / 4f) * fSigma_m * (fw_0 + fw_el); // Eq. (9.2) q
        }
        public float Eq_93_____(float fI_T, float fI_p, float ff_y, float fE)
        {
            if (!MathF.d_equal(fI_p, 0f) && (!MathF.d_equal(fE, 0f)))
                return (fI_T / fI_p) / (5.3f * (ff_y * fE)); // Eq. (9.3) Design Ratio
            else
                return 0f;
        }
        public float Eq_94_____(float fSigma_cr, float fTheta, float ff_y)
        {
            if (!MathF.d_equal(fTheta * ff_y, 0f))
                return fSigma_cr / (fTheta * ff_y); // Eq. (9.4) Design Ratio
            else
                return 0f;
        }
        public float Eq_95_____(float fI_net, float ff_yk, float fe, float fGamma_M0, float fb_G)
        {
            if (!MathF.d_equal(fe, 0f) && (!MathF.d_equal(fGamma_M0, 0f) && (!MathF.d_equal(fb_G, 0f))))
                return (fI_net / fe) * (ff_yk / fGamma_M0) * (MathF.fPI / fb_G); // Eq (9.5) V_Ed
            else
                return 0f;
        }
        public float Eq_101____(float fRho, float fAlpha_ult_k, float fGamma_M1)
        {
            if (!MathF.d_equal(fGamma_M1, 0f))
                return ((fRho * fAlpha_ult_k) / fGamma_M1) / 1f; // Eq. (10.1) Design Ratio
            else
                return 0f;
        }
        public float Eq_102____(float fAlpha_ult_k, float fAlpha_cr)
        {
            if (!MathF.d_equal(fAlpha_cr, 0f))
                return MathF.Sqrt(fAlpha_ult_k / fAlpha_cr); // Eq. (10.2) Lambda_p
            else
                return 0f;
        }
        public float Eq_103____(float fSigma_x_Ed, float fSigma_z_Ed, float ff_y, float fTau_Ed)
        {
            return MathF.Pow2(fSigma_x_Ed / ff_y) + MathF.Pow2(fSigma_z_Ed / ff_y) - (MathF.Pow2(fSigma_x_Ed / ff_y) * MathF.Pow2(fSigma_z_Ed / ff_y)) + (3f * MathF.Pow2(fTau_Ed / ff_y)); // Eq. (10.3) 1/Alpha...
        }
        public float Eq_A1_____(float fk_Sigma_p, float fSigma_E)
        {
            return fk_Sigma_p * fSigma_E; // Eq. (A.1) Sigma_cr_p
        }
        //--------------------------------------------------------------40---------------------------------------------------------------
        public float Eq_A2_____(float fAlpha, float fGamma, float fDelta, float fPsy)
        {
            if (!MathF.d_equal((fPsy + 1f), 0f) && (!MathF.d_equal((1f + fDelta), 0f)))
                return 2f * ((1f + MathF.Pow2(fAlpha) + fGamma - 1f)) / (MathF.Pow2(fAlpha) * (fPsy + 1f) * (1f + fDelta)); // Eq. (A.2) k_Sigma_p
            else
                return 0f;
        }
        public float Eq_A3_____(float fRho_c, float ff_y, float fA_sl_1, float fSigma_com_Ed, float fGamma_M1)
        {
            if (!MathF.d_equal(fSigma_com_Ed * fGamma_M1, 0f))
                return (fRho_c * ff_y * fA_sl_1) / (fSigma_com_Ed * fGamma_M1); // Eq. (A.3) A_c_eff_loc
            else
                return 0f;
        }
        public float Eq_B1_____(float fPhi_p, float fLambda_p)
        {
            return 1f / (fPhi_p + MathF.Sqrt(fPhi_p - fLambda_p)); // Eq. (B.1) Rho
        }
        public float Eq_C1_____(float fSigma, float fEpsilon)
        {
            return fSigma * (1f + fEpsilon); // Eq. (C.1) Sigma_true
        }
        public float Eq_C2_____(float fAlpha_u, float fAlpha_1, float fAlpha_2)
        {
            return fAlpha_u / (fAlpha_1 * fAlpha_2); // Eq. (C.2) Design Ratio
        }
        public float Eq_D1_____(float fb_2, float ft_2, float ff_yf_r, float fh_w, float ft_1, float fb_1, float fChi, float ff_yf, float fGamma_M0, float fGamma_M1)
        {
            return MathF.Min(((fb_2 * ft_2 * ff_yf_r) / fGamma_M0) * (fh_w + ((ft_1 + ft_2) / 2f)), ((fb_1 * ft_1 * ff_yf_r) / fGamma_M0) * (fh_w + ((ft_1 + ft_2) / 2f)), ((fb_1 * ft_1 * fChi * ff_yf) / fGamma_M1) * (fh_w + ((ft_1 + ft_2) / 2f))); // Eq. (D.1) M_Rd
        }
        public float Eq_D2_____(float fb, float fa)
        {
            return 0.43f + (MathF.Pow2(fb / fa)); // Eq. (D.2) k_Sigma
        }
        public float Eq_D4_____(float fChi_c, float ff_yw, float fh_w, float ft_w, float fGamma_M1)
        {
            if (!MathF.d_equal(fGamma_M1, 0f))
                return fChi_c * (ff_yw / (fGamma_M1 * MathF.Sqrt(3f))) * fh_w * ft_w; // Eq. (D.4) V_Rd
            else
                return 0f;
        }
        public float Eq_D6_____(float ff_y, float fTau_cr_l)
        {
            if (!MathF.d_equal(fTau_cr_l, 0f))
                return MathF.Sqrt(ff_y / (fTau_cr_l * MathF.Sqrt(3f))); // Eq. (D.6) Lambda_c_l
            else
                return 0f;
        }
        public float Eq_D7_____(float fE, float ft_w, float fa_max)
        {
            if (!MathF.d_equal(fa_max, 0f))
                return 4.83f * fE * (MathF.Pow2(ft_w / fa_max)); // Eq. (D.7) Tau_cr_l
            else
                return 0f;
        }
        //------------------------------------------------50-------------------------------------------------------------
        public float Eq_D9_____(float ff_y, float fTau_cr_g)
        {
            if (!MathF.d_equal(fTau_cr_g, 0f))
                return MathF.Sqrt(ff_y / (fTau_cr_g * MathF.Sqrt(3f))); // Eq. (D.9) Lambda_c_g
            else
                return 0f;
        }
        public float Eq_E3_____(float fLambda_p, float fSigma_com_Ed_ser, float ff_y)
        {
            if (!MathF.d_equal(ff_y, 0f))
                return fLambda_p * MathF.Sqrt(fSigma_com_Ed_ser / ff_y); // Eq. (E.3) Lambda_p_ser
            else
                return 0f;
        }
        public float Eq_E4_____(float fI_gr, float fSigma_gr, float fI_eff, float fSigma_com_Ed_ser)
        {
            return fI_gr - ((fSigma_gr / fSigma_com_Ed_ser) * (fI_gr - (fI_eff * fSigma_com_Ed_ser))); // Eq. (E.4)
        }
    }
}
