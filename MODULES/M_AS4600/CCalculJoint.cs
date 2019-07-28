using System;
using System.Windows.Forms;
using BaseClasses;
using MATH;
using MATERIAL;
using CRSC;
using M_NZS3101;

namespace M_AS4600
{
    public class CCalculJoint
    {
        AS_4600 eq = new AS_4600(); // TODO Ondrej - toto by sa asi malo prerobit na staticke triedy, je to nejaka kniznica metod s rovnicami a tabulkovymi hodnotami
        NZS_3101 eq_concrete = new NZS_3101(); // TODO Ondrej - toto by sa asi malo prerobit na staticke triedy, je to nejaka kniznica metod s rovnicami a tabulkovymi hodnotami
        
        public CConnectionJointTypes joint;
        public designInternalForces sDIF;
        public designInternalForces_AS4600 sDIF_AS4600;
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
        public float fEta_max_Footing = 0;

        public CCalculJoint(bool bIsDebugging_temp, bool bUseCRSCGeometricalAxes, CConnectionJointTypes joint_temp, CModel model, designInternalForces sDIF_temp, bool bSaveDetails = false)
        { 
            if (joint_temp == null)
            {
                throw new ArgumentNullException("Joint object is not defined");
            }

            bIsDebugging = bIsDebugging_temp;
            joint = joint_temp;
            sDIF = sDIF_temp;

            // Set design internal forces according AS 4600 symbols of axes
            sDIF_AS4600.fN = sDIF_temp.fN;
            sDIF_AS4600.fN_c = sDIF_temp.fN_c;
            sDIF_AS4600.fN_t = sDIF_temp.fN_t;
            sDIF_AS4600.fT = sDIF_temp.fT;

            if (bUseCRSCGeometricalAxes)
            {
                sDIF_AS4600.fV_xu_xx = sDIF_temp.fV_yy;
                sDIF_AS4600.fV_yv_yy = sDIF_temp.fV_zz;
                sDIF_AS4600.fM_xu_xx = sDIF_temp.fM_yy;
                sDIF_AS4600.fM_yv_yy = sDIF_temp.fM_zz;
            }
            else
            {
                sDIF_AS4600.fV_xu_xx = sDIF_temp.fV_yu;
                sDIF_AS4600.fV_yv_yy = sDIF_temp.fV_zv;
                sDIF_AS4600.fM_xu_xx = sDIF_temp.fM_yu;
                sDIF_AS4600.fM_yv_yy = sDIF_temp.fM_zv;
            }

            CFoundation foundation = model.GetFoundationForJointFromModel(joint);
            CalculateDesignRatio(bIsDebugging, joint, foundation, sDIF_AS4600, bSaveDetails);
        }

        public void CalculateDesignRatio(bool bIsDebugging, CConnectionJointTypes joint_temp, CFoundation foundation, designInternalForces_AS4600 sDIF_AS400, bool bSaveDetails = false)
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

            if (crsc_mainMember.m_Mat is CMat_03_00) // Material is Steel
            {
                ff_yk_2_MainMember = ((CMat_03_00)crsc_mainMember.m_Mat).Get_f_yk_by_thickness(ft_2_crscmainMember);
                ff_uk_2_MainMember = ((CMat_03_00)crsc_mainMember.m_Mat).Get_f_uk_by_thickness(ft_2_crscmainMember);
            }
            else
            {
                throw new Exception("Invalid component material.");
            }

            if (joint_temp.m_SecondaryMembers != null && joint_temp.m_SecondaryMembers.Length > 0) // Some secondary member exists (otherwise it is base plate connection)
            {
                crsc_secMember = (CCrSc_TW)joint_temp.m_SecondaryMembers[0].CrScStart;
                ft_2_crscsecMember = (float)crsc_secMember.t_min;

                if (crsc_secMember.m_Mat is CMat_03_00) // Material is Steel
                {
                    ff_yk_2_SecondaryMember = ((CMat_03_00)crsc_secMember.m_Mat).Get_f_yk_by_thickness(ft_2_crscsecMember);
                    ff_uk_2_SecondaryMember = ((CMat_03_00)crsc_secMember.m_Mat).Get_f_uk_by_thickness(ft_2_crscsecMember);
                }
                else
                {
                    throw new Exception("Invalid component material.");
                }
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
            {
                CalculateDesignRatioApexOrKneeJoint(joint_temp, sDIF_AS4600, bSaveDetails); // Apex or Knee Joint
            }
            else if (joint_temp.m_SecondaryMembers != null)
            {
                if (joint_temp is CConnectionJoint_T001 || joint_temp is CConnectionJoint_T002 || joint_temp is CConnectionJoint_T003)
                {
                    CalculateDesignRatioGirtOrPurlinJoint(joint_temp, sDIF_AS4600, bSaveDetails); // purlin, girt or eave purlin
                }
                else if (joint_temp is CConnectionJoint_S001) // Front / back column connection to the main rafter
                {
                    CalculateDesignRatioFrontOrBackColumnToMainRafterJoint(joint_temp, sDIF_AS4600, bSaveDetails);
                }
                else
                {
                    // Exception - not defined type
                    throw new Exception("Joint type design is not implemented!");
                }
            }
            else if (joint_temp is CConnectionJoint_TA01 || joint_temp is CConnectionJoint_TB01 || joint_temp is CConnectionJoint_TC01 || joint_temp is CConnectionJoint_TD01)
            {
                CalculateDesignRatioBaseJoint(joint_temp, sDIF_AS4600, bSaveDetails); // Base plates (main column or front/back column connection to the foundation)
                CalculateDesignRatioBaseJointFooting(foundation, sDIF_AS4600, bSaveDetails); // Base plates (main column or front/back column connection to the foundation)
            }
            else
            {
                // Exception - not defined type
                throw new Exception("Joint type design is not implemented!");
            }
        }

        public void CalculateDesignRatioApexOrKneeJoint(CConnectionJointTypes joint_temp, designInternalForces_AS4600 sDIF_temp, bool bSaveDetails = false)
        {
            CJointDesignDetails_ApexOrKnee designDetails = new CJointDesignDetails_ApexOrKnee();

            // Bending Joint apex, knee joint

            int iNumberOfPlatesInJoint = joint.m_arrPlates.Length;

            float fN_oneside = 1f / iNumberOfPlatesInJoint * sDIF_temp.fN;
            float fM_xu_oneside = 1f / iNumberOfPlatesInJoint * sDIF_temp.fM_xu_xx;
            float fV_yv_oneside = 1f / iNumberOfPlatesInJoint * sDIF_temp.fV_yv_yy;

            // Plate design
            designDetails.fPhi_Plate = 0.65f; // TODO - overit ci je to spravne

            // Plate tension design
            designDetails.fA_n_plate = plate.fA_n;
            designDetails.fN_t_plate = eq.Eq_5423_2__(screw.Diameter_thread, plate.S_f_min, designDetails.fA_n_plate, ff_uk_1_plate);
            designDetails.fEta_N_t_5423_plate = eq.Eq_5423_1__(Math.Abs(fN_oneside), designDetails.fPhi_Plate, designDetails.fN_t_plate);
            fEta_max = MathF.Max(fEta_max, designDetails.fEta_N_t_5423_plate);

            // Plate shear resistance
            designDetails.fA_vn_yv_plate = plate.fA_vn_zv;
            designDetails.fV_y_yv_plate = eq.Eq_723_5___(designDetails.fA_vn_yv_plate, ff_yk_1_plate);
            designDetails.fEta_V_yv_3341_plate = eq.Eq_3341____(fV_yv_oneside, designDetails.fPhi_Plate, designDetails.fV_y_yv_plate);
            fEta_max = MathF.Max(fEta_max, designDetails.fEta_V_yv_3341_plate);

            // Plate bending resistance
            designDetails.fM_xu_resitance_plate = eq.Eq_7222_4__(joint.m_arrPlates[0].fW_el_yu, ff_yk_1_plate);
            float fDesignResistance_M_plate;
            eq.Eq_723_10__(Math.Abs(fM_xu_oneside), designDetails.fPhi_Plate, designDetails.fM_xu_resitance_plate, out fDesignResistance_M_plate, out designDetails.fEta_Mb_plate);
            fEta_max = MathF.Max(fEta_max, designDetails.fEta_Mb_plate);

            // Connection -shear force design
            // Shear in connection
            designDetails.fPhi_shear_screw = 0.5f;
            designDetails.fVb_MainMember = eq.Get_Vb_5424(ft_1_plate, ft_2_crscmainMember, screw.Diameter_thread, ff_uk_1_plate, ff_uk_2_MainMember);
            designDetails.fVb_SecondaryMember = eq.Get_Vb_5424(ft_1_plate, ft_2_crscsecMember, screw.Diameter_thread, ff_uk_1_plate, ff_uk_2_SecondaryMember);

            designDetails.iNumberOfScrewsInShear = joint_temp.m_arrPlates[0].ScrewArrangement.Screws.Length; // Temporary

            designDetails.fEta_MainMember = Math.Abs(sDIF_temp.fV_yv_yy) / (designDetails.iNumberOfScrewsInShear * designDetails.fVb_MainMember);
            designDetails.fEta_SecondaryMember = Math.Abs(sDIF_temp.fV_yv_yy) / (designDetails.iNumberOfScrewsInShear * designDetails.fVb_SecondaryMember);

            // Bending and shear and normal force interaction - shear design of screws
            designDetails.fMb_MainMember_oneside_plastic = 0;
            designDetails.fMb_SecondaryMember_oneside_plastic = 0;

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
                designDetails.fMb_MainMember_oneside_plastic += fHolesCentersRadiiInOneGroup[i] * designDetails.fVb_MainMember;
                designDetails.fMb_SecondaryMember_oneside_plastic += fHolesCentersRadiiInOneGroup[i] * designDetails.fVb_SecondaryMember;

                fSumri2tormax += MathF.Pow2(fHolesCentersRadiiInOneGroup[i]) / fr_max;
            }

            // Plastic resistance (Design Ratio)
            designDetails.fEta_Mb_MainMember_oneside_plastic = Math.Abs(fM_xu_oneside) / designDetails.fMb_MainMember_oneside_plastic;
            fEta_max = MathF.Max(fEta_max, designDetails.fEta_Mb_MainMember_oneside_plastic);
            designDetails.fEta_Mb_SecondaryMember_oneside_plastic = Math.Abs(fM_xu_oneside) / designDetails.fMb_SecondaryMember_oneside_plastic;
            fEta_max = MathF.Max(fEta_max, designDetails.fEta_Mb_SecondaryMember_oneside_plastic);

            // Elastic resistance
            designDetails.fV_asterix_b_max_screw_Mxu = Math.Abs(fM_xu_oneside) / fSumri2tormax;
            designDetails.fV_asterix_b_max_screw_Vyv = Math.Abs(fV_yv_oneside) / fHolesCentersRadiiInOneGroup.Length;
            designDetails.fV_asterix_b_max_screw_N = Math.Abs(fN_oneside) / fHolesCentersRadiiInOneGroup.Length;

            designDetails.fV_asterix_b_max_screw = 0;

            if (designDetails.fV_asterix_b_max_screw_Mxu != 0 && designDetails.fV_asterix_b_max_screw_Vyv != 0 && designDetails.fV_asterix_b_max_screw_N != 0)
                designDetails.fV_asterix_b_max_screw = MathF.Sqrt(MathF.Sqrt(MathF.Pow2(designDetails.fV_asterix_b_max_screw_Mxu) + MathF.Pow2(designDetails.fV_asterix_b_max_screw_Vyv)) + MathF.Pow2(designDetails.fV_asterix_b_max_screw_N));
            else if ((designDetails.fV_asterix_b_max_screw_Mxu != 0 || designDetails.fV_asterix_b_max_screw_Vyv != 0) && designDetails.fV_asterix_b_max_screw_N == 0)
                designDetails.fV_asterix_b_max_screw = MathF.Sqrt(MathF.Pow2(designDetails.fV_asterix_b_max_screw_Mxu) + MathF.Pow2(designDetails.fV_asterix_b_max_screw_Vyv));
            else if ((designDetails.fV_asterix_b_max_screw_Mxu != 0 || designDetails.fV_asterix_b_max_screw_N != 0) && designDetails.fV_asterix_b_max_screw_Vyv == 0)
                designDetails.fV_asterix_b_max_screw = MathF.Sqrt(MathF.Pow2(designDetails.fV_asterix_b_max_screw_Mxu) + MathF.Pow2(designDetails.fV_asterix_b_max_screw_N));
            else if ((designDetails.fV_asterix_b_max_screw_Vyv != 0 || designDetails.fV_asterix_b_max_screw_N != 0) && designDetails.fV_asterix_b_max_screw_Mxu == 0)
                designDetails.fV_asterix_b_max_screw = MathF.Sqrt(MathF.Pow2(designDetails.fV_asterix_b_max_screw_Vyv) + MathF.Pow2(designDetails.fV_asterix_b_max_screw_N));
            else
                designDetails.fV_asterix_b_max_screw = designDetails.fV_asterix_b_max_screw_Mxu + designDetails.fV_asterix_b_max_screw_Vyv + designDetails.fV_asterix_b_max_screw_N; // Vsetky alebo len jedna zlozka je nenulova, mozeme pouzit sumu

            designDetails.fEta_Vb_5424_MainMember = eq.Eq_5424_1__(designDetails.fV_asterix_b_max_screw, designDetails.fPhi_shear_screw, designDetails.fVb_MainMember);
            fEta_max = MathF.Max(fEta_max, designDetails.fEta_Vb_5424_MainMember);

            designDetails.fEta_Vb_5424_SecondaryMember = eq.Eq_5424_1__(designDetails.fV_asterix_b_max_screw, designDetails.fPhi_shear_screw, designDetails.fVb_SecondaryMember);
            fEta_max = MathF.Max(fEta_max, designDetails.fEta_Vb_5424_SecondaryMember);

            designDetails.fEta_Vb_5424 = Math.Max(designDetails.fEta_Vb_5424_MainMember, designDetails.fEta_Vb_5424_SecondaryMember);

            // 5.4.2.5 Connection shear as limited by end distance
            designDetails.fe = 0.03f; // TODO - temporary - urcit min vzdialenost skrutky od okraja plechu alebo prierezu
            designDetails.fV_fv_MainMember = eq.Eq_5425_2__(ft_2_crscmainMember, designDetails.fe, ff_uk_2_MainMember);
            designDetails.fV_fv_SecondaryMember = eq.Eq_5425_2__(ft_2_crscsecMember, designDetails.fe, ff_uk_2_SecondaryMember);
            designDetails.fV_fv_Plate = eq.Eq_5425_2__(ft_1_plate, designDetails.fe, ff_uk_1_plate);

            // Distance to an end of the connected part is parallel to the line of the applied force
            // Nemalo by rozhodovat pre moment (skrutka namahana rovnobezne s okrajom je uprostred plechu) ale moze rozhovat pre N a V

            designDetails.fV_asterix_fv = 0;

            if (designDetails.fV_asterix_b_max_screw_Vyv != 0 || designDetails.fV_asterix_b_max_screw_N != 0)
                designDetails.fV_asterix_fv = MathF.Sqrt(MathF.Pow2(designDetails.fV_asterix_b_max_screw_Vyv) + MathF.Pow2(designDetails.fV_asterix_b_max_screw_N));

            designDetails.fEta_V_fv_5425_MainMember = eq.Eq_5425_1__(designDetails.fV_asterix_fv, designDetails.fV_fv_MainMember, ff_uk_2_MainMember, ff_yk_2_MainMember);
            designDetails.fEta_V_fv_5425_SecondaryMember = eq.Eq_5425_1__(designDetails.fV_asterix_fv, designDetails.fV_fv_SecondaryMember, ff_uk_2_SecondaryMember, ff_yk_2_SecondaryMember);
            designDetails.fEta_V_fv_5425_Plate = eq.Eq_5425_1__(designDetails.fV_asterix_fv, designDetails.fV_fv_Plate, ff_uk_1_plate, ff_yk_1_plate);

            designDetails.fEta_V_fv_5425 = MathF.Max(designDetails.fEta_V_fv_5425_MainMember, designDetails.fEta_V_fv_5425_SecondaryMember, designDetails.fEta_V_fv_5425_Plate);
            fEta_max = MathF.Max(fEta_max, designDetails.fEta_V_fv_5425);

            // 5.4.2.6 Screws in shear
            // The design shear capacity φVw of the screw shall be determined by testing in accordance with Section 8.

            designDetails.fV_w_nom_screw_5426 = screw.ShearStrength_nominal; // N
            designDetails.fEta_V_w_5426 = Math.Max(designDetails.fV_asterix_b_max_screw, designDetails.fV_asterix_fv) / (0.5f * designDetails.fV_w_nom_screw_5426);
            fEta_max = MathF.Max(fEta_max, designDetails.fEta_V_w_5426);

            int iNumberOfDecimalPlaces = 3;
            if (bIsDebugging)
                MessageBox.Show("Calculation finished.\n"
                              + "Design Ratio η = " + Math.Round(designDetails.fEta_Vb_5424, iNumberOfDecimalPlaces) + " [-]" + "\t" + " 5.4.2.4" + "\n"
                              + "Design Ratio η = " + Math.Round(designDetails.fEta_V_fv_5425, iNumberOfDecimalPlaces) + " [-]" + "\t" + " 5.4.2.5" + "\n"
                              + "Design Ratio η max = " + Math.Round(fEta_max, iNumberOfDecimalPlaces) + " [-]");

            // Tension in members
            designDetails.fPhi_CrSc = 0.65f; // TODO - overit ci je to spravne
            // 5.4.2.3 Tension in the connected part
            designDetails.fA_n_MainMember = (float)crsc_mainMember.A_g - plate.INumberOfConnectorsInSection * 2 * screw.Diameter_thread; // TODO - spocitat presne podla poctu a rozmeru otvorov v jednom reze
            designDetails.fN_t_section_MainMember = eq.Eq_5423_2__(screw.Diameter_thread, plate.S_f_min, designDetails.fA_n_MainMember, ff_uk_2_MainMember);
            designDetails.fEta_N_t_5423_MainMember = eq.Eq_5423_1__(sDIF_temp.fN_t, designDetails.fPhi_CrSc, designDetails.fN_t_section_MainMember);
            fEta_max = MathF.Max(fEta_max, designDetails.fEta_N_t_5423_MainMember);

