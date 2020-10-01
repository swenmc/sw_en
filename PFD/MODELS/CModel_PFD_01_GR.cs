using BaseClasses;
using CRSC;
using BaseClasses.Helpers;
using M_EC1.AS_NZS;
using MATERIAL;
using MATH;
using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Linq;
using System.Collections.ObjectModel;

namespace PFD
{
    [Serializable]
    public class CModel_PFD_01_GR : CModel_PFD
    {
        private int iOneColumnGirtNo;
        private CComponentListVM _clVM;
        private CPFDViewModel _pfdVM;

        public CModel_PFD_01_GR
        (
                BuildingGeometryDataInput sGeometryInputData,
                CComponentListVM componentListVM,
                List<CConnectionJointTypes> joints,
                List<CFoundation> foundations,
                List<CSlab> slabs,
                CPFDViewModel vm
        )
        {
            eKitset = EModelType_FS.eKitsetGableRoofEnclosed;
            _clVM = componentListVM;
            _pfdVM = vm;
            ObservableCollection<CComponentInfo> componentList = componentListVM?.ComponentList;
            fH1_frame = sGeometryInputData.fH_1;
            fW_frame = sGeometryInputData.fW;
            fL_tot = sGeometryInputData.fL;
            iFrameNo = vm.Frames;
            fH2_frame = sGeometryInputData.fH_2;
            fFrontFrameRakeAngle_deg = vm.FrontFrameRakeAngle;
            fBackFrameRakeAngle_deg = vm.BackFrameRakeAngle;

            iFrameNodesNo = 5;
            iFrameMembersNo = iFrameNodesNo - 1;
            iEavesPurlinNoInOneFrame = 2;

            //fL1_frame = fL_tot / (iFrameNo - 1);
            L1_Bays = vm._baysWidthOptionsVM.GetBaysWidths();

            fDist_Girt = vm.GirtDistance;
            fDist_Purlin = vm.PurlinDistance;
            fDist_FrontColumns = vm.ColumnDistance;
            fDist_BackColumns = fDist_FrontColumns; // TODO - docasne, nezadavame zatial rozne vzdialenosti medzi wind post na prednej a zadnej strane

            fBottomGirtPosition = vm.BottomGirtPosition;
            fDist_FrontGirts = vm.GirtDistance; // Ak nie je rovnake ako pozdlzne tak su koncove pruty sikmo pretoze sa uvazuje jeden uzol na stlpe pre pozdlzny aj priecny smer nosnikov
            fDist_BackGirts = vm.GirtDistance;
            fFrontFrameRakeAngle_temp_rad = fFrontFrameRakeAngle_deg * MathF.fPI / 180f;
            fBackFrameRakeAngle_temp_rad = fBackFrameRakeAngle_deg * MathF.fPI / 180f;

            DoorBlocksProperties = vm.DoorBlocksProperties;

            m_eSLN = ESLN.e3DD_1D; // 1D members in 3D model
            m_eNDOF = (int)ENDOF.e3DEnv; // DOF in 3D
            m_eGCS = EGCS.eGCSLeftHanded; // Global coordinate system

            fRoofPitch_rad = (float)Math.Atan((fH2_frame - fH1_frame) / (0.5f * fW_frame));

            iEavesPurlinNo = iEavesPurlinNoInOneFrame * (iFrameNo - 1);
            iMainColumnNo = iFrameNo * 2;
            iRafterNo = iFrameNo * 2;

            iOneColumnGirtNo = 0;
            iGirtNoInOneFrame = 0;

            m_arrMat = new CMat[17];
            m_arrCrSc = new CCrSc[17];

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
            m_arrMat[(int)EMemberGroupNames.eFrontWindPost] = MaterialFactory.GetMaterial(componentList[(int)EMemberGroupNames.eFrontWindPost].Material);
            m_arrMat[(int)EMemberGroupNames.eBackWindPost] = MaterialFactory.GetMaterial(componentList[(int)EMemberGroupNames.eBackWindPost].Material);
            m_arrMat[(int)EMemberGroupNames.eFrontGirt] = MaterialFactory.GetMaterial(componentList[(int)EMemberGroupNames.eFrontGirt].Material);
            m_arrMat[(int)EMemberGroupNames.eBackGirt] = MaterialFactory.GetMaterial(componentList[(int)EMemberGroupNames.eBackGirt].Material);
            m_arrMat[(int)EMemberGroupNames.eGirtBracing] = MaterialFactory.GetMaterial(componentList[(int)EMemberGroupNames.eGirtBracing].Material);
            m_arrMat[(int)EMemberGroupNames.ePurlinBracing] = MaterialFactory.GetMaterial(componentList[(int)EMemberGroupNames.ePurlinBracing].Material);
            m_arrMat[(int)EMemberGroupNames.eFrontGirtBracing] = MaterialFactory.GetMaterial(componentList[(int)EMemberGroupNames.eFrontGirtBracing].Material);
            m_arrMat[(int)EMemberGroupNames.eBackGirtBracing] = MaterialFactory.GetMaterial(componentList[(int)EMemberGroupNames.eBackGirtBracing].Material);
            m_arrMat[(int)EMemberGroupNames.eCrossBracing_Walls] = MaterialFactory.GetMaterial(componentList[(int)EMemberGroupNames.eCrossBracing_Walls].Material);
            m_arrMat[(int)EMemberGroupNames.eCrossBracing_Roof] = MaterialFactory.GetMaterial(componentList[(int)EMemberGroupNames.eCrossBracing_Roof].Material);

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
            m_arrCrSc[(int)EMemberGroupNames.eFrontWindPost] = CrScFactory.GetCrSc(componentList[(int)EMemberGroupNames.eFrontWindPost].Section);
            m_arrCrSc[(int)EMemberGroupNames.eBackWindPost] = CrScFactory.GetCrSc(componentList[(int)EMemberGroupNames.eBackWindPost].Section);
            m_arrCrSc[(int)EMemberGroupNames.eFrontGirt] = CrScFactory.GetCrSc(componentList[(int)EMemberGroupNames.eFrontGirt].Section);
            m_arrCrSc[(int)EMemberGroupNames.eBackGirt] = CrScFactory.GetCrSc(componentList[(int)EMemberGroupNames.eBackGirt].Section);
            m_arrCrSc[(int)EMemberGroupNames.eGirtBracing] = CrScFactory.GetCrSc(componentList[(int)EMemberGroupNames.eGirtBracing].Section);
            m_arrCrSc[(int)EMemberGroupNames.ePurlinBracing] = CrScFactory.GetCrSc(componentList[(int)EMemberGroupNames.ePurlinBracing].Section);
            m_arrCrSc[(int)EMemberGroupNames.eFrontGirtBracing] = CrScFactory.GetCrSc(componentList[(int)EMemberGroupNames.eFrontGirtBracing].Section);
            m_arrCrSc[(int)EMemberGroupNames.eBackGirtBracing] = CrScFactory.GetCrSc(componentList[(int)EMemberGroupNames.eBackGirtBracing].Section);
            m_arrCrSc[(int)EMemberGroupNames.eCrossBracing_Walls] = CrScFactory.GetCrSc(componentList[(int)EMemberGroupNames.eCrossBracing_Walls].Section);
            m_arrCrSc[(int)EMemberGroupNames.eCrossBracing_Roof] = CrScFactory.GetCrSc(componentList[(int)EMemberGroupNames.eCrossBracing_Roof].Section);

            for (int i = 0; i < m_arrCrSc.Length; i++)
            {
                m_arrCrSc[i].ID = i + 1;
            }

            m_arrCrSc[(int)EMemberGroupNames.eMainColumn].CSColor = Colors.Chocolate;       //  1 Main Column
            m_arrCrSc[(int)EMemberGroupNames.eRafter].CSColor = Colors.Green;               //  2 Main Rafter
            m_arrCrSc[(int)EMemberGroupNames.eMainColumn_EF].CSColor = Colors.DarkOrchid;   //  3 Main Column - Edge Frame
            m_arrCrSc[(int)EMemberGroupNames.eRafter_EF].CSColor = Colors.GreenYellow;      //  4 Main Rafter - Edge Frame
            m_arrCrSc[(int)EMemberGroupNames.eEavesPurlin].CSColor = Colors.DarkCyan;       //  5 Eaves Purlin
            m_arrCrSc[(int)EMemberGroupNames.eGirtWall].CSColor = Colors.Orange;            //  6 Girt - Wall
            m_arrCrSc[(int)EMemberGroupNames.ePurlin].CSColor = Colors.SlateBlue;           //  7 Purlin
            m_arrCrSc[(int)EMemberGroupNames.eFrontWindPost].CSColor = Colors.BlueViolet;   //  8 Front Wind Post
            m_arrCrSc[(int)EMemberGroupNames.eBackWindPost].CSColor = Colors.BlueViolet;    //  9 Back Wind Post
            m_arrCrSc[(int)EMemberGroupNames.eFrontGirt].CSColor = Colors.Brown;            // 10 Front Girt
            m_arrCrSc[(int)EMemberGroupNames.eBackGirt].CSColor = Colors.YellowGreen;       // 11 Back Girt
            m_arrCrSc[(int)EMemberGroupNames.eGirtBracing].CSColor = Colors.Orange;         // 12 Girt Bracing
            m_arrCrSc[(int)EMemberGroupNames.ePurlinBracing].CSColor = Colors.DarkOrange;   // 13 Purlin Bracing
            m_arrCrSc[(int)EMemberGroupNames.eFrontGirtBracing].CSColor = Colors.LimeGreen; // 14 Girt Bracing - Front Side
            m_arrCrSc[(int)EMemberGroupNames.eBackGirtBracing].CSColor = Colors.LightSeaGreen; // 15 Girt Bracing - Back Side
            m_arrCrSc[(int)EMemberGroupNames.eCrossBracing_Walls].CSColor = Colors.Olive;         // 16 Cross Bracing - Walls
            m_arrCrSc[(int)EMemberGroupNames.eCrossBracing_Roof].CSColor = Colors.Olive;         // 17 Cross Bracing - Roof


            // Member Groups
            listOfModelMemberGroups = new List<CMemberGroup>(17);

            //CDatabaseComponents database_temp = new CDatabaseComponents(); // TODO - Ondrej - prerobit triedu na nacitanie z databazy
            // See UC component list

            // TODO - nastavovat v GUI - zaviest databazu pre rozne typy prutov a typy load combinations
            /*
            int iLimitFractionDenominator_PermanentLoad = 250;
            int iLimitFractionDenominator_Total = 150;

            int iLimitFractionDenominator_Total_FrameColumn = 150;
            int iLimitFractionDenominator_Total_FrameRafter = 250;
            */

            // TODO - doplnit potrebne vstupne hodnoty
            float fVerticalDisplacementLimitDenominator_Rafter_PL = vm._designOptionsVM.VerticalDisplacementLimitDenominator_Rafter_PL;
            float fVerticalDisplacementLimitDenominator_Rafter_IL = 150f;
            float fVerticalDisplacementLimitDenominator_Rafter_TL = vm._designOptionsVM.VerticalDisplacementLimitDenominator_Rafter_TL;
            float fHorizontalDisplacementLimitDenominator_Column_PL = 50f;
            float fHorizontalDisplacementLimitDenominator_Column_IL = 100f;
            float fHorizontalDisplacementLimitDenominator_Column_TL = vm._designOptionsVM.HorizontalDisplacementLimitDenominator_Column_TL;
            float fVerticalDisplacementLimitDenominator_Purlin_PL = vm._designOptionsVM.VerticalDisplacementLimitDenominator_Purlin_PL;
            float fVerticalDisplacementLimitDenominator_Purlin_IL = 100f;
            float fVerticalDisplacementLimitDenominator_Purlin_TL = vm._designOptionsVM.VerticalDisplacementLimitDenominator_Purlin_TL;
            float fHorizontalDisplacementLimitDenominator_Girt_PL = 50f;
            float fHorizontalDisplacementLimitDenominator_Girt_IL = 100f;
            float fHorizontalDisplacementLimitDenominator_Girt_TL = vm._designOptionsVM.HorizontalDisplacementLimitDenominator_Girt_TL;
            float fHorizontalDisplacementLimitDenominator_WindPost_PL = 50f;
            float fHorizontalDisplacementLimitDenominator_WindPost_IL = 100f;
            float fHorizontalDisplacementLimitDenominator_WindPost_TL = vm._designOptionsVM.HorizontalDisplacementLimitDenominator_Windpost_TL;

            float fVerticalDisplacementLimit_Rafter_PL = 1f / fVerticalDisplacementLimitDenominator_Rafter_PL;
            float fVerticalDisplacementLimit_Rafter_IL = 1f / fVerticalDisplacementLimitDenominator_Rafter_IL;
            float fVerticalDisplacementLimit_Rafter_TL = 1f / fVerticalDisplacementLimitDenominator_Rafter_TL;
            float fHorizontalDisplacementLimit_Column_PL = 1f / fHorizontalDisplacementLimitDenominator_Column_PL;
            float fHorizontalDisplacementLimit_Column_IL = 1f / fHorizontalDisplacementLimitDenominator_Column_IL;
            float fHorizontalDisplacementLimit_Column_TL = 1f / fHorizontalDisplacementLimitDenominator_Column_TL;
            float fVerticalDisplacementLimit_Purlin_PL = 1f / fVerticalDisplacementLimitDenominator_Purlin_PL;
            float fVerticalDisplacementLimit_Purlin_IL = 1f / fVerticalDisplacementLimitDenominator_Purlin_IL;
            float fVerticalDisplacementLimit_Purlin_TL = 1f / fVerticalDisplacementLimitDenominator_Purlin_TL;
            float fHorizontalDisplacementLimit_Girt_PL = 1f / fHorizontalDisplacementLimitDenominator_Girt_PL;
            float fHorizontalDisplacementLimit_Girt_IL = 1f / fHorizontalDisplacementLimitDenominator_Girt_IL;
            float fHorizontalDisplacementLimit_Girt_TL = 1f / fHorizontalDisplacementLimitDenominator_Girt_TL;
            float fHorizontalDisplacementLimit_WindPost_PL = 1f / fHorizontalDisplacementLimitDenominator_WindPost_PL;
            float fHorizontalDisplacementLimit_WindPost_IL = 1f / fHorizontalDisplacementLimitDenominator_WindPost_IL;
            float fHorizontalDisplacementLimit_WindPost_TL = 1f / fHorizontalDisplacementLimitDenominator_WindPost_TL;

            listOfModelMemberGroups.Add(new CMemberGroup(1, componentList[(int)EMemberGroupNames.eMainColumn].ComponentName, EMemberType_FS.eMC, EMemberType_FS_Position.MainColumn, m_arrCrSc[(int)EMemberGroupNames.eMainColumn], fHorizontalDisplacementLimitDenominator_Column_PL, fHorizontalDisplacementLimitDenominator_Column_IL, fHorizontalDisplacementLimitDenominator_Column_TL, 0));
            listOfModelMemberGroups.Add(new CMemberGroup(2, componentList[(int)EMemberGroupNames.eRafter].ComponentName, EMemberType_FS.eMR, EMemberType_FS_Position.MainRafter, m_arrCrSc[(int)EMemberGroupNames.eRafter], fVerticalDisplacementLimitDenominator_Rafter_PL, fVerticalDisplacementLimitDenominator_Rafter_IL, fVerticalDisplacementLimitDenominator_Rafter_TL, 0));
            listOfModelMemberGroups.Add(new CMemberGroup(3, componentList[(int)EMemberGroupNames.eMainColumn_EF].ComponentName, EMemberType_FS.eEC, EMemberType_FS_Position.EdgeColumn, m_arrCrSc[(int)EMemberGroupNames.eMainColumn_EF], fHorizontalDisplacementLimitDenominator_Column_PL, fHorizontalDisplacementLimitDenominator_Column_IL, fHorizontalDisplacementLimitDenominator_Column_TL, 0));
            listOfModelMemberGroups.Add(new CMemberGroup(4, componentList[(int)EMemberGroupNames.eRafter_EF].ComponentName, EMemberType_FS.eER, EMemberType_FS_Position.EdgeRafter, m_arrCrSc[(int)EMemberGroupNames.eRafter_EF], fVerticalDisplacementLimitDenominator_Rafter_PL, fVerticalDisplacementLimitDenominator_Rafter_IL, fVerticalDisplacementLimitDenominator_Rafter_TL, 0));
            listOfModelMemberGroups.Add(new CMemberGroup(5, componentList[(int)EMemberGroupNames.eEavesPurlin].ComponentName, EMemberType_FS.eEP, EMemberType_FS_Position.EdgePurlin, m_arrCrSc[(int)EMemberGroupNames.eEavesPurlin], fVerticalDisplacementLimitDenominator_Purlin_PL, fVerticalDisplacementLimitDenominator_Purlin_IL, fVerticalDisplacementLimitDenominator_Purlin_TL, 0));
            listOfModelMemberGroups.Add(new CMemberGroup(6, componentList[(int)EMemberGroupNames.eGirtWall].ComponentName, EMemberType_FS.eG, EMemberType_FS_Position.Girt, m_arrCrSc[(int)EMemberGroupNames.eGirtWall], fHorizontalDisplacementLimitDenominator_Girt_PL, fHorizontalDisplacementLimitDenominator_Girt_IL, fHorizontalDisplacementLimitDenominator_Girt_TL, 0));
            listOfModelMemberGroups.Add(new CMemberGroup(7, componentList[(int)EMemberGroupNames.ePurlin].ComponentName, EMemberType_FS.eP, EMemberType_FS_Position.Purlin, m_arrCrSc[(int)EMemberGroupNames.ePurlin], fVerticalDisplacementLimitDenominator_Purlin_PL, fVerticalDisplacementLimitDenominator_Purlin_IL, fVerticalDisplacementLimitDenominator_Purlin_TL, 0));
            listOfModelMemberGroups.Add(new CMemberGroup(8, componentList[(int)EMemberGroupNames.eFrontWindPost].ComponentName, EMemberType_FS.eWP, EMemberType_FS_Position.WindPostFrontSide, m_arrCrSc[(int)EMemberGroupNames.eFrontWindPost], fHorizontalDisplacementLimitDenominator_WindPost_PL, fHorizontalDisplacementLimitDenominator_WindPost_IL, fHorizontalDisplacementLimitDenominator_WindPost_TL, 0));
            listOfModelMemberGroups.Add(new CMemberGroup(9, componentList[(int)EMemberGroupNames.eBackWindPost].ComponentName, EMemberType_FS.eWP, EMemberType_FS_Position.WindPostBackSide, m_arrCrSc[(int)EMemberGroupNames.eBackWindPost], fHorizontalDisplacementLimitDenominator_WindPost_PL, fHorizontalDisplacementLimitDenominator_WindPost_IL, fHorizontalDisplacementLimitDenominator_WindPost_TL, 0));
            listOfModelMemberGroups.Add(new CMemberGroup(10, componentList[(int)EMemberGroupNames.eFrontGirt].ComponentName, EMemberType_FS.eG, EMemberType_FS_Position.GirtFrontSide, m_arrCrSc[(int)EMemberGroupNames.eFrontGirt], fHorizontalDisplacementLimitDenominator_Girt_PL, fHorizontalDisplacementLimitDenominator_Girt_IL, fHorizontalDisplacementLimitDenominator_Girt_TL, 0));
            listOfModelMemberGroups.Add(new CMemberGroup(11, componentList[(int)EMemberGroupNames.eBackGirt].ComponentName, EMemberType_FS.eG, EMemberType_FS_Position.GirtBackSide, m_arrCrSc[(int)EMemberGroupNames.eBackGirt], fHorizontalDisplacementLimitDenominator_Girt_PL, fHorizontalDisplacementLimitDenominator_Girt_IL, fHorizontalDisplacementLimitDenominator_Girt_TL, 0));
            listOfModelMemberGroups.Add(new CMemberGroup(12, componentList[(int)EMemberGroupNames.eGirtBracing].ComponentName, EMemberType_FS.eGB, EMemberType_FS_Position.BracingBlockGirts, m_arrCrSc[(int)EMemberGroupNames.eGirtBracing], 0, 0, 0, 0));
            listOfModelMemberGroups.Add(new CMemberGroup(13, componentList[(int)EMemberGroupNames.ePurlinBracing].ComponentName, EMemberType_FS.ePB, EMemberType_FS_Position.BracingBlockPurlins, m_arrCrSc[(int)EMemberGroupNames.ePurlinBracing], 0, 0, 0, 0));
            listOfModelMemberGroups.Add(new CMemberGroup(14, componentList[(int)EMemberGroupNames.eFrontGirtBracing].ComponentName, EMemberType_FS.eGB, EMemberType_FS_Position.BracingBlockGirtsFrontSide, m_arrCrSc[(int)EMemberGroupNames.eFrontGirtBracing], 0, 0, 0, 0));
            listOfModelMemberGroups.Add(new CMemberGroup(15, componentList[(int)EMemberGroupNames.eBackGirtBracing].ComponentName, EMemberType_FS.eGB, EMemberType_FS_Position.BracingBlockGirtsBackSide, m_arrCrSc[(int)EMemberGroupNames.eBackGirtBracing], 0, 0, 0, 0));
            listOfModelMemberGroups.Add(new CMemberGroup(16, componentList[(int)EMemberGroupNames.eCrossBracing_Walls].ComponentName, EMemberType_FS.eCB, EMemberType_FS_Position.CrossBracingWall, m_arrCrSc[(int)EMemberGroupNames.eCrossBracing_Walls], 0, 0, 0, 0));
            listOfModelMemberGroups.Add(new CMemberGroup(17, componentList[(int)EMemberGroupNames.eCrossBracing_Roof].ComponentName, EMemberType_FS.eCB, EMemberType_FS_Position.CrossBracingRoof, m_arrCrSc[(int)EMemberGroupNames.eCrossBracing_Roof], 0, 0, 0, 0));

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

            // alignments
            float falignment_column, falignment_knee_rafter, falignment_apex_rafter;
            GetJointalignments((float)m_arrCrSc[(int)EMemberGroupNames.eMainColumn].h, (float)m_arrCrSc[(int)EMemberGroupNames.eRafter].h, out falignment_column, out falignment_knee_rafter, out falignment_apex_rafter);

            // Member Eccentricities
            // Zadane hodnoty predpokladaju ze prierez je symetricky, je potrebne zobecnit
            CMemberEccentricity eccentricityPurlin = new CMemberEccentricity(0, (float)(0.5 * m_arrCrSc[(int)EMemberGroupNames.eRafter].h - 0.5 * m_arrCrSc[(int)EMemberGroupNames.ePurlin].h));
            CMemberEccentricity eccentricityGirtLeft_X0 = new CMemberEccentricity(0, (float)(-(0.5 * m_arrCrSc[(int)EMemberGroupNames.eMainColumn].h - 0.5 * m_arrCrSc[(int)EMemberGroupNames.eGirtWall].h)));
            CMemberEccentricity eccentricityGirtRight_XB = new CMemberEccentricity(0, (float)(0.5 * m_arrCrSc[(int)EMemberGroupNames.eMainColumn].h - 0.5 * m_arrCrSc[(int)EMemberGroupNames.eGirtWall].h));

            float feccentricityEavePurlin_y = (float)(0.5 * m_arrCrSc[(int)EMemberGroupNames.eMainColumn].h + m_arrCrSc[(int)EMemberGroupNames.eEavesPurlin].y_min);
            float feccentricityEavePurlin_z = -falignment_column + (float)m_arrCrSc[(int)EMemberGroupNames.eRafter].h * (float)Math.Cos(fRoofPitch_rad) - (float)m_arrCrSc[(int)EMemberGroupNames.eEavesPurlin].z_max;
            CMemberEccentricity eccentricityEavePurlin = new CMemberEccentricity(-feccentricityEavePurlin_y, feccentricityEavePurlin_z);

            // Moze byt automaticke alebo uzivatelsky nastavitelne
            //bWindPostEndUnderRafter = m_arrCrSc[(int)EMemberGroupNames.eRafter_EF].h > 0.49f ? true : false; // TODO - nastavovat podla velkosti edge frame rafter // true - stlp konci na spodnej hrane rafter, false - stlp konci na hornej hrane rafter

            if (vm._generalOptionsVM.WindPostUnderRafter)
            {
                eccentricityColumnFront_Z = new CMemberEccentricity(0, -(float)(m_arrCrSc[(int)EMemberGroupNames.eRafter_EF].y_min + m_arrCrSc[(int)EMemberGroupNames.eFrontWindPost].z_max));
                eccentricityColumnBack_Z = new CMemberEccentricity(0, -(float)(m_arrCrSc[(int)EMemberGroupNames.eRafter_EF].y_max + m_arrCrSc[(int)EMemberGroupNames.eBackWindPost].z_min));

                eccentricityGirtFront_Y0 = new CMemberEccentricity(0, eccentricityColumnFront_Z.MFz_local + (float)(m_arrCrSc[(int)EMemberGroupNames.eFrontWindPost].z_max - m_arrCrSc[(int)EMemberGroupNames.eFrontGirt].z_max));
                eccentricityGirtBack_YL = new CMemberEccentricity(0, eccentricityColumnBack_Z.MFz_local + (float)(m_arrCrSc[(int)EMemberGroupNames.eBackWindPost].z_min - m_arrCrSc[(int)EMemberGroupNames.eBackGirt].z_min));
            }
            else
            {
                eccentricityColumnFront_Z = new CMemberEccentricity(0, -(float)(m_arrCrSc[(int)EMemberGroupNames.eRafter_EF].y_max + m_arrCrSc[(int)EMemberGroupNames.eFrontWindPost].z_max));
                eccentricityColumnBack_Z = new CMemberEccentricity(0, -(float)(m_arrCrSc[(int)EMemberGroupNames.eRafter_EF].y_min + m_arrCrSc[(int)EMemberGroupNames.eBackWindPost].z_min));

                eccentricityGirtFront_Y0 = new CMemberEccentricity(0, eccentricityColumnFront_Z.MFz_local + (float)(m_arrCrSc[(int)EMemberGroupNames.eFrontWindPost].z_max - m_arrCrSc[(int)EMemberGroupNames.eFrontGirt].z_max + m_arrCrSc[(int)EMemberGroupNames.eRafter_EF].b));
                eccentricityGirtBack_YL = new CMemberEccentricity(0, eccentricityColumnBack_Z.MFz_local + (float)(m_arrCrSc[(int)EMemberGroupNames.eBackWindPost].z_min - m_arrCrSc[(int)EMemberGroupNames.eBackGirt].z_min - m_arrCrSc[(int)EMemberGroupNames.eRafter_EF].b));
            }

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
            {
                //task 600
                //to Mato netusim co tu ma byt
                //iMainColumnFlyBracing_EveryXXGirt = Math.Max(0, (int)(fL1_frame / fDist_Girt));
                iMainColumnFlyBracing_EveryXXGirt = Math.Max(0, (int)(L1_Bays[0] / fDist_Girt));
            }

            // Rafter fly bracing
            // Index of purlin 0 - no bracing 1 - every, 2 - every second purlin, 3 - every third purlin, ...
            // Poziciu fly bracing - kazda xx purlin nastavovat v GUI, alebo umoznit urcit automaticky, napr. cca tak aby bola vdialenost medzi fly bracing rovna L1

            bool bUseRafterFlyBracingPlates = true; // Use fly bracing plates in purlin to rafter joint

            if (bUseDefaultOrUserDefinedValueForFlyBracing)
                iRafterFlyBracing_EveryXXPurlin = sGeometryInputData.iRafterFlyBracingEveryXXPurlin;
            else
            {                
                iRafterFlyBracing_EveryXXPurlin = Math.Max(0, (int)(L1_Bays[0] / fDist_Purlin));
            }                

            // Front and Back Wind Post
            bool bUseFrontColumnFlyBracingPlates = true; // Use fly bracing plates in girt to column joint
            int iFrontColumnFlyBracing_EveryXXGirt = sGeometryInputData.iFrontWindPostFlyBracingEveryXXGirt;

            bool bUseBackColumnFlyBracingPlates = true; // Use fly bracing plates in girt to column joint
            int iBackColumnFlyBracing_EveryXXGirt = sGeometryInputData.iBackWindPostFlyBracingEveryXXGirt;

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

            componentListVM.SetColumnFlyBracingPosition_Items(iOneColumnGirtNo);  //zakomentovane 20.12.2019 - nechapem naco to tu je

            if (!bGenerateGirts || iMainColumnFlyBracing_EveryXXGirt == 0 || iMainColumnFlyBracing_EveryXXGirt > iGirtNoInOneFrame) // Index 0 means do not use fly bracing, more than number of girts per main column means no fly bracing too
                bUseMainColumnFlyBracingPlates = false;

            float fFirstGirtPosition = fBottomGirtPosition;
            float fFirstPurlinPosition = fDist_Purlin;
            float fRafterLength = MathF.Sqrt(MathF.Pow2(fH2_frame - fH1_frame) + MathF.Pow2(0.5f * fW_frame));

            iOneRafterPurlinNo = 0;
            iPurlinNoInOneFrame = 0;

            bool bGeneratePurlins = componentList[(int)EMemberGroupNames.ePurlin].Generate.Value;
            if (bGeneratePurlins)
            {
                iOneRafterPurlinNo = (int)((fRafterLength - fFirstPurlinPosition) / fDist_Purlin) + 1;
                iPurlinNoInOneFrame = 2 * iOneRafterPurlinNo;
            }
            componentListVM.SetRafterFlyBracingPosition_Items(iOneRafterPurlinNo); //zakomentovane 20.12.2019 - nechapem naco to tu je
            //vm._crossBracingOptionsVM.SetRoofPositions(componentListVM.RafterFlyBracingPosition_Items);

            if (!bGeneratePurlins || iRafterFlyBracing_EveryXXPurlin == 0 || iRafterFlyBracing_EveryXXPurlin > iPurlinNoInOneFrame) // Index 0 means do not use fly bracing, more than number of purlins per rafter means no fly bracing too
                bUseRafterFlyBracingPlates = false;

            iFrontColumnNoInOneFrame = 0;

            bool bGenerateFrontColumns = componentList[(int)EMemberGroupNames.eFrontWindPost].Generate.Value;
            if (bGenerateFrontColumns)
            {
                iOneRafterFrontColumnNo = (int)((0.5f * fW_frame - 0.45f * fDist_FrontColumns) / fDist_FrontColumns);
                iFrontColumnNoInOneFrame = 2 * iOneRafterFrontColumnNo;
                // Update value of distance between columns
                fDist_FrontColumns = (fW_frame / (iFrontColumnNoInOneFrame + 1));
            }

            const int iFrontColumnNodesNo = 2; // Number of Nodes for Front Wind Post
            int iFrontColumninOneRafterNodesNo = iFrontColumnNodesNo * iOneRafterFrontColumnNo; // Number of Nodes for Front Wind Posts under one Rafter
            int iFrontColumninOneFrameNodesNo = 2 * iFrontColumninOneRafterNodesNo; // Number of Nodes for Front Wind Posts under one Frame

            iBackColumnNoInOneFrame = 0;

            bool bGenerateBackColumns = componentList[(int)EMemberGroupNames.eBackWindPost].Generate.Value;
            if (bGenerateBackColumns)
            {
                iOneRafterBackColumnNo = (int)((0.5f * fW_frame - 0.45f * fDist_BackColumns) / fDist_BackColumns);
                iBackColumnNoInOneFrame = 2 * iOneRafterBackColumnNo;
                // Update value of distance between columns
                fDist_BackColumns = (fW_frame / (iBackColumnNoInOneFrame + 1));
            }

            const int iBackColumnNodesNo = 2; // Number of Nodes for Back Wind Post
            int iBackColumninOneRafterNodesNo = iBackColumnNodesNo * iOneRafterBackColumnNo; // Number of Nodes for Back Wind Posts under one Rafter
            int iBackColumninOneFrameNodesNo = 2 * iBackColumninOneRafterNodesNo; // Number of Nodes for Back Wind Posts under one Frame

            // Number of Nodes - Front Girts
            int iFrontIntermediateColumnNodesForGirtsOneRafterNo = 0;
            int iFrontIntermediateColumnNodesForGirtsOneFrameNo = 0;
            iFrontGirtsNoInOneFrame = 0;
            iArrNumberOfNodesPerFrontColumnFromLeft = new int[iOneRafterFrontColumnNo];

            bool bGenerateFrontGirts = componentList[(int)EMemberGroupNames.eFrontGirt].Generate.Value;

            if (bGenerateFrontGirts)
            {
                iFrontIntermediateColumnNodesForGirtsOneRafterNo = GetNumberofIntermediateNodesInColumnsForOneFrame(true, iOneRafterFrontColumnNo, fH1_frame, fBottomGirtPosition, fDist_FrontColumns, fz_UpperLimitForFrontGirts);
                iFrontIntermediateColumnNodesForGirtsOneFrameNo = 2 * iFrontIntermediateColumnNodesForGirtsOneRafterNo;

                // Number of Girts - Main Frame Column
                //iOneColumnGirtNo = (int)((fH1_frame - fUpperGirtLimit - fBottomGirtPosition) / fDist_Girt) + 1;

                //iFrontGirtsNoInOneFrame = iOneColumnGirtNo;
                iArrNumberOfGirtsPerFrontColumnFromLeft = new int[iFrontColumnNoInOneFrame + 1];
                iArrNumberOfGirtsPerFrontColumnFromLeft[0] = iOneColumnGirtNo;

                // Number of girts under one rafter at the frontside of building - middle girts are considered twice
                for (int i = 0; i < iOneRafterFrontColumnNo; i++)
                {
                    int temp = GetNumberofIntermediateNodesInOneColumnForGirts(true, fH1_frame, fBottomGirtPosition, fDist_FrontColumns, fz_UpperLimitForFrontGirts, i);
                    //iFrontGirtsNoInOneFrame += temp;
                    iArrNumberOfGirtsPerFrontColumnFromLeft[i + 1] = temp;
                    iArrNumberOfNodesPerFrontColumnFromLeft[i] = temp;
                }

                iFrontGirtsNoInOneFrame = iArrNumberOfGirtsPerFrontColumnFromLeft.Sum();
                iFrontGirtsNoInOneFrame *= 2;
                // Girts in the middle are considered twice - remove one set
                iFrontGirtsNoInOneFrame -= iArrNumberOfNodesPerFrontColumnFromLeft[iOneRafterFrontColumnNo - 1];
            }
            componentListVM.SetFrontColumnFlyBracingPosition_Items(iOneColumnGirtNo); //zakomentovane 20.12.2019 - nechapem naco to tu je

            if (!bGenerateFrontGirts || iFrontColumnFlyBracing_EveryXXGirt == 0) // Index 0 means do not use fly bracing
                bUseFrontColumnFlyBracingPlates = false;

            // Number of Nodes - Back Girts
            int iBackIntermediateColumnNodesForGirtsOneRafterNo = 0;
            int iBackIntermediateColumnNodesForGirtsOneFrameNo = 0;
            iBackGirtsNoInOneFrame = 0;
            iArrNumberOfNodesPerBackColumnFromLeft = new int[iOneRafterBackColumnNo];

            bool bGenerateBackGirts = componentList[(int)EMemberGroupNames.eBackGirt].Generate.Value;

            if (bGenerateBackGirts)
            {
                iBackIntermediateColumnNodesForGirtsOneRafterNo = GetNumberofIntermediateNodesInColumnsForOneFrame(true, iOneRafterBackColumnNo, fH1_frame, fBottomGirtPosition, fDist_BackColumns, fz_UpperLimitForBackGirts);
                iBackIntermediateColumnNodesForGirtsOneFrameNo = 2 * iBackIntermediateColumnNodesForGirtsOneRafterNo;

                // Number of Girts - Main Frame Column
                //iOneColumnGirtNo = (int)((fH1_frame - fUpperGirtLimit - fBottomGirtPosition) / fDist_Girt) + 1;

                //iBackGirtsNoInOneFrame = iOneColumnGirtNo;

                iArrNumberOfGirtsPerBackColumnFromLeft = new int[iBackColumnNoInOneFrame + 1];
                iArrNumberOfGirtsPerBackColumnFromLeft[0] = iOneColumnGirtNo;

                // Number of girts under one rafter at the frontside of building - middle girts are considered twice
                for (int i = 0; i < iOneRafterBackColumnNo; i++)
                {
                    int temp = GetNumberofIntermediateNodesInOneColumnForGirts(true, fH1_frame, fBottomGirtPosition, fDist_BackColumns, fz_UpperLimitForBackGirts, i);
                    //iBackGirtsNoInOneFrame += temp;
                    iArrNumberOfGirtsPerBackColumnFromLeft[i + 1] = temp;
                    iArrNumberOfNodesPerBackColumnFromLeft[i] = temp;
                }

                iBackGirtsNoInOneFrame = iArrNumberOfGirtsPerBackColumnFromLeft.Sum();
                iBackGirtsNoInOneFrame *= 2;
                // Girts in the middle are considered twice - remove one set
                iBackGirtsNoInOneFrame -= iArrNumberOfNodesPerBackColumnFromLeft[iOneRafterBackColumnNo - 1];
            }
            componentListVM.SetBackColumnFlyBracingPosition_Items(iOneColumnGirtNo); //zakomentovane 20.12.2019 - nechapem naco to tu je

            if (!bGenerateBackGirts || iBackColumnFlyBracing_EveryXXGirt == 0) // Index 0 means do not use fly bracing
                bUseBackColumnFlyBracingPlates = false;

            // Sidewall girts bracing blocks
            bool bGenerateGirtBracingSideWalls = componentList[(int)EMemberGroupNames.eGirtBracing].Generate.Value;

            int iNumberOfGBSideWallsNodesInOneBayOneSide = 0;
            int iNumberOfGBSideWallsNodesInOneBay = 0;
            int iGBSideWallsNodesNo = 0;

            int iNumberOfGBSideWallsMembersInOneBayOneSide = 0;
            int iNumberOfGBSideWallsMembersInOneBay = 0;
            
            // TODO 408 - Zapracovat toto nastavenie do GUI - prebrat s Ondrejom a dopracovat funkcionalitu tak ze sa budu generovat len bracing blocks na stenach 
            // alebo pre purlins v kazdom druhom rade (medzera medzi girts alebo purlins)

            bool bUseGBEverySecondGUI = vm._generalOptionsVM.BracingEverySecondRowOfGirts; 
            bool bUseGBEverySecond = bUseGBEverySecondGUI && (iOneColumnGirtNo % 2 != 0); // Nastavena hodnota je true a pocet bracing blocks na vysku steny je neparny

            if (bGenerateGirtBracingSideWalls)
            {
                iNumberOfGBSideWallsNodesInOneBayOneSide = iNumberOfTransverseSupports_Girts * (iOneColumnGirtNo + 1);
                iNumberOfGBSideWallsNodesInOneBay = 2 * iNumberOfGBSideWallsNodesInOneBayOneSide;
                iGBSideWallsNodesNo = iNumberOfGBSideWallsNodesInOneBay * (iFrameNo - 1);

                iNumberOfGBSideWallsMembersInOneBayOneSide = iNumberOfTransverseSupports_Girts * iOneColumnGirtNo;
                iNumberOfGBSideWallsMembersInOneBay = 2 * iNumberOfGBSideWallsMembersInOneBayOneSide;
                iGBSideWallsMembersNo = iNumberOfGBSideWallsMembersInOneBay * (iFrameNo - 1);
            }

            // Purlin bracing blocks
            bool bGeneratePurlinBracing = componentList[(int)EMemberGroupNames.ePurlinBracing].Generate.Value;

            int iNumberOfPBNodesInOneBayOneSide = 0;
            int iNumberOfPBNodesInOneBay = 0;
            int iPBNodesNo = 0;

            int iNumberOfPBMembersInOneBayOneSide = 0;
            int iNumberOfPBMembersInOneBay = 0;

            bool bUsePBEverySecondGUI = vm._generalOptionsVM.BracingEverySecondRowOfPurlins;
            bool bUsePBEverySecond = bUsePBEverySecondGUI && (iOneRafterPurlinNo % 2 != 0); // Nastavena hodnota je true a pocet bracing blocks na stranu strechy je neparny

            if (bGeneratePurlinBracing)
            {
                iNumberOfPBNodesInOneBayOneSide = iNumberOfTransverseSupports_Purlins * (iOneRafterPurlinNo + 1);
                iNumberOfPBNodesInOneBay = 2 * iNumberOfPBNodesInOneBayOneSide;
                iPBNodesNo = iNumberOfPBNodesInOneBay * (iFrameNo - 1);

                iNumberOfPBMembersInOneBayOneSide = iNumberOfTransverseSupports_Purlins * iOneRafterPurlinNo;
                iNumberOfPBMembersInOneBay = 2 * iNumberOfPBMembersInOneBayOneSide;
                iPBMembersNo = iNumberOfPBMembersInOneBay * (iFrameNo - 1);
            }

            // Front side girts bracing blocks
            bool bGenerateGirtBracingFrontSide = componentList[(int)EMemberGroupNames.eFrontGirtBracing].Generate.Value;

            int[] iArrGB_FS_NumberOfNodesPerBay = new int[iArrNumberOfNodesPerFrontColumnFromLeft.Length + 1];
            int[] iArrGB_FS_NumberOfNodesPerBayFirstNode = new int[iArrNumberOfNodesPerFrontColumnFromLeft.Length + 1];
            int[] iArrGB_FS_NumberOfMembersPerBay = new int[iArrNumberOfNodesPerFrontColumnFromLeft.Length + 1];
            int iNumberOfGB_FSNodesInOneFrame = 0;

            if (bGenerateGirtBracingFrontSide)
            {
                // First bay - pocet girts urcime podla poctu uzlov pre girts na edge / main column
                iArrGB_FS_NumberOfNodesPerBay[0] = (iOneColumnGirtNo + 1) * iNumberOfTransverseSupports_FrontGirts; // Pridame o jeden rad uzlov viac - nachadzaju sa na edge rafter
                iArrGB_FS_NumberOfNodesPerBayFirstNode[0] = (iOneColumnGirtNo + 1);
                iArrGB_FS_NumberOfMembersPerBay[0] = iOneColumnGirtNo * iNumberOfTransverseSupports_FrontGirts;

                iNumberOfGB_FSNodesInOneFrame = iArrGB_FS_NumberOfNodesPerBay[0];
                iNumberOfGB_FSMembersInOneFrame = iArrGB_FS_NumberOfMembersPerBay[0];

                for (int i = 0; i < iArrNumberOfNodesPerFrontColumnFromLeft.Length; i++)
                {
                    iArrGB_FS_NumberOfNodesPerBay[i+1] = (iArrNumberOfNodesPerFrontColumnFromLeft[i] + 1) * iNumberOfTransverseSupports_FrontGirts;
                    iArrGB_FS_NumberOfNodesPerBayFirstNode[i + 1] = iArrNumberOfNodesPerFrontColumnFromLeft[i] + 1;
                    iArrGB_FS_NumberOfMembersPerBay[i+1] = iArrNumberOfNodesPerFrontColumnFromLeft[i] * iNumberOfTransverseSupports_FrontGirts;

                    iNumberOfGB_FSNodesInOneFrame += iArrGB_FS_NumberOfNodesPerBay[i + 1];
                    iNumberOfGB_FSMembersInOneFrame += iArrGB_FS_NumberOfMembersPerBay[i + 1];
                }

                iNumberOfGB_FSNodesInOneFrame *= 2;
                iNumberOfGB_FSMembersInOneFrame *= 2;
                // Girt bracing block nodes / members in the middle are considered twice - remove one set

                iNumberOfGB_FSNodesInOneFrame -= iArrGB_FS_NumberOfNodesPerBay[iOneRafterFrontColumnNo];
                iNumberOfGB_FSMembersInOneFrame -= iArrGB_FS_NumberOfMembersPerBay[iOneRafterFrontColumnNo];
            }

            // Back side girts bracing blocks
            bool bGenerateGirtBracingBackSide = componentList[(int)EMemberGroupNames.eBackGirtBracing].Generate.Value;

            int[] iArrGB_BS_NumberOfNodesPerBay = new int[iArrNumberOfNodesPerBackColumnFromLeft.Length + 1];
            int[] iArrGB_BS_NumberOfNodesPerBayFirstNode = new int[iArrNumberOfNodesPerBackColumnFromLeft.Length + 1];
            int[] iArrGB_BS_NumberOfMembersPerBay = new int[iArrNumberOfNodesPerBackColumnFromLeft.Length + 1];
            int iNumberOfGB_BSNodesInOneFrame = 0;

            if (bGenerateGirtBracingBackSide)
            {
                // First bay - pocet girts urcime podla poctu uzlov pre girts na edge / main column
                iArrGB_BS_NumberOfNodesPerBay[0] = (iOneColumnGirtNo + 1) * iNumberOfTransverseSupports_BackGirts; // Pridame o jeden rad uzlov viac - nachadzaju sa na edge rafter
                iArrGB_BS_NumberOfNodesPerBayFirstNode[0] = (iOneColumnGirtNo + 1);
                iArrGB_BS_NumberOfMembersPerBay[0] = iOneColumnGirtNo * iNumberOfTransverseSupports_BackGirts;

                iNumberOfGB_BSNodesInOneFrame = iArrGB_BS_NumberOfNodesPerBay[0];
                iNumberOfGB_BSMembersInOneFrame = iArrGB_BS_NumberOfMembersPerBay[0];

                for (int i = 0; i < iArrNumberOfNodesPerBackColumnFromLeft.Length; i++)
                {
                    iArrGB_BS_NumberOfNodesPerBay[i + 1] = (iArrNumberOfNodesPerBackColumnFromLeft[i] + 1) * iNumberOfTransverseSupports_BackGirts;
                    iArrGB_BS_NumberOfNodesPerBayFirstNode[i + 1] = iArrNumberOfNodesPerBackColumnFromLeft[i] + 1;
                    iArrGB_BS_NumberOfMembersPerBay[i + 1] = iArrNumberOfNodesPerBackColumnFromLeft[i] * iNumberOfTransverseSupports_BackGirts;

                    iNumberOfGB_BSNodesInOneFrame += iArrGB_BS_NumberOfNodesPerBay[i + 1];
                    iNumberOfGB_BSMembersInOneFrame += iArrGB_BS_NumberOfMembersPerBay[i + 1];
                }

                iNumberOfGB_BSNodesInOneFrame *= 2;
                iNumberOfGB_BSMembersInOneFrame *= 2;
                // Girt bracing block nodes / members in the middle are considered twice - remove one set

                iNumberOfGB_BSNodesInOneFrame -= iArrGB_BS_NumberOfNodesPerBay[iOneRafterBackColumnNo];
                iNumberOfGB_BSMembersInOneFrame -= iArrGB_BS_NumberOfMembersPerBay[iOneRafterBackColumnNo];
            }

            //----------------------------------------------------------------------------------------------------------------------------
            // Cross-bracing

            bool bGenerateSideWallCrossBracing = componentList[(int)EMemberGroupNames.eCrossBracing_Walls].Generate.Value;
            bool bGenerateRoofCrossBracing = componentList[(int)EMemberGroupNames.eCrossBracing_Roof].Generate.Value;

            // Bug 596 (vyrobena metoda resetCounters)
            vm._crossBracingOptionsVM.ResetCounters();

            //Prva bay ma index 0
            if (bGenerateSideWallCrossBracing)
            {
                foreach(CCrossBracingInfo cb in vm._crossBracingOptionsVM.CrossBracingList)
                {
                    if(cb.WallLeft)
                    {
                        cb.NumberOfCrossBracingMembers_WallLeftSide = 2;
                        cb.NumberOfCrossBracingMembers_Walls += cb.NumberOfCrossBracingMembers_WallLeftSide;
                    }

                    if(cb.WallRight)
                    {
                        cb.NumberOfCrossBracingMembers_WallRightSide = 2;
                        cb.NumberOfCrossBracingMembers_Walls += cb.NumberOfCrossBracingMembers_WallRightSide;
                    }

                    cb.NumberOfCrossBracingMembers_Bay += cb.NumberOfCrossBracingMembers_Walls; // Celkovy pocet prutov cross bracing v Bay
                    iNumberOfCrossBracingMembers_Walls_Total += cb.NumberOfCrossBracingMembers_Walls; // Celkovy pocet prutov cross bracing pre valls v celom modeli
                }
            }

            if (bGenerateRoofCrossBracing)
            {
                foreach(CCrossBracingInfo cb in vm._crossBracingOptionsVM.CrossBracingList)
                {
                    if (cb.Roof)
                    {
                        // Index of purlin 0 - no bracing 1 - every, 2 - every second purlin, 3 - every third purlin, ...
                        //if (cb.EveryXXPurlin < 1) throw new ArgumentOutOfRangeException("Invalid index of purlin for cross-bracing. Index is " + cb.iEveryXXPurlin);
                        if (cb.EveryXXPurlin == 0) throw new ArgumentOutOfRangeException("Invalid count of purlins. Could not be NONE.");

                        cb.NumberOfCrossesPerRafter_Maximum = iOneRafterPurlinNo + 1;
                        cb.NumberOfCrossesPerRafter = cb.NumberOfCrossesPerRafter_Maximum / cb.EveryXXPurlin; // TODO - spocitat podla poctu purlins a nastavenia iRoofCrossBracingEveryXXPurlin
                        //to Mato - takto som myslel,ze vyriesim ten stav ze kazdu 10tu z 12 ale nie, lebo potom dava vsade dvojmo posledny cross
                        //cb.NumberOfCrossesPerRafter = (int)Math.Ceiling((decimal)cb.NumberOfCrossesPerRafter_Maximum / cb.EveryXXPurlin); // TODO - spocitat podla poctu purlins a nastavenia iRoofCrossBracingEveryXXPurlin

                        if ((cb.FirstCrossOnRafter && !cb.LastCrossOnRafter) || (!cb.FirstCrossOnRafter && cb.LastCrossOnRafter))
                            cb.NumberOfCrossesPerRafter = 1;
                        else if (cb.FirstCrossOnRafter && cb.LastCrossOnRafter)
                            cb.NumberOfCrossesPerRafter = 2;

                        // 2 pruty * 2 strany (gable roof !!!!) * pocet krizov na jeden rafter v danej bay
                        cb.NumberOfCrossBracingMembers_BayRoof = 2 * 2 * cb.NumberOfCrossesPerRafter; // Rozdiel oproti MR
                        cb.NumberOfCrossBracingMembers_Bay += cb.NumberOfCrossBracingMembers_BayRoof; // Celkovy pocet prutov cross bracing v Bay
                        iNumberOfCrossBracingMembers_Roof_Total += cb.NumberOfCrossBracingMembers_BayRoof; // Celkovy pocet prutov cross bracing pre roof v celom modeli // Rozne podla vstupu v GUI a ine pre gable roof a monopitch
                    }
                }
            }

            //----------------------------------------------------------------------------------------------------------------------------

            m_arrNodes = new CNode[iFrameNodesNo * iFrameNo + iFrameNo * iGirtNoInOneFrame + iFrameNo * iPurlinNoInOneFrame + iFrontColumninOneFrameNodesNo + iBackColumninOneFrameNodesNo + iFrontIntermediateColumnNodesForGirtsOneFrameNo + iBackIntermediateColumnNodesForGirtsOneFrameNo + iGBSideWallsNodesNo + iPBNodesNo + iNumberOfGB_FSNodesInOneFrame + iNumberOfGB_BSNodesInOneFrame];
            m_arrMembers = new CMember[iMainColumnNo + iRafterNo + iEavesPurlinNo + (iFrameNo - 1) * iGirtNoInOneFrame + (iFrameNo - 1) * iPurlinNoInOneFrame + iFrontColumnNoInOneFrame + iBackColumnNoInOneFrame + iFrontGirtsNoInOneFrame + iBackGirtsNoInOneFrame + iGBSideWallsMembersNo + iPBMembersNo + iNumberOfGB_FSMembersInOneFrame + iNumberOfGB_BSMembersInOneFrame + iNumberOfCrossBracingMembers_Walls_Total + iNumberOfCrossBracingMembers_Roof_Total];

            float fCutOffOneSide = 0.005f; // Cut 5 mm from each side of member

            // alignments (zaporna hodnota skracuje prut)
            float fMainColumnStart = 0.0f; // Dlzka orezu pruta stlpa na zaciatku (pri base plate) (zaporna hodnota skracuje prut)
            float fMainColumnEnd = -falignment_column - fCutOffOneSide; // Dlzka orezu pruta stlpa na konci (zaporna hodnota skracuje prut)
            float fRafterStart = falignment_knee_rafter - fCutOffOneSide;
            float fRafterEnd = -falignment_apex_rafter - fCutOffOneSide;                                                // Calculate according to h of rafter and roof pitch
            float fEavesPurlinStart = -(float)m_arrCrSc[(int)EMemberGroupNames.eRafter].y_max - fCutOffOneSide;
            float fEavesPurlinEnd = (float)m_arrCrSc[(int)EMemberGroupNames.eRafter].y_min - fCutOffOneSide;
            float fGirtStart = -(float)m_arrCrSc[(int)EMemberGroupNames.eMainColumn].y_max - fCutOffOneSide;
            float fGirtEnd = (float)m_arrCrSc[(int)EMemberGroupNames.eMainColumn].y_min - fCutOffOneSide;
            float fPurlinStart = -(float)m_arrCrSc[(int)EMemberGroupNames.eRafter].y_max - fCutOffOneSide;
            float fPurlinEnd = (float)m_arrCrSc[(int)EMemberGroupNames.eRafter].y_min - fCutOffOneSide;

            float fFrontColumnStart = 0.0f;
            float fFrontColumnEnd = (vm._generalOptionsVM.WindPostUnderRafter ? (float)m_arrCrSc[(int)EMemberGroupNames.eRafter_EF].z_min : (float)m_arrCrSc[(int)EMemberGroupNames.eRafter_EF].z_max) / (float)Math.Cos(fRoofPitch_rad) + (float)m_arrCrSc[(int)EMemberGroupNames.eFrontWindPost].y_min * (float)Math.Tan(fRoofPitch_rad) /*- fCutOffOneSide*/;
            float fBackColumnStart = 0.0f;
            float fBackColumnEnd = (vm._generalOptionsVM.WindPostUnderRafter ? (float)m_arrCrSc[(int)EMemberGroupNames.eRafter_EF].z_min : (float)m_arrCrSc[(int)EMemberGroupNames.eRafter_EF].z_max) / (float)Math.Cos(fRoofPitch_rad) + (float)m_arrCrSc[(int)EMemberGroupNames.eBackWindPost].y_min * (float)Math.Tan(fRoofPitch_rad) /*- fCutOffOneSide*/;

            float fFrontGirtStart = (float)m_arrCrSc[(int)EMemberGroupNames.eFrontWindPost].y_min - fCutOffOneSide;    // Just in case that cross-section of column is symmetric about z-z
            float fFrontGirtEnd = (float)m_arrCrSc[(int)EMemberGroupNames.eFrontWindPost].y_min - fCutOffOneSide;      // Just in case that cross-section of column is symmetric about z-z
            float fBackGirtStart = (float)m_arrCrSc[(int)EMemberGroupNames.eBackWindPost].y_min - fCutOffOneSide;      // Just in case that cross-section of column is symmetric about z-z
            float fBackGirtEnd = (float)m_arrCrSc[(int)EMemberGroupNames.eBackWindPost].y_min - fCutOffOneSide;        // Just in case that cross-section of column is symmetric about z-z
            float fFrontGirtStart_MC = (float)m_arrCrSc[(int)EMemberGroupNames.eMainColumn_EF].z_min - fCutOffOneSide;  // Connection to the main frame column (column symmetrical about y-y)
            float fFrontGirtEnd_MC = (float)m_arrCrSc[(int)EMemberGroupNames.eMainColumn_EF].z_min - fCutOffOneSide;    // Connection to the main frame column (column symmetrical about y-y)
            float fBackGirtStart_MC = (float)m_arrCrSc[(int)EMemberGroupNames.eMainColumn_EF].z_min - fCutOffOneSide;   // Connection to the main frame column (column symmetrical about y-y)
            float fBackGirtEnd_MC = (float)m_arrCrSc[(int)EMemberGroupNames.eMainColumn_EF].z_min - fCutOffOneSide;     // Connection to the main frame column (column symmetrical about y-y)

            float fGBSideWallStart = -(float)m_arrCrSc[(int)EMemberGroupNames.eGirtWall].y_max - fCutOffOneSide;
            float fGBSideWallEnd = (float)m_arrCrSc[(int)EMemberGroupNames.eGirtWall].y_min - fCutOffOneSide;

            float fGBFrontSideStart = -(float)m_arrCrSc[(int)EMemberGroupNames.eFrontGirt].y_max - fCutOffOneSide;
            float fGBFrontSideEnd = (float)m_arrCrSc[(int)EMemberGroupNames.eFrontGirt].y_min - fCutOffOneSide;

            float fGBBackSideStart = -(float)m_arrCrSc[(int)EMemberGroupNames.eBackGirt].y_max - fCutOffOneSide;
            float fGBBackSideEnd = (float)m_arrCrSc[(int)EMemberGroupNames.eBackGirt].y_min - fCutOffOneSide;

            float fColumnsRotation = MathF.fPI / 2.0f;
            float fGirtsRotation = MathF.fPI / 2.0f;

            listOfSupportedNodes_S1 = new List<CNode>();
            listOfSupportedNodes_S2 = new List<CNode>();
            // Nodes Automatic Generation
            // Nodes List - Nodes Array

            // Nodes - Frames
            for (int i = 0; i < iFrameNo; i++)
            {
                //m_arrNodes[i * iFrameNodesNo + 0] = new CNode(i * iFrameNodesNo + 1, 000000, i * fL1_frame, 00000, 0);
                m_arrNodes[i * iFrameNodesNo + 0] = new CNode(i * iFrameNodesNo + 1, 000000, GetBaysWidthUntilFrameIndex(i), 00000, 0);
                m_arrNodes[i * iFrameNodesNo + 0].Name = "Main Column Base Node - left";
                listOfSupportedNodes_S1.Add(m_arrNodes[i * iFrameNodesNo + 0]);
                RotateFrontOrBackFrameNodeAboutZ(m_arrNodes[i * iFrameNodesNo + 0]);

                //m_arrNodes[i * iFrameNodesNo + 1] = new CNode(i * iFrameNodesNo + 2, 000000, i * fL1_frame, fH1_frame, 0);
                m_arrNodes[i * iFrameNodesNo + 1] = new CNode(i * iFrameNodesNo + 2, 000000, GetBaysWidthUntilFrameIndex(i), fH1_frame, 0);
                m_arrNodes[i * iFrameNodesNo + 1].Name = "Main Column Top Node - left";
                RotateFrontOrBackFrameNodeAboutZ(m_arrNodes[i * iFrameNodesNo + 1]);

                //m_arrNodes[i * iFrameNodesNo + 2] = new CNode(i * iFrameNodesNo + 3, 0.5f * fW_frame, i * fL1_frame, fH2_frame, 0);
                m_arrNodes[i * iFrameNodesNo + 2] = new CNode(i * iFrameNodesNo + 3, 0.5f * fW_frame, GetBaysWidthUntilFrameIndex(i), fH2_frame, 0);
                m_arrNodes[i * iFrameNodesNo + 2].Name = "Apex Node";
                RotateFrontOrBackFrameNodeAboutZ(m_arrNodes[i * iFrameNodesNo + 2]);

                //m_arrNodes[i * iFrameNodesNo + 3] = new CNode(i * iFrameNodesNo + 4, fW_frame, i * fL1_frame, fH1_frame, 0);
                m_arrNodes[i * iFrameNodesNo + 3] = new CNode(i * iFrameNodesNo + 4, fW_frame, GetBaysWidthUntilFrameIndex(i), fH1_frame, 0);
                m_arrNodes[i * iFrameNodesNo + 3].Name = "Main Column Top Node - right";
                RotateFrontOrBackFrameNodeAboutZ(m_arrNodes[i * iFrameNodesNo + 3]);

                //m_arrNodes[i * iFrameNodesNo + 4] = new CNode(i * iFrameNodesNo + 5, fW_frame, i * fL1_frame, 00000, 0);
                m_arrNodes[i * iFrameNodesNo + 4] = new CNode(i * iFrameNodesNo + 5, fW_frame, GetBaysWidthUntilFrameIndex(i), 00000, 0);
                m_arrNodes[i * iFrameNodesNo + 4].Name = "Main Column Base Node - right";
                listOfSupportedNodes_S1.Add(m_arrNodes[i * iFrameNodesNo + 4]);
                RotateFrontOrBackFrameNodeAboutZ(m_arrNodes[i * iFrameNodesNo + 4]);
            }

            // Members
            for (int i = 0; i < iFrameNo; i++)
            {
                int iCrscColumnIndex = (int)EMemberGroupNames.eMainColumn;
                int iCrscRafterIndex = (int)EMemberGroupNames.eRafter;
                EMemberType_FS eColumnType = EMemberType_FS.eMC;
                EMemberType_FS eRafterType = EMemberType_FS.eMR;
                EMemberType_FS_Position eColumnType_Position = EMemberType_FS_Position.MainColumn;
                EMemberType_FS_Position eRafterType_Position = EMemberType_FS_Position.MainRafter;

                if (i == 0 || i == (iFrameNo - 1))
                {
                    iCrscColumnIndex = (int)EMemberGroupNames.eMainColumn_EF;
                    iCrscRafterIndex = (int)EMemberGroupNames.eRafter_EF;
                    eColumnType = EMemberType_FS.eEC;
                    eRafterType = EMemberType_FS.eER;
                    eColumnType_Position = EMemberType_FS_Position.EdgeColumn;
                    eRafterType_Position = EMemberType_FS_Position.EdgeRafter;
                }

                // Main Column
                m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * (iFrameNodesNo - 1) + 0] = new CMember((i * iEavesPurlinNoInOneFrame) + i * (iFrameNodesNo - 1) + 1, m_arrNodes[i * iFrameNodesNo + 0], m_arrNodes[i * iFrameNodesNo + 1], m_arrCrSc[iCrscColumnIndex], eColumnType, eColumnType_Position, null, null, fMainColumnStart, fMainColumnEnd, 0f, 0);
                CreateAndAssignIrregularTransverseSupportGroupAndLTBsegmentGroup(bUseMainColumnFlyBracingPlates, iMainColumnFlyBracing_EveryXXGirt, fBottomGirtPosition, fDist_Girt, ref m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * (iFrameNodesNo - 1) + 0]);

