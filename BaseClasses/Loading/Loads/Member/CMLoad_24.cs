using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BaseClasses
{
    public class CMLoad_24 : CMLoad
    {
        //----------------------------------------------------------------------------
        private float m_fq; // Force Value
        private float m_faA;     // Distance of Load from Member Start / User Input
        private float m_fa;      // Distance of Midpoint(Centre) of Load from Member Start 
        private float m_fs;      // Distance of Load / Length along which load acts
        //----------------------------------------------------------------------------
        public float Fq
        {
            get { return m_fq; }
            set { m_fq = value; }
        }
        public float FaA
        {
            get { return m_faA; }
            set { m_faA = value; }
        }
        public float Fa
        {
            get { return m_fa; }
            set { m_fa = value; }
        }
        public float Fs
        {
            get { return m_fs; }
            set { m_fs = value; }
        }
        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        //----------------------------------------------------------------------------
        public CMLoad_24(float fq, float faA, float fs)
        {
            Fq = fq;
            FaA = faA;
            Fs = fs;
            // Calculate Load Centre Position from Start of Member
            m_fa = m_faA + m_fs / 2.0f;
        }
    }
}
