using DATABASE;
using DATABASE.DTO;
using MATERIAL;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
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
    }
}
