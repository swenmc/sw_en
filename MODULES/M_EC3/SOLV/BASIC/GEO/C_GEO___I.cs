using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using M_BASE;

namespace M_EC3
{
    // 1 Load Cross-section Geometry
    public class C_GEO___I
    {
        //Rozne konstruktory alebo cele triedy pre jednotlive type prierezov ???

        // All
        public float m_fh;
        public float m_fb;
        public float m_fh_w;
        public float m_ft_w;
        public float m_ft_max;
        public float m_ft_min;

        // Doubly symmetrical I

        public float m_ft_f;
        public float m_fr;
        public float m_fc_w;
        public float m_fc_f;


        // Mono - symmetrical I
        public float m_fb_fu;
        public float m_ft_fu;
        public float m_fb_fb;
        public float m_ft_fb;
        public float m_fc_fu;
        public float m_fc_fb;
        public float m_fr_su;
        public float m_fr_sb;


        public C_GEO___I(ECrScSymmetry1 eSym, ECrScPrType1 eProd)
        {

            // Dimmensions
            if (eSym == ECrScSymmetry1.eDS)
            {
                // Doubly symmetrical I
                /////////////////////////////////////////////////////////
                // Load from database
                /*
                m_fh;
                m_fb;
                m_ft_w;
                m_ft_f;
                */

                if (eProd == ECrScPrType1.eCrSc_rold)
                {
                    // load
                    //m_fr;
                }
                else
                {
                    // fload a_s
                    float fa_s = 0f;
                    /////////////////////////////////////////////////////////

                     m_fr = (float)Math.Sqrt(2.0f) * fa_s;
                }


                m_fc_w = m_fh - 2.0f * m_ft_f - 2.0f * m_fr;
                m_fc_f = m_fb / 2.0f - m_ft_w / 2.0f - m_fr;

                m_fh_w = m_fh - 2.0f * m_ft_f;

                m_ft_max = Math.Max(m_ft_w, m_ft_f);
                m_ft_min = Math.Min(m_ft_w, m_ft_f);




            }
            else
            {
                // Mono symmetrical I
                /////////////////////////////////////////////////////////
               /*
                m_fh;
                m_fb_fu;
                m_ft_fu;
                m_ft_w;
                m_fb_fb;
                m_ft_fb;
                */

                // load
                float fa_su = 0f;
                float fa_sb = 0f;

                //////////////////////////////////////////////////////////

                m_fr_su = (float)Math.Sqrt(2.0f) * fa_su;
                m_fr_sb = (float)Math.Sqrt(2.0f) * fa_sb;

                m_fc_w = m_fh - m_ft_fu - m_ft_fb - m_fr_su - m_fr_sb;
                m_fc_fu = m_fb_fu / 2.0f - m_ft_w / 2.0f - m_fr_su;
                m_fc_fb = m_fb_fb / 2.0f - m_ft_w / 2.0f - m_fr_sb;

                m_fh_w = m_fh - m_ft_fu - m_ft_fb;

                // Pouzit funkciu s params  - lubovolny pocet premennych
                m_ft_max = Math.Max(m_ft_w, m_ft_fu);
                m_ft_max = Math.Max(m_ft_max, m_ft_fb);

                m_ft_min = Math.Min(m_ft_w, m_ft_fu);
                m_ft_min = Math.Min(m_ft_min, m_ft_fb);
            }
        }
    }
}
