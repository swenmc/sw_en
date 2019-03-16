using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MATH;
using BaseClasses;

namespace FEM_CALC_BASE
{
    public class SimpleBeamCalculation
    {
        public SimpleBeamCalculation() { }

        // SBD funkcie (zahrnaju kombinacie)
        public void CalculateInternalForcesOnSimpleBeam_SBD(int iNumberOfLoadCombinations, int iNumberOfDesignSections, CMember member, float[] fx_positions,
            float[] fE_d_load_values_LCS_y, float[] fE_d_load_values_LCS_z, out basicInternalForces[,] sBIF_x, out designBucklingLengthFactors[] sBucklingLengthFactors, out designMomentValuesForCb[] sMomentValuesforCb)
        {
            sBucklingLengthFactors = new designBucklingLengthFactors[iNumberOfLoadCombinations];
            sMomentValuesforCb = new designMomentValuesForCb[iNumberOfLoadCombinations];
            sBIF_x = new basicInternalForces[iNumberOfLoadCombinations, iNumberOfDesignSections];

            // Temporary calculation of internal forces - each combination
            for (int i = 0; i < iNumberOfLoadCombinations; i++)
            {
                CExample_2D_51_SB memberModel_qy = new CExample_2D_51_SB(member.FLength, fE_d_load_values_LCS_y[i]);
                CExample_2D_51_SB memberModel_qz = new CExample_2D_51_SB(member.FLength, fE_d_load_values_LCS_z[i]);

                float fM_abs_max = 0;

                for (int j = 0; j < iNumberOfDesignSections; j++)
                {
                    sBIF_x[i, j].fV_yu = memberModel_qy.m_arrMLoads[0].Get_SSB_V_x(fx_positions[j], member.FLength);
                    sBIF_x[i, j].fM_zv = memberModel_qy.m_arrMLoads[0].Get_SSB_M_x(fx_positions[j], member.FLength);

                    sBIF_x[i, j].fV_zv = memberModel_qz.m_arrMLoads[0].Get_SSB_V_x(fx_positions[j], member.FLength);
                    sBIF_x[i, j].fM_yu = memberModel_qz.m_arrMLoads[0].Get_SSB_M_x(fx_positions[j], member.FLength);

                    sBIF_x[i, j].fN = 0f; // TODO - doplnit vypocet
                    sBIF_x[i, j].fT = 0f; // TODO - doplnit vypocet

                    if (Math.Abs(sBIF_x[i, j].fM_yu) > Math.Abs(fM_abs_max))
                        fM_abs_max = sBIF_x[i, j].fM_yu;
                }

                sBucklingLengthFactors[i].fBeta_x_FB_fl_ex = 1f; // TODO - nastavit pre member - moze zavisiet od zatazenia
                sBucklingLengthFactors[i].fBeta_y_FB_fl_ey = 1f; // TODO - nastavit pre member - moze zavisiet od zatazenia
                sBucklingLengthFactors[i].fBeta_z_TB_TFB_l_ez = 1f; // TODO - nastavit pre member - moze zavisiet od zatazenia
                sBucklingLengthFactors[i].fBeta_LTB_fl_LTB = 1f; // TODO - nastavit pre member - moze zavisiet od zatazenia

                sMomentValuesforCb[i].fM_max = fM_abs_max;
                sMomentValuesforCb[i].fM_14 = memberModel_qz.m_arrMLoads[0].Get_SSB_M_x(0.25f * member.FLength, member.FLength);
                sMomentValuesforCb[i].fM_24 = memberModel_qz.m_arrMLoads[0].Get_SSB_M_x(0.50f * member.FLength, member.FLength);
                sMomentValuesforCb[i].fM_34 = memberModel_qz.m_arrMLoads[0].Get_SSB_M_x(0.75f * member.FLength, member.FLength);
            }
        }

