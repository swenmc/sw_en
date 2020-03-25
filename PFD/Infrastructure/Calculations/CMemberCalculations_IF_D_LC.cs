using BaseClasses;
using FEM_CALC_BASE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFD.Infrastructure
{
    public class CMemberCalculations_IF_D_LC
    {
        //-------------------------------------------------------------------------------------------------------------------------------
        CMemberCalculations_IF_D_LCAsyncStub stub = null;
        public delegate void CMemberCalculations_IF_D_LCAsyncStub(CMember m, bool DeterminateCombinationResultsByFEMSolver, int iNumberOfDesignSections, int iNumberOfDesignSegments,
            float[] fx_positions, CModel_PFD Model, List<CFrame> frameModels, bool UseFEMSolverCalculationForSimpleBeam, List<CBeam_Simple> beamSimpleModels, bool MUseCRSCGeometricalAxes);
        private Object theLock;

        public List<CMemberInternalForcesInLoadCases> MemberInternalForcesInLoadCases;
        public List<CMemberDeflectionsInLoadCases> MemberDeflectionsInLoadCases;

        //-------------------------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------------------------
        public CMemberCalculations_IF_D_LC(Object lockObject)
        {
            theLock = lockObject;
        }
                

        


        //-------------------------------------------------------------------------------------------------------------------------------
        public IAsyncResult BeginMemberCalculations_IF_D_LC(CMember m, bool DeterminateCombinationResultsByFEMSolver, int iNumberOfDesignSections, int iNumberOfDesignSegments,
            float[] fx_positions, CModel_PFD Model, List<CFrame> frameModels, bool UseFEMSolverCalculationForSimpleBeam, List<CBeam_Simple> beamSimpleModels, bool MUseCRSCGeometricalAxes, AsyncCallback cb, object s)
        {
            stub = new CMemberCalculations_IF_D_LCAsyncStub(Calculate_InternalForcesAndDeflections_LoadCases);
            //using delegate for asynchronous implementation
            return stub.BeginInvoke(m, DeterminateCombinationResultsByFEMSolver, iNumberOfDesignSections, iNumberOfDesignSegments,
                    fx_positions, Model, frameModels, UseFEMSolverCalculationForSimpleBeam, beamSimpleModels, MUseCRSCGeometricalAxes, cb, null);
        }

        //-------------------------------------------------------------------------------------------------------------------------------
        public void EndMemberCalculations_IF_D_LC(IAsyncResult call)
        {
            stub.EndInvoke(call);
        }

        public void Calculate_InternalForcesAndDeflections_LoadCases(CMember m, bool DeterminateCombinationResultsByFEMSolver, int iNumberOfDesignSections, int iNumberOfDesignSegments,
            float[] fx_positions, CModel_PFD Model, List<CFrame> frameModels, bool UseFEMSolverCalculationForSimpleBeam, List<CBeam_Simple> beamSimpleModels, bool MUseCRSCGeometricalAxes)
        {
            designMomentValuesForCb[] sMomentValuesforCb = null;
            basicInternalForces[] sBIF_x = null;
            basicDeflections[] sBDeflections_x = null;
            designBucklingLengthFactors[] sBucklingLengthFactors = null;
            SimpleBeamCalculation calcModel = new SimpleBeamCalculation();

            MemberInternalForcesInLoadCases = new List<CMemberInternalForcesInLoadCases>();
            MemberDeflectionsInLoadCases = new List<CMemberDeflectionsInLoadCases>();
            // Calculate Internal Forces For Load Cases

            // Pre load cases nebudem urcovat MomentValuesforCb, pretoze neposudzujeme samostatne load case a pre kombinacie je to nepouzitelne, pretoze hodnoty Mmax mozu byt pre kazdy load case na inej casti segmentu, takze sa to neda pre segment
            // superponovat, ale je potrebne urcit maximum Mmax pre segment z priebehu vnutornych sil v kombinacii

            if (!m.BIsGenerated) return;
            if (!m.BIsSelectedForIFCalculation) return; // Only structural members (not auxiliary members or members with deactivated calculation of internal forces)

            if (!DeterminateCombinationResultsByFEMSolver)
            {
                for (int i = 0; i < iNumberOfDesignSections; i++)
                    fx_positions[i] = ((float)i / (float)iNumberOfDesignSegments) * m.FLength; // Int must be converted to the float to get decimal numbers

                foreach (CLoadCase lc in Model.m_arrLoadCases)
                {
                    // Frame member
                    if (m.EMemberType == EMemberType_FS.eMC || m.EMemberType == EMemberType_FS.eMR ||
                        m.EMemberType == EMemberType_FS.eEC || m.EMemberType == EMemberType_FS.eER)
                    {
                        // BEFENet - calculate load cases only

                        // Set indices to search in results
                        int iFrameIndex = CModelHelper.GetFrameIndexForMember(m, frameModels);  //podla ID pruta treba identifikovat do ktoreho ramu patri

                        // Calculate internal forces and deflections in load cases
                        // Pocitame vsetko pretoze pre urcenie priehybu v SLS potrebujeme Ieff urcene z vysledkov posudenia pre vnutorne sily v SLS

                        sBucklingLengthFactors = new designBucklingLengthFactors[iNumberOfDesignSections];
                        //sMomentValuesforCb = new designMomentValuesForCb[iNumberOfDesignSections];

                        for (int j = 0; j < fx_positions.Length; j++)
                        {
                            designBucklingLengthFactors sBucklingLengthFactors_temp = new designBucklingLengthFactors();
                            //designMomentValuesForCb sMomentValuesforCb_temp = new designMomentValuesForCb();
                            //SetMomentValuesforCb_design_And_BucklingFactors_Frame(fx_positions[j], iFrameIndex, lc.ID, m, ref sBucklingLengthFactors_temp/*, ref sMomentValuesforCb_temp*/);
                            SetBucklingFactors_Frame(fx_positions[j], iFrameIndex, lc.ID, m, Model, frameModels, ref sBucklingLengthFactors_temp);

                            sBucklingLengthFactors[j] = sBucklingLengthFactors_temp;
                            //sMomentValuesforCb[j] = sMomentValuesforCb_temp;
                        }

                        // ULS - internal forces
                        sBIF_x = frameModels[iFrameIndex].LoadCombInternalForcesResults[lc.ID][m.ID].InternalForces.ToArray();

                        // SLS - deflections
                        sBDeflections_x = frameModels[iFrameIndex].LoadCombInternalForcesResults[lc.ID][m.ID].Deflections.ToArray();
                    }
                    else // Single member
                    {
                        if (UseFEMSolverCalculationForSimpleBeam)
                        {
                            // BEFENet - calculate load cases only

                            // Set indices to search in results
                            int iSimpleBeamIndex = CModelHelper.GetSimpleBeamIndexForMember(m, beamSimpleModels);  //podla ID pruta treba identifikovat do ktoreho simple beam modelu patri

                            // Calculate internal forces and deflections in load cases
                            // Pocitame vsetko pretoze pre urcenie priehybu v SLS potrebujeme Ieff urcene z vysledkov posudenia pre vnutorne sily v SLS
                            sBucklingLengthFactors = new designBucklingLengthFactors[iNumberOfDesignSections];
                            sMomentValuesforCb = new designMomentValuesForCb[iNumberOfDesignSections];

                            for (int j = 0; j < fx_positions.Length; j++)
                            {
                                designBucklingLengthFactors sBucklingLengthFactors_temp = new designBucklingLengthFactors();
                                //designMomentValuesForCb sMomentValuesforCb_temp = new designMomentValuesForCb();
                                //SetMomentValuesforCb_design_And_BucklingFactors_SimpleBeamSegment(fx_positions[j], iSimpleBeamIndex, lc.ID, m, ref sBucklingLengthFactors_temp/*, ref sMomentValuesforCb_temp*/);
                                SetBucklingFactors_SimpleBeamSegment(fx_positions[j], iSimpleBeamIndex, lc.ID, m, Model, ref sBucklingLengthFactors_temp);

                                sBucklingLengthFactors[j] = sBucklingLengthFactors_temp;
                                //sMomentValuesforCb[j] = sMomentValuesforCb_temp;
                            }

                            // ULS - internal forces
                            sBIF_x = beamSimpleModels[iSimpleBeamIndex].LoadCombInternalForcesResults[lc.ID][m.ID].InternalForces.ToArray();

                            // SLS - deflections
                            sBDeflections_x = beamSimpleModels[iSimpleBeamIndex].LoadCombInternalForcesResults[lc.ID][m.ID].Deflections.ToArray();
                        }
                        else
                        {
                            // Calculate internal forces and deflections in load cases
                            // Pocitame vsetko pretoze pre urcenie priehybu v SLS potrebujeme Ieff urcene z vysledkov posudenia pre vnutorne sily v SLS

                            // ULS - internal forces
                            calcModel.CalculateInternalForcesOnSimpleBeam_PFD(MUseCRSCGeometricalAxes, iNumberOfDesignSections, fx_positions, m, lc, out sBIF_x, out sBucklingLengthFactors/*, out sMomentValuesforCb*/);

                            // SLS - deflections
                            calcModel.CalculateDeflectionsOnSimpleBeam_PFD(MUseCRSCGeometricalAxes, iNumberOfDesignSections, fx_positions, m, lc, out sBDeflections_x);
                        }
                    }

                    // Add results
                    if (sBIF_x != null) MemberInternalForcesInLoadCases.Add(new CMemberInternalForcesInLoadCases(m, lc, sBIF_x, /*sMomentValuesforCb,*/ sBucklingLengthFactors));
                    if (sBDeflections_x != null) MemberDeflectionsInLoadCases.Add(new CMemberDeflectionsInLoadCases(m, lc, sBDeflections_x));

                } //end foreach load case
            }
        }


        private void SetBucklingFactors_SimpleBeamSegment(float fx, int iSimpleBeamIndex, int lcombID, CMember member, CModel_PFD Model, ref designBucklingLengthFactors bucklingLengthFactors)
        {
            if (iSimpleBeamIndex < 0)
            {
                //To Mato - V Load Cases som zaskrtol druhe option a tu sa mi dostalo iSimpleBeamIndex = -1;
                return;
            }
            // Create load combination (FEM solver object)
            BriefFiniteElementNet.LoadCombination lcomb = new BriefFiniteElementNet.LoadCombination();
            lcomb = ConvertLoadCombinationtoBFENet(Model.m_arrLoadCombs[lcombID - 1], Model);

            float fSegmentStart_abs, fSegmentEnd_abs;
            GetSegmentStartAndEndFor_xLocation(fx, member, out fSegmentStart_abs, out fSegmentEnd_abs);
            GetSegmentBucklingFactors_xLocation(fx, member, lcombID, ref bucklingLengthFactors);
        }

        private void SetBucklingFactors_Frame(float fx, int iFrameIndex, int lcombID, CMember member, CModel_PFD Model, List<CFrame> frameModels, ref designBucklingLengthFactors bucklingLengthFactors)
        {
            // Create load combination (FEM solver object)
            BriefFiniteElementNet.LoadCombination lcomb = new BriefFiniteElementNet.LoadCombination();
            lcomb = ConvertLoadCombinationtoBFENet(Model.m_arrLoadCombs[lcombID - 1], Model);

            int iMemberIndex_FM = frameModels[iFrameIndex].GetMemberIndexInFrame(member);

            float fSegmentStart_abs, fSegmentEnd_abs;
            GetSegmentStartAndEndFor_xLocation(fx, member, out fSegmentStart_abs, out fSegmentEnd_abs);
            GetSegmentBucklingFactors_xLocation(fx, member, lcombID, ref bucklingLengthFactors);
        }

        private BriefFiniteElementNet.LoadCombination ConvertLoadCombinationtoBFENet(CLoadCombination loadcomb_input, CModel_PFD Model)
        {
            // Create load combination (FEM solver object)
            BriefFiniteElementNet.LoadCombination lcomb_output = new BriefFiniteElementNet.LoadCombination();
            lcomb_output.LcID = loadcomb_input.ID;

            // Add specific load cases into the combination and set load factors
            for (int j = 0; j < Model.m_arrLoadCombs[loadcomb_input.ID - 1].LoadCasesList.Count; j++)
            {
                BriefFiniteElementNet.LoadType loadtype = CModelToBFEMNetConverter.GetBFEMLoadType(Model.m_arrLoadCombs[loadcomb_input.ID - 1].LoadCasesList[j].Type);
                BriefFiniteElementNet.LoadCase loadCase = new BriefFiniteElementNet.LoadCase(Model.m_arrLoadCases[j].Name, loadtype);
                lcomb_output.Add(loadCase, Model.m_arrLoadCombs[loadcomb_input.ID - 1].LoadCasesFactorsList[j]);
            }

            return lcomb_output;
        }

        private void GetSegmentStartAndEndFor_xLocation(float fx, CMember member, out float fSegmentStart_abs, out float fSegmentEnd_abs)
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

        public void GetSegmentBucklingFactors_xLocation(float fx, CMember member, int lcombID, ref designBucklingLengthFactors bucklingLengthFactors)
        {
            bucklingLengthFactors = new designBucklingLengthFactors();
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
                        else if (member.LTBSegmentGroup[i].BucklingLengthFactors.Count == 1) // Defined only once - prut ma rovnake faktory pre vzperne dlzky pre vsetky kombinacie.
                        {
                            bucklingLengthFactors = member.LTBSegmentGroup[i].BucklingLengthFactors[0];

                        }
                        else // if(bucklingLengthFactors.Count > 1) // Different values for load combinations
                            bucklingLengthFactors = member.LTBSegmentGroup[i].BucklingLengthFactors[lcombID - 1];
                    }
                }
            }
        }





    }
}
