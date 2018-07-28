using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MATH;
using MATH.ARRAY;
using System.Data.SQLite;
using System.Configuration;
using BaseClasses;

namespace M_EC1.AS_NZS
{
    public class CCalcul_1170_2
    {
        public SQLiteConnection conn;
        BuildingDataInput sBuildInput;
        WindLoadDataInput sWindInput;

        // Table 3.1
        float fV_R_ULS; // m / s (ULS)
        float fV_R_SLS; // m / s (SLS)

        public float[] fM_D_array_angles_360;
        public float[] fM_D_array_values_360;
        public float[] fV_sit_ULS_Theta_360;
        public float[] fV_sit_SLS_Theta_360;

        public float[] fM_D_array_angles_9;
        public float[] fM_D_array_values_9;
        public float[] fV_sit_ULS_Theta_9;
        public float[] fV_sit_SLS_Theta_9;
        public string[] sWindDirections_9;

        float fs_shielding; // Shielding parameter Table 4.3 
        float fM_s = 1.0f;  // Shielding multiplier (Cl 4.3) Table 4.3
        float fM_t = 1.0f;  // Topographic multiplier (Cl 4.4)

        float fSiteTerrainSlope_Phi = 1/20; // 3 deg
        float fSiteTerrainSlope_Phi_deg = 3;

        float fM_z_cat; // Terrain multiplier
        float fz_max; // m
        float fz;
        float fRho_air = 1.2f; // kgm^3

        // Table 4.1
        // Terrain category


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

            // M_D
            fM_D_array_angles_9 = new float[9] { 0, 45, 90, 135, 180, 225, 270, 315, 360 };
            SetWindDirectionFactors_9_Items();

            // M_z_cat
            SetTerrainHeightMultiplier();

            // M_s
            float fl_s = 1000f;    // Average spacing of shielding buildings
            float fh_s = 0.1f;     // Average roof height of shielding buildings
            float fb_s = 0.1f;     // Average breadth of shielding buildings, normal to the wind stream
            int in_s = 1;          // Number of upwind shielding buildings within a 45° sector of radius 20h and with hs >= z
            fs_shielding = AS_NZS_1170_2.Eq_43_1____(fl_s, fh_s, fb_s, sWindInput.fh, in_s);
            SetShieldingMultiplier();

            // M_t
            bool bConsiderHillSlope = false;

            float fE_meters = 0;
            if (bConsiderHillSlope)
            {
                float fM_lee = 1.0f; // TODO load from database
                float fM_h = AS_NZS_1170_2.Get_Mh_v1__(false, 0, 0, 0, fz); // TODO - fill values
                fM_t = AS_NZS_1170_2.Eq_44_1____(fM_h, fM_lee, fE_meters);
            }

            // 2.2 Site wind speed
            fM_D_array_angles_360 = new float[360];
            fM_D_array_values_360 = new float[360];
            fV_sit_ULS_Theta_360 = new float[360];
            fV_sit_SLS_Theta_360 = new float[360];

            fV_sit_ULS_Theta_9 = new float[9];
            fV_sit_SLS_Theta_9 = new float[9];

            // Calculate value of V_sit for each angle (0-360 deg)
            int j = 0;

            for (int i = 0; i < 360; i++)
            {
                fM_D_array_angles_360[i] = i;
                // Wind direction multiplier (Cl. 3.3) Table 3.2
                fM_D_array_values_360[i] = ArrayF.GetLinearInterpolationValuePositive(i, fM_D_array_angles_9, fM_D_array_values_9);
                fV_sit_ULS_Theta_360[i] = AS_NZS_1170_2.Eq_22______(fV_R_ULS, fM_D_array_values_360[i], fM_z_cat, fM_s, fM_t);
                fV_sit_SLS_Theta_360[i] = AS_NZS_1170_2.Eq_22______(fV_R_SLS, fM_D_array_values_360[i], fM_z_cat, fM_s, fM_t);

                if (MathF.d_equal(fM_D_array_angles_9[j], fM_D_array_angles_360[i])) // Same angle
                {
                    fV_sit_ULS_Theta_9[j] = fV_sit_ULS_Theta_360[i];
                    fV_sit_SLS_Theta_9[j] = fV_sit_SLS_Theta_360[i];

                    j++;
                }
            }

            fV_sit_ULS_Theta_9[8] = fV_sit_ULS_Theta_9[0];
            fV_sit_SLS_Theta_9[8] = fV_sit_SLS_Theta_9[0];

            float fV_sit_ULS_Theta_0 = 0;
            float fV_sit_ULS_Theta_90 = 0;
            float fV_sit_ULS_Theta_180 = 0;
            float fV_sit_ULS_Theta_270 = 0;

            float fV_sit_SLS_Theta_0 = 0;
            float fV_sit_SLS_Theta_90 = 0;
            float fV_sit_SLS_Theta_180 = 0;
            float fV_sit_SLS_Theta_270 = 0;

            // 2.3 Design wind speed
            // TODO Martin
            // Ak by sme chceli navrhovat efektivnejsie je potrebne urobit prepocet rychlosti podla orientacie budovy voci svetovym stranam, vid obrazky 2.2 a 2.3 v norme
            // Temporary - zatial nastavujeme konzervativne

