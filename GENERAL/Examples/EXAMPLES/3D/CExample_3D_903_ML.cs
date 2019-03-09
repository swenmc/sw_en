using BaseClasses;
using MATH;
using CRSC;
using MATERIAL;
using System.Collections.Generic;
using System.Windows.Media;

namespace Examples
{
    public class CExample_3D_903_ML : CExample
    {
        public CExample_3D_903_ML()
        {
            // Test - Member Loads
            m_eSLN = ESLN.e3DD_1D; // 1D members in 3D model
            m_eNDOF = (int)ENDOF.e3DEnv; // DOF in 3D
            m_eGCS = EGCS.eGCSLeftHanded; // Global coordinate system

            m_arrNodes = new CNode[8];
            m_arrMembers = new CMember[4];
            m_arrMat = new CMat[1];
            m_arrCrSc = new CCrSc[1];
            m_arrNSupports = new CNSupport[2];
            //m_arrNLoads = new BaseClasses.CNLoad[35];

            // Materials
            // Materials List - Materials Array - Fill Data of Materials Array
            m_arrMat[0] = new CMat_03_00();

            // Cross-sections
            // CrSc List - CrSc Array - Fill Data of Cross-sections Array

            m_arrCrSc[0] = new CCrSc_3_270XX_C(1, 0.27f, 0.070f, 0.00115f, Colors.Orange);

            // Nodes Automatic Generation
            // Nodes List - Nodes Array

            // Nodes

            m_arrNodes[00] = new CNode(01, 1f, 1f, 0f, 0);
            m_arrNodes[01] = new CNode(02, 5f, 1f, 0f, 0);

            m_arrNodes[02] = new CNode(02, 1f, 2f, 0f, 0);
            m_arrNodes[03] = new CNode(03, 4f, 2f, 0f, 0);

            m_arrNodes[04] = new CNode(04, 2f, 3f, 0f, 0);
            m_arrNodes[05] = new CNode(05, 5f, 3f, 0f, 0);

            m_arrNodes[06] = new CNode(06, 3f, 4f, 0f, 0);
            m_arrNodes[07] = new CNode(07, 5f, 4f, 0f, 0);

            // Member eccentricity

            //CMemberEccentricity eccmember = new CMemberEccentricity(-0.2f, -0.3f);
            CMemberEccentricity eccmember = new CMemberEccentricity(0f, 0f);
            // Members Automatic Generation
            // Members List - Members Array

            // Member Groups
            listOfModelMemberGroups = new List<CMemberGroup>(1);
            listOfModelMemberGroups.Add(new CMemberGroup(1, "Column", m_arrCrSc[0], 0));

            // Members
            m_arrMembers[000] = new CMember(001, m_arrNodes[01], m_arrNodes[00], m_arrCrSc[0], EMemberType_FS.eP, eccmember, eccmember, 0.0f, 0.0f, MathF.fPI, 0); // Opacny smer ako x plochy
            m_arrMembers[001] = new CMember(002, m_arrNodes[02], m_arrNodes[03], m_arrCrSc[0], EMemberType_FS.eP, eccmember, eccmember, 0.0f, 0.0f, MathF.fPI, 0);
            m_arrMembers[002] = new CMember(003, m_arrNodes[04], m_arrNodes[05], m_arrCrSc[0], EMemberType_FS.eP, eccmember, eccmember, 0.0f, 0.0f, MathF.fPI, 0);
            m_arrMembers[003] = new CMember(004, m_arrNodes[06], m_arrNodes[07], m_arrCrSc[0], EMemberType_FS.eP, eccmember, eccmember, 0.0f, 0.0f, MathF.fPI, 0);

            //List<CMember> listOfAllMembers = new List<CMember>() { m_arrMembers[000], m_arrMembers[001], m_arrMembers[002], m_arrMembers[003] };

            // Nodal Supports - fill values

            // Set values
            bool[] bSupport1 = { true, false, true, true, false, false };
            bool[] bSupport2 = { false, false, true, true, false, false };

            // Create Support Objects
            //m_arrNSupports[0] = new CNSupport(6, 1, m_arrNodes[00], bSupport1, 0);
            //m_arrNSupports[1] = new CNSupport(6, 2, m_arrNodes[01], bSupport2, 0);

            // Surface Load
            m_arrSLoads = new CSLoad_Free[1];
            FreeSurfaceLoadsMemberTypeData memberTypeData = new FreeSurfaceLoadsMemberTypeData(EMemberType_FS.eP, 0.5f);
            List<FreeSurfaceLoadsMemberTypeData> listOfLoadedMemberTypeData = new List<FreeSurfaceLoadsMemberTypeData>(1);
            BaseClasses.GraphObj.CPoint loadAreaControlPoint = new BaseClasses.GraphObj.CPoint(1, 0f, 0f, 0f, 0);
            float fLoadValue = -1000f; // N/m^2 (Pa)
            float fLoadArea_x = 5; //m
            float fLoadArea_y = 2; //m
            m_arrSLoads[0] = new CSLoad_FreeUniform(listOfLoadedMemberTypeData, ELoadCoordSystem.eLCS, ELoadDirection.eLD_Y, loadAreaControlPoint, fLoadArea_x, fLoadArea_y, fLoadValue, 0, 0, 0, Colors.DarkGoldenrod, true, true, true, 0);
            List<CSLoad_Free> listOfSurfaceLoads = new List<CSLoad_Free> { m_arrSLoads[0] };

            // Load Cases
            m_arrLoadCases = new CLoadCase[1];
            m_arrLoadCases[0] = new CLoadCase(1, "LC1", ELCGTypeForLimitState.eUniversal, ELCType.ePermanentLoad, ELCMainDirection.eGeneral, listOfSurfaceLoads);

            // Member Loads
            List<CMLoad> listOfMemberLoads = new List<CMLoad>();

            bool bUseGeneratedMemberLoads = true;

            if (!bUseGeneratedMemberLoads)
            {
                // User-defined member loads
                m_arrMLoads = new CMLoad[4];
                m_arrMLoads[0] = new CMLoad_21(1, 500, m_arrMembers[0], EMLoadTypeDistr.eMLT_QUF_W_21, ELoadType.eLT_F, ELoadCoordSystem.eLCS, ELoadDirection.eLD_Z, true, 0);
                m_arrMLoads[1] = new CMLoad_22(2, 750, 0.3f * m_arrMembers[1].FLength, m_arrMembers[1], EMLoadTypeDistr.eMLT_QUF_PA_22, ELoadType.eLT_F, ELoadCoordSystem.eLCS, ELoadDirection.eLD_Z, true, 0);
                m_arrMLoads[2] = new CMLoad_23(3, 800, 0.3f * m_arrMembers[2].FLength, m_arrMembers[2], EMLoadTypeDistr.eMLT_QUF_PB_23, ELoadType.eLT_F, ELoadCoordSystem.eLCS, ELoadDirection.eLD_Z, true, 0);
                m_arrMLoads[3] = new CMLoad_24(4, 600, 0.3f * m_arrMembers[3].FLength, 0.6f * m_arrMembers[3].FLength, m_arrMembers[3], EMLoadTypeDistr.eMLT_QUF_PG_24, ELoadType.eLT_F, ELoadCoordSystem.eLCS, ELoadDirection.eLD_Z, true, 0);

                listOfMemberLoads.Add(m_arrMLoads[0]);
                listOfMemberLoads.Add(m_arrMLoads[1]);
                listOfMemberLoads.Add(m_arrMLoads[2]);
                listOfMemberLoads.Add(m_arrMLoads[3]);

                m_arrLoadCases[0].MemberLoadsList = listOfMemberLoads;
            }
            else
            {
                // Member loads generated from surface load
                CLoadGenerator.GenerateMemberLoads(m_arrLoadCases, m_arrMembers); // Test
            }
        }
    }
}
