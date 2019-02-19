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
            List<CNode> nodes,
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
            m_arrNSupports = (supports!= null) ? supports.ToArray() : null;
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
            m_arrNodes = nodes.ToArray();
            m_arrMembers = members.ToArray();
            m_arrLoadCases = loadcases;
            m_arrLoadCombs = loadcombinations;

            // TODO 201 - Ondrej
            // ???? Doplnit objekty joints z hlavneho modelu medzi prvkami ramu a zaistit spravne priradenie joints
            // Alebo je potrebne upravit posudky prutov a spojov tak aby sa posudzovali objekty z 3D modelu a nie z modelov samostatnych ramov

        }
    }
}
