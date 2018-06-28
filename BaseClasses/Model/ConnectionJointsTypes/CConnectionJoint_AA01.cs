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
    public class CConnectionJoint_AA01 : CConnectionJointTypes
    {
        // Main Column to Foundation Connection
        float m_ft;

        public CConnectionJoint_AA01() { }

        public CConnectionJoint_AA01(CNode Node_temp, CMember MainFrameColumn_temp, bool bIsDisplayed_temp)
        {
            m_Node = Node_temp;
            m_MainMember = MainFrameColumn_temp;

            m_ft = 0.003f;

            BIsDisplayed = bIsDisplayed_temp;

            // Todo - set correct dimensions of plate acc. to column cross-section size
            float fb_plate = (float)(0.85 * MainFrameColumn_temp.CrScStart.b);
            float fh_plate = (float)(0.9 * MainFrameColumn_temp.CrScStart.h);
            int iHoleNo = 2;
            float fd_hole = 0.02f;

            CPoint ControlPoint_P1 = new CPoint(0, m_Node.X + 0.5f * fh_plate, m_Node.Y - 0.5f * fb_plate, m_Node.Z, 0);

            m_arrPlates = new CPlate[1];
            m_arrPlates[0] = new CConCom_Plate_BB_BG(ControlPoint_P1, fb_plate, fh_plate, 0.18f, m_ft, iHoleNo, fd_hole, 0 ,0 ,90 ,bIsDisplayed_temp); // Rotation angle in degrees
        }
    }
}
