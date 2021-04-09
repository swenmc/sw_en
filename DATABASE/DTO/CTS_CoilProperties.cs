using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DATABASE.DTO
{
    [Serializable]
    public class CTS_CoilProperties
    {
        // TODO - implementovat properties

        public int ID;
        public double widthCoil;
        public int thicknessID;
        public double thicknessCore;
        public int coatingID;
        public string coatingName;
        public int[] colorRangeIDs;
        public double mass_kg_lm;
        public double coilmass_kg_m2;
        public double coilLengthPerTonne;
        public double price_PPTONNE_NZD;
        public double price_PPLM_NZD;
        public double price_PPSM_NZD;

        public CTS_CoilProperties() { }
    }
}