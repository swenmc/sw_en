using BaseClasses.GraphObj;
using MATH;
using System;
using System.Windows.Media.Media3D;
using BaseClasses.Helpers;

namespace BaseClasses
{
    public class CConnectionJoint_E001 : CConnectionJointTypes
    {
        // Purlin to Rafter Joint
        public float m_ft;

        public CConnectionJoint_E001() { }

        public CConnectionJoint_E001(CNode Node_temp, CMember MainFrameRafter_temp, CMember Purlin_temp)
        {
            bIsJointDefinedinGCS = true;

            m_Node = Node_temp;
            ControlPoint = m_Node.GetPoint3D();
            m_MainMember = MainFrameRafter_temp;
            m_SecondaryMembers = new CMember[1];
            m_SecondaryMembers[0] = Purlin_temp;

            m_ft = 0.003f;

            float fPurlinVectorDirection = m_SecondaryMembers[0].NodeEnd.X - m_Node.X; // If positive rotate joint plates 0 deg, if negative rotate 180 deg
            float fRotatePlatesInJointAngle = fPurlinVectorDirection > 0 ? 0 : 180;

            // Auxiliary values

            // Local coordinates (cross-section system y/z)
            float fy = (float)(((0.5f * m_SecondaryMembers[0].CrScStart.b + m_ft)* Math.Cos(-m_SecondaryMembers[0].DTheta_x)) + (0.5f * m_SecondaryMembers[0].CrScStart.h * Math.Sin(-m_SecondaryMembers[0].DTheta_x)));
            float fz = (float)(0.5f * m_SecondaryMembers[0].CrScStart.h / Math.Cos(-m_SecondaryMembers[0].DTheta_x));

            float fControlPointYCoord1;

            if (fPurlinVectorDirection > 0)
            {
                fControlPointYCoord1 = (float)(m_Node.Y - 0.5f * m_MainMember.CrScStart.b - m_ft);
            }
            else
            {
                fControlPointYCoord1 = (float)(m_Node.Y + 0.5f * m_MainMember.CrScStart.b + m_ft);
            }

            float rotation_x_member = -(float)(m_SecondaryMembers[0].DTheta_x / MathF.dPI * 180); // Conversion to deg

            Point3D ControlPoint_P1 = new Point3D(m_Node.X + fy, fControlPointYCoord1, m_Node.Z - fz);
            CScrew referenceScrew = new CScrew("TEK", "14");
            CScrewArrangement_LL screwArrangement = new CScrewArrangement_LL(0, referenceScrew);

            m_arrPlates = new CPlate[1];
            m_arrPlates[0] = new CConCom_Plate_LL("LLH", ControlPoint_P1, 0.05f, (float)m_SecondaryMembers[0].CrScStart.b, (float)m_SecondaryMembers[0].CrScStart.h, 0.05f, 0.003f, 90, rotation_x_member, fRotatePlatesInJointAngle, screwArrangement); // Rotation angle in degrees
        }

        public override CConnectionJointTypes RecreateJoint()
        {
            return new CConnectionJoint_E001(m_Node, m_MainMember, m_SecondaryMembers[0]);
        }
    }
}
