using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using M_BASE;

namespace M_EC3
{
    class CL_Anx_B
    {
        M_BASE.CMetal oBaseMetal = new M_BASE.CMetal(); // Temporary - presunie sa do vyssej urovne

        // Annex B [informative] – Method 1: Interaction factors kij for interaction formula in 6.3.3(4)
        
        // Table B.1
        // Interaction factors k_ij
        // Table B.1: Interaction factors kij for members not susceptible to torsional deformations

        // Interaction factor fk_yy
        // Plastic cross-sectional properties class 1, class 2
        float Eq_k_yy_12_(float fN_Ed, float fLambda_rel_y, float fChi_y, float fN_Rk, float fC_my, float fGamma_M1)
        {
            if (fChi_y > 0f && fN_Rk > 0f)
                return Math.Min(fC_my * (1 + (fLambda_rel_y - 0.2f) * (fN_Ed / (fChi_y * fN_Rk / fGamma_M1))), fC_my * (1f + 0.8f * (fN_Ed / (fChi_y * fN_Rk / fGamma_M1))));
            else
                return 0f;
        }
        // Elastic cross-sectional properties class 3, class 4
        float Eq_k_yy_34_(float fN_Ed, float fLambda_rel_y, float fChi_y, float fN_Rk, float fC_my, float fGamma_M1)
        {
            if (fChi_y > 0f && fN_Rk > 0f)
                return Math.Min(fC_my * (1f + 0.6f * fLambda_rel_y * (fN_Ed / (fChi_y * fN_Rk / fGamma_M1))), fC_my * (1f + 0.6f * (fN_Ed / (fChi_y * fN_Rk / fGamma_M1))));
            else
                return 0f;
        }
        float Eq_k_yy____(int iClass, float fN_Ed, float fLambda_rel_y, float fChi_y, float fN_Rk, float fC_my, float fGamma_M1)
        {
            if (iClass < 3)
                return Eq_k_yy_12_(fN_Ed, fLambda_rel_y, fChi_y, fN_Rk, fC_my, fGamma_M1);
            else
                return Eq_k_yy_34_(fN_Ed, fLambda_rel_y, fChi_y, fN_Rk, fC_my, fGamma_M1);
        }

        // Interaction factor fk_yz
        // Plastic cross-sectional properties class 1, class 2
        float Eq_k_yz_12_(ECrScShType1 eCS_Type, float fN_Ed, float fLambda_rel_z, float fChi_z, float fN_Rk, float fC_mz, float fGamma_M1)
        {
            return 0.6f * Eq_k_zz_12_(eCS_Type, fN_Ed, fLambda_rel_z, fChi_z, fN_Rk, fC_mz, fGamma_M1);
        }
        // Elastic cross-sectional properties class 3, class 4
        float Eq_k_yz_34_(float fN_Ed, float fLambda_rel_z, float fChi_z, float fN_Rk, float fC_mz, float fGamma_M1)
        {
            return Eq_k_zz_34_(fN_Ed, fLambda_rel_z, fChi_z, fN_Rk, fC_mz, fGamma_M1);
        }
        float Eq_k_yz____(int iClass, ECrScShType1 eCS_Type, float fN_Ed, float fLambda_rel_z, float fChi_z, float fN_Rk, float fC_mz, float fGamma_M1)
        {
            if (iClass < 3)
                return Eq_k_yz_12_(eCS_Type, fN_Ed, fLambda_rel_z, fChi_z, fN_Rk, fC_mz, fGamma_M1);
            else
                return Eq_k_yz_34_(fN_Ed, fLambda_rel_z, fChi_z, fN_Rk, fC_mz, fGamma_M1);
        }

        // Interaction factor fk_zy
        // Plastic cross-sectional properties class 1, class 2
        float Eq_k_zy_12_B1(float fN_Ed, float fLambda_rel_y, float fChi_y, float fN_Rk, float fC_my, float fGamma_M1)
        {
            return 0.6f * Eq_k_yy_12_(fN_Ed, fLambda_rel_y, fChi_y, fN_Rk, fC_my, fGamma_M1);
        }
        // Elastic cross-sectional properties class 3, class 4
        float Eq_k_zy_34_B1(float fN_Ed, float fLambda_rel_y, float fChi_y, float fN_Rk, float fC_my, float fGamma_M1)
        {
            return 0.8f * Eq_k_yy_34_(fN_Ed, fLambda_rel_y, fChi_y, fN_Rk, fC_my, fGamma_M1);
        }
        float Eq_k_zy____B1(int iClass, float fN_Ed, float fLambda_rel_y, float fChi_y, float fN_Rk, float fC_my, float fGamma_M1)
        {
            if (iClass < 3)
                return Eq_k_zy_12_B1(fN_Ed, fLambda_rel_y, fChi_y, fN_Rk, fC_my, fGamma_M1);
            else
                return Eq_k_zy_34_B1(fN_Ed, fLambda_rel_y, fChi_y, fN_Rk, fC_my, fGamma_M1);
        }

