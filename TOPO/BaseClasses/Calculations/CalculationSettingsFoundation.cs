using _3DTools;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using BaseClasses.GraphObj;

namespace BaseClasses
{
    [Serializable]
    public class CalculationSettingsFoundation
    {
        //To Mato dopln si property ktore potrebujes
        //-------------------------------------------------------------------------------------------------------------
        private string m_ConcreteGrade;
        private float m_ConcreteDensity;
        private string m_ReinforcementGrade;

        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        public string ConcreteGrade
        {
            get
            {
                return m_ConcreteGrade;
            }

            set
            {
                m_ConcreteGrade = value;
            }
        }

        public float ConcreteDensity
        {
            get
            {
                return m_ConcreteDensity;
            }

            set
            {
                m_ConcreteDensity = value;
            }
        }

        public string ReinforcementGrade
        {
            get
            {
                return m_ReinforcementGrade;
            }

            set
            {
                m_ReinforcementGrade = value;
            }
        }


        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        public CalculationSettingsFoundation() { }
    }
}
