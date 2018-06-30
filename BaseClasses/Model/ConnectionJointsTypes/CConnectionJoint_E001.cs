using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using BaseClasses.GraphObj;
using MATH;


namespace BaseClasses
{
    public class CConnectionJoint_E001 : CConnectionJointTypes
    {
        // Purlin to Rafter Joint
        float m_ft;

        public CConnectionJoint_E001() { }

        public CConnectionJoint_E001(CNode Node_temp, CMember MainFrameRafter_temp, CMember Purlin_temp, bool bIsDisplayed_temp)
        {
            bIsJointDefinedinGCS = true;

            m_Node = Node_temp;
            m_MainMember = MainFrameRafter_temp;
            m_SecondaryMembers = new CMember[1];
            m_SecondaryMembers[0] = Purlin_temp;

            m_ft = 0.003f;

            BIsDisplayed = bIsDisplayed_temp;

            float fPurlinVectorDirection = m_SecondaryMembers[0].NodeEnd.X - m_Node.X; // If positive rotate joint plates 0 deg, if negative rotate 180 deg
            float fRotatePlatesInJointAngle = fPurlinVectorDirection > 0 ? 0 : 180;

            // Auxiliary values

            // Local coordinates (cross-section system y/z)
            float fy = (float)(((0.5f * m_SecondaryMembers[0].CrScStart.b + m_ft)* Math.Cos(-m_SecondaryMembers[0].DTheta_x)) + (0.5f * m_SecondaryMembers[0].CrScStart.h * Math.Sin(-m_SecondaryMembers[0].DTheta_x)));
            float fz = (float)(0.5f * m_SecondaryMembers[0].CrScStart.h / Math.Cos(-m_SecondaryMembers[0].DTheta_x));

            float fControlPointYCoord1;

            if (fPurlinVectorDirection > 0)
            {
                fControlPointYCoord1 = (float)(m_Node.Y - 0.5f * m_MainMember.CrScStart.b - m_ft);
            }
            else
            {
                fControlPointYCoord1 = (float)(m_Node.Y + 0.5f * m_MainMember.CrScStart.b + m_ft);
            }

            float rotation_x_member = -(float)(m_SecondaryMembers[0].DTheta_x / MathF.dPI * 180); // Conversion to deg

            CPoint ControlPoint_P1 = new CPoint(0, m_Node.X + fy, fControlPointYCoord1, m_Node.Z - fz, 0);

            m_arrPlates = new CPlate[1];
            m_arrPlates[0] = new CConCom_Plate_LL(ControlPoint_P1, 0.05f, (float)m_SecondaryMembers[0].CrScStart.b, (float)m_SecondaryMembers[0].CrScStart.h, 0.05f, 0.003f, 90, rotation_x_member, fRotatePlatesInJointAngle, 0, BIsDisplayed); // Rotation angle in degrees
        }
    }
}
