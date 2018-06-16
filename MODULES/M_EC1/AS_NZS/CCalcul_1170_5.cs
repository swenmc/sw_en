using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace M_EC1.AS_NZS
{
    public class CCalcul_1170_5
    {
        int iSoilClass; // Site subsoil class


        int iBuildingImportanceLevel = 2;
        float fAnnualProbability_ULS = 1f / 500f;
        float fAnnualProbability_SLS = 1f / 25f;

        float fHazardFactor = 0.13f;

        float fReturnPeriodFactor_R_ULS = 1.00f; // ULS
        float fReturnPeriodFactor_R_SLS = 0.25f; // SLS
        float fNear_fault_N = 1.0f; // Near-fault factor

        // Elastic site spectra

        float fT_period = 0.4f;
        float fC_h = 3f; // Spectral shape factor

        float fC_ULS = 0.39f;
        float fC_SLS = 0.1f; // Horizontal elastic site spectra coefficient

        // Peak ground acceleration
        float fT_period_PGA = 0f;
        float fC_h_PGA = 1.12f; // Spectral shape factor

        float fC_PGA_ULS = 0.39f;
        float fC_PGA_SLS = 0.1f; // Peak ground coefficient

        float fC_v_ULS;
        float fC_v_SLS;

        float fNu_ULS = 1.25f; // Structural ductility factor
        float fNu_SLS = 1.00f;

        float fkNu_ULS = 1.14f; // Ductility coefficient
        float fkNu_SLS = 1.00f;

        float fS_p_ULS_stab = 1.00f; // Structural performance factor
        float fS_p_ULS_strength = 0.90f;
        float fS_p_SLS = 0.70f;

        float fC_T1 = 0; // ??????????????
        float fCv_T1 = 0; // ?????????????

        float fC_d_T1_ULS_stab;
        float fC_d_T1_ULS_strength;
        float fC_d_T1_SLS;

        float fC_v_Tv_ULS_stab;
        float fC_v_Tv_ULS_strength;
        float fC_v_Tv_SLS;

        public CCalcul_1170_5()
        {
            // Seismic Weight
            float fg_roof = 200f; // kN / m^2
            float fg_walls = 200f; // kN / m^2

            float fq_roof = 250f; // kN / m^2

            // One Portal Frame
            float fW = 60; // Width
            float fL1_PF_spacing = 5.765f; // L1
            float fH1_columns = 6f;

            float fG_roof = fg_roof * fL1_PF_spacing * fW;
            float fG_walls = fg_roof * fH1_columns * fL1_PF_spacing;

            float fG_tot = fG_roof + fG_walls;

            float fCdCT_ULS = 0.536f;
            float fCdCT_SLS = 0.116f;

            float fG_tot_ULS = fCdCT_ULS * fG_tot;
            float fG_tot_SLS = fCdCT_SLS * fG_tot;

            fC_v_ULS = 0.7f * fC_PGA_ULS;
            fC_v_ULS = 0.7f * fC_PGA_SLS;

            fC_d_T1_ULS_stab = fC_T1 * fS_p_ULS_stab / fkNu_ULS;
            fC_d_T1_ULS_strength = fC_T1 * fS_p_ULS_strength / fkNu_ULS;
            fC_d_T1_SLS = fC_T1 * fS_p_SLS / fkNu_SLS;

            fC_v_Tv_ULS_stab = fCv_T1 * fS_p_ULS_stab / fkNu_ULS;
            fC_v_Tv_ULS_strength = fCv_T1 * fS_p_ULS_strength / fkNu_ULS;
            fC_v_Tv_SLS = fCv_T1 * fS_p_SLS / fkNu_SLS;
        }




    }
}
