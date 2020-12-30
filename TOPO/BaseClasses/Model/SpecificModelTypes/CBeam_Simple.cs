using CRSC;
using MATERIAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseClasses
{
    public class CBeam_Simple : CModel
    {
        public CBeam_Simple(CMember member, CNode[] nodes, CLoadCase[] loadCases, CLoadCombination[] loadCombinations, CNSupport[] supports)
        {
            m_eSLN = ESLN.e3DD_1D; // 1D members in 3D model
            m_eNDOF = (int)ENDOF.e3DEnv; // DOF in 3D
            m_eGCS = EGCS.eGCSLeftHanded; // Global coordinate system

            // RAM DEFINOVANY V XZ
            m_arrNodes = nodes;
            m_arrMembers = new CMember[1] { member };
            m_arrLoadCases = loadCases;
            m_arrLoadCombs = loadCombinations;
            m_arrNSupports = supports;

            // TODO 201 - Ondrej
            // ???? Doplnit objekty joints z hlavneho modelu medzi prvkami ramu a zaistit spravne priradenie joints
            // Alebo je potrebne upravit posudky prutov a spojov tak aby sa posudzovali objekty z 3D modelu a nie z modelov samostatnych ramov

            SetMaterialAndCrossSection();
        }

        public void SetMaterialAndCrossSection()
        {
            if (m_arrMembers == null) return;
            if (m_arrMembers.Length < 1) return;
            // Materials
            // Materials List - Materials Array - Fill Data of Materials Array
            m_arrMat = new Dictionary<EMemberGroupNames, CMat>();
            m_arrMat[(EMemberGroupNames)0] = m_arrMembers[0].CrScStart.m_Mat;

            // Cross-sections
            // CrSc List - CrSc Array - Fill Data of Cross-sections Array
            // Cross-section
            m_arrCrSc = new CCrSc[1];
            m_arrCrSc[0] = m_arrMembers[0].CrScStart;
            m_arrCrSc[0].m_Mat = m_arrMembers[0].CrScStart.m_Mat; // Set CrSc Material
        }
    }
}
