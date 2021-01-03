using BaseClasses;
using CRSC;
using MATERIAL;
using System;

namespace Examples
{
    public class CExample_2D_15_KOSOUHLYRAM : CExample
    {
        public CExample_2D_15_KOSOUHLYRAM()
        {
            m_eSLN = ESLN.e2DD_1D; // 1D members in 2D model
            m_eNDOF = (int)ENDOF.e2DEnv; // DOF in 2D
            m_eGCS = EGCS.eGCSLeftHanded; // Global coordinate system

            m_arrNodes = new CNode[3];
            m_arrMembers = new CMember[2];
            m_arrMat = new System.Collections.Generic.Dictionary<EMemberGroupNames, CMat>();
            m_arrCrSc = new System.Collections.Generic.Dictionary<EMemberGroupNames, CCrSc>();
            m_arrNSupports = new CNSupport[2];
            m_arrNLoads = new CNLoad[3];
            m_arrMLoads = new CMLoad[4];
            m_arrLoadCases = new CLoadCase[1];
            m_arrLoadCombs = new CLoadCombination[1];

            // Materials
            // Materials List - Materials Array - Fill Data of Materials Array
            m_arrMat[0] = new CMat(20e+9f,0.3f,1.2e-5f,7850f);

            // Cross-sections
            // CrSc List - CrSc Array - Fill Data of Cross-sections Array
            // Cross-section
            m_arrCrSc[0] = new CCrSc_GE();
            m_arrCrSc[0].A_g = 0.15f;
            m_arrCrSc[0].I_y = 0.003125f;
            m_arrCrSc[0].m_Mat = m_arrMat[0]; // Set CrSc Material

            m_arrCrSc[(EMemberGroupNames)1] = new CCrSc_GE();
            m_arrCrSc[(EMemberGroupNames)1].A_g = 0.12f;
            m_arrCrSc[(EMemberGroupNames)1].I_y = 0.0016f;
            m_arrCrSc[(EMemberGroupNames)1].m_Mat = m_arrMat[0]; // Set CrSc Material

            // Nodes
            // Nodes List - Nodes Array

            // Node 1
            m_arrNodes[0] = new CNode();
            m_arrNodes[0].ID = 1;
            m_arrNodes[0].X = 0f;
            m_arrNodes[0].Y = 0f;
            m_arrNodes[0].Z = 2.5f;

            // Node 2
            m_arrNodes[1] = new CNode();
            m_arrNodes[1].ID = 2;
            m_arrNodes[1].X = 5f;
            m_arrNodes[1].Y = 0f;
            m_arrNodes[1].Z = 4.0f;

            // Node 3
            m_arrNodes[2] = new CNode();
            m_arrNodes[2].ID = 3;
            m_arrNodes[2].X = 8f;
            m_arrNodes[2].Y = 0f;
            m_arrNodes[2].Z = 0f;

            // Members
            // Members List - Members Array

            // Member 1 - 1-2
            m_arrMembers[0] = new CMember();
            m_arrMembers[0].ID = 1;
            m_arrMembers[0].NodeStart = m_arrNodes[0];
            m_arrMembers[0].NodeEnd = m_arrNodes[1];
            m_arrMembers[0].CrScStart = m_arrCrSc[0];

            // Member 2 - 2-3
            m_arrMembers[1] = new CMember();
            m_arrMembers[1].ID = 2;
            m_arrMembers[1].NodeStart = m_arrNodes[1];
            m_arrMembers[1].NodeEnd = m_arrNodes[2];
            m_arrMembers[1].CrScStart = m_arrCrSc[(EMemberGroupNames)1];

            // Nodal Supports - fill values
            // Support 1 - NodeIDs: 1
            m_arrNSupports[0] = new CNSupport(m_eNDOF);
            m_arrNSupports[0].ID = 1;
            m_arrNSupports[0].m_bRestrain[0] = true; // true - 1 restraint (infinity) / false - 0 - free (zero rigidity)
            m_arrNSupports[0].m_bRestrain[1] = true;
            m_arrNSupports[0].m_bRestrain[2] = true;
            m_arrNSupports[0].m_iNodeCollection = new int[1];
            m_arrNSupports[0].m_iNodeCollection[0] = 1;

            // Support 2 - NodeIDs: 3
            m_arrNSupports[1] = new CNSupport(m_eNDOF);
            m_arrNSupports[1].ID = 2;
            m_arrNSupports[1].m_bRestrain[0] = true; // true - 1 restraint (infinity) / false - 0 - free (zero rigidity)
            m_arrNSupports[1].m_bRestrain[1] = true;
            m_arrNSupports[1].m_bRestrain[2] = false;
            m_arrNSupports[1].m_iNodeCollection = new int[1];
            m_arrNSupports[1].m_iNodeCollection[0] = 3;

            // Sort by ID
            Array.Sort(m_arrNSupports, new BaseClasses.CCompare_NSupportID());

            // Nodal loads
            CNLoadSingle NLoad_F1 = new CNLoadSingle(1,m_arrNodes[1], ENLoadType.eNLT_Fy, -4000, true, 0);
            m_arrNLoads[0] = NLoad_F1;

            CNLoadSingle NLoad_F2 = new CNLoadSingle(2, m_arrNodes[2], ENLoadType.eNLT_Fy, -3000, true, 0);
            m_arrNLoads[1] = NLoad_F1;

            CNLoadSingle NLoad_M = new CNLoadSingle(3, m_arrNodes[2], ENLoadType.eNLT_Mz, -675, true, 0);
            m_arrNLoads[2] = NLoad_M;

            // Member loads
            // Load 1 - MemberIDs: 1
            CMLoad_21 MLoad_n1 = new CMLoad_21(-2299f);
            MLoad_n1.ID = 1;
            MLoad_n1.MLoadTypeDistr = EMLoadTypeDistr.eMLT_QUF_W_21;
            MLoad_n1.MLoadType = ELoadType.eLT_F;
            MLoad_n1.ELoadCS = ELoadCoordSystem.eLCS;
            MLoad_n1.ELoadDir = ELoadDirection.eLD_X;
            MLoad_n1.IMemberCollection = new int[1];
            MLoad_n1.IMemberCollection[0] = 1;
            m_arrMLoads[0] = MLoad_n1;

            // Load 2 - MemberIDs: 1
            CMLoad_21 MLoad_q1 = new CMLoad_21(7663f);
            MLoad_q1.ID = 2;
            MLoad_q1.MLoadTypeDistr = EMLoadTypeDistr.eMLT_QUF_W_21;
            MLoad_q1.MLoadType = ELoadType.eLT_F;
            MLoad_q1.ELoadCS = ELoadCoordSystem.eLCS;
            MLoad_q1.ELoadDir = ELoadDirection.eLD_Z;
            MLoad_q1.IMemberCollection = new int[1];
            MLoad_q1.IMemberCollection[0] = 1;
            m_arrMLoads[1] = MLoad_q1;

            // Load 3 - MemberIDs: 2
            CMLoad_21 MLoad_n2 = new CMLoad_21(3200f);
            MLoad_n2.ID = 3;
            MLoad_n2.MLoadTypeDistr = EMLoadTypeDistr.eMLT_QUF_W_21;
            MLoad_n2.MLoadType = ELoadType.eLT_F;
            MLoad_n2.ELoadCS = ELoadCoordSystem.eLCS;
            MLoad_n2.ELoadDir = ELoadDirection.eLD_X;
            MLoad_n2.IMemberCollection = new int[1];
            MLoad_n2.IMemberCollection[0] = 2;
            m_arrMLoads[2] = MLoad_n2;

            // Load 4 - MemberIDs: 2
            CMLoad_21 MLoad_q2 = new CMLoad_21(2400f);
            MLoad_q2.ID = 4;
            MLoad_q2.MLoadTypeDistr = EMLoadTypeDistr.eMLT_QUF_W_21;
            MLoad_q2.MLoadType = ELoadType.eLT_F;
            MLoad_q2.ELoadCS = ELoadCoordSystem.eLCS;
            MLoad_q2.ELoadDir = ELoadDirection.eLD_Z;
            MLoad_q2.IMemberCollection = new int[1];
            MLoad_q2.IMemberCollection[0] = 2;
            m_arrMLoads[3] = MLoad_q2;

            // Load Cases
            // Load Case 1
            CLoadCase LoadCase0 = new CLoadCase();
            LoadCase0.ID = 1;

            m_arrLoadCases[0] = LoadCase0;

            // Load Combinations
            // Load Combination 1
            CLoadCombination LoadComb0 = new CLoadCombination();
            LoadComb0.ID = 1;

            m_arrLoadCombs[0] = LoadComb0;
        }
    }
}
