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
                CCalcul_1170_5 eq
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

            const int iFrameNodesNo = 5;
            const int iEavesPurlinNoInOneFrame = 2;
            iEavesPurlinNo = iEavesPurlinNoInOneFrame * (iFrameNo - 1);
            iMainColumnNo = iFrameNo * 2;
            iRafterNo = iFrameNo * 2;

            iOneColumnGirtNo = 0;
            iGirtNoInOneFrame = 0;

            m_arrMat = new CMat[1];
            m_arrCrSc = new CCrSc[9];

            // Materials
            // Materials List - Materials Array - Fill Data of Materials Array
            m_arrMat[0] = new CMat_03_00("G550", 0.1f, 550e+6f, 550e+6f);

            // Cross-sections
            // CrSc List - CrSc Array - Fill Data of Cross-sections Array

            // TODO Ondrej - Nastavit objekt prierezu podla databazy models, tabulka KitsetGableRoofEnclosed alebo KitsetGableRoofEnclosedCrscID
            m_arrCrSc[(int)EMemberGroupNames.eMainColumn] = new CCrSc_3_63020_BOX(1, 0.63f, 0.2f, 0.00195f, 0.00195f, Colors.Chocolate);   // Main Column
            m_arrCrSc[(int)EMemberGroupNames.eRafter] = new CCrSc_3_63020_BOX(2, 0.63f, 0.2f, 0.00195f, 0.00195f, Colors.Green);           // Rafter
            m_arrCrSc[(int)EMemberGroupNames.eEavesPurlin] = new CCrSc_3_50020_C(3, 0.5f, 0.2f, 0.00195f, Colors.DarkCyan);                // Eaves Purlin
            m_arrCrSc[(int)EMemberGroupNames.eGirtWall] = new CCrSc_3_270XX_C(4, 0.27f, 0.07f, 0.00115f, Colors.Orange);                   // Girt - Wall
            m_arrCrSc[(int)EMemberGroupNames.ePurlin] = new CCrSc_3_270XX_C(5, 0.27f, 0.07f, 0.00095f, Colors.SlateBlue);                  // Purlin
            m_arrCrSc[(int)EMemberGroupNames.eFrontColumn] = new CCrSc_3_270XX_C_NESTED(6, 0.29f, 0.071f, 0.00115f, Colors.BlueViolet);    // Front Column
            m_arrCrSc[(int)EMemberGroupNames.eBackColumn] = new CCrSc_3_270XX_C_NESTED(7, 0.29f, 0.071f, 0.00115f, Colors.BlueViolet);     // Back Column
            m_arrCrSc[(int)EMemberGroupNames.eFrontGirt] = new CCrSc_3_270XX_C(8, 0.27f, 0.07f, 0.00115f, Colors.Brown);                   // Front Girt
            m_arrCrSc[(int)EMemberGroupNames.eBackGirt] = new CCrSc_3_270XX_C(9, 0.27f, 0.07f, 0.00095f, Colors.YellowGreen);              // Back Girt

            // Member Groups
            listOfModelMemberGroups = new List<CMemberGroup>(9);

            // See UC component list
            listOfModelMemberGroups.Add(new CMemberGroup(1, "Main Column" , m_arrCrSc[(int)EMemberGroupNames.eMainColumn], 0));
            listOfModelMemberGroups.Add(new CMemberGroup(2, "Rafter"      , m_arrCrSc[(int)EMemberGroupNames.eRafter], 0));
            listOfModelMemberGroups.Add(new CMemberGroup(3, "Eaves Purlin", m_arrCrSc[(int)EMemberGroupNames.eEavesPurlin], 0));
            listOfModelMemberGroups.Add(new CMemberGroup(4, "Girt - Wall" , m_arrCrSc[(int)EMemberGroupNames.eGirtWall], 0));
            listOfModelMemberGroups.Add(new CMemberGroup(5, "Purlin"      , m_arrCrSc[(int)EMemberGroupNames.ePurlin], 0));
            listOfModelMemberGroups.Add(new CMemberGroup(6, "Front Column", m_arrCrSc[(int)EMemberGroupNames.eFrontColumn], 0));
            listOfModelMemberGroups.Add(new CMemberGroup(7, "Back Column" , m_arrCrSc[(int)EMemberGroupNames.eBackColumn], 0));
            listOfModelMemberGroups.Add(new CMemberGroup(8, "Front Girt"  , m_arrCrSc[(int)EMemberGroupNames.eFrontGirt], 0));
            listOfModelMemberGroups.Add(new CMemberGroup(9, "Back Girt"   , m_arrCrSc[(int)EMemberGroupNames.eBackGirt], 0));

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
            int iEveryXXPurlin = 3; // Index of purlin 1 - every, 2 - every second purlin, 3 - every third purlin

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
                RotateFrontOrBackFrameNodeAboutZ(m_arrNodes[i * iFrameNodesNo + 0]);

                m_arrNodes[i * iFrameNodesNo + 1] = new CNode(i * iFrameNodesNo + 2, 000000, i * fL1_frame, fH1_frame, 0);
                RotateFrontOrBackFrameNodeAboutZ(m_arrNodes[i * iFrameNodesNo + 1]);

                m_arrNodes[i * iFrameNodesNo + 2] = new CNode(i * iFrameNodesNo + 3, 0.5f * fW_frame, i * fL1_frame, fH2_frame, 0);
                RotateFrontOrBackFrameNodeAboutZ(m_arrNodes[i * iFrameNodesNo + 2]);

                m_arrNodes[i * iFrameNodesNo + 3] = new CNode(i * iFrameNodesNo + 4, fW_frame, i * fL1_frame, fH1_frame, 0);
                RotateFrontOrBackFrameNodeAboutZ(m_arrNodes[i * iFrameNodesNo + 3]);

                m_arrNodes[i * iFrameNodesNo + 4] = new CNode(i * iFrameNodesNo + 5, fW_frame, i * fL1_frame, 00000, 0);
                RotateFrontOrBackFrameNodeAboutZ(m_arrNodes[i * iFrameNodesNo + 4]);
            }

            // Members
            for (int i = 0; i < iFrameNo; i++)
            {
                // Main Column
                m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * (iFrameNodesNo - 1) + 0] = new CMember(i * (iFrameNodesNo - 1) + 1, m_arrNodes[i * iFrameNodesNo + 0], m_arrNodes[i * iFrameNodesNo + 1], m_arrCrSc[(int)EMemberGroupNames.eMainColumn], EMemberType_FormSteel.eMC, null, null, fMainColumnStart, fMainColumnEnd, 0f, 0);
                // Rafters
                m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * (iFrameNodesNo - 1) + 1] = new CMember(i * (iFrameNodesNo - 1) + 2, m_arrNodes[i * iFrameNodesNo + 1], m_arrNodes[i * iFrameNodesNo + 2], m_arrCrSc[(int)EMemberGroupNames.eRafter], EMemberType_FormSteel.eMR, null, null, fRafterStart, fRafterEnd, 0f, 0);
                m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * (iFrameNodesNo - 1) + 2] = new CMember(i * (iFrameNodesNo - 1) + 3, m_arrNodes[i * iFrameNodesNo + 2], m_arrNodes[i * iFrameNodesNo + 3], m_arrCrSc[(int)EMemberGroupNames.eRafter], EMemberType_FormSteel.eMR, null, null, fRafterEnd, fRafterStart, 0f, 0);
                // Main Column
                m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * (iFrameNodesNo - 1) + 3] = new CMember(i * (iFrameNodesNo - 1) + 4, m_arrNodes[i * iFrameNodesNo + 3], m_arrNodes[i * iFrameNodesNo + 4], m_arrCrSc[(int)EMemberGroupNames.eMainColumn], EMemberType_FormSteel.eMC, null, null, fMainColumnEnd, fMainColumnStart, 0f, 0);

                // Eaves Purlins
                if (i < (iFrameNo - 1))
                {
                    m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * (iFrameNodesNo - 1) + 4] = new CMember((i * iEavesPurlinNoInOneFrame) + i * (iFrameNodesNo - 1) + 5, m_arrNodes[i * iFrameNodesNo + 1], m_arrNodes[(i + 1) * iFrameNodesNo + 1], m_arrCrSc[(int)EMemberGroupNames.eEavesPurlin], EMemberType_FormSteel.eEP, eccentricityEavePurlin, eccentricityEavePurlin, fEavesPurlinStart, fEavesPurlinEnd, (float)Math.PI, 0);
                    m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * (iFrameNodesNo - 1) + 5] = new CMember((i * iEavesPurlinNoInOneFrame) + i * (iFrameNodesNo - 1) + 6, m_arrNodes[i * iFrameNodesNo + 3], m_arrNodes[(i + 1) * iFrameNodesNo + 3], m_arrCrSc[(int)EMemberGroupNames.eEavesPurlin], EMemberType_FormSteel.eEP, eccentricityEavePurlin, eccentricityEavePurlin, fEavesPurlinStart, fEavesPurlinEnd, 0f, 0);
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
                        m_arrMembers[i_temp_numberofMembers + i * iGirtNoInOneFrame + j] = new CMember(i_temp_numberofMembers + i * iGirtNoInOneFrame + j + 1, m_arrNodes[i_temp_numberofNodes + i * iGirtNoInOneFrame + j], m_arrNodes[i_temp_numberofNodes + (i + 1) * iGirtNoInOneFrame + j], m_arrCrSc[(int)EMemberGroupNames.eGirtWall], EMemberType_FormSteel.eG, eccentricityGirtLeft_X0, eccentricityGirtLeft_X0, fGirtStart, fGirtEnd, fGirtsRotation, 0);
                        RotateFrontOrBackFrameNodeAboutZ(m_arrNodes[i_temp_numberofMembers + i * iGirtNoInOneFrame + j]);
                    }

                    for (int j = 0; j < iOneColumnGirtNo; j++)
                    {
                        m_arrMembers[i_temp_numberofMembers + i * iGirtNoInOneFrame + iOneColumnGirtNo + j] = new CMember(i_temp_numberofMembers + i * iGirtNoInOneFrame + iOneColumnGirtNo + j + 1, m_arrNodes[i_temp_numberofNodes + i * iGirtNoInOneFrame + iOneColumnGirtNo + j], m_arrNodes[i_temp_numberofNodes + (i + 1) * iGirtNoInOneFrame + iOneColumnGirtNo + j], m_arrCrSc[(int)EMemberGroupNames.eGirtWall], EMemberType_FormSteel.eG, eccentricityGirtRight_XB, eccentricityGirtRight_XB, fGirtStart, fGirtEnd, fGirtsRotation, 0);
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
                            // Left Rafter
                            m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * (iFrameNodesNo - 1) + 1].IntermediateTransverseSupportGroup = lTransverseSupportGroup_Rafter;
                            // Right Rafter
                            m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * (iFrameNodesNo - 1) + 2].IntermediateTransverseSupportGroup = lTransverseSupportGroup_Rafter;
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

                        m_arrMembers[i_temp_numberofMembers + i * iPurlinNoInOneFrame + j] = new CMember(i_temp_numberofMembers + i * iPurlinNoInOneFrame + j + 1, m_arrNodes[i_temp_numberofNodes + i * iPurlinNoInOneFrame + j], m_arrNodes[i_temp_numberofNodes + (i + 1) * iPurlinNoInOneFrame + j], m_arrCrSc[(int)EMemberGroupNames.ePurlin], EMemberType_FormSteel.eP, temp/*eccentricityPurlin*/, temp /*eccentricityPurlin*/, fPurlinStart, fPurlinEnd, fRotationAngle, 0);
                    }

                    for (int j = 0; j < iOneRafterPurlinNo; j++)
                    {
                        m_arrMembers[i_temp_numberofMembers + i * iPurlinNoInOneFrame + iOneRafterPurlinNo + j] = new CMember(i_temp_numberofMembers + i * iPurlinNoInOneFrame + iOneRafterPurlinNo + j + 1, m_arrNodes[i_temp_numberofNodes + i * iPurlinNoInOneFrame + iOneRafterPurlinNo + j], m_arrNodes[i_temp_numberofNodes + (i + 1) * iPurlinNoInOneFrame + iOneRafterPurlinNo + j], m_arrCrSc[(int)EMemberGroupNames.ePurlin], EMemberType_FormSteel.eP, eccentricityPurlin, eccentricityPurlin, fPurlinStart, fPurlinEnd, fRoofPitch_rad, 0);
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
                        AddDoorBlock(doorBlocksPropertiesToInsert[i], doorBlocksProperties[i], 0.5f);
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

            // Loading
            #region POKUS TODO 186 - frames
            // Komentar 7.2.2019
            // To Ondrej
            // Vytvorenie zatazenia priamo na frames
            // Negeneruje sa z surface loads
            // Toto zatazenie nie je priradene do load casesov takze vo vypocte sa nepouzije
            // Pouzit len na inspiraciu, zatial nemazat

            // Snow load factor - projection on roof
            float fSlopeFactor = ((0.5f * fW_frame) / ((0.5f * fW_frame) / (float)Math.Cos(fRoofPitch_rad))); // Consider projection acc. to Figure 4.1

            // Loads
            float fValueLoadRafterDead1 = -generalLoad.fDeadLoadTotal_Roof * fL1_frame;
            float fValueLoadRafterDead2 = -generalLoad.fDeadLoadTotal_Roof * fL1_frame;
            float fValueLoadRafterImposed = -generalLoad.fImposedLoadTotal_Roof * fL1_frame;

            float fValueLoadRafterSnowULS_Nu_1 = -snow.fs_ULS_Nu_1 * fSlopeFactor * fL1_frame; // Design value (projection on roof)
            float fValueLoadRafterSnowULS_Nu_2 = -snow.fs_ULS_Nu_2 * fSlopeFactor * fL1_frame;
            float fValueLoadRafterSnowSLS_Nu_1 = -snow.fs_SLS_Nu_1 * fSlopeFactor * fL1_frame;
            float fValueLoadRafterSnowSLS_Nu_2 = -snow.fs_SLS_Nu_2 * fSlopeFactor * fL1_frame;

            // Wind load

            // Cpe,min
            float fValueLoadColumnWind1PlusX_CpeMin = -0.5f;
            float fValueLoadColumnWind2PlusX_CpeMin = 0.2f;
            float fValueLoadRafterWind1PlusX_CpeMin = 0.3f;
            float fValueLoadRafterWind2PlusX_CpeMin = 0.2f;

            float fValueLoadColumnWind1MinusX_CpeMin = fValueLoadColumnWind2PlusX_CpeMin;
            float fValueLoadColumnWind2MinusX_CpeMin = fValueLoadColumnWind1PlusX_CpeMin;
            float fValueLoadRafterWind1MinusX_CpeMin = fValueLoadRafterWind2PlusX_CpeMin;
            float fValueLoadRafterWind2MinusX_CpeMin = fValueLoadRafterWind1PlusX_CpeMin;

            float fValueLoadColumnnWindPlusY_CpeMin = 0.2f;
            float fValueLoadRafterWindPlusY_CpeMin = 0.3f;
            float fValueLoadColumnnWindMinusY_CpeMin = fValueLoadColumnnWindPlusY_CpeMin;
            float fValueLoadRafterWindMinusY_CpeMin = fValueLoadRafterWindPlusY_CpeMin;

            // Cpe,max
            float fValueLoadColumnWind1PlusX_CpeMax = -0.5f;
            float fValueLoadColumnWind2PlusX_CpeMax = 0.2f;
            float fValueLoadRafterWind1PlusX_CpeMax = 0.3f;
            float fValueLoadRafterWind2PlusX_CpeMax = 0.2f;

            float fValueLoadColumnWind1MinusX_CpeMax = fValueLoadColumnWind2PlusX_CpeMax;
            float fValueLoadColumnWind2MinusX_CpeMax = fValueLoadColumnWind1PlusX_CpeMax;
            float fValueLoadRafterWind1MinusX_CpeMax = fValueLoadRafterWind2PlusX_CpeMax;
            float fValueLoadRafterWind2MinusX_CpeMax = fValueLoadRafterWind1PlusX_CpeMax;

            float fValueLoadColumnnWindPlusY_CpeMax = 0.2f;
            float fValueLoadRafterWindPlusY_CpeMax = 0.3f;
            float fValueLoadColumnnWindMinusY_CpeMax = fValueLoadColumnnWindPlusY_CpeMax;
            float fValueLoadRafterWindMinusY_CpeMax = fValueLoadRafterWindPlusY_CpeMax;

            List<CMLoad> memberLoadDead1Rafters = new List<CMLoad>();
            List<CMLoad> memberLoadDead2Rafters = new List<CMLoad>();
            List<CMLoad> memberLoadImposedRafters = new List<CMLoad>();
            List<CMLoad> memberMaxLoadSnowAllRafters = new List<CMLoad>();
            List<CMLoad> memberMaxLoadSnowLeftRafters = new List<CMLoad>();
            List<CMLoad> memberMaxLoadSnowRightRafters = new List<CMLoad>();

            List<CMLoad> memberLoadWindFramesPlusX_CpeMin = new List<CMLoad>();
            List<CMLoad> memberLoadWindFramesMinusX_CpeMin = new List<CMLoad>();

            List<CMLoad> memberLoadWindFramesPlusY_CpeMin = new List<CMLoad>();
            List<CMLoad> memberLoadWindFramesMinusY_CpeMin = new List<CMLoad>();

            List<CMLoad> memberLoadWindFramesPlusX_CpeMax = new List<CMLoad>();
            List<CMLoad> memberLoadWindFramesMinusX_CpeMax = new List<CMLoad>();

            List<CMLoad> memberLoadWindFramesPlusY_CpeMax = new List<CMLoad>();
            List<CMLoad> memberLoadWindFramesMinusY_CpeMax = new List<CMLoad>();

            GenerateLoadOnRafters_Old(fValueLoadRafterDead1, fValueLoadRafterDead1, ref memberLoadDead1Rafters);
            GenerateLoadOnRafters_Old(fValueLoadRafterDead2, fValueLoadRafterDead2, ref memberLoadDead2Rafters);
            GenerateLoadOnRafters_Old(fValueLoadRafterImposed, fValueLoadRafterImposed, ref memberLoadImposedRafters);
            GenerateLoadOnRafters_Old(fValueLoadRafterSnowULS_Nu_1, fValueLoadRafterSnowULS_Nu_1, ref memberMaxLoadSnowAllRafters);
            GenerateLoadOnRafters_Old(fValueLoadRafterSnowULS_Nu_2, 0, ref memberMaxLoadSnowLeftRafters);
            GenerateLoadOnRafters_Old(0, fValueLoadRafterSnowULS_Nu_2, ref memberMaxLoadSnowRightRafters);

            // Wind

            // Cpe,min
            // + X
            GenerateLoadOnFrames_Old(fValueLoadColumnWind1PlusX_CpeMin, fValueLoadColumnWind2PlusX_CpeMin, fValueLoadRafterWind1PlusX_CpeMin, fValueLoadRafterWind2PlusX_CpeMin, ref memberLoadWindFramesPlusX_CpeMin);

            // - X
            GenerateLoadOnFrames_Old(fValueLoadColumnWind1MinusX_CpeMin, fValueLoadColumnWind2MinusX_CpeMin, fValueLoadRafterWind1MinusX_CpeMin, fValueLoadRafterWind2MinusX_CpeMin, ref memberLoadWindFramesMinusX_CpeMin);

            // + Y
            GenerateLoadOnFrames_Old(fValueLoadColumnnWindPlusY_CpeMin, fValueLoadColumnnWindPlusY_CpeMin, fValueLoadRafterWindPlusY_CpeMin, fValueLoadRafterWindPlusY_CpeMin, ref memberLoadWindFramesPlusY_CpeMin);

            // - Y
            GenerateLoadOnFrames_Old(fValueLoadColumnnWindMinusY_CpeMin, fValueLoadColumnnWindMinusY_CpeMin, fValueLoadRafterWindMinusY_CpeMin, fValueLoadRafterWindMinusY_CpeMin, ref memberLoadWindFramesMinusY_CpeMin);

            // Cpe,max
            // + X
            GenerateLoadOnFrames_Old(fValueLoadColumnWind1PlusX_CpeMax, fValueLoadColumnWind2PlusX_CpeMax, fValueLoadRafterWind1PlusX_CpeMax, fValueLoadRafterWind2PlusX_CpeMax, ref memberLoadWindFramesPlusX_CpeMax);

            // - X
            GenerateLoadOnFrames_Old(fValueLoadColumnWind1MinusX_CpeMax, fValueLoadColumnWind2MinusX_CpeMax, fValueLoadRafterWind1MinusX_CpeMax, fValueLoadRafterWind2MinusX_CpeMax, ref memberLoadWindFramesMinusX_CpeMax);

            // + Y
            GenerateLoadOnFrames_Old(fValueLoadColumnnWindPlusY_CpeMax, fValueLoadColumnnWindPlusY_CpeMax, fValueLoadRafterWindPlusY_CpeMax, fValueLoadRafterWindPlusY_CpeMax, ref memberLoadWindFramesPlusY_CpeMax);

            // - Y
            GenerateLoadOnFrames_Old(fValueLoadColumnnWindMinusY_CpeMax, fValueLoadColumnnWindMinusY_CpeMax, fValueLoadRafterWindMinusY_CpeMax, fValueLoadRafterWindMinusY_CpeMax, ref memberLoadWindFramesMinusY_CpeMax);

            // FRAMES
            // Pokus 14.2.2019

            List<CMLoad> memberLoadDead_Frames = new List<CMLoad>();
            List<CMLoad> memberLoadImposed_Frames = new List<CMLoad>();
            List<CMLoad> memberMaxLoadSnowAll_Frames = new List<CMLoad>();
            List<CMLoad> memberMaxLoadSnowLeft_Frames = new List<CMLoad>();
            List<CMLoad> memberMaxLoadSnowRight_Frames = new List<CMLoad>();



            #endregion

            #region Surface Loads
            // Surface Free Loads
            // Roof Surface Geometry
            // Control Points
            CPoint pRoofFrontLeft  = new CPoint(0,               0,      0, fH1_frame, 0);
            CPoint pRoofFrontApex  = new CPoint(0, 0.5f * fW_frame,      0, fH2_frame, 0);
            CPoint pRoofFrontRight = new CPoint(0,        fW_frame,      0, fH1_frame, 0);
            CPoint pRoofBackLeft   = new CPoint(0,               0, fL_tot, fH1_frame, 0);
            CPoint pRoofBackApex   = new CPoint(0, 0.5f * fW_frame, fL_tot, fH2_frame, 0);
            CPoint pRoofBackRight  = new CPoint(0,        fW_frame, fL_tot, fH1_frame, 0);

            // Dimensions
            float fRoof_X = fL_tot;
            float fRoof_Y = 0.5f * fW_frame / (float)Math.Cos(fRoofPitch_rad);

            // Wall Surface Geometry
            // Control Point
            CPoint pWallFrontLeft  = new CPoint(0,        0,      0,   0, 0);
            CPoint pWallFrontRight = new CPoint(0, fW_frame,      0,   0, 0);
            CPoint pWallBackRight  = new CPoint(0, fW_frame, fL_tot,   0, 0);
            CPoint pWallBackLeft   = new CPoint(0,        0, fL_tot,   0, 0);

            // Dimensions
            float fWallLeftOrRight_X = fL_tot;
            float fWallLeftOrRight_Y = fH1_frame;

            // Dimensions
            float fWallFrontOrBack_X = fW_frame;
            float fWallFrontOrBack_Y1 = fH1_frame;
            float fWallFrontOrBack_Y2 = fH2_frame;

            // Types and loading widths of loaded members under free surface loads
            List<FreeSurfaceLoadsMemberTypeData> listOfLoadedMemberTypeDataRoof = new List<FreeSurfaceLoadsMemberTypeData>(2);
            listOfLoadedMemberTypeDataRoof.Add(new FreeSurfaceLoadsMemberTypeData(EMemberType_FormSteel.eP, fDist_Purlin));
            listOfLoadedMemberTypeDataRoof.Add(new FreeSurfaceLoadsMemberTypeData(EMemberType_FormSteel.eEP, 0.5f * fDist_Purlin));
            List<FreeSurfaceLoadsMemberTypeData> listOfLoadedMemberTypeDataWallLeftRight = new List<FreeSurfaceLoadsMemberTypeData>(2);
            listOfLoadedMemberTypeDataWallLeftRight.Add(new FreeSurfaceLoadsMemberTypeData(EMemberType_FormSteel.eG, fDist_Girt));
            listOfLoadedMemberTypeDataWallLeftRight.Add(new FreeSurfaceLoadsMemberTypeData(EMemberType_FormSteel.eEP, 0.5f * fDist_Girt));
            List<FreeSurfaceLoadsMemberTypeData> listOfLoadedMemberTypeDataWallFront = new List<FreeSurfaceLoadsMemberTypeData>(1);
            listOfLoadedMemberTypeDataWallFront.Add(new FreeSurfaceLoadsMemberTypeData(EMemberType_FormSteel.eG, fDist_FrontGirts));
            List<FreeSurfaceLoadsMemberTypeData> listOfLoadedMemberTypeDataWallBack = new List<FreeSurfaceLoadsMemberTypeData>(1);
            listOfLoadedMemberTypeDataWallBack.Add(new FreeSurfaceLoadsMemberTypeData(EMemberType_FormSteel.eG, fDist_FrontGirts));

            // Hodnota zatazenia v smere kladnej osi je kladna, hodnota zatazenia v smere zapornej osi je zaporna
            // Permanent load
            List<CSLoad_Free> surfaceDeadLoad = new List<CSLoad_Free>(6);
            surfaceDeadLoad.Add(new CSLoad_FreeUniform(listOfLoadedMemberTypeDataRoof, ELoadCoordSystem.eGCS, ELoadDir.eLD_Z, pRoofFrontApex, fRoof_X, fRoof_Y, -generalLoad.fDeadLoadTotal_Roof, -fRoofPitch_rad / (float)Math.PI * 180f, 0, 90, false, true, true, 0));
            surfaceDeadLoad.Add(new CSLoad_FreeUniform(listOfLoadedMemberTypeDataRoof, ELoadCoordSystem.eGCS, ELoadDir.eLD_Z, pRoofFrontRight, fRoof_X, fRoof_Y, -generalLoad.fDeadLoadTotal_Roof, fRoofPitch_rad / (float)Math.PI * 180f, 0, 90, false, true, true, 0));
            surfaceDeadLoad.Add(new CSLoad_FreeUniform(listOfLoadedMemberTypeDataWallLeftRight, ELoadCoordSystem.eGCS, ELoadDir.eLD_Z, pWallFrontLeft, fWallLeftOrRight_X, fWallLeftOrRight_Y, -generalLoad.fDeadLoadTotal_Wall, 90, 0, 90, Colors.DeepPink, true, true, true, 0));
            surfaceDeadLoad.Add(new CSLoad_FreeUniform(listOfLoadedMemberTypeDataWallLeftRight, ELoadCoordSystem.eGCS, ELoadDir.eLD_Z, pWallBackRight, fWallLeftOrRight_X, fWallLeftOrRight_Y, -generalLoad.fDeadLoadTotal_Wall, 90, 0, 180+90, Colors.DeepPink, true, true, true, 0));
            surfaceDeadLoad.Add(new CSLoad_FreeUniform(listOfLoadedMemberTypeDataWallFront, ELoadCoordSystem.eGCS, ELoadDir.eLD_Z, pWallFrontLeft, fWallFrontOrBack_X, fWallFrontOrBack_Y1, 0.5f * fWallFrontOrBack_X, fWallFrontOrBack_Y2, fWallFrontOrBack_Y1, -generalLoad.fDeadLoadTotal_Wall, 90, 0, 0, Colors.DeepPink, false, true, false, true, 0));
            surfaceDeadLoad.Add(new CSLoad_FreeUniform(listOfLoadedMemberTypeDataWallBack, ELoadCoordSystem.eGCS, ELoadDir.eLD_Z, pWallBackRight, fWallFrontOrBack_X, fWallFrontOrBack_Y1, 0.5f * fWallFrontOrBack_X, fWallFrontOrBack_Y2, fWallFrontOrBack_Y1, -generalLoad.fDeadLoadTotal_Wall, 90, 0, 180, Colors.DeepPink, false, true, false, true, 0));

            // Imposed Load - Roof
            List<CSLoad_Free> surfaceRoofImposedLoad = new List<CSLoad_Free>(2);
            surfaceRoofImposedLoad.Add(new CSLoad_FreeUniform(listOfLoadedMemberTypeDataRoof, ELoadCoordSystem.eGCS, ELoadDir.eLD_Z, pRoofFrontApex, fRoof_X, fRoof_Y, -generalLoad.fImposedLoadTotal_Roof, -fRoofPitch_rad / (float)Math.PI * 180f, 0, 90, Colors.Red, false, true, true, 0));
            surfaceRoofImposedLoad.Add(new CSLoad_FreeUniform(listOfLoadedMemberTypeDataRoof, ELoadCoordSystem.eGCS, ELoadDir.eLD_Z, pRoofFrontRight, fRoof_X, fRoof_Y, -generalLoad.fImposedLoadTotal_Roof, fRoofPitch_rad / (float)Math.PI * 180f, 0, 90, Colors.Red, false, true, true, 0));

            // Snow Load - Roof
            float fsnowULS_Nu_1 = -snow.fs_ULS_Nu_1 * fSlopeFactor; // Design value (projection on roof)
            float fsnowULS_Nu_2 = -snow.fs_ULS_Nu_2 * fSlopeFactor;
            float fsnowSLS_Nu_1 = -snow.fs_SLS_Nu_1 * fSlopeFactor;
            float fsnowSLS_Nu_2 = -snow.fs_SLS_Nu_2 * fSlopeFactor;

            List <CSLoad_Free> surfaceRoofSnowLoad_ULS_Nu_1 = new List<CSLoad_Free>(2);
            surfaceRoofSnowLoad_ULS_Nu_1.Add(new CSLoad_FreeUniform(listOfLoadedMemberTypeDataRoof, ELoadCoordSystem.eGCS, ELoadDir.eLD_Z, pRoofFrontApex, fRoof_X, fRoof_Y, fsnowULS_Nu_1, -fRoofPitch_rad / (float)Math.PI * 180f, 0, 90, Colors.SeaGreen, false, true, true, 0));
            surfaceRoofSnowLoad_ULS_Nu_1.Add(new CSLoad_FreeUniform(listOfLoadedMemberTypeDataRoof, ELoadCoordSystem.eGCS, ELoadDir.eLD_Z, pRoofFrontRight, fRoof_X, fRoof_Y, fsnowULS_Nu_1, fRoofPitch_rad / (float)Math.PI * 180f, 0, 90, Colors.SeaGreen, false, true, true, 0));
            List<CSLoad_Free> surfaceRoofSnowLoad_ULS_Nu_2_Left = new List<CSLoad_Free>(1);
            surfaceRoofSnowLoad_ULS_Nu_2_Left.Add(new CSLoad_FreeUniform(listOfLoadedMemberTypeDataRoof, ELoadCoordSystem.eGCS, ELoadDir.eLD_Z, pRoofFrontApex, fRoof_X, fRoof_Y, fsnowULS_Nu_2, -fRoofPitch_rad / (float)Math.PI * 180f, 0, 90, Colors.SeaGreen, false, true, true, 0));
            List<CSLoad_Free> surfaceRoofSnowLoad_ULS_Nu_2_Right = new List<CSLoad_Free>(1);
            surfaceRoofSnowLoad_ULS_Nu_2_Right.Add(new CSLoad_FreeUniform(listOfLoadedMemberTypeDataRoof, ELoadCoordSystem.eGCS, ELoadDir.eLD_Z, pRoofFrontRight, fRoof_X, fRoof_Y, fsnowULS_Nu_2, fRoofPitch_rad / (float)Math.PI * 180f, 0, 90, Colors.SeaGreen, false, true, true, 0));

            List<CSLoad_Free> surfaceRoofSnowLoad_SLS_Nu_1 = new List<CSLoad_Free>(2);
            surfaceRoofSnowLoad_SLS_Nu_1.Add(new CSLoad_FreeUniform(listOfLoadedMemberTypeDataRoof, ELoadCoordSystem.eGCS, ELoadDir.eLD_Z, pRoofFrontApex, fRoof_X, fRoof_Y, fsnowSLS_Nu_1, -fRoofPitch_rad / (float)Math.PI * 180f, 0, 90, Colors.SeaGreen, false, true, true, 0));
            surfaceRoofSnowLoad_SLS_Nu_1.Add(new CSLoad_FreeUniform(listOfLoadedMemberTypeDataRoof, ELoadCoordSystem.eGCS, ELoadDir.eLD_Z, pRoofFrontRight, fRoof_X, fRoof_Y, fsnowSLS_Nu_1, fRoofPitch_rad / (float)Math.PI * 180f, 0, 90, Colors.SeaGreen, false, true, true, 0));
            List<CSLoad_Free> surfaceRoofSnowLoad_SLS_Nu_2_Left = new List<CSLoad_Free>(1);
            surfaceRoofSnowLoad_SLS_Nu_2_Left.Add(new CSLoad_FreeUniform(listOfLoadedMemberTypeDataRoof, ELoadCoordSystem.eGCS, ELoadDir.eLD_Z, pRoofFrontApex, fRoof_X, fRoof_Y, fsnowSLS_Nu_2, -fRoofPitch_rad / (float)Math.PI * 180f, 0, 90, Colors.SeaGreen, false, true, true, 0));
            List<CSLoad_Free> surfaceRoofSnowLoad_SLS_Nu_2_Right = new List<CSLoad_Free>(1);
            surfaceRoofSnowLoad_SLS_Nu_2_Right.Add(new CSLoad_FreeUniform(listOfLoadedMemberTypeDataRoof, ELoadCoordSystem.eGCS, ELoadDir.eLD_Z, pRoofFrontRight, fRoof_X, fRoof_Y, fsnowSLS_Nu_2, fRoofPitch_rad / (float)Math.PI * 180f, 0, 90, Colors.SeaGreen, false, true, true, 0));

            // Wind Load
            // Internal pressure
            // ULS
            // Cpi, min
            List<CSLoad_Free> surfaceWindLoad_ULS_PlusX_Cpimin = new List<CSLoad_Free>(6);
            List<CSLoad_Free> surfaceWindLoad_ULS_MinusX_Cpimin = new List<CSLoad_Free>(6);
            List<CSLoad_Free> surfaceWindLoad_ULS_PlusY_Cpimin = new List<CSLoad_Free>(6);
            List<CSLoad_Free> surfaceWindLoad_ULS_MinusY_Cpimin = new List<CSLoad_Free>(6);

            // Cpi, max
            List<CSLoad_Free> surfaceWindLoad_ULS_PlusX_Cpimax = new List<CSLoad_Free>(6);
            List<CSLoad_Free> surfaceWindLoad_ULS_MinusX_Cpimax = new List<CSLoad_Free>(6);
            List<CSLoad_Free> surfaceWindLoad_ULS_PlusY_Cpimax = new List<CSLoad_Free>(6);
            List<CSLoad_Free> surfaceWindLoad_ULS_MinusY_Cpimax = new List<CSLoad_Free>(6);

            SetSurfaceWindLoads_Cpi(
            listOfLoadedMemberTypeDataRoof,
            listOfLoadedMemberTypeDataWallLeftRight,
            listOfLoadedMemberTypeDataWallFront,
            listOfLoadedMemberTypeDataWallBack,
            ELSType.eLS_ULS,
            pRoofFrontApex,
            pRoofFrontRight,
            fRoof_X,
            fRoof_Y,
            pWallFrontLeft,
            pWallBackRight,
            fWallLeftOrRight_X,
            fWallLeftOrRight_Y,
            fWallFrontOrBack_X,
            fWallFrontOrBack_Y1,
            fWallFrontOrBack_Y2,
            wind,
            out surfaceWindLoad_ULS_PlusX_Cpimin,
            out surfaceWindLoad_ULS_MinusX_Cpimin,
            out surfaceWindLoad_ULS_PlusY_Cpimin,
            out surfaceWindLoad_ULS_MinusY_Cpimin,
            out surfaceWindLoad_ULS_PlusX_Cpimax,
            out surfaceWindLoad_ULS_MinusX_Cpimax,
            out surfaceWindLoad_ULS_PlusY_Cpimax,
            out surfaceWindLoad_ULS_MinusY_Cpimax
            );

            // SLS
            // Cpi, min
            List<CSLoad_Free> surfaceWindLoad_SLS_PlusX_Cpimin = new List<CSLoad_Free>(6);
            List<CSLoad_Free> surfaceWindLoad_SLS_MinusX_Cpimin = new List<CSLoad_Free>(6);
            List<CSLoad_Free> surfaceWindLoad_SLS_PlusY_Cpimin = new List<CSLoad_Free>(6);
            List<CSLoad_Free> surfaceWindLoad_SLS_MinusY_Cpimin = new List<CSLoad_Free>(6);

            // Cpi, max
            List<CSLoad_Free> surfaceWindLoad_SLS_PlusX_Cpimax = new List<CSLoad_Free>(6);
            List<CSLoad_Free> surfaceWindLoad_SLS_MinusX_Cpimax = new List<CSLoad_Free>(6);
            List<CSLoad_Free> surfaceWindLoad_SLS_PlusY_Cpimax = new List<CSLoad_Free>(6);
            List<CSLoad_Free> surfaceWindLoad_SLS_MinusY_Cpimax = new List<CSLoad_Free>(6);

            SetSurfaceWindLoads_Cpi(
            listOfLoadedMemberTypeDataRoof,
            listOfLoadedMemberTypeDataWallLeftRight,
            listOfLoadedMemberTypeDataWallFront,
            listOfLoadedMemberTypeDataWallBack,
            ELSType.eLS_SLS,
            pRoofFrontApex,
            pRoofFrontRight,
            fRoof_X,
            fRoof_Y,
            pWallFrontLeft,
            pWallBackRight,
            fWallLeftOrRight_X,
            fWallLeftOrRight_Y,
            fWallFrontOrBack_X,
            fWallFrontOrBack_Y1,
            fWallFrontOrBack_Y2,
            wind,
            out surfaceWindLoad_SLS_PlusX_Cpimin,
            out surfaceWindLoad_SLS_MinusX_Cpimin,
            out surfaceWindLoad_SLS_PlusY_Cpimin,
            out surfaceWindLoad_SLS_MinusY_Cpimin,
            out surfaceWindLoad_SLS_PlusX_Cpimax,
            out surfaceWindLoad_SLS_MinusX_Cpimax,
            out surfaceWindLoad_SLS_PlusY_Cpimax,
            out surfaceWindLoad_SLS_MinusY_Cpimax
            );

            // External presssure
            // ULS
            // Cpe, min
            List<CSLoad_Free> surfaceWindLoad_ULS_PlusX_Cpemin = new List<CSLoad_Free>(6);
            List<CSLoad_Free> surfaceWindLoad_ULS_MinusX_Cpemin = new List<CSLoad_Free>(6);
            List<CSLoad_Free> surfaceWindLoad_ULS_PlusY_Cpemin = new List<CSLoad_Free>(6);
            List<CSLoad_Free> surfaceWindLoad_ULS_MinusY_Cpemin = new List<CSLoad_Free>(6);

            // Cpe, max
            List<CSLoad_Free> surfaceWindLoad_ULS_PlusX_Cpemax = new List<CSLoad_Free>(6);
            List<CSLoad_Free> surfaceWindLoad_ULS_MinusX_Cpemax = new List<CSLoad_Free>(6);
            List<CSLoad_Free> surfaceWindLoad_ULS_PlusY_Cpemax = new List<CSLoad_Free>(6);
            List<CSLoad_Free> surfaceWindLoad_ULS_MinusY_Cpemax = new List<CSLoad_Free>(6);

            SetSurfaceWindLoads_Cpe(
            listOfLoadedMemberTypeDataRoof,
            listOfLoadedMemberTypeDataWallLeftRight,
            listOfLoadedMemberTypeDataWallFront,
            listOfLoadedMemberTypeDataWallBack,
            ELSType.eLS_ULS,
            pRoofFrontLeft,
            pRoofFrontApex,
            pRoofFrontRight,
            pRoofBackLeft,
            pRoofBackApex,
            pRoofBackRight,
            fRoof_X,
            fRoof_Y,
            pWallFrontLeft,
            pWallFrontRight,
            pWallBackRight,
            pWallBackLeft,
            fWallLeftOrRight_X,
            fWallLeftOrRight_Y,
            fWallFrontOrBack_X,
            fWallFrontOrBack_Y1,
            fWallFrontOrBack_Y2,
            wind,
            out surfaceWindLoad_ULS_PlusX_Cpemin,
            out surfaceWindLoad_ULS_MinusX_Cpemin,
            out surfaceWindLoad_ULS_PlusY_Cpemin,
            out surfaceWindLoad_ULS_MinusY_Cpemin,
            out surfaceWindLoad_ULS_PlusX_Cpemax,
            out surfaceWindLoad_ULS_MinusX_Cpemax,
            out surfaceWindLoad_ULS_PlusY_Cpemax,
            out surfaceWindLoad_ULS_MinusY_Cpemax
            );

            // SLS
            // Cpe, min
            List<CSLoad_Free> surfaceWindLoad_SLS_PlusX_Cpemin = new List<CSLoad_Free>(6);
            List<CSLoad_Free> surfaceWindLoad_SLS_MinusX_Cpemin = new List<CSLoad_Free>(6);
            List<CSLoad_Free> surfaceWindLoad_SLS_PlusY_Cpemin = new List<CSLoad_Free>(6);
            List<CSLoad_Free> surfaceWindLoad_SLS_MinusY_Cpemin = new List<CSLoad_Free>(6);

            // Cpe, max
            List<CSLoad_Free> surfaceWindLoad_SLS_PlusX_Cpemax = new List<CSLoad_Free>(6);
            List<CSLoad_Free> surfaceWindLoad_SLS_MinusX_Cpemax = new List<CSLoad_Free>(6);
            List<CSLoad_Free> surfaceWindLoad_SLS_PlusY_Cpemax = new List<CSLoad_Free>(6);
            List<CSLoad_Free> surfaceWindLoad_SLS_MinusY_Cpemax = new List<CSLoad_Free>(6);

            SetSurfaceWindLoads_Cpe(
            listOfLoadedMemberTypeDataRoof,
            listOfLoadedMemberTypeDataWallLeftRight,
            listOfLoadedMemberTypeDataWallFront,
            listOfLoadedMemberTypeDataWallBack,
            ELSType.eLS_SLS,
            pRoofFrontLeft,
            pRoofFrontApex,
            pRoofFrontRight,
            pRoofBackLeft,
            pRoofBackApex,
            pRoofBackRight,
            fRoof_X,
            fRoof_Y,
            pWallFrontLeft,
            pWallFrontRight,
            pWallBackRight,
            pWallBackLeft,
            fWallLeftOrRight_X,
            fWallLeftOrRight_Y,
            fWallFrontOrBack_X,
            fWallFrontOrBack_Y1,
            fWallFrontOrBack_Y2,
            wind,
            out surfaceWindLoad_SLS_PlusX_Cpemin,
            out surfaceWindLoad_SLS_MinusX_Cpemin,
            out surfaceWindLoad_SLS_PlusY_Cpemin,
            out surfaceWindLoad_SLS_MinusY_Cpemin,
            out surfaceWindLoad_SLS_PlusX_Cpemax,
            out surfaceWindLoad_SLS_MinusX_Cpemax,
            out surfaceWindLoad_SLS_PlusY_Cpemax,
            out surfaceWindLoad_SLS_MinusY_Cpemax
            );
            #endregion

            #region Earthquake - nodal loads
            // Earthquake
            int iNumberOfLoadsInXDirection = iFrameNo;
            int iNumberOfLoadsInYDirection = 2;

            List<CNLoad> nodalLoadEQ_ULS_PlusX = new List<CNLoad>(iNumberOfLoadsInXDirection);
            List<CNLoad> nodalLoadEQ_ULS_PlusY = new List<CNLoad>(iNumberOfLoadsInYDirection);

            List<CNLoad> nodalLoadEQ_SLS_PlusX = new List<CNLoad>(iNumberOfLoadsInXDirection);
            List<CNLoad> nodalLoadEQ_SLS_PlusY = new List<CNLoad>(iNumberOfLoadsInYDirection);

            for (int i = 0; i < iNumberOfLoadsInXDirection; i++)
            {
                nodalLoadEQ_ULS_PlusX.Add(new CNLoadSingle(m_arrNodes[i * 5 + 1], ENLoadType.eNLT_Fx, eq.fV_x_ULS_stregnth, true, 0));
                nodalLoadEQ_SLS_PlusX.Add(new CNLoadSingle(m_arrNodes[i * 5 + 1], ENLoadType.eNLT_Fx, eq.fV_x_SLS, true, 0));
            }

            for (int i = 0; i < iNumberOfLoadsInYDirection; i++)
            {
                nodalLoadEQ_ULS_PlusY.Add(new CNLoadSingle(m_arrNodes[i * 2 + 1], ENLoadType.eNLT_Fy, eq.fV_y_ULS_stregnth, true, 0));
                nodalLoadEQ_SLS_PlusY.Add(new CNLoadSingle(m_arrNodes[i * 2 + 1], ENLoadType.eNLT_Fy, eq.fV_y_SLS, true, 0));
            }
            #endregion

            #region Load Cases
            // Load Cases
            m_arrLoadCases = new CLoadCase[44];
            m_arrLoadCases[00] = new CLoadCase(01, "Dead load G", ELCGTypeForLimitState.eUniversal, ELCType.ePermanentLoad, ELCMainDirection.eGeneral, surfaceDeadLoad);                                                          // 01
            m_arrLoadCases[01] = new CLoadCase(02, "Imposed load Q", ELCGTypeForLimitState.eUniversal, ELCType.eImposedLoad_ST, ELCMainDirection.eGeneral, surfaceRoofImposedLoad);                                               // 02

            // ULS - Load Case
            m_arrLoadCases[02] = new CLoadCase(03, "Snow load Su - full", ELCGTypeForLimitState.eULSOnly, ELCType.eSnow, ELCMainDirection.ePlusZ, surfaceRoofSnowLoad_ULS_Nu_1);                                                // 03
            m_arrLoadCases[03] = new CLoadCase(04, "Snow load Su - left", ELCGTypeForLimitState.eULSOnly, ELCType.eSnow, ELCMainDirection.ePlusZ, surfaceRoofSnowLoad_ULS_Nu_2_Left);                                           // 04
            m_arrLoadCases[04] = new CLoadCase(05, "Snow load Su - right", ELCGTypeForLimitState.eULSOnly, ELCType.eSnow, ELCMainDirection.ePlusZ, surfaceRoofSnowLoad_ULS_Nu_2_Right);                                         // 05
            m_arrLoadCases[05] = new CLoadCase(06, "Wind load Wu - Cpi,min - Left - X+", ELCGTypeForLimitState.eULSOnly, ELCType.eWind, ELCMainDirection.ePlusX, surfaceWindLoad_ULS_PlusX_Cpimin);                             // 06
            m_arrLoadCases[06] = new CLoadCase(07, "Wind load Wu - Cpi,min - Right - X-", ELCGTypeForLimitState.eULSOnly, ELCType.eWind, ELCMainDirection.eMinusX, surfaceWindLoad_ULS_MinusX_Cpimin);                          // 07
            m_arrLoadCases[07] = new CLoadCase(08, "Wind load Wu - Cpi,min - Front - Y+", ELCGTypeForLimitState.eULSOnly, ELCType.eWind, ELCMainDirection.ePlusY, surfaceWindLoad_ULS_PlusY_Cpimin);                            // 08
            m_arrLoadCases[08] = new CLoadCase(09, "Wind load Wu - Cpi,min - Rear - Y-", ELCGTypeForLimitState.eULSOnly, ELCType.eWind, ELCMainDirection.eMinusY, surfaceWindLoad_ULS_MinusY_Cpimin);                           // 09
            m_arrLoadCases[09] = new CLoadCase(10, "Wind load Wu - Cpi,max - Left - X+", ELCGTypeForLimitState.eULSOnly, ELCType.eWind, ELCMainDirection.ePlusX, surfaceWindLoad_ULS_PlusX_Cpimax);                             // 10
            m_arrLoadCases[10] = new CLoadCase(11, "Wind load Wu - Cpi,max - Right - X-", ELCGTypeForLimitState.eULSOnly, ELCType.eWind, ELCMainDirection.eMinusX, surfaceWindLoad_ULS_MinusX_Cpimax);                          // 11
            m_arrLoadCases[11] = new CLoadCase(12, "Wind load Wu - Cpi,max - Front - Y+", ELCGTypeForLimitState.eULSOnly, ELCType.eWind, ELCMainDirection.ePlusY, surfaceWindLoad_ULS_PlusY_Cpimax);                            // 12
            m_arrLoadCases[12] = new CLoadCase(13, "Wind load Wu - Cpi,max - Rear - Y-", ELCGTypeForLimitState.eULSOnly, ELCType.eWind, ELCMainDirection.eMinusY, surfaceWindLoad_ULS_MinusY_Cpimax);                           // 13
            m_arrLoadCases[13] = new CLoadCase(14, "Wind load Wu - Cpe,min - Left - X+", ELCGTypeForLimitState.eULSOnly, ELCType.eWind, ELCMainDirection.ePlusX, surfaceWindLoad_ULS_PlusX_Cpemin);                             // 14
            m_arrLoadCases[14] = new CLoadCase(15, "Wind load Wu - Cpe,min - Right - X-", ELCGTypeForLimitState.eULSOnly, ELCType.eWind, ELCMainDirection.eMinusX, surfaceWindLoad_ULS_MinusX_Cpemin);                          // 15
            m_arrLoadCases[15] = new CLoadCase(16, "Wind load Wu - Cpe,min - Front - Y+", ELCGTypeForLimitState.eULSOnly, ELCType.eWind, ELCMainDirection.ePlusY, surfaceWindLoad_ULS_PlusY_Cpemin);                            // 16
            m_arrLoadCases[16] = new CLoadCase(17, "Wind load Wu - Cpe,min - Rear - Y-", ELCGTypeForLimitState.eULSOnly, ELCType.eWind, ELCMainDirection.eMinusY, surfaceWindLoad_ULS_MinusY_Cpemin);                           // 17
            m_arrLoadCases[17] = new CLoadCase(18, "Wind load Wu - Cpe,max - Left - X+", ELCGTypeForLimitState.eULSOnly, ELCType.eWind, ELCMainDirection.ePlusX, surfaceWindLoad_ULS_PlusX_Cpemax);                             // 18
            m_arrLoadCases[18] = new CLoadCase(19, "Wind load Wu - Cpe,max - Right - X-", ELCGTypeForLimitState.eULSOnly, ELCType.eWind, ELCMainDirection.eMinusX, surfaceWindLoad_ULS_MinusX_Cpemax);                          // 19
            m_arrLoadCases[19] = new CLoadCase(20, "Wind load Wu - Cpe,max - Front - Y+", ELCGTypeForLimitState.eULSOnly, ELCType.eWind, ELCMainDirection.ePlusY, surfaceWindLoad_ULS_PlusY_Cpemax);                            // 20
            m_arrLoadCases[20] = new CLoadCase(21, "Wind load Wu - Cpe,max - Rear - Y-", ELCGTypeForLimitState.eULSOnly, ELCType.eWind, ELCMainDirection.eMinusY, surfaceWindLoad_ULS_MinusY_Cpemax);                           // 21
            m_arrLoadCases[21] = new CLoadCase(22, "Earthquake load Eu - X", ELCGTypeForLimitState.eULSOnly, ELCType.eEarthquake, ELCMainDirection.ePlusX, nodalLoadEQ_ULS_PlusX);                                              // 22
            m_arrLoadCases[22] = new CLoadCase(23, "Earthquake load Eu - Y", ELCGTypeForLimitState.eULSOnly, ELCType.eEarthquake, ELCMainDirection.ePlusY, nodalLoadEQ_ULS_PlusY);                                              // 23

            // SLS - Load Case
            m_arrLoadCases[23] = new CLoadCase(24, "Snow load Ss - full", ELCGTypeForLimitState.eSLSOnly, ELCType.eSnow, ELCMainDirection.ePlusZ, surfaceRoofSnowLoad_SLS_Nu_1);                                                // 24
            m_arrLoadCases[24] = new CLoadCase(25, "Snow load Ss - left", ELCGTypeForLimitState.eSLSOnly, ELCType.eSnow, ELCMainDirection.ePlusZ, surfaceRoofSnowLoad_SLS_Nu_2_Left);                                           // 25
            m_arrLoadCases[25] = new CLoadCase(26, "Snow load Ss - right", ELCGTypeForLimitState.eSLSOnly, ELCType.eSnow, ELCMainDirection.ePlusZ, surfaceRoofSnowLoad_SLS_Nu_2_Right);                                         // 26
            m_arrLoadCases[26] = new CLoadCase(27, "Wind load Ws - Cpi,min - Left - X+", ELCGTypeForLimitState.eSLSOnly, ELCType.eWind, ELCMainDirection.ePlusX, surfaceWindLoad_SLS_PlusX_Cpimin);                             // 27
            m_arrLoadCases[27] = new CLoadCase(28, "Wind load Ws - Cpi,min - Right - X-", ELCGTypeForLimitState.eSLSOnly, ELCType.eWind, ELCMainDirection.eMinusX, surfaceWindLoad_SLS_MinusX_Cpimin);                          // 28
            m_arrLoadCases[28] = new CLoadCase(29, "Wind load Ws - Cpi,min - Front - Y+", ELCGTypeForLimitState.eSLSOnly, ELCType.eWind, ELCMainDirection.ePlusY, surfaceWindLoad_SLS_PlusY_Cpimin);                            // 29
            m_arrLoadCases[29] = new CLoadCase(30, "Wind load Ws - Cpi,min - Rear - Y-", ELCGTypeForLimitState.eSLSOnly, ELCType.eWind, ELCMainDirection.eMinusY, surfaceWindLoad_SLS_MinusY_Cpimin);                           // 30
            m_arrLoadCases[30] = new CLoadCase(31, "Wind load Ws - Cpi,max - Left - X+", ELCGTypeForLimitState.eSLSOnly, ELCType.eWind, ELCMainDirection.ePlusX, surfaceWindLoad_SLS_PlusX_Cpimax);                             // 31
            m_arrLoadCases[31] = new CLoadCase(32, "Wind load Ws - Cpi,max - Right - X-", ELCGTypeForLimitState.eSLSOnly, ELCType.eWind, ELCMainDirection.eMinusX, surfaceWindLoad_SLS_MinusX_Cpimax);                          // 32
            m_arrLoadCases[32] = new CLoadCase(33, "Wind load Ws - Cpi,max - Front - Y+", ELCGTypeForLimitState.eSLSOnly, ELCType.eWind, ELCMainDirection.ePlusY, surfaceWindLoad_SLS_PlusY_Cpimax);                            // 33
            m_arrLoadCases[33] = new CLoadCase(34, "Wind load Ws - Cpi,max - Rear - Y-", ELCGTypeForLimitState.eSLSOnly, ELCType.eWind, ELCMainDirection.eMinusY, surfaceWindLoad_SLS_MinusY_Cpimax);                           // 34
            m_arrLoadCases[34] = new CLoadCase(35, "Wind load Ws - Cpe,min - Left - X+", ELCGTypeForLimitState.eSLSOnly, ELCType.eWind, ELCMainDirection.ePlusX, surfaceWindLoad_SLS_PlusX_Cpemin);                             // 35
            m_arrLoadCases[35] = new CLoadCase(36, "Wind load Ws - Cpe,min - Right - X-", ELCGTypeForLimitState.eSLSOnly, ELCType.eWind, ELCMainDirection.eMinusX, surfaceWindLoad_SLS_MinusX_Cpemin);                          // 36
            m_arrLoadCases[36] = new CLoadCase(37, "Wind load Ws - Cpe,min - Front - Y+", ELCGTypeForLimitState.eSLSOnly, ELCType.eWind, ELCMainDirection.ePlusY, surfaceWindLoad_SLS_PlusY_Cpemin);                            // 37
            m_arrLoadCases[37] = new CLoadCase(38, "Wind load Ws - Cpe,min - Rear - Y-", ELCGTypeForLimitState.eSLSOnly, ELCType.eWind, ELCMainDirection.eMinusY,surfaceWindLoad_SLS_MinusY_Cpemin);                            // 38
            m_arrLoadCases[38] = new CLoadCase(39, "Wind load Ws - Cpe,max - Left - X+", ELCGTypeForLimitState.eSLSOnly, ELCType.eWind, ELCMainDirection.ePlusX, surfaceWindLoad_SLS_PlusX_Cpemax);                             // 39
            m_arrLoadCases[39] = new CLoadCase(40, "Wind load Ws - Cpe,max - Right - X-", ELCGTypeForLimitState.eSLSOnly, ELCType.eWind, ELCMainDirection.eMinusX, surfaceWindLoad_SLS_MinusX_Cpemax);                          // 40
            m_arrLoadCases[40] = new CLoadCase(41, "Wind load Ws - Cpe,max - Front - Y+", ELCGTypeForLimitState.eSLSOnly, ELCType.eWind, ELCMainDirection.ePlusY, surfaceWindLoad_SLS_PlusY_Cpemax);                            // 41
            m_arrLoadCases[41] = new CLoadCase(42, "Wind load Ws - Cpe,max - Rear - Y-", ELCGTypeForLimitState.eSLSOnly, ELCType.eWind, ELCMainDirection.eMinusY, surfaceWindLoad_SLS_MinusY_Cpemax);                           // 42
            m_arrLoadCases[42] = new CLoadCase(43, "Earthquake load Es - X", ELCGTypeForLimitState.eSLSOnly, ELCType.eEarthquake, ELCMainDirection.ePlusX, nodalLoadEQ_SLS_PlusX);                                              // 43
            m_arrLoadCases[43] = new CLoadCase(44, "Earthquake load Es - Y", ELCGTypeForLimitState.eSLSOnly, ELCType.eEarthquake, ELCMainDirection.ePlusY, nodalLoadEQ_SLS_PlusY);                                              // 44
            #endregion

            #region POKUS TODO 186 - Generating of member load from surface load (girts and purlins)
            // TO Ondrej - 7.2.2019
            // Toto je pokus o generovanie prutoveho zatazenia z plosneho
            // Jedna sa o zoznamy prutov typu girts a typu purlins
            // Problem je v tom suradnice bodov rovin beriem z celej stavby, by sa mali preberat priamo z objektu CSLoad_FreeUniform.cs,
            // Mozes sa tymto insprirovat ale treba to vyladit

            // TODO No. 54
            // tieto zoznamy sa maju nahradit funckiou v TODO 54 ktora ich vytvori pre jednotlive zatazovacie plochy zo suradnic ploch
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

            // TODO No 49 and 50 - in work, naplnit zoznamy prutov ktore lezia v rovine definujucej zatazenie, presnost 1 mm

            // Loading width of member (Zatazovacia sirka pruta)
            float fLoadingWidthPurlin = fDist_Purlin;
            float fLoadingWidthEdgePurlin_Roof = 0.5f * fDist_Purlin;
            float fLoadingWidthEdgePurlin_Wall = 0.5f * fDist_Girt;
            float fLoadingWidthGirt = fDist_Girt;

            // TODO No. 54 - po implementacii tento cyklus odstranit a napojit metodu 
            // FillListOfMemberData z CSLoad_Free.cs

            foreach (CMember m in m_arrMembers)
            {
                ///////////////////////////////////////////////////////////////////////////////////////////////////
                // Girts
                ///////////////////////////////////////////////////////////////////////////////////////////////////

                // List of all girts
                if (m.EMemberType == EMemberType_FormSteel.eG)
                    listOfGirts.Add(m);

                // TODO - Ondrej tieto suradnice by sa mali preberat priamo z objektu CSLoad_FreeUniform.cs, transformacna funkcia 
                // CreateTransformCoordGroup

                p1 = new Point3D(pWallFrontLeft.X, pWallFrontLeft.Y, pWallFrontLeft.Z);
                p2 = new Point3D(pRoofFrontLeft.X, pRoofFrontLeft.Y, pRoofFrontLeft.Z);
                p3 = new Point3D(pRoofBackLeft.X, pRoofBackLeft.Y, pRoofBackLeft.Z);

                // List of girts - left wall
                if (m.EMemberType == EMemberType_FormSteel.eG && Drawing3D.MemberLiesOnPlane(p1, p2, p3, m, 0.001))
                    listOfGirtsLeftSide.Add(m);

                p1 = new Point3D(pWallFrontRight.X, pWallFrontRight.Y, pWallFrontRight.Z);
                p2 = new Point3D(pRoofFrontRight.X, pRoofFrontRight.Y, pRoofFrontRight.Z);
                p3 = new Point3D(pRoofBackRight.X, pRoofBackRight.Y, pRoofBackRight.Z);

                // List of girts - right wall
                if (m.EMemberType == EMemberType_FormSteel.eG && Drawing3D.MemberLiesOnPlane(p1, p2, p3, m, 0.001))
                    listOfGirtsRightSide.Add(m);

                p1 = new Point3D(pWallFrontLeft.X, pWallFrontLeft.Y, pWallFrontLeft.Z);
                p2 = new Point3D(pWallFrontRight.X, pWallFrontRight.Y, pWallFrontRight.Z);
                p3 = new Point3D(pRoofFrontRight.X, pRoofFrontRight.Y, pRoofFrontRight.Z);

                // List of girts - front wall
                if (m.EMemberType == EMemberType_FormSteel.eG && Drawing3D.MemberLiesOnPlane(p1, p2, p3, m, 0.001))
                    listOfGirtsFrontSide.Add(m);

                p1 = new Point3D(pWallBackLeft.X, pWallBackLeft.Y, pWallBackLeft.Z);
                p2 = new Point3D(pWallBackRight.X, pWallBackRight.Y, pWallBackRight.Z);
                p3 = new Point3D(pRoofBackRight.X, pRoofBackRight.Y, pRoofBackRight.Z);

                // List of girts - back wall
                if (m.EMemberType == EMemberType_FormSteel.eG && Drawing3D.MemberLiesOnPlane(p1, p2, p3, m, 0.001))
                    listOfGirtsBackSide.Add(m);

                ///////////////////////////////////////////////////////////////////////////////////////////////////
                // Purlins
                ///////////////////////////////////////////////////////////////////////////////////////////////////

                // List of all purlins
                if (m.EMemberType == EMemberType_FormSteel.eP)
                    listOfPurlins.Add(m);

                // List of all edge purlins
                if (m.EMemberType == EMemberType_FormSteel.eEP)
                    listOfEavePurlins.Add(m);

                p1 = new Point3D(pRoofFrontLeft.X, pRoofFrontLeft.Y, pRoofFrontLeft.Z);
                p2 = new Point3D(pRoofFrontApex.X, pRoofFrontApex.Y, pRoofFrontApex.Z);
                p3 = new Point3D(pRoofBackApex.X, pRoofBackApex.Y, pRoofBackApex.Z);

                // List of purlins - left side of the roof
                if (m.EMemberType == EMemberType_FormSteel.eP && Drawing3D.MemberLiesOnPlane(p1, p2, p3, m, 0.001))
                    listOfPurlinsLeftSide.Add(m);

                // List of edge purlins - left side of the roof (tento zoznam pouzit aj pre zatazenie lavej steny)
                if (m.EMemberType == EMemberType_FormSteel.eEP && Drawing3D.MemberLiesOnPlane(p1, p2, p3, m, 0.001))
                    listOfEavePurlinsLeftSide.Add(m);

                p1 = new Point3D(pRoofFrontApex.X, pRoofFrontApex.Y, pRoofFrontApex.Z);
                p2 = new Point3D(pRoofFrontRight.X, pRoofFrontRight.Y, pRoofFrontRight.Z);
                p3 = new Point3D(pRoofBackRight.X, pRoofBackRight.Y, pRoofBackRight.Z);

                // List of purlins - right side of the roof
                if (m.EMemberType == EMemberType_FormSteel.eP && Drawing3D.MemberLiesOnPlane(p1, p2, p3, m, 0.001))
                    listOfPurlinsRightSide.Add(m);

                // List of edge purlins - right side of the roof (tento zoznam pouzit aj pre zatazenie pravej steny)
                if (m.EMemberType == EMemberType_FormSteel.eEP && Drawing3D.MemberLiesOnPlane(p1, p2, p3, m, 0.001))
                    listOfEavePurlinsRightSide.Add(m);
            }

            // TODO 50
            // Ukazka - purlin - imposed load
            // Preorganizovat properties v triedach surface load tak, aby sa dalo dostat k hodnote zatazenia a prenasobit vzdialenostou medzi vaznicami
            // Vypocitane zatazenie priradit prutom zo zoznamu listOfPurlins v Load Case v m_arrLoadCases[01]
            #endregion

            // TODO - Ondrej, pripravit staticku triedu a metody pre generovanie member load zo surface load v zlozke Loading
            // TODO 186 - To Ondrej - Tu je trieda v ktorej by mohlo byt zapracovane generovanie zatazenia

            // Generator prutoveho zatazenia z plosneho zatazenia by mohol byt niekde stranou v tomto CExample je toto uz velmi vela
            // Pre urcenie spravneho znamienka generovaneho member load bude potrebne poznat uhol medzi normalou plochy definujucej zatazenie a osovym systemom pruta

            bool bGenerateLoadsOnPurlinsAndGirts = false;

            if(bGenerateLoadsOnPurlinsAndGirts)
            CLoadGenerator.GenerateMemberLoads(m_arrLoadCases, listOfPurlins, fDist_Purlin);

            bool bGenerateLoadsOnFrameMembers = true;
            if (bGenerateLoadsOnFrameMembers)
            {
                GenerateLoadsOnFrames(
                    -generalLoad.fDeadLoadTotal_Wall,
                    -generalLoad.fDeadLoadTotal_Roof,
                    -generalLoad.fImposedLoadTotal_Roof,
                    fsnowULS_Nu_1,
                    fsnowULS_Nu_2,
                    fsnowSLS_Nu_1,
                    fsnowSLS_Nu_2,
                    out memberLoadDead_Frames,
                    out memberLoadImposed_Frames,
                    out memberMaxLoadSnowAll_Frames,
                    out memberMaxLoadSnowLeft_Frames,
                    out memberMaxLoadSnowRight_Frames);

                // Assign generated member loads to the load cases

                m_arrLoadCases[00].MemberLoadsList = memberLoadDead_Frames;
                m_arrLoadCases[01].MemberLoadsList = memberLoadImposed_Frames;
                m_arrLoadCases[02].MemberLoadsList = memberMaxLoadSnowAll_Frames;
                m_arrLoadCases[03].MemberLoadsList = memberMaxLoadSnowLeft_Frames;
                m_arrLoadCases[04].MemberLoadsList = memberMaxLoadSnowRight_Frames;
            }

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
                m_arrNodes[i_temp_numberofNodes + iColumnNoInOneFrame + iOneRafterColumnNo + i] = new CNode(i_temp_numberofNodes + 3 * iOneRafterColumnNo + i + 2, fW_frame - ((i + 1) * fDist_Columns), fy_Global_Coord, z_glob, 0);
                RotateFrontOrBackFrameNodeAboutZ(m_arrNodes[i_temp_numberofNodes + iColumnNoInOneFrame + iOneRafterColumnNo + i]);
            }
        }

        public void AddColumnsMembers(int i_temp_numberofNodes, int i_temp_numberofMembers, int iOneRafterColumnNo, int iColumnNoInOneFrame, CMemberEccentricity eccentricityColumn, float fColumnAlignmentStart, float fColumnAlignmentEnd, CCrSc section, float fMemberRotation)
        {
            // Members - Columns
            for (int i = 0; i < iOneRafterColumnNo; i++)
            {
                m_arrMembers[i_temp_numberofMembers + i] = new CMember(i_temp_numberofMembers + i + 1, m_arrNodes[i_temp_numberofNodes + i], m_arrNodes[i_temp_numberofNodes + iColumnNoInOneFrame + i], section, EMemberType_FormSteel.eC, eccentricityColumn, eccentricityColumn, fColumnAlignmentStart, fColumnAlignmentEnd, fMemberRotation, 0);
            }

            for (int i = 0; i < iOneRafterColumnNo; i++)
            {
                m_arrMembers[i_temp_numberofMembers + iOneRafterColumnNo + i] = new CMember(i_temp_numberofMembers + iOneRafterColumnNo + i + 1, m_arrNodes[i_temp_numberofNodes + iOneRafterColumnNo + i], m_arrNodes[i_temp_numberofNodes + iColumnNoInOneFrame + iOneRafterColumnNo + i], section, EMemberType_FormSteel.eC, eccentricityColumn, eccentricityColumn, fColumnAlignmentStart, fColumnAlignmentEnd, fMemberRotation, 0);
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
                    m_arrNodes[i_temp_numberofNodes + iTemp + j] = new CNode(i_temp_numberofNodes + iTemp + j, (i + 1) * fDist_Columns, fy_Global_Coord, fBottomGirtPosition + j * fDist_Girts, 0);
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
                        m_arrMembers[i_temp_numberofMembers + j] = new CMember(i_temp_numberofMembers + j + 1, m_arrNodes[iFrameNodesNo * iFrameNo + iTempJumpBetweenFrontAndBack_GirtsNumberInLongidutinalDirection + j], m_arrNodes[i_temp_numberofNodes + j], section, EMemberType_FormSteel.eG, eGirtEccentricity, eGirtEccentricity, fGirtStart_MC, fGirtEnd, fMemberRotation, 0);
                    }

                    iTemp += iOneColumnGirtNo_temp;
                }
                else if (i < iOneRafterColumnNo) // Other sessions
                {
                    for (int j = 0; j < iArrNumberOfNodesPerColumn[i - 1]; j++)
                    {
                        m_arrMembers[i_temp_numberofMembers + iTemp + j] = new CMember(i_temp_numberofMembers + iTemp + j + 1, m_arrNodes[i_temp_numberofNodes + iTemp2 + j], m_arrNodes[i_temp_numberofNodes + iArrNumberOfNodesPerColumn[i - 1] + iTemp2 + j], section, EMemberType_FormSteel.eG, eGirtEccentricity, eGirtEccentricity, fGirtStart, fGirtEnd, fMemberRotation, 0);
                    }

                    iTemp2 += iArrNumberOfNodesPerColumn[i - 1];
                    iTemp += iArrNumberOfNodesPerColumn[i - 1];
                }
                else // Last session - prechadza cez stred budovy
                {
                    for (int j = 0; j < iArrNumberOfNodesPerColumn[i - 1]; j++)
                    {
                        m_arrMembers[i_temp_numberofMembers + iTemp + j] = new CMember(i_temp_numberofMembers + iTemp + j + 1, m_arrNodes[i_temp_numberofNodes + iTemp2 + j], m_arrNodes[i_temp_numberofNodes + iIntermediateColumnNodesForGirtsOneFrameNo - iArrNumberOfNodesPerColumn[iOneRafterColumnNo - 1] + j], section, EMemberType_FormSteel.eG, eGirtEccentricity, eGirtEccentricity, fGirtStart, fGirtEnd, fMemberRotation, 0);
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
                        m_arrMembers[i_temp_numberofMembers + iNumberOfMembers_temp + j] = new CMember(i_temp_numberofMembers + iNumberOfMembers_temp + j + 1, m_arrNodes[iFrameNodesNo * iFrameNo + iTempJumpBetweenFrontAndBack_GirtsNumberInLongidutinalDirection + iOneColumnGirtNo_temp + j], m_arrNodes[i_temp_numberofNodes + iIntermediateColumnNodesForGirtsOneRafterNo + j], section, EMemberType_FormSteel.eG, eGirtEccentricity, eGirtEccentricity, fGirtStart_MC, fGirtEnd, -fMemberRotation, 0);
                    }

                    iTemp += iOneColumnGirtNo_temp;
                }
                else // Other sessions (not in the middle)
                {
                    for (int j = 0; j < iArrNumberOfNodesPerColumn[i - 1]; j++)
                    {
                        m_arrMembers[i_temp_numberofMembers + iNumberOfMembers_temp + iTemp + j] = new CMember(i_temp_numberofMembers + iNumberOfMembers_temp + iTemp + j + 1, m_arrNodes[i_temp_numberofNodes + iIntermediateColumnNodesForGirtsOneRafterNo + iTemp2 + j], m_arrNodes[i_temp_numberofNodes + iIntermediateColumnNodesForGirtsOneRafterNo + iArrNumberOfNodesPerColumn[i - 1] + iTemp2 + j], section, EMemberType_FormSteel.eG, eGirtEccentricity, eGirtEccentricity, fGirtStart, fGirtEnd, -fMemberRotation, 0);
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

        public void AddDoorBlock(PropertiesToInsertOpening insertprop, DoorProperties prop, float fLimitDistanceFromColumn)
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

        // Loading

        // Main Columns
        public void GenerateLoadOnMainColumns_Old(float fValue1, float fValue2, ref List<CMLoad> list)
        {
            for (int i = 0; i < iFrameNo; i++)
            {
                CMLoad loadleft = new CMLoad_21(i *2 + 1, fValue1, m_arrMembers[i * (2 + 2 + 2)], EMLoadTypeDistr.eMLT_QUF_W_21, EMLoadType.eMLT_F, EMLoadDirPCC1.eMLD_PCC_FZV_MYU, true, 0);
                CMLoad loadright = new CMLoad_21(i * 2 + 2, fValue2, m_arrMembers[3 + i * (2 + 2 + 2)], EMLoadTypeDistr.eMLT_QUF_W_21, EMLoadType.eMLT_F, EMLoadDirPCC1.eMLD_PCC_FZV_MYU, true, 0);
                list.Add(loadleft);
                list.Add(loadright);
            }
        }

        // Rafters
        public void GenerateLoadOnRafters_Old(float fValue1, float fValue2, ref List<CMLoad> list)
        {
            for (int i = 0; i < iFrameNo; i++)
            {
                CMLoad loadleft = new CMLoad_21(i * 2 + 1, fValue1, m_arrMembers[1 + i * (2 + 2 + 2)], EMLoadTypeDistr.eMLT_QUF_W_21, EMLoadType.eMLT_F, EMLoadDirPCC1.eMLD_PCC_FZV_MYU, true, 0);
                CMLoad loadright = new CMLoad_21(i * 2 + 2, fValue2, m_arrMembers[1 + i * (2 + 2 + 2) + 1], EMLoadTypeDistr.eMLT_QUF_W_21, EMLoadType.eMLT_F, EMLoadDirPCC1.eMLD_PCC_FZV_MYU, true, 0);
                list.Add(loadleft);
                list.Add(loadright);
            }
        }

        // Frames
        public void GenerateLoadOnFrames_Old(float fValueColumn1, float fValueColumn2, float fValueRafter1, float fValueRafter2, ref List<CMLoad> list)
        {
            GenerateLoadOnMainColumns_Old(fValueColumn1, fValueColumn2, ref list);
            GenerateLoadOnRafters_Old(fValueRafter1, fValueRafter2, ref list);
        }

        public void GenerateLoadsOnFrames(
            float fValueLoadColumnDead,
            float fValueLoadRafterDead,
            float fValueLoadRafterImposed,
            float fValueLoadRafterSnowULS_Nu_1,
            float fValueLoadRafterSnowULS_Nu_2,
            float fValueLoadRafterSnowSLS_Nu_1,
            float fValueLoadRafterSnowSLS_Nu_2,

            out List<CMLoad> memberLoadDead,
            out List<CMLoad> memberLoadImposed,
            out List<CMLoad> memberMaxLoadSnowAll_ULS,
            out List<CMLoad> memberMaxLoadSnowLeft_ULS,
            out List<CMLoad> memberMaxLoadSnowRight_ULS
            )
        {
            memberLoadDead = new List<CMLoad>();
            memberLoadImposed = new List<CMLoad>();
            memberMaxLoadSnowAll_ULS = new List<CMLoad>();
            memberMaxLoadSnowLeft_ULS = new List<CMLoad>();
            memberMaxLoadSnowRight_ULS = new List<CMLoad>();

            for (int i = 0; i < iFrameNo; i++)
            {
                // Create lists of loads on frame members
                List<CMLoad> memberLoadDeadFrame;
                List<CMLoad> memberLoadImposedFrame;
                List<CMLoad> memberMaxLoadSnowAll_ULSFrame;
                List<CMLoad> memberMaxLoadSnowLeft_ULSFrame;
                List<CMLoad> memberMaxLoadSnowRight_ULSFrame;

                // Generate loads on member of particular frame
                GenerateLoadsOnFrame(i,
                fValueLoadColumnDead,
                fValueLoadRafterDead,
                fValueLoadRafterImposed,
                fValueLoadRafterSnowULS_Nu_1,
                fValueLoadRafterSnowULS_Nu_2,
                fValueLoadRafterSnowSLS_Nu_1,
                fValueLoadRafterSnowSLS_Nu_2,
                out memberLoadDeadFrame,
                out memberLoadImposedFrame,
                out memberMaxLoadSnowAll_ULSFrame,
                out memberMaxLoadSnowLeft_ULSFrame,
                out memberMaxLoadSnowRight_ULSFrame);

                // Fill output list - loads on members of all frames
                foreach (CMLoad l in memberLoadDeadFrame)
                    memberLoadDead.Add(l);

                foreach (CMLoad l in memberLoadImposedFrame)
                    memberLoadImposed.Add(l);

                foreach (CMLoad l in memberMaxLoadSnowAll_ULSFrame)
                    memberMaxLoadSnowAll_ULS.Add(l);

                foreach (CMLoad l in memberMaxLoadSnowLeft_ULSFrame)
                    memberMaxLoadSnowLeft_ULS.Add(l);

                foreach (CMLoad l in memberMaxLoadSnowRight_ULSFrame)
                    memberMaxLoadSnowRight_ULS.Add(l);
            }
        }

        public void GenerateLoadsOnFrame(int iFrameIndex,
            float fValueLoadColumnDead,
            float fValueLoadRafterDead,
            float fValueLoadRafterImposed,
            float fValueLoadRafterSnowULS_Nu_1,
            float fValueLoadRafterSnowULS_Nu_2,
            float fValueLoadRafterSnowSLS_Nu_1,
            float fValueLoadRafterSnowSLS_Nu_2,

            out List<CMLoad> memberLoadDead,
            out List<CMLoad> memberLoadImposed,
            out List<CMLoad> memberMaxLoadSnowAll_ULS,
            out List<CMLoad> memberMaxLoadSnowLeft_ULS,
            out List<CMLoad> memberMaxLoadSnowRight_ULS
            )
        {
            float fFrameTributaryWidth = fL1_frame;
            float fFrameGCSCoordinate_Y = iFrameIndex * fL1_frame;

            // Half tributary width - first and last frame
            if (iFrameIndex == 0 || iFrameIndex == iFrameNo-1)
                fFrameTributaryWidth *= 0.5f;

            // Dead Loads
            memberLoadDead = new List<CMLoad>(4);
            // Columns
            CMLoad loadColumnLeft_DL = new CMLoad_21(iFrameIndex * 2 + 1, fValueLoadColumnDead * fFrameTributaryWidth, m_arrMembers[iFrameIndex * (2 + 2 + 2)], EMLoadTypeDistr.eMLT_QUF_W_21, EMLoadType.eMLT_F, EMLoadDirPCC1.eMLD_PCC_FXX_MXX, true, 0);
            CMLoad loadColumnRight_DL = new CMLoad_21(iFrameIndex * 2 + 2, fValueLoadColumnDead * fFrameTributaryWidth, m_arrMembers[3 + iFrameIndex * (2 + 2 + 2)], EMLoadTypeDistr.eMLT_QUF_W_21, EMLoadType.eMLT_F, EMLoadDirPCC1.eMLD_PCC_FXX_MXX, true, 0);
            memberLoadDead.Add(loadColumnLeft_DL);
            memberLoadDead.Add(loadColumnRight_DL);

            // Rafters
            // TODO - zapracovat do konstruktora nastavenie GCS smeru zatazenia, teraz je to nespravne v PCS
            CMLoad loadRafterLeft_DL = new CMLoad_21(iFrameIndex * 2 + 1, fValueLoadRafterDead * fFrameTributaryWidth, m_arrMembers[1 + iFrameIndex * (2 + 2 + 2)], EMLoadTypeDistr.eMLT_QUF_W_21, EMLoadType.eMLT_F, EMLoadDirPCC1.eMLD_PCC_FZV_MYU, true, 0);
            CMLoad loadRafterRight_DL = new CMLoad_21(iFrameIndex * 2 + 2, fValueLoadRafterDead * fFrameTributaryWidth, m_arrMembers[1 + iFrameIndex * (2 + 2 + 2) + 1], EMLoadTypeDistr.eMLT_QUF_W_21, EMLoadType.eMLT_F, EMLoadDirPCC1.eMLD_PCC_FZV_MYU, true, 0);
            memberLoadDead.Add(loadRafterLeft_DL);
            memberLoadDead.Add(loadRafterRight_DL);

            // Imposed Loads - roof
            // Rafters
            // TODO - zapracovat do konstruktora nastavenie GCS smeru zatazenia, teraz je to nespravne v PCS
            CMLoad loadRafterLeft_IL = new CMLoad_21(iFrameIndex * 2 + 1, fValueLoadRafterImposed * fFrameTributaryWidth, m_arrMembers[1 + iFrameIndex * (2 + 2 + 2)], EMLoadTypeDistr.eMLT_QUF_W_21, EMLoadType.eMLT_F, EMLoadDirPCC1.eMLD_PCC_FZV_MYU, true, 0);
            CMLoad loadRafterRight_IL = new CMLoad_21(iFrameIndex * 2 + 2, fValueLoadRafterImposed * fFrameTributaryWidth, m_arrMembers[1 + iFrameIndex * (2 + 2 + 2) + 1], EMLoadTypeDistr.eMLT_QUF_W_21, EMLoadType.eMLT_F, EMLoadDirPCC1.eMLD_PCC_FZV_MYU, true, 0);
            memberLoadImposed = new List<CMLoad>(2);
            memberLoadImposed.Add(loadRafterLeft_IL);
            memberLoadImposed.Add(loadRafterRight_IL);

            // Snow Loads - roof
            // Rafters
            // TODO - zapracovat do konstruktora nastavenie GCS smeru zatazenia, teraz je to nespravne v PCS
            CMLoad loadRafterLeft_SL1_All_ULS = new CMLoad_21(iFrameIndex * 2 + 1, fValueLoadRafterSnowULS_Nu_1 * fFrameTributaryWidth, m_arrMembers[1 + iFrameIndex * (2 + 2 + 2)], EMLoadTypeDistr.eMLT_QUF_W_21, EMLoadType.eMLT_F, EMLoadDirPCC1.eMLD_PCC_FZV_MYU, true, 0);
            CMLoad loadRafterRight_SL1_All_ULS = new CMLoad_21(iFrameIndex * 2 + 2, fValueLoadRafterSnowULS_Nu_1 * fFrameTributaryWidth, m_arrMembers[1 + iFrameIndex * (2 + 2 + 2) + 1], EMLoadTypeDistr.eMLT_QUF_W_21, EMLoadType.eMLT_F, EMLoadDirPCC1.eMLD_PCC_FZV_MYU, true, 0);
            memberMaxLoadSnowAll_ULS = new List<CMLoad>(2);
            memberMaxLoadSnowAll_ULS.Add(loadRafterLeft_SL1_All_ULS);
            memberMaxLoadSnowAll_ULS.Add(loadRafterRight_SL1_All_ULS);

            // Rafters
            // TODO - zapracovat do konstruktora nastavenie GCS smeru zatazenia, teraz je to nespravne v PCS
            CMLoad loadRafterLeft_SL2_Left_ULS = new CMLoad_21(iFrameIndex * 2 + 1, fValueLoadRafterSnowULS_Nu_2 * fFrameTributaryWidth, m_arrMembers[1 + iFrameIndex * (2 + 2 + 2)], EMLoadTypeDistr.eMLT_QUF_W_21, EMLoadType.eMLT_F, EMLoadDirPCC1.eMLD_PCC_FZV_MYU, true, 0);
            memberMaxLoadSnowLeft_ULS = new List<CMLoad>(1);
            memberMaxLoadSnowLeft_ULS.Add(loadRafterLeft_SL2_Left_ULS);

            // Rafters
            // TODO - zapracovat do konstruktora nastavenie GCS smeru zatazenia, teraz je to nespravne v PCS
            CMLoad loadRafterRight_SL3_Right_ULS = new CMLoad_21(iFrameIndex * 2 + 2, fValueLoadRafterSnowULS_Nu_2 * fFrameTributaryWidth, m_arrMembers[1 + iFrameIndex * (2 + 2 + 2) + 1], EMLoadTypeDistr.eMLT_QUF_W_21, EMLoadType.eMLT_F, EMLoadDirPCC1.eMLD_PCC_FZV_MYU, true, 0);
            memberMaxLoadSnowRight_ULS = new List<CMLoad>(1);
            memberMaxLoadSnowRight_ULS.Add(loadRafterRight_SL3_Right_ULS);
        }

        private void SetSurfaceWindLoads_Cpi(
        List<FreeSurfaceLoadsMemberTypeData> listOfLoadedMemberTypeDataRoof,
        List<FreeSurfaceLoadsMemberTypeData> listOfLoadedMemberTypeDataWallLeftRight,
        List<FreeSurfaceLoadsMemberTypeData> listOfLoadedMemberTypeDataWallFront,
        List<FreeSurfaceLoadsMemberTypeData> listOfLoadedMemberTypeDataWallBack,
        ELSType eLSType,
        CPoint pRoofFrontApex,
        CPoint pRoofFrontRight,
        float fRoof_X,
        float fRoof_Y,
        CPoint pWallFrontLeft,
        CPoint pWallBackRight,
        float fWallLeftOrRight_X,
        float fWallLeftOrRight_Y,
        float fWallFrontOrBack_X,
        float fWallFrontOrBack_Y1,
        float fWallFrontOrBack_Y2,
        CCalcul_1170_2 wind,
        out List<CSLoad_Free> surfaceWindLoadPlusX_Cpimin,
        out List<CSLoad_Free> surfaceWindLoadMinusX_Cpimin,
        out List<CSLoad_Free> surfaceWindLoadPlusY_Cpimin,
        out List<CSLoad_Free> surfaceWindLoadMinusY_Cpimin,
        out List<CSLoad_Free> surfaceWindLoadPlusX_Cpimax,
        out List<CSLoad_Free> surfaceWindLoadMinusX_Cpimax,
        out List<CSLoad_Free> surfaceWindLoadPlusY_Cpimax,
        out List<CSLoad_Free> surfaceWindLoadMinusY_Cpimax
        )
        {
            // Cpi, min (underpressure, negative air pressure)
            SetSurfaceWindLoads_Cpi(
            listOfLoadedMemberTypeDataRoof,
            listOfLoadedMemberTypeDataWallLeftRight,
            listOfLoadedMemberTypeDataWallFront,
            listOfLoadedMemberTypeDataWallBack,
            eLSType,
            0,
            pRoofFrontApex,
            pRoofFrontRight,
            fRoof_X,
            fRoof_Y,
            pWallFrontLeft,
            pWallBackRight,
            fWallLeftOrRight_X,
            fWallLeftOrRight_Y,
            fWallFrontOrBack_X,
            fWallFrontOrBack_Y1,
            fWallFrontOrBack_Y2,
            wind,
            out surfaceWindLoadPlusX_Cpimin,
            out surfaceWindLoadMinusX_Cpimin,
            out surfaceWindLoadPlusY_Cpimin,
            out surfaceWindLoadMinusY_Cpimin
            );

            // Cpi, max (overpressure, possitive air pressure)
            SetSurfaceWindLoads_Cpi(
            listOfLoadedMemberTypeDataRoof,
            listOfLoadedMemberTypeDataWallLeftRight,
            listOfLoadedMemberTypeDataWallFront,
            listOfLoadedMemberTypeDataWallBack,
            eLSType,
            1,
            pRoofFrontApex,
            pRoofFrontRight,
            fRoof_X,
            fRoof_Y,
            pWallFrontLeft,
            pWallBackRight,
            fWallLeftOrRight_X,
            fWallLeftOrRight_Y,
            fWallFrontOrBack_X,
            fWallFrontOrBack_Y1,
            fWallFrontOrBack_Y2,
            wind,
            out surfaceWindLoadPlusX_Cpimax,
            out surfaceWindLoadMinusX_Cpimax,
            out surfaceWindLoadPlusY_Cpimax,
            out surfaceWindLoadMinusY_Cpimax
            );
        }

        private void SetSurfaceWindLoads_Cpi(
        List<FreeSurfaceLoadsMemberTypeData> listOfLoadedMemberTypeDataRoof,
        List<FreeSurfaceLoadsMemberTypeData> listOfLoadedMemberTypeDataWallLeftRight,
        List<FreeSurfaceLoadsMemberTypeData> listOfLoadedMemberTypeDataWallFront,
        List<FreeSurfaceLoadsMemberTypeData> listOfLoadedMemberTypeDataWallBack,
        ELSType eLSType,
        int iCodeForCpiMinMaxValue,
        CPoint pRoofFrontApex,
        CPoint pRoofFrontRight,
        float fRoof_X,
        float fRoof_Y,
        CPoint pWallFrontLeft,
        CPoint pWallBackRight,
        float fWallLeftOrRight_X,
        float fWallLeftOrRight_Y,
        float fWallFrontOrBack_X,
        float fWallFrontOrBack_Y1,
        float fWallFrontOrBack_Y2,
        CCalcul_1170_2 wind,
        out List<CSLoad_Free> surfaceWindLoadPlusX_Cpi,
        out List<CSLoad_Free> surfaceWindLoadMinusX_Cpi,
        out List<CSLoad_Free> surfaceWindLoadPlusY_Cpi,
        out List<CSLoad_Free> surfaceWindLoadMinusY_Cpi
        )
        {
            // Wind Load
            Color cColorWindWalls = Colors.Cyan;

            float[] fp_i_roof_Theta_4;

            float[] fp_i_W_wall_Theta_4;
            float[] fp_i_wall_Theta_4;

            if (eLSType == ELSType.eLS_ULS)
            {
                if (iCodeForCpiMinMaxValue == 0) // ULS - Cpi,min
                {
                    // Roof
                    fp_i_roof_Theta_4 = wind.fp_i_min_ULS_Theta_4;

                    // Walls
                    fp_i_W_wall_Theta_4 = wind.fp_i_min_ULS_Theta_4;
                    fp_i_wall_Theta_4 = wind.fp_i_min_ULS_Theta_4;
                }
                else // ULS - Cpi,max
                {
                    // Roof
                    fp_i_roof_Theta_4 = wind.fp_i_max_ULS_Theta_4;

                    // Walls
                    fp_i_W_wall_Theta_4 = wind.fp_i_max_ULS_Theta_4;
                    fp_i_wall_Theta_4 = wind.fp_i_max_ULS_Theta_4;
                }
            }
            else
            {
                if (iCodeForCpiMinMaxValue == 0)  // SLS - Cpi,min
                {
                    // Roof
                    fp_i_roof_Theta_4 = wind.fp_i_min_SLS_Theta_4;

                    // Walls
                    fp_i_W_wall_Theta_4 = wind.fp_i_min_SLS_Theta_4;
                    fp_i_wall_Theta_4 = wind.fp_i_min_SLS_Theta_4;
                }
                else  // SLS - Cpi,max
                {
                    // Roof
                    fp_i_roof_Theta_4 = wind.fp_i_max_SLS_Theta_4;

                    // Walls
                    fp_i_W_wall_Theta_4 = wind.fp_i_max_SLS_Theta_4;
                    fp_i_wall_Theta_4 = wind.fp_i_max_SLS_Theta_4;
                }
            }

            // Cpi
            surfaceWindLoadPlusX_Cpi = new List<CSLoad_Free>(6);
            surfaceWindLoadPlusX_Cpi.Add(new CSLoad_FreeUniform(listOfLoadedMemberTypeDataRoof, ELoadCoordSystem.eLCS, ELoadDir.eLD_Z, pRoofFrontApex, fRoof_X, fRoof_Y, fp_i_roof_Theta_4[(int)ELCMainDirection.ePlusX], -fRoofPitch_rad / (float)Math.PI * 180f, 0, 90, false, false, true, 0));
            surfaceWindLoadPlusX_Cpi.Add(new CSLoad_FreeUniform(listOfLoadedMemberTypeDataRoof, ELoadCoordSystem.eLCS, ELoadDir.eLD_Z, pRoofFrontRight, fRoof_X, fRoof_Y, fp_i_roof_Theta_4[(int)ELCMainDirection.ePlusX], fRoofPitch_rad / (float)Math.PI * 180f, 0, 90, false, false, true, 0));
            surfaceWindLoadPlusX_Cpi.Add(new CSLoad_FreeUniform(listOfLoadedMemberTypeDataWallFront, ELoadCoordSystem.eLCS, ELoadDir.eLD_Z, pWallFrontLeft, fWallLeftOrRight_X, fWallLeftOrRight_Y, fp_i_W_wall_Theta_4[(int)ELCMainDirection.ePlusX], 90, 0, 90, cColorWindWalls, true, true, true, 0));
            surfaceWindLoadPlusX_Cpi.Add(new CSLoad_FreeUniform(listOfLoadedMemberTypeDataWallBack, ELoadCoordSystem.eLCS, ELoadDir.eLD_Z, pWallBackRight, fWallLeftOrRight_X, fWallLeftOrRight_Y, fp_i_wall_Theta_4[(int)ELCMainDirection.ePlusX], 90, 0, 180 + 90, cColorWindWalls, true, true, true, 0));
            surfaceWindLoadPlusX_Cpi.Add(new CSLoad_FreeUniform(listOfLoadedMemberTypeDataWallLeftRight, ELoadCoordSystem.eLCS, ELoadDir.eLD_Z, pWallFrontLeft, fWallFrontOrBack_X, fWallFrontOrBack_Y1, 0.5f * fWallFrontOrBack_X, fWallFrontOrBack_Y2, fWallFrontOrBack_Y1, fp_i_wall_Theta_4[(int)ELCMainDirection.ePlusX], 90, 0, 0, cColorWindWalls, false, false, false, true, 0));
            surfaceWindLoadPlusX_Cpi.Add(new CSLoad_FreeUniform(listOfLoadedMemberTypeDataWallLeftRight, ELoadCoordSystem.eLCS, ELoadDir.eLD_Z, pWallBackRight, fWallFrontOrBack_X, fWallFrontOrBack_Y1, 0.5f * fWallFrontOrBack_X, fWallFrontOrBack_Y2, fWallFrontOrBack_Y1, fp_i_wall_Theta_4[(int)ELCMainDirection.ePlusX], 90, 0, 180, cColorWindWalls, false, false, false, true, 0));

            surfaceWindLoadMinusX_Cpi = new List<CSLoad_Free>(6);
            surfaceWindLoadMinusX_Cpi.Add(new CSLoad_FreeUniform(listOfLoadedMemberTypeDataRoof, ELoadCoordSystem.eLCS, ELoadDir.eLD_Z, pRoofFrontApex, fRoof_X, fRoof_Y, fp_i_roof_Theta_4[(int)ELCMainDirection.eMinusX], -fRoofPitch_rad / (float)Math.PI * 180f, 0, 90, false, false, true, 0));
            surfaceWindLoadMinusX_Cpi.Add(new CSLoad_FreeUniform(listOfLoadedMemberTypeDataRoof,ELoadCoordSystem.eLCS, ELoadDir.eLD_Z, pRoofFrontRight, fRoof_X, fRoof_Y, fp_i_roof_Theta_4[(int)ELCMainDirection.eMinusX], fRoofPitch_rad / (float)Math.PI * 180f, 0, 90, false, false, true, 0));
            surfaceWindLoadMinusX_Cpi.Add(new CSLoad_FreeUniform(listOfLoadedMemberTypeDataWallLeftRight, ELoadCoordSystem.eLCS, ELoadDir.eLD_Z, pWallFrontLeft, fWallLeftOrRight_X, fWallLeftOrRight_Y, fp_i_wall_Theta_4[(int)ELCMainDirection.eMinusX], 90, 0, 90, cColorWindWalls, true, true, true, 0));
            surfaceWindLoadMinusX_Cpi.Add(new CSLoad_FreeUniform(listOfLoadedMemberTypeDataWallLeftRight, ELoadCoordSystem.eLCS, ELoadDir.eLD_Z, pWallBackRight, fWallLeftOrRight_X, fWallLeftOrRight_Y, fp_i_W_wall_Theta_4[(int)ELCMainDirection.eMinusX], 90, 0, 180 + 90, cColorWindWalls, true, true, true, 0));
            surfaceWindLoadMinusX_Cpi.Add(new CSLoad_FreeUniform(listOfLoadedMemberTypeDataWallFront, ELoadCoordSystem.eLCS, ELoadDir.eLD_Z, pWallFrontLeft, fWallFrontOrBack_X, fWallFrontOrBack_Y1, 0.5f * fWallFrontOrBack_X, fWallFrontOrBack_Y2, fWallFrontOrBack_Y1, fp_i_wall_Theta_4[(int)ELCMainDirection.eMinusX], 90, 0, 0, cColorWindWalls, false, false, false, true, 0));
            surfaceWindLoadMinusX_Cpi.Add(new CSLoad_FreeUniform(listOfLoadedMemberTypeDataWallBack, ELoadCoordSystem.eLCS, ELoadDir.eLD_Z, pWallBackRight, fWallFrontOrBack_X, fWallFrontOrBack_Y1, 0.5f * fWallFrontOrBack_X, fWallFrontOrBack_Y2, fWallFrontOrBack_Y1, fp_i_wall_Theta_4[(int)ELCMainDirection.eMinusX], 90, 0, 180, cColorWindWalls, false, false, false, true, 0));

            surfaceWindLoadPlusY_Cpi = new List<CSLoad_Free>(6);
            surfaceWindLoadPlusY_Cpi.Add(new CSLoad_FreeUniform(listOfLoadedMemberTypeDataRoof, ELoadCoordSystem.eLCS, ELoadDir.eLD_Z, pRoofFrontApex, fRoof_X, fRoof_Y, fp_i_roof_Theta_4[(int)ELCMainDirection.ePlusY], -fRoofPitch_rad / (float)Math.PI * 180f, 0, 90, false, false, true, 0));
            surfaceWindLoadPlusY_Cpi.Add(new CSLoad_FreeUniform(listOfLoadedMemberTypeDataRoof,ELoadCoordSystem.eLCS, ELoadDir.eLD_Z, pRoofFrontRight, fRoof_X, fRoof_Y, fp_i_roof_Theta_4[(int)ELCMainDirection.ePlusY], fRoofPitch_rad / (float)Math.PI * 180f, 0, 90, false, false, true, 0));
            surfaceWindLoadPlusY_Cpi.Add(new CSLoad_FreeUniform(listOfLoadedMemberTypeDataWallLeftRight, ELoadCoordSystem.eLCS, ELoadDir.eLD_Z, pWallFrontLeft, fWallLeftOrRight_X, fWallLeftOrRight_Y, fp_i_wall_Theta_4[(int)ELCMainDirection.ePlusY], 90, 0, 90, cColorWindWalls, true, true, true, 0));
            surfaceWindLoadPlusY_Cpi.Add(new CSLoad_FreeUniform(listOfLoadedMemberTypeDataWallLeftRight, ELoadCoordSystem.eLCS, ELoadDir.eLD_Z, pWallBackRight, fWallLeftOrRight_X, fWallLeftOrRight_Y, fp_i_wall_Theta_4[(int)ELCMainDirection.ePlusY], 90, 0, 180 + 90, cColorWindWalls, true, true, true, 0));
            surfaceWindLoadPlusY_Cpi.Add(new CSLoad_FreeUniform(listOfLoadedMemberTypeDataWallFront, ELoadCoordSystem.eLCS, ELoadDir.eLD_Z, pWallFrontLeft, fWallFrontOrBack_X, fWallFrontOrBack_Y1, 0.5f * fWallFrontOrBack_X, fWallFrontOrBack_Y2, fWallFrontOrBack_Y1, fp_i_W_wall_Theta_4[(int)ELCMainDirection.ePlusY], 90, 0, 0, cColorWindWalls, false, false, false, true, 0));
            surfaceWindLoadPlusY_Cpi.Add(new CSLoad_FreeUniform(listOfLoadedMemberTypeDataWallBack, ELoadCoordSystem.eLCS, ELoadDir.eLD_Z, pWallBackRight, fWallFrontOrBack_X, fWallFrontOrBack_Y1, 0.5f * fWallFrontOrBack_X, fWallFrontOrBack_Y2, fWallFrontOrBack_Y1, fp_i_wall_Theta_4[(int)ELCMainDirection.ePlusY], 90, 0, 180, cColorWindWalls, false, false, false, true, 0));

            surfaceWindLoadMinusY_Cpi = new List<CSLoad_Free>(6);
            surfaceWindLoadMinusY_Cpi.Add(new CSLoad_FreeUniform(listOfLoadedMemberTypeDataRoof, ELoadCoordSystem.eLCS, ELoadDir.eLD_Z, pRoofFrontApex, fRoof_X, fRoof_Y, fp_i_roof_Theta_4[(int)ELCMainDirection.eMinusY], -fRoofPitch_rad / (float)Math.PI * 180f, 0, 90, false, false, true, 0));
            surfaceWindLoadMinusY_Cpi.Add(new CSLoad_FreeUniform(listOfLoadedMemberTypeDataRoof,ELoadCoordSystem.eLCS, ELoadDir.eLD_Z, pRoofFrontRight, fRoof_X, fRoof_Y, fp_i_roof_Theta_4[(int)ELCMainDirection.eMinusY], fRoofPitch_rad / (float)Math.PI * 180f, 0, 90, false, false, true, 0));
            surfaceWindLoadMinusY_Cpi.Add(new CSLoad_FreeUniform(listOfLoadedMemberTypeDataWallLeftRight, ELoadCoordSystem.eLCS, ELoadDir.eLD_Z, pWallFrontLeft, fWallLeftOrRight_X, fWallLeftOrRight_Y, fp_i_wall_Theta_4[(int)ELCMainDirection.eMinusY], 90, 0, 90, cColorWindWalls, true, true, true, 0));
            surfaceWindLoadMinusY_Cpi.Add(new CSLoad_FreeUniform(listOfLoadedMemberTypeDataWallLeftRight, ELoadCoordSystem.eLCS, ELoadDir.eLD_Z, pWallBackRight, fWallLeftOrRight_X, fWallLeftOrRight_Y, fp_i_wall_Theta_4[(int)ELCMainDirection.eMinusY], 90, 0, 180 + 90, cColorWindWalls, true, true, true, 0));
            surfaceWindLoadMinusY_Cpi.Add(new CSLoad_FreeUniform(listOfLoadedMemberTypeDataWallFront, ELoadCoordSystem.eLCS, ELoadDir.eLD_Z, pWallFrontLeft, fWallFrontOrBack_X, fWallFrontOrBack_Y1, 0.5f * fWallFrontOrBack_X, fWallFrontOrBack_Y2, fWallFrontOrBack_Y1, fp_i_wall_Theta_4[(int)ELCMainDirection.eMinusY], 90, 0, 0, cColorWindWalls, false, false, false, true, 0));
            surfaceWindLoadMinusY_Cpi.Add(new CSLoad_FreeUniform(listOfLoadedMemberTypeDataWallBack, ELoadCoordSystem.eLCS, ELoadDir.eLD_Z, pWallBackRight, fWallFrontOrBack_X, fWallFrontOrBack_Y1, 0.5f * fWallFrontOrBack_X, fWallFrontOrBack_Y2, fWallFrontOrBack_Y1, fp_i_W_wall_Theta_4[(int)ELCMainDirection.eMinusY], 90, 0, 180, cColorWindWalls, false, false, false, true, 0));
        }

        private void SetSurfaceWindLoads_Cpe(
        List<FreeSurfaceLoadsMemberTypeData> listOfLoadedMemberTypeDataRoof,
        List<FreeSurfaceLoadsMemberTypeData> listOfLoadedMemberTypeDataWallLeftRight,
        List<FreeSurfaceLoadsMemberTypeData> listOfLoadedMemberTypeDataWallFront,
        List<FreeSurfaceLoadsMemberTypeData> listOfLoadedMemberTypeDataWallBack,
        ELSType eLSType,
        CPoint pRoofFrontLeft,
        CPoint pRoofFrontApex,
        CPoint pRoofFrontRight,
        CPoint pRoofBackLeft,
        CPoint pRoofBackApex,
        CPoint pRoofBackRight,
        float fRoof_X,
        float fRoof_Y,
        CPoint pWallFrontLeft,
        CPoint pWallFrontRight,
        CPoint pWallBackRight,
        CPoint pWallBackLeft,
        float fWallLeftOrRight_X,
        float fWallLeftOrRight_Y,
        float fWallFrontOrBack_X,
        float fWallFrontOrBack_Y1,
        float fWallFrontOrBack_Y2,
        CCalcul_1170_2 wind,
        out List<CSLoad_Free> surfaceWindLoadPlusX_Cpemin,
        out List<CSLoad_Free> surfaceWindLoadMinusX_Cpemin,
        out List<CSLoad_Free> surfaceWindLoadPlusY_Cpemin,
        out List<CSLoad_Free> surfaceWindLoadMinusY_Cpemin,
        out List<CSLoad_Free> surfaceWindLoadPlusX_Cpemax,
        out List<CSLoad_Free> surfaceWindLoadMinusX_Cpemax,
        out List<CSLoad_Free> surfaceWindLoadPlusY_Cpemax,
        out List<CSLoad_Free> surfaceWindLoadMinusY_Cpemax)
        {
            // Wind Load
            Color cColorWindPressure = Colors.DeepPink;
            Color cColorWindSagging = Colors.DarkCyan;

            float[,] fp_e_min_U_roof_Theta_4;
            float[,] fp_e_min_D_roof_Theta_4;
            float[,] fp_e_min_R_roof_Theta_4;
            float[,] fp_e_max_U_roof_Theta_4;
            float[,] fp_e_max_D_roof_Theta_4;
            float[,] fp_e_max_R_roof_Theta_4;

            float[] fp_e_W_wall_Theta_4;
            float[] fp_e_L_wall_Theta_4;
            float[,] fp_e_S_wall_Theta_4;

            if (eLSType == ELSType.eLS_ULS)
            {
                // Roof
                fp_e_min_U_roof_Theta_4 = wind.fp_e_min_U_roof_ULS_Theta_4;
                fp_e_min_D_roof_Theta_4 = wind.fp_e_min_D_roof_ULS_Theta_4;
                fp_e_min_R_roof_Theta_4 = wind.fp_e_min_R_roof_ULS_Theta_4;
                fp_e_max_U_roof_Theta_4 = wind.fp_e_max_U_roof_ULS_Theta_4;
                fp_e_max_D_roof_Theta_4 = wind.fp_e_max_D_roof_ULS_Theta_4;
                fp_e_max_R_roof_Theta_4 = wind.fp_e_max_R_roof_ULS_Theta_4;

                // Walls
                fp_e_W_wall_Theta_4 = wind.fp_e_W_wall_ULS_Theta_4;
                fp_e_L_wall_Theta_4 = wind.fp_e_L_wall_ULS_Theta_4;
                fp_e_S_wall_Theta_4 = wind.fp_e_S_wall_ULS_Theta_4;
            }
            else
            {
                // Roof
                fp_e_min_U_roof_Theta_4 = wind.fp_e_min_U_roof_SLS_Theta_4;
                fp_e_min_D_roof_Theta_4 = wind.fp_e_min_D_roof_SLS_Theta_4;
                fp_e_min_R_roof_Theta_4 = wind.fp_e_min_R_roof_SLS_Theta_4;
                fp_e_max_U_roof_Theta_4 = wind.fp_e_max_U_roof_SLS_Theta_4;
                fp_e_max_D_roof_Theta_4 = wind.fp_e_max_D_roof_SLS_Theta_4;
                fp_e_max_R_roof_Theta_4 = wind.fp_e_max_R_roof_SLS_Theta_4;

                // Walls
                fp_e_W_wall_Theta_4 = wind.fp_e_W_wall_SLS_Theta_4;
                fp_e_L_wall_Theta_4 = wind.fp_e_L_wall_SLS_Theta_4;
                fp_e_S_wall_Theta_4 = wind.fp_e_S_wall_SLS_Theta_4;
            }

            // Cpe, min
            SetSurfaceWindLoads_Cpe(
            listOfLoadedMemberTypeDataRoof,
            listOfLoadedMemberTypeDataWallLeftRight,
            listOfLoadedMemberTypeDataWallFront,
            listOfLoadedMemberTypeDataWallBack,
            eLSType,
            0,
            pRoofFrontLeft,
            pRoofFrontApex,
            pRoofFrontRight,
            pRoofBackLeft,
            pRoofBackApex,
            pRoofBackRight,
            fRoof_X,
            fRoof_Y,
            pWallFrontLeft,
            pWallFrontRight,
            pWallBackRight,
            pWallBackLeft,
            fWallLeftOrRight_X,
            fWallLeftOrRight_Y,
            fWallFrontOrBack_X,
            fWallFrontOrBack_Y1,
            fWallFrontOrBack_Y2,
            wind,
            out surfaceWindLoadPlusX_Cpemin,
            out surfaceWindLoadMinusX_Cpemin,
            out surfaceWindLoadPlusY_Cpemin,
            out surfaceWindLoadMinusY_Cpemin
            );

            // Cpe, max
            SetSurfaceWindLoads_Cpe(
            listOfLoadedMemberTypeDataRoof,
            listOfLoadedMemberTypeDataWallLeftRight,
            listOfLoadedMemberTypeDataWallFront,
            listOfLoadedMemberTypeDataWallBack,
            eLSType,
            1,
            pRoofFrontLeft,
            pRoofFrontApex,
            pRoofFrontRight,
            pRoofBackLeft,
            pRoofBackApex,
            pRoofBackRight,
            fRoof_X,
            fRoof_Y,
            pWallFrontLeft,
            pWallFrontRight,
            pWallBackRight,
            pWallBackLeft,
            fWallLeftOrRight_X,
            fWallLeftOrRight_Y,
            fWallFrontOrBack_X,
            fWallFrontOrBack_Y1,
            fWallFrontOrBack_Y2,
            wind,
            out surfaceWindLoadPlusX_Cpemax,
            out surfaceWindLoadMinusX_Cpemax,
            out surfaceWindLoadPlusY_Cpemax,
            out surfaceWindLoadMinusY_Cpemax
            );
        }

        private void SetSurfaceWindLoads_Cpe(
        List<FreeSurfaceLoadsMemberTypeData> listOfLoadedMemberTypeDataRoof,
        List<FreeSurfaceLoadsMemberTypeData> listOfLoadedMemberTypeDataWallLeftRight,
        List<FreeSurfaceLoadsMemberTypeData> listOfLoadedMemberTypeDataWallFront,
        List<FreeSurfaceLoadsMemberTypeData> listOfLoadedMemberTypeDataWallBack,
        ELSType eLSType,
        int iCodeForCpeMinMaxValue,
        CPoint pRoofFrontLeft,
        CPoint pRoofFrontApex,
        CPoint pRoofFrontRight,
        CPoint pRoofBackLeft,
        CPoint pRoofBackApex,
        CPoint pRoofBackRight,
        float fRoof_X,
        float fRoof_Y,
        CPoint pWallFrontLeft,
        CPoint pWallFrontRight,
        CPoint pWallBackRight,
        CPoint pWallBackLeft,
        float fWallLeftOrRight_X,
        float fWallLeftOrRight_Y,
        float fWallFrontOrBack_X,
        float fWallFrontOrBack_Y1,
        float fWallFrontOrBack_Y2,
        CCalcul_1170_2 wind,
        out List<CSLoad_Free> surfaceWindLoadPlusX_Cpe,
        out List<CSLoad_Free> surfaceWindLoadMinusX_Cpe,
        out List<CSLoad_Free> surfaceWindLoadPlusY_Cpe,
        out List<CSLoad_Free> surfaceWindLoadMinusY_Cpe
        )
        {
            // Wind Load
            Color cColorWindPressure = Colors.DeepPink;
            Color cColorWindSagging = Colors.DarkCyan;

            float[,] fp_e_U_roof_Theta_4;
            float[,] fp_e_D_roof_Theta_4;
            float[,] fp_e_R_roof_Theta_4;

            float[] fp_e_W_wall_Theta_4;
            float[] fp_e_L_wall_Theta_4;
            float[,] fp_e_S_wall_Theta_4;

            if (eLSType == ELSType.eLS_ULS)
            {
                // Roof
                if (iCodeForCpeMinMaxValue == 0)
                {
                    fp_e_U_roof_Theta_4 = wind.fp_e_min_U_roof_ULS_Theta_4;
                    fp_e_D_roof_Theta_4 = wind.fp_e_min_D_roof_ULS_Theta_4;
                    fp_e_R_roof_Theta_4 = wind.fp_e_min_R_roof_ULS_Theta_4;
                }
                else
                {
                    fp_e_U_roof_Theta_4 = wind.fp_e_max_U_roof_ULS_Theta_4;
                    fp_e_D_roof_Theta_4 = wind.fp_e_max_D_roof_ULS_Theta_4;
                    fp_e_R_roof_Theta_4 = wind.fp_e_max_R_roof_ULS_Theta_4;
                }

                // Walls
                fp_e_W_wall_Theta_4 = wind.fp_e_W_wall_ULS_Theta_4;
                fp_e_L_wall_Theta_4 = wind.fp_e_L_wall_ULS_Theta_4;
                fp_e_S_wall_Theta_4 = wind.fp_e_S_wall_ULS_Theta_4;
            }
            else
            {
                // Roof
                if (iCodeForCpeMinMaxValue == 0)
                {
                    fp_e_U_roof_Theta_4 = wind.fp_e_min_U_roof_SLS_Theta_4;
                    fp_e_D_roof_Theta_4 = wind.fp_e_min_D_roof_SLS_Theta_4;
                    fp_e_R_roof_Theta_4 = wind.fp_e_min_R_roof_SLS_Theta_4;
                }
                else
                {
                    fp_e_U_roof_Theta_4 = wind.fp_e_max_U_roof_SLS_Theta_4;
                    fp_e_D_roof_Theta_4 = wind.fp_e_max_D_roof_SLS_Theta_4;
                    fp_e_R_roof_Theta_4 = wind.fp_e_max_R_roof_SLS_Theta_4;
                }

                // Walls
                fp_e_W_wall_Theta_4 = wind.fp_e_W_wall_SLS_Theta_4;
                fp_e_L_wall_Theta_4 = wind.fp_e_L_wall_SLS_Theta_4;
                fp_e_S_wall_Theta_4 = wind.fp_e_S_wall_SLS_Theta_4;
            }

            // Cpe
            surfaceWindLoadPlusX_Cpe = new List<CSLoad_Free>(6);
            surfaceWindLoadPlusX_Cpe.Add(new CSLoad_FreeUniformGroup(listOfLoadedMemberTypeDataRoof, ELoadCoordSystem.eLCS, ELoadDir.eLD_Z, pRoofFrontLeft, wind.fC_pe_U_roof_dimensions, fRoof_Y, fRoof_X, ELCMainDirection.ePlusX, fp_e_U_roof_Theta_4, 0, -fRoofPitch_rad / (float)Math.PI * 180f, 0, false, false, true, 0));
            surfaceWindLoadPlusX_Cpe.Add(new CSLoad_FreeUniformGroup(listOfLoadedMemberTypeDataRoof, ELoadCoordSystem.eLCS, ELoadDir.eLD_Z, pRoofFrontApex, wind.fC_pe_D_roof_dimensions, fRoof_Y, fRoof_X, ELCMainDirection.ePlusX, fp_e_D_roof_Theta_4, 0, fRoofPitch_rad / (float)Math.PI * 180f, 0, false, false, true, 0, wind.iFirst_D_SegmentColorID));
            surfaceWindLoadPlusX_Cpe.Add(new CSLoad_FreeUniformGroup(listOfLoadedMemberTypeDataWallFront, ELoadCoordSystem.eLCS, ELoadDir.eLD_Z, pWallFrontLeft, wind.fC_pe_S_wall_dimensions, fWallFrontOrBack_X, fWallFrontOrBack_Y1, fWallFrontOrBack_Y2, ELCMainDirection.ePlusX, fp_e_S_wall_Theta_4, 90, 0, 0, false, false, true, 0));
            surfaceWindLoadPlusX_Cpe.Add(new CSLoad_FreeUniformGroup(listOfLoadedMemberTypeDataWallBack, ELoadCoordSystem.eLCS, ELoadDir.eLD_Z, pWallBackLeft, wind.fC_pe_S_wall_dimensions, fWallFrontOrBack_X, fWallFrontOrBack_Y1, fWallFrontOrBack_Y2, ELCMainDirection.ePlusX, fp_e_S_wall_Theta_4, 90, 0, 0, true, true, true, 0));
            surfaceWindLoadPlusX_Cpe.Add(new CSLoad_FreeUniform(listOfLoadedMemberTypeDataWallLeftRight, ELoadCoordSystem.eLCS, ELoadDir.eLD_Z, pWallFrontLeft, fWallLeftOrRight_X, fWallLeftOrRight_Y, fp_e_W_wall_Theta_4[(int)ELCMainDirection.ePlusX], 90, 0, 90, cColorWindPressure, false, false, true, 0));
            surfaceWindLoadPlusX_Cpe.Add(new CSLoad_FreeUniform(listOfLoadedMemberTypeDataWallLeftRight, ELoadCoordSystem.eLCS, ELoadDir.eLD_Z, pWallFrontRight, fWallLeftOrRight_X, fWallLeftOrRight_Y, fp_e_L_wall_Theta_4[(int)ELCMainDirection.ePlusX], 90, 0, 90, cColorWindSagging, false, false, true, 0));

            surfaceWindLoadMinusX_Cpe = new List<CSLoad_Free>(6);
            surfaceWindLoadMinusX_Cpe.Add(new CSLoad_FreeUniformGroup(listOfLoadedMemberTypeDataRoof, ELoadCoordSystem.eLCS, ELoadDir.eLD_Z, pRoofBackApex, wind.fC_pe_D_roof_dimensions, fRoof_Y, fRoof_X, ELCMainDirection.eMinusX, fp_e_D_roof_Theta_4, 0, fRoofPitch_rad / (float)Math.PI * 180f, 180, false, false, true, 0, wind.iFirst_D_SegmentColorID));
            surfaceWindLoadMinusX_Cpe.Add(new CSLoad_FreeUniformGroup(listOfLoadedMemberTypeDataRoof, ELoadCoordSystem.eLCS, ELoadDir.eLD_Z, pRoofBackRight, wind.fC_pe_U_roof_dimensions, fRoof_Y, fRoof_X, ELCMainDirection.eMinusX, fp_e_U_roof_Theta_4, 0, -fRoofPitch_rad / (float)Math.PI * 180f, 180, false, false, true, 0));
            surfaceWindLoadMinusX_Cpe.Add(new CSLoad_FreeUniformGroup(listOfLoadedMemberTypeDataWallFront, ELoadCoordSystem.eLCS, ELoadDir.eLD_Z, pWallFrontRight, wind.fC_pe_S_wall_dimensions, fWallFrontOrBack_X, fWallFrontOrBack_Y1, fWallFrontOrBack_Y2, ELCMainDirection.eMinusX, fp_e_S_wall_Theta_4, 90, 0, 180, true, true, true, 0));
            surfaceWindLoadMinusX_Cpe.Add(new CSLoad_FreeUniformGroup(listOfLoadedMemberTypeDataWallBack, ELoadCoordSystem.eLCS, ELoadDir.eLD_Z, pWallBackRight, wind.fC_pe_S_wall_dimensions, fWallFrontOrBack_X, fWallFrontOrBack_Y1, fWallFrontOrBack_Y2, ELCMainDirection.eMinusX, fp_e_S_wall_Theta_4, 90, 0, 180, false, true, true, 0));
            surfaceWindLoadMinusX_Cpe.Add(new CSLoad_FreeUniform(listOfLoadedMemberTypeDataWallLeftRight, ELoadCoordSystem.eLCS, ELoadDir.eLD_Z, pWallFrontLeft, fWallLeftOrRight_X, fWallLeftOrRight_Y, fp_e_L_wall_Theta_4[(int)ELCMainDirection.eMinusX], 90, 0, 90, cColorWindSagging, true, true, true, 0));
            surfaceWindLoadMinusX_Cpe.Add(new CSLoad_FreeUniform(listOfLoadedMemberTypeDataWallLeftRight, ELoadCoordSystem.eLCS, ELoadDir.eLD_Z, pWallFrontRight, fWallLeftOrRight_X, fWallLeftOrRight_Y, fp_e_W_wall_Theta_4[(int)ELCMainDirection.eMinusX], 90, 0, 90, cColorWindPressure, true, false, true, 0));

            surfaceWindLoadPlusY_Cpe = new List<CSLoad_Free>(6);
            surfaceWindLoadPlusY_Cpe.Add(new CSLoad_FreeUniformGroup(listOfLoadedMemberTypeDataRoof, ELoadCoordSystem.eLCS, ELoadDir.eLD_Z, pRoofFrontApex, wind.fC_pe_R_roof_dimensions, fRoof_X, fRoof_Y, ELCMainDirection.ePlusY, fp_e_R_roof_Theta_4, -fRoofPitch_rad / (float)Math.PI * 180f, 0, 90, false, false, true, 0));
            surfaceWindLoadPlusY_Cpe.Add(new CSLoad_FreeUniformGroup(listOfLoadedMemberTypeDataRoof, ELoadCoordSystem.eLCS, ELoadDir.eLD_Z, pRoofFrontRight, wind.fC_pe_R_roof_dimensions, fRoof_X, fRoof_Y, ELCMainDirection.ePlusY, fp_e_R_roof_Theta_4, fRoofPitch_rad / (float)Math.PI * 180f, 0, 90, false, false, true, 0));
            surfaceWindLoadPlusY_Cpe.Add(new CSLoad_FreeUniformGroup(listOfLoadedMemberTypeDataWallLeftRight, ELoadCoordSystem.eLCS, ELoadDir.eLD_Z, pWallFrontLeft, wind.fC_pe_S_wall_dimensions, fWallLeftOrRight_X, fWallLeftOrRight_Y, ELCMainDirection.ePlusY, fp_e_S_wall_Theta_4, 90, 0, 90, true, true, true, 0));
            surfaceWindLoadPlusY_Cpe.Add(new CSLoad_FreeUniformGroup(listOfLoadedMemberTypeDataWallLeftRight, ELoadCoordSystem.eLCS, ELoadDir.eLD_Z, pWallFrontRight, wind.fC_pe_S_wall_dimensions, fWallLeftOrRight_X, fWallLeftOrRight_Y, ELCMainDirection.ePlusY, fp_e_S_wall_Theta_4, 90, 0, 90, false, false, true, 0));
            surfaceWindLoadPlusY_Cpe.Add(new CSLoad_FreeUniform(listOfLoadedMemberTypeDataWallFront, ELoadCoordSystem.eLCS, ELoadDir.eLD_Z, pWallFrontLeft, fWallFrontOrBack_X, fWallFrontOrBack_Y1, fWallFrontOrBack_Y2, fp_e_W_wall_Theta_4[(int)ELCMainDirection.ePlusY], 90, 0, 0, cColorWindPressure, true, true, false, true, 0));
            surfaceWindLoadPlusY_Cpe.Add(new CSLoad_FreeUniform(listOfLoadedMemberTypeDataWallBack, ELoadCoordSystem.eLCS, ELoadDir.eLD_Z, pWallBackRight, fWallFrontOrBack_X, fWallFrontOrBack_Y1, fWallFrontOrBack_Y2, fp_e_L_wall_Theta_4[(int)ELCMainDirection.ePlusY], 90, 0, 180, cColorWindSagging, false, false, false, true, 0));

            surfaceWindLoadMinusY_Cpe = new List<CSLoad_Free>(6);
            surfaceWindLoadMinusY_Cpe.Add(new CSLoad_FreeUniformGroup(listOfLoadedMemberTypeDataRoof, ELoadCoordSystem.eLCS, ELoadDir.eLD_Z, pRoofBackLeft, wind.fC_pe_R_roof_dimensions, fRoof_X, fRoof_Y, ELCMainDirection.eMinusY, fp_e_R_roof_Theta_4, fRoofPitch_rad / (float)Math.PI * 180f, 0, 180 + 90, false, false, true, 0));
            surfaceWindLoadMinusY_Cpe.Add(new CSLoad_FreeUniformGroup(listOfLoadedMemberTypeDataRoof, ELoadCoordSystem.eLCS, ELoadDir.eLD_Z, pRoofBackApex, wind.fC_pe_R_roof_dimensions, fRoof_X, fRoof_Y, ELCMainDirection.eMinusY, fp_e_R_roof_Theta_4, -fRoofPitch_rad / (float)Math.PI * 180f, 0, 180 + 90, false, false, true, 0));
            surfaceWindLoadMinusY_Cpe.Add(new CSLoad_FreeUniformGroup(listOfLoadedMemberTypeDataWallLeftRight, ELoadCoordSystem.eLCS, ELoadDir.eLD_Z, pWallBackLeft, wind.fC_pe_S_wall_dimensions, fWallLeftOrRight_X, fWallLeftOrRight_Y, ELCMainDirection.eMinusY, fp_e_S_wall_Theta_4, 90, 0, 180 + 90, false, false, true, 0));
            surfaceWindLoadMinusY_Cpe.Add(new CSLoad_FreeUniformGroup(listOfLoadedMemberTypeDataWallLeftRight, ELoadCoordSystem.eLCS, ELoadDir.eLD_Z, pWallBackRight, wind.fC_pe_S_wall_dimensions, fWallLeftOrRight_X, fWallLeftOrRight_Y, ELCMainDirection.eMinusY, fp_e_S_wall_Theta_4, 90, 0, 180 + 90, true, true, true, 0));
            surfaceWindLoadMinusY_Cpe.Add(new CSLoad_FreeUniform(listOfLoadedMemberTypeDataWallFront, ELoadCoordSystem.eLCS, ELoadDir.eLD_Z, pWallFrontLeft, fWallFrontOrBack_X, fWallFrontOrBack_Y1, fWallFrontOrBack_Y2, fp_e_L_wall_Theta_4[(int)ELCMainDirection.eMinusY], 90, 0, 0, cColorWindSagging, false, false, false, true, 0));
            surfaceWindLoadMinusY_Cpe.Add(new CSLoad_FreeUniform(listOfLoadedMemberTypeDataWallBack, ELoadCoordSystem.eLCS, ELoadDir.eLD_Z, pWallBackRight, fWallFrontOrBack_X, fWallFrontOrBack_Y1, fWallFrontOrBack_Y2, fp_e_W_wall_Theta_4[(int)ELCMainDirection.eMinusY], 90, 0, 180, cColorWindPressure, true, true, false, true, 0));
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
