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
        private static Dictionary<string, CrScProperties> dict_sectionProps = null;

        public static Dictionary<string, CrScProperties> LoadSectionProperties()
        {
            if (dict_sectionProps != null) return dict_sectionProps;

            CrScProperties crsc = null;
            dict_sectionProps = new Dictionary<string, CrScProperties>();

            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["SectionsSQLiteDB"].ConnectionString))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("Select * from tableSections_m", conn); // Nacitavame v metroch
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        crsc = GetCrScProperties(reader);
                        dict_sectionProps.Add(crsc.sectionName_short, crsc);
                    }
                }
            }
            return dict_sectionProps;
        }

        public static CrScProperties GetSectionProperties(string sectionName_short)
        {
            if (string.IsNullOrEmpty(sectionName_short)) return null;

            if(dict_sectionProps == null) LoadSectionProperties();

            CrScProperties crsc = null;
            dict_sectionProps.TryGetValue(sectionName_short, out crsc);
            return crsc;
        }

        public static string GetSectionColor(string sectionName_short)
        {
            CrScProperties prop = GetSectionProperties(sectionName_short);
            if (prop != null) return prop.colorName;
            else return string.Empty;
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
                        properties.VisibleInGUI = bool.Parse(reader["bVisibleInGUI"].ToString());
                        properties.VisibleInReport = bool.Parse(reader["bVisibleInReport"].ToString());
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
        /*
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
        }*/
        /*
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
        */

        private static CSectionProperties LoadSectionProperties_m(string name)
        {
            CSectionProperties properties = null;
            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["SectionsSQLiteDB"].ConnectionString))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("Select * from tableSections_m WHERE sectionName_short = @sectionName_short", conn);
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

        private static CSectionProperties LoadSectionProperties_mm(string name)
        {
            CSectionProperties properties_string_milimeter = new CSectionProperties();
            CrScProperties properties_number_meter = null;

            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["SectionsSQLiteDB"].ConnectionString))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("Select * from tableSections_m WHERE sectionName_short = @sectionName_short", conn);
                command.Parameters.AddWithValue("@sectionName_short", name);

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        properties_number_meter = GetCrScProperties(reader);
                    }
                }
            }

            // Konverzia jednotiek - pre zobrazenie v tabulke
            // From meter [m] and pascal [Pa] to milimeter [mm] and megapascal [MPa]
            // Set decimal places

            float fFactor_Dimension = 1e+3f; // m to mm
            float fFactor_Area = 1e+6f; // m^2 to mm^2
            float fFactor_FirstMomentOfArea = 1e+9f; // m^3 to mm^3
            float fFactor_SecondMomentOfArea = 1e+12f; // m^4 to mm^4
            float fFactor_WarpingConstant = 1e+18f; // m^6 to mm^6

            float fFactor_Stress = 1e-6f; // Pa to MPa

            int iDecimalPlaces_Dimension = 2;
            int iDecimalPlaces_Area = 1;
            int iDecimalPlaces_FirstMomentOfArea = 0;
            int iDecimalPlaces_SecondMomentOfArea = 0;
            int iDecimalPlaces_WarpingConstant = 0;

            int iDecimalPlaces_Stress = 3;

            int iDecimalPlaces_Factor = 3;

            properties_string_milimeter.ID = properties_number_meter.DatabaseID;
            properties_string_milimeter.sectionName_short = properties_number_meter.sectionName_short;
            properties_string_milimeter.sectionName_long = properties_number_meter.sectionName_long;
            properties_string_milimeter.h = (Math.Round(properties_number_meter.h * fFactor_Dimension, iDecimalPlaces_Dimension)).ToString();
            properties_string_milimeter.b = (Math.Round(properties_number_meter.b * fFactor_Dimension, iDecimalPlaces_Dimension)).ToString();
            properties_string_milimeter.t = (Math.Round(properties_number_meter.t_min * fFactor_Dimension, iDecimalPlaces_Dimension)).ToString();
            properties_string_milimeter.A_g = (Math.Round(properties_number_meter.A_g * fFactor_Area, iDecimalPlaces_Area)).ToString();
            properties_string_milimeter.I_y0 = (Math.Round(properties_number_meter.I_y0 * fFactor_SecondMomentOfArea, iDecimalPlaces_SecondMomentOfArea)).ToString();
            properties_string_milimeter.I_z0 = (Math.Round(properties_number_meter.I_z0 * fFactor_SecondMomentOfArea, iDecimalPlaces_SecondMomentOfArea)).ToString();
            //properties_string_milimeter.W_el_y0 = (Math.Round(properties_number_meter.W_el_y0 * fFactor_FirstMomentOfArea, iDecimalPlaces_FirstMomentOfArea)).ToString();
            //properties_string_milimeter.W_el_z0 = (Math.Round(properties_number_meter.W_el_z0 * fFactor_FirstMomentOfArea, iDecimalPlaces_FirstMomentOfArea)).ToString();
            //properties_string_milimeter.Iyz0 = (Math.Round(properties_number_meter.I_yz0 * fFactor_SecondMomentOfArea, iDecimalPlaces_SecondMomentOfArea)).ToString();
            properties_string_milimeter.Iy = (Math.Round(properties_number_meter.I_y * fFactor_SecondMomentOfArea, iDecimalPlaces_SecondMomentOfArea)).ToString();
            properties_string_milimeter.Iz = (Math.Round(properties_number_meter.I_z * fFactor_SecondMomentOfArea, iDecimalPlaces_SecondMomentOfArea)).ToString();
            properties_string_milimeter.W_el_y = (Math.Round(properties_number_meter.W_y_el * fFactor_FirstMomentOfArea, iDecimalPlaces_FirstMomentOfArea)).ToString();
            properties_string_milimeter.W_el_z = (Math.Round(properties_number_meter.W_z_el * fFactor_FirstMomentOfArea, iDecimalPlaces_FirstMomentOfArea)).ToString();
            properties_string_milimeter.It = (Math.Round(properties_number_meter.I_t * fFactor_SecondMomentOfArea, iDecimalPlaces_SecondMomentOfArea)).ToString();
            properties_string_milimeter.Iw = (Math.Round(properties_number_meter.I_w * fFactor_WarpingConstant, iDecimalPlaces_WarpingConstant)).ToString();
            properties_string_milimeter.yc = (Math.Round(properties_number_meter.D_y_gc * fFactor_Dimension, iDecimalPlaces_Dimension)).ToString();
            properties_string_milimeter.zc = (Math.Round(properties_number_meter.D_z_gc * fFactor_Dimension, iDecimalPlaces_Dimension)).ToString();
            properties_string_milimeter.ys = (Math.Round(properties_number_meter.D_y_sc * fFactor_Dimension, iDecimalPlaces_Dimension)).ToString();
            properties_string_milimeter.zs = (Math.Round(properties_number_meter.D_z_sc * fFactor_Dimension, iDecimalPlaces_Dimension)).ToString();
            properties_string_milimeter.ycs = (Math.Round(properties_number_meter.D_y_s * fFactor_Dimension, iDecimalPlaces_Dimension)).ToString();
            properties_string_milimeter.zcs = (Math.Round(properties_number_meter.D_z_s * fFactor_Dimension, iDecimalPlaces_Dimension)).ToString();
            properties_string_milimeter.betay = (Math.Round(properties_number_meter.Beta_y * fFactor_Dimension, iDecimalPlaces_Dimension)).ToString();
            properties_string_milimeter.betaz = (Math.Round(properties_number_meter.Beta_z * fFactor_Dimension, iDecimalPlaces_Dimension)).ToString();
            properties_string_milimeter.alpha_deg = (Math.Round(properties_number_meter.Alpha_rad * 180 / Math.PI, 3)).ToString();
            //properties_string_milimeter.Bending_curve_x1 = (Math.Round(properties_number_meter * fFactor_Stress, iDecimalPlaces_Stress)).ToString();
            //properties_string_milimeter.Bending_curve_x2 = (Math.Round(properties_number_meter * fFactor_Stress, iDecimalPlaces_Stress)).ToString();
            //properties_string_milimeter.Bending_curve_x3 = (Math.Round(properties_number_meter * fFactor_Stress, iDecimalPlaces_Stress)).ToString();
            //properties_string_milimeter.Bending_curve_y = (Math.Round(properties_number_meter * fFactor_Stress, iDecimalPlaces_Stress)).ToString();
            //properties_string_milimeter.Compression_curve_1 = (Math.Round(properties_number_meter * fFactor_Stress, iDecimalPlaces_Stress)).ToString();
            //properties_string_milimeter.Compression_curve_2 = (Math.Round(properties_number_meter * fFactor_Stress, iDecimalPlaces_Stress)).ToString();
            //properties_string_milimeter.Compression_curve_3 = (Math.Round(properties_number_meter * fFactor_Stress, iDecimalPlaces_Stress)).ToString();
            properties_string_milimeter.fol_b = (Math.Round(properties_number_meter.fol_b * fFactor_Stress, iDecimalPlaces_Stress)).ToString();
            properties_string_milimeter.fod_b = (Math.Round(properties_number_meter.fod_b * fFactor_Stress, iDecimalPlaces_Stress)).ToString();
            properties_string_milimeter.fol_c = (Math.Round(properties_number_meter.fol_c * fFactor_Stress, iDecimalPlaces_Stress)).ToString();
            properties_string_milimeter.fod_c = (Math.Round(properties_number_meter.fod_c * fFactor_Stress, iDecimalPlaces_Stress)).ToString();
            properties_string_milimeter.A_stiff = (Math.Round(properties_number_meter.A_stiff * fFactor_Area, iDecimalPlaces_Area)).ToString();
            properties_string_milimeter.n_stiff = properties_number_meter.n_stiff.ToString();
            properties_string_milimeter.y_stiff = (Math.Round(properties_number_meter.y_stiff * fFactor_Dimension, iDecimalPlaces_Dimension)).ToString();
            //properties_string_milimeter.b_1_flat_portion = (Math.Round(properties_number_meter.b_1_flat_portion * fFactor_Dimension, iDecimalPlaces_Dimension)).ToString();
            //properties_string_milimeter.b_tot = (Math.Round(properties_number_meter.b_tot * fFactor_Dimension, iDecimalPlaces_Dimension)).ToString();
            //properties_string_milimeter.b_tot_length = (Math.Round(properties_number_meter.b_tot_length * fFactor_Dimension, iDecimalPlaces_Dimension)).ToString();
            //properties_string_milimeter.A_f1 = (Math.Round(properties_number_meter.A_f1 * fFactor_Area, iDecimalPlaces_Area)).ToString();
            //properties_string_milimeter.A_vy = (Math.Round(properties_number_meter.A_vy * fFactor_Area, iDecimalPlaces_Area)).ToString();
            //properties_string_milimeter.fvy_red_factor = (Math.Round(properties_number_meter.fvy_red_factor, iDecimalPlaces_Factor)).ToString();
            //properties_string_milimeter.d_1_flat_portion = (Math.Round(properties_number_meter.d_1_flat_portion * fFactor_Dimension, iDecimalPlaces_Dimension)).ToString();
            //properties_string_milimeter.d_tot = (Math.Round(properties_number_meter.d_tot * fFactor_Dimension, iDecimalPlaces_Dimension)).ToString();
            //properties_string_milimeter.d_tot_length = (Math.Round(properties_number_meter.d_tot_length * fFactor_Dimension, iDecimalPlaces_Dimension)).ToString();
            //properties_string_milimeter.A_w1 = (Math.Round(properties_number_meter.A_w1 * fFactor_Area, iDecimalPlaces_Area)).ToString();
            //properties_string_milimeter.A_vz = (Math.Round(properties_number_meter.A_vz * fFactor_Area, iDecimalPlaces_Area)).ToString();
            //properties_string_milimeter.fvz_red_factor = (Math.Round(properties_number_meter.fvz_red_factor, iDecimalPlaces_Factor)).ToString();

            return properties_string_milimeter;
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
            properties.r_ee = reader["r_ee"].ToString();
            properties.d_mu = reader["d_mu"].ToString();
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
            crsc.colorName = reader["colorName"].ToString();
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
            crsc.n_stiff = reader["n_stiff"].ToString() == "" ? 0 : int.Parse(reader["n_stiff"].ToString());
            crsc.y_stiff = reader["y_stiff"].ToString() == "" ? double.NaN : double.Parse(reader["y_stiff"].ToString(), nfi);

            crsc.b_1_flat_portion = double.Parse(reader["b_1_flat_portion"].ToString(), nfi);
            crsc.b_tot = double.Parse(reader["b_tot"].ToString(), nfi);
            crsc.b_tot_length = double.Parse(reader["b_tot_length"].ToString(), nfi);
            crsc.A_f1 = double.Parse(reader["A_f1"].ToString(), nfi);
            crsc.A_vy = double.Parse(reader["A_vy"].ToString(), nfi);
            crsc.fvy_red_factor = double.Parse(reader["fvy_red_factor"].ToString(), nfi);
            crsc.d_1_flat_portion = double.Parse(reader["d_1_flat_portion"].ToString(), nfi);
            crsc.r_ee = double.Parse(reader["r_ee"].ToString(), nfi);
            crsc.d_mu = double.Parse(reader["d_mu"].ToString(), nfi);
            crsc.d_tot = double.Parse(reader["d_tot"].ToString(), nfi);
            crsc.d_tot_length = double.Parse(reader["d_tot_length"].ToString(), nfi);
            crsc.A_w1 = double.Parse(reader["A_w1"].ToString(), nfi);
            crsc.A_vz = double.Parse(reader["A_vz"].ToString(), nfi);
            crsc.fvz_red_factor = double.Parse(reader["fvz_red_factor"].ToString(), nfi);

            crsc.IsBuiltUp = bool.Parse(reader["bIsBuiltUp"].ToString());
            crsc.iScrewsNumber =  int.Parse(reader["screwsNumber"].ToString());
            crsc.iScrewsGauge = int.Parse(reader["screwsGauge"].ToString());
            crsc.dScrewDistance = double.Parse(reader["screwsDistance"].ToString(), nfi);

            // Nacitame string so zoznam priemetov sirok
            string ribsProjectionSpacing_mm = reader["ribsProjectionSpacing_mm"].ToString();

            // String prevedieme na pole double
            crsc.ribsProjectionSpacing = StringHelper.ConvertStringArray(ribsProjectionSpacing_mm, ';');

            // Prevedieme z mm na metre
            for(int i =0; i< crsc.ribsProjectionSpacing.Length; i++)
            {
                crsc.ribsProjectionSpacing[i] *= 0.001;
            }

            crsc.dPrice_PPLM_NZD = double.Parse(reader["price_PPLM_NZD"].ToString(), nfi);

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
            list.Add(properties.r_ee);
            list.Add(properties.d_mu);
            list.Add(properties.d_tot);
            list.Add(properties.d_tot_length);
            list.Add(properties.A_w1);
            list.Add(properties.A_vz );
            list.Add(properties.fvz_red_factor);

            return list;
        }
    }
}