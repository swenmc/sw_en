using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using M_BASE.Concrete;
using MATH;

namespace M_EC2
{
    public struct sRes
    {
        // Results

        public float m_fLambda_y;
        public float m_fLambda_z;

        public float m_fLambda_y_lim;
        public float m_fLambda_z_lim;

        public float m_fM_0_Ed_y;
        public float m_fM_0_Ed_z;

        public float m_fBeta_y_ef;
        public float m_fBeta_z_ef;

        public float m_f1_r_y;
        public float m_f1_r_z;

        public float m_fPhi_y_ef;
        public float m_fPhi_z_ef;

        public float m_fM_Ed_y;
        public float m_fM_Ed_z;

        public float m_fe_tot_z;
        public float m_fe_tot_y;

        public float m_fPhi_RH;
        public float m_fBeta_H;
        public float m_fPhi_Infinity_ft_0;

        public float m_fN_Rd;
        public float m_fN_eu;
        public float m_fM_Rd_y;
        public float m_fM_Rd_z;

        public float m_fa;

        public float m_fDesRatio1;
        public float m_fDesRatio2;
        public float m_fDesRatio;
    }


    public partial class Form_M_EC2 : Form
    {
        // Internal Forces

        public float m_fN_Ed;
        public float m_fM_Ed_1_y;
        public float m_fM_Ed_1_z;
        public float m_fM_Ed_1;

        public float m_fN_0_Ed_qp;
        public float m_fM_0_Ed_qp_y;
        public float m_fM_0_Ed_qp_z;
        public float m_fM_0_Ed_qp;

        public float m_fM_0_1_y;
        public float m_fM_0_1_z;
        public float m_fM_0_2_y;
        public float m_fM_0_2_z;

        public float m_fe_Sd_y;
        public float m_fe_Sd_z;

        // Concrete

        public float  m_fGamma_Mc = 1.5f;

        private float m_fLambda = 0.8f;

        public float FLambda
        {
            get { return m_fLambda; }
            set { m_fLambda = value; }
        }

        public float m_fAlpha_cc;
        public float m_fEta;


       // Reinforcement
       // Material
        private float m_ff_yk;

        public float Ff_yk
        {
            get { return m_ff_yk; }
            set { m_ff_yk = value; }
        }

        private float m_ff_uk;

        public float Ff_uk
        {
            get { return m_ff_uk; }
            set { m_ff_uk = value; }
        }

        private float m_fE_s;

        public float FE_s
        {
            get { return m_fE_s; }
            set { m_fE_s = value; }
        }

        public float m_fGamma_Ms = 1.15f;

        // Reinforcement  -  Geometry and properties
        // Longitudinal


        // Shear / Transversal
        private float m_fd_s_s;

        public float Fd_s_s
        {
            get { return m_fd_s_s; }
            set { m_fd_s_s = value; }
        }
        private float m_fA_s_s;

        public float FA_s_s
        {
            get { return m_fA_s_s; }
            set { m_fA_s_s = value; }
        }
        private float m_fs_s;

        public float Fs_s
        {
            get { return m_fs_s; }
            set { m_fs_s = value; }
        }



        // Cross - section 

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

        float m_fA;

