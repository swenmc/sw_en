using BaseClasses;
using M_EC1.AS_NZS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFD
{
    public class CNodalLoadGenerator
    {
        private int iNumberOfLoadsInXDirection;
        private int iNumberOfLoadsInYDirection;
        private int iFrameNodesNo;
        private CLoadCase[] m_arrLoadCases;
        CNode[] m_arrNodes;
        private float fL1;
        CCalcul_1170_5 eq;

        public List<CNLoad> nodalLoadEQ_ULS_PlusX;
        public List<CNLoad> nodalLoadEQ_ULS_PlusY;
        public List<CNLoad> nodalLoadEQ_SLS_PlusX;
        public List<CNLoad> nodalLoadEQ_SLS_PlusY;

        public CNodalLoadGenerator(int numberOfLoadsInXDirection, int numberOfLoadsInYDirection, int iframenodesNo,
            CLoadCase[] arrLoadCases, CNode[] arrNodes, /*float fL1_frame,*/ CCalcul_1170_5 calc_eq)
        {
            iNumberOfLoadsInXDirection = numberOfLoadsInXDirection;
            iNumberOfLoadsInYDirection = numberOfLoadsInYDirection;
            iFrameNodesNo = iframenodesNo;
            m_arrLoadCases = arrLoadCases;
            m_arrNodes = arrNodes;
            //fL1 = fL1_frame; // Docasne riesenie zatazenie krajnych ramov pre smer X a redukuje na polovicu, TODO - dopracovat detailnejsie, ina tuhost a hmotnost krajnych ramov
            eq = calc_eq;
        }

        public void GenerateNodalLoads()
        {
            nodalLoadEQ_ULS_PlusX = new List<CNLoad>(iNumberOfLoadsInXDirection);
            nodalLoadEQ_ULS_PlusY = new List<CNLoad>(iNumberOfLoadsInYDirection);

            nodalLoadEQ_SLS_PlusX = new List<CNLoad>(iNumberOfLoadsInXDirection);
            nodalLoadEQ_SLS_PlusY = new List<CNLoad>(iNumberOfLoadsInYDirection);

            int iLastIndex = 0;
            for (int i = 0; i < iNumberOfLoadsInXDirection; i++)
            {
                float fLoadFactor = 1f;

                if (i == 0 || i == iNumberOfLoadsInXDirection - 1)
                    fLoadFactor = 0.5f /** fL1 / fL1*/; // Pre krajne ramy je uvazovana polovica zatazovacej sirky - TODO - dopracovat preciznejsie

                nodalLoadEQ_ULS_PlusX.Add(new CNLoadSingle(i + 1, m_arrNodes[i * iFrameNodesNo + 1], ENLoadType.eNLT_Fx, fLoadFactor * eq.fV_x_ULS_strength, true, 0));
                nodalLoadEQ_SLS_PlusX.Add(new CNLoadSingle(i + 1, m_arrNodes[i * iFrameNodesNo + 1], ENLoadType.eNLT_Fx, fLoadFactor * eq.fV_x_SLS, true, 0));
            }

            iLastIndex += iNumberOfLoadsInXDirection;

            int iNodeIndexMultiplier = 2; // Gable Roof uzly s indexom 1 a 3

            if(iFrameNodesNo == 4) // Monopitch roof
                iNodeIndexMultiplier = 1; // Monopitch Roof uzly s indexom 1 a 2

            for (int i = 0; i < iNumberOfLoadsInYDirection; i++)
            {
                nodalLoadEQ_ULS_PlusY.Add(new CNLoadSingle(iLastIndex + i + 1, m_arrNodes[i * iNodeIndexMultiplier + 1], ENLoadType.eNLT_Fy, eq.fV_y_ULS_strength, true, 0));
                nodalLoadEQ_SLS_PlusY.Add(new CNLoadSingle(iLastIndex + i + 1, m_arrNodes[i * iNodeIndexMultiplier + 1], ENLoadType.eNLT_Fy, eq.fV_y_SLS, true, 0));
            }

            iLastIndex += iNumberOfLoadsInYDirection;

            // ULS
            m_arrLoadCases[(int)ELCName.eEQ_Eu_Left_X_Plus].NodeLoadsList = nodalLoadEQ_ULS_PlusX;     // 22
            m_arrLoadCases[(int)ELCName.eEQ_Eu_Front_Y_Plus].NodeLoadsList = nodalLoadEQ_ULS_PlusY;    // 23

            // SLS
            m_arrLoadCases[(int)ELCName.eEQ_Es_Left_X_Plus].NodeLoadsList = nodalLoadEQ_SLS_PlusX;     // 43
            m_arrLoadCases[(int)ELCName.eEQ_Es_Front_Y_Plus].NodeLoadsList = nodalLoadEQ_SLS_PlusY;    // 44
        }
    }
}
