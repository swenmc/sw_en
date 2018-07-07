using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BaseClasses;
using MATERIAL;
using CRSC;

namespace sw_en_GUI.EXAMPLES._3D
{
    class CExample_3D_11:CExample
    {
        public CExample_3D_11()
        {
            m_eSLN = ESLN.e3DD_1D; // 1D members in 3D model
            m_eNDOF = (int)ENDOF.e3DEnv; // DOF in 3D
            m_eGCS = EGCS.eGCSLeftHanded; // Global coordinate system

            m_arrNodes = new CNode[4];
            m_arrMembers = new CMember[3];
            m_arrMat = new CMat[1];
            m_arrCrSc = new CCrSc[1];
            m_arrNSupports = new CNSupport[2];
            m_arrMLoads = new CMLoad[3];
            m_arrLoadCases = new CLoadCase[1];
            m_arrLoadCombs = new CLoadCombination[1];

            // Materials
            // Materials List - Materials Array - Fill Data of Materials Array
            m_arrMat[0] = new CMat_03_00();

            // Cross-sections
            // CrSc List - CrSc Array - Fill Data of Cross-sections Array
            m_arrCrSc[0] = new CCrSc_3_00(0, 8, 0.300f, 0.125f, 0.0162f, 0.0108f, 0.0108f, 0.0065f, 0.2416f); // I 300 section
            m_arrCrSc[0].I_t = 5.69e-07f;
            m_arrCrSc[0].I_y = 9.79e-05f;
            m_arrCrSc[0].I_z = 4.49e-06f;
            m_arrCrSc[0].A_g = 6.90e-03f;
            m_arrCrSc[0].A_vy = 4.01e-03f;
            m_arrCrSc[0].A_vz = 2.89e-03f;

            // Nodes
            // Nodes List - Nodes Array

            // Geometry
            float fGeom_a = 4f,
                  fGeom_b = 5f,
                  fGeom_c = 3.5f;     // Unit [m]

            m_arrNodes[0] = new CNode(1, fGeom_a,        0,        0, 0);
            m_arrNodes[1] = new CNode(2,       0,        0,        0, 0);
            m_arrNodes[2] = new CNode(3, fGeom_a,        0, -fGeom_c, 0);
            m_arrNodes[3] = new CNode(4, fGeom_a, -fGeom_b,        0, 0);

            // Sort by ID
            //Array.Sort(m_arrNodes, new BaseClasses.CCompare_NodeID());

            // Members
            // Members List - Members Array

            m_arrMembers[0] = new CMember(1, m_arrNodes[0], m_arrNodes[1], m_arrCrSc[0], 0);
            m_arrMembers[1] = new CMember(2, m_arrNodes[0], m_arrNodes[2], m_arrCrSc[0], 0);
            m_arrMembers[2] = new CMember(3, m_arrNodes[0], m_arrNodes[3], m_arrCrSc[0], 0);

            // Nodal Supports - fill values

            // Restraints - list of node degreess of freedom
            // UX, UY, UZ, RX, RY, RZ
            // false - 0 - free DOF
            // true - 1 - restrained (rigid)

            // Set values
            bool[] bSupport1 = { true, true, true, true, true, true};
            bool[] bSupport2 = { true, true, true, false, false, false };

            // Create Support Objects
            m_arrNSupports[0] = new CNSupport(6, 1, m_arrNodes[1], bSupport1, 0);
            m_arrNSupports[1] = new CNSupport(6, 2, m_arrNodes[2], bSupport2, 0);

            // Fill list/collection of supported nodes
            m_arrNSupports[0].m_iNodeCollection = new int[2];
            m_arrNSupports[1].m_iNodeCollection = new int[1];

            m_arrNSupports[0].m_iNodeCollection[0] = 2; // Node ID 2
            m_arrNSupports[0].m_iNodeCollection[1] = 3; // Node ID 3

            m_arrNSupports[1].m_iNodeCollection[0] = 4; // Node ID 4

            // Create load objects
            m_arrMLoads[0] = new CMLoad_21(5000f);  // q - whole member
            m_arrMLoads[1] = new CMLoad_12(-17000f); // F - in the middle of member
            m_arrMLoads[2] = new CMLoad_12(20000f); // M - in the middle of member

            // Fill list/collection of loaded members
            m_arrMLoads[0].ID = 1;
            m_arrMLoads[1].ID = 2;
            m_arrMLoads[2].ID = 3;

            m_arrMLoads[0].MLoadTypeDistr = EMLoadTypeDistr.eMLT_QUF_W_21;
            m_arrMLoads[0].MLoadType = EMLoadType.eMLT_F;
            m_arrMLoads[0].EDirPPC = EMLoadDirPCC1.eMLD_PCC_FZV_MYU;

            m_arrMLoads[1].MLoadTypeDistr = EMLoadTypeDistr.eMLT_FS_H_12;
            m_arrMLoads[1].MLoadType = EMLoadType.eMLT_F;
            m_arrMLoads[1].EDirPPC = EMLoadDirPCC1.eMLD_PCC_FZV_MYU;

            m_arrMLoads[2].MLoadTypeDistr = EMLoadTypeDistr.eMLT_FS_H_12;
            m_arrMLoads[2].MLoadType = EMLoadType.eMLT_M;
            m_arrMLoads[2].EDirPPC = EMLoadDirPCC1.eMLD_PCC_FXX_MXX;

            m_arrMLoads[0].IMemberCollection = new int[1];
            m_arrMLoads[1].IMemberCollection = new int[1];
            m_arrMLoads[2].IMemberCollection = new int[1];

            m_arrMLoads[0].IMemberCollection[0] = 1;  // Member ID 1 (1-2)
            m_arrMLoads[1].IMemberCollection[0] = 2;  // Member ID 2 (1-3)
            m_arrMLoads[2].IMemberCollection[0] = 3;  // Member ID 3 (1-4)

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
