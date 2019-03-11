using System;
using System.Windows.Forms;
using BaseClasses;
using MATH;
using CRSC;
using System.Collections.Generic;
using System.Data;
using M_NZS3101;

namespace M_AS4600
{
    public class CCalculJoint
    {
        AS_4600 eq = new AS_4600(); // TODO Ondrej - toto by sa asi malo prerobit na staticke triedy, je to nejaka kniznica metod s rovnicami a tabulkovymi hodnotami
        NZS_3101 eq_concrete = new NZS_3101(); // TODO Ondrej - toto by sa asi malo prerobit na staticke triedy, je to nejaka kniznica metod s rovnicami a tabulkovymi hodnotami

        public CConnectionJointTypes joint;
        public designInternalForces sDIF;
        bool bIsDebugging;

        CScrew screw;
        CPlate plate;
        CCrSc_TW crsc_mainMember;
        CCrSc_TW crsc_secMember;

        public float ft_1_plate;
        public float ft_2_crscmainMember;
        public float ft_2_crscsecMember;

        public float ff_yk_1_plate;
        public float ff_uk_1_plate;

        public float ff_yk_2_MainMember;
        public float ff_uk_2_MainMember;

        public float ff_yk_2_SecondaryMember;
        public float ff_uk_2_SecondaryMember;

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

            // Validation
            // Check that main member is defined
            if (joint_temp.m_MainMember == null)
                throw new ArgumentNullException("Error " + "Joint No: " + joint_temp.ID + " Main member is not defined.");

            // Check that some screws exist in the connection
            if (joint_temp.m_arrPlates[0].ScrewArrangement == null || joint_temp.m_arrPlates[0].ScrewArrangement.Screws == null)
                return; // Invalid data, joint without connectors

            //df = nominal screw diameter
            screw = joint_temp.m_arrPlates[0].ScrewArrangement.referenceScrew; // Parametre prvej skrutky prveho plechu
            plate = joint_temp.m_arrPlates[0];
            crsc_mainMember = (CCrSc_TW)joint_temp.m_MainMember.CrScStart;

            ft_1_plate = (float)plate.Ft;
            ft_2_crscmainMember = (float)crsc_mainMember.t_min;

            // Validate thickness of elements
            if (ft_1_plate < 0.0001f || ft_2_crscmainMember < 0.0001f)
            {
                throw new Exception("Invalid component thickness. Check thickness cross-section or plate.");
            }

            ff_yk_1_plate = plate.m_Mat.Get_f_yk_by_thickness((float)ft_1_plate);
            ff_uk_1_plate = plate.m_Mat.Get_f_uk_by_thickness((float)ft_1_plate);

            ff_yk_2_MainMember = crsc_mainMember.m_Mat.Get_f_yk_by_thickness(ft_2_crscmainMember);
            ff_uk_2_MainMember = crsc_mainMember.m_Mat.Get_f_uk_by_thickness(ft_2_crscmainMember);

            if (joint_temp.m_SecondaryMembers != null && joint_temp.m_SecondaryMembers.Length > 0) // Some secondary member exists (otherwise it is base plate connection)
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

            if (joint_temp is CConnectionJoint_A001 || joint_temp is CConnectionJoint_B001)
                CalculateDesignRatioApexOrKneeJoint(joint_temp, sDIF_temp); // Apex or Knee Joint
            else if (joint_temp.m_SecondaryMembers != null)
            {
                if(joint_temp is CConnectionJoint_T001 || joint_temp is CConnectionJoint_T002 || joint_temp is CConnectionJoint_T003)
                    CalculateDesignRatioGirtOrPurlinJoint(joint_temp, sDIF_temp); // purlin, girt or eave purlin
                else if(joint_temp is CConnectionJoint_S001) // Front / back column connection to the main rafter
                    CalculateDesignRatioFrontOrBackColumnToMainRafterJoint(joint_temp, sDIF_temp);
                else
                {
                    // Exception - not defined type
                    throw new Exception("Joint type design is not implemented!");
                }
            }
            else if(joint_temp is CConnectionJoint_TA01 || joint_temp is CConnectionJoint_TB01)
                CalculateDesignRatioBaseJoint(joint_temp, sDIF_temp); // Base plates (main column or front/back column connection to the foundation)
            else
            {
                // Exception - not defined type
                throw new Exception("Joint type design is not implemented!");
            }
        }

