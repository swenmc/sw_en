using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BaseClasses;

namespace CENEX
{
    public class CTest4
    {
        public CNode[] arrNodes = new CNode[22];
		public CMember[] arrLines = new CMember[41];
        public CNSupport[] arrSupports = new CNSupport[2];
       //public CNForce[] arrForces = new CNForce[11];
		int eNDOF = (int)ENDOF.e3DEnv;

        public CTest4()
        {
            // Nodes Automatic Generation
            // Nodes List - Nodes Array

            // 1-bottom chord
            arrNodes[00] = new CNode(01, 00000, 0, 12000, 0);
            arrNodes[01] = new CNode(02, 08000, 0, 12000, 0);
            arrNodes[02] = new CNode(03, 16000, 0, 12000, 0);
            arrNodes[03] = new CNode(04, 24000, 0, 12000, 0);
            arrNodes[04] = new CNode(05, 32000, 0, 12000, 0);
            arrNodes[05] = new CNode(06, 40000, 0, 12000, 0);
            arrNodes[06] = new CNode(07, 48000, 0, 12000, 0);
            arrNodes[07] = new CNode(08, 56000, 0, 12000, 0);
            arrNodes[08] = new CNode(09, 64000, 0, 12000, 0);
            arrNodes[09] = new CNode(10, 72000, 0, 12000, 0);
            arrNodes[10] = new CNode(11, 80000, 0, 12000, 0);
            // 2-upper chord
            arrNodes[11] = new CNode(12, 00000, 0, 08000, 0);
            arrNodes[12] = new CNode(13, 08000, 0, 05900, 0);
            arrNodes[13] = new CNode(14, 16000, 0, 04000, 0);
            arrNodes[14] = new CNode(15, 24000, 0, 03000, 0);
            arrNodes[15] = new CNode(16, 32000, 0, 02300, 0);
            arrNodes[16] = new CNode(17, 40000, 0, 02000, 0);
            arrNodes[17] = new CNode(18, 48000, 0, 02300, 0);
            arrNodes[18] = new CNode(19, 56000, 0, 03000, 0);
            arrNodes[19] = new CNode(20, 64000, 0, 04000, 0);
            arrNodes[20] = new CNode(21, 72000, 0, 05900, 0);
            arrNodes[21] = new CNode(22, 80000, 0, 08000, 0);

            // Setridit pole podle ID
            //Array.Sort(arrNodes, new CCompare_NodeID());

            // Lines Automatic Generation
            // Lines List - Lines Array

            // 1-bottom chord
            arrLines[00] = new CMember(01, arrNodes[00], arrNodes[01], 0);
            arrLines[01] = new CMember(02, arrNodes[01], arrNodes[02], 0);
            arrLines[02] = new CMember(03, arrNodes[02], arrNodes[03], 0);
            arrLines[03] = new CMember(04, arrNodes[03], arrNodes[04], 0);
            arrLines[04] = new CMember(05, arrNodes[04], arrNodes[05], 0);
            arrLines[05] = new CMember(06, arrNodes[05], arrNodes[06], 0);
            arrLines[06] = new CMember(07, arrNodes[06], arrNodes[07], 0);
            arrLines[07] = new CMember(08, arrNodes[07], arrNodes[08], 0);
            arrLines[08] = new CMember(09, arrNodes[08], arrNodes[09], 0);
            arrLines[09] = new CMember(10, arrNodes[09], arrNodes[10], 0);
            // 2-upper chord
            arrLines[10] = new CMember(11, arrNodes[11], arrNodes[12], 0);
            arrLines[11] = new CMember(12, arrNodes[12], arrNodes[13], 0);
            arrLines[12] = new CMember(13, arrNodes[13], arrNodes[14], 0);
            arrLines[13] = new CMember(14, arrNodes[14], arrNodes[15], 0);
            arrLines[14] = new CMember(15, arrNodes[15], arrNodes[16], 0);
            arrLines[15] = new CMember(16, arrNodes[16], arrNodes[17], 0);
            arrLines[16] = new CMember(17, arrNodes[17], arrNodes[18], 0);
            arrLines[17] = new CMember(18, arrNodes[18], arrNodes[19], 0);
            arrLines[18] = new CMember(19, arrNodes[19], arrNodes[20], 0);
            arrLines[19] = new CMember(20, arrNodes[20], arrNodes[21], 0);
            // 3-stops / columns / verticals
            arrLines[20] = new CMember(21, arrNodes[00], arrNodes[11], 0);
            arrLines[21] = new CMember(22, arrNodes[01], arrNodes[12], 0);
            arrLines[22] = new CMember(23, arrNodes[02], arrNodes[13], 0);
            arrLines[23] = new CMember(24, arrNodes[03], arrNodes[14], 0);
            arrLines[24] = new CMember(25, arrNodes[04], arrNodes[15], 0);
            arrLines[25] = new CMember(26, arrNodes[05], arrNodes[16], 0);
            arrLines[26] = new CMember(27, arrNodes[06], arrNodes[17], 0);
            arrLines[27] = new CMember(28, arrNodes[07], arrNodes[18], 0);
            arrLines[28] = new CMember(29, arrNodes[08], arrNodes[19], 0);
            arrLines[29] = new CMember(30, arrNodes[09], arrNodes[20], 0);
            arrLines[30] = new CMember(31, arrNodes[10], arrNodes[21], 0);
            // 4-diagonals
            arrLines[31] = new CMember(32, arrNodes[01], arrNodes[11], 0);
            arrLines[32] = new CMember(33, arrNodes[02], arrNodes[12], 0);
            arrLines[33] = new CMember(34, arrNodes[03], arrNodes[13], 0);
            arrLines[34] = new CMember(35, arrNodes[04], arrNodes[14], 0);
            arrLines[35] = new CMember(36, arrNodes[05], arrNodes[15], 0);
            arrLines[36] = new CMember(37, arrNodes[05], arrNodes[17], 0);
            arrLines[37] = new CMember(38, arrNodes[06], arrNodes[18], 0);
            arrLines[38] = new CMember(39, arrNodes[07], arrNodes[19], 0);
            arrLines[39] = new CMember(40, arrNodes[08], arrNodes[20], 0);
            arrLines[40] = new CMember(41, arrNodes[09], arrNodes[21], 0);

            // Setridit pole podle ID
            //Array.Sort(arrLines, new CCompare_LineID());

            // Nodal Supports - fill values

            // Set values
            bool[] bSupport1 = { true, false, true, false, false, false };
            bool[] bSupport2 = { false, false, true, false, false, false };

            // Create Support Objects
            arrSupports[0] = new CNSupport(eNDOF, 1, arrNodes[00], bSupport1, 0);
			arrSupports[1] = new CNSupport(eNDOF, 2, arrNodes[10], bSupport2, 0);

            // Setridit pole podle ID
            Array.Sort(arrSupports, new CCompare_NSupportID());

            // Member Releases / hinges - fill values

            // Set values
            bool?[] bMembRelase1 = { false, false, false, false, true, false };

            // Create Release / Hinge Objects
            arrLines[31].CnRelease1 = new CNRelease(0, bMembRelase1, 0);
			arrLines[32].CnRelease1 = new CNRelease(0, bMembRelase1, 0);
			arrLines[33].CnRelease1 = new CNRelease(0, bMembRelase1, 0);
			arrLines[34].CnRelease1 = new CNRelease(0, bMembRelase1, 0);
			arrLines[35].CnRelease1 = new CNRelease(0, bMembRelase1, 0);
			arrLines[36].CnRelease1 = new CNRelease(0, bMembRelase1, 0);
			arrLines[37].CnRelease1 = new CNRelease(0, bMembRelase1, 0);
			arrLines[38].CnRelease1 = new CNRelease(0, bMembRelase1, 0);
			arrLines[39].CnRelease1 = new CNRelease(0, bMembRelase1, 0);
			arrLines[40].CnRelease1 = new CNRelease(0, bMembRelase1, 0);

            // Nodal Forces - fill values
			//arrForces[00] = new CNForce(arrNodes[00], -00.0f, 0.0f, -050.0f, 0);
			//arrForces[01] = new CNForce(arrNodes[01], -00.0f, 0.0f, -100.0f, 0);
			//arrForces[02] = new CNForce(arrNodes[02], -00.0f, 0.0f, -100.0f, 0);
			//arrForces[03] = new CNForce(arrNodes[03], -00.0f, 0.0f, -100.0f, 0);
			//arrForces[04] = new CNForce(arrNodes[04], -00.0f, 0.0f, -100.0f, 0);
			//arrForces[05] = new CNForce(arrNodes[05], -00.0f, 0.0f, -100.0f, 0);
			//arrForces[06] = new CNForce(arrNodes[06], -00.0f, 0.0f, -100.0f, 0);
			//arrForces[07] = new CNForce(arrNodes[07], -00.0f, 0.0f, -100.0f, 0);
			//arrForces[08] = new CNForce(arrNodes[08], -00.0f, 0.0f, -100.0f, 0);
			//arrForces[09] = new CNForce(arrNodes[09], -00.0f, 0.0f, -100.0f, 0);
			//arrForces[10] = new CNForce(arrNodes[10], -00.0f, 0.0f, -050.0f, 0);
        }
    }
}