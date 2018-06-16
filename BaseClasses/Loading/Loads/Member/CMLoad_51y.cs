using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
    }
}
