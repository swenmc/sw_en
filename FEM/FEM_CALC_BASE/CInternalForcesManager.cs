using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaseClasses;
using MATH;

namespace FEM_CALC_BASE
{
    public static class CMemberResultsManager
    {
        // Internal forces - ULS
        public static void SetMemberInternalForcesInLoadCombination(CMember m, CLoadCombination lcomb, List<CMemberInternalForcesInLoadCases> listMemberLoadForces, int iNumberOfMemberResultsSections, out designBucklingLengthFactors sBucklingLengthFactors, out designMomentValuesForCb sMomentValuesforCb_output, out basicInternalForces[] sBIF_x_output)
        {
            sBucklingLengthFactors = new designBucklingLengthFactors();
            sMomentValuesforCb_output = new designMomentValuesForCb();
            sBIF_x_output = new basicInternalForces[iNumberOfMemberResultsSections];

            if (listMemberLoadForces != null) // If some results data exist
            {
                for(int lcIndex = 0; lcIndex < lcomb.LoadCasesList.Count; lcIndex++)
                {
                    CMemberInternalForcesInLoadCases mlf = listMemberLoadForces.Find(i => i.Member.ID == m.ID && i.LoadCase.ID == lcomb.LoadCasesList[lcIndex].ID);
                    if (mlf != null)
                    {
                        // TODO - malo by sa nastavovat u pruta ale moze zavisiet od zatazenia
                        sBucklingLengthFactors.fBeta_x_FB_fl_ex = 1f;
                        sBucklingLengthFactors.fBeta_y_FB_fl_ey = 1f;
                        sBucklingLengthFactors.fBeta_z_TB_TFB_l_ez = 1f;
                        sBucklingLengthFactors.fBeta_LTB_fl_LTB = 1f;

                        float fcurrentLoadCaseFactor = lcomb.LoadCasesFactorsList[lcIndex];

                        sMomentValuesforCb_output.fM_14 += fcurrentLoadCaseFactor * mlf.BendingMomentValues.fM_14;
                        sMomentValuesforCb_output.fM_24 += fcurrentLoadCaseFactor * mlf.BendingMomentValues.fM_24;
                        sMomentValuesforCb_output.fM_34 += fcurrentLoadCaseFactor * mlf.BendingMomentValues.fM_34;
                        sMomentValuesforCb_output.fM_max += fcurrentLoadCaseFactor * mlf.BendingMomentValues.fM_max;

                        int j = 0;
                        foreach (basicInternalForces bif in mlf.InternalForces)
                        {
                            sBIF_x_output[j].fN += fcurrentLoadCaseFactor * bif.fN;
                            sBIF_x_output[j].fV_yu += fcurrentLoadCaseFactor * bif.fV_yu;
                            sBIF_x_output[j].fV_yy += fcurrentLoadCaseFactor * bif.fV_yy;
                            sBIF_x_output[j].fV_zv += fcurrentLoadCaseFactor * bif.fV_zv;
                            sBIF_x_output[j].fV_zz += fcurrentLoadCaseFactor * bif.fV_zz;
                            sBIF_x_output[j].fT += fcurrentLoadCaseFactor * bif.fT;
                            sBIF_x_output[j].fM_yu += fcurrentLoadCaseFactor * bif.fM_yu;
                            sBIF_x_output[j].fM_yy += fcurrentLoadCaseFactor * bif.fM_yy;
                            sBIF_x_output[j].fM_zv += fcurrentLoadCaseFactor * bif.fM_zv;
                            sBIF_x_output[j].fM_zz += fcurrentLoadCaseFactor * bif.fM_zz;

                            j++;
                        }
                    }
                }
            }
        }

        //  Deflections - SLS
        public static void SetMemberDeflectionsInLoadCombination(CMember m, CLoadCombination lcomb, List<CMemberDeflectionsInLoadCases> listMemberDeflections, int iNumberOfMemberResultsSections, out basicDeflections[] sBDeflections_x_output)
        {
            sBDeflections_x_output = new basicDeflections[iNumberOfMemberResultsSections];

            if (listMemberDeflections != null) // If some results data exist
            {
                for(int lcIndex = 0; lcIndex < lcomb.LoadCasesList.Count; lcIndex++)
                {
                    CMemberDeflectionsInLoadCases mdefl = listMemberDeflections.Find(i => i.Member.ID == m.ID && i.LoadCase.ID == lcomb.LoadCasesList[lcIndex].ID);
                    if (mdefl != null)
                    {
                        float fcurrentLoadCaseFactor = lcomb.LoadCasesFactorsList[lcIndex];

                        int j = 0;
                        foreach (basicDeflections bdef in mdefl.Deflections)
                        {
                            sBDeflections_x_output[j].fDelta_yu += fcurrentLoadCaseFactor * bdef.fDelta_yu;
                            sBDeflections_x_output[j].fDelta_yy += fcurrentLoadCaseFactor * bdef.fDelta_yy;
                            sBDeflections_x_output[j].fDelta_zv += fcurrentLoadCaseFactor * bdef.fDelta_zv;
                            sBDeflections_x_output[j].fDelta_zz += fcurrentLoadCaseFactor * bdef.fDelta_zz;
                            sBDeflections_x_output[j].fDelta_tot += (float)Math.Sqrt(MathF.Pow2(fcurrentLoadCaseFactor * bdef.fDelta_yu) + MathF.Pow2(fcurrentLoadCaseFactor * bdef.fDelta_zv));

                            j++;
                        }
                    }
                }
            }
        }
    }
}
