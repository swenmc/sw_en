using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaseClasses;
using M_EC1.AS_NZS;

namespace PFD
{
    [Serializable]
    public class CModel_PFD : CExample
    {
        public float fRoofPitch_rad;
        public float fSlopeFactor; // Snow load
        public int iFrameNo;
        public int iFrameNodesNo;

        public int iEavesPurlinNoInOneFrame;
        public int iPurlinNoInOneFrame;
        public int iGirtNoInOneFrame;

        public List<CEntity3D> componentList;
        public List<CBlock_3D_001_DoorInBay> DoorsModels;
        public List<CBlock_3D_002_WindowInBay> WindowsModels;

        public virtual void CalculateLoadValuesAndGenerateLoads(
                CCalcul_1170_1 generalLoad,
                CCalcul_1170_2 wind,
                CCalcul_1170_3 snow,
                CCalcul_1170_5 eq,
                bool bGenerateNodalLoads,
                bool bGenerateLoadsOnGirts,
                bool bGenerateLoadsOnPurlins,
                bool bGenerateLoadsOnColumns,
                bool bGenerateLoadsOnFrameMembers,
                bool bGenerateSurfaceLoads) { }
    }
}
