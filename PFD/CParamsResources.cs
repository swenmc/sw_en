using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFD
{
    public static class CParamsResources
    {
        // UNITS
        public const string sUnit_PlateLength = "[mm]";

        // PLATES
        public struct PlateNameS
        {
            public const string Name = "Name";
            public const string DBName = " ";
            public const string Unit = " ";
            public const string Symbol = " ";
        }

        public struct PlateThicknessS
        {
            public const string Name = "Thickness";
            public const string DBName = "PlateThick";
            public const string Unit = sUnit_PlateLength;
            public const string Symbol = "t";
        }

        public struct PlateWidthS
        {
            public const string Name = "Width";
            public const string DBName = "PlateWidth";
            public const string Unit = sUnit_PlateLength;
            public const string Symbol = "b";
        }

        public struct PlateWidth1S
        {
            public const string Name = "Width 1";
            public const string DBName = "PlateWidth1";
            public const string Unit = sUnit_PlateLength;
            public const string Symbol = "b1";
        }

        public struct PlateWidth2S
        {
            public const string Name = "Width 2";
            public const string DBName = "PlateWidth2";
            public const string Unit = sUnit_PlateLength;
            public const string Symbol = "b2";
        }

        public struct PlateHeightS
        {
            public const string Name = "Height";
            public const string DBName = "PlateHeight";
            public const string Unit = sUnit_PlateLength;
            public const string Symbol = "h";
        }

        public struct PlateHeight1S
        {
            public const string Name = "Height 1";
            public const string DBName = "PlateHeight1";
            public const string Unit = sUnit_PlateLength;
            public const string Symbol = "h1";
        }

        public struct PlateHeight2S
        {
            public const string Name = "Height 2";
            public const string DBName = "PlateHeight2";
            public const string Unit = sUnit_PlateLength;
            public const string Symbol = "h2";
        }

        public struct PlateLipS
        {
            public const string Name = "Lip";
            public const string DBName = "PlateHLip";
            public const string Unit = sUnit_PlateLength;
            public const string Symbol = "c lip";
        }

        // MEMBER PROPERTIES

        public struct RafterWidthS
        {
            public const string Name = "Rafter width";
            public const string DBName = "RafterWidth";
            public const string Unit = sUnit_PlateLength;
            public const string Symbol = "b rafter";
        }
    }
}
