using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MATH;

namespace M_EC3.SOLV.CODE
{
    public enum eJoint_Shape
    {
        eJ_T,        // T
        eJ_Y,        // Y
        eJ_X,        // X
        eJ_K,        // K
        eJ_KT        // KT
    };

    public enum eBoltClass
    {
        eBClass_046,        // Bolt Class  4.6
        eBClass_048,        // Bolt Class  4.8
        eBClass_056,        // Bolt Class  5.6
        eBClass_058,        // Bolt Class  5.8
        eBClass_068,        // Bolt Class  6.8
        eBClass_088,        // Bolt Class  8.8
        eBClass_109,        // Bolt Class 10.9
    };

    public class EN1993_1_8
    {
        public float Eq_32____10(float ff_u, float fd, float ft, float fgamma_M2)
        {
            return 1.5f * ff_u * fd * ft / fgamma_M2; // Eq. (3.2) FbRd
        }

        public float Eq_33______(float fd, float ft_p)
        {
            return Math.Min(9f / (8 * fd + 3f * ft_p), 1f); // Eq. (3.3) Beta_p
        }


        // Tabulka 3.4
        public float Eq_Tab34_b_(eBoltClass eBClass, float falpha_v, float ff_ub, float fA, float fgamma_M2)
        {
            switch (eBClass)
            {
                case eBoltClass.eBClass_046:
                case eBoltClass.eBClass_056:
                case eBoltClass.eBClass_088:
                    {
                        falpha_v = 0.6f;
                        break;
                    }
                case eBoltClass.eBClass_048:
                case eBoltClass.eBClass_058:
                case eBoltClass.eBClass_068:
                case eBoltClass.eBClass_109:
                    {
                        falpha_v = 0.5f;
                        break;
                    }
                default:
                    {
                        falpha_v = 0.5f;
                        break;
                    }
            }
            return falpha_v * ff_ub * fA / fgamma_M2; ; // Tab 3.4 FvRd - Bolts
        }

        public float Eq_Tab34_r_(float ff_ur, float fA_0, float fgamma_M2)
        {
            return 0.6f * ff_ur * fA_0 / fgamma_M2; ; // Tab 3.4 FvRd - Rivets
        }

        // pokracovanie tab 3.4

        // Cislovane rovnice
        public float Eq_34______(float fk_t, float fk_s, float fd, float ft_b_resin, float fBeta, float ff_b_resin, float fGamma_M4)
        {
            return fk_t * fk_s * fd * ft_b_resin * fBeta * ff_b_resin / fGamma_M4; // Eq. (3.4) fFb_Rd_resin
        }
        public float Eq_35______(float fL_j, float fd)
        {
            return Math.Max(Math.Min(1f - ((fL_j - 15 * fd) / (200f * fd)), 1f), 0.75f); // Eq. (3.5) fBeta_Lf
        }
        public float Eq_36______(float fk_s, int in_Count, float fMu, float fF_p_C, float fGamma_M3)
        {
            return fk_s * in_Count * fMu / fGamma_M3 * fF_p_C; // Eq. (3.6) fF_s_Rd
        }
        public float Eq_37______(float ff_ub, float fA_s)
        {
            return 0.7f * ff_ub * fA_s; // Eq. (3.7) fF_p_C
        }
        public float Eq_38a_____(float fk_s, int in_Count, float fMu, float fF_p_C, float fF_t_Ed_ser, float fGamma_M3_ser)
        {
            return fk_s * in_Count * fMu * (fF_p_C - 0.8f * fF_t_Ed_ser) / fGamma_M3_ser; // Eq. (3.8a) fF_s_Rd_ser
        }
        public float Eq_38b_____(float fk_s, int in_Count, float fMu, float fF_p_C, float fF_t_Ed, float fGamma_M3)
        {
            return fk_s * in_Count * fMu * (fF_p_C - 0.8f * fF_t_Ed) / fGamma_M3; // Eq. (3.8b) fF_s_Rd
        }
        public float Eq_39______(float ff_u, float fA_nt, float fGamma_M2, float ff_y, float fA_nv, float fGamma_M0)
        {
            return (ff_u * fA_nt / fGamma_M2) + ((1f / MathF.fSqrt3) * ff_y * fA_nv / fGamma_M0); // Eq. (3.9) fV_eff_1_Rd
        }
        public float Eq_310_____(float ff_u, float fA_nt, float fGamma_M2, float ff_y, float fA_nv, float fGamma_M0)
        {
            return (0.5f * ff_u * fA_nt / fGamma_M2) + ((1f / MathF.fSqrt3) * ff_y * fA_nv / fGamma_M0); // Eq. (3.10) fV_eff_2_Rd
        }
        public float Eq_311_____(float fe_2, float fd_0, float ft, float ff_u, float fGamma_M2)
        {
            return 2 * (fe_2 - 0.5f * fd_0) * ft * ff_u / fGamma_M2; // Eq. (3.11) fN_u_Rd
        }
        public float Eq_312_____(float fBeta_2, float fA_net, float ff_u, float fGamma_M2)
        {
            return fBeta_2 * fA_net * ff_u / fGamma_M2; // Eq. (3.12) fN_u_Rd
        }
        public float Eq_313_____(float fBeta_3, float fA_net, float ff_u, float fGamma_M2)
        {
            return fBeta_3 * fA_net * ff_u / fGamma_M2; // Eq. (3.13) fN_u_Rd
        }
        public float Eq_314_____(float fSigma_h_Ed, float ff_h_Rd)
        {
            return fSigma_h_Ed / ff_h_Rd; // Eq. (3.14) Design Ratio
        }
        public float Eq_315_____(float fE, float fF_Ed_ser, float fd_0, float fd, float ft)
        {
            return 0.591f * MathF.Sqrt((fE * fF_Ed_ser * (fd_0 - fd)) / (MathF.Pow2(fd) * ft)); ; // Eq. (3.15) fSigma_h_Ed
        }
        public float Eq_316_____(float ff_y, float fGamma_M6_ser)
        {
            return 2.5f * ff_y / fGamma_M6_ser; // Eq. (3.16) ff_h_Ed
        }


