using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SQLite;
using System.Globalization;
using DATABASE.DTO;

namespace DATABASE
{
    public static class CFibreglassManager
    {
        private static Dictionary<string, CFibreglassProperties> items = null;

        public static Dictionary<string, CFibreglassProperties> LoadFibreglassProperties()
        {
            if (items != null) return items;
            CFibreglassProperties fg = null;
            items = new Dictionary<string, CFibreglassProperties>();

            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["FibreglassSQLiteDB"].ConnectionString))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("Select * from tableFibreglass_m", conn); // Nacitavame v metroch
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        fg = GetCFibreglassProperties(reader);
                        items.Add(fg.name, fg);
                    }
                }
            }
            return items;
        }

        public static CFibreglassProperties GetFibreglassProperties(string fgName)
        {
            Dictionary<string, CFibreglassProperties> dictItems = LoadFibreglassProperties();

            CFibreglassProperties fg = null;
            dictItems.TryGetValue(fgName, out fg);
            return fg;
        }

        public static CFibreglassProperties LoadFibreglassProperties_meters(string fgName)
        {
            CFibreglassProperties fg = new CFibreglassProperties();

            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["FibreglassSQLiteDB"].ConnectionString))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("Select * from tableFibreglass_m WHERE name = @name", conn);
                command.Parameters.AddWithValue("@name", fgName);

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        fg = GetCFibreglassProperties(reader);
                    }
                }
            }
            return fg;
        }

        private static CFibreglassProperties GetCFibreglassProperties(SQLiteDataReader reader)
        {
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";

            CFibreglassProperties fg = new CFibreglassProperties();
            fg.DatabaseID = reader.GetInt32(reader.GetOrdinal("ID"));
            fg.name = reader["name"].ToString();
            fg.widthCoil_m = double.Parse(reader["widthCoil_m"].ToString(), nfi);
            fg.widthTot_m = double.Parse(reader["widthTot_m"].ToString(), nfi);
            fg.widthModular_m = double.Parse(reader["widthModular_m"].ToString(), nfi);
            fg.widthRib_m = double.Parse(reader["widthRib_m"].ToString(), nfi);
            fg.widthUpRib_m = double.Parse(reader["widthUpRib_m"].ToString(), nfi);
            fg.height_m = double.Parse(reader["height_m"].ToString(), nfi);
            fg.thickness_for_name = reader["thickness_for_name"].ToString();
            fg.thickness_m = double.Parse(reader["thickness_m"].ToString(), nfi);
            fg.flatsheet_mass_g_m2 = double.Parse(reader["flatsheet_mass_g_m2"].ToString(), nfi);
            fg.flatsheet_mass_kg_m2 = double.Parse(reader["flatsheet_mass_kg_m2"].ToString(), nfi);
            fg.mass_kg_m2 = double.Parse(reader["mass_kg_m2"].ToString(), nfi);
            fg.mass_kg_lm = double.Parse(reader["mass_kg_lm"].ToString(), nfi);
            fg.price_PPSM_NZD = double.Parse(reader["price_PPSM_NZD"].ToString(), nfi);
            fg.price_PPLM_NZ = double.Parse(reader["price_PPLM_NZD"].ToString(), nfi);
            fg.price_PPKG_NZD = double.Parse(reader["price_PPKG_NZD"].ToString(), nfi);

            return fg;
        }
    }
}