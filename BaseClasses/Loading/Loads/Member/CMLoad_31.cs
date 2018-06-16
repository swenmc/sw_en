using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BaseClasses
{
    public class CMLoad_31 : CMLoad
    {
        //----------------------------------------------------------------------------
        private float m_fq; // Force Value
        //----------------------------------------------------------------------------
        public float Fq
        {
            get { return m_fq; }
            set { m_fq = value; }
        }

        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        public CMLoad_31()
        {
        
        
        }
        public CMLoad_31(float fq)
        {
            float Fq = fq;

        }
    }
}