            float fV_des_ULS_Theta_0 = MathF.Max(30, fV_sit_ULS_Theta_0); // Minimum 30 m/s, see Note A1 in Cl. 2.3
            float fV_des_ULS_Theta_90 = MathF.Max(30, fV_sit_ULS_Theta_90);
            float fV_des_ULS_Theta_180 = MathF.Max(30, fV_sit_ULS_Theta_180);
            float fV_des_ULS_Theta_270 = MathF.Max(30, fV_sit_ULS_Theta_270);

            float fV_des_SLS_Theta_0 = fV_sit_SLS_Theta_0;
            float fV_des_SLS_Theta_90 = fV_sit_SLS_Theta_90;
            float fV_des_SLS_Theta_180 = fV_sit_SLS_Theta_180;
            float fV_des_SLS_Theta_270 = fV_sit_SLS_Theta_270;

            /*
            float fq_ULS = 0.6f * MathF.Pow2(fV_sit_ULS); // Pa
            float fq_SLS = 0.6f * MathF.Pow2(fV_sit_ULS); // Pa
            */

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


        // Table 3.2 - Wind Direction Factors
        protected void SetWindDirectionFactors_9_Items()
        {
            fM_D_array_values_9 = new float [9]; // 8 wind directions (N (0 deg), NE (45 deg), E (90 deg), SE (135 deg), S (180 deg), SW (225 deg), W (270 deg), NW (315 deg)) + 1 for 360 deg
            sWindDirections_9 = new string[9];

            // Connect to database
            using (conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["MainSQLiteDB"].ConnectionString))
            {
                conn.Open();
                SQLiteDataReader reader = null;
                string sTableName;
                SQLiteCommand command;

                // Set wind region string
                sTableName = "WindRegions";

                command = new SQLiteCommand("Select * from " + sTableName + " where ID = '" + (int)sWindInput.eWindRegion + "'", conn);

                string sWindRegion="";

                using (reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        sWindRegion = reader["windRegion"].ToString();
                    }
                }

                // Set wind direction factor
                sTableName = "ASNZS1170_2_Tab3_2_WDM";

                command = new SQLiteCommand("Select * from " + sTableName, conn);

                using (reader = command.ExecuteReader())
                {
                    int i = 0;
                    while (reader.Read())
                    {
                        // 3.3.1 Regions A and W
                        fM_D_array_values_9[i] = float.Parse(reader[sWindRegion].ToString());

                        // 3.3.2 - Regions B,C and D
                        if(sWindInput.eWindRegion == EWindRegion.eB ||
                           sWindInput.eWindRegion == EWindRegion.eC ||
                           sWindInput.eWindRegion == EWindRegion.eD)
                            fM_D_array_values_9[i] = 0.95f;

                        sWindDirections_9[i] = reader["cardinalDirection"].ToString();

                        i++;
                    }
                }

                reader.Close();
            }

            // Copy first item for Beta = 0 deg (Notrth) to the last item for Beta = 360 deg
            fM_D_array_values_9[8] = fM_D_array_values_9[0];
            sWindDirections_9[8] = sWindDirections_9[0];
        }

        protected void SetShieldingMultiplier()
        {
            // Table 4.3

            float[] Table_4_3_Column1 = new float[4];
            float[] Table_4_3_Column2 = new float[4];

            // Connect to database
            using (conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["MainSQLiteDB"].ConnectionString))
            {
                conn.Open();
                SQLiteDataReader reader = null;

                string sTableName = "ASNZS1170_2_Tab4_3_SM";

                SQLiteCommand command = new SQLiteCommand("Select * from " + sTableName, conn);

                using (reader = command.ExecuteReader())
                {
                    int i = 0;
                    while (reader.Read())
                    {
                        Table_4_3_Column1[i] = float.Parse(reader["shieldingParameter"].ToString());
                        Table_4_3_Column2[i] = float.Parse(reader["shieldingMultiplierMs"].ToString());
                        i++;
                    }
                }

                reader.Close();
            }

            // Interpolation
            fM_s = (float)ArrayF.GetLinearInterpolationValuePositive(fs_shielding, Table_4_3_Column1, Table_4_3_Column2);
        }

        protected void SetTerrainHeightMultiplier()
        {
            // Table 4.1

            float[,] Table_4_1 = new float[12,5];

            // Connect to database
            using (conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["MainSQLiteDB"].ConnectionString))
            {
                conn.Open();
                SQLiteDataReader reader = null;

                string sTableName = "ASNZS1170_2_Tab4_1_THM";

                SQLiteCommand command = new SQLiteCommand("Select * from " + sTableName , conn);

                using (reader = command.ExecuteReader())
                {
                    int i = 0;
                    while (reader.Read())
                    {
                        Table_4_1[i, 0] = float.Parse(reader["height_z_m"].ToString());
                        Table_4_1[i, 1] = float.Parse(reader["tercat_1_Mzcat"].ToString());
                        Table_4_1[i, 2] = float.Parse(reader["tercat_2_Mzcat"].ToString());
                        Table_4_1[i, 3] = float.Parse(reader["tercat_3_Mzcat"].ToString());
                        Table_4_1[i, 4] = float.Parse(reader["tercat_4_Mzcat"].ToString());
                        i++;
                    }
                }

                reader.Close();
            }

            // Bilinear Interpolation
            float[] arrayHeaderColumnValues = new float[] { 1, 2, 3, 4 }; // Terrain categories in table 4.1
            fM_z_cat = ArrayF.GetBilinearInterpolationValuePositive(Table_4_1, arrayHeaderColumnValues, fz, sWindInput.fTerrainCategory);
        }
    }
}
