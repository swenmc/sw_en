namespace BaseClasses
{
    public class CMLoad_51z : CMLoad
    {
        //----------------------------------------------------------------------------
        private float m_ft_0_u; // Temperature Value for PCC / Upper+Z / Right+Y  (positive direction) // Teplota hore alebo vpravo na prvku / priecnom reze prierezom
        private float m_ft_0_b; // Temperature Value for PCC / Upper-Z / Right-Y  (negative direction) // Teplota dole alebo vlavo na prvku / priecnom reze prierezom
        //----------------------------------------------------------------------------
        public float Ft_0_u
        {
            get { return m_ft_0_u; }
            set { m_ft_0_u = value; }
        }

        public float Ft_0_b
        {
            get { return m_ft_0_b; }
            set { m_ft_0_b = value; }
        }
        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        public CMLoad_51z()
        {
        
        
        }
        public CMLoad_51z(float ft_0_u, float ft_0_b)
        {
            Ft_0_u = ft_0_u;
            Ft_0_b = ft_0_b;
        }
    }
}
