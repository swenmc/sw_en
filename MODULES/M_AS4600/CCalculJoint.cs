using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaseClasses;
using MATH;
using CRSC;
namespace M_AS4600
{
    public class CCalculJoint
    {
        public CConnectionJointTypes joint;
        public designInternalForces sDIF;
        bool bIsDebugging;

        public CCalculJoint(CConnectionJointTypes joint_temp, designInternalForces sDIF_temp)
        {
            joint = joint_temp;
            sDIF = sDIF_temp;

            CalculateDesignRatio(bIsDebugging, joint_temp, sDIF_temp);
        }

        public void CalculateDesignRatio(bool bIsDebugging, CConnectionJointTypes joint_temp, designInternalForces sDIF_temp)
        {
            AS_4600 eq = new AS_4600();

            //df = nominal screw diameter
            CScrew screw = (CScrew)joint_temp.m_arrConnectors[0];
            CPlate plate = joint_temp.m_arrPlates[0];
            CCrSc_TW crsc_mainMember = (CCrSc_TW)joint_temp.m_MainMember.CrScStart;
            CCrSc_TW crsc_secMember = (CCrSc_TW)joint_temp.m_SecondaryMembers[0].CrScStart;
            float ft_2_crscmainMember = (float)crsc_mainMember.t_min;
            float ft_2_crscsecMember = (float)crsc_secMember.t_min;

            float ft_2_crsc = Math.Min(ft_2_crscmainMember, ft_2_crscsecMember);

            float fVb_MainMember = eq.Get_Vb_5424(plate.fThickness_tz, ft_2_crscmainMember, screw.m_fDiameter, plate.m_Mat.Get_f_uk_by_thickness(plate.fThickness_tz), crsc_mainMember.m_Mat.Get_f_uk_by_thickness((float)crsc_mainMember.t_min));
            float fVb_SecondaryMember = eq.Get_Vb_5424(plate.fThickness_tz, ft_2_crscsecMember, screw.m_fDiameter, plate.m_Mat.Get_f_uk_by_thickness(plate.fThickness_tz), crsc_secMember.m_Mat.Get_f_uk_by_thickness((float)crsc_secMember.t_min));

            int fNumberOfScrewsInTension = 0;
            int fNumberOfScrewsInShear = joint_temp.m_arrConnectors.Length; // Temporary

            float fDesignRatio_MainMember = sDIF.fV_zv / (fNumberOfScrewsInShear * fVb_MainMember);
            float fDesignRatio_SecondaryMember = sDIF.fV_zv / (fNumberOfScrewsInShear * fVb_SecondaryMember);
        }
    }
}
