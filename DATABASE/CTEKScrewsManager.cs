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
            properties.threadType1 = reader["threadType1"].ToString();
            properties.threadsPerInch1 = reader["threadsPerInch1"].ToString();
            properties.threadType2 = reader["threadType2"].ToString();
            properties.threadsPerInch2 = reader["threadsPerInch2"].ToString();
            properties.threadType3 = reader["threadType3"].ToString();
            properties.threadsPerInch3 = reader["threadsPerInch3"].ToString();
            properties.headSizeInch = reader["headSizeInch"].ToString();
            properties.headSizemm = reader["headSizemm"].ToString();
            properties.washerSizemm = reader["washerSizemm"].ToString();
            properties.washerThicknessmm = reader["washerThicknessmm"].ToString();
            properties.preDrillHoleDiametermm_3mmthickness = reader["preDrillHoleDiametermm_3mmthickness"].ToString();
            properties.shearStrength_N = reader["ShearStrength_N"].ToString();
            properties.axialTensileStrength_N = reader["AxialTensileStrength_N"].ToString();
            properties.torsionalStrength_Nm = reader["TorsionalStrength_Nm"].ToString();

            return properties;
        }
    }
}