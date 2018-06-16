using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MATH;

namespace M_EC8
{
    class CL
    {
        /*

        public float Eq_32______(float fa_g, float fS, float fT, float fT_B, float feta)
        {
            if (fT_B > 0f)
                return fa_g * fS * (1f + (fT / fT_B) * (feta * 2.5f - 1f)); // Eq. (3.2) Se(T)
            else
                return 0f;
        }

        public float Eq_33______(float fa_g, float fS, float feta)
        {
            return fa_g * fS * feta * 2.5f; // Eq. (3.3) Se(T)
        }
        public float Eq_34______(float fa_g, float fS, float feta, float fT_C, float fT)
        {
            if (fT > 0f)
                return fa_g * fS * feta * 2.5f * (fT_C / fT); // Eq. (3.4) Se(T)
            else
                return 0f;
        }
        public float Eq_35______(float fa_g, float fS, float feta, float fT_C, float fT_D, float fT)
        {
            if (MathF.Pow2(fT) > 0f)
                return fa_g * fS * feta * 2.5f * ((fT_C * fT_D) / MathF.Pow2(fT)); // Eq. (3.5) Se(T)
            else
                return 0f;
        }
        public float Eq_37_____(float fS_e, float fT)
        {

            return fS_e * (fT) * (float)Math.Pow((fT / (2f * (float)Math.PI)), 2f); // Eq. (3.7) SDe(T)

        }
        public float Eq_38______(float fa_vg, float fT, float fT_B, float feta)
        {
            if (fT_B > 0f)
                return fa_vg * (1f + (fT / fT_B) * (feta * 3f - 1f)); // Eq. (3.8) Sve(T)
            else
                return 0f;
        }
        public float Eq_39______(float fa_vg, float feta)
        {

            return fa_vg * feta * 3f; // Eq. (3.9) Sve(T)

        }
        public float Eq_310______(float fa_vg, float feta, float fT_C, float fT)
        {
            if (fT > 0f)
                return fa_vg * feta * 3f * (fT_C / fT); // Eq. (3.10) Sve(T)
            else
                return 0f;
        }
        public float Eq_311_____(float fa_vg, float feta, float fT_C, float fT_D, float fT)
        {
            if (MathF.Pow2(fT) > 0f)
                return fa_vg * feta * 3f * ((fT_C * fT_D) / MathF.Pow2(fT)); // Eq. (3.4) Se(T)
            else
                return 0f;
        }
        public float Eq_312______(float fa_g, float fS, float fT_C, float fT_D)
        {

            return 0.025f * fa_g * fS * fT_C * fT_D; // Eq. (3.12) dg

        }
        public float Eq_412_____(float fx, float fL_e)
        {
            if (fL_e > 0f)
                return 1f + 0.6f * (fx / fL_e); // Eq. (4.12) delta
            else
                return 0f;
        }



        */