        public void CalculateInternalForcesOnSimpleBeam_SBD(int iNumberOfDesignSections, CMember member, float[] fx_positions, 
            float fE_d_load_value_LCS_y, float fE_d_load_value_LCS_z, out basicInternalForces[,] sBIF_x, out designBucklingLengthFactors[] sBucklingLengthFactors, out designMomentValuesForCb[] sMomentValuesforCb)
        {
            int iNumberOfLoadCombinations = 1;
            float[] fE_d_load_values_LCS_y = new float[1] { fE_d_load_value_LCS_y };
            float[] fE_d_load_values_LCS_z = new float[1] { fE_d_load_value_LCS_z };

            CalculateInternalForcesOnSimpleBeam_SBD(iNumberOfLoadCombinations, iNumberOfDesignSections, member, fx_positions, fE_d_load_values_LCS_y, fE_d_load_values_LCS_z, out sBIF_x, out sBucklingLengthFactors, out sMomentValuesforCb);
        }

        public void CalculateInternalForcesOnSimpleBeam_SBD(int iNumberOfDesignSections, float[] fx_positions, CMember member,
        CMLoad_21 memberload, out basicInternalForces[,] sBIF_x, out designBucklingLengthFactors[] sBucklingLengthFactors,  out designMomentValuesForCb[] sMomentValuesforCb)
        {
            int iNumberOfLoadCombinations = 1;
            float[] fE_d_load_values_LCS_y = new float[1] { memberload.ELoadDir == ELoadDirection.eLD_Y ? memberload.Fq : 0};
            float[] fE_d_load_values_LCS_z = new float[1] { memberload.ELoadDir == ELoadDirection.eLD_Z ? memberload.Fq : 0};

            CalculateInternalForcesOnSimpleBeam_SBD(iNumberOfLoadCombinations, iNumberOfDesignSections, member, fx_positions, fE_d_load_values_LCS_y, fE_d_load_values_LCS_z, out sBIF_x, out sBucklingLengthFactors, out sMomentValuesforCb);
        }

        // PFD - nove funkcie (nezahrnaju kombinacie)

        // Internal Forces
        public void CalculateInternalForcesOnSimpleBeam_PFD(int iNumberOfDesignSections, CMember member, float[] fx_positions,
            CMLoad load, out basicInternalForces[] sBIF_x, out designBucklingLengthFactors sBucklingLengthFactors, out designMomentValuesForCb sMomentValuesforCb)
        {
            sMomentValuesforCb = new designMomentValuesForCb();
            sBIF_x = new basicInternalForces[iNumberOfDesignSections];

            CExample_2D_51_SB memberModel = new CExample_2D_51_SB(member, load);

            float fM_abs_max = 0;

            for (int j = 0; j < iNumberOfDesignSections; j++)
            {

                if (load.ELoadDir == ELoadDirection.eLD_X)
                {
                    sBIF_x[j].fN = memberModel.m_arrMLoads[0].Get_SSB_V_x(fx_positions[j], member.FLength);
                }
                else if (load.ELoadDir == ELoadDirection.eLD_Y)
                {
                    sBIF_x[j].fV_yu = memberModel.m_arrMLoads[0].Get_SSB_V_x(fx_positions[j], member.FLength);
                    sBIF_x[j].fM_zv = memberModel.m_arrMLoads[0].Get_SSB_M_x(fx_positions[j], member.FLength);
                }
                else
                {
                    sBIF_x[j].fV_zv = memberModel.m_arrMLoads[0].Get_SSB_M_x(fx_positions[j], member.FLength);
                    sBIF_x[j].fM_yu = memberModel.m_arrMLoads[0].Get_SSB_M_x(fx_positions[j], member.FLength);
                }

                sBIF_x[j].fT = 0f; // TODO - doplnit vypocet

                if (Math.Abs(sBIF_x[j].fM_yu) > Math.Abs(fM_abs_max))
                    fM_abs_max = sBIF_x[j].fM_yu;
            }

            sBucklingLengthFactors.fBeta_x_FB_fl_ex = 1f; // TODO - nastavit pre member - moze zavisiet od zatazenia
            sBucklingLengthFactors.fBeta_y_FB_fl_ey = 1f; // TODO - nastavit pre member - moze zavisiet od zatazenia
            sBucklingLengthFactors.fBeta_z_TB_TFB_l_ez = 1f; // TODO - nastavit pre member - moze zavisiet od zatazenia
            sBucklingLengthFactors.fBeta_LTB_fl_LTB = 1f; // TODO - nastavit pre member - moze zavisiet od zatazenia

            if (load.ELoadDir == ELoadDirection.eLD_Z)
            {
                sMomentValuesforCb.fM_max = fM_abs_max;
                sMomentValuesforCb.fM_14 = memberModel.m_arrMLoads[0].Get_SSB_M_x(0.25f * member.FLength, member.FLength);
                sMomentValuesforCb.fM_24 = memberModel.m_arrMLoads[0].Get_SSB_M_x(0.50f * member.FLength, member.FLength);
                sMomentValuesforCb.fM_34 = memberModel.m_arrMLoads[0].Get_SSB_M_x(0.75f * member.FLength, member.FLength);
            }
        }

