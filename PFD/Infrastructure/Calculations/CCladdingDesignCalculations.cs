using BaseClasses;
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
    public class CCladdingDesignCalculations
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
        private bool DeterminateMemberLocalDisplacementsForULS;

        private int membersIFCalcCount;
        private int membersDesignCalcCount;

        public List<CMemberInternalForcesInLoadCases> MemberInternalForcesInLoadCases;
        public List<CMemberDeflectionsInLoadCases> MemberDeflectionsInLoadCases;

        public List<CMemberInternalForcesInLoadCombinations> MemberInternalForcesInLoadCombinations;
        public List<CMemberDeflectionsInLoadCombinations> MemberDeflectionsInLoadCombinations;

        public List<CMemberLoadCombinationRatio_ULS> MemberDesignResults_ULS;
        public List<CMemberLoadCombinationRatio_SLS> MemberDesignResults_SLS;

        /////////////////////////////////////////////////////////////////////////////////////////////////////////
        public sDesignResults sDesignResults_ULSandSLS = new sDesignResults();
        public sDesignResults sDesignResults_ULS = new sDesignResults();
        public sDesignResults sDesignResults_SLS = new sDesignResults();

        public CCladdingDesignCalculations(Solver solverWindow,
            CModel_PFD_01_GR model)
        {
            SolverWindow = solverWindow;
            Model = model;

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
        }

        public void CalculateAll()
        {
            Calculate_InternalForces();
            Calculate_MemberDesign_LoadCombinations();

            SolverWindow.Progress = 100;
            SolverWindow.UpdateProgress();
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

                    // Calculate internal forces and deflections in load cases
                    // Pocitame vsetko pretoze pre urcenie priehybu v SLS potrebujeme Ieff urcene z vysledkov posudenia pre vnutorne sily v SLS

                    // ULS - internal forces
                    calcModel.CalculateInternalForcesOnSimpleBeam_PFD(MUseCRSCGeometricalAxes, iNumberOfDesignSections, fx_positions, m, lc, out sBIF_x, out sBucklingLengthFactors/*, out sMomentValuesforCb*/);

                    // SLS - deflections
                    calcModel.CalculateDeflectionsOnSimpleBeam_PFD(MUseCRSCGeometricalAxes, iNumberOfDesignSections, fx_positions, m, lc, out sBDeflections_x);

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

                    // Single Member

                    CMemberResultsManager.SetMemberInternalForcesInLoadCombination(true, m, lcomb, MemberInternalForcesInLoadCases, iNumberOfDesignSections, out sBucklingLengthFactors_design, out sMomentValuesforCb_design, out sBIF_x_design);

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
                    if (lcomb.eLComType == ELSType.eLS_SLS || DeterminateMemberLocalDisplacementsForULS) // Pocitat defaultne len pre kombinacie SLS, GUI option pre vypocet aj pre ULS combinations
                    {
                        // Member basic deflections
                        basicDeflections[] sBDeflection_x_design = new basicDeflections[iNumberOfDesignSections];

                        CMemberResultsManager.SetMemberDeflectionsInLoadCombination(true, m, lcomb, MemberDeflectionsInLoadCases, iNumberOfDesignSections, out sBDeflection_x_design);

                        // Add results
                        if (sBDeflection_x_design != null) MemberDeflectionsInLoadCombinations.Add(new CMemberDeflectionsInLoadCombinations(m, lcomb, sBDeflection_x_design));
                    }
                }
            }
        }

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
                        memberDesignModel.SetDesignForcesAndMemberDesign_PFD(true,
                            false,
                            false,
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

                        //SetMaximumDesignRatioByComponentType(m, lcomb, memberDesignModel, ref sDesignResults_ULSandSLS);
                        //SetMaximumDesignRatioByComponentType(m, lcomb, memberDesignModel, ref sDesignResults_ULS);
                    }
                    else // Combination Type - SLS (TODO - pre urcenie Ieff potrebujeme spocitat aj strength)
                    {
                        // Member design deflections
                        designDeflections[] sMemberDDeflection_x;

                        // Member Design
                        CMemberDesign memberDesignModel = new CMemberDesign();

                        // Set basic deflections for member and load combination
                        CMemberDeflectionsInLoadCombinations mDeflections = MemberDeflectionsInLoadCombinations.Find(i => i.Member.ID == m.ID && i.LoadCombination.ID == lcomb.ID);

                        float fDeflectionLimitDenominator_Fraction = 1; // 25-1000
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
                                fDeflectionLimitDenominator_Fraction = currentMemberTypeGroupOfMembers.DeflectionLimitFraction_Denominator_PermanentLoad;
                                fDeflectionLimit = currentMemberTypeGroupOfMembers.DeflectionLimit_PermanentLoad;
                            }
                            else
                            {
                                fDeflectionLimitDenominator_Fraction = currentMemberTypeGroupOfMembers.DeflectionLimitFraction_Denominator_Total;
                                fDeflectionLimit = currentMemberTypeGroupOfMembers.DeflectionLimit_Total;
                            }
                        }

                        // Design check procedure
                        memberDesignModel.SetDesignDeflections_PFD(MUseCRSCGeometricalAxes,
                            iNumberOfDesignSections,
                            m,
                            fDeflectionLimitDenominator_Fraction,
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

                        //SetMaximumDesignRatioByComponentType(m, lcomb, memberDesignModel, ref sDesignResults_ULSandSLS);
                        //SetMaximumDesignRatioByComponentType(m, lcomb, memberDesignModel, ref sDesignResults_SLS);
                    }
                }

                SolverWindow.Progress += step;
                SolverWindow.UpdateProgress();
            }
        }
    }
}
