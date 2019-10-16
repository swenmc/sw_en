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
        // STEEL
        public static Dictionary<string, CMatProperties> LoadSteelMaterialProperties()
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
        public static CMatProperties LoadSteelMaterialProperties(string name)
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
        public static CMatProperties LoadSteelMaterialProperties(int ID)
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
        public static CMaterialProperties LoadSteelMaterialPropertiesString(string name)
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
                        properties = GetSteelMaterialPropertiesString(reader);
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
            mat.iNumberOfIntervals = int.Parse(reader["iNumberOfIntervals"].ToString());
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
        private static CMaterialProperties GetSteelMaterialPropertiesString(SQLiteDataReader reader)
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
        public static List<string> LoadSteelMaterialPropertiesStringList(string name)
        {
            CMaterialProperties properties = new CMaterialProperties();
            properties = LoadSteelMaterialPropertiesString(name);
            return FillListOfSteelMaterialPropertiesString(properties);
        }
        public static void LoadSteelMaterialProperties(CMat_03_00 mat, string matName) // Grade
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

                        SetSteelMaterialProperties(reader, ref mat);
                    }
                }
            }
        }
        private static void SetSteelMaterialProperties(SQLiteDataReader reader, ref CMat_03_00 mat)
        {
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";

            float fFactorUnit_Stress = 1e+6f; // From MPa -> Pa, asi by bolo lepsie zmenit jednotky priamo v databaze ??? Ale MPa sa udavaju najcastejsie v podkladoch a tabulkach
            float fFactorUnit_Thickness = 0.001f; // From mm to m

            mat.ID = reader.GetInt32(reader.GetOrdinal("ID"));
            mat.Standard = reader["Standard"].ToString();
            mat.Name = reader["Grade"].ToString();
            mat.m_fE = float.Parse(reader["E"].ToString(), nfi) * fFactorUnit_Stress;
            mat.m_fG = float.Parse(reader["G"].ToString(), nfi) * fFactorUnit_Stress;
            mat.m_fNu = float.Parse(reader["Nu"].ToString(), nfi);
            mat.m_fRho = 7850; // TODO - zapracovat do databazy pripadne do GUI

            // Load number intervals of thickness depending values
            int intervals = int.Parse(reader["iNumberOfIntervals"].ToString());
            // Resize fields
            Array.Resize<float>(ref mat.m_ft_interval, intervals);
            Array.Resize<float>(ref mat.m_ff_yk, intervals);
            Array.Resize<float>(ref mat.m_ff_u, intervals);

            mat.Note = reader["note"].ToString();

            if (intervals == 1)
            {
                mat.m_ft_interval = new float[intervals];
                mat.m_ft_interval[0] = float.Parse(reader["t1"].ToString(), nfi) * fFactorUnit_Thickness;
                mat.m_ff_yk[0] = float.Parse(reader["f_y1"].ToString(), nfi) * fFactorUnit_Stress;
                mat.m_ff_u[0] = float.Parse(reader["f_u1"].ToString(), nfi) * fFactorUnit_Stress;
            }
            else if (intervals == 2)
            {
                mat.m_ft_interval = new float[intervals];
                mat.m_ft_interval[0] = float.Parse(reader["t1"].ToString(), nfi) * fFactorUnit_Thickness;
                mat.m_ff_yk[0] = float.Parse(reader["f_y1"].ToString(), nfi) * fFactorUnit_Stress;
                mat.m_ff_u[0] = float.Parse(reader["f_u1"].ToString(), nfi) * fFactorUnit_Stress;
                mat.m_ft_interval[1] = float.Parse(reader["t2"].ToString(), nfi) * fFactorUnit_Thickness;
                mat.m_ff_yk[1] = float.Parse(reader["f_y2"].ToString(), nfi) * fFactorUnit_Stress;
                mat.m_ff_u[1] = float.Parse(reader["f_u2"].ToString(), nfi) * fFactorUnit_Stress;
            }
            else if (intervals == 3)
            {
                mat.m_ft_interval = new float[intervals];
                mat.m_ft_interval[0] = float.Parse(reader["t1"].ToString(), nfi) * fFactorUnit_Thickness;
                mat.m_ff_yk[0] = float.Parse(reader["f_y1"].ToString(), nfi) * fFactorUnit_Stress;
                mat.m_ff_u[0] = float.Parse(reader["f_u1"].ToString(), nfi) * fFactorUnit_Stress;
                mat.m_ft_interval[1] = float.Parse(reader["t2"].ToString(), nfi) * fFactorUnit_Thickness;
                mat.m_ff_yk[1] = float.Parse(reader["f_y2"].ToString(), nfi) * fFactorUnit_Stress;
                mat.m_ff_u[1] = float.Parse(reader["f_u2"].ToString(), nfi) * fFactorUnit_Stress;
                mat.m_ft_interval[2] = float.Parse(reader["t3"].ToString(), nfi) * fFactorUnit_Thickness;
                mat.m_ff_yk[2] = float.Parse(reader["f_y3"].ToString(), nfi) * fFactorUnit_Stress;
                mat.m_ff_u[2] = float.Parse(reader["f_u3"].ToString(), nfi) * fFactorUnit_Stress;
            }
            else if (intervals == 4)
            {
                mat.m_ft_interval = new float[intervals];
                mat.m_ft_interval[0] = float.Parse(reader["t1"].ToString(), nfi) * fFactorUnit_Thickness;
                mat.m_ff_yk[0] = float.Parse(reader["f_y1"].ToString(), nfi) * fFactorUnit_Stress;
                mat.m_ff_u[0] = float.Parse(reader["f_u1"].ToString(), nfi) * fFactorUnit_Stress;
                mat.m_ft_interval[1] = float.Parse(reader["t2"].ToString(), nfi) * fFactorUnit_Thickness;
                mat.m_ff_yk[1] = float.Parse(reader["f_y2"].ToString(), nfi) * fFactorUnit_Stress;
                mat.m_ff_u[1] = float.Parse(reader["f_u2"].ToString(), nfi) * fFactorUnit_Stress;
                mat.m_ft_interval[2] = float.Parse(reader["t3"].ToString(), nfi) * fFactorUnit_Thickness;
                mat.m_ff_yk[2] = float.Parse(reader["f_y3"].ToString(), nfi) * fFactorUnit_Stress;
                mat.m_ff_u[2] = float.Parse(reader["f_u3"].ToString(), nfi) * fFactorUnit_Stress;
                mat.m_ft_interval[3] = float.Parse(reader["t4"].ToString(), nfi) * fFactorUnit_Thickness;
                mat.m_ff_yk[3] = float.Parse(reader["f_y4"].ToString(), nfi) * fFactorUnit_Stress;
                mat.m_ff_u[3] = float.Parse(reader["f_u4"].ToString(), nfi) * fFactorUnit_Stress;
            }
        }
        public static void SetSteelMaterialProperties(CMatProperties properties, ref CMat_03_00 mat)
        {
            float fFactorUnit_Stress = 1e+6f; // From MPa -> Pa, asi by bolo lepsie zmenit jednotky priamo v databaze ??? Ale MPa sa udavaju najcastejsie v podkladoch a tabulkach
            float fFactorUnit_Thickness = 0.001f; // From mm to m

            mat.ID = properties.ID;
            mat.Standard = properties.Standard;
            mat.Name = properties.Grade;
            mat.m_fE = (float)properties.E;
            mat.m_fG = (float)properties.G;
            mat.m_fNu = (float)properties.Nu;
            mat.m_fRho = 7850; // TODO - zapracovat do databazy pripadne do GUI

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
                mat.m_ft_interval[0] = (float)properties.t1 * fFactorUnit_Thickness;
                mat.m_ff_yk[0] = (float)properties.f_y1 * fFactorUnit_Stress; // From MPa -> Pa, asi by bolo lepsie zmenit jednotky priamo v databaze ??? Ale MPa sa udavaju najcastejsie v podkladoch a tabulkach
                mat.m_ff_u[0] = (float)properties.f_u1 * fFactorUnit_Stress;
            }
            else if (intervals == 2)
            {
                mat.m_ft_interval = new float[intervals];
                mat.m_ft_interval[0] = (float)properties.t1 * fFactorUnit_Thickness;
                mat.m_ff_yk[0] = (float)properties.f_y1 * fFactorUnit_Stress;
                mat.m_ff_u[0] = (float)properties.f_u1 * fFactorUnit_Stress;
                mat.m_ft_interval[1] = (float)properties.t2 * fFactorUnit_Thickness;
                mat.m_ff_yk[1] = (float)properties.f_y2 * fFactorUnit_Stress;
                mat.m_ff_u[1] = (float)properties.f_u2 * fFactorUnit_Stress;
            }
            else if (intervals == 3)
            {
                mat.m_ft_interval = new float[intervals];
                mat.m_ft_interval[0] = (float)properties.t1 * fFactorUnit_Thickness;
                mat.m_ff_yk[0] = (float)properties.f_y1 * fFactorUnit_Stress;
                mat.m_ff_u[0] = (float)properties.f_u1 * fFactorUnit_Stress;
                mat.m_ft_interval[1] = (float)properties.t2 * fFactorUnit_Thickness;
                mat.m_ff_yk[1] = (float)properties.f_y2 * fFactorUnit_Stress;
                mat.m_ff_u[1] = (float)properties.f_u2 * fFactorUnit_Stress;
                mat.m_ft_interval[2] = (float)properties.t3 * fFactorUnit_Thickness;
                mat.m_ff_yk[2] = (float)properties.f_y3 * fFactorUnit_Stress;
                mat.m_ff_u[2] = (float)properties.f_u3 * fFactorUnit_Stress;
            }
            else if (intervals == 4)
            {
                mat.m_ft_interval = new float[intervals];
                mat.m_ft_interval[0] = (float)properties.t1 * fFactorUnit_Thickness;
                mat.m_ff_yk[0] = (float)properties.f_y1 * fFactorUnit_Stress;
                mat.m_ff_u[0] = (float)properties.f_u1 * fFactorUnit_Stress;
                mat.m_ft_interval[1] = (float)properties.t2 * fFactorUnit_Thickness;
                mat.m_ff_yk[1] = (float)properties.f_y2 * fFactorUnit_Stress;
                mat.m_ff_u[1] = (float)properties.f_u2 * fFactorUnit_Stress;
                mat.m_ft_interval[2] = (float)properties.t3 * fFactorUnit_Thickness;
                mat.m_ff_yk[2] = (float)properties.f_y3 * fFactorUnit_Stress;
                mat.m_ff_u[2] = (float)properties.f_u3 * fFactorUnit_Stress;
                mat.m_ft_interval[3] = (float)properties.t4 * fFactorUnit_Thickness;
                mat.m_ff_yk[3] = (float)properties.f_y4 * fFactorUnit_Stress;
                mat.m_ff_u[3] = (float)properties.f_u4 * fFactorUnit_Stress;
            }
        }
        private static List<string> FillListOfSteelMaterialPropertiesString(CMaterialProperties properties)
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

        // CONCRETE
        public static List<string> LoadMaterialPropertiesStringList_RC(string name)
        {
            CMaterialProperties_RC properties = new CMaterialProperties_RC();
            properties = LoadMaterialPropertiesString_RC(name);
            return FillListOfMaterialPropertiesString_RC(properties);
        }

        private static CMatPropertiesRC GetMaterialProperties_RC(SQLiteDataReader reader)
        {
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";

            CMatPropertiesRC mat = new CMatPropertiesRC();
            mat.ID = reader.GetInt32(reader.GetOrdinal("ID"));
            mat.Standard = reader["Standard"].ToString();
            mat.Grade = reader["Grade"].ToString();
            //mat.E = reader["E"].ToString() == "" ? double.NaN : double.Parse(reader["E"].ToString(), nfi);
            //mat.G = reader["G"].ToString() == "" ? double.NaN : double.Parse(reader["G"].ToString(), nfi);
            mat.Nu = reader["poisson_ratio_nu"].ToString() == "" ? double.NaN : double.Parse(reader["poisson_ratio_nu"].ToString(), nfi);
            mat.Fc = reader["fc_cylinder_Pa"].ToString() == "" ? double.NaN : double.Parse(reader["fc_cylinder_Pa"].ToString(), nfi);
            mat.Rho = reader["density_rho"].ToString() == "" ? double.NaN : double.Parse(reader["density_rho"].ToString(), nfi);
            mat.Alpha = reader["alpha"].ToString() == "" ? double.NaN : double.Parse(reader["alpha"].ToString(), nfi);

            return mat;
        }

        public static CMaterialProperties_RC LoadMaterialPropertiesString_RC(string name)
        {
            CMaterialProperties_RC properties = null;
            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["MaterialsRCSQLiteDB"].ConnectionString))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("Select * from Concrete_NZS3101 WHERE Grade = @Grade", conn);
                command.Parameters.AddWithValue("@Grade", name);

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        properties = GetMaterialPropertiesString_RC(reader);
                    }
                }
            }
            return properties;
        }

        private static CMaterialProperties_RC GetMaterialPropertiesString_RC(SQLiteDataReader reader)
        {
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";

            CMaterialProperties_RC mat = new CMaterialProperties_RC();
            mat.ID = reader.GetInt32(reader.GetOrdinal("ID"));
            mat.Standard = reader["Standard"].ToString();
            mat.Grade = reader["Grade"].ToString();
            //mat.E = reader["E"].ToString();
            //mat.G = reader["G"].ToString();
            mat.Nu = reader["poisson_ratio_nu"].ToString();
            mat.Fc = reader["fc_cylinder_Pa"].ToString();
            mat.Rho = reader["density_rho"].ToString();
            mat.Alpha = reader["alpha"].ToString();
            return mat;
        }

        public static Dictionary<string, CMatPropertiesRC> LoadMaterialPropertiesRC()
        {
            CMatPropertiesRC mat = null;
            Dictionary<string, CMatPropertiesRC> items = new Dictionary<string, CMatPropertiesRC>();

            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["MaterialsRCSQLiteDB"].ConnectionString))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("Select * from Concrete_NZS3101", conn);
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        mat = GetMaterialProperties_RC(reader);
                        items.Add(mat.Grade, mat);
                    }
                }
            }
            return items;
        }

        public static CMatPropertiesRC LoadMaterialPropertiesRC(string name)
        {
            CMatPropertiesRC properties = null;
            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["MaterialsRCSQLiteDB"].ConnectionString))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("Select * from Concrete_NZS3101 WHERE Grade = @Grade", conn);
                command.Parameters.AddWithValue("@Grade", name);

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        properties = GetMaterialProperties_RC(reader);
                    }
                }
            }
            return properties;
        }

        private static List<string> FillListOfMaterialPropertiesString_RC(CMaterialProperties_RC properties)
        {
            List<string> list = new List<string>();
            list.Add(properties.Standard);
            list.Add(properties.Grade);
            //list.Add(properties.E);
            //list.Add(properties.G);
            list.Add(properties.Nu);
            list.Add(properties.Fc);
            list.Add(properties.Rho);
            list.Add(properties.Alpha);

            return list;
        }

        // REINFORCEMENT
        private static CMatPropertiesRF GetMaterialProperties_RF(SQLiteDataReader reader)
        {
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";

            CMatPropertiesRF mat = new CMatPropertiesRF();
            mat.ID = reader.GetInt32(reader.GetOrdinal("ID"));
            mat.Standard = reader["Standard"].ToString();
            mat.Grade = reader["Grade"].ToString();
            //mat.E = reader["E"].ToString() == "" ? double.NaN : double.Parse(reader["E"].ToString(), nfi);
            //mat.G = reader["G"].ToString() == "" ? double.NaN : double.Parse(reader["G"].ToString(), nfi);
            //mat.Nu = reader["poisson_ratio_nu"].ToString() == "" ? double.NaN : double.Parse(reader["Nu"].ToString(), nfi);
            mat.Ry = reader["Ry_Pa"].ToString() == "" ? double.NaN : double.Parse(reader["Ry_Pa"].ToString(), nfi);
            mat.Rho = reader["density_rho"].ToString() == "" ? double.NaN : double.Parse(reader["density_rho"].ToString(), nfi);
            //mat.Alpha = reader["alpha"].ToString() == "" ? double.NaN : double.Parse(reader["alpha"].ToString(), nfi);

            // TODO - skontrolovat
            mat.E = 200e+9;
            mat.G = 80e+9;
            mat.Nu = 0.25;
            mat.Alpha = 1.2e-6;

            return mat;
        }

        public static Dictionary<string, CMatPropertiesRF> LoadMaterialPropertiesRF()
        {
            CMatPropertiesRF mat = null;
            Dictionary<string, CMatPropertiesRF> items = new Dictionary<string, CMatPropertiesRF>();

            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["MaterialsRCSQLiteDB"].ConnectionString))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("Select * from Reinforcement_NZS4671", conn);
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        mat = GetMaterialProperties_RF(reader);
                        items.Add(mat.Grade, mat);
                    }
                }
            }
            return items;
        }

        public static CMatPropertiesRF LoadMaterialPropertiesRF(string name)
        {
            CMatPropertiesRF properties = null;
            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["MaterialsRCSQLiteDB"].ConnectionString))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("Select * from Reinforcement_NZS4671 WHERE Grade = @Grade", conn);
                command.Parameters.AddWithValue("@Grade", name);

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        properties = GetMaterialProperties_RF(reader);
                    }
                }
            }
            return properties;
        }

        // BOLTS / ANCHORS / THREATENED RODS

        private static CMatPropertiesBOLT GetMaterialProperties_BOLT(SQLiteDataReader reader)
        {
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";

            CMatPropertiesBOLT mat = new CMatPropertiesBOLT();
            mat.ID = reader.GetInt32(reader.GetOrdinal("ID"));
            mat.Standard = reader["Standard"].ToString();
            mat.Grade = reader["Grade"].ToString();
            //mat.E = reader["E"].ToString() == "" ? double.NaN : double.Parse(reader["E"].ToString(), nfi);
            //mat.G = reader["G"].ToString() == "" ? double.NaN : double.Parse(reader["G"].ToString(), nfi);
            //mat.Nu = reader["poisson_ratio_nu"].ToString() == "" ? double.NaN : double.Parse(reader["Nu"].ToString(), nfi);
            mat.Fy = reader["fy"].ToString() == "" ? double.NaN : double.Parse(reader["fy"].ToString(), nfi);
            mat.Fu = reader["fu"].ToString() == "" ? double.NaN : double.Parse(reader["fu"].ToString(), nfi);
            //mat.Rho = reader["density_rho"].ToString() == "" ? double.NaN : double.Parse(reader["density_rho"].ToString(), nfi);
            //mat.Alpha = reader["alpha"].ToString() == "" ? double.NaN : double.Parse(reader["alpha"].ToString(), nfi);

            // TODO - skontrolovat
            mat.E = 200e+9;
            mat.G = 80e+9;
            mat.Nu = 0.25;
            mat.Rho = 7850;
            mat.Alpha = 1.2e-6;

            return mat;
        }

        public static Dictionary<string, CMatPropertiesBOLT> LoadMaterialPropertiesBOLT()
        {
            CMatPropertiesBOLT mat = null;
            Dictionary<string, CMatPropertiesBOLT> items = new Dictionary<string, CMatPropertiesBOLT>();

            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["BoltsSQLiteDB"].ConnectionString))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("Select * from Material", conn);
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        mat = GetMaterialProperties_BOLT(reader);
                        items.Add(mat.Grade, mat);
                    }
                }
            }
            return items;
        }

        public static CMatPropertiesBOLT LoadMaterialPropertiesBOLT(string name)
        {
            CMatPropertiesBOLT properties = null;
            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["BoltsSQLiteDB"].ConnectionString))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("Select * from Material WHERE Grade = @Grade", conn);
                command.Parameters.AddWithValue("@Grade", name);

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        properties = GetMaterialProperties_BOLT(reader);
                    }
                }
            }
            return properties;
        }
    }
}
