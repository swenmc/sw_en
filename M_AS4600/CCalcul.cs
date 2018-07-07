using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MATH;
using BaseClasses;
using CRSC;

namespace M_AS4600
{
   public struct IF // Design Internal Forces // TODO
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
       };

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

    public class CCalcul
    {
        IF sIF;

        CCrSc_TW cs; // Thin-walled cross-section

        Cb_option eCb_option = Cb_option.eCb_Tab_D2_1;
        SecShape eCS_shape = SecShape.eC_lip;
        SectionShape_Table_D3 eCS_shape_Tab_D3 = SectionShape_Table_D3.eLC;
        SecSymmetry eCS_sym = SecSymmetry.eMS_xu;
        LoadPosition_D2_1 eLoadPosition = LoadPosition_D2_1.eCompFlange;
        LatBracing_D2_1 eLateralBracing = LatBracing_D2_1.eNoBracing;

        float fl_member = 5f;
        float fl_ez = 5f;
        float fl_ex = 5f;
        float fl_ey = 5f;
        float fl_LTB = 5f;

        float fM_max = 2100f;
        float fM_14 = 144f;
        float fM_24 = 541f;
        float fM_34 = 144f;

        float ff_y;
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

        float fx_cfl_par, fy_cfl_par;
        public float fA_cfl, fJ_cfl, fI_x_cfl, fI_y_cfl, fI_xy_cfl, fI_w_cfl;

        public float fPhi_b = 0.9f;
        public float fPhi_v = 0.9f;
        public float fPhi_t = 0.9f;
        public float fPhi_c = 0.85f;

        // AS 4600 variables
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
        public float fDesignRatio_N;

        public float fM_p_xu, fM_y_xu;
        public float fM_p_yv, fM_y_yv;
        public float fM_be_xu;
        public float fM_bd_xu;
        public float fM_bl_xu;

        public float fC_b;
        public float ff_ol_bend;
        public float ff_od_bend;
        public float fLambda_l_xu;
        public float fLambda_d_xu;
        
        float fk;

        public float fEta_max = 0.0f;

        public CCalcul (float fN, float fN_c, float fN_t, float fV_xu, float fV_yv, float fT, float fM_xu, float fM_yv, CCrSc_TW cs_temp)
        {
            AS_4600 eq = new AS_4600();

            sIF.fN = fN;
            sIF.fN_c = fN;
            sIF.fN_t = fN;
            sIF.fV_xu = fV_xu;
            sIF.fV_yv = fV_yv;
            sIF.fT = fT;
            sIF.fM_xu = fM_xu;
            sIF.fM_yv = fM_yv;

            cs = cs_temp;

            // Set material properties
            ff_y = cs.m_Mat.m_ff_yk_0;
            fE = cs.m_Mat.m_fE;
            fG = cs.m_Mat.m_fG;
            fNu = cs.m_Mat.m_fNu;

            //Set cross-section properties
            fh = (float)cs.h;
            fb = (float)cs.b;
            ft = (float)cs.t_min;
            fA_g = (float)cs.A_g;
            fZ_f_xu = (float)cs.W_y_el; // Elastic section modulus
            fZ_f_yv = (float)cs.W_z_el;
            fS_f_xu = (float)cs.W_y_pl; // Plastic section modulus
            fS_f_yv = (float)cs.W_z_pl;

            fA_f_xu = (float)cs.A_vy;
            fA_w_yv = (float)cs.A_vz;
            fZ_ft_xu = fZ_f_xu; //Todo // section modulus of the full unreduced section for the extreme tension fibre about the appropriate axis
            fZ_ft_yv = fZ_f_yv; //Todo // section modulus of the full unreduced section for the extreme tension fibre about the appropriate axis
            fI_xu = (float)cs.I_y;
            fI_yv = (float)cs.I_z;

            float fh_x = fb;
            float fh_y = 0.02f; // No lip

            fI_yc = 54564f;
            fd_l = 0.005f; // Overall stiffener dimension (or overall depth of lip) Figure 2.4.2(a) Clause 2.4 / TABLE 2.4.2 / D1.2.1.2 Simple lipped channels in compression
            fd_1 = fh - 2 * ft; // Web Depth Web-flange juncture // Todo // depth of the flat portion of a web
            fa = fl_member; // Todo

            // Design
            // Compression

            float fa_CEQ = 0f;
            float fb_CEQ = 0f;
            float fc_CEQ = 0f;
            float fd_CEQ = 0f;

            fx_o = (float)cs.D_y_s;
            fy_o = (float)cs.D_z_s;

            fr_x = (float)cs.i_y_rg;
            fr_y = (float)cs.i_z_rg;

            //float fr_o1 = cs.i_yz_rg;
            fr_o1 = eq.Eq_D111_6__(fr_x, fr_y, fx_o, fy_o);

            ff_oz = eq.Eq_D111_5__(fG, fE, (float)cs.I_t, (float)cs.I_w, fA_g, fl_ez, fr_o1);

            ff_ox = eq.Eq_D111_3__(fE, fl_ex, fr_x);
            ff_oy = eq.Eq_D111_3__(fE, fl_ey, fr_y);

            eq.Eq_D111_9__(ff_oz, ff_ox, ff_oy, fr_o1, fx_o, fy_o, out fa_CEQ, out fb_CEQ, out fc_CEQ, out fd_CEQ);
            CCardanoCubicEQSolver cubic_solver = new CCardanoCubicEQSolver(fa_CEQ, fb_CEQ, fc_CEQ, fd_CEQ);

            float ff_oc_real_1 = (float)cubic_solver.x_min_positive;

            ff_oc = (float)cubic_solver.x_min_positive > 0 ? (float)cubic_solver.x_min_positive : 0f;

            //if(ff_oc <= 0f)
            // Error  

            // 7.2.1.2.1 Compression members without holes
            fN_y = eq.Eq_7212_5__(fA_g, ff_y);
            fN_oc = eq.Eq_7212_4__(fA_g, ff_oc);
            flambda_c = eq.Eq_7212_3__(fN_y, fN_oc);
            fN_ce = eq.Eq_7212_1__(flambda_c, fN_y);

            // 7.2.1.3 Local buckling
            // 7.2.1.3.1 Compression members without holes

            fk = 4.0f; //see kst

            ff_ol = eq.Eq_D131____(fk, fE, fNu, ft, fb);
            fN_ol = eq.Eq_7213_4__(fA_g, ff_ol);
            flambda_l = eq.Eq_7213_3__(fN_ce, fN_ol);
            fN_cl = eq.Eq_7213____(flambda_l, fN_ol, fN_ce);

            // 7.2.1.4 Distorsial buckling
            // 7.2.1.4.1 Compression members without holes

            // General channel in compression (picture D2(a))

            eq.Calc_CFL_Properties(fb, fd_l, ft, out fA_cfl, out fx_cfl_par, out fy_cfl_par, out fJ_cfl, out fI_x_cfl, out fI_y_cfl, out fI_xy_cfl);
            // The values of A, J, Ix, Iy, Ixy, Iw are for the compression flange and lip alone.
            float fb_w = fb; // ???

            ff_od = eq.Eq_D121_1__(fE, fA_cfl, fI_x_cfl, fI_y_cfl, fI_xy_cfl, fJ_cfl, fI_w_cfl, fx_o, fy_o, fh_x, fh_y, fb_w, ft);

            if (ff_od <= 0f)
                ff_od = 1f; // Temp TODO - osetrit error

            fN_od = eq.Eq_7214_4__(fA_g, ff_od);
            flambda_d = eq.Eq_7214_3__(fN_ce, fN_ol);
            fN_cd = eq.Eq_7214____(flambda_d, fN_y, fN_od);

            fN_c_min = MathF.Min(fN_ce, fN_cl, fN_cd);

            fDesignRatio_N = Math.Abs(sIF.fN_c / fN_c_min);
            fEta_max = MathF.Max(fEta_max, fDesignRatio_N);

            // 7.2.2 Design of members subject to bending

            // 7.2.2.2.2 Lateral-torsional buckling

            fM_p_xu = eq.Eq_7222_6__(fS_f_xu, ff_y);
            fM_y_xu = eq.Eq_7222_4__(fZ_f_xu, ff_y);

            // Bending about xu-axis

            float fM_o_xu = 0f;
            float fM_ol_xu = 0f;
            float fM_od_xu = 0f;

            float fM_be_xu = fM_y_xu; // Default value
            float fM_bd_xu = fM_y_xu;
            float fM_bl_xu = fM_y_xu;

            if (sIF.fM_xu != 0.0f)
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

                fC_b = eq.Eq_D211_2__(fM_max, fM_14, fM_24, fM_34);

                if (eCS_sym == SecSymmetry.eDS || eCS_sym == SecSymmetry.eMS_xu || eCS_shape == SecShape.eZ)
                    fM_o_xu = eq.Eq_D211_1__(fC_b, fA_g, fr_o1, ff_oy, ff_oz); // D 2.1.1.2(a)
                                                                               //else if()
                                                                               // D 2.1.1.2(b)
                else if (eCS_sym == SecSymmetry.eCS && eCS_shape == SecShape.eZ)
                    fM_o_xu = eq.Eq_D211_7__(fE, fC_b, fh, fI_yc, fl_LTB); // D 2.1.1.3 - Eq. D 2.1.1(7)
                else
                    fM_o_xu = 0.0f; // Neni definovano, zadat manualne

                fM_be_xu = eq.Eq_7222____(fM_o_xu, fM_y_xu);

                // Inelastic reserve
                if (fM_o_xu > 2.78f * fM_y_xu)
                    fM_be_xu = eq.Eq_7222_5__(fM_p_xu, fM_y_xu, fM_o_xu);

                // 7.2.2.3 Local buckling

                float fk_bend = 4.0f; // Todo - factors for bending
                // ft, fb - asi pre kazdy element -> rozhoduje najstihlejsi ???

                ff_ol_bend = eq.Eq_D131____(fk_bend, fE, fNu, ft, fb);
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
                float ff_od_bend = eq.Eq_D121_1_DB(fE, fA_cfl, fI_x_cfl, fI_y_cfl, fI_xy_cfl, fJ_cfl, fI_w_cfl, fx_o, fy_o, fh_x, fh_y, fb_f, fb_w, ft);
                fM_od_xu = eq.Eq_7224_4__(fZ_f_xu, ff_od);
                float fLambda_d_xu = eq.Eq_7224_3__(fM_y_xu, fM_od_xu);
                fM_bd_xu = eq.Eq_7224____(fM_y_xu, fM_od_xu, fLambda_d_xu);
            }

            // D2.3  Local buckling stresses
            float fk_LB_D23 = 4.0f; //see Todo - value valid for bending

            float ff_ol_D23 = eq.Eq_D131____(fk_LB_D23, fE, fNu, ft, fb);

            // 7.2.3 Design of member subject to shear, an combined bending and shear
            float fV_v_yv;
            float fV_y_yv = eq.Eq_723_5___(fA_w_yv, ff_y);
            float fk_v_yv;

            TransStiff_D3 eTrStiff = TransStiff_D3.eD3c_StiffFlanges; // Todo

            switch (eTrStiff)
            {
                case TransStiff_D3.eD3b_HasTrStiff:
                    fk_v_yv = eq.Eq_D3_b____(fa, fd_1);
                    break;
                case TransStiff_D3.eD3c_StiffFlanges:
                    fk_v_yv = eq.Eq_D3_c____(eCS_shape_Tab_D3, fb_f, fa, fd_1, ft_f, ft_w);
                    break;
                case TransStiff_D3.eD3a_NoTrStiff:
                default:
                    fk_v_yv = 5.34f; // D3(a)
                    break;
            }

            float fV_cr_yv = eq.Eq_D3_1____(fE, fA_w_yv, fk_v_yv, fNu, fd_1, ft);

            float fLambda_v_yv = eq.Eq_723_4___(fV_y_yv, fV_cr_yv);

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

            float fM_s_xu;

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

            float fEta_M_xu,
            fEta_V_yv,
            fEta_7235_yv,
            fEta_723_10_xu,
            fEta_723_11_yv,
            fEta_723_12_10,
            fEta_723_12_13;

            float fM_b_xu =  MathF.Min(fM_be_xu, MathF.Min(fM_bl_xu, fM_bd_xu)); // Design resistance value 7.2.2
            float fM_b_xu_drv;
            float fV_v_yv_drv;

            // 7.2.3.5 Combined bending and shear
            eq.Eq_723_9___(sIF.fM_xu, fPhi_b, fM_s_xu, sIF.fV_yv, fPhi_v, fV_v_yv, out fEta_M_xu, out fEta_V_yv, out fEta_7235_yv);
            fEta_max = MathF.Max(fEta_max, fEta_7235_yv);

            if (eTrStiff == TransStiff_D3.eD3b_HasTrStiff)
            {
                eq.Eq_723_10__(sIF.fM_xu, fPhi_b, fM_b_xu, out fM_b_xu_drv, out fEta_723_10_xu);
                fEta_max = MathF.Max(fEta_max, fEta_723_10_xu);
            }

            // Shear
            eq.Eq_723_11__(sIF.fV_yv, fPhi_v, fV_v_yv, out fV_v_yv_drv, out fEta_723_11_yv);
            fEta_max = MathF.Max(fEta_max, fEta_723_11_yv);

            if ((Math.Abs(sIF.fM_xu) / (fPhi_b * fM_s_xu)) > 0.5f && (Math.Abs(sIF.fV_yv) / (fPhi_v * fV_v_yv)) > 0.7f)
            {
                eq.Eq_723_12__(sIF.fM_xu, fPhi_b, fM_s_xu, sIF.fV_yv, fPhi_v, fV_v_yv, out fEta_M_xu, out fEta_V_yv, out fEta_723_12_13, out fEta_723_12_10);
                fEta_max = MathF.Max(fEta_max, fEta_723_12_10);
            }


            float fM_b_yv = float.MaxValue; // Todo

            float fEta_N_724, fEta_Mxu_724, fEta_Myv_724, fEta_724 = 0f;
            float fEta_N_725_1, fEta_Mxu_725_1, fEta_Myv_725_1, fEta_725_1;
            float fEta_N_725_2, fEta_Mxu_725_2, fEta_Myv_725_2, fEta_725_2;

            float fN_t_min = fA_g * ff_y; // Resistance // Todo
            float fPhi_t = 0.9f; // Todo

            float fM_s_xu_f = eq.Eq_725_3___(fZ_ft_xu, ff_y);
            float fM_s_yv_f = eq.Eq_725_3___(fZ_ft_yv, ff_y);

            if (sIF.fN < 0.0f) // Compression
            {
                // 7.2.4 Design of members subject to combined axial compression and bending
                eq.Eq_724_____(fPhi_c, fPhi_b, sIF.fN_c, fN_c_min, sIF.fM_xu, fM_b_xu, sIF.fM_yv, fM_b_yv, out fEta_N_724, out fEta_Mxu_724, out fEta_Myv_724, out fEta_724);
                fEta_max = MathF.Max(fEta_max, fEta_724);
            }
            else
            {
                // 7.2.5 Design of members subject to combined axial tension and bending
                eq.Eq_725_1___(fPhi_t, fPhi_b, sIF.fN_t, fN_t_min, sIF.fM_xu, fM_b_xu, sIF.fM_yv, fM_b_yv, out fEta_N_725_1, out fEta_Mxu_725_1, out fEta_Myv_725_1, out fEta_725_1);
                fEta_max = MathF.Max(fEta_max, fEta_725_1);

                eq.Eq_725_2___(fPhi_t, fPhi_b, sIF.fN_t, fN_t_min, sIF.fM_xu, fM_s_xu_f, sIF.fM_yv, fM_s_yv_f, out fEta_N_725_2, out fEta_Mxu_725_2, out fEta_Myv_725_2, out fEta_725_2);
                fEta_max = MathF.Max(fEta_max, fEta_725_2);
            }

            MessageBox.Show("Calculation finished.\n"
                          + "Design Ratio Nc η = " + fDesignRatio_N + " [-]" + " " + "\n"
                          + "Design Ratio η = " + fEta_7235_yv + " [-]" + " 7.2.3.5" + "\n"
                          + "Design Ratio η = " + fEta_723_11_yv + " [-]" + " 7.2.3(11)" + "\n"
                          + "Design Ratio η = " + fEta_724 + " [-]" + " 7.2.4" + "\n"
                          + "Design Ratio ηmax = "+ fEta_max + " [-]");
        }
    }
}
