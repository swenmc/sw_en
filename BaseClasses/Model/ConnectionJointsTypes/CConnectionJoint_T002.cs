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
    public class CConnectionJoint_T002 : CConnectionJointTypes
    {
        // Eaves Purlin to Rafter/Main Column Joint
        float m_ft;
        float m_ft_main_plate;
        float m_fPlate_Angle_Leg;

        public CConnectionJoint_T002() { }

        public CConnectionJoint_T002(CNode Node_temp, CMember MainFrameColumn_temp, CMember EavesPurlin_temp, float ft_temp_main_plate, bool bIsDisplayed_temp)
        {
            bIsJointDefinedinGCS = false;

            m_Node = Node_temp;
            m_MainMember = MainFrameColumn_temp;
            m_SecondaryMembers = new CMember[1];
            m_SecondaryMembers[0] = EavesPurlin_temp;
            m_ft_main_plate = ft_temp_main_plate; // Thickness of plate in knee joint of the frame (main column and rafter)
            BIsDisplayed = bIsDisplayed_temp;

            m_ft = 0.003f;
            m_fPlate_Angle_Leg = 0.05f;

            // Joint is defined in start point and LCS of secondary member [0,y,z]
            // Plates are usually defined in x,y coordinates

            CPoint ControlPoint_P1 = new CPoint(0, 0.5f * (float)m_MainMember.CrScStart.b + m_ft_main_plate, (float)(m_SecondaryMembers[0].CrScStart.y_min - m_fPlate_Angle_Leg), -0.5f * m_SecondaryMembers[0].CrScStart.h, 0);

            m_arrPlates = new CPlate[1];
            m_arrPlates[0] = new CConCom_Plate_LL(ControlPoint_P1, m_fPlate_Angle_Leg, (float)m_SecondaryMembers[0].CrScStart.b, (float)m_SecondaryMembers[0].CrScStart.h, m_fPlate_Angle_Leg, m_ft, 90, 0, 90, 0, BIsDisplayed); // Rotation angle in degrees

            // Identification of current joint node location (start or end definition node of secondary member)
            if (m_Node.ID != m_SecondaryMembers[0].NodeStart.ID) // If true - joint at start node, if false joint at end node (se we need to rotate joint about z-axis 180 deg)
            {
                // Rotate and move joint defined in the start point [0,0,0] to the end point
                ControlPoint_P1 = new CPoint(0, m_SecondaryMembers[0].FLength - 0.5f * (float)m_MainMember.CrScStart.b - m_ft_main_plate, (float)(m_SecondaryMembers[0].CrScStart.y_max + m_fPlate_Angle_Leg), -0.5f * m_SecondaryMembers[0].CrScStart.h, 0);

                m_arrPlates[0] = new CConCom_Plate_LL(ControlPoint_P1, m_fPlate_Angle_Leg, (float)m_SecondaryMembers[0].CrScStart.b, (float)m_SecondaryMembers[0].CrScStart.h, m_fPlate_Angle_Leg, m_ft, 90, 0, 180 + 90, 0, BIsDisplayed); // Rotation angle in degrees
            }
        }
    }
}
