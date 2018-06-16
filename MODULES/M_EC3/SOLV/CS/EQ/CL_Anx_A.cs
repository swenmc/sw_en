using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace M_EC3
{
    class CL_Anx_A
    {
        M_BASE.CMetal oBaseMetal = new M_BASE.CMetal(); // Temporary - presunie sa do vyssej urovne

        // Annex A [informative] – Method 1: Interaction factors kij for interaction formula in 6.3.3(4)
        
        // Table A.1
        // Interaction factors k_ij

        // Interaction factor fk_yy
        // Plastic cross-sectional properties class 1, class 2
        float Eq_k_yy_12_(float fN_Ed, float fN_cr_y, float fMu_y, float fC_my, float fC_mLT, float fC_yy)
        {
            if (fN_cr_y > 0f)
                return fC_my * fC_mLT * (fMu_y / (1f - (Math.Abs(fN_Ed) / fN_cr_y))) * 1f / fC_yy;
            else
                return 0f;
        }
        // Elastic cross-sectional properties class 3, class 4
        float Eq_k_yy_34_(float fN_Ed, float fN_cr_y, float fMu_y, float fC_my, float fC_mLT)
        {
            if (fN_cr_y > 0f)
                return fC_my * fC_mLT * (fMu_y / (1f - (Math.Abs(fN_Ed) / fN_cr_y)));
            else
                return 0f;
        }
        float Eq_k_yy____(int iClass, float fN_Ed, float fN_cr_y, float fMu_y, float fC_my, float fC_mLT, float fC_yy)
        {
            if (iClass < 3)
                return Eq_k_yy_12_(fN_Ed, fN_cr_y, fMu_y, fC_my, fC_mLT, fC_yy);
            else
                return Eq_k_yy_34_(fN_Ed, fN_cr_y, fMu_y, fC_my, fC_mLT);
        }

        // Interaction factor fk_yz
        // Plastic cross-sectional properties class 1, class 2
        float Eq_k_yz_12_(float fN_Ed, float fN_cr_z, float fMu_y, float fC_mz, float fC_yz, float fw_y, float fw_z)
        {
            if (fN_cr_z > 0f)
                return fC_mz * (fMu_y / (1f - (Math.Abs(fN_Ed) / fN_cr_z))) * 1f / fC_yz * 0.6f * (float)Math.Sqrt(fw_z / fw_y);
            else
                return 0f;
        }
        // Elastic cross-sectional properties class 3, class 4
        float Eq_k_yz_34_(float fN_Ed, float fN_cr_z, float fMu_y, float fC_mz)
        {
            if (fN_cr_z > 0f)
                return fC_mz * (fMu_y / (1f - (Math.Abs(fN_Ed) / fN_cr_z)));
            else
                return 0f;
        }
        float Eq_k_yz____(int iClass, float fN_Ed, float fN_cr_z, float fMu_y, float fC_mz, float fC_mLT, float fC_yz, float fw_y, float fw_z)
        {
            if (iClass < 3)
                return Eq_k_yz_12_(fN_Ed, fN_cr_z, fMu_y, fC_mz, fC_yz, fw_y, fw_z);
            else
                return Eq_k_yz_34_(fN_Ed, fN_cr_z, fMu_y, fC_mz);
        }

        // Interaction factor fk_zy
        // Plastic cross-sectional properties class 1, class 2
        float Eq_k_zy_12_(float fN_Ed, float fN_cr_y, float fMu_z, float fC_my, float fC_mLT, float fC_zy, float fw_y, float fw_z)
        {
            if (fN_cr_y > 0f)
                return fC_my * fC_mLT * (fMu_z / (1f - (Math.Abs(fN_Ed) / fN_cr_y))) * 1f / fC_zy * 0.6f * (float)Math.Sqrt(fw_y / fw_z);
            else
                return 0f;
        }
        // Elastic cross-sectional properties class 3, class 4
        float Eq_k_zy_34_(float fN_Ed, float fN_cr_y, float fMu_z, float fC_my, float fC_mLT)
        {
            if (fN_cr_y > 0f)
                return fC_my * fC_mLT * (fMu_z / (1f - (Math.Abs(fN_Ed) / fN_cr_y)));
            else
                return 0f;
        }
        float Eq_k_zy____(int iClass, float fN_Ed, float fN_cr_y, float fMu_z, float fC_my, float fC_mLT, float fC_zy, float fw_y, float fw_z)
        {
            if (iClass < 3)
                return Eq_k_zy_12_(fN_Ed, fN_cr_y, fMu_z, fC_my, fC_mLT, fC_zy,fw_y, fw_z);
            else
                return Eq_k_zy_34_(fN_Ed, fN_cr_y, fMu_z, fC_my, fC_mLT);
        }

        // Interaction factor fk_zz
        // Plastic cross-sectional properties class 1, class 2
        float Eq_k_zz_12_(float fN_Ed, float fN_cr_z, float fMu_z, float fC_mz, float fC_zz)
        {
            if (fN_cr_z > 0f)
                return fC_mz * (fMu_z / (1f - (Math.Abs(fN_Ed) / fN_cr_z))) * 1f / fC_zz;
            else
                return 0f;
        }
        // Elastic cross-sectional properties class 3, class 4
        float Eq_k_zz_34_(float fN_Ed, float fN_cr_z, float fMu_y, float fC_mz)
        {
            if (fN_cr_z > 0f)
                return fC_mz * (fMu_y / (1f - (Math.Abs(fN_Ed) / fN_cr_z)));
            else
                return 0f;
        }
        float Eq_k_zz____(int iClass, float fN_Ed, float fN_cr_z, float fMu_z, float fC_mz, float fC_zz)
        {
            if (iClass < 3)
                return Eq_k_zz_12_(fN_Ed, fN_cr_z, fMu_z, fC_mz, fC_zz);
            else
                return Eq_k_zz_34_(fN_Ed, fN_cr_z, fMu_z, fC_mz);
        }


        void Eq_k_ij_12__(float fN_Ed, float fN_cr_y, float fN_cr_z, float fMu_y, float fMu_z,
                        float fC_my, float fC_mz, float fC_mLT, float fC_yy, float fC_yz, float fC_zy, float fC_zz, float fw_y, float fw_z,
                        float fk_yy, float fk_yz, float fk_zy, float fk_zz)
        {
            fk_yy = Eq_k_yy_12_(fN_Ed, fN_cr_y, fMu_y, fC_my, fC_mLT, fC_yy);
            fk_yz = Eq_k_yz_12_(fN_Ed, fN_cr_z, fMu_y, fC_mz, fC_yz, fw_y, fw_z);
            fk_zy = Eq_k_zy_12_(fN_Ed, fN_cr_y, fMu_z, fC_my, fC_mLT, fC_zy, fw_y, fw_z);
            fk_zz = Eq_k_zz_12_(fN_Ed, fN_cr_z, fMu_z, fC_mz, fC_zz);
        }
        void Eq_k_ij_34__(float fN_Ed, float fN_cr_y, float fN_cr_z, float fMu_y, float fMu_z,
                        float fC_my, float fC_mz, float fC_mLT,
                        float fk_yy, float fk_yz, float fk_zy, float fk_zz)
        {
            fk_yy = Eq_k_yy_34_(fN_Ed, fN_cr_y, fMu_y, fC_my, fC_mLT);
            fk_yz = Eq_k_yz_34_(fN_Ed, fN_cr_z, fMu_y, fC_mz);
            fk_zy = Eq_k_zy_34_(fN_Ed, fN_cr_y, fMu_z, fC_my, fC_mLT);
            fk_zz = Eq_k_zz_34_(fN_Ed, fN_cr_z, fMu_z, fC_mz);
        }


        void Eq_k_ij_____(int iClass, float fN_Ed, float fN_cr_y, float fN_cr_z, float fMu_y, float fMu_z,
                        float fC_my, float fC_mz, float fC_mLT, float fC_yy, float fC_yz, float fC_zy, float fC_zz, float fw_y, float fw_z,
                        float fk_yy, float fk_yz, float fk_zy, float fk_zz)
        {
            if (iClass < 3)
            {
                fk_yy = Eq_k_yy_12_(fN_Ed, fN_cr_y, fMu_y, fC_my, fC_mLT, fC_yy);
                fk_yz = Eq_k_yz_12_(fN_Ed, fN_cr_z, fMu_y, fC_mz, fC_yz, fw_y, fw_z);
                fk_zy = Eq_k_zy_12_(fN_Ed, fN_cr_y, fMu_z, fC_my, fC_mLT, fC_zy, fw_y, fw_z);
                fk_zz = Eq_k_zz_12_(fN_Ed, fN_cr_z, fMu_z, fC_mz, fC_zz);
            }
            else
            {
                fk_yy = Eq_k_yy_34_(fN_Ed, fN_cr_y, fMu_y, fC_my, fC_mLT);
                fk_yz = Eq_k_yz_34_(fN_Ed, fN_cr_z, fMu_y, fC_mz);
                fk_zy = Eq_k_zy_34_(fN_Ed, fN_cr_y, fMu_z, fC_my, fC_mLT);
                fk_zz = Eq_k_zz_34_(fN_Ed, fN_cr_z, fMu_z, fC_mz);
            }
        }


        // Auxiliary equations

        float Eq_Mu_y___(float fN_Ed, float fN_cr_y, float fChi_y)
        {
            if (fChi_y > 0 && fN_cr_y > 0f)
                return (1f - Math.Abs(fN_Ed) / fN_cr_y) / (1f - fChi_y * Math.Abs(fN_Ed) / fN_cr_y);
            else
                return 0;
        }
        float Eq_Mu_z___(float fN_Ed, float fN_cr_z, float fChi_z)
        {
            if (fChi_z > 0 && fN_cr_z > 0f)
                return (1f - Math.Abs(fN_Ed) / fN_cr_z) / (1f - fChi_z * Math.Abs(fN_Ed) / fN_cr_z);
            else
                return 0;
        }
        float Eq_w_y____(float fW_pl_y, float fW_el_y)
        {
            return Math.Min(fW_pl_y / fW_el_y, 1.5f); 
        }
        float Eq_w_z____(float fW_pl_z, float fW_el_z)
        {
            return Math.Min(fW_pl_z / fW_el_z, 1.5f);
        }
        float Eq_n_pl____(float fN_Ed, float fN_Rk, float fGamma_M1)
        {
            return Math.Abs(fN_Ed) / (fN_Rk / fGamma_M1);
        }


        float Eq_a_LT____(float fI_T, float fI_y)
        {
            return Math.Max(1f - (fI_T / fI_y),0f);
        }
        float Eq_b_LT____(float fa_LT, float fLambda_rel_0, float fM_y_Ed, float fChi_LT, float fM_pl_y_Rd, float fM_z_Ed, float fM_pl_z_Rd)
        {
            return 0.5f * fa_LT * (float)Math.Pow(fLambda_rel_0, 2f) * (Math.Abs(fM_y_Ed) / (fChi_LT * fM_pl_y_Rd)) * (Math.Abs(fM_z_Ed) / fM_pl_z_Rd);
        }
        float Eq_c_LT____(float fa_LT, float fLambda_rel_0, float fLambda_rel_z, float fM_y_Ed, float fC_my, float fChi_LT, float fM_pl_y_Rd)
        {
            return 10f * fa_LT * ((float)Math.Pow(fLambda_rel_0, 2f) / (5f + (float)Math.Pow(fLambda_rel_z, 4f))) * (Math.Abs(fM_y_Ed) / (fC_my * fChi_LT * fM_pl_y_Rd));
        }
        float Eq_d_LT____(float fa_LT, float fLambda_rel_0, float fLambda_rel_z, float fM_y_Ed, float fC_my, float fChi_LT, float fM_pl_y_Rd, float fM_z_Ed, float fC_mz, float fM_pl_z_Rd)
        {
            return 2f * fa_LT * ((float)Math.Pow(fLambda_rel_0, 2f) / (0.1f + (float)Math.Pow(fLambda_rel_z, 4f))) * (Math.Abs(fM_y_Ed) / (fC_my * fChi_LT * fM_pl_y_Rd)) * (Math.Abs(fM_z_Ed) / (fC_mz * fM_pl_z_Rd));
        }
        float Eq_e_LT____(float fa_LT, float fLambda_rel_0, float fLambda_rel_z, float fM_y_Ed, float fC_my, float fChi_LT, float fM_pl_y_Rd)
        {
            return 1.7f * fa_LT * ((float)Math.Pow(fLambda_rel_0, 2f) / (0.1f + (float)Math.Pow(fLambda_rel_z, 4f))) * (Math.Abs(fM_y_Ed) / (fC_my * fChi_LT * fM_pl_y_Rd));
        }


        float Eq_C_yy____(float fw_y, float fC_my, float fLambda_rel_max, float fn_pl, float fb_LT, float fW_el_y, float fW_pl_y)
        {
            return Math.Max(1f + (fw_y - 1) * ((2f - 1.6f / fw_y * (float)Math.Pow(fC_my, 2f) * fLambda_rel_max - 1.6f / fw_y * (float)Math.Pow(fC_my, 2f) * (float)Math.Pow(fLambda_rel_max, 2f)) * fn_pl - fb_LT), fW_el_y / fW_pl_y);
        }
        float Eq_C_yz____(float fw_z, float fw_y, float fC_mz, float fLambda_rel_max, float fn_pl, float fc_LT, float fW_el_z, float fW_pl_z)
        {
            return Math.Max(1f + (fw_z - 1f) * ((2f - (14f / (float)Math.Pow(fw_z, 5f) * (float)Math.Pow(fC_mz, 2f) * (float)Math.Pow(fLambda_rel_max, 2f))) * fn_pl - fc_LT), 0.6f * (float)Math.Sqrt(fw_z / fw_y) * fW_el_z / fW_pl_z);
        }
        float Eq_C_zy____(float fw_y, float fw_z, float fC_my, float fLambda_rel_max, float fn_pl, float fd_LT, float fW_el_y, float fW_pl_y)
        {
            return Math.Max(1f + (fw_y - 1f) * ((2f - (14f / (float)Math.Pow(fw_y, 5f) * (float)Math.Pow(fC_my, 2f) * (float)Math.Pow(fLambda_rel_max, 2f))) * fn_pl - fd_LT), 0.6f * (float)Math.Sqrt(fw_y / fw_z) * fW_el_y / fW_pl_y);
        }
        float Eq_C_zz____(float fw_z, float fC_mz, float fLambda_rel_max, float fn_pl, float fe_LT, float fW_el_z, float fW_pl_z)
        {
            return Math.Max(1f + (fw_z - 1) * ((2 - 1.6f / fw_z * (float)Math.Pow(fC_mz, 2f) * fLambda_rel_max - 1.6f / fw_z * (float)Math.Pow(fC_mz, 2f) * (float)Math.Pow(fLambda_rel_max, 2)) * fn_pl - fe_LT), fW_el_z / fW_pl_z);
        }


        float Eq_Lambda_rel_max(float fLambda_rel_y, float fLambda_rel_z)
        {
            return Math.Max(fLambda_rel_y, fLambda_rel_z);
        }
        void Get_Cmi____(float fC_my_0, float fC_mz_0, float fC_m_LT, float fLambda_rel_0, float fC_1, float fN_Ed, float fN_cr_y, float fN_cr_z, float fN_cr_TF, float fa_LT, float fEps_y, float fC_my, float fC_mz, float fC_mLT)
        {
            if (fLambda_rel_0 <= 0.2f * (float)Math.Sqrt(fC_1) * (float)Math.Pow((1f - (fN_Ed / fN_cr_z)) * (1f - (fN_Ed / fN_cr_TF)), 0.25f))
            {
                fC_my = fC_my_0;
                fC_mz = fC_mz_0;
                fC_mLT = 1f;
            }
            else
            {
                fC_my = fC_my_0 + (1f - fC_my_0) * (((float)Math.Sqrt(fEps_y) * fa_LT) / (1f + ((float)Math.Sqrt(fEps_y) * fa_LT)));
                fC_mz = fC_mz_0;
                fC_mLT = Math.Max((float)Math.Pow(fC_my, 2f) * (fa_LT / ((float)Math.Sqrt((1f - (fN_Ed / fN_cr_z)) * (1f - (fN_Ed / fN_cr_TF))))), 1f);

            }
        }

        // Eps - overloaded functions
        float Eq_Eps_y_13(float fN_Ed, float fM_y_Ed, float fA, float fW_el_y)
        {
            if (fN_Ed != 0f)
                return (fM_y_Ed / fN_Ed) * (fA / fW_el_y);
            else
                return 0f;
        }
        float Eq_Eps_y_04(float fN_Ed, float fM_y_Ed, float fA_eff, float fW_eff_y)
        {
            if (fN_Ed != 0f)
                return (fM_y_Ed / fN_Ed) * (fA_eff / fW_eff_y);
            else
                return 0f;
        }
        float Eq_Eps_y___(int iClass, float fN_Ed, float fM_y_Ed, float fA, float fW_el_y, float fA_eff, float fW_eff_y)
        {
            if (fN_Ed != 0f)
            {
                if (iClass < 4)
                    return Eq_Eps_y_13(fN_Ed, fM_y_Ed, fA, fW_el_y);
                else
                    return Eq_Eps_y_04(fN_Ed, fM_y_Ed, fA_eff, fW_eff_y);
            }
            else
                return 0f;
        }
        float Eq_Eps_y___(int iClass, float fN_Ed, float fM_y_Ed, float fA_i, float fW_i_y)
        {
            if (fN_Ed != 0f)
            {
                if (iClass < 4)
                    return Eq_Eps_y_13(fN_Ed, fM_y_Ed, fA_i, fW_i_y); // Same body
                else
                    return Eq_Eps_y_04(fN_Ed, fM_y_Ed, fA_i, fW_i_y); // Same body
            }
            else
                return 0f;
        }

        // Table A.2
        // Factors of equvalent critical constant moment

        float Get_C_mi_0_D1(float fPsi_i, float fN_Ed, float fN_cr_i)
        {
            // Table A.2: Equivalent uniform moment factors Cmi,0 (p.78)
            // Moment diagram 1

            return 0.79f + 0.21f * fPsi_i + 0.36f * (fPsi_i - 0.33f) * (fN_Ed / fN_cr_i);
        }
        float Get_C_mi_0_D1(float fM_1a, float fM_1b, float fN_Ed, float fN_cr_i)
        {
            // Table A.2: Equivalent uniform moment factors Cmi,0 (p.78)
            // Moment diagram 1

            return Get_C_mi_0_D1(oBaseMetal.Get_Psi_i___(fM_1a, fM_1b), fN_Ed, fN_cr_i);
        }
        float Get_C_mi_0_D2(float fE, float fI_i, float fDeflection_i_x, float fL, float fM_i_Ed_x, float fN_Ed, float fN_cr_i)
        {
            // Moment diagram 2

            if (fN_cr_i > 0f && Math.Abs(fM_i_Ed_x) > 0f)
                return 1f + ((((float)Math.PI * (float)Math.PI * fE * fI_i * Math.Abs(fDeflection_i_x)) / (fL * fL * Math.Abs(fM_i_Ed_x)) - 1f) * fN_Ed / fN_cr_i);
            else
                return 0;
        }
        float Get_C_mi_0_D3(float fN_Ed, float fN_cr_i)
        {
            // Moment diagram 3

            if (fN_cr_i > 0f)
                return 1f - (0.18f * fN_Ed / fN_cr_i);
            else
                return 0f;
        }
        float Get_C_mi_0_D4(float fN_Ed, float fN_cr_i)
        {
            // Moment diagram 4

            if (fN_cr_i > 0f)
                return 1f + (0.03f * fN_Ed / fN_cr_i);
            else
                return 0f;
        }
    }
}
