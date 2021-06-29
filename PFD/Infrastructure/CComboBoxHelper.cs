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
        private static List<CComboColor> m_ColorList = null;
        public static List<CComboColor> ColorList
        {
            get
            {
                if (m_ColorList == null)
                {
                    m_ColorList = ColorDict.Values.ToList();
                }
                return m_ColorList;
            }
        }

        //public static List<CComboColor> ColorList = new List<CComboColor>() {
        //        new CComboColor("AliceBlue", Colors.AliceBlue),
        //        new CComboColor("AntiqueWhite", Colors.AntiqueWhite),
        //        new CComboColor("Aqua", Colors.Aqua),
        //        new CComboColor("Aquamarine", Colors.Aquamarine),
        //        new CComboColor("Azure", Colors.Azure),
        //        new CComboColor("Beige", Colors.Beige),
        //        new CComboColor("Bisque", Colors.Bisque),
        //        new CComboColor("Black", Colors.Black), // DB
        //        new CComboColor("BlanchedAlmond", Colors.BlanchedAlmond),
        //        new CComboColor("Blue", Colors.Blue), // DB
        //        new CComboColor("BlueViolet", Colors.BlueViolet),
        //        new CComboColor("Brown", Colors.Brown),
        //        new CComboColor("BurlyWood", Colors.BurlyWood),
        //        new CComboColor("CadetBlue", Colors.CadetBlue),
        //        new CComboColor("Chartreuse", Colors.Chartreuse), // DB
        //        new CComboColor("Chocolate", Colors.Chocolate),
        //        new CComboColor("Coral", Colors.Coral),
        //        new CComboColor("CornflowerBlue", Colors.CornflowerBlue),
        //        new CComboColor("Cornsilk", Colors.Cornsilk),
        //        new CComboColor("Crimson", Colors.Crimson), // DB
        //        new CComboColor("Cyan", Colors.Cyan),
        //        new CComboColor("DarkBlue", Colors.DarkBlue),
        //        new CComboColor("DarkCyan", Colors.DarkCyan),
        //        new CComboColor("DarkGoldenrod", Colors.DarkGoldenrod),
        //        new CComboColor("DarkGray", Colors.DarkGray),
        //        new CComboColor("DarkGreen", Colors.DarkGreen), // DB
        //        new CComboColor("DarkKhaki", Colors.DarkKhaki),
        //        new CComboColor("DarkMagenta", Colors.DarkMagenta),
        //        new CComboColor("DarkOliveGreen", Colors.DarkOliveGreen),
        //        new CComboColor("DarkOrange", Colors.DarkOrange), // DB
        //        new CComboColor("DarkOrchid", Colors.DarkOrchid),
        //        new CComboColor("DarkRed", Colors.DarkRed),
        //        new CComboColor("DarkSalmon", Colors.DarkSalmon),
        //        new CComboColor("DarkSeaGreen", Colors.DarkSeaGreen),
        //        new CComboColor("DarkSlateBlue", Colors.DarkSlateBlue),
        //        new CComboColor("DarkSlateGray", Colors.DarkSlateGray),
        //        new CComboColor("DarkTurquoise", Colors.DarkTurquoise),
        //        new CComboColor("DarkViolet", Colors.DarkViolet),
        //        new CComboColor("DeepPink", Colors.DeepPink),
        //        new CComboColor("DeepSkyBlue", Colors.DeepSkyBlue),
        //        new CComboColor("DimGray", Colors.DimGray),
        //        new CComboColor("DodgerBlue", Colors.DodgerBlue),
        //        new CComboColor("Firebrick", Colors.Firebrick),
        //        new CComboColor("FloralWhite", Colors.FloralWhite),
        //        new CComboColor("ForestGreen", Colors.ForestGreen),
        //        new CComboColor("Fuchsia", Colors.Fuchsia), // DB
        //        new CComboColor("Gainsboro", Colors.Gainsboro), // DB
        //        new CComboColor("GhostWhite", Colors.GhostWhite),
        //        new CComboColor("Gold", Colors.Gold),
        //        new CComboColor("Goldenrod", Colors.Goldenrod),
        //        new CComboColor("Gray", Colors.Gray),
        //        new CComboColor("Green", Colors.Green), // DB
        //        new CComboColor("GreenYellow", Colors.GreenYellow), // DB
        //        new CComboColor("Honeydew", Colors.Honeydew),
        //        new CComboColor("HotPink", Colors.HotPink),
        //        new CComboColor("IndianRed", Colors.IndianRed),
        //        new CComboColor("Indigo", Colors.Indigo),
        //        new CComboColor("Ivory", Colors.Ivory),
        //        new CComboColor("Khaki", Colors.Khaki), // DB
        //        new CComboColor("Lavender", Colors.Lavender),
        //        new CComboColor("LavenderBlush", Colors.LavenderBlush),
        //        new CComboColor("LawnGreen", Colors.LawnGreen),
        //        new CComboColor("LemonChiffon", Colors.LemonChiffon),
        //        new CComboColor("LightBlue", Colors.LightBlue),
        //        new CComboColor("LightCoral", Colors.LightCoral),
        //        new CComboColor("LightCyan", Colors.LightCyan),
        //        new CComboColor("LightGoldenrodYellow", Colors.LightGoldenrodYellow),
        //        new CComboColor("LightGray", Colors.LightGray),
        //        new CComboColor("LightGreen", Colors.LightGreen),
        //        new CComboColor("LightPink", Colors.LightPink),
        //        new CComboColor("LightSalmon", Colors.LightSalmon),
        //        new CComboColor("LightSeaGreen", Colors.LightSeaGreen),
        //        new CComboColor("LightSkyBlue", Colors.LightSkyBlue),
        //        new CComboColor("LightSlateGray", Colors.LightSlateGray),
        //        new CComboColor("LightSteelBlue", Colors.LightSteelBlue),
        //        new CComboColor("LightYellow", Colors.LightYellow),
        //        new CComboColor("Lime", Colors.Lime), // DB
        //        new CComboColor("LimeGreen", Colors.LimeGreen),
        //        new CComboColor("Linen", Colors.Linen),
        //        new CComboColor("Magenta", Colors.Magenta),
        //        new CComboColor("Maroon", Colors.Maroon), // DB
        //        new CComboColor("MediumAquamarine", Colors.MediumAquamarine),
        //        new CComboColor("MediumBlue", Colors.MediumBlue),
        //        new CComboColor("MediumOrchid", Colors.MediumOrchid),
        //        new CComboColor("MediumPurple", Colors.MediumPurple), // DB
        //        new CComboColor("MediumSeaGreen", Colors.MediumSeaGreen),
        //        new CComboColor("MediumSlateBlue", Colors.MediumSlateBlue),
        //        new CComboColor("MediumSpringGreen", Colors.MediumSpringGreen),
        //        new CComboColor("MediumTurquoise", Colors.MediumTurquoise),
        //        new CComboColor("MediumVioletRed", Colors.MediumVioletRed), // DB
        //        new CComboColor("MidnightBlue", Colors.MidnightBlue),
        //        new CComboColor("MintCream", Colors.MintCream),
        //        new CComboColor("MistyRose", Colors.MistyRose),
        //        new CComboColor("Moccasin", Colors.Moccasin),
        //        new CComboColor("NavajoWhite", Colors.NavajoWhite),
        //        new CComboColor("Navy", Colors.Navy), // DB
        //        new CComboColor("OldLace", Colors.OldLace),
        //        new CComboColor("Olive", Colors.Olive),
        //        new CComboColor("OliveDrab", Colors.OliveDrab),
        //        new CComboColor("Orange", Colors.Orange), // DB
        //        new CComboColor("OrangeRed", Colors.OrangeRed),
        //        new CComboColor("Orchid", Colors.Orchid),
        //        new CComboColor("PaleGoldenrod", Colors.PaleGoldenrod),
        //        new CComboColor("PaleGreen", Colors.PaleGreen),
        //        new CComboColor("PaleTurquoise", Colors.PaleTurquoise),
        //        new CComboColor("PaleVioletRed", Colors.PaleVioletRed),
        //        new CComboColor("PapayaWhip", Colors.PapayaWhip),
        //        new CComboColor("PeachPuff", Colors.PeachPuff),
        //        new CComboColor("Peru", Colors.Peru),
        //        new CComboColor("Pink", Colors.Pink),
        //        new CComboColor("Plum", Colors.Plum),
        //        new CComboColor("PowderBlue", Colors.PowderBlue),
        //        new CComboColor("Purple", Colors.Purple), // DB
        //        new CComboColor("Red", Colors.Red), // DB
        //        new CComboColor("RosyBrown", Colors.RosyBrown),
        //        new CComboColor("RoyalBlue", Colors.RoyalBlue),
        //        new CComboColor("SaddleBrown", Colors.SaddleBrown),
        //        new CComboColor("Salmon", Colors.Salmon),
        //        new CComboColor("SandyBrown", Colors.SandyBrown),
        //        new CComboColor("SeaGreen", Colors.SeaGreen),
        //        new CComboColor("SeaShell", Colors.SeaShell),
        //        new CComboColor("Sienna", Colors.Sienna),
        //        new CComboColor("Silver", Colors.Silver),
        //        new CComboColor("SkyBlue", Colors.SkyBlue),
        //        new CComboColor("SlateBlue", Colors.SlateBlue),
        //        new CComboColor("SlateGray", Colors.SlateGray),
        //        new CComboColor("Snow", Colors.Snow),
        //        new CComboColor("SpringGreen", Colors.SpringGreen),
        //        new CComboColor("SteelBlue", Colors.SteelBlue),
        //        new CComboColor("Tan", Colors.Tan),
        //        new CComboColor("Teal", Colors.Teal), // DB
        //        new CComboColor("Thistle", Colors.Thistle),
        //        new CComboColor("Tomato", Colors.Tomato),
        //        new CComboColor("Transparent", Colors.Transparent),
        //        new CComboColor("Turquoise", Colors.Turquoise),
        //        new CComboColor("Olive", Colors.Olive), // DB
        //        new CComboColor("Wheat", Colors.Wheat), // DB
        //        new CComboColor("White", Colors.White),
        //        new CComboColor("WhiteSmoke", Colors.WhiteSmoke),
        //        new CComboColor("Yellow", Colors.Yellow), // DB
        //        new CComboColor("YellowGreen", Colors.YellowGreen) // DB
        //};

        private static Dictionary<string, CComboColor> m_ColorDict = null;

        public static Dictionary<string, CComboColor> ColorDict
        {
            get
            {
                if (m_ColorDict == null)
                {
                    m_ColorDict = new Dictionary<string, CComboColor>() {
                        {"AliceBlue", new CComboColor("AliceBlue", Colors.AliceBlue)},
                        {"AntiqueWhite", new CComboColor("AntiqueWhite", Colors.AntiqueWhite) },
                        {"Aqua", new CComboColor("Aqua", Colors.Aqua)},
                        {"Aquamarine", new CComboColor("Aquamarine", Colors.Aquamarine)},
                        {"Azure", new CComboColor("Azure", Colors.Azure)},
                        {"Beige", new CComboColor("Beige", Colors.Beige)},
                        {"Bisque", new CComboColor("Bisque", Colors.Bisque)},
                        {"Black", new CComboColor("Black", Colors.Black)}, // DB
                        {"BlanchedAlmond", new CComboColor("BlanchedAlmond", Colors.BlanchedAlmond)},
                        {"Blue", new CComboColor("Blue", Colors.Blue)}, // DB
                        {"BlueViolet", new CComboColor("BlueViolet", Colors.BlueViolet)},
                        {"Brown", new CComboColor("Brown", Colors.Brown)},
                        {"BurlyWood", new CComboColor("BurlyWood", Colors.BurlyWood)},
                        {"CadetBlue", new CComboColor("CadetBlue", Colors.CadetBlue)},
                        {"Chartreuse", new CComboColor("Chartreuse", Colors.Chartreuse)}, // DB
                        {"Chocolate", new CComboColor("Chocolate", Colors.Chocolate)},
                        {"Coral", new CComboColor("Coral", Colors.Coral)},
                        {"CornflowerBlue", new CComboColor("CornflowerBlue", Colors.CornflowerBlue)},
                        {"Cornsilk", new CComboColor("Cornsilk", Colors.Cornsilk)},
                        {"Crimson", new CComboColor("Crimson", Colors.Crimson)}, // DB
                        {"Cyan", new CComboColor("Cyan", Colors.Cyan)},
                        {"DarkBlue", new CComboColor("DarkBlue", Colors.DarkBlue)},
                        {"DarkCyan", new CComboColor("DarkCyan", Colors.DarkCyan)},
                        {"DarkGoldenrod", new CComboColor("DarkGoldenrod", Colors.DarkGoldenrod)},
                        {"DarkGray", new CComboColor("DarkGray", Colors.DarkGray)},
                        {"DarkGreen", new CComboColor("DarkGreen", Colors.DarkGreen)}, // DB
                        {"DarkKhaki", new CComboColor("DarkKhaki", Colors.DarkKhaki)},
                        {"DarkMagenta", new CComboColor("DarkMagenta", Colors.DarkMagenta)},
                        {"DarkOliveGreen", new CComboColor("DarkOliveGreen", Colors.DarkOliveGreen)},
                        {"DarkOrange", new CComboColor("DarkOrange", Colors.DarkOrange)}, // DB
                        {"DarkOrchid", new CComboColor("DarkOrchid", Colors.DarkOrchid)},
                        {"DarkRed", new CComboColor("DarkRed", Colors.DarkRed)},
                        {"DarkSalmon", new CComboColor("DarkSalmon", Colors.DarkSalmon)},
                        {"DarkSeaGreen", new CComboColor("DarkSeaGreen", Colors.DarkSeaGreen)},
                        {"DarkSlateBlue", new CComboColor("DarkSlateBlue", Colors.DarkSlateBlue)},
                        {"DarkSlateGray", new CComboColor("DarkSlateGray", Colors.DarkSlateGray)},
                        {"DarkTurquoise", new CComboColor("DarkTurquoise", Colors.DarkTurquoise)},
                        {"DarkViolet", new CComboColor("DarkViolet", Colors.DarkViolet)},
                        {"DeepPink", new CComboColor("DeepPink", Colors.DeepPink)},
                        {"DeepSkyBlue", new CComboColor("DeepSkyBlue", Colors.DeepSkyBlue)},
                        {"DimGray", new CComboColor("DimGray", Colors.DimGray)},
                        {"DodgerBlue", new CComboColor("DodgerBlue", Colors.DodgerBlue)},
                        {"Firebrick", new CComboColor("Firebrick", Colors.Firebrick)},
                        {"FloralWhite", new CComboColor("FloralWhite", Colors.FloralWhite)},
                        {"ForestGreen", new CComboColor("ForestGreen", Colors.ForestGreen)},
                        {"Fuchsia", new CComboColor("Fuchsia", Colors.Fuchsia)}, // DB
                        {"Gainsboro", new CComboColor("Gainsboro", Colors.Gainsboro)}, // DB
                        {"GhostWhite", new CComboColor("GhostWhite", Colors.GhostWhite)},
                        {"Gold", new CComboColor("Gold", Colors.Gold)},
                        {"Goldenrod", new CComboColor("Goldenrod", Colors.Goldenrod)},
                        {"Gray", new CComboColor("Gray", Colors.Gray)},
                        {"Green", new CComboColor("Green", Colors.Green)}, // DB
                        {"GreenYellow", new CComboColor("GreenYellow", Colors.GreenYellow)}, // DB
                        {"Honeydew", new CComboColor("Honeydew", Colors.Honeydew)},
                        {"HotPink", new CComboColor("HotPink", Colors.HotPink)},
                        {"IndianRed", new CComboColor("IndianRed", Colors.IndianRed)},
                        {"Indigo", new CComboColor("Indigo", Colors.Indigo)},
                        {"Ivory", new CComboColor("Ivory", Colors.Ivory)},
                        {"Khaki", new CComboColor("Khaki", Colors.Khaki)}, // DB
                        {"Lavender", new CComboColor("Lavender", Colors.Lavender)},
                        {"LavenderBlush", new CComboColor("LavenderBlush", Colors.LavenderBlush)},
                        {"LawnGreen", new CComboColor("LawnGreen", Colors.LawnGreen)},
                        {"LemonChiffon", new CComboColor("LemonChiffon", Colors.LemonChiffon)},
                        {"LightBlue", new CComboColor("LightBlue", Colors.LightBlue)},
                        {"LightCoral", new CComboColor("LightCoral", Colors.LightCoral)},
                        {"LightCyan", new CComboColor("LightCyan", Colors.LightCyan)},
                        {"LightGoldenrodYellow", new CComboColor("LightGoldenrodYellow", Colors.LightGoldenrodYellow)},
                        {"LightGray", new CComboColor("LightGray", Colors.LightGray)},
                        {"LightGreen", new CComboColor("LightGreen", Colors.LightGreen)},
                        {"LightPink", new CComboColor("LightPink", Colors.LightPink)},
                        {"LightSalmon", new CComboColor("LightSalmon", Colors.LightSalmon)},
                        {"LightSeaGreen", new CComboColor("LightSeaGreen", Colors.LightSeaGreen)},
                        {"LightSkyBlue", new CComboColor("LightSkyBlue", Colors.LightSkyBlue)},
                        {"LightSlateGray", new CComboColor("LightSlateGray", Colors.LightSlateGray)},
                        {"LightSteelBlue", new CComboColor("LightSteelBlue", Colors.LightSteelBlue)},
                        {"LightYellow", new CComboColor("LightYellow", Colors.LightYellow)},
                        {"Lime", new CComboColor("Lime", Colors.Lime)}, // DB
                        {"LimeGreen", new CComboColor("LimeGreen", Colors.LimeGreen)},
                        {"Linen", new CComboColor("Linen", Colors.Linen)},
                        {"Magenta", new CComboColor("Magenta", Colors.Magenta)},
                        {"Maroon", new CComboColor("Maroon", Colors.Maroon)}, // DB
                        {"MediumAquamarine", new CComboColor("MediumAquamarine", Colors.MediumAquamarine)},
                        {"MediumBlue", new CComboColor("MediumBlue", Colors.MediumBlue)},
                        {"MediumOrchid", new CComboColor("MediumOrchid", Colors.MediumOrchid)},
                        {"MediumPurple", new CComboColor("MediumPurple", Colors.MediumPurple)}, // DB
                        {"MediumSeaGreen", new CComboColor("MediumSeaGreen", Colors.MediumSeaGreen)},
                        {"MediumSlateBlue", new CComboColor("MediumSlateBlue", Colors.MediumSlateBlue)},
                        {"MediumSpringGreen", new CComboColor("MediumSpringGreen", Colors.MediumSpringGreen)},
                        {"MediumTurquoise", new CComboColor("MediumTurquoise", Colors.MediumTurquoise)},
                        {"MediumVioletRed", new CComboColor("MediumVioletRed", Colors.MediumVioletRed)}, // DB
                        {"MidnightBlue", new CComboColor("MidnightBlue", Colors.MidnightBlue)},
                        {"MintCream", new CComboColor("MintCream", Colors.MintCream)},
                        {"MistyRose", new CComboColor("MistyRose", Colors.MistyRose)},
                        {"Moccasin", new CComboColor("Moccasin", Colors.Moccasin)},
                        {"NavajoWhite", new CComboColor("NavajoWhite", Colors.NavajoWhite)},
                        {"Navy", new CComboColor("Navy", Colors.Navy)}, // DB
                        {"OldLace", new CComboColor("OldLace", Colors.OldLace)},
                        {"Olive", new CComboColor("Olive", Colors.Olive)},
                        {"OliveDrab", new CComboColor("OliveDrab", Colors.OliveDrab)},
                        {"Orange", new CComboColor("Orange", Colors.Orange)}, // DB
                        {"OrangeRed", new CComboColor("OrangeRed", Colors.OrangeRed)},
                        {"Orchid", new CComboColor("Orchid", Colors.Orchid)},
                        {"PaleGoldenrod", new CComboColor("PaleGoldenrod", Colors.PaleGoldenrod)},
                        {"PaleGreen", new CComboColor("PaleGreen", Colors.PaleGreen)},
                        {"PaleTurquoise", new CComboColor("PaleTurquoise", Colors.PaleTurquoise)},
                        {"PaleVioletRed", new CComboColor("PaleVioletRed", Colors.PaleVioletRed)},
                        {"PapayaWhip", new CComboColor("PapayaWhip", Colors.PapayaWhip)},
                        {"PeachPuff", new CComboColor("PeachPuff", Colors.PeachPuff)},
                        {"Peru", new CComboColor("Peru", Colors.Peru)},
                        {"Pink", new CComboColor("Pink", Colors.Pink)},
                        {"Plum", new CComboColor("Plum", Colors.Plum)},
                        {"PowderBlue", new CComboColor("PowderBlue", Colors.PowderBlue)},
                        {"Purple", new CComboColor("Purple", Colors.Purple)}, // DB
                        {"Red", new CComboColor("Red", Colors.Red)}, // DB
                        {"RosyBrown", new CComboColor("RosyBrown", Colors.RosyBrown)},
                        {"RoyalBlue", new CComboColor("RoyalBlue", Colors.RoyalBlue)},
                        {"SaddleBrown", new CComboColor("SaddleBrown", Colors.SaddleBrown)},
                        {"Salmon", new CComboColor("Salmon", Colors.Salmon)},
                        {"SandyBrown", new CComboColor("SandyBrown", Colors.SandyBrown)},
                        {"SeaGreen", new CComboColor("SeaGreen", Colors.SeaGreen)},
                        {"SeaShell", new CComboColor("SeaShell", Colors.SeaShell)},
                        {"Sienna", new CComboColor("Sienna", Colors.Sienna)},
                        {"Silver", new CComboColor("Silver", Colors.Silver)},
                        {"SkyBlue", new CComboColor("SkyBlue", Colors.SkyBlue)},
                        {"SlateBlue", new CComboColor("SlateBlue", Colors.SlateBlue)},
                        {"SlateGray", new CComboColor("SlateGray", Colors.SlateGray)},
                        {"Snow", new CComboColor("Snow", Colors.Snow)},
                        {"SpringGreen", new CComboColor("SpringGreen", Colors.SpringGreen)},
                        {"SteelBlue", new CComboColor("SteelBlue", Colors.SteelBlue)},
                        {"Tan", new CComboColor("Tan", Colors.Tan)},
                        {"Teal", new CComboColor("Teal", Colors.Teal)}, // DB
                        {"Thistle", new CComboColor("Thistle", Colors.Thistle)},
                        {"Tomato", new CComboColor("Tomato", Colors.Tomato)},
                        {"Transparent", new CComboColor("Transparent", Colors.Transparent)},
                        {"Turquoise", new CComboColor("Turquoise", Colors.Turquoise)},
                        {"Wheat", new CComboColor("Wheat", Colors.Wheat)},// DB
                        {"White", new CComboColor("White", Colors.White)},
                        {"WhiteSmoke", new CComboColor("WhiteSmoke", Colors.WhiteSmoke)},
                        {"Yellow", new CComboColor("Yellow", Colors.Yellow)}, // DB
                        {"YellowGreen", new CComboColor("YellowGreen", Colors.YellowGreen)} // DB
                    };
                }
                return m_ColorDict;
            }
        }

        //private static List<CComboColor> m_ColorListWithTransparent = null;
        //public static List<CComboColor> ColorListWithTransparent
        //{
        //    get
        //    {
        //        if (m_ColorListWithTransparent == null)
        //        {
        //            List<CComboColor> colors = new List<CComboColor>(ColorDict.Values);
        //            colors.Insert(0, new CComboColor("Transparent", null));
        //            m_ColorListWithTransparent = colors;
        //        }

        //        return m_ColorListWithTransparent;
        //    }
        //}


        public static string GetColorName(Color color)
        {
            Type colors = typeof(Colors);
            foreach (var prop in colors.GetProperties())
            {
                if (((Color)prop.GetValue(null, null)) == color)
                    return prop.Name;
            }

            throw new Exception("The provided Color is not named.");
        }

        public static int GetColorIndex(Color color)
        {
            for (int i = 0; i < ColorList.Count; i++)
            {
                if (ColorList[i].Name == GetColorName(color))
                    return i;
            }

            // return -1; // Exception
            throw new Exception("The provided Color was not found.");
        }
        //public static int GetColorIndexWithTransparent(Color? color)
        //{
        //    for (int i = 0; i < ColorListWithTransparent.Count; i++)
        //    {
        //        if (color.HasValue)
        //        {
        //            if (ColorListWithTransparent[i].Name == GetColorName(color.Value))
        //                return i;
        //        }
        //        else return 0;
        //    }

        //    // return -1; // Exception
        //    throw new Exception("The provided Color was not found.");
        //}

        public static void FillComboboxValues(string sDBName, string sTableName, string sColumnName, ComboBox combobox)
        {
            combobox.ItemsSource = CDatabaseManager.GetStringList(sDBName, sTableName, sColumnName);
        }

        //public static void FillComboboxWithColors(ComboBox combobox, int[] colorDatabaseIDs = null)
        //{
        //    // DOCASNE - Todo Ondrej, funckiu treba rozsitit tak aby brala len niektore IDs farieb z databazy
        //    // Vsade potom treba nacitavanie parametrov farieb prerobit tak, aby sa nebralo len ID z comboboxu ale uvazovalo sa spravne databazove ID

        //    List<CoatingColour> colours;

        //    if (colorDatabaseIDs == null)
        //    {
        //        // Nacitavame vsetky farby - stare riesenie
        //        colours = CCoatingColorManager.LoadColours("TrapezoidalSheetingSQLiteDB");
        //    }
        //    else
        //    {
        //        // Nenacitavame vsetky farby ale len farby pre prislusny typ coating podla zoznamu IDs priradenemu coating type
        //        // TODO Ondrej - skus to nejako premysliet ako by to slo krajsie, mozno by sa toto malo presnut do CCoatingColorManager
        //        // Vyfiltrujeme farby podla nastaveneho typu coating

        //        colours = new List<CoatingColour>();
        //        for (int i = 0; i < colorDatabaseIDs.Length; i++)
        //        {
        //            CoatingColour color = CCoatingColorManager.LoadCoatingProperties(colorDatabaseIDs[i]);
        //            colours.Add(color);
        //        }
        //    }

        //    combobox.ItemsSource = colours;
        //}

        public static List<CoatingColour> GetCoatingColors(int coatingIndex)
        {
            return CCoatingColorManager.LoadCoatingColours(coatingIndex + 1);
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
