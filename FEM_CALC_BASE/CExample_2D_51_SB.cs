using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MATH;
using BaseClasses;
using MATERIAL;
using CRSC;
using BaseClasses.CRSC;

namespace FEM_CALC_BASE
{
    public class CExample_2D_51_SB : CExample
    {
        float fq;
        float fL;
        EMLoadDirPCC1 eLoadDirection;
        float fI;
        float fE;

        public CExample_2D_51_SB(CCrSc crsc, float fL_temp, EMLoadDirPCC1 eLoadDirection_temp, float fq_temp)
        {
            m_eSLN = ESLN.e2DD_1D; // 1D members in 2D model
            m_eNDOF = (int)ENDOF.e2DEnv; // DOF in 2D
            m_eGCS = EGCS.eGCSLeftHanded; // Global coordinate system

            m_arrNodes = new CNode[2];
            m_arrMembers = new CMember[1];
            m_arrMat = new CMat[1];
            m_arrCrSc = new CCrSc[1];
            m_arrNSupports = new CNSupport[2];
            m_arrMLoads = new CMLoad[1];
            m_arrLoadCases = new CLoadCase[1];
            m_arrLoadCombs = new CLoadCombination[1];

            fL = fL_temp;
            fq = fq_temp;
            eLoadDirection = eLoadDirection_temp;

            // Cross-sections
            // CrSc List - CrSc Array - Fill Data of Cross-sections Array
            // Cross-section
            m_arrCrSc[0] = crsc;

            // Materials
            // Materials List - Materials Array - Fill Data of Materials Array
            m_arrMat[0] = m_arrCrSc[0].m_Mat;

            if (eLoadDirection == EMLoadDirPCC1.eMLD_PCC_FYU_MZV)
                fI = (float)m_arrCrSc[0].I_z;
            else if (eLoadDirection == EMLoadDirPCC1.eMLD_PCC_FZV_MYU)
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
            CMLoad_21 MLoad_q1 = new CMLoad_21(fq);
            MLoad_q1.ID = 1;
            MLoad_q1.MLoadTypeDistr = EMLoadTypeDistr.eMLT_QUF_W_21;
            MLoad_q1.MLoadType = EMLoadType.eMLT_F;
            MLoad_q1.EDirPPC = eLoadDirection; // Load in LCS
            MLoad_q1.IMemberCollection = new int[1];
            MLoad_q1.IMemberCollection[0] = 1;
            m_arrMLoads[0] = MLoad_q1;

            // Load Cases
            // Load Case 1
            CLoadCase LoadCase0 = new CLoadCase();
            LoadCase0.ID = 1;

            m_arrLoadCases[0] = LoadCase0;

            // Load Combinations
            // Load Combination 1
            CLoadCombination LoadComb0 = new CLoadCombination();
            LoadComb0.ID = 1;

            m_arrLoadCombs[0] = LoadComb0;
        }

        public float GetSupportReactionValue_R()
        {
            return 0.5f * fq * fL;
        }
        public float GetV_max()
        {
            return GetSupportReactionValue_R();
        }
        public float GetM_max()
        {
            return 0.125f * fq * fL * fL;
        }
        public float GetV_x(float fx)
        {
            return fq * (0.5f * fL - fx);
        }
        public float GetM_x(float fx)
        {
            return 0.5f * fq * fx * (fL - fx);
        }
        public float GetDelta_max()
        {
            return (5f * fq * MathF.Pow4(fL)) / (384f * fE * fI);
        }
        public float GetDelta_x(float fx)
        {
            return ((fq * fx) / (24f * fE * fI)) * (MathF.Pow3(fL) - 2f * fL * MathF.Pow2(fx) + MathF.Pow3(fx));
        }
    }
}
