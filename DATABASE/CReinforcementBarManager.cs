using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SQLite;
using System.Globalization;
using DATABASE.DTO;
using MATERIAL;

namespace DATABASE
{
    public static class CReinforcementBarManager
    {
        // REINFORCEMENT BAR
        private static CReinforcementBarProperties GetReinforcementBarProperties(SQLiteDataReader reader)
        {
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";

            CReinforcementBarProperties rb = new CReinforcementBarProperties();
            rb.ID = reader.GetInt32(reader.GetOrdinal("ID"));
            rb.Diameter_mm = reader["diameter_mm"].ToString() == "" ? float.NaN : float.Parse(reader["diameter_mm"].ToString(), nfi);
            rb.Diameter_m = reader["diameter_m"].ToString() == "" ? float.NaN : float.Parse(reader["diameter_m"].ToString(), nfi);
            rb.Area_As1 = reader["Area_As1"].ToString() == "" ? float.NaN : float.Parse(reader["Area_As1"].ToString(), nfi);
            rb.Mass = reader["MassPerLength"].ToString() == "" ? float.NaN : float.Parse(reader["MassPerLength"].ToString(), nfi);

            return rb;
        }

        public static Dictionary<int, CReinforcementBarProperties> LoadReiforcementBarProperties()
        {
            CReinforcementBarProperties rb = null;
            Dictionary<int, CReinforcementBarProperties> items = new Dictionary<int, CReinforcementBarProperties>();

            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["MaterialsRCSQLiteDB"].ConnectionString))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("Select * from Reinforcement_Diameters", conn);
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        rb = GetReinforcementBarProperties(reader);
                        items.Add((int)rb.Diameter_mm, rb);
                    }
                }
            }
            return items;
        }

        public static CReinforcementBarProperties LoadMaterialPropertiesRF(int diameter_mm)
        {
            CReinforcementBarProperties properties = null;
            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["MaterialsRCSQLiteDB"].ConnectionString))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("Select * from Reinforcement_Diameters WHERE Diameter_mm = @Diameter_mm", conn);
                command.Parameters.AddWithValue("@Diameter_mm", diameter_mm);

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        properties = GetReinforcementBarProperties(reader);
                    }
                }
            }
            return properties;
        }
    }
}
