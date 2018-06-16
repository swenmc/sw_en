using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace M_EC3
{
    class CH_EC3_30102
    {
        float m_fT_Ed;
        public float FT_Ed
        {
            get { return m_fT_Ed; }
        }

        float m_fT_t_Ed;
        public float FT_t_Ed
        {
            get { return m_fT_t_Ed; }
        }

        float m_fT_w_Ed;
        public float FT_w_Ed
        {
            get { return m_fT_w_Ed; }
        }

        float m_fTau_Ed;
        public float FTau_Ed
        {
            set { m_fTau_Ed = value; }
        }

        float m_fTau_t_Ed;
        public float FTau_t_Ed
        {
             set { m_fTau_t_Ed = value; }
        }

        float m_fW_t;

        public float FW_t
        {
            get { return m_fW_t; }
            set { m_fW_t = value; }
        }

        float m_ft_max;

        public float FT_max
        {
            get { return m_ft_max; }
            set { m_ft_max = value; }
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

        float m_fT_Rd;

        public float FT_Rd
        {
            get { return m_fT_Rd; }
            set { m_fT_Rd = value; }
        }

        float fDsgRatio;

        public float FDsgRatio
        {
            get { return fDsgRatio; }
            set { fDsgRatio = value; }
        }

        public CH_EC3_30102()
        {
            m_fT_Ed = 70000f; //nacitanie z databazy vysledkov

            m_fW_t = 0.14f;    // Nacitanie z databazy prierezov
            //m_ft_max = 0.005f;   // Nacitanie y vlastnosti prierezu
            // bool bCrSc_closed = false; // Nacitanie z databazy prierezov

            m_ff_y = 235e+8f; // Nacitanie z databazy materialov
            m_fGamma_M0 = 1f; // Nacitanie z NA normy

            CL_62 obCL_62 = new CL_62();
            CL obCL = new CL();

            m_fTau_t_Ed = obCL.Get_fTau_t_solid(m_fT_t_Ed, m_fW_t);
            m_fT_Rd = m_fTau_t_Ed * m_ff_y / ((float)Math.Sqrt(3f) * m_fGamma_M0);
            fDsgRatio = obCL_62.Eq_623_____(Math.Abs(m_fT_Ed), m_fT_Rd);
        }
    }
}
