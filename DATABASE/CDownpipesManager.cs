using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Data.SQLite;
using System.Globalization;
using DATABASE.DTO;

namespace DATABASE
{
    public static class CDownpipesManager
    {
        public static Dictionary<string, CDownpipeProperties> DictDownpipesProperties;

        public static List<CDownpipeProperties> LoadDownpipessProperties()
        {
            CDownpipeProperties properties = null;
            List<CDownpipeProperties> items = new List<CDownpipeProperties>();

            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["AccessoriesSQLiteDB"].ConnectionString))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("Select * from Downpipes", conn);
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        properties = GetDownpipesProperties(reader);

                        items.Add(properties);
                    }
                }
            }
            return items;
        }

        public static void LoadDownpipessPropertiesDictionary()
        {
            DictDownpipesProperties = new Dictionary<string, CDownpipeProperties>();
            
            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["AccessoriesSQLiteDB"].ConnectionString))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("Select * from Downpipes", conn);
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        CDownpipeProperties properties = GetDownpipesProperties(reader);
                        DictDownpipesProperties.Add(properties.Name, properties);
                    }
                }
            }
        }

        public static CDownpipeProperties GetDownpipesProperties(string name)
        {
            if (DictDownpipesProperties == null) LoadDownpipessPropertiesDictionary();

            CDownpipeProperties prop = null;
            DictDownpipesProperties.TryGetValue(name, out prop);

            return prop;
        }

        public static CDownpipeProperties GetDownpipesProperties(int id)
        {
            CDownpipeProperties properties = null;
            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["AccessoriesSQLiteDB"].ConnectionString))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("Select * from Downpipes WHERE ID = @id", conn);
                command.Parameters.AddWithValue("@id", id);

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        properties = GetDownpipesProperties(reader);
                    }
                }
            }
            return properties;
        }

        private static CDownpipeProperties GetDownpipesProperties(SQLiteDataReader reader)
        {
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";

            CDownpipeProperties properties = new CDownpipeProperties();
            properties.ID = reader.GetInt32(reader.GetOrdinal("ID"));
            properties.Name = reader["name"].ToString();
            properties.Shape = reader["shape"].ToString();
            properties.Diameter = double.Parse(reader["diameter_mm"].ToString(), nfi) / 1000f;
            properties.Length = double.Parse(reader["length_m"].ToString(), nfi);
            properties.Mass_kg_lm = double.Parse(reader["mass_kg_lm"].ToString(), nfi);
            properties.Mass_kg_piece = double.Parse(reader["mass_kg_piece"].ToString(), nfi);
            properties.Price_PPLM_NZD = double.Parse(reader["price_PPLM_NZD"].ToString(), nfi);
            properties.Price_PPP_NZD = double.Parse(reader["price_PPP_NZD"].ToString(), nfi);

            return properties;
        }
    }
}