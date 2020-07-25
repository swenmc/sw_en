using BaseClasses;
using CRSC;
using M_EC1.AS_NZS;
using MATH;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media.Media3D;

namespace PFD
{
    public struct MemberLoadParameters
    {
        public ELoadDirection eMemberLoadDirection;
        public float fSurfaceLoadValueFactor;
        public float fMemberLoadValueSign;
    }

    public class CMemberLoadGenerator
    {
        private int iFrameNodesNo;
        private int iEavesPurlinNoInOneFrame;
        private int iFramesNo;
        //private float fL1_frame;
        private List<float> m_L1_Bays;
        private float fL_tot;
        private CLoadCase[] m_arrLoadCases;
        private CMember[] m_arrMembers;

        CCrSc GirtCrSc;
        CCrSc PurlinCrSc;
        float fDistanceGirts;
        float fDistancePurlins;
        CCrSc ColumnCrSc;
        CCrSc RafterCrSc;
        CCrSc ColumnCrSc_EF;
        CCrSc RafterCrSc_EF;

        float fValueLoadColumnDead;
        float fValueLoadRafterDead;
        float fValueLoadRafterImposed;
        float fValueLoadRafterSnowULS_Nu_1;
        float fValueLoadRafterSnowULS_Nu_2;
        float fValueLoadRafterSnowSLS_Nu_1;
        float fValueLoadRafterSnowSLS_Nu_2;
        CCalcul_1170_2 wind;

        bool bConsiderFactors_Kci_Kce_Ka_Generator = true; // Zohladnuje faktory Kci, Kce a Ka az v generatore zatazenia

        public CMemberLoadGenerator() // Pouzije sa ak negenerujeme zatazenie na ramoch
        { }

        // Pouzije sa ak generujeme zatazenie na ramoch
        public CMemberLoadGenerator(
            int frameNodesNo,
            int eavesPurlinNoInOneFrame,
            int framesNo,
            //float L1_frame,
            List<float> L1_Bays,
            float L_tot,
            float fSlopeFactor,
            CCrSc GirtCrSc_temp,
            CCrSc PurlinCrSc_temp,
            float fDistanceGirts_temp,
            float fDistancePurlins_temp,
            CCrSc ColumnCrSc_temp,
            CCrSc RafterCrSc_temp,
            CCrSc ColumnCrSc_EF_temp,
            CCrSc RafterCrSc_EF_temp,
            CLoadCase[] arrLoadCases,
            CMember[] arrMembers,
            CCalcul_1170_1 generalLoad,
            CCalcul_1170_3 snow,
            CCalcul_1170_2 calc_wind)
        {
            iFrameNodesNo = frameNodesNo;
            iEavesPurlinNoInOneFrame =eavesPurlinNoInOneFrame;
            iFramesNo = framesNo;
            //fL1_frame = L1_frame;
            m_L1_Bays = L1_Bays;
            fL_tot = L_tot;
            m_arrLoadCases = arrLoadCases;
            m_arrMembers = arrMembers;

            GirtCrSc = GirtCrSc_temp;
            PurlinCrSc = PurlinCrSc_temp;
            fDistanceGirts = fDistanceGirts_temp;
            fDistancePurlins = fDistancePurlins_temp;
            ColumnCrSc = ColumnCrSc_temp;
            RafterCrSc = RafterCrSc_temp;
            ColumnCrSc_EF = ColumnCrSc_EF_temp;
            RafterCrSc_EF = RafterCrSc_EF_temp;

            fValueLoadColumnDead = -generalLoad.fDeadLoadTotal_Wall;
            fValueLoadRafterDead = -generalLoad.fDeadLoadTotal_Roof;
            fValueLoadRafterImposed = -generalLoad.fImposedLoadTotal_Roof;

            // Snow Load - Roof
            fValueLoadRafterSnowULS_Nu_1 = -snow.fs_ULS_Nu_1 * fSlopeFactor; // Design value (projection on roof)
            fValueLoadRafterSnowULS_Nu_2 = -snow.fs_ULS_Nu_2 * fSlopeFactor;
            fValueLoadRafterSnowSLS_Nu_1 = -snow.fs_SLS_Nu_1 * fSlopeFactor;
            fValueLoadRafterSnowSLS_Nu_2 = -snow.fs_SLS_Nu_2 * fSlopeFactor;

            wind = calc_wind;
        }

        public CMemberLoadGenerator(CModel_PFD model, CCalcul_1170_1 generalLoad, CCalcul_1170_3 snow, CCalcul_1170_2 calc_wind)
        {
            iFrameNodesNo = model.iFrameNodesNo;
            iEavesPurlinNoInOneFrame = model.iEavesPurlinNoInOneFrame;
            iFramesNo = model.iFrameNo;
            //fL1_frame = model.fL1_frame;
            m_L1_Bays = model.L1_Bays;
            fL_tot = model.fL_tot;
            m_arrLoadCases = model.m_arrLoadCases;
            m_arrMembers = model.m_arrMembers;

            GirtCrSc = model.m_arrCrSc[(int)EMemberGroupNames.eGirtWall];
            PurlinCrSc = model.m_arrCrSc[(int)EMemberGroupNames.ePurlin];
            fDistanceGirts = model.fDist_Girt;
            fDistancePurlins = model.fDist_Purlin;
            ColumnCrSc = model.m_arrCrSc[(int)EMemberGroupNames.eMainColumn];
            RafterCrSc = model.m_arrCrSc[(int)EMemberGroupNames.eRafter];
            ColumnCrSc_EF = model.m_arrCrSc[(int)EMemberGroupNames.eMainColumn_EF];
            RafterCrSc_EF = model.m_arrCrSc[(int)EMemberGroupNames.eRafter_EF];

            fValueLoadColumnDead = -generalLoad.fDeadLoadTotal_Wall;
            fValueLoadRafterDead = -generalLoad.fDeadLoadTotal_Roof;
            fValueLoadRafterImposed = -generalLoad.fImposedLoadTotal_Roof;

            // Snow Load - Roof
            fValueLoadRafterSnowULS_Nu_1 = -snow.fs_ULS_Nu_1 * model.fSlopeFactor; // Design value (projection on roof)
            fValueLoadRafterSnowULS_Nu_2 = -snow.fs_ULS_Nu_2 * model.fSlopeFactor;
            fValueLoadRafterSnowSLS_Nu_1 = -snow.fs_SLS_Nu_1 * model.fSlopeFactor;
            fValueLoadRafterSnowSLS_Nu_2 = -snow.fs_SLS_Nu_2 * model.fSlopeFactor;

            wind = calc_wind;

            // Validation of options
            if (bConsiderFactors_Kci_Kce_Ka_Generator == true &&
                (wind.bConsiderAreaReductionFactor_Ka == true || wind.bConsiderCombinationFactor_Kci_and_Kce == true))
            {
                // Ak uz boli K_ca, Kce alebo Kci redukovane, tak je nastavenie pre tento generator nevalidne
                throw new Exception("Wind pressure reduction factor Kci, Kce or Ka was already considered. Check options.");
            }
        }

        // Loading

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Generate frame loads directly from load values
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        // Frame member loads
        public LoadCasesMemberLoads GetGenerateMemberLoadsOnFrames()
        {
            //List<List<CMLoad>> listOfMemberLoadLists = new List<List<CMLoad>>();
            LoadCasesMemberLoads loadCasesMemberLoads = new LoadCasesMemberLoads();

            List<CMLoad> memberLoadDead = new List<CMLoad>();
            List<CMLoad> memberLoadImposed = new List<CMLoad>();

            List<CMLoad> memberMaxLoadSnowAll_ULS = new List<CMLoad>();
            List<CMLoad> memberMaxLoadSnowLeft_ULS = new List<CMLoad>();
            List<CMLoad> memberMaxLoadSnowRight_ULS = new List<CMLoad>();
            List<CMLoad> memberLoadInternalPressure_ULS_Cpimin_Left = new List<CMLoad>();
            List<CMLoad> memberLoadInternalPressure_ULS_Cpimin_Right = new List<CMLoad>();
            List<CMLoad> memberLoadInternalPressure_ULS_Cpimin_Front = new List<CMLoad>();
            List<CMLoad> memberLoadInternalPressure_ULS_Cpimin_Rear = new List<CMLoad>();
            List<CMLoad> memberLoadInternalPressure_ULS_Cpimax_Left = new List<CMLoad>();
            List<CMLoad> memberLoadInternalPressure_ULS_Cpimax_Right = new List<CMLoad>();
            List<CMLoad> memberLoadInternalPressure_ULS_Cpimax_Front = new List<CMLoad>();
            List<CMLoad> memberLoadInternalPressure_ULS_Cpimax_Rear = new List<CMLoad>();
            List<CMLoad> memberLoadExternalPressure_ULS_Cpemin_Left = new List<CMLoad>();
            List<CMLoad> memberLoadExternalPressure_ULS_Cpemin_Right = new List<CMLoad>();
            List<CMLoad> memberLoadExternalPressure_ULS_Cpemin_Front = new List<CMLoad>();
            List<CMLoad> memberLoadExternalPressure_ULS_Cpemin_Rear = new List<CMLoad>();
            List<CMLoad> memberLoadExternalPressure_ULS_Cpemax_Left = new List<CMLoad>();
            List<CMLoad> memberLoadExternalPressure_ULS_Cpemax_Right = new List<CMLoad>();
            List<CMLoad> memberLoadExternalPressure_ULS_Cpemax_Front = new List<CMLoad>();
            List<CMLoad> memberLoadExternalPressure_ULS_Cpemax_Rear = new List<CMLoad>();

            List<CMLoad> memberLoad_ULS_EQ_X_Plus_Left = new List<CMLoad>(); // TODO - Empty
            List<CMLoad> memberLoad_ULS_EQ_Y_Plus_Front = new List<CMLoad>(); // TODO - Empty

            List<CMLoad> memberMaxLoadSnowAll_SLS = new List<CMLoad>();
            List<CMLoad> memberMaxLoadSnowLeft_SLS = new List<CMLoad>();
            List<CMLoad> memberMaxLoadSnowRight_SLS = new List<CMLoad>();
            List<CMLoad> memberLoadInternalPressure_SLS_Cpimin_Left = new List<CMLoad>();
            List<CMLoad> memberLoadInternalPressure_SLS_Cpimin_Right = new List<CMLoad>();
            List<CMLoad> memberLoadInternalPressure_SLS_Cpimin_Front = new List<CMLoad>();
            List<CMLoad> memberLoadInternalPressure_SLS_Cpimin_Rear = new List<CMLoad>();
            List<CMLoad> memberLoadInternalPressure_SLS_Cpimax_Left = new List<CMLoad>();
            List<CMLoad> memberLoadInternalPressure_SLS_Cpimax_Right = new List<CMLoad>();
            List<CMLoad> memberLoadInternalPressure_SLS_Cpimax_Front = new List<CMLoad>();
            List<CMLoad> memberLoadInternalPressure_SLS_Cpimax_Rear = new List<CMLoad>();
            List<CMLoad> memberLoadExternalPressure_SLS_Cpemin_Left = new List<CMLoad>();
            List<CMLoad> memberLoadExternalPressure_SLS_Cpemin_Right = new List<CMLoad>();
            List<CMLoad> memberLoadExternalPressure_SLS_Cpemin_Front = new List<CMLoad>();
            List<CMLoad> memberLoadExternalPressure_SLS_Cpemin_Rear = new List<CMLoad>();
            List<CMLoad> memberLoadExternalPressure_SLS_Cpemax_Left = new List<CMLoad>();
            List<CMLoad> memberLoadExternalPressure_SLS_Cpemax_Right = new List<CMLoad>();
            List<CMLoad> memberLoadExternalPressure_SLS_Cpemax_Front = new List<CMLoad>();
            List<CMLoad> memberLoadExternalPressure_SLS_Cpemax_Rear = new List<CMLoad>();

            List<CMLoad> memberLoad_SLS_EQ_X_Plus_Left = new List<CMLoad>(); // TODO - Empty
            List<CMLoad> memberLoad_SLS_EQ_Y_Plus_Front = new List<CMLoad>(); // TODO - Empty

            for (int i = 0; i < iFramesNo; i++)
            {
                // Generate loads on member of particular frame
                GenerateLoadsOnFrame(i,
                iFrameNodesNo,
                iEavesPurlinNoInOneFrame,
                fValueLoadColumnDead,
                fValueLoadRafterDead,
                GirtCrSc,
                PurlinCrSc,
                fDistanceGirts,
                fDistancePurlins,
                ColumnCrSc,
                RafterCrSc,
                ColumnCrSc_EF,
                RafterCrSc_EF,

                fValueLoadRafterImposed,
                fValueLoadRafterSnowULS_Nu_1,
                fValueLoadRafterSnowULS_Nu_2,
                fValueLoadRafterSnowSLS_Nu_1,
                fValueLoadRafterSnowSLS_Nu_2,
                wind,
                ref memberLoadDead,
                ref memberLoadImposed,

                ref memberMaxLoadSnowAll_ULS,
                ref memberMaxLoadSnowLeft_ULS,
                ref memberMaxLoadSnowRight_ULS,
                ref memberLoadInternalPressure_ULS_Cpimin_Left,
                ref memberLoadInternalPressure_ULS_Cpimin_Right,
                ref memberLoadInternalPressure_ULS_Cpimin_Front,
                ref memberLoadInternalPressure_ULS_Cpimin_Rear,
                ref memberLoadInternalPressure_ULS_Cpimax_Left,
                ref memberLoadInternalPressure_ULS_Cpimax_Right,
                ref memberLoadInternalPressure_ULS_Cpimax_Front,
                ref memberLoadInternalPressure_ULS_Cpimax_Rear,
                ref memberLoadExternalPressure_ULS_Cpemin_Left,
                ref memberLoadExternalPressure_ULS_Cpemin_Right,
                ref memberLoadExternalPressure_ULS_Cpemin_Front,
                ref memberLoadExternalPressure_ULS_Cpemin_Rear,
                ref memberLoadExternalPressure_ULS_Cpemax_Left,
                ref memberLoadExternalPressure_ULS_Cpemax_Right,
                ref memberLoadExternalPressure_ULS_Cpemax_Front,
                ref memberLoadExternalPressure_ULS_Cpemax_Rear,

                ref memberMaxLoadSnowAll_SLS,
                ref memberMaxLoadSnowLeft_SLS,
                ref memberMaxLoadSnowRight_SLS,
                ref memberLoadInternalPressure_SLS_Cpimin_Left,
                ref memberLoadInternalPressure_SLS_Cpimin_Right,
                ref memberLoadInternalPressure_SLS_Cpimin_Front,
                ref memberLoadInternalPressure_SLS_Cpimin_Rear,
                ref memberLoadInternalPressure_SLS_Cpimax_Left,
                ref memberLoadInternalPressure_SLS_Cpimax_Right,
                ref memberLoadInternalPressure_SLS_Cpimax_Front,
                ref memberLoadInternalPressure_SLS_Cpimax_Rear,
                ref memberLoadExternalPressure_SLS_Cpemin_Left,
                ref memberLoadExternalPressure_SLS_Cpemin_Right,
                ref memberLoadExternalPressure_SLS_Cpemin_Front,
                ref memberLoadExternalPressure_SLS_Cpemin_Rear,
                ref memberLoadExternalPressure_SLS_Cpemax_Left,
                ref memberLoadExternalPressure_SLS_Cpemax_Right,
                ref memberLoadExternalPressure_SLS_Cpemax_Front,
                ref memberLoadExternalPressure_SLS_Cpemax_Rear
                );
            }

            // Add list to the output list
            loadCasesMemberLoads.Add((int)ELCName.eDL_G + 1, memberLoadDead);
            loadCasesMemberLoads.Add((int)ELCName.eIL_Q + 1, memberLoadImposed);

            loadCasesMemberLoads.Add((int)ELCName.eSL_Su_Full + 1, memberMaxLoadSnowAll_ULS);
            loadCasesMemberLoads.Add((int)ELCName.eSL_Su_Left + 1, memberMaxLoadSnowLeft_ULS);
            loadCasesMemberLoads.Add((int)ELCName.eSL_Su_Right + 1, memberMaxLoadSnowRight_ULS);
            loadCasesMemberLoads.Add((int)ELCName.eWL_Wu_Cpi_min_Left_X_Plus + 1, memberLoadInternalPressure_ULS_Cpimin_Left);
            loadCasesMemberLoads.Add((int)ELCName.eWL_Wu_Cpi_min_Right_X_Minus + 1, memberLoadInternalPressure_ULS_Cpimin_Right);
            loadCasesMemberLoads.Add((int)ELCName.eWL_Wu_Cpi_min_Front_Y_Plus + 1, memberLoadInternalPressure_ULS_Cpimin_Front);
            loadCasesMemberLoads.Add((int)ELCName.eWL_Wu_Cpi_min_Rear_Y_Minus + 1, memberLoadInternalPressure_ULS_Cpimin_Rear);
            loadCasesMemberLoads.Add((int)ELCName.eWL_Wu_Cpi_max_Left_X_Plus + 1, memberLoadInternalPressure_ULS_Cpimax_Left);
            loadCasesMemberLoads.Add((int)ELCName.eWL_Wu_Cpi_max_Right_X_Minus + 1, memberLoadInternalPressure_ULS_Cpimax_Right);
            loadCasesMemberLoads.Add((int)ELCName.eWL_Wu_Cpi_max_Front_Y_Plus + 1, memberLoadInternalPressure_ULS_Cpimax_Front);
            loadCasesMemberLoads.Add((int)ELCName.eWL_Wu_Cpi_max_Rear_Y_Minus + 1, memberLoadInternalPressure_ULS_Cpimax_Rear);
            loadCasesMemberLoads.Add((int)ELCName.eWL_Wu_Cpe_min_Left_X_Plus + 1, memberLoadExternalPressure_ULS_Cpemin_Left);
            loadCasesMemberLoads.Add((int)ELCName.eWL_Wu_Cpe_min_Right_X_Minus + 1, memberLoadExternalPressure_ULS_Cpemin_Right);
            loadCasesMemberLoads.Add((int)ELCName.eWL_Wu_Cpe_min_Front_Y_Plus + 1, memberLoadExternalPressure_ULS_Cpemin_Front);
            loadCasesMemberLoads.Add((int)ELCName.eWL_Wu_Cpe_min_Rear_Y_Minus + 1, memberLoadExternalPressure_ULS_Cpemin_Rear);
            loadCasesMemberLoads.Add((int)ELCName.eWL_Wu_Cpe_max_Left_X_Plus + 1, memberLoadExternalPressure_ULS_Cpemax_Left);
            loadCasesMemberLoads.Add((int)ELCName.eWL_Wu_Cpe_max_Right_X_Minus + 1, memberLoadExternalPressure_ULS_Cpemax_Right);
            loadCasesMemberLoads.Add((int)ELCName.eWL_Wu_Cpe_max_Front_Y_Plus + 1, memberLoadExternalPressure_ULS_Cpemax_Front);
            loadCasesMemberLoads.Add((int)ELCName.eWL_Wu_Cpe_max_Rear_Y_Minus + 1, memberLoadExternalPressure_ULS_Cpemax_Rear);

            loadCasesMemberLoads.Add((int)ELCName.eEQ_Eu_Left_X_Plus + 1, memberLoad_ULS_EQ_X_Plus_Left); // TODO - Empty
            loadCasesMemberLoads.Add((int)ELCName.eEQ_Eu_Front_Y_Plus + 1, memberLoad_ULS_EQ_Y_Plus_Front); // TODO - Empty

            loadCasesMemberLoads.Add((int)ELCName.eSL_Ss_Full + 1, memberMaxLoadSnowAll_SLS);
            loadCasesMemberLoads.Add((int)ELCName.eSL_Ss_Left + 1, memberMaxLoadSnowLeft_SLS);
            loadCasesMemberLoads.Add((int)ELCName.eSL_Ss_Right + 1, memberMaxLoadSnowRight_SLS);
            loadCasesMemberLoads.Add((int)ELCName.eWL_Ws_Cpi_min_Left_X_Plus + 1, memberLoadInternalPressure_SLS_Cpimin_Left);
            loadCasesMemberLoads.Add((int)ELCName.eWL_Ws_Cpi_min_Right_X_Minus + 1, memberLoadInternalPressure_SLS_Cpimin_Right);
            loadCasesMemberLoads.Add((int)ELCName.eWL_Ws_Cpi_min_Front_Y_Plus + 1, memberLoadInternalPressure_SLS_Cpimin_Front);
            loadCasesMemberLoads.Add((int)ELCName.eWL_Ws_Cpi_min_Rear_Y_Minus + 1, memberLoadInternalPressure_SLS_Cpimin_Rear);
            loadCasesMemberLoads.Add((int)ELCName.eWL_Ws_Cpi_max_Left_X_Plus + 1, memberLoadInternalPressure_SLS_Cpimax_Left);
            loadCasesMemberLoads.Add((int)ELCName.eWL_Ws_Cpi_max_Right_X_Minus + 1, memberLoadInternalPressure_SLS_Cpimax_Right);
            loadCasesMemberLoads.Add((int)ELCName.eWL_Ws_Cpi_max_Front_Y_Plus + 1, memberLoadInternalPressure_SLS_Cpimax_Front);
            loadCasesMemberLoads.Add((int)ELCName.eWL_Ws_Cpi_max_Rear_Y_Minus + 1, memberLoadInternalPressure_SLS_Cpimax_Rear);
            loadCasesMemberLoads.Add((int)ELCName.eWL_Ws_Cpe_min_Left_X_Plus + 1, memberLoadExternalPressure_SLS_Cpemin_Left);
            loadCasesMemberLoads.Add((int)ELCName.eWL_Ws_Cpe_min_Right_X_Minus + 1, memberLoadExternalPressure_SLS_Cpemin_Right);
            loadCasesMemberLoads.Add((int)ELCName.eWL_Ws_Cpe_min_Front_Y_Plus + 1, memberLoadExternalPressure_SLS_Cpemin_Front);
            loadCasesMemberLoads.Add((int)ELCName.eWL_Ws_Cpe_min_Rear_Y_Minus + 1, memberLoadExternalPressure_SLS_Cpemin_Rear);
            loadCasesMemberLoads.Add((int)ELCName.eWL_Ws_Cpe_max_Left_X_Plus + 1, memberLoadExternalPressure_SLS_Cpemax_Left);
            loadCasesMemberLoads.Add((int)ELCName.eWL_Ws_Cpe_max_Right_X_Minus + 1, memberLoadExternalPressure_SLS_Cpemax_Right);
            loadCasesMemberLoads.Add((int)ELCName.eWL_Ws_Cpe_max_Front_Y_Plus + 1, memberLoadExternalPressure_SLS_Cpemax_Front);
            loadCasesMemberLoads.Add((int)ELCName.eWL_Ws_Cpe_max_Rear_Y_Minus + 1, memberLoadExternalPressure_SLS_Cpemax_Rear);

            loadCasesMemberLoads.Add((int)ELCName.eEQ_Es_Left_X_Plus + 1, memberLoad_SLS_EQ_X_Plus_Left); // TODO - Empty
            loadCasesMemberLoads.Add((int)ELCName.eEQ_Es_Front_Y_Plus + 1, memberLoad_SLS_EQ_Y_Plus_Front); // TODO - Empty

            return loadCasesMemberLoads;
        }

