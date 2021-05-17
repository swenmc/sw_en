using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SQLite;
using System.Globalization;
using DATABASE.DTO;
using MATERIAL;

namespace DATABASE
{
    public static class CCladdingAccessoriesItemManager
    {
        // Cladding Accessories Item Manager

        public static List<CCladdingAccessories_Item_Properties> LoadItemProperties()
        {
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";

            CCladdingAccessories_Item_Properties item;
            List<CCladdingAccessories_Item_Properties> items = new List<CCladdingAccessories_Item_Properties>();

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

        public static List<string> GetPlateW_Names()
        {
            List<CCladdingAccessories_Item_Properties> items = LoadItemProperties();

            List<string> names = new List<string>();

            foreach (CCladdingAccessories_Item_Properties item in items)
                names.Add(item.Name);

            return names;
        }

        public static CCladdingAccessories_Item_Properties GetItemProperties(int id)
        {
            CCladdingAccessories_Item_Properties properties = null;
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

        public static CCladdingAccessories_Item_Properties GetItemProperties(string name)
        {
            CCladdingAccessories_Item_Properties properties = null;
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

        private static CCladdingAccessories_Item_Properties GetItemProperties(SQLiteDataReader reader)
        {
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";

            CCladdingAccessories_Item_Properties properties = new CCladdingAccessories_Item_Properties();

        properties.ID = reader.GetInt32(reader.GetOrdinal("ID"));
            properties.Name = double.Parse(reader[Name
        properties.Code = double.Parse(reader["code"].ToString(), nfi);
            properties.Default_spacing_m = double.Parse(reader["default_spacing_m"].ToString(), nfi);
            properties.Standard = double.Parse(reader["Standard"].ToString(), nfi);
            properties.IsFixingItem = double.Parse(reader["isFixingItem"].ToString(), nfi);
            properties.FixingIDs = double.Parse(reader["fixingIDs"].ToString(), nfi);
            properties.Mass_kg = double.Parse(reader["mass_kg"].ToString(), nfi);
            properties.Price_PPP_NZD = double.Parse(reader["price_PPP_NZD"].ToString(), nfi);
            properties.Price_PPKG_NZD = double.Parse(reader["price_PPKG_NZD"].ToString(), nfi);
            properties.GCD_page = double.Parse(reader["GCD_page"].ToString(), nfi);
            properties.Note1 = double.Parse(reader["Note1"].ToString(), nfi);
            properties.Note2 = double.Parse(reader["Note2"].ToString(), nfi);

        properties.ID = reader.GetInt32(reader.GetOrdinal("ID"));
            properties.Name = reader["washerName"].ToString();
            properties.dim1x = double.Parse(reader["dim1x_mm"].ToString(), nfi) / 1000;
            properties.dim2y = double.Parse(reader["dim2y_mm"].ToString(), nfi) / 1000;
            properties.thickness = double.Parse(reader["thickness_mm"].ToString(), nfi) / 1000;
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
