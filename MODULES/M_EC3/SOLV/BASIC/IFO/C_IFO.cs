using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace M_EC3
{
    // Load Section Internal Forces
    public class C_IFO
    {
        // Internal Forces in Section

        float m_fN_Ed;

        public float FN_Ed
        {
            get { return m_fN_Ed; }
        }
        float m_fV_y_Ed;

        public float FV_y_Ed
        {
            get { return m_fV_y_Ed; }
        }
        float m_fV_z_Ed;

        public float FV_z_Ed
        {
            get { return m_fV_z_Ed; }
        }
        float m_fT_Ed;

        public float FT_Ed
        {
            get { return m_fT_Ed; }
        }
        float m_fM_y_Ed;

        public float FM_y_Ed
        {
            get { return m_fM_y_Ed; }
        }
        float m_fM_z_Ed;

        public float FM_z_Ed
        {
            get { return m_fM_z_Ed; }
        }
    }
}
