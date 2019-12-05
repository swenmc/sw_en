using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SQLite;
using System.Globalization;
using DATABASE.DTO;
using MATERIAL;

namespace DATABASE
{
    public static class CWashersManager
    {
        // Rectangular Washers W

        public static List<CRectWasher_W_Properties> LoadPlate_W_Descriptions()
        {
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";

            CRectWasher_W_Properties item;
            List<CRectWasher_W_Properties> items = new List<CRectWasher_W_Properties>();

            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["WashersSQLiteDB"].ConnectionString))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("Select * from Washers", conn);
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        item = GetPlate_W_Properties(reader);
                        items.Add(item);
                    }
                }
            }
            return items;
        }

        public static List<string> GetPlateLL_Names()
        {
            List<CRectWasher_W_Properties> items = LoadPlate_W_Descriptions();

            List<string> names = new List<string>();

            foreach (CRectWasher_W_Properties item in items)
                names.Add(item.Name);

            return names;
        }

        public static CRectWasher_W_Properties GetPlate_W_Properties(int id)
        {
            CRectWasher_W_Properties properties = null;
            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["WashersSQLiteDB"].ConnectionString))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("Select * from Washers WHERE ID = @id", conn);
                command.Parameters.AddWithValue("@id", id);

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        properties = GetPlate_W_Properties(reader);
                    }
                }
            }
            return properties;
        }

        public static CRectWasher_W_Properties GetPlate_W_Properties(string name)
        {
            CRectWasher_W_Properties properties = null;
            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["WashersSQLiteDB"].ConnectionString))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("Select * from Washers WHERE washerName = @name", conn);
                command.Parameters.AddWithValue("@name", name);

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        properties = GetPlate_W_Properties(reader);
                    }
                }
            }
            return properties;
        }

        private static CRectWasher_W_Properties GetPlate_W_Properties(SQLiteDataReader reader)
        {
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";

            CRectWasher_W_Properties properties = new CRectWasher_W_Properties();
            properties.ID = reader.GetInt32(reader.GetOrdinal("ID"));
            properties.Name = reader["plateName"].ToString();
            properties.dim1x = double.Parse(reader["dim1x"].ToString(), nfi) / 1000;
            properties.dim2y = double.Parse(reader["dim2y"].ToString(), nfi) / 1000;
            properties.thickness = double.Parse(reader["t"].ToString(), nfi) / 1000;
            properties.NumberOfHoles = Int32.Parse(reader["iNumberHoles"].ToString());
            properties.Area = double.Parse(reader["area"].ToString(), nfi) / 1000000;
            properties.Volume = double.Parse(reader["volume"].ToString(), nfi) / 1000000000;
            properties.Mass = double.Parse(reader["mass"].ToString(), nfi);
            properties.Price_PPSM_NZD = double.Parse(reader["price_PPSM_NZD"].ToString(), nfi);
            properties.Price_PPKG_NZD = double.Parse(reader["price_PPKG_NZD"].ToString(), nfi);
            properties.Price_PPP_NZD = double.Parse(reader["price_PPP_NZD"].ToString(), nfi);

            return properties;
        }
    }
}