        // Table 3.10 Design criteria for pin connections
        // Vzorce, ktore su v tabulkach alebo nie su ocislovane netreba pisat

        public float Eq_Tab310_F_v_Rd____(float fA, float ff_up, float fGamma_M2)
        {
            return 0.6f * fA * (ff_up / fGamma_M2); ; // fF_v_Rd
        }
        public float Eq_Tab310_F_b_Rd____(float ft, float fd, float ff_y, float fGamma_M0)
        {
            return 1.5f * ft * fd * (ff_y / fGamma_M0); // fF_b_Rd
        }
        public float Eq_Tab310_F_b_Rd_ser(float ft, float fd, float ff_y, float fGamma_M6_ser)
        {
            return 0.6f * ft * fd * (ff_y / fGamma_M6_ser); // fF_b_Rd_ser
        }
        public float Eq_Tab310_M_Rd______(float fW_el, float ff_yp, float fGamma_M0)
        {
            return 1.5f * fW_el * (ff_yp / fGamma_M0); // fM_Rd
        }
        public float Eq_Tab310_M_Rd_ser__(float fW_el, float ff_yp, float fGamma_M6_ser)
        {
            return 0.8f * fW_el * (ff_yp / fGamma_M6_ser); // fM_Rd_ser
        }
        // End of Table 3.10

        public float Eq_41______(float fSigma_T, float fTau_T, float fTau_L, float ff_u, float fBeta_w, float fGamma_M2)
        {
            return (MathF.Sqrt(MathF.Pow2(fSigma_T) + 3f * (MathF.Pow2(fTau_T) + MathF.Pow2(fTau_L)))) / (ff_u / (fBeta_w * fGamma_M2)); // Eq. (3.14) Design Ratio
        }
        public float Eq_41______(float fSigma_T, float ff_u, float fGamma_M2)
        {
            return fSigma_T / (0.9f * ff_u / fGamma_M2); // Eq. (3.14) Design Ratio
        }



























        //---------------------------------------------CSN_EN_1993_1_8---------------------------------------------------

