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
        private static Dictionary<string, CPlate_B_Properties> plateB_properties = new Dictionary<string, CPlate_B_Properties>();
        private static Dictionary<string, CPlate_F_Properties> plateF_properties = new Dictionary<string, CPlate_F_Properties>();
        private static Dictionary<string, CPlate_L_Properties> plateL_properties = new Dictionary<string, CPlate_L_Properties>();
        private static Dictionary<string, CPlate_LL_Properties> plateLL_properties = new Dictionary<string, CPlate_LL_Properties>();

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
                        item.MainMemberPrefix_FS = reader["mainMemberPrefix_FS"].ToString();
                        item.SecondaryMemberPrefix_FS = reader["secondaryMemberPrefix_FS"].ToString();
                        item.SecondaryMember2Prefix_FS = reader["secondaryMember2Prefix_FS"].ToString();
                        item.MainMemberPrefix_FS_position = reader["mainMemberPrefix_FS_position"].ToString();
                        item.SecondaryMemberPrefix_FS_position = reader["secondaryMemberPrefix_FS_position"].ToString();
                        item.SecondaryMember2Prefix_FS_position = reader["secondaryMember2Prefix_FS_position"].ToString();
                        item.MainMemberPrefix_FS_ID = Int32.Parse(reader["mainMemberPrefix_FS_ID"].ToString());
                        item.SecondaryMemberPrefix_FS_ID = item.SecondaryMemberPrefix_FS =="" ? -1 : Int32.Parse(reader["secondaryMemberPrefix_FS_ID"].ToString());
                        item.SecondaryMember2Prefix_FS_ID = item.SecondaryMember2Prefix_FS == "" ? -1 : Int32.Parse(reader["secondaryMember2Prefix_FS_ID"].ToString());
                        item.MainMemberPrefix_FS_position_ID = Int32.Parse(reader["mainMemberPrefix_FS_position_ID"].ToString());
                        item.SecondaryMemberPrefix_FS_position_ID = item.SecondaryMemberPrefix_FS_position == "" ? -1 : Int32.Parse(reader["secondaryMemberPrefix_FS_position_ID"].ToString());
                        item.SecondaryMember2Prefix_FS_position_ID = item.SecondaryMember2Prefix_FS_position == "" ? -1 : Int32.Parse(reader["secondaryMember2Prefix_FS_position_ID"].ToString());
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

        public static CPlate_B_Properties GetPlate_B_Properties(int id)
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


        public static CPlate_B_Properties GetPlate_B_Properties(string name)
        {
            CPlate_B_Properties properties = null;

            if (plateB_properties.TryGetValue(name, out properties)) return properties;

            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["JointsSQLiteDB"].ConnectionString))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("Select * from plateSerieB WHERE plateName = @name", conn);
                command.Parameters.AddWithValue("@name", name);

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        properties = GetPlate_B_Properties(reader);
                        plateB_properties.Add(name, properties);
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
            properties.dim1 = double.Parse(reader["dim1"].ToString(), nfi) / 1000;
            properties.dim2y = double.Parse(reader["dim2y"].ToString(), nfi) / 1000;
            properties.dim3 = double.Parse(reader["dim3"].ToString(), nfi) / 1000;
            properties.t = double.Parse(reader["t"].ToString(), nfi) / 1000;
            properties.iNumberHolesAnchors = Int32.Parse(reader["iNumberHolesAnchors"].ToString());
            properties.iNoOfAnchorsInRow = Int32.Parse(reader["iNoOfAnchorsInRow"].ToString());
            properties.iNoOfAnchorsInColumn = Int32.Parse(reader["iNoOfAnchorsInColumn"].ToString());
            properties.a1_pos_cp_x = double.Parse(reader["a1_pos_cp_x"].ToString(), nfi) / 1000;
            properties.a1_pos_cp_y = double.Parse(reader["a1_pos_cp_y"].ToString(), nfi) / 1000;
            properties.dist_x1 = double.Parse(reader["dist_x1"].ToString(), nfi) / 1000;
            properties.dist_y1 = double.Parse(reader["dist_y1"].ToString(), nfi) / 1000;
            properties.dist_x2 = reader["dist_x2"].ToString() == "" ? double.NaN : double.Parse(reader["dist_x2"].ToString(), nfi) / 1000;
            properties.dist_y2 = reader["dist_y2"].ToString() == "" ? double.NaN : double.Parse(reader["dist_y2"].ToString(), nfi) / 1000;
            properties.TotalDim_x = double.Parse(reader["totalDim_x"].ToString(), nfi) / 1000;
            properties.TotalDim_y = double.Parse(reader["totalDim_y"].ToString(), nfi) / 1000;
            properties.Area = double.Parse(reader["area"].ToString(), nfi) / 1000000;
            properties.Volume = double.Parse(reader["volume"].ToString(), nfi) / 1000000000;
            properties.Mass = double.Parse(reader["mass"].ToString(), nfi);
            properties.Price_PPSM_NZD = double.Parse(reader["price_PPSM_NZD"].ToString(), nfi);
            properties.Price_PPKG_NZD = double.Parse(reader["price_PPKG_NZD"].ToString(), nfi);
            properties.Price_PPP_NZD = double.Parse(reader["price_PPP_NZD"].ToString(), nfi);
            return properties;
        }

        // Task 413
        // TO Ondrej - Dalo by sa to nejako zjednodusit alebo zrefaktorovat? aby som tu nemal tolko podobnych a skoro rovnakych funkcii
        // Pozri sa na plates v databaze joints
        // Plate Serie B ma viac premmenych ale ostatne maju skoro vsetko rovnake

        // Rozdiel u jednotlivych plate series je v tom, ze niektore maju ine parametre, 80% je rovnakych
        // Dalo by sa to urobit tak ze by som vyrobil string s nazvami premmennych ktore chcem z databazy, zadal by som nazov tabulky a univerzalne by mi to vracalo nejaky zoznam hodnot ?

        // TOTO maju vsetky plates

        /*
        private int m_ID;
        private string m_Name;
        private double m_thickness;
        private int m_iNumberOfHolesScrews;
        private double m_totalDim_x;
        private double m_totalDim_y;
        private double m_area;
        private double m_volume;
        private double m_mass;
        private double m_price_PPSM_NZD;
        private double m_price_PPKG_NZD;
        private double m_price_PPP_NZD;
        */


        // PLATE SERIES L

        public static List<CPlate_L_Properties> LoadPlate_L_Descriptions()
        {
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";

            CPlate_L_Properties item;
            List<CPlate_L_Properties> items = new List<CPlate_L_Properties>();

            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["JointsSQLiteDB"].ConnectionString))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("Select * from plateSerieL", conn);
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        item = GetPlate_L_Properties(reader);
                        items.Add(item);
                    }
                }
            }
            return items;
        }

        public static List<string> GetPlateL_Names()
        {
            List<CPlate_L_Properties> items = LoadPlate_L_Descriptions();

            List<string> names = new List<string>();

            foreach (CPlate_L_Properties item in items)
                names.Add(item.Name);

            return names;
        }

        public static CPlate_L_Properties GetPlate_L_Properties(int id)
        {
            CPlate_L_Properties properties = null;
            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["JointsSQLiteDB"].ConnectionString))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("Select * from plateSerieL WHERE ID = @id", conn);
                command.Parameters.AddWithValue("@id", id);

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        properties = GetPlate_L_Properties(reader);
                    }
                }
            }
            return properties;
        }

        public static CPlate_L_Properties GetPlate_L_Properties(string name)
        {
            CPlate_L_Properties properties = null;
            if (plateL_properties.TryGetValue(name, out properties)) return properties;

            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["JointsSQLiteDB"].ConnectionString))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("Select * from plateSerieL WHERE plateName = @name", conn);
                command.Parameters.AddWithValue("@name", name);

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        properties = GetPlate_L_Properties(reader);
                        plateL_properties.Add(name, properties);
                    }
                }
            }
            return properties;
        }

        private static CPlate_L_Properties GetPlate_L_Properties(SQLiteDataReader reader)
        {
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";

            CPlate_L_Properties properties = new CPlate_L_Properties();
            properties.ID = reader.GetInt32(reader.GetOrdinal("ID"));
            properties.Name = reader["plateName"].ToString();
            properties.dim1 = double.Parse(reader["dim1"].ToString(), nfi) / 1000;
            properties.dim2y = double.Parse(reader["dim2y"].ToString(), nfi) / 1000;
            properties.dim3 = double.Parse(reader["dim3"].ToString(), nfi) / 1000;
            properties.thickness = double.Parse(reader["t"].ToString(), nfi) / 1000;
            properties.NumberOfHolesScrews = Int32.Parse(reader["iNumberHolesScrews"].ToString());
            properties.TotalDim_x = double.Parse(reader["totalDim_x"].ToString(), nfi) / 1000;
            properties.TotalDim_y = double.Parse(reader["totalDim_y"].ToString(), nfi) / 1000;
            properties.Area = double.Parse(reader["area"].ToString(), nfi) / 1000000;
            properties.Volume = double.Parse(reader["volume"].ToString(), nfi) / 1000000000;
            properties.Mass = double.Parse(reader["mass"].ToString(), nfi);
            properties.Price_PPSM_NZD = double.Parse(reader["price_PPSM_NZD"].ToString(), nfi);
            properties.Price_PPKG_NZD = double.Parse(reader["price_PPKG_NZD"].ToString(), nfi);
            properties.Price_PPP_NZD = double.Parse(reader["price_PPP_NZD"].ToString(), nfi);

            return properties;
        }

        // PLATE SERIES F

        public static List<CPlate_F_Properties> LoadPlate_F_Descriptions()
        {
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";

            CPlate_F_Properties item;
            List<CPlate_F_Properties> items = new List<CPlate_F_Properties>();

            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["JointsSQLiteDB"].ConnectionString))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("Select * from plateSerieF", conn);
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        item = GetPlate_F_Properties(reader);
                        items.Add(item);
                    }
                }
            }
            return items;
        }

        public static List<string> GetPlateF_Names()
        {
            List<CPlate_F_Properties> items = LoadPlate_F_Descriptions();

            List<string> names = new List<string>();

            foreach (CPlate_F_Properties item in items)
                names.Add(item.Name);

            return names;
        }

        public static CPlate_F_Properties GetPlate_F_Properties(int id)
        {
            CPlate_F_Properties properties = null;
            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["JointsSQLiteDB"].ConnectionString))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("Select * from plateSerieF WHERE ID = @id", conn);
                command.Parameters.AddWithValue("@id", id);

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        properties = GetPlate_F_Properties(reader);
                    }
                }
            }
            return properties;
        }

        public static CPlate_F_Properties GetPlate_F_Properties(string name)
        {
            CPlate_F_Properties properties = null;
            if (plateF_properties.TryGetValue(name, out properties)) return properties;
            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["JointsSQLiteDB"].ConnectionString))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("Select * from plateSerieF WHERE plateName = @name", conn);
                command.Parameters.AddWithValue("@name", name);

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        properties = GetPlate_F_Properties(reader);
                        plateF_properties.Add(name, properties);
                    }
                }
            }
            return properties;
        }

        private static CPlate_F_Properties GetPlate_F_Properties(SQLiteDataReader reader)
        {
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";

            CPlate_F_Properties properties = new CPlate_F_Properties();
            properties.ID = reader.GetInt32(reader.GetOrdinal("ID"));
            properties.Name = reader["plateName"].ToString();
            properties.dim11 = double.Parse(reader["dim11"].ToString(), nfi) / 1000;
            properties.dim12 = double.Parse(reader["dim12"].ToString(), nfi) / 1000;
            properties.dim2y = double.Parse(reader["dim2y"].ToString(), nfi) / 1000;
            properties.dim3 = double.Parse(reader["dim3"].ToString(), nfi) / 1000;
            properties.thickness = double.Parse(reader["t"].ToString(), nfi) / 1000;
            properties.NumberOfHolesScrews = Int32.Parse(reader["iNumberHolesScrews"].ToString());
            properties.TotalDim_x = double.Parse(reader["totalDim_x"].ToString(), nfi) / 1000;
            properties.TotalDim_y = double.Parse(reader["totalDim_y"].ToString(), nfi) / 1000;
            properties.Area = double.Parse(reader["area"].ToString(), nfi) / 1000000;
            properties.Volume = double.Parse(reader["volume"].ToString(), nfi) / 1000000000;
            properties.Mass = double.Parse(reader["mass"].ToString(), nfi);
            properties.Price_PPSM_NZD = double.Parse(reader["price_PPSM_NZD"].ToString(), nfi);
            properties.Price_PPKG_NZD = double.Parse(reader["price_PPKG_NZD"].ToString(), nfi);
            properties.Price_PPP_NZD = double.Parse(reader["price_PPP_NZD"].ToString(), nfi);

            return properties;
        }

        // PLATE SERIES LL

        public static List<CPlate_LL_Properties> LoadPlate_LL_Descriptions()
        {
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";

            CPlate_LL_Properties item;
            List<CPlate_LL_Properties> items = new List<CPlate_LL_Properties>();

            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["JointsSQLiteDB"].ConnectionString))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("Select * from plateSerieLL", conn);
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        item = GetPlate_LL_Properties(reader);
                        items.Add(item);
                    }
                }
            }
            return items;
        }

        public static List<string> GetPlateLL_Names()
        {
            List<CPlate_LL_Properties> items = LoadPlate_LL_Descriptions();

            List<string> names = new List<string>();

            foreach (CPlate_LL_Properties item in items)
                names.Add(item.Name);

            return names;
        }

        public static CPlate_LL_Properties GetPlate_LL_Properties(int id)
        {
            CPlate_LL_Properties properties = null;
            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["JointsSQLiteDB"].ConnectionString))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("Select * from plateSerieLL WHERE ID = @id", conn);
                command.Parameters.AddWithValue("@id", id);

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        properties = GetPlate_LL_Properties(reader);
                    }
                }
            }
            return properties;
        }

        public static CPlate_LL_Properties GetPlate_LL_Properties(string name)
        {
            CPlate_LL_Properties properties = null;
            if (plateLL_properties.TryGetValue(name, out properties)) return properties;
            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["JointsSQLiteDB"].ConnectionString))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("Select * from plateSerieLL WHERE plateName = @name", conn);
                command.Parameters.AddWithValue("@name", name);

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        properties = GetPlate_LL_Properties(reader);
                        plateLL_properties.Add(name, properties);
                    }
                }
            }
            return properties;
        }

        private static CPlate_LL_Properties GetPlate_LL_Properties(SQLiteDataReader reader)
        {
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";

            CPlate_LL_Properties properties = new CPlate_LL_Properties();
            properties.ID = reader.GetInt32(reader.GetOrdinal("ID"));
            properties.Name = reader["plateName"].ToString();
            properties.dim11 = double.Parse(reader["dim11"].ToString(), nfi) / 1000;
            properties.dim12 = double.Parse(reader["dim12"].ToString(), nfi) / 1000;
            properties.dim2y = double.Parse(reader["dim2y"].ToString(), nfi) / 1000;
            properties.dim3 = double.Parse(reader["dim3"].ToString(), nfi) / 1000;
            properties.thickness = double.Parse(reader["t"].ToString(), nfi) / 1000;
            properties.NumberOfHolesScrews = Int32.Parse(reader["iNumberHolesScrews"].ToString());
            properties.TotalDim_x = double.Parse(reader["totalDim_x"].ToString(), nfi) / 1000;
            properties.TotalDim_y = double.Parse(reader["totalDim_y"].ToString(), nfi) / 1000;
            properties.Area = double.Parse(reader["area"].ToString(), nfi) / 1000000;
            properties.Volume = double.Parse(reader["volume"].ToString(), nfi) / 1000000000;
            properties.Mass = double.Parse(reader["mass"].ToString(), nfi);
            properties.Price_PPSM_NZD = double.Parse(reader["price_PPSM_NZD"].ToString(), nfi);
            properties.Price_PPKG_NZD = double.Parse(reader["price_PPKG_NZD"].ToString(), nfi);
            properties.Price_PPP_NZD = double.Parse(reader["price_PPP_NZD"].ToString(), nfi);

            return properties;
        }

        //----------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public static List<CPlate_ScrewArrangementProperties> LoadPlate_ArrangementDescriptions(string sPlateSerie)
        {
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";

            CPlate_ScrewArrangementProperties item;
            List<CPlate_ScrewArrangementProperties> items = new List<CPlate_ScrewArrangementProperties>();

            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["JointsSQLiteDB"].ConnectionString))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("Select * from plateSerie"+ sPlateSerie +"_screwArrangement", conn);
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        item = GetPlate_ScrewArrangementProperties(reader);
                        items.Add(item);
                    }
                }
            }
            return items;
        }

        public static List<string> GetPlate_ArrangementNames(string sPlateSerie)
        {
            List<CPlate_ScrewArrangementProperties> items = LoadPlate_ArrangementDescriptions(sPlateSerie);

            List<string> names = new List<string>();

            foreach (CPlate_ScrewArrangementProperties item in items)
                names.Add(item.Name);

            return names;
        }

        public static List<string> GetArrayPlate_ArrangementNames(string sPlateSerie)
        {
            List<string> list_Serie_Names = CJointsManager.GetPlate_ArrangementNames(sPlateSerie);

            return list_Serie_Names;
        }

        private static CPlate_ScrewArrangementProperties GetPlate_ScrewArrangementProperties(SQLiteDataReader reader)
        {
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";

            CPlate_ScrewArrangementProperties properties = new CPlate_ScrewArrangementProperties();
            properties.ID = reader.GetInt32(reader.GetOrdinal("ID"));
            properties.Name = reader["arrangementName"].ToString();

            return properties;
        }
    }
}
