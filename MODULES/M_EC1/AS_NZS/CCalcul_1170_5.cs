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
    public class CCalcul_1170_5
    {
        public SQLiteConnection conn;

        // AS / NZS 4600:2018, cl. 1.6.4.2.2 Structural ductility factor
        public float fNu_ULS = 1.25f; // Structural ductility factor, 1.6.4.2.2(a)

        // NZS 1170.5:2004
        /*
        4.3.2 Serviceability limit state
        The structural ductility factor, μ, for the serviceability limit state SLS1 shall be
        1.0 ≤ nu ≤ 1.25 and for SLS2 shall be within the limits 1.0. ≤ n ≤ 2.0.
        */
        public float fNu_SLS = 1.00f; // Structural ductility factor, 1.6.4.2.2

        //float fkNu_ULS; // Ductility coefficient (depends on site soil class)
        //float fkNu_SLS;

        // AS/NZS 4600
        // When considering lateral stability of a whole structure, the structural performance factor (Sp) shall be taken as 1.0.
        float fS_p_ULS_stab = 1.00f; // Structural performance factor
        public float fS_p_ULS_strength = 0.90f;
        public float fS_p_SLS = 0.70f;

        float fC_d_T1x_ULS_stab;
        public float fC_d_T1x_ULS_strength;
        float fC_d_T1x_SLS;

        float fC_d_T1y_ULS_stab;
        public float fC_d_T1y_ULS_strength;
        float fC_d_T1y_SLS;

        /*
        float fC_v_Tvx_ULS_stab;
        float fC_v_Tvx_ULS_strength;
        float fC_v_Tvx_SLS;

        float fC_v_Tvy_ULS_stab;
        float fC_v_Tvy_ULS_strength;
        float fC_v_Tvy_SLS;
        */

        public float fV_x_ULS_stab;
        public float fV_x_ULS_strength;
        public float fV_x_SLS;

        public float fV_y_ULS_stab;
        public float fV_y_ULS_strength;
        public float fV_y_SLS;

        public float fG_tot_x;
        public float fG_tot_y;

        public float fN_TxD_ULS;
        public float fC_Tx_ULS;
        public float fk_Nu_Tx_ULS_stab;
        public float fk_Nu_Tx_ULS_strength;
        public float fk_Nu_Tx_SLS;

        public float fN_TyD_ULS;
        public float fC_Ty_ULS;
        public float fk_Nu_Ty_ULS_stab;
        public float fk_Nu_Ty_ULS_strength;
        public float fk_Nu_Ty_SLS;

        public CCalcul_1170_5(float fT_1x, float fT_1y, float param_fG_tot_x, float param_fG_tot_y, BuildingDataInput sBuildInput, SeisLoadDataInput sSeisInput)
        {
            fG_tot_x = param_fG_tot_x;
            fG_tot_y = param_fG_tot_y;
            // AS/NZS 4600:2018 - 1.6.4.2.4 Structural performance factor (a)
            if (1 < fNu_ULS && fNu_ULS <= 2)
                fS_p_ULS_strength = 1.3f - 0.3f * fNu_ULS;
            else
                fS_p_ULS_strength = 0.7f;

            float fR_ULS = GetReturnPeriodFactor_R(sBuildInput.fAnnualProbabilityULS_EQ);
            float fR_SLS = GetReturnPeriodFactor_R(sBuildInput.fAnnualProbabilitySLS);

            fN_TxD_ULS = GetNearFaultFactor_N_TD(sBuildInput.fAnnualProbabilityULS_EQ, sSeisInput.fProximityToFault_D_km, sSeisInput.fPeriodAlongXDirection_Tx);
            fN_TyD_ULS = GetNearFaultFactor_N_TD(sBuildInput.fAnnualProbabilityULS_EQ, sSeisInput.fProximityToFault_D_km, sSeisInput.fPeriodAlongYDirection_Ty);

            float fN_TxD_SLS = GetNearFaultFactor_N_TD(sBuildInput.fAnnualProbabilitySLS, sSeisInput.fProximityToFault_D_km, sSeisInput.fPeriodAlongXDirection_Tx);
            float fN_TyD_SLS = GetNearFaultFactor_N_TD(sBuildInput.fAnnualProbabilitySLS, sSeisInput.fProximityToFault_D_km, sSeisInput.fPeriodAlongYDirection_Ty);

            fC_Tx_ULS = AS_NZS_1170_5.Eq_31_1____(sSeisInput.fSpectralShapeFactor_Ch_Tx, sSeisInput.fZoneFactor_Z, fR_ULS, fN_TxD_ULS);
            fC_Ty_ULS = AS_NZS_1170_5.Eq_31_1____(sSeisInput.fSpectralShapeFactor_Ch_Ty, sSeisInput.fZoneFactor_Z, fR_ULS, fN_TyD_ULS);

            float fC_Tx_SLS = AS_NZS_1170_5.Eq_31_1____(sSeisInput.fSpectralShapeFactor_Ch_Tx, sSeisInput.fZoneFactor_Z, fR_SLS, fN_TxD_SLS);
            float fC_Ty_SLS = AS_NZS_1170_5.Eq_31_1____(sSeisInput.fSpectralShapeFactor_Ch_Ty, sSeisInput.fZoneFactor_Z, fR_SLS, fN_TyD_SLS);

            fC_d_T1x_ULS_stab = AS_NZS_1170_5.Eq_5221_ULS(fC_Tx_ULS, fS_p_ULS_stab, sSeisInput.fZoneFactor_Z, fR_ULS, fT_1x, fNu_ULS, sSeisInput.eSiteSubsoilClass, out fk_Nu_Tx_ULS_stab);
            fC_d_T1x_ULS_strength = AS_NZS_1170_5.Eq_5221_ULS(fC_Tx_ULS, fS_p_ULS_strength, sSeisInput.fZoneFactor_Z, fR_ULS, fT_1x, fNu_ULS, sSeisInput.eSiteSubsoilClass, out fk_Nu_Tx_ULS_strength);
            fC_d_T1x_SLS = AS_NZS_1170_5.Get_C_D_T1_5212_SLS(fC_Tx_SLS, fT_1x, fNu_SLS, sSeisInput.eSiteSubsoilClass, out fk_Nu_Tx_SLS);

            fC_d_T1y_ULS_stab = AS_NZS_1170_5.Eq_5221_ULS(fC_Ty_ULS, fS_p_ULS_stab, sSeisInput.fZoneFactor_Z, fR_ULS, fT_1y, fNu_ULS, sSeisInput.eSiteSubsoilClass, out fk_Nu_Ty_ULS_stab);
            fC_d_T1y_ULS_strength = AS_NZS_1170_5.Eq_5221_ULS(fC_Ty_ULS, fS_p_ULS_strength, sSeisInput.fZoneFactor_Z, fR_ULS, fT_1y, fNu_ULS, sSeisInput.eSiteSubsoilClass, out fk_Nu_Ty_ULS_strength);
            fC_d_T1y_SLS = AS_NZS_1170_5.Get_C_D_T1_5212_SLS(fC_Ty_SLS, fT_1y, fNu_SLS, sSeisInput.eSiteSubsoilClass, out fk_Nu_Ty_SLS);

            /*
            // TODO - Martin - v pripade potreby dopracovat podla cl. 3.2 a 5.4
            fC_v_Tvx_ULS_stab = AS_NZS_1170_5.Eq_5221_ULS(fC_v_ULS, fS_p_ULS_stab, sSeisInput.fZoneFactor_Z, fR_ULS, fT_1x, fNu_ULS, sSeisInput.eSiteSubsoilClass);
            fC_v_Tvx_ULS_strength = AS_NZS_1170_5.Eq_5221_ULS(fC_v_ULS, fS_p_ULS_strength, sSeisInput.fZoneFactor_Z, fR_ULS, fT_1x, fNu_ULS, sSeisInput.eSiteSubsoilClass);
            fC_v_Tvx_SLS = AS_NZS_1170_5.Get_C_D_T1_5212_SLS(fC_v_SLS, fT_1x, fNu_SLS, sSeisInput.eSiteSubsoilClass);

            fC_v_Tvy_ULS_stab = AS_NZS_1170_5.Eq_5221_ULS(fC_v_ULS, fS_p_ULS_stab, sSeisInput.fZoneFactor_Z, fR_ULS, fT_1y, fNu_ULS, sSeisInput.eSiteSubsoilClass);
            fC_v_Tvy_ULS_strength = AS_NZS_1170_5.Eq_5221_ULS(fC_v_ULS, fS_p_ULS_strength, sSeisInput.fZoneFactor_Z, fR_ULS, fT_1y, fNu_ULS, sSeisInput.eSiteSubsoilClass);
            fC_v_Tvy_SLS = AS_NZS_1170_5.Get_C_D_T1_5212_SLS(fC_v_SLS, fT_1y, fNu_SLS, sSeisInput.eSiteSubsoilClass);
            */

            // Forces at one frame (number of frames)
            fV_x_ULS_stab = AS_NZS_1170_5.Eq_62_1____(fC_d_T1x_ULS_stab, fG_tot_x);
            fV_x_ULS_strength = AS_NZS_1170_5.Eq_62_1____(fC_d_T1x_ULS_strength, fG_tot_x);
            fV_x_SLS = AS_NZS_1170_5.Eq_62_1____(fC_d_T1x_SLS, fG_tot_x);

            // Forces at one frame (sides of structure = 2)
            fV_y_ULS_stab = AS_NZS_1170_5.Eq_62_1____(fC_d_T1y_ULS_stab, fG_tot_y);
            fV_y_ULS_strength = AS_NZS_1170_5.Eq_62_1____(fC_d_T1y_ULS_strength, fG_tot_y);
            fV_y_SLS = AS_NZS_1170_5.Eq_62_1____(fC_d_T1y_SLS, fG_tot_y);

            // TODO - tento vypocet nezohladnuje ine zatazenie a tuhost koncovych ramov
        }

        protected float GetReturnPeriodFactor_R(float fRequiredAnnualProbabilityOfExceedance)
        {
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";
            // Table 3.5

            float[] Table_3_5_Column1 = new float[9];
            float[] Table_3_5_Column2 = new float[9];

            // Connect to database
            using (conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["MainSQLiteDB"].ConnectionString))
            {
                conn.Open();
                SQLiteDataReader reader = null;

                string sTableName = "ASNZS1170_5_Tab3_5_RPF";

                SQLiteCommand command = new SQLiteCommand("Select * from " + sTableName, conn);

                using (reader = command.ExecuteReader())
                {
                    int i = 0;
                    while (reader.Read())
                    {
                        Table_3_5_Column1[i] = float.Parse(reader["rapoe_decimal"].ToString(), nfi);
                        Table_3_5_Column2[i] = float.Parse(reader["FactorR"].ToString(), nfi);
                        i++;
                    }
                }

                reader.Close();
            }

            // Interpolation
            return (float)ArrayF.GetLinearInterpolationValuePositive(fRequiredAnnualProbabilityOfExceedance, Table_3_5_Column1, Table_3_5_Column2);
        }
        protected float GetNearFaultFactor_N_TD(float fRequiredAnnualProbabilityOfExceedance, float fD_km, float fT)
        {
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";
            // 3.1.6 Near-fault Factor
            if (fRequiredAnnualProbabilityOfExceedance >= 1f / 250f) // 3.1.6.1
                return 1.0f;
            else // 3.1.6.2
            {
                // Table 3.7
                float[] Table_3_7_Column1 = new float[5];
                float[] Table_3_7_Column2 = new float[5];

                // Connect to database
                using (conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["MainSQLiteDB"].ConnectionString))
                {
                    conn.Open();
                    SQLiteDataReader reader = null;

                    string sTableName = "ASNZS1170_5_Tab3_7_MNFF";

                    SQLiteCommand command = new SQLiteCommand("Select * from " + sTableName, conn);

                    using (reader = command.ExecuteReader())
                    {
                        int i = 0;
                        while (reader.Read())
                        {
                            Table_3_7_Column1[i] = float.Parse(reader["periodT_sec"].ToString(), nfi);
                            Table_3_7_Column2[i] = float.Parse(reader["N_max_T"].ToString(), nfi);
                            i++;
                        }
                    }

                    reader.Close();
                }

                // Interpolation
                float fN_max_T = (float)ArrayF.GetLinearInterpolationValuePositive(fT, Table_3_7_Column1, Table_3_7_Column2); // Table 3.7

                // 3.1.6.2
                if (fD_km < 2f)
                    return fN_max_T;
                else if (fD_km <= 20f) // D <= 2 km
                    return 1f + (fN_max_T - 1f) * (20f - fD_km) / 18f; // 2 km < D <= 20 km
                else
                    return 1f; // D > 20 km
            }
        }
    }
}
