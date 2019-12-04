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
    // Flashing and gutters
    // Mozno aj Fibreglass ???

    public static class CFlashingsAndGuttersManager
    {
        public static Dictionary<string, CLengthItemProperties> DictFlashingProperties;

        public static List<CLengthItemProperties> LoadFlashingsProperties(string TableName)
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
                        properties = GetFlashingProperties(reader);

                        items.Add(properties);
                    }
                }
            }
            return items;
        }

        public static void LoadFlashingsPropertiesDictionary(string TableName)
        {
            DictFlashingProperties = new Dictionary<string, CLengthItemProperties>();
            
            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["AccessoriesSQLiteDB"].ConnectionString))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("Select * from " + TableName, conn);
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        CLengthItemProperties properties = GetFlashingProperties(reader);
                        DictFlashingProperties.Add(properties.Name, properties);
                    }
                }
            }
        }

        public static CLengthItemProperties GetFlashingProperties(string name, string TableName)
        {
            if (DictFlashingProperties == null) LoadFlashingsPropertiesDictionary(TableName);

            CLengthItemProperties prop = null;
            DictFlashingProperties.TryGetValue(name, out prop);

            return prop;
        }

        public static CLengthItemProperties GetFlashingProperties(int id, string TableName)
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
                        properties = GetFlashingProperties(reader);
                    }
                }
            }
            return properties;
        }

        private static CLengthItemProperties GetFlashingProperties(SQLiteDataReader reader)
        {
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";

            CLengthItemProperties properties = new CLengthItemProperties();
            properties.ID = reader.GetInt32(reader.GetOrdinal("ID"));
            properties.Name = reader["name"].ToString();
            properties.Thickness = double.Parse(reader["thickness_mm"].ToString(), nfi) / 1000f;

            try // TODO - doriesit ci dame do databazy len rozvinutu sirku width_total_mm alebo aj width_mm pre fibreglass
            {
                properties.Width_total = double.Parse(reader["width_total_mm"].ToString(), nfi) / 1000f;
            }
            catch
            {
                properties.Width_total = double.Parse(reader["width_mm"].ToString(), nfi) / 1000f;
            }

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