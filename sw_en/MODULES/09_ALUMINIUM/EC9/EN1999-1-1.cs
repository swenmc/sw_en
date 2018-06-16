using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.OleDb;
using DATABASE;

namespace CENEX
{
    class EN1999_1_1
    {
        // Aluminium structures
        // Eurocode 9: Design of aluminium structures – Part 1-1: General structural rules

        #region Variables for Cross section - general

        // 1-rolled, 2-welded, 3-multi – cells; 4 - others
        int cstype = 1;

        // 1-single, 2-multi
        int csshape = 1;

        // Solid cross-section 101 – square, 102 – rectangle, 103 circle
        int cssolidshape = 101;

        // Welded cross-section
        int csweldedshape = 201;

        // 1-rolled, 2- welded, 3-rolled and welded
        int csproduction = 1;

        // Rolled single section 1-I and H, 2-U, 3-LR, 4-LN, 5-CHS, 6-SHS, 7-RHS, 8-I asymetrical, 9-T;
        int csprofshape = 1;

        public int Csprofshape
        {
            get { return csprofshape; }
            set { csprofshape = value; }
        }

        // Weight of whole cross-section (include all member for built-up sections)
        double cs_weight = 110;




        #endregion
        #region Variables from DATA1


        // VARIABLES DEFINITION




        #region Variables for Material
        // Table 3.2a
        // Material name - whole with all parameters
        // "." or "," is not allowed in variable´s definition, first letter must be small or _ !!!
        int mat_num;

        public int Mat_num
        {
            get { return mat_num; }
            set { mat_num = value; }
        }
        string mat_name;

        public string Mat_name
        {
            get { return mat_name; }
            set { mat_name = value; }
        }
        string en_aw_num;

        public string En_aw_num
        {
            get { return en_aw_num; }
            set { en_aw_num = value; }
        }
        string mat_shape;

        public string Mat_shape
        {
            get { return mat_shape; }
            set { mat_shape = value; }
        }
        string mat_temp;

        public string Mat_temp
        {
            get { return mat_temp; }
            set { mat_temp = value; }
        }
        string mat_max_t;

        public string Mat_max_t
        {
            get { return mat_max_t; }
            set { mat_max_t = value; }
        }
        double fo;

        public double Fo
        {
            get { return fo; }
            set { fo = value; }
        }
        double fu;

        public double Fu
        {
            get { return fu; }
            set { fu = value; }
        }
        double mat_A;

        public double Mat_A
        {
            get { return mat_A; }
            set { mat_A = value; }
        }
        double fo_haz;

        public double Fo_haz
        {
            get { return fo_haz; }
            set { fo_haz = value; }
        }
        double fu_haz;

        public double Fu_haz
        {
            get { return fu_haz; }
            set { fu_haz = value; }
        }
        double ro0_haz;

        public double Ro0_haz
        {
            get { return ro0_haz; }
            set { ro0_haz = value; }
        }


        double rou_haz;

        public double Rou_haz
        {
            get { return rou_haz; }
            set { rou_haz = value; }
        }
        string mat_BC = "A"; // auxiliary temporary 

        public string Mat_BC
        {
            get { return mat_BC; }
            set { mat_BC = value; }
        }
        double mat_np;

        public double Mat_np
        {
            get { return mat_np; }
            set { mat_np = value; }
        }
        double gamaM1;

        public double GamaM1
        {
            get { return gamaM1; }
            set { gamaM1 = value; }
        }
        double gamaM2;

        public double GamaM2
        {
            get { return gamaM2; }
            set { gamaM2 = value; }
        }
        double gamaM3;

        public double GamaM3
        {
            get { return gamaM3; }
            set { gamaM3 = value; }
        }

        // Clause 3.2.5
        double _E = 70000; // modul pruznosti
        double _G = 27000; // modul pruznosti ve smyku
        double mat_nu = 0.3; // soucinitel pricne deformace v pruzne oblasti
        double mat_alpha = 0.000023; // soucinitel delkove tepelne roztaznosti
        double mat_ro = 2700; // hustota


        double fo_w;

        public double Fo_w
        {
            get { return fo_w; }
            set { fo_w = value; }
        }
        double fo_f;

        public double Fo_f
        {
            get { return fo_f; }
            set { fo_f = value; }
        }






        #endregion


        #endregion
        #region Variables
        #region Variables - CLASSIFICATION

        double beta;

        double beta1;
        double beta2;
        double beta3;

        double beta1_eps;
        double beta2_eps;
        double beta3_eps;

        double epsilon;

        int classall;



        #endregion
        #region Variables - CROSS-SECTION GEOMETRY


        // Vertical dimensions
        double h;
        double hw;
        double hf; // axial distance between upper and bottom flange

        // Horizontal dimensions
        double b;
        double bf;
        double bf1;
        double bf2;
        double tw;
        double tf; // generally
        double tf1; // compresion flange - usually upper
        double tf2; // tesnion flange - usually bottom

        // Used for shar area calculation
        double d_hole;
        double b_haz;

        #endregion
        #region Variables - CROSS-SECTION


        double _A;

        public double A
        {
            get { return _A; }
            set { _A = value; }
        }
        double _Ag;

        public double Ag
        {
            get { return _Ag; }
            set { _Ag = value; }
        }
        double _Anet;

        public double Anet
        {
            get { return _Anet; }
            set { _Anet = value; }
        }
        double _Aeff;

        public double Aeff
        {
            get { return _Aeff; }
            set { _Aeff = value; }
        }

        double _Ae;

        public double Ae
        {
            get { return _Ae; }
            set { _Ae = value; }
        }
        double _Ahaz; // plocha tepelne ovlivnenych oblasti Tab. 6.5

        public double Ahaz
        {
            get { return _Ahaz; }
            set { _Ahaz = value; }
        }

        double _Ay_v;

        public double Ay_v
        {
            get { return _Ay_v; }
            set { _Ay_v = value; }
        }
        double _Az_v;

        public double Az_v
        {
            get { return _Az_v; }
            set { _Az_v = value; }
        }

        double _Aw;

        public double Aw
        {
            get { return _Aw; }
            set { _Aw = value; }
        }
        double _Af;

        public double Af
        {
            get { return _Af; }
            set { _Af = value; }
        }
        double _Af1;

        public double Af1
        {
            get { return _Af1; }
            set { _Af1 = value; }
        }
        double _Af2;

        public double Af2
        {
            get { return _Af2; }
            set { _Af2 = value; }
        }
        double _Afc;

        public double Afc
        {
            get { return _Afc; }
            set { _Afc = value; }
        }
        double _Aft;

        public double Aft
        {
            get { return _Aft; }
            set { _Aft = value; }
        }

        double eta_v; // eta_v and eta_w !!! check

        double _Wy_pl;

        public double Wy_pl
        {
            get { return _Wy_pl; }
            set { _Wy_pl = value; }
        }
        double _Wz_pl;

        public double Wz_pl
        {
            get { return _Wz_pl; }
            set { _Wz_pl = value; }
        }
        double _Wy_el;

        public double Wy_el
        {
            get { return _Wy_el; }
            set { _Wy_el = value; }
        }
        double _Wz_el;

        public double Wz_el
        {
            get { return _Wz_el; }
            set { _Wz_el = value; }
        }
        double _Wy_eff;

        public double Wy_eff
        {
            get { return _Wy_eff; }
            set { _Wy_eff = value; }
        }
        double _Wz_eff;

        double _Wy_pl_haz;

        public double Wy_pl_haz
        {
            get { return _Wy_pl_haz; }
            set { _Wy_pl_haz = value; }
        }
        double _Wz_pl_haz;

        public double Wz_pl_haz
        {
            get { return _Wz_pl_haz; }
            set { _Wz_pl_haz = value; }
        }
        double _Wy_el_haz;

        public double Wy_el_haz
        {
            get { return _Wy_el_haz; }
            set { _Wy_el_haz = value; }
        }
        double _Wz_el_haz;

        public double Wz_el_haz
        {
            get { return _Wz_el_haz; }
            set { _Wz_el_haz = value; }
        }
        double _Wy_eff_haz;

        public double Wy_eff_haz
        {
            get { return _Wy_eff_haz; }
            set { _Wy_eff_haz = value; }
        }
        double _Wz_eff_haz;

        public double Wz_eff_haz
        {
            get { return _Wz_eff_haz; }
            set { _Wz_eff_haz = value; }
        }

        double _Wy_net;

        public double Wy_net
        {
            get { return _Wy_net; }
            set { _Wy_net = value; }
        }
        double _Wz_net;

        public double Wz_net
        {
            get { return _Wz_net; }
            set { _Wz_net = value; }
        }

        double _Wt_pl; // torsion

        double _Wy;

        public double Wy
        {
            get { return _Wy; }
            set { _Wy = value; }
        }
        double _Wz;

        public double Wz
        {
            get { return _Wz; }
            set { _Wz = value; }
        }

        // Moment of inertia (Second moment of area) y-y and z-z
        double _Iy;

        public double Iy
        {
            get { return _Iy; }
            set { _Iy = value; }
        }
        double _Iz;

        public double Iz
        {
            get { return _Iz; }
            set { _Iz = value; }
        }

        // St. Venant torsional constant
        double _IT;

        public double IT
        {
            get { return _IT; }
            set { _IT = value; }
        }
        // Warping constant, Iw Vysecovy moment setrvacnosti
        double _Iw;

