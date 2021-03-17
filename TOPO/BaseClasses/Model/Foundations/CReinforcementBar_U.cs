using _3DTools;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using BaseClasses.GraphObj;
using MATH;
using DATABASE;
using DATABASE.DTO;
using BaseClasses.GraphObj.Objects_3D;

namespace BaseClasses
{
    // Class Reinforcement bar - shape U
    [Serializable]
    public class CReinforcementBar_U : CReinforcementBar
    {
        private bool m_bIsTop_U;

        public bool IsTop_U
        {
            get
            {
                return m_bIsTop_U;
            }

            set
            {
                m_bIsTop_U = value;
            }
        }

        public float m_arcRadius;

        public float ArcRadius
        {
            get
            {
                return m_arcRadius;
            }

            set
            {
                m_arcRadius = value;
            }
        }

        public float m_arcRadiusNet;

        public float ArcRadiusNet
        {
            get
            {
                return m_arcRadiusNet;
            }

            set
            {
                m_arcRadiusNet = value;
            }
        }

        public CReinforcementBar_U() { }

        public CReinforcementBar_U(int iBar_ID, string materialName, string barName, bool bBarIsInXDirection_temp, Point3D pControlEdgePoint, float fProjectionLength,
            float farcRadiusNet, float fDiameter, /*Color volColor,*/ float fvolOpacity, bool bIsTop_U, bool bIsDisplayed, float fTime)
        {
            if (string.IsNullOrEmpty(materialName)) return;
            ID = iBar_ID;
            Name = barName;
            BarIsInXDirection = bBarIsInXDirection_temp;
            ControlPoint = pControlEdgePoint;
            StartPoint = new Point3D(ControlPoint.X, ControlPoint.Y, ControlPoint.Z);
            //m_EndPoint - závisi od pootocenia

            EndPoint = new Point3D(ControlPoint.X + fProjectionLength, ControlPoint.Y, ControlPoint.Z);
            if (!BarIsInXDirection)
            {
                EndPoint = new Point3D(ControlPoint.X, ControlPoint.Y + fProjectionLength, ControlPoint.Z);
            }

            m_arcRadiusNet = farcRadiusNet;
            m_arcRadius = farcRadiusNet + 0.5f * fDiameter;
            Diameter = fDiameter;
            ProjectionLength = fProjectionLength;

            //m_volColor_2 = volColor;
            Opacity = fvolOpacity;

            m_bIsTop_U = bIsTop_U;
            BIsDisplayed = bIsDisplayed;
            FTime = fTime;

            Area_As_1 = MathF.fPI * MathF.Pow2(fDiameter) / 4f; // Reinforcement bar cross-sectional area

            SetMaterialPropertiesFromDatabase(materialName);
        }
    }
}
