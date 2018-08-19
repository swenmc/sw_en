namespace BaseClasses
{
    public class CMLoad_11 : CMLoad
    {
        //----------------------------------------------------------------------------
        private float m_fF;      // Force Value
        private float m_fa;      // Distance from Member Start
        //----------------------------------------------------------------------------
        public float FF
        {
            get { return m_fF; }
            set { m_fF = value; }
        }
        public float Fa
        {
            get { return m_fa; }
            set { m_fa = value; }
        }


        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        public CMLoad_11()
        {
        
        
        }


        public CMLoad_11(float fF, float fa)
        {
            m_fF = fF;
            m_fa = fa;
        }
    }
}