        public void AssignMemberLoadListsToLoadCases(LoadCasesMemberLoads loadCasesMemberLoads)
        {
            foreach (CLoadCase lc in m_arrLoadCases)
            {
                lc.MemberLoadsList = loadCasesMemberLoads[lc.ID];
            }
        }

        public void GenerateLoadsOnFrame(
            int iFrameIndex,
            int iFrameNodesNo,
            int iEavesPurlinNoInOneFrame,
            float fValueLoadWallCladdingSelfWeight_SurfaceLoad,
            float fValueLoadRoofCladdingSelfWeight_SurfaceLoad,
            CCrSc GirtCrSc,
            CCrSc PurlinCrSc,
            float fDistanceGirts,
            float fDistancePurlins,
            CCrSc ColumnCrSc,
            CCrSc RafterCrSc,
            CCrSc ColumnCrSc_EF,
            CCrSc RafterCrSc_EF,
            float fValueLoadRafterImposed,
            float fValueLoadRafterSnowULS_Nu_1,
            float fValueLoadRafterSnowULS_Nu_2,
            float fValueLoadRafterSnowSLS_Nu_1,
            float fValueLoadRafterSnowSLS_Nu_2,
            CCalcul_1170_2 wind,
            ref List<CMLoad> memberLoadDead,
            ref List<CMLoad> memberLoadImposed,

            ref List<CMLoad> memberMaxLoadSnowAll_ULS,
            ref List<CMLoad> memberMaxLoadSnowLeft_ULS,
            ref List<CMLoad> memberMaxLoadSnowRight_ULS,

            ref List<CMLoad> memberLoadInternalPressure_ULS_Cpimin_Left,
            ref List<CMLoad> memberLoadInternalPressure_ULS_Cpimin_Right,
            ref List<CMLoad> memberLoadInternalPressure_ULS_Cpimin_Front,
            ref List<CMLoad> memberLoadInternalPressure_ULS_Cpimin_Rear,

            ref List<CMLoad> memberLoadInternalPressure_ULS_Cpimax_Left,
            ref List<CMLoad> memberLoadInternalPressure_ULS_Cpimax_Right,
            ref List<CMLoad> memberLoadInternalPressure_ULS_Cpimax_Front,
            ref List<CMLoad> memberLoadInternalPressure_ULS_Cpimax_Rear,

            ref List<CMLoad> memberLoadExternalPressure_ULS_Cpemin_Left,
            ref List<CMLoad> memberLoadExternalPressure_ULS_Cpemin_Right,
            ref List<CMLoad> memberLoadExternalPressure_ULS_Cpemin_Front,
            ref List<CMLoad> memberLoadExternalPressure_ULS_Cpemin_Rear,

            ref List<CMLoad> memberLoadExternalPressure_ULS_Cpemax_Left,
            ref List<CMLoad> memberLoadExternalPressure_ULS_Cpemax_Right,
            ref List<CMLoad> memberLoadExternalPressure_ULS_Cpemax_Front,
            ref List<CMLoad> memberLoadExternalPressure_ULS_Cpemax_Rear,

            ref List<CMLoad> memberMaxLoadSnowAll_SLS,
            ref List<CMLoad> memberMaxLoadSnowLeft_SLS,
            ref List<CMLoad> memberMaxLoadSnowRight_SLS,

            ref List<CMLoad> memberLoadInternalPressure_SLS_Cpimin_Left,
            ref List<CMLoad> memberLoadInternalPressure_SLS_Cpimin_Right,
            ref List<CMLoad> memberLoadInternalPressure_SLS_Cpimin_Front,
            ref List<CMLoad> memberLoadInternalPressure_SLS_Cpimin_Rear,

            ref List<CMLoad> memberLoadInternalPressure_SLS_Cpimax_Left,
            ref List<CMLoad> memberLoadInternalPressure_SLS_Cpimax_Right,
            ref List<CMLoad> memberLoadInternalPressure_SLS_Cpimax_Front,
            ref List<CMLoad> memberLoadInternalPressure_SLS_Cpimax_Rear,

            ref List<CMLoad> memberLoadExternalPressure_SLS_Cpemin_Left,
            ref List<CMLoad> memberLoadExternalPressure_SLS_Cpemin_Right,
            ref List<CMLoad> memberLoadExternalPressure_SLS_Cpemin_Front,
            ref List<CMLoad> memberLoadExternalPressure_SLS_Cpemin_Rear,

            ref List<CMLoad> memberLoadExternalPressure_SLS_Cpemax_Left,
            ref List<CMLoad> memberLoadExternalPressure_SLS_Cpemax_Right,
            ref List<CMLoad> memberLoadExternalPressure_SLS_Cpemax_Front,
            ref List<CMLoad> memberLoadExternalPressure_SLS_Cpemax_Rear
            )
        {
            bool bIsGableRoof = iFrameNodesNo == 5;

            int iNumberOfColumnsInFrame = 2;
            int iNumberOfEavePurlinsInFrame = 2;
            int iNumberOfRaftersInFrame = 2;

            if (!bIsGableRoof)
                iNumberOfRaftersInFrame = 1;

            int iFrameMembersNo = iFrameNodesNo - 1;

            int indexColumn1Left = (iFrameIndex * iEavesPurlinNoInOneFrame) + iFrameIndex * iFrameMembersNo + 0;
            int indexColumn2Right = (iFrameIndex * iEavesPurlinNoInOneFrame) + iFrameIndex * iFrameMembersNo + (iFrameMembersNo - 1);
            int indexRafter1Left = (iFrameIndex * iEavesPurlinNoInOneFrame) + iFrameIndex * iFrameMembersNo + 1;
            int indexRafter2Right = (iFrameIndex * iEavesPurlinNoInOneFrame) + iFrameIndex * iFrameMembersNo + (iFrameMembersNo - 1 - 1);

            // Additional surface dead load
            float fSelfWeight_Girts_SurfaceLoad = -(float)(GirtCrSc.A_g * GirtCrSc.m_Mat.m_fRho * GlobalConstants.G_ACCELERATION / fDistanceGirts);
            float fSelfWeight_Purlins_SurfaceLoad = -(float)(PurlinCrSc.A_g * PurlinCrSc.m_Mat.m_fRho * GlobalConstants.G_ACCELERATION / fDistancePurlins);

            float fSelfWeightColumn = -(float)(ColumnCrSc.A_g * ColumnCrSc.m_Mat.m_fRho * GlobalConstants.G_ACCELERATION);
            float fSelfWeightRafter = -(float)(RafterCrSc.A_g * RafterCrSc.m_Mat.m_fRho * GlobalConstants.G_ACCELERATION);

            //task 600
            //float fFrameTributaryWidth = fL1_frame;
            //float fFrameGCSCoordinate_Y = iFrameIndex * fL1_frame;
            float fFrameTributaryWidth = 0;
            float fFrameGCSCoordinate_Y = GetBaysWidthUntilFrameIndex(iFrameIndex);

            // Half tributary width - first and last frame
            if (iFrameIndex == 0 || iFrameIndex == iFramesNo - 1)
            {
                if (iFrameIndex == 0) fFrameTributaryWidth = m_L1_Bays.First();
                else fFrameTributaryWidth = m_L1_Bays.Last();

                fFrameTributaryWidth *= 0.5f;
                fSelfWeightColumn = -(float)(ColumnCrSc_EF.A_g * ColumnCrSc_EF.m_Mat.m_fRho * GlobalConstants.G_ACCELERATION);
                fSelfWeightRafter = -(float)(RafterCrSc_EF.A_g * RafterCrSc_EF.m_Mat.m_fRho * GlobalConstants.G_ACCELERATION);
            }
            else
            {
                fFrameTributaryWidth = 0.5f * m_L1_Bays[iFrameIndex - 1] + 0.5f * m_L1_Bays[iFrameIndex];
            }

            // Total surface dead load
            float fValueLoadColumnDead_Surface = fValueLoadWallCladdingSelfWeight_SurfaceLoad + fSelfWeight_Girts_SurfaceLoad;
            float fValueLoadRafterDead_Surface = fValueLoadRoofCladdingSelfWeight_SurfaceLoad + fSelfWeight_Purlins_SurfaceLoad;

            // Final uniform linear member load
            float fValueLoadColumnDead = fValueLoadColumnDead_Surface * fFrameTributaryWidth + fSelfWeightColumn;
            float fValueLoadRafterDead = fValueLoadRafterDead_Surface * fFrameTributaryWidth + fSelfWeightRafter;

            // Dead Loads
            // Columns
            CMLoad loadColumnLeft_DL = new CMLoad_21(iFrameIndex * iFrameMembersNo + 1, fValueLoadColumnDead, m_arrMembers[indexColumn1Left], EMLoadTypeDistr.eMLT_QUF_W_21, ELoadType.eLT_F, ELoadCoordSystem.eLCS, ELoadDirection.eLD_X, true, 0);
            // Osovy system praveho stlpa smeruje zhora nadol, takze hodnota zatazenia v LCS je s opacnym znamienkom (* -1)
            CMLoad loadColumnRight_DL = new CMLoad_21(iFrameIndex * iFrameMembersNo + 2, -fValueLoadColumnDead, m_arrMembers[indexColumn2Right], EMLoadTypeDistr.eMLT_QUF_W_21, ELoadType.eLT_F, ELoadCoordSystem.eLCS, ELoadDirection.eLD_X, true, 0);
            memberLoadDead.Add(loadColumnLeft_DL);
            memberLoadDead.Add(loadColumnRight_DL);

            // Rafters
            // TODO - zapracovat do konstruktora nastavenie GCS smeru zatazenia, teraz je to nespravne v PCS
            CMLoad loadRafterLeft_DL = new CMLoad_21(iFrameIndex * iFrameMembersNo + 3, fValueLoadRafterDead, m_arrMembers[indexRafter1Left], EMLoadTypeDistr.eMLT_QUF_W_21, ELoadType.eLT_F, ELoadCoordSystem.eLCS, ELoadDirection.eLD_Z, true, 0);
            memberLoadDead.Add(loadRafterLeft_DL);

            if (bIsGableRoof)
            {
                CMLoad loadRafterRight_DL = new CMLoad_21(iFrameIndex * iFrameMembersNo + 4, fValueLoadRafterDead, m_arrMembers[indexRafter2Right], EMLoadTypeDistr.eMLT_QUF_W_21, ELoadType.eLT_F, ELoadCoordSystem.eLCS, ELoadDirection.eLD_Z, true, 0);
                memberLoadDead.Add(loadRafterRight_DL);
            }

            // Imposed Loads - roof
            // Rafters
            // TODO - zapracovat do konstruktora nastavenie GCS smeru zatazenia, teraz je to nespravne v PCS
            CMLoad loadRafterLeft_IL = new CMLoad_21(iFrameIndex * 2 + 1, fValueLoadRafterImposed * fFrameTributaryWidth, m_arrMembers[1 + iFrameIndex * (iNumberOfColumnsInFrame + iNumberOfRaftersInFrame + iNumberOfEavePurlinsInFrame)], EMLoadTypeDistr.eMLT_QUF_W_21, ELoadType.eLT_F, ELoadCoordSystem.eLCS, ELoadDirection.eLD_Z, true, 0);
            memberLoadImposed.Add(loadRafterLeft_IL);

            if (bIsGableRoof)
            {
                CMLoad loadRafterRight_IL = new CMLoad_21(iFrameIndex * 2 + 2, fValueLoadRafterImposed * fFrameTributaryWidth, m_arrMembers[1 + iFrameIndex * (iNumberOfColumnsInFrame + iNumberOfRaftersInFrame + iNumberOfEavePurlinsInFrame) + 1], EMLoadTypeDistr.eMLT_QUF_W_21, ELoadType.eLT_F, ELoadCoordSystem.eLCS, ELoadDirection.eLD_Z, true, 0);
                memberLoadImposed.Add(loadRafterRight_IL);
            }

            // Snow Loads - roof
            // Rafters
            // TODO - zapracovat do konstruktora nastavenie GCS smeru zatazenia, teraz je to nespravne v PCS
            CMLoad loadRafterLeft_SL1_All_ULS = new CMLoad_21(iFrameIndex * 2 + 1, fValueLoadRafterSnowULS_Nu_1 * fFrameTributaryWidth, m_arrMembers[indexRafter1Left], EMLoadTypeDistr.eMLT_QUF_W_21, ELoadType.eLT_F, ELoadCoordSystem.eLCS, ELoadDirection.eLD_Z, true, 0);
            memberMaxLoadSnowAll_ULS.Add(loadRafterLeft_SL1_All_ULS);

            if (bIsGableRoof)
            {
                CMLoad loadRafterRight_SL1_All_ULS = new CMLoad_21(iFrameIndex * 2 + 2, fValueLoadRafterSnowULS_Nu_1 * fFrameTributaryWidth, m_arrMembers[indexRafter2Right], EMLoadTypeDistr.eMLT_QUF_W_21, ELoadType.eLT_F, ELoadCoordSystem.eLCS, ELoadDirection.eLD_Z, true, 0);
                memberMaxLoadSnowAll_ULS.Add(loadRafterRight_SL1_All_ULS);
            }

            // Rafters
            float fSnowLoadOnRafterLengthMultiplier = 1;
            if (!bIsGableRoof)
                fSnowLoadOnRafterLengthMultiplier = 0.5f;

            // TODO - zapracovat do konstruktora nastavenie GCS smeru zatazenia, teraz je to nespravne v PCS
            CMLoad loadRafterLeft_SL2_Left_ULS = new CMLoad_22(iFrameIndex + 1, fValueLoadRafterSnowULS_Nu_2 * fFrameTributaryWidth, fSnowLoadOnRafterLengthMultiplier * m_arrMembers[indexRafter1Left].FLength, m_arrMembers[indexRafter1Left], EMLoadTypeDistr.eMLT_QUF_PA_22, ELoadType.eLT_F, ELoadCoordSystem.eLCS, ELoadDirection.eLD_Z, true, 0);
            memberMaxLoadSnowLeft_ULS.Add(loadRafterLeft_SL2_Left_ULS);

            // Rafters
            // TODO - zapracovat do konstruktora nastavenie GCS smeru zatazenia, teraz je to nespravne v PCS
            CMLoad loadRafterRight_SL3_Right_ULS = new CMLoad_23(iFrameIndex + 1, fValueLoadRafterSnowULS_Nu_2 * fFrameTributaryWidth, fSnowLoadOnRafterLengthMultiplier * m_arrMembers[indexRafter2Right].FLength, m_arrMembers[indexRafter2Right], EMLoadTypeDistr.eMLT_QUF_PB_23, ELoadType.eLT_F, ELoadCoordSystem.eLCS, ELoadDirection.eLD_Z, true, 0);
            memberMaxLoadSnowRight_ULS.Add(loadRafterRight_SL3_Right_ULS);

            // Wind Loads
            // Internal Pressure
            // ULS
            // Cpi,min
            SetFrameMembersWindLoads_InternalPressure(
                        iFrameIndex,
                        indexColumn1Left,
                        indexColumn2Right,
                        indexRafter1Left,
                        indexRafter2Right,
                        (int)ELCMainDirection.ePlusX,
                        fFrameTributaryWidth,
                        ELSType.eLS_ULS,
                        ELCWindType.eWL_Cpi_min,
                        wind,
                ref memberLoadInternalPressure_ULS_Cpimin_Left);

            SetFrameMembersWindLoads_InternalPressure(
                        iFrameIndex,
                        indexColumn1Left,
                        indexColumn2Right,
                        indexRafter1Left,
                        indexRafter2Right,
                        (int)ELCMainDirection.eMinusX,
                        fFrameTributaryWidth,
                        ELSType.eLS_ULS,
                        ELCWindType.eWL_Cpi_min,
                        wind,
                ref memberLoadInternalPressure_ULS_Cpimin_Right);

            SetFrameMembersWindLoads_InternalPressure(
                        iFrameIndex,
                        indexColumn1Left,
                        indexColumn2Right,
                        indexRafter1Left,
                        indexRafter2Right,
                        (int)ELCMainDirection.ePlusY,
                        fFrameTributaryWidth,
                        ELSType.eLS_ULS,
                        ELCWindType.eWL_Cpi_min,
                        wind,
                ref memberLoadInternalPressure_ULS_Cpimin_Front);

            SetFrameMembersWindLoads_InternalPressure(
                        iFrameIndex,
                        indexColumn1Left,
                        indexColumn2Right,
                        indexRafter1Left,
                        indexRafter2Right,
                        (int)ELCMainDirection.eMinusY,
                        fFrameTributaryWidth,
                        ELSType.eLS_ULS,
                        ELCWindType.eWL_Cpi_min,
                        wind,
                ref memberLoadInternalPressure_ULS_Cpimin_Rear);

            // Cpi,max
            SetFrameMembersWindLoads_InternalPressure(
                        iFrameIndex,
                        indexColumn1Left,
                        indexColumn2Right,
                        indexRafter1Left,
                        indexRafter2Right,
                        (int)ELCMainDirection.ePlusX,
                        fFrameTributaryWidth,
                        ELSType.eLS_ULS,
                        ELCWindType.eWL_Cpi_max,
                        wind,
                ref memberLoadInternalPressure_ULS_Cpimax_Left);

            SetFrameMembersWindLoads_InternalPressure(
                        iFrameIndex,
                        indexColumn1Left,
                        indexColumn2Right,
                        indexRafter1Left,
                        indexRafter2Right,
                        (int)ELCMainDirection.eMinusX,
                        fFrameTributaryWidth,
                        ELSType.eLS_ULS,
                        ELCWindType.eWL_Cpi_max,
                        wind,
                ref memberLoadInternalPressure_ULS_Cpimax_Right);

            SetFrameMembersWindLoads_InternalPressure(
                        iFrameIndex,
                        indexColumn1Left,
                        indexColumn2Right,
                        indexRafter1Left,
                        indexRafter2Right,
                        (int)ELCMainDirection.ePlusY,
                        fFrameTributaryWidth,
                        ELSType.eLS_ULS,
                        ELCWindType.eWL_Cpi_max,
                        wind,
                ref memberLoadInternalPressure_ULS_Cpimax_Front);

            SetFrameMembersWindLoads_InternalPressure(
                        iFrameIndex,
                        indexColumn1Left,
                        indexColumn2Right,
                        indexRafter1Left,
                        indexRafter2Right,
                        (int)ELCMainDirection.eMinusY,
                        fFrameTributaryWidth,
                        ELSType.eLS_ULS,
                        ELCWindType.eWL_Cpi_max,
                        wind,
                ref memberLoadInternalPressure_ULS_Cpimax_Rear);

            // External Pressure
            // ULS
            // Cpe,min
            SetFrameMembersWindLoads_LeftOrRight(iFrameIndex,
                        indexColumn1Left,
                        indexColumn2Right,
                        indexRafter1Left,
                        indexRafter2Right,
                        (int)ELCMainDirection.ePlusX,
                        fFrameTributaryWidth,
                        ELSType.eLS_ULS,
                        ELCWindType.eWL_Cpe_min,
                        wind,
                        ref memberLoadExternalPressure_ULS_Cpemin_Left);

            SetFrameMembersWindLoads_LeftOrRight(iFrameIndex,
                        indexColumn1Left,
                        indexColumn2Right,
                        indexRafter1Left,
                        indexRafter2Right,
                        (int)ELCMainDirection.eMinusX,
                        fFrameTributaryWidth,
                        ELSType.eLS_ULS,
                        ELCWindType.eWL_Cpe_min,
                        wind,
                        ref memberLoadExternalPressure_ULS_Cpemin_Right);


            SetFrameMembersWindLoads_FrontOrRear(iFrameIndex,
                        indexColumn1Left,
                        indexColumn2Right,
                        indexRafter1Left,
                        indexRafter2Right,
                        (int)ELCMainDirection.ePlusY,
                        fFrameTributaryWidth,
                        ELSType.eLS_ULS,
                        ELCWindType.eWL_Cpe_min,
                        wind,
                        ref memberLoadExternalPressure_ULS_Cpemin_Front);

            SetFrameMembersWindLoads_FrontOrRear(iFrameIndex,
                        indexColumn1Left,
                        indexColumn2Right,
                        indexRafter1Left,
                        indexRafter2Right,
                        (int)ELCMainDirection.eMinusY,
                        fFrameTributaryWidth,
                        ELSType.eLS_ULS,
                        ELCWindType.eWL_Cpe_min,
                        wind,
                        ref memberLoadExternalPressure_ULS_Cpemin_Rear);

            // Cpe,max
            SetFrameMembersWindLoads_LeftOrRight(iFrameIndex,
                        indexColumn1Left,
                        indexColumn2Right,
                        indexRafter1Left,
                        indexRafter2Right,
                        (int)ELCMainDirection.ePlusX,
                        fFrameTributaryWidth,
                        ELSType.eLS_ULS,
                        ELCWindType.eWL_Cpe_max,
                        wind,
                        ref memberLoadExternalPressure_ULS_Cpemax_Left);

            SetFrameMembersWindLoads_LeftOrRight(iFrameIndex,
                        indexColumn1Left,
                        indexColumn2Right,
                        indexRafter1Left,
                        indexRafter2Right,
                        (int)ELCMainDirection.eMinusX,
                        fFrameTributaryWidth,
                        ELSType.eLS_ULS,
                        ELCWindType.eWL_Cpe_max,
                        wind,
                        ref memberLoadExternalPressure_ULS_Cpemax_Right);

            SetFrameMembersWindLoads_FrontOrRear(iFrameIndex,
                        indexColumn1Left,
                        indexColumn2Right,
                        indexRafter1Left,
                        indexRafter2Right,
                        (int)ELCMainDirection.ePlusY,
                        fFrameTributaryWidth,
                        ELSType.eLS_ULS,
                        ELCWindType.eWL_Cpe_max,
                        wind,
                        ref memberLoadExternalPressure_ULS_Cpemax_Front);

            SetFrameMembersWindLoads_FrontOrRear(iFrameIndex,
                        indexColumn1Left,
                        indexColumn2Right,
                        indexRafter1Left,
                        indexRafter2Right,
                        (int)ELCMainDirection.eMinusY,
                        fFrameTributaryWidth,
                        ELSType.eLS_ULS,
                        ELCWindType.eWL_Cpe_max,
                        wind,
                        ref memberLoadExternalPressure_ULS_Cpemax_Rear);

            // Snow Loads - roof
            // Rafters
            // TODO - zapracovat do konstruktora nastavenie GCS smeru zatazenia, teraz je to nespravne v PCS
            CMLoad loadRafterLeft_SL1_All_SLS = new CMLoad_21(iFrameIndex * 2 + 1, fValueLoadRafterSnowULS_Nu_1 * fFrameTributaryWidth, m_arrMembers[indexRafter1Left], EMLoadTypeDistr.eMLT_QUF_W_21, ELoadType.eLT_F, ELoadCoordSystem.eLCS, ELoadDirection.eLD_Z, true, 0);
            memberMaxLoadSnowAll_SLS.Add(loadRafterLeft_SL1_All_SLS);

            if (bIsGableRoof)
            {
                CMLoad loadRafterRight_SL1_All_SLS = new CMLoad_21(iFrameIndex * 2 + 2, fValueLoadRafterSnowSLS_Nu_1 * fFrameTributaryWidth, m_arrMembers[indexRafter2Right], EMLoadTypeDistr.eMLT_QUF_W_21, ELoadType.eLT_F, ELoadCoordSystem.eLCS, ELoadDirection.eLD_Z, true, 0);
                memberMaxLoadSnowAll_SLS.Add(loadRafterRight_SL1_All_SLS);
            }

            // Rafters
            // TODO - zapracovat do konstruktora nastavenie GCS smeru zatazenia, teraz je to nespravne v PCS
            CMLoad loadRafterLeft_SL2_Left_SLS = new CMLoad_22(iFrameIndex + 1, fValueLoadRafterSnowSLS_Nu_2 * fFrameTributaryWidth, fSnowLoadOnRafterLengthMultiplier * m_arrMembers[indexRafter1Left].FLength, m_arrMembers[indexRafter1Left], EMLoadTypeDistr.eMLT_QUF_PA_22, ELoadType.eLT_F, ELoadCoordSystem.eLCS, ELoadDirection.eLD_Z, true, 0);
            memberMaxLoadSnowLeft_SLS.Add(loadRafterLeft_SL2_Left_SLS);

            // Rafters
            // TODO - zapracovat do konstruktora nastavenie GCS smeru zatazenia, teraz je to nespravne v PCS
            CMLoad loadRafterRight_SL3_Right_SLS = new CMLoad_23(iFrameIndex + 1, fValueLoadRafterSnowSLS_Nu_2 * fFrameTributaryWidth, fSnowLoadOnRafterLengthMultiplier * m_arrMembers[indexRafter2Right].FLength, m_arrMembers[indexRafter2Right], EMLoadTypeDistr.eMLT_QUF_PB_23, ELoadType.eLT_F, ELoadCoordSystem.eLCS, ELoadDirection.eLD_Z, true, 0);
            memberMaxLoadSnowRight_SLS.Add(loadRafterRight_SL3_Right_SLS);

            // Wind Loads
            // Internal Pressure
            // SLS
            // Cpi,min
            SetFrameMembersWindLoads_InternalPressure(
                        iFrameIndex,
                        indexColumn1Left,
                        indexColumn2Right,
                        indexRafter1Left,
                        indexRafter2Right,
                        (int)ELCMainDirection.ePlusX,
                        fFrameTributaryWidth,
                        ELSType.eLS_SLS,
                        ELCWindType.eWL_Cpi_min,
                        wind,
                ref memberLoadInternalPressure_SLS_Cpimin_Left);

            SetFrameMembersWindLoads_InternalPressure(
                        iFrameIndex,
                        indexColumn1Left,
                        indexColumn2Right,
                        indexRafter1Left,
                        indexRafter2Right,
                        (int)ELCMainDirection.eMinusX,
                        fFrameTributaryWidth,
                        ELSType.eLS_SLS,
                        ELCWindType.eWL_Cpi_min,
                        wind,
                ref memberLoadInternalPressure_SLS_Cpimin_Right);

            SetFrameMembersWindLoads_InternalPressure(
                        iFrameIndex,
                        indexColumn1Left,
                        indexColumn2Right,
                        indexRafter1Left,
                        indexRafter2Right,
                        (int)ELCMainDirection.ePlusY,
                        fFrameTributaryWidth,
                        ELSType.eLS_SLS,
                        ELCWindType.eWL_Cpi_min,
                        wind,
                ref memberLoadInternalPressure_SLS_Cpimin_Front);

            SetFrameMembersWindLoads_InternalPressure(
                        iFrameIndex,
                        indexColumn1Left,
                        indexColumn2Right,
                        indexRafter1Left,
                        indexRafter2Right,
                        (int)ELCMainDirection.eMinusY,
                        fFrameTributaryWidth,
                        ELSType.eLS_SLS,
                        ELCWindType.eWL_Cpi_min,
                        wind,
                ref memberLoadInternalPressure_SLS_Cpimin_Rear);

            // Cpi,max
            SetFrameMembersWindLoads_InternalPressure(
                        iFrameIndex,
                        indexColumn1Left,
                        indexColumn2Right,
                        indexRafter1Left,
                        indexRafter2Right,
                        (int)ELCMainDirection.ePlusX,
                        fFrameTributaryWidth,
                        ELSType.eLS_SLS,
                        ELCWindType.eWL_Cpi_max,
                        wind,
                ref memberLoadInternalPressure_SLS_Cpimax_Left);

            SetFrameMembersWindLoads_InternalPressure(
                        iFrameIndex,
                        indexColumn1Left,
                        indexColumn2Right,
                        indexRafter1Left,
                        indexRafter2Right,
                        (int)ELCMainDirection.eMinusX,
                        fFrameTributaryWidth,
                        ELSType.eLS_SLS,
                        ELCWindType.eWL_Cpi_max,
                        wind,
                ref memberLoadInternalPressure_SLS_Cpimax_Right);

            SetFrameMembersWindLoads_InternalPressure(
                        iFrameIndex,
                        indexColumn1Left,
                        indexColumn2Right,
                        indexRafter1Left,
                        indexRafter2Right,
                        (int)ELCMainDirection.ePlusY,
                        fFrameTributaryWidth,
                        ELSType.eLS_SLS,
                        ELCWindType.eWL_Cpi_max,
                        wind,
                ref memberLoadInternalPressure_SLS_Cpimax_Front);

            SetFrameMembersWindLoads_InternalPressure(
                        iFrameIndex,
                        indexColumn1Left,
                        indexColumn2Right,
                        indexRafter1Left,
                        indexRafter2Right,
                        (int)ELCMainDirection.eMinusY,
                        fFrameTributaryWidth,
                        ELSType.eLS_SLS,
                        ELCWindType.eWL_Cpi_max,
                        wind,
                ref memberLoadInternalPressure_SLS_Cpimax_Rear);

            // External Pressure
            // SLS
            // Cpe,min
            SetFrameMembersWindLoads_LeftOrRight(iFrameIndex,
                        indexColumn1Left,
                        indexColumn2Right,
                        indexRafter1Left,
                        indexRafter2Right,
                        (int)ELCMainDirection.ePlusX,
                        fFrameTributaryWidth,
                        ELSType.eLS_SLS,
                        ELCWindType.eWL_Cpe_min,
                        wind,
                        ref memberLoadExternalPressure_SLS_Cpemin_Left);

            SetFrameMembersWindLoads_LeftOrRight(iFrameIndex,
                        indexColumn1Left,
                        indexColumn2Right,
                        indexRafter1Left,
                        indexRafter2Right,
                        (int)ELCMainDirection.eMinusX,
                        fFrameTributaryWidth,
                        ELSType.eLS_SLS,
                        ELCWindType.eWL_Cpe_min,
                        wind,
                        ref memberLoadExternalPressure_SLS_Cpemin_Right);


            SetFrameMembersWindLoads_FrontOrRear(iFrameIndex,
                        indexColumn1Left,
                        indexColumn2Right,
                        indexRafter1Left,
                        indexRafter2Right,
                        (int)ELCMainDirection.ePlusY,
                        fFrameTributaryWidth,
                        ELSType.eLS_SLS,
                        ELCWindType.eWL_Cpe_min,
                        wind,
                        ref memberLoadExternalPressure_SLS_Cpemin_Front);

            SetFrameMembersWindLoads_FrontOrRear(iFrameIndex,
                        indexColumn1Left,
                        indexColumn2Right,
                        indexRafter1Left,
                        indexRafter2Right,
                        (int)ELCMainDirection.eMinusY,
                        fFrameTributaryWidth,
                        ELSType.eLS_SLS,
                        ELCWindType.eWL_Cpe_min,
                        wind,
                        ref memberLoadExternalPressure_SLS_Cpemin_Rear);

            // Cpe,max
            SetFrameMembersWindLoads_LeftOrRight(iFrameIndex,
                        indexColumn1Left,
                        indexColumn2Right,
                        indexRafter1Left,
                        indexRafter2Right,
                        (int)ELCMainDirection.ePlusX,
                        fFrameTributaryWidth,
                        ELSType.eLS_SLS,
                        ELCWindType.eWL_Cpe_max,
                        wind,
                        ref memberLoadExternalPressure_SLS_Cpemax_Left);

            SetFrameMembersWindLoads_LeftOrRight(iFrameIndex,
                        indexColumn1Left,
                        indexColumn2Right,
                        indexRafter1Left,
                        indexRafter2Right,
                        (int)ELCMainDirection.eMinusX,
                        fFrameTributaryWidth,
                        ELSType.eLS_SLS,
                        ELCWindType.eWL_Cpe_max,
                        wind,
                        ref memberLoadExternalPressure_SLS_Cpemax_Right);

            SetFrameMembersWindLoads_FrontOrRear(iFrameIndex,
                        indexColumn1Left,
                        indexColumn2Right,
                        indexRafter1Left,
                        indexRafter2Right,
                        (int)ELCMainDirection.ePlusY,
                        fFrameTributaryWidth,
                        ELSType.eLS_SLS,
                        ELCWindType.eWL_Cpe_max,
                        wind,
                        ref memberLoadExternalPressure_SLS_Cpemax_Front);

            SetFrameMembersWindLoads_FrontOrRear(iFrameIndex,
                        indexColumn1Left,
                        indexColumn2Right,
                        indexRafter1Left,
                        indexRafter2Right,
                        (int)ELCMainDirection.eMinusY,
                        fFrameTributaryWidth,
                        ELSType.eLS_SLS,
                        ELCWindType.eWL_Cpe_max,
                        wind,
                        ref memberLoadExternalPressure_SLS_Cpemax_Rear);
        }

