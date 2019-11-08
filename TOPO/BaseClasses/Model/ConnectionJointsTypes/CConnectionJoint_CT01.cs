using BaseClasses.GraphObj;
using System.Windows.Media.Media3D;
using BaseClasses.Helpers;

namespace BaseClasses
{
    public class CConnectionJoint_CT01 : CConnectionJointTypes
    {
        // Column to Rafter Connection (Front and back side of portal)
        float m_ft;
        int m_iHoleNo;
        float m_fd_hole;
        float m_flip;

        public CConnectionJoint_CT01() { }

        public CConnectionJoint_CT01(CNode Node_temp, CMember Rafter_temp, CMember SecondaryConnectedMember_temp, bool bIsDisplayed_temp)
        {
            // TODO !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            // ZAPRACOVAT !!!!!!!!!!!!

            bIsJointDefinedinGCS = false;

            m_Node = Node_temp;
            m_pControlPoint = m_Node.GetPoint3D();
            m_MainMember = Rafter_temp;
            m_SecondaryMembers = new CMember[1];
            m_SecondaryMembers[0] = SecondaryConnectedMember_temp;

            BIsGenerated = true;
            BIsDisplayed = bIsDisplayed_temp;

            // Plate properties
            // Todo - set correct dimensions of plate acc. to column cross-section size
            float fb_plate = 0.5f; // TODO ??????????? Calculate
            float fh_plate = 0.1f; // Strip 100 mm
            m_ft = 0.001f;
            m_iHoleNo = 0; // TEMPORARY

            float fAlignment_x = 0; // Odsadenie plechu od definicneho uzla pruta

            Point3D ControlPoint_P1 = new Point3D(fAlignment_x, /*m_MainMember.CrScStart.y_min*/ - 0.5f * fb_plate, -0.5f * fh_plate);
            CScrew referenceScrew = new CScrew("TEK", "14");
            CScrewArrangement_L screwArrangement = new CScrewArrangement_L(m_iHoleNo, referenceScrew);

            m_arrPlates = new CPlate[1];
            m_arrPlates[0] = new CConCom_Plate_F_or_L("LJ", ControlPoint_P1, fb_plate, fh_plate, 0, m_ft, 90, 0, 90, screwArrangement, bIsDisplayed_temp); // Rotation angle in degrees

            if (m_Node.ID != m_SecondaryMembers[0].NodeStart.ID) // If true - joint at start node, if false joint at end node (se we need to rotate joint about z-axis 180 deg)
            {
                // Rotate and move joint defined in the start point [0,0,0] to the end point
                ControlPoint_P1 = new Point3D(m_SecondaryMembers[0].FLength - fAlignment_x, /*m_MainMember.CrScStart.y_max*/ + 0.5f * fb_plate, -0.5f * fh_plate);
                m_arrPlates[0] = new CConCom_Plate_F_or_L("LJ", ControlPoint_P1, fb_plate, fh_plate, m_flip, m_ft, 90, 0, 180+90, screwArrangement, bIsDisplayed_temp); // Rotation angle in degrees
            }
        }

        public override CConnectionJointTypes RecreateJoint()
        {
            return new CConnectionJoint_CT01(m_Node, m_MainMember, m_SecondaryMembers[0], BIsDisplayed);
        }
    }
}
