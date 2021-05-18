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
            properties.Name = reader["Name"].ToString();
            properties.Code = reader["code"].ToString();
            properties.Default_spacing_m = double.Parse(reader["default_spacing_m"].ToString(), nfi);
            properties.Standard = reader["Standard"].ToString();
            properties.IsFixingItem = bool.Parse(reader["isFixingItem"].ToString() == "TRUE" ? true : false);
            properties.FixingIDs = reader["fixingIDs"].ToString();
            properties.Mass_kg = double.Parse(reader["mass_kg"].ToString(), nfi);
            properties.Price_PPP_NZD = double.Parse(reader["price_PPP_NZD"].ToString(), nfi);
            properties.Price_PPKG_NZD = double.Parse(reader["price_PPKG_NZD"].ToString(), nfi);
            properties.GCD_page = Int32.Parse(reader["GCD_page"].ToString());
            properties.Note1 = reader["Note1"].ToString();
            properties.Note2 = reader["Note2"].ToString();

            return properties;
        }
    }
}
