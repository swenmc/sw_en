using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BaseClasses;
using MATERIAL;
using CRSC;
using BaseClasses.CRSC;

namespace sw_en_GUI.EXAMPLES._2D
{
    public class CExample_2D_12 : CExample
    {
        public CExample_2D_12()
        {
            m_eSLN = ESLN.e2DD_1D; // 1D members in 2D model
            m_eNDOF = (int)ENDOF.e2DEnv; // DOF in 2D
            m_eGCS = EGCS.eGCSLeftHanded; // Global coordinate system

            m_arrNodes = new CNode[5];
            m_arrMembers = new CMember[4];
            m_arrMat = new CMat[1];
            m_arrCrSc = new CCrSc[3];
            m_arrNSupports = new CNSupport[1];
            m_arrMLoads = new CMLoad[1];
            m_arrLoadCases = new CLoadCase[1];
            m_arrLoadCombs = new CLoadCombination[1];

            // Auxiliary
            // Use basic SI units
            // Sobota, J. > Statika stavebnych konstrukcii 2

            // Load
            float fq = 20f;   // Unit [N/m]

            // Geometry
            float fa = 3.0f,
                  fb = 4.0f,
                  fc = 1.0f,
                  fd = 2.0f,
                  fe = 2.5f;// Unit [m]

            // Materials
            // Materials List - Materials Array - Fill Data of Materials Array
            m_arrMat[0] = new CMat();

            // Auxiliary, dopocitat 
            m_arrMat[0].m_fE = 10000000f;          // Unit [Pa]
            m_arrMat[0].m_fNu = 0.3f;              // Unit [-]
            m_arrMat[0].m_fG = 4000000;            // Unit [Pa]

            // Cross-sections
            // CrSc List - CrSc Array - Fill Data of Cross-sections Array
            // Cross-section
            m_arrCrSc[0] = new CCrSc_0_00();  //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            m_arrCrSc[0].A_g = 1.18095238095E-02f; // Unit [m^2]
            m_arrCrSc[0].I_y = 9.52380952381E-05f; // Unit [m^4]
            m_arrCrSc[0].m_Mat = m_arrMat[0]; // Set CrSc Material

            m_arrCrSc[1] = new CCrSc_0_00();  //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            m_arrCrSc[1].A_g = 1.48571428571E-02f; // Unit [m^2]
            m_arrCrSc[1].I_y = 1.90476190476E-04f; // Unit [m^4]
            m_arrCrSc[1].m_Mat = m_arrMat[0]; // Set CrSc Material

            m_arrCrSc[2] = new CCrSc_0_00();  //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            m_arrCrSc[2].A_g = 1.35238095238E-02f; // Unit [m^2]
            m_arrCrSc[2].I_y = 1.42857142857E-07f; // Unit [m^4]
            m_arrCrSc[2].m_Mat = m_arrMat[0]; // Set CrSc Material

            // Nodes
            // Nodes List - Nodes Array

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
            m_arrNodes[1].Y = -fc - fd;
            m_arrNodes[1].Z = 0f;

            // Node 3
            m_arrNodes[2] = new CNode();
            m_arrNodes[2].ID = 3;
            m_arrNodes[2].X = fa;
            m_arrNodes[2].Y = -fc - fd - fe;
            m_arrNodes[2].Z = 0f;

            // Node 4
            m_arrNodes[3] = new CNode();
            m_arrNodes[3].ID = 4;
            m_arrNodes[3].X = fa + fb;
            m_arrNodes[3].Y = -fc - fd - fe;
            m_arrNodes[3].Z = 0f;

            // Node 5
            m_arrNodes[4] = new CNode();
            m_arrNodes[4].ID = 5;
            m_arrNodes[4].X = fa + fb;
            m_arrNodes[4].Y = fc;
            m_arrNodes[4].Z = 0f;

            // Sort by ID
            //Array.Sort(m_arrNodes, new BaseClasses.CCompare_NodeID());

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
            m_arrMembers[1].CrScStart = m_arrCrSc[1];

            // Member 3 - 3-4
            m_arrMembers[2] = new CMember();
            m_arrMembers[2].ID = 3;
            m_arrMembers[2].NodeStart = m_arrNodes[2];
            m_arrMembers[2].NodeEnd = m_arrNodes[3];
            m_arrMembers[2].CrScStart = m_arrCrSc[1];

            // Member 4 - 4-5
            m_arrMembers[3] = new CMember();
            m_arrMembers[3].ID = 4;
            m_arrMembers[3].NodeStart = m_arrNodes[3];
            m_arrMembers[3].NodeEnd = m_arrNodes[4];
            m_arrMembers[3].CrScStart = m_arrCrSc[2];

            //Sort by ID
            //Array.Sort(m_arrMembers, new BaseClasses.CCompare_MemberID());

            // Nodal Supports - fill values
            // Support 1 - NodeIDs: 1,5
            m_arrNSupports[0] = new CNSupport(m_eNDOF);
            m_arrNSupports[0].ID = 1;
            m_arrNSupports[0].m_bRestrain[0] = true; // true - 1 restraint (infinity) / false - 0 - free (zero rigidity)
            m_arrNSupports[0].m_bRestrain[1] = true;
            m_arrNSupports[0].m_bRestrain[2] = true;
            m_arrNSupports[0].m_iNodeCollection = new int[2];
            m_arrNSupports[0].m_iNodeCollection[0] = 1;
            m_arrNSupports[0].m_iNodeCollection[1] = 5;

            // Sort by ID
            Array.Sort(m_arrNSupports, new BaseClasses.CCompare_NSupportID());

            // Member loads
            // Load 1 - MemberIDs: 3
            CMLoad_21 MLoad_q = new CMLoad_21(fq);
            MLoad_q.ID = 1;
            MLoad_q.MLoadTypeDistr = EMLoadTypeDistr.eMLT_QUF_W_21;
            MLoad_q.MLoadType = EMLoadType.eMLT_F;
            MLoad_q.EDirPPC = EMLoadDirPCC1.eMLD_PCC_FYU_MZV;
            MLoad_q.IMemberCollection = new int[1];
            MLoad_q.IMemberCollection[0] = 3;

            m_arrMLoads[0] = MLoad_q;

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
