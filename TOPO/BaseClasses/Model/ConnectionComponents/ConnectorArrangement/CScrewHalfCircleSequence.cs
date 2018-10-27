using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BaseClasses
{
    [Serializable]
    public class CScrewHalfCircleSequence : CScrewSequence
    {
        private float m_fRadius;

        public float Radius
        {
            get
            {
                return m_fRadius;
            }

            set
            {
                m_fRadius = value;
            }
        }

        public CScrewHalfCircleSequence()
        { }

        public CScrewHalfCircleSequence(float radius, int numOfScrews)
        {
            m_fRadius = radius;
            INumberOfConnectors = numOfScrews;
        }
    }
}
