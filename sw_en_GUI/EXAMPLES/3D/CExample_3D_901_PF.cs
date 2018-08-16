using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using BaseClasses;
using BaseClasses.GraphObj;
using MATH;
using DATABASE;
using MATERIAL;
using CRSC;
using Combinatorics.Collections;
using M_EC1.AS_NZS;

namespace sw_en_GUI.EXAMPLES._3D
{
    public class CExample_3D_901_PF : CExample
    {
        public float fH1_frame;
        float fH2_frame;
        public float fW_frame;
        public float fL1_frame;
        public float fRoofPitch_rad;
        int iFrameNo;
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

        public CExample_3D_901_PF
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

            // TODO Nastavit podla databazy models, tabulka KitsetGableRoofEnclosed alebo KitsetGableRoofEnclosedCrscID
            m_arrCrSc[0] = new CCrSc_3_63020_BOX(0.63f, 0.2f, 0.00195f, 0.00195f, Colors.Chocolate);  // Main Column
            m_arrCrSc[1] = new CCrSc_3_63020_BOX(0.63f, 0.2f, 0.00195f, 0.00195f, Colors.Green);      // Rafter
            m_arrCrSc[2] = new CCrSc_3_50020_C(0.5f, 0.2f, 0.00195f, Colors.DarkCyan);                // Eaves Purlin
            m_arrCrSc[3] = new CCrSc_3_270XX_C(0.27f, 0.07f, 0.00115f, Colors.Orange);                // Girt - Wall
            m_arrCrSc[4] = new CCrSc_3_270XX_C(0.27f, 0.07f, 0.00095f, Colors.SlateBlue);             // Purlin
            m_arrCrSc[5] = new CCrSc_3_10075_BOX(0.3f, 0.1f, 0.0075f, Colors.BlueViolet);             // Front Column
            m_arrCrSc[6] = new CCrSc_3_10075_BOX(0.3f, 0.1f, 0.0075f, Colors.BlueViolet);             // Back Column
            m_arrCrSc[7] = new CCrSc_3_270XX_C(0.27f, 0.07f, 0.00115f, Colors.Brown);                 // Front Girt
            m_arrCrSc[8] = new CCrSc_3_270XX_C(0.27f, 0.07f, 0.00095f, Colors.YellowGreen);           // Back Girt

            // Member Eccentricities
            // Zadane hodnoty predpokladaju ze prierez je symetricky, je potrebne zobecnit
            CMemberEccentricity eccentricityPurlin = new CMemberEccentricity(0, (float)(0.5 * m_arrCrSc[1].h - 0.5 * m_arrCrSc[4].h));
            CMemberEccentricity eccentricityGirtLeft_X0 = new CMemberEccentricity(0, (float)(-(0.5 * m_arrCrSc[0].h - 0.5 * m_arrCrSc[3].h)));
            CMemberEccentricity eccentricityGirtRight_XB = new CMemberEccentricity(0, (float)(0.5 * m_arrCrSc[0].h - 0.5 * m_arrCrSc[3].h));

            CMemberEccentricity eccentricityEavePurlin = new CMemberEccentricity(-(float)(0.5 * m_arrCrSc[0].h + m_arrCrSc[2].y_min), 0);

            CMemberEccentricity eccentricityGirtFront_Y0 = new CMemberEccentricity(0, 0);
            CMemberEccentricity eccentricityGirtBack_YL = new CMemberEccentricity(0, 0);

            // Limit pre poziciu horneho nosnika, mala by to byt polovica suctu vysky edge (eave) purlin h a sirky nosnika b (neberie sa h pretoze nosnik je otoceny o 90 stupnov)
            fUpperGirtLimit = (float)(m_arrCrSc[2].h + m_arrCrSc[3].b);

            // Limit pre poziciu horneho nosnika (front / back girt) na prednej alebo zadnej stene budovy
            // Nosnik alebo pripoj nosnika nesmie zasahovat do prievlaku (rafter)

            fz_UpperLimitForFrontGirts = (float)((0.5 * m_arrCrSc[1].h) / Math.Cos(fRoofPitch_rad) + 0.5f * m_arrCrSc[7].b);
            fz_UpperLimitForBackGirts = (float)((0.5 * m_arrCrSc[1].h) / Math.Cos(fRoofPitch_rad) + 0.5f * m_arrCrSc[8].b);

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

            GetJointAllignments((float)m_arrCrSc[0].h, (float)m_arrCrSc[1].h, out fallignment_column, out fallignment_knee_rafter, out fallignment_apex_rafter);

