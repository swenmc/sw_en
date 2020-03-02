using System;
using System.Windows;
using System.Collections.Generic;
using DATABASE;
using DATABASE.DTO;
using BaseClasses.GraphObj;
using System.Globalization;
using System.Windows.Media.Media3D;
using BaseClasses.Helpers;

namespace BaseClasses
{
    public class CConnectionJoint_B001 : CConnectionJointTypes
    {
        // Rafter to Main Column Joint - Knee Joint

        public float m_fb_1;
        public float m_fb_2;
        public float m_fh_1;
        public float m_fh_2;
        public float m_ft;
        public float m_fSlope_rad;
        public float m_ft_rafter;
        public float m_fJointAngleAboutZ_deg;

        public float m_fRafterVectorDirection;
        Point m_pUpperLeftPointOfPlate;

        public CConnectionJoint_B001() { }

        public CConnectionJoint_B001(CNode Node_temp, CMember MainFrameColumn_temp, CMember MainFrameRafter_temp, float fSLope_rad_temp, float fb_2_temp, float fh_1_temp, float ft, float ft_rafter, float fJointAngleAboutZ_deg)
        {
            bIsJointDefinedinGCS = true;

            m_Node = Node_temp;
            m_pControlPoint = m_Node.GetPoint3D();
            m_MainMember = MainFrameColumn_temp;
            m_SecondaryMembers = new CMember[1];
            m_SecondaryMembers[0] = MainFrameRafter_temp;

            m_fb_2 = fb_2_temp;
            m_fh_1 = fh_1_temp;

            m_fSlope_rad = fSLope_rad_temp;

            m_fb_1 = (float)m_MainMember.CrScStart.h;
            m_fh_2 = m_fh_1 + (float)Math.Tan(m_fSlope_rad) * m_fb_2;
            m_ft = ft;
            m_ft_rafter = ft_rafter;
            m_fJointAngleAboutZ_deg = fJointAngleAboutZ_deg;

            float ftemp_a = 0.5f * (float)m_SecondaryMembers[0].CrScStart.h / (float)Math.Cos(m_fSlope_rad);
            float ftemp_b = 0.5f * (float)m_SecondaryMembers[0].CrScStart.h / (float)Math.Cos(m_fSlope_rad);

            Name = "Column to Rafter Knee Joint";

            // Find left top edge intersection between column and rafter
            // Todo prepracovat a pokial mozno zjednodusit, potrebujeme len ziskat polohu plechu v globalnom smere Z

            Point Line1_Start = new Point();
            Point Line1_End = new Point();
            Point Line2_Start = new Point();
            Point Line2_End = new Point();

            m_fRafterVectorDirection = m_SecondaryMembers[0].NodeEnd.X - m_Node.X; // If positive rotate joint plates 0 deg, if negative rotate 180 deg
            float fRotatePlatesInJointAngle = m_fRafterVectorDirection > 0 ? (0  + fJointAngleAboutZ_deg): (180 + fJointAngleAboutZ_deg);
            float fDistanceX = m_fRafterVectorDirection > 0 ? -0.5f * (float)m_MainMember.CrScStart.h : 0.5f * (float)m_MainMember.CrScStart.h;

            Line1_Start.X = m_MainMember.NodeStart.X + fDistanceX; // Column
            Line1_Start.Y = m_MainMember.NodeStart.Z;
            Line1_End.X = m_MainMember.NodeEnd.X + fDistanceX;
            Line1_End.Y = m_MainMember.NodeEnd.Z;

            Line2_Start.X = m_SecondaryMembers[0].NodeStart.X; // Rafter
            Line2_Start.Y = m_SecondaryMembers[0].NodeStart.Z + (0.5f * m_SecondaryMembers[0].CrScStart.h) / Math.Cos(fSLope_rad_temp);
            Line2_End.X = m_SecondaryMembers[0].NodeEnd.X;
            Line2_End.Y = m_SecondaryMembers[0].NodeEnd.Z + (0.5f * m_SecondaryMembers[0].CrScStart.h) / Math.Cos(fSLope_rad_temp);

            m_pUpperLeftPointOfPlate = (Point)Intersection(Line1_Start, Line1_End, Line2_Start, Line2_End);

            float fControlPointXCoord;
            float fControlPointYCoord1;
            float fControlPointYCoord2;

            float fGap = 0.002f; // 2 mm
            if (m_fRafterVectorDirection > 0)
            {
                fControlPointXCoord = m_Node.X - (float)m_MainMember.CrScStart.z_max;
                fControlPointYCoord1 = (float)(m_Node.Y + m_MainMember.CrScStart.y_min /*- 0.5f * m_MainMember.CrScStart.b*/- fGap);
                fControlPointYCoord2 = (float)(m_Node.Y + m_MainMember.CrScStart.y_max /* + 0.5f * m_MainMember.CrScStart.b*/ + m_ft + fGap);
            }
            else
            {
                fControlPointXCoord = m_Node.X + (float)m_MainMember.CrScStart.z_max;
                fControlPointYCoord1 = (float)(m_Node.Y + m_MainMember.CrScStart.y_min /*- 0.5f * m_MainMember.CrScStart.b*/ - fGap - m_ft);
                fControlPointYCoord2 = (float)(m_Node.Y + m_MainMember.CrScStart.y_max /*0.5f * m_MainMember.CrScStart.b*/ + fGap + m_ft - m_ft);
            }

            Point3D ControlPoint_P1 = new Point3D(fControlPointXCoord, fControlPointYCoord1, m_pUpperLeftPointOfPlate.Y - m_fh_1);
            Point3D ControlPoint_P2 = new Point3D(fControlPointXCoord, fControlPointYCoord2, m_pUpperLeftPointOfPlate.Y - m_fh_1);

            // Screw arrangement parameters
            CRSC.CCrSc_TW rafterCrsc = null;

            if (m_SecondaryMembers[0].CrScStart is CRSC.CCrSc_TW)
            {
                rafterCrsc = (CRSC.CCrSc_TW)m_SecondaryMembers[0].CrScStart;
            }
            else
                throw new ArgumentNullException("Invalid cross-section type.");
            
            float fCrscDepth = (float)rafterCrsc.h;
            float fWebEndArcExternalRadius = (float)rafterCrsc.r_ee; // External edge radius
            float fCrscWebStraightDepth = fCrscDepth - 2 * fWebEndArcExternalRadius;
            float fStiffenerSize = (float)rafterCrsc.d_mu; // Nerovna cast v strede steny (zjednodusenia - pre nested  crsc sa uvazuje symetria, pre 270 sa do tohto uvazuje aj stredna rovna cast, hoci v nej mozu byt skrutky)
            CScrew referenceScrew = new CScrew("TEK", "14");
            CScrewArrangementCircleApexOrKnee screwArrangement1 = CJointHelper.GetDefaultCircleScrewArrangement(fCrscDepth, fWebEndArcExternalRadius, fCrscWebStraightDepth, fStiffenerSize, referenceScrew);
            CScrewArrangementCircleApexOrKnee screwArrangement2 = CJointHelper.GetDefaultCircleScrewArrangement(fCrscDepth, fWebEndArcExternalRadius, fCrscWebStraightDepth, fStiffenerSize, referenceScrew);

            bool bScrewInPlusZDirection1 = m_Node == m_MainMember.NodeStart ? true : false;
            bool bScrewInPlusZDirection2 = m_Node == m_MainMember.NodeStart ? false : true;

            m_arrPlates = new CPlate[2];
            m_arrPlates[0] = new CConCom_Plate_KA("KA", ControlPoint_P1, m_fb_1, m_fh_1, m_fb_2, m_fh_2, m_ft, 90, 0, fRotatePlatesInJointAngle, bScrewInPlusZDirection1, screwArrangement1); // Rotation angle in degrees
            m_arrPlates[1] = new CConCom_Plate_KA("KA", ControlPoint_P2, m_fb_1, m_fh_1, m_fb_2, m_fh_2, m_ft, 90, 0, fRotatePlatesInJointAngle, bScrewInPlusZDirection2, screwArrangement2);  // Rotation angle in degrees
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

        public override CConnectionJointTypes RecreateJoint()
        {
            return new CConnectionJoint_B001(m_Node, m_MainMember, m_SecondaryMembers[0], m_fSlope_rad, m_fb_2, m_fh_1, m_ft, m_ft_rafter, m_fJointAngleAboutZ_deg);
        }

        public override void UpdateJoint()
        {
            float fPlate1_b_X1 = 0;
            float fPlate1_h_Y1 = 0;
            float fPlate1_h_Y2 = 0;

            float fPlate2_b_X1 = 0;
            float fPlate2_h_Y1 = 0;
            float fPlate2_h_Y2 = 0;

            GetKneePlateGeneralParameters(m_arrPlates[0], out fPlate1_b_X1, out fPlate1_h_Y1, out fPlate1_h_Y2);
            GetKneePlateGeneralParameters(m_arrPlates[1], out fPlate2_b_X1, out fPlate2_h_Y1, out fPlate2_h_Y2);

            float fControlPointXCoord1;
            float fControlPointXCoord2;
            float fControlPointYCoord1;
            float fControlPointYCoord2;

            float fGap = 0.002f; // 2 mm
            if (m_fRafterVectorDirection > 0)
            {
                fControlPointXCoord1 = m_Node.X - (float)m_MainMember.CrScStart.z_max;
                fControlPointXCoord2 = m_Node.X - (float)m_MainMember.CrScStart.z_max;
                fControlPointYCoord1 = (float)(m_Node.Y + m_MainMember.CrScStart.y_min /*- 0.5f * m_MainMember.CrScStart.b*/- fGap);
                fControlPointYCoord2 = (float)(m_Node.Y + m_MainMember.CrScStart.y_max /* + 0.5f * m_MainMember.CrScStart.b*/ + m_arrPlates[1].Ft + fGap);
            }
            else
            {
                fControlPointXCoord1 = m_Node.X + (float)m_MainMember.CrScStart.z_max;
                fControlPointXCoord2 = m_Node.X + (float)m_MainMember.CrScStart.z_max;
                fControlPointYCoord1 = (float)(m_Node.Y + m_MainMember.CrScStart.y_min /*- 0.5f * m_MainMember.CrScStart.b*/ - fGap - m_arrPlates[0].Ft);
                fControlPointYCoord2 = (float)(m_Node.Y + m_MainMember.CrScStart.y_max /*0.5f * m_MainMember.CrScStart.b*/ + fGap + m_arrPlates[1].Ft - m_arrPlates[1].Ft);
            }

            m_arrPlates[0].m_pControlPoint = new Point3D(fControlPointXCoord1, fControlPointYCoord1, m_pUpperLeftPointOfPlate.Y - fPlate1_h_Y1);
            m_arrPlates[1].m_pControlPoint = new Point3D(fControlPointXCoord2, fControlPointYCoord2, m_pUpperLeftPointOfPlate.Y - fPlate2_h_Y1);
        }

        private void GetKneePlateGeneralParameters(CPlate plate, out float fb_X1, out float fh_Y1, out float fh_Y2)
        {
            // Todo - dalo by sa pridat aj dalsie spolocne parametre plates K
            if (plate is CConCom_Plate_KA)
            {
                CConCom_Plate_KA plate_KA = (CConCom_Plate_KA)plate;
                fb_X1 = plate_KA.Fb_X1;
                fh_Y1 = plate_KA.Fh_Y1;
                fh_Y2 = plate_KA.Fh_Y2;
            }
            else if (plate is CConCom_Plate_KB)
            {
                CConCom_Plate_KB plate_KB = (CConCom_Plate_KB)plate;
                fb_X1 = plate_KB.Fb_X1;
                fh_Y1 = plate_KB.Fh_Y1;
                fh_Y2 = plate_KB.Fh_Y2;
            }
            else if (plate is CConCom_Plate_KBS)
            {
                CConCom_Plate_KBS plate_KBS = (CConCom_Plate_KBS)plate;
                fb_X1 = plate_KBS.Fb_X1;
                fh_Y1 = plate_KBS.Fh_Y1;
                fh_Y2 = plate_KBS.Fh_Y2;
            }
            else if (plate is CConCom_Plate_KC)
            {
                CConCom_Plate_KC plate_KC = (CConCom_Plate_KC)plate;
                fb_X1 = plate_KC.Fb_X1;
                fh_Y1 = plate_KC.Fh_Y1;
                fh_Y2 = plate_KC.Fh_Y2;
            }
            else if (plate is CConCom_Plate_KCS)
            {
                CConCom_Plate_KCS plate_KCS = (CConCom_Plate_KCS)plate;
                fb_X1 = plate_KCS.Fb_X1;
                fh_Y1 = plate_KCS.Fh_Y1;
                fh_Y2 = plate_KCS.Fh_Y2;
            }
            else if (plate is CConCom_Plate_KD)
            {
                CConCom_Plate_KD plate_KD = (CConCom_Plate_KD)plate;
                fb_X1 = plate_KD.Fb_X1;
                fh_Y1 = plate_KD.Fh_Y1;
                fh_Y2 = plate_KD.Fh_Y2;
            }
            else if (plate is CConCom_Plate_KDS)
            {
                CConCom_Plate_KDS plate_KDS = (CConCom_Plate_KDS)plate;
                fb_X1 = plate_KDS.Fb_X1;
                fh_Y1 = plate_KDS.Fh_Y1;
                fh_Y2 = plate_KDS.Fh_Y2;
            }
            else if (plate is CConCom_Plate_KES)
            {
                CConCom_Plate_KES plate_KES = (CConCom_Plate_KES)plate;
                fb_X1 = plate_KES.Fb_X1;
                fh_Y1 = plate_KES.Fh_Y1;
                fh_Y2 = plate_KES.Fh_Y2;
            }
            else if (plate is CConCom_Plate_KFS)
            {
                CConCom_Plate_KFS plate_KFS = (CConCom_Plate_KFS)plate;
                fb_X1 = plate_KFS.Fb_X1;
                fh_Y1 = plate_KFS.Fh_Y1;
                fh_Y2 = plate_KFS.Fh_Y2;
            }
            else
            {
                throw new Exception("Invalid type of apex plate");
            }
        }



        //odlozeny zakomentovany kod z refaktoringu
        ////-----------------------------------------------------------------------------------------------
        //// TODO Ondrej - refaktorovat s CPlateHelper.GetDefaultCircleScrewArrangement
        //float fMinimumStraightEdgeDistance = 0.010f; // Minimalna vzdialenost skrutky od hrany ohybu pozdlzneho rebra / vyztuhy na priereze (hrana zakrivenej casti)

        //float fCrscDepth = (float)rafterCrsc.h;
        //float fWebEndArcExternalRadius = (float)rafterCrsc.r_ee; // External edge radius
        //float fCrscWebStraightDepth = fCrscDepth - 2 * fWebEndArcExternalRadius;
        //float fStiffenerSize = (float)rafterCrsc.d_mu; // Nerovna cast v strede steny (zjednodusenia - pre nested  crsc sa uvazuje symetria, pre 270 sa do tohto uvazuje aj stredna rovna cast, hoci v nej mozu byt skrutky)

        //bool bUseAdditionalCornerScrews = true;
        //int iAdditionalConnectorInCornerNumber = 4; // 4 screws in each corner
        //float fMinimumDistanceBetweenScrews = 0.02f;
        //float fAdditionalConnectorDistance = Math.Max(fMinimumDistanceBetweenScrews, 0.05f * fCrscWebStraightDepth);
        //float fConnectorRadiusInCircleSequence = 0.5f * (fCrscWebStraightDepth - 2 * fMinimumStraightEdgeDistance);
        //float fDistanceBetweenScrewsInCircle = 0.050f;

        //if (fCrscDepth < 0.5f) // Zmenseny default - TODO mohol by byt urceny podla tvaru prierezu
        //{
        //    fDistanceBetweenScrewsInCircle = 0.030f;
        //}

        //// http://www.ambrsoft.com/TrigoCalc/Sphere/Arc_.htm
        //float fAngle = 2f * (float)Math.Acos((0.5f * (fStiffenerSize + 2f * fMinimumDistanceBetweenScrews)) / fConnectorRadiusInCircleSequence);
        //int iConnectorNumberInCircleSequence = (int)((fAngle * fConnectorRadiusInCircleSequence) / fDistanceBetweenScrewsInCircle) + 1; // Pocet medzier + 1
        //CScrew referenceScrew = new CScrew("TEK", "14");

        //List<CScrewSequenceGroup> screwSeqGroups = new List<CScrewSequenceGroup>();
        //CScrewSequenceGroup gr1 = new CScrewSequenceGroup();
        //gr1.NumberOfHalfCircleSequences = 2;
        //gr1.NumberOfRectangularSequences = 4;
        //gr1.ListSequence.Add(new CScrewHalfCircleSequence(fConnectorRadiusInCircleSequence, iConnectorNumberInCircleSequence));
        //gr1.ListSequence.Add(new CScrewHalfCircleSequence(fConnectorRadiusInCircleSequence, iConnectorNumberInCircleSequence));
        //screwSeqGroups.Add(gr1);
        //CScrewSequenceGroup gr2 = new CScrewSequenceGroup();
        //gr2.NumberOfHalfCircleSequences = 2;
        //gr2.NumberOfRectangularSequences = 4;
        //gr2.ListSequence.Add(new CScrewHalfCircleSequence(fConnectorRadiusInCircleSequence, iConnectorNumberInCircleSequence));
        //gr2.ListSequence.Add(new CScrewHalfCircleSequence(fConnectorRadiusInCircleSequence, iConnectorNumberInCircleSequence));
        //screwSeqGroups.Add(gr2);
        ////-----------------------------------------------------------------------------------------------

        //CScrewArrangementCircleApexOrKnee screwArrangement1 = new CScrewArrangementCircleApexOrKnee(referenceScrew, fCrscDepth, fCrscWebStraightDepth, fStiffenerSize, 1, screwSeqGroups, bUseAdditionalCornerScrews, fConnectorRadiusInCircleSequence, fConnectorRadiusInCircleSequence, iAdditionalConnectorInCornerNumber, fAdditionalConnectorDistance, fAdditionalConnectorDistance);
        //CScrewArrangementCircleApexOrKnee screwArrangement2 = new CScrewArrangementCircleApexOrKnee(referenceScrew, fCrscDepth, fCrscWebStraightDepth, fStiffenerSize, 1, screwSeqGroups, bUseAdditionalCornerScrews, fConnectorRadiusInCircleSequence, fConnectorRadiusInCircleSequence, iAdditionalConnectorInCornerNumber, fAdditionalConnectorDistance, fAdditionalConnectorDistance);
    }
}
