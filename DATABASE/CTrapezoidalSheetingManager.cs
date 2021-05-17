using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SQLite;
using System.Globalization;
using System.Linq;
using DATABASE.DTO;
using MATH;

namespace DATABASE
{
    public static class CTrapezoidalSheetingManager
    {
        private static Dictionary<string, CTS_CrscProperties> items = null;

        public static Dictionary<string, CTS_CrscProperties> LoadSectionProperties()
        {
            if (items != null) return items;
            CTS_CrscProperties crsc = null;
            items = new Dictionary<string, CTS_CrscProperties>();

            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["TrapezoidalSheetingSQLiteDB"].ConnectionString))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("Select * from tableSections_m", conn); // Nacitavame v metroch
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        crsc = GetCTS_CrscProperties(reader);
                        items.Add(crsc.name, crsc);
                    }
                }
            }
            return items;
        }

        public static CTS_CrscProperties GetSectionProperties(string sectionName_short)
        {
            Dictionary<string, CTS_CrscProperties> dictItems = LoadSectionProperties();

            CTS_CrscProperties crsc = null;
            dictItems.TryGetValue(sectionName_short, out crsc);
            return crsc;
        }

        public static List<CTS_SectionPropertiesText> LoadSectionPropertiesNamesSymbolsUnits()
        {
            CTS_SectionPropertiesText properties;
            List<CTS_SectionPropertiesText> items = new List<CTS_SectionPropertiesText>();

            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["TrapezoidalSheetingSQLiteDB"].ConnectionString))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("Select * from sectionProperties", conn);
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        properties = new CTS_SectionPropertiesText();
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

        public static CTS_CrscProperties LoadCrossSectionProperties_meters(string sectionName_short)
        {
            CTS_CrscProperties crsc = new CTS_CrscProperties();

            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["TrapezoidalSheetingSQLiteDB"].ConnectionString))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("Select * from tableSections_m WHERE name = @name", conn);
                command.Parameters.AddWithValue("@name", sectionName_short);

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        crsc = GetCTS_CrscProperties(reader);
                    }
                }
            }
            return crsc;
        }
        public static List<string> LoadSectionPropertiesStringList(string name)
        {
            CTS_SectionProperties properties = new CTS_SectionProperties();
            properties = LoadSectionProperties_mm(name);
            return FillListOfSectionPropertiesString(properties);
        }

        private static CTS_SectionProperties LoadSectionProperties_m(string name)
        {
            CTS_SectionProperties properties = null;
            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["TrapezoidalSheetingSQLiteDB"].ConnectionString))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("Select * from tableSections_m WHERE name = @name", conn);
                command.Parameters.AddWithValue("@name", name);

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

        private static CTS_SectionProperties LoadSectionProperties_mm(string name)
        {
            CTS_SectionProperties properties_string_milimeter = new CTS_SectionProperties();
            CTS_CrscProperties properties_number_meter = null;

            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["TrapezoidalSheetingSQLiteDB"].ConnectionString))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("Select * from tableSections_m WHERE name = @name", conn);
                command.Parameters.AddWithValue("@name", name);

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        properties_number_meter = GetCTS_CrscProperties(reader);
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

            // TODO - doupravovat jednotky

            properties_string_milimeter.DatabaseID = properties_number_meter.DatabaseID;
            properties_string_milimeter.name = properties_number_meter.name;
            properties_string_milimeter.widthTot_m = (Math.Round(properties_number_meter.widthTot_m * fFactor_Dimension, iDecimalPlaces_Dimension)).ToString();
            properties_string_milimeter.widthModular_m = (Math.Round(properties_number_meter.widthModular_m * fFactor_Dimension, iDecimalPlaces_Dimension)).ToString();
            properties_string_milimeter.widthRib_m = (Math.Round(properties_number_meter.widthRib_m * fFactor_Dimension, iDecimalPlaces_Dimension)).ToString();
            properties_string_milimeter.widthUpRib_m = (Math.Round(properties_number_meter.widthUpRib_m * fFactor_Dimension, iDecimalPlaces_Dimension)).ToString();
            properties_string_milimeter.height_m = (Math.Round(properties_number_meter.height_m * fFactor_Dimension, iDecimalPlaces_Dimension)).ToString();
            properties_string_milimeter.thickness_m = (Math.Round(properties_number_meter.thicknessCore_m * fFactor_Dimension, iDecimalPlaces_Dimension)).ToString();
            //properties_string_milimeter.mass_kg_m2 = (Math.Round(properties_number_meter.mass_kg_m2 * fFactor_Dimension, iDecimalPlaces_Dimension)).ToString();
            //properties_string_milimeter.mass_kg_lm = (Math.Round(properties_number_meter.mass_kg_lm * fFactor_Dimension, iDecimalPlaces_Dimension)).ToString();
            properties_string_milimeter.maxSimpleSpan = (Math.Round(properties_number_meter.maxSimpleSpan * fFactor_Dimension, iDecimalPlaces_Dimension)).ToString();
            properties_string_milimeter.maxEavesOverhang = (Math.Round(properties_number_meter.maxEavesOverhang * fFactor_Dimension, iDecimalPlaces_Dimension)).ToString();
            properties_string_milimeter.A_g = (Math.Round(properties_number_meter.A_g * fFactor_Area, iDecimalPlaces_Dimension)).ToString();
            properties_string_milimeter.Iy = (Math.Round(properties_number_meter.Iy * fFactor_SecondMomentOfArea, iDecimalPlaces_Dimension)).ToString();
            properties_string_milimeter.Iz = (Math.Round(properties_number_meter.Iz * fFactor_SecondMomentOfArea, iDecimalPlaces_Dimension)).ToString();
            properties_string_milimeter.W_el_y = (Math.Round(properties_number_meter.W_el_y * fFactor_FirstMomentOfArea, iDecimalPlaces_Dimension)).ToString();
            properties_string_milimeter.W_el_y_t = (Math.Round(properties_number_meter.W_el_y_t * fFactor_FirstMomentOfArea, iDecimalPlaces_Dimension)).ToString();
            properties_string_milimeter.W_el_y_b = (Math.Round(properties_number_meter.W_el_y_b * fFactor_FirstMomentOfArea, iDecimalPlaces_Dimension)).ToString();
            properties_string_milimeter.W_el_z = (Math.Round(properties_number_meter.W_el_z * fFactor_FirstMomentOfArea, iDecimalPlaces_Dimension)).ToString();
            properties_string_milimeter.It = (Math.Round(properties_number_meter.It * fFactor_SecondMomentOfArea, iDecimalPlaces_Dimension)).ToString();
            properties_string_milimeter.Iw = (Math.Round(properties_number_meter.Iw * fFactor_WarpingConstant, iDecimalPlaces_Dimension)).ToString();
            properties_string_milimeter.phi_P_no = (Math.Round(properties_number_meter.phi_P_no * fFactor_Dimension, iDecimalPlaces_Dimension)).ToString();
            properties_string_milimeter.A_e = (Math.Round(properties_number_meter.A_e * fFactor_Dimension, iDecimalPlaces_Dimension)).ToString();
            properties_string_milimeter.phi_M_yo_pos = (Math.Round(properties_number_meter.phi_M_yo_pos * fFactor_Dimension, iDecimalPlaces_Dimension)).ToString();
            properties_string_milimeter.I_e_y_pos = (Math.Round(properties_number_meter.I_e_y_pos * fFactor_SecondMomentOfArea, iDecimalPlaces_Dimension)).ToString();
            properties_string_milimeter.W_e_y_t_pos = (Math.Round(properties_number_meter.W_e_y_t_pos * fFactor_FirstMomentOfArea, iDecimalPlaces_Dimension)).ToString();
            properties_string_milimeter.W_e_y_b_pos = (Math.Round(properties_number_meter.W_e_y_b_pos * fFactor_FirstMomentOfArea, iDecimalPlaces_Dimension)).ToString();
            properties_string_milimeter.phi_M_yo_neg = (Math.Round(properties_number_meter.phi_M_yo_neg * fFactor_Dimension, iDecimalPlaces_Dimension)).ToString();
            properties_string_milimeter.I_e_y_neg = (Math.Round(properties_number_meter.I_e_y_neg * fFactor_SecondMomentOfArea, iDecimalPlaces_Dimension)).ToString();
            properties_string_milimeter.W_e_y_t_neg = (Math.Round(properties_number_meter.W_e_y_t_neg * fFactor_FirstMomentOfArea, iDecimalPlaces_Dimension)).ToString();
            properties_string_milimeter.W_e_y_b_neg = (Math.Round(properties_number_meter.W_e_y_b_neg * fFactor_FirstMomentOfArea, iDecimalPlaces_Dimension)).ToString();
            properties_string_milimeter.phi_V_nz = (Math.Round(properties_number_meter.phi_V_nz * fFactor_Dimension, iDecimalPlaces_Dimension)).ToString();

            return properties_string_milimeter;
        }

        private static CTS_SectionProperties GetSectionProperties(SQLiteDataReader reader)
        {
            CTS_SectionProperties properties = new CTS_SectionProperties();
            properties.DatabaseID = reader.GetInt32(reader.GetOrdinal("ID"));
            properties.name = reader["name"].ToString();
            properties.material_Name = reader["material_Name"].ToString();
            properties.widthTot_m = reader["widthTot_m"].ToString();
            properties.widthModular_m = reader["widthModular_m"].ToString();
            properties.widthRib_m = reader["widthRib_m"].ToString();
            properties.widthUpRib_m = reader["widthUpRib_m"].ToString();
            properties.height_m = reader["height_m"].ToString();
            properties.thickness_m = reader["thickness_m"].ToString();
            //properties.mass_kg_m2 = reader["mass_kg_m2"].ToString();
            //properties.mass_kg_lm = reader["mass_kg_lm"].ToString();
            //properties.price_PPSM_NZD = reader["price_PPSM_NZD"].ToString();
            //properties.price_PPLM_NZD = reader["price_PPLM_NZD"].ToString();
            //properties.price_PPKG_NZD = reader["price_PPKG_NZD"].ToString();
            properties.maxSimpleSpan = reader["maxSimpleSpan"].ToString();
            properties.maxEavesOverhang = reader["maxEavesOverhang"].ToString();
            properties.A_g = reader["A_g"].ToString();
            properties.Iy = reader["Iy"].ToString();
            properties.Iz = reader["Iz"].ToString();
            properties.W_el_y = reader["W_el_y"].ToString();
            properties.W_el_y_t = reader["W_el_y_t"].ToString();
            properties.W_el_y_b = reader["W_el_y_b"].ToString();
            properties.W_el_z = reader["W_el_z"].ToString();
            properties.It = reader["It"].ToString();
            properties.Iw = reader["Iw"].ToString();
            properties.phi_P_no = reader["phi_P_no"].ToString();
            properties.A_e = reader["A_e"].ToString();
            properties.phi_M_yo_pos = reader["phi_M_yo_pos"].ToString();
            properties.I_e_y_pos = reader["I_e_y_pos"].ToString();
            properties.W_e_y_t_pos = reader["W_e_y_t_pos"].ToString();
            properties.W_e_y_b_pos = reader["W_e_y_b_pos"].ToString();
            properties.phi_M_yo_neg = reader["phi_M_yo_neg"].ToString();
            properties.I_e_y_neg = reader["I_e_y_neg"].ToString();
            properties.W_e_y_t_neg = reader["W_e_y_t_neg"].ToString();
            properties.W_e_y_b_neg = reader["W_e_y_b_neg"].ToString();
            properties.phi_V_nz = reader["phi_V_nz"].ToString();

            return properties;
        }
        private static CTS_CrscProperties GetCTS_CrscProperties(SQLiteDataReader reader)
        {
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";

            CTS_CrscProperties crsc = new CTS_CrscProperties();
            crsc.DatabaseID = reader.GetInt32(reader.GetOrdinal("ID"));
            crsc.name = reader["name"].ToString();
            crsc.material_Name = reader["material_Name"].ToString();
            crsc.widthCoil_m = double.Parse(reader["widthCoil_m"].ToString(), nfi);
            crsc.widthTot_m = double.Parse(reader["widthTot_m"].ToString(), nfi);
            crsc.widthModular_m = double.Parse(reader["widthModular_m"].ToString(), nfi);
            crsc.widthRib_m = double.Parse(reader["widthRib_m"].ToString(), nfi);
            crsc.widthUpRib_m = double.Parse(reader["widthUpRib_m"].ToString(), nfi);
            crsc.height_m = double.Parse(reader["height_m"].ToString(), nfi);
            crsc.thicknessCore_m = double.Parse(reader["thicknessCore_m"].ToString(), nfi);
            crsc.thicknessTotal_m = double.Parse(reader["thicknessTot_m"].ToString(), nfi);
            //crsc.mass_kg_m2 = double.Parse(reader["mass_kg_m2"].ToString(), nfi);
            //crsc.mass_kg_lm = double.Parse(reader["mass_kg_lm"].ToString(), nfi);
            crsc.coil_IDs = reader["coil_IDs"].ToString();
            crsc.coatingIDs = reader["coatingIDs"].ToString();
            crsc.thicknessID = Int32.Parse(reader["thicknessID"].ToString());
            crsc.maxEavesOverhang = double.Parse(reader["maxEavesOverhang"].ToString(), nfi);
            crsc.A_g = double.Parse(reader["A_g"].ToString(), nfi);
            crsc.Iy = double.Parse(reader["Iy"].ToString(), nfi);
            crsc.Iz = double.Parse(reader["Iz"].ToString(), nfi);
            crsc.W_el_y = double.Parse(reader["W_el_y"].ToString(), nfi);
            crsc.W_el_y_t = double.Parse(reader["W_el_y_t"].ToString(), nfi);
            crsc.W_el_y_b = double.Parse(reader["W_el_y_b"].ToString(), nfi);
            crsc.W_el_z = double.Parse(reader["W_el_z"].ToString(), nfi);
            crsc.It = double.Parse(reader["It"].ToString(), nfi);
            crsc.Iw = double.Parse(reader["Iw"].ToString(), nfi);
            crsc.phi_P_no = double.Parse(reader["phi_P_no"].ToString(), nfi);
            crsc.A_e = double.Parse(reader["A_e"].ToString(), nfi);
            crsc.phi_M_yo_pos = double.Parse(reader["phi_M_yo_pos"].ToString(), nfi);
            crsc.I_e_y_pos = double.Parse(reader["I_e_y_pos"].ToString(), nfi);
            crsc.W_e_y_t_pos = double.Parse(reader["W_e_y_t_pos"].ToString(), nfi);
            crsc.W_e_y_b_pos = double.Parse(reader["W_e_y_b_pos"].ToString(), nfi);
            crsc.phi_M_yo_neg = double.Parse(reader["phi_M_yo_neg"].ToString(), nfi);
            crsc.I_e_y_neg = double.Parse(reader["I_e_y_neg"].ToString(), nfi);
            crsc.W_e_y_t_neg = double.Parse(reader["W_e_y_t_neg"].ToString(), nfi);
            crsc.W_e_y_b_neg = double.Parse(reader["W_e_y_b_neg"].ToString(), nfi);
            crsc.phi_V_nz = double.Parse(reader["phi_V_nz"].ToString(), nfi);

            return crsc;
        }
        private static List<string> FillListOfSectionPropertiesString(CTS_SectionProperties properties)
        {
            List<string> list = new List<string>();
            list.Add(properties.widthTot_m);
            list.Add(properties.widthModular_m);
            list.Add(properties.widthRib_m);
            list.Add(properties.widthUpRib_m);
            list.Add(properties.height_m);
            list.Add(properties.thickness_m);
            //list.Add(properties.mass_kg_m2);
            //list.Add(properties.mass_kg_lm);
            list.Add(properties.maxSimpleSpan);
            list.Add(properties.maxEavesOverhang);
            list.Add(properties.A_g);
            list.Add(properties.Iy);
            list.Add(properties.Iz);
            list.Add(properties.W_el_y);
            list.Add(properties.W_el_y_t);
            list.Add(properties.W_el_y_b);
            list.Add(properties.W_el_z);
            list.Add(properties.It);
            list.Add(properties.Iw);
            list.Add(properties.phi_P_no);
            list.Add(properties.A_e);
            list.Add(properties.phi_M_yo_pos);
            list.Add(properties.I_e_y_pos);
            list.Add(properties.W_e_y_t_pos);
            list.Add(properties.W_e_y_b_pos);
            list.Add(properties.phi_M_yo_neg);
            list.Add(properties.I_e_y_neg);
            list.Add(properties.W_e_y_t_neg);
            list.Add(properties.W_e_y_b_neg);
            list.Add(properties.phi_V_nz);

            return list;
        }


        // Coating
        private static Dictionary<string, CTS_CoatingProperties> coatingItems = null;

        public static Dictionary<string, CTS_CoatingProperties> LoadCoatingProperties()
        {
            if (coatingItems != null) return coatingItems;
            CTS_CoatingProperties coatingProp = null;
            coatingItems = new Dictionary<string, CTS_CoatingProperties>();

            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["TrapezoidalSheetingSQLiteDB"].ConnectionString))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("Select * from coating", conn);
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        coatingProp = GetCoatingProperties(reader);
                        coatingItems.Add(coatingProp.Name, coatingProp);
                    }
                }
            }
            return coatingItems;
        }

        public static List<CTS_CoatingProperties> LoadCoatingPropertiesList()
        {
            CTS_CoatingProperties properties;
            List<CTS_CoatingProperties> items = new List<CTS_CoatingProperties>();

            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["TrapezoidalSheetingSQLiteDB"].ConnectionString))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("Select * from coating", conn);
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        properties = new CTS_CoatingProperties();
                        properties = GetCoatingProperties(reader);
                        items.Add(properties);
                    }
                }
            }
            return items;
        }

        public static CTS_CoatingProperties LoadCoatingProperties(string name)
        {
            CTS_CoatingProperties properties = null;
            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["TrapezoidalSheetingSQLiteDB"].ConnectionString))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("Select * from coating WHERE name_short = @name_short", conn);
                command.Parameters.AddWithValue("@name_short", name);

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        properties = GetCoatingProperties(reader);
                    }
                }
            }
            return properties;
        }

        public static CTS_CoatingProperties GetCoatingProperties(SQLiteDataReader reader)
        {
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";

            CTS_CoatingProperties coatingprop = new CTS_CoatingProperties();
            coatingprop.ID = reader.GetInt32(reader.GetOrdinal("ID"));
            coatingprop.Name = reader["name_short"].ToString();
            coatingprop.ColorIDs = StringHelper.ConvertStringArrayOfIDs(reader["colorRangeIDs"].ToString());
            //coatingprop.PriceCode = Int32.Parse(reader["priceCode"].ToString(), nfi);

            return coatingprop;
        }

        // Coil
        private static Dictionary<int, CTS_CoilProperties> coilItems = null;

        public static Dictionary<int, CTS_CoilProperties> LoadCoilProperties()
        {
            if (coilItems != null) return coilItems;
            CTS_CoilProperties coilProp = null;
            coilItems = new Dictionary<int, CTS_CoilProperties>();

            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["TrapezoidalSheetingSQLiteDB"].ConnectionString))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("Select * from coils", conn);
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        coilProp = GetCoilProperties(reader);
                        coilItems.Add(coilProp.ID, coilProp);
                    }
                }
            }
            return coilItems;
        }

        private static List<CTS_CoilProperties> coilItemsList = null;
        public static List<CTS_CoilProperties> LoadCoilPropertiesList()
        {
            CTS_CoilProperties properties;
            if (coilItemsList != null) return coilItemsList;

            List<CTS_CoilProperties> items = new List<CTS_CoilProperties>();

            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["TrapezoidalSheetingSQLiteDB"].ConnectionString))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("Select * from coils", conn);
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        properties = new CTS_CoilProperties();
                        properties = GetCoilProperties(reader);
                        items.Add(properties);
                    }
                }
            }
            coilItemsList = items;
            return items;
        }

        public static CTS_CoilProperties LoadCoilProperties(int id)
        {
            CTS_CoilProperties properties = null;
            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["TrapezoidalSheetingSQLiteDB"].ConnectionString))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("Select * from coils WHERE ID = @id", conn);
                command.Parameters.AddWithValue("@id", id);

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        properties = GetCoilProperties(reader);
                    }
                }
            }
            return properties;
        }

        public static CTS_CoilProperties LoadCoilProperties(double coilWidth, int thicknessID, int coatingID, int colorID)
        {
            List<CTS_CoilProperties> items = LoadCoilPropertiesList();
            
            CTS_CoilProperties coil = items.FirstOrDefault(c => (MathF.d_equal(c.widthCoil, coilWidth)) &&
                            (c.thicknessID == thicknessID) &&
                            (c.coatingID == coatingID) &&
                            (c.colorRangeIDs.Contains(colorID)));

            return coil;
        }

        public static CTS_CoilProperties GetCoilProperties(SQLiteDataReader reader)
        {
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";

            CTS_CoilProperties coilprop = new CTS_CoilProperties();
            coilprop.ID = reader.GetInt32(reader.GetOrdinal("ID"));
            coilprop.widthCoil = double.Parse(reader["widthCoil_mm"].ToString(), nfi) / 1000;
            coilprop.thicknessID = Int32.Parse(reader["thicknessID"].ToString());
            coilprop.thicknessCore = double.Parse(reader["thicknessCore_mm"].ToString(), nfi) / 1000;
            coilprop.coatingID = Int32.Parse(reader["coatingID"].ToString());
            coilprop.coatingName = reader["coatingName"].ToString();
            coilprop.colorRangeIDs = StringHelper.ConvertStringArrayOfIDs(reader["colorRangeIDs"].ToString());
            coilprop.mass_kg_lm = double.Parse(reader["mass_kg_lm"].ToString(), nfi);
            coilprop.coilmass_kg_m2 = double.Parse(reader["coilmass_kg_m2"].ToString(), nfi);
            coilprop.coilLengthPerTonne = double.Parse(reader["coilLengthPerTonne"].ToString(), nfi);
            coilprop.price_PPTONNE_NZD = double.Parse(reader["price_PPTONNE_NZD"].ToString(), nfi);
            coilprop.price_PPLM_NZD = double.Parse(reader["price_PPLM_NZD"].ToString(), nfi);
            coilprop.price_PPSM_NZD = double.Parse(reader["price_PPSM_NZD"].ToString(), nfi);

            return coilprop;
        }

        public static CTS_CoilProperties GetCladdingCoilProperties(CTS_CoatingProperties coatingProp, CoatingColour color, CTS_CrscProperties prop)
        {
            // Potrebujeme vyhladat vlastnosti coil, asi by to bolo lepsie cez SQL dotaz priamo v databaze ???
            //optimalizovane na vykon ale nacitava sa aj tak cely zoznam, problem nacitania priamo z DB je kvoli ColorRangeIDs

            // Vstupne parametre pre zistenie coil

            // Coil Width (Smartdek a Speedclad 940 mm, Purlindek 860 mm)
            double coilWidth = prop.widthCoil_m;

            // Thickness alebo Thickness ID
            //double thickness = prop.thicknessCore_m;
            int thicknessID = prop.thicknessID;

            // coating ID alebo coatingName
            //string coatingName = coatingProp.Name;
            int coatingID = coatingProp.ID;

            // color ID in colorRangeIDs (je potrebne pre Formclad IDs 14-17 alebo 18)
            int colorID = color.ID;

            CTS_CoilProperties coil = CTrapezoidalSheetingManager.LoadCoilProperties(coilWidth, thicknessID, coatingID, colorID);

            if (coil != null)
                return coil;
            else
            {
                throw new Exception("Unable to find coil in the database.");
            }
        }

        // Thickness
        private static Dictionary<int, CTS_ThicknessProperties> thicknessItems = null;

        public static Dictionary<int, CTS_ThicknessProperties> LoadThicknessProperties()
        {
            if (thicknessItems != null) return thicknessItems;
            CTS_ThicknessProperties thicknessProp = null;
            thicknessItems = new Dictionary<int, CTS_ThicknessProperties>();

            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["TrapezoidalSheetingSQLiteDB"].ConnectionString))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("Select * from thicknesses", conn);
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        thicknessProp = GetThicknessProperties(reader);
                        thicknessItems.Add(thicknessProp.ID, thicknessProp);
                    }
                }
            }
            return thicknessItems;
        }

        public static List<CTS_ThicknessProperties> LoadThicknessPropertiesList()
        {
            CTS_ThicknessProperties properties;
            List<CTS_ThicknessProperties> items = new List<CTS_ThicknessProperties>();

            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["TrapezoidalSheetingSQLiteDB"].ConnectionString))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("Select * from thicknesses", conn);
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        properties = new CTS_ThicknessProperties();
                        properties = GetThicknessProperties(reader);
                        items.Add(properties);
                    }
                }
            }
            return items;
        }

        public static CTS_ThicknessProperties LoadThicknessProperties(int id)
        {
            CTS_ThicknessProperties properties = null;
            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["TrapezoidalSheetingSQLiteDB"].ConnectionString))
            {
                conn.Open();
                SQLiteCommand command = new SQLiteCommand("Select * from thicknesses WHERE ID = @id", conn);
                command.Parameters.AddWithValue("@id", id);

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        properties = GetThicknessProperties(reader);
                    }
                }
            }
            return properties;
        }

        public static CTS_ThicknessProperties GetThicknessProperties(SQLiteDataReader reader)
        {
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";

            CTS_ThicknessProperties thicknessprop = new CTS_ThicknessProperties();
            thicknessprop.ID = reader.GetInt32(reader.GetOrdinal("ID"));
            thicknessprop.thicknessCore = double.Parse(reader["thicknessCore_mm"].ToString(), nfi) / 100f;
            thicknessprop.thicknessTot = double.Parse(reader["thicknessTot_mm"].ToString(), nfi) / 100f;
            thicknessprop.claddingIDs = StringHelper.ConvertStringArrayOfIDs(reader["claddingIDs"].ToString());
            thicknessprop.coatingIDs = StringHelper.ConvertStringArrayOfIDs(reader["coatingIDs"].ToString());
            thicknessprop.coil_IDs = StringHelper.ConvertStringArrayOfIDs(reader["coil_IDs"].ToString());

            return thicknessprop;
        }








        
    }
}