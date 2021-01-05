using BaseClasses;
using CRSC;
using MATERIAL;
using System;
using System.Collections.Generic;

namespace Examples
{
    public class CExample_2D_14_PF : CExample
    {
        public CExample_2D_14_PF(CMat material, CCrSc crscColumn, CCrSc crscRafter, float fB, float fH1, float fH2, float fLoadColumn_q1, List<CMLoad> loadRafter1, List<CMLoad> loadRafter2, float fLoadColumn_q2)
        {
            m_eSLN = ESLN.e2DD_1D; // 1D members in 2D model
            m_eNDOF = (int)ENDOF.e2DEnv; // DOF in 2D
            m_eGCS = EGCS.eGCSLeftHanded; // Global coordinate system

            m_arrNodes = new CNode[5];
            m_arrMembers = new CMember[4];
            m_arrMat = new Dictionary<EMemberType_FS_Position, CMat>();
            m_arrCrSc = new Dictionary<EMemberType_FS_Position, CCrSc>(); // new CCrSc[2];
            m_arrNSupports = new CNSupport[1];
            m_arrMLoads = new CMLoad[1 + loadRafter1.Count + loadRafter2.Count + 1];
            m_arrLoadCases = new CLoadCase[1];
            m_arrLoadCombs = new CLoadCombination[1];

            // Materials
            // Materials List - Materials Array - Fill Data of Materials Array
            m_arrMat[0] = material;

            // Cross-sections
            // CrSc List - CrSc Array - Fill Data of Cross-sections Array
            // Cross-section
            m_arrCrSc[0] = crscColumn;
            m_arrCrSc[0].m_Mat = m_arrMat[0]; // Set CrSc Material

            m_arrCrSc[(EMemberType_FS_Position)1] = crscRafter;
            m_arrCrSc[(EMemberType_FS_Position)1].m_Mat = m_arrMat[0]; // Set CrSc Material

            // Nodes
            // Nodes List - Nodes Array

            // RAM DEFINOVANY V XZ

            // Node 1
            m_arrNodes[0] = new CNode();
            m_arrNodes[0].ID = 1;
            m_arrNodes[0].X = 0f;
            m_arrNodes[0].Y = 0f;
            m_arrNodes[0].Z = 0f;

            // Node 2
            m_arrNodes[1] = new CNode();
            m_arrNodes[1].ID = 2;
            m_arrNodes[1].X = 0f;
            m_arrNodes[1].Y = 0f;
            m_arrNodes[1].Z = fH1;

            // Node 3
            m_arrNodes[2] = new CNode();
            m_arrNodes[2].ID = 3;
            m_arrNodes[2].X = 0.5f * fB;
            m_arrNodes[2].Y = 0;
            m_arrNodes[2].Z = fH2;

            // Node 4
            m_arrNodes[3] = new CNode();
            m_arrNodes[3].ID = 4;
            m_arrNodes[3].X = fB;
            m_arrNodes[3].Y = 0f;
            m_arrNodes[3].Z = fH1;

            // Node 5
            m_arrNodes[4] = new CNode();
            m_arrNodes[4].ID = 5;
            m_arrNodes[4].X = fB;
            m_arrNodes[4].Y = 0f;
            m_arrNodes[4].Z = 0f;

            // Members
            // Members List - Members Array

            // Member 1 - 1-2
            m_arrMembers[0] = new CMember();
            m_arrMembers[0].ID = 1;
            m_arrMembers[0].NodeStart = m_arrNodes[0];
            m_arrMembers[0].NodeEnd = m_arrNodes[1];
            m_arrMembers[0].CrScStart = m_arrCrSc[0];
            m_arrMembers[0].Fill_Basic();

            // Member 2 - 2-3
            m_arrMembers[1] = new CMember();
            m_arrMembers[1].ID = 2;
            m_arrMembers[1].NodeStart = m_arrNodes[1];
            m_arrMembers[1].NodeEnd = m_arrNodes[2];
            m_arrMembers[1].CrScStart = m_arrCrSc[(EMemberType_FS_Position)1];
            m_arrMembers[1].Fill_Basic();

            // Member 3 - 3-4
            m_arrMembers[2] = new CMember();
            m_arrMembers[2].ID = 3;
            m_arrMembers[2].NodeStart = m_arrNodes[2];
            m_arrMembers[2].NodeEnd = m_arrNodes[3];
            m_arrMembers[2].CrScStart = m_arrCrSc[(EMemberType_FS_Position)1];
            m_arrMembers[2].Fill_Basic();

            // Member 4 - 4-5
            m_arrMembers[3] = new CMember();
            m_arrMembers[3].ID = 4;
            m_arrMembers[3].NodeStart = m_arrNodes[3];
            m_arrMembers[3].NodeEnd = m_arrNodes[4];
            m_arrMembers[3].CrScStart = m_arrCrSc[0];
            m_arrMembers[3].Fill_Basic();

            // Nodal Supports - fill values
            // Support 1 - NodeIDs: 1,5
            m_arrNSupports[0] = new CNSupport(m_eNDOF);
            m_arrNSupports[0].ID = 1;
            m_arrNSupports[0].m_bRestrain[0] = true; // true - 1 restraint (infinity) / false - 0 - free (zero rigidity)
            m_arrNSupports[0].m_bRestrain[1] = true;
            m_arrNSupports[0].m_bRestrain[2] = false;
            m_arrNSupports[0].m_iNodeCollection = new int[2];
            m_arrNSupports[0].m_iNodeCollection[0] = 1;
            m_arrNSupports[0].m_iNodeCollection[1] = 5;

            // Sort by ID
            Array.Sort(m_arrNSupports, new BaseClasses.CCompare_NSupportID());

            // Member loads
            // Load 1 - MemberIDs: 1
            CMLoad_21 MLoad_q1 = new CMLoad_21(fLoadColumn_q1);
            MLoad_q1.ID = 1;
            MLoad_q1.MLoadTypeDistr = EMLoadTypeDistr.eMLT_QUF_W_21;
            MLoad_q1.MLoadType = ELoadType.eLT_F;
            MLoad_q1.ELoadCS = ELoadCoordSystem.eLCS;
            MLoad_q1.ELoadDir = ELoadDirection.eLD_Z;
            MLoad_q1.IMemberCollection = new int[1];
            MLoad_q1.IMemberCollection[0] = 1;
            MLoad_q1.Member = m_arrMembers[0];
            m_arrMLoads[0] = MLoad_q1;

            // Load 2 - MemberIDs: 2
            for(int i = 0; i< loadRafter1.Count; i++)
            {
                m_arrMLoads[1 + i] = loadRafter1[i];
                m_arrMLoads[1 + i].IMemberCollection = new int[1];
                m_arrMLoads[1 + i].IMemberCollection[0] = 2;
                m_arrMLoads[1 + i].Member = m_arrMembers[1];
            }

            // Load 3 - MemberIDs: 3
            for (int i = 0; i < loadRafter2.Count; i++)
            {
                m_arrMLoads[1 + loadRafter1.Count + i] = loadRafter2[i];
                m_arrMLoads[1 + loadRafter1.Count + i].IMemberCollection = new int[1];
                m_arrMLoads[1 + loadRafter1.Count + i].IMemberCollection[0] = 3;
                m_arrMLoads[1 + loadRafter1.Count + i].Member = m_arrMembers[2];
            }

            // Load 4 - MemberIDs: 4
            CMLoad_21 MLoad_q4 = new CMLoad_21(fLoadColumn_q2);
            MLoad_q4.ID = 1 + loadRafter1.Count + loadRafter2.Count + 1;
            MLoad_q4.MLoadTypeDistr = EMLoadTypeDistr.eMLT_QUF_W_21;
            MLoad_q4.MLoadType = ELoadType.eLT_F;
            MLoad_q4.ELoadCS = ELoadCoordSystem.eLCS;
            MLoad_q4.ELoadDir = ELoadDirection.eLD_Z;
            MLoad_q4.IMemberCollection = new int[1];
            MLoad_q4.IMemberCollection[0] = 4;
            MLoad_q4.Member = m_arrMembers[3];
            m_arrMLoads[1 + loadRafter1.Count + loadRafter2.Count] = MLoad_q4;

            // Load Cases
            // Load Case 1
            CLoadCase LoadCase0 = new CLoadCase();
            LoadCase0.ID = 1;
            LoadCase0.Name = "lcase1";
            LoadCase0.Type = ELCType.ePermanentLoad;
            LoadCase0.MType_LS = ELCGTypeForLimitState.eUniversal;
            LoadCase0.Factor = 1.00f;

            for (int i = 0; i < m_arrMLoads.Length; i++)
                LoadCase0.MemberLoadsList.Add(m_arrMLoads[i]);

            m_arrLoadCases[0] = LoadCase0;

            // Load Combinations
            // Load Combination 1
            CLoadCombination LoadComb0 = new CLoadCombination();
            LoadComb0.ID = 1;
            LoadCase0.Name = "lcomb1";
            LoadComb0.eLComType = ELSType.eLS_ULS;
            LoadComb0.LoadCasesList = new List<CLoadCase>() {LoadCase0};
            LoadComb0.LoadCasesFactorsList = new List<float>() {1.00f};

            m_arrLoadCombs[0] = LoadComb0;
        }
    }
}
