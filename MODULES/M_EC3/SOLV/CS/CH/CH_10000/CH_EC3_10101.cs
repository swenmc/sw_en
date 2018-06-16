using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace M_EC3
{
    class CH_EC3_10101
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

        /*
        float m_ff_u;

        public float Ffu
        {
            get { return m_ff_u; }
        }

        float m_fGamma_M2;

        public float FGamma_M2
        {
            get { return m_fGamma_M2; }
        }
        */

        float m_fN_t_Rd;

        public float FN_t_Rd
        {
            get { return m_fN_t_Rd; }
            set { m_fN_t_Rd = value; }
        }

        float fDsgRatio;

        public float FDsgRatio
        {
            get { return fDsgRatio; }
            set { fDsgRatio = value; }
        }

        public CH_EC3_10101(CCrSc objCrSc, C_MAT objMat)
        {
            m_fN_Ed = 70000f; //Nacitanie z databazy vysledkov

            m_fA = objCrSc.m_fA;    // Nacitanie z databazy prierezov

            m_ff_y = objMat.Ff_y; // Nacitanie z databazy materialov
            m_fGamma_M0 = 1f; // Nacitanie z NA normy

            CL_62 obCL_62 = new CL_62();
            CL obCL = new CL();

            // Auxiliary temporary variables
            /*
            bool  bHoleExist = false; // No holes implemented
            int   iConCat = -1; // No connection cathegory implemented
            float fA_net = m_fA; // Net area not implemented (holes or opennings are not allowed)
            */

            //m_fN_t_Rd = obCL.Get_fN_t_Rd(bHoleExist, iConCat, m_fA, fA_net, m_ff_y, m_ff_u, m_fGamma_M0, m_fGamma_M2);
            
            m_fN_t_Rd = obCL_62.Eq_66______(m_fA, m_ff_y, m_fGamma_M0);
            fDsgRatio = obCL_62.Eq_65______(Math.Abs(m_fN_Ed), m_fN_t_Rd);
        }
    }
}
