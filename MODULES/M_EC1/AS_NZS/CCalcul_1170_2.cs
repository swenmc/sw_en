using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MATH;
using BaseClasses;

namespace M_EC1.AS_NZS
{
    public class CCalcul_1170_2
    {
        BuildingDataInput sBuildInput;
        WindLoadDataInput sWindInput;

        // Table 3.3

        // Table 3.1

        float fV_R_ULS = 45; //m /s (ULS)
        float fV_R_SLS = 37; //m /s (SLS)

        float fM_D = 1.0f; // Wind direction multiplier (CI 3.3) Table 3.2
        float fM_s = 1.0f; // Shielding multiplier (CI 4.3) Table 4.3
        float fM_t = 1.0f; // Topographic multiplier (CI 4.4)

        float fSiteTerrainSlope_Phi = 1/20; // 3 deg
        float fSiteTerrainSlope_Phi_deg = 3;

        float fTerCat = 2.5f; // CI 4.2.1
        float fM_z_cat; // Terrain multiplier
        float z_max = 10; // m
        float fRho_air = 1.2f; // kgm^3

        // Table 4.1
        // Terrain category

        // CI2.3
        float fv_sit_beta_ULS = 41; // m/s    // Site wind speed
        float fv_sit_beta_SLS = 34; // m/s    // Site wind speed

        float fv_des_theta_ULS = 41; // m/s    // Site wind speed
        float fv_des_theta_SLS = 34; // m/s    // Site wind speed

        // Design wind pressure

        float fp_ULS;
        float fp_SLS;

        // Wind factors
        // ROOF
        // Along
        float fC_pe_r_al_2_min = -0.9f;
        float fC_pe_r_al_2_max = -0.4f;

        float fC_pe_al_3_min = -0.9f;
        float fC_pe_al_3_max = -0.4f;

        float fC_pe_al_4_min = -0.5f;
        float fC_pe_al_4_max =  0.0f;

        float fC_pe_al_5_min = -0.3f;
        float fC_pe_al_5_max = 0.1f;

        float fC_pe_al_6_min = -0.2f;
        float fC_pe_al_6_max = 0.2f;

        // Across

        float fC_pe_ac_2_min = -0.9f;
        float fC_pe_ac_2_max = -0.4f;

        float fC_pe_ac_3_min = -0.9f;
        float fC_pe_ac_3_max = -0.4f;

        float fC_pe_ac_4_min = -0.5f;
        float fC_pe_ac_4_max = 0.0f;

        float fC_pe_ac_5_min = -0.3f;
        float fC_pe_ac_5_max = 0.1f;

        float fC_pe_ac_6_min = -0.2f;
        float fC_pe_ac_6_max = 0.2f;

        // WALLS

        float fC_pe_r_al_1 = 0.7f;

        float fC_pe_ac_1 = 0.7f;

        public CCalcul_1170_2(BuildingDataInput sBuildingData_temp, WindLoadDataInput sWindData_temp)
        {
            sBuildInput = sBuildingData_temp;
            sWindInput = sWindData_temp;

            float fH_hill = 240f; // eight of the hill
            float fD_c_hill = 800f; // The sloping distance from the base of the hill to the crest
            float fD_s_hill = 235f; // Distance from the crest of the hill to the site

            fSiteTerrainSlope_Phi = (float)Math.Atan(fH_hill / fD_c_hill);
            fSiteTerrainSlope_Phi_deg = fSiteTerrainSlope_Phi / MathF.fPI * 180f;
            float fY = (fD_c_hill - fD_s_hill) / fD_c_hill;
            float fHillSlopeFactor = 0.4f; // TODO dopocitat interpolaciou

            if (fSiteTerrainSlope_Phi_deg <= 3) // SITES ON FLAT LAND OR UNDULATING HILLS LESS THAN 3˚. (1 : 20)
                fM_t = 1.0f;
            else if (fSiteTerrainSlope_Phi_deg < 24) // SITES WITH A SLOPE > 3˚ AND < 24˚
                fM_t = 1.0f;
            else // C - SITES WITHIN 10 TIMES THE HEIGHT OF AN ADJACENT HILL OR ESCARPMENT
            {
                float fEffectiveDistance_10H = fH_hill * 10;
                float fSiteDistance = (fEffectiveDistance_10H - fD_s_hill) / fEffectiveDistance_10H;
                float fSiteHillSlopeFactor = fHillSlopeFactor * fSiteDistance;

                fM_t = 1.0f + fSiteHillSlopeFactor;
            }

            fM_z_cat = sWindInput.fh / fTerCat;

            float fv_sit_ULS = fV_R_ULS * fM_D * fM_z_cat * fM_s * fM_t;
            float fv_sit_SLS = fV_R_ULS * fM_D * fM_z_cat * fM_s * fM_t;

            float fq_ULS = 0.6f * MathF.Pow2(fv_sit_ULS); // Pa
            float fq_SLS = 0.6f * MathF.Pow2(fv_sit_ULS); // Pa

            float fC_pe = 0f;
            float fC_pi = 0f;
            float fK_a = 0f;
            float fK_l = 0f;
            float fK_p = 0f;

            float fK_ce = 0.8f;
            float fK_ci = 0.0f;

            float fC_fig_e = fC_pe * fK_a * fK_ce * fK_l * fK_p; // Aerodynamic shape factor
            float fC_fig_i = fC_pi * fK_ci; // Aerodynamic shape factor

            float fC_dyn = 1.0f; // Dynamic response factor

            fp_ULS = (0.5f * fRho_air) * MathF.Pow2(fv_des_theta_ULS) * fC_fig_e * fC_dyn;
            fp_SLS = (0.5f * fRho_air) * MathF.Pow2(fv_des_theta_SLS) * fC_fig_e * fC_dyn;
        }

        // Table 5.3A
    }
}
