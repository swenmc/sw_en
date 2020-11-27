using BaseClasses;
using FEM_CALC_BASE;
using MATH;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFD.Infrastructure
{
    public class CMemberCalculations
    {
        //-------------------------------------------------------------------------------------------------------------------------------
        CMemberCalculationsAsyncStub stub = null;
        public delegate void CMemberCalculationsAsyncStub(CMember m, bool DeterminateCombinationResultsByFEMSolver, int iNumberOfDesignSections, int iNumberOfDesignSegments,
            float[] fx_positions, CModel_PFD Model, List<CFrame> frameModels, bool UseFEMSolverCalculationForSimpleBeam, List<CBeam_Simple> beamSimpleModels, 
            bool MUseCRSCGeometricalAxes, bool DeterminateMemberLocalDisplacementsForULS, int leftWallCrosses, int rightWallCrosses, int roofCrosses, float roofMassFactor);

        private Object theLock;

        public List<CMemberInternalForcesInLoadCases> MemberInternalForcesInLoadCases;
        public List<CMemberDeflectionsInLoadCases> MemberDeflectionsInLoadCases;
        public List<CMemberInternalForcesInLoadCombinations> MemberInternalForcesInLoadCombinations;
        public List<CMemberDeflectionsInLoadCombinations> MemberDeflectionsInLoadCombinations;

        //-------------------------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------------------------
        public CMemberCalculations(Object lockObject)
        {
            theLock = lockObject;
        }
        

        //-------------------------------------------------------------------------------------------------------------------------------
        public IAsyncResult BeginMemberCalculations(CMember m, bool DeterminateCombinationResultsByFEMSolver, int iNumberOfDesignSections, int iNumberOfDesignSegments,
            float[] fx_positions, CModel_PFD Model, List<CFrame> frameModels, bool UseFEMSolverCalculationForSimpleBeam, List<CBeam_Simple> beamSimpleModels, 
            bool MUseCRSCGeometricalAxes, bool DeterminateMemberLocalDisplacementsForULS, int leftWallCrosses, int rightWallCrosses, int roofCrosses, float roofMassFactor, AsyncCallback cb, object s)
        {
            stub = new CMemberCalculationsAsyncStub(Calculate);
            //using delegate for asynchronous implementation
            return stub.BeginInvoke(m, DeterminateCombinationResultsByFEMSolver, iNumberOfDesignSections, iNumberOfDesignSegments,
                    fx_positions, Model, frameModels, UseFEMSolverCalculationForSimpleBeam, beamSimpleModels, MUseCRSCGeometricalAxes, DeterminateMemberLocalDisplacementsForULS,
                    leftWallCrosses, rightWallCrosses, roofCrosses, roofMassFactor, cb, null);
        }

        //-------------------------------------------------------------------------------------------------------------------------------
        public void EndMemberCalculations(IAsyncResult call)
        {
            stub.EndInvoke(call);
        }

        public void Calculate(CMember m, bool DeterminateCombinationResultsByFEMSolver, int iNumberOfDesignSections, int iNumberOfDesignSegments,
            float[] fx_positions, CModel_PFD Model, List<CFrame> frameModels, bool UseFEMSolverCalculationForSimpleBeam, List<CBeam_Simple> beamSimpleModels, 
            bool MUseCRSCGeometricalAxes, bool DeterminateMemberLocalDisplacementsForULS, int leftWallCrosses, int rightWallCrosses, int roofCrosses, float roofMassFactor)
        {
            Calculate_InternalForcesAndDeflections_LoadCases(m, DeterminateCombinationResultsByFEMSolver, iNumberOfDesignSections, iNumberOfDesignSegments,
                fx_positions, Model, frameModels, UseFEMSolverCalculationForSimpleBeam, beamSimpleModels, MUseCRSCGeometricalAxes, leftWallCrosses, rightWallCrosses, roofCrosses, roofMassFactor);

            Calculate_InternalForces_LoadCombinations(m, DeterminateCombinationResultsByFEMSolver, iNumberOfDesignSections, iNumberOfDesignSegments,
                fx_positions, Model, frameModels, beamSimpleModels, MUseCRSCGeometricalAxes);

            Calculate_Deflections_LoadCombinations(m, DeterminateCombinationResultsByFEMSolver, iNumberOfDesignSections, iNumberOfDesignSegments,
                fx_positions, Model, frameModels, beamSimpleModels, MUseCRSCGeometricalAxes, DeterminateMemberLocalDisplacementsForULS);
        }

        public void Calculate_InternalForcesAndDeflections_LoadCases(CMember m, bool DeterminateCombinationResultsByFEMSolver, int iNumberOfDesignSections, int iNumberOfDesignSegments,
            float[] fx_positions, CModel_PFD Model, List<CFrame> frameModels, bool UseFEMSolverCalculationForSimpleBeam, List<CBeam_Simple> beamSimpleModels, bool MUseCRSCGeometricalAxes,
            int leftWallCrosses, int rightWallCrosses, int roofCrosses, float roofMassFactor)
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
                        if (m.EMemberType_FEM != EMemberType_FEM.Tension)
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
                        else // Tension only
                        {
                            //---------------------------------------------------------------------------------------------
                            //---------------------------------------------------------------------------------------------
                            //---------------------------------------------------------------------------------------------

                            // Vypocet vnutornych sil v cross-bracing                            

                            // Alternativna ale pracnejsia moznost je, ze toto cele by sa vysunuho pred klasicky vypocet vnutornych sil a zaviedlo by sa cosi ako
                            // "inicializacne" vnutorne sily
                            // Tie by sa dali nastavit roznym prutom pre jednotlive load cases este pred spustenim vypoctu
                            // a v ramci vypoctu by sa k nim pridali vnutorne sily, ktore pocitame teraz

                            // Predstav si to ako nieco co vznikne napriklad už pri stavbe budovy,
                            // napriklad uz pri montazi cross-bracing member napneme prut tak aby bol priamy, a je v nom osova sila N= 10 kN
                            // Potom pri vypocte vetra sa k tomu prida dalsich 50 kN od vetra, takze vysledna sila v load case Wind by bolo 60 kN

                            sBucklingLengthFactors = new designBucklingLengthFactors[iNumberOfDesignSections];
                            sMomentValuesforCb = new designMomentValuesForCb[iNumberOfDesignSections];

                            sBIF_x = new basicInternalForces[iNumberOfDesignSections];
                            //sBIF_x[0].fN = 1000f;
                            //sBIF_x[1].fN = 1000f;
                            //sBIF_x[2].fN = 1000f;
                            //sBIF_x[3].fN = 1000f;
                            //sBIF_x[4].fN = 1000f;
                            //sBIF_x[5].fN = 1000f;
                            //sBIF_x[6].fN = 1000f;
                            //sBIF_x[7].fN = 1000f;
                            //sBIF_x[8].fN = 1000f;
                            //sBIF_x[9].fN = 1000f;
                            //sBIF_x[10].fN = 1000f; // TODO 633 Docasne hodnoty, ked bude vsetko napojene a funkcne tak by sa mali tieto hodnoty odstraniť

                            // Cross-bracing - Wind in + /- Y direction
                            if (m.EMemberTypePosition == EMemberType_FS_Position.CrossBracingWall || m.EMemberTypePosition == EMemberType_FS_Position.CrossBracingRoof)
                            {
                                float fN_left = 0, fN_right = 0; // Axial force in member (left or right side)

                                float angle_rad = (float)Math.Atan(m.Delta_Z / m.Delta_Y); // Uhol sklonu cross-bracing member od horizontály - ziskat z priemetu pruta do GCS Z a Y

                                // Number of wall side bracing crosses (pocet krizov na jednej strane)
                                int iNumberOfActiveCrosses_left = leftWallCrosses;
                                int iNumberOfActiveCrosses_right = rightWallCrosses;
                                // Uvazujeme rovnomerne rozdelenie do jednotlivych krizov, neplati to vsak pre dlhe budovy
                                
                                // Roof cross-bracing - active bays - uvazovat len tie bays ktore maju krize na celej streche a ktore maju aj krize na oboch stranach side wall bracing
                                int iNumberOfActiveBays_RoofBracing = roofCrosses;
                                int iNumberOfActiveBays = Math.Min(Math.Min(iNumberOfActiveCrosses_left, iNumberOfActiveCrosses_right), iNumberOfActiveBays_RoofBracing);

                                // Pomocny vypocet - vietor alebo zemetrasenie
                                if (lc.Type == ELCType.eWind || lc.Type == ELCType.eEarthquake)
                                {
                                    // Wind
                                    if (lc.Type == ELCType.eWind && (lc.MainDirection == ELCMainDirection.ePlusY || lc.MainDirection == ELCMainDirection.eMinusY))
                                    {
                                        // Wind load - Vietor
                                        // Wall cross-bracing
                                        float fp_tot;

                                        List<CSLoad_Free> surfaceLoads = lc.SurfaceLoadsList;

                                        if (surfaceLoads == null)
                                            fp_tot = 0;

                                        if (lc.LC_Wind_Type == ELCWindType.eWL_Cpe_max || lc.LC_Wind_Type == ELCWindType.eWL_Cpe_min)
                                        {
                                            // Windward - W
                                            float fp_e_W = Math.Abs(((CSLoad_FreeUniform)surfaceLoads[4]).fValue); // Pa - tlak vetra na stenu (front alebo back)
                                            // Leeward - L
                                            float fp_e_L = Math.Abs(((CSLoad_FreeUniform)surfaceLoads[5]).fValue); // Pa - sanie vetra na stenu (front alebo back)

                                            if (lc.MainDirection == ELCMainDirection.eMinusY)
                                            {
                                                fp_e_W = Math.Abs(((CSLoad_FreeUniform)surfaceLoads[5]).fValue);
                                                fp_e_L = Math.Abs(((CSLoad_FreeUniform)surfaceLoads[4]).fValue);
                                            }

                                            fp_tot = Math.Abs(fp_e_W) + Math.Abs(fp_e_L);
                                        }
                                        else
                                        {
                                            // Internal pressure
                                            fp_tot = Math.Abs(((CSLoad_FreeUniform)surfaceLoads[0]).fValue);
                                        }

                                        float fA_trib_left, fA_trib_right;

                                        if (Model.eKitset == EModelType_FS.eKitsetMonoRoofEnclosed)
                                        {
                                            // Pre monopitch je A left a A right rozdielne podla vysky
                                            fA_trib_left = Model.fW_frame_overall * 0.5f * (Model.fH1_frame_overall + (Model.fH1_frame_overall + Model.fH2_frame_overall) / 2f) / 2f;
                                            fA_trib_right = Model.fW_frame_overall * 0.5f * (Model.fH2_frame_overall + (Model.fH1_frame_overall + Model.fH2_frame_overall) / 2f) / 2f;
                                        }
                                        else if (Model.eKitset == EModelType_FS.eKitsetGableRoofEnclosed)
                                        {
                                            // Area - Left Side
                                            fA_trib_left = Model.fW_frame_overall * 0.5f * (Model.fH1_frame_overall + Model.fH2_frame_overall) / 2f; // Polovica plochy cela budovy
                                            // Area - Right Side
                                            fA_trib_right = Model.fW_frame_overall * 0.5f * (Model.fH1_frame_overall + Model.fH2_frame_overall) / 2f; // Polovica plochy cela budovy
                                        }
                                        else
                                        {
                                            throw new NotImplementedException("Building model is not implemented.");
                                        }

                                        // Force in corner of edge frame
                                        float fW_left = fA_trib_left * fp_tot;
                                        float fW_right = fA_trib_right * fp_tot;

                                        // Force in side wall cross bracing member
                                        fN_left = Math.Abs((fA_trib_left * fp_tot / iNumberOfActiveCrosses_left) * (float)Math.Cos(angle_rad));
                                        fN_right = Math.Abs((fA_trib_right * fp_tot / iNumberOfActiveCrosses_right) * (float)Math.Cos(angle_rad));

                                        //-------------------------------------------------------------------------------------------------------------------------------
                                        // Roof cross-bracing
                                        // Pocet aktivnych bays je minimum pre walls vlavo, vpravo a roof

                                        if (m.EMemberTypePosition == EMemberType_FS_Position.CrossBracingRoof)
                                        {
                                            float fWidth_trib_left, fWidth_trib_right;
                                            float fR_left, fR_right;

                                            if (Model.eKitset == EModelType_FS.eKitsetMonoRoofEnclosed)
                                            {
                                                // Pre monopitch je Width left a A right rozdielne podla vysky
                                                fWidth_trib_left = 0.5f * Model.fH1_frame_overall;
                                                fWidth_trib_right = 0.5f * Model.fH2_frame_overall;
                                                float fWidth_trib_middle = 0.5f * (Model.fH1_frame_overall + Model.fH2_frame_overall); // Polovica výšky čela budovy

                                                // Linear Loads
                                                float fq_wind_left = fp_tot * fWidth_trib_left;
                                                float fq_wind_right = fp_tot * fWidth_trib_right;
                                                float fq_wind_middle = fp_tot * fWidth_trib_middle;

                                                fR_left = 0.5f * (fq_wind_left + fq_wind_middle) * 0.5f * Model.fW_frame_overall;
                                                fR_right = 0.5f * (fq_wind_right + fq_wind_middle) * 0.5f * Model.fW_frame_overall;
                                            }
                                            else if (Model.eKitset == EModelType_FS.eKitsetGableRoofEnclosed)
                                            {
                                                // Tributary width - Left Side
                                                fWidth_trib_left = 0.5f * Model.fH1_frame_overall; // Polovica výšky čela budovy
                                                                                                   // Tributary width - Right Side
                                                fWidth_trib_right = 0.5f * Model.fH1_frame_overall; // Polovica výšky čela budovy
                                                                                                    // Tributary width - Middle - Apex (Ridge)
                                                float fWidth_trib_ridge = 0.5f * Model.fH2_frame_overall; // Polovica výšky čela budovy

                                                // Linear Loads
                                                float fq_wind_left = fp_tot * fWidth_trib_left;
                                                float fq_wind_right = fp_tot * fWidth_trib_right;
                                                float fq_wind_ridge = fp_tot * fWidth_trib_ridge;

                                                fR_left = 0.5f * (fq_wind_left + fq_wind_ridge) * 0.5f * Model.fW_frame_overall;
                                                fR_right = 0.5f * (fq_wind_right + fq_wind_ridge) * 0.5f * Model.fW_frame_overall;
                                            }
                                            else
                                            {
                                                throw new NotImplementedException("Building model is not implemented.");
                                            }

                                            angle_rad = (float)Math.Atan(m.Delta_X / m.Delta_Y); // Uhol sklonu cross-bracing member - ziskat z priemetu pruta do GCS X a Y

                                            // Force in end cross bracing member
                                            fN_left = Math.Abs((fR_left / iNumberOfActiveBays) * (float)Math.Cos(angle_rad));
                                            fN_right = Math.Abs((fR_right / iNumberOfActiveBays) * (float)Math.Cos(angle_rad));
                                        }
                                    }

                                    // Earthquake

                                    // Side cross-bracing
                                    if (lc.Type == ELCType.eEarthquake && (lc.MainDirection == ELCMainDirection.ePlusY || lc.MainDirection == ELCMainDirection.eMinusY))
                                    {
                                        float fE_left; // N - EQ
                                        float fE_right; // N - EQ

                                        List<CNLoad> nodalLoads = lc.NodeLoadsList;

                                        if (nodalLoads == null)
                                        { fE_left = 0; fE_right = 0; }
                                        else
                                        {
                                            fE_left = ((CNLoadSingle)nodalLoads[0]).Value;
                                            fE_right = ((CNLoadSingle)nodalLoads[1]).Value;
                                        }

                                        fN_left = Math.Abs((fE_left / iNumberOfActiveCrosses_left) * (float)Math.Cos(angle_rad));
                                        fN_right = Math.Abs((fE_right / iNumberOfActiveCrosses_right) * (float)Math.Cos(angle_rad));

                                        if (m.EMemberTypePosition == EMemberType_FS_Position.CrossBracingWall)
                                        {
                                            // Roof cross-bracing

                                            // Dopocitat zo sil indikovanych hmotnostou strechy
                                            // Tu by sme sa potrebovali dostat k hmotnosti strechy, ktora sa pouziva pre vypocet zatazovacich sil v EQ load cases
                                            // Tento vypocet je v MainWindow.xaml.cs na riadku 762
                                            // Potrebujeme pre cross-bracing urcit aka cast hmotnosti prislucha k zatazeniu ktore je na roof bracing, takze rafters, eave purlins, purlins a roof
                                            // !!!!!!!!!!!!!!! Docasne uvazujem 10 % zo zatazenia pouziteho pre wall cross-bracing
                                            //float fRoofMassFactorTemp = 0.1f;
                                            float fRoofMassFactorTemp = roofMassFactor;

                                            angle_rad = (float)Math.Atan(m.Delta_X / m.Delta_Y); // Uhol sklonu cross-bracing member - ziskat z priemetu pruta do GCS X a Y

                                            // Force in end cross bracing member
                                            fN_left = fRoofMassFactorTemp * Math.Abs((fE_left / iNumberOfActiveBays) * (float)Math.Cos(angle_rad));
                                            fN_right = fRoofMassFactorTemp * Math.Abs((fE_right / iNumberOfActiveBays) * (float)Math.Cos(angle_rad));
                                        }
                                    }
                                }

                                if (MathF.d_equal(m.NodeStart.X, 0) && MathF.d_equal(m.NodeEnd.X, 0))
                                {
                                    sBIF_x[0].fN = fN_left;
                                    sBIF_x[1].fN = fN_left;
                                    sBIF_x[2].fN = fN_left;
                                    sBIF_x[3].fN = fN_left;
                                    sBIF_x[4].fN = fN_left;
                                    sBIF_x[5].fN = fN_left;
                                    sBIF_x[6].fN = fN_left;
                                    sBIF_x[7].fN = fN_left;
                                    sBIF_x[8].fN = fN_left;
                                    sBIF_x[9].fN = fN_left;
                                    sBIF_x[10].fN = fN_left;
                                }
                                else
                                {
                                    sBIF_x[0].fN = fN_right;
                                    sBIF_x[1].fN = fN_right;
                                    sBIF_x[2].fN = fN_right;
                                    sBIF_x[3].fN = fN_right;
                                    sBIF_x[4].fN = fN_right;
                                    sBIF_x[5].fN = fN_right;
                                    sBIF_x[6].fN = fN_right;
                                    sBIF_x[7].fN = fN_right;
                                    sBIF_x[8].fN = fN_right;
                                    sBIF_x[9].fN = fN_right;
                                    sBIF_x[10].fN = fN_right;
                                }
                            }

                            sBDeflections_x = new basicDeflections[iNumberOfDesignSections];

                            //---------------------------------------------------------------------------------------------
                            //---------------------------------------------------------------------------------------------
                            //---------------------------------------------------------------------------------------------
                        }
                    }

                    //To Mato - tieto vysledky by sa nemali tak nahodou na zaciatku tohto cyklu nastavovat na null??? 
                    // podla mna to funguje tak,ze ked sa nastavia sBIF_x pre LoadCase index 1 tak aj keby index 2 uz hodnoty sBIF_x nenastavil,tak sa ulozia lebo uz to nebude sBIF_x null
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


        public void Calculate_InternalForces_LoadCombinations(CMember m, bool DeterminateCombinationResultsByFEMSolver, int iNumberOfDesignSections, int iNumberOfDesignSegments,
            float[] fx_positions, CModel_PFD Model, List<CFrame> frameModels, List<CBeam_Simple> beamSimpleModels, bool MUseCRSCGeometricalAxes)
        {
            MemberInternalForcesInLoadCombinations = new List<CMemberInternalForcesInLoadCombinations>();

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
                            SetMomentValuesforCb_design_And_BucklingFactors_Frame(fx_positions[j], iFrameIndex, lcomb.ID, m, Model, frameModels, iNumberOfDesignSections, iNumberOfDesignSegments, ref sBucklingLengthFactors_temp, ref sMomentValuesforCb_temp);

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
                            SetMomentValuesforCb_design_And_BucklingFactors_SimpleBeamSegment(fx_positions[j], iSimpleBeamIndex, lcomb.ID, m, Model, beamSimpleModels, iNumberOfDesignSections, iNumberOfDesignSegments, ref sBucklingLengthFactors_temp, ref sMomentValuesforCb_temp);

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

        private void SetMomentValuesforCb_design_And_BucklingFactors_Frame(float fx, int iFrameIndex, int lcombID, CMember member, CModel_PFD Model, List<CFrame> frameModels, int iNumberOfDesignSections, int iNumberOfDesignSegments, ref designBucklingLengthFactors bucklingLengthFactors, ref designMomentValuesForCb sMomentValuesforCb_design)
        {
            // Create load combination (FEM solver object)
            BriefFiniteElementNet.LoadCombination lcomb = new BriefFiniteElementNet.LoadCombination();
            lcomb = ConvertLoadCombinationtoBFENet(Model.m_arrLoadCombs[lcombID - 1], Model);

            int iMemberIndex_FM = frameModels[iFrameIndex].GetMemberIndexInFrame(member);

            float fSegmentStart_abs, fSegmentEnd_abs;
            GetSegmentStartAndEndFor_xLocation(fx, member, out fSegmentStart_abs, out fSegmentEnd_abs);
            GetSegmentBucklingFactors_xLocation(fx, member, lcombID, ref bucklingLengthFactors);
            sMomentValuesforCb_design = GetMomentValuesforCb_design_Segment(fSegmentStart_abs, fSegmentEnd_abs, iNumberOfDesignSections, iNumberOfDesignSegments, lcomb,
            ((BriefFiniteElementNet.FrameElement2Node)(frameModels[iFrameIndex].BFEMNetModel.Elements[iMemberIndex_FM])));

            /*
            sMomentValuesforCb_design.fM_14 = frameModels[iFrameIndex].LoadCombInternalForcesResults[lcombID][member.ID].InternalForces[2].fM_yy;
            */
        }

        private designMomentValuesForCb GetMomentValuesforCb_design_Segment(float fSegmentStart_abs, float fSegmentEnd_abs, int iNumberOfDesignSections, int iNumberOfDesignSegments,
            BriefFiniteElementNet.LoadCombination lcomb, BriefFiniteElementNet.FrameElement2Node memberBFENet)
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

        private void SetMomentValuesforCb_design_And_BucklingFactors_SimpleBeamSegment(float fx, int iSimpleBeamIndex, int lcombID, CMember member, CModel_PFD Model, List<CBeam_Simple> beamSimpleModels, int iNumberOfDesignSections, int iNumberOfDesignSegments, ref designBucklingLengthFactors bucklingLengthFactors, ref designMomentValuesForCb sMomentValuesforCb_design)
        {
            if (iSimpleBeamIndex < 0)
            {
                //To Mato - V Load Cases som zaskrtol druhe option a tu sa mi dostalo iSimpleBeamIndex = -1;
                // To Ondrej - Vyskusal som to a prebehlo to bez problemov
                return;
            }
            // Create load combination (FEM solver object)
            BriefFiniteElementNet.LoadCombination lcomb = new BriefFiniteElementNet.LoadCombination();
            lcomb = ConvertLoadCombinationtoBFENet(Model.m_arrLoadCombs[lcombID - 1], Model);

            float fSegmentStart_abs, fSegmentEnd_abs;
            GetSegmentStartAndEndFor_xLocation(fx, member, out fSegmentStart_abs, out fSegmentEnd_abs);
            GetSegmentBucklingFactors_xLocation(fx, member, lcombID, ref bucklingLengthFactors);
            sMomentValuesforCb_design = GetMomentValuesforCb_design_Segment(fSegmentStart_abs, fSegmentEnd_abs, iNumberOfDesignSections, iNumberOfDesignSegments, lcomb,
            ((BriefFiniteElementNet.FrameElement2Node)(beamSimpleModels[iSimpleBeamIndex].BFEMNetModel.Elements[0])));

            /*
            sMomentValuesforCb_design.fM_14 = beamSimpleModels[iSimpleBeamIndex].LoadCombInternalForcesResults[lcombID][member.ID].InternalForces[2].fM_yy;
            */
        }


        public void Calculate_Deflections_LoadCombinations(CMember m, bool DeterminateCombinationResultsByFEMSolver, int iNumberOfDesignSections, int iNumberOfDesignSegments,
            float[] fx_positions, CModel_PFD Model, List<CFrame> frameModels, List<CBeam_Simple> beamSimpleModels, bool MUseCRSCGeometricalAxes, bool DeterminateMemberLocalDisplacementsForULS)
        {
            MemberDeflectionsInLoadCombinations = new List<CMemberDeflectionsInLoadCombinations>();

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

    }
}