        public float Eq_32______(float fa_g, float fS, float fT, float fT_B, float fEta)
        {
            if (!MathF.d_equal(fT_B, 0))
                return fa_g * fS * (1f + (fT / fT_B) * ((fEta * 2.5f) - 1f)); // Eq. (3.2) Se(T)
            else
                return 0f;
        }
        public float Eq_33______(float fa_g, float fS, float fEta)
        {
            return fa_g * fS * fEta * 2.5f; // Eq. (3.3) Se(T)
        }
        public float Eq_34______(float fa_g, float fS, float fEta, float fT_C, float fT)
        {
            if (!MathF.d_equal(fT, 0))
                return fa_g * fS * fEta * 2.5f * fT_C / fT; // Eq. (3.4) Se(T)
            else
                return 0f;
        }
        public float Eq_35______(float fa_g, float fS, float fEta, float fT_C, float fT_D, float fT)
        {
            if (!MathF.d_equal(MathF.Pow2(fT), 0))
                return fa_g * fS * fEta * 2.5f * ((fT_C * fT_D) / MathF.Pow2(fT)); // Eq. (3.5) Se(T)
            else
                return 0f;
        }
        public float Eq_37______(float fS_e, float fT)
        {
            return fS_e * (fT) * MathF.Pow2(fT / (2f * MathF.fPI)); // Eq. (3.7) SDe(T)
        }
        public float Eq_38______(float fa_vg, float fT, float fT_B, float fEta)
        {
            if (!MathF.d_equal(fT_B, 0))
                return fa_vg * (1f + ((fT / fT_B) * ((fEta * 3f)) - 1f)); // Eq. (3.8) Sve(T)
            else
                return 0f;
        }
        public float Eq_39______(float fa_vg, float fEta)
        {
            return fa_vg * fEta * 3f; // Eq. (3.9) Sve(T)
        }
        public float Eq_310____(float fa_vg, float fEta, float fT_C, float fT)
        {
            if (!MathF.d_equal(fT, 0))
                return fa_vg * fEta * 3f * fT_C / fT; // Eq. (3.10) Sve(T)
            else
                return 0f;
        }
        public float Eq_311____(float fa_vg, float fEta, float fT_C, float fT_D, float fT)
        {
            if (!MathF.d_equal(MathF.Pow2(fT), 0))
                return fa_vg * fEta * 3f * ((fT_C * fT_D) / MathF.Pow2(fT)); // Eq. (3.4) Se(T)
            else
                return 0f;
        }
        public float Eq_312____(float fa_g, float fS, float fT_C, float fT_D)
        {
            return 0.025f * fa_g * fS * fT_C * fT_D; // Eq. (3.12) dg
        }
        //----------------------------------------10-----------------------------------------------------------------
        public float Eq_313____(float fa_g, float fS, float fT, float fT_B, float fq)
        {
            if (!MathF.d_equal(fT_B, 0f) && MathF.d_equal(fq, 0f))
                return fa_g * fS * (2 / 3f + ((fT / fT_B) * ((2.5f / fq) - 2 / 3f))); // Eq. (3.13) Sd(T)
            else
                return 0f;
        }
        public float Eq_314____(float fa_g, float fS, float fq)
        {
            if (!MathF.d_equal(fq, 0))
                return fa_g * fS * 2.5f / fq; // Eq. (3.14) Sd(T)
            else
                return 0f;
        }
        public float Eq_315____(float fa_g, float fS, float fq, float fT_C, float fT, float fBeta)
        {
            if (!MathF.d_equal(fq, 0f) && MathF.d_equal(fT_C, 0f))
                return fa_g * fS * 2.5f / fq * (fT_C / fT); // Eq. (3.15) Sd(T) 
            else
                return 0f;
        }
        public float Eq_316____(float fa_g, float fS, float fT_D, float fT, float fT_C, float fq, float fBeta)
        {
            if (!MathF.d_equal(fq, 0f) && !MathF.d_equal(MathF.Pow2(fT), 0f))
                return Math.Max(((fa_g * fS * 2.5f) / fq) * ((fT_C * fT_D) / MathF.Pow2(fT)), fBeta * fa_g); // Eq. (3.16) Sd(T) 
            else
                return 0f;
        }
        public float Eq_41a____(float fe_ox, float fr_x)
        {
            return (fe_ox) / (0.30f * fr_x); // Eq. (4.1a) Design Ratio
        }
        public float Eq_41b____(float fr_x, float fl_s)
        {
            return fr_x / fl_s;  // Eq. (4.1b) Design Ratio
        }
        public float Eq_42_____(float fPsy_E_i, float fPhi, float fPsy_2_i)
        {
            return fPhi * fPsy_2_i; // Eq. (4.2) PsyE,i
        }
        public float Eq_43_____(float fe_ai, float fL_i)
        {
            return +-0.05f * fL_i;   // Eq. (4.3) ea,i
        }
        public float Eq_44_____(float fT_1, float fT_C, float fs)
        {
            return fT_1 / (4f * fT_C); // Eq. (4.4) Design Ratio
        }
        public float Eq_45_____(float fS_d, float fT_1, float fm, float fLambda)
        {
            return fS_d * fT_1 * fm * fLambda; // Eq. (4.5) Fb
        }
        //----------------------------------------20------------------------------------------------------------
        public float Eq_46_____(float fC_t, float fH)
        {
            return (fC_t) * (float)Math.Pow(fH, 3 / 4f); // Eq. (4.6) T1
        }
        public float Eq_47_____(float fA_c)
        {
            if (!MathF.d_equal(MathF.Sqrt(fA_c), 0))
                return 0.075f / (MathF.Sqrt(fA_c)); //  Eq. (4.7) C1 
            else
                return 0f;
        }
        public float Eq_49_____(float fd)
        {
            return 2f * MathF.Sqrt(fd); // Eq. (4.9) T1 
        }
        public float Eq_412____(float fx, float fL_e)
        {
            if (!MathF.d_equal(fL_e, 0))
                return 1f + (0.6f * (fx / fL_e)); // Eq. (4.12) Delta
            else
                return 0f;
        }
        public float Eq_413____(float fk, float fn)
        {
            return fk / (3f * (MathF.Sqrt(fn))); // Eq. (4.13) Design Ratio
        }
        public float Eq_414____(float fT_k, float fs)
        {
            return fT_k / (0.20f * fs); // Eq. (4.14) Design Ratio
        }
        public float Eq_415____(float fT_j, float fT_i)
        {
            return fT_j / (0.9f * fT_i); //  Eq. (4.15) Tj
        }
        public float Eq_417____(float fe_ai, float fF_i)
        {
            return fe_ai * fF_i; // Eq. (4.17) Mai
        }
        //-------------------------------------------30-------------------------------------------------------------------
        public float Eq_423____(float fq_d, float fd_e)
        {
            return fq_d * fd_e; // Eq. (4.23) ds
        }
        public float Eq_424____(float fS_a, float fW_a, float fGamma_a, float fq_a)
        {
            if (!MathF.d_equal(fq_a, 0))
                return (fS_a * fW_a * fGamma_a) / fq_a; // Eq. (4.24) Fa
            else
                return 0f;
        }
        public float Eq_425____(float fAlpha, float fS, float fz, float fH, float fT_a, float fT_1)
        {
            if (!MathF.d_equal(fH, 0f) && !MathF.d_equal(fT_1, 0f))
                return fAlpha * fS * (3f * (1f + (fz / fH)) / (1f + MathF.Pow2(1f - (fT_a / fT_1))) - 0.5f); // Eq. (4.25) Sa
            else
                return 0f;
        }
        public float Eq_427____(float fE_d, float fR_d)
        {
            return fE_d / fR_d; // Eq. (4.27) Design Ratio
        }
        public float Eq_428____(float fP_tot, float fd_r, float fV_tot, float fh)
        {
            if (!MathF.d_equal((fV_tot * fh), 0))
                return MathF.Min(((fP_tot * fd_r) / (fV_tot * fh)), 0.1f); // Eq. (4.28) Theta
            else
                return 0f;
        }
        public float Eq_430____(float fE_F_G, float fGamma_Rd, float fOmega, float fE_F_E)
        {
            return fE_F_G + (fGamma_Rd * fOmega * fE_F_E); // Eq. (4.30) EFd
        }
        public float Eq_431____(float fd_r, float fv, float fh)
        {
            return (fd_r * fv) / (0.005f * fh); // Eq. (4.31) Design Ratio
        }
        public float Eq_432____(float fd_r, float fv, float fh)
        {
            return (fd_r * fv) / (0.0075f * fh); // Eq. (4.32) Design Ratio
        }
        public float Eq_433____(float fd_r, float fv, float fh)
        {
            return (fd_r * fv) / (0.010f * fh); // Eq. (4.33) Design Ratio
        }
        public float Eq_51_____(float fq_o, float fk_W)
        {
            return (fq_o * fk_W) / 1.5f; // Eq. (5.1) q
        }
        //--------------------------------------------40-----------------------------------------------------------------------
        public float Eq_52_____(float fk_w)
        {
            return 1f; // Eq. (5.2) kw
        }
        public float Eq_54_____(float fq_o, float fT_1, float fT_C)
        {
            if (fT_1 >= fT_C)
                return (2f * fq_o - 1f); // Eq. (5.4) mu
            else
                return 0f;
        }
        public float Eq_55_____(float fq_o, float fT_C, float fT_1)
        {
            if (fT_1 >= fT_C)
                return 1f + (2f * (fq_o - 1f)) * (fT_C / fT_1); // Eq. (5.5) Mu Phi
            else
                return 0f;
        }
        public float Eq_56_____(float fb_w, float fmin, float fb_c, float fh_Wi)
        {
            return fb_w / MathF.Min(fb_c + (fh_Wi), (2f * fb_c)); // Eq. (5.6) Design Ratio
        }
        public float Eq_57_____(float fb_wo, float fmax, float fh_s)
        {
            return fb_wo / MathF.Max(0.15f, fh_s / 20f);// Eq. (5.7) Design Ratio
        }
        public float Eq_510____(float fV_Ed, float fq)
        {
            return fV_Ed * ((fq + 1f) / 2f); // Eq. (5.10) Ved
        }
        public float Eq_511____(float fp, float ff_cd, float fMu_Phi, float fEpsilon_sy_d, float ff_yd)
        {
            if (!MathF.d_equal(fMu_Phi, 0))
                return fp + (0.0018f / (fMu_Phi * fEpsilon_sy_d) * ff_cd * ff_yd); // Eq. (5.11) pmax
            else
                return 0f;
        }
        public float Eq_512____(float ff_ctm, float ff_yk)
        {
            if (!MathF.d_equal(ff_yk, 0))
                return 0.5f * (ff_ctm / ff_yk); // Eq. (5.12) pmin
            else
                return 0f;
        }
        public float Eq_513____(float fmin, float fh_w, float fd_bw, float fd_bL)
        {
            return MathF.Min(fh_w / 4f, 24f * fd_bw, 255f, 8f * fd_bL); // Eq. (5.13) s
        }
        public float Eq_514_____(float fmax, float fh_c, float fl_d)
        {
            return MathF.Max(fh_c, fl_d / 6f, 0.45f); // Eq. (5.14) lcr
        }
        //-------------------------------------------------50------------------------------------------------------------
        public float Eq_515____(float fAlpha, float fOmega_wd, float fMu_Phi, float fv_d, float fEpsilon_sy_d, float fb_c, float fb_o)
        {
            if (!MathF.d_equal(fb_o, 0))
                return (fAlpha * fOmega_wd) / (30f * fMu_Phi * fv_d * fEpsilon_sy_d * (fb_c / fb_o)) - 0.035f; // Eq. (5.15) Design Ratio
            else
                return 0f;
        }
        public float Eq_517a___(float fs, float fb_o, float fh_o)
        {
            if (!MathF.d_equal(fb_o, 0f) && !MathF.d_equal(fh_o, 0f))
                return (1f - (fs / 2f * fb_o)) * (1f - (fs / (2f * fh_o))); // Eq. (5.17a) Alpha s
            else
                return 0f;
        }
        public float Eq_516b___(float fAlpha_n)
        {
            return fAlpha_n / 1f; // Eq. (5.16b) Design Ratio
        }
        public float Eq_517b___(float fs, float fD_o)
        {
            if (!MathF.d_equal(fD_o, 0))
                return MathF.Pow2(1f - (fs / (2f * fD_o))); // Eq. (5.17b) Alpha s
            else
                return 0f;

        }
        public float Eq_516c___(float fAlpha_n)
        {
            return fAlpha_n / 1f; // Eq. (5.16c) Design Ratio
        }
        public float Eq_517c___(float fs, float fD_o)
        {
            if (!MathF.d_equal(fD_o, 0))
                return (1 - (fs / (2f * fD_o))); // Eq. (5.17c)  Alpha s
            else
                return 0f;
        }
        public float Eq_518____(float fmin, float fb_o, float fd_bL)
        {
            return MathF.Min(fb_o / 2f, 175f, 8f * fd_bL); // Eq. (5.18) s
        }
        public float Eq_520____(float fMu_Phi, float fv_d, float fOmega_v, float fEpsilon_sy_d, float fb_c, float fb_o)
        {
            if (!MathF.d_equal(fb_o, 0))
                return 30f * fMu_Phi * (fv_d + fOmega_v) * (fEpsilon_sy_d * (fb_c / fb_o)) - 0.035f; // Eq. (5.20) Alpha * Omega wd
            else
                return 0f;
        }
        public float Eq_521____(float fv_d, float fOmega_v, float fl_w, float fb_c, float fb_o)
        {
            if (!MathF.d_equal(fb_o, 0))
                return (fv_d + fOmega_v) * (fl_w * fb_c) / fb_o; // Eq. (5.21) Xu
            else
                return 0f;
        }
        public float Eq_522____(float fGamma_Rd, float fA_s1, float fA_s2, float ff_yd, float fV_c)
        {
            return fGamma_Rd * (fA_s1 + fA_s2) * ff_yd - fV_c; // Eq. (5.22) Vjhd
        }
        //------------------------------------------------60----------------------------------------------------------------------------
        public float Eq_523____(float fGamma_Rd, float fA_s1, float ff_yd, float fV_c)
        {
            return fGamma_Rd * fA_s1 * ff_yd - fV_c; // Eq. (5.23) Vjhd
        }
        public float Eq_524____(float fEpsilon, float fV_Ed)
        {
            return fEpsilon * fV_Ed; // Eq. (5.24) Ved
        }
        public float Eq_525____(float fGamma_Rd, float fM_Rd, float fS_e, float fT_c, float fq, float fM_Ed, float fT_1)
        {
            if (!MathF.d_equal(fq, 0f) && !MathF.d_equal(fM_Ed, 0f) && !MathF.d_equal(fT_1, 0f))
                return fq * MathF.Sqrt(MathF.Pow2((fGamma_Rd / fq) * (fM_Rd / fM_Ed)) + 0.1f * (MathF.Pow2((fS_e * fT_c) / (fS_e * fT_1))) / fq); // Eq. (5.25) Epsilon
            else
                return 0f;
        }
        public float Eq_526____(float fGamma_Rd, float fM_Rd, float fM_Ed, float fV_Ed, float fq)
        {
            if (!MathF.d_equal(fM_Ed, 0))
                return (fGamma_Rd * (fM_Rd / fM_Ed) * fV_Ed) / (fq * fV_Ed); // Eq. (5.26) Ved
            else
                return 0f;
        }
        public float Eq_527____(float fXi, float ff_ctd, float fb_wd, float fd)
        {
            return (2f + fXi) * ff_ctd * fb_wd * fd; // Eq. (5.27) |Ve|max
        }
        public float Eq_528____(float fA_s, float ff_yd, float fAlpha)
        {
            return 2f * fA_s * ff_yd * (float)Math.Sin(fAlpha); // Eq. (5.28) 0.5 Vemax
        }
        public float Eq_529____(float fmin, float fh_w, float fd_bw, float fd_bL)
        {
            return MathF.Min(fh_w / 4f, 24f * fd_bw, 175f, 6f * fd_bL); // Eq. (5.29)  s
        }
        public float Eq_530____(float fmax, float fh_c, float fl_c1)
        {
            return MathF.Max(1.5f * fh_c, fl_c1 / 6f, 0.6f); // Eq. (5.30) lcr
        }
        public float Eq_531____(float fd_bw, float fd_bL_max, float ff_ydL, float ff_ydw)
        {
            if (!MathF.d_equal(ff_ydw, 0))
                return fd_bw / (0.4f * fd_bL_max * (float)Math.Sqrt((ff_ydL / ff_ydw))); // Eq. (5.31) Design Ratio
            else
                return 0f;
        }
        public float Eq_532____(float fb_o, float fd_bL)
        {
            return MathF.Min(fb_o / 3f, 125f, 6f * fd_bL); // Eq. (5.32) s
        }
        //-----------------------------------------------70--------------------------------------------------------------------------------
        public float Eq_533____(float fEta, float ff_cd, float fv_d, float fb_j, float fh_jc)
        {
            if (!MathF.d_equal(fEta, 0))
                return fEta * ff_cd * (float)Math.Sqrt((1f - (fv_d / fEta))) * fb_j * fh_jc; // Eq. (5.33) Vjhd
            else
                return 0f;
        }
        public float Eq_534a___(float fb_c, float fb_w, float fh_c)
        {

            return MathF.Min(fb_c, (fb_w + 0.5f * fh_c)); // Eq. (5.34a) bj

        }
        public float Eq_534b___(float fb_w, float fb_c, float fh_c)
        {

            return MathF.Min(fb_w, (fb_c + 0.5f * fh_c)); // Eq: (5.34b) bj

        }
        public float Eq_535____(float fA_sh, float ff_ywd, float fh_jw, float fV_jhd, float fb_j, float fh_jc, float ff_ctd, float fv_d, float ff_cd)
        {
            if (!MathF.d_equal((fb_j * fh_jc), 0))
                return ((fA_sh * ff_ywd) / (fb_j * fh_jw)) / (MathF.Pow2(fV_jhd / fb_j * fh_jc) / (ff_ctd + (fv_d * ff_cd))) - 1f; // Eq. (5.35) Design Ratio
            else
                return 0f;
        }
        public float Eq_536a___(float fA_sh, float ff_ywd, float fGamma_Rd, float fA_s1, float fA_s2, float ff_yd, float fv_d)
        {
            return (fA_sh * ff_ywd) / (fGamma_Rd * (fA_s1 + fA_s2) * ff_yd * (1f - (0.8f * fv_d))); // Eq. (5.36a) Design Ratio
        }
        public float Eq_536b___(float fA_sh, float ff_ywd, float fGamma_Rd, float fA_s2, float ff_yd, float fv_d)
        {
            return (fA_sh * ff_ywd) / (fGamma_Rd * fA_s2 * ff_yd * (1f - (0.8f * fv_d))); // Eq. (5.36b) Design Ratio
        }
        public float Eq_537____(float fA_sv_i, float fA_sh, float fh_jc, float fh_jw)
        {
            if (!MathF.d_equal(fh_jw, 0))
                return fA_sv_i / ((2f / 3f) * fA_sh * (fh_jc / fh_jw)); // Eq. (5.37) Design Ratio
            else
                return 0f;
        }
        public float Eq_538____(float fV_Ed, float fV_Rd_c, float fRho_n, float ff_yd_h, float fb_wo, float fAlpha_s, float fl_w)
        {
            return fV_Ed / (fV_Rd_c + (0.75f * fRho_n * ff_yd_h * fb_wo * fAlpha_s * fl_w)); // Eq. (5.38) Design Ratio
        }
        public float Eq_539____(float fRho_n, float ff_yd_h, float fb_wo, float fz, float fRho_v, float ff_yd_v, float fN_Ed)
        {
            return (fRho_n * ff_yd_h * fb_wo * fz) / ((fRho_v * ff_yd_v * fb_wo * fz) + MathF.Min(fN_Ed)); // Eq. (5.39) Design Ratio
        }
        public float Eq_540____(float fV_dd, float fV_id, float fV_fd)
        {
            return fV_dd + fV_id + fV_fd; // Eq. (5.40) VRd,S
        }
        //---------------------------------------80---------------------------------------------------------------
        public float Eq_544____(float ff_ck)
        {
            return 0.6f * (1f - (ff_ck / 250f)); // Eq. (5.44)  Eta
        }
        public float Eq_548____(float fV_Ed, float ff_ctd, float fb_w, float fd)
        {
            return fV_Ed / (ff_ctd * fb_w * fd); // Eq. (5.48) Design Ratio
        }
        public float Eq_549____(float fV_Ed, float fA_si, float ff_yd, float fAlpha)
        {
            return fV_Ed / (2f * fA_si * ff_yd * (float)Math.Sin(fAlpha)); // Eq. (5.49) Design Ratio
        }
        public float Eq_550b___(float fd_bL, float fh_c, float ff_ctm, float fv_d, float fGamma_Rd, float ff_yd)
        {
            if (!MathF.d_equal((fGamma_Rd * ff_yd), 0))
                return (fd_bL / fh_c) / (((7.5f * ff_ctm) / (fGamma_Rd * ff_yd)) * (1f + (0.8f * fv_d))); // Eq. (5.50b) Design Ratio
            else
                return 0f;
        }
        public float Eq_551____(float fh)
        {
            return MathF.Min(fh / 4f, 100); // Eq.(5.51) s
        }
        public float Eq_552____(float fs, float fd_bL, float ff_yld, float ff_ywd)
        {
            if (!MathF.d_equal(ff_ywd, 0))
                return fs * (fd_bL / 50f) * (ff_yld / ff_ywd); // Eq. (5.52) Ast
            else
                return 0f;
        }
        public float Eq_553____(float fk_p, float fq)
        {
            return fk_p * fq; // Eq. (5.53) qp
        }
        public float Eq_61_____(float fR_d, float fGamma_ov, float fR_fy)
        {
            return fR_d / (1.1f * fGamma_ov * fR_fy); // Eq. (6.1) Design Ratio
        }
        public float Eq_62_____(float fM_Ed, float fM_pl_Rd)
        {
            return (fM_Ed / fM_pl_Rd) / 1.0f; // Eq. (6.2) Design Ratio
        }
        public float Eq_63_____(float fN_ed, float fN_pl_Rd)
        {
            return (fN_ed / fN_pl_Rd) / 0.15f; // Eq. (6.3) Design Ratio
        }
        //-------------------------------------------90-------------------------------------------------------------------
        public float Eq_64_____(float fV_Ed, float fV_pl_Rd)
        {
            return (fV_Ed / fV_pl_Rd) / 0.5f; // Eq. (6.4) Design Ratio
        }
        public float Eq_65_____(float fV_Ed_G, float fV_Ed_M)
        {
            return fV_Ed_G + fV_Ed_M; // Eq. (6.5) VEd
        }
        public float Eq_66_____(float fM_Ed_G, float fGamma_ov, float fOmega, float fM_Ed_E)
        {
            return fM_Ed_G + 1.1f * fGamma_ov * fOmega * fM_Ed_E; // Eq. (6.6) MEd
        }
        public float Eq_67_____(float fV_Ed, float fV_pl_Rd)
        {
            return (fV_Ed / fV_pl_Rd) / 0.5f; // Eq. (6.7) Design Ratio
        }
        public float Eq_68_____(float fV_wp_Ed, float fV_wp_Rd)
        {
            return (fV_wp_Ed / fV_wp_Rd) / 1.0f; // Eq. (6.8) Design Ratio
        }
        public float Eq_69_____(float fV_wp_Ed, float fV_wb_Rd)
        {
            return fV_wp_Ed / fV_wb_Rd; // Eq. (6.9) Design Ratio
        }
        public float Eq_610____(float fDelta, float fL)
        {
            if (!MathF.d_equal(fL, 0))
                return fDelta / 0.5f * fL; // Eq. (6.10) Theta p
            else
                return 0f;
        }
        public float Eq_612____(float fN_pl_Rd, float fM_Ed, float fN_Ed_G, float fGamma_ov, float fOmega, float fN_Ed_E)
        {
            return (fN_pl_Rd * (fM_Ed)) / (fN_Ed_G + (1.1f * fGamma_ov * fOmega * fN_Ed_E)); // Eq. (6.12) Design Ratio
        }
        public float Eq_613____(float ff_y, float fb, float ft_f, float fd)
        {
            return ff_y * fb * ft_f * (fd - ft_f); // Eq. (6.13) Mp,link
        }
        public float Eq_614____(float ff_y, float ft_w, float fd, float ft_f)
        {
            return (ff_y / (float)Math.Sqrt(3f)) * ft_w * (fd - ft_f); // Eq. (6.14) Vp,link
        }
        //---------------------------------------100-------------------------------------------------------------
        public float Eq_615____(float fV_p_link)
        {
            return fV_p_link; // Eq. (6.15) VEd
        }
        public float Eq_616____(float fM_p_link)
        {
            return fM_p_link; // Eq. (6.16) MEd
        }
        public float Eq_621____(float fM_p_link, float fV_p_link)
        {
            if (!MathF.d_equal(fV_p_link, 0))
                return 1.6f * fM_p_link / fV_p_link; // Eq. (6.21) e<es
            else
                return 0f;
        }
        public float Eq_622____(float fM_p_link, float fV_p_link)
        {
            if (!MathF.d_equal(fV_p_link, 0))
                return 3.0f * fM_p_link / fV_p_link; // Eq. (6.22) e>eL
            else
                return 0f;
        }
        public float Eq_623____(float fe_L)
        {
            return fe_L; // Eq. (6.23) es<e<eL
        }
        public float Eq_624____(float fAlpha, float fM_p_link, float fV_p_link)
        {
            if (!MathF.d_equal(fV_p_link, 0))
                return 0.8f * (1f + fAlpha) * (fM_p_link / fV_p_link); // Eq. (6.24) e<es
            else
                return 0f;
        }
        public float Eq_625____(float fAlpha, float fM_p_link, float fV_p_link)
        {
            if (!MathF.d_equal(fV_p_link, 0))
                return 1.5f * (1f + fAlpha) * (fM_p_link / fV_p_link); // Eq. (6.25) e>el
            else
                return 0f;
        }
        public float Eq_626_____(float fe_L)
        {
            return fe_L; // Eq. (6.26) es<e
        }
        public float Eq_630____(float fN_Ed, float fM_Ed, float V_Ed, float fN_Ed_G, float fGamma_ov, float fOmega, float fN_Ed_E)
        {
            return fN_Ed_G + 1.1f * fGamma_ov * fOmega * fN_Ed_E; // Eq. (6.30) NRd(MEd, VEd)
        }
        public float Eq_631____(float fE_d, float fE_d_G, float fGamma_ov, float fOmega_i, float fE_d_E)
        {
            return fE_d / (fE_d_G + (1.1f * fGamma_ov * fOmega_i * fE_d_E)); // Eq. (6.31) Design Ratio
        }
        //----------------------------------------110--------------------------------------------------------------
        public float Eq_71_____(float fE_a, float fE_cm)
        {
            return fE_a / fE_cm; // Eq. (7.1) n
        }
        public float Eq_73_____(float fV_wp_Ed, float fV_wp_Rd)
        {
            return fV_wp_Ed / (0.8f * fV_wp_Rd); // Eq. (7.3)  Design Ratio
        }
        public float Eq_74_____(float fx, float fd, float fEpsilon_cu2, float fEpsilon_a)
        {

            if (!MathF.d_equal((fEpsilon_cu2 + fEpsilon_a), 0))
                return (fx / fd) / (fEpsilon_cu2 / (fEpsilon_cu2 + fEpsilon_a));  // Eq. (7.4) Design Ratio
            else
                return 0f;
        }
        public float Eq_75_____(float fAlpha, float fOmega_wd, float fMu_Phi, float fv_d, float fEpsilon_sy_d, float fb_c, float fb_o)
        {
            if (!MathF.d_equal(fb_o, 0))
                return (fAlpha * fOmega_wd) / ((30f * fMu_Phi * fv_d * fEpsilon_sy_d * (fb_c / fb_o)) - 0.035f); // Eq. (7.5) Design Ratio
            else
                return 0f;
        }
        public float Eq_76_____(float fN_Ed, float fA_a, float ff_yd, float fA_c, float ff_cd, float fA_s, float ff_sd)
        {

            if (!MathF.d_equal((fA_a * ff_yd + fA_c * ff_cd + fA_s * ff_sd), 0))
                return fN_Ed / (fA_a * ff_yd + fA_c * ff_cd + fA_s * ff_sd); // Eq. (7.6) vd=Ned/Npl,Rd
            else
                return 0f;
        }
        public float Eq_77_____(float fb_o, float fd_bL)
        {
            return MathF.Min((fb_o / 2f), 260f, 9f * fd_bL); // Eq. (7.7) s
        }
        public float Eq_78_____(float fb_o, float fd_bL)
        {
            return MathF.Min((fb_o / 2f), 175f, 8f * fd_bL); // Eq. (7.8) s
        }