        public double Iw
        {
            get { return _Iw; }
            set { _Iw = value; }
        }
        // Modul odporu prierezu v kruteni
        double _Wt;

        public double Wt
        {
            get { return _Wt; }
            set { _Wt = value; }
        }


        // Radius of inertia y-y and z-z

        double iy_r;

        public double Iy_r
        {
            get { return iy_r; }
            set { iy_r = value; }
        }
        double iz_r;

        public double Iz_r
        {
            get { return iz_r; }
            set { iz_r = value; }
        }

        // Buckling NcrT and NcrTF calculation
        // for symmetrical cross-section y0 and z0 = 0;
        double y0 = 0;
        double z0 = 0;

        double i0_r;


        double alpha64y;

        public double Alpha64y
        {
            get { return alpha64y; }
            set { alpha64y = value; }
        }
        double alpha64z;

        public double Alpha64z
        {
            get { return alpha64z; }
            set { alpha64z = value; }
        }




        #endregion
        #region Variables - USER INPUT OR FEM SOLVER
        #region Variables - internal forces


        // Internal forces in beam cross-section
        double _NEd;
        double _NEd_t;
        double _NEd_p;
        double _Vy_Ed;
        double _Vz_Ed; // usually _VEd in code
        double _Mx_Ed;

        public double N_Ed
        {
            get { return _NEd; }
            set { _NEd = value; }
        }

        public double N_Ed_t
        {
            get { return _NEd_t; }
            set { _NEd_t = value; }
        }
        public double N_Ed_p
        {
            get { return _NEd_p; }
            set { _NEd_p = value; }
        }
        public double Vy_Ed
        {
            get { return _Vy_Ed; }
            set { _Vy_Ed = value; }
        }
        public double Vz_Ed
        {
            get { return _Vz_Ed; }
            set { _Vz_Ed = value; }
        }


        public double Mx_Ed
        {
            get { return _Mx_Ed; }
            set { _Mx_Ed = value; }
        }

        double _My_Ed;

        public double My_Ed
        {
            get { return _My_Ed; }
            set { _My_Ed = value; }
        }
        double _Mz_Ed;

        public double Mz_Ed
        {
            get { return _Mz_Ed; }
            set { _Mz_Ed = value; }
        }


        // Shear stress - torsion

        double tau_t_Ed;
        double tau_w_Ed;

        // End bending moments of member
        double _My_a;

        public double My_a
        {
            get { return _My_a; }
            set { _My_a = value; }
        }
        double _My_b;
        public double My_b
        {
            get { return _My_b; }
            set { _My_b = value; }
        }
        double _Mz_a;
        public double Mz_a
        {
            get { return _Mz_a; }
            set { _Mz_a = value; }
        }
        double _Mz_b;
        public double Mz_b
        {
            get { return _Mz_b; }
            set { _Mz_b = value; }
        }

        // transverse load
        double _FEd;

        public double FEd
        {
            get { return _FEd; }
            set { _FEd = value; }
        }

        #endregion
        #region Beam properties - length

        double _L_teor;

        public double L_teor
        {
            get { return _L_teor; }
            set { _L_teor = value; }
        }
        double _L_y_buck;

        public double L_y_buck
        {
            get { return _L_y_buck; }
            set { _L_y_buck = value; }
        }
        double _L_z_buck;

        public double L_z_buck
        {
            get { return _L_z_buck; }
            set { _L_z_buck = value; }
        }
        double _L_y_LT;

        public double L_y_LT
        {
            get { return _L_y_LT; }
            set { _L_y_LT = value; }
        }
        double _L_z_LT;

        public double L_z_LT
        {
            get { return _L_z_LT; }
            set { _L_z_LT = value; }
        }
        // Torsion
        double _L_T;

        public double L_T
        {
            get { return _L_T; }
            set { _L_T = value; }
        }
        #endregion

        #endregion
        #region Variables EN1999_1_1
        #region Variables _N


        double _No_Rd;
        double _Nu_Rd;
        double _Nu_Rdb;
        double _Nu_Rdc;
        double _Nt_Rd;

        double _Nc_Rd;
        double _NRd;

        double _Nb_Rd;
        double _Nb_y_Rd;
        double _Nb_z_Rd;
        double _Ny_cr;
        double _Nz_cr;
        double _N_cr_TF;
        double _N_cr_T;

        #endregion
        #region Variables _V

        double _Vy_Rd;
        double _Vz_Rd; // usually same as _VRd in code
        double _Vy_pl_Rd;
        double _Vz_pl_Rd;

        double _Vy_T_Rd;
        double _Vz_T_Rd;



        double _Vz_w_Rd;// Interaction - Section 6.7.6

        public double Vz_w_Rd
        {
            get { return _Vz_w_Rd; }
            set { _Vz_w_Rd = value; }
        }


        double _Vt_Rd; // torsion

        double fo_V; // used in Section 6.2.8 Bending and shear



        // page 100
        double ro_v;
        double lambda_w;
        double eta_w;

        #endregion
        #region Variables _T

        double _TRd;
        double _TEd;
        double _Tt_Ed;
        double _Tw_Ed;






        #endregion
        #region Variables _M


        double _My_u_Rd;

        public double My_u_Rd
        {
            get { return _My_u_Rd; }
            set { _My_u_Rd = value; }
        }
        double _Mz_u_Rd;

        public double Mz_u_Rd
        {
            get { return _Mz_u_Rd; }
            set { _Mz_u_Rd = value; }
        }
        double _My_c_Rd;

        public double My_c_Rd
        {
            get { return _My_c_Rd; }
            set { _My_c_Rd = value; }
        }
        double _Mz_c_Rd;

        public double Mz_c_Rd
        {
            get { return _Mz_c_Rd; }
            set { _Mz_c_Rd = value; }
        }
        double _My_el_Rd;

        public double My_el_Rd
        {
            get { return _My_el_Rd; }
            set { _My_el_Rd = value; }
        }
        double _Mz_el_Rd;

        public double Mz_el_Rd
        {
            get { return _Mz_el_Rd; }
            set { _Mz_el_Rd = value; }
        }
        double _My_pl_Rd;

        public double My_pl_Rd
        {
            get { return _My_pl_Rd; }
            set { _My_pl_Rd = value; }
        }
        double _Mz_pl_Rd;

        public double Mz_pl_Rd
        {
            get { return _Mz_pl_Rd; }
            set { _Mz_pl_Rd = value; }
        }
        double _My_v_Rd;

        public double My_v_Rd
        {
            get { return _My_v_Rd; }
            set { _My_v_Rd = value; }
        }
        double _Mz_v_Rd;

        public double Mz_v_Rd
        {
            get { return _Mz_v_Rd; }
            set { _Mz_v_Rd = value; }
        }
        double _My_Rd;

        public double My_Rd
        {
            get { return _My_Rd; }
            set { _My_Rd = value; }
        }
        double _Mz_Rd;

        public double Mz_Rd
        {
            get { return _Mz_Rd; }
            set { _Mz_Rd = value; }
        }

        double _M_b_Rd; // code
        double _My_b_Rd;

        public double My_b_Rd
        {
            get { return _My_b_Rd; }
            set { _My_b_Rd = value; }
        }

        // Factors
        double alpha_y;

        public double Alpha_y
        {
            get { return alpha_y; }
            set { alpha_y = value; }
        }
        double alpha_z;

        public double Alpha_z
        {
            get { return alpha_z; }
            set { alpha_z = value; }
        }

        // Lateral buckling - critical moment

        double _My_cr; // see  Annex I.1

        public double My_cr
        {
            get { return _My_cr; }
            set { _My_cr = value; }
        }

        // Interaction - Section 6.7.6

        double _My_f_Rd;

        public double My_f_Rd
        {
            get { return _My_f_Rd; }
            set { _My_f_Rd = value; }
        }




        #endregion
        #region Variables - buckling (auxiliary)

        double fi_y_buck;
        double fi_z_buck;
        double chi_y_buck;
        public double Chi_y_buck
        {
            get { return chi_y_buck; }
            set { chi_y_buck = value; }
        }

        double chi_z_buck;

        public double Chi_z_buck
        {
            get { return chi_z_buck; }
            set { chi_z_buck = value; }
        }
        double chi_T_buck;
        public double Chi_T_buck
        {
            get { return chi_T_buck; }
            set { chi_T_buck = value; }
        }

        double lambda_y_buck_rel;
        double lambda_z_buck_rel;

        // general 

        double alpha_buck;

        public double Alfa_buck
        {
            get { return alpha_buck; }
            set { alpha_buck = value; }
        }
        double fi_buck;

        public double Fi_buck
        {
            get { return fi_buck; }
            set { fi_buck = value; }
        }
        double chi_buck;

        public double Chi_buck
        {
            get { return chi_buck; }
            set { chi_buck = value; }
        }


        double omega_0; // see 6.2.9.3

        public double Omega_0
        {
            get { return omega_0; }
            set { omega_0 = value; }
        }

        double eta_0;

        public double Eta_0
        {
            get { return eta_0; }
            set { eta_0 = value; }
        }
        double gama_0;

        public double Gama_0
        {
            get { return gama_0; }
            set { gama_0 = value; }
        }
        double xi_0;

        public double Xi_0
        {
            get { return xi_0; }
            set { xi_0 = value; }
        }
        #endregion
        #region Variables - lateral buckling (auxiliary)