            designDetails.fA_n_SecondaryMember = (float)crsc_secMember.A_g - plate.INumberOfConnectorsInSection * 2 * screw.Diameter_thread; // TODO - spocitat presne podla poctu a rozmeru otvorov v jednom reze
            designDetails.fN_t_section_SecondaryMember = eq.Eq_5423_2__(screw.Diameter_thread, plate.S_f_min, designDetails.fA_n_SecondaryMember, ff_uk_2_SecondaryMember);
            designDetails.fEta_N_t_5423_SecondaryMember = eq.Eq_5423_1__(sDIF_temp.fN_t, designDetails.fPhi_CrSc, designDetails.fN_t_section_SecondaryMember);
            fEta_max = MathF.Max(fEta_max, designDetails.fEta_N_t_5423_SecondaryMember);

            // Validation - negative design ratio
            if (designDetails.fEta_N_t_5423_plate < 0 ||
                designDetails.fEta_V_yv_3341_plate < 0 ||
                designDetails.fEta_Mb_plate < 0 ||
                designDetails.fEta_MainMember < 0 ||
                designDetails.fEta_SecondaryMember < 0 ||
                designDetails.fEta_Mb_MainMember_oneside_plastic < 0 ||
                designDetails.fEta_Mb_SecondaryMember_oneside_plastic < 0 ||
                designDetails.fEta_Vb_5424_MainMember < 0 ||
                designDetails.fEta_Vb_5424_SecondaryMember < 0 ||
                designDetails.fEta_V_fv_5425_MainMember < 0 ||
                designDetails.fEta_V_fv_5425_SecondaryMember < 0 ||
                designDetails.fEta_V_fv_5425_Plate < 0 ||
                designDetails.fEta_V_fv_5425 < 0 ||
                designDetails.fEta_V_w_5426 < 0 ||
                designDetails.fEta_N_t_5423_MainMember < 0 ||
                designDetails.fEta_N_t_5423_SecondaryMember < 0)
            {
                throw new Exception("Design ratio is invalid!");
            }

            // Validation - infinity design ratio
            if (fEta_max > 9e+10)
            {
                throw new Exception("Design ratio is invalid!");
            }

            // Store details
            if (bSaveDetails)
                joint_temp.DesignDetails = designDetails;
        }

        public void CalculateDesignRatioGirtOrPurlinJoint(CConnectionJointTypes joint_temp, designInternalForces_AS4600 sDIF_temp, bool bSaveDetails = false)
        {
            CJointDesignDetails_GirtOrPurlin designDetails = new CJointDesignDetails_GirtOrPurlin();

            bool bDisplayWarningForContitions5434and5435 = false;
            // Purlins, girts .....
            designDetails.iNumberOfScrewsInTension = plate.ScrewArrangement.IHolesNumber;

            // 5.4.3 Screwed connections in tension
            // 5.4.3.2 Pull-out and pull-over (pull-through)
            designDetails.fPhi_N_screw = 0.5f;
            // K vytiahnutiu alebo pretlaceniu moze dost v pripojeni k main member alebo pri posobeni sily Vx(Vy) na secondary member (to asi zanedbame)
            designDetails.fN_t_5432_MainMember = eq.Get_Nt_5432(screw.Type, ft_1_plate, ft_2_crscmainMember, screw.Diameter_thread, screw.D_h_headdiameter, screw.T_w_washerthickness, screw.D_w_washerdiameter, ff_uk_1_plate, ff_uk_2_MainMember);
            designDetails.fEta_N_t_5432_MainMember = eq.Eq_5432_1__(sDIF_temp.fN_t / designDetails.iNumberOfScrewsInTension, designDetails.fPhi_N_screw, designDetails.fN_t_5432_MainMember);
            fEta_max = MathF.Max(fEta_max, designDetails.fEta_N_t_5432_MainMember);

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
            designDetails.fC_for5434_MainMember = eq.Get_C_Tab_5424(screw.Diameter_thread, ft_2_crscmainMember);
            designDetails.fV_b_for5434_MainMember = eq.Eq_5424_6__(designDetails.fC_for5434_MainMember, ft_2_crscmainMember, screw.Diameter_thread, ff_uk_2_MainMember); // Eq. 5.4.2.4(6)
            designDetails.fd_w_for5434_plate = eq.Get_d_apostrophe_w(screw.Type, ft_1_plate, screw.D_h_headdiameter, screw.T_w_washerthickness, screw.D_w_washerdiameter);
            designDetails.fN_ov_for5434_plate = eq.Eq_5432_3__(ft_1_plate, screw.D_w_washerdiameter, ff_uk_1_plate); // 5.4.3.2(b) Eq. 5.4.3.2(3) - Nov

            bool bIsEccentricallyLoadedJoint = false;

            if (bIsEccentricallyLoadedJoint)
                designDetails.fN_ov_for5434_plate *= 0.5f; // Use 50% of resistance value in case of eccentrically loaded connection

            float fV_asterix_b_for5434_MainMember = 0;

            if (sDIF_temp.fV_xu_xx != 0 || sDIF_temp.fV_yv_yy != 0)
                fV_asterix_b_for5434_MainMember = MathF.Sqrt(MathF.Pow2(sDIF_temp.fV_xu_xx / designDetails.iNumberOfScrewsInTension) + MathF.Pow2(sDIF_temp.fV_yv_yy / designDetails.iNumberOfScrewsInTension));

            designDetails.fPhi_shear_Vb_Nov = 0.65f;
            designDetails.fEta_5434_MainMember = eq.Eq_5434____(fV_asterix_b_for5434_MainMember, sDIF_temp.fN_t / designDetails.iNumberOfScrewsInTension, designDetails.fPhi_shear_Vb_Nov, designDetails.fV_b_for5434_MainMember, designDetails.fN_ov_for5434_plate);
            fEta_max = MathF.Max(fEta_max, designDetails.fEta_5434_MainMember);

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
            designDetails.fV_b_for5435_MainMember = eq.Get_Vb_5424(ft_1_plate, ft_2_crscmainMember, screw.Diameter_thread, ff_uk_1_plate, ff_uk_2_MainMember);
            designDetails.fN_ou_for5435_MainMember = eq.Eq_5432_2__(ft_2_crscmainMember, screw.Diameter_thread, ff_uk_2_MainMember); // 5.4.3.2(a) Eq. 5.4.3.2(2) - Nou

            designDetails.fV_asterix_b_for5435_MainMember = 0;

            if (sDIF_temp.fV_xu_xx != 0 || sDIF_temp.fV_yv_yy != 0)
                designDetails.fV_asterix_b_for5435_MainMember = MathF.Sqrt(MathF.Pow2(sDIF_temp.fV_xu_xx / designDetails.iNumberOfScrewsInTension) + MathF.Pow2(sDIF_temp.fV_yv_yy / designDetails.iNumberOfScrewsInTension));

            designDetails.fEta_5435_MainMember = eq.Eq_5435____(designDetails.fV_asterix_b_for5435_MainMember, sDIF_temp.fN_t / designDetails.iNumberOfScrewsInTension, 0.6f, designDetails.fV_b_for5435_MainMember, designDetails.fN_ou_for5435_MainMember);
            fEta_max = MathF.Max(fEta_max, designDetails.fEta_5435_MainMember);

            // 5.4.2.5 Connection shear as limited by end distance
            designDetails.fe_Plate = 0.03f; // TODO - temporary - urcit min vzdialenost skrutky od okraja plechu

            // Distance to an end of the connected part is parallel to the line of the applied force
            designDetails.fV_asterix_fv_plate = Math.Abs(sDIF_temp.fV_yv_yy / designDetails.iNumberOfScrewsInTension);
            designDetails.fV_fv_Plate = eq.Eq_5425_2__(ft_1_plate, designDetails.fe_Plate, ff_uk_1_plate);
            designDetails.fEta_V_fv_5425_Plate = eq.Eq_5425_1__(designDetails.fV_asterix_fv_plate, designDetails.fV_fv_Plate, ff_uk_1_plate, ff_yk_1_plate);
            fEta_max = MathF.Max(fEta_max, designDetails.fEta_V_fv_5425_Plate);

            // 5.4.2.6 Screws in shear
            // The design shear capacity φVw of the screw shall be determined by testing in accordance with Section 8.

            designDetails.fPhi_V_screw = 0.5f;
            designDetails.fV_w_nom_screw_5426 = screw.ShearStrength_nominal; // N
            designDetails.fEta_V_w_5426 = (sDIF_temp.fN_t / designDetails.iNumberOfScrewsInTension) / (designDetails.fPhi_V_screw * designDetails.fV_w_nom_screw_5426);
            fEta_max = MathF.Max(fEta_max, designDetails.fEta_V_w_5426);

            // 5.4.3.3 Screws in tension
            // The tensile capacity of the screw shall be determined by testing in accordance with Section 8.
            designDetails.fPhi_N_t_screw = 0.5f;
            designDetails.fN_t_nom_screw_5433 = screw.AxialTensileStrength_nominal; // N
            designDetails.fEta_N_t_screw_5433 = Math.Max(designDetails.fV_asterix_b_for5435_MainMember, designDetails.fV_asterix_fv_plate) / (designDetails.fPhi_N_t_screw * designDetails.fN_t_nom_screw_5433);
            fEta_max = MathF.Max(fEta_max, designDetails.fEta_V_w_5426);

            // 5.4.3.6 Screws subject to combined shear and tension
            // A screw required to resist simultaneously a design shear force and a design tensile where V screw and N screw shall be determined by testing in accordance with Section 8.

            designDetails.fEta_V_N_t_screw_5436 = eq.Eq_5436____(Math.Max(designDetails.fV_asterix_b_for5435_MainMember, designDetails.fV_asterix_fv_plate), (sDIF_temp.fN_t / designDetails.iNumberOfScrewsInTension), designDetails.fPhi_N_t_screw, designDetails.fV_w_nom_screw_5426, designDetails.fN_t_nom_screw_5433);
            fEta_max = MathF.Max(fEta_max, designDetails.fEta_V_N_t_screw_5436);

            // Plate design
            int iNumberOfPlatesInJoint = joint.m_arrPlates.Length;

            float fN_oneside = 1f / iNumberOfPlatesInJoint * sDIF_temp.fN;
            float fV_yv_oneside = 1f / iNumberOfPlatesInJoint * sDIF_temp.fV_yv_yy;

            // Plate tension design
            designDetails.fPhi_plate = 0.65f;
            designDetails.fA_n_plate = plate.fA_n;
            designDetails.fN_t_plate = eq.Eq_5423_2__(screw.Diameter_thread, plate.S_f_min, designDetails.fA_n_plate, ff_uk_1_plate);
            designDetails.fEta_N_t_5423_plate = eq.Eq_5423_1__(fN_oneside, designDetails.fPhi_plate, designDetails.fN_t_plate);
            fEta_max = MathF.Max(fEta_max, designDetails.fEta_N_t_5423_plate);

            // Plate shear resistance
            designDetails.fA_vn_yv_plate = plate.fA_vn_zv;
            designDetails.fV_y_yv_plate = eq.Eq_723_5___(designDetails.fA_vn_yv_plate, ff_yk_1_plate);
            designDetails.fEta_V_yv_3341_plate = eq.Eq_3341____(fV_yv_oneside, designDetails.fPhi_plate, designDetails.fV_y_yv_plate);
            fEta_max = MathF.Max(fEta_max, designDetails.fEta_V_yv_3341_plate);

            // Pripoj plechu sekundarneho pruta
            designDetails.iNumberOfScrewsInConnectionOfSecondaryMember = 16; // Temporary (pocet plechov * pocet skrutiek v jednom ramene pripojneho plechu = 2 * 8)

            // Shear
            designDetails.fV_asterix_b_SecondaryMember = 0;

            if (sDIF_temp.fV_xu_xx != 0 || sDIF_temp.fV_yv_yy != 0 || sDIF_temp.fN != 0)
                designDetails.fV_asterix_b_SecondaryMember = MathF.Sqrt(MathF.Pow2(sDIF_temp.fV_yv_yy / designDetails.iNumberOfScrewsInConnectionOfSecondaryMember) + MathF.Pow2(sDIF_temp.fN / designDetails.iNumberOfScrewsInConnectionOfSecondaryMember));

            designDetails.fVb_SecondaryMember = eq.Get_Vb_5424(ft_1_plate, ft_2_crscsecMember, screw.Diameter_thread, ff_uk_1_plate, ff_uk_2_SecondaryMember);
            designDetails.fEta_Vb_5424_SecondaryMember = eq.Eq_5424_1__(designDetails.fV_asterix_b_SecondaryMember, designDetails.fPhi_V_screw, designDetails.fVb_SecondaryMember);
            fEta_max = MathF.Max(fEta_max, designDetails.fEta_Vb_5424_SecondaryMember);

            // Tension force in secondary member, distance between end of member and screw
            designDetails.fe_SecondaryMember = 0.03f; // TODO - temporary - urcit min vzdialenost skrutky od okraja nosnika
            designDetails.fV_asterix_fv_SecondaryMember = Math.Abs(sDIF_temp.fN / designDetails.iNumberOfScrewsInConnectionOfSecondaryMember);
            designDetails.fV_fv_SecondaryMember = eq.Eq_5425_2__(ft_2_crscsecMember, designDetails.fe_SecondaryMember, ff_uk_2_SecondaryMember);
            designDetails.fEta_V_fv_5425_SecondaryMember = eq.Eq_5425_1__(designDetails.fV_asterix_fv_SecondaryMember, designDetails.fV_fv_SecondaryMember, ff_uk_2_SecondaryMember, ff_yk_2_SecondaryMember);
            fEta_max = MathF.Max(fEta_max, designDetails.fEta_V_fv_5425_SecondaryMember);

            // Validation - negative design ratio
            if (designDetails.fEta_N_t_5432_MainMember < 0 ||
                designDetails.fEta_5434_MainMember < 0 ||
                designDetails.fEta_5435_MainMember < 0 ||
                designDetails.fEta_V_fv_5425_Plate < 0 ||
                designDetails.fEta_V_w_5426 < 0 ||
                designDetails.fEta_N_t_screw_5433 < 0 ||
                designDetails.fEta_V_N_t_screw_5436 < 0 ||
                designDetails.fEta_N_t_5423_plate < 0 ||
                designDetails.fEta_V_yv_3341_plate < 0 ||
                designDetails.fEta_Vb_5424_SecondaryMember < 0 ||
                designDetails.fEta_V_fv_5425_SecondaryMember < 0)
            {
                throw new Exception("Design ratio is invalid!");
            }

            // Validation - infinity design ratio
            if (fEta_max > 9e+10)
            {
                throw new Exception("Design ratio is invalid!");
            }

            // Store details
            if (bSaveDetails)
                joint_temp.DesignDetails = designDetails;
        }

