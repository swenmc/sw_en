using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SQLite;
using System.Globalization;
using DATABASE.DTO;

namespace DATABASE
{
    public static class CSectionManager
    {
        private static Dictionary<string, CrScProperties> items = null;

        public static Dictionary<string, CrScProperties> LoadSectionProperties()
        {
            if (items != null) return items;
            CrScProperties crsc = null;
            items = new Dictionary<string, CrScProperties>();

            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["SectionsSQLiteDB"].ConnectionString))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("Select * from tableSections_m", conn); // Nacitavame v metroch
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        crsc = GetCrScProperties(reader);
                        items.Add(crsc.sectionName_short, crsc);
                    }
                }
            }
            return items;
        }

        public static CrScProperties GetSectionProperties(string sectionName_short)
        {
            Dictionary<string, CrScProperties> dictItems = LoadSectionProperties();

            CrScProperties crsc = null;
            dictItems.TryGetValue(sectionName_short, out crsc);
            return crsc;
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
        public static CrScProperties LoadCrossSectionProperties_meters(string sectionName_short)
        {
            CrScProperties crsc = new CrScProperties();

            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["SectionsSQLiteDB"].ConnectionString))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("Select * from tableSections_m WHERE sectionName_short = @sectionName_short", conn);
                command.Parameters.AddWithValue("@sectionName_short", sectionName_short);

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        crsc = GetCrScProperties(reader);
                    }
                }
            }
            return crsc;
        }
        public static List<string> LoadSectionPropertiesStringList(string name)
        {
            CSectionProperties properties = new CSectionProperties();
            properties = LoadSectionProperties_mm(name);
            return FillListOfSectionPropertiesString(properties);
        }
        private static CSectionProperties LoadSectionProperties_mm(int ID)
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
                        properties = GetSectionProperties(reader);
                    }
                }
            }
            return properties;
        }
        private static CSectionProperties LoadSectionProperties_mm(string name)
        {
            CSectionProperties properties = null;
            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["SectionsSQLiteDB"].ConnectionString))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("Select * from tableSections_mm WHERE sectionName_short = @sectionName_short", conn);
                command.Parameters.AddWithValue("@sectionName_short", name);

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        properties = GetSectionProperties(reader);
                    }
                }
            }
            return properties;
        }
        private static CSectionProperties GetSectionProperties(SQLiteDataReader reader)
        {
            CSectionProperties properties = new CSectionProperties();
            properties.ID = reader.GetInt32(reader.GetOrdinal("ID"));
            properties.sectionName_short = reader["sectionName_short"].ToString();
            properties.sectionName_long = reader["sectionName_long"].ToString();
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
            properties.fol_b = reader["fol_b"].ToString();
            properties.fod_b = reader["fod_b"].ToString();
            properties.fol_c = reader["fol_c"].ToString();
            properties.fod_c = reader["fod_c"].ToString();
            properties.A_stiff = reader["A_stiff"].ToString();
            properties.n_stiff = reader["n_stiff"].ToString();
            properties.y_stiff = reader["y_stiff"].ToString();
            properties.b_1_flat_portion = reader["b_1_flat_portion"].ToString();
            properties.b_tot = reader["b_tot"].ToString();
            properties.b_tot_length = reader["b_tot_length"].ToString();
            properties.A_f1 = reader["A_f1"].ToString();
            properties.A_vy = reader["A_vy"].ToString();
            properties.fvy_red_factor = reader["fvy_red_factor"].ToString();
            properties.d_1_flat_portion = reader["d_1_flat_portion"].ToString();
            properties.d_tot = reader["d_tot"].ToString();
            properties.d_tot_length = reader["d_tot_length"].ToString();
            properties.A_w1 = reader["A_w1"].ToString();
            properties.A_vz = reader["A_vz"].ToString();
            properties.fvz_red_factor = reader["fvz_red_factor"].ToString();

            return properties;
        }
        private static CrScProperties GetCrScProperties(SQLiteDataReader reader)
        {
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";

            CrScProperties crsc = new CrScProperties();
            crsc.DatabaseID = reader.GetInt32(reader.GetOrdinal("ID"));
            crsc.sectionName_short = reader["sectionName_short"].ToString();
            crsc.sectionName_long = reader["sectionName_long"].ToString();
            crsc.sectionColorName = reader["sectionColorName"].ToString();
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
            crsc.D_y_s = double.Parse(reader["ycs"].ToString(), nfi); // Vzdialenost medzi taziskom G a stredom smyku S
            crsc.D_z_s = double.Parse(reader["zcs"].ToString(), nfi);
            crsc.Beta_y = double.Parse(reader["betay"].ToString(), nfi);
            crsc.Beta_z = double.Parse(reader["betaz"].ToString(), nfi);
            crsc.Alpha_rad = double.Parse(reader["alpha_deg"].ToString(), nfi) / 180 * Math.PI;
            //crsc.Bending_curve_stress_x1 = double.Parse(reader["Bending_curve_x1"].ToString(), nfi);
            //crsc.Bending_curve_stress_x2 = double.Parse(reader["Bending_curve_x2"].ToString(), nfi);
            //crsc.Bending_curve_stress_x3 = double.Parse(reader["Bending_curve_x3"].ToString(), nfi);
            //crsc.Bending_curve_stress_y = double.Parse(reader["Bending_curve_y"].ToString(), nfi);
            //crsc.Compression_curve_stress_1 = double.Parse(reader["Compression_curve_1"].ToString(), nfi);
            //crsc.Compression_curve_stress_2 = double.Parse(reader["Compression_curve_2"].ToString(), nfi);
            //crsc.Compression_curve_stress_3 = double.Parse(reader["Compression_curve_3"].ToString(), nfi);

            crsc.fol_b = reader["fol_b"].ToString() == "" ? double.NaN : double.Parse(reader["fol_b"].ToString(), nfi);
            crsc.fod_b = reader["fod_b"].ToString() == "" ? double.NaN : double.Parse(reader["fod_b"].ToString(), nfi);
            crsc.fol_c = reader["fol_c"].ToString() == "" ? double.NaN : double.Parse(reader["fol_c"].ToString(), nfi);
            crsc.fod_c = reader["fod_c"].ToString() == "" ? double.NaN : double.Parse(reader["fod_c"].ToString(), nfi);

            crsc.A_stiff = reader["A_stiff"].ToString() == "" ? double.NaN : double.Parse(reader["A_stiff"].ToString(), nfi);
            crsc.n_stiff = reader["n_stiff"].ToString() == "" ? 0 : int.Parse(reader["n_stiff"].ToString(), nfi);
            crsc.y_stiff = reader["y_stiff"].ToString() == "" ? double.NaN : double.Parse(reader["y_stiff"].ToString(), nfi);

            crsc.b_1_flat_portion = double.Parse(reader["b_1_flat_portion"].ToString(), nfi);
            crsc.b_tot = double.Parse(reader["b_tot"].ToString(), nfi);
            crsc.b_tot_length = double.Parse(reader["b_tot_length"].ToString(), nfi);
            crsc.A_f1 = double.Parse(reader["A_f1"].ToString(), nfi);
            crsc.A_vy = double.Parse(reader["A_vy"].ToString(), nfi);
            crsc.fvy_red_factor = double.Parse(reader["fvy_red_factor"].ToString(), nfi);
            crsc.d_1_flat_portion = double.Parse(reader["d_1_flat_portion"].ToString(), nfi);
            crsc.d_tot = double.Parse(reader["d_tot"].ToString(), nfi);
            crsc.d_tot_length = double.Parse(reader["d_tot_length"].ToString(), nfi);
            crsc.A_w1 = double.Parse(reader["A_w1"].ToString(), nfi);
            crsc.A_vz = double.Parse(reader["A_vz"].ToString(), nfi);
            crsc.fvz_red_factor = double.Parse(reader["fvz_red_factor"].ToString(), nfi);
            return crsc;
        }
        private static List<string> FillListOfSectionPropertiesString(CSectionProperties properties)
        {
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
            list.Add(properties.fol_b);
            list.Add(properties.fod_b);
            list.Add(properties.fol_c);
            list.Add(properties.fod_c);
            list.Add(properties.A_stiff);
            list.Add(properties.n_stiff);
            list.Add(properties.y_stiff);
            list.Add(properties.b_1_flat_portion);
            list.Add(properties.b_tot);
            list.Add(properties.b_tot_length);
            list.Add(properties.A_f1);
            list.Add(properties.A_vy);
            list.Add(properties.fvy_red_factor);
            list.Add(properties.d_1_flat_portion);
            list.Add(properties.d_tot);
            list.Add(properties.d_tot_length);
            list.Add(properties.A_w1);
            list.Add(properties.A_vz );
            list.Add(properties.fvz_red_factor);

            return list;
        }
    }
}