        double chi_LT;
        double alpha_LT;
        double lambda_y_LT_rel;
        double lambda_y_LT0_rel;
        double chi_y_LT;

        double alpha_y_LT;
        double beta_y_LT;
        double fi_y_LT;




        #endregion
        #endregion
        #endregion
        #region Constructor

        //Konstruktor - loading data from  database using SQL command
        //material name is parameter of constructor method public EN1999_1_1
        //coonection to database

        OleDbDataReader dat_reader;
        DatabaseConnection dat_conn;

        public EN1999_1_1(string combo1)
        {

            string sql1;
            // string sql2;

            sql1 = "Select mat_num, mat_name, en_aw_num, mat_shape, mat_temp, mat_max_t, fo, fu, mat_A, fo_haz, fu_haz, ro0_haz, rou_haz, mat_BC,mat_np, gamaM1, gamaM2, gamaM3 from ALUMINIUM_32a where mat_name like '" + combo1 + "'";

            #region Database DATA-ALUMINIUM variables reader
            dat_conn = DatabaseConnection.getInstance();
            dat_reader = dat_conn.getDBReader(sql1);

            try
            {
                while (dat_reader.Read())
                {
                    // ALUMINIUM DATABASE DATA

                    #region Data list
                    try
                    {
                        mat_num = Convert.ToInt16(dat_reader.GetValue(0).ToString());
                    }
                    catch (FormatException) { }
                    try
                    {
                        mat_name = dat_reader.GetValue(1).ToString();
                    }
                    catch (FormatException) { }
                    try
                    {
                        en_aw_num = dat_reader.GetValue(2).ToString();
                    }
                    catch (FormatException) { }
                    try
                    {
                        mat_shape = dat_reader.GetValue(3).ToString();
                    }
                    catch (FormatException) { }
                    try
                    {
                        mat_temp = dat_reader.GetValue(4).ToString();
                    }
                    catch (FormatException) { }
                    try
                    {
                        mat_max_t = dat_reader.GetValue(5).ToString();
                    }
                    catch (FormatException) { }
                    try
                    {
                        fo = Convert.ToDouble(dat_reader.GetValue(6).ToString());
                    }
                    catch (FormatException) { }
                    try
                    {
                        fu = Convert.ToDouble(dat_reader.GetValue(7).ToString());
                    }
                    catch (FormatException) { }
                    try
                    {
                        mat_A = Convert.ToDouble(dat_reader.GetValue(8).ToString());
                    }
                    catch (FormatException) { }
                    try
                    {
                        fo_haz = Convert.ToDouble(dat_reader.GetValue(9).ToString());
                    }
                    catch (FormatException) { }
                    try
                    {
                        fu_haz = Convert.ToDouble(dat_reader.GetValue(10).ToString());
                    }
                    catch (FormatException) { }
                    try
                    {
                        ro0_haz = Convert.ToDouble(dat_reader.GetValue(11).ToString());
                    }
                    catch (FormatException) { }
                    try
                    {
                        rou_haz = Convert.ToDouble(dat_reader.GetValue(12).ToString());
                    }
                    catch (FormatException) { }
                    try
                    {
                        mat_BC = dat_reader.GetValue(13).ToString();
                    }
                    catch (FormatException) { }
                    try
                    {
                        mat_np = Convert.ToDouble(dat_reader.GetValue(14).ToString());
                    }
                    catch (FormatException) { }
                    try
                    {
                        gamaM1 = Convert.ToDouble(dat_reader.GetValue(15).ToString());
                    }
                    catch (FormatException) { }
                    try
                    {
                        gamaM2 = Convert.ToDouble(dat_reader.GetValue(16).ToString());
                    }
                    catch (FormatException) { }
                    try
                    {
                        gamaM3 = Convert.ToDouble(dat_reader.GetValue(17).ToString());
                    }
                    catch (FormatException) { }

                }
            }
            catch (FormatException) { }


                    #endregion

            #endregion




        }

        #endregion
        #region Cross-section data - auxiliary calcutation
        public void CS_data()
        {

            // Section 6.2.6 Shear / Smyk
            // Shear area
            // a) prurezy se stojinami ve smyku (I, H, U, L, T, ....)
            Ay_v = (hw - d_hole * tw - (1 - Ro0_haz) * b_haz * tw); // !!!!!!!!!!!!!!!!! INPUT DATA FOR HOLES AND WELDING !!!!!

            // b) prurezy plne a kruhove trubky

            if (csprofshape == 5)

                eta_v = 0.6; // circular hollow sections
            eta_v = 0.8; // solid sections

            Ay_v = eta_v * Ae;
            Az_v = eta_v * Ae;

        }
        #endregion
        #region Section 6.1 Generally
        public void EN_1999_1_1()
        {

            // 6.1.4.3 Parametry stihlosti
            double sigma;

            double b = 1; // not assigned
            double c = 1; // not assigned
            double t = 1; // not assigned
            double eta = 1; // not assigned
            double psi; // pomer napeti

            beta = eta * b / t; // (6.6)

            eta = 1 / (1 + 0.1 * Math.Pow((c / t - 1), 2)); // (6.7a)
            eta = Math.Max(0.5, 1 / (1 + 2.5 * (Math.Pow((c / t - 1), 2)) / (b / t))); // (6.7b)
            eta = Math.Max(0.33, 1 / (1 + 4.5 * (Math.Pow((c / t - 1), 2)) / (b / t))); // (6.7c)

            // c) Tvar 1 - rovnomerne napeti v tlaku, slozite vystuzeni, neobvykle tvary

            double sigma_cr0 = 1; // not assigned
            double sigma_cr = 1; // not assigned

            beta = b / t * Math.Pow((sigma_cr0 / sigma_cr), 0.4); // (6.8)
            // d) Tvar 1 nerovnomerne napeti v tlaku
            // e) Tvar 2 
            double _R = 1; // not assigned


            beta = b / t * (1 / (Math.Sqrt(1 + 0.006 * (Math.Pow(b, 4) / (_R * _R * t * t))))); // (6.9)

            // (5)
            double _D = 1; // not assigned
            beta = 3 * Math.Sqrt(_D / t);

        }

