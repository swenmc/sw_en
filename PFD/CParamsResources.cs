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
        public const string sUnit_ConnectorLength = "[mm]";

        public const string sUnit_PlateSurface = "[mm²]";
        public const string sUnit_PlateVolume = "[mm³]";
        public const string sUnit_PlateMass = "[kg]";

        public const string sUnit_Rotation = "[deg]";

        public const string sUnit_CountIntNumber = "[-]";
        public const string sUnit_None = "";

        // AUXILIARY
        public const string sSymbol_None = "";
        public const string sDBName_None = "";

        // BUILDING PROPERTIES

        public struct RoofSlopeS
        {
            public const string Name = "Slope";
            public const string DBName = "RoofSlope";
            public const string Unit = sUnit_Rotation;
            public const string Symbol = "α";
        }

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

        public struct PlateWidth3S
        {
            public const string Name = "Width 3";
            public const string DBName = "PlateWidth3";
            public const string Unit = sUnit_PlateLength;
            public const string Symbol = "b3";
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
            public const string DBName = "PlateLip";
            public const string Unit = sUnit_PlateLength;
            public const string Symbol = "c lip";
        }

        public struct PlateLip1S
        {
            public const string Name = "Lip 1";
            public const string DBName = "PlateLip1";
            public const string Unit = sUnit_PlateLength;
            public const string Symbol = "c lip.1";
        }

        public struct PlateLip2S
        {
            public const string Name = "Lip 2";
            public const string DBName = "PlateLip2";
            public const string Unit = sUnit_PlateLength;
            public const string Symbol = "c lip.2";
        }

        public struct PlateAngleS
        {
            public const string Name = "Angle";
            public const string DBName = "PlateAngle";
            public const string Unit = sUnit_Rotation;
            public const string Symbol = "α.1";
        }

        public struct PlateAngle2S
        {
            public const string Name = "Angle 2";
            public const string DBName = "PlateAngle2";
            public const string Unit = sUnit_Rotation;
            public const string Symbol = "α.2";
        }

        public struct PlatePerimeterS
        {
            public const string Name = "Perimeter - Cutting route distance";
            public const string DBName = "PlatePerimeterCuttingRouteDistance";
            public const string Unit = sUnit_PlateLength;
            public const string Symbol = "Lcr";
        }

        public struct PlateSurfaceS
        {
            public const string Name = "Surface";
            public const string DBName = "PlateSurface";
            public const string Unit = sUnit_PlateSurface;
            public const string Symbol = "S";
        }

        public struct PlateAreaS
        {
            public const string Name = "Area";
            public const string DBName = "PlateArea";
            public const string Unit = sUnit_PlateSurface;
            public const string Symbol = "A";
        }

        public struct PlateVolumeS
        {
            public const string Name = "Volume";
            public const string DBName = "PlateVolume";
            public const string Unit = sUnit_PlateVolume;
            public const string Symbol = "V";
        }

        public struct PlateMassS
        {
            public const string Name = "Mass";
            public const string DBName = "PlateMass";
            public const string Unit = sUnit_PlateMass;
            public const string Symbol = "m";
        }

        public struct PlateDrillingRouteDistanceS
        {
            public const string Name = "Drilling route distance";
            public const string DBName = "DrillingRouteDistance";
            public const string Unit = sUnit_PlateLength;
            public const string Symbol = "Ldr";
        }

        // WASHER

        public struct WasherNameS
        {
            public const string Name = "Name";
            public const string DBName = sDBName_None;
            public const string Unit = sUnit_None;
            public const string Symbol = sSymbol_None;
        }

        public struct WasherThicknessS
        {
            public const string Name = "Thickness";
            public const string DBName = "WasherThick";
            public const string Unit = sUnit_PlateLength;
            public const string Symbol = "t";
        }

        public struct WasherWidthxS
        {
            public const string Name = "Width x";
            public const string DBName = "WasherWidthx";
            public const string Unit = sUnit_PlateLength;
            public const string Symbol = "bx";
        }

        public struct WasherWidthyS
        {
            public const string Name = "Width y";
            public const string DBName = "WasherWidthy";
            public const string Unit = sUnit_PlateLength;
            public const string Symbol = "by";
        }

        public struct PlateWasherNameS
        {
            public const string Name = "Plate Washer Name";
            public const string DBName = sDBName_None;
            public const string Unit = sUnit_None;
            public const string Symbol = sSymbol_None;
        }

        public struct PlateWasherThicknessS
        {
            public const string Name = "Plate Washer Thickness";
            public const string DBName = "PlateWasherThick";
            public const string Unit = sUnit_PlateLength;
            public const string Symbol = "t";
        }

        public struct PlateWasherWidthxS
        {
            public const string Name = "Plate Washer Width x";
            public const string DBName = "PlateWasherWidthx";
            public const string Unit = sUnit_PlateLength;
            public const string Symbol = "bx";
        }

        public struct PlateWasherWidthyS
        {
            public const string Name = "Plate Washer Width y";
            public const string DBName = "PlateWasherWidthy";
            public const string Unit = sUnit_PlateLength;
            public const string Symbol = "by";
        }

        public struct BearingWasherNameS
        {
            public const string Name = "Bearing Washer Name";
            public const string DBName = sDBName_None;
            public const string Unit = sUnit_None;
            public const string Symbol = sSymbol_None;
        }

        public struct BearingWasherThicknessS
        {
            public const string Name = "Bearing Washer Thickness";
            public const string DBName = "BearingWasherThick";
            public const string Unit = sUnit_PlateLength;
            public const string Symbol = "t";
        }

        public struct BearingWasherWidthxS
        {
            public const string Name = "Bearing Washer Width x";
            public const string DBName = "BearingWasherWidthx";
            public const string Unit = sUnit_PlateLength;
            public const string Symbol = "bx";
        }

        public struct BearingWasherWidthyS
        {
            public const string Name = "Bearing Washer Width y";
            public const string DBName = "BearingWasherWidthy";
            public const string Unit = sUnit_PlateLength;
            public const string Symbol = "by";
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
        public struct UseExtraScrewsS
        {
            public const string Name = "Use extra screws";
            public const string DBName = "UseExtraScrews";
            public const string Unit = sUnit_None;
            public const string Symbol = sSymbol_None;
        }
        public struct ExtraScrewsRowsS
        {
            public const string Name = "Number of extra screws rows";
            public const string DBName = "";
            public const string Unit = sUnit_None;
            public const string Symbol = sSymbol_None;
        }
        public struct ExtraScrewsInRowS
        {
            public const string Name = "Number of extra screws in row";
            public const string DBName = "";
            public const string Unit = sUnit_None;
            public const string Symbol = sSymbol_None;
        }

        public struct PositionOfCornerSequence_xS
        {
            public const string Name = "Position of additional screws x";
            public const string DBName = "PositionOfAdditionalScrewsX";
            public const string Unit = sUnit_PlateLength;
            public const string Symbol = "ax";
        }

        public struct PositionOfCornerSequence_yS
        {
            public const string Name = "Position of additional screws y";
            public const string DBName = "PositionOfAdditionalScrewsY";
            public const string Unit = sUnit_PlateLength;
            public const string Symbol = "ay";
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

        public struct DistanceOfAdditionalCornerScrewsInxS
        {
            public const string Name = "Distance of additional screws x";
            public const string DBName = "DistanceOfAdditionalScrewsInx";
            public const string Unit = sUnit_PlateLength;
            public const string Symbol = "bx";
        }

        public struct DistanceOfAdditionalCornerScrewsInyS
        {
            public const string Name = "Distance of additional screws y";
            public const string DBName = "DistanceOfAdditionalScrewsIny";
            public const string Unit = sUnit_PlateLength;
            public const string Symbol = "by";
        }


        public struct PositionOfExtraScrewsSequence_yS
        {
            public const string Name = "Position of extra screws y";
            public const string DBName = "PositionOfExtraScrewsY";
            public const string Unit = sUnit_PlateLength;
            public const string Symbol = "ay";
        }

        public struct DistanceOfExtraScrewsInxS
        {
            public const string Name = "Distance of extra screws x";
            public const string DBName = "DistanceOfExtraScrewsInx";
            public const string Unit = sUnit_PlateLength;
            public const string Symbol = "bx";
        }

        public struct DistanceOfExtraScrewsInyS
        {
            public const string Name = "Distance of extra screws y";
            public const string DBName = "DistanceOfExtraScrewsIny";
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

        // ANCHOR PROPERTIES
        public struct AnchorNameS
        {
            public const string Name = "Anchor name";
            public const string DBName = "AnchorName";
            public const string Unit = sUnit_None;
            public const string Symbol = "Name";
        }

        public struct AnchorDiameterS
        {
            public const string Name = "Anchor diameter";
            public const string DBName = "AnchorDiameter";
            public const string Unit = sUnit_ConnectorLength;
            public const string Symbol = "d";
        }

        public struct AnchorLengthS
        {
            public const string Name = "Anchor length";
            public const string DBName = "AnchorLength";
            public const string Unit = sUnit_ConnectorLength;
            public const string Symbol = "l";
        }

        public struct AnchorEffectiveDepthS
        {
            public const string Name = "Anchor effective depth";
            public const string DBName = "AnchorEffectiveDepth";
            public const string Unit = sUnit_ConnectorLength;
            public const string Symbol = "h eff";
        }
    }
}
