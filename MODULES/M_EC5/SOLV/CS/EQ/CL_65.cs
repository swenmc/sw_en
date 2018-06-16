using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EC5
{
    public enum eMatPro
    {
        eMatPro_LVL,       // LVL                        // LAMINATED VENEER LUMBER (LVL) - vrstvene drevo                       
        eMatPro_GL,        // glued laminated timber     // GLUED LAMINATED TIMBER        - lepene lamelove drevo  
        eMatPro_S,         // solid timber               // SOLID TIMBER                  - rastle drevo
        eMatPro_O,         // other                      
    };

    class CL_65
    {
        double Eq_660a____(double dV, double db, double dh_ef)
        {
            if (db > 0 && dh_ef > 0)
                return dV / (db * dh_ef); // Eq. (6.60) tau_d
            else
                return 0;
        }
        double Eq_660b____(double dtau_d, double dk_v, double df_vd)
        {
            if (dk_v * df_vd > 0)
                return Math.Abs(dtau_d) / (dk_v * df_vd); // Eq. (6.60) ratio for tau_d
            else
                return 0;
        }
        int    Eq_661_____()
        {
            return 1; // Eq. (6.61) k_v
        }
        double Eq_662_____(double dk_n, double dh, double dalpha, double dx, double di)
        {
            if (dh > 0 && dalpha > 0)
                return Math.Min(1, (dk_n * (1 + ((1.1 * Math.Pow(di, 1.5)) / Math.Sqrt(dh)))) / (Math.Sqrt(dh) * (Math.Sqrt(dalpha * (1 - dalpha))) + (0.8 * (dx / dh) * Math.Sqrt(1 / dalpha * Math.Pow(dalpha, 2))))); // Eq. (6.62) k_v
            else
                return 0;
        }
        double Eq_663_____(eMatPro eMP_63)
        {
            switch (eMP_63)  // // Eq. (6.63) k_n - depends on material
            {
                case eMatPro.eMatPro_LVL:
                    return 4.5;
                case eMatPro.eMatPro_S:
                    return 5;
                case eMatPro.eMatPro_GL:
                    return 6.5;
                default:
                    return 5; // Temporary
            }
        }

        // Others
        double Eq_alpha___(double dh_ef, double dh)
        {
            if (dh > 0)
                return dh_ef / dh; // alpha for 6.5.2
            else
                return 0;
        }
        double Eq_ksys___(eMatPro eMP_63, int iLoadLamNumber) // ???? vrstvene vs. skrutkovane
        {
            double dlimit;

            switch (eMP_63)  // // Eq. (6.63) k_n - depends on material
            {
                case eMatPro.eMatPro_LVL: // nailed or screwed laminations
                    {
                        dlimit = 1.1;
                        break;
                    }
                case eMatPro.eMatPro_GL:  // laminations pre-stressed or glued together
                    {
                        dlimit = 1.2;
                        break;
                    }
                default:
                    {
                        dlimit = 1.1;     // Temporary
                        break;
                    }
            }

            if (iLoadLamNumber < 1)
                return 0;                                       // error in loaded laminations number (zero value)
            else if (1 <= iLoadLamNumber && iLoadLamNumber < 8)
                return dlimit / 8 * iLoadLamNumber;             // linear 
            else //(iLoadLamNumber >= 8)
                return dlimit;                                  // constant - maximum
        }
    }
}