        // Table 6.2
        public void Table_62(double[] args)
        {
            double[,] tab = new double[4, 6] 
      {
      {11, 16, 22, 3, 4.5, 6},
      {9, 13, 18, 2.5, 4, 5},
      {13, 16.5, 18, 3.5, 4.5, 5},
      {10, 13.5, 15, 3, 3.5, 4} 
      };
            double beta = tab[1, 2]; // Auxiliary value

            // Class A (according Table 3.2) without welds
            // Internal part
            // Outstanding part
            // Class A (according Table 3.2) with welds








        }
        public void Epsilon()
        {



            // Method for Table definition????

            epsilon = Math.Sqrt(250 / fo);

            // (4) for not max

            double z1 = 1; // not assigned
            double z2 = 1; // not assigned

            epsilon = Math.Sqrt((250 / fo) * (z1 / z2));

        }
        // 6.1.5 Unosnost pri lokalnim bouleni
        public void Table_63(double[] args)
        {
            double ro_c;


            // Table 6.3 C1 and C2
            double[,] tab = new double[4, 4] 
      {
      {32, 220, 10, 24},
      {29, 198, 9, 20},
      {29, 198, 9, 20},
      {25, 150, 8, 16} 
      };
            double _C1_ro = tab[1, 1]; // Auxiliary value;
            double _C2_ro = tab[3, 1]; // Auxiliary value;
            //
            // Formulas (6.11) and (6.12)
            if (beta <= beta3_eps) ro_c = 1;
            else ro_c = (_C1_ro / (beta / epsilon)) - (_C2_ro / Math.Pow((beta / epsilon), 2));
        }
        // Section 6.1.6.2 Zavaznost oslabeni
        public void Table_63()
        {
            Ro0_haz = fo_haz / fo; // (6.13)
            rou_haz = fu_haz / fu; // (6.14)
        }
        // Clause 6.1.6.3 Rozsah tepelne ovlivnene oblasti
        #endregion
        #region Section 6.2 Resistance / Unosnost prurezu
        public void EN1999_1_1_62()
        {
            #region Section 6.2.1 General / Vseobecne
            // 6.2.1 General / Vseobecne

            double sigma_x_Ed = 10; // Auxiliary value
            double sigma_z_Ed = 10;// Auxiliary value
            double tau_Ed = 10;// Auxiliary value
            double _C_NP14 = 1.2; // see NAD / NP 14


            double left_site_615 = Math.Pow((sigma_x_Ed / (fo / gamaM1)), 2) + Math.Pow((sigma_z_Ed / (fo / gamaM1)), 2) - (sigma_x_Ed / (fo / gamaM1)) * (sigma_z_Ed / (fo / gamaM1)) + 3 * Math.Pow((tau_Ed / (fo / gamaM1)), 2); // (6.15)
            double left_site_615a = sigma_x_Ed / (fo / gamaM1); // (6.15a)
            double left_site_615b = sigma_z_Ed / (fo / gamaM1); // (6.15b)
            double left_site_615c = (Math.Sqrt(3) * tau_Ed) / (fo / gamaM1); // (6.15c)
            #endregion
            #region Section 6.2.2 Section properties / Vlastnosti prurezu
            // 6.2.2.1
            // 6.2.2.2
            #endregion
            #region Section 6.2.3 Tension (p. 66)
            // Section 6.2.3 Tension / Tah (p. 66)


            // a) The design plastic resistance of the cross-section (6.18)
            _No_Rd = (_Ag * fo) / gamaM1; // (6.18)
            // b) The design ultimate resistance of the net cross-section
            _Nu_Rdb = (0.9 * Anet * fu) / gamaM2; // (6.19a)
            // c) The design tension resistance Nt,Rd 
            _Nu_Rdc = (Aeff * fu) / gamaM2; // (6.19b)

            // The design tension resistance
            _Nt_Rd = Math.Min(_No_Rd, _Nu_Rdb);
            _Nt_Rd = Math.Min(_Nt_Rd, _Nu_Rdc);

            // Shape L, T, U section see 6.3.1.5 !!!


            #endregion
            #region Section 6.2.4 Compression (p. 67)
            // Section 6.2.4 Compression /Tlak (p. 67)

            // The design resistance of the cross-section for uniform compression
            // a)
            _Nu_Rd = Anet * fo / gamaM2; // (6.21)
            // b)
            _Nc_Rd = Aeff * fo / gamaM1; // (6.22)

            _Nc_Rd = Math.Min(_Nu_Rd, _Nc_Rd);

            #endregion
            #region Section 6.2.5 Bending moment (p. 67)
            // Section 6.2.5 Bending moment / Ohybovy moment (p. 67)
            // The design resistance for bending



            double alpha64y_u;
            double alpha64z_u;

            double alpha64y_w;
            double alpha64z_w;

            // Formula (6.26) and (6.27)

            // 2 alternatives
            // First
            double alpha64y_3u = 1;
            double alpha64z_3u = 1;

            double alpha64y_3w = Wy_el_haz / Wy_el;
            double alpha64z_3w = Wz_el_haz / Wz_el;

            // Second

            alpha64y_3u = (1 + ((beta3 - beta) / (beta3_eps - beta2_eps)) * ((Wy_pl / Wy_el) - 1)); // (6.26)
            alpha64z_3u = (1 + ((beta3 - beta) / (beta3_eps - beta2_eps)) * ((Wz_pl / Wz_el) - 1)); // (6.26)

            alpha64y_3w = ((Wy_el_haz / Wy_el) + ((beta3 - beta) / (beta3_eps - beta2_eps)) * ((Wy_pl_haz - Wy_el_haz) / Wy_el)); // (6.27)
            alpha64z_3w = ((Wz_el_haz / Wz_el) + ((beta3 - beta) / (beta3_eps - beta2_eps)) * ((Wz_pl_haz - Wz_el_haz) / Wz_el)); // (6.27)






            // Table 6.4 Value of alpha  / Hodnoty tvaroveho soucinitele alpha 





            if (classall == 1) alpha64y_u = Wy_pl / Wy_el; // (6.25 and Table 6.4)
            if (classall == 2) alpha64y_u = Wy_pl / Wy_el; // (6.25 and Table 6.4)
            if (classall == 3) alpha64y_u = alpha64y_3u; // alpha = 1 or (6.25 and Table 6.4)
            if (classall == 4) alpha64y_u = Wy_eff / Wy_el; // (6.25 and Table 6.4)

            if (classall == 1) alpha64z_u = Wz_pl / Wz_el;
            if (classall == 2) alpha64z_u = Wz_pl / Wz_el;
            if (classall == 3) alpha64z_u = alpha64z_3u;
            if (classall == 4) alpha64z_u = _Wz_eff / Wz_el;

            if (classall == 1) alpha64y_w = Wy_pl_haz / Wy_el; // (6.25 and Table 6.4)
            if (classall == 2) alpha64y_w = Wy_pl_haz / Wy_el; // (6.25 and Table 6.4)
            if (classall == 3) alpha64y_w = alpha64y_3w; // alpha = 1 or (6.25 and Table 6.4)
            if (classall == 4) alpha64y_w = Wy_eff_haz / Wy_el; // (6.25 and Table 6.4)

            if (classall == 1) alpha64z_w = Wz_pl_haz / Wz_el;
            if (classall == 2) alpha64z_w = Wz_pl_haz / Wz_el;
            if (classall == 3) alpha64z_w = alpha64z_3w;
            if (classall == 4) alpha64z_w = Wz_eff_haz / Wz_el;

            // Auxiliary 

            byte comboboxindex = 0; //  !!!!!!!!!! COMBOBOX IS NOT ASSIGNED YET !!!!!!!!!

            if (comboboxindex == 0) // auxiliary
            {
                Alpha64y = alpha64y_u = 1;// auxiliary
                Alpha64z = alpha64z_u = 1;// auxiliary
            }
            else
            {
                Alpha64y = alpha64y_w = 1;// auxiliary
                Alpha64z = alpha64z_w = 1;// auxiliary
            }
            _My_u_Rd = Wy_net * fu / gamaM2; // (6.24)
            _Mz_u_Rd = Wz_net * fu / gamaM2; // (6.24)

            _My_c_Rd = Alpha64y * Wy_el * fo / gamaM1; // (6.25)
            _Mz_c_Rd = Alpha64z * Wz_el * fo / gamaM1; // (6.25)


            // Section 6.2.5.2    Navrhovy prurez
            #endregion
            #region Section 6.2.6 Shear / Smyk (p. 69)

            // (2)

            if (hw / tw < 39 * epsilon)
            {
                _Vy_Rd = Ay_v * fo / ((Math.Sqrt(3)) * gamaM1);
                _Vz_Rd = Az_v * fo / ((Math.Sqrt(3)) * gamaM1);
            }
            else
            {


                // see Sections 6.7.4 - 6.7.6


            }
            #endregion
            #region Section 6.2.7 Torsion / Krouceni (p. 70)
            #region Section 6.2.7.1 Torsion  without deplanation / Krouceni bez deplanace
            _TRd = _Wt_pl * fo / (Math.Sqrt(3) * gamaM1);

            // Section 6.2.7.2 Torsion with deplanation / Krouceni s deplanaci

            _TEd = _Tt_Ed + _Tw_Ed; // (6.33)

            // Section 6.2.7.3 Kombinace smykove sily a krouticiho momentu

            // I and H sections

            _Vt_Rd = (Math.Sqrt(1 - (tau_t_Ed * Math.Sqrt(3)) / (1.25 * fo / gamaM1))) * _Vz_Rd; // (6.35) 

            // U section

            _Vt_Rd = (Math.Sqrt(1 - (tau_t_Ed * Math.Sqrt(3)) / (1.25 * fo / gamaM1)) - ((tau_w_Ed * Math.Sqrt(3)) / (fo / gamaM1))) * _Vz_Rd; // (6.36)

            // closed / hollow sections CHS, SHS, RHS etc.

            _Vt_Rd = (1 - (tau_t_Ed * Math.Sqrt(3)) / (fo / gamaM1)) * _Vz_Rd; // (6.37) see 6.2.6 for _Vz_Rd
            #endregion
            #endregion
            #region Section 6.2.8 Bending and shear / Ohyb a smyk

            fo_V = fo * (1 - Math.Pow((2 * _Vz_Ed / _Vz_Rd - 1), 2)); // (6.38)

            // (4) I symmetrical class 1 or 2

            _My_v_Rd = tf * bf * (h - tf) * fo / gamaM1 + ((tw * hw * hw) / 4) * (fo_V / gamaM1); // (6.39)
            #endregion
            #region Section 6.2.9 Bending and axial force / Ohyb a osoba sila

            // Section 6.2.9.1 Opened cross-sections / Otevrene prurezy

            // (1)

            _NRd = Aeff * fo / gamaM1;
            _My_Rd = alpha_y * Wy_el * fo / gamaM1;
            _Mz_Rd = alpha_z * Wz_el * fo / gamaM1;

            omega_0 = 1; // see 6.2.9.3

            // Default factors // CHECK IT !!!! 
            eta_0 = 1;
            gama_0 = 1;
            xi_0 = 1;

            // Alternative factors

            // alpha_y; // see 6.2.5
            // alpha_z; // see 6.2.5

            eta_0 = Math.Pow(alpha_z, 2) * Math.Pow(alpha_y, 2); // (6.42a)
            eta_0 = Math.Max(1, eta_0);
            eta_0 = Math.Min(eta_0, 2);

            gama_0 = Math.Pow(alpha_z, 2); // (6.42b)
            gama_0 = Math.Max(1, gama_0);
            gama_0 = Math.Min(gama_0, 1.56);

            xi_0 = Math.Pow(alpha_y, 2); // (6.42c)
            xi_0 = Math.Max(1, xi_0);
            xi_0 = Math.Min(xi_0, 2);


            // Section 6.2.9.2 Hollow sections and solid sections / Dute a plne prurezy

            // psi factor

            double psi = 1.3; // hollow setions
            psi = 2; // solid sections

            // ALternatively

            psi = alpha_y * alpha_z;
            psi = Math.Max(1, psi);
            psi = Math.Min(psi, 1.3); // hollow sections
            psi = Math.Min(psi, 1.2); // solid sections



            #endregion
            #region Section 6.2.10 Bending, shear and axial force / Ohyb, smyk a osova sila

            double ro_V_z = Math.Pow(((2 * _Vz_Ed / _Vz_Rd) - 1), 2); // (6.46)
            if (_Vz_Ed > 0.5 * _Vz_pl_Rd) fo = (1 - ro_V_z) * fo; // (6.46)




            #endregion
        }
        #endregion
        #region Section 6.3 Buckling resistance of beams / Vzperna unosnost prutu
        public void EN1999_1_1_63()
        {
            #region Section 6.3.1 Uniform members in compression / Tlacene pruty
            // (1) Prut vystaveny osovemu tlaku muze selhat jednim ze tri zpusobu:
            // i) rovinnym vybocenim (viz 6.3.1.1. az 6.3.1.3)
            // j) zkroucenim nebo prostorovym vybocenim (viz 6.3.1.1. az 6.3.1.4)
            // k) lokalni plastifikaci (viz 6.2.4)

            #region Section 6.3.1.1 Buckling resistance / Vzperna unosnost

            double omega_x = 1; // !!!! just temporary, see 6.3.3.3
            double chi_buck; // generally for any buckling mode

            double kappa; // longitudinal welds see Table 6.5
            kappa = 1; // prostorovy vzper
            kappa = omega_x; // see 6.3.3.3 - beams with transverse welds









            if (classall <= 3) Aeff = _A;
            _Nb_y_Rd = kappa * chi_y_buck * Aeff * fo / gamaM1; // (6.49)
            _Nb_z_Rd = kappa * chi_z_buck * Aeff * fo / gamaM1; // (6.49)

            _Nb_Rd = Math.Min(_Nb_y_Rd, _Nb_z_Rd);



            // Section 6.3.1.2 Buckling curves / Krivky vzperne pevnosti

            // Critical force
            _Ny_cr = (Math.PI * Math.PI * _Iy * _E) / Math.Pow(_L_y_buck, 2);
            _Nz_cr = (Math.PI * Math.PI * _Iz * _E) / Math.Pow(_L_z_buck, 2);


            // 6.3.1.3 Slenderness for flexural buckling

            /*
            if (classall <= 3) lambda_y_buck_rel = Math.Sqrt((_A * fo) / _Ny_cr); // (6.51)
            else lambda_y_buck_rel = Math.Sqrt((_Aeff * fy) / _Ny_cr);

            if (classall <= 3) lambda_z_buck_rel = Math.Sqrt((_A * fy) / _Nz_cr); // (6.51)
            else lambda_z_buck_rel = Math.Sqrt((_Aeff * fo) / _Nz_cr);
            */


            lambda_y_buck_rel = (_L_y_buck / iy_r) * (1 / Math.PI) * Math.Sqrt((Aeff / _A) * (fo / _E)); // (6.52)
            lambda_z_buck_rel = (_L_z_buck / iz_r) * (1 / Math.PI) * Math.Sqrt((Aeff / _A) * (fo / _E)); // (6.52)



            double lambda_buck_rel = Math.Max(lambda_y_buck_rel, lambda_z_buck_rel); // maximum value of relative slenderness 


            // Section 6.3.1.4 Slenderness for torsional and torsional-flexural buckling

            // SEE ANNEX I

            double lambda_T_buck_rel;

            if (classall <= 3) lambda_T_buck_rel = Math.Sqrt((_A * fo) / (Math.Min(_N_cr_TF, _N_cr_T))); // (6.53)  !!! _Aeff according to Table 6.7 !!!
            else lambda_T_buck_rel = Math.Sqrt((Aeff * fo) / (Math.Min(_N_cr_TF, _N_cr_T)));

            // Local variables definition

            double alpha_y_buck;
            double alpha_z_buck = 0.1; // Auxiliary temporary value

            double fi_T_buck;
            double alpha_T_buck = alpha_z_buck; // check it!!!







            fi_buck = 0.5 * (1 + alpha_buck * (lambda_buck_rel - 0.2) + Math.Pow(lambda_buck_rel, 2));
            chi_buck = 1 / (fi_buck + Math.Sqrt(Math.Pow(fi_buck, 2) - Math.Pow(lambda_buck_rel, 2)));
            if (chi_buck > 1) chi_buck = 1;



            // Table 6.5

            // Material vzpernosti tridy A podle 3.2
            if (mat_BC.Equals("A"))
            {
                double ro_haz = 1; // temporary numerical  value // ???? ro0_haz ????
                double _A1 = _A - Ahaz * (ro_haz);
                kappa = 1 - (1 - (_A1 / _A)) * Math.Pow(10, -lambda_buck_rel) - (0.05 + 0.1 * (_A1 / _A)) * Math.Pow(lambda_buck_rel, 1.3 * (1 - lambda_buck_rel));
            }

            // Material vzpernosti tridy B podle 3.2
            if (mat_BC.Equals("B"))
            {
                if (lambda_buck_rel <= 0.2) kappa = 1;
                else kappa = 1 + 0.04 * Math.Pow((4 * lambda_buck_rel), 0.5 - lambda_buck_rel) - 0.22 * Math.Pow((lambda_buck_rel), 1.4 * (1 - lambda_buck_rel));
            }
            // Table 6.6 

            // Buckling class A / Vzpernostni trida A (Table 3.2)
            if (mat_BC.Equals("A"))
            {
                double alpha = 0.2;
                double lambda_buck_rel0 = 0.1;
            }
            // Buckling class B / Vzpernostni trida B (Table 3.2)
            if (mat_BC.Equals("B"))
            {
                double alpha = 0.32;
                double lambda_buck_rel0 = 0;
            }

            // Table 6.7 

            // Cross-section - general (obecne) - z vyztuzenych presahujich casti
            // if (all shapes) 
            {
                double alpha = 0.35;
                double lambda_buck_rel0 = 0.4;
                // _Aeff;
            }
            // Cross-section - bond ??? T , angles L, X sections
            if (csprofshape == 3 || csprofshape == 4 || csprofshape == 9)
            {
                double alpha = 0.35;
                double lambda_buck_rel0 = 0.4;
                Aeff = _A;
            }


            #endregion
            #endregion
            #region Section 6.3.2 Uniform members in bending / Ohybane pruty
            #region Section 6.3.2.1 Lateral torsional buckling resistance / Unosnost v klopeni

            // (2)

            _My_b_Rd = chi_y_LT * Alpha64y * Wy_el * fo / gamaM1;
            // alpha - see Table 6.4

            #region Section 6.3.2.2

            if (classall == 1 || classall == 2)
            {
                alpha_LT = 0.1; // p. 78
                lambda_y_LT_rel = 0.6;
            }
            else
            {
                alpha_LT = 0.2;
                lambda_y_LT_rel = 0.4;
            }

            fi_y_LT = 0.5 * (1 + alpha_y_LT * (lambda_y_LT_rel - lambda_y_LT0_rel) + beta_y_LT * Math.Pow(lambda_y_LT_rel, 2));
            chi_y_LT = 1 / (fi_y_LT + Math.Sqrt(Math.Pow(fi_y_LT, 2) - beta_y_LT * Math.Pow(lambda_y_LT_rel, 2))); // (6.56)
            if (chi_y_LT > 1) chi_y_LT = 1;
            if (chi_y_LT > 1 / Math.Pow(lambda_y_LT_rel, 2)) chi_y_LT = 1 / Math.Pow(lambda_y_LT_rel, 2);



            #region Section 6.3.2.3 Slenderness

            if (Alpha64y > Wy_pl / Wy_el) Alpha64y = Wy_pl / Wy_el;
            lambda_y_LT_rel = Math.Sqrt((Alpha64y * fo) / _My_cr);

            // See Annex I.1 and I.2 / viz Priloha I.1 a I.2


            #endregion

            #endregion













            #endregion
            #endregion
            #region Section 6.3.3 Uniform members in bending and axial compression / Pruty v ohybu a osovem tlaku
            #region Section 6.3.3.1 Flexural buckling / Rovinny vzper

            // (1) Prut otevreneho prurezu a dvema osami symetrie 

            // (6.61a)  (6.61b) (6.61c)
            double eta_c = 0.8;
            double xi_yc = 0.8;
            double xi_zc = 0.8;

            // Alternative

            eta_c = Eta_0 * chi_z_buck;
            eta_c = Math.Max(eta_c, 0.8);

            xi_yc = Xi_0 * chi_y_buck;
            xi_yc = Math.Max(xi_yc, 0.8);

            xi_zc = Xi_0 * chi_y_buck;
            xi_zc = Math.Max(xi_yc, 0.8);

            // for eta_0 and xi_0 see 6.2.9.1

            omega_x = Omega_0 = 1; // pruty namahane ohybem a tlakem bez lokalizovanych svaru a pro stejne momenty na obou koncich, jinak viz 6.3.3.3 popripade 6.3.3.4
            // pro ohyb k hlavní ose větsi tuhosti (ose y)

            double ratio_659y = (Math.Pow((_NEd_p / (chi_y_buck * omega_x * _NRd)), xi_yc) + (_My_Ed / (Omega_0 * _My_Rd))) / 1; // (6.59)

            // pro ohyb k hlavní ose menší tuhosti (ose z)

            double ratio_660z = (Math.Pow((_NEd_p / (chi_z_buck * omega_x * _NRd)), eta_c) + Math.Pow((_Mz_Ed / (Omega_0 * _Mz_Rd)), xi_zc)) / 1; // (6.60)



            // (2) 


            // Pro plne prurezy lze pouzit (6.60) s exponentem 0.8 nebo 

            eta_c = Math.Max(2 * chi_buck, 0.8); // (6.61d) value chi ?????
            double xi_c = Math.Max(1.56 * chi_buck, 0.8); // (6.61e) value chi ?????

            // (3) Pro dute prurezy a trubky

            double chi_buck_min = Math.Min(chi_y_buck, chi_z_buck);

            double psi_c = 0.8;

            // alternative
            // podle smeru vyboceni

            if (chi_y_buck < chi_z_buck) psi_c = 1.3 * chi_y_buck;
            else psi_c = 1.3 * chi_z_buck;
            psi_c = Math.Max(psi_c, 0.8);

            double ratio_662 = (Math.Pow((_NEd_p / (chi_buck_min * omega_x * _NRd)), psi_c) + (1 / Omega_0) * Math.Pow((Math.Pow((_My_Ed / _My_Rd), 1.7) + Math.Pow((_Mz_Ed / _Mz_Rd), 1.7)), 0.6)) / 1; // (6.62)

            // (4) Pro otevrene prurezy s jednou osou symetrie lze pri ohybu k jedne z hlavních os pouzit vztah (6.59), kde se pouzije xi_zc, _Mz_Rd, chi_z_buck

            // (5) promenne ve vztazich (6.59) az (6.62)

            if (classall <= 3) _NRd = _A * fo / gamaM1;
            else _NRd = Aeff * fo / gamaM1;


            // pruty s podelnymi svary ale bez lokalizovanych svaru (p. 80)

            if (classall <= 3) _NRd = kappa * _A * fo / gamaM1;
            else _NRd = kappa * Aeff * fo / gamaM1;

            alpha_y = Math.Min(1.25, alpha_y);
            alpha_z = Math.Min(1.25, alpha_z);

            _My_Rd = alpha_y * _Wy * fo / gamaM1;
            _Mz_Rd = alpha_z * _Wz * fo / gamaM1;



            #endregion
            #region Section 6.3.3.2 Lateral flexural buckling / Klopeni

            //(1) Pruty s otevrenym prurezem symetrickym k hlavni ose vetsi tuhosti, osove symetricke nebo se dvema osami symetrie


            eta_c = 0.8;
            xi_zc = 0.8;
            // Alternatively
            eta_c = Math.Max(Eta_0 * chi_z_buck, 0.8);
            double gama_c = Gama_0;
            xi_zc = Math.Max(Xi_0 * chi_z_buck, 0.8);

            // see 6.2.9.1
            // omega - soucinitele oslabeni tepelne ovlivnenymi oblastmi, viz 6.3.3.3, pro posuzovany prurez viz 6.3.3.5
            double omega_x_LT = 1; // auxiliary temporary value

            double ratio_663 = Math.Pow(_NEd_p / (chi_z_buck * omega_x * _NRd), eta_c) + Math.Pow((_My_Ed / (chi_LT * omega_x_LT * _My_Rd)), gama_c) + Math.Pow(_Mz_Ed / (Omega_0 * _Mz_Rd), xi_zc);

            // (2) Ma byt splnena podminka 6.3.3.1


            #endregion
            #region Section 6.3.3.3 Uniform members with localised welds / Pruty s lokalizovanymi svary


            Omega_0 = omega_x = omega_x_LT = Math.Min((rou_haz * fu / gamaM2) / (fo / gamaM1), 1); // (6.64)


            //  (2) Je-li oslabeni tepelne ovlivnenymi oblastmi pouze pobliz koncu prutu nebo pobliz inflexnich bodu

            double _L_c = 1000; // auxiliary temporary value

            double x_s = 10; // auxiliary temporary value


            omega_x = Omega_0 / (chi_buck + (1 - chi_buck) * Math.Sin((Math.PI * x_s) / _L_c)); // (6.65)
            omega_x_LT = Omega_0 / (chi_LT + (1 - chi_LT) * Math.Sin((Math.PI * x_s) / _L_c)); // (6.66)
            Omega_0 = Math.Min((rou_haz * fu / gamaM2) / fo / gamaM1, 1); // (6.67)

            // (3) Vypocet chi_buck (chi_y_buck nebo chi_z_buck) a chi_LT v prurezu s lokalizovanym svarem se ma provest pro mez pevnosti tepelne ovlivneneho materialu a pro pomernou stihlost

            double lambda_y_buck_haz_rel = lambda_y_buck_rel * Math.Sqrt(Omega_0); // (6.68a)
            double lambda_z_buck_haz_rel = lambda_z_buck_rel * Math.Sqrt(Omega_0); // (6.68a)
            double lambda_y_LT_haz_rel = lambda_y_LT_rel * Math.Sqrt(Omega_0); // (6.68b)

            // (4) Pro celku oslabene oblasti vetsi nez je mensi z sirek plechu prurezu se souceinitel rou_haz pro mistni poruseni ve vztazich ..... ro







            #endregion
            #region Section 6.3.3.4 Uniform members with localised reduction of cross-section / Pruty s lokalizovanou redukci prurezu

            // ENTER CODE


            #endregion
            #region Section 6.3.3.5 Unequal end moments or transverse load

            // (1) 

            omega_x = 1 / (chi_buck + (1 - chi_buck) * Math.Sin((Math.PI * x_s) / _L_c)); // (6.69)
            omega_x_LT = 1 / (chi_LT + (1 - chi_LT) * Math.Sin((Math.PI * x_s) / _L_c)); // (6.70)

            // x_s  - see picture 6.14 (p. 82)

            // (2)

            // x_s for End moments

            double cos_x_s; // auxiliary

            // End moments variables
            double _My_Ed_1 = _My_a;
            double _My_Ed_2 = _My_b;
            double _Mz_Ed_1 = _Mz_a;
            double _Mz_Ed_2 = _Mz_b;

            cos_x_s = (_My_Ed_1 - _My_Ed_2) / _My_Rd * _NRd / _NEd_p * 1 / (1 / (chi_buck - 1)); // (6.71)
            x_s = Math.Max((_L_c * Math.Acos(cos_x_s)) / Math.PI, 0); // (6.71)







            #endregion
            #endregion
        }
        #endregion
        #region Section 6.4 Uniform built-up compression members / Clenene tlacene pruty staleho prurezu
        public void EN1999_1_1_64()
        {


            // CODE p. 82 - 87

        }
        #endregion
        #region// 6.5 Webs without stiffeners


