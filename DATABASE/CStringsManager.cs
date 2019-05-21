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
