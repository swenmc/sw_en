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
    public class CMemberDesignCalculations
    {
        const int iNumberOfDesignSections = 11; // 11 rezov, 10 segmentov
        const int iNumberOfSegments = iNumberOfDesignSections - 1;

        float[] fx_positions;
        double step;
        private Solver SolverWindow;
        private CModel_PFD_01_GR Model;
        private bool MUseCRSCGeometricalAxes;
        private bool DeterminateCombinationResultsByFEMSolver;
        private bool UseFEMSolverCalculationForSimpleBeam;

        public List<CMemberInternalForcesInLoadCases> MemberInternalForcesInLoadCases;
        public List<CMemberDeflectionsInLoadCases> MemberDeflectionsInLoadCases;

        public List<CMemberInternalForcesInLoadCombinations> MemberInternalForcesInLoadCombinations;
        public List<CMemberDeflectionsInLoadCombinations> MemberDeflectionsInLoadCombinations;

        public List<CMemberLoadCombinationRatio_ULS> MemberDesignResults_ULS = new List<CMemberLoadCombinationRatio_ULS>();
        public List<CMemberLoadCombinationRatio_SLS> MemberDesignResults_SLS = new List<CMemberLoadCombinationRatio_SLS>();
        public List<CJointLoadCombinationRatio_ULS> JointDesignResults_ULS;

        private List<CFrame> frameModels;
        private List<CBeam_Simple> beamSimpleModels;
        
        /////////////////////////////////////////////////////////////////////////////////////////////////////////
        float fMaximumDesignRatioWholeStructure = 0;
        float fMaximumDesignRatioMainColumn = 0;
        float fMaximumDesignRatioMainRafter = 0;
        float fMaximumDesignRatioEndColumn = 0;
        float fMaximumDesignRatioEndRafter = 0;
        float fMaximumDesignRatioGirts = 0;
        float fMaximumDesignRatioPurlins = 0;
        float fMaximumDesignRatioColumns = 0;

        CMember MaximumDesignRatioWholeStructureMember = new CMember();
        CMember MaximumDesignRatioMainColumn = new CMember();
        CMember MaximumDesignRatioMainRafter = new CMember();
        CMember MaximumDesignRatioEndColumn = new CMember();
        CMember MaximumDesignRatioEndRafter = new CMember();
        CMember MaximumDesignRatioGirt = new CMember();
        CMember MaximumDesignRatioPurlin = new CMember();
        CMember MaximumDesignRatioColumn = new CMember();


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
            step = (100.0 - SolverWindow.Progress) / (Model.m_arrMembers.Length * 2.0);

            MemberInternalForcesInLoadCases = new List<CMemberInternalForcesInLoadCases>();
            MemberDeflectionsInLoadCases = new List<CMemberDeflectionsInLoadCases>();

            MemberInternalForcesInLoadCombinations = new List<CMemberInternalForcesInLoadCombinations>();
            MemberDeflectionsInLoadCombinations = new List<CMemberDeflectionsInLoadCombinations>();
        }

        public void CalculateAll()
        {
            //if (debugging) System.Diagnostics.Trace.WriteLine("before calculations: " + (DateTime.Now - start).TotalMilliseconds);

            // Zostavovat modely a pocitat vn. sily by malo stacit len pre load cases
            // Pre Load Combinations by sme mali len poprenasobovat hodnoty z load cases faktormi a spocitat ich hodnoty ako jednoduchy sucet, nemusi sa vytvarat nahradny vypoctovy model
            // Potom by mal prebehnut cyklus pre design (vsetky pruty a vsetky load combination, ale uz len pre memberDesignModel s hodnotami vn sil v rezoch)

            CalculateInternalForces_LoadCase();

            CalculateInternalForces_LoadCombination_And_MemberDesign();

            SolverWindow.Progress = 100;
            SolverWindow.UpdateProgress();
            SolverWindow.SetSumaryFinished();


            ShowResultsInMessageBox();
        }

        public void CalculateInternalForces_LoadCase()
        {
            designMomentValuesForCb sMomentValuesforCb = new designMomentValuesForCb();
            basicInternalForces[] sBIF_x = null;
            basicDeflections[] sBDeflections_x = null;
            designBucklingLengthFactors sBucklingLengthFactors = new designBucklingLengthFactors();
            SimpleBeamCalculation calcModel = new SimpleBeamCalculation();

            int count = 0;
            SolverWindow.SetMemberDesignLoadCase();
            // Calculate Internal Forces For Load Cases
            foreach (CMember m in Model.m_arrMembers)
            {
                SolverWindow.SetMemberDesignLoadCaseProgress(++count, Model.m_arrMembers.Length);

                if (m.BIsDSelectedForIFCalculation) // Only structural members (not auxiliary members or members with deactivated calculation of internal forces)
                {
                    if (!DeterminateCombinationResultsByFEMSolver)
                    {
                        for (int i = 0; i < iNumberOfDesignSections; i++)
                            fx_positions[i] = ((float)i / (float)iNumberOfSegments) * m.FLength; // Int must be converted to the float to get decimal numbers

                        foreach (CLoadCase lc in Model.m_arrLoadCases)
                        {
                            // Frame member
                            if (m.EMemberType == EMemberType_FS.eMC || m.EMemberType == EMemberType_FS.eMR ||
                                m.EMemberType == EMemberType_FS.eEC || m.EMemberType == EMemberType_FS.eER)
                            {
                                // BEFENet - calculate load cases only

                                // Set indices to search in results
                                int iFrameIndex = CModelHelper.GetFrameIndexForMember(m, frameModels);  //podla ID pruta treba identifikovat do ktoreho ramu patri
                                
                                // Calculate Internal forces just for Load Cases that are included in ULS
                                if (lc.MType_LS == ELCGTypeForLimitState.eUniversal || lc.MType_LS == ELCGTypeForLimitState.eULSOnly)
                                {
                                    // Nastavit vysledky pre prut ramu

                                    // TODO - faktory vzpernej dlzky mozu byt ine pre kazdy segment pruta a kazdy load case alebo load combination
                                    // TODO - vymysliet system ako s tym pracovat a priradzovat, je potrebne pouzit pri posudeni v mieste x na prute
                                    // Podobne aj hodnoty M14,M24,M34 a Mmax sa maju brat z hodnot na segmente, nie na celom prute

                                    SetDefaultBucklingFactors(m, ref sBucklingLengthFactors);

                                    SetMomentValuesforCb_design_Frame(iFrameIndex, lc.ID, m, ref sMomentValuesforCb);

                                    sBIF_x = frameModels[iFrameIndex].LoadCombInternalForcesResults[lc.ID][m.ID].InternalForces.ToArray();
                                }

                                if (lc.MType_LS == ELCGTypeForLimitState.eUniversal || lc.MType_LS == ELCGTypeForLimitState.eSLSOnly)
                                {
                                    sBDeflections_x = frameModels[iFrameIndex].LoadCombInternalForcesResults[lc.ID][m.ID].Deflections.ToArray();
                                }

                                if (sBIF_x != null) MemberInternalForcesInLoadCases.Add(new CMemberInternalForcesInLoadCases(m, lc, sBIF_x, sMomentValuesforCb));
                                if (sBDeflections_x != null) MemberDeflectionsInLoadCases.Add(new CMemberDeflectionsInLoadCases(m, lc, sBDeflections_x));
                            }
                            else // Single member
                            {
                                if (UseFEMSolverCalculationForSimpleBeam)
                                {
                                    // BEFENet - calculate load cases only

                                    // Set indices to search in results
                                    int iSimpleBeamIndex = CModelHelper.GetSimpleBeamIndexForMember(m, beamSimpleModels);  //podla ID pruta treba identifikovat do ktoreho simple beam modelu patri

                                    // Calculate Internal forces just for Load Cases that are included in ULS
                                    if (lc.MType_LS == ELCGTypeForLimitState.eUniversal || lc.MType_LS == ELCGTypeForLimitState.eULSOnly)
                                    {
                                        SetDefaultBucklingFactors(m, ref sBucklingLengthFactors);

                                        SetMomentValuesforCb_design_SimpleBeam(iSimpleBeamIndex, lc.ID, m, ref sMomentValuesforCb);

                                        sBIF_x = beamSimpleModels[iSimpleBeamIndex].LoadCombInternalForcesResults[lc.ID][m.ID].InternalForces.ToArray();
                                    }

                                    if (lc.MType_LS == ELCGTypeForLimitState.eUniversal || lc.MType_LS == ELCGTypeForLimitState.eSLSOnly)
                                    {
                                        sBDeflections_x = beamSimpleModels[iSimpleBeamIndex].LoadCombInternalForcesResults[lc.ID][m.ID].Deflections.ToArray();
                                    }

                                    if (sBIF_x != null) MemberInternalForcesInLoadCases.Add(new CMemberInternalForcesInLoadCases(m, lc, sBIF_x, sMomentValuesforCb));
                                    if (sBDeflections_x != null) MemberDeflectionsInLoadCases.Add(new CMemberDeflectionsInLoadCases(m, lc, sBDeflections_x));
                                }
                                else
                                {
                                    // Calculate Internal forces just for Load Cases that are included in ULS
                                    if (lc.MType_LS == ELCGTypeForLimitState.eUniversal || lc.MType_LS == ELCGTypeForLimitState.eULSOnly)
                                    {
                                        // ULS - internal forces
                                        calcModel.CalculateInternalForcesOnSimpleBeam_PFD(MUseCRSCGeometricalAxes, iNumberOfDesignSections, fx_positions, m, lc, out sBIF_x, out sBucklingLengthFactors, out sMomentValuesforCb);
                                    }

                                    if (lc.MType_LS == ELCGTypeForLimitState.eUniversal || lc.MType_LS == ELCGTypeForLimitState.eSLSOnly)
                                    {
                                        // SLS - deflections
                                        calcModel.CalculateDeflectionsOnSimpleBeam_PFD(MUseCRSCGeometricalAxes, iNumberOfDesignSections, fx_positions, m, lc, out sBDeflections_x);
                                    }

                                    if (sBIF_x != null) MemberInternalForcesInLoadCases.Add(new CMemberInternalForcesInLoadCases(m, lc, sBIF_x, sMomentValuesforCb));
                                    if (sBDeflections_x != null) MemberDeflectionsInLoadCases.Add(new CMemberDeflectionsInLoadCases(m, lc, sBDeflections_x));
                                }
                            }
                        }//end foreach load case
                    }
                }
                SolverWindow.Progress += step;
                SolverWindow.UpdateProgress();
            }

        }
        public void CalculateInternalForces_LoadCombination_And_MemberDesign()
        {
            // Design of members
            // Calculate Internal Forces For Load Combinations
            MemberDesignResults_ULS = new List<CMemberLoadCombinationRatio_ULS>();
            MemberDesignResults_SLS = new List<CMemberLoadCombinationRatio_SLS>();

            JointDesignResults_ULS = new List<CJointLoadCombinationRatio_ULS>();

            SolverWindow.SetMemberDesignLoadCombination();
            int count = 0;
            foreach (CMember m in Model.m_arrMembers)
            {
                SolverWindow.SetMemberDesignLoadCombinationProgress(++count, Model.m_arrMembers.Length);

                if (m.BIsDSelectedForIFCalculation) // Only structural members (not auxiliary members or members with deactivated calculation of internal forces)
                {
                    for (int i = 0; i < iNumberOfDesignSections; i++)
                        fx_positions[i] = ((float)i / (float)iNumberOfSegments) * m.FLength; // Int must be converted to the float to get decimal numbers

                    foreach (CLoadCombination lcomb in Model.m_arrLoadCombs)
                    {
                        if (lcomb.eLComType == ELSType.eLS_ULS) // Do not perform internal foces calculation for ULS
                        {
                            // Member basic internal forces
                            designBucklingLengthFactors sBucklingLengthFactors_design = new designBucklingLengthFactors();
                            designMomentValuesForCb sMomentValuesforCb_design = new designMomentValuesForCb();
                            basicInternalForces[] sBIF_x_design;

                            // Chcelo by to ten Example3 upravit a zobecnit tak aby sa z neho dali tahat rozne vysledky, podobne ako sa to da zo samotnej kniznice BFENet

                            // Frame member - vysledky pocitane pre load combinations
                            if (DeterminateCombinationResultsByFEMSolver && (m.EMemberType == EMemberType_FS.eMC || m.EMemberType == EMemberType_FS.eMR || m.EMemberType == EMemberType_FS.eEC || m.EMemberType == EMemberType_FS.eER))
                            {
                                // Nastavit vysledky pre prut ramu
                                // TODO - faktory vzpernej dlzky mozu byt ine pre kazdy segment pruta a kazdy load case alebo load combination
                                // TODO - vymysliet system ako s tym pracovat a priradzovat, je potrebne pouzit pri posudeni v mieste x na prute
                                // Podobne aj veliciny M14,M24,M34 a Mmax sa maju brat z priebehu ohyboveho momentu na segmente v absolutnej hodnote

                                SetDefaultBucklingFactors(m, ref sBucklingLengthFactors_design);

                                int iFrameIndex = CModelHelper.GetFrameIndexForMember(m, frameModels);  //podla ID pruta treba identifikovat do ktoreho ramu patri

                                SetMomentValuesforCb_design_Frame(iFrameIndex, lcomb.ID, m, ref sMomentValuesforCb_design);

                                sBIF_x_design = frameModels[iFrameIndex].LoadCombInternalForcesResults[lcomb.ID][m.ID].InternalForces.ToArray();

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
                            else if (DeterminateCombinationResultsByFEMSolver) // Single Beam Members - vysledky pocitane v BFENet pre Load Combinations
                            {
                                // Nastavit vysledky pre prut simple beam modelu
                                SetDefaultBucklingFactors(m, ref sBucklingLengthFactors_design);

                                int iSimpleBeamIndex = CModelHelper.GetSimpleBeamIndexForMember(m, beamSimpleModels);  //podla ID pruta treba identifikovat do ktoreho simple beam modelu patri

                                SetMomentValuesforCb_design_SimpleBeam(iSimpleBeamIndex, lcomb.ID, m, ref sMomentValuesforCb_design);

                                sBIF_x_design = beamSimpleModels[iSimpleBeamIndex].LoadCombInternalForcesResults[lcomb.ID][m.ID].InternalForces.ToArray();

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
                            else // Single Member or Frame Member (only LC calculated) - vysledky pocitane pre load cases
                            {
                                CMemberResultsManager.SetMemberInternalForcesInLoadCombination(m, lcomb, MemberInternalForcesInLoadCases, iNumberOfDesignSections, out sBucklingLengthFactors_design, out sMomentValuesforCb_design, out sBIF_x_design);
                            }

                            // 22.2.2019 - Ulozime vnutorne sily v kombinacii - pre zobrazenie v Internal forces
                            if (sBIF_x_design != null) MemberInternalForcesInLoadCombinations.Add(new CMemberInternalForcesInLoadCombinations(m, lcomb, sBIF_x_design, sMomentValuesforCb_design));

                            // Member design internal forces
                            if (m.BIsDSelectedForDesign) // Only structural members (not auxiliary members or members with deactivated design)
                            {
                                designInternalForces[] sMemberDIF_x;

                                // Member Design
                                CMemberDesign memberDesignModel = new CMemberDesign();
                                // TODO - sBucklingLengthFactors_design  a sMomentValuesforCb_design nemaju byt priradene prutu ale segmentu pruta pre kazdy load case / load combination
                                memberDesignModel.SetDesignForcesAndMemberDesign_PFD(MUseCRSCGeometricalAxes, iNumberOfDesignSections, m, sBIF_x_design, sBucklingLengthFactors_design, sMomentValuesforCb_design, out sMemberDIF_x);
                                MemberDesignResults_ULS.Add(new CMemberLoadCombinationRatio_ULS(m, lcomb, memberDesignModel.fMaximumDesignRatio, sMemberDIF_x[memberDesignModel.fMaximumDesignRatioLocationID], sBucklingLengthFactors_design, sMomentValuesforCb_design)); 
                                
                                // Set maximum design ratio of whole structure
                                if (memberDesignModel.fMaximumDesignRatio > fMaximumDesignRatioWholeStructure)
                                {
                                    fMaximumDesignRatioWholeStructure = memberDesignModel.fMaximumDesignRatio;
                                    MaximumDesignRatioWholeStructureMember = m;
                                }

                                // Joint Design
                                CJointDesign jointDesignModel = new CJointDesign();

                                designInternalForces sjointStartDIF_x;
                                designInternalForces sjointEndDIF_x;
                                CConnectionJointTypes jointStart;
                                CConnectionJointTypes jointEnd;

                                jointDesignModel.SetDesignForcesAndJointDesign_PFD(iNumberOfDesignSections, MUseCRSCGeometricalAxes, Model, m, sBIF_x_design, out jointStart, out jointEnd, out sjointStartDIF_x, out sjointEndDIF_x);

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

                                SetMaximumDesignRatioByComponentType(m, memberDesignModel);
                            }
                        }
                        else // SLS
                        {
                            // Member basic deflections
                            basicDeflections[] sBDeflection_x_design;

                            // Member design deflections
                            if (m.BIsDSelectedForDesign) // Only structural members (not auxiliary members or members with deactivated design)
                            {
                                designDeflections[] sDDeflection_x;
                                CMemberDesign memberDesignModel = new CMemberDesign();

                                // TODO - Pripravit vysledky na jednotlivych prutoch povodneho 3D modelu pre pruty ramov aj ostatne pruty ktore su samostatne
                                // Frame member - vysledky pocitane pre load combinations
                                if (DeterminateCombinationResultsByFEMSolver && (m.EMemberType == EMemberType_FS.eMC || m.EMemberType == EMemberType_FS.eMR || m.EMemberType == EMemberType_FS.eEC || m.EMemberType == EMemberType_FS.eER))
                                {
                                    int iFrameIndex = CModelHelper.GetFrameIndexForMember(m, frameModels);  //podla ID pruta treba identifikovat do ktoreho ramu patri
                                    sBDeflection_x_design = frameModels[iFrameIndex].LoadCombInternalForcesResults[lcomb.ID][m.ID].Deflections.ToArray();
                                }
                                else if (DeterminateCombinationResultsByFEMSolver)  // Single Beam Members - vysledky pocitane v BFENet pre Load Combinations
                                {
                                    int iSimpleBeamIndex = CModelHelper.GetSimpleBeamIndexForMember(m, beamSimpleModels);  //podla ID pruta treba identifikovat do ktoreho simple beam modelu patri
                                    sBDeflection_x_design = beamSimpleModels[iSimpleBeamIndex].LoadCombInternalForcesResults[lcomb.ID][m.ID].Deflections.ToArray();
                                }
                                else // Single Member or Frame Member (only LC calculated) - vysledky pocitane pre load cases
                                {
                                    CMemberResultsManager.SetMemberDeflectionsInLoadCombination(m, lcomb, MemberDeflectionsInLoadCases, iNumberOfDesignSections, out sBDeflection_x_design);
                                }

                                memberDesignModel.SetDesignDeflections_PFD(MUseCRSCGeometricalAxes, iNumberOfDesignSections, m, sBDeflection_x_design, out sDDeflection_x);
                                MemberDesignResults_SLS.Add(new CMemberLoadCombinationRatio_SLS(m, lcomb, memberDesignModel.fMaximumDesignRatio, sDDeflection_x[memberDesignModel.fMaximumDesignRatioLocationID]));

                                // 22.2.2019 - Ulozime priehyby v kombinacii - pre zobrazenie v Internal forces
                                if (sBDeflection_x_design != null) MemberDeflectionsInLoadCombinations.Add(new CMemberDeflectionsInLoadCombinations(m, lcomb, sBDeflection_x_design));

                                // Set maximum design ratio of whole structure
                                if (memberDesignModel.fMaximumDesignRatio > fMaximumDesignRatioWholeStructure)
                                {
                                    fMaximumDesignRatioWholeStructure = memberDesignModel.fMaximumDesignRatio;
                                    MaximumDesignRatioWholeStructureMember = m;
                                }

                                // Output (for debugging)
                                bool bDebugging = false; // Testovacie ucely
                                if (bDebugging)
                                    System.Diagnostics.Trace.WriteLine("Member ID: " + m.ID + "\t | " +
                                                      "Load Combination ID: " + lcomb.ID + "\t | " +
                                                      "Design Ratio: " + Math.Round(memberDesignModel.fMaximumDesignRatio, 3).ToString());
                            }
                        }
                    }
                }
                SolverWindow.Progress += step;
                SolverWindow.UpdateProgress();
            }
        }

        private void SetMaximumDesignRatioByComponentType(CMember m, CMemberDesign memberDesignModel)
        {
            // Output - set maximum design ratio by component Type
            switch (m.EMemberType)
            {
                case EMemberType_FS.eMC: // Main Column
                    {
                        if (memberDesignModel.fMaximumDesignRatio > fMaximumDesignRatioMainColumn)
                        {
                            fMaximumDesignRatioMainColumn = memberDesignModel.fMaximumDesignRatio;
                            MaximumDesignRatioMainColumn = m;
                        }
                        break;
                    }
                case EMemberType_FS.eMR: // Main Rafter
                    {
                        if (memberDesignModel.fMaximumDesignRatio > fMaximumDesignRatioMainRafter)
                        {
                            fMaximumDesignRatioMainRafter = memberDesignModel.fMaximumDesignRatio;
                            MaximumDesignRatioMainRafter = m;
                        }
                        break;
                    }
                case EMemberType_FS.eEC: // End Column
                    {
                        if (memberDesignModel.fMaximumDesignRatio > fMaximumDesignRatioEndColumn)
                        {
                            fMaximumDesignRatioEndColumn = memberDesignModel.fMaximumDesignRatio;
                            MaximumDesignRatioEndColumn = m;
                        }
                        break;
                    }
                case EMemberType_FS.eER: // End Rafter
                    {
                        if (memberDesignModel.fMaximumDesignRatio > fMaximumDesignRatioEndRafter)
                        {
                            fMaximumDesignRatioEndRafter = memberDesignModel.fMaximumDesignRatio;
                            MaximumDesignRatioEndRafter = m;
                        }
                        break;
                    }
                case EMemberType_FS.eG: // Girt
                    {
                        if (memberDesignModel.fMaximumDesignRatio > fMaximumDesignRatioGirts)
                        {
                            fMaximumDesignRatioGirts = memberDesignModel.fMaximumDesignRatio;
                            MaximumDesignRatioGirt = m;
                        }
                        break;
                    }
                case EMemberType_FS.eP: // Purlin
                    {
                        if (memberDesignModel.fMaximumDesignRatio > fMaximumDesignRatioPurlins)
                        {
                            fMaximumDesignRatioPurlins = memberDesignModel.fMaximumDesignRatio;
                            MaximumDesignRatioPurlin = m;
                        }
                        break;
                    }
                case EMemberType_FS.eC: // Column
                    {
                        if (memberDesignModel.fMaximumDesignRatio > fMaximumDesignRatioColumns)
                        {
                            fMaximumDesignRatioColumns = memberDesignModel.fMaximumDesignRatio;
                            MaximumDesignRatioColumn = m;
                        }
                        break;
                    }
                default:
                    // TODO - modifikovat podla potrieb pre ukladanie - doplnit vsetky typy
                    break;
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

        private void SetMomentValuesforCb_design_Frame(int iFrameIndex, int lcombID, CMember member, ref designMomentValuesForCb sMomentValuesforCb_design)
        {
            // Create load combination (FEM solver object)
            BriefFiniteElementNet.LoadCombination lcomb = new BriefFiniteElementNet.LoadCombination();
            lcomb.LcID = lcombID;

            // Add specific load cases into the combination and set load factors
            for (int j = 0; j < Model.m_arrLoadCombs[lcombID - 1].LoadCasesList.Count; j++)
            {
                BriefFiniteElementNet.LoadType loadtype = CModelToBFEMNetConverter.GetBFEMLoadType(Model.m_arrLoadCombs[lcombID - 1].LoadCasesList[j].Type);
                BriefFiniteElementNet.LoadCase loadCase = new BriefFiniteElementNet.LoadCase(Model.m_arrLoadCases[j].Name, loadtype);
                lcomb.Add(loadCase, Model.m_arrLoadCombs[lcombID - 1].LoadCasesFactorsList[j]);
            }

            int iMemberIndex_FM = frameModels[iFrameIndex].GetMemberIndexInFrame(member);

            BriefFiniteElementNet.Force f14 = ((BriefFiniteElementNet.FrameElement2Node)(frameModels[iFrameIndex].BFEMNetModel.Elements[iMemberIndex_FM])).GetInternalForceAt(0.25 * member.FLength, lcomb);
            BriefFiniteElementNet.Force f24 = ((BriefFiniteElementNet.FrameElement2Node)(frameModels[iFrameIndex].BFEMNetModel.Elements[iMemberIndex_FM])).GetInternalForceAt(0.50 * member.FLength, lcomb);
            BriefFiniteElementNet.Force f34 = ((BriefFiniteElementNet.FrameElement2Node)(frameModels[iFrameIndex].BFEMNetModel.Elements[iMemberIndex_FM])).GetInternalForceAt(0.75 * member.FLength, lcomb);

            sMomentValuesforCb_design.fM_14 = (float)Math.Abs(f14.My);
            sMomentValuesforCb_design.fM_24 = (float)Math.Abs(f24.My);
            sMomentValuesforCb_design.fM_34 = (float)Math.Abs(f34.My);
            sMomentValuesforCb_design.fM_max = 0;

            for (int i = 0; i < iNumberOfDesignSections; i++)
            {
                float fx = ((float)i / (float)iNumberOfSegments) * member.FLength;
                BriefFiniteElementNet.Force f = ((BriefFiniteElementNet.FrameElement2Node)(frameModels[iFrameIndex].BFEMNetModel.Elements[iMemberIndex_FM])).GetInternalForceAt(fx, lcomb);

                if (Math.Abs(f.My) > sMomentValuesforCb_design.fM_max)
                    sMomentValuesforCb_design.fM_max = (float)Math.Abs(f.My);
            }

            /*
            if (MUseCRSCGeometricalAxes)
            {
                sMomentValuesforCb_design.fM_14 = frameModels[iFrameIndex].LoadCombInternalForcesResults[lcombID][member.ID].InternalForces[2].fM_yy;
                sMomentValuesforCb_design.fM_24 = frameModels[iFrameIndex].LoadCombInternalForcesResults[lcombID][member.ID].InternalForces[5].fM_yy;
                sMomentValuesforCb_design.fM_34 = frameModels[iFrameIndex].LoadCombInternalForcesResults[lcombID][member.ID].InternalForces[7].fM_yy;
            }
            else
            {
                sMomentValuesforCb_design.fM_14 = frameModels[iFrameIndex].LoadCombInternalForcesResults[lcombID][member.ID].InternalForces[2].fM_yu;
                sMomentValuesforCb_design.fM_24 = frameModels[iFrameIndex].LoadCombInternalForcesResults[lcombID][member.ID].InternalForces[5].fM_yu;
                sMomentValuesforCb_design.fM_34 = frameModels[iFrameIndex].LoadCombInternalForcesResults[lcombID][member.ID].InternalForces[7].fM_yu;
            }

            sMomentValuesforCb_design.fM_max = MathF.Max(sMomentValuesforCb_design.fM_14, sMomentValuesforCb_design.fM_24, sMomentValuesforCb_design.fM_34); // TODO - urcit z priebehu sil na danom prute
            */
        }

        private void SetMomentValuesforCb_design_SimpleBeam(int iSimpleBeamIndex, int lcombID, CMember member, ref designMomentValuesForCb sMomentValuesforCb_design)
        {
            // Create load combination (FEM solver object)
            BriefFiniteElementNet.LoadCombination lcomb = new BriefFiniteElementNet.LoadCombination();
            lcomb.LcID = lcombID;

            // Add specific load cases into the combination and set load factors
            for (int j = 0; j < Model.m_arrLoadCombs[lcombID - 1].LoadCasesList.Count; j++)
            {
                BriefFiniteElementNet.LoadType loadtype = CModelToBFEMNetConverter.GetBFEMLoadType(Model.m_arrLoadCombs[lcombID - 1].LoadCasesList[j].Type);
                BriefFiniteElementNet.LoadCase loadCase = new BriefFiniteElementNet.LoadCase(Model.m_arrLoadCases[j].Name, loadtype);
                lcomb.Add(loadCase, Model.m_arrLoadCombs[lcombID - 1].LoadCasesFactorsList[j]);
            }

            BriefFiniteElementNet.Force f14 = ((BriefFiniteElementNet.FrameElement2Node)(beamSimpleModels[iSimpleBeamIndex].BFEMNetModel.Elements[0])).GetInternalForceAt(0.25 * member.FLength, lcomb);
            BriefFiniteElementNet.Force f24 = ((BriefFiniteElementNet.FrameElement2Node)(beamSimpleModels[iSimpleBeamIndex].BFEMNetModel.Elements[0])).GetInternalForceAt(0.50 * member.FLength, lcomb);
            BriefFiniteElementNet.Force f34 = ((BriefFiniteElementNet.FrameElement2Node)(beamSimpleModels[iSimpleBeamIndex].BFEMNetModel.Elements[0])).GetInternalForceAt(0.75 * member.FLength, lcomb);

            sMomentValuesforCb_design.fM_14 = (float)Math.Abs(f14.My);
            sMomentValuesforCb_design.fM_24 = (float)Math.Abs(f24.My);
            sMomentValuesforCb_design.fM_34 = (float)Math.Abs(f34.My);
            sMomentValuesforCb_design.fM_max = 0;

            for (int i = 0; i < iNumberOfDesignSections; i++)
            {
                float fx = ((float)i / (float)iNumberOfSegments) * member.FLength;
                BriefFiniteElementNet.Force f = ((BriefFiniteElementNet.FrameElement2Node)(beamSimpleModels[iSimpleBeamIndex].BFEMNetModel.Elements[0])).GetInternalForceAt(fx, lcomb);

                if (Math.Abs(f.My) > sMomentValuesforCb_design.fM_max)
                    sMomentValuesforCb_design.fM_max = (float)Math.Abs(f.My);
            }

            /*
            if (MUseCRSCGeometricalAxes)
            {
                sMomentValuesforCb_design.fM_14 = beamSimpleModels[iSimpleBeamIndex].LoadCombInternalForcesResults[lcombID][member.ID].InternalForces[2].fM_yy;
                sMomentValuesforCb_design.fM_24 = beamSimpleModels[iSimpleBeamIndex].LoadCombInternalForcesResults[lcombID][member.ID].InternalForces[5].fM_yy;
                sMomentValuesforCb_design.fM_34 = beamSimpleModels[iSimpleBeamIndex].LoadCombInternalForcesResults[lcombID][member.ID].InternalForces[7].fM_yy;
            }
            else
            {
                sMomentValuesforCb_design.fM_14 = beamSimpleModels[iSimpleBeamIndex].LoadCombInternalForcesResults[lcombID][member.ID].InternalForces[2].fM_yu;
                sMomentValuesforCb_design.fM_24 = beamSimpleModels[iSimpleBeamIndex].LoadCombInternalForcesResults[lcombID][member.ID].InternalForces[5].fM_yu;
                sMomentValuesforCb_design.fM_34 = beamSimpleModels[iSimpleBeamIndex].LoadCombInternalForcesResults[lcombID][member.ID].InternalForces[7].fM_yu;
            }

            sMomentValuesforCb_design.fM_max = MathF.Max(sMomentValuesforCb_design.fM_14, sMomentValuesforCb_design.fM_24, sMomentValuesforCb_design.fM_34); // TODO - urcit z priebehu sil na danom prute
            */
        }

        private void ShowResultsInMessageBox()
        {
            string txt = "Calculation Results \n" +
                    "Maximum design ratio \n" +
                    "Member ID: " + MaximumDesignRatioWholeStructureMember.ID.ToString() + "\t Design Ratio η: " + Math.Round(fMaximumDesignRatioWholeStructure, 3).ToString() + "\n\n\n" +
                    "Maximum design ratio - main columns\n" +
                    "Member ID: " + MaximumDesignRatioMainColumn.ID.ToString() + "\t Design Ratio η: " + Math.Round(fMaximumDesignRatioMainColumn, 3).ToString() + "\n\n" +
                    "Maximum design ratio - rafters\n" +
                    "Member ID: " + MaximumDesignRatioMainRafter.ID.ToString() + "\t Design Ratio η: " + Math.Round(fMaximumDesignRatioMainRafter, 3).ToString() + "\n\n" +
                    "Maximum design ratio - end columns\n" +
                    "Member ID: " + MaximumDesignRatioEndColumn.ID.ToString() + "\t Design Ratio η: " + Math.Round(fMaximumDesignRatioEndColumn, 3).ToString() + "\n\n" +
                    "Maximum design ratio - end rafters\n" +
                    "Member ID: " + MaximumDesignRatioEndRafter.ID.ToString() + "\t Design Ratio η: " + Math.Round(fMaximumDesignRatioEndRafter, 3).ToString() + "\n\n" +
                    "Maximum design ratio - girts\n" +
                    "Member ID: " + MaximumDesignRatioGirt.ID.ToString() + "\t Design Ratio η: " + Math.Round(fMaximumDesignRatioGirts, 3).ToString() + "\n\n" +
                    "Maximum design ratio - purlins\n" +
                    "Member ID: " + MaximumDesignRatioPurlin.ID.ToString() + "\t Design Ratio η: " + Math.Round(fMaximumDesignRatioPurlins, 3).ToString() + "\n\n" +
                    "Maximum design ratio - columns\n" +
                    "Member ID: " + MaximumDesignRatioColumn.ID.ToString() + "\t Design Ratio η: " + Math.Round(fMaximumDesignRatioColumns, 3).ToString() + "\n\n";

            SolverWindow.ShowMessageBox(txt);
        }
    }
}
