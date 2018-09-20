using BaseClasses.GraphObj;


namespace BaseClasses
{
    public class CConnectionJoint_AA01 : CConnectionJointTypes
    {
        // Main Column to Foundation Connection
        float m_ft;

        public CConnectionJoint_AA01() { }

        public CConnectionJoint_AA01(CNode Node_temp, CMember MainFrameColumn_temp, bool bIsDisplayed_temp)
        {
            bIsJointDefinedinGCS = true;

            m_Node = Node_temp;
            m_MainMember = MainFrameColumn_temp;

            m_ft = 0.003f;

            BIsDisplayed = bIsDisplayed_temp;

            // Todo - set correct dimensions of plate acc. to column cross-section size
            float fb_plate = (float)(0.85 * MainFrameColumn_temp.CrScStart.b);
            float fh_plate = (float)(0.9 * MainFrameColumn_temp.CrScStart.h);
            int iHoleNo = 2;

            CPoint ControlPoint_P1 = new CPoint(0, m_Node.X + 0.5f * fh_plate, m_Node.Y - 0.5f * fb_plate, m_Node.Z, 0);
            CScrew referenceScrew = new CScrew("TEK", "14");
            CAnchor referenceAnchor = new CAnchor(0.02f, 0.18f, 0.5f, true);

            CScrewArrangement_BB_BG screwArrangement = new CScrewArrangement_BB_BG(iHoleNo, referenceScrew, referenceAnchor);

            m_arrPlates = new CPlate[1];
            m_arrPlates[0] = new CConCom_Plate_BB_BG("LH", ControlPoint_P1, fb_plate, fh_plate, 0.18f, m_ft, iHoleNo, referenceScrew, referenceAnchor, 0 ,0 ,90 , screwArrangement, bIsDisplayed_temp); // Rotation angle in degrees
        }
    }
}
