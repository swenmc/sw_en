using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MATH;

namespace M_EC2
{
    class EC2
    {
        // Settings

        public bool m_bStabBuck;
        // Variables

        // Internal Forces

        public float m_fN_Ed;

        private float m_fM_Ed_1_y;

        public float FM_Ed_1_y
        {
            get { return m_fM_Ed_1_y; }
            set { m_fM_Ed_1_y = value; }
        }

        private float m_fM_Ed_1_z;

        public float FM_Ed_1_z
        {
            get { return m_fM_Ed_1_z; }
            set { m_fM_Ed_1_z = value; }
        }

        public float m_fM_0_1_y, m_fM_0_2_y;
        public float m_fM_0_1_z, m_fM_0_2_z;

        public float m_fN_0_Ed_qp;
        public float m_fM_0_Ed_qp_y, m_fM_0_Ed_qp_z;

        public float m_fM_1_Ed, m_fM_0_Ed_qp;

        // Cross-Section
        float fb;

        public float Fb
        {
            get { return fb; }
            set { fb = value; }
        }
        float fh;

        public float Fh
        {
            get { return fh; }
            set { fh = value; }
        }

        float m_fa_s_t_y;
        float m_fa_s_t_z;

        float m_fA;

        public float FA
        {
            get { return m_fA; }
            set { m_fA = value; }
        }

        float m_fA_c;

        public float F_A_c
        {
            get { return m_fA_c; }
            set { m_fA_c = value; }
        }

        float m_fA_s;

        public float FA_s
        {
            get { return m_fA_s; }
            set { m_fA_s = value; }
        }

        float m_fI_y;

        public float FI_y
        {
            get { return m_fI_y; }
            set { m_fI_y = value; }
        }

        float m_fI_z;

        public float FI_z
        {
            get { return m_fI_z; }
            set { m_fI_z = value; }
        }

        public float m_fu;

        public float m_fi_s_y;
        public float m_fi_s_z;


        // Member
        // Buckling properties

        //public float m_fLambda_y;
        //public float m_fLambda_z;
        public float m_fn_bal;
        public float m_ft_0;
        public float m_ft;
        public float m_fRH;
        public float m_fT_Delta_t_i;
        public float m_fAlpha;
        public float m_fc_y;
        public float m_fc_z;
        public float m_fL_0_y;
        public float m_fL_0_z;


        // Material
        // Strength

        // Reinforcement

        float m_ff_yk;

        public float Ff_yk
        {
            get { return m_ff_yk; }
            set { m_ff_yk = value; }
        }

        public float m_fE_s;

        float ff_yd;

        public float Ff_yd
        {
            get { return ff_yd; }
            set { ff_yd = value; }
        }


        // Concrete

        public float m_ff_cm;
        public float m_ff_ck_ft_0;

        float fLambda;

        public float FLambda
        {
            get { return fLambda; }
            set { fLambda = value; }
        }

        public float m_ff_ck;

        public float m_ff_cd;

        public float Ff_cd
        {
            get { return m_ff_cd; }
            set { m_ff_cd = value; }
        }

        public float m_fE_cm;

        private float m_fEta;

        public float FEta
        {
            get { return m_fEta; }
            set { m_fEta = value; }
        }

        private float fGamma_Mc;

        public float FGamma_Mc
        {
            get { return fGamma_Mc; }
            set { fGamma_Mc = value; }
        }

        private float fGamma_Ms;

        public float FGamma_Ms
        {
            get { return fGamma_Ms; }
            set { fGamma_Ms = value; }
        }

        // Output
        // Resistances and Design Ratio

        public sRes sResults;