        // Internal Pressure
        // Poznamka: Faktor Ka * Kci >= 0.8 sa pre internal pressure nezohladnuje
        private void SetFrameMembersWindLoads_InternalPressure(
           int iFrameIndex,
           int indexColumn1Left,
           int indexColumn2Right,
           int indexRafter1Left,
           int indexRafter2Right,
           int iWindDirectionIndex,
           float fFrameTributaryWidth,
           ELSType eLSType,
           ELCWindType eLCWindType,
           CCalcul_1170_2 wind,
           ref List<CMLoad> listOfMemberLoads)
        {
            float fK_ci_min = 1.0f;
            float fK_ci_max = 1.0f;
            float fK_ci;

            if (bConsiderFactors_Kci_Kce_Ka_Generator)
            {
                Set_ActionCombinationFactors_Kci(
                            4,
                            wind.fC_pi_min,
                            wind.fC_pi_max,
                            out fK_ci_min,
                            out fK_ci_max
                            );
            }

            float[] fp_i_Theta_4;

            if (eLSType == ELSType.eLS_ULS)
            {
                if (eLCWindType == ELCWindType.eWL_Cpi_min) // ULS - Cpi,min
                {
                    fp_i_Theta_4 = wind.fp_i_min_ULS_Theta_4;
                    fK_ci = fK_ci_min;
                }
                else //if (eLCWindType == ELCWindType.eWL_Cpi_max) // ULS - Cpi,max
                {
                    fp_i_Theta_4 = wind.fp_i_max_ULS_Theta_4;
                    fK_ci = fK_ci_max;
                }
            }
            else
            {
                if (eLCWindType == ELCWindType.eWL_Cpi_min) // SLS - Cpi,min
                {
                    fp_i_Theta_4 = wind.fp_i_min_SLS_Theta_4;
                    fK_ci = fK_ci_min;
                }
                else //if (eLCWindType == ELCWindType.eWL_Cpi_max) // SLS - Cpi,max
                {
                    fp_i_Theta_4 = wind.fp_i_max_SLS_Theta_4;
                    fK_ci = fK_ci_max;
                }
            }

            float fReductionFactor_Kci_Column1Left = fK_ci;
            float fReductionFactor_Kci_Column2Right = fK_ci;
            float fReductionFactor_Kci_Rafter1Left = fK_ci;
            float fReductionFactor_Kci_Rafter2Right = fK_ci;

            // Columns
            CMLoad loadColumnLeft_WindLoad_Cpi = new CMLoad_21(iFrameIndex * 4 + 1, fReductionFactor_Kci_Column1Left * fp_i_Theta_4[iWindDirectionIndex] * fFrameTributaryWidth, m_arrMembers[indexColumn1Left], EMLoadTypeDistr.eMLT_QUF_W_21, ELoadType.eLT_F, ELoadCoordSystem.eLCS, ELoadDirection.eLD_Z, true, 0);
            CMLoad loadColumnRight_WindLoad_Cpi = new CMLoad_21(iFrameIndex * 4 + 2, fReductionFactor_Kci_Column2Right * fp_i_Theta_4[iWindDirectionIndex] * fFrameTributaryWidth, m_arrMembers[indexColumn2Right], EMLoadTypeDistr.eMLT_QUF_W_21, ELoadType.eLT_F, ELoadCoordSystem.eLCS, ELoadDirection.eLD_Z, true, 0);
            listOfMemberLoads.Add(loadColumnLeft_WindLoad_Cpi);
            listOfMemberLoads.Add(loadColumnRight_WindLoad_Cpi);
            // Rafters
            CMLoad loadRafterLeft_WindLoad_Cpi = new CMLoad_21(iFrameIndex * 4 + 3, fReductionFactor_Kci_Rafter1Left * fp_i_Theta_4[iWindDirectionIndex] * fFrameTributaryWidth, m_arrMembers[indexRafter1Left], EMLoadTypeDistr.eMLT_QUF_W_21, ELoadType.eLT_F, ELoadCoordSystem.eLCS, ELoadDirection.eLD_Z, true, 0);
            CMLoad loadRafterRight_WindLoad_Cpi = new CMLoad_21(iFrameIndex * 4 + 4, fReductionFactor_Kci_Rafter2Right * fp_i_Theta_4[iWindDirectionIndex] * fFrameTributaryWidth, m_arrMembers[indexRafter2Right], EMLoadTypeDistr.eMLT_QUF_W_21, ELoadType.eLT_F, ELoadCoordSystem.eLCS, ELoadDirection.eLD_Z, true, 0);
            listOfMemberLoads.Add(loadRafterLeft_WindLoad_Cpi);
            listOfMemberLoads.Add(loadRafterRight_WindLoad_Cpi);
        }

