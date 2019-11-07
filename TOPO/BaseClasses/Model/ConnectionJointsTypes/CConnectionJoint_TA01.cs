﻿using BaseClasses.GraphObj;
using System.Windows.Media.Media3D;

namespace BaseClasses
{
    public class CConnectionJoint_TA01 : CConnectionJointTypes
    {
        // Main Column to Foundation Connection
        float m_ft;
        int m_iHoleNo;
        float m_fd_hole;
        float m_flip;

        public CConnectionJoint_TA01() { }

        public CConnectionJoint_TA01(CNode Node_temp, CMember MainFrameColumn_temp, bool bIsDisplayed_temp)
        {
            bIsJointDefinedinGCS = false;

            m_Node = Node_temp;
            m_MainMember = MainFrameColumn_temp;
            BIsGenerated = true;
            BIsDisplayed = bIsDisplayed_temp;

            Name = "Main Column Base Joint";

            // Plate properties
            // Todo - set correct dimensions of plate acc. to column cross-section size
            m_ft = 0.003f;
            m_flip = 0.18f;

            //to Mato - tu je problem task 355
            if (MATH.MathF.d_equal(m_MainMember.FAlignment_Start, 0)) m_MainMember.FAlignment_Start = -m_ft; // Skratit prut (skutocny rozmer) o hrubku base plate
            else m_MainMember.FAlignment_End = -m_ft;  //doplnene pre columns na pravej strane
            m_MainMember.Fill_Basic();

            float fTolerance = 0.001f; // Gap between cross-section surface and plate surface
            float fb_plate = (float)(MainFrameColumn_temp.CrScStart.b + 2 * fTolerance + 2 * m_ft);
            float fh_plate = (float)(MainFrameColumn_temp.CrScStart.h);

            float fAlignment_x = 0; // Odsadenie plechu od definicneho uzla pruta

            float flocaleccentricity_y = m_MainMember.EccentricityStart == null ? 0f : m_MainMember.EccentricityStart.MFy_local;
            float flocaleccentricity_z = m_MainMember.EccentricityStart == null ? 0f : m_MainMember.EccentricityStart.MFz_local;

            Point3D ControlPoint_P1 = new Point3D(fAlignment_x, m_MainMember.CrScStart.y_min + flocaleccentricity_y - m_ft - fTolerance, -0.5f * fh_plate + flocaleccentricity_z);
            CAnchor referenceAnchor = new CAnchor("M16", "8.8", 0.33f, 0.3f, true);
            CScrew referenceScrew = new CScrew("TEK", "14");

            CScrewArrangement screwArrangement;
            string sPlatePrefix;

            SetPlateTypeAndScrewArrangement(m_MainMember.CrScStart.Name_short, referenceScrew, fh_plate, out sPlatePrefix, out screwArrangement);

            m_arrPlates = new CPlate[1];
            m_arrPlates[0] = new CConCom_Plate_B_basic(sPlatePrefix, ControlPoint_P1, fb_plate, fh_plate, m_flip, m_ft, 90, 0, 90, referenceAnchor, screwArrangement, bIsDisplayed_temp); // Rotation angle in degrees

            if (m_Node.ID != m_MainMember.NodeStart.ID) // If true - joint at start node, if false joint at end node (se we need to rotate joint about z-axis 180 deg)
            {
                // Rotate and move joint defined in the start point [0,0,0] to the end point
                ControlPoint_P1 = new Point3D(m_MainMember.FLength - fAlignment_x, m_MainMember.CrScStart.y_max + flocaleccentricity_y + m_ft + fTolerance, -0.5f * fh_plate + flocaleccentricity_z);
                m_arrPlates[0] = new CConCom_Plate_B_basic(sPlatePrefix, ControlPoint_P1, fb_plate, fh_plate, m_flip, m_ft, 90, 0, 180+90, referenceAnchor, screwArrangement, bIsDisplayed_temp); // Rotation angle in degrees
            }
        }

        public override CConnectionJointTypes RecreateJoint()
        {
            return new CConnectionJoint_TA01(m_Node, m_MainMember, BIsDisplayed);
        }
    }
}
