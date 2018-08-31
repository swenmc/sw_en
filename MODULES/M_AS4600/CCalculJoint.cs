using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
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

        public float fEta_max = 0;

        public CCalculJoint(CConnectionJointTypes joint_temp, designInternalForces sDIF_temp)
        {
            joint = joint_temp;
            sDIF = sDIF_temp;

            CalculateDesignRatio(bIsDebugging, joint_temp, sDIF_temp);
        }

        public void CalculateDesignRatio(bool bIsDebugging, CConnectionJointTypes joint_temp, designInternalForces sDIF_temp)
        {
            AS_4600 eq = new AS_4600();

            // 1.6.3 Design capacity Rd
            // (i) For members      Φ = 0.80
            // (ii) For connections Φ = 0.65

            /*
            (i) Screwed connections: 5.4
                Screwed connections in shear — 5.4.2
                tension in the connected part 5.4.2.3                       0.65
                tilting and hole bearing 5.4.2.4                            0.50
                tearout(limited by end distance) 5.4.2.5                    0.60 or 0.70
                Screwed connections in tension — 5.4.3
                pull -out of connected parts 5.4.3.2                        0.50
                pull - over(pull - through) of connected parts 5.4.3.2      0.50
            */

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

            float fEta_MainMember = sDIF.fV_zv / (fNumberOfScrewsInShear * fVb_MainMember);
            float fEta_SecondaryMember = sDIF.fV_zv / (fNumberOfScrewsInShear * fVb_SecondaryMember);

            float fM_xu_oneside = 0.5f * sDIF_temp.fM_yu;
            float fV_yv_oneside = 0.5f * sDIF_temp.fV_zv;

            float fVb_MainMember_oneside_plastic = 0;
            float fVb_SecondaryMember_oneside_plastic = 0;

            float fSumri2tormax = 0; // F_max = Mxu / (Σ ri^2 / r_max)
            CConCom_Plate_KA a = (CConCom_Plate_KA)plate;

            float fr_max = MathF.Max(a.HolesCenterRadii);

            // 5.4.2.4 Tilting and hole bearing
            // Bending - Calculate shear strength of plate connection - main member
            for (int i = 0; i < a.IHolesNumber / a.INumberOfCircleJoints; i++)
            {
                fVb_MainMember_oneside_plastic += a.HolesCenterRadii[i] * fVb_MainMember;
                fVb_SecondaryMember_oneside_plastic += a.HolesCenterRadii[/*a.IHolesNumber / 2 +*/ i] * fVb_SecondaryMember;

                fSumri2tormax += MathF.Pow2(a.HolesCenterRadii[i]) / fr_max;
            }

            float fV_asterix_b_max_screw_Mxu = Math.Abs(fM_xu_oneside) / fSumri2tormax;
            float fV_asterix_b_max_screw_Vyv = Math.Abs(sDIF.fV_zv) / (a.IHolesNumber / a.INumberOfCircleJoints);
            float fV_asterix_b_max_screw_N = Math.Abs(sDIF.fN) / (a.IHolesNumber / a.INumberOfCircleJoints);

            float fV_asterix_b_max_screw = MathF.Sqrt(MathF.Sqrt(MathF.Pow2(fV_asterix_b_max_screw_Mxu) + MathF.Pow2(fV_asterix_b_max_screw_Vyv)) + MathF.Pow2(fV_asterix_b_max_screw_N));

            float fEta_Vb_5424 = eq.Eq_5424_1__(fV_asterix_b_max_screw, 0.5f, fVb_MainMember);
            fEta_max = MathF.Max(fEta_max, fEta_Vb_5424);

            // 5.4.2.5 Connection shear as limited by end distance
            float fe = 0.03f; // TODO - temporary - urcit min vzdialenost skrutky od okraja plechu alebo prierezu
            float fV_fv_MainMember = eq.Eq_5425_2__(ft_2_crscmainMember, fe, crsc_mainMember.m_Mat.Get_f_uk_by_thickness((float)crsc_mainMember.t_min));
            float fV_fv_SecondaryMember = eq.Eq_5425_2__(ft_2_crscsecMember, fe, crsc_secMember.m_Mat.Get_f_uk_by_thickness((float)crsc_secMember.t_min));
            float fV_fv_Plate = eq.Eq_5425_2__(plate.fThickness_tz, fe, plate.m_Mat.Get_f_uk_by_thickness((float)plate.fThickness_tz));

            // Distance to an end of the connected part is parallel to the line of the applied force
            // Nemalo by rozhodovat pre moment (skrutka namahana rovnobezne s okrajom je uprostred plechu) ale moze rozhovat pre N a V
            float fV_asterix_fv = MathF.Sqrt(MathF.Pow2(fV_asterix_b_max_screw_Vyv) + MathF.Pow2(fV_asterix_b_max_screw_N));

            float fEta_V_fv_5425_MainMember = fV_asterix_fv / fV_fv_MainMember;
            float fEta_V_fv_5425_SecondaryMember = fV_asterix_fv / fV_fv_MainMember;
            float fEta_V_fv_5425_Plate = fV_asterix_fv / fV_fv_MainMember;

            float fEta_V_fv_5425 = MathF.Max(fEta_V_fv_5425_MainMember, fEta_V_fv_5425_SecondaryMember, fEta_V_fv_5425_Plate);
            fEta_max = MathF.Max(fEta_max, fEta_V_fv_5425);

            // Validation - negative design ratio
            if (fEta_Vb_5424 < 0 ||
                fEta_V_fv_5425 < 0
)            {
                throw new Exception("Design ratio is invalid!");
            }

            // Validation - inifiniti design ratio
            if (fEta_max > 9e+10)
            {
                throw new Exception("Design ratio is invalid!");
            }

            int iNumberOfDecimalPlaces = 3;
            if (bIsDebugging)
                MessageBox.Show("Calculation finished.\n"
                              + "Design Ratio η = " + Math.Round(fEta_Vb_5424, iNumberOfDecimalPlaces) + " [-]" + "\t" + " 5.4.2.4" + "\n"
                              + "Design Ratio η = " + Math.Round(fEta_V_fv_5425, iNumberOfDecimalPlaces) + " [-]" + "\t" + " 5.4.2.5" + "\n"
                              + "Design Ratio η max = " + Math.Round(fEta_max, iNumberOfDecimalPlaces) + " [-]");
        }
    }
}
