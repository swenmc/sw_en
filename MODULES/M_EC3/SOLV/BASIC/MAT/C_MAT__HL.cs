using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using M_BASE;

namespace M_EC3
{
    // 2 Cross-section Material Data
    class C_MAT__HL:C_MAT
    {
        // Characteristic Yield Strength - web
        public float m_ff_y_w;
        // Ultimate strength - web
        public float m_ff_u_w;

        // Characteristic Yield Strength - flange
        public float m_ff_y_f;
        // Ultimate strength - flange
        public float m_ff_u_f;

        // Characteristic Yield Strength - upper flange
        public float m_ff_y_fu;
        // Ultimate strength - upper flange
        public float m_ff_u_fu;

        // Characteristic Yield Strength - bottom flange
        public float m_ff_y_fb;
        // Ultimate strength - bottom flange
        public float m_ff_u_fb;

        public float m_fEps_w;
        public float m_fEps_f;
        public float m_fEps_fu;
        public float m_fEps_fb;


        public C_MAT__HL(C_GEO__HL objGeo, ECrScPrType1 eProd)
        {

          
            if (eProd != ECrScPrType1.eCrSc_wld && eProd != ECrScPrType1.eCrSc_wldnorm)
            {
                Ff_y = GetfykForT(objGeo.m_ft);
                Ff_u = GetfukForT(objGeo.m_ft);

                if (Ff_y <= 0.0f || Ff_u <= 0.0f)
                {
                    /*  
        
                    return FALSE;

                      continue;
                     * */
                }

                m_ff_y_f = m_ff_y_fu = m_ff_y_fb = Ff_y;
                m_ff_y_w = Ff_y;

                m_ff_u_f = m_ff_u_fu = m_ff_u_fb = Ff_u;
                m_ff_u_w = Ff_u;

                FEps = GetEpsForF(Ff_y);
                m_fEps_fu = m_fEps_fb = m_fEps_f = FEps;
                m_fEps_w = FEps;
            }
            else
            {
                // Upper Flange
                m_ff_y_fu = GetfykForT(objGeo.m_ft_fu);
                m_ff_u_fu = GetfukForT(objGeo.m_ft_fu);

                if (m_ff_y_fu <= 0.0f || m_ff_u_fu <= 0.0f)
                {
                    // Exception
                    //return FALSE;

                    //continue;
                }

                m_fEps_fu = GetEpsForF(m_ff_y_fu);

                // Bottom Flange
                m_ff_y_fb = GetfykForT(objGeo.m_ft_fb);
                m_ff_u_fb = GetfukForT(objGeo.m_ft_fb);

                if (m_ff_y_fb <= 0.0f || m_ff_u_fb <= 0.0f)
                {
                    // Exception
                    //return FALSE;

                    //continue;
                }

                m_fEps_fb = GetEpsForF(m_ff_y_fb);


                // Minimum for flanges
                m_ff_y_f = Math.Min(m_ff_y_fu, m_ff_y_fb);
                m_ff_u_f = Math.Min(m_ff_u_fu, m_ff_u_fb);

                m_fEps_f = GetEpsForF(m_ff_y_f);

            // Web
            m_ff_y_w = GetfykForT(objGeo.m_ft_w);
            m_ff_u_w = GetfukForT(objGeo.m_ft_w);

            if (m_ff_y_w <= 0.0f || m_ff_u_w <= 0.0f)
            {
                // Exception
                //return FALSE;

                //continue;
            }

            m_fEps_w = GetEpsForF(m_ff_y_w);


            // Main Material Parameters - minimum for all parts
            Ff_y = Math.Min(m_ff_y_f, m_ff_y_w);
            Ff_u = Math.Min(m_ff_u_f, m_ff_u_w);
            FEps = GetEpsForF();

            }
        }
    }
}
