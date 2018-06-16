using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using M_BASE;

namespace M_EC3
{
    // 1 Load Cross-section Geometry
    public class C_GEO__TU
    {
        //Rozne konstruktory alebo cele triedy pre jednotlive type prierezov ???

        // All
        public float m_fd;
        public float m_ft;

        public float m_ft_w;
        public float m_ft_f;

        public float m_ft_max;
        public float m_ft_min;


        public C_GEO__TU(ECrScPrType1 eProd)
        {
            // Load from Database or parametric cross-section input
           /*
            m_fd;
            m_ft; 
           */

            m_ft_w = m_ft_f = m_ft_max = m_ft_min = m_ft;
        }
    }
}
