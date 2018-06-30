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
    public class CConnectionJoint_C001 : CConnectionJointTypes
    {
        // Eaves Purlin to Rafter/Main Column Joint
        float m_ft;

        public CConnectionJoint_C001() { }

        public CConnectionJoint_C001(CNode Node_temp, CMember MainFrameRafter_temp, CMember Purlin_temp, bool bIsDisplayed_temp)
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

            float fControlPointYCoord1;

            if (fPurlinVectorDirection > 0)
            {
                fControlPointYCoord1 = (float)(m_Node.Y - 0.5f * m_MainMember.CrScStart.b - m_ft);
            }
            else
            {
                fControlPointYCoord1 = (float)(m_Node.Y + 0.5f * m_MainMember.CrScStart.b + m_ft);
            }

            CPoint ControlPoint_P1 = new CPoint(0, m_Node.X, fControlPointYCoord1, m_Node.Z - 0.5 * m_SecondaryMembers[0].CrScStart.h, 0);

            m_arrPlates = new CPlate[1];
            m_arrPlates[0] = new CConCom_Plate_LL(ControlPoint_P1, 0.05f, (float)m_SecondaryMembers[0].CrScStart.b, (float)m_SecondaryMembers[0].CrScStart.h, 0.05f, 0.003f, 90, 0, fRotatePlatesInJointAngle, 0, BIsDisplayed); // Rotation angle in degrees
        }
    }
}
