using System;
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

        // Simply supported beam
        // Docasne, hodnoty reakcii zavisia od typu podopretia pruta - rozpracovane v projkte FEM_CALC
        public override float Get_SSB_SupportReactionValue_RA_Start(float fL)
        {
            throw new NotImplementedException();
        }
        public override float Get_SSB_SupportReactionValue_RB_End(float fL)
        {
            throw new NotImplementedException();
        }
        public override float Get_SSB_V_max(float fL)
        {
            throw new NotImplementedException();
        }
        public override float Get_SSB_M_max(float fL)
        {
            throw new NotImplementedException();
        }
        public override float Get_SSB_V_x(float fx, float fL)
        {
            throw new NotImplementedException();
        }
        public override float Get_SSB_M_x(float fx, float fL)
        {
            throw new NotImplementedException();
        }
        public override float Get_SSB_Delta_max(float fL, float fE, float fI)
        {
            throw new NotImplementedException();
        }
        public override float Get_SSB_Delta_x(float fx, float fL, float fE, float fI)
        {
            throw new NotImplementedException();
        }
    }
}