        public void CalculateInternalForcesOnSimpleBeam_PFD(int iNumberOfDesignSections, float[] fx_positions, CMember member,
        CMLoad memberload, out basicInternalForces[] sBIF_x, out designBucklingLengthFactors sBucklingLengthFactors, out designMomentValuesForCb sMomentValuesforCb)
        {
            CalculateInternalForcesOnSimpleBeam_PFD(iNumberOfDesignSections, member, fx_positions, memberload, out sBIF_x, out sBucklingLengthFactors, out sMomentValuesforCb);
        }

        // Deflections
        public void CalculateDeflectionsOnSimpleBeam_PFD(int iNumberOfDesignSections, CMember member, float[] fx_positions,
            CMLoad load, out basicDeflections[] sBDeflections_x)
        {
            sBDeflections_x = new basicDeflections[iNumberOfDesignSections];

            CExample_2D_51_SB memberModel = new CExample_2D_51_SB(member, load);

            for (int j = 0; j < iNumberOfDesignSections; j++)
            {
                if (load.ELoadDir == ELoadDirection.eLD_Z)
                {
                    sBDeflections_x[j].fDelta_yu = memberModel.m_arrMLoads[0].Get_SSB_Delta_x(fx_positions[j], member.FLength, member.CrScStart.m_Mat.m_fE, (float)member.CrScStart.I_z);
                    sBDeflections_x[j].fDelta_yy = memberModel.m_arrMLoads[0].Get_SSB_Delta_x(fx_positions[j], member.FLength, member.CrScStart.m_Mat.m_fE, (float)member.CrScStart.I_z);
                }
                else
                {
                    sBDeflections_x[j].fDelta_zv = memberModel.m_arrMLoads[0].Get_SSB_Delta_x(fx_positions[j], member.FLength, member.CrScStart.m_Mat.m_fE, (float)member.CrScStart.I_y);
                    sBDeflections_x[j].fDelta_zz = memberModel.m_arrMLoads[0].Get_SSB_Delta_x(fx_positions[j], member.FLength, member.CrScStart.m_Mat.m_fE, (float)member.CrScStart.I_y);
                }

                sBDeflections_x[j].fDelta_tot = MathF.Sqrt(MathF.Pow2(sBDeflections_x[j].fDelta_yy) + MathF.Pow2(sBDeflections_x[j].fDelta_zz)); // Vektorovy sucin pre vyslednicu
            }
        }

        public void CalculateDeflectionsOnSimpleBeam_PFD(int iNumberOfDesignSections, float[] fx_positions, CMember member,
        CMLoad memberload, out basicDeflections[] sBDeflections_x)
        {
            CalculateDeflectionsOnSimpleBeam_PFD(iNumberOfDesignSections, member, fx_positions, memberload, out sBDeflections_x);
        }
    }
}
