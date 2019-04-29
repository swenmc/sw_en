using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Data.SQLite;
using System.Configuration;
using BaseClasses;

namespace M_EC1.AS_NZS
{
    public class CCalcul_1170_3
    {
        public SQLiteConnection conn;
        BuildingDataInput sBuildInput;
        BuildingGeometryDataInput sGeometryInput;
        SnowLoadDataInput sSnowInput;

        public float fs_ULS_Nu_1;
        public float fs_ULS_Nu_2;
        public float fs_SLS_Nu_1;
        public float fs_SLS_Nu_2;
        public float fs_e_ULS_Nu_1;
        public float fs_e_ULS_Nu_2;
        public float fs_e_SLS_Nu_1;
        public float fs_e_SLS_Nu_2;

        public ESnowElevationRegions eSnowElevationRegion;
        public float fs_g_ULS;
        public float fs_g_SLS;
        public float fC_e;
        public float fNu1_Alpha1;
        public float fNu2_Alpha1;

        public CCalcul_1170_3(BuildingDataInput sBuildingData_temp, BuildingGeometryDataInput sGeometryData_temp, SnowLoadDataInput sSnowData_temp)
        {
            sBuildInput = sBuildingData_temp;
            sGeometryInput = sGeometryData_temp;
            sSnowInput = sSnowData_temp;

            // Determine snow elevation region (alpine, sub-alpine, not significant snow)
            eSnowElevationRegion = GetSnowElevationRegion(sSnowInput.eCountry, sSnowInput.eSnowRegion, sSnowInput.fh_0_SiteElevation_meters);

            // 5 Ground snow load
            fs_g_ULS = AS_NZS_1170_3.Get_sg_5___(sSnowInput.eCountry, sSnowInput.eSnowRegion, eSnowElevationRegion, sSnowInput.fh_0_SiteElevation_meters, sBuildInput.fAnnualProbabilityULS_Snow);
            fs_g_SLS = AS_NZS_1170_3.Get_sg_5___(sSnowInput.eCountry, sSnowInput.eSnowRegion, eSnowElevationRegion, sSnowInput.fh_0_SiteElevation_meters, sBuildInput.fAnnualProbabilitySLS);

            // 4.2.1 Roof snow load
            fC_e = AS_NZS_1170_3.Get_Ce_422_(eSnowElevationRegion, sSnowInput.eExposureCategory);
            
            AS_NZS_1170_3.Set_Nu_64(sGeometryInput.fRoofPitch_deg, ref fC_e, out fNu1_Alpha1, out fNu2_Alpha1);

            fs_ULS_Nu_1 = AS_NZS_1170_3.Eq_42_1____(fs_g_ULS, fC_e, fNu1_Alpha1);
            fs_ULS_Nu_2 = AS_NZS_1170_3.Eq_42_1____(fs_g_ULS, fC_e, fNu2_Alpha1);

            fs_SLS_Nu_1 = AS_NZS_1170_3.Eq_42_1____(fs_g_SLS, fC_e, fNu1_Alpha1);
            fs_SLS_Nu_2 = AS_NZS_1170_3.Eq_42_1____(fs_g_SLS, fC_e, fNu2_Alpha1);

            // 4.2.3 Snow overhanging the edge of a roof
            fs_e_ULS_Nu_1 = AS_NZS_1170_3.Eq_42_2____(fs_ULS_Nu_1, eSnowElevationRegion);
            fs_e_ULS_Nu_2 = AS_NZS_1170_3.Eq_42_2____(fs_ULS_Nu_2, eSnowElevationRegion);

            fs_e_SLS_Nu_1 = AS_NZS_1170_3.Eq_42_2____(fs_SLS_Nu_1, eSnowElevationRegion);
            fs_e_SLS_Nu_2 = AS_NZS_1170_3.Eq_42_2____(fs_SLS_Nu_2, eSnowElevationRegion);
        }

        public ESnowElevationRegions GetSnowElevationRegion(ECountry country, ESnowRegion snowRegion, float fSiteElevation)
        {
            ESnowElevationRegions eSnowElevationRegion;

            if (sSnowInput.eCountry == ECountry.eNewZealand)
            {
                float fLimitSubAlpine_min = 0;
                float fLimitSubAlpine_max = 0;

                NumberFormatInfo nfi = new NumberFormatInfo();
                nfi.NumberDecimalSeparator = ".";

                // Connect to database
                using (conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["MainSQLiteDB"].ConnectionString))
                {
                    conn.Open();
                    SQLiteDataReader reader = null;

                    string sTableName = "SnowRegions";

                    SQLiteCommand command = new SQLiteCommand("Select * from " + sTableName + " where ID = '" + (int)sSnowInput.eSnowRegion + "'", conn);

                    using (reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            fLimitSubAlpine_min = float.Parse(reader["subalpine_min"].ToString(), nfi);
                            fLimitSubAlpine_max = float.Parse(reader["subalpine_max"].ToString(), nfi);
                        }
                    }

                    reader.Close();
                }

                if (fSiteElevation < fLimitSubAlpine_min)
                    eSnowElevationRegion = ESnowElevationRegions.eNoSignificantSnow;
                else if (fSiteElevation <= fLimitSubAlpine_max)
                    eSnowElevationRegion = ESnowElevationRegions.eSubAlpine;
                else
                    eSnowElevationRegion = ESnowElevationRegions.eAlpine;
            }
            else
            {
                eSnowElevationRegion = ESnowElevationRegions.eNoSignificantSnow; // Exception - not implemented for Australia (or other country)
            }

            return eSnowElevationRegion;
        }
    }
}