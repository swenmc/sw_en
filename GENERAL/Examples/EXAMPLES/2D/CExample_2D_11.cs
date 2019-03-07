using BaseClasses;
using CRSC;
using MATERIAL;
using System;

namespace Examples
{
    public class CExample_2D_11 : CExample
    {
        public CExample_2D_11()
        {
            m_eSLN = ESLN.e2DD_1D; // 1D members in 2D model
            m_eNDOF = (int)ENDOF.e2DEnv; // DOF in 2D
            m_eGCS = EGCS.eGCSLeftHanded; // Global coordinate system

            m_arrNodes = new CNode[5];
            m_arrMembers = new CMember[4];
            m_arrMat = new CMat[1];
            m_arrCrSc = new CCrSc[1];
            m_arrNSupports = new CNSupport[3];
            m_arrNReleases = new CNRelease[1];
            m_arrNLoads = new CNLoad[1];
            m_arrMLoads = new CMLoad[4];
            m_arrLoadCases = new CLoadCase[1];
            m_arrLoadCombs = new CLoadCombination[1];


            // Auxiliary
            // Load
            float fF1 = 45000f;   // Unit [N]
            float fF2 = 55000f;   // Unit [N]
            float fM = 60000000f;   // Unit [Nm]
            float fq = 22f;   // Unit [N/m]

            // Geometry
            float fa = 2.8f,
                  fb = 5.6f,
                  fc = 4.2f;     // Unit [m]

            // Materials
            // Materials List - Materials Array - Fill Data of Materials Array
            m_arrMat[0] = new CMat();

            m_arrMat[0].m_fE = 10000000f;          // Unit [Pa]
            m_arrMat[0].m_fNu = 0.3f;              // Unit [-]
            m_arrMat[0].m_fG = 4000000;            // Unit [Pa]

            // Cross-sections
            // CrSc List - CrSc Array - Fill Data of Cross-sections Array
            m_arrCrSc[0] = new CCrSc_0_00();

            m_arrCrSc[0].A_g = 0.12f;   // Unit [m^2]
            m_arrCrSc[0].I_y = 0.0016f; // Unit [m^4]
            m_arrCrSc[0].I_z = 0.0016f; // Unit [m^4]
            m_arrCrSc[0].m_Mat = m_arrMat[0]; // Set CrSc Material

            // Nodes
            // Nodes List - Nodes Array

            // Nodes
            // Node 1
            m_arrNodes[0] = new CNode();
            m_arrNodes[0].ID = 1;
            m_arrNodes[0].X = fa + fb;
            m_arrNodes[0].Y = fc;
            m_arrNodes[0].Z = 0f;

            // Node 2
            m_arrNodes[1] = new CNode();
            m_arrNodes[1].ID = 2;
            m_arrNodes[1].X = fb;
            m_arrNodes[1].Y = 0f;
            m_arrNodes[1].Z = 0f;

            // Node 3
            m_arrNodes[2] = new CNode();
            m_arrNodes[2].ID = 3;
            m_arrNodes[2].X = 0f;
            m_arrNodes[2].Y = 0f;
            m_arrNodes[2].Z = 0f;

            // Node 4
            m_arrNodes[3] = new CNode();
            m_arrNodes[3].ID = 4;
            m_arrNodes[3].X = 0f;
            m_arrNodes[3].Y = -fa;
            m_arrNodes[3].Z = 0f;

            // Node 5
            m_arrNodes[4] = new CNode();
            m_arrNodes[4].ID = 5;
            m_arrNodes[4].X = 0f;
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
            m_arrMembers[1].CrScStart = m_arrCrSc[0];

            // Member 3 - 3-4
            m_arrMembers[2] = new CMember();
            m_arrMembers[2].ID = 3;
            m_arrMembers[2].NodeStart = m_arrNodes[2];
            m_arrMembers[2].NodeEnd = m_arrNodes[3];
            m_arrMembers[2].CrScStart = m_arrCrSc[0];

            // Member 4 - 3-5
            m_arrMembers[3] = new CMember();
            m_arrMembers[3].ID = 4;
            m_arrMembers[3].NodeStart = m_arrNodes[2];
            m_arrMembers[3].NodeEnd = m_arrNodes[4];
            m_arrMembers[3].CrScStart = m_arrCrSc[0];

            //Sort by ID
            //Array.Sort(m_arrMembers, new BaseClasses.CCompare_MemberID());

            // Nodal Supports - fill values
            // Support 1 - NodeIDs: 1,4
            m_arrNSupports[0] = new CNSupport(m_eNDOF);
            m_arrNSupports[0].ID = 1;
            m_arrNSupports[0].m_bRestrain[0] = true; // true - 1 restraint (infinity) / false - 0 - free (zero rigidity)
            m_arrNSupports[0].m_bRestrain[1] = true;
            m_arrNSupports[0].m_bRestrain[2] = true;
            m_arrNSupports[0].m_iNodeCollection = new int[2];
            m_arrNSupports[0].m_iNodeCollection[0] = 1;
            m_arrNSupports[0].m_iNodeCollection[1] = 4;

            // Support 2 - NodeIDs: 3
            m_arrNSupports[1] = new CNSupport(m_eNDOF);
            m_arrNSupports[1].ID = 2;
            m_arrNSupports[1].m_bRestrain[0] = true; // true - 1 restraint (infinity) / false - 0 - free (zero rigidity) - support in GCS X-axis
            m_arrNSupports[1].m_bRestrain[1] = false;
            m_arrNSupports[1].m_bRestrain[2] = false;
            m_arrNSupports[1].m_iNodeCollection = new int[1];
            m_arrNSupports[1].m_iNodeCollection[0] = 3;

            // Support 3 - NodeIDs: 5
            m_arrNSupports[2] = new CNSupport(m_eNDOF);
            m_arrNSupports[2].ID = 3;
            m_arrNSupports[2].m_bRestrain[0] = true; // true - 1 restraint (infinity) / false - 0 - free (zero rigidity) - support in GCS X and Y-axis
            m_arrNSupports[2].m_bRestrain[1] = true;
            m_arrNSupports[2].m_bRestrain[2] = false;
            m_arrNSupports[2].m_iNodeCollection = new int[1];
            m_arrNSupports[2].m_iNodeCollection[0] = 5;

            // Sort by ID
            Array.Sort(m_arrNSupports, new BaseClasses.CCompare_NSupportID());

            // Member releases
            bool?[] bRelTemp = new bool?[m_eNDOF];
            bRelTemp[0] = true; // true - 1 restraint (infinity rigidity) / false - 0 - free (zero rigidity) - support in LCS X and Y-axis
            bRelTemp[1] = true;
            bRelTemp[2] = false;

            CNRelease NRelease1 = new CNRelease(m_eNDOF, m_arrNodes[1], m_arrMembers[0], bRelTemp, 0);
            NRelease1.m_iNodeCollection = new int[1];
            NRelease1.m_iNodeCollection[0] = 2;
            NRelease1.m_iMembCollection = new int[2];
            NRelease1.m_iMembCollection[0] = 1;
            NRelease1.m_iMembCollection[1] = 2;
            m_arrNReleases[0] = NRelease1;
            m_arrMembers[0].CnRelease2 = m_arrNReleases[0]; // Release at end node
            m_arrMembers[1].CnRelease1 = m_arrNReleases[0]; // Release at start node

            // Nodal loads
            // Load 1 - NodeIDs: 2
            CNLoadSingle NLoad0 = new CNLoadSingle();
            NLoad0.INLoad_ID = 1;
            NLoad0.NLoadType = ENLoadType.eNLT_Fy;
            NLoad0.INodeCollection = new int[1];
            NLoad0.INodeCollection[0] = 2;
            NLoad0.Value = fF2; // Positive

            m_arrNLoads[0] = NLoad0;

            // Member loads
            // Load 1 and 2 - MemberIDs: 1

            float fAlpha_1 = (236.3099f / 360f) * 2 * MATH.MathF.fPI;  // Radians
            float fF1x = Math.Abs(fF1 * (float)Math.Cos(fAlpha_1));    // Force in local coordinate system of member + possitive orientation  in x-axis
            float fF1y = fF1 * (float)Math.Sin(fAlpha_1);              // Force in local coordinate system of member + negative orientation in y-axis

            CMLoad_11 MLoad_F1x = new CMLoad_11(fF1x, 0.5f * m_arrMembers[0].FLength);
            MLoad_F1x.ID = 1;
            MLoad_F1x.MLoadTypeDistr = EMLoadTypeDistr.eMLT_FS_H_12;
            MLoad_F1x.MLoadType = EMLoadType.eMLT_F;
            MLoad_F1x.ELoadCS = ELoadCoordSystem.eLCS;
            MLoad_F1x.ELoadDir = ELoadDirection.eLD_X;
            MLoad_F1x.IMemberCollection = new int[1];
            MLoad_F1x.IMemberCollection[0] = 1;

            m_arrMLoads[0] = MLoad_F1x;

            CMLoad_11 MLoad_F1y = new CMLoad_11(fF1y, 0.5f * m_arrMembers[0].FLength);
            MLoad_F1y.ID = 2;
            MLoad_F1y.MLoadTypeDistr = EMLoadTypeDistr.eMLT_FS_H_12;
            MLoad_F1y.MLoadType = EMLoadType.eMLT_F;
            MLoad_F1y.ELoadCS = ELoadCoordSystem.eLCS;
            MLoad_F1y.ELoadDir = ELoadDirection.eLD_Y;
            MLoad_F1y.IMemberCollection = new int[1];
            MLoad_F1y.IMemberCollection[0] = 1;

            m_arrMLoads[1] = MLoad_F1y;

            // Load 3 - MemberIDs: 3
            CMLoad_21 MLoad_q = new CMLoad_21(fq);
            MLoad_q.ID = 3;
            MLoad_q.MLoadTypeDistr = EMLoadTypeDistr.eMLT_QUF_W_21;
            MLoad_q.MLoadType = EMLoadType.eMLT_F;
            MLoad_q.ELoadCS = ELoadCoordSystem.eLCS;
            MLoad_q.ELoadDir = ELoadDirection.eLD_Y;
            MLoad_q.IMemberCollection = new int[1];
            MLoad_q.IMemberCollection[0] = 2;

            m_arrMLoads[2] = MLoad_q;

            // Load 4 - MemberIDs: 4
            CMLoad_11 MLoad_M = new CMLoad_11(-fM, 0.5f * m_arrMembers[3].FLength);
            MLoad_M.ID = 4;
            MLoad_M.MLoadTypeDistr = EMLoadTypeDistr.eMLT_FS_H_12;
            MLoad_M.MLoadType = EMLoadType.eMLT_M;
            MLoad_M.ELoadDir = ELoadDirection.eLD_Y;
            MLoad_M.IMemberCollection = new int[1];
            MLoad_M.IMemberCollection[0] = 4;

            m_arrMLoads[3] = MLoad_M;

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
