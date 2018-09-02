using DATABASE.DTO;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Data.SQLite;

namespace DATABASE
{
    public static class CTrapezoidalSheetingManager
    {
        public static List<CTrapezoidalSheetingColours> LoadTrapezoidalSheetingColours()
        {
            CTrapezoidalSheetingColours colour;
            List<CTrapezoidalSheetingColours> items = new List<CTrapezoidalSheetingColours>();
            
            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["TrapezoidalSheetingSQLiteDB"].ConnectionString))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("Select * from colours", conn);
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {

                        colour = new CTrapezoidalSheetingColours();
                        colour.ID = reader.GetInt32(reader.GetOrdinal("ID"));
                        colour.Name = reader["name"].ToString();
                        colour.CodeRGB = reader["codeRGB"].ToString();
                        colour.CodeHEX = "#"+reader["codeHEX"].ToString();
                        colour.CodeHSV = reader["codeHSV"].ToString();
                        items.Add(colour);
                    }
                }
            }
            return items;
        }
    }
}