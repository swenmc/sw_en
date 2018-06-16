using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace M_EC3
{
    // 1 Load Basic Material Data
    class C_MAT
    {
        // Basic Material Data

        // Default strength
        // Characteristic Yield Strength
        private float m_ff_y;

        public float Ff_y
        {
            get { return m_ff_y; }
            set { m_ff_y = value; }
        }
        // Ultimate strength
        private float m_ff_u;

        public float Ff_u
        {
            get { return m_ff_u; }
            set { m_ff_u = value; }
        }

        private float m_fEps;

        public float FEps
        {
            get { return m_fEps; }
            set { m_fEps = value; }
        }

        // Young's modulus
        private float m_fE;

        public float FE
        {
            get { return m_fE; }
            set { m_fE = value; }
        }

        // Stainless Steel
        private bool m_bStainlessS;

        public bool BStainlessS
        {
            get { return m_bStainlessS; }
            set { m_bStainlessS = value; }
        }




        public C_MAT()
        {
            // Fill variable  E, stainless, from database


        }






        public float GetfykForT(float ft)
        {
            // load from database strength depending on actual thickness
            // Fill variable  ff_y
            return 0f;
        }

        public float GetfukForT(float ft)
        {
            // load from database strength depending on actual thickness
            // Fill variable ff_u
            return 0f;
        }

        public float GetEpsForF(float ff_y, float fE, bool bStainlessS)
        {
            if (!bStainlessS)
                return (float)Math.Sqrt(2.35e+8f / ff_y);
            else
                return (float)Math.Sqrt(2.35e+8f * fE / (ff_y * 2.1e+11f));
        }
        public float GetEpsForF(float ff_y)
        {
            if (!m_bStainlessS)
                return (float)Math.Sqrt(2.35e+8f / ff_y);
            else
                return (float)Math.Sqrt(2.35e+8f * m_fE / (ff_y * 2.1e+11f));
        }
        public float GetEpsForF()
        {
            if (!m_bStainlessS)
                return (float)Math.Sqrt(2.35e+8f / m_ff_y);
            else
                return (float)Math.Sqrt(2.35e+8f * m_fE / (m_ff_y * 2.1e+11f));
        }


    }
}
