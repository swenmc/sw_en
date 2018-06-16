using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using M_BASE;

namespace M_EC3
{
    class C___L : CL_55
    {
        public float    m_ft,
                        m_fc,
                        m_fct,
                        m_fht,
                        m_fbh2t,
                        m_fLambda_3,
                        m_fLambda_3_a,
                        m_fLambda_3_b;

        // Whole Cross-section
        public int m_iClass;

        public C___L(C_GEO___L objGeo, C_MAT___L objMat, C_STR___L objStr)
        {
            //Table 5.2
            GetClassTab52_OUT_L(objStr.m_fSigma_x_Ed_min,
                         objGeo.m_fh,
                         objGeo.m_fb,
                         objGeo.m_ft_a,
                         objGeo.m_ft_b,
                         objGeo.m_fr,
                         objMat.FEps,
                         m_ft,
                         m_fc,
                         m_fct,
                         m_fht,
                         m_fbh2t,
                         m_fLambda_3,
                         m_fLambda_3_a,
                         m_fLambda_3_b,
                         m_iClass,
                         objMat.BStainlessS);
        }
    }
}
