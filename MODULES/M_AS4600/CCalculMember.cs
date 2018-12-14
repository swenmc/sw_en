﻿using BaseClasses;
using CRSC;
using MATH;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace M_AS4600
{
    public enum SecShape
    {
        eC,          // Channel (U or C) section
        eZ,          // Z-section
        eC_lip,      // C-section with flange lips
        eI,          // I-section
        eGE          // General
    };

    public enum SecSymmetry
    {
        eDS,         // Doubly symmetric
        eMS_xu,      // Singly symmetric about xu-axis
        eMS_yv,      // Singly symmetric about yv-axis
        eCS,         // Centrally symmetric
        eAS          // Asymmetric
    };

    public enum SecShape_OSC
    {
        eO,      // Open section It.Bredt / It << 1
        eS,      // Solid bar (flat, round, ...)
        eC       // Closed section (one or more closed cells) It.Bredt / It > 0.95
    };

    public enum Cb_option
    {
        eCb_1,         // Cb = 1.0
        eCb_D2112,     // Cb - Equation D2.1.1(2)
        eCb_Tab_D2_1   // Cb - Table D2.1
    };

    public enum SectionShape_Table_D3
    {
        eLC,         // Lipped channel
        eHFC,        // Hollow flange channel
        eTHFB,       // Triangular hollow flange beam
        eRHFB,       // Rectangular hollow flange beam
        eOTHER       // Other shape - general
    };

    public enum LoadPosition_D2_1
    {
        eTensFlange,       // Tension flange
        eShearCentre,      // Shear centre
        eCompFlange        // Compression flange
    };

    public enum LatBracing_D2_1
    {
        eNoBracing,        // No bracing (a = l)
        eHalf,             // One central brace (a = 0.5l)
        eThird             // Third point bracing (a = 0.33l)
    };

    public enum TransStiff_D3
    {
        eD3a_NoTrStiff,    // Un-reinforced webs
        eD3b_HasTrStiff,   // Webs with transverse stiffeners satisfying the requirements of Clause 3.3.8.1  (Todo, check criteria)
        eD3c_StiffFlanges  // Webs restrained at the top and bottom edges by flanges
    };

    public class CCalculMember
    {
        public struct designInternalForces_AS4600
        {
            public float fN;
            public float fN_c;
            public float fN_t;
            public float fV_xu;
            public float fV_yv;
            public float fV_xx;
            public float fV_yy;
            public float fT;
            public float fM_xu;
            public float fM_yv;
            public float fM_xx;
            public float fM_yy;
        }

        designInternalForces_AS4600 sDIF;

        CCrSc_TW cs; // Thin-walled cross-section

        //Cb_option eCb_option = Cb_option.eCb_Tab_D2_1;
        Cb_option eCb_option = Cb_option.eCb_D2112;
        SecShape eCS_shape = SecShape.eC_lip;
        SectionShape_Table_D3 eCS_shape_Tab_D3 = SectionShape_Table_D3.eLC;
        SecSymmetry eCS_sym = SecSymmetry.eMS_xu;
        LoadPosition_D2_1 eLoadPosition = LoadPosition_D2_1.eCompFlange;
        LatBracing_D2_1 eLateralBracing = LatBracing_D2_1.eNoBracing;

        float fl_member;
        float fl_ez;
        float fl_ex;
        float fl_ey;
        float fl_LTB;

        float fM_max;
        float fM_14;
        float fM_24;
        float fM_34;

        float ff_y;
        float ff_u;
        float fE;
        float fG;
        float fNu;

        float fA_g;
        float fA_f_xu;
        float fA_w_yv;
        float fZ_f_xu;
        float fZ_f_yv;
        float fZ_ft_xu;
        float fZ_ft_yv;
        float fS_f_xu;
        float fS_f_yv;
        float fI_xu;
        float fI_yv;
        float fI_yc; // D 2.1.1.3
        float fr_x, fr_y, fr_o1, fx_o, fy_o;

        float fh, fb, ft, ft_w, ft_f;
        float fd_l;
        float fd_1;
        float fa;
        float fb_f;

        float fb_w;
        float fh_x;
        float fh_y;

        float fx_cfl_par, fy_cfl_par;
        public float fA_cfl, fJ_cfl, fI_x_cfl, fI_y_cfl, fI_xy_cfl, fI_w_cfl;

        public float fPhi_t = 0.9f; // Todo

        /*
         For members subject to bending, meeting the geometric requirements of Clause 7.1.2, phib shall be taken as 0.90. 
         For all other members subject to bending, phib specified in Clause 1.6.3(c)(i) applies.
        */
        public float fPhi_b_section = 0.95f; // TODO - Moze sa pouzit len ak je section splna podmienky 7.1.2
        public float fPhi_b = 0.9f;
        public float fPhi_v = 0.9f;
        public float fPhi_c = 0.85f;

        // AS 4600 variables
        public AS_4600 eq = new AS_4600();

        public float fN_t_min;

        public float ff_oc;
        public float flambda_c;
        public float fN_y;
        public float fN_oc;
        public float fN_ce;

        public float ff_oz; //z = x
        public float ff_ox; //x = y
        public float ff_oy; //y = z

        public float ff_ol;
        public float flambda_l;
        public float fN_ol;
        public float fN_cl;

        public float ff_od;
        public float flambda_d;
        public float fN_od;
        public float fN_cd;

        public float fN_c_min;

        public float fM_p_xu, fM_y_xu;
        public float fM_p_yv, fM_y_yv;
        public float fM_be_xu;
        public float fM_bd_xu;
        public float fM_bl_xu;

        public float fC_b;
        public float fM_o_xu;
        public float ff_ol_bend;
        public float fM_ol_xu;
        public float fM_od_xu;
        public float ff_od_bend;
        public float fLambda_l_xu;
        public float fLambda_d_xu;

        public float fM_b_yv;
        
        float fk;
        public float fV_v_yv;
        public float fV_y_yv;
        public float fV_cr_yv;
        public float fLambda_v_yv;

        public float fM_s_xu;
        public float fM_b_xu;
        public float fM_s_xu_f;
        public float fM_s_yv_f;

        public float fEta_Nt = 0.0f;
        public float fEta_721_N = 0.0f;
        public float fEta_722_M_xu = 0.0f;
        public float fEta_723_9_xu_yv = 0.0f;
        public float fEta_723_10_xu = 0.0f;
        public float fEta_723_11_V_yv = 0.0f;
        public float fEta_723_12_xu_yv_10 = 0.0f;
        public float fEta_724 = 0f;
        public float fEta_725_1 = 0f;
        public float fEta_725_2 = 0f;
        public float fEta_max = 0.0f;

        // SLS
        public float fLimitDeflection;
        public float fEta_defl_yu = 0f;
        public float fEta_defl_zv = 0f;
        public float fEta_defl_yy = 0f;
        public float fEta_defl_zz = 0f;
        public float fEta_defl_tot = 0f;

        public CCalculMember(bool bIsDebugging, CMember member, float fM_asterix_xu, float fM_asterix_yv, designBucklingLengthFactors sBucklingLengthFactors, designMomentValuesForCb sMomentValuesForCb)
        {
            designInternalForces sDIF_x_temp;
            sDIF_x_temp.fN = 0.0f;
            sDIF_x_temp.fN_t = 0.0f;
            sDIF_x_temp.fN_c = 0.0f;
            sDIF_x_temp.fV_yu = 0.0f;
            sDIF_x_temp.fV_yy = 0.0f;
            sDIF_x_temp.fV_zv = 0.0f;
            sDIF_x_temp.fV_zz = 0.0f;
            sDIF_x_temp.fT = 0.0f;
            sDIF_x_temp.fM_yu = fM_asterix_xu;
            sDIF_x_temp.fM_yy = fM_asterix_xu;
            sDIF_x_temp.fM_zv = fM_asterix_yv;
            sDIF_x_temp.fM_zz = fM_asterix_yv;

            CalculateDesignRatio(bIsDebugging, sDIF_x_temp, (CCrSc_TW)member.CrScStart, member.FLength, sBucklingLengthFactors, sMomentValuesForCb);

            // Validation
            if (fEta_max > 9e+10)
            {
                throw new Exception("Design ratio is invalid! " + "Member ID: " + member.ID);
            }
        }

        public CCalculMember(bool bIsDebugging, designInternalForces sDIF_x_temp, CMember member, designBucklingLengthFactors sBucklingLengthFactors, designMomentValuesForCb sMomentValuesForCb)
        {
            CalculateDesignRatio(bIsDebugging, sDIF_x_temp, (CCrSc_TW)member.CrScStart, member.FLength, sBucklingLengthFactors, sMomentValuesForCb);

            // Validation
            if (fEta_max > 9e+10)
            {
                throw new Exception("Design ratio is invalid! " + "Member ID: " + member.ID);
            }
        }

        public CCalculMember(bool bIsDebugging, designDeflections sDDeflections_x_temp, CMember member)
        {
            CalculateDesignRatio(bIsDebugging, sDDeflections_x_temp, member.FLength);

            // Validation
            if (fEta_max > 9e+10)
            {
                throw new Exception("Design ratio is invalid! " + "Member ID: " + member.ID);
            }
        }

        public void CalculateDesignRatio(bool bIsDebugging, designDeflections sDDeflections_x_temp, float fL_temp)
        {
            float fLimit = 1f / 360f; // TODO nastavovat podla obsahu kombinacie SLS a typu prvku
            float fLimitDeflection = fL_temp * fLimit;

            // Calculate deflection design ratio
            fEta_defl_yu = Math.Abs(sDDeflections_x_temp.fDelta_yu) / fLimitDeflection;
            fEta_defl_zv = Math.Abs(sDDeflections_x_temp.fDelta_zv) / fLimitDeflection;
            fEta_defl_yy = Math.Abs(sDDeflections_x_temp.fDelta_yy) / fLimitDeflection;
            fEta_defl_zz = Math.Abs(sDDeflections_x_temp.fDelta_zz) / fLimitDeflection;
            fEta_defl_tot = sDDeflections_x_temp.fDelta_tot / fLimitDeflection;

            // Set maximum design ratio
            fEta_max = fEta_defl_tot;
        }

        public void CalculateDesignRatio(bool bIsDebugging, designInternalForces sDIF_x_temp, CCrSc_TW cs_temp, float fL_temp, designBucklingLengthFactors sBucklingLengthFactors,  designMomentValuesForCb sMomentValuesForCb)
        {
            SetDesignInputParameters(sDIF_x_temp, cs_temp, fL_temp,  sBucklingLengthFactors,  sMomentValuesForCb);

            // Design

            // Tension

            fN_t_min = fA_g * ff_y; // Resistance // Todo
            fEta_Nt = sDIF.fN_t / fN_t_min;

            // Compression
            fx_o = (float)cs.D_y_s;
            fy_o = (float)cs.D_z_s;

            // TODO - doplnit vypocet polomerov zotrvacnosti do prierezov // Doriesit jednoznacnost indexov
            cs.i_y_rg = Math.Sqrt(cs.I_y / cs.A_g);
            cs.i_z_rg = Math.Sqrt(cs.I_z / cs.A_g);

            fr_x = (float)cs.i_y_rg;
            fr_y = (float)cs.i_z_rg;

            //float fr_o1 = cs.i_yz_rg;
            fr_o1 = eq.Eq_D111_6__(fr_x, fr_y, fx_o, fy_o);

            ff_ox = eq.Eq_D111_3__(fE, fl_ex, fr_x);
            ff_oy = eq.Eq_D111_3__(fE, fl_ey, fr_y);
            ff_oz = eq.Eq_D111_5__(fG, fE, (float)cs.I_t, (float)cs.I_w, fA_g, fl_ez, fr_o1);

            if (!MathF.d_equal(sDIF.fN_c, 0))
            {
                double fa_CEQ = 0f;
                double fb_CEQ = 0f;
                double fc_CEQ = 0f;
                double fd_CEQ = 0f;

                eq.Eq_D111_9__(ff_oz, ff_ox, ff_oy, fr_o1, fx_o, fy_o, out fa_CEQ, out fb_CEQ, out fc_CEQ, out fd_CEQ);
                CCardanoCubicEQSolver cubic_solver = new CCardanoCubicEQSolver(fa_CEQ, fb_CEQ, fc_CEQ, fd_CEQ);

                double ff_oc_real_1 = cubic_solver.x_min_positive;

                ff_oc = (float)cubic_solver.x_min_positive > 0 ? (float)cubic_solver.x_min_positive : 0f;

                //if(ff_oc <= 0f)
                // Error // TODO - kontrolovat vystup z funkcie ci je to kladne napatie

                // 7.2.1.2.1 Compression members without holes
                fN_y = eq.Eq_7212_5__(fA_g, ff_y);
                fN_oc = eq.Eq_7212_4__(fA_g, ff_oc);
                flambda_c = eq.Eq_7212_3__(fN_y, fN_oc);
                fN_ce = eq.Eq_7212_1__(flambda_c, fN_y);

                // 7.2.1.3 Local buckling
                // 7.2.1.3.1 Compression members without holes

                fk = 4.0f; //see kst

                //ff_ol = eq.Eq_D131____(fk, fE, fNu, ft, fb); // Nacitavat z vlastnosti prierezu
                fN_ol = eq.Eq_7213_4__(fA_g, ff_ol);
                flambda_l = eq.Eq_7213_3__(fN_ce, fN_ol);
                fN_cl = eq.Eq_7213____(flambda_l, fN_ol, fN_ce);

                // 7.2.1.4 Distorsial buckling
                // 7.2.1.4.1 Compression members without holes

                // General channel in compression (picture D2(a))

                bool bUseCalculation_D2_a = false;
                if (bUseCalculation_D2_a) // Je mozne pouzit len v pripade jednoducheho C, inak nacitat z vlastnosti prierezu
                {
                    eq.Calc_CFL_Properties(fb, fd_l, ft, out fA_cfl, out fx_cfl_par, out fy_cfl_par, out fJ_cfl, out fI_x_cfl, out fI_y_cfl, out fI_xy_cfl);
                    // The values of A, J, Ix, Iy, Ixy, Iw are for the compression flange and lip alone.

                    if (cs.IsShapeSolid) // Open Cross-section
                    {
                        ff_od = eq.Eq_D121_1__(fE, fA_cfl, fI_x_cfl, fI_y_cfl, fI_xy_cfl, fJ_cfl, fI_w_cfl, fx_o, fy_o, fh_x, fh_y, fb_w, ft);

                        if (ff_od <= 0f) // TODO - Overit ci moze byt zaporne a dalej sa ma uvazovat abs hodnota ????
                            ff_od = (float)cs.Compression_curve_stress_1; // Temp TODO - osetrit error
                    }
                    else
                        ff_od = ff_y; // Closed cross-section - ignore distorsional buckling
                }

                fN_od = eq.Eq_7214_4__(fA_g, ff_od);
                flambda_d = eq.Eq_7214_3__(fN_ce, fN_ol);
                fN_cd = eq.Eq_7214____(flambda_d, fN_y, fN_od);

                fN_c_min = MathF.Min(fN_ce, fN_cl, fN_cd);

                fEta_721_N = Math.Abs(sDIF.fN_c / fN_c_min);
                fEta_max = MathF.Max(fEta_max, fEta_721_N);
            }
            else
            {
                fEta_721_N = 0;
            }

            // 7.2.2 Design of members subject to bending

            CalculateBendingStrength_722(ELSType.eLS_ULS, sDIF.fM_xu, sDIF.fM_yv);

            fEta_722_M_xu = Math.Abs(sDIF.fM_xu) / (fPhi_b * fM_b_xu); // 7.2.2
            fEta_max = MathF.Max(fEta_max, fEta_722_M_xu);

            // D2.3  Local buckling stresses
            float fk_LB_D23 = 4.0f; //see Todo - value valid for bending

            float ff_ol_D23 = eq.Eq_D131____(fk_LB_D23, fE, fNu, ft, fb); // TODO - overit ci nacitvat z vlastnosti prierezu alebo pocitat

            // 7.2.3 Design of member subject to shear, an combined bending and shear
            fV_y_yv = eq.Eq_723_5___(fA_w_yv, ff_y);
            float fk_v_yv;

            TransStiff_D3 eTrStiff = TransStiff_D3.eD3c_StiffFlanges; // Todo
            float fd_1_straightToTotal = 0.35f; // TODO - urcit pri prierezoch pomer medzi nevystuzenou castou stojiny a celkovou vyskou
            float fd_1_straight = fd_1_straightToTotal * fd_1;

            switch (eTrStiff)
            {
                case TransStiff_D3.eD3b_HasTrStiff:
                    fk_v_yv = eq.Eq_D3_b____(fa, fd_1_straight);
                    break;
                case TransStiff_D3.eD3c_StiffFlanges:
                    fk_v_yv = eq.Eq_D3_c____(eCS_shape_Tab_D3, fb_f, fa, fd_1_straight, ft_f, ft_w);
                    break;
                case TransStiff_D3.eD3a_NoTrStiff:
                default:
                    fk_v_yv = 5.34f; // D3(a)
                    break;
            }

            fV_cr_yv = eq.Eq_D3_1____(fE, fA_w_yv, fk_v_yv, fNu, fd_1_straight, ft);

            fLambda_v_yv = eq.Eq_723_4___(fV_y_yv, fV_cr_yv);

            bool bIsMemberwithTransStiffeners = false;

            if (!bIsMemberwithTransStiffeners)
            {
                // 7.2.3.2 Beams without transverse web stiffeners
                fV_v_yv = eq.Eq_7232____(fV_y_yv, fV_cr_yv, fLambda_v_yv);
            }
            else
            {
                // 7.2.3.3 Beams with transverse web stiffeners
                fV_v_yv = eq.Eq_7233____(fV_y_yv, fV_cr_yv, fLambda_v_yv);
            }

            if (eTrStiff == TransStiff_D3.eD3b_HasTrStiff)
            {
                float fLambda_l_xu = eq.Eq_7223_3__(fM_y_xu, fM_ol_xu);
                float fM_be_xu_temp = eq.Eq_7223____(fM_ol_xu, fM_y_xu, fLambda_l_xu); // Mbe acc. to Clause 7.2.2.3 with Mbe = My, Mbd acc. to Clause 7.2.2.4
                fM_s_xu = MathF.Min(fM_be_xu_temp, fM_bd_xu);
            }
            else
            {
                float fLambda_l_xu = eq.Eq_7223_3__(fM_y_xu, fM_ol_xu);
                fM_s_xu = eq.Eq_7223____(fM_ol_xu, fM_y_xu, fLambda_l_xu); // TODO
            }

            float fEta_M_xu, fEta_V_yv;
            float fEta_723_12_xu_yv_13;
            float fM_b_xu_drv;
            float fV_v_yv_drv;

            // 7.2.3.5 Combined bending and shear
            eq.Eq_723_9___(sDIF.fM_xu, fPhi_b_section, fM_s_xu, sDIF.fV_yv, fPhi_v, fV_v_yv, out fEta_M_xu, out fEta_V_yv, out fEta_723_9_xu_yv);
            fEta_max = MathF.Max(fEta_max, fEta_723_9_xu_yv);

            if (eTrStiff == TransStiff_D3.eD3b_HasTrStiff)
            {
                eq.Eq_723_10__(sDIF.fM_xu, fPhi_b, fM_b_xu, out fM_b_xu_drv, out fEta_723_10_xu);
                fEta_max = MathF.Max(fEta_max, fEta_723_10_xu);
            }

            // Shear
            eq.Eq_723_11__(sDIF.fV_yv, fPhi_v, fV_v_yv, out fV_v_yv_drv, out fEta_723_11_V_yv);
            fEta_max = MathF.Max(fEta_max, fEta_723_11_V_yv);

            if ((Math.Abs(sDIF.fM_xu) / (fPhi_b_section * fM_s_xu)) > 0.5f && (Math.Abs(sDIF.fV_yv) / (fPhi_v * fV_v_yv)) > 0.7f)
            {
                eq.Eq_723_12__(sDIF.fM_xu, fPhi_b_section, fM_s_xu, sDIF.fV_yv, fPhi_v, fV_v_yv, out fEta_M_xu, out fEta_V_yv, out fEta_723_12_xu_yv_13, out fEta_723_12_xu_yv_10);
                fEta_max = MathF.Max(fEta_max, fEta_723_12_xu_yv_10);
            }

            float fEta_N_724, fEta_Mxu_724, fEta_Myv_724; 
            float fEta_N_725_1, fEta_Mxu_725_1, fEta_Myv_725_1;
            float fEta_N_725_2, fEta_Mxu_725_2, fEta_Myv_725_2;

            fM_s_xu_f = eq.Eq_725_3___(fZ_ft_xu, ff_y);
            fM_s_yv_f = eq.Eq_725_3___(fZ_ft_yv, ff_y);

            if (!MathF.d_equal(sDIF.fN, 0.0f)) // Some axial force exists
            {
                if (sDIF.fN < 0.0f) // Compression
                {
                    // 7.2.4 Design of members subject to combined axial compression and bending
                    eq.Eq_724_____(fPhi_c, fPhi_b, sDIF.fN_c, fN_c_min, sDIF.fM_xu, fM_b_xu, sDIF.fM_yv, fM_b_yv, out fEta_N_724, out fEta_Mxu_724, out fEta_Myv_724, out fEta_724);
                    fEta_max = MathF.Max(fEta_max, fEta_724);
                }
                else
                {
                    // 7.2.5 Design of members subject to combined axial tension and bending
                    eq.Eq_725_1___(fPhi_t, fPhi_b, sDIF.fN_t, fN_t_min, sDIF.fM_xu, fM_b_xu, sDIF.fM_yv, fM_b_yv, out fEta_N_725_1, out fEta_Mxu_725_1, out fEta_Myv_725_1, out fEta_725_1);
                    fEta_max = MathF.Max(fEta_max, fEta_725_1);

                    eq.Eq_725_2___(fPhi_t, fPhi_b, sDIF.fN_t, fN_t_min, sDIF.fM_xu, fM_s_xu_f, sDIF.fM_yv, fM_s_yv_f, out fEta_N_725_2, out fEta_Mxu_725_2, out fEta_Myv_725_2, out fEta_725_2);
                    fEta_max = MathF.Max(fEta_max, fEta_725_2);
                }
            }

            // Validation - negative design ratio
            if (fEta_721_N < 0 ||
                fEta_722_M_xu < 0 ||
                fEta_723_9_xu_yv < 0 ||
                fEta_723_10_xu < 0 ||
                fEta_723_11_V_yv < 0 ||
                fEta_723_12_xu_yv_10 < 0 ||
                fEta_724 < 0 ||
                fEta_725_1 < 0 ||
                fEta_725_2 < 0)
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
                System.Windows.MessageBox.Show("Calculation finished.\n"
                              + "Design Ratio η = " + Math.Round(fEta_721_N, iNumberOfDecimalPlaces) + " [-]" + "\t" + " 7.2.1" + "\n"
                              + "Design Ratio η = " + Math.Round(fEta_722_M_xu, iNumberOfDecimalPlaces) + " [-]" + "\t" + " 7.2.2" + "\n"
                              + "Design Ratio η = " + Math.Round(fEta_723_9_xu_yv, iNumberOfDecimalPlaces) + " [-]" + "\t" + " 7.2.3(9)" + "\n"
                              + "Design Ratio η = " + Math.Round(fEta_723_10_xu, iNumberOfDecimalPlaces) + " [-]" + "\t" + " 7.2.3(10)" + "\n"
                              + "Design Ratio η = " + Math.Round(fEta_723_11_V_yv, iNumberOfDecimalPlaces) + " [-]" + "\t" + " 7.2.3(11)" + "\n"
                              + "Design Ratio η = " + Math.Round(fEta_723_12_xu_yv_10, iNumberOfDecimalPlaces) + " [-]" + "\t" + " 7.2.3(12)" + "\n"
                              + "Design Ratio η = " + Math.Round(fEta_724, iNumberOfDecimalPlaces) + " [-]" + "\t" + " 7.2.4" + "\n"
                              + "Design Ratio η = " + Math.Round(fEta_725_1, iNumberOfDecimalPlaces) + " [-]" + "\t" + " 7.2.5(1)" + "\n"
                              + "Design Ratio η = " + Math.Round(fEta_725_2, iNumberOfDecimalPlaces) + " [-]" + "\t" + " 7.2.5(2)" + "\n"
                              + "Design Ratio η max = " + Math.Round(fEta_max, iNumberOfDecimalPlaces) + " [-]");
        }

        void SetDesignInputParameters(designInternalForces sDIF_x_temp, CCrSc_TW cs_temp, float fL_temp, designBucklingLengthFactors sBucklingLengthFactors, designMomentValuesForCb sMomentValuesForCb)
        {
            cs = cs_temp;

            // Set design internal forces according AS 4600 symbols of axes
            sDIF.fN = sDIF_x_temp.fN;
            sDIF.fN_c = sDIF_x_temp.fN_c;
            sDIF.fN_t = sDIF_x_temp.fN_t;
            sDIF.fV_xu = sDIF_x_temp.fV_yu;
            sDIF.fV_yv = sDIF_x_temp.fV_zv;
            sDIF.fV_xx = sDIF_x_temp.fV_yy;
            sDIF.fV_yy = sDIF_x_temp.fV_zz;
            sDIF.fT = sDIF_x_temp.fT;
            sDIF.fM_xu = sDIF_x_temp.fM_yu;
            sDIF.fM_yv = sDIF_x_temp.fM_zv;
            sDIF.fM_xx = sDIF_x_temp.fM_yy;
            sDIF.fM_yv = sDIF_x_temp.fM_zz;

            // Set cross-section properties
            fh = (float)cs.h;
            fb = (float)cs.b;
            ft = (float)cs.t_min;
            fA_g = (float)cs.A_g;
            fZ_f_xu = (float)cs.W_y_el; // Elastic section modulus
            fZ_f_yv = (float)cs.W_z_el;
            fS_f_xu = (float)cs.W_y_pl; // Plastic section modulus
            fS_f_yv = (float)cs.W_z_pl;

            // TODO - pridat plasticke moduly do parametrov prierezu
            if (fS_f_xu <= 0)
                fS_f_xu = 1.001f * fZ_f_xu; // Konzervativne + len 0.1%
            if (fS_f_yv <= 0)
                fS_f_yv = 1.001f * fZ_f_yv; // Konzervativne + len 0.1%

            fA_f_xu = (float)cs.A_vy;
            fA_w_yv = (float)cs.A_vz;

            // TODO - pridat smykove plochy do parametrov prierezu (malo by sa jednat o celkovu smykovu plochy, obe pasnice resp stojiny atd)
            if (fA_f_xu <= 0)
                fA_f_xu = 2 * ft * fb;
            if (fA_w_yv <= 0)
                fA_w_yv = ft * fh;

            fZ_ft_xu = fZ_f_xu; //Todo // section modulus of the full unreduced section for the extreme tension fibre about the appropriate axis
            fZ_ft_yv = fZ_f_yv; //Todo // section modulus of the full unreduced section for the extreme tension fibre about the appropriate axis
            fI_xu = (float)cs.I_y;
            fI_yv = (float)cs.I_z;

            ff_ol = cs.fol_c > 0 ? (float)cs.fol_c : 1e+9f; // Hodnota z databazy alebo 1000 MPa, ak nie je definovane
            ff_od = cs.fod_c > 0 ? (float)cs.fod_c : 1e+9f;
            ff_ol_bend = cs.fol_b > 0 ? (float)cs.fol_b : 1e+9f;
            ff_od_bend = cs.fod_b > 0 ? (float)cs.fod_b : 1e+9f;

            fb_w = fb; // ??? // TODO - doriesit
            fh_x = fb;
            fh_y = 0.02f; // No lip

            // Set material properties
            ff_y = cs.m_Mat.Get_f_yk_by_thickness(ft);
            ff_u = cs.m_Mat.Get_f_uk_by_thickness(ft);
            fE = cs.m_Mat.m_fE;
            fG = cs.m_Mat.m_fG;
            fNu = cs.m_Mat.m_fNu;

            fI_yc = 54564f;
            fd_l = 0.005f; // Overall stiffener dimension (or overall depth of lip) Figure 2.4.2(a) Clause 2.4 / TABLE 2.4.2 / D1.2.1.2 Simple lipped channels in compression
            fd_1 = fh - 2 * ft; // Web Depth Web-flange juncture // Todo // depth of the flat portion of a web

            fl_member = fL_temp;
            fl_ex = sBucklingLengthFactors.fBeta_x_FB_fl_ex * fl_member; // Flexural buckling - major axis (Nc)
            fl_ey = sBucklingLengthFactors.fBeta_y_FB_fl_ey * fl_member; // Flexural buckling - minor axis (Nc)
            fl_ez = sBucklingLengthFactors.fBeta_z_TB_TFB_l_ez * fl_member; // Torsional / Torsional-flexural buckling (Nc)

            fl_LTB = sBucklingLengthFactors.fBeta_LTB_fl_LTB * fl_member; // Laterla-torsional buckling (Mx)

            fM_max = sMomentValuesForCb.fM_max;
            fM_14 = sMomentValuesForCb.fM_14;
            fM_24 = sMomentValuesForCb.fM_24;
            fM_34 = sMomentValuesForCb.fM_34;

            fa = fl_member; // Todo
            ft_w = ft_f = ft; // TODO
            fb_f = fb; // TODO
        }

        public void CalculateBendingStrength_722(ELSType eLSType, float fM_asterix_xu, float fM_asterix_yv)
        {
            // 7.2.2 Design of members subject to bending
            fM_p_xu = eq.Eq_7222_6__(fS_f_xu, ff_y);
            fM_y_xu = eq.Eq_7222_4__(fZ_f_xu, ff_y);

            fM_p_yv = eq.Eq_7222_6__(fS_f_yv, ff_y);
            fM_y_yv = eq.Eq_7222_4__(fZ_f_yv, ff_y);

            if (eLSType == ELSType.eLS_SLS)
            {
                // M - moment due to nominal loads on member to be considered <= My
                // SLS
                fM_p_xu = Math.Min(Math.Abs(fM_asterix_xu), fM_y_xu);
                fM_y_xu = Math.Min(Math.Abs(fM_asterix_xu), fM_y_xu);

                fM_p_yv = Math.Min(Math.Abs(fM_asterix_yv), fM_y_yv);
                fM_y_yv = Math.Min(Math.Abs(fM_asterix_yv), fM_y_yv);
            }

            // TODO - doplnit plasticke moduly prierezu do databazy
            if (fM_p_xu <= 0)
                fM_p_xu = fM_y_xu;

            if (fM_p_yv <= 0)
                fM_p_yv = fM_y_yv;

            // Bending about xu-axis
            // Default values (used for design ratio in case that fM_xu = 0)
            fM_o_xu = fM_y_xu;
            fM_ol_xu = fM_y_xu;
            fM_od_xu = fM_y_xu;

            fM_be_xu = fM_y_xu; // Default value
            fM_bd_xu = fM_y_xu;
            fM_bl_xu = fM_y_xu;

            // Bending about yv-axis
            // Default values (used for design ratio in case that fM_yv = 0)
            float fM_o_yv = fM_y_yv;
            float fM_ol_yv = fM_y_yv;
            float fM_od_yv = fM_y_yv;

            float fM_be_yv = fM_y_yv; // Default value
            float fM_bd_yv = fM_y_yv;
            float fM_bl_yv = fM_y_yv;

            // TODO - skontrolovat a overit vypocet unosnosti pre osu y/v
            float fM_b_yv = MathF.Min(fM_be_yv, MathF.Min(fM_bl_yv, fM_bd_yv)); // Design resistance value 7.2.2

            // 7.2.2.2.2 Lateral-torsional buckling

            if (!MathF.d_equal(sDIF.fM_xu, 0.0f))
            {
                switch (eCb_option)
                {
                    case Cb_option.eCb_D2112:
                        fC_b = eq.Eq_D211_2__(fM_max, fM_14, fM_24, fM_34);
                        break;
                    case Cb_option.eCb_Tab_D2_1:
                        fC_b = eq.Get_Cb_Tab_D2_1(eLoadPosition, eLateralBracing);
                        break;
                    case Cb_option.eCb_1:
                    default:
                        fC_b = 1.0f;
                        break;
                }

                SecShape_OSC eCS_type_OSC = SecShape_OSC.eO; // TODO - dopracovat identifikaciu tvaru do databazy

                if (cs.I_t > 1e-8) // TODO - upravit a nacitat z databazy
                    eCS_type_OSC = SecShape_OSC.eC;

                if (eCS_type_OSC == SecShape_OSC.eO) // Open
                {
                    if (eCS_sym == SecSymmetry.eDS || eCS_sym == SecSymmetry.eMS_xu || eCS_shape == SecShape.eZ)
                        fM_o_xu = eq.Eq_D211_1__(fC_b, fA_g, fr_o1, ff_oy, ff_oz); // D 2.1.1.2(a)
                                                                                   //else if()
                                                                                   // D 2.1.1.2(b)
                    else if (eCS_sym == SecSymmetry.eCS && eCS_shape == SecShape.eZ)
                        fM_o_xu = eq.Eq_D211_7__(fE, fC_b, fh, fI_yc, fl_LTB); // D 2.1.1.3 - Eq. D 2.1.1(7)
                    else
                        fM_o_xu = 0.0f; // Neni definovano, zadat manualne
                }
                else if (eCS_type_OSC == SecShape_OSC.eC) // Closed
                {
                    fM_o_xu = (float)((fC_b * MathF.fPI / fl_LTB) * Math.Sqrt(fE * fG * cs.I_t * cs.I_z));
                }
                else
                {
                    fM_o_xu = 0.0f; // Neni definovano, zadat manualne
                }

                fM_be_xu = eq.Eq_7222____(fM_o_xu, fM_y_xu);

                // Inelastic reserve
                if (fM_o_xu > 2.78f * fM_y_xu)
                    fM_be_xu = eq.Eq_7222_5__(fM_p_xu, fM_y_xu, fM_o_xu);

                // 7.2.2.3 Local buckling

                float fk_bend = 4.0f; // Todo - factors for bending
                // ft, fb - asi pre kazdy element -> rozhoduje najstihlejsi ???

                //ff_ol_bend = eq.Eq_D131____(fk_bend, fE, fNu, ft, fb); // Nacitavat z vlastnosti prierezu
                fM_ol_xu = eq.Eq_7223_4__(fZ_f_xu, ff_ol_bend);
                fLambda_l_xu = eq.Eq_7223_3__(fM_be_xu, fM_ol_xu);
                fM_bl_xu = eq.Eq_7223____(fM_ol_xu, fM_be_xu, fLambda_l_xu);

                // Inelastic reserve
                if (fLambda_l_xu <= 0.776f && fM_be_xu > fM_y_xu)
                {
                    float fC_yl_xu = eq.Eq_7223_8__(fLambda_l_xu);

                    bool bIsCompr = true; // TEMP Todo

                    if (bIsCompr)
                    {
                        // Sections symmetric about the axis of bending or sections with first yield in compression
                        fM_bl_xu = eq.Eq_7223_5__(fM_p_xu, fM_y_xu, fC_yl_xu);
                    }
                    else
                    {
                        // Sections with first yield in tension
                        float fM_yc_xu = fM_y_xu; // Conservative
                        float fC_yt_xu = 3.0f;
                        float fM_yt3_xu = eq.Eq_7223_9__(fM_p_xu, fM_y_xu, fC_yt_xu);
                        fM_bl_xu = eq.Eq_7223_6__(fM_p_xu, fM_yc_xu, fM_yt3_xu, fC_yl_xu);
                    }
                }

                // 7.2.2.4 Distorsional buckling
                // 7.2.2.4.2 Beams without holes

                bool bUseCalculation_DistBuckling_Bending = false;
                if (bUseCalculation_DistBuckling_Bending) // Je mozne pouzit len v pripade jednoducheho tvaru, inak nacitat z vlastnosti prierezu
                {
                    if (cs.IsShapeSolid) // Open Cross-section
                    {
                        ff_od_bend = eq.Eq_D121_1_DB(fE, fA_cfl, fI_x_cfl, fI_y_cfl, fI_xy_cfl, fJ_cfl, fI_w_cfl, fx_o, fy_o, fh_x, fh_y, fb_f, fb_w, ft);
                    }
                    else
                        ff_od_bend = ff_y; // Closed cross-section - ignore distorsional buckling
                }

                fM_od_xu = eq.Eq_7224_4__(fZ_f_xu, ff_od_bend);
                fLambda_d_xu = eq.Eq_7224_3__(fM_y_xu, fM_od_xu);
                fM_bd_xu = eq.Eq_7224____(fM_y_xu, fM_od_xu, fLambda_d_xu);
            }

            fM_b_xu = MathF.Min(fM_be_xu, MathF.Min(fM_bl_xu, fM_bd_xu)); // Design resistance value 7.2.2
        }

        List<string> zoznamMenuNazvy = new List<string>(4);          // premenne zobrazene v tabulke
        List<string> zoznamMenuHodnoty = new List<string>(4);        // hodnoty danych premennych
        List<string> zoznamMenuJednotky = new List<string>(4);       // jednotky danych premennych

        public void DisplayDesignResultsInGridView(ELSType eCombinationType, DataGrid dataGrid)
        {
            DeleteLists();

            if (eCombinationType == ELSType.eLS_ULS)
                SetResultsDetailsFor_ULS(this);
            else
                SetResultsDetailsFor_SLS(this);
            
            // Create Table
            DataTable table = new DataTable("Table");
            table.Columns.Add("Symbol", typeof(String));
            table.Columns.Add("Value", typeof(String));
            table.Columns.Add("Unit", typeof(String));

            table.Columns.Add("Symbol1", typeof(String));
            table.Columns.Add("Value1", typeof(String));
            table.Columns.Add("Unit1", typeof(String));

            table.Columns.Add("Symbol2", typeof(String));
            table.Columns.Add("Value2", typeof(String));
            table.Columns.Add("Unit2", typeof(String));

            // Set Column Caption
            table.Columns["Symbol1"].Caption = table.Columns["Symbol2"].Caption = "Symbol";
            table.Columns["Value1"].Caption = table.Columns["Value2"].Caption = "Value";
            table.Columns["Unit1"].Caption = table.Columns["Unit2"].Caption = "Unit";

            // Create Dataset
            DataSet ds = new DataSet();
            // Add Table to Dataset
            ds.Tables.Add(table);

            for (int i = 0; i < zoznamMenuNazvy.Count; i++)
            {
                DataRow row = table.NewRow();
                try
                {
                    row["Symbol"] = zoznamMenuNazvy[i];
                    row["Value"] = zoznamMenuHodnoty[i];
                    row["Unit"] = zoznamMenuJednotky[i];
                    i++;
                    if (i >= zoznamMenuNazvy.Count) break;
                    row["Symbol1"] = zoznamMenuNazvy[i];
                    row["Value1"] = zoznamMenuHodnoty[i];
                    row["Unit1"] = zoznamMenuJednotky[i];
                    i++;
                    if (i >= zoznamMenuNazvy.Count) break;
                    row["Symbol2"] = zoznamMenuNazvy[i];
                    row["Value2"] = zoznamMenuHodnoty[i];
                    row["Unit2"] = zoznamMenuJednotky[i];
                }
                catch (ArgumentOutOfRangeException) { }
                table.Rows.Add(row);
            }
            
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "Symbol", Binding = new Binding("Symbol") });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "Value", Binding = new Binding("Value") });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "Unit", Binding = new Binding("Unit") });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "Symbol", Binding = new Binding("Symbol1") });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "Value", Binding = new Binding("Value1") });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "Unit", Binding = new Binding("Unit1") });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "Symbol", Binding = new Binding("Symbol2") });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "Value", Binding = new Binding("Value2") });
            dataGrid.Columns.Add(new DataGridTextColumn { Header = "Unit", Binding = new Binding("Unit2") });
            dataGrid.DataContext = ds.Tables[0];  //draw the table to datagridview

            // Styling datagrid
            //TODO - ponastavovat rozne Style property

            Style alignRight = new Style();
            alignRight.Setters.Add(new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Right));

            Style alignLeft = new Style();
            alignLeft.Setters.Add(new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Left));
            //alignLeft.Setters.Add(new Setter(DataGridCell.WidthProperty, DataGridLength.SizeToCells));
            Style alignCenter = new Style();
            alignCenter.Setters.Add(new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center));
            alignCenter.Setters.Add(new Setter(DataGridCell.BackgroundProperty, new SolidColorBrush(Colors.AliceBlue)));
            //alignCenter.Setters.Add(new Setter(DataGridCell.WidthProperty, DataGridLength.SizeToCells));

            dataGrid.Columns[0].CellStyle = alignLeft;
            dataGrid.Columns[1].CellStyle = alignRight;
            dataGrid.Columns[2].CellStyle = alignLeft;
            dataGrid.Columns[3].CellStyle = alignLeft;
            dataGrid.Columns[4].CellStyle = alignRight;
            dataGrid.Columns[5].CellStyle = alignLeft;
            dataGrid.Columns[6].CellStyle = alignLeft;
            dataGrid.Columns[7].CellStyle = alignRight;
            dataGrid.Columns[8].CellStyle = alignLeft;

            // Set Column Header
            dataGrid.Columns[0].Header = dataGrid.Columns[3].Header = dataGrid.Columns[6].Header = "Symbol";
            dataGrid.Columns[1].Header = dataGrid.Columns[4].Header = dataGrid.Columns[7].Header = "Value";
            dataGrid.Columns[2].Header = dataGrid.Columns[5].Header = dataGrid.Columns[8].Header = "Unit";

            //Set Column Width
            //Results_GridView.Columns[0].Width = Results_GridView.Columns[3].Width = Results_GridView.Columns[6].Width = 117;
            //Results_GridView.Columns[1].Width = Results_GridView.Columns[4].Width = Results_GridView.Columns[7].Width = 90;
            //Results_GridView.Columns[2].Width = Results_GridView.Columns[5].Width = Results_GridView.Columns[8].Width = 90;
        }


        private void DeleteLists()
        {
            // Deleting lists for updating actual values
            zoznamMenuNazvy.Clear();
            zoznamMenuHodnoty.Clear();
            zoznamMenuJednotky.Clear();
        }

        private void SetResultsDetailsFor_ULS(CCalculMember obj_CalcDesign)
        {
            float fUnitFactor_Force = 0.001f;     // from N to kN
            float fUnitFactor_Moment = 0.001f;    // from Nm to kNm
            float fUnitFactor_Stress = 0.000001f; // from Pa to MPa

            int iNumberOfDecimalPlaces = 3;


            // Display results in datagrid
            // AS 4600 output variables
            // Tension
            if (obj_CalcDesign.fEta_Nt > 0)
            {
                zoznamMenuNazvy.Add("φ t");
                zoznamMenuHodnoty.Add(obj_CalcDesign.fPhi_t.ToString());
                zoznamMenuJednotky.Add("[-]");

                zoznamMenuNazvy.Add("N t,min");
                zoznamMenuHodnoty.Add(Math.Round(obj_CalcDesign.fN_t_min * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                zoznamMenuJednotky.Add("[N]");

                zoznamMenuNazvy.Add("η Nt");
                zoznamMenuHodnoty.Add(Math.Round(obj_CalcDesign.fEta_Nt, iNumberOfDecimalPlaces).ToString());
                zoznamMenuJednotky.Add("[-]");
            }

            // Compression
            // Global Buckling
            if (obj_CalcDesign.fEta_721_N > 0 || obj_CalcDesign.fEta_722_M_xu > 0)
            {
                zoznamMenuNazvy.Add("f ox");
                zoznamMenuHodnoty.Add(Math.Round(obj_CalcDesign.ff_ox * fUnitFactor_Stress, iNumberOfDecimalPlaces).ToString());
                zoznamMenuJednotky.Add("[Pa]");

                zoznamMenuNazvy.Add("f oy");
                zoznamMenuHodnoty.Add(Math.Round(obj_CalcDesign.ff_oy * fUnitFactor_Stress, iNumberOfDecimalPlaces).ToString());
                zoznamMenuJednotky.Add("[Pa]");

                zoznamMenuNazvy.Add("f oz");
                zoznamMenuHodnoty.Add(Math.Round(obj_CalcDesign.ff_oz * fUnitFactor_Stress, iNumberOfDecimalPlaces).ToString());
                zoznamMenuJednotky.Add("[Pa]");

                if (obj_CalcDesign.fEta_721_N > 0)
                {
                    zoznamMenuNazvy.Add("f oc");
                    zoznamMenuHodnoty.Add(Math.Round(obj_CalcDesign.ff_oc * fUnitFactor_Stress, iNumberOfDecimalPlaces).ToString());
                    zoznamMenuJednotky.Add("[Pa]");

                    zoznamMenuNazvy.Add("λ c");
                    zoznamMenuHodnoty.Add(Math.Round(obj_CalcDesign.flambda_c, iNumberOfDecimalPlaces).ToString());
                    zoznamMenuJednotky.Add("[-]");

                    zoznamMenuNazvy.Add("N y");
                    zoznamMenuHodnoty.Add(Math.Round(obj_CalcDesign.fN_y * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                    zoznamMenuJednotky.Add("[N]");

                    zoznamMenuNazvy.Add("N oc");
                    zoznamMenuHodnoty.Add(Math.Round(obj_CalcDesign.fN_oc * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                    zoznamMenuJednotky.Add("[N]");

                    zoznamMenuNazvy.Add("N ce");
                    zoznamMenuHodnoty.Add(Math.Round(obj_CalcDesign.fN_ce * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                    zoznamMenuJednotky.Add("[N]");

                    // Local Buckling
                    zoznamMenuNazvy.Add("f ol");
                    zoznamMenuHodnoty.Add(Math.Round(obj_CalcDesign.ff_oy * fUnitFactor_Stress, iNumberOfDecimalPlaces).ToString());
                    zoznamMenuJednotky.Add("[Pa]");

                    zoznamMenuNazvy.Add("λ l");
                    zoznamMenuHodnoty.Add(Math.Round(obj_CalcDesign.flambda_l, iNumberOfDecimalPlaces).ToString());
                    zoznamMenuJednotky.Add("[-]");

                    zoznamMenuNazvy.Add("N ol");
                    zoznamMenuHodnoty.Add(Math.Round(obj_CalcDesign.fN_ol * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                    zoznamMenuJednotky.Add("[N]");

                    zoznamMenuNazvy.Add("N cl");
                    zoznamMenuHodnoty.Add(Math.Round(obj_CalcDesign.fN_cl * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                    zoznamMenuJednotky.Add("[N]");

                    // Distorsial Buckling
                    zoznamMenuNazvy.Add("f od");
                    zoznamMenuHodnoty.Add(Math.Round(obj_CalcDesign.ff_od * fUnitFactor_Stress, iNumberOfDecimalPlaces).ToString());
                    zoznamMenuJednotky.Add("[Pa]");

                    zoznamMenuNazvy.Add("λ d");
                    zoznamMenuHodnoty.Add(Math.Round(obj_CalcDesign.flambda_d, iNumberOfDecimalPlaces).ToString());
                    zoznamMenuJednotky.Add("[-]");

                    zoznamMenuNazvy.Add("N od");
                    zoznamMenuHodnoty.Add(Math.Round(obj_CalcDesign.fN_od * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                    zoznamMenuJednotky.Add("[N]");

                    zoznamMenuNazvy.Add("N cd");
                    zoznamMenuHodnoty.Add(Math.Round(obj_CalcDesign.fN_cd * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                    zoznamMenuJednotky.Add("[N]");

                    zoznamMenuNazvy.Add("N c,min");
                    zoznamMenuHodnoty.Add(Math.Round(obj_CalcDesign.fN_c_min * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                    zoznamMenuJednotky.Add("[N]");

                    zoznamMenuNazvy.Add("φ c");
                    zoznamMenuHodnoty.Add(Math.Round(obj_CalcDesign.fPhi_c, iNumberOfDecimalPlaces).ToString());
                    zoznamMenuJednotky.Add("[-]");

                    zoznamMenuNazvy.Add("η Nc");
                    zoznamMenuHodnoty.Add(Math.Round(obj_CalcDesign.fEta_721_N, iNumberOfDecimalPlaces).ToString());
                    zoznamMenuJednotky.Add("[-]");
                }
            }

            // Bending

            zoznamMenuNazvy.Add("M p,x");
            zoznamMenuHodnoty.Add(Math.Round(obj_CalcDesign.fM_p_xu * fUnitFactor_Moment, iNumberOfDecimalPlaces).ToString());
            zoznamMenuJednotky.Add("[Nm]");

            zoznamMenuNazvy.Add("M y,x");
            zoznamMenuHodnoty.Add(Math.Round(obj_CalcDesign.fM_y_xu * fUnitFactor_Moment, iNumberOfDecimalPlaces).ToString());
            zoznamMenuJednotky.Add("[Nm]");

            zoznamMenuNazvy.Add("M p,y");
            zoznamMenuHodnoty.Add(Math.Round(obj_CalcDesign.fM_p_yv * fUnitFactor_Moment, iNumberOfDecimalPlaces).ToString());
            zoznamMenuJednotky.Add("[Nm]");

            zoznamMenuNazvy.Add("M y,y");
            zoznamMenuHodnoty.Add(Math.Round(obj_CalcDesign.fM_y_yv * fUnitFactor_Moment, iNumberOfDecimalPlaces).ToString());
            zoznamMenuJednotky.Add("[Nm]");

            if (obj_CalcDesign.fEta_722_M_xu > 0)
            {
                // Bending about x/u axis
                zoznamMenuNazvy.Add("C b");
                zoznamMenuHodnoty.Add(Math.Round(obj_CalcDesign.fC_b, iNumberOfDecimalPlaces).ToString());
                zoznamMenuJednotky.Add("[-]");

                zoznamMenuNazvy.Add("M o,x");
                zoznamMenuHodnoty.Add(Math.Round(obj_CalcDesign.fM_o_xu * fUnitFactor_Moment, iNumberOfDecimalPlaces).ToString());
                zoznamMenuJednotky.Add("[Nm]");

                zoznamMenuNazvy.Add("M be,x");
                zoznamMenuHodnoty.Add(Math.Round(obj_CalcDesign.fM_be_xu * fUnitFactor_Moment, iNumberOfDecimalPlaces).ToString());
                zoznamMenuJednotky.Add("[Nm]");

                // Local Buckling
                zoznamMenuNazvy.Add("f ol,x");
                zoznamMenuHodnoty.Add(Math.Round(obj_CalcDesign.ff_ol_bend * fUnitFactor_Stress, iNumberOfDecimalPlaces).ToString());
                zoznamMenuJednotky.Add("[Pa]");

                zoznamMenuNazvy.Add("M ol,x");
                zoznamMenuHodnoty.Add(Math.Round(obj_CalcDesign.fM_ol_xu * fUnitFactor_Moment, iNumberOfDecimalPlaces).ToString());
                zoznamMenuJednotky.Add("[Nm]");

                zoznamMenuNazvy.Add("λ l,x");
                zoznamMenuHodnoty.Add(Math.Round(obj_CalcDesign.fLambda_l_xu, iNumberOfDecimalPlaces).ToString());
                zoznamMenuJednotky.Add("[-]");

                zoznamMenuNazvy.Add("M bl,x");
                zoznamMenuHodnoty.Add(Math.Round(obj_CalcDesign.fM_bl_xu * fUnitFactor_Moment, iNumberOfDecimalPlaces).ToString());
                zoznamMenuJednotky.Add("[Nm]");

                // Distrosial buckling
                zoznamMenuNazvy.Add("f od,x");
                zoznamMenuHodnoty.Add(Math.Round(obj_CalcDesign.ff_od_bend * fUnitFactor_Stress, iNumberOfDecimalPlaces).ToString());
                zoznamMenuJednotky.Add("[Pa]");

                zoznamMenuNazvy.Add("M od,x");
                zoznamMenuHodnoty.Add(Math.Round(obj_CalcDesign.fM_od_xu * fUnitFactor_Moment, iNumberOfDecimalPlaces).ToString());
                zoznamMenuJednotky.Add("[Nm]");

                zoznamMenuNazvy.Add("λ d,x");
                zoznamMenuHodnoty.Add(Math.Round(obj_CalcDesign.fLambda_d_xu, iNumberOfDecimalPlaces).ToString());
                zoznamMenuJednotky.Add("[-]");

                zoznamMenuNazvy.Add("M bd,x");
                zoznamMenuHodnoty.Add(Math.Round(obj_CalcDesign.fM_bd_xu * fUnitFactor_Moment, iNumberOfDecimalPlaces).ToString());
                zoznamMenuJednotky.Add("[Nm]");

                zoznamMenuNazvy.Add("φ b");
                zoznamMenuHodnoty.Add(Math.Round(obj_CalcDesign.fPhi_b, iNumberOfDecimalPlaces).ToString());
                zoznamMenuJednotky.Add("[-]");

                zoznamMenuNazvy.Add("η");
                zoznamMenuHodnoty.Add(Math.Round(obj_CalcDesign.fEta_722_M_xu, iNumberOfDecimalPlaces).ToString());
                zoznamMenuJednotky.Add("[-]");
            }

            // Shear
            if (obj_CalcDesign.fEta_723_9_xu_yv > 0)
            {
                zoznamMenuNazvy.Add("V y,y");
                zoznamMenuHodnoty.Add(Math.Round(obj_CalcDesign.fV_y_yv * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                zoznamMenuJednotky.Add("[N]");

                zoznamMenuNazvy.Add("V v,y");
                zoznamMenuHodnoty.Add(Math.Round(obj_CalcDesign.fV_v_yv * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                zoznamMenuJednotky.Add("[N]");

                zoznamMenuNazvy.Add("V cr,y");
                zoznamMenuHodnoty.Add(Math.Round(obj_CalcDesign.fV_cr_yv * fUnitFactor_Force, iNumberOfDecimalPlaces).ToString());
                zoznamMenuJednotky.Add("[N]");

                zoznamMenuNazvy.Add("λ v,y");
                zoznamMenuHodnoty.Add(Math.Round(obj_CalcDesign.fLambda_v_yv, iNumberOfDecimalPlaces).ToString());
                zoznamMenuJednotky.Add("[-]");

                zoznamMenuNazvy.Add("φ v");
                zoznamMenuHodnoty.Add(Math.Round(obj_CalcDesign.fPhi_v, iNumberOfDecimalPlaces).ToString());
                zoznamMenuJednotky.Add("[-]");

                zoznamMenuNazvy.Add("η");
                zoznamMenuHodnoty.Add(Math.Round(obj_CalcDesign.fEta_723_9_xu_yv, iNumberOfDecimalPlaces).ToString());
                zoznamMenuJednotky.Add("[-]");

                zoznamMenuNazvy.Add("η");
                zoznamMenuHodnoty.Add(Math.Round(obj_CalcDesign.fEta_723_11_V_yv, iNumberOfDecimalPlaces).ToString());
                zoznamMenuJednotky.Add("[-]");

                if (obj_CalcDesign.fEta_723_10_xu > 0)
                {
                    zoznamMenuNazvy.Add("η");
                    zoznamMenuHodnoty.Add(Math.Round(obj_CalcDesign.fEta_723_10_xu, iNumberOfDecimalPlaces).ToString());
                    zoznamMenuJednotky.Add("[-]");
                }

                if (obj_CalcDesign.fEta_723_12_xu_yv_10 > 0)
                {
                    zoznamMenuNazvy.Add("η");
                    zoznamMenuHodnoty.Add(Math.Round(obj_CalcDesign.fEta_723_12_xu_yv_10, iNumberOfDecimalPlaces).ToString());
                    zoznamMenuJednotky.Add("[-]");
                }
            }

            // Interation of internal forces
            zoznamMenuNazvy.Add("M s,x");
            zoznamMenuHodnoty.Add(Math.Round(obj_CalcDesign.fM_s_xu * fUnitFactor_Moment, iNumberOfDecimalPlaces).ToString());
            zoznamMenuJednotky.Add("[Nm]");

            zoznamMenuNazvy.Add("M b,x");
            zoznamMenuHodnoty.Add(Math.Round(obj_CalcDesign.fM_b_xu * fUnitFactor_Moment, iNumberOfDecimalPlaces).ToString());
            zoznamMenuJednotky.Add("[Nm]");

            // Compression and bending
            if (obj_CalcDesign.fEta_721_N > 0 && obj_CalcDesign.fEta_724 > 0)
            {
                zoznamMenuNazvy.Add("η");
                zoznamMenuHodnoty.Add(Math.Round(obj_CalcDesign.fEta_724, iNumberOfDecimalPlaces).ToString());
                zoznamMenuJednotky.Add("[-]");
            }

            // Tension and bending
            if (obj_CalcDesign.fEta_Nt > 0 && obj_CalcDesign.fEta_725_1 > 0)
            {
                zoznamMenuNazvy.Add("M s,x,f");
                zoznamMenuHodnoty.Add(Math.Round(obj_CalcDesign.fM_s_xu_f * fUnitFactor_Moment, iNumberOfDecimalPlaces).ToString());
                zoznamMenuJednotky.Add("[Nm]");

                zoznamMenuNazvy.Add("M s,y,f");
                zoznamMenuHodnoty.Add(Math.Round(obj_CalcDesign.fM_s_yv_f * fUnitFactor_Moment, iNumberOfDecimalPlaces).ToString());
                zoznamMenuJednotky.Add("[Nm]");

                zoznamMenuNazvy.Add("η");
                zoznamMenuHodnoty.Add(Math.Round(obj_CalcDesign.fEta_725_1, iNumberOfDecimalPlaces).ToString());
                zoznamMenuJednotky.Add("[-]");

                zoznamMenuNazvy.Add("η");
                zoznamMenuHodnoty.Add(Math.Round(obj_CalcDesign.fEta_725_2, iNumberOfDecimalPlaces).ToString());
                zoznamMenuJednotky.Add("[-]");
            }

            // Maximum design ratio
            zoznamMenuNazvy.Add("η max");
            zoznamMenuHodnoty.Add(Math.Round(obj_CalcDesign.fEta_max, iNumberOfDecimalPlaces).ToString());
            zoznamMenuJednotky.Add("[-]");
        }

        private void SetResultsDetailsFor_SLS(CCalculMember obj_CalcDesign)
        {
            // Display results in datagrid

            // Deflection
            // δ

            zoznamMenuNazvy.Add("δ lim");
            zoznamMenuHodnoty.Add(obj_CalcDesign.fLimitDeflection.ToString());
            zoznamMenuJednotky.Add("[mm]");

            // Design ratio
            zoznamMenuNazvy.Add("η x/u");
            zoznamMenuHodnoty.Add(obj_CalcDesign.fEta_defl_yu.ToString());
            zoznamMenuJednotky.Add("[mm]");

            zoznamMenuNazvy.Add("η y/v");
            zoznamMenuHodnoty.Add(obj_CalcDesign.fEta_defl_zv.ToString());
            zoznamMenuJednotky.Add("[mm]");

            zoznamMenuNazvy.Add("η x");
            zoznamMenuHodnoty.Add(obj_CalcDesign.fEta_defl_yy.ToString());
            zoznamMenuJednotky.Add("[mm]");

            zoznamMenuNazvy.Add("η y");
            zoznamMenuHodnoty.Add(obj_CalcDesign.fEta_defl_zz.ToString());
            zoznamMenuJednotky.Add("[mm]");
        }

    }
}