        // Interaction factor fk_zz
        // Plastic cross-sectional properties class 1, class 2
        float Eq_k_zz_12_(ECrScShType1 eCS_Type, float fN_Ed, float fLambda_rel_z, float fChi_z, float fN_Rk, float fC_mz, float fGamma_M1)
        {
            // I-sections
            if (eCS_Type == ECrScShType1.eCrScType_I)
                return Math.Min(fC_mz * (1f + (2f * fLambda_rel_z - 0.6f) * (fN_Ed / (fChi_z * fN_Rk / fGamma_M1))), fC_mz * (1f + 1.4f * (fN_Ed / (fChi_z * fN_Rk / fGamma_M1))));
            else
                return Math.Min(fC_mz * (1f + (fLambda_rel_z - 0.2f) * (fN_Ed / (fChi_z * fN_Rk / fGamma_M1))), fC_mz * (1f + 0.8f * (fN_Ed / (fChi_z * fN_Rk / fGamma_M1))));
        }
        // Elastic cross-sectional properties class 3, class 4
        float Eq_k_zz_34_(float fN_Ed, float fLambda_rel_z, float fChi_z, float fN_Rk, float fC_mz, float fGamma_M1)
        {
            return Math.Min(fC_mz * (1f + 0.6f * (fN_Ed / (fChi_z * fN_Rk / fGamma_M1))), fC_mz * (1f + 0.6f * (fN_Ed / (fChi_z * fN_Rk / fGamma_M1))));
        }
        float Eq_k_zz____(int iClass, ECrScShType1 eCS_Type, float fN_Ed, float fLambda_rel_z, float fChi_z, float fN_Rk, float fC_mz, float fGamma_M1)
        {
            if (iClass < 3)
                return Eq_k_zz_12_(eCS_Type, fN_Ed, fLambda_rel_z, fChi_z, fN_Rk, fC_mz, fGamma_M1);
            else
                return Eq_k_zz_34_(fN_Ed, fLambda_rel_z, fChi_z, fN_Rk, fC_mz, fGamma_M1);
        }

        // Table B.2
        // Interaction factors k_ij
        // Table B.2: Interaction factors kij for members susceptible to torsional deformations

        // Interaction factor fk_zy
        // Plastic cross-sectional properties class 1, class 2
        float Eq_k_zy_12_B2(float fN_Ed, float fLambda_rel_z, float fChi_z, float fN_Rk, float fC_mLT, float fGamma_M1)
        {
            if (fLambda_rel_z >= 0.4f)
                return Math.Max(1f - (0.1f * fLambda_rel_z / (fC_mLT - 0.25f)) * (fN_Ed / (fChi_z * fN_Rk / fGamma_M1)), 1f - ((0.1f / (fC_mLT - 0.25f)) * (fN_Ed / (fChi_z * fN_Rk / fGamma_M1))));
            else
                return Math.Min(0.6f + fLambda_rel_z, 1f - ((0.1f * fLambda_rel_z / (fC_mLT - 0.25f)) * (fN_Ed / (fChi_z * fN_Rk / fGamma_M1))));
        }
        // Elastic cross-sectional properties class 3, class 4
        float Eq_k_zy_34_B2(float fN_Ed, float fLambda_rel_z, float fChi_z, float fN_Rk, float fC_mLT, float fGamma_M1)
        {
            return Math.Max(1f - ((0.05f * fLambda_rel_z / fC_mLT - 0.25f) * (fN_Ed / (fChi_z * fN_Rk / fGamma_M1))), 1f - ((0.05f / fC_mLT - 0.25f) * (fN_Ed / (fChi_z * fN_Rk / fGamma_M1))));
        }
        float Eq_k_zy____B2(int iClass, float fN_Ed, float fLambda_rel_z, float fChi_z, float fN_Rk, float fC_mLT, float fGamma_M1)
        {
            if (iClass < 3)
                return Eq_k_zy_12_B2(fN_Ed, fLambda_rel_z, fChi_z, fN_Rk, fC_mLT, fGamma_M1);
            else
                return Eq_k_zy_34_B2(fN_Ed, fLambda_rel_z, fChi_z, fN_Rk, fC_mLT, fGamma_M1);
        }

