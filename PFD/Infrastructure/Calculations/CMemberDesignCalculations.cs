﻿using BaseClasses;
using FEM_CALC_BASE;
using M_BASE;
using MATH;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFD.Infrastructure
{
    public class CMemberDesignCalculations
    {
        const int iNumberOfDesignSections = 11; // 11 rezov, 10 segmentov
        const int iNumberOfDesignSegments = iNumberOfDesignSections - 1;

        float[] fx_positions;
        double step;
        private Solver SolverWindow;
        private CModel_PFD_01_GR Model;
        private bool MUseCRSCGeometricalAxes;
        private bool DeterminateCombinationResultsByFEMSolver;
        private bool UseFEMSolverCalculationForSimpleBeam;
        private int membersIFCalcCount;
        private int membersDesignCalcCount;

        public List<CMemberInternalForcesInLoadCases> MemberInternalForcesInLoadCases;
        public List<CMemberDeflectionsInLoadCases> MemberDeflectionsInLoadCases;

        public List<CMemberInternalForcesInLoadCombinations> MemberInternalForcesInLoadCombinations;
        public List<CMemberDeflectionsInLoadCombinations> MemberDeflectionsInLoadCombinations;

        public List<CMemberLoadCombinationRatio_ULS> MemberDesignResults_ULS;
        public List<CMemberLoadCombinationRatio_SLS> MemberDesignResults_SLS;
        public List<CJointLoadCombinationRatio_ULS> JointDesignResults_ULS;

        private List<CFrame> frameModels;
        private List<CBeam_Simple> beamSimpleModels;

        /////////////////////////////////////////////////////////////////////////////////////////////////////////
        public sDesignResults sDesignResults_ULSandSLS = new sDesignResults();
        public sDesignResults sDesignResults_ULS = new sDesignResults();
        public sDesignResults sDesignResults_SLS = new sDesignResults();

        public CMemberDesignCalculations(Solver solverWindow,
            CModel_PFD_01_GR model,
            bool useCRSCGeometricalAxes,
            bool determinateCombinationResultsByFEMSolver,
            bool bUseFEMSolverCalculationForSimpleBeam,
            List<CFrame> FrameModels, List<CBeam_Simple> BeamSimpleModels)
        {
            SolverWindow = solverWindow;
            Model = model;
            MUseCRSCGeometricalAxes = useCRSCGeometricalAxes;
            DeterminateCombinationResultsByFEMSolver = determinateCombinationResultsByFEMSolver;
            UseFEMSolverCalculationForSimpleBeam = bUseFEMSolverCalculationForSimpleBeam;
            beamSimpleModels = BeamSimpleModels;
            frameModels = FrameModels;

            fx_positions = new float[iNumberOfDesignSections];
            membersIFCalcCount = CModelHelper.GetMembersSetForCalculationsCount(model.m_arrMembers);
            membersDesignCalcCount = CModelHelper.GetMembersSetForDesignCalculationsCount(model.m_arrMembers);
            step = (100.0 - SolverWindow.Progress) / (membersIFCalcCount + membersDesignCalcCount);

            MemberInternalForcesInLoadCases = new List<CMemberInternalForcesInLoadCases>();
            MemberDeflectionsInLoadCases = new List<CMemberDeflectionsInLoadCases>();

            MemberInternalForcesInLoadCombinations = new List<CMemberInternalForcesInLoadCombinations>();
            MemberDeflectionsInLoadCombinations = new List<CMemberDeflectionsInLoadCombinations>();

            MemberDesignResults_ULS = new List<CMemberLoadCombinationRatio_ULS>();
            MemberDesignResults_SLS = new List<CMemberLoadCombinationRatio_SLS>();
            JointDesignResults_ULS = new List<CJointLoadCombinationRatio_ULS>();
        }

        public void CalculateAll()
        {
            //if (debugging) System.Diagnostics.Trace.WriteLine("before calculations: " + (DateTime.Now - start).TotalMilliseconds);
            //Calculate_InternalForcesAndDeflections_LoadCases();
            //Calculate_InternalForces_LoadCombinations();
            //Calculate_Deflections_LoadCombinations();

            //To Mato - zamyslam sa preco pocitame aj IF a Deflections for LoadCases aj pre LoadCombinations
            //problem bol v tom,ze pocitali sa IF aj pre load case aj pre load combination, pricom pre load case bezal progress ale pre loadCombination uz nie, ani pre deflections
            Calculate_InternalForces();
            Calculate_MemberDesign_LoadCombinations();

            SolverWindow.Progress = 100;
            SolverWindow.UpdateProgress();
            SolverWindow.SetSumaryFinished(
               GetTextForResultsMessageBox(sDesignResults_ULSandSLS) +
               GetTextForResultsMessageBox(sDesignResults_ULS) +
               GetTextForResultsMessageBox(sDesignResults_SLS));
        }

        public void Calculate_InternalForces()
        {
            int count = 0;
            SolverWindow.SetMemberDesignLoadCase();
            SolverWindow.SetMemberDesignLoadCaseProgress(count, membersIFCalcCount);
            // Calculate Internal Forces For Load Cases

            // Pre load cases nebudem urcovat MomentValuesforCb, pretoze neposudzujeme samostatne load case a pre kombinacie je to nepouzitelne, pretoze hodnoty Mmax mozu byt pre kazdy load case na inej casti segmentu, takze sa to neda pre segment
            // superponovat, ale je potrebne urcit maximum Mmax pre segment z priebehu vnutornych sil v kombinacii

            foreach (CMember m in Model.m_arrMembers)
            {
                if (!m.BIsGenerated) continue;
                if (!m.BIsSelectedForIFCalculation) continue; // Only structural members (not auxiliary members or members with deactivated calculation of internal forces)

                SolverWindow.SetMemberDesignLoadCaseProgress(++count, membersIFCalcCount);

                Calculate_InternalForcesAndDeflections_LoadCases(m);
                Calculate_InternalForces_LoadCombinations(m);
                Calculate_Deflections_LoadCombinations(m);


                SolverWindow.Progress += step;
                SolverWindow.UpdateProgress();
            }
        }

        public void Calculate_InternalForcesAndDeflections_LoadCases(CMember m)
        {
            designMomentValuesForCb[] sMomentValuesforCb = null;
            basicInternalForces[] sBIF_x = null;
            basicDeflections[] sBDeflections_x = null;
            designBucklingLengthFactors[] sBucklingLengthFactors = null;
            SimpleBeamCalculation calcModel = new SimpleBeamCalculation();

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
                            SetBucklingFactors_Frame(fx_positions[j], iFrameIndex, lc.ID, m, ref sBucklingLengthFactors_temp);

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
                                SetBucklingFactors_SimpleBeamSegment(fx_positions[j], iSimpleBeamIndex, lc.ID, m, ref sBucklingLengthFactors_temp);

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

        public void Calculate_InternalForces_LoadCombinations(CMember m)
        {
            // Calculate Internal Forces For Load Combinations
            if (!m.BIsGenerated) return;

            if (m.BIsSelectedForIFCalculation) // Only structural members (not auxiliary members or members with deactivated calculation of internal forces)
            {
                for (int i = 0; i < iNumberOfDesignSections; i++)
                    fx_positions[i] = ((float)i / (float)iNumberOfDesignSegments) * m.FLength; // Int must be converted to the float to get decimal numbers

                // Note:  Kvoli urceniu Ieff pre posudenie priehybu budeme pravdepodobne potrebovat aj vnuntorne sily pre SLS, to znamena ze sa bude pocitat pre vsetky kombinacie
                foreach (CLoadCombination lcomb in Model.m_arrLoadCombs)
                {
                    // Member basic internal forces
                    designBucklingLengthFactors[] sBucklingLengthFactors_design = new designBucklingLengthFactors[iNumberOfDesignSections];
                    designMomentValuesForCb[] sMomentValuesforCb_design = new designMomentValuesForCb[iNumberOfDesignSections];
                    basicInternalForces[] sBIF_x_design = new basicInternalForces[iNumberOfDesignSections];

                    // Frame member - vysledky pocitane pre load combinations
                    if (DeterminateCombinationResultsByFEMSolver && (m.EMemberType == EMemberType_FS.eMC || m.EMemberType == EMemberType_FS.eMR || m.EMemberType == EMemberType_FS.eEC || m.EMemberType == EMemberType_FS.eER))
                    {
                        int iFrameIndex = CModelHelper.GetFrameIndexForMember(m, frameModels);  //podla ID pruta treba identifikovat do ktoreho ramu patri
                        sBIF_x_design = frameModels[iFrameIndex].LoadCombInternalForcesResults[lcomb.ID][m.ID].InternalForces.ToArray();
                        sBucklingLengthFactors_design = new designBucklingLengthFactors[iNumberOfDesignSections];
                        sMomentValuesforCb_design = new designMomentValuesForCb[iNumberOfDesignSections];

                        for (int j = 0; j < fx_positions.Length; j++)
                        {
                            designBucklingLengthFactors sBucklingLengthFactors_temp = new designBucklingLengthFactors();
                            designMomentValuesForCb sMomentValuesforCb_temp = new designMomentValuesForCb();
                            SetMomentValuesforCb_design_And_BucklingFactors_Frame(fx_positions[j], iFrameIndex, lcomb.ID, m, ref sBucklingLengthFactors_temp, ref sMomentValuesforCb_temp);

                            sBucklingLengthFactors_design[j] = sBucklingLengthFactors_temp;
                            sMomentValuesforCb_design[j] = sMomentValuesforCb_temp;
                        }
                    }
                    else if (DeterminateCombinationResultsByFEMSolver) // Single Beam Members - vysledky pocitane v BFENet pre Load Combinations
                    {
                        int iSimpleBeamIndex = CModelHelper.GetSimpleBeamIndexForMember(m, beamSimpleModels);  //podla ID pruta treba identifikovat do ktoreho simple beam modelu patri
                        sBIF_x_design = beamSimpleModels[iSimpleBeamIndex].LoadCombInternalForcesResults[lcomb.ID][m.ID].InternalForces.ToArray();
                        sBucklingLengthFactors_design = new designBucklingLengthFactors[iNumberOfDesignSections];
                        sMomentValuesforCb_design = new designMomentValuesForCb[iNumberOfDesignSections];

                        for (int j = 0; j < fx_positions.Length; j++)
                        {
                            designBucklingLengthFactors sBucklingLengthFactors_temp = new designBucklingLengthFactors();
                            designMomentValuesForCb sMomentValuesforCb_temp = new designMomentValuesForCb();
                            SetMomentValuesforCb_design_And_BucklingFactors_SimpleBeamSegment(fx_positions[j], iSimpleBeamIndex, lcomb.ID, m, ref sBucklingLengthFactors_temp, ref sMomentValuesforCb_temp);

                            sBucklingLengthFactors_design[j] = sBucklingLengthFactors_temp;
                            sMomentValuesforCb_design[j] = sMomentValuesforCb_temp;
                        }
                    }
                    else // Single Member or Frame Member (only LC calculated) - vysledky pocitane pre load cases
                    {
                        CMemberResultsManager.SetMemberInternalForcesInLoadCombination(MUseCRSCGeometricalAxes, m, lcomb, MemberInternalForcesInLoadCases, iNumberOfDesignSections, out sBucklingLengthFactors_design, out sMomentValuesforCb_design, out sBIF_x_design);
                    }

                    ValidateAndSetMomentValuesforCbAbsoluteValue(ref sMomentValuesforCb_design);

                    if (DeterminateCombinationResultsByFEMSolver)
                    {
                        // BFENet ma vracia vysledky pre ohybove momenty s opacnym znamienkom ako je nasa znamienkova dohoda
                        // Preto hodnoty momentov prenasobime
                        float fInternalForceSignFactor = -1; // TODO 191 - TO Ondrej Vnutorne sily z BFENet maju opacne znamienko, takze ich potrebujeme zmenit, alebo musime zaviest ine vykreslovanie pre momenty a ine pre sily

                        for (int i = 0; i < sBIF_x_design.Length; i++)
                        {
                            sBIF_x_design[i].fT *= fInternalForceSignFactor;

                            if (MUseCRSCGeometricalAxes)
                            {
                                sBIF_x_design[i].fM_yy *= fInternalForceSignFactor;
                                sBIF_x_design[i].fM_zz *= fInternalForceSignFactor;
                            }
                            else
                            {
                                sBIF_x_design[i].fM_yu *= fInternalForceSignFactor;
                                sBIF_x_design[i].fM_zv *= fInternalForceSignFactor;
                            }
                        }
                    }

                    // Add results
                    if (sBIF_x_design != null) MemberInternalForcesInLoadCombinations.Add(new CMemberInternalForcesInLoadCombinations(m, lcomb, sBIF_x_design, sMomentValuesforCb_design, sBucklingLengthFactors_design));
                }
            }
        }

        public void Calculate_Deflections_LoadCombinations(CMember m)
        {
            // Calculate Deflections For Load Combinations
            if (!m.BIsGenerated) return;

            if (m.BIsSelectedForIFCalculation) // Only structural members (not auxiliary members or members with deactivated calculation of internal forces)
            {
                for (int i = 0; i < iNumberOfDesignSections; i++)
                    fx_positions[i] = ((float)i / (float)iNumberOfDesignSegments) * m.FLength; // Int must be converted to the float to get decimal numbers

                foreach (CLoadCombination lcomb in Model.m_arrLoadCombs)
                {
                    if (lcomb.eLComType == ELSType.eLS_SLS) // Postacuje zatial pocitat len pre kombinacie SLS, zmazat pokial bude potrebne pracovat v posudeni kombinacii typu ULS s priehybom (napriklad urcenie vzpernej dlzky ramu podla horizontalnej vychylky)
                    {
                        // Member basic deflections
                        basicDeflections[] sBDeflection_x_design = new basicDeflections[iNumberOfDesignSections];

                        // Frame member - vysledky pocitane pre load combinations
                        if (DeterminateCombinationResultsByFEMSolver && (m.EMemberType == EMemberType_FS.eMC || m.EMemberType == EMemberType_FS.eMR || m.EMemberType == EMemberType_FS.eEC || m.EMemberType == EMemberType_FS.eER))
                        {
                            int iFrameIndex = CModelHelper.GetFrameIndexForMember(m, frameModels);  //podla ID pruta treba identifikovat do ktoreho ramu patri

                            sBDeflection_x_design = frameModels[iFrameIndex].LoadCombInternalForcesResults[lcomb.ID][m.ID].Deflections.ToArray();
                        }
                        else if (DeterminateCombinationResultsByFEMSolver) // Single Beam Members - vysledky pocitane v BFENet pre Load Combinations
                        {
                            int iSimpleBeamIndex = CModelHelper.GetSimpleBeamIndexForMember(m, beamSimpleModels);  //podla ID pruta treba identifikovat do ktoreho simple beam modelu patri

                            sBDeflection_x_design = beamSimpleModels[iSimpleBeamIndex].LoadCombInternalForcesResults[lcomb.ID][m.ID].Deflections.ToArray();
                        }
                        else // Single Member or Frame Member (only LC calculated) - vysledky pocitane pre load cases
                        {
                            CMemberResultsManager.SetMemberDeflectionsInLoadCombination(MUseCRSCGeometricalAxes, m, lcomb, MemberDeflectionsInLoadCases, iNumberOfDesignSections, out sBDeflection_x_design);
                        }

                        // Add results
                        if (sBDeflection_x_design != null) MemberDeflectionsInLoadCombinations.Add(new CMemberDeflectionsInLoadCombinations(m, lcomb, sBDeflection_x_design));
                    }
                }
            }

        }

        //public void Calculate_InternalForcesAndDeflections_LoadCases()
        //{
        //    designMomentValuesForCb[] sMomentValuesforCb = null;
        //    basicInternalForces[] sBIF_x = null;
        //    basicDeflections[] sBDeflections_x = null;
        //    designBucklingLengthFactors[] sBucklingLengthFactors = null;
        //    SimpleBeamCalculation calcModel = new SimpleBeamCalculation();

        //    int count = 0;
        //    SolverWindow.SetMemberDesignLoadCase();
        //    SolverWindow.SetMemberDesignLoadCaseProgress(count, membersIFCalcCount);
        //    // Calculate Internal Forces For Load Cases

        //    // Pre load cases nebudem urcovat MomentValuesforCb, pretoze neposudzujeme samostatne load case a pre kombinacie je to nepouzitelne, pretoze hodnoty Mmax mozu byt pre kazdy load case na inej casti segmentu, takze sa to neda pre segment
        //    // superponovat, ale je potrebne urcit maximum Mmax pre segment z priebehu vnutornych sil v kombinacii

        //    foreach (CMember m in Model.m_arrMembers)
        //    {
        //        if (!m.BIsGenerated) continue;
        //        if (!m.BIsSelectedForIFCalculation) continue; // Only structural members (not auxiliary members or members with deactivated calculation of internal forces)

        //        SolverWindow.SetMemberDesignLoadCaseProgress(++count, membersIFCalcCount);
        //        if (!DeterminateCombinationResultsByFEMSolver)
        //        {
        //            for (int i = 0; i < iNumberOfDesignSections; i++)
        //                fx_positions[i] = ((float)i / (float)iNumberOfDesignSegments) * m.FLength; // Int must be converted to the float to get decimal numbers

        //            foreach (CLoadCase lc in Model.m_arrLoadCases)
        //            {
        //                // Frame member
        //                if (m.EMemberType == EMemberType_FS.eMC || m.EMemberType == EMemberType_FS.eMR ||
        //                    m.EMemberType == EMemberType_FS.eEC || m.EMemberType == EMemberType_FS.eER)
        //                {
        //                    // BEFENet - calculate load cases only

        //                    // Set indices to search in results
        //                    int iFrameIndex = CModelHelper.GetFrameIndexForMember(m, frameModels);  //podla ID pruta treba identifikovat do ktoreho ramu patri

        //                    // Calculate internal forces and deflections in load cases
        //                    // Pocitame vsetko pretoze pre urcenie priehybu v SLS potrebujeme Ieff urcene z vysledkov posudenia pre vnutorne sily v SLS

        //                    sBucklingLengthFactors = new designBucklingLengthFactors[iNumberOfDesignSections];
        //                    //sMomentValuesforCb = new designMomentValuesForCb[iNumberOfDesignSections];

        //                    for (int j = 0; j < fx_positions.Length; j++)
        //                    {
        //                        designBucklingLengthFactors sBucklingLengthFactors_temp = new designBucklingLengthFactors();
        //                        //designMomentValuesForCb sMomentValuesforCb_temp = new designMomentValuesForCb();
        //                        //SetMomentValuesforCb_design_And_BucklingFactors_Frame(fx_positions[j], iFrameIndex, lc.ID, m, ref sBucklingLengthFactors_temp/*, ref sMomentValuesforCb_temp*/);
        //                        SetBucklingFactors_Frame(fx_positions[j], iFrameIndex, lc.ID, m, ref sBucklingLengthFactors_temp);

        //                        sBucklingLengthFactors[j] = sBucklingLengthFactors_temp;
        //                        //sMomentValuesforCb[j] = sMomentValuesforCb_temp;
        //                    }

        //                    // ULS - internal forces
        //                    sBIF_x = frameModels[iFrameIndex].LoadCombInternalForcesResults[lc.ID][m.ID].InternalForces.ToArray();

        //                    // SLS - deflections
        //                    sBDeflections_x = frameModels[iFrameIndex].LoadCombInternalForcesResults[lc.ID][m.ID].Deflections.ToArray();
        //                }
        //                else // Single member
        //                {
        //                    if (UseFEMSolverCalculationForSimpleBeam)
        //                    {
        //                        // BEFENet - calculate load cases only

        //                        // Set indices to search in results
        //                        int iSimpleBeamIndex = CModelHelper.GetSimpleBeamIndexForMember(m, beamSimpleModels);  //podla ID pruta treba identifikovat do ktoreho simple beam modelu patri

        //                        // Calculate internal forces and deflections in load cases
        //                        // Pocitame vsetko pretoze pre urcenie priehybu v SLS potrebujeme Ieff urcene z vysledkov posudenia pre vnutorne sily v SLS
        //                        sBucklingLengthFactors = new designBucklingLengthFactors[iNumberOfDesignSections];
        //                        sMomentValuesforCb = new designMomentValuesForCb[iNumberOfDesignSections];

        //                        for (int j = 0; j < fx_positions.Length; j++)
        //                        {
        //                            designBucklingLengthFactors sBucklingLengthFactors_temp = new designBucklingLengthFactors();
        //                            //designMomentValuesForCb sMomentValuesforCb_temp = new designMomentValuesForCb();
        //                            //SetMomentValuesforCb_design_And_BucklingFactors_SimpleBeamSegment(fx_positions[j], iSimpleBeamIndex, lc.ID, m, ref sBucklingLengthFactors_temp/*, ref sMomentValuesforCb_temp*/);
        //                            SetBucklingFactors_SimpleBeamSegment(fx_positions[j], iSimpleBeamIndex, lc.ID, m, ref sBucklingLengthFactors_temp);

        //                            sBucklingLengthFactors[j] = sBucklingLengthFactors_temp;
        //                            //sMomentValuesforCb[j] = sMomentValuesforCb_temp;
        //                        }

        //                        // ULS - internal forces
        //                        sBIF_x = beamSimpleModels[iSimpleBeamIndex].LoadCombInternalForcesResults[lc.ID][m.ID].InternalForces.ToArray();

        //                        // SLS - deflections
        //                        sBDeflections_x = beamSimpleModels[iSimpleBeamIndex].LoadCombInternalForcesResults[lc.ID][m.ID].Deflections.ToArray();
        //                    }
        //                    else
        //                    {
        //                        // Calculate internal forces and deflections in load cases
        //                        // Pocitame vsetko pretoze pre urcenie priehybu v SLS potrebujeme Ieff urcene z vysledkov posudenia pre vnutorne sily v SLS

        //                        // ULS - internal forces
        //                        calcModel.CalculateInternalForcesOnSimpleBeam_PFD(MUseCRSCGeometricalAxes, iNumberOfDesignSections, fx_positions, m, lc, out sBIF_x, out sBucklingLengthFactors/*, out sMomentValuesforCb*/);

        //                        // SLS - deflections
        //                        calcModel.CalculateDeflectionsOnSimpleBeam_PFD(MUseCRSCGeometricalAxes, iNumberOfDesignSections, fx_positions, m, lc, out sBDeflections_x);
        //                    }
        //                }

        //                // Add results
        //                if (sBIF_x != null) MemberInternalForcesInLoadCases.Add(new CMemberInternalForcesInLoadCases(m, lc, sBIF_x, /*sMomentValuesforCb,*/ sBucklingLengthFactors));
        //                if (sBDeflections_x != null) MemberDeflectionsInLoadCases.Add(new CMemberDeflectionsInLoadCases(m, lc, sBDeflections_x));

        //            } //end foreach load case
        //        }
        //        SolverWindow.Progress += step;
        //        SolverWindow.UpdateProgress();
        //    }
        //}

        //public void Calculate_InternalForces_LoadCombinations()
        //{
        //    // Calculate Internal Forces For Load Combinations

        //    foreach (CMember m in Model.m_arrMembers)
        //    {
        //        if (!m.BIsGenerated) continue;

        //        if (m.BIsSelectedForIFCalculation) // Only structural members (not auxiliary members or members with deactivated calculation of internal forces)
        //        {
        //            for (int i = 0; i < iNumberOfDesignSections; i++)
        //                fx_positions[i] = ((float)i / (float)iNumberOfDesignSegments) * m.FLength; // Int must be converted to the float to get decimal numbers

        //            // Note:  Kvoli urceniu Ieff pre posudenie priehybu budeme pravdepodobne potrebovat aj vnuntorne sily pre SLS, to znamena ze sa bude pocitat pre vsetky kombinacie
        //            foreach (CLoadCombination lcomb in Model.m_arrLoadCombs)
        //            {
        //                // Member basic internal forces
        //                designBucklingLengthFactors[] sBucklingLengthFactors_design = new designBucklingLengthFactors[iNumberOfDesignSections];
        //                designMomentValuesForCb[] sMomentValuesforCb_design = new designMomentValuesForCb[iNumberOfDesignSections];
        //                basicInternalForces[] sBIF_x_design = new basicInternalForces[iNumberOfDesignSections];

        //                // Frame member - vysledky pocitane pre load combinations
        //                if (DeterminateCombinationResultsByFEMSolver && (m.EMemberType == EMemberType_FS.eMC || m.EMemberType == EMemberType_FS.eMR || m.EMemberType == EMemberType_FS.eEC || m.EMemberType == EMemberType_FS.eER))
        //                {
        //                    int iFrameIndex = CModelHelper.GetFrameIndexForMember(m, frameModels);  //podla ID pruta treba identifikovat do ktoreho ramu patri
        //                    sBIF_x_design = frameModels[iFrameIndex].LoadCombInternalForcesResults[lcomb.ID][m.ID].InternalForces.ToArray();
        //                    sBucklingLengthFactors_design = new designBucklingLengthFactors[iNumberOfDesignSections];
        //                    sMomentValuesforCb_design = new designMomentValuesForCb[iNumberOfDesignSections];

        //                    for (int j = 0; j < fx_positions.Length; j++)
        //                    {
        //                        designBucklingLengthFactors sBucklingLengthFactors_temp = new designBucklingLengthFactors();
        //                        designMomentValuesForCb sMomentValuesforCb_temp = new designMomentValuesForCb();
        //                        SetMomentValuesforCb_design_And_BucklingFactors_Frame(fx_positions[j], iFrameIndex, lcomb.ID, m, ref sBucklingLengthFactors_temp, ref sMomentValuesforCb_temp);

        //                        sBucklingLengthFactors_design[j] = sBucklingLengthFactors_temp;
        //                        sMomentValuesforCb_design[j] = sMomentValuesforCb_temp;
        //                    }
        //                }
        //                else if (DeterminateCombinationResultsByFEMSolver) // Single Beam Members - vysledky pocitane v BFENet pre Load Combinations
        //                {
        //                    int iSimpleBeamIndex = CModelHelper.GetSimpleBeamIndexForMember(m, beamSimpleModels);  //podla ID pruta treba identifikovat do ktoreho simple beam modelu patri
        //                    sBIF_x_design = beamSimpleModels[iSimpleBeamIndex].LoadCombInternalForcesResults[lcomb.ID][m.ID].InternalForces.ToArray();
        //                    sBucklingLengthFactors_design = new designBucklingLengthFactors[iNumberOfDesignSections];
        //                    sMomentValuesforCb_design = new designMomentValuesForCb[iNumberOfDesignSections];

        //                    for (int j = 0; j < fx_positions.Length; j++)
        //                    {
        //                        designBucklingLengthFactors sBucklingLengthFactors_temp = new designBucklingLengthFactors();
        //                        designMomentValuesForCb sMomentValuesforCb_temp = new designMomentValuesForCb();
        //                        SetMomentValuesforCb_design_And_BucklingFactors_SimpleBeamSegment(fx_positions[j], iSimpleBeamIndex, lcomb.ID, m, ref sBucklingLengthFactors_temp, ref sMomentValuesforCb_temp);

        //                        sBucklingLengthFactors_design[j] = sBucklingLengthFactors_temp;
        //                        sMomentValuesforCb_design[j] = sMomentValuesforCb_temp;
        //                    }
        //                }
        //                else // Single Member or Frame Member (only LC calculated) - vysledky pocitane pre load cases
        //                {
        //                    CMemberResultsManager.SetMemberInternalForcesInLoadCombination(MUseCRSCGeometricalAxes, m, lcomb, MemberInternalForcesInLoadCases, iNumberOfDesignSections, out sBucklingLengthFactors_design, out sMomentValuesforCb_design, out sBIF_x_design);
        //                }

        //                ValidateAndSetMomentValuesforCbAbsoluteValue(ref sMomentValuesforCb_design);

        //                if (DeterminateCombinationResultsByFEMSolver)
        //                {
        //                    // BFENet ma vracia vysledky pre ohybove momenty s opacnym znamienkom ako je nasa znamienkova dohoda
        //                    // Preto hodnoty momentov prenasobime
        //                    float fInternalForceSignFactor = -1; // TODO 191 - TO Ondrej Vnutorne sily z BFENet maju opacne znamienko, takze ich potrebujeme zmenit, alebo musime zaviest ine vykreslovanie pre momenty a ine pre sily

        //                    for (int i = 0; i < sBIF_x_design.Length; i++)
        //                    {
        //                        sBIF_x_design[i].fT *= fInternalForceSignFactor;

        //                        if (MUseCRSCGeometricalAxes)
        //                        {
        //                            sBIF_x_design[i].fM_yy *= fInternalForceSignFactor;
        //                            sBIF_x_design[i].fM_zz *= fInternalForceSignFactor;
        //                        }
        //                        else
        //                        {
        //                            sBIF_x_design[i].fM_yu *= fInternalForceSignFactor;
        //                            sBIF_x_design[i].fM_zv *= fInternalForceSignFactor;
        //                        }
        //                    }
        //                }

        //                // Add results
        //                if (sBIF_x_design != null) MemberInternalForcesInLoadCombinations.Add(new CMemberInternalForcesInLoadCombinations(m, lcomb, sBIF_x_design, sMomentValuesforCb_design, sBucklingLengthFactors_design));
        //            }
        //        }
        //    }
        //}

        //public void Calculate_Deflections_LoadCombinations()
        //{
        //    // Calculate Deflections For Load Combinations

        //    foreach (CMember m in Model.m_arrMembers)
        //    {
        //        if (!m.BIsGenerated) continue;

        //        if (m.BIsSelectedForIFCalculation) // Only structural members (not auxiliary members or members with deactivated calculation of internal forces)
        //        {
        //            for (int i = 0; i < iNumberOfDesignSections; i++)
        //                fx_positions[i] = ((float)i / (float)iNumberOfDesignSegments) * m.FLength; // Int must be converted to the float to get decimal numbers

        //            foreach (CLoadCombination lcomb in Model.m_arrLoadCombs)
        //            {
        //                if (lcomb.eLComType == ELSType.eLS_SLS) // Postacuje zatial pocitat len pre kombinacie SLS, zmazat pokial bude potrebne pracovat v posudeni kombinacii typu ULS s priehybom (napriklad urcenie vzpernej dlzky ramu podla horizontalnej vychylky)
        //                {
        //                    // Member basic deflections
        //                    basicDeflections[] sBDeflection_x_design = new basicDeflections[iNumberOfDesignSections];

        //                    // Frame member - vysledky pocitane pre load combinations
        //                    if (DeterminateCombinationResultsByFEMSolver && (m.EMemberType == EMemberType_FS.eMC || m.EMemberType == EMemberType_FS.eMR || m.EMemberType == EMemberType_FS.eEC || m.EMemberType == EMemberType_FS.eER))
        //                    {
        //                        int iFrameIndex = CModelHelper.GetFrameIndexForMember(m, frameModels);  //podla ID pruta treba identifikovat do ktoreho ramu patri

        //                        sBDeflection_x_design = frameModels[iFrameIndex].LoadCombInternalForcesResults[lcomb.ID][m.ID].Deflections.ToArray();
        //                    }
        //                    else if (DeterminateCombinationResultsByFEMSolver) // Single Beam Members - vysledky pocitane v BFENet pre Load Combinations
        //                    {
        //                        int iSimpleBeamIndex = CModelHelper.GetSimpleBeamIndexForMember(m, beamSimpleModels);  //podla ID pruta treba identifikovat do ktoreho simple beam modelu patri

        //                        sBDeflection_x_design = beamSimpleModels[iSimpleBeamIndex].LoadCombInternalForcesResults[lcomb.ID][m.ID].Deflections.ToArray();
        //                    }
        //                    else // Single Member or Frame Member (only LC calculated) - vysledky pocitane pre load cases
        //                    {
        //                        CMemberResultsManager.SetMemberDeflectionsInLoadCombination(MUseCRSCGeometricalAxes, m, lcomb, MemberDeflectionsInLoadCases, iNumberOfDesignSections, out sBDeflection_x_design);
        //                    }

        //                    // Add results
        //                    if (sBDeflection_x_design != null) MemberDeflectionsInLoadCombinations.Add(new CMemberDeflectionsInLoadCombinations(m, lcomb, sBDeflection_x_design));
        //                }
        //            }
        //        }
        //    }
        //}

        public void Calculate_MemberDesign_LoadCombinations()
        {
            sDesignResults_ULSandSLS.sLimitStateType = "ULS and SLS";
            sDesignResults_ULS.sLimitStateType = "ULS";
            sDesignResults_SLS.sLimitStateType = "SLS";

            // Design of members

            int count = 0;
            SolverWindow.SetMemberDesignLoadCombination();
            SolverWindow.SetMemberDesignLoadCombinationProgress(count, membersDesignCalcCount);
            foreach (CMember m in Model.m_arrMembers)
            {
                if (!m.BIsGenerated) continue;
                if (!m.BIsSelectedForDesign) continue; // Only structural members (not auxiliary members or members with deactivated design)

                SolverWindow.SetMemberDesignLoadCombinationProgress(++count, membersDesignCalcCount);

                foreach (CLoadCombination lcomb in Model.m_arrLoadCombs)
                {
                    if (lcomb.eLComType == ELSType.eLS_ULS) // Combination Type - ULS
                    {
                        // Member design internal forces
                        designInternalForces[] sMemberDIF_x;

                        // Member Design
                        CMemberDesign memberDesignModel = new CMemberDesign();
                        // TODO - sBucklingLengthFactors_design  a sMomentValuesforCb_design nemaju byt priradene prutu ale segmentu pruta pre kazdy load case / load combination

                        // Set basic internal forces, buckling lengths and bending moments for determination of Cb for member and load combination
                        CMemberInternalForcesInLoadCombinations mInternal_forces_and_design_parameters = MemberInternalForcesInLoadCombinations.Find(i => i.Member.ID == m.ID && i.LoadCombination.ID == lcomb.ID);

                        // Design check procedure
                        memberDesignModel.SetDesignForcesAndMemberDesign_PFD(MUseCRSCGeometricalAxes,
                            iNumberOfDesignSections,
                            m,
                            mInternal_forces_and_design_parameters.InternalForces, // TO Ondrej - toto by sme asi mohli predavat cele ako jeden parameter mInternal_forces_and_design_parameters namiesto troch
                            mInternal_forces_and_design_parameters.BucklingLengthFactors,
                            mInternal_forces_and_design_parameters.BendingMomentValues,
                            out sMemberDIF_x);

                        // Add design check results to the list
                        MemberDesignResults_ULS.Add(new CMemberLoadCombinationRatio_ULS(m, lcomb, memberDesignModel.fMaximumDesignRatio, sMemberDIF_x[memberDesignModel.fMaximumDesignRatioLocationID], mInternal_forces_and_design_parameters.BucklingLengthFactors[memberDesignModel.fMaximumDesignRatioLocationID], mInternal_forces_and_design_parameters.BendingMomentValues[memberDesignModel.fMaximumDesignRatioLocationID]));

                        // Set maximum design ratio of whole structure
                        if (memberDesignModel.fMaximumDesignRatio > sDesignResults_ULSandSLS.fMaximumDesignRatioWholeStructure)
                        {
                            sDesignResults_ULSandSLS.fMaximumDesignRatioWholeStructure = memberDesignModel.fMaximumDesignRatio;
                            sDesignResults_ULSandSLS.GoverningLoadCombinationStructure = lcomb;
                            sDesignResults_ULSandSLS.MaximumDesignRatioWholeStructureMember = m;
                        }

                        if (memberDesignModel.fMaximumDesignRatio > sDesignResults_ULS.fMaximumDesignRatioWholeStructure)
                        {
                            sDesignResults_ULS.fMaximumDesignRatioWholeStructure = memberDesignModel.fMaximumDesignRatio;
                            sDesignResults_ULS.GoverningLoadCombinationStructure = lcomb;
                            sDesignResults_ULS.MaximumDesignRatioWholeStructureMember = m;
                        }

                        // Joint Design
                        CJointDesign jointDesignModel = new CJointDesign();

                        designInternalForces sjointStartDIF_x;
                        designInternalForces sjointEndDIF_x;
                        CConnectionJointTypes jointStart;
                        CConnectionJointTypes jointEnd;

                        jointDesignModel.SetDesignForcesAndJointDesign_PFD(iNumberOfDesignSections, MUseCRSCGeometricalAxes, Model, m, mInternal_forces_and_design_parameters.InternalForces, out jointStart, out jointEnd, out sjointStartDIF_x, out sjointEndDIF_x);

                        // Validation - Main member of joint must be defined
                        if (jointStart.m_MainMember == null)
                            throw new ArgumentNullException("Error" + "Joint No: " + jointStart.ID + " Main member is not defined.");
                        if (jointEnd.m_MainMember == null)
                            throw new ArgumentNullException("Error" + "Joint No: " + jointEnd.ID + " Main member is not defined.");

                        // Start Joint
                        JointDesignResults_ULS.Add(new CJointLoadCombinationRatio_ULS(m, jointStart, lcomb, jointDesignModel.fDesignRatio_Start, sjointStartDIF_x));

                        // End Joint
                        JointDesignResults_ULS.Add(new CJointLoadCombinationRatio_ULS(m, jointEnd, lcomb, jointDesignModel.fDesignRatio_End, sjointEndDIF_x));

                        // Output (for debugging - member results)
                        bool bDebugging = false; // Testovacie ucely
                        if (bDebugging)
                            System.Diagnostics.Trace.WriteLine("Member ID: " + m.ID + "\t | " +
                                              "Load Combination ID: " + lcomb.ID + "\t | " +
                                              "Design Ratio: " + Math.Round(memberDesignModel.fMaximumDesignRatio, 3).ToString() + "\n");

                        // Output (for debugging - member connection / joint results)
                        if (bDebugging)
                            System.Diagnostics.Trace.WriteLine("Member ID: " + m.ID + "\t | " +
                                              "Joint ID: " + jointStart.ID + "\t | " +
                                              "Load Combination ID: " + lcomb.ID + "\t | " +
                                              "Design Ratio: " + Math.Round(jointDesignModel.fDesignRatio_Start, 3).ToString() + "\n");

                        if (bDebugging)
                            System.Diagnostics.Trace.WriteLine("Member ID: " + m.ID + "\t | " +
                                              "Joint ID: " + jointEnd.ID + "\t | " +
                                              "Load Combination ID: " + lcomb.ID + "\t | " +
                                              "Design Ratio: " + Math.Round(jointDesignModel.fDesignRatio_End, 3).ToString() + "\n");

                        SetMaximumDesignRatioByComponentType(m, lcomb, memberDesignModel, ref sDesignResults_ULSandSLS);
                        SetMaximumDesignRatioByComponentType(m, lcomb, memberDesignModel, ref sDesignResults_ULS);
                    }
                    else // Combination Type - SLS (TODO - pre urcenie Ieff potrebujeme spocitat aj strength)
                    {
                        // Member design deflections
                        designDeflections[] sMemberDDeflection_x;

                        // Member Design
                        CMemberDesign memberDesignModel = new CMemberDesign();

                        // Set basic deflections for member and load combination
                        CMemberDeflectionsInLoadCombinations mDeflections = MemberDeflectionsInLoadCombinations.Find(i => i.Member.ID == m.ID && i.LoadCombination.ID == lcomb.ID);

                        int iDeflectionLimitDenominator_Fraction = 1; // int 25-1000
                        float fDeflectionLimit = 0f;

                        // Find group of current member (definition of member type)
                        CMemberGroup currentMemberTypeGroupOfMembers = Model.listOfModelMemberGroups.FirstOrDefault(gr => gr.MemberType_FS == m.EMemberType);

                        // To Mato - pozor ak su to dvere, okno, tak currentMemberTypeGroupOfMembers je null
                        // Ak je to aktualne, tak musime teda este doplnit funkcionalitu tak, aby sa pre pruty blokov tiez vytvorili skupiny

                        if (currentMemberTypeGroupOfMembers != null)
                        {
                            // Set deflection limit depending of member type and load combination type
                            if (lcomb.IsCombinationOfPermanentLoadCasesOnly())
                            {
                                iDeflectionLimitDenominator_Fraction = currentMemberTypeGroupOfMembers.DeflectionLimitFraction_Denominator_PermanentLoad;
                                fDeflectionLimit = currentMemberTypeGroupOfMembers.DeflectionLimit_PermanentLoad;
                            }
                            else
                            {
                                iDeflectionLimitDenominator_Fraction = currentMemberTypeGroupOfMembers.DeflectionLimitFraction_Denominator_Total;
                                fDeflectionLimit = currentMemberTypeGroupOfMembers.DeflectionLimit_Total;
                            }
                        }

                        // Design check procedure
                        memberDesignModel.SetDesignDeflections_PFD(MUseCRSCGeometricalAxes,
                            iNumberOfDesignSections,
                            m,
                            iDeflectionLimitDenominator_Fraction,
                            fDeflectionLimit,
                            mDeflections.Deflections,
                            out sMemberDDeflection_x);

                        // Add design check results to the list
                        MemberDesignResults_SLS.Add(new CMemberLoadCombinationRatio_SLS(m, lcomb, memberDesignModel.fMaximumDesignRatio, sMemberDDeflection_x[memberDesignModel.fMaximumDesignRatioLocationID]));

                        // Set maximum design ratio of whole structure
                        if (memberDesignModel.fMaximumDesignRatio > sDesignResults_ULSandSLS.fMaximumDesignRatioWholeStructure)
                        {
                            sDesignResults_ULSandSLS.fMaximumDesignRatioWholeStructure = memberDesignModel.fMaximumDesignRatio;
                            sDesignResults_ULSandSLS.GoverningLoadCombinationStructure = lcomb;
                            sDesignResults_ULSandSLS.MaximumDesignRatioWholeStructureMember = m;
                        }

                        if (memberDesignModel.fMaximumDesignRatio > sDesignResults_SLS.fMaximumDesignRatioWholeStructure)
                        {
                            sDesignResults_SLS.fMaximumDesignRatioWholeStructure = memberDesignModel.fMaximumDesignRatio;
                            sDesignResults_SLS.GoverningLoadCombinationStructure = lcomb;
                            sDesignResults_SLS.MaximumDesignRatioWholeStructureMember = m;
                        }

                        // Output (for debugging)
                        bool bDebugging = false; // Testovacie ucely
                        if (bDebugging)
                            System.Diagnostics.Trace.WriteLine("Member ID: " + m.ID + "\t | " +
                                              "Load Combination ID: " + lcomb.ID + "\t | " +
                                              "Design Ratio: " + Math.Round(memberDesignModel.fMaximumDesignRatio, 3).ToString());

                        SetMaximumDesignRatioByComponentType(m, lcomb, memberDesignModel, ref sDesignResults_ULSandSLS);
                        SetMaximumDesignRatioByComponentType(m, lcomb, memberDesignModel, ref sDesignResults_SLS);
                    }
                }

                SolverWindow.Progress += step;
                SolverWindow.UpdateProgress();
            }
        }

        private void SetMaximumDesignRatioByComponentType(CMember m, CLoadCombination lcomb, CMemberDesign memberDesignModel, ref sDesignResults s)
        {
            // Output - set maximum design ratio by component Type
            if (memberDesignModel.fMaximumDesignRatio > s.DesignResults[m.EMemberTypePosition].MaximumDesignRatio)
            {
                s.DesignResults[m.EMemberTypePosition].MaximumDesignRatio = memberDesignModel.fMaximumDesignRatio;
                s.DesignResults[m.EMemberTypePosition].MemberWithMaximumDesignRatio = m;
                s.DesignResults[m.EMemberTypePosition].GoverningLoadCombination = lcomb;
            }
        }

        private void SetDefaultBucklingFactors(CMember m, ref designBucklingLengthFactors sBucklingLengthFactors)
        {
            // TODO - faktory vzpernej dlzky mozu byt ine pre kazdy segment pruta a kazdy load case alebo load combination
            // TODO - vymysliet system ako s tym pracovat a priradzovat, je potrebne pouzit pri posudeni v mieste x na prute

            sBucklingLengthFactors.fBeta_x_FB_fl_ex = 1.0f;

            sBucklingLengthFactors.fBeta_y_FB_fl_ey = 1.0f;
            sBucklingLengthFactors.fBeta_z_TB_TFB_l_ez = 1.0f;
            sBucklingLengthFactors.fBeta_LTB_fl_LTB = 1.0f;

            if (m.EMemberType == EMemberType_FS.eMR || m.EMemberType == EMemberType_FS.eER)
            {
                sBucklingLengthFactors.fBeta_y_FB_fl_ey = 0.5f;
                sBucklingLengthFactors.fBeta_z_TB_TFB_l_ez = 0.5f;
                sBucklingLengthFactors.fBeta_LTB_fl_LTB = 0.5f;
            }

            if (m.LTBSegmentGroup != null) // Temporary
                sBucklingLengthFactors = m.LTBSegmentGroup[0].BucklingLengthFactors[0];
        }

        private void SetMomentValuesforCb_design_And_BucklingFactors_Frame(float fx, int iFrameIndex, int lcombID, CMember member, ref designBucklingLengthFactors bucklingLengthFactors, ref designMomentValuesForCb sMomentValuesforCb_design)
        {
            // Create load combination (FEM solver object)
            BriefFiniteElementNet.LoadCombination lcomb = new BriefFiniteElementNet.LoadCombination();
            lcomb = ConvertLoadCombinationtoBFENet(Model.m_arrLoadCombs[lcombID - 1]);

            int iMemberIndex_FM = frameModels[iFrameIndex].GetMemberIndexInFrame(member);

            float fSegmentStart_abs, fSegmentEnd_abs;
            GetSegmentStartAndEndFor_xLocation(fx, member, out fSegmentStart_abs, out fSegmentEnd_abs);
            GetSegmentBucklingFactors_xLocation(fx, member, lcombID, ref bucklingLengthFactors);
            sMomentValuesforCb_design = GetMomentValuesforCb_design_Segment(fSegmentStart_abs, fSegmentEnd_abs, lcomb,
            ((BriefFiniteElementNet.FrameElement2Node)(frameModels[iFrameIndex].BFEMNetModel.Elements[iMemberIndex_FM])));

            /*
            sMomentValuesforCb_design.fM_14 = frameModels[iFrameIndex].LoadCombInternalForcesResults[lcombID][member.ID].InternalForces[2].fM_yy;
            */
        }

        private void SetBucklingFactors_Frame(float fx, int iFrameIndex, int lcombID, CMember member, ref designBucklingLengthFactors bucklingLengthFactors)
        {
            // Create load combination (FEM solver object)
            BriefFiniteElementNet.LoadCombination lcomb = new BriefFiniteElementNet.LoadCombination();
            lcomb = ConvertLoadCombinationtoBFENet(Model.m_arrLoadCombs[lcombID - 1]);

            int iMemberIndex_FM = frameModels[iFrameIndex].GetMemberIndexInFrame(member);

            float fSegmentStart_abs, fSegmentEnd_abs;
            GetSegmentStartAndEndFor_xLocation(fx, member, out fSegmentStart_abs, out fSegmentEnd_abs);
            GetSegmentBucklingFactors_xLocation(fx, member, lcombID, ref bucklingLengthFactors);
        }

        private void SetMomentValuesforCb_design_And_BucklingFactors_SimpleBeamSegment(float fx, int iSimpleBeamIndex, int lcombID, CMember member, ref designBucklingLengthFactors bucklingLengthFactors, ref designMomentValuesForCb sMomentValuesforCb_design)
        {
            if (iSimpleBeamIndex < 0)
            {
                //To Mato - V Load Cases som zaskrtol druhe option a tu sa mi dostalo iSimpleBeamIndex = -1;
                return;
            }
            // Create load combination (FEM solver object)
            BriefFiniteElementNet.LoadCombination lcomb = new BriefFiniteElementNet.LoadCombination();
            lcomb = ConvertLoadCombinationtoBFENet(Model.m_arrLoadCombs[lcombID - 1]);

            float fSegmentStart_abs, fSegmentEnd_abs;
            GetSegmentStartAndEndFor_xLocation(fx, member, out fSegmentStart_abs, out fSegmentEnd_abs);
            GetSegmentBucklingFactors_xLocation(fx, member, lcombID, ref bucklingLengthFactors);
            sMomentValuesforCb_design = GetMomentValuesforCb_design_Segment(fSegmentStart_abs, fSegmentEnd_abs, lcomb,
            ((BriefFiniteElementNet.FrameElement2Node)(beamSimpleModels[iSimpleBeamIndex].BFEMNetModel.Elements[0])));

            /*
            sMomentValuesforCb_design.fM_14 = beamSimpleModels[iSimpleBeamIndex].LoadCombInternalForcesResults[lcombID][member.ID].InternalForces[2].fM_yy;
            */
        }

        private void SetBucklingFactors_SimpleBeamSegment(float fx, int iSimpleBeamIndex, int lcombID, CMember member, ref designBucklingLengthFactors bucklingLengthFactors)
        {
            if (iSimpleBeamIndex < 0)
            {
                //To Mato - V Load Cases som zaskrtol druhe option a tu sa mi dostalo iSimpleBeamIndex = -1;
                return;
            }
            // Create load combination (FEM solver object)
            BriefFiniteElementNet.LoadCombination lcomb = new BriefFiniteElementNet.LoadCombination();
            lcomb = ConvertLoadCombinationtoBFENet(Model.m_arrLoadCombs[lcombID - 1]);

            float fSegmentStart_abs, fSegmentEnd_abs;
            GetSegmentStartAndEndFor_xLocation(fx, member, out fSegmentStart_abs, out fSegmentEnd_abs);
            GetSegmentBucklingFactors_xLocation(fx, member, lcombID, ref bucklingLengthFactors);
        }

        private designMomentValuesForCb GetMomentValuesforCb_design_Segment(float fSegmentStart_abs, float fSegmentEnd_abs, BriefFiniteElementNet.LoadCombination lcomb, BriefFiniteElementNet.FrameElement2Node memberBFENet)
        {
            float fSegmentLength = fSegmentEnd_abs - fSegmentStart_abs;
            BriefFiniteElementNet.Force f14 = memberBFENet.GetInternalForceAt(fSegmentStart_abs + 0.25 * fSegmentLength, lcomb);
            BriefFiniteElementNet.Force f24 = memberBFENet.GetInternalForceAt(fSegmentStart_abs + 0.50 * fSegmentLength, lcomb);
            BriefFiniteElementNet.Force f34 = memberBFENet.GetInternalForceAt(fSegmentStart_abs + 0.75 * fSegmentLength, lcomb);

            designMomentValuesForCb sMomentValuesforCb_design_segment;
            sMomentValuesforCb_design_segment.fM_14 = (float)f14.My;
            sMomentValuesforCb_design_segment.fM_24 = (float)f24.My;
            sMomentValuesforCb_design_segment.fM_34 = (float)f34.My;
            sMomentValuesforCb_design_segment.fM_max = 0;

            for (int i = 0; i < iNumberOfDesignSections; i++)
            {
                float fx = fSegmentStart_abs + ((float)i / (float)iNumberOfDesignSegments) * fSegmentLength;
                BriefFiniteElementNet.Force f = memberBFENet.GetInternalForceAt(fx, lcomb);

                if (Math.Abs(f.My) > Math.Abs(sMomentValuesforCb_design_segment.fM_max))
                    sMomentValuesforCb_design_segment.fM_max = (float)f.My;
            }

            // Check that M_max is more or equal to the maximum from (M_14, M_24, M_34) - symbols M_3, M_4, M_5 used in exception message
            if (Math.Abs(sMomentValuesforCb_design_segment.fM_max) < MathF.Max(Math.Abs(sMomentValuesforCb_design_segment.fM_14), Math.Abs(sMomentValuesforCb_design_segment.fM_24), Math.Abs(sMomentValuesforCb_design_segment.fM_34)))
            {
                throw new Exception("Maximum value of bending moment doesn't correspond with values of bending moment at segment M₃, M₄, M₅.");
            }

            return sMomentValuesforCb_design_segment;
        }

        private void ValidateAndSetMomentValuesforCbAbsoluteValue(ref designMomentValuesForCb[] sMomentValuesforCb_design)
        {
            if (sMomentValuesforCb_design == null || sMomentValuesforCb_design.Length == 0)
                return;

            for (int i = 0; i < sMomentValuesforCb_design.Length; i++)
            {
                // Validate
                if (sMomentValuesforCb_design[i].fM_14 == float.NaN ||
                   sMomentValuesforCb_design[i].fM_24 == float.NaN ||
                   sMomentValuesforCb_design[i].fM_34 == float.NaN ||
                   sMomentValuesforCb_design[i].fM_max == float.NaN)
                {
                    throw new ArgumentNullException("Invalid value of bending moment.");
                }

                /*
                Mmax. = absolute value of the maximum moment in the unbraced segment
                M3 = absolute value of the moment at quarter point of the unbraced segment
                M4 = absolute value of the moment at mid-point of the unbraced segment
                M5 = absolute value of the moment at three-quarter point of the unbraced segment
                */

                if (sMomentValuesforCb_design[i].fM_14 < 0)
                    sMomentValuesforCb_design[i].fM_14 *= -1f;

                if (sMomentValuesforCb_design[i].fM_24 < 0)
                    sMomentValuesforCb_design[i].fM_24 *= -1f;

                if (sMomentValuesforCb_design[i].fM_34 < 0)
                    sMomentValuesforCb_design[i].fM_34 *= -1f;

                if (sMomentValuesforCb_design[i].fM_max < 0)
                    sMomentValuesforCb_design[i].fM_max *= -1f;

                // Check that M_max is more or equal to the maximum from (M_14, M_24, M_34) - symbols M_3, M_4, M_5 used in exception message
                if (sMomentValuesforCb_design[i].fM_max < MathF.Max(sMomentValuesforCb_design[i].fM_14, sMomentValuesforCb_design[i].fM_24, sMomentValuesforCb_design[i].fM_34))
                {
                    throw new Exception("Maximum value of bending moment doesn't correspond with values of bending moment at segment M₃, M₄, M₅.");
                }
            }
        }

        private BriefFiniteElementNet.LoadCombination ConvertLoadCombinationtoBFENet(CLoadCombination loadcomb_input)
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
                        else if (member.LTBSegmentGroup[i].BucklingLengthFactors.Count == 1) // Defined only once
                        {
                            bucklingLengthFactors = member.LTBSegmentGroup[i].BucklingLengthFactors[0];

                        }
                        else // if(bucklingLengthFactors.Count > 1) // Different values for load combinations
                            bucklingLengthFactors = member.LTBSegmentGroup[i].BucklingLengthFactors[lcombID - 1];
                    }
                }
            }
        }

        private string GetTextForResultsMessageBox(sDesignResults s)
        {
            StringBuilder sb = new StringBuilder();

            if (s.fMaximumDesignRatioWholeStructure > 0)
            {
                sb.Append("Calculation Results \n");
                sb.Append($"Limit State Type: {s.sLimitStateType}\n");

                if (s.MaximumDesignRatioWholeStructureMember != null)
                {
                    sb.Append("Maximum design ratio \n");
                    sb.Append($"Member ID: {s.MaximumDesignRatioWholeStructureMember.ID.ToString()}\t");
                    if (s.GoverningLoadCombinationStructure != null)
                    {
                        sb.Append($"Load Combination ID: {s.GoverningLoadCombinationStructure.ID}\t");
                        sb.Append($"Design Ratio η: {Math.Round(s.fMaximumDesignRatioWholeStructure, 3)}\n\n\n");
                    }
                }

                DesignResultItem item = s.DesignResults[EMemberType_FS_Position.MainColumn];
                if (item.MemberWithMaximumDesignRatio != null)
                {
                    sb.Append("Maximum design ratio - main columns\n");
                    sb.Append($"Member ID: {item.MemberWithMaximumDesignRatio.ID.ToString()}\t");
                    if (item.GoverningLoadCombination != null)
                    {
                        sb.Append($"Load Combination ID: {item.GoverningLoadCombination.ID}\t");
                        sb.Append($"Design Ratio η: {Math.Round(item.MaximumDesignRatio, 3).ToString()}\n\n");
                    }
                }

                item = s.DesignResults[EMemberType_FS_Position.MainRafter];
                if (item.MemberWithMaximumDesignRatio != null)
                {
                    sb.Append("Maximum design ratio - main rafters\n");
                    sb.Append($"Member ID: {item.MemberWithMaximumDesignRatio.ID.ToString()}\t");
                    if (item.GoverningLoadCombination != null)
                    {
                        sb.Append($"Load Combination ID: {item.GoverningLoadCombination.ID}\t");
                        sb.Append($"Design Ratio η: {Math.Round(item.MaximumDesignRatio, 3).ToString()}\n\n");
                    }
                }

                item = s.DesignResults[EMemberType_FS_Position.EdgeColumn];
                if (item.MemberWithMaximumDesignRatio != null)
                {
                    sb.Append("Maximum design ratio - edge columns\n");
                    sb.Append($"Member ID: {item.MemberWithMaximumDesignRatio.ID.ToString()}\t");
                    if (item.GoverningLoadCombination != null)
                    {
                        sb.Append($"Load Combination ID: {item.GoverningLoadCombination.ID}\t");
                        sb.Append($"Design Ratio η: {Math.Round(item.MaximumDesignRatio, 3).ToString()}\n\n");
                    }
                }

                item = s.DesignResults[EMemberType_FS_Position.EdgeRafter];
                if (item.MemberWithMaximumDesignRatio != null)
                {
                    sb.Append("Maximum design ratio - edge rafters\n");
                    sb.Append($"Member ID: {item.MemberWithMaximumDesignRatio.ID.ToString()}\t");
                    if (item.GoverningLoadCombination != null)
                    {
                        sb.Append($"Load Combination ID: {item.GoverningLoadCombination.ID}\t");
                        sb.Append($"Design Ratio η: {Math.Round(item.MaximumDesignRatio, 3).ToString()}\n\n");
                    }
                }

                item = s.DesignResults[EMemberType_FS_Position.Girt];
                if (item.MemberWithMaximumDesignRatio != null)
                {
                    sb.Append("Maximum design ratio - girts\n");
                    sb.Append($"Member ID: {item.MemberWithMaximumDesignRatio.ID.ToString()}\t");
                    if (item.GoverningLoadCombination != null)
                    {
                        sb.Append($"Load Combination ID: {item.GoverningLoadCombination.ID}\t");
                        sb.Append($"Design Ratio η: {Math.Round(item.MaximumDesignRatio, 3).ToString()}\n\n");
                    }
                }

                item = s.DesignResults[EMemberType_FS_Position.Purlin];
                if (item.MemberWithMaximumDesignRatio != null)
                {
                    sb.Append("Maximum design ratio - purlins\n");
                    sb.Append($"Member ID: {item.MemberWithMaximumDesignRatio.ID.ToString()}\t");
                    if (item.GoverningLoadCombination != null)
                    {
                        sb.Append($"Load Combination ID: {item.GoverningLoadCombination.ID}\t");
                        sb.Append($"Design Ratio η: {Math.Round(item.MaximumDesignRatio, 3).ToString()}\n\n");
                    }
                }

                item = s.DesignResults[EMemberType_FS_Position.EdgePurlin];
                if (item.MemberWithMaximumDesignRatio != null)
                {
                    sb.Append("Maximum design ratio - edge purlins\n");
                    sb.Append($"Member ID: {item.MemberWithMaximumDesignRatio.ID.ToString()}\t");
                    if (item.GoverningLoadCombination != null)
                    {
                        sb.Append($"Load Combination ID: {item.GoverningLoadCombination.ID}\t");
                        sb.Append($"Design Ratio η: {Math.Round(item.MaximumDesignRatio, 3).ToString()}\n\n");
                    }
                }

                item = s.DesignResults[EMemberType_FS_Position.ColumnFrontSide];
                if (item.MemberWithMaximumDesignRatio != null)
                {
                    sb.Append("Maximum design ratio - front columns\n");
                    sb.Append($"Member ID: {item.MemberWithMaximumDesignRatio.ID.ToString()}\t");
                    if (item.GoverningLoadCombination != null)
                    {
                        sb.Append($"Load Combination ID: {item.GoverningLoadCombination.ID}\t");
                        sb.Append($"Design Ratio η: {Math.Round(item.MaximumDesignRatio, 3).ToString()}\n\n");
                    }
                }

                item = s.DesignResults[EMemberType_FS_Position.ColumnBackSide];
                if (item.MemberWithMaximumDesignRatio != null)
                {
                    sb.Append("Maximum design ratio - back columns\n");
                    sb.Append($"Member ID: {item.MemberWithMaximumDesignRatio.ID.ToString()}\t");
                    if (item.GoverningLoadCombination != null)
                    {
                        sb.Append($"Load Combination ID: {item.GoverningLoadCombination.ID}\t");
                        sb.Append($"Design Ratio η: {Math.Round(item.MaximumDesignRatio, 3).ToString()}\n\n");
                    }
                }

                item = s.DesignResults[EMemberType_FS_Position.GirtFrontSide];
                if (item.MemberWithMaximumDesignRatio != null)
                {
                    sb.Append("Maximum design ratio - front girts\n");
                    sb.Append($"Member ID: {item.MemberWithMaximumDesignRatio.ID.ToString()}\t");
                    if (item.GoverningLoadCombination != null)
                    {
                        sb.Append($"Load Combination ID: {item.GoverningLoadCombination.ID}\t");
                        sb.Append($"Design Ratio η: {Math.Round(item.MaximumDesignRatio, 3).ToString()}\n\n");
                    }
                }
                item = s.DesignResults[EMemberType_FS_Position.GirtBackSide];
                if (item.MemberWithMaximumDesignRatio != null)
                {
                    sb.Append("Maximum design ratio - back girts\n");
                    sb.Append($"Member ID: {item.MemberWithMaximumDesignRatio.ID.ToString()}\t");
                    if (item.GoverningLoadCombination != null)
                    {
                        sb.Append($"Load Combination ID: {item.GoverningLoadCombination.ID}\t");
                        sb.Append($"Design Ratio η: {Math.Round(item.MaximumDesignRatio, 3).ToString()}\n\n");
                    }
                }
                
            }

            return sb.ToString();
        }

        private void ShowResultsInMessageBox(string txtULSandSLS, string txtULS, string txtSLS)
        {
            SolverWindow.ShowMessageBox(txtULSandSLS + txtULS + txtSLS);
        }
    }
}
