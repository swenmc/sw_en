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

                            //sMomentValuesforCb_output[j].fM_max += fcurrentLoadCaseFactor * mlf.BendingMomentValues[j].fM_max;
                            //sMomentValuesforCb_output[j].fM_14 += fcurrentLoadCaseFactor * mlf.BendingMomentValues[j].fM_14;
                            //sMomentValuesforCb_output[j].fM_24 += fcurrentLoadCaseFactor * mlf.BendingMomentValues[j].fM_24;
                            //sMomentValuesforCb_output[j].fM_34 += fcurrentLoadCaseFactor * mlf.BendingMomentValues[j].fM_34;

                            // Validate
                            //if (sMomentValuesforCb_output[j].fM_14 == float.NaN ||
                            //   sMomentValuesforCb_output[j].fM_24 == float.NaN ||
                            //   sMomentValuesforCb_output[j].fM_34 == float.NaN ||
                            //   sMomentValuesforCb_output[j].fM_max == float.NaN)
                            //{
                            //    throw new ArgumentNullException("Invalid value of bending moment.");
                            //}

                            float fx = (float)j / (float)(iNumberOfMemberResultsSections - 1) * m.FLength;
                            sBucklingLengthFactors[j] = GetSegmentBucklingFactors_xLocation(fx, m, lcomb.ID);
                        }
                    }
                }

                // Set segment bending moments to calculate Cb
                // We can't use superposition of load case results because value Mmax can be in various positions in load cases included in load combination
                // We must determinate moments from final diagram of bending moment of load combination and particular LTB segment
                float[] fx_positions = new float[iNumberOfMemberResultsSections];

                for (int k = 0; k<fx_positions.Length; k++)
                {
                    fx_positions[k] = (float)k / (float)(iNumberOfMemberResultsSections - 1) * m.FLength;
                }

                sMomentValuesforCb_output = GetMomentValuesforCb_design(bUseCRSCGeometricalAxes, fx_positions, m, sBIF_x_output);
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

        // TODO Refaktorovat s metodou v projekte PFD
        private static void GetSegmentStartAndEndFor_xLocation(float fx, CMember member, out float fSegmentStart_abs, out float fSegmentEnd_abs)
        {
            fSegmentStart_abs = 0f;
            fSegmentEnd_abs = member.FLength;

            if (member.LTBSegmentGroup != null && member.LTBSegmentGroup.Count > 1) // More than one LTB segment exists
            {
                for (int i = 0; i < member.LTBSegmentGroup.Count; i++)
                {
                    if (fx >= member.LTBSegmentGroup[i].SegmentStartCoord_Abs && fx <= member.LTBSegmentGroup[i].SegmentEndCoord_Abs)
                    {
                        fSegmentStart_abs = member.LTBSegmentGroup[i].SegmentStartCoord_Abs;
                        fSegmentEnd_abs = member.LTBSegmentGroup[i].SegmentEndCoord_Abs;
                    }
                }
            }
        }

        private static designMomentValuesForCb[] GetMomentValuesforCb_design(bool bUseCRSCGeometricalAxes, float [] fx_positions, CMember m, basicInternalForces[] sBIF_x_member)
        {
            // For each x location at member
            designMomentValuesForCb[] sMomentValuesforCb_segment = new designMomentValuesForCb[fx_positions.Length];

            for (int i = 0; i < fx_positions.Length; i++)
            {
                designMomentValuesForCb sMomentValuesforCb;

                GetMomentValuesforCb_design_Segment(bUseCRSCGeometricalAxes, fx_positions[i], fx_positions, m, sBIF_x_member, out sMomentValuesforCb);
                sMomentValuesforCb_segment[i] = sMomentValuesforCb;
            }

            return sMomentValuesforCb_segment;
        }

        private static designMomentValuesForCb GetMomentValuesforCb_design_Segment(bool bUseCRSCGeometricalAxes, float fx, float [] fx_positions, CMember m, basicInternalForces[] sBIF_x_member, out designMomentValuesForCb sMomentValuesforCb)
        {
            // x - location at member
            float fSegmentStart_abs;
            float fSegmentEnd_abs;

            GetSegmentStartAndEndFor_xLocation(fx, m, out fSegmentStart_abs, out fSegmentEnd_abs);

            float fSegmentLength = fSegmentEnd_abs - fSegmentStart_abs;

            float fx_M_14 = fSegmentStart_abs + 0.25f * fSegmentLength;
            float fx_M_24 = fSegmentStart_abs + 0.50f * fSegmentLength;
            float fx_M_34 = fSegmentStart_abs + 0.75f * fSegmentLength;

            int resultSectionID_SegmentStart = GetCloseResultsSectionIDFor_x_position(fSegmentStart_abs, fx_positions);
            int resultSectionID_M_14 = GetCloseResultsSectionIDFor_x_position(fx_M_14, fx_positions);
            int resultSectionID_M_24 = GetCloseResultsSectionIDFor_x_position(fx_M_24, fx_positions);
            int resultSectionID_M_34 = GetCloseResultsSectionIDFor_x_position(fx_M_34, fx_positions);
            int resultSectionID_SegmentEnd = GetCloseResultsSectionIDFor_x_position(fSegmentEnd_abs, fx_positions);

            if (bUseCRSCGeometricalAxes)
            {
                sMomentValuesforCb.fM_14 = sBIF_x_member[resultSectionID_M_14].fM_yy;
                sMomentValuesforCb.fM_24 = sBIF_x_member[resultSectionID_M_24].fM_yy;
                sMomentValuesforCb.fM_34 = sBIF_x_member[resultSectionID_M_34].fM_yy;
            }
            else
            {
                sMomentValuesforCb.fM_14 = sBIF_x_member[resultSectionID_M_14].fM_yu;
                sMomentValuesforCb.fM_24 = sBIF_x_member[resultSectionID_M_24].fM_yu;
                sMomentValuesforCb.fM_34 = sBIF_x_member[resultSectionID_M_34].fM_yu;
            }

            sMomentValuesforCb.fM_max = 0;

            int iNumberOfResultSectionsAtSegment = resultSectionID_SegmentEnd - resultSectionID_SegmentStart + 1;

            for (int i = 0; i < iNumberOfResultSectionsAtSegment; i++)
            {
                float fM_max_temp;

                if (bUseCRSCGeometricalAxes)
                {
                    fM_max_temp = sBIF_x_member[resultSectionID_SegmentStart + i].fM_yy;
                }
                else
                {
                    fM_max_temp = sBIF_x_member[resultSectionID_SegmentStart + i].fM_yu;
                }

                if (Math.Abs(fM_max_temp) > Math.Abs(sMomentValuesforCb.fM_max))
                    sMomentValuesforCb.fM_max = fM_max_temp;
            }

            // Check that M_max is more or equal to the maximum from (M_14, M_24, M_34) - symbols M_3, M_4, M_5 used in exception message
            if (Math.Abs(sMomentValuesforCb.fM_max) < MathF.Max(Math.Abs(sMomentValuesforCb.fM_14), Math.Abs(sMomentValuesforCb.fM_24), Math.Abs(sMomentValuesforCb.fM_34)))
            {
                throw new Exception("Maximum value of bending moment doesn't correspond with values of bending moment at segment M₃, M₄, M₅.");
            }

            return sMomentValuesforCb;
        }

        private static int GetCloseResultsSectionIDFor_x_position(float fx, float[] fx_positions)
        {
            int resultID = -1;

            float fdistance = float.MaxValue; // Smallest distance from general position x to the Result Section Position

            for (int i = 0; i < fx_positions.Length; i++)
            {
               if(Math.Abs(fx - fx_positions[i]) < fdistance)
                {
                    resultID = i;
                    fdistance = Math.Abs(fx - fx_positions[i]);
                }
            }

            return resultID;
        }

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
