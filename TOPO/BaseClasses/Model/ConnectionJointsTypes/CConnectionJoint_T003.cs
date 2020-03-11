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
    public class CConnectionJoint_T003 : CConnectionJointTypes
    {
        // Beam / Purlin to Main Rafter Joint - use Fly bracing plates "F"
        public float m_ft;
        public float m_ft_main_plate;
        public float m_fPlate_Angle_LeftLeg;
        public float m_fPlate_Angle_RightLeg_BX1;
        public float m_fPlate_Angle_RightLeg_BX2;
        public float m_fPlate_Angle_Height;
        public string m_sPlateType_F_left;
        public string m_sPlateType_F_right;
        public EPlateNumberAndPositionInJoint m_ePlateNumberAndPosition;
        public bool m_bTopOfPlateInCrscVerticalAxisPlusDirection;

        public CConnectionJoint_T003() { }

        public CConnectionJoint_T003(string sPlateType_F_left, string sPlateType_F_right, CNode Node_temp, CMember MainMember_temp, CMember SecondaryConnectedMember_temp, float ft_temp_main_plate, EPlateNumberAndPositionInJoint ePlateNumberAndPosition, bool bTopOfPlateInCrscVerticalAxisPlusDirection)
        {
            bIsJointDefinedinGCS = false;

            m_sPlateType_F_left = sPlateType_F_left;
            m_sPlateType_F_right = sPlateType_F_right;
            m_ePlateNumberAndPosition = ePlateNumberAndPosition;
            m_Node = Node_temp;
            m_pControlPoint = m_Node.GetPoint3D();
            m_MainMember = MainMember_temp;
            m_SecondaryMembers = new CMember[1];
            m_SecondaryMembers[0] = SecondaryConnectedMember_temp;
            m_ft_main_plate = ft_temp_main_plate; // Thickness of plate in knee joint of the frame (main column and rafter)
            m_bTopOfPlateInCrscVerticalAxisPlusDirection = bTopOfPlateInCrscVerticalAxisPlusDirection;

            m_ft = 0.002f; // Plate serie F
            m_fPlate_Angle_LeftLeg = 0.05f;
            m_fPlate_Angle_RightLeg_BX1 = 0.035f;
            m_fPlate_Angle_RightLeg_BX2 = 0.120f;
            m_fPlate_Angle_Height = 0.630f;

            if (MainMember_temp != null && MainMember_temp.CrScStart != null)
                m_fPlate_Angle_Height = (float)MainMember_temp.CrScStart.h;

            //--------------------------------------------------------------------------------------------------
            // Prepisem default hodnotami z databazy
            string sPlatePrefix;
            CPlate_F_Properties plateProp;
            SetPlate_F_Type(m_MainMember.CrScStart.Name_short, out sPlatePrefix, out plateProp);

            if (plateProp != null)
            {
                sPlateType_F_left = sPlatePrefix + " - LH";
                sPlateType_F_right = sPlatePrefix + " - RH";
                m_ft = (float)plateProp.thickness;
                m_fPlate_Angle_LeftLeg = (float)plateProp.dim3;
                m_fPlate_Angle_RightLeg_BX1 = (float)plateProp.dim11;
                m_fPlate_Angle_RightLeg_BX2 = (float)plateProp.dim12;
                m_fPlate_Angle_Height = (float)plateProp.dim2y;
            }
            //--------------------------------------------------------------------------------------------------

            float fCutOffOneSide = 0.005f;
            float fAlignment_x = 0.0f;

            if (m_SecondaryMembers[0] != null)
                fAlignment_x = -m_SecondaryMembers[0].FAlignment_Start + m_ft_main_plate - fCutOffOneSide;

            // Joint is defined in start point and LCS of secondary member [0,y,z]
            // Plates are usually defined in x,y coordinates

            float flocaleccentricity_y = m_SecondaryMembers[0].EccentricityStart == null ? 0f : m_SecondaryMembers[0].EccentricityStart.MFy_local;
            float flocaleccentricity_z = m_SecondaryMembers[0].EccentricityStart == null ? 0f : m_SecondaryMembers[0].EccentricityStart.MFz_local;

            int iConnectorNumberinOnePlate = 14;

            CScrew referenceScrew = new CScrew("TEK", "14");
            CScrewArrangement_F screwArrangement1 = new CScrewArrangement_F(iConnectorNumberinOnePlate, referenceScrew);
            CScrewArrangement_F screwArrangement2 = new CScrewArrangement_F(iConnectorNumberinOnePlate, referenceScrew);

            Point3D ControlPoint_P1 = new Point3D(fAlignment_x, (float)(m_SecondaryMembers[0].CrScStart.y_min + flocaleccentricity_y), -m_fPlate_Angle_Height + m_SecondaryMembers[0].CrScStart.z_max + flocaleccentricity_z);
            Point3D ControlPoint_P2 = new Point3D(fAlignment_x, (float)(m_SecondaryMembers[0].CrScStart.y_max + flocaleccentricity_y), -m_fPlate_Angle_Height + m_SecondaryMembers[0].CrScStart.z_max + flocaleccentricity_z);
            Vector3D RotationVector_P1 = new Vector3D(90, 0, 0);
            Vector3D RotationVector_P2 = new Vector3D(90, 0, 180);

            if (!m_bTopOfPlateInCrscVerticalAxisPlusDirection)
            {
                ControlPoint_P1 = new Point3D(fAlignment_x, (float)(m_SecondaryMembers[0].CrScStart.y_max + flocaleccentricity_y), m_fPlate_Angle_Height + m_SecondaryMembers[0].CrScStart.z_min + flocaleccentricity_z);
                ControlPoint_P2 = new Point3D(fAlignment_x, (float)(m_SecondaryMembers[0].CrScStart.y_min + flocaleccentricity_y), m_fPlate_Angle_Height + m_SecondaryMembers[0].CrScStart.z_min + flocaleccentricity_z);
                RotationVector_P1 = new Vector3D(180 + 90, 0, 0);
                RotationVector_P2 = new Vector3D(180 + 90, 0, 180);
            }

            CConCom_Plate_F_or_L pLeftPlate = new CConCom_Plate_F_or_L(sPlateType_F_left, ControlPoint_P1, m_fPlate_Angle_RightLeg_BX1, m_fPlate_Angle_RightLeg_BX2, m_fPlate_Angle_Height, m_fPlate_Angle_LeftLeg, m_ft, (float)m_SecondaryMembers[0].CrScStart.h, (float)RotationVector_P1.X, (float)RotationVector_P1.Y, (float)RotationVector_P1.Z, screwArrangement1); // Rotation angle in degrees
            CConCom_Plate_F_or_L pRightPlate = new CConCom_Plate_F_or_L(sPlateType_F_right, ControlPoint_P2, m_fPlate_Angle_RightLeg_BX1, m_fPlate_Angle_RightLeg_BX2, m_fPlate_Angle_Height, m_fPlate_Angle_LeftLeg, m_ft, (float)m_SecondaryMembers[0].CrScStart.h, (float)RotationVector_P2.X, (float)RotationVector_P2.Y, (float)RotationVector_P2.Z, screwArrangement2); // Rotation angle in degrees

            // Identification of current joint node location (start or end definition node of secondary member)
            if (m_Node.ID != m_SecondaryMembers[0].NodeStart.ID) // If true - joint at start node, if false joint at end node (so we need to rotate joint about z-axis 180 deg)
            {
                if (m_SecondaryMembers[0] != null)
                    fAlignment_x = -m_SecondaryMembers[0].FAlignment_End + m_ft_main_plate - fCutOffOneSide;

                // Rotate and move joint defined in the start point [0,0,0] to the end point
                ControlPoint_P1 = new Point3D(m_SecondaryMembers[0].FLength - fAlignment_x, (float)(m_SecondaryMembers[0].CrScStart.y_max + flocaleccentricity_y), -m_fPlate_Angle_Height + m_SecondaryMembers[0].CrScStart.z_max + flocaleccentricity_z);
                ControlPoint_P2 = new Point3D(m_SecondaryMembers[0].FLength - fAlignment_x, (float)(m_SecondaryMembers[0].CrScStart.y_min + flocaleccentricity_y), -m_fPlate_Angle_Height + m_SecondaryMembers[0].CrScStart.z_max + flocaleccentricity_z);
                RotationVector_P1 = new Vector3D(90, 0, 180 + 0);
                RotationVector_P2 = new Vector3D(90, 0, 180 + 180);

                if (!m_bTopOfPlateInCrscVerticalAxisPlusDirection)
                {
                    ControlPoint_P1 = new Point3D(m_SecondaryMembers[0].FLength - fAlignment_x, (float)(m_SecondaryMembers[0].CrScStart.y_min + flocaleccentricity_y), m_fPlate_Angle_Height + m_SecondaryMembers[0].CrScStart.z_min + flocaleccentricity_z);
                    ControlPoint_P2 = new Point3D(m_SecondaryMembers[0].FLength - fAlignment_x, (float)(m_SecondaryMembers[0].CrScStart.y_max + flocaleccentricity_y), m_fPlate_Angle_Height + m_SecondaryMembers[0].CrScStart.z_min + flocaleccentricity_z);
                    RotationVector_P1 = new Vector3D(180 + 90, 0, 180 + 0);
                    RotationVector_P2 = new Vector3D(180 + 90, 0, 180 + 180);
                }

                pLeftPlate = new CConCom_Plate_F_or_L(sPlateType_F_left, ControlPoint_P1, m_fPlate_Angle_RightLeg_BX1, m_fPlate_Angle_RightLeg_BX2, m_fPlate_Angle_Height, m_fPlate_Angle_LeftLeg, m_ft, (float)m_SecondaryMembers[0].CrScStart.h, (float)RotationVector_P1.X, (float)RotationVector_P1.Y, (float)RotationVector_P1.Z, screwArrangement1); // Rotation angle in degrees
                pRightPlate = new CConCom_Plate_F_or_L(sPlateType_F_right, ControlPoint_P2, m_fPlate_Angle_RightLeg_BX1, m_fPlate_Angle_RightLeg_BX2, m_fPlate_Angle_Height, m_fPlate_Angle_LeftLeg, m_ft, (float)m_SecondaryMembers[0].CrScStart.h, (float)RotationVector_P2.X, (float)RotationVector_P2.Y, (float)RotationVector_P2.Z, screwArrangement2); // Rotation angle in degrees
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
            return new CConnectionJoint_T003(m_sPlateType_F_left, m_sPlateType_F_right, m_Node, m_MainMember, m_SecondaryMembers[0], m_ft_main_plate, m_ePlateNumberAndPosition, m_bTopOfPlateInCrscVerticalAxisPlusDirection);
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

            Point3D ControlPoint_P1 = new Point3D(fAlignment_x, (float)(m_SecondaryMembers[0].CrScStart.y_min + flocaleccentricity_y), -m_fPlate_Angle_Height + m_SecondaryMembers[0].CrScStart.z_max + flocaleccentricity_z);
            Point3D ControlPoint_P2 = new Point3D(fAlignment_x, (float)(m_SecondaryMembers[0].CrScStart.y_max + flocaleccentricity_y), -m_fPlate_Angle_Height + m_SecondaryMembers[0].CrScStart.z_max + flocaleccentricity_z);
            Vector3D RotationVector_P1 = new Vector3D(90, 0, 0);
            Vector3D RotationVector_P2 = new Vector3D(90, 0, 180);

            if (!m_bTopOfPlateInCrscVerticalAxisPlusDirection)
            {
                ControlPoint_P1 = new Point3D(fAlignment_x, (float)(m_SecondaryMembers[0].CrScStart.y_max + flocaleccentricity_y), m_fPlate_Angle_Height + m_SecondaryMembers[0].CrScStart.z_min + flocaleccentricity_z);
                ControlPoint_P2 = new Point3D(fAlignment_x, (float)(m_SecondaryMembers[0].CrScStart.y_min + flocaleccentricity_y), m_fPlate_Angle_Height + m_SecondaryMembers[0].CrScStart.z_min + flocaleccentricity_z);
                RotationVector_P1 = new Vector3D(180 + 90, 0, 0);
                RotationVector_P2 = new Vector3D(180 + 90, 0, 180);
            }

            if (m_Node.ID != m_SecondaryMembers[0].NodeStart.ID) // If true - joint at start node, if false joint at end node (so we need to rotate joint about z-axis 180 deg)
            {
                if (m_SecondaryMembers[0] != null)
                    fAlignment_x = -m_SecondaryMembers[0].FAlignment_End + m_ft_main_plate - fCutOffOneSide;

                // Rotate and move joint defined in the start point [0,0,0] to the end point
                ControlPoint_P1 = new Point3D(m_SecondaryMembers[0].FLength - fAlignment_x, (float)(m_SecondaryMembers[0].CrScStart.y_max + flocaleccentricity_y), -m_fPlate_Angle_Height + m_SecondaryMembers[0].CrScStart.z_max + flocaleccentricity_z);
                ControlPoint_P2 = new Point3D(m_SecondaryMembers[0].FLength - fAlignment_x, (float)(m_SecondaryMembers[0].CrScStart.y_min + flocaleccentricity_y), -m_fPlate_Angle_Height + m_SecondaryMembers[0].CrScStart.z_max + flocaleccentricity_z);
                RotationVector_P1 = new Vector3D(90, 0, 180 + 0);
                RotationVector_P2 = new Vector3D(90, 0, 180 + 180);

                if (!m_bTopOfPlateInCrscVerticalAxisPlusDirection)
                {
                    ControlPoint_P1 = new Point3D(m_SecondaryMembers[0].FLength - fAlignment_x, (float)(m_SecondaryMembers[0].CrScStart.y_min + flocaleccentricity_y), m_fPlate_Angle_Height + m_SecondaryMembers[0].CrScStart.z_min + flocaleccentricity_z);
                    ControlPoint_P2 = new Point3D(m_SecondaryMembers[0].FLength - fAlignment_x, (float)(m_SecondaryMembers[0].CrScStart.y_max + flocaleccentricity_y), m_fPlate_Angle_Height + m_SecondaryMembers[0].CrScStart.z_min + flocaleccentricity_z);
                    RotationVector_P1 = new Vector3D(180 + 90, 0, 180 + 0);
                    RotationVector_P2 = new Vector3D(180 + 90, 0, 180 + 180);
                }
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
