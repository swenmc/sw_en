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
        public static Dictionary<string, CMatProperties> LoadMaterialProperties()
        {
            CMatProperties mat = null;
            Dictionary<string, CMatProperties> items = new Dictionary<string, CMatProperties>();

            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["MaterialsSQLiteDB"].ConnectionString))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("Select * from materialSteelAS4600", conn);
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        mat = GetMaterialProperties(reader);
                        items.Add(mat.Grade, mat);
                    }
                }
            }
            return items;
        }
        public static List<CMaterialPropertiesText> LoadMaterialPropertiesNamesSymbolsUnits()
        {
            CMaterialPropertiesText properties;
            List<CMaterialPropertiesText> items = new List<CMaterialPropertiesText>();

            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["MaterialsSQLiteDB"].ConnectionString))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("Select * from materialProperties", conn);
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        properties = new CMaterialPropertiesText();
                        properties.ID = reader.GetInt32(reader.GetOrdinal("ID"));
                        properties.Text = reader["propertyText"].ToString();
                        properties.Symbol = reader["propertySymbol"].ToString();
                        properties.Name = reader["propertyName"].ToString();
                        properties.Unit_SI = reader["unit_SI"].ToString();
                        properties.Unit_NcmkPa = reader["unit_NcmkPa"].ToString();
                        properties.Unit_NmmMpa = reader["unit_NmmMPa"].ToString();
                        items.Add(properties);
                    }
                }
            }
            return items;
        }
        public static CMatProperties LoadMaterialProperties(string name)
        {
            CMatProperties properties = null;
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
        public static List<string> GetMaterialTypesList()
        {
            List<string> materialTypes = new List<string>();
            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["MaterialsSQLiteDB"].ConnectionString))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("Select * from materialSteelAS4600", conn);
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        materialTypes.Add(reader["Grade"].ToString());
                    }
                }
            }
            return materialTypes;
        }
        public static CMatProperties LoadMaterialProperties(int ID)
        {
            CMatProperties properties = null;
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
        public static CMaterialProperties LoadMaterialPropertiesString(string name)
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
                        properties = GetMaterialPropertiesString(reader);
                    }
                }
            }
            return properties;
        }
        private static CMatProperties GetMaterialProperties(SQLiteDataReader reader)
        {
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";

            CMatProperties mat = new CMatProperties();
            mat.ID = reader.GetInt32(reader.GetOrdinal("ID"));
            mat.Standard = reader["Standard"].ToString();
            mat.Grade = reader["Grade"].ToString();
            mat.E = reader["E"].ToString() == "" ? double.NaN : double.Parse(reader["E"].ToString(), nfi);
            mat.G = reader["G"].ToString() == "" ? double.NaN : double.Parse(reader["G"].ToString(), nfi);
            mat.Nu = reader["Nu"].ToString() == "" ? double.NaN : double.Parse(reader["Nu"].ToString(), nfi);
            mat.iNumberOfIntervals = int.Parse(reader["iNumberOfIntervals"].ToString(), nfi);
            mat.t1 = reader["t1"].ToString() == "" ? double.NaN : double.Parse(reader["t1"].ToString(), nfi);
            mat.t2 = reader["t2"].ToString() == "" ? double.NaN : double.Parse(reader["t2"].ToString(), nfi);
            mat.t3 = reader["t3"].ToString() == "" ? double.NaN : double.Parse(reader["t3"].ToString(), nfi);
            mat.t4 = reader["t4"].ToString() == "" ? double.NaN : double.Parse(reader["t4"].ToString(), nfi);
            mat.f_y1 = reader["f_y1"].ToString() == "" ? double.NaN : double.Parse(reader["f_y1"].ToString(), nfi);
            mat.f_u1 = reader["f_u1"].ToString() == "" ? double.NaN : double.Parse(reader["f_u1"].ToString(), nfi);
            mat.f_y2 = reader["f_y2"].ToString() == "" ? double.NaN : double.Parse(reader["f_y2"].ToString(), nfi);
            mat.f_u2 = reader["f_u2"].ToString() == "" ? double.NaN : double.Parse(reader["f_u2"].ToString(), nfi);
            mat.f_y3 = reader["f_y3"].ToString() == "" ? double.NaN : double.Parse(reader["f_y3"].ToString(), nfi);
            mat.f_u3 = reader["f_u3"].ToString() == "" ? double.NaN : double.Parse(reader["f_u3"].ToString(), nfi);
            mat.f_y4 = reader["f_y4"].ToString() == "" ? double.NaN : double.Parse(reader["f_y4"].ToString(), nfi);
            mat.f_u4 = reader["f_u4"].ToString() == "" ? double.NaN : double.Parse(reader["f_u4"].ToString(), nfi);
            mat.Note = reader["note"].ToString();
            return mat;
        }
        private static CMaterialProperties GetMaterialPropertiesString(SQLiteDataReader reader)
        {
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";

            CMaterialProperties mat = new CMaterialProperties();
            mat.ID = reader.GetInt32(reader.GetOrdinal("ID"));
            mat.Standard = reader["Standard"].ToString();
            mat.Grade = reader["Grade"].ToString();
            mat.E = reader["E"].ToString();
            mat.G = reader["G"].ToString();
            mat.Nu = reader["Nu"].ToString();
            mat.INumberOfIntervals = reader["iNumberOfIntervals"].ToString();
            mat.T1 = reader["t1"].ToString();
            mat.T2 = reader["t2"].ToString();
            mat.T3 = reader["t3"].ToString();
            mat.T4 = reader["t4"].ToString();
            mat.F_y1 = reader["f_y1"].ToString();
            mat.F_u1 = reader["f_u1"].ToString();
            mat.F_y2 = reader["f_y2"].ToString();
            mat.F_u2 = reader["f_u2"].ToString();
            mat.F_y3 = reader["f_y3"].ToString();
            mat.F_u3 = reader["f_u3"].ToString();
            mat.F_y4 = reader["f_y4"].ToString();
            mat.F_u4 = reader["f_u4"].ToString();
            mat.Note = reader["note"].ToString();
            return mat;
        }
        public static List<string> LoadMaterialPropertiesStringList(string name)
        {
            CMaterialProperties properties = new CMaterialProperties();
            properties = LoadMaterialPropertiesString(name);
            return FillListOfMaterialPropertiesString(properties);
        }
        public static void LoadMaterialProperties(CMat_03_00 mat, string matName) // Grade
        {
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";

            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["MaterialsSQLiteDB"].ConnectionString))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("Select * from materialSteelAS4600 WHERE Grade = @Grade", conn);
                command.Parameters.AddWithValue("@Grade", matName);

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        mat.Name = matName;

                        SetMaterialProperties(reader, ref mat);
                    }
                }
            }
        }
        private static void SetMaterialProperties(SQLiteDataReader reader, ref CMat_03_00 mat)
        {
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";

            mat.ID = reader.GetInt32(reader.GetOrdinal("ID"));
            mat.Standard = reader["Standard"].ToString();
            mat.Name = reader["Grade"].ToString();
            mat.m_fE = float.Parse(reader["E"].ToString(), nfi) * 1e+6f;
            mat.m_fG = float.Parse(reader["G"].ToString(), nfi) * 1e+6f;
            mat.m_fNu = float.Parse(reader["Nu"].ToString(), nfi) * 1e+6f;

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
        public static void SetMaterialProperties(CMatProperties properties, ref CMat_03_00 mat)
        {
            mat.ID = properties.ID;
            mat.Standard = properties.Standard;
            mat.Name = properties.Grade;
            mat.m_fE = (float)properties.E;
            mat.m_fG = (float)properties.G;
            mat.m_fNu = (float)properties.Nu;

            // Load number intervals of thickness depending values
            int intervals = properties.iNumberOfIntervals;
            // Resize fields
            Array.Resize<float>(ref mat.m_ft_interval, intervals);
            Array.Resize<float>(ref mat.m_ff_yk, intervals);
            Array.Resize<float>(ref mat.m_ff_u, intervals);

            mat.Note = properties.Note;

            if (intervals == 1)
            {
                mat.m_ft_interval = new float[intervals];
                mat.m_ft_interval[0] = (float)properties.t1;
                mat.m_ff_yk[0] = (float)properties.f_y1 * 1e+6f; // From MPa -> Pa, asi by bolo lepsie zmenit jednotky priamo v databaze ??? Ale MPa sa udavaju najcastejsie v podkladoch a tabulkach
                mat.m_ff_u[0] = (float)properties.f_u1 * 1e+6f;
            }
            else if (intervals == 2)
            {
                mat.m_ft_interval = new float[intervals];
                mat.m_ft_interval[0] = (float)properties.t1;
                mat.m_ff_yk[0] = (float)properties.f_y1 * 1e+6f;
                mat.m_ff_u[0] = (float)properties.f_u1 * 1e+6f;
                mat.m_ft_interval[1] = (float)properties.t2;
                mat.m_ff_yk[1] = (float)properties.f_y2 * 1e+6f;
                mat.m_ff_u[1] = (float)properties.f_u2 * 1e+6f;
            }
            else if (intervals == 3)
            {
                mat.m_ft_interval = new float[intervals];
                mat.m_ft_interval[0] = (float)properties.t1;
                mat.m_ff_yk[0] = (float)properties.f_y1 * 1e+6f;
                mat.m_ff_u[0] = (float)properties.f_u1 * 1e+6f;
                mat.m_ft_interval[1] = (float)properties.t2;
                mat.m_ff_yk[1] = (float)properties.f_y2 * 1e+6f;
                mat.m_ff_u[1] = (float)properties.f_u2 * 1e+6f;
                mat.m_ft_interval[2] = (float)properties.t3;
                mat.m_ff_yk[2] = (float)properties.f_y3 * 1e+6f;
                mat.m_ff_u[2] = (float)properties.f_u3 * 1e+6f;
            }
            else if (intervals == 4)
            {
                mat.m_ft_interval = new float[intervals];
                mat.m_ft_interval[0] = (float)properties.t1;
                mat.m_ff_yk[0] = (float)properties.f_y1 * 1e+6f;
                mat.m_ff_u[0] = (float)properties.f_u1 * 1e+6f;
                mat.m_ft_interval[1] = (float)properties.t2;
                mat.m_ff_yk[1] = (float)properties.f_y2 * 1e+6f;
                mat.m_ff_u[1] = (float)properties.f_u2 * 1e+6f;
                mat.m_ft_interval[2] = (float)properties.t3;
                mat.m_ff_yk[2] = (float)properties.f_y3 * 1e+6f;
                mat.m_ff_u[2] = (float)properties.f_u3 * 1e+6f;
                mat.m_ft_interval[3] = (float)properties.t4;
                mat.m_ff_yk[3] = (float)properties.f_y4 * 1e+6f;
                mat.m_ff_u[3] = (float)properties.f_u4 * 1e+6f;
            }
        }
        private static List<string> FillListOfMaterialPropertiesString(CMaterialProperties properties)
        {
            List<string> list = new List<string>();
            list.Add(properties.Standard);
            list.Add(properties.Grade);
            list.Add(properties.E);
            list.Add(properties.G);
            list.Add(properties.Nu);
            list.Add(properties.INumberOfIntervals);
            list.Add(properties.T1);
            list.Add(properties.F_y1);
            list.Add(properties.F_u1);
            list.Add(properties.T2);
            list.Add(properties.F_y2);
            list.Add(properties.F_u2);
            list.Add(properties.T3);
            list.Add(properties.F_y3);
            list.Add(properties.F_u3);
            list.Add(properties.T4);
            list.Add(properties.F_y4);
            list.Add(properties.F_u4);
            list.Add(properties.Note);

            return list;
        }
    }
}
