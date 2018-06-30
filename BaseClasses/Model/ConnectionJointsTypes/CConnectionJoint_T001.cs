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
    public class CConnectionJoint_T001 : CConnectionJointTypes
    {
        // Girts to Main Column Joint
        float m_ft;

        public CConnectionJoint_T001() { }

        public CConnectionJoint_T001(CNode Node_temp, CMember MainFrameColumn_temp, CMember Girt_temp, bool bIsDisplayed_temp)
        {
            bIsJointDefinedinGCS = false;

            m_Node = Node_temp;
            m_MainMember = MainFrameColumn_temp;
            m_SecondaryMembers = new CMember[1];
            m_SecondaryMembers[0] = Girt_temp;

            m_ft = 0.003f;

            BIsDisplayed = bIsDisplayed_temp;

            float fPlate_Angle_Leg = 0.05f;

            // Joint is defined in start point and LCS of secondary member [0,y,z]
            // Plates are usually defined in x,y coordinates

            CPoint ControlPoint_P1 = new CPoint(0, 0.5f * (float)m_MainMember.CrScStart.b, (float)(m_SecondaryMembers[0].CrScStart.y_min), - 0.5f * m_SecondaryMembers[0].CrScStart.h, 0);
            CPoint ControlPoint_P2 = new CPoint(0, 0.5f * (float)m_MainMember.CrScStart.b, (float)(m_SecondaryMembers[0].CrScStart.y_max), - 0.5f * m_SecondaryMembers[0].CrScStart.h, 0);

            m_arrPlates = new CPlate[2];
            m_arrPlates[0] = new CConCom_Plate_F_or_L(ControlPoint_P1, fPlate_Angle_Leg, (float)m_SecondaryMembers[0].CrScStart.h, fPlate_Angle_Leg, m_ft, 90, 0, 0, 0, BIsDisplayed); // Rotation angle in degrees
            m_arrPlates[1] = new CConCom_Plate_F_or_L(ControlPoint_P2, fPlate_Angle_Leg, (float)m_SecondaryMembers[0].CrScStart.h, fPlate_Angle_Leg, m_ft, 90, 0, 90, 0, BIsDisplayed); // Rotation angle in degrees

            // Identification of current joint node location (start or end definition node of secondary member)
            if (m_Node.ID != m_SecondaryMembers[0].NodeStart.ID) // If true - joint at start node, if false joint at end node (se we need to rotate joint about z-axis 180 deg)
            {
                // Rotate and move joint defined in the start point [0,0,0] to the end point
                ControlPoint_P1 = new CPoint(0, m_SecondaryMembers[0].FLength - 0.5f * (float)m_MainMember.CrScStart.b, (float)(m_SecondaryMembers[0].CrScStart.y_max), - 0.5f * m_SecondaryMembers[0].CrScStart.h, 0);
                ControlPoint_P2 = new CPoint(0, m_SecondaryMembers[0].FLength - 0.5f * (float)m_MainMember.CrScStart.b, (float)(m_SecondaryMembers[0].CrScStart.y_min), - 0.5f * m_SecondaryMembers[0].CrScStart.h, 0);

                m_arrPlates[0] = new CConCom_Plate_F_or_L(ControlPoint_P1, fPlate_Angle_Leg, (float)m_SecondaryMembers[0].CrScStart.h, fPlate_Angle_Leg, m_ft, 90, 0, 180+0, 0, BIsDisplayed); // Rotation angle in degrees
                m_arrPlates[1] = new CConCom_Plate_F_or_L(ControlPoint_P2, fPlate_Angle_Leg, (float)m_SecondaryMembers[0].CrScStart.h, fPlate_Angle_Leg, m_ft, 90, 0, 180+90, 0, BIsDisplayed); // Rotation angle in degrees
            }
        }
    }
}
