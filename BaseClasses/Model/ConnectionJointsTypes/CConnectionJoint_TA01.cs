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
    public class CConnectionJoint_TA01 : CConnectionJointTypes
    {
        // Main Column to Foundation Connection
        float m_ft;
        int m_iHoleNo;
        float m_fd_hole;
        float m_flip;

        public CConnectionJoint_TA01() { }

        public CConnectionJoint_TA01(CNode Node_temp, CMember MainFrameColumn_temp, bool bIsDisplayed_temp)
        {
            bIsJointDefinedinGCS = false;

            m_Node = Node_temp;
            m_MainMember = MainFrameColumn_temp;

            BIsDisplayed = bIsDisplayed_temp;

            // Plate properties
            // Todo - set correct dimensions of plate acc. to column cross-section size
            float fTolerance = 0.001f; // Gap between cross-section surface and plate surface
            float fb_plate = (float)(MainFrameColumn_temp.CrScStart.b_in - 2 * fTolerance);
            float fh_plate = (float)(MainFrameColumn_temp.CrScStart.h_in - 2 * fTolerance);
            m_ft = 0.003f;
            m_iHoleNo = 2;
            m_fd_hole = 0.02f;
            m_flip = 0.18f;

            float fAlignment_x = 0; // Odsadenie plechu od definicneho uzla pruta

            CPoint ControlPoint_P1 = new CPoint(0, fAlignment_x, /*m_MainMember.CrScStart.y_min*/ - 0.5f * fb_plate, -0.5f * fh_plate, 0);

            m_arrPlates = new CPlate[1];
            m_arrPlates[0] = new CConCom_Plate_BB_BG(ControlPoint_P1, fb_plate, fh_plate, m_flip, m_ft, m_iHoleNo, m_fd_hole, 90, 0, 90, bIsDisplayed_temp); // Rotation angle in degrees

            if (m_Node.ID != m_MainMember.NodeStart.ID) // If true - joint at start node, if false joint at end node (se we need to rotate joint about z-axis 180 deg)
            {
                // Rotate and move joint defined in the start point [0,0,0] to the end point
                ControlPoint_P1 = new CPoint(0, m_MainMember.FLength - fAlignment_x, /*m_MainMember.CrScStart.y_max*/ + 0.5f * fb_plate, -0.5f * fh_plate, 0);
                m_arrPlates[0] = new CConCom_Plate_BB_BG(ControlPoint_P1, fb_plate, fh_plate, m_flip, m_ft, m_iHoleNo, m_fd_hole, 90, 0, 180+90, bIsDisplayed_temp); // Rotation angle in degrees
            }
        }
    }
}
