﻿using CRSC;
using MATERIAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseClasses
{
    public class CFrame : CModel
    {
        public CFrame(CMember[] members, CNode[] nodes, CLoadCase[] loadCases, CLoadCombination[] loadCombinations, CNSupport[] supports)
        {
            m_eSLN = ESLN.e2DD_1D; // 1D members in 2D model
            m_eNDOF = (int)ENDOF.e2DEnv; // DOF in 2D
            m_eGCS = EGCS.eGCSLeftHanded; // Global coordinate system

            // RAM DEFINOVANY V XZ
            m_arrNodes = nodes;
            m_arrMembers = members;
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
            if (m_arrMembers.Length < 2) return;
            // Materials
            // Materials List - Materials Array - Fill Data of Materials Array
            m_arrMat = new Dictionary<EMemberGroupNames, CMat>(); // CMat[1];
            m_arrMat[0] = m_arrMembers[0].CrScStart.m_Mat;

            // Cross-sections
            // CrSc List - CrSc Array - Fill Data of Cross-sections Array
            // Cross-section
            m_arrCrSc = new CCrSc[2];
            m_arrCrSc[0] = m_arrMembers[0].CrScStart;
            m_arrCrSc[0].m_Mat = m_arrMembers[0].CrScStart.m_Mat; // Set CrSc Material

            m_arrCrSc[1] = m_arrMembers[1].CrScStart;
            m_arrCrSc[1].m_Mat = m_arrMembers[1].CrScStart.m_Mat; // Set CrSc Material
        }
    }
}
