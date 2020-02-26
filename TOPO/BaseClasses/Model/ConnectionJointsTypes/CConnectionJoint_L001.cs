using BaseClasses.GraphObj;
using System.Windows.Media.Media3D;
using BaseClasses.Helpers;

namespace BaseClasses
{
    public class CConnectionJoint_L001 : CConnectionJointTypes
    {
        // Back Girts to Back Wind Post Joint
        public float m_ft;

        public CConnectionJoint_L001() { }

        public CConnectionJoint_L001(CNode Node_temp, CMember FrontColumn_temp, CMember Girt_temp)
        {
            bIsJointDefinedinGCS = true;

            m_Node = Node_temp;
            m_pControlPoint = m_Node.GetPoint3D();
            m_MainMember = FrontColumn_temp;
            m_SecondaryMembers = new CMember[1];
            m_SecondaryMembers[0] = Girt_temp;

            m_ft = 0.003f;

            float fGirtVectorDirection = m_SecondaryMembers[0].NodeEnd.X - m_Node.X; // If positive rotate joint plates 180 deg, if negative rotate 0 deg
            float fRotatePlatesInJointAngle = fGirtVectorDirection > 0 ? 180 : 0;

            float fControlPointXCoord1;
            float fControlPointYCoord1;
            float fControlPointYCoord2;

            if (fGirtVectorDirection > 0)
            {
                fControlPointXCoord1 = (float)(m_Node.X - 0.5f * m_MainMember.CrScStart.b);

                fControlPointYCoord1 = (float)(m_Node.Y + 0.5f * m_SecondaryMembers[0].CrScStart.h);
                fControlPointYCoord2 = (float)(m_Node.Y - 0.5f * m_SecondaryMembers[0].CrScStart.h);
            }
            else
            {
                fControlPointXCoord1 = (float)(m_Node.X + 0.5f * m_MainMember.CrScStart.b);

                fControlPointYCoord1 = (float)(m_Node.Y - 0.5f * m_SecondaryMembers[0].CrScStart.h);
                fControlPointYCoord2 = (float)(m_Node.Y + 0.5f * m_SecondaryMembers[0].CrScStart.h);
            }

            float fTemp = 0.5f * (float)m_SecondaryMembers[0].CrScStart.b; // TODO - pomocna hodnota - odstranit po prepocitani suradnic prierezu voci tazisku, resp stredu geometrie

            // Todo - zohladnit rozmer hlavneho stlpa a medzilahlych stlpikov, aktualne sa uvazuju len medzilahle stlpiky
            // Todo - pripravit nastroj, ktory identifikuje ktore pruty su pripojene, ku ktoremu uzlu (teoreticky je mozne ukladat do to zoznamu v objekte Node pri generovani prutov v CExample_3D_901_PF ale bude to komplikovanejsie pre mezilahle uzly na prute (napr. pre purlins - uzly na rafter alebo girts - uzly na columns)
            // Todo - zohladnit roznu orientaciu pruta kde je start a kde je end node
            // Todo - Prepracovat tak, ze poloha plechov m_arrPlates bude naviazana na LCS pruta (v start a end node) a plechy sa s prutom budu otacat okolo jeho LCS osi x (uhol DTheta)

            Point3D ControlPoint_P1 = new Point3D(fControlPointXCoord1, fControlPointYCoord1, m_Node.Z + 0.5f * m_SecondaryMembers[0].CrScStart.b + fTemp);
            Point3D ControlPoint_P2 = new Point3D(fControlPointXCoord1, fControlPointYCoord2, m_Node.Z - 0.5f * m_SecondaryMembers[0].CrScStart.b + fTemp);

            CScrew referenceScrew = new CScrew("TEK", "14");
            CScrewArrangement_L screwArrangement = new CScrewArrangement_L(0, referenceScrew);

            m_arrPlates = new CPlate[2];
            m_arrPlates[0] = new CConCom_Plate_F_or_L("LH", ControlPoint_P1, 0.05f, (float)m_SecondaryMembers[0].CrScStart.h, 0.05f, 0.003f, (float)m_SecondaryMembers[0].CrScStart.h, 0, 0, fRotatePlatesInJointAngle, screwArrangement); // Rotation angle in degrees
            m_arrPlates[1] = new CConCom_Plate_F_or_L("LH", ControlPoint_P2, 0.05f, (float)m_SecondaryMembers[0].CrScStart.h, 0.05f, 0.003f, (float)m_SecondaryMembers[0].CrScStart.h, 180, 0, fRotatePlatesInJointAngle, screwArrangement); // Rotation angle in degrees
        }

        public override CConnectionJointTypes RecreateJoint()
        {
            return new CConnectionJoint_L001(m_Node, m_MainMember, m_SecondaryMembers[0]);
        }
    }
}
