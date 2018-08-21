using BaseClasses;
using CRSC;
using MATERIAL;
using System;

namespace Examples
{
    public class CExample_3D_04 :CExample
    {
        /*
        public CNode[] m_arrNodes = new CNode[22];
        public CMember[] m_arrMembers = new CMember[41];
        public CNSupport[] m_arrNSupports = new CNSupport[2];
        //public CNForce[] arrForces = new CNForce[11];
        int eNDOF = (int)ENDOF.e3DEnv;
        */

        public CExample_3D_04()
        {
            m_eSLN = ESLN.e3DD_1D; // 1D members in 3D model
            m_eNDOF = (int)ENDOF.e3DEnv; // DOF in 3D
            m_eGCS = EGCS.eGCSLeftHanded; // Global coordinate system

            m_arrNodes = new BaseClasses.CNode[22];
            m_arrMembers = new CMember[41];
            m_arrMat = new CMat[1];
            m_arrCrSc = new CCrSc[1];
            m_arrNSupports = new BaseClasses.CNSupport[2];
            //m_arrNLoads = new BaseClasses.CNLoad[11];

            // Materials
            // Materials List - Materials Array - Fill Data of Materials Array
            m_arrMat[0] = new CMat_03_00();

            // Cross-sections
            // CrSc List - CrSc Array - Fill Data of Cross-sections Array
            m_arrCrSc[0] = new CCrSc_0_05(0.6f, 0.4f);

            // Nodes Automatic Generation
            // Nodes List - Nodes Array

            // 1-bottom chord
            m_arrNodes[00] = new CNode(01, 00000, 0, 12000, 0);
            m_arrNodes[01] = new CNode(02, 08000, 0, 12000, 0);
            m_arrNodes[02] = new CNode(03, 16000, 0, 12000, 0);
            m_arrNodes[03] = new CNode(04, 24000, 0, 12000, 0);
            m_arrNodes[04] = new CNode(05, 32000, 0, 12000, 0);
            m_arrNodes[05] = new CNode(06, 40000, 0, 12000, 0);
            m_arrNodes[06] = new CNode(07, 48000, 0, 12000, 0);
            m_arrNodes[07] = new CNode(08, 56000, 0, 12000, 0);
            m_arrNodes[08] = new CNode(09, 64000, 0, 12000, 0);
            m_arrNodes[09] = new CNode(10, 72000, 0, 12000, 0);
            m_arrNodes[10] = new CNode(11, 80000, 0, 12000, 0);
            // 2-upper chord
            m_arrNodes[11] = new CNode(12, 00000, 0, 08000, 0);
            m_arrNodes[12] = new CNode(13, 08000, 0, 05900, 0);
            m_arrNodes[13] = new CNode(14, 16000, 0, 04000, 0);
            m_arrNodes[14] = new CNode(15, 24000, 0, 03000, 0);
            m_arrNodes[15] = new CNode(16, 32000, 0, 02300, 0);
            m_arrNodes[16] = new CNode(17, 40000, 0, 02000, 0);
            m_arrNodes[17] = new CNode(18, 48000, 0, 02300, 0);
            m_arrNodes[18] = new CNode(19, 56000, 0, 03000, 0);
            m_arrNodes[19] = new CNode(20, 64000, 0, 04000, 0);
            m_arrNodes[20] = new CNode(21, 72000, 0, 05900, 0);
            m_arrNodes[21] = new CNode(22, 80000, 0, 08000, 0);

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

            // 1-bottom chord
            m_arrMembers[00] = new CMember(01, m_arrNodes[00], m_arrNodes[01], m_arrCrSc[0], 0);
            m_arrMembers[01] = new CMember(02, m_arrNodes[01], m_arrNodes[02], m_arrCrSc[0], 0);
            m_arrMembers[02] = new CMember(03, m_arrNodes[02], m_arrNodes[03], m_arrCrSc[0], 0);
            m_arrMembers[03] = new CMember(04, m_arrNodes[03], m_arrNodes[04], m_arrCrSc[0], 0);
            m_arrMembers[04] = new CMember(05, m_arrNodes[04], m_arrNodes[05], m_arrCrSc[0], 0);
            m_arrMembers[05] = new CMember(06, m_arrNodes[05], m_arrNodes[06], m_arrCrSc[0], 0);
            m_arrMembers[06] = new CMember(07, m_arrNodes[06], m_arrNodes[07], m_arrCrSc[0], 0);
            m_arrMembers[07] = new CMember(08, m_arrNodes[07], m_arrNodes[08], m_arrCrSc[0], 0);
            m_arrMembers[08] = new CMember(09, m_arrNodes[08], m_arrNodes[09], m_arrCrSc[0], 0);
            m_arrMembers[09] = new CMember(10, m_arrNodes[09], m_arrNodes[10], m_arrCrSc[0], 0);
            // 2-upper chord
            m_arrMembers[10] = new CMember(11, m_arrNodes[11], m_arrNodes[12], m_arrCrSc[0], 0);
            m_arrMembers[11] = new CMember(12, m_arrNodes[12], m_arrNodes[13], m_arrCrSc[0], 0);
            m_arrMembers[12] = new CMember(13, m_arrNodes[13], m_arrNodes[14], m_arrCrSc[0], 0);
            m_arrMembers[13] = new CMember(14, m_arrNodes[14], m_arrNodes[15], m_arrCrSc[0], 0);
            m_arrMembers[14] = new CMember(15, m_arrNodes[15], m_arrNodes[16], m_arrCrSc[0], 0);
            m_arrMembers[15] = new CMember(16, m_arrNodes[16], m_arrNodes[17], m_arrCrSc[0], 0);
            m_arrMembers[16] = new CMember(17, m_arrNodes[17], m_arrNodes[18], m_arrCrSc[0], 0);
            m_arrMembers[17] = new CMember(18, m_arrNodes[18], m_arrNodes[19], m_arrCrSc[0], 0);
            m_arrMembers[18] = new CMember(19, m_arrNodes[19], m_arrNodes[20], m_arrCrSc[0], 0);
            m_arrMembers[19] = new CMember(20, m_arrNodes[20], m_arrNodes[21], m_arrCrSc[0], 0);
            // 3-stops / columns / verticals
            m_arrMembers[20] = new CMember(21, m_arrNodes[00], m_arrNodes[11], m_arrCrSc[0], 0);
            m_arrMembers[21] = new CMember(22, m_arrNodes[01], m_arrNodes[12], m_arrCrSc[0], 0);
            m_arrMembers[22] = new CMember(23, m_arrNodes[02], m_arrNodes[13], m_arrCrSc[0], 0);
            m_arrMembers[23] = new CMember(24, m_arrNodes[03], m_arrNodes[14], m_arrCrSc[0], 0);
            m_arrMembers[24] = new CMember(25, m_arrNodes[04], m_arrNodes[15], m_arrCrSc[0], 0);
            m_arrMembers[25] = new CMember(26, m_arrNodes[05], m_arrNodes[16], m_arrCrSc[0], 0);
            m_arrMembers[26] = new CMember(27, m_arrNodes[06], m_arrNodes[17], m_arrCrSc[0], 0);
            m_arrMembers[27] = new CMember(28, m_arrNodes[07], m_arrNodes[18], m_arrCrSc[0], 0);
            m_arrMembers[28] = new CMember(29, m_arrNodes[08], m_arrNodes[19], m_arrCrSc[0], 0);
            m_arrMembers[29] = new CMember(30, m_arrNodes[09], m_arrNodes[20], m_arrCrSc[0], 0);
            m_arrMembers[30] = new CMember(31, m_arrNodes[10], m_arrNodes[21], m_arrCrSc[0], 0);
            // 4-diagonals
            m_arrMembers[31] = new CMember(32, m_arrNodes[01], m_arrNodes[11], m_arrCrSc[0], 0);
            m_arrMembers[32] = new CMember(33, m_arrNodes[02], m_arrNodes[12], m_arrCrSc[0], 0);
            m_arrMembers[33] = new CMember(34, m_arrNodes[03], m_arrNodes[13], m_arrCrSc[0], 0);
            m_arrMembers[34] = new CMember(35, m_arrNodes[04], m_arrNodes[14], m_arrCrSc[0], 0);
            m_arrMembers[35] = new CMember(36, m_arrNodes[05], m_arrNodes[15], m_arrCrSc[0], 0);
            m_arrMembers[36] = new CMember(37, m_arrNodes[05], m_arrNodes[17], m_arrCrSc[0], 0);
            m_arrMembers[37] = new CMember(38, m_arrNodes[06], m_arrNodes[18], m_arrCrSc[0], 0);
            m_arrMembers[38] = new CMember(39, m_arrNodes[07], m_arrNodes[19], m_arrCrSc[0], 0);
            m_arrMembers[39] = new CMember(40, m_arrNodes[08], m_arrNodes[20], m_arrCrSc[0], 0);
            m_arrMembers[40] = new CMember(41, m_arrNodes[09], m_arrNodes[21], m_arrCrSc[0], 0);

            // Setridit pole podle ID
            //Array.Sort(m_arrMembers, new CCompare_LineID());

            // Nodal Supports - fill values

            // Set values
            bool[] bSupport1 = { true, false, true, false, false, false };
            bool[] bSupport2 = { false, false, true, false, false, false };

            // Create Support Objects
            m_arrNSupports[0] = new CNSupport(6, 1, m_arrNodes[00], bSupport1, 0);
            m_arrNSupports[1] = new CNSupport(6, 2, m_arrNodes[10], bSupport2, 0);

            // Setridit pole podle ID
            Array.Sort(m_arrNSupports, new CCompare_NSupportID());

            // Member Releases / hinges - fill values

            // Set values
            bool?[] bMembRelase1 = { false, false, false, false, true, false };

            // Create Release / Hinge Objects
            m_arrMembers[31].CnRelease1 = new CNRelease(0, bMembRelase1, 0);
            m_arrMembers[32].CnRelease1 = new CNRelease(0, bMembRelase1, 0);
            m_arrMembers[33].CnRelease1 = new CNRelease(0, bMembRelase1, 0);
            m_arrMembers[34].CnRelease1 = new CNRelease(0, bMembRelase1, 0);
            m_arrMembers[35].CnRelease1 = new CNRelease(0, bMembRelase1, 0);
            m_arrMembers[36].CnRelease1 = new CNRelease(0, bMembRelase1, 0);
            m_arrMembers[37].CnRelease1 = new CNRelease(0, bMembRelase1, 0);
            m_arrMembers[38].CnRelease1 = new CNRelease(0, bMembRelase1, 0);
            m_arrMembers[39].CnRelease1 = new CNRelease(0, bMembRelase1, 0);
            m_arrMembers[40].CnRelease1 = new CNRelease(0, bMembRelase1, 0);

            // Nodal Forces - fill values
            //arrForces[00] = new CNForce(m_arrNodes[00], -00.0f, 0.0f, -050.0f, 0);
            //arrForces[01] = new CNForce(m_arrNodes[01], -00.0f, 0.0f, -100.0f, 0);
            //arrForces[02] = new CNForce(m_arrNodes[02], -00.0f, 0.0f, -100.0f, 0);
            //arrForces[03] = new CNForce(m_arrNodes[03], -00.0f, 0.0f, -100.0f, 0);
            //arrForces[04] = new CNForce(m_arrNodes[04], -00.0f, 0.0f, -100.0f, 0);
            //arrForces[05] = new CNForce(m_arrNodes[05], -00.0f, 0.0f, -100.0f, 0);
            //arrForces[06] = new CNForce(m_arrNodes[06], -00.0f, 0.0f, -100.0f, 0);
            //arrForces[07] = new CNForce(m_arrNodes[07], -00.0f, 0.0f, -100.0f, 0);
            //arrForces[08] = new CNForce(m_arrNodes[08], -00.0f, 0.0f, -100.0f, 0);
            //arrForces[09] = new CNForce(m_arrNodes[09], -00.0f, 0.0f, -100.0f, 0);
            //arrForces[10] = new CNForce(m_arrNodes[10], -00.0f, 0.0f, -050.0f, 0);
        }
    }
}
