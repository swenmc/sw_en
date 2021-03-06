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
        public float fRoofPitch_rad;

        public CFrame(EModelType_FS eKitset_temp, float fRoofPitch_rad_temp, CMember[] members, CNode[] nodes, CLoadCase[] loadCases, CLoadCombination[] loadCombinations, CNSupport[] supports)
        {
            m_eSLN = ESLN.e2DD_1D; // 1D members in 2D model
            m_eNDOF = (int)ENDOF.e2DEnv; // DOF in 2D
            m_eGCS = EGCS.eGCSLeftHanded; // Global coordinate system

            eKitset = eKitset_temp;
            fRoofPitch_rad = fRoofPitch_rad_temp;

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
            m_arrMat = new Dictionary<EMemberType_FS_Position, CMat>();
            // Cross-section
            m_arrCrSc = new Dictionary<EMemberType_FS_Position, CCrSc>();

            //initialize Materials and Cross Sections according to members types
            foreach (CMember m in m_arrMembers)
            {
                if (!m_arrMat.ContainsKey(m.EMemberTypePosition))
                {
                    m_arrMat[m.EMemberTypePosition] = m.CrScStart.m_Mat;
                }
                if (!m_arrCrSc.ContainsKey(m.EMemberTypePosition))
                {
                    m_arrCrSc[m.EMemberTypePosition] = m.CrScStart;
                    m_arrCrSc[m.EMemberTypePosition].m_Mat = m.CrScStart.m_Mat;  //to Mato - treba aj toto? nejako mi to pride,ze je to dvojmo
                }
            }

            //// Materials
            //// Materials List - Materials Array - Fill Data of Materials Array
            //m_arrMat = new Dictionary<EMemberType_FS_Position, CMat>(); // CMat[1];
            //m_arrMat[0] = m_arrMembers[0].CrScStart.m_Mat;

            //// Cross-sections
            //// CrSc List - CrSc Array - Fill Data of Cross-sections Array
            //// Cross-section
            //m_arrCrSc = new Dictionary<EMemberType_FS_Position, CCrSc>(); //new CCrSc[2];
            //m_arrCrSc[0] = m_arrMembers[0].CrScStart;
            //m_arrCrSc[0].m_Mat = m_arrMembers[0].CrScStart.m_Mat; // Set CrSc Material

            //m_arrCrSc.Add((EMemberType_FS_Position)1, m_arrMembers[1].CrScStart);
            //m_arrCrSc[(EMemberType_FS_Position)1].m_Mat = m_arrMembers[1].CrScStart.m_Mat;

            ////m_arrCrSc[1] = m_arrMembers[1].CrScStart;
            ////m_arrCrSc[1].m_Mat = m_arrMembers[1].CrScStart.m_Mat; // Set CrSc Material

            //// TODO 680 - pridat materialy a prierezy pre ER a MR canopy
        }
    }
}
