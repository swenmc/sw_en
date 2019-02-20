using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseClasses
{
    public static class ExtensionMethods
    {
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




    }
}
