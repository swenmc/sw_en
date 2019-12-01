using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DATABASE.DTO
{
    public class CrScProperties
    {
        public int DatabaseID;
        public string sectionName_short;
        public string sectionName_long;
        public string colorName;
        public double h;
        public double b;
        public double t_min;
        public double t_max;
        public double A_g;
        public double I_y0;
        public double I_z0;
        public double I_y;
        public double I_z;
        public double W_y_el;
        public double W_z_el;
        public double I_t;
        public double I_w;
        public double D_y_gc; // Poloha taziska v povodnom suradnicovom systeme
        public double D_z_gc;
        public double D_y_sc;// Poloha stredu smyku v povodnom suradnicovom systeme
        public double D_z_sc;
        public double D_y_s;// Vzdialenost medzi taziskom G a stredom smyku S
        public double D_z_s;
        public double Beta_y;
        public double Beta_z;
        public double Alpha_rad;
        public double fol_b;
        public double fod_b;
        public double fol_c;
        public double fod_c;
        public double A_stiff;
        public int n_stiff;
        public double y_stiff;
        public double b_1_flat_portion;
        public double b_tot;
        public double b_tot_length;
        public double A_f1;
        public double A_vy;
        public double fvy_red_factor;
        public double d_1_flat_portion;
        public double d_tot;
        public double d_tot_length;
        public double A_w1;
        public double A_vz;
        public double fvz_red_factor;
        public bool IsBuiltUp;
        public int iScrewsNumber;
        public int iScrewsGauge;
        public double dScrewDistance;
        public string ribsProjectionSpacing_mm;
        public double dPrice_PPLM_NZD;

        public CrScProperties(){}
    }
}
