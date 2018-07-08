using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BaseClasses;

namespace MATERIAL
{
    // Default timber material class
    public class CMat_05_00: CMat
    {
        //// Default - timber
        //// General material properties

        //// Design properties
        //public float m_ff_yk_0;
        //public float m_ff_u_0;

        // Constructor
        public CMat_05_00()
        {
            m_sMatType = 5;
            m_fG = m_fE / (2f * (1f + m_fNu));
        }
    }
}
