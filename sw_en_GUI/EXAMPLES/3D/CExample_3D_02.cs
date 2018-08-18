using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BaseClasses;
using MATERIAL;
using CRSC;
using BaseClasses.CRSC;

namespace sw_en_GUI.EXAMPLES._3D
{
    class CExample_3D_02 : CExample
    {
        /*
        public CNode[] m_arrNodes = new CNode[12];
        public CMember[] m_arrMembers = new CMember[21];
        public CNSupport[] arrSupports = new CNSupport[2];
        //public CNForce[] arrForces = new CNForce[5];
        int eNDOF = (int)ENDOF.e3DEnv;
        */

        public CExample_3D_02()
        {
            m_eSLN = ESLN.e3DD_1D; // 1D members in 3D model
            m_eNDOF = (int)ENDOF.e3DEnv; // DOF in 3D
            m_eGCS = EGCS.eGCSLeftHanded; // Global coordinate system

            m_arrNodes = new BaseClasses.CNode[12];
            m_arrMembers = new CMember[21];
            m_arrMat = new CMat[1];
            m_arrCrSc = new CCrSc[1];
            m_arrNSupports = new BaseClasses.CNSupport[2];
            //m_arrNLoads = new BaseClasses.CNLoad[5];

            // Materials
            // Materials List - Materials Array - Fill Data of Materials Array
            m_arrMat[0] = new CMat_03_00();

            // Cross-sections
            // CrSc List - CrSc Array - Fill Data of Cross-sections Array
            m_arrCrSc[0] = new CCrSc_0_05(0.1f, 0.05f);

            // Nodes Automatic Generation
            // Nodes List - Nodes Array

            // Upper Chord Nodes
            m_arrNodes[00] = new CNode(01, 0, 0, 0, 0);
            m_arrNodes[01] = new CNode(02, 1, 0, 0, 0);
            m_arrNodes[02] = new CNode(03, 2, 0, 0, 0);
            m_arrNodes[03] = new CNode(04, 3, 0, 0, 0);
            m_arrNodes[04] = new CNode(05, 4, 0, 0, 0);
            m_arrNodes[05] = new CNode(06, 5, 0, 0, 0);
            m_arrNodes[06] = new CNode(07, 6, 0, 0, 0);

            // Bottom Chord Nodes
            m_arrNodes[07] = new CNode(08, 1, 0, 4, 0);
            m_arrNodes[08] = new CNode(09, 2, 0, 6, 0);
            m_arrNodes[09] = new CNode(10, 3, 0, 7, 0);
            m_arrNodes[10] = new CNode(11, 4, 0, 6, 0);
            m_arrNodes[11] = new CNode(12, 5, 0, 4, 0);

            // Setridit pole podle ID
            //Array.Sort(m_arrNodes, new CCompare_NodeID());

            // Lines Automatic Generation
            // Lines List - Lines Array

            // Upper Chord Lines
            m_arrMembers[00] = new CMember(01, m_arrNodes[0], m_arrNodes[1], m_arrCrSc[0], 0);
            m_arrMembers[01] = new CMember(02, m_arrNodes[1], m_arrNodes[2], m_arrCrSc[0], 0);
            m_arrMembers[02] = new CMember(03, m_arrNodes[2], m_arrNodes[3], m_arrCrSc[0], 0);
            m_arrMembers[03] = new CMember(04, m_arrNodes[3], m_arrNodes[4], m_arrCrSc[0], 0);
            m_arrMembers[04] = new CMember(05, m_arrNodes[4], m_arrNodes[5], m_arrCrSc[0], 0);
            m_arrMembers[05] = new CMember(06, m_arrNodes[5], m_arrNodes[6], m_arrCrSc[0], 0);
            // Bottom Chord Lines
            m_arrMembers[06] = new CMember(07, m_arrNodes[00], m_arrNodes[07], m_arrCrSc[0], 0);
            m_arrMembers[07] = new CMember(08, m_arrNodes[07], m_arrNodes[08], m_arrCrSc[0], 0);
            m_arrMembers[08] = new CMember(09, m_arrNodes[08], m_arrNodes[09], m_arrCrSc[0], 0);
            m_arrMembers[09] = new CMember(10, m_arrNodes[09], m_arrNodes[10], m_arrCrSc[0], 0);
            m_arrMembers[10] = new CMember(11, m_arrNodes[10], m_arrNodes[11], m_arrCrSc[0], 0);
            m_arrMembers[11] = new CMember(12, m_arrNodes[11], m_arrNodes[06], m_arrCrSc[0], 0);
            // Diagonal Lines
            m_arrMembers[12] = new CMember(13, m_arrNodes[01], m_arrNodes[07], m_arrCrSc[0], 0);
            m_arrMembers[13] = new CMember(14, m_arrNodes[01], m_arrNodes[08], m_arrCrSc[0], 0);
            m_arrMembers[14] = new CMember(15, m_arrNodes[02], m_arrNodes[08], m_arrCrSc[0], 0);
            m_arrMembers[15] = new CMember(16, m_arrNodes[02], m_arrNodes[09], m_arrCrSc[0], 0);
            m_arrMembers[16] = new CMember(17, m_arrNodes[03], m_arrNodes[09], m_arrCrSc[0], 0);
            m_arrMembers[17] = new CMember(18, m_arrNodes[04], m_arrNodes[09], m_arrCrSc[0], 0);
            m_arrMembers[18] = new CMember(19, m_arrNodes[04], m_arrNodes[10], m_arrCrSc[0], 0);
            m_arrMembers[19] = new CMember(20, m_arrNodes[05], m_arrNodes[10], m_arrCrSc[0], 0);
            m_arrMembers[20] = new CMember(21, m_arrNodes[05], m_arrNodes[11], m_arrCrSc[0], 0);

            // Setridit pole podle ID
            //Array.Sort(m_arrMembers, new CCompare_LineID());

            // Nodal Supports - fill values

            // Set values
            bool[] bSupport1 = { true, false, true, false, true, false };
            bool[] bSupport2 = { true, false, true, false, false, false };
            bool[] bSupport3 = { true, false, false, false, false, false };

            // Create Support Objects
            m_arrNSupports[0] = new BaseClasses.CNSupport(6, 1, m_arrNodes[0], bSupport2, 0);
            m_arrNSupports[1] = new BaseClasses.CNSupport(6, 2, m_arrNodes[6], bSupport3, 0);


            // Setridit pole podle ID
            Array.Sort(m_arrNSupports, new CCompare_NSupportID());

            // Member Releases / hinges - fill values

            // Set values
            //bool[] bMembRelase1 = { true, false, false, false, false, false };
            //bool[] bMembRelase2 = { false, false, true, false, false, false };
            //bool[] bMembRelase3 = { false, false, false, false, true, false };
            bool?[] bMembRelase4 = { true, false, true, false, true, false };


            // Create Release / Hinge Objects
            m_arrMembers[15].CnRelease1 = new CNRelease(0, bMembRelase4, 0);
            m_arrMembers[17].CnRelease1 = new CNRelease(0, bMembRelase4, 0);

            // Nodal Forces - fill values

            //arrForces[0] = new CNForce(m_arrNodes[1], 0.0f, 0.0f, -050.0f, 0);
            //arrForces[1] = new CNForce(m_arrNodes[2], 0.0f, 0.0f, -100.0f, 0);
            //arrForces[2] = new CNForce(m_arrNodes[3], 0.0f, 0.0f, -100.0f, 0);
            //arrForces[3] = new CNForce(m_arrNodes[4], 0.0f, 0.0f, -100.0f, 0);
            //arrForces[4] = new CNForce(m_arrNodes[5], 0.0f, 0.0f, -050.0f, 0);
        }
    }
}