        // Konstruktor
        public EC2(float fN_Ed,
            float fM_Ed_1_y,
            float fM_Ed_1_z,

            float fM_0_1_y,
            float fM_0_2_y,
            float fM_0_1_z,
            float fM_0_2_z,

            float fN_0_Ed_qp,
            float fM_0_Ed_qp_y,
            float fM_0_Ed_qp_z,

            float fM_1_Ed,
            float fM_0_Ed_qp,

            float fb,
            float fh,
            float fa_s_t_y,
            float fa_s_t_z,
            float fA,
            float fA_s,
            float fA_c,
            float fu,
            float fi_s_y,
            float fi_s_z,

        float fn_bal,
        float ft_0,
        float ft,
        float fRH,
        float fT_Delta_t_i,
        float fAlpha,
        float fc_y,
                    float fc_z,

       float fL_0_y,
       float fL_0_z,

            float ff_cm,
            float ff_ck,
            float fE_cm,
            float fLambda,
            float fEta,
            float fGamma_Mc,

            float ff_yk,
            float fE_s,
            float fGamma_Ms
            )
        {
            //
            // Settings

            m_bStabBuck = true; // temp

            // Internal Forces
            m_fN_Ed = fN_Ed;
            m_fM_Ed_1_y = fM_Ed_1_y;
            m_fM_Ed_1_z = fM_Ed_1_z;

            m_fM_0_1_y = fM_0_1_y;
            m_fM_0_2_y = fM_0_2_y;
            m_fM_0_1_z = fM_0_1_z;
            m_fM_0_2_z = fM_0_2_z;

            m_fN_0_Ed_qp = fN_0_Ed_qp;
            m_fM_0_Ed_qp_y = fM_0_Ed_qp_y;
            m_fM_0_Ed_qp_z = fM_0_Ed_qp_z;

            m_fM_1_Ed = fM_1_Ed;
            m_fM_0_Ed_qp = fM_0_Ed_qp;


            // priradenie hodnot premennym zavolanim vlastnosti premennej v inej triede
            // Cross-Section
            Fb = fb;
            Fh = fh;

            m_fa_s_t_y = fa_s_t_y;
            m_fa_s_t_z = fa_s_t_z;

            m_fA = fA;
            m_fA_s = fA_s;
            m_fA_c = fA_c;

            m_fu = fu;
            m_fi_s_y = fi_s_y;
            m_fi_s_z = fi_s_z;

            m_fn_bal = fn_bal;
            m_ft_0 = ft_0;
            m_ft = ft;
            m_fRH = fRH;
            m_fT_Delta_t_i = fT_Delta_t_i;
            m_fAlpha = fAlpha;
            m_fc_y = fc_y;
            m_fc_z = fc_z;

            m_fL_0_y = fL_0_y;
            m_fL_0_z = fL_0_z;

            // Materials
            // Concrete
            m_ff_cm = ff_cm;
            m_ff_ck = ff_ck;
            m_fE_cm = fE_cm;
            FLambda = fLambda;
            m_fEta = fEta;
            FGamma_Mc = fGamma_Mc;

            Ff_cd = ff_ck / FGamma_Mc;

            // Steel
            m_ff_yk = ff_yk;
            m_fE_s = fE_s;
            FGamma_Ms = fGamma_Ms;
            Ff_yd = ff_yk / FGamma_Ms;


            //Calculation

            // Design - temporary
            if (m_fN_Ed > 0.0f)
                EC2_2_TAH();
            else if (m_fN_Ed < 0.0f && MathF.d_equal(FM_Ed_1_y, 0.0f) && MathF.d_equal(FM_Ed_1_z, 0.0f))
            {
                if (!m_bStabBuck)
                    EC2_3_TLAK(); // No Buckling
                else
                {
                    EC2_Buckling();  // Buckling
                    sResults.m_fDesRatio1 = sResults.m_fDesRatio2 = 0f;

                    // Final Check
                    bool bLimit2 = true;
                    EC2_Buckling_Bending(sResults.m_fM_Ed_y, bLimit2, out sResults.m_fM_Rd_y, out sResults.m_fDesRatio1);
                    EC2_Buckling_Bending(sResults.m_fM_Ed_z, bLimit2, out sResults.m_fM_Rd_z, out sResults.m_fDesRatio2);
                    EC2_Buckling_BiBending();
                }
            }
            else if (m_fN_Ed == 0.0f)
            {
               if(FM_Ed_1_y != 0.0f)
                EC2_4_OHYB(Ff_cd, ff_yd, fA_s, fb, fh, fLambda, FM_Ed_1_y, out sResults.m_fM_Rd_y, out sResults.m_fDesRatio1); // Design

               if (FM_Ed_1_z != 0.0f)
                EC2_4_OHYB(Ff_cd, ff_yd, fA_s, fh, fb, fLambda, FM_Ed_1_z, out sResults.m_fM_Rd_z, out sResults.m_fDesRatio2); // Design

               if ((!MathF.d_equal(FM_Ed_1_y, 0.0f)) && (MathF.d_equal(FM_Ed_1_z, 0.0f)))
               {
                   sResults.m_fDesRatio = sResults.m_fDesRatio1;
                   sResults.m_fDesRatio2 = 0f;
               }
               else if ((MathF.d_equal(FM_Ed_1_y, 0.0f)) && (!MathF.d_equal(FM_Ed_1_z, 0.0f)))
               {
                   sResults.m_fDesRatio = sResults.m_fDesRatio2;
                   sResults.m_fDesRatio1 = 0f;
               }
               else
               {
                   EC2_BiBending();
               }
            }
            else if (m_fN_Ed < 0.0f && m_bStabBuck)
            {
                EC2_Buckling();  // Buckling 

                bool bLimit2 = true;
                // Buckling and Bending

                // Note
                // pre referenciu ref alebo out - argument s ref musi byt inicializovany, out nie 

                EC2_Buckling_Bending(sResults.m_fM_Ed_y, bLimit2, out sResults.m_fM_Rd_y, out sResults.m_fDesRatio1);
                EC2_Buckling_Bending(sResults.m_fM_Ed_z, bLimit2, out sResults.m_fM_Rd_z, out sResults.m_fDesRatio2);
                EC2_Buckling_BiBending(); // Final Check
            }
            else
            {
                // Error / Exception
            }
        }

        //////////////////////////////////////////////////////////////
        // HLAVNY VYPOCET
        //////////////////////////////////////////////////////////////

        public void EC2_CrScProp()
        {
            this.FI_y = fb / 12 * MathF.Pow3(fh);
            this.FI_z = fh / 12 * MathF.Pow3(fb);
        }

        public void EC2_2_TAH()
        {
            // NAPR. SEM MOZES PISAT VYPOCET
            // Asi bude najlepsie vytvorit samostatnu metodu pre kazde posudenie (TAH, TLAK, OHYB,  VZPER, OHYB+VZPER, ....)


        }

        public void EC2_3_TLAK()
        {
            // NAPR. SEM MOZES PISAT VYPOCET
            // Asi bude najlepsie vytvorit samostatnu metodu pre kazde posudenie (TAH, TLAK, OHYB,  VZPER, OHYB+VZPER, ....)


        }
        public void EC2_4_OHYB(float ff_cd, float ff_yd, float fA_s, float fb, float fh, float fLambda, float fM_Ed, out float fM_Rd, out float fRatio)
        {
            // Jednotlive metody byte som pomenoval podla nejako podla čísel článkov ale nesmu tam byt bodky "."

            // 6.1 Ohybový moment s normálovou silou nebo bez normálové síly

            // Auxiliary values
            float fx = Eq_x1(fA_s, ff_yd, fb, fLambda, ff_cd);
            float fXi = Eq_Xi(fx, fh);
            float fXi_bal_1 = Eq_Xi_bal_1(ff_yd);
            float fz = Eq_z(fh, fLambda, fx);
            //float fEps_cu;
            //float fEps_sy;

            // Output results
            fM_Rd = Eq_M_Rd(fA_s, ff_yd, fz);
            fRatio = Eq_Ratio(fM_Ed, fM_Rd);
        }

