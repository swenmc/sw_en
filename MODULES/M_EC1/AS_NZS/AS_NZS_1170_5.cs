using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MATH;
using BaseClasses;

namespace M_EC1.AS_NZS
{
    public static class AS_NZS_1170_5
    {
        public static float Eq_31_1____(float fC_h_T, float fZ, float fR, float fN_TD)
        {
            return fC_h_T * fZ * fR * fN_TD; // Eq. (3.1(1)) // fC(T)
        }
        public static float Eq_41_1____(float fSumWiDi2, float fg, float fSumFidi)
        {
            return 2f * MathF.fPI * MathF.Sqrt(fSumWiDi2 / (fg * fSumFidi)); // Eq. (4.1(1)) // fT_1
        }
        public static float Eq_41_1____(float fSumWiDi2, float fSumFidi)
        {
            return 2f * MathF.fPI * MathF.Sqrt(fSumWiDi2 / (9.81f * fSumFidi)); // Eq. (4.1(1)) // fT_1
        }
        public static float Eq_42_1____(float fG_i, float fSum_Psi_E_Qi)
        {
            return fG_i + fSum_Psi_E_Qi; // Eq. (4.2(1)) // fW_i
        }
        public static float Eq_52_1____(float fC_T1, float fS_p, float fk_Nu)
        {
            return fC_T1 * fS_p / fk_Nu; // Eq. (5.2(1)) // fCd(T1)
        }
        public static float Eq_52_2____(float fZ, float fR_u)
        {
            if (fZ * fR_u > 0.7f) // 3.1.1 Elastic site spectra
                fR_u = 0.7f / fZ; // Exception !!! // Set R_u value at limit

            return MathF.Max(((fZ / 20f) + 0.02f) * fR_u, 0.03f * fR_u); // Eq. (5.2(2)) // fCd(T1) minimum value
        }
        public static float Eq_5221_ULS(float fC_T1, float fS_p, float fk_Nu, float fZ, float fR_u)
        {
            return MathF.Max(Eq_52_1____(fC_T1, fS_p, fk_Nu), Eq_52_2____(fZ, fR_u)); // fCd(T1) - maximum from Eq. (5.2(1)) and Eq. (5.2(2)) - for ULS
        }
        public static float Eq_5221_ULS(float fC_T1, float fS_p, float fZ, float fR_u, float fT_1, float fNu, ESiteSubSoilClass eSiteSoilClass, out float fk_Nu)
        {
            fk_Nu = Get_k_nu__5211_ULS(fT_1, fNu, eSiteSoilClass);
            return MathF.Max(Eq_52_1____(fC_T1, fS_p, fk_Nu), Eq_52_2____(fZ, fR_u)); // fCd(T1) - maximum from Eq. (5.2(1)) and Eq. (5.2(2)) - for ULS
        }
        public static float Get_k_nu__5211_ULS(float fT_1, float fNu, ESiteSubSoilClass eSiteSoilClass)
        {
            fT_1 = MathF.Max(fT_1, 0.4f); // 5.2.1.1 Ultimate limit state - Note in the last row - page 32 (T_1 > 0.4s)

            switch (eSiteSoilClass)
            {
                case ESiteSubSoilClass.eE:
                    {
                        if (fT_1 > 1f || fNu < 1.5f)
                            return fNu;
                        else
                            return (fNu - 1.5f) * fT_1 + 1.5f;
                    }
                default: // Soil classes: A, B, C, D
                    {
                        if (fT_1 >= 0.7f)
                            return fNu;
                        else
                            return (((fNu - 1f) * fT_1) / 0.7f) + 1f;
                    }
            }
        }
        public static float Get_C_D_T1_5212_SLS(float fC_T1, float fT_1, float fNu, ESiteSubSoilClass eSiteSoilClass, out float fk_Nu)
        {
            /*
            4.3.2 Serviceability limit state
            The structural ductility factor, μ, for the serviceability limit state SLS1 shall be
            1.0 ≤ nu ≤ 1.25 and for SLS2 shall be within the limits 1.0. ≤ nu ≤ 2.0.
            */

            // SLS 1
            fNu = MathF.Min(MathF.Max(1.0f, fNu), 1.25f);

            fk_Nu = Get_k_nu__5211_ULS(fT_1, fNu, eSiteSoilClass);
            float fS_p = 0.7f; // 5.2.1.2
            return Eq_52_1____(fC_T1, fS_p, fk_Nu); // Eq. (5.2(1)) // fCd(T1)
        }
        public static float Eq_62_1____(float fC_d_T1, float fW_t)
        {
            return fC_d_T1 * fW_t; // Eq. (6.2(1)) // fV
        }
        public static float Eq_62_2____(bool bIsTopLevel, float fV, float fW_i, float fh_i, float fSumWihi)
        {
            float fF_t = 0;

            if (bIsTopLevel)
                fF_t = 0.08f * fV;

            return fF_t + 0.92f * fV * ((fW_i * fh_i) / fSumWihi); // Eq. (6.2(2)) // fF_i
        }
    }
}
