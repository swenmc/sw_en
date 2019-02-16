using BaseClasses;
using CRSC;
using MATERIAL;
using System;
using System.Collections.Generic;

namespace Examples
{
    public class CExample_2D_15_PF : CExample
    {
        public CExample_2D_15_PF(
            List<CMember> members,
            List<CNSupport> supports,
            CLoadCase[] loadcases,
            CLoadCombination[] loadcombinations)
        {
            m_eSLN = ESLN.e2DD_1D; // 1D members in 2D model
            m_eNDOF = (int)ENDOF.e2DEnv; // DOF in 2D
            m_eGCS = EGCS.eGCSLeftHanded; // Global coordinate system

            m_arrNodes = new CNode[5];
            m_arrMembers = new CMember[4];
            m_arrMat = new CMat[1];
            m_arrCrSc = new CCrSc[2];
            m_arrNSupports = supports.ToArray();
            m_arrLoadCases = loadcases;
            m_arrLoadCombs = loadcombinations;

            // Materials
            // Materials List - Materials Array - Fill Data of Materials Array
            m_arrMat[0] = members[0].CrScStart.m_Mat;

            // Cross-sections
            // CrSc List - CrSc Array - Fill Data of Cross-sections Array
            // Cross-section
            m_arrCrSc[0] = members[0].CrScStart;
            m_arrCrSc[0].m_Mat = members[0].CrScStart.m_Mat; // Set CrSc Material

            m_arrCrSc[1] = members[1].CrScStart;
            m_arrCrSc[1].m_Mat = members[1].CrScStart.m_Mat; // Set CrSc Material

            // Nodes
            // Nodes List - Nodes Array

            // RAM DEFINOVANY V XZ

            // Node 1
            m_arrNodes[0] = new CNode();
            m_arrNodes[0].ID = 1;
            m_arrNodes[0].X = 0f;
            m_arrNodes[0].Y = 0f;
            m_arrNodes[0].Z = 0f;

            // Node 2
            m_arrNodes[1] = new CNode();
            m_arrNodes[1].ID = 2;
            m_arrNodes[1].X = 0f;
            m_arrNodes[1].Y = 0f;
            m_arrNodes[1].Z = members[0].NodeEnd.Z;

            // Node 3
            m_arrNodes[2] = new CNode();
            m_arrNodes[2].ID = 3;
            m_arrNodes[2].X = members[1].NodeEnd.X;
            m_arrNodes[2].Y = 0;
            m_arrNodes[2].Z = members[1].NodeEnd.Z;

            // Node 4
            m_arrNodes[3] = new CNode();
            m_arrNodes[3].ID = 4;
            m_arrNodes[3].X = members[2].NodeEnd.X;
            m_arrNodes[3].Y = 0f;
            m_arrNodes[3].Z = members[2].NodeEnd.Z;

            // Node 5
            m_arrNodes[4] = new CNode();
            m_arrNodes[4].ID = 5;
            m_arrNodes[4].X = members[3].NodeEnd.X;
            m_arrNodes[4].Y = 0f;
            m_arrNodes[4].Z = 0f;

            for (int i = 0; i < members.Count; i++)
            {
                m_arrMembers[i] = members[i];
                m_arrMembers[i].NodeStart = m_arrNodes[i];
                m_arrMembers[i].NodeEnd = m_arrNodes[i+1];
            }

            // Nodal Supports - fill values
            // Support 1 - NodeIDs: 1,5
            m_arrNSupports[0] = new CNSupport(m_eNDOF);
            m_arrNSupports[0].ID = 1;
            m_arrNSupports[0].m_iNodeCollection = new int[2];
            m_arrNSupports[0].m_iNodeCollection[0] = 1;
            m_arrNSupports[0].m_iNodeCollection[1] = 5;

            // Sort by ID
            Array.Sort(m_arrNSupports, new BaseClasses.CCompare_NSupportID());
        }
    }
}