        // Flexural-Buckling
        public void EC2_Buckling()
        {

            // Transform Internal Forces to Design Forces (absolute values)
            float fN_c_Ed = Math.Abs(m_fN_Ed);
            m_fM_Ed_1_y = Math.Abs(m_fM_Ed_1_y);
            m_fM_Ed_1_z = Math.Abs(m_fM_Ed_1_z);

            m_fM_0_1_y = Math.Abs(m_fM_0_1_y);
            m_fM_0_2_y = Math.Abs(m_fM_0_2_y);
            m_fM_0_1_z = Math.Abs(m_fM_0_1_z);
            m_fM_0_2_z = Math.Abs(m_fM_0_2_z);

            m_fN_0_Ed_qp = Math.Abs(m_fN_0_Ed_qp);
            m_fM_0_Ed_qp_y = Math.Abs(m_fM_0_Ed_qp_y);
            m_fM_0_Ed_qp_z = Math.Abs(m_fM_0_Ed_qp_z);

            m_ff_ck_ft_0 = m_ff_ck; // Strength in time t0


            // Calculation - Design Procedure
            float fOmega = Eq_fOmega(m_fA_s, Ff_yd, m_fA_c, m_ff_cd);
            float fB = Eq_B(fOmega);
            float fn = Eq_n(fN_c_Ed, m_fA_c, m_ff_cd);

            float fn_u = Eq_n_u(fOmega);
            float fK_r = Eq_K_r(fn_u, fn, m_fn_bal);

            float fSigma_c = Eq_Sigma_c(m_fN_0_Ed_qp, m_fA_c);
            float f045f_ck_t_0 = Eq_045f_ck_t_0(m_ff_ck_ft_0);
            float fh_0 = Eq_h_0(m_fA_c, m_fu);
            float fAlpha_1 = Eq_Alpha_1(m_ff_cm);
            float fAlpha_2 = Eq_Alpha_2(m_ff_cm);
            float fAlpha_3 = Eq_Alpha_3(m_ff_cm);
            sResults.m_fPhi_RH = Eq_Phi_RH(m_fRH, fh_0, fAlpha_1, fAlpha_2);
            float fBeta_f_cm = Eq_Beta_f_cm(m_ff_cm);
            float fBeta_t_0 = Eq_Beta_ft_0(m_ft_0);
            float fPhi_0 = Eq_Phi_0(sResults.m_fPhi_RH, fBeta_f_cm, fBeta_t_0);
            sResults.m_fBeta_H = Eq_Beta_H(m_fRH, fh_0, fAlpha_3);
            float ft_0_T = Eq_t_0_T(m_fT_Delta_t_i, m_ft_0);
            float ft_0_2 = Eq_t_0_2(ft_0_T, m_fAlpha); // Uprava hodnoty t_0 - je zadana uzivatelom a potom sa modifikuje ???
            float fBeta_c_ft_ft_0 = Eq_Beta_c_ft_ft_0(m_ft, ft_0_2, sResults.m_fBeta_H);
            sResults.m_fPhi_Infinity_ft_0 = Eq_Ph_Infinity_ft_0(fPhi_0, fBeta_c_ft_ft_0);

            float fEps_d = Eq_Eps_d(ff_yd, m_fE_s);

            // y-y
            sResults.m_fLambda_y = Eq_Lambda(m_fL_0_y, fh);
            sResults.m_fBeta_y_ef = Eq_Beta(m_ff_ck, sResults.m_fLambda_y);
            float fr_m_y = Eq_r_m(m_fM_0_1_y, m_fM_0_2_y);
            float fC_y = Eq_C(fr_m_y);
            float fe_i_z = Eq_e_i(m_fL_0_y);
            sResults.m_fM_0_Ed_y = Eq_M_0_Ed(m_fM_Ed_1_y, fN_c_Ed, fe_i_z);
            float fe_0_z = Eq_e_0(sResults.m_fM_0_Ed_y, fN_c_Ed);
            sResults.m_fPhi_y_ef = Eq_Phi_ef(sResults.m_fPhi_Infinity_ft_0, m_fM_0_Ed_qp_y, m_fM_Ed_1_y); // fM_0_Ed_y alebo m_fM_Ed_1_y ???!!!!
            float fA_y = Eq_A(sResults.m_fPhi_y_ef);
            sResults.m_fLambda_y_lim = Eq_fLambda_lim(fA_y, fB, fC_y, fn);
            float fK_Phi_y = Eq_fK_Phi(sResults.m_fBeta_y_ef, sResults.m_fPhi_y_ef);
            float fd_y = Eq_d(fh, m_fi_s_y);
            float f1_r_0_y = Eq_1_r_0(fEps_d, fd_y);
            sResults.m_f1_r_y = Eq_1_r(fK_r, fK_Phi_y, f1_r_0_y);
            float fM_2_y = Eq_M_2(fN_c_Ed, sResults.m_f1_r_y, m_fL_0_y, m_fc_y);
            float fe_2_z = Eq_e_2(fM_2_y, fN_c_Ed);
            sResults.m_fM_Ed_y = Eq_M_Ed(sResults.m_fM_0_Ed_y, fM_2_y);
            sResults.m_fe_tot_z = Eq_e_tot(sResults.m_fM_Ed_y, fN_c_Ed);

            // z-z
            sResults.m_fLambda_z = Eq_Lambda(m_fL_0_z, fb);
            sResults.m_fBeta_z_ef = Eq_Beta(m_ff_ck, sResults.m_fLambda_z);
            float fr_m_z = Eq_r_m(m_fM_0_1_z, m_fM_0_2_z);
            float fC_z = Eq_C(fr_m_z);
            float fe_i_y = Eq_e_i(m_fL_0_z);
            sResults.m_fM_0_Ed_z = Eq_M_0_Ed(m_fM_Ed_1_z, fN_c_Ed, fe_i_y);
            float fe_0_y = Eq_e_0(sResults.m_fM_0_Ed_z, fN_c_Ed);
            sResults.m_fPhi_z_ef = Eq_Phi_ef(sResults.m_fPhi_Infinity_ft_0, m_fM_0_Ed_qp_z, m_fM_Ed_1_z);
            float fA_z = Eq_A(sResults.m_fPhi_z_ef);
            sResults.m_fLambda_z_lim = Eq_fLambda_lim(fA_z, fB, fC_z, fn);
            float fK_Phi_z = Eq_fK_Phi(sResults.m_fBeta_z_ef, sResults.m_fPhi_z_ef);
            float fd_z = Eq_d(fb, m_fi_s_z);
            float f1_r_0_z = Eq_1_r_0(fEps_d, fd_z);
            sResults.m_f1_r_z = Eq_1_r(fK_r, fK_Phi_z, f1_r_0_z);
            float fM_2_z = Eq_M_2(fN_c_Ed, sResults.m_f1_r_z, m_fL_0_z, m_fc_z);
            float fe_2_y = Eq_e_2(fM_2_z, fN_c_Ed);
            sResults.m_fM_Ed_z = Eq_M_Ed(sResults.m_fM_0_Ed_z, fM_2_z);
            sResults.m_fe_tot_y = Eq_e_tot(sResults.m_fM_Ed_z, fN_c_Ed);
        }

