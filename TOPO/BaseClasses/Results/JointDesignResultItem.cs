using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseClasses
{    
    public class JointDesignResultItem
    {        
        private string m_ComponentName;
        private string m_JointType;
        private string m_LoadCombination;
        private int m_JointID;        
        private float m_DesignRatio;

        

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
        public string JointType
        {
            get
            {
                return m_JointType;
            }

            set
            {
                m_JointType = value;
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

        public int JointID
        {
            get
            {
                return m_JointID;
            }

            set
            {
                m_JointID = value;
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

        

        public JointDesignResultItem() { }
        public JointDesignResultItem(string componentName, string jointType, string loadCombination, int memberID, float designRatio)
        {            
            m_ComponentName = componentName;
            m_JointType = jointType;
            m_LoadCombination = loadCombination;
            m_JointID = memberID;
            m_DesignRatio = designRatio;
        }
        
    }
}
