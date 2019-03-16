using System;

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
