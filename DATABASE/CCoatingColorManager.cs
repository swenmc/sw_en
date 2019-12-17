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
        public static List<CoatingColour> LoadColours(string sDatabaseName, string sTableName = "colours")
        {
            CoatingColour colour;
            List<CoatingColour> items = new List<CoatingColour>();
            
            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings[sDatabaseName].ConnectionString))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("Select * from "+ sTableName, conn);
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        colour = new CoatingColour();
                        colour = GetColorProperties(reader);
                        items.Add(colour);
                    }
                }
            }
            return items;
        }

        public static CoatingColour LoadCoatingProperties(int id)
        {
            CoatingColour properties = null;
            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["TrapezoidalSheetingSQLiteDB"].ConnectionString))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("Select * from colours WHERE ID = @id", conn);
                command.Parameters.AddWithValue("@id", id);

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        properties = GetColorProperties(reader);
                    }
                }
            }
            return properties;
        }

        public static CoatingColour GetColorProperties(SQLiteDataReader reader)
        {
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";

            CoatingColour colour = new CoatingColour();
            colour.ID = reader.GetInt32(reader.GetOrdinal("ID"));
            colour.Name = reader["name"].ToString();
            colour.CodeRGB = reader["codeRGB"].ToString();
            colour.CodeHEX = "#" + reader["codeHEX"].ToString();
            colour.CodeHSV = reader["codeHSV"].ToString();

            try
            {
                colour.PriceCode = Int32.Parse(reader["priceCode"].ToString());
            }
            catch
            {
                colour.PriceCode = -1; // Nie je definovane pre coating colors of trapezoidal sheets pretoze rovnaka farba moze mat inu cenu z dovodu ineho dodavatela, inej hrubky pozinkovania a podobne
            }

            return colour;
        }
    }
}