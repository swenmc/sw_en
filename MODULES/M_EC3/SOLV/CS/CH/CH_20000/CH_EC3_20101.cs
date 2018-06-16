using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace M_EC3
{
    class CH_EC3_20101
    {
        float m_fV_z_Ed;
        public float FV_z_Ed
        {
            get { return m_fV_z_Ed; }
        }

        float m_fA_vz;
        public float FA_vz
        {
            get { return m_fA_vz; }
            set { m_fA_vz= value; }
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

        float m_fV_pl_z_Rd;

        public float FV_pl_z_Rd
        {
            get { return m_fV_pl_z_Rd; }
            set { m_fV_pl_z_Rd = value; }
        }

        float fDsgRatio;

        public float FDsgRatio
        {
            get { return fDsgRatio; }
            set { fDsgRatio = value; }
        }

        public CH_EC3_20101()
        {
            m_fV_z_Ed = 70000f; //Nacitanie z databazy vysledkov

            m_fA_vz = 0.04f;    // Nacitanie z databazy prierezov

            m_ff_y = 235e+8f; // Nacitanie z databazy materialov
            m_fGamma_M0 = 1f; // Nacitanie z NA normy

            CL_62 obCL_62 = new CL_62();
            CL obCL = new CL();

            m_fV_pl_z_Rd = obCL_62.Eq_618_____(m_fA_vz, m_ff_y, m_fGamma_M0);
            fDsgRatio = obCL_62.Eq_617_____(Math.Abs(m_fV_z_Ed), m_fV_pl_z_Rd);
        }
    }
}