        public float Eq_31_____(float ff_ub, float fA_s, float fGamma_M7)
        {
            if (!MathF.d_equal(fGamma_M7, 0f))
                return 0.7f * ff_ub * fA_s / fGamma_M7; // Eq. (3.1) F_p_Cd
            else
                return 0f;
        }
        public float Eq_32_____(float fF_b_Rd, float ff_u, float fd, float ft, float fGamma_M2)
        {
            if (!MathF.d_equal(fGamma_M2, 0f))
                return fF_b_Rd / (1.5f * ff_u * fd * ft / fGamma_M2); // Eq. (3.2) Design Ratio
            else
                return 0f;
        }
        public float Eq_33_____(float fd, float ft_p)
        {
            if (!MathF.d_equal((8f * fd) + (3f * ft_p), 0f))
                return 9f * fd / ((8f * fd) + (3f * ft_p)); // Eq. (3.3) Beta_p
            else
                return 0f;
        }
        public float Eq_34_____(float fk_t, float fk_s, float fd, float ft_b_resin, float fBeta, float ff_b_resin, float fGamma_M4)
        {
            if (!MathF.d_equal(fGamma_M4, 0f))
                return (fk_t * fk_s * fd * ft_b_resin * fBeta * ff_b_resin) / fGamma_M4; // Eq. (3.4) F_b_Rd_resin
            else
                return 0f;
        }
        public float Eq_35_____(float fL_j, float fd)
        {
            if (!MathF.d_equal(fd, 0f))
                return 1f - ((fL_j - (15f * fd)) / 200f * fd); // Eq. (3.5) Beta_Lf
            else
                return 0f;
        }
        public float Eq_36_____(float fk_s, float fn, float fMu, float fF_p_C, float fGamma_M3)
        {
            if (!MathF.d_equal(fGamma_M3, 0f))
                return ((fk_s * fn * fMu) / fGamma_M3) * fF_p_C; // Eq. (3.6) F_s_Rd
            else
                return 0f;
        }
        public float Eq_37_____(float ff_ub, float fA_s)
        {
            return 0.7f * ff_ub * fA_s; // Eq. (3.7) F_p_C
        }
        public float Eq_38a____(float fk_s, float fn, float fMu, float fF_p_C, float fF_t_Ed_ser, float fGamma_M3_ser)
        {
            if (!MathF.d_equal(fGamma_M3_ser, 0f))
                return (fk_s * fn * fMu * (fF_p_C - (0.8f * fF_t_Ed_ser))) / fGamma_M3_ser; // Eq. (3.8a) F_s_Rd_ser
            else
                return 0f;
        }
        public float Eq_38b____(float fk_s, float fn, float fMu, float fF_p_C, float fF_t_Ed, float fGamma_M3)
        {
            if (!MathF.d_equal(fGamma_M3, 0f))
                return (fk_s * fn * fMu * (fF_p_C - (0.8f * fF_t_Ed))) / fGamma_M3; // Eq. (3.8b) F_s_Rd
            else
                return 0f;
        }
        public float Eq_39_____(float ff_u, float fA_nt, float fGamma_M2, float ff_y, float fA_nv, float fGamma_M0)
        {
            return (ff_u * fA_nt / fGamma_M2) + ((1f / MathF.Sqrt(3f)) * ff_y * fA_nv / fGamma_M0); // Eq. (3.9) V_eff_1_Rd
        }
        //------------------------------------------------------10----------------------------------------------------
        public float Eq_310____(float ff_u, float fA_nt, float fGamma_M2, float ff_y, float fA_nv, float fGamma_M0)
        {
            return 0.5f * ((ff_u * fA_nt / fGamma_M2) + ((1f / MathF.Sqrt(3f)) * ff_y * fA_nv / fGamma_M0)); // Eq. (3.10) V_eff_2_Rd
        }
        public float Eq_311____(float fe_2, float fd_0, float ft, float ff_u, float fGamma_M2)
        {
            if (!MathF.d_equal(fGamma_M2, 0f))
                return (2.0f * (fe_2 - (0.5f * fd_0)) * ft * ff_u) / fGamma_M2; // Eq. (3.11) N_u_Rd
            else
                return 0f;
        }
        public float Eq_312____(float fBeta_2, float fA_net, float ff_u, float fGamma_M2)
        {
            if (!MathF.d_equal(fGamma_M2, 0f))
                return (fBeta_2 * fA_net * ff_u) / fGamma_M2; // Eq. (3.12) N_u_Rd
            else
                return 0f;
        }
        public float Eq_313____(float fBeta_3, float fA_net, float ff_u, float fGamma_M2)
        {
            if (!MathF.d_equal(fGamma_M2, 0f))
                return (fBeta_3 * fA_net * ff_u) / fGamma_M2; // Eq. (3.13) N_u_Rd
            else
                return 0f;
        }
        public float Eq_314____(float fSigma_h_Ed, float ff_h_Rd)
        {
            return fSigma_h_Ed / ff_h_Rd; // Eq. (3.14) Design Ratio
        }
        public float Eq_315____(float fE, float fF_Ed_ser, float fd_0, float fd, float ft)
        {
            if (!MathF.d_equal(fd, 0f) && !MathF.d_equal(ft, 0f))
                return 0.591f * (MathF.Sqrt((fE * fF_Ed_ser * (fd_0 - fd)) / MathF.Pow2(fd) * ft)); // Eq. (3.15) Sigma_h_Ed
            else
                return 0f;
        }
        public float Eq_316____(float ff_y, float fGamma_M6_ser)
        {
            if (!MathF.d_equal(fGamma_M6_ser, 0f))
                return 2.5f * ff_y / fGamma_M6_ser; // Eq. (3.16) f_h_Ed
            else
                return 0f;
        }
        public float Eq_42_____(float fF_w_Ed, float fF_w_Rd)
        {
            return fF_w_Ed / fF_w_Rd; // Eq. (4.2) Design Ratio
        }
        public float Eq_43_____(float ff_vw_d, float fa)
        {
            return ff_vw_d * fa; // Eq. (4.3) F_w_Rd
        }
        public float Eq_44_____(float ff_u, float fBeta_w, float fGamma_M2)
        {
            if (!MathF.d_equal(fBeta_w, 0f) && !MathF.d_equal(fGamma_M2, 0f))
                return (ff_u / MathF.Sqrt(3f)) / (fBeta_w * fGamma_M2); // Eq. (4.4) f_vw_d
            else
                return 0f;
        }
        //-----------------------------------------------------20----------------------------------------
        public float Eq_45_____(float ff_vw_d, float fA_w)
        {
            return ff_vw_d * fA_w; // Eq. (4.5) F_w_Rd
        }
        public float Eq_46a____(float ft_w, float fs, float fk, float ft_f)
        {
            return ft_w + (2f * fs) + (7f * fk * ft_f); // Eq. (4.6a) b_eff
        }
        public float Eq_46b____(float ft_f, float ft_p, float ff_y_f, float ff_y_p)
        {
            if (!MathF.d_equal(ft_p, 0f) && !MathF.d_equal(ff_y_p, 0f))
                return (ft_f / ft_p) * (ff_y_f / ff_y_p); // Eq. (4.6b) k
            else
                return 0f;
        }
        public float Eq_46d____(float fa)
        {
            return MathF.Sqrt(2f * fa); // Eq. (4.6d) s
        }
        public float Eq_47_____(float fb_eff, float ff_y_p, float ff_u_p, float fb_p)
        {
            return fb_eff / ((ff_y_p / ff_u_p) * fb_p); // Eq. (4.7) Design Ratio
        }
        public float Eq_48_____(float ft_w, float ft_f)
        {
            return (2f * ft_w) + (0.5f * ft_f); // Eq. (4.8) b_eff
        }
        public float Eq_49_____(float fL_j, float fa)
        {
            return 1.2f - (0.2f * fL_j / (150f / fa)); // Eq. (4.9) Beta_LW_1
        }
        public float Eq_410____(float fL_W)
        {
            return 1.1f - (fL_W / 17f); // Eq. (4.10) Beta_LW_2
        }
        public float Eq_52d____(float fS_j_ini, float fE, float fl_c, float fL_c)
        {
            if (!MathF.d_equal(fL_c, 0f))
                return fS_j_ini / (30f * fE * fl_c / fL_c); // Eq. (5.2d) Design Ratio
            else
                return 0f;
        }
        public float Eq_53_____(float fM_b1_Ed, float fM_b2_Ed, float fV_c1_Ed, float fV_c2_Ed, float fz)
        {
            return ((fM_b1_Ed - fM_b2_Ed) / fz) - ((fV_c1_Ed - fV_c2_Ed) / 2f); // Eq. (5.3) V_wp_Ed
        }
        //-------------------------------------------------------30------------------------------------------------
        public float Eq_61_____(float fC_f_d, float fN_c_Ed)
        {
            return fC_f_d * fN_c_Ed; // Eq. (6.1) F_f_Rd
        }
        public float Eq_62_____(float fAlpha_b, float ff_ub, float fA_s, float fGamma_Mb)
        {
            if (!MathF.d_equal(fGamma_Mb, 0f))
                return (fAlpha_b * ff_ub * fA_s) / fGamma_Mb; // Eq. (6.2) F_2_vb_Rd
            else
                return 0f;
        }
        public float Eq_63_____(float fF_f_Rd, float fn, float fF_vb_Rd)
        {
            return fF_f_Rd + (fn * fF_vb_Rd); // Eq. (6.3) F_v_Rd
        }
        public float Eq_64_____(float ff_jd, float fb_eff, float fl_eff)
        {
            return ff_jd * fb_eff * fl_eff; // Eq. (6.4) F_C_Rd
        }
        public float Eq_65_____(float ft, float ff_y, float ff_jd, float fGamma_M0)
        {
            if (!MathF.d_equal(ff_jd, 0f) && !MathF.d_equal(fGamma_M0, 0f))
                return ft * MathF.Sqrt(ff_y / (3f * ff_jd * fGamma_M0)); // Eq. (6.5) c
            else
                return 0f;
        }
        public float Eq_66_____(float fBeta_j, float fF_Rdu, float fb_ef, float fl_eff)
        {
            if (!MathF.d_equal(fb_ef, 0f) && !MathF.d_equal(fl_eff, 0f))
                return (fBeta_j * fF_Rdu) / (fb_ef / fl_eff); // Eq. (6.6) f_jd
            else
                return 0f;
        }
        public float Eq_67_____(float ff_y_wc, float fA_vc, float fGamma_M0)
        {
            if (!MathF.d_equal(fGamma_M0, 0f))
                return (0.9f * ff_y_wc * fA_vc) / (MathF.Sqrt(3f) * fGamma_M0); // Eq. (6.7) V_wp_Rd
            else
                return 0f;
        }
        public float Eq_68_____(float fM_pl_fc_Rd, float fd_s)
        {
            if (!MathF.d_equal(fd_s, 0f))
                return 4f * fM_pl_fc_Rd / fd_s; // Eq. (6.8) V_wp_add_Rd
            else
                return 0f;
        }
        public float Eq_69_____(float fOmega, float fk_wc, float fb_eff_c_wc, float ft_wc, float ff_y_wc, float fGamma_M0)
        {
            if (!MathF.d_equal(fGamma_M0, 0f))
                return (fOmega * fk_wc * fb_eff_c_wc * ft_wc * ff_y_wc) / fGamma_M0; // Eq. (6.9) F_c_wc_Rd
            else
                return 0f;
        }
        public float Eq_610____(float ft_fb, float fa_b, float ft_fc, float fs)
        {
            return ft_fb + (2f * MathF.Sqrt(2f) * fa_b) + (5f * (ft_fc + fs));
        }
        //----------------------------------------------------40------------------------------------------------------
        public float Eq_611____(float ft_fb, float fa_b, float ft_fc, float fs, float fs_p)
        {
            return ft_fb + (2f * MathF.Sqrt(2f) * fa_b) + (5f * (ft_fc + fs)) + fs_p; // Eq. (6.11) b_eff_c_wc
        }
        public float Eq_612____(float ft_a, float fr_a, float ft_fc, float fs)
        {
            return (2f * ft_a) + (0.6f * fr_a) + (5f * (ft_fc + fs)); // Eq. (6.12) b_eff_c_wc
        }
        public float Eq_613c___(float fb_eff_c_wc, float fd_wc, float ff_y_wc, float fE, float ft_wc)
        {
            if (!MathF.d_equal(fE, 0f) && !MathF.d_equal(ft_wc, 0f))
                return 0.932f * MathF.Sqrt((fb_eff_c_wc * fd_wc * ff_y_wc) / fE * MathF.Pow2(ft_wc)); // Eq. (6.13c) Lambda_p
            else
                return 0f;
        }
        public float Eq_615____(float fOmega, float fb_eff_t_wc, float ft_wc, float ff_y_wc, float fGamma_M0)
        {
            if (!MathF.d_equal(fGamma_M0, 0f))
                return (fOmega * fb_eff_t_wc * ft_wc * ff_y_wc) / fGamma_M0; // Eq. (6.15) F_t_wc_Rd
            else
                return 0f;
        }
        public float Eq_616____(float ft_fb, float fa_b, float ft_fc, float fs)
        {
            return ft_fb + ((2f * MathF.fSqrt2) * fa_b) + (5f * (ft_fc + fs)); // Eq. (6.16) b_eff_t_wc
        }
        public float Eq_617____(float ft_wc)
        {
            return 1.5f * ft_wc; // Eq. (6.17) t_w_eff
        }
        public float Eq_618____(float ft_wc)
        {
            return 2.0f * ft_wc; // Eq. (6.18) t_w_eff
        }
        public float Eq_619a___(float ft_wc)
        {
            return 1.4f * ft_wc; // Eq. (6.19a) t_w_eff
        }
        public float Eq_619b___(float ft_wc)
        {
            return 1.3f * ft_wc; // Eq. (6.19b) t_w_eff
        }
        public float Eq_620____(float fb_eff_b_fc, float ft_fb, float ff_y_fb, float fGamma_M0)
        {
            if (!MathF.d_equal(fGamma_M0, 0f))
                return (fb_eff_b_fc * ft_fb * ff_y_fb) / fGamma_M0; // Eq. (6.20) F_fc_Rd
            else
                return 0f;
        }
        //------------------------------------------------50---------------------------------------------------
        public float Eq_621____(float fM_c_Rd, float fh, float ft_fb)
        {
            if (!MathF.d_equal((fh - ft_fb), 0f))
                return fM_c_Rd / (fh - ft_fb); // Eq. (6.21) F_c_fb_Rd
            else
                return 0f;
        }
        public float Eq_622____(float fb_eff_t_wb, float ft_wb, float ff_y_wb, float fGamma_M0)
        {
            if (!MathF.d_equal(fGamma_M0, 0f))
                return fb_eff_t_wb * ft_wb * ff_y_wb / fGamma_M0; // Eq. (6.22) F_t_wb_Rd
            else
                return 0f;
        }
        public float Eq_623____(float fM_j_Ed, float fM_j_Rd)
        {
            return (fM_j_Ed / fM_j_Rd) / 1.0f; // Eq. (6.23) Design Ratio
        }
        public float Eq_624____(float fM_j_Ed, float fM_j_Rd, float fN_j_Ed, float fN_j_Rd)
        {
            return ((fM_j_Ed / fM_j_Rd) + (fN_j_Ed / fN_j_Rd)) / 1.0f; // Eq. (6.24) Design Ratio
        }
        public float Eq_626____(float fF_tr_Rd, float fF_tx_Rd, float fh_r, float fh_x)
        {
            return fF_tr_Rd / (fF_tx_Rd * fh_r / fh_x); // Eq. (6.26) Design Ratio
        }
        public float Eq_628b___(float fM_j_Ed, float fM_j_Rd, float fPsy)
        {
            return (float)Math.Pow((1.5f * fM_j_Ed / fM_j_Rd), fPsy); // Eq. (6.28b)
        }
        public float Eq_632____(float ft, float fd, float ff_ub, float ff_y)
        {
            return ft / (0.36f * fd * MathF.Sqrt(ff_ub / ff_y)); // Eq. (6.32) Design Ratio
        }
        public float Eq_633____(float fh_c, float fh_b)
        {
            return 0.025f * fh_c / fh_b; // Eq. (6.33) Phi_Cd
        }
        public float Eq_71_____(float fN_0_Ed, float fM_0_Ed, float fA_0, float fW_el_0)
        {
            return (fN_0_Ed / fA_0) + (fM_0_Ed / fW_el_0); // Eq. (7.1) Sigma_0_Ed
        }
        public float Eq_72_____(float fN_p_Ed, float fM_0_Ed, float fA_0, float fW_el_0)
        {
            return (fN_p_Ed / fA_0) + (fM_0_Ed / fW_el_0); // Eq. (7.2) Sigma_p_Ed
        }
        //-------------------------------------------------60------------------------------------------------------
        public float Eq_74_____(float fN_i_Ed, float fN_i_Rd, float fM_ip_i_Ed, float fM_ip_i_Rd, float fM_op_i_Ed, float fM_op_i_Rd)
        {
            return ((fN_i_Ed / fN_i_Rd) + (fM_ip_i_Ed / fM_ip_i_Rd) + (fM_op_i_Ed / fM_op_i_Rd)) / 1.0f; // Eq. (7.4) Design Ratio
        }
        public float Eq_75_____(float fN_i_Ed, float fN_i_Rd, float fM_ip_i_Ed, float fM_ip_i_Rd)
        {
            return ((fN_i_Ed / fN_i_Rd) + (fM_ip_i_Ed / fM_ip_i_Rd)) / 1.0f; // Eq. (7.2) Design Ratio
        }
        public float Eq_76_____(float ff_yi, float ft_i, float fb_eff, float fb_eff_s, float fGamma_M5)
        {
            if (!MathF.d_equal(fGamma_M5, 0f))
                return 2f * ff_yi * ft_i * (fb_eff + fb_eff_s) / fGamma_M5; // Eq. (7.6) N_i_Rd
            else
                return 0f;
        }
    }
}
