using BaseClasses;
using CRSC;
using MATERIAL;
using System;

namespace Examples
{
    public class CExample_3D_06 : CExample
    {
        /*
        public CNode[] m_arrNodes = new CNode[62];
        public CMember[] m_arrMembers = new CMember[102];
        public CNSupport[] m_arrNSupports = new CNSupport[19];
        //public CNForce[] arrForces = new CNForce[1];
        int eNDOF = (int)ENDOF.e3DEnv;
        */
        public CExample_3D_06()
        {
            m_eSLN = ESLN.e3DD_1D; // 1D members in 3D model
            m_eNDOF = (int)ENDOF.e3DEnv; // DOF in 3D
            m_eGCS = EGCS.eGCSLeftHanded; // Global coordinate system

            m_arrNodes = new CNode[62];
            m_arrMembers = new CMember[102];
            m_arrMat = new System.Collections.Generic.Dictionary<EMemberGroupNames, CMat>();
            m_arrCrSc = new CCrSc[1];
            m_arrNSupports = new CNSupport[19];
            //m_arrNLoads = new BaseClasses.CNLoad[1];

            // Materials
            // Materials List - Materials Array - Fill Data of Materials Array
            m_arrMat[0] = new CMat_03_00();

            // Cross-sections
            // CrSc List - CrSc Array - Fill Data of Cross-sections Array
            m_arrCrSc[0] = new CCrSc_0_05(0.1f, 0.05f);

            // Nodes Automatic Generation
            // Nodes List - Nodes Array

            // Nodes
            m_arrNodes[0] = new CNode(1, 0, -20000, 3000, 0);
            m_arrNodes[1] = new CNode(2, 0, -20000, 6000, 0);
            m_arrNodes[2] = new CNode(3, 0, -20000, 9000, 0);
            m_arrNodes[3] = new CNode(4, 0, -15000, 3000, 0);
            m_arrNodes[4] = new CNode(5, 0, -15000, 6000, 0);
            m_arrNodes[5] = new CNode(6, 0, -15000, 9000, 0);
            m_arrNodes[6] = new CNode(7, 0, -10000, 3000, 0);
            m_arrNodes[7] = new CNode(8, 0, -10000, 9000, 0);
            m_arrNodes[8] = new CNode(9, 0, -5000, 3000, 0);
            m_arrNodes[9] = new CNode(10, 0, -5000, 9000, 0);
            m_arrNodes[10] = new CNode(11, 0, 0, 3000, 0);
            m_arrNodes[11] = new CNode(12, 0, 0, 9000, 0);
            m_arrNodes[12] = new CNode(13, 3000, -15000, 2739, 0);
            m_arrNodes[13] = new CNode(14, 3000, -10000, 2739, 0);
            m_arrNodes[14] = new CNode(15, 3000, -5000, 2739, 0);
            m_arrNodes[15] = new CNode(16, 3000, 0, 2739, 0);
            m_arrNodes[16] = new CNode(17, 6250, -20000, 2454, 0);
            m_arrNodes[17] = new CNode(18, 6250, -20000, 6000, 0);
            m_arrNodes[18] = new CNode(19, 6250, -20000, 9000, 0);
            m_arrNodes[19] = new CNode(20, 6250, -15000, 2454, 0);
            m_arrNodes[20] = new CNode(21, 6250, -15000, 6000, 0);
            m_arrNodes[21] = new CNode(22, 6250, -15000, 9000, 0);
            m_arrNodes[22] = new CNode(23, 6250, -10000, 2454, 0);
            m_arrNodes[23] = new CNode(24, 6250, -5000, 2454, 0);
            m_arrNodes[24] = new CNode(25, 6250, 0, 2454, 0);
            m_arrNodes[25] = new CNode(26, 6250, 0, 9000, 0);
            m_arrNodes[26] = new CNode(27, 12500, -20000, 1906, 0);
            m_arrNodes[27] = new CNode(28, 12500, -20000, 6000, 0);
            m_arrNodes[28] = new CNode(29, 12500, -20000, 9000, 0);
            m_arrNodes[29] = new CNode(30, 12500, -15000, 1906, 0);
            m_arrNodes[30] = new CNode(31, 12500, -15000, 6000, 0);
            m_arrNodes[31] = new CNode(32, 12500, -15000, 9000, 0);
            m_arrNodes[32] = new CNode(33, 12500, -10000, 1906, 0);
            m_arrNodes[33] = new CNode(34, 12500, -5000, 1906, 0);
            m_arrNodes[34] = new CNode(35, 12500, 0, 1906, 0);
            m_arrNodes[35] = new CNode(36, 12500, 0, 9000, 0);
            m_arrNodes[36] = new CNode(37, 18750, -20000, 2454, 0);
            m_arrNodes[37] = new CNode(38, 18750, -20000, 6000, 0);
            m_arrNodes[38] = new CNode(39, 18750, -20000, 9000, 0);
            m_arrNodes[39] = new CNode(40, 18750, -15000, 2454, 0);
            m_arrNodes[40] = new CNode(41, 18750, -15000, 6000, 0);
            m_arrNodes[41] = new CNode(42, 18750, -15000, 9000, 0);
            m_arrNodes[42] = new CNode(43, 18750, -10000, 2454, 0);
            m_arrNodes[43] = new CNode(44, 18750, -5000, 2454, 0);
            m_arrNodes[44] = new CNode(45, 18750, 0, 2454, 0);
            m_arrNodes[45] = new CNode(46, 18750, 0, 9000, 0);
            m_arrNodes[46] = new CNode(47, 22000, -15000, 2739, 0);
            m_arrNodes[47] = new CNode(48, 22000, -10000, 2739, 0);
            m_arrNodes[48] = new CNode(49, 22000, -5000, 2739, 0);
            m_arrNodes[49] = new CNode(50, 22000, 0, 2739, 0);
            m_arrNodes[50] = new CNode(51, 25000, -20000, 3000, 0);
            m_arrNodes[51] = new CNode(52, 25000, -20000, 6000, 0);
            m_arrNodes[52] = new CNode(53, 25000, -20000, 9000, 0);
            m_arrNodes[53] = new CNode(54, 25000, -15000, 3000, 0);
            m_arrNodes[54] = new CNode(55, 25000, -15000, 6000, 0);
            m_arrNodes[55] = new CNode(56, 25000, -15000, 9000, 0);
            m_arrNodes[56] = new CNode(57, 25000, -10000, 3000, 0);
            m_arrNodes[57] = new CNode(58, 25000, -10000, 9000, 0);
            m_arrNodes[58] = new CNode(59, 25000, -5000, 3000, 0);
            m_arrNodes[59] = new CNode(60, 25000, -5000, 9000, 0);
            m_arrNodes[60] = new CNode(61, 25000, 0, 3000, 0);
            m_arrNodes[61] = new CNode(62, 25000, 0, 9000, 0);

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

            // Members
            m_arrMembers[0] = new CMember(1, m_arrNodes[1], m_arrNodes[0], m_arrCrSc[0], 0);
            m_arrMembers[1] = new CMember(2, m_arrNodes[2], m_arrNodes[1], m_arrCrSc[0], 0);
            m_arrMembers[2] = new CMember(3, m_arrNodes[3], m_arrNodes[0], m_arrCrSc[0], 0);
            m_arrMembers[3] = new CMember(4, m_arrNodes[4], m_arrNodes[1], m_arrCrSc[0], 0);
            m_arrMembers[4] = new CMember(5, m_arrNodes[4], m_arrNodes[3], m_arrCrSc[0], 0);
            m_arrMembers[5] = new CMember(6, m_arrNodes[5], m_arrNodes[4], m_arrCrSc[0], 0);
            m_arrMembers[6] = new CMember(7, m_arrNodes[6], m_arrNodes[3], m_arrCrSc[0], 0);
            m_arrMembers[7] = new CMember(8, m_arrNodes[7], m_arrNodes[6], m_arrCrSc[0], 0);
            m_arrMembers[8] = new CMember(9, m_arrNodes[8], m_arrNodes[6], m_arrCrSc[0], 0);
            m_arrMembers[9] = new CMember(10, m_arrNodes[7], m_arrNodes[8], m_arrCrSc[0], 0);
            m_arrMembers[10] = new CMember(11, m_arrNodes[9], m_arrNodes[6], m_arrCrSc[0], 0);
            m_arrMembers[11] = new CMember(12, m_arrNodes[9], m_arrNodes[8], m_arrCrSc[0], 0);
            m_arrMembers[12] = new CMember(13, m_arrNodes[10], m_arrNodes[8], m_arrCrSc[0], 0);
            m_arrMembers[13] = new CMember(14, m_arrNodes[11], m_arrNodes[10], m_arrCrSc[0], 0);
            m_arrMembers[14] = new CMember(15, m_arrNodes[3], m_arrNodes[12], m_arrCrSc[0], 0);
            m_arrMembers[15] = new CMember(16, m_arrNodes[6], m_arrNodes[13], m_arrCrSc[0], 0);
            m_arrMembers[16] = new CMember(17, m_arrNodes[8], m_arrNodes[14], m_arrCrSc[0], 0);
            m_arrMembers[17] = new CMember(18, m_arrNodes[10], m_arrNodes[15], m_arrCrSc[0], 0);
            m_arrMembers[18] = new CMember(19, m_arrNodes[0], m_arrNodes[16], m_arrCrSc[0], 0);
            m_arrMembers[19] = new CMember(20, m_arrNodes[1], m_arrNodes[17], m_arrCrSc[0], 0);
            m_arrMembers[20] = new CMember(21, m_arrNodes[4], m_arrNodes[20], m_arrCrSc[0], 0);
            m_arrMembers[21] = new CMember(22, m_arrNodes[23], m_arrNodes[6], m_arrCrSc[0], 0);
            m_arrMembers[22] = new CMember(23, m_arrNodes[8], m_arrNodes[22], m_arrCrSc[0], 0);
            m_arrMembers[23] = new CMember(24, m_arrNodes[12], m_arrNodes[19], m_arrCrSc[0], 0);
            m_arrMembers[24] = new CMember(25, m_arrNodes[13], m_arrNodes[22], m_arrCrSc[0], 0);
            m_arrMembers[25] = new CMember(26, m_arrNodes[14], m_arrNodes[23], m_arrCrSc[0], 0);
            m_arrMembers[26] = new CMember(27, m_arrNodes[15], m_arrNodes[24], m_arrCrSc[0], 0);
            m_arrMembers[27] = new CMember(28, m_arrNodes[17], m_arrNodes[16], m_arrCrSc[0], 0);
            m_arrMembers[28] = new CMember(29, m_arrNodes[18], m_arrNodes[17], m_arrCrSc[0], 0);
            m_arrMembers[29] = new CMember(30, m_arrNodes[19], m_arrNodes[16], m_arrCrSc[0], 0);
            m_arrMembers[30] = new CMember(31, m_arrNodes[20], m_arrNodes[17], m_arrCrSc[0], 0);
            m_arrMembers[31] = new CMember(32, m_arrNodes[20], m_arrNodes[19], m_arrCrSc[0], 0);
            m_arrMembers[32] = new CMember(33, m_arrNodes[21], m_arrNodes[20], m_arrCrSc[0], 0);
            m_arrMembers[33] = new CMember(34, m_arrNodes[22], m_arrNodes[19], m_arrCrSc[0], 0);
            m_arrMembers[34] = new CMember(35, m_arrNodes[23], m_arrNodes[22], m_arrCrSc[0], 0);
            m_arrMembers[35] = new CMember(36, m_arrNodes[24], m_arrNodes[23], m_arrCrSc[0], 0);
            m_arrMembers[36] = new CMember(37, m_arrNodes[25], m_arrNodes[24], m_arrCrSc[0], 0);
            m_arrMembers[37] = new CMember(38, m_arrNodes[16], m_arrNodes[26], m_arrCrSc[0], 0);
            m_arrMembers[38] = new CMember(39, m_arrNodes[17], m_arrNodes[27], m_arrCrSc[0], 0);
            m_arrMembers[39] = new CMember(40, m_arrNodes[19], m_arrNodes[29], m_arrCrSc[0], 0);
            m_arrMembers[40] = new CMember(41, m_arrNodes[20], m_arrNodes[30], m_arrCrSc[0], 0);
            m_arrMembers[41] = new CMember(42, m_arrNodes[22], m_arrNodes[32], m_arrCrSc[0], 0);
            m_arrMembers[42] = new CMember(43, m_arrNodes[33], m_arrNodes[22], m_arrCrSc[0], 0);
            m_arrMembers[43] = new CMember(44, m_arrNodes[23], m_arrNodes[32], m_arrCrSc[0], 0);
            m_arrMembers[44] = new CMember(45, m_arrNodes[23], m_arrNodes[33], m_arrCrSc[0], 0);
            m_arrMembers[45] = new CMember(46, m_arrNodes[24], m_arrNodes[34], m_arrCrSc[0], 0);
            m_arrMembers[46] = new CMember(47, m_arrNodes[27], m_arrNodes[26], m_arrCrSc[0], 0);
            m_arrMembers[47] = new CMember(48, m_arrNodes[28], m_arrNodes[27], m_arrCrSc[0], 0);
            m_arrMembers[48] = new CMember(49, m_arrNodes[29], m_arrNodes[26], m_arrCrSc[0], 0);
            m_arrMembers[49] = new CMember(50, m_arrNodes[30], m_arrNodes[27], m_arrCrSc[0], 0);
            m_arrMembers[50] = new CMember(51, m_arrNodes[30], m_arrNodes[29], m_arrCrSc[0], 0);
            m_arrMembers[51] = new CMember(52, m_arrNodes[31], m_arrNodes[30], m_arrCrSc[0], 0);
            m_arrMembers[52] = new CMember(53, m_arrNodes[32], m_arrNodes[29], m_arrCrSc[0], 0);
            m_arrMembers[53] = new CMember(54, m_arrNodes[33], m_arrNodes[32], m_arrCrSc[0], 0);
            m_arrMembers[54] = new CMember(55, m_arrNodes[34], m_arrNodes[33], m_arrCrSc[0], 0);
            m_arrMembers[55] = new CMember(56, m_arrNodes[35], m_arrNodes[34], m_arrCrSc[0], 0);
            m_arrMembers[56] = new CMember(57, m_arrNodes[26], m_arrNodes[36], m_arrCrSc[0], 0);
            m_arrMembers[57] = new CMember(58, m_arrNodes[27], m_arrNodes[37], m_arrCrSc[0], 0);
            m_arrMembers[58] = new CMember(59, m_arrNodes[29], m_arrNodes[39], m_arrCrSc[0], 0);
            m_arrMembers[59] = new CMember(60, m_arrNodes[30], m_arrNodes[40], m_arrCrSc[0], 0);
            m_arrMembers[60] = new CMember(61, m_arrNodes[32], m_arrNodes[42], m_arrCrSc[0], 0);
            m_arrMembers[61] = new CMember(62, m_arrNodes[43], m_arrNodes[32], m_arrCrSc[0], 0);
            m_arrMembers[62] = new CMember(63, m_arrNodes[33], m_arrNodes[42], m_arrCrSc[0], 0);
            m_arrMembers[63] = new CMember(64, m_arrNodes[33], m_arrNodes[43], m_arrCrSc[0], 0);
            m_arrMembers[64] = new CMember(65, m_arrNodes[34], m_arrNodes[44], m_arrCrSc[0], 0);
            m_arrMembers[65] = new CMember(66, m_arrNodes[37], m_arrNodes[36], m_arrCrSc[0], 0);
            m_arrMembers[66] = new CMember(67, m_arrNodes[38], m_arrNodes[37], m_arrCrSc[0], 0);
            m_arrMembers[67] = new CMember(68, m_arrNodes[39], m_arrNodes[36], m_arrCrSc[0], 0);
            m_arrMembers[68] = new CMember(69, m_arrNodes[40], m_arrNodes[37], m_arrCrSc[0], 0);
            m_arrMembers[69] = new CMember(70, m_arrNodes[40], m_arrNodes[39], m_arrCrSc[0], 0);
            m_arrMembers[70] = new CMember(71, m_arrNodes[41], m_arrNodes[40], m_arrCrSc[0], 0);
            m_arrMembers[71] = new CMember(72, m_arrNodes[42], m_arrNodes[39], m_arrCrSc[0], 0);
            m_arrMembers[72] = new CMember(73, m_arrNodes[43], m_arrNodes[42], m_arrCrSc[0], 0);
            m_arrMembers[73] = new CMember(74, m_arrNodes[44], m_arrNodes[43], m_arrCrSc[0], 0);
            m_arrMembers[74] = new CMember(75, m_arrNodes[45], m_arrNodes[44], m_arrCrSc[0], 0);
            m_arrMembers[75] = new CMember(76, m_arrNodes[39], m_arrNodes[46], m_arrCrSc[0], 0);
            m_arrMembers[76] = new CMember(77, m_arrNodes[42], m_arrNodes[47], m_arrCrSc[0], 0);
            m_arrMembers[77] = new CMember(78, m_arrNodes[43], m_arrNodes[48], m_arrCrSc[0], 0);
            m_arrMembers[78] = new CMember(79, m_arrNodes[44], m_arrNodes[49], m_arrCrSc[0], 0);
            m_arrMembers[79] = new CMember(80, m_arrNodes[36], m_arrNodes[50], m_arrCrSc[0], 0);
            m_arrMembers[80] = new CMember(81, m_arrNodes[37], m_arrNodes[51], m_arrCrSc[0], 0);
            m_arrMembers[81] = new CMember(82, m_arrNodes[40], m_arrNodes[54], m_arrCrSc[0], 0);
            m_arrMembers[82] = new CMember(83, m_arrNodes[43], m_arrNodes[56], m_arrCrSc[0], 0);
            m_arrMembers[83] = new CMember(84, m_arrNodes[58], m_arrNodes[42], m_arrCrSc[0], 0);
            m_arrMembers[84] = new CMember(85, m_arrNodes[46], m_arrNodes[53], m_arrCrSc[0], 0);
            m_arrMembers[85] = new CMember(86, m_arrNodes[47], m_arrNodes[56], m_arrCrSc[0], 0);
            m_arrMembers[86] = new CMember(87, m_arrNodes[48], m_arrNodes[58], m_arrCrSc[0], 0);
            m_arrMembers[87] = new CMember(88, m_arrNodes[49], m_arrNodes[60], m_arrCrSc[0], 0);
            m_arrMembers[88] = new CMember(89, m_arrNodes[51], m_arrNodes[50], m_arrCrSc[0], 0);
            m_arrMembers[89] = new CMember(90, m_arrNodes[52], m_arrNodes[51], m_arrCrSc[0], 0);
            m_arrMembers[90] = new CMember(91, m_arrNodes[53], m_arrNodes[50], m_arrCrSc[0], 0);
            m_arrMembers[91] = new CMember(92, m_arrNodes[54], m_arrNodes[51], m_arrCrSc[0], 0);
            m_arrMembers[92] = new CMember(93, m_arrNodes[54], m_arrNodes[53], m_arrCrSc[0], 0);
            m_arrMembers[93] = new CMember(94, m_arrNodes[55], m_arrNodes[54], m_arrCrSc[0], 0);
            m_arrMembers[94] = new CMember(95, m_arrNodes[56], m_arrNodes[53], m_arrCrSc[0], 0);
            m_arrMembers[95] = new CMember(96, m_arrNodes[57], m_arrNodes[56], m_arrCrSc[0], 0);
            m_arrMembers[96] = new CMember(97, m_arrNodes[58], m_arrNodes[56], m_arrCrSc[0], 0);
            m_arrMembers[97] = new CMember(98, m_arrNodes[58], m_arrNodes[57], m_arrCrSc[0], 0);
            m_arrMembers[98] = new CMember(99, m_arrNodes[56], m_arrNodes[59], m_arrCrSc[0], 0);
            m_arrMembers[99] = new CMember(100, m_arrNodes[59], m_arrNodes[58], m_arrCrSc[0], 0);
            m_arrMembers[100] = new CMember(101, m_arrNodes[60], m_arrNodes[58], m_arrCrSc[0], 0);
            m_arrMembers[101] = new CMember(102, m_arrNodes[61], m_arrNodes[60], m_arrCrSc[0], 0);



            // Setridit pole podle ID
            //Array.Sort(m_arrMembers, new CCompare_LineID());

            // Nodal Supports - fill values

            // Set values
            bool[] bSupport1 = { true, false, true, false, false, false };
            bool[] bSupport2 = { false, false, true, false, false, false };

            // Create Support Objects
            m_arrNSupports[0] = new CNSupport(6, 1, m_arrNodes[11], bSupport1, 0);
            m_arrNSupports[1] = new CNSupport(6, 2, m_arrNodes[61], bSupport1, 0);
            m_arrNSupports[2] = new CNSupport(6, 3, m_arrNodes[9], bSupport1, 0);
            m_arrNSupports[3] = new CNSupport(6, 4, m_arrNodes[59], bSupport1, 0);
            m_arrNSupports[4] = new CNSupport(6, 5, m_arrNodes[7], bSupport1, 0);
            m_arrNSupports[5] = new CNSupport(6, 6, m_arrNodes[57], bSupport1, 0);
            m_arrNSupports[6] = new CNSupport(6, 7, m_arrNodes[5], bSupport1, 0);
            m_arrNSupports[7] = new CNSupport(6, 8, m_arrNodes[21], bSupport1, 0);
            m_arrNSupports[8] = new CNSupport(6, 9, m_arrNodes[31], bSupport1, 0);
            m_arrNSupports[9] = new CNSupport(6, 10, m_arrNodes[41], bSupport1, 0);
            m_arrNSupports[10] = new CNSupport(6, 11, m_arrNodes[55], bSupport1, 0);
            m_arrNSupports[11] = new CNSupport(6, 12, m_arrNodes[2], bSupport1, 0);
            m_arrNSupports[12] = new CNSupport(6, 13, m_arrNodes[18], bSupport1, 0);
            m_arrNSupports[13] = new CNSupport(6, 14, m_arrNodes[28], bSupport1, 0);
            m_arrNSupports[14] = new CNSupport(6, 15, m_arrNodes[38], bSupport1, 0);
            m_arrNSupports[15] = new CNSupport(6, 16, m_arrNodes[52], bSupport1, 0);
            m_arrNSupports[16] = new CNSupport(6, 17, m_arrNodes[25], bSupport1, 0);
            m_arrNSupports[17] = new CNSupport(6, 18, m_arrNodes[35], bSupport1, 0);
            m_arrNSupports[18] = new CNSupport(6, 19, m_arrNodes[45], bSupport1, 0);


            // Setridit pole podle ID
            Array.Sort(m_arrNSupports, new CCompare_NSupportID());

            // Member Releases / hinges - fill values

            // Set values
            bool?[] bMembRelase1 = { false, false, false, false, true, false };

            // Create Release / Hinge Objects
            m_arrMembers[00].CnRelease1 = new CNRelease(0, bMembRelase1, 0);

            // Nodal Forces - fill values
            //arrForces[00] = new CNForce(m_arrNodes[00], -00.0f, 0.0f, -020.0f, 0);





        }
    }
}
