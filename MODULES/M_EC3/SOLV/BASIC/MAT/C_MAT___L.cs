using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using M_BASE;

namespace M_EC3
{
    // 2 Cross-section Material Data
    class C_MAT___L : C_MAT
    {
        // Characteristic Yield Strength - web
        public float m_ff_y_w;
        // Ultimate strength - web
        public float m_ff_u_w;

        // Characteristic Yield Strength - flange
        public float m_ff_y_f;
        // Ultimate strength - flange
        public float m_ff_u_f;

        public float m_fEps_w;
        public float m_fEps_f;

        public C_MAT___L(C_GEO___L objGeo, ECrScSymmetry1 eSym)
        {
            // Main Material Parameters - minimum for all parts
            Ff_y = GetfykForT(objGeo.m_ft_max);
            Ff_u = GetfukForT(objGeo.m_ft_max);

            if (Ff_y <= 0.0f || Ff_u <= 0.0f)
            {
                /*
                return FALSE;

              continue;
                 */
            }

            FEps = GetEpsForF();

            // Auxiliary assignment
            m_ff_y_w = m_ff_y_f = Ff_y;
            m_ff_u_w = m_ff_u_f = Ff_u;
            m_fEps_w = m_fEps_f = FEps;
        }
    }
}
