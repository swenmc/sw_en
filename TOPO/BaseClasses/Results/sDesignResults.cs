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
        
        public float fMaximumDesignRatioWholeStructure;
        //public float fMaximumDesignRatioMainColumn;
        //public float fMaximumDesignRatioMainRafter;
        //public float fMaximumDesignRatioEndColumn;
        //public float fMaximumDesignRatioEndRafter;
        //public float fMaximumDesignRatioGirts;
        //public float fMaximumDesignRatioPurlins;
        //public float fMaximumDesignRatioColumns;

        public CLoadCombination GoverningLoadCombinationStructure;
        //public CLoadCombination GoverningLoadCombinationMainColumn;
        //public CLoadCombination GoverningLoadCombinationMainRafter;
        //public CLoadCombination GoverningLoadCombinationEndColumn;
        //public CLoadCombination GoverningLoadCombinationEndRafter;
        //public CLoadCombination GoverningLoadCombinationGirts;
        //public CLoadCombination GoverningLoadCombinationPurlins;
        //public CLoadCombination GoverningLoadCombinationColumns;

        public CMember MaximumDesignRatioWholeStructureMember;
        //public CMember MaximumDesignRatioMainColumn;
        //public CMember MaximumDesignRatioMainRafter;
        //public CMember MaximumDesignRatioEndColumn;
        //public CMember MaximumDesignRatioEndRafter;
        //public CMember MaximumDesignRatioGirt;
        //public CMember MaximumDesignRatioPurlin;
        //public CMember MaximumDesignRatioColumn;

        private Dictionary<EMemberType_DB, DesignResultItem> m_DesignResults;
        public Dictionary<EMemberType_DB, DesignResultItem> DesignResults
        {
            get
            {
                if (m_DesignResults == null) InitDesignResultsDictionary();
                return m_DesignResults;
            }

            
        }

        private void InitDesignResultsDictionary()
        {
            m_DesignResults = new Dictionary<EMemberType_DB, DesignResultItem>();

            m_DesignResults.Add(EMemberType_DB.MainColumn, new DesignResultItem());
            m_DesignResults.Add(EMemberType_DB.MainRafter, new DesignResultItem());
            m_DesignResults.Add(EMemberType_DB.EdgeColumn, new DesignResultItem());
            m_DesignResults.Add(EMemberType_DB.EdgeRafter, new DesignResultItem());
            m_DesignResults.Add(EMemberType_DB.EdgePurlin, new DesignResultItem());
            m_DesignResults.Add(EMemberType_DB.Girt, new DesignResultItem());
            m_DesignResults.Add(EMemberType_DB.Purlin, new DesignResultItem());
            m_DesignResults.Add(EMemberType_DB.ColumnFrontSide, new DesignResultItem());
            m_DesignResults.Add(EMemberType_DB.ColumnBackSide, new DesignResultItem());
            m_DesignResults.Add(EMemberType_DB.GirtFrontSide, new DesignResultItem());
            m_DesignResults.Add(EMemberType_DB.GirtBackSide, new DesignResultItem());

            m_DesignResults.Add(EMemberType_DB.DoorFrame, new DesignResultItem());
            m_DesignResults.Add(EMemberType_DB.WindowFrame, new DesignResultItem());
            m_DesignResults.Add(EMemberType_DB.DoorTrimmer, new DesignResultItem());
            m_DesignResults.Add(EMemberType_DB.DoorLintel, new DesignResultItem());
        }
    }
}
