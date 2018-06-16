using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace M_EC3
{
    class CH_EC3_10201
    {
        float m_fN_Ed;
        public float FN_Ed
        {
            get { return m_fN_Ed; }
        }

        float m_fA;
        public float FA
        {
            get { return m_fA; }
            set { m_fA = value; }
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

        float m_fN_c_Rd;

        public float FN_c_Rd
        {
            get { return m_fN_c_Rd; }
            set { m_fN_c_Rd = value; }
        }

        float fDsgRatio;

        public float FDsgRatio
        {
            get { return fDsgRatio; }
            set { fDsgRatio = value; }
        }

        public CH_EC3_10201()
        {
            m_fN_Ed = 70000f; //Nacitanie z databazy vysledkov

            m_fA = 0.04f;    // Nacitanie z databazy prierezov

            m_ff_y = 235e+8f; // Nacitanie z databazy materialov
            m_fGamma_M0 = 1f; // Nacitanie z NA normy

            CL_62 obCL_62 = new CL_62();
            CL obCL = new CL();

            m_fN_c_Rd = obCL_62.Eq_610_____(m_fA, m_ff_y, m_fGamma_M0);
            fDsgRatio = obCL_62.Eq_69______(Math.Abs(m_fN_Ed), m_fN_c_Rd);
        }
    }
}
