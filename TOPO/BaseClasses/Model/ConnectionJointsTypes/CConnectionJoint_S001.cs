using System;
using System.Windows;
using System.Collections.Generic;
using System.Globalization;
using MATH;
using DATABASE;
using DATABASE.DTO;
using BaseClasses.GraphObj;
using System.Windows.Media.Media3D;
using BaseClasses.Helpers;

namespace BaseClasses
{
    public class CConnectionJoint_S001 : CConnectionJointTypes
    {
        // Column to Edge Rafter Joint
        public float m_ft;
        public float m_fRoofPitch_rad;
        public bool m_bSwitchConnectedSide_Z;

        public CConnectionJoint_S001() { }

        public CConnectionJoint_S001(CNode Node_temp, CMember MainMember_temp, CMember SecondaryConnectedMember_temp, float fRoofPitch_rad, bool bSwitchConnectedSide_Z, bool bIsDisplayed_temp)
        {
            bIsJointDefinedinGCS = false;

            m_Node = Node_temp;
            m_pControlPoint = m_Node.GetPoint3D();
            m_MainMember = MainMember_temp;
            m_SecondaryMembers = new CMember[1];
            m_SecondaryMembers[0] = SecondaryConnectedMember_temp;
            m_fRoofPitch_rad = fRoofPitch_rad;
            BIsGenerated = true;
            BIsDisplayed = bIsDisplayed_temp;
            m_bSwitchConnectedSide_Z = bSwitchConnectedSide_Z;
            m_ft = 0.002f;

            // Joint is defined in start point and LCS of secondary member [0,y,z]
            // Plates are usually defined in x,y coordinates

            bool bUsePlatesTypeN = false; // Nastavuje sa pouzity typ zavetrovacieho plechu

            if (bUsePlatesTypeN)
            {
                // Plate position in x-direction on the secondary member
                float fAlignment_x = 0.1f; // Posun plechu v smere osi x LCS pruta (kladna hodnota je posun v smere +x)
                float fPlateCenterDistanceInx = 0.15f; // Distance between plates

                float flocaleccentricity_y = m_SecondaryMembers[0].EccentricityStart == null ? 0f : m_SecondaryMembers[0].EccentricityStart.MFy_local;
                float flocaleccentricity_z = m_SecondaryMembers[0].EccentricityStart == null ? 0f : m_SecondaryMembers[0].EccentricityStart.MFz_local;

                float alpha2_rad = MathF.fPI / 8f; // 22.5 deg
                float fbX1 = 0.1f;
                float fbX3 = (float)m_SecondaryMembers[0].CrScStart.b;
                float fhY = 0.1f;
                float x_a = (float)Math.Tan(alpha2_rad) * (float)m_SecondaryMembers[0].CrScStart.h;

                // Rotation about longitudinal axis
                float fRotationAboutLCS_x_deg = 0;
                float fControlPointPosition_y_start = -fbX1 - x_a + (float)(m_SecondaryMembers[0].CrScStart.y_min + flocaleccentricity_y);
                float fControlPointPosition_y_end = fbX1 + x_a + (float)(m_SecondaryMembers[0].CrScStart.y_max + flocaleccentricity_y);
                float fControlPointPosition_z_start = -0.5f * (float)m_SecondaryMembers[0].CrScStart.h + flocaleccentricity_z;
                float fControlPointPosition_z_end = -0.5f * (float)m_SecondaryMembers[0].CrScStart.h + flocaleccentricity_z;

                if (bSwitchConnectedSide_Z)
                {
                    fRotationAboutLCS_x_deg = 180;
                    fControlPointPosition_y_start = fbX1 + x_a + (float)(m_SecondaryMembers[0].CrScStart.y_max + flocaleccentricity_y);
                    fControlPointPosition_y_end = -fbX1 - x_a + (float)(m_SecondaryMembers[0].CrScStart.y_min + flocaleccentricity_y);
                    fControlPointPosition_z_start = 0.5f * (float)m_SecondaryMembers[0].CrScStart.h + flocaleccentricity_z;
                    fControlPointPosition_z_end = 0.5f * (float)m_SecondaryMembers[0].CrScStart.h + flocaleccentricity_z;
                }

                Point3D ControlPoint_P1 = new Point3D(fAlignment_x, fControlPointPosition_y_start, fControlPointPosition_z_start);
                Point3D ControlPoint_P2 = new Point3D(fAlignment_x + fPlateCenterDistanceInx, fControlPointPosition_y_start, fControlPointPosition_z_start);

                int iConnectorNumberinOnePlate = 12;

                CScrew referenceScrew = new CScrew("TEK", "12");
                CScrewArrangement_N screwArrangement = new CScrewArrangement_N(iConnectorNumberinOnePlate, referenceScrew);

                CConCom_Plate_N pPlate1 = new CConCom_Plate_N("N", ControlPoint_P1, fbX1, (float)m_SecondaryMembers[0].CrScStart.b, fhY, (float)m_SecondaryMembers[0].CrScStart.h, m_ft, 0, fRotationAboutLCS_x_deg, 90, screwArrangement, BIsDisplayed); // Rotation angle in degrees
                CConCom_Plate_N pPlate2 = new CConCom_Plate_N("N", ControlPoint_P2, fbX1, (float)m_SecondaryMembers[0].CrScStart.b, fhY, (float)m_SecondaryMembers[0].CrScStart.h, m_ft, 0, fRotationAboutLCS_x_deg, 90, screwArrangement, BIsDisplayed); // Rotation angle in degrees

                // Identification of current joint node location (start or end definition node of secondary member)
                if (m_Node.ID != m_SecondaryMembers[0].NodeStart.ID) // If true - joint at start node, if false joint at end node (so we need to rotate joint about z-axis 180 deg)
                {
                    // Rotate and move joint defined in the start point [0,0,0] to the end point
                    ControlPoint_P1 = new Point3D(m_SecondaryMembers[0].FLength - fAlignment_x, fControlPointPosition_y_end, fControlPointPosition_z_start);
                    ControlPoint_P2 = new Point3D(m_SecondaryMembers[0].FLength - fAlignment_x - fPlateCenterDistanceInx, fControlPointPosition_y_end, fControlPointPosition_z_start);

                    pPlate1 = new CConCom_Plate_N("N", ControlPoint_P1, fbX1, fbX3, fhY, (float)m_SecondaryMembers[0].CrScStart.h, m_ft, 0, fRotationAboutLCS_x_deg, 180 + 90, screwArrangement, BIsDisplayed); // Rotation angle in degrees
                    pPlate2 = new CConCom_Plate_N("N", ControlPoint_P2, fbX1, fbX3, fhY, (float)m_SecondaryMembers[0].CrScStart.h, m_ft, 0, fRotationAboutLCS_x_deg, 180 + 90, screwArrangement, BIsDisplayed); // Rotation angle in degrees
                }

                int iPlateNumber = 2; // Number of plates (metal strips) in the connection

                if (iPlateNumber == 2)
                {
                    m_arrPlates = new CPlate[2];
                    m_arrPlates[0] = pPlate1;
                    m_arrPlates[1] = pPlate2;
                }
                else
                {
                    m_arrPlates = new CPlate[1]; //  Just one plate in joint

                    m_arrPlates[0] = pPlate1;
                }
            }
            else
            {
                // Plate position in x-direction on the secondary member
                float fAlignment_x = m_SecondaryMembers[0].EccentricityStart == null ? 0f : m_SecondaryMembers[0].FAlignment_End; // Posun plechu v smere osi x LCS pruta (kladna hodnota je posun v smere +x)

                float flocaleccentricity_y = m_SecondaryMembers[0].EccentricityStart == null ? 0f : m_SecondaryMembers[0].EccentricityStart.MFy_local;
                float flocaleccentricity_z = m_SecondaryMembers[0].EccentricityStart == null ? 0f : m_SecondaryMembers[0].EccentricityStart.MFz_local;

                float fGamma_rad = MathF.fPI / 6f;
                float fColumnDepth = (float)m_SecondaryMembers[0].CrScStart.h;
                float fhY = 0.05f; // 
                float fbX1 = fColumnDepth / (float)Math.Cos(fGamma_rad) + Math.Min(0.5f * fhY, 0.5f * (float)m_MainMember.CrScStart.b);
                float fOffsetAboveRafter = 0.01f; // Dodatocny posun plechu smerom hore nad rafter tak, aby sa neprekryvali plochy plechu a rafteru. Bolo by potrebne predefinovat plechy, aby mal konce kde sa prekryva s rafterom uplne ploche
 
                // Rotation about longitudinal axis
                float fRotationAboutLCS_x_deg = 0;
                float fControlPointPosition_y_start = flocaleccentricity_y;
                float fControlPointPosition_y_end = flocaleccentricity_y;
                float fControlPointPosition_z_start = 0.5f * (float)m_SecondaryMembers[0].CrScStart.h + flocaleccentricity_z;
                float fControlPointPosition_z_end = 0.5f * (float)m_SecondaryMembers[0].CrScStart.h + flocaleccentricity_z;

                if (bSwitchConnectedSide_Z)
                {
                    fRotationAboutLCS_x_deg = 180;
                    fControlPointPosition_y_start = flocaleccentricity_y;
                    fControlPointPosition_y_end = flocaleccentricity_y;
                    fControlPointPosition_z_start = -0.5f * (float)m_SecondaryMembers[0].CrScStart.h + flocaleccentricity_z;
                    fControlPointPosition_z_end = -0.5f * (float)m_SecondaryMembers[0].CrScStart.h + flocaleccentricity_z;
                }

                // Zaporna suradnica x posuva plech pred zaciatok pruta
                Point3D ControlPoint_P1 = new Point3D(-fAlignment_x - fOffsetAboveRafter - fhY, fControlPointPosition_y_start, fControlPointPosition_z_start);

                float fbPlate2 = 0.05f;
                float fhPlate2 = 0.27f;
                Point3D ControlPoint_P2 = new Point3D(-fAlignment_x - fOffsetAboveRafter - fhPlate2, fControlPointPosition_y_start + (float)m_SecondaryMembers[0].CrScStart.y_min - fbPlate2, fControlPointPosition_z_start);
                Point3D ControlPoint_P3 = new Point3D(-fAlignment_x - fOffsetAboveRafter - fhPlate2, fControlPointPosition_y_start + (float)m_SecondaryMembers[0].CrScStart.y_max, fControlPointPosition_z_start);

                int iConnectorNumberinOnePlate = 6;

                CScrew referenceScrew = new CScrew("TEK", "12");
                CScrewArrangement_M screwArrangement = new CScrewArrangement_M(iConnectorNumberinOnePlate, referenceScrew);
                CScrewArrangement_L screwArrangement_L = new CScrewArrangement_L(16, referenceScrew);

                CConCom_Plate_M pPlate1 = new CConCom_Plate_M("M", ControlPoint_P1, fbX1, fbX1, fhY, m_ft, (float)m_SecondaryMembers[0].CrScStart.b, m_fRoofPitch_rad, fGamma_rad, 0, fRotationAboutLCS_x_deg, 90, screwArrangement, BIsDisplayed); // Rotation angle in degrees

                CConCom_Plate_F_or_L pPlate2 = new CConCom_Plate_F_or_L("LH", ControlPoint_P2, fbPlate2, fhPlate2, fbPlate2, m_ft, 0, 0, 90, screwArrangement_L, true);
                CConCom_Plate_F_or_L pPlate3 = new CConCom_Plate_F_or_L("LH", ControlPoint_P3, fbPlate2, fhPlate2, fbPlate2, m_ft, 0, 0, 90, screwArrangement_L, true);

                // Identification of current joint node location (start or end definition node of secondary member)
                if (m_Node.ID != m_SecondaryMembers[0].NodeStart.ID) // If true - joint at start node, if false joint at end node (so we need to rotate joint about z-axis 180 deg)
                {
                    // Rotate and move joint defined in the start point [0,0,0] to the end point
                    ControlPoint_P1 = new Point3D(m_SecondaryMembers[0].FLength + fAlignment_x + fOffsetAboveRafter - fhY, fControlPointPosition_y_end, fControlPointPosition_z_start);

                    ControlPoint_P2 = new Point3D(m_SecondaryMembers[0].FLength + fAlignment_x + fOffsetAboveRafter - fhPlate2, fControlPointPosition_y_end, fControlPointPosition_z_start);
                    ControlPoint_P3 = new Point3D(m_SecondaryMembers[0].FLength + fAlignment_x + fOffsetAboveRafter - fhPlate2, fControlPointPosition_y_end, fControlPointPosition_z_start);

                    pPlate1 = new CConCom_Plate_M("M", ControlPoint_P1, fbX1, fbX1, fhY, m_ft, (float)m_SecondaryMembers[0].CrScStart.b, m_fRoofPitch_rad, fGamma_rad, 0, fRotationAboutLCS_x_deg, 180 + 90, screwArrangement, BIsDisplayed); // Rotation angle in degrees

                    pPlate2 = new CConCom_Plate_F_or_L("LH", ControlPoint_P2, fbPlate2, fhPlate2, fbPlate2, m_ft, 0, 0, -90, screwArrangement_L, true);
                    pPlate3 = new CConCom_Plate_F_or_L("LH", ControlPoint_P3, fbPlate2, fhPlate2, fbPlate2, m_ft, 0, 0, -90, screwArrangement_L, true);
                }

                m_arrPlates = new CPlate[3]; // Three plates in joint (1 + 2)
                m_arrPlates[0] = pPlate2;
                m_arrPlates[1] = pPlate3;
                m_arrPlates[2] = pPlate1; // TODO ??? Dlhy pasik plate M posielam do pola ako posledny, lebo z prvej plate sa urcuju rozmery prutov pre preview a je lepsie tam mat plate s kratkymi stranami
            }
        }

        public override CConnectionJointTypes RecreateJoint()
        {
            return new CConnectionJoint_S001(m_Node, m_MainMember,m_SecondaryMembers[0], m_fRoofPitch_rad, m_bSwitchConnectedSide_Z, BIsDisplayed);
        }
    }
}
