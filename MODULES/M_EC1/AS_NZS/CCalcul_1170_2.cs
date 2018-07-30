using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MATH;
using MATH.ARRAY;
using System.Data.SQLite;
using System.Configuration;
using BaseClasses;
using System.Globalization;

namespace M_EC1.AS_NZS
{
    public class CCalcul_1170_2
    {
        public enum EFrictionalDragCoeff
        {
            eRibsAcross,
            eCorrugationAcross,
            eRibsParallel,
            eCorrugationParallel,
            eSmooth
        }

        public SQLiteConnection conn;
        BuildingDataInput sBuildInput;
        BuildingGeometryDataInput sGeometryInput;
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

        public float[] fV_sit_Theta_angles_8;
        public float[] fV_sit_ULS_Theta_4;
        public float[] fV_sit_SLS_Theta_4;

        public float[] fV_des_ULS_Theta_4;
        public float[] fV_des_SLS_Theta_4;

        public float[] fp_ULS_Theta_4;
        public float[] fp_SLS_Theta_4;

        float fs_shielding; // Shielding parameter Table 4.3 
        float fM_s = 1.0f;  // Shielding multiplier (Cl 4.3) Table 4.3
        float fM_t = 1.0f;  // Topographic multiplier (Cl 4.4)

        float fSiteTerrainSlope_Phi = 1/20; // 3 deg
        float fSiteTerrainSlope_Phi_deg = 3;

        float fM_z_cat; // Terrain multiplier
        float fz_max; // m
        float fz;
        float fRho_air = 1.2f; // kgm^3

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