        // External pressure
        // Poznamka: Faktor Ka * Kce >= 0.8 sa pre internal pressure nezohladnuje
        private void SetFrameMembersWindLoads_LeftOrRight(
            int iFrameIndex,
            int indexColumn1Left,
            int indexColumn2Right,
            int indexRafter1Left,
            int indexRafter2Right,
            int iDirectionIndex,
            float fFrameTributaryWidth,
            ELSType eLSType,
            ELCWindType eLCWindType,
            CCalcul_1170_2 wind,
            ref List<CMLoad> listOfMemberLoads)
        {
            // External Pressure
            if (iDirectionIndex != (int)ELCMainDirection.ePlusX && iDirectionIndex != (int)ELCMainDirection.eMinusX)
                return; // Invalid direction - return empty list of loads

            float fK_a_Column1Left = 1f;
            float fK_a_Column2Right = 1f;
            float fK_a_Rafter1Left = 1f;
            float fK_a_Rafter2Right = 1f;
            float fK_ce_min_roof = 1f;
            float fK_ce_max_roof = 1f;
            float fK_ce_wall = 1f;
            float fK_ce_roof = 1f;

            if (bConsiderFactors_Kci_Kce_Ka_Generator)
            {
                // Calculate reduction factors Ka, Kci, Kce
                // 5.4.2 Area reduction factor (Ka)
                Calculate_Wind_Area_Reduction_Factors_Ka(
                indexColumn1Left,
                indexColumn2Right,
                indexRafter1Left,
                indexRafter2Right,
                fFrameTributaryWidth,
                out fK_a_Column1Left,
                out fK_a_Column2Right,
                out fK_a_Rafter1Left,
                out fK_a_Rafter2Right
                );

                // For any roofs and side walls, the product Ka. Kc,e shall not be less than 0.8.

                Set_ActionCombinationFactors_Kce(
                4,
                out fK_ce_min_roof,
                out fK_ce_max_roof,
                out fK_ce_wall
                );
            }

            // Left or Right Main Direction
            float fColumnLeftLoadValue;
            float fColumnRightLoadValue;

            if (eLSType == ELSType.eLS_ULS)
            {
                if (iDirectionIndex == (int)ELCMainDirection.ePlusX)
                {
                    fColumnLeftLoadValue = -wind.fp_e_W_wall_ULS_Theta_4[iDirectionIndex] * fFrameTributaryWidth;
                    fColumnRightLoadValue = -wind.fp_e_L_wall_ULS_Theta_4[iDirectionIndex] * fFrameTributaryWidth;
                }
                else
                {
                    fColumnLeftLoadValue = -wind.fp_e_L_wall_ULS_Theta_4[iDirectionIndex] * fFrameTributaryWidth;
                    fColumnRightLoadValue = -wind.fp_e_W_wall_ULS_Theta_4[iDirectionIndex] * fFrameTributaryWidth;
                }
            }
            else
            {
                if (iDirectionIndex == (int)ELCMainDirection.ePlusX)
                {
                    fColumnLeftLoadValue = -wind.fp_e_W_wall_SLS_Theta_4[iDirectionIndex] * fFrameTributaryWidth;
                    fColumnRightLoadValue = -wind.fp_e_L_wall_SLS_Theta_4[iDirectionIndex] * fFrameTributaryWidth;
                }
                else
                {
                    fColumnLeftLoadValue = -wind.fp_e_L_wall_SLS_Theta_4[iDirectionIndex] * fFrameTributaryWidth;
                    fColumnRightLoadValue = -wind.fp_e_W_wall_SLS_Theta_4[iDirectionIndex] * fFrameTributaryWidth;
                }
            }


            float fReductionFactor_Ka_Kce_Column1Left = Set_Product_Ka_Kce(fK_a_Column1Left, fK_ce_wall);
            float fReductionFactor_Ka_Kce_Column2Right = Set_Product_Ka_Kce(fK_a_Column2Right,fK_ce_wall);

            // Columns
            CMLoad loadColumnLeft_WindLoad = new CMLoad_21(iFrameIndex * 4 + 1, fReductionFactor_Ka_Kce_Column1Left * fColumnLeftLoadValue, m_arrMembers[indexColumn1Left], EMLoadTypeDistr.eMLT_QUF_W_21, ELoadType.eLT_F, ELoadCoordSystem.eLCS, ELoadDirection.eLD_Z, true, 0);
            CMLoad loadColumnRight_WindLoad = new CMLoad_21(iFrameIndex * 4 + 2, fReductionFactor_Ka_Kce_Column2Right * fColumnRightLoadValue, m_arrMembers[indexColumn2Right], EMLoadTypeDistr.eMLT_QUF_W_21, ELoadType.eLT_F, ELoadCoordSystem.eLCS, ELoadDirection.eLD_Z, true, 0);
            listOfMemberLoads.Add(loadColumnLeft_WindLoad);
            listOfMemberLoads.Add(loadColumnRight_WindLoad);

            // Rafters - generate loads depending on win pressure zone on the roof
            float[,] fp_e_U_roof_Theta_4;
            float[,] fp_e_D_roof_Theta_4;

            if (eLSType == ELSType.eLS_ULS)
            {
                if (eLCWindType == ELCWindType.eWL_Cpe_min) // ULS - Cpe,min
                {
                    fp_e_U_roof_Theta_4 = wind.fp_e_min_U_roof_ULS_Theta_4;
                    fp_e_D_roof_Theta_4 = wind.fp_e_min_D_roof_ULS_Theta_4;
                    fK_ce_roof = fK_ce_min_roof;
                }
                else //if (eLCWindType == ELCWindType.eWL_Cpe_max) // ULS - Cpe,max
                {
                    fp_e_U_roof_Theta_4 = wind.fp_e_max_U_roof_ULS_Theta_4;
                    fp_e_D_roof_Theta_4 = wind.fp_e_max_D_roof_ULS_Theta_4;
                    fK_ce_roof = fK_ce_max_roof;
                }
            }
            else
            {
                if (eLCWindType == ELCWindType.eWL_Cpe_min) // SLS - Cpe,min
                {
                    fp_e_U_roof_Theta_4 = wind.fp_e_min_U_roof_SLS_Theta_4;
                    fp_e_D_roof_Theta_4 = wind.fp_e_min_D_roof_SLS_Theta_4;
                    fK_ce_roof = fK_ce_min_roof;
                }
                else //if (eLCWindType == ELCWindType.eWL_Cpe_max) // ULS - Cpe,max
                {
                    fp_e_U_roof_Theta_4 = wind.fp_e_max_U_roof_SLS_Theta_4;
                    fp_e_D_roof_Theta_4 = wind.fp_e_max_D_roof_SLS_Theta_4;
                    fK_ce_roof = fK_ce_max_roof;
                }
            }

            float fReductionFactor_Ka_Kce_Rafter1Left = Set_Product_Ka_Kce(fK_a_Rafter1Left, fK_ce_roof);
            float fReductionFactor_Ka_Kce_Rafter2Right = Set_Product_Ka_Kce(fK_a_Rafter2Right, fK_ce_roof);

            float[,] fRafterLeft_LoadValues;
            float[] fRafterLeft_RoofDimensions;

            float[,] fRafterRight_LoadValues;
            float[] fRafterRight_RoofDimensions;

            if (iDirectionIndex == (int)ELCMainDirection.ePlusX)
            {
                fRafterLeft_LoadValues = fp_e_U_roof_Theta_4;
                fRafterLeft_RoofDimensions = wind.fC_pe_U_roof_dimensions;

                fRafterRight_LoadValues = fp_e_D_roof_Theta_4;
                fRafterRight_RoofDimensions = wind.fC_pe_D_roof_dimensions;
            }
            else
            {
                fRafterLeft_LoadValues = fp_e_D_roof_Theta_4;
                fRafterLeft_RoofDimensions = wind.fC_pe_D_roof_dimensions;

                fRafterRight_LoadValues = fp_e_U_roof_Theta_4;
                fRafterRight_RoofDimensions = wind.fC_pe_U_roof_dimensions;
            }

            int iLastLoadIndex = iFrameIndex * 4 + 2;

            // Left Rafter
            GenerateListOfWindLoadsOnRafter_LeftRight(indexRafter1Left,
                fReductionFactor_Ka_Kce_Rafter1Left,
                fFrameTributaryWidth,
                iLastLoadIndex,
                fRafterLeft_LoadValues,
                fRafterLeft_RoofDimensions,
                iDirectionIndex,
                ref listOfMemberLoads);

            // Right Rafter
            GenerateListOfWindLoadsOnRafter_LeftRight(indexRafter2Right,
                fReductionFactor_Ka_Kce_Rafter2Right,
                fFrameTributaryWidth,
                iLastLoadIndex,
                fRafterRight_LoadValues,
                fRafterRight_RoofDimensions,
                iDirectionIndex,
                ref listOfMemberLoads);
        }

