using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using M_BASE;

namespace M_EC3
{
    // 1 Load Cross-section Geometry
    public class C_GEO__HL
    {
        //Rozne konstruktory alebo cele triedy pre jednotlive type prierezov ???

        // All
        public float m_fh;
        public float m_fb;
        public float m_ft;

        public float m_fh_w;

        public float m_ft_w;
        public float m_ft_wl;
        public float m_ft_wr;

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
        public float m_fc_fu;
        public float m_fc_fb;
  


        public C_GEO__HL(ECrScPrType1 eProd)
        {

            // Dimmensions
            if (eProd != ECrScPrType1.eCrSc_wld && eProd != ECrScPrType1.eCrSc_wldnorm)
            {
                
                float ft_wl;
                float ft_wr;
                // Rolled
                /////////////////////////////////////////////////////////
                // Load from database
                /*
                m_fh;
                m_fb;
                m_ft;  
                m_fr; 
                */

                m_fc_w = m_fh - 2.0f * m_ft - 2.0f * m_fr;
                m_fc_f = m_fb - 2.0f * m_ft - 2.0f * m_fr;

                m_fh_w = m_fh - 2.0f * m_ft;

                m_ft_w = ft_wl = ft_wr = m_ft;
                m_ft_f = m_ft_fu = m_ft_fb = m_ft;

                m_ft_fu = m_ft;                                             // 6.2.9.1(5)
                m_ft_fb = m_ft;

                m_ft_max = m_ft;
                m_ft_min = m_ft;
            }
            else
            {


                // Welded
                /////////////////////////////////////////////////////////
                /*
                m_fh  
                m_fb  
                m_ft_wl;
                m_ft_wr;
                m_ft_fu 
                m_ft_fb 
                 */

                if (m_ft_wl != m_ft_wr)
                {
                    // Exception
                   /*
                    return FALSE;

                    continue;
                    * */
                }

                //////////////////////////////////////////////////////////

                m_fc_w = m_fh - m_ft_fu - m_ft_fb;
                m_fc_f = m_fb - m_ft_wl - m_ft_wr;

                m_fh_w = m_fc_w;

                m_ft_w = m_ft_wl;
                m_ft_f = Math.Min(m_ft_fu, m_ft_fb);

                // Pouzit funkciu params
                m_ft_max = Math.Max(m_ft_fu, m_ft_fb);
                m_ft_max = Math.Max(m_ft_max, m_ft_w);

                m_ft_min = Math.Min(m_ft_fu, m_ft_fb);
                m_ft_min = Math.Min(m_ft_min, m_ft_w); 
            }
        }
    }
}
