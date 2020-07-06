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
        public CLoadCombination GoverningLoadCombinationStructure;
        public CMember MaximumDesignRatioWholeStructureMember;

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
            m_DesignResults.Add(EMemberType_FS_Position.WindPostFrontSide, new DesignResultItem());
            m_DesignResults.Add(EMemberType_FS_Position.WindPostBackSide, new DesignResultItem());
            m_DesignResults.Add(EMemberType_FS_Position.GirtFrontSide, new DesignResultItem());
            m_DesignResults.Add(EMemberType_FS_Position.GirtBackSide, new DesignResultItem());

            m_DesignResults.Add(EMemberType_FS_Position.DoorFrame, new DesignResultItem());
            m_DesignResults.Add(EMemberType_FS_Position.WindowFrame, new DesignResultItem());
            m_DesignResults.Add(EMemberType_FS_Position.DoorTrimmer, new DesignResultItem());
            m_DesignResults.Add(EMemberType_FS_Position.DoorLintel, new DesignResultItem());

            m_DesignResults.Add(EMemberType_FS_Position.CrossBracingWall, new DesignResultItem());
            m_DesignResults.Add(EMemberType_FS_Position.CrossBracingRoof, new DesignResultItem());
        }
    }
}