        public CCalcul_1170_2(BuildingDataInput sBuildingData_temp, BuildingGeometryDataInput sGeometryData_temp, WindLoadDataInput sWindData_temp)
        {
            sBuildInput = sBuildingData_temp;
            sGeometryInput = sGeometryData_temp;
            sWindInput = sWindData_temp;

            fz = sGeometryInput.fH_2; // Set height of building

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
            fs_shielding = AS_NZS_1170_2.Eq_43_1____(fl_s, fh_s, fb_s, sGeometryInput.fH_2, in_s);
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

            fV_sit_ULS_Theta_4 = new float[4];
            fV_sit_SLS_Theta_4 = new float[4];

            for (int i = 0; i < fV_sit_ULS_Theta_4.Length; i++)
            {
                fV_sit_ULS_Theta_4[i] = 0;
                fV_sit_SLS_Theta_4[i] = 0;
            }

            fV_sit_Theta_angles_8 = new float[8]
            {sWindInput.iWindDirectionIndex + 000 - 45,
             sWindInput.iWindDirectionIndex + 000 + 45,
             sWindInput.iWindDirectionIndex + 090 - 45,
             sWindInput.iWindDirectionIndex + 090 + 45,
             sWindInput.iWindDirectionIndex + 180 - 45,
             sWindInput.iWindDirectionIndex + 180 + 45,
             sWindInput.iWindDirectionIndex + 270 - 45,
             sWindInput.iWindDirectionIndex + 270 + 45
            };

            SetDesignWindSpeedValue(sWindInput.iWindDirectionIndex + 000, fV_sit_ULS_Theta_360, ref fV_sit_ULS_Theta_4[0]); // 000
            SetDesignWindSpeedValue(sWindInput.iWindDirectionIndex + 090, fV_sit_ULS_Theta_360, ref fV_sit_ULS_Theta_4[1]); // 090
            SetDesignWindSpeedValue(sWindInput.iWindDirectionIndex + 180, fV_sit_ULS_Theta_360, ref fV_sit_ULS_Theta_4[2]); // 180
            SetDesignWindSpeedValue(sWindInput.iWindDirectionIndex + 270, fV_sit_ULS_Theta_360, ref fV_sit_ULS_Theta_4[3]); // 270

            SetDesignWindSpeedValue(sWindInput.iWindDirectionIndex + 000, fV_sit_SLS_Theta_360, ref fV_sit_SLS_Theta_4[0]); // 000
            SetDesignWindSpeedValue(sWindInput.iWindDirectionIndex + 090, fV_sit_SLS_Theta_360, ref fV_sit_SLS_Theta_4[1]); // 090
            SetDesignWindSpeedValue(sWindInput.iWindDirectionIndex + 180, fV_sit_SLS_Theta_360, ref fV_sit_SLS_Theta_4[2]); // 180
            SetDesignWindSpeedValue(sWindInput.iWindDirectionIndex + 270, fV_sit_SLS_Theta_360, ref fV_sit_SLS_Theta_4[3]); // 270

            // 2.3 Design wind speed
            fV_des_ULS_Theta_4 = new float[4];
            fV_des_SLS_Theta_4 = new float[4];

            // Minimum 30 m/s, see Note A1 in Cl. 2.3
            for (int i = 0; i < fV_des_ULS_Theta_4.Length; i++)
            {
                fV_des_ULS_Theta_4[i] = MathF.Max(30, fV_sit_ULS_Theta_4[i]);
                fV_des_SLS_Theta_4[i] = fV_sit_SLS_Theta_4[i];
            }

            /*
            float fq_ULS = 0.6f * MathF.Pow2(fV_sit_ULS); // Pa
            float fq_SLS = 0.6f * MathF.Pow2(fV_sit_ULS); // Pa
            */

            /*
            An ‘impermeable surface’ means a surface having a ratio of total open area to total surface area of less
            than 0.1%. A ‘permeable surface’ means a surface having a ratio of total open area, including leakage, to
            total surface area between 0.1% and 0.5%. Other surfaces with open areas greater than 0.5% are deemed
            to have ‘large openings’ and internal pressures shall be obtained from Table 5.1(B).
            */

            // TODO Martin - vypocty faktorov su zjednodusene, po vydani prvej verzie tomu venovat viac casu a prepracovat

            // Internal pressure
            float fC_pi_min = -0.2f;
            float fC_pi_max = 0.0f;

            // External pressure

            // Gable Roof (TODO - change for other roof shape, see Fig. 5.2)
            float fb; // Width perpendicular to the wind direction
            float fd; // Length in wind direction;
            float fh = 0.5f * (sGeometryInput.fH_2 - sGeometryInput.fH_1) + sGeometryInput.fH_1;

            float fRatioDtoB_Theta0or180 = sGeometryInput.fL / sGeometryInput.fW;
            float fRatioHtoD_Theta0or180 = fh / sGeometryInput.fL;

            float fRatioDtoB_Theta90or270 = sGeometryInput.fW / sGeometryInput.fL;
            float fRatioHtoD_Theta90or270 = fh / sGeometryInput.fW;

            // Table 5.2(A) - Walls external pressure coefficients (Cpe) for rectangular enclosed buildings - windward wall (W)
            bool bIsBuildingOnGround = true;
            bool bIsVariableWindSpeedinHeight = false;

            float fC_pe_W_wall;

            if (fh > 25 || (fh <= 25f && bIsVariableWindSpeedinHeight) || !bIsBuildingOnGround)
                fC_pe_W_wall = 0.8f;
            else
                fC_pe_W_wall = 0.7f;

            // Table 5.2(B) - Walls external pressure coefficients (Cpe) for rectangular enclosed buildings - leeward wall (L)
            float fC_pe_L_wall_Theta0or180;
            float fC_pe_L_wall_Theta90or270;

            float[] fx;
            float[] fy;

            // 0 deg
            if (sGeometryInput.fRoofPitch_deg < 10)
            {
                fx = new float[5] { 0, 1, 2, 4, 9999 };
                fy = new float[5] { -0.5f, -0.5f, -0.3f, -0.2f, -0.2f };
            }
            else if (sGeometryInput.fRoofPitch_deg < 25)
            {
                fx = new float[4] { 10, 15, 20, 25 };
                fy = new float[4] { -0.3f, -0.3f, -0.4f, -0.5f }; // - 0.5 for 25 deg ???
            }
            else
            {
                fx = new float[4] { 0, 0.1f, 0.3f, 9999 };
                fy = new float[4] { -0.75f, -0.75f, -0.5f, -0.5f };
            }

            fC_pe_L_wall_Theta0or180 = ArrayF.GetLinearInterpolationValuePositive(fRatioDtoB_Theta0or180, fx, fy);

            // 90 deg
            fx = new float[5] { 0, 1, 2, 4, 9999 };
            fy = new float[5] { -0.5f, -0.5f, -0.3f, -0.2f, -0.2f };
            fC_pe_L_wall_Theta90or270 = ArrayF.GetLinearInterpolationValuePositive(fRatioDtoB_Theta90or270, fx, fy);

            // Table 5.2(C) - Walls external pressure coefficients (Cpe) for rectangular enclosed buildings - side walls (S)
            float[] fC_pe_S_wall_dimensions = new float[5] {0, fh, 2 * fh, 3 * fh, 9999 };
            float[] fC_pe_S_wall_values = new float[5] { -0.65f, -0.65f, -0.5f, -0.3f, -0.2f };

            // Roof
            float[] fC_pe_U_roof_dimensions;
            float[] fC_pe_U_roof_values_min;
            float[] fC_pe_U_roof_values_max;

            float[] fC_pe_D_roof_dimensions;
            float[] fC_pe_D_roof_values_min = new float[1]; // TODO - odtranit a alokovat podla potrebnej velkosti
            float[] fC_pe_D_roof_values_max = new float[1]; // TODO - odtranit a alokovat podla potrebnej velkosti

            float[] fC_pe_R_roof_dimensions;
            float[] fC_pe_R_roof_values_min;
            float[] fC_pe_R_roof_values_max;

            if(sGeometryInput.fRoofPitch_deg < 10) // Table 5.3(A)
            {
                // TODO Martin - Interpolation shall only be carried out on values of the same sign ???? To bude neprijemne !!!!

                // Table 5.3(A) - Roofs - external pressure coefficients (Cpe) for rectangular enclosed buildings - for upwind slope (U), and downwind slope (D) and (R) for gable roofs, for Alpha < 10°
                float[] fC_pe_UD_roof_dimensions = new float[6];
                float[] fC_pe_UD_roof_values_min = new float[6];
                float[] fC_pe_UD_roof_values_max = new float[6];

                Calculate_Cpe_Table_5_3_A(fh, fRatioHtoD_Theta0or180, ref fC_pe_UD_roof_dimensions, ref fC_pe_UD_roof_values_min, ref fC_pe_UD_roof_values_max);

                fC_pe_U_roof_dimensions = fC_pe_UD_roof_dimensions;
                fC_pe_U_roof_values_min = fC_pe_UD_roof_values_min;
                fC_pe_U_roof_values_max = fC_pe_UD_roof_values_max;

                for (int i = 0; i < fC_pe_UD_roof_dimensions.Length; i++)
                {
                    // Find dimension corresponding to the half of building width (change of gable roof slope from U to D)
                    // and set factor for U
                    if (fC_pe_UD_roof_dimensions[i] < (fC_pe_UD_roof_dimensions.Length - 1) && fC_pe_UD_roof_dimensions[i+1] >= 0.5f * sGeometryInput.fW)
                    {
                        fC_pe_D_roof_dimensions = new float[fC_pe_UD_roof_dimensions.Length - i];
                        fC_pe_D_roof_values_min = new float[fC_pe_UD_roof_values_min.Length - i];
                        fC_pe_D_roof_values_max = new float[fC_pe_UD_roof_values_max.Length - i];

                        for (int k = i; k < fC_pe_UD_roof_dimensions.Length - i; k++)
                        {
                            fC_pe_D_roof_dimensions[k] = fC_pe_UD_roof_dimensions[i + k];
                            fC_pe_D_roof_values_min[k] = fC_pe_UD_roof_values_min[i + k];
                            fC_pe_D_roof_values_max[k] = fC_pe_UD_roof_values_max[i + k];
                        }
                        
                    }

                    break; // Do not continue in cycle
                }
            }
            else // More or equal to 10°
            {
                fC_pe_U_roof_dimensions = new float[1] { sGeometryInput.fW };
                fC_pe_U_roof_values_min = new float[1];
                fC_pe_U_roof_values_max = new float[1];

                fC_pe_D_roof_dimensions = new float[1] { sGeometryInput.fW };
                fC_pe_D_roof_values_min = new float[1];
                fC_pe_D_roof_values_max = new float[1];

                // Table 5.3(B) - Roofs - external pressure coefficients (Cpe) for rectangular enclosed buildings - for upwind slope (U) Alpha >= 10°
                Calculate_Cpe_Table_5_3_B(fh, sGeometryInput.fRoofPitch_deg, fRatioHtoD_Theta0or180, ref fC_pe_U_roof_values_min, ref fC_pe_U_roof_values_max);

                // Table 5.3(C) - Roofs - external pressure coefficients (Cpe) for rectangular enclosed buildings - downhill slope (D), and (R) for hip roofs, for Alpha >= 10°
                Calculate_Cpe_Table_5_3_C(fh, sGeometryInput.fRoofPitch_deg, fRatioHtoD_Theta0or180, fRatioDtoB_Theta0or180, ref fC_pe_D_roof_values_min, ref fC_pe_D_roof_values_max);
            }

            // Wind 90 or 270 deg
            fC_pe_R_roof_dimensions = new float[6];
            fC_pe_R_roof_values_min = new float[6];
            fC_pe_R_roof_values_max = new float[6];
            Calculate_Cpe_Table_5_3_A(fh, fRatioHtoD_Theta90or270, ref fC_pe_R_roof_dimensions, ref fC_pe_R_roof_values_min, ref fC_pe_R_roof_values_max);

            // 5.4.2 Area reduction factor(Ka) for roofs and side walls
            float fRoofArea = sGeometryInput.fW / (float)Math.Cos(sGeometryInput.fRoofPitch_deg / 180 * Math.PI) * sGeometryInput.fL;
            float fWallArea_0or180 = sGeometryInput.fH_1 * sGeometryInput.fL;
            float fWallArea_90or270 = sGeometryInput.fH_1 * sGeometryInput.fW + 0.5f * (sGeometryInput.fH_2 - sGeometryInput.fH_1) * sGeometryInput.fW; // Gable Roof

            fx = new float[5] { 0, 10, 25, 100, 9999 };
            fy = new float[5] { 1.0f, 1.0f, 0.9f, 0.8f, 0.8f };

            float fK_a_roof = ArrayF.GetLinearInterpolationValuePositive(fRoofArea, fx, fy);
            float fK_a_wall_0or180 = ArrayF.GetLinearInterpolationValuePositive(fWallArea_0or180, fx, fy);
            float fK_a_wall_90or270 = ArrayF.GetLinearInterpolationValuePositive(fWallArea_90or270, fx, fy);

            // 5.4.4 Local pressure factor(K l) for cladding
            float fK_l = 1f; // K_l - local pressure factor, as given in Paragraph D1.3
            // Table 5.7 - reduction factor(Kr) due to parapets
            // 5.4.5 Permeable cladding reduction factor(Kp) for roofs and side walls
            float fK_p = 1f; // K_p - net porosity factor, as given in Paragraph D1.4

            float fK_ce = 0.8f; // TODO - dopracovat podla kombinacii external and internal pressure
            float fK_ci = 1.0f; // TODO - dopracovat podla kombinacii external and internal pressure

            // Aerodynamic shape factor (C fig)
            // Internal and external pressure factors

            // Internal presssure
            float fC_fig_i_min = AS_NZS_1170_2.Eq_52_1____(fC_pi_min, fK_ci); // Aerodynamic shape factor
            float fC_fig_i_max = AS_NZS_1170_2.Eq_52_1____(fC_pi_max, fK_ci); // Aerodynamic shape factor

            // External pressure
            // Walls

            float fC_fig_e_W_wall_0or180 = AS_NZS_1170_2.Eq_52_2____(fC_pe_W_wall, fK_a_wall_0or180, fK_ce, fK_l, fK_p); // Aerodynamic shape factor
            float fC_fig_e_W_wall_90or270 = AS_NZS_1170_2.Eq_52_2____(fC_pe_W_wall, fK_a_wall_90or270, fK_ce, fK_l, fK_p); // Aerodynamic shape factor

            float fC_fig_e_L_wall_0or180 = AS_NZS_1170_2.Eq_52_2____(fC_pe_L_wall_Theta0or180, fK_a_wall_0or180, fK_ce, fK_l, fK_p); // Aerodynamic shape factor
            float fC_fig_e_L_wall_90or270 = AS_NZS_1170_2.Eq_52_2____(fC_pe_L_wall_Theta90or270, fK_a_wall_90or270, fK_ce, fK_l, fK_p); // Aerodynamic shape factor

            float[] fC_fig_e_S_wall_0or180 = new float[fC_pe_S_wall_values.Length];

            for (int i = 0; i < fC_fig_e_S_wall_0or180.Length; i++)
                fC_fig_e_S_wall_0or180[i] = AS_NZS_1170_2.Eq_52_2____(fC_pe_S_wall_values[i], fK_a_wall_90or270, fK_ce, fK_l, fK_p); // Aerodynamic shape factor

            float[] fC_fig_e_S_wall_90or270 = new float[fC_pe_S_wall_values.Length];

            for(int i = 0; i< fC_fig_e_S_wall_90or270.Length; i++)
                fC_fig_e_S_wall_90or270[i] = AS_NZS_1170_2.Eq_52_2____(fC_pe_S_wall_values[i], fK_a_wall_0or180, fK_ce, fK_l, fK_p); // Aerodynamic shape factor

            // Roof
            float[] fC_fig_e_U_roof_values_min = new float[fC_pe_U_roof_values_min.Length];
            float[] fC_fig_e_U_roof_values_max = new float[fC_pe_U_roof_values_max.Length];

            for (int i = 0; i < fC_fig_e_U_roof_values_min.Length; i++)
            {
                fC_fig_e_U_roof_values_min[i] = AS_NZS_1170_2.Eq_52_2____(fC_pe_U_roof_values_min[i], fK_a_roof, fK_ce, fK_l, fK_p); // Aerodynamic shape factor
                fC_fig_e_U_roof_values_max[i] = AS_NZS_1170_2.Eq_52_2____(fC_pe_U_roof_values_max[i], fK_a_roof, fK_ce, fK_l, fK_p); // Aerodynamic shape factor
            }

            float[] fC_fig_e_D_roof_values_min = new float[fC_pe_D_roof_values_min.Length];
            float[] fC_fig_e_D_roof_values_max = new float[fC_pe_D_roof_values_max.Length];

            for (int i = 0; i < fC_fig_e_D_roof_values_min.Length; i++)
            {
                fC_fig_e_D_roof_values_min[i] = AS_NZS_1170_2.Eq_52_2____(fC_pe_D_roof_values_min[i], fK_a_roof, fK_ce, fK_l, fK_p); // Aerodynamic shape factor
                fC_fig_e_D_roof_values_max[i] = AS_NZS_1170_2.Eq_52_2____(fC_pe_D_roof_values_max[i], fK_a_roof, fK_ce, fK_l, fK_p); // Aerodynamic shape factor
            }

            float[] fC_fig_e_R_roof_values_min = new float[fC_pe_R_roof_values_min.Length];
            float[] fC_fig_e_R_roof_values_max = new float[fC_pe_R_roof_values_max.Length];

            for (int i = 0; i < fC_fig_e_R_roof_values_min.Length; i++)
            {
                fC_fig_e_R_roof_values_min[i] = AS_NZS_1170_2.Eq_52_2____(fC_pe_R_roof_values_min[i], fK_a_roof, fK_ce, fK_l, fK_p); // Aerodynamic shape factor
                fC_fig_e_R_roof_values_max[i] = AS_NZS_1170_2.Eq_52_2____(fC_pe_R_roof_values_max[i], fK_a_roof, fK_ce, fK_l, fK_p); // Aerodynamic shape factor
            }

            float fC_dyn = 1.0f; // Dynamic response factor

            // Surface pressures
            // Internal presssure

            float[] fp_i_min_ULS_Theta_4 = new float[4];
            float[] fp_i_min_SLS_Theta_4 = new float[4];

            for (int i = 0; i < fp_i_min_ULS_Theta_4.Length; i++)
            {
                fp_i_min_ULS_Theta_4[i] = AS_NZS_1170_2.Eq_24_1____(fRho_air, fV_des_ULS_Theta_4[i], fC_fig_i_min, fC_dyn);
                fp_i_min_SLS_Theta_4[i] = AS_NZS_1170_2.Eq_24_1____(fRho_air, fV_des_SLS_Theta_4[i], fC_fig_i_min, fC_dyn);
            }

            float[] fp_i_max_ULS_Theta_4 = new float[4];
            float[] fp_i_max_SLS_Theta_4 = new float[4];

            for (int i = 0; i < fp_i_max_ULS_Theta_4.Length; i++)
            {
                fp_i_max_ULS_Theta_4[i] = AS_NZS_1170_2.Eq_24_1____(fRho_air, fV_des_ULS_Theta_4[i], fC_fig_i_min, fC_dyn);
                fp_i_max_SLS_Theta_4[i] = AS_NZS_1170_2.Eq_24_1____(fRho_air, fV_des_SLS_Theta_4[i], fC_fig_i_min, fC_dyn);
            }

            // External pressure
            // Walls

            float[] fp_e_W_wall_ULS_Theta_4 = new float[4];
            float[] fp_e_W_wall_SLS_Theta_4 = new float[4];

            fp_e_W_wall_ULS_Theta_4[0] = AS_NZS_1170_2.Eq_24_1____(fRho_air, fV_des_ULS_Theta_4[0], fC_fig_e_W_wall_0or180, fC_dyn);
            fp_e_W_wall_ULS_Theta_4[1] = AS_NZS_1170_2.Eq_24_1____(fRho_air, fV_des_ULS_Theta_4[1], fC_fig_e_W_wall_90or270, fC_dyn);
            fp_e_W_wall_ULS_Theta_4[2] = AS_NZS_1170_2.Eq_24_1____(fRho_air, fV_des_ULS_Theta_4[2], fC_fig_e_W_wall_0or180, fC_dyn);
            fp_e_W_wall_ULS_Theta_4[3] = AS_NZS_1170_2.Eq_24_1____(fRho_air, fV_des_ULS_Theta_4[3], fC_fig_e_W_wall_90or270, fC_dyn);

            fp_e_W_wall_SLS_Theta_4[0] = AS_NZS_1170_2.Eq_24_1____(fRho_air, fV_des_SLS_Theta_4[0], fC_fig_e_W_wall_0or180, fC_dyn);
            fp_e_W_wall_SLS_Theta_4[1] = AS_NZS_1170_2.Eq_24_1____(fRho_air, fV_des_SLS_Theta_4[1], fC_fig_e_W_wall_90or270, fC_dyn);
            fp_e_W_wall_SLS_Theta_4[2] = AS_NZS_1170_2.Eq_24_1____(fRho_air, fV_des_SLS_Theta_4[2], fC_fig_e_W_wall_0or180, fC_dyn);
            fp_e_W_wall_SLS_Theta_4[3] = AS_NZS_1170_2.Eq_24_1____(fRho_air, fV_des_SLS_Theta_4[3], fC_fig_e_W_wall_90or270, fC_dyn);

            float[] fp_e_L_wall_ULS_Theta_4 = new float[4];
            float[] fp_e_L_wall_SLS_Theta_4 = new float[4];

            fp_e_L_wall_ULS_Theta_4[0] = AS_NZS_1170_2.Eq_24_1____(fRho_air, fV_des_ULS_Theta_4[0], fC_fig_e_L_wall_0or180, fC_dyn);
            fp_e_L_wall_ULS_Theta_4[1] = AS_NZS_1170_2.Eq_24_1____(fRho_air, fV_des_ULS_Theta_4[1], fC_fig_e_L_wall_90or270, fC_dyn);
            fp_e_L_wall_ULS_Theta_4[2] = AS_NZS_1170_2.Eq_24_1____(fRho_air, fV_des_ULS_Theta_4[2], fC_fig_e_L_wall_0or180, fC_dyn);
            fp_e_L_wall_ULS_Theta_4[3] = AS_NZS_1170_2.Eq_24_1____(fRho_air, fV_des_ULS_Theta_4[3], fC_fig_e_L_wall_90or270, fC_dyn);

            fp_e_L_wall_SLS_Theta_4[0] = AS_NZS_1170_2.Eq_24_1____(fRho_air, fV_des_SLS_Theta_4[0], fC_fig_e_L_wall_0or180, fC_dyn);
            fp_e_L_wall_SLS_Theta_4[1] = AS_NZS_1170_2.Eq_24_1____(fRho_air, fV_des_SLS_Theta_4[1], fC_fig_e_L_wall_90or270, fC_dyn);
            fp_e_L_wall_SLS_Theta_4[2] = AS_NZS_1170_2.Eq_24_1____(fRho_air, fV_des_SLS_Theta_4[2], fC_fig_e_L_wall_0or180, fC_dyn);
            fp_e_L_wall_SLS_Theta_4[3] = AS_NZS_1170_2.Eq_24_1____(fRho_air, fV_des_SLS_Theta_4[3], fC_fig_e_L_wall_90or270, fC_dyn);

            float[,] fp_e_S_wall_ULS_Theta_4 = new float[4, fC_fig_e_S_wall_0or180.Length];
            float[,] fp_e_S_wall_SLS_Theta_4 = new float[4, fC_fig_e_S_wall_0or180.Length];

            for (int i = 0; i < 4; i++)
            {
                for (int k = 0; k < fC_fig_e_S_wall_0or180.Length; k++)
                {
                    if (i == 0 || i == 2)
                    {
                        fp_e_S_wall_ULS_Theta_4[i, k] = AS_NZS_1170_2.Eq_24_1____(fRho_air, fV_des_ULS_Theta_4[i], fC_fig_e_S_wall_0or180[k], fC_dyn);
                        fp_e_S_wall_SLS_Theta_4[i, k] = AS_NZS_1170_2.Eq_24_1____(fRho_air, fV_des_SLS_Theta_4[i], fC_fig_e_S_wall_0or180[k], fC_dyn);
                    }
                    else
                    {
                        fp_e_S_wall_ULS_Theta_4[i, k] = AS_NZS_1170_2.Eq_24_1____(fRho_air, fV_des_ULS_Theta_4[i], fC_fig_e_S_wall_90or270[k], fC_dyn);
                        fp_e_S_wall_SLS_Theta_4[i, k] = AS_NZS_1170_2.Eq_24_1____(fRho_air, fV_des_SLS_Theta_4[i], fC_fig_e_S_wall_90or270[k], fC_dyn);
                    }
                }
            }

            // Roof
            float[,] fp_e_min_U_roof_ULS_Theta_4 = new float[4, fC_fig_e_U_roof_values_min.Length];
            float[,] fp_e_min_U_roof_SLS_Theta_4 = new float[4, fC_fig_e_U_roof_values_min.Length];

            float[,] fp_e_max_U_roof_ULS_Theta_4 = new float[4, fC_fig_e_U_roof_values_max.Length];
            float[,] fp_e_max_U_roof_SLS_Theta_4 = new float[4, fC_fig_e_U_roof_values_max.Length];

            for (int i = 0; i < 4; i++)
            {
                for (int k = 0; k < fC_fig_e_U_roof_values_min.Length; k++)
                {
                    fp_e_min_U_roof_ULS_Theta_4[i, k] = AS_NZS_1170_2.Eq_24_1____(fRho_air, fV_des_ULS_Theta_4[i], fC_fig_e_U_roof_values_min[k], fC_dyn);
                    fp_e_min_U_roof_SLS_Theta_4[i, k] = AS_NZS_1170_2.Eq_24_1____(fRho_air, fV_des_SLS_Theta_4[i], fC_fig_e_U_roof_values_min[k], fC_dyn);
                }

                for (int k = 0; k < fC_fig_e_U_roof_values_max.Length; k++)
                {
                    fp_e_max_U_roof_ULS_Theta_4[i, k] = AS_NZS_1170_2.Eq_24_1____(fRho_air, fV_des_ULS_Theta_4[i], fC_fig_e_U_roof_values_max[k], fC_dyn);
                    fp_e_max_U_roof_SLS_Theta_4[i, k] = AS_NZS_1170_2.Eq_24_1____(fRho_air, fV_des_SLS_Theta_4[i], fC_fig_e_U_roof_values_max[k], fC_dyn);
                }
            }

            float[,] fp_e_min_D_roof_ULS_Theta_4 = new float[4, fC_fig_e_D_roof_values_min.Length];
            float[,] fp_e_min_D_roof_SLS_Theta_4 = new float[4, fC_fig_e_D_roof_values_min.Length];

            float[,] fp_e_max_D_roof_ULS_Theta_4 = new float[4, fC_fig_e_D_roof_values_max.Length];
            float[,] fp_e_max_D_roof_SLS_Theta_4 = new float[4, fC_fig_e_D_roof_values_max.Length];

            for (int i = 0; i < 4; i++)
            {
                for (int k = 0; k < fC_fig_e_D_roof_values_min.Length; k++)
                {
                    fp_e_min_D_roof_ULS_Theta_4[i, k] = AS_NZS_1170_2.Eq_24_1____(fRho_air, fV_des_ULS_Theta_4[i], fC_fig_e_D_roof_values_min[k], fC_dyn);
                    fp_e_min_D_roof_SLS_Theta_4[i, k] = AS_NZS_1170_2.Eq_24_1____(fRho_air, fV_des_SLS_Theta_4[i], fC_fig_e_D_roof_values_min[k], fC_dyn);
                }

                for (int k = 0; k < fC_fig_e_D_roof_values_max.Length; k++)
                {
                    fp_e_max_D_roof_ULS_Theta_4[i, k] = AS_NZS_1170_2.Eq_24_1____(fRho_air, fV_des_ULS_Theta_4[i], fC_fig_e_D_roof_values_max[k], fC_dyn);
                    fp_e_max_D_roof_SLS_Theta_4[i, k] = AS_NZS_1170_2.Eq_24_1____(fRho_air, fV_des_SLS_Theta_4[i], fC_fig_e_D_roof_values_max[k], fC_dyn);
                }
            }

            float[,] fp_e_min_R_roof_ULS_Theta_4 = new float[4, fC_fig_e_R_roof_values_min.Length];
            float[,] fp_e_min_R_roof_SLS_Theta_4 = new float[4, fC_fig_e_R_roof_values_min.Length];

            float[,] fp_e_max_R_roof_ULS_Theta_4 = new float[4, fC_fig_e_R_roof_values_max.Length];
            float[,] fp_e_max_R_roof_SLS_Theta_4 = new float[4, fC_fig_e_R_roof_values_max.Length];

            for (int i = 0; i < 4; i++)
            {
                for (int k = 0; k < fC_fig_e_R_roof_values_min.Length; k++)
                {
                    fp_e_min_R_roof_ULS_Theta_4[i, k] = AS_NZS_1170_2.Eq_24_1____(fRho_air, fV_des_ULS_Theta_4[i], fC_fig_e_R_roof_values_min[k], fC_dyn);
                    fp_e_min_R_roof_SLS_Theta_4[i, k] = AS_NZS_1170_2.Eq_24_1____(fRho_air, fV_des_SLS_Theta_4[i], fC_fig_e_R_roof_values_min[k], fC_dyn);
                }

                for (int k = 0; k < fC_fig_e_R_roof_values_max.Length; k++)
                {
                    fp_e_max_R_roof_ULS_Theta_4[i, k] = AS_NZS_1170_2.Eq_24_1____(fRho_air, fV_des_ULS_Theta_4[i], fC_fig_e_R_roof_values_max[k], fC_dyn);
                    fp_e_max_R_roof_SLS_Theta_4[i, k] = AS_NZS_1170_2.Eq_24_1____(fRho_air, fV_des_SLS_Theta_4[i], fC_fig_e_R_roof_values_max[k], fC_dyn);
                }
            }

            // 5.5 Frictional drag forces for enclosed buildings
            float fRatioDtoH_Theta0or180 = 1.0f / fRatioHtoD_Theta0or180;
            float fRatioDtoH_Theta90or270 = 1.0f / fRatioHtoD_Theta90or270;

            float fC_f_Theta0or180 = 0.0f;
            float fC_f_Theta90or270 = 0.0f;
            float fArea_Theta0or180 = 0.0f;
            float fArea_Theta90or270 = 0.0f;

            EFrictionalDragCoeff eSurfaceDescription = EFrictionalDragCoeff.eRibsAcross; // Todo nastavit podle typu cladding a roofing (plechy)

            // 0 or 180 deg
            if (fRatioDtoH_Theta0or180 > 4 || fRatioDtoB_Theta0or180 > 4)
            {
                fb = sGeometryInput.fL;
                fd = sGeometryInput.fW;

                float fx_length = MathF.Max(fd, MathF.Min(4 * fh, 4 * fb));

                fArea_Theta0or180 = GetArea_Table_5_9(fh, fb, fd);

                fC_f_Theta0or180 = GetFrictionalDragCoefficient_Table_5_9(fx_length, fh, fb, eSurfaceDescription);
            }

            if (fRatioDtoH_Theta90or270 > 4 || fRatioDtoB_Theta90or270 > 4)
            {
                fb = sGeometryInput.fW;
                fd = sGeometryInput.fL;

                float fx_length = MathF.Max(fd, MathF.Min(4 * fh, 4 * fb));

                fArea_Theta90or270 = GetArea_Table_5_9(fh, fb, fd);

                fC_f_Theta90or270 = GetFrictionalDragCoefficient_Table_5_9(fx_length, fh, sGeometryInput.fW, eSurfaceDescription);
            }

            float fK_c = 1.0f; // TODO - dopracovat podla kombinacii external and internal pressure

            float fC_fig_wall_Theta0or180 = AS_NZS_1170_2.Eq_52_3____(fC_f_Theta0or180, fK_a_wall_90or270, fK_c);
            float fC_fig_wall_Theta90or270 = AS_NZS_1170_2.Eq_52_3____(fC_f_Theta90or270, fK_a_wall_0or180, fK_c);
            float fC_fig_roof_Theta0or180 = AS_NZS_1170_2.Eq_52_3____(fC_f_Theta0or180, fK_a_roof, fK_c);
            float fC_fig_roof_Theta90or270 = AS_NZS_1170_2.Eq_52_3____(fC_f_Theta90or270, fK_a_roof, fK_c);

            float[] fp_e_drag_wall_ULS_Theta_4 = new float[4];
            float[] fp_e_drag_wall_SLS_Theta_4 = new float[4];

            fp_e_drag_wall_ULS_Theta_4[0] = AS_NZS_1170_2.Eq_24_1____(fRho_air, fV_des_ULS_Theta_4[0], fC_fig_wall_Theta0or180, fC_dyn);
            fp_e_drag_wall_ULS_Theta_4[1] = AS_NZS_1170_2.Eq_24_1____(fRho_air, fV_des_ULS_Theta_4[1], fC_fig_wall_Theta90or270, fC_dyn);
            fp_e_drag_wall_ULS_Theta_4[2] = AS_NZS_1170_2.Eq_24_1____(fRho_air, fV_des_ULS_Theta_4[2], fC_fig_wall_Theta0or180, fC_dyn);
            fp_e_drag_wall_ULS_Theta_4[3] = AS_NZS_1170_2.Eq_24_1____(fRho_air, fV_des_ULS_Theta_4[3], fC_fig_wall_Theta90or270, fC_dyn);

            fp_e_drag_wall_SLS_Theta_4[0] = AS_NZS_1170_2.Eq_24_1____(fRho_air, fV_des_SLS_Theta_4[0], fC_fig_wall_Theta0or180, fC_dyn);
            fp_e_drag_wall_SLS_Theta_4[1] = AS_NZS_1170_2.Eq_24_1____(fRho_air, fV_des_SLS_Theta_4[1], fC_fig_wall_Theta90or270, fC_dyn);
            fp_e_drag_wall_SLS_Theta_4[2] = AS_NZS_1170_2.Eq_24_1____(fRho_air, fV_des_SLS_Theta_4[2], fC_fig_wall_Theta0or180, fC_dyn);
            fp_e_drag_wall_SLS_Theta_4[3] = AS_NZS_1170_2.Eq_24_1____(fRho_air, fV_des_SLS_Theta_4[3], fC_fig_wall_Theta90or270, fC_dyn);

            float[] fp_e_drag_roof_ULS_Theta_4 = new float[4];
            float[] fp_e_drag_roof_SLS_Theta_4 = new float[4];

            fp_e_drag_roof_ULS_Theta_4[0] = AS_NZS_1170_2.Eq_24_1____(fRho_air, fV_des_ULS_Theta_4[0], fC_fig_roof_Theta0or180, fC_dyn);
            fp_e_drag_roof_ULS_Theta_4[1] = AS_NZS_1170_2.Eq_24_1____(fRho_air, fV_des_ULS_Theta_4[1], fC_fig_roof_Theta90or270, fC_dyn);
            fp_e_drag_roof_ULS_Theta_4[2] = AS_NZS_1170_2.Eq_24_1____(fRho_air, fV_des_ULS_Theta_4[2], fC_fig_roof_Theta0or180, fC_dyn);
            fp_e_drag_roof_ULS_Theta_4[3] = AS_NZS_1170_2.Eq_24_1____(fRho_air, fV_des_ULS_Theta_4[3], fC_fig_roof_Theta90or270, fC_dyn);

            fp_e_drag_roof_SLS_Theta_4[0] = AS_NZS_1170_2.Eq_24_1____(fRho_air, fV_des_SLS_Theta_4[0], fC_fig_roof_Theta0or180, fC_dyn);
            fp_e_drag_roof_SLS_Theta_4[1] = AS_NZS_1170_2.Eq_24_1____(fRho_air, fV_des_SLS_Theta_4[1], fC_fig_roof_Theta90or270, fC_dyn);
            fp_e_drag_roof_SLS_Theta_4[2] = AS_NZS_1170_2.Eq_24_1____(fRho_air, fV_des_SLS_Theta_4[2], fC_fig_roof_Theta0or180, fC_dyn);
            fp_e_drag_roof_SLS_Theta_4[3] = AS_NZS_1170_2.Eq_24_1____(fRho_air, fV_des_SLS_Theta_4[3], fC_fig_roof_Theta90or270, fC_dyn);
        }

