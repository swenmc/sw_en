using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace M_EC3
{
    public class CL_62
    {
        public float Eq_61______(float fSigma_x_Ed, float fSigma_z_Ed, float fTau_Ed, float ff_y, float fGamma_M0)
        {
            if (fGamma_M0 > 0f && ff_y > 0f)
            {
                float ff_y_d = ff_y / fGamma_M0;
                return ((float)Math.Pow(fSigma_x_Ed / ff_y_d, 2f) + (float)Math.Pow(fSigma_z_Ed / ff_y_d, 2f) - (fSigma_x_Ed / ff_y_d) * (fSigma_z_Ed / ff_y_d) + 3f * (float)Math.Pow(fTau_Ed / ff_y_d, 2f)) / 1f; // Eq. (6.1) Design Ratio
            }
            else
                return 0f;
        }
        public float Eq_62______(float fN_Ed, float fM_y_Ed, float fM_z_Ed, float fN_Rd, float fM_y_Rd, float fM_z_Rd)
        {
            if (fN_Rd > 0f && fM_y_Rd > 0f && fM_z_Rd > 0f)
                return (fN_Ed / fN_Rd + fM_y_Ed / fM_y_Rd + fM_z_Ed / fM_z_Rd) / 1f; // Eq. (6.2) Design Ratio
            else
                return 0f;
        }
        public float Eq_63______(float ft, float fd_0)  // Temporary
        {
            return ft; // Eq. (6.3) fA_holes
        }
        public float Eq_64______(float fN_Ed, float fe_N)
        {
            return fN_Ed * fe_N; // Eq. (6.4) fDelta_M_Ed
        }
        public float Eq_65______(float fN_Ed, float fN_t_Rd)
        {
            if (fN_t_Rd > 0f)
                return (fN_Ed / fN_t_Rd) / 1f; // Eq. (6.5) Design Ratio
            else
                return 0f;
        }
        public float Eq_66______(float fA, float ff_y, float fGamma_M0)
        {
            if (fGamma_M0 > 0f)
                return fA * ff_y / fGamma_M0; // Eq. (6.6) fN_pl_Rd
            else
                return 0f;
        }
        public float Eq_67______(float fA_net, float ff_u, float fGamma_M2)
        {
            if (fGamma_M2 > 0f)
                return 0.9f * fA_net * ff_u / fGamma_M2; // Eq. (6.7) fN_u_Rd
            else
                return 0f;
        }
        public float Eq_68______(float fA_net, float ff_y, float fGamma_M0)
        {
            if (fGamma_M0 > 0f)
                return fA_net * ff_y / fGamma_M0; // Eq. (6.6) fN_pl_Rd
            else
                return 0f;
        }
        public float Eq_69______(float fN_Ed, float fN_c_Rd)
        {
            if (fN_c_Rd > 0f)
                return (fN_Ed / fN_c_Rd) / 1f; // Eq. (6.9) Design Ratio
            else
                return 0f;
        }
        public float Eq_610_____(float fA, float ff_y, float fGamma_M0)
        {
            if (fGamma_M0 > 0f)
                return fA * ff_y / fGamma_M0; // Eq. (6.10) fN_c_Rd
            else
                return 0f;
        }
        public float Eq_611_____(float fA_eff, float ff_y, float fGamma_M0)
        {
            if (fGamma_M0 > 0f)
                return fA_eff * ff_y / fGamma_M0; // Eq. (6.11) fN_c_Rd
            else
                return 0f;
        }
        public float Eq_612_____(float fM_Ed, float fM_c_Rd)
        {
            if (fM_c_Rd > 0f)
                return (fM_Ed / fM_c_Rd) / 1f; // Eq. (6.12) Design Ratio
            else
                return 0f;
        }
        public float Eq_613_____(float fW_pl, float ff_y, float fGamma_M0)
        {
            if (fGamma_M0 > 0f)
                return fW_pl * ff_y / fGamma_M0; // Eq. (6.13) fM_pl_Rd and fM_c_Rd
            else
                return 0f;
        }
        public float Eq_614_____(float fW_el, float ff_y, float fGamma_M0)
        {
            if (fGamma_M0 > 0f)
                return fW_el * ff_y / fGamma_M0; // Eq. (6.14) fM_el_Rd and fM_c_Rd
            else
                return 0f;
        }
        public float Eq_615_____(float fW_eff, float ff_y, float fGamma_M0)
        {
            if (fGamma_M0 > 0f)
                return fW_eff * ff_y / fGamma_M0; // Eq. (6.14) fM_eff_Rd and fM_c_Rd
            else
                return 0f;
        }
        public float Eq_M_c_Rd__(int iClass, float fW_pl, float fW_el, float fW_eff, float ff_y, float fGamma_M0)
        {
            if (iClass <= 2)
                return Eq_613_____(fW_pl, ff_y, fGamma_M0);
            else if (iClass == 3)
                return Eq_614_____(fW_el, ff_y, fGamma_M0);
            else /*(iClass == 4)*/
                return Eq_615_____(fW_eff, ff_y, fGamma_M0);
        }
        public float Eq_617_____(float fV_Ed, float fV_c_Rd)
        {
            if (fV_c_Rd > 0f)
                return (fV_Ed / fV_c_Rd) / 1f; // Eq. (6.17) Design Ratio
            else
                return 0f;
        }
        public float Eq_618_____(float fA_V, float ff_y, float fGamma_M0)
        {
            if (fGamma_M0 > 0f)
                return fA_V * (ff_y / (float)Math.Sqrt(3f)) / fGamma_M0; // Eq. (6.18) fV_pl_Rd and fV_c_Rd
            else
                return 0f;
        }
        public float Eq_619_____(float fTau_Ed, float ff_y, float fGamma_M0)
        {
            if (fGamma_M0 > 0f && ff_y > 0f)
                return (fTau_Ed / (ff_y / (float)Math.Sqrt(3f)) / fGamma_M0) / 1f; // Eq. (6.19) Design Ratio
            else
                return 0f;
        }
        public float Eq_620_____(float fV_Ed, float fS, float fI, float ft)
        {
            if (fI > 0f && ft > 0f)
                return (fV_Ed * fS) / (fI * ft); // Eq. (6.20) fTau_Ed
            else
                return 0f;
        }
        public float Eq_621_____(float fV_Ed, float fA_w)
        {
            if (fA_w > 0f)
                return fV_Ed / fA_w; // Eq. (6.21) fTau_Ed - web I and H cross-section
            else
                return 0f;
        }
        public float Eq_622_____(float fh_w, float ft_w, float fEps, float fEta)
        {
            if (ft_w > 0f && fEta > 0f)
                return (fh_w / ft_w) / (72f * fEps / fEta); // Eq. (6.22) Slenderness ratio of web
            else
                return 0f;
        }
        public float Eq_623_____(float fT_Ed, float fT_Rd)
        {
            if (fT_Rd > 0f)
                return (fT_Ed / fT_Rd) / 1f; // Eq. (6.23) Design Ratio
            else
                return 0f;
        }
        public float Eq_624_____(float fT_t_Ed, float fT_w_Ed)
        {
            return fT_t_Ed + fT_w_Ed; // Eq. (6.24) fT_Ed
        }
        public float Eq_625_____(float fV_Ed, float fV_pl_T_Rd)
        {
            if (fV_pl_T_Rd > 0f)
                return (fV_Ed / fV_pl_T_Rd) / 1f; // Eq. (6.25) Design Ratio
            else
                return 0f;
        }
        public float Eq_626_____(float fTau_t_Ed, float ff_y, float fGamma_M0, float fV_pl_Rd)
        {
            if (ff_y > 0f && fGamma_M0 > 0f && fV_pl_Rd > 0f)
                return ((float)Math.Sqrt(1f - (fTau_t_Ed / 1.25f * (ff_y / (float)Math.Sqrt(3f)) * fGamma_M0))) * fV_pl_Rd; // Eq. (6.26) fV_pl_T_Rd // I and H cross-section
            else
                return 0f;
        }
        public float Eq_627_____(float fTau_t_Ed, float fTau_w_Ed, float ff_y, float fGamma_M0, float fV_pl_Rd)
        {
            if (ff_y > 0f && fGamma_M0 > 0f && fV_pl_Rd > 0f)
                return (((float)Math.Sqrt(1f - (fTau_t_Ed / 1.25f * (ff_y / (float)Math.Sqrt(3f)) * fGamma_M0))) - fTau_w_Ed / (ff_y / (float)Math.Sqrt(3f)) * fGamma_M0) * fV_pl_Rd; // Eq. (6.27) fV_pl_T_Rd // C and U cross-section
            else
                return 0f;
        }
        public float Eq_628_____(float fTau_t_Ed, float ff_y, float fGamma_M0, float fV_pl_Rd)
        {
            if (ff_y > 0f && fGamma_M0 > 0f && fV_pl_Rd > 0f)
                return (1f - (fTau_t_Ed / ((ff_y / (float)Math.Sqrt(3f)) * fGamma_M0))) * fV_pl_Rd; // Eq. (6.28) fV_pl_T_Rd // hollow cross-section
            else
                return 0f;
        }
        public float Eq_629_____(float fRho, float ff_y)
        {
            return (1f - fRho) * ff_y; // Eq. (6.29) ff_y_V - reduced value of yield strength
        }
        public float Eq_Rho_628V(float fV_Ed, float fV_pl_Rd)
        {
            if(fV_pl_Rd > 0f)
            return (float)Math.Pow(((2f * fV_Ed / fV_pl_Rd) - 1f), 2f); // fRho - shear reduction factor of yield strength
            else
                return 0f;
        }
        public float Eq_Rho_628T(float fV_Ed, float fV_pl_T_Rd)
        {
            if (fV_pl_T_Rd > 0f)
                return (float)Math.Pow(((2f * fV_Ed / fV_pl_T_Rd) - 1f), 2f); // fRho - shear reduction factor of yield strength
            else
                return 0f;
        }
        public float Eq_630_____(float fW_pl_y, float fRho, float fA_w, float ft_w, float ff_y, float fGamma_M0, float fM_y_c_Rd)
        {
            if (fW_pl_y > 0f && ft_w > 0f && fGamma_M0 > 0f && ff_y > 0f)
                return (float)Math.Min(fW_pl_y - (((fRho * (float)Math.Pow(fA_w, 2f)) / (4f * ft_w))) * ff_y / fGamma_M0, fM_y_c_Rd); // Eq. (6.30) fM_y_V_Rd - symmetrical I cross-section with identical flanges
            else
                return 0f;
        }
        public float Eq_631_____(float fM_Ed, float fM_N_Rd)
        {
            if (fM_N_Rd > 0f)
                return (fM_Ed / fM_N_Rd) / 1f; // Eq. (6.31) Design Ratio
            else
                return 0f;
        }
        public float Eq_632_____(float fM_pl_Rd, float fN_Ed, float fN_pl_Rd)
        {
            if (fN_pl_Rd > 0f)
                return fM_pl_Rd * (1f - (float)Math.Pow((fN_Ed / fN_pl_Rd), 2f)); // Eq. (6.32) fM_N_Rd
            else
                return 0f;
        }
        public float Eq_633_____(float fN_Ed, float fN_pl_Rd)
        {
            if (fN_pl_Rd > 0f)
                return fN_Ed / (0.25f * fN_pl_Rd); // Eq. (6.33) Limit - symmetrical I and H cross-section
            else
                return 0f;
        }
        public float Eq_634_____(float fN_Ed, float fh_w, float ft_w, float ff_y, float fGamma_M0)
        {
            if (fGamma_M0 > 0f)
                return fN_Ed / (0.5f * fh_w * ft_w * ff_y / fGamma_M0); // Eq. (6.34) Limit - symmetrical I and H cross-section
            else
                return 0f;
        }
        public float Eq_635_____(float fN_Ed, float fh_w, float ft_w, float ff_y, float fGamma_M0)
        {
            if (fGamma_M0 > 0f)
                return fN_Ed / (fh_w * ft_w * ff_y / fGamma_M0); // Eq. (6.35) Limit - symmetrical I and H cross-section
            else
                return 0f;
        }
        public float Eq_636_____(float fM_pl_y, float fn, float fa, float fM_pl_y_Rd)
        {
            if (fM_pl_y_Rd > 0f && fa > 0f)
                return (float)Math.Min(fM_pl_y_Rd * (1f - fn) / (1f - 0.5f * fa), fM_pl_y_Rd); // Eq. (6.36) fM_N_y_Rd - symmetrical I cross-section with identical flanges
            else
                return 0f;
        }
        public float Eq_637_____(float fM_pl_z_Rd)
        {
            return fM_pl_z_Rd; // Eq. (6.37) fM_N_z_Rd - symmetrical I cross-section with identical flanges
        }
        public float Eq_638_____(float fM_pl_z_Rd, float fn, float fa)
        {
            return fM_pl_z_Rd * (1f - (float)Math.Pow(((fn - fa) / (1f - fa)), 2f)); ; // Eq. (6.38) fM_N_z_Rd - symmetrical I cross-section with identical flanges
        }
        public float Eq_639_____(float fM_pl_y, float fn, float fa_w, float fM_pl_y_Rd)
        {
            if (fM_pl_y_Rd > 0f && fa_w > 0f)
                return (float)Math.Min(fM_pl_y_Rd * (1f - fn) / (1f - 0.5f * fa_w), fM_pl_y_Rd); // Eq. (6.39) fM_N_y_Rd - hollow rectangular and square sections
            else
                return 0f;
        }
        public float Eq_640_____(float fM_pl_z, float fn, float fa_f, float fM_pl_z_Rd)
        {
            if (fM_pl_z_Rd > 0f && fa_f > 0f)
                return (float)Math.Min(fM_pl_z_Rd * (1f - fn) / (1f - 0.5f * fa_f), fM_pl_z_Rd); // Eq. (6.40) fM_N_z_Rd - hollow rectangular and square sections
            else
                return 0f;
        }
        public float Eq_641_____(float fM_y_Ed, float fM_N_y_Rd, float fM_z_Ed, float fM_N_z_Rd, float _z, float fAlpha, float fBeta)
        {
            if (fM_N_y_Rd > 0f && fM_N_z_Rd > 0f)
                return ((float)Math.Pow(fM_y_Ed / fM_N_y_Rd, fAlpha) + (float)Math.Pow(fM_z_Ed / fM_N_z_Rd, fBeta))/ 1f; // Eq. (6.41) Design Ratio
            else
                return 0f;
        }
        public float Eq_642_____(float fSig_x_Ed, float ff_y, float fGamma_M0)
        {
            return fSig_x_Ed / (ff_y / fGamma_M0); // Eq. (6.42) Design Ratio
        }
        public float Eq_643_____(float fSig_x_Ed, float ff_y, float fGamma_M0)
        {
            return fSig_x_Ed / (ff_y / fGamma_M0); // Eq. (6.43) Design Ratio
        }
        public float Eq_644_____(float fN_Ed, float fM_y_Ed, float fM_z_Ed, float fA_eff, float fW_eff_y_min, float fW_eff_z_min, float fe_Ny, float fe_Nz, float ff_y, float fGamma_M0)
        {
            if (fA_eff > 0f && fW_eff_y_min > 0f && fW_eff_z_min > 0f && ff_y > 0f && fGamma_M0 > 0f)
                return (fN_Ed / (fA_eff * ff_y / fGamma_M0)) + ((fM_y_Ed + fN_Ed * fe_Ny) / (fW_eff_y_min * ff_y / fGamma_M0)) + ((fM_z_Ed + fN_Ed * fe_Nz) / (fW_eff_z_min * ff_y / fGamma_M0)); // Eq. (6.44) Design Ratio
            else
                return 0f;
        }
        public float Eq_645_____(float fRho, float ff_y)
        {
            return (1f - fRho) / ff_y; // Eq. (6.45) ff_y_V - reduced yield strength
        }
    }
}
