﻿using BaseClasses;
using CRSC;
using MATERIAL;
using System.Collections.Generic;
using System.Windows.Media;

namespace Examples
{
    public class CExample_3D_902_OM : CExample
    {
        public CExample_3D_902_OM()
        {
            m_eSLN = ESLN.e3DD_1D; // 1D members in 3D model
            m_eNDOF = (int)ENDOF.e3DEnv; // DOF in 3D
            m_eGCS = EGCS.eGCSRightHanded; // Global coordinate system

            m_arrNodes = new CNode[4];
            m_arrMembers = new CMember[2];
            m_arrMat = new Dictionary<EMemberType_FS_Position, CMat>();
            m_arrCrSc = new Dictionary<EMemberType_FS_Position, CCrSc>();
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

            //m_arrCrSc[0] = new CCrSc_3_270XX_C(0, 0.27f, 0.10f, 0.008f, Colors.Orange);
            //m_arrCrSc[0] = new CCrSc_3_270XX_C_BACK_TO_BACK(0, 0.27f, 0.10f, 0.05f, 0.008f, Colors.Orange);
            //m_arrCrSc[0] = new CCrSc_3_50020_C(0, 0.5f, 0.20f, 0.008f, Colors.Orange);
            //m_arrCrSc[0] = new CCrSc_3_50020_C_NESTED(0, 0.5f, 0.20f, 0.008f, Colors.Orange);
            //m_arrCrSc[0] = new CCrSc_3_63020_BOX(0, 0.63f, 0.20f, 0.01f, 0.01f, Colors.Orange);
            //m_arrCrSc[0] = new CCrSc_3_270XX_C(0, 0.27f, 0.10f, 0.01f, Colors.Orange);
            //m_arrCrSc[0] = new CCrSc_3_270XX_C(0.27f, 0.07f, 0.00115f);
            //m_arrCrSc[0] = new CCrSc_3_Z(0, 0.5f, 0.2f, 0.05f, 0.02f, Colors.DarkCyan);

            // Pokusy
            //m_arrCrSc[0] = new CCrSc_3_51_C_TEMP(0,0.27f, 0.1f, 0.02f, Colors.DarkGreen);
            //m_arrCrSc[0] = new CCrSc_3_51_TRIANGLE_TEMP(0.866025f * 0.3f, 0.3f, 0.05f, Colors.DarkGreen);
            m_arrCrSc[0] = new CCrSc_3_FLAT(1, 0.1f, 0.001f);

            //m_arrCrSc[0].CSColor = Colors.Orange;

            // Nodes Automatic Generation
            // Nodes List - Nodes Array

            // Nodes

            //m_arrNodes[00] = new CNode(01, 1f, 1f, 0000.0f, 0);
            //m_arrNodes[01] = new CNode(02, 5f, 1f, 0000.0f, 0);

            //m_arrNodes[00] = new CNode(01, 1f, 3f, 1f, 0);
            //m_arrNodes[01] = new CNode(02, 1f, 1f, 2f, 0);

            m_arrNodes[00] = new CNode(01, 0f, 2f, 1f, 0);
            m_arrNodes[01] = new CNode(02, 0f, 1f, 2f, 0);

            m_arrNodes[02] = new CNode(03, 1f, 1f, 2f, 0);
            m_arrNodes[03] = new CNode(04, 1f, 2f, 1f, 0);

            // Setridit pole podle ID
            //Array.Sort(m_arrNodes, new CCompare_NodeID());

            // Member eccentricity

            //CMemberEccentricity eccmember = new CMemberEccentricity(-0.2f, -0.3f);
            CMemberEccentricity eccmember = new CMemberEccentricity(0.0f, 0.0f);
            // Members Automatic Generation
            // Members List - Members Array

            // Member Groups
            listOfModelMemberGroups = new List<CMemberGroup>(1);
            //listOfModelMemberGroups.Add(new CMemberGroup(1, "Column", EMemberType_FS.eWP, EMemberType_FS_Position.ColumnFrontSide, m_arrCrSc[0], 200, 200, 200, 0));
            listOfModelMemberGroups.Add(new CMemberGroup(1, "Girt", EMemberType_FS.eG, EMemberType_FS_Position.Girt, m_arrCrSc[0], 200, 200, 200, 0));
            listOfModelMemberGroups.Add(new CMemberGroup(2, "Girt", EMemberType_FS.eG, EMemberType_FS_Position.Girt, m_arrCrSc[0], 200, 200, 200, 0));

            // Members
            //m_arrMembers[000] = new CMember(001, m_arrNodes[00], m_arrNodes[01], m_arrCrSc[0], -0.2f, -0.2f, 0.74f, 0);
            //m_arrMembers[000] = new CMember(001, m_arrNodes[00], m_arrNodes[01], m_arrCrSc[0], -0.2f, -0.2f, 0, 0);
            //m_arrMembers[000] = new CMember(001, m_arrNodes[00], m_arrNodes[01], m_arrCrSc[0], 0, 0, 0f, 0);

            //m_arrMembers[000] = new CMember(001, m_arrNodes[00], m_arrNodes[01], m_arrCrSc[0], EMemberType_FS.eWP, EMemberType_FS_Position.ColumnFrontSide, eccmember, eccmember, -0.2f, -0.3f, 0.0f, 0);
            m_arrMembers[000] = new CMember(001, m_arrNodes[00], m_arrNodes[01], m_arrCrSc[0], EMemberType_FS.eG, EMemberType_FS_Position.Girt, eccmember, eccmember, 0.0f, 0.0f, 0.0f, 0);
            m_arrMembers[001] = new CMember(002, m_arrNodes[02], m_arrNodes[03], m_arrCrSc[0], EMemberType_FS.eG, EMemberType_FS_Position.Girt, eccmember, eccmember, 0.0f, 0.0f, 0.0f, 0);
            //m_arrMembers[001] = new CMember(002, m_arrNodes[02], m_arrNodes[03], m_arrCrSc[1], EMemberType_FS.eWP, eccmember, eccmember, 0.0f, 0.0f, 0.0f, 0);

            // Setridit pole podle ID
            //Array.Sort(m_arrMembers, new CCompare_LineID());

            // Nodal Supports - fill values

            // Set values
            bool[] bSupport1 = { true, false, true, true, false, false };
            bool[] bSupport2 = { false, false, true, true, false, false };

            // Create Support Objects
            //m_arrNSupports[0] = new CNSupport(6, 1, m_arrNodes[00], bSupport1, 0);
            //m_arrNSupports[1] = new CNSupport(6, 2, m_arrNodes[01], bSupport2, 0);

            // Setridit pole podle ID
            //Array.Sort(m_arrNSupports, new CCompare_NSupportID());

            // Member Releases / hinges - fill values

            // Set values
            bool?[] bMembRelase1 = { false, false, false, false, true, false };

            // Create Release / Hinge Objects
            //m_arrMembers[02].CnRelease1 = new CNRelease(6, m_arrMembers[02].NodeStart, bMembRelase1, 0);

            // Connection Joints
            m_arrConnectionJoints = new List<CConnectionJointTypes>();
            // Joints
            //m_arrConnectionJoints.Add(new CConnectionJoint_S001(m_arrMembers[000].NodeStart, null, m_arrMembers[0], true, true));
            //m_arrConnectionJoints.Add(new CConnectionJoint_S001(m_arrMembers[000].NodeEnd, null, m_arrMembers[0], false, true));
            //m_arrConnectionJoints.Add(new CConnectionJoint_T003("FB - LH", m_arrMembers[000].NodeStart, null, m_arrMembers[000], 0.003f, EPlateNumberAndPositionInJoint.eOneLeftPlate, true, true));
            //m_arrConnectionJoints.Add(new CConnectionJoint_T003("FB - LH", "FB - RH", m_arrMembers[000].NodeStart, null, m_arrMembers[000], 0.003f, EPlateNumberAndPositionInJoint.eTwoPlates, true, true));
            //m_arrConnectionJoints.Add(new CConnectionJoint_T003("FB - LH", "FB - RH", m_arrMembers[000].NodeEnd, null, m_arrMembers[000], 0.003f, EPlateNumberAndPositionInJoint.eTwoPlates, true, true));
            //m_arrConnectionJoints.Add(new CConnectionJoint_S001(m_arrMembers[000].NodeStart, null, m_arrMembers[000], 0.3f, false, true, true));
            //m_arrConnectionJoints.Add(new CConnectionJoint_S001(m_arrMembers[000].NodeEnd, null, m_arrMembers[000], 0.2f, false, true, true));
            m_arrConnectionJoints.Add(new CConnectionJoint_U001(EJointType.eCrossBracing_MainColumn, m_arrMembers[000].NodeStart, null, m_arrMembers[000], true));
            m_arrConnectionJoints.Add(new CConnectionJoint_U001(EJointType.eCrossBracing_MainColumn, m_arrMembers[000].NodeEnd, null, m_arrMembers[000], true));

            m_arrConnectionJoints.Add(new CConnectionJoint_U001(EJointType.eCrossBracing_MainColumn, m_arrMembers[001].NodeStart, null, m_arrMembers[001], true));
            m_arrConnectionJoints.Add(new CConnectionJoint_U001(EJointType.eCrossBracing_MainColumn, m_arrMembers[001].NodeEnd, null, m_arrMembers[001], true));

            // Nodal loads
            m_arrNLoads = new CNLoad[1];
            m_arrNLoads[0] = new CNLoadSingle(1, m_arrNodes[00], ENLoadType.eNLT_Fx, 20, true, 0);

            // Member loads
            m_arrMLoads = new CMLoad[2];
            //m_arrMLoads[0] = new CMLoad_21(1, -150, m_arrMembers[0], EMLoadTypeDistr.eMLT_QUF_W_21, ELoadType.eLT_F, ELoadCoordSystem.eLCS, ELoadDirection.eLD_Y, true, 0);
            //m_arrMLoads[1] = new CMLoad_22(2, 160, 0.3f * m_arrMembers[0].FLength, m_arrMembers[0], EMLoadTypeDistr.eMLT_QUF_PA_22, ELoadType.eLT_F, ELoadCoordSystem.eLCS, ELoadDirection.eLD_Y, true, 0);
            //m_arrMLoads[2] = new CMLoad_23(3, -250, 0.3f * m_arrMembers[0].FLength, m_arrMembers[0], EMLoadTypeDistr.eMLT_QUF_PB_23, ELoadType.eLT_F, ELoadCoordSystem.eLCS, ELoadDirection.eLD_Z, true, 0);
            //m_arrMLoads[3] = new CMLoad_24(4, 260, 0.3f * m_arrMembers[0].FLength, 0.6f * m_arrMembers[0].FLength, m_arrMembers[0], EMLoadTypeDistr.eMLT_QUF_PG_24, ELoadType.eLT_F, ELoadCoordSystem.eLCS, ELoadDirection.eLD_Z, true, 0);
            m_arrMLoads[0] = new CMLoad_24(1, -250, 0.3f * m_arrMembers[0].FLength, 0.6f * m_arrMembers[0].FLength, m_arrMembers[0], EMLoadTypeDistr.eMLT_QUF_PG_24, ELoadType.eLT_F, ELoadCoordSystem.eLCS, ELoadDirection.eLD_Y, true, 0);
            m_arrMLoads[1] = new CMLoad_24(2, -250, 0.3f * m_arrMembers[1].FLength, 0.6f * m_arrMembers[1].FLength, m_arrMembers[1], EMLoadTypeDistr.eMLT_QUF_PG_24, ELoadType.eLT_F, ELoadCoordSystem.eLCS, ELoadDirection.eLD_Y, true, 0);

            m_arrLoadCases = new CLoadCase[1];
            m_arrLoadCases[0] = new CLoadCase(1, "LC1", ELCGTypeForLimitState.eUniversal, ELCType.ePermanentLoad, ELCMainDirection.eGeneral, new List<CNLoad> { m_arrNLoads[0] }, new List<CMLoad> { m_arrMLoads[0], m_arrMLoads[1]/*, m_arrMLoads[2], m_arrMLoads[3]*/ });
        }
    }
}
