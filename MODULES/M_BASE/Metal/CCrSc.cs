using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace M_EC3
{
    public class CCrSc
    {
        // Gross Cross-section Area
        public float m_fA;
        // Shear Area
        public float m_fA_v_y;
        public float m_fA_v_z;

        // Second moment of Area / Moment of Inertia
        public float m_fI_y;

        public float FI_y
        {
            get { return m_fI_y; }
        }

        public float m_fI_z;

        public float FI_z
        {
            get { return m_fI_z; }
        }
        public float m_fI_t;

        //
        public float m_fI_yy;
        public float m_fI_zz;
        public float m_fI_yz;

        public float m_fy_S;
        public float m_fz_S;

        // 
        float m_fi_y;
        float m_fi_z;

        public float m_fW_el_y_min;
        public float m_fW_el_z_min;
        public float m_fW_el_y_max;
        public float m_fW_el_z_max;
        public float m_fW_pl_y;
        public float m_fW_pl_z;

        public float m_fy_M;
        public float m_fz_M;

        float m_f_M;

        public float m_fI_w;

        public float m_f_y;
        public float m_f_z;

        // First Moment of Area
        public float m_fS_y;
        public float m_fS_z;

        public CCrSc()
        {

            // Load variables from database



            // Calculate additional variables
            m_fi_y = (float)Math.Sqrt(m_fI_y / m_fA);
            m_fi_z = (float)Math.Sqrt(m_fI_z / m_fA);
            m_f_M = (float)Math.Sqrt((m_fI_y + m_fI_z) / m_fA + m_fy_M * m_fy_M + m_fz_M * m_fz_M);
        }
    }
}
