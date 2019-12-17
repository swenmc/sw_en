using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Data.SQLite;
using System.Globalization;
using DATABASE.DTO;

namespace DATABASE
{
    // TODO Prerobit na obecne nacitanie cien elementov ktore su definovane primarne dlzkou
    // 1D items
    // LengthItem and gutters
    // Mozno aj Fibreglass ???

    public static class CLengthItemManager
    {
        public static Dictionary<string, CLengthItemProperties> DictLengthItemProperties;

        public static List<CLengthItemProperties> LoadLengthItemsProperties(string TableName)
        {
            CLengthItemProperties properties = null;
            List<CLengthItemProperties> items = new List<CLengthItemProperties>();

            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["AccessoriesSQLiteDB"].ConnectionString))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("Select * from "+ TableName, conn);
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        properties = GetLengthItemProperties(reader);

                        items.Add(properties);
                    }
                }
            }
            return items;
        }

        public static void LoadLengthItemsPropertiesDictionary(string TableName)
        {
            DictLengthItemProperties = new Dictionary<string, CLengthItemProperties>();
            
            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["AccessoriesSQLiteDB"].ConnectionString))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("Select * from " + TableName, conn);
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        CLengthItemProperties properties = GetLengthItemProperties(reader);
                        DictLengthItemProperties.Add(properties.Name, properties);
                    }
                }
            }
        }

        public static CLengthItemProperties GetLengthItemProperties(string name, string TableName)
        {
            /*if (DictLengthItemProperties == null)*/ LoadLengthItemsPropertiesDictionary(TableName);

            CLengthItemProperties prop = null;
            DictLengthItemProperties.TryGetValue(name, out prop);

            return prop;
        }

        public static CLengthItemProperties GetLengthItemProperties(int id, string TableName)
        {
            CLengthItemProperties properties = null;
            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["AccessoriesSQLiteDB"].ConnectionString))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("Select * from "+ TableName + " WHERE ID = @id", conn);
                command.Parameters.AddWithValue("@id", id);

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        properties = GetLengthItemProperties(reader);
                    }
                }
            }
            return properties;
        }

        private static CLengthItemProperties GetLengthItemProperties(SQLiteDataReader reader)
        {
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";

            CLengthItemProperties properties = new CLengthItemProperties();
            properties.ID = reader.GetInt32(reader.GetOrdinal("ID"));
            properties.Name = reader["name"].ToString();
            properties.Thickness = double.Parse(reader["thickness_mm"].ToString(), nfi) / 1000f;
            properties.Width_total = double.Parse(reader["width_total_mm"].ToString(), nfi) / 1000f;

            properties.Density1_kg_m3 = double.Parse(reader["density1_kg_m3"].ToString(), nfi);
            properties.Mass1_kg_m2 = double.Parse(reader["mass1_kg_m2"].ToString(), nfi);
            properties.Mass1_kg_lm = double.Parse(reader["mass1_kg_lm"].ToString(), nfi);
            properties.Price1_PPLM_NZD = double.Parse(reader["price1_PPLM_NZD"].ToString(), nfi);
            properties.Price1_PPSM_NZD = double.Parse(reader["price1_PPSM_NZD"].ToString(), nfi);
            properties.Price1_PPKG_NZD = double.Parse(reader["price1_PPKG_NZD"].ToString(), nfi);

            properties.Density4_kg_m3 = double.Parse(reader["density4_kg_m3"].ToString(), nfi);
            properties.Mass4_kg_m2 = double.Parse(reader["mass4_kg_m2"].ToString(), nfi);
            properties.Mass4_kg_lm = double.Parse(reader["mass4_kg_lm"].ToString(), nfi);
            properties.Price4_PPLM_NZD = double.Parse(reader["price4_PPLM_NZD"].ToString(), nfi);
            properties.Price4_PPSM_NZD = double.Parse(reader["price4_PPSM_NZD"].ToString(), nfi);
            properties.Price4_PPKG_NZD = double.Parse(reader["price4_PPKG_NZD"].ToString(), nfi);

            properties.Note = reader["note"].ToString();

            return properties;
        }
    }
}