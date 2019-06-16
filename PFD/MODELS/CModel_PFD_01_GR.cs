using BaseClasses;
using CRSC;
using BaseClasses.GraphObj;
using M_EC1.AS_NZS;
using MATERIAL;
using MATH;
using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Linq;
using System.Collections.ObjectModel;
using BaseClasses.Helpers;

namespace PFD
{
    public class CModel_PFD_01_GR : CModel_PFD
    {
        public float fH1_frame;
        float fH2_frame;
        public float fW_frame;
        public float fL1_frame;
        public float fRoofPitch_rad;
        public int iFrameNo;
        public float fL_tot;
        public float fDist_Girt;
        public float fDist_Purlin;
        public float fDist_FrontColumns;
        public float fDist_BackColumns;
        public float fBottomGirtPosition;
        public float fUpperGirtLimit;
        public float fDist_FrontGirts;
        public float fDist_BackGirts;
        public float fz_UpperLimitForFrontGirts;
        public float fz_UpperLimitForBackGirts;
        public float fFrontFrameRakeAngle_temp_rad;
        public float fBackFrameRakeAngle_temp_rad;
        public float fSlopeFactor;

        public int iFrameNodesNo = 5;

        public int iMainColumnNo;
        public int iRafterNo;
        public int iEavesPurlinNo;
        public int iGirtNoInOneFrame;
        public int iEavesPurlinNoInOneFrame = 2;
        public int iPurlinNoInOneFrame;
        public int iFrontColumnNoInOneFrame;
        public int iBackColumnNoInOneFrame;
        public int iFrontGirtsNoInOneFrame;
        public int iBackGirtsNoInOneFrame;

        int[] iArrNumberOfNodesPerFrontColumn;
        int[] iArrNumberOfNodesPerBackColumn;
        int iOneColumnGirtNo;

        private float fFrontFrameRakeAngle_deg;
        private float fBackFrameRakeAngle_deg;
        private int iMainColumnFlyBracing_EveryXXGirt;
        private int iOneRafterBackColumnNo = 0;
        private int iOneRafterFrontColumnNo = 0;
        private int iRafterFlyBracing_EveryXXPurlin;


        public List<CBlock_3D_001_DoorInBay> DoorsModels;
        public List<CBlock_3D_002_WindowInBay> WindowsModels;

