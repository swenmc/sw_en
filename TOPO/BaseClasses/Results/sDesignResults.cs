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

        private Dictionary<EMemberType_FS_Position, DesignResultItem> m_DesignResults;
        public Dictionary<EMemberType_FS_Position, DesignResultItem> DesignResults
        {
            get
            {
                if (m_DesignResults == null) InitDesignResultsDictionary();
                return m_DesignResults;
            }
        }

        private void InitDesignResultsDictionary()
        {
            m_DesignResults = new Dictionary<EMemberType_FS_Position, DesignResultItem>();

            m_DesignResults.Add(EMemberType_FS_Position.MainColumn, new DesignResultItem());
            m_DesignResults.Add(EMemberType_FS_Position.MainRafter, new DesignResultItem());
            m_DesignResults.Add(EMemberType_FS_Position.EdgeColumn, new DesignResultItem());
            m_DesignResults.Add(EMemberType_FS_Position.EdgeRafter, new DesignResultItem());
            m_DesignResults.Add(EMemberType_FS_Position.EdgePurlin, new DesignResultItem());
            m_DesignResults.Add(EMemberType_FS_Position.Girt, new DesignResultItem());
            m_DesignResults.Add(EMemberType_FS_Position.Purlin, new DesignResultItem());
            m_DesignResults.Add(EMemberType_FS_Position.ColumnFrontSide, new DesignResultItem());
            m_DesignResults.Add(EMemberType_FS_Position.ColumnBackSide, new DesignResultItem());
            m_DesignResults.Add(EMemberType_FS_Position.GirtFrontSide, new DesignResultItem());
            m_DesignResults.Add(EMemberType_FS_Position.GirtBackSide, new DesignResultItem());

            m_DesignResults.Add(EMemberType_FS_Position.DoorFrame, new DesignResultItem());
            m_DesignResults.Add(EMemberType_FS_Position.WindowFrame, new DesignResultItem());
            m_DesignResults.Add(EMemberType_FS_Position.DoorTrimmer, new DesignResultItem());
            m_DesignResults.Add(EMemberType_FS_Position.DoorLintel, new DesignResultItem());
        }
    }
}
