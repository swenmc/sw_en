using DATABASE.DTO;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Data.SQLite;
using System.Globalization;
using MATH;

namespace DATABASE
{
    public static class CDatabaseManager
    {
        public static List<string> GetStringList(string connStringDBName, string tableName, string columnName)
        {
            List<string> items = new List<string>();
            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings[connStringDBName].ConnectionString))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("Select * from " + tableName, conn);
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        items.Add(reader[columnName].ToString());
                    }
                }
            }
            return items;
        }

        public static float GetDesignLifeValueFromDatabase(int DesignLifeIndex)
        {
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";
            float DesignLife_Value = 0;
            // Connect to database
            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["MainSQLiteDB"].ConnectionString))
            {
                conn.Open();
                SQLiteDataReader reader = null;
                string sTableName = "ASNZS1170_Tab3_3_DWL";

                SQLiteCommand command = new SQLiteCommand("Select * from " + sTableName + " where ID = '" + DesignLifeIndex + "'", conn);

                using (reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        DesignLife_Value = float.Parse(reader["time_in_years"].ToString(), nfi);
                    }
                }

                reader.Close();
            }
            return DesignLife_Value;
        }

        public static CAnnualProbability GetAnnualProbabilityValuesFromDatabase(int DesignLifeIndex, int ImportanceClassIndex)
        {
            CAnnualProbability annualProb = null;
            // Connect to database
            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["MainSQLiteDB"].ConnectionString))
            {
                /*
                AnnualProbabilityULS_Wind = 1f / 250f;
                AnnualProbabilityULS_Snow = 1f / 250f;
                AnnualProbabilityULS_EQ = 1f / 250f;
                AnnualProbabilitySLS = 1f / 25f;
                */

                conn.Open();
                SQLiteDataReader reader = null;
                string sTableName = "ASNZS1170_Tab3_3_APOE";

                SQLiteCommand command = new SQLiteCommand("Select * from " + sTableName + " where designWorkingLife_ID = @designLifeIndex AND importanceLevel_ID = @importanceClassIndex", conn);
                command.Parameters.AddWithValue("@designLifeIndex", DesignLifeIndex);
                command.Parameters.AddWithValue("@importanceClassIndex", ImportanceClassIndex);

                using (reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        //SnowRegionIndex = int.Parse(reader["snow_zone"].ToString());
                        //WindRegionIndex = int.Parse(reader["wind_zone"].ToString());

                        // TODO - Ondrej osetrit pripady ked nie je v databaze vyplnena hodnota
                        // Osetrit ako nacitat zlomok zadany ako string v databaze do float
                        // Bolo by super ak by sa zlomok v textboxe zobrazoval ako zlomok a potom sa previedol na float az vo vypocte
                        // uzivatel by mohol zadavat do textboxu zlomok x / y alebo priamo float

                        // TODO Ondrej - pridal som tuto triedu pre zadavanie zlomkov ako riesenie vyssie uvedeneho, mozes sa na to prosim pozriet a pripadne ju presunut na nejake bazove miesto aby sa dala pre GUI pouzivat univerzalne
                        // Pridana trieda "FractionConverter.cs" - presunut inam ???

                        /*
                        AnnualProbabilityULS_Wind = float.Parse(reader["apoeULS_Wind"].ToString());
                        AnnualProbabilityULS_Snow = float.Parse(reader["apoeULS_Snow"].ToString());
                        AnnualProbabilityULS_EQ = float.Parse(reader["apoeULS_Earthquake"].ToString());
                        AnnualProbabilitySLS = float.Parse(reader["SLS1"].ToString());
                        float AnnualProbabilitySLS_2 = float.Parse(reader["SLS2"].ToString());
                        */

                        string sAnnualProbabilityULS_Wind = "";
                        try
                        {
                            if (!reader.IsDBNull(reader.GetOrdinal("apoeULS_Wind")))
                            {
                                sAnnualProbabilityULS_Wind = reader["apoeULS_Wind"].ToString();
                            }
                        }
                        catch (ArgumentNullException)
                        {
                            sAnnualProbabilityULS_Wind = "0.2";
                        }

                        string sAnnualProbabilityULS_Snow = "";
                        try
                        {
                            if (!reader.IsDBNull(reader.GetOrdinal("apoeULS_Snow")))
                            {
                                sAnnualProbabilityULS_Snow = reader["apoeULS_Snow"].ToString();
                            }
                        }
                        catch (ArgumentNullException)
                        {
                            sAnnualProbabilityULS_Snow = "0.2";
                        }

                        string sAnnualProbabilityULS_EQ = "";
                        try
                        {
                            if (!reader.IsDBNull(reader.GetOrdinal("apoeULS_Earthquake")))
                            {
                                sAnnualProbabilityULS_EQ = reader["apoeULS_Earthquake"].ToString();
                            }
                        }
                        catch (ArgumentNullException)
                        {
                            sAnnualProbabilityULS_EQ = "0.2";
                        }

                        string sAnnualProbabilitySLS1 = "";
                        try
                        {
                            if (!reader.IsDBNull(reader.GetOrdinal("SLS1")))
                            {
                                sAnnualProbabilitySLS1 = reader["SLS1"].ToString();
                            }
                        }
                        catch (ArgumentNullException)
                        {
                            sAnnualProbabilitySLS1 = "0.2";
                        }

                        string sAnnualProbabilitySLS2 = "";
                        try
                        {
                            if (!reader.IsDBNull(reader.GetOrdinal("SLS2")))
                            {
                                sAnnualProbabilitySLS2 = reader["SLS2"].ToString();
                            }
                        }
                        catch (ArgumentNullException)
                        {
                            sAnnualProbabilitySLS2 = "0.2";
                        }

                        annualProb = new CAnnualProbability();
                        annualProb.AnnualProbabilityULS_Wind = (float)FractionConverter.Convert(sAnnualProbabilityULS_Wind);
                        annualProb.AnnualProbabilityULS_Snow = (float)FractionConverter.Convert(sAnnualProbabilityULS_Snow);
                        annualProb.AnnualProbabilityULS_EQ = (float)FractionConverter.Convert(sAnnualProbabilityULS_EQ);
                        annualProb.AnnualProbabilitySLS = (float)FractionConverter.Convert(sAnnualProbabilitySLS1);
                        //annualProb.AnnualProbabilitySLS2 = (float)FractionConverter.Convert(sAnnualProbabilitySLS2); //TODO Martin - doriesit SLS1 a SLS2

                        try
                        {
                            if (!reader.IsDBNull(reader.GetOrdinal("R_ULS_Wind_inyears")))
                            {
                                annualProb.R_ULS_Wind = float.Parse(reader["R_ULS_Wind_inyears"].ToString());
                            }
                        }
                        catch (ArgumentNullException)
                        {
                            annualProb.R_ULS_Wind = 5f;
                        }

                        try
                        {
                            if (!reader.IsDBNull(reader.GetOrdinal("R_ULS_Snow_inyears")))
                            {
                                annualProb.R_ULS_Snow = float.Parse(reader["R_ULS_Snow_inyears"].ToString());
                            }
                        }
                        catch (ArgumentNullException)
                        {
                            annualProb.R_ULS_Snow = 5f;
                        }

                        try
                        {
                            if (!reader.IsDBNull(reader.GetOrdinal("R_ULS_Earthquake_inyears")))
                            {
                                annualProb.R_ULS_EQ = float.Parse(reader["R_ULS_Earthquake_inyears"].ToString());
                            }
                        }
                        catch (ArgumentNullException)
                        {
                            annualProb.R_ULS_EQ = 5f;
                        }

                        try
                        {
                            if (!reader.IsDBNull(reader.GetOrdinal("R_SLS1_inyears")))
                            {
                                annualProb.R_SLS = float.Parse(reader["R_SLS1_inyears"].ToString());
                            }
                        }
                        catch (ArgumentNullException)
                        {
                            annualProb.R_SLS = 5f;
                        }

                        /*
                        try
                        {
                            if (!reader.IsDBNull(reader.GetOrdinal("R_SLS2_inyears")))
                            {
                                annualProb.R_SLS2 = float.Parse(reader["R_SLS2_inyears"].ToString());
                            }
                        }
                        catch (ArgumentNullException)
                        {
                            annualProb.R_SLS2 = 5f;
                        }*/
                    }
                }

                reader.Close();
            }
            return annualProb;
        }



    }
}