            float fMainColumnStart = 0.0f;
            float fMainColumnEnd = -fallignment_column - fCutOffOneSide;
            float fRafterStart = fallignment_knee_rafter - fCutOffOneSide;
            float fRafterEnd = -fallignment_apex_rafter - fCutOffOneSide;              // Calculate according to h of rafter and roof pitch
            float fEavesPurlinStart = -0.5f * (float)m_arrCrSc[1].b - fCutOffOneSide;  // Just in case that cross-section of rafter is symmetric about z-z
            float fEavesPurlinEnd = -0.5f * (float)m_arrCrSc[1].b - fCutOffOneSide;    // Just in case that cross-section of rafter is symmetric about z-z
            float fGirtStart = -0.5f * (float)m_arrCrSc[0].b - fCutOffOneSide;         // Just in case that cross-section of main column is symmetric about z-z
            float fGirtEnd = -0.5f * (float)m_arrCrSc[0].b - fCutOffOneSide;           // Just in case that cross-section of main column is symmetric about z-z
            float fPurlinStart = -0.5f * (float)m_arrCrSc[1].b - fCutOffOneSide;       // Just in case that cross-section of rafter is symmetric about z-z
            float fPurlinEnd = -0.5f * (float)m_arrCrSc[1].b - fCutOffOneSide;         // Just in case that cross-section of rafter is symmetric about z-z
            float fFrontColumnStart = 0f;
            float fFrontColumnEnd = -0.5f * (float)m_arrCrSc[1].h - fCutOffOneSide;    // TODO - Calculate according to h of rafter and roof pitch
            float fBackColumnStart = 0f;
            float fBackColumnEnd = -0.5f * (float)m_arrCrSc[1].h - fCutOffOneSide;     // TODO - Calculate according to h of rafter and roof pitch
            float fFrontGirtStart = -0.5f * (float)m_arrCrSc[5].b - fCutOffOneSide;    // Just in case that cross-section of column is symmetric about z-z
            float fFrontGirtEnd = -0.5f * (float)m_arrCrSc[5].b - fCutOffOneSide;      // Just in case that cross-section of column is symmetric about z-z
            float fBackGirtStart = -0.5f * (float)m_arrCrSc[6].b - fCutOffOneSide;     // Just in case that cross-section of column is symmetric about z-z
            float fBackGirtEnd = -0.5f * (float)m_arrCrSc[6].b - fCutOffOneSide;       // Just in case that cross-section of column is symmetric about z-z
            float fFrontGirtStart_MC = -0.5f * (float)m_arrCrSc[0].h - fCutOffOneSide; // Connection to the main frame column (column symmetrical about y-y)
            float fFrontGirtEnd_MC = -0.5f * (float)m_arrCrSc[0].h - fCutOffOneSide;   // Connection to the main frame column (column symmetrical about y-y)
            float fBackGirtStart_MC = -0.5f * (float)m_arrCrSc[0].h - fCutOffOneSide;  // Connection to the main frame column (column symmetrical about y-y)
            float fBackGirtEnd_MC = -0.5f * (float)m_arrCrSc[0].h - fCutOffOneSide;    // Connection to the main frame column (column symmetrical about y-y)

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
                m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * (iFrameNodesNo - 1) + 0] = new CMember(i * (iFrameNodesNo - 1) + 1, m_arrNodes[i * iFrameNodesNo + 0], m_arrNodes[i * iFrameNodesNo + 1], m_arrCrSc[0], EMemberType_FormSteel.eMC, null, null, fMainColumnStart, fMainColumnEnd, 0f, 0);
                // Rafters
                m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * (iFrameNodesNo - 1) + 1] = new CMember(i * (iFrameNodesNo - 1) + 2, m_arrNodes[i * iFrameNodesNo + 1], m_arrNodes[i * iFrameNodesNo + 2], m_arrCrSc[1], EMemberType_FormSteel.eMR, null, null, fRafterStart, fRafterEnd, 0f, 0);
                m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * (iFrameNodesNo - 1) + 2] = new CMember(i * (iFrameNodesNo - 1) + 3, m_arrNodes[i * iFrameNodesNo + 2], m_arrNodes[i * iFrameNodesNo + 3], m_arrCrSc[1], EMemberType_FormSteel.eMR, null, null, fRafterEnd, fRafterStart, 0f, 0);
                // Main Column
                m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * (iFrameNodesNo - 1) + 3] = new CMember(i * (iFrameNodesNo - 1) + 4, m_arrNodes[i * iFrameNodesNo + 3], m_arrNodes[i * iFrameNodesNo + 4], m_arrCrSc[0], EMemberType_FormSteel.eMC, null, null, fMainColumnEnd, fMainColumnStart, 0f, 0);

                // Eaves Purlins
                if (i < (iFrameNo - 1))
                {
                    m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * (iFrameNodesNo - 1) + 4] = new CMember((i * iEavesPurlinNoInOneFrame) + i * (iFrameNodesNo - 1) + 5, m_arrNodes[i * iFrameNodesNo + 1], m_arrNodes[(i + 1) * iFrameNodesNo + 1], m_arrCrSc[2], EMemberType_FormSteel.eEP, eccentricityEavePurlin, eccentricityEavePurlin, fEavesPurlinStart, fEavesPurlinEnd, (float)Math.PI, 0);
                    m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * (iFrameNodesNo - 1) + 5] = new CMember((i * iEavesPurlinNoInOneFrame) + i * (iFrameNodesNo - 1) + 6, m_arrNodes[i * iFrameNodesNo + 3], m_arrNodes[(i + 1) * iFrameNodesNo + 3], m_arrCrSc[2], EMemberType_FormSteel.eEP, eccentricityEavePurlin, eccentricityEavePurlin, fEavesPurlinStart, fEavesPurlinEnd, 0f, 0);
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
                        m_arrMembers[i_temp_numberofMembers + i * iGirtNoInOneFrame + j] = new CMember(i_temp_numberofMembers + i * iGirtNoInOneFrame + j + 1, m_arrNodes[i_temp_numberofNodes + i * iGirtNoInOneFrame + j], m_arrNodes[i_temp_numberofNodes + (i + 1) * iGirtNoInOneFrame + j], m_arrCrSc[3], EMemberType_FormSteel.eG, eccentricityGirtLeft_X0, eccentricityGirtLeft_X0, fGirtStart, fGirtEnd, fGirtsRotation, 0);
                        RotateFrontOrBackFrameNodeAboutZ(m_arrNodes[i_temp_numberofMembers + i * iGirtNoInOneFrame + j]);
                    }

                    for (int j = 0; j < iOneColumnGirtNo; j++)
                    {
                        m_arrMembers[i_temp_numberofMembers + i * iGirtNoInOneFrame + iOneColumnGirtNo + j] = new CMember(i_temp_numberofMembers + i * iGirtNoInOneFrame + iOneColumnGirtNo + j + 1, m_arrNodes[i_temp_numberofNodes + i * iGirtNoInOneFrame + iOneColumnGirtNo + j], m_arrNodes[i_temp_numberofNodes + (i + 1) * iGirtNoInOneFrame + iOneColumnGirtNo + j], m_arrCrSc[3], EMemberType_FormSteel.eG, eccentricityGirtRight_XB, eccentricityGirtRight_XB, fGirtStart, fGirtEnd, fGirtsRotation, 0);
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
                for (int i = 0; i < (iFrameNo - 1); i++)
                {
                    for (int j = 0; j < iOneRafterPurlinNo; j++)
                    {
                        CMemberEccentricity temp = new CMemberEccentricity();
                        temp.MFz_local = - eccentricityPurlin.MFz_local; // We need to change sign of eccentrictiy for purlins on the left side because z axis of these purlins is oriented downwards
 
                        m_arrMembers[i_temp_numberofMembers + i * iPurlinNoInOneFrame + j] = new CMember(i_temp_numberofMembers + i * iPurlinNoInOneFrame + j + 1, m_arrNodes[i_temp_numberofNodes + i * iPurlinNoInOneFrame + j], m_arrNodes[i_temp_numberofNodes + (i + 1) * iPurlinNoInOneFrame + j], m_arrCrSc[4], EMemberType_FormSteel.eP, temp/*eccentricityPurlin*/, temp /*eccentricityPurlin*/, fPurlinStart, fPurlinEnd, -(fRoofPitch_rad + (float)Math.PI), 0);
                    }

                    for (int j = 0; j < iOneRafterPurlinNo; j++)
                    {
                        m_arrMembers[i_temp_numberofMembers + i * iPurlinNoInOneFrame + iOneRafterPurlinNo + j] = new CMember(i_temp_numberofMembers + i * iPurlinNoInOneFrame + iOneRafterPurlinNo + j + 1, m_arrNodes[i_temp_numberofNodes + i * iPurlinNoInOneFrame + iOneRafterPurlinNo + j], m_arrNodes[i_temp_numberofNodes + (i + 1) * iPurlinNoInOneFrame + iOneRafterPurlinNo + j], m_arrCrSc[4], EMemberType_FormSteel.eP, eccentricityPurlin, eccentricityPurlin, fPurlinStart, fPurlinEnd, fRoofPitch_rad, 0);
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
                AddColumnsMembers(i_temp_numberofNodes, i_temp_numberofMembers, iOneRafterBackColumnNo, iFrontColumnNoInOneFrame, fFrontColumnStart, fFrontColumnEnd, m_arrCrSc[5], fColumnsRotation);
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
                AddColumnsMembers(i_temp_numberofNodes, i_temp_numberofMembers, iOneRafterBackColumnNo, iBackColumnNoInOneFrame, fBackColumnStart, fBackColumnEnd, m_arrCrSc[6], fColumnsRotation);
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
                AddFrontOrBackGirtsMembers(iFrameNodesNo, iOneRafterFrontColumnNo, iArrNumberOfNodesPerFrontColumn, i_temp_numberofNodes, i_temp_numberofMembers, iFrontIntermediateColumnNodesForGirtsOneRafterNo, iFrontIntermediateColumnNodesForGirtsOneFrameNo, 0, fDist_Girt, eccentricityGirtFront_Y0, fFrontGirtStart_MC, fFrontGirtStart, fFrontGirtEnd, m_arrCrSc[7], fColumnsRotation);
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
                AddFrontOrBackGirtsMembers(iFrameNodesNo, iOneRafterBackColumnNo, iArrNumberOfNodesPerBackColumn, i_temp_numberofNodes, i_temp_numberofMembers, iBackIntermediateColumnNodesForGirtsOneRafterNo, iBackIntermediateColumnNodesForGirtsOneFrameNo, iGirtNoInOneFrame * (iFrameNo - 1), fDist_Girt, eccentricityGirtBack_YL, fBackGirtStart_MC, fBackGirtStart, fBackGirtEnd, m_arrCrSc[8], fColumnsRotation);
            }

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
                    m_arrConnectionJoints.Add(new CConnectionJoint_B001(m_arrNodes[i * 5 + 1], m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * 4], m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * 4 + 1], fRoofPitch_rad, 2 * (float)m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * 4].CrScStart.h, 2 * (float)m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * 4 + 1].CrScStart.h, ft_knee_joint_plate, ft_rafter_joint_plate, i == 0 ? fFrontFrameRakeAngle_temp_deg : fBackFrameRakeAngle_temp_deg, true));
                else //if(i< (iFrameNo - 1) // Intermediate frame
                    m_arrConnectionJoints.Add(new CConnectionJoint_B001(m_arrNodes[i * 5 + 1], m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * 4], m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * 4 + 1], fRoofPitch_rad, 2 * (float)m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * 4].CrScStart.h, 2 * (float)m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * 4 + 1].CrScStart.h, ft_knee_joint_plate, ft_rafter_joint_plate, i == 0 ? fFrontFrameRakeAngle_temp_deg : fBackFrameRakeAngle_temp_deg, true));
            }

            // Knee Joints 2
            for (int i = 0; i < iFrameNo; i++)
            {
                if (i == 0 || i == (iFrameNo - 1)) // Front or Last Frame
                    m_arrConnectionJoints.Add(new CConnectionJoint_B001(m_arrNodes[i * 5 + 3], m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * 4 + 3], m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * 4 + 2], fRoofPitch_rad, 2 * (float)m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * 4 + 3].CrScStart.h, 2 * (float)m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * 4 + 2].CrScStart.h, ft_knee_joint_plate, ft_rafter_joint_plate, i == 0 ? fFrontFrameRakeAngle_temp_deg : fBackFrameRakeAngle_temp_deg, true));
                else //if(i< (iFrameNo - 1) // Intermediate frame
                    m_arrConnectionJoints.Add(new CConnectionJoint_B001(m_arrNodes[i * 5 + 3], m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * 4 + 3], m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * 4 + 2], fRoofPitch_rad, 2 * (float)m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * 4 + 3].CrScStart.h, 2 * (float)m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * 4 + 2].CrScStart.h, ft_knee_joint_plate, ft_rafter_joint_plate, i == 0 ? fFrontFrameRakeAngle_temp_deg : fBackFrameRakeAngle_temp_deg, true));
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
                    m_arrConnectionJoints.Add(new CConnectionJoint_T001("LH", current_member.NodeStart, m_arrMembers[1], current_member, ft_knee_joint_plate, EPlateNumberAndPositionInJoint.eTwoPlates, true, true));
                    m_arrConnectionJoints.Add(new CConnectionJoint_T001("LH", current_member.NodeEnd, m_arrMembers[1], current_member, ft_knee_joint_plate, EPlateNumberAndPositionInJoint.eTwoPlates, true, true));
                }
            }

            // Front Columns Foundation Joints
            if (bGenerateFrontColumns)
            {
                for (int i = 0; i < iFrontColumnNoInOneFrame; i++)
                {
                    CMember current_member = m_arrMembers[iMainColumnNo + iRafterNo + iEavesPurlinNo + (iFrameNo - 1) * iGirtNoInOneFrame + (iFrameNo - 1) * iPurlinNoInOneFrame + i];
                    m_arrConnectionJoints.Add(new CConnectionJoint_TB01(current_member.NodeStart, current_member, true));
                }
            }

            // Back Columns Foundation Joints
            if (bGenerateBackColumns)
            {
                for (int i = 0; i < iBackColumnNoInOneFrame; i++)
                {
                    CMember current_member = m_arrMembers[iMainColumnNo + iRafterNo + iEavesPurlinNo + (iFrameNo - 1) * iGirtNoInOneFrame + (iFrameNo - 1) * iPurlinNoInOneFrame + iFrontColumnNoInOneFrame + i];
                    m_arrConnectionJoints.Add(new CConnectionJoint_TB01(current_member.NodeStart, current_member, true));
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

            // Member Releases / hinges - fill values

            // Set values
            bool?[] bMembRelase1 = { false, false, false, false, true, false };

            // Create Release / Hinge Objects
            //m_arrMembers[02].CnRelease1 = new CNRelease(6, m_arrMembers[02].NodeStart, bMembRelase1, 0);

            // Loading

            // Loads
            m_arrNLoads = new CNLoad[3];
            m_arrNLoads[0] = new CNLoadAll(m_arrNodes[1], 0, 0, -4f, 0, 0, 0, true, 0);
            m_arrNLoads[1] = new CNLoadAll(m_arrNodes[2], 0, 0, -4f, 0, 0, 0, true, 0);
            m_arrNLoads[2] = new CNLoadAll(m_arrNodes[3], 0, 0, -4f, 0, 0, 0, true, 0);

            float fValueLoadRafterDead1 = -0.2f;
            float fValueLoadRafterDead2 = -0.1f;
            float fValueLoadRafterImposed = -0.1f;
            float fValueLoadRafterSnow1 = -0.8f;
            float fValueLoadRafterSnow2 = -0.4f;

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

            GenerateLoadOnRafters(fValueLoadRafterDead1, fValueLoadRafterDead1, ref memberLoadDead1Rafters);
            GenerateLoadOnRafters(fValueLoadRafterDead2, fValueLoadRafterDead2, ref memberLoadDead2Rafters);
            GenerateLoadOnRafters(fValueLoadRafterImposed, fValueLoadRafterImposed, ref memberLoadImposedRafters);
            GenerateLoadOnRafters(fValueLoadRafterSnow1, fValueLoadRafterSnow1, ref memberMaxLoadSnowAllRafters);
            GenerateLoadOnRafters(fValueLoadRafterSnow1, fValueLoadRafterSnow2, ref memberMaxLoadSnowLeftRafters);
            GenerateLoadOnRafters(fValueLoadRafterSnow2, fValueLoadRafterSnow1, ref memberMaxLoadSnowRightRafters);

            // Wind

            // Cpe,min
            // + X
            GenerateLoadOnFrames(fValueLoadColumnWind1PlusX_CpeMin, fValueLoadColumnWind2PlusX_CpeMin, fValueLoadRafterWind1PlusX_CpeMin, fValueLoadRafterWind2PlusX_CpeMin, ref memberLoadWindFramesPlusX_CpeMin);

            // - X
            GenerateLoadOnFrames(fValueLoadColumnWind1MinusX_CpeMin, fValueLoadColumnWind2MinusX_CpeMin, fValueLoadRafterWind1MinusX_CpeMin, fValueLoadRafterWind2MinusX_CpeMin, ref memberLoadWindFramesMinusX_CpeMin);

            // + Y
            GenerateLoadOnFrames(fValueLoadColumnnWindPlusY_CpeMin, fValueLoadColumnnWindPlusY_CpeMin, fValueLoadRafterWindPlusY_CpeMin, fValueLoadRafterWindPlusY_CpeMin, ref memberLoadWindFramesPlusY_CpeMin);

            // - Y
            GenerateLoadOnFrames(fValueLoadColumnnWindMinusY_CpeMin, fValueLoadColumnnWindMinusY_CpeMin, fValueLoadRafterWindMinusY_CpeMin, fValueLoadRafterWindMinusY_CpeMin, ref memberLoadWindFramesMinusY_CpeMin);

            // Cpe,max
            // + X
            GenerateLoadOnFrames(fValueLoadColumnWind1PlusX_CpeMax, fValueLoadColumnWind2PlusX_CpeMax, fValueLoadRafterWind1PlusX_CpeMax, fValueLoadRafterWind2PlusX_CpeMax, ref memberLoadWindFramesPlusX_CpeMax);

            // - X
            GenerateLoadOnFrames(fValueLoadColumnWind1MinusX_CpeMax, fValueLoadColumnWind2MinusX_CpeMax, fValueLoadRafterWind1MinusX_CpeMax, fValueLoadRafterWind2MinusX_CpeMax, ref memberLoadWindFramesMinusX_CpeMax);

            // + Y
            GenerateLoadOnFrames(fValueLoadColumnnWindPlusY_CpeMax, fValueLoadColumnnWindPlusY_CpeMax, fValueLoadRafterWindPlusY_CpeMax, fValueLoadRafterWindPlusY_CpeMax, ref memberLoadWindFramesPlusY_CpeMax);

            // - Y
            GenerateLoadOnFrames(fValueLoadColumnnWindMinusY_CpeMax, fValueLoadColumnnWindMinusY_CpeMax, fValueLoadRafterWindMinusY_CpeMax, fValueLoadRafterWindMinusY_CpeMax, ref memberLoadWindFramesMinusY_CpeMax);

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

            // Permanent load
            List<CSLoad_Free> surfaceDeadLoad = new List<CSLoad_Free>(5);
            surfaceDeadLoad.Add(new CSLoad_FreeUniform(ELoadCoordSystem.eGCS, ELoadDir.eLD_Z, pRoofFrontApex, fRoof_X, fRoof_Y, generalLoad.fDeadLoadTotal_Roof, -fRoofPitch_rad / (float)Math.PI * 180f, 0, 90, true, true, true, 0));
            surfaceDeadLoad.Add(new CSLoad_FreeUniform(ELoadCoordSystem.eGCS, ELoadDir.eLD_Z, pRoofFrontRight, fRoof_X, fRoof_Y, generalLoad.fDeadLoadTotal_Roof, fRoofPitch_rad / (float)Math.PI * 180f, 0, 90, true, true, true, 0));
            surfaceDeadLoad.Add(new CSLoad_FreeUniform(ELoadCoordSystem.eGCS, ELoadDir.eLD_Z, pWallFrontLeft, fWallLeftOrRight_X, fWallLeftOrRight_Y, generalLoad.fDeadLoadTotal_Wall, 90, 0, 90, Colors.DeepPink, true, true, true, 0));
            surfaceDeadLoad.Add(new CSLoad_FreeUniform(ELoadCoordSystem.eGCS, ELoadDir.eLD_Z, pWallBackRight, fWallLeftOrRight_X, fWallLeftOrRight_Y, generalLoad.fDeadLoadTotal_Wall, 90, 0, 180+90, Colors.DeepPink, true, true, true, 0));
            surfaceDeadLoad.Add(new CSLoad_FreeUniform(ELoadCoordSystem.eGCS, ELoadDir.eLD_Z, pWallFrontLeft, fWallFrontOrBack_X, fWallFrontOrBack_Y1, 0.5f * fWallFrontOrBack_X, fWallFrontOrBack_Y2, fWallFrontOrBack_Y1, generalLoad.fDeadLoadTotal_Wall, 90, 0, 0, Colors.DeepPink, true, true, false, true, 0));
            surfaceDeadLoad.Add(new CSLoad_FreeUniform(ELoadCoordSystem.eGCS, ELoadDir.eLD_Z, pWallBackRight, fWallFrontOrBack_X, fWallFrontOrBack_Y1, 0.5f * fWallFrontOrBack_X, fWallFrontOrBack_Y2, fWallFrontOrBack_Y1, generalLoad.fDeadLoadTotal_Wall, 90, 0, 180, Colors.DeepPink, true, true, false, true, 0));

            // Imposed Load - Roof
            List<CSLoad_Free> surfaceRoofImposedLoad = new List<CSLoad_Free>(2);
            surfaceRoofImposedLoad.Add(new CSLoad_FreeUniform(ELoadCoordSystem.eGCS, ELoadDir.eLD_Z, pRoofFrontApex, fRoof_X, fRoof_Y, generalLoad.fImposedLoadTotal_Roof, -fRoofPitch_rad / (float)Math.PI * 180f, 0, 90, Colors.Red, true, true, true, 0));
            surfaceRoofImposedLoad.Add(new CSLoad_FreeUniform(ELoadCoordSystem.eGCS, ELoadDir.eLD_Z, pRoofFrontRight, fRoof_X, fRoof_Y, generalLoad.fImposedLoadTotal_Roof, fRoofPitch_rad / (float)Math.PI * 180f, 0, 90, Colors.Red, true, true, true, 0));

            // Snow Load - Roof
            float fSlopeFactor = ((0.5f * fW_frame) / ((0.5f * fW_frame) / (float)Math.Cos(fRoofPitch_rad))); // Consider projection acc. to Figure 4.1
            float fsnowULS_Nu_1 = snow.fs_ULS_Nu_1 * fSlopeFactor; // Design value (projection on roof)
            float fsnowULS_Nu_2 = snow.fs_ULS_Nu_2 * fSlopeFactor;
            float fsnowSLS_Nu_1 = snow.fs_SLS_Nu_1 * fSlopeFactor;
            float fsnowSLS_Nu_2 = snow.fs_SLS_Nu_2 * fSlopeFactor;

            List <CSLoad_Free> surfaceRoofSnowLoad_Nu_1 = new List<CSLoad_Free>(2);
            surfaceRoofSnowLoad_Nu_1.Add(new CSLoad_FreeUniform(ELoadCoordSystem.eGCS, ELoadDir.eLD_Z, pRoofFrontApex, fRoof_X, fRoof_Y, fsnowULS_Nu_1, -fRoofPitch_rad / (float)Math.PI * 180f, 0, 90, Colors.SeaGreen, true, true, true, 0));
            surfaceRoofSnowLoad_Nu_1.Add(new CSLoad_FreeUniform(ELoadCoordSystem.eGCS, ELoadDir.eLD_Z, pRoofFrontRight, fRoof_X, fRoof_Y, fsnowULS_Nu_1, fRoofPitch_rad / (float)Math.PI * 180f, 0, 90, Colors.SeaGreen, true, true, true, 0));

            List<CSLoad_Free> surfaceRoofSnowLoad_Nu_2_Left = new List<CSLoad_Free>(1);
            surfaceRoofSnowLoad_Nu_2_Left.Add(new CSLoad_FreeUniform(ELoadCoordSystem.eGCS, ELoadDir.eLD_Z, pRoofFrontApex, fRoof_X, fRoof_Y, fsnowULS_Nu_2, -fRoofPitch_rad / (float)Math.PI * 180f, 0, 90, Colors.SeaGreen, true, true, true, 0));
            List<CSLoad_Free> surfaceRoofSnowLoad_Nu_2_Right = new List<CSLoad_Free>(1);
            surfaceRoofSnowLoad_Nu_2_Right.Add(new CSLoad_FreeUniform(ELoadCoordSystem.eGCS, ELoadDir.eLD_Z, pRoofFrontRight, fRoof_X, fRoof_Y, fsnowULS_Nu_2, fRoofPitch_rad / (float)Math.PI * 180f, 0, 90, Colors.SeaGreen, true, true, true, 0));

            // Wind Load

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

            SetSurfaceWindLoads(
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

            SetSurfaceWindLoads(
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

            // Load Cases
            m_arrLoadCases = new CLoadCase[36];
            m_arrLoadCases[00] = new CLoadCase(01, "Dead load G", ELCType.ePermanentLoad, ELCMainDirection.eGeneral, memberLoadDead1Rafters, surfaceDeadLoad);                                  // 01
            m_arrLoadCases[01] = new CLoadCase(02, "Imposed load Q", ELCType.eImposedLoad_ST, ELCMainDirection.eGeneral, memberLoadImposedRafters, surfaceRoofImposedLoad);                     // 02

            // ULS - Load Case
            m_arrLoadCases[02] = new CLoadCase(03, "Snow load Su - full", ELCType.eSnow, ELCMainDirection.ePlusZ, memberMaxLoadSnowAllRafters, surfaceRoofSnowLoad_Nu_1);                       // 03
            m_arrLoadCases[03] = new CLoadCase(04, "Snow load Su - left", ELCType.eSnow, ELCMainDirection.ePlusZ, memberMaxLoadSnowLeftRafters, surfaceRoofSnowLoad_Nu_2_Left);                 // 04
            m_arrLoadCases[04] = new CLoadCase(05, "Snow load Su - right", ELCType.eSnow, ELCMainDirection.ePlusZ, memberMaxLoadSnowRightRafters, surfaceRoofSnowLoad_Nu_2_Right);              // 05
            m_arrLoadCases[05] = new CLoadCase(06, "Wind load Wu - Cpi - Left - X+", ELCType.eWind, ELCMainDirection.ePlusX);                                                                   // 06
            m_arrLoadCases[06] = new CLoadCase(07, "Wind load Wu - Cpi - Right - X-", ELCType.eWind, ELCMainDirection.eMinusX);                                                                 // 07
            m_arrLoadCases[07] = new CLoadCase(08, "Wind load Wu - Cpi - Front - Y+", ELCType.eWind, ELCMainDirection.ePlusY);                                                                  // 08
            m_arrLoadCases[08] = new CLoadCase(09, "Wind load Wu - Cpi - Rear - Y-", ELCType.eWind, ELCMainDirection.eMinusY);                                                                  // 09
            m_arrLoadCases[09] = new CLoadCase(10, "Wind load Wu - Cpe,min - Left - X+", ELCType.eWind, ELCMainDirection.ePlusX, memberLoadWindFramesPlusX_CpeMin, surfaceWindLoad_ULS_PlusX_Cpemin);     // 10
            m_arrLoadCases[10] = new CLoadCase(11, "Wind load Wu - Cpe,min - Right - X-", ELCType.eWind, ELCMainDirection.eMinusX, memberLoadWindFramesMinusX_CpeMin, surfaceWindLoad_ULS_MinusX_Cpemin); // 11
            m_arrLoadCases[11] = new CLoadCase(12, "Wind load Wu - Cpe,min - Front - Y+", ELCType.eWind, ELCMainDirection.ePlusY, memberLoadWindFramesPlusY_CpeMin, surfaceWindLoad_ULS_PlusY_Cpemin);    // 12
            m_arrLoadCases[12] = new CLoadCase(13, "Wind load Wu - Cpe,min - Rear - Y-", ELCType.eWind, ELCMainDirection.eMinusY,memberLoadWindFramesMinusY_CpeMin, surfaceWindLoad_ULS_MinusY_Cpemin);   // 13
            m_arrLoadCases[13] = new CLoadCase(14, "Wind load Wu - Cpe,max - Left - X+", ELCType.eWind, ELCMainDirection.ePlusX, memberLoadWindFramesPlusX_CpeMax, surfaceWindLoad_ULS_PlusX_Cpemax);     // 14
            m_arrLoadCases[14] = new CLoadCase(15, "Wind load Wu - Cpe,max - Right - X-", ELCType.eWind, ELCMainDirection.eMinusX, memberLoadWindFramesMinusX_CpeMax, surfaceWindLoad_ULS_MinusX_Cpemax); // 15
            m_arrLoadCases[15] = new CLoadCase(16, "Wind load Wu - Cpe,max - Front - Y+", ELCType.eWind, ELCMainDirection.ePlusY, memberLoadWindFramesPlusY_CpeMax, surfaceWindLoad_ULS_PlusY_Cpemax);    // 16
            m_arrLoadCases[16] = new CLoadCase(17, "Wind load Wu - Cpe,max - Rear - Y-", ELCType.eWind, ELCMainDirection.eMinusY,memberLoadWindFramesMinusY_CpeMax, surfaceWindLoad_ULS_MinusY_Cpemax);   // 17
            m_arrLoadCases[17] = new CLoadCase(18, "Earthquake load Eu - X", ELCType.eEarthquake, ELCMainDirection.ePlusX);                                                                     // 18
            m_arrLoadCases[18] = new CLoadCase(19, "Earthquake load Eu - Y", ELCType.eEarthquake, ELCMainDirection.ePlusY);                                                                     // 19

            // SLS - Load Case
            m_arrLoadCases[19] = new CLoadCase(20, "Snow load Ss - full", ELCType.eSnow, ELCMainDirection.ePlusZ, memberMaxLoadSnowAllRafters);                                                 // 20
            m_arrLoadCases[20] = new CLoadCase(21, "Snow load Ss - left", ELCType.eSnow, ELCMainDirection.ePlusZ, memberMaxLoadSnowLeftRafters);                                                // 21
            m_arrLoadCases[21] = new CLoadCase(22, "Snow load Ss - right", ELCType.eSnow, ELCMainDirection.ePlusZ, memberMaxLoadSnowRightRafters);                                              // 22
            m_arrLoadCases[22] = new CLoadCase(23, "Wind load Ws - Cpi - Left - X+", ELCType.eWind, ELCMainDirection.ePlusX);                                                                   // 23
            m_arrLoadCases[23] = new CLoadCase(24, "Wind load Ws - Cpi - Right - X-", ELCType.eWind, ELCMainDirection.eMinusX);                                                                 // 24
            m_arrLoadCases[24] = new CLoadCase(25, "Wind load Ws - Cpi - Front - Y+", ELCType.eWind, ELCMainDirection.ePlusY);                                                                  // 25
            m_arrLoadCases[25] = new CLoadCase(26, "Wind load Ws - Cpi - Rear - Y-", ELCType.eWind, ELCMainDirection.eMinusY);                                                                  // 26
            m_arrLoadCases[26] = new CLoadCase(27, "Wind load Ws - Cpe,min - Left - X+", ELCType.eWind, ELCMainDirection.ePlusX, memberLoadWindFramesPlusX_CpeMin, surfaceWindLoad_SLS_PlusX_Cpemin);     // 27
            m_arrLoadCases[27] = new CLoadCase(28, "Wind load Ws - Cpe,min - Right - X-", ELCType.eWind, ELCMainDirection.eMinusX, memberLoadWindFramesMinusX_CpeMin, surfaceWindLoad_SLS_MinusX_Cpemin); // 28
            m_arrLoadCases[28] = new CLoadCase(29, "Wind load Ws - Cpe,min - Front - Y+", ELCType.eWind, ELCMainDirection.ePlusY, memberLoadWindFramesPlusY_CpeMin, surfaceWindLoad_SLS_PlusY_Cpemin);    // 29
            m_arrLoadCases[29] = new CLoadCase(30, "Wind load Ws - Cpe,min - Rear - Y-", ELCType.eWind, ELCMainDirection.eMinusY,memberLoadWindFramesMinusY_CpeMin, surfaceWindLoad_SLS_MinusY_Cpemin);   // 30
            m_arrLoadCases[30] = new CLoadCase(31, "Wind load Ws - Cpe,max - Left - X+", ELCType.eWind, ELCMainDirection.ePlusX, memberLoadWindFramesPlusX_CpeMax, surfaceWindLoad_SLS_PlusX_Cpemax);     // 31
            m_arrLoadCases[31] = new CLoadCase(32, "Wind load Ws - Cpe,max - Right - X-", ELCType.eWind, ELCMainDirection.eMinusX, memberLoadWindFramesMinusX_CpeMax, surfaceWindLoad_SLS_MinusX_Cpemax); // 32
            m_arrLoadCases[32] = new CLoadCase(33, "Wind load Ws - Cpe,max - Front - Y+", ELCType.eWind, ELCMainDirection.ePlusY, memberLoadWindFramesPlusY_CpeMax, surfaceWindLoad_SLS_PlusY_Cpemax);    // 33
            m_arrLoadCases[33] = new CLoadCase(34, "Wind load Ws - Cpe,max - Rear - Y-", ELCType.eWind, ELCMainDirection.eMinusY,memberLoadWindFramesMinusY_CpeMax, surfaceWindLoad_SLS_MinusY_Cpemax);   // 34
            m_arrLoadCases[34] = new CLoadCase(35, "Earthquake load Es - X", ELCType.eEarthquake, ELCMainDirection.ePlusX);                                                                     // 35
            m_arrLoadCases[35] = new CLoadCase(36, "Earthquake load Es - Y", ELCType.eEarthquake, ELCMainDirection.ePlusY);                                                                     // 36

            // Load Case Groups
            m_arrLoadCaseGroups = new CLoadCaseGroup[12];

            // Dead Load
            m_arrLoadCaseGroups[0] = new CLoadCaseGroup(1, "Dead load", ELCGTypeForLimitState.eUniversal, ELCGType.eTogether);
            m_arrLoadCaseGroups[0].MLoadCasesList.Add(m_arrLoadCases[00]);

            // Imposed Load
            m_arrLoadCaseGroups[1] = new CLoadCaseGroup(2, "Imposed load", ELCGTypeForLimitState.eUniversal, ELCGType.eExclusive);
            m_arrLoadCaseGroups[1].MLoadCasesList.Add(m_arrLoadCases[01]);

            // ULS Load Case Groups
            // Snow Load
            m_arrLoadCaseGroups[2] = new CLoadCaseGroup(3, "Snow load", ELCGTypeForLimitState.eULSOnly, ELCGType.eExclusive);
            m_arrLoadCaseGroups[2].MLoadCasesList.Add(m_arrLoadCases[02]);
            m_arrLoadCaseGroups[2].MLoadCasesList.Add(m_arrLoadCases[03]);
            m_arrLoadCaseGroups[2].MLoadCasesList.Add(m_arrLoadCases[04]);

            // Wind Load
            m_arrLoadCaseGroups[3] = new CLoadCaseGroup(4, "Wind load - Cpi", ELCGTypeForLimitState.eULSOnly, ELCGType.eExclusive);
            m_arrLoadCaseGroups[3].MLoadCasesList.Add(m_arrLoadCases[05]);
            m_arrLoadCaseGroups[3].MLoadCasesList.Add(m_arrLoadCases[06]);
            m_arrLoadCaseGroups[3].MLoadCasesList.Add(m_arrLoadCases[07]);
            m_arrLoadCaseGroups[3].MLoadCasesList.Add(m_arrLoadCases[08]);

            // Wind Load
            m_arrLoadCaseGroups[4] = new CLoadCaseGroup(5, "Wind load - Cpe,min", ELCGTypeForLimitState.eULSOnly, ELCGType.eExclusive);
            m_arrLoadCaseGroups[4].MLoadCasesList.Add(m_arrLoadCases[09]);
            m_arrLoadCaseGroups[4].MLoadCasesList.Add(m_arrLoadCases[10]);
            m_arrLoadCaseGroups[4].MLoadCasesList.Add(m_arrLoadCases[11]);
            m_arrLoadCaseGroups[4].MLoadCasesList.Add(m_arrLoadCases[12]);

            // Wind Load
            m_arrLoadCaseGroups[5] = new CLoadCaseGroup(6, "Wind load - Cpe,max", ELCGTypeForLimitState.eULSOnly, ELCGType.eExclusive);
            m_arrLoadCaseGroups[5].MLoadCasesList.Add(m_arrLoadCases[13]);
            m_arrLoadCaseGroups[5].MLoadCasesList.Add(m_arrLoadCases[14]);
            m_arrLoadCaseGroups[5].MLoadCasesList.Add(m_arrLoadCases[15]);
            m_arrLoadCaseGroups[5].MLoadCasesList.Add(m_arrLoadCases[16]);

            // Earthquake Load
            m_arrLoadCaseGroups[6] = new CLoadCaseGroup(7, "Earthquake", ELCGTypeForLimitState.eULSOnly, ELCGType.eExclusive);
            m_arrLoadCaseGroups[6].MLoadCasesList.Add(m_arrLoadCases[17]);
            m_arrLoadCaseGroups[6].MLoadCasesList.Add(m_arrLoadCases[18]);

            // SLS Load Case Groups
            // Snow Load
            m_arrLoadCaseGroups[7] = new CLoadCaseGroup(8, "Snow load", ELCGTypeForLimitState.eSLSOnly, ELCGType.eExclusive);
            m_arrLoadCaseGroups[7].MLoadCasesList.Add(m_arrLoadCases[19]);
            m_arrLoadCaseGroups[7].MLoadCasesList.Add(m_arrLoadCases[20]);
            m_arrLoadCaseGroups[7].MLoadCasesList.Add(m_arrLoadCases[21]);

            // Wind Load
            m_arrLoadCaseGroups[8] = new CLoadCaseGroup(9, "Wind load - Cpi", ELCGTypeForLimitState.eSLSOnly, ELCGType.eExclusive);
            m_arrLoadCaseGroups[8].MLoadCasesList.Add(m_arrLoadCases[22]);
            m_arrLoadCaseGroups[8].MLoadCasesList.Add(m_arrLoadCases[23]);
            m_arrLoadCaseGroups[8].MLoadCasesList.Add(m_arrLoadCases[24]);
            m_arrLoadCaseGroups[8].MLoadCasesList.Add(m_arrLoadCases[25]);

            // Wind Load
            m_arrLoadCaseGroups[9] = new CLoadCaseGroup(10, "Wind load - Cpe,min", ELCGTypeForLimitState.eSLSOnly, ELCGType.eExclusive);
            m_arrLoadCaseGroups[9].MLoadCasesList.Add(m_arrLoadCases[26]);
            m_arrLoadCaseGroups[9].MLoadCasesList.Add(m_arrLoadCases[27]);
            m_arrLoadCaseGroups[9].MLoadCasesList.Add(m_arrLoadCases[28]);
            m_arrLoadCaseGroups[9].MLoadCasesList.Add(m_arrLoadCases[29]);

            // Wind Load
            m_arrLoadCaseGroups[10] = new CLoadCaseGroup(11, "Wind load - Cpe,max", ELCGTypeForLimitState.eSLSOnly, ELCGType.eExclusive);
            m_arrLoadCaseGroups[10].MLoadCasesList.Add(m_arrLoadCases[30]);
            m_arrLoadCaseGroups[10].MLoadCasesList.Add(m_arrLoadCases[31]);
            m_arrLoadCaseGroups[10].MLoadCasesList.Add(m_arrLoadCases[32]);
            m_arrLoadCaseGroups[10].MLoadCasesList.Add(m_arrLoadCases[33]);

            // Earthquake Load
            m_arrLoadCaseGroups[11] = new CLoadCaseGroup(12, "Earthquake", ELCGTypeForLimitState.eSLSOnly, ELCGType.eExclusive);
            m_arrLoadCaseGroups[11].MLoadCasesList.Add(m_arrLoadCases[34]);
            m_arrLoadCaseGroups[11].MLoadCasesList.Add(m_arrLoadCases[35]);

            // Load Combinations

            // TEMPORARY - ukazka
            // TODO No 40 - Ondrej
            // Vysledkom ma byt toto pole Load Combinations, kombinacie maju obsahovat zoznam load cases a prislusne faktory ktorymi treba load cases prenasobovat
            CLoadCombinationsGenerator generator = new CLoadCombinationsGenerator(m_arrLoadCaseGroups);
            generator.GenerateAll();
            //generator.GenerateULS();
            //generator.GenerateSLS();
            //generator.WriteCombinationsLoadCases();
            generator.WritePermutations();
            generator.WriteCombinations();

            // TODO No 40  Notes to Ondrej
            // Zopar navrhov na vylepsenie

            //////////////////////////////////////////////////////////////////////////////////////////////////////
            // (A) Priorita 4
            // Bolo by super zobrazit v dalsom stlpci v UC_LoadCombinationsList predpis "Combination Key", podla ktoreho sa to vygenerovalo
            // pripadne aj clanok normy v dalsom stlpci, lahsie by sa to potom kontrolovalo a vyzeralo viac profi :)
            // Napr.:
            // [0.9*G + Wu,Cpi + Wu,Cpe,max + ψc*Q] ψc = 0    | AS/NZS 1170.0, cl. 4.2.1(a)
            // [1.2*G + 1.5*Q]                                | AS/NZS 1170.0, cl. 4.2.1(b)(ii)
            // [G + Eu + ψE*Q] ψE = 0                         | AS/NZS 1170.0, cl. 4.2.2(f)

            // (B)
            // Nezobrazuju sa kombinacie so zemetrasenim EQ load cases ID 17,18 a 34,35

            //m_arrLoadCombs = new CLoadCombination[generator.Combinations.Count];
            //for (int i = 0; i < m_arrLoadCombs.Length; i++)
            //    m_arrLoadCombs[i] = generator.Combinations[i];
            m_arrLoadCombs = generator.Combinations.ToArray();

            // Limit States
            m_arrLimitStates = new CLimitState[3];
            m_arrLimitStates[0] = new CLimitState("Ultimate Limit State - Stability", ELSType.eLS_ULS);
            m_arrLimitStates[1] = new CLimitState("Ultimate Limit State - Strength" , ELSType.eLS_ULS);
            m_arrLimitStates[2] = new CLimitState("Serviceability Limit State"      , ELSType.eLS_SLS);
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
            float fx = (float)Geom2D.GetRotatedPosition_x_CCW(node.X, node.Y, fAngle_rad);
            float fy = (float)Geom2D.GetRotatedPosition_y_CCW(node.X, node.Y, fAngle_rad);

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

        public void AddColumnsMembers(int i_temp_numberofNodes, int i_temp_numberofMembers, int iOneRafterColumnNo, int iColumnNoInOneFrame, float fColumnAlignmentStart, float fColumnAlignmentEnd, CCrSc section, float fMemberRotation)
        {
            // Members - Columns
            for (int i = 0; i < iOneRafterColumnNo; i++)
            {
                m_arrMembers[i_temp_numberofMembers + i] = new CMember(i_temp_numberofMembers + i + 1, m_arrNodes[i_temp_numberofNodes + i], m_arrNodes[i_temp_numberofNodes + iColumnNoInOneFrame + i], section, EMemberType_FormSteel.eC, null, null, fColumnAlignmentStart, fColumnAlignmentEnd, fMemberRotation, 0);
            }

            for (int i = 0; i < iOneRafterColumnNo; i++)
            {
                m_arrMembers[i_temp_numberofMembers + iOneRafterColumnNo + i] = new CMember(i_temp_numberofMembers + iOneRafterColumnNo + i + 1, m_arrNodes[i_temp_numberofNodes + iOneRafterColumnNo + i], m_arrNodes[i_temp_numberofNodes + iColumnNoInOneFrame + iOneRafterColumnNo + i], section, EMemberType_FormSteel.eC, null, null, fColumnAlignmentStart, fColumnAlignmentEnd, fMemberRotation, 0);
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

        public void AddFrontOrBackGirtsMembers(int iFrameNodesNo, int iOneRafterColumnNo, int[] iArrNumberOfNodesPerColumn, int i_temp_numberofNodes, int i_temp_numberofMembers, int iIntermediateColumnNodesForGirtsOneRafterNo, int iIntermediateColumnNodesForGirtsOneFrameNo, int iTempJumpBetweenFrontAndBack_GirtsNumberInLongidutinalDirection, float fDist_Girts, CMemberEccentricity eGirtEccentricity, float fGirtStart_MC, float fGirtStart, float fGirtEnd, CCrSc section, float fMemberRotation)
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
                m_arrCrSc[arraysizeoriginal + i - 1].ICrSc_ID = arraysizeoriginal + i/* -1 + 1*/; // Odcitat index pretoze prvy prierez ignorujeme a pridat 1 pre ID (+ 1)
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

                // TODO - nizsie uvedeny kod zmaze priradenie pruta do zoznamu prutov v priereze (list prutov ktorym je prierez priradeny), prut je totizto uz priradeny k prierezu kedze bol uz priradeny v bloku
                // TODO - dvojite priradenie by sa malo vyriesit nejako elegantnejsie

                // We need to remove assignment of member to the girt cross-section, it is already assigned
                CCrSc GirtCrossSection = block.ReferenceGirt.CrScStart;

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
                }
            }

            // TODO - odstranit spoje na deaktivovanych prutoch

            // Add block member connections to the main model connections
            foreach (CConnectionJointTypes joint in block.m_arrConnectionJoints)
                m_arrConnectionJoints.Add(joint);
        }

        // Loading

        // Main Columns
        public void GenerateLoadOnMainColumns(float fValue1, float fValue2, ref List<CMLoad> list)
        {
            for (int i = 0; i < iFrameNo; i++)
            {
                CMLoad loadleft = new CMLoad_21(fValue1, m_arrMembers[i * (2 + 2 + 2)], EMLoadTypeDistr.eMLT_FS_G_11, EMLoadType.eMLT_F, EMLoadDirPCC1.eMLD_PCC_FZV_MYU, true, 0);
                CMLoad loadright = new CMLoad_21(fValue2, m_arrMembers[3 + i * (2 + 2 + 2)], EMLoadTypeDistr.eMLT_FS_G_11, EMLoadType.eMLT_F, EMLoadDirPCC1.eMLD_PCC_FZV_MYU, true, 0);
                list.Add(loadleft);
                list.Add(loadright);
            }
        }

        // Rafters
        public void GenerateLoadOnRafters(float fValue1, float fValue2, ref List<CMLoad> list)
        {
            for (int i = 0; i < iFrameNo; i++)
            {
                CMLoad loadleft = new CMLoad_21(fValue1, m_arrMembers[1 + i * (2 + 2 + 2)], EMLoadTypeDistr.eMLT_FS_G_11, EMLoadType.eMLT_F, EMLoadDirPCC1.eMLD_PCC_FZV_MYU, true, 0);
                CMLoad loadright = new CMLoad_21(fValue2, m_arrMembers[1 + i * (2 + 2 + 2) + 1], EMLoadTypeDistr.eMLT_FS_G_11, EMLoadType.eMLT_F, EMLoadDirPCC1.eMLD_PCC_FZV_MYU, true, 0);
                list.Add(loadleft);
                list.Add(loadright);
            }
        }

        // Frames
        public void GenerateLoadOnFrames(float fValueColumn1, float fValueColumn2, float fValueRafter1, float fValueRafter2, ref List<CMLoad> list)
        {
            GenerateLoadOnMainColumns(fValueColumn1, fValueColumn2, ref list);
            GenerateLoadOnRafters(fValueRafter1, fValueRafter2, ref list);
        }

        private void SetSurfaceWindLoads(
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
            SetSurfaceWindLoads(
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
            SetSurfaceWindLoads(
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

        private void SetSurfaceWindLoads(
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
            surfaceWindLoadPlusX_Cpe.Add(new CSLoad_FreeUniformGroup(ELoadCoordSystem.eLCS, ELoadDir.eLD_Z, pRoofFrontLeft, wind.fC_pe_U_roof_dimensions, fRoof_Y, fRoof_X, ELCMainDirection.ePlusX, fp_e_U_roof_Theta_4, 0, -fRoofPitch_rad / (float)Math.PI * 180f, 0, false, false, true, 0));
            surfaceWindLoadPlusX_Cpe.Add(new CSLoad_FreeUniformGroup(ELoadCoordSystem.eLCS, ELoadDir.eLD_Z, pRoofFrontApex, wind.fC_pe_D_roof_dimensions, fRoof_Y, fRoof_X, ELCMainDirection.ePlusX, fp_e_D_roof_Theta_4, 0, fRoofPitch_rad / (float)Math.PI * 180f, 0, false, false, true, 0, wind.iFirst_D_SegmentColorID));
            surfaceWindLoadPlusX_Cpe.Add(new CSLoad_FreeUniformGroup(ELoadCoordSystem.eLCS, ELoadDir.eLD_Z, pWallFrontLeft, wind.fC_pe_S_wall_dimensions, fWallFrontOrBack_X, fWallFrontOrBack_Y1, fWallFrontOrBack_Y2, ELCMainDirection.ePlusX, fp_e_S_wall_Theta_4, 90, 0, 0, false, false, true, 0));
            surfaceWindLoadPlusX_Cpe.Add(new CSLoad_FreeUniformGroup(ELoadCoordSystem.eLCS, ELoadDir.eLD_Z, pWallBackLeft, wind.fC_pe_S_wall_dimensions, fWallFrontOrBack_X, fWallFrontOrBack_Y1, fWallFrontOrBack_Y2, ELCMainDirection.ePlusX, fp_e_S_wall_Theta_4, 90, 0, 0, true, false, true, 0));
            surfaceWindLoadPlusX_Cpe.Add(new CSLoad_FreeUniform(ELoadCoordSystem.eLCS, ELoadDir.eLD_Z, pWallFrontLeft, fWallLeftOrRight_X, fWallLeftOrRight_Y, fp_e_W_wall_Theta_4[(int)ELCMainDirection.ePlusX], 90, 0, 90, cColorWindPressure, false, false, true, 0));
            surfaceWindLoadPlusX_Cpe.Add(new CSLoad_FreeUniform(ELoadCoordSystem.eLCS, ELoadDir.eLD_Z, pWallFrontRight, fWallLeftOrRight_X, fWallLeftOrRight_Y, fp_e_L_wall_Theta_4[(int)ELCMainDirection.ePlusX], 90, 0, 90, cColorWindSagging, false, false, true, 0));

            surfaceWindLoadMinusX_Cpe = new List<CSLoad_Free>(6);
            surfaceWindLoadMinusX_Cpe.Add(new CSLoad_FreeUniformGroup(ELoadCoordSystem.eLCS, ELoadDir.eLD_Z, pRoofBackApex, wind.fC_pe_D_roof_dimensions, fRoof_Y, fRoof_X, ELCMainDirection.eMinusX, fp_e_D_roof_Theta_4, 0, fRoofPitch_rad / (float)Math.PI * 180f, 180, false, false, true, 0, wind.iFirst_D_SegmentColorID));
            surfaceWindLoadMinusX_Cpe.Add(new CSLoad_FreeUniformGroup(ELoadCoordSystem.eLCS, ELoadDir.eLD_Z, pRoofBackRight, wind.fC_pe_U_roof_dimensions, fRoof_Y, fRoof_X, ELCMainDirection.eMinusX, fp_e_U_roof_Theta_4, 0, -fRoofPitch_rad / (float)Math.PI * 180f, 180, false, false, true, 0));
            surfaceWindLoadMinusX_Cpe.Add(new CSLoad_FreeUniformGroup(ELoadCoordSystem.eLCS, ELoadDir.eLD_Z, pWallFrontRight, wind.fC_pe_S_wall_dimensions, fWallFrontOrBack_X, fWallFrontOrBack_Y1, fWallFrontOrBack_Y2, ELCMainDirection.eMinusX, fp_e_S_wall_Theta_4, 90, 0, 180, true, false, true, 0));
            surfaceWindLoadMinusX_Cpe.Add(new CSLoad_FreeUniformGroup(ELoadCoordSystem.eLCS, ELoadDir.eLD_Z, pWallBackRight, wind.fC_pe_S_wall_dimensions, fWallFrontOrBack_X, fWallFrontOrBack_Y1, fWallFrontOrBack_Y2, ELCMainDirection.eMinusX, fp_e_S_wall_Theta_4, 90, 0, 180, false, false, true, 0));
            surfaceWindLoadMinusX_Cpe.Add(new CSLoad_FreeUniform(ELoadCoordSystem.eLCS, ELoadDir.eLD_Z, pWallFrontLeft, fWallLeftOrRight_X, fWallLeftOrRight_Y, fp_e_L_wall_Theta_4[(int)ELCMainDirection.eMinusX], 90, 0, 90, cColorWindSagging, true, false, true, 0));
            surfaceWindLoadMinusX_Cpe.Add(new CSLoad_FreeUniform(ELoadCoordSystem.eLCS, ELoadDir.eLD_Z, pWallFrontRight, fWallLeftOrRight_X, fWallLeftOrRight_Y, fp_e_W_wall_Theta_4[(int)ELCMainDirection.eMinusX], 90, 0, 90, cColorWindPressure, true, false, true, 0));

            surfaceWindLoadPlusY_Cpe = new List<CSLoad_Free>(6);
            surfaceWindLoadPlusY_Cpe.Add(new CSLoad_FreeUniformGroup(ELoadCoordSystem.eLCS, ELoadDir.eLD_Z, pRoofFrontApex, wind.fC_pe_R_roof_dimensions, fRoof_X, fRoof_Y, ELCMainDirection.ePlusY, fp_e_R_roof_Theta_4, -fRoofPitch_rad / (float)Math.PI * 180f, 0, 90, false, false, true, 0));
            surfaceWindLoadPlusY_Cpe.Add(new CSLoad_FreeUniformGroup(ELoadCoordSystem.eLCS, ELoadDir.eLD_Z, pRoofFrontRight, wind.fC_pe_R_roof_dimensions, fRoof_X, fRoof_Y, ELCMainDirection.ePlusY, fp_e_R_roof_Theta_4, fRoofPitch_rad / (float)Math.PI * 180f, 0, 90, false, false, true, 0));
            surfaceWindLoadPlusY_Cpe.Add(new CSLoad_FreeUniformGroup(ELoadCoordSystem.eLCS, ELoadDir.eLD_Z, pWallFrontLeft, wind.fC_pe_S_wall_dimensions, fWallLeftOrRight_X, fWallLeftOrRight_Y, ELCMainDirection.ePlusY, fp_e_S_wall_Theta_4, 90, 0, 90, true, false, true, 0));
            surfaceWindLoadPlusY_Cpe.Add(new CSLoad_FreeUniformGroup(ELoadCoordSystem.eLCS, ELoadDir.eLD_Z, pWallFrontRight, wind.fC_pe_S_wall_dimensions, fWallLeftOrRight_X, fWallLeftOrRight_Y, ELCMainDirection.ePlusY, fp_e_S_wall_Theta_4, 90, 0, 90, false, false, true, 0));
            surfaceWindLoadPlusY_Cpe.Add(new CSLoad_FreeUniform(ELoadCoordSystem.eLCS, ELoadDir.eLD_Z, pWallFrontLeft, fWallFrontOrBack_X, fWallFrontOrBack_Y1, fWallFrontOrBack_Y2, fp_e_W_wall_Theta_4[(int)ELCMainDirection.ePlusY], 90, 0, 0, cColorWindPressure, true, true, false, true, 0));
            surfaceWindLoadPlusY_Cpe.Add(new CSLoad_FreeUniform(ELoadCoordSystem.eLCS, ELoadDir.eLD_Z, pWallBackRight, fWallFrontOrBack_X, fWallFrontOrBack_Y1, fWallFrontOrBack_Y2, fp_e_L_wall_Theta_4[(int)ELCMainDirection.ePlusY], 90, 0, 180, cColorWindSagging, false, false, false, true, 0));

            surfaceWindLoadMinusY_Cpe = new List<CSLoad_Free>(6);
            surfaceWindLoadMinusY_Cpe.Add(new CSLoad_FreeUniformGroup(ELoadCoordSystem.eLCS, ELoadDir.eLD_Z, pRoofBackLeft, wind.fC_pe_R_roof_dimensions, fRoof_X, fRoof_Y, ELCMainDirection.eMinusY, fp_e_R_roof_Theta_4, fRoofPitch_rad / (float)Math.PI * 180f, 0, 180 + 90, false, false, true, 0));
            surfaceWindLoadMinusY_Cpe.Add(new CSLoad_FreeUniformGroup(ELoadCoordSystem.eLCS, ELoadDir.eLD_Z, pRoofBackApex, wind.fC_pe_R_roof_dimensions, fRoof_X, fRoof_Y, ELCMainDirection.eMinusY, fp_e_R_roof_Theta_4, -fRoofPitch_rad / (float)Math.PI * 180f, 0, 180 + 90, false, false, true, 0));
            surfaceWindLoadMinusY_Cpe.Add(new CSLoad_FreeUniformGroup(ELoadCoordSystem.eLCS, ELoadDir.eLD_Z, pWallBackLeft, wind.fC_pe_S_wall_dimensions, fWallLeftOrRight_X, fWallLeftOrRight_Y, ELCMainDirection.eMinusY, fp_e_S_wall_Theta_4, 90, 0, 180 + 90, false, false, true, 0));
            surfaceWindLoadMinusY_Cpe.Add(new CSLoad_FreeUniformGroup(ELoadCoordSystem.eLCS, ELoadDir.eLD_Z, pWallBackRight, wind.fC_pe_S_wall_dimensions, fWallLeftOrRight_X, fWallLeftOrRight_Y, ELCMainDirection.eMinusY, fp_e_S_wall_Theta_4, 90, 0, 180 + 90, true, false, true, 0));
            surfaceWindLoadMinusY_Cpe.Add(new CSLoad_FreeUniform(ELoadCoordSystem.eLCS, ELoadDir.eLD_Z, pWallFrontLeft, fWallFrontOrBack_X, fWallFrontOrBack_Y1, fWallFrontOrBack_Y2, fp_e_L_wall_Theta_4[(int)ELCMainDirection.eMinusY], 90, 0, 0, cColorWindSagging, false, false, false, true, 0));
            surfaceWindLoadMinusY_Cpe.Add(new CSLoad_FreeUniform(ELoadCoordSystem.eLCS, ELoadDir.eLD_Z, pWallBackRight, fWallFrontOrBack_X, fWallFrontOrBack_Y1, fWallFrontOrBack_Y2, fp_e_W_wall_Theta_4[(int)ELCMainDirection.eMinusY], 90, 0, 180, cColorWindPressure, true, true, false, true, 0));
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




        }


    }
}
