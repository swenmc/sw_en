using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using BaseClasses;
using System.Data.SQLite;
using System.Configuration;
using System.Windows.Controls;
using System.Globalization;
using DATABASE;
using DATABASE.DTO;
using BaseClasses.Helpers;

namespace PFD
{
    public static class CComboBoxHelper
    {
        // Zoznam vsetkych farieb v System.Windows.Media.Colors
        // Chcelo by to upravit zoznam a nastavit do comboboxu len niektore vybrane farby
        public static List<CComboColor> ColorList = new List<CComboColor>() {
                new CComboColor("AliceBlue", Colors.AliceBlue),
                new CComboColor("AntiqueWhite", Colors.AntiqueWhite),
                new CComboColor("Aqua", Colors.Aqua),
                new CComboColor("Aquamarine", Colors.Aquamarine),
                new CComboColor("Azure", Colors.Azure),
                new CComboColor("Beige", Colors.Beige),
                new CComboColor("Bisque", Colors.Bisque),
                new CComboColor("Black", Colors.Black), // DB
                new CComboColor("BlanchedAlmond", Colors.BlanchedAlmond),
                new CComboColor("Blue", Colors.Blue), // DB
                new CComboColor("BlueViolet", Colors.BlueViolet),
                new CComboColor("Brown", Colors.Brown),
                new CComboColor("BurlyWood", Colors.BurlyWood),
                new CComboColor("CadetBlue", Colors.CadetBlue),
                new CComboColor("Chartreuse", Colors.Chartreuse), // DB
                new CComboColor("Chocolate", Colors.Chocolate),
                new CComboColor("Coral", Colors.Coral),
                new CComboColor("CornflowerBlue", Colors.CornflowerBlue),
                new CComboColor("Cornsilk", Colors.Cornsilk),
                new CComboColor("Crimson", Colors.Crimson), // DB
                new CComboColor("Cyan", Colors.Cyan),
                new CComboColor("DarkBlue", Colors.DarkBlue),
                new CComboColor("DarkCyan", Colors.DarkCyan),
                new CComboColor("DarkGoldenrod", Colors.DarkGoldenrod),
                new CComboColor("DarkGray", Colors.DarkGray),
                new CComboColor("DarkGreen", Colors.DarkGreen), // DB
                new CComboColor("DarkKhaki", Colors.DarkKhaki),
                new CComboColor("DarkMagenta", Colors.DarkMagenta),
                new CComboColor("DarkOliveGreen", Colors.DarkOliveGreen),
                new CComboColor("DarkOrange", Colors.DarkOrange), // DB
                new CComboColor("DarkOrchid", Colors.DarkOrchid),
                new CComboColor("DarkRed", Colors.DarkRed),
                new CComboColor("DarkSalmon", Colors.DarkSalmon),
                new CComboColor("DarkSeaGreen", Colors.DarkSeaGreen),
                new CComboColor("DarkSlateBlue", Colors.DarkSlateBlue),
                new CComboColor("DarkSlateGray", Colors.DarkSlateGray),
                new CComboColor("DarkTurquoise", Colors.DarkTurquoise),
                new CComboColor("DarkViolet", Colors.DarkViolet),
                new CComboColor("DeepPink", Colors.DeepPink),
                new CComboColor("DeepSkyBlue", Colors.DeepSkyBlue),
                new CComboColor("DimGray", Colors.DimGray),
                new CComboColor("DodgerBlue", Colors.DodgerBlue),
                new CComboColor("Firebrick", Colors.Firebrick),
                new CComboColor("FloralWhite", Colors.FloralWhite),
                new CComboColor("ForestGreen", Colors.ForestGreen),
                new CComboColor("Fuchsia", Colors.Fuchsia), // DB
                new CComboColor("Gainsboro", Colors.Gainsboro), // DB
                new CComboColor("GhostWhite", Colors.GhostWhite),
                new CComboColor("Gold", Colors.Gold),
                new CComboColor("Goldenrod", Colors.Goldenrod),
                new CComboColor("Gray", Colors.Gray),
                new CComboColor("Green", Colors.Green), // DB
                new CComboColor("GreenYellow", Colors.GreenYellow), // DB
                new CComboColor("Honeydew", Colors.Honeydew),
                new CComboColor("HotPink", Colors.HotPink),
                new CComboColor("IndianRed", Colors.IndianRed),
                new CComboColor("Indigo", Colors.Indigo),
                new CComboColor("Ivory", Colors.Ivory),
                new CComboColor("Khaki", Colors.Khaki), // DB
                new CComboColor("Lavender", Colors.Lavender),
                new CComboColor("LavenderBlush", Colors.LavenderBlush),
                new CComboColor("LawnGreen", Colors.LawnGreen),
                new CComboColor("LemonChiffon", Colors.LemonChiffon),
                new CComboColor("LightBlue", Colors.LightBlue),
                new CComboColor("LightCoral", Colors.LightCoral),
                new CComboColor("LightCyan", Colors.LightCyan),
                new CComboColor("LightGoldenrodYellow", Colors.LightGoldenrodYellow),
                new CComboColor("LightGray", Colors.LightGray),
                new CComboColor("LightGreen", Colors.LightGreen),
                new CComboColor("LightPink", Colors.LightPink),
                new CComboColor("LightSalmon", Colors.LightSalmon),
                new CComboColor("LightSeaGreen", Colors.LightSeaGreen),
                new CComboColor("LightSkyBlue", Colors.LightSkyBlue),
                new CComboColor("LightSlateGray", Colors.LightSlateGray),
                new CComboColor("LightSteelBlue", Colors.LightSteelBlue),
                new CComboColor("LightYellow", Colors.LightYellow),
                new CComboColor("Lime", Colors.Lime), // DB
                new CComboColor("LimeGreen", Colors.LimeGreen),
                new CComboColor("Linen", Colors.Linen),
                new CComboColor("Magenta", Colors.Magenta),
                new CComboColor("Maroon", Colors.Maroon), // DB
                new CComboColor("MediumAquamarine", Colors.MediumAquamarine),
                new CComboColor("MediumBlue", Colors.MediumBlue),
                new CComboColor("MediumOrchid", Colors.MediumOrchid),
                new CComboColor("MediumPurple", Colors.MediumPurple), // DB
                new CComboColor("MediumSeaGreen", Colors.MediumSeaGreen),
                new CComboColor("MediumSlateBlue", Colors.MediumSlateBlue),
                new CComboColor("MediumSpringGreen", Colors.MediumSpringGreen),
                new CComboColor("MediumTurquoise", Colors.MediumTurquoise),
                new CComboColor("MediumVioletRed", Colors.MediumVioletRed), // DB
                new CComboColor("MidnightBlue", Colors.MidnightBlue),
                new CComboColor("MintCream", Colors.MintCream),
                new CComboColor("MistyRose", Colors.MistyRose),
                new CComboColor("Moccasin", Colors.Moccasin),
                new CComboColor("NavajoWhite", Colors.NavajoWhite),
                new CComboColor("Navy", Colors.Navy), // DB
                new CComboColor("OldLace", Colors.OldLace),
                new CComboColor("Olive", Colors.Olive),
                new CComboColor("OliveDrab", Colors.OliveDrab),
                new CComboColor("Orange", Colors.Orange), // DB
                new CComboColor("OrangeRed", Colors.OrangeRed),
                new CComboColor("Orchid", Colors.Orchid),
                new CComboColor("PaleGoldenrod", Colors.PaleGoldenrod),
                new CComboColor("PaleGreen", Colors.PaleGreen),
                new CComboColor("PaleTurquoise", Colors.PaleTurquoise),
                new CComboColor("PaleVioletRed", Colors.PaleVioletRed),
                new CComboColor("PapayaWhip", Colors.PapayaWhip),
                new CComboColor("PeachPuff", Colors.PeachPuff),
                new CComboColor("Peru", Colors.Peru),
                new CComboColor("Pink", Colors.Pink),
                new CComboColor("Plum", Colors.Plum),
                new CComboColor("PowderBlue", Colors.PowderBlue),
                new CComboColor("Purple", Colors.Purple), // DB
                new CComboColor("Red", Colors.Red), // DB
                new CComboColor("RosyBrown", Colors.RosyBrown),
                new CComboColor("RoyalBlue", Colors.RoyalBlue),
                new CComboColor("SaddleBrown", Colors.SaddleBrown),
                new CComboColor("Salmon", Colors.Salmon),
                new CComboColor("SandyBrown", Colors.SandyBrown),
                new CComboColor("SeaGreen", Colors.SeaGreen),
                new CComboColor("SeaShell", Colors.SeaShell),
                new CComboColor("Sienna", Colors.Sienna),
                new CComboColor("Silver", Colors.Silver),
                new CComboColor("SkyBlue", Colors.SkyBlue),
                new CComboColor("SlateBlue", Colors.SlateBlue),
                new CComboColor("SlateGray", Colors.SlateGray),
                new CComboColor("Snow", Colors.Snow),
                new CComboColor("SpringGreen", Colors.SpringGreen),
                new CComboColor("SteelBlue", Colors.SteelBlue),
                new CComboColor("Tan", Colors.Tan),
                new CComboColor("Teal", Colors.Teal), // DB
                new CComboColor("Thistle", Colors.Thistle),
                new CComboColor("Tomato", Colors.Tomato),
                new CComboColor("Transparent", Colors.Transparent),
                new CComboColor("Turquoise", Colors.Turquoise),
                new CComboColor("Olive", Colors.Olive), // DB
                new CComboColor("Wheat", Colors.Wheat), // DB
                new CComboColor("White", Colors.White),
                new CComboColor("WhiteSmoke", Colors.WhiteSmoke),
                new CComboColor("Yellow", Colors.Yellow), // DB
                new CComboColor("YellowGreen", Colors.YellowGreen) // DB
        };

