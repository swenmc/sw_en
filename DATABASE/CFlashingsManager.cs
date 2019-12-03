using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Data.SQLite;
using System.Globalization;
using DATABASE.DTO;

namespace DATABASE
{
    public static class CFlashingsManager
    {
        public static Dictionary<string, CFlashingProperties> DictFlashingProperties;

        public static List<CFlashingProperties> LoadFlashingsProperties()
        {
            CFlashingProperties properties = null;
            List<CFlashingProperties> items = new List<CFlashingProperties>();

            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["AccessoriesSQLiteDB"].ConnectionString))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("Select * from Flashing", conn);
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        properties = GetFlashingProperties(reader);

                        items.Add(properties);
                    }
                }
            }
            return items;
        }

        public static void LoadFlashingsPropertiesDictionary()
        {
            DictFlashingProperties = new Dictionary<string, CFlashingProperties>();
            
            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["AccessoriesSQLiteDB"].ConnectionString))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("Select * from Flashing", conn);
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        CFlashingProperties properties = GetFlashingProperties(reader);
                        DictFlashingProperties.Add(properties.Name, properties);
                    }
                }
            }
        }

        public static CFlashingProperties GetFlashingProperties(string name)
        {
            if (DictFlashingProperties == null) LoadFlashingsPropertiesDictionary();

            CFlashingProperties prop = null;
            DictFlashingProperties.TryGetValue(name, out prop);

            return prop;
        }

        public static CFlashingProperties GetFlashingProperties(int id)
        {
            CFlashingProperties properties = null;
            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["AccessoriesSQLiteDB"].ConnectionString))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("Select * from Flashing WHERE ID = @id", conn);
                command.Parameters.AddWithValue("@id", id);

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        properties = GetFlashingProperties(reader);
                    }
                }
            }
            return properties;
        }

        private static CFlashingProperties GetFlashingProperties(SQLiteDataReader reader)
        {
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";

            CFlashingProperties properties = new CFlashingProperties();
            properties.ID = reader.GetInt32(reader.GetOrdinal("ID"));
            properties.Name = reader["Name"].ToString();

            properties.Thickness = double.Parse(reader["thickness_mm"].ToString(), nfi) / 1000f;
            properties.Width_total = double.Parse(reader["width_total_mm"].ToString(), nfi) / 1000f;
            properties.Mass_kg_m2 = double.Parse(reader["mass_kg_m2"].ToString(), nfi);
            properties.Mass_kg_lm = double.Parse(reader["mass_kg_lm"].ToString(), nfi);
            properties.Price_PPLM_NZD = double.Parse(reader["price_PPLM_NZD"].ToString(), nfi);
            properties.Price_PPSM_NZD = double.Parse(reader["price_PPSM_NZD"].ToString(), nfi);
            properties.Price_PPKG_NZD = double.Parse(reader["price_PPKG_NZD"].ToString(), nfi);
            properties.Note = reader["note"].ToString();

            return properties;
        }
    }
}