        // Flexural-Buckling and Single Bending
        // Note
        // pre referenciu ref alebo out - argument s ref musi byt inicializovany, out nie 

        public void EC2_Buckling_Bending(float fM_Ed, bool bLimit2, out float fM_Rd_min, out float fDesRatio)
        {
            // bLimit2 = true; // Limit!

            float m_fA_s_1 = m_fA_s / 4f; // Temp - Plocha jedneho pozdlzneho pruta

            float fA_s_t_1 = m_fA_s_1 * 2f;  // Suma
            float fA_s_t_2 = m_fA_s_1 * 2f;  // Suma

            float fz_1 = ((m_fA_s_1 * 2) * ((Fh / 2f) - m_fa_s_t_y)) / fA_s_t_1; // Suma !!!
            float fz_2 = ((m_fA_s_1 * 2) * ((Fh / 2f) - m_fa_s_t_y)) / fA_s_t_2; // Suma !!!
            float fz_s = fz_1 + fz_2; // vzialenost tahanej a tlacenej vystuze
            float fd_1 = Fh / 2f - fz_1;
            float fd_2 = Fh / 2f - fz_2;

            float fd = Fh - fd_1;      // Vzdalenost tahane vystuze As1 od tlacene hrany pruzeru
            float fd_strip = Fh - fd_2; // Vzdalenost tlacene vystuze As2 od tazene hrany pruzeru
            float fSigma_s = 400e+6f; // 400 MPa - [Pa]

            float fEps_lim = 700e+6f / (700e+6f + Ff_yd); // [Pa]
            float fEps_lim_2 = 700e+6f / (700e+6f - Ff_yd); // [Pa]

            float fF_s1 = Eq_F_s1(fA_s_t_1, ff_yd);
            float fF_s2 = Eq_F_s2(fA_s_t_2, ff_yd);
            float fF_s = Eq_F_s(fA_s_t_1, fA_s_t_2, Ff_yd);
            float fDelta_F_s = Eq_Delta_F_s(fA_s_t_1, fA_s_t_2, ff_yd);
            float fe_0 = Eq_e_0(fh); // Ma byt mensie nez 20 mm alebo vacsie ?????????

            //Bod 0
            float fN_Rd_0 = Eq_N_Rd_0(fb, fh, m_fEta, m_ff_cd, m_fA_s, fSigma_s);
            float fM_Rd_0 = Eq_M_Rd_0(fA_s_t_1, fz_1, fA_s_t_2, fz_2, fSigma_s);

            //Bod 0´
            sResults.m_fN_eu = Eq_N_eu(fb, fh, m_fEta, m_ff_cd, m_fA_s, fSigma_s);

            //Bod 1
            float fN_Rd_1 = Eq_N_Rd_1(FLambda, Fb, fd, m_ff_cd, fF_s2);
            float fM_Rd_1 = Eq_M_Rd_1(FLambda, Fb, fd, Fh, m_ff_cd, fF_s2, fz_2);

            if (fd < fEps_lim_2 * fd_2) // Distance between face and centroid of reinforcement / fd >= fEps_lim_2 * fd_2 -  TRUE (OK)
                bLimit2 = false; // Error

            //Bod 2
            float fN_cu_lim = Eq_N_cu_lim(fLambda, fEps_lim, Fb, fd, Ff_cd, fDelta_F_s);
            float fM_cu_lim = Eq_M_cu_lim(FLambda, fEps_lim, Fb, fd, fh, m_ff_cd, fF_s1, fz_1, fF_s2, fz_2);
            //Bod 3
            float fx = Eq_x(fF_s1, FLambda, Fb, m_fEta, m_ff_cd);
            float fN_Rd_3 = Eq_N_Rd_3();
            float fM_Rd_3 = Eq_M_Rd_3(fF_s1, fd, fLambda, fx);
            //Bod 4
            float fN_Rtd_lim = Eq_N_Rtd_lim(fF_s1);
            float fM_Rtd_lim = Eq_M_Rtd_lim(fF_s1, fz_1);
            //Bod 5
            float fN_Rtd_0 = Eq_N_Rtd_0(fF_s1, fF_s2);
            float fM_Rtd_0 = Eq_M_Rtd_0(fF_s1, fz_1, fF_s2, fz_2);

            // Resistance

            float fM_Rd_0_1 = (fM_Rd_0 + ((fM_Rd_1 - fM_Rd_0) / (fN_Rd_0 - fN_Rd_1)) * (fN_Rd_0 - m_fN_Ed));
            float fM_Rd_1_2 = (fM_Rd_1 + ((fM_cu_lim - fM_Rd_1) / (fN_Rd_1 - fN_cu_lim)) * (fN_Rd_1 - m_fN_Ed));
            float fM_Rd_2_3 = (fM_Rd_3 + ((fM_cu_lim - fM_Rd_3) / (fN_cu_lim - fN_Rd_3)) * (m_fN_Ed - fN_Rd_3));
            fM_Rd_min = MathF.Min(fM_Rd_0_1, fM_Rd_1_2, fM_Rd_2_3);

            fDesRatio = fM_Ed / fM_Rd_min;
        }

