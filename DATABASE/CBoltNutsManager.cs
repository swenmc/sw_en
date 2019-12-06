using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Data.SQLite;
using System.Globalization;
using DATABASE.DTO;

namespace DATABASE
{
    public static class CBoltNutsManager
    {
        public static Dictionary<string, CBoltNutProperties> DictBoltNutProperties;

        public static List<CBoltNutProperties> LoadBoltNutsProperties(string TableName)
        {
            CBoltNutProperties properties = null;
            List<CBoltNutProperties> items = new List<CBoltNutProperties>();

            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["BoltsSQLiteDB"].ConnectionString))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("Select * from "+ TableName, conn);
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        properties = GetBoltNutProperties(reader);

                        items.Add(properties);
                    }
                }
            }
            return items;
        }

        private static void LoadBoltNutsPropertiesDictionary(string TableName)
        {
            DictBoltNutProperties = new Dictionary<string, CBoltNutProperties>();
            
            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["BoltsSQLiteDB"].ConnectionString))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("Select * from " + TableName, conn);
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        CBoltNutProperties properties = GetBoltNutProperties(reader);
                        DictBoltNutProperties.Add(properties.Name, properties);
                    }
                }
            }
        }

        public static CBoltNutProperties GetBoltNutProperties(string name, string TableName)
        {
            if (DictBoltNutProperties == null) LoadBoltNutsPropertiesDictionary(TableName);

            CBoltNutProperties prop = null;
            DictBoltNutProperties.TryGetValue(name, out prop);

            return prop;
        }

        public static CBoltNutProperties GetBoltNutProperties(int id, string TableName)
        {
            CBoltNutProperties properties = null;
            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["BoltsSQLiteDB"].ConnectionString))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("Select * from" + TableName +" WHERE ID = @id", conn);
                command.Parameters.AddWithValue("@id", id);

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        properties = GetBoltNutProperties(reader);
                    }
                }
            }
            return properties;
        }

        private static CBoltNutProperties GetBoltNutProperties(SQLiteDataReader reader)
        {
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";

            CBoltNutProperties properties = new CBoltNutProperties();
            properties.ID = reader.GetInt32(reader.GetOrdinal("ID"));
            properties.Name = reader["Name"].ToString();
            //properties.Standard = reader["Standard"].ToString();
            properties.Pitch_coarse = reader["pitch_coarse"].ToString() == "" ? double.NaN : double.Parse(reader["pitch_coarse"].ToString(), nfi);
            properties.SizeAcrossFlats_max = reader["sizeAcrossFlats_max_mm"].ToString() == "" ? double.NaN : double.Parse(reader["sizeAcrossFlats_max_mm"].ToString(), nfi) / 1000f;
            properties.SizeAcrossFlats_min = reader["sizeAcrossFlats_min_mm"].ToString() == "" ? double.NaN : double.Parse(reader["sizeAcrossFlats_min_mm"].ToString(), nfi) / 1000f;
            properties.SizeAcrossCorners = reader["sizeAcrossCorners_mm"].ToString() == "" ? double.NaN : double.Parse(reader["sizeAcrossCorners_mm"].ToString(), nfi) / 1000f;
            properties.Thickness_max = reader["thickness_max_mm"].ToString() == "" ? double.NaN : double.Parse(reader["thickness_max_mm"].ToString(), nfi) / 1000f;
            properties.Thickness_min = reader["thickness_min_mm"].ToString() == "" ? double.NaN : double.Parse(reader["thickness_min_mm"].ToString(), nfi) / 1000f;
            properties.Mass = reader["mass_kg"].ToString() == "" ? double.NaN : double.Parse(reader["mass_kg"].ToString(), nfi);
            properties.Price_PPKG_NZD = reader["price_PPKG_NZD"].ToString() == "" ? double.NaN : double.Parse(reader["price_PPKG_NZD"].ToString(), nfi);
            properties.Price_PPP_NZD = reader["price_PPP_NZD"].ToString() == "" ? double.NaN : double.Parse(reader["price_PPP_NZD"].ToString(), nfi);

            return properties;
        }
    }
}