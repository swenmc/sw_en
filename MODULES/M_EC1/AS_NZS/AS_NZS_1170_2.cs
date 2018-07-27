using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MATH;
using BaseClasses;
using DATABASE;

namespace M_EC1.AS_NZS
{
    public static class AS_NZS_1170_2
    {
        public static float Eq_22______(float fV_R, float fM_d, float fM_z_cat, float fM_s, float fM_t)
        {
            return fV_R * fM_d * fM_z_cat * fM_s * fM_t; // Eq. (2.2) // fV_sit_beta
        }
        public static float Eq_24_1____(float fRho_air, float fV_des_theta, float fC_fig, float fC_dyn)
        {
            return (0.5f * fRho_air) * MathF.Pow2(fV_des_theta) * fC_fig * fC_dyn; // Eq. (2.4(1)) // fp
        }
        public static float Eq_24_2____(float fRho_air, float fV_des_theta, float fC_fig, float fC_dyn)
        {
            return (0.5f * fRho_air) * MathF.Pow2(fV_des_theta) * fC_fig * fC_dyn; // Eq. (2.4(1)) // ff
        }
        public static float Eq_25_3____(float fRho_air, float fV_des_theta, float fC_fig, float fC_dyn, float fA_z)
        {
            return (0.5f * fRho_air) * MathF.Pow2(fV_des_theta) * fC_fig * fC_dyn * fA_z; // Eq. (2.5(3)) // fF
        }
        public static float Eq_25_4____(float fN_g)
        {
            return 0.7f * MathF.Pow2((float)Math.Log(fN_g)) - 17.4f * (float)Math.Log(fN_g) + 100.0f; // Eq. (2.5(4)) // fSigma_to_Sigma_max
        }
        public static float Eq_32_V_R__(int iTimeinYears, EWindRegion eRegion)
        {
            // 3.4 Factors for regions C and D , F_C and F_D

            float fF_C = 1.0f; // 3.4 (b)
            float fF_D = 1.0f; // 3.4 (b)

            if (iTimeinYears > 50) // 3.4 (a)
            {
                fF_C = 1.05f; // 3.4 (a)
                fF_D = 1.1f; // 3.4 (a)
            }


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
        public static float Eq_43_1____(float fl_s, float fh_s, float fb_s)
        {
            if (fh_s * fb_s > 0)
                return fl_s / MathF.Sqrt(fh_s * fb_s); // Eq. (4.3(1)) // fs // Shielding parameter
            else
                return 0; // Exception
        }
        public static float Eq_43_2____(float fh, int in_s)
        {
            if (in_s > 0)
                return fh * ((10f / in_s) + 5f); // Eq. (4.3(2)) // fls // Average spacing of shielding buildings
            else
                return 0; // Exception
        }
        public static float Eq_43_1____(float fl_s, float fh_s, float fb_s, float fh, int in_s)
        {
            fl_s = Eq_43_2____(fh, in_s);
            return Eq_43_1____(fl_s, fh_s, fb_s);
        }
        public static float Eq_44_1____(float fM_h, float fM_lee, float fE_meters)
        {
            // 4.4.1 (a)
            if (fE_meters > 500f)
                return fM_h * fM_lee * (1.0f + 0.00015f * fE_meters); // Eq. (4.4(1)) // fM_t
            else
                return MathF.Max(fM_h, fM_lee);
        }
        public static float Eq_44_2____(float fH, float fx, float fz, float fL_1, float fL_2)
        {
            return 1.0f + (fH / (3.5f * (fz + fL_1))) * (1.0f - (Math.Abs(fx) / fL_2)); // Eq. (4.4(2)) // fM_h
        }
        public static float Eq_44_3____(float fx, float fL_2)
        {
            return 1.0f + 0.7f * (1.0f - (Math.Abs(fx) / fL_2)); // Eq. (4.4(3)) // fM_h
        }
        public static float Get_Mh_442_(bool bIsEscarpment, float fH, float fH_2Lu_ratio, float fx, float fz)
        {
            /*
            H = height of the hill, ridge or escarpment
            Lu = horizontal distance upwind from the crest of the hill, ridge or escarpment to a level half the height below the crest
            x = horizontal distance upwind or downwind of the structure to the crest of the hill, ridge or escarpment
            L1 = length scale, to determine the vertical variation of Mh, to be taken as the greater of 0.36 Lu or 0.4 H
            L2 = length scale, to determine the horizontal variation of Mh, to be taken as 4 L1 upwind for all types, and downwind for hills and ridges, or 10 L1 downwind for escarpments
            z = reference height on the structure above the average local ground level
            */

            float fL_u = 0.5f * fH / fH_2Lu_ratio;

            bool bIsInSeparationZone = true; // TODO - doplnit zadanie podla obrazku
            float fL_1 = Math.Max(0.36f * fL_u, 0.4f * fH);
            float fL_2 = MathF.Max(1.44f * fL_u, 1.6f * fH);

            if(bIsEscarpment)
                fL_2 = MathF.Max(3.6f * fL_u, 4f * fH);

            if (fH_2Lu_ratio < 0.05f)
                return 1.0f; // 4.4.2 (a)
            else if (fH_2Lu_ratio < 0.45f || !bIsInSeparationZone)
                return Eq_44_2____(fH, fx, fz, fL_1, fL_2); // 4.4.2 (b) or (c) (ii)
            else
            {
                return Eq_44_3____(fx, fL_2); // 4.4.2(c) (i) // Is in separation zone
            }
        }
        public static float Get_Mh_v1__(bool bIsEscarpment, float fH, float fL_u, float fx, float fz)
        {
            float fH_2Lu_ratio = fH / (2f * fL_u);
            return Get_Mh_442_(bIsEscarpment, fH, fH_2Lu_ratio, fx, fz);
        }
        public static float Get_Mh_v2__(bool bIsEscarpment, float fH, float fHillSlope_degrees, float fx, float fz)
        {
            float fH_2Lu_ratio = (float)Math.Tan((fHillSlope_degrees / 180) * Math.PI);
            return Get_Mh_442_(bIsEscarpment, fH, fH_2Lu_ratio, fx, fz);
        }
        public static float Get_Mt_factor_alt(float fH_hill, float fD_c_hill, float fD_s_hill)
        {
            // return Mt value

            //fH_hill = 240f; // Height of the hill
            //fD_c_hill = 800f; // The sloping distance from the base of the hill to the crest
            //fD_s_hill = 235f; // Distance from the crest of the hill to the site

            float fSiteTerrainSlope_Phi = (float)Math.Atan(fH_hill / fD_c_hill);
            float fSiteTerrainSlope_Phi_deg = fSiteTerrainSlope_Phi / MathF.fPI * 180f;

            float fY = (fD_c_hill - fD_s_hill) / fD_c_hill;
            float fHillSlopeFactor = 0.4f; // TODO dopocitat interpolaciou

            if (fSiteTerrainSlope_Phi_deg <= 3) // SITES ON FLAT LAND OR UNDULATING HILLS LESS THAN 3˚. (1 : 20)
                return 1.0f;
            else if (fSiteTerrainSlope_Phi_deg < 24) // SITES WITH A SLOPE > 3˚ AND < 24˚
                return 1.0f;
            else // C - SITES WITHIN 10 TIMES THE HEIGHT OF AN ADJACENT HILL OR ESCARPMENT
            {
                float fEffectiveDistance_10H = fH_hill * 10;
                float fSiteDistance = (fEffectiveDistance_10H - fD_s_hill) / fEffectiveDistance_10H;
                float fSiteHillSlopeFactor = fHillSlopeFactor * fSiteDistance;

                return 1.0f + fSiteHillSlopeFactor;
            }
        }

        static int[,] arrTable3_1 = new int[15, 6]
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
        public static int Table31_Interpolation_positive(int x, EWindRegion eRegion, int[,] TableValues)
        {
            if (TableValues != null)
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