        private void GenerateListOfWindLoadsOnRafter_LeftRight(
            int indexRafter,
            float fReductionFactor_Ka_Kce,
            float fFrameTributaryWidth,
            int iLastLoadIndex,
            float[,] fp_e_roof_Theta_4,
            float[] fRoof_dimensions,
            int iWindDirectionIndex,
            ref List<CMLoad> listOfMemberLoads)
        {
            int iIndexOfMemberLoad = 0;
            float fMemberProjectedLength_X = (float)m_arrMembers[indexRafter].Delta_X;
            do
            {
                // TODO - zapracovat identifikaciu polohy pruta voci jednotlivym oblastiam zatazenia ak nezacina v X = 0

                float fq = -fp_e_roof_Theta_4[iWindDirectionIndex, iIndexOfMemberLoad] * fFrameTributaryWidth;

                float fstart_abs_Projected = fRoof_dimensions[iIndexOfMemberLoad];
                float floadsegmentlengthProjected = Math.Min(fRoof_dimensions[iIndexOfMemberLoad + 1], fMemberProjectedLength_X) - fRoof_dimensions[iIndexOfMemberLoad];

                if (iWindDirectionIndex != (int)ELCMainDirection.ePlusX) // Minus X - Right Wind Pressure
                {
                    // Start position of load on member (member LCS from left to right |----> x-axis)   (load direction from right <----|)
                    fstart_abs_Projected = fMemberProjectedLength_X - fRoof_dimensions[iIndexOfMemberLoad] - floadsegmentlengthProjected;
                }

                float fstart_abs = fstart_abs_Projected * m_arrMembers[indexRafter].FLength / fMemberProjectedLength_X;
                float floadsegmentlength = floadsegmentlengthProjected * m_arrMembers[indexRafter].FLength / fMemberProjectedLength_X;

                CMLoad loadRafterSegment = new CMLoad_24(iLastLoadIndex + 1, fReductionFactor_Ka_Kce * fq, fstart_abs, floadsegmentlength, m_arrMembers[indexRafter], EMLoadTypeDistr.eMLT_QUF_PG_24, ELoadType.eLT_F, ELoadCoordSystem.eLCS, ELoadDirection.eLD_Z, true, 0);
                listOfMemberLoads.Add(loadRafterSegment);

                iLastLoadIndex++;
                iIndexOfMemberLoad++;
            }
            while (fRoof_dimensions[iIndexOfMemberLoad] < fMemberProjectedLength_X);
        }

