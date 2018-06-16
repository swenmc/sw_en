using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using M_BASE;

namespace M_EC3
{
    // 1 Load Cross-section Geometry
    public class C_GEO___L
    {
        //Rozne konstruktory alebo cele triedy pre jednotlive type prierezov ???

        // All
        public float m_fh;
        public float m_fb;
        public float m_ft_w;
        public float m_ft_f;
        public float m_ft_a;
        public float m_ft_b;

        public float m_ft;
        public float m_fr;

        public float m_fc_a;
        public float m_fc_b;

        public float m_fAlpha_Axis;
        public float m_ft_max;
        public float m_ft_min;


        public C_GEO___L(ECrScSymmetry1 eSym, ECrScPrType1 eProd)
        {
           //For all types
            /*
            m_fb;
            m_fh;
            m_fy_S;
            m_fz_S;
            */

            // Rolled monosymmetrical
            if (eProd == ECrScPrType1.eCrSc_rold || eProd == ECrScPrType1.eCrSc_cldfrm && eSym == ECrScSymmetry1.eMS)
            {
               // m_ft;
               // m_fr;

                m_ft_a = m_ft;
                m_ft_b = m_ft;

            }
            // Rolled asymetrical
            else if (eProd == ECrScPrType1.eCrSc_rold || eProd == ECrScPrType1.eCrSc_cldfrm && eSym == ECrScSymmetry1.eAS)
            {
                m_fh = m_fb;
                m_ft_a = m_ft;
                m_ft_b = m_ft;

               // m_fr;
            }
            // Welded monosymmetrical
            else if (eProd == ECrScPrType1.eCrSc_wld || eProd == ECrScPrType1.eCrSc_wldnorm && eSym == ECrScSymmetry1.eMS)
            {
                m_fh = m_fb;

              //  m_ft;

                m_fr = 0.0f;

                //m_fz_S = m_fy_S;
                m_ft_a = m_ft;
                m_ft_b = m_ft;
            }
            // Welded asymmetrical
            else
            {
              /*
                m_fh;
                m_fb;
                m_ft_a;
                m_ft_b;
                m_fr = 0.0f;
               */

            }

           // m_fAlpha_Axis = ;

            m_fc_a = m_fh - m_ft_b - m_fr;
            m_fc_b = m_fb - m_ft_a - m_fr;

            m_ft_max = Math.Max(m_ft_a, m_ft_b);
            m_ft_min = Math.Min(m_ft_a, m_ft_b);

            m_ft_w = m_ft_a;
            m_ft_f = m_ft_b;
        }
    }
}
