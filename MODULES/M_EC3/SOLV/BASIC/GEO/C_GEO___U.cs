using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using M_BASE;

namespace M_EC3
{
    // 1 Load Cross-section Geometry
    public class C_GEO___U
    {
        //Rozne konstruktory alebo cele triedy pre jednotlive type prierezov ???

        // All
        public float m_fh;
        public float m_fb;
        public float m_fh_w;
        public float m_ft_w;
        public float m_ft_max;
        public float m_ft_min;

        public float m_ft_f;
        public float m_fr;
        public float m_fc_w;
        public float m_fc_f;

        public float m_fb_fu;
        public float m_ft_fu;
        public float m_fb_fb;
        public float m_ft_fb;

        public C_GEO___U(ECrScPrType1 eProd)
        {
            if (eProd != ECrScPrType1.eCrSc_wld && eProd != ECrScPrType1.eCrSc_wldnorm)
            {
                /*
              m_fh   
              m_fb   
              m_ft_w 
              m_ft_f 
              m_fr 
                 */
            }
            else
            {
                /*
              m_fh     
              m_fb_fu  
              m_ft_fu  
              m_ft_w   
              m_fb_fb  
              m_ft_fb
                 */

                if (m_fb_fu != m_fb_fb || m_ft_fu != m_ft_fb)      // 6.2.9.1(5)
                {
                    /*
                    return FALSE;

                  continue;
                     */
                }

                m_fb = m_fb_fu;
                m_ft_f = m_ft_fu;
                m_fr = 0.0f;
            }

            //"e_y", ry_S); ry_S /= 1.e3f;  

            m_fc_w = m_fh - 2.0f * m_ft_f - 2.0f * m_fr;
            m_fc_f = m_fb - m_ft_w - m_fr;

            m_fh_w = m_fh - 2.0f * m_ft_f;

            m_ft_max = Math.Max(m_ft_w, m_ft_f);
            m_ft_min = Math.Min(m_ft_w, m_ft_f);
        }
    }
}