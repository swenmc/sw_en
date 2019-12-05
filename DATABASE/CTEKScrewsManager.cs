using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Data.SQLite;
using System.Globalization;
using DATABASE.DTO;

namespace DATABASE
{
    public static class CTEKScrewsManager
    {
        // String data
        public static Dictionary<string, CTEKScrewProperties> DictTEKScrewProperties;

        public static List<CTEKScrewProperties> LoadTEKScrewsProperties()
        {
            CTEKScrewProperties properties = null;
            List<CTEKScrewProperties> items = new List<CTEKScrewProperties>();

            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["TEKScrewsSQLiteDB"].ConnectionString))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("Select * from TEKScrews", conn);
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        properties = GetScrewProperties(reader);

                        items.Add(properties);
                    }
                }
            }
            return items;
        }

        private static void LoadTEKScrewsPropertiesDictionary()
        {
            DictTEKScrewProperties = new Dictionary<string, CTEKScrewProperties>();
            
            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["TEKScrewsSQLiteDB"].ConnectionString))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("Select * from TEKScrews", conn);
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        CTEKScrewProperties properties = GetScrewProperties(reader);
                        DictTEKScrewProperties.Add(properties.gauge, properties);
                    }
                }
            }
        }

        public static CTEKScrewProperties GetScrewProperties(string gauge)
        {
            if (DictTEKScrewProperties == null) LoadTEKScrewsPropertiesDictionary();

            CTEKScrewProperties prop = null;
            DictTEKScrewProperties.TryGetValue(gauge, out prop);

            return prop;
        }

        public static CTEKScrewProperties GetScrewProperties(int id)
        {
            CTEKScrewProperties properties = null;
            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["TEKScrewsSQLiteDB"].ConnectionString))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("Select * from TEKScrews WHERE ID = @id", conn);
                command.Parameters.AddWithValue("@id", id);

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        properties = GetScrewProperties(reader);
                    }
                }
            }
            return properties;
        }

        private static CTEKScrewProperties GetScrewProperties(SQLiteDataReader reader)
        {
            CTEKScrewProperties properties = new CTEKScrewProperties();
            properties.ID = reader.GetInt32(reader.GetOrdinal("ID"));
            properties.gauge = reader["gauge"].ToString();
            properties.threadDiameter = reader["threadDiameter"].ToString();
            properties.shankDiameter = reader["shankDiameter"].ToString();
            properties.shankLength = reader["shankLength"].ToString();
            properties.threadType1 = reader["threadType1"].ToString();
            properties.threadsPerInch1 = reader["threadsPerInch1"].ToString();
            properties.threadType2 = reader["threadType2"].ToString();
            properties.threadsPerInch2 = reader["threadsPerInch2"].ToString();
            properties.threadType3 = reader["threadType3"].ToString();
            properties.threadsPerInch3 = reader["threadsPerInch3"].ToString();
            properties.headSizeInch = reader["headSizeInch"].ToString();
            properties.headSizemm = reader["headSizemm"].ToString();
            properties.headThicknessmm = reader["headThicknessmm"].ToString();
            properties.washerSizemm = reader["washerSizemm"].ToString();
            properties.washerThicknessmm = reader["washerThicknessmm"].ToString();
            properties.preDrillHoleDiametermm_3mmthickness = reader["preDrillHoleDiametermm_3mmthickness"].ToString();
            properties.shearStrength_N = reader["ShearStrength_N"].ToString();
            properties.axialTensileStrength_N = reader["AxialTensileStrength_N"].ToString();
            properties.torsionalStrength_Nm = reader["TorsionalStrength_Nm"].ToString();
            properties.mass_kg = reader["mass_kg"].ToString();

            return properties;
        }

        // Numerical data
        public static CTEKScrewProp GetScrewProperties2(string gauge)
        {
            /*
            if (DictTEKScrewProperties == null) LoadTEKScrewsPropertiesDictionary();

            CTEKScrewProp prop = null;
            DictTEKScrewProperties.TryGetValue(gauge, out prop);

            return prop;
            */

            CTEKScrewProp properties = null;
            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["TEKScrewsSQLiteDB"].ConnectionString))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("Select * from TEKScrews WHERE gauge = @gauge", conn);
                command.Parameters.AddWithValue("@gauge", gauge);

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        properties = GetScrewProperties2(reader);
                    }
                }
            }
            return properties;
        }

        public static CTEKScrewProp GetScrewProperties2(int id)
        {
            CTEKScrewProp properties = null;
            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["TEKScrewsSQLiteDB"].ConnectionString))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("Select * from TEKScrews WHERE ID = @id", conn);
                command.Parameters.AddWithValue("@id", id);

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        properties = GetScrewProperties2(reader);
                    }
                }
            }
            return properties;
        }

        private static CTEKScrewProp GetScrewProperties2(SQLiteDataReader reader)
        {
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";

            CTEKScrewProp properties = new CTEKScrewProp();
            properties.ID = reader.GetInt32(reader.GetOrdinal("ID"));
            properties.gauge = reader["gauge"].ToString() == "" ? 0 : Int32.Parse(reader["gauge"].ToString()); // Gauge v mm
            properties.threadDiameter = reader["threadDiameter"].ToString() == "" ? double.NaN : double.Parse(reader["threadDiameter"].ToString(), nfi) / 1000f;
            properties.shankDiameter = reader["shankDiameter"].ToString() == "" ? double.NaN : double.Parse(reader["shankDiameter"].ToString(), nfi) / 1000f;
            properties.shankLength = reader["shankLength"].ToString() == "" ? double.NaN : double.Parse(reader["shankLength"].ToString(), nfi) / 1000f;
            properties.threadType1 = reader["threadType1"].ToString();
            properties.threadsPerInch1 = reader["threadsPerInch1"].ToString() == "" ? 0 : Int32.Parse(reader["threadsPerInch1"].ToString());
            properties.threadType2 = reader["threadType2"].ToString();
            properties.threadsPerInch2 = reader["threadsPerInch2"].ToString() == "" ? 0 : Int32.Parse(reader["threadsPerInch2"].ToString());
            properties.threadType3 = reader["threadType3"].ToString();
            properties.threadsPerInch3 = reader["threadsPerInch3"].ToString() == "" ? 0 : Int32.Parse(reader["threadsPerInch3"].ToString());
            properties.headSizeInch = reader["headSizeInch"].ToString() == "" ? double.NaN : double.Parse(reader["headSizeInch"].ToString(), nfi); // INCHES
            properties.headSize = reader["headSizemm"].ToString() == "" ? double.NaN : double.Parse(reader["headSizemm"].ToString(), nfi) / 1000f;
            properties.headThickness = reader["headThicknessmm"].ToString() == "" ? double.NaN : double.Parse(reader["headThicknessmm"].ToString(), nfi) / 1000f;
            properties.washerSize = reader["washerSizemm"].ToString() == "" ? double.NaN : double.Parse(reader["washerSizemm"].ToString(), nfi) / 1000f;
            properties.washerThickness = reader["washerThicknessmm"].ToString() == "" ? double.NaN : double.Parse(reader["washerThicknessmm"].ToString(), nfi) / 1000f;
            properties.preDrillHoleDiameter_3mmthickness = reader["preDrillHoleDiametermm_3mmthickness"].ToString() == "" ? double.NaN : double.Parse(reader["preDrillHoleDiametermm_3mmthickness"].ToString(), nfi) / 1000f;
            properties.shearStrength_N = reader["ShearStrength_N"].ToString() == "" ? double.NaN : double.Parse(reader["ShearStrength_N"].ToString(), nfi);
            properties.axialTensileStrength_N = reader["AxialTensileStrength_N"].ToString() == "" ? double.NaN : double.Parse(reader["AxialTensileStrength_N"].ToString(), nfi);
            properties.torsionalStrength_Nm = reader["TorsionalStrength_Nm"].ToString() == "" ? double.NaN : double.Parse(reader["TorsionalStrength_Nm"].ToString(), nfi);
            properties.mass_kg = reader["mass_kg"].ToString() == "" ? double.NaN : double.Parse(reader["mass_kg"].ToString(), nfi);
            properties.price_PPP_NZD = reader["price_PPP_NZD"].ToString() == "" ? double.NaN : double.Parse(reader["price_PPP_NZD"].ToString(), nfi);
            properties.price_PPKG_NZD = reader["price_PPKG_NZD"].ToString() == "" ? double.NaN : double.Parse(reader["price_PPKG_NZD"].ToString(), nfi);

            return properties;
        }
    }
}