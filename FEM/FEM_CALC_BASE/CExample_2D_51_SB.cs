using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MATH;
using BaseClasses;
using MATERIAL;
using CRSC;

namespace FEM_CALC_BASE
{
    public class CExample_2D_51_SB : CExample
    {
        float fL;
        float fI;
        float fE;

        // Docasne riesenie
        // TODO 51 - nahradny model pruta zatazeneho spojitym zatazenim pre vypocet vn. sil pre lokalny smer y alebo z
        // TODO - Ondrej , upravit vsetko tak aby sem vstupoval priamo objekt pruta, limit state, load case, load
        public CExample_2D_51_SB(CMember member, CMLoad load)
        {
            m_eSLN = ESLN.e2DD_1D; // 1D members in 2D model
            m_eNDOF = (int)ENDOF.e2DEnv; // DOF in 2D
            m_eGCS = EGCS.eGCSLeftHanded; // Global coordinate system

            m_arrNodes = new CNode[2];
            m_arrMembers = new CMember[1];
            m_arrMat = new System.Collections.Generic.Dictionary<EMemberGroupNames, CMat>();
            m_arrCrSc = new CCrSc[1];
            m_arrNSupports = new CNSupport[2];
            m_arrMLoads = new CMLoad[1];
            m_arrLoadCases = new CLoadCase[1];
            m_arrLoadCombs = new CLoadCombination[1];

            fL = member.FLength;

            // Cross-sections
            // CrSc List - CrSc Array - Fill Data of Cross-sections Array
            // Cross-section
            m_arrCrSc[0] = member.CrScStart;

            // Materials
            // Materials List - Materials Array - Fill Data of Materials Array
            m_arrMat[0] = m_arrCrSc[0].m_Mat;

            if (load.ELoadDir == ELoadDirection.eLD_Y)
                fI = (float)m_arrCrSc[0].I_z;
            else if (load.ELoadDir == ELoadDirection.eLD_Z)
                fI = (float)m_arrCrSc[0].I_y;
            else
                fI = 0;

            fE = m_arrMat[0].m_fE;

            // Nodes
            // Nodes List - Nodes Array

            // Node 1
            m_arrNodes[0] = new CNode();
            m_arrNodes[0].ID = 1;
            m_arrNodes[0].X = 0f;
            m_arrNodes[0].Y = 0f;
            m_arrNodes[0].Z = 0f;

            // Node 2
            m_arrNodes[1] = new CNode();
            m_arrNodes[1].ID = 2;
            m_arrNodes[1].X = fL;
            m_arrNodes[1].Y = 0f;
            m_arrNodes[1].Z = 0f;

            // Members
            // Members List - Members Array

            // Member 1 - 1-2
            m_arrMembers[0] = new CMember();
            m_arrMembers[0].ID = 1;
            m_arrMembers[0].NodeStart = m_arrNodes[0];
            m_arrMembers[0].NodeEnd = m_arrNodes[1];
            m_arrMembers[0].CrScStart = m_arrCrSc[0];

            // Nodal Supports - fill values
            // Support 1 - NodeIDs: 1
            m_arrNSupports[0] = new CNSupport(m_eNDOF);
            m_arrNSupports[0].ID = 1;
            m_arrNSupports[0].m_bRestrain[0] = true; // true - 1 restraint (infinity) / false - 0 - free (zero rigidity)
            m_arrNSupports[0].m_bRestrain[1] = true;
            m_arrNSupports[0].m_bRestrain[2] = false;
            m_arrNSupports[0].m_iNodeCollection = new int[1];
            m_arrNSupports[0].m_iNodeCollection[0] = 1;

            // Support 2 - NodeIDs: 1
            m_arrNSupports[1] = new CNSupport(m_eNDOF);
            m_arrNSupports[1].ID = 2;
            m_arrNSupports[1].m_bRestrain[0] = false; // true - 1 restraint (infinity) / false - 0 - free (zero rigidity)
            m_arrNSupports[1].m_bRestrain[1] = true;
            m_arrNSupports[1].m_bRestrain[2] = false;
            m_arrNSupports[1].m_iNodeCollection = new int[1];
            m_arrNSupports[1].m_iNodeCollection[0] = 2;

            // Sort by ID
            Array.Sort(m_arrNSupports, new CCompare_NSupportID());

            // Member loads
            // Load 1 - MemberIDs: 1
            CMLoad memberLoad_0 = null;

            if (load is CMLoad_21)
            {
                // Nastavujem do kopie, aby som neprepisal priradenie originalneho zatazenia prutu v globalnom modeli
                CMLoad_21 load_uniform = new CMLoad_21();
                load_uniform.ID = 1;
                load_uniform.Fq = ((CMLoad_21)load).Fq;
                load_uniform.MLoadTypeDistr = load.MLoadTypeDistr;
                load_uniform.ELoadType_FMTS = load.ELoadType_FMTS;
                load_uniform.ELoadCS = load.ELoadCS;
                load_uniform.ELoadDir = load.ELoadDir;
                memberLoad_0 = load_uniform;
            }
            else if (load is CMLoad_24)
            {
                // Nastavujem do kopie, aby som neprepisal priradenie originalneho zatazenia prutu v globalnom modeli
                CMLoad_24 load_partial = new CMLoad_24();
                load_partial.ID = 1;
                load_partial.FaA = ((CMLoad_24)load).FaA;
                load_partial.Fa = ((CMLoad_24)load).Fa;
                load_partial.Fs = ((CMLoad_24)load).Fs;
                load_partial.Fd_calc = ((CMLoad_24)load).Fd_calc;
                load_partial.Fq = ((CMLoad_24)load).Fq;
                load_partial.MLoadTypeDistr = load.MLoadTypeDistr;
                load_partial.ELoadType_FMTS = load.ELoadType_FMTS;
                load_partial.ELoadCS = load.ELoadCS;
                load_partial.ELoadDir = load.ELoadDir;
                memberLoad_0 = load_partial;
            }
            else
            {
                // Not defined
                throw new ArgumentNullException("Invalid load type.");
            }

            memberLoad_0.IMemberCollection = new int[1];
            memberLoad_0.IMemberCollection[0] = 1;
            memberLoad_0.Member = member;
            m_arrMLoads[0] = memberLoad_0;

            // Load Cases
            // Load Case 1
            CLoadCase LoadCase0 = new CLoadCase();
            LoadCase0.ID = 1;
            LoadCase0.MemberLoadsList.Add(memberLoad_0);

            m_arrLoadCases[0] = LoadCase0;

            // Load Combinations
            // Load Combination 1
            CLoadCombination LoadComb0 = new CLoadCombination();
            LoadComb0.ID = 1;
            LoadComb0.LoadCasesFactorsList = new List<float> { 1.00f};
            LoadComb0.LoadCasesList = new List<CLoadCase> { LoadCase0 };

            m_arrLoadCombs[0] = LoadComb0;
        }

        public CExample_2D_51_SB(float fL_temp, float fq_temp)
        {
            fL = fL_temp;
            //fq = fq_temp;
        }
    }
}