        public float FA
        {
            get { return m_fA; }
            set { m_fA = value; }
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

        private float m_fA_s_t;

        public float FA_s_t
        {
            get { return m_fA_s_t; }
            set { m_fA_s_t = value; }
        }

        private float m_fA_s_1;

        public float FA_s_1
        {
            get { return m_fA_s_1; }
            set { m_fA_s_1 = value; }
        }

        float m_fad;

        public float Fad
        {
            get { return m_fad; }
            set { m_fad = value; }
        }
        float m_fbc;

        public float Fbc
        {
            get { return m_fbc; }
            set { m_fbc = value; }
        }
        float m_fa_s_t_y;

        public float Fa_s_t_y
        {
            get { return m_fa_s_t_y; }
            set { m_fa_s_t_y = value; }
        }

        float m_fa_s_t_z;

        public float Fa_s_t_z
        {
            get { return m_fa_s_t_z; }
            set { m_fa_s_t_z = value; }
        }

        float m_ft_b;

        public float Ft_b
        {
            get { return m_ft_b; }
            set { m_ft_b = value; }
        }


        private int m_iNo;

        public int INo
        {
            get { return m_iNo; }
            set { m_iNo = value; }
        }

        float m_fA_c;

        public float FA_c
        {
            get { return m_fA_c; }
            set { m_fA_c = value; }
        }

        private float m_fd_s;

        public float Fd_s
        {
            get { return m_fd_s; }
            set { m_fd_s = value; }
        }

        float m_fu;

        public float Fu
        {
            get { return m_fu; }
            set { m_fu = value; }
        }

        float m_fI_s_y;

        public float FI_s_y
        {
            get { return m_fI_s_y; }
            set { m_fI_s_y = value; }
        }
        float m_fI_s_z;

        public float FI_s_z
        {
            get { return m_fI_s_z; }
            set { m_fI_s_z = value; }
        }

        float m_fi_s_y;

        public float Fi_s_y
        {
            get { return m_fi_s_y; }
            set { m_fi_s_y = value; }
        }

        float m_fi_s_z;

        public float Fi_s_z
        {
            get { return m_fi_s_z; }
            set { m_fi_s_z = value; }
        }


        // Member Geometry

        public float m_fL;
        public float m_fBeta_y;
        public float m_fBeta_z;

        public float m_fL_0_y;
        public float m_fL_0_z;


        // Settings and Auxiliary Values for Buckling

        public float m_fn_bal;
        public float m_ft_0;
        public float m_ft;
        public float m_fRH;
        public float m_fT_Delta_t_i;
        public float m_fAlpha;
        public float m_fc_y;
        public float m_fc_z;

        // Results

        public sRes sResults;





        MatTemp m_objDatabase = new MatTemp(); // Objet databazy Concrete a Reinforcement

        public Form_M_EC2()
        {
            InitializeComponent();

            // Text and Default Values
            Default_data();
            // Display Deafult Values
            Display_Input_Data();
        }

        // Metoda ktora prepocita medzivysledky z udajov zadanych uzivatelom
        private void Calculate_Auxiliary()
        {
            // Vypocet vstupov 
            // Premenne ktore je mozne spocitat pred hlavnym vypoctom
            // Sive policka

            // Resulting IF
            if (MathF.d_equal(m_fM_Ed_1_y, 0f) && MathF.d_equal(m_fM_Ed_1_z, 0f))
                m_fM_Ed_1 = 0f;
            else
                m_fM_Ed_1 = MathF.Sqrt(MathF.Pow2(m_fM_Ed_1_y) + MathF.Pow2(m_fM_Ed_1_z));
            //m_fM_Ed_1 = (float)Math.Sqrt(Math.Pow(m_fM_Ed_1_y,2) + Math.Pow(m_fM_Ed_1_z,2));
            if (MathF.d_equal(m_fM_0_Ed_qp_y, 0f) && MathF.d_equal(m_fM_0_Ed_qp_z, 0f))
                m_fM_0_Ed_qp = 0f;
            else
                m_fM_0_Ed_qp = MathF.Sqrt(MathF.Pow2(m_fM_0_Ed_qp_y) + MathF.Pow2(m_fM_0_Ed_qp_z));
            //m_fM_0_Ed_qp = (float)Math.Sqrt(Math.Pow(m_fM_0_Ed_qp_y, 2) + Math.Pow(m_fM_0_Ed_qp_z, 2));

            if (MathF.d_equal(m_fN_Ed, 0f))
                m_fe_Sd_y = m_fe_Sd_z = 0f;
            else
            {
                m_fe_Sd_y = Math.Abs(m_fM_Ed_1_y / m_fN_Ed);
                m_fe_Sd_z = Math.Abs(m_fM_Ed_1_z / m_fN_Ed);
            }

            m_fL_0_y = m_fBeta_y * m_fL;
            m_fL_0_z = m_fBeta_z * m_fL;

            float fA_s_1 = MathF.fPI * MathF.Pow2(Fd_s) / 4; // Plocha jedneho pruta pre konkretny priemer a polohu
            float fA_s_t_1; //Plocha vsetkych nosnych pozdlznych prutov jedneho typu
            FA_s_t = fA_s_t_1 = m_iNo * fA_s_1; // Suma plocha vsetkych nosnych pozdlznych prutov


            Fad = m_fd_s_s;
            Fbc = m_fd_s_s;

            Fa_s_t_y = Ft_b + m_fd_s_s + m_fd_s / 2f;
            Fa_s_t_z = Ft_b + m_fd_s_s + m_fd_s / 2f;

            // Suma !!! 
            FI_s_y = ((1f / 64f) * MathF.fPI * MathF.Pow4(m_fd_s) + fA_s_1 * MathF.Pow2((fh / 2f) - Fa_s_t_y)) * m_iNo;
            FI_s_z = ((1f / 64f) * MathF.fPI * MathF.Pow4(m_fd_s) + fA_s_1 * MathF.Pow2((fb / 2f) - Fa_s_t_z)) * m_iNo;

            Fi_s_y = MathF.Sqrt(FI_s_y / FA_s_t);
            Fi_s_z = MathF.Sqrt(FI_s_z / FA_s_t);

            int iNo_Cut = 2; // Pocet strihov na v jednom reze prierezu 2 alebo 4 /2 alebo 4 strizny strmen
            float m_fA_s_s_1 = MathF.fPI * MathF.Pow2(m_fd_s_s) / 4; // Plocha prierezu pruta jedneho strmena / jeden prut smykovej vystuze
            m_fA_s_s = m_fA_s_s_1 * iNo_Cut; // Celkova plocha smykovej vystuze v reze

            FA = Fb * Fh;
            FA_c = FA - FA_s_t;

            // Moments of Inertia - whole cross-section
            FI_y = 1 / 12f * Fb * MathF.Pow3(Fh);
            FI_z = 1 / 12f * Fh * MathF.Pow3(Fb);

            Fu = 2 * Fb + 2 * fh; // obvod prierezu
        }
        // Metoda ktora sa spusti po stlaceni tlacidla calculate
        private void Calculate_Click(object sender, EventArgs e)
        {
            // Load Actual Input Data / Načítanie dat
            Load_data();

            // Vypocet pomocnych dat
            Calculate_Auxiliary();
            
            // Vypocet hlavnych vysledkov

            // Vytvori sa objekt triedy vypoctu
            EC2 objekt_EC2 = new EC2(

            m_fN_Ed,
            m_fM_Ed_1_y,
            m_fM_Ed_1_z,

            m_fM_0_1_y,
            m_fM_0_2_y,
            m_fM_0_1_z,
            m_fM_0_2_z,

            m_fN_0_Ed_qp,
            m_fM_0_Ed_qp_y,
            m_fM_0_Ed_qp_z,

            m_fM_Ed_1,
            m_fM_0_Ed_qp,

            Fb,
            Fh,
            Fa_s_t_y,
            Fa_s_t_z,
            FA,
            FA_s_t,
            FA_c,
            Fu,
            Fi_s_y,
            Fi_s_z,

            m_fn_bal,
            m_ft_0,
            m_ft,
            m_fRH,
            m_fT_Delta_t_i,
            m_fAlpha,
            m_fc_y,
            m_fc_z,

            m_fL_0_y,
            m_fL_0_z,

           m_objDatabase.m_ff_cm * 1e+6f,
           m_objDatabase.m_ff_ck * 1e+6f,
           m_objDatabase.m_fE_cm * 1e+9f,
           m_fLambda,
           m_fEta,
           m_fGamma_Mc,

           m_ff_yk * 1e+6f,
           m_fE_s * 1e+9f,
           m_fGamma_Ms);

            // Get Results
            sResults = objekt_EC2.sResults;

            // Check Results
            string sHeaderResultsError = "Resistance Error";
            string sTextResultsError = "Some resistance value or design ratio can't be calculated. Check input, please.";
            if ((!MathF.d_equal(m_fN_Ed,0) && sResults.m_fN_Rd <= 0f) ||
                (!MathF.d_equal(m_fM_Ed_1_y, 0) &&  sResults.m_fM_Rd_y <= 0f) ||
                (!MathF.d_equal(m_fM_Ed_1_z, 0) && sResults.m_fM_Rd_z <= 0f) ||
                (!MathF.d_equal(m_fM_Ed_1_y, 0) && sResults.m_fDesRatio1 < 0f) ||
                (!MathF.d_equal(m_fM_Ed_1_z, 0) && sResults.m_fDesRatio2 < 0f) ||
                (sResults.m_fDesRatio < 0f))
            {
                MessageBox.Show(sTextResultsError, sHeaderResultsError);
                return;
            }

            // Zapísanie výsledkov do READONLY textboxov
            Display_data();
        }
        // Metoda - Load data from textboxes
        // Tato metoda nacita udaje z textboxov a skonvertuje na cislo
        public void Load_data()
        {
            string sHeaderFormatError = "FORMAT ERROR";
            string sTextFormatError = "Wrong numerical format! Enter number, please.";
            try
            {
                // Internal Forces
                m_fN_Ed = (float)Convert.ToDouble(Value_N_Ed.Text.ToString());
                m_fM_Ed_1_y = (float)Convert.ToDouble(Value_M_Ed_1_y.Text.ToString());
                m_fM_Ed_1_z = (float)Convert.ToDouble(Value_M_Ed_1_z.Text.ToString());

                m_fN_0_Ed_qp = (float)Convert.ToDouble(Value_N_0_Ed_qp.Text.ToString());
                m_fM_0_Ed_qp_y = (float)Convert.ToDouble(Value_M_0_Ed_qp_y.Text.ToString());
                m_fM_0_Ed_qp_z = (float)Convert.ToDouble(Value_M_0_Ed_qp_z.Text.ToString());

                m_fM_0_1_y = (float)Convert.ToDouble(Value_M_0_1_y.Text.ToString());
                m_fM_0_1_z = (float)Convert.ToDouble(Value_M_0_1_z.Text.ToString());
                m_fM_0_2_y = (float)Convert.ToDouble(Value_M_0_2_y.Text.ToString());
                m_fM_0_2_z = (float)Convert.ToDouble(Value_M_0_2_z.Text.ToString());

                // Cross-Section
                fb = (float)Convert.ToDouble(Value_b.Text.ToString());
                fh = (float)Convert.ToDouble(Value_h.Text.ToString());

                // Member
                m_fL = (float)Convert.ToDouble(Value_L.Text.ToString());
                m_fBeta_y = (float)Convert.ToDouble(Value_Beta_cr_y.Text.ToString());
                m_fBeta_z = (float)Convert.ToDouble(Value_Beta_cr_z.Text.ToString());

                // Reinforcement
                m_fGamma_Ms = (float)Convert.ToDouble(Value_Gamma_Ms.Text.ToString());
                m_fs_s = (float)Convert.ToDouble(Value_ss.Text.ToString());
                m_ft_b = (float)Convert.ToDouble(Value_tb.Text.ToString());

                // Materials
                // Concrete settings
                m_fAlpha_cc = (float)Convert.ToDouble(Value_Alpha_cc.Text.ToString());
                m_fEta = (float)Convert.ToDouble(Value_Eta.Text.ToString());
                m_fLambda = (float)Convert.ToDouble(Value_Lambda.Text.ToString());
                m_fGamma_Mc = (float)Convert.ToDouble(Value_Gamma_Mc.Text.ToString());

                m_fn_bal = (float)Convert.ToDouble(Value_n_bal.Text.ToString());
                m_ft_0 = (float)Convert.ToDouble(Value_t_0.Text.ToString());
                m_ft = (float)Convert.ToDouble(Value_t.Text.ToString());
                m_fRH = (float)Convert.ToDouble(Value_RH.Text.ToString());
                m_fT_Delta_t_i = (float)Convert.ToDouble(Value_T_Delta_t_i.Text.ToString());
                m_fAlpha = (float)Convert.ToDouble(Value_Alpha.Text.ToString());
                m_fc_y = (float)Convert.ToDouble(Value_c_y.Text.ToString());
                m_fc_z = (float)Convert.ToDouble(Value_c_z.Text.ToString());
            }
            catch
            {
                MessageBox.Show(sTextFormatError, sHeaderFormatError);
                return;
            }
       }

        public void Default_data()
        {
            // EditBoxes

            // Internal Forces
            m_fN_Ed = 0f;
            m_fM_Ed_1_y = 0f;
            m_fM_Ed_1_z = 0f;

            m_fN_0_Ed_qp = 0f;
            m_fM_0_Ed_qp_y = 0f;
            m_fM_0_Ed_qp_z = 0f;

            m_fM_0_1_y = 0f;
            m_fM_0_1_z = 0f;
            m_fM_0_2_y = 0f;
            m_fM_0_2_z = 0f;


            // Cross -section Data
            fb = 0.5f;
            fh = 0.5f;

            // Member Data
            m_fL = 10f;
            m_fBeta_y = 1f;
            m_fBeta_z = 1f;

            // Reinforcement
            INo = 4; // Pocet aktivnych pozdlznych prutov jedneho typu - 1/4 - symetria  !!!!
            m_fs_s = 0.25f;
            m_ft_b = 0.02f;

            // Materials
            // Concrete settings
            // Temp - default
            m_fAlpha_cc = 1.0f;
            m_fEta = 1.0f;
            m_fLambda = 0.8f;
            m_fGamma_Mc = 1.5f;

            m_fn_bal = 0.4f;
            m_ft_0 = 28f; // Days
            m_ft = 25550f; // Days
            m_fRH = 50;
            m_fT_Delta_t_i = 20f;
            m_fAlpha = 0.0f;
            m_fc_y = 9f;
            m_fc_z = 9f;

            // Reinforcement
            m_fGamma_Ms = 1.15f;

            // ComboBoxes

            comboBox1_Concrete.SelectedIndex = 4;
            comboBox2_Reinfor.SelectedIndex = 4;
            comboBox2_Reinfor_Long_ds.SelectedIndex = 15;
            comboBox2_Reinfor_Tran_dss.SelectedIndex = 7;
            comboBox3_CrScShape.SelectedIndex = 1;
        }

        private void But_LoadTestData_Click(object sender, EventArgs e)
        {
            // Load Test Data acc. to xls file

            // EditBoxes

            // Internal Forces
            m_fN_Ed = -2880f;
            m_fM_Ed_1_y = 85f;
            m_fM_Ed_1_z = 50f;

            m_fN_0_Ed_qp = -800f;
            m_fM_0_Ed_qp_y = 23f;
            m_fM_0_Ed_qp_z = 15f;

            m_fM_0_1_y = 85f;
            m_fM_0_1_z = 85f;
            m_fM_0_2_y = 85f;
            m_fM_0_2_z = 85f;

            // Transformacia na N, Nm

            m_fN_Ed *= 1000f;
            m_fM_Ed_1_y *= 1000f;
            m_fM_Ed_1_z *= 1000f;

            m_fN_0_Ed_qp *= 1000f;
            m_fM_0_Ed_qp_y *= 1000f;
            m_fM_0_Ed_qp_z *= 1000f;

            m_fM_0_1_y *= 1000f;
            m_fM_0_1_z *= 1000f;
            m_fM_0_2_y *= 1000f;
            m_fM_0_2_z *= 1000f;

            // Cross -section Data
            fb = 0.4f;
            fh = 0.4f;

            // Member Data
            m_fL = 2.8f;
            m_fBeta_y = 1f;
            m_fBeta_z = 1f;

            // Reinforcement
            INo = 4; // Pocet aktivnych pozdlznych prutov jedneho typu - 1/4 - symetria  !!!!
            m_fs_s = 0.25f;
            m_ft_b = 0.0275f;

            // Materials
            // Concrete settings
            // Temp - default
            m_fAlpha_cc = 1.0f;
            m_fEta = 1.0f;
            m_fLambda = 0.8f;
            m_fGamma_Mc = 1.5f;

            m_fn_bal = 0.4f;
            m_ft_0 = 28f; // Days
            m_ft = 25550f; // Days
            m_fRH = 50;
            m_fT_Delta_t_i = 20f;
            m_fAlpha = 0.0f;
            m_fc_y = 9f;
            m_fc_z = 9f;

            // Reinforcement
            m_fGamma_Ms = 1.15f;

            // ComboBoxes

            comboBox1_Concrete.SelectedIndex = 4;
            comboBox2_Reinfor.SelectedIndex = 4;
            comboBox2_Reinfor_Long_ds.SelectedIndex = 15;
            comboBox2_Reinfor_Tran_dss.SelectedIndex = 7;
            comboBox3_CrScShape.SelectedIndex = 1;

            // Update display of Inputs
            Display_Input_Data();
        }

        // Metoda - Nastaví vypocitane hodnoty v textboxoch
        public void Display_data()
        {
            Update_IF_Data();
            Update_Member_Data();
            Update_Reinforcement_Long_Data();
            Update_Reinforcement_Trans_Data();
            Update_CrSc_Data();
            Update_Results_Data();
        }

        public void Display_Input_Data()
        {
            // Internal Forces
            Value_N_Ed.Text = m_fN_Ed.ToString();
            Value_M_Ed_1_y.Text = m_fM_Ed_1_y.ToString();
            Value_M_Ed_1_z.Text = m_fM_Ed_1_z.ToString();

            Value_N_0_Ed_qp.Text = m_fN_0_Ed_qp.ToString();
            Value_M_0_Ed_qp_y.Text = m_fM_0_Ed_qp_y.ToString();
            Value_M_0_Ed_qp_z.Text = m_fM_0_Ed_qp_z.ToString();

            Value_M_0_1_y.Text = m_fM_0_1_y.ToString();
            Value_M_0_1_z.Text = m_fM_0_1_z.ToString();
            Value_M_0_2_y.Text = m_fM_0_2_y.ToString();
            Value_M_0_2_z.Text = m_fM_0_2_z.ToString();

            // Cross -section Data
            Value_b.Text = fb.ToString();
            Value_h.Text = fh.ToString();

            // Member Data
            Value_L.Text = m_fL.ToString();
            Value_Beta_cr_y.Text = m_fBeta_y.ToString();
            Value_Beta_cr_z.Text = m_fBeta_z.ToString();

            // Reinforcement
            Value_Gamma_Ms.Text = m_fGamma_Ms.ToString();
            Value_ss.Text = m_fs_s.ToString();
            Value_tb.Text = m_ft_b.ToString();

            // Materials
            // Concrete settings
            Value_Alpha_cc.Text = m_fAlpha_cc.ToString();
            Value_Eta.Text = m_fEta.ToString();
            Value_Lambda.Text = m_fLambda.ToString();
            Value_Gamma_Mc.Text = m_fGamma_Mc.ToString();

            Value_n_bal.Text = m_fn_bal.ToString();
            Value_t_0.Text = m_ft_0.ToString();
            Value_t.Text = m_ft.ToString();
            Value_RH.Text = m_fRH.ToString();
            Value_T_Delta_t_i.Text = m_fT_Delta_t_i.ToString();
            Value_Alpha.Text = m_fAlpha.ToString();
            Value_c_y.Text = m_fc_y.ToString();
            Value_c_z.Text = m_fc_z.ToString();
        }

        public void Update_IF_Data()
        {
            // Internal Forces
            Value_M_Ed_1.Text = m_fM_Ed_1.ToString();
            Value_M_0_Ed_qp.Text = m_fM_0_Ed_qp.ToString();

            Value_e_Sd_y.Text = m_fe_Sd_y.ToString();
            Value_e_Sd_z.Text = m_fe_Sd_z.ToString();
        }

        public void Update_Member_Data()
        {
            // Member
            Value_L_y.Text = m_fL_0_y.ToString();
            Value_L_z.Text = m_fL_0_z.ToString();
        }

        public void Update_CrSc_Data()
        {
            // Cross-Section
            Value_A.Text = FA.ToString();
            Value_Iy.Text = FI_y.ToString();
            Value_Iz.Text = FI_z.ToString();

            Value_Ac.Text = FA_c.ToString();
        }

        public void Update_Concrete_Data()
        {
            // Concrete Database Data

            Value_f_ck.Text = m_objDatabase.m_ff_ck.ToString();
            Value_f_ck_cube.Text = m_objDatabase.m_ff_ck_cube.ToString();
            Value_f_cm.Text = m_objDatabase.m_ff_cm.ToString();
            Value_f_ctm.Text = m_objDatabase.m_ff_ctm.ToString();
            Value_E_cm.Text = m_objDatabase.m_fE_cm.ToString();
        }

        public void Update_Reinforcement_Data()
        {
            // Reinforcement Database Data

            Value_f_yk.Text = m_ff_yk.ToString();
            Value_f_uk.Text = m_ff_uk.ToString();
            Value_E_s.Text = m_fE_s.ToString();
            // Value_Gamma_Ms.Text = m_fGamma_Ms.ToString();
        }

        public void Update_Reinforcement_Long_Data()
        {
            // Longitudinal Reinforcement Data

            Value_Ast.Text = FA_s_t.ToString();
        }

        public void Update_Reinforcement_Trans_Data()
        {
            // Transversal Reinforcement Data

            Value_A_ss.Text = FA_s_s.ToString();
        }

        public void Update_Results_Data()
        {
            // Results
            Value_Lambda_y.Text = sResults.m_fLambda_y.ToString();
            Value_Lambda_z.Text = sResults.m_fLambda_z.ToString();

            Value_Lambda_y_lim.Text = sResults.m_fLambda_y_lim.ToString();
            Value_Lambda_z_lim.Text = sResults.m_fLambda_z_lim.ToString();

            Value_M_0_Ed_y.Text = sResults.m_fM_0_Ed_y.ToString();
            Value_M_0_Ed_z.Text = sResults.m_fM_0_Ed_z.ToString();

            Value_Beta_y_ef.Text = sResults.m_fBeta_y_ef.ToString();
            Value_Beta_z_ef.Text = sResults.m_fBeta_z_ef.ToString();

            Value_1_ry.Text = sResults.m_f1_r_y.ToString();
            Value_1_rz.Text = sResults.m_f1_r_z.ToString();

            Value_Phi_y_ef.Text = sResults.m_fPhi_y_ef.ToString();
            Value_Phi_z_ef.Text = sResults.m_fPhi_z_ef.ToString();

            Value_M_Ed_y.Text = sResults.m_fM_Ed_y.ToString();
            Value_M_Ed_z.Text = sResults.m_fM_Ed_z.ToString();

            Value_e_tot_y.Text = sResults.m_fe_tot_y.ToString();
            Value_e_tot_z.Text = sResults.m_fe_tot_z.ToString();

            Value_Phi_RH.Text = sResults.m_fPhi_RH.ToString();
            Value_Beta_H.Text = sResults.m_fBeta_H.ToString();
            Value_Phi_Inf_t0.Text = sResults.m_fPhi_Infinity_ft_0.ToString();

            Value_N_Rd.Text = sResults.m_fN_Rd.ToString();
            Value_N_eu.Text = sResults.m_fN_eu.ToString();

            Value_M_Rd_y.Text = sResults.m_fM_Rd_y.ToString();
            Value_M_Rd_z.Text = sResults.m_fM_Rd_z.ToString();

            Value_factor_a.Text = sResults.m_fa.ToString();

            Value_DesRatio1.Text = sResults.m_fDesRatio1.ToString();
            Value_DesRatio2.Text = sResults.m_fDesRatio2.ToString();
            Value_DesRatio.Text = sResults.m_fDesRatio.ToString();
        }


        private void ComboBox_Rein3_SelectedIndexChanged(object sender, EventArgs e)
        {
            INo = comboBox2_Reinfor_Long_Count.SelectedIndex;
            INo++; //Number of bars
        }

        private void comboBox1_Concrete_SelectedIndexChanged(object sender, EventArgs e)
        {
            m_objDatabase.m_ff_ck = m_objDatabase.Get_f_ck((short)comboBox1_Concrete.SelectedIndex);
            m_objDatabase.m_ff_ck_cube = m_objDatabase.Get_f_ck_cube((short)comboBox1_Concrete.SelectedIndex);
           
            m_objDatabase.GetConData();

            Update_Concrete_Data();
        }

        private void comboBox2_Reinfor_SelectedIndexChanged(object sender, EventArgs e)
        {
            m_ff_yk = m_objDatabase.Get_Reinf_f_yk((short)comboBox2_Reinfor.SelectedIndex);
            m_ff_uk = m_objDatabase.Get_Reinf_f_tk((short)comboBox2_Reinfor.SelectedIndex);
            m_fE_s = 200f; // 200 GPa

            Update_Reinforcement_Data();
        }

        private void comboBox2_Reinfor_Long_ds_SelectedIndexChanged(object sender, EventArgs e)
        {
            Fd_s = m_objDatabase.Get_Reinf_d_s((short)comboBox2_Reinfor_Long_ds.SelectedIndex);

            // Convert mm to m
            Fd_s /= 1000;

            Calculate_Auxiliary();
            Update_Reinforcement_Long_Data();
            Update_CrSc_Data(); // Value Ac is changed
        }

        private void comboBox2_Reinfor_Tran_dss_SelectedIndexChanged(object sender, EventArgs e)
        {
            Fd_s_s = m_objDatabase.Get_Reinf_d_s((short)comboBox2_Reinfor_Tran_dss.SelectedIndex);
            
            // Convert mm to m
            Fd_s_s /= 1000;

            Calculate_Auxiliary();
            Update_Reinforcement_Trans_Data();
        }

        private void comboBox3_CrScShape_SelectedIndexChanged(object sender, EventArgs e)
        {
            Calculate_Auxiliary();
            Update_CrSc_Data();
        }
    }
}
