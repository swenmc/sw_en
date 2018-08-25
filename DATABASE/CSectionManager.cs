using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Data.SQLite;
using System.Globalization;
using DATABASE.DTO;
using CRSC;

namespace DATABASE
{
    public static class CSectionManager
    {
        public static List<CSectionProperties> LoadSectionProperties()
        {
            CSectionProperties properties;
            List<CSectionProperties> items = new List<CSectionProperties>();
            
            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["SectionsSQLiteDB"].ConnectionString))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("Select * from tableSections_mm", conn);
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        properties = new CSectionProperties();
                        properties.ID = reader.GetInt32(reader.GetOrdinal("ID"));
                        properties.sectionName = reader["section"].ToString();
                        properties.h = reader["h"].ToString();
                        properties.b = reader["b"].ToString();
                        properties.t = reader["t"].ToString();
                        properties.A_g = reader["A_g"].ToString();
                        properties.I_y0 = reader["I_y0"].ToString();
                        properties.I_z0 = reader["I_z0"].ToString();
                        properties.W_el_y0 = reader["W_el_y0"].ToString();
                        properties.W_el_z0 = reader["W_el_z0"].ToString();
                        properties.Iyz0 = reader["Iyz0"].ToString();
                        properties.Iy = reader["Iy"].ToString();
                        properties.Iz = reader["Iz"].ToString();
                        properties.W_el_y = reader["W_el_y"].ToString();
                        properties.W_el_z = reader["W_el_z"].ToString();
                        properties.It = reader["It"].ToString();
                        properties.Iw = reader["Iw"].ToString();
                        properties.yc = reader["yc"].ToString();
                        properties.zc = reader["zc"].ToString();
                        properties.ys = reader["ys"].ToString();
                        properties.zs = reader["zs"].ToString();
                        properties.ycs = reader["ycs"].ToString();
                        properties.zcs = reader["zcs"].ToString();
                        properties.betay = reader["betay"].ToString();
                        properties.betaz = reader["betaz"].ToString();
                        properties.alpha_deg = reader["alpha_deg"].ToString();
                        properties.Bending_curve_x1 = reader["Bending_curve_x1"].ToString();
                        properties.Bending_curve_x2 = reader["Bending_curve_x2"].ToString();
                        properties.Bending_curve_x3 = reader["Bending_curve_x3"].ToString();
                        properties.Bending_curve_y = reader["Bending_curve_y"].ToString();
                        properties.Compression_curve_1 = reader["Compression_curve_1"].ToString();
                        properties.Compression_curve_2 = reader["Compression_curve_2"].ToString();
                        properties.Compression_curve_3 = reader["Compression_curve_3"].ToString();
                        items.Add(properties);
                    }
                }
            }
            return items;
        }

        public static CSectionProperties LoadSectionProperties(int ID)
        {
            CSectionProperties properties = null;
            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["SectionsSQLiteDB"].ConnectionString))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("Select * from tableSections_mm WHERE ID = @id", conn);
                command.Parameters.AddWithValue("@id", ID);

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        properties = new CSectionProperties();
                        properties.ID = reader.GetInt32(reader.GetOrdinal("ID"));
                        properties.sectionName = reader["section"].ToString();
                        properties.h = reader["h"].ToString();
                        properties.b = reader["b"].ToString();
                        properties.t = reader["t"].ToString();
                        properties.A_g = reader["A_g"].ToString();
                        properties.I_y0 = reader["I_y0"].ToString();
                        properties.I_z0 = reader["I_z0"].ToString();
                        properties.W_el_y0 = reader["W_el_y0"].ToString();
                        properties.W_el_z0 = reader["W_el_z0"].ToString();
                        properties.Iyz0 = reader["Iyz0"].ToString();
                        properties.Iy = reader["Iy"].ToString();
                        properties.Iz = reader["Iz"].ToString();
                        properties.W_el_y = reader["W_el_y"].ToString();
                        properties.W_el_z = reader["W_el_z"].ToString();
                        properties.It = reader["It"].ToString();
                        properties.Iw = reader["Iw"].ToString();
                        properties.yc = reader["yc"].ToString();
                        properties.zc = reader["zc"].ToString();
                        properties.ys = reader["ys"].ToString();
                        properties.zs = reader["zs"].ToString();
                        properties.ycs = reader["ycs"].ToString();
                        properties.zcs = reader["zcs"].ToString();
                        properties.betay = reader["betay"].ToString();
                        properties.betaz = reader["betaz"].ToString();
                        properties.alpha_deg = reader["alpha_deg"].ToString();
                        properties.Bending_curve_x1 = reader["Bending_curve_x1"].ToString();
                        properties.Bending_curve_x2 = reader["Bending_curve_x2"].ToString();
                        properties.Bending_curve_x3 = reader["Bending_curve_x3"].ToString();
                        properties.Bending_curve_y = reader["Bending_curve_y"].ToString();
                        properties.Compression_curve_1 = reader["Compression_curve_1"].ToString();
                        properties.Compression_curve_2 = reader["Compression_curve_2"].ToString();
                        properties.Compression_curve_3 = reader["Compression_curve_3"].ToString();
                     }
                }
            }
            return properties;
        }

        public static void LoadCrossSectionProperties(CCrSc_TW crsc, string sectionName)
        {
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";

            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["SectionsSQLiteDB"].ConnectionString))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("Select * from tableSections_mm WHERE section = @section", conn);
                command.Parameters.AddWithValue("@section", sectionName);

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        crsc.ICrSc_ID = reader.GetInt32(reader.GetOrdinal("ID"));
                        crsc.Name = sectionName;
                        crsc.h = double.Parse(reader["h"].ToString(), nfi);
                        crsc.b = double.Parse(reader["b"].ToString(), nfi);
                        crsc.t_min = double.Parse(reader["t"].ToString(), nfi);
                        crsc.t_max = double.Parse(reader["t"].ToString(), nfi);
                        crsc.A_g = double.Parse(reader["A_g"].ToString(), nfi);
                        crsc.I_y0 = double.Parse(reader["I_y0"].ToString(), nfi);
                        crsc.I_z0 = double.Parse(reader["I_z0"].ToString(), nfi);
                        //crsc.W_el_y0 = double.Parse(reader["W_el_y0"].ToString(), nfi);
                        //crsc.W_el_z0 = double.Parse(reader["W_el_z0"].ToString(), nfi);
                        //crsc.Iyz0 = double.Parse(reader["Iyz0"].ToString(), nfi);
                        crsc.I_y = double.Parse(reader["Iy"].ToString(), nfi);
                        crsc.I_z = double.Parse(reader["Iz"].ToString(), nfi);
                        crsc.W_y_el = double.Parse(reader["W_el_y"].ToString(), nfi);
                        crsc.W_z_el = double.Parse(reader["W_el_z"].ToString(), nfi);
                        crsc.I_t = double.Parse(reader["It"].ToString(), nfi);
                        crsc.I_w = double.Parse(reader["Iw"].ToString(), nfi);
                        crsc.D_y_gc = double.Parse(reader["yc"].ToString(), nfi); // Poloha taziska v povodnom suradnicovom systeme
                        crsc.D_z_gc = double.Parse(reader["zc"].ToString(), nfi);
                        crsc.D_y_sc = double.Parse(reader["ys"].ToString(), nfi); // Poloha stredu smyku v povodnom suradnicovom systeme
                        crsc.D_z_sc = double.Parse(reader["zs"].ToString(), nfi);
                        crsc.D_y_s  = double.Parse(reader["ycs"].ToString(), nfi); // Vzdialenost medzi taziskom G a stredom smyku S
                        crsc.D_z_s  = double.Parse(reader["zcs"].ToString(), nfi);
                        crsc.Beta_y = double.Parse(reader["betay"].ToString(), nfi);
                        crsc.Beta_z = double.Parse(reader["betaz"].ToString(), nfi);
                        crsc.Alpha_rad = double.Parse(reader["alpha_deg"].ToString(), nfi) / 180 * Math.PI;
                        crsc.Bending_curve_stress_x1 = double.Parse(reader["Bending_curve_x1"].ToString(), nfi);
                        crsc.Bending_curve_stress_x2 = double.Parse(reader["Bending_curve_x2"].ToString(), nfi);
                        crsc.Bending_curve_stress_x3 = double.Parse(reader["Bending_curve_x3"].ToString(), nfi);
                        crsc.Bending_curve_stress_y = double.Parse(reader["Bending_curve_y"].ToString(), nfi);
                        crsc.Compression_curve_stress_1 = double.Parse(reader["Compression_curve_1"].ToString(), nfi);
                        crsc.Compression_curve_stress_2 = double.Parse(reader["Compression_curve_2"].ToString(), nfi);
                        crsc.Compression_curve_stress_3 = double.Parse(reader["Compression_curve_3"].ToString(), nfi);
                    }
                }
            }
            
        }

        public static List<string> LoadSectionPropertiesStringList(int ID)
        {
            CSectionProperties properties = LoadSectionProperties(ID);

            List<string> list = new List<string>();
            list.Add(properties.h);
            list.Add(properties.b);
            list.Add(properties.t);
            list.Add(properties.A_g);
            list.Add(properties.I_y0);
            list.Add(properties.I_z0);
            list.Add(properties.W_el_y0);
            list.Add(properties.W_el_z0);
            list.Add(properties.Iyz0);
            list.Add(properties.Iy);
            list.Add(properties.Iz);
            list.Add(properties.W_el_y);
            list.Add(properties.W_el_z);
            list.Add(properties.It);
            list.Add(properties.Iw);
            list.Add(properties.yc);
            list.Add(properties.zc);
            list.Add(properties.ys);
            list.Add(properties.zs);
            list.Add(properties.ycs);
            list.Add(properties.zcs);
            list.Add(properties.betay);
            list.Add(properties.betaz);
            list.Add(properties.alpha_deg);
            list.Add(properties.Bending_curve_x1);
            list.Add(properties.Bending_curve_x2);
            list.Add(properties.Bending_curve_x3);
            list.Add(properties.Bending_curve_y);
            list.Add(properties.Compression_curve_1);
            list.Add(properties.Compression_curve_2);
            list.Add(properties.Compression_curve_3);

            return list;
        }

            public static List<CSectionPropertiesText> LoadSectionPropertiesNamesSymbolsUnits()
        {
            CSectionPropertiesText properties;
            List<CSectionPropertiesText> items = new List<CSectionPropertiesText>();

            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["SectionsSQLiteDB"].ConnectionString))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("Select * from sectionProperties", conn);
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        properties = new CSectionPropertiesText();
                        properties.ID = reader.GetInt32(reader.GetOrdinal("ID"));
                        properties.text = reader["propertyText"].ToString();
                        properties.symbol = reader["propertySymbol"].ToString();
                        properties.name = reader["propertyName"].ToString();
                        properties.unit_SI = reader["unit_SI"].ToString();
                        properties.unit_NcmkPa = reader["unit_NcmkPa"].ToString();
                        properties.unit_NmmMpa = reader["unit_NmmMPa"].ToString();
                        items.Add(properties);
                    }
                }
            }
            return items;
        }
    }
}
