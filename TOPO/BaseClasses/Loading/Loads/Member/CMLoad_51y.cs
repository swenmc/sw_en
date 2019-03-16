using System;
namespace BaseClasses
{
    public class CMLoad_51y : CMLoad
    {
        //----------------------------------------------------------------------------
        private float m_ft_0_r; // Temperature Value for PCC / Upper+Z / Right+Y  (positive direction) // Teplota hore alebo vpravo na prvku / priecnom reze prierezom
        private float m_ft_0_l; // Temperature Value for PCC / Upper-Z / Right-Y  (negative direction) // Teplota dole alebo vlavo na prvku / priecnom reze prierezom
        //----------------------------------------------------------------------------
        public float Ft_0_r
        {
            get { return m_ft_0_r; }
            set { m_ft_0_r = value; }
        }

        public float Ft_0_l
        {
            get { return m_ft_0_l; }
            set { m_ft_0_l = value; }
        }
        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        public CMLoad_51y()
        {
        
        
        }

        public CMLoad_51y(float ft_0_r, float ft_0_l)
        {
            Ft_0_r = ft_0_r;
            Ft_0_l = ft_0_l;
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
