using BaseClasses;
using CRSC;
using MATERIAL;

namespace Examples
{
    public class CExample_3D_50 : CExample
    {
        public CExample_3D_50()
        {
            m_eSLN = ESLN.e3DD_1D; // 1D members in 3D model
            m_eNDOF = (int)ENDOF.e3DEnv; // DOF in 3D
            m_eGCS = EGCS.eGCSLeftHanded; // Global coordinate system

            m_arrNodes = new CNode[2];
            m_arrMembers = new CMember[1];
            m_arrMat = new CMat[1];
            m_arrCrSc = new CCrSc[1];
            m_arrNSupports = new CNSupport[2];
            m_arrNReleases = new CNRelease[0];
            m_arrNLoads = new CNLoad[0];
            m_arrMLoads = new CMLoad[1];
            m_arrLoadCases = new CLoadCase[1];
            m_arrLoadCombs = new CLoadCombination[1];

            // Materials
            // Materials List - Materials Array - Fill Data of Materials Array
            m_arrMat[0] = new CMat_03_00();

            // Cross-sections
            // CrSc List - CrSc Array - Fill Data of Cross-sections Array

            m_arrCrSc[0] = new CCrSc_0_05(0.3f, 0.1f); // Solid square section

            // Nodes
            // Nodes List - Nodes Array

            m_arrNodes[0] = new CNode(1, 0, 0, 0, 0);
            m_arrNodes[1] = new CNode(2, 5, 0, 0, 0);

            // Sort by ID
            //Array.Sort(m_arrNodes, new BaseClasses.CCompare_NodeID());

            // Members
            // Members List - Members Array

            m_arrMembers[0] = new CMember(1, m_arrNodes[0], m_arrNodes[1], m_arrCrSc[0], 0);

            //Sort by ID
            //Array.Sort(m_arrMembers, new BaseClasses.CCompare_MemberID());

            // Nodal Supports - fill values
            // Set values
            //bool[] bSupport1 = { true, true, true, false, false, false };
            //bool[] bSupport2 = { false, true, true, false, false, false };

            //bool[] bSupport1 = { false, false, false, true, true, true };
            //bool[] bSupport2 = { true, true, true, true, true,true };

            bool[] bSupport1 = { true, true, true, true, false, false};
            bool[] bSupport2 = { false, true, true, true, false, false};

            // Create Support Objects
            // Pozn. Jednym z parametrov by malo byt pole ID uzlov v ktorych je zadefinovana tato podpora
            // objekt podpory bude len jeden a dotknute uzly budu vediet ze na ich podpora existuje a ake je konkretne ID jej nastaveni
            m_arrNSupports[0] = new BaseClasses.CNSupport(6, 1, m_arrNodes[0], bSupport1, 0);
            m_arrNSupports[1] = new BaseClasses.CNSupport(6, 2, m_arrNodes[1], bSupport2, 0);

            m_arrNSupports[0].m_iNodeCollection = new int[1];
            m_arrNSupports[1].m_iNodeCollection = new int[1];

            m_arrNSupports[0].m_iNodeCollection[0] = m_arrNodes[0].ID;
            m_arrNSupports[1].m_iNodeCollection[0] = m_arrNodes[1].ID;

            // Sort by ID
            //Array.Sort(m_arrNSupports, new BaseClasses.CCompare_NSupportID());

            // Member Releases

            bool?[] bRelease1 = { false, false, false, false, false, false };
            bool?[] bRelease2 = { true, true, true, false, false, false };
            //m_arrNReleases[0] = new BaseClasses.CNRelease(6, m_arrMembers[0].NodeStart, m_arrMembers[0], bRelease1, 0);
            //m_arrNReleases[1] = new BaseClasses.CNRelease(6, m_arrMembers[0].NodeEnd, m_arrMembers[0], bRelease2, 0);

            // Nodal Loads
            /*
            m_arrNLoads[0] = new BaseClasses.CNLoadSingle(m_arrNodes[0], ENLoadType.eNLT_Fx, 1.0f, true, 0);
            m_arrNLoads[1] = new BaseClasses.CNLoadSingle(m_arrNodes[1], ENLoadType.eNLT_Mx, 1.0f, true, 0);
            m_arrNLoads[2] = new BaseClasses.CNLoadSingle(m_arrNodes[0], ENLoadType.eNLT_Fy, 1.0f, true, 0);
            m_arrNLoads[3] = new BaseClasses.CNLoadSingle(m_arrNodes[1], ENLoadType.eNLT_My, 1.0f, true, 0);
            m_arrNLoads[4] = new BaseClasses.CNLoadSingle(m_arrNodes[0], ENLoadType.eNLT_Fz, 1.0f, true, 0);
            m_arrNLoads[5] = new BaseClasses.CNLoadSingle(m_arrNodes[1], ENLoadType.eNLT_Mz, 1.0f, true, 0);
            */

            // Member Loads
            //m_arrMLoads[0] = new BaseClasses.CMLoad_12(1.0f, m_arrMembers[0], EMLoadTypeDistr.eMLT_FS_H_12,ELoadType.eLT_F,ELoadDirection.eLD_Z, true, 0);

            m_arrMLoads[0] = new CMLoad_21(1, -0.8f, m_arrMembers[0], EMLoadTypeDistr.eMLT_FS_H_12, ELoadType.eLT_F, ELoadCoordSystem.eLCS, ELoadDirection.eLD_Z, true, 0);
            //m_arrMLoads[1] = new BaseClasses.CMLoad_21(1.0f, m_arrMembers[0], EMLoadTypeDistr.eMLT_FS_H_12, ELoadType.eLT_F, ELoadDirection.eLD_Y, true, 0);
            //m_arrMLoads[2] = new BaseClasses.CMLoad_21(1.5f, m_arrMembers[0], EMLoadTypeDistr.eMLT_FS_H_12, ELoadType.eLT_F, ELoadDirection.eLD_Z, true, 0);

            m_arrMLoads[0].IMemberCollection = new int[1];
            m_arrMLoads[0].IMemberCollection[0] = 1;  // Member ID 1 (1-2)

            // Load Cases
            // Load Case 1
            m_arrLoadCases[0] = new CLoadCase();
            m_arrLoadCases[0].ID = 1;

            // Load Combinations
            // Load Combination 1
            m_arrLoadCombs[0] = new CLoadCombination();
            m_arrLoadCombs[0].ID = 1;
        }
    }
}
