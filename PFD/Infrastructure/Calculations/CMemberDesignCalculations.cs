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
    public struct sDesignResults
    {
        public string sLimitStateType;

        public float fMaximumDesignRatioWholeStructure;
        public float fMaximumDesignRatioMainColumn;
        public float fMaximumDesignRatioMainRafter;
        public float fMaximumDesignRatioEndColumn;
        public float fMaximumDesignRatioEndRafter;
        public float fMaximumDesignRatioGirts;
        public float fMaximumDesignRatioPurlins;
        public float fMaximumDesignRatioColumns;

        public CLoadCombination GoverningLoadCombinationStructure;
        public CLoadCombination GoverningLoadCombinationMainColumn;
        public CLoadCombination GoverningLoadCombinationMainRafter;
        public CLoadCombination GoverningLoadCombinationEndColumn;
        public CLoadCombination GoverningLoadCombinationEndRafter;
        public CLoadCombination GoverningLoadCombinationGirts;
        public CLoadCombination GoverningLoadCombinationPurlins;
        public CLoadCombination GoverningLoadCombinationColumns;

        public CMember MaximumDesignRatioWholeStructureMember;
        public CMember MaximumDesignRatioMainColumn;
        public CMember MaximumDesignRatioMainRafter;
        public CMember MaximumDesignRatioEndColumn;
        public CMember MaximumDesignRatioEndRafter;
        public CMember MaximumDesignRatioGirt;
        public CMember MaximumDesignRatioPurlin;
        public CMember MaximumDesignRatioColumn;
    }

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
            step = (100.0 - SolverWindow.Progress) / (Model.m_arrMembers.Length * 2.0);

            MemberInternalForcesInLoadCases = new List<CMemberInternalForcesInLoadCases>();
            MemberDeflectionsInLoadCases = new List<CMemberDeflectionsInLoadCases>();

            MemberInternalForcesInLoadCombinations = new List<CMemberInternalForcesInLoadCombinations>();
            MemberDeflectionsInLoadCombinations = new List<CMemberDeflectionsInLoadCombinations>();
        }

        public void CalculateAll()
        {
            //if (debugging) System.Diagnostics.Trace.WriteLine("before calculations: " + (DateTime.Now - start).TotalMilliseconds);
            CalculateInternalForces_LoadCase();

            CalculateInternalForces_LoadCombination_And_MemberDesign();

            SolverWindow.Progress = 100;
            SolverWindow.UpdateProgress();
            SolverWindow.SetSumaryFinished(
               GetTextForResultsMessageBox(sDesignResults_ULSandSLS) + 
               GetTextForResultsMessageBox(sDesignResults_ULS) +
               GetTextForResultsMessageBox(sDesignResults_SLS));
        }

        public void CalculateInternalForces_LoadCase()
        {
            designMomentValuesForCb[] sMomentValuesforCb = null;
            basicInternalForces[] sBIF_x = null;
            basicDeflections[] sBDeflections_x = null;
            designBucklingLengthFactors[] sBucklingLengthFactors = null;
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
                                    //SetDefaultBucklingFactors(m, ref sBucklingLengthFactors);
                                    //SetMomentValuesforCb_design_Frame(iFrameIndex, lc.ID, m, ref sMomentValuesforCb);

                                    sBucklingLengthFactors = new designBucklingLengthFactors[iNumberOfDesignSections];
                                    sMomentValuesforCb = new designMomentValuesForCb[iNumberOfDesignSections];

                                    for (int j = 0; j < fx_positions.Length; j++)
                                    {
                                        designBucklingLengthFactors sBucklingLengthFactors_temp = new designBucklingLengthFactors();
                                        designMomentValuesForCb sMomentValuesforCb_temp = new designMomentValuesForCb();
                                        SetMomentValuesforCb_design_And_BucklingFactors_Frame(fx_positions[j], iFrameIndex, lc.ID, m, ref sBucklingLengthFactors_temp, ref sMomentValuesforCb_temp);

                                        sBucklingLengthFactors[j] = sBucklingLengthFactors_temp;
                                        sMomentValuesforCb[j] = sMomentValuesforCb_temp;
                                    }

                                    sBIF_x = frameModels[iFrameIndex].LoadCombInternalForcesResults[lc.ID][m.ID].InternalForces.ToArray();

                                    //sMomentValuesforCb = frameModels[iFrameIndex].LoadCombInternalForcesResults[lc.ID][m.ID].DesignMomentValuesForCb.ToArray();
                                    //sBucklingLengthFactors = frameModels[iFrameIndex].LoadCombInternalForcesResults[lc.ID][m.ID].DesignBucklingLengthFactors.ToArray();
                                }

                                if (lc.MType_LS == ELCGTypeForLimitState.eUniversal || lc.MType_LS == ELCGTypeForLimitState.eSLSOnly)
                                {
                                    sBDeflections_x = frameModels[iFrameIndex].LoadCombInternalForcesResults[lc.ID][m.ID].Deflections.ToArray();
                                }

                                if (sBIF_x != null) MemberInternalForcesInLoadCases.Add(new CMemberInternalForcesInLoadCases(m, lc, sBIF_x, sMomentValuesforCb, sBucklingLengthFactors));
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
                                        //SetDefaultBucklingFactors(m, ref sBucklingLengthFactors);
                                        //SetMomentValuesforCb_design_And_BucklingFactors_SimpleBeamSegment

                                        sBucklingLengthFactors = new designBucklingLengthFactors[iNumberOfDesignSections];
                                        sMomentValuesforCb = new designMomentValuesForCb[iNumberOfDesignSections];

                                        for (int j = 0; j < fx_positions.Length; j++)
                                        {
                                            designBucklingLengthFactors sBucklingLengthFactors_temp = new designBucklingLengthFactors();
                                            designMomentValuesForCb sMomentValuesforCb_temp = new designMomentValuesForCb();
                                            SetMomentValuesforCb_design_And_BucklingFactors_SimpleBeamSegment(fx_positions[j], iSimpleBeamIndex, lc.ID, m, ref sBucklingLengthFactors_temp, ref sMomentValuesforCb_temp);

                                            sBucklingLengthFactors[j] =sBucklingLengthFactors_temp;
                                            sMomentValuesforCb[j] = sMomentValuesforCb_temp;
                                        }

                                        sBIF_x = beamSimpleModels[iSimpleBeamIndex].LoadCombInternalForcesResults[lc.ID][m.ID].InternalForces.ToArray();
                                        //sMomentValuesforCb = beamSimpleModels[iSimpleBeamIndex].LoadCombInternalForcesResults[lc.ID][m.ID].DesignMomentValuesForCb.ToArray();
                                        //sBucklingLengthFactors = beamSimpleModels[iSimpleBeamIndex].LoadCombInternalForcesResults[lc.ID][m.ID].DesignBucklingLengthFactors.ToArray();
                                    }

                                    if (lc.MType_LS == ELCGTypeForLimitState.eUniversal || lc.MType_LS == ELCGTypeForLimitState.eSLSOnly)
                                    {
                                        sBDeflections_x = beamSimpleModels[iSimpleBeamIndex].LoadCombInternalForcesResults[lc.ID][m.ID].Deflections.ToArray();
                                    }

                                    if (sBIF_x != null) MemberInternalForcesInLoadCases.Add(new CMemberInternalForcesInLoadCases(m, lc, sBIF_x, sMomentValuesforCb, sBucklingLengthFactors));
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

                                    if (sBIF_x != null) MemberInternalForcesInLoadCases.Add(new CMemberInternalForcesInLoadCases(m, lc, sBIF_x, sMomentValuesforCb, sBucklingLengthFactors));
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
            sDesignResults_ULSandSLS.sLimitStateType = "ULS and SLS";
            sDesignResults_ULS.sLimitStateType = "ULS";
            sDesignResults_SLS.sLimitStateType = "SLS";

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
                            designBucklingLengthFactors[] sBucklingLengthFactors_design = new designBucklingLengthFactors[iNumberOfDesignSections];
                            designMomentValuesForCb[] sMomentValuesforCb_design = new designMomentValuesForCb[iNumberOfDesignSections];
                            basicInternalForces[] sBIF_x_design = new basicInternalForces[iNumberOfDesignSections];

                            // Frame member - vysledky pocitane pre load combinations
                            if (DeterminateCombinationResultsByFEMSolver && (m.EMemberType == EMemberType_FS.eMC || m.EMemberType == EMemberType_FS.eMR || m.EMemberType == EMemberType_FS.eEC || m.EMemberType == EMemberType_FS.eER))
                            {
                                int iFrameIndex = CModelHelper.GetFrameIndexForMember(m, frameModels);  //podla ID pruta treba identifikovat do ktoreho ramu patri

                                //SetDefaultBucklingFactors(m, ref sBucklingLengthFactors_design);
                                //SetMomentValuesforCb_design_Frame(iFrameIndex, lcomb.ID, m, ref sMomentValuesforCb_design);

                                sBIF_x_design = frameModels[iFrameIndex].LoadCombInternalForcesResults[lcomb.ID][m.ID].InternalForces.ToArray();
                                //sMomentValuesforCb_design = frameModels[iFrameIndex].LoadCombInternalForcesResults[lcomb.ID][m.ID].DesignMomentValuesForCb.ToArray();
                                //sBucklingLengthFactors_design = frameModels[iFrameIndex].LoadCombInternalForcesResults[lcomb.ID][m.ID].DesignBucklingLengthFactors.ToArray();

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
                                int iSimpleBeamIndex = CModelHelper.GetSimpleBeamIndexForMember(m, beamSimpleModels);  //podla ID pruta treba identifikovat do ktoreho simple beam modelu patri

                                // Nastavit vysledky pre prut simple beam modelu
                                //SetDefaultBucklingFactors(m, ref sBucklingLengthFactors_design);
                                //SetMomentValuesforCb_design_SimpleBeam(iSimpleBeamIndex, lcomb.ID, m, ref sMomentValuesforCb_design);

                                sBIF_x_design = beamSimpleModels[iSimpleBeamIndex].LoadCombInternalForcesResults[lcomb.ID][m.ID].InternalForces.ToArray();
                                //sMomentValuesforCb_design = beamSimpleModels[iSimpleBeamIndex].LoadCombInternalForcesResults[lcomb.ID][m.ID].DesignMomentValuesForCb.ToArray();
                                //sBucklingLengthFactors_design = beamSimpleModels[iSimpleBeamIndex].LoadCombInternalForcesResults[lcomb.ID][m.ID].DesignBucklingLengthFactors.ToArray();

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
                                CMemberResultsManager.SetMemberInternalForcesInLoadCombination(MUseCRSCGeometricalAxes, m, lcomb, MemberInternalForcesInLoadCases, iNumberOfDesignSections, out sBucklingLengthFactors_design, out sMomentValuesforCb_design, out sBIF_x_design);
                            }

                            // 22.2.2019 - Ulozime vnutorne sily v kombinacii - pre zobrazenie v Internal forces
                            if (sBIF_x_design != null) MemberInternalForcesInLoadCombinations.Add(new CMemberInternalForcesInLoadCombinations(m, lcomb, sBIF_x_design, sMomentValuesforCb_design, sBucklingLengthFactors_design));

                            // Member design internal forces
                            if (m.BIsDSelectedForDesign) // Only structural members (not auxiliary members or members with deactivated design)
                            {
                                designInternalForces[] sMemberDIF_x;

                                // Member Design
                                CMemberDesign memberDesignModel = new CMemberDesign();
                                // TODO - sBucklingLengthFactors_design  a sMomentValuesforCb_design nemaju byt priradene prutu ale segmentu pruta pre kazdy load case / load combination
                                memberDesignModel.SetDesignForcesAndMemberDesign_PFD(MUseCRSCGeometricalAxes, iNumberOfDesignSections, m, sBIF_x_design, sBucklingLengthFactors_design, sMomentValuesforCb_design, out sMemberDIF_x);
                                MemberDesignResults_ULS.Add(new CMemberLoadCombinationRatio_ULS(m, lcomb, memberDesignModel.fMaximumDesignRatio, sMemberDIF_x[memberDesignModel.fMaximumDesignRatioLocationID], sBucklingLengthFactors_design[memberDesignModel.fMaximumDesignRatioLocationID], sMomentValuesforCb_design[memberDesignModel.fMaximumDesignRatioLocationID])); 

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

                                SetMaximumDesignRatioByComponentType(m, lcomb, memberDesignModel, ref sDesignResults_ULSandSLS);
                                SetMaximumDesignRatioByComponentType(m, lcomb, memberDesignModel, ref sDesignResults_ULS);
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
                                    CMemberResultsManager.SetMemberDeflectionsInLoadCombination(MUseCRSCGeometricalAxes, m, lcomb, MemberDeflectionsInLoadCases, iNumberOfDesignSections, out sBDeflection_x_design);
                                }

                                // Find group of current member (definition of member type)
                                CMemberGroup currentMemberTypeGroupOfMembers = m.GetMemberGroupFromList(Model.listOfModelMemberGroups);

                                float fDeflectionLimit;

                                // Set deflection limit depending of member type and load combination type
                                if (lcomb.IsCombinationOfPermanentLoadCasesOnly())
                                    fDeflectionLimit = currentMemberTypeGroupOfMembers.DeflectionLimit_PermanentLoad;
                                else
                                    fDeflectionLimit = currentMemberTypeGroupOfMembers.DeflectionLimit_Total;

                                memberDesignModel.SetDesignDeflections_PFD(MUseCRSCGeometricalAxes, iNumberOfDesignSections, m, fDeflectionLimit, sBDeflection_x_design, out sDDeflection_x);
                                MemberDesignResults_SLS.Add(new CMemberLoadCombinationRatio_SLS(m, lcomb, memberDesignModel.fMaximumDesignRatio, sDDeflection_x[memberDesignModel.fMaximumDesignRatioLocationID]));

                                // 22.2.2019 - Ulozime priehyby v kombinacii - pre zobrazenie v Internal forces
                                if (sBDeflection_x_design != null) MemberDeflectionsInLoadCombinations.Add(new CMemberDeflectionsInLoadCombinations(m, lcomb, sBDeflection_x_design));

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
                    }
                }
                SolverWindow.Progress += step;
                SolverWindow.UpdateProgress();
            }
        }

        private void SetMaximumDesignRatioByComponentType(CMember m, CLoadCombination lcomb, CMemberDesign memberDesignModel, ref sDesignResults s)
        {
            // Output - set maximum design ratio by component Type
            switch (m.EMemberType)
            {
                case EMemberType_FS.eMC: // Main Column
                    {
                        if (memberDesignModel.fMaximumDesignRatio > s.fMaximumDesignRatioMainColumn)
                        {
                            s.fMaximumDesignRatioMainColumn = memberDesignModel.fMaximumDesignRatio;
                            s.GoverningLoadCombinationMainColumn = lcomb;
                            s.MaximumDesignRatioMainColumn = m;
                        }
                        break;
                    }
                case EMemberType_FS.eMR: // Main Rafter
                    {
                        if (memberDesignModel.fMaximumDesignRatio > s.fMaximumDesignRatioMainRafter)
                        {
                            s.fMaximumDesignRatioMainRafter = memberDesignModel.fMaximumDesignRatio;
                            s.GoverningLoadCombinationMainRafter = lcomb;
                            s.MaximumDesignRatioMainRafter = m;
                        }
                        break;
                    }
                case EMemberType_FS.eEC: // End Column
                    {
                        if (memberDesignModel.fMaximumDesignRatio > s.fMaximumDesignRatioEndColumn)
                        {
                            s.fMaximumDesignRatioEndColumn = memberDesignModel.fMaximumDesignRatio;
                            s.GoverningLoadCombinationEndColumn = lcomb;
                            s.MaximumDesignRatioEndColumn = m;
                        }
                        break;
                    }
                case EMemberType_FS.eER: // End Rafter
                    {
                        if (memberDesignModel.fMaximumDesignRatio > s.fMaximumDesignRatioEndRafter)
                        {
                            s.fMaximumDesignRatioEndRafter = memberDesignModel.fMaximumDesignRatio;
                            s.GoverningLoadCombinationEndRafter = lcomb;
                            s.MaximumDesignRatioEndRafter = m;
                        }
                        break;
                    }
                case EMemberType_FS.eG: // Girt
                    {
                        if (memberDesignModel.fMaximumDesignRatio > s.fMaximumDesignRatioGirts)
                        {
                            s.fMaximumDesignRatioGirts = memberDesignModel.fMaximumDesignRatio;
                            s.GoverningLoadCombinationGirts = lcomb;
                            s.MaximumDesignRatioGirt = m;
                        }
                        break;
                    }
                case EMemberType_FS.eP: // Purlin
                    {
                        if (memberDesignModel.fMaximumDesignRatio > s.fMaximumDesignRatioPurlins)
                        {
                            s.fMaximumDesignRatioPurlins = memberDesignModel.fMaximumDesignRatio;
                            s.GoverningLoadCombinationPurlins = lcomb;
                            s.MaximumDesignRatioPurlin = m;
                        }
                        break;
                    }
                case EMemberType_FS.eC: // Column
                    {
                        if (memberDesignModel.fMaximumDesignRatio > s.fMaximumDesignRatioColumns)
                        {
                            s.fMaximumDesignRatioColumns = memberDesignModel.fMaximumDesignRatio;
                            s.GoverningLoadCombinationColumns = lcomb;
                            s.MaximumDesignRatioColumn = m;
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

        private void SetMomentValuesforCb_design_And_BucklingFactors_Frame(float fx, int iFrameIndex, int lcombID, CMember member, ref designBucklingLengthFactors bucklingLengthFactors, ref designMomentValuesForCb sMomentValuesforCb_design)
        {
            // Create load combination (FEM solver object)
            BriefFiniteElementNet.LoadCombination lcomb = new BriefFiniteElementNet.LoadCombination();
            lcomb = ConvertLoadCombinationtoBFENet(Model.m_arrLoadCombs[lcombID - 1]);

            int iMemberIndex_FM = frameModels[iFrameIndex].GetMemberIndexInFrame(member);

            float fSegmentStart_abs, fSegmentEnd_abs;
            GetSegmentStartAndEndFor_xLocation(fx, member, lcombID, out fSegmentStart_abs, out fSegmentEnd_abs);
            GetSegmentBucklingFactors_xLocation(fx, member, lcombID, ref bucklingLengthFactors);
            sMomentValuesforCb_design = GetMomentValuesforCb_design_Segment(fSegmentStart_abs, fSegmentEnd_abs, lcomb,
            ((BriefFiniteElementNet.FrameElement2Node)(frameModels[iFrameIndex].BFEMNetModel.Elements[iMemberIndex_FM])));

            /*
            sMomentValuesforCb_design.fM_14 = frameModels[iFrameIndex].LoadCombInternalForcesResults[lcombID][member.ID].InternalForces[2].fM_yy;
            */
        }

        private void SetMomentValuesforCb_design_And_BucklingFactors_SimpleBeamSegment(float fx, int iSimpleBeamIndex, int lcombID, CMember member, ref designBucklingLengthFactors bucklingLengthFactors,  ref designMomentValuesForCb sMomentValuesforCb_design)
        {
            // Create load combination (FEM solver object)
            BriefFiniteElementNet.LoadCombination lcomb = new BriefFiniteElementNet.LoadCombination();
            lcomb = ConvertLoadCombinationtoBFENet(Model.m_arrLoadCombs[lcombID - 1]);

            float fSegmentStart_abs, fSegmentEnd_abs;
            GetSegmentStartAndEndFor_xLocation(fx, member, lcombID, out fSegmentStart_abs, out fSegmentEnd_abs);
            GetSegmentBucklingFactors_xLocation(fx, member, lcombID, ref bucklingLengthFactors);
            sMomentValuesforCb_design = GetMomentValuesforCb_design_Segment(fSegmentStart_abs, fSegmentEnd_abs, lcomb,
            ((BriefFiniteElementNet.FrameElement2Node)(beamSimpleModels[iSimpleBeamIndex].BFEMNetModel.Elements[0])));

            /*
            sMomentValuesforCb_design.fM_14 = beamSimpleModels[iSimpleBeamIndex].LoadCombInternalForcesResults[lcombID][member.ID].InternalForces[2].fM_yy;
            */
        }

        private designMomentValuesForCb GetMomentValuesforCb_design_Segment(
            float fSegmentStart_abs,
            float fSegmentEnd_abs,
            BriefFiniteElementNet.LoadCombination lcomb,
            BriefFiniteElementNet.FrameElement2Node memberBFENet)
        {
            float fSegmentLength = fSegmentEnd_abs - fSegmentStart_abs;
            BriefFiniteElementNet.Force f14 = memberBFENet.GetInternalForceAt(fSegmentStart_abs + 0.25 * fSegmentLength, lcomb);
            BriefFiniteElementNet.Force f24 = memberBFENet.GetInternalForceAt(fSegmentStart_abs + 0.50 * fSegmentLength, lcomb);
            BriefFiniteElementNet.Force f34 = memberBFENet.GetInternalForceAt(fSegmentStart_abs + 0.75 * fSegmentLength, lcomb);

            designMomentValuesForCb sMomentValuesforCb_design_segment;
            sMomentValuesforCb_design_segment.fM_14 = (float)Math.Abs(f14.My);
            sMomentValuesforCb_design_segment.fM_24 = (float)Math.Abs(f24.My);
            sMomentValuesforCb_design_segment.fM_34 = (float)Math.Abs(f34.My);
            sMomentValuesforCb_design_segment.fM_max = 0;

            for (int i = 0; i < iNumberOfDesignSections; i++)
            {
                float fx = fSegmentStart_abs + ((float)i / (float)iNumberOfSegments) * fSegmentLength;
                BriefFiniteElementNet.Force f = memberBFENet.GetInternalForceAt(fx, lcomb);

                if (Math.Abs(f.My) > sMomentValuesforCb_design_segment.fM_max)
                    sMomentValuesforCb_design_segment.fM_max = (float)Math.Abs(f.My);
            }

            return sMomentValuesforCb_design_segment;
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

        private void GetSegmentStartAndEndFor_xLocation(float fx, CMember member, int lcombID, out float fSegmentStart_abs, out float fSegmentEnd_abs)
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
                        if(member.LTBSegmentGroup[i].BucklingLengthFactors == null || member.LTBSegmentGroup[i].BucklingLengthFactors.Count == 0) // Default
                        {
                            bucklingLengthFactors = new designBucklingLengthFactors();
                            bucklingLengthFactors.fBeta_x_FB_fl_ex = 1.0f;
                            bucklingLengthFactors.fBeta_y_FB_fl_ey = 1.0f;
                            bucklingLengthFactors.fBeta_z_TB_TFB_l_ez = 1.0f;
                            bucklingLengthFactors.fBeta_LTB_fl_LTB = 1.0f;
                        }
                        else if(member.LTBSegmentGroup[i].BucklingLengthFactors.Count == 1) // Defined only once
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
            string txt = "";

            if (s.fMaximumDesignRatioWholeStructure > 0)
            {
                txt = "Calculation Results \n" +
                        "Limit State Type: " + s.sLimitStateType + "\n" +
                        "Maximum design ratio \n" +
                        "Member ID: " + s.MaximumDesignRatioWholeStructureMember.ID.ToString() + "\t" +
                        "Load Combination ID: " + s.GoverningLoadCombinationStructure.ID + "\t" +
                        "Design Ratio η: " + Math.Round(s.fMaximumDesignRatioWholeStructure, 3).ToString() +
                         "\n\n\n" +
                        "Maximum design ratio - main columns\n" +
                        "Member ID: " + s.MaximumDesignRatioMainColumn.ID.ToString() + "\t" +
                        "Load Combination ID: " + s.GoverningLoadCombinationMainColumn.ID + "\t" +
                        "Design Ratio η: " + Math.Round(s.fMaximumDesignRatioMainColumn, 3).ToString() +
                         "\n\n" +
                        "Maximum design ratio - rafters\n" +
                        "Member ID: " + s.MaximumDesignRatioMainRafter.ID.ToString() + "\t" +
                        "Load Combination ID: " + s.GoverningLoadCombinationMainRafter.ID + "\t" +
                        "Design Ratio η: " + Math.Round(s.fMaximumDesignRatioMainRafter, 3).ToString() +
                        "\n\n" +
                        "Maximum design ratio - end columns\n" +
                        "Member ID: " + s.MaximumDesignRatioEndColumn.ID.ToString() + "\t" +
                        "Load Combination ID: " + s.GoverningLoadCombinationEndColumn.ID + "\t" +
                        "Design Ratio η: " + Math.Round(s.fMaximumDesignRatioEndColumn, 3).ToString() +
                         "\n\n" +
                        "Maximum design ratio - end rafters\n" +
                        "Member ID: " + s.MaximumDesignRatioEndRafter.ID.ToString() + "\t" +
                        "Load Combination ID: " + s.GoverningLoadCombinationEndRafter.ID + "\t" +
                        "Design Ratio η: " + Math.Round(s.fMaximumDesignRatioEndRafter, 3).ToString() +
                         "\n\n" +
                        "Maximum design ratio - girts\n" +
                        "Member ID: " + s.MaximumDesignRatioGirt.ID.ToString() + "\t" +
                        "Load Combination ID: " + s.GoverningLoadCombinationGirts.ID + "\t" +
                        "Design Ratio η: " + Math.Round(s.fMaximumDesignRatioGirts, 3).ToString() +
                         "\n\n" +
                        "Maximum design ratio - purlins\n" +
                        "Member ID: " + s.MaximumDesignRatioPurlin.ID.ToString() + "\t" +
                        "Load Combination ID: " + s.GoverningLoadCombinationPurlins.ID + "\t" +
                        "Design Ratio η: " + Math.Round(s.fMaximumDesignRatioPurlins, 3).ToString() +
                         "\n\n" +
                        "Maximum design ratio - columns\n" +
                        "Member ID: " + s.MaximumDesignRatioColumn.ID.ToString() + "\t" +
                        "Load Combination ID: " + s.GoverningLoadCombinationColumns.ID + "\t" +
                        "Design Ratio η: " + Math.Round(s.fMaximumDesignRatioColumns, 3).ToString() +
                         "\n\n\n";
            }

            return txt;
        }

        private void ShowResultsInMessageBox(string txtULSandSLS, string txtULS, string txtSLS)
        {
            SolverWindow.ShowMessageBox(txtULSandSLS + txtULS + txtSLS);
        }
    }
}
