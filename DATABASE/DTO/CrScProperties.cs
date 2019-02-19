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
        public string SectionNameDatabase;
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

        public CrScProperties(){}

    }
}
