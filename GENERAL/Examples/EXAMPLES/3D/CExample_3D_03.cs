using BaseClasses;
using CRSC;
using MATERIAL;
using System;

namespace Examples
{
    public class CExample_3D_03 :CExample
    {
        /*
        public CNode[] m_arrNodes = new CNode[18];
        public CMember[] m_arrMembers = new CMember[23];
        public CNSupport[] m_arrNSupports = new CNSupport[4];
        //public CNForce[] arrForces = new CNForce[3];
        int eNDOF = (int)ENDOF.e3DEnv;
        */
        public CExample_3D_03()
        {
            m_eSLN = ESLN.e3DD_1D; // 1D members in 3D model
            m_eNDOF = (int)ENDOF.e3DEnv; // DOF in 3D
            m_eGCS = EGCS.eGCSLeftHanded; // Global coordinate system

            m_arrNodes = new CNode[18];
            m_arrMembers = new CMember[23];
            m_arrMat = new CMat[1];
            m_arrCrSc = new CCrSc[1];
            m_arrNSupports = new CNSupport[4];
            //m_arrNLoads = new BaseClasses.CNLoad[3];

            // Materials
            // Materials List - Materials Array - Fill Data of Materials Array
            m_arrMat[0] = new CMat_03_00();

            // Cross-sections
            // CrSc List - CrSc Array - Fill Data of Cross-sections Array
            m_arrCrSc[0] = new CCrSc_0_05(0.1f, 0.05f);
            // Nodes Automatic Generation
            // Nodes List - Nodes Array

            // 1-level
            m_arrNodes[00] = new CNode(01, 500, 0, 00, 0);
            m_arrNodes[01] = new CNode(02, 1000, 0, 00, 0);
            m_arrNodes[02] = new CNode(03, 1500, 0, 00, 0);
            m_arrNodes[03] = new CNode(04, 1750, 0, 00, 0);
            // 2-level
            m_arrNodes[04] = new CNode(05, 350, 0, 300, 0);
            m_arrNodes[05] = new CNode(06, 500, 0, 300, 0);
            m_arrNodes[06] = new CNode(07, 1000, 0, 300, 0);
            m_arrNodes[07] = new CNode(08, 1500, 0, 300, 0);
            m_arrNodes[08] = new CNode(09, 1800, 0, 300, 0);
            // 3-level
            m_arrNodes[09] = new CNode(10, 200, 0, 600, 0);
            m_arrNodes[10] = new CNode(11, 500, 0, 600, 0);
            m_arrNodes[11] = new CNode(12, 1000, 0, 600, 0);
            m_arrNodes[12] = new CNode(13, 1500, 0, 600, 0);
            m_arrNodes[13] = new CNode(14, 1950, 0, 600, 0);
            // 4-level
            m_arrNodes[14] = new CNode(15, 50, 0, 900, 0);
            m_arrNodes[15] = new CNode(16, 500, 0, 900, 0);
            m_arrNodes[16] = new CNode(17, 1000, 0, 900, 0);
            m_arrNodes[17] = new CNode(18, 1500, 0, 900, 0);

            // Convert coordinates to meters
            foreach (CNode node in m_arrNodes)
            {
                node.X /= 1000;
                node.Y /= 1000;
                node.Z /= 1000;
            }

            // Setridit pole podle ID
            //Array.Sort(m_arrNodes, new CCompare_NodeID());

            // Members Automatic Generation
            // Members List - Members Array

            // 1-level - horizontal beams
            m_arrMembers[00] = new CMember(01, m_arrNodes[00], m_arrNodes[01], m_arrCrSc[0], 0);
            m_arrMembers[01] = new CMember(02, m_arrNodes[01], m_arrNodes[02], m_arrCrSc[0], 0);
            m_arrMembers[02] = new CMember(03, m_arrNodes[02], m_arrNodes[03], m_arrCrSc[0], 0);
            // 1-columns
            m_arrMembers[03] = new CMember(04, m_arrNodes[00], m_arrNodes[04], m_arrCrSc[0], 0);
            m_arrMembers[04] = new CMember(05, m_arrNodes[00], m_arrNodes[05], m_arrCrSc[0], 0);
            m_arrMembers[05] = new CMember(06, m_arrNodes[01], m_arrNodes[06], m_arrCrSc[0], 0);
            m_arrMembers[06] = new CMember(07, m_arrNodes[02], m_arrNodes[07], m_arrCrSc[0], 0);
            // 2-level - horizontal beams
            m_arrMembers[07] = new CMember(08, m_arrNodes[04], m_arrNodes[05], m_arrCrSc[0], 0);
            m_arrMembers[08] = new CMember(09, m_arrNodes[05], m_arrNodes[06], m_arrCrSc[0], 0);
            m_arrMembers[09] = new CMember(10, m_arrNodes[06], m_arrNodes[07], m_arrCrSc[0], 0);
            m_arrMembers[10] = new CMember(11, m_arrNodes[07], m_arrNodes[08], m_arrCrSc[0], 0);
            // 2-columns
            m_arrMembers[11] = new CMember(12, m_arrNodes[04], m_arrNodes[09], m_arrCrSc[0], 0);
            m_arrMembers[12] = new CMember(13, m_arrNodes[05], m_arrNodes[10], m_arrCrSc[0], 0);
            m_arrMembers[13] = new CMember(14, m_arrNodes[06], m_arrNodes[11], m_arrCrSc[0], 0);
            m_arrMembers[14] = new CMember(15, m_arrNodes[07], m_arrNodes[12], m_arrCrSc[0], 0);
            // 3-level - horizontal beams
            m_arrMembers[15] = new CMember(16, m_arrNodes[09], m_arrNodes[10], m_arrCrSc[0], 0);
            m_arrMembers[16] = new CMember(17, m_arrNodes[10], m_arrNodes[11], m_arrCrSc[0], 0);
            m_arrMembers[17] = new CMember(18, m_arrNodes[11], m_arrNodes[12], m_arrCrSc[0], 0);
            m_arrMembers[18] = new CMember(19, m_arrNodes[12], m_arrNodes[13], m_arrCrSc[0], 0);
            // 3-columns
            m_arrMembers[19] = new CMember(20, m_arrNodes[09], m_arrNodes[14], m_arrCrSc[0], 0);
            m_arrMembers[20] = new CMember(21, m_arrNodes[10], m_arrNodes[15], m_arrCrSc[0], 0);
            m_arrMembers[21] = new CMember(22, m_arrNodes[11], m_arrNodes[16], m_arrCrSc[0], 0);
            m_arrMembers[22] = new CMember(23, m_arrNodes[12], m_arrNodes[17], m_arrCrSc[0], 0);

            // Setridit pole podle ID
            //Array.Sort(m_arrMembers, new CCompare_LineID());

            // Nodal Supports - fill values

            // Set values
            bool[] bSupport1 = { true, false, true, false, true, false };
            //bool[] bSupport2 = { true, false, true, false, false, false };
            //bool[] bSupport3 = { true, false, false, false, false, false };

            // Create Support Objects
            m_arrNSupports[0] = new CNSupport(6, 1, m_arrNodes[14], bSupport1, 0);
            m_arrNSupports[1] = new CNSupport(6, 2, m_arrNodes[15], bSupport1, 0);
            m_arrNSupports[2] = new CNSupport(6, 3, m_arrNodes[16], bSupport1, 0);
            m_arrNSupports[3] = new CNSupport(6, 4, m_arrNodes[17], bSupport1, 0);

            // Setridit pole podle ID
            Array.Sort(m_arrNSupports, new CCompare_NSupportID());

            // Member Releases / hinges - fill values

            // Set values
            //bool[] bMembRelase1 = { true, false, false, false, false, false };
            //bool[] bMembRelase2 = { false, false, true, false, false, false };
            //bool[] bMembRelase3 = { false, false, false, false, true, false };
            bool?[] bMembRelase4 = { false, false, false, false, true, false };


            // Create Release / Hinge Objects
            m_arrMembers[03].CnRelease1 = new CNRelease(6, m_arrMembers[03].NodeStart, bMembRelase4, 0);
            m_arrMembers[11].CnRelease1 = new CNRelease(6, m_arrMembers[11].NodeStart, bMembRelase4, 0);
            m_arrMembers[19].CnRelease1 = new CNRelease(6, m_arrMembers[19].NodeStart, bMembRelase4, 0);

            // Nodal Forces - fill values

            //arrForces[0] = new CNForce(m_arrNodes[3],  -40.0f, 0.0f, -050.0f, 0);
            //arrForces[1] = new CNForce(m_arrNodes[8],  -30.0f, 0.0f, -100.0f, 0);
            //arrForces[2] = new CNForce(m_arrNodes[13], -20.0f, 0.0f, -100.0f, 0);
        }
    }
}
