using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MATH;

namespace M_EC3
{
    class CL_63
    {
        float Eq_646_____(float fN_Ed, float fN_b_Rd)
        {
            if (fN_b_Rd > 0f)
                return fN_Ed / fN_b_Rd; // Eq. (6.46) Design Ratio
            else
                return 0f;
        }
        float Eq_647_____(float fChi, float fA, float ff_y, float fGamma_M1)
        {
            if (fGamma_M1 > 0f)
                return fChi * fA * ff_y / fGamma_M1; // Eq. (6.47) fN_b_Rd
            else
                return 0f;
        }
        float Eq_648_____(float fChi, float fA_eff, float ff_y, float fGamma_M1)
        {
            if (fGamma_M1 > 0f)
                return fChi * fA_eff * ff_y / fGamma_M1; // Eq. (6.48) fN_b_Rd
            else
                return 0f;
        }
        float Eq_649_Chi_(float fLambda_rel, float fPhi)
        {
                       if(fLambda_rel > 0f)
            return (float)Math.Min(1f / (fPhi + (float)Math.Sqrt(Math.Pow(fPhi, 2f) - (float)Math.Pow(fLambda_rel, 2f))), 1f); // Eq. (6.49) fChi
                       else
                           return 0f;
        }
        float Eq_649_Phi_(float fAlpha, float fLambda_rel, float fPhi)
        {
            return 0.5f * (1f + (fAlpha * (fLambda_rel - 0.2f) + (float)Math.Pow(fLambda_rel, 2))); // Eq. (6.49) fPhi
        }
        float Eq_Lambda_rel(float fA, float ff_y, float fN_cr)
        {
            if (fN_cr > 0f)
                return (float)Math.Sqrt(fA * ff_y / fN_cr); // fLambda_rel
            else
                return 0f;
        }
        /*
        float Eq_Lambda_rel(float fA_eff, float ff_y, float fN_cr)
        {
         if (fN_cr > 0f)
            return Math.Sqrt(fA_eff * ff_y / fN_cr); // fLambda_rel
         else
            return 0f;
        }
        */
        float Eq_650_____(float fL_cr, float fi, float fLambda_1)
        {
            if (fi > 0f && fLambda_1 > 0f)
                return (fL_cr / fi) * (1f / fLambda_1); // Eq. (6.50) fLambda_rel
            else
                return 0f;
        }
        float Eq_651_____(float fL_cr, float fi, float fLambda_1, float fA, float fA_eff)
        {
            if (fi > 0f && fLambda_1 > 0f)
                return (fL_cr / fi) * ((float)Math.Sqrt(fA_eff / fA) / fLambda_1); // Eq. (6.51) fLambda_rel
            else
                return 0f;
        }
        float Eq_Lambda_1(float fE, float ff_y)
        {
            if (ff_y > 0f)
                return (float)Math.PI * (float)Math.Sqrt(fE / ff_y); // fLambda_1
            else
                return 0f;
        }
        float Eq_Lambda_1(float fEpsilon)
        {
            return 93.9f * fEpsilon; // fLambda_1
        }
        float Eq_Epsilon__(float ff_y)
        {
            if (ff_y > 0f)
                return (float)Math.Sqrt(2.35e-11f / ff_y); // fEpsilon - SI Units [N/m2]
            else
                return 0f;
        }
        float Eq_652_____(float fA, float ff_y, float fN_cr)
        {
            if (fN_cr > 0f)
                return (float)Math.Sqrt(fA * ff_y / fN_cr); // Eq. (6.52) fLambda_rel_T
            else
                return 0f;
        }
        /*
        float Eq_653_____(float fA_eff, float ff_y, float fN_cr)
        {
            if (fN_cr > 0f)
                return (float)Math.Sqrt(fA_eff * ff_y / fN_cr); // Eq. (6.53) fLambda_rel_T
            else
                return 0f;
        }
        */
        float Eq_654_____(float fM_Ed, float fM_b_Rd)
        {
            if (fM_b_Rd > 0f)
                return fM_Ed / fM_b_Rd; // Eq. (6.54) Design Ratio
            else
                return 0f;
        }
        float Eq_655_____(float fChi_LT, float fW_y, float ff_y, float fGamma_M1)
        {
            if (fGamma_M1 > 0f)
                return fChi_LT * fW_y * ff_y / fGamma_M1; // Eq. (6.55) fM_b_Rd
            else
                return 0f;
        }
        float Eq_656_Chi_LT(float fLambda_rel_LT, float fPhi_LT)
        {
            if (fLambda_rel_LT > 0f)
                return (float)Math.Min(1f / (fPhi_LT + (float)Math.Sqrt(Math.Pow(fPhi_LT, 2f) - (float)Math.Pow(fLambda_rel_LT, 2f))), 1f); // Eq. (6.56) fChi_LT
            else
                return 0f;
        }
        float Eq_656_Phi_LT(float fAlpha_LT, float fLambda_rel_LT)
        {
            return 0.5f * (1f + (fAlpha_LT * (fLambda_rel_LT - 0.2f) + (float)Math.Pow(fLambda_rel_LT, 2f))); // Eq. (6.56) fPhi_LT
        }
        float Eq_Lambda_rel_LT(float fW_y, float ff_y, float fM_cr)
        {
            if (fM_cr > 0f)
                return (float)Math.Sqrt(fW_y * ff_y / fM_cr); // fLambda_rel_LT
            else
                return 0f;
        }
        float Eq_657_Chi_LT(float fLambda_rel_LT, float fPhi_LT, float fBeta)
        {
            if (fLambda_rel_LT > 0f)
                return (float)Math.Min(1f / (fPhi_LT + (float)Math.Sqrt(Math.Pow(fPhi_LT, 2f) - (fBeta * (float)Math.Pow(fLambda_rel_LT, 2f)))), (float)Math.Min(1f, 1f / Math.Pow(fLambda_rel_LT, 2f))); // Eq. (6.57) fChi_LT
            else
                return 0f;
        }
        float Eq_657_Phi_LT(float fAlpha_LT, float fLambda_rel_LT, float fLambda_rel_LT_0, float fBeta)
        {
            return 0.5f * (1f + (fAlpha_LT * (fLambda_rel_LT - fLambda_rel_LT_0) + (fBeta * (float)Math.Pow(fLambda_rel_LT, 2)))); // Eq. (6.57) fPhi_LT
        }
        float Eq_658_____(float fChi_LT, float ff)
        {
            if (ff > 0f)
                return fChi_LT / ff; // Eq. (6.58) fChi_LT_mod
            else
                return 0f;
        }
        float Eq_658_f___(float fk_c, float fLambda_rel_LT)
        {
            if (fLambda_rel_LT > 0f)
                return (float)Math.Min(1f - 0.5f * (1f - fk_c) * (1f - 2f * Math.Pow((fLambda_rel_LT - 0.8f), 2f)), 1f); // Eq. (6.58) ff
            else
                return 0f;
        }
        float Eq_659_____(float fk_c, float fL_c, float fi_fz, float fLambda_1, float fLambda_rel_c0, float fM_c_Rd, float fM_y_Ed)
        {
            if (fi_fz > 0f && fLambda_1 > 0f && fM_y_Ed > 0f)
                return (float)Math.Min(fk_c * fL_c / (fi_fz * fLambda_1), fLambda_rel_c0 * fM_c_Rd / fM_y_Ed); // Eq. (6.59) fLambda_rel_f
            else
                return 0f;
        }
        float Eq_660_____(float fk_fL, float fChi, float fM_c_Rd)
        {
            if (fM_c_Rd > 0f)
                return (float)Math.Min(fk_fL * fChi * fM_c_Rd, fM_c_Rd); // Eq. (6.60) fM_b_Rd
            else
                return 0f;
        }
        float Eq_661_____(float fN_Ed, float fChi_y, float fN_Rk, float fk_yy, float fM_y_Ed, float fDelta_M_y_Ed, float fChi_LT, float fM_y_Rk, float fk_yz, float fM_z_Ed, float fDelta_M_z_Ed, float fM_z_Rk, float fGamma_M1)
        {
            if (fChi_y > 0f && fN_Rk > 0f && fChi_LT > 0f && fM_y_Rk > 0f && fM_z_Rk > 0f)
                return ((fN_Ed / (fChi_y * (fN_Rk / fGamma_M1))) + (fk_yy * (fM_y_Ed + fDelta_M_y_Ed) / (fChi_LT * (fM_y_Rk / fGamma_M1))) + (fk_yz * ((fM_z_Ed + fDelta_M_z_Ed) / (fM_z_Rk / fGamma_M1)))) / 1f; // Eq. (6.61) Design Ratio
            else
                return 0f;
        }
        float Eq_662_____(float fN_Ed, float fChi_z, float fN_Rk, float fk_zy, float fM_y_Ed, float fDelta_M_y_Ed, float fChi_LT, float fM_y_Rk, float fk_zz, float fM_z_Ed, float fDelta_M_z_Ed, float fM_z_Rk, float fGamma_M1)
        {
            if (fChi_z > 0f && fN_Rk > 0f && fChi_LT > 0f && fM_y_Rk > 0f && fM_z_Rk > 0f)
                return ((fN_Ed / (fChi_z * (fN_Rk / fGamma_M1))) + (fk_zy * (fM_y_Ed + fDelta_M_y_Ed) / (fChi_LT * (fM_y_Rk / fGamma_M1))) + (fk_zz * ((fM_z_Ed + fDelta_M_z_Ed) / (fM_z_Rk / fGamma_M1)))) / 1f; // Eq. (6.62) Design Ratio
            else
                return 0f;
        }
        // Equations
        float Eq_661_Nc__(float fN_Ed, float fChi_y, float fN_Rk, float fGamma_M1)
        {
            if (fChi_y > 0f && fN_Rk > 0f)
                return (fN_Ed / (fChi_y * (fN_Rk / fGamma_M1))) / 1f; // Eq. (6.61) Design Ratio
            else
                return 0f;
        }

