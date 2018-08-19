namespace BaseClasses
{
    public class CMLoad_34 : CMLoad
    {
        //----------------------------------------------------------------------------
        private float m_fq; // Force Value
        private float m_fa;      // Distance of Load from Member Start
        private float m_fs;      // Distance of Load / Length along which load acts
        //----------------------------------------------------------------------------
        public float Fq
        {
            get { return m_fq; }
            set { m_fq = value; }
        }
        public float Fa
        {
            get { return m_fa; }
            set { m_fa = value; }
        }
        public float Fs
        {
            get { return m_fs; }
            set { m_fs = value; }
        }

        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        public CMLoad_34()
        {
        
        
        }
        public CMLoad_34(float fq, float fa, float fs)
        {
            Fq = fq;
            Fa = fa;
            Fs = fs;

        }
    }
}
