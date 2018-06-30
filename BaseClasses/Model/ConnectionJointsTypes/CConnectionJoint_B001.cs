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
    public class CConnectionJoint_B001 : CConnectionJointTypes
    {
        // Rafter to Main Column Joint - Knee Joint

        float m_fb_1;
        float m_fb_2;
        float m_fh_1;
        float m_fh_2;
        float m_ft;
        float m_fSlope_rad;

        public CConnectionJoint_B001() { }

        public CConnectionJoint_B001(CNode Node_temp, CMember MainFrameColumn_temp, CMember MainFrameRafter_temp, float fSLope_rad_temp, float fb_2_temp, float fh_1_temp, float ft, bool bIsDisplayed_temp)
        {
            bIsJointDefinedinGCS = true;

            m_Node = Node_temp;
            m_MainMember = MainFrameColumn_temp;
            m_SecondaryMembers = new CMember[1];
            m_SecondaryMembers[0] = MainFrameRafter_temp;

            m_fb_2 = fb_2_temp;
            m_fh_1 = fh_1_temp;

            m_fSlope_rad = fSLope_rad_temp;

            m_fb_1 = (float)m_MainMember.CrScStart.h;
            m_fh_2 = m_fh_1 + (float)Math.Tan(m_fSlope_rad) * m_fb_2;
            m_ft = ft;

            float ftemp_a = 0.5f * (float)m_SecondaryMembers[0].CrScStart.h / (float)Math.Cos(m_fSlope_rad);
            float ftemp_b = 0.5f * (float)m_SecondaryMembers[0].CrScStart.h / (float)Math.Cos(m_fSlope_rad);

            BIsDisplayed = bIsDisplayed_temp;

            // Find left top edge intersection between column and rafter
            // Todo prepracovat a pokial mozno zjednodusit, potrebujeme len ziskat polohu plechu v globalnom smere Z

            Point Line1_Start = new Point();
            Point Line1_End = new Point();
            Point Line2_Start = new Point();
            Point Line2_End = new Point();

            float fRafterVectorDirection = m_SecondaryMembers[0].NodeEnd.X - m_Node.X; // If positive rotate joint plates 0 deg, if negative rotate 180 deg
            float fRotatePlatesInJointAngle = fRafterVectorDirection > 0 ? 0 : 180;
            float fDistanceX = fRafterVectorDirection > 0 ? -0.5f * (float)m_MainMember.CrScStart.h : 0.5f * (float)m_MainMember.CrScStart.h;

            Line1_Start.X = m_MainMember.NodeStart.X + fDistanceX; // Column
            Line1_Start.Y = m_MainMember.NodeStart.Z;
            Line1_End.X = m_MainMember.NodeEnd.X + fDistanceX;
            Line1_End.Y = m_MainMember.NodeEnd.Z;

            Line2_Start.X = m_SecondaryMembers[0].NodeStart.X; // Rafter
            Line2_Start.Y = m_SecondaryMembers[0].NodeStart.Z + (0.5f * m_SecondaryMembers[0].CrScStart.h) / Math.Cos(fSLope_rad_temp);
            Line2_End.X = m_SecondaryMembers[0].NodeEnd.X;
            Line2_End.Y = m_SecondaryMembers[0].NodeEnd.Z + (0.5f * m_SecondaryMembers[0].CrScStart.h) / Math.Cos(fSLope_rad_temp);

            Point pUpperLeftPointOfPlate = new Point();
            pUpperLeftPointOfPlate = (Point)Intersection(Line1_Start, Line1_End, Line2_Start, Line2_End);

            float fControlPointXCoord;
            float fControlPointYCoord1;
            float fControlPointYCoord2;

            if (fRafterVectorDirection > 0)
            {
                fControlPointXCoord = m_Node.X - 0.5f * m_fb_1;
                fControlPointYCoord1 = (float)(m_Node.Y - 0.5f * m_MainMember.CrScStart.b - 0.5f * m_ft);
                fControlPointYCoord2 = (float)(m_Node.Y + 0.5f * m_MainMember.CrScStart.b - 0.5f * m_ft);
            }
            else
            {
                fControlPointXCoord = m_Node.X + 0.5f * m_fb_1;
                fControlPointYCoord1 = (float)(m_Node.Y - 0.5f * m_MainMember.CrScStart.b - 0.5f * m_ft - m_ft);
                fControlPointYCoord2 = (float)(m_Node.Y + 0.5f * m_MainMember.CrScStart.b - 0.5f * m_ft - m_ft);
            }

            CPoint ControlPoint_P1 = new CPoint(0, fControlPointXCoord, fControlPointYCoord1, pUpperLeftPointOfPlate.Y - m_fh_1, 0);
            CPoint ControlPoint_P2 = new CPoint(1, fControlPointXCoord, fControlPointYCoord2, pUpperLeftPointOfPlate.Y - m_fh_1, 0);

            m_arrPlates = new CPlate[2];
            m_arrPlates[0] = new CConCom_Plate_KA(ControlPoint_P1, m_fb_1, m_fh_1, m_fb_2, m_fh_2, m_ft, 90, 0, fRotatePlatesInJointAngle, BIsDisplayed); // Rotation angle in degrees
            m_arrPlates[1] = new CConCom_Plate_KA(ControlPoint_P2, m_fb_1, m_fh_1, m_fb_2, m_fh_2, m_ft, 90, 0, fRotatePlatesInJointAngle, BIsDisplayed);  // Rotation angle in degrees
        }

        public static Point ? Intersection(Point start1, Point end1, Point start2, Point end2)
        {
            double a1 = end1.Y - start1.Y;
            double b1 = start1.X - end1.X;
            double c1 = a1 * start1.X + b1 * start1.Y;

            double a2 = end2.Y - start2.Y;
            double b2 = start2.X - end2.X;
            double c2 = a2 * start2.X + b2 * start2.Y;

            double det = a1 * b2 - a2 * b1;
            if (det == 0)
            { // lines are parallel
                return null;
            }

            double x = (b2 * c1 - b1 * c2) / det;
            double y = (a1 * c2 - a2 * c1) / det;

            return new Point(x, y);
        }
    }
}
