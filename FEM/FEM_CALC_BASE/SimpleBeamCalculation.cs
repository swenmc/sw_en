using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaseClasses;
using CRSC;

namespace FEM_CALC_BASE
{
    public class SimpleBeamCalculation
    {
        public SimpleBeamCalculation() { }

        designMomentValuesForCb[] sMomentValuesforCb;
        basicInternalForces[,] sBIF_x;

        public void CalculateInternalForcesOnSimpleBeam(int iNumberOfLoadCombinations, int iNumberOfDesignSections, CCrSc_TW section, float fTheoreticalLengthOfMember, float[] fx_positions, float[] fE_d_load_values_LCS_y, float[] fE_d_load_values_LCS_z, out basicInternalForces[,] sBIF_x, out designMomentValuesForCb[] sMomentValuesforCb)
        {
            sMomentValuesforCb = new designMomentValuesForCb[iNumberOfLoadCombinations];
            sBIF_x = new basicInternalForces[iNumberOfLoadCombinations, iNumberOfDesignSections];

            // Temporary calculation of internal forces - each combination
            for (int i = 0; i < iNumberOfLoadCombinations; i++)
            {
                CExample_2D_51_SB memberModel_qy = new CExample_2D_51_SB(section, fTheoreticalLengthOfMember, EMLoadDirPCC1.eMLD_PCC_FYU_MZV, fE_d_load_values_LCS_y[i]);
                CExample_2D_51_SB memberModel_qz = new CExample_2D_51_SB(section, fTheoreticalLengthOfMember, EMLoadDirPCC1.eMLD_PCC_FZV_MYU, fE_d_load_values_LCS_z[i]);

                float fM_abs_max = 0;

                for (int j = 0; j < iNumberOfDesignSections; j++)
                {
                    sBIF_x[i, j].fV_yu = memberModel_qy.GetV_x(fx_positions[j]);
                    sBIF_x[i, j].fM_zv = memberModel_qy.GetM_x(fx_positions[j]);

                    sBIF_x[i, j].fV_zv = memberModel_qz.GetV_x(fx_positions[j]);
                    sBIF_x[i, j].fM_yu = memberModel_qz.GetM_x(fx_positions[j]);

                    sBIF_x[i, j].fN = 0f; // TODO - doplnit vypocet
                    sBIF_x[i, j].fT = 0f; // TODO - doplnit vypocet

                    if (Math.Abs(sBIF_x[i, j].fM_yu) > Math.Abs(fM_abs_max))
                        fM_abs_max = sBIF_x[i, j].fM_yu;
                }

                sMomentValuesforCb[i].fM_max = fM_abs_max;
                sMomentValuesforCb[i].fM_14 = memberModel_qz.GetM_x(0.25f * fTheoreticalLengthOfMember);
                sMomentValuesforCb[i].fM_24 = memberModel_qz.GetM_x(0.50f * fTheoreticalLengthOfMember);
                sMomentValuesforCb[i].fM_34 = memberModel_qz.GetM_x(0.75f * fTheoreticalLengthOfMember);
            }
        }

        public void CalculateInternalForcesOnSimpleBeam(int iNumberOfDesignSections, CCrSc_TW section, float fTheoreticalLengthOfMember, float[] fx_positions, 
            float fE_d_load_value_LCS_y, float fE_d_load_value_LCS_z, out basicInternalForces[,] sBIF_x, out designMomentValuesForCb[] sMomentValuesforCb)
        {
            int iNumberOfLoadCombinations = 1;
            float[] fE_d_load_values_LCS_y = new float[1] { fE_d_load_value_LCS_y };
            float[] fE_d_load_values_LCS_z = new float[1] { fE_d_load_value_LCS_z };

            CalculateInternalForcesOnSimpleBeam(iNumberOfLoadCombinations, iNumberOfDesignSections, section, fTheoreticalLengthOfMember, fx_positions, fE_d_load_values_LCS_y, fE_d_load_values_LCS_z, out sBIF_x, out sMomentValuesforCb);
        }

        public void CalculateInternalForcesOnSimpleBeam(int iNumberOfDesignSections, float[] fx_positions, CMember member,
          CMLoad_21 memberload, out basicInternalForces[,] sBIF_x, out designMomentValuesForCb[] sMomentValuesforCb)
        {
            int iNumberOfLoadCombinations = 1;
            float[] fE_d_load_values_LCS_y = new float[1] { memberload.EDirPPC == EMLoadDirPCC1.eMLD_PCC_FYU_MZV ? memberload.Fq : 0};
            float[] fE_d_load_values_LCS_z = new float[1] { memberload.EDirPPC == EMLoadDirPCC1.eMLD_PCC_FZV_MYU ? memberload.Fq : 0};

            CalculateInternalForcesOnSimpleBeam(iNumberOfLoadCombinations, iNumberOfDesignSections, (CCrSc_TW)member.CrScStart, member.FLength, fx_positions, fE_d_load_values_LCS_y, fE_d_load_values_LCS_z, out sBIF_x, out sMomentValuesforCb);
        }

    }
}
