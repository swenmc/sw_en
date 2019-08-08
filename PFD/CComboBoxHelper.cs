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

namespace PFD
{
    public static class CComboBoxHelper
    {
        // Skopirovane CComponentListVM
        // Zoznam vsetkych farieb v System.Windows.Media.Colors
        // Chcelo by to upravit zoznam a nastavit do comboboxu len niektore vybrane farby
        public static List<Color> ColorList = new List<Color>() {
                Colors.AliceBlue,
                Colors.AntiqueWhite,
                Colors.Aqua,
                Colors.Aquamarine,
                Colors.Azure,
                Colors.Beige,
                Colors.Bisque,
                Colors.Black,
                Colors.BlanchedAlmond,
                Colors.Blue,
                Colors.BlueViolet,
                Colors.Brown,
                Colors.BurlyWood,
                Colors.CadetBlue,
                Colors.Chartreuse,
                Colors.Chocolate,
                Colors.Coral,
                Colors.CornflowerBlue,
                Colors.Cornsilk,
                Colors.Crimson,
                Colors.Cyan,
                Colors.DarkBlue,
                Colors.DarkCyan,
                Colors.DarkGoldenrod,
                Colors.DarkGray,
                Colors.DarkGreen,
                Colors.DarkKhaki,
                Colors.DarkMagenta,
                Colors.DarkOliveGreen,
                Colors.DarkOrange,
                Colors.DarkOrchid,
                Colors.DarkRed,
                Colors.DarkSalmon,
                Colors.DarkSeaGreen,
                Colors.DarkSlateBlue,
                Colors.DarkSlateGray,
                Colors.DarkTurquoise,
                Colors.DarkViolet,
                Colors.DeepPink,
                Colors.DeepSkyBlue,
                Colors.DimGray,
                Colors.DodgerBlue,
                Colors.Firebrick,
                Colors.FloralWhite,
                Colors.ForestGreen,
                Colors.Fuchsia,
                Colors.Gainsboro,
                Colors.GhostWhite,
                Colors.Gold,
                Colors.Goldenrod,
                Colors.Gray,
                Colors.Green,
                Colors.GreenYellow,
                Colors.Honeydew,
                Colors.HotPink,
                Colors.IndianRed,
                Colors.Indigo,
                Colors.Ivory,
                Colors.Khaki,
                Colors.Lavender,
                Colors.LavenderBlush,
                Colors.LawnGreen,
                Colors.LemonChiffon,
                Colors.LightBlue,
                Colors.LightCoral,
                Colors.LightCyan,
                Colors.LightGoldenrodYellow,
                Colors.LightGray,
                Colors.LightGreen,
                Colors.LightPink,
                Colors.LightSalmon,
                Colors.LightSeaGreen,
                Colors.LightSkyBlue,
                Colors.LightSlateGray,
                Colors.LightSteelBlue,
                Colors.LightYellow,
                Colors.Lime,
                Colors.LimeGreen,
                Colors.Linen,
                Colors.Magenta,
                Colors.Maroon,
                Colors.MediumAquamarine,
                Colors.MediumBlue,
                Colors.MediumOrchid,
                Colors.MediumPurple,
                Colors.MediumSeaGreen,
                Colors.MediumSlateBlue,
                Colors.MediumSpringGreen,
                Colors.MediumTurquoise,
                Colors.MediumVioletRed,
                Colors.MidnightBlue,
                Colors.MintCream,
                Colors.MistyRose,
                Colors.Moccasin,
                Colors.NavajoWhite,
                Colors.Navy,
                Colors.OldLace,
                Colors.Olive,
                Colors.OliveDrab,
                Colors.Orange,
                Colors.OrangeRed,
                Colors.Orchid,
                Colors.PaleGoldenrod,
                Colors.PaleGreen,
                Colors.PaleTurquoise,
                Colors.PaleVioletRed,
                Colors.PapayaWhip,
                Colors.PeachPuff,
                Colors.Peru,
                Colors.Pink,
                Colors.Plum,
                Colors.PowderBlue,
                Colors.Purple,
                Colors.Red,
                Colors.RosyBrown,
                Colors.RoyalBlue,
                Colors.SaddleBrown,
                Colors.Salmon,
                Colors.SandyBrown,
                Colors.SeaGreen,
                Colors.SeaShell,
                Colors.Sienna,
                Colors.Silver,
                Colors.SkyBlue,
                Colors.SlateBlue,
                Colors.SlateGray,
                Colors.Snow,
                Colors.SpringGreen,
                Colors.SteelBlue,
                Colors.Tan,
                Colors.Teal,
                Colors.Thistle,
                Colors.Tomato,
                Colors.Transparent,
                Colors.Turquoise,
                Colors.Olive,
                Colors.Wheat,
                Colors.White,
                Colors.WhiteSmoke,
                Colors.Yellow,
                Colors.YellowGreen
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
            //combobox.ItemsSource = ColorList;
            combobox.ItemsSource = typeof(Colors).GetProperties();
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
