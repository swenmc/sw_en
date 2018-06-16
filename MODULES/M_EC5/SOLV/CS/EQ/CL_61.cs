using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EC5
{
    public enum eCS_Shape
    {
        eCS_circle,        // round/circular solid cross-section
        eCS_square,        // square solid cross-section
        eCS_rectan,        // rectangular solid cross-section
        eCS_other,         // other shapes of cross-section
    };

    public class CL_61
    {
        public double Eq_61______(double dsigma_t0d, double df_t0d)
        {
            if (df_t0d > 0)
                return Math.Abs(dsigma_t0d) / df_t0d; // Eq. (6.1) ratio for sigma_t0d
            else
                return 0;
        }
        public double Eq_62______(double dsigma_c0d, double df_c0d)
        {
            if (df_c0d > 0)
                return Math.Abs(dsigma_c0d) / df_c0d; // Eq. (6.2) ratio for sigma_c0d
            else
                return 0;
        }
        public double Eq_63______(double dsigma_c90d, double df_c90d, double dk_c90)
        {
            if (dk_c90 * df_c90d > 0)
                return Math.Abs(dsigma_c90d) / (dk_c90 * df_c90d); // Eq. (6.3) ratio for sigma_c90d
            else
                return 0;
        }
        public double Eq_64______(double dl, double dh)
        {
            if (dl > 0)
                return (2.38 - (dl / 250e-3))*(1 + (dh/(12* dl))); // Eq. (6.4) k_c90
            else
                return 0;
        }
        public double Eq_65______(double dl, double dh)
        {
            if (dl > 0)
                return (2.38 - (dl / 250e-3)) * (1 + (dh / (6 * dl))); // Eq. (6.5) k_c90
            else
                return 0;
        }
        public double Eq_66______(double dl, double dl_ef)
        {
            if (dl > 0)
                return (2.38 - (dl / 250e-3)) * Math.Pow(dl_ef / dl, 0.5); // Eq. (6.6) k_c90
            else
                return 0;
        }
        public double Eq_67______(double dl, double dh)
        {
                return dl + (dh / 3); // Eq. (6.7) l_ef
        }
        public double Eq_68______(double dl, double dh)
        {
            return dl + (2 * dh / 3); // Eq. (6.8) l_ef
        }
        public double Eq_69______(double dl, double dl_s, double dh)
        {
            if (dh < 0.04)
                dh = 0.04;
            return 0.5 * (dl + dl_s + (2 * dh / 3)); // Eq. (6.9) l_ef
        }
        public double Eq_610_____(double dl, double dl_ef)
        {
            if (dl > 0)
                return dl_ef / dl; // Eq. (6.10) k_c90
            else
                return 0;
        }
        public double Eq_611_____(double dsigma_myd, double dsigma_mzd, double df_myd, double df_mzd, double dk_m)
        {
            if (df_myd > 0 && df_mzd > 0)
                return (Math.Abs(dsigma_myd) / df_myd) + (dk_m * Math.Abs(dsigma_mzd) / df_mzd); // Eq. (6.11) ratio for sigma_myd and sigma_mzd
            else
                return 0;
        }
        public double Eq_612_____(double dsigma_myd, double dsigma_mzd, double df_myd, double df_mzd, double dk_m)
        {
            if (df_myd > 0 && df_mzd > 0)
                return (dk_m * Math.Abs(dsigma_myd) / df_myd) + (Math.Abs(dsigma_mzd) / df_mzd); // Eq. (6.12) ratio for sigma_myd and sigma_mzd
            else
                return 0;
        }
        public double Eq_613_____(double dtau_d, double df_vd)
        {
            if (df_vd > 0)
                return Math.Abs(dtau_d) / df_vd; // Eq. (6.13) ratio for tau_d
            else
                return 0;
        }
        public double Eq_614_____(double dtau_tord, double df_vd, double dk_shape)
        {
            if (dk_shape > 0 && df_vd > 0)
                return Math.Abs(dtau_tord) / (dk_shape * df_vd); // Eq. (6.14) ratio for tau_tord
            else
                return 0;
        }
        public double Eq_615_____(eCS_Shape eCSshape, double dh, double db)
        {
            switch (eCSshape)  // Eq. (6.15) k_shape - depends on shape of cross-section
            {
                case eCS_Shape.eCS_circle:
                    return 1.2;
                case eCS_Shape.eCS_square:
                    return 1.15;
                case eCS_Shape.eCS_rectan:
                    {
                        if (db > 0)
                            return Math.Min(1 + (0.15 * dh / db), 2);
                        else
                            return 0;
                    }
                default:
                    return 1;
            }
        }
    }
}
