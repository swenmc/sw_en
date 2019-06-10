using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
