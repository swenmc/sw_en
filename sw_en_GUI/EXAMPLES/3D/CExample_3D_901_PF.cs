using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using BaseClasses;
using MATH;
using MATERIAL;
using CRSC;

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
        public float fBottomGirdPosition;
        public float fDist_FrontGirts;
        public float fDist_BackGirts;

        int iMainColumnNo;
        int iRafterNo;
        int iEavesPurlinNo;
        int iGirdNoInOneFrame;
        int iPurlinNoInOneFrame;
        int iFrontColumnNoInOneFrame;
        int iBackColumnNoInOneFrame;
        int iFrontGirtsNoInOneFrame;
        int iBackGirtsNoInOneFrame;

        public CExample_3D_901_PF(float fH1_temp, float fW_temp, float fL1_temp, int iFrameNo_temp, float fH2_temp, float fDist_Gird_temp, float fDist_Purlin_temp, float fDist_FrontColumns_temp, float fBottomGirdPosition_temp)
        {
            // Todo asi prepracovat na zoznam tried objektov

            string[,] componentTypesList = new string[9, 3]
            {
                {"Main Column","2x50020","AS"},
                {"Rafter","2x50020",""},
                {"Eaves Purlin","2x50020",""},
                {"Purlin","2x50020",""},
                {"Purlin Brace","2x50020",""},
                {"Gird","2x50020",""},
                {"Gird Brace","2x50020",""},
                {"Roofing","2x50020",""},
                {"Wall Cladding","2x50020",""}
            };

            fH1_frame = fH1_temp;
            fW_frame = fW_temp;
            fL1_frame = fL1_temp;
            iFrameNo = iFrameNo_temp;
            fH2_frame = fH2_temp;

            fL_tot = (iFrameNo - 1) * fL1_frame;

            fDist_Girt = fDist_Gird_temp;
            fDist_Purlin = fDist_Purlin_temp;
            fDist_FrontColumns = fDist_FrontColumns_temp;
            fBottomGirdPosition = fBottomGirdPosition_temp;
            fDist_FrontGirts = fDist_Gird_temp; // Ak nie je rovnake ako pozdlzne tak su koncove pruty sikmo pretoze sa uvazuje jeden uzol na stlpe pre pozdlzny aj priecny smer nosnikov
            fDist_BackGirts = fDist_Gird_temp;

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
            iGirdNoInOneFrame = 0;

            bool bGenerateGirts = true;
            if (bGenerateGirts)
            {
                iOneColumnGridNo = (int)((fH1_frame - fBottomGirdPosition) / fDist_Girt) + 1;
                iGirdNoInOneFrame = 2 * iOneColumnGridNo;
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
                iFrontIntermediateColumnNodesForGirtsOneRafterNo = GetNumberofIntermediateNodesInColumnsForOneFrame(iOneRafterFrontColumnNo, fBottomGirdPosition, fDist_FrontColumns);
                iFrontIntermediateColumnNodesForGirtsOneFrameNo = 2 * iFrontIntermediateColumnNodesForGirtsOneRafterNo;

                // Number of Girts - Main Frame Column
                iOneColumnGridNo = (int)((fH1_frame - fBottomGirdPosition) / fDist_Girt) + 1;

                iFrontGirtsNoInOneFrame = iOneColumnGridNo;

                // Number of girts under one rafter at the frontside of building - middle girts are considered twice
                for (int i = 0; i < iOneRafterFrontColumnNo; i++)
                {
                    int temp = GetNumberofIntermediateNodesInOneColumnForGirts(fBottomGirdPosition, fDist_FrontColumns, i);
                    iFrontGirtsNoInOneFrame += temp;
                    iArrNumberOfNodesPerFrontColumn[i] = temp;
                }

                iFrontGirtsNoInOneFrame *= 2;
                // Girts in the middle are considered twice - remove one set
                iFrontGirtsNoInOneFrame -= iArrNumberOfNodesPerFrontColumn[iOneRafterFrontColumnNo-1];
            }

            // Number of Nodes - Back Girts
            int iBackIntermediateColumnNodesForGirtsOneRafterNo = 0;
            int iBackIntermediateColumnNodesForGirtsOneFrameNo = 0;
            iBackGirtsNoInOneFrame = 0;

            bool bGenerateBackGirts = false;
            if (bGenerateBackGirts)
            {
                iBackIntermediateColumnNodesForGirtsOneRafterNo = GetNumberofIntermediateNodesInColumnsForOneFrame(iOneRafterBackColumnNo, fBottomGirdPosition, fDist_BackColumns);
                iBackIntermediateColumnNodesForGirtsOneFrameNo = 2 * iBackIntermediateColumnNodesForGirtsOneRafterNo;

                // Number of Girts - Main Frame Column
                iOneColumnGridNo = (int)((fH1_frame - fBottomGirdPosition) / fDist_Girt) + 1;
            }

            // Alligments
            float fMainColumnStart = 0f;
            float fMainColumnEnd = 0f;
            float fRafterStart = 0f;
            float fRafterEnd = 0f;
            float fEavesPurlinStart = 0f;
            float fEavesPurlinEnd = 0f;
            float fGirtStart = 0f;
            float fGirtEnd = 0f;
            float fPurlinStart = 0f;
            float fPurlinEnd = 0f;
            float fFrontColumnStart = 0f;
            float fFrontColumnEnd = 0f;
            float fBackColumnStart = 0f;
            float fBackColumnEnd = 0f;
            float fFrontGirtStart = 0f;
            float fFrontGirtEnd = 0f;
            float fBackGirtStart = 0f;
            float fBackGirtEnd = 0f;
            float fFrontGirtStart_MC = 0f; // Connection to the main frame column
            float fFrontGirtEnd_MC = 0f; // Connection to the main frame column
            float fBackGirtStart_MC = 0f; // Connection to the main frame column
            float fBackGirtEnd_MC = 0f; // Connection to the main frame column


            m_arrNodes = new BaseClasses.CNode[iFrameNodesNo * iFrameNo + iFrameNo * iGirdNoInOneFrame + iFrameNo * iPurlinNoInOneFrame + iFrontColumninOneFrameNodesNo + iBackColumninOneFrameNodesNo + iFrontIntermediateColumnNodesForGirtsOneFrameNo];
            m_arrMembers = new CMember[iMainColumnNo + iRafterNo + iEavesPurlinNo + (iFrameNo - 1) * iGirdNoInOneFrame + (iFrameNo - 1) * iPurlinNoInOneFrame + iFrontColumnNoInOneFrame + iBackColumnNoInOneFrame + iFrontGirtsNoInOneFrame];
            m_arrMat = new CMat_00[1];
            m_arrCrSc = new CRSC.CCrSc[8];
            m_arrNSupports = new BaseClasses.CNSupport[2 * iFrameNo];

            // Materials
            // Materials List - Materials Array - Fill Data of Materials Array
            m_arrMat[0] = new CMat_03_00();

            // Cross-sections
            // CrSc List - CrSc Array - Fill Data of Cross-sections Array
            m_arrCrSc[0] = new CCrSc_3_51_BOX_TEMP(0.5f, 0.2f, 0.002f, Colors.Violet); // Main Column
            m_arrCrSc[1] = new CCrSc_3_51_BOX_TEMP(0.4f, 0.2f, 0.00115f, Colors.Green); // Rafter
            m_arrCrSc[2] = new CCrSc_3_51_C_LIP2_FS50020(0.5f, 0.1f, 0.02f, 0.05f, 0.01f, Colors.Thistle);  // Eaves Purlin
            m_arrCrSc[3] = new CCrSc_3_51_C_LIP2_FS50020(0.3f, 0.08f, 0.02f, 0.05f, 0.01f, Colors.Orange);  // Girt Purlin
            m_arrCrSc[4] = new CCrSc_3_51_C_LIP2_FS50020(0.35f, 0.10f, 0.015f, 0.065f, 0.001f, Colors.SlateBlue); // Purlin
            m_arrCrSc[5] = new CCrSc_3_51_C_LIP2_FS50020(0.45f, 0.10f, 0.015f, 0.065f, 0.001f, Colors.Beige); // Front Column
            m_arrCrSc[6] = new CCrSc_3_51_C_LIP2_FS50020(0.45f, 0.10f, 0.015f, 0.065f, 0.001f, Colors.Beige); // Back Column
            m_arrCrSc[7] = new CCrSc_3_51_C_LIP2_FS50020(0.25f, 0.08f, 0.015f, 0.065f, 0.001f, Colors.Aquamarine); // Front Girt

            // Nodes Automatic Generation
            // Nodes List - Nodes Array

            // Nodes - Frames
            for (int i = 0; i < iFrameNo; i++)
            {
                m_arrNodes[i * iFrameNodesNo + 0] = new CNode(i * iFrameNodesNo + 1, 000000, i * fL1_frame, 00000, 0);
                m_arrNodes[i * iFrameNodesNo + 1] = new CNode(i * iFrameNodesNo + 2, 000000, i * fL1_frame, fH1_frame, 0);
                m_arrNodes[i * iFrameNodesNo + 2] = new CNode(i * iFrameNodesNo + 3, 0.5f * fW_frame, i * fL1_frame, fH2_frame, 0);
                m_arrNodes[i * iFrameNodesNo + 3] = new CNode(i * iFrameNodesNo + 4, fW_frame, i * fL1_frame, fH1_frame, 0);
                m_arrNodes[i * iFrameNodesNo + 4] = new CNode(i * iFrameNodesNo + 5, fW_frame, i * fL1_frame, 00000, 0);
            }

            // Members
            for (int i = 0; i < iFrameNo; i++)
            {
                // Main Column
                m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * (iFrameNodesNo - 1) + 0] = new CMember(i * (iFrameNodesNo - 1) + 1, m_arrNodes[i * iFrameNodesNo + 0], m_arrNodes[i * iFrameNodesNo + 1], m_arrCrSc[0], fMainColumnStart, -fMainColumnEnd, 0f, 0);
                // Rafters
                m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * (iFrameNodesNo - 1) + 1] = new CMember(i * (iFrameNodesNo - 1) + 2, m_arrNodes[i * iFrameNodesNo + 1], m_arrNodes[i * iFrameNodesNo + 2], m_arrCrSc[1], fRafterStart, fRafterEnd, 0f, 0);
                m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * (iFrameNodesNo - 1) + 2] = new CMember(i * (iFrameNodesNo - 1) + 3, m_arrNodes[i * iFrameNodesNo + 2], m_arrNodes[i * iFrameNodesNo + 3], m_arrCrSc[1], fRafterStart, fRafterEnd, 0f, 0);
                // Main Column
                m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * (iFrameNodesNo - 1) + 3] = new CMember(i * (iFrameNodesNo - 1) + 4, m_arrNodes[i * iFrameNodesNo + 3], m_arrNodes[i * iFrameNodesNo + 4], m_arrCrSc[0], fMainColumnStart, fMainColumnEnd, 0f, 0);

                // Eaves Purlins
                if (i < (iFrameNo - 1))
                {
                    m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * (iFrameNodesNo - 1) + 4] = new CMember((i * iEavesPurlinNoInOneFrame) + i * (iFrameNodesNo - 1) + 5, m_arrNodes[i * iFrameNodesNo + 1], m_arrNodes[(i + 1) * iFrameNodesNo + 1], m_arrCrSc[2], fEavesPurlinStart, fEavesPurlinEnd, 0f, 0);
                    m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * (iFrameNodesNo - 1) + 5] = new CMember((i * iEavesPurlinNoInOneFrame) + i * (iFrameNodesNo - 1) + 6, m_arrNodes[i * iFrameNodesNo + 3], m_arrNodes[(i + 1) * iFrameNodesNo + 3], m_arrCrSc[2], fEavesPurlinStart, fEavesPurlinEnd, 0f, 0);
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
                        m_arrNodes[i_temp_numberofNodes + i * iGirdNoInOneFrame + j] = new CNode(i_temp_numberofNodes + i * iGirdNoInOneFrame + j + 1, 000000, i * fL1_frame, fBottomGirdPosition + j * fDist_Girt, 0);
                    }

                    for (int j = 0; j < iOneColumnGridNo; j++)
                    {
                        m_arrNodes[i_temp_numberofNodes + i * iGirdNoInOneFrame + iOneColumnGridNo + j] = new CNode(i_temp_numberofNodes + i * iGirdNoInOneFrame + iOneColumnGridNo + j + 1, fW_frame, i * fL1_frame, fBottomGirdPosition + j * fDist_Girt, 0);
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
                        m_arrMembers[i_temp_numberofMembers + i * iGirdNoInOneFrame + j] = new CMember(i_temp_numberofMembers + i * iGirdNoInOneFrame + j + 1, m_arrNodes[i_temp_numberofNodes + i * iGirdNoInOneFrame + j], m_arrNodes[i_temp_numberofNodes + (i + 1) * iGirdNoInOneFrame + j], m_arrCrSc[3], fGirtStart, fGirtEnd, 0f, 0);
                    }

                    for (int j = 0; j < iOneColumnGridNo; j++)
                    {
                        m_arrMembers[i_temp_numberofMembers + i * iGirdNoInOneFrame + iOneColumnGridNo + j] = new CMember(i_temp_numberofMembers + i * iGirdNoInOneFrame + iOneColumnGridNo + j + 1, m_arrNodes[i_temp_numberofNodes + i * iGirdNoInOneFrame + iOneColumnGridNo + j], m_arrNodes[i_temp_numberofNodes + (i + 1) * iGirdNoInOneFrame + iOneColumnGridNo + j], m_arrCrSc[3], fGirtStart, fGirtEnd, 0f, 0);
                    }
                }
            }

            // Nodes - Purlins
            i_temp_numberofNodes += bGenerateGirts ? (iGirdNoInOneFrame * iFrameNo) : 0;
            if (bGeneratePurlins)
            {
                for (int i = 0; i < iFrameNo; i++)
                {
                    for (int j = 0; j < iOneRafterPurlinNo; j++)
                    {
                        float x_glob, z_glob;
                        CalcPurlinNodeCoord(fFirstPurlinPosition + j * fDist_Purlin, out x_glob, out z_glob);

                        m_arrNodes[i_temp_numberofNodes + i * iPurlinNoInOneFrame + j] = new CNode(i_temp_numberofNodes + i * iPurlinNoInOneFrame + j + 1, x_glob, i * fL1_frame, z_glob, 0);
                    }

                    for (int j = 0; j < iOneRafterPurlinNo; j++)
                    {
                        float x_glob, z_glob;
                        CalcPurlinNodeCoord(fFirstPurlinPosition + j * fDist_Purlin, out x_glob, out z_glob);

                        m_arrNodes[i_temp_numberofNodes + i * iPurlinNoInOneFrame + iOneRafterPurlinNo + j] = new CNode(i_temp_numberofNodes + i * iPurlinNoInOneFrame + iOneRafterPurlinNo + j + 1, fW_frame - x_glob, i * fL1_frame, z_glob, 0);
                    }
                }
            }

            // Members - Purlins
            i_temp_numberofMembers += bGenerateGirts ? (iGirdNoInOneFrame * (iFrameNo - 1)) : 0;
            if (bGeneratePurlins)
            {
                for (int i = 0; i < (iFrameNo - 1); i++)
                {
                    for (int j = 0; j < iOneRafterPurlinNo; j++)
                    {
                        m_arrMembers[i_temp_numberofMembers + i * iPurlinNoInOneFrame + j] = new CMember(i_temp_numberofMembers + i * iPurlinNoInOneFrame + j + 1, m_arrNodes[i_temp_numberofNodes + i * iPurlinNoInOneFrame + j], m_arrNodes[i_temp_numberofNodes + (i + 1) * iPurlinNoInOneFrame + j], m_arrCrSc[4], fPurlinStart, fPurlinEnd, 0f, 0);
                    }

                    for (int j = 0; j < iOneRafterPurlinNo; j++)
                    {
                        m_arrMembers[i_temp_numberofMembers + i * iPurlinNoInOneFrame + iOneRafterPurlinNo + j] = new CMember(i_temp_numberofMembers + i * iPurlinNoInOneFrame + iOneRafterPurlinNo + j + 1, m_arrNodes[i_temp_numberofNodes + i * iPurlinNoInOneFrame + iOneRafterPurlinNo + j], m_arrNodes[i_temp_numberofNodes + (i + 1) * iPurlinNoInOneFrame + iOneRafterPurlinNo + j], m_arrCrSc[4], fPurlinStart, fPurlinEnd, 0f, 0);
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
                AddColumnsMembers(i_temp_numberofNodes, i_temp_numberofMembers, iOneRafterBackColumnNo, iFrontColumnNoInOneFrame, fFrontColumnStart, fFrontColumnEnd, m_arrCrSc[5]);
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
                AddColumnsMembers(i_temp_numberofNodes, i_temp_numberofMembers, iOneRafterBackColumnNo, iBackColumnNoInOneFrame, fBackColumnStart, fBackColumnEnd, m_arrCrSc[6]);
            }

            // Front Girts Columns
            // Nodes - Front Girts
            i_temp_numberofNodes += bGenerateBackColumns ? iBackColumninOneFrameNodesNo : 0;
            if (bGenerateFrontGirts)
            {
                int iTemp = 0;

                for (int i = 0; i < iOneRafterFrontColumnNo; i++)
                {
                    float z_glob;
                    CalcColumnNodeCoord_Z((i + 1) * fDist_FrontColumns, out z_glob);

                    for (int j = 0; j < iArrNumberOfNodesPerFrontColumn[i]; j++)
                    {
                        m_arrNodes[i_temp_numberofNodes + iTemp + j] = new CNode(i_temp_numberofNodes + iTemp + j, (i + 1) * fDist_FrontColumns, 0, fBottomGirdPosition + j * fDist_FrontGirts, 0);
                    }

                    iTemp += iArrNumberOfNodesPerFrontColumn[i];
                }

                iTemp = 0;

                for (int i = 0; i < iOneRafterFrontColumnNo; i++)
                {
                    float z_glob;
                    CalcColumnNodeCoord_Z((i + 1) * fDist_FrontColumns, out z_glob);

                    for (int j = 0; j < iArrNumberOfNodesPerFrontColumn[i]; j++)
                    {
                        m_arrNodes[i_temp_numberofNodes + iFrontIntermediateColumnNodesForGirtsOneRafterNo + iTemp + j] = new CNode(i_temp_numberofNodes + iFrontIntermediateColumnNodesForGirtsOneRafterNo + iTemp + j + 1, fW_frame - (i + 1) * fDist_FrontColumns, 0, fBottomGirdPosition + j * fDist_FrontGirts, 0);
                    }

                    iTemp += iArrNumberOfNodesPerFrontColumn[i];
                }
            }

            // Front Girts
            // Members - Front Girts
            // TODO - doplnit riesenie pre maly rozpon ked neexistuju mezilahle stlpiky, prepojenie mezi hlavnymi stplmi ramu na celu sirku budovy
            // TODO - toto riesenie plati len ak existuju girts v pozdlznom smere, ak budu deaktivovane a nevytvoria sa uzly na stlpoch tak sa musia pruty na celnych stenach generovat uplne inak, musia sa vygenerovat aj uzly na stlpoch ....

            i_temp_numberofMembers += bGenerateBackColumns ? iBackColumnNoInOneFrame : 0;
            if (bGenerateFrontGirts)
            {
                int iTemp = 0;
                int iTemp2 = 0;
                int iOneColumnGridNo_temp = (int)((fH1_frame - fBottomGirdPosition) / fDist_Girt) + 1;

                for (int i = 0; i < iOneRafterFrontColumnNo + 1; i++)
                {
                    if (i == 0) // First session depends on number of girts at main frame column
                    {
                        for (int j = 0; j < iOneColumnGridNo_temp; j++)
                        {
                            m_arrMembers[i_temp_numberofMembers + j] = new CMember(i_temp_numberofMembers + j + 1, m_arrNodes[iFrameNodesNo * iFrameNo + j], m_arrNodes[i_temp_numberofNodes + j], m_arrCrSc[7], fFrontGirtStart_MC, fFrontGirtEnd, 0f, 0);
                        }

                        iTemp += iOneColumnGridNo_temp;
                    }
                    else if (i < iOneRafterFrontColumnNo) // Other sessions
                    {
                        for (int j = 0; j < iArrNumberOfNodesPerFrontColumn[i-1]; j++)
                        {
                            m_arrMembers[i_temp_numberofMembers + iTemp + j] = new CMember(i_temp_numberofMembers + iTemp + j + 1, m_arrNodes[i_temp_numberofNodes + iTemp2 + j], m_arrNodes[i_temp_numberofNodes + iArrNumberOfNodesPerFrontColumn[i-1] + iTemp2 + j], m_arrCrSc[7], fFrontGirtStart, fFrontGirtEnd, 0f, 0);
                        }

                        iTemp2 += iArrNumberOfNodesPerFrontColumn[i - 1];
                        iTemp += iArrNumberOfNodesPerFrontColumn[i-1];
                    }
                    else // Last session - prechadza cez stred budovy
                    {
                        for (int j = 0; j < iArrNumberOfNodesPerFrontColumn[i-1]; j++)
                        {
                            m_arrMembers[i_temp_numberofMembers + iTemp + j] = new CMember(i_temp_numberofMembers + iTemp + j + 1, m_arrNodes[i_temp_numberofNodes + iTemp2 + j], m_arrNodes[i_temp_numberofNodes + iFrontIntermediateColumnNodesForGirtsOneFrameNo - iArrNumberOfNodesPerFrontColumn[iOneRafterFrontColumnNo - 1] + j], m_arrCrSc[7], fFrontGirtStart, fFrontGirtEnd, 0f, 0);
                        }

                        iTemp += iArrNumberOfNodesPerFrontColumn[i-1];
                    }
                }

                iTemp = 0;
                iTemp2 = 0;

                for (int i = 0; i < iOneRafterFrontColumnNo; i++)
                {
                    int iNumberOfMembers_temp = iOneColumnGridNo_temp + iFrontIntermediateColumnNodesForGirtsOneRafterNo;

                    if (i == 0) // First session depends on number of girts at main frame column
                    {
                        for (int j = 0; j < iOneColumnGridNo_temp; j++)
                        {
                            m_arrMembers[i_temp_numberofMembers + iNumberOfMembers_temp + j] = new CMember(i_temp_numberofMembers + iNumberOfMembers_temp + j + 1, m_arrNodes[iFrameNodesNo * iFrameNo + iOneColumnGridNo_temp + j], m_arrNodes[i_temp_numberofNodes + iFrontIntermediateColumnNodesForGirtsOneRafterNo + j], m_arrCrSc[7], fFrontGirtStart_MC, fFrontGirtEnd, 0f, 0);
                        }

                        iTemp += iOneColumnGridNo_temp;
                    }
                    else // Other sessions (not in the middle)
                    {
                        for (int j = 0; j < iArrNumberOfNodesPerFrontColumn[i-1]; j++)
                        {
                            m_arrMembers[i_temp_numberofMembers + iNumberOfMembers_temp + iTemp + j] = new CMember(i_temp_numberofMembers + iNumberOfMembers_temp + iTemp + j + 1, m_arrNodes[i_temp_numberofNodes + iFrontIntermediateColumnNodesForGirtsOneRafterNo + iTemp2 + j], m_arrNodes[i_temp_numberofNodes + iFrontIntermediateColumnNodesForGirtsOneRafterNo + iArrNumberOfNodesPerFrontColumn[i-1] + iTemp2 + j], m_arrCrSc[7], fFrontGirtStart, fFrontGirtEnd, 0f, 0);
                        }

                        iTemp2 += iArrNumberOfNodesPerFrontColumn[i - 1];
                        iTemp += iArrNumberOfNodesPerFrontColumn[i-1];
                    }
                }
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

            // Loading

            m_arrLimitStates = new CLimitState[2];
            m_arrLimitStates[0] = new CLimitState(ELSType.eLS_ULS);
            m_arrLimitStates[1] = new CLimitState(ELSType.eLS_SLS);

            m_arrLoadCombs = new CLoadCombination[6];
            m_arrLoadCombs[0] = new CLoadCombination(ELSType.eLS_ULS);
            m_arrLoadCombs[1] = new CLoadCombination(ELSType.eLS_ULS);
            m_arrLoadCombs[2] = new CLoadCombination(ELSType.eLS_ULS);
            m_arrLoadCombs[3] = new CLoadCombination(ELSType.eLS_ULS);
            m_arrLoadCombs[4] = new CLoadCombination(ELSType.eLS_SLS);
            m_arrLoadCombs[5] = new CLoadCombination(ELSType.eLS_SLS);

            m_arrLoadCases = new CLoadCase[6];
            m_arrLoadCases[0] = new CLoadCase();
            m_arrLoadCases[1] = new CLoadCase();
            m_arrLoadCases[2] = new CLoadCase();
            m_arrLoadCases[3] = new CLoadCase();
            m_arrLoadCases[4] = new CLoadCase();
            m_arrLoadCases[5] = new CLoadCase();

            m_arrLimitStates = new CLimitState[2];
            m_arrLimitStates[0] = new CLimitState(ELSType.eLS_ULS);
            m_arrLimitStates[1] = new CLimitState(ELSType.eLS_SLS);

            m_arrNLoads = new BaseClasses.CNLoad[3];
            m_arrNLoads[0] = new CNLoadAll(m_arrNodes[1], 0, 0, -4f, 0, 0, 0, true, 0);
            m_arrNLoads[1] = new CNLoadAll(m_arrNodes[2], 0, 0, -4f, 0, 0, 0, true, 0);
            m_arrNLoads[2] = new CNLoadAll(m_arrNodes[3], 0, 0, -4f, 0, 0, 0, true, 0);

            m_arrMLoads = new BaseClasses.CMLoad[2];
            m_arrMLoads[0] = new CMLoad_21(-0.8f, m_arrMembers[1], EMLoadTypeDistr.eMLT_FS_G_11, EMLoadType.eMLT_F, EMLoadDirPCC1.eMLD_PCC_FZV_MYU, true, 0);
            m_arrMLoads[1] = new CMLoad_21(-0.9f, m_arrMembers[2], EMLoadTypeDistr.eMLT_FS_G_11, EMLoadType.eMLT_F, EMLoadDirPCC1.eMLD_PCC_FZV_MYU, true, 0);
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

        public void AddColumnsNodes(int i_temp_numberofNodes, int i_temp_numberofMembers, int iOneRafterColumnNo, int iColumnNoInOneFrame, float fDist_Columns, float fy_Global_Coord)
        {
            float z_glob;

            // Bottom nodes
            for (int i = 0; i < iOneRafterColumnNo; i++)
            {
                CalcColumnNodeCoord_Z((i + 1) * fDist_Columns, out z_glob);
                m_arrNodes[i_temp_numberofNodes + i] = new CNode(i_temp_numberofNodes + i + 1, (i + 1) * fDist_Columns, fy_Global_Coord, 0, 0);
            }

            // Bottom nodes
            for (int i = 0; i < iOneRafterColumnNo; i++)
            {
                CalcColumnNodeCoord_Z((i + 1) * fDist_Columns, out z_glob);
                m_arrNodes[i_temp_numberofNodes + iOneRafterColumnNo + i] = new CNode(i_temp_numberofNodes + iOneRafterColumnNo + i + 1, fW_frame - ((i + 1) * fDist_Columns), fy_Global_Coord, 0, 0);
            }

            // Top nodes
            for (int i = 0; i < iOneRafterColumnNo; i++)
            {
                CalcColumnNodeCoord_Z((i + 1) * fDist_Columns, out z_glob);
                m_arrNodes[i_temp_numberofNodes + iColumnNoInOneFrame + i] = new CNode(i_temp_numberofNodes + 2 * iOneRafterColumnNo + i + 1, (i + 1) * fDist_Columns, fy_Global_Coord, z_glob, 0);
            }

            // Top nodes
            for (int i = 0; i < iOneRafterColumnNo; i++)
            {
                CalcColumnNodeCoord_Z((i + 1) * fDist_Columns, out z_glob);
                m_arrNodes[i_temp_numberofNodes + iColumnNoInOneFrame + iOneRafterColumnNo + i] = new CNode(i_temp_numberofNodes + 3 * iOneRafterColumnNo + i + 2, fW_frame - ((i + 1) * fDist_Columns), fy_Global_Coord, z_glob, 0);
            }
        }

        public void AddColumnsMembers(int i_temp_numberofNodes, int i_temp_numberofMembers, int iOneRafterColumnNo, int iColumnNoInOneFrame, float fColumnAlignmentStart, float fColumnAlignmentEnd, CCrSc section)
        {
            // Members - Columns
            for (int i = 0; i < iOneRafterColumnNo; i++)
            {
                m_arrMembers[i_temp_numberofMembers + i] = new CMember(i_temp_numberofMembers + i + 1, m_arrNodes[i_temp_numberofNodes + i], m_arrNodes[i_temp_numberofNodes + iColumnNoInOneFrame + i], section, fColumnAlignmentStart, fColumnAlignmentEnd, 0f, 0);
            }

            for (int i = 0; i < iOneRafterColumnNo; i++)
            {
                m_arrMembers[i_temp_numberofMembers + iOneRafterColumnNo + i] = new CMember(i_temp_numberofMembers + iOneRafterColumnNo + i + 1, m_arrNodes[i_temp_numberofNodes + iOneRafterColumnNo + i], m_arrNodes[i_temp_numberofNodes + iColumnNoInOneFrame + iOneRafterColumnNo + i], section, fColumnAlignmentStart, fColumnAlignmentEnd, 0f, 0);
            }
        }

        public int GetNumberofIntermediateNodesInOneColumnForGirts(float fBottomGirdPosition_temp, float fDistBetweenColumns, int iColumnIndex)
        {
            float fz_gcs_column_temp;
            CalcColumnNodeCoord_Z((iColumnIndex + 1) * fDistBetweenColumns, out fz_gcs_column_temp);
            int iNumber_of_segments = (int)((fz_gcs_column_temp - fBottomGirdPosition_temp) / fDist_Girt);
            return iNumber_of_segments + 1;
        }

        public int GetNumberofIntermediateNodesInColumnsForOneFrame(int iOneRafterColumnNo, float fBottomGirdPosition_temp, float fDistBetweenColumns)
        {
            int iNo_temp = 0;
            for (int i = 0; i < iOneRafterColumnNo; i++)
            {
                iNo_temp += GetNumberofIntermediateNodesInOneColumnForGirts(fBottomGirdPosition_temp, fDistBetweenColumns, i);
            }

            return iNo_temp;
        }
    }
}
