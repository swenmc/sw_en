using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using BaseClasses;
using BaseClasses.GraphObj;
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
        public float fBottomGirtPosition;
        public float fDist_FrontGirts;
        public float fDist_BackGirts;

        int iMainColumnNo;
        int iRafterNo;
        int iEavesPurlinNo;
        int iGirtNoInOneFrame;
        int iPurlinNoInOneFrame;
        int iFrontColumnNoInOneFrame;
        int iBackColumnNoInOneFrame;
        int iFrontGirtsNoInOneFrame;
        int iBackGirtsNoInOneFrame;

        public CExample_3D_901_PF(float fH1_temp, float fW_temp, float fL1_temp, int iFrameNo_temp, float fH2_temp, float fDist_Girt_temp, float fDist_Purlin_temp, float fDist_FrontColumns_temp, float fBottomGirtPosition_temp)
        {
            try
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

                bool bGenerateGirts = true;
                if (bGenerateGirts)
                {
                    iOneColumnGridNo = (int)((fH1_frame - fBottomGirtPosition) / fDist_Girt) + 1;
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
                try
                {
                    if (bGenerateFrontGirts)
                    {
                        iFrontIntermediateColumnNodesForGirtsOneRafterNo = GetNumberofIntermediateNodesInColumnsForOneFrame(iOneRafterFrontColumnNo, fBottomGirtPosition, fDist_FrontColumns);
                        iFrontIntermediateColumnNodesForGirtsOneFrameNo = 2 * iFrontIntermediateColumnNodesForGirtsOneRafterNo;

                        // Number of Girts - Main Frame Column
                        iOneColumnGridNo = (int)((fH1_frame - fBottomGirtPosition) / fDist_Girt) + 1;

                        iFrontGirtsNoInOneFrame = iOneColumnGridNo;

                        // Number of girts under one rafter at the frontside of building - middle girts are considered twice
                        for (int i = 0; i < iOneRafterFrontColumnNo; i++)
                        {
                            int temp = GetNumberofIntermediateNodesInOneColumnForGirts(fBottomGirtPosition, fDist_FrontColumns, i);
                            iFrontGirtsNoInOneFrame += temp;
                            iArrNumberOfNodesPerFrontColumn[i] = temp;
                        }

                        iFrontGirtsNoInOneFrame *= 2;
                        // Girts in the middle are considered twice - remove one set
                        iFrontGirtsNoInOneFrame -= iArrNumberOfNodesPerFrontColumn[iOneRafterFrontColumnNo - 1];
                    }
                }
                catch
                {
                    //tha index outof range exception
                }


                // Number of Nodes - Back Girts
                int iBackIntermediateColumnNodesForGirtsOneRafterNo = 0;
                int iBackIntermediateColumnNodesForGirtsOneFrameNo = 0;
                iBackGirtsNoInOneFrame = 0;
                int[] iArrNumberOfNodesPerBackColumn = new int[iOneRafterBackColumnNo];

                bool bGenerateBackGirts = true;
                try
                {
                    if (bGenerateBackGirts)
                    {
                        iBackIntermediateColumnNodesForGirtsOneRafterNo = GetNumberofIntermediateNodesInColumnsForOneFrame(iOneRafterBackColumnNo, fBottomGirtPosition, fDist_BackColumns);
                        iBackIntermediateColumnNodesForGirtsOneFrameNo = 2 * iBackIntermediateColumnNodesForGirtsOneRafterNo;

                        // Number of Girts - Main Frame Column
                        iOneColumnGridNo = (int)((fH1_frame - fBottomGirtPosition) / fDist_Girt) + 1;

                        iBackGirtsNoInOneFrame = iOneColumnGridNo;

                        // Number of girts under one rafter at the frontside of building - middle girts are considered twice
                        for (int i = 0; i < iOneRafterBackColumnNo; i++)
                        {
                            int temp = GetNumberofIntermediateNodesInOneColumnForGirts(fBottomGirtPosition, fDist_BackColumns, i);
                            iBackGirtsNoInOneFrame += temp;
                            iArrNumberOfNodesPerBackColumn[i] = temp;
                        }

                        iBackGirtsNoInOneFrame *= 2;
                        // Girts in the middle are considered twice - remove one set
                        iBackGirtsNoInOneFrame -= iArrNumberOfNodesPerBackColumn[iOneRafterBackColumnNo - 1];
                    }
                }
                catch
                {

                }


                m_arrNodes = new BaseClasses.CNode[iFrameNodesNo * iFrameNo + iFrameNo * iGirtNoInOneFrame + iFrameNo * iPurlinNoInOneFrame + iFrontColumninOneFrameNodesNo + iBackColumninOneFrameNodesNo + iFrontIntermediateColumnNodesForGirtsOneFrameNo + iBackIntermediateColumnNodesForGirtsOneFrameNo];
                m_arrMembers = new CMember[iMainColumnNo + iRafterNo + iEavesPurlinNo + (iFrameNo - 1) * iGirtNoInOneFrame + (iFrameNo - 1) * iPurlinNoInOneFrame + iFrontColumnNoInOneFrame + iBackColumnNoInOneFrame + iFrontGirtsNoInOneFrame + iBackGirtsNoInOneFrame];
                m_arrMat = new CMat_00[1];
                m_arrCrSc = new CRSC.CCrSc[9];
                m_arrNSupports = new BaseClasses.CNSupport[2 * iFrameNo];

                // Materials
                // Materials List - Materials Array - Fill Data of Materials Array
                m_arrMat[0] = new CMat_03_00();

                // Cross-sections
                // CrSc List - CrSc Array - Fill Data of Cross-sections Array
                m_arrCrSc[0] = new CCrSc_3_63020_BOX(0.63f, 0.2f, 0.00195f, 0.00195f, Colors.Violet); // Main Column
                m_arrCrSc[1] = new CCrSc_3_63020_BOX(0.63f, 0.2f, 0.00195f, 0.00195f, Colors.Green); // Rafter
                m_arrCrSc[2] = new CCrSc_3_50020_C(0.5f, 0.2f, 0.001f, Colors.Thistle);  // Eaves Purlin
                m_arrCrSc[3] = new CCrSc_3_270XX_C(0.27f, 0.10f, 0.001f, Colors.Orange);  // Girt - Wall
                m_arrCrSc[4] = new CCrSc_3_270XX_C(0.27f, 0.10f, 0.00095f, Colors.SlateBlue); // Purlin
                m_arrCrSc[5] = new CCrSc_3_10075_BOX(0.25f, 0.25f, 0.001f, Colors.Beige); // Front Column
                m_arrCrSc[6] = new CCrSc_3_10075_BOX(0.25f, 0.10f, 0.001f, Colors.BlueViolet); // Back Column
                m_arrCrSc[7] = new CCrSc_3_270XX_C(0.27f, 0.10f, 0.001f, Colors.Aquamarine); // Front Girt
                m_arrCrSc[8] = new CCrSc_3_270XX_C(0.27f, 0.10f, 0.00095f, Colors.YellowGreen); // Back Girt

                // Alignments
                float fMainColumnStart = 0.0f;
                float fMainColumnEnd = -0.20f * (float)m_arrCrSc[1].h; // ??? // TODO - dopocitat
                float fRafterStart = -0.50f * (float)m_arrCrSc[0].h;     // TODO - dopocitat
                float fRafterEnd = -0.25f * (float)m_arrCrSc[1].h;       // TODO - Calculate according to h of rafter and roof pitch
                float fEavesPurlinStart = -0.5f * (float)m_arrCrSc[1].b;  // Just in case that cross-section of rafter is symmetric about z-z
                float fEavesPurlinEnd = -0.5f * (float)m_arrCrSc[1].b;   // Just in case that cross-section of rafter is symmetric about z-z
                float fGirtStart = -0.5f * (float)m_arrCrSc[0].b;        // Just in case that cross-section of main column is symmetric about z-z
                float fGirtEnd = -0.5f * (float)m_arrCrSc[0].b;          // Just in case that cross-section of main column is symmetric about z-z
                float fPurlinStart = -0.5f * (float)m_arrCrSc[1].b;      // Just in case that cross-section of rafter is symmetric about z-z
                float fPurlinEnd = -0.5f * (float)m_arrCrSc[1].b;        // Just in case that cross-section of rafter is symmetric about z-z
                float fFrontColumnStart = 0f;
                float fFrontColumnEnd = -0.5f * (float)m_arrCrSc[1].h;   // TODO - Calculate according to h of rafter and roof pitch
                float fBackColumnStart = 0f;
                float fBackColumnEnd = -0.5f * (float)m_arrCrSc[1].h;   // TODO - Calculate according to h of rafter and roof pitch
                float fFrontGirtStart = -0.5f * (float)m_arrCrSc[5].b;   // Just in case that cross-section of column is symmetric about z-z
                float fFrontGirtEnd = -0.5f * (float)m_arrCrSc[5].b;   // Just in case that cross-section of column is symmetric about z-z
                float fBackGirtStart = -0.5f * (float)m_arrCrSc[6].b;   // Just in case that cross-section of column is symmetric about z-z
                float fBackGirtEnd = -0.5f * (float)m_arrCrSc[6].b;   // Just in case that cross-section of column is symmetric about z-z
                float fFrontGirtStart_MC = -0.5f * (float)m_arrCrSc[0].h; // Connection to the main frame column (column symmetrical about y-y)
                float fFrontGirtEnd_MC = -0.5f * (float)m_arrCrSc[0].h;   // Connection to the main frame column (column symmetrical about y-y)
                float fBackGirtStart_MC = -0.5f * (float)m_arrCrSc[0].h;  // Connection to the main frame column (column symmetrical about y-y)
                float fBackGirtEnd_MC = -0.5f * (float)m_arrCrSc[0].h;    // Connection to the main frame column (column symmetrical about y-y)

                float fColumnsRotation = MathF.fPI / 2.0f;
                float fGirtsRotation = MathF.fPI / 2.0f;

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
                    m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * (iFrameNodesNo - 1) + 0] = new CMember(i * (iFrameNodesNo - 1) + 1, m_arrNodes[i * iFrameNodesNo + 0], m_arrNodes[i * iFrameNodesNo + 1], m_arrCrSc[0], fMainColumnStart, fMainColumnEnd, 0f, 0);
                    // Rafters
                    m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * (iFrameNodesNo - 1) + 1] = new CMember(i * (iFrameNodesNo - 1) + 2, m_arrNodes[i * iFrameNodesNo + 1], m_arrNodes[i * iFrameNodesNo + 2], m_arrCrSc[1], fRafterStart, fRafterEnd, 0f, 0);
                    m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * (iFrameNodesNo - 1) + 2] = new CMember(i * (iFrameNodesNo - 1) + 3, m_arrNodes[i * iFrameNodesNo + 2], m_arrNodes[i * iFrameNodesNo + 3], m_arrCrSc[1], fRafterEnd, fRafterStart, 0f, 0);
                    // Main Column
                    m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * (iFrameNodesNo - 1) + 3] = new CMember(i * (iFrameNodesNo - 1) + 4, m_arrNodes[i * iFrameNodesNo + 3], m_arrNodes[i * iFrameNodesNo + 4], m_arrCrSc[0], fMainColumnEnd, fMainColumnStart, 0f, 0);

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
                            m_arrNodes[i_temp_numberofNodes + i * iGirtNoInOneFrame + j] = new CNode(i_temp_numberofNodes + i * iGirtNoInOneFrame + j + 1, 000000, i * fL1_frame, fBottomGirtPosition + j * fDist_Girt, 0);
                        }

                        for (int j = 0; j < iOneColumnGridNo; j++)
                        {
                            m_arrNodes[i_temp_numberofNodes + i * iGirtNoInOneFrame + iOneColumnGridNo + j] = new CNode(i_temp_numberofNodes + i * iGirtNoInOneFrame + iOneColumnGridNo + j + 1, fW_frame, i * fL1_frame, fBottomGirtPosition + j * fDist_Girt, 0);
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
                            m_arrMembers[i_temp_numberofMembers + i * iGirtNoInOneFrame + j] = new CMember(i_temp_numberofMembers + i * iGirtNoInOneFrame + j + 1, m_arrNodes[i_temp_numberofNodes + i * iGirtNoInOneFrame + j], m_arrNodes[i_temp_numberofNodes + (i + 1) * iGirtNoInOneFrame + j], m_arrCrSc[3], fGirtStart, fGirtEnd, fGirtsRotation, 0);
                        }

                        for (int j = 0; j < iOneColumnGridNo; j++)
                        {
                            m_arrMembers[i_temp_numberofMembers + i * iGirtNoInOneFrame + iOneColumnGridNo + j] = new CMember(i_temp_numberofMembers + i * iGirtNoInOneFrame + iOneColumnGridNo + j + 1, m_arrNodes[i_temp_numberofNodes + i * iGirtNoInOneFrame + iOneColumnGridNo + j], m_arrNodes[i_temp_numberofNodes + (i + 1) * iGirtNoInOneFrame + iOneColumnGridNo + j], m_arrCrSc[3], fGirtStart, fGirtEnd, fGirtsRotation, 0);
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
                i_temp_numberofMembers += bGenerateGirts ? (iGirtNoInOneFrame * (iFrameNo - 1)) : 0;
                if (bGeneratePurlins)
                {
                    for (int i = 0; i < (iFrameNo - 1); i++)
                    {
                        for (int j = 0; j < iOneRafterPurlinNo; j++)
                        {
                            m_arrMembers[i_temp_numberofMembers + i * iPurlinNoInOneFrame + j] = new CMember(i_temp_numberofMembers + i * iPurlinNoInOneFrame + j + 1, m_arrNodes[i_temp_numberofNodes + i * iPurlinNoInOneFrame + j], m_arrNodes[i_temp_numberofNodes + (i + 1) * iPurlinNoInOneFrame + j], m_arrCrSc[4], fPurlinStart, fPurlinEnd, -fRoofPitch_rad, 0);
                        }

                        for (int j = 0; j < iOneRafterPurlinNo; j++)
                        {
                            m_arrMembers[i_temp_numberofMembers + i * iPurlinNoInOneFrame + iOneRafterPurlinNo + j] = new CMember(i_temp_numberofMembers + i * iPurlinNoInOneFrame + iOneRafterPurlinNo + j + 1, m_arrNodes[i_temp_numberofNodes + i * iPurlinNoInOneFrame + iOneRafterPurlinNo + j], m_arrNodes[i_temp_numberofNodes + (i + 1) * iPurlinNoInOneFrame + iOneRafterPurlinNo + j], m_arrCrSc[4], fPurlinStart, fPurlinEnd, fRoofPitch_rad, 0);
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
                m_arrConnectionJointsTypes = new CConnectionJointTypes[3];

                // Roof Pitch Joints
                m_arrConnectionJointsTypes[0] = new CConnectionJointTypes(iFrameNo, 2, 0, 0); // Two plates in Joint
                m_arrConnectionJointsTypes[0].m_arrAssignedNodesWithJointType = new CNode[5];
                m_arrConnectionJointsTypes[0] = new CConnectionJoint_A001(m_arrNodes[2], m_arrMembers[1], m_arrMembers[2], fRoofPitch_rad, 2 * (float)m_arrMembers[1].CrScStart.h, 0.003f, true);

                // Create Joints
                m_arrConnectionJointsGroup = new CConnectionJointTypes[3, iFrameNo];


                for (int i = 0; i < iFrameNo; i++)
                {
                    m_arrConnectionJointsGroup[0, i] = new CConnectionJoint_A001(m_arrNodes[i * 5 + 2], m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * 4 + 1], m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * 4 + 2], fRoofPitch_rad, 2 * (float)m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * 4 + 1].CrScStart.h, 0.003f, true);
                }

                m_arrConnectionJointsTypes[1] = new CConnectionJointTypes(iFrameNo, 2, 0, 0); // Two plates in Joint
                m_arrConnectionJointsTypes[1].m_arrAssignedNodesWithJointType = new CNode[5];
                m_arrConnectionJointsTypes[1] = new CConnectionJoint_B001(m_arrNodes[1], m_arrMembers[0], m_arrMembers[1], fRoofPitch_rad, 2 * (float)m_arrMembers[0].CrScStart.h, 2 * (float)m_arrMembers[1].CrScStart.h, 0.003f, true);


                // Create Joints
                for (int i = 0; i < iFrameNo; i++)
                {
                    m_arrConnectionJointsGroup[1, i] = new CConnectionJoint_B001(m_arrNodes[i * 5 + 1], m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * 4], m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * 4 + 1], fRoofPitch_rad, 2 * (float)m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * 4].CrScStart.h, 2 * (float)m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * 4 + 1].CrScStart.h, 0.003f, true);
                }

                m_arrConnectionJointsTypes[2] = new CConnectionJointTypes(iFrameNo, 2, 0, 0); // Two plates in Joint
                m_arrConnectionJointsTypes[2].m_arrAssignedNodesWithJointType = new CNode[5];
                m_arrConnectionJointsTypes[2] = new CConnectionJoint_B001(m_arrNodes[3], m_arrMembers[3], m_arrMembers[2], fRoofPitch_rad, 2 * (float)m_arrMembers[3].CrScStart.h, 2 * (float)m_arrMembers[2].CrScStart.h, 0.003f, true);

                // Create Joints
                for (int i = 0; i < iFrameNo; i++)
                {
                    m_arrConnectionJointsGroup[2, i] = new CConnectionJoint_B001(m_arrNodes[i * 5 + 3], m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * 4 + 3], m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * 4 + 2], fRoofPitch_rad, 2 * (float)m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * 4 + 3].CrScStart.h, 2 * (float)m_arrMembers[(i * iEavesPurlinNoInOneFrame) + i * 4 + 2].CrScStart.h, 0.003f, true);
                }

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
            catch(Exception ex)
            {
               
            }
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

        public void AddColumnsMembers(int i_temp_numberofNodes, int i_temp_numberofMembers, int iOneRafterColumnNo, int iColumnNoInOneFrame, float fColumnAlignmentStart, float fColumnAlignmentEnd, CCrSc section, float fMemberRotation)
        {
            // Members - Columns
            for (int i = 0; i < iOneRafterColumnNo; i++)
            {
                m_arrMembers[i_temp_numberofMembers + i] = new CMember(i_temp_numberofMembers + i + 1, m_arrNodes[i_temp_numberofNodes + i], m_arrNodes[i_temp_numberofNodes + iColumnNoInOneFrame + i], section, fColumnAlignmentStart, fColumnAlignmentEnd, fMemberRotation, 0);
            }

            for (int i = 0; i < iOneRafterColumnNo; i++)
            {
                m_arrMembers[i_temp_numberofMembers + iOneRafterColumnNo + i] = new CMember(i_temp_numberofMembers + iOneRafterColumnNo + i + 1, m_arrNodes[i_temp_numberofNodes + iOneRafterColumnNo + i], m_arrNodes[i_temp_numberofNodes + iColumnNoInOneFrame + iOneRafterColumnNo + i], section, fColumnAlignmentStart, fColumnAlignmentEnd, fMemberRotation, 0);
            }
        }

        public int GetNumberofIntermediateNodesInOneColumnForGirts(float fBottomGirtPosition_temp, float fDistBetweenColumns, int iColumnIndex)
        {
            float fz_gcs_column_temp;
            CalcColumnNodeCoord_Z((iColumnIndex + 1) * fDistBetweenColumns, out fz_gcs_column_temp);
            int iNumber_of_segments = (int)((fz_gcs_column_temp - fBottomGirtPosition_temp) / fDist_Girt);
            return iNumber_of_segments + 1;
        }

        public int GetNumberofIntermediateNodesInColumnsForOneFrame(int iOneRafterColumnNo, float fBottomGirtPosition_temp, float fDistBetweenColumns)
        {
            int iNo_temp = 0;
            for (int i = 0; i < iOneRafterColumnNo; i++)
            {
                iNo_temp += GetNumberofIntermediateNodesInOneColumnForGirts(fBottomGirtPosition_temp, fDistBetweenColumns, i);
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
                }

                iTemp += iArrNumberOfNodesPerColumn[i];
            }
        }

        public void AddFrontOrBackGirtsMembers(int iFrameNodesNo, int iOneRafterColumnNo, int[] iArrNumberOfNodesPerColumn, int i_temp_numberofNodes, int i_temp_numberofMembers, int iIntermediateColumnNodesForGirtsOneRafterNo, int iIntermediateColumnNodesForGirtsOneFrameNo, int iTempJumpBetweenFrontAndBack_GirtsNumberInLongidutinalDirection, float fDist_Girts, float fGirtStart_MC, float fGirtStart, float fGirtEnd, CCrSc section, float fMemberRotation)
        {
            int iTemp = 0;
            int iTemp2 = 0;
            int iOneColumnGridNo_temp = (int)((fH1_frame - fBottomGirtPosition) / fDist_Girt) + 1;

            for (int i = 0; i < iOneRafterColumnNo + 1; i++)
            {
                if (i == 0) // First session depends on number of girts at main frame column
                {
                    for (int j = 0; j < iOneColumnGridNo_temp; j++)
                    {
                        m_arrMembers[i_temp_numberofMembers + j] = new CMember(i_temp_numberofMembers + j + 1, m_arrNodes[iFrameNodesNo * iFrameNo + iTempJumpBetweenFrontAndBack_GirtsNumberInLongidutinalDirection + j], m_arrNodes[i_temp_numberofNodes + j], section, fGirtStart_MC, fGirtEnd, fMemberRotation, 0);
                    }

                    iTemp += iOneColumnGridNo_temp;
                }
                else if (i < iOneRafterColumnNo) // Other sessions
                {
                    for (int j = 0; j < iArrNumberOfNodesPerColumn[i - 1]; j++)
                    {
                        m_arrMembers[i_temp_numberofMembers + iTemp + j] = new CMember(i_temp_numberofMembers + iTemp + j + 1, m_arrNodes[i_temp_numberofNodes + iTemp2 + j], m_arrNodes[i_temp_numberofNodes + iArrNumberOfNodesPerColumn[i - 1] + iTemp2 + j], section, fGirtStart, fGirtEnd, fMemberRotation, 0);
                    }

                    iTemp2 += iArrNumberOfNodesPerColumn[i - 1];
                    iTemp += iArrNumberOfNodesPerColumn[i - 1];
                }
                else // Last session - prechadza cez stred budovy
                {
                    for (int j = 0; j < iArrNumberOfNodesPerColumn[i - 1]; j++)
                    {
                        m_arrMembers[i_temp_numberofMembers + iTemp + j] = new CMember(i_temp_numberofMembers + iTemp + j + 1, m_arrNodes[i_temp_numberofNodes + iTemp2 + j], m_arrNodes[i_temp_numberofNodes + iIntermediateColumnNodesForGirtsOneFrameNo - iArrNumberOfNodesPerColumn[iOneRafterColumnNo - 1] + j], section, fGirtStart, fGirtEnd, fMemberRotation, 0);
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
                        m_arrMembers[i_temp_numberofMembers + iNumberOfMembers_temp + j] = new CMember(i_temp_numberofMembers + iNumberOfMembers_temp + j + 1, m_arrNodes[iFrameNodesNo * iFrameNo + iTempJumpBetweenFrontAndBack_GirtsNumberInLongidutinalDirection + iOneColumnGridNo_temp + j], m_arrNodes[i_temp_numberofNodes + iIntermediateColumnNodesForGirtsOneRafterNo + j], section, fGirtStart_MC, fGirtEnd, -fMemberRotation, 0);
                    }

                    iTemp += iOneColumnGridNo_temp;
                }
                else // Other sessions (not in the middle)
                {
                    for (int j = 0; j < iArrNumberOfNodesPerColumn[i - 1]; j++)
                    {
                        m_arrMembers[i_temp_numberofMembers + iNumberOfMembers_temp + iTemp + j] = new CMember(i_temp_numberofMembers + iNumberOfMembers_temp + iTemp + j + 1, m_arrNodes[i_temp_numberofNodes + iIntermediateColumnNodesForGirtsOneRafterNo + iTemp2 + j], m_arrNodes[i_temp_numberofNodes + iIntermediateColumnNodesForGirtsOneRafterNo + iArrNumberOfNodesPerColumn[i - 1] + iTemp2 + j], section, fGirtStart, fGirtEnd, -fMemberRotation, 0);
                    }

                    iTemp2 += iArrNumberOfNodesPerColumn[i - 1];
                    iTemp += iArrNumberOfNodesPerColumn[i - 1];
                }
            }
        }
    }
}