        public void CalculateDesignRatioFrontOrBackColumnToMainRafterJoint(CConnectionJointTypes joint_temp, designInternalForces_AS4600 sDIF_temp, bool bSaveDetails = false)
        {
            CJointDesignDetails_FrontOrBackColumnToMainRafterJoint designDetails = new CJointDesignDetails_FrontOrBackColumnToMainRafterJoint();

            // TODO - refactoring s CalculateDesignRatioGirtOrPurlinJoint
            bool bDisplayWarningForContitions5434and5435 = false;
            designDetails.iNumberOfScrewsInTension = plate.ScrewArrangement.IHolesNumber * 2 / 3; // TODO - urcit presny pocet skrutiek v spoji ktore su pripojene k main member a ktore k secondary member, tahovu silu prenasaju skrutky pripojene k main member

            CConCom_Plate_N plateN = (CConCom_Plate_N)joint_temp.m_arrPlates[0];

            // Tension force in plate (metal strip)
            designDetails.fPhi_N_screw = 0.5f;
            float fDIF_N_plate = Math.Abs(sDIF_temp.fV_yv_yy) / (float)Math.Sin(plateN.Alpha1_rad);
            float fDIF_V_connection_one_side = fDIF_N_plate * (float)Math.Cos(plateN.Alpha1_rad);

            // 5.4.3 Screwed connections in tension
            // 5.4.3.2 Pull-out and pull-over (pull-through)

            // K vytiahnutiu alebo pretlaceniu moze dost v pripojeni k main member alebo pri posobeni sily Vx(Vy) na secondary member (to asi zanedbame)

            designDetails.fN_t_5432_MainMember = eq.Get_Nt_5432(screw.Type, ft_1_plate, ft_2_crscmainMember, screw.Diameter_thread, screw.D_h_headdiameter, screw.T_w_washerthickness, screw.D_w_washerdiameter, ff_uk_1_plate, ff_uk_2_MainMember);
            designDetails.fEta_N_t_5432_MainMember = eq.Eq_5432_1__(sDIF_temp.fV_yv_yy / designDetails.iNumberOfScrewsInTension, designDetails.fPhi_N_screw, designDetails.fN_t_5432_MainMember);
            fEta_max = MathF.Max(fEta_max, designDetails.fEta_N_t_5432_MainMember);

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
            designDetails.fPhi_shear_Vb_Nov = 0.65f;
            designDetails.fC_for5434_MainMember = eq.Get_C_Tab_5424(screw.Diameter_thread, ft_2_crscmainMember);
            designDetails.fV_b_for5434_MainMember = eq.Eq_5424_6__(designDetails.fC_for5434_MainMember, ft_2_crscmainMember, screw.Diameter_thread, ff_uk_2_MainMember); // Eq. 5.4.2.4(6)
            designDetails.fd_w_for5434_plate = eq.Get_d_apostrophe_w(screw.Type, ft_1_plate, screw.D_h_headdiameter, screw.T_w_washerthickness, screw.D_w_washerdiameter);
            designDetails.fN_ov_for5434_plate = eq.Eq_5432_3__(ft_1_plate, screw.D_w_washerdiameter, ff_uk_1_plate); // 5.4.3.2(b) Eq. 5.4.3.2(3) - Nov

            bool bIsEccentricallyLoadedJoint = false;

            if (bIsEccentricallyLoadedJoint)
                designDetails.fN_ov_for5434_plate *= 0.5f; // Use 50% of resistance value in case of eccentrically loaded connection

            designDetails.fV_asterix_b_for5434_MainMember = fDIF_V_connection_one_side / (designDetails.iNumberOfScrewsInTension / 2);
            designDetails.fEta_5434_MainMember = eq.Eq_5434____(designDetails.fV_asterix_b_for5434_MainMember, Math.Abs(sDIF_temp.fV_yv_yy) / designDetails.iNumberOfScrewsInTension, designDetails.fPhi_shear_Vb_Nov, designDetails.fV_b_for5434_MainMember, designDetails.fN_ov_for5434_plate);
            fEta_max = MathF.Max(fEta_max, designDetails.fEta_5434_MainMember);

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
            designDetails.fV_b_for5435_MainMember = eq.Get_Vb_5424(ft_1_plate, ft_2_crscmainMember, screw.Diameter_thread, ff_uk_1_plate, ff_uk_2_MainMember);
            designDetails.fN_ou_for5435_MainMember = eq.Eq_5432_2__(ft_2_crscmainMember, screw.Diameter_thread, ff_uk_2_MainMember); // 5.4.3.2(a) Eq. 5.4.3.2(2) - Nou

            designDetails.fV_asterix_b_for5435_MainMember = fDIF_V_connection_one_side / (designDetails.iNumberOfScrewsInTension / 2);
            designDetails.fEta_5435_MainMember = eq.Eq_5435____(designDetails.fV_asterix_b_for5435_MainMember, Math.Abs(sDIF_temp.fV_yv_yy), 0.6f, designDetails.fV_b_for5435_MainMember, designDetails.fN_ou_for5435_MainMember);
            fEta_max = MathF.Max(fEta_max, designDetails.fEta_5435_MainMember);

            // 5.4.2.5 Connection shear as limited by end distance
            designDetails.fe_Plate = 0.03f; // TODO - temporary - urcit min vzdialenost skrutky od okraja plechu

            // Distance to an end of the connected part is parallel to the line of the applied force
            designDetails.fV_asterix_fv_plate = fDIF_V_connection_one_side / (designDetails.iNumberOfScrewsInTension / 2);
            designDetails.fV_fv_Plate = eq.Eq_5425_2__(ft_1_plate, designDetails.fe_Plate, ff_uk_1_plate);
            designDetails.fEta_V_fv_5425_Plate = eq.Eq_5425_1__(designDetails.fV_asterix_fv_plate, designDetails.fV_fv_Plate, ff_uk_1_plate, ff_yk_1_plate);
            fEta_max = MathF.Max(fEta_max, designDetails.fEta_V_fv_5425_Plate);

            // 5.4.2.6 Screws in shear
            // The design shear capacity φVw of the screw shall be determined by testing in accordance with Section 8.

            designDetails.fV_w_nom_screw_5426 = screw.ShearStrength_nominal; // N
            designDetails.fEta_V_w_5426 = Math.Max(designDetails.fV_asterix_b_for5435_MainMember, designDetails.fV_asterix_fv_plate) / (0.5f * designDetails.fV_w_nom_screw_5426);
            fEta_max = MathF.Max(fEta_max, designDetails.fEta_V_w_5426);

            // 5.4.3.3 Screws in tension
            // The tensile capacity of the screw shall be determined by testing in accordance with Section 8.

            designDetails.fN_t_nom_screw_5433 = screw.AxialTensileStrength_nominal; // N
            designDetails.fEta_N_t_screw_5433 = (Math.Abs(sDIF_temp.fV_yv_yy) / (designDetails.iNumberOfScrewsInTension / 2)) / (0.5f * designDetails.fN_t_nom_screw_5433);
            fEta_max = MathF.Max(fEta_max, designDetails.fEta_V_w_5426);

            // 5.4.3.6 Screws subject to combined shear and tension
            // A screw required to resist simultaneously a design shear force and a design tensile where V screw and N screw shall be determined by testing in accordance with Section 8.

            designDetails.fEta_V_N_t_screw_5436 = eq.Eq_5436____(Math.Max(designDetails.fV_asterix_b_for5435_MainMember, designDetails.fV_asterix_fv_plate), Math.Abs(sDIF_temp.fV_yv_yy) / (designDetails.iNumberOfScrewsInTension / 2), 0.5f, designDetails.fV_w_nom_screw_5426, designDetails.fN_t_nom_screw_5433);
            fEta_max = MathF.Max(fEta_max, designDetails.fEta_V_N_t_screw_5436);

            // Plate design
            // Plate tension design
            designDetails.fPhi_plate = 0.65f;
            designDetails.fA_n_plate = plate.fA_n;
            designDetails.fN_t_plate = eq.Eq_5423_2__(screw.Diameter_thread, plate.S_f_min, designDetails.fA_n_plate, ff_uk_1_plate);
            designDetails.fEta_N_t_5423_plate = eq.Eq_5423_1__(fDIF_N_plate, designDetails.fPhi_plate, designDetails.fN_t_plate);
            fEta_max = MathF.Max(fEta_max, designDetails.fEta_N_t_5423_plate);

            // Validation - negative design ratio
            if (designDetails.fEta_N_t_5432_MainMember < 0 ||
                designDetails.fEta_5434_MainMember < 0 ||
                designDetails.fEta_5435_MainMember < 0 ||
                designDetails.fEta_V_fv_5425_Plate < 0 ||
                designDetails.fEta_V_w_5426 < 0 ||
                designDetails.fEta_N_t_screw_5433 < 0 ||
                designDetails.fEta_V_N_t_screw_5436 < 0 ||
                designDetails.fEta_N_t_5423_plate < 0 ||
                designDetails.fEta_V_yv_3341_plate < 0 ||
                designDetails.fEta_Vb_5424_SecondaryMember < 0 ||
                designDetails.fEta_V_fv_5425_SecondaryMember < 0)
            {
                throw new Exception("Design ratio is invalid!");
            }

            // Validation - infinity design ratio
            if (fEta_max > 9e+10)
            {
                throw new Exception("Design ratio is invalid!");
            }

            // Store details
            if (bSaveDetails)
                joint_temp.DesignDetails = designDetails;
        }

        public void CalculateDesignRatioBaseJoint(CConnectionJointTypes joint_temp, designInternalForces_AS4600 sDIF_temp, bool bSaveDetails = false)
        {
            CConCom_Plate_B_basic basePlate;

            if (plate is CConCom_Plate_B_basic)
                basePlate = (CConCom_Plate_B_basic)plate;
            else
            {
                throw new Exception("Invalid object of base plate.");
            }

            CJointDesignDetails_BaseJoint designDetails = new CJointDesignDetails_BaseJoint();
            // Okopirovane z CalculateDesignRatioApexOrKneeJoint
            // TODO - refaktorovat

            int iNumberOfPlatesInJoint = joint.m_arrPlates.Length;

            float fN = 1f / iNumberOfPlatesInJoint * sDIF_temp.fN;
            float fM_xu = 1f / iNumberOfPlatesInJoint * sDIF_temp.fM_xu_xx;
            float fV_yv = 1f / iNumberOfPlatesInJoint * sDIF_temp.fV_yv_yy;

            // Plate design
            designDetails.fPhi_plate = 0.65f; // TODO - overit ci je to spravne

            // Plate tension design
            designDetails.fA_n_plate = plate.fA_n;
            designDetails.fN_t_plate = eq.Eq_5423_2__(screw.Diameter_thread, plate.S_f_min, designDetails.fA_n_plate, ff_uk_1_plate);
            designDetails.fEta_N_t_5423_plate = eq.Eq_5423_1__(Math.Abs(fN), designDetails.fPhi_plate, designDetails.fN_t_plate);
            fEta_max = MathF.Max(fEta_max, designDetails.fEta_N_t_5423_plate);

            // Plate shear resistance
            designDetails.fA_vn_yv_plate = plate.fA_vn_zv;
            designDetails.fV_y_yv_plate = eq.Eq_723_5___(designDetails.fA_vn_yv_plate, ff_yk_1_plate);
            designDetails.fEta_V_yv_3341_plate = eq.Eq_3341____(fV_yv, designDetails.fPhi_plate, designDetails.fV_y_yv_plate);
            fEta_max = MathF.Max(fEta_max, designDetails.fEta_V_yv_3341_plate);

            // Plate bending resistance
            designDetails.fM_xu_resistance_plate = eq.Eq_7222_4__(joint.m_arrPlates[0].fW_el_yu, ff_yk_1_plate);
            float fDesignReistance_M_plate;
            eq.Eq_723_10__(Math.Abs(fM_xu), designDetails.fPhi_plate, designDetails.fM_xu_resistance_plate, out fDesignReistance_M_plate, out designDetails.fEta_Mb_plate);
            fEta_max = MathF.Max(fEta_max, designDetails.fEta_Mb_plate);

            // Connection -shear force design
            // Shear in connection
            designDetails.fPhi_shear_screw = 0.5f;
            designDetails.fVb_MainMember = eq.Get_Vb_5424(ft_1_plate, ft_2_crscmainMember, screw.Diameter_thread, ff_uk_1_plate, ff_uk_2_MainMember);

            int iNumberOfScrewsInShear = joint_temp.m_arrPlates[0].ScrewArrangement.Screws.Length; // Temporary

            designDetails.fEta_MainMember = Math.Abs(sDIF_temp.fV_yv_yy) / (iNumberOfScrewsInShear * designDetails.fVb_MainMember);

            designDetails.fMb_MainMember_oneside_plastic = 0;

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
                designDetails.fMb_MainMember_oneside_plastic += fHolesCentersRadiiInOneGroup[i] * designDetails.fVb_MainMember;

                fSumri2tormax += MathF.Pow2(fHolesCentersRadiiInOneGroup[i]) / fr_max;
            }

            float fN_oneside = sDIF_temp.fN / 2f;
            float fM_xu_oneside = sDIF_temp.fM_xu_xx / 2f; // Divided by Number of sides
            float fV_yv_oneside = sDIF_temp.fV_yv_yy / 2f;

            // Plastic resistance (Design Ratio)
            designDetails.fEta_Mb_MainMember_oneside_plastic = Math.Abs(fM_xu_oneside) / designDetails.fMb_MainMember_oneside_plastic;
            fEta_max = MathF.Max(fEta_max, designDetails.fEta_Mb_MainMember_oneside_plastic);

            // Elastic resistance
            designDetails.fV_asterix_b_max_screw_Mxu = Math.Abs(fM_xu_oneside) / fSumri2tormax;
            designDetails.fV_asterix_b_max_screw_Vyv = Math.Abs(fV_yv_oneside) / fHolesCentersRadiiInOneGroup.Length;
            designDetails.fV_asterix_b_max_screw_N = Math.Abs(fN_oneside) / fHolesCentersRadiiInOneGroup.Length;

            designDetails.fV_asterix_b_max_screw = 0;

            if (designDetails.fV_asterix_b_max_screw_Mxu != 0 && designDetails.fV_asterix_b_max_screw_Vyv != 0 && designDetails.fV_asterix_b_max_screw_N != 0)
                designDetails.fV_asterix_b_max_screw = MathF.Sqrt(MathF.Sqrt(MathF.Pow2(designDetails.fV_asterix_b_max_screw_Mxu) + MathF.Pow2(designDetails.fV_asterix_b_max_screw_Vyv)) + MathF.Pow2(designDetails.fV_asterix_b_max_screw_N));
            else if ((designDetails.fV_asterix_b_max_screw_Mxu != 0 || designDetails.fV_asterix_b_max_screw_Vyv != 0) && designDetails.fV_asterix_b_max_screw_N == 0)
                designDetails.fV_asterix_b_max_screw = MathF.Sqrt(MathF.Pow2(designDetails.fV_asterix_b_max_screw_Mxu) + MathF.Pow2(designDetails.fV_asterix_b_max_screw_Vyv));
            else if ((designDetails.fV_asterix_b_max_screw_Mxu != 0 || designDetails.fV_asterix_b_max_screw_N != 0) && designDetails.fV_asterix_b_max_screw_Vyv == 0)
                designDetails.fV_asterix_b_max_screw = MathF.Sqrt(MathF.Pow2(designDetails.fV_asterix_b_max_screw_Mxu) + MathF.Pow2(designDetails.fV_asterix_b_max_screw_N));
            else if ((designDetails.fV_asterix_b_max_screw_Vyv != 0 || designDetails.fV_asterix_b_max_screw_N != 0) && designDetails.fV_asterix_b_max_screw_Mxu == 0)
                designDetails.fV_asterix_b_max_screw = MathF.Sqrt(MathF.Pow2(designDetails.fV_asterix_b_max_screw_Vyv) + MathF.Pow2(designDetails.fV_asterix_b_max_screw_N));
            else
                designDetails.fV_asterix_b_max_screw = designDetails.fV_asterix_b_max_screw_Mxu + designDetails.fV_asterix_b_max_screw_Vyv + designDetails.fV_asterix_b_max_screw_N; // Vsetky alebo len jedna zlozka je nenulova, mozeme pouzit sumu

            designDetails.fEta_Vb_5424_MainMember = eq.Eq_5424_1__(designDetails.fV_asterix_b_max_screw, designDetails.fPhi_shear_screw, designDetails.fVb_MainMember);
            fEta_max = MathF.Max(fEta_max, designDetails.fEta_Vb_5424_MainMember);

            // 5.4.2.5 Connection shear as limited by end distance
            designDetails.fe = 0.03f; // TODO - temporary - urcit min vzdialenost skrutky od okraja plechu alebo prierezu
            designDetails.fV_fv_MainMember = eq.Eq_5425_2__(ft_2_crscmainMember, designDetails.fe, ff_uk_2_MainMember);
            designDetails.fV_fv_Plate = eq.Eq_5425_2__(ft_1_plate, designDetails.fe, ff_uk_1_plate);

            // Distance to an end of the connected part is parallel to the line of the applied force
            // Nemalo by rozhodovat pre moment (skrutka namahana rovnobezne s okrajom je uprostred plechu) ale moze rozhovat pre N a V
            designDetails.fV_asterix_fv = 0;

            if (designDetails.fV_asterix_b_max_screw_Vyv != 0 || designDetails.fV_asterix_b_max_screw_N != 0)
                designDetails.fV_asterix_fv = MathF.Sqrt(MathF.Pow2(designDetails.fV_asterix_b_max_screw_Vyv) + MathF.Pow2(designDetails.fV_asterix_b_max_screw_N));

            designDetails.fEta_V_fv_5425_MainMember = eq.Eq_5425_1__(designDetails.fV_asterix_fv, designDetails.fV_fv_MainMember, ff_uk_2_MainMember, ff_yk_2_MainMember);
            designDetails.fEta_V_fv_5425_Plate = eq.Eq_5425_1__(designDetails.fV_asterix_fv, designDetails.fV_fv_Plate, ff_uk_1_plate, ff_yk_1_plate);

            designDetails.fEta_V_fv_5425 = Math.Max(designDetails.fEta_V_fv_5425_MainMember, designDetails.fEta_V_fv_5425_Plate);
            fEta_max = MathF.Max(fEta_max, designDetails.fEta_V_fv_5425);

            // 5.4.2.6 Screws in shear
            // The design shear capacity φVw of the screw shall be determined by testing in accordance with Section 8.

            designDetails.fV_w_nom_screw_5426 = screw.ShearStrength_nominal; // N
            designDetails.fEta_V_w_5426 = Math.Max(designDetails.fV_asterix_b_max_screw, designDetails.fV_asterix_fv) / (0.5f * designDetails.fV_w_nom_screw_5426);
            fEta_max = MathF.Max(fEta_max, designDetails.fEta_V_w_5426);

            int iNumberOfDecimalPlaces = 3;
            if (bIsDebugging)
                MessageBox.Show("Calculation finished.\n"
                              + "Design Ratio η = " + Math.Round(designDetails.fEta_Vb_5424_MainMember, iNumberOfDecimalPlaces) + " [-]" + "\t" + " 5.4.2.4" + "\n"
                              + "Design Ratio η = " + Math.Round(designDetails.fEta_V_fv_5425, iNumberOfDecimalPlaces) + " [-]" + "\t" + " 5.4.2.5" + "\n"
                              + "Design Ratio η max = " + Math.Round(fEta_max, iNumberOfDecimalPlaces) + " [-]");

            // Tension in members
            designDetails.fPhi_CrSc = 0.65f; // TODO - overit ci je to spravne
            // 5.4.2.3 Tension in the connected part
            designDetails.fA_n_MainMember = (float)crsc_mainMember.A_g - plate.INumberOfConnectorsInSection * 2 * 2 * screw.Diameter_thread; // TODO - spocitat presne podla poctu a rozmeru otvorov v jednom reze
            designDetails.fN_t_section_MainMember = eq.Eq_5423_2__(screw.Diameter_thread, plate.S_f_min, designDetails.fA_n_MainMember, ff_uk_2_MainMember);
            designDetails.fEta_N_t_5423_MainMember = eq.Eq_5423_1__(sDIF_temp.fN_t, designDetails.fPhi_CrSc, designDetails.fN_t_section_MainMember);
            fEta_max = MathF.Max(fEta_max, designDetails.fEta_N_t_5423_MainMember);


            // TODO - tu este chybaju posudenia samotnej dosky na napati betonu pod nou na lokalny tlak betonu pod plechom, ohyb, oslabeny prierez v ohybe , tahu, tlaku, smyku atd vid xls



