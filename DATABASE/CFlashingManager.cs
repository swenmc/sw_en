using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SQLite;
using System.Globalization;
using DATABASE.DTO;

namespace DATABASE
{
    public static class CFlashingManager
    {
        private static Dictionary<string, CFlashingItemProperties> items = null;

        public static Dictionary<string, CFlashingItemProperties> LoadFlashingProperties()
        {
            if (items != null) return items;
            CFlashingItemProperties fl = null;
            items = new Dictionary<string, CFlashingItemProperties>();

            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["Accessories_FlashingsAndGuttersSQLiteDB"].ConnectionString))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("Select * from Flashings", conn);
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        fl = GetFlashingProperties(reader);
                        items.Add(fl.Prefix, fl);
                    }
                }
            }
            return items;
        }

        public static CFlashingItemProperties GetFlashingProperties(string flPrefix)
        {
            Dictionary<string, CFlashingItemProperties> dictItems = LoadFlashingProperties();

            CFlashingItemProperties fl = null;
            dictItems.TryGetValue(flPrefix, out fl);
            return fl;
        }

        public static CFlashingItemProperties GetFlashingProperties(int id)
        {
            CFlashingItemProperties properties = null;
            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["Accessories_FlashingsAndGuttersSQLiteDB"].ConnectionString))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("Select * from Flashings WHERE ID = @id", conn);
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

        public static CFlashingItemProperties LoadFlashingProperties_meters(string flPrefix)
        {
            CFlashingItemProperties fl = new CFlashingItemProperties();

            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["Accessories_FlashingsAndGuttersSQLiteDB"].ConnectionString))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("Select * from Flashings WHERE Prefix = @Prefix", conn);
                command.Parameters.AddWithValue("@Prefix", flPrefix);

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        fl = GetFlashingProperties(reader);
                    }
                }
            }
            return fl;
        }

        private static CFlashingItemProperties GetFlashingProperties(SQLiteDataReader reader)
        {
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";

            CFlashingItemProperties fl = new CFlashingItemProperties();

            fl.ID = reader.GetInt32(reader.GetOrdinal("ID"));
            fl.Prefix = reader["Prefix"].ToString();
            fl.Type_ID = reader.GetInt32(reader.GetOrdinal("Type_ID"));
            fl.Type_Name = reader["Type_Name"].ToString();
            fl.Group_ID = reader.GetInt32(reader.GetOrdinal("Group_ID"));
            fl.Group_Name = reader["Group_Name"].ToString();
            fl.Elements_snakeModel_deg_mm = reader["elements_snakeModel_deg_mm"].ToString();
            fl.ArrElements_snakeModel_deg_mm = StringHelper.ConvertStringArray(fl.Elements_snakeModel_deg_mm, ';');
            fl.Width_total = double.Parse(reader["total_width_mm"].ToString(), nfi) / 1000; // m
            fl.Thickness = double.Parse(reader["thickness_mm"].ToString(), nfi) / 1000; // m
            fl.Mass_kg_lm = double.Parse(reader["mass_kg_per_m"].ToString(), nfi);
            fl.Price_PPLM_NZD = double.Parse(reader["price_PPLM_NZD"].ToString(), nfi);
            fl.Price_PPKG_NZD = double.Parse(reader["price_PPKG_NZD"].ToString(), nfi);
            fl.Note = reader["Note"].ToString();

            return fl;
        }
    }
}