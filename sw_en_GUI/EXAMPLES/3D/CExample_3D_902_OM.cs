using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using BaseClasses;
using MATERIAL;
using CRSC;

namespace sw_en_GUI.EXAMPLES._3D
{
    public class CExample_3D_902_OM : CExample
    {
        public CExample_3D_902_OM()
        {
            m_eSLN = ESLN.e3DD_1D; // 1D members in 3D model
            m_eNDOF = (int)ENDOF.e3DEnv; // DOF in 3D
            m_eGCS = EGCS.eGCSLeftHanded; // Global coordinate system

            m_arrNodes = new CNode[2];
            m_arrMembers = new CMember[1];
            m_arrMat = new CMat[1];
            m_arrCrSc = new CCrSc[1];
            m_arrNSupports = new CNSupport[2];
            //m_arrNLoads = new BaseClasses.CNLoad[35];

            // Materials
            // Materials List - Materials Array - Fill Data of Materials Array
            m_arrMat[0] = new CMat_03_00();

            // Cross-sections
            // CrSc List - CrSc Array - Fill Data of Cross-sections Array
            //m_arrCrSc[0] = new CCrSc_3_07(0, 0.5f, 0.2f, 0.020f);
            //m_arrCrSc[0] = new CCrSc_0_05(0.5f, 0.2f);
            //m_arrCrSc[0] = new CCrSc_3_51_C_LIP2_FS50020(1f, 0.3f, 0.01f, 0.05f, 0.005f);
            //m_arrCrSc[0] = new CCrSc_3_51_BOX_TEMP(1f, 0.3f, 0.003f);

            //m_arrCrSc[0] = new CCrSc_3_51_TRIANGLE_TEMP(0.866025f * 0.3f, 0.3f, 0.05f);
            m_arrCrSc[0] = new CCrSc_3_270XX_C(0.27f, 0.10f, 0.001f, Colors.Orange);
            //m_arrCrSc[0] = new CCrSc_3_50020_C(0.5f, 0.20f, 0.001f, Colors.Orange);
            //m_arrCrSc[0] = new CCrSc_3_63020_BOX(0.63f, 0.20f, 0.001f, 0.001f, Colors.Orange);

            //m_arrCrSc[0].CSColor = Colors.Orange;

            // Nodes Automatic Generation
            // Nodes List - Nodes Array

            // Nodes
            //m_arrNodes[00] = new CNode(01, 1.5f, 0.5f, 0000.0f, 0);
            //m_arrNodes[01] = new CNode(02, 2.5f, 0.5f, 0001.0f, 0);
            m_arrNodes[00] = new CNode(01, 0, 0, 0000.0f, 0);
            m_arrNodes[01] = new CNode(02, 1, 0, 0000.0f, 0);

            // Setridit pole podle ID
            //Array.Sort(m_arrNodes, new CCompare_NodeID());

            // Members Automatic Generation
            // Members List - Members Array

            // Members
            //m_arrMembers[000] = new CMember(001, m_arrNodes[00], m_arrNodes[01], m_arrCrSc[0], -0.2f, -0.2f, 0.74f, 0);
            //m_arrMembers[000] = new CMember(001, m_arrNodes[00], m_arrNodes[01], m_arrCrSc[0], -0.2f, -0.2f, 0, 0);
            m_arrMembers[000] = new CMember(001, m_arrNodes[00], m_arrNodes[01], m_arrCrSc[0], 0, 0, 0f, 0);

            // Setridit pole podle ID
            //Array.Sort(m_arrMembers, new CCompare_LineID());

            // Nodal Supports - fill values

            // Set values
            bool[] bSupport1 = { true, false, true, true, false, false };
            bool[] bSupport2 = { false, false, true, true, false, false };

            // Create Support Objects
            m_arrNSupports[0] = new CNSupport(6, 1, m_arrNodes[00], bSupport1, 0);
            m_arrNSupports[1] = new CNSupport(6, 2, m_arrNodes[01], bSupport2, 0);

            // Setridit pole podle ID
            Array.Sort(m_arrNSupports, new CCompare_NSupportID());

            // Member Releases / hinges - fill values

            // Set values
            bool?[] bMembRelase1 = { false, false, false, false, true, false };

            // Create Release / Hinge Objects
            //m_arrMembers[02].CnRelease1 = new CNRelease(6, m_arrMembers[02].NodeStart, bMembRelase1, 0);

            // Connection Joints
            m_arrConnectionJoints = new List<CConnectionJointTypes>();
            // Joints
            //m_arrConnectionJoints.Add(new CConnectionJoint_TA01(m_arrMembers[000].NodeStart, m_arrMembers[0], true));
            //m_arrConnectionJoints.Add(new CConnectionJoint_TA01(m_arrMembers[000].NodeEnd, m_arrMembers[0], true));
            m_arrConnectionJoints.Add(new CConnectionJoint_T001(m_arrMembers[000].NodeStart, m_arrMembers[000], m_arrMembers[000], false, true));
        }
    }
}