        // Bi-axial Bending
        public void EC2_BiBending()
        {

            // Interaction of Internal Forces
            // Biaxial Bending
            sResults.m_fN_Rd = Eq_N_Rd(m_fA_c, m_ff_cd, FA_s, ff_yd);
            float fRatio_N_Ed_N_Rd = Eq_Ratio_N_Ed_N_Rd(Math.Abs(m_fN_Ed), sResults.m_fN_Rd);
            float fRatio_My = Eq_Ratio_M_Ed_M_Rd(m_fM_Ed_1_y, sResults.m_fM_Rd_y);
            float fRatio_Mz = Eq_Ratio_M_Ed_M_Rd(m_fM_Ed_1_z, sResults.m_fM_Rd_z);
            sResults.m_fa = Get_a_Table(fRatio_N_Ed_N_Rd);
            float fRatio_M = Eq_DesRatio(m_fM_Ed_1_y, m_fM_Ed_1_z, sResults.m_fM_Rd_y, sResults.m_fM_Rd_z, sResults.m_fa);

            sResults.m_fDesRatio = fRatio_M;
        }

        // Flexural-Buckling and Bi-axial Bending
        public void EC2_Buckling_BiBending()
        {

        // Interaction of Internal Forces
        // Axial Force and Biaxial Bending
        sResults.m_fN_Rd = Eq_N_Rd(m_fA_c, m_ff_cd, FA_s,ff_yd);
        float fRatio_N_Ed_N_Rd = Eq_Ratio_N_Ed_N_Rd(Math.Abs(m_fN_Ed), sResults.m_fN_Rd);
        float fRatio_My = Eq_Ratio_M_Ed_M_Rd(sResults.m_fM_Ed_y, sResults.m_fM_Rd_y);
        float fRatio_Mz = Eq_Ratio_M_Ed_M_Rd(sResults.m_fM_Ed_z, sResults.m_fM_Rd_z);
        sResults.m_fa = Get_a_Table(fRatio_N_Ed_N_Rd);

        float fRatio_M = Eq_DesRatio(sResults.m_fM_Ed_y, sResults.m_fM_Ed_z, sResults.m_fM_Rd_y, sResults.m_fM_Rd_z, sResults.m_fa);

        float fRatio_N_Ed_N_eu = Eq_Ratio_N_Ed_N_Rd(Math.Abs(m_fN_Ed), Math.Abs(sResults.m_fN_eu));

        sResults.m_fDesRatio = MathF.Max(fRatio_N_Ed_N_Rd, fRatio_M, fRatio_N_Ed_N_eu);
        }














    /////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // Code/Standard Formulas and Equations
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Rectangular Cross-ection
        // Uniaxial bending  6.1

        public float Eq_x1(float fa_s, float ff_yd, float fb, float flambda, float ff_cd)
        {
            return fa_s * ff_yd / (fb * flambda * ff_cd); // fx
        }
        public float Eq_Xi(float fx, float fh)
        {
            return fx / fh; // fXi
        }
        public float Eq_Xi_bal_1(float ff_yd)
        {
            return 700f / (700f + ff_yd);  // fXi_bal_1 = fEps_cu / (fEps_cu + fEps_sy) 
        }
        //fXi < fXi_bal_1;
        public float Eq_z(float fh, float fLambda, float fx)
        {
            return fh - fLambda * fx / 2f; // dz
        }
        public float Eq_M_Rd(float fA_s, float ff_yd, float fz)
        {
            return fA_s * ff_yd * fz; // M_Rd
        }
        public float Eq_A_s_min(float ff_ctm, float ff_yk, float fb, float fh)
        {
            return Math.Max(0.26f * (ff_ctm / ff_yk) * fb * fh, 0.0013f * fb * fh); // fA_s_min
        }
        public float Eq_A_s_max(float fA_c, float fh, float fb, float ff_cd, float ff_yd)
        {
            return Math.Min(0.04f * fA_c, 0.8f * 0.45f * fb * fh * ff_cd / ff_yd); // fA_s_max
        }
        public float Eq_Ratio(float fM_Ed, float fM_Rd)
        {
            return fM_Ed / fM_Rd; // Design Ratio
        }

        // 6.2 Shear
        // Shear Force  6.2.2

        public float Eq_C_Rd(float fGamma_Mc)
        {
            return 0.18f / fGamma_Mc; // fC_Rd
        }
        public float Eq_k_Shear622(float fh)
        {
            return 1 + MathF.Sqrt(200 / fh); //fk
        }
        public float Eq_Rho_l(float fA_sl, float fb, float fh)
        {
            return fA_sl / (fb * fh); // fRho_l
        }
        public float Eq_v_min(float fk, float ff_ck)
        {
            return 0.035f * MathF.Pow_3_2(fk) * MathF.Sqrt(ff_ck); //fv_min
        }
        public float Eq_V_Rd_c(float fCRd_c, float fk, float fRho_l, float ff_ck, float fb_w, float fh)
        {
            return (fCRd_c * fk * MathF.Pow_1_3(100 * fRho_l * ff_ck)) * fb_w * fh;  // fV_Rd_c
        }
        public float Eq_Ratio_V_min(float fV_Rd_c, float fv_min, float fb, float fh)
        {
            return fV_Rd_c / (fv_min * fb * fh); // Ratio 
        }
        public float Eq_Ratio_V(float fV_Ed, float fV_Rd_c)
        {
            return fV_Ed / fV_Rd_c; // Design Ratio 
        }

