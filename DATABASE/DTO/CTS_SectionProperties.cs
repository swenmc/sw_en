using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DATABASE.DTO
{
    public class CTS_SectionProperties
    {
       public int DatabaseID;
       public string name;
       public string material_ID;
       public string material_Name;
       public string widthTot_m;
       public string widthModular_m;
       public string widthRib_m;
       public string widthUpRib_m;
       public string height_m;
       public string thickness_for_name;
       public string thickness_m;
       //public string mass_kg_m2;
       //public string mass_kg_lm;
       //public string price_PPSM_NZD;
       //public string price_PPLM_NZD;
       //public string price_PPKG_NZD;
       public string maxSimpleSpan;
       public string maxEavesOverhang;
       public string A_g;
       public string Iy;
       public string Iz;
       public string W_el_y;
       public string W_el_y_t;
       public string W_el_y_b;
       public string W_el_z;
       public string It;
       public string Iw;
       public string phi_P_no;
       public string A_e;
       public string phi_M_yo_pos;
       public string I_e_y_pos;
       public string W_e_y_t_pos;
       public string W_e_y_b_pos;
       public string phi_M_yo_neg;
       public string I_e_y_neg;
       public string W_e_y_t_neg;
       public string W_e_y_b_neg;
       public string phi_V_nz;

       public CTS_SectionProperties() { }
    }
}
