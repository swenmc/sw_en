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
        AS_4600 eq = new AS_4600();
        public CConnectionJointTypes joint;
        public designInternalForces sDIF;
        bool bIsDebugging;

        CScrew screw;
        CPlate plate;
        CCrSc_TW crsc_mainMember;
        CCrSc_TW crsc_secMember;

        float ft_1_plate;
        float ft_2_crscmainMember;
        float ft_2_crscsecMember;

        float ff_yk_1_plate;
        float ff_uk_1_plate;

        float ff_yk_2_MainMember;
        float ff_uk_2_MainMember;

        float ff_yk_2_SecondaryMember;
        float ff_uk_2_SecondaryMember;

        public float fEta_max = 0;

        public CCalculJoint(bool bIsDebugging_temp, CConnectionJointTypes joint_temp, designInternalForces sDIF_temp)
        {
            if (joint_temp == null)
            {
                throw new ArgumentNullException("Joint object is not defined");
            }

            bIsDebugging = bIsDebugging_temp;
            joint = joint_temp;
            sDIF = sDIF_temp;

            CalculateDesignRatio(bIsDebugging, joint, sDIF);
        }

        public void CalculateDesignRatio(bool bIsDebugging, CConnectionJointTypes joint_temp, designInternalForces sDIF_temp)
        {
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
            screw = joint_temp.m_arrPlates[0].ScrewArrangement.Screws[0]; // Parametre prvej skrutky prveho plechu
            plate = joint_temp.m_arrPlates[0];
            crsc_mainMember = (CCrSc_TW)joint_temp.m_MainMember.CrScStart;

            ft_1_plate = (float)plate.Ft;
            ft_2_crscmainMember = (float)crsc_mainMember.t_min;

            ff_yk_1_plate = plate.m_Mat.Get_f_yk_by_thickness((float)ft_1_plate);
            ff_uk_1_plate = plate.m_Mat.Get_f_uk_by_thickness((float)ft_1_plate);

            ff_yk_2_MainMember = crsc_mainMember.m_Mat.Get_f_yk_by_thickness(ft_2_crscmainMember);
            ff_uk_2_MainMember = crsc_mainMember.m_Mat.Get_f_uk_by_thickness(ft_2_crscmainMember);

            if (joint_temp.m_SecondaryMembers != null || joint_temp.m_SecondaryMembers.Length > 0) // Some secondary member exists (otherwise it is base plate connection)
            {
                crsc_secMember = (CCrSc_TW)joint_temp.m_SecondaryMembers[0].CrScStart;
                ft_2_crscsecMember = (float)crsc_secMember.t_min;
                ff_yk_2_SecondaryMember = crsc_secMember.m_Mat.Get_f_yk_by_thickness(ft_2_crscsecMember);
                ff_uk_2_SecondaryMember = crsc_secMember.m_Mat.Get_f_uk_by_thickness(ft_2_crscsecMember);
            }

            // 5.4.1
            if (!(0.003f <= screw.Diameter_thread && screw.Diameter_thread <= 0.007f))
                throw new Exception("Conditions acc. to cl 5.4.1 are not fulfilled!");

            // TODO Ondrej - Zistit typ vstupujuceho objektu (konkretny potomok CConnectionJointTypes ... ) a na zaklade toho spustit posudenie pre dany typ spoja
            // Pripadne prist s lepsim napadom ako to ma byt usporiadane, kazdy spoj ma nejaku geometriu, pre ktoru by malo byt specificke posudenie, niektore typy spojov ju maju velmi podobnu, takze sa da pouzit rovnaka metoda

            // Rozdelit spoje podla typu triedy spoja

            // Apex - CConnectionJoint_A001, plates serie J
            // Knee - CConnectionJoint_B001, plates serie K
            // Ostatne spoje CConnectionJoint_T001, plates serie CConCom_Plate_F_or_L, L resp. LL

            // Kotvenie k zakladu
            // Main Columns - CConnectionJoint_TA01, plates serie B
            // Other Columns  CConnectionJoint_TB01, plates serie B

            Type t = joint_temp.GetType();

            if (t == typeof(CConnectionJoint_A001) || t == typeof(CConnectionJoint_B001))
                CalculateDesignRatioApexOrKneeJoint(joint_temp, sDIF_temp); // Apex or Knee Joint
            else if (joint_temp.m_SecondaryMembers != null)
                CalculateDesignRatioGirtOrPurlinJoint(joint_temp, sDIF_temp); // purlin, girt
            else
                CalculateDesignRatioBaseJoint(joint_temp, sDIF_temp); // base plate
        }

        public void CalculateDesignRatioApexOrKneeJoint(CConnectionJointTypes joint_temp, designInternalForces sDIF_temp)
        {
            /// Bending Joint apex, knee joint

            int iNumberOfPlatesInJoint = joint.m_arrPlates.Length;

            float fN_oneside = 1f / iNumberOfPlatesInJoint * sDIF_temp.fN;
            float fM_xu_oneside = 1f / iNumberOfPlatesInJoint * sDIF_temp.fM_yu;
            float fV_yv_oneside = 1f / iNumberOfPlatesInJoint * sDIF_temp.fV_zv;

            // Plate design

            // Plate tension design
            float fA_n_plate = plate.fA_n;
            float fN_t_plate = eq.Eq_5423_2__(screw.Diameter_thread, plate.S_f_min, fA_n_plate, ff_uk_1_plate);
            float fEta_N_t_5423_plate = eq.Eq_5423_1__(fN_oneside, 0.65f, fN_t_plate);
            fEta_max = MathF.Max(fEta_max, fEta_N_t_5423_plate);

            // Plate shear resistance
            float fA_vn_yv_plate = plate.fA_vn_zv;
            float fV_y_yv_plate = eq.Eq_723_5___(fA_vn_yv_plate, ff_yk_1_plate);
            float fEta_V_yv_3341_plate = eq.Eq_3341____(fV_yv_oneside, 0.65f, fV_y_yv_plate);
            fEta_max = MathF.Max(fEta_max, fEta_V_yv_3341_plate);

            // Plate bending resistance
            float fM_xu_resitance_plate = eq.Eq_7222_4__(joint.m_arrPlates[0].fW_el_yu, ff_yk_1_plate);
            float fEta_Mb_plate, fDesignReistance_M_plate;
            eq.Eq_723_10__(Math.Abs(fM_xu_oneside), 0.65f, fM_xu_resitance_plate, out fDesignReistance_M_plate, out fEta_Mb_plate);
            fEta_max = MathF.Max(fEta_max, fEta_Mb_plate);

            // Connection -shear force design
            // Shear in connection
            float fVb_MainMember = eq.Get_Vb_5424(ft_1_plate, ft_2_crscmainMember, screw.Diameter_thread, ff_uk_1_plate, ff_uk_2_MainMember);
            float fVb_SecondaryMember = eq.Get_Vb_5424(ft_1_plate, ft_2_crscsecMember, screw.Diameter_thread, ff_uk_1_plate, ff_uk_2_SecondaryMember);

            int iNumberOfScrewsInShear = joint_temp.m_arrPlates[0].ScrewArrangement.Screws.Length; // Temporary

            float fEta_MainMember = sDIF.fV_zv / (iNumberOfScrewsInShear * fVb_MainMember);
            float fEta_SecondaryMember = sDIF.fV_zv / (iNumberOfScrewsInShear * fVb_SecondaryMember);

            float fMb_MainMember_oneside_plastic = 0;
            float fMb_SecondaryMember_oneside_plastic = 0;

            float fSumri2tormax = 0; // F_max = Mxu / (Σ ri^2 / r_max)

            // TEMPORARY
            // Moze sa lisit podla rozneho usporiadania skrutiek a vzdialenosti skrutiek od ich fiktivneho taziska (mali by byt symetricky)
            CConCom_Plate_KA a = (CConCom_Plate_KA)plate; // TODO - Ondrej potrebujeme sa dostat k property konkretneho objektu, ktory vstupil do funkcie (potomok CPlate)

            float fr_max = MathF.Max(a.HolesCenterRadii);

            // 5.4.2.4 Tilting and hole bearing
            // Bending - Calculate shear strength of plate connection - main member
            for (int i = 0; i < a.ScrewArrangement.IHolesNumber / ((CScrewArrangementCircleApexOrKnee)a.ScrewArrangement).INumberOfCircleGroupsInJoint; i++)
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
            float fV_asterix_b_max_screw_Vyv = Math.Abs(sDIF.fV_zv) / (a.ScrewArrangement.IHolesNumber / ((CScrewArrangementCircleApexOrKnee)a.ScrewArrangement).INumberOfCircleGroupsInJoint);
            float fV_asterix_b_max_screw_N = Math.Abs(sDIF.fN) / (a.ScrewArrangement.IHolesNumber / ((CScrewArrangementCircleApexOrKnee)a.ScrewArrangement).INumberOfCircleGroupsInJoint);

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
                fEta_V_fv_5425 < 0)
            {
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

            // Tension in members
            // 5.4.2.3 Tension in the connected part
            float fA_n_MainMember = (float)crsc_mainMember.A_g - plate.INumberOfConnectorsInSection * 2 * screw.Diameter_thread; // TODO - spocitat presne podla poctu a rozmeru otvorov v jednom reze
            float fN_t_section_MainMember = eq.Eq_5423_2__(screw.Diameter_thread, plate.S_f_min, fA_n_MainMember, ff_uk_2_MainMember);
            float fEta_N_t_5423_MainMember = eq.Eq_5423_1__(sDIF_temp.fN_t, 0.65f, fN_t_section_MainMember);
            fEta_max = MathF.Max(fEta_max, fEta_N_t_5423_MainMember);

            float fA_n_SecondaryMember = (float)crsc_secMember.A_g - plate.INumberOfConnectorsInSection * 2 * screw.Diameter_thread; // TODO - spocitat presne podla poctu a rozmeru otvorov v jednom reze
            float fN_t_section_SecondaryMember = eq.Eq_5423_2__(screw.Diameter_thread, plate.S_f_min, fA_n_SecondaryMember, ff_uk_2_SecondaryMember);
            float fEta_N_t_5423_SecondaryMember = eq.Eq_5423_1__(sDIF_temp.fN_t, 0.65f, fN_t_section_SecondaryMember);
            fEta_max = MathF.Max(fEta_max, fEta_N_t_5423_SecondaryMember);
        }

        public void CalculateDesignRatioGirtOrPurlinJoint(CConnectionJointTypes joint_temp, designInternalForces sDIF_temp)
        {
            bool bDisplayWarningForContitions5434and5435 = false;
            /// Purlins, girts .....
            int iNumberOfScrewsInTension = plate.ScrewArrangement.IHolesNumber;

            // 5.4.3 Screwed connections in tension
            // 5.4.3.2 Pull-out and pull-over (pull-through)

            // K vytiahnutiu alebo pretlaceniu moze dost v pripojeni k main member alebo pri posobeni sily Vx(Vy) na secondary member (to asi zanedbame)

            float fN_t_5432_MainMember = eq.Get_Nt_5432(screw.Type, ft_1_plate, ft_2_crscmainMember, screw.Diameter_thread, screw.D_h_headdiameter, screw.T_w_washerthickness, screw.D_w_washerdiameter, ff_uk_1_plate, ff_uk_2_MainMember);
            float fEta_N_t_5432_MainMember = eq.Eq_5432_1__(sDIF_temp.fN_t / iNumberOfScrewsInTension, 0.5f, fN_t_5432_MainMember);
            fEta_max = MathF.Max(fEta_max, fEta_N_t_5432_MainMember);

            // 5.4.3.4 Screwed connections subject to combined shear and pull-over

            // Check conditions
            bool bIsFulFilled_5434 = eq.Conditions_5434_FulFilled(ft_1_plate, ft_2_crscmainMember, screw.T_w_washerthickness, screw.D_w_washerdiameter, screw.Gauge, ff_uk_1_plate);

            if (bDisplayWarningForContitions5434and5435 && !bIsFulFilled_5434)
                throw new Exception("Conditions acc. to cl 5.4.3.4 are not fulfilled!");

            /*
            Vb and Nov shall be determined in accordance with Clauses 5.4.2.4 and 5.4.3.2(b), respectively. In using Clause 5.4.2.4, only Equation 5.4.2.4(6) needs to be considered.
            A value of Φ = 0.65 shall be used.
            */

            // Pripoj plechu k hlavnemu prutu
            // Tension and shear
            float fC_for5434_MainMember = eq.Get_C_Tab_5424(screw.Diameter_thread, ft_2_crscmainMember);
            float fV_b_for5434_MainMember = eq.Eq_5424_6__(fC_for5434_MainMember, ft_2_crscmainMember, screw.Diameter_thread, ff_uk_2_MainMember); // Eq. 5.4.2.4(6)
            float fd_w_for5434_plate = eq.Get_d_aphostrof_w(screw.Type, ft_1_plate, screw.D_h_headdiameter, screw.T_w_washerthickness, screw.D_w_washerdiameter);
            float fN_ov_for5434_plate = eq.Eq_5432_3__(ft_1_plate, screw.D_w_washerdiameter, ff_uk_1_plate); // 5.4.3.2(b) Eq. 5.4.3.2(3) - Nov

            bool bIsEccentricallyLoadedJoint = false;

            if (bIsEccentricallyLoadedJoint)
                fN_ov_for5434_plate *= 0.5f; // Use 50% of resistance value in case of eccentrically loaded connection

            float fV_asterix_b_for5434_MainMember = MathF.Sqrt(MathF.Pow2(sDIF_temp.fV_yu / iNumberOfScrewsInTension) + MathF.Pow2(sDIF_temp.fV_zv / iNumberOfScrewsInTension));
            float fEta_5434_MainMember = eq.Eq_5434____(fV_asterix_b_for5434_MainMember, sDIF_temp.fN_t / iNumberOfScrewsInTension, 0.65f, fV_b_for5434_MainMember, fN_ov_for5434_plate);
            fEta_max = MathF.Max(fEta_max, fEta_5434_MainMember);

            // 5.4.3.5 Screwed connections subject to combined shear and pull-out

            // Check conditions
            bool bIsFulFilled_5435 = eq.Conditions_5435_FulFilled(ft_2_crscmainMember, screw.Gauge, ff_yk_2_MainMember, ff_uk_2_MainMember);

            if (bDisplayWarningForContitions5434and5435 && !bIsFulFilled_5435)
                throw new Exception("Conditions acc. to cl 5.4.3.5 are not fulfilled!");

            /*
            Vb and Nou shall be determined in accordance with Clauses 5.4.2.4 and 5.4.3.2(a), respectively. In using Clause 5.4.3.2, only Equation 5.4.3.2(2) needs to be considered.
            A value of Φ = 0.60 shall be used.
            */

            // Pripoj k hlavnemu prutu
            float fV_b_for5435_MainMember = eq.Get_Vb_5424(ft_1_plate, ft_2_crscmainMember, screw.Diameter_thread, ff_uk_1_plate, ff_uk_2_MainMember);
            float fN_ou_for5435_MainMember = eq.Eq_5432_2__(ft_2_crscmainMember, screw.Diameter_thread, ff_uk_2_MainMember); // 5.4.3.2(a) Eq. 5.4.3.2(2) - Nou

            float fV_asterix_b_for5435_MainMember = MathF.Sqrt(MathF.Pow2(sDIF_temp.fV_yu / iNumberOfScrewsInTension) + MathF.Pow2(sDIF_temp.fV_zv / iNumberOfScrewsInTension));
            float fEta_5435_MainMember = eq.Eq_5435____(fV_asterix_b_for5435_MainMember, sDIF_temp.fN_t / iNumberOfScrewsInTension, 0.6f, fV_b_for5435_MainMember, fN_ou_for5435_MainMember);
            fEta_max = MathF.Max(fEta_max, fEta_5435_MainMember);

            // 5.4.2.5 Connection shear as limited by end distance
            float fe_Plate = 0.03f; // TODO - temporary - urcit min vzdialenost skrutky od okraja plechu

            // Distance to an end of the connected part is parallel to the line of the applied force
            float fV_asterix_fv_plate = Math.Abs(sDIF_temp.fV_zv / iNumberOfScrewsInTension);
            float fV_fv_Plate = eq.Eq_5425_2__(ft_1_plate, fe_Plate, ff_uk_1_plate);
            float fEta_V_fv_5425_Plate = eq.Eq_5425_1__(fV_asterix_fv_plate, fV_fv_Plate, ff_uk_1_plate, ff_yk_1_plate);
            fEta_max = MathF.Max(fEta_max, fEta_V_fv_5425_Plate);

            // TODO
            // Overit podla coho sa navrhuju samotne skrutky !!!! Mame k dispozicii unosnosti z experimentov ???
            /*
            5.4.2.6 Screws in shear
            The design shear capacity of the screw shall be determined by testing in accordance with Section 8.

            5.4.3.3 Screws in tension
            The tensile capacity of the screw shall be determined by testing in accordance with Section 8.

            5.4.3.6 Screws subject to combined shear and tension
            A screw required to resist simultaneously a design shear force and a design tensile where Vscrew and Nscrew shall be determined by testing in accordance with Section 8.
            */

            // Plate design
            int iNumberOfPlatesInJoint = joint.m_arrPlates.Length;

            float fN_oneside = 1f / iNumberOfPlatesInJoint * sDIF_temp.fN;
            float fV_yv_oneside = 1f / iNumberOfPlatesInJoint * sDIF_temp.fV_zv;

            // Plate tension design
            float fA_n_plate = plate.fA_n;
            float fN_t_plate = eq.Eq_5423_2__(screw.Diameter_thread, plate.S_f_min, fA_n_plate, ff_uk_1_plate);
            float fEta_N_t_5423_plate = eq.Eq_5423_1__(fN_oneside, 0.65f, fN_t_plate);
            fEta_max = MathF.Max(fEta_max, fEta_N_t_5423_plate);

            // Plate shear resistance
            float fA_vn_yv_plate = plate.fA_vn_zv;
            float fV_y_yv_plate = eq.Eq_723_5___(fA_vn_yv_plate, ff_yk_1_plate);
            float fEta_V_yv_3341_plate = eq.Eq_3341____(fV_yv_oneside, 0.65f, fV_y_yv_plate);
            fEta_max = MathF.Max(fEta_max, fEta_V_yv_3341_plate);

            // Pripoj plechu sekundarneho pruta
            int iNumberOfScrewsInConnectionOfSecondaryMember = 16; // Temporary (pocet plechov * pocet skrutiek v jednom ramene pripojneho plechu = 2 * 8)
            // Shear
            float fV_asterix_b_SecondaryMember = MathF.Sqrt(MathF.Pow2(sDIF_temp.fV_zv / iNumberOfScrewsInConnectionOfSecondaryMember) + MathF.Pow2(sDIF_temp.fN / iNumberOfScrewsInConnectionOfSecondaryMember));
            float fVb_SecondaryMember = eq.Get_Vb_5424(ft_1_plate, ft_2_crscsecMember, screw.Diameter_thread, ff_uk_1_plate, ff_uk_2_SecondaryMember);
            float fEta_Vb_5424_SecondaryMember = eq.Eq_5424_1__(fV_asterix_b_SecondaryMember, 0.5f, fVb_SecondaryMember);
            fEta_max = MathF.Max(fEta_max, fEta_Vb_5424_SecondaryMember);

            // Tension force in secondary member, distance between end of member and screw
            float fe_SecondaryMember = 0.03f; // TODO - temporary - urcit min vzdialenost skrutky od okraja nosnika
            float fV_asterix_fv_SecondaryMember = Math.Abs(sDIF_temp.fN / iNumberOfScrewsInConnectionOfSecondaryMember);
            float fV_fv_SecondaryMember = eq.Eq_5425_2__(ft_2_crscsecMember, fe_SecondaryMember, ff_uk_2_SecondaryMember);
            float fEta_V_fv_5425_SecondaryMember = eq.Eq_5425_1__(fV_asterix_fv_SecondaryMember, fV_fv_SecondaryMember, ff_uk_2_SecondaryMember, ff_yk_2_SecondaryMember);
            fEta_max = MathF.Max(fEta_max, fEta_V_fv_5425_SecondaryMember);
        }

        public void CalculateDesignRatioBaseJoint(CConnectionJointTypes joint_temp, designInternalForces sDIF_temp)
        {
            // Not implemented
            fEta_max = 0;
        }
    }
}
