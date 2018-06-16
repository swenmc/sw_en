using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EC5
{
    class CL_63
    {
        double Eq_621____(double dlambda_y, double df_c0k, double dE_005)
        {
            return (dlambda_y / Math.PI) * Math.Sqrt(df_c0k / dE_005);  // Eq. (6.21) lambda_rel_y
        }
        double Eq_622____(double dlambda_z, double df_c0k, double dE_005)
        {
            return (dlambda_z / Math.PI) * Math.Sqrt(df_c0k / dE_005);  // Eq. (6.21) lambda_rel_z
        }
        double Eq_623_____(double dsigma_c0d, double dsigma_myd, double dsigma_mzd, double df_c0d, double df_myd, double df_mzd, double dk_cy, double dk_m)
        {
            if (df_c0d > 0 && df_myd > 0 && df_mzd > 0)
                return (Math.Abs(dsigma_c0d) / (dk_cy * df_c0d)) + (Math.Abs(dsigma_myd) / (df_myd)) + dk_m * (Math.Abs(dsigma_mzd) / (df_mzd)); // Eq. (6.23) ratio for sigma Nc, My, Mz
            else
                return 0;
        }
        double Eq_624_____(double dsigma_c0d, double dsigma_myd, double dsigma_mzd, double df_c0d, double df_myd, double df_mzd, double dk_cz, double dk_m)
        {
            if (df_c0d > 0 && df_myd > 0 && df_mzd > 0)
                return (Math.Abs(dsigma_c0d) / (dk_cz * df_c0d)) + dk_m * (Math.Abs(dsigma_myd) / (df_myd)) + (Math.Abs(dsigma_mzd) / (df_mzd)); // Eq. (6.24) ratio for sigma Nc, My, Mz
            else
                return 0;
        }
        double Eq_625_____(double dk_y, double dlambda_rel_y)
        {
            if ((Math.Pow(dk_y, 2) - Math.Pow(dlambda_rel_y, 2)) > 0)
                return 1 / (dk_y + Math.Sqrt(Math.Pow(dk_y, 2) - Math.Pow(dlambda_rel_y, 2))); // Eq. (6.25) k_cy
            else
                return 0;
        }
        double Eq_626_____(double dk_z, double dlambda_rel_z)
        {
            if ((Math.Pow(dk_z, 2) - Math.Pow(dlambda_rel_z, 2)) > 0)
                return 1 / (dk_z + Math.Sqrt(Math.Pow(dk_z, 2) - Math.Pow(dlambda_rel_z, 2))); // Eq. (6.26) k_cz
            else
                return 0;
        }
        double Eq_627_____(double dbeta_c, double dlambda_rel_y)
        {
            if (dlambda_rel_y > 0)
                return 0.5 * (dbeta_c * (dlambda_rel_y - 0.3) + Math.Pow(dlambda_rel_y, 2)); // Eq. (6.27) k_y
            else
                return 0;
        }
        double Eq_628_____(double dbeta_c, double dlambda_rel_z)
        {
            if (dlambda_rel_z > 0)
                return 0.5 * (dbeta_c * (dlambda_rel_z - 0.3) + Math.Pow(dlambda_rel_z, 2)); // Eq. (6.28) k_z
            else
                return 0;
        }

        // (6.29) beta c

        double Eq_630_____(double df_mk, double dsigma_mcrit)
        {
            if (dsigma_mcrit > 0)
                return Math.Sqrt(df_mk / dsigma_mcrit); // Eq. (6.30) lambda_relm
            else
                return 0;
        }
        double Eq_631_____(double dE_005, double dI_z, double dG_005, double dI_tor, double dl_ef, double dW_y)
        {
            if (dl_ef * dW_y > 0)
                return (Math.PI * Math.Sqrt(dE_005 * dI_z * dG_005 * dI_tor)) / (dl_ef * dW_y); // Eq. (6.31) sigma_mcrit
            else
                return 0;
        }
        double Eq_632_____(double db, double dh, double dl_ef, double dE_005)
        {
            if (dl_ef * dh > 0)
                return 0.78 * Math.Pow(db, 2) * dE_005 / (dl_ef * dh); // Eq. (6.32) sigma_mcrit
            else
                return 0;
        }
        double Eq_633_____(double dsigma_md, double dk_crit, double df_md)
        {
            if (dk_crit * df_md > 0)
                return Math.Abs(dsigma_md) / (dk_crit * df_md); // Eq. (6.33) ratio for sigma_md
            else
                return 0;
        }
        double Eq_634_____(double dlambda_relm)
        {
            if (dlambda_relm > 0)
            {
                if (dlambda_relm <= 0.75) // Eq. (6.34) k_crit min
                    return 1;
                else if (dlambda_relm <= 1.4)
                    return 1.56 - 0.75 * dlambda_relm;
                else
                    return 1 / Math.Pow(dlambda_relm, 2);
            }
            else
                return 0;
        }
        double Eq_635_____(double dsigma_md, double dk_crit, double df_md, double dsigma_cd, double dk_cz, double df_c0d)
        {
            if (dk_crit * df_md > 0 && dk_cz * df_c0d > 0)
                return Math.Pow(Math.Abs(dsigma_md) / (dk_crit * df_md), 2) + Math.Abs(dsigma_cd) / (dk_cz * df_c0d); // Eq. (6.35) ratio for sigma_md and sigma_cd
            else
                return 0;
        }
    }
}
