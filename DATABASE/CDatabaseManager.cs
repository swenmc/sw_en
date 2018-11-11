using DATABASE.DTO;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Data.SQLite;
using System.Globalization;

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

                // TODO - Ondrej - SQL subquery in database
                // vybrat vsetky riadky s designWorkingLife_ID a z uz vybranych riadkov vybrat vsetky riadky s 
                // s uvedenym importanceLevel_ID
                // vysledkom dotazu ma byt jeden riadok, pricom hodnoty apoeULS_xxx a SLS1 sa zapisu do premennych
                // TODO Mato - Ak je to OK, tak koment vymazat
                // 26.09.2018 - TO Ondrej, nefunguje to - ten SQL dodaz nie je spravny, malo by to nacitavat hodnoty podla kombinacie stlpcov designWorkingLife_ID a importanceLevel_ID
                // berie to podla importanceLevel_ID

                //SQLiteCommand command = new SQLiteCommand(
                //    "Select * from " +
                //    " ( " +
                //    "Select * from " + sTableName + " where designWorkingLife_ID = '" + DesignLifeIndex +
                //    "')," +
                //    " ( " +
                //    "Select * from " + sTableName + " where importanceLevel_ID = '" + ImportanceClassIndex +
                //    "')"
                //    , conn);

                // tak este raz...ak je to OK, tak zmazat vsetky predosle komenty
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
                        string sAnnualProbabilityULS_Wind = reader["apoeULS_Wind"].ToString();
                        string sAnnualProbabilityULS_Snow = reader["apoeULS_Snow"].ToString();
                        string sAnnualProbabilityULS_EQ = reader["apoeULS_Earthquake"].ToString();
                        string sAnnualProbabilitySLS1 = reader["SLS1"].ToString();
                        string sAnnualProbabilitySLS2 = "";
                        try
                        {
                            if (!reader.IsDBNull(reader.GetOrdinal("SLS2")))
                            {
                                sAnnualProbabilitySLS2 = reader["SLS2"].ToString();
                            }
                        }
                        catch (ArgumentNullException) { }
                        annualProb = new CAnnualProbability();
                        annualProb.AnnualProbabilityULS_Wind = (float)FractionConverter.Convert(sAnnualProbabilityULS_Wind);
                        annualProb.AnnualProbabilityULS_Snow = (float)FractionConverter.Convert(sAnnualProbabilityULS_Snow);
                        annualProb.AnnualProbabilityULS_EQ = (float)FractionConverter.Convert(sAnnualProbabilityULS_EQ);
                        annualProb.AnnualProbabilitySLS = (float)FractionConverter.Convert(sAnnualProbabilitySLS1);

                        //TODO Martin - doriesit SLS1 a SLS2
                        //AnnualProbabilitySLS2 = (float)FractionConverter.Convert(sAnnualProbabilitySLS2);

                        annualProb.R_ULS_Wind = float.Parse(reader["R_ULS_Wind_inyears"].ToString());
                        annualProb.R_ULS_Snow = float.Parse(reader["R_ULS_Snow_inyears"].ToString());
                        annualProb.R_ULS_EQ = float.Parse(reader["R_ULS_Earthquake_inyears"].ToString());

                        try
                        {
                            if (!reader.IsDBNull(reader.GetOrdinal("R_SLS1_inyears")))
                            {
                                annualProb.R_SLS = float.Parse(reader["R_SLS1_inyears"].ToString());
                            }
                        }
                        catch (ArgumentNullException) { }
                    }
                }

                reader.Close();
            }
            return annualProb;
        }



    }
}
