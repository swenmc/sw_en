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
        private string m_ConcreteGrade;
        private float m_ConcreteDensity;
        private string m_ReinforcementGrade;
        private float m_SoilReductionFactor_Phi;
        private float m_SoilReductionFactorEQ_Phi;
        private float m_SoilBearingCapacity;
        private float m_FloorSlabThickness;

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

        public float SoilReductionFactor_Phi
        {
            get
            {
                return m_SoilReductionFactor_Phi;
            }

            set
            {
                m_SoilReductionFactor_Phi = value;
            }
        }

        public float SoilReductionFactorEQ_Phi
        {
            get
            {
                return m_SoilReductionFactorEQ_Phi;
            }

            set
            {
                m_SoilReductionFactorEQ_Phi = value;
            }
        }

        public float SoilBearingCapacity
        {
            get
            {
                return m_SoilBearingCapacity;
            }

            set
            {
                m_SoilBearingCapacity = value;
            }
        }

        public float FloorSlabThickness
        {
            get
            {
                return m_FloorSlabThickness;
            }

            set
            {
                m_FloorSlabThickness = value;
            }
        }


        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------
        public CalculationSettingsFoundation() { }
    }
}
