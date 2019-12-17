using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DATABASE.DTO
{
    public class CFibreglassProperties
    {
       public int DatabaseID;
       public string name;
       public double widthCoil_m;
       public double widthTot_m;
       public double widthModular_m;
       public double widthRib_m;
       public double widthUpRib_m;
       public double height_m;
       public string thickness_for_name;
       public double thickness_m;
       public double flatsheet_mass_g_m2;
       public double flatsheet_mass_kg_m2;
       public double mass_kg_m2;
       public double mass_kg_lm;
       public double price_PPSM_NZD;
       public double price_PPLM_NZ;
       public double price_PPKG_NZD;

       public CFibreglassProperties() { }
    }
}
