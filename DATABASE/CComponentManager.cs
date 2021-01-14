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
                        compPrefix.ComponentColorCodeRGB = reader["defaultColorRGB"].ToString();
                        compPrefix.ComponentColorCodeHEX = reader["defaultColorHEX"].ToString();
                        compPrefix.ComponentColorName = reader["defaultColorName"].ToString();
                        items.Add(compPrefix);
                    }
                }
            }
            return items;
        }
        public static Dictionary<int, CComponentPrefixes> LoadComponentsFromDB()
        {
            CComponentPrefixes compPrefix;
            Dictionary<int, CComponentPrefixes> items = new Dictionary<int, CComponentPrefixes>();

            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["ModelsSQLiteDB"].ConnectionString))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("Select * from componentPrefixes_FS_Position", conn);
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        compPrefix = new CComponentPrefixes();
                        compPrefix.ID = reader.GetInt32(reader.GetOrdinal("ID"));
                        compPrefix.ComponentName = reader["componentName"].ToString();
                        compPrefix.ComponentPrefix = reader["componentPrefix"].ToString();
                        compPrefix.ComponentColorCodeRGB = reader["defaultColorRGB"].ToString();
                        compPrefix.ComponentColorCodeHEX = reader["defaultColorHEX"].ToString();
                        compPrefix.ComponentColorName = reader["defaultColorName"].ToString();
                        items.Add(compPrefix.ID, compPrefix);
                    }
                }
            }
            return items;
        }
    }
}
