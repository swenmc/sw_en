using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SQLite;
using System.Globalization;
using DATABASE.DTO;
using MATERIAL;

namespace DATABASE
{
    public static class CQuantityLibrary
    {
        private static Dictionary<string, QuantityLibraryItem> QuantityLibrary = null;

        public static Dictionary<string, QuantityLibraryItem> GetQuantityLibrary()
        {
            if (QuantityLibrary != null) return QuantityLibrary;

            QuantityLibraryItem item = null;
            Dictionary<string, QuantityLibraryItem> items = new Dictionary<string, QuantityLibraryItem>();

            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["StringsSQLiteDB"].ConnectionString))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("Select * from QuantityLibrary", conn);
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        item = GetQuantityLibraryItem(reader);
                        items.Add(item.UnitIdentificator, item);
                    }
                }
            }
            QuantityLibrary = items;
            return items;
        }

        public static string GetReportUnit(string UnitIdentificator)
        {
            if (QuantityLibrary == null) QuantityLibrary = GetQuantityLibrary();
            return QuantityLibrary[UnitIdentificator].ReportUnit;
        }
       
        private static QuantityLibraryItem GetQuantityLibraryItem(SQLiteDataReader reader)
        {
            QuantityLibraryItem item = new QuantityLibraryItem();
            item.ID = reader.GetInt32(reader.GetOrdinal("ID"));
            item.UnitIdentificator = reader["UnitIdentificator"].ToString();
            item.BasicUnit = reader["BasicUnit"].ToString();
            item.GUIUnit = reader["GUIUnit"].ToString();
            if(!reader.IsDBNull(reader.GetOrdinal("GUIUnitFactor"))) item.GUIUnitFactor = reader.GetFloat(reader.GetOrdinal("GUIUnitFactor"));
            if (!reader.IsDBNull(reader.GetOrdinal("GUIDecimalPlaces"))) item.GUIDecimalPlaces = reader.GetInt32(reader.GetOrdinal("GUIDecimalPlaces"));            
            item.ReportUnit = reader["ReportUnit"].ToString();            
            if (!reader.IsDBNull(reader.GetOrdinal("ReportUnitFactor"))) item.ReportUnitFactor = reader.GetFloat(reader.GetOrdinal("ReportUnitFactor"));
            if (!reader.IsDBNull(reader.GetOrdinal("ReportDecimalPlaces"))) item.ReportDecimalPlaces = reader.GetInt32(reader.GetOrdinal("ReportDecimalPlaces"));
            
            return item;
        }
       
    }
}