        public float Eq_712____(float fb, float ft_f, float ff_ydf, float ff_ydw)
        {

            if (!MathF.d_equal(ff_ydw, 0))
                return MathF.Sqrt((fb * ft_f / 8f) * (ff_ydf / ff_ydw)); // Eq. (7.12) dbw
            else
                return 0f;
        }
        public float Eq_713____(float fI_1, float fI_2)
        {
            return (0.6f * fI_1) + (0.4f * fI_2); // Eq. (7.13) Ieq
        }
        public float Eq_714____(float fE, float fI_a, float fr, float fE_cm, float fI_c, float fI_s)
        {
            return 0.9f * (fE * fI_a + fr * fE_cm * fI_c + fE * fI_s); // Eq. (7.14) (EIc)
        }
        //----------------------------------------120------------------------------------------------------------
        public float Eq_716____(float fM_p_link, float fV_p_link)
        {
            if (!MathF.d_equal(fV_p_link, 0))
                return (2f * fM_p_link) / fV_p_link;  // Eq. (7.16) e
            else
                return 0f;
        }
        public float Eq_717____(float fM_p_link, float fV_p_link)
        {
            if (!MathF.d_equal(fV_p_link, 0))
                return (2f * fM_p_link) / fV_p_link; // Eq. (7.17) e
            else
                return 0f;
        }
        public float Eq_718____(float fV_Ed, float fV_Rd)
        {
            return fV_Ed / fV_Rd; // Eq. (7.18) Design Ratio
        }
        public float Eq_719____(float fA_pl, float ff_yd)
        {
            return (fA_pl * ff_yd) / (float)Math.Sqrt(3f); // Eq. (7.19) VRd
        }
        public float Eq_A1_____(float fa_g, float fS, float fT_C, float fT_D, float fEta, float fT, float fT_E, float fT_F)
        {
            if (!MathF.d_equal((fT_F - fT_E), 0))
                return 0.025f * fa_g * fS * fT_C * fT_D * (2.5f * fEta + ((fT - fT_E) / (fT_F - fT_E)) * (1f - (2.5f * fEta))); // Eq. (A.1) SDe(t)
            else
                return 0f;
        }
        public float Eq_A2_____(float fd_g)
        {
            return fd_g; // Eq. (A.2) SDe(t)
        }
        public float Eq_B1_____(float fm_i, float fPhi_i)
        {
            return fm_i * fPhi_i; // Eq. (B.1) Fi
        }
        public float Eq_B4_____(float fF_b, float fTau)
        {
            return fF_b / fTau; // Eq. (B.4) F*
        }
        public float Eq_B5_____(float fd_n, float fTau)
        {
            return fd_n / fTau; // Eq. (B.5) d*
        }
        public float Eq_C4_____(float fb_b, float fd_eff, float ff_cd)
        {
            return fb_b * fd_eff * ff_cd; // Eq. (C.4) FRd1
        }
        //-----------------------------------------130----------------------------------------------------
        public float Eq_C5_____(float fh_c, float fd_eff, float ff_cd)
        {
            return 0.7f * fh_c * fd_eff * ff_cd; // Eq. (C.5) FRd2
        }
        public float Eq_C6_____(float fA_T, float fF_Rd2, float ff_yd_T)
        {
            if (!MathF.d_equal(ff_yd_T, 0))
                return fA_T / (fF_Rd2 / ff_yd_T); // Eq. (C.6) Design Ratio
            else
                return 0f;
        }
        public float Eq_C7_____(float fb_eff, float fd_eff, float ff_cd)
        {
            return fb_eff * fd_eff * ff_cd; // Eq. (C.7) FRd1+FRd2
        }
        public float Eq_C8_____(float fn, float fP_Rd)
        {
            return fn * fP_Rd; // Eq. (C.8) FRd3
        }
        public float Eq_C10____(float fb_b, float fd_eff, float ff_cd)
        {
            return fb_b * fd_eff * ff_cd; // Eq. (C.10) FRd1
        }
        public float Eq_C11____(float fh_c, float fd_eff, float ff_cd)
        {
            return 0.7f * fh_c * fd_eff * ff_cd; // Eq. (C.11) FRd2
        }
        public float Eq_C12____(float fA_T, float fF_Rd2, float ff_yd_T)
        {
            return fA_T / (fF_Rd2 / ff_yd_T); // Eq. (C.12) Design Ratio
        }
        public float Eq_C13____(float fh_c, float fb_b, float fd_eff, float ff_cd)
        {
            return ((0.7f * fh_c) + fb_b) * fd_eff * ff_cd; // Eq. (C.13) FRd1+FRd2
        }
        public float Eq_C14____(float fA_s, float ff_yd, float fb_eff, float fd_eff, float ff_cd)
        {
            return (fA_s * ff_yd) + (fb_eff * fd_eff * ff_cd); // Eq. (C.14) Fst+Fsc
        }
        public float Eq_C15____(float fF_sc, float fF_st, float fF_Rd1, float fF_Rd2)
        {
            return (1.2f * (fF_sc + fF_st)) / (fF_Rd1 + fF_Rd2); // Eq. (C.15) Design Ratio
        }
        public float Eq_C16____(float fn, float fP_Rd)
        {
            return fn * fP_Rd; // Eq. (C.16) FRcd
        }
        //------------------------------------------------140----------------------------------------------------------------
        public float Eq_C17____(float fh_c, float fb_b, float fd_eff, float ff_cd, float fn, float fP_Rd)
        {
            return (0.7f * fh_c + fb_b) * fd_eff * ff_cd + fn * fP_Rd; // Eq. (C.17) FRd1+Frd2+Frd3
        }
        public float Eq_C18____(float fF_sc, float fF_st, float fF_Rd1, float fF_Rd2, float fF_Rd3)
        {
            return (1.2f * (fF_sc + fF_st)) / (fF_Rd1 + fF_Rd2 + fF_Rd3); // Eq. (C.18) Design Ratio
        }
    }
}
