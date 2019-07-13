using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Data.SQLite;
using System.Globalization;
using DATABASE.DTO;

namespace DATABASE
{
    public static class CBoltsManager
    {
        public static Dictionary<string, CBoltProperties> DictBoltProperties;

        public static List<CBoltProperties> LoadBoltsProperties()
        {
            CBoltProperties properties = null;
            List<CBoltProperties> items = new List<CBoltProperties>();

            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["BoltsSQLiteDB"].ConnectionString))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("Select * from Bolts", conn);
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        properties = GetBoltProperties(reader);

                        items.Add(properties);
                    }
                }
            }
            return items;
        }

        private static void LoadBoltsPropertiesDictionary()
        {
            DictBoltProperties = new Dictionary<string, CBoltProperties>();
            
            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["BoltsSQLiteDB"].ConnectionString))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("Select * from Bolts", conn);
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        CBoltProperties properties = GetBoltProperties(reader);
                        DictBoltProperties.Add(properties.Name, properties);
                    }
                }
            }
        }

        public static CBoltProperties GetBoltProperties(string name)
        {
            if (DictBoltProperties == null) LoadBoltsPropertiesDictionary();

            CBoltProperties prop = null;
            DictBoltProperties.TryGetValue(name, out prop);

            return prop;
        }

        public static CBoltProperties GetBoltProperties(int id)
        {
            CBoltProperties properties = null;
            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["BoltsSQLiteDB"].ConnectionString))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("Select * from Bolts WHERE ID = @id", conn);
                command.Parameters.AddWithValue("@id", id);

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        properties = GetBoltProperties(reader);
                    }
                }
            }
            return properties;
        }

        private static CBoltProperties GetBoltProperties(SQLiteDataReader reader)
        {
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";

            CBoltProperties properties = new CBoltProperties();
            properties.ID = reader.GetInt32(reader.GetOrdinal("ID"));
            properties.Name = reader["Name"].ToString();
            properties.Standard = reader["Standard"].ToString();
            properties.ThreadDiameter = reader["threadDiameter"].ToString() == "" ? double.NaN : double.Parse(reader["threadDiameter"].ToString(), nfi) / 1000f;
            properties.ShankDiameter = reader["shankDiameter"].ToString() == "" ? double.NaN : double.Parse(reader["shankDiameter"].ToString(), nfi) / 1000f;
            properties.PitchDiameter = reader["pitchDiameter"].ToString() == "" ? double.NaN : double.Parse(reader["pitchDiameter"].ToString(), nfi) / 1000f;
            properties.Pitch_coarse = reader["pitch_coarse"].ToString() == "" ? double.NaN : double.Parse(reader["pitch_coarse"].ToString(), nfi) / 1000f;
            properties.Pitch_fine = reader["pitch_fine"].ToString() == "" ? double.NaN : double.Parse(reader["pitch_fine"].ToString(), nfi) / 1000f;
            properties.Code = reader["code"].ToString();
            properties.ThreadAngle_deg = reader["threadAngle"].ToString() == "" ? double.NaN : double.Parse(reader["threadAngle"].ToString(), nfi);
            properties.H = reader["H"].ToString() == "" ? double.NaN : double.Parse(reader["H"].ToString(), nfi) / 1000f;

            return properties;
        }
    }
}