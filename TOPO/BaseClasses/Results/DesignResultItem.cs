using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseClasses
{    
    public class DesignResultItem
    {
        private float m_MaximumDesignRatio;
        private CLoadCombination m_GoverningLoadCombination;
        private CMember m_MemberWithMaximumDesignRatio;

        public float MaximumDesignRatio
        {
            get
            {
                return m_MaximumDesignRatio;
            }

            set
            {
                m_MaximumDesignRatio = value;
            }
        }

        public CLoadCombination GoverningLoadCombination
        {
            get
            {
                return m_GoverningLoadCombination;
            }

            set
            {
                m_GoverningLoadCombination = value;
            }
        }

        public CMember MemberWithMaximumDesignRatio
        {
            get
            {
                return m_MemberWithMaximumDesignRatio;
            }

            set
            {
                m_MemberWithMaximumDesignRatio = value;
            }
        }

        public DesignResultItem() { }
        public DesignResultItem(float maximumDesignRatio, CLoadCombination governingLoadCombination, CMember memberWithMaximumDesignRatio)
        {
            m_MaximumDesignRatio = maximumDesignRatio;
            m_GoverningLoadCombination = governingLoadCombination;
            m_MemberWithMaximumDesignRatio = memberWithMaximumDesignRatio;
        }


    }
}