            /*
            // Anchors
            designDetails.fN_asterix_joint_uplif = Math.Max(sDIF_temp.fN, 0); // Tension in column - positive
            designDetails.fN_asterix_joint_bearing = Math.Min(sDIF_temp.fN, 0); // Compression in column - negative

            designDetails.fV_asterix_x_joint = Math.Abs(sDIF_temp.fV_xu_xx);
            designDetails.fV_asterix_y_joint = Math.Abs(sDIF_temp.fV_yv_yy);
            designDetails.fV_asterix_res_joint = 0f;

            if(!MathF.d_equal(designDetails.fV_asterix_x_joint,0) || !MathF.d_equal(designDetails.fV_asterix_y_joint, 0))
                designDetails.fV_asterix_res_joint = MathF.Sqrt(MathF.Pow2(designDetails.fV_asterix_x_joint) + MathF.Pow2(designDetails.fV_asterix_y_joint));

            //int iNumberAnchors = plate.AnchorArrangement.Anchors.Length;
            designDetails.iNumberAnchors = basePlate.AnchorArrangement.IHolesNumber;
            designDetails.iNumberAnchors_t = designDetails.iNumberAnchors; // Total number of anchors active in tension - all anchors active as default
            designDetails.iNumberAnchors_v = designDetails.iNumberAnchors; // Total number of anchors active in shear - all anchors active as default

            CAnchorArrangement_BB_BG anchorArrangement;

            if (basePlate.AnchorArrangement is CAnchorArrangement_BB_BG)
                anchorArrangement = (CAnchorArrangement_BB_BG)basePlate.AnchorArrangement;
            else
            {
                throw new Exception("Not implemented arrangmement of anchors.");
            }

            int iNumberAnchors_x = anchorArrangement.NumberOfAnchorsInYDirection;
            int iNumberAnchors_y = anchorArrangement.NumberOfAnchorsInZDirection;

            designDetails.fN_asterix_anchor_uplif = designDetails.fN_asterix_joint_uplif / designDetails.iNumberAnchors_t; // Design axial force per anchor
            designDetails.fV_asterix_anchor = designDetails.fV_asterix_res_joint / designDetails.iNumberAnchors_v; // Design shear force per anchor

            designDetails.fplateWidth_x = (float)joint.m_MainMember.CrScStart.b; // TODO - zapracovat priamo nacitanie parametrov plate type BA - BG
            designDetails.fplateWidth_y = (float)joint.m_MainMember.CrScStart.h; // TODO - zapracovat priamo nacitanie parametrov plate type BA - BG

            designDetails.fFootingDimension_x = 1.1f; // TODO - napojit na velkost zakladu
            designDetails.fFootingDimension_y = 1.1f; // TODO - napojit na velkost zakladu
            designDetails.fFootingHeight = 0.4f; // TODO - napojit na velkost zakladu

            designDetails.fe_x_AnchorToPlateEdge = 0.05f; // TODO - Distance between anchor and plate edge
            designDetails.fe_y_AnchorToPlateEdge = 0.05f; // TODO - Distance between anchor and plate edge

            designDetails.fe_x_BasePlateToFootingEdge = 0.5f * (designDetails.fFootingDimension_x - designDetails.fplateWidth_x); // TODO - Distance between base plate and footing edge - zapracovat excentricitu
            designDetails.fe_y_BasePlateToFootingEdge = 0.5f * (designDetails.fFootingDimension_y - designDetails.fplateWidth_y); // TODO - Distance between base plate and footing edge - zapracovat excentricitu

            designDetails.fu_x_Washer = 0.08f;
            designDetails.fu_y_Washer = 0.08f;

            designDetails.ff_apostrophe_c = 25e+6f; // Characteristic compressive (cylinder) concrete strength
            designDetails.fRho_c = 2300f; // Density of concrete - TODO - nacitat z materialu zakladov

            // Anchors (bolts)
            designDetails.fd_s = basePlate.AnchorArrangement.referenceAnchor.Diameter_thread;
            designDetails.fd_f = basePlate.AnchorArrangement.referenceAnchor.Diameter_shank;

            designDetails.fA_c = basePlate.AnchorArrangement.referenceAnchor.Area_c_thread; // Core / thread area
            designDetails.fA_o = basePlate.AnchorArrangement.referenceAnchor.Area_o_shank; // Shank area

            designDetails.ff_y_anchor = basePlate.AnchorArrangement.referenceAnchor.m_Mat.m_ff_yk[0];
            designDetails.ff_u_anchor = basePlate.AnchorArrangement.referenceAnchor.m_Mat.m_ff_u[0];

            // AS / NZS 4600:2018 - 5.3 Bolted connections
            // Base plate design
            // 5.3.2 Tearout
            designDetails.fPhi_v_532 = 0.7f;
            designDetails.fV_f_532 = eq.Eq_532_2___(ft_1_plate, designDetails.fe_y_AnchorToPlateEdge, ff_uk_1_plate);
            designDetails.fEta_532_1 = eq.Eq_5351_1__(designDetails.fV_asterix_anchor, designDetails.fPhi_v_532, designDetails.fV_f_532);
            fEta_max = MathF.Max(fEta_max, designDetails.fEta_532_1);

            // 5.3.4.2 Bearing capacity without considering bolt hole deformation
            designDetails.fPhi_v_534 = 0.6f;
            designDetails.fAlpha_5342 = eq.Get_Alpha_Table_5342_A(ETypesOfBearingConnection.eType3);
            designDetails.fC_5342 = eq.Get_Factor_C_Table_5342_B(designDetails.fd_f, ft_1_plate);
            designDetails.fV_b_5342 = eq.Eq_5342____(designDetails.fAlpha_5342, designDetails.fC_5342, designDetails.fd_f, ft_1_plate, ff_uk_1_plate);
            designDetails.fEta_5342 = Math.Abs(designDetails.fV_asterix_anchor) / (designDetails.fPhi_v_534 * designDetails.fV_b_5342);
            fEta_max = MathF.Max(fEta_max, designDetails.fEta_5342);

            // 5.3.4.3 Bearing capacity at a bolt hole deformation of 6 mm
            designDetails.fV_b_5343 = eq.Eq_5343____(designDetails.fd_f, ft_1_plate, ff_uk_1_plate);
            designDetails.fEta_5343 = Math.Abs(designDetails.fV_asterix_anchor) / (designDetails.fPhi_v_534 * designDetails.fV_b_5343);
            fEta_max = MathF.Max(fEta_max, designDetails.fEta_5343);

            // Bolt design / Anchor design
            // 5.3.5.1 Bolt in shear
            designDetails.fPhi_535 = 0.8f;
            int iNumberOfShearPlanesOfBolt_core = 1; // Jednostrizny spoj - strih jardom skrutky
            designDetails.fV_fv_5351_2_anchor = eq.Eq_5351_2__(designDetails.ff_u_anchor, iNumberOfShearPlanesOfBolt_core, designDetails.fA_c, 0, designDetails.fA_o); // Uvazovane konzervativne jedna smykova plocha a zavit je aj v smykovej ploche
            designDetails.fEta_5351_2 = eq.Eq_5351_1__(designDetails.fV_asterix_anchor, designDetails.fPhi_535, designDetails.fV_fv_5351_2_anchor);
            fEta_max = MathF.Max(fEta_max, designDetails.fEta_5351_2);

            // 5.3.5.2 Bolt in tension
            designDetails.fN_ft_5352_1 = eq.Eq_5352_2__(designDetails.fA_c, designDetails.ff_u_anchor);
            designDetails.fEta_5352_1 = eq.Eq_5352_1__(designDetails.fN_asterix_anchor_uplif, designDetails.fPhi_535, designDetails.fN_ft_5352_1);
            fEta_max = MathF.Max(fEta_max, designDetails.fEta_5352_1);

            // 5.3.5.3 Bolt subject to combined shear and tension
            float fPortion_V_5353;
            float fPortion_N_5353;
            designDetails.fEta_5353 = eq.Eq_5353____(designDetails.fV_asterix_anchor, designDetails.fPhi_535, designDetails.fV_fv_5351_2_anchor, designDetails.fN_asterix_anchor_uplif, 0.8f, designDetails.fN_ft_5352_1, out fPortion_V_5353, out fPortion_N_5353);
            fEta_max = MathF.Max(fEta_max, designDetails.fEta_5353);

            // NZS 3101.1 - 2006
            designDetails.fElasticityFactor_1764 = 0.75f; // EQ load combination - 0.75, other 1.00

            // 17.5.6 Strength of cast -in anchors

            // 17.5.6.4 Strength reduction factors
            designDetails.fPhi_anchor_tension_173 = 0.75f;
            designDetails.fPhi_anchor_shear_174   = 0.65f;

            designDetails.fPhi_concrete_tension_174a = 0.65f;
            designDetails.fPhi_concrete_shear_174b   = 0.65f;

            // 17.5.7.1  Steel strength of anchor in tension
            // Group of anchors
            designDetails.fA_se = designDetails.fA_c; // Effective cross-sectional area of an anchor
            designDetails.fN_s_176_group = eq_concrete.Eq_17_6____(designDetails.iNumberAnchors_t, designDetails.fA_se, designDetails.ff_u_anchor);
            designDetails.fEta_17571_group = eq_concrete.Eq_17_1____(designDetails.fN_asterix_joint_uplif, designDetails.fPhi_anchor_tension_173, designDetails.fN_s_176_group);
            fEta_max = MathF.Max(fEta_max, designDetails.fEta_17571_group);

            // 17.5.7.2  Strength of concrete breakout of anchor
            // Group of anchors

            // Figure C17.4 – Definition of dimension e´n for group anchors
            designDetails.fe_apostrophe_n = 0f;                           // the distance between the resultant tension load on a group of anchors in tension and the centroid of the group of anchors loaded in tension(always taken as positive)
            designDetails.fConcreteCover = 0.07f;
            designDetails.fh_ef = designDetails.fFootingHeight - designDetails.fConcreteCover;        // effective anchor embedment depth
            designDetails.fs_2_x = 0f;                                    // centre-to-centre spacing of the anchors
            designDetails.fs_1_y = 0.23f;                                 // centre-to-centre spacing of the anchors
            designDetails.fs_min = Math.Min(designDetails.fs_2_x, designDetails.fs_1_y);
            designDetails.fc_2_x = 0.55f;
            designDetails.fc_1_y = 0.55f;
            designDetails.fc_min = Math.Min(designDetails.fc_2_x, designDetails.fc_1_y);
            designDetails.fk = 10f; // for cast-in anchors
            designDetails.fLambda_53 = eq_concrete.Eq_5_3_____(designDetails.fRho_c);

            designDetails.fe_x_AnchorToPlateEdge = 0.5f * (designDetails.fplateWidth_x - (iNumberAnchors_x - 1) * designDetails.fs_2_x);
            designDetails.fe_y_AnchorToPlateEdge = 0.5f * (designDetails.fplateWidth_y - (iNumberAnchors_y - 1) * designDetails.fs_1_y);

            designDetails.fPsi_1_group = eq_concrete.Eq_17_8____(designDetails.fe_apostrophe_n, designDetails.fh_ef);
            designDetails.fPsi_2 = eq_concrete.Get_Psi_2__(designDetails.fc_min, designDetails.fh_ef);

            // Ψ3 = 1.25 for cast -in anchors in uncracked concrete
            // Ψ3 = 1.0 for concrete which is cracked at service load levels.
            designDetails.fPsi_3 = 1.25f; // modification factor or cracking of concrete
            designDetails.fA_no_group = (2f * 1.5f * designDetails.fh_ef) * (2f * 1.5f * designDetails.fh_ef);

            float fAn_Length_x_group = Math.Min(designDetails.fc_2_x, 1.5f * designDetails.fh_ef) + 1.5f * designDetails.fh_ef + ((iNumberAnchors_x - 1) * designDetails.fs_2_x);
            float fAn_Length_y_group = Math.Min(designDetails.fc_1_y, 1.5f * designDetails.fh_ef) + 1.5f * designDetails.fh_ef + ((iNumberAnchors_y - 1) * designDetails.fs_1_y);
            designDetails.fA_n_group = Math.Min(fAn_Length_x_group * fAn_Length_y_group, designDetails.iNumberAnchors_t * designDetails.fA_no_group);

            designDetails.fN_b_179_group = eq_concrete.Eq_17_9____(designDetails.fk, designDetails.fLambda_53, Math.Min(designDetails.ff_apostrophe_c, 70e+6f), designDetails.fh_ef);
            designDetails.fN_b_179a_group = eq_concrete.Eq_17_9a___(designDetails.fLambda_53, designDetails.ff_apostrophe_c, designDetails.fh_ef);

            if(0.280f <= designDetails.fh_ef && designDetails.fh_ef <= 0.635f && designDetails.fN_b_179_group > designDetails.fN_b_179a_group)
            {
                designDetails.fN_b_179_group = designDetails.fN_b_179a_group;
            }

            designDetails.fN_cb_177_group = eq_concrete.Eq_17_7____(designDetails.fPsi_1_group, designDetails.fPsi_2, designDetails.fPsi_3, designDetails.fA_n_group, designDetails.fA_no_group, designDetails.fN_b_179_group);

            designDetails.fEta_17572_group = eq_concrete.Eq_17_1____(designDetails.fN_asterix_joint_uplif, designDetails.fPhi_concrete_tension_174a, designDetails.fN_cb_177_group);
            fEta_max = MathF.Max(fEta_max, designDetails.fEta_17572_group);

            // Single anchor - edge
            designDetails.fPsi_1_single = 1.0f;
            designDetails.fA_no_single = (2f * 1.5f * designDetails.fh_ef) * (2f * 1.5f * designDetails.fh_ef);
            float fAn_Length_x_single = Math.Min(designDetails.fc_2_x, 1.5f * designDetails.fh_ef) + 1.5f * designDetails.fh_ef;
            float fAn_Length_y_single = Math.Min(designDetails.fc_1_y, 1.5f * designDetails.fh_ef) + 1.5f * designDetails.fh_ef;
            designDetails.fA_n_single = Math.Min(fAn_Length_x_single * fAn_Length_y_single, designDetails.fA_no_single);

            designDetails.fN_b_179_single = eq_concrete.Eq_17_9____(designDetails.fk, designDetails.fLambda_53, Math.Min(designDetails.ff_apostrophe_c, 70e+6f), designDetails.fh_ef);
            designDetails.fN_b_179a_single = eq_concrete.Eq_17_9a___(designDetails.fLambda_53, designDetails.ff_apostrophe_c, designDetails.fh_ef);

            if (0.280f <= designDetails.fh_ef && designDetails.fh_ef <= 0.635f && designDetails.fN_b_179_single > designDetails.fN_b_179a_single)
            {
                designDetails.fN_b_179_single = designDetails.fN_b_179a_single;
            }

            designDetails.fN_cb_177_single = eq_concrete.Eq_17_7____(designDetails.fPsi_1_single, designDetails.fPsi_2, designDetails.fPsi_3, designDetails.fA_n_single, designDetails.fA_no_single, designDetails.fN_b_179_single);

            designDetails.fEta_17572_single = eq_concrete.Eq_17_1____(designDetails.fN_asterix_anchor_uplif, designDetails.fPhi_concrete_tension_174a, designDetails.fN_cb_177_single);
            fEta_max = MathF.Max(fEta_max, designDetails.fEta_17572_single);

            // 17.5.7.3  Lower characteristic tension pullout strength of anchor
            // Group of anchors

            designDetails.fm_x = 0.06f; // Input
            designDetails.fm_y = 0.06f; // Input
            designDetails.fA_brg = designDetails.fm_x * designDetails.fm_y; // bearing area of the head of stud or anchor
            designDetails.fN_p_1711_single = eq_concrete.Eq_17_11___(designDetails.ff_apostrophe_c, designDetails.fA_brg);

            // Modification factor for pullout strength
            // Ψ4 = 1.0 for concrete cracked at service load levels but with the extent of cracking controlled by reinforcement distributed in accordance with 2.4.4.4 and 2.4.4.5
            // Ψ4 = 1.4 for concrete with no cracking at service load levels
            designDetails.fPsi_4 = 1.0f;
            designDetails.fN_pn_1710_single = eq_concrete.Eq_17_10___(designDetails.fPsi_4, designDetails.fN_p_1711_single);
            designDetails.fN_pn_1710_group = designDetails.iNumberAnchors_t * designDetails.fN_pn_1710_single;

            designDetails.fEta_17573_group = eq_concrete.Eq_17_1____(designDetails.fN_asterix_joint_uplif, designDetails.fPhi_anchor_tension_173, designDetails.fN_pn_1710_group);
            fEta_max = MathF.Max(fEta_max, designDetails.fEta_17573_group);

            // The side face blowout strength of a headed anchor with deep embedment close to an edge
            designDetails.fN_sb_1713_single = float.PositiveInfinity;
            designDetails.fEta_17574_single = 0;

            if (designDetails.fc_min < 0.4f * designDetails.fh_ef)
            {
                // 17.5.7.4 Lower characteristic concrete side face blowout strength
                // Single anchor - edge
                designDetails.fc_1_17574 = designDetails.fc_1_y;

                if (designDetails.fN_asterix_anchor_uplif > 0) // Tension in anchor
                    designDetails.fc_1_17574 = designDetails.fc_min;

                // Anchors subject to shear are located in narrow sections of limited thickness
                float fc_1_limit = MathF.Max(designDetails.fc_2_x / 1.5f, designDetails.fh_ef / 1.5f, designDetails.fs_min / 3f);

                if (designDetails.fc_1_17574 > fc_1_limit)
                    designDetails.fc_1_17574 = fc_1_limit;

                designDetails.fk_1 = eq_concrete.Get_k_1____(designDetails.fc_1_17574, designDetails.fc_2_x);

                designDetails.fN_sb_1713_single = eq_concrete.Eq_17_13___(designDetails.fk_1, designDetails.fc_1_17574, designDetails.fLambda_53, designDetails.fA_brg, designDetails.ff_apostrophe_c);

                designDetails.fEta_17574_single = eq_concrete.Eq_17_1____(designDetails.fN_asterix_anchor_uplif, designDetails.fPhi_concrete_tension_174a, designDetails.fN_sb_1713_single);
                fEta_max = MathF.Max(fEta_max, designDetails.fEta_17574_single);
            }

            // Lower characteristic strength in tension
            designDetails.fN_n_nominal_min = MathF.Min(
                designDetails.fN_s_176_group,                                       // 17.5.7.1
                designDetails.fN_cb_177_group,                                      // 17.5.7.2
                designDetails.iNumberAnchors_t * designDetails.fN_cb_177_single,    // 17.5.7.2
                designDetails.fN_pn_1710_group,                                     // 17.5.7.3
                designDetails.iNumberAnchors_t * designDetails.fN_sb_1713_single);  // 17.5.7.4

            // Lower design strength in tension
            designDetails.fN_d_design_min = designDetails.fElasticityFactor_1764 * MathF.Min(
                designDetails.fPhi_anchor_tension_173 * designDetails.fN_s_176_group,                                         // 17.5.7.1
                designDetails.fPhi_concrete_tension_174a * designDetails.fN_cb_177_group,                                     // 17.5.7.2
                designDetails.fPhi_concrete_tension_174a * designDetails.iNumberAnchors_t * designDetails.fN_cb_177_single,   // 17.5.7.2
                designDetails.fPhi_anchor_tension_173 * designDetails.fN_pn_1710_group,                                       // 17.5.7.3
                designDetails.fPhi_concrete_tension_174a * designDetails.iNumberAnchors_t * designDetails.fN_sb_1713_single); // 17.5.7.4

            // 17.5.8 Lower characteristic strength of anchor in shear

            // 17.5.8.1 Lower characteristic shear strength of steel of anchor
            // Group of anchors

            // TODO - rozlisovat typ kotvy - rovnica 17-14 alebo 17-15
            designDetails.fV_s_1714_group = eq_concrete.Eq_17_14___(designDetails.iNumberAnchors_v, designDetails.fA_se, designDetails.ff_u_anchor, designDetails.ff_y_anchor);
            designDetails.fV_s_1715_group = eq_concrete.Eq_17_15___(designDetails.iNumberAnchors_v, designDetails.fA_se, designDetails.ff_u_anchor, designDetails.ff_y_anchor);

            designDetails.fV_s_17581_group = Math.Min(designDetails.fV_s_1714_group, designDetails.fV_s_1715_group);
            designDetails.fEta_17581_group = eq_concrete.Eq_17_2____(designDetails.fV_asterix_res_joint, designDetails.fPhi_anchor_shear_174, designDetails.fV_s_17581_group);
            fEta_max = MathF.Max(fEta_max, designDetails.fEta_17573_group);

            // 17.5.8.2 Lower characteristic concrete breakout strength of the anchor in shear perpendicular to edge
            // Group of anchors

            designDetails.fe_apostrophe_v = 0;
            designDetails.fPsi_5_group = eq_concrete.Eq_17_18___(designDetails.fc_1_y, designDetails.fe_apostrophe_v, designDetails.fs_2_x); // s - perpendicular to shear force - Figure C17.7 – Definition of dimensions e´
            designDetails.fPsi_6 = eq_concrete.Get_Psi_6__(designDetails.fc_1_y, designDetails.fc_2_x);

            // Ψ7 = modification factor for cracked concrete, given by:
            // Ψ7 = 1.0 for anchors in cracked concrete with no supplementary reinforcement or with smaller than 12 mm diameter reinforcing bar as supplementary reinforcement
            // Ψ7 = 1.2 for anchors in cracked concrete with a 12 mm diameter reinforcing bar or greater as supplementary reinforcement
            // Ψ7 = 1.4 for concrete that is not cracked at service load levels.
            designDetails.fPsi_7 = 1.0f;

            designDetails.fA_vo = 2 * (1.5f * designDetails.fc_1_y) * (1.5f * designDetails.fc_1_y); // projected concrete failure area of an anchor in shear, when not limited by corner influences, spacing, or member thickness
            float fAv_Length_x_group = 1.5f * designDetails.fc_1_y + Math.Min(1.5f * designDetails.fc_1_y, designDetails.fc_2_x) + (iNumberAnchors_x - 1) * designDetails.fs_2_x;
            float fAv_Depth_h = Math.Min(1.5f * designDetails.fc_1_y, designDetails.fFootingHeight);
            designDetails.fA_v_group = fAv_Length_x_group * fAv_Depth_h; // projected concrete failure area of an anchor or group of anchors in shear
            designDetails.fd_o = designDetails.fd_f;

            designDetails.fk_2 = 0.6f;
            designDetails.fl = Math.Min(designDetails.fh_ef, 8f * designDetails.fd_o); // load-bearing length of anchors for shear, equal to hef for anchors with constant stiffness over the full length of the embedded section but less than 8do.Shall be taken as 0.8 times the effective embedment depth for hooked metal plates.

            if (designDetails.iNumberAnchors != 1 && designDetails.fs_min != 0 && designDetails.fs_min < 0.065f)
                throw new Exception("Distance between anchors s is smaller than 65 mm.");

            designDetails.fV_b_1717a = eq_concrete.Eq_17_17a__(designDetails.fk_2, designDetails.fl, designDetails.fd_o, designDetails.fLambda_53, designDetails.ff_apostrophe_c, designDetails.fc_1_y);
            designDetails.fV_b_1717b = eq_concrete.Eq_17_17b__(designDetails.fLambda_53, designDetails.ff_apostrophe_c, designDetails.fc_1_y);
            designDetails.fV_b_1717 = Math.Min(designDetails.fV_b_1717a, designDetails.fV_b_1717b);
            designDetails.fV_cb_1716_group = eq_concrete.Eq_17_16___(designDetails.fA_v_group, designDetails.fA_vo, designDetails.fPsi_5_group, designDetails.fPsi_6, designDetails.fPsi_7, designDetails.fV_b_1717);

            designDetails.fEta_17582_group = eq_concrete.Eq_17_2____(designDetails.fV_asterix_res_joint, designDetails.fPhi_concrete_shear_174b, designDetails.fV_cb_1716_group);
            fEta_max = MathF.Max(fEta_max, designDetails.fEta_17582_group);

            // Single of anchor - edge

            designDetails.fPsi_5_single = 1.0f;
            float fAv_Length_x_signle = (1.5f * designDetails.fc_1_y + Math.Min(1.5f * designDetails.fc_1_y, 0.5f * designDetails.fFootingDimension_x));
            designDetails.fA_v_single = fAv_Length_x_signle * fAv_Depth_h;
            designDetails.fV_cb_1716_single = eq_concrete.Eq_17_16___(designDetails.fA_v_single, designDetails.fA_vo, designDetails.fPsi_5_single, designDetails.fPsi_6, designDetails.fPsi_7, designDetails.fV_b_1717);

            designDetails.fEta_17582_single = eq_concrete.Eq_17_2____(designDetails.fV_asterix_anchor, designDetails.fPhi_concrete_shear_174b, designDetails.fV_cb_1716_single);
            fEta_max = MathF.Max(fEta_max, designDetails.fEta_17582_single);

            // 17.5.8.3 Lower characteristic concrete breakout strength of the anchor in shear parallel to edge
            // Group of anchors
            // TODO - zohladnit len paralelnu smykovu silu Vx ???

            designDetails.fV_cb_1721_group = eq_concrete.Eq_17_21___(designDetails.fA_v_group, designDetails.fA_vo, designDetails.fPsi_5_group, designDetails.fPsi_7, designDetails.fV_b_1717);

            designDetails.fEta_17583_group = eq_concrete.Eq_17_2____(designDetails.fV_asterix_res_joint, designDetails.fPhi_concrete_shear_174b, designDetails.fV_cb_1721_group);
            fEta_max = MathF.Max(fEta_max, designDetails.fEta_17583_group);

            // Single anchor - edge
            designDetails.fV_cb_1721_single = eq_concrete.Eq_17_21___(designDetails.fA_v_single, designDetails.fA_vo, designDetails.fPsi_5_single, designDetails.fPsi_7, designDetails.fV_b_1717);

            designDetails.fEta_17583_single = eq_concrete.Eq_17_2____(designDetails.fV_asterix_res_joint, designDetails.fPhi_concrete_shear_174b, designDetails.fV_cb_1721_single);
            fEta_max = MathF.Max(fEta_max, designDetails.fEta_17583_single);

            // 17.5.8.4 Lower characteristic concrete pry-out of the anchor in shear
            // Group of anchors

            designDetails.fN_cb_17584_group = Math.Min(designDetails.fN_pn_1710_group, designDetails.fN_cb_177_group); // ??? Zohladnit aj N_pn ???
            designDetails.fk_cp_17584 = eq_concrete.Get_k_cp___(designDetails.fh_ef);
            designDetails.fV_cp_1722_group = eq_concrete.Eq_17_22___(designDetails.fk_cp_17584, designDetails.fN_cb_17584_group);

            designDetails.fEta_17584_group = eq_concrete.Eq_17_2____(designDetails.fV_asterix_anchor, designDetails.fPhi_concrete_shear_174b, designDetails.fV_cp_1722_group);
            fEta_max = MathF.Max(fEta_max, designDetails.fEta_17584_group);

            // Lower characteristic strength in shear
            designDetails.fV_n_nominal_min = MathF.Min(
                designDetails.fV_s_17581_group,                                     // 17.5.8.1
                designDetails.fV_cb_1716_group,                                     // 17.5.8.2
                designDetails.iNumberAnchors_v * designDetails.fV_cb_1716_single,   // 17.5.8.2
                designDetails.fV_cb_1721_group,                                     // 17.5.8.3
                designDetails.iNumberAnchors_v * designDetails.fV_cb_1721_single,   // 17.5.8.3
                designDetails.fV_cp_1722_group);                                    // 17.5.8.4

            // Lower design strength in shear
            designDetails.fV_d_design_min = designDetails.fElasticityFactor_1764 * MathF.Min(
                designDetails.fPhi_anchor_shear_174 * designDetails.fV_s_17581_group,                                          // 17.5.8.1
                designDetails.fPhi_concrete_shear_174b * designDetails.fV_cb_1716_group,                                       // 17.5.8.2
                designDetails.fPhi_concrete_shear_174b * designDetails.iNumberAnchors_v * designDetails.fV_cb_1716_single,     // 17.5.8.2
                designDetails.fPhi_concrete_shear_174b * designDetails.fV_cb_1721_group,                                       // 17.5.8.3
                designDetails.fPhi_concrete_shear_174b * designDetails.iNumberAnchors_v * designDetails.fV_cb_1721_single,     // 17.5.8.3
                designDetails.fPhi_concrete_shear_174b * designDetails.fV_cp_1722_group);                                      // 17.5.8.4

            // 17.5.6.6 Interaction of tension and shear – simplified procedures
            // Group of anchors

            // 17.5.6.6(Eq. 17–5)
            designDetails.fEta_17566_group = eq_concrete.Eq_17566___(designDetails.fN_asterix_joint_uplif, designDetails.fN_d_design_min, designDetails.fV_asterix_res_joint, designDetails.fV_d_design_min);
            fEta_max = MathF.Max(fEta_max, designDetails.fEta_17566_group);

            // C17.5.6.6 (Figure C17.1)
            bool bUseC17566Equation = true;
            float fEta_C17566_group = 0;

            if (bUseC17566Equation)
            {
                fEta_C17566_group = eq_concrete.Eq_C17566__(designDetails.fN_asterix_joint_uplif, designDetails.fN_d_design_min, designDetails.fV_asterix_res_joint, designDetails.fV_d_design_min);
                fEta_max = MathF.Max(fEta_max, fEta_C17566_group);
            }

            // Footings
            designDetails.fGamma_F_uplift = 0.9f; // Load Factor -uplift
            designDetails.fGamma_F_bearing = 1.2f; // Load Factor - bearing

            //float fN_asterix_joint_uplif =
            //float fN_asterix_joint_bearing 

            designDetails.fc_nominal_soil_bearing_capacity = 58000f; // Pa - soil bearing capacity - TODO - user defined

            // Footing pad
            designDetails.fA_footing = designDetails.fFootingDimension_x * designDetails.fFootingDimension_y; // Area of footing pad
            designDetails.fV_footing = designDetails.fA_footing * designDetails.fFootingHeight;  // Volume of footing pad
            float fRho_c_footing = 2300f; // Density of dry concrete - foundations
            designDetails.fG_footing = designDetails.fV_footing * fRho_c_footing; // Self-weight [N] - footing pad

            // Tributary floor volume
            float ft_floor = 0.125f; // TODO - user-defined
            float fa_tributary_floor = 0.6f; // TODO - tributary dimension;
            float fLength_x_tributary_floor = designDetails.fFootingDimension_x + 2 * fa_tributary_floor; // TODO - zistit ci je stlp v rohu, ak ano uvazovat len jednu stranu
            float fLength_y_tributary_floor = designDetails.fFootingDimension_y + fa_tributary_floor; // Okraj - uvazujem sa len jedna strana patky
            float fA_tributary_floor = fLength_x_tributary_floor * fLength_y_tributary_floor - designDetails.fA_footing;
            float fV_tributary_floor = fA_tributary_floor * ft_floor;
            float fRho_c_floor = 2200f; // Density of dry concrete - concrete floor
            designDetails.fG_tributary_floor = fV_tributary_floor * fRho_c_floor; // Self-weight [N] - tributary concrete floor

            // Additional material above the footing
            float ft_additional_material = 0.3f; // User-defined
            float fRho_additional_material = 2200f; // Can be concrete or soil
            float fVolume_additional_material = designDetails.fA_footing * ft_additional_material;
            designDetails.fG_additional_material = fVolume_additional_material * fRho_additional_material; // Self-weight [N]

            // Uplift
            float fG_design_footing_uplift = designDetails.fGamma_F_uplift * designDetails.fG_footing;
            float fG_design_tributary_floor_uplift = designDetails.fGamma_F_uplift * fG_design_footing_uplift;
            float fG_design_additional_material_uplift = designDetails.fGamma_F_uplift * designDetails.fG_additional_material;
            designDetails.fG_design_uplift = fG_design_footing_uplift + fG_design_tributary_floor_uplift + fG_design_additional_material_uplift;

            // Bearing
            float fG_design_footing_bearing = designDetails.fGamma_F_bearing * designDetails.fG_footing;
            float fG_design_additional_material_bearing = designDetails.fGamma_F_bearing * designDetails.fG_additional_material;
            designDetails.fG_design_bearing = fG_design_footing_bearing + fG_design_additional_material_bearing;

            // Design ratio - uplift and bearing force
            designDetails.fEta_footing_uplift = Math.Abs(designDetails.fN_asterix_joint_uplif) / designDetails.fG_design_uplift;
            fEta_max = MathF.Max(fEta_max, designDetails.fEta_footing_uplift);

            designDetails.fN_design_bearing_total = Math.Abs(designDetails.fN_asterix_joint_bearing) + designDetails.fG_design_bearing;
            designDetails.fPressure_bearing = Math.Abs(designDetails.fN_design_bearing_total) / designDetails.fA_footing;
            designDetails.fSafetyFactor = 0.5f; // TODO - zistit aky je faktor
            designDetails.fEta_footing_bearing = designDetails.fPressure_bearing / (designDetails.fSafetyFactor * designDetails.fc_nominal_soil_bearing_capacity);
            fEta_max = MathF.Max(fEta_max, designDetails.fEta_footing_bearing);

            // Bending - design of reinforcement
            // Reinforcement bars in x direction (parallel to the wall)
            designDetails.fq_linear_xDirection = Math.Abs(designDetails.fN_design_bearing_total) / designDetails.fFootingDimension_x;
            designDetails.fM_asterix_footingdesign_xDirection = designDetails.fq_linear_xDirection * MathF.Pow2(designDetails.fFootingDimension_x) / 8f; // ??? jednoducho podpoprety nosnik ???

            designDetails.fd_reinforcement_xDirection = 0.016f; // TODO - user defined
            float fd_reinforcement_yDirection = 0.016f; // TODO - user defined (default above the reinforcement in x-direction)
            designDetails.fA_s1_Xdirection = MathF.fPI * MathF.Pow2(designDetails.fd_reinforcement_xDirection) / 4f; // Reinforcement bar cross-sectional area
            float fA_s1_Ydirection = MathF.fPI * MathF.Pow2(fd_reinforcement_yDirection) / 4f; // Reinforcement bar cross-sectional area
            designDetails.iNumberOfBarsInXDirection = 11; // TODO - user defined
            int iNumberOfBarsInYDirection = 11; // TODO - user defined

            designDetails.fA_s_tot_Xdirection = designDetails.iNumberOfBarsInXDirection * designDetails.fA_s1_Xdirection;
            float fA_s_tot_Ydirection = iNumberOfBarsInYDirection * fA_s1_Ydirection;

            float fConcreteCover_reinforcement_side = 0.075f;
            float fSpacing_xDirection = (designDetails.fFootingDimension_x - 2 * fConcreteCover_reinforcement_side - fd_reinforcement_yDirection) / (iNumberOfBarsInYDirection - 1);
            designDetails.fSpacing_yDirection = (designDetails.fFootingDimension_y - 2 * fConcreteCover_reinforcement_side - designDetails.fd_reinforcement_xDirection) / (designDetails.iNumberOfBarsInXDirection - 1);

            string sReinforcingSteelGrade_Name = "500E";
            float fReinforcementStrength_fy = 500e+6f; // TODO - user defined

            designDetails.fAlpha_c = 0.85f;
            designDetails.fPhi_b_foundations = 0.85f;

            designDetails.fConcreteCover_reinforcement_xDirection = 0.075f;

            designDetails.fd_effective_xDirection = designDetails.fFootingHeight - designDetails.fConcreteCover_reinforcement_xDirection - 0.5f * designDetails.fd_reinforcement_xDirection;
            float fd_effective_yDirection = designDetails.fFootingHeight - designDetails.fConcreteCover_reinforcement_xDirection - designDetails.fd_reinforcement_xDirection - 0.5f * fd_reinforcement_yDirection;
            designDetails.fx_u_xDirection = (designDetails.fA_s1_Xdirection * fReinforcementStrength_fy) / (designDetails.fAlpha_c * designDetails.ff_apostrophe_c * designDetails.fFootingDimension_y);
            float fM_b_Reincorcement_xDirection = designDetails.fA_s_tot_Xdirection * fReinforcementStrength_fy * (designDetails.fd_effective_xDirection - (designDetails.fx_u_xDirection / 2f));
            float fM_b_Concrete_xDirection = designDetails.fAlpha_c * designDetails.ff_apostrophe_c * designDetails.fFootingDimension_y * designDetails.fx_u_xDirection * (designDetails.fd_effective_xDirection - (designDetails.fx_u_xDirection / 2f));
            designDetails.fM_b_footing_xDirection = Math.Min(fM_b_Reincorcement_xDirection, fM_b_Concrete_xDirection); // Note: Values should be identical.

            designDetails.fEta_bending_M_footing = Math.Abs(designDetails.fM_asterix_footingdesign_xDirection) / (designDetails.fPhi_b_foundations * designDetails.fM_b_footing_xDirection);
            fEta_max = MathF.Max(fEta_max, designDetails.fEta_bending_M_footing);

            // TODO - zapracovat Winklerov nosnik na pruznom podlozi je jednotlive patky, suvisly zakladovy pas zatazeny viacerymi stlpmi

            // Minimum reinforcement

            // | f´c (MPa) | fy = 300 MPa | fy = 500 MPa |
            // | 25        | 0.0047       | 0.0028       |
            // | 30        | 0.0047       | 0.0028       |
            // | 40        | 0.0053       | 0.0032       |
            // | 50        | 0.0059       | 0.0035       |

            // Minimum longitudinal reinforcement ratio
            designDetails.fp_ratio_xDirection = 2 * designDetails.fA_s_tot_Xdirection / (designDetails.fFootingDimension_y * designDetails.fFootingHeight); // Same reinforcement at the bottom and top surface
            designDetails.fp_ratio_limit_minimum_xDirection = eq_concrete.Eq_9_1_ratio(designDetails.ff_apostrophe_c, fReinforcementStrength_fy);

            designDetails.fEta_MinimumReinforcement_xDirection = designDetails.fp_ratio_limit_minimum_xDirection / designDetails.fp_ratio_xDirection;
            fEta_max = MathF.Max(fEta_max, designDetails.fEta_MinimumReinforcement_xDirection);

            //  Shear
            designDetails.fV_asterix_footingdesign_shear = designDetails.fq_linear_xDirection * designDetails.fFootingDimension_x / 2f; // ??? jednoducho podpoprety nosnik ???
            designDetails.fA_cv_xDirection = designDetails.fd_effective_xDirection * designDetails.fFootingDimension_y;

            // pw - proportion of flexural tension reinforcement within one-quarter of the effective depth of the member closest to the extreme tension reinforcement to the shear area, Acv
            designDetails.fp_w_xDirection = designDetails.fA_s_tot_Xdirection / designDetails.fA_cv_xDirection;
            designDetails.fk_a = eq_concrete.Get_k_a_93934(0.02f); // TODO - user defined
            designDetails.fk_d = eq_concrete.Get_k_d_93934(); // TODO - dopracovat
            designDetails.fv_b_xDirection = eq_concrete.Get_v_b_93934(designDetails.fp_w_xDirection, designDetails.ff_apostrophe_c);
            designDetails.fv_c_xDirection = eq_concrete.Eq_9_5_____(designDetails.fk_d, designDetails.fk_a, designDetails.fv_b_xDirection);
            designDetails.fV_c_xDirection = eq_concrete.Eq_9_4_____(designDetails.fv_c_xDirection, designDetails.fA_cv_xDirection);
            designDetails.fPhi_v_foundations = 0.85f;

            designDetails.fEta_shear_V_footing = Math.Abs(designDetails.fV_asterix_footingdesign_shear) / (designDetails.fPhi_v_foundations * designDetails.fV_c_xDirection);
            fEta_max = MathF.Max(fEta_max, designDetails.fEta_shear_V_footing);

            // Punching shear
            float fReactionArea_dimension_x = designDetails.fplateWidth_x;
            float fReactionArea_dimension_y = designDetails.fplateWidth_y;

            float fcriticalPerimeterDimension_x = 2f * Math.Min(designDetails.fd_effective_xDirection, designDetails.fe_x_BasePlateToFootingEdge) + fReactionArea_dimension_x;
            float fcriticalPerimeterDimension_y = 2f * Math.Min(fd_effective_yDirection, designDetails.fe_y_BasePlateToFootingEdge) + fReactionArea_dimension_y; // TODO - Zohladnit ak je stlp blizsie k okraju nez fd

            designDetails.fcriticalPerimeter_b0 = 2 * fcriticalPerimeterDimension_x + 2 * fcriticalPerimeterDimension_y;

            // Ratio of the long side to the short side of the concentrated load
            designDetails.fBeta_c = Math.Max(fReactionArea_dimension_x, fReactionArea_dimension_y) / Math.Min(fReactionArea_dimension_x, fReactionArea_dimension_y); // 12.7
            designDetails.fAlpha_s = 15; // 20 for interior columns, 15 for edge columns, 10 for corner columns TODO - zapracovat identifikaciu stlpa
            designDetails.fd_average = (designDetails.fd_effective_xDirection + fd_effective_yDirection) / 2f;
            designDetails.fk_ds = eq_concrete.Get_k_ds_12732(designDetails.fd_average);

            // Nominal shear stress resisted by the concrete
            designDetails.fv_c_126 = eq_concrete.Eq_12_6____(designDetails.fk_ds, designDetails.fBeta_c, designDetails.ff_apostrophe_c);
            designDetails.fv_c_127 = eq_concrete.Eq_12_7____(designDetails.fk_ds, designDetails.fAlpha_s, designDetails.fd_average, designDetails.fcriticalPerimeter_b0, designDetails.ff_apostrophe_c);
            designDetails.fv_c_128 = eq_concrete.Eq_12_8____(designDetails.fk_ds, designDetails.ff_apostrophe_c);

            designDetails.fv_c_12732 = MathF.Min(designDetails.fv_c_126, designDetails.fv_c_127, designDetails.fv_c_128);

            // 12.7.3.4 Maximum nominal shear stress
            float fv_c_max = 0.5f * MathF.Sqrt(designDetails.ff_apostrophe_c);
            if (designDetails.fv_c_12732 > fv_c_max)
                designDetails.fv_c_12732 = fv_c_max;

            designDetails.fV_c_12731 = eq_concrete.Get_V_c_12731(designDetails.fv_c_12732, designDetails.fcriticalPerimeter_b0, designDetails.fd_average);

            // 12.7.4 Shear reinforcement consisting of bars or wires or stirrups
            float fReinforcementArea_A_v_xDirection = 2 * designDetails.fA_s_tot_Xdirection * fcriticalPerimeterDimension_y / designDetails.fFootingDimension_y; // TODO ?? horna aj spodna vyztuz ak su rovnake
            float fReinforcementArea_A_v_yDirection = 2 * fA_s_tot_Ydirection * fcriticalPerimeterDimension_x / designDetails.fFootingDimension_x;

            float ff_yv = fReinforcementStrength_fy;

            designDetails.fV_s_xDirection = fReinforcementArea_A_v_xDirection * ff_yv * designDetails.fd_effective_xDirection / designDetails.fSpacing_yDirection;
            designDetails.fV_s_yDirection = fReinforcementArea_A_v_yDirection * ff_yv * fd_effective_yDirection / fSpacing_xDirection;

            // 12.7.3.1 Nominal shear strength for punching shear
            designDetails.fV_n_12731_xDirection = eq_concrete.Eq_12_4____(designDetails.fV_s_xDirection, designDetails.fV_c_12731);
            designDetails.fEta_punching_12731_xDirection = eq_concrete.Eq_12_5____(designDetails.fN_asterix_joint_bearing, designDetails.fPhi_v_foundations, designDetails.fV_n_12731_xDirection);
            fEta_max = MathF.Max(fEta_max, designDetails.fEta_punching_12731_xDirection);

            designDetails.fV_n_12731_yDirection = eq_concrete.Eq_12_4____(designDetails.fV_s_yDirection, designDetails.fV_c_12731);
            designDetails.fEta_punching_12731_yDirection = eq_concrete.Eq_12_5____(designDetails.fN_asterix_joint_bearing, designDetails.fPhi_v_foundations, designDetails.fV_n_12731_yDirection);
            fEta_max = MathF.Max(fEta_max, designDetails.fEta_punching_12731_yDirection);
            */

