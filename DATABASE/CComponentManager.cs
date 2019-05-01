using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SQLite;
using System.Globalization;
using DATABASE.DTO;
using MATERIAL;

namespace DATABASE
{
    public static class CComponentManager
    {
        
        public static List<CComponentPrefixes> LoadComponentsPrefixes()
        {
            CComponentPrefixes compPrefix;
            List<CComponentPrefixes> items = new List<CComponentPrefixes>();

            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["ModelsSQLiteDB"].ConnectionString))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("Select * from componentPrefixes", conn);
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        compPrefix = new CComponentPrefixes();
                        compPrefix.ID = reader.GetInt32(reader.GetOrdinal("ID"));
                        compPrefix.ComponentName = reader["componentName"].ToString();
                        compPrefix.ComponentPrefix = reader["componentPrefix"].ToString();
                        compPrefix.ComponentColorCodeRGB = reader["componentColorRGB"].ToString();
                        compPrefix.ComponentColorName = reader["componentColorName"].ToString();
                        items.Add(compPrefix);
                    }
                }
            }
            return items;
        }

        
    }
}