        protected void Calculate_Cpe_Table_5_3_A(float fh, float fRatioHtoD, ref float []fC_pe_roof_dimensions, ref float []fC_pe_roof_values_min, ref float []fC_pe_roof_values_max)
        {
            float[] fx = new float[6] { 0, 0.5f * fh, fh, 2 * fh, 3 * fh, 9999 };
            float[] fy_min_htod_05 = new float[6] { -0.9f, -0.9f, -0.9f, -0.5f, -0.3f, -0.2f };
            float[] fy_min_htod_10 = new float[6] { -1.3f, -1.3f, -0.7f, -0.7f, -0.7f, -0.7f }; //???

            float[] fy_max_htod_05 = new float[6] { -0.4f, -0.4f, -0.4f, -0.0f, 0.1f, 0.2f };
            float[] fy_max_htod_10 = new float[6] { -0.6f, -0.6f, -0.3f, -0.3f, -0.3f, -0.3f }; //???

            if (fRatioHtoD <= 0.5f)
            {
                fC_pe_roof_dimensions = fx;
                fC_pe_roof_values_min = fy_min_htod_05;
                fC_pe_roof_values_max = fy_max_htod_05;
            }
            else if (fRatioHtoD < 1.0f)
            {
                for (int i = 0; i < 6; i++)
                {
                    fC_pe_roof_dimensions[i] = fx[i];

                    // Interpolate Cpe min
                    fC_pe_roof_values_min[i] = ArrayF.GetLinearInterpolationValuePositive(0.5f, 1.0f, fy_min_htod_05[i], fy_min_htod_10[i], fRatioHtoD);

                    // Interpolate Cpe max
                    fC_pe_roof_values_max[i] = ArrayF.GetLinearInterpolationValuePositive(0.5f, 1.0f, fy_max_htod_05[i], fy_max_htod_10[i], fRatioHtoD);
                }
            }
            else
            {
                fC_pe_roof_dimensions = fx;
                fC_pe_roof_values_min = fy_min_htod_10;
                fC_pe_roof_values_max = fy_max_htod_10;
            }
        }

