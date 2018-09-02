using System;
using System.Collections.Generic;
using DATABASE;
using DATABASE.DTO;
using BaseClasses.GraphObj;
using System.Globalization;

namespace BaseClasses
{
    public class CConnectionJoint_A001 : CConnectionJointTypes
    {
        // Rafter to Rafter Joint - Roof Tip
        float m_fb;
        float m_fh_1;
        float m_fh_2;
        float m_ft;
        float m_fSlope_rad;

        public CConnectionJoint_A001() { }

        public CConnectionJoint_A001(CNode Node_temp, CMember MainRafter_temp, CMember SecondaryRafter_temp, float fSLope_rad_temp, float fb_temp, float ft, float fJointAngleAboutZ_deg, bool bIsDisplayed_temp)
        {
            bIsJointDefinedinGCS = true;

            m_Node = Node_temp;
            m_MainMember = MainRafter_temp;
            m_SecondaryMembers = new CMember[1];
            m_SecondaryMembers[0] = SecondaryRafter_temp;

            m_fb = fb_temp;
            m_fSlope_rad = fSLope_rad_temp;

            // TODO nacitavat parametre z prierezu
            float fCrscWebStraightDepth = 0.63f - 2 * 0.025f - 2 * 0.002f; // BOX 63020 web straight depth
            float fStiffenerSize = 0.18f; // BOX 63020, distance without applied screws in the middle of cross-section

            bool bUseAdditionalCornerScrews = true;
            int iAdditionalConnectorNumber = 2 * 4 * 4; //2 kruhy, 4 rohy, 4 skrutky v kazom rohu
            m_fh_1 = (float)m_MainMember.CrScStart.h / (float)Math.Cos(m_fSlope_rad);
            m_fh_2 = m_fh_1 + (float)Math.Tan(m_fSlope_rad) * 0.5f * m_fb;
            m_ft = ft;

            BIsDisplayed = bIsDisplayed_temp;

            CPoint ControlPoint_P1 = new CPoint(0, m_Node.X + 0.5 * m_fb, m_Node.Y - 0.5f * m_MainMember.CrScStart.b - 0.5f * m_ft, m_Node.Z - (m_fh_2 - 0.5 * m_fh_1), 0);
            CPoint ControlPoint_P2 = new CPoint(1, m_Node.X - 0.5 * m_fb, m_Node.Y + 0.5f * m_MainMember.CrScStart.b + 1.5f * m_ft, m_Node.Z - (m_fh_2 - 0.5 * m_fh_1), 0);

            int iConnectorNumber = 80; // Plates LH LI, LK
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";
            List<CTEKScrewProperties> screws = CTEKScrewsManager.LoadTEKScrewsProperties();
            float fDiameter_temp = float.Parse(screws[4].shankDiameter, nfi) / 1000f; // TODO - zapracovat skurtky do objektu JOINT a PLATE // 0.0055f; // Default - same size as screw
            float fScrewLength = 0.009f;

            m_arrPlates = new CPlate[2];
            m_arrPlates[0] = new CConCom_Plate_JB("JB", ControlPoint_P1, m_fb, m_fh_1, m_fh_2, 0.050f, m_ft, 90, 0, 180 + fJointAngleAboutZ_deg, iConnectorNumber, fDiameter_temp, fScrewLength, fCrscWebStraightDepth, fStiffenerSize, bUseAdditionalCornerScrews, iAdditionalConnectorNumber, BIsDisplayed); // Rotation angle in degrees
            m_arrPlates[1] = new CConCom_Plate_JB("JB", ControlPoint_P2, m_fb, m_fh_1, m_fh_2, 0.050f, m_ft, 90, 0, 0 + fJointAngleAboutZ_deg, iConnectorNumber, fDiameter_temp, fScrewLength, fCrscWebStraightDepth, fStiffenerSize, bUseAdditionalCornerScrews, iAdditionalConnectorNumber, BIsDisplayed); // Rotation angle in degrees
        }
    }
}
