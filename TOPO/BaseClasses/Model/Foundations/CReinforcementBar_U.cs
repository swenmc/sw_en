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

        public CReinforcementBar_U(int iBar_ID, string materialName, string barName, bool bBarIsInXDirection_temp, Point3D pControlEdgePoint, float fProjectionLength, float fDiameter, /*Color volColor,*/ float fvolOpacity, bool bIsTop_U, bool bIsDisplayed, float fTime)
        {
            ID = iBar_ID;
            Name = barName;
            BarIsInXDirection = bBarIsInXDirection_temp;
            m_pControlPoint = pControlEdgePoint;
            StartPoint = new Point3D(m_pControlPoint.X, m_pControlPoint.Y, m_pControlPoint.Z);
            //m_EndPoint - závisi od pootocenia

            EndPoint = new Point3D(m_pControlPoint.X + fProjectionLength, m_pControlPoint.Y, m_pControlPoint.Z);
            if (!BarIsInXDirection)
            {
                EndPoint = new Point3D(m_pControlPoint.X, m_pControlPoint.Y + fProjectionLength, m_pControlPoint.Z);
            }

            Diameter = fDiameter;
            ProjectionLength = fProjectionLength;

            //m_volColor_2 = volColor;
            Opacity = fvolOpacity;

            BIsDisplayed = bIsDisplayed;
            FTime = fTime;

            Area_As_1 = MathF.fPI * MathF.Pow2(fDiameter) / 4f; // Reinforcement bar cross-sectional area

            SetMaterialPropertiesFromDatabase(materialName);
        }

        public override List<Point3D> GetWireFramePoints_Volume(Model3DGroup volumeModel)
        {
            // TODO Ondrej - pripravit wireframe model pre cely tvar U (zlucit wireframe poinst z 5 objektov)
            return new List<Point3D>(); // Not implemented - zatial vraciam prazdny list
        }
    }
}
