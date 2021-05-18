using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SQLite;
using System.Globalization;
using System.Text.RegularExpressions;
using DATABASE.DTO;
using MATERIAL;

namespace DATABASE
{
    public static class CCladdingAccessoriesManager
    {
        // Cladding Accessories Item Manager

        public static List<CCladdingAccessories_Item_Properties> LoadItemProperties()
        {
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";

            CCladdingAccessories_Item_Properties item;
            List<CCladdingAccessories_Item_Properties> items = new List<CCladdingAccessories_Item_Properties>();

            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["CladdingAccessoriesSQLiteDB"].ConnectionString))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("Select * from Items", conn);
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        item = GetItemProperties(reader);
                        items.Add(item);
                    }
                }
            }
            return items;
        }

        public static List<string> GetItem_Names()
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
            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["CladdingAccessoriesSQLiteDB"].ConnectionString))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("Select * from Items WHERE ID = @id", conn);
                command.Parameters.AddWithValue("@id", id);

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        properties = GetItemProperties(reader);
                    }
                }
            }
            return properties;
        }

        public static CCladdingAccessories_Item_Properties GetItemProperties(string name)
        {
            CCladdingAccessories_Item_Properties properties = null;
            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["CladdingAccessoriesSQLiteDB"].ConnectionString))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("Select * from Items WHERE Name = @name", conn);
                command.Parameters.AddWithValue("@name", name);

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        properties = GetItemProperties(reader);
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
            properties.Default_spacing_m = reader["default_spacing_m"].ToString();
            properties.Standard = reader["Standard"].ToString();
            properties.IsFixingItem = reader["isFixingItem"].ToString() == "TRUE" ? true : false;
            properties.FixingIDs = reader["fixingIDs"].ToString();
            properties.Mass_kg = double.Parse(reader["mass_kg"].ToString(), nfi);
            properties.Price_PPP_NZD = double.Parse(reader["price_PPP_NZD"].ToString(), nfi);
            properties.Price_PPKG_NZD = double.Parse(reader["price_PPKG_NZD"].ToString(), nfi);
            properties.GCD_page = Int32.Parse(reader["GCD_page"].ToString());
            properties.Note1 = reader["Note1"].ToString();
            properties.Note2 = reader["Note2"].ToString();

            if(properties.FixingIDs != null && properties.FixingIDs !="") // Ak sa nejedna o prazdny string mozeme urcit IDs
               properties.FixingIDsArray = StringHelper.ConvertStringArrayOfIDs(properties.FixingIDs, ';');

            return properties;
        }

        // Cladding Accessories Fixing Manager

        public static List<CCladdingAccessories_Fixing_Properties> LoadFixingProperties()
        {
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";

            CCladdingAccessories_Fixing_Properties item;
            List<CCladdingAccessories_Fixing_Properties> items = new List<CCladdingAccessories_Fixing_Properties>();

            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["CladdingAccessoriesSQLiteDB"].ConnectionString))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("Select * from Fixing", conn);
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        item = GetFixingProperties(reader);
                        items.Add(item);
                    }
                }
            }
            return items;
        }

        public static List<string> GetFixing_Names()
        {
            List<CCladdingAccessories_Fixing_Properties> items = LoadFixingProperties();

            List<string> names = new List<string>();

            foreach (CCladdingAccessories_Fixing_Properties item in items)
                names.Add(item.Name);

            return names;
        }

        public static CCladdingAccessories_Fixing_Properties GetFixingProperties(int id)
        {
            CCladdingAccessories_Fixing_Properties properties = null;
            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["CladdingAccessoriesSQLiteDB"].ConnectionString))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("Select * from Fixing WHERE ID = @id", conn);
                command.Parameters.AddWithValue("@id", id);

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        properties = GetFixingProperties(reader);
                    }
                }
            }
            return properties;
        }

        public static CCladdingAccessories_Fixing_Properties GetFixingProperties(string name)
        {
            CCladdingAccessories_Fixing_Properties properties = null;
            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["CladdingAccessoriesSQLiteDB"].ConnectionString))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("Select * from Fixing WHERE Name = @name", conn);
                command.Parameters.AddWithValue("@name", name);

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        properties = GetFixingProperties(reader);
                    }
                }
            }
            return properties;
        }

        private static CCladdingAccessories_Fixing_Properties GetFixingProperties(SQLiteDataReader reader)
        {
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";

            CCladdingAccessories_Fixing_Properties properties = new CCladdingAccessories_Fixing_Properties();

            properties.ID = reader.GetInt32(reader.GetOrdinal("ID"));
            properties.Name = reader["Name"].ToString();
            properties.Standard = reader["Standard"].ToString();
            properties.Mass_kg = double.Parse(reader["mass_kg"].ToString(), nfi);
            properties.Price_PPP_NZD = double.Parse(reader["price_PPP_NZD"].ToString(), nfi);
            properties.Price_PPKG_NZD = double.Parse(reader["price_PPKG_NZD"].ToString(), nfi);
            properties.Note = reader["Note"].ToString();

            return properties;
        }
    }
}