using System;
using System.Windows;
using System.Collections.Generic;
using DATABASE;
using DATABASE.DTO;
using BaseClasses.GraphObj;
using System.Globalization;

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

        public CConnectionJoint_B001(CNode Node_temp, CMember MainFrameColumn_temp, CMember MainFrameRafter_temp, float fSLope_rad_temp, float fb_2_temp, float fh_1_temp, float ft, float ft_rafter, float fJointAngleAboutZ_deg, bool bIsDisplayed_temp)
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

            BIsGenerated = true;
            BIsDisplayed = bIsDisplayed_temp;

            Name = "Column to Rafter Knee Joint";

            // Find left top edge intersection between column and rafter
            // Todo prepracovat a pokial mozno zjednodusit, potrebujeme len ziskat polohu plechu v globalnom smere Z

            Point Line1_Start = new Point();
            Point Line1_End = new Point();
            Point Line2_Start = new Point();
            Point Line2_End = new Point();

            float fRafterVectorDirection = m_SecondaryMembers[0].NodeEnd.X - m_Node.X; // If positive rotate joint plates 0 deg, if negative rotate 180 deg
            float fRotatePlatesInJointAngle = fRafterVectorDirection > 0 ? (0  + fJointAngleAboutZ_deg): (180 + fJointAngleAboutZ_deg);
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
                fControlPointYCoord1 = (float)(m_Node.Y + m_MainMember.CrScStart.y_min /*- 0.5f * m_MainMember.CrScStart.b*/- 0.5f * m_ft);
                fControlPointYCoord2 = (float)(m_Node.Y + m_MainMember.CrScStart.y_max /* + 0.5f * m_MainMember.CrScStart.b*/ + 1.5f * m_ft);
            }
            else
            {
                fControlPointXCoord = m_Node.X + 0.5f * m_fb_1;
                fControlPointYCoord1 = (float)(m_Node.Y + m_MainMember.CrScStart.y_min /*- 0.5f * m_MainMember.CrScStart.b*/ - 0.5f * m_ft - m_ft);
                fControlPointYCoord2 = (float)(m_Node.Y + m_MainMember.CrScStart.y_max /*0.5f * m_MainMember.CrScStart.b*/ + 1.5f * m_ft - m_ft);
            }

            CPoint ControlPoint_P1 = new CPoint(0, fControlPointXCoord, fControlPointYCoord1, pUpperLeftPointOfPlate.Y - m_fh_1, 0);
            CPoint ControlPoint_P2 = new CPoint(1, fControlPointXCoord, fControlPointYCoord2, pUpperLeftPointOfPlate.Y - m_fh_1, 0);

            // Screw arrangement parameters
            // TODO nacitavat parametre z prierezu
            float fCrscWebStraightDepth = (float)m_SecondaryMembers[0].CrScStart.h - 2 * 0.025f - 2 * (float)m_SecondaryMembers[0].CrScStart.t_min; // BOX 63020 web straight depth
            float fStiffenerSize = 0.2857142f * (float)m_SecondaryMembers[0].CrScStart.h; // TODO - - dynamicky podla typu a velkosti prierezu -  0.18f; // BOX 63020, distance without applied screws in the middle of cross-section
            bool bUseAdditionalCornerScrews = true;
            int iAdditionalConnectorInCornerNumber = 4; // 4 screws in each corner
            float fAdditionalConnectorDistance = Math.Max(0.02f, 0.05f * fCrscWebStraightDepth);
            float fConnectorRadiusInCircleSequence = 0.45f * fCrscWebStraightDepth; // TODO - dynamicky podla velkosti prierezu
            int iConnectorNumberInCircleSequence = (int)((2f * MATH.MathF.fPI * fConnectorRadiusInCircleSequence) / 0.05f); // 20; // TODO - dynamicky podla velkosti plate
            CScrew referenceScrew = new CScrew("TEK", "14");

            List<CScrewSequenceGroup> screwSeqGroups = new List<CScrewSequenceGroup>();
            CScrewSequenceGroup gr1 = new CScrewSequenceGroup();
            gr1.NumberOfHalfCircleSequences = 2;
            gr1.NumberOfRectangularSequences = 4;
            gr1.ListSequence.Add(new CScrewHalfCircleSequence(fConnectorRadiusInCircleSequence, iConnectorNumberInCircleSequence));
            gr1.ListSequence.Add(new CScrewHalfCircleSequence(fConnectorRadiusInCircleSequence, iConnectorNumberInCircleSequence));
            screwSeqGroups.Add(gr1);
            CScrewSequenceGroup gr2 = new CScrewSequenceGroup();
            gr2.NumberOfHalfCircleSequences = 2;
            gr2.NumberOfRectangularSequences = 4;
            gr2.ListSequence.Add(new CScrewHalfCircleSequence(fConnectorRadiusInCircleSequence, iConnectorNumberInCircleSequence));
            gr2.ListSequence.Add(new CScrewHalfCircleSequence(fConnectorRadiusInCircleSequence, iConnectorNumberInCircleSequence));
            screwSeqGroups.Add(gr2);

            CScrewArrangementCircleApexOrKnee screwArrangement = new CScrewArrangementCircleApexOrKnee(referenceScrew, (float)m_MainMember.CrScStart.h, fCrscWebStraightDepth, fStiffenerSize, 1, screwSeqGroups, bUseAdditionalCornerScrews, fConnectorRadiusInCircleSequence, fConnectorRadiusInCircleSequence, iAdditionalConnectorInCornerNumber, fAdditionalConnectorDistance, fAdditionalConnectorDistance);

            m_arrPlates = new CPlate[2];
            m_arrPlates[0] = new CConCom_Plate_KA("KA", ControlPoint_P1, m_fb_1, m_fh_1, m_fb_2, m_fh_2, m_ft, 90, 0, fRotatePlatesInJointAngle, screwArrangement, BIsDisplayed); // Rotation angle in degrees
            m_arrPlates[1] = new CConCom_Plate_KA("KA", ControlPoint_P2, m_fb_1, m_fh_1, m_fb_2, m_fh_2, m_ft, 90, 0, fRotatePlatesInJointAngle, screwArrangement, BIsDisplayed);  // Rotation angle in degrees
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
