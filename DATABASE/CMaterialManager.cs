using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SQLite;
using System.Globalization;
using DATABASE.DTO;
using MATERIAL;

namespace DATABASE
{
    public static class CMaterialManager
    {
        public static List<CMaterialProperties> LoadMaterialProperties()
        {
            CMaterialProperties properties;
            List<CMaterialProperties> items = new List<CMaterialProperties>();

            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["MaterialsSQLiteDB"].ConnectionString))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("Select * from materialSteelAS4600", conn);
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        properties = new CMaterialProperties();
                        properties.ID = reader.GetInt32(reader.GetOrdinal("ID"));
                        properties.Standard = reader["Standard"].ToString();
                        properties.Grade = reader["Grade"].ToString();
                        properties.iNumberOfIntervals = reader["iNumberOfIntervals"].ToString();
                        properties.t1 = reader["t1"].ToString();
                        properties.t2 = reader["t2"].ToString();
                        properties.t3 = reader["t3"].ToString();
                        properties.t4 = reader["t4"].ToString();
                        properties.f_y1 = reader["f_y1"].ToString();
                        properties.f_u1 = reader["f_u1"].ToString();
                        properties.f_y2 = reader["f_y2"].ToString();
                        properties.f_u2 = reader["f_u2"].ToString();
                        properties.f_y3 = reader["f_y3"].ToString();
                        properties.f_u3 = reader["f_u3"].ToString();
                        properties.f_y4 = reader["f_y4"].ToString();
                        properties.f_u4 = reader["f_u4"].ToString();
                        items.Add(properties);
                    }
                }
            }
            return items;
        }

        public static CMaterialProperties LoadMaterialProperties(int ID)
        {
            CMaterialProperties properties = null;
            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["MaterialsSQLiteDB"].ConnectionString))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("Select * from materialSteelAS4600 WHERE ID = @id", conn);
                command.Parameters.AddWithValue("@id", ID);

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        properties = GetMaterialProperties(reader);
                    }
                }
            }
            return properties;
        }

        public static CMaterialProperties LoadMaterialProperties(string name)
        {
            CMaterialProperties properties = null;
            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["MaterialsSQLiteDB"].ConnectionString))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("Select * from materialSteelAS4600 WHERE Grade = @Grade", conn);
                command.Parameters.AddWithValue("@Grade", name);

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        properties = GetMaterialProperties(reader);
                    }
                }
            }
            return properties;
        }

        private static CMaterialProperties GetMaterialProperties(SQLiteDataReader reader)
        {
            CMaterialProperties properties = new CMaterialProperties();
            properties.ID = reader.GetInt32(reader.GetOrdinal("ID"));
            properties.Standard = reader["Standard"].ToString();
            properties.Grade = reader["Grade"].ToString();
            properties.iNumberOfIntervals = reader["iNumberOfIntervals"].ToString();
            properties.t1 = reader["t1"].ToString();
            properties.t2 = reader["t2"].ToString();
            properties.t3 = reader["t3"].ToString();
            properties.t4 = reader["t4"].ToString();
            properties.f_y1 = reader["f_y1"].ToString();
            properties.f_u1 = reader["f_u1"].ToString();
            properties.f_y2 = reader["f_y2"].ToString();
            properties.f_u2 = reader["f_u2"].ToString();
            properties.f_y3 = reader["f_y3"].ToString();
            properties.f_u3 = reader["f_u3"].ToString();
            properties.f_y4 = reader["f_y4"].ToString();
            properties.f_u4 = reader["f_u4"].ToString();

            return properties;
        }

        public static void SetMaterialValuesFromDatabase(CMat[] materials)
        {
            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["MaterialsSQLiteDB"].ConnectionString))
            {
                conn.Open();

                foreach (CMat_03_00 mat in materials)
                {
                    //??? predpokladam,ze na zaklade nejakej property z mat treba vybrat z DB a nie natvrdo 1
                    int ID = 1;
                    string stringcommand = "Select * from materialSteelAS4600 where ID = '" + ID + "'";

                    using (SQLiteCommand command = new SQLiteCommand(stringcommand, conn))
                    {
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                mat.Standard = reader["Standard"].ToString();
                                mat.Name = /*mat.Grade =*/ reader["Grade"].ToString();

                                // Load number intervals of thickness depending values
                                int intervals = reader.GetInt32(reader.GetOrdinal("iNumberOfIntervals"));
                                // Resize fields
                                Array.Resize<float>(ref mat.m_ft_interval, intervals);
                                Array.Resize<float>(ref mat.m_ff_yk, intervals);
                                Array.Resize<float>(ref mat.m_ff_u, intervals);

                                mat.Note = reader["note"].ToString();

                                if (intervals == 1)
                                {
                                    mat.m_ft_interval = new float[intervals];
                                    mat.m_ft_interval[0] = reader.GetFloat(reader.GetOrdinal("t1"));
                                    mat.m_ff_yk[0] = reader.GetFloat(reader.GetOrdinal("f_y1")) * 1e+6f; // From MPa -> Pa, asi by bolo lepsie zmenit jednotky priamo v databaze ??? Ale MPa sa udavaju najcastejsie v podkladoch a tabulkach
                                    mat.m_ff_u[0] = reader.GetFloat(reader.GetOrdinal("f_u1")) * 1e+6f;
                                }
                                else if (intervals == 2)
                                {
                                    mat.m_ft_interval = new float[intervals];
                                    mat.m_ft_interval[0] = reader.GetFloat(reader.GetOrdinal("t1"));
                                    mat.m_ff_yk[0] = reader.GetFloat(reader.GetOrdinal("f_y1")) * 1e+6f;
                                    mat.m_ff_u[0] = reader.GetFloat(reader.GetOrdinal("f_u1")) * 1e+6f;
                                    mat.m_ft_interval[1] = reader.GetFloat(reader.GetOrdinal("t2"));
                                    mat.m_ff_yk[1] = reader.GetFloat(reader.GetOrdinal("f_y2")) * 1e+6f;
                                    mat.m_ff_u[1] = reader.GetFloat(reader.GetOrdinal("f_u2")) * 1e+6f;
                                }
                                else if (intervals == 3)
                                {
                                    mat.m_ft_interval = new float[intervals];
                                    mat.m_ft_interval[0] = reader.GetFloat(reader.GetOrdinal("t1"));
                                    mat.m_ff_yk[0] = reader.GetFloat(reader.GetOrdinal("f_y1")) * 1e+6f;
                                    mat.m_ff_u[0] = reader.GetFloat(reader.GetOrdinal("f_u1")) * 1e+6f;
                                    mat.m_ft_interval[1] = reader.GetFloat(reader.GetOrdinal("t2"));
                                    mat.m_ff_yk[1] = reader.GetFloat(reader.GetOrdinal("f_y2")) * 1e+6f;
                                    mat.m_ff_u[1] = reader.GetFloat(reader.GetOrdinal("f_u2")) * 1e+6f;
                                    mat.m_ft_interval[2] = reader.GetFloat(reader.GetOrdinal("t3"));
                                    mat.m_ff_yk[2] = reader.GetFloat(reader.GetOrdinal("f_y3")) * 1e+6f;
                                    mat.m_ff_u[2] = reader.GetFloat(reader.GetOrdinal("f_u3")) * 1e+6f;
                                }
                                else if (intervals == 4)
                                {
                                    mat.m_ft_interval = new float[intervals];
                                    mat.m_ft_interval[0] = reader.GetFloat(reader.GetOrdinal("t1"));
                                    mat.m_ff_yk[0] = reader.GetFloat(reader.GetOrdinal("f_y1")) * 1e+6f;
                                    mat.m_ff_u[0] = reader.GetFloat(reader.GetOrdinal("f_u1")) * 1e+6f;
                                    mat.m_ft_interval[1] = reader.GetFloat(reader.GetOrdinal("t2"));
                                    mat.m_ff_yk[1] = reader.GetFloat(reader.GetOrdinal("f_y2")) * 1e+6f;
                                    mat.m_ff_u[1] = reader.GetFloat(reader.GetOrdinal("f_u2")) * 1e+6f;
                                    mat.m_ft_interval[2] = reader.GetFloat(reader.GetOrdinal("t3"));
                                    mat.m_ff_yk[2] = reader.GetFloat(reader.GetOrdinal("f_y3")) * 1e+6f;
                                    mat.m_ff_u[2] = reader.GetFloat(reader.GetOrdinal("f_u3")) * 1e+6f;
                                    mat.m_ft_interval[3] = reader.GetFloat(reader.GetOrdinal("t4"));
                                    mat.m_ff_yk[3] = reader.GetFloat(reader.GetOrdinal("f_y4")) * 1e+6f;
                                    mat.m_ff_u[3] = reader.GetFloat(reader.GetOrdinal("f_u4")) * 1e+6f;
                                }
                            } //end while
                        } //end reader
                    }
                }
            }
        }

        public static void LoadMaterialProperties(CMat_03_00 mat, string matNameDatabase) // Grade
        {
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";

            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["MaterialsSQLiteDB"].ConnectionString))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("Select * from materialSteelAS4600 WHERE Grade = @Grade", conn);
                command.Parameters.AddWithValue("@Grade", matNameDatabase);

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        mat.NameDatabase = matNameDatabase;

                        SetMaterialProperties(reader, ref mat);
                    }
                }
            }
        }

        public static List<string> LoadMaterialPropertiesStringList(int ID)
        {
            CMaterialProperties properties = new CMaterialProperties();
            properties = LoadMaterialProperties(ID);
            return FillListOfMaterialPropertiesString(properties);
        }

        public static List<string> LoadMaterialPropertiesStringList(string name)
        {
            CMaterialProperties properties = new CMaterialProperties();
            properties = LoadMaterialProperties(name);
            return FillListOfMaterialPropertiesString(properties);
        }

        private static List<string> FillListOfMaterialPropertiesString(CMaterialProperties properties)
        {
            List<string> list = new List<string>();
            list.Add(properties.iNumberOfIntervals);
            list.Add(properties.t1);
            list.Add(properties.t2);
            list.Add(properties.t3);
            list.Add(properties.t4);
            list.Add(properties.f_y1);
            list.Add(properties.f_u1);
            list.Add(properties.f_y2);
            list.Add(properties.f_u2);
            list.Add(properties.f_y3);
            list.Add(properties.f_u3);
            list.Add(properties.f_y4);
            list.Add(properties.f_u4);

            return list;
        }

        private static void SetMaterialProperties(SQLiteDataReader reader, ref CMat_03_00 mat)
        {
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";

            mat.ID = reader.GetInt32(reader.GetOrdinal("ID"));
            mat.Standard = reader["Standard"].ToString();
            mat.Name = /*mat.Grade =*/ reader["Grade"].ToString();

            // Load number intervals of thickness depending values
            int intervals = reader.GetInt32(reader.GetOrdinal("iNumberOfIntervals"));
            // Resize fields
            Array.Resize<float>(ref mat.m_ft_interval, intervals);
            Array.Resize<float>(ref mat.m_ff_yk, intervals);
            Array.Resize<float>(ref mat.m_ff_u, intervals);

            mat.Note = reader["note"].ToString();

            if (intervals == 1)
            {
                mat.m_ft_interval = new float[intervals];
                mat.m_ft_interval[0] = float.Parse(reader["t1"].ToString(), nfi);
                mat.m_ff_yk[0] = float.Parse(reader["f_y1"].ToString(), nfi) * 1e+6f; // From MPa -> Pa, asi by bolo lepsie zmenit jednotky priamo v databaze ??? Ale MPa sa udavaju najcastejsie v podkladoch a tabulkach
                mat.m_ff_u[0] = float.Parse(reader["f_u1"].ToString(), nfi) * 1e+6f;
            }
            else if (intervals == 2)
            {
                mat.m_ft_interval = new float[intervals];
                mat.m_ft_interval[0] = float.Parse(reader["t1"].ToString(), nfi);
                mat.m_ff_yk[0] = float.Parse(reader["f_y1"].ToString(), nfi) * 1e+6f;
                mat.m_ff_u[0] = float.Parse(reader["f_u1"].ToString(), nfi) * 1e+6f;
                mat.m_ft_interval[1] = float.Parse(reader["t2"].ToString(), nfi);
                mat.m_ff_yk[1] = float.Parse(reader["f_y2"].ToString(), nfi) * 1e+6f;
                mat.m_ff_u[1] = float.Parse(reader["f_u2"].ToString(), nfi) * 1e+6f;
            }
            else if (intervals == 3)
            {
                mat.m_ft_interval = new float[intervals];
                mat.m_ft_interval[0] = float.Parse(reader["t1"].ToString(), nfi);
                mat.m_ff_yk[0] = float.Parse(reader["f_y1"].ToString(), nfi) * 1e+6f;
                mat.m_ff_u[0] = float.Parse(reader["f_u1"].ToString(), nfi) * 1e+6f;
                mat.m_ft_interval[1] = float.Parse(reader["t2"].ToString(), nfi);
                mat.m_ff_yk[1] = float.Parse(reader["f_y2"].ToString(), nfi) * 1e+6f;
                mat.m_ff_u[1] = float.Parse(reader["f_u2"].ToString(), nfi) * 1e+6f;
                mat.m_ft_interval[2] = float.Parse(reader["t3"].ToString(), nfi);
                mat.m_ff_yk[2] = float.Parse(reader["f_y3"].ToString(), nfi) * 1e+6f;
                mat.m_ff_u[2] = float.Parse(reader["f_u3"].ToString(), nfi) * 1e+6f;
            }
            else if (intervals == 4)
            {
                mat.m_ft_interval = new float[intervals];
                mat.m_ft_interval[0] = float.Parse(reader["t1"].ToString(), nfi);
                mat.m_ff_yk[0] = float.Parse(reader["f_y1"].ToString(), nfi) * 1e+6f;
                mat.m_ff_u[0] = float.Parse(reader["f_u1"].ToString(), nfi) * 1e+6f;
                mat.m_ft_interval[1] = float.Parse(reader["t2"].ToString(), nfi);
                mat.m_ff_yk[1] = float.Parse(reader["f_y2"].ToString(), nfi) * 1e+6f;
                mat.m_ff_u[1] = float.Parse(reader["f_u2"].ToString(), nfi) * 1e+6f;
                mat.m_ft_interval[2] = float.Parse(reader["t3"].ToString(), nfi);
                mat.m_ff_yk[2] = float.Parse(reader["f_y3"].ToString(), nfi) * 1e+6f;
                mat.m_ff_u[2] = float.Parse(reader["f_u3"].ToString(), nfi) * 1e+6f;
                mat.m_ft_interval[3] = float.Parse(reader["t4"].ToString(), nfi);
                mat.m_ff_yk[3] = float.Parse(reader["f_y4"].ToString(), nfi) * 1e+6f;
                mat.m_ff_u[3] = float.Parse(reader["f_u4"].ToString(), nfi) * 1e+6f;
            }
        }
    }
}
