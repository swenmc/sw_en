using BaseClasses;
using CRSC;
using MATERIAL;
using System;

namespace Examples
{
    class CExample_2D_01 : CExample
    {
        /*
        public BaseClasses.CNode[] m_arrNodes = new BaseClasses.CNode[6];
        public CMember[] arrMembers = new CMember[9];
        public CMat[] arrMat = new CMat[5];
        public CRSC.CCrSc[] m_arrCrSc = new CRSC.CCrSc[3];
        public BaseClasses.CNSupport[] arrNSupports = new BaseClasses.CNSupport[3];
        public BaseClasses.CNLoad[] arrNLoads = new BaseClasses.CNLoad[3];
        */

        public CExample_2D_01()
        {
            m_eSLN = ESLN.e2DD_1D; // 1D members in 2D model
            m_eNDOF = (int)ENDOF.e2DEnv; // DOF in 2D
            m_eGCS = EGCS.eGCSLeftHanded; // Global coordinate system

            m_arrNodes = new CNode[6];
            m_arrMembers = new CMember[9];
            m_arrMat = new System.Collections.Generic.Dictionary<EMemberType_FS_Position, CMat>(); // CMat[1];
            m_arrCrSc = new System.Collections.Generic.Dictionary<EMemberType_FS_Position, CCrSc>();// new CCrSc[1];
            m_arrNSupports = new CNSupport[3];
            //m_arrNLoads = new BaseClasses.CNLoad[3];

            // Materials
            // Materials List - Materials Array - Fill Data of Materials Array
            m_arrMat[0] = new CMat_03_00();

            // Cross-sections
            // CrSc List - CrSc Array - Fill Data of Cross-sections Array
            m_arrCrSc[0] = new CCrSc_0_05(0.1f, 0.05f);

            // Nodes
            // Nodes List - Nodes Array

            m_arrNodes[0] = new CNode(1, 0.500f, 0, 2.500f, 0);
            m_arrNodes[1] = new CNode(2, 2.500f, 0, 2.500f, 0);
            m_arrNodes[2] = new CNode(3, 5.500f, 0, 2.500f, 0);
            m_arrNodes[3] = new CNode(4, 0.500f, 0, 0.500f, 0);
            m_arrNodes[4] = new CNode(5, 2.500f, 0, 0.500f, 0);
            m_arrNodes[5] = new CNode(6, 5.500f, 0, 0.500f, 0);

            // Sort by ID
            //Array.Sort(m_arrNodes, new BaseClasses.CCompare_NodeID());

            // Members
            // Members List - Members Array

            m_arrMembers[0] = new CMember(1, m_arrNodes[0], m_arrNodes[1], m_arrCrSc[0], 0);
            m_arrMembers[1] = new CMember(2, m_arrNodes[1], m_arrNodes[2], m_arrCrSc[0], 0);
            m_arrMembers[2] = new CMember(3, m_arrNodes[0], m_arrNodes[3], m_arrCrSc[0], 0);
            m_arrMembers[3] = new CMember(4, m_arrNodes[1], m_arrNodes[4], m_arrCrSc[0], 0);
            m_arrMembers[4] = new CMember(5, m_arrNodes[2], m_arrNodes[5], m_arrCrSc[0], 0);
            m_arrMembers[5] = new CMember(6, m_arrNodes[3], m_arrNodes[4], m_arrCrSc[0], 0);
            m_arrMembers[6] = new CMember(7, m_arrNodes[4], m_arrNodes[5], m_arrCrSc[0], 0);
            m_arrMembers[7] = new CMember(8, m_arrNodes[1], m_arrNodes[3], m_arrCrSc[0], 0);
            m_arrMembers[8] = new CMember(9, m_arrNodes[1], m_arrNodes[5], m_arrCrSc[0], 0);

            //Sort by ID
            //Array.Sort(m_arrMembers, new BaseClasses.CCompare_MemberID());

            // Nodal Supports - fill values
            // Set values
            bool[] bSupport1 = { true, false, true, false, true, false };
            bool[] bSupport2 = { false, false, true, false, true, false };
            bool[] bSupport3 = { true, false, false, false, false, false };

            // Create Support Objects
            // Pozn. Jednym z parametrov by malo byt pole ID uzlov v ktorych je zadefinovana tato podpora
            // objekt podpory bude len jeden a dotknute uzly budu vediet ze na ich podpora existuje a ake je konkretne ID jej nastaveni
            m_arrNSupports[0] = new CNSupport(6, 1, m_arrNodes[0], bSupport1, 0);
            m_arrNSupports[1] = new CNSupport(6, 2, m_arrNodes[2], bSupport2, 0);
            m_arrNSupports[2] = new CNSupport(6, 3, m_arrNodes[5], bSupport3, 0);

            // Sort by ID
            Array.Sort(m_arrNSupports, new CCompare_NSupportID());
        }
    }
}