        // Shear Force  6.2.3
        public float Eq_Tau_Rd(float fF_ctk0_05, float fGamma_Mc)
        {
            return 0.25f * fF_ctk0_05 / fGamma_Mc; //  fTau_Rd
        }
        public float Eq_k_Shear623(float fh)
        {
            if (1.6f - fh / 1000 <= 1)
                return 1f;
            else
                return 1.6f - fh / 1000;
        }

        // opakuje sa
        //        public float Eq_Rho_l(float fA_sl, float fb, float fh)
        //        {
        //        return Asl / (fb*fh); // fRho_l
        //        }

        public float Eq_V_cd(float fTau_Rd, float fk, float fRho_l, float fb, float fh)
        {
            // Únosnost přenašená tlačeným betonem
            return fTau_Rd * fk * (1.2f + 40f * fRho_l) * fb * fh; // fV_cd 
        }


        //θ
        //cot θ

        public float Eq_v(float ff_ck)
        {
            return 0.6f * (1 - ff_ck / 250); // fv
        }
        public float Eq_Rho_w(float fA_sw, float fb, float fs, float fv, float ff_cd, float ff_y_wd)
        {
            return Math.Min(fA_sw / (fb * fs), 0.5f * fv * ff_cd / ff_y_wd); // fRho_w
        }
        public float Eq_Rho_w_min(float ff_ck, float ff_yk)
        {
            return (0.08f * MathF.Sqrt(ff_ck)) / ff_yk; // fRho_w_min
            //ρw ≥ ρw,min
        }
        public float Eq_z(float fd)
        {
            return 0.9f * fd; // fz
        }

        // Únosnost třmínku
        public float Eq_V_Rd_s(float fA_sw, float ff_y_wd, float fs, float fz, float fDelta)
        {
            return ((fA_sw * ff_y_wd) / fs) * fz * (float)Math.Atan(fDelta); // V_Rd_s
        }

