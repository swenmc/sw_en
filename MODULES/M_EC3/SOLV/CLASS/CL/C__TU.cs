using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace M_EC3
{
    class C__TU : CL_55
    {

        public float m_fdt,
                     m_fLambda_1,
                     m_fLambda_2,
                     m_fLambda_3;
        public int m_iClass;

        public C__TU(C_GEO__TU objGeo, C_MAT__TU objMat, C_IFO objIFO, C_NAD objNAD, C_STR__TU objStr)
        {
            // Table 5.2
            GetClassTab52_TU(objStr.m_fSigma,
                           objIFO.FN_Ed,
                           objGeo.m_fd,
                           objGeo.m_ft,
                           objMat.FEps,
                           m_fdt,
                           m_fLambda_1,
                           m_fLambda_2,
                           m_fLambda_3,
                           m_iClass,
                           objMat.BStainlessS);

            if (m_iClass == 4)
            {
                // Error 1002

                /*
                 return FALSE;

               continue;
                 */
            }
        }
    }
}
