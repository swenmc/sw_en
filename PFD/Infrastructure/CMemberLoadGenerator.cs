using BaseClasses;
using CRSC;
using M_EC1.AS_NZS;
using MATH;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFD
{
    public class CMemberLoadGenerator
    {
        private int iFrameNo;
        private float fL1_frame;
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

        public CMemberLoadGenerator(
            int frameNo,
            float L1_frame,
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
            iFrameNo = frameNo;
            fL1_frame = L1_frame;
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

        public CMemberLoadGenerator(CModel_PFD_01_GR model, CCalcul_1170_1 generalLoad, CCalcul_1170_3 snow, CCalcul_1170_2 calc_wind)
        {
            iFrameNo = model.iFrameNo;
            fL1_frame = model.fL1_frame;
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
                (wind.bConsiderAreaReductionFactor_Ka == true || wind.bConsiderAreaReductionFactor_Kci_and_Kce == true))
            {
                // Ak uz boli K_ca, Kce alebo Kci redukovane, tak je nastavenie pre tento generator nevalidne
                throw new Exception("Wind pressure reduction factor Kci, Kce or Ka was already considered. Check options.");
            }
        }


        // Loading

        // Frame member loads
        public void GenerateLoadsOnFrames()
        {
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

            for (int i = 0; i < iFrameNo; i++)
            {
                // Generate loads on member of particular frame
                GenerateLoadsOnFrame(i,
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
            
            // Assign generated member loads to the load cases
            // Universal
            m_arrLoadCases[(int)ELCName.eDL_G].MemberLoadsList = memberLoadDead;
            m_arrLoadCases[(int)ELCName.eIL_Q].MemberLoadsList = memberLoadImposed;
            
            // ULS
            m_arrLoadCases[(int)ELCName.eSL_Su_Full].MemberLoadsList = memberMaxLoadSnowAll_ULS;
            m_arrLoadCases[(int)ELCName.eSL_Su_Left].MemberLoadsList = memberMaxLoadSnowLeft_ULS;
            m_arrLoadCases[(int)ELCName.eSL_Su_Right].MemberLoadsList = memberMaxLoadSnowRight_ULS;
            m_arrLoadCases[(int)ELCName.eWL_Wu_Cpi_min_Left_X_Plus].MemberLoadsList = memberLoadInternalPressure_ULS_Cpimin_Left;
            m_arrLoadCases[(int)ELCName.eWL_Wu_Cpi_min_Right_X_Minus].MemberLoadsList = memberLoadInternalPressure_ULS_Cpimin_Right;
            m_arrLoadCases[(int)ELCName.eWL_Wu_Cpi_min_Front_Y_Plus].MemberLoadsList = memberLoadInternalPressure_ULS_Cpimin_Front;
            m_arrLoadCases[(int)ELCName.eWL_Wu_Cpi_min_Rear_Y_Minus].MemberLoadsList = memberLoadInternalPressure_ULS_Cpimin_Rear;
            m_arrLoadCases[(int)ELCName.eWL_Wu_Cpi_max_Left_X_Plus].MemberLoadsList = memberLoadInternalPressure_ULS_Cpimax_Left;
            m_arrLoadCases[(int)ELCName.eWL_Wu_Cpi_max_Right_X_Minus].MemberLoadsList = memberLoadInternalPressure_ULS_Cpimax_Right;
            m_arrLoadCases[(int)ELCName.eWL_Wu_Cpi_max_Front_Y_Plus].MemberLoadsList = memberLoadInternalPressure_ULS_Cpimax_Front;
            m_arrLoadCases[(int)ELCName.eWL_Wu_Cpi_max_Rear_Y_Minus].MemberLoadsList = memberLoadInternalPressure_ULS_Cpimax_Rear;
            m_arrLoadCases[(int)ELCName.eWL_Wu_Cpe_min_Left_X_Plus].MemberLoadsList = memberLoadExternalPressure_ULS_Cpemin_Left;
            m_arrLoadCases[(int)ELCName.eWL_Wu_Cpe_min_Right_X_Minus].MemberLoadsList = memberLoadExternalPressure_ULS_Cpemin_Right;
            m_arrLoadCases[(int)ELCName.eWL_Wu_Cpe_min_Front_Y_Plus].MemberLoadsList = memberLoadExternalPressure_ULS_Cpemin_Front;
            m_arrLoadCases[(int)ELCName.eWL_Wu_Cpe_min_Rear_Y_Minus].MemberLoadsList = memberLoadExternalPressure_ULS_Cpemin_Rear;
            m_arrLoadCases[(int)ELCName.eWL_Wu_Cpe_max_Left_X_Plus].MemberLoadsList = memberLoadExternalPressure_ULS_Cpemax_Left;
            m_arrLoadCases[(int)ELCName.eWL_Wu_Cpe_max_Right_X_Minus].MemberLoadsList = memberLoadExternalPressure_ULS_Cpemax_Right;
            m_arrLoadCases[(int)ELCName.eWL_Wu_Cpe_max_Front_Y_Plus].MemberLoadsList = memberLoadExternalPressure_ULS_Cpemax_Front;
            m_arrLoadCases[(int)ELCName.eWL_Wu_Cpe_max_Rear_Y_Minus].MemberLoadsList = memberLoadExternalPressure_ULS_Cpemax_Rear;            

            // SLS
            m_arrLoadCases[(int)ELCName.eSL_Ss_Full].MemberLoadsList = memberMaxLoadSnowAll_SLS;
            m_arrLoadCases[(int)ELCName.eSL_Ss_Left].MemberLoadsList = memberMaxLoadSnowLeft_SLS;
            m_arrLoadCases[(int)ELCName.eSL_Ss_Right].MemberLoadsList = memberMaxLoadSnowRight_SLS;
            m_arrLoadCases[(int)ELCName.eWL_Ws_Cpi_min_Left_X_Plus].MemberLoadsList = memberLoadInternalPressure_SLS_Cpimin_Left;
            m_arrLoadCases[(int)ELCName.eWL_Ws_Cpi_min_Right_X_Minus].MemberLoadsList = memberLoadInternalPressure_SLS_Cpimin_Right;
            m_arrLoadCases[(int)ELCName.eWL_Ws_Cpi_min_Front_Y_Plus].MemberLoadsList = memberLoadInternalPressure_SLS_Cpimin_Front;
            m_arrLoadCases[(int)ELCName.eWL_Ws_Cpi_min_Rear_Y_Minus].MemberLoadsList = memberLoadInternalPressure_SLS_Cpimin_Rear;
            m_arrLoadCases[(int)ELCName.eWL_Ws_Cpi_max_Left_X_Plus].MemberLoadsList = memberLoadInternalPressure_SLS_Cpimax_Left;
            m_arrLoadCases[(int)ELCName.eWL_Ws_Cpi_max_Right_X_Minus].MemberLoadsList = memberLoadInternalPressure_SLS_Cpimax_Right;
            m_arrLoadCases[(int)ELCName.eWL_Ws_Cpi_max_Front_Y_Plus].MemberLoadsList = memberLoadInternalPressure_SLS_Cpimax_Front;
            m_arrLoadCases[(int)ELCName.eWL_Ws_Cpi_max_Rear_Y_Minus].MemberLoadsList = memberLoadInternalPressure_SLS_Cpimax_Rear;
            m_arrLoadCases[(int)ELCName.eWL_Ws_Cpe_min_Left_X_Plus].MemberLoadsList = memberLoadExternalPressure_SLS_Cpemin_Left;
            m_arrLoadCases[(int)ELCName.eWL_Ws_Cpe_min_Right_X_Minus].MemberLoadsList = memberLoadExternalPressure_SLS_Cpemin_Right;
            m_arrLoadCases[(int)ELCName.eWL_Ws_Cpe_min_Front_Y_Plus].MemberLoadsList = memberLoadExternalPressure_SLS_Cpemin_Front;
            m_arrLoadCases[(int)ELCName.eWL_Ws_Cpe_min_Rear_Y_Minus].MemberLoadsList = memberLoadExternalPressure_SLS_Cpemin_Rear;
            m_arrLoadCases[(int)ELCName.eWL_Ws_Cpe_max_Left_X_Plus].MemberLoadsList = memberLoadExternalPressure_SLS_Cpemax_Left;
            m_arrLoadCases[(int)ELCName.eWL_Ws_Cpe_max_Right_X_Minus].MemberLoadsList = memberLoadExternalPressure_SLS_Cpemax_Right;
            m_arrLoadCases[(int)ELCName.eWL_Ws_Cpe_max_Front_Y_Plus].MemberLoadsList = memberLoadExternalPressure_SLS_Cpemax_Front;
            m_arrLoadCases[(int)ELCName.eWL_Ws_Cpe_max_Rear_Y_Minus].MemberLoadsList = memberLoadExternalPressure_SLS_Cpemax_Rear;
        }

        public void GenerateLoadsOnFrame(
            int iFrameIndex,
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
            int iEavesPurlinNoInOneFrame = 2;
            int iFrameNodesNo = 5;

            int indexColumn1Left = (iFrameIndex * iEavesPurlinNoInOneFrame) + iFrameIndex * (iFrameNodesNo - 1) + 0;
            int indexColumn2Right = (iFrameIndex * iEavesPurlinNoInOneFrame) + iFrameIndex * (iFrameNodesNo - 1) + 3;
            int indexRafter1Left = (iFrameIndex * iEavesPurlinNoInOneFrame) + iFrameIndex * (iFrameNodesNo - 1) + 1;
            int indexRafter2Right = (iFrameIndex * iEavesPurlinNoInOneFrame) + iFrameIndex * (iFrameNodesNo - 1) + 2;

            float fFrameTributaryWidth = fL1_frame;
            float fFrameGCSCoordinate_Y = iFrameIndex * fL1_frame;

            // Additional surface dead load
            float fSelfWeight_Girts_SurfaceLoad = -(float)(GirtCrSc.A_g * GirtCrSc.m_Mat.m_fRho * GlobalConstants.fg_acceleration / fDistanceGirts);
            float fSelfWeight_Purlins_SurfaceLoad = -(float)(PurlinCrSc.A_g * PurlinCrSc.m_Mat.m_fRho * GlobalConstants.fg_acceleration / fDistancePurlins);

            float fSelfWeightColumn = -(float)(ColumnCrSc.A_g * ColumnCrSc.m_Mat.m_fRho * GlobalConstants.fg_acceleration);
            float fSelfWeightRafter = -(float)(RafterCrSc.A_g * RafterCrSc.m_Mat.m_fRho * GlobalConstants.fg_acceleration);

            // Half tributary width - first and last frame
            if (iFrameIndex == 0 || iFrameIndex == iFrameNo - 1)
            {
                fFrameTributaryWidth *= 0.5f;
                fSelfWeightColumn = -(float)(ColumnCrSc_EF.A_g * ColumnCrSc_EF.m_Mat.m_fRho * GlobalConstants.fg_acceleration);
                fSelfWeightRafter = -(float)(RafterCrSc_EF.A_g * RafterCrSc_EF.m_Mat.m_fRho * GlobalConstants.fg_acceleration);
            }

            // Total surface dead load
            float fValueLoadColumnDead_Surface = fValueLoadWallCladdingSelfWeight_SurfaceLoad + fSelfWeight_Girts_SurfaceLoad;
            float fValueLoadRafterDead_Surface = fValueLoadRoofCladdingSelfWeight_SurfaceLoad + fSelfWeight_Purlins_SurfaceLoad;

            // Final uniform linear member load
            float fValueLoadColumnDead = fValueLoadColumnDead_Surface * fFrameTributaryWidth + fSelfWeightColumn;
            float fValueLoadRafterDead = fValueLoadRafterDead_Surface * fFrameTributaryWidth + fSelfWeightRafter;

            // Dead Loads
            // Columns
            CMLoad loadColumnLeft_DL = new CMLoad_21(iFrameIndex * 4 + 1, fValueLoadColumnDead, m_arrMembers[indexColumn1Left], EMLoadTypeDistr.eMLT_QUF_W_21, ELoadType.eLT_F, ELoadCoordSystem.eLCS, ELoadDirection.eLD_X, true, 0);
            // Osovy system praveho stlpa smeruje zhora nadol, takze hodnota zatazenia v LCS je s opacnym znamienkom (* -1)
            CMLoad loadColumnRight_DL = new CMLoad_21(iFrameIndex * 4 + 2, -fValueLoadColumnDead, m_arrMembers[indexColumn2Right], EMLoadTypeDistr.eMLT_QUF_W_21, ELoadType.eLT_F, ELoadCoordSystem.eLCS, ELoadDirection.eLD_X, true, 0);
            memberLoadDead.Add(loadColumnLeft_DL);
            memberLoadDead.Add(loadColumnRight_DL);

            // Rafters
            // TODO - zapracovat do konstruktora nastavenie GCS smeru zatazenia, teraz je to nespravne v PCS
            CMLoad loadRafterLeft_DL = new CMLoad_21(iFrameIndex * 4 + 3, fValueLoadRafterDead, m_arrMembers[indexRafter1Left], EMLoadTypeDistr.eMLT_QUF_W_21, ELoadType.eLT_F, ELoadCoordSystem.eLCS, ELoadDirection.eLD_Z, true, 0);
            CMLoad loadRafterRight_DL = new CMLoad_21(iFrameIndex * 4 + 4, fValueLoadRafterDead, m_arrMembers[indexRafter2Right], EMLoadTypeDistr.eMLT_QUF_W_21, ELoadType.eLT_F, ELoadCoordSystem.eLCS, ELoadDirection.eLD_Z, true, 0);
            memberLoadDead.Add(loadRafterLeft_DL);
            memberLoadDead.Add(loadRafterRight_DL);

            // Imposed Loads - roof
            // Rafters
            // TODO - zapracovat do konstruktora nastavenie GCS smeru zatazenia, teraz je to nespravne v PCS
            CMLoad loadRafterLeft_IL = new CMLoad_21(iFrameIndex * 2 + 1, fValueLoadRafterImposed * fFrameTributaryWidth, m_arrMembers[1 + iFrameIndex * (2 + 2 + 2)], EMLoadTypeDistr.eMLT_QUF_W_21, ELoadType.eLT_F, ELoadCoordSystem.eLCS, ELoadDirection.eLD_Z, true, 0);
            CMLoad loadRafterRight_IL = new CMLoad_21(iFrameIndex * 2 + 2, fValueLoadRafterImposed * fFrameTributaryWidth, m_arrMembers[1 + iFrameIndex * (2 + 2 + 2) + 1], EMLoadTypeDistr.eMLT_QUF_W_21, ELoadType.eLT_F, ELoadCoordSystem.eLCS, ELoadDirection.eLD_Z, true, 0);
            memberLoadImposed.Add(loadRafterLeft_IL);
            memberLoadImposed.Add(loadRafterRight_IL);

            // Snow Loads - roof
            // Rafters
            // TODO - zapracovat do konstruktora nastavenie GCS smeru zatazenia, teraz je to nespravne v PCS
            CMLoad loadRafterLeft_SL1_All_ULS = new CMLoad_21(iFrameIndex * 2 + 1, fValueLoadRafterSnowULS_Nu_1 * fFrameTributaryWidth, m_arrMembers[indexRafter1Left], EMLoadTypeDistr.eMLT_QUF_W_21, ELoadType.eLT_F, ELoadCoordSystem.eLCS, ELoadDirection.eLD_Z, true, 0);
            CMLoad loadRafterRight_SL1_All_ULS = new CMLoad_21(iFrameIndex * 2 + 2, fValueLoadRafterSnowULS_Nu_1 * fFrameTributaryWidth, m_arrMembers[indexRafter2Right], EMLoadTypeDistr.eMLT_QUF_W_21, ELoadType.eLT_F, ELoadCoordSystem.eLCS, ELoadDirection.eLD_Z, true, 0);
            memberMaxLoadSnowAll_ULS.Add(loadRafterLeft_SL1_All_ULS);
            memberMaxLoadSnowAll_ULS.Add(loadRafterRight_SL1_All_ULS);

            // Rafters
            // TODO - zapracovat do konstruktora nastavenie GCS smeru zatazenia, teraz je to nespravne v PCS
            CMLoad loadRafterLeft_SL2_Left_ULS = new CMLoad_21(iFrameIndex + 1, fValueLoadRafterSnowULS_Nu_2 * fFrameTributaryWidth, m_arrMembers[indexRafter1Left], EMLoadTypeDistr.eMLT_QUF_W_21, ELoadType.eLT_F, ELoadCoordSystem.eLCS, ELoadDirection.eLD_Z, true, 0);
            memberMaxLoadSnowLeft_ULS.Add(loadRafterLeft_SL2_Left_ULS);

            // Rafters
            // TODO - zapracovat do konstruktora nastavenie GCS smeru zatazenia, teraz je to nespravne v PCS
            CMLoad loadRafterRight_SL3_Right_ULS = new CMLoad_21(iFrameIndex + 1, fValueLoadRafterSnowULS_Nu_2 * fFrameTributaryWidth, m_arrMembers[indexRafter2Right], EMLoadTypeDistr.eMLT_QUF_W_21, ELoadType.eLT_F, ELoadCoordSystem.eLCS, ELoadDirection.eLD_Z, true, 0);
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
                        0,
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
                        0,
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
                        0,
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
                        0,
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
                        1,
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
                        1,
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
                        1,
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
                        1,
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
                        0,
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
                        0,
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
                        0,
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
                        0,
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
                        1,
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
                        1,
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
                        1,
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
                        1,
                        wind,
                        ref memberLoadExternalPressure_ULS_Cpemax_Rear);

            // Snow Loads - roof
            // Rafters
            // TODO - zapracovat do konstruktora nastavenie GCS smeru zatazenia, teraz je to nespravne v PCS
            CMLoad loadRafterLeft_SL1_All_SLS = new CMLoad_21(iFrameIndex * 2 + 1, fValueLoadRafterSnowULS_Nu_1 * fFrameTributaryWidth, m_arrMembers[indexRafter1Left], EMLoadTypeDistr.eMLT_QUF_W_21, ELoadType.eLT_F, ELoadCoordSystem.eLCS, ELoadDirection.eLD_Z, true, 0);
            CMLoad loadRafterRight_SL1_All_SLS = new CMLoad_21(iFrameIndex * 2 + 2, fValueLoadRafterSnowSLS_Nu_1 * fFrameTributaryWidth, m_arrMembers[indexRafter2Right], EMLoadTypeDistr.eMLT_QUF_W_21, ELoadType.eLT_F, ELoadCoordSystem.eLCS, ELoadDirection.eLD_Z, true, 0);
            memberMaxLoadSnowAll_SLS.Add(loadRafterLeft_SL1_All_SLS);
            memberMaxLoadSnowAll_SLS.Add(loadRafterRight_SL1_All_SLS);

            // Rafters
            // TODO - zapracovat do konstruktora nastavenie GCS smeru zatazenia, teraz je to nespravne v PCS
            CMLoad loadRafterLeft_SL2_Left_SLS = new CMLoad_21(iFrameIndex + 1, fValueLoadRafterSnowSLS_Nu_2 * fFrameTributaryWidth, m_arrMembers[indexRafter1Left], EMLoadTypeDistr.eMLT_QUF_W_21, ELoadType.eLT_F, ELoadCoordSystem.eLCS, ELoadDirection.eLD_Z, true, 0);
            memberMaxLoadSnowLeft_SLS.Add(loadRafterLeft_SL2_Left_SLS);

            // Rafters
            // TODO - zapracovat do konstruktora nastavenie GCS smeru zatazenia, teraz je to nespravne v PCS
            CMLoad loadRafterRight_SL3_Right_SLS = new CMLoad_21(iFrameIndex + 1, fValueLoadRafterSnowSLS_Nu_2 * fFrameTributaryWidth, m_arrMembers[indexRafter2Right], EMLoadTypeDistr.eMLT_QUF_W_21, ELoadType.eLT_F, ELoadCoordSystem.eLCS, ELoadDirection.eLD_Z, true, 0);
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
                        0,
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
                        0,
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
                        0,
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
                        0,
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
                        1,
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
                        1,
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
                        1,
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
                        1,
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
                        0,
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
                        0,
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
                        0,
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
                        0,
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
                        1,
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
                        1,
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
                        1,
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
                        1,
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
           int iCodeForCpeMinMaxValue,
           CCalcul_1170_2 wind,
           ref List<CMLoad> listOfMemberLoads)
        {
            float fK_ci_min = 1;
            float fK_ci_max = 1;
            float fK_ci;

            if (bConsiderFactors_Kci_Kce_Ka_Generator)
            {
                Set_ActionCombinationFactors_Kci(
                            wind.fC_pi_min,
                            wind.fC_pi_max,
                            out fK_ci_min,
                            out fK_ci_max
                            );
            }

            float[] fp_i_Theta_4;

            if (eLSType == ELSType.eLS_ULS)
            {
                if (iCodeForCpeMinMaxValue == 0) // ULS - Cpi,min
                {
                    fp_i_Theta_4 = wind.fp_i_min_ULS_Theta_4;
                    fK_ci = fK_ci_min;
                }
                else // ULS - Cpi,max
                {
                    fp_i_Theta_4 = wind.fp_i_max_ULS_Theta_4;
                    fK_ci = fK_ci_max;
                }
            }
            else
            {
                if (iCodeForCpeMinMaxValue == 0) // SLS - Cpi,min
                {
                    fp_i_Theta_4 = wind.fp_i_min_SLS_Theta_4;
                    fK_ci = fK_ci_min;
                }
                else // SLS - Cpi,max
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
            int iCodeForCpeMinMaxValue,
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
                out fK_ce_min_roof,
                out fK_ce_max_roof,
                out fK_ce_wall
                );
            }

            // Left or Right Main Direction
            float fColumnLeftLoadValue;
            float fColumnRightLoadValue;

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

            float fReductionFactor_Ka_Kce_Column1Left = fK_a_Column1Left * fK_ce_wall < 0.8f ? 0.8f : fK_a_Column1Left * fK_ce_wall;
            float fReductionFactor_Ka_Kce_Column2Right = fK_a_Column2Right * fK_ce_wall < 0.8f ? 0.8f : fK_a_Column2Right * fK_ce_wall;

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
                if (iCodeForCpeMinMaxValue == 0) // ULS - Cpe,min
                {
                    fp_e_U_roof_Theta_4 = wind.fp_e_min_U_roof_ULS_Theta_4;
                    fp_e_D_roof_Theta_4 = wind.fp_e_min_D_roof_ULS_Theta_4;
                    fK_ce_roof = fK_ce_min_roof;
                }
                else // ULS - Cpe,max
                {
                    fp_e_U_roof_Theta_4 = wind.fp_e_max_U_roof_ULS_Theta_4;
                    fp_e_D_roof_Theta_4 = wind.fp_e_max_D_roof_ULS_Theta_4;
                    fK_ce_roof = fK_ce_max_roof;
                }
            }
            else
            {
                if (iCodeForCpeMinMaxValue == 0) // SLS - Cpe,min
                {
                    fp_e_U_roof_Theta_4 = wind.fp_e_min_U_roof_SLS_Theta_4;
                    fp_e_D_roof_Theta_4 = wind.fp_e_min_D_roof_SLS_Theta_4;
                    fK_ce_roof = fK_ce_min_roof;
                }
                else // ULS - Cpe,max
                {
                    fp_e_U_roof_Theta_4 = wind.fp_e_max_U_roof_SLS_Theta_4;
                    fp_e_D_roof_Theta_4 = wind.fp_e_max_D_roof_SLS_Theta_4;
                    fK_ce_roof = fK_ce_max_roof;
                }
            }

            float fReductionFactor_Ka_Kce_Rafter1Left = fK_a_Rafter1Left * fK_ce_roof < 0.8f ? 0.8f : fK_a_Rafter1Left * fK_ce_roof;
            float fReductionFactor_Ka_Kce_Rafter2Right = fK_a_Rafter2Right * fK_ce_roof < 0.8f ? 0.8f : fK_a_Rafter2Right * fK_ce_roof;

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
            int iCodeForCpeMinMaxValue,
            CCalcul_1170_2 wind,
            ref List<CMLoad> listOfMemberLoads)
        {
            // External pressure
            //listOfMemberLoads = new List<CMLoad>(); // Generate 4 loads - type uniform per whole length (CMLoad_21)

            if (iWindDirectionIndex != (int)ELCMainDirection.ePlusY && iWindDirectionIndex != (int)ELCMainDirection.eMinusY)
                return; // Invalid direction - return empty list of loads

            float fFrameCoordinate_GCS_Y = iFrameIndex * fL1_frame; // Expected equal distance between frames (bay width)

            if (iWindDirectionIndex != (int)ELCMainDirection.ePlusY) // Minus Y - Rear Wind Pressure
            {
                fFrameCoordinate_GCS_Y = fL_tot - fFrameCoordinate_GCS_Y;
            }

            // Set minimum and maximum coordinate of tributary area
            float fTributaryWidth_Y_Coordinate_Min = fFrameCoordinate_GCS_Y - 0.5f * fL1_frame;
            float fTributaryWidth_Y_Coordinate_Max = fFrameCoordinate_GCS_Y + 0.5f * fL1_frame;

            if (iFrameIndex == 0) // First Frame
            {
                if (iWindDirectionIndex == (int)ELCMainDirection.ePlusY) // First frame
                    fTributaryWidth_Y_Coordinate_Min = 0;
                else // Last frame
                    fTributaryWidth_Y_Coordinate_Max = (iFrameNo - 1) * fL1_frame;
            }

            if (iFrameIndex == iFrameNo - 1) // Last Frame
            {
                if (iWindDirectionIndex == (int)ELCMainDirection.ePlusY) // First frame
                    fTributaryWidth_Y_Coordinate_Max = (iFrameNo - 1) * fL1_frame;
                else // Last frame
                    fTributaryWidth_Y_Coordinate_Min = 0;
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
                out fK_ce_min_roof,
                out fK_ce_max_roof,
                out fK_ce_wall
                );
            }

            if (eLSType == ELSType.eLS_ULS)
            {
                fLoadValues_Columns = wind.fp_e_S_wall_ULS_Theta_4;

                if (iCodeForCpeMinMaxValue == 0)
                {
                    fLoadValues_Rafters = wind.fp_e_min_R_roof_ULS_Theta_4;
                    fK_ce_roof = fK_ce_min_roof;
                }
                else
                {
                    fLoadValues_Rafters = wind.fp_e_max_R_roof_ULS_Theta_4;
                    fK_ce_roof = fK_ce_max_roof;
                }
            }
            else
            {
                fLoadValues_Columns = wind.fp_e_S_wall_SLS_Theta_4;

                if (iCodeForCpeMinMaxValue == 0)
                {
                    fLoadValues_Rafters = wind.fp_e_min_R_roof_SLS_Theta_4;
                    fK_ce_roof = fK_ce_min_roof;
                }
                else
                {
                    fLoadValues_Rafters = wind.fp_e_max_R_roof_SLS_Theta_4;
                    fK_ce_roof = fK_ce_max_roof;
                }
            }

            float fReductionFactor_Ka_Kce_Column1Left =  fK_a_Column1Left  * fK_ce_wall < 0.8f ? 0.8f : fK_a_Column1Left  * fK_ce_wall;
            float fReductionFactor_Ka_Kce_Column2Right = fK_a_Column2Right * fK_ce_wall < 0.8f ? 0.8f : fK_a_Column2Right * fK_ce_wall;
            float fReductionFactor_Ka_Kce_Rafter1Left =  fK_a_Rafter1Left  * fK_ce_roof < 0.8f ? 0.8f : fK_a_Rafter1Left  * fK_ce_roof;
            float fReductionFactor_Ka_Kce_Rafter2Right = fK_a_Rafter2Right * fK_ce_roof < 0.8f ? 0.8f : fK_a_Rafter2Right * fK_ce_roof;

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

            fK_a_Column1Left = wind.Get_AreaReductionFactor_Ka(fTributaryArea_Column1Left);
            fK_a_Column2Right = wind.Get_AreaReductionFactor_Ka(fTributaryArea_Column2Right);
            fK_a_Rafter1Left = wind.Get_AreaReductionFactor_Ka(fTributaryArea_Rafter1Left);
            fK_a_Rafter2Right = wind.Get_AreaReductionFactor_Ka(fTributaryArea_Rafter2Right);
        }

        private void Set_ActionCombinationFactors_Kci(
            float fC_pi_min,
            float fC_pi_max,
            out float fK_ci_min,
            out float fK_ci_max
            )
        {
            // 5.4.3 Action combination factor
            fK_ci_min = 1.0f;
            fK_ci_max = 1.0f;

            if (Math.Abs(fC_pi_min) >= 0.2f) fK_ci_min = 0.8f; // TODO - dopracovat podla kombinacii external and internal pressure
            if (Math.Abs(fC_pi_max) >= 0.2f) fK_ci_max = 0.8f; // TODO - dopracovat podla kombinacii external and internal pressure
        }

        private void Set_ActionCombinationFactors_Kce(
            out float fK_ce_min,
            out float fK_ce_max,
            out float fK_ce_wall)
        {
            // 5.4.3 Action combination factor
            fK_ce_min = 0.8f; // TODO - dopracovat podla kombinacii external and internal pressure
            fK_ce_max = 0.8f; // TODO - dopracovat podla kombinacii external and internal pressure
            fK_ce_wall = 0.8f;
        }
    }
}
