﻿using BaseClasses.GraphObj;
using System.Windows.Media.Media3D;
using BaseClasses.Helpers;

namespace BaseClasses
{
    public class CConnectionJoint_TA01 : CConnectionJointTypes
    {
        // Main Column to Foundation Connection
        public float m_ft;
        public int m_iHoleNo;
        public float m_fd_hole;
        public float m_flip;

        public CConnectionJoint_TA01() { }

        public CConnectionJoint_TA01(EJointType jointType_temp, CNode Node_temp, CMember MainFrameColumn_temp)
        {
            bIsJointDefinedinGCS = false;

            JointType = jointType_temp;
            m_Node = Node_temp;
            ControlPoint = m_Node.GetPoint3D();
            m_MainMember = MainFrameColumn_temp;

            Name = "Main Column Base Joint";

            float flocaleccentricity_y = m_MainMember.EccentricityStart == null ? 0f : m_MainMember.EccentricityStart.MFy_local;
            float flocaleccentricity_z = m_MainMember.EccentricityStart == null ? 0f : m_MainMember.EccentricityStart.MFz_local;

            CAnchor referenceAnchor = new CAnchor("M16", "8.8", 0.33f, 0.3f, true);
            CScrew referenceScrew = new CScrew("TEK", "14");

            CScrewArrangement screwArrangement;
            string sPlatePrefix;
            CWasher_W washerPlateTop;
            CWasher_W washerBearing;

            DATABASE.DTO.CPlate_B_Properties plateProp;
            SetBasePlateTypeAndScrewArrangement(m_MainMember.CrScStart.Name_short, referenceScrew, referenceAnchor, out sPlatePrefix, out plateProp, out screwArrangement, out washerPlateTop, out washerBearing);

            float fb_plate = (float)plateProp.dim1;
            float fh_plate = (float)plateProp.dim2y;
            m_flip = (float)plateProp.dim3;
            m_ft = (float)plateProp.t;

            referenceAnchor.WasherPlateTop = washerPlateTop;
            referenceAnchor.WasherBearing = washerBearing;

            // Skratime prut v uzle spoja - base plate o hrubku plechu
            if (m_MainMember.NodeStart.Equals(m_Node)) m_MainMember.FAlignment_Start -= m_ft; // Skratit prut (skutocny rozmer) o hrubku base plate - columns at the left side
            else if (m_MainMember.NodeEnd.Equals(m_Node)) m_MainMember.FAlignment_End -= m_ft;  // Skratit prut (skutocny rozmer) o hrubku base plate - columns at the right side
            else
                throw new System.Exception("Joint node is not definition node of member.");

            // Recalculate member parameters
            m_MainMember.Fill_Basic();

            Point3D ControlPoint_P1 = new Point3D(0, m_MainMember.CrScStart.y_min + flocaleccentricity_y - m_ft, -0.5f * fh_plate + flocaleccentricity_z);
            Vector3D RotationVector = new Vector3D(90, 0, 90);

            m_arrPlates = new CPlate[1];
            m_arrPlates[0] = new CConCom_Plate_B_basic(sPlatePrefix, ControlPoint_P1, fb_plate, fh_plate, m_flip, m_ft, (float)RotationVector.X, (float)RotationVector.Y, (float)RotationVector.Z, referenceAnchor, screwArrangement); // Rotation angle in degrees

            if (m_Node.ID != m_MainMember.NodeStart.ID) // If true - joint at start node, if false joint at end node (se we need to rotate joint about z-axis 180 deg)
            {
                // Rotate and move joint defined in the start point [0,0,0] to the end point
                ControlPoint_P1 = new Point3D(m_MainMember.FLength, m_MainMember.CrScStart.y_max + flocaleccentricity_y + m_ft, -0.5f * fh_plate + flocaleccentricity_z);
                RotationVector = new Vector3D(90, 0, 180+90);
                m_arrPlates[0] = new CConCom_Plate_B_basic(sPlatePrefix, ControlPoint_P1, fb_plate, fh_plate, m_flip, m_ft, (float)RotationVector.X, (float)RotationVector.Y, (float)RotationVector.Z, referenceAnchor, screwArrangement); // Rotation angle in degrees
            }
        }

        public override CConnectionJointTypes RecreateJoint()
        {
            return new CConnectionJoint_TA01(JointType, m_Node, m_MainMember);
        }

        public override void UpdateJoint()
        {
            // Update Plate Control Point
            float flocaleccentricity_y = m_MainMember.EccentricityStart == null ? 0f : m_MainMember.EccentricityStart.MFy_local;
            float flocaleccentricity_z = m_MainMember.EccentricityStart == null ? 0f : m_MainMember.EccentricityStart.MFz_local;

            m_arrPlates[0].ControlPoint = new Point3D(0, m_MainMember.CrScStart.y_min + flocaleccentricity_y - m_arrPlates[0].Ft, -0.5f * m_arrPlates[0].Height_hy + flocaleccentricity_z);
            Vector3D RotationVector = new Vector3D(90, 0, 90);

            if (m_Node.ID != m_MainMember.NodeStart.ID) // If true - joint at start node, if false joint at end node (se we need to rotate joint about z-axis 180 deg)
            {
                // Rotate and move joint defined in the start point [0,0,0] to the end point
                m_arrPlates[0].ControlPoint = new Point3D(m_MainMember.FLength, m_MainMember.CrScStart.y_max + flocaleccentricity_y + m_arrPlates[0].Ft, -0.5f * m_arrPlates[0].Height_hy + flocaleccentricity_z);
                RotationVector = new Vector3D(90, 0, 180 + 90);
            }

            m_arrPlates[0].SetPlateRotation(RotationVector);

            // Update Member Alignment
            if (m_Node.ID == m_MainMember.NodeStart.ID)
                m_MainMember.FAlignment_Start = -m_arrPlates[0].Ft;

            if (m_Node.ID == m_MainMember.NodeEnd.ID)
                m_MainMember.FAlignment_End = -m_arrPlates[0].Ft;

            // Update plate and its screw and anchor arrangement - positions of screws and anchors in plate

            // To Ondrej - toto by sa dalo asi nejako zjednodusit a upravit
            CConCom_Plate_B_basic plate = (CConCom_Plate_B_basic)m_arrPlates[0];
            if (plate.AnchorArrangement != null)
            {
                plate.AnchorArrangement.Calc_HolesCentersCoord3D(plate.Width_bx, plate.Height_hy, plate.Fl_Z);
                plate.AnchorArrangement.GenerateAnchors_BasePlate();
            }
            plate.UpdatePlateData(plate.AnchorArrangement);
            m_arrPlates[0] = plate;
        }
    }
}
