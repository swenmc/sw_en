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
        public bool IsFront = true;
        Point m_pUpperLeftPointOfPlate;

        public CConnectionJoint_B001() { }

        public CConnectionJoint_B001(EJointType jointType_temp, CNode Node_temp, CMember MainFrameColumn_temp, CMember MainFrameRafter_temp, CMember MainFrameRafterCanopy_temp, float fSLope_rad_temp, float fb_2_temp, float fh_1_temp, float ft, float ft_rafter, float fJointAngleAboutZ_deg)
        {
            bIsJointDefinedinGCS = true;

            JointType = jointType_temp;
            m_Node = Node_temp;
            m_pControlPoint = m_Node.GetPoint3D();
            m_MainMember = MainFrameColumn_temp;

            if (jointType_temp == EJointType.eKnee_EgdeRafter_Column || jointType_temp == EJointType.eKnee_MainRafter_Column)
            {
                m_SecondaryMembers = new CMember[1];
                m_SecondaryMembers[0] = MainFrameRafter_temp;
            }
            else
            {
                m_SecondaryMembers = new CMember[2];
                m_SecondaryMembers[0] = MainFrameRafter_temp;
                m_SecondaryMembers[1] = MainFrameRafterCanopy_temp;
            }

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

            Point? p = Intersection(Line1_Start, Line1_End, Line2_Start, Line2_End);
            if (p != null) m_pUpperLeftPointOfPlate = p.Value;

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

            Vector3D RotationVector_P1 = new Vector3D(90, 0, fRotatePlatesInJointAngle);
            Vector3D RotationVector_P2 = new Vector3D(90, 0, fRotatePlatesInJointAngle);

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
            m_arrPlates[0] = new CConCom_Plate_KA("KA", ControlPoint_P1, m_fb_1, m_fh_1, m_fb_2, m_fh_2, m_ft, (float)RotationVector_P1.X, (float)RotationVector_P1.Y, (float)RotationVector_P1.Z, bScrewInPlusZDirection1, screwArrangement1); // Rotation angle in degrees
            m_arrPlates[1] = new CConCom_Plate_KA("KA", ControlPoint_P2, m_fb_1, m_fh_1, m_fb_2, m_fh_2, m_ft, (float)RotationVector_P2.X, (float)RotationVector_P2.Y, (float)RotationVector_P2.Z, bScrewInPlusZDirection2, screwArrangement2);  // Rotation angle in degrees
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
            return new CConnectionJoint_B001(JointType, m_Node, m_MainMember, m_SecondaryMembers[0], m_SecondaryMembers.Length == 2 ? m_SecondaryMembers[1] : null, m_fSlope_rad, m_fb_2, m_fh_1, m_ft, m_ft_rafter, m_fJointAngleAboutZ_deg);
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

            // Find left top edge intersection between column and rafter
            // Todo prepracovat a pokial mozno zjednodusit, potrebujeme len ziskat polohu plechu v globalnom smere Z

            Point Line1_Start = new Point();
            Point Line1_End = new Point();
            Point Line2_Start = new Point();
            Point Line2_End = new Point();

            m_fRafterVectorDirection = m_SecondaryMembers[0].NodeEnd.X - m_Node.X; // If positive rotate joint plates 0 deg, if negative rotate 180 deg
            float fRotatePlatesInJointAngle = m_fRafterVectorDirection > 0 ? (0 + m_fJointAngleAboutZ_deg) : (180 + m_fJointAngleAboutZ_deg);
            float fDistanceX = m_fRafterVectorDirection > 0 ? -0.5f * (float)m_MainMember.CrScStart.h : 0.5f * (float)m_MainMember.CrScStart.h;

            Line1_Start.X = m_MainMember.NodeStart.X + fDistanceX; // Column
            Line1_Start.Y = m_MainMember.NodeStart.Z;
            Line1_End.X = m_MainMember.NodeEnd.X + fDistanceX;
            Line1_End.Y = m_MainMember.NodeEnd.Z;

            Line2_Start.X = m_SecondaryMembers[0].NodeStart.X; // Rafter
            Line2_Start.Y = m_SecondaryMembers[0].NodeStart.Z + (0.5f * m_SecondaryMembers[0].CrScStart.h) / Math.Cos(m_fSlope_rad);
            Line2_End.X = m_SecondaryMembers[0].NodeEnd.X;
            Line2_End.Y = m_SecondaryMembers[0].NodeEnd.Z + (0.5f * m_SecondaryMembers[0].CrScStart.h) / Math.Cos(m_fSlope_rad);

            Point? p = Intersection(Line1_Start, Line1_End, Line2_Start, Line2_End);
            if(p != null) m_pUpperLeftPointOfPlate = p.Value;

            float fControlPointXCoord1;
            float fControlPointXCoord2;
            float fControlPointYCoord1;
            float fControlPointYCoord2;

            float fGap = 0.002f; // 2 mm
            if (m_fRafterVectorDirection > 0) // Left Side
            {
                fControlPointXCoord1 = m_Node.X - (float)m_MainMember.CrScStart.z_max;
                fControlPointXCoord2 = m_Node.X - (float)m_MainMember.CrScStart.z_max;
                fControlPointYCoord1 = (float)(m_Node.Y + m_MainMember.CrScStart.y_min /*- 0.5f * m_MainMember.CrScStart.b*/- fGap);
                fControlPointYCoord2 = (float)(m_Node.Y + m_MainMember.CrScStart.y_max /* + 0.5f * m_MainMember.CrScStart.b*/ + m_arrPlates[1].Ft + fGap);

                // BUG 638 - upravit control point
                if (/*m_arrPlates[1] is CConCom_Plate_KA ||*/
                    m_arrPlates[1] is CConCom_Plate_KB ||
                    m_arrPlates[1] is CConCom_Plate_KBS ||
                    m_arrPlates[1] is CConCom_Plate_KC ||
                    m_arrPlates[1] is CConCom_Plate_KCS ||
                    m_arrPlates[1] is CConCom_Plate_KD ||
                    m_arrPlates[1] is CConCom_Plate_KDS ||
                    m_arrPlates[1] is CConCom_Plate_KES ||
                    m_arrPlates[1] is CConCom_Plate_KFS ||
                    m_arrPlates[1] is CConCom_Plate_KGS ||
                    m_arrPlates[1] is CConCom_Plate_KHS)
                {
                    fControlPointYCoord2 -= m_arrPlates[1].Ft;
                }
            }
            else // Right Side
            {
                fControlPointXCoord1 = m_Node.X + (float)m_MainMember.CrScStart.z_max;
                fControlPointXCoord2 = m_Node.X + (float)m_MainMember.CrScStart.z_max;
                fControlPointYCoord1 = (float)(m_Node.Y + m_MainMember.CrScStart.y_min /*- 0.5f * m_MainMember.CrScStart.b*/ - fGap - m_arrPlates[0].Ft);
                fControlPointYCoord2 = (float)(m_Node.Y + m_MainMember.CrScStart.y_max /*0.5f * m_MainMember.CrScStart.b*/ + fGap + m_arrPlates[1].Ft - m_arrPlates[1].Ft);

                // BUG 638 - upravit control point

                if (/*m_arrPlates[0] is CConCom_Plate_KA ||*/
                    m_arrPlates[0] is CConCom_Plate_KB ||
                    m_arrPlates[0] is CConCom_Plate_KBS ||
                    m_arrPlates[0] is CConCom_Plate_KC ||
                    m_arrPlates[0] is CConCom_Plate_KCS ||
                    m_arrPlates[0] is CConCom_Plate_KD ||
                    m_arrPlates[0] is CConCom_Plate_KDS ||
                    m_arrPlates[0] is CConCom_Plate_KES ||
                    m_arrPlates[0] is CConCom_Plate_KFS ||
                    m_arrPlates[0] is CConCom_Plate_KGS ||
                    m_arrPlates[0] is CConCom_Plate_KHS)
                {
                    // Pretocit smer skrutiek
                    ((CPlate_Frame)m_arrPlates[0]).ScrewInPlusZDirection = !((CPlate_Frame)m_arrPlates[0]).ScrewInPlusZDirection;
                    m_arrPlates[0].ScrewArrangement.UpdateArrangmentData();
                    m_arrPlates[0].UpdatePlateData(m_arrPlates[0].ScrewArrangement);
                    fControlPointYCoord1 += m_arrPlates[0].Ft;
                    ((CPlate_Frame)m_arrPlates[0]).MirrorPlate();

                    /*
                    // ????? Volat individualne Calc_KneePlateData podla typu plate
                    if (plate is CConCom_Plate_KA)
                    screwArrangement.Calc_KneePlateData(m_fbX1, m_fbX2, 0, m_fhY1, Ft, FSlope_rad, ScrewInPlusZDirection);
                    else if(plate is CConCom_Plate_KB)
                    screwArrangement.Calc_KneePlateData(m_fbX1, m_fbX2, 0, m_fhY1, Ft, FSlope_rad, ScrewInPlusZDirection);
                    */
                }

                if (/*m_arrPlates[1] is CConCom_Plate_KA ||*/
                    m_arrPlates[1] is CConCom_Plate_KB ||
                    m_arrPlates[1] is CConCom_Plate_KBS ||
                    m_arrPlates[1] is CConCom_Plate_KC ||
                    m_arrPlates[1] is CConCom_Plate_KCS ||
                    m_arrPlates[1] is CConCom_Plate_KD ||
                    m_arrPlates[1] is CConCom_Plate_KDS ||
                    m_arrPlates[1] is CConCom_Plate_KES ||
                    m_arrPlates[1] is CConCom_Plate_KFS ||
                    m_arrPlates[1] is CConCom_Plate_KGS ||
                    m_arrPlates[1] is CConCom_Plate_KHS)
                {
                    ((CPlate_Frame)m_arrPlates[1]).ScrewInPlusZDirection = !((CPlate_Frame)m_arrPlates[1]).ScrewInPlusZDirection;
                    m_arrPlates[1].ScrewArrangement.UpdateArrangmentData();
                    m_arrPlates[1].UpdatePlateData(m_arrPlates[1].ScrewArrangement);
                    //fControlPointYCoord2 -= m_arrPlates[1].Ft;
                    //((CPlate_Frame)m_arrPlates[1]).MirrorPlate();
                }
            }

            if (!IsFront)
            {

            }

            m_arrPlates[0].m_pControlPoint = new Point3D(fControlPointXCoord1, fControlPointYCoord1, m_pUpperLeftPointOfPlate.Y - fPlate1_h_Y1);
            m_arrPlates[1].m_pControlPoint = new Point3D(fControlPointXCoord2, fControlPointYCoord2, m_pUpperLeftPointOfPlate.Y - fPlate2_h_Y1);

            Vector3D RotationVector_P1 = new Vector3D(90, 0, fRotatePlatesInJointAngle);
            Vector3D RotationVector_P2 = new Vector3D(90, 0, fRotatePlatesInJointAngle);

            m_arrPlates[0].SetPlateRotation(RotationVector_P1);
            m_arrPlates[1].SetPlateRotation(RotationVector_P2);
        }

        private void GetKneePlateGeneralParameters(CPlate plate, out float fb_X1, out float fh_Y1, out float fh_Y2)
        {
            // Todo - dalo by sa pridat aj dalsie spolocne parametre plates K
            // TO Ondrej - Asi by malo zmysel spravit samostatneho predka pre K a J plate a dat premenne ktore su v triedach KXX spolocne do K-predka a v JXX do J-predka
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
            else if (plate is CConCom_Plate_KGS)
            {
                CConCom_Plate_KGS plate_KHS = (CConCom_Plate_KGS)plate;
                fb_X1 = plate_KHS.Fb_X1;
                fh_Y1 = plate_KHS.Fh_Y1;
                fh_Y2 = plate_KHS.Fh_Y2;
            }
            else if (plate is CConCom_Plate_KHS)
            {
                CConCom_Plate_KHS plate_KHS = (CConCom_Plate_KHS)plate;
                fb_X1 = plate_KHS.Fb_X1;
                fh_Y1 = plate_KHS.Fh_Y1;
                fh_Y2 = plate_KHS.Fh_Y2;
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