                // Rafters
                m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * (iFrameNodesNo - 1) + 1] = new CMember((i * iEavesPurlinNoInOneFrame) + i * (iFrameNodesNo - 1) + 2, m_arrNodes[i * iFrameNodesNo + 1], m_arrNodes[i * iFrameNodesNo + 2], m_arrCrSc[iCrscRafterIndex], eRafterType, eRafterType_Position, null, null, fRafterStart, fRafterEnd, 0f, 0);
                CreateAndAssignIrregularTransverseSupportGroupAndLTBsegmentGroup(bUseRafterFlyBracingPlates, iRafterFlyBracing_EveryXXPurlin, fFirstPurlinPosition, fDist_Purlin, ref m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * (iFrameNodesNo - 1) + 1]);

                m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * (iFrameNodesNo - 1) + 2] = new CMember((i * iEavesPurlinNoInOneFrame) + i * (iFrameNodesNo - 1) + 3, m_arrNodes[i * iFrameNodesNo + 2], m_arrNodes[i * iFrameNodesNo + 3], m_arrCrSc[iCrscRafterIndex], eRafterType, eRafterType_Position, null, null, fRafterEnd, fRafterStart, 0f, 0);
                // Reversed sequence of ILS
                CreateAndAssignReversedIrregularTransverseSupportGroupAndLTBsegmentGroup(bUseRafterFlyBracingPlates, iRafterFlyBracing_EveryXXPurlin, fFirstPurlinPosition, fDist_Purlin, ref m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * (iFrameNodesNo - 1) + 2]);

                // Main Column
                m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * (iFrameNodesNo - 1) + 3] = new CMember((i * iEavesPurlinNoInOneFrame) + i * (iFrameNodesNo - 1) + 4, m_arrNodes[i * iFrameNodesNo + 3], m_arrNodes[i * iFrameNodesNo + 4], m_arrCrSc[iCrscColumnIndex], eColumnType, eColumnType_Position, null, null, fMainColumnEnd, fMainColumnStart, 0f, 0);

                // Reversed sequence of ILS
                CreateAndAssignReversedIrregularTransverseSupportGroupAndLTBsegmentGroup(bUseMainColumnFlyBracingPlates, iMainColumnFlyBracing_EveryXXGirt, fBottomGirtPosition, fDist_Girt, ref m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * (iFrameNodesNo - 1) + 3]);

                // Eaves Purlins
                if (i < (iFrameNo - 1))
                {
                    // Left - osa z prierezu smeruje dole
                    CMemberEccentricity eccEavePurlinLeft = new CMemberEccentricity(eccentricityEavePurlin.MFy_local, -eccentricityEavePurlin.MFz_local);
                    m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * (iFrameNodesNo - 1) + iFrameMembersNo] = new CMember((i * iEavesPurlinNoInOneFrame) + i * (iFrameNodesNo - 1) + iFrameMembersNo + 1, m_arrNodes[i * iFrameNodesNo + 1], m_arrNodes[(i + 1) * iFrameNodesNo + 1], m_arrCrSc[(int)EMemberGroupNames.eEavesPurlin], EMemberType_FS.eEP, EMemberType_FS_Position.EdgePurlin, eccEavePurlinLeft, eccEavePurlinLeft, fEavesPurlinStart, fEavesPurlinEnd, (float)Math.PI, 0);

                    // Right - osa z prierezu smeruje hore
                    m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * (iFrameNodesNo - 1) + iFrameMembersNo + 1] = new CMember((i * iEavesPurlinNoInOneFrame) + i * (iFrameNodesNo - 1) + iFrameMembersNo + 1 + 1, m_arrNodes[i * iFrameNodesNo + 3], m_arrNodes[(i + 1) * iFrameNodesNo + 3], m_arrCrSc[(int)EMemberGroupNames.eEavesPurlin], EMemberType_FS.eEP, EMemberType_FS_Position.EdgePurlin, eccentricityEavePurlin, eccentricityEavePurlin, fEavesPurlinStart, fEavesPurlinEnd, 0f, 0);
                    CreateAndAssignRegularTransverseSupportGroupAndLTBsegmentGroup(m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * (iFrameNodesNo - 1) + iFrameMembersNo], iNumberOfTransverseSupports_EdgePurlins);
                    CreateAndAssignRegularTransverseSupportGroupAndLTBsegmentGroup(m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * (iFrameNodesNo - 1) + iFrameMembersNo + 1], iNumberOfTransverseSupports_EdgePurlins);
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
                        //task 600
                        //m_arrNodes[i_temp_numberofNodes + i * iGirtNoInOneFrame + j] = new CNode(i_temp_numberofNodes + i * iGirtNoInOneFrame + j + 1, 000000, i * fL1_frame, fBottomGirtPosition + j * fDist_Girt, 0);
                        m_arrNodes[i_temp_numberofNodes + i * iGirtNoInOneFrame + j] = new CNode(i_temp_numberofNodes + i * iGirtNoInOneFrame + j + 1, 000000, GetBaysWidthUntilFrameIndex(i), fBottomGirtPosition + j * fDist_Girt, 0);
                        RotateFrontOrBackFrameNodeAboutZ(m_arrNodes[i_temp_numberofNodes + i * iGirtNoInOneFrame + j]);
                    }

                    for (int j = 0; j < iOneColumnGirtNo; j++)
                    {
                        //m_arrNodes[i_temp_numberofNodes + i * iGirtNoInOneFrame + iOneColumnGirtNo + j] = new CNode(i_temp_numberofNodes + i * iGirtNoInOneFrame + iOneColumnGirtNo + j + 1, fW_frame, i * fL1_frame, fBottomGirtPosition + j * fDist_Girt, 0);
                        m_arrNodes[i_temp_numberofNodes + i * iGirtNoInOneFrame + iOneColumnGirtNo + j] = new CNode(i_temp_numberofNodes + i * iGirtNoInOneFrame + iOneColumnGirtNo + j + 1, fW_frame, GetBaysWidthUntilFrameIndex(i), fBottomGirtPosition + j * fDist_Girt, 0);
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
                        m_arrMembers[i_temp_numberofMembers + i * iGirtNoInOneFrame + j] = new CMember(i_temp_numberofMembers + i * iGirtNoInOneFrame + j + 1, m_arrNodes[i_temp_numberofNodes + i * iGirtNoInOneFrame + j], m_arrNodes[i_temp_numberofNodes + (i + 1) * iGirtNoInOneFrame + j], m_arrCrSc[(int)EMemberGroupNames.eGirtWall], EMemberType_FS.eG, EMemberType_FS_Position.Girt, eccentricityGirtLeft_X0, eccentricityGirtLeft_X0, fGirtStart, fGirtEnd, fGirtsRotation, 0);
                        RotateFrontOrBackFrameNodeAboutZ(m_arrNodes[i_temp_numberofMembers + i * iGirtNoInOneFrame + j]);
                        CreateAndAssignRegularTransverseSupportGroupAndLTBsegmentGroup(m_arrMembers[i_temp_numberofMembers + i * iGirtNoInOneFrame + j], iNumberOfTransverseSupports_Girts);
                    }

                    for (int j = 0; j < iOneColumnGirtNo; j++)
                    {
                        m_arrMembers[i_temp_numberofMembers + i * iGirtNoInOneFrame + iOneColumnGirtNo + j] = new CMember(i_temp_numberofMembers + i * iGirtNoInOneFrame + iOneColumnGirtNo + j + 1, m_arrNodes[i_temp_numberofNodes + i * iGirtNoInOneFrame + iOneColumnGirtNo + j], m_arrNodes[i_temp_numberofNodes + (i + 1) * iGirtNoInOneFrame + iOneColumnGirtNo + j], m_arrCrSc[(int)EMemberGroupNames.eGirtWall], EMemberType_FS.eG, EMemberType_FS_Position.Girt, eccentricityGirtRight_XB, eccentricityGirtRight_XB, fGirtStart, fGirtEnd, fGirtsRotation, 0);
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

                        //task 600
                        //m_arrNodes[i_temp_numberofNodes + i * iPurlinNoInOneFrame + j] = new CNode(i_temp_numberofNodes + i * iPurlinNoInOneFrame + j + 1, x_glob, i * fL1_frame, z_glob, 0);
                        m_arrNodes[i_temp_numberofNodes + i * iPurlinNoInOneFrame + j] = new CNode(i_temp_numberofNodes + i * iPurlinNoInOneFrame + j + 1, x_glob, GetBaysWidthUntilFrameIndex(i), z_glob, 0);
                        RotateFrontOrBackFrameNodeAboutZ(m_arrNodes[i_temp_numberofNodes + i * iPurlinNoInOneFrame + j]);
                    }

                    for (int j = 0; j < iOneRafterPurlinNo; j++)
                    {
                        float x_glob, z_glob;
                        CalcPurlinNodeCoord(fFirstPurlinPosition + j * fDist_Purlin, out x_glob, out z_glob);

                        //m_arrNodes[i_temp_numberofNodes + i * iPurlinNoInOneFrame + iOneRafterPurlinNo + j] = new CNode(i_temp_numberofNodes + i * iPurlinNoInOneFrame + iOneRafterPurlinNo + j + 1, fW_frame - x_glob, i * fL1_frame, z_glob, 0);
                        m_arrNodes[i_temp_numberofNodes + i * iPurlinNoInOneFrame + iOneRafterPurlinNo + j] = new CNode(i_temp_numberofNodes + i * iPurlinNoInOneFrame + iOneRafterPurlinNo + j + 1, fW_frame - x_glob, GetBaysWidthUntilFrameIndex(i), z_glob, 0);
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

                        m_arrMembers[i_temp_numberofMembers + i * iPurlinNoInOneFrame + j] = new CMember(i_temp_numberofMembers + i * iPurlinNoInOneFrame + j + 1, m_arrNodes[i_temp_numberofNodes + i * iPurlinNoInOneFrame + j], m_arrNodes[i_temp_numberofNodes + (i + 1) * iPurlinNoInOneFrame + j], m_arrCrSc[(int)EMemberGroupNames.ePurlin], EMemberType_FS.eP, EMemberType_FS_Position.Purlin, temp/*eccentricityPurlin*/, temp /*eccentricityPurlin*/, fPurlinStart, fPurlinEnd, fRotationAngle, 0);
                        CreateAndAssignRegularTransverseSupportGroupAndLTBsegmentGroup(m_arrMembers[i_temp_numberofMembers + i * iPurlinNoInOneFrame + j], iNumberOfTransverseSupports_Purlins);
                    }

                    for (int j = 0; j < iOneRafterPurlinNo; j++)
                    {
                        m_arrMembers[i_temp_numberofMembers + i * iPurlinNoInOneFrame + iOneRafterPurlinNo + j] = new CMember(i_temp_numberofMembers + i * iPurlinNoInOneFrame + iOneRafterPurlinNo + j + 1, m_arrNodes[i_temp_numberofNodes + i * iPurlinNoInOneFrame + iOneRafterPurlinNo + j], m_arrNodes[i_temp_numberofNodes + (i + 1) * iPurlinNoInOneFrame + iOneRafterPurlinNo + j], m_arrCrSc[(int)EMemberGroupNames.ePurlin], EMemberType_FS.eP, EMemberType_FS_Position.Purlin, eccentricityPurlin, eccentricityPurlin, fPurlinStart, fPurlinEnd, fRoofPitch_rad, 0);
                        CreateAndAssignRegularTransverseSupportGroupAndLTBsegmentGroup(m_arrMembers[i_temp_numberofMembers + i * iPurlinNoInOneFrame + iOneRafterPurlinNo + j], iNumberOfTransverseSupports_Purlins);
                    }
                }
            }

            // Front Wind Posts
            // Nodes - Front Wind Posts
            i_temp_numberofNodes += bGeneratePurlins ? (iPurlinNoInOneFrame * iFrameNo) : 0;
            if (bGenerateFrontColumns)
            {
                AddColumnsNodes(false, i_temp_numberofNodes, i_temp_numberofMembers, iOneRafterFrontColumnNo, iFrontColumnNoInOneFrame, fH1_frame, fDist_FrontColumns, 0);
            }

            // Members - Front Wind Posts
            i_temp_numberofMembers += bGeneratePurlins ? (iPurlinNoInOneFrame * (iFrameNo - 1)) : 0;
            if (bGenerateFrontColumns)
            {
                AddColumnsMembers(i_temp_numberofNodes, i_temp_numberofMembers, iOneRafterFrontColumnNo, iFrontColumnNoInOneFrame, eccentricityColumnFront_Z, fFrontColumnStart, fFrontColumnEnd, m_arrCrSc[(int)EMemberGroupNames.eFrontWindPost], fColumnsRotation, bUseFrontColumnFlyBracingPlates, iFrontColumnFlyBracing_EveryXXGirt, fBottomGirtPosition, fDist_FrontGirts);
            }

            // Back Wind Posts
            // Nodes - Back Wind Posts
            i_temp_numberofNodes += bGenerateFrontColumns ? iFrontColumninOneFrameNodesNo : 0;

            if (bGenerateBackColumns)
            {
                AddColumnsNodes(false, i_temp_numberofNodes, i_temp_numberofMembers, iOneRafterBackColumnNo, iBackColumnNoInOneFrame, fH1_frame, fDist_BackColumns, fL_tot);
            }

            // Members - Back Wind Posts
            i_temp_numberofMembers += bGenerateFrontColumns ? iFrontColumnNoInOneFrame : 0;
            if (bGenerateBackColumns)
            {
                AddColumnsMembers(i_temp_numberofNodes, i_temp_numberofMembers, iOneRafterBackColumnNo, iBackColumnNoInOneFrame, eccentricityColumnBack_Z, fBackColumnStart, fBackColumnEnd, m_arrCrSc[(int)EMemberGroupNames.eBackWindPost], fColumnsRotation, bUseBackColumnFlyBracingPlates, iBackColumnFlyBracing_EveryXXGirt, fBottomGirtPosition, fDist_BackGirts);
            }

            // Front Girts
            // Nodes - Front Girts
            i_temp_numberofNodes += bGenerateBackColumns ? iBackColumninOneFrameNodesNo : 0;
            float fIntermediateSupportSpacingGirtsFrontSide = fDist_FrontColumns / (iNumberOfTransverseSupports_FrontGirts + 1); // number of LTB segments = number of support + 1

            if (bGenerateFrontGirts)
            {
                AddFrontOrBackGirtsNodes(false, iOneRafterFrontColumnNo, iArrNumberOfNodesPerFrontColumnFromLeft, i_temp_numberofNodes, iFrontIntermediateColumnNodesForGirtsOneRafterNo, fH1_frame, fDist_FrontGirts, fDist_FrontColumns, 0);
            }

            // Front Girts
            // Members - Front Girts
            // TODO - doplnit riesenie pre maly rozpon ked neexistuju mezilahle stlpiky, prepojenie mezi hlavnymi stplmi ramu na celu sirku budovy
            // TODO - toto riesenie plati len ak existuju girts v pozdlznom smere, ak budu deaktivovane a nevytvoria sa uzly na stlpoch tak sa musia pruty na celnych stenach generovat uplne inak, musia sa vygenerovat aj uzly na stlpoch ....
            // TODO - pri vacsom sklone strechy (cca > 35 stupnov) by bolo dobre dogenerovat prvky ktore nie su na oboch stranach pripojene k stlpom ale su na jeden strane pripojene na stlp a na druhej strane na rafter, inak vznikaju prilis velke prazdne oblasti bez podpory (trojuhoniky) pod hlavnym ramom

            i_temp_numberofMembers += bGenerateBackColumns ? iBackColumnNoInOneFrame : 0;
            if (bGenerateFrontGirts)
            {
                AddFrontOrBackGirtsMembers(iFrameNodesNo, iOneRafterFrontColumnNo, iOneColumnGirtNo, iArrNumberOfNodesPerFrontColumnFromLeft, iArrNumberOfGirtsPerFrontColumnFromLeft, i_temp_numberofNodes, i_temp_numberofMembers, iFrontIntermediateColumnNodesForGirtsOneRafterNo, iFrontIntermediateColumnNodesForGirtsOneFrameNo, 0, fDist_Girt, eccentricityGirtFront_Y0, fFrontGirtStart_MC, fFrontGirtStart, fFrontGirtEnd, m_arrCrSc[(int)EMemberGroupNames.eFrontGirt], EMemberType_FS_Position.GirtFrontSide, fColumnsRotation, iNumberOfTransverseSupports_FrontGirts);
            }

            // Back Girts
            // Nodes - Back Girts

            i_temp_numberofNodes += bGenerateFrontGirts ? iFrontIntermediateColumnNodesForGirtsOneFrameNo : 0;
            float fIntermediateSupportSpacingGirtsBackSide = fDist_BackColumns / (iNumberOfTransverseSupports_BackGirts + 1); // number of LTB segments = number of support + 1

            if (bGenerateBackGirts)
            {
                AddFrontOrBackGirtsNodes(false, iOneRafterBackColumnNo, iArrNumberOfNodesPerBackColumnFromLeft, i_temp_numberofNodes, iBackIntermediateColumnNodesForGirtsOneRafterNo, fH1_frame, fDist_BackGirts, fDist_BackColumns, fL_tot);
            }

            // Back Girts
            // Members - Back Girts

            i_temp_numberofMembers += bGenerateFrontGirts ? iFrontGirtsNoInOneFrame : 0;
            if (bGenerateBackGirts)
            {
                AddFrontOrBackGirtsMembers(iFrameNodesNo, iOneRafterBackColumnNo, iOneColumnGirtNo, iArrNumberOfNodesPerBackColumnFromLeft, iArrNumberOfGirtsPerBackColumnFromLeft, i_temp_numberofNodes, i_temp_numberofMembers, iBackIntermediateColumnNodesForGirtsOneRafterNo, iBackIntermediateColumnNodesForGirtsOneFrameNo, iGirtNoInOneFrame * (iFrameNo - 1), fDist_Girt, eccentricityGirtBack_YL, fBackGirtStart_MC, fBackGirtStart, fBackGirtEnd, m_arrCrSc[(int)EMemberGroupNames.eBackGirt], EMemberType_FS_Position.GirtBackSide, fColumnsRotation, iNumberOfTransverseSupports_BackGirts);
            }

            // Girt Bracing - Side walls
            // Nodes - Girt Bracing - Side walls

            i_temp_numberofNodes += bGenerateBackGirts ? iBackIntermediateColumnNodesForGirtsOneFrameNo : 0;
            if (bGenerateGirtBracingSideWalls)
            {
                for (int i = 0; i < (iFrameNo-1); i++)
                {
                    float fIntermediateSupportSpacingGirts = L1_Bays[i] / (iNumberOfTransverseSupports_Girts + 1); // number of LTB segments = number of support + 1

                    for (int j = 0; j < (iOneColumnGirtNo + 1); j++) // Left side
                    {
                        float zCoord = j < iOneColumnGirtNo ? (fBottomGirtPosition + j * fDist_Girt) : fH1_frame;

                        for (int k = 0; k < iNumberOfTransverseSupports_Girts ; k++)
                        {
                            //task 600
                            //m_arrNodes[i_temp_numberofNodes + i * iNumberOfGBSideWallsNodesInOneBay + j * iNumberOfTransverseSupports_Girts + k] = new CNode(i_temp_numberofNodes + i * iNumberOfGBSideWallsNodesInOneBay + j * iNumberOfTransverseSupports_Girts + k + 1, 000000, i * fL1_frame + (k + 1) * fIntermediateSupportSpacingGirts, zCoord, 0);
                            m_arrNodes[i_temp_numberofNodes + i * iNumberOfGBSideWallsNodesInOneBay + j * iNumberOfTransverseSupports_Girts + k] = new CNode(i_temp_numberofNodes + i * iNumberOfGBSideWallsNodesInOneBay + j * iNumberOfTransverseSupports_Girts + k + 1, 000000, GetBaysWidthUntilFrameIndex(i) + (k + 1) * fIntermediateSupportSpacingGirts, zCoord, 0);
                        }
                    }

                    for (int j = 0; j < (iOneColumnGirtNo + 1); j++) // Right side
                    {
                        float zCoord = j < iOneColumnGirtNo ? (fBottomGirtPosition + j * fDist_Girt) : fH1_frame;

                        for (int k = 0; k < iNumberOfTransverseSupports_Girts; k++)
                        {
                            //task 600
                            //m_arrNodes[i_temp_numberofNodes + i * iNumberOfGBSideWallsNodesInOneBay + iNumberOfGBSideWallsNodesInOneBayOneSide + j * iNumberOfTransverseSupports_Girts + k] = new CNode(i_temp_numberofNodes + i * iNumberOfGBSideWallsNodesInOneBay + iNumberOfGBSideWallsNodesInOneBayOneSide + j * iNumberOfTransverseSupports_Girts + k + 1, fW_frame, i * fL1_frame + (k + 1) * fIntermediateSupportSpacingGirts, zCoord, 0);
                            m_arrNodes[i_temp_numberofNodes + i * iNumberOfGBSideWallsNodesInOneBay + iNumberOfGBSideWallsNodesInOneBayOneSide + j * iNumberOfTransverseSupports_Girts + k] = new CNode(i_temp_numberofNodes + i * iNumberOfGBSideWallsNodesInOneBay + iNumberOfGBSideWallsNodesInOneBayOneSide + j * iNumberOfTransverseSupports_Girts + k + 1, fW_frame, GetBaysWidthUntilFrameIndex(i) + (k + 1) * fIntermediateSupportSpacingGirts, zCoord, 0);
                        }
                    }
                }
            }

            // Members - Girt Bracing - Side walls

            i_temp_numberofMembers += bGenerateBackGirts ? iBackGirtsNoInOneFrame : 0;
            if (bGenerateGirtBracingSideWalls)
            {
                for (int i = 0; i < (iFrameNo - 1); i++)
                {
                    for (int j = 0; j < iOneColumnGirtNo; j++) // Left side
                    {
                        bool bDeactivateMember = false;
                        if (bUseGBEverySecond && j % 2 == 1) bDeactivateMember = true;

                        float fGBSideWallEnd_Current = fGBSideWallEnd;

                        if (j == iOneColumnGirtNo - 1) // Last
                            fGBSideWallEnd_Current = (float)m_arrCrSc[(int)EMemberGroupNames.eEavesPurlin].z_min + feccentricityEavePurlin_z - fCutOffOneSide;

                        for (int k = 0; k < iNumberOfTransverseSupports_Girts; k++)
                        {
                            int memberIndex = i_temp_numberofMembers + i * iNumberOfGBSideWallsMembersInOneBay + j * iNumberOfTransverseSupports_Girts + k;
                            int startNodeIndex = i_temp_numberofNodes + i * iNumberOfGBSideWallsNodesInOneBay + j * iNumberOfTransverseSupports_Girts + k;
                            int endNodeIndex = i_temp_numberofNodes + i * iNumberOfGBSideWallsNodesInOneBay + (j + 1) * iNumberOfTransverseSupports_Girts + k;
                            m_arrMembers[memberIndex] = new CMember(memberIndex + 1, m_arrNodes[startNodeIndex], m_arrNodes[endNodeIndex], m_arrCrSc[(int)EMemberGroupNames.eGirtBracing], EMemberType_FS.eGB, EMemberType_FS_Position.BracingBlockGirts, eccentricityGirtLeft_X0, eccentricityGirtLeft_X0, fGBSideWallStart, fGBSideWallEnd_Current, MathF.fPI, 0);

                            if (bDeactivateMember) DeactivateMemberAndItsJoints(ref m_arrMembers[memberIndex]);
                        }
                    }

                    for (int j = 0; j < iOneColumnGirtNo; j++) // Right side
                    {
                        bool bDeactivateMember = false;
                        if (bUseGBEverySecond && j % 2 == 1) bDeactivateMember = true;

                        float fGBSideWallEnd_Current = fGBSideWallEnd;

                        if (j == iOneColumnGirtNo - 1) // Last
                            fGBSideWallEnd_Current = (float)m_arrCrSc[(int)EMemberGroupNames.eEavesPurlin].z_min + feccentricityEavePurlin_z - fCutOffOneSide;

                        for (int k = 0; k < iNumberOfTransverseSupports_Girts; k++)
                        {
                            int memberIndex = i_temp_numberofMembers + i * iNumberOfGBSideWallsMembersInOneBay + iNumberOfGBSideWallsMembersInOneBayOneSide + j * iNumberOfTransverseSupports_Girts + k;
                            int startNodeIndex = i_temp_numberofNodes + i * iNumberOfGBSideWallsNodesInOneBay + iNumberOfGBSideWallsNodesInOneBayOneSide + j * iNumberOfTransverseSupports_Girts + k;
                            int endNodeIndex = i_temp_numberofNodes + i * iNumberOfGBSideWallsNodesInOneBay + +iNumberOfGBSideWallsNodesInOneBayOneSide + (j + 1) * iNumberOfTransverseSupports_Girts + k;
                            m_arrMembers[memberIndex] = new CMember(memberIndex + 1, m_arrNodes[startNodeIndex], m_arrNodes[endNodeIndex], m_arrCrSc[(int)EMemberGroupNames.eGirtBracing], EMemberType_FS.eGB, EMemberType_FS_Position.BracingBlockGirts, eccentricityGirtRight_XB, eccentricityGirtRight_XB, fGBSideWallStart, fGBSideWallEnd_Current, MathF.fPI, 0);

                            if(bDeactivateMember) DeactivateMemberAndItsJoints(ref m_arrMembers[memberIndex]);
                        }
                    }
                }
            }

            // Purlin Bracing
            // Nodes - Purlin Bracing

            i_temp_numberofNodes += bGenerateGirtBracingSideWalls ? iGBSideWallsNodesNo : 0;
            if (bGeneratePurlinBracing)
            {
                for (int i = 0; i < (iFrameNo - 1); i++)
                {
                    float fIntermediateSupportSpacingPurlins = L1_Bays[i] / (iNumberOfTransverseSupports_Purlins + 1); // number of LTB segments = number of support + 1                    

                    for (int j = 0; j < (iOneRafterPurlinNo + 1); j++) // Left side - eave purlin and purlins
                    {
                        float x_glob, z_glob;

                        if (j == 0) // First row of nodes
                        { x_glob = 0; z_glob = fH1_frame; } // Left edge of roof
                        else
                            CalcPurlinNodeCoord(fFirstPurlinPosition + (j - 1) * fDist_Purlin, out x_glob, out z_glob);

                        for (int k = 0; k < iNumberOfTransverseSupports_Purlins; k++)
                        {
                            //task 600
                            //m_arrNodes[i_temp_numberofNodes + i * iNumberOfPBNodesInOneBay + j * iNumberOfTransverseSupports_Purlins + k] = new CNode(i_temp_numberofNodes + i * iNumberOfPBNodesInOneBay + j * iNumberOfTransverseSupports_Purlins + k + 1, x_glob, i * fL1_frame + (k + 1) * fIntermediateSupportSpacingPurlins, z_glob, 0);
                            m_arrNodes[i_temp_numberofNodes + i * iNumberOfPBNodesInOneBay + j * iNumberOfTransverseSupports_Purlins + k] = new CNode(i_temp_numberofNodes + i * iNumberOfPBNodesInOneBay + j * iNumberOfTransverseSupports_Purlins + k + 1, x_glob, GetBaysWidthUntilFrameIndex(i) + (k + 1) * fIntermediateSupportSpacingPurlins, z_glob, 0);
                        }
                    }

                    for (int j = 0; j < (iOneRafterPurlinNo + 1); j++) // Right side - eave purlin and purlins
                    {
                        float x_glob, z_glob;

                        if (j == 0) // First row nodes
                        { x_glob = 0; z_glob = fH1_frame; } // Right edge of roof (x uvazujeme zprava)
                        else
                            CalcPurlinNodeCoord(fFirstPurlinPosition + (j - 1) * fDist_Purlin, out x_glob, out z_glob);

                        for (int k = 0; k < iNumberOfTransverseSupports_Purlins; k++)
                        {
                            //task 600
                            //m_arrNodes[i_temp_numberofNodes + i * iNumberOfPBNodesInOneBay + iNumberOfPBNodesInOneBayOneSide + j * iNumberOfTransverseSupports_Purlins + k] = new CNode(i_temp_numberofNodes + i * iNumberOfPBNodesInOneBay + iNumberOfPBNodesInOneBayOneSide + j * iNumberOfTransverseSupports_Purlins + k + 1, fW_frame - x_glob, i * fL1_frame + (k + 1) * fIntermediateSupportSpacingPurlins, z_glob, 0);
                            m_arrNodes[i_temp_numberofNodes + i * iNumberOfPBNodesInOneBay + iNumberOfPBNodesInOneBayOneSide + j * iNumberOfTransverseSupports_Purlins + k] = new CNode(i_temp_numberofNodes + i * iNumberOfPBNodesInOneBay + iNumberOfPBNodesInOneBayOneSide + j * iNumberOfTransverseSupports_Purlins + k + 1, fW_frame - x_glob, GetBaysWidthUntilFrameIndex(i) + (k + 1) * fIntermediateSupportSpacingPurlins, z_glob, 0);
                        }
                    }
                }
            }

            // Members - Purlin Bracing

            i_temp_numberofMembers += bGenerateGirtBracingSideWalls ? iGBSideWallsMembersNo : 0;

            if (bGeneratePurlinBracing)
            {
                for (int i = 0; i < (iFrameNo - 1); i++)
                {
                    for (int j = 0; j < iOneRafterPurlinNo; j++) // Left side
                    {
                        bool bDeactivateMember = false;
                        if (bUsePBEverySecond && j % 2 == 1) bDeactivateMember = true;

                        float fPBStart = (float)m_arrCrSc[(int)EMemberGroupNames.ePurlin].y_min - fCutOffOneSide;
                        float fPBEnd = -(float)m_arrCrSc[(int)EMemberGroupNames.ePurlin].y_max - fCutOffOneSide;

                        float fPBStart_Current = fPBStart;

                        if (j == 0) // First
                        {
                            // TODO - refaktorovat s monopitch
                            float b = (float)m_arrCrSc[(int)EMemberGroupNames.ePurlin].z_max * (float)Math.Tan(Math.Abs(fRoofPitch_rad));
                            float c = (float)m_arrCrSc[(int)EMemberGroupNames.eMainColumn].z_max / (float)Math.Cos(Math.Abs(fRoofPitch_rad));
                            float d = (float)m_arrCrSc[(int)EMemberGroupNames.eEavesPurlin].b * (float)Math.Cos(Math.Abs(fRoofPitch_rad));
                            float e = (float)m_arrCrSc[(int)EMemberGroupNames.eRafter].z_max - (float)m_arrCrSc[(int)EMemberGroupNames.ePurlin].z_max;
                            float f = e * (float)Math.Tan(Math.Abs(fRoofPitch_rad));
                            fPBStart_Current = c - b - d - f - fCutOffOneSide;
                        }

                        for (int k = 0; k < iNumberOfTransverseSupports_Purlins; k++)
                        {
                            int memberIndex = i_temp_numberofMembers + i * iNumberOfPBMembersInOneBay + j * iNumberOfTransverseSupports_Purlins + k;
                            int startNodeIndex = i_temp_numberofNodes + i * iNumberOfPBNodesInOneBay + j * iNumberOfTransverseSupports_Purlins + k;
                            int endNodeIndex = i_temp_numberofNodes + i * iNumberOfPBNodesInOneBay + (j + 1) * iNumberOfTransverseSupports_Purlins + k;
                            m_arrMembers[memberIndex] = new CMember(memberIndex + 1, m_arrNodes[startNodeIndex], m_arrNodes[endNodeIndex], m_arrCrSc[(int)EMemberGroupNames.ePurlinBracing], EMemberType_FS.ePB, EMemberType_FS_Position.BracingBlockPurlins, eccentricityPurlin, eccentricityPurlin, fPBStart_Current, fPBEnd, 0, 0);

                            if (bDeactivateMember) DeactivateMemberAndItsJoints(ref m_arrMembers[memberIndex]);
                        }
                    }

                    for (int j = 0; j < iOneRafterPurlinNo; j++) // Right side
                    {
                        bool bDeactivateMember = false;
                        if (bUsePBEverySecond && j % 2 == 1) bDeactivateMember = true;

                        // Opacna orientacia osi LCS y na pravej strane
                        float fPBStart = -(float)m_arrCrSc[(int)EMemberGroupNames.ePurlin].y_max - fCutOffOneSide;
                        float fPBEnd = (float)m_arrCrSc[(int)EMemberGroupNames.ePurlin].y_min - fCutOffOneSide;

                        float fPBStart_Current = fPBStart;

                        if (j == 0) // First
                        {
                            // TODO - refaktorovat s monopitch
                            float b = (float)m_arrCrSc[(int)EMemberGroupNames.ePurlin].z_max * (float)Math.Tan(Math.Abs(fRoofPitch_rad));
                            float c = (float)m_arrCrSc[(int)EMemberGroupNames.eMainColumn].z_max / (float)Math.Cos(Math.Abs(fRoofPitch_rad));
                            float d = (float)m_arrCrSc[(int)EMemberGroupNames.eEavesPurlin].b * (float)Math.Cos(Math.Abs(fRoofPitch_rad));
                            float e = (float)m_arrCrSc[(int)EMemberGroupNames.eRafter].z_max - (float)m_arrCrSc[(int)EMemberGroupNames.ePurlin].z_max;
                            float f = e * (float)Math.Tan(Math.Abs(fRoofPitch_rad));
                            fPBStart_Current = c - b - d - f - fCutOffOneSide;
                        }

                        for (int k = 0; k < iNumberOfTransverseSupports_Purlins; k++)
                        {
                            int memberIndex = i_temp_numberofMembers + i * iNumberOfPBMembersInOneBay + iNumberOfPBMembersInOneBayOneSide + j * iNumberOfTransverseSupports_Purlins + k;
                            int startNodeIndex = i_temp_numberofNodes + i * iNumberOfPBNodesInOneBay + iNumberOfPBNodesInOneBayOneSide + j * iNumberOfTransverseSupports_Purlins + k;
                            int endNodeIndex = i_temp_numberofNodes + i * iNumberOfPBNodesInOneBay + +iNumberOfPBNodesInOneBayOneSide + (j + 1) * iNumberOfTransverseSupports_Purlins + k;
                            m_arrMembers[memberIndex] = new CMember(memberIndex + 1, m_arrNodes[startNodeIndex], m_arrNodes[endNodeIndex], m_arrCrSc[(int)EMemberGroupNames.ePurlinBracing], EMemberType_FS.ePB, EMemberType_FS_Position.BracingBlockPurlins, eccentricityPurlin, eccentricityPurlin, fPBStart_Current, fPBEnd, MathF.fPI, 0);

                            if (bDeactivateMember) DeactivateMemberAndItsJoints(ref m_arrMembers[memberIndex]);
                        }
                    }
                }
            }

            // Girt Bracing - Front side
            // Nodes - Girt Bracing - Front side

            //TO Mato - to co to tu je?  bGeneratePurlinBracing??? asi skor bGenerateGirtBracingFrontSide nie?
            // To Ondrej - funguje to tak, ze sa tu nastavi aktualny pocet existujucich uzlov a to tak, ze sa pripocita pocet, ktory vznikol v predchadzajucom if
            // Mas pravdu, ze by sa to asi malo pripocitat uz v tom predchadzajucom if a tu by potom netrebalo kontrolovat ci je true a ci sa ma nieco pripocitat alebo nic - 0

            i_temp_numberofNodes += bGeneratePurlinBracing ? iPBNodesNo : 0;
            int iNumberOfGB_FSNodesInOneSideAndMiddleBay = 0;

            if (bGenerateGirtBracingFrontSide)
            {
                AddFrontOrBackGirtsBracingBlocksNodes(i_temp_numberofNodes, iArrGB_FS_NumberOfNodesPerBay, iArrGB_FS_NumberOfNodesPerBayFirstNode,
                iNumberOfTransverseSupports_FrontGirts, fH1_frame, fIntermediateSupportSpacingGirtsFrontSide, fDist_FrontGirts, fDist_FrontColumns, 0, out iNumberOfGB_FSNodesInOneSideAndMiddleBay);
            }

            // Members - Girt Bracing - Front side
            i_temp_numberofMembers += bGeneratePurlinBracing ? iPBMembersNo : 0;
            if (bGenerateGirtBracingFrontSide)
            {
               float fGBFrontSideEndToRafter = (float)m_arrCrSc[(int)EMemberGroupNames.eRafter_EF].z_min / (float)Math.Cos(fRoofPitch_rad) - (float)m_arrCrSc[(int)EMemberGroupNames.eFrontGirtBracing].y_max * (float)Math.Tan(fRoofPitch_rad) - fCutOffOneSide;

               AddFrontOrBackGirtsBracingBlocksMembers(i_temp_numberofNodes, i_temp_numberofMembers, iArrGB_FS_NumberOfNodesPerBay, iArrGB_FS_NumberOfNodesPerBayFirstNode, iArrGB_FS_NumberOfMembersPerBay,
               iNumberOfGB_FSNodesInOneSideAndMiddleBay, iNumberOfTransverseSupports_FrontGirts, eccentricityGirtFront_Y0, fGBFrontSideStart, fGBFrontSideEnd, fGBFrontSideEndToRafter, m_arrCrSc[(int)EMemberGroupNames.eFrontGirtBracing],
               EMemberType_FS_Position.BracingBlockGirtsFrontSide, fColumnsRotation, bUseGBEverySecond);
            }

            // Girt Bracing - Back side
            // Nodes - Girt Bracing - Back side
            i_temp_numberofNodes += bGenerateGirtBracingFrontSide ? iNumberOfGB_FSNodesInOneFrame : 0;
            int iNumberOfGB_BSNodesInOneSideAndMiddleBay = 0;

            if (bGenerateGirtBracingBackSide)
            {
                AddFrontOrBackGirtsBracingBlocksNodes(i_temp_numberofNodes, iArrGB_BS_NumberOfNodesPerBay, iArrGB_BS_NumberOfNodesPerBayFirstNode,
                iNumberOfTransverseSupports_BackGirts, fH1_frame, fIntermediateSupportSpacingGirtsBackSide, fDist_BackGirts, fDist_BackColumns, fL_tot, out iNumberOfGB_BSNodesInOneSideAndMiddleBay);
            }

            // Members - Girt Bracing - Back side
            i_temp_numberofMembers += bGenerateGirtBracingFrontSide ? iNumberOfGB_FSMembersInOneFrame : 0;
            if (bGenerateGirtBracingBackSide)
            {
                float fGBBackSideEndToRafter = (float)m_arrCrSc[(int)EMemberGroupNames.eRafter_EF].z_min / (float)Math.Cos(fRoofPitch_rad) - (float)m_arrCrSc[(int)EMemberGroupNames.eBackGirtBracing].y_max * (float)Math.Tan(fRoofPitch_rad) - fCutOffOneSide;

                AddFrontOrBackGirtsBracingBlocksMembers(i_temp_numberofNodes, i_temp_numberofMembers, iArrGB_BS_NumberOfNodesPerBay, iArrGB_BS_NumberOfNodesPerBayFirstNode, iArrGB_BS_NumberOfMembersPerBay,
                iNumberOfGB_BSNodesInOneSideAndMiddleBay, iNumberOfTransverseSupports_BackGirts, eccentricityGirtBack_YL, fGBBackSideStart, fGBBackSideEnd, fGBBackSideEndToRafter, m_arrCrSc[(int)EMemberGroupNames.eBackGirtBracing],
                EMemberType_FS_Position.BracingBlockGirtsBackSide, fColumnsRotation, bUseGBEverySecond);
            }

            i_temp_numberofMembers += bGenerateGirtBracingBackSide ? iNumberOfGB_BSMembersInOneFrame : 0;












            //----------------------------------------------------------------------------------------------------------------------------
            // TO Ondrej - IN WORK Cross-bracing

            // Cross-bracing

            if(bGenerateSideWallCrossBracing || bGenerateRoofCrossBracing)
            {
                // Cyklus pre kazdu bay , cross bracing properties pre bay zadanu v GUI
                foreach (CCrossBracingInfo cb in vm._crossBracingOptionsVM.CrossBracingList)
                {
                    if (!cb.WallLeft && !cb.WallRight && !cb.Roof) continue; // Ak nie je v bay zaskrtnute generovanie cross bracing tak pokracujeme dalsou bay

                    GenerateCrossBracingMembersInBay(
                    bGenerateSideWallCrossBracing && (cb.WallLeft || cb.WallRight),
                    bGenerateRoofCrossBracing && cb.Roof,
                    bGenerateGirts,
                    i_temp_numberofMembers,
                    new CMemberEccentricity((float)m_arrCrSc[(int)EMemberGroupNames.eMainColumn].z_max + (float)m_arrCrSc[(int)EMemberGroupNames.eCrossBracing_Walls].t_min, 0f),
                    new CMemberEccentricity((float)m_arrCrSc[(int)EMemberGroupNames.eRafter].z_max + (float)m_arrCrSc[(int)EMemberGroupNames.eCrossBracing_Walls].t_min, 0f),
                    0.5f * (float)m_arrCrSc[(int)EMemberGroupNames.eCrossBracing_Walls].b,
                    0.5f * (float)m_arrCrSc[(int)EMemberGroupNames.eCrossBracing_Walls].b,
                    0.5f * (float)m_arrCrSc[(int)EMemberGroupNames.eCrossBracing_Roof].b,
                    0.5f * (float)m_arrCrSc[(int)EMemberGroupNames.eCrossBracing_Roof].b,
                    m_arrCrSc[(int)EMemberGroupNames.eCrossBracing_Walls],
                    m_arrCrSc[(int)EMemberGroupNames.eCrossBracing_Roof],
                    0f,
                    0.5f * MathF.fPI,
                    cb);

                    i_temp_numberofMembers += cb.NumberOfCrossBracingMembers_Bay; // Navysime celkovy pocet o pocet prutov, ktore boli vygenerovane v danej bay
                }
            }
            //----------------------------------------------------------------------------------------------------------------------------







            ValidateIDs();

            FillIntermediateNodesForMembers();

            if (vm._generalOptionsVM.VariousCrossSections)
            {
                changeMembersVariousCrsc();
            }


            #region Joints
            if (joints == null)
                CreateJoints(bGenerateGirts, bUseMainColumnFlyBracingPlates, bGeneratePurlins, bUseRafterFlyBracingPlates, bGenerateFrontColumns, bGenerateBackColumns, bGenerateFrontGirts,
                             bGenerateBackGirts, bGenerateGirtBracingSideWalls, bGeneratePurlinBracing, bGenerateGirtBracingFrontSide, bGenerateGirtBracingBackSide, bGenerateSideWallCrossBracing, bGenerateRoofCrossBracing, vm._generalOptionsVM.WindPostUnderRafter, iOneColumnGirtNo);
            else
                m_arrConnectionJoints = joints;
            #endregion

            CountPlates_ValidationPurpose(false);

            #region Blocks

            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // Blocks
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            DoorsModels = new List<CBlock_3D_001_DoorInBay>();
            WindowsModels = new List<CBlock_3D_002_WindowInBay>();
            vm.SetModelBays(iFrameNo);
            bool isChangedFromCode = vm.IsSetFromCode;

            //TODO - to Mato - toto by sme mali nejako otestovat,ked to bolo zmenene tak,ze to ide z CPFDViewModelu,ci to funguje tak ako predtym a tak ako ma
            if (DoorBlocksProperties != null)
            {
                foreach (DoorProperties dp in DoorBlocksProperties.ToList())
                {
                    if (!bGenerateGirts && (dp.sBuildingSide == "Right" || dp.sBuildingSide == "Left")) { if (!isChangedFromCode) vm.IsSetFromCode = true; DoorBlocksProperties.Remove(dp); if (!isChangedFromCode) vm.IsSetFromCode = false; continue; }
                    else if (!bGenerateFrontGirts && dp.sBuildingSide == "Front") { if (!isChangedFromCode) vm.IsSetFromCode = true; DoorBlocksProperties.Remove(dp); if (!isChangedFromCode) vm.IsSetFromCode = false; continue; }
                    else if (!bGenerateBackGirts && dp.sBuildingSide == "Back") { if (!isChangedFromCode) vm.IsSetFromCode = true; DoorBlocksProperties.Remove(dp); if (!isChangedFromCode) vm.IsSetFromCode = false; continue; }

                    if (!dp.ValidateBays()) { if (!isChangedFromCode) vm.IsSetFromCode = true; DoorBlocksProperties.Remove(dp); if (!isChangedFromCode) vm.IsSetFromCode = false; continue; }

                    if (!dp.Validate()) { if (!isChangedFromCode) vm.IsSetFromCode = true; DoorBlocksProperties.Remove(dp); if (!isChangedFromCode) vm.IsSetFromCode = false; continue; }
                    else if (dp.Validate()) // Ak su vlastnosti dveri validne vyrobime blok dveri a nastavime rebates pre floor slab
                    {
                        AddDoorBlock(dp, iOneColumnGirtNo, iOneColumnGirtNo, 0.5f, fH1_frame, vm.RecreateJoints);

                        // TODO - Ondrej - potrebujem vm.FootingVM.RebateWidth_LRSide a vm.FootingVM.RebateWidth_FBSide
                        // Ale som trosku zacykleny lebo tento model sa vyraba skor nez VM existuje a zase rebate width sa naplna v CSlab, ktora sa vytvara az po vytvoreni bloku dveri
                        // Prosim pomoz mi to nejako usporiadat :)
                        // Mozno by bolo spravnejsie keby sa Rebate width nastavovala v UC_Doors pre Roller Door a tym padom by 
                        //v UC_Footing - Floor uz boli len vlastnosti saw cut, control joints a perimeters
                        // Potom by som vsetko co sa tyka rebates bral z doorBlocksProperties

                        if (dp.sBuildingSide == "Right" || dp.sBuildingSide == "Left")
                        {
                            //dp.SetRebateProperties((float)DoorsModels.Last().m_arrCrSc[1].b, 0.5f /*vm.FootingVM.RebateWidth_LRSide*/,
                            // fL1_frame, fDist_FrontColumns, fDist_BackColumns); // Vlastnosti rebate pre LR Side
                            dp.SetRebateProperties((float)DoorsModels.Last().m_arrCrSc[1].b, 0.5f /*vm.FootingVM.RebateWidth_LRSide*/,
                             GetBayWidth(dp.iBayNumber), fDist_FrontColumns, fDist_BackColumns); // Vlastnosti rebate pre LR Side
                        }
                        else
                        {
                            //dp.SetRebateProperties((float)DoorsModels.Last().m_arrCrSc[1].b, 0.4f /*vm.FootingVM.RebateWidth_FBSide*/,
                            //fL1_frame, fDist_FrontColumns, fDist_BackColumns); // Vlastnosti Rebate pre FB Side
                            dp.SetRebateProperties((float)DoorsModels.Last().m_arrCrSc[1].b, 0.4f /*vm.FootingVM.RebateWidth_FBSide*/,
                            GetBayWidth(dp.iBayNumber), fDist_FrontColumns, fDist_BackColumns); // Vlastnosti Rebate pre FB Side
                        }
                            
                    }
                }

                //refaktoring 24.1.2020
                //for (int i = 0; i < doorBlocksProperties.Count; i++)
                //{
                //    if (!bGenerateGirts && (doorBlocksProperties[i].sBuildingSide == "Right" || doorBlocksProperties[i].sBuildingSide == "Left")) continue;
                //    else if (!bGenerateFrontGirts && doorBlocksProperties[i].sBuildingSide == "Front") continue;
                //    else if (!bGenerateBackGirts && doorBlocksProperties[i].sBuildingSide == "Back") continue;

                //    if (!doorBlocksProperties[i].ValidateBays()) continue;

                //    if (doorBlocksProperties[i].Validate()) // Ak su vlastnosti dveri validne vyrobime blok dveri a nastavime rebates pre floor slab
                //    {
                //        AddDoorBlock(doorBlocksProperties[i], 0.5f, fH1_frame);

                //        // TODO - Ondrej - potrebujem vm.FootingVM.RebateWidth_LRSide a vm.FootingVM.RebateWidth_FBSide
                //        // Ale som trosku zacykleny lebo tento model sa vyraba skor nez VM existuje a zase rebate width sa naplna v CSlab, ktora sa vytvara az po vytvoreni bloku dveri
                //        // Prosim pomoz mi to nejako usporiadat :)
                //        // Mozno by bolo spravnejsie keby sa Rebate width nastavovala v UC_Doors pre Roller Door a tym padom by 
                //        //v UC_Footing - Floor uz boli len vlastnosti saw cut, control joints a perimeters
                //        // Potom by som vsetko co sa tyka rebates bral z doorBlocksProperties

                //        if (doorBlocksProperties[i].sBuildingSide == "Right" || doorBlocksProperties[i].sBuildingSide == "Left")
                //            doorBlocksProperties[i].SetRebateProperties((float)DoorsModels.Last().m_arrCrSc[1].b, 0.5f /*vm.FootingVM.RebateWidth_LRSide*/,
                //             fL1_frame, fDist_FrontColumns, fDist_BackColumns); // Vlastnosti rebate pre LR Side
                //        else
                //            doorBlocksProperties[i].SetRebateProperties((float)DoorsModels.Last().m_arrCrSc[1].b, 0.4f /*vm.FootingVM.RebateWidth_FBSide*/,
                //            fL1_frame, fDist_FrontColumns, fDist_BackColumns); // Vlastnosti Rebate pre FB Side
                //    }
                //}
            }

            if (vm.WindowBlocksProperties != null)
            {
                foreach (WindowProperties wp in vm.WindowBlocksProperties.ToList())
                {
                    if (!bGenerateGirts && (wp.sBuildingSide == "Right" || wp.sBuildingSide == "Left")) { if (!isChangedFromCode) vm.IsSetFromCode = true; vm.WindowBlocksProperties.Remove(wp); if (!isChangedFromCode) vm.IsSetFromCode = false; continue; }
                    else if (!bGenerateFrontGirts && wp.sBuildingSide == "Front") { if (!isChangedFromCode) vm.IsSetFromCode = true; vm.WindowBlocksProperties.Remove(wp); if (!isChangedFromCode) vm.IsSetFromCode = false; continue; }
                    else if (!bGenerateBackGirts && wp.sBuildingSide == "Back") { if (!isChangedFromCode) vm.IsSetFromCode = true; vm.WindowBlocksProperties.Remove(wp); if (!isChangedFromCode) vm.IsSetFromCode = false; continue; }

                    if (!wp.ValidateBays()) { if (!isChangedFromCode) vm.IsSetFromCode = true; vm.WindowBlocksProperties.Remove(wp); if (!isChangedFromCode) vm.IsSetFromCode = false; continue; }

                    if (!wp.Validate()) { if (!isChangedFromCode) vm.IsSetFromCode = true; vm.WindowBlocksProperties.Remove(wp); if (!isChangedFromCode) vm.IsSetFromCode = false; continue; }
                    else if (wp.Validate())
                    {
                        AddWindowBlock(wp, iOneColumnGirtNo, iOneColumnGirtNo, 0.5f, fH1_frame, vm.RecreateJoints);
                    }
                }
                //refaktoring 24.1.2020
                //for (int i = 0; i < windowBlocksProperties.Count; i++)
                //{
                //    if (!bGenerateGirts && (windowBlocksProperties[i].sBuildingSide == "Right" || windowBlocksProperties[i].sBuildingSide == "Left")) continue;
                //    else if (!bGenerateFrontGirts && windowBlocksProperties[i].sBuildingSide == "Front") continue;
                //    else if (!bGenerateBackGirts && windowBlocksProperties[i].sBuildingSide == "Back") continue;

                //    if (!windowBlocksProperties[i].ValidateBays()) continue;

                //    if (windowBlocksProperties[i].Validate()) AddWindowBlock(windowBlocksProperties[i], 0.5f);
                //}
            }

            CountPlates_ValidationPurpose(false);

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
                //BUG 327
                if (m_arrConnectionJoints[i].m_SecondaryMembers != null)
                {
                    foreach (CMember secMem in m_arrConnectionJoints[i].m_SecondaryMembers)
                    {
                        if (secMem.BIsGenerated == false)
                        {
                            CConnectionJointTypes joint = m_arrConnectionJoints[i];
                            DeactivateJoint(ref joint);
                        }
                    }
                }
            }

            // Opakovana kontrola po odstraneni spojov s MainMember = null
            int iCountOfJoints_NotGenerated = 0; // Number of joints on deactivated members (girts where dorr and window blocks are inserted) // Mozno sa to na nieco pouzije :)
            for (int i = 0; i < m_arrConnectionJoints.Count; i++)
            {
                if (m_arrConnectionJoints[i].m_MainMember == null)
                    throw new ArgumentNullException("Main member is not assigned to the joint No.:" + m_arrConnectionJoints[i].ID.ToString() + " Joint index in the list: " + i);

                if(m_arrConnectionJoints[i].BIsGenerated == false)
                {
                    iCountOfJoints_NotGenerated++;
                }
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
            // To Mato - kto kedy povie, ze je to OK a potom sa zmazu nejake taketo zbytocnosti???
            foreach (CMember member in m_arrMembers)
            {
                if (member.CrScStart.m_Mat is CMat_03_00)
                    DATABASE.CMaterialManager.LoadSteelMaterialProperties((CMat_03_00)member.CrScStart.m_Mat, member.CrScStart.m_Mat.Name);
            }
            //------------------------------------------------------------

            CountPlates_ValidationPurpose(false);

            // End of blocks
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            #endregion

            AddMembersToMemberGroupsLists();


            vm.SetComponentListAccordingToDoorsAndWindows();

            // Set members Generate, Display, Calculate, Design, MaterialList properties
            CModelHelper.SetMembersAccordingTo(m_arrMembers, componentList);

            #region Supports

            //m_arrNSupports = new CNSupport[2 * iFrameNo];

            // Nodal Supports - fill values

            // Set values
            bool[] bSupport1 = { true, true, true, false, vm.SupportTypeIndex == 0 ? true : false, false }; // Main and Edge Column (fixed / released rotation about Y axis)
            bool[] bSupport2 = { true, true, true, false, false, false }; // Wind Post

            m_arrNSupports = new CNSupport[listOfSupportedNodes_S1.Count + listOfSupportedNodes_S2.Count];

            for (int i = 0; i < m_arrNSupports.Length; i++)
            {
                if(i < listOfSupportedNodes_S1.Count)
                    m_arrNSupports[i] = new CNSupport(6, i + 1, listOfSupportedNodes_S1[i], bSupport1, 0);
                else
                    m_arrNSupports[i] = new CNSupport(6, i + 1, listOfSupportedNodes_S2[i - listOfSupportedNodes_S1.Count], bSupport2, 0);
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

            if (foundations == null)
            {
                CreateFoundations(bGenerateFrontColumns, bGenerateBackColumns, vm._generalOptionsVM.UseStraightReinforcementBars);
            }
            else
                m_arrFoundations = foundations;
            #endregion

            #region Floor slab, saw cuts and control joints

            if (slabs == null)
            {
                CreateFloorSlab(bGenerateFrontColumns, bGenerateBackColumns, bGenerateFrontGirts, bGenerateBackGirts, vm._generalOptionsVM.WindPostUnderRafter);
            }
            else
                m_arrSlabs = slabs;
            #endregion

            #region Cladding
            m_arrGOCladding = new List<BaseClasses.GraphObj.CCladding>(1) { new BaseClasses.GraphObj.CCladding(0, eKitset, sGeometryInputData, (CCrSc_TW)m_arrCrSc[(int)EMemberGroupNames.eMainColumn_EF],
               (Color)ColorConverter.ConvertFromString(_pfdVM.WallCladdingColors.ElementAtOrDefault(_pfdVM.WallCladdingColorIndex).CodeHEX),
               (Color)ColorConverter.ConvertFromString(_pfdVM.RoofCladdingColors.ElementAtOrDefault(_pfdVM.RoofCladdingColorIndex).CodeHEX), true, 0) };
            #endregion

            #region Doors
            if (_pfdVM.DoorBlocksProperties != null)
            {
                float fRotationZDegrees = 0f;
                Point3D pControlEdgePoint = new Point3D((_pfdVM.DoorBlocksProperties[0].iBayNumber - 1) * fDist_FrontColumns + _pfdVM.DoorBlocksProperties[0].fDoorCoordinateXinBlock, -0.15, 0);

                if (_pfdVM.DoorBlocksProperties[0].sBuildingSide == "Back")
                    pControlEdgePoint.Y = fL_tot + 0.15f;

                if (_pfdVM.DoorBlocksProperties[0].sBuildingSide == "Left" || _pfdVM.DoorBlocksProperties[0].sBuildingSide == "Right")
                {
                    fRotationZDegrees = 90f;
                    pControlEdgePoint = new Point3D(-0.30, GetBaysWidthUntilFrameIndex(_pfdVM.DoorBlocksProperties[0].iBayNumber - 1) + _pfdVM.DoorBlocksProperties[0].fDoorCoordinateXinBlock, 0);

                    if (_pfdVM.DoorBlocksProperties[0].sBuildingSide == "Right")
                        pControlEdgePoint.X = fW_frame + 0.30f;
                }

                BaseClasses.GraphObj.CStructure_Door door1 = new BaseClasses.GraphObj.CStructure_Door(0, 1,
                   pControlEdgePoint, _pfdVM.DoorBlocksProperties[0].fDoorsWidth, _pfdVM.DoorBlocksProperties[0].fDoorsHeight, 0.1f,
                   new DiffuseMaterial(new SolidColorBrush((Color)ColorConverter.ConvertFromString((_pfdVM.Flashings.Single(i => i.Name == "Roller Door Trimmer")).CoatingColor.CodeHEX))),
                   new DiffuseMaterial(new SolidColorBrush((Color)ColorConverter.ConvertFromString(_pfdVM.DoorBlocksProperties[0].CoatingColor.CodeHEX))), 0.010f, fRotationZDegrees, true, 0f);

                m_arrGOStrDoors = new BaseClasses.GraphObj.CStructure_Door[1] { door1 };
            }
            #endregion

            #region Windows
            if (_pfdVM.WindowBlocksProperties != null)
            {
                float fRotationZDegrees = 0f;
                Point3D pControlEdgePoint = new Point3D((_pfdVM.WindowBlocksProperties[0].iBayNumber - 1) * fDist_FrontColumns + _pfdVM.WindowBlocksProperties[0].fWindowCoordinateXinBay, -0.15, _pfdVM.WindowBlocksProperties[0].fWindowCoordinateZinBay);

                if (_pfdVM.WindowBlocksProperties[0].sBuildingSide == "Back")
                    pControlEdgePoint.Y = fL_tot + 0.15f;

                if (_pfdVM.WindowBlocksProperties[0].sBuildingSide == "Left" || _pfdVM.WindowBlocksProperties[0].sBuildingSide == "Right")
                {
                    fRotationZDegrees = 90f;
                    pControlEdgePoint = new Point3D(-0.30, GetBaysWidthUntilFrameIndex(_pfdVM.WindowBlocksProperties[0].iBayNumber - 1) + _pfdVM.WindowBlocksProperties[0].fWindowCoordinateXinBay, _pfdVM.WindowBlocksProperties[0].fWindowCoordinateZinBay);

                    if (_pfdVM.WindowBlocksProperties[0].sBuildingSide == "Right")
                        pControlEdgePoint.X = fW_frame + 0.30f;
                }

                BaseClasses.GraphObj.CStructure_Window window1 = new BaseClasses.GraphObj.CStructure_Window(0, EWindowShapeType.eClassic, 1,
                   pControlEdgePoint, _pfdVM.WindowBlocksProperties[0].fWindowsWidth, _pfdVM.WindowBlocksProperties[0].fWindowsHeight, 0.1f,
                   new DiffuseMaterial(new SolidColorBrush((Color)ColorConverter.ConvertFromString((_pfdVM.Flashings.Single(i => i.Name == "Window")).CoatingColor.CodeHEX))),
                   new DiffuseMaterial(Brushes.LightBlue), 0.010f, fRotationZDegrees, true, 0f);

                fRotationZDegrees = 0f;
                BaseClasses.GraphObj.CStructure_Window window2_temp = new BaseClasses.GraphObj.CStructure_Window(1, EWindowShapeType.eClassic, 3,
                   new Point3D(2, -0.15, 1.2f), 1.5f / 3.0f, 0.95f, 0.1f,
                   new DiffuseMaterial(new SolidColorBrush((Color)ColorConverter.ConvertFromString((_pfdVM.Flashings.Single(i => i.Name == "Window")).CoatingColor.CodeHEX))),
                   new DiffuseMaterial(Brushes.LightBlue), 0.010f, 0f, true, 0f);

                m_arrGOStrWindows = new BaseClasses.GraphObj.CStructure_Window[2] { window1, window2_temp };
            }
            #endregion
        }

        //temp test zatial task 612
        private void changeMembersVariousCrsc()
        {
            ChangeFrameMembersVariousCrsc();

            ChangeBayMembersVariousCrsc();
        }
        private void ChangeFrameMembersVariousCrsc()
        {
            if (_clVM.FramesComponentList == null)
            {
                _clVM.InitControlsAccordingToFrames(_pfdVM.Frames);
            }

            double dist = 0;
            int index = 0;
            foreach (FrameMembersInfo fmi in _clVM.FramesComponentList)
            {
                CMember[] frameColumns = ModelHelper.GetMembersInDistance(this, dist, (int)EGCSDirection.Y, EMemberType_FS.eMC, EMemberType_FS.eEC);
                CMember[] frameRafters = ModelHelper.GetMembersInDistance(this, dist, (int)EGCSDirection.Y, EMemberType_FS.eMR, EMemberType_FS.eER);

                foreach (CMember m in frameColumns)
                {
                    m.CrScStart = CrScFactory.GetCrSc(fmi.ColumnSection);
                    //m.EccentricityStart  niekde bude potrebne asi prestavit eccentricity
                    m.m_Mat = MaterialFactory.GetMaterial(fmi.ColumnMaterial);
                }
                foreach (CMember m in frameRafters)
                {
                    m.CrScStart = CrScFactory.GetCrSc(fmi.RafterSection);
                    m.m_Mat = MaterialFactory.GetMaterial(fmi.RafterMaterial);
                }

                dist += L1_Bays.ElementAtOrDefault(index++);
            }
        }

        private void ChangeBayMembersVariousCrsc()
        {
            if (_clVM.BaysComponentList == null)
            {
                _clVM.InitControlsAccordingToFrames(_pfdVM.Frames);
            }

            double dist = 0;
            int index = 0;
            foreach (BayMembersInfo bmi in _clVM.BaysComponentList)
            {
                CMember[] bayMembers = ModelHelper.GetMembersInDistanceInterval(this, dist, dist + L1_Bays.ElementAtOrDefault(index++), (int)EGCSDirection.Y, true, true, false);
                
                foreach (CMember m in bayMembers)
                {
                    if (m.EMemberTypePosition == EMemberType_FS_Position.EdgePurlin)
                    {
                        m.CrScStart = CrScFactory.GetCrSc(bmi.Section_EP);
                        m.m_Mat = MaterialFactory.GetMaterial(bmi.Material_EP);
                    }
                    else if (m.EMemberTypePosition == EMemberType_FS_Position.Purlin)
                    {
                        m.CrScStart = CrScFactory.GetCrSc(bmi.Section_P);
                        m.m_Mat = MaterialFactory.GetMaterial(bmi.Material_P);
                    }
                    else if (m.EMemberTypePosition == EMemberType_FS_Position.Girt)
                    {
                        m.CrScStart = CrScFactory.GetCrSc(bmi.Section_G);
                        m.m_Mat = MaterialFactory.GetMaterial(bmi.Material_G);
                    }
                    else if (m.EMemberTypePosition == EMemberType_FS_Position.BracingBlockGirts)
                    {
                        m.CrScStart = CrScFactory.GetCrSc(bmi.Section_GB);
                        m.m_Mat = MaterialFactory.GetMaterial(bmi.Material_GB);
                    }
                    else if (m.EMemberTypePosition == EMemberType_FS_Position.BracingBlockPurlins)
                    {
                        m.CrScStart = CrScFactory.GetCrSc(bmi.Section_PB);
                        m.m_Mat = MaterialFactory.GetMaterial(bmi.Material_PB);
                    }
                    else if (m.EMemberTypePosition == EMemberType_FS_Position.CrossBracingWall)
                    {
                        m.CrScStart = CrScFactory.GetCrSc(bmi.Section_CBW);
                        m.m_Mat = MaterialFactory.GetMaterial(bmi.Material_CBW);
                    }
                    else if (m.EMemberTypePosition == EMemberType_FS_Position.CrossBracingRoof)
                    {
                        m.CrScStart = CrScFactory.GetCrSc(bmi.Section_CBR);
                        m.m_Mat = MaterialFactory.GetMaterial(bmi.Material_CBR);
                    }
                }

                dist += L1_Bays.ElementAtOrDefault(index);
            }
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
            DateTime start = DateTime.Now;
            // Loading
            #region Load Cases
            // Load Cases
            CLoadCaseGenerator loadCaseGenerator = new CLoadCaseGenerator();
            m_arrLoadCases = loadCaseGenerator.GenerateLoadCases();
            //System.Diagnostics.Trace.WriteLine("----> after loadCaseGenerator.GenerateLoadCases(): " + (DateTime.Now - start).TotalMilliseconds);
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
                //System.Diagnostics.Trace.WriteLine("----> after surfaceLoadGenerator.GenerateSurfaceLoads(): " + (DateTime.Now - start).TotalMilliseconds);
            }

            #endregion

            #region Earthquake - nodal loads
            // Earthquake

            if (bGenerateNodalLoads)
            {
                int iNumberOfLoadsInXDirection = iFrameNo;
                int iNumberOfLoadsInYDirection = 2;

                CNodalLoadGenerator nodalLoadGenerator = new CNodalLoadGenerator(iNumberOfLoadsInXDirection, iNumberOfLoadsInYDirection, iFrameNodesNo, m_arrLoadCases, m_arrNodes,/* fL1_frame,*/ eq);
                nodalLoadGenerator.GenerateNodalLoads();
                //System.Diagnostics.Trace.WriteLine("----> after nodalLoadGenerator.GenerateNodalLoads(): " + (DateTime.Now - start).TotalMilliseconds);
            }
            #endregion

            #region Member Loads
            if (bGenerateLoadsOnGirts || bGenerateLoadsOnPurlins || bGenerateLoadsOnColumns || bGenerateLoadsOnFrameMembers)
            {
                CMemberLoadGenerator loadGenerator =
                new CMemberLoadGenerator(
                iFrameNodesNo,
                iEavesPurlinNoInOneFrame,
                iFrameNo,
                //fL1_frame,
                L1_Bays,
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
                //System.Diagnostics.Trace.WriteLine("----> after member loads loadGenerator: " + (DateTime.Now - start).TotalMilliseconds);

                #region Secondary Member Loads (girts, purlins, wind posts, door trimmers)
                // Purlins, eave purlins, girts, ....
                LoadCasesMemberLoads memberLoadsOnPurlinsGirtsColumns = new LoadCasesMemberLoads();
                // Generate single member loads
                if (bGenerateLoadsOnGirts || bGenerateLoadsOnPurlins || bGenerateLoadsOnColumns)
                {
                    memberLoadsOnPurlinsGirtsColumns = loadGenerator.GetGeneratedMemberLoads(m_arrLoadCases, m_arrMembers);
                    //System.Diagnostics.Trace.WriteLine("----> after loadGenerator.GetGeneratedMemberLoads: " + (DateTime.Now - start).TotalMilliseconds);
                    loadGenerator.AssignMemberLoadListsToLoadCases(memberLoadsOnPurlinsGirtsColumns);
                    //System.Diagnostics.Trace.WriteLine("----> after AssignMemberLoadListsToLoadCases: " + (DateTime.Now - start).TotalMilliseconds);
                }
                #endregion

                #region Frame Member Loads (main and edge columns and rafters)
                // Frame Member Loads
                LoadCasesMemberLoads memberLoadsOnFrames = new LoadCasesMemberLoads();
                if (bGenerateLoadsOnFrameMembers)
                {
                    memberLoadsOnFrames = loadGenerator.GetGenerateMemberLoadsOnFrames();
                    loadGenerator.AssignMemberLoadListsToLoadCases(memberLoadsOnFrames);
                    //System.Diagnostics.Trace.WriteLine("----> after Frame Member Loads AssignMemberLoadListsToLoadCases: " + (DateTime.Now - start).TotalMilliseconds);
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
                    //System.Diagnostics.Trace.WriteLine("----> after Merge Member Load Lists AssignMemberLoadListsToLoadCases: " + (DateTime.Now - start).TotalMilliseconds);
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

            //System.Diagnostics.Trace.WriteLine("----> after generator.GenerateAll(): " + (DateTime.Now - start).TotalMilliseconds);
            #endregion

            #region Limit states
            // Limit States
            m_arrLimitStates = new CLimitState[3];
            m_arrLimitStates[0] = new CLimitState("Ultimate Limit State - Stability", ELSType.eLS_ULS);
            m_arrLimitStates[1] = new CLimitState("Ultimate Limit State - Strength", ELSType.eLS_ULS);
            m_arrLimitStates[2] = new CLimitState("Serviceability Limit State", ELSType.eLS_SLS);
            #endregion
        }

        public void AddFrontOrBackGirtsBracingBlocksNodes(int i_temp_numberofNodes, int [] iArrGB_NumberOfNodesPerBay, int [] iArrGB_NumberOfNodesPerBayFirstNode,
            int iNumberOfTransverseSupports, float fHeight, float fIntermediateSupportSpacing,  float fDist_Girts, float fDist_Columns, float fy_Global_Coord, out int iNumberOfGB_NodesInOneSideAndMiddleBay)
        {
            int iTemp = 0;

            for (int i = 0; i < iArrGB_NumberOfNodesPerBay.Length; i++) // Left side
            {
                for (int j = 0; j < iArrGB_NumberOfNodesPerBayFirstNode[i]; j++) // Bay
                {
                    for (int k = 0; k < iNumberOfTransverseSupports; k++)
                    {
                        float x_glob = i * fDist_FrontColumns + (k + 1) * fIntermediateSupportSpacing;
                        float z_glob;

                        if (j < iArrGB_NumberOfNodesPerBayFirstNode[i] - 1)
                            z_glob = (fBottomGirtPosition + j * fDist_Girts);
                        else
                            CalcColumnNodeCoord_Z(false, fH1_frame, x_glob, out z_glob); // Top bracing blocks under the edge rafter

                        m_arrNodes[i_temp_numberofNodes + iTemp + j * iNumberOfTransverseSupports + k] = new CNode(i_temp_numberofNodes + iTemp + j * iNumberOfTransverseSupports + k + 1, x_glob, fy_Global_Coord, z_glob, 0);
                        RotateFrontOrBackFrameNodeAboutZ(m_arrNodes[i_temp_numberofNodes + iTemp + j * iNumberOfTransverseSupports + k]);
                    }
                }
                iTemp += iArrGB_NumberOfNodesPerBay[i];
            }

            iNumberOfGB_NodesInOneSideAndMiddleBay = iTemp;
            iTemp = 0;

            for (int i = 0; i < iArrGB_NumberOfNodesPerBay.Length - 1; i++) // Right side
            {
                for (int j = 0; j < iArrGB_NumberOfNodesPerBayFirstNode[i]; j++) // Bay
                {
                    for (int k = 0; k < iNumberOfTransverseSupports; k++)
                    {
                        float x_glob = i * fDist_Columns + (k + 1) * fIntermediateSupportSpacing;
                        float z_glob;

                        if (j < iArrGB_NumberOfNodesPerBayFirstNode[i] - 1)
                            z_glob = (fBottomGirtPosition + j * fDist_Girts);
                        else
                            CalcColumnNodeCoord_Z(false, fH1_frame, x_glob, out z_glob); // Top bracing blocks under the edge rafter

                        m_arrNodes[i_temp_numberofNodes + iNumberOfGB_NodesInOneSideAndMiddleBay + iTemp + j * iNumberOfTransverseSupports + k] = new CNode(i_temp_numberofNodes + iNumberOfGB_NodesInOneSideAndMiddleBay + iTemp + j * iNumberOfTransverseSupports + k + 1, fW_frame - x_glob, fy_Global_Coord, z_glob, 0);
                        RotateFrontOrBackFrameNodeAboutZ(m_arrNodes[i_temp_numberofNodes + iNumberOfGB_NodesInOneSideAndMiddleBay + iTemp + j * iNumberOfTransverseSupports + k]);
                    }
                }
                iTemp += iArrGB_NumberOfNodesPerBay[i];
            }
        }

        public void AddFrontOrBackGirtsBracingBlocksMembers(int i_temp_numberofNodes, int i_temp_numberofMembers, int[] iArrGB_NumberOfNodesPerBay, int[] iArrGB_NumberOfNodesPerBayFirstNode, int[] iArrGB_NumberOfMembersPerBay,
            int iNumberOfGB_NodesInOneSideAndMiddleBay, int iNumberOfTransverseSupports, CMemberEccentricity eGirtEccentricity, float fGBAlignmentStart, float fGBAlignmentEnd, float fGBAlignmentEndToRafter, CCrSc section,
            EMemberType_FS_Position eMemberType_FS_Position, float fColumnsRotation, bool bUseBraicingEverySecond)
        {
            float fRealLengthLimit = 0.25f; // Limit pre dlzku pruta, ak je prut kratsi ako limit, nastavimme mu bGenerate na false

            int iTemp = 0;
            int iTemp2 = 0;

            for (int i = 0; i < iArrGB_NumberOfMembersPerBay.Length; i++) // Left side
            {
                for (int j = 0; j < (iArrGB_NumberOfNodesPerBayFirstNode[i] - 1); j++) // Bay
                {
                    bool bDeactivateMember = false;
                    if (bUseBraicingEverySecond && j % 2 == 1) bDeactivateMember = true;

                    float fGBAlignmentEnd_Current = fGBAlignmentEnd;

                    if (j == iArrGB_NumberOfNodesPerBayFirstNode[i] - 1 - 1) // Last
                        fGBAlignmentEnd_Current = fGBAlignmentEndToRafter;

                    for (int k = 0; k < iNumberOfTransverseSupports; k++)
                    {
                        int memberIndex = i_temp_numberofMembers + iTemp2 + j * iNumberOfTransverseSupports + k;
                        int startNodeIndex = i_temp_numberofNodes + iTemp + j * iNumberOfTransverseSupports + k;
                        int endNodeIndex = i_temp_numberofNodes + iTemp + (j + 1) * iNumberOfTransverseSupports + k;
                        m_arrMembers[memberIndex] = new CMember(memberIndex + 1, m_arrNodes[startNodeIndex], m_arrNodes[endNodeIndex], section, EMemberType_FS.eGB, eMemberType_FS_Position, eGirtEccentricity, eGirtEccentricity, fGBAlignmentStart, fGBAlignmentEnd_Current, fColumnsRotation, 0);

                        if (m_arrMembers[memberIndex].FLength_real < fRealLengthLimit)
                            DeactivateMember(ref m_arrMembers[memberIndex]);

                        if(bDeactivateMember) DeactivateMemberAndItsJoints(ref m_arrMembers[memberIndex]);
                    }
                }
                iTemp += iArrGB_NumberOfNodesPerBay[i];
                iTemp2 += iArrGB_NumberOfMembersPerBay[i];
            }

            int iNumberOfGB_MembersInOneSideAndMiddleBay = iTemp2;
            iTemp = 0;
            iTemp2 = 0;

            for (int i = 0; i < iArrGB_NumberOfMembersPerBay.Length - 1; i++) // Right side
            {
                for (int j = 0; j < (iArrGB_NumberOfNodesPerBayFirstNode[i] - 1); j++) // Bay
                {
                    bool bDeactivateMember = false;
                    if (bUseBraicingEverySecond && j % 2 == 1) bDeactivateMember = true;

                    float fGBAlignmentEnd_Current = fGBAlignmentEnd;

                    if (j == iArrGB_NumberOfNodesPerBayFirstNode[i] - 1 - 1) // Last
                        fGBAlignmentEnd_Current = fGBAlignmentEndToRafter;

                    for (int k = 0; k < iNumberOfTransverseSupports; k++)
                    {
                        int memberIndex = i_temp_numberofMembers + iNumberOfGB_MembersInOneSideAndMiddleBay + iTemp2 + j * iNumberOfTransverseSupports + k;
                        int startNodeIndex = i_temp_numberofNodes + iNumberOfGB_NodesInOneSideAndMiddleBay + iTemp + j * iNumberOfTransverseSupports + k;
                        int endNodeIndex = i_temp_numberofNodes + iNumberOfGB_NodesInOneSideAndMiddleBay + iTemp + (j + 1) * iNumberOfTransverseSupports + k;
                        m_arrMembers[memberIndex] = new CMember(memberIndex + 1, m_arrNodes[startNodeIndex], m_arrNodes[endNodeIndex], section, EMemberType_FS.eGB, eMemberType_FS_Position, eGirtEccentricity, eGirtEccentricity, fGBAlignmentStart, fGBAlignmentEnd_Current, fColumnsRotation, 0);

                        if (m_arrMembers[memberIndex].FLength_real < fRealLengthLimit)
                            DeactivateMember(ref m_arrMembers[memberIndex]);

                        if (bDeactivateMember) DeactivateMemberAndItsJoints(ref m_arrMembers[memberIndex]);
                    }
                }
                iTemp += iArrGB_NumberOfNodesPerBay[i];
                iTemp2 += iArrGB_NumberOfMembersPerBay[i];
            }
        }

        /*
        public void AddDoorBlock(DoorProperties prop, float fLimitDistanceFromColumn, float fSideWallHeight, bool addJoints)
        {
            CMember mReferenceGirt;
            CMember mColumnLeft;
            CMember mColumnRight;
            CMember mEavesPurlin;
            CBlock_3D_001_DoorInBay door;
            Point3D pControlPointBlock;
            float fBayWidth;
            float fBayHeight = fH1_frame; // TODO - spocitat vysku bay v mieste bloku (pre front a back budu dve vysky v mieste vlozenia stlpov bloku
            int iFirstMemberToDeactivate;
            bool bIsReverseSession;
            bool bIsFirstBayInFrontorBackSide;
            bool bIsLastBayInFrontorBackSide;

            DeterminateBasicPropertiesToInsertBlock(prop.sBuildingSide, prop.iBayNumber, out mReferenceGirt, out mColumnLeft, out mColumnRight, out mEavesPurlin, out pControlPointBlock, out fBayWidth, out iFirstMemberToDeactivate, out bIsReverseSession, out bIsFirstBayInFrontorBackSide, out bIsLastBayInFrontorBackSide);

            // Set girt to connect columns / trimmers
            int iNumberOfGirtsToDeactivate = (int)((prop.fDoorsHeight - fBottomGirtPosition) / fDist_Girt) + 1; // Number of intermediate girts + Bottom Girt (prevzate z CBlock_3D_001_DoorInBay)
            CMember mGirtToConnectDoorTrimmers = m_arrMembers[(mReferenceGirt.ID - 1) + iNumberOfGirtsToDeactivate]; // Toto je girt, ku ktoremu sa pripoja stlpy dveri (len v pripade ze sa nepripoja k eave purlin alebo edge rafter) - 1 -index reference girt

            door = new CBlock_3D_001_DoorInBay(
                prop,
                fLimitDistanceFromColumn,
                fBottomGirtPosition,
                fDist_Girt,
                mReferenceGirt,
                mGirtToConnectDoorTrimmers,
                mColumnLeft,
                mColumnRight,
                mEavesPurlin,
                fBayWidth,
                fBayHeight,
                fUpperGirtLimit,
                bIsReverseSession,
                bIsFirstBayInFrontorBackSide,
                bIsLastBayInFrontorBackSide);

            AddDoorOrWindowBlockProperties(pControlPointBlock, iFirstMemberToDeactivate, door, addJoints);

            DoorsModels.Add(door);
        }

        public void AddWindowBlock(WindowProperties prop, float fLimitDistanceFromColumn, bool addJoints)
        {
            CMember mReferenceGirt;
            CMember mColumnLeft;
            CMember mColumnRight;
            CMember mEavesPurlin;
            CBlock_3D_002_WindowInBay window;
            Point3D pControlPointBlock;
            float fBayWidth;
            float fBayHeight = fH1_frame; // TODO - spocitat vysku bay v mieste bloku (pre front a back budu dve vysky v mieste vlozenia stlpov bloku
            int iFirstGirtInBay;
            int iFirstMemberToDeactivate;
            bool bIsReverseSession;
            bool bIsFirstBayInFrontorBackSide;
            bool bIsLastBayInFrontorBackSide;

            DeterminateBasicPropertiesToInsertBlock(prop.sBuildingSide, prop.iBayNumber, out mReferenceGirt, out mColumnLeft, out mColumnRight, out mEavesPurlin, out pControlPointBlock, out fBayWidth, out iFirstGirtInBay, out bIsReverseSession, out bIsFirstBayInFrontorBackSide, out bIsLastBayInFrontorBackSide);

            // Prevzate z CBlock_3D_002_WindowInBay
            int iNumberOfGirtsUnderWindow = (int)((prop.fWindowCoordinateZinBay - fBottomGirtPosition) / fDist_Girt) + 1;
            float fCoordinateZOfGirtUnderWindow = (iNumberOfGirtsUnderWindow - 1) * fDist_Girt + fBottomGirtPosition;

            if (prop.fWindowCoordinateZinBay <= fBottomGirtPosition)
            {
                iNumberOfGirtsUnderWindow = 0;
                fCoordinateZOfGirtUnderWindow = 0f;
            }

            int iNumberOfGirtsToDeactivate = (int)((prop.fWindowsHeight + prop.fWindowCoordinateZinBay - fCoordinateZOfGirtUnderWindow) / fDist_Girt); // Number of intermediate girts to deactivate

            CMember mGirtToConnectWindowColumns_Bottom = null;

            if(iNumberOfGirtsUnderWindow > 0)
               mGirtToConnectWindowColumns_Bottom = m_arrMembers[(mReferenceGirt.ID - 1) + (iNumberOfGirtsUnderWindow - 1)]; // Toto je girt, ku ktoremu sa pripoja stlpiky okna v dolnej casti

            CMember mGirtToConnectWindowColumns_Top = m_arrMembers[(mReferenceGirt.ID - 1) + (iNumberOfGirtsUnderWindow - 1) + iNumberOfGirtsToDeactivate + 1]; // Toto je girt, ku ktoremu sa pripoja stlpiky okna v hornej casti (len v pripade ze sa nepripoja k eave purlin alebo edge rafter)

            window = new CBlock_3D_002_WindowInBay(
                prop,
                fLimitDistanceFromColumn,
                fBottomGirtPosition,
                fDist_Girt,
                mReferenceGirt,
                mGirtToConnectWindowColumns_Bottom,
                mGirtToConnectWindowColumns_Top,
                mColumnLeft,
                mColumnRight,
                mEavesPurlin,
                fBayWidth,
                fBayHeight,
                fUpperGirtLimit,
                bIsReverseSession,
                bIsFirstBayInFrontorBackSide,
                bIsLastBayInFrontorBackSide);

            iFirstMemberToDeactivate = iFirstGirtInBay + window.iNumberOfGirtsUnderWindow;

            AddDoorOrWindowBlockProperties(pControlPointBlock, iFirstMemberToDeactivate, window, addJoints);

            WindowsModels.Add(window);
        }
        public override void DeterminateBasicPropertiesToInsertBlock(
            string sBuildingSide,                     // Identification of building side (left, right, front, back)
            int iBayNumber,                           // Bay number (1-n) in positive X or Y direction
            int iSideWallLeftColumnGirtNoInOneFrame,  // Number of girts in the left side wall per one column (one frame)
            int iSideWallRightColumnGirtNoInOneFrame, // Number of girts in the right side wall per one column (one frame)
            out CMember mReferenceGirt,               // Reference girt - first girts that needs to be deactivated and replaced by new member (some parameters are same for deactivated and new member)
            out CMember mColumnLeft,                  // Left column of bay
            out CMember mColumnRight,                 // Right column of bay
            out CMember mEavesPurlin,                  // Eave purlin for left and right side
            out Point3D pControlPointBlock,            // Conctrol point to insert block - defined as left column base point
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

                if (sBuildingSide == "Left")
                mEavesPurlin = m_arrMembers[(iBlockFrame * iEavesPurlinNoInOneFrame) + iBlockFrame * (iFrameNodesNo - 1) + 4];
                else
                mEavesPurlin = m_arrMembers[(iBlockFrame * iEavesPurlinNoInOneFrame) + iBlockFrame * (iFrameNodesNo - 1) + 5];
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
                    iArrayOfGirtsPerColumnCount = iArrNumberOfNodesPerFrontColumnFromLeft;
                    //iNumberOfGirtsInWall = iFrontGirtsNoInOneFrame;
                    fBayWidth = fDist_FrontColumns;
                }
                else // Back side properties
                {
                    iNumberOfIntermediateColumns = iBackColumnNoInOneFrame;
                    iArrayOfGirtsPerColumnCount = iArrNumberOfNodesPerBackColumnFromLeft;
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
                mEavesPurlin = null; // Not defined for the front and back side
            }

            pControlPointBlock = new Point3D(mColumnLeft.NodeStart.X, mColumnLeft.NodeStart.Y, mColumnLeft.NodeStart.Z);
        }

        //Tuto funkciu mam pozriet - Mato chce:
        //rozsirujem tam velkosti poli a take veci CModel_PFD_01_GR - riadok 1751
        //vlastne tie objekty z objektu CBlock pridavam do celkoveho zoznamu, ale napriklad prerez pre girts som ignoroval aby tam nebol 2x
        //Chce to vymysliet nejaky koncept ako to ma fungovat a chce to programatorsku hlavu 🙂
        //tie moje "patlacky" ako sa to tam dolepuje do poli atd by som nebral velmi vazne
        //Malo by ty to fungovat tak, ze ked pridam prve dvere tak sa tie prierezy pridaju a ked pridavam dalsie, tak uz sa pridavaju len uzly a pruty a prierez sa len nastavi
        //uz by sa nemal vytvarat novy
        public void AddDoorOrWindowBlockProperties(Point3D pControlPointBlock, int iFirstMemberToDeactivate, CBlock block, bool addJoints = true)
        {
            float fBlockRotationAboutZaxis_rad = 0;

            if (block.BuildingSide == "Left" || block.BuildingSide == "Right")
                fBlockRotationAboutZaxis_rad = MathF.fPI / 2.0f; // Parameter of block - depending on side of building (front, back (0 deg), left, right (90 deg))

            //----------------------------------------------------------------------------------------------------------------------------------------------------
            // TODO 405 - TO Ondrej - tu sa znazim pripravit obrysove body otvoru v 3D - GCS
            // Opening definition points
            // Transformation from LCS of block to GCS // Create definition points in 3D

            List<Point3D> openningPointsInGCS = new List<Point3D>();

            foreach (System.Windows.Point p2D in block.openningPoints)
            {
                Point3D p3D = new Point3D(p2D.X, 0, p2D.Y);
                RotateAndTranslatePointAboutZ_CCW(pControlPointBlock, ref p3D, fBlockRotationAboutZaxis_rad);
                openningPointsInGCS.Add(p3D); // Output - s tymito suradnicami by sa mala porovnavat pozicia girt bracing na jednotlivych stranach budovy
            }
            //----------------------------------------------------------------------------------------------------------------------------------------------------

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
                //m_arrCrSc[arraysizeoriginal + i - 1].ID = arraysizeoriginal + i; // -1 + 1; // Odcitat index pretoze prvy prierez ignorujeme a pridat 1 pre ID (+ 1)
            }

            //task 405 - je to hotove, ale chcelo by to mozno aj zistit ako je to narocne na pamat, lebo su tam vyhladavacky
            DeactivateBracingBlocksThroughtBlock(block, openningPointsInGCS);

            // Nodes
            arraysizeoriginal = m_arrNodes.Length;
            Array.Resize(ref m_arrNodes, m_arrNodes.Length + block.m_arrNodes.Length);

            int iNumberofMembersToDeactivate = block.INumberOfGirtsToDeactivate;

            // Deactivate already generated members in the bay (space between frames) where is the block inserted
            for (int i = 0; i < iNumberofMembersToDeactivate; i++)
            {
                // Deactivate Members
                // Deactivate Member Joints
                CMember m = m_arrMembers[iFirstMemberToDeactivate + i];
                DeactivateMemberAndItsJoints(ref m);

                // -------------------------------------------------------------------------------------------------
                // Deactivate bracing blocks and joints
                // Find bracing blocks for deactivated girt
                DeactivateMemberBracingBlocks(m, block, openningPointsInGCS);
            }

            // Copy block nodes into the model
            for (int i = 0; i < block.m_arrNodes.Length; i++)
            {
                RotateAndTranslateNodeAboutZ_CCW(pControlPointBlock, ref block.m_arrNodes[i], fBlockRotationAboutZaxis_rad);
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
            if (addJoints)
            {
                foreach (CConnectionJointTypes joint in block.m_arrConnectionJoints)
                    m_arrConnectionJoints.Add(joint); // Add joint
            }

            // Validation
            
            //// Number of added joints
            //int iNumberOfAddedJoints = 0;
            //// Number of added plates
            //int iNumberOfAddedPlates = 0;
            //
            //if (addJoints)
            //{
            //    foreach (CConnectionJointTypes joint in block.m_arrConnectionJoints)
            //    {
            //        m_arrConnectionJoints.Add(joint); // Add joint
            //
            //        iNumberOfAddedJoints++;
            //
            //        foreach (CPlate plate in joint.m_arrPlates)
            //        {
            //            iNumberOfAddedPlates++;
            //
            //            if (plate is CConCom_Plate_B_basic)
            //            {
            //                CConCom_Plate_B_basic basePlate = (CConCom_Plate_B_basic)plate;
            //
            //                foreach (CAnchor anchor in basePlate.AnchorArrangement.Anchors)
            //                {
            //                    iNumberOfAddedPlates++; // anchor.WasherBearing
            //                    iNumberOfAddedPlates++; // anchor.WasherPlateTop
            //                }
            //            }
            //        }
            //    }
            //}
            //
            //System.Diagnostics.Trace.WriteLine(
            //    "Number of added joints: " + iNumberOfAddedJoints + "\n" +
            //    "Number of added plates and washers: " + iNumberOfAddedPlates);
        }

        */
    }
}