        private void SetFrameMembersWindLoads_FrontOrRear(
            int iFrameIndex,
            int indexColumn1Left,
            int indexColumn2Right,
            int indexRafter1Left,
            int indexRafter2Right,
            int iWindDirectionIndex,
            float fFrameTributaryWidth,
            ELSType eLSType,
            ELCWindType eLCWind,
            CCalcul_1170_2 wind,
            ref List<CMLoad> listOfMemberLoads)
        {
            // External pressure
            //listOfMemberLoads = new List<CMLoad>(); // Generate 4 loads - type uniform per whole length (CMLoad_21)

            if (iWindDirectionIndex != (int)ELCMainDirection.ePlusY && iWindDirectionIndex != (int)ELCMainDirection.eMinusY)
                return; // Invalid direction - return empty list of loads

            //task 600
            //float fFrameCoordinate_GCS_Y = iFrameIndex * fL1_frame; // Expected equal distance between frames (bay width)
            float fFrameCoordinate_GCS_Y = GetBaysWidthUntilFrameIndex(iFrameIndex);

            if (iWindDirectionIndex != (int)ELCMainDirection.ePlusY) // Minus Y - Rear Wind Pressure
            {
                fFrameCoordinate_GCS_Y = fL_tot - fFrameCoordinate_GCS_Y;
            }

            //task 600
            // Set minimum and maximum coordinate of tributary area
            //float fTributaryWidth_Y_Coordinate_Min = fFrameCoordinate_GCS_Y - 0.5f * fL1_frame;
            //float fTributaryWidth_Y_Coordinate_Max = fFrameCoordinate_GCS_Y + 0.5f * fL1_frame;
            float fTributaryWidth_Y_Coordinate_Min = 0;
            float fTributaryWidth_Y_Coordinate_Max = 0;

            if (iFrameIndex == 0) // First Frame
            {
                if (iWindDirectionIndex == (int)ELCMainDirection.ePlusY) // First frame
                {
                    fTributaryWidth_Y_Coordinate_Min = 0;
                    fTributaryWidth_Y_Coordinate_Max = 0.5f * m_L1_Bays.First();
                }
                else // Last frame
                {
                    //fTributaryWidth_Y_Coordinate_Max = (iFramesNo - 1) * fL1_frame;
                    fTributaryWidth_Y_Coordinate_Min = fL_tot - 0.5f * m_L1_Bays.First();
                    fTributaryWidth_Y_Coordinate_Max = fL_tot;
                }
            }
            else if (iFrameIndex == iFramesNo - 1) // Last Frame
            {
                if (iWindDirectionIndex == (int)ELCMainDirection.ePlusY) // First frame
                {
                    //fTributaryWidth_Y_Coordinate_Max = (iFramesNo - 1) * fL1_frame;
                    fTributaryWidth_Y_Coordinate_Min = fL_tot - 0.5f * m_L1_Bays.Last();
                    fTributaryWidth_Y_Coordinate_Max = fL_tot;
                }
                else // Last frame
                {
                    fTributaryWidth_Y_Coordinate_Min = 0;
                    fTributaryWidth_Y_Coordinate_Max = 0.5f * m_L1_Bays.Last();
                }
            }
            else
            {
                if (iWindDirectionIndex == (int)ELCMainDirection.ePlusY)
                {                    
                    fTributaryWidth_Y_Coordinate_Min = fFrameCoordinate_GCS_Y - 0.5f * m_L1_Bays[iFrameIndex - 1];
                    fTributaryWidth_Y_Coordinate_Max = fFrameCoordinate_GCS_Y + 0.5f * m_L1_Bays[iFrameIndex];
                }
                else
                {
                    fTributaryWidth_Y_Coordinate_Min = fFrameCoordinate_GCS_Y - 0.5f * m_L1_Bays[(iFramesNo - 1) - iFrameIndex - 1];
                    fTributaryWidth_Y_Coordinate_Max = fFrameCoordinate_GCS_Y + 0.5f * m_L1_Bays[(iFramesNo - 1) - iFrameIndex];
                }
            }

            // Find all x-coordinates of wind load zones that start within interval < fTributaryWidth_Y_Coordinate_Min; fTributaryWidth_Y_Coordinate_Max>

            float[,] fLoadValues_Columns;
            float[,] fLoadValues_Rafters;

            // For any roofs and side walls, the product Ka. Kc,e shall not be less than 0.8.
            float fK_a_Column1Left = 1f;
            float fK_a_Column2Right = 1f;
            float fK_a_Rafter1Left = 1f;
            float fK_a_Rafter2Right = 1f;
            float fK_ce_min_roof = 1f;
            float fK_ce_max_roof = 1f;
            float fK_ce_wall = 1f;
            float fK_ce_roof = 1f;

            if (bConsiderFactors_Kci_Kce_Ka_Generator)
            {
                // Calculate reduction factors Ka, Kci, Kce
                // 5.4.2 Area reduction factor(Ka)
                Calculate_Wind_Area_Reduction_Factors_Ka(
                indexColumn1Left,
                indexColumn2Right,
                indexRafter1Left,
                indexRafter2Right,
                fFrameTributaryWidth,
                out fK_a_Column1Left,
                out fK_a_Column2Right,
                out fK_a_Rafter1Left,
                out fK_a_Rafter2Right
                );

                // For any roofs and side walls, the product Ka. Kc,e shall not be less than 0.8.

                Set_ActionCombinationFactors_Kce(
                4,
                out fK_ce_min_roof,
                out fK_ce_max_roof,
                out fK_ce_wall
                );
            }

            if (eLSType == ELSType.eLS_ULS)
            {
                fLoadValues_Columns = wind.fp_e_S_wall_ULS_Theta_4;

                if (eLCWind == ELCWindType.eWL_Cpe_min)
                {
                    fLoadValues_Rafters = wind.fp_e_min_R_roof_ULS_Theta_4;
                    fK_ce_roof = fK_ce_min_roof;
                }
                else //if((eLCWind == ELCWindType.eWL_Cpe_max)
                {
                    fLoadValues_Rafters = wind.fp_e_max_R_roof_ULS_Theta_4;
                    fK_ce_roof = fK_ce_max_roof;
                }
            }
            else
            {
                fLoadValues_Columns = wind.fp_e_S_wall_SLS_Theta_4;

                if (eLCWind == ELCWindType.eWL_Cpe_min)
                {
                    fLoadValues_Rafters = wind.fp_e_min_R_roof_SLS_Theta_4;
                    fK_ce_roof = fK_ce_min_roof;
                }
                else //if(eLCWind == ELCWindType.eWL_Cpe_max)
                {
                    fLoadValues_Rafters = wind.fp_e_max_R_roof_SLS_Theta_4;
                    fK_ce_roof = fK_ce_max_roof;
                }
            }

            float fReductionFactor_Ka_Kce_Column1Left = Set_Product_Ka_Kce(fK_a_Column1Left, fK_ce_wall);
            float fReductionFactor_Ka_Kce_Column2Right = Set_Product_Ka_Kce(fK_a_Column2Right, fK_ce_wall);
            float fReductionFactor_Ka_Kce_Rafter1Left = Set_Product_Ka_Kce(fK_a_Rafter1Left, fK_ce_roof);
            float fReductionFactor_Ka_Kce_Rafter2Right = Set_Product_Ka_Kce(fK_a_Rafter2Right, fK_ce_roof);

            float[] fx_dimensions_Columns = wind.fC_pe_S_wall_dimensions; // Wall - Columns
            float[] fx_dimensions_Rafters = wind.fC_pe_R_roof_dimensions; // Roof - Rafters

            // Columns
            float fValue_q_columns;

            CalculateWindLoadValueOnFrameMember(fLoadValues_Columns,
                fx_dimensions_Columns,
                fTributaryWidth_Y_Coordinate_Min,
                fTributaryWidth_Y_Coordinate_Max,
                iWindDirectionIndex,
                fFrameTributaryWidth,
                -1,
            out fValue_q_columns);

            // Create Member Load - Left and Right Column
            CMLoad loadColumn1 = new CMLoad_21(1, fReductionFactor_Ka_Kce_Column1Left * fValue_q_columns, m_arrMembers[indexColumn1Left], EMLoadTypeDistr.eMLT_QUF_W_21, ELoadType.eLT_F, ELoadCoordSystem.eLCS, ELoadDirection.eLD_Z, true, 0);
            CMLoad loadColumn2 = new CMLoad_21(2, fReductionFactor_Ka_Kce_Column2Right * fValue_q_columns, m_arrMembers[indexColumn2Right], EMLoadTypeDistr.eMLT_QUF_W_21, ELoadType.eLT_F, ELoadCoordSystem.eLCS, ELoadDirection.eLD_Z, true, 0);

            listOfMemberLoads.Add(loadColumn1);
            listOfMemberLoads.Add(loadColumn2);

            // Rafters
            float fValue_q_rafters;

            CalculateWindLoadValueOnFrameMember(fLoadValues_Rafters,
                fx_dimensions_Rafters,
                fTributaryWidth_Y_Coordinate_Min,
                fTributaryWidth_Y_Coordinate_Max,
                iWindDirectionIndex,
                fFrameTributaryWidth,
                -1,
            out fValue_q_rafters);

            // Create Member Load - Left and Right Rafter
            CMLoad loadRafter1 = new CMLoad_21(3, fReductionFactor_Ka_Kce_Rafter1Left * fValue_q_rafters, m_arrMembers[indexRafter1Left], EMLoadTypeDistr.eMLT_QUF_W_21, ELoadType.eLT_F, ELoadCoordSystem.eLCS, ELoadDirection.eLD_Z, true, 0);
            CMLoad loadRafter2 = new CMLoad_21(4, fReductionFactor_Ka_Kce_Rafter2Right * fValue_q_rafters, m_arrMembers[indexRafter2Right], EMLoadTypeDistr.eMLT_QUF_W_21, ELoadType.eLT_F, ELoadCoordSystem.eLCS, ELoadDirection.eLD_Z, true, 0);

            listOfMemberLoads.Add(loadRafter1);
            listOfMemberLoads.Add(loadRafter2);
        }

        private void CalculateWindLoadValueOnFrameMember(
            float[,] fLoadValues,
            float[] fx_dimensions,
            float fTributaryWidth_Y_Coordinate_Min,
            float fTributaryWidth_Y_Coordinate_Max,
            int iDirectionIndex,
            float fFrameTributaryWidth,
            float fLoadDirectionIndex,
            out float fValue_q
            )
        {
            // Validation of zone geometry - item values in the array must be ascending
            if (MathF.d_equal(fx_dimensions[0], fx_dimensions[1]))
            {
                //fValue_q = float.NaN;
                fValue_q = 0; // TODO - rozhodnut co vracat z funckie
                return; // Invalid input data
            }

            List<int> x_dimensionsInTributaryAreaIndices = new List<int>();

            int iIndex = 0;

            do
            {
                if (fx_dimensions[iIndex] >= fTributaryWidth_Y_Coordinate_Min &&
                   fx_dimensions[iIndex] <= fTributaryWidth_Y_Coordinate_Max)
                    x_dimensionsInTributaryAreaIndices.Add(iIndex);

                iIndex++;
            }
            while (fx_dimensions[iIndex] < fTributaryWidth_Y_Coordinate_Max);

            int iNumberOfParticularZonesInTributaryArea;

            fValue_q = 0;
            fLoadDirectionIndex = -1;

            if (x_dimensionsInTributaryAreaIndices.Count != 0) // Nasli sme nejake hranicne hodnoty x v ramci fTributaryWidth, existuje viacero zon s rozdielnou hodnotou zatazenia
            {
                // Determinate number of contributing zones (number of particular widths)
                if (MathF.d_equal(fx_dimensions[x_dimensionsInTributaryAreaIndices[0]], fTributaryWidth_Y_Coordinate_Min)) // First zone starts in the coordinate of tributary width
                    iNumberOfParticularZonesInTributaryArea = x_dimensionsInTributaryAreaIndices.Count;
                else if (MathF.d_equal(fx_dimensions[x_dimensionsInTributaryAreaIndices[x_dimensionsInTributaryAreaIndices.Count - 1]], fTributaryWidth_Y_Coordinate_Max)) // Last zone ends in the coordinate of tributary width
                    iNumberOfParticularZonesInTributaryArea = x_dimensionsInTributaryAreaIndices.Count;
                else
                    iNumberOfParticularZonesInTributaryArea = x_dimensionsInTributaryAreaIndices.Count + 1;

                // Calculate load value
                for (int i = 0; i < iNumberOfParticularZonesInTributaryArea; i++)
                {
                    float fParticularContributingWidth;

                    if (MathF.d_equal(fx_dimensions[x_dimensionsInTributaryAreaIndices[0]], fTributaryWidth_Y_Coordinate_Min)) // First zone starts in the coordinate of tributary width
                    {
                        if (x_dimensionsInTributaryAreaIndices.Count > (i + 1))
                            fParticularContributingWidth = Math.Min(x_dimensionsInTributaryAreaIndices[i + 1], fTributaryWidth_Y_Coordinate_Max) - x_dimensionsInTributaryAreaIndices[i];
                        else // First zone is longer than tributary coordinate
                            fParticularContributingWidth = fTributaryWidth_Y_Coordinate_Max - x_dimensionsInTributaryAreaIndices[i];

                        fValue_q += fLoadDirectionIndex * fLoadValues[iDirectionIndex, x_dimensionsInTributaryAreaIndices[i]] * fParticularContributingWidth;
                    }
                    else // First zone starts before the coordinate of tributary width
                    {
                        if (i == 0) // First zone
                        {
                            fParticularContributingWidth = fx_dimensions[x_dimensionsInTributaryAreaIndices[i]] - fTributaryWidth_Y_Coordinate_Min;
                        }
                        else if (i < iNumberOfParticularZonesInTributaryArea - 1)
                        {
                            fParticularContributingWidth = fx_dimensions[x_dimensionsInTributaryAreaIndices[i]] - fx_dimensions[x_dimensionsInTributaryAreaIndices[i - 1]];
                        }
                        else // Last zone
                        {
                            fParticularContributingWidth = fTributaryWidth_Y_Coordinate_Max - fx_dimensions[x_dimensionsInTributaryAreaIndices[i - 1]];
                        }

                        if (i < iNumberOfParticularZonesInTributaryArea - 1)
                        {
                            // Prva uvazovana zona zacina pred fTributaryWidth_Y_Coordinate_Min berieme zatazenie pre index z pozicie -1 pred prvym ulozenym indexom pre miesta v intervale
                            fValue_q += fLoadDirectionIndex * fLoadValues[iDirectionIndex, x_dimensionsInTributaryAreaIndices[i] - 1] * fParticularContributingWidth;
                        }
                        else
                        {
                            // Posledna hodnota zatazenia sa berie z posledneho ulozeneho indexu
                            // Pocet poloziek je -1 pretoze hodnot x ulozenych v ramci celkovej tributary width je o jednu menej nez pocet particular tributary width
                            fValue_q += fLoadDirectionIndex * fLoadValues[iDirectionIndex, x_dimensionsInTributaryAreaIndices[i - 1]] * fParticularContributingWidth;
                        }
                    }
                }
            }
            else
            {
                // Ak sme nenasli v oblasti zatazovacej sirky ziadnu hodnotu z pola fx_dimensions
                // znamena to ze cela oblast zatazovacej sirky lezi v jednej zone

                // Nastavime index zaciatku tejto zony
                x_dimensionsInTributaryAreaIndices.Add(iIndex - 1);
                // Nastavime pocet particular tributary width
                iNumberOfParticularZonesInTributaryArea = 1; // Nie je potrebne

                fValue_q = -fLoadValues[iDirectionIndex, x_dimensionsInTributaryAreaIndices[0]] * fFrameTributaryWidth;
            }
        }

        private void Calculate_Wind_Area_Reduction_Factors_Ka(
            int indexColumn1Left,
            int indexColumn2Right,
            int indexRafter1Left,
            int indexRafter2Right,
            float fFrameTributaryWidth,
            out float fK_a_Column1Left,
            out float fK_a_Column2Right,
            out float fK_a_Rafter1Left,
            out float fK_a_Rafter2Right
            )
        {
            // Calculate reduction factors Ka, Kci, Kce
            // 5.4.2 Area reduction factor(Ka)

            float fTributaryArea_Column1Left = m_arrMembers[indexColumn1Left].FLength * fFrameTributaryWidth;
            float fTributaryArea_Column2Right = m_arrMembers[indexColumn2Right].FLength * fFrameTributaryWidth;
            float fTributaryArea_Rafter1Left = m_arrMembers[indexRafter1Left].FLength * fFrameTributaryWidth;
            float fTributaryArea_Rafter2Right = m_arrMembers[indexRafter2Right].FLength * fFrameTributaryWidth;

            fK_a_Column1Left = AS_NZS_1170_2.Get_AreaReductionFactor_Ka_Table54(fTributaryArea_Column1Left);
            fK_a_Column2Right = AS_NZS_1170_2.Get_AreaReductionFactor_Ka_Table54(fTributaryArea_Column2Right);
            fK_a_Rafter1Left = AS_NZS_1170_2.Get_AreaReductionFactor_Ka_Table54(fTributaryArea_Rafter1Left);
            fK_a_Rafter2Right = AS_NZS_1170_2.Get_AreaReductionFactor_Ka_Table54(fTributaryArea_Rafter2Right);
        }

        private static void Set_ActionCombinationFactors_Kci(
            int iNumberOfEffectiveSurfaces,
            float fC_pi_min,
            float fC_pi_max,
            out float fK_ci_min,
            out float fK_ci_max
            )
        {
            // 5.4.3 Action combination factor
            fK_ci_min = 1.0f;
            fK_ci_max = 1.0f;

            if (iNumberOfEffectiveSurfaces == 1)
            {
                fK_ci_min = 1.0f;
                fK_ci_max = 1.0f;
            }
            else if (iNumberOfEffectiveSurfaces == 2)
            {
                if (Math.Abs(fC_pi_min) >= 0.2f) fK_ci_min = 0.9f; // TODO - dopracovat podla kombinacii external and internal pressure
                if (Math.Abs(fC_pi_max) >= 0.2f) fK_ci_max = 0.9f; // TODO - dopracovat podla kombinacii external and internal pressure
            }
            else
            {
                if (Math.Abs(fC_pi_min) >= 0.2f) fK_ci_min = 0.8f; // TODO - dopracovat podla kombinacii external and internal pressure
                if (Math.Abs(fC_pi_max) >= 0.2f) fK_ci_max = 0.8f; // TODO - dopracovat podla kombinacii external and internal pressure
            }
        }

        private static void Set_ActionCombinationFactors_Kci(
            int iNumberOfEffectiveSurfaces,
            float fC_pi,
            out float fK_ci)
        {
            // 5.4.3 Action combination factor
            fK_ci = 1.0f;

            if (iNumberOfEffectiveSurfaces == 1)
            {
                fK_ci = 1.0f;
            }
            else if (iNumberOfEffectiveSurfaces == 2)
            {
                if (Math.Abs(fC_pi) >= 0.2f) fK_ci = 0.9f; // TODO - dopracovat podla kombinacii external and internal pressure
            }
            else
            {
                if (Math.Abs(fC_pi) >= 0.2f) fK_ci = 0.8f; // TODO - dopracovat podla kombinacii external and internal pressure
            }
        }

