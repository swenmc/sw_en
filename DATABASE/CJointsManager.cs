using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SQLite;
using System.Globalization;
using DATABASE.DTO;
using MATERIAL;

namespace DATABASE
{
    public static class CJointsManager
    {
        public static List<CConnectionDescription> LoadJointsConnectionDescriptions()
        {
            CConnectionDescription item;
            List<CConnectionDescription> items = new List<CConnectionDescription>();

            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["JointsSQLiteDB"].ConnectionString))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("Select * from connectionDescription", conn);
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        item = new CConnectionDescription();
                        item.ID = reader.GetInt32(reader.GetOrdinal("ID"));
                        item.Name = reader["name"].ToString();
                        item.JoinType = reader["jointType"].ToString();
                        item.Note = reader["note"].ToString();
                        items.Add(item);
                    }
                }
            }
            return items;
        }

        public static List<string> GetPlateSeries()
        {
            List<string> items = new List<string>();

            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["JointsSQLiteDB"].ConnectionString))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("Select * from plateSeries", conn);
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        items.Add(reader["serieName"].ToString());
                    }
                }
            }
            return items;
        }





        // PLATE SERIES B - BASE PLATES

        public static List<CPlate_B_Properties> LoadPlate_B_Descriptions()
        {
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";

            CPlate_B_Properties item;
            List<CPlate_B_Properties> items = new List<CPlate_B_Properties>();

            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["JointsSQLiteDB"].ConnectionString))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("Select * from plateSerieB", conn);
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        item = GetPlate_B_Properties(reader);
                        items.Add(item);
                    }
                }
            }
            return items;
        }

        public static List<string> GetPlateB_Names()
        {
            List<CPlate_B_Properties> items = LoadPlate_B_Descriptions();

            List<string> names = new List<string>();

            foreach (CPlate_B_Properties item in items)
                names.Add(item.Name);

            return names;
        }

        public static string[] GetArrayPlateB_Names()
        {
            string[] arr_Serie_B_Names = new string[13];
            List<string> list_Serie_B_Names = CJointsManager.GetPlateB_Names();

            arr_Serie_B_Names = list_Serie_B_Names.ToArray();

            return arr_Serie_B_Names;
        }

        public static CPlate_B_Properties GetSPlate_B_Properties(int id)
        {
            CPlate_B_Properties properties = null;
            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["JointsSQLiteDB"].ConnectionString))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("Select * from plateSerieB WHERE ID = @id", conn);
                command.Parameters.AddWithValue("@id", id);

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        properties = GetPlate_B_Properties(reader);
                    }
                }
            }
            return properties;
        }

        private static CPlate_B_Properties GetPlate_B_Properties(SQLiteDataReader reader)
        {
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";

            CPlate_B_Properties properties = new CPlate_B_Properties();
            properties.ID = reader.GetInt32(reader.GetOrdinal("ID"));
            properties.Name = reader["plateName"].ToString();
            properties.dim1 = double.Parse(reader["dim1"].ToString(), nfi);
            properties.dim2 = double.Parse(reader["dim2"].ToString(), nfi);
            properties.dim3 = double.Parse(reader["dim3"].ToString(), nfi);
            properties.t = double.Parse(reader["t"].ToString(), nfi);
            properties.iNumberHolesAnchors = Int32.Parse(reader["iNumberHolesAnchors"].ToString());
            properties.iNoOfAnchorsInRow = Int32.Parse(reader["iNoOfAnchorsInRow"].ToString());
            properties.iNoOfAnchorsInColumn = Int32.Parse(reader["iNoOfAnchorsInColumn"].ToString());
            properties.a1_pos_cp_x = double.Parse(reader["a1_pos_cp_x"].ToString(), nfi);
            properties.a1_pos_cp_y = double.Parse(reader["a1_pos_cp_y"].ToString(), nfi);
            properties.dist_x1 = double.Parse(reader["dist_x1"].ToString(), nfi);
            properties.dist_y1 = double.Parse(reader["dist_y1"].ToString(), nfi);
            properties.dist_x2 = reader["dist_x2"].ToString() == "" ? double.NaN : double.Parse(reader["dist_x2"].ToString(), nfi);
            properties.dist_y2 = reader["dist_y2"].ToString() == "" ? double.NaN : double.Parse(reader["dist_y2"].ToString(), nfi);

            return properties;
        }
    }
}
