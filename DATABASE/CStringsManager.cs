using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SQLite;
using System.Globalization;
using DATABASE.DTO;
using MATERIAL;

namespace DATABASE
{
    public static class CStringsManager
    {
        public static List<DataExportTables> LoadBasicLoadParameters()
        {
            return LoadStringsTable("BasicLoadParameters");
        }
        public static List<DataExportTables> LoadDeadLoadParameters()
        {
            return LoadStringsTable("AS1170_1");
        }

        private static List<DataExportTables> LoadStringsTable(string tableName)
        {
            List<DataExportTables> items = new List<DataExportTables>();
            DataExportTables data = null;

            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["StringsSQLiteDB"].ConnectionString))
            {
                conn.Open();

                /*
                // TO Ondrej - pokus zmazat riadky s null ID
                if (true)
                {
                    string queryString = $"Delete from " + tableName + " where ID = NULL";
                    SQLiteCommand command_delete = new SQLiteCommand(queryString, conn);
                    string queryString2 = $"Delete from " + tableName + " where ID = ''";
                    SQLiteCommand command_delete2 = new SQLiteCommand(queryString2, conn);
                    string queryString3 = $"Delete from " + tableName + " where ID IS NULL";
                    SQLiteCommand command_delete3 = new SQLiteCommand(queryString3, conn);
                }*/


                SQLiteCommand command = new SQLiteCommand($"Select * from {tableName}", conn);
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        data = GetDataForExport(reader);
                        if (data != null) items.Add(data);
                    }
                }
            }
            return items;
        }

        private static DataExportTables GetDataForExport(SQLiteDataReader reader)
        {
            //To Mato: v databaze ak je ID,tak tam nemaju co hladat NULL hodnoty
            // To Ondrej: Mam taky mensi problem s convertorom http://converttosqlite.com/convert
            // Ak je v xls nejako dotknuta bunka na prazdnom riadku, tak to vsetky tie prazdne riadky konvertuje do sql
            // Chcel som to zmazat cez SQL prikaz vid vyssie ale nezadarilo sa.
            // Mozes mi urobit taku utilitku ze ked zmenim databazove subory, tak pri spusteni programu mi to automaticky odmaze zo vsetkych databaz a vsetkych tabuliek

            if (reader.IsDBNull(reader.GetOrdinal("ID"))) return null;

            DataExportTables data = new DataExportTables();
            data.ID = reader.GetInt32(reader.GetOrdinal("ID"));
            data.Description_ENU_USA = reader["Description_ENU_USA"].ToString();
            data.Description_CSY = reader["Description_CSY"].ToString();
            data.Description_SVK = reader["Description_SVK"].ToString();
            data.Symbol = reader["Symbol"].ToString();
            data.Identificator = reader["Identificator"].ToString();
            data.Unit = reader["Unit"].ToString();
            if (reader.IsDBNull(reader.GetOrdinal("UnitFactor"))) data.UnitFactor = 1;
            else data.UnitFactor = reader.GetFloat(reader.GetOrdinal("UnitFactor"));
            
            return data;
        }
        
    }
}
