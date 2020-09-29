using System;

namespace BaseClasses.GraphObj
{
    [Serializable]
    public class CAreaCircle : CEntity3D
    {
        public double m_Diameter;

        public double Diameter
        {
            get
            {
                return m_Diameter;
            }

            set
            {
                m_Diameter = value;
            }
        }

        public CAreaCircle()
        {
        
        }

        // Constructor 2
        public CAreaCircle(int iArea_ID, double diameter, int fTime)
        {
            ID = iArea_ID;
            m_Diameter = diameter;
            FTime = fTime;
        }
    }
}
