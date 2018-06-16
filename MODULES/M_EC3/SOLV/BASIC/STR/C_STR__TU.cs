using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using M_BASE;

namespace M_EC3
{
    class C_STR__TU
    {
        public float m_fSigma;

        public C_STR__TU(C_IFO objIFO, C_GEO__TU objGeo, CCrSc objCrSc)
        {
            float m_fSigma_N = objIFO.FN_Ed / objCrSc.m_fA;
            float m_fSigma_My = objIFO.FM_y_Ed / objCrSc.m_fI_y * objGeo.m_fd / 2.0f;
            float m_fSigma_Mz = objIFO.FM_z_Ed / objCrSc.m_fI_z * objGeo.m_fd / 2.0f;
            m_fSigma = m_fSigma_N - Math.Max(Math.Abs(m_fSigma_My), Math.Abs(m_fSigma_Mz)); 
        }
    }
}
