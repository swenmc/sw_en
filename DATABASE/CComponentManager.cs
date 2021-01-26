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
        //TODO vsimol som si ze, to treba refaktorovat s CModelsManager, tam je podobna funkcia
        public static Dictionary<string, CComponentPrefixes> LoadComponentsPrefixes()
        {
            CComponentPrefixes compPrefix;
            Dictionary<string, CComponentPrefixes> items = new Dictionary<string, CComponentPrefixes>();

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
                        items.Add(compPrefix.ComponentPrefix, compPrefix);
                    }
                }
            }
            return items;
        }

        //private static Dictionary<int, CComponentPrefixes> m_DictComponentPrefixes;
        public static Dictionary<int, CComponentPrefixes> LoadComponentsFromDB()
        {
            //if (m_DictComponentPrefixes != null) return m_DictComponentPrefixes; //rusim toto cachovanie kvoli tomu,ze sa prepisu hodnoty a vlastne nevrati potom take ako su povodne

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
            //m_DictComponentPrefixes = items;
            return items;
        }
    }
}