        public static void FillComboboxValues(string sDBName, string sTableName, string sColumnName, ComboBox combobox)
        {
            combobox.ItemsSource = CDatabaseManager.GetStringList(sDBName, sTableName, sColumnName);
        }

        public static void FillComboboxWithColors(ComboBox combobox)
        {
            List<CTrapezoidalSheetingColours> colours = CTrapezoidalSheetingManager.LoadTrapezoidalSheetingColours();
            combobox.ItemsSource = colours;

            //List<Tuple<string, string>> color_items = new List<Tuple<string, string>>();

            //// Connect to database and fill items of all comboboxes
            //using (SQLiteConnection conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings[sDBName].ConnectionString))
            //{
            //    conn.Open();
            //    SQLiteCommand command = new SQLiteCommand("Select * from " + sTableName, conn);
            //    using (SQLiteDataReader reader = command.ExecuteReader())
            //    {
            //        while (reader.Read())
            //        {
            //            color_items.Add(Tuple.Create<string, string>(reader[sColumnText].ToString(), "#"+reader[sColumnColor].ToString()));
            //        }
            //    }
            //}
            //combobox.ItemsSource = color_items;
        }

        public static void FillComboboxWithColors_All(ComboBox combobox)
        {
            combobox.ItemsSource = ColorList;
            //combobox.ItemsSource = typeof(Colors).GetProperties();
        }

        // TODO Ondrej - ak je mozne zobecnit tuto funkciu tak, aby to vracalo rozne typy podla typu, aky zistilo v stlpci "sColumnName"
        // Hruza a des. Neverim,ze taku vseobecnu metodu nam treba
        // To bol len taky "napad", ze by to bolo super :)

        public static float GetValueFromDatabasebyRowID(string sDBName, string sTableName, string sColumnName, int IDValue, string sKeyColumnName = "ID")
        {
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";
            float fValue = float.NaN;

            // Connect to database
            using (SQLiteConnection conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings[sDBName].ConnectionString))
            {
                conn.Open();
                SQLiteDataReader reader = null;

                SQLiteCommand command = new SQLiteCommand("Select * from " + sTableName + " where " + sKeyColumnName + " = '" + IDValue + "'", conn);

                using (reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        fValue = float.Parse(reader[sColumnName].ToString(), nfi);
                    }
                }

                reader.Close();
            }

            return fValue;
        }
    }
}
