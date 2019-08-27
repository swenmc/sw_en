using System;
using System.Collections.Generic;
using DATABASE;
using DATABASE.DTO;
using BaseClasses.GraphObj;
using System.Globalization;
using System.Windows.Media.Media3D;

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
        float m_fJointAngleAboutZ_deg;

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
            m_fJointAngleAboutZ_deg = fJointAngleAboutZ_deg;
            m_fh_1 = (float)m_MainMember.CrScStart.h / (float)Math.Cos(m_fSlope_rad);
            m_fh_2 = m_fh_1 + (float)Math.Tan(m_fSlope_rad) * 0.5f * m_fb;
            m_ft = ft;

            BIsGenerated = true;
            BIsDisplayed = bIsDisplayed_temp;

            Name = "Rafter Apex Joint";

            Point3D ControlPoint_P1 = new Point3D(m_Node.X + 0.5 * m_fb, m_Node.Y + m_MainMember.CrScStart.y_min /*- 0.5f * m_MainMember.CrScStart.b*/ - /*0.5f **/ m_ft, m_Node.Z - (m_fh_2 - 0.5 * m_fh_1));
            Point3D ControlPoint_P2 = new Point3D(m_Node.X - 0.5 * m_fb, m_Node.Y + m_MainMember.CrScStart.y_max /*+ 0.5f * m_MainMember.CrScStart.b*/ + /*1.5f **/ m_ft, m_Node.Z - (m_fh_2 - 0.5 * m_fh_1));

            // Screw arrangement parameters
            // TODO nacitavat parametre z prierezu
            float fCrscWebStraightDepth = (float)MainRafter_temp.CrScStart.h - 2 * 0.025f - 2 * (float)MainRafter_temp.CrScStart.t_min; // BOX 63020 web straight depth
            float fStiffenerSize = 0.2857142f * (float)MainRafter_temp.CrScStart.h; // TODO - - dynamicky podla typu a velkosti prierezu -  0.18f; // BOX 63020, distance without applied screws in the middle of cross-section
            bool bUseAdditionalCornerScrews = true;
            int iAdditionalConnectorInCornerNumber = 4; // 4 screws in each corner
            float fAdditionalConnectorDistance = Math.Max(0.02f, 0.05f * fCrscWebStraightDepth);
            float fConnectorRadiusInCircleSequence = 0.45f * fCrscWebStraightDepth; // TODO - dynamicky podla velkosti prierezu
            int iConnectorNumberInCircleSequence = (int) ((2f * MATH.MathF.fPI * fConnectorRadiusInCircleSequence) / 0.05f); // 20; // TODO - dynamicky podla velkosti plate
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
            m_arrPlates[0] = new CConCom_Plate_JB("JB", ControlPoint_P1, m_fb, m_fh_1, m_fh_2, 0.050f, m_ft, 90, 0, 180 + fJointAngleAboutZ_deg, screwArrangement, BIsDisplayed); // Rotation angle in degrees
            m_arrPlates[1] = new CConCom_Plate_JB("JB", ControlPoint_P2, m_fb, m_fh_1, m_fh_2, 0.050f, m_ft, 90, 0, 0 + fJointAngleAboutZ_deg, screwArrangement, BIsDisplayed); // Rotation angle in degrees
        }

        public override CConnectionJointTypes RecreateJoint()
        {
            return new CConnectionJoint_A001(m_Node, m_MainMember, m_SecondaryMembers[0], m_fSlope_rad, m_fb, m_ft, m_fJointAngleAboutZ_deg, BIsDisplayed);
        }
    }
}
