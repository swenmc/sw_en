using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseClasses
{    
    public class MemberDesignResultItem
    {
        private string m_Prefix;
        private string m_ComponentName;
        private string m_LoadCombination;
        private int m_MemberID;        
        private float m_DesignRatio;

        public string Prefix
        {
            get
            {
                return m_Prefix;
            }

            set
            {
                m_Prefix = value;
            }
        }

        public string ComponentName
        {
            get
            {
                return m_ComponentName;
            }

            set
            {
                m_ComponentName = value;
            }
        }

        public string LoadCombination
        {
            get
            {
                return m_LoadCombination;
            }

            set
            {
                m_LoadCombination = value;
            }
        }

        public int MemberID
        {
            get
            {
                return m_MemberID;
            }

            set
            {
                m_MemberID = value;
            }
        }

        public float DesignRatio
        {
            get
            {
                return m_DesignRatio;
            }

            set
            {
                m_DesignRatio = value;
            }
        }

        public MemberDesignResultItem() { }
        public MemberDesignResultItem(string prefix, string componentName, string loadCombination, int memberID, float designRatio)
        {
            m_Prefix = prefix;
            m_ComponentName = componentName;
            m_LoadCombination = loadCombination;
            m_MemberID = memberID;
            m_DesignRatio = designRatio;
        }
        
    }
}