        public CModel_PFD_01_GR
        (
                BuildingGeometryDataInput sGeometryInputData,                
                int iFrameNo_temp,                
                float fDist_Girt_temp,
                float fDist_Purlin_temp,
                float fDist_FrontColumns_temp,
                float fBottomGirtPosition_temp,
                float fFrontFrameRakeAngle_temp_deg,
                float fBackFrameRakeAngle_temp_deg,
                ObservableCollection<DoorProperties> doorBlocksProperties,
                ObservableCollection<WindowProperties> windowBlocksProperties,
                CComponentListVM componentListVM,
                List<CConnectionJointTypes> joints
            )
        {
            ObservableCollection<CComponentInfo> componentList = componentListVM?.ComponentList;
            fH1_frame = sGeometryInputData.fH_1;
            fW_frame = sGeometryInputData.fW;
            fL_tot = sGeometryInputData.fL;
            iFrameNo = iFrameNo_temp;
            fH2_frame = sGeometryInputData.fH_2;
            fFrontFrameRakeAngle_deg = fFrontFrameRakeAngle_temp_deg;
            fBackFrameRakeAngle_deg = fBackFrameRakeAngle_temp_deg;

            iFrameNo = iFrameNo_temp;                        
            fL1_frame = fL_tot / (iFrameNo - 1);

            fDist_Girt = fDist_Girt_temp;
            fDist_Purlin = fDist_Purlin_temp;
            fDist_FrontColumns = fDist_FrontColumns_temp;
            fBottomGirtPosition = fBottomGirtPosition_temp;
            fDist_FrontGirts = fDist_Girt_temp; // Ak nie je rovnake ako pozdlzne tak su koncove pruty sikmo pretoze sa uvazuje jeden uzol na stlpe pre pozdlzny aj priecny smer nosnikov
            fDist_BackGirts = fDist_Girt_temp;
            fFrontFrameRakeAngle_temp_rad = fFrontFrameRakeAngle_temp_deg * MathF.fPI / 180f;
            fBackFrameRakeAngle_temp_rad = fBackFrameRakeAngle_temp_deg * MathF.fPI / 180f;

            m_eSLN = ESLN.e3DD_1D; // 1D members in 3D model
            m_eNDOF = (int)ENDOF.e3DEnv; // DOF in 3D
            m_eGCS = EGCS.eGCSLeftHanded; // Global coordinate system

            fRoofPitch_rad = (float)Math.Atan((fH2_frame - fH1_frame) / (0.5f * fW_frame));

            iEavesPurlinNo = iEavesPurlinNoInOneFrame * (iFrameNo - 1);
            iMainColumnNo = iFrameNo * 2;
            iRafterNo = iFrameNo * 2;

            iOneColumnGirtNo = 0;
            iGirtNoInOneFrame = 0;

            m_arrMat = new CMat[11];
            m_arrCrSc = new CCrSc[11];

            // Materials
            // Materials List - Materials Array - Fill Data of Materials Array
            // TODO - napojit na GUI a na databazu
            m_arrMat[(int)EMemberGroupNames.eMainColumn] = MaterialFactory.GetMaterial(componentList[(int)EMemberGroupNames.eMainColumn].Material);
            m_arrMat[(int)EMemberGroupNames.eRafter] = MaterialFactory.GetMaterial(componentList[(int)EMemberGroupNames.eRafter].Material);
            m_arrMat[(int)EMemberGroupNames.eMainColumn_EF] = MaterialFactory.GetMaterial(componentList[(int)EMemberGroupNames.eMainColumn_EF].Material);
            m_arrMat[(int)EMemberGroupNames.eRafter_EF] = MaterialFactory.GetMaterial(componentList[(int)EMemberGroupNames.eRafter_EF].Material);
            m_arrMat[(int)EMemberGroupNames.eEavesPurlin] = MaterialFactory.GetMaterial(componentList[(int)EMemberGroupNames.eEavesPurlin].Material);
            m_arrMat[(int)EMemberGroupNames.eGirtWall] = MaterialFactory.GetMaterial(componentList[(int)EMemberGroupNames.eGirtWall].Material);
            m_arrMat[(int)EMemberGroupNames.ePurlin] = MaterialFactory.GetMaterial(componentList[(int)EMemberGroupNames.ePurlin].Material);
            m_arrMat[(int)EMemberGroupNames.eFrontColumn] = MaterialFactory.GetMaterial(componentList[(int)EMemberGroupNames.eFrontColumn].Material);
            m_arrMat[(int)EMemberGroupNames.eBackColumn] = MaterialFactory.GetMaterial(componentList[(int)EMemberGroupNames.eBackColumn].Material);
            m_arrMat[(int)EMemberGroupNames.eFrontGirt] = MaterialFactory.GetMaterial(componentList[(int)EMemberGroupNames.eFrontGirt].Material);
            m_arrMat[(int)EMemberGroupNames.eBackGirt] = MaterialFactory.GetMaterial(componentList[(int)EMemberGroupNames.eBackGirt].Material);

            // Cross-sections
            // CrSc List - CrSc Array - Fill Data of Cross-sections Array

            // TODO Ondrej - Nastavit objekt prierezu podla databazy models, tabulka KitsetGableRoofEnclosed alebo KitsetGableRoofEnclosedCrscID
            // Napojit na GUI

            m_arrCrSc[(int)EMemberGroupNames.eMainColumn] = CrScFactory.GetCrSc(componentList[(int)EMemberGroupNames.eMainColumn].Section);
            m_arrCrSc[(int)EMemberGroupNames.eRafter] = CrScFactory.GetCrSc(componentList[(int)EMemberGroupNames.eRafter].Section);
            m_arrCrSc[(int)EMemberGroupNames.eMainColumn_EF] = CrScFactory.GetCrSc(componentList[(int)EMemberGroupNames.eMainColumn_EF].Section);
            m_arrCrSc[(int)EMemberGroupNames.eRafter_EF] = CrScFactory.GetCrSc(componentList[(int)EMemberGroupNames.eRafter_EF].Section);
            m_arrCrSc[(int)EMemberGroupNames.eEavesPurlin] = CrScFactory.GetCrSc(componentList[(int)EMemberGroupNames.eEavesPurlin].Section);
            m_arrCrSc[(int)EMemberGroupNames.eGirtWall] = CrScFactory.GetCrSc(componentList[(int)EMemberGroupNames.eGirtWall].Section);
            m_arrCrSc[(int)EMemberGroupNames.ePurlin] = CrScFactory.GetCrSc(componentList[(int)EMemberGroupNames.ePurlin].Section);
            m_arrCrSc[(int)EMemberGroupNames.eFrontColumn] = CrScFactory.GetCrSc(componentList[(int)EMemberGroupNames.eFrontColumn].Section);
            m_arrCrSc[(int)EMemberGroupNames.eBackColumn] = CrScFactory.GetCrSc(componentList[(int)EMemberGroupNames.eBackColumn].Section);
            m_arrCrSc[(int)EMemberGroupNames.eFrontGirt] = CrScFactory.GetCrSc(componentList[(int)EMemberGroupNames.eFrontGirt].Section);
            m_arrCrSc[(int)EMemberGroupNames.eBackGirt] = CrScFactory.GetCrSc(componentList[(int)EMemberGroupNames.eBackGirt].Section);

            for (int i = 0; i < m_arrCrSc.Length; i++)
            {
                m_arrCrSc[i].ID = i + 1;
            }

            m_arrCrSc[(int)EMemberGroupNames.eMainColumn].CSColor = Colors.Chocolate;       // Main Column
            m_arrCrSc[(int)EMemberGroupNames.eRafter].CSColor = Colors.Green;               // Main Rafter
            m_arrCrSc[(int)EMemberGroupNames.eMainColumn_EF].CSColor = Colors.DarkOrchid;   // Main Column - Edge Frame
            m_arrCrSc[(int)EMemberGroupNames.eRafter_EF].CSColor = Colors.GreenYellow;      // Main Rafter - Edge Frame
            m_arrCrSc[(int)EMemberGroupNames.eEavesPurlin].CSColor = Colors.DarkCyan;       // Eaves Purlin
            m_arrCrSc[(int)EMemberGroupNames.eGirtWall].CSColor = Colors.Orange;            // Girt - Wall
            m_arrCrSc[(int)EMemberGroupNames.ePurlin].CSColor = Colors.SlateBlue;           // Purlin
            m_arrCrSc[(int)EMemberGroupNames.eFrontColumn].CSColor = Colors.BlueViolet;     // Front Column
            m_arrCrSc[(int)EMemberGroupNames.eBackColumn].CSColor = Colors.BlueViolet;     // Back Column
            m_arrCrSc[(int)EMemberGroupNames.eFrontGirt].CSColor = Colors.Brown;            // Front Girt
            m_arrCrSc[(int)EMemberGroupNames.eBackGirt].CSColor = Colors.YellowGreen;       // Back Girt

            // Member Groups
            listOfModelMemberGroups = new List<CMemberGroup>(11);

            CDatabaseComponents database_temp = new CDatabaseComponents(); // TODO - Ondrej - prerobit triedu na nacitanie z databazy
                                                                           // See UC component list

            // TODO - nastavovat v GUI - zaviest databazu pre rozne typy prutov a typy load combinations
            int iLimitFractionDenominator_PermanentLoad = 300;
            int iLimitFractionDenominator_Total = 150;
            int iLimitFractionDenominator_Total_Frame = 250;

            float fLimitPermanentLoad = 1f / 300f;
            float fLimitTotal = 1 / 150f;
            float fLimitTotal_Frame = 1f / 250f;

            listOfModelMemberGroups.Add(new CMemberGroup(1, componentList[(int)EMemberGroupNames.eMainColumn].ComponentName, EMemberType_FS.eMC, EMemberType_FS_Position.MainColumn, m_arrCrSc[(int)EMemberGroupNames.eMainColumn], iLimitFractionDenominator_PermanentLoad, iLimitFractionDenominator_Total_Frame, 0));
            listOfModelMemberGroups.Add(new CMemberGroup(2, componentList[(int)EMemberGroupNames.eRafter].ComponentName, EMemberType_FS.eMR, EMemberType_FS_Position.MainRafter, m_arrCrSc[(int)EMemberGroupNames.eRafter], iLimitFractionDenominator_PermanentLoad, iLimitFractionDenominator_Total_Frame, 0));
            listOfModelMemberGroups.Add(new CMemberGroup(3, componentList[(int)EMemberGroupNames.eMainColumn_EF].ComponentName, EMemberType_FS.eEC, EMemberType_FS_Position.EdgeColumn, m_arrCrSc[(int)EMemberGroupNames.eMainColumn_EF], iLimitFractionDenominator_PermanentLoad, iLimitFractionDenominator_Total_Frame, 0));
            listOfModelMemberGroups.Add(new CMemberGroup(4, componentList[(int)EMemberGroupNames.eRafter_EF].ComponentName, EMemberType_FS.eER, EMemberType_FS_Position.EdgeRafter, m_arrCrSc[(int)EMemberGroupNames.eRafter_EF], iLimitFractionDenominator_PermanentLoad, iLimitFractionDenominator_Total_Frame, 0));
            listOfModelMemberGroups.Add(new CMemberGroup(5, componentList[(int)EMemberGroupNames.eEavesPurlin].ComponentName, EMemberType_FS.eEP, EMemberType_FS_Position.EdgePurlin, m_arrCrSc[(int)EMemberGroupNames.eEavesPurlin], iLimitFractionDenominator_PermanentLoad, iLimitFractionDenominator_Total, 0));
            listOfModelMemberGroups.Add(new CMemberGroup(6, componentList[(int)EMemberGroupNames.eGirtWall].ComponentName, EMemberType_FS.eG, EMemberType_FS_Position.Girt, m_arrCrSc[(int)EMemberGroupNames.eGirtWall], iLimitFractionDenominator_PermanentLoad, iLimitFractionDenominator_Total, 0));
            listOfModelMemberGroups.Add(new CMemberGroup(7, componentList[(int)EMemberGroupNames.ePurlin].ComponentName, EMemberType_FS.eP, EMemberType_FS_Position.Purlin, m_arrCrSc[(int)EMemberGroupNames.ePurlin], iLimitFractionDenominator_PermanentLoad, iLimitFractionDenominator_Total, 0));
            listOfModelMemberGroups.Add(new CMemberGroup(8, componentList[(int)EMemberGroupNames.eFrontColumn].ComponentName, EMemberType_FS.eC, EMemberType_FS_Position.ColumnFrontSide, m_arrCrSc[(int)EMemberGroupNames.eFrontColumn], iLimitFractionDenominator_PermanentLoad, iLimitFractionDenominator_Total, 0));
            listOfModelMemberGroups.Add(new CMemberGroup(9, componentList[(int)EMemberGroupNames.eBackColumn].ComponentName, EMemberType_FS.eC, EMemberType_FS_Position.ColumnBackSide, m_arrCrSc[(int)EMemberGroupNames.eBackColumn], iLimitFractionDenominator_PermanentLoad, iLimitFractionDenominator_Total, 0));
            listOfModelMemberGroups.Add(new CMemberGroup(10, componentList[(int)EMemberGroupNames.eFrontGirt].ComponentName, EMemberType_FS.eG, EMemberType_FS_Position.GirtFrontSide, m_arrCrSc[(int)EMemberGroupNames.eFrontGirt], iLimitFractionDenominator_PermanentLoad, iLimitFractionDenominator_Total, 0));
            listOfModelMemberGroups.Add(new CMemberGroup(11, componentList[(int)EMemberGroupNames.eBackGirt].ComponentName, EMemberType_FS.eG, EMemberType_FS_Position.GirtBackSide, m_arrCrSc[(int)EMemberGroupNames.eBackGirt], iLimitFractionDenominator_PermanentLoad, iLimitFractionDenominator_Total, 0));

            // Priradit material prierezov, asi by sa to malo robit uz pri vytvoreni prierezu ale trebalo by upravovat konstruktory :)
            if (m_arrMat.Length >= m_arrCrSc.Length)
            {
                for (int i = 0; i < m_arrCrSc.Length; i++)
                {
                    m_arrCrSc[i].m_Mat = m_arrMat[i];
                }
            }
            else
                throw new Exception("Cross-section material is not defined.");

            // Member Eccentricities
            // Zadane hodnoty predpokladaju ze prierez je symetricky, je potrebne zobecnit
            CMemberEccentricity eccentricityPurlin = new CMemberEccentricity(0, (float)(0.5 * m_arrCrSc[(int)EMemberGroupNames.eRafter].h - 0.5 * m_arrCrSc[(int)EMemberGroupNames.ePurlin].h));
            CMemberEccentricity eccentricityGirtLeft_X0 = new CMemberEccentricity(0, (float)(-(0.5 * m_arrCrSc[(int)EMemberGroupNames.eMainColumn].h - 0.5 * m_arrCrSc[(int)EMemberGroupNames.eGirtWall].h)));
            CMemberEccentricity eccentricityGirtRight_XB = new CMemberEccentricity(0, (float)(0.5 * m_arrCrSc[(int)EMemberGroupNames.eMainColumn].h - 0.5 * m_arrCrSc[(int)EMemberGroupNames.eGirtWall].h));

            CMemberEccentricity eccentricityEavePurlin = new CMemberEccentricity(-(float)(0.5 * m_arrCrSc[(int)EMemberGroupNames.eMainColumn].h + m_arrCrSc[(int)EMemberGroupNames.eEavesPurlin].y_min), 0);

            CMemberEccentricity eccentricityColumnFront_Z = new CMemberEccentricity(0, -(float)(0.5 * m_arrCrSc[(int)EMemberGroupNames.eRafter].b + 0.5 * m_arrCrSc[(int)EMemberGroupNames.eFrontColumn].h));
            CMemberEccentricity eccentricityColumnBack_Z = new CMemberEccentricity(0, (float)(0.5 * m_arrCrSc[(int)EMemberGroupNames.eRafter].b + 0.5 * m_arrCrSc[(int)EMemberGroupNames.eBackColumn].h));

            CMemberEccentricity eccentricityGirtFront_Y0 = new CMemberEccentricity(0, 0);
            CMemberEccentricity eccentricityGirtBack_YL = new CMemberEccentricity(0, 0);

            // Member Intermediate Supports
            m_arrIntermediateTransverseSupports = new CIntermediateTransverseSupport[1];
            CIntermediateTransverseSupport forkSupport = new CIntermediateTransverseSupport(1, EITSType.eBothFlanges, 0);
            m_arrIntermediateTransverseSupports[0] = forkSupport;

            bool bUseDefaultOrUserDefinedValueForFlyBracing = true; // TODO - zaviest checkbox ci sa maju pouzit hodnoty z databazy / uzivatelom nastavene, alebo sa ma generovat uplne automaticky

            // Frame column fly bracing
            // Index of girt 0 - no bracing 1 - every, 2 - every second girt, 3 - every third girt, ...
            // Poziciu fly bracing - kazdy xx girt nastavovat v GUI, alebo umoznit urcit automaticky, napr. cca tak aby bola vdialenost medzi fly bracing rovna L1

            bool bUseMainColumnFlyBracingPlates = true; // Use fly bracing plates in girt to column joint
            
            if (bUseDefaultOrUserDefinedValueForFlyBracing)
                iMainColumnFlyBracing_EveryXXGirt = sGeometryInputData.iMainColumnFlyBracingEveryXXGirt;
            else
                iMainColumnFlyBracing_EveryXXGirt = Math.Max(0, (int)(fL1_frame / fDist_Girt));

            // Rafter fly bracing
            // Index of purlin 0 - no bracing 1 - every, 2 - every second purlin, 3 - every third purlin, ...
            // Poziciu fly bracing - kazda xx purlin nastavovat v GUI, alebo umoznit urcit automaticky, napr. cca tak aby bola vdialenost medzi fly bracing rovna L1

            bool bUseRafterFlyBracingPlates = true; // Use fly bracing plates in purlin to rafter joint
            

            if (bUseDefaultOrUserDefinedValueForFlyBracing)
                iRafterFlyBracing_EveryXXPurlin = sGeometryInputData.iRafterFlyBracingEveryXXPurlin;
            else
                iRafterFlyBracing_EveryXXPurlin = Math.Max(0, (int)(fL1_frame / fDist_Purlin));

            // Front and Back Column
            bool bUseFrontColumnFlyBracingPlates = true; // Use fly bracing plates in girt to column joint
            int iFrontColumnFlyBracing_EveryXXGirt = sGeometryInputData.iFrontColumnFlyBracingEveryXXGirt;

            bool bUseBackColumnFlyBracingPlates = true; // Use fly bracing plates in girt to column joint
            int iBackColumnFlyBracing_EveryXXGirt = sGeometryInputData.iBackColumnFlyBracingEveryXXGirt;

            // Transverse bracing - girts, purlins, front girts, back girts
            /*
            bool bUseTransverseBracingBeam_Purlins = true;
            bool bUseTransverseBracingBeam_Girts = true;
            bool bUseTransverseBracingBeam_FrontGirts = true;
            bool bUseTransverseBracingBeam_BackGirts = true;
            */
            int iNumberOfTransverseSupports_EdgePurlins = sGeometryInputData.iEdgePurlin_ILS_Number; // TODO - napojit na generovanie bracing blocks alebo zadavat rucne v GUI
            int iNumberOfTransverseSupports_Purlins = sGeometryInputData.iPurlin_ILS_Number;
            int iNumberOfTransverseSupports_Girts = sGeometryInputData.iGirt_ILS_Number;
            int iNumberOfTransverseSupports_FrontGirts = sGeometryInputData.iGirtFrontSide_ILS_Number;
            int iNumberOfTransverseSupports_BackGirts = sGeometryInputData.iGirtBackSide_ILS_Number;

            // Limit pre poziciu horneho nosnika, mala by to byt polovica suctu vysky edge (eave) purlin h a sirky nosnika b (neberie sa h pretoze nosnik je otoceny o 90 stupnov)
            fUpperGirtLimit = (float)(m_arrCrSc[(int)EMemberGroupNames.eEavesPurlin].h + m_arrCrSc[(int)EMemberGroupNames.eGirtWall].b);

            // Limit pre poziciu horneho nosnika (front / back girt) na prednej alebo zadnej stene budovy
            // Nosnik alebo pripoj nosnika nesmie zasahovat do prievlaku (rafter)
            fz_UpperLimitForFrontGirts = (float)((0.5 * m_arrCrSc[(int)EMemberGroupNames.eRafter].h) / Math.Cos(fRoofPitch_rad) + 0.5f * m_arrCrSc[(int)EMemberGroupNames.eFrontGirt].b);
            fz_UpperLimitForBackGirts = (float)((0.5 * m_arrCrSc[(int)EMemberGroupNames.eRafter].h) / Math.Cos(fRoofPitch_rad) + 0.5f * m_arrCrSc[(int)EMemberGroupNames.eBackGirt].b);

            // Side wall - girts
            bool bGenerateGirts = componentList[(int)EMemberGroupNames.eGirtWall].Generate.Value;
            if (bGenerateGirts)
            {
                iOneColumnGirtNo = (int)((fH1_frame - fUpperGirtLimit - fBottomGirtPosition) / fDist_Girt) + 1;
                iGirtNoInOneFrame = 2 * iOneColumnGirtNo;
            }
            componentListVM.SetColumnFlyBracingPosition_Items(iOneColumnGirtNo);

            if (!bGenerateGirts || iMainColumnFlyBracing_EveryXXGirt == 0 || iMainColumnFlyBracing_EveryXXGirt > iGirtNoInOneFrame) // Index 0 means do not use fly bracing, more than number of girts per main column means no fly bracing too
                bUseMainColumnFlyBracingPlates = false;

            float fFirstGirtPosition = fBottomGirtPosition;
            float fFirstPurlinPosition = fDist_Purlin;
            float fRafterLength = MathF.Sqrt(MathF.Pow2(fH2_frame - fH1_frame) + MathF.Pow2(0.5f * fW_frame));

            int iOneRafterPurlinNo = 0;
            iPurlinNoInOneFrame = 0;

            bool bGeneratePurlins = componentList[(int)EMemberGroupNames.ePurlin].Generate.Value;
            if (bGeneratePurlins)
            {
                iOneRafterPurlinNo = (int)((fRafterLength - fFirstPurlinPosition) / fDist_Purlin) + 1;
                iPurlinNoInOneFrame = 2 * iOneRafterPurlinNo;
            }
            componentListVM.SetRafterFlyBracingPosition_Items(iOneRafterPurlinNo);

            if (!bGeneratePurlins || iRafterFlyBracing_EveryXXPurlin == 0 || iRafterFlyBracing_EveryXXPurlin > iPurlinNoInOneFrame) // Index 0 means do not use fly bracing, more than number of purlins per rafter means no fly bracing too
                bUseRafterFlyBracingPlates = false;

            
            iFrontColumnNoInOneFrame = 0;

            bool bGenerateFrontColumns = componentList[(int)EMemberGroupNames.eFrontColumn].Generate.Value;
            if (bGenerateFrontColumns)
            {
                iOneRafterFrontColumnNo = (int)((0.5f * fW_frame) / fDist_FrontColumns);
                iFrontColumnNoInOneFrame = 2 * iOneRafterFrontColumnNo;
                // Update value of distance between columns
                fDist_FrontColumns = (fW_frame / (iFrontColumnNoInOneFrame + 1));
            }

            const int iFrontColumnNodesNo = 2; // Number of Nodes for Front Column
            int iFrontColumninOneRafterNodesNo = iFrontColumnNodesNo * iOneRafterFrontColumnNo; // Number of Nodes for Front Columns under one Rafter
            int iFrontColumninOneFrameNodesNo = 2 * iFrontColumninOneRafterNodesNo; // Number of Nodes for Front Columns under one Frame

            
            iBackColumnNoInOneFrame = 0;

            fDist_BackColumns = fDist_FrontColumns; // Todo Temporary - umoznit ine roztece medzi zadnymi a prednymi stlpmi

            bool bGenerateBackColumns = componentList[(int)EMemberGroupNames.eBackColumn].Generate.Value;
            if (bGenerateBackColumns)
            {
                iOneRafterBackColumnNo = (int)((0.5f * fW_frame) / fDist_BackColumns);
                iBackColumnNoInOneFrame = 2 * iOneRafterBackColumnNo;
                // Update value of distance between columns
                fDist_BackColumns = (fW_frame / (iBackColumnNoInOneFrame + 1));
            }

            const int iBackColumnNodesNo = 2; // Number of Nodes for Back Column
            int iBackColumninOneRafterNodesNo = iBackColumnNodesNo * iOneRafterBackColumnNo; // Number of Nodes for Back Columns under one Rafter
            int iBackColumninOneFrameNodesNo = 2 * iBackColumninOneRafterNodesNo; // Number of Nodes for Back Columns under one Frame

            // Number of Nodes - Front Girts
            int iFrontIntermediateColumnNodesForGirtsOneRafterNo = 0;
            int iFrontIntermediateColumnNodesForGirtsOneFrameNo = 0;
            iFrontGirtsNoInOneFrame = 0;
            iArrNumberOfNodesPerFrontColumn = new int[iOneRafterFrontColumnNo];

            bool bGenerateFrontGirts = componentList[(int)EMemberGroupNames.eFrontGirt].Generate.Value;

            if (bGenerateFrontGirts)
            {
                iFrontIntermediateColumnNodesForGirtsOneRafterNo = GetNumberofIntermediateNodesInColumnsForOneFrame(iOneRafterFrontColumnNo, fBottomGirtPosition, fDist_FrontColumns, fz_UpperLimitForFrontGirts);
                iFrontIntermediateColumnNodesForGirtsOneFrameNo = 2 * iFrontIntermediateColumnNodesForGirtsOneRafterNo;

                // Number of Girts - Main Frame Column
                iOneColumnGirtNo = (int)((fH1_frame - fUpperGirtLimit - fBottomGirtPosition) / fDist_Girt) + 1;

                iFrontGirtsNoInOneFrame = iOneColumnGirtNo;

                // Number of girts under one rafter at the frontside of building - middle girts are considered twice
                for (int i = 0; i < iOneRafterFrontColumnNo; i++)
                {
                    int temp = GetNumberofIntermediateNodesInOneColumnForGirts(fBottomGirtPosition, fDist_FrontColumns, fz_UpperLimitForFrontGirts, i);
                    iFrontGirtsNoInOneFrame += temp;
                    iArrNumberOfNodesPerFrontColumn[i] = temp;
                }

                iFrontGirtsNoInOneFrame *= 2;
                // Girts in the middle are considered twice - remove one set
                iFrontGirtsNoInOneFrame -= iArrNumberOfNodesPerFrontColumn[iOneRafterFrontColumnNo - 1];
            }
            componentListVM.SetFrontColumnFlyBracingPosition_Items(iOneColumnGirtNo);

            if (!bGenerateFrontGirts || iFrontColumnFlyBracing_EveryXXGirt == 0) // Index 0 means do not use fly bracing
                bUseFrontColumnFlyBracingPlates = false;

            // Number of Nodes - Back Girts
            int iBackIntermediateColumnNodesForGirtsOneRafterNo = 0;
            int iBackIntermediateColumnNodesForGirtsOneFrameNo = 0;
            iBackGirtsNoInOneFrame = 0;
            iArrNumberOfNodesPerBackColumn = new int[iOneRafterBackColumnNo];

            bool bGenerateBackGirts = componentList[(int)EMemberGroupNames.eBackGirt].Generate.Value;

            if (bGenerateBackGirts)
            {
                iBackIntermediateColumnNodesForGirtsOneRafterNo = GetNumberofIntermediateNodesInColumnsForOneFrame(iOneRafterBackColumnNo, fBottomGirtPosition, fDist_BackColumns, fz_UpperLimitForBackGirts);
                iBackIntermediateColumnNodesForGirtsOneFrameNo = 2 * iBackIntermediateColumnNodesForGirtsOneRafterNo;

                // Number of Girts - Main Frame Column
                iOneColumnGirtNo = (int)((fH1_frame - fUpperGirtLimit - fBottomGirtPosition) / fDist_Girt) + 1;

                iBackGirtsNoInOneFrame = iOneColumnGirtNo;

                // Number of girts under one rafter at the frontside of building - middle girts are considered twice
                for (int i = 0; i < iOneRafterBackColumnNo; i++)
                {
                    int temp = GetNumberofIntermediateNodesInOneColumnForGirts(fBottomGirtPosition, fDist_BackColumns, fz_UpperLimitForBackGirts, i);
                    iBackGirtsNoInOneFrame += temp;
                    iArrNumberOfNodesPerBackColumn[i] = temp;
                }

                iBackGirtsNoInOneFrame *= 2;
                // Girts in the middle are considered twice - remove one set
                iBackGirtsNoInOneFrame -= iArrNumberOfNodesPerBackColumn[iOneRafterBackColumnNo - 1];
            }
            componentListVM.SetBackColumnFlyBracingPosition_Items(iOneColumnGirtNo);

            if (!bGenerateBackGirts || iBackColumnFlyBracing_EveryXXGirt == 0) // Index 0 means do not use fly bracing
                bUseBackColumnFlyBracingPlates = false;

            m_arrNodes = new CNode[iFrameNodesNo * iFrameNo + iFrameNo * iGirtNoInOneFrame + iFrameNo * iPurlinNoInOneFrame + iFrontColumninOneFrameNodesNo + iBackColumninOneFrameNodesNo + iFrontIntermediateColumnNodesForGirtsOneFrameNo + iBackIntermediateColumnNodesForGirtsOneFrameNo];
            m_arrMembers = new CMember[iMainColumnNo + iRafterNo + iEavesPurlinNo + (iFrameNo - 1) * iGirtNoInOneFrame + (iFrameNo - 1) * iPurlinNoInOneFrame + iFrontColumnNoInOneFrame + iBackColumnNoInOneFrame + iFrontGirtsNoInOneFrame + iBackGirtsNoInOneFrame];

            float fCutOffOneSide = 0.005f; // Cut 5 mm from each side of member
            // Alignments

            float fallignment_column, fallignment_knee_rafter, fallignment_apex_rafter;

            GetJointAllignments((float)m_arrCrSc[(int)EMemberGroupNames.eMainColumn].h, (float)m_arrCrSc[(int)EMemberGroupNames.eRafter].h, out fallignment_column, out fallignment_knee_rafter, out fallignment_apex_rafter);

            float fMainColumnStart = 0.0f;
            float fMainColumnEnd = -fallignment_column - fCutOffOneSide;
            float fRafterStart = fallignment_knee_rafter - fCutOffOneSide;
            float fRafterEnd = -fallignment_apex_rafter - fCutOffOneSide;                                                // Calculate according to h of rafter and roof pitch
            float fEavesPurlinStart = -(float)m_arrCrSc[(int)EMemberGroupNames.eRafter].y_max - fCutOffOneSide;
            float fEavesPurlinEnd = (float)m_arrCrSc[(int)EMemberGroupNames.eRafter].y_min - fCutOffOneSide;
            float fGirtStart = -(float)m_arrCrSc[(int)EMemberGroupNames.eMainColumn].y_max - fCutOffOneSide;
            float fGirtEnd = (float)m_arrCrSc[(int)EMemberGroupNames.eMainColumn].y_min - fCutOffOneSide;
            float fPurlinStart = -(float)m_arrCrSc[(int)EMemberGroupNames.eRafter].y_max - fCutOffOneSide;
            float fPurlinEnd = (float)m_arrCrSc[(int)EMemberGroupNames.eRafter].y_min - fCutOffOneSide;
            float fFrontColumnStart = 0.0f;
            float fFrontColumnEnd = 0.08f * (float)m_arrCrSc[(int)EMemberGroupNames.eRafter].h - fCutOffOneSide;         // TODO - Calculate according to h of rafter and roof pitch
            float fBackColumnStart = 0.0f;
            float fBackColumnEnd = 0.08f * (float)m_arrCrSc[(int)EMemberGroupNames.eRafter].h - fCutOffOneSide;          // TODO - Calculate according to h of rafter and roof pitch
            float fFrontGirtStart = -0.5f * (float)m_arrCrSc[(int)EMemberGroupNames.eFrontColumn].b - fCutOffOneSide;    // Just in case that cross-section of column is symmetric about z-z
            float fFrontGirtEnd = -0.5f * (float)m_arrCrSc[(int)EMemberGroupNames.eFrontColumn].b - fCutOffOneSide;      // Just in case that cross-section of column is symmetric about z-z
            float fBackGirtStart = -0.5f * (float)m_arrCrSc[(int)EMemberGroupNames.eBackColumn].b - fCutOffOneSide;      // Just in case that cross-section of column is symmetric about z-z
            float fBackGirtEnd = -0.5f * (float)m_arrCrSc[(int)EMemberGroupNames.eBackColumn].b - fCutOffOneSide;        // Just in case that cross-section of column is symmetric about z-z
            float fFrontGirtStart_MC = -0.5f * (float)m_arrCrSc[(int)EMemberGroupNames.eMainColumn].h - fCutOffOneSide;  // Connection to the main frame column (column symmetrical about y-y)
            float fFrontGirtEnd_MC = -0.5f * (float)m_arrCrSc[(int)EMemberGroupNames.eMainColumn].h - fCutOffOneSide;    // Connection to the main frame column (column symmetrical about y-y)
            float fBackGirtStart_MC = -0.5f * (float)m_arrCrSc[(int)EMemberGroupNames.eMainColumn].h - fCutOffOneSide;   // Connection to the main frame column (column symmetrical about y-y)
            float fBackGirtEnd_MC = -0.5f * (float)m_arrCrSc[(int)EMemberGroupNames.eMainColumn].h - fCutOffOneSide;     // Connection to the main frame column (column symmetrical about y-y)

            float fColumnsRotation = MathF.fPI / 2.0f;
            float fGirtsRotation = MathF.fPI / 2.0f;

            // Nodes Automatic Generation
            // Nodes List - Nodes Array

            // Nodes - Frames
            for (int i = 0; i < iFrameNo; i++)
            {
                m_arrNodes[i * iFrameNodesNo + 0] = new CNode(i * iFrameNodesNo + 1, 000000, i * fL1_frame, 00000, 0);
                m_arrNodes[i * iFrameNodesNo + 0].Name = "Main Column Base Node - left";
                RotateFrontOrBackFrameNodeAboutZ(m_arrNodes[i * iFrameNodesNo + 0]);

                m_arrNodes[i * iFrameNodesNo + 1] = new CNode(i * iFrameNodesNo + 2, 000000, i * fL1_frame, fH1_frame, 0);
                m_arrNodes[i * iFrameNodesNo + 1].Name = "Main Column Top Node - left";
                RotateFrontOrBackFrameNodeAboutZ(m_arrNodes[i * iFrameNodesNo + 1]);

                m_arrNodes[i * iFrameNodesNo + 2] = new CNode(i * iFrameNodesNo + 3, 0.5f * fW_frame, i * fL1_frame, fH2_frame, 0);
                m_arrNodes[i * iFrameNodesNo + 2].Name = "Apex Node";
                RotateFrontOrBackFrameNodeAboutZ(m_arrNodes[i * iFrameNodesNo + 2]);

                m_arrNodes[i * iFrameNodesNo + 3] = new CNode(i * iFrameNodesNo + 4, fW_frame, i * fL1_frame, fH1_frame, 0);
                m_arrNodes[i * iFrameNodesNo + 3].Name = "Main Column Top Node - right";
                RotateFrontOrBackFrameNodeAboutZ(m_arrNodes[i * iFrameNodesNo + 3]);

                m_arrNodes[i * iFrameNodesNo + 4] = new CNode(i * iFrameNodesNo + 5, fW_frame, i * fL1_frame, 00000, 0);
                m_arrNodes[i * iFrameNodesNo + 4].Name = "Main Column Base Node - right";
                RotateFrontOrBackFrameNodeAboutZ(m_arrNodes[i * iFrameNodesNo + 4]);
            }

            // Members
            for (int i = 0; i < iFrameNo; i++)
            {
                int iCrscColumnIndex = (int)EMemberGroupNames.eMainColumn;
                int iCrscRafterIndex = (int)EMemberGroupNames.eRafter;
                EMemberType_FS eColumnType = EMemberType_FS.eMC;
                EMemberType_FS eRafterType = EMemberType_FS.eMR;

                if (i == 0 || i == (iFrameNo - 1))
                {
                    iCrscColumnIndex = (int)EMemberGroupNames.eMainColumn_EF;
                    iCrscRafterIndex = (int)EMemberGroupNames.eRafter_EF;
                    eColumnType = EMemberType_FS.eEC;
                    eRafterType = EMemberType_FS.eER;
                }

                // Main Column
                m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * (iFrameNodesNo - 1) + 0] = new CMember((i * iEavesPurlinNoInOneFrame) + i * (iFrameNodesNo - 1) + 1, m_arrNodes[i * iFrameNodesNo + 0], m_arrNodes[i * iFrameNodesNo + 1], m_arrCrSc[iCrscColumnIndex], eColumnType, null, null, fMainColumnStart, fMainColumnEnd, 0f, 0);
                CreateAndAssignIrregularTransverseSupportGroupAndLTBsegmentGroup(bUseMainColumnFlyBracingPlates, iMainColumnFlyBracing_EveryXXGirt, fBottomGirtPosition, fDist_Girt, ref m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * (iFrameNodesNo - 1) + 0]);