        protected void Calculate_Cpe_Table_5_3_B(float fh, float fAlpha_deg, float fRatioHtoD, ref float[] fC_pe_roof_values_min, ref float[] fC_pe_roof_values_max)
        {
            float[] fx = new float[8] { 10, 15, 20, 25, 30, 35, 45, 9999 };
            float[] fy_min_htod_025 = new float[8] { -0.7f, -0.5f, -0.3f, -0.2f, -0.2f,  0.0f, 0, 0};
            float[] fy_min_htod_050 = new float[8] { -0.9f, -0.9f, -0.4f, -0.3f, -0.2f, -0.2f, 0, 0};
            float[] fy_min_htod_100 = new float[8] { -1.3f, -1.3f, -0.7f, -0.5f, -0.3f, -0.2f, 0, 0}; //???

            float[] fy_max_htod_025 = new float[8] { -0.3f,  0.0f,  0.2f,  0.3f,  0.4f,  0.5f, 0.57f ,0.57f};
            float[] fy_max_htod_050 = new float[8] { -0.4f, -0.3f,  0.0f,  0.2f,  0.3f,  0.4f, 0.57f, 0.57f};
            float[] fy_max_htod_100 = new float[8] { -0.6f, -0.5f, -0.3f,  0.0f,  0.2f,  0.3f, 0.57f, 0.57f}; //???

            if (fRatioHtoD <= 0.25f)
            {
                for (int i = 0; i < 1; i++)
                {
                    // Interpolate Cpe min
                    fC_pe_roof_values_min[i] = ArrayF.GetLinearInterpolationValuePositive(fAlpha_deg, fx, fy_min_htod_025);

                    // Interpolate Cpe max
                    fC_pe_roof_values_max[i] = ArrayF.GetLinearInterpolationValuePositive(fAlpha_deg, fx, fy_max_htod_025);
                }
            }
            else if (fRatioHtoD < 0.5f)
            {
                float[] fValuesMinTemp = new float[8];
                float[] fValuesMaxTemp = new float[8];

                // Interpolate h to d
                for (int i = 0; i < 8; i++)
                {
                    // Interpolate Cpe min
                    fValuesMinTemp[i] = ArrayF.GetLinearInterpolationValuePositive(0.25f, 0.50f, fy_min_htod_025[i], fy_min_htod_050[i], fRatioHtoD);

                    // Interpolate Cpe max
                    fValuesMaxTemp[i] = ArrayF.GetLinearInterpolationValuePositive(0.25f, 0.50f, fy_max_htod_025[i], fy_max_htod_050[i], fRatioHtoD);
                }

                // Interpolate - angle (roof pitch)
                for (int i = 0; i < 1; i++)
                {
                    // Interpolate Cpe min
                    fC_pe_roof_values_min[i] = ArrayF.GetLinearInterpolationValuePositive(fAlpha_deg, fx, fValuesMinTemp);

                    // Interpolate Cpe max
                    fC_pe_roof_values_max[i] = ArrayF.GetLinearInterpolationValuePositive(fAlpha_deg, fx, fValuesMaxTemp);
                }
            }
            else if (fRatioHtoD < 1.0f)
            {
                float[] fValuesMinTemp = new float[8];
                float[] fValuesMaxTemp = new float[8];

                // Interpolate h to d
                for (int i = 0; i < 8; i++)
                {
                    // Interpolate Cpe min
                    fValuesMinTemp[i] = ArrayF.GetLinearInterpolationValuePositive(0.5f, 1.0f, fy_min_htod_050[i], fy_min_htod_100[i], fRatioHtoD);

                    // Interpolate Cpe max
                    fValuesMaxTemp[i] = ArrayF.GetLinearInterpolationValuePositive(0.5f, 1.0f, fy_max_htod_050[i], fy_max_htod_100[i], fRatioHtoD);
                }

                // Interpolate - angle (roof pitch)
                for (int i = 0; i < 1; i++)
                {
                    // Interpolate Cpe min
                    fC_pe_roof_values_min[i] = ArrayF.GetLinearInterpolationValuePositive(fAlpha_deg, fx, fValuesMinTemp);

                    // Interpolate Cpe max
                    fC_pe_roof_values_max[i] = ArrayF.GetLinearInterpolationValuePositive(fAlpha_deg, fx, fValuesMaxTemp);
                }
            }
            else
            {
                for (int i = 0; i < 1; i++)
                {
                    // Interpolate Cpe min
                    fC_pe_roof_values_min[i] = ArrayF.GetLinearInterpolationValuePositive(fAlpha_deg, fx, fy_min_htod_100);

                    // Interpolate Cpe max
                    fC_pe_roof_values_max[i] = ArrayF.GetLinearInterpolationValuePositive(fAlpha_deg, fx, fy_max_htod_100);
                }
            }

            if (fAlpha_deg >= 45)
            {
                fC_pe_roof_values_min[0] = 0;
                fC_pe_roof_values_max[0] = 0.8f * (float)Math.Sin(fAlpha_deg / 180 * Math.PI);
            }
        }

