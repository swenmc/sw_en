using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BaseClasses;

namespace CENEX
{
	[Serializable]
    public class CTest2
    {

        public CNode[] arrNodes = new CNode[12];
        public CMember[] arrLines = new CMember[21];
        public CNSupport[] arrSupports = new CNSupport[2];
        //public CNForce[] arrForces = new CNForce[5];
		int eNDOF = (int)ENDOF.e3DEnv;

                public CTest2()
        {
            // Nodes Automatic Generation
            // Nodes List - Nodes Array

            // Upper Chord Nodes
            arrNodes[00] = new CNode(01, 000, 0, 00, 0);
            arrNodes[01] = new CNode(02, 100, 0, 00, 0);
            arrNodes[02] = new CNode(03, 200, 0, 00, 0);
            arrNodes[03] = new CNode(04, 300, 0, 00, 0);
            arrNodes[04] = new CNode(05, 400, 0, 00, 0);
            arrNodes[05] = new CNode(06, 500, 0, 00, 0);
            arrNodes[06] = new CNode(07, 600, 0, 00, 0);

            // Bottom Chord Nodes
            arrNodes[07] = new CNode(08, 100, 0, 40, 0);
            arrNodes[08] = new CNode(09, 200, 0, 60, 0);
            arrNodes[09] = new CNode(10, 300, 0, 70, 0);
            arrNodes[10] = new CNode(11, 400, 0, 60, 0);
            arrNodes[11] = new CNode(12, 500, 0, 40, 0);

            // Setridit pole podle ID
            //Array.Sort(arrNodes, new CCompare_NodeID());

            // Lines Automatic Generation
            // Lines List - Lines Array

            // Upper Chord Lines
            arrLines[00] = new CMember(01, arrNodes[0], arrNodes[1], 0);
            arrLines[01] = new CMember(02, arrNodes[1], arrNodes[2], 0);
            arrLines[02] = new CMember(03, arrNodes[2], arrNodes[3], 0);
            arrLines[03] = new CMember(04, arrNodes[3], arrNodes[4], 0);
            arrLines[04] = new CMember(05, arrNodes[4], arrNodes[5], 0);
            arrLines[05] = new CMember(06, arrNodes[5], arrNodes[6], 0);
            // Bottom Chord Lines
            arrLines[06] = new CMember(07, arrNodes[00], arrNodes[07], 0);
            arrLines[07] = new CMember(08, arrNodes[07], arrNodes[08], 0);
            arrLines[08] = new CMember(09, arrNodes[08], arrNodes[09], 0);
            arrLines[09] = new CMember(10, arrNodes[09], arrNodes[10], 0);
            arrLines[10] = new CMember(11, arrNodes[10], arrNodes[11], 0);
            arrLines[11] = new CMember(12, arrNodes[11], arrNodes[06], 0);
            // Diagonal Lines
            arrLines[12] = new CMember(13, arrNodes[01], arrNodes[07], 0);
            arrLines[13] = new CMember(14, arrNodes[01], arrNodes[08], 0);
            arrLines[14] = new CMember(15, arrNodes[02], arrNodes[08], 0);
            arrLines[15] = new CMember(16, arrNodes[02], arrNodes[09], 0);
            arrLines[16] = new CMember(17, arrNodes[03], arrNodes[09], 0);
            arrLines[17] = new CMember(18, arrNodes[04], arrNodes[09], 0);
            arrLines[18] = new CMember(19, arrNodes[04], arrNodes[10], 0);
            arrLines[19] = new CMember(20, arrNodes[05], arrNodes[10], 0);
            arrLines[20] = new CMember(21, arrNodes[05], arrNodes[11], 0);

            // Setridit pole podle ID
            //Array.Sort(arrLines, new CCompare_LineID());

            // Nodal Supports - fill values

            // Set values
            bool[] bSupport1 = { true, false, true, false, true, false };
            bool[] bSupport2 = { true, false, true, false, false, false };
            bool[] bSupport3 = { true, false, false, false, false, false };

            // Create Support Objects
            arrSupports[0] = new CNSupport(eNDOF, 1, arrNodes[0], bSupport2, 0);
			arrSupports[1] = new CNSupport(eNDOF, 2, arrNodes[6], bSupport3, 0);


            // Setridit pole podle ID
            Array.Sort(arrSupports, new CCompare_NSupportID());

            // Member Releases / hinges - fill values

            // Set values
            //bool[] bMembRelase1 = { true, false, false, false, false, false };
            //bool[] bMembRelase2 = { false, false, true, false, false, false };
            //bool[] bMembRelase3 = { false, false, false, false, true, false };
            bool?[] bMembRelase4 = { true, false, true, false, true, false };


            // Create Release / Hinge Objects
            arrLines[15].CnRelease1 = new CNRelease(0, bMembRelase4, 0);
			arrLines[17].CnRelease1 = new CNRelease(0, bMembRelase4, 0);

            // Nodal Forces - fill values

			//arrForces[0] = new CNForce(arrNodes[1], 0.0f, 0.0f, -050.0f, 0);
			//arrForces[1] = new CNForce(arrNodes[2], 0.0f, 0.0f, -100.0f, 0);
			//arrForces[2] = new CNForce(arrNodes[3], 0.0f, 0.0f, -100.0f, 0);
			//arrForces[3] = new CNForce(arrNodes[4], 0.0f, 0.0f, -100.0f, 0);
			//arrForces[4] = new CNForce(arrNodes[5], 0.0f, 0.0f, -050.0f, 0);
        }
    }
}
