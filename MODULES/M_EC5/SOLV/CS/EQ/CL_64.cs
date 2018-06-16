using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EC5
{
    public enum eBeamShape
    {
        eBS_regular,         // regular beam
        eBS_double_tapered,  // double tapered beam
        eBS_curved,          // curved beam
        eBS_pitched,         // pitched cambered beam
    };

    class CL_64
    {
        double Eq_636___(double dN, double dA)
        {
            return dN / dA;  // Eq. (6.36) sigma_N
        }
        double Eq_637___(double dM_d, double db, double dh)
        {
            return 6 * dM_d / (db * Math.Pow(dh, 2));  // Eq. (6.37) sigma_malphad and sigma_m0d
        }
        double Eq_638___(double dsigma_malphad, double dk_malpha, double df_md)
        {
            if (dk_malpha * df_md > 0)
                return Math.Abs(dsigma_malphad) / (dk_malpha * df_md);  // Eq. (6.38) ratio for sigma_malphad
            else
                return 0;
        }
        double Eq_639___(double df_md, double df_vd, double df_t90d, double dalpha)
        {
            return 1 / Math.Sqrt(1 + Math.Pow((df_md / (1.5 * df_vd)) * Math.Tan(dalpha), 2) + Math.Pow((df_md / df_t90d) * Math.Pow(Math.Tan(dalpha), 2), 2));  // Eq. (6.39) k_malpha
        }
        double Eq_640___(double df_md, double df_vd, double df_c90d, double dalpha)
        {
            return 1 / Math.Sqrt(1 + Math.Pow((df_md / (1.5 * df_vd)) * Math.Tan(dalpha), 2) + Math.Pow((df_md / df_c90d) * Math.Pow(Math.Tan(dalpha), 2), 2));  // Eq. (6.40) k_malpha
        }
        double Eq_41____(double dsigma_md, double dk_tau, double df_md)
        {
            if (dk_tau * df_md > 0)
                return Math.Abs(dsigma_md) / (dk_tau * df_md);  // Eq. (6.41) ratio for sigma_md
            else
                return 0;
        }
        double Eq_42____(double dk_l, double dM_apd, double db, double dh_ap)
        {
            return dk_l * 6 * dM_apd / (db * Math.Pow(dh_ap, 2));  // Eq. (6.42) sigma_md

        }
        double Eq_43____(double dk_1, double dk_2, double dk_3, double dk_4, double dh_ap, double dr)
        {
            if (dr > 0)
                return dk_1 + dk_2 * (dh_ap / dr) + dk_3 * Math.Pow(dh_ap / dr, 2) + dk_4 * (dh_ap / dr) * Math.Pow(dh_ap / dr, 2);  // Eq. (6.43) k_l
            else
                return 0;
        }
        double Eq_44____(double dalpha_ap)
        {
            return 1 + 1.4 * Math.Tan(dalpha_ap) + 5.4 * Math.Pow(Math.Tan(dalpha_ap), 2);  // Eq. (6.44) k_1
        }
        double Eq_45____(double dalpha_ap)
        {
            return 0.35 - 8 * Math.Tan(dalpha_ap);  // Eq. (6.45) k_2
        }
        double Eq_46____(double dalpha_ap)
        {
            return 0.6 + 8.3 * Math.Tan(dalpha_ap) - 7.8 * Math.Pow(Math.Tan(dalpha_ap), 2);  // Eq. (6.46) k_3
        }
        double Eq_47____(double dalpha_ap)
        {
            return 6 * Math.Tan(dalpha_ap);  // Eq. (6.47) k_4
        }
        double Eq_48____(double dr_in, double dh_ap)
        {
            return dr_in + 0.5 * dh_ap;  // Eq. (6.48) r
        }
        double Eq_49____(double dr_in, double dt)
        {
            if (dr_in / dt < 240)
                return 0.76 + 0.001 * (dr_in / dt);  // Eq. (6.49) kr
            else
                return 1;
        }
        double Eq_50____(double dsigma_t90d, double dk_dis, double dk_vol, double df_t90d)
        {
            if (dk_dis * dk_vol * df_t90d > 0)
                return Math.Abs(dsigma_t90d) / (dk_dis * dk_vol * df_t90d);  // Eq. (6.50) rartio for sigma_t90d
            else
                return 0;
        }
        double Eq_51____(bool bsolidtimber, double dV_0, double dV)
        {
            if (bsolidtimber)
                return 1;
            else
                return Math.Pow(dV_0 / dV, 0.2);  // Eq. (6.51) k_vol
        }
        double Eq_52____(eBeamShape eBS_shape)
        {
            switch (eBS_shape)  // Eq. (6.52) k_dis - depends on shape of beam
            {
                case eBeamShape.eBS_double_tapered:
                case eBeamShape.eBS_curved:
                    return 1.4;
                case eBeamShape.eBS_pitched:
                    return 1.7;
                default:
                    return 1;
            }
        }
        double Eq_53____(double dtau_d, double df_vd, double dsigma_t90d, double dk_dis, double dk_vol, double df_t90d)
        {
            if (dk_dis * dk_vol * df_t90d > 0)
                return ((Math.Abs(dtau_d) / df_vd) + (Math.Abs(dsigma_t90d) / (dk_dis * dk_vol * df_t90d))) / 1;  // Eq. (6.53) rartio for tau_d and sigma_t90d
            else
                return 0;
        }
        double Eq_54____(double dk_p, double dM_apd, double db, double dh_ap)
        {
            return dk_p * 6 * dM_apd / (db * Math.Pow(dh_ap, 2));  // Eq. (6.54) sigma_t90d
        }
        double Eq_55____(double dk_p, double dM_apd, double db, double dh_ap, double dp_d)
        {
            return (dk_p * 6 * dM_apd / (db * Math.Pow(dh_ap, 2))) - (0.6 * dp_d / db);  // Eq. (6.55) sigma_t90d
        }
        double Eq_56____(double dk_p, double dk_5, double dk_6, double dk_7, double dh_ap, double dr)
        {
            if (dr > 0)
                return dk_5 + dk_6 * (dh_ap / dr) + dk_7 * Math.Pow(dh_ap / dr, 2);  // Eq. (6.56) k_p
            else
                return 0;
        }
        double Eq_57____(double dalpha_ap)
        {
            return 0.2 * Math.Tan(dalpha_ap);  // Eq. (6.56) k_5
        }
        double Eq_58____(double dalpha_ap)
        {
            return 0.25 - 1.5 * Math.Tan(dalpha_ap) + 2.6 * Math.Pow(Math.Tan(dalpha_ap), 2);  // Eq. (6.58) k_6
        }
        double Eq_59____(double dalpha_ap)
        {
            return 2.1 * Math.Tan(dalpha_ap) - 4 * Math.Pow(Math.Tan(dalpha_ap), 2);  // Eq. (6.59) k_7
        }
    }
}
