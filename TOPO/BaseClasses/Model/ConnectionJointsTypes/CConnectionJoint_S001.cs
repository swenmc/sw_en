using System;
using System.Windows;
using System.Collections.Generic;
using DATABASE;
using DATABASE.DTO;
using BaseClasses.GraphObj;
using System.Globalization;

namespace BaseClasses
{
    public class CConnectionJoint_S001 : CConnectionJointTypes
    {
        // Column to Main Rafter Joint
        float m_ft;

        public CConnectionJoint_S001() { }

        public CConnectionJoint_S001(CNode Node_temp, CMember MainMember_temp, CMember SecondaryConnectedMember_temp, bool bIsAlignmentMainMemberWidth, bool bIsDisplayed_temp)
        {
            bIsJointDefinedinGCS = false;

            m_Node = Node_temp;
            m_MainMember = MainMember_temp;
            m_SecondaryMembers = new CMember[1];
            m_SecondaryMembers[0] = SecondaryConnectedMember_temp;
            BIsDisplayed = bIsDisplayed_temp;

            m_ft = 0.002f;

            float fAlignment_x = 0.0f;

            if (m_MainMember != null)
            {
                if (bIsAlignmentMainMemberWidth) // Odsadenie moze byt suradnice v smere sirky pruta (y) alebo vysky pruta (z), pre symetricke prierezy je to polovica rozmeru ale je potrebne zapracovat obecne s ymin,ymax a zmin, zmax
                    fAlignment_x = 0.5f * (float)m_MainMember.CrScStart.b;
                else
                    fAlignment_x = 0.5f * (float)m_MainMember.CrScStart.h;
            }

            // Joint is defined in start point and LCS of secondary member [0,y,z]
            // Plates are usually defined in x,y coordinates

            float flocaleccentricity_y = m_SecondaryMembers[0].EccentricityStart == null ? 0f : m_SecondaryMembers[0].EccentricityStart.MFy_local;
            float flocaleccentricity_z = m_SecondaryMembers[0].EccentricityStart == null ? 0f : m_SecondaryMembers[0].EccentricityStart.MFz_local;

            CPoint ControlPoint_P1 = new CPoint(0, fAlignment_x, (float)(m_SecondaryMembers[0].CrScStart.y_min + flocaleccentricity_y), -0.5f * m_SecondaryMembers[0].CrScStart.h + flocaleccentricity_z, 0);

            int iConnectorNumberinOnePlate = 12;

            CScrew referenceScrew = new CScrew("TEK", "12");
            CScrewArrangement_N screwArrangement = new CScrewArrangement_N(iConnectorNumberinOnePlate, referenceScrew);

            CConCom_Plate_N pPlate = new CConCom_Plate_N("N", ControlPoint_P1, 0.1f, (float)m_SecondaryMembers[0].CrScStart.b, 0.1f, (float)m_SecondaryMembers[0].CrScStart.h, m_ft, 90, 0, 0, screwArrangement, BIsDisplayed); // Rotation angle in degrees

            // Identification of current joint node location (start or end definition node of secondary member)
            if (m_Node.ID != m_SecondaryMembers[0].NodeStart.ID) // If true - joint at start node, if false joint at end node (so we need to rotate joint about z-axis 180 deg)
            {
                // Rotate and move joint defined in the start point [0,0,0] to the end point
                ControlPoint_P1 = new CPoint(0, m_SecondaryMembers[0].FLength - fAlignment_x, (float)(m_SecondaryMembers[0].CrScStart.y_max + flocaleccentricity_y), -0.5f * m_SecondaryMembers[0].CrScStart.h + flocaleccentricity_z, 0);

                pPlate = new CConCom_Plate_N("N", ControlPoint_P1, 0.1f, (float)m_SecondaryMembers[0].CrScStart.b, 0.1f, (float)m_SecondaryMembers[0].CrScStart.h, m_ft, 90, 0, 180 + 0, screwArrangement, BIsDisplayed); // Rotation angle in degrees

            }

            /*
            if (ePlateNumber == EPlateNumberAndPositionInJoint.eTwoPlates)
            {
                m_arrPlates = new CPlate[2];
                m_arrPlates[0] = pLeftPlate;
                m_arrPlates[1] = pRightPlate;
            }
            else
            {
                m_arrPlates = new CPlate[1]; //  Just one plate in joint

                m_arrPlates[0] = pLeftPlate;
            }
            */
        }
    }
}
