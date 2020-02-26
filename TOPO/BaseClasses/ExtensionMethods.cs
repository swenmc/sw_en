using BaseClasses.GraphObj;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace BaseClasses
{
    public static class ExtensionMethods
    {
        public static T Clone<T>(this T source)
        {
            var settings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Auto,
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented
            };

            var serialized = JsonConvert.SerializeObject(source, settings);
            return JsonConvert.DeserializeObject<T>(serialized, settings);
        }

        public static CPlate GetClonedPlate(this CPlate plate)
        {
            // Bug 396
            // To Ondrej - mozem tu takto dat vsetky triedy CPlate?
            // Ako to mam robit ked su tu v ramci nejakej triedy aj dalsi potomkovia

            CPlate clone = null;
            if (plate is CConCom_Plate_BB_BG)
            {
                CConCom_Plate_BB_BG p = plate as CConCom_Plate_BB_BG;
                clone = p.Clone();
            }
            else if (plate is CConCom_Plate_B_basic)
            {
                CConCom_Plate_B_basic p = plate as CConCom_Plate_B_basic;
                clone = p.Clone();
            }
            else if(plate is CConCom_Plate_F_or_L)
            {
                CConCom_Plate_F_or_L p = plate as CConCom_Plate_F_or_L;
                clone = p.Clone();
            }
            else if (plate is CConCom_Plate_G)
            {
                CConCom_Plate_G p = plate as CConCom_Plate_G;
                clone = p.Clone();
            }
            else if (plate is CConCom_Plate_H)
            {
                CConCom_Plate_H p = plate as CConCom_Plate_H;
                clone = p.Clone();
            }
            else if (plate is CConCom_Plate_JA)
            {
                CConCom_Plate_JA p = plate as CConCom_Plate_JA;
                clone = p.Clone();
            }
            else if (plate is CConCom_Plate_JB)
            {
                CConCom_Plate_JB p = plate as CConCom_Plate_JB;
                clone = p.Clone();
            }
            else if (plate is CConCom_Plate_JBS)
            {
                CConCom_Plate_JBS p = plate as CConCom_Plate_JBS;
                clone = p.Clone();
            }
            else if (plate is CConCom_Plate_JCS)
            {
                CConCom_Plate_JCS p = plate as CConCom_Plate_JCS;
                clone = p.Clone();
            }
            else if (plate is CConCom_Plate_KA)
            {
                CConCom_Plate_KA p = plate as CConCom_Plate_KA;
                clone = p.Clone();
            }
            else if (plate is CConCom_Plate_KB)
            {
                CConCom_Plate_KB p = plate as CConCom_Plate_KB;
                clone = p.Clone();
            }
            else if (plate is CConCom_Plate_KBS)
            {
                CConCom_Plate_KBS p = plate as CConCom_Plate_KBS;
                clone = p.Clone();
            }
            else if (plate is CConCom_Plate_KC)
            {
                CConCom_Plate_KC p = plate as CConCom_Plate_KC;
                clone = p.Clone();
            }
            else if (plate is CConCom_Plate_KCS)
            {
                CConCom_Plate_KCS p = plate as CConCom_Plate_KCS;
                clone = p.Clone();
            }
            else if (plate is CConCom_Plate_KD)
            {
                CConCom_Plate_KD p = plate as CConCom_Plate_KD;
                clone = p.Clone();
            }
            else if (plate is CConCom_Plate_KDS)
            {
                CConCom_Plate_KDS p = plate as CConCom_Plate_KDS;
                clone = p.Clone();
            }
            else if (plate is CConCom_Plate_KES)
            {
                CConCom_Plate_KES p = plate as CConCom_Plate_KES;
                clone = p.Clone();
            }
            else if (plate is CConCom_Plate_KFS)
            {
                CConCom_Plate_KFS p = plate as CConCom_Plate_KFS;
                clone = p.Clone();
            }
            else if (plate is CConCom_Plate_KK)
            {
                CConCom_Plate_KK p = plate as CConCom_Plate_KK;
                clone = p.Clone();
            }
            else if (plate is CConCom_Plate_LL)
            {
                CConCom_Plate_LL p = plate as CConCom_Plate_LL;
                clone = p.Clone();
            }
            else if (plate is CConCom_Plate_M)
            {
                CConCom_Plate_M p = plate as CConCom_Plate_M;
                clone = p.Clone();
            }
            else if (plate is CConCom_Plate_N)
            {
                CConCom_Plate_N p = plate as CConCom_Plate_N;
                clone = p.Clone();
            }
            else if (plate is CConCom_Plate_O)
            {
                CConCom_Plate_O p = plate as CConCom_Plate_O;
                clone = p.Clone();
            }
            else if (plate is CConCom_Plate_Q_T_Y)
            {
                CConCom_Plate_Q_T_Y p = plate as CConCom_Plate_Q_T_Y;
                clone = p.Clone();
            }
            else if (plate is CWasher_W)
            {
                CWasher_W p = plate as CWasher_W;
                clone = p.Clone();
            }

            return clone;
        }

        public static CConnectionJointTypes GetClonedJoint(this CConnectionJointTypes joint)
        {
            CConnectionJointTypes clone = null;

            //temp 23.1.2020 snazim sa ostranit OutOfMemoryException
            joint.Visual_ConnectionJoint = null;
            foreach (CPlate plate in joint.m_arrPlates) plate.Visual_Plate = null;
            //end temp


            if (joint is CConnectionJoint_A001)
            {
                CConnectionJoint_A001 j = joint as CConnectionJoint_A001;
                clone = j.Clone();
            }
            else if (joint is CConnectionJoint_B001)
            {
                CConnectionJoint_B001 j = joint as CConnectionJoint_B001;
                clone = j.Clone();
            }
            else if (joint is CConnectionJoint_C001)
            {
                CConnectionJoint_C001 j = joint as CConnectionJoint_C001;
                clone = j.Clone();
            }
            else if (joint is CConnectionJoint_CT01)
            {
                CConnectionJoint_CT01 j = joint as CConnectionJoint_CT01;
                clone = j.Clone();
            }
            else if (joint is CConnectionJoint_D001)
            {
                CConnectionJoint_D001 j = joint as CConnectionJoint_D001;
                clone = j.Clone();
            }
            else if (joint is CConnectionJoint_E001)
            {
                CConnectionJoint_E001 j = joint as CConnectionJoint_E001;
                clone = j.Clone();
            }
            else if (joint is CConnectionJoint_J001)
            {
                CConnectionJoint_J001 j = joint as CConnectionJoint_J001;
                clone = j.Clone();
            }
            else if (joint is CConnectionJoint_L001)
            {
                CConnectionJoint_L001 j = joint as CConnectionJoint_L001;
                clone = j.Clone();
            }
            else if (joint is CConnectionJoint_S001)
            {
                CConnectionJoint_S001 j = joint as CConnectionJoint_S001;
                clone = j.Clone();
            }
            else if (joint is CConnectionJoint_T001)
            {
                CConnectionJoint_T001 j = joint as CConnectionJoint_T001;
                clone = j.Clone();
            }
            else if (joint is CConnectionJoint_T002)
            {
                CConnectionJoint_T002 j = joint as CConnectionJoint_T002;
                clone = j.Clone();
            }
            else if (joint is CConnectionJoint_T003)
            {
                CConnectionJoint_T003 j = joint as CConnectionJoint_T003;
                clone = j.Clone();
            }
            else if (joint is CConnectionJoint_TA01)
            {
                CConnectionJoint_TA01 j = joint as CConnectionJoint_TA01;
                clone = j.Clone();
            }
            else if (joint is CConnectionJoint_TB01)
            {
                CConnectionJoint_TB01 j = joint as CConnectionJoint_TB01;
                clone = j.Clone();
            }
            else if (joint is CConnectionJoint_TC01)
            {
                CConnectionJoint_TC01 j = joint as CConnectionJoint_TC01;
                clone = j.Clone();
            }
            else if (joint is CConnectionJoint_TD01)
            {
                CConnectionJoint_TD01 j = joint as CConnectionJoint_TD01;
                clone = j.Clone();
            }

            return clone;
        }

        public static CFoundation GetClonedPad(this CFoundation pad)
        {
            if (pad == null) return null;

            CFoundation clone;

            CReinforcementBar Reference_Top_Bar_x = pad.Reference_Top_Bar_x.GetClonedReinforcementBar();
            CReinforcementBar Reference_Top_Bar_y = pad.Reference_Top_Bar_y.GetClonedReinforcementBar();
            CReinforcementBar Reference_Bottom_Bar_x = pad.Reference_Bottom_Bar_x.GetClonedReinforcementBar();
            CReinforcementBar Reference_Bottom_Bar_y = pad.Reference_Bottom_Bar_y.GetClonedReinforcementBar();

            List<CReinforcementBar> Top_Bars_x = new List<CReinforcementBar>();
            List<CReinforcementBar> Top_Bars_y = new List<CReinforcementBar>();
            List<CReinforcementBar> Bottom_Bars_x = new List<CReinforcementBar>();
            List<CReinforcementBar> Bottom_Bars_y = new List<CReinforcementBar>();

            if (pad.Top_Bars_x != null)
                foreach (CReinforcementBar bar in pad.Top_Bars_x)
                {
                    Top_Bars_x.Add(bar.GetClonedReinforcementBar());
                }

            if (pad.Top_Bars_y != null)
                foreach (CReinforcementBar bar in pad.Top_Bars_y)
                {
                    Top_Bars_y.Add(bar.GetClonedReinforcementBar());
                }

            if (pad.Bottom_Bars_x != null)
                foreach (CReinforcementBar bar in pad.Bottom_Bars_x)
                {
                    Bottom_Bars_x.Add(bar.GetClonedReinforcementBar());
                }

            if (pad.Bottom_Bars_y != null)
                foreach (CReinforcementBar bar in pad.Bottom_Bars_y)
                {
                    Bottom_Bars_y.Add(bar.GetClonedReinforcementBar());
                }
            //pad.Reference_Top_Bar_x = null;
            //pad.Reference_Top_Bar_y = null;
            //pad.Reference_Bottom_Bar_x = null;
            //pad.Reference_Bottom_Bar_y = null;

            //pad.Top_Bars_x = null;
            //pad.Top_Bars_y = null;
            //pad.Bottom_Bars_x = null;
            //pad.Bottom_Bars_y = null;

            clone = pad.Clone();

            clone.Reference_Top_Bar_x = Reference_Top_Bar_x;
            clone.Reference_Top_Bar_y = Reference_Top_Bar_y;
            clone.Reference_Bottom_Bar_x = Reference_Bottom_Bar_x;
            clone.Reference_Bottom_Bar_y = Reference_Bottom_Bar_y;

            clone.Top_Bars_x = Top_Bars_x;
            clone.Top_Bars_y = Top_Bars_y;
            clone.Bottom_Bars_x = Bottom_Bars_x;
            clone.Bottom_Bars_y = Bottom_Bars_y;

            return clone;
        }
        public static CReinforcementBar GetClonedReinforcementBar(this CReinforcementBar bar)
        {
            CReinforcementBar clone = null;
            if (bar is CReinforcementBarStraight)
            {
                CReinforcementBarStraight r = bar as CReinforcementBarStraight;
                clone = r.Clone();
                clone.BIsDisplayed = bar.BIsDisplayed;
            }
            else if (bar is CReinforcementBar_U)
            {
                CReinforcementBar_U r = bar as CReinforcementBar_U;                
                clone = r.Clone();
                clone.BIsDisplayed = bar.BIsDisplayed;
            }
            return clone;
        }

        public static CDimension GetClonedDimension(this CDimension dimension)
        {
            CDimension clone = null;
            if (dimension is CDimensionLinear)
            {
                CDimensionLinear d = dimension as CDimensionLinear;
                clone = d.Clone();
            }
            else if (dimension is CDimensionArc)
            {
                CDimensionArc d = dimension as CDimensionArc;
                clone = d.Clone();
            }
            return clone;
        }

        public static void Dispose(this Viewport3D viewPort)
        {
            if (viewPort == null) return;
            if (viewPort.Children != null) viewPort.Children.Clear();
            viewPort.Camera = null;
            viewPort = null;
        }

        
        public static string GetFriendlyName(this EMemberType_FS_Position pos)
        {
            switch (pos)
            {
                case EMemberType_FS_Position.EdgeColumn: return "Edge Column";
                case EMemberType_FS_Position.EdgeRafter: return "Edge Rafter";
                case EMemberType_FS_Position.MainColumn: return "Main Column";
                case EMemberType_FS_Position.MainRafter: return "Main Rafter";
                case EMemberType_FS_Position.EdgePurlin: return "Edge Purlin";
                case EMemberType_FS_Position.Purlin: return "Purlin";
                case EMemberType_FS_Position.Girt: return "Girt";
                case EMemberType_FS_Position.GirtFrontSide: return "Girt Front Side";
                case EMemberType_FS_Position.GirtBackSide: return "Girt Back Side";
                case EMemberType_FS_Position.WindPostFrontSide: return "Wind Post Front Side";
                case EMemberType_FS_Position.WindPostBackSide: return "Wind Post Back Side";
                case EMemberType_FS_Position.DoorFrame: return "Door Frame";
                case EMemberType_FS_Position.DoorLintel: return "Door Lintel";
                case EMemberType_FS_Position.DoorTrimmer: return "Door Trimmer";
                case EMemberType_FS_Position.WindowFrame: return "Window Frame";
            }
            return "Unknown EMemberType_FS_Position";
        }


        public static string GetFriendlyName(this ELCType lcType)
        {
            switch (lcType)
            {
                case ELCType.ePermanentLoad: return "Permanent load";
                case ELCType.eImposedLoad_ST: return "Imposed load - short-term";
                case ELCType.eImposedLoad_LT: return "Imposed load - long-term";
                case ELCType.eSnow: return "Snow";
                case ELCType.eWind: return "Wind";
                case ELCType.eEarthquake: return "Earthquake";

            }
            return "Unknown ELCType";
        }
        public static string GetFriendlyName(this ELCName lcName)
        {
            switch (lcName)
            {
                case ELCName.eDL_G: return "Dead load G";
                case ELCName.eIL_Q: return "Imposed load Q";
                case ELCName.eSL_Su_Full: return "Snow load Su - full";
                case ELCName.eSL_Su_Left: return "Snow load Su - left";
                case ELCName.eSL_Su_Right: return "Snow load Su - right";
                case ELCName.eWL_Wu_Cpi_min_Left_X_Plus: return "Wind load Wu - Cpi,min - Left - X+";
                case ELCName.eWL_Wu_Cpi_min_Right_X_Minus: return "Wind load Wu - Cpi,min - Right - X-";
                case ELCName.eWL_Wu_Cpi_min_Front_Y_Plus: return "Wind load Wu - Cpi,min - Front - Y+";
                case ELCName.eWL_Wu_Cpi_min_Rear_Y_Minus: return "Wind load Wu - Cpi,min - Rear - Y-";
                case ELCName.eWL_Wu_Cpi_max_Left_X_Plus: return "Wind load Wu - Cpi,max - Left - X+";
                case ELCName.eWL_Wu_Cpi_max_Right_X_Minus: return "Wind load Wu - Cpi,max - Right - X-";
                case ELCName.eWL_Wu_Cpi_max_Front_Y_Plus: return "Wind load Wu - Cpi,max - Front - Y+";
                case ELCName.eWL_Wu_Cpi_max_Rear_Y_Minus: return "Wind load Wu - Cpi,max - Rear - Y-";
                case ELCName.eWL_Wu_Cpe_min_Left_X_Plus: return "Wind load Wu - Cpe,min - Left - X+";
                case ELCName.eWL_Wu_Cpe_min_Right_X_Minus: return "Wind load Wu - Cpe,min - Right - X-";
                case ELCName.eWL_Wu_Cpe_min_Front_Y_Plus: return "Wind load Wu - Cpe,min - Front - Y+";
                case ELCName.eWL_Wu_Cpe_min_Rear_Y_Minus: return "Wind load Wu - Cpe,min - Rear - Y-";
                case ELCName.eWL_Wu_Cpe_max_Left_X_Plus: return "Wind load Wu - Cpe,max - Left - X+";
                case ELCName.eWL_Wu_Cpe_max_Right_X_Minus: return "Wind load Wu - Cpe,max - Right - X-";
                case ELCName.eWL_Wu_Cpe_max_Front_Y_Plus: return "Wind load Wu - Cpe,max - Front - Y+";
                case ELCName.eWL_Wu_Cpe_max_Rear_Y_Minus: return "Wind load Wu - Cpe,max - Rear - Y-";
                case ELCName.eEQ_Eu_Left_X_Plus: return "Earthquake load Eu - X";
                case ELCName.eEQ_Eu_Front_Y_Plus: return "Earthquake load Eu - Y";
                case ELCName.eSL_Ss_Full: return "Snow load Ss - full";
                case ELCName.eSL_Ss_Left: return "Snow load Ss - left";
                case ELCName.eSL_Ss_Right: return "Snow load Ss - right";
                case ELCName.eWL_Ws_Cpi_min_Left_X_Plus: return "Wind load Ws - Cpi,min - Left - X+";
                case ELCName.eWL_Ws_Cpi_min_Right_X_Minus: return "Wind load Ws - Cpi,min - Right - X-";
                case ELCName.eWL_Ws_Cpi_min_Front_Y_Plus: return "Wind load Ws - Cpi,min - Front - Y+";
                case ELCName.eWL_Ws_Cpi_min_Rear_Y_Minus: return "Wind load Ws - Cpi,min - Rear - Y-";
                case ELCName.eWL_Ws_Cpi_max_Left_X_Plus: return "Wind load Ws - Cpi,max - Left - X+";
                case ELCName.eWL_Ws_Cpi_max_Right_X_Minus: return "Wind load Ws - Cpi,max - Right - X-";
                case ELCName.eWL_Ws_Cpi_max_Front_Y_Plus: return "Wind load Ws - Cpi,max - Front - Y+";
                case ELCName.eWL_Ws_Cpi_max_Rear_Y_Minus: return "Wind load Ws - Cpi,max - Rear - Y-";
                case ELCName.eWL_Ws_Cpe_min_Left_X_Plus: return "Wind load Ws - Cpe,min - Left - X+";
                case ELCName.eWL_Ws_Cpe_min_Right_X_Minus: return "Wind load Ws - Cpe,min - Right - X-";
                case ELCName.eWL_Ws_Cpe_min_Front_Y_Plus: return "Wind load Ws - Cpe,min - Front - Y+";
                case ELCName.eWL_Ws_Cpe_min_Rear_Y_Minus: return "Wind load Ws - Cpe,min - Rear - Y-";
                case ELCName.eWL_Ws_Cpe_max_Left_X_Plus: return "Wind load Ws - Cpe,max - Left - X+";
                case ELCName.eWL_Ws_Cpe_max_Right_X_Minus: return "Wind load Ws - Cpe,max - Right - X-";
                case ELCName.eWL_Ws_Cpe_max_Front_Y_Plus: return "Wind load Ws - Cpe,max - Front - Y+";
                case ELCName.eWL_Ws_Cpe_max_Rear_Y_Minus: return "Wind load Ws - Cpe,max - Rear - Y-";
                case ELCName.eEQ_Es_Left_X_Plus: return "Earthquake load Es - X";
                case ELCName.eEQ_Es_Front_Y_Plus: return "Earthquake load Es - Y";
            }
            return "Unknown ELCName";
        }

        public static string GetFriendlyName(this ESnowElevationRegions snowElevationRegion)
        {
            switch (snowElevationRegion)
            {
                case ESnowElevationRegions.eAlpine: return "Alpine";
                case ESnowElevationRegions.eNoSignificantSnow: return "No Significant Snow";
                case ESnowElevationRegions.eSubAlpine: return "Sub Alpine";
            }
            return "Unknown Snow Elevation Region";
        }

        public static string GetFriendlyName(this EPDFPageContentType t)
        {
            switch (t)
            {
                case EPDFPageContentType.Isometric_View: return "Isometric View";
                case EPDFPageContentType.Front_Elevation: return "Front Elevation";
                case EPDFPageContentType.Back_Elevation: return "Back Elevation";
                case EPDFPageContentType.Left_Elevation: return "Left Elevation";
                case EPDFPageContentType.Right_Elevation: return "Right Elevation";
                case EPDFPageContentType.Roof_Layout: return "Roof Layout";
                case EPDFPageContentType.Middle_Frame: return "Middle Frame";
                case EPDFPageContentType.Columns: return "Columns";
                case EPDFPageContentType.Foundation_Pads: return "Foundation Pads";
                case EPDFPageContentType.Floor_Plan: return "Floor Plan";
                case EPDFPageContentType.Details_Standard_1: return "Details - Standard 1";
                case EPDFPageContentType.Details_Standard_2: return "Details - Standard 2";
                case EPDFPageContentType.Details_Joints: return "Details - Joints";
                case EPDFPageContentType.Details_Footing_Pads: return "Details - Footing Pads";
                case EPDFPageContentType.Details_Floor: return "Details - Floor";


            }
            return "Unknown EPDFPageContentType";
        }

        public static float GetLoadValue(this CMLoad load)
        {
            float fLoadValue = 0f;

            if (load is CMLoad_21)
            {
                CMLoad_21 l = (CMLoad_21)load;
                fLoadValue = l.Fq;
            }
            else if (load is CMLoad_22)
            {
                CMLoad_22 l = (CMLoad_22)load;
                fLoadValue = l.Fq;
            }
            else if (load is CMLoad_23)
            {
                CMLoad_23 l = (CMLoad_23)load;
                fLoadValue = l.Fq;
            }
            else if (load is CMLoad_24)
            {
                CMLoad_24 l = (CMLoad_24)load;
                fLoadValue = l.Fq;
            }
            else
            {
                // Not implemented
            }
            return fLoadValue;
        }

        public static float GetDistanceTo(this Point3D p, Point3D p2)
        {
            double Delta_X = p2.X - p.X;
            double Delta_Y = p2.Y - p.Y;
            double Delta_Z = p2.Z - p.Z;

            float distance = (float)Math.Sqrt((float)Math.Pow(Delta_X, 2f) + (float)Math.Pow(Delta_Y, 2f) + (float)Math.Pow(Delta_Z, 2f));
            return distance;
        }
    }
}
