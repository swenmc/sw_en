using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MATH;

namespace M_EC3.SOLV.CODE
{
    public class EN1993_1_2
    {
        //-------------------------------------------------EN_CSN_1993_1_2------------------------------------------

        public float Eq_22a____(float fx_k_Theta, float fGamma_M_fi)
        {
            if (!MathF.d_equal(fGamma_M_fi, 0f))
                return fx_k_Theta / fGamma_M_fi; // Eq. (2.2a) xd,fi
            else
                return 0f;
        }
        public float Eq_22b____(float fx_k_Theta, float fGamma_M_fi)
        {
            if (!MathF.d_equal(fx_k_Theta, 0f))
                return fGamma_M_fi / fx_k_Theta; // Eq. (2.2a) xd,fi
            else
                return 0f;
        }
        public float Eq_23_____(float fE_fi_d, float fR_fi_d_t)
        {
            if (!MathF.d_equal(fR_fi_d_t, 0f))
                return fE_fi_d / fR_fi_d_t; // Eq. (2.3) Design Ratio
            else
                return 0f;
        }
        public float Eq_25_____(float fG_k, float fPsy_fi, float fQ_k_1, float fGamma_G, float fGamma_Q_1)
        {
            if (!MathF.d_equal((fGamma_G * fG_k) + (fGamma_Q_1 * fQ_k_1), 0f))
                return (fG_k + (fPsy_fi * fQ_k_1)) / ((fGamma_G * fG_k) + fGamma_Q_1 * fQ_k_1); // Eq. (2.5) Eta fi
            else
                return 0f;
        }
        public float Eq_25a____(float fG_k, float fPsy_fi, float fQ_k_1, float fGamma_G, float fGamma_Q_1, float fPsy_0_1)
        {
            if (!MathF.d_equal((fGamma_G * fG_k) + (fGamma_Q_1 * fPsy_0_1 * fQ_k_1), 0f))
                return (fG_k + (fPsy_fi * fQ_k_1) / (fGamma_G * fG_k) + (fGamma_Q_1 * fPsy_0_1 * fQ_k_1)); // Eq. (2.5a) Eta fi
            else
                return 0f;
        }
        public float Eq_25b____(float fG_k, float fPsy_fi, float fQ_k_1, float fGamma_G, float fXi, float fGamma_Q_1, float fPsy_0_1)
        {
            if (!MathF.d_equal(fXi * fGamma_G + (fGamma_Q_1 * fQ_k_1), 0f))
                return (fG_k + (fPsy_fi * fQ_k_1)) / ((fXi * fGamma_G * fG_k) + (fGamma_Q_1 * fQ_k_1)); // Eq. (2.5b) Eta fi
            else
                return 0f;
        }
        public float Eq_31a____(float fTheta_a)
        {
            return (1.2f * (float)Math.Pow(10f, -5f) * fTheta_a) + (0.4f * (float)Math.Pow(10f, -8f) * MathF.Pow2(fTheta_a)) - (2.416f * (float)Math.Pow(10f, -4f)); // Eq. (3.1a) delta lIl
        }
        public float Eq_31c____(float fTheta_a)
        {
            return 2f * (float)Math.Pow(10f, -5f) * fTheta_a - (6.2f * (float)Math.Pow(10f, -3f)); // Eq. (3.1c) delta lIl
        }
        public float Eq_32b____(float fTheta_a)
        {
            if (!MathF.d_equal((738f - fTheta_a), 0f))
                return 666f + (13002f / (738f - fTheta_a)); // Eq. (3.2b) ca
            else
                return 0f;
        }
        public float Eq_32c____(float fTheta_a)
        {
            if (!MathF.d_equal((fTheta_a - 731f), 0f))
                return 545f + (17820f / (fTheta_a - 731f));
            else
                return 0f;
        }
        //-------------------------------------------------10------------------------------------------------------------
        public float Eq_33a____(float fTheta_a)
        {
            return 54f - (3.33f * (float)Math.Pow(10f, -2f) * fTheta_a); // Eq. (3.3a) Lambda a
        }
        public float Eq_42_____(float ff_y)
        {
            if (!MathF.d_equal(ff_y, 0f))
                return 0.85f * MathF.Sqrt(235f / ff_y); // Eq. (4.2) Epsilon
            else
                return 0f;
        }
        public float Eq_43_____(float fk_y_Theta, float fN_Rd, float fGamma_M_0, float fGamma_M_fi)
        {
            if (!MathF.d_equal(fGamma_M_fi, 0f))
                return fk_y_Theta * fN_Rd * (fGamma_M_0 / fGamma_M_fi); // Eq. (4.3) Nfi,Theta,Rd
            else
                return 0f;
        }
        public float Eq_45_____(float fChi_fi, float fA, float fk_y_Theta, float ff_y, float fGamma_M_fi)
        {
            if (!MathF.d_equal(fGamma_M_fi, 0f))
                return (fChi_fi * fA * fk_y_Theta * ff_y) / fGamma_M_fi; // Eq. (4.5) Nb,fi,t,Rd
            else
                return 0f;
        }
        public float Eq_46_____(float fPhi_Theta, float fLambda_Theta)
        {
            if (!MathF.d_equal(fPhi_Theta + MathF.Sqrt(MathF.Pow2(fPhi_Theta) - MathF.Pow2(fLambda_Theta)), 0f))
                return 1 / (fPhi_Theta + (MathF.Sqrt(MathF.Pow2(fPhi_Theta) - MathF.Pow2(fLambda_Theta)))); // Eq. (4.6) Chi fi
            else
                return 0f;
        }
        public float Eq_47_____(float fLambda, float fk_y_Theta, float fk_E_Theta)
        {
            if (!MathF.d_equal(fk_E_Theta, 0f))
                return fLambda * MathF.Pow2(fk_y_Theta / fk_E_Theta); // Eq. (4.7) Lambda_Theta
            else
                return 0f;
        }
        public float Eq_48_____(float fk_y_Theta, float fGamma_M_0, float fGamma_M_fi, float fM_Rd)
        {
            if (!MathF.d_equal(fGamma_M_fi, 0f))
                return fk_y_Theta * (fGamma_M_0 / fGamma_M_fi) * fM_Rd; // Eq. (4.8) Mfi_Theta_Rd
            else
                return 0f;
        }
        public float Eq_410____(float fM_fi_Theta_Rd, float fk_1, float fk_2)
        {
            if (!MathF.d_equal((fk_1 * fk_2), 0f))
                return fM_fi_Theta_Rd / (fk_1 * fk_2); // Eq. (4.10) M_fi_t_Rd
            else
                return 0f;
        }
        public float Eq_411____(float fChi_LT_fi, float fW_pl_y, float fk_y_Theta_com, float ff_y, float fGamma_M_fi)
        {
            if (!MathF.d_equal(fGamma_M_fi, 0f))
                return fChi_LT_fi * fW_pl_y * fk_y_Theta_com * ff_y / fGamma_M_fi; // Eq. (4.11) M_b_fi_t_Rd
            else
                return 0f;
        }
        public float Eq_412____(float fPhi_LT_Theta_com, float fLambda_LT_Theta_com)
        {
            if (!MathF.d_equal(fPhi_LT_Theta_com + MathF.Sqrt(MathF.Pow2(fPhi_LT_Theta_com) - MathF.Pow2(fLambda_LT_Theta_com)), 0f))
                return 1 / (fPhi_LT_Theta_com + (MathF.Sqrt(MathF.Pow2(fPhi_LT_Theta_com) - MathF.Pow2(fLambda_LT_Theta_com)))); // Eq. (4.12) Chi_LT_fi
            else
                return 0f;
        }
        //---------------------------------------------------------------20-------------------------------------------------------
        public float Eq_414____(float ff_y)
        {
            if (!MathF.d_equal(ff_y, 0f))
                return 0.65f * MathF.Sqrt(235f / ff_y); // Eq. (4.14) Alpha
            else
                return 0f;
        }
        public float Eq_416____(float fk_y_Theta_web, float fV_Rd, float fGamma_M_0, float fGamma_M_fi)
        {
            if (!MathF.d_equal(fGamma_M_fi, 0f))
                return fk_y_Theta_web * fV_Rd * (fGamma_M_0 / fGamma_M_fi); // Eq. (4.16) V_fi_t_Rd
            else
                return 0f;
        }
        public float Eq_417____(float fk_y_Theta, float fM_Rd, float fGamma_M_0, float fGamma_M_fi)
        {
            if (!MathF.d_equal(fGamma_M_fi, 0f))
                return fk_y_Theta * fM_Rd * (fGamma_M_0 / fGamma_M_fi); // Eq. (4.17) M_fi_t_Rd
            else
                return 0f;
        }
        public float Eq_418____(float fk_y_Theta_max, float fM_Rd, float fGamma_M_0, float fGamma_M_fi, float fk_1, float fk_2)
        {
            if (!MathF.d_equal(fGamma_M_fi, 0f) && !MathF.d_equal((fk_1 * fk_2), 0f))
                return fk_y_Theta_max * fM_Rd * (fGamma_M_0 / fGamma_M_fi) / (fk_1 / fk_2); // Eq. (4.18) M_fi_t_Rd
            else
                return 0f;
        }
        public float Eq_419____(float fChi_LT_fi, float fW_el_y, float fk_y_Theta_com, float ff_y, float fGamma_M_fi)
        {
            if (!MathF.d_equal(fGamma_M_fi, 0f))
                return fChi_LT_fi * fW_el_y * fk_y_Theta_com * ff_y / fGamma_M_fi; // Eq. (4.19) M_b_fi_t_Rd
            else
                return 0f;
        }
        public float Eq_420____(float fk_y_Theta_web, float fV_Rd, float fGamma_M_0, float fGamma_M_fi)
        {
            if (!MathF.d_equal(fGamma_M_0, 0f))
                return fk_y_Theta_web * fV_Rd * (fGamma_M_0 / fGamma_M_fi); // Eq. (4.20) V_fi_t_Rd
            else
                return 0f;
        }
        public float Eq_421a___(float fN_fi_Ed, float fk_y, float fM_y_Ed, float fk_z, float fM_z_fi_Ed, float fChi_z_fi, float fA, float fk_y_Theta, float ff_y, float fGamma_M_fi, float fW_pl_y, float fW_pl_z)
        {
            return (fN_fi_Ed / (fChi_z_fi * fA * fk_y_Theta * (ff_y / fGamma_M_fi)) + ((fk_y * fM_z_fi_Ed) / (fW_pl_y * fk_y_Theta * (ff_y / fGamma_M_fi)) + ((fk_z * fM_z_fi_Ed) / ((fW_pl_z * fk_y_Theta) * (ff_y / fGamma_M_fi)) / 1f)));
        }
        public float Eq_423____(float fE_fi_d, float fR_fi_d_0)
        {
            if (!MathF.d_equal(fR_fi_d_0, 0f))
                return fE_fi_d / fR_fi_d_0; // Eq. (4.23) Mu0
            else
                return 0f;
        }
        public float Eq_424____(float fEta_fi, float fGamma_M_fi, float fGamma_M0)
        {
            if (!MathF.d_equal(fGamma_M0, 0f))
                return fEta_fi * (fGamma_M_fi / fGamma_M0); // Eq. (4.24) Mu0
            else
                return 0f;
        }
        public float Eq_425____(float fk_sh, float fA_m, float fV, float fh_net, float fDeltat, float fc_a, float fRho_a)
        {
            if (!MathF.d_equal((fc_a * fRho_a), 0f))
                return fk_sh * ((fA_m / fV) / fc_a * fRho_a) * fh_net * fDeltat; // Eq. (4.25) DeltaTheta_a_t
            else
                return 0f;
        }
        //----------------------------------------------30------------------------------------------------------------------
        public float Eq_A1a____(float ff_u_Theta, float ff_y_Theta, float fEpsilon)
        {
            return (50f * (ff_u_Theta - ff_y_Theta) * fEpsilon) + (2f * ff_y_Theta) - ff_u_Theta; // Eq. (A.1a) Sigma_a

        }
        public float Eq_A1c____(float ff_u_Theta, float fEpsilon)
        {
            return ff_u_Theta * (1f - (20f * (fEpsilon - 0.15f))); // Eq. (A.1c) Sigma
        }
        public float Eq_A2a____(float ff_y_Theta)
        {
            return 1.25f * ff_y_Theta; // Eq. (A.2a) f_u_Theta
        }
        public float Eq_A2b____(float ff_y_Theta, float fTheta_a)
        {
            return ff_y_Theta * (2f - (0.0025f * fTheta_a)); // Eq. (A.2b) f_u_Theta
        }
        public float Eq_B2_____(float fl_z, float fl_f, float fAlpha, float fT_z)
        {
            return fl_z + fl_f + (fAlpha * fT_z); // Eq. (B.2) Sigma*Tm^4+Alpha*Tm
        }
        public float Eq_B3_____(float fPhi_f, float fEpsilon_f, float fa_z, float fSigma, float fT_f)
        {
            return fPhi_f * fEpsilon_f * (1f - fa_z) * fSigma * MathF.Pow4(fT_f); // Eq. (B.3) lf
        }
        public float Eq_B4_____(float fC_1, float fPhi_f_1, float fC_2, float fPhi_f_2, float fd_1, float fC_3, float fPhi_f_3, float fC_4, float fPhi_f_4, float fd_2)
        {
            if (!MathF.d_equal(((fC_1 + fC_2) * fd_1 + (fC_3 + fC_4) * fd_2), 0f))
                return ((((fC_1 * fPhi_f_1) + (fC_2 * fPhi_f_2)) * fd_1) + (((fC_3 * fPhi_f_3) + (fC_4 * fPhi_f_4)) * fd_2)) / (((fC_1 + fC_2) * fd_1) + ((fC_3 + fC_4) * fd_2)); // Eq. (B.4) Phi_f
            else
                return 0f;
        }
        public float Eq_B5_____(float fC_1, float fPhi_z_1, float fC_2, float fPhi_z_2, float fd_1, float fC_3, float fPhi_z_3, float fC_4, float fPhi_z_4, float fd_2)
        {
            if (!MathF.d_equal(((fC_1 + fC_2) * fd_1 + (fC_3 + fC_4) * fd_2), 0f))
                return ((((fC_1 * fPhi_z_1) + (fC_2 * fPhi_z_2)) * fd_1) + (((fC_3 * fPhi_z_3) + (fC_4 * fPhi_z_4)) * fd_2)) / (((fC_1 + fC_2) * fd_1) + ((fC_3 + fC_4) * fd_2)); // Eq. (B.5) Phi_z
            else
                return 0f;
        }
        public float Eq_B6_____(float fPhi_z, float fEpsilon_z, float fSigma, float fT_z)
        {
            return fPhi_z * fEpsilon_z * fSigma * MathF.Pow4(fT_z); // Eq. (B.6) l_z
        }
        public float Eq_B7_____(float fPhi_z_m, float fEpsilon_z_m, float fPhi_z_n, float fEpsilon_z_n, float fSigma, float fT_z)
        {
            return ((fPhi_z_m * fEpsilon_z_m) + (fPhi_z_n * fEpsilon_z_n)) * fSigma * MathF.Pow4(fT_z); // Eq. (B.7) l_z
        }
        //-------------------------------------------------------40----------------------------------------------------------------
        public float Eq_B8a____(float fh)
        {
            return 2f * fh / 3f; // Eq. (B.8a) Lambda
        }
        public float Eq_B10b___(float fw_i, float fs)
        {
            return fw_i + (0.4f * fs);
        }
        public float Eq_B11a___(float fh)
        {
            return fh / 2f; // Eq. (B.11a) l
        }
        public float Eq_B11c___(float fs, float fX, float fx)
        {
            return (fs * fX) / fx; // Eq. (B.11c) l
        }
        public float Eq_B12____(float fPhi_z, float fEpsilon_z, float fSigma, float fT_z)
        {
            return fPhi_z * fEpsilon_z * fSigma * MathF.Pow4(fT_z); // Eq. (B.12) l_z
        }
        public float Eq_B13____(float fPhi_z_m, float fEpsilon_z_m, float fPhi_z_n, float fEpsilon_z_n, float fSigma, float fT_z)
        {
            return ((fPhi_z_m * fEpsilon_z_m) + (fPhi_z_n * fEpsilon_z_n)) * fSigma * MathF.Pow4(fT_z); // Eq. (B.13) l_z
        }
        public float Eq_B16b___(float fw_i, float fs)
        {
            return fw_i + 0.4f * fs; // Eq. (B.16b) Lambda_i
        }
        public float Eq_B17a___(float fh)
        {
            return fh / 2f; // Eq. (B.17a) l
        }
        public float Eq_B17c___(float fs, float fX, float fx)
        {
            return (fs * fX) / fx; // Eq. (B.17c) l
        }
        public float Eq_B18____(float fl_z_1, float fl_z_2, float fd_1, float fl_z_3, float fl_z_4, float fd_2, float fC_1, float fC_2, float fC_3, float fC_4)
        {
            if (!MathF.d_equal(((fC_1 + fC_2) * fd_1) + ((fC_3 + fC_4) * fd_2), 0f))
                return (((fl_z_1 + fl_z_2) * fd_1) + ((fl_z_3 + fl_z_4) * fd_2)) / (((fC_1 + fC_2) * fd_1) + ((fC_3 + fC_4) * fd_2)); // Eq. (B.18) l_z
            else
                return 0f;
        }
        //-----------------------------------------------------------50----------------------------------------------------------------
        public float Eq_B20____(float fEpsilon_z_1, float fEpsilon_z_2, float fEpsilon_z_3)
        {
            return (fEpsilon_z_1 + fEpsilon_z_2 + fEpsilon_z_3) * 3f; // Eq. (B.20) a_z
        }
        public float Eq_B21____(float fl_z_1, float fl_z_2, float fd_1, float fl_z_3, float fl_z_4, float fd_2, float fC_1, float fC_2, float fC_3, float fC_4)
        {
            if (!MathF.d_equal(((fC_1 + fC_2) * fd_1) + ((fC_3 + fC_4) * fd_2), 0f))
                return (((fl_z_1 + fl_z_2) * fd_1) + ((fl_z_3 + fl_z_4) * fd_2)) / (((fC_1 + fC_2) * fd_1) + ((fC_3 + fC_4) * fd_2)); // Eq. (B.21) l_z
            else
                return 0f;
        }
        public float Eq_B22a___(float fC_1, float fEpsilon_z_1, float fSigma, float fT_o)
        {
            return fC_1 * fEpsilon_z_1 * fSigma * MathF.Pow4(fT_o); // Eq. (B.22a) l_z_1
        }
        public float Eq_B22b___(float fC_2, float fEpsilon_z_2, float fSigma, float fT_z_2)
        {
            return fC_2 * fEpsilon_z_2 * fSigma * MathF.Pow4(fT_z_2); // Eq. (B.22b) l_z_2
        }
        public float Eq_B22c___(float fC_3, float fEpsilon_z_3, float fSigma, float fT_z_1, float fT_z_2)
        {
            return fC_3 * fEpsilon_z_3 * fSigma * (MathF.Pow4(fT_z_1) + MathF.Pow4(fT_z_2)) / 2f; // Eq. (B.22c) l_z_3
        }
        public float Eq_B22d___(float fC_4, float fEpsilon_z_4, float fSigma, float fT_z_1, float fT_z_2)
        {
            return fC_4 * fEpsilon_z_4 * fSigma * (MathF.Pow4(fT_z_1) + MathF.Pow4(fT_z_2)) / 2f; // Eq. (B.22d) l_z_3
        }
        public float Eq_B23a___(float fC_1, float fEpsilon_z_1, float fSigma, float fT_o)
        {
            return fC_1 * fEpsilon_z_1 * fSigma * MathF.Pow4(fT_o); // Eq. (B.23a) l_z_1
        }
        public float Eq_B23c___(float fh_z, float fd_2, float fC_3, float fEpsilon_z_3, float fSigma, float fT_z_1, float fT_x)
        {
            return (fh_z / fd_2) * fC_3 * fEpsilon_z_3 * fSigma * (MathF.Pow4(fT_z_1) + MathF.Pow4(fT_x)) / 2f; // Eq. (B.23c) l_z_3
        }
        public float Eq_B23d___(float fh_z, float fd_2, float fC_4, float fEpsilon_z_4, float fSigma, float fT_z_1, float fT_x)
        {
            return (fh_z / fd_2) * fC_4 * fEpsilon_z_4 * fSigma * (MathF.Pow4(fT_z_1) + MathF.Pow4(fT_x)) / 2f; // Eq. (B.23d) l_z_4
        }
        public float Eq_B24a___(float fC_1, float fEpsilon_z_1, float fSigma, float fT_o)
        {
            return fC_1 * fEpsilon_z_1 * fSigma * MathF.Pow4(fT_o); // Eq. (B.24a) l_z_1
        }
        //---------------------------------------------------------60-----------------------------------------------------------------
        public float Eq_B24b___(float fC_2, float fEpsilon_z_2, float fSigma, float fT_z_2)
        {
            return fC_2 * fEpsilon_z_2 * fSigma * MathF.Pow4(fT_z_2); // Eq. (B.24b) l_z_2
        }
        public float Eq_B24c___(float fC_3, float fEpsilon_z_3, float fSigma, float fT_z_1, float fT_z_2)
        {
            return fC_3 * fEpsilon_z_3 * fSigma * (MathF.Pow4(fT_z_1) + MathF.Pow4(fT_z_2)) / 2; // Eq. (B.24c) l_z_3
        }
        public float Eq_B24d___(float fC_4, float fEpsilon_z_4, float fSigma, float fT_z_1, float fT_z_2)
        {
            return fC_4 * fEpsilon_z_4 * fSigma * (MathF.Pow4(fT_z_1) + MathF.Pow4(fT_z_2)) / 2f; // Eq. (B.24d) l_z_4
        }
        public float Eq_B25a___(float fC_1, float fEpsilon_z_1, float fSigma, float fT_o)
        {
            return fC_1 * fEpsilon_z_1 * fSigma * MathF.Pow4(fT_o); // Eq. (B.25a) l_z_1
        }
        public float Eq_B25b___(float fPhi_z_2, float fC_2, float fEpsilon_z_2, float fSigma, float fT_z_2)
        {
            return fPhi_z_2 * fC_2 * fEpsilon_z_2 * fSigma * MathF.Pow4(fT_z_2) / 2f; // Eq. (B.25b) l_z_2
        }
        public float Eq_B25c___(float fPhi_z_3, float fC_3, float fEpsilon_z_3, float fSigma, float fT_z_1, float fT_z_2)
        {
            return fPhi_z_3 * fC_3 * fEpsilon_z_3 * fSigma * (MathF.Pow4(fT_z_1) + MathF.Pow4(fT_z_2)) / 2f; // Eq. (B.25c) l_z_3
        }
        public float Eq_C3_____(float fTheta_a)
        {
            return 14.6f + (1.27f * (float)Math.Pow(10f, -2f) * fTheta_a); // Eq. (C.1) Lambda_a
        }
        public float Eq_D1_____(float fF_v_Rd, float fk_b_Theta, float fGamma_M2, float fGamma_M_fi)
        {
            if (!MathF.d_equal((fGamma_M_fi), 0f))
                return fF_v_Rd * fk_b_Theta * (fGamma_M2 / fGamma_M_fi); // Eq. (D.1) F_v_t_Rd
            else
                return 0f;
        }
        public float Eq_D2_____(float fF_b_Rd, float fk_b_Theta, float fGamma_M2, float fGamma_M_fi)
        {
            if (!MathF.d_equal(fGamma_M_fi, 0f))
                return fF_b_Rd * fk_b_Theta * (fGamma_M2 / fGamma_M_fi); // Eq. (D.2) F_b_t_Rd
            else
                return 0f;
        }
        //--------------------------------------------------70-----------------------------------------------------------
        public float Eq_D3_____(float fF_t_Rd, float fk_b_Theta, float fGamma_M2, float fGamma_M_fi)
        {
            if (!MathF.d_equal(fGamma_M_fi, 0f))
                return fF_t_Rd * fk_b_Theta * (fGamma_M2 / fGamma_M_fi); // Eq. (D.3) F_ten_t_Rd
            else
                return 0f;
        }
        public float Eq_D4_____(float fF_w_Rd, float fk_w_Theta, float fGamma_M2, float fGamma_M_fi)
        {
            if (!MathF.d_equal(fGamma_M_fi, 0f))
                return fF_w_Rd * fk_w_Theta * (fGamma_M2 / fGamma_M_fi); // Eq. (D.4) F_w_t_Rd
            else
                return 0f;
        }
        public float Eq_D5_____(float fTheta_o, float fh, float fD)
        {
            if (!MathF.d_equal(fD, 0f))
                return 0.88f * fTheta_o * (1f - (0.3f * (fh / fD))); // Eq. (D.5) Theta_
            else
                return 0f;
        }
        public float Eq_D6_____(float fTheta_o)
        {
            return 0.88f * fTheta_o; // Eq. (D.6) Theta_h
        }
        public float Eq_D7_____(float fTheta_o, float fh, float fD)
        {
            if (!MathF.d_equal(fD, 0f))
                return 0.88f * fTheta_o * (1f + (0.2f * (1f - 2f * (fh / fD)))); // Eq. (D.7) Theta_h
            else
                return 0f;
        }
    }
}
