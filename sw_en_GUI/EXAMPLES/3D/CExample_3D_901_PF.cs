﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using BaseClasses;
using BaseClasses.GraphObj;
using MATH;
using MATERIAL;
using CRSC;
using Combinatorics.Collections;

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
                List<DoorProperties> doorBlocksProperties,
                List<WindowProperties> windowBlocksProperties
            )
        {
        // Todo asi prepracovat na zoznam tried objektov

        string[,] componentTypesList = new string[9, 3]
            {
                {"Main Column","2x50020","AS"},
                {"Rafter","2x50020",""},
                {"Eaves Purlin","2x50020",""},
                {"Purlin","2x50020",""},
                {"Purlin Brace","2x50020",""},
                {"Girt","2x50020",""},
                {"Girt Brace","2x50020",""},
                {"Roofing","2x50020",""},
                {"Wall Cladding","2x50020",""}
            };

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

            int iOneColumnGridNo = 0;
            iGirtNoInOneFrame = 0;

            m_arrMat = new CMat[1];
            m_arrCrSc = new CCrSc[9];

            // Materials
            // Materials List - Materials Array - Fill Data of Materials Array
            m_arrMat[0] = new CMat_03_00("G550", 0.1f, 550e+6f, 550e+6f);

            // Cross-sections
            // CrSc List - CrSc Array - Fill Data of Cross-sections Array
            m_arrCrSc[0] = new CCrSc_3_63020_BOX(0.63f, 0.2f, 0.00195f, 0.00195f, Colors.Violet);     // Main Column
            m_arrCrSc[1] = new CCrSc_3_63020_BOX(0.63f, 0.2f, 0.00195f, 0.00195f, Colors.LightGreen); // Rafter
            m_arrCrSc[2] = new CCrSc_3_50020_C(0.5f, 0.2f, 0.00195f, Colors.Thistle);                 // Eaves Purlin
            m_arrCrSc[3] = new CCrSc_3_270XX_C(0.27f, 0.07f, 0.00115f, Colors.Orange);                // Girt - Wall
            m_arrCrSc[4] = new CCrSc_3_270XX_C(0.27f, 0.07f, 0.00095f, Colors.SlateBlue);             // Purlin
            m_arrCrSc[5] = new CCrSc_3_10075_BOX(0.3f, 0.1f, 0.0075f, Colors.Beige);                  // Front Column
            m_arrCrSc[6] = new CCrSc_3_10075_BOX(0.3f, 0.1f, 0.0075f, Colors.BlueViolet);             // Back Column
            m_arrCrSc[7] = new CCrSc_3_270XX_C(0.27f, 0.07f, 0.00115f, Colors.Aquamarine);            // Front Girt
            m_arrCrSc[8] = new CCrSc_3_270XX_C(0.27f, 0.07f, 0.00095f, Colors.YellowGreen);           // Back Girt

            // Member Eccentricities
            // Zadane hodnoty predpokladaju ze prierez je symetricky, je potrebne zobecnit
            CMemberEccentricity eccentricityPurlin = new CMemberEccentricity(0, (float)(0.5 * m_arrCrSc[1].h - 0.5 * m_arrCrSc[4].h));
            CMemberEccentricity eccentricityGirtLeft_X0 = new CMemberEccentricity(0, (float)(-(0.5 * m_arrCrSc[0].h - 0.5 * m_arrCrSc[3].h)));
            CMemberEccentricity eccentricityGirtRight_XB = new CMemberEccentricity(0, (float)(0.5 * m_arrCrSc[0].h - 0.5 * m_arrCrSc[3].h));

            CMemberEccentricity eccentricityEavePurlin = new CMemberEccentricity(-(float)(0.5 * m_arrCrSc[0].h + m_arrCrSc[2].y_min), 0);

            // Limit pre poziciu horneho nosnika, mala by to byt polovica suctu vysky edge (eave) purlin h a sirky nosnika b (neberie sa h pretoze nosnik je otoceny o 90 stupnov)
            fUpperGirtLimit = (float)(m_arrCrSc[2].h + m_arrCrSc[3].b);

            // Limit pre poziciu horneho nosnika (front / back girt) na prednej alebo zadnej stene budovy
            // Nosnik alebo pripoj nosnika nesmie zasahovat do prievlaku (rafter)

            fz_UpperLimitForFrontGirts = (float)((0.5 * m_arrCrSc[1].h) / Math.Cos(fRoofPitch_rad) + 0.5f * m_arrCrSc[7].b);
            fz_UpperLimitForBackGirts = (float)((0.5 * m_arrCrSc[1].h) / Math.Cos(fRoofPitch_rad) + 0.5f * m_arrCrSc[8].b);

            bool bGenerateGirts = true;
            if (bGenerateGirts)
            {
                iOneColumnGridNo = (int)((fH1_frame - fUpperGirtLimit - fBottomGirtPosition) / fDist_Girt) + 1;
                iGirtNoInOneFrame = 2 * iOneColumnGridNo;
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
            int[] iArrNumberOfNodesPerFrontColumn = new int[iOneRafterFrontColumnNo];

            bool bGenerateFrontGirts = true;

            if (bGenerateFrontGirts)
            {
                iFrontIntermediateColumnNodesForGirtsOneRafterNo = GetNumberofIntermediateNodesInColumnsForOneFrame(iOneRafterFrontColumnNo, fBottomGirtPosition, fDist_FrontColumns, fz_UpperLimitForFrontGirts);
                iFrontIntermediateColumnNodesForGirtsOneFrameNo = 2 * iFrontIntermediateColumnNodesForGirtsOneRafterNo;

                // Number of Girts - Main Frame Column
                iOneColumnGridNo = (int)((fH1_frame - fUpperGirtLimit - fBottomGirtPosition) / fDist_Girt) + 1;

                iFrontGirtsNoInOneFrame = iOneColumnGridNo;

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
            int[] iArrNumberOfNodesPerBackColumn = new int[iOneRafterBackColumnNo];

            bool bGenerateBackGirts = true;

            if (bGenerateBackGirts)
            {
                iBackIntermediateColumnNodesForGirtsOneRafterNo = GetNumberofIntermediateNodesInColumnsForOneFrame(iOneRafterBackColumnNo, fBottomGirtPosition, fDist_BackColumns, fz_UpperLimitForBackGirts);
                iBackIntermediateColumnNodesForGirtsOneFrameNo = 2 * iBackIntermediateColumnNodesForGirtsOneRafterNo;

                // Number of Girts - Main Frame Column
                iOneColumnGridNo = (int)((fH1_frame - fUpperGirtLimit - fBottomGirtPosition) / fDist_Girt) + 1;

                iBackGirtsNoInOneFrame = iOneColumnGridNo;

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
            m_arrNSupports = new CNSupport[2 * iFrameNo];

            float fCutOffOneSide = 0.005f; // Cut 5 mm from each side of member
            // Alignments
            float fMainColumnStart = 0.0f;
            float fMainColumnEnd = -0.20f * (float)m_arrCrSc[1].h - fCutOffOneSide; // ??? // TODO - dopocitat
            float fRafterStart = -0.50f * (float)m_arrCrSc[0].h - fCutOffOneSide;     // TODO - dopocitat
            float fRafterEnd = -0.25f * (float)m_arrCrSc[1].h - fCutOffOneSide;       // TODO - Calculate according to h of rafter and roof pitch
            float fEavesPurlinStart = -0.5f * (float)m_arrCrSc[1].b - fCutOffOneSide;  // Just in case that cross-section of rafter is symmetric about z-z
            float fEavesPurlinEnd = -0.5f * (float)m_arrCrSc[1].b - fCutOffOneSide;   // Just in case that cross-section of rafter is symmetric about z-z
            float fGirtStart = -0.5f * (float)m_arrCrSc[0].b - fCutOffOneSide;        // Just in case that cross-section of main column is symmetric about z-z
            float fGirtEnd = -0.5f * (float)m_arrCrSc[0].b - fCutOffOneSide;          // Just in case that cross-section of main column is symmetric about z-z
            float fPurlinStart = -0.5f * (float)m_arrCrSc[1].b - fCutOffOneSide;      // Just in case that cross-section of rafter is symmetric about z-z
            float fPurlinEnd = -0.5f * (float)m_arrCrSc[1].b - fCutOffOneSide;        // Just in case that cross-section of rafter is symmetric about z-z
            float fFrontColumnStart = 0f;
            float fFrontColumnEnd = -0.5f * (float)m_arrCrSc[1].h - fCutOffOneSide;   // TODO - Calculate according to h of rafter and roof pitch
            float fBackColumnStart = 0f;
            float fBackColumnEnd = -0.5f * (float)m_arrCrSc[1].h - fCutOffOneSide;   // TODO - Calculate according to h of rafter and roof pitch
            float fFrontGirtStart = -0.5f * (float)m_arrCrSc[5].b - fCutOffOneSide;   // Just in case that cross-section of column is symmetric about z-z
            float fFrontGirtEnd = -0.5f * (float)m_arrCrSc[5].b - fCutOffOneSide;   // Just in case that cross-section of column is symmetric about z-z
            float fBackGirtStart = -0.5f * (float)m_arrCrSc[6].b - fCutOffOneSide;   // Just in case that cross-section of column is symmetric about z-z
            float fBackGirtEnd = -0.5f * (float)m_arrCrSc[6].b - fCutOffOneSide;   // Just in case that cross-section of column is symmetric about z-z
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
                    for (int j = 0; j < iOneColumnGridNo; j++)
                    {
                        m_arrNodes[i_temp_numberofNodes + i * iGirtNoInOneFrame + j] = new CNode(i_temp_numberofNodes + i * iGirtNoInOneFrame + j + 1, 000000, i * fL1_frame, fBottomGirtPosition + j * fDist_Girt, 0);
                        RotateFrontOrBackFrameNodeAboutZ(m_arrNodes[i_temp_numberofNodes + i * iGirtNoInOneFrame + j]);
                    }

                    for (int j = 0; j < iOneColumnGridNo; j++)
                    {
                        m_arrNodes[i_temp_numberofNodes + i * iGirtNoInOneFrame + iOneColumnGridNo + j] = new CNode(i_temp_numberofNodes + i * iGirtNoInOneFrame + iOneColumnGridNo + j + 1, fW_frame, i * fL1_frame, fBottomGirtPosition + j * fDist_Girt, 0);
                        RotateFrontOrBackFrameNodeAboutZ(m_arrNodes[i_temp_numberofNodes + i * iGirtNoInOneFrame + iOneColumnGridNo + j]);
                    }
                }
            }

            // Members - Girts
            int i_temp_numberofMembers = iMainColumnNo + iRafterNo + iEavesPurlinNoInOneFrame * (iFrameNo - 1);
            if (bGenerateGirts)
            {
                for (int i = 0; i < (iFrameNo - 1); i++)
                {
                    for (int j = 0; j < iOneColumnGridNo; j++)
                    {
                        m_arrMembers[i_temp_numberofMembers + i * iGirtNoInOneFrame + j] = new CMember(i_temp_numberofMembers + i * iGirtNoInOneFrame + j + 1, m_arrNodes[i_temp_numberofNodes + i * iGirtNoInOneFrame + j], m_arrNodes[i_temp_numberofNodes + (i + 1) * iGirtNoInOneFrame + j], m_arrCrSc[3], EMemberType_FormSteel.eG, eccentricityGirtLeft_X0, eccentricityGirtLeft_X0, fGirtStart, fGirtEnd, fGirtsRotation, 0);
                        RotateFrontOrBackFrameNodeAboutZ(m_arrNodes[i_temp_numberofMembers + i * iGirtNoInOneFrame + j]);
                    }

                    for (int j = 0; j < iOneColumnGridNo; j++)
                    {
                        m_arrMembers[i_temp_numberofMembers + i * iGirtNoInOneFrame + iOneColumnGridNo + j] = new CMember(i_temp_numberofMembers + i * iGirtNoInOneFrame + iOneColumnGridNo + j + 1, m_arrNodes[i_temp_numberofNodes + i * iGirtNoInOneFrame + iOneColumnGridNo + j], m_arrNodes[i_temp_numberofNodes + (i + 1) * iGirtNoInOneFrame + iOneColumnGridNo + j], m_arrCrSc[3], EMemberType_FormSteel.eG, eccentricityGirtRight_XB, eccentricityGirtRight_XB, fGirtStart, fGirtEnd, fGirtsRotation, 0);
                        RotateFrontOrBackFrameNodeAboutZ(m_arrNodes[i_temp_numberofMembers + i * iGirtNoInOneFrame + iOneColumnGridNo + j]);
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
                AddFrontOrBackGirtsMembers(iFrameNodesNo, iOneRafterFrontColumnNo, iArrNumberOfNodesPerFrontColumn, i_temp_numberofNodes, i_temp_numberofMembers, iFrontIntermediateColumnNodesForGirtsOneRafterNo, iFrontIntermediateColumnNodesForGirtsOneFrameNo, 0, fDist_Girt, fFrontGirtStart_MC, fFrontGirtStart, fFrontGirtEnd, m_arrCrSc[7], fColumnsRotation);
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
                AddFrontOrBackGirtsMembers(iFrameNodesNo, iOneRafterBackColumnNo, iArrNumberOfNodesPerBackColumn, i_temp_numberofNodes, i_temp_numberofMembers, iBackIntermediateColumnNodesForGirtsOneRafterNo, iBackIntermediateColumnNodesForGirtsOneFrameNo, iGirtNoInOneFrame * (iFrameNo - 1), fDist_Girt, fBackGirtStart_MC, fBackGirtStart, fBackGirtEnd, m_arrCrSc[8], fColumnsRotation);
            }

            // Nodal Supports - fill values

            // Set values
            bool[] bSupport1 = { true, false, true, true, false, false };
            bool[] bSupport2 = { false, false, true, true, false, false };

            // Create Support Objects
            for (int i = 0; i < iFrameNo; i++)
            {
                m_arrNSupports[i * 2 + 0] = new CNSupport(6, i * 2 + 1, m_arrNodes[i * iFrameNodesNo], bSupport1, 0);
                m_arrNSupports[i * 2 + 1] = new CNSupport(6, i * 2 + 2, m_arrNodes[i * iFrameNodesNo + (iFrameNodesNo - 1)], bSupport2, 0);
            }

            // Setridit pole podle ID
            Array.Sort(m_arrNSupports, new CCompare_NSupportID());

            // Member Releases / hinges - fill values

            // Set values
            bool?[] bMembRelase1 = { false, false, false, false, true, false };

            // Create Release / Hinge Objects
            //m_arrMembers[02].CnRelease1 = new CNRelease(6, m_arrMembers[02].NodeStart, bMembRelase1, 0);

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
                    else if ((iNumberOfSymmetricalGirtsHalf + iNumberOfGirtsInMiddle - 1) < i && i < (iNumberOfSymmetricalGirtsHalf + iNumberOfGirtsInMiddle + iOneColumnGridNo)) // Joint at member start - connected to the second main column
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
                    else if ((iNumberOfSymmetricalGirtsHalf + iNumberOfGirtsInMiddle - 1) < i && i < (iNumberOfSymmetricalGirtsHalf + iNumberOfGirtsInMiddle + iOneColumnGridNo)) // Joint at member start - connected to the second main column
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

            if (doorBlocksProperties != null)
            {
                foreach (DoorProperties prop in doorBlocksProperties)
                {
                    AddDoorBlock(prop, 0.5f);
                }
            }

            if (windowBlocksProperties != null)
            {
                foreach (WindowProperties prop in windowBlocksProperties)
                {
                    AddWindowBlock(prop, 0.5f);
                }
            }

            // End of blocks
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            // Loading

            // Loads
            m_arrNLoads = new CNLoad[3];
            m_arrNLoads[0] = new CNLoadAll(m_arrNodes[1], 0, 0, -4f, 0, 0, 0, true, 0);
            m_arrNLoads[1] = new CNLoadAll(m_arrNodes[2], 0, 0, -4f, 0, 0, 0, true, 0);
            m_arrNLoads[2] = new CNLoadAll(m_arrNodes[3], 0, 0, -4f, 0, 0, 0, true, 0);

            // Rafters
            float fValueLoadRafterDead = -0.3f;
            float fValueLoadRafterImposed = -0.1f;
            float fValueLoadRafterSnow1 = -0.8f;
            float fValueLoadRafterSnow2 = -0.4f;

            // Columns
            float fValueLoadColumnWind1PlusX = -0.5f;
            float fValueLoadColumnWind2PlusX = 0.2f;

            float fValueLoadColumnWind1MinusX = -0.2f;
            float fValueLoadColumnWind2MinusX = 0.5f;

            List<CMLoad> memberLoadDeadRafters = new List<CMLoad>();
            List<CMLoad> memberLoadImposedRafters = new List<CMLoad>();
            List<CMLoad> memberMaxLoadSnowAllRafters = new List<CMLoad>();
            List<CMLoad> memberMaxLoadSnowLeftRafters = new List<CMLoad>();
            List<CMLoad> memberMaxLoadSnowRightRafters = new List<CMLoad>();

            List<CMLoad> memberMaxLoadWindColumnPlusX = new List<CMLoad>();
            List<CMLoad> memberMaxLoadWindColumnMinusX = new List<CMLoad>();

            GenerateLoadOnRafters(fValueLoadRafterDead, fValueLoadRafterDead, ref memberLoadDeadRafters);
            GenerateLoadOnRafters(fValueLoadRafterImposed, fValueLoadRafterImposed, ref memberLoadImposedRafters);
            GenerateLoadOnRafters(fValueLoadRafterSnow1, fValueLoadRafterSnow1, ref memberMaxLoadSnowAllRafters);
            GenerateLoadOnRafters(fValueLoadRafterSnow1, fValueLoadRafterSnow2, ref memberMaxLoadSnowLeftRafters);
            GenerateLoadOnRafters(fValueLoadRafterSnow2, fValueLoadRafterSnow1, ref memberMaxLoadSnowRightRafters);

            GenerateLoadOnMainColumns(fValueLoadColumnWind1PlusX, fValueLoadColumnWind2PlusX, ref memberMaxLoadWindColumnPlusX);
            GenerateLoadOnMainColumns(fValueLoadColumnWind1MinusX, fValueLoadColumnWind2MinusX, ref memberMaxLoadWindColumnMinusX);

            // Load Cases
            m_arrLoadCases = new CLoadCase[20];
            m_arrLoadCases[0] = new CLoadCase(1, "Dead Load G", "Permanent load", memberLoadDeadRafters);                                     // 1
            m_arrLoadCases[1] = new CLoadCase(2, "Dead load Gs", "Services and superimposed dead load");                                      // 2
            m_arrLoadCases[2] = new CLoadCase(3, "Imposed load Q", "Imposed load", memberLoadImposedRafters);                                 // 3
            m_arrLoadCases[3] = new CLoadCase(4, "Snow load Su - full", "Snow load", memberMaxLoadSnowAllRafters);                            // 4
            m_arrLoadCases[4] = new CLoadCase(5, "Snow load Su - left", "Snow load", memberMaxLoadSnowLeftRafters);                           // 5
            m_arrLoadCases[5] = new CLoadCase(6, "Snow load Su - right", "Snow load", memberMaxLoadSnowRightRafters);                         // 6
            m_arrLoadCases[6] = new CLoadCase(7, "Wind load Wu - Cpi - Left - X+", "Wind load");                                              // 7
            m_arrLoadCases[7] = new CLoadCase(8, "Wind load Wu - Cpi - Right - X-", "Wind load");                                             // 8
            m_arrLoadCases[8] = new CLoadCase(9, "Wind load Wu - Cpi - Front - Y+", "Wind load");                                             // 9
            m_arrLoadCases[9] = new CLoadCase(10, "Wind load Wu - Cpi - Rear - Y-", "Wind load");                                             // 10
            m_arrLoadCases[10] = new CLoadCase(11, "Wind load Wu - Cpe,min - Left - X+", "Wind load", memberMaxLoadWindColumnPlusX);          // 11
            m_arrLoadCases[11] = new CLoadCase(12, "Wind load Wu - Cpe,min - Right - X-", "Wind load", memberMaxLoadWindColumnMinusX);        // 12
            m_arrLoadCases[12] = new CLoadCase(13, "Wind load Wu - Cpe,min - Front - Y+", "Wind load");        // 13
            m_arrLoadCases[13] = new CLoadCase(14, "Wind load Wu - Cpe,min - Rear - Y-", "Wind load");         // 14
            m_arrLoadCases[14] = new CLoadCase(15, "Wind load Wu - Cpe,max - Left - X+", "Wind load");         // 15
            m_arrLoadCases[15] = new CLoadCase(16, "Wind load Wu - Cpe,max - Right - X-", "Wind load");        // 16
            m_arrLoadCases[16] = new CLoadCase(17, "Wind load Wu - Cpe,max - Front - Y+", "Wind load");        // 17
            m_arrLoadCases[17] = new CLoadCase(18, "Wind load Wu - Cpe,max - Rear - Y-", "Wind load");         // 18
            m_arrLoadCases[18] = new CLoadCase(19, "Earthquake load Eu - X", "Earthquake load");               // 19
            m_arrLoadCases[19] = new CLoadCase(20, "Earthquake load Eu - Y", "Earthquake load");               // 20

            // Load Combinations
            m_arrLoadCombs = new CLoadCombination[6];
            m_arrLoadCombs[0] = new CLoadCombination(1, "CO 1", ELSType.eLS_ULS);
            m_arrLoadCombs[1] = new CLoadCombination(2, "CO 2", ELSType.eLS_ULS);
            m_arrLoadCombs[2] = new CLoadCombination(3, "CO 3", ELSType.eLS_ULS);
            m_arrLoadCombs[3] = new CLoadCombination(4, "CO 4", ELSType.eLS_ULS);
            m_arrLoadCombs[4] = new CLoadCombination(5, "CO 5", ELSType.eLS_SLS);
            m_arrLoadCombs[5] = new CLoadCombination(6, "CO 6", ELSType.eLS_SLS);


            // Create combinations
            //TODO - vytvorit kombinacie podla predpisu v CLoadCombinations

            /*
            Combinations<CLoadCase> a = new Combinations<CLoadCase>(m_arrLoadCases,3, GenerateOption.WithoutRepetition);
            Permutations<CLoadCase> b = new Permutations<CLoadCase>(m_arrLoadCases,GenerateOption.WithoutRepetition);
            */

            // G + Gs
            m_arrLoadCombs[0].LoadCasesList.Add(m_arrLoadCases[0]);
            m_arrLoadCombs[0].LoadCasesFactorsList.Add(1.35f);
            m_arrLoadCombs[0].LoadCasesList.Add(m_arrLoadCases[1]);
            m_arrLoadCombs[0].LoadCasesFactorsList.Add(1.35f);

            // G + Gs + Q
            m_arrLoadCombs[1].LoadCasesList.Add(m_arrLoadCases[0]);
            m_arrLoadCombs[1].LoadCasesFactorsList.Add(1.20f);
            m_arrLoadCombs[1].LoadCasesList.Add(m_arrLoadCases[1]);
            m_arrLoadCombs[1].LoadCasesFactorsList.Add(1.20f);
            m_arrLoadCombs[1].LoadCasesList.Add(m_arrLoadCases[2]);
            m_arrLoadCombs[1].LoadCasesFactorsList.Add(1.50f);

            // G + Cpi front + W Cpe,max  front
            m_arrLoadCombs[2].LoadCasesList.Add(m_arrLoadCases[0]);
            m_arrLoadCombs[2].LoadCasesFactorsList.Add(0.90f);
            m_arrLoadCombs[2].LoadCasesList.Add(m_arrLoadCases[8]);
            m_arrLoadCombs[2].LoadCasesFactorsList.Add(0.70f);
            m_arrLoadCombs[2].LoadCasesList.Add(m_arrLoadCases[16]);
            m_arrLoadCombs[2].LoadCasesFactorsList.Add(1.00f);

            // G + Cpi front + W Cpe,min  front
            m_arrLoadCombs[3].LoadCasesList.Add(m_arrLoadCases[0]);
            m_arrLoadCombs[3].LoadCasesFactorsList.Add(0.90f);
            m_arrLoadCombs[3].LoadCasesList.Add(m_arrLoadCases[8]);
            m_arrLoadCombs[3].LoadCasesFactorsList.Add(0.70f);
            m_arrLoadCombs[3].LoadCasesList.Add(m_arrLoadCases[12]);
            m_arrLoadCombs[3].LoadCasesFactorsList.Add(1.00f);

            // G + Cpi front + W Cpe,max left
            m_arrLoadCombs[4].LoadCasesList.Add(m_arrLoadCases[0]);
            m_arrLoadCombs[4].LoadCasesFactorsList.Add(0.90f);
            m_arrLoadCombs[4].LoadCasesList.Add(m_arrLoadCases[6]);
            m_arrLoadCombs[4].LoadCasesFactorsList.Add(0.70f);
            m_arrLoadCombs[4].LoadCasesList.Add(m_arrLoadCases[10]);
            m_arrLoadCombs[4].LoadCasesFactorsList.Add(1.00f);

            // G + Cpi front + W Cpe,min lef
            m_arrLoadCombs[5].LoadCasesList.Add(m_arrLoadCases[0]);
            m_arrLoadCombs[5].LoadCasesFactorsList.Add(0.90f);
            m_arrLoadCombs[5].LoadCasesList.Add(m_arrLoadCases[6]);
            m_arrLoadCombs[5].LoadCasesFactorsList.Add(0.70f);
            m_arrLoadCombs[5].LoadCasesList.Add(m_arrLoadCases[14]);
            m_arrLoadCombs[5].LoadCasesFactorsList.Add(1.00f);

            // Limit States
            m_arrLimitStates = new CLimitState[2];
            m_arrLimitStates[0] = new CLimitState(ELSType.eLS_ULS);
            m_arrLimitStates[1] = new CLimitState(ELSType.eLS_SLS);
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

            if (MathF.d_equal(node_temp.Y, 0) && !MathF.d_equal(fFrontFrameRakeAngle_temp_rad, 0)) // Front Frame
            {
                node_temp.Y += node_temp.X * (float)Math.Tan(fFrontFrameRakeAngle_temp_rad);
            }

            if (MathF.d_equal(node_temp.Y, fL_tot) && !MathF.d_equal(fBackFrameRakeAngle_temp_rad, 0)) // Back Frame
            {
                node_temp.Y += node_temp.X * (float)Math.Tan(fBackFrameRakeAngle_temp_rad);
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

        public void AddFrontOrBackGirtsMembers(int iFrameNodesNo, int iOneRafterColumnNo, int[] iArrNumberOfNodesPerColumn, int i_temp_numberofNodes, int i_temp_numberofMembers, int iIntermediateColumnNodesForGirtsOneRafterNo, int iIntermediateColumnNodesForGirtsOneFrameNo, int iTempJumpBetweenFrontAndBack_GirtsNumberInLongidutinalDirection, float fDist_Girts, float fGirtStart_MC, float fGirtStart, float fGirtEnd, CCrSc section, float fMemberRotation)
        {
            int iTemp = 0;
            int iTemp2 = 0;
            int iOneColumnGridNo_temp = (int)((fH1_frame - fUpperGirtLimit - fBottomGirtPosition) / fDist_Girt) + 1;

            for (int i = 0; i < iOneRafterColumnNo + 1; i++)
            {
                if (i == 0) // First session depends on number of girts at main frame column
                {
                    for (int j = 0; j < iOneColumnGridNo_temp; j++)
                    {
                        m_arrMembers[i_temp_numberofMembers + j] = new CMember(i_temp_numberofMembers + j + 1, m_arrNodes[iFrameNodesNo * iFrameNo + iTempJumpBetweenFrontAndBack_GirtsNumberInLongidutinalDirection + j], m_arrNodes[i_temp_numberofNodes + j], section, EMemberType_FormSteel.eG, null, null, fGirtStart_MC, fGirtEnd, fMemberRotation, 0);
                    }

                    iTemp += iOneColumnGridNo_temp;
                }
                else if (i < iOneRafterColumnNo) // Other sessions
                {
                    for (int j = 0; j < iArrNumberOfNodesPerColumn[i - 1]; j++)
                    {
                        m_arrMembers[i_temp_numberofMembers + iTemp + j] = new CMember(i_temp_numberofMembers + iTemp + j + 1, m_arrNodes[i_temp_numberofNodes + iTemp2 + j], m_arrNodes[i_temp_numberofNodes + iArrNumberOfNodesPerColumn[i - 1] + iTemp2 + j], section, EMemberType_FormSteel.eG, null, null, fGirtStart, fGirtEnd, fMemberRotation, 0);
                    }

                    iTemp2 += iArrNumberOfNodesPerColumn[i - 1];
                    iTemp += iArrNumberOfNodesPerColumn[i - 1];
                }
                else // Last session - prechadza cez stred budovy
                {
                    for (int j = 0; j < iArrNumberOfNodesPerColumn[i - 1]; j++)
                    {
                        m_arrMembers[i_temp_numberofMembers + iTemp + j] = new CMember(i_temp_numberofMembers + iTemp + j + 1, m_arrNodes[i_temp_numberofNodes + iTemp2 + j], m_arrNodes[i_temp_numberofNodes + iIntermediateColumnNodesForGirtsOneFrameNo - iArrNumberOfNodesPerColumn[iOneRafterColumnNo - 1] + j], section, EMemberType_FormSteel.eG, null, null, fGirtStart, fGirtEnd, fMemberRotation, 0);
                    }

                    iTemp += iArrNumberOfNodesPerColumn[i - 1];
                }
            }

            iTemp = 0;
            iTemp2 = 0;

            for (int i = 0; i < iOneRafterColumnNo; i++)
            {
                int iNumberOfMembers_temp = iOneColumnGridNo_temp + iIntermediateColumnNodesForGirtsOneRafterNo;

                if (i == 0) // First session depends on number of girts at main frame column
                {
                    for (int j = 0; j < iOneColumnGridNo_temp; j++)
                    {
                        m_arrMembers[i_temp_numberofMembers + iNumberOfMembers_temp + j] = new CMember(i_temp_numberofMembers + iNumberOfMembers_temp + j + 1, m_arrNodes[iFrameNodesNo * iFrameNo + iTempJumpBetweenFrontAndBack_GirtsNumberInLongidutinalDirection + iOneColumnGridNo_temp + j], m_arrNodes[i_temp_numberofNodes + iIntermediateColumnNodesForGirtsOneRafterNo + j], section, EMemberType_FormSteel.eG, null, null, fGirtStart_MC, fGirtEnd, -fMemberRotation, 0);
                    }

                    iTemp += iOneColumnGridNo_temp;
                }
                else // Other sessions (not in the middle)
                {
                    for (int j = 0; j < iArrNumberOfNodesPerColumn[i - 1]; j++)
                    {
                        m_arrMembers[i_temp_numberofMembers + iNumberOfMembers_temp + iTemp + j] = new CMember(i_temp_numberofMembers + iNumberOfMembers_temp + iTemp + j + 1, m_arrNodes[i_temp_numberofNodes + iIntermediateColumnNodesForGirtsOneRafterNo + iTemp2 + j], m_arrNodes[i_temp_numberofNodes + iIntermediateColumnNodesForGirtsOneRafterNo + iArrNumberOfNodesPerColumn[i - 1] + iTemp2 + j], section, EMemberType_FormSteel.eG, null, null, fGirtStart, fGirtEnd, -fMemberRotation, 0);
                    }

                    iTemp2 += iArrNumberOfNodesPerColumn[i - 1];
                    iTemp += iArrNumberOfNodesPerColumn[i - 1];
                }
            }
        }

        public void AddDoorBlock(DoorProperties prop, float fLimitDistanceFromColumn)
        {
            // Left side X = 0, Right Side X = GableWidth
            // Insert after frame ID

            // TODO - vyrobit strukturu parametrov pre dvere zadanych v GUI
            /*
            int iSideMultiplier = 0; // 0 lef side X = 0, 1 - right side X = Gable Width
            int iBlockFrame = 1; // (Second Bay, Left side) - User-defined
            float fDoorsHeight = 2.1f; // User-defined
            float fDoorsWidth = 1.1f; // User-defined
            float fDoorCoordinateXinBlock = 1.3f; // User-defined
            float fLimitDistanceFromColumn = 0.5f; // Default but could be also defined by user
            */

            // TODO - prepracovat a pouzivat priamo vstupnu strukturu
            int iSideMultiplier = prop.sBuildingSide == "Left" ? 0 : 1; // 0 lef side X = 0, 1 - right side X = Gable Width
            int iBlockFrame = prop.iBayNumber - 1; // ID of frame in the bay
            float fDoorsHeight = prop.fDoorsHeight;
            float fDoorsWidth = prop.fDoorsWidth;
            float fDoorCoordinateXinBlock = prop.fDoorCoordinateXinBlock;

            int iBayColumn = (iBlockFrame * 6) + (iSideMultiplier == 0 ?  0 : (4-1)); // (2 columns + 2 rafters + 2 eaves purlins) = 6, For Y = GableWidth + 4 number of members in one frame - 1 (index)

            int iFirstMemberToDeactivate = iMainColumnNo + iRafterNo + iEavesPurlinNo + iBlockFrame * iGirtNoInOneFrame + iSideMultiplier * (iGirtNoInOneFrame / 2);

            CMember mReferenceGirt = m_arrMembers[iFirstMemberToDeactivate]; // Deactivated member properties define properties of block girts
            CMember mFrameColumn = m_arrMembers[iBayColumn];

            CBlock door = new CBlock_3D_001_DoorInBay(prop.sBuildingSide, fDoorsHeight, fDoorsWidth, fDoorCoordinateXinBlock, fLimitDistanceFromColumn, fBottomGirtPosition, fDist_Girt, mReferenceGirt, mFrameColumn, fL1_frame);

            CPoint pControlPointBlock = new CPoint(0, iSideMultiplier * fW_frame, iBlockFrame * fL1_frame, 0, 0);
            AddDoorOrWindowBlockProperties(pControlPointBlock, iFirstMemberToDeactivate, door);
        }

        public void AddWindowBlock(WindowProperties prop, float fLimitDistanceFromColumn)
        {
            // Left side X = 0, Right Side X = GableWidth
            // Insert after frame ID

            // TODO - prepracovat a pouzivat priamo vstupnu strukturu
            int iSideMultiplier = prop.sBuildingSide == "Left" ? 0 : 1; // 0 lef side X = 0, 1 - right side X = Gable Width
            int iBlockFrame = prop.iBayNumber - 1; // ID of frame in the bay
            float fWindowHeight = prop.fWindowsHeight;
            float fWindowWidth = prop.fWindowsWidth;
            float fWindowCoordinateXinBlock = prop.fWindowCoordinateXinBay;
            float fWindowCoordinateZinBay = prop.fWindowCoordinateZinBay;
            int iNumberOfWindowColumns = prop.iNumberOfWindowColumns;

            int iBayColumn = (iBlockFrame * 6) + (iSideMultiplier == 0 ? 0 : (4 - 1)); // (2 columns + 2 rafters + 2 eaves purlins) = 6, For Y = GableWidth + 4 number of members in one frame - 1 (index)
            int iFirstGirtInBay = iMainColumnNo + iRafterNo + iEavesPurlinNo + iBlockFrame * iGirtNoInOneFrame + iSideMultiplier * (iGirtNoInOneFrame / 2);

            CMember mReferenceGirt = m_arrMembers[iFirstGirtInBay]; // First girt in bay member properties define properties of block girts
            CMember mFrameColumn = m_arrMembers[iBayColumn];

            CBlock_3D_002_WindowInBay window = new CBlock_3D_002_WindowInBay(prop.sBuildingSide, fWindowHeight, fWindowWidth, fWindowCoordinateXinBlock, fWindowCoordinateZinBay, iNumberOfWindowColumns, fLimitDistanceFromColumn, fBottomGirtPosition, fDist_Girt, mReferenceGirt, mFrameColumn, fL1_frame, fH1_frame);

            CPoint pControlPointBlock = new CPoint(0, iSideMultiplier * fW_frame, iBlockFrame * fL1_frame, 0, 0);
            int iFirstMemberToDeactivate = iFirstGirtInBay + window.iNumberOfGirtsUnderWindow;

            AddDoorOrWindowBlockProperties(pControlPointBlock, iFirstMemberToDeactivate, window);
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

            float fBlockRotationAboutZaxis_rad = MathF.fPI / 2.0f; // Parameter of block - depending on side of building (front, back (0 deg), left, right (90 deg))

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

                if (m_arrCrSc[3].AssignedMembersList.Count > 0) // Check that list is not empty
                {
                    if (m_arrCrSc[3].Equals(m_arrMembers[arraysizeoriginal + i].CrScStart))
                        m_arrCrSc[3].AssignedMembersList.RemoveAt(m_arrCrSc[3].AssignedMembersList.Count - 1);
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
    }
}
