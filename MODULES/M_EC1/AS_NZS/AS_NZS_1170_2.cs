using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MATH;

namespace M_EC1.AS_NZS
{
    public class AS_NZS_1170_2
    {
        public enum EWindRegion
        {
            eA1,
            eA2,
            eA3,
            eA4,
            eA5,
            eA6,
            eA7,
            eW,
            eB,
            eC,
            eD
        }

        public float Eq_22______(float fV_R, float fM_d, float fM_z_cat, float fM_s, float fM_t)
        {
            return fV_R * fM_d * fM_z_cat * fM_s * fM_t; // Eq. (2.2) // fV_sit_beta
        }
        public float Eq_24_1____(float fRho_air, float fV_des_theta, float fC_fig, float fC_dyn)
        {
            return (0.5f * fRho_air) * MathF.Pow2(fV_des_theta) * fC_fig * fC_dyn; // Eq. (2.4(1)) // fp
        }
        public float Eq_24_2____(float fRho_air, float fV_des_theta, float fC_fig, float fC_dyn)
        {
            return (0.5f * fRho_air) * MathF.Pow2(fV_des_theta) * fC_fig * fC_dyn; // Eq. (2.4(1)) // ff
        }
        public float Eq_25_3____(float fRho_air, float fV_des_theta, float fC_fig, float fC_dyn, float fA_z)
        {
            return (0.5f * fRho_air) * MathF.Pow2(fV_des_theta) * fC_fig * fC_dyn * fA_z; // Eq. (2.5(3)) // fF
        }
        public float Eq_25_4____(float fN_g)
        {
            return 0.7f * MathF.Pow2((float)Math.Log(fN_g)) - 17.4f * (float)Math.Log(fN_g) + 100.0f; // Eq. (2.5(4)) // fSigma_to_Sigma_max
        }
        public float Eq_25_4____(int iTimeinYears, EWindRegion eRegion, float fF_C = 1.0f, float fF_D = 1.0f)
        {
            if (iTimeinYears >= 5)
            {
                switch (eRegion) // Table 3.1
                {
                    case EWindRegion.eA1:
                    case EWindRegion.eA2:
                    case EWindRegion.eA3:
                    case EWindRegion.eA4:
                    case EWindRegion.eA5:
                    case EWindRegion.eA6:
                    case EWindRegion.eA7:
                        return (float)Math.Round(67f - 41f * (float)Math.Pow(iTimeinYears, -0.1f)); // Non-cyclonic
                    case EWindRegion.eW:
                        return (float)Math.Round(104f - 70f * (float)Math.Pow(iTimeinYears, -0.045f)); // Non-cyclonic
                    case EWindRegion.eB:
                        return (float)Math.Round(106f - 92f * (float)Math.Pow(iTimeinYears, -0.1f)); // Non-cyclonic
                    case EWindRegion.eC:
                        return (float)(Math.Round(fF_C * (122f - 104f * (float)Math.Pow(iTimeinYears, -0.1f)))); // Cyclonic
                    case EWindRegion.eD:
                        return (float)(Math.Round(fF_D * (156f - 142f * (float)Math.Pow(iTimeinYears, -0.1f)))); // Cyclonic
                    default:
                        return 0;
                }
            }
            else // Table values
            {
                if (eRegion < EWindRegion.eC)
                {
                    return Table31_Interpolation_positive(iTimeinYears, eRegion, arrTable3_1);
                }
                else
                {
                    float iV_temp = Table31_Interpolation_positive(iTimeinYears, eRegion, arrTable3_1);

                    if (eRegion == EWindRegion.eC)
                    {
                        return (int)(iV_temp *= ((eRegion == EWindRegion.eC) ? fF_C : fF_D));
                    }
                    else
                        return 0;
                }
            }
        }
        int[,] arrTable3_1 = new int[15, 6]
            {
            {1, 30, 34, 26, 23, 23},
            {5, 32, 39, 28, 33, 35},
            {10, 34, 41, 33, 39, 43},
            {20, 37, 43, 38, 45, 51},
            {25, 37, 43, 39, 47, 53},
            {50, 39, 45, 44, 52, 60},
            {100, 41, 47, 48, 56, 66},
            {200, 43, 49, 52, 61, 72},
            {250, 43, 49, 53, 62, 74},
            {500, 45, 51, 57, 66, 80},
            {1000, 46, 53, 60, 70, 85},
            {2000, 48, 54, 63, 73, 90},
            {2500, 48, 55, 64, 74, 91},
            {5000, 50, 56, 67, 78, 95},
            {10000, 51, 58, 69, 81, 99}
            };

        // Interpolation - aby fungovalo obecne, je potrebne doriesit kladne a zaporne hodnoty
        public int Table31_Interpolation_positive(int x, EWindRegion eRegion, int[,] TableValues)
        {
            if (TableValues.Length != null)
            {
                for (int i = 0; i < TableValues.Length / 6; i++)
                {
                    if (x > TableValues[i, 0])
                    {
                        // Find nearest higher value
                        if (x - TableValues[i, 0] == 0)
                        {
                            if (eRegion < EWindRegion.eW)
                                return TableValues[i, 1];
                            else
                                return TableValues[i, (int)(eRegion + 6)];
                        }
                        else if (x - TableValues[i, 0] > 0) // xi > xi-1
                        {
                            if (eRegion < EWindRegion.eW)
                                return TableValues[i, 1] + (TableValues[i, 0] - x) * ((TableValues[i - 1, 1] - TableValues[i, 1]) / (TableValues[i, 0] - TableValues[i - 1, 0]));
                            else
                                return TableValues[i, (int)(eRegion + 6)] + (TableValues[i, 0] - x) * ((TableValues[i - 1, (int)(eRegion + 6)] - TableValues[i, (int)(eRegion + 6)]) / (TableValues[i, 0] - TableValues[i - 1, 0]));
                        }
                        else
                        {
                            // Dopracovat TODO
                            return 0;
                        }
                    }
                    else
                    {
                        if (eRegion < EWindRegion.eW)
                            return TableValues[0, 1]; // Minimum
                        else
                            return TableValues[0, (int)(eRegion + 6)]; // Minimum
                    }
                }

                return 0;
            }
            else
            {
                return 0;
            }
        }
    }
}
