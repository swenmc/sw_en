using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using M_BASE;

namespace M_EC3
{
    // 2 Cross-section Material Data
    class C_MAT__TU:C_MAT
    {
        // Characteristic Yield Strength - web
        public float m_ff_y_w;
        // Ultimate strength - web
        public float m_ff_u_w;

        // Characteristic Yield Strength - flange
        public float m_ff_y_f;
        // Characteristic Yield Strength - upper flange
        public float m_ff_y_fu;
        // Characteristic Yield Strength - bottom flange
        public float m_ff_y_fb;
        // Ultimate strength - flange
        public float m_ff_u_f;
        // Ultimate strength - upper flange
        public float m_ff_u_fu;
        // Ultimate strength - bottom flange
        public float m_ff_u_fb;

        public float m_fEps_w;
        public float m_fEps_f;
        public float m_fEps_fu;
        public float m_fEps_fb;

        public C_MAT__TU(C_GEO__TU objGeo)
        {
            // Main Material Parameters - minimum for all parts
            Ff_y = GetfykForT(objGeo.m_ft);
            Ff_u =  GetfykForT(objGeo.m_ft);
            FEps = GetEpsForF();

            if (Ff_y <= 0.0f || Ff_u <= 0.0f)
            {
                /*
                return FALSE;

              continue;
                 */
            }

            m_ff_y_w = Ff_y;
            m_ff_y_f = m_ff_y_fu = m_ff_y_fb = Ff_y;
            m_ff_u_f = m_ff_u_fu = m_ff_u_fb = Ff_u;
            m_fEps_w = m_fEps_f = m_fEps_fu = m_fEps_fb = FEps;
        }
    }
}