            // Validation - negative design ratio
            if (designDetails.fEta_N_t_5423_plate < 0 ||
                designDetails.fEta_V_yv_3341_plate < 0 ||
                designDetails.fEta_Mb_plate < 0 ||
                designDetails.fEta_MainMember < 0 ||
                designDetails.fEta_Mb_MainMember_oneside_plastic < 0 ||
                designDetails.fEta_Vb_5424_MainMember < 0 ||
                designDetails.fEta_V_fv_5425_MainMember < 0 ||
                designDetails.fEta_V_fv_5425_Plate < 0 ||
                designDetails.fEta_V_w_5426 < 0 ||
                designDetails.fEta_N_t_5423_MainMember < 0// ||
                //designDetails.fEta_532_1 < 0 ||
                //designDetails.fEta_5342 < 0 ||
                //designDetails.fEta_5343 < 0 ||
                //designDetails.fEta_5351_2 < 0 ||
                //designDetails.fEta_5352_1 < 0 ||
                //designDetails.fEta_5353 < 0 ||
                //designDetails.fEta_17571_group < 0 ||
                //designDetails.fEta_17572_group < 0 ||
                //designDetails.fEta_17572_single < 0 ||
                //designDetails.fEta_17573_group < 0 ||
                //designDetails.fEta_17574_single < 0 ||
                //designDetails.fEta_17581_group < 0 ||
                //designDetails.fEta_17582_group < 0 ||
                //designDetails.fEta_17582_single < 0 ||
                //designDetails.fEta_17583_group < 0 ||
                //designDetails.fEta_17583_single < 0 ||
                //designDetails.fEta_17584_group < 0 ||
                //designDetails.fEta_17566_group < 0 ||
                //fEta_C17566_group < 0 ||
                //designDetails.fEta_footing_uplift < 0 ||
                //designDetails.fEta_footing_bearing < 0 ||
                //designDetails.fEta_bending_M_footing < 0 ||
                //designDetails.fEta_MinimumReinforcement_xDirection < 0 ||
                //designDetails.fEta_shear_V_footing < 0 ||
                //designDetails.fEta_punching_12731_xDirection < 0 ||
                //designDetails.fEta_punching_12731_yDirection < 0
                )
            {
                throw new Exception("Design ratio is invalid!");
            }

