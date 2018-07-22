using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using BaseClasses.GraphObj;
using MATH;


namespace BaseClasses
{
    public class CConnectionJoint_T001 : CConnectionJointTypes
    {
        // Beam to Main Rafter / Main Column / Front Column / Back Column Joint
        float m_ft;
        float m_ft_main_plate;
        float m_fPlate_Angle_Leg;

        public CConnectionJoint_T001() { }

        public CConnectionJoint_T001(string sPlateType_ForL, CNode Node_temp, CMember MainMember_temp, CMember SecondaryConnectedMember_temp, float ft_temp_main_plate, EPlateNumberAndPositionInJoint ePlateNumberAndPosition, bool bIsAlignmentMainMemberWidth, bool bIsDisplayed_temp)
        {
            bIsJointDefinedinGCS = false;

            m_Node = Node_temp;
            m_MainMember = MainMember_temp;
            m_SecondaryMembers = new CMember[1];
            m_SecondaryMembers[0] = SecondaryConnectedMember_temp;
            m_ft_main_plate = ft_temp_main_plate; // Thickness of plate in knee joint of the frame (main column and rafter)
            BIsDisplayed = bIsDisplayed_temp;

            m_ft = 0.003f;
            m_fPlate_Angle_Leg = 0.05f;

            float fAlignment_x = 0.0f;

            if (m_MainMember != null)
            {
                if (bIsAlignmentMainMemberWidth) // Odsadenie moze byt suradnice v smere sirky pruta (y) alebo vysky pruta (z), pre symetricke prierezy je to polovica rozmeru ale je potrebne zapracovat obecne s ymin,ymax a zmin, zmax
                    fAlignment_x = 0.5f * (float)m_MainMember.CrScStart.b + m_ft_main_plate;
                else
                    fAlignment_x = 0.5f * (float)m_MainMember.CrScStart.h + m_ft_main_plate;
            }

            // Joint is defined in start point and LCS of secondary member [0,y,z]
            // Plates are usually defined in x,y coordinates

            float flocaleccentricity_y = m_SecondaryMembers[0].EccentricityStart == null ? 0f : m_SecondaryMembers[0].EccentricityStart.MFy_local;
            float flocaleccentricity_z = m_SecondaryMembers[0].EccentricityStart == null ? 0f : m_SecondaryMembers[0].EccentricityStart.MFz_local;

            CPoint ControlPoint_P1 = new CPoint(0, fAlignment_x, (float)(m_SecondaryMembers[0].CrScStart.y_min + flocaleccentricity_y), -0.5f * m_SecondaryMembers[0].CrScStart.h + flocaleccentricity_z, 0);
            CPoint ControlPoint_P2 = new CPoint(0, fAlignment_x, (float)(m_SecondaryMembers[0].CrScStart.y_max + flocaleccentricity_y), -0.5f * m_SecondaryMembers[0].CrScStart.h + flocaleccentricity_z, 0);

            int iConnectorNumberinOnePlate = 16; // Plates LH LI, LK

            if (sPlateType_ForL == "LJ") // TODO - tento string prepracovat na enum pre jednotlive typy plechov, pripravit databazu plechov
                iConnectorNumberinOnePlate = 8; // Plate LJ

            float fDiameter_temp = 0.0055f; // Default - same size as screw
            float fScrewLength = 0.009f;
            //float fScrewWeight = 0.012f;

            // TODO Ondrej 15/07/2018
            // Tu sa pridavaju plechy (plates) do spoja (joint), vklada sa do pozicie v LCS pruta
            // Spoj sa vklada na zaciatok alebo na koniec pruta (pootocenie okolo "z" 0 alebo 180)
            // Uvedena rotacia plechu znamena ako sa ma plech otocit zo systemu v ktorom je definovany do systemu v LCS pruta
            // Kedze v tomto pripade je jeden plech v spoji zlava a druhy zprava ma kazdy plech vlastny CP a pootocenie je okolo "z" je pre jeden 0 a pre druhy +90
            // Spolu s plechom by sa pri tento stransformacii mali pootocit aj skrutky priradene k plechu (vid m_arrPlateConnectors)

            // Update 2
            // Po tomto vlozeni plechov a ich skrutiek do spoja by sa mali suradnice vsetkych plechov a skrutiek v spoji prepocitat z povodnych suradnic plechov, v ktorych su plechy zadane do suradnicoveho systemu spoja a ulozit

            CConCom_Plate_F_or_L pLeftPlate = new CConCom_Plate_F_or_L(sPlateType_ForL, ControlPoint_P1, m_fPlate_Angle_Leg, (float)m_SecondaryMembers[0].CrScStart.h, m_fPlate_Angle_Leg, m_ft, 90, 0, 0, iConnectorNumberinOnePlate, fDiameter_temp, fScrewLength, BIsDisplayed); // Rotation angle in degrees
            CConCom_Plate_F_or_L pRightPlate = new CConCom_Plate_F_or_L(sPlateType_ForL, ControlPoint_P2, m_fPlate_Angle_Leg, (float)m_SecondaryMembers[0].CrScStart.h, m_fPlate_Angle_Leg, m_ft, 90, 0, 90, iConnectorNumberinOnePlate, fDiameter_temp, fScrewLength, BIsDisplayed); // Rotation angle in degrees

            // Identification of current joint node location (start or end definition node of secondary member)
            if (m_Node.ID != m_SecondaryMembers[0].NodeStart.ID) // If true - joint at start node, if false joint at end node (so we need to rotate joint about z-axis 180 deg)
            {
                // Rotate and move joint defined in the start point [0,0,0] to the end point
                ControlPoint_P1 = new CPoint(0, m_SecondaryMembers[0].FLength - fAlignment_x, (float)(m_SecondaryMembers[0].CrScStart.y_max + flocaleccentricity_y), -0.5f * m_SecondaryMembers[0].CrScStart.h + flocaleccentricity_z, 0);
                ControlPoint_P2 = new CPoint(0, m_SecondaryMembers[0].FLength - fAlignment_x, (float)(m_SecondaryMembers[0].CrScStart.y_min + flocaleccentricity_y), -0.5f * m_SecondaryMembers[0].CrScStart.h + flocaleccentricity_z, 0);

                pLeftPlate = new CConCom_Plate_F_or_L(sPlateType_ForL, ControlPoint_P1, m_fPlate_Angle_Leg, (float)m_SecondaryMembers[0].CrScStart.h, m_fPlate_Angle_Leg, m_ft, 90, 0, 180 + 0, iConnectorNumberinOnePlate, fDiameter_temp, fScrewLength, BIsDisplayed); // Rotation angle in degrees
                pRightPlate = new CConCom_Plate_F_or_L(sPlateType_ForL, ControlPoint_P2, m_fPlate_Angle_Leg, (float)m_SecondaryMembers[0].CrScStart.h, m_fPlate_Angle_Leg, m_ft, 90, 0, 180 + 90, iConnectorNumberinOnePlate, fDiameter_temp, fScrewLength, BIsDisplayed); // Rotation angle in degrees
            }

            if (ePlateNumberAndPosition == EPlateNumberAndPositionInJoint.eTwoPlates)
            {
                m_arrPlates = new CPlate[2];
                m_arrPlates[0] = pLeftPlate;
                m_arrPlates[1] = pRightPlate;
            }
            else
            {
                m_arrPlates = new CPlate[1]; //  Just one plate in joint

                if (ePlateNumberAndPosition == EPlateNumberAndPositionInJoint.eOneLeftPlate)
                {
                    m_arrPlates[0] = pLeftPlate;

                    if (m_Node.ID != m_SecondaryMembers[0].NodeStart.ID)
                        m_arrPlates[0] = pRightPlate;
                }
                else
                {
                    m_arrPlates[0] = pRightPlate;

                    if (m_Node.ID != m_SecondaryMembers[0].NodeStart.ID)
                        m_arrPlates[0] = pLeftPlate;
                }
            }
        }
    }
}
