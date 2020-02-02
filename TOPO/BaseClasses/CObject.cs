using System;

namespace BaseClasses
{
    [Serializable]
    abstract public class CObject
    {
        int m_ID; // Unique object ID

        public int ID
        {
            get { return m_ID; }
            set { m_ID = value; }
        }

        float m_fTime;

        public float FTime
        {
            get { return m_fTime; }
            set { m_fTime = value; }
        }

        bool m_bIsGenerated;

        public bool BIsGenerated
        {
            get { return m_bIsGenerated; }
            set { m_bIsGenerated = value; }
        }

        private bool m_bIsDebugging;

        public bool BIsDebugging
        {
            get { return m_bIsDebugging; }
            set { m_bIsDebugging = value; }
        }

        private string m_Name;

        public string Name
        {
            get { return m_Name; }
            set { m_Name = value; }
        }

        private string m_Prefix;

        public string Prefix
        {
            get { return m_Prefix; }
            set { m_Prefix = value; }
        }

        //----------------------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------------------------------
        public CObject()
        {
            // Set as default property that object is generated
            m_bIsGenerated = true;
        }

        //----------------------------------------------------------------------------------------------------------------
        public void Delete()
        { }

        public void Edit()
        { }

        public void Draw()
        { }

        // Refresh, modify, update, redraw, copy, changeID .....
    }
}
