using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace M_EC3
{
    class C_NAD
    {
        // 6.3.2.2(4) und 6.3.2.3(1)
        public float m_fLambda_rel_LT_0_IR;
        public float m_fLambda_rel_LT_0_IS;
        public float m_fBeta_LT_IR;
        public float m_fBeta_LT_IS;

        private float m_fGamma_M0;

        public float FGamma_M0
        {
            get { return m_fGamma_M0; }
            set { m_fGamma_M0 = value; }
        }

        private float m_fGamma_M1;

        public float FGamma_M1
        {
            get { return m_fGamma_M1; }
            set { m_fGamma_M1 = value; }
        }

        private float m_fGamma_M2;

        public float FGamma_M2
        {
            get { return m_fGamma_M2; }
            set { m_fGamma_M2 = value; }
        }

        // Faktor for Shear Area and Shear buckling of I-Profile
        float m_fEta;
        int m_iMaxLambdaTension;
        int m_iMaxLambdaCompres;
        // Buckling Factor Modification
        bool m_bModifikationsArt_Chi_LT;
        int m_iBiegedrillknicklinie;
        // Selected Annex A / B
        int m_iInteraktionsVerfahren;

        bool m_bEuropean_LTB_Curve;


        public C_NAD()
        {

        }
        public C_NAD(C_MAT objMat)
        {
            // Nacitat nastavenia z dialogu podla NAD

            if (m_bModifikationsArt_Chi_LT)
                m_bModifikationsArt_Chi_LT = !objMat.BStainlessS; // EN 1993-1-1, 6.3.2.3

        }
    }
}