        // Únosnost tlakových diagonál
        public float Eq_V_Rd_max(float fv, float ff_cd, float fb, float fz, float fDelta)
        {
            return fv * ff_cd * fb * fz * ((float)Math.Atan(fDelta) / (1f + (float)MathF.Pow2((float)Math.Atan(fDelta)))); // fV_Rd_max 
        }
        public float Eq_V_Rd(float fV_cd, float fV_Rd_s, float fV_Rd_max)
        {
            return Math.Min(fV_cd + fV_Rd_s, fV_Rd_max); // fV_Rd
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Výpočet vzperu EN 1992-1-1-5.8.8 Metoda založená na menovitej krivosti:
        // 5.8.8 Metoda zalozena na menovitej krivosti
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        // Input 
        // fM_0_1
        // fM_0_2
        // fn_bal
        // ft_0
        // ft
        // fRH
        // fT_Delta_t_i
        // fAlpha
        // fc

        public float Eq_Lambda(float fL_0, float fh)
        {
            return fL_0 * MathF.Sqrt(12f) / fh; // fLambda
        }
        public float Eq_A(float fPhi_ef)
        {
            return 1f / (1f + 0.2f * fPhi_ef); // fA
        }
        public float Eq_B(float fOmega)
        {
            return MathF.Sqrt(1 + 2 * fOmega); // fB
        }
        public float Eq_C(float fr_m)
        {
            return 1.7f - fr_m;  // fC
        }
        public float Eq_r_m(float fM_0_1, float fM_0_2)
        {
            if (!MathF.d_equal(fM_0_2, 0f))
                return fM_0_1 / fM_0_2; // r_m
            else if (!MathF.d_equal(fM_0_1, 0f))
                return fM_0_2 / fM_0_1; // r_m
            else
                return 0f;
        }
        public float Eq_fLambda_lim(float fA, float fB, float fC, float fn)
        {
            if (fn >= 0f)
                return 20 * fA * fB * fC / MathF.Sqrt(fn); // fLambda_lim
            else
                return 0f;
        }
        /*
        public float Eq_(float b)
        {
        return fLambda < fLambda_lim; //
        }
        */
        public float Eq_e_i(float fL_0)
        {
            return fL_0 / 400; // fe_i  Imperfection
        }
        public float Eq_M_0_Ed(float fM_1_Ed, float fN_Ed, float fe_i)
        {
            return fM_1_Ed + fN_Ed * fe_i; // fM_0_Ed
        }
        public float Eq_e_0(float fM_0_Ed, float fN_Ed)
        {
            if (!MathF.d_equal(fN_Ed, 0f))
                return fM_0_Ed / fN_Ed;  // fe_0
            else
                return 0f;
        }
        public float Eq_fOmega(float fA_s, float ff_yd, float fA_c, float ff_cd)
        {
            return fA_s * ff_yd / (fA_c * ff_cd); // fOmega
        }
        public float Eq_n_u(float fOmega)
        {
            return 1f + fOmega; // fn_u
        }
        public float Eq_n(float fN_Ed, float fA_c, float ff_cd)
        {
            return fN_Ed / (fA_c * ff_cd); // fn
        }
        public float Eq_K_r(float fn_u, float fn, float fn_bal)
        {
            if (!MathF.d_equal(fn_u - fn_bal, 0f))
                return Math.Min((fn_u - fn) / (fn_u - fn_bal), 1.0f); //fK_r
            else
                return 0f;
        }
        public float Eq_Beta(float ff_ck, float fLambda)
        {
            // ff_ck [Pa]
            return 0.35f + ff_ck / 200000000 - fLambda / 150; // fBeta
        }
        public float Eq_Sigma_c(float fN_0_Ed_qp, float fA_c)
        {
            return fN_0_Ed_qp / fA_c; // fSigma_c
        }
        public float Eq_045f_ck_t_0(float ff_ck_ft_0)
        {
            return 0.45f * ff_ck_ft_0; //
        }
        public float Eq_h_0(float fA_c, float fu)
        {
            if (fu > 0f)
                return 2 * fA_c / fu; // fh_0
            else
                return 0f;
        }
        public float Eq_Alpha_1(float ff_cm)
        {
            return (float)Math.Pow(35000000f / ff_cm, 0.7f); // fAlpha_1 [Pa]
        }
        public float Eq_Alpha_2(float ff_cm)
        {
            return (float)Math.Pow(35000000f / ff_cm, 0.2f); // fAlpha_2 [Pa]
        }
        public float Eq_Alpha_3(float ff_cm)
        {
            return (float)Math.Pow(35000000f / ff_cm, 0.5f); // fAlpha_3 [Pa]
        }
        public float Eq_Phi_RH(float fRH, float fh_0, float fAlpha_1, float fAlpha_2)
        {
            // Convert h_0 from m to mm ???!!!
            fh_0 *= 1000f;
            return (1 + (1 - fRH / 100) / (0.1f * MathF.Pow_1_3(fh_0)) * fAlpha_1) * fAlpha_2; // fPhi_RH
        }
        public float Eq_Beta_f_cm(float ff_cm)
        {
            // Convert f_cm from Pa to MPa !!!
            ff_cm /= 1000000f;
            return 16.8f / MathF.Sqrt(ff_cm); // fBeta_f_cm [MPa]
        }
        public float Eq_Beta_ft_0(float ft_0)
        {
            return 1 / (0.1f + (float)Math.Pow(ft_0, 0.2f)); // fBeta_ft_0
        }
        public float Eq_Phi_0(float fPhi_RH, float fBeta_f_cm, float fBeta_t_0)
        {
            return fPhi_RH * fBeta_f_cm * fBeta_t_0; // fPhi_0
        }
        public float Eq_Beta_H(float fRH, float fh_0, float fAlpha_3)
        {
            // Convert h_0 from m to mm ???!!!
            fh_0 *= 1000f;
            return Math.Min(1.5f * (1 + MathF.PowN(0.012f * fRH, 18)) * fh_0 + 250 * fAlpha_3, 1500 * fAlpha_3);    // fBeta_H
        }
        public float Eq_t_0_T(float fT_Delta_t_i, float fDelta_t_i)
        {
            return /*∑*/ (float)Math.Pow(Math.E, -(4000 / (273 + fT_Delta_t_i) - 13.65f)) * fDelta_t_i;  // ft_0_T
        }
        public float Eq_t_0_2(float ft_0_T, float fAlpha)
        {
            return Math.Max(ft_0_T * (float)Math.Pow(9 / (2 + (float)Math.Pow(ft_0_T, 1.2f) + 1), fAlpha), 0.5f);  // ft_0
        }
        public float Eq_Beta_c_ft_ft_0(float ft, float ft_0, float fBeta_H)
        {
            return (float)Math.Pow((ft - ft_0) / (fBeta_H + ft - ft_0), 0.3f);    // fBeta_c_ft_ft_0
        }
        public float Eq_Ph_Infinity_ft_0(float fPhi_0, float fBeta_c_ft_ft_0)
        {
            return fPhi_0 * fBeta_c_ft_ft_0;  // fPh_Infinity_ft_0
        }
        public float Eq_Phi_ef(float fPhi_Infinity_ft_0, float fM_0_Ed_qp, float fM_0_Ed)
        {
            if(!MathF.d_equal(fM_0_Ed, 0f))
                return fPhi_Infinity_ft_0 * fM_0_Ed_qp / fM_0_Ed; // fPhi_ef
            else
                return 0f;
        }
        public float Eq_fK_Phi(float fBeta, float fPhi_ef)
        {
            return Math.Max(1 + fBeta * fPhi_ef, 1.0f); // fK_Phi
        }
        public float Eq_Eps_d(float ff_yd, float fE_s)
        {
            return ff_yd / fE_s; // fEps_d 
        }
        public float Eq_d(float fh, float fi_s)
        {
            return fh / 2f + fi_s; // fd - Not section depth but factor
        }
        public float Eq_1_r_0(float fEps_d, float fd)
        {
            return fEps_d / (0.45f * fd); // f1_r_0
        }
        public float Eq_1_r(float fK_r, float fK_Phi, float f1_r0)
        {
            return fK_r * fK_Phi * f1_r0; // f1_r
        }
        public float Eq_M_2(float fN_Ed, float f1_r, float fL_0, float fc)
        {
            return /*fN_Ed.e2*/ fN_Ed * f1_r * MathF.Pow2(fL_0) / fc; // fM_2
        }
        public float Eq_e_2(float fM_2, float fN_Ed)
        {
            if (!MathF.d_equal(fN_Ed, 0f))
                return fM_2 / fN_Ed; // f_e_2
            else
                return 0f;
        }
        public float Eq_M_Ed(float fM_0_Ed, float fM_2)
        {
            return fM_0_Ed + fM_2; // fM_Ed
        }
        public float Eq_e_tot(float fM_Ed, float fN_Ed)
        {
            if (!MathF.d_equal(fN_Ed, 0f))
                return fM_Ed / fN_Ed; // fe_tot
            else
                return 0f;
        }

        // Axial Force and Bending Moment
        // N + M_Ed
        public float Eq_F_s1(float fA_s1, float ff_yd)
        {
            return fA_s1 * ff_yd; // fF_s1
        }
        public float Eq_F_s2(float fA_s2, float ff_yd)
        {
            return fA_s2 * ff_yd; // fF_s2
        }
        public float Eq_F_s(float fA_s1, float fA_s2, float ff_yd)
        {
            return (fA_s1 + fA_s2) * ff_yd; // fF_s
        }
        public float Eq_Delta_F_s(float fA_s1, float fA_s2, float ff_yd)
        {
            return (fA_s2 - fA_s1) * ff_yd; // fDelta_F_s
        }
        public float Eq_e_0(float fh)
        {
            // Convert fh from m to mm !!!
            fh *= 1000f;
            return Math.Min(fh / 30, 20); // fe_0 [mm]
        }
        //Bod 0
        public float Eq_N_Rd_0(float fb, float fh, float fEta, float ff_cd, float fA_s, float fSigma_s)
        {
            return -(fb * fh * fEta * ff_cd +/*∑*/fA_s * fSigma_s); // fN_cu = fN_Rd_0
        }
        public float Eq_M_Rd_0(float fA_s1, float fz_s1, float fA_s2, float fz_s2, float fSigma_s)
        {
            return (fA_s2 * fz_s2 - fA_s1 * fz_s1) * fSigma_s; // fM_cu = fM_Rd_0
        }
        //Bod 0´
        public float Eq_N_eu(float fb, float fh, float fEta, float ff_cd, float fA_s, float fSigma_s)
        {
            return -(0.8f * fb * fh * fEta * ff_cd +/*∑*/fA_s * fSigma_s);  // fN_eu = fN_Rd_0´ 
        }
        //Bod 1
        public float Eq_N_Rd_1(float ffLamda, float fb, float fd, float ff_cd, float fF_s2)
        {
            return -(fLambda * fb * fd * ff_cd + fF_s2); // fN_Rd_1
        }
        public float Eq_M_Rd_1(float fLambda, float fb, float fd, float fh, float ff_cd, float fF_s2, float fz_s2)
        {
            return fLambda * fb * fd * (0.5f * fh - 0.4f * fd) * ff_cd + fF_s2 * fz_s2; // fM_Rd_1
        }
        // fd> = fEps_lim_2*fd_2; // Distance between face and centroid of reinforcement
        //Bod 2
        public float Eq_N_cu_lim(float fLamda, float fEps_lim, float fb, float fd, float ff_cd, float fDelta_F_s)
        {
            return -(fLambda * fEps_lim * fb * fd * ff_cd + fDelta_F_s); // fN_cu_lim 
        }
        public float Eq_M_cu_lim(float fLamda, float fEps_lim, float fb, float fd, float fh, float ff_cd, float fF_s1, float fz_1, float fF_s2, float fz_2)
        {
            return fLambda * fEps_lim * fb * fd * (0.5f * fh - 0.4f * fEps_lim * fd) * ff_cd + fF_s2 * fz_2 + fF_s1 * fz_1; // fM_cu_lim = fM_Rd_lim
        }
        //Bod 3
        public float Eq_N_Rd_3()
        {
            return 0f; // fN_Rd_3
        }
        public float Eq_M_Rd_3(float fF_s1, float fd, float fLambda, float fx)
        {
            return fF_s1 * (fd - 0.5f * fLambda * fx); // fM_Rd_3
        }
        public float Eq_x(float fF_s1, float fLamda, float fb, float fEta, float ff_cd)
        {
            return fF_s1 / (fLambda * fb * fEta * ff_cd); // fx
        }
        //Bod 4
        public float Eq_N_Rtd_lim(float fF_s1)
        {
            return fF_s1; // fN_Rtd_lim = fF_s1
        }
        public float Eq_M_Rtd_lim(float fF_s1, float fz_1)
        {
            return fF_s1 * fz_1; // fM_Rtd_lim
        }
        //Bod 5
        public float Eq_N_Rtd_0(float fF_s1, float fF_s2)
        {
            return fF_s1 + fF_s2;  // fN_Rtd_0
        }
        public float Eq_M_Rtd_0(float fF_s1, float fz_1, float fF_s2, float fz_2)
        {
            return fF_s1 * fz_1 - fF_s2 * fz_2; // fM_Rtd_0
        }

        // Interaction of Internal Forces
        // Axial Force and Biaxial Bending
        public float Eq_N_Rd(float fA_c, float ff_cd, float fA_s, float ff_yd)
        {
            return fA_c * ff_cd + fA_s * ff_yd; // fN_Rd
        }
        public float Eq_Ratio_M_Ed_M_Rd(float fM_Ed, float fM_Rd)
        {
            return fM_Ed / fM_Rd; // Design Ratio
        }
        public float Eq_Ratio_N_Ed_N_Rd(float fN_Ed, float fN_Rd)
        {
            return fN_Ed / fN_Rd; //Design Ratio
        }
        static float[,] arrTable_a = new float[3, 2]
{
{0.1f, 1.0f},
{0.7f, 1.5f},
{1.0f, 2.0f}
};
        public float Get_a_Table(float fRatio_N_Ed_N_Rd)
        {
            if (fRatio_N_Ed_N_Rd < 0.7f)
                return (((fRatio_N_Ed_N_Rd - arrTable_a[0, 0]) / ((arrTable_a[1, 0] - arrTable_a[0, 0]) / (arrTable_a[1, 1] - arrTable_a[0, 1]))) + 1);
            else
                return (((fRatio_N_Ed_N_Rd - arrTable_a[1, 0]) / ((arrTable_a[2, 0] - arrTable_a[1, 0]) / (arrTable_a[2, 1] - arrTable_a[1, 1]))) + 1.5f);
        }
        public float Eq_DesRatio(float fM_y_Ed, float fM_z_Ed, float fM_y_Rd, float fM_z_Rd, float fa)
        {
            if (MathF.d_equal(fM_y_Ed,0f) && MathF.d_equal(fM_z_Ed,0f))
                return 0f;
            else if(MathF.d_equal(fM_y_Ed,0f))
                return (float)Math.Pow(fM_z_Ed / fM_z_Rd, fa) / 1f;
            else if (MathF.d_equal(fM_z_Ed,0f))
                return (float)Math.Pow(fM_y_Ed / fM_y_Ed, fa) / 1f;
            else
                return ((float)Math.Pow(fM_z_Ed / fM_z_Rd, fa) + (float)Math.Pow(fM_y_Ed / fM_y_Rd, fa)) / 1f; // Design Ratio
        }










    }
}
