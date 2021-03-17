﻿using System;
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
        public bool m_bWindPostEndUnderRafter;
        public bool m_bSwitchConnectedSide_Z;
        public bool bUsePlatesTypeN = false; // Nastavuje sa pouzity typ zavetrovacieho plechu, N - stary typ, M - novy typ
        public bool bUseSamePlates = false; // Nastavuje ci sa ma pouzit na oboch stranach plate G alebo na jednej strane G a na druhej H

        public CConnectionJoint_S001() { }

        public CConnectionJoint_S001(EJointType jointType_temp, CNode Node_temp, CMember MainMember_temp, CMember SecondaryConnectedMember_temp, float fRoofPitch_rad, bool bWindPostEndUnderRafter, bool bSwitchConnectedSide_Z)
        {
            bIsJointDefinedinGCS = false;

            JointType = jointType_temp;
            m_Node = Node_temp;
            ControlPoint = m_Node.GetPoint3D();
            m_MainMember = MainMember_temp;
            m_SecondaryMembers = new CMember[1];
            m_SecondaryMembers[0] = SecondaryConnectedMember_temp;
            m_fRoofPitch_rad = fRoofPitch_rad;
            m_bWindPostEndUnderRafter = bWindPostEndUnderRafter;
            m_bSwitchConnectedSide_Z = bSwitchConnectedSide_Z;
            m_ft = 0.002f;

            // Joint is defined in start point and LCS of secondary member [0,y,z]
            // Plates are usually defined in x,y coordinates

            if (bUsePlatesTypeN) // Stary typ N
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

                CScrew referenceScrew = new CScrew("TEK", "14");
                CScrewArrangement_N screwArrangement = new CScrewArrangement_N(iConnectorNumberinOnePlate, referenceScrew);

                CConCom_Plate_N pPlate1 = new CConCom_Plate_N("N", ControlPoint_P1, fbX1, (float)m_SecondaryMembers[0].CrScStart.b, fhY, (float)m_SecondaryMembers[0].CrScStart.h, m_ft, 0, fRotationAboutLCS_x_deg, 90, screwArrangement); // Rotation angle in degrees
                CConCom_Plate_N pPlate2 = new CConCom_Plate_N("N", ControlPoint_P2, fbX1, (float)m_SecondaryMembers[0].CrScStart.b, fhY, (float)m_SecondaryMembers[0].CrScStart.h, m_ft, 0, fRotationAboutLCS_x_deg, 90, screwArrangement); // Rotation angle in degrees

                // Identification of current joint node location (start or end definition node of secondary member)
                if (m_Node.ID != m_SecondaryMembers[0].NodeStart.ID) // If true - joint at start node, if false joint at end node (so we need to rotate joint about z-axis 180 deg)
                {
                    // Rotate and move joint defined in the start point [0,0,0] to the end point
                    ControlPoint_P1 = new Point3D(m_SecondaryMembers[0].FLength - fAlignment_x, fControlPointPosition_y_end, fControlPointPosition_z_start);
                    ControlPoint_P2 = new Point3D(m_SecondaryMembers[0].FLength - fAlignment_x - fPlateCenterDistanceInx, fControlPointPosition_y_end, fControlPointPosition_z_start);

                    pPlate1 = new CConCom_Plate_N("N", ControlPoint_P1, fbX1, fbX3, fhY, (float)m_SecondaryMembers[0].CrScStart.h, m_ft, 0, fRotationAboutLCS_x_deg, 180 + 90, screwArrangement); // Rotation angle in degrees
                    pPlate2 = new CConCom_Plate_N("N", ControlPoint_P2, fbX1, fbX3, fhY, (float)m_SecondaryMembers[0].CrScStart.h, m_ft, 0, fRotationAboutLCS_x_deg, 180 + 90, screwArrangement); // Rotation angle in degrees
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
            else if(!bWindPostEndUnderRafter) // Novy typ - M
            {
                // Plate position in x-direction on the secondary member
                float fAlignment_x;
                    fAlignment_x = m_SecondaryMembers[0].EccentricityStart == null ? 0f : m_SecondaryMembers[0].FAlignment_Start; // Posun v smere osi x LCS pruta

                if (m_Node.ID != m_SecondaryMembers[0].NodeStart.ID) // End node
                    fAlignment_x = m_SecondaryMembers[0].EccentricityStart == null ? 0f : m_SecondaryMembers[0].FAlignment_End; // Posun v smere osi x LCS pruta

                float flocaleccentricity_y = m_SecondaryMembers[0].EccentricityStart == null ? 0f : m_SecondaryMembers[0].EccentricityStart.MFy_local;
                float flocaleccentricity_z = m_SecondaryMembers[0].EccentricityStart == null ? 0f : m_SecondaryMembers[0].EccentricityStart.MFz_local;

                float fGamma_rad = MathF.fPI / 6f;
                float fColumnDepth = (float)m_SecondaryMembers[0].CrScStart.h;
                float fh_Plate1 = 0.05f; //
                float fbX1_Plate1 = fColumnDepth / (float)Math.Cos(fGamma_rad) + 0.5f * fh_Plate1;
                float fOffsetAboveRafter = 0.01f; // Dodatocny posun plechu smerom hore nad rafter tak, aby sa neprekryvali plochy plechu a rafteru. Bolo by potrebne predefinovat plechy, aby mal konce kde sa prekryva s rafterom uplne ploche
                float ft_Plate1 = 0.001f;

                float fb_Plate2 = 0.05f;
                float fh_Plate2 = 0.27f;
                float ft_Plate2 = 0.001f;

                float fPlateOffset_y = 0.5f * (float)m_SecondaryMembers[0].CrScStart.b + (float)m_SecondaryMembers[0].CrScStart.y_min;

                // Rotation about longitudinal axis
                // Front Side of Building
                float fRotationAboutLCS_x_deg = 0;
                float fControlPointPosition_x_start = -fAlignment_x - fOffsetAboveRafter + fh_Plate1;
                float fControlPointPosition_x_end = m_SecondaryMembers[0].FLength + fAlignment_x + fOffsetAboveRafter - fh_Plate1;
                float fControlPointPosition_y_start = flocaleccentricity_y + fPlateOffset_y;
                float fControlPointPosition_y_end = flocaleccentricity_y + fPlateOffset_y;
                float fControlPointPosition_z_start = 0.5f * (float)m_SecondaryMembers[0].CrScStart.h + flocaleccentricity_z;
                float fControlPointPosition_z_end = 0.5f * (float)m_SecondaryMembers[0].CrScStart.h + flocaleccentricity_z;

                float fRotation2AboutLCS_y_deg = 0;
                float fRotation2AboutLCS_z_deg = -90;
                float fControlPoint2Position_x_start = -fAlignment_x;
                float fControlPoint2Position_x_end = m_SecondaryMembers[0].FLength + fAlignment_x - fh_Plate2;
                float fControlPoint2Position_y_start = flocaleccentricity_y + (float)m_SecondaryMembers[0].CrScStart.y_min;
                float fControlPoint2Position_y_end = flocaleccentricity_y + (float)m_SecondaryMembers[0].CrScStart.y_min;
                float fControlPoint2Position_z_start = (float)m_SecondaryMembers[0].CrScStart.z_min + flocaleccentricity_z;
                float fControlPoint2Position_z_end = (float)m_SecondaryMembers[0].CrScStart.z_min + flocaleccentricity_z;

                float fRotation3AboutLCS_y_deg = 180 + 90;
                float fRotation3AboutLCS_z_deg = -90;
                float fControlPoint3Position_x_start = -fAlignment_x;
                float fControlPoint3Position_x_end = m_SecondaryMembers[0].FLength + fAlignment_x - fh_Plate2;
                float fControlPoint3Position_y_start = flocaleccentricity_y + (float)m_SecondaryMembers[0].CrScStart.y_max;
                float fControlPoint3Position_y_end = flocaleccentricity_y + (float)m_SecondaryMembers[0].CrScStart.y_max;
                float fControlPoint3Position_z_start = (float)m_SecondaryMembers[0].CrScStart.z_min + flocaleccentricity_z;
                float fControlPoint3Position_z_end = (float)m_SecondaryMembers[0].CrScStart.z_min + flocaleccentricity_z;

                if (bSwitchConnectedSide_Z) // Back Side of Building
                {
                    fRotationAboutLCS_x_deg = 180;
                    fControlPointPosition_y_start = flocaleccentricity_y + fPlateOffset_y;
                    fControlPointPosition_y_end = flocaleccentricity_y + fPlateOffset_y;
                    fControlPointPosition_z_start = -0.5f * (float)m_SecondaryMembers[0].CrScStart.h + flocaleccentricity_z;
                    fControlPointPosition_z_end = -0.5f * (float)m_SecondaryMembers[0].CrScStart.h + flocaleccentricity_z;

                    fRotation2AboutLCS_y_deg = 90;
                    fRotation2AboutLCS_z_deg = -90;
                    fControlPoint2Position_x_start = -fAlignment_x;
                    fControlPoint2Position_x_end = m_SecondaryMembers[0].FLength + fAlignment_x - fh_Plate2;
                    fControlPoint2Position_y_start = flocaleccentricity_y + (float)m_SecondaryMembers[0].CrScStart.y_min;
                    fControlPoint2Position_y_end = flocaleccentricity_y + (float)m_SecondaryMembers[0].CrScStart.y_min;
                    fControlPoint2Position_z_start = (float)m_SecondaryMembers[0].CrScStart.z_max + flocaleccentricity_z;
                    fControlPoint2Position_z_end = (float)m_SecondaryMembers[0].CrScStart.z_max + flocaleccentricity_z;

                    fRotation3AboutLCS_y_deg = 180;
                    fRotation3AboutLCS_z_deg = -90;
                    fControlPoint3Position_x_start = -fAlignment_x;
                    fControlPoint3Position_x_end = m_SecondaryMembers[0].FLength + fAlignment_x - fh_Plate2;
                    fControlPoint3Position_y_start = flocaleccentricity_y + (float)m_SecondaryMembers[0].CrScStart.y_max;
                    fControlPoint3Position_y_end = flocaleccentricity_y + (float)m_SecondaryMembers[0].CrScStart.y_max;
                    fControlPoint3Position_z_start = (float)m_SecondaryMembers[0].CrScStart.z_max + flocaleccentricity_z;
                    fControlPoint3Position_z_end = (float)m_SecondaryMembers[0].CrScStart.z_max + flocaleccentricity_z;
                }

                // Zaporna suradnica x posuva plech pred zaciatok pruta
                Point3D ControlPoint_P1 = new Point3D(fControlPointPosition_x_start, fControlPointPosition_y_start, fControlPointPosition_z_start);
                Point3D ControlPoint_P2 = new Point3D(fControlPoint2Position_x_start, fControlPoint2Position_y_start, fControlPoint2Position_z_start);
                Point3D ControlPoint_P3 = new Point3D(fControlPoint3Position_x_start, fControlPoint3Position_y_start, fControlPoint3Position_z_start);

                int iConnectorNumberinOnePlate = 6;

                CScrew referenceScrew = new CScrew("TEK", "14");
                CScrewArrangement_M screwArrangement = new CScrewArrangement_M(iConnectorNumberinOnePlate, referenceScrew);
                CScrewArrangement_L screwArrangement_L = new CScrewArrangement_L(16, referenceScrew, 0.010f, 0.010f, 0.030f, 0.090f, fh_Plate2, fh_Plate2, 0f, 0f);

                CConCom_Plate_M pPlate1 = new CConCom_Plate_M("M", ControlPoint_P1, fbX1_Plate1, fbX1_Plate1, fh_Plate1, ft_Plate1, (float)m_SecondaryMembers[0].CrScStart.b, m_fRoofPitch_rad, fGamma_rad, 0, fRotationAboutLCS_x_deg, 90, screwArrangement); // Rotation angle in degrees

                CConCom_Plate_F_or_L pPlate2 = new CConCom_Plate_F_or_L("LH", ControlPoint_P2, fb_Plate2, fh_Plate2, fb_Plate2, ft_Plate2, (float)m_SecondaryMembers[0].CrScStart.h, 0, fRotation2AboutLCS_y_deg, fRotation2AboutLCS_z_deg, screwArrangement_L);
                CConCom_Plate_F_or_L pPlate3 = new CConCom_Plate_F_or_L("LH", ControlPoint_P3, fb_Plate2, fh_Plate2, fb_Plate2, ft_Plate2, (float)m_SecondaryMembers[0].CrScStart.h, 0, fRotation3AboutLCS_y_deg, fRotation3AboutLCS_z_deg, screwArrangement_L);

                // Identification of current joint node location (start or end definition node of secondary member)
                if (m_Node.ID != m_SecondaryMembers[0].NodeStart.ID) // If true - joint at start node, if false joint at end node (so we need to rotate joint about z-axis 180 deg)
                {
                    // Rotate and move joint defined in the start point [0,0,0] to the end point
                    ControlPoint_P1 = new Point3D(fControlPointPosition_x_end, fControlPointPosition_y_end, fControlPointPosition_z_start);

                    ControlPoint_P2 = new Point3D(fControlPoint2Position_x_end, fControlPoint2Position_y_end, fControlPoint2Position_z_end);
                    ControlPoint_P3 = new Point3D(fControlPoint3Position_x_end, fControlPoint3Position_y_end, fControlPoint3Position_z_end);

                    pPlate1 = new CConCom_Plate_M("M", ControlPoint_P1, fbX1_Plate1, fbX1_Plate1, fh_Plate1, ft_Plate1, (float)m_SecondaryMembers[0].CrScStart.b, m_fRoofPitch_rad, fGamma_rad, 0, fRotationAboutLCS_x_deg, 180 + 90, screwArrangement); // Rotation angle in degrees

                    pPlate2 = new CConCom_Plate_F_or_L("LH", ControlPoint_P2, fb_Plate2, fh_Plate2, fb_Plate2, ft_Plate2, (float)m_SecondaryMembers[0].CrScStart.h, 0, fRotation2AboutLCS_y_deg, fRotation2AboutLCS_z_deg, screwArrangement_L);
                    pPlate3 = new CConCom_Plate_F_or_L("LH", ControlPoint_P3, fb_Plate2, fh_Plate2, fb_Plate2, ft_Plate2, (float)m_SecondaryMembers[0].CrScStart.h, 0, fRotation3AboutLCS_y_deg, fRotation3AboutLCS_z_deg, screwArrangement_L);
                }

                m_arrPlates = new CPlate[3]; // Three plates in joint (1 + 2)
                m_arrPlates[0] = pPlate2;
                m_arrPlates[1] = pPlate3;
                m_arrPlates[2] = pPlate1; // TODO ??? Dlhy pasik plate M posielam do pola ako posledny, lebo z prvej plate sa urcuju rozmery prutov pre preview a je lepsie tam mat plate s kratkymi stranami
            }
            else if(bUseSamePlates)
            {
                // Plate position in x-direction on the secondary member
                float fAlignment_x;
                fAlignment_x = m_SecondaryMembers[0].FAlignment_Start; // Posun v smere osi x LCS pruta

                if (m_Node.ID != m_SecondaryMembers[0].NodeStart.ID) // End node
                    fAlignment_x = m_SecondaryMembers[0].FAlignment_End; // Posun v smere osi x LCS pruta

                float flocaleccentricity_y = m_SecondaryMembers[0].EccentricityStart == null ? 0f : m_SecondaryMembers[0].EccentricityStart.MFy_local;
                float flocaleccentricity_z = m_SecondaryMembers[0].EccentricityStart == null ? 0f : m_SecondaryMembers[0].EccentricityStart.MFz_local;

                float fb1_Plate = 0.05f;
                float fb2_Plate = 0.3f;
                float fhY1_Plate = 0.3f;
                float fhY2_Plate = 0.8f;
                float flZ_Plate = 0.1f;

                //float fPlateOffset_y = 0.5f * (float)m_SecondaryMembers[0].CrScStart.b + (float)m_SecondaryMembers[0].CrScStart.y_min;

                // Rotation about longitudinal axis
                // Front Side of Building
                float fRotation1AboutLCS_y_deg_start = 270;
                float fRotation1AboutLCS_z_deg_start = 90;
                float fRotation1AboutLCS_y_deg_end = 270;
                float fRotation1AboutLCS_z_deg_end = 270;
                float fControlPoint1Position_x_start = -fAlignment_x + fhY1_Plate;
                float fControlPoint1Position_x_end = m_SecondaryMembers[0].FLength + fAlignment_x - fhY1_Plate;
                float fControlPoint1Position_y_start = flocaleccentricity_y + (float)m_SecondaryMembers[0].CrScStart.y_min;
                float fControlPoint1Position_y_end = flocaleccentricity_y + (float)m_SecondaryMembers[0].CrScStart.y_max;
                float fControlPoint1Position_z_start = (float)m_SecondaryMembers[0].CrScStart.z_min + flocaleccentricity_z;
                float fControlPoint1Position_z_end = (float)m_SecondaryMembers[0].CrScStart.z_min + flocaleccentricity_z;

                float fRotation2AboutLCS_y_deg_start = 90;
                float fRotation2AboutLCS_z_deg_start = 90;
                float fRotation2AboutLCS_y_deg_end = 90;
                float fRotation2AboutLCS_z_deg_end = 270;
                float fControlPoint2Position_x_start = -fAlignment_x + fhY1_Plate;
                float fControlPoint2Position_x_end = m_SecondaryMembers[0].FLength + fAlignment_x - fhY1_Plate;
                float fControlPoint2Position_y_start = flocaleccentricity_y + (float)m_SecondaryMembers[0].CrScStart.y_max;
                float fControlPoint2Position_y_end = flocaleccentricity_y + (float)m_SecondaryMembers[0].CrScStart.y_min;
                float fControlPoint2Position_z_start = (float)m_SecondaryMembers[0].CrScStart.z_min + flocaleccentricity_z;
                float fControlPoint2Position_z_end = (float)m_SecondaryMembers[0].CrScStart.z_min + flocaleccentricity_z;

                if (bSwitchConnectedSide_Z) // Back Side of Building
                {
                    fRotation1AboutLCS_y_deg_start = 90;
                    fRotation1AboutLCS_z_deg_start = 90;
                    fRotation1AboutLCS_y_deg_end = 90;
                    fRotation1AboutLCS_z_deg_end = 270;
                    fControlPoint1Position_x_start = -fAlignment_x + fhY1_Plate;
                    fControlPoint1Position_x_end = m_SecondaryMembers[0].FLength + fAlignment_x - fhY1_Plate;
                    fControlPoint1Position_y_start = flocaleccentricity_y + (float)m_SecondaryMembers[0].CrScStart.y_max;
                    fControlPoint1Position_y_end = flocaleccentricity_y + (float)m_SecondaryMembers[0].CrScStart.y_min;
                    fControlPoint1Position_z_start = (float)m_SecondaryMembers[0].CrScStart.z_max + flocaleccentricity_z;
                    fControlPoint1Position_z_end = (float)m_SecondaryMembers[0].CrScStart.z_max + flocaleccentricity_z;

                    fRotation2AboutLCS_y_deg_start = 270;
                    fRotation2AboutLCS_z_deg_start = 90;
                    fRotation2AboutLCS_y_deg_end = 270;
                    fRotation2AboutLCS_z_deg_end = 270;
                    fControlPoint2Position_x_start = -fAlignment_x + fhY1_Plate;
                    fControlPoint2Position_x_end = m_SecondaryMembers[0].FLength + fAlignment_x - fhY1_Plate;
                    fControlPoint2Position_y_start = flocaleccentricity_y + (float)m_SecondaryMembers[0].CrScStart.y_min;
                    fControlPoint2Position_y_end = flocaleccentricity_y + (float)m_SecondaryMembers[0].CrScStart.y_max;
                    fControlPoint2Position_z_start = (float)m_SecondaryMembers[0].CrScStart.z_max + flocaleccentricity_z;
                    fControlPoint2Position_z_end = (float)m_SecondaryMembers[0].CrScStart.z_max + flocaleccentricity_z;
                }

                // Zaporna suradnica x posuva plech pred zaciatok pruta
                Point3D ControlPoint_P1 = new Point3D(fControlPoint1Position_x_start, fControlPoint1Position_y_start, fControlPoint1Position_z_start);
                Point3D ControlPoint_P2 = new Point3D(fControlPoint2Position_x_start, fControlPoint2Position_y_start, fControlPoint2Position_z_start);

                CScrew referenceScrew = new CScrew("TEK", "14");
                CScrewArrangement_G screwArrangement_G1 = new CScrewArrangement_G(referenceScrew);
                CScrewArrangement_G screwArrangement_G2 = new CScrewArrangement_G(referenceScrew);

                CConCom_Plate_G pPlate1 = new CConCom_Plate_G("G - LH", ControlPoint_P1, fb1_Plate, fb2_Plate, fhY1_Plate, fhY2_Plate, flZ_Plate, (float)m_MainMember.CrScStart.h, m_ft, 0, fRotation1AboutLCS_y_deg_start, fRotation1AboutLCS_z_deg_start, screwArrangement_G1);
                CConCom_Plate_G pPlate2 = new CConCom_Plate_G("G - RH", ControlPoint_P2, fb1_Plate, fb2_Plate, fhY1_Plate, fhY2_Plate, flZ_Plate, (float)m_MainMember.CrScStart.h, m_ft, 0, fRotation2AboutLCS_y_deg_start, fRotation2AboutLCS_z_deg_start, screwArrangement_G2);

                // Identification of current joint node location (start or end definition node of secondary member)
                if (m_Node.ID != m_SecondaryMembers[0].NodeStart.ID) // If true - joint at start node, if false joint at end node (so we need to rotate joint about z-axis 180 deg)
                {
                    // Rotate and move joint defined in the start point [0,0,0] to the end point
                    ControlPoint_P1 = new Point3D(fControlPoint1Position_x_end, fControlPoint1Position_y_end, fControlPoint1Position_z_end);
                    ControlPoint_P2 = new Point3D(fControlPoint2Position_x_end, fControlPoint2Position_y_end, fControlPoint2Position_z_end);

                    pPlate1 = new CConCom_Plate_G("G - LH", ControlPoint_P1, fb1_Plate, fb2_Plate, fhY1_Plate, fhY2_Plate, flZ_Plate, (float)m_MainMember.CrScStart.h, m_ft, 0, fRotation1AboutLCS_y_deg_end, fRotation1AboutLCS_z_deg_end, screwArrangement_G1);
                    pPlate2 = new CConCom_Plate_G("G - RH", ControlPoint_P2, fb1_Plate, fb2_Plate, fhY1_Plate, fhY2_Plate, flZ_Plate, (float)m_MainMember.CrScStart.h, m_ft, 0, fRotation2AboutLCS_y_deg_end, fRotation2AboutLCS_z_deg_end, screwArrangement_G2);
                }

                m_arrPlates = new CPlate[2];
                m_arrPlates[0] = pPlate1;
                m_arrPlates[1] = pPlate2;
            }
            else
            {
                // Plate position in x-direction on the secondary member
                float fAlignment_x;
                fAlignment_x = m_SecondaryMembers[0].FAlignment_Start; // Posun v smere osi x LCS pruta

                if (m_Node.ID != m_SecondaryMembers[0].NodeStart.ID) // End node
                    fAlignment_x = m_SecondaryMembers[0].FAlignment_End; // Posun v smere osi x LCS pruta

                float flocaleccentricity_y = m_SecondaryMembers[0].EccentricityStart == null ? 0f : m_SecondaryMembers[0].EccentricityStart.MFy_local;
                float flocaleccentricity_z = m_SecondaryMembers[0].EccentricityStart == null ? 0f : m_SecondaryMembers[0].EccentricityStart.MFz_local;

                float fOffsetAboveColumnEnd =(float)m_MainMember.CrScStart.h / (float)Math.Cos(fRoofPitch_rad); // Dodatocny posun plechu smerom hore na hornu hranu rafter.

                float fb1_Plate1 = 0.05f;
                float fb2_Plate1 = (float)m_SecondaryMembers[0].CrScStart.h - (float)m_MainMember.CrScStart.b;
                float fhY1_Plate = fb2_Plate1;// 0.3f;
                float fhY2_Plate = fhY1_Plate + fOffsetAboveColumnEnd;//0.8f; // Celkovy rozmer
                float flZ_Plate = 0.1f;

                float fb_Plate2 = (float)m_SecondaryMembers[0].CrScStart.h;
                float fhY1_Plate2 = fb_Plate2; // 0.5f;
                float fhY2_Plate2 = fhY1_Plate2 + 2f * (float)m_MainMember.CrScStart.b; // 0.7f; // Celkovy rozmer

                //float fPlateOffset_y = 0.5f * (float)m_SecondaryMembers[0].CrScStart.b + (float)m_SecondaryMembers[0].CrScStart.y_min;

                // Rotation about longitudinal axis
                // Front Side of Building
                float fRotation1AboutLCS_y_deg_start = 270;
                float fRotation1AboutLCS_z_deg_start = 90;
                float fRotation1AboutLCS_y_deg_end = 270;
                float fRotation1AboutLCS_z_deg_end = 270;
                float fControlPoint1Position_x_start = -fAlignment_x + fhY2_Plate - fOffsetAboveColumnEnd;
                float fControlPoint1Position_x_end = m_SecondaryMembers[0].FLength + fAlignment_x - fhY2_Plate + fOffsetAboveColumnEnd;
                float fControlPoint1Position_y_start = flocaleccentricity_y + (float)m_SecondaryMembers[0].CrScStart.y_min;
                float fControlPoint1Position_y_end = flocaleccentricity_y + (float)m_SecondaryMembers[0].CrScStart.y_max;
                float fControlPoint1Position_z_start = (float)m_SecondaryMembers[0].CrScStart.z_min + flocaleccentricity_z + (float)m_MainMember.CrScStart.b;
                float fControlPoint1Position_z_end = (float)m_SecondaryMembers[0].CrScStart.z_min + flocaleccentricity_z + (float)m_MainMember.CrScStart.b;

                float fRotation2AboutLCS_y_deg_start = 90;
                float fRotation2AboutLCS_z_deg_start = 90;
                float fRotation2AboutLCS_y_deg_end = 90;
                float fRotation2AboutLCS_z_deg_end = 270;
                float fControlPoint2Position_x_start = -fAlignment_x + fhY1_Plate2/* + m_ft*/;
                float fControlPoint2Position_x_end = m_SecondaryMembers[0].FLength + fAlignment_x - fhY1_Plate2/* - m_ft*/;
                float fControlPoint2Position_y_start = flocaleccentricity_y + (float)m_SecondaryMembers[0].CrScStart.y_max;
                float fControlPoint2Position_y_end = flocaleccentricity_y + (float)m_SecondaryMembers[0].CrScStart.y_min;
                float fControlPoint2Position_z_start = (float)m_SecondaryMembers[0].CrScStart.z_min + flocaleccentricity_z;
                float fControlPoint2Position_z_end = (float)m_SecondaryMembers[0].CrScStart.z_min + flocaleccentricity_z;

                if (bSwitchConnectedSide_Z) // Back Side of Building
                {
                    fRotation1AboutLCS_y_deg_start = 90;
                    fRotation1AboutLCS_z_deg_start = 90;
                    fRotation1AboutLCS_y_deg_end = 90;
                    fRotation1AboutLCS_z_deg_end = 270;
                    fControlPoint1Position_x_start = -fAlignment_x + fhY2_Plate - fOffsetAboveColumnEnd;
                    fControlPoint1Position_x_end = m_SecondaryMembers[0].FLength + fAlignment_x - fhY2_Plate + fOffsetAboveColumnEnd;
                    fControlPoint1Position_y_start = flocaleccentricity_y + (float)m_SecondaryMembers[0].CrScStart.y_max;
                    fControlPoint1Position_y_end = flocaleccentricity_y + (float)m_SecondaryMembers[0].CrScStart.y_min;
                    fControlPoint1Position_z_start = (float)m_SecondaryMembers[0].CrScStart.z_max + flocaleccentricity_z - (float)m_MainMember.CrScStart.b;
                    fControlPoint1Position_z_end = (float)m_SecondaryMembers[0].CrScStart.z_max + flocaleccentricity_z - (float)m_MainMember.CrScStart.b;

                    fRotation2AboutLCS_y_deg_start = 270;
                    fRotation2AboutLCS_z_deg_start = 90;
                    fRotation2AboutLCS_y_deg_end = 270;
                    fRotation2AboutLCS_z_deg_end = 270;
                    fControlPoint2Position_x_start = -fAlignment_x + fhY1_Plate2/* + m_ft*/;
                    fControlPoint2Position_x_end = m_SecondaryMembers[0].FLength + fAlignment_x - fhY1_Plate2/* - m_ft*/;
                    fControlPoint2Position_y_start = flocaleccentricity_y + (float)m_SecondaryMembers[0].CrScStart.y_min;
                    fControlPoint2Position_y_end = flocaleccentricity_y + (float)m_SecondaryMembers[0].CrScStart.y_max;
                    fControlPoint2Position_z_start = (float)m_SecondaryMembers[0].CrScStart.z_max + flocaleccentricity_z;
                    fControlPoint2Position_z_end = (float)m_SecondaryMembers[0].CrScStart.z_max + flocaleccentricity_z;
                }

                // Zaporna suradnica x posuva plech pred zaciatok pruta
                Point3D ControlPoint_P1 = new Point3D(fControlPoint1Position_x_start, fControlPoint1Position_y_start, fControlPoint1Position_z_start);
                Point3D ControlPoint_P2 = new Point3D(fControlPoint2Position_x_start, fControlPoint2Position_y_start, fControlPoint2Position_z_start);

                CScrew referenceScrew = new CScrew("TEK", "14");
                CScrewArrangement_G screwArrangement_G1 = new CScrewArrangement_G(referenceScrew);
                CScrewArrangement_H screwArrangement_H2 = new CScrewArrangement_H(referenceScrew);

                CConCom_Plate_G pPlate1 = new CConCom_Plate_G("G - LH", ControlPoint_P1, fb1_Plate1, fb2_Plate1, fhY1_Plate, fhY2_Plate, flZ_Plate, (float)m_MainMember.CrScStart.h, m_ft, 0, fRotation1AboutLCS_y_deg_start, fRotation1AboutLCS_z_deg_start, screwArrangement_G1);

                string plate2Name = "H - LH"; //m_fRoofPitch_rad > 0 ? "H - LH" : "H - RH";
                CConCom_Plate_H pPlate2 = new CConCom_Plate_H(plate2Name, ControlPoint_P2, fb_Plate2, fhY1_Plate2, fhY2_Plate2, (float)m_MainMember.CrScStart.b, m_ft, m_fRoofPitch_rad, 0, fRotation2AboutLCS_y_deg_start, fRotation2AboutLCS_z_deg_start, screwArrangement_H2);

                // Identification of current joint node location (start or end definition node of secondary member)
                if (m_Node.ID != m_SecondaryMembers[0].NodeStart.ID) // If true - joint at start node, if false joint at end node (so we need to rotate joint about z-axis 180 deg)
                {
                    // Rotate and move joint defined in the start point [0,0,0] to the end point
                    ControlPoint_P1 = new Point3D(fControlPoint1Position_x_end, fControlPoint1Position_y_end, fControlPoint1Position_z_end);
                    ControlPoint_P2 = new Point3D(fControlPoint2Position_x_end, fControlPoint2Position_y_end, fControlPoint2Position_z_end);

                    pPlate1 = new CConCom_Plate_G("G - LH", ControlPoint_P1, fb1_Plate1, fb2_Plate1, fhY1_Plate, fhY2_Plate, flZ_Plate, (float)m_MainMember.CrScStart.h, m_ft, 0, fRotation1AboutLCS_y_deg_end, fRotation1AboutLCS_z_deg_end, screwArrangement_G1);

                    plate2Name = "H - RH";
                    pPlate2 = new CConCom_Plate_H(plate2Name, ControlPoint_P2, fb_Plate2, fhY1_Plate2, fhY2_Plate2, (float)m_MainMember.CrScStart.b, m_ft, m_fRoofPitch_rad, 0, fRotation2AboutLCS_y_deg_end, fRotation2AboutLCS_z_deg_end, screwArrangement_H2);
                }

                m_arrPlates = new CPlate[2];
                m_arrPlates[0] = pPlate1;
                m_arrPlates[1] = pPlate2;
            }
        }

        public override CConnectionJointTypes RecreateJoint()
        {
            return new CConnectionJoint_S001(JointType, m_Node, m_MainMember,m_SecondaryMembers[0], m_fRoofPitch_rad, m_bWindPostEndUnderRafter, m_bSwitchConnectedSide_Z);
        }

        public override void UpdateJoint()
        {
            if (bUsePlatesTypeN) // Stary typ N
            {
                throw new NotImplementedException("TODO");
            }
            else if (!m_bWindPostEndUnderRafter) // Novy typ - M
            {
                // Plate position in x-direction on the secondary member
                float fAlignment_x;
                fAlignment_x = m_SecondaryMembers[0].EccentricityStart == null ? 0f : m_SecondaryMembers[0].FAlignment_Start; // Posun v smere osi x LCS pruta

                if (m_Node.ID != m_SecondaryMembers[0].NodeStart.ID) // End node
                    fAlignment_x = m_SecondaryMembers[0].EccentricityStart == null ? 0f : m_SecondaryMembers[0].FAlignment_End; // Posun v smere osi x LCS pruta

                float flocaleccentricity_y = m_SecondaryMembers[0].EccentricityStart == null ? 0f : m_SecondaryMembers[0].EccentricityStart.MFy_local;
                float flocaleccentricity_z = m_SecondaryMembers[0].EccentricityStart == null ? 0f : m_SecondaryMembers[0].EccentricityStart.MFz_local;

                // TODO - dopracovat co ma byt nastavitelne a co sa ma dopocitat

                float fGamma_rad = MathF.fPI / 6f;
                float fColumnDepth = (float)m_SecondaryMembers[0].CrScStart.h;
                float fh_Plate1 = m_arrPlates[2].Height_hy;
                float fbX1_Plate1 = fColumnDepth / (float)Math.Cos(fGamma_rad) + 0.5f * fh_Plate1;
                float fOffsetAboveRafter = 0.01f; // Dodatocny posun plechu smerom hore nad rafter tak, aby sa neprekryvali plochy plechu a rafteru. Bolo by potrebne predefinovat plechy, aby mal konce kde sa prekryva s rafterom uplne ploche
                float ft_Plate1 = m_arrPlates[2].Ft;

                float fb_Plate2 = ((CConCom_Plate_F_or_L)m_arrPlates[0]).Fb_X1;
                float fh_Plate2 = m_arrPlates[0].Height_hy;
                float ft_Plate2 = m_arrPlates[0].Ft;

                float fPlateOffset_y = 0.5f * (float)m_SecondaryMembers[0].CrScStart.b + (float)m_SecondaryMembers[0].CrScStart.y_min;

                // Rotation about longitudinal axis
                // Front Side of Building
                float fRotationAboutLCS_x_deg = 0;
                float fControlPointPosition_x_start = -fAlignment_x - fOffsetAboveRafter + fh_Plate1;
                float fControlPointPosition_x_end = m_SecondaryMembers[0].FLength + fAlignment_x + fOffsetAboveRafter - fh_Plate1;
                float fControlPointPosition_y_start = flocaleccentricity_y + fPlateOffset_y;
                float fControlPointPosition_y_end = flocaleccentricity_y + fPlateOffset_y;
                float fControlPointPosition_z_start = 0.5f * (float)m_SecondaryMembers[0].CrScStart.h + flocaleccentricity_z;
                float fControlPointPosition_z_end = 0.5f * (float)m_SecondaryMembers[0].CrScStart.h + flocaleccentricity_z;

                float fRotation2AboutLCS_y_deg = 0;
                float fRotation2AboutLCS_z_deg = -90;
                float fControlPoint2Position_x_start = -fAlignment_x;
                float fControlPoint2Position_x_end = m_SecondaryMembers[0].FLength + fAlignment_x - fh_Plate2;
                float fControlPoint2Position_y_start = flocaleccentricity_y + (float)m_SecondaryMembers[0].CrScStart.y_min;
                float fControlPoint2Position_y_end = flocaleccentricity_y + (float)m_SecondaryMembers[0].CrScStart.y_min;
                float fControlPoint2Position_z_start = (float)m_SecondaryMembers[0].CrScStart.z_min + flocaleccentricity_z;
                float fControlPoint2Position_z_end = (float)m_SecondaryMembers[0].CrScStart.z_min + flocaleccentricity_z;

                float fRotation3AboutLCS_y_deg = 180 + 90;
                float fRotation3AboutLCS_z_deg = -90;
                float fControlPoint3Position_x_start = -fAlignment_x;
                float fControlPoint3Position_x_end = m_SecondaryMembers[0].FLength + fAlignment_x - fh_Plate2;
                float fControlPoint3Position_y_start = flocaleccentricity_y + (float)m_SecondaryMembers[0].CrScStart.y_max;
                float fControlPoint3Position_y_end = flocaleccentricity_y + (float)m_SecondaryMembers[0].CrScStart.y_max;
                float fControlPoint3Position_z_start = (float)m_SecondaryMembers[0].CrScStart.z_min + flocaleccentricity_z;
                float fControlPoint3Position_z_end = (float)m_SecondaryMembers[0].CrScStart.z_min + flocaleccentricity_z;

                if (m_bSwitchConnectedSide_Z) // Back Side of Building
                {
                    fRotationAboutLCS_x_deg = 180;
                    fControlPointPosition_y_start = flocaleccentricity_y + fPlateOffset_y;
                    fControlPointPosition_y_end = flocaleccentricity_y + fPlateOffset_y;
                    fControlPointPosition_z_start = -0.5f * (float)m_SecondaryMembers[0].CrScStart.h + flocaleccentricity_z;
                    fControlPointPosition_z_end = -0.5f * (float)m_SecondaryMembers[0].CrScStart.h + flocaleccentricity_z;

                    fRotation2AboutLCS_y_deg = 90;
                    fRotation2AboutLCS_z_deg = -90;
                    fControlPoint2Position_x_start = -fAlignment_x;
                    fControlPoint2Position_x_end = m_SecondaryMembers[0].FLength + fAlignment_x - fh_Plate2;
                    fControlPoint2Position_y_start = flocaleccentricity_y + (float)m_SecondaryMembers[0].CrScStart.y_min;
                    fControlPoint2Position_y_end = flocaleccentricity_y + (float)m_SecondaryMembers[0].CrScStart.y_min;
                    fControlPoint2Position_z_start = (float)m_SecondaryMembers[0].CrScStart.z_max + flocaleccentricity_z;
                    fControlPoint2Position_z_end = (float)m_SecondaryMembers[0].CrScStart.z_max + flocaleccentricity_z;

                    fRotation3AboutLCS_y_deg = 180;
                    fRotation3AboutLCS_z_deg = -90;
                    fControlPoint3Position_x_start = -fAlignment_x;
                    fControlPoint3Position_x_end = m_SecondaryMembers[0].FLength + fAlignment_x - fh_Plate2;
                    fControlPoint3Position_y_start = flocaleccentricity_y + (float)m_SecondaryMembers[0].CrScStart.y_max;
                    fControlPoint3Position_y_end = flocaleccentricity_y + (float)m_SecondaryMembers[0].CrScStart.y_max;
                    fControlPoint3Position_z_start = (float)m_SecondaryMembers[0].CrScStart.z_max + flocaleccentricity_z;
                    fControlPoint3Position_z_end = (float)m_SecondaryMembers[0].CrScStart.z_max + flocaleccentricity_z;
                }

                // Zaporna suradnica x posuva plech pred zaciatok pruta
                Point3D ControlPoint_P1 = new Point3D(fControlPointPosition_x_start, fControlPointPosition_y_start, fControlPointPosition_z_start);
                Point3D ControlPoint_P2 = new Point3D(fControlPoint2Position_x_start, fControlPoint2Position_y_start, fControlPoint2Position_z_start);
                Point3D ControlPoint_P3 = new Point3D(fControlPoint3Position_x_start, fControlPoint3Position_y_start, fControlPoint3Position_z_start);

                // Identification of current joint node location (start or end definition node of secondary member)
                if (m_Node.ID != m_SecondaryMembers[0].NodeStart.ID) // If true - joint at start node, if false joint at end node (so we need to rotate joint about z-axis 180 deg)
                {
                    // Rotate and move joint defined in the start point [0,0,0] to the end point
                    ControlPoint_P1 = new Point3D(fControlPointPosition_x_end, fControlPointPosition_y_end, fControlPointPosition_z_start);

                    ControlPoint_P2 = new Point3D(fControlPoint2Position_x_end, fControlPoint2Position_y_end, fControlPoint2Position_z_end);
                    ControlPoint_P3 = new Point3D(fControlPoint3Position_x_end, fControlPoint3Position_y_end, fControlPoint3Position_z_end);
                }

                m_arrPlates[0].ControlPoint = ControlPoint_P2;
                m_arrPlates[1].ControlPoint = ControlPoint_P3;
                m_arrPlates[2].ControlPoint = ControlPoint_P1;
            }
            else if (bUseSamePlates)
            {
                throw new NotImplementedException("TODO");
            }
            else
            {
                // Plate position in x-direction on the secondary member
                float fAlignment_x;
                fAlignment_x = m_SecondaryMembers[0].FAlignment_Start; // Posun v smere osi x LCS pruta

                if (m_Node.ID != m_SecondaryMembers[0].NodeStart.ID) // End node
                    fAlignment_x = m_SecondaryMembers[0].FAlignment_End; // Posun v smere osi x LCS pruta

                float flocaleccentricity_y = m_SecondaryMembers[0].EccentricityStart == null ? 0f : m_SecondaryMembers[0].EccentricityStart.MFy_local;
                float flocaleccentricity_z = m_SecondaryMembers[0].EccentricityStart == null ? 0f : m_SecondaryMembers[0].EccentricityStart.MFz_local;

                float fOffsetAboveColumnEnd = (float)m_MainMember.CrScStart.h / (float)Math.Cos(m_fRoofPitch_rad); // Dodatocny posun plechu smerom hore na hornu hranu rafter.

                // TODO - dopracovat co ma byt nastavitelne a co sa ma dopocitat

                float fb1_Plate1 = ((CConCom_Plate_G)m_arrPlates[0]).Fb_X1;
                float fb2_Plate1 = (float)m_SecondaryMembers[0].CrScStart.h - (float)m_MainMember.CrScStart.b;
                float fhY1_Plate = fb2_Plate1;// 0.3f;
                float fhY2_Plate = fhY1_Plate + fOffsetAboveColumnEnd;//0.8f; // Celkovy rozmer
                float flZ_Plate = ((CConCom_Plate_G)m_arrPlates[0]).Fl_Z;

                float fb_Plate2 = (float)m_SecondaryMembers[0].CrScStart.h;
                float fhY1_Plate2 = fb_Plate2; // 0.5f;
                float fhY2_Plate2 = fhY1_Plate2 + 2f * (float)m_MainMember.CrScStart.b; // 0.7f; // Celkovy rozmer

                //float fPlateOffset_y = 0.5f * (float)m_SecondaryMembers[0].CrScStart.b + (float)m_SecondaryMembers[0].CrScStart.y_min;

                // Rotation about longitudinal axis
                // Front Side of Building
                float fRotation1AboutLCS_y_deg_start = 270;
                float fRotation1AboutLCS_z_deg_start = 90;
                float fRotation1AboutLCS_y_deg_end = 270;
                float fRotation1AboutLCS_z_deg_end = 270;
                float fControlPoint1Position_x_start = -fAlignment_x + fhY2_Plate - fOffsetAboveColumnEnd;
                float fControlPoint1Position_x_end = m_SecondaryMembers[0].FLength + fAlignment_x - fhY2_Plate + fOffsetAboveColumnEnd;
                float fControlPoint1Position_y_start = flocaleccentricity_y + (float)m_SecondaryMembers[0].CrScStart.y_min;
                float fControlPoint1Position_y_end = flocaleccentricity_y + (float)m_SecondaryMembers[0].CrScStart.y_max;
                float fControlPoint1Position_z_start = (float)m_SecondaryMembers[0].CrScStart.z_min + flocaleccentricity_z + (float)m_MainMember.CrScStart.b;
                float fControlPoint1Position_z_end = (float)m_SecondaryMembers[0].CrScStart.z_min + flocaleccentricity_z + (float)m_MainMember.CrScStart.b;

                float fRotation2AboutLCS_y_deg_start = 90;
                float fRotation2AboutLCS_z_deg_start = 90;
                float fRotation2AboutLCS_y_deg_end = 90;
                float fRotation2AboutLCS_z_deg_end = 270;
                float fControlPoint2Position_x_start = -fAlignment_x + fhY1_Plate2/* + m_ft*/;
                float fControlPoint2Position_x_end = m_SecondaryMembers[0].FLength + fAlignment_x - fhY1_Plate2/* - m_ft*/;
                float fControlPoint2Position_y_start = flocaleccentricity_y + (float)m_SecondaryMembers[0].CrScStart.y_max;
                float fControlPoint2Position_y_end = flocaleccentricity_y + (float)m_SecondaryMembers[0].CrScStart.y_min;
                float fControlPoint2Position_z_start = (float)m_SecondaryMembers[0].CrScStart.z_min + flocaleccentricity_z;
                float fControlPoint2Position_z_end = (float)m_SecondaryMembers[0].CrScStart.z_min + flocaleccentricity_z;

                if (m_bSwitchConnectedSide_Z) // Back Side of Building
                {
                    fRotation1AboutLCS_y_deg_start = 90;
                    fRotation1AboutLCS_z_deg_start = 90;
                    fRotation1AboutLCS_y_deg_end = 90;
                    fRotation1AboutLCS_z_deg_end = 270;
                    fControlPoint1Position_x_start = -fAlignment_x + fhY2_Plate - fOffsetAboveColumnEnd;
                    fControlPoint1Position_x_end = m_SecondaryMembers[0].FLength + fAlignment_x - fhY2_Plate + fOffsetAboveColumnEnd;
                    fControlPoint1Position_y_start = flocaleccentricity_y + (float)m_SecondaryMembers[0].CrScStart.y_max;
                    fControlPoint1Position_y_end = flocaleccentricity_y + (float)m_SecondaryMembers[0].CrScStart.y_min;
                    fControlPoint1Position_z_start = (float)m_SecondaryMembers[0].CrScStart.z_max + flocaleccentricity_z - (float)m_MainMember.CrScStart.b;
                    fControlPoint1Position_z_end = (float)m_SecondaryMembers[0].CrScStart.z_max + flocaleccentricity_z - (float)m_MainMember.CrScStart.b;

                    fRotation2AboutLCS_y_deg_start = 270;
                    fRotation2AboutLCS_z_deg_start = 90;
                    fRotation2AboutLCS_y_deg_end = 270;
                    fRotation2AboutLCS_z_deg_end = 270;
                    fControlPoint2Position_x_start = -fAlignment_x + fhY1_Plate2/* + m_ft*/;
                    fControlPoint2Position_x_end = m_SecondaryMembers[0].FLength + fAlignment_x - fhY1_Plate2/* - m_ft*/;
                    fControlPoint2Position_y_start = flocaleccentricity_y + (float)m_SecondaryMembers[0].CrScStart.y_min;
                    fControlPoint2Position_y_end = flocaleccentricity_y + (float)m_SecondaryMembers[0].CrScStart.y_max;
                    fControlPoint2Position_z_start = (float)m_SecondaryMembers[0].CrScStart.z_max + flocaleccentricity_z;
                    fControlPoint2Position_z_end = (float)m_SecondaryMembers[0].CrScStart.z_max + flocaleccentricity_z;
                }

                // Zaporna suradnica x posuva plech pred zaciatok pruta
                Point3D ControlPoint_P1 = new Point3D(fControlPoint1Position_x_start, fControlPoint1Position_y_start, fControlPoint1Position_z_start);
                Point3D ControlPoint_P2 = new Point3D(fControlPoint2Position_x_start, fControlPoint2Position_y_start, fControlPoint2Position_z_start);

                // Identification of current joint node location (start or end definition node of secondary member)
                if (m_Node.ID != m_SecondaryMembers[0].NodeStart.ID) // If true - joint at start node, if false joint at end node (so we need to rotate joint about z-axis 180 deg)
                {
                    // Rotate and move joint defined in the start point [0,0,0] to the end point
                    ControlPoint_P1 = new Point3D(fControlPoint1Position_x_end, fControlPoint1Position_y_end, fControlPoint1Position_z_end);
                    ControlPoint_P2 = new Point3D(fControlPoint2Position_x_end, fControlPoint2Position_y_end, fControlPoint2Position_z_end);
                }

                m_arrPlates[0].ControlPoint = ControlPoint_P1;
                m_arrPlates[1].ControlPoint = ControlPoint_P2;
            }
        }
    }
}