                // Rafters
                m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * (iFrameNodesNo - 1) + 1] = new CMember((i * iEavesPurlinNoInOneFrame) + i * (iFrameNodesNo - 1) + 2, m_arrNodes[i * iFrameNodesNo + 1], m_arrNodes[i * iFrameNodesNo + 2], m_arrCrSc[iCrscRafterIndex], eRafterType, null, null, fRafterStart, fRafterEnd, 0f, 0);
                CreateAndAssignIrregularTransverseSupportGroupAndLTBsegmentGroup(bUseRafterFlyBracingPlates, iRafterFlyBracing_EveryXXPurlin, fFirstPurlinPosition, fDist_Purlin, ref m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * (iFrameNodesNo - 1) + 1]);

                m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * (iFrameNodesNo - 1) + 2] = new CMember((i * iEavesPurlinNoInOneFrame) + i * (iFrameNodesNo - 1) + 3, m_arrNodes[i * iFrameNodesNo + 2], m_arrNodes[i * iFrameNodesNo + 3], m_arrCrSc[iCrscRafterIndex], eRafterType, null, null, fRafterEnd, fRafterStart, 0f, 0);
                // Reversed sequence of ILS
                CreateAndAssignReversedIrregularTransverseSupportGroupAndLTBsegmentGroup(bUseRafterFlyBracingPlates, iRafterFlyBracing_EveryXXPurlin, fFirstPurlinPosition, fDist_Purlin, ref m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * (iFrameNodesNo - 1) + 2]);

                // Main Column
                m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * (iFrameNodesNo - 1) + 3] = new CMember((i * iEavesPurlinNoInOneFrame) + i * (iFrameNodesNo - 1) + 4, m_arrNodes[i * iFrameNodesNo + 3], m_arrNodes[i * iFrameNodesNo + 4], m_arrCrSc[iCrscColumnIndex], eColumnType, null, null, fMainColumnEnd, fMainColumnStart, 0f, 0);
                // Reversed sequence of ILS
                CreateAndAssignReversedIrregularTransverseSupportGroupAndLTBsegmentGroup(bUseMainColumnFlyBracingPlates, iMainColumnFlyBracing_EveryXXGirt, fBottomGirtPosition, fDist_Girt, ref m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * (iFrameNodesNo - 1) + 3]);

                // Eaves Purlins
                if (i < (iFrameNo - 1))
                {
                    m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * (iFrameNodesNo - 1) + 4] = new CMember((i * iEavesPurlinNoInOneFrame) + i * (iFrameNodesNo - 1) + 5, m_arrNodes[i * iFrameNodesNo + 1], m_arrNodes[(i + 1) * iFrameNodesNo + 1], m_arrCrSc[(int)EMemberGroupNames.eEavesPurlin], EMemberType_FS.eEP, eccentricityEavePurlin, eccentricityEavePurlin, fEavesPurlinStart, fEavesPurlinEnd, (float)Math.PI, 0);
                    m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * (iFrameNodesNo - 1) + 5] = new CMember((i * iEavesPurlinNoInOneFrame) + i * (iFrameNodesNo - 1) + 6, m_arrNodes[i * iFrameNodesNo + 3], m_arrNodes[(i + 1) * iFrameNodesNo + 3], m_arrCrSc[(int)EMemberGroupNames.eEavesPurlin], EMemberType_FS.eEP, eccentricityEavePurlin, eccentricityEavePurlin, fEavesPurlinStart, fEavesPurlinEnd, 0f, 0);
                    CreateAndAssignRegularTransverseSupportGroupAndLTBsegmentGroup(m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * (iFrameNodesNo - 1) + 4], iNumberOfTransverseSupports_EdgePurlins);
                    CreateAndAssignRegularTransverseSupportGroupAndLTBsegmentGroup(m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * (iFrameNodesNo - 1) + 5], iNumberOfTransverseSupports_EdgePurlins);
                }
            }

            // Nodes - Girts
            int i_temp_numberofNodes = iFrameNodesNo * iFrameNo;
            if (bGenerateGirts)
            {
                for (int i = 0; i < iFrameNo; i++)
                {
                    for (int j = 0; j < iOneColumnGirtNo; j++)
                    {
                        m_arrNodes[i_temp_numberofNodes + i * iGirtNoInOneFrame + j] = new CNode(i_temp_numberofNodes + i * iGirtNoInOneFrame + j + 1, 000000, i * fL1_frame, fBottomGirtPosition + j * fDist_Girt, 0);
                        RotateFrontOrBackFrameNodeAboutZ(m_arrNodes[i_temp_numberofNodes + i * iGirtNoInOneFrame + j]);
                    }

                    for (int j = 0; j < iOneColumnGirtNo; j++)
                    {
                        m_arrNodes[i_temp_numberofNodes + i * iGirtNoInOneFrame + iOneColumnGirtNo + j] = new CNode(i_temp_numberofNodes + i * iGirtNoInOneFrame + iOneColumnGirtNo + j + 1, fW_frame, i * fL1_frame, fBottomGirtPosition + j * fDist_Girt, 0);
                        RotateFrontOrBackFrameNodeAboutZ(m_arrNodes[i_temp_numberofNodes + i * iGirtNoInOneFrame + iOneColumnGirtNo + j]);
                    }
                }
            }

            // Members - Girts
            int i_temp_numberofMembers = iMainColumnNo + iRafterNo + iEavesPurlinNoInOneFrame * (iFrameNo - 1);
            if (bGenerateGirts)
            {
                for (int i = 0; i < (iFrameNo - 1); i++)
                {
                    for (int j = 0; j < iOneColumnGirtNo; j++)
                    {
                        m_arrMembers[i_temp_numberofMembers + i * iGirtNoInOneFrame + j] = new CMember(i_temp_numberofMembers + i * iGirtNoInOneFrame + j + 1, m_arrNodes[i_temp_numberofNodes + i * iGirtNoInOneFrame + j], m_arrNodes[i_temp_numberofNodes + (i + 1) * iGirtNoInOneFrame + j], m_arrCrSc[(int)EMemberGroupNames.eGirtWall], EMemberType_FS.eG, eccentricityGirtLeft_X0, eccentricityGirtLeft_X0, fGirtStart, fGirtEnd, fGirtsRotation, 0);
                        RotateFrontOrBackFrameNodeAboutZ(m_arrNodes[i_temp_numberofMembers + i * iGirtNoInOneFrame + j]);
                        CreateAndAssignRegularTransverseSupportGroupAndLTBsegmentGroup(m_arrMembers[i_temp_numberofMembers + i * iGirtNoInOneFrame + j], iNumberOfTransverseSupports_Girts);
                    }

                    for (int j = 0; j < iOneColumnGirtNo; j++)
                    {
                        m_arrMembers[i_temp_numberofMembers + i * iGirtNoInOneFrame + iOneColumnGirtNo + j] = new CMember(i_temp_numberofMembers + i * iGirtNoInOneFrame + iOneColumnGirtNo + j + 1, m_arrNodes[i_temp_numberofNodes + i * iGirtNoInOneFrame + iOneColumnGirtNo + j], m_arrNodes[i_temp_numberofNodes + (i + 1) * iGirtNoInOneFrame + iOneColumnGirtNo + j], m_arrCrSc[(int)EMemberGroupNames.eGirtWall], EMemberType_FS.eG, eccentricityGirtRight_XB, eccentricityGirtRight_XB, fGirtStart, fGirtEnd, fGirtsRotation, 0);
                        RotateFrontOrBackFrameNodeAboutZ(m_arrNodes[i_temp_numberofMembers + i * iGirtNoInOneFrame + iOneColumnGirtNo + j]);
                        CreateAndAssignRegularTransverseSupportGroupAndLTBsegmentGroup(m_arrMembers[i_temp_numberofMembers + i * iGirtNoInOneFrame + iOneColumnGirtNo + j], iNumberOfTransverseSupports_Girts);
                    }
                }
            }

            // Nodes - Purlins
            i_temp_numberofNodes += bGenerateGirts ? (iGirtNoInOneFrame * iFrameNo) : 0;
            if (bGeneratePurlins)
            {
                for (int i = 0; i < iFrameNo; i++)
                {
                    for (int j = 0; j < iOneRafterPurlinNo; j++)
                    {
                        float x_glob, z_glob;
                        CalcPurlinNodeCoord(fFirstPurlinPosition + j * fDist_Purlin, out x_glob, out z_glob);

                        m_arrNodes[i_temp_numberofNodes + i * iPurlinNoInOneFrame + j] = new CNode(i_temp_numberofNodes + i * iPurlinNoInOneFrame + j + 1, x_glob, i * fL1_frame, z_glob, 0);
                        RotateFrontOrBackFrameNodeAboutZ(m_arrNodes[i_temp_numberofNodes + i * iPurlinNoInOneFrame + j]);
                    }

                    for (int j = 0; j < iOneRafterPurlinNo; j++)
                    {
                        float x_glob, z_glob;
                        CalcPurlinNodeCoord(fFirstPurlinPosition + j * fDist_Purlin, out x_glob, out z_glob);

                        m_arrNodes[i_temp_numberofNodes + i * iPurlinNoInOneFrame + iOneRafterPurlinNo + j] = new CNode(i_temp_numberofNodes + i * iPurlinNoInOneFrame + iOneRafterPurlinNo + j + 1, fW_frame - x_glob, i * fL1_frame, z_glob, 0);
                        RotateFrontOrBackFrameNodeAboutZ(m_arrNodes[i_temp_numberofNodes + i * iPurlinNoInOneFrame + iOneRafterPurlinNo + j]);
                    }
                }
            }

            // Members - Purlins
            i_temp_numberofMembers += bGenerateGirts ? (iGirtNoInOneFrame * (iFrameNo - 1)) : 0;
            if (bGeneratePurlins)
            {
                for (int i = 0; i < (iFrameNo - 1); i++)
                {
                    for (int j = 0; j < iOneRafterPurlinNo; j++)
                    {
                        CMemberEccentricity temp = new CMemberEccentricity();
                        float fRotationAngle;

                        bool bOrientationOfLocalZAxisIsUpward = true;

                        if (bOrientationOfLocalZAxisIsUpward)
                        {
                            fRotationAngle = -fRoofPitch_rad;
                            temp.MFz_local = eccentricityPurlin.MFz_local;
                        }
                        else
                        {
                            fRotationAngle = -(fRoofPitch_rad + (float)Math.PI);
                            temp.MFz_local = -eccentricityPurlin.MFz_local; // We need to change sign of eccentrictiy for purlins on the left side because z axis of these purlins is oriented downwards
                        }

                        m_arrMembers[i_temp_numberofMembers + i * iPurlinNoInOneFrame + j] = new CMember(i_temp_numberofMembers + i * iPurlinNoInOneFrame + j + 1, m_arrNodes[i_temp_numberofNodes + i * iPurlinNoInOneFrame + j], m_arrNodes[i_temp_numberofNodes + (i + 1) * iPurlinNoInOneFrame + j], m_arrCrSc[(int)EMemberGroupNames.ePurlin], EMemberType_FS.eP, temp/*eccentricityPurlin*/, temp /*eccentricityPurlin*/, fPurlinStart, fPurlinEnd, fRotationAngle, 0);
                        CreateAndAssignRegularTransverseSupportGroupAndLTBsegmentGroup(m_arrMembers[i_temp_numberofMembers + i * iPurlinNoInOneFrame + j], iNumberOfTransverseSupports_Purlins);
                    }

                    for (int j = 0; j < iOneRafterPurlinNo; j++)
                    {

                        m_arrMembers[i_temp_numberofMembers + i * iPurlinNoInOneFrame + iOneRafterPurlinNo + j] = new CMember(i_temp_numberofMembers + i * iPurlinNoInOneFrame + iOneRafterPurlinNo + j + 1, m_arrNodes[i_temp_numberofNodes + i * iPurlinNoInOneFrame + iOneRafterPurlinNo + j], m_arrNodes[i_temp_numberofNodes + (i + 1) * iPurlinNoInOneFrame + iOneRafterPurlinNo + j], m_arrCrSc[(int)EMemberGroupNames.ePurlin], EMemberType_FS.eP, eccentricityPurlin, eccentricityPurlin, fPurlinStart, fPurlinEnd, fRoofPitch_rad, 0);
                        CreateAndAssignRegularTransverseSupportGroupAndLTBsegmentGroup(m_arrMembers[i_temp_numberofMembers + i * iPurlinNoInOneFrame + iOneRafterPurlinNo + j], iNumberOfTransverseSupports_Purlins);
                    }
                }
            }

            // Front Columns
            // Nodes - Front Columns
            i_temp_numberofNodes += bGeneratePurlins ? (iPurlinNoInOneFrame * iFrameNo) : 0;
            if (bGenerateFrontColumns)
            {
                AddColumnsNodes(i_temp_numberofNodes, i_temp_numberofMembers, iOneRafterFrontColumnNo, iFrontColumnNoInOneFrame, fDist_FrontColumns, 0);
            }

            // Members - Front Columns
            i_temp_numberofMembers += bGeneratePurlins ? (iPurlinNoInOneFrame * (iFrameNo - 1)) : 0;
            if (bGenerateFrontColumns)
            {
                AddColumnsMembers(i_temp_numberofNodes, i_temp_numberofMembers, iOneRafterFrontColumnNo, iFrontColumnNoInOneFrame, eccentricityColumnFront_Z, fFrontColumnStart, fFrontColumnEnd, m_arrCrSc[(int)EMemberGroupNames.eFrontColumn], fColumnsRotation, bUseFrontColumnFlyBracingPlates, iFrontColumnFlyBracing_EveryXXGirt, fBottomGirtPosition, fDist_FrontGirts);
            }

            // Back Columns
            // Nodes - Back Columns
            i_temp_numberofNodes += bGenerateFrontColumns ? iFrontColumninOneFrameNodesNo : 0;

            if (bGenerateBackColumns)
            {
                AddColumnsNodes(i_temp_numberofNodes, i_temp_numberofMembers, iOneRafterBackColumnNo, iBackColumnNoInOneFrame, fDist_BackColumns, fL_tot);
            }

            // Members - Back Columns
            i_temp_numberofMembers += bGenerateFrontColumns ? iFrontColumnNoInOneFrame : 0;
            if (bGenerateBackColumns)
            {
                AddColumnsMembers(i_temp_numberofNodes, i_temp_numberofMembers, iOneRafterBackColumnNo, iBackColumnNoInOneFrame, eccentricityColumnBack_Z, fBackColumnStart, fBackColumnEnd, m_arrCrSc[(int)EMemberGroupNames.eBackColumn], fColumnsRotation, bUseBackColumnFlyBracingPlates, iBackColumnFlyBracing_EveryXXGirt, fBottomGirtPosition, fDist_BackGirts);
            }

            // Front Girts
            // Nodes - Front Girts
            i_temp_numberofNodes += bGenerateBackColumns ? iBackColumninOneFrameNodesNo : 0;
            if (bGenerateFrontGirts)
            {
                AddFrontOrBackGirtsNodes(iOneRafterFrontColumnNo, iArrNumberOfNodesPerFrontColumn, i_temp_numberofNodes, iFrontIntermediateColumnNodesForGirtsOneRafterNo, fDist_FrontGirts, fDist_FrontColumns, 0);
            }

            // Front Girts
            // Members - Front Girts
            // TODO - doplnit riesenie pre maly rozpon ked neexistuju mezilahle stlpiky, prepojenie mezi hlavnymi stplmi ramu na celu sirku budovy
            // TODO - toto riesenie plati len ak existuju girts v pozdlznom smere, ak budu deaktivovane a nevytvoria sa uzly na stlpoch tak sa musia pruty na celnych stenach generovat uplne inak, musia sa vygenerovat aj uzly na stlpoch ....
            // TODO - pri vacsom sklone strechy (cca > 35 stupnov) by bolo dobre dogenerovat prvky ktore nie su na oboch stranach pripojene k stlpom ale su na jeden strane pripojene na stlp a na druhej strane na rafter, inak vznikaju prilis velke prazdne oblasti bez podpory (trojuhoniky) pod hlavnym ramom

            i_temp_numberofMembers += bGenerateBackColumns ? iBackColumnNoInOneFrame : 0;
            if (bGenerateFrontGirts)
            {
                AddFrontOrBackGirtsMembers(iFrameNodesNo, iOneRafterFrontColumnNo, iArrNumberOfNodesPerFrontColumn, i_temp_numberofNodes, i_temp_numberofMembers, iFrontIntermediateColumnNodesForGirtsOneRafterNo, iFrontIntermediateColumnNodesForGirtsOneFrameNo, 0, fDist_Girt, eccentricityGirtFront_Y0, fFrontGirtStart_MC, fFrontGirtStart, fFrontGirtEnd, m_arrCrSc[(int)EMemberGroupNames.eFrontGirt], fColumnsRotation, iNumberOfTransverseSupports_FrontGirts);
            }

            // Back Girts
            // Nodes - Back Girts

            i_temp_numberofNodes += bGenerateFrontGirts ? iFrontIntermediateColumnNodesForGirtsOneFrameNo : 0;
            if (bGenerateBackGirts)
            {
                AddFrontOrBackGirtsNodes(iOneRafterBackColumnNo, iArrNumberOfNodesPerBackColumn, i_temp_numberofNodes, iBackIntermediateColumnNodesForGirtsOneRafterNo, fDist_BackGirts, fDist_BackColumns, fL_tot);
            }

            // Back Girts
            // Members - Back Girts

            i_temp_numberofMembers += bGenerateFrontGirts ? iFrontGirtsNoInOneFrame : 0;
            if (bGenerateBackGirts)
            {
                AddFrontOrBackGirtsMembers(iFrameNodesNo, iOneRafterBackColumnNo, iArrNumberOfNodesPerBackColumn, i_temp_numberofNodes, i_temp_numberofMembers, iBackIntermediateColumnNodesForGirtsOneRafterNo, iBackIntermediateColumnNodesForGirtsOneFrameNo, iGirtNoInOneFrame * (iFrameNo - 1), fDist_Girt, eccentricityGirtBack_YL, fBackGirtStart_MC, fBackGirtStart, fBackGirtEnd, m_arrCrSc[(int)EMemberGroupNames.eBackGirt], fColumnsRotation, iNumberOfTransverseSupports_BackGirts);
            }


            #region Joints
            if (joints == null)
            {
                CreateJoints(bGenerateGirts, bUseMainColumnFlyBracingPlates, bGeneratePurlins, bUseRafterFlyBracingPlates, bGenerateFrontColumns, bGenerateBackColumns, bGenerateFrontGirts, bGenerateBackGirts);
            }
            else m_arrConnectionJoints = joints;
            #endregion




            #region Blocks

            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // Blocks
            // TODO - pokusny blok dveri, je potreba refaktorovat
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            DoorsModels = new List<CBlock_3D_001_DoorInBay>();
            WindowsModels = new List<CBlock_3D_002_WindowInBay>();

            if (doorBlocksProperties != null)
            {
                for (int i = 0; i < doorBlocksProperties.Count; i++)
                {
                    if (!bGenerateGirts && (doorBlocksProperties[i].sBuildingSide == "Right" || doorBlocksProperties[i].sBuildingSide == "Left")) continue;
                    else if (!bGenerateFrontGirts && doorBlocksProperties[i].sBuildingSide == "Front") continue;
                    else if (!bGenerateBackGirts && doorBlocksProperties[i].sBuildingSide == "Back") continue;

                    if (doorBlocksProperties[i].Validate()) AddDoorBlock(doorBlocksProperties[i], 0.5f, fH1_frame);
                }
            }

            if (windowBlocksProperties != null)
            {
                for (int i = 0; i < windowBlocksProperties.Count; i++)
                {
                    if (!bGenerateGirts && (windowBlocksProperties[i].sBuildingSide == "Right" || windowBlocksProperties[i].sBuildingSide == "Left")) continue;
                    else if (!bGenerateFrontGirts && windowBlocksProperties[i].sBuildingSide == "Front") continue;
                    else if (!bGenerateBackGirts && windowBlocksProperties[i].sBuildingSide == "Back") continue;

                    if (windowBlocksProperties[i].Validate()) AddWindowBlock(windowBlocksProperties[i], 0.5f);
                }
            }

            // Validation - check that all created joints have assigned Main Member
            // Check all joints after definition of doors and windows members and joints
            for (int i = 0; i < m_arrConnectionJoints.Count; i++)
            {
                if (m_arrConnectionJoints[i].m_MainMember == null)
                {
                    //throw new ArgumentNullException("Main member is not assigned to the joint No.:" + m_arrConnectionJoints[i].ID.ToString() + " Joint index in the list: " + i);

                    // TODO BUG 46 - TO Ondrej // Odstranenie spojov ktore patria k deaktivovanym prutom (pruty boli deaktivovane, pretoze sa nachadazju na miest vlozeneho bloku)
                    // Odstranenie by malo nastavat uz vo funckii ktora generuje bloky okien a dveri

                    // Toto je docasne riesenie - vymazeme spoj zo zoznamu
                    m_arrConnectionJoints.RemoveAt(i); // Remove joint from the list
                }
            }

            // Opakovana kontrola po odstraneni spojov s MainMember = null
            for (int i = 0; i < m_arrConnectionJoints.Count; i++)
            {
                if (m_arrConnectionJoints[i].m_MainMember == null)
                    throw new ArgumentNullException("Main member is not assigned to the joint No.:" + m_arrConnectionJoints[i].ID.ToString() + " Joint index in the list: " + i);
            }

            // Validation - duplicity of node ID
            for (int i = 0; i < m_arrNodes.Length; i++)
            {
                for (int j = 0; j < m_arrNodes.Length; j++)
                {
                    if ((m_arrNodes[i] != m_arrNodes[j]) && (m_arrNodes[i].ID == m_arrNodes[j].ID))
                        throw new ArgumentNullException("Duplicity in Node ID.\nNode index: " + i + " and Node index: " + j);
                }
            }

            //------------------------------------------------------------
            // Vid TODO 234 - docasne priradenie vlastnosti materialu
            // Pre objekty dveri je potrebne pridat prierezy do Component List - Tab Members a nacitat ich parametre, potom sa moze nacitanie z databazy zmazat
            // Po zapracovani TODO 234 mozno tento kod zmazat
            foreach (CMember member in m_arrMembers)
            {
                if (member.CrScStart.m_Mat is CMat_03_00)
                    DATABASE.CMaterialManager.LoadMaterialProperties((CMat_03_00)member.CrScStart.m_Mat, member.CrScStart.m_Mat.Name);
            }
            //------------------------------------------------------------

            // End of blocks
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            #endregion

            AddMembersToMemberGroupsLists();

            // Set members Generate, Display, Calculate, Design, MaterialList properties
            CModelHelper.SetMembersAccordingTo(m_arrMembers, componentList);


            #region Supports

            //m_arrNSupports = new CNSupport[2 * iFrameNo];

            // Nodal Supports - fill values

            // Set values
            bool[] bSupport1 = { true, true, true, false, false, false };
            bool[] bSupport2 = { true, true, true, false, false, false };

            // Create Support Objects
            /*
            for (int i = 0; i < iFrameNo; i++)
            {
                m_arrNSupports[i * 2 + 0] = new CNSupport(6, i * 2 + 1, m_arrNodes[i * iFrameNodesNo], bSupport1, 0);
                m_arrNSupports[i * 2 + 1] = new CNSupport(6, i * 2 + 2, m_arrNodes[i * iFrameNodesNo + (iFrameNodesNo - 1)], bSupport2, 0);
            }
            */

            m_arrNSupports = new CNSupport[0];
            for (int i = 0; i < m_arrNodes.Length; i++)
            {
                int arraysizeoriginal = m_arrNSupports.Length;

                // Create support at each node with global Z = 0
                if (MathF.d_equal(m_arrNodes[i].Z, 0))
                {
                    // Resize array
                    Array.Resize(ref m_arrNSupports, m_arrNSupports.Length + 1);

                    m_arrNSupports[m_arrNSupports.Length - 1] = new CNSupport(6, i + 1, m_arrNodes[i], bSupport1, 0);
                }
            }

            // Setridit pole podle ID
            Array.Sort(m_arrNSupports, new CCompare_NSupportID());
            #endregion

            #region Member Releases
            // Member Releases / hinges - fill values

            // Set values
            bool?[] bMembRelase1 = { false, false, false, false, true, false };

            // Create Release / Hinge Objects
            //m_arrMembers[02].CnRelease1 = new CNRelease(6, m_arrMembers[02].NodeStart, bMembRelase1, 0);
            #endregion

            #region Foundations
            bool bGenerateFoundations = true;

            if (bGenerateFoundations)
            {
                // Foundations
                // Footings
                m_arrFoundations = new CFoundation[iMainColumnNo + iFrontColumnNoInOneFrame + iBackColumnNoInOneFrame];

                // Main Column - Footings
                // TODO - Predbezne doporucene hodnoty velkosti zakladov vypocitane z rozmerov budovy
                float fMainColumnFooting_aX = (float)Math.Round(MathF.Max(0.7f, Math.Min(fW_frame * 0.08f, fL1_frame * 0.40f)), 1);
                float fMainColumnFooting_bY = (float)Math.Round(MathF.Max(0.6f, Math.Min(fW_frame * 0.07f, fL1_frame * 0.35f)), 1);
                float fMainColumnFooting_h = 0.4f;

                // TODO - zapracovat excentricku poziciu zakladov

                for (int i = 0; i < iFrameNo; i++)
                {
                    // Left
                    CPoint controlPoint_left = new CPoint(i * 2 + 1, m_arrNodes[i * iFrameNodesNo + 0].X - 0.5f * fMainColumnFooting_aX, m_arrNodes[i * iFrameNodesNo + 0].Y - 0.5f * fMainColumnFooting_bY, m_arrNodes[i * iFrameNodesNo + 0].Z - fMainColumnFooting_h, 0);
                    m_arrFoundations[i * 2] = new CFoundation(i * 2 + 1, EFoundationType.ePad, controlPoint_left, fMainColumnFooting_aX, fMainColumnFooting_bY, fMainColumnFooting_h, Colors.Beige, 0.5f, true, 0);
                    // Right
                    CPoint controlPoint_right = new CPoint(i * 2 + 2, m_arrNodes[i * iFrameNodesNo + 4].X - 0.5f * fMainColumnFooting_aX, m_arrNodes[i * iFrameNodesNo + 4].Y - 0.5f * fMainColumnFooting_bY, m_arrNodes[i * iFrameNodesNo + 4].Z - fMainColumnFooting_h, 0);
                    m_arrFoundations[i * 2 + 1] = new CFoundation(i * 2 + 2, EFoundationType.ePad, controlPoint_right, fMainColumnFooting_aX, fMainColumnFooting_bY, fMainColumnFooting_h, Colors.Beige, 0.5f, true, 0);
                }

                int iLastFoundationIndex = iMainColumnNo;

                // Front and Back Wall Columns - Footings
                if (bGenerateFrontColumns)
                {
                    float fFrontColumnFooting_aX = (float)Math.Round(MathF.Max(0.5f, fDist_FrontColumns * 0.40f), 1);
                    float fFrontColumnFooting_bY = (float)Math.Round(MathF.Max(0.5f, fDist_FrontColumns * 0.40f), 1);
                    float fFrontColumnFooting_h = 0.4f;

                    // Search footings control points
                    List<CNode> listOfControlPoints = new List<CNode>();
                    for (int i = 0; i < m_arrMembers.Length; i++)
                    {
                        // Find foundation definition nodes
                        if (MathF.d_equal(m_arrMembers[i].NodeStart.Z, 0) &&
                            m_arrMembers[i].EMemberType == EMemberType_FS.eC &&
                            m_arrMembers[i].CrScStart.Equals(listOfModelMemberGroups[(int)EMemberGroupNames.eFrontColumn].CrossSection))
                            listOfControlPoints.Add(m_arrMembers[i].NodeStart);
                    }

                    for (int i = 0; i < listOfControlPoints.Count; i++)
                    {
                        CPoint controlPoint = new CPoint(iLastFoundationIndex + i + 1, listOfControlPoints[i].X - 0.5f * fFrontColumnFooting_aX, listOfControlPoints[i].Y - 0.5f * fFrontColumnFooting_bY, listOfControlPoints[i].Z - fFrontColumnFooting_h, 0);
                        m_arrFoundations[iLastFoundationIndex + i] = new CFoundation(iLastFoundationIndex + i + 1, EFoundationType.ePad, controlPoint, fFrontColumnFooting_aX, fFrontColumnFooting_bY, fFrontColumnFooting_h, Colors.LightSeaGreen, 0.5f, true, 0);
                    }

                    iLastFoundationIndex += listOfControlPoints.Count;
                }

                if (bGenerateBackColumns)
                {
                    float fBackColumnFooting_aX = (float)Math.Round(MathF.Max(0.5f, fDist_BackColumns * 0.40f), 1);
                    float fBackColumnFooting_bY = (float)Math.Round(MathF.Max(0.5f, fDist_BackColumns * 0.40f), 1);
                    float fBackColumnFooting_h = 0.4f;

                    // Search footings control points
                    List<CNode> listOfControlPoints = new List<CNode>();
                    for (int i = 0; i < m_arrMembers.Length; i++)
                    {
                        // Find foundation definition nodes
                        if (MathF.d_equal(m_arrMembers[i].NodeStart.Z, 0) &&
                            m_arrMembers[i].EMemberType == EMemberType_FS.eC &&
                            m_arrMembers[i].CrScStart.Equals(listOfModelMemberGroups[(int)EMemberGroupNames.eBackColumn].CrossSection))
                            listOfControlPoints.Add(m_arrMembers[i].NodeStart);
                    }

                    for (int i = 0; i < listOfControlPoints.Count; i++)
                    {
                        CPoint controlPoint = new CPoint(iLastFoundationIndex + i + 1, listOfControlPoints[i].X - 0.5f * fBackColumnFooting_aX, listOfControlPoints[i].Y - 0.5f * fBackColumnFooting_bY, listOfControlPoints[i].Z - fBackColumnFooting_h, 0);
                        m_arrFoundations[iLastFoundationIndex + i] = new CFoundation(iLastFoundationIndex + i + 1, EFoundationType.ePad, controlPoint, fBackColumnFooting_aX, fBackColumnFooting_bY, fBackColumnFooting_h, Colors.Coral, 0.5f, true, 0);
                    }

                    iLastFoundationIndex += listOfControlPoints.Count;
                }

                // Validation - skontroluje ci je velkost pola zhodna s poctom vygenerovanych prvkov
                if (m_arrFoundations.Length != iLastFoundationIndex)
                    throw new Exception("Incorrect number of generated foundations");

                // Ground Floor Slab
                float fFloorSlab_AdditionalOffset_X = 0.1f;
                float fFloorSlab_AdditionalOffset_Y = 0.1f;
                float fFloorSlab_aX = fW_frame + (float)m_arrCrSc[0].h + 2 * fFloorSlab_AdditionalOffset_X;
                float fFloorSlab_bY = fL_tot + (float)m_arrCrSc[0].b + 2 * fFloorSlab_AdditionalOffset_Y;
                float fTolerance = 0.001f; // Tolerance - 3D graphics collision
                float fFloorSlab_h = 0.125f;
                float fFloorSlab_eX = -0.5f * (float)m_arrCrSc[0].h - fFloorSlab_AdditionalOffset_X;
                float fFloorSlab_eY = -0.5f * (float)m_arrCrSc[0].b - fFloorSlab_AdditionalOffset_Y;

                CPoint controlPoint_FloorSlab = new CPoint(iLastFoundationIndex + 1, m_arrNodes[0].X + fFloorSlab_eX, m_arrNodes[0].Y + fFloorSlab_eY, m_arrNodes[0].Z - fFloorSlab_h + fTolerance, 0);
                m_arrGOVolumes = new CVolume[1];
                //m_arrGOVolumes[0] = new CVolume(1, EVolumeShapeType.eShape3DPrism_8Edges, controlPoint_FloorSlab, fFloorSlab_aX, fFloorSlab_bY, fFloorSlab_h, Colors.Gray, 0.5f, true, 0);
                m_arrGOVolumes[0] = new CVolume(1, EVolumeShapeType.eShape3DPrism_8Edges, controlPoint_FloorSlab, fFloorSlab_aX, fFloorSlab_bY, fFloorSlab_h, new DiffuseMaterial(new SolidColorBrush(Colors.DarkGray)), true, 0);
            }
            #endregion

            
            SetJointDefaultParameters();
        }




        public override void CalculateLoadValuesAndGenerateLoads(
                CCalcul_1170_1 generalLoad,
                CCalcul_1170_2 wind,
                CCalcul_1170_3 snow,
                CCalcul_1170_5 eq,
                bool bGenerateNodalLoads,
                bool bGenerateLoadsOnGirts,
                bool bGenerateLoadsOnPurlins,
                bool bGenerateLoadsOnColumns,
                bool bGenerateLoadsOnFrameMembers,
                bool bGenerateSurfaceLoads)
        {
            // Loading
            #region Load Cases
            // Load Cases
            CLoadCaseGenerator loadCaseGenerator = new CLoadCaseGenerator();
            m_arrLoadCases = loadCaseGenerator.GenerateLoadCases();
            #endregion

            // Snow load factor - projection on roof
            // Faktor ktory prepocita zatazenie z podorysneho rozmeru premietnute na stresnu rovinu
            fSlopeFactor = ((0.5f * fW_frame) / ((0.5f * fW_frame) / (float)Math.Cos(fRoofPitch_rad))); // Consider projection acc. to Figure 4.1

            #region Surface Loads
            // Surface Loads

            if (bGenerateSurfaceLoads)
            {
                CSurfaceLoadGenerator surfaceLoadGenerator = new CSurfaceLoadGenerator(fH1_frame, fH2_frame, fW_frame, fL_tot, fRoofPitch_rad,
                    fDist_Purlin, fDist_Girt, fDist_FrontGirts, fDist_BackGirts, fDist_FrontColumns, fDist_BackColumns,
                    fSlopeFactor, m_arrLoadCases, generalLoad, wind, snow);
                surfaceLoadGenerator.GenerateSurfaceLoads();
            }

            #endregion

            #region Earthquake - nodal loads
            // Earthquake

            if (bGenerateNodalLoads)
            {
                int iNumberOfLoadsInXDirection = iFrameNo;
                int iNumberOfLoadsInYDirection = 2;

                CNodalLoadGenerator nodalLoadGenerator = new CNodalLoadGenerator(iNumberOfLoadsInXDirection, iNumberOfLoadsInYDirection, m_arrLoadCases, m_arrNodes,/* fL1_frame,*/ eq);
                nodalLoadGenerator.GenerateNodalLoads();
            }
            #endregion

            #region Member Loads
            if (bGenerateLoadsOnGirts || bGenerateLoadsOnPurlins || bGenerateLoadsOnColumns || bGenerateLoadsOnFrameMembers)
            {
                CMemberLoadGenerator loadGenerator =
                new CMemberLoadGenerator(iFrameNo,
                fL1_frame,
                fL_tot,
                fSlopeFactor,
                m_arrCrSc[(int)EMemberGroupNames.eGirtWall],
                m_arrCrSc[(int)EMemberGroupNames.ePurlin],
                fDist_Girt,
                fDist_Purlin,
                m_arrCrSc[(int)EMemberGroupNames.eMainColumn],
                m_arrCrSc[(int)EMemberGroupNames.eRafter],
                m_arrCrSc[(int)EMemberGroupNames.eMainColumn_EF],
                m_arrCrSc[(int)EMemberGroupNames.eRafter_EF],
                m_arrLoadCases,
                m_arrMembers,
                generalLoad,
                snow,
                wind);

                #region Secondary Member Loads (girts, purlins, wind posts, door trimmers)
                // Purlins, eave purlins, girts, ....
                LoadCasesMemberLoads memberLoadsOnPurlinsGirtsColumns = new LoadCasesMemberLoads();
                // Generate single member loads
                if (bGenerateLoadsOnGirts || bGenerateLoadsOnPurlins || bGenerateLoadsOnColumns)
                {
                    memberLoadsOnPurlinsGirtsColumns = loadGenerator.GetGeneratedMemberLoads(m_arrLoadCases, m_arrMembers);
                    loadGenerator.AssignMemberLoadListsToLoadCases(memberLoadsOnPurlinsGirtsColumns);
                }
                #endregion

                #region Frame Member Loads (main and edge columns and rafters)
                // Frame Member Loads
                LoadCasesMemberLoads memberLoadsOnFrames = new LoadCasesMemberLoads();
                if (bGenerateLoadsOnFrameMembers)
                {
                    memberLoadsOnFrames = loadGenerator.GetGenerateMemberLoadsOnFrames();
                    loadGenerator.AssignMemberLoadListsToLoadCases(memberLoadsOnFrames);
                }
                #endregion

                #region Merge Member Load Lists
                if ((bGenerateLoadsOnGirts || bGenerateLoadsOnPurlins || bGenerateLoadsOnColumns) && bGenerateLoadsOnFrameMembers)
                {
                    if (memberLoadsOnFrames.Count != memberLoadsOnPurlinsGirtsColumns.Count)
                    {
                        throw new Exception("Not all member load list in all load cases were generated for frames and single members.");
                    }

                    // Merge lists
                    memberLoadsOnFrames.Merge(memberLoadsOnPurlinsGirtsColumns); //Merge both to first LoadCasesMemberLoads
                    // Assign merged list of member loads to the load cases
                    loadGenerator.AssignMemberLoadListsToLoadCases(memberLoadsOnFrames);
                }
                #endregion
            }

            #endregion

            #region Load Groups
            // Create load groups and assigned load cases to the load group
            // Load Case Groups
            m_arrLoadCaseGroups = new CLoadCaseGroup[10];

            // Dead Load
            m_arrLoadCaseGroups[0] = new CLoadCaseGroup(1, "Dead load", ELCGTypeForLimitState.eUniversal, ELCGType.eTogether);
            m_arrLoadCaseGroups[0].MLoadCasesList.Add(m_arrLoadCases[00]);

            // Imposed Load
            m_arrLoadCaseGroups[1] = new CLoadCaseGroup(2, "Imposed load", ELCGTypeForLimitState.eUniversal, ELCGType.eExclusive);
            m_arrLoadCaseGroups[1].MLoadCasesList.Add(m_arrLoadCases[01]);

            // ULS Load Case Groups
            // Snow Load - only one item from group can be in combination
            m_arrLoadCaseGroups[2] = new CLoadCaseGroup(3, "Snow load", ELCGTypeForLimitState.eULSOnly, ELCGType.eExclusive);
            m_arrLoadCaseGroups[2].MLoadCasesList.Add(m_arrLoadCases[02]);
            m_arrLoadCaseGroups[2].MLoadCasesList.Add(m_arrLoadCases[03]);
            m_arrLoadCaseGroups[2].MLoadCasesList.Add(m_arrLoadCases[04]);

            // Wind Load - only one item from group can be in combination
            m_arrLoadCaseGroups[3] = new CLoadCaseGroup(4, "Wind load - Cpi", ELCGTypeForLimitState.eULSOnly, ELCGType.eExclusive);
            m_arrLoadCaseGroups[3].MLoadCasesList.Add(m_arrLoadCases[05]);
            m_arrLoadCaseGroups[3].MLoadCasesList.Add(m_arrLoadCases[06]);
            m_arrLoadCaseGroups[3].MLoadCasesList.Add(m_arrLoadCases[07]);
            m_arrLoadCaseGroups[3].MLoadCasesList.Add(m_arrLoadCases[08]);
            m_arrLoadCaseGroups[3].MLoadCasesList.Add(m_arrLoadCases[09]);
            m_arrLoadCaseGroups[3].MLoadCasesList.Add(m_arrLoadCases[10]);
            m_arrLoadCaseGroups[3].MLoadCasesList.Add(m_arrLoadCases[11]);
            m_arrLoadCaseGroups[3].MLoadCasesList.Add(m_arrLoadCases[12]);

            // Wind Load - only one item from group can be in combination
            m_arrLoadCaseGroups[4] = new CLoadCaseGroup(5, "Wind load - Cpe", ELCGTypeForLimitState.eULSOnly, ELCGType.eExclusive);
            m_arrLoadCaseGroups[4].MLoadCasesList.Add(m_arrLoadCases[13]);
            m_arrLoadCaseGroups[4].MLoadCasesList.Add(m_arrLoadCases[14]);
            m_arrLoadCaseGroups[4].MLoadCasesList.Add(m_arrLoadCases[15]);
            m_arrLoadCaseGroups[4].MLoadCasesList.Add(m_arrLoadCases[16]);
            m_arrLoadCaseGroups[4].MLoadCasesList.Add(m_arrLoadCases[17]);
            m_arrLoadCaseGroups[4].MLoadCasesList.Add(m_arrLoadCases[18]);
            m_arrLoadCaseGroups[4].MLoadCasesList.Add(m_arrLoadCases[19]);
            m_arrLoadCaseGroups[4].MLoadCasesList.Add(m_arrLoadCases[20]);

            // Earthquake Load
            m_arrLoadCaseGroups[5] = new CLoadCaseGroup(6, "Earthquake", ELCGTypeForLimitState.eULSOnly, ELCGType.eExclusive);
            m_arrLoadCaseGroups[5].MLoadCasesList.Add(m_arrLoadCases[21]);
            m_arrLoadCaseGroups[5].MLoadCasesList.Add(m_arrLoadCases[22]);

            // SLS Load Case Groups
            // Snow Load - only one item from group can be in combination
            m_arrLoadCaseGroups[6] = new CLoadCaseGroup(7, "Snow load", ELCGTypeForLimitState.eSLSOnly, ELCGType.eExclusive);
            m_arrLoadCaseGroups[6].MLoadCasesList.Add(m_arrLoadCases[23]);
            m_arrLoadCaseGroups[6].MLoadCasesList.Add(m_arrLoadCases[24]);
            m_arrLoadCaseGroups[6].MLoadCasesList.Add(m_arrLoadCases[25]);

            // Wind Load - only one item from group can be in combination
            m_arrLoadCaseGroups[7] = new CLoadCaseGroup(8, "Wind load - Cpi", ELCGTypeForLimitState.eSLSOnly, ELCGType.eExclusive);
            m_arrLoadCaseGroups[7].MLoadCasesList.Add(m_arrLoadCases[26]);
            m_arrLoadCaseGroups[7].MLoadCasesList.Add(m_arrLoadCases[27]);
            m_arrLoadCaseGroups[7].MLoadCasesList.Add(m_arrLoadCases[28]);
            m_arrLoadCaseGroups[7].MLoadCasesList.Add(m_arrLoadCases[29]);
            m_arrLoadCaseGroups[7].MLoadCasesList.Add(m_arrLoadCases[30]);
            m_arrLoadCaseGroups[7].MLoadCasesList.Add(m_arrLoadCases[31]);
            m_arrLoadCaseGroups[7].MLoadCasesList.Add(m_arrLoadCases[32]);
            m_arrLoadCaseGroups[7].MLoadCasesList.Add(m_arrLoadCases[33]);

            // Wind Load - only one item from group can be in combination
            m_arrLoadCaseGroups[8] = new CLoadCaseGroup(9, "Wind load - Cpe", ELCGTypeForLimitState.eSLSOnly, ELCGType.eExclusive);
            m_arrLoadCaseGroups[8].MLoadCasesList.Add(m_arrLoadCases[34]);
            m_arrLoadCaseGroups[8].MLoadCasesList.Add(m_arrLoadCases[35]);
            m_arrLoadCaseGroups[8].MLoadCasesList.Add(m_arrLoadCases[36]);
            m_arrLoadCaseGroups[8].MLoadCasesList.Add(m_arrLoadCases[37]);
            m_arrLoadCaseGroups[8].MLoadCasesList.Add(m_arrLoadCases[38]);
            m_arrLoadCaseGroups[8].MLoadCasesList.Add(m_arrLoadCases[39]);
            m_arrLoadCaseGroups[8].MLoadCasesList.Add(m_arrLoadCases[40]);
            m_arrLoadCaseGroups[8].MLoadCasesList.Add(m_arrLoadCases[41]);

            // Earthquake Load
            m_arrLoadCaseGroups[9] = new CLoadCaseGroup(10, "Earthquake", ELCGTypeForLimitState.eSLSOnly, ELCGType.eExclusive);
            m_arrLoadCaseGroups[9].MLoadCasesList.Add(m_arrLoadCases[42]);
            m_arrLoadCaseGroups[9].MLoadCasesList.Add(m_arrLoadCases[43]);

            #endregion

            #region Load Combinations
            // Load Combinations
            CLoadCombinationsGenerator generator = new CLoadCombinationsGenerator(m_arrLoadCaseGroups);
            generator.GenerateAll();            
            m_arrLoadCombs = generator.Combinations.ToArray();
            #endregion

            #region Limit states
            // Limit States
            m_arrLimitStates = new CLimitState[3];
            m_arrLimitStates[0] = new CLimitState("Ultimate Limit State - Stability", ELSType.eLS_ULS);
            m_arrLimitStates[1] = new CLimitState("Ultimate Limit State - Strength", ELSType.eLS_ULS);
            m_arrLimitStates[2] = new CLimitState("Serviceability Limit State", ELSType.eLS_SLS);
            #endregion
        }

        public void CalcPurlinNodeCoord(float x_rel, out float x_global, out float z_global)
        {
            x_global = (float)Math.Cos(fRoofPitch_rad) * x_rel;
            z_global = fH1_frame + (float)Math.Sin(fRoofPitch_rad) * x_rel;
        }

        public void CalcColumnNodeCoord_Z(float x, out float z_global)
        {
            z_global = fH1_frame + (float)Math.Tan(fRoofPitch_rad) * x;
        }

        // Rotate Node in Front or Back Frame about Z (angle between X and Front or Back Frame
        public void RotateFrontOrBackFrameNodeAboutZ(CNode node_temp)
        {
            // Rake Angles - Front and Back Frame
            // Upravi suradnice Y vsetkych uzlov s Y = 0 a s Y = L
            // Prepocita suradnicu podla hodnoty X a uhlu, ktory je nastaveny medzi globalnou osou X a prednym (prvym) alebo zadnym (poslednym) ramom
            // Tato uprava umoznuje zosikmenie prveho / posledneho ramu voci mezilahlym

            if (node_temp != null)
            {
                if (MathF.d_equal(node_temp.Y, 0) && !MathF.d_equal(fFrontFrameRakeAngle_temp_rad, 0)) // Front Frame
                {
                    node_temp.Y += node_temp.X * (float)Math.Tan(fFrontFrameRakeAngle_temp_rad);
                }

                if (MathF.d_equal(node_temp.Y, fL_tot) && !MathF.d_equal(fBackFrameRakeAngle_temp_rad, 0)) // Back Frame
                {
                    node_temp.Y += node_temp.X * (float)Math.Tan(fBackFrameRakeAngle_temp_rad);
                }
            }
        }

        public void RotateAndTranslateNodeAboutZ_CCW(CPoint pControlPoint, CNode node, float fAngle_rad)
        {
            // Rotate node
            float fx = (float)Geom2D.GetRotatedPosition_x_CCW_rad(node.X, node.Y, fAngle_rad);
            float fy = (float)Geom2D.GetRotatedPosition_y_CCW_rad(node.X, node.Y, fAngle_rad);

            // Set rotated coordinates
            node.X = fx;
            node.Y = fy;

            // Translate node
            node.X += (float)pControlPoint.X;
            node.Y += (float)pControlPoint.Y;
        }

        public void AddColumnsNodes(int i_temp_numberofNodes, int i_temp_numberofMembers, int iOneRafterColumnNo, int iColumnNoInOneFrame, float fDist_Columns, float fy_Global_Coord)
        {
            float z_glob;

            // Bottom nodes
            for (int i = 0; i < iOneRafterColumnNo; i++)
            {
                CalcColumnNodeCoord_Z((i + 1) * fDist_Columns, out z_glob);
                m_arrNodes[i_temp_numberofNodes + i] = new CNode(i_temp_numberofNodes + i + 1, (i + 1) * fDist_Columns, fy_Global_Coord, 0, 0);
                RotateFrontOrBackFrameNodeAboutZ(m_arrNodes[i_temp_numberofNodes + i]);
            }

            // Bottom nodes
            for (int i = 0; i < iOneRafterColumnNo; i++)
            {
                CalcColumnNodeCoord_Z((i + 1) * fDist_Columns, out z_glob);
                m_arrNodes[i_temp_numberofNodes + iOneRafterColumnNo + i] = new CNode(i_temp_numberofNodes + iOneRafterColumnNo + i + 1, fW_frame - ((i + 1) * fDist_Columns), fy_Global_Coord, 0, 0);
                RotateFrontOrBackFrameNodeAboutZ(m_arrNodes[i_temp_numberofNodes + iOneRafterColumnNo + i]);
            }

            // Top nodes
            for (int i = 0; i < iOneRafterColumnNo; i++)
            {
                CalcColumnNodeCoord_Z((i + 1) * fDist_Columns, out z_glob);
                m_arrNodes[i_temp_numberofNodes + iColumnNoInOneFrame + i] = new CNode(i_temp_numberofNodes + 2 * iOneRafterColumnNo + i + 1, (i + 1) * fDist_Columns, fy_Global_Coord, z_glob, 0);
                RotateFrontOrBackFrameNodeAboutZ(m_arrNodes[i_temp_numberofNodes + iColumnNoInOneFrame + i]);
            }

            // Top nodes
            for (int i = 0; i < iOneRafterColumnNo; i++)
            {
                CalcColumnNodeCoord_Z((i + 1) * fDist_Columns, out z_glob);
                m_arrNodes[i_temp_numberofNodes + iColumnNoInOneFrame + iOneRafterColumnNo + i] = new CNode(i_temp_numberofNodes + 3 * iOneRafterColumnNo + i + 1, fW_frame - ((i + 1) * fDist_Columns), fy_Global_Coord, z_glob, 0);
                RotateFrontOrBackFrameNodeAboutZ(m_arrNodes[i_temp_numberofNodes + iColumnNoInOneFrame + iOneRafterColumnNo + i]);
            }
        }

        public void AddColumnsMembers(
            int i_temp_numberofNodes,
            int i_temp_numberofMembers,
            int iOneRafterColumnNo,
            int iColumnNoInOneFrame,
            CMemberEccentricity eccentricityColumn,
            float fColumnAlignmentStart,
            float fColumnAlignmentEnd,
            CCrSc section,
            float fMemberRotation,
            bool bUseFlyBracing,
            int iFlyBracing_Every_XXSupportingMember,
            float fFirstSupportingMemberPositionAbsolute,
            float fSupportingMembersDistance)
        {
            // Members - Columns
            for (int i = 0; i < iOneRafterColumnNo; i++)
            {
                m_arrMembers[i_temp_numberofMembers + i] = new CMember(i_temp_numberofMembers + i + 1, m_arrNodes[i_temp_numberofNodes + i], m_arrNodes[i_temp_numberofNodes + iColumnNoInOneFrame + i], section, EMemberType_FS.eC, eccentricityColumn, eccentricityColumn, fColumnAlignmentStart, fColumnAlignmentEnd, fMemberRotation, 0);
                CreateAndAssignIrregularTransverseSupportGroupAndLTBsegmentGroup(bUseFlyBracing, iFlyBracing_Every_XXSupportingMember, fFirstSupportingMemberPositionAbsolute, fSupportingMembersDistance, ref m_arrMembers[i_temp_numberofMembers + i]);
            }

            for (int i = 0; i < iOneRafterColumnNo; i++)
            {
                m_arrMembers[i_temp_numberofMembers + iOneRafterColumnNo + i] = new CMember(i_temp_numberofMembers + iOneRafterColumnNo + i + 1, m_arrNodes[i_temp_numberofNodes + iOneRafterColumnNo + i], m_arrNodes[i_temp_numberofNodes + iColumnNoInOneFrame + iOneRafterColumnNo + i], section, EMemberType_FS.eC, eccentricityColumn, eccentricityColumn, fColumnAlignmentStart, fColumnAlignmentEnd, fMemberRotation, 0);
                CreateAndAssignIrregularTransverseSupportGroupAndLTBsegmentGroup(bUseFlyBracing, iFlyBracing_Every_XXSupportingMember, fFirstSupportingMemberPositionAbsolute, fSupportingMembersDistance, ref m_arrMembers[i_temp_numberofMembers + iOneRafterColumnNo + i]);
            }
        }

        public int GetNumberofIntermediateNodesInOneColumnForGirts(float fBottomGirtPosition_temp, float fDistBetweenColumns, float fz_UpperLimitForGirts, int iColumnIndex)
        {
            float fz_gcs_column_temp;

            CalcColumnNodeCoord_Z((iColumnIndex + 1) * fDistBetweenColumns, out fz_gcs_column_temp);
            int iNumber_of_segments = (int)((fz_gcs_column_temp - fz_UpperLimitForGirts - fBottomGirtPosition_temp) / fDist_Girt);
            return iNumber_of_segments + 1;
        }

        public int GetNumberofIntermediateNodesInColumnsForOneFrame(int iOneRafterColumnNo, float fBottomGirtPosition_temp, float fDistBetweenColumns, float fz_UpperLimitForGirts)
        {
            int iNo_temp = 0;
            for (int i = 0; i < iOneRafterColumnNo; i++)
            {
                iNo_temp += GetNumberofIntermediateNodesInOneColumnForGirts(fBottomGirtPosition_temp, fDistBetweenColumns, fz_UpperLimitForGirts, i);
            }

            return iNo_temp;
        }

        public void AddFrontOrBackGirtsNodes(int iOneRafterColumnNo, int[] iArrNumberOfNodesPerColumn, int i_temp_numberofNodes, int iIntermediateColumnNodesForGirtsOneRafterNo, float fDist_Girts, float fDist_Columns, float fy_Global_Coord)
        {
            int iTemp = 0;

            for (int i = 0; i < iOneRafterColumnNo; i++)
            {
                float z_glob;
                CalcColumnNodeCoord_Z((i + 1) * fDist_Columns, out z_glob);

                for (int j = 0; j < iArrNumberOfNodesPerColumn[i]; j++)
                {
                    m_arrNodes[i_temp_numberofNodes + iTemp + j] = new CNode(i_temp_numberofNodes + iTemp + j + 1, (i + 1) * fDist_Columns, fy_Global_Coord, fBottomGirtPosition + j * fDist_Girts, 0);
                    RotateFrontOrBackFrameNodeAboutZ(m_arrNodes[i_temp_numberofNodes + iTemp + j]);
                }

                iTemp += iArrNumberOfNodesPerColumn[i];
            }

            iTemp = 0;

            for (int i = 0; i < iOneRafterColumnNo; i++)
            {
                float z_glob;
                CalcColumnNodeCoord_Z((i + 1) * fDist_Columns, out z_glob);

                for (int j = 0; j < iArrNumberOfNodesPerColumn[i]; j++)
                {
                    m_arrNodes[i_temp_numberofNodes + iIntermediateColumnNodesForGirtsOneRafterNo + iTemp + j] = new CNode(i_temp_numberofNodes + iIntermediateColumnNodesForGirtsOneRafterNo + iTemp + j + 1, fW_frame - (i + 1) * fDist_Columns, fy_Global_Coord, fBottomGirtPosition + j * fDist_Girts, 0);
                    RotateFrontOrBackFrameNodeAboutZ(m_arrNodes[i_temp_numberofNodes + iIntermediateColumnNodesForGirtsOneRafterNo + iTemp + j]);
                }

                iTemp += iArrNumberOfNodesPerColumn[i];
            }
        }

        public void AddFrontOrBackGirtsMembers(int iFrameNodesNo, int iOneRafterColumnNo, int[] iArrNumberOfNodesPerColumn, int i_temp_numberofNodes, int i_temp_numberofMembers,
            int iIntermediateColumnNodesForGirtsOneRafterNo, int iIntermediateColumnNodesForGirtsOneFrameNo, int iTempJumpBetweenFrontAndBack_GirtsNumberInLongidutinalDirection,
            float fDist_Girts, CMemberEccentricity eGirtEccentricity, float fGirtStart_MC, float fGirtStart, float fGirtEnd, CCrSc section, float fMemberRotation, int iNumberOfTransverseSupports)
        {
            int iTemp = 0;
            int iTemp2 = 0;
            int iOneColumnGirtNo_temp = (int)((fH1_frame - fUpperGirtLimit - fBottomGirtPosition) / fDist_Girt) + 1;

            for (int i = 0; i < iOneRafterColumnNo + 1; i++)
            {
                if (i == 0) // First session depends on number of girts at main frame column
                {
                    for (int j = 0; j < iOneColumnGirtNo_temp; j++)
                    {
                        m_arrMembers[i_temp_numberofMembers + j] = new CMember(i_temp_numberofMembers + j + 1, m_arrNodes[iFrameNodesNo * iFrameNo + iTempJumpBetweenFrontAndBack_GirtsNumberInLongidutinalDirection + j], m_arrNodes[i_temp_numberofNodes + j], section, EMemberType_FS.eG, eGirtEccentricity, eGirtEccentricity, fGirtStart_MC, fGirtEnd, fMemberRotation, 0);
                        CreateAndAssignRegularTransverseSupportGroupAndLTBsegmentGroup(m_arrMembers[i_temp_numberofMembers + j], iNumberOfTransverseSupports);
                    }

                    iTemp += iOneColumnGirtNo_temp;
                }
                else if (i < iOneRafterColumnNo) // Other sessions
                {
                    for (int j = 0; j < iArrNumberOfNodesPerColumn[i - 1]; j++)
                    {
                        m_arrMembers[i_temp_numberofMembers + iTemp + j] = new CMember(i_temp_numberofMembers + iTemp + j + 1, m_arrNodes[i_temp_numberofNodes + iTemp2 + j], m_arrNodes[i_temp_numberofNodes + iArrNumberOfNodesPerColumn[i - 1] + iTemp2 + j], section, EMemberType_FS.eG, eGirtEccentricity, eGirtEccentricity, fGirtStart, fGirtEnd, fMemberRotation, 0);
                        CreateAndAssignRegularTransverseSupportGroupAndLTBsegmentGroup(m_arrMembers[i_temp_numberofMembers + iTemp + j], iNumberOfTransverseSupports);
                    }

                    iTemp2 += iArrNumberOfNodesPerColumn[i - 1];
                    iTemp += iArrNumberOfNodesPerColumn[i - 1];
                }
                else // Last session - prechadza cez stred budovy
                {
                    for (int j = 0; j < iArrNumberOfNodesPerColumn[i - 1]; j++)
                    {
                        m_arrMembers[i_temp_numberofMembers + iTemp + j] = new CMember(i_temp_numberofMembers + iTemp + j + 1, m_arrNodes[i_temp_numberofNodes + iTemp2 + j], m_arrNodes[i_temp_numberofNodes + iIntermediateColumnNodesForGirtsOneFrameNo - iArrNumberOfNodesPerColumn[iOneRafterColumnNo - 1] + j], section, EMemberType_FS.eG, eGirtEccentricity, eGirtEccentricity, fGirtStart, fGirtEnd, fMemberRotation, 0);
                        CreateAndAssignRegularTransverseSupportGroupAndLTBsegmentGroup(m_arrMembers[i_temp_numberofMembers + iTemp + j], iNumberOfTransverseSupports);
                    }

                    iTemp += iArrNumberOfNodesPerColumn[i - 1];
                }
            }

            iTemp = 0;
            iTemp2 = 0;

            for (int i = 0; i < iOneRafterColumnNo; i++)
            {
                int iNumberOfMembers_temp = iOneColumnGirtNo_temp + iIntermediateColumnNodesForGirtsOneRafterNo;

                if (i == 0) // First session depends on number of girts at main frame column
                {
                    for (int j = 0; j < iOneColumnGirtNo_temp; j++)
                    {
                        m_arrMembers[i_temp_numberofMembers + iNumberOfMembers_temp + j] = new CMember(i_temp_numberofMembers + iNumberOfMembers_temp + j + 1, m_arrNodes[iFrameNodesNo * iFrameNo + iTempJumpBetweenFrontAndBack_GirtsNumberInLongidutinalDirection + iOneColumnGirtNo_temp + j], m_arrNodes[i_temp_numberofNodes + iIntermediateColumnNodesForGirtsOneRafterNo + j], section, EMemberType_FS.eG, eGirtEccentricity, eGirtEccentricity, fGirtStart_MC, fGirtEnd, -fMemberRotation, 0);
                        CreateAndAssignRegularTransverseSupportGroupAndLTBsegmentGroup(m_arrMembers[i_temp_numberofMembers + iNumberOfMembers_temp + j], iNumberOfTransverseSupports);
                    }

                    iTemp += iOneColumnGirtNo_temp;
                }
                else // Other sessions (not in the middle)
                {
                    for (int j = 0; j < iArrNumberOfNodesPerColumn[i - 1]; j++)
                    {
                        m_arrMembers[i_temp_numberofMembers + iNumberOfMembers_temp + iTemp + j] = new CMember(i_temp_numberofMembers + iNumberOfMembers_temp + iTemp + j + 1, m_arrNodes[i_temp_numberofNodes + iIntermediateColumnNodesForGirtsOneRafterNo + iTemp2 + j], m_arrNodes[i_temp_numberofNodes + iIntermediateColumnNodesForGirtsOneRafterNo + iArrNumberOfNodesPerColumn[i - 1] + iTemp2 + j], section, EMemberType_FS.eG, eGirtEccentricity, eGirtEccentricity, fGirtStart, fGirtEnd, -fMemberRotation, 0);
                        CreateAndAssignRegularTransverseSupportGroupAndLTBsegmentGroup(m_arrMembers[i_temp_numberofMembers + iNumberOfMembers_temp + iTemp + j], iNumberOfTransverseSupports);
                    }

                    iTemp2 += iArrNumberOfNodesPerColumn[i - 1];
                    iTemp += iArrNumberOfNodesPerColumn[i - 1];
                }
            }
        }

        public void GetJointAllignments(float fh_column, float fh_rafter, out float allignment_column, out float allignment_knee_rafter, out float allignment_apex_rafter)
        {
            float cosAlpha = (float)Math.Cos(fRoofPitch_rad);
            float sinAlpha = (float)Math.Sin(fRoofPitch_rad);
            float tanAlpha = (float)Math.Tan(fRoofPitch_rad);

            /*
            float y = fh_rafter / cosAlpha;
            float a = sinAlpha * 0.5f * y;
            float x = cosAlpha * 2f * a;
            float x2 = 0.5f * fh_column - x;
            float y2 = tanAlpha * x2;
            allignment_column = 0.5f * y + y2;

            float x3 = 0.5f * x;
            float x4 = 0.5f * fh_column - x3;
            allignment_knee_rafter = x4 / cosAlpha;

            allignment_apex_rafter = a;
            */

            float y = fh_rafter / cosAlpha;
            allignment_apex_rafter = sinAlpha * 0.5f * y;
            float x = cosAlpha * 2f * allignment_apex_rafter;
            allignment_column = 0.5f * y + (tanAlpha * (0.5f * fh_column - x));
            allignment_knee_rafter = (0.5f * fh_column - (0.5f * x)) / cosAlpha;
        }

        public void AddDoorBlock(DoorProperties prop, float fLimitDistanceFromColumn, float fSideWallHeight)
        {
            CMember mReferenceGirt;
            CMember mColumnLeft;
            CMember mColumnRight;
            CBlock_3D_001_DoorInBay door;
            CPoint pControlPointBlock;
            float fBayWidth;
            float fBayHeight = fH1_frame; // TODO - spocitat vysku bay v mieste bloku (pre front a back budu dve vysky v mieste vlozenia stlpov bloku
            int iFirstMemberToDeactivate;
            bool bIsReverseSession;
            bool bIsFirstBayInFrontorBackSide;
            bool bIsLastBayInFrontorBackSide;

            DeterminateBasicPropertiesToInsertBlock(prop.sBuildingSide, prop.iBayNumber, out mReferenceGirt, out mColumnLeft, out mColumnRight, out pControlPointBlock, out fBayWidth, out iFirstMemberToDeactivate, out bIsReverseSession, out bIsFirstBayInFrontorBackSide, out bIsLastBayInFrontorBackSide);

            door = new CBlock_3D_001_DoorInBay(
                prop,
                fLimitDistanceFromColumn,
                fBottomGirtPosition,
                fDist_Girt,
                mReferenceGirt,
                mColumnLeft,
                mColumnRight,
                fBayWidth,
                fBayHeight,
                fUpperGirtLimit,
                bIsReverseSession,
                bIsFirstBayInFrontorBackSide,
                bIsLastBayInFrontorBackSide);

            AddDoorOrWindowBlockProperties(pControlPointBlock, iFirstMemberToDeactivate, door);

            DoorsModels.Add(door);
        }

        public void AddWindowBlock(WindowProperties prop, float fLimitDistanceFromColumn)
        {
            CMember mReferenceGirt;
            CMember mColumnLeft;
            CMember mColumnRight;
            CBlock_3D_002_WindowInBay window;
            CPoint pControlPointBlock;
            float fBayWidth;
            float fBayHeight = fH1_frame; // TODO - spocitat vysku bay v mieste bloku (pre front a back budu dve vysky v mieste vlozenia stlpov bloku
            int iFirstGirtInBay;
            int iFirstMemberToDeactivate;
            bool bIsReverseSession;
            bool bIsFirstBayInFrontorBackSide;
            bool bIsLastBayInFrontorBackSide;

            DeterminateBasicPropertiesToInsertBlock(prop.sBuildingSide, prop.iBayNumber, out mReferenceGirt, out mColumnLeft, out mColumnRight, out pControlPointBlock, out fBayWidth, out iFirstGirtInBay, out bIsReverseSession, out bIsFirstBayInFrontorBackSide, out bIsLastBayInFrontorBackSide);

            window = new CBlock_3D_002_WindowInBay(
                prop,
                fLimitDistanceFromColumn,
                fBottomGirtPosition,
                fDist_Girt,
                mReferenceGirt,
                mColumnLeft,
                mColumnRight,
                fBayWidth,
                fBayHeight,
                fUpperGirtLimit,
                bIsReverseSession,
                bIsFirstBayInFrontorBackSide,
                bIsLastBayInFrontorBackSide);

            iFirstMemberToDeactivate = iFirstGirtInBay + window.iNumberOfGirtsUnderWindow;

            AddDoorOrWindowBlockProperties(pControlPointBlock, iFirstMemberToDeactivate, window);

            WindowsModels.Add(window);
        }

        public void DeterminateBasicPropertiesToInsertBlock(
            string sBuildingSide,                     // Identification of building side (left, right, front, back)
            int iBayNumber,                           // Bay number (1-n) in positive X or Y direction
            out CMember mReferenceGirt,               // Reference girt - first girts that needs to be deactivated and replaced by new member (some parameters are same for deactivated and new member)
            out CMember mColumnLeft,                  // Left column of bay
            out CMember mColumnRight,                 // Right column of bay
            out CPoint pControlPointBlock,            // Conctrol point to insert block - defined as left column base point
            out float fBayWidth,                      // Width of bay (distance between bay columns)
            out int iFirstMemberToDeactivate,         // Index of first girt in the bay which is in collision with the block and must be deactivated
            out bool bIsReverseSession,               // Front or back wall bay can have reverse direction of girts in X
            out bool bIsFirstBayInFrontorBackSide,
            out bool bIsLastBayInFrontorBackSide
            )
        {
            bIsReverseSession = false;            // Set to true value just for front or back wall (right part of wall)
            bIsFirstBayInFrontorBackSide = false; // Set to true value just for front or back wall (first bay)
            bIsLastBayInFrontorBackSide = false;  // Set to true value just for front or back wall (last bay)

            if (sBuildingSide == "Left" || sBuildingSide == "Right")
            {
                // Left side X = 0, Right Side X = GableWidth
                // Insert after frame ID
                int iSideMultiplier = sBuildingSide == "Left" ? 0 : 1; // 0 left side X = 0, 1 - right side X = Gable Width
                int iBlockFrame = iBayNumber - 1; // ID of frame in the bay, starts with zero

                int iBayColumnLeft = (iBlockFrame * 6) + (iSideMultiplier == 0 ? 0 : (4 - 1)); // (2 columns + 2 rafters + 2 eaves purlins) = 6, For Y = GableWidth + 4 number of members in one frame - 1 (index)
                int iBayColumnRight = ((iBlockFrame + 1) * 6) + (iSideMultiplier == 0 ? 0 : (4 - 1));
                fBayWidth = fL1_frame;
                iFirstMemberToDeactivate = iMainColumnNo + iRafterNo + iEavesPurlinNo + iBlockFrame * iGirtNoInOneFrame + iSideMultiplier * (iGirtNoInOneFrame / 2);

                mReferenceGirt = m_arrMembers[iFirstMemberToDeactivate]; // Deactivated member properties define properties of block girts
                mColumnLeft = m_arrMembers[iBayColumnLeft];
                mColumnRight = m_arrMembers[iBayColumnRight];
            }
            else // Front or Back Side
            {
                // Insert after sequence ID
                int iNumberOfIntermediateColumns;
                int[] iArrayOfGirtsPerColumnCount;
                //int iNumberOfGirtsInWall;

                if (sBuildingSide == "Front")  // Front side properties
                {
                    iNumberOfIntermediateColumns = iFrontColumnNoInOneFrame;
                    iArrayOfGirtsPerColumnCount = iArrNumberOfNodesPerFrontColumn;
                    //iNumberOfGirtsInWall = iFrontGirtsNoInOneFrame;
                    fBayWidth = fDist_FrontColumns;
                }
                else // Back side properties
                {
                    iNumberOfIntermediateColumns = iBackColumnNoInOneFrame;
                    iArrayOfGirtsPerColumnCount = iArrNumberOfNodesPerBackColumn;
                    //iNumberOfGirtsInWall = iBackGirtsNoInOneFrame;
                    fBayWidth = fDist_BackColumns;
                }

                int iSideMultiplier = sBuildingSide == "Front" ? 0 : 1; // 0 front side Y = 0, 1 - back side Y = Length
                int iBlockSequence = iBayNumber - 1; // ID of sequence, starts with zero
                int iColumnNumberLeft;
                int iColumnNumberRight;
                int iNumberOfFirstGirtInWallToDeactivate = 0;
                int iNumberOfMembers_tempForGirts = iMainColumnNo + iRafterNo + iEavesPurlinNo + (iFrameNo - 1) * iGirtNoInOneFrame + (iFrameNo - 1) * iPurlinNoInOneFrame + iFrontColumnNoInOneFrame + iBackColumnNoInOneFrame + iSideMultiplier * iFrontGirtsNoInOneFrame;
                int iNumberOfMembers_tempForColumns = iMainColumnNo + iRafterNo + iEavesPurlinNo + (iFrameNo - 1) * iGirtNoInOneFrame + (iFrameNo - 1) * iPurlinNoInOneFrame + iSideMultiplier * iFrontColumnNoInOneFrame;

                if (iBlockSequence == 0) // Main Column - first bay
                {
                    if (sBuildingSide == "Front")
                    {
                        iColumnNumberLeft = 0;
                        iColumnNumberRight = iNumberOfMembers_tempForColumns + iBlockSequence;
                    }
                    else
                    {
                        iColumnNumberLeft = (iFrameNo - 1) * 6;
                        iColumnNumberRight = iNumberOfMembers_tempForColumns + iBlockSequence;
                    }

                    iFirstMemberToDeactivate = iNumberOfMembers_tempForGirts + iNumberOfFirstGirtInWallToDeactivate;

                    bIsFirstBayInFrontorBackSide = true; // First bay
                }
                else
                {
                    if (iBlockSequence < (int)(iNumberOfIntermediateColumns / 2) + 1) // Left session
                    {
                        iColumnNumberLeft = iNumberOfMembers_tempForColumns + iBlockSequence - 1;
                        iColumnNumberRight = iNumberOfMembers_tempForColumns + iBlockSequence;

                        iNumberOfFirstGirtInWallToDeactivate += iOneColumnGirtNo;

                        for (int i = 0; i < iBlockSequence - 1; i++)
                            iNumberOfFirstGirtInWallToDeactivate += iArrayOfGirtsPerColumnCount[i];
                    }
                    else // Right session
                    {
                        bIsReverseSession = true; // Nodes and members are numbered from right to the left

                        iColumnNumberLeft = iNumberOfMembers_tempForColumns + (int)(iNumberOfIntermediateColumns / 2) + iNumberOfIntermediateColumns - iBlockSequence;
                        iColumnNumberRight = iNumberOfMembers_tempForColumns + (int)(iNumberOfIntermediateColumns / 2) + iNumberOfIntermediateColumns - iBlockSequence - 1;

                        // Number of girts in left session
                        iNumberOfFirstGirtInWallToDeactivate += iOneColumnGirtNo;

                        for (int i = 0; i < (int)(iNumberOfIntermediateColumns / 2); i++)
                            iNumberOfFirstGirtInWallToDeactivate += iArrayOfGirtsPerColumnCount[i];

                        if (iBlockSequence < iNumberOfIntermediateColumns)
                            iNumberOfFirstGirtInWallToDeactivate += iOneColumnGirtNo;

                        for (int i = 0; i < iNumberOfIntermediateColumns - iBlockSequence - 1; i++)
                            iNumberOfFirstGirtInWallToDeactivate += iArrayOfGirtsPerColumnCount[i];

                        if (iBlockSequence == iNumberOfIntermediateColumns) // Last bay
                        {
                            bIsLastBayInFrontorBackSide = true;
                            iColumnNumberRight = iSideMultiplier == 0 ? 3 : (iFrameNo - 1) * 6 + 3;
                        }
                    }

                    iFirstMemberToDeactivate = iNumberOfMembers_tempForGirts + iNumberOfFirstGirtInWallToDeactivate;
                }

                mReferenceGirt = m_arrMembers[iFirstMemberToDeactivate]; // Deactivated member properties define properties of block girts
                mColumnLeft = m_arrMembers[iColumnNumberLeft];
                mColumnRight = m_arrMembers[iColumnNumberRight];
            }

            pControlPointBlock = new CPoint(0, mColumnLeft.NodeStart.X, mColumnLeft.NodeStart.Y, mColumnLeft.NodeStart.Z, 0);
        }


        //Tuto funkciu mam pozriet - Mato chce:
        //rozsirujem tam velkosti poli a take veci CModel_PFD_01_GR - riadok 1751
        //vlastne tie objekty z objektu CBlock pridavam do celkoveho zoznamu, ale napriklad prerez pre girts som ignoroval aby tam nebol 2x
        //Chce to vymysliet nejaky koncept ako to ma fungovat a chce to programatorsku hlavu 🙂
        //tie moje "patlacky" ako sa to tam dolepuje do poli atd by som nebral velmi vazne
        //Malo by ty to fungovat tak, ze ked pridam prve dvere tak sa tie prierezy pridaju a ked pridavam dalsie, tak uz sa pridavaju len uzly a pruty a prierez sa len nastavi
        //uz by sa nemal vytvarat novy
        public void AddDoorOrWindowBlockProperties(CPoint pControlPointBlock, int iFirstMemberToDeactivate, CBlock block)
        {
            int arraysizeoriginal;
            
            // Cross-sections
            
            // Copy block cross-sections into the model
            for (int i = 1; i < block.m_arrCrSc.Length; i++) // Zacina sa od i = 1 - preskocit prvy prvok v poli doors, pretoze odkaz na girt section uz existuje, nie je potrebne prierez kopirovat znova
            {
                CCrSc foundCrsc = m_arrCrSc.FirstOrDefault(c => c.ID == block.m_arrCrSc[i].ID);
                if (foundCrsc != null) continue;

                arraysizeoriginal = m_arrCrSc.Length;
                Array.Resize(ref m_arrCrSc, arraysizeoriginal + 1); // ( - 1) Prvy prvok v poli blocks crsc ignorujeme
                // Preskocit prvy prvok v poli block crsc, pretoze odkaz na girt section uz existuje, nie je potrebne prierez kopirovat znova
                m_arrCrSc[arraysizeoriginal] = block.m_arrCrSc[i];
                //m_arrCrSc[arraysizeoriginal + i - 1].ID = arraysizeoriginal + i/* -1 + 1*/; // Odcitat index pretoze prvy prierez ignorujeme a pridat 1 pre ID (+ 1)
            }
            
            // Nodes
            arraysizeoriginal = m_arrNodes.Length;
            Array.Resize(ref m_arrNodes, m_arrNodes.Length + block.m_arrNodes.Length);

            int iNumberofMembersToDeactivate = block.INumberOfGirtsToDeactivate;

            // Deactivate already generated members in the bay (space between frames) where is the block inserted
            for (int i = 0; i < iNumberofMembersToDeactivate; i++)
            {
                // Deactivate Members
                CMember m = m_arrMembers[iFirstMemberToDeactivate + i];
                if (m != null)
                {
                    m.BIsGenerated = false;
                    m.BIsDisplayed = false;
                    m.BIsSelectedForIFCalculation = false;
                    m.BIsSelectedForDesign = false;
                    m.BIsSelectedForMaterialList = false;
                }

                // Deactivate Member Joints
                // TODO Ondrej - potrebujeme zistit, ktore spoje su pripojene na prut a deaktivovat ich, aby sa nevytvorili, asi by sme mali na tieto veci vyrobit nejaku mapu alebo dictionary
                // Doplnene 16.3.2019 - potrebne otestovat, ci funguje
                CConnectionJointTypes jStart;
                CConnectionJointTypes jEnd;
                GetModelMemberStartEndConnectionJoints(m, out jStart, out jEnd);

                if (jStart != null)
                {
                    jStart.BIsGenerated = false;
                    jStart.BIsDisplayed = false;
                    jStart.BIsSelectedForIFCalculation = false;
                    jStart.BIsSelectedForDesign = false;
                    jStart.BIsSelectedForMaterialList = false;
                }

                if (jEnd != null)
                {
                    jEnd.BIsGenerated = false;
                    jEnd.BIsDisplayed = false;
                    jEnd.BIsSelectedForIFCalculation = false;
                    jEnd.BIsSelectedForDesign = false;
                    jEnd.BIsSelectedForMaterialList = false;
                }
            }

            float fBlockRotationAboutZaxis_rad = 0;

            if (block.BuildingSide == "Left" || block.BuildingSide == "Right")
                fBlockRotationAboutZaxis_rad = MathF.fPI / 2.0f; // Parameter of block - depending on side of building (front, back (0 deg), left, right (90 deg))

            // Copy block nodes into the model
            for (int i = 0; i < block.m_arrNodes.Length; i++)
            {
                RotateAndTranslateNodeAboutZ_CCW(pControlPointBlock, block.m_arrNodes[i], fBlockRotationAboutZaxis_rad);
                m_arrNodes[arraysizeoriginal + i] = block.m_arrNodes[i];
                m_arrNodes[arraysizeoriginal + i].ID = arraysizeoriginal + i + 1;
            }

            // Members
            arraysizeoriginal = m_arrMembers.Length;
            Array.Resize(ref m_arrMembers, m_arrMembers.Length + block.m_arrMembers.Length);

            // Copy block members into the model
            for (int i = 0; i < block.m_arrMembers.Length; i++)
            {
                // Position of definition nodes was already changed, we dont need to rotate member definition nodes NodeStart and NodeEnd
                // Recalculate basic member data (PointA, PointB, delta projection length)
                block.m_arrMembers[i].Fill_Basic();

                m_arrMembers[arraysizeoriginal + i] = block.m_arrMembers[i];
                m_arrMembers[arraysizeoriginal + i].ID = arraysizeoriginal + i + 1;
            }

            // Add block member connections to the main model connections
            foreach (CConnectionJointTypes joint in block.m_arrConnectionJoints)
                m_arrConnectionJoints.Add(joint);
        }

        // Load model component cross-sections
        public void SetCrossSectionsFromDatabase()
        {
            // Todo - Ondrej
            // Chcel som napojit obsah m_arrCrSc podla 
            // MDBModels tabulka KitsetGableRoofEnclosed alebo KitsetGableRoofEnclosedCrscID
            // Skusis mi aspon jeden prierez napojit prosim ?
            // pole m_arrCrSc by sa malo naplnit prierezmi podla mena prierezu (270115, 27095, 63020, ...) v databaze pre jednotlive typy members (purlin, girt, main column, ...)
            // kazdemu menu prierezu zodpoveda iny objekt z tried, ktore su v CRSC / CrSc_3 / FS
        }

        // Add members to the member group list
        // Base on cross-section ID 
        // TODO - spravnejsie by bolo pridavat member do zoznamu priamo pri vytvoreni
        public void AddMembersToMemberGroupsLists()
        {
            int i = 0;

            // Opravene ID prierezu sa bralo z poradia v databaze a prepisovalo ID prierezu z modelu
            foreach (CMember member in m_arrMembers)
            {
                foreach (CMemberGroup group in listOfModelMemberGroups) // TODO - dalo by sa nahradit napriklad switchom ak pozname presne typy
                {
                    if (member.BIsGenerated && member.CrScStart.ID == group.CrossSection.ID) // In case that cross-section ID is same add member to the list
                    {
                        member.EMemberTypePosition = group.MemberType_FS_Position; // TODO - docasne riesenie, nastavime prutu typ podla pozicie (referencny typ pruta nastaveny v skupine), spravne by sme ho mohli nastavit uz v konstruktore pruta pri jeho vytvoreni
                        group.ListOfMembers.Add(member);
                        //listOfModelMemberGroups[group.CrossSection.ICrSc_ID].ListOfMembers.Add(member);
                        i++;
                        break;
                    }
                }
            }

            // Check
            // TODO - aktivovat po vyrieseni mazania nevygenerovanych prutov zo zoznamu a pridani prutov tvoriacich bloky (dvere, okna, ...)
            /*
            if (i != m_arrMembers.Length)
                throw new Exception("Not all members were added.");
            */
        }

        // TODO - spravnejsie by bolo nastavovat defaultne parametre spoja uz pri vytvoreni
        public void SetJointDefaultParameters()
        {
            foreach (CConnectionJointTypes joint in m_arrConnectionJoints)
            {
                if (joint.m_MainMember != null && joint.m_MainMember.BIsGenerated) // In case that joint main member is generated
                {
                    if (joint.m_MainMember.BIsDisplayed == true) joint.BIsDisplayed = true;
                    if (joint.m_MainMember.BIsSelectedForDesign == true) joint.BIsSelectedForDesign = true;
                    if (joint.m_MainMember.BIsSelectedForMaterialList == true) joint.BIsSelectedForMaterialList = true;
                }
            }
        }

        private List<CSegment_LTB> GenerateIntermediateLTBSegmentsOnMember(List<CIntermediateTransverseSupport> lTransverseSupportGroup, bool bIsRelativeCoordinate_x, float fMemberLength)
        {
            List<CSegment_LTB> LTB_segment_group = null; // TODO - rozhodnut co sa ma generovat ak na prute nie su medzilahle podpory, nic alebo vzdy jeden segment ???

            if (lTransverseSupportGroup != null && lTransverseSupportGroup.Count > 0)
            {
                LTB_segment_group = new List<CSegment_LTB>();

                // Create lateral-torsional buckling segments
                for (int j = 0; j < lTransverseSupportGroup.Count + 1; j++)
                {
                    // Number of segments = number of intermediate supports + 1 - type BothFlanges
                    // TODO - doriesit ako generovat segmenty ak nie su vsetky lateral supports typu BothFlanges
                    // Najprv by sa mal najst pocet podpor s BothFlanges, z toho urcit pocet segmentov a zohladnovat len tie coordinates x,
                    // ktore sa vztahuju na podpory s BothFlanges

                    float fSegmentStart_abs = 0;
                    float fSegmentEnd_abs = fMemberLength;

                    if (j == 0) // First Segment
                    {
                        fSegmentStart_abs = 0f;
                        fSegmentEnd_abs = lTransverseSupportGroup[j].Fx_position_abs;
                    }
                    else if (j < lTransverseSupportGroup.Count)
                    {
                        fSegmentStart_abs = lTransverseSupportGroup[j - 1].Fx_position_abs;
                        fSegmentEnd_abs = lTransverseSupportGroup[j].Fx_position_abs;
                    }
                    else // Last
                    {
                        fSegmentStart_abs = lTransverseSupportGroup[j - 1].Fx_position_abs;
                        fSegmentEnd_abs = fMemberLength;
                    }

                    CSegment_LTB segment = new CSegment_LTB(j + 1, bIsRelativeCoordinate_x, fSegmentStart_abs, fSegmentEnd_abs, fMemberLength);

                    LTB_segment_group.Add(segment);
                }
            }

            return LTB_segment_group;
        }

        private List<CIntermediateTransverseSupport> GenerateRegularIntermediateTransverseSupports(float fMemberLength, int iNumberOfTransverseSupports)
        {
            List<CIntermediateTransverseSupport> TransverseSupportGroup = null;

            float fFirstSupportPosition = fMemberLength / (iNumberOfTransverseSupports + 1); // number of LTB segments = number of support + 1
            float fDistOfSupports = fFirstSupportPosition;

            if (iNumberOfTransverseSupports > 0)
            {
                TransverseSupportGroup = new List<CIntermediateTransverseSupport>();
                for (int j = 0; j < iNumberOfTransverseSupports; j++) // Each suport
                {
                    float fxLocationOfSupport = Math.Min(fFirstSupportPosition + j * fDistOfSupports, fMemberLength);

                    if (fxLocationOfSupport < fMemberLength)
                        TransverseSupportGroup.Add(new CIntermediateTransverseSupport(j + 1, EITSType.eBothFlanges, fxLocationOfSupport / fMemberLength, fxLocationOfSupport, 0));
                    // TODO - To Ondrej, nie som si isty ci mam v kazdej podpore CIntermediateTransverseSupport ukladat jej poziciu (aktualny stav) alebo ma CMember nie list CIntermediateTransverseSupport ale list nejakych struktur (x, CIntermediateTransverseSupport), takze x miesta budu definovane v tejto strukture v objekte CMember a samotny objekt CIntermediateTransverseSupport nebude vediet kde je
                }
            }

            return TransverseSupportGroup;
        }

        private void CreateAndAssignRegularTransverseSupportGroupAndLTBsegmentGroup(CMember member, int iNumberOfTransverseSupports)
        {
            List<CIntermediateTransverseSupport> TransverseSupportGroup = GenerateRegularIntermediateTransverseSupports(member.FLength, iNumberOfTransverseSupports);
            List<CSegment_LTB> LTB_segment_group = GenerateIntermediateLTBSegmentsOnMember(TransverseSupportGroup, false, member.FLength);

            // Assign transverse support group and LTB segment group to the member
            member.IntermediateTransverseSupportGroup = TransverseSupportGroup;
            member.LTBSegmentGroup = LTB_segment_group;
        }

        private void CreateIrregularTransverseSupportGroupAndLTBsegmentGroup(
        bool bUseFlyBracing,
        int iFlyBracing_Every_XXSupportingMember,
        float fFirstSupportingMemberPositionAbsolute,
        float fSupportingMembersDistance,
        ref CMember member,
        out List<CIntermediateTransverseSupport> lTransverseSupportGroup_Member,
        out List<CSegment_LTB> LTB_segment_group_Member)
        {
            lTransverseSupportGroup_Member = null;
            LTB_segment_group_Member = null;

            // Define fly bracing positions and LTB segments on member
            float fMemberLength = member.FLength;

            if (bUseFlyBracing && iFlyBracing_Every_XXSupportingMember > 0)
            {
                    lTransverseSupportGroup_Member = new List<CIntermediateTransverseSupport>();
                    float fFirstFlyBracePosition = fFirstSupportingMemberPositionAbsolute + (iFlyBracing_Every_XXSupportingMember - 1) * fSupportingMembersDistance;
                    int iNumberOfFlyBracesOnMember = fFirstFlyBracePosition < fMemberLength ? (int)((fMemberLength - fFirstFlyBracePosition) / (iFlyBracing_Every_XXSupportingMember * fSupportingMembersDistance)) + 1 : 0;

                    for (int j = 0; j < iNumberOfFlyBracesOnMember; j++) // Each fly brace
                    {
                        float fxLocationOfFlyBrace = fFirstFlyBracePosition + (j * iFlyBracing_Every_XXSupportingMember) * fSupportingMembersDistance;

                        if (fxLocationOfFlyBrace < fMemberLength)
                            lTransverseSupportGroup_Member.Add(new CIntermediateTransverseSupport(j + 1, EITSType.eBothFlanges, fxLocationOfFlyBrace / fMemberLength, fxLocationOfFlyBrace, 0));
                        // TODO - To Ondrej, nie som si isty ci mam v kazdej podpore CIntermediateTransverseSupport ukladat jej poziciu (aktualny stav) alebo ma CMember nie list CIntermediateTransverseSupport ale list nejakych struktur (x, CIntermediateTransverseSupport), takze x miesta budu definovane v tejto strukture v objekte CMember a samotny objekt CIntermediateTransverseSupport nebude vediet kde je
                    }

                    if (lTransverseSupportGroup_Member.Count > 0)
                    {
                        LTB_segment_group_Member = GenerateIntermediateLTBSegmentsOnMember(lTransverseSupportGroup_Member, false, fMemberLength);
                    }
            }
        }

        private void CreateAndAssignIrregularTransverseSupportGroupAndLTBsegmentGroup(
            bool bUseFlyBracing,
            int iFlyBracing_Every_XXSupportingMember,
            float fFirstSupportingMemberPositionAbsolute,
            float fSupportingMembersDistance,
            ref CMember member)
        {
            List<CIntermediateTransverseSupport> lTransverseSupportGroup_Member;
            List<CSegment_LTB> LTB_segment_group_Member;

            CreateIrregularTransverseSupportGroupAndLTBsegmentGroup(
                bUseFlyBracing,
                iFlyBracing_Every_XXSupportingMember,
                fFirstSupportingMemberPositionAbsolute,
                fSupportingMembersDistance,
                ref member,
                out lTransverseSupportGroup_Member,
                out LTB_segment_group_Member);

            if (lTransverseSupportGroup_Member != null && lTransverseSupportGroup_Member.Count > 0)
            {
                // Assign transverse support group to the member
                member.IntermediateTransverseSupportGroup = lTransverseSupportGroup_Member;
                member.LTBSegmentGroup = LTB_segment_group_Member;
            }
        }

        private void CreateAndAssignReversedIrregularTransverseSupportGroupAndLTBsegmentGroup(
            bool bUseFlyBracing,
            int iFlyBracing_Every_XXSupportingMember,
            float fFirstSupportingMemberPositionAbsolute,
            float fSupportingMembersDistance,
            ref CMember member)
        {
            // Suradnice pre member opacne orientovane, takze pouzit L - x
            float fmemberLength = member.FLength;
            List<CIntermediateTransverseSupport> lTransverseSupportGroup_Member = new List<CIntermediateTransverseSupport>();
            List<CSegment_LTB> LTB_segment_group_Member = new List<CSegment_LTB>();
            CreateIrregularTransverseSupportGroupAndLTBsegmentGroup(bUseFlyBracing, iFlyBracing_Every_XXSupportingMember, fBottomGirtPosition, fDist_Girt, ref member, out lTransverseSupportGroup_Member, out LTB_segment_group_Member);

            // Reverse ILS and LTB segments
            List<CIntermediateTransverseSupport> lTransverseSupportGroup_Member_Reverse = new List<CIntermediateTransverseSupport>();
            List<CSegment_LTB> LTB_segment_group_Member_Reverse = new List<CSegment_LTB>();

            if (lTransverseSupportGroup_Member != null && lTransverseSupportGroup_Member.Count != 0) // List is initialized and some intermediate supports exists
            {
                for (int j = 0; j < lTransverseSupportGroup_Member.Count; j++)
                {
                    int iSupportIndex = lTransverseSupportGroup_Member.Count - j - 1;
                    CIntermediateTransverseSupport lTransverseSupport = new CIntermediateTransverseSupport(j + 1, lTransverseSupportGroup_Member[iSupportIndex].Type, 1f - lTransverseSupportGroup_Member[iSupportIndex].Fx_position_rel, fmemberLength - lTransverseSupportGroup_Member[iSupportIndex].Fx_position_abs);
                    lTransverseSupportGroup_Member_Reverse.Add(lTransverseSupport);
                }

                LTB_segment_group_Member_Reverse = GenerateIntermediateLTBSegmentsOnMember(lTransverseSupportGroup_Member_Reverse, false, fmemberLength);

                member.IntermediateTransverseSupportGroup = lTransverseSupportGroup_Member_Reverse;
                member.LTBSegmentGroup = LTB_segment_group_Member_Reverse;
            }
        }



        private void CreateJoints(bool bGenerateGirts, bool bUseMainColumnFlyBracingPlates, bool bGeneratePurlins, bool bUseRafterFlyBracingPlates, 
            bool bGenerateFrontColumns, bool bGenerateBackColumns, bool bGenerateFrontGirts, bool bGenerateBackGirts)
        {
            // Connection Joints
            m_arrConnectionJoints = new List<CConnectionJointTypes>();

            // Frame Main Column Joints to Foundation
            for (int i = 0; i < iFrameNo; i++)
            {
                m_arrConnectionJoints.Add(new CConnectionJoint_TA01(m_arrNodes[i * iFrameNodesNo + 0], m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * (iFrameNodesNo - 1) + 0], true));
                m_arrConnectionJoints.Add(new CConnectionJoint_TA01(m_arrNodes[i * iFrameNodesNo + 4], m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * (iFrameNodesNo - 1) + 3], true));
            }

            float ft_rafter_joint_plate = 0.003f; // m

            // Frame Rafter Joints
            for (int i = 0; i < iFrameNo; i++)
            {
                if (i == 0 || i == (iFrameNo - 1)) // Front or Last Frame
                    m_arrConnectionJoints.Add(new CConnectionJoint_A001(m_arrNodes[i * 5 + 2], m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * 4 + 1], m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * 4 + 2], fRoofPitch_rad, 2 * (float)m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * 4 + 1].CrScStart.h, ft_rafter_joint_plate, i == 0 ? fFrontFrameRakeAngle_deg : fBackFrameRakeAngle_deg, true));
                else //if(i< (iFrameNo - 1) // Intermediate frame
                    m_arrConnectionJoints.Add(new CConnectionJoint_A001(m_arrNodes[i * 5 + 2], m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * 4 + 1], m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * 4 + 2], fRoofPitch_rad, 2 * (float)m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * 4 + 1].CrScStart.h, ft_rafter_joint_plate, 0, true));
            }

            float ft_knee_joint_plate = 0.003f; // m

            // Knee Joints 1
            for (int i = 0; i < iFrameNo; i++)
            {
                if (i == 0 || i == (iFrameNo - 1)) // Front or Last Frame
                    m_arrConnectionJoints.Add(new CConnectionJoint_B001(m_arrNodes[i * 5 + 1], m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * 4], m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * 4 + 1], fRoofPitch_rad, 1.1f * (float)m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * 4].CrScStart.h, 2 * (float)m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * 4 + 1].CrScStart.h, ft_knee_joint_plate, ft_rafter_joint_plate, i == 0 ? fFrontFrameRakeAngle_deg : fBackFrameRakeAngle_deg, true));
                else //if(i< (iFrameNo - 1) // Intermediate frame
                    m_arrConnectionJoints.Add(new CConnectionJoint_B001(m_arrNodes[i * 5 + 1], m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * 4], m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * 4 + 1], fRoofPitch_rad, 1.1f * (float)m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * 4].CrScStart.h, 2 * (float)m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * 4 + 1].CrScStart.h, ft_knee_joint_plate, ft_rafter_joint_plate, i == 0 ? fFrontFrameRakeAngle_deg : fBackFrameRakeAngle_deg, true));
            }

            // Knee Joints 2
            for (int i = 0; i < iFrameNo; i++)
            {
                if (i == 0 || i == (iFrameNo - 1)) // Front or Last Frame
                    m_arrConnectionJoints.Add(new CConnectionJoint_B001(m_arrNodes[i * 5 + 3], m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * 4 + 3], m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * 4 + 2], fRoofPitch_rad, 1.1f * (float)m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * 4 + 3].CrScStart.h, 2 * (float)m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * 4 + 2].CrScStart.h, ft_knee_joint_plate, ft_rafter_joint_plate, i == 0 ? fFrontFrameRakeAngle_deg : fBackFrameRakeAngle_deg, true));
                else //if(i< (iFrameNo - 1) // Intermediate frame
                    m_arrConnectionJoints.Add(new CConnectionJoint_B001(m_arrNodes[i * 5 + 3], m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * 4 + 3], m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * 4 + 2], fRoofPitch_rad, 1.1f * (float)m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * 4 + 3].CrScStart.h, 2 * (float)m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * 4 + 2].CrScStart.h, ft_knee_joint_plate, ft_rafter_joint_plate, i == 0 ? fFrontFrameRakeAngle_deg : fBackFrameRakeAngle_deg, true));
            }

            // Eaves Purlin Joints
            if (iEavesPurlinNo > 0)
            {
                for (int i = 0; i < iEavesPurlinNo / iEavesPurlinNoInOneFrame; i++)
                {
                    CMember current_member = m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * (iFrameNodesNo - 1) + 4];
                    //m_arrConnectionJoints.Add(new CConnectionJoint_C001(current_member.NodeStart, m_arrMembers[0], current_member, true));
                    //m_arrConnectionJoints.Add(new CConnectionJoint_C001(current_member.NodeEnd, m_arrMembers[0], current_member, true));
                    m_arrConnectionJoints.Add(new CConnectionJoint_T001("LH", current_member.NodeStart, m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * (iFrameNodesNo - 1) + 0], current_member, ft_knee_joint_plate, EPlateNumberAndPositionInJoint.eOneRightPlate, true, true));
                    m_arrConnectionJoints.Add(new CConnectionJoint_T001("LH", current_member.NodeEnd, m_arrMembers[(i * iEavesPurlinNoInOneFrame) + (i + 1) * (iFrameNodesNo - 1) + 0], current_member, ft_knee_joint_plate, EPlateNumberAndPositionInJoint.eOneRightPlate, true, true));

                    current_member = m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * (iFrameNodesNo - 1) + 5];
                    m_arrConnectionJoints.Add(new CConnectionJoint_T001("LH", current_member.NodeStart, m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * (iFrameNodesNo - 1) + 3], current_member, ft_knee_joint_plate, EPlateNumberAndPositionInJoint.eOneRightPlate, true, true));
                    m_arrConnectionJoints.Add(new CConnectionJoint_T001("LH", current_member.NodeEnd, m_arrMembers[(i * iEavesPurlinNoInOneFrame) + (i + 1) * (iFrameNodesNo - 1) + 3], current_member, ft_knee_joint_plate, EPlateNumberAndPositionInJoint.eOneRightPlate, true, true));
                }
            }

            // Girt Joints
            if (bGenerateGirts)
            {
                for (int i = 0; i < (iFrameNo - 1) * iGirtNoInOneFrame; i++)
                {
                    CMember current_member = m_arrMembers[iMainColumnNo + iRafterNo + iEavesPurlinNo + i];
                    //m_arrConnectionJoints.Add(new CConnectionJoint_D001(current_member.NodeStart, m_arrMembers[0], current_member, true));
                    //m_arrConnectionJoints.Add(new CConnectionJoint_D001(current_member.NodeEnd, m_arrMembers[0], current_member, true));
                    m_arrConnectionJoints.Add(new CConnectionJoint_T001("LH", current_member.NodeStart, m_arrMembers[0], current_member, 0, EPlateNumberAndPositionInJoint.eTwoPlates, true, true));
                    m_arrConnectionJoints.Add(new CConnectionJoint_T001("LH", current_member.NodeEnd, m_arrMembers[0], current_member, 0, EPlateNumberAndPositionInJoint.eTwoPlates, true, true));

                    int iCurrentFrameIndex = i / iGirtNoInOneFrame;
                    int iFirstGirtInFrameLeftSide = iMainColumnNo + iRafterNo + iEavesPurlinNo + iCurrentFrameIndex * iGirtNoInOneFrame;
                    int iFirstGirtInFrameRightSide = iFirstGirtInFrameLeftSide + iGirtNoInOneFrame / 2;
                    int iCurrentMemberIndex = iMainColumnNo + iRafterNo + iEavesPurlinNo + i;

                    int iFirstGirtOnCurrentSideIndex;
                    if (iCurrentMemberIndex < iFirstGirtInFrameRightSide)
                        iFirstGirtOnCurrentSideIndex = iFirstGirtInFrameLeftSide;
                    else
                        iFirstGirtOnCurrentSideIndex = iFirstGirtInFrameRightSide;

                    if (bUseMainColumnFlyBracingPlates && iMainColumnFlyBracing_EveryXXGirt > 0 && (iCurrentMemberIndex - iFirstGirtOnCurrentSideIndex + 1) % iMainColumnFlyBracing_EveryXXGirt == 0)
                    {
                        m_arrConnectionJoints.Add(new CConnectionJoint_T003("FB", current_member.NodeStart, m_arrMembers[0], current_member, 0, EPlateNumberAndPositionInJoint.eTwoPlates, true, true));
                        m_arrConnectionJoints.Add(new CConnectionJoint_T003("FB", current_member.NodeEnd, m_arrMembers[0], current_member, 0, EPlateNumberAndPositionInJoint.eTwoPlates, true, true));
                    }
                    else
                    {
                        m_arrConnectionJoints.Add(new CConnectionJoint_T001("LH", current_member.NodeStart, m_arrMembers[0], current_member, 0, EPlateNumberAndPositionInJoint.eTwoPlates, true, true));
                        m_arrConnectionJoints.Add(new CConnectionJoint_T001("LH", current_member.NodeEnd, m_arrMembers[0], current_member, 0, EPlateNumberAndPositionInJoint.eTwoPlates, true, true));
                    }
                }
            }

            // Purlin Joints
            if (bGeneratePurlins)
            {
                for (int i = 0; i < (iFrameNo - 1) * iPurlinNoInOneFrame; i++)
                {
                    CMember current_member = m_arrMembers[iMainColumnNo + iRafterNo + iEavesPurlinNo + (iFrameNo - 1) * iGirtNoInOneFrame + i];
                    //m_arrConnectionJoints.Add(new CConnectionJoint_E001(current_member.NodeStart, m_arrMembers[1], current_member, true));
                    //m_arrConnectionJoints.Add(new CConnectionJoint_E001(current_member.NodeEnd, m_arrMembers[1], current_member, true));

                    int iCurrentFrameIndex = i / iPurlinNoInOneFrame;
                    int iFirstPurlinInFrameLeftSide = iMainColumnNo + iRafterNo + iEavesPurlinNo + (iFrameNo - 1) * iGirtNoInOneFrame + iCurrentFrameIndex * iPurlinNoInOneFrame;
                    int iFirstPurlinInFrameRightSide = iFirstPurlinInFrameLeftSide + iPurlinNoInOneFrame / 2;
                    int iCurrentMemberIndex = iMainColumnNo + iRafterNo + iEavesPurlinNo + (iFrameNo - 1) * iGirtNoInOneFrame + i;

                    int iFirstPurlinOnCurrentSideIndex;
                    if (iCurrentMemberIndex < iFirstPurlinInFrameRightSide)
                        iFirstPurlinOnCurrentSideIndex = iFirstPurlinInFrameLeftSide;
                    else
                        iFirstPurlinOnCurrentSideIndex = iFirstPurlinInFrameRightSide;

                    if (bUseRafterFlyBracingPlates && iRafterFlyBracing_EveryXXPurlin > 0 && (iCurrentMemberIndex - iFirstPurlinOnCurrentSideIndex + 1) % iRafterFlyBracing_EveryXXPurlin == 0)
                    {
                        m_arrConnectionJoints.Add(new CConnectionJoint_T003("FB", current_member.NodeStart, m_arrMembers[1], current_member, ft_knee_joint_plate, EPlateNumberAndPositionInJoint.eTwoPlates, true, true));
                        m_arrConnectionJoints.Add(new CConnectionJoint_T003("FB", current_member.NodeEnd, m_arrMembers[1], current_member, ft_knee_joint_plate, EPlateNumberAndPositionInJoint.eTwoPlates, true, true));
                    }
                    else
                    {
                        m_arrConnectionJoints.Add(new CConnectionJoint_T001("LH", current_member.NodeStart, m_arrMembers[1], current_member, ft_knee_joint_plate, EPlateNumberAndPositionInJoint.eTwoPlates, true, true));
                        m_arrConnectionJoints.Add(new CConnectionJoint_T001("LH", current_member.NodeEnd, m_arrMembers[1], current_member, ft_knee_joint_plate, EPlateNumberAndPositionInJoint.eTwoPlates, true, true));
                    }
                }
            }

            // Front Columns Foundation Joints / Top Joint to the rafter
            if (bGenerateFrontColumns)
            {
                for (int i = 0; i < iFrontColumnNoInOneFrame; i++)
                {
                    CMember current_member = m_arrMembers[iMainColumnNo + iRafterNo + iEavesPurlinNo + (iFrameNo - 1) * iGirtNoInOneFrame + (iFrameNo - 1) * iPurlinNoInOneFrame + i];
                    m_arrConnectionJoints.Add(new CConnectionJoint_TB01(current_member.NodeStart, current_member, true));

                    if (i < (int)(iFrontColumnNoInOneFrame / 2))
                        m_arrConnectionJoints.Add(new CConnectionJoint_S001(current_member.NodeEnd, m_arrMembers[1], current_member, true, true)); // Front Left Main Rafter (0 to 0.5*W)
                    else
                        m_arrConnectionJoints.Add(new CConnectionJoint_S001(current_member.NodeEnd, m_arrMembers[2], current_member, true, true)); // Front Right Main Rafter(0.5*W to W)
                }
            }

            // Back Columns Foundation Joints / Top Joint to the rafter
            if (bGenerateBackColumns)
            {
                for (int i = 0; i < iBackColumnNoInOneFrame; i++)
                {
                    CMember current_member = m_arrMembers[iMainColumnNo + iRafterNo + iEavesPurlinNo + (iFrameNo - 1) * iGirtNoInOneFrame + (iFrameNo - 1) * iPurlinNoInOneFrame + iFrontColumnNoInOneFrame + i];
                    m_arrConnectionJoints.Add(new CConnectionJoint_TB01(current_member.NodeStart, current_member, true));

                    if (i < (int)(iBackColumnNoInOneFrame / 2))
                        m_arrConnectionJoints.Add(new CConnectionJoint_S001(current_member.NodeEnd, m_arrMembers[(iFrameNo - 1) * 6 + 1], current_member, false, true)); // Back Left Main Rafter (0 to 0.5*W)
                    else
                        m_arrConnectionJoints.Add(new CConnectionJoint_S001(current_member.NodeEnd, m_arrMembers[(iFrameNo - 1) * 6 + 2], current_member, false, true)); // Back Right Main Rafter(0.5*W to W)
                }
            }

            // Front Girt Joints
            if (bGenerateFrontGirts)
            {
                for (int i = 0; i < iFrontGirtsNoInOneFrame; i++)
                {
                    CMember current_member = m_arrMembers[iMainColumnNo + iRafterNo + iEavesPurlinNo + (iFrameNo - 1) * iGirtNoInOneFrame + (iFrameNo - 1) * iPurlinNoInOneFrame + iFrontColumnNoInOneFrame + iBackColumnNoInOneFrame + i];
                    //m_arrConnectionJoints.Add(new CConnectionJoint_J001(current_member.NodeStart, m_arrMembers[iMainColumnNo + iRafterNo + iEavesPurlinNo + (iFrameNo - 1) * iGirtNoInOneFrame + (iFrameNo - 1) * iPurlinNoInOneFrame], current_member, true));
                    //m_arrConnectionJoints.Add(new CConnectionJoint_J001(current_member.NodeEnd, m_arrMembers[iMainColumnNo + iRafterNo + iEavesPurlinNo + (iFrameNo - 1) * iGirtNoInOneFrame + (iFrameNo - 1) * iPurlinNoInOneFrame], current_member, true));

                    // Number of girts in the middle
                    int iNumberOfGirtsInMiddle = iArrNumberOfNodesPerFrontColumn[iOneRafterFrontColumnNo - 1];
                    // Number of girts without middle
                    int iNumberOfSymmetricalGirts = iFrontGirtsNoInOneFrame - iNumberOfGirtsInMiddle;
                    // Number of girts in the half of frame (without middle)
                    int iNumberOfSymmetricalGirtsHalf = iNumberOfSymmetricalGirts / 2;

                    // Joint at member start - connected to the first main column
                    if (i < iGirtNoInOneFrame / 2) // First column of girts are connected to the first main column
                    {
                        m_arrConnectionJoints.Add(new CConnectionJoint_T001("LH", current_member.NodeStart, m_arrMembers[0], current_member, 0, EPlateNumberAndPositionInJoint.eTwoPlates, false, true)); // Use height (z dimension)
                    }
                    else if ((iNumberOfSymmetricalGirtsHalf + iNumberOfGirtsInMiddle - 1) < i && i < (iNumberOfSymmetricalGirtsHalf + iNumberOfGirtsInMiddle + iOneColumnGirtNo)) // Joint at member start - connected to the second main column
                    {
                        m_arrConnectionJoints.Add(new CConnectionJoint_T001("LH", current_member.NodeStart, m_arrMembers[3], current_member, 0, EPlateNumberAndPositionInJoint.eTwoPlates, false, true)); // Use height (z dimension)
                    }
                    else
                    {
                        m_arrConnectionJoints.Add(new CConnectionJoint_T001("LH", current_member.NodeStart, m_arrMembers[iMainColumnNo + iRafterNo + iEavesPurlinNo + (iFrameNo - 1) * iGirtNoInOneFrame + (iFrameNo - 1) * iPurlinNoInOneFrame], current_member, 0, EPlateNumberAndPositionInJoint.eTwoPlates, true, true));
                    }

                    // Joint at member end
                    m_arrConnectionJoints.Add(new CConnectionJoint_T001("LH", current_member.NodeEnd, m_arrMembers[iMainColumnNo + iRafterNo + iEavesPurlinNo + (iFrameNo - 1) * iGirtNoInOneFrame + (iFrameNo - 1) * iPurlinNoInOneFrame], current_member, 0, EPlateNumberAndPositionInJoint.eTwoPlates, true, true));
                }
            }

            // Back Girt Joints
            if (bGenerateBackGirts)
            {
                for (int i = 0; i < iBackGirtsNoInOneFrame; i++)
                {
                    CMember current_member = m_arrMembers[iMainColumnNo + iRafterNo + iEavesPurlinNo + (iFrameNo - 1) * iGirtNoInOneFrame + (iFrameNo - 1) * iPurlinNoInOneFrame + iFrontColumnNoInOneFrame + iBackColumnNoInOneFrame + iFrontGirtsNoInOneFrame + i];
                    //m_arrConnectionJoints.Add(new CConnectionJoint_L001(current_member.NodeStart, m_arrMembers[iMainColumnNo + iRafterNo + iEavesPurlinNo + (iFrameNo - 1) * iGirtNoInOneFrame + (iFrameNo - 1) * iPurlinNoInOneFrame + iFrontColumnNoInOneFrame], current_member, true));
                    //m_arrConnectionJoints.Add(new CConnectionJoint_L001(current_member.NodeEnd, m_arrMembers[iMainColumnNo + iRafterNo + iEavesPurlinNo + (iFrameNo - 1) * iGirtNoInOneFrame + (iFrameNo - 1) * iPurlinNoInOneFrame + iFrontColumnNoInOneFrame], current_member, true));

                    // Number of girts in the middle
                    int iNumberOfGirtsInMiddle = iArrNumberOfNodesPerBackColumn[iOneRafterBackColumnNo - 1];
                    // Number of girts without middle
                    int iNumberOfSymmetricalGirts = iBackGirtsNoInOneFrame - iNumberOfGirtsInMiddle;
                    // Number of girts in the half of frame (without middle)
                    int iNumberOfSymmetricalGirtsHalf = iNumberOfSymmetricalGirts / 2;

                    // Joint at member start - connected to the first main column
                    if (i < iGirtNoInOneFrame / 2) // First column of girts are connected to the first main column
                    {
                        m_arrConnectionJoints.Add(new CConnectionJoint_T001("LH", current_member.NodeStart, m_arrMembers[((iFrameNo - 1) * iEavesPurlinNoInOneFrame) + (iFrameNo - 1) * (iFrameNodesNo - 1)], current_member, 0, EPlateNumberAndPositionInJoint.eTwoPlates, false, true)); // Use height (z dimension)
                    }
                    else if ((iNumberOfSymmetricalGirtsHalf + iNumberOfGirtsInMiddle - 1) < i && i < (iNumberOfSymmetricalGirtsHalf + iNumberOfGirtsInMiddle + iOneColumnGirtNo)) // Joint at member start - connected to the second main column
                    {
                        m_arrConnectionJoints.Add(new CConnectionJoint_T001("LH", current_member.NodeStart, m_arrMembers[((iFrameNo - 1) * iEavesPurlinNoInOneFrame) + (iFrameNo - 1) * (iFrameNodesNo - 1) + 3], current_member, 0, EPlateNumberAndPositionInJoint.eTwoPlates, false, true)); // Use height (z dimension)
                    }
                    else
                    {
                        m_arrConnectionJoints.Add(new CConnectionJoint_T001("LH", current_member.NodeStart, m_arrMembers[iMainColumnNo + iRafterNo + iEavesPurlinNo + (iFrameNo - 1) * iGirtNoInOneFrame + (iFrameNo - 1) * iPurlinNoInOneFrame + iFrontColumnNoInOneFrame], current_member, 0, EPlateNumberAndPositionInJoint.eTwoPlates, true, true));
                    }

                    m_arrConnectionJoints.Add(new CConnectionJoint_T001("LH", current_member.NodeEnd, m_arrMembers[iMainColumnNo + iRafterNo + iEavesPurlinNo + (iFrameNo - 1) * iGirtNoInOneFrame + (iFrameNo - 1) * iPurlinNoInOneFrame + iFrontColumnNoInOneFrame], current_member, 0, EPlateNumberAndPositionInJoint.eTwoPlates, true, true));
                }
            }

            // Validation - check that all created joints have assigned Main Member
            // Check all joints before definition of doors and windows members and joints
            for (int i = 0; i < m_arrConnectionJoints.Count; i++)
            {
                if (m_arrConnectionJoints[i].m_MainMember == null)
                    throw new ArgumentNullException("Main member is not assigned to the joint No.:" + m_arrConnectionJoints[i].ID.ToString() + " Joint index in the list: " + i);
            }

            // Validation - duplicity of node ID
            for (int i = 0; i < m_arrNodes.Length; i++)
            {
                for (int j = 0; j < m_arrNodes.Length; j++)
                {
                    if ((m_arrNodes[i] != m_arrNodes[j]) && (m_arrNodes[i].ID == m_arrNodes[j].ID))
                        throw new ArgumentNullException("Duplicity in Node ID.\nNode index: " + i + " and Node index: " + j);
                }
            }
        }
    }
}
