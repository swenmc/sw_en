using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace M_EC3
{
    class CH_EC3_40201
    {
        float m_fM_z_Ed;
        public float FM_z_Ed
        {
            get { return m_fM_z_Ed; }
        }

        float m_fW_pl_z;
        public float FW_pl_z
        {
            get { return m_fW_pl_z; }
            set { m_fW_pl_z= value; }
        }

        float m_ff_y;

        public float Ff_y
        {
            get { return m_ff_y; }
        }

        float m_fGamma_M0;

        public float FGamma_M0
        {
            get { return m_fGamma_M0; }
        }

        float m_fM_pl_z_Rd;

        public float FM_pl_z_Rd
        {
            set { m_fM_pl_z_Rd = value; }
        }

        float fDsgRatio;

        public float FDsgRatio
        {
            get { return fDsgRatio; }
            set { fDsgRatio = value; }
        }

        public CH_EC3_40201()
        {
            m_fM_z_Ed = 70000f; //Nacitanie z databazy vysledkov

            m_fW_pl_z = 0.04f;    // Nacitanie z databazy prierezov

            m_ff_y = 235e+8f; // Nacitanie z databazy materialov
            m_fGamma_M0 = 1f; // Nacitanie z NA normy

            CL_62 obCL_62 = new CL_62();
            CL obCL = new CL();

            m_fM_pl_z_Rd = obCL_62.Eq_613_____(m_fW_pl_z, m_ff_y, m_fGamma_M0);
            fDsgRatio = obCL_62.Eq_612_____(Math.Abs(m_fM_z_Ed), m_fM_pl_z_Rd);
        }
    }
}