        public void CalculateDesignRatioApexOrKneeJoint(CConnectionJointTypes joint_temp, designInternalForces sDIF_temp)
        {
            // Bending Joint apex, knee joint

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
            // fHolesCentersRadii - Moze sa lisit podla rozneho usporiadania skrutiek a vzdialenosti skrutiek od ich fiktivneho taziska (mali by byt symetricky)

            float[] fHolesCentersRadiiInOneGroup = null;
            int iNumberOfScrewGroupsInPlate = 0;
            float fr_max = 0;

            if (plate.ScrewArrangement != null) // Screw arrangement exist
            {
                if (plate.ScrewArrangement.ListOfSequenceGroups != null && plate.ScrewArrangement.ListOfSequenceGroups.Count > 0) // Screw arrangement groups are defined
                {
                    fHolesCentersRadiiInOneGroup = plate.ScrewArrangement.ListOfSequenceGroups[0].HolesRadii; // Use first group data (symmetry is expected
                    iNumberOfScrewGroupsInPlate = plate.ScrewArrangement.ListOfSequenceGroups.Count;
                }
                else
                {
                    throw new ArgumentException("Groups of screws are not defined. Check screw arrangement data.");
                }

                if (fHolesCentersRadiiInOneGroup != null)
                    fr_max = MathF.Max(fHolesCentersRadiiInOneGroup);
                else
                {
                    throw new ArgumentException("Radii of screws are not defined. Check screw arrangement data.");
                }
            }

            // 5.4.2.4 Tilting and hole bearing
            // Bending - Calculate shear strength of plate connection - main member
            for (int i = 0; i < fHolesCentersRadiiInOneGroup.Length; i++)
            {
                fMb_MainMember_oneside_plastic += fHolesCentersRadiiInOneGroup[i] * fVb_MainMember;
                fMb_SecondaryMember_oneside_plastic += fHolesCentersRadiiInOneGroup[i] * fVb_SecondaryMember;

                fSumri2tormax += MathF.Pow2(fHolesCentersRadiiInOneGroup[i]) / fr_max;
            }

            // Plastic resistance (Design Ratio)
            float fEta_Mb_MainMember_oneside_plastic = Math.Abs(fM_xu_oneside) / fMb_MainMember_oneside_plastic;
            fEta_max = MathF.Max(fEta_max, fEta_Mb_MainMember_oneside_plastic);
            float fEta_Mb_SecondaryMember_oneside_plastic = Math.Abs(fM_xu_oneside) / fMb_SecondaryMember_oneside_plastic;
            fEta_max = MathF.Max(fEta_max, fEta_Mb_SecondaryMember_oneside_plastic);

            // Elastic resistance
            float fV_asterix_b_max_screw_Mxu = Math.Abs(fM_xu_oneside) / fSumri2tormax;
            float fV_asterix_b_max_screw_Vyv = Math.Abs(fV_yv_oneside) / fHolesCentersRadiiInOneGroup.Length;
            float fV_asterix_b_max_screw_N = Math.Abs(fN_oneside) / fHolesCentersRadiiInOneGroup.Length;

            float fV_asterix_b_max_screw = 0;

            if (fV_asterix_b_max_screw_Mxu != 0 || fV_asterix_b_max_screw_Vyv != 0 || fV_asterix_b_max_screw_N != 0)
                fV_asterix_b_max_screw = MathF.Sqrt(MathF.Sqrt(MathF.Pow2(fV_asterix_b_max_screw_Mxu) + MathF.Pow2(fV_asterix_b_max_screw_Vyv)) + MathF.Pow2(fV_asterix_b_max_screw_N));

            float fEta_Vb_5424 = eq.Eq_5424_1__(fV_asterix_b_max_screw, 0.5f, fVb_MainMember);
            fEta_max = MathF.Max(fEta_max, fEta_Vb_5424);

            // 5.4.2.5 Connection shear as limited by end distance
            float fe = 0.03f; // TODO - temporary - urcit min vzdialenost skrutky od okraja plechu alebo prierezu
            float fV_fv_MainMember = eq.Eq_5425_2__(ft_2_crscmainMember, fe, ff_uk_2_MainMember);
            float fV_fv_SecondaryMember = eq.Eq_5425_2__(ft_2_crscsecMember, fe, ff_uk_2_SecondaryMember);
            float fV_fv_Plate = eq.Eq_5425_2__(ft_1_plate, fe, ff_uk_1_plate);

            // Distance to an end of the connected part is parallel to the line of the applied force
            // Nemalo by rozhodovat pre moment (skrutka namahana rovnobezne s okrajom je uprostred plechu) ale moze rozhovat pre N a V

            float fV_asterix_fv = 0;

            if (fV_asterix_b_max_screw_Vyv != 0 || fV_asterix_b_max_screw_N != 0)
                fV_asterix_fv = MathF.Sqrt(MathF.Pow2(fV_asterix_b_max_screw_Vyv) + MathF.Pow2(fV_asterix_b_max_screw_N));

            float fEta_V_fv_5425_MainMember = eq.Eq_5425_1__(fV_asterix_fv, fV_fv_MainMember, ff_uk_2_MainMember, ff_yk_2_MainMember);
            float fEta_V_fv_5425_SecondaryMember = eq.Eq_5425_1__(fV_asterix_fv, fV_fv_SecondaryMember, ff_uk_2_SecondaryMember, ff_yk_2_SecondaryMember);
            float fEta_V_fv_5425_Plate = eq.Eq_5425_1__(fV_asterix_fv, fV_fv_Plate, ff_uk_1_plate, ff_yk_1_plate);

            float fEta_V_fv_5425 = MathF.Max(fEta_V_fv_5425_MainMember, fEta_V_fv_5425_SecondaryMember, fEta_V_fv_5425_Plate);
            fEta_max = MathF.Max(fEta_max, fEta_V_fv_5425);

            // 5.4.2.6 Screws in shear
            // The design shear capacity φVw of the screw shall be determined by testing in accordance with Section 8.

            float fV_w_nom_screw_5426 = screw.ShearStrength_nominal; // N
            float fEta_V_w_5426 = Math.Max(fV_asterix_b_max_screw, fV_asterix_fv) / (0.5f * fV_w_nom_screw_5426);
            fEta_max = MathF.Max(fEta_max, fEta_V_w_5426);

            // Validation - negative design ratio
            if (fEta_Vb_5424 < 0 ||
                fEta_V_fv_5425 < 0)
            {
                throw new Exception("Design ratio is invalid!");
            }

            // Validation - inifinity design ratio
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
            // Purlins, girts .....
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
            float fd_w_for5434_plate = eq.Get_d_apostrophe_w(screw.Type, ft_1_plate, screw.D_h_headdiameter, screw.T_w_washerthickness, screw.D_w_washerdiameter);
            float fN_ov_for5434_plate = eq.Eq_5432_3__(ft_1_plate, screw.D_w_washerdiameter, ff_uk_1_plate); // 5.4.3.2(b) Eq. 5.4.3.2(3) - Nov

            bool bIsEccentricallyLoadedJoint = false;

            if (bIsEccentricallyLoadedJoint)
                fN_ov_for5434_plate *= 0.5f; // Use 50% of resistance value in case of eccentrically loaded connection

            float fV_asterix_b_for5434_MainMember = 0;

            if (sDIF_temp.fV_yu != 0 || sDIF_temp.fV_zv != 0)
                fV_asterix_b_for5434_MainMember = MathF.Sqrt(MathF.Pow2(sDIF_temp.fV_yu / iNumberOfScrewsInTension) + MathF.Pow2(sDIF_temp.fV_zv / iNumberOfScrewsInTension));

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

            float fV_asterix_b_for5435_MainMember = 0;

            if (sDIF_temp.fV_yu != 0 || sDIF_temp.fV_zv != 0)
                fV_asterix_b_for5435_MainMember = MathF.Sqrt(MathF.Pow2(sDIF_temp.fV_yu / iNumberOfScrewsInTension) + MathF.Pow2(sDIF_temp.fV_zv / iNumberOfScrewsInTension));

            float fEta_5435_MainMember = eq.Eq_5435____(fV_asterix_b_for5435_MainMember, sDIF_temp.fN_t / iNumberOfScrewsInTension, 0.6f, fV_b_for5435_MainMember, fN_ou_for5435_MainMember);
            fEta_max = MathF.Max(fEta_max, fEta_5435_MainMember);

            // 5.4.2.5 Connection shear as limited by end distance
            float fe_Plate = 0.03f; // TODO - temporary - urcit min vzdialenost skrutky od okraja plechu

            // Distance to an end of the connected part is parallel to the line of the applied force
            float fV_asterix_fv_plate = Math.Abs(sDIF_temp.fV_zv / iNumberOfScrewsInTension);
            float fV_fv_Plate = eq.Eq_5425_2__(ft_1_plate, fe_Plate, ff_uk_1_plate);
            float fEta_V_fv_5425_Plate = eq.Eq_5425_1__(fV_asterix_fv_plate, fV_fv_Plate, ff_uk_1_plate, ff_yk_1_plate);
            fEta_max = MathF.Max(fEta_max, fEta_V_fv_5425_Plate);

            // 5.4.2.6 Screws in shear
            // The design shear capacity φVw of the screw shall be determined by testing in accordance with Section 8.

            float fV_w_nom_screw_5426 = screw.ShearStrength_nominal; // N
            float fEta_V_w_5426 = (sDIF_temp.fN_t / iNumberOfScrewsInTension) / (0.5f * fV_w_nom_screw_5426);
            fEta_max = MathF.Max(fEta_max, fEta_V_w_5426);

            // 5.4.3.3 Screws in tension
            // The tensile capacity of the screw shall be determined by testing in accordance with Section 8.

            float fN_t_nom_screw_5433 = screw.AxialTensileStrength_nominal; // N
            float fEta_N_t_screw_5433 = Math.Max(fV_asterix_b_for5435_MainMember, fV_asterix_fv_plate) / (0.5f * fN_t_nom_screw_5433);
            fEta_max = MathF.Max(fEta_max, fEta_V_w_5426);

            // 5.4.3.6 Screws subject to combined shear and tension
            // A screw required to resist simultaneously a design shear force and a design tensile where V screw and N screw shall be determined by testing in accordance with Section 8.

            float fEta_V_N_t_screw_5436 = eq.Eq_5436____(Math.Max(fV_asterix_b_for5435_MainMember, fV_asterix_fv_plate), (sDIF_temp.fN_t / iNumberOfScrewsInTension), 0.5f, fV_w_nom_screw_5426, fN_t_nom_screw_5433);
            fEta_max = MathF.Max(fEta_max, fEta_V_N_t_screw_5436);

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
            float fV_asterix_b_SecondaryMember = 0;

            if (sDIF_temp.fV_yu != 0 || sDIF_temp.fV_zv != 0 || sDIF_temp.fN != 0)
                fV_asterix_b_SecondaryMember = MathF.Sqrt(MathF.Pow2(sDIF_temp.fV_zv / iNumberOfScrewsInConnectionOfSecondaryMember) + MathF.Pow2(sDIF_temp.fN / iNumberOfScrewsInConnectionOfSecondaryMember));

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

        public void CalculateDesignRatioFrontOrBackColumnToMainRafterJoint(CConnectionJointTypes joint_temp, designInternalForces sDIF_temp)
        {
            // TODO - refactoring s CalculateDesignRatioGirtOrPurlinJoint
            bool bDisplayWarningForContitions5434and5435 = false;
            int iNumberOfScrewsInTension = plate.ScrewArrangement.IHolesNumber * 2 / 3; // TODO - urcit presny pocet skrutiek v spoji ktore su pripojene k main member a ktore k secondary member, tahovu silu prenasaju skrutky pripojene k main member

            CConCom_Plate_N plateN = (CConCom_Plate_N)joint_temp.m_arrPlates[0];

            // Tension force in plate (metal strip)
            float fDIF_N_plate = Math.Abs(sDIF_temp.fV_zv) / (float)Math.Sin(plateN.Alpha1_rad);
            float fDIF_V_connection_one_side = fDIF_N_plate * (float)Math.Cos(plateN.Alpha1_rad);

            // 5.4.3 Screwed connections in tension
            // 5.4.3.2 Pull-out and pull-over (pull-through)

            // K vytiahnutiu alebo pretlaceniu moze dost v pripojeni k main member alebo pri posobeni sily Vx(Vy) na secondary member (to asi zanedbame)

            float fN_t_5432_MainMember = eq.Get_Nt_5432(screw.Type, ft_1_plate, ft_2_crscmainMember, screw.Diameter_thread, screw.D_h_headdiameter, screw.T_w_washerthickness, screw.D_w_washerdiameter, ff_uk_1_plate, ff_uk_2_MainMember);
            float fEta_N_t_5432_MainMember = eq.Eq_5432_1__(sDIF_temp.fV_zv / iNumberOfScrewsInTension, 0.5f, fN_t_5432_MainMember);
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
            float fd_w_for5434_plate = eq.Get_d_apostrophe_w(screw.Type, ft_1_plate, screw.D_h_headdiameter, screw.T_w_washerthickness, screw.D_w_washerdiameter);
            float fN_ov_for5434_plate = eq.Eq_5432_3__(ft_1_plate, screw.D_w_washerdiameter, ff_uk_1_plate); // 5.4.3.2(b) Eq. 5.4.3.2(3) - Nov

            bool bIsEccentricallyLoadedJoint = false;

            if (bIsEccentricallyLoadedJoint)
                fN_ov_for5434_plate *= 0.5f; // Use 50% of resistance value in case of eccentrically loaded connection

            float fV_asterix_b_for5434_MainMember = fDIF_V_connection_one_side / (iNumberOfScrewsInTension / 2);
            float fEta_5434_MainMember = eq.Eq_5434____(fV_asterix_b_for5434_MainMember, Math.Abs(sDIF_temp.fV_zv) / iNumberOfScrewsInTension, 0.65f, fV_b_for5434_MainMember, fN_ov_for5434_plate);
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

            float fV_asterix_b_for5435_MainMember = fDIF_V_connection_one_side / (iNumberOfScrewsInTension / 2);
            float fEta_5435_MainMember = eq.Eq_5435____(fV_asterix_b_for5435_MainMember, Math.Abs(sDIF_temp.fV_zv), 0.6f, fV_b_for5435_MainMember, fN_ou_for5435_MainMember);
            fEta_max = MathF.Max(fEta_max, fEta_5435_MainMember);

            // 5.4.2.5 Connection shear as limited by end distance
            float fe_Plate = 0.03f; // TODO - temporary - urcit min vzdialenost skrutky od okraja plechu

            // Distance to an end of the connected part is parallel to the line of the applied force
            float fV_asterix_fv_plate = fDIF_V_connection_one_side / (iNumberOfScrewsInTension / 2);
            float fV_fv_Plate = eq.Eq_5425_2__(ft_1_plate, fe_Plate, ff_uk_1_plate);
            float fEta_V_fv_5425_Plate = eq.Eq_5425_1__(fV_asterix_fv_plate, fV_fv_Plate, ff_uk_1_plate, ff_yk_1_plate);
            fEta_max = MathF.Max(fEta_max, fEta_V_fv_5425_Plate);

            // 5.4.2.6 Screws in shear
            // The design shear capacity φVw of the screw shall be determined by testing in accordance with Section 8.

            float fV_w_nom_screw_5426 = screw.ShearStrength_nominal; // N
            float fEta_V_w_5426 = Math.Max(fV_asterix_b_for5435_MainMember, fV_asterix_fv_plate) / (0.5f * fV_w_nom_screw_5426);
            fEta_max = MathF.Max(fEta_max, fEta_V_w_5426);

            // 5.4.3.3 Screws in tension
            // The tensile capacity of the screw shall be determined by testing in accordance with Section 8.

            float fN_t_nom_screw_5433 = screw.AxialTensileStrength_nominal; // N
            float fEta_N_t_screw_5433 = (Math.Abs(sDIF_temp.fV_zv) / (iNumberOfScrewsInTension / 2)) / (0.5f * fN_t_nom_screw_5433);
            fEta_max = MathF.Max(fEta_max, fEta_V_w_5426);

            // 5.4.3.6 Screws subject to combined shear and tension
            // A screw required to resist simultaneously a design shear force and a design tensile where V screw and N screw shall be determined by testing in accordance with Section 8.

            float fEta_V_N_t_screw_5436 = eq.Eq_5436____(Math.Max(fV_asterix_b_for5435_MainMember, fV_asterix_fv_plate), Math.Abs(sDIF_temp.fV_zv) / (iNumberOfScrewsInTension / 2), 0.5f, fV_w_nom_screw_5426, fN_t_nom_screw_5433);
            fEta_max = MathF.Max(fEta_max, fEta_V_N_t_screw_5436);

            // Plate design
            // Plate tension design
            float fA_n_plate = plate.fA_n;
            float fN_t_plate = eq.Eq_5423_2__(screw.Diameter_thread, plate.S_f_min, fA_n_plate, ff_uk_1_plate);
            float fEta_N_t_5423_plate = eq.Eq_5423_1__(fDIF_N_plate, 0.65f, fN_t_plate);
            fEta_max = MathF.Max(fEta_max, fEta_N_t_5423_plate);
        }

        public void CalculateDesignRatioBaseJoint(CConnectionJointTypes joint_temp, designInternalForces sDIF_temp)
        {
            // Okopirovane z CalculateDesignRatioApexOrKneeJoint
            // TODO - refaktorovat

            int iNumberOfPlatesInJoint = joint.m_arrPlates.Length;

            float fN = 1f / iNumberOfPlatesInJoint * sDIF_temp.fN;
            float fM_xu = 1f / iNumberOfPlatesInJoint * sDIF_temp.fM_yu;
            float fV_yv = 1f / iNumberOfPlatesInJoint * sDIF_temp.fV_zv;

            // Plate design

            // Plate tension design
            float fA_n_plate = plate.fA_n;
            float fN_t_plate = eq.Eq_5423_2__(screw.Diameter_thread, plate.S_f_min, fA_n_plate, ff_uk_1_plate);
            float fEta_N_t_5423_plate = eq.Eq_5423_1__(fN, 0.65f, fN_t_plate);
            fEta_max = MathF.Max(fEta_max, fEta_N_t_5423_plate);

            // Plate shear resistance
            float fA_vn_yv_plate = plate.fA_vn_zv;
            float fV_y_yv_plate = eq.Eq_723_5___(fA_vn_yv_plate, ff_yk_1_plate);
            float fEta_V_yv_3341_plate = eq.Eq_3341____(fV_yv, 0.65f, fV_y_yv_plate);
            fEta_max = MathF.Max(fEta_max, fEta_V_yv_3341_plate);

            // Plate bending resistance
            float fM_xu_resitance_plate = eq.Eq_7222_4__(joint.m_arrPlates[0].fW_el_yu, ff_yk_1_plate);
            float fEta_Mb_plate, fDesignReistance_M_plate;
            eq.Eq_723_10__(Math.Abs(fM_xu), 0.65f, fM_xu_resitance_plate, out fDesignReistance_M_plate, out fEta_Mb_plate);
            fEta_max = MathF.Max(fEta_max, fEta_Mb_plate);

            // Connection -shear force design
            // Shear in connection
            float fVb_MainMember = eq.Get_Vb_5424(ft_1_plate, ft_2_crscmainMember, screw.Diameter_thread, ff_uk_1_plate, ff_uk_2_MainMember);

            int iNumberOfScrewsInShear = joint_temp.m_arrPlates[0].ScrewArrangement.Screws.Length; // Temporary

            float fEta_MainMember = sDIF.fV_zv / (iNumberOfScrewsInShear * fVb_MainMember);

            float fMb_MainMember_oneside_plastic = 0;

            float fSumri2tormax = 0; // F_max = Mxu / (Σ ri^2 / r_max)

            // TEMPORARY
            // fHolesCentersRadii - Moze sa lisit podla rozneho usporiadania skrutiek a vzdialenosti skrutiek od ich fiktivneho taziska (mali by byt symetricky)

            float[] fHolesCentersRadiiInOneGroup = null;
            int iNumberOfScrewGroupsInPlate = 0;
            float fr_max = 0;

            if (plate.ScrewArrangement != null) // Screw arrangement exist
            {
                if (plate.ScrewArrangement.ListOfSequenceGroups != null && plate.ScrewArrangement.ListOfSequenceGroups.Count > 0) // Screw arrangement groups are defined
                {
                    fHolesCentersRadiiInOneGroup = plate.ScrewArrangement.ListOfSequenceGroups[0].HolesRadii; // Use first group data (symmetry is expected
                    iNumberOfScrewGroupsInPlate = plate.ScrewArrangement.ListOfSequenceGroups.Count;
                }
                else
                {
                    throw new ArgumentException("Groups of screws are not defined. Check screw arrangement data.");
                }

                if (fHolesCentersRadiiInOneGroup != null)
                    fr_max = MathF.Max(fHolesCentersRadiiInOneGroup);
                else
                {
                    throw new ArgumentException("Radii of screws are not defined. Check screw arrangement data.");
                }
            }

            // 5.4.2.4 Tilting and hole bearing
            // Bending - Calculate shear strength of plate connection - main member
            for (int i = 0; i < fHolesCentersRadiiInOneGroup.Length; i++)
            {
                fMb_MainMember_oneside_plastic += fHolesCentersRadiiInOneGroup[i] * fVb_MainMember;

                fSumri2tormax += MathF.Pow2(fHolesCentersRadiiInOneGroup[i]) / fr_max;
            }

            float fN_oneside = sDIF_temp.fN / 2f;
            float fM_xu_oneside = sDIF_temp.fM_yu / 2f; // Divided by Number of sides
            float fV_yv_oneside = sDIF_temp.fV_zv / 2f;

            // Plastic resistance (Design Ratio)
            float fEta_Mb_MainMember_oneside_plastic = Math.Abs(fM_xu_oneside) / fMb_MainMember_oneside_plastic;
            fEta_max = MathF.Max(fEta_max, fEta_Mb_MainMember_oneside_plastic);

            // Elastic resistance
            float fV_asterix_b_max_screw_Mxu = Math.Abs(fM_xu_oneside) / fSumri2tormax;
            float fV_asterix_b_max_screw_Vyv = Math.Abs(fV_yv_oneside) / fHolesCentersRadiiInOneGroup.Length;
            float fV_asterix_b_max_screw_N = Math.Abs(fN_oneside) / fHolesCentersRadiiInOneGroup.Length;

            float fV_asterix_b_max_screw = 0;

            if (fV_asterix_b_max_screw_Mxu != 0 || fV_asterix_b_max_screw_Vyv  != 0 || fV_asterix_b_max_screw_N != 0)
                fV_asterix_b_max_screw = MathF.Sqrt(MathF.Sqrt(MathF.Pow2(fV_asterix_b_max_screw_Mxu) + MathF.Pow2(fV_asterix_b_max_screw_Vyv)) + MathF.Pow2(fV_asterix_b_max_screw_N));

            float fEta_Vb_5424 = eq.Eq_5424_1__(fV_asterix_b_max_screw, 0.5f, fVb_MainMember);
            fEta_max = MathF.Max(fEta_max, fEta_Vb_5424);

            // 5.4.2.5 Connection shear as limited by end distance
            float fe = 0.03f; // TODO - temporary - urcit min vzdialenost skrutky od okraja plechu alebo prierezu
            float fV_fv_MainMember = eq.Eq_5425_2__(ft_2_crscmainMember, fe, ff_uk_2_MainMember);
            float fV_fv_Plate = eq.Eq_5425_2__(ft_1_plate, fe, ff_uk_1_plate);

            // Distance to an end of the connected part is parallel to the line of the applied force
            // Nemalo by rozhodovat pre moment (skrutka namahana rovnobezne s okrajom je uprostred plechu) ale moze rozhovat pre N a V
            float fV_asterix_fv = 0;

            if (fV_asterix_b_max_screw_Vyv != 0 || fV_asterix_b_max_screw_N != 0)
                fV_asterix_fv = MathF.Sqrt(MathF.Pow2(fV_asterix_b_max_screw_Vyv) + MathF.Pow2(fV_asterix_b_max_screw_N));

            float fEta_V_fv_5425_MainMember = eq.Eq_5425_1__(fV_asterix_fv, fV_fv_MainMember, ff_uk_2_MainMember, ff_yk_2_MainMember);
            float fEta_V_fv_5425_Plate = eq.Eq_5425_1__(fV_asterix_fv, fV_fv_Plate, ff_uk_1_plate, ff_yk_1_plate);

            float fEta_V_fv_5425 = Math.Max(fEta_V_fv_5425_MainMember, fEta_V_fv_5425_Plate);
            fEta_max = MathF.Max(fEta_max, fEta_V_fv_5425);

            // 5.4.2.6 Screws in shear
            // The design shear capacity φVw of the screw shall be determined by testing in accordance with Section 8.

            float fV_w_nom_screw_5426 = screw.ShearStrength_nominal; // N
            float fEta_V_w_5426 = Math.Max(fV_asterix_b_max_screw, fV_asterix_fv) / (0.5f * fV_w_nom_screw_5426);
            fEta_max = MathF.Max(fEta_max, fEta_V_w_5426);

            // Validation - negative design ratio
            if (fEta_Vb_5424 < 0 ||
                fEta_V_fv_5425 < 0)
            {
                throw new Exception("Design ratio is invalid!");
            }

            // Validation - inifinity design ratio
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
            float fA_n_MainMember = (float)crsc_mainMember.A_g - plate.INumberOfConnectorsInSection * 2 * 2 * screw.Diameter_thread; // TODO - spocitat presne podla poctu a rozmeru otvorov v jednom reze
            float fN_t_section_MainMember = eq.Eq_5423_2__(screw.Diameter_thread, plate.S_f_min, fA_n_MainMember, ff_uk_2_MainMember);
            float fEta_N_t_5423_MainMember = eq.Eq_5423_1__(sDIF_temp.fN_t, 0.65f, fN_t_section_MainMember);
            fEta_max = MathF.Max(fEta_max, fEta_N_t_5423_MainMember);


            // TODO - tu este chybaju posudenia samotnej dosky na napati betonu pod nou na lokalny tlak betonu pod plechom, ohyb, oslabeny prierez v ohybe , tahu, tlaku, smyku atd vid xls




            // Anchors
            float fN_asterix_joint_uplif = Math.Max(sDIF_temp.fN, 0); // Tension in column - positive
            float fN_asterix_joint_bearing = Math.Min(sDIF_temp.fN, 0); // Compression in column - negative

            float fV_asterix_x_joint = Math.Abs(sDIF_temp.fV_yy);
            float fV_asterix_y_joint = Math.Abs(sDIF_temp.fV_zz);
            float fV_asterix_res_joint = 0f;

            if(!MathF.d_equal(fV_asterix_x_joint,0) || !MathF.d_equal(fV_asterix_y_joint, 0))
                fV_asterix_res_joint = MathF.Sqrt(MathF.Pow2(fV_asterix_x_joint) + MathF.Pow2(fV_asterix_y_joint));

            //int iNumberAnchors = plate.AnchorArrangement.Anchors.Length;
            int iNumberAnchors = plate.AnchorArrangement.IHolesNumber;
            int iNumberAnchors_t = iNumberAnchors; // Total number of anchors active in tension - all anchors active as default
            int iNumberAnchors_v = iNumberAnchors; // Total number of anchors active in shear - all anchors active as default

            CAnchorArrangement_BB_BG anchorArrangement;

            if (plate.AnchorArrangement is CAnchorArrangement_BB_BG)
                anchorArrangement = (CAnchorArrangement_BB_BG)plate.AnchorArrangement;
            else
            {
                throw new Exception("Not implemented arrangmement of anchors.");
            }

            int iNumberAnchors_x = anchorArrangement.NumberOfAnchorsInYDirection;
            int iNumberAnchors_y = anchorArrangement.NumberOfAnchorsInZDirection;

            float fN_asterix_anchor_uplif = fN_asterix_joint_uplif / iNumberAnchors_t; // Design axial force per anchor
            float fV_asterix_anchor = fV_asterix_res_joint / iNumberAnchors_v; // Design shear force per anchor

            float fplateWidth_x = (float)joint.m_MainMember.CrScStart.b; // TODO - zapracovat priamo nacitanie parametrov plate type BA - BG
            float fplateWidth_y = (float)joint.m_MainMember.CrScStart.h; // TODO - zapracovat priamo nacitanie parametrov plate type BA - BG

            float fFootingDimension_x = 1.1f; // TODO - napojit na velkost zakladu
            float fFootingDimension_y = 1.1f; // TODO - napojit na velkost zakladu
            float fFootingHeight = 0.4f; // TODO - napojit na velkost zakladu

            float fe_x_AnchorToPlateEdge = 0.05f; // TODO - Distance between anchor and plate edge
            float fe_y_AnchorToPlateEdge = 0.05f; // TODO - Distance between anchor and plate edge

            float fe_x_BasePlateToFootingEdge = 0.5f * (fFootingDimension_x - fplateWidth_x); // TODO - Distance between base plate and footing edge - zapracovat excentricitu
            float fe_y_BasePlateToFootingEdge = 0.5f * (fFootingDimension_y - fplateWidth_y); // TODO - Distance between base plate and footing edge - zapracovat excentricitu

            float fu_x_Washer = 0.08f;
            float fu_y_Washer = 0.08f;

            float ff_apostrophe_c = 25e+6f; // Characteristic compressive (cylinder) concrete strength
            float fRho_c = 2300f; // Density of concrete - TODO - nacitat z materialu zakladov

            // Anchors (bolts)
            float fd_s = plate.AnchorArrangement.referenceAnchor.Diameter_thread;
            float fd_f = plate.AnchorArrangement.referenceAnchor.Diameter_shank;

            float fA_c = plate.AnchorArrangement.referenceAnchor.Area_c_thread; // Core / thread area
            float fA_o = plate.AnchorArrangement.referenceAnchor.Area_o_shank; // Shank area

            float ff_y_anchor = plate.AnchorArrangement.referenceAnchor.m_Mat.Get_f_yk_by_thickness(fd_f);
            float ff_u_anchor = plate.AnchorArrangement.referenceAnchor.m_Mat.Get_f_uk_by_thickness(fd_f);

            // AS / NZS 4600:2018 - 5.3 Bolted connections
            // Base plate design
            // 5.3.2 Tearout
            float fV_f_532 = eq.Eq_532_2___(ft_1_plate, fe_y_AnchorToPlateEdge, ff_uk_1_plate);
            float fDesignRatio_532_1 = eq.Eq_5351_1__(fV_asterix_anchor, 0.7f, fV_f_532);
            fEta_max = MathF.Max(fEta_max, fDesignRatio_532_1);

            // 5.3.4.2 Bearing capacity without considering bolt hole deformation
            float fAlpha_5342 = eq.Get_Alpha_Table_5342_A(ETypesOfBearingConnection.eType3);
            float fC_5342 = eq.Get_Factor_C_Table_5342_B(fd_f, ft_1_plate);
            float fV_b_5342 = eq.Eq_5342____(fAlpha_5342, fC_5342, fd_f, ft_1_plate, ff_uk_1_plate);
            float fDesignRatio_5342 = fV_asterix_anchor / (0.6f * fV_b_5342);
            fEta_max = MathF.Max(fEta_max, fDesignRatio_5342);

            // 5.3.4.3 Bearing capacity at a bolt hole deformation of 6 mm
            float fV_b_5343 = eq.Eq_5343____(fd_f, ft_1_plate, ff_uk_1_plate);
            float fDesignRatio_5343 = fV_asterix_anchor / (0.6f * fV_b_5343);
            fEta_max = MathF.Max(fEta_max, fDesignRatio_5343);

            // Bolt design / Anchor design
            // 5.3.5.1 Bolt in shear
            int iNumberOfShearPlanesOfBolt_core = 1; // Jednostrizny spoj - strih jardom skrutky
            float fV_fv_5351_2_anchor = eq.Eq_5351_2__(ff_u_anchor, iNumberOfShearPlanesOfBolt_core, fA_c, 0, fA_o); // Uvazovane konzervativne jedna smykova plocha a zavit je aj v smykovej ploche
            float fDesignRatio_5351_2 = eq.Eq_5351_1__(fV_asterix_anchor, 0.8f, fV_fv_5351_2_anchor);
            fEta_max = MathF.Max(fEta_max, fDesignRatio_5351_2);

            // 5.3.5.2 Bolt in tension
            float fN_ft_5352_1 = eq.Eq_5352_2__(fA_c, ff_u_anchor);
            float fDesignRatio_5352_1 = eq.Eq_5352_1__(fN_asterix_anchor_uplif, 0.8f, fN_ft_5352_1);
            fEta_max = MathF.Max(fEta_max, fDesignRatio_5352_1);

            // 5.3.5.3 Bolt subject to combined shear and tension
            float fPortion_V_5353;
            float fPortion_N_5353;
            float fDesignRatio_5353 = eq.Eq_5353____(fV_asterix_anchor, 0.8f, fV_fv_5351_2_anchor, fN_asterix_anchor_uplif, 0.8f, fN_ft_5352_1, out fPortion_V_5353, out fPortion_N_5353);
            fEta_max = MathF.Max(fEta_max, fDesignRatio_5353);

            float fElasticityFactor_1764 = 0.75f; // EQ load combination - 0.75, other 1.00

            // NZS 3101.1 - 2006
            // 17.5.6 Strength of cast -in anchors

            // 17.5.6.4 Strength reduction factors
            float fPhi_anchor_tension_173 = 0.75f;
            float fPhi_anchor_shear_174   = 0.65f;

            float fPhi_concrete_tension_174a = 0.65f;
            float fPhi_concrete_shear_174b   = 0.65f;

            // 17.5.7.1  Steel strength of anchor in tension
            // Group of anchors
            float fA_se = fA_c; // Effective cross-sectional area of an anchor
            float fN_s_176_group = eq_concrete.Eq_17_6____(iNumberAnchors_t, fA_se, ff_u_anchor);
            float fDesignRatio_17571_group = eq_concrete.Eq_17_1____(fN_asterix_joint_uplif, fPhi_anchor_tension_173, fN_s_176_group);
            fEta_max = MathF.Max(fEta_max, fDesignRatio_17571_group);

            // 17.5.7.2  Strength of concrete breakout of anchor
            // Group of anchors

            // Figure C17.4 – Definition of dimension e´n for group anchors
            float fe_apostrophe_n = 0f;                           // the distance between the resultant tension load on a group of anchors in tension and the centroid of the group of anchors loaded in tension(always taken as positive)
            float fConcreteCover = 0.07f;
            float fh_ef = fFootingHeight - fConcreteCover;        // effective anchor embedment depth
            float fs_2_x = 0f;                                    // centre-to-centre spacing of the anchors
            float fs_1_y = 0.23f;                                 // centre-to-centre spacing of the anchors
            float fs_min = Math.Min(fs_2_x, fs_1_y);
            float fc_2_x = 0.55f;
            float fc_1_y = 0.55f;
            float fc_min = Math.Min(fc_2_x, fc_1_y);
            float fk = 10f; // for cast-in anchors
            float fLambda_53 = eq_concrete.Eq_5_3_____(fRho_c);

            fe_x_AnchorToPlateEdge = 0.5f * (fplateWidth_x - (iNumberAnchors_x - 1) * fs_2_x);
            fe_y_AnchorToPlateEdge = 0.5f * (fplateWidth_y - (iNumberAnchors_y - 1) * fs_1_y);



            float fPsi_1_group = eq_concrete.Eq_17_8____(fe_apostrophe_n, fh_ef);
            float fPsi_2 = eq_concrete.Get_Psi_2__(fc_min, fh_ef);

            // Ψ3 = 1.25 for cast -in anchors in uncracked concrete
            // Ψ3 = 1.0 for concrete which is cracked at service load levels.
            float fPsi_3 = 1.25f; // modification factor or cracking of concrete
            float fA_no_group = (2f * 1.5f * fh_ef) * (2f * 1.5f * fh_ef);

            float fAn_Length_x_group = Math.Min(fc_2_x, 1.5f * fh_ef) + 1.5f * fh_ef + ((iNumberAnchors_x - 1) * fs_2_x);
            float fAn_Length_y_group = Math.Min(fc_1_y, 1.5f * fh_ef) + 1.5f * fh_ef + ((iNumberAnchors_y - 1) * fs_1_y);
            float fA_n_group = Math.Min(fAn_Length_x_group * fAn_Length_y_group, iNumberAnchors_t * fA_no_group);

            float fN_b_179_group = eq_concrete.Eq_17_9____(fk, fLambda_53, Math.Min(ff_apostrophe_c, 70e+6f), fh_ef);
            float fN_b_179a_group = eq_concrete.Eq_17_9a___(fLambda_53, ff_apostrophe_c, fh_ef);

            if(0.280f <= fh_ef && fh_ef <= 0.635f && fN_b_179_group > fN_b_179a_group)
            {
                fN_b_179_group = fN_b_179a_group;
            }

            float fN_cb_177_group = eq_concrete.Eq_17_7____(fPsi_1_group, fPsi_2, fPsi_3, fA_n_group, fA_no_group, fN_b_179_group);

            float fDesignRatio_17572_group = eq_concrete.Eq_17_1____(fN_asterix_joint_uplif, fPhi_concrete_tension_174a, fN_cb_177_group);
            fEta_max = MathF.Max(fEta_max, fDesignRatio_17572_group);

            // Single anchor - edge
            float fPsi_1_single = 1.0f;
            float fA_no_single = (2f * 1.5f * fh_ef) * (2f * 1.5f * fh_ef);
            float fAn_Length_x_single = Math.Min(fc_2_x, 1.5f * fh_ef) + 1.5f * fh_ef;
            float fAn_Length_y_single = Math.Min(fc_1_y, 1.5f * fh_ef) + 1.5f * fh_ef;
            float fA_n_single = Math.Min(fAn_Length_x_single * fAn_Length_y_single, fA_no_single);

            float fN_b_179_single = eq_concrete.Eq_17_9____(fk, fLambda_53, Math.Min(ff_apostrophe_c, 70e+6f), fh_ef);
            float fN_b_179a_single = eq_concrete.Eq_17_9a___(fLambda_53, ff_apostrophe_c, fh_ef);

            if (0.280f <= fh_ef && fh_ef <= 0.635f && fN_b_179_single > fN_b_179a_single)
            {
                fN_b_179_single = fN_b_179a_single;
            }

            float fN_cb_177_single = eq_concrete.Eq_17_7____(fPsi_1_single, fPsi_2, fPsi_3, fA_n_single, fA_no_single, fN_b_179_single);

            float fDesignRatio_17572_single = eq_concrete.Eq_17_1____(fN_asterix_anchor_uplif, fPhi_concrete_tension_174a, fN_cb_177_single);
            fEta_max = MathF.Max(fEta_max, fDesignRatio_17572_single);

            // 17.5.7.3  Lower characteristic tension pullout strength of anchor
            // Group of anchors

            float fm_x = 0.06f;
            float fm_y = 0.06f;
            float fA_brg = fm_x * fm_y; // bearing area of the head of stud or anchor
            float fN_p_1711_single = eq_concrete.Eq_17_11___(ff_apostrophe_c, fA_brg);

            // Modification factor for pullout strength
            // Ψ4 = 1.0 for concrete cracked at service load levels but with the extent of cracking controlled by reinforcement distributed in accordance with 2.4.4.4 and 2.4.4.5
            // Ψ4 = 1.4 for concrete with no cracking at service load levels
            float fPsi_4 = 1.0f;
            float fN_pn_1710_single = eq_concrete.Eq_17_10___(fPsi_4, fN_p_1711_single);
            float fN_pn_1710_group = iNumberAnchors_t * fN_pn_1710_single;

            float fDesignRatio_17573_group = eq_concrete.Eq_17_1____(fN_asterix_joint_uplif, fPhi_anchor_tension_173, fN_pn_1710_group);
            fEta_max = MathF.Max(fEta_max, fDesignRatio_17573_group);

            // The side face blowout strength of a headed anchor with deep embedment close to an edge
            float fN_sb_1713_single = float.PositiveInfinity;
            float fDesignRatio_17574_single = 0;

            if (fc_min < 0.4f * fh_ef)
            {
                // 17.5.7.4 Lower characteristic concrete side face blowout strength
                // Single anchor - edge
                float fc_1_17574 = fc_1_y;

                if (fN_asterix_anchor_uplif > 0) // Tension in anchor
                    fc_1_17574 = fc_min;

                // Anchors subject to shear are located in narrow sections of limited thickness
                float fc_1_limit = MathF.Max(fc_2_x / 1.5f, fh_ef / 1.5f, fs_min / 3f);

                if (fc_1_17574 > fc_1_limit)
                    fc_1_17574 = fc_1_limit;

                float fk_1 = eq_concrete.Get_k_1____(fc_1_17574, fc_2_x);

                fN_sb_1713_single = eq_concrete.Eq_17_13___(fk_1, fc_1_17574, fLambda_53, fA_brg, ff_apostrophe_c);

                fDesignRatio_17574_single = eq_concrete.Eq_17_1____(fN_asterix_anchor_uplif, fPhi_concrete_tension_174a, fN_sb_1713_single);
                fEta_max = MathF.Max(fEta_max, fDesignRatio_17574_single);
            }

            // Lower characteristic strength in tension
            float fN_n_nominal_min = MathF.Min(
                fN_s_176_group,                         // 17.5.7.1
                fN_cb_177_group,                        // 17.5.7.2
                iNumberAnchors_t * fN_cb_177_single,    // 17.5.7.2
                fN_pn_1710_group,                       // 17.5.7.3
                iNumberAnchors_t * fN_sb_1713_single);  // 17.5.7.4

            // Lower design strength in tension
            float fN_d_design_min = fElasticityFactor_1764 * MathF.Min(
                fPhi_anchor_tension_173 * fN_s_176_group,                           // 17.5.7.1
                fPhi_concrete_tension_174a * fN_cb_177_group,                       // 17.5.7.2
                fPhi_concrete_tension_174a * iNumberAnchors_t * fN_cb_177_single,   // 17.5.7.2
                fPhi_anchor_tension_173 * fN_pn_1710_group,                         // 17.5.7.3
                fPhi_concrete_tension_174a * iNumberAnchors_t * fN_sb_1713_single); // 17.5.7.4

            // 17.5.8 Lower characteristic strength of anchor in shear

            // 17.5.8.1 Lower characteristic shear strength of steel of anchor
            // Group of anchors

            // TODO - rozlisovat typ kotvy - rovnica 17-14 alebo 17-15
            float fV_s_1714_group = eq_concrete.Eq_17_14___(iNumberAnchors_v, fA_se, ff_u_anchor, ff_y_anchor);
            float fV_s_1715_group = eq_concrete.Eq_17_15___(iNumberAnchors_v, fA_se, ff_u_anchor, ff_y_anchor);

            float fV_s_17581_group = Math.Min(fV_s_1714_group, fV_s_1715_group);
            float fDesignRatio_17581_group = eq_concrete.Eq_17_2____(fV_asterix_res_joint, fPhi_anchor_shear_174, fV_s_17581_group);
            fEta_max = MathF.Max(fEta_max, fDesignRatio_17573_group);

            // 17.5.8.2 Lower characteristic concrete breakout strength of the anchor in shear perpendicular to edge
            // Group of anchors

            float fe_apostrophe_v = 0;
            float fPsi_5_group = eq_concrete.Eq_17_18___(fc_1_y, fe_apostrophe_v, fs_2_x); // s - perpendicular to shear force - Figure C17.7 – Definition of dimensions e´
            float fPsi_6 = eq_concrete.Get_Psi_6__(fc_1_y, fc_2_x);

            // Ψ7 = modification factor for cracked concrete, given by:
            // Ψ7 = 1.0 for anchors in cracked concrete with no supplementary reinforcement or with smaller than 12 mm diameter reinforcing bar as supplementary reinforcement
            // Ψ7 = 1.2 for anchors in cracked concrete with a 12 mm diameter reinforcing bar or greater as supplementary reinforcement
            // Ψ7 = 1.4 for concrete that is not cracked at service load levels.
            float fPsi_7 = 1.0f;

            float fA_vo = 2 * (1.5f * fc_1_y) * (1.5f * fc_1_y); // projected concrete failure area of an anchor in shear, when not limited by corner influences, spacing, or member thickness
            float fAv_Length_x_group = 1.5f * fc_1_y + Math.Min(1.5f * fc_1_y, fc_2_x) + (iNumberAnchors_x - 1) * fs_2_x;
            float fAv_Depth_h = Math.Min(1.5f * fc_1_y, fFootingHeight);
            float fA_v_group = fAv_Length_x_group * fAv_Depth_h; // projected concrete failure area of an anchor or group of anchors in shear
            float fd_o = fd_f;

            float fk_2 = 0.6f;
            float fl = Math.Min(fh_ef, 8f * fd_o); // load-bearing length of anchors for shear, equal to hef for anchors with constant stiffness over the full length of the embedded section but less than 8do.Shall be taken as 0.8 times the effective embedment depth for hooked metal plates.

            if (iNumberAnchors != 1 && fs_min != 0 && fs_min < 0.065f)
                throw new Exception("Distance between anchors s is smaller than 65 mm.");

            float fV_b_1717a = eq_concrete.Eq_17_17a__(fk_2, fl, fd_o, fLambda_53, ff_apostrophe_c, fc_1_y);
            float fV_b_1717b = eq_concrete.Eq_17_17b__(fLambda_53, ff_apostrophe_c, fc_1_y);
            float fV_b_1717 = Math.Min(fV_b_1717a, fV_b_1717b);
            float fV_cb_1716_group = eq_concrete.Eq_17_16___(fA_v_group, fA_vo, fPsi_5_group, fPsi_6, fPsi_7, fV_b_1717);

            float fDesignRatio_17582_group = eq_concrete.Eq_17_2____(fV_asterix_res_joint, fPhi_concrete_shear_174b, fV_cb_1716_group);
            fEta_max = MathF.Max(fEta_max, fDesignRatio_17582_group);

            // Single of anchor - edge

            float fPsi_5_single = 1.0f;
            float fAv_Length_x_signle = (1.5f * fc_1_y + Math.Min(1.5f * fc_1_y, 0.5f * fFootingDimension_x));
            float fA_v_single = fAv_Length_x_signle * fAv_Depth_h;
            float fV_cb_1716_single = eq_concrete.Eq_17_16___(fA_v_single, fA_vo, fPsi_5_single, fPsi_6, fPsi_7, fV_b_1717);

            float fDesignRatio_17582_single = eq_concrete.Eq_17_2____(fV_asterix_anchor, fPhi_concrete_shear_174b, fV_cb_1716_single);
            fEta_max = MathF.Max(fEta_max, fDesignRatio_17582_single);

            // 17.5.8.3 Lower characteristic concrete breakout strength of the anchor in shear parallel to edge
            // Group of anchors
            // TODO - zohladnit len paralelnu smykovu silu Vx ???

            float fV_cb_1721_group = eq_concrete.Eq_17_21___(fA_v_group, fA_vo, fPsi_5_group, fPsi_7, fV_b_1717);

            float fDesignRatio_17583_group = eq_concrete.Eq_17_2____(fV_asterix_res_joint, fPhi_concrete_shear_174b, fV_cb_1721_group);
            fEta_max = MathF.Max(fEta_max, fDesignRatio_17583_group);

            // Single anchor - edge
            float fV_cb_1721_single = eq_concrete.Eq_17_21___(fA_v_single, fA_vo, fPsi_5_single, fPsi_7, fV_b_1717);

            float fDesignRatio_17583_single = eq_concrete.Eq_17_2____(fV_asterix_res_joint, fPhi_concrete_shear_174b, fV_cb_1721_single);
            fEta_max = MathF.Max(fEta_max, fDesignRatio_17583_single);

            // 17.5.8.4 Lower characteristic concrete pry-out of the anchor in shear
            // Group of anchors

            float fN_cb_17584_group = Math.Min(fN_pn_1710_group, fN_cb_177_group); // ??? Zohladnit aj N_pn ???
            float fk_cp_17584 = eq_concrete.Get_k_cp___(fh_ef);
            float fV_cp_1722_group = eq_concrete.Eq_17_22___(fk_cp_17584, fN_cb_17584_group);

            float fDesignRatio_17584_single = eq_concrete.Eq_17_2____(fV_asterix_anchor, fPhi_concrete_shear_174b, fV_cp_1722_group);
            fEta_max = MathF.Max(fEta_max, fDesignRatio_17584_single);

            // Lower characteristic strength in shear
            float fV_n_nominal_min = MathF.Min(
                fV_s_17581_group,                       // 17.5.8.1
                fV_cb_1716_group,                       // 17.5.8.2
                iNumberAnchors_v * fV_cb_1716_single,   // 17.5.8.2
                fV_cb_1721_group,                       // 17.5.8.3
                iNumberAnchors_v * fV_cb_1721_single,   // 17.5.8.3
                fV_cp_1722_group);                      // 17.5.8.4

            // Lower design strength in shear
            float fV_d_design_min = fElasticityFactor_1764 * MathF.Min(
                fPhi_anchor_shear_174 * fV_s_17581_group,                            // 17.5.8.1
                fPhi_concrete_shear_174b * fV_cb_1716_group,                         // 17.5.8.2
                fPhi_concrete_shear_174b * iNumberAnchors_v * fV_cb_1716_single,     // 17.5.8.2
                fPhi_concrete_shear_174b * fV_cb_1721_group,                         // 17.5.8.3
                fPhi_concrete_shear_174b * iNumberAnchors_v * fV_cb_1721_single,     // 17.5.8.3
                fPhi_concrete_shear_174b * fV_cp_1722_group);                        // 17.5.8.4

            // 17.5.6.6 Interaction of tension and shear – simplified procedures
            // Group of anchors

            // 17.5.6.6(Eq. 17–5)
            float fDesignRatio_17566_group = eq_concrete.Eq_17566___(fN_asterix_joint_uplif, fN_d_design_min, fV_asterix_res_joint, fV_d_design_min);
            fEta_max = MathF.Max(fEta_max, fDesignRatio_17566_group);

            // C17.5.6.6 (Figure C17.1)
            bool bUseC17566Equation = true;
            float fDesignRatio_C17566_group;

            if (bUseC17566Equation)
            {
                fDesignRatio_C17566_group = eq_concrete.Eq_C17566__(fN_asterix_joint_uplif, fN_d_design_min, fV_asterix_res_joint, fV_d_design_min);
                fEta_max = MathF.Max(fEta_max, fDesignRatio_C17566_group);
            }

            // Footings
            float fGamma_F_uplift = 0.9f; // Load Factor -uplift
            float fGamma_F_bearing = 1.2f; // Load Factor - bearing

            //float fN_asterix_joint_uplif =
            //float fN_asterix_joint_bearing 

            float fc_nominal_soil_bearing_capacity = 58000f; // Pa - soil bearing capacity - TODO - user defined

            // Footing pad
            float fA_footing = fFootingDimension_x * fFootingDimension_y; // Area of footing pad
            float fV_footing = fA_footing * fFootingHeight;  // Volume of footing pad
            float fRho_c_footing = 2300f; // Density of dry concrete - foundations
            float fG_footing = fV_footing * fRho_c_footing; // Self-weight [N] - footing pad

            // Tributary floor volume
            float ft_floor = 0.125f; // TODO - user-defined
            float fa_tributary_floor = 0.6f; // TODO - tributary dimension;
            float fLength_x_tributary_floor = fFootingDimension_x + 2 * fa_tributary_floor; // TODO - zistit ci je stlp v rohu, ak ano uvazovat len jednu stranu
            float fLength_y_tributary_floor = fFootingDimension_y + fa_tributary_floor; // Okraj - uvazujem sa len jedna strana patky
            float fA_tributary_floor = fLength_x_tributary_floor * fLength_y_tributary_floor - fA_footing;
            float fV_tributary_floor = fA_tributary_floor * ft_floor;
            float fRho_c_floor = 2200f; // Density of dry concrete - concrete floor
            float fG_tributary_floor = fV_tributary_floor * fRho_c_floor; // Self-weight [N] - tributary concrete floor

            // Addiional material above the footing
            float ft_additional_material = 0.3f; // User-defined
            float fRho_additional_material = 2200f; // Can be concrete or soil
            float fVolume_additional_material = fA_footing * ft_additional_material;
            float fG_additional_material = fVolume_additional_material * fRho_additional_material; // Self-weight [N]

            // Uplift
            float fG_design_footing_uplift = fGamma_F_uplift * fG_footing;
            float fG_design_tributary_floor_uplift = fGamma_F_uplift * fG_design_footing_uplift;
            float fG_design_additional_material_uplift = fGamma_F_uplift * fG_additional_material;
            float fG_design_uplift = fG_design_footing_uplift + fG_design_tributary_floor_uplift + fG_design_additional_material_uplift;

            float fG_design_footing_bearing = fGamma_F_bearing * fG_footing;
            float fG_design_additional_material_bearing = fGamma_F_bearing * fG_additional_material;
            float fG_design_bearing = fG_design_footing_bearing + fG_design_additional_material_bearing;

            // Design ratio - uplift and bearing force
            float fDesignRatio_footing_uplift = fN_asterix_joint_uplif / fG_design_uplift;
            fEta_max = MathF.Max(fEta_max, fDesignRatio_footing_uplift);

            float fN_design_bearing_total = Math.Abs(fN_asterix_joint_bearing) + fG_design_bearing;
            float fPressure_bearing = fN_design_bearing_total / fA_footing;
            float fSafetyFactor = 0.5f; // TODO - zistit aky je faktor
            float fDesignRatio_footing_bearing = fPressure_bearing / (fSafetyFactor * fc_nominal_soil_bearing_capacity);
            fEta_max = MathF.Max(fEta_max, fDesignRatio_footing_bearing);

            // Bending - design of reinforcement
            // Reinforcement bars in x direction (parallel to the wall)
            float fq_linear_xDirection = Math.Abs(fN_design_bearing_total) / fFootingDimension_x;
            float fM_asterix_footingdesign_xDirection = fq_linear_xDirection * MathF.Pow2(fFootingDimension_x) / 8f; // ??? jednoducho podpoprety nosnik ???

            float fd_reinforcement_xDirection = 0.016f; // TODO - user defined
            float fd_reinforcement_yDirection = 0.016f; // TODO - user defined (default above the reinforcement in x-direction)
            float fA_s1_Xdirection = MathF.fPI * MathF.Pow2(fd_reinforcement_xDirection) / 4f; // Reinforcement bar cross-sectional area
            float fA_s1_Ydirection = MathF.fPI * MathF.Pow2(fd_reinforcement_yDirection) / 4f; // Reinforcement bar cross-sectional area
            int iNumberOfBarsInXDirection = 11; // TODO - user defined
            int iNumberOfBarsInYDirection = 11; // TODO - user defined

            float fA_s_tot_Xdirection = iNumberOfBarsInXDirection * fA_s1_Xdirection;
            float fA_s_tot_Ydirection = iNumberOfBarsInYDirection * fA_s1_Ydirection;

            float fConcreteCover_reinforcement_side = 0.075f;
            float fSpacing_xDirection = (fFootingDimension_x - 2 * fConcreteCover_reinforcement_side - fd_reinforcement_yDirection) / (iNumberOfBarsInYDirection - 1);
            float fSpacing_yDirection = (fFootingDimension_y - 2 * fConcreteCover_reinforcement_side - fd_reinforcement_xDirection) / (iNumberOfBarsInXDirection - 1);

            string sReinforcingSteelGrade_Name = "500E";
            float fReinforcementStrength_fy = 500e+6f; // TODO - user defined

            float fAlpha_c = 0.85f;
            float fPhi_b_foundations = 0.85f;

            float fConcreteCover_reinforcement_xDirection = 0.075f;

            float fd_effective_xDirection = fFootingHeight - fConcreteCover_reinforcement_xDirection - 0.5f * fd_reinforcement_xDirection;
            float fd_effective_yDirection = fFootingHeight - fConcreteCover_reinforcement_xDirection - fd_reinforcement_xDirection - 0.5f * fd_reinforcement_yDirection;
            float fx_u_xDirection = (fA_s1_Xdirection * fReinforcementStrength_fy) / (fAlpha_c * ff_apostrophe_c * fFootingDimension_y);
            float fM_b_Reincorcement_xDirection = fA_s_tot_Xdirection * fReinforcementStrength_fy * (fd_effective_xDirection - (fx_u_xDirection / 2f));
            float fM_b_Concrete_xDirection = fAlpha_c * ff_apostrophe_c * fFootingDimension_y * fx_u_xDirection * (fd_effective_xDirection - (fx_u_xDirection / 2f));
            float fM_b_footing_xDirection = Math.Min(fM_b_Reincorcement_xDirection, fM_b_Concrete_xDirection); // Note: Values should be identical.

            float fDesignRatio_bending_M_footing = fM_asterix_footingdesign_xDirection / (fPhi_b_foundations * fM_b_footing_xDirection);
            fEta_max = MathF.Max(fEta_max, fDesignRatio_bending_M_footing);

            // TODO - zapracovat Winklerov nosnik na pruznom podlozi je jednotlive patky, suvisly zakladovy pas zatazeny viacerymi stlpmi

            // Minimum reinforcement

            // | f´c (MPa) | fy = 300 MPa | fy = 500 MPa |
            // | 25        | 0.0047       | 0.0028       |
            // | 30        | 0.0047       | 0.0028       |
            // | 40        | 0.0053       | 0.0032       |
            // | 50        | 0.0059       | 0.0035       |

            // Minimum longitudinal reinforcement ratio
            float fp_ratio_xDirection = 2 * fA_s_tot_Xdirection / (fFootingDimension_y * fFootingHeight); // Same reinforcement at the bottom and top surface
            float fp_ratio_limit_minimum_xDirection = eq_concrete.Eq_9_1_ratio(ff_apostrophe_c, fReinforcementStrength_fy);

            float fDesignRatio_MinimumReinforcement_xDirection = fp_ratio_limit_minimum_xDirection / fp_ratio_xDirection;
            fEta_max = MathF.Max(fEta_max, fDesignRatio_MinimumReinforcement_xDirection);

            //  Shear
            float fV_asterix_footingdesign_shear = fq_linear_xDirection * fFootingDimension_x / 2f; // ??? jednoducho podpoprety nosnik ???
            float fA_cv_xDirection = fd_effective_xDirection * fFootingDimension_y;

            // pw - proportion of flexural tension reinforcement within one-quarter of the effective depth of the member closest to the extreme tension reinforcement to the shear area, Acv
            float fp_w_xDirection = fA_s_tot_Xdirection / fA_cv_xDirection;
            float fk_a = eq_concrete.Get_k_a_93934(0.02f); // TODO - user defined
            float fk_d = eq_concrete.Get_k_d_93934(); // TODO - dopracovat
            float fv_b_xDirection = eq_concrete.Get_v_b_93934(fp_w_xDirection, ff_apostrophe_c);
            float fv_c_xDirection = eq_concrete.Eq_9_5_____(fk_d, fk_a, fv_b_xDirection);
            float fV_c_xDirection = eq_concrete.Eq_9_4_____(fv_c_xDirection, fA_cv_xDirection);
            float fPhi_v_foundations = 0.85f;

            float fDesignRatio_shear_V_footing = fV_asterix_footingdesign_shear / (fPhi_v_foundations * fV_c_xDirection);
            fEta_max = MathF.Max(fEta_max, fDesignRatio_shear_V_footing);

            // Punching shear
            float fReactionArea_dimension_x = fplateWidth_x;
            float fReactionArea_dimension_y = fplateWidth_y;

            float fcriticalPerimeterDimension_x = 2f * Math.Min(fd_effective_xDirection, fe_x_BasePlateToFootingEdge) + fReactionArea_dimension_x;
            float fcriticalPerimeterDimension_y = 2f * Math.Min(fd_effective_yDirection, fe_y_BasePlateToFootingEdge) + fReactionArea_dimension_y; // TODO - Zohladnit ak je stlp blizsie k okraju nez fd

            float fcriticalPerimeter_b0 = 2 * fcriticalPerimeterDimension_x + 2 * fcriticalPerimeterDimension_y;

            // Ratio of the long side to the short side of the concentrated load
            float fBeta_c = Math.Max(fReactionArea_dimension_x, fReactionArea_dimension_y) / Math.Min(fReactionArea_dimension_x, fReactionArea_dimension_y); // 12.7
            float fAlpha_s = 15; // 20 for interior columns, 15 for edge columns, 10 for corner columns TODO - zapracovat identifikaciu stlpa
            float fd_average = (fd_effective_xDirection + fd_effective_yDirection) / 2f;
            float fk_ds = eq_concrete.Get_k_ds_12732(fd_average);

            // Nominal shear stress resisted by the concrete
            float fv_c_126 = eq_concrete.Eq_12_6____(fk_ds, fBeta_c, ff_apostrophe_c);
            float fv_c_127 = eq_concrete.Eq_12_7____(fk_ds, fAlpha_s, fd_average, fcriticalPerimeter_b0, ff_apostrophe_c);
            float fv_c_128 = eq_concrete.Eq_12_8____(fk_ds, ff_apostrophe_c);

            float fv_c_12732 = MathF.Min(fv_c_126, fv_c_127, fv_c_128);

            // 12.7.3.4 Maximum nominal shear stress
            float fv_c_max = 0.5f * MathF.Sqrt(ff_apostrophe_c);
            if (fv_c_12732 > fv_c_max)
                fv_c_12732 = fv_c_max;

            float fV_c_12731 = eq_concrete.Get_V_c_12731(fv_c_12732, fcriticalPerimeter_b0, fd_average);

            // 12.7.4 Shear reinforcement consisting of bars or wires or stirrups
            float fReinforcementArea_A_v_xDirection = 2 * fA_s_tot_Xdirection * fcriticalPerimeterDimension_y / fFootingDimension_y; // TODO ?? horna aj spodna vyztuz ak su rovnake
            float fReinforcementArea_A_v_yDirection = 2 * fA_s_tot_Ydirection * fcriticalPerimeterDimension_x / fFootingDimension_x;

            float ff_yv = fReinforcementStrength_fy;

            float fV_s_xDirection = fReinforcementArea_A_v_xDirection * ff_yv * fd_effective_xDirection / fSpacing_yDirection;
            float fV_s_yDirection = fReinforcementArea_A_v_yDirection * ff_yv * fd_effective_yDirection / fSpacing_xDirection;

            // 12.7.3.1 Nominal shear strength for punching shear
            float fV_n_12731_xDirection = eq_concrete.Eq_12_4____(fV_s_xDirection, fV_c_12731);
            float fDesignRatio_punching_12731_xDirection = eq_concrete.Eq_12_5____(Math.Abs(fN_asterix_joint_bearing), fPhi_v_foundations, fV_n_12731_xDirection);
            fEta_max = MathF.Max(fEta_max, fDesignRatio_punching_12731_xDirection);

            float fV_n_12731_yDirection = eq_concrete.Eq_12_4____(fV_s_yDirection, fV_c_12731);
            float fDesignRatio_punching_12731_yDirection = eq_concrete.Eq_12_5____(Math.Abs(fN_asterix_joint_bearing), fPhi_v_foundations, fV_n_12731_yDirection);
            fEta_max = MathF.Max(fEta_max, fDesignRatio_punching_12731_yDirection);
        }
    }
}
