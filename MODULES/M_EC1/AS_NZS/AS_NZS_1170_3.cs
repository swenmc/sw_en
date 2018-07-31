using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MATH;
using BaseClasses;

namespace M_EC1.AS_NZS
{
    public static class AS_NZS_1170_3
    {
        public static float Eq_42_1____(float fs_g, float fC_e, float fNu_i)
        {
            return fs_g * fC_e * fNu_i; // Eq. (4.2(1)) // fs
        }
        public static float Get_Ce_422_(ESnowElevationRegions snowRegion, ERoofExposureCategory eRoofCategory)
        {
            /*
            Exposure categories are defined as follows:
            (a)Sheltered Sites where the roof is protected from the wind by obstructions such as
            other structures, terrain features or numbers of closely spaced trees higher than the
            roof.

            (b) Semi-sheltered Sites where the roof is only partially protected by numbers of
            scattered obstructions higher than the roof(e.g., scattered trees).

            (c) Windswept Sites where the roof is exposed on all sides, with no protection provided
            by obstructions, trees, or terrain features higher than the roof.
            */
            if (snowRegion == ESnowElevationRegions.eSubAlpine || eRoofCategory == ERoofExposureCategory.eSheltered)
                return 1f;
            else if (eRoofCategory == ERoofExposureCategory.eSemiSheltered)
                return 0.75f;
            else if (eRoofCategory == ERoofExposureCategory.eWindswept)
                return 0.6f;
            else
                return 1f; // Exception -return 1.0 if not defined
        }
        public static float Eq_42_2____(float fs_g, float fC_e, float fNu_i, float fk, float fGamma)
        {
            return fk * MathF.Pow2(fs_g * fC_e * fNu_i) / fGamma; // Eq. (4.2(2)) // fs_e
        }
        public static float Eq_42_2____(float fs, float fk, float fGamma)
        {
            return fk * MathF.Pow2(fs) / fGamma; // Eq. (4.2(2)) // fs_e
        }
        public static float Eq_42_2____(float fs, ESnowElevationRegions snowRegion)
        {
            float fGamma = 2900f; // 2.9  kN / m^3

            if (snowRegion == ESnowElevationRegions.eAlpine)
                fGamma = 4300f; // 4.3 kN / m^3
            /*
            else if (snowRegion == ESnowRegions.eSubAlpine)
                fGamma = 2900; // 2.9  kN / m^3
            */

            return 0.5f * MathF.Pow2(fs) / fGamma; // Eq. (4.2(2)) // fs_e
        }
        public static float Get_F_424__(float fs, float fb, float fAlpha_deg)
        {
            return fs * fb * (float)Math.Sin(fAlpha_deg / 180 * Math.PI);
        }
        public static float Get_F_424__(float fs_g, float fC_e, float fNu_i, float fb, float fAlpha_deg)
        {
            return (fs_g * fC_e * fNu_i) * fb * (float)Math.Sin(fAlpha_deg / 180 * Math.PI);
        }
        public static float Get_kp_52__(float fP, ECountry country)
        {
            if (country == ECountry.eAustralia)
                return 0.26f * (float)Math.Log(1f / fP) + 0.22f;  // fk_p (Australia)
            else if (country == ECountry.eNewZealand)
                return 0.215f * (float)Math.Log(1f / fP) + 0.18f; // fk_p (New Zealand)
            else
                return -1; // Exception - Country is note defined
        }
        public static float Eq_53_1____(float fk_p, float fk_t, float fh_0_meters)
        {
            /*
            kt = multiplying factor for terrain classification (Australia only), given in Table 5.2 or as follows:
            = 0.7 for areas in terrain classification 1(net removal of ground snow depth)
            = 1.0 for areas in terrain classification 2(no removal or increase of ground snow depth)
            = 1.3 for areas in terrain classification 3(net increase of ground snow depth)
            */
            return fk_p * fk_t * (float)Math.Pow(fh_0_meters / 1000f, 4.4f); // fs_g
        }
        public static float Eq_53_2____(float fk_p, float fk_t, float fh_0_meters)
        {
            /*
            kt = multiplying factor for terrain classification (Australia only), given in Table 5.2 or as follows:
            = 0.7 for areas in terrain classification 1(net removal of ground snow depth)
            = 1.0 for areas in terrain classification 2(no removal or increase of ground snow depth)
            = 1.3 for areas in terrain classification 3(net increase of ground snow depth)
            */
            return fk_p * fk_t * (float)Math.Pow((fh_0_meters + 300f) / 1000f, 4.4f); // fs_g
        }
        public static float Eq_53_3____(float fk_p, float fh_0_meters)
        {
            return fk_p * (float)Math.Pow(fh_0_meters / 1000f, 4.4f); // fs_g
        }
        public static float Eq_53_4____(float fk_p, float fh_0_meters)
        {
            return fk_p * (float)Math.Pow((fh_0_meters + 450f) / 1000f, 3.2f); // fs_g
        }
        public static float Eq_54_1____(float fk_p, float fk_l, float fh_0_meters)
        {
            /*
            kl = multiplying factor for latitude, given as follows:
            = 0.2 for Region AN (Northern Tablelands of N.S.W)
            = 0.7 for Region AC (Central Tablelands of N.S.W)
            = 1.0 for Region AS (Southern Tablelands of N.S.W., A.C.T.and Vic)
            = 1.6 for Region AT (Tasmania)
            */

            return fk_p * fk_l * (((2.8f * fh_0_meters) / 1000f) - 1.2f); // fs_g
        }
        public static float Eq_54_2____(float fk_p, float fk_l, float fh_0_meters)
        {
            /*
            kl = multiplying factor for latitude, given as follows:
            = 0.2 for Region AN (Northern Tablelands of N.S.W)
            = 0.7 for Region AC (Central Tablelands of N.S.W)
            = 1.0 for Region AS (Southern Tablelands of N.S.W., A.C.T.and Vic)
            = 1.6 for Region AT (Tasmania)
            */

            return fk_p * fk_l * (((2.8f * (fh_0_meters + 300f)) / 1000f) - 1.2f); // fs_g
        }
        public static float Eq_54_3____(float fk_p, float fh_0_meters)
        {
            return fk_p * ((2.4f * (fh_0_meters - 250f)) / 1000f); // fs_g
        }
        public static float Eq_54_4____(float fk_p, float fh_0_meters)
        {
            return fk_p * ((2.7f * (fh_0_meters - 150f)) / 1000f); // fs_g
        }
        public static float Eq_54_5____(float fk_p, float fh_0_meters)
        {
            return fk_p * ((3f * (fh_0_meters - 150f)) / 1000f); // fs_g
        }
        public static float Eq_54_6____(float fk_p, float fh_0_meters)
        {
            return fk_p * 1.2f * (((3f * fh_0_meters) / 1000f) + 0.3f); // fs_g
        }
        public static float Eq_54_7____(float fk_p, float fh_0_meters)
        {
            return fk_p * 1.2f * (((2f * fh_0_meters) / 1000f) + 0.7f); // fs_g
        }
        public static float Eq_54_8____(float fk_p, float fh_0_meters)
        {
            return fk_p * 1.2f * (((1.5f * fh_0_meters) / 1000f) + 0.3f); // fs_g
        }
        public static float Eq_54_9____(float fk_p, float fh_0_meters)
        {
            return fk_p * 1.2f * (((2f * fh_0_meters) / 1000f) + 0.1f); // fs_g
        }
        public static float Get_sg_53__(ECountry country, ESnowRegion eSnowRegion, ESnowElevationRegions snowRegionElevation, float fk_p, float fh_0_meters)
        {
            // Alpine regions
            if (country == ECountry.eAustralia)
            {
                return -1; // Exception - not implemented
            }
            else if (country == ECountry.eNewZealand)
            {
                if (snowRegionElevation == ESnowElevationRegions.eAlpine)
                {
                    switch (eSnowRegion)
                    {
                        case ESnowRegion.eN0:
                        case ESnowRegion.eN1:
                            {
                                if (fh_0_meters >= 1200f)
                                    return Eq_54_3____(fk_p, fh_0_meters);
                                else
                                    return -1f; // Exception
                            }
                        case ESnowRegion.eN2:
                        case ESnowRegion.eN3:
                        case ESnowRegion.eN4:
                        case ESnowRegion.eN5:
                            {
                                if (fh_0_meters >= 900f)
                                    return Eq_54_4____(fk_p, fh_0_meters);
                                else
                                    return -1f; // Exception
                            }
                        default:
                            return -1f; // Exception - not defined snow region
                    }
                }
                else
                    return -1;  // Exception - not alpine region
            }
            else
            {
                return -1; // Exception - country is not implemented
            }
        }
        public static float Get_sg_54__(ECountry country, ESnowRegion eSnowRegion, ESnowElevationRegions snowRegionElevation, float fk_p, float fh_0_meters)
        {
            float fs_g;
            if (country == ECountry.eAustralia)
                fs_g = Get_sg_542_(eSnowRegion, fk_p, fh_0_meters); // cl. 5.4.2 Australia
            else if (country == ECountry.eNewZealand)
                fs_g = Get_sg_543_(eSnowRegion, fk_p, fh_0_meters); // cl. 5.4.3 New Zealand
            else
                return - 1; // Exception - not implemented country

            //Clause 4.2.1 For all sub - alpine areas where the ground snow load(sg) is less than 0.75 kPa, design shall be on the basis of a balanced distributed load only of 0.4 kPa.

            if (fs_g < 7500f) // 0.75 kPa
                fs_g = 4000f; // 0.4 kPa

            return fs_g;
        }
        public static float Get_sg_542_(ESnowRegion eSnowRegion, float fk_p, float fh_0_meters)
        {
            return -1; // Not implemented for Australia
        }
        public static float Get_sg_543_(ESnowRegion eSnowRegion, float fk_p, float fh_0_meters)
        {
            // Sub-alpine regions
            switch (eSnowRegion)
            {
                case ESnowRegion.eN0:
                    return 0;
                case ESnowRegion.eN1:
                    {
                        if ((fh_0_meters >= 400f) && (fh_0_meters <= 1200f))
                            return Eq_54_3____(fk_p, fh_0_meters);
                        else
                            return -1f; // Exception
                    }
                case ESnowRegion.eN2:
                    {
                        if ((fh_0_meters >= 200f) && (fh_0_meters <= 900f))
                            return Eq_54_4____(fk_p, fh_0_meters);
                        else
                            return -1f; // Exception
                    }
                case ESnowRegion.eN3:
                    {
                        if ((fh_0_meters >= 150f) && (fh_0_meters <= 900f))
                            return Eq_54_5____(fk_p, fh_0_meters);
                        else
                            return -1f; // Exception
                    }
                case ESnowRegion.eN4:
                    {
                        if (fh_0_meters < 400f)
                            return Eq_54_6____(fk_p, fh_0_meters);
                        else
                            return Eq_54_7____(fk_p, fh_0_meters);
                    }
                case ESnowRegion.eN5:
                    {
                        if (fh_0_meters < 400f)
                            return Eq_54_8____(fk_p, fh_0_meters);
                        else
                            return Eq_54_9____(fk_p, fh_0_meters);
                    }
                default:
                    {
                        return -1f; // Exception - not defined snow region
                    }
            }
        }
        public static float Get_sg_53_54(ECountry country, ESnowRegion eSnowRegion, ESnowElevationRegions snowRegionElevation, float fk_p, float fh_0_meters)
        {
            if (snowRegionElevation == ESnowElevationRegions.eAlpine)
                return Get_sg_53__(country, eSnowRegion, snowRegionElevation, fk_p, fh_0_meters);
            else if (snowRegionElevation == ESnowElevationRegions.eSubAlpine)
                return Get_sg_54__(country, eSnowRegion, snowRegionElevation, fk_p, fh_0_meters);
            else if (snowRegionElevation == ESnowElevationRegions.eNoSignificantSnow)
                return 0;
            else
                return -1; // Exception - not defined snow elevation region
        }
        public static float Get_sg_5___(ECountry country, ESnowRegion eSnowRegion, ESnowElevationRegions snowRegionElevation, float fh_0_meters, float fP)
        {
            float fk_p = Get_kp_52__(fP, country);

            return Get_sg_53_54(country, eSnowRegion, snowRegionElevation, fk_p, fh_0_meters);
        }
        public static float Get_Nu_1_62(float fAlpha_deg)
        {
            // FIGURE 6.1 BALANCED SHAPE COEFFICIENT ON A SLOPE
            return MathF.Min(MathF.Max(0, 0.7f * (60f - fAlpha_deg) / 50f), 0.7f); // fNu_1 Figure 6.1
        }
        public static float Get_Nu_1_63(float fAlpha_deg)
        {
            // FIGURE 6.2 MONO-PITCHED ROOF
            return Get_Nu_1_62(fAlpha_deg); // fNu_1 Figure 6.2
        }
        public static float Get_Nu_2_64(float fAlpha_deg)
        {
            return MathF.Min(MathF.Max(0, 0.56f * (60f - fAlpha_deg) / 50f), 0.56f); // fNu_2 Figure 6.3
        }
        public static void Set_Nu_64(float fAlpha_1, float fAlpha_2, ref float fC_e, out float fNu1_Alpha1, out float fNu2_Alpha1, out float fNu1_Alpha2, out float fNu2_Alpha2)
        {
            fNu1_Alpha1 = Get_Nu_1_62(fAlpha_1);
            fNu1_Alpha2 = Get_Nu_1_62(fAlpha_2);

            fNu2_Alpha1 = Get_Nu_2_64(fAlpha_1);
            fNu2_Alpha2 = Get_Nu_2_64(fAlpha_2);

            if (fC_e < 0.95f) // Figure 6.3 DUO-PITCHED ROOFS
                fC_e = 0.95f;
        }
        public static void Set_Nu_64(float fAlpha_1, ref float fC_e, out float fNu1_Alpha1, out float fNu2_Alpha1)
        {
            fNu1_Alpha1 = Get_Nu_1_62(fAlpha_1);

            fNu2_Alpha1 = Get_Nu_2_64(fAlpha_1);

            if (fC_e < 0.95f) // Figure 6.3 DUO-PITCHED ROOFS
                fC_e = 0.95f;
        }

    }
}
