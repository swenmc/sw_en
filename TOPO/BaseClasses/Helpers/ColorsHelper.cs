﻿
using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace BaseClasses.Helpers
{
    public static class ColorsHelper
    {
        private static List<Color> m_ColorList = null;

        public static List<Color> ColorList
        {
            get
            {
                if (m_ColorList == null)
                {
                    m_ColorList = new List<Color>() {
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
                }
                return m_ColorList;
            }
        }


        public static Color GetColorWithIndex(int index)
        {
            int finalIndex = index % ColorList.Count;  //in case index > ColorList.Count

            return ColorList[finalIndex];
        }

    }
}
