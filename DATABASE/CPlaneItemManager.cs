using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Data.SQLite;
using System.Globalization;
using DATABASE.DTO;

namespace DATABASE
{
    // TODO Prerobit na obecne nacitanie cien elementov ktore su definovane primarne plochou
    // 2D items
    // Doors, Windows, Roof Netting

    public static class CPlaneItemManager
    {
        public static Dictionary<string, CPlaneItemProperties> DictPlaneItemProperties;

        public static List<CPlaneItemProperties> LoadPlaneItemsProperties(string TableName)
        {
            CPlaneItemProperties properties = null;
            List<CPlaneItemProperties> items = new List<CPlaneItemProperties>();

            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["AccessoriesSQLiteDB"].ConnectionString))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("Select * from "+ TableName, conn);
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        properties = GetPlaneItemProperties(reader);

                        items.Add(properties);
                    }
                }
            }
            return items;
        }

        public static void LoadPlaneItemsPropertiesDictionary(string TableName)
        {
            DictPlaneItemProperties = new Dictionary<string, CPlaneItemProperties>();
            
            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["AccessoriesSQLiteDB"].ConnectionString))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("Select * from " + TableName, conn);
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        CPlaneItemProperties properties = GetPlaneItemProperties(reader);
                        DictPlaneItemProperties.Add(properties.Name, properties);
                    }
                }
            }
        }

        public static CPlaneItemProperties GetPlaneItemProperties(string name, string TableName)
        {
            if (DictPlaneItemProperties == null) LoadPlaneItemsPropertiesDictionary(TableName);

            CPlaneItemProperties prop = null;
            DictPlaneItemProperties.TryGetValue(name, out prop);

            return prop;
        }

        public static CPlaneItemProperties GetPlaneItemProperties(int id, string TableName)
        {
            CPlaneItemProperties properties = null;
            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["AccessoriesSQLiteDB"].ConnectionString))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("Select * from "+ TableName + " WHERE ID = @id", conn);
                command.Parameters.AddWithValue("@id", id);

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        properties = GetPlaneItemProperties(reader);
                    }
                }
            }
            return properties;
        }

        private static CPlaneItemProperties GetPlaneItemProperties(SQLiteDataReader reader)
        {
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";

            CPlaneItemProperties properties = new CPlaneItemProperties();
            properties.ID = reader.GetInt32(reader.GetOrdinal("ID"));
            properties.Name = reader["name"].ToString();

            try
            {
                properties.Thickness = double.Parse(reader["thickness_mm"].ToString(), nfi) / 1000f;
            }
            catch
            {
                properties.Thickness = 0; // TODO - nie vsetky plosne elementy maju definovanu hrubku
            }

            properties.Mass_kg_m2 = double.Parse(reader["mass_kg_m2"].ToString(), nfi);
            properties.Price_PPSM_NZD = double.Parse(reader["price_PPSM_NZD"].ToString(), nfi);
            properties.Price_PPKG_NZD = double.Parse(reader["price_PPKG_NZD"].ToString(), nfi);
            properties.Note = reader["note"].ToString();

            return properties;
        }
    }
}