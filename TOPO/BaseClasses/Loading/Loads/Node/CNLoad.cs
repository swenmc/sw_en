using System;

namespace BaseClasses
{
    [Serializable]
    public abstract class CNLoad : CLoad
    {
        //----------------------------------------------------------------------------
        private int m_iNLoad_ID;
        private CNode m_Node;
        private int[] m_iNodeCollection; // List / Collection of nodes IDs where load is defined

        //----------------------------------------------------------------------------
        public int INLoad_ID
        {
            get { return m_iNLoad_ID; }
            set { m_iNLoad_ID = value; }
        }

        public CNode Node
        {
            get { return m_Node; }
            set { m_Node = value; }
        }
        public int[] INodeCollection
        {
            get { return m_iNodeCollection; }
            set { m_iNodeCollection = value; }
        }

        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        public CNLoad()
        {

        }
        public CNLoad(CNode nNode,
              bool bIsDisplayed, float fTime)
        {
            Node = nNode;
            BIsDisplayed = bIsDisplayed;
            FTime = fTime;
        }
    }
}
