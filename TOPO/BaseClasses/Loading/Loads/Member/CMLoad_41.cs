namespace BaseClasses
{
    public class CMLoad_41 : CMLoad
    {
        //----------------------------------------------------------------------------
        private float m_fq; // Force Value
        private float m_fa;      // Slope projection
        private float m_fb;      // Distance of horiyontal plateau of Load / User Input
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
        public float Fb
        {
            get { return m_fb; }
            set { m_fb = value; }
        }

        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        public CMLoad_41()
        {
        
        
        }
        public CMLoad_41(float fq, float fa, float fb)
        {
            Fq = fq;
            Fa = fa;
            Fb = fb;

        }
    }
}