        private static void Set_ActionCombinationFactors_Kce(
            int iNumberOfEffectiveSurfaces,
            out float fK_ce_min,
            out float fK_ce_max,
            out float fK_ce_wall)
        {
            // 5.4.3 Action combination factor
            if (iNumberOfEffectiveSurfaces == 1)
            {
                fK_ce_min = 1.0f;
                fK_ce_max = 1.0f;
                fK_ce_wall = 1.0f;
            }
            else if (iNumberOfEffectiveSurfaces == 2)
            {
                fK_ce_min = 0.9f; // TODO - dopracovat podla kombinacii external and internal pressure
                fK_ce_max = 0.9f; // TODO - dopracovat podla kombinacii external and internal pressure
                fK_ce_wall = 0.9f;
            }
            else
            {
                fK_ce_min = 0.8f; // TODO - dopracovat podla kombinacii external and internal pressure
                fK_ce_max = 0.8f; // TODO - dopracovat podla kombinacii external and internal pressure
                fK_ce_wall = 0.8f;
            }
        }

        private static void Set_ActionCombinationFactors_Kce(
                int iNumberOfEffectiveSurfaces,
                out float fK_ce)
        {
            // 5.4.3 Action combination factor
            if (iNumberOfEffectiveSurfaces == 1)
            {
                fK_ce = 1.0f;
            }
            else if (iNumberOfEffectiveSurfaces == 2)
            {
                fK_ce = 0.9f; // TODO - dopracovat podla kombinacii external and internal pressure
            }
            else
            {
                fK_ce = 0.8f; // TODO - dopracovat podla kombinacii external and internal pressure
            }
        }