        protected void Calculate_Cpe_Table_5_3_C(float fh, float fAlpha_deg, float fRatioHtoD, float fRatioBtoD, ref float[] fC_pe_roof_values_min, ref float[] fC_pe_roof_values_max)
        {
            float[] fx = new float[5] { 10, 15, 20, 25, 9999 };
            float[] fy_htod_025 = new float[5] { -0.3f, -0.5f, -0.6f, -0.6f, -0.6f };
            float[] fy_htod_050 = new float[5] { -0.5f, -0.5f, -0.6f, -0.6f, -0.6f };
            float[] fy_htod_100 = new float[5] { -0.7f, -0.6f, -0.6f, -0.6f, -0.6f }; 

            if (fRatioHtoD <= 0.25f)
            {
                for (int i = 0; i < 1; i++)
                {
                    // Interpolate Cpe min
                    fC_pe_roof_values_min[i] = ArrayF.GetLinearInterpolationValuePositive(fAlpha_deg, fx, fy_htod_025);

                    // Interpolate Cpe max
                    fC_pe_roof_values_max[i] = fC_pe_roof_values_min[i];
                }
            }
            else if (fRatioHtoD < 0.5f)
            {
                float[] fValuesTemp = new float[5];

                // Interpolate h to d
                for (int i = 0; i < 5; i++)
                {
                    fValuesTemp[i] = ArrayF.GetLinearInterpolationValuePositive(0.25f, 0.50f, fy_htod_025[i], fy_htod_050[i], fRatioHtoD);
                }

                // Interpolate - angle (roof pitch)
                for (int i = 0; i < 1; i++)
                {
                    // Interpolate Cpe min
                    fC_pe_roof_values_min[i] = ArrayF.GetLinearInterpolationValuePositive(fAlpha_deg, fx, fValuesTemp);

                    // Interpolate Cpe max
                    fC_pe_roof_values_max[i] = fC_pe_roof_values_min[i];
                }
            }
            else if (fRatioHtoD < 1.0f)
            {
                float[] fValuesTemp = new float[5];

                // Interpolate h to d
                for (int i = 0; i < 5; i++)
                {
                    fValuesTemp[i] = ArrayF.GetLinearInterpolationValuePositive(0.5f, 1.0f, fy_htod_050[i], fy_htod_100[i], fRatioHtoD);
                }

                // Interpolate - angle (roof pitch)
                for (int i = 0; i < 1; i++)
                {
                    // Interpolate Cpe min
                    fC_pe_roof_values_min[i] = ArrayF.GetLinearInterpolationValuePositive(fAlpha_deg, fx, fValuesTemp);

                    // Interpolate Cpe max
                    fC_pe_roof_values_max[i] = fC_pe_roof_values_min[i];
                }
            }
            else
            {
                for (int i = 0; i < 1; i++)
                {
                    // Interpolate Cpe min
                    fC_pe_roof_values_min[i] = ArrayF.GetLinearInterpolationValuePositive(fAlpha_deg, fx, fy_htod_100);

                    // Interpolate Cpe max
                    fC_pe_roof_values_max[i] = fC_pe_roof_values_min[i];
                }
            }

            if (fAlpha_deg >= 25)
            {
                if (fRatioBtoD <= 3)
                    fC_pe_roof_values_min[0] = fC_pe_roof_values_max[0] = -0.6f;
                else if (fRatioBtoD < 8)
                    fC_pe_roof_values_min[0] = fC_pe_roof_values_max[0] = -0.06f * (7.0f + fRatioBtoD);
                else
                    fC_pe_roof_values_min[0] = fC_pe_roof_values_max[0] = -0.9f;
            }
        }
        // Table 3.2 - Wind Direction Factors
        protected void SetWindDirectionFactors_9_Items()
        {
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";

            fM_D_array_values_9 = new float[9]; // 8 wind directions (N (0 deg), NE (45 deg), E (90 deg), SE (135 deg), S (180 deg), SW (225 deg), W (270 deg), NW (315 deg)) + 1 for 360 deg
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

                string sWindRegion = "";

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
                        fM_D_array_values_9[i] = float.Parse(reader[sWindRegion].ToString(), nfi);

                        // 3.3.2 - Regions B,C and D
                        if (sWindInput.eWindRegion == EWindRegion.eB ||
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
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";
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
                        Table_4_3_Column1[i] = float.Parse(reader["shieldingParameter"].ToString(), nfi);
                        Table_4_3_Column2[i] = float.Parse(reader["shieldingMultiplierMs"].ToString(), nfi);
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
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";

            // Table 4.1

            float[,] Table_4_1 = new float[12, 5];

            // Connect to database
            using (conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["MainSQLiteDB"].ConnectionString))
            {
                conn.Open();
                SQLiteDataReader reader = null;

                string sTableName = "ASNZS1170_2_Tab4_1_THM";

                SQLiteCommand command = new SQLiteCommand("Select * from " + sTableName, conn);

                using (reader = command.ExecuteReader())
                {
                    int i = 0;
                    while (reader.Read())
                    {
                        Table_4_1[i, 0] = float.Parse(reader["height_z_m"].ToString(), nfi);
                        Table_4_1[i, 1] = float.Parse(reader["tercat_1_Mzcat"].ToString(), nfi);
                        Table_4_1[i, 2] = float.Parse(reader["tercat_2_Mzcat"].ToString(), nfi);
                        Table_4_1[i, 3] = float.Parse(reader["tercat_3_Mzcat"].ToString(), nfi);
                        Table_4_1[i, 4] = float.Parse(reader["tercat_4_Mzcat"].ToString(), nfi);
                        i++;
                    }
                }

                reader.Close();
            }

            // Bilinear Interpolation
            float[] arrayHeaderColumnValues = new float[] { 1, 2, 3, 4 }; // Terrain categories in table 4.1
            fM_z_cat = ArrayF.GetBilinearInterpolationValuePositive(Table_4_1, arrayHeaderColumnValues, fz, sWindInput.fTerrainCategory);
        }