        float Eq_661_My__(float fk_yy, float fM_y_Ed, float fDelta_M_y_Ed, float fChi_LT, float fM_y_Rk, float fGamma_M1)
        {
            if (fChi_LT > 0f && fM_y_Rk > 0f)
                return (fk_yy * (fM_y_Ed + fDelta_M_y_Ed) / (fChi_LT * (fM_y_Rk / fGamma_M1))) / 1f; // Eq. (6.61) Design Ratio
            else
                return 0f;
        }

        float Eq_661_Mz__(float fk_yz, float fM_z_Ed, float fDelta_M_z_Ed, float fM_z_Rk, float fGamma_M1)
        {
            if (fM_z_Rk > 0f)
                return (fk_yz * ((fM_z_Ed + fDelta_M_z_Ed) / (fM_z_Rk / fGamma_M1))) / 1f; // Eq. (6.61) Design Ratio
            else
                return 0f;
        }
        float Eq_662_Nc__(float fN_Ed, float fChi_z, float fN_Rk, float fGamma_M1)
        {
            if (fChi_z > 0f && fN_Rk > 0f)
                return (fN_Ed / (fChi_z * (fN_Rk / fGamma_M1))) / 1f; // Eq. (6.62) Design Ratio
            else
                return 0f;
        }
        float Eq_662_My__(float fk_zy, float fM_y_Ed, float fDelta_M_y_Ed, float fChi_LT, float fM_y_Rk, float fGamma_M1)
        {
            if (fChi_LT > 0f && fM_y_Rk > 0f)
                return (fk_zy * (fM_y_Ed + fDelta_M_y_Ed) / (fChi_LT * (fM_y_Rk / fGamma_M1))) / 1f; // Eq. (6.62) Design Ratio
            else
                return 0f;
        }
        float Eq_662_Mz__(float fk_zz, float fM_z_Ed, float fDelta_M_z_Ed, float fM_z_Rk, float fGamma_M1)
        {
            if (fM_z_Rk > 0f)
                return (fk_zz * ((fM_z_Ed + fDelta_M_z_Ed) / (fM_z_Rk / fGamma_M1))) / 1f; // Eq. (6.62) Design Ratio
            else
                return 0f;
        }

