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

        int iMainColumnNo;
        int iRafterNo;
        int iEavesPurlinNo;
        int iGirtNoInOneFrame;
        int iPurlinNoInOneFrame;
        int iFrontColumnNoInOneFrame;
        int iBackColumnNoInOneFrame;
        int iFrontGirtsNoInOneFrame;
        int iBackGirtsNoInOneFrame;

        int[] iArrNumberOfNodesPerFrontColumn;
        int[] iArrNumberOfNodesPerBackColumn;
        int iOneColumnGirtNo;

        public CModel_PFD_01_GR
            (
                float fH1_temp,
                float fW_temp,
                float fL1_temp,
                int iFrameNo_temp,
                float fH2_temp,
                float fDist_Girt_temp,
                float fDist_Purlin_temp,
                float fDist_FrontColumns_temp,
                float fBottomGirtPosition_temp,
                float fFrontFrameRakeAngle_temp_deg,
                float fBackFrameRakeAngle_temp_deg,
                List<PropertiesToInsertOpening> doorBlocksPropertiesToInsert,
                List<PropertiesToInsertOpening> windowBlocksPropertiesToInsert,
                List<DoorProperties> doorBlocksProperties,
                List<WindowProperties> windowBlocksProperties,
                CCalcul_1170_1 generalLoad,
                CCalcul_1170_2 wind,
                CCalcul_1170_3 snow,
                CCalcul_1170_5 eq,
                bool bGenerateNodalLoads,
                bool bGenerateLoadsOnMembers,
                bool bGenerateLoadsOnPurlinsAndGirts,
                bool bGenerateLoadsOnFrameMembers,
                bool bGenerateSurfaceLoads
            )
        {
            fH1_frame = fH1_temp;
            fW_frame = fW_temp;
            fL1_frame = fL1_temp;
            iFrameNo = iFrameNo_temp;
            fH2_frame = fH2_temp;

            fL_tot = (iFrameNo - 1) * fL1_frame;

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

            const int iEavesPurlinNoInOneFrame = 2;
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
            m_arrMat[(int)EMemberGroupNames.eMainColumn] = new CMat_03_00(1, "G550", 200e+6f, 0.3f, 0.1f, 550e+6f, 550e+6f);
            m_arrMat[(int)EMemberGroupNames.eRafter] = new CMat_03_00(2, "G550", 200e+6f, 0.3f, 0.1f, 550e+6f, 550e+6f);
            m_arrMat[(int)EMemberGroupNames.eMainColumn_EF] = new CMat_03_00(3, "G550", 200e+6f, 0.3f, 0.1f, 550e+6f, 550e+6f);
            m_arrMat[(int)EMemberGroupNames.eRafter_EF] = new CMat_03_00(4, "G550", 200e+6f, 0.3f, 0.1f, 550e+6f, 550e+6f);
            m_arrMat[(int)EMemberGroupNames.eEavesPurlin] = new CMat_03_00(5, "G550", 200e+6f, 0.3f, 0.1f, 550e+6f, 550e+6f);
            m_arrMat[(int)EMemberGroupNames.eGirtWall] = new CMat_03_00(6, "G550", 200e+6f, 0.3f, 0.1f, 550e+6f, 550e+6f);
            m_arrMat[(int)EMemberGroupNames.ePurlin] = new CMat_03_00(7, "G550", 200e+6f, 0.3f, 0.1f, 550e+6f, 550e+6f);
            m_arrMat[(int)EMemberGroupNames.eFrontColumn] = new CMat_03_00(8, "G550", 200e+6f, 0.3f, 0.1f, 550e+6f, 550e+6f);
            m_arrMat[(int)EMemberGroupNames.eBackColumn] = new CMat_03_00(9, "G550", 200e+6f, 0.3f, 0.1f, 550e+6f, 550e+6f);
            m_arrMat[(int)EMemberGroupNames.eFrontGirt] = new CMat_03_00(10, "G550", 200e+6f, 0.3f, 0.1f, 550e+6f, 550e+6f);
            m_arrMat[(int)EMemberGroupNames.eBackGirt] = new CMat_03_00(11, "G550", 200e+6f, 0.3f, 0.1f, 550e+6f, 550e+6f);

            // Cross-sections
            // CrSc List - CrSc Array - Fill Data of Cross-sections Array

            // TODO Ondrej - Nastavit objekt prierezu podla databazy models, tabulka KitsetGableRoofEnclosed alebo KitsetGableRoofEnclosedCrscID
            // Napojit na GUI
            m_arrCrSc[(int)EMemberGroupNames.eMainColumn] = new CCrSc_3_63020_BOX(1, 0.63f, 0.2f, 0.00195f, 0.00195f, Colors.Chocolate);       // Main Column
            m_arrCrSc[(int)EMemberGroupNames.eRafter] = new CCrSc_3_63020_BOX(2, 0.63f, 0.2f, 0.00195f, 0.00195f, Colors.Green);               // Rafter
            m_arrCrSc[(int)EMemberGroupNames.eMainColumn_EF] = new CCrSc_3_63020_BOX(3, 0.63f, 0.2f, 0.00195f, 0.00195f, Colors.DarkOrchid);   // Main Column - Edge Frame
            m_arrCrSc[(int)EMemberGroupNames.eRafter_EF] = new CCrSc_3_63020_BOX(4, 0.63f, 0.2f, 0.00195f, 0.00195f, Colors.GreenYellow);      // Rafter - Edge Frame
            m_arrCrSc[(int)EMemberGroupNames.eEavesPurlin] = new CCrSc_3_50020_C(5, 0.5f, 0.2f, 0.00195f, Colors.DarkCyan);                    // Eaves Purlin
            m_arrCrSc[(int)EMemberGroupNames.eGirtWall] = new CCrSc_3_270XX_C(6, 0.27f, 0.07f, 0.00115f, Colors.Orange);                       // Girt - Wall
            m_arrCrSc[(int)EMemberGroupNames.ePurlin] = new CCrSc_3_270XX_C(7, 0.27f, 0.07f, 0.00095f, Colors.SlateBlue);                      // Purlin
            m_arrCrSc[(int)EMemberGroupNames.eFrontColumn] = new CCrSc_3_270XX_C_NESTED(8, 0.29f, 0.071f, 0.00115f, Colors.BlueViolet);        // Front Column
            m_arrCrSc[(int)EMemberGroupNames.eBackColumn] = new CCrSc_3_270XX_C_NESTED(9, 0.29f, 0.071f, 0.00115f, Colors.BlueViolet);         // Back Column
            m_arrCrSc[(int)EMemberGroupNames.eFrontGirt] = new CCrSc_3_270XX_C(10, 0.27f, 0.07f, 0.00115f, Colors.Brown);                       // Front Girt
            m_arrCrSc[(int)EMemberGroupNames.eBackGirt] = new CCrSc_3_270XX_C(11, 0.27f, 0.07f, 0.00095f, Colors.YellowGreen);                  // Back Girt

            // Member Groups
            listOfModelMemberGroups = new List<CMemberGroup>(11);

            CDatabaseComponents database_temp = new CDatabaseComponents(); // TODO - Ondrej - prerobit triedu na nacitanie z databazy
            // See UC component list
            listOfModelMemberGroups.Add(new CMemberGroup(1, database_temp.arr_Member_Types_Prefix[(int)EMemberType_FS.eMC, 1], m_arrCrSc[(int)EMemberGroupNames.eMainColumn], 0));
            listOfModelMemberGroups.Add(new CMemberGroup(2, database_temp.arr_Member_Types_Prefix[(int)EMemberType_FS.eMR, 1], m_arrCrSc[(int)EMemberGroupNames.eRafter], 0));
            listOfModelMemberGroups.Add(new CMemberGroup(3, database_temp.arr_Member_Types_Prefix[(int)EMemberType_FS.eEC, 1], m_arrCrSc[(int)EMemberGroupNames.eMainColumn_EF], 0));
            listOfModelMemberGroups.Add(new CMemberGroup(4, database_temp.arr_Member_Types_Prefix[(int)EMemberType_FS.eER, 1], m_arrCrSc[(int)EMemberGroupNames.eRafter_EF], 0));
            listOfModelMemberGroups.Add(new CMemberGroup(5, database_temp.arr_Member_Types_Prefix[(int)EMemberType_FS.eEP, 1], m_arrCrSc[(int)EMemberGroupNames.eEavesPurlin], 0));
            listOfModelMemberGroups.Add(new CMemberGroup(6, database_temp.arr_Member_Types_Prefix[(int)EMemberType_FS.eG, 1], m_arrCrSc[(int)EMemberGroupNames.eGirtWall], 0));
            listOfModelMemberGroups.Add(new CMemberGroup(7, database_temp.arr_Member_Types_Prefix[(int)EMemberType_FS.eP, 1], m_arrCrSc[(int)EMemberGroupNames.ePurlin], 0));
            listOfModelMemberGroups.Add(new CMemberGroup(8, database_temp.arr_Member_Types_Prefix[(int)EMemberType_FS.eC, 1] + " - Front Side", m_arrCrSc[(int)EMemberGroupNames.eFrontColumn], 0));
            listOfModelMemberGroups.Add(new CMemberGroup(9, database_temp.arr_Member_Types_Prefix[(int)EMemberType_FS.eC, 1] + " - Back Side", m_arrCrSc[(int)EMemberGroupNames.eBackColumn], 0));
            listOfModelMemberGroups.Add(new CMemberGroup(10, database_temp.arr_Member_Types_Prefix[(int)EMemberType_FS.eG, 1] + " - Front Side", m_arrCrSc[(int)EMemberGroupNames.eFrontGirt], 0));
            listOfModelMemberGroups.Add(new CMemberGroup(11, database_temp.arr_Member_Types_Prefix[(int)EMemberType_FS.eG, 1] + " - Back Side", m_arrCrSc[(int)EMemberGroupNames.eBackGirt], 0));

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
            CIntermediateTransverseSupport forkSupport = new CIntermediateTransverseSupport(1, EITSType.eBothFlanges,0);
            m_arrIntermediateTransverseSupports[0] = forkSupport;

            // Fly bracing
            bool bUseFlyBracingPlates = true; // Use fly bracing plates in purlin to rafter joint

            // TODO - poziciu fly bracing - kazda xx purlin nastavovat v GUI, alebo umoznit urcit automaticky, napr. cca tak aby bola vdialenost medzi fly bracing rovna L1

            int iEveryXXPurlin = Math.Max(0, (int)(fL1_frame / fDist_Purlin));
            //int iEveryXXPurlin = 3; // Index of purlin 1 - every, 2 - every second purlin, 3 - every third purlin

            // Limit pre poziciu horneho nosnika, mala by to byt polovica suctu vysky edge (eave) purlin h a sirky nosnika b (neberie sa h pretoze nosnik je otoceny o 90 stupnov)
            fUpperGirtLimit = (float)(m_arrCrSc[(int)EMemberGroupNames.eEavesPurlin].h + m_arrCrSc[(int)EMemberGroupNames.eGirtWall].b);

            // Limit pre poziciu horneho nosnika (front / back girt) na prednej alebo zadnej stene budovy
            // Nosnik alebo pripoj nosnika nesmie zasahovat do prievlaku (rafter)

            fz_UpperLimitForFrontGirts = (float)((0.5 * m_arrCrSc[(int)EMemberGroupNames.eRafter].h) / Math.Cos(fRoofPitch_rad) + 0.5f * m_arrCrSc[(int)EMemberGroupNames.eFrontGirt].b);
            fz_UpperLimitForBackGirts = (float)((0.5 * m_arrCrSc[(int)EMemberGroupNames.eRafter].h) / Math.Cos(fRoofPitch_rad) + 0.5f * m_arrCrSc[(int)EMemberGroupNames.eBackGirt].b);

            bool bGenerateGirts = true;
            if (bGenerateGirts)
            {
                iOneColumnGirtNo = (int)((fH1_frame - fUpperGirtLimit - fBottomGirtPosition) / fDist_Girt) + 1;
                iGirtNoInOneFrame = 2 * iOneColumnGirtNo;
            }

            float fFirstPurlinPosition = fDist_Purlin;
            float fRafterLength = MathF.Sqrt(MathF.Pow2(fH2_frame - fH1_frame) + MathF.Pow2(0.5f * fW_frame));

            int iOneRafterPurlinNo = 0;
            iPurlinNoInOneFrame = 0;

            bool bGeneratePurlins = true;
            if (bGeneratePurlins)
            {
                iOneRafterPurlinNo = (int)((fRafterLength - fFirstPurlinPosition) / fDist_Purlin) + 1;
                iPurlinNoInOneFrame = 2 * iOneRafterPurlinNo;
            }

            int iOneRafterFrontColumnNo = 0;
            iFrontColumnNoInOneFrame = 0;

            bool bGenerateFrontColumns = true;
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

            int iOneRafterBackColumnNo = 0;
            iBackColumnNoInOneFrame = 0;

            fDist_BackColumns = fDist_FrontColumns; // Todo Temporary - umoznit ine roztece medzi zadnymi a prednymi stplmi

            bool bGenerateBackColumns = true;
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

            bool bGenerateFrontGirts = true;

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

            // Number of Nodes - Back Girts
            int iBackIntermediateColumnNodesForGirtsOneRafterNo = 0;
            int iBackIntermediateColumnNodesForGirtsOneFrameNo = 0;
            iBackGirtsNoInOneFrame = 0;
            iArrNumberOfNodesPerBackColumn = new int[iOneRafterBackColumnNo];

            bool bGenerateBackGirts = true;

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
            float fEavesPurlinStart = -0.5f * (float)m_arrCrSc[(int)EMemberGroupNames.eRafter].b - fCutOffOneSide;       // Just in case that cross-section of rafter is symmetric about z-z
            float fEavesPurlinEnd = -0.5f * (float)m_arrCrSc[(int)EMemberGroupNames.eRafter].b - fCutOffOneSide;         // Just in case that cross-section of rafter is symmetric about z-z
            float fGirtStart = -0.5f * (float)m_arrCrSc[(int)EMemberGroupNames.eMainColumn].b - fCutOffOneSide;          // Just in case that cross-section of main column is symmetric about z-z
            float fGirtEnd = -0.5f * (float)m_arrCrSc[(int)EMemberGroupNames.eMainColumn].b - fCutOffOneSide;            // Just in case that cross-section of main column is symmetric about z-z
            float fPurlinStart = -0.5f * (float)m_arrCrSc[(int)EMemberGroupNames.eRafter].b - fCutOffOneSide;            // Just in case that cross-section of rafter is symmetric about z-z
            float fPurlinEnd = -0.5f * (float)m_arrCrSc[(int)EMemberGroupNames.eRafter].b - fCutOffOneSide;              // Just in case that cross-section of rafter is symmetric about z-z
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
                // Rafters
                m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * (iFrameNodesNo - 1) + 1] = new CMember((i * iEavesPurlinNoInOneFrame) + i * (iFrameNodesNo - 1) + 2, m_arrNodes[i * iFrameNodesNo + 1], m_arrNodes[i * iFrameNodesNo + 2], m_arrCrSc[iCrscRafterIndex], eRafterType, null, null, fRafterStart, fRafterEnd, 0f, 0);
                m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * (iFrameNodesNo - 1) + 2] = new CMember((i * iEavesPurlinNoInOneFrame) + i * (iFrameNodesNo - 1) + 3, m_arrNodes[i * iFrameNodesNo + 2], m_arrNodes[i * iFrameNodesNo + 3], m_arrCrSc[iCrscRafterIndex], eRafterType, null, null, fRafterEnd, fRafterStart, 0f, 0);
                // Main Column
                m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * (iFrameNodesNo - 1) + 3] = new CMember((i * iEavesPurlinNoInOneFrame) + i * (iFrameNodesNo - 1) + 4, m_arrNodes[i * iFrameNodesNo + 3], m_arrNodes[i * iFrameNodesNo + 4], m_arrCrSc[iCrscColumnIndex], eColumnType, null, null, fMainColumnEnd, fMainColumnStart, 0f, 0);

                // Eaves Purlins
                if (i < (iFrameNo - 1))
                {
                    m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * (iFrameNodesNo - 1) + 4] = new CMember((i * iEavesPurlinNoInOneFrame) + i * (iFrameNodesNo - 1) + 5, m_arrNodes[i * iFrameNodesNo + 1], m_arrNodes[(i + 1) * iFrameNodesNo + 1], m_arrCrSc[(int)EMemberGroupNames.eEavesPurlin], EMemberType_FS.eEP, eccentricityEavePurlin, eccentricityEavePurlin, fEavesPurlinStart, fEavesPurlinEnd, (float)Math.PI, 0);
                    m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * (iFrameNodesNo - 1) + 5] = new CMember((i * iEavesPurlinNoInOneFrame) + i * (iFrameNodesNo - 1) + 6, m_arrNodes[i * iFrameNodesNo + 3], m_arrNodes[(i + 1) * iFrameNodesNo + 3], m_arrCrSc[(int)EMemberGroupNames.eEavesPurlin], EMemberType_FS.eEP, eccentricityEavePurlin, eccentricityEavePurlin, fEavesPurlinStart, fEavesPurlinEnd, 0f, 0);
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
                    }

                    for (int j = 0; j < iOneColumnGirtNo; j++)
                    {
                        m_arrMembers[i_temp_numberofMembers + i * iGirtNoInOneFrame + iOneColumnGirtNo + j] = new CMember(i_temp_numberofMembers + i * iGirtNoInOneFrame + iOneColumnGirtNo + j + 1, m_arrNodes[i_temp_numberofNodes + i * iGirtNoInOneFrame + iOneColumnGirtNo + j], m_arrNodes[i_temp_numberofNodes + (i + 1) * iGirtNoInOneFrame + iOneColumnGirtNo + j], m_arrCrSc[(int)EMemberGroupNames.eGirtWall], EMemberType_FS.eG, eccentricityGirtRight_XB, eccentricityGirtRight_XB, fGirtStart, fGirtEnd, fGirtsRotation, 0);
                        RotateFrontOrBackFrameNodeAboutZ(m_arrNodes[i_temp_numberofMembers + i * iGirtNoInOneFrame + iOneColumnGirtNo + j]);
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
                // Define fly bracing position on rafter // Tento kod moze byt vyssie
                if (bUseFlyBracingPlates && iEveryXXPurlin > 0)
                {
                    for (int i = 0; i < iFrameNo; i++) // Each frame
                    {
                        List<CIntermediateTransverseSupport> lTransverseSupportGroup_Rafter = new List<CIntermediateTransverseSupport>();
                        float fFirstFlyBracePosition = fFirstPurlinPosition + (iEveryXXPurlin - 1) * fDist_Purlin;
                        int iNumberOfFlyBracesOnRafter = fFirstFlyBracePosition < fRafterLength ? (int)((fRafterLength - fFirstFlyBracePosition) / (iEveryXXPurlin * fDist_Purlin)) + 1 : 0;

                        for (int j = 0; j < iNumberOfFlyBracesOnRafter; j++) // Each fly brace
                        {
                            float fxLocationOfFlyBrace = fFirstFlyBracePosition + (j * iEveryXXPurlin) * fDist_Purlin;

                            if(fxLocationOfFlyBrace < fRafterLength)
                             lTransverseSupportGroup_Rafter.Add(new CIntermediateTransverseSupport(j + 1, EITSType.eBothFlanges, fxLocationOfFlyBrace / fRafterLength, fxLocationOfFlyBrace, 0));
                            // TODO - To Ondrej, nie som si isty ci mam v kazdej podpore CIntermediateTransverseSupport ukladat jej poziciu (aktualny stav) alebo ma CMember nie list CIntermediateTransverseSupport ale list nejakych struktur (x, CIntermediateTransverseSupport), takze x miesta budu definovane v tejto strukture v objekte CMember a samotny objekt CIntermediateTransverseSupport nebude vediet kde je
                        }

                        if (lTransverseSupportGroup_Rafter.Count > 0)
                        {
                            int iLeftRafterIndex = (i * iEavesPurlinNoInOneFrame) + i * (iFrameNodesNo - 1) + 1;
                            int iRightRafterIndex = (i * iEavesPurlinNoInOneFrame) + i * (iFrameNodesNo - 1) + 2;

                            List<CSegment_LTB> LTB_segment_group_rafter = new List<CSegment_LTB>();

                            // Create lateral-torsional buckling segments
                            for (int j = 0; j < lTransverseSupportGroup_Rafter.Count + 1; j++)
                            {
                                // Number of segments = number of intermediate supports + 1 - type BothFlanges
                                // TODO - doriesit ako generovat segmenty ak nie su vsetky lateral supports typu BothFlanges
                                // Najprv by sa mal najst pocet podpor s BothFlanges, z toho urcit pocet segmentov a zohladnovat len tie coordinates x,
                                // ktore sa vztahuju na podpory s BothFlanges

                                float fSegmentStart_abs = 0;
                                float fSegmentEnd_abs = fRafterLength;

                                if (j == 0) // First Segment
                                {
                                    fSegmentStart_abs = 0f;
                                    fSegmentEnd_abs = lTransverseSupportGroup_Rafter[j].Fx_position_abs;
                                }
                                else if(j < lTransverseSupportGroup_Rafter.Count)
                                {
                                    fSegmentStart_abs = lTransverseSupportGroup_Rafter[j-1].Fx_position_abs;
                                    fSegmentEnd_abs = lTransverseSupportGroup_Rafter[j].Fx_position_abs;
                                }
                                else // Last
                                {
                                    fSegmentStart_abs = lTransverseSupportGroup_Rafter[j-1].Fx_position_abs;
                                    fSegmentEnd_abs = fRafterLength;
                                }

                                CSegment_LTB segment = new CSegment_LTB(j + 1, false, fSegmentStart_abs, fSegmentEnd_abs, fRafterLength);

                                LTB_segment_group_rafter.Add(segment);
                            }

                            // Assign transverse support group to the rafter
                            // Left Rafter
                            m_arrMembers[iLeftRafterIndex].IntermediateTransverseSupportGroup = lTransverseSupportGroup_Rafter;
                            m_arrMembers[iLeftRafterIndex].LTBSegmentGroup = LTB_segment_group_rafter;
                            // Right Rafter
                            m_arrMembers[iRightRafterIndex].IntermediateTransverseSupportGroup = lTransverseSupportGroup_Rafter; // TODO - neviem ci pre pravy rafter nie su suradnice opacne orientovane , takze pouzit L - x
                            m_arrMembers[iRightRafterIndex].LTBSegmentGroup = LTB_segment_group_rafter; // TODO - neviem ci pre pravy rafter nie su suradnice opacne orientovane , takze pouzit L - x
                        }
                    }
                }

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
                    }

                    for (int j = 0; j < iOneRafterPurlinNo; j++)
                    {
                        m_arrMembers[i_temp_numberofMembers + i * iPurlinNoInOneFrame + iOneRafterPurlinNo + j] = new CMember(i_temp_numberofMembers + i * iPurlinNoInOneFrame + iOneRafterPurlinNo + j + 1, m_arrNodes[i_temp_numberofNodes + i * iPurlinNoInOneFrame + iOneRafterPurlinNo + j], m_arrNodes[i_temp_numberofNodes + (i + 1) * iPurlinNoInOneFrame + iOneRafterPurlinNo + j], m_arrCrSc[(int)EMemberGroupNames.ePurlin], EMemberType_FS.eP, eccentricityPurlin, eccentricityPurlin, fPurlinStart, fPurlinEnd, fRoofPitch_rad, 0);
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
                AddColumnsMembers(i_temp_numberofNodes, i_temp_numberofMembers, iOneRafterBackColumnNo, iFrontColumnNoInOneFrame, eccentricityColumnFront_Z, fFrontColumnStart, fFrontColumnEnd, m_arrCrSc[(int)EMemberGroupNames.eFrontColumn], fColumnsRotation);
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
                AddColumnsMembers(i_temp_numberofNodes, i_temp_numberofMembers, iOneRafterBackColumnNo, iBackColumnNoInOneFrame, eccentricityColumnBack_Z, fBackColumnStart, fBackColumnEnd, m_arrCrSc[(int)EMemberGroupNames.eBackColumn], fColumnsRotation);
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
                AddFrontOrBackGirtsMembers(iFrameNodesNo, iOneRafterFrontColumnNo, iArrNumberOfNodesPerFrontColumn, i_temp_numberofNodes, i_temp_numberofMembers, iFrontIntermediateColumnNodesForGirtsOneRafterNo, iFrontIntermediateColumnNodesForGirtsOneFrameNo, 0, fDist_Girt, eccentricityGirtFront_Y0, fFrontGirtStart_MC, fFrontGirtStart, fFrontGirtEnd, m_arrCrSc[(int)EMemberGroupNames.eFrontGirt], fColumnsRotation);
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
                AddFrontOrBackGirtsMembers(iFrameNodesNo, iOneRafterBackColumnNo, iArrNumberOfNodesPerBackColumn, i_temp_numberofNodes, i_temp_numberofMembers, iBackIntermediateColumnNodesForGirtsOneRafterNo, iBackIntermediateColumnNodesForGirtsOneFrameNo, iGirtNoInOneFrame * (iFrameNo - 1), fDist_Girt, eccentricityGirtBack_YL, fBackGirtStart_MC, fBackGirtStart, fBackGirtEnd, m_arrCrSc[(int)EMemberGroupNames.eBackGirt], fColumnsRotation);
            }

            #region Joints
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
                    m_arrConnectionJoints.Add(new CConnectionJoint_A001(m_arrNodes[i * 5 + 2], m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * 4 + 1], m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * 4 + 2], fRoofPitch_rad, 2 * (float)m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * 4 + 1].CrScStart.h, ft_rafter_joint_plate, i == 0 ? fFrontFrameRakeAngle_temp_deg : fBackFrameRakeAngle_temp_deg, true));
                else //if(i< (iFrameNo - 1) // Intermediate frame
                    m_arrConnectionJoints.Add(new CConnectionJoint_A001(m_arrNodes[i * 5 + 2], m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * 4 + 1], m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * 4 + 2], fRoofPitch_rad, 2 * (float)m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * 4 + 1].CrScStart.h, ft_rafter_joint_plate, 0, true));
            }

            float ft_knee_joint_plate = 0.003f; // m

            // Knee Joints 1
            for (int i = 0; i < iFrameNo; i++)
            {
                if (i == 0 || i == (iFrameNo - 1)) // Front or Last Frame
                    m_arrConnectionJoints.Add(new CConnectionJoint_B001(m_arrNodes[i * 5 + 1], m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * 4], m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * 4 + 1], fRoofPitch_rad, 1.1f * (float)m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * 4].CrScStart.h, 2 * (float)m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * 4 + 1].CrScStart.h, ft_knee_joint_plate, ft_rafter_joint_plate, i == 0 ? fFrontFrameRakeAngle_temp_deg : fBackFrameRakeAngle_temp_deg, true));
                else //if(i< (iFrameNo - 1) // Intermediate frame
                    m_arrConnectionJoints.Add(new CConnectionJoint_B001(m_arrNodes[i * 5 + 1], m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * 4], m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * 4 + 1], fRoofPitch_rad, 1.1f * (float)m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * 4].CrScStart.h, 2 * (float)m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * 4 + 1].CrScStart.h, ft_knee_joint_plate, ft_rafter_joint_plate, i == 0 ? fFrontFrameRakeAngle_temp_deg : fBackFrameRakeAngle_temp_deg, true));
            }

            // Knee Joints 2
            for (int i = 0; i < iFrameNo; i++)
            {
                if (i == 0 || i == (iFrameNo - 1)) // Front or Last Frame
                    m_arrConnectionJoints.Add(new CConnectionJoint_B001(m_arrNodes[i * 5 + 3], m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * 4 + 3], m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * 4 + 2], fRoofPitch_rad, 1.1f * (float)m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * 4 + 3].CrScStart.h, 2 * (float)m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * 4 + 2].CrScStart.h, ft_knee_joint_plate, ft_rafter_joint_plate, i == 0 ? fFrontFrameRakeAngle_temp_deg : fBackFrameRakeAngle_temp_deg, true));
                else //if(i< (iFrameNo - 1) // Intermediate frame
                    m_arrConnectionJoints.Add(new CConnectionJoint_B001(m_arrNodes[i * 5 + 3], m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * 4 + 3], m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * 4 + 2], fRoofPitch_rad, 1.1f * (float)m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * 4 + 3].CrScStart.h, 2 * (float)m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * 4 + 2].CrScStart.h, ft_knee_joint_plate, ft_rafter_joint_plate, i == 0 ? fFrontFrameRakeAngle_temp_deg : fBackFrameRakeAngle_temp_deg, true));
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
                    if(iCurrentMemberIndex < iFirstPurlinInFrameRightSide)
                        iFirstPurlinOnCurrentSideIndex = iFirstPurlinInFrameLeftSide;
                    else
                        iFirstPurlinOnCurrentSideIndex = iFirstPurlinInFrameRightSide;

                    if (bUseFlyBracingPlates && iEveryXXPurlin > 0 && (iCurrentMemberIndex - iFirstPurlinOnCurrentSideIndex + 1)%iEveryXXPurlin == 0)
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

                    if (i < (int)(iFrontColumnNoInOneFrame / 2))
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
                    throw new ArgumentNullException("Main member is not assigned to the joint No.:"+ m_arrConnectionJoints[i].ID.ToString() + " Joint index in the list: " + i);
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

            #endregion

            #region Blocks

            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // Blocks
            // TODO - pokusny blok dveri, je potreba refaktorovat, napojit na GUI, vytvorit zoznam tychto objektov -> viacero dveri v budove na roznych poziciach a s roznymi parametrami
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            if (doorBlocksPropertiesToInsert != null && doorBlocksProperties != null)
            {
                if (doorBlocksPropertiesToInsert.Count == doorBlocksProperties.Count)
                {
                    for (int i = 0; i < doorBlocksProperties.Count; i++)
                    {
                        AddDoorBlock(doorBlocksPropertiesToInsert[i], doorBlocksProperties[i], 0.5f, fH1_frame);
                    }
                }
                else
                {
                    // Exception
                }
            }

            if (windowBlocksPropertiesToInsert != null && windowBlocksProperties != null)
            {
                if (windowBlocksPropertiesToInsert.Count == windowBlocksProperties.Count)
                {
                    for (int i = 0; i < windowBlocksProperties.Count; i++)
                    {
                        AddWindowBlock(windowBlocksPropertiesToInsert[i], windowBlocksProperties[i], 0.5f);
                    }
                }
                else
                {
                    // Exception
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

            // End of blocks
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            #endregion

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

                    m_arrNSupports[m_arrNSupports.Length-1] = new CNSupport(6, i + 1, m_arrNodes[i], bSupport1, 0);
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
                m_arrGOVolumes[0] = new CVolume(1, EVolumeShapeType.eShape3DPrism_8Edges, controlPoint_FloorSlab, fFloorSlab_aX, fFloorSlab_bY, fFloorSlab_h, new DiffuseMaterial(new SolidColorBrush (Colors.DarkGray)), true, 0);
            }
            #endregion

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
                    fDist_Purlin, fDist_Girt, fDist_FrontGirts, fSlopeFactor, m_arrLoadCases, generalLoad, wind, snow);
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

            // POKUS TODO 186
            #region Member Loads (girts, purlins, wind posts, door trimmers)
            // Generated from surface load (on girts and purlins)
            // TO Ondrej - 7.2.2019
            // Toto je pokus o generovanie prutoveho zatazenia z plosneho
            // Jedna sa o zoznamy prutov typu girts a typu purlins
            // Problem je v tom suradnice bodov rovin beriem z celej stavby, by sa mali preberat priamo z objektu CSLoad_FreeUniform.cs,
            // Mozes sa tymto insprirovat ale treba to vyladit tak aby sa CMLoad generovali vzdy len pre pruty ktore su pod CSurfaceLoad, pripadne generovat viacero CMLoad_24, ak je prut pod viacerymi CSurfaceLoads

            // TODO No. 54
            // tieto zoznamy sa maju nahradit funckiou v TODO 54 ktora ich vytvori pre jednotlive zatazovacie plochy zo suradnic ploch
            // Uvedomil som si ze to ma jeden nedostatok, member (napr. purlin) moze byt jednou castou pod jednou plochou a druhou castou pod druhou plochou
            // takze nie je spravne mat v ploche zoznam prutov ktore zatazuje
            // Member moze byt pod roznymi plochami, takze to nie je jednoznacne, musi to fungovat tak ze je zatazovacie plochy neobsahuju takyto zoznam, ale algoritmus vygeneruje CMLoad_21 alebo skupinu CMLoad_24 na jednom prute, pre pruty ktore su pod jednou plochou alebo pod viacerymi plochami
            // Generate linear Loads
            List<CMember> listOfPurlins = new List<CMember>(0);
            List<CMember> listOfEavePurlins = new List<CMember>(0);
            List<CMember> listOfGirts = new List<CMember>(0);

            List<CMember> listOfPurlinsLeftSide = new List<CMember>(0);
            List<CMember> listOfPurlinsRightSide = new List<CMember>(0);
            List<CMember> listOfEavePurlinsLeftSide = new List<CMember>(0);
            List<CMember> listOfEavePurlinsRightSide = new List<CMember>(0);
            List<CMember> listOfGirtsLeftSide = new List<CMember>(0);
            List<CMember> listOfGirtsRightSide = new List<CMember>(0);
            List<CMember> listOfGirtsFrontSide = new List<CMember>(0);
            List<CMember> listOfGirtsBackSide = new List<CMember>(0);

            Point3D p1;
            Point3D p2;
            Point3D p3;

            // Roof Surface Geometry
            // Control Points
            CPoint pRoofFrontLeft = new CPoint(0, 0, 0, fH1_frame, 0);
            CPoint pRoofFrontApex = new CPoint(0, 0.5f * fW_frame, 0, fH2_frame, 0);
            CPoint pRoofFrontRight = new CPoint(0, fW_frame, 0, fH1_frame, 0);
            CPoint pRoofBackLeft = new CPoint(0, 0, fL_tot, fH1_frame, 0);
            CPoint pRoofBackApex = new CPoint(0, 0.5f * fW_frame, fL_tot, fH2_frame, 0);
            CPoint pRoofBackRight = new CPoint(0, fW_frame, fL_tot, fH1_frame, 0);

            // Wall Surface Geometry
            CPoint pWallFrontLeft = new CPoint(0, 0, 0, 0, 0);
            CPoint pWallFrontRight = new CPoint(0, fW_frame, 0, 0, 0);
            CPoint pWallBackRight = new CPoint(0, fW_frame, fL_tot, 0, 0);
            CPoint pWallBackLeft = new CPoint(0, 0, fL_tot, 0, 0);

            // TODO No 49 and 50 - in work, naplnit zoznamy prutov ktore lezia v rovine definujucej zatazenie, presnost 1 mm
            // Uvedomil som si ze to ma jeden nedostatok, member (napr. purlin) moze byt jednou castou pod jednou plochou a druhou castou pod druhou plochou
            // takze nie je spravne mat v ploche zoznam prutov ktore zatazuje

            // Loading width of member (Zatazovacia sirka pruta)
            float fLoadingWidthPurlin = fDist_Purlin;
            float fLoadingWidthEdgePurlin_Roof = 0.5f * fDist_Purlin;
            float fLoadingWidthEdgePurlin_Wall = 0.5f * fDist_Girt;
            float fLoadingWidthGirt = fDist_Girt;

            // TODO No. 54 - po implementacii tento cyklus odstranit a napojit metodu
            // Uvedomil som si ze to ma jeden nedostatok, member (napr. purlin) moze byt jednou castou pod jednou plochou a druhou castou pod druhou plochou
            // takze nie je spravne mat v ploche zoznam prutov ktore zatazuje
            // FillListOfMemberData z CSLoad_Free.cs

            foreach (CMember m in m_arrMembers)
            {
                ///////////////////////////////////////////////////////////////////////////////////////////////////
                // Girts
                ///////////////////////////////////////////////////////////////////////////////////////////////////

                // List of all girts
                if (m.EMemberType == EMemberType_FS.eG)
                    listOfGirts.Add(m);

                // TODO - Ondrej tieto suradnice by sa mali preberat priamo z objektu CSLoad_FreeUniform.cs, transformacna funkcia 
                // CreateTransformCoordGroup

                p1 = new Point3D(pWallFrontLeft.X, pWallFrontLeft.Y, pWallFrontLeft.Z);
                p2 = new Point3D(pRoofFrontLeft.X, pRoofFrontLeft.Y, pRoofFrontLeft.Z);
                p3 = new Point3D(pRoofBackLeft.X, pRoofBackLeft.Y, pRoofBackLeft.Z);

                // List of girts - left wall
                if (m.EMemberType == EMemberType_FS.eG && Drawing3D.MemberLiesOnPlane(p1, p2, p3, m, 0.001))
                    listOfGirtsLeftSide.Add(m);

                p1 = new Point3D(pWallFrontRight.X, pWallFrontRight.Y, pWallFrontRight.Z);
                p2 = new Point3D(pRoofFrontRight.X, pRoofFrontRight.Y, pRoofFrontRight.Z);
                p3 = new Point3D(pRoofBackRight.X, pRoofBackRight.Y, pRoofBackRight.Z);

                // List of girts - right wall
                if (m.EMemberType == EMemberType_FS.eG && Drawing3D.MemberLiesOnPlane(p1, p2, p3, m, 0.001))
                    listOfGirtsRightSide.Add(m);

                p1 = new Point3D(pWallFrontLeft.X, pWallFrontLeft.Y, pWallFrontLeft.Z);
                p2 = new Point3D(pWallFrontRight.X, pWallFrontRight.Y, pWallFrontRight.Z);
                p3 = new Point3D(pRoofFrontRight.X, pRoofFrontRight.Y, pRoofFrontRight.Z);

                // List of girts - front wall
                if (m.EMemberType == EMemberType_FS.eG && Drawing3D.MemberLiesOnPlane(p1, p2, p3, m, 0.001))
                    listOfGirtsFrontSide.Add(m);

                p1 = new Point3D(pWallBackLeft.X, pWallBackLeft.Y, pWallBackLeft.Z);
                p2 = new Point3D(pWallBackRight.X, pWallBackRight.Y, pWallBackRight.Z);
                p3 = new Point3D(pRoofBackRight.X, pRoofBackRight.Y, pRoofBackRight.Z);

                // List of girts - back wall
                if (m.EMemberType == EMemberType_FS.eG && Drawing3D.MemberLiesOnPlane(p1, p2, p3, m, 0.001))
                    listOfGirtsBackSide.Add(m);

                ///////////////////////////////////////////////////////////////////////////////////////////////////
                // Purlins
                ///////////////////////////////////////////////////////////////////////////////////////////////////

                // List of all purlins
                if (m.EMemberType == EMemberType_FS.eP)
                    listOfPurlins.Add(m);

                // List of all edge purlins
                if (m.EMemberType == EMemberType_FS.eEP)
                    listOfEavePurlins.Add(m);

                p1 = new Point3D(pRoofFrontLeft.X, pRoofFrontLeft.Y, pRoofFrontLeft.Z);
                p2 = new Point3D(pRoofFrontApex.X, pRoofFrontApex.Y, pRoofFrontApex.Z);
                p3 = new Point3D(pRoofBackApex.X, pRoofBackApex.Y, pRoofBackApex.Z);

                // List of purlins - left side of the roof
                if (m.EMemberType == EMemberType_FS.eP && Drawing3D.MemberLiesOnPlane(p1, p2, p3, m, 0.001))
                    listOfPurlinsLeftSide.Add(m);

                // List of edge purlins - left side of the roof (tento zoznam pouzit aj pre zatazenie lavej steny)
                if (m.EMemberType == EMemberType_FS.eEP && Drawing3D.MemberLiesOnPlane(p1, p2, p3, m, 0.001))
                    listOfEavePurlinsLeftSide.Add(m);

                p1 = new Point3D(pRoofFrontApex.X, pRoofFrontApex.Y, pRoofFrontApex.Z);
                p2 = new Point3D(pRoofFrontRight.X, pRoofFrontRight.Y, pRoofFrontRight.Z);
                p3 = new Point3D(pRoofBackRight.X, pRoofBackRight.Y, pRoofBackRight.Z);

                // List of purlins - right side of the roof
                if (m.EMemberType == EMemberType_FS.eP && Drawing3D.MemberLiesOnPlane(p1, p2, p3, m, 0.001))
                    listOfPurlinsRightSide.Add(m);

                // List of edge purlins - right side of the roof (tento zoznam pouzit aj pre zatazenie pravej steny)
                if (m.EMemberType == EMemberType_FS.eEP && Drawing3D.MemberLiesOnPlane(p1, p2, p3, m, 0.001))
                    listOfEavePurlinsRightSide.Add(m);
            }

            // TODO 50
            // Ukazka - purlin - imposed load
            // Preorganizovat properties v triedach surface load tak, aby sa dalo dostat k hodnote zatazenia a prenasobit vzdialenostou medzi vaznicami
            // Vypocitane zatazenie priradit prutom zo zoznamu listOfPurlins v Load Case v m_arrLoadCases[01]

            // TODO - Ondrej, pripravit staticku triedu a metody pre generovanie member load zo surface load v zlozke Loading
            // TODO 186 - To Ondrej - Tu je trieda CLoadGenerator, v ktorej by mohlo byt zapracovane generovanie zatazenia

            // Generator prutoveho zatazenia z plosneho zatazenia by mohol byt niekde stranou v tomto CExample je toto uz velmi vela
            // Pre urcenie spravneho znamienka generovaneho member load bude potrebne poznat uhol medzi normalou plochy definujucej zatazenie a osovym systemom pruta

            if(bGenerateLoadsOnMembers && bGenerateLoadsOnPurlinsAndGirts) // TODO - tu je mensi problem s tym ze ak je vypnute generovanie surface loads tak aj tieto zoznamy su prazdne kedze na surface loads zavisia, ak surface loads nie su vygenerovane, mali by sa dogenerovat
            CLoadGenerator.GenerateMemberLoads(m_arrLoadCases, listOfPurlins, fDist_Purlin);

            #endregion

            #region Frame Member Loads
            // Frame Member Loads
            if (bGenerateLoadsOnMembers && bGenerateLoadsOnFrameMembers)
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

                loadGenerator.GenerateLoadsOnFrames();
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
            //generator.GenerateULS();
            //generator.GenerateSLS();
            //generator.WriteCombinationsLoadCases();
            //generator.WritePermutations();
            //generator.WriteCombinations();

            //m_arrLoadCombs = new CLoadCombination[generator.Combinations.Count];
            //for (int i = 0; i < m_arrLoadCombs.Length; i++)
            //    m_arrLoadCombs[i] = generator.Combinations[i];
            m_arrLoadCombs = generator.Combinations.ToArray();
            #endregion

            #region Limit states
            // Limit States
            m_arrLimitStates = new CLimitState[3];
            m_arrLimitStates[0] = new CLimitState("Ultimate Limit State - Stability", ELSType.eLS_ULS);
            m_arrLimitStates[1] = new CLimitState("Ultimate Limit State - Strength" , ELSType.eLS_ULS);
            m_arrLimitStates[2] = new CLimitState("Serviceability Limit State"      , ELSType.eLS_SLS);
            #endregion

            AddMembersToMemberGroupsLists();
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

        public void AddColumnsMembers(int i_temp_numberofNodes, int i_temp_numberofMembers, int iOneRafterColumnNo, int iColumnNoInOneFrame, CMemberEccentricity eccentricityColumn, float fColumnAlignmentStart, float fColumnAlignmentEnd, CCrSc section, float fMemberRotation)
        {
            // Members - Columns
            for (int i = 0; i < iOneRafterColumnNo; i++)
            {
                m_arrMembers[i_temp_numberofMembers + i] = new CMember(i_temp_numberofMembers + i + 1, m_arrNodes[i_temp_numberofNodes + i], m_arrNodes[i_temp_numberofNodes + iColumnNoInOneFrame + i], section, EMemberType_FS.eC, eccentricityColumn, eccentricityColumn, fColumnAlignmentStart, fColumnAlignmentEnd, fMemberRotation, 0);
            }

            for (int i = 0; i < iOneRafterColumnNo; i++)
            {
                m_arrMembers[i_temp_numberofMembers + iOneRafterColumnNo + i] = new CMember(i_temp_numberofMembers + iOneRafterColumnNo + i + 1, m_arrNodes[i_temp_numberofNodes + iOneRafterColumnNo + i], m_arrNodes[i_temp_numberofNodes + iColumnNoInOneFrame + iOneRafterColumnNo + i], section, EMemberType_FS.eC, eccentricityColumn, eccentricityColumn, fColumnAlignmentStart, fColumnAlignmentEnd, fMemberRotation, 0);
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
            float fDist_Girts, CMemberEccentricity eGirtEccentricity, float fGirtStart_MC, float fGirtStart, float fGirtEnd, CCrSc section, float fMemberRotation)
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
                    }

                    iTemp += iOneColumnGirtNo_temp;
                }
                else if (i < iOneRafterColumnNo) // Other sessions
                {
                    for (int j = 0; j < iArrNumberOfNodesPerColumn[i - 1]; j++)
                    {
                        m_arrMembers[i_temp_numberofMembers + iTemp + j] = new CMember(i_temp_numberofMembers + iTemp + j + 1, m_arrNodes[i_temp_numberofNodes + iTemp2 + j], m_arrNodes[i_temp_numberofNodes + iArrNumberOfNodesPerColumn[i - 1] + iTemp2 + j], section, EMemberType_FS.eG, eGirtEccentricity, eGirtEccentricity, fGirtStart, fGirtEnd, fMemberRotation, 0);
                    }

                    iTemp2 += iArrNumberOfNodesPerColumn[i - 1];
                    iTemp += iArrNumberOfNodesPerColumn[i - 1];
                }
                else // Last session - prechadza cez stred budovy
                {
                    for (int j = 0; j < iArrNumberOfNodesPerColumn[i - 1]; j++)
                    {
                        m_arrMembers[i_temp_numberofMembers + iTemp + j] = new CMember(i_temp_numberofMembers + iTemp + j + 1, m_arrNodes[i_temp_numberofNodes + iTemp2 + j], m_arrNodes[i_temp_numberofNodes + iIntermediateColumnNodesForGirtsOneFrameNo - iArrNumberOfNodesPerColumn[iOneRafterColumnNo - 1] + j], section, EMemberType_FS.eG, eGirtEccentricity, eGirtEccentricity, fGirtStart, fGirtEnd, fMemberRotation, 0);
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
                    }

                    iTemp += iOneColumnGirtNo_temp;
                }
                else // Other sessions (not in the middle)
                {
                    for (int j = 0; j < iArrNumberOfNodesPerColumn[i - 1]; j++)
                    {
                        m_arrMembers[i_temp_numberofMembers + iNumberOfMembers_temp + iTemp + j] = new CMember(i_temp_numberofMembers + iNumberOfMembers_temp + iTemp + j + 1, m_arrNodes[i_temp_numberofNodes + iIntermediateColumnNodesForGirtsOneRafterNo + iTemp2 + j], m_arrNodes[i_temp_numberofNodes + iIntermediateColumnNodesForGirtsOneRafterNo + iArrNumberOfNodesPerColumn[i - 1] + iTemp2 + j], section, EMemberType_FS.eG, eGirtEccentricity, eGirtEccentricity, fGirtStart, fGirtEnd, -fMemberRotation, 0);
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

        public void AddDoorBlock(PropertiesToInsertOpening insertprop, DoorProperties prop, float fLimitDistanceFromColumn, float fSideWallHeight)
        {
            CMember mReferenceGirt;
            CMember mColumn;
            CBlock_3D_001_DoorInBay door;
            CPoint pControlPointBlock;
            float fBayWidth;
            int iFirstMemberToDeactivate;
            bool bIsReverseSession;
            bool bIsFirstBayInFrontorBackSide;
            bool bIsLastBayInFrontorBackSide;

            DeterminateBasicPropertiesToInsertBlock(insertprop, out mReferenceGirt, out mColumn, out pControlPointBlock, out fBayWidth, out iFirstMemberToDeactivate, out bIsReverseSession, out bIsFirstBayInFrontorBackSide, out bIsLastBayInFrontorBackSide);

            door = new CBlock_3D_001_DoorInBay(
                insertprop.sBuildingSide,
                prop.fDoorsHeight,
                prop.fDoorsWidth,
                prop.fDoorCoordinateXinBlock,
                fLimitDistanceFromColumn,
                fBottomGirtPosition,
                fDist_Girt,
                mReferenceGirt,
                mColumn,
                fBayWidth,
                fSideWallHeight,
                bIsReverseSession,
                bIsFirstBayInFrontorBackSide,
                bIsLastBayInFrontorBackSide);

            AddDoorOrWindowBlockProperties(pControlPointBlock, iFirstMemberToDeactivate, door);
        }

        public void AddWindowBlock(PropertiesToInsertOpening insertprop, WindowProperties prop, float fLimitDistanceFromColumn)
        {
            CMember mReferenceGirt;
            CMember mColumn;
            CBlock_3D_002_WindowInBay window;
            CPoint pControlPointBlock;
            float fBayWidth;
            float fBayHeight = fH1_frame;
            int iFirstGirtInBay;
            int iFirstMemberToDeactivate;
            bool bIsReverseSession;
            bool bIsFirstBayInFrontorBackSide;
            bool bIsLastBayInFrontorBackSide;

            DeterminateBasicPropertiesToInsertBlock(insertprop, out mReferenceGirt, out mColumn, out pControlPointBlock, out fBayWidth, out iFirstGirtInBay, out bIsReverseSession, out bIsFirstBayInFrontorBackSide, out bIsLastBayInFrontorBackSide);

            window = new CBlock_3D_002_WindowInBay(
                insertprop.sBuildingSide,
                prop.fWindowsHeight,
                prop.fWindowsWidth,
                prop.fWindowCoordinateXinBay,
                prop.fWindowCoordinateZinBay,
                prop.iNumberOfWindowColumns,
                fLimitDistanceFromColumn,
                fBottomGirtPosition,
                fDist_Girt,
                mReferenceGirt,
                mColumn,
                fBayWidth,
                fBayHeight,
                bIsReverseSession,
                bIsFirstBayInFrontorBackSide,
                bIsLastBayInFrontorBackSide);

            iFirstMemberToDeactivate = iFirstGirtInBay + window.iNumberOfGirtsUnderWindow;

            AddDoorOrWindowBlockProperties(pControlPointBlock, iFirstMemberToDeactivate, window);
        }

        public void DeterminateBasicPropertiesToInsertBlock(
            PropertiesToInsertOpening insertprop,
            out CMember mReferenceGirt,
            out CMember mColumn,
            out CPoint pControlPointBlock,
            out float fBayWidth,
            out int iFirstMemberToDeactivate,
            out bool bIsReverseSession,
            out bool bIsFirstBayInFrontorBackSide,
            out bool bIsLastBayInFrontorBackSide
            )
        {
            bIsReverseSession = false;            // Set to true value just for front or back wall (right part of wall)
            bIsFirstBayInFrontorBackSide = false; // Set to true value just for front or back wall (first bay)
            bIsLastBayInFrontorBackSide = false;  // Set to true value just for front or back wall (last bay)

            if (insertprop.sBuildingSide == "Left" || insertprop.sBuildingSide == "Right")
            {
                // Left side X = 0, Right Side X = GableWidth
                // Insert after frame ID
                int iSideMultiplier = insertprop.sBuildingSide == "Left" ? 0 : 1; // 0 left side X = 0, 1 - right side X = Gable Width
                int iBlockFrame = insertprop.iBayNumber - 1; // ID of frame in the bay, starts with zero

                int iBayColumn = (iBlockFrame * 6) + (iSideMultiplier == 0 ? 0 : (4 - 1)); // (2 columns + 2 rafters + 2 eaves purlins) = 6, For Y = GableWidth + 4 number of members in one frame - 1 (index)

                fBayWidth = fL1_frame;
                iFirstMemberToDeactivate = iMainColumnNo + iRafterNo + iEavesPurlinNo + iBlockFrame * iGirtNoInOneFrame + iSideMultiplier * (iGirtNoInOneFrame / 2);

                mReferenceGirt = m_arrMembers[iFirstMemberToDeactivate]; // Deactivated member properties define properties of block girts
                mColumn = m_arrMembers[iBayColumn];
            }
            else // Front or Back Side
            {
                // Insert after sequence ID
                int iNumberOfIntermediateColumns;
                int[] iArrayOfGirtsPerColumnCount;
                //int iNumberOfGirtsInWall;

                if (insertprop.sBuildingSide == "Front")  // Front side properties
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

                int iSideMultiplier = insertprop.sBuildingSide == "Front" ? 0 : 1; // 0 front side Y = 0, 1 - back side Y = Length
                int iBlockSequence = insertprop.iBayNumber - 1; // ID of sequence, starts with zero
                int iColumnNumber;
                int iNumberOfFirstGirtInWallToDeactivate = 0;
                int iNumberOfMembers_tempForGirts = iMainColumnNo + iRafterNo + iEavesPurlinNo + (iFrameNo - 1) * iGirtNoInOneFrame + (iFrameNo - 1) * iPurlinNoInOneFrame + iFrontColumnNoInOneFrame + iBackColumnNoInOneFrame + iSideMultiplier * iFrontGirtsNoInOneFrame;

                if (iBlockSequence == 0) // Main Column
                {
                    if (insertprop.sBuildingSide == "Front")
                    {
                        iColumnNumber = 0;
                    }
                    else
                    {
                        iColumnNumber = (iFrameNo - 1) * 6;
                    }

                    iFirstMemberToDeactivate = iNumberOfMembers_tempForGirts + iNumberOfFirstGirtInWallToDeactivate;

                    bIsFirstBayInFrontorBackSide = true; // First bay
                }
                else
                {
                    int iNumberOfMembers_tempForColumns = iMainColumnNo + iRafterNo + iEavesPurlinNo + (iFrameNo - 1) * iGirtNoInOneFrame + (iFrameNo - 1) * iPurlinNoInOneFrame + iSideMultiplier * iFrontColumnNoInOneFrame;

                    if (iBlockSequence < (int)(iNumberOfIntermediateColumns / 2) + 1) // Left session
                    {
                        iColumnNumber = iNumberOfMembers_tempForColumns + iBlockSequence - 1;

                        iNumberOfFirstGirtInWallToDeactivate += iOneColumnGirtNo;

                        for (int i = 0; i < iBlockSequence - 1; i++)
                            iNumberOfFirstGirtInWallToDeactivate += iArrayOfGirtsPerColumnCount[i];
                    }
                    else // Right session
                    {
                        bIsReverseSession = true; // Nodes and members are numbered from right to the left

                        iColumnNumber = iNumberOfMembers_tempForColumns + (int)(iNumberOfIntermediateColumns / 2) + iNumberOfIntermediateColumns - iBlockSequence;

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
                        }
                    }

                    iFirstMemberToDeactivate = iNumberOfMembers_tempForGirts + iNumberOfFirstGirtInWallToDeactivate;
                }

                mReferenceGirt = m_arrMembers[iFirstMemberToDeactivate]; // Deactivated member properties define properties of block girts
                mColumn = m_arrMembers[iColumnNumber];
            }

            pControlPointBlock = new CPoint(0, mColumn.NodeStart.X, mColumn.NodeStart.Y, mColumn.NodeStart.Z, 0);
        }

        public void AddDoorOrWindowBlockProperties(CPoint pControlPointBlock, int iFirstMemberToDeactivate, CBlock block)
        {
            int arraysizeoriginal;

            // Cross-sections
            arraysizeoriginal = m_arrCrSc.Length;

            Array.Resize(ref m_arrCrSc, m_arrCrSc.Length + block.m_arrCrSc.Length - 1); // ( - 1) Prvy prvok v poli blocks crsc ignorujeme

            // Copy block cross-sections into the model
            for (int i = 1; i < block.m_arrCrSc.Length; i++) // Zacina sa od i = 1 - preskocit prvy prvok v poli doors, pretoze odkaz na girt section uz existuje, nie je potrebne prierez kopirovat znova
            {
                // Preskocit prvy prvok v poli block crsc, pretoze odkaz na girt section uz existuje, nie je potrebne prierez kopirovat znova
                m_arrCrSc[arraysizeoriginal + i - 1] = block.m_arrCrSc[i];
                m_arrCrSc[arraysizeoriginal + i - 1].ID = arraysizeoriginal + i/* -1 + 1*/; // Odcitat index pretoze prvy prierez ignorujeme a pridat 1 pre ID (+ 1)
            }

            // Nodes
            arraysizeoriginal = m_arrNodes.Length;
            Array.Resize(ref m_arrNodes, m_arrNodes.Length + block.m_arrNodes.Length);

            int iNumberofMembersToDeactivate = block.INumberOfGirtsToDeactivate;

            // Deactivate already generated members in the bay (space between frames) where is the block inserted
            for (int i = 0; i < iNumberofMembersToDeactivate; i++)
            {
                m_arrMembers[iFirstMemberToDeactivate + i].BIsDisplayed = false;

                // Deactivate Member Joints
                // TODO Ondrej - potrebujeme zistit, ktore spoje su pripojene na prut a deaktivovat ich, aby sa nevytvorili, asi by sme mali na tieto veci vyrobit nejaku mapu alebo dictionary
            }

            float fBlockRotationAboutZaxis_rad = 0;

            if(block.BuildingSide == "Left" || block.BuildingSide == "Right")
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

                // TODO - nizsie uvedeny kod zmaze priradenie pruta do zoznamu prutov v priereze (list prutov ktorym je prierez priradeny), 
                // prut je totizto uz priradeny k prierezu kedze bol uz priradeny v bloku
                // TODO - dvojite priradenie by sa malo vyriesit nejako elegantnejsie

                // We need to remove assignment of member to the girt cross-section, it is already assigned

                //19.8.2018
                //Neviem ci je toto stale problem,kedze uz je metoda na ziskanie AssignedMembersList - takze komentujem...
                /*CCrSc GirtCrossSection = block.ReferenceGirt.CrScStart;
                if (GirtCrossSection.AssignedMembersList.Count > 0) // Check that list is not empty
                {
                    if (GirtCrossSection.Equals(m_arrMembers[arraysizeoriginal + i].CrScStart))
                        GirtCrossSection.AssignedMembersList.RemoveAt(GirtCrossSection.AssignedMembersList.Count - 1);
                }

                // We need to remove assignment of member to the block cross-section, it is already assigned

                if (m_arrCrSc.Length > 0 && m_arrCrSc[m_arrCrSc.Length - 1].AssignedMembersList.Count > 0) // Check that list is not empty
                {
                    if (m_arrCrSc[m_arrCrSc.Length - 1].Equals(m_arrMembers[arraysizeoriginal + i].CrScStart))
                        m_arrCrSc[m_arrCrSc.Length - 1].AssignedMembersList.RemoveAt(m_arrCrSc[m_arrCrSc.Length - 1].AssignedMembersList.Count - 1);
                }*/
            }

            // TODO / BUG No 46 - odstranit spoje na deaktivovanych prutoch

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
            // ale stroskotal som na tom, ze vsetko co sa tyka databaz by malo byt v projekte DATABASE a ked som to chcel presunut tak 
            // mi v DATABASE napriklad chybal objekt combobox a aj dalsie referencie pretoze je to WINDOWS.FORMS a nie WPF
            // Vysledok bol taky ze som to akurat dobabral
            // Triedy CDatabaseComponents, CDatabaseModels a CDatabaseManager ako aj connectionStrings z app.config by asi mali byt v projekte DATABASE

            // Komentar platil pred reorganizaciou projektov
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
                    if (member.CrScStart.ID == group.CrossSection.ID) // In case that cross-section ID is same add member to the list
                    {
                        member.BIsDSelectedForIFCalculation = true; // TODO - mozno by sa malo nastavovat uz v konstruktore CMember
                        member.BIsDSelectedForDesign = true;
                        member.BIsSelectedForMaterialList = true;

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
    }
}