        // Table B.3
        // Factors of equvalent critical constant moment C_mi
        // Beam loading – 1 - uniform loading; 2 - concentrated load

        float Get_C_mi_D1(float fM_i_a, float fM_i_b)
        {
            // Moment diagram 1 (Table B.3, p. 79)
            return 0.6f + 0.4f * oBaseMetal.Get_Psi_i___(fM_i_a, fM_i_b);
        }
        float Get_C_mi_D2(ETrLoadType1 eLT, float fM_i_a, float fM_i_b, float fM_i_s)
        {
            // Moment diagram 2 (Table B.3, p. 79)

            float fAlpha_i_s, fM_i_h;
            float fPsi_i = oBaseMetal.Get_Psi_i___(fM_i_a, fM_i_b);

            if (Math.Abs(fM_i_a) > Math.Abs(fM_i_b))
                fM_i_h = fM_i_a;
            else
                fM_i_h = fM_i_b;

            // ?????????????????????????????????????
            // Calculate fAlpha_i_s
            if (Math.Abs(fM_i_s) <= Math.Abs(fM_i_h) && fM_i_h != 0f)
                fAlpha_i_s = fM_i_s / fM_i_h;
            else  // Exception
            {
                if (fM_i_s / fM_i_h < 0f)
                    fAlpha_i_s = -1f;
                else
                    fAlpha_i_s = 1f;
            }
            // ????????????????????????????????????


            if (eLT == ETrLoadType1.eTrLoadType_U)  //Uniform loading
            {
                if (fAlpha_i_s > 0f)
                    return (float)Math.Max(0.2f + 0.8f * fAlpha_i_s, 0.4f);
                else
                {
                    if (fPsi_i >= 0f)
                        return (float)Math.Max(0.1f - 0.8f * fAlpha_i_s, 0.4f);
                    else
                        return (float)Math.Max(0.1f * (0.1f - fPsi_i) - 0.8f * fAlpha_i_s, 0.4f);
                }
            }
            else /*if (eLT == ETrLoadType1.eTrLoadType_C || eLT != ETrLoadType1.eTrLoadType_U)*/  // Concentrated loading
            {
                if (fAlpha_i_s > 0)
                    return (float)Math.Max(0.2 + 0.8 * fAlpha_i_s, 0.4f);
                else
                {
                    if (fPsi_i >= 0f)
                        return (float)Math.Max(-0.8 * fAlpha_i_s, 0.4f);
                    else
                        return (float)Math.Max(0.2 * (-fPsi_i) - 0.8 * fAlpha_i_s, 0.4f);
                }
            }
        }
        float Get_C_mi_D3(ETrLoadType1 eLT, float fM_i_a, float fM_i_b, float fM_i_s)
        {
            // Moment diagram 3 (Table B.3, p. 79)

            float fAlpha_i_h, fM_i_h;

            float fPsi_i = oBaseMetal.Get_Psi_i___(fM_i_a, fM_i_b);

            if (Math.Abs(fM_i_a) > Math.Abs(fM_i_b))
                fM_i_h = fM_i_a;
            else
                fM_i_h = fM_i_b;

            // ?????????????????????????????????????
            // Calculate fAlpha_i_h
            if (Math.Abs(fM_i_h) <= Math.Abs(fM_i_s) && fM_i_s != 0f)
                fAlpha_i_h = fM_i_h / fM_i_s;
            else // Exception
            {
                if (fM_i_h / fM_i_s < 0f)
                    fAlpha_i_h = -1f;
                else
                    fAlpha_i_h = 1f;
            }
            // ????????????????????????????????????

            if (eLT == ETrLoadType1.eTrLoadType_U)  //Uniform loading
            {
                if (fAlpha_i_h > 0f)
                    return 0.95f + 0.05f * fAlpha_i_h;
                else
                {
                    if (fPsi_i >= 0f)
                        return 0.95f - 0.05f * fAlpha_i_h;
                    else
                        return 0.95f + 0.05f * fAlpha_i_h * (1f + 2f * fPsi_i);
                }
            }
            else /*if (eLT == ETrLoadType1.eTrLoadType_C || eLT != ETrLoadType1.eTrLoadType_U)*/  // Concentrated loading
            {
                if (fAlpha_i_h > 0f)
                    return 0.90f + 0.1f * fAlpha_i_h;
                else
                {
                    if (fPsi_i >= 0f)
                        return 0.90f - 0.1f * fAlpha_i_h;
                    else
                        return 0.90f + 0.1f * fAlpha_i_h * (1f + 2f * fPsi_i);
                }
            }
        }
    }
}
