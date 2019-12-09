using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DATABASE.DTO
{
    public class CTS_CrscProperties
    {
       public int DatabaseID;
       public string name;
       public int material_ID;
       public string material_Name;
       public double widthTot_m;
       public double widthModular_m;
       public double widthRib_m;
       public double widthUpRib_m;
       public double height_m;
       public string thickness_for_name;
       public double thickness_m;
       public double mass_kg_m2;
       public double mass_kg_lm;
       public double price1_PPSM_NZD;
       public double price1_PPLM_NZ;
       public double price1_PPKG_NZD;
       public double price2_PPSM_NZD;
       public double price2_PPLM_NZ;
       public double price2_PPKG_NZD;
       public double price3_PPSM_NZD;
       public double price3_PPLM_NZ;
       public double price3_PPKG_NZD;
       public double maxSimpleSpan;
       public double maxEavesOverhang;
       public double A_g;
       public double Iy;
       public double Iz;
       public double W_el_y;
       public double W_el_y_t;
       public double W_el_y_b;
       public double W_el_z;
       public double It;
       public double Iw;
       public double phi_P_no;
       public double A_e;
       public double phi_M_yo_pos;
       public double I_e_y_pos;
       public double W_e_y_t_pos;
       public double W_e_y_b_pos;
       public double phi_M_yo_neg;
       public double I_e_y_neg;
       public double W_e_y_t_neg;
       public double W_e_y_b_neg;
       public double phi_V_nz;

       public CTS_CrscProperties() { }
    }
}