        private static float Set_Product_Ka_Kce(float fKa, float fK_ce)
        {
            if (fKa * fK_ce < 0.8f)
                return 0.8f;
            else
                return fKa * fK_ce;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Generate member loads from surface loads
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        static bool bDebugging = false; // Console output

        public LoadCasesMemberLoads GetGeneratedMemberLoads(CLoadCase[] m_arrLoadCases, CMember[] allMembersInModel)
        {
            //DateTime start = DateTime.Now;
            LoadCasesMemberLoads loadCasesMemberLoads = new LoadCasesMemberLoads();

            //int totalCyclesCount = 0;
            //int generateMemberLoad = 0;

            foreach (CLoadCase lc in m_arrLoadCases)
            {
                List<CMLoad> listOfMemberLoads = new List<CMLoad>();

                int iLoadID = 0;
                int c = 0;
                foreach (CSLoad_Free csload in lc.SurfaceLoadsList)
                {
                    c++;
                    foreach (CMember m in allMembersInModel)
                    {
                        foreach (FreeSurfaceLoadsMemberTypeData mtypedata in csload.listOfLoadedMemberTypeData)
                        {
                            //totalCyclesCount++;
                            if (m.EMemberType == mtypedata.memberType) // Prut je rovnakeho typu ako je niektory z typov prutov zo skupiny typov ktoru plocha zatazuje
                            {
                                if (csload is CSLoad_FreeUniformGroup)
                                {
                                    Transform3DGroup loadGroupTransform = ((CSLoad_FreeUniformGroup)csload).CreateTransformCoordGroupOfLoadGroup();
                                    //System.Diagnostics.Trace.WriteLine("---->  ----> after CreateTransformCoordGroupOfLoadGroup(): " + (DateTime.Now - start).TotalMilliseconds);
                                    foreach (CSLoad_FreeUniform l in ((CSLoad_FreeUniformGroup)csload).LoadList)
                                    {                                        
                                        if (MemberLiesOnSurfaceLoadPlane(l, m, loadGroupTransform)) // Prut lezi na ploche
                                        {
                                            if (bDebugging) System.Diagnostics.Trace.WriteLine($"LoadCase: {lc.Name} Surface: {c} contains member: {m.ID}");

                                            if (m.BIsDisplayed) // TODO - tu by mala byt podmienka ci je prut aktivny pre vypocet (nie len ci je zobrazeny) potrebujeme doriesit co s prutmi, ktore boli v mieste kde sa vlozili dvere, zatial som ich nemazal, lebo som si nebol isty ci by mi sedeli ID pre generovanie zatazenia, chcel som ich len deaktivovat
                                            {
                                                //generateMemberLoad++;
                                                GenerateMemberLoad(l, m, lc.MType_LS, lc.Type, lc.LC_Wind_Type, lc.MainDirection, lc.Type == ELCType.eWind ? wind : null, loadGroupTransform, mtypedata.fLoadingWidth, ref iLoadID, ref listOfMemberLoads);
                                                //System.Diagnostics.Trace.WriteLine($"----> {totalCyclesCount}  ----> {generateMemberLoad} after GenerateMemberLoad: " + (DateTime.Now - start).TotalMilliseconds);
                                            }
                                            //System.Diagnostics.Trace.WriteLine($"---->  {totalCyclesCount}----> after LoadList : MemberLiesOnSurfaceLoadPlane" + (DateTime.Now - start).TotalMilliseconds);
                                        }
                                        else
                                        {
                                            /*System.Diagnostics.Trace.WriteLine($"ERROR: Member {m.ID} not on plane. LoadCase: {lc.Name} Surface: {c}");*/
                                            //System.Diagnostics.Trace.WriteLine($"---->  {totalCyclesCount}----> after LoadList : Not MemberLiesOnSurfaceLoadPlane" + (DateTime.Now - start).TotalMilliseconds);
                                            continue;
                                        }
                                        
                                    }
                                }
                                else if (csload is CSLoad_FreeUniform)
                                {
                                    CSLoad_FreeUniform l = (CSLoad_FreeUniform)csload;

                                    if (MemberLiesOnSurfaceLoadPlane(l, m, null)) // Prut lezi na ploche
                                    {
                                        if (bDebugging) System.Diagnostics.Trace.WriteLine($"LoadCase: {lc.Name} Surface: {c} contains member: {m.ID}");

                                        if (m.BIsDisplayed) // TODO - tu by mala byt podmienka ci je prut aktivny pre vypocet (nie len ci je zobrazeny) potrebujeme doriesit co s prutmi, ktore boli v mieste kde sa vlozili dvere, zatial som ich nemazal, lebo som si nebol isty ci by mi sedeli ID pre generovanie zatazenia, chcel som ich len deaktivovat
                                        {
                                            GenerateMemberLoad(l, m, lc.MType_LS, lc.Type, lc.LC_Wind_Type, lc.MainDirection, lc.Type == ELCType.eWind ? wind : null, null, mtypedata.fLoadingWidth, ref iLoadID, ref listOfMemberLoads);
                                            //System.Diagnostics.Trace.WriteLine($"----> {totalCyclesCount} ----> {generateMemberLoad} after GenerateMemberLoad: " + (DateTime.Now - start).TotalMilliseconds);
                                        }
                                        //System.Diagnostics.Trace.WriteLine($"---->  {totalCyclesCount}----> after CSLoad_FreeUniform : YES MemberLiesOnSurfaceLoadPlane" + (DateTime.Now - start).TotalMilliseconds);
                                    }
                                    else { /*System.Diagnostics.Trace.WriteLine($"ERROR: Member {m.ID} not on plane. LoadCase: {lc.Name} Surface: {c}");*/
                                        //System.Diagnostics.Trace.WriteLine($"---->  {totalCyclesCount}----> after CSLoad_FreeUniform : NO MemberLiesOnSurfaceLoadPlane" + (DateTime.Now - start).TotalMilliseconds);
                                        continue;
                                    }                                    
                                }
                            } // member type is included in group of types
                        } //foreach memberType in group of types loaded by surface load
                    } //foreach member
                } //foreach surface load in load case

                loadCasesMemberLoads.Add(lc.ID, listOfMemberLoads);
            } //foreach loadcase

            //System.Diagnostics.Trace.WriteLine("----> totalCyclesCount: " + totalCyclesCount);
            return loadCasesMemberLoads;
        }

        private static bool MemberLiesOnSurfaceLoadPlane(CSLoad_FreeUniform l, CMember m, Transform3DGroup loadGroupTransform)
        {
            //25.3.2020 - doplneny if aby zbytovne nevytvaralo uz vytvorene points
            if(l.PointsGCS == null) l.PointsGCS = GetSurfaceLoadCoordinates_GCS(l, loadGroupTransform); // Positions in global coordinate system GCS

            if (l.PointsGCS.Count < 2) { return false; }

            return Drawing3D.MemberLiesOnPlane(l.PointsGCS[0], l.PointsGCS[1], l.PointsGCS[2], m);
        }

        private static void GenerateMemberLoad(CSLoad_FreeUniform l, CMember m, ELCGTypeForLimitState lcTypeForLS, ELCType lcType, ELCWindType lcWindType, ELCMainDirection lCMainDirection, CCalcul_1170_2 wind, Transform3DGroup loadGroupTransform, float fDist, ref int iLoadID, ref List<CMLoad> listOfMemberLoads)
        {
            // Transformacia pruta do LCS plochy
            GeneralTransform3D inverseTrans = GetSurfaceLoadTransformFromGCSToLCS(l, loadGroupTransform);
            Point3D pStart = new Point3D(m.NodeStart.X, m.NodeStart.Y, m.NodeStart.Z);
            Point3D pEnd = new Point3D(m.NodeEnd.X, m.NodeEnd.Y, m.NodeEnd.Z);

            Point3D pStartLCS = inverseTrans.Transform(pStart);
            Point3D pEndLCS = inverseTrans.Transform(pEnd);

            //zakomentovane...nevidim pouzitie 25.3.2020
            // Transformacia bodov plochy do LCS pruta
            //Transform3DGroup trans = m.CreateTransformCoordGroup(m, true);
            //GeneralTransform3D inverseTrans2 = trans.Inverse;
            //List<Point3D> surfaceDefPointsGCS = GetSurfaceLoadCoordinates_GCS(l, loadGroupTransform);

            //zakomentovane...nevidim pouzitie 25.3.2020
            //List<Point3D> surfaceDefPointsLCSMember = new List<Point3D>();
            //foreach (Point3D p in surfaceDefPointsGCS) surfaceDefPointsLCSMember.Add(inverseTrans.Transform(p));

            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            // Load direction transformation

            // Surface Load Direction Vector
            l.SetLoadDirectionVector(l.fValue); // Set vector depending on value

            // Member coordinate system LCS in GCS
            Transform3DGroup memberTransformGroupLCS_to_GCS = m.CreateTransformCoordGroup(m, true);

            // Surface coordinate system LCS in GCS
            Transform3DGroup loadTransformGroupLCS_to_GCS = GetSurfaceLoadTransformFromLCSToGCS(l, loadGroupTransform);

            // Surface load direction vector in GCS
            Vector3D vLoadDirectioninGCS = GetTransformedVector(l.LoadDirectionVector, loadTransformGroupLCS_to_GCS);
            Vector3D vloadDirectioninLCS = ((Transform3D)(memberTransformGroupLCS_to_GCS.Inverse)).Transform(vLoadDirectioninGCS);

            // Ak nie su vsetky osi pruta kolme na osi plochy, moze nastat pripad ze je potrebne vygenerovat viac zatazeni
            // (tj. zatazenie z plochy je potrebne rozlozit do viacerych smerov na prute, vznike teda viacero objektov member load)
            // Zistime ktore zlozky su ine nez 0 a ma sa pre ne generovat zatazenie

            List<MemberLoadParameters> listMemberLoadParams = new List<MemberLoadParameters>();

            if (!MathF.d_equal(Math.Abs(vloadDirectioninLCS.X), 0, 0.001))
            {
                MemberLoadParameters parameters_LCS_X = new MemberLoadParameters();

                parameters_LCS_X.fSurfaceLoadValueFactor = (float)vloadDirectioninLCS.X;
                parameters_LCS_X.eMemberLoadDirection = ELoadDirection.eLD_X;
                parameters_LCS_X.fMemberLoadValueSign = vloadDirectioninLCS.X < 0.0 ? -1 : 1;

                listMemberLoadParams.Add(parameters_LCS_X);
            }

            if (!MathF.d_equal(Math.Abs(vloadDirectioninLCS.Y), 0, 0.001))
            {
                MemberLoadParameters parameters_LCS_Y = new MemberLoadParameters();

                parameters_LCS_Y.fSurfaceLoadValueFactor = (float)vloadDirectioninLCS.Y;
                parameters_LCS_Y.eMemberLoadDirection = ELoadDirection.eLD_Y;
                parameters_LCS_Y.fMemberLoadValueSign = vloadDirectioninLCS.Y < 0.0 ? -1 : 1;

                listMemberLoadParams.Add(parameters_LCS_Y);
            }

            if (!MathF.d_equal(Math.Abs(vloadDirectioninLCS.Z), 0, 0.001))
            {
                MemberLoadParameters parameters_LCS_Z = new MemberLoadParameters();

                parameters_LCS_Z.fSurfaceLoadValueFactor = (float)vloadDirectioninLCS.Z;
                parameters_LCS_Z.eMemberLoadDirection = ELoadDirection.eLD_Z;
                parameters_LCS_Z.fMemberLoadValueSign = vloadDirectioninLCS.Z < 0.0 ? -1 : 1;

                listMemberLoadParams.Add(parameters_LCS_Z);
            }

            // Validation
            if (listMemberLoadParams.Count == 0) // Nepodarilo sa vygenerovat ziadne zatazenie pruta
            {
                throw new Exception("Error. Member load can't be generated.");
            }


            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            // Surface load - local coordinate system

            // Rectangular

            //            4 [0,cY]  _____________  3 [cX,cY]
            //                     |             |
            //                     |             |
            // ^ y                 |             |
            // |                   |_____________|
            // |          1 [0,0]                  2 [cX,0]
            // o----->x

            // Trapezoidal
            //                            4 [cX2,cY2]
            //                           /\
            //                          /  \
            //                         /    \
            //                        /      \
            //                       /        \
            //            5 [0,cY1] /          \  3 [cX1,cY1]
            //                     |            |
            // ^ y                 |            |
            // |                   |____________|
            // |          1 [0,0]                  2 [cX1,0]
            // o----->x

            //tu si nie som uplne isty,ci je dany 1. a 3. bodom (obdlznik je definovany 2 bodmi, bottomleft a topright)
            // To Ondrej - ak tomu rozumiem spravne tak rectangle Rect je definovany v lavotocivom systeme x,y s [0,0] v TopLeft,
            // zatial co body plochy su definovane v pravotocivom systeme x,y s [0,0] BottomLeft
            // Neviem ci to nemoze robit problemy

            Point p1r1 = Drawing3D.GetPoint_IgnoreZ(l.SurfaceDefinitionPoints[0]);
            Point p2r1 = Drawing3D.GetPoint_IgnoreZ(l.SurfaceDefinitionPoints[2]);

            // TO Ondrej - Tento rozmer musi byt vzdy kolmy na lokalnu osu x pruta
            bool bIsMemberLCS_xInSameDirectionAsLoadAxis_LCS_x;

            if (MathF.d_equal(pStartLCS.X, pEndLCS.X))
            {
                bIsMemberLCS_xInSameDirectionAsLoadAxis_LCS_x = false;
                pStartLCS.X -= fDist / 2;
                pEndLCS.X += fDist / 2;
            }
            else
            {
                bIsMemberLCS_xInSameDirectionAsLoadAxis_LCS_x = true;
                pStartLCS.Y -= fDist / 2;
                pEndLCS.Y += fDist / 2;
            }

            Point p1r2 = Drawing3D.GetPoint_IgnoreZ(pStartLCS);
            Point p2r2 = Drawing3D.GetPoint_IgnoreZ(pEndLCS);

            Rect loadRect = new Rect(p1r1, p2r1); // Rectangle defined in LCS of surface load
            Rect memberRect = new Rect(p1r2, p2r2); // To Ondrej Tu bol problem - vracia to napriklad obdlznik s nulovym rozmerom Height ak ma prut globalne Y suradnice rovnake, dorobil som podmienku if (MathF.d_equal(pStartLCS.X, pEndLCS.X))

            Rect intersection = Drawing3D.GetRectanglesIntersection(loadRect, memberRect);

            double dMemberLoadStartCoordinate_x_axis;
            double dIntersectionLengthInMember_x_axis;
            double dIntersectionLengthInMember_yz_axis;

            // TODO - toto by sa asi tiez dalo nejako pekne vyriesit cez Vector LCS pruta v LCS plochy, resp opacne
            // TODO - tu bude potrebne zapravoat ako je x plochy a x pruta vzajomne pootocene, ak o 180 stupnov, tak bude treba prehodit vsetko L - x
            if (bIsMemberLCS_xInSameDirectionAsLoadAxis_LCS_x) // x pruta a x plochy su na jednej priamke
            {
                if (memberRect.Left - loadRect.Left > 0)
                    dMemberLoadStartCoordinate_x_axis = 0; // Prut zacina za plochou
                else
                    dMemberLoadStartCoordinate_x_axis = loadRect.Left - memberRect.Left; // Prut zacina pred plochou

                // Opacny smer osi pruta x voci osi x load surface
                if (pStartLCS.X > pEndLCS.X)
                {
                    dMemberLoadStartCoordinate_x_axis = m.FLength - intersection.Width;

                    if (loadRect.Width >= pStartLCS.X)
                        dMemberLoadStartCoordinate_x_axis = 0;
                }

                dIntersectionLengthInMember_x_axis = intersection.Width;   // Length of applied load
                dIntersectionLengthInMember_yz_axis = intersection.Height; // Tributary width
            }
            else  // x pruta a x plochy nie na jednej priamke
            {
                if (memberRect.Top - loadRect.Top > 0)
                    dMemberLoadStartCoordinate_x_axis = 0; // Prut zacina za plochou
                else
                    dMemberLoadStartCoordinate_x_axis = loadRect.Top - memberRect.Top;  // Prut zacina pred plochou

                // Opacny smer osi pruta x voci osi y load surface
                if (pStartLCS.Y > pEndLCS.Y)
                {
                    dMemberLoadStartCoordinate_x_axis = m.FLength - intersection.Height;

                    if (loadRect.Height >= pStartLCS.Y)
                        dMemberLoadStartCoordinate_x_axis = 0;
                }

                dIntersectionLengthInMember_x_axis = intersection.Height; // Length of applied load
                dIntersectionLengthInMember_yz_axis = intersection.Width; // Tributary width
            }

            foreach (MemberLoadParameters loadparam in listMemberLoadParams)
            {
                float fq = (float)(loadparam.fMemberLoadValueSign * Math.Abs(l.fValue * loadparam.fSurfaceLoadValueFactor) * dIntersectionLengthInMember_yz_axis); // Load Value

                if ((lcTypeForLS == ELCGTypeForLimitState.eULSOnly || lcTypeForLS == ELCGTypeForLimitState.eSLSOnly) && lcType == ELCType.eWind)
                {
                    float fC_fig = 1.0f; // Wind aerodynamic factor factor

                    // Cfig.i = Cp,i * Kc,i
                    // Cfig.e = Cp,e * Ka * Kc,e * Kl * Kp

                    float fK_p = 1.0f; // TODO - mohlo by byt nastavitelne z GUI
                    if (!wind.bCondiderPermeableCladdingFactor_Kp)
                        fK_p = wind.fK_p;

                    float fK_l = 1.0f;
                    if (!wind.bConsiderLocalPressureFactor_Kl)
                        fK_l = GetLocalWindPressureFactor_K_l(wind, m, loadparam.eMemberLoadDirection);

                    float fK_a = 1.0f;
                    if (!wind.bConsiderAreaReductionFactor_Ka)
                        fK_a = AS_NZS_1170_2.Get_AreaReductionFactor_Ka_Table54((float)dIntersectionLengthInMember_yz_axis * m.FLength); // Faktor je konstanta pre cely prut - zavisi od zatazovacej plochy pruta

                    // External / Internal Pressure Coefficients - ako jediny je vzdy zohladneny uz vo vypocte surface loads
                    float fC_pi_aux = 1.0f; // Faktor je uz zahrnuty v surface load value
                    float fC_pe_aux = 1.0f; // Faktor je uz zahrnuty v surface load value

                    float fC_pi_real = 1.0f; // Faktor je uz zahrnuty v surface load value
                    float fC_pe_real = 1.0f; // Faktor je uz zahrnuty v surface load value

                    float fp_basic = float.MaxValue;

                    if (lcTypeForLS == ELCGTypeForLimitState.eULSOnly)
                    {
                        fp_basic = wind.fp_basic_ULS_Theta_4[(int)lCMainDirection];
                    }

                    if (lcTypeForLS == ELCGTypeForLimitState.eSLSOnly)
                    {
                        fp_basic = wind.fp_basic_SLS_Theta_4[(int)lCMainDirection];
                    }

                    if (lcWindType == ELCWindType.eWL_Cpi_min || lcWindType == ELCWindType.eWL_Cpi_max)
                        fC_pi_real = Math.Abs(l.fValue) / fp_basic; // Faktor je uz zahrnuty v surface load value
                    else if (lcWindType == ELCWindType.eWL_Cpe_min || lcWindType == ELCWindType.eWL_Cpe_max)
                        fC_pe_real = Math.Abs(l.fValue) / fp_basic; // Faktor je uz zahrnuty v surface load value
                    else
                    {
                        fC_pi_real = 1.0f; // Faktor je uz zahrnuty v surface load value
                        fC_pe_real = 1.0f; // Faktor je uz zahrnuty v surface load value
                    }

                    float fK_ci = 1.0f;
                    float fK_ce = 1.0f;

                    if (!wind.bConsiderCombinationFactor_Kci_and_Kce)
                    {
                        Set_ActionCombinationFactors_Kci(2, fC_pi_real, out fK_ci);
                        Set_ActionCombinationFactors_Kce(2, out fK_ce);
                    }

                    // Rozdiel pre hodnoty Kce na streche a na stene zatial nezohladujeme - bolo by potrebne rozlisit typ pruta a smer zatazenia podobne ako pre Kl
                    if (lcWindType == ELCWindType.eWL_Cpi_min || lcWindType == ELCWindType.eWL_Cpi_max)
                        fC_fig = AS_NZS_1170_2.Eq_52_1____(fC_pi_aux, fK_ci);
                    else if (lcWindType == ELCWindType.eWL_Cpe_min || lcWindType == ELCWindType.eWL_Cpe_max)
                        fC_fig = AS_NZS_1170_2.Eq_52_2____(fC_pe_aux, fK_ce, fK_a, fK_l, fK_p);
                     else
                        fC_fig = 1.0f;

                    fq *= fC_fig; // Upravime hodnotu zatazenia
                }

                if (intersection == Rect.Empty)
                {
                    return;
                }
                else if (MathF.d_equal(dIntersectionLengthInMember_x_axis, m.FLength)) // Intersection in x direction of member is same as member length - generate uniform load per whole member length
                {
                    listOfMemberLoads.Add(new CMLoad_21(iLoadID, fq, m, EMLoadTypeDistr.eMLT_QUF_W_21, ELoadType.eLT_F, ELoadCoordSystem.eLCS, loadparam.eMemberLoadDirection, true, 0));
                    iLoadID += 1;
                }
                else
                {
                    //nie som si isty,ci to je spravne
                    float faA = (float)dMemberLoadStartCoordinate_x_axis; // Load start point on member (absolute coordinate x)
                    float fs = (float)dIntersectionLengthInMember_x_axis; // Load segment length on member (absolute coordinate x)

                    listOfMemberLoads.Add(new CMLoad_24(iLoadID, fq, faA, fs, m, EMLoadTypeDistr.eMLT_QUF_PG_24, ELoadType.eLT_F, ELoadCoordSystem.eLCS, loadparam.eMemberLoadDirection, true, 0));
                    iLoadID += 1;
                }
            }
        }

        private static float GetLocalWindPressureFactor_K_l(CCalcul_1170_2 wind, CMember m, ELoadDirection loadDirection)
        {
            if (wind != null) // Jedna sa o load case s externym tlakom / sanim vetra
            {
                // Pruty podporujuce cladding
                if (m.EMemberType == EMemberType_FS.eBG || m.EMemberType == EMemberType_FS.eG) // Girts
                    return wind.fLocalPressureFactorKl_Girt;
                else if (m.EMemberType == EMemberType_FS.eP)
                    return wind.fLocalPressureFactorKl_Purlin;
                else if (m.EMemberType == EMemberType_FS.eEP)
                {
                    if (loadDirection == ELoadDirection.eLD_Y)
                        return wind.fLocalPressureFactorKl_EavePurlin_Wall;
                    else if (loadDirection == ELoadDirection.eLD_Z)
                        return wind.fLocalPressureFactorKl_EavePurlin_Roof;
                    else
                        return 1.0f;
                }
                else // Iny typ pruta - nie je v kontakte s cladding
                    return 1.0f;
            }
            return 1.0f;
        }

        //public static List<Point3D> GetSurfaceLoadCoordinates_GCS(CSLoad_FreeUniform load, Transform3D groupTransform)
        //{
        //    Model3DGroup gr = load.CreateM_3D_G_Load();
        //    if (gr.Children.Count < 1) return new List<Point3D>();

        //    Transform3DGroup trans = new Transform3DGroup();
        //    trans.Children.Add(gr.Transform);
        //    if (groupTransform != null)
        //    {
        //        trans.Children.Add(groupTransform);
        //    }

        //    List<Point3D> transPoints = new List<Point3D>();
        //    foreach (Point3D p in load.SurfaceDefinitionPoints)
        //        transPoints.Add(trans.Transform(p));

        //    return transPoints;
        //}
        public static List<Point3D> GetSurfaceLoadCoordinates_GCS(CSLoad_FreeUniform load, Transform3D groupTransform)
        {
            Transform3DGroup trans = new Transform3DGroup();
            trans.Children.Add(load.CreateTransformCoordGroup());
            if (groupTransform != null)
            {
                trans.Children.Add(groupTransform);
            }

            List<Point3D> transPoints = new List<Point3D>();
            foreach (Point3D p in load.SurfaceDefinitionPoints)
                transPoints.Add(trans.Transform(p));

            return transPoints;
        }

        public static GeneralTransform3D GetSurfaceLoadTransformFromGCSToLCS(CSLoad_FreeUniform load, Transform3D groupTransform)
        {            
            Transform3DGroup trans = new Transform3DGroup();
            trans.Children.Add(load.CreateTransformCoordGroup());
            if (groupTransform != null)
            {
                trans.Children.Add(groupTransform);
            }
            return trans.Inverse;
        }
        public static Transform3DGroup GetSurfaceLoadTransformFromLCSToGCS(CSLoad_FreeUniform load, Transform3D groupTransform)
        {
            Transform3DGroup trans = new Transform3DGroup();
            trans.Children.Add(load.CreateTransformCoordGroup());
            if (groupTransform != null)
            {
                trans.Children.Add(groupTransform);
            }
            return trans;
        }

        public static Vector3D GetTransformedVector(Vector3D v, Transform3D transformation)
        {
            Vector3D v_out = new Vector3D();

            v_out = transformation.Transform(v);

            return v_out;
        }

        public static Vector3D GetTransformedVector(Vector3D v, GeneralTransform3D transformation)
        {
            Vector3D v_out = new Vector3D();
            Point3D p_out = new Point3D();

            p_out = transformation.Transform(new Point3D(v.X, v.Y, v.Z));

            // Set output vector
            v_out.X = p_out.X;
            v_out.Y = p_out.Y;
            v_out.Z = p_out.Z;

            return v_out;
        }

        public static Matrix3D GetLocalToGlobalTransformMatrix(Vector3D vGlobalVector, Vector3D vLocalVector)
        {
            Vector3D X1 = new Vector3D(vGlobalVector.X, 0, 0);
            Vector3D X2 = new Vector3D(0, vGlobalVector.Y, 0);
            Vector3D X3 = new Vector3D(0, 0, vGlobalVector.Z);

            Vector3D X1_LCS = new Vector3D(vLocalVector.X, 0, 0);
            Vector3D X2_LCS = new Vector3D(0, vLocalVector.Y, 0);
            Vector3D X3_LCS = new Vector3D(0, 0, vLocalVector.Z);

            // This matrix will transform points from the rotated axis to the global
            Matrix3D LocalToGlobalTransformMatrix = new Matrix3D(
                Vector3D.DotProduct(X1, X1_LCS),
                Vector3D.DotProduct(X1, X2_LCS),
                Vector3D.DotProduct(X1, X3_LCS),
                0,
                Vector3D.DotProduct(X2, X1_LCS),
                Vector3D.DotProduct(X2, X2_LCS),
                Vector3D.DotProduct(X2, X3_LCS),
                0,
                Vector3D.DotProduct(X3, X1_LCS),
                Vector3D.DotProduct(X3, X2_LCS),
                Vector3D.DotProduct(X3, X3_LCS),
                0,

                0,
                0,
                0,
                1);

            return LocalToGlobalTransformMatrix;
        }

        public static Matrix3D GetGlobalToLocalTransformMatrix(Vector3D vGlobalVector, Vector3D vLocalVector)
        {
            Vector3D X1 = new Vector3D(vGlobalVector.X, 0, 0);
            Vector3D X2 = new Vector3D(0, vGlobalVector.Y, 0);
            Vector3D X3 = new Vector3D(0, 0, vGlobalVector.Z);

            Vector3D X1_LCS = new Vector3D(vLocalVector.X, 0, 0);
            Vector3D X2_LCS = new Vector3D(0, vLocalVector.Y, 0);
            Vector3D X3_LCS = new Vector3D(0, 0, vLocalVector.Z);

            // This matrix will transform points from the global system back to the rotated axis
            Matrix3D GlobalToLocalTransformMatrix = new Matrix3D(
                Vector3D.DotProduct(X1_LCS, X1),
                Vector3D.DotProduct(X1_LCS, X2),
                Vector3D.DotProduct(X1_LCS, X3),
                0,
                Vector3D.DotProduct(X2_LCS, X1),
                Vector3D.DotProduct(X2_LCS, X2),
                Vector3D.DotProduct(X2_LCS, X3),
                0,
                Vector3D.DotProduct(X3_LCS, X1),
                Vector3D.DotProduct(X3_LCS, X2),
                Vector3D.DotProduct(X3_LCS, X3),
                0,

                0,
                0,
                0,
                1);

            return GlobalToLocalTransformMatrix;
        }

        public float GetBaysWidthUntilFrameIndex(int frameIndex)
        {
            float w = 0;
            for (int i = 0; i < frameIndex; i++)
            {
                w += m_L1_Bays[i];
            }
            return w;
        }
    }
}
