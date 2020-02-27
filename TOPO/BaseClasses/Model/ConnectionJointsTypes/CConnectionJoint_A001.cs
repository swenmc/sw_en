using System;
using System.Collections.Generic;
using DATABASE;
using DATABASE.DTO;
using BaseClasses.GraphObj;
using System.Globalization;
using System.Windows.Media.Media3D;
using BaseClasses.Helpers;

namespace BaseClasses
{
    public class CConnectionJoint_A001 : CConnectionJointTypes
    {
        // Rafter to Rafter Joint - Roof Tip
        public float m_fb;
        public float m_fh_1;
        public float m_fh_2;
        public float m_ft;
        public float m_fSlope_rad;
        public float m_fJointAngleAboutZ_deg;

        public CConnectionJoint_A001() { }

        public CConnectionJoint_A001(CNode Node_temp, CMember MainRafter_temp, CMember SecondaryRafter_temp, float fSLope_rad_temp, float fb_temp, float ft, float fJointAngleAboutZ_deg)
        {
            bIsJointDefinedinGCS = true;

            m_Node = Node_temp;
            m_pControlPoint = m_Node.GetPoint3D();
            m_MainMember = MainRafter_temp;
            m_SecondaryMembers = new CMember[1];
            m_SecondaryMembers[0] = SecondaryRafter_temp;

            m_fb = fb_temp;
            m_fSlope_rad = fSLope_rad_temp;
            m_fJointAngleAboutZ_deg = fJointAngleAboutZ_deg;
            m_fh_1 = (float)m_MainMember.CrScStart.h / (float)Math.Cos(m_fSlope_rad);
            m_fh_2 = m_fh_1 + (float)Math.Tan(m_fSlope_rad) * 0.5f * m_fb;
            m_ft = ft;

            Name = "Rafter Apex Joint";

            Point3D ControlPoint_P1 = new Point3D(m_Node.X + 0.5 * m_fb, m_Node.Y + m_MainMember.CrScStart.y_min /*- 0.5f * m_MainMember.CrScStart.b*/ - /*0.5f **/ m_ft, m_Node.Z - (m_fh_2 - 0.5 * m_fh_1));
            Point3D ControlPoint_P2 = new Point3D(m_Node.X - 0.5 * m_fb, m_Node.Y + m_MainMember.CrScStart.y_max /*+ 0.5f * m_MainMember.CrScStart.b*/ + /*1.5f **/ m_ft, m_Node.Z - (m_fh_2 - 0.5 * m_fh_1));

            // Screw arrangement parameters

            CRSC.CCrSc_TW rafterCrsc = null;

            if (m_SecondaryMembers[0].CrScStart is CRSC.CCrSc_TW)
            {
                rafterCrsc = (CRSC.CCrSc_TW)m_SecondaryMembers[0].CrScStart;
            }
            else
                throw new ArgumentNullException("Invalid cross-section type.");

            float fMinimumStraightEdgeDistance = 0.005f; // Minimalna vzdialenost skrutky od hrany ohybu pozdlzneho rebra / vyztuhy na priereze (hrana zakrivenej casti)

            float fCrscWebStraightDepth = (float)rafterCrsc.d_tot; // BOX 63020 web straight depth
            float fStiffenerSize = (float)rafterCrsc.d_mu; // Nerovna cast v strede steny (zjednodusenia - pre nested  crsc sa uvazuje symetria, pre 270 sa do tohto uvazuje aj stredna rovna cast, hoci v nej mozu byt skrutky)
            bool bUseAdditionalCornerScrews = true;
            int iAdditionalConnectorInCornerNumber = 4; // 4 screws in each corner
            float fMinimumDistanceBetweenScrews = 0.02f;
            float fAdditionalConnectorDistance = Math.Max(fMinimumDistanceBetweenScrews, 0.05f * fCrscWebStraightDepth);
            float fConnectorRadiusInCircleSequence = 0.5f * (fCrscWebStraightDepth - 2 * fMinimumStraightEdgeDistance);
            float fDistanceBetweenScrewsInCircle = 0.04f;
            float fAngle = 2f * (float)Math.Acos((0.5f * (fStiffenerSize + 2f * fMinimumDistanceBetweenScrews)) / fConnectorRadiusInCircleSequence);
            int iConnectorNumberInCircleSequence = (int) ((fAngle * fConnectorRadiusInCircleSequence) / fDistanceBetweenScrewsInCircle); // 20; // TODO - dynamicky podla velkosti plate
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
            m_arrPlates[0] = new CConCom_Plate_JB("JB", ControlPoint_P1, m_fb, m_fh_1, m_fh_2, 0.050f, m_ft, 90, 0, 180 + fJointAngleAboutZ_deg, true, screwArrangement); // Rotation angle in degrees
            m_arrPlates[1] = new CConCom_Plate_JB("JB", ControlPoint_P2, m_fb, m_fh_1, m_fh_2, 0.050f, m_ft, 90, 0, 0 + fJointAngleAboutZ_deg, true, screwArrangement); // Rotation angle in degrees
        }

        public override CConnectionJointTypes RecreateJoint()
        {
            return new CConnectionJoint_A001(m_Node, m_MainMember, m_SecondaryMembers[0], m_fSlope_rad, m_fb, m_ft, m_fJointAngleAboutZ_deg);
        }
    }
}
