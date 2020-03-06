using System;
using System.Windows;
using System.Collections.Generic;
using DATABASE;
using DATABASE.DTO;
using BaseClasses.GraphObj;
using System.Windows.Media.Media3D;
using BaseClasses.Helpers;

namespace BaseClasses
{
    public class CConnectionJoint_T002 : CConnectionJointTypes
    {
        // Purlin to Rafter/Main Column Joint
        public float m_ft;
        public float m_ft_main_plate;
        public float m_fPlate_Angle_Leg;

        public CConnectionJoint_T002() { }

        public CConnectionJoint_T002(CNode Node_temp, CMember MainFrameColumn_temp, CMember EavesPurlin_temp, float ft_temp_main_plate)
        {
            bIsJointDefinedinGCS = false;

            m_Node = Node_temp;
            m_pControlPoint = m_Node.GetPoint3D();
            m_MainMember = MainFrameColumn_temp;
            m_SecondaryMembers = new CMember[1];
            m_SecondaryMembers[0] = EavesPurlin_temp;
            m_ft_main_plate = ft_temp_main_plate; // Thickness of plate in knee joint of the frame (main column and rafter)

            m_ft = 0.001f; // Plate serie LL ???
            m_fPlate_Angle_Leg = 0.05f;

            // Joint is defined in start point and LCS of secondary member [0,y,z]
            // Plates are usually defined in x,y coordinates

            // TODO Ondrej 15/07/2018
            // Tu sa pridava plech (plate) do spoja (joint), vklada sa do pozicie v LCS pruta
            // Spoj sa vklada na zaciatok alebo na koniec pruta (pootocenie okolo "z" 0 alebo 180)
            // Kedze v tomto pripade je tu jeden plech, ktory je definovany tak ze osa x, v ktorej je plech zadany zviera s osou x pruta uhol 90 stupnov, pootocenie je okolo "z" +90
            // Uvedena rotacia plechu znamena ako sa ma plech otocit zo systemu v ktorom je definovany do systemu v LCS pruta
            // Spolu s plechom by sa pri tento stransformacii mali pootocit aj skrutky priradene k plechu (vid m_arrPlateConnectors)

            // Update 2
            // Po tomto vlozeni plechov a ich skrutiek do spoja by sa mali suradnice vsetkych plechov a skrutiek v spoji prepocitat z povodnych suradnic plechov, v ktorych su plechy zadane do suradnicoveho systemu spoja a ulozit

            float fCutOffOneSide = 0.005f;
            float fAlignment_x = 0;

            if (m_SecondaryMembers[0] != null)
                fAlignment_x = -m_SecondaryMembers[0].FAlignment_Start + m_ft_main_plate - fCutOffOneSide;

            float flocaleccentricity_y = m_SecondaryMembers[0].EccentricityStart == null ? 0f : m_SecondaryMembers[0].EccentricityStart.MFy_local;
            float flocaleccentricity_z = m_SecondaryMembers[0].EccentricityStart == null ? 0f : m_SecondaryMembers[0].EccentricityStart.MFz_local;

            Point3D ControlPoint_P1 = new Point3D(fAlignment_x, (float)(m_SecondaryMembers[0].CrScStart.y_min - m_fPlate_Angle_Leg + flocaleccentricity_y), m_SecondaryMembers[0].CrScStart.z_min /*- 0.5f * m_SecondaryMembers[0].CrScStart.h*/ - m_ft + flocaleccentricity_z);

            int iConnectorNumberinOnePlate = 32;
            CScrew referenceScrew = new CScrew("TEK", "14");
            CScrewArrangement_LL screwArrangement = new CScrewArrangement_LL(iConnectorNumberinOnePlate, referenceScrew);

            m_arrPlates = new CPlate[1];
            m_arrPlates[0] = new CConCom_Plate_LL("LLH", ControlPoint_P1, m_fPlate_Angle_Leg, (float)m_SecondaryMembers[0].CrScStart.b, (float)m_SecondaryMembers[0].CrScStart.h, m_fPlate_Angle_Leg, m_ft, 90, 0, 90, screwArrangement); // Rotation angle in degrees

            // Identification of current joint node location (start or end definition node of secondary member)
            if (m_Node.ID != m_SecondaryMembers[0].NodeStart.ID) // If true - joint at start node, if false joint at end node (se we need to rotate joint about z-axis 180 deg)
            {
                if (m_SecondaryMembers[0] != null)
                    fAlignment_x = -m_SecondaryMembers[0].FAlignment_End + m_ft_main_plate - fCutOffOneSide;

                // Rotate and move joint defined in the start point [0,0,0] to the end point
                ControlPoint_P1 = new Point3D(m_SecondaryMembers[0].FLength - fAlignment_x, (float)(m_SecondaryMembers[0].CrScStart.y_max + m_fPlate_Angle_Leg + flocaleccentricity_y), m_SecondaryMembers[0].CrScStart.z_min /* -0.5f * m_SecondaryMembers[0].CrScStart.h*/ - m_ft + flocaleccentricity_z);

                m_arrPlates[0] = new CConCom_Plate_LL("LLH", ControlPoint_P1, m_fPlate_Angle_Leg, (float)m_SecondaryMembers[0].CrScStart.b, (float)m_SecondaryMembers[0].CrScStart.h, m_fPlate_Angle_Leg, m_ft, 90, 0, 180 + 90, screwArrangement); // Rotation angle in degrees
            }
        }

        public override CConnectionJointTypes RecreateJoint()
        {
            return new CConnectionJoint_T002(m_Node, m_MainMember, m_SecondaryMembers[0], m_ft_main_plate);
        }
    }
}
