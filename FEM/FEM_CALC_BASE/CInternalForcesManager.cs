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
        public static void SetMemberInternalForcesInLoadCombination(bool bUseCRSCGeometricalAxes, CMember m, CLoadCombination lcomb, List<CMemberInternalForcesInLoadCombinations> listMemberLoadForces, 
            int iNumberOfMemberResultsSections, 
            out designBucklingLengthFactors[] sBucklingLengthFactors, out designMomentValuesForCb[] sMomentValuesforCb_output, out basicInternalForces[] sBIF_x_output)
        {
            sBucklingLengthFactors = new designBucklingLengthFactors[iNumberOfMemberResultsSections];
            sMomentValuesforCb_output = new designMomentValuesForCb[iNumberOfMemberResultsSections];
            sBIF_x_output = new basicInternalForces[iNumberOfMemberResultsSections];

            if (listMemberLoadForces != null) // If some results data exist
            {
                CMemberInternalForcesInLoadCombinations mlf = listMemberLoadForces.Find(i => i.Member.ID == m.ID && i.LoadCombination.ID == lcomb.ID);

                if (mlf != null)
                {
                    for(int j=0; j < mlf.InternalForces.Length; j++)
                    {
                        sBIF_x_output[j].fN = mlf.InternalForces[j].fN;
                        sBIF_x_output[j].fT = mlf.InternalForces[j].fT;

                        if (bUseCRSCGeometricalAxes)
                        {
                            sBIF_x_output[j].fV_yy = mlf.InternalForces[j].fV_yy;
                            sBIF_x_output[j].fV_zz = mlf.InternalForces[j].fV_zz;
                            sBIF_x_output[j].fM_yy = mlf.InternalForces[j].fM_yy;
                            sBIF_x_output[j].fM_zz = mlf.InternalForces[j].fM_zz;
                        }
                        else
                        {
                            sBIF_x_output[j].fV_yu = mlf.InternalForces[j].fV_yu;
                            sBIF_x_output[j].fV_zv = mlf.InternalForces[j].fV_zv;
                            sBIF_x_output[j].fM_yu = mlf.InternalForces[j].fM_yu;
                            sBIF_x_output[j].fM_zv = mlf.InternalForces[j].fM_zv;
                        }

                        sMomentValuesforCb_output[j].fM_max = mlf.BendingMomentValues[j].fM_max;
                        sMomentValuesforCb_output[j].fM_14 = mlf.BendingMomentValues[j].fM_14;
                        sMomentValuesforCb_output[j].fM_24 = mlf.BendingMomentValues[j].fM_24;
                        sMomentValuesforCb_output[j].fM_34 = mlf.BendingMomentValues[j].fM_34;

                        float fx = (float)j / (float)(iNumberOfMemberResultsSections - 1) * m.FLength;
                        sBucklingLengthFactors[j] = GetSegmentBucklingFactors_xLocation(fx, m, lcomb.ID);
                    }
                }
            }
        }

        public static void SetMemberInternalForcesInLoadCombination(bool bUseCRSCGeometricalAxes, CMember m, CLoadCombination lcomb, List<CMemberInternalForcesInLoadCases> listMemberLoadForces, 
            int iNumberOfMemberResultsSections, 
            out designBucklingLengthFactors[] sBucklingLengthFactors, out designMomentValuesForCb[] sMomentValuesforCb_output, out basicInternalForces[] sBIF_x_output)
        {
            sBucklingLengthFactors = new designBucklingLengthFactors[iNumberOfMemberResultsSections];
            sMomentValuesforCb_output = new designMomentValuesForCb[iNumberOfMemberResultsSections];
            sBIF_x_output = new basicInternalForces[iNumberOfMemberResultsSections];

            if (listMemberLoadForces != null) // If some results data exist
            {
                for(int lcIndex = 0; lcIndex < lcomb.LoadCasesList.Count; lcIndex++)
                {
                    CMemberInternalForcesInLoadCases mlf = listMemberLoadForces.Find(i => i.Member.ID == m.ID && i.LoadCase.ID == lcomb.LoadCasesList[lcIndex].ID);
                    if (mlf != null)
                    {
                        float fcurrentLoadCaseFactor = lcomb.LoadCasesFactorsList[lcIndex];

                        for (int j = 0; j < mlf.InternalForces.Length; j++)
                        {
                            sBIF_x_output[j].fN += fcurrentLoadCaseFactor * mlf.InternalForces[j].fN;

                            if (bUseCRSCGeometricalAxes)
                            {
                                sBIF_x_output[j].fV_yy += fcurrentLoadCaseFactor * mlf.InternalForces[j].fV_yy;
                                sBIF_x_output[j].fV_zz += fcurrentLoadCaseFactor * mlf.InternalForces[j].fV_zz;
                                sBIF_x_output[j].fM_yy += fcurrentLoadCaseFactor * mlf.InternalForces[j].fM_yy;
                                sBIF_x_output[j].fM_zz += fcurrentLoadCaseFactor * mlf.InternalForces[j].fM_zz;
                            }
                            else
                            {
                                sBIF_x_output[j].fT += fcurrentLoadCaseFactor * mlf.InternalForces[j].fT;
                                sBIF_x_output[j].fV_yu += fcurrentLoadCaseFactor * mlf.InternalForces[j].fV_yu;
                                sBIF_x_output[j].fV_zv += fcurrentLoadCaseFactor * mlf.InternalForces[j].fV_zv;
                                sBIF_x_output[j].fM_yu += fcurrentLoadCaseFactor * mlf.InternalForces[j].fM_yu;
                                sBIF_x_output[j].fM_zv += fcurrentLoadCaseFactor * mlf.InternalForces[j].fM_zv;
                            }

                            sMomentValuesforCb_output[j].fM_max += fcurrentLoadCaseFactor * mlf.BendingMomentValues[j].fM_max;
                            sMomentValuesforCb_output[j].fM_14 += fcurrentLoadCaseFactor * mlf.BendingMomentValues[j].fM_14;
                            sMomentValuesforCb_output[j].fM_24 += fcurrentLoadCaseFactor * mlf.BendingMomentValues[j].fM_24;
                            sMomentValuesforCb_output[j].fM_34 += fcurrentLoadCaseFactor * mlf.BendingMomentValues[j].fM_34;

                            // Validate
                            if (sMomentValuesforCb_output[j].fM_14 == float.NaN ||
                               sMomentValuesforCb_output[j].fM_24 == float.NaN ||
                               sMomentValuesforCb_output[j].fM_34 == float.NaN ||
                               sMomentValuesforCb_output[j].fM_max == float.NaN)
                            {
                                throw new ArgumentNullException("Invalid value of bending moment.");
                            }

                            float fx = (float)j / (float)(iNumberOfMemberResultsSections - 1) * m.FLength;
                            sBucklingLengthFactors[j] = GetSegmentBucklingFactors_xLocation(fx, m, lcomb.ID);
                        }
                    }
                }
            }
        }

        //  Deflections - SLS
        public static void SetMemberDeflectionsInLoadCombination(bool bUseCRSCGeometricalAxes, CMember m, CLoadCombination lcomb, List<CMemberDeflectionsInLoadCombinations> listMemberDeflections, int iNumberOfMemberResultsSections, out basicDeflections[] sBDeflections_x_output)
        {
            sBDeflections_x_output = new basicDeflections[iNumberOfMemberResultsSections];

            if (listMemberDeflections != null) // If some results data exist
            {
                CMemberDeflectionsInLoadCombinations mdefl = listMemberDeflections.Find(i => i.Member.ID == m.ID && i.LoadCombination.ID == lcomb.ID);
                if (mdefl != null)
                {
                    int j = 0;
                    foreach (basicDeflections bdef in mdefl.Deflections)
                    {
                        if (bUseCRSCGeometricalAxes)
                        {
                            sBDeflections_x_output[j].fDelta_yy = bdef.fDelta_yy;
                            sBDeflections_x_output[j].fDelta_zz = bdef.fDelta_zz;
                        }
                        else
                        {
                            sBDeflections_x_output[j].fDelta_zv = bdef.fDelta_zv;
                            sBDeflections_x_output[j].fDelta_yu = bdef.fDelta_yu;
                        }

                        if (bUseCRSCGeometricalAxes && (!MathF.d_equal(sBDeflections_x_output[j].fDelta_yy, 0) || !MathF.d_equal(sBDeflections_x_output[j].fDelta_zz, 0)))
                            sBDeflections_x_output[j].fDelta_tot = MathF.Sqrt(MathF.Pow2(sBDeflections_x_output[j].fDelta_yy) + MathF.Pow2(sBDeflections_x_output[j].fDelta_zz)); // Vektorovy sucin pre vyslednicu
                        else if (!MathF.d_equal(sBDeflections_x_output[j].fDelta_yu, 0) || !MathF.d_equal(sBDeflections_x_output[j].fDelta_zv, 0))
                            sBDeflections_x_output[j].fDelta_tot = MathF.Sqrt(MathF.Pow2(sBDeflections_x_output[j].fDelta_yu) + MathF.Pow2(sBDeflections_x_output[j].fDelta_zv)); // Vektorovy sucin pre vyslednicu
                        else
                            sBDeflections_x_output[j].fDelta_tot = 0;

                        j++;
                    }
                }
            }
        }

        public static void SetMemberDeflectionsInLoadCombination(bool bUseCRSCGeometricalAxes, CMember m, CLoadCombination lcomb, List<CMemberDeflectionsInLoadCases> listMemberDeflections, int iNumberOfMemberResultsSections, out basicDeflections[] sBDeflections_x_output)
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
                            if (bUseCRSCGeometricalAxes)
                            {
                                sBDeflections_x_output[j].fDelta_yy += fcurrentLoadCaseFactor * bdef.fDelta_yy;
                                sBDeflections_x_output[j].fDelta_zz += fcurrentLoadCaseFactor * bdef.fDelta_zz;
                            }
                            else
                            {
                                sBDeflections_x_output[j].fDelta_yu += fcurrentLoadCaseFactor * bdef.fDelta_yu;
                                sBDeflections_x_output[j].fDelta_zv += fcurrentLoadCaseFactor * bdef.fDelta_zv;
                            }

                            if (bUseCRSCGeometricalAxes && (!MathF.d_equal(sBDeflections_x_output[j].fDelta_yy, 0) || !MathF.d_equal(sBDeflections_x_output[j].fDelta_zz, 0)))
                                sBDeflections_x_output[j].fDelta_tot = MathF.Sqrt(MathF.Pow2(sBDeflections_x_output[j].fDelta_yy) + MathF.Pow2(sBDeflections_x_output[j].fDelta_zz)); // Vektorovy sucin pre vyslednicu
                            else if (!MathF.d_equal(sBDeflections_x_output[j].fDelta_yu, 0) || !MathF.d_equal(sBDeflections_x_output[j].fDelta_zv, 0))
                                sBDeflections_x_output[j].fDelta_tot = MathF.Sqrt(MathF.Pow2(sBDeflections_x_output[j].fDelta_yu) + MathF.Pow2(sBDeflections_x_output[j].fDelta_zv)); // Vektorovy sucin pre vyslednicu
                            else
                                sBDeflections_x_output[j].fDelta_tot = 0;

                            j++;
                        }
                    }
                }
            }
        }

        // Refaktorovat
        public static designBucklingLengthFactors GetSegmentBucklingFactors_xLocation(float fx, CMember member, int lcombID)
        {
            designBucklingLengthFactors bucklingLengthFactors = new designBucklingLengthFactors();
            bucklingLengthFactors.fBeta_x_FB_fl_ex = 1.0f;
            bucklingLengthFactors.fBeta_y_FB_fl_ey = 1.0f;
            bucklingLengthFactors.fBeta_z_TB_TFB_l_ez = 1.0f;
            bucklingLengthFactors.fBeta_LTB_fl_LTB = 1.0f;

            if (member.LTBSegmentGroup != null && member.LTBSegmentGroup.Count > 1) // More than one LTB segment exists
            {
                for (int i = 0; i < member.LTBSegmentGroup.Count; i++)
                {
                    if (fx >= member.LTBSegmentGroup[i].SegmentStartCoord_Abs && fx <= member.LTBSegmentGroup[i].SegmentEndCoord_Abs)
                    {
                        if (member.LTBSegmentGroup[i].BucklingLengthFactors == null || member.LTBSegmentGroup[i].BucklingLengthFactors.Count == 0) // Default
                        {
                            bucklingLengthFactors = new designBucklingLengthFactors();
                            bucklingLengthFactors.fBeta_x_FB_fl_ex = 1.0f;
                            bucklingLengthFactors.fBeta_y_FB_fl_ey = 1.0f;
                            bucklingLengthFactors.fBeta_z_TB_TFB_l_ez = 1.0f;
                            bucklingLengthFactors.fBeta_LTB_fl_LTB = 1.0f;
                        }
                        else if (member.LTBSegmentGroup[i].BucklingLengthFactors.Count == 1) // Defined only once
                        {
                            bucklingLengthFactors = member.LTBSegmentGroup[i].BucklingLengthFactors[0];

                        }
                        else // if(bucklingLengthFactors.Count > 1) // Different values for load combinations
                            bucklingLengthFactors = member.LTBSegmentGroup[i].BucklingLengthFactors[lcombID - 1];
                    }
                }
            }

            return bucklingLengthFactors;
        }
    }
}
