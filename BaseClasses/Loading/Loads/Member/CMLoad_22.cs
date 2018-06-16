using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BaseClasses
{
    public class CMLoad_22 : CMLoad
    {
        //----------------------------------------------------------------------------
        private float m_fq; // Force Value
        private float m_fa;      // Distance from Member Start along which load acts
        //----------------------------------------------------------------------------
        public float Fq
        {
            get { return m_fq; }
            set { m_fq = value; }
        }
        public float Fa
        {
            get { return m_fa; }
            set { m_fa = value; }
        }

        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        public CMLoad_22()
        {
        
        
        }
        public CMLoad_22(float fq, float fa)
        {
            Fq = fq;
            Fa = fa;

        }
    }
}
