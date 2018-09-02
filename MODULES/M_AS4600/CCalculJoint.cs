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

            float ft_1_plate = (float)plate.fThickness_tz;
            float ft_2_crscmainMember = (float)crsc_mainMember.t_min;
            float ft_2_crscsecMember = (float)crsc_secMember.t_min;

            float ff_yk_1_plate = plate.m_Mat.Get_f_yk_by_thickness((float)ft_1_plate);
            float ff_uk_1_plate = plate.m_Mat.Get_f_uk_by_thickness((float)ft_1_plate);

            float ff_yk_2_MainMember = crsc_mainMember.m_Mat.Get_f_yk_by_thickness(ft_2_crscmainMember);
            float ff_uk_2_MainMember = crsc_mainMember.m_Mat.Get_f_uk_by_thickness(ft_2_crscmainMember);

            float ff_yk_2_SecondaryMember = crsc_secMember.m_Mat.Get_f_yk_by_thickness(ft_2_crscsecMember);
            float ff_uk_2_SecondaryMember = crsc_secMember.m_Mat.Get_f_uk_by_thickness(ft_2_crscsecMember);


            /// Bending Joint apex, knee joint

            float fVb_MainMember = eq.Get_Vb_5424(ft_1_plate, ft_2_crscmainMember, screw.m_fDiameter, ff_uk_1_plate, ff_uk_2_MainMember);
            float fVb_SecondaryMember = eq.Get_Vb_5424(ft_1_plate, ft_2_crscsecMember, screw.m_fDiameter, ff_uk_1_plate, ff_uk_2_SecondaryMember);

            int fNumberOfScrewsInTension = 0;
            int fNumberOfScrewsInShear = joint_temp.m_arrConnectors.Length; // Temporary

            float fEta_MainMember = sDIF.fV_zv / (fNumberOfScrewsInShear * fVb_MainMember);
            float fEta_SecondaryMember = sDIF.fV_zv / (fNumberOfScrewsInShear * fVb_SecondaryMember);

            float fN_oneside = 0.5f * sDIF_temp.fN;
            float fM_xu_oneside = 0.5f * sDIF_temp.fM_yu;
            float fV_yv_oneside = 0.5f * sDIF_temp.fV_zv;

            float fMb_MainMember_oneside_plastic = 0;
            float fMb_SecondaryMember_oneside_plastic = 0;

            float fSumri2tormax = 0; // F_max = Mxu / (Σ ri^2 / r_max)
            CConCom_Plate_KA a = (CConCom_Plate_KA)plate;

            float fr_max = MathF.Max(a.HolesCenterRadii);

            // 5.4.2.4 Tilting and hole bearing
            // Bending - Calculate shear strength of plate connection - main member
            for (int i = 0; i < a.IHolesNumber / a.INumberOfCircleJoints; i++)
            {
                fMb_MainMember_oneside_plastic += a.HolesCenterRadii[i] * fVb_MainMember;
                fMb_SecondaryMember_oneside_plastic += a.HolesCenterRadii[/*a.IHolesNumber / 2 +*/ i] * fVb_SecondaryMember;

                fSumri2tormax += MathF.Pow2(a.HolesCenterRadii[i]) / fr_max;
            }

            // Plastic resistance (Design Ratio)
            float fEta_Mb_MainMember_oneside_plastic = Math.Abs(fM_xu_oneside) / fMb_MainMember_oneside_plastic;
            fEta_max = MathF.Max(fEta_max, fEta_Mb_MainMember_oneside_plastic);
            float fEta_Mb_SecondaryMember_oneside_plastic = Math.Abs(fM_xu_oneside) / fMb_SecondaryMember_oneside_plastic;
            fEta_max = MathF.Max(fEta_max, fEta_Mb_SecondaryMember_oneside_plastic);

            // Elastic resistance
            float fV_asterix_b_max_screw_Mxu = Math.Abs(fM_xu_oneside) / fSumri2tormax;
            float fV_asterix_b_max_screw_Vyv = Math.Abs(sDIF.fV_zv) / (a.IHolesNumber / a.INumberOfCircleJoints);
            float fV_asterix_b_max_screw_N = Math.Abs(sDIF.fN) / (a.IHolesNumber / a.INumberOfCircleJoints);

            float fV_asterix_b_max_screw = MathF.Sqrt(MathF.Sqrt(MathF.Pow2(fV_asterix_b_max_screw_Mxu) + MathF.Pow2(fV_asterix_b_max_screw_Vyv)) + MathF.Pow2(fV_asterix_b_max_screw_N));

            float fEta_Vb_5424 = eq.Eq_5424_1__(fV_asterix_b_max_screw, 0.5f, fVb_MainMember);
            fEta_max = MathF.Max(fEta_max, fEta_Vb_5424);

            // 5.4.2.5 Connection shear as limited by end distance
            float fe = 0.03f; // TODO - temporary - urcit min vzdialenost skrutky od okraja plechu alebo prierezu
            float fV_fv_MainMember = eq.Eq_5425_2__(ft_2_crscmainMember, fe, ff_uk_2_MainMember);
            float fV_fv_SecondaryMember = eq.Eq_5425_2__(ft_2_crscsecMember, fe, ff_uk_2_SecondaryMember);
            float fV_fv_Plate = eq.Eq_5425_2__(ft_1_plate, fe, ff_uk_1_plate);

            // Distance to an end of the connected part is parallel to the line of the applied force
            // Nemalo by rozhodovat pre moment (skrutka namahana rovnobezne s okrajom je uprostred plechu) ale moze rozhovat pre N a V
            float fV_asterix_fv = MathF.Sqrt(MathF.Pow2(fV_asterix_b_max_screw_Vyv) + MathF.Pow2(fV_asterix_b_max_screw_N));

            float fEta_V_fv_5425_MainMember = eq.Eq_5425_1__(fV_asterix_fv, fV_fv_MainMember, ff_uk_2_MainMember, ff_yk_2_MainMember);
            float fEta_V_fv_5425_SecondaryMember = eq.Eq_5425_1__(fV_asterix_fv, fV_fv_SecondaryMember, ff_uk_2_SecondaryMember, ff_yk_2_SecondaryMember);
            float fEta_V_fv_5425_Plate = eq.Eq_5425_1__(fV_asterix_fv, fV_fv_Plate, ff_uk_1_plate, ff_yk_1_plate);

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


            /// Purlins, girts .....



            // 5.4.2.3 Tension in the connected part
            float fA_n_MainMember = (float)crsc_mainMember.A_g - plate.INumberOfConnectorsInSection * 2 * screw.m_fDiameter; // TODO - spocitat presne podla poctu a rozmeru otvorov v jednom reze
            float fN_t_section_MainMember = eq.Eq_5423_2__(screw.m_fDiameter, plate.S_f_min, fA_n_MainMember, ff_uk_2_MainMember);
            float fEta_N_t_5423_MainMember = eq.Eq_5423_1__(sDIF_temp.fN_t, 0.65f, fN_t_section_MainMember);
            fEta_max = MathF.Max(fEta_max, fEta_N_t_5423_MainMember);

            float fA_n_SecondaryMember = (float)crsc_secMember.A_g - plate.INumberOfConnectorsInSection * 2 * screw.m_fDiameter; // TODO - spocitat presne podla poctu a rozmeru otvorov v jednom reze
            float fN_t_section_SecondaryMember = eq.Eq_5423_2__(screw.m_fDiameter, plate.S_f_min, fA_n_SecondaryMember, ff_uk_2_SecondaryMember);
            float fEta_N_t_5423_SecondaryMember = eq.Eq_5423_1__(sDIF_temp.fN_t, 0.65f, fN_t_section_SecondaryMember);
            fEta_max = MathF.Max(fEta_max, fEta_N_t_5423_SecondaryMember);

            // 5.4.3 Screwed connections in tension
            // 5.4.3.2 Pull-out and pull-over (pull-through)

            // K vytiahnutiu alebo pretlaceniu moze dost v pripojeni k main member alebo pri posobeni sily Vx(Vy) na secondary member

            float fN_t_5432_MainMember = eq.Get_Nt_5432(screw.Type, ft_1_plate, ft_2_crscmainMember, screw.m_fDiameter, screw.D_h_headdiameter,  screw.T_w_washerthickness, screw.D_w_washerdiameter, ff_uk_1_plate, ff_uk_2_MainMember);
            float fEta_N_t_5432_MainMember = eq.Eq_5432_1__(sDIF_temp.fN_t / fNumberOfScrewsInTension, 0.5f, fN_t_5432_MainMember);
            fEta_max = MathF.Max(fEta_max, fEta_N_t_5432_MainMember);

            // 5.4.3.4 Screwed connections subject to combined shear and pull-over

            // Check conditions
            bool bIsFilFilled_5434 = eq.Conditions_5434_FulFilled(ft_1_plate, ft_2_crscmainMember, screw.T_w_washerthickness, screw.D_w_washerdiameter, screw.m_iGauge, ff_uk_1_plate);

            if (!bIsFilFilled_5434)
                throw new Exception("Conditions acc. to cl 5.4.3.4 are not fulfilled!");
            
            /*
            Vb and Nov shall be determined in accordance with Clauses 5.4.2.4 and 5.4.3.2(b), respectively.In using Clause 5.4.2.4, only Equation 5.4.2.4(6) needs to be considered.
            A value of Φ = 0.65 shall be used.
            */

            // Pripoj plechu k hlavnemu prutu
            // Tension and shear
            float fC_for5434_MainMember = eq.Get_C_Tab_5424(screw.m_fDiameter, ft_2_crscmainMember);
            float fV_b_for5434_MainMember = eq.Eq_5424_6__(fC_for5434_MainMember, ft_2_crscmainMember, screw.m_fDiameter, ff_uk_2_MainMember);
            float fd_w_for5434_plate = eq.Get_d_aphostrof_w(screw.Type, ft_1_plate, screw.D_h_headdiameter, screw.T_w_washerthickness, screw.D_w_washerdiameter);
            float fN_ov_for5434_plate = eq.Eq_5432_2__(ft_1_plate, fd_w_for5434_plate, ff_uk_1_plate);

            bool bIsEccentricallyLoadedJoint = false;

            if (bIsEccentricallyLoadedJoint)
                fN_ov_for5434_plate *= 0.5f; // Use 50% of resistance value in case of eccentrically loaded connection

            float fV_asterix_b_for5434_MainMember = MathF.Sqrt(MathF.Pow2(sDIF_temp.fV_yu / fNumberOfScrewsInTension) + MathF.Pow2(sDIF_temp.fV_zv / fNumberOfScrewsInTension));
            float fEta_5434_MainMember = eq.Eq_5434____(fV_asterix_b_for5434_MainMember, sDIF_temp.fN_t / fNumberOfScrewsInTension, 0.65f, fV_b_for5434_MainMember, fN_ov_for5434_plate);
            fEta_max = MathF.Max(fEta_max, fEta_5434_MainMember);

            // 5.4.3.5 Screwed connections subject to combined shear and pull-out

            // Check conditions
            bool bIsFilFilled_5435 = eq.Conditions_5435_FulFilled(ft_2_crscmainMember, screw.m_iGauge, ff_yk_2_MainMember, ff_uk_2_MainMember);

            if (!bIsFilFilled_5435)
                throw new Exception("Conditions acc. to cl 5.4.3.5 are not fulfilled!");

            /*
            Vb and Nou shall be determined in accordance with Clauses 5.4.2.4 and 5.4.3.2(a), respectively. In using Clause 5.4.3.2, only Equation 5.4.3.2(2) needs to be considered.
            A value of Φ = 0.60 shall be used.
            */

            // Pripoj k hlavnemu prutu
            float fV_b_for5435_MainMember = eq.Get_Vb_5424(ft_1_plate, ft_2_crscmainMember, screw.m_fDiameter, ff_uk_1_plate, ff_uk_2_MainMember);
            float fN_ou_for5435_MainMember = eq.Eq_5432_2__(ft_2_crscmainMember,screw.m_fDiameter, ff_uk_2_MainMember);

            float fV_asterix_b_for5435_MainMember = MathF.Sqrt(MathF.Pow2(sDIF_temp.fV_yu / fNumberOfScrewsInTension) + MathF.Pow2(sDIF_temp.fV_zv / fNumberOfScrewsInTension));
            float fEta_5435_MainMember = eq.Eq_5435____(fV_asterix_b_for5435_MainMember, sDIF_temp.fN_t / fNumberOfScrewsInTension, 0.6f, fV_b_for5435_MainMember, fN_ou_for5435_MainMember);
            fEta_max = MathF.Max(fEta_max, fEta_5435_MainMember);

            // TODO - smyk na okraji

            // TODO
            // Pripoj plechu sekundarneho pruta
            // Shear

            // TODO
            // Overit
            // 5.4.3.6 ??????????????????
        }
    }
}