        #endregion
        #region// 6.6 Webs with stiffeners



        #endregion
        #region Section 6.7 Compact cross-section / Plnostenne nosniky
        public void EN1999_1_1_67()
        {
            #region Section 6.7.1 Generally / Vseobecne


            #endregion
            #region Section 6.7.2 Resistance of uniform member in plane bending / Unosnost nosniku pri ohybu v rovine

            double _My_o_Rd = 0.1; // auxiliary temporary value
            double _Mz_o_Rd = 0.1; // auxiliary temporary value


            double ratio_6115y = _My_Ed / _My_o_Rd; // (6.115)
            double ratio_6115z = _Mz_Ed / _Mz_o_Rd; // (6.115)


            _My_o_Rd = _Wy_net * fo / gamaM1; // (6.116)
            _Mz_o_Rd = _Wz_net * fo / gamaM1; // (6.116)

            // _Wnet - ro0_haz * t - see 6.1.6.2

            // (4)

            _My_c_Rd = _Wy_eff * fo / gamaM1; // (6.117)
            _Mz_c_Rd = _Wz_eff * fo / gamaM1; // (6.117)

            // _Weff - ro0_haz * t - see 6.2.5



            #endregion
            #region Section 6.7.3 Resistance of uniform member with longitudinal stiffeners / Unosnost nosniku s podelnymi vyztuhami steny

            // (6) 

            double lambda_st_buck_rel;
            double _Ist = 1; // temporary numerical value - just auxiliary
            double b1 = 1; // temporary numerical value - just auxiliary
            double b2 = 1; // temporary numerical value - just auxiliary
            double bw = 0.5; // temporary numerical value - just auxiliary
            double a = 1; // temporary numerical value - just auxiliary
            double ac = 0.9; // temporary numerical value - just auxiliary
            double _Ast_eff = 0.1; // temporary numerical value - just auxiliary
            double _Ncr = 0.1; // temporary numerical value - just auxiliary

            lambda_st_buck_rel = Math.Sqrt((_Ast_eff * fo) / _Ncr); // (6.118)

            _Ncr = 1.05 * _E * (Math.Sqrt(_Ist * tw * tw * tw * bw) / (b1 * b2)); // (6.119)
            _Ncr = ((Math.PI * _E * _Ist) / (a * a)) + (_E * tw * tw * tw * bw * a * a) / (4 * Math.PI * Math.PI * (1 - mat_nu * mat_nu) * (b1 * b1 * b2 * b2)); // (6.120)
            ac = 4.33 * Math.Pow((_Ist * b1 * b1 * b2 * b2) / (tw * tw * tw * bw), 1 / 4); // (6.121)

            #endregion
            #region Section 6.7.4 Resistance in shear / Unosnost ve smyku

            double ratio_6122ay = _Vy_Ed / _Vy_Rd; // missing number of formula p. 98
            double ratio_6122az = _Vz_Ed / _Vz_Rd;

            #region Section 6.7.4.1 Plnostenny nosnik s vyztuhami pouze v podporach

            // (2) Na bouleni ve smyku se maji posoudit steny s

            if (hw / tw > (2.37 / eta_0) * Math.Sqrt(_E / fo))
            {


                // Table 6.13 - soucinitel ro_v pro bouleni ve smyku p. 99
                // see main variables declaration

                // (3) Stihlostni pomer viz obr. 6.28 a (6.123)

                lambda_w = 0.35 * bw / tw * Math.Sqrt(fo / _E); // (6.123)


                double fa_w = fo; // yield stress - web - default - same grade of material for all CS parts
                double fo_w = fo; // yield stress - total collapse

                eta_w = Math.Max(0.7 + 0.35 * fa_w / fo_w, 1.2);

                // ro_v definition accordint to Table 6.13
                // stiffeners 
                // rigidity of stiffeners
                byte end_stiffener_rig = 1; // 1 - rigid, 2 - not-rigid
                if (end_stiffener_rig == 1)
                {
                    // rigid stffener
                    if (lambda_w < 0.83 / eta_w) ro_v = eta_w;
                    else if (0.83 / eta_w <= lambda_w && lambda_w < 0.937) ro_v = 0.83 / lambda_w;
                    else ro_v = 2.3 / (1.66 + lambda_w);
                }
                else
                {
                    //non-rigid stiffener
                    if (lambda_w < 0.83 / eta_w) ro_v = eta_w;
                    else if (0.83 / eta_w <= lambda_w && lambda_w < 0.937) ro_v = 0.83 / lambda_w;
                    else ro_v = ro_v = 0.83 / lambda_w;
                }
                // Posouzeni stojiny nosniku na bouleni
                // (3) Pro stojiny s pricnymi vyztuhami pouze v podporach  - navrhova unosnost ve smyku

                _Vz_Rd = ro_v * tw * hw * fo / (Math.Sqrt(3) * gamaM1); // (6.122)


            }
            #endregion
            #region Section 6.7.4.2 Plnostenny nosnik s mezilehlymi vyztuhami


            double k_tau = 0.1; // auxiliary temporary value
            double k_tau_st = 0.1; // auxiliary temporary value



            double _Wz_w_Rd = 0.1; // auxiliary temporary value
            double _Wz_t_Rd = 0.1; // auxiliary temporary value





            // (2) Na bouleni od smyku se maji posoudit steny s pricnymi vyztuhami v podporach a s pomerem

            // Number of transverse stiffeners
            int stif_tran_n = 2; // auxiliary temporary value
            // Number of longitudinal stiffeners
            int stif_long_n = 0; // auxiliary temporary value

            if (hw / tw > (1.02 / eta_w) * Math.Sqrt((k_tau * _E / fo)))
            {
                //(3)
                _Vz_Rd = _Wz_w_Rd + _Wz_t_Rd; // (6.124)
                // (4)
                _Vz_Rd = ro_v * tw * hw * fo / (Math.Sqrt(3) * gamaM1);
                // (5)  
                lambda_w = (0.81 / Math.Sqrt(k_tau)) * bw / tw * Math.Sqrt(fo / _E); // (6.126)
                // (9)

                double a_max;
                double k_tau1;
                double bw1;
                k_tau_st = 0;

                a_max = Math.Max(a, 0); // Maximum distance between transverse stiffeners
                bw1 = Math.Max(b, 0); // Maximum distance between longitudinal stiffeners
                k_tau1 = 5.34 + 4 * Math.Pow((bw / a), 2) + k_tau_st; // (6.127)
                k_tau1 = 4 + 5.34 * Math.Pow((bw / a), 2) + k_tau_st; // (6.128)


                if (stif_long_n != 0)
                    lambda_w = Math.Min(lambda_w, 0.81 / Math.Sqrt(k_tau1) * bw1 / tw * Math.Sqrt(fo / _E)); // (6.130) 


                // (7)
                k_tau = 5.34 + 4 * Math.Pow((bw / a), 2) + k_tau_st; // (6.127)
                k_tau = 4 + 5.34 * Math.Pow((bw / a), 2) + k_tau_st; // (6.128)
                k_tau_st = Math.Min(9 * Math.Pow(bw / a, 2) * Math.Pow(_Ist / (tw * tw * tw * bw), 3 / 4), 2.1 / tw * Math.Pow(_Ist / bw, 1 / 3)); // (6.129)
                // (8)
                if (stif_long_n <= 2 && a / bw >= 3) k_tau_st = Math.Min(9 * Math.Pow(bw / a, 2) * Math.Pow(_Ist / (tw * tw * tw * bw), 3 / 4), 2.1 / tw * Math.Pow(_Ist / bw, 1 / 3)); // (6.129)
                else if (stif_long_n <= 2 && a / bw < 3) k_tau = 4.1 + ((6.3 + (0.18 * _Ist / (tw * tw * tw * bw))) / Math.Pow(a, 2)) + 2.2 * (Math.Pow(_Ist / (tw * tw * tw * bw), 1 / 3)); // (6.129)
                // (10)
                // see Pictures 6.32
                double _Vz_f_Rd;
                double _My_f_Rd = 2; // auxiliary temporary value
                bf = Math.Min(bf, 15 * tw + 15 * tw + tw); // for I or H,T, for U, Z 15 * tw +tw,
                double c = a * (0.08 + ((4.4 * bf * tf * tf * fo_f) / (tw * bw * bw * fo_w))); // (6.131a)
                if (_My_Ed < _My_f_Rd) _Vz_f_Rd = (bf * tf * tf * fo_f / (c * gamaM1)) * (1 - (_My_Ed / _My_f_Rd));  // (6.131)
                // (11)
                double _Af1 = 2; // auxiliary temporary value
                double _Af2 = 2; // auxiliary temporary value
                if (_NEd != 0) _My_f_Rd = _My_f_Rd * (1 - _NEd / ((_Af1 + _Af2) * fo_f / gamaM1)); // (6.132)
                if (_My_Ed > _My_f_Rd) _Vz_f_Rd = 0; // see Section 6.7.6


            }

            #endregion
            #endregion
            #region Section 6.7.5 Resistance in transverse forces / Unosnost na pricne sily

            // (2)

            // a) Zatizena je jedna pasnice a zatizeni je preneseno smykovymi silami ve stojine, viz obrazek 6.30 (a);
            // b) Zatizena je jedna pasnice a zatizeni se stojinou prenasi do druhe pasnice, viz obrazek (b);
            // c) Zatizena je jedna pasnice pobliz nevyztuzeneho konce, viz obrazek 6.30 (c);

            // USER INPUT (p. 99)
            // Picture 6.30 / Obrazek 6.30 - Pusobeni zatizeni a soucinitele bouleni

            byte pict_630_n = 1; // a) = 0; b) =1; c) = 2

            // Picture 6.31 / Obrazek 6.31 - Delka tuheho roznosu

            byte pict_631_n = 1; // 0 - 5

            double _Leff;
            double _Ly;
            double chi_F;
            double _FRd;
            double k_F = 0.1; // temporary numerical value - just auxiliary
            double _Fcr;
            double lambda_F = 0.1; // temporary numerical value - just auxiliary




            // Section 6.7.5.3 Delka tuheho roznosu

            double ss = 1; // temporary numerical value - just auxiliary
            double c_end = 1; // temporary numerical value - just auxiliary     // in code c - see Picture 6.30 c)

            // Section 6.7.5.5 Effective loading length / Ucinna zatezovaci delka

            // (1)
            double m1 = (fo_f * bf) / (fo_w * tw); // (6.141)
            // u komorovych nosniku je bf v (6.141) omezeno 15 tf na kazde strane nosniku

            double m2;
            if (lambda_F > 0.5) m2 = 0.02 * Math.Pow(hw / tf, 2); // (6.142)
            else m2 = 0;
            // (2)
            if (pict_630_n == 0 || pict_630_n == 1)
                _Ly = Math.Min(ss + 2 * tf * (1 + Math.Sqrt(m1 + m2)), a); // (6.143) _Ly je maximalne rovna vzdalenosti medzi sousednimi pricnymi vyztuhami "a"
            else
            // (3)
            {
                if (pict_631_n == 5) ss = 0;
                double _Le = Math.Min((k_F * _E * tw * tw) / (2 * fo_w * hw), ss + c_end); // (6.146)

                // _Ly is minimum value from (6.143) (6.144) (6.145)

                _Ly = Math.Min(ss + 2 * tf * (1 + Math.Sqrt(m1 + m2)), a);
                _Ly = Math.Min(_Ly, _Le + tf * Math.Sqrt(m1 / 2 + Math.Pow(_Le / tf, 2) + m2)); // (6.144)
                _Ly = Math.Min(_Ly, _Le + tf * Math.Sqrt(m1 + m2)); // (6.145)
            }


            // Section 6.7.5.4 Reduction factor for resistance chi_F  / Redukcni soucinitel chi_F pro unosnost



            // (2)
            if (stif_long_n == 0)
            {
                if (pict_630_n == 0) k_F = 6 + 2 * Math.Pow(bw / a, 2); // Picture 6.30 a)
                else if (pict_630_n == 1) k_F = 3.5 + 2 * Math.Pow(bw / a, 2); // Picture 6.30 b)
                else if (pict_630_n == 2) k_F = Math.Min(2 + 6 * (ss + c_end) / bw, 6); // Picture 6.30 c)
            }
            // (3)
            else
            {
                double b1_panel = 1; // b1 in (6.140)
                double gama_s = 0.1; // temporary numerical value - just auxiliary

                if (0.05 <= b1_panel / hw && b1_panel / hw <= 0.3 && pict_630_n == 0) gama_s = Math.Min(10.9 * _Ist / (hw * tw * tw * tw), 13 * Math.Pow(a / hw, 3) + 210 * (0.3 - (b1_panel / hw))); // (6.140)
                k_F = 6 + 2 * Math.Pow((hw / a), 2) + (5.44 * b1_panel / a - 0.21) * Math.Sqrt(gama_s); // (6.139)
            }

            // (1)
            _Fcr = 0.9 * k_F * _E * tw * tw * tw / hw; // (6.138)
            lambda_F = Math.Sqrt(_Ly * tw * fo_w / _Fcr); // (6.138)
            chi_F = Math.Min(0.5 / lambda_F, 1); // (6.136)        


            // Section 6.7.5.2 Design resistance / Navrhova unosnost
            // (1)
            _Leff = chi_F * _Ly; // (6.135)
            _FRd = _Leff * tw * fo_w / gamaM1; // (6.134)
            // (4) 
            double ratio_6133 = FEd / _FRd; // (6.133)

            #endregion
            #region Section 6.7.6 Interaction / Interakce
            //6.7.6.1 Interaction between shear force, bending moment and axial force / Interakce mezi smykovou silou, ohybovym momentem a osovou silou
            // (2)
            double ratio_6147 = (((_My_Ed + My_f_Rd) / (2 * _My_pl_Rd)) + (_Vz_Ed / Vz_w_Rd) * (1 - (My_f_Rd / _My_pl_Rd))) / 1; // (6.147)
            // (3)
            double _My_N_Rd;
            if (_NEd != 0) _My_N_Rd = _My_pl_Rd * (1 - Math.Pow((_NEd / ((Af1 + Af2) * fo / gamaM1)), 2)); // (6.148)
            // 6.7.6.2 Interaction between transverse force, bending moment and axial force / Interakce mezi pricnou silou, ohybovym momentem a osovou silou 
            // (1)
            double ratio_6149 = (_FEd / _FRd + 0.8 * (_My_Ed / _My_c_Rd + _NEd / _Nc_Rd)) / 1.4; // (6.149)
            // (2)
            #endregion
            #region Section 6.7.7 Distortional buckling due to flanges bending / Bouleni od ohybu pasnic
            //(1)
            double k;
            if (classall == 1) k = 0.3;
            else if (classall == 2) k = 0.4;
            else if (classall == 3) k = 0.5;
            else k = 0.5;

            double ratio_6150 = (bw / tw) / ((k * _E / fo_f) * Math.Sqrt(_Aw / _Afc)); // (6.150)

            // (2)

            // radius
            double r;

            double ratio_6151 = (bw / tw) / ((k * _E / fo_f) * Math.Sqrt(_Aw / _Afc) * 1 / (1 + (bw * _E) / (3 * ratio_6115y * fo_f))); // (6.151)
            // (3)

            if (stif_tran_n >= 2)
            {

                ratio_6150 = ratio_6150 * (1 + Math.Pow(bw / a, 2));
                ratio_6151 = ratio_6151 * (1 + Math.Pow(bw / a, 2));
            }
            #endregion
            #region Section 6.7.8 Web stiffeners / Vyztuhy stojiny
            // 6.7.8.1 Rigid end stiffener / Tuha koncova vyztuha

            // (5) Koncova vyztuha ma mit prurezovou plochu nejmene
            // 4 * hf * tw^2 /e
            //e > 0.1 * hf;

            // (6)

            double _Iep = 0.1; // temporary numerical value - just auxiliary
            double _Rz_Ed = 0.1; // temporary numerical value - just auxiliary
            double _Wz_Ed = 0.1; // temporary numerical value - just auxiliary

            double ratio_6152 = (bw * bw * bw * tf * _Rz_Ed / (250 * _Wz_Ed)) / _Iep; // (6.152)

            // 6.7.8.2 Non-rigid end stiffener / Netuha koncova vyztuha

            // (1) Netuhou koncovou výztuhu může tvořit jedna oboustranná výztuha podle obrázku 6.27(c). Může působit
            //     jako podporová výztuha přenášející reakci do uložení nosníku (viz 6.2.11).
            // (2) Únosnost šroubovaného přípoje podle obrázku 6.27(d) lze pokládat pro vzdálenost mezi šrouby je p < 40tw
            //     za stejnou s únosností netuhé koncové výztuhy.

            // 6.7.8.3 Intermediate transverse stiffeners / Mezilehle pricne vyztuhy

            // (3)

            // _Ist;
            double ratio_6153 = 0.5; // auxiliary temporary value;
            double ratio_6154 = 0.5; // auxiliary temporary value;

            if (a / hw < Math.Sqrt(2)) ratio_6153 = ((1.5 * Math.Pow(hw, 3) * Math.Pow(tw, 3)) / a * a) / _Ist; // (6.153)
            if (a / hw >= Math.Sqrt(2)) ratio_6154 = ((0.75 * hw * Math.Pow(tw, 3))) / _Ist; // (6.154)
            
            double ratio_6783;
            if (a / hw < Math.Sqrt(2)) ratio_6783 = ratio_6153;
            else ratio_6783 = ratio_6154;

            double fv = fo; // For same material grade as main part of member
            // ro_v !!!!!!!! new solution
            double _Vz_Ed_stif = _Vz_Ed - ro_v * bw * tw * fv / gamaM1; // without formula number

            // 6.7.8.4 Longitudinal stiffeners / Podelne vyztuhy

            // (1) lambda_w - see 6.7.4.2 (5)

            // (2)

            // (3)

            // 6.7.8.5 Welds / Svary

            // (1)

            double shear_flow_weld;
            
            if (_Vz_Ed <= (ro_v * hw *tw *fo) / (Math.Sqrt(3))) shear_flow_weld = _Vz_Ed / hw;

            else shear_flow_weld = eta_w * tw * fo /( (Math.Sqrt(3))* gamaM1); // check eta_w !!!! ??????




            
            
            
            
            #endregion
        }
        #endregion
        #region Section 6.8 Members with corrugated web / Pruty s tvarovanou stojinou
         public void EN1999_1_1_68()
        {
        #region Section 6.8.1 Bending moment of resistance / Ohybovy moment unosnosti

        // (1) Ohybovy moment lze urcit ze vztahu

            double sigma_xMz = 200; // auxiliary temporary value; // napeti v pasnici od Mz - pricny ohybovy moment v pasnici !! pouze tlacene


        double ro_z = 1 - 0.4 * Math.Sqrt((sigma_xMz / (fo/gamaM1))); // (6.156) 
        double fo_r = ro_z * fo; // zahrnuje redukci kvuli pricnym momentum v pasnicich

        double _My_Rd_6155a = (bf2*tf2 *hf * fo_r) / gamaM1; // tension flange / tazena pasnice
        double _My_Rd_6155b = (bf1*tf1 *hf * fo_r) / gamaM1; // compression flange / tlacena pasnice
        double _My_Rd_6155c = (bf1*tf1 *hf * chi_LT * fo_r) / gamaM1; // compression flange / tlacena pasnice
        
        _My_Rd = Math.Min(_My_Rd_6155b, _My_Rd_6155c); // min _My_Rd for compresion flange
        _My_Rd = Math.Min(_My_Rd_6155a, _My_Rd);; // min _My_Rd according to (6.155)

        // for chi_y_LT or chi_LT see (6.3.2)
        
        #endregion

            #region Section 6.8.2 Shear resistance / Unosnost ve smyku


// Variables - see picture 6.33

        double a_max =100; // auxiliary temporary value
        double a = 135; // auxiliary temporary value
        double a0 = 100; // auxiliary temporary value
        double a1 = 100; // auxiliary temporary value
        double a2 = 40; // auxiliary temporary value
        double a3 = 15; // auxiliary temporary value
        double _Ix = 0.5; // auxiliary temporary value
        double ro_c = 0.5; // auxiliary temporary value    

// (2) Redukcni soucinitel pro lokalni bouleni

             // pomerna stihlost
        double lambda_c_l = 0.35 * a_max / tw * Math.Sqrt(fo / _E); // (6.159)

        double ro_c_l = Math.Min(1.15 / (0.9 + lambda_c_l), 1); // (6.158)

// (3) Redukcni soucinitel pro celkove bouleni

        double _Bx = ((2 * a) / (a0 + a1 + 2 * a2)) * ((_E * Math.Pow(tw, 3)) / 10.9);
        double _Bz = (_E * _Ix) / (2 * a);


             // kriticke smykove napeti

             double tau_cr_g = (32.4/(tw * hw *hw)) * Math.Pow(_Bx * Math.Pow(_Bz,3),1/4); // (6.162)

             // pomerna stihlost
             double lambda_c_g = Math.Sqrt(fo / Math.Sqrt(3) * tau_cr_g); // (6.161)

        double ro_c_g = Math.Min(1.5 / (0.5 + lambda_c_g * lambda_c_g), 1); // (6.160)

// (4) Redukcni soucinitel oslabeni ucinkem tepelne ovlivnene oblasti je uveden v 6.1.6




// (1)

        _Vz_Rd = ro_c * tw * hw * fo / ((Math.Sqrt(3)) * gamaM1); // (6.157)

           #endregion

        }

        #endregion


        // ANNEXES

        // Annex I - Informative - see class EN1999-1-1Annex_I

    }




        }

