using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EC5
{
    public class CL_62  // Clause 6.2
    {
        double Eq_616_____(double dsigma_c_alpha_d, double df_c0d, double dk_c90, double df_c90d, double dalpha)
        {
            if (df_c0d > 0 && dk_c90 > 0 && df_c90d > 0)
                return Math.Abs(dsigma_c_alpha_d) / (df_c0d / (df_c0d / (dk_c90 * df_c90d) * Math.Pow(Math.Sin(dalpha), 2) + Math.Cos(Math.Pow(dalpha, 2)))); // Eq. (6.16) ratio for sigma Nc
            else
                return 0;
        }
        double Eq_617_____(double dsigma_t0d, double dsigma_myd, double dsigma_mzd, double df_t0d, double df_myd, double df_mzd, double dk_m)
        {
            if (df_t0d > 0 && df_myd > 0 && df_mzd > 0)
                return (Math.Abs(dsigma_t0d) / (df_t0d)) + (Math.Abs(dsigma_myd) / (df_myd)) + dk_m * (Math.Abs(dsigma_mzd) / (df_mzd)); // Eq. (6.17) ratio for sigma Nt, My, Mz
            else
                return 0;
        }
        double Eq_618_____(double dsigma_t0d, double dsigma_myd, double dsigma_mzd, double df_t0d, double df_myd, double df_mzd, double dk_m)
        {
            if (df_t0d > 0 && df_myd > 0 && df_mzd > 0)
                return (Math.Abs(dsigma_t0d) / (df_t0d)) + dk_m * (Math.Abs(dsigma_myd) / (df_myd)) + (Math.Abs(dsigma_mzd) / (df_mzd)); // Eq. (6.18) ratio for sigma Nt, My, Mz
            else
                return 0;
        }
        double Eq_619_____(double dsigma_c0d, double dsigma_myd, double dsigma_mzd, double df_c0d, double df_myd, double df_mzd, double dk_m)
        {
            if (df_c0d > 0 && df_myd > 0 && df_mzd > 0)
                return Math.Pow(Math.Abs(dsigma_c0d) / (df_c0d), 2) + (Math.Abs(dsigma_myd) / (df_myd)) + dk_m * (Math.Abs(dsigma_mzd) / (df_mzd)); // Eq. (6.19) ratio for sigma Nc, My, Mz
            else
                return 0;
        }
        double Eq_620_____(double dsigma_c0d, double dsigma_myd, double dsigma_mzd, double df_c0d, double df_myd, double df_mzd, double dk_m)
        {
            if (df_c0d > 0 && df_myd > 0 && df_mzd > 0)
                return Math.Pow(Math.Abs(dsigma_c0d) / (df_c0d), 2) + dk_m * (Math.Abs(dsigma_myd) / (df_myd)) + (Math.Abs(dsigma_mzd) / (df_mzd)); // Eq. (6.20) ratio for sigma Nc, My, Mz
            else
                return 0;
        }
    }
}
