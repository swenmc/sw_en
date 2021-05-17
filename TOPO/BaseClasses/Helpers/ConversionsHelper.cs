using DATABASE;
using DATABASE.DTO;
using MATERIAL;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BaseClasses.Helpers
{
    public static class ConversionsHelper
    {
        public static string[] ConvertArrayFloatToString(float[] array_float, int iDecimalPlaces = 3)
        {
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";
            if (array_float != null)
            {
                string[] array_string = new string[array_float.Length];
                for (int i = 0; i < array_string.Length; i++)
                    array_string[i] = (Math.Round(array_float[i], iDecimalPlaces)).ToString(nfi);

                return array_string;
            }
            return null;
        }

        public static double GetDoubleFromText(string strValue)
        {
            // only space, capital A-Z, lowercase a-z, and digits 0-9 are allowed in the string
            //string s  = Regex.Replace(strValue, "[^0-9,.]", "");
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";

            strValue = strValue.Replace(",", ".");
            int index = strValue.IndexOf(" ");
            if (index > 0) strValue = strValue.Substring(0, index);
            return double.Parse(strValue, nfi);
        }
        public static float ParseFloat(string strValue)
        {
            if (string.IsNullOrEmpty(strValue)) return 0f;

            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";

            strValue = strValue.Replace(",", ".");            
            return float.Parse(strValue, nfi);
        }

        
    }
}
