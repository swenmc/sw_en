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

        // Elastic site spectra

        // Peak ground acceleration
        float fT_period_PGA = 0f;
        float fC_h_PGA = 1.12f; // Spectral shape factor

        float fC_PGA_ULS = 0.39f;
        float fC_PGA_SLS = 0.1f; // Peak ground coefficient

        float fC_v_ULS;
        float fC_v_SLS;

        // AS / NZS 4600:2018, cl. 1.6.4.2.2 Structural ductility factor
        float fNu_ULS = 1.25f; // Structural ductility factor, 1.6.4.2.2(a)

        // NZS 117.5:2004
        /*
        4.3.2 Serviceability limit state
        The structural ductility factor, μ, for the serviceability limit state SLS1 shall be
        1.0 ≤ nu ≤ 1.25 and for SLS2 shall be within the limits 1.0. ≤ n ≤ 2.0.
        */
        float fNu_SLS = 1.00f; // Structural ductility factor, 1.6.4.2.2

        //float fkNu_ULS; // Ductility coefficient (depends on site soil class)
        //float fkNu_SLS;

        // AS/NZS 4600
        // When considering lateral stability of a whole structure, the structural performance factor (Sp) shall be taken as 1.0.
        float fS_p_ULS_stab = 1.00f; // Structural performance factor
        float fS_p_ULS_strength = 0.90f;
        float fS_p_SLS = 0.70f;

        float fC_T1 = 0; // ??????????????
        float fCv_T1 = 0; // ?????????????

        float fC_d_T1_ULS_stab;
        float fC_d_T1_ULS_strength;
        float fC_d_T1_SLS;

        float fC_v_Tv_ULS_stab;
        float fC_v_Tv_ULS_strength;
        float fC_v_Tv_SLS;

        public CCalcul_1170_5(float fW, float fL1_PF_spacing, float fH1_columns, BuildingDataInput sBuildInput, SeisLoadDataInput sSeisInput)
        {
            // AS/NZS 4600:2018 - 1.6.4.2.4 Structural performance factor (a)
            if (1 < fNu_ULS && fNu_ULS <= 2)
                fS_p_ULS_strength = 1.3f - 0.3f * fNu_ULS;
            else
                fS_p_ULS_strength = 0.7f;

            // Seismic Weight
            float fg_roof = 200f; // kN / m^2
            float fg_walls = 200f; // kN / m^2

            float fq_roof = 250f; // kN / m^2

            float fG_roof = fg_roof * fL1_PF_spacing * fW;
            float fG_walls = fg_roof * fH1_columns * fL1_PF_spacing;

            float fG_tot = fG_roof + fG_walls;

            float fCdCT_ULS = 0.536f;
            float fCdCT_SLS = 0.116f;

            float fG_tot_ULS = fCdCT_ULS * fG_tot;
            float fG_tot_SLS = fCdCT_SLS * fG_tot;

            float fR_ULS = GetReturnPeriodFactor_R(sBuildInput.fAnnualProbabilityULS_EQ);
            float fR_SLS = GetReturnPeriodFactor_R(sBuildInput.fAnnualProbabilitySLS);

            float fN_TxD_ULS = GetNearFaultFactor_N_TD(sBuildInput.fAnnualProbabilityULS_EQ, sSeisInput.fProximityToFault_D_km, sSeisInput.fPeriodAlongXDirection_Tx);
            float fN_TyD_ULS = GetNearFaultFactor_N_TD(sBuildInput.fAnnualProbabilityULS_EQ, sSeisInput.fProximityToFault_D_km, sSeisInput.fPeriodAlongYDirection_Ty);

            float fN_TxD_SLS = GetNearFaultFactor_N_TD(sBuildInput.fAnnualProbabilitySLS, sSeisInput.fProximityToFault_D_km, sSeisInput.fPeriodAlongXDirection_Tx);
            float fN_TyD_SLS = GetNearFaultFactor_N_TD(sBuildInput.fAnnualProbabilitySLS, sSeisInput.fProximityToFault_D_km, sSeisInput.fPeriodAlongYDirection_Ty);

            float fC_Tx_ULS = AS_NZS_1170_5.Eq_31_1____(sSeisInput.fSpectralShapeFactor_Ch_Tx, sSeisInput.fZoneFactor_Z, fR_ULS, fN_TxD_ULS);
            float fC_Ty_ULS = AS_NZS_1170_5.Eq_31_1____(sSeisInput.fSpectralShapeFactor_Ch_Ty, sSeisInput.fZoneFactor_Z, fR_ULS, fN_TyD_ULS);

            float fC_Tx_SLS = AS_NZS_1170_5.Eq_31_1____(sSeisInput.fSpectralShapeFactor_Ch_Tx, sSeisInput.fZoneFactor_Z, fR_SLS, fN_TxD_SLS);
            float fC_Ty_SLS = AS_NZS_1170_5.Eq_31_1____(sSeisInput.fSpectralShapeFactor_Ch_Ty, sSeisInput.fZoneFactor_Z, fR_SLS, fN_TyD_SLS);

            float fT_1 = 1; // TODO // 4.1.2.1

            fC_d_T1_ULS_stab = AS_NZS_1170_5.Eq_5221_ULS(fC_T1, fS_p_ULS_stab, sSeisInput.fZoneFactor_Z, fR_ULS, fT_1, fNu_ULS, sSeisInput.eSiteSubsoilClass);
            fC_d_T1_ULS_strength = AS_NZS_1170_5.Eq_5221_ULS(fC_T1, fS_p_ULS_strength, sSeisInput.fZoneFactor_Z, fR_ULS, fT_1, fNu_ULS, sSeisInput.eSiteSubsoilClass);
            fC_d_T1_SLS = AS_NZS_1170_5.Get_C_D_T1_5212_SLS(fC_T1, fT_1, fNu_SLS, sSeisInput.eSiteSubsoilClass);

            // Vetical - use T1 ???
            fC_v_ULS = 0.7f * fC_PGA_ULS;
            fC_v_SLS = 0.7f * fC_PGA_SLS;

            fC_v_Tv_ULS_stab = AS_NZS_1170_5.Eq_5221_ULS(fC_v_ULS, fS_p_ULS_stab, sSeisInput.fZoneFactor_Z, fR_ULS, fT_1, fNu_ULS, sSeisInput.eSiteSubsoilClass);
            fC_v_Tv_ULS_strength = AS_NZS_1170_5.Eq_5221_ULS(fC_v_ULS, fS_p_ULS_strength, sSeisInput.fZoneFactor_Z, fR_ULS, fT_1, fNu_ULS, sSeisInput.eSiteSubsoilClass);
            fC_v_Tv_SLS = AS_NZS_1170_5.Get_C_D_T1_5212_SLS(fC_v_SLS, fT_1, fNu_SLS, sSeisInput.eSiteSubsoilClass);
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
