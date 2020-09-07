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
    public class CConnectionJoint_T001 : CConnectionJointTypes
    {
        // Beam to Main Rafter / Main Column / Front Wind Post / Back Wind Post Joint
        public float m_ft;
        public float m_ft_main_plate;
        public float m_fPlate_Angle_Leg;
        public string m_sPlateType_ForL;
        public EPlateNumberAndPositionInJoint m_ePlateNumberAndPosition;

        public CConnectionJoint_T001() { }

        public CConnectionJoint_T001(string sPlateType_ForL, CNode Node_temp, CMember MainMember_temp, CMember SecondaryConnectedMember_temp, float ft_temp_main_plate, EPlateNumberAndPositionInJoint ePlateNumberAndPosition)
        {
            bIsJointDefinedinGCS = false;

            m_sPlateType_ForL = sPlateType_ForL;
            m_ePlateNumberAndPosition = ePlateNumberAndPosition;
            m_Node = Node_temp;
            m_pControlPoint = m_Node.GetPoint3D();
            m_MainMember = MainMember_temp;
            m_SecondaryMembers = new CMember[1];
            m_SecondaryMembers[0] = SecondaryConnectedMember_temp;
            m_ft_main_plate = ft_temp_main_plate; // Thickness of plate in knee joint of the frame (main column and rafter)

            m_ft = 0.001f; // Plate serie L
            m_fPlate_Angle_Leg = 0.05f;
            float m_fPlate_Angle_Height = (float)m_SecondaryMembers[0].CrScStart.h;

            //--------------------------------------------------------------------------------------------------
            // Prepisem default hodnotami z databazy
            string sPlatePrefix;
            CPlate_L_Properties plateProp;
            SetPlate_L_Type(m_SecondaryMembers[0].CrScStart.Name_short, out sPlatePrefix, out plateProp);

            if (plateProp != null)
            {
                m_sPlateType_ForL = sPlatePrefix;
                m_ft = (float)plateProp.thickness;
                m_fPlate_Angle_Leg = (float)plateProp.dim1;
                //m_fPlate_Angle_LeftLeg = (float)plateProp.dim1;
                //m_fPlate_Angle_RightLeg = (float)plateProp.dim3;
                m_fPlate_Angle_Height = (float)plateProp.dim2y;
            }
            //--------------------------------------------------------------------------------------------------

            float fCutOffOneSide = 0.005f;
            float fAlignment_x = 0;

            if (m_SecondaryMembers[0] != null)
                fAlignment_x = -m_SecondaryMembers[0].FAlignment_Start + m_ft_main_plate - fCutOffOneSide;

            // Joint is defined in start point and LCS of secondary member [0,y,z]
            // Plates are usually defined in x,y coordinates

            float flocaleccentricity_y = m_SecondaryMembers[0].EccentricityStart == null ? 0f : m_SecondaryMembers[0].EccentricityStart.MFy_local;
            float flocaleccentricity_z = m_SecondaryMembers[0].EccentricityStart == null ? 0f : m_SecondaryMembers[0].EccentricityStart.MFz_local;

            Point3D ControlPoint_P1 = new Point3D(fAlignment_x, (float)(m_SecondaryMembers[0].CrScStart.y_min + flocaleccentricity_y), (float)m_SecondaryMembers[0].CrScStart.z_min /*-0.5f * m_SecondaryMembers[0].CrScStart.h*/ + flocaleccentricity_z);
            Point3D ControlPoint_P2 = new Point3D(fAlignment_x, (float)(m_SecondaryMembers[0].CrScStart.y_max + flocaleccentricity_y), (float)m_SecondaryMembers[0].CrScStart.z_min /*-0.5f * m_SecondaryMembers[0].CrScStart.h*/ + flocaleccentricity_z);
            Vector3D RotationVector_P1 = new Vector3D(90, 0, 0);
            Vector3D RotationVector_P2 = new Vector3D(90, 0, 90);

            int iConnectorNumberinOnePlate = 16; // Plates LH, LI, LK

            if (sPlateType_ForL == "LJ") // TODO - tento string prepracovat na enum pre jednotlive typy plechov, pripravit databazu plechov
                iConnectorNumberinOnePlate = 8; // Plate LJ

            CScrew referenceScrew = new CScrew("TEK", "14");
            CScrewArrangement_L screwArrangement = new CScrewArrangement_L(iConnectorNumberinOnePlate, referenceScrew, 0.010f, 0.010f, 0.030f, 0.090f, m_fPlate_Angle_Height, m_fPlate_Angle_Height, 0f, 0f);

            // TODO Ondrej 15/07/2018
            // Tu sa pridavaju plechy (plates) do spoja (joint), vklada sa do pozicie v LCS pruta
            // Spoj sa vklada na zaciatok alebo na koniec pruta (pootocenie okolo "z" 0 alebo 180)
            // Uvedena rotacia plechu znamena ako sa ma plech otocit zo systemu v ktorom je definovany do systemu v LCS pruta
            // Kedze v tomto pripade je jeden plech v spoji zlava a druhy zprava ma kazdy plech vlastny CP a pootocenie je okolo "z" je pre jeden 0 a pre druhy +90
            // Spolu s plechom by sa pri tento stransformacii mali pootocit aj skrutky priradene k plechu (vid m_arrPlateConnectors)

            // Update 2
            // Po tomto vlozeni plechov a ich skrutiek do spoja by sa mali suradnice vsetkych plechov a skrutiek v spoji prepocitat z povodnych suradnic plechov, v ktorych su plechy zadane do suradnicoveho systemu spoja a ulozit

            CConCom_Plate_F_or_L pLeftPlate = new CConCom_Plate_F_or_L(sPlateType_ForL, ControlPoint_P1, m_fPlate_Angle_Leg, m_fPlate_Angle_Height, m_fPlate_Angle_Leg, m_ft, (float)m_SecondaryMembers[0].CrScStart.h, (float)RotationVector_P1.X, (float)RotationVector_P1.Y, (float)RotationVector_P1.Z, screwArrangement); // Rotation angle in degrees
            CConCom_Plate_F_or_L pRightPlate = new CConCom_Plate_F_or_L(sPlateType_ForL, ControlPoint_P2, m_fPlate_Angle_Leg, m_fPlate_Angle_Height, m_fPlate_Angle_Leg, m_ft, (float)m_SecondaryMembers[0].CrScStart.h, (float)RotationVector_P2.X, (float)RotationVector_P2.Y, (float)RotationVector_P2.Z, screwArrangement); // Rotation angle in degrees

            // Identification of current joint node location (start or end definition node of secondary member)
            if (m_Node.ID != m_SecondaryMembers[0].NodeStart.ID) // If true - joint at start node, if false joint at end node (so we need to rotate joint about z-axis 180 deg)
            {
                if (m_SecondaryMembers[0] != null)
                    fAlignment_x = -m_SecondaryMembers[0].FAlignment_End + m_ft_main_plate - fCutOffOneSide;

                // Rotate and move joint defined in the start point [0,0,0] to the end point
                ControlPoint_P1 = new Point3D(m_SecondaryMembers[0].FLength - fAlignment_x, (float)(m_SecondaryMembers[0].CrScStart.y_max + flocaleccentricity_y), (float)m_SecondaryMembers[0].CrScStart.z_min /*- 0.5f * m_SecondaryMembers[0].CrScStart.h*/ + flocaleccentricity_z);
                ControlPoint_P2 = new Point3D(m_SecondaryMembers[0].FLength - fAlignment_x, (float)(m_SecondaryMembers[0].CrScStart.y_min + flocaleccentricity_y), (float)m_SecondaryMembers[0].CrScStart.z_min /*- 0.5f * m_SecondaryMembers[0].CrScStart.h*/ + flocaleccentricity_z);
                RotationVector_P1 = new Vector3D(90, 0, 180 + 0);
                RotationVector_P2 = new Vector3D(90, 0, 180 + 90);

                pLeftPlate = new CConCom_Plate_F_or_L(sPlateType_ForL, ControlPoint_P1, m_fPlate_Angle_Leg, m_fPlate_Angle_Height, m_fPlate_Angle_Leg, m_ft, (float)m_SecondaryMembers[0].CrScStart.h, (float)RotationVector_P1.X, (float)RotationVector_P1.Y, (float)RotationVector_P1.Z, screwArrangement); // Rotation angle in degrees
                pRightPlate = new CConCom_Plate_F_or_L(sPlateType_ForL, ControlPoint_P2, m_fPlate_Angle_Leg, m_fPlate_Angle_Height, m_fPlate_Angle_Leg, m_ft, (float)m_SecondaryMembers[0].CrScStart.h, (float)RotationVector_P2.X, (float)RotationVector_P2.Y, (float)RotationVector_P2.Z, screwArrangement); // Rotation angle in degrees
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

        public override CConnectionJointTypes RecreateJoint()
        {
            return new CConnectionJoint_T001(m_sPlateType_ForL, m_Node, m_MainMember, m_SecondaryMembers[0], m_ft_main_plate, m_ePlateNumberAndPosition);
        }

        public override void UpdateJoint()
        {
            float fCutOffOneSide = 0.005f;
            float fAlignment_x = 0;

            if (m_SecondaryMembers[0] != null)
                fAlignment_x = -m_SecondaryMembers[0].FAlignment_Start + m_ft_main_plate - fCutOffOneSide;

            // Joint is defined in start point and LCS of secondary member [0,y,z]
            // Plates are usually defined in x,y coordinates

            float flocaleccentricity_y = m_SecondaryMembers[0].EccentricityStart == null ? 0f : m_SecondaryMembers[0].EccentricityStart.MFy_local;
            float flocaleccentricity_z = m_SecondaryMembers[0].EccentricityStart == null ? 0f : m_SecondaryMembers[0].EccentricityStart.MFz_local;

            Point3D ControlPoint_P1 = new Point3D(fAlignment_x, (float)(m_SecondaryMembers[0].CrScStart.y_min + flocaleccentricity_y), (float)m_SecondaryMembers[0].CrScStart.z_min /*-0.5f * m_SecondaryMembers[0].CrScStart.h*/ + flocaleccentricity_z);
            Point3D ControlPoint_P2 = new Point3D(fAlignment_x, (float)(m_SecondaryMembers[0].CrScStart.y_max + flocaleccentricity_y), (float)m_SecondaryMembers[0].CrScStart.z_min /*-0.5f * m_SecondaryMembers[0].CrScStart.h*/ + flocaleccentricity_z);
            Vector3D RotationVector_P1 = new Vector3D(90, 0, 0);
            Vector3D RotationVector_P2 = new Vector3D(90, 0, 90);

            if (m_Node.ID != m_SecondaryMembers[0].NodeStart.ID) // If true - joint at start node, if false joint at end node (so we need to rotate joint about z-axis 180 deg)
            {
                if (m_SecondaryMembers[0] != null)
                    fAlignment_x = -m_SecondaryMembers[0].FAlignment_End + m_ft_main_plate - fCutOffOneSide;

                // Rotate and move joint defined in the start point [0,0,0] to the end point
                ControlPoint_P1 = new Point3D(m_SecondaryMembers[0].FLength - fAlignment_x, (float)(m_SecondaryMembers[0].CrScStart.y_max + flocaleccentricity_y), (float)m_SecondaryMembers[0].CrScStart.z_min /*- 0.5f * m_SecondaryMembers[0].CrScStart.h*/ + flocaleccentricity_z);
                ControlPoint_P2 = new Point3D(m_SecondaryMembers[0].FLength - fAlignment_x, (float)(m_SecondaryMembers[0].CrScStart.y_min + flocaleccentricity_y), (float)m_SecondaryMembers[0].CrScStart.z_min /*- 0.5f * m_SecondaryMembers[0].CrScStart.h*/ + flocaleccentricity_z);
                RotationVector_P1 = new Vector3D(90, 0, 180 + 0);
                RotationVector_P2 = new Vector3D(90, 0, 180 + 90);
            }

            if (m_ePlateNumberAndPosition == EPlateNumberAndPositionInJoint.eTwoPlates)
            {
                m_arrPlates[0].m_pControlPoint = ControlPoint_P1;
                m_arrPlates[1].m_pControlPoint = ControlPoint_P2;

                m_arrPlates[0].SetPlateRotation(RotationVector_P1);
                m_arrPlates[1].SetPlateRotation(RotationVector_P2);
            }
            else
            {
                //  Just one plate in joint

                if (m_ePlateNumberAndPosition == EPlateNumberAndPositionInJoint.eOneLeftPlate)
                {
                    m_arrPlates[0].m_pControlPoint = ControlPoint_P1;
                    m_arrPlates[0].SetPlateRotation(RotationVector_P1);

                    if (m_Node.ID != m_SecondaryMembers[0].NodeStart.ID)
                    {
                        m_arrPlates[0].m_pControlPoint = ControlPoint_P2;
                        m_arrPlates[0].SetPlateRotation(RotationVector_P2);
                    }
                }
                else
                {
                    m_arrPlates[0].m_pControlPoint = ControlPoint_P2;
                    m_arrPlates[0].SetPlateRotation(RotationVector_P2);

                    if (m_Node.ID != m_SecondaryMembers[0].NodeStart.ID)
                    {
                        m_arrPlates[0].m_pControlPoint = ControlPoint_P1;
                        m_arrPlates[0].SetPlateRotation(RotationVector_P1);
                    }
                }
            }
        }
    }
}
