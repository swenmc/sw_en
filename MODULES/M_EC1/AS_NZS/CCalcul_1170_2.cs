using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MATH;
//using System.Data.SQLite;
using System.Configuration;
using BaseClasses;

namespace M_EC1.AS_NZS
{
    public class CCalcul_1170_2
    {
        //public SQLiteConnection conn;
        BuildingDataInput sBuildInput;
        WindLoadDataInput sWindInput;

        // Table 3.1
        float fV_R_ULS; // m / s (ULS)
        float fV_R_SLS; // m / s (SLS)

        float fM_D = 1.0f; // Wind direction multiplier (CI 3.3) Table 3.2
        float fM_s = 1.0f; // Shielding multiplier (CI 4.3) Table 4.3
        float fM_t = 1.0f; // Topographic multiplier (CI 4.4)

        float fSiteTerrainSlope_Phi = 1/20; // 3 deg
        float fSiteTerrainSlope_Phi_deg = 3;

        float fM_z_cat; // Terrain multiplier // Database - TODO
        float fz_max; // m
        float fz;
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

            fz = sWindInput.fh; // Set height of building



            // Regional wind speed - AS/NZS 1170.2 Table 3.1
            fV_R_ULS = AS_NZS_1170_2.Eq_32_V_R__((int)sBuildInput.fR_ULS_Wind, sWindInput.eWindRegion); //m /s (ULS)
            fV_R_SLS = AS_NZS_1170_2.Eq_32_V_R__((int)sBuildInput.fR_SLS, sWindInput.eWindRegion);      //m /s (SLS)

            // fM_D =
            fM_z_cat = fz / sWindInput.fTerrainCategory;

            float fl_s = 1000f; // Average spacing of shielding buildings
            float fh_s = 0;     // Average roof height of shielding buildings
            float fb_s = 0f;    // Average breadth of shielding buildings, normal to the wind stream
            int in_s = 0;       // Number of upwind shielding buildings within a 45° sector of radius 20h and with hs >= z

            float fs_shielding = AS_NZS_1170_2.Eq_43_1____(fl_s, fh_s, fb_s, sWindInput.fh, in_s);
            // fM_s = // Database and interpolation

            // M_t
            bool bConsiderHillSlope = false;

            float fE_meters = 0;
            if (bConsiderHillSlope)
            {
                float fM_lee = 1.0f; // TODO load from database
                float fM_h = AS_NZS_1170_2.Get_Mh_v1__(false, 0, 0, 0, fz); // TODO - fill values
                fM_t = AS_NZS_1170_2.Eq_44_1____(fM_h, fM_lee, fE_meters);
            }

            // Site Wind speed
            float fV_sit_ULS = AS_NZS_1170_2.Eq_22______(fV_R_ULS, fM_D, fM_z_cat, fM_s, fM_t);
            float fV_sit_SLS = AS_NZS_1170_2.Eq_22______(fV_R_SLS, fM_D, fM_z_cat, fM_s, fM_t);

            float fq_ULS = 0.6f * MathF.Pow2(fV_sit_ULS); // Pa
            float fq_SLS = 0.6f * MathF.Pow2(fV_sit_ULS); // Pa

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

        // TODO - Ondrej - zapracovat pristup k databaze z tohto projektu
        // Presunut databazovy subor ??? a napojit DLL pre SQL

        // Table 3.2 - Wind Direction Factor

        /*
        protected void SetWindDirectionFactor()
        {
            // Connect to database
            using (conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["MainSQLiteDB"].ConnectionString))
            {
                conn.Open();
                SQLiteDataReader reader = null;

                string sTableName = "ASNZS1170_2_Tab3_2_WDM";

                SQLiteCommand command = new SQLiteCommand("Select * from " + sTableName + " where ID = '" + sWindInput.WindDirectionIndex + "'", conn);

                using (reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                       // 3.3.1 Regions A and W
                        fM_D = reader[sWindInput.sWindRegion].ToString();

                        // 3.3.2 - Regions B,C and D
                        if(sWindInput.eWindDirection == EWindDirection.eB ||
                        sWindInput.eWindDirection == EWindDirection.eC ||
                        sWindInput.eWindDirection == EWindDirection.eD)
                        fM_D = 0.95f;
                    }
                }

                reader.Close();
            }
        }

        protected void SetTerrainHeightMultiplier()
        {
        // Table 4.1
        // TODO
        }

     */
    }
}
