using BaseClasses.GraphObj;


namespace BaseClasses
{
    public class CConnectionJoint_D001 : CConnectionJointTypes
    {
        // Girts to Main Column Joint
        float m_ft;

        public CConnectionJoint_D001() { }

        public CConnectionJoint_D001(CNode Node_temp, CMember MainFrameColumn_temp, CMember Girt_temp, bool bIsDisplayed_temp)
        {
            bIsJointDefinedinGCS = true;

            m_Node = Node_temp;
            m_MainMember = MainFrameColumn_temp;
            m_SecondaryMembers = new CMember[1];
            m_SecondaryMembers[0] = Girt_temp;

            m_ft = 0.003f;

            BIsDisplayed = bIsDisplayed_temp;

            float fGirtVectorDirection = m_SecondaryMembers[0].NodeEnd.X - m_Node.X; // If positive rotate joint plates 0 deg, if negative rotate 180 deg
            float fRotatePlatesInJointAngle = fGirtVectorDirection > 0 ? 270 : 90;

            float fControlPointYCoord1;

            if (fGirtVectorDirection > 0)
            {
                fControlPointYCoord1 = (float)(m_Node.Y - 0.5f * m_MainMember.CrScStart.b - m_ft);
            }
            else
            {
                fControlPointYCoord1 = (float)(m_Node.Y + 0.5f * m_MainMember.CrScStart.b + m_ft);
            }

            float fTemp = 0.5f * (float)m_SecondaryMembers[0].CrScStart.b; // TODO - pomocna hodnota - odstranit po prepocitani suradnic prierezu voci tazisku, resp stredu geometrie

            CPoint ControlPoint_P1 = new CPoint(0, m_Node.X + 0.5f * (float)m_SecondaryMembers[0].CrScStart.h, fControlPointYCoord1, m_Node.Z + 0.5f * m_SecondaryMembers[0].CrScStart.b + fTemp, 0);
            CPoint ControlPoint_P2 = new CPoint(0, m_Node.X - 0.5f * (float)m_SecondaryMembers[0].CrScStart.h, fControlPointYCoord1, m_Node.Z - 0.5f * m_SecondaryMembers[0].CrScStart.b + fTemp, 0);

            CScrew referenceScrew = new CScrew("TEK", "14");
            CScrewArrangement screwArrangement = new CScrewArrangement(0, referenceScrew);

            m_arrPlates = new CPlate[2];
            m_arrPlates[0] = new CConCom_Plate_F_or_L("LH", ControlPoint_P1, 0.05f, (float)m_SecondaryMembers[0].CrScStart.h, 0.05f, 0.003f, 0, 0, fRotatePlatesInJointAngle, screwArrangement, BIsDisplayed); // Rotation angle in degrees
            m_arrPlates[1] = new CConCom_Plate_F_or_L("LH", ControlPoint_P2, 0.05f, (float)m_SecondaryMembers[0].CrScStart.h, 0.05f, 0.003f, 0, 180, 180+fRotatePlatesInJointAngle, screwArrangement, BIsDisplayed); // Rotation angle in degrees
        }
    }
}