        // General Method 6.3.4

        float Eq_663_____(float fChi_op, float fAlpha_ult_k, float fGamma_M1)
        {
            if (fGamma_M1 > 0f)
                return (fChi_op * fAlpha_ult_k / fGamma_M1) / 1f; // Eq. (6.63) Condition
            else
                return 0f;
        }
        float Eq_664_____(float fAlpha_ult_k, float fAlpha_cr_op)
        {
            if (fAlpha_ult_k > 0f && fAlpha_cr_op > 0f)
                return (float)Math.Sqrt(fAlpha_ult_k / fAlpha_cr_op); // Eq. (6.64) fLambda_rel_op
            else
                return 0f;
        }
        float Eq_665_____(float fN_Ed, float fN_Rk, float fM_y_Ed, float fM_y_Rk, float fGamma_M1, float fChi_op)
        {
            if (fN_Rk > 0f && fM_y_Rk > 0f && fChi_op > 0f)
                return ((fN_Ed / (fN_Rk / fGamma_M1)) + (fM_y_Ed / (fM_y_Rk / fGamma_M1))) / fChi_op; // Eq. (6.65) Design Ratio
            else
                return 0f;
        }
        float Eq_666_____(float fN_Ed, float fChi, float fN_Rk, float fM_y_Ed, float fChi_LT, float fM_y_Rk, float fGamma_M1)
        {
            if (fChi > 0f && fN_Rk > 0f && fChi_LT > 0f && fM_y_Rk > 0f)
                return ((fN_Ed / (fChi * fN_Rk / fGamma_M1)) + (fM_y_Ed / (fChi_LT * fM_y_Rk / fGamma_M1))) / 1f; // Eq. (6.66) Design Ratio
            else
                return 0f;
        }

        // Maria Kralova EN 1993-3-1
        float Eq_B21c___(float fH, float fV_H, float fD_o, float fm_o, float fR)
        {
            return (1.0f / 30.0f) * (float)MathF.Pow_1_3((fH * fV_H) / fD_o) * (float)Math.Sqrt(fm_o / (fH * fR));
        }

        
        
        


    }
}
