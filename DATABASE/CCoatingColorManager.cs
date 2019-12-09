using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SQLite;
using System.Globalization;
using DATABASE.DTO;

namespace DATABASE
{
    public static class CCoatingColorManager
    {
        // "TrapezoidalSheetingSQLiteDB"
        // colours
        public static List<CoatingColours> LoadCoatingColours(string sDatabaseName, string sTableName = "colours")
        {
            CoatingColours colour;
            List<CoatingColours> items = new List<CoatingColours>();
            
            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings[sDatabaseName].ConnectionString))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("Select * from "+ sTableName, conn);
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {

                        colour = new CoatingColours();
                        colour.ID = reader.GetInt32(reader.GetOrdinal("ID"));
                        colour.Name = reader["name"].ToString();
                        colour.CodeRGB = reader["codeRGB"].ToString();
                        colour.CodeHEX = "#"+reader["codeHEX"].ToString();
                        colour.CodeHSV = reader["codeHSV"].ToString();
                        colour.PriceCode = Int32.Parse(reader["priceCode"].ToString());
                        items.Add(colour);
                    }
                }
            }
            return items;
        }
    }
}