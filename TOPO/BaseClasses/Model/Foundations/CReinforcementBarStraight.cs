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
    // Class CReinforcementBarStraight
    [Serializable]
    public class CReinforcementBarStraight : CReinforcementBar
    {
        public CReinforcementBarStraight() { }

        public CReinforcementBarStraight(int iBar_ID, string materialName, string barName, bool bBarIsInXDirection_temp, Point3D pControlEdgePoint, float fLength, 
            float fDiameter, /*Color volColor,*/ float fvolOpacity, bool bIsDisplayed, float fTime)
        {
            if (materialName == null) return; // MessageBox.Show("tato co to");

            ID = iBar_ID;
            Name = barName;
            BarIsInXDirection = bBarIsInXDirection_temp;
            m_pControlPoint = pControlEdgePoint;
            StartPoint = new Point3D(m_pControlPoint.X, m_pControlPoint.Y, m_pControlPoint.Z);
            //m_EndPoint - závisi od pootocenia

            EndPoint = new Point3D(m_pControlPoint.X + fLength, m_pControlPoint.Y, m_pControlPoint.Z);
            if (!BarIsInXDirection)
            {
                EndPoint = new Point3D(m_pControlPoint.X, m_pControlPoint.Y + fLength, m_pControlPoint.Z);
            }

            Diameter = fDiameter;
            TotalLength = fLength;
            ProjectionLength = fLength;

            //m_volColor_2 = volColor;
            Opacity = fvolOpacity;

            BIsDisplayed = bIsDisplayed;
            FTime = fTime;

            Area_As_1 = MathF.fPI * MathF.Pow2(fDiameter) / 4f; // Reinforcement bar cross-sectional area

            SetMaterialPropertiesFromDatabase(materialName);
        }
    }
}
