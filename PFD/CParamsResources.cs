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
        public const string sUnit_CrscLength = "[mm]";
        public const string sUnit_PlateLength = "[mm]";
        public const string sUnit_CountIntNumber = "[-]";
        public const string sUnit_None = "";

        // AUXILIARY
        public const string sSymbol_None = "";
        public const string sDBName_None = "";

        // PLATE PROPERTIES

        public struct PlateNameS
        {
            public const string Name = "Name";
            public const string DBName = sDBName_None;
            public const string Unit = sUnit_None;
            public const string Symbol = sSymbol_None;
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

        // MEMBER / CROSS-SECTION PROPERTIES

        public struct RafterWidthS
        {
            public const string Name = "Rafter width";
            public const string DBName = "RafterWidth";
            public const string Unit = sUnit_PlateLength;
            public const string Symbol = "b rafter";
        }

        public struct CrscDepthS
        {
            public const string Name = "Cross-section depth";
            public const string DBName =  "CrscDepth";
            public const string Unit = sUnit_CrscLength;
            public const string Symbol = "h";
        }

        public struct CrscWebStraightDepthS
        {
            public const string Name = "Cross-section web straight depth";
            public const string DBName = "CrscWebStraightDepth";
            public const string Unit = sUnit_CrscLength;
            public const string Symbol = "hw";
        }

        public struct CrscWebMiddleStiffenerSizeS
        {
            public const string Name = "Middle stiffener size";
            public const string DBName = "CrscWebMiddleStiffenerSize";
            public const string Unit = sUnit_CrscLength;
            public const string Symbol = "bs";
        }

        // SCREW ARRANGEMENT PROPERTIES

        // CIRCLE
        public struct NumberOfCirclesInGroupS
        {
            public const string Name = "Number of circles in group";
            public const string DBName = "NumberOfCirclesInGroup";
            public const string Unit = sUnit_CountIntNumber;
            public const string Symbol = "No";
        }

        public struct NumberOfScrewsInCircleSequenceS
        {
            public const string Name = "Number of screws in circle sequence";
            public const string DBName = "HolesInCircleSequenceNumber";
            public const string Unit = sUnit_CountIntNumber;
            public const string Symbol = "No";
        }

        public struct RadiusOfScrewsInCircleSequenceS
        {
            public const string Name = "Radius";
            public const string DBName = "Radius";
            public const string Unit = sUnit_PlateLength;
            public const string Symbol = "r";
        }

        public struct UseAdditionalCornerScrewsS
        {
            public const string Name = "Use additional corner screws";
            public const string DBName = "UseAdditionalCornerScrews";
            public const string Unit = sUnit_None;
            public const string Symbol = sSymbol_None;
        }

        public struct NumberOfAdditionalScrewsS
        {
            public const string Name = "Number of additional screws";
            public const string DBName = "NumberOfAdditionalScrews";
            public const string Unit = sUnit_CountIntNumber;
            public const string Symbol = "No";
        }

        public struct NumberOfAdditionalScrewsInCornerS
        {
            public const string Name = "Number of additional screws in corner";
            public const string DBName = "NumberOfAdditionalScrewsInCorner";
            public const string Unit = sUnit_CountIntNumber;
            public const string Symbol = "No";
        }

        public struct NumberOfAdditionalScrewsInRowS
        {
            public const string Name = "Number of additional screws in row";
            public const string DBName = "NumberOfAdditionalScrewsInRow";
            public const string Unit = sUnit_CountIntNumber;
            public const string Symbol = "No";
        }

        public struct NumberOfAdditionalScrewsInColumnS
        {
            public const string Name = "Number of additional screws in column";
            public const string DBName = "NumberOfAdditionalScrewsInRow";
            public const string Unit = sUnit_CountIntNumber;
            public const string Symbol = "No";
        }

        public struct DistanceOfAdditionalScrewsInxS
        {
            public const string Name = "Distance of additional screws x";
            public const string DBName = "DistanceOfAdditionalScrewsInx";
            public const string Unit = sUnit_PlateLength;
            public const string Symbol = "bx";
        }

        public struct DistanceOfAdditionalScrewsInyS
        {
            public const string Name = "Distance of additional screws y";
            public const string DBName = "DistanceOfAdditionalScrewsIny";
            public const string Unit = sUnit_PlateLength;
            public const string Symbol = "by";
        }

        // RECTANGULAR


        // SCREW PROPERTIES
        public struct ScrewGaugeS
        {
            public const string Name = "Screw gauge";
            public const string DBName = "ScrewGauge";
            public const string Unit = sUnit_CountIntNumber;
            public const string Symbol = "g";
        }
    }
}
