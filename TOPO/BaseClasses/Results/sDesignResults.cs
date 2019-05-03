using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseClasses
{    
    public struct sDesignResults
    {
        public string sLimitStateType;

        //dalo by sa to riesit ako Dictionary<EMemberTypeDB, Trojica(maxDesignRatio, member, loadCombination)>
        

        public float fMaximumDesignRatioWholeStructure;
        public float fMaximumDesignRatioMainColumn;
        public float fMaximumDesignRatioMainRafter;
        public float fMaximumDesignRatioEndColumn;
        public float fMaximumDesignRatioEndRafter;
        public float fMaximumDesignRatioGirts;
        public float fMaximumDesignRatioPurlins;
        public float fMaximumDesignRatioColumns;

        public CLoadCombination GoverningLoadCombinationStructure;
        public CLoadCombination GoverningLoadCombinationMainColumn;
        public CLoadCombination GoverningLoadCombinationMainRafter;
        public CLoadCombination GoverningLoadCombinationEndColumn;
        public CLoadCombination GoverningLoadCombinationEndRafter;
        public CLoadCombination GoverningLoadCombinationGirts;
        public CLoadCombination GoverningLoadCombinationPurlins;
        public CLoadCombination GoverningLoadCombinationColumns;

        public CMember MaximumDesignRatioWholeStructureMember;
        public CMember MaximumDesignRatioMainColumn;
        public CMember MaximumDesignRatioMainRafter;
        public CMember MaximumDesignRatioEndColumn;
        public CMember MaximumDesignRatioEndRafter;
        public CMember MaximumDesignRatioGirt;
        public CMember MaximumDesignRatioPurlin;
        public CMember MaximumDesignRatioColumn;
    }
}