        protected void SetDesignWindSpeedValue(int initialAngleBetweenBetaAndTheta_deg, float[] fV_sit_Theta_360, ref float fV_des_Theta)
        {
            int intervalMinimum_deg = initialAngleBetweenBetaAndTheta_deg - 45;
            int intervalMaximum_deg = initialAngleBetweenBetaAndTheta_deg + 45;

            for (int i = intervalMinimum_deg; i < intervalMaximum_deg; i++)
            {
                if (i < 0)
                {
                    if (fV_sit_Theta_360[360 + intervalMinimum_deg + i] > fV_des_Theta)
                        fV_des_Theta = fV_sit_Theta_360[360 + intervalMinimum_deg + i];
                }
                else if (i < intervalMaximum_deg)
                {
                    if (fV_sit_Theta_360[i] > fV_des_Theta)
                        fV_des_Theta = fV_sit_Theta_360[i];
                }
                else // intervalMaximum_deg > 360
                {
                    if (fV_sit_Theta_360[-360 + intervalMaximum_deg + i] > fV_des_Theta)
                        fV_des_Theta = fV_sit_Theta_360[-360 + intervalMaximum_deg + i];
                }
            }
        }

        // Table 5.9
        protected float GetFrictionalDragCoefficient_Table_5_9(float fx, float fh, float fb, EFrictionalDragCoeff eSurfaceDescription)
        {
            if (fx >= Math.Min(4 * fh, 4 * fb))
            {
                switch (eSurfaceDescription)
                {
                    case EFrictionalDragCoeff.eRibsAcross:
                        return 0.04f;
                    case EFrictionalDragCoeff.eCorrugationAcross:
                        return 0.02f;
                    default:
                        return 0.01f;
                }
            }
            else
                return 0;
        }

        protected float GetArea_Table_5_9(float fh, float fb, float fd)
        {
            if (fh <= fb)
                return (fb + 2 * fh) / (fd - 4 * fh);
            else
                return (fb + 2 * fh) / (fd - 4 * fb);
        }
    }
}