using System;
using System.Windows;
using System.Collections.Generic;
using System.Globalization;
using MATH;
using DATABASE;
using DATABASE.DTO;
using BaseClasses.GraphObj;

namespace BaseClasses
{
    public class CConnectionJoint_S001 : CConnectionJointTypes
    {
        // Column to Main Rafter Joint
        float m_ft;

        public CConnectionJoint_S001() { }

        public CConnectionJoint_S001(CNode Node_temp, CMember MainMember_temp, CMember SecondaryConnectedMember_temp, bool bIsDisplayed_temp)
        {
            bIsJointDefinedinGCS = false;

            m_Node = Node_temp;
            m_MainMember = MainMember_temp;
            m_SecondaryMembers = new CMember[1];
            m_SecondaryMembers[0] = SecondaryConnectedMember_temp;
            BIsDisplayed = bIsDisplayed_temp;

            m_ft = 0.002f;

            // Joint is defined in start point and LCS of secondary member [0,y,z]
            // Plates are usually defined in x,y coordinates

            // Plate position in x-direction on the secondary member
            float fAlignment_x = 0.1f; // Posun plechu v smere osi x LCS pruta (kladna hodnota je posun v smere +x)
            // Distance between plates
            float fPlateCenterDistanceInx = 0.2f;

            float flocaleccentricity_y = m_SecondaryMembers[0].EccentricityStart == null ? 0f : m_SecondaryMembers[0].EccentricityStart.MFy_local;
            float flocaleccentricity_z = m_SecondaryMembers[0].EccentricityStart == null ? 0f : m_SecondaryMembers[0].EccentricityStart.MFz_local;

            float alpha2_rad = MathF.fPI / 8f; // 22.5 deg
            float fbX1 = 0.1f;
            float fbX3 = (float)m_SecondaryMembers[0].CrScStart.b;
            float fhY = 0.1f;
            float x_a = (float)Math.Tan(alpha2_rad) * (float)m_SecondaryMembers[0].CrScStart.h;

            CPoint ControlPoint_P1 = new CPoint(0, fAlignment_x, -fbX1 - x_a + (float)(m_SecondaryMembers[0].CrScStart.y_min + flocaleccentricity_y), -0.5f * m_SecondaryMembers[0].CrScStart.h + flocaleccentricity_z, 0);
            CPoint ControlPoint_P2 = new CPoint(0, fAlignment_x + fPlateCenterDistanceInx, -fbX1 - x_a + (float)(m_SecondaryMembers[0].CrScStart.y_min + flocaleccentricity_y), -0.5f * m_SecondaryMembers[0].CrScStart.h + flocaleccentricity_z, 0);

            int iConnectorNumberinOnePlate = 12;

            CScrew referenceScrew = new CScrew("TEK", "12");
            CScrewArrangement_N screwArrangement = new CScrewArrangement_N(iConnectorNumberinOnePlate, referenceScrew);

            CConCom_Plate_N pPlate1 = new CConCom_Plate_N("N", ControlPoint_P1, fbX1, (float)m_SecondaryMembers[0].CrScStart.b, fhY, (float)m_SecondaryMembers[0].CrScStart.h, m_ft, 0, 0, 90, screwArrangement, BIsDisplayed); // Rotation angle in degrees
            CConCom_Plate_N pPlate2 = new CConCom_Plate_N("N", ControlPoint_P2, fbX1, (float)m_SecondaryMembers[0].CrScStart.b, fhY, (float)m_SecondaryMembers[0].CrScStart.h, m_ft, 0, 0, 90, screwArrangement, BIsDisplayed); // Rotation angle in degrees

            // Identification of current joint node location (start or end definition node of secondary member)
            if (m_Node.ID != m_SecondaryMembers[0].NodeStart.ID) // If true - joint at start node, if false joint at end node (so we need to rotate joint about z-axis 180 deg)
            {
                // Rotate and move joint defined in the start point [0,0,0] to the end point
                ControlPoint_P1 = new CPoint(0, m_SecondaryMembers[0].FLength - fAlignment_x, fbX1 + x_a + (float)(m_SecondaryMembers[0].CrScStart.y_max + flocaleccentricity_y), -0.5f * m_SecondaryMembers[0].CrScStart.h + flocaleccentricity_z, 0);
                ControlPoint_P2 = new CPoint(0, m_SecondaryMembers[0].FLength - fAlignment_x - fPlateCenterDistanceInx, fbX1 + x_a + (float)(m_SecondaryMembers[0].CrScStart.y_max + flocaleccentricity_y), -0.5f * m_SecondaryMembers[0].CrScStart.h + flocaleccentricity_z, 0);

                pPlate1 = new CConCom_Plate_N("N", ControlPoint_P1, fbX1, fbX3, fhY, (float)m_SecondaryMembers[0].CrScStart.h, m_ft, 0, 0, 180 + 90, screwArrangement, BIsDisplayed); // Rotation angle in degrees
                pPlate2 = new CConCom_Plate_N("N", ControlPoint_P2, fbX1, fbX3, fhY, (float)m_SecondaryMembers[0].CrScStart.h, m_ft, 0, 0, 180 + 90, screwArrangement, BIsDisplayed); // Rotation angle in degrees
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
    }
}