            // Validation - infinity design ratio
            if (fEta_max > 9e+10)
            {
                throw new Exception("Design ratio is invalid!");
            }

            // Store details
            if (bSaveDetails)
                joint_temp.DesignDetails = designDetails;
        }

        public void CalculateDesignRatioBaseJointFooting(CFoundation foundation, designInternalForces_AS4600 sDIF_temp, bool bSaveDetails = false)
        {
            CConCom_Plate_B_basic basePlate;

            if (plate is CConCom_Plate_B_basic)
                basePlate = (CConCom_Plate_B_basic)plate;
            else
            {
                throw new Exception("Invalid object of base plate.");
            }

            CJointDesignDetails_BaseJointFooting designDetails = new CJointDesignDetails_BaseJointFooting();

            // Anchors
            designDetails.fN_asterix_joint_uplif = Math.Max(sDIF_temp.fN, 0); // Tension in column - positive
            designDetails.fN_asterix_joint_bearing = Math.Min(sDIF_temp.fN, 0); // Compression in column - negative

            designDetails.fV_asterix_x_joint = Math.Abs(sDIF_temp.fV_xu_xx);
            designDetails.fV_asterix_y_joint = Math.Abs(sDIF_temp.fV_yv_yy);
            designDetails.fV_asterix_res_joint = 0f;

            if (!MathF.d_equal(designDetails.fV_asterix_x_joint, 0) || !MathF.d_equal(designDetails.fV_asterix_y_joint, 0))
                designDetails.fV_asterix_res_joint = MathF.Sqrt(MathF.Pow2(designDetails.fV_asterix_x_joint) + MathF.Pow2(designDetails.fV_asterix_y_joint));

            int iNumberAnchors = basePlate.AnchorArrangement.Anchors.Length;
            designDetails.iNumberAnchors = basePlate.AnchorArrangement.IHolesNumber;
            designDetails.iNumberAnchors_t = designDetails.iNumberAnchors; // Total number of anchors active in tension - all anchors active as default
            designDetails.iNumberAnchors_v = designDetails.iNumberAnchors; // Total number of anchors active in shear - all anchors active as default

            CAnchorArrangement_BB_BG anchorArrangement;

            if (basePlate.AnchorArrangement is CAnchorArrangement_BB_BG)
                anchorArrangement = (CAnchorArrangement_BB_BG)basePlate.AnchorArrangement;
            else
            {
                throw new Exception("Not implemented arrangmement of anchors.");
            }

            int iNumberAnchors_x = anchorArrangement.NumberOfAnchorsInYDirection;
            int iNumberAnchors_y = anchorArrangement.NumberOfAnchorsInZDirection;

            designDetails.fN_asterix_anchor_uplif = designDetails.fN_asterix_joint_uplif / designDetails.iNumberAnchors_t; // Design axial force per anchor
            designDetails.fV_asterix_anchor = designDetails.fV_asterix_res_joint / designDetails.iNumberAnchors_v; // Design shear force per anchor

            designDetails.fplateWidth_x = (float)joint.m_MainMember.CrScStart.b; // TODO - zapracovat priamo nacitanie parametrov plate type BA - BG
            designDetails.fplateWidth_y = (float)joint.m_MainMember.CrScStart.h; // TODO - zapracovat priamo nacitanie parametrov plate type BA - BG

            designDetails.fFootingDimension_x = foundation.m_fDim1; // Input
            designDetails.fFootingDimension_y = foundation.m_fDim2; // Input
            designDetails.fFootingHeight = foundation.m_fDim3; // Input

            designDetails.fe_x_AnchorToPlateEdge = 0.05f; // TODO - Distance between anchor and plate edge
            designDetails.fe_y_AnchorToPlateEdge = 0.05f; // TODO - Distance between anchor and plate edge

            designDetails.fe_x_BasePlateToFootingEdge = 0.5f * (designDetails.fFootingDimension_x - designDetails.fplateWidth_x); // TODO - Distance between base plate and footing edge - zapracovat excentricitu
            designDetails.fe_y_BasePlateToFootingEdge = 0.5f * (designDetails.fFootingDimension_y - designDetails.fplateWidth_y); // TODO - Distance between base plate and footing edge - zapracovat excentricitu

            designDetails.fu_x_Washer = 0.08f; // Input
            designDetails.fu_y_Washer = 0.08f; // Input

            CMat_02_00 materialConcrete = new CMat_02_00();
            materialConcrete = (CMat_02_00)foundation.m_Mat;

            designDetails.ff_apostrophe_c = (float)materialConcrete.Fck; // Characteristic compressive (cylinder) concrete strength
            designDetails.fRho_c = materialConcrete.m_fRho; // Density of concrete - TODO - nacitat z materialu zakladov

            // Anchors (bolts)
            designDetails.fd_s = basePlate.AnchorArrangement.referenceAnchor.Diameter_thread;
            designDetails.fd_f = basePlate.AnchorArrangement.referenceAnchor.Diameter_shank;

            designDetails.fA_c = basePlate.AnchorArrangement.referenceAnchor.Area_c_thread; // Core / thread area
            designDetails.fA_o = basePlate.AnchorArrangement.referenceAnchor.Area_o_shank; // Shank area

            designDetails.ff_y_anchor = basePlate.AnchorArrangement.referenceAnchor.m_Mat.m_ff_yk[0];
            designDetails.ff_u_anchor = basePlate.AnchorArrangement.referenceAnchor.m_Mat.m_ff_u[0];

            // AS / NZS 4600:2018 - 5.3 Bolted connections
            // Base plate design
            // 5.3.2 Tearout
            designDetails.fPhi_v_532 = 0.7f;
            designDetails.fV_f_532 = eq.Eq_532_2___(ft_1_plate, designDetails.fe_y_AnchorToPlateEdge, ff_uk_1_plate);
            designDetails.fEta_532_1 = eq.Eq_5351_1__(designDetails.fV_asterix_anchor, designDetails.fPhi_v_532, designDetails.fV_f_532);
            fEta_max_Footing = MathF.Max(fEta_max_Footing, designDetails.fEta_532_1);

            // 5.3.4.2 Bearing capacity without considering bolt hole deformation
            designDetails.fPhi_v_534 = 0.6f;
            designDetails.fAlpha_5342 = eq.Get_Alpha_Table_5342_A(ETypesOfBearingConnection.eType3);
            designDetails.fC_5342 = eq.Get_Factor_C_Table_5342_B(designDetails.fd_f, ft_1_plate);
            designDetails.fV_b_5342 = eq.Eq_5342____(designDetails.fAlpha_5342, designDetails.fC_5342, designDetails.fd_f, ft_1_plate, ff_uk_1_plate);
            designDetails.fEta_5342 = Math.Abs(designDetails.fV_asterix_anchor) / (designDetails.fPhi_v_534 * designDetails.fV_b_5342);
            fEta_max_Footing = MathF.Max(fEta_max_Footing, designDetails.fEta_5342);

            // 5.3.4.3 Bearing capacity at a bolt hole deformation of 6 mm
            designDetails.fV_b_5343 = eq.Eq_5343____(designDetails.fd_f, ft_1_plate, ff_uk_1_plate);
            designDetails.fEta_5343 = Math.Abs(designDetails.fV_asterix_anchor) / (designDetails.fPhi_v_534 * designDetails.fV_b_5343);
            fEta_max_Footing = MathF.Max(fEta_max_Footing, designDetails.fEta_5343);

            // Bolt design / Anchor design
            // 5.3.5.1 Bolt in shear
            designDetails.fPhi_535 = 0.8f;
            int iNumberOfShearPlanesOfBolt_core = 1; // Jednostrizny spoj - strih jardom skrutky
            designDetails.fV_fv_5351_2_anchor = eq.Eq_5351_2__(designDetails.ff_u_anchor, iNumberOfShearPlanesOfBolt_core, designDetails.fA_c, 0, designDetails.fA_o); // Uvazovane konzervativne jedna smykova plocha a zavit je aj v smykovej ploche
            designDetails.fEta_5351_2 = eq.Eq_5351_1__(designDetails.fV_asterix_anchor, designDetails.fPhi_535, designDetails.fV_fv_5351_2_anchor);
            fEta_max_Footing = MathF.Max(fEta_max_Footing, designDetails.fEta_5351_2);

            // 5.3.5.2 Bolt in tension
            designDetails.fN_ft_5352_1 = eq.Eq_5352_2__(designDetails.fA_c, designDetails.ff_u_anchor);
            designDetails.fEta_5352_1 = eq.Eq_5352_1__(designDetails.fN_asterix_anchor_uplif, designDetails.fPhi_535, designDetails.fN_ft_5352_1);
            fEta_max_Footing = MathF.Max(fEta_max_Footing, designDetails.fEta_5352_1);

            // 5.3.5.3 Bolt subject to combined shear and tension
            float fPortion_V_5353;
            float fPortion_N_5353;
            designDetails.fEta_5353 = eq.Eq_5353____(designDetails.fV_asterix_anchor, designDetails.fPhi_535, designDetails.fV_fv_5351_2_anchor, designDetails.fN_asterix_anchor_uplif, 0.8f, designDetails.fN_ft_5352_1, out fPortion_V_5353, out fPortion_N_5353);
            fEta_max_Footing = MathF.Max(fEta_max_Footing, designDetails.fEta_5353);

            // NZS 3101.1 - 2006
            designDetails.fElasticityFactor_1764 = 0.75f; // EQ load combination - 0.75, other 1.00

            // 17.5.6 Strength of cast -in anchors

            // 17.5.6.4 Strength reduction factors
            designDetails.fPhi_anchor_tension_173 = 0.75f;
            designDetails.fPhi_anchor_shear_174 = 0.65f;

            designDetails.fPhi_concrete_tension_174a = 0.65f;
            designDetails.fPhi_concrete_shear_174b = 0.65f;

            // 17.5.7.1  Steel strength of anchor in tension
            // Group of anchors
            designDetails.fA_se = designDetails.fA_c; // Effective cross-sectional area of an anchor
            designDetails.fN_s_176_group = eq_concrete.Eq_17_6____(designDetails.iNumberAnchors_t, designDetails.fA_se, designDetails.ff_u_anchor);
            designDetails.fEta_17571_group = eq_concrete.Eq_17_1____(designDetails.fN_asterix_joint_uplif, designDetails.fPhi_anchor_tension_173, designDetails.fN_s_176_group);
            fEta_max_Footing = MathF.Max(fEta_max_Footing, designDetails.fEta_17571_group);

            // 17.5.7.2  Strength of concrete breakout of anchor
            // Group of anchors

            // Figure C17.4 – Definition of dimension e´n for group anchors
            designDetails.fe_apostrophe_n = 0f;                           // the distance between the resultant tension load on a group of anchors in tension and the centroid of the group of anchors loaded in tension(always taken as positive)
            designDetails.fConcreteCover = 0.07f; // Input
            designDetails.fh_ef = designDetails.fFootingHeight - designDetails.fConcreteCover;        // effective anchor embedment depth
            designDetails.fs_2_x = 0f;                                    // centre-to-centre spacing of the anchors
            designDetails.fs_1_y = 0.23f;                                 // centre-to-centre spacing of the anchors
            designDetails.fs_min = Math.Min(designDetails.fs_2_x, designDetails.fs_1_y);
            designDetails.fc_2_x = 0.55f;
            designDetails.fc_1_y = 0.55f;
            designDetails.fc_min = Math.Min(designDetails.fc_2_x, designDetails.fc_1_y);
            designDetails.fk = 10f; // for cast-in anchors
            designDetails.fLambda_53 = eq_concrete.Eq_5_3_____(designDetails.fRho_c);

            designDetails.fe_x_AnchorToPlateEdge = 0.5f * (designDetails.fplateWidth_x - (iNumberAnchors_x - 1) * designDetails.fs_2_x);
            designDetails.fe_y_AnchorToPlateEdge = 0.5f * (designDetails.fplateWidth_y - (iNumberAnchors_y - 1) * designDetails.fs_1_y);

            designDetails.fPsi_1_group = eq_concrete.Eq_17_8____(designDetails.fe_apostrophe_n, designDetails.fh_ef);
            designDetails.fPsi_2 = eq_concrete.Get_Psi_2__(designDetails.fc_min, designDetails.fh_ef);

            // Ψ3 = 1.25 for cast -in anchors in uncracked concrete
            // Ψ3 = 1.0 for concrete which is cracked at service load levels.
            designDetails.fPsi_3 = 1.25f; // modification factor or cracking of concrete
            designDetails.fA_no_group = (2f * 1.5f * designDetails.fh_ef) * (2f * 1.5f * designDetails.fh_ef);

            float fAn_Length_x_group = Math.Min(designDetails.fc_2_x, 1.5f * designDetails.fh_ef) + 1.5f * designDetails.fh_ef + ((iNumberAnchors_x - 1) * designDetails.fs_2_x);
            float fAn_Length_y_group = Math.Min(designDetails.fc_1_y, 1.5f * designDetails.fh_ef) + 1.5f * designDetails.fh_ef + ((iNumberAnchors_y - 1) * designDetails.fs_1_y);
            designDetails.fA_n_group = Math.Min(fAn_Length_x_group * fAn_Length_y_group, designDetails.iNumberAnchors_t * designDetails.fA_no_group);

            designDetails.fN_b_179_group = eq_concrete.Eq_17_9____(designDetails.fk, designDetails.fLambda_53, Math.Min(designDetails.ff_apostrophe_c, 70e+6f), designDetails.fh_ef);
            designDetails.fN_b_179a_group = eq_concrete.Eq_17_9a___(designDetails.fLambda_53, designDetails.ff_apostrophe_c, designDetails.fh_ef);

            if (0.280f <= designDetails.fh_ef && designDetails.fh_ef <= 0.635f && designDetails.fN_b_179_group > designDetails.fN_b_179a_group)
            {
                designDetails.fN_b_179_group = designDetails.fN_b_179a_group;
            }

            designDetails.fN_cb_177_group = eq_concrete.Eq_17_7____(designDetails.fPsi_1_group, designDetails.fPsi_2, designDetails.fPsi_3, designDetails.fA_n_group, designDetails.fA_no_group, designDetails.fN_b_179_group);

            designDetails.fEta_17572_group = eq_concrete.Eq_17_1____(designDetails.fN_asterix_joint_uplif, designDetails.fPhi_concrete_tension_174a, designDetails.fN_cb_177_group);
            fEta_max_Footing = MathF.Max(fEta_max_Footing, designDetails.fEta_17572_group);

            // Single anchor - edge
            designDetails.fPsi_1_single = 1.0f;
            designDetails.fA_no_single = (2f * 1.5f * designDetails.fh_ef) * (2f * 1.5f * designDetails.fh_ef);
            float fAn_Length_x_single = Math.Min(designDetails.fc_2_x, 1.5f * designDetails.fh_ef) + 1.5f * designDetails.fh_ef;
            float fAn_Length_y_single = Math.Min(designDetails.fc_1_y, 1.5f * designDetails.fh_ef) + 1.5f * designDetails.fh_ef;
            designDetails.fA_n_single = Math.Min(fAn_Length_x_single * fAn_Length_y_single, designDetails.fA_no_single);

            designDetails.fN_b_179_single = eq_concrete.Eq_17_9____(designDetails.fk, designDetails.fLambda_53, Math.Min(designDetails.ff_apostrophe_c, 70e+6f), designDetails.fh_ef);
            designDetails.fN_b_179a_single = eq_concrete.Eq_17_9a___(designDetails.fLambda_53, designDetails.ff_apostrophe_c, designDetails.fh_ef);

            if (0.280f <= designDetails.fh_ef && designDetails.fh_ef <= 0.635f && designDetails.fN_b_179_single > designDetails.fN_b_179a_single)
            {
                designDetails.fN_b_179_single = designDetails.fN_b_179a_single;
            }

            designDetails.fN_cb_177_single = eq_concrete.Eq_17_7____(designDetails.fPsi_1_single, designDetails.fPsi_2, designDetails.fPsi_3, designDetails.fA_n_single, designDetails.fA_no_single, designDetails.fN_b_179_single);

            designDetails.fEta_17572_single = eq_concrete.Eq_17_1____(designDetails.fN_asterix_anchor_uplif, designDetails.fPhi_concrete_tension_174a, designDetails.fN_cb_177_single);
            fEta_max_Footing = MathF.Max(fEta_max_Footing, designDetails.fEta_17572_single);

            // 17.5.7.3  Lower characteristic tension pullout strength of anchor
            // Group of anchors

            designDetails.fm_x = 0.06f; // Input
            designDetails.fm_y = 0.06f; // Input
            designDetails.fA_brg = designDetails.fm_x * designDetails.fm_y; // bearing area of the head of stud or anchor
            designDetails.fN_p_1711_single = eq_concrete.Eq_17_11___(designDetails.ff_apostrophe_c, designDetails.fA_brg);

            // Modification factor for pullout strength
            // Ψ4 = 1.0 for concrete cracked at service load levels but with the extent of cracking controlled by reinforcement distributed in accordance with 2.4.4.4 and 2.4.4.5
            // Ψ4 = 1.4 for concrete with no cracking at service load levels
            designDetails.fPsi_4 = 1.0f;
            designDetails.fN_pn_1710_single = eq_concrete.Eq_17_10___(designDetails.fPsi_4, designDetails.fN_p_1711_single);
            designDetails.fN_pn_1710_group = designDetails.iNumberAnchors_t * designDetails.fN_pn_1710_single;

            designDetails.fEta_17573_group = eq_concrete.Eq_17_1____(designDetails.fN_asterix_joint_uplif, designDetails.fPhi_anchor_tension_173, designDetails.fN_pn_1710_group);
            fEta_max_Footing = MathF.Max(fEta_max_Footing, designDetails.fEta_17573_group);

            // The side face blowout strength of a headed anchor with deep embedment close to an edge
            designDetails.fN_sb_1713_single = float.PositiveInfinity;
            designDetails.fEta_17574_single = 0;

            if (designDetails.fc_min < 0.4f * designDetails.fh_ef)
            {
                // 17.5.7.4 Lower characteristic concrete side face blowout strength
                // Single anchor - edge
                designDetails.fc_1_17574 = designDetails.fc_1_y;

                if (designDetails.fN_asterix_anchor_uplif > 0) // Tension in anchor
                    designDetails.fc_1_17574 = designDetails.fc_min;

                // Anchors subject to shear are located in narrow sections of limited thickness
                float fc_1_limit = MathF.Max(designDetails.fc_2_x / 1.5f, designDetails.fh_ef / 1.5f, designDetails.fs_min / 3f);

                if (designDetails.fc_1_17574 > fc_1_limit)
                    designDetails.fc_1_17574 = fc_1_limit;

                designDetails.fk_1 = eq_concrete.Get_k_1____(designDetails.fc_1_17574, designDetails.fc_2_x);

                designDetails.fN_sb_1713_single = eq_concrete.Eq_17_13___(designDetails.fk_1, designDetails.fc_1_17574, designDetails.fLambda_53, designDetails.fA_brg, designDetails.ff_apostrophe_c);

                designDetails.fEta_17574_single = eq_concrete.Eq_17_1____(designDetails.fN_asterix_anchor_uplif, designDetails.fPhi_concrete_tension_174a, designDetails.fN_sb_1713_single);
                fEta_max_Footing = MathF.Max(fEta_max_Footing, designDetails.fEta_17574_single);
            }

            // Lower characteristic strength in tension
            designDetails.fN_n_nominal_min = MathF.Min(
                designDetails.fN_s_176_group,                                       // 17.5.7.1
                designDetails.fN_cb_177_group,                                      // 17.5.7.2
                designDetails.iNumberAnchors_t * designDetails.fN_cb_177_single,    // 17.5.7.2
                designDetails.fN_pn_1710_group,                                     // 17.5.7.3
                designDetails.iNumberAnchors_t * designDetails.fN_sb_1713_single);  // 17.5.7.4

            // Lower design strength in tension
            designDetails.fN_d_design_min = designDetails.fElasticityFactor_1764 * MathF.Min(
                designDetails.fPhi_anchor_tension_173 * designDetails.fN_s_176_group,                                         // 17.5.7.1
                designDetails.fPhi_concrete_tension_174a * designDetails.fN_cb_177_group,                                     // 17.5.7.2
                designDetails.fPhi_concrete_tension_174a * designDetails.iNumberAnchors_t * designDetails.fN_cb_177_single,   // 17.5.7.2
                designDetails.fPhi_anchor_tension_173 * designDetails.fN_pn_1710_group,                                       // 17.5.7.3
                designDetails.fPhi_concrete_tension_174a * designDetails.iNumberAnchors_t * designDetails.fN_sb_1713_single); // 17.5.7.4

            // 17.5.8 Lower characteristic strength of anchor in shear

            // 17.5.8.1 Lower characteristic shear strength of steel of anchor
            // Group of anchors

            // TODO - rozlisovat typ kotvy - rovnica 17-14 alebo 17-15
            designDetails.fV_s_1714_group = eq_concrete.Eq_17_14___(designDetails.iNumberAnchors_v, designDetails.fA_se, designDetails.ff_u_anchor, designDetails.ff_y_anchor);
            designDetails.fV_s_1715_group = eq_concrete.Eq_17_15___(designDetails.iNumberAnchors_v, designDetails.fA_se, designDetails.ff_u_anchor, designDetails.ff_y_anchor);

            designDetails.fV_s_17581_group = Math.Min(designDetails.fV_s_1714_group, designDetails.fV_s_1715_group);
            designDetails.fEta_17581_group = eq_concrete.Eq_17_2____(designDetails.fV_asterix_res_joint, designDetails.fPhi_anchor_shear_174, designDetails.fV_s_17581_group);
            fEta_max_Footing = MathF.Max(fEta_max_Footing, designDetails.fEta_17573_group);

            // 17.5.8.2 Lower characteristic concrete breakout strength of the anchor in shear perpendicular to edge
            // Group of anchors

            designDetails.fe_apostrophe_v = 0;
            designDetails.fPsi_5_group = eq_concrete.Eq_17_18___(designDetails.fc_1_y, designDetails.fe_apostrophe_v, designDetails.fs_2_x); // s - perpendicular to shear force - Figure C17.7 – Definition of dimensions e´
            designDetails.fPsi_6 = eq_concrete.Get_Psi_6__(designDetails.fc_1_y, designDetails.fc_2_x);

            // Ψ7 = modification factor for cracked concrete, given by:
            // Ψ7 = 1.0 for anchors in cracked concrete with no supplementary reinforcement or with smaller than 12 mm diameter reinforcing bar as supplementary reinforcement
            // Ψ7 = 1.2 for anchors in cracked concrete with a 12 mm diameter reinforcing bar or greater as supplementary reinforcement
            // Ψ7 = 1.4 for concrete that is not cracked at service load levels.
            designDetails.fPsi_7 = 1.0f;

            designDetails.fA_vo = 2 * (1.5f * designDetails.fc_1_y) * (1.5f * designDetails.fc_1_y); // projected concrete failure area of an anchor in shear, when not limited by corner influences, spacing, or member thickness
            float fAv_Length_x_group = 1.5f * designDetails.fc_1_y + Math.Min(1.5f * designDetails.fc_1_y, designDetails.fc_2_x) + (iNumberAnchors_x - 1) * designDetails.fs_2_x;
            float fAv_Depth_h = Math.Min(1.5f * designDetails.fc_1_y, designDetails.fFootingHeight);
            designDetails.fA_v_group = fAv_Length_x_group * fAv_Depth_h; // projected concrete failure area of an anchor or group of anchors in shear
            designDetails.fd_o = designDetails.fd_f;

            designDetails.fk_2 = 0.6f;
            designDetails.fl = Math.Min(designDetails.fh_ef, 8f * designDetails.fd_o); // load-bearing length of anchors for shear, equal to hef for anchors with constant stiffness over the full length of the embedded section but less than 8do.Shall be taken as 0.8 times the effective embedment depth for hooked metal plates.

            if (designDetails.iNumberAnchors != 1 && designDetails.fs_min != 0 && designDetails.fs_min < 0.065f)
                throw new Exception("Distance between anchors s is smaller than 65 mm.");

            designDetails.fV_b_1717a = eq_concrete.Eq_17_17a__(designDetails.fk_2, designDetails.fl, designDetails.fd_o, designDetails.fLambda_53, designDetails.ff_apostrophe_c, designDetails.fc_1_y);
            designDetails.fV_b_1717b = eq_concrete.Eq_17_17b__(designDetails.fLambda_53, designDetails.ff_apostrophe_c, designDetails.fc_1_y);
            designDetails.fV_b_1717 = Math.Min(designDetails.fV_b_1717a, designDetails.fV_b_1717b);
            designDetails.fV_cb_1716_group = eq_concrete.Eq_17_16___(designDetails.fA_v_group, designDetails.fA_vo, designDetails.fPsi_5_group, designDetails.fPsi_6, designDetails.fPsi_7, designDetails.fV_b_1717);

            designDetails.fEta_17582_group = eq_concrete.Eq_17_2____(designDetails.fV_asterix_res_joint, designDetails.fPhi_concrete_shear_174b, designDetails.fV_cb_1716_group);
            fEta_max_Footing = MathF.Max(fEta_max_Footing, designDetails.fEta_17582_group);

            // Single of anchor - edge

            designDetails.fPsi_5_single = 1.0f;
            float fAv_Length_x_signle = (1.5f * designDetails.fc_1_y + Math.Min(1.5f * designDetails.fc_1_y, 0.5f * designDetails.fFootingDimension_x));
            designDetails.fA_v_single = fAv_Length_x_signle * fAv_Depth_h;
            designDetails.fV_cb_1716_single = eq_concrete.Eq_17_16___(designDetails.fA_v_single, designDetails.fA_vo, designDetails.fPsi_5_single, designDetails.fPsi_6, designDetails.fPsi_7, designDetails.fV_b_1717);

            designDetails.fEta_17582_single = eq_concrete.Eq_17_2____(designDetails.fV_asterix_anchor, designDetails.fPhi_concrete_shear_174b, designDetails.fV_cb_1716_single);
            fEta_max_Footing = MathF.Max(fEta_max_Footing, designDetails.fEta_17582_single);

            // 17.5.8.3 Lower characteristic concrete breakout strength of the anchor in shear parallel to edge
            // Group of anchors
            // TODO - zohladnit len paralelnu smykovu silu Vx ???

            designDetails.fV_cb_1721_group = eq_concrete.Eq_17_21___(designDetails.fA_v_group, designDetails.fA_vo, designDetails.fPsi_5_group, designDetails.fPsi_7, designDetails.fV_b_1717);

            designDetails.fEta_17583_group = eq_concrete.Eq_17_2____(designDetails.fV_asterix_res_joint, designDetails.fPhi_concrete_shear_174b, designDetails.fV_cb_1721_group);
            fEta_max_Footing = MathF.Max(fEta_max_Footing, designDetails.fEta_17583_group);

            // Single anchor - edge
            designDetails.fV_cb_1721_single = eq_concrete.Eq_17_21___(designDetails.fA_v_single, designDetails.fA_vo, designDetails.fPsi_5_single, designDetails.fPsi_7, designDetails.fV_b_1717);

            designDetails.fEta_17583_single = eq_concrete.Eq_17_2____(designDetails.fV_asterix_res_joint, designDetails.fPhi_concrete_shear_174b, designDetails.fV_cb_1721_single);
            fEta_max_Footing = MathF.Max(fEta_max_Footing, designDetails.fEta_17583_single);

            // 17.5.8.4 Lower characteristic concrete pry-out of the anchor in shear
            // Group of anchors

            designDetails.fN_cb_17584_group = Math.Min(designDetails.fN_pn_1710_group, designDetails.fN_cb_177_group); // ??? Zohladnit aj N_pn ???
            designDetails.fk_cp_17584 = eq_concrete.Get_k_cp___(designDetails.fh_ef);
            designDetails.fV_cp_1722_group = eq_concrete.Eq_17_22___(designDetails.fk_cp_17584, designDetails.fN_cb_17584_group);

            designDetails.fEta_17584_group = eq_concrete.Eq_17_2____(designDetails.fV_asterix_anchor, designDetails.fPhi_concrete_shear_174b, designDetails.fV_cp_1722_group);
            fEta_max_Footing = MathF.Max(fEta_max_Footing, designDetails.fEta_17584_group);

            // Lower characteristic strength in shear
            designDetails.fV_n_nominal_min = MathF.Min(
                designDetails.fV_s_17581_group,                                     // 17.5.8.1
                designDetails.fV_cb_1716_group,                                     // 17.5.8.2
                designDetails.iNumberAnchors_v * designDetails.fV_cb_1716_single,   // 17.5.8.2
                designDetails.fV_cb_1721_group,                                     // 17.5.8.3
                designDetails.iNumberAnchors_v * designDetails.fV_cb_1721_single,   // 17.5.8.3
                designDetails.fV_cp_1722_group);                                    // 17.5.8.4

            // Lower design strength in shear
            designDetails.fV_d_design_min = designDetails.fElasticityFactor_1764 * MathF.Min(
                designDetails.fPhi_anchor_shear_174 * designDetails.fV_s_17581_group,                                          // 17.5.8.1
                designDetails.fPhi_concrete_shear_174b * designDetails.fV_cb_1716_group,                                       // 17.5.8.2
                designDetails.fPhi_concrete_shear_174b * designDetails.iNumberAnchors_v * designDetails.fV_cb_1716_single,     // 17.5.8.2
                designDetails.fPhi_concrete_shear_174b * designDetails.fV_cb_1721_group,                                       // 17.5.8.3
                designDetails.fPhi_concrete_shear_174b * designDetails.iNumberAnchors_v * designDetails.fV_cb_1721_single,     // 17.5.8.3
                designDetails.fPhi_concrete_shear_174b * designDetails.fV_cp_1722_group);                                      // 17.5.8.4

            // 17.5.6.6 Interaction of tension and shear – simplified procedures
            // Group of anchors

            // 17.5.6.6(Eq. 17–5)
            designDetails.fEta_17566_group = eq_concrete.Eq_17566___(designDetails.fN_asterix_joint_uplif, designDetails.fN_d_design_min, designDetails.fV_asterix_res_joint, designDetails.fV_d_design_min);
            fEta_max_Footing = MathF.Max(fEta_max_Footing, designDetails.fEta_17566_group);

            // C17.5.6.6 (Figure C17.1)
            bool bUseC17566Equation = true;
            float fEta_C17566_group = 0;

            if (bUseC17566Equation)
            {
                fEta_C17566_group = eq_concrete.Eq_C17566__(designDetails.fN_asterix_joint_uplif, designDetails.fN_d_design_min, designDetails.fV_asterix_res_joint, designDetails.fV_d_design_min);
                fEta_max_Footing = MathF.Max(fEta_max_Footing, fEta_C17566_group);
            }

            // Footings
            designDetails.fGamma_F_uplift = 0.9f; // Load Factor -uplift
            designDetails.fGamma_F_bearing = 1.2f; // Load Factor - bearing

            //float fN_asterix_joint_uplif =
            //float fN_asterix_joint_bearing 

            designDetails.fc_nominal_soil_bearing_capacity = 58000f; // Pa - soil bearing capacity - TODO - user defined

            // Footing pad
            designDetails.fA_footing = designDetails.fFootingDimension_x * designDetails.fFootingDimension_y; // Area of footing pad
            designDetails.fV_footing = designDetails.fA_footing * designDetails.fFootingHeight;  // Volume of footing pad
            float fRho_c_footing = materialConcrete.m_fRho; // Density of dry concrete - foundations
            designDetails.fG_footing = designDetails.fV_footing * fRho_c_footing; // Self-weight [N] - footing pad

            // Tributary floor volume
            float ft_floor = 0.125f; // TODO - user-defined
            float fa_tributary_floor = 0.6f; // TODO - tributary dimension;
            float fLength_x_tributary_floor = designDetails.fFootingDimension_x + 2 * fa_tributary_floor; // TODO - zistit ci je stlp v rohu, ak ano uvazovat len jednu stranu
            float fLength_y_tributary_floor = designDetails.fFootingDimension_y + fa_tributary_floor; // Okraj - uvazujem sa len jedna strana patky
            float fA_tributary_floor = fLength_x_tributary_floor * fLength_y_tributary_floor - designDetails.fA_footing;
            float fV_tributary_floor = fA_tributary_floor * ft_floor;
            float fRho_c_floor = 2200f; // Density of dry concrete - concrete floor
            designDetails.fG_tributary_floor = fV_tributary_floor * fRho_c_floor; // Self-weight [N] - tributary concrete floor

            // Additional material above the footing
            float ft_additional_material = 0.0f; // User-defined
            float fRho_additional_material = 2200f; // Can be concrete or soil
            float fVolume_additional_material = designDetails.fA_footing * ft_additional_material;
            designDetails.fG_additional_material = fVolume_additional_material * fRho_additional_material; // Self-weight [N]

            // Uplift
            float fG_design_footing_uplift = designDetails.fGamma_F_uplift * designDetails.fG_footing;
            float fG_design_tributary_floor_uplift = designDetails.fGamma_F_uplift * fG_design_footing_uplift;
            float fG_design_additional_material_uplift = designDetails.fGamma_F_uplift * designDetails.fG_additional_material;
            designDetails.fG_design_uplift = fG_design_footing_uplift + fG_design_tributary_floor_uplift + fG_design_additional_material_uplift;

            // Bearing
            float fG_design_footing_bearing = designDetails.fGamma_F_bearing * designDetails.fG_footing;
            float fG_design_additional_material_bearing = designDetails.fGamma_F_bearing * designDetails.fG_additional_material;
            designDetails.fG_design_bearing = fG_design_footing_bearing + fG_design_additional_material_bearing;

            // Design ratio - uplift and bearing force
            designDetails.fEta_footing_uplift = Math.Abs(designDetails.fN_asterix_joint_uplif) / designDetails.fG_design_uplift;
            fEta_max_Footing = MathF.Max(fEta_max_Footing, designDetails.fEta_footing_uplift);

            designDetails.fN_design_bearing_total = Math.Abs(designDetails.fN_asterix_joint_bearing) + designDetails.fG_design_bearing;
            designDetails.fPressure_bearing = Math.Abs(designDetails.fN_design_bearing_total) / designDetails.fA_footing;
            designDetails.fSafetyFactor = 0.5f; // TODO - zistit aky je faktor
            designDetails.fEta_footing_bearing = designDetails.fPressure_bearing / (designDetails.fSafetyFactor * designDetails.fc_nominal_soil_bearing_capacity);
            fEta_max_Footing = MathF.Max(fEta_max_Footing, designDetails.fEta_footing_bearing);

            // Bending - design of reinforcement
            // Reinforcement bars in x direction (parallel to the wall)
            designDetails.fq_linear_xDirection = Math.Abs(designDetails.fN_design_bearing_total) / designDetails.fFootingDimension_x;
            designDetails.fM_asterix_footingdesign_xDirection = designDetails.fq_linear_xDirection * MathF.Pow2(designDetails.fFootingDimension_x) / 8f; // ??? jednoducho podpoprety nosnik ???

            designDetails.fd_reinforcement_xDirection = foundation.Reference_Bottom_Bar_x.Diameter; // TODO - user defined
            float fd_reinforcement_yDirection = foundation.Reference_Bottom_Bar_y.Diameter; // TODO - user defined (default above the reinforcement in x-direction)
            designDetails.fA_s1_Xdirection = MathF.fPI * MathF.Pow2(designDetails.fd_reinforcement_xDirection) / 4f; // Reinforcement bar cross-sectional area
            float fA_s1_Ydirection = MathF.fPI * MathF.Pow2(fd_reinforcement_yDirection) / 4f; // Reinforcement bar cross-sectional area
            designDetails.iNumberOfBarsInXDirection = foundation.Count_Bottom_Bars_x; // TODO - user defined
            int iNumberOfBarsInYDirection = foundation.Count_Bottom_Bars_y; // TODO - user defined

            designDetails.fA_s_tot_Xdirection = designDetails.iNumberOfBarsInXDirection * designDetails.fA_s1_Xdirection;
            float fA_s_tot_Ydirection = iNumberOfBarsInYDirection * fA_s1_Ydirection;

            float fConcreteCover_reinforcement_side = foundation.ConcreteCover;
            float fSpacing_xDirection = (designDetails.fFootingDimension_x - 2 * fConcreteCover_reinforcement_side - fd_reinforcement_yDirection) / (iNumberOfBarsInYDirection - 1);
            designDetails.fSpacing_yDirection = (designDetails.fFootingDimension_y - 2 * fConcreteCover_reinforcement_side - designDetails.fd_reinforcement_xDirection) / (designDetails.iNumberOfBarsInXDirection - 1);

            string sReinforcingSteelGrade_Name = "500E"; // Input
            float fReinforcementStrength_fy = 500e+6f; //foundation.Streng // TODO - user defined

            designDetails.fAlpha_c = 0.85f;
            designDetails.fPhi_b_foundations = 0.85f;

            designDetails.fConcreteCover_reinforcement_xDirection = foundation.ConcreteCover + 0.5f * fd_reinforcement_yDirection;

            designDetails.fd_effective_xDirection = designDetails.fFootingHeight - designDetails.fConcreteCover_reinforcement_xDirection - 0.5f * designDetails.fd_reinforcement_xDirection;
            float fd_effective_yDirection = designDetails.fFootingHeight - designDetails.fConcreteCover_reinforcement_xDirection - designDetails.fd_reinforcement_xDirection - 0.5f * fd_reinforcement_yDirection;
            designDetails.fx_u_xDirection = (designDetails.fA_s1_Xdirection * fReinforcementStrength_fy) / (designDetails.fAlpha_c * designDetails.ff_apostrophe_c * designDetails.fFootingDimension_y);
            float fM_b_Reincorcement_xDirection = designDetails.fA_s_tot_Xdirection * fReinforcementStrength_fy * (designDetails.fd_effective_xDirection - (designDetails.fx_u_xDirection / 2f));
            float fM_b_Concrete_xDirection = designDetails.fAlpha_c * designDetails.ff_apostrophe_c * designDetails.fFootingDimension_y * designDetails.fx_u_xDirection * (designDetails.fd_effective_xDirection - (designDetails.fx_u_xDirection / 2f));
            designDetails.fM_b_footing_xDirection = Math.Min(fM_b_Reincorcement_xDirection, fM_b_Concrete_xDirection); // Note: Values should be identical.

            designDetails.fEta_bending_M_footing = Math.Abs(designDetails.fM_asterix_footingdesign_xDirection) / (designDetails.fPhi_b_foundations * designDetails.fM_b_footing_xDirection);
            fEta_max_Footing = MathF.Max(fEta_max_Footing, designDetails.fEta_bending_M_footing);

            // TODO - zapracovat Winklerov nosnik na pruznom podlozi je jednotlive patky, suvisly zakladovy pas zatazeny viacerymi stlpmi

            // Minimum reinforcement

            // | f´c (MPa) | fy = 300 MPa | fy = 500 MPa |
            // | 25        | 0.0047       | 0.0028       |
            // | 30        | 0.0047       | 0.0028       |
            // | 40        | 0.0053       | 0.0032       |
            // | 50        | 0.0059       | 0.0035       |

            // Minimum longitudinal reinforcement ratio
            designDetails.fp_ratio_xDirection = 2 * designDetails.fA_s_tot_Xdirection / (designDetails.fFootingDimension_y * designDetails.fFootingHeight); // Same reinforcement at the bottom and top surface
            designDetails.fp_ratio_limit_minimum_xDirection = eq_concrete.Eq_9_1_ratio(designDetails.ff_apostrophe_c, fReinforcementStrength_fy);

            designDetails.fEta_MinimumReinforcement_xDirection = designDetails.fp_ratio_limit_minimum_xDirection / designDetails.fp_ratio_xDirection;
            fEta_max_Footing = MathF.Max(fEta_max_Footing, designDetails.fEta_MinimumReinforcement_xDirection);

            //  Shear
            designDetails.fV_asterix_footingdesign_shear = designDetails.fq_linear_xDirection * designDetails.fFootingDimension_x / 2f; // ??? jednoducho podpoprety nosnik ???
            designDetails.fA_cv_xDirection = designDetails.fd_effective_xDirection * designDetails.fFootingDimension_y;

            // pw - proportion of flexural tension reinforcement within one-quarter of the effective depth of the member closest to the extreme tension reinforcement to the shear area, Acv
            designDetails.fp_w_xDirection = designDetails.fA_s_tot_Xdirection / designDetails.fA_cv_xDirection;
            designDetails.fk_a = eq_concrete.Get_k_a_93934(0.02f); // TODO - user defined
            designDetails.fk_d = eq_concrete.Get_k_d_93934(); // TODO - dopracovat
            designDetails.fv_b_xDirection = eq_concrete.Get_v_b_93934(designDetails.fp_w_xDirection, designDetails.ff_apostrophe_c);
            designDetails.fv_c_xDirection = eq_concrete.Eq_9_5_____(designDetails.fk_d, designDetails.fk_a, designDetails.fv_b_xDirection);
            designDetails.fV_c_xDirection = eq_concrete.Eq_9_4_____(designDetails.fv_c_xDirection, designDetails.fA_cv_xDirection);
            designDetails.fPhi_v_foundations = 0.85f;

            designDetails.fEta_shear_V_footing = Math.Abs(designDetails.fV_asterix_footingdesign_shear) / (designDetails.fPhi_v_foundations * designDetails.fV_c_xDirection);
            fEta_max_Footing = MathF.Max(fEta_max_Footing, designDetails.fEta_shear_V_footing);

            // Punching shear
            float fReactionArea_dimension_x = designDetails.fplateWidth_x;
            float fReactionArea_dimension_y = designDetails.fplateWidth_y;

            float fcriticalPerimeterDimension_x = 2f * Math.Min(designDetails.fd_effective_xDirection, designDetails.fe_x_BasePlateToFootingEdge) + fReactionArea_dimension_x;
            float fcriticalPerimeterDimension_y = 2f * Math.Min(fd_effective_yDirection, designDetails.fe_y_BasePlateToFootingEdge) + fReactionArea_dimension_y; // TODO - Zohladnit ak je stlp blizsie k okraju nez fd

            designDetails.fcriticalPerimeter_b0 = 2 * fcriticalPerimeterDimension_x + 2 * fcriticalPerimeterDimension_y;

            // Ratio of the long side to the short side of the concentrated load
            designDetails.fBeta_c = Math.Max(fReactionArea_dimension_x, fReactionArea_dimension_y) / Math.Min(fReactionArea_dimension_x, fReactionArea_dimension_y); // 12.7
            designDetails.fAlpha_s = 15; // 20 for interior columns, 15 for edge columns, 10 for corner columns TODO - zapracovat identifikaciu stlpa
            designDetails.fd_average = (designDetails.fd_effective_xDirection + fd_effective_yDirection) / 2f;
            designDetails.fk_ds = eq_concrete.Get_k_ds_12732(designDetails.fd_average);

            // Nominal shear stress resisted by the concrete
            designDetails.fv_c_126 = eq_concrete.Eq_12_6____(designDetails.fk_ds, designDetails.fBeta_c, designDetails.ff_apostrophe_c);
            designDetails.fv_c_127 = eq_concrete.Eq_12_7____(designDetails.fk_ds, designDetails.fAlpha_s, designDetails.fd_average, designDetails.fcriticalPerimeter_b0, designDetails.ff_apostrophe_c);
            designDetails.fv_c_128 = eq_concrete.Eq_12_8____(designDetails.fk_ds, designDetails.ff_apostrophe_c);

            designDetails.fv_c_12732 = MathF.Min(designDetails.fv_c_126, designDetails.fv_c_127, designDetails.fv_c_128);

            // 12.7.3.4 Maximum nominal shear stress
            float fv_c_max = 0.5f * MathF.Sqrt(designDetails.ff_apostrophe_c);
            if (designDetails.fv_c_12732 > fv_c_max)
                designDetails.fv_c_12732 = fv_c_max;

            designDetails.fV_c_12731 = eq_concrete.Get_V_c_12731(designDetails.fv_c_12732, designDetails.fcriticalPerimeter_b0, designDetails.fd_average);

            // 12.7.4 Shear reinforcement consisting of bars or wires or stirrups
            float fReinforcementArea_A_v_xDirection = 2 * designDetails.fA_s_tot_Xdirection * fcriticalPerimeterDimension_y / designDetails.fFootingDimension_y; // TODO ?? horna aj spodna vyztuz ak su rovnake
            float fReinforcementArea_A_v_yDirection = 2 * fA_s_tot_Ydirection * fcriticalPerimeterDimension_x / designDetails.fFootingDimension_x;

            float ff_yv = fReinforcementStrength_fy;

            designDetails.fV_s_xDirection = fReinforcementArea_A_v_xDirection * ff_yv * designDetails.fd_effective_xDirection / designDetails.fSpacing_yDirection;
            designDetails.fV_s_yDirection = fReinforcementArea_A_v_yDirection * ff_yv * fd_effective_yDirection / fSpacing_xDirection;

            // 12.7.3.1 Nominal shear strength for punching shear
            designDetails.fV_n_12731_xDirection = eq_concrete.Eq_12_4____(designDetails.fV_s_xDirection, designDetails.fV_c_12731);
            designDetails.fEta_punching_12731_xDirection = eq_concrete.Eq_12_5____(designDetails.fN_asterix_joint_bearing, designDetails.fPhi_v_foundations, designDetails.fV_n_12731_xDirection);
            fEta_max_Footing = MathF.Max(fEta_max_Footing, designDetails.fEta_punching_12731_xDirection);

            designDetails.fV_n_12731_yDirection = eq_concrete.Eq_12_4____(designDetails.fV_s_yDirection, designDetails.fV_c_12731);
            designDetails.fEta_punching_12731_yDirection = eq_concrete.Eq_12_5____(designDetails.fN_asterix_joint_bearing, designDetails.fPhi_v_foundations, designDetails.fV_n_12731_yDirection);
            fEta_max_Footing = MathF.Max(fEta_max_Footing, designDetails.fEta_punching_12731_yDirection);

            // Validation - negative design ratio
            if (//designDetails.fEta_N_t_5423_plate < 0 ||
                //designDetails.fEta_V_yv_3341_plate < 0 ||
                //designDetails.fEta_Mb_plate < 0 ||
                //designDetails.fEta_MainMember < 0 ||
                //designDetails.fEta_Mb_MainMember_oneside_plastic < 0 ||
                //designDetails.fEta_Vb_5424_MainMember < 0 ||
                //designDetails.fEta_V_fv_5425_MainMember < 0 ||
                //designDetails.fEta_V_fv_5425_Plate < 0 ||
                //designDetails.fEta_V_w_5426 < 0 ||
                //designDetails.fEta_N_t_5423_MainMember < 0 ||
                designDetails.fEta_532_1 < 0 ||
                designDetails.fEta_5342 < 0 ||
                designDetails.fEta_5343 < 0 ||
                designDetails.fEta_5351_2 < 0 ||
                designDetails.fEta_5352_1 < 0 ||
                designDetails.fEta_5353 < 0 ||
                designDetails.fEta_17571_group < 0 ||
                designDetails.fEta_17572_group < 0 ||
                designDetails.fEta_17572_single < 0 ||
                designDetails.fEta_17573_group < 0 ||
                designDetails.fEta_17574_single < 0 ||
                designDetails.fEta_17581_group < 0 ||
                designDetails.fEta_17582_group < 0 ||
                designDetails.fEta_17582_single < 0 ||
                designDetails.fEta_17583_group < 0 ||
                designDetails.fEta_17583_single < 0 ||
                designDetails.fEta_17584_group < 0 ||
                designDetails.fEta_17566_group < 0 ||
                fEta_C17566_group < 0 ||
                designDetails.fEta_footing_uplift < 0 ||
                designDetails.fEta_footing_bearing < 0 ||
                designDetails.fEta_bending_M_footing < 0 ||
                designDetails.fEta_MinimumReinforcement_xDirection < 0 ||
                designDetails.fEta_shear_V_footing < 0 ||
                designDetails.fEta_punching_12731_xDirection < 0 ||
                designDetails.fEta_punching_12731_yDirection < 0)
            {
                throw new Exception("Design ratio is invalid!");
            }

            // Validation - infinity design ratio
            if (fEta_max_Footing > 9e+10)
            {
                throw new Exception("Design ratio is invalid!");
            }

            // Store details
            if (bSaveDetails)
                foundation.DesignDetails = designDetails;
        }
    }
}
