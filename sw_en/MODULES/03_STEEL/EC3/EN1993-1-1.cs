using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.OleDb;
using System.Windows.Forms;
using DATABASE;

namespace CENEX
{
    public class EN1993_1_1
    {
        

        #region Variables from DATA1


        // VARIABLES DEFINITION




        #region Variables for Cross section - general (not in database DATA1-SECTIONS)

        // 1-rolled, 2-welded, 3-multi –rolled; 4 - others
        int cstype = 1;

        // 1-single, 2-multi
        int csshape = 1;

        // Solid cross-section 101 – square, 102 – rectangle, 103 circle
        int cssolidshape = 101;

        // Welded cross-section (just from plates) 201 – I symetrical Y,Z –same flange thickness , 202- I symetrical Y,Z –various flange thickness, 203- I asymetrical; 204 – welded box symetrical Y,Z – same flange and web thickness,
        int csweldedshape = 201;

        // 1-rolled, 2 – coldrolled, 3 - coldrolled and .., 4- welded, 5- rolled and welded-general, 6- rolled with plates
        int csproduction = 1;

        // Weight of whole cross-section (include all member for built-up sections)
        double cs_weight = 110;




        #endregion

        #region Variables from STEEL-SHEET
        // Material properties


        int mat_num;
        string mat_name;
        int mat_prod_code_num;
        string mat_prod_code_name1;
        string mat_prod_code_name2;
        int steel_grade_num;

        // Steel – characteristic yield and ultimates stress for both thickness
        double fy1;

        public double Fy1
        {
            get { return fy1; }
            set { fy1 = value; }
        }
        double fu1;

        public double Fu1
        {
            get { return fu1; }
            set { fu1 = value; }
        }

        double fy2;

        public double Fy2
        {
            get { return fy2; }
            set { fy2 = value; }
        }
        double fu2;

        public double Fu2
        {
            get { return fu2; }
            set { fu2 = value; }
        }



        // Material factors

        #region Material factors gamaMi
        // partial factor for resistance of cross-sections whatever the class is
        double gamaM0; // missing formula or DATABASE data
        public double GamaM0
        {
            get { return gamaM0; }
            set { gamaM0 = value; }
        }

        // partial factor for resistance of members to instability assessed by member checks
        double gamaM1; // missing formula or DATABASE data

        public double GamaM1
        {
            get { return gamaM1; }
            set { gamaM1 = value; }
        }
        // partial factor for resistance of cross-sections in tension to fracture
        double gamaM2; // missing formula or DATABASE data

        public double GamaM2
        {
            get { return gamaM2; }
            set { gamaM2 = value; }
        }


        // Welding

        double betaw;

        public double Betaw
        {
            get { return betaw; }
            set { betaw = value; }
        }

        // Shear
        double eta_shear;

        public double Eta_shear
        {
            get { return eta_shear; }
            set { eta_shear = value; }
        }




        #endregion


        // Steel – Youngs modulus
        double _E;

        public double E
        {
            get { return _E; }
            set { _E = value; }
        }
        // Steel – Shear modulus
        double _G;

        public double G
        {
            get { return _G; }
            set { _G = value; }
        }
        // Steel – Poisson constant
        double nu_pois;

        public double Nu_pois
        {
            get { return nu_pois; }
            set { nu_pois = value; }
        }

        //

        double alpha_temp;

        public double Alpha_temp
        {
            get { return alpha_temp; }
            set { alpha_temp = value; }
        }


        // Steel – Epsilon constant
        double epsilon_fy;
        double lambda1_fy;

        #endregion

        #region Variables from SECTIONS-SHEET
        // Cross-section properties


        // Rolled cross-section row
        int csprof_namenum;

        public int Csprof_namenum
        {
            get { return csprof_namenum; }
            set { csprof_namenum = value; }
        }
        // Cross-section name
        string csprof_name;

        public string Csprof_name
        {
            get { return csprof_name; }
            set { csprof_name = value; }
        }
        // Rolled single section 1-I and H, 2-U, 3-LR, 4-LN, 5-CHS. 6-SHS, 7-RHS; 8-SLIMDEK, 9-T; 10 - TIE
        int csprofshape;

        public int Csprofshape
        {
            get { return csprofshape; }
            set { csprofshape = value; }
        }
        // CS profile production: 1-rolled, 2 – coldrolled, 3 - coldrolled and ..
        int csprof_production;

        public int Csprof_production
        {
            get { return csprof_production; }
            set { csprof_production = value; }
        }
        // CS production codes
        string csprodcode1_name1;

        public string Csprodcode1_name1
        {
            get { return csprodcode1_name1; }
            set { csprodcode1_name1 = value; }
        }
        string csprodcode2_name1;

        public string Csprodcode2_name1
        {
            get { return csprodcode2_name1; }
            set { csprodcode2_name1 = value; }
        }

        string csprodcode1_name2;

        public string Csprodcode1_name2
        {
            get { return csprodcode1_name2; }
            set { csprodcode1_name2 = value; }
        }
        string csprodcode2_name2;

        public string Csprodcode2_name2
        {
            get { return csprodcode2_name2; }
            set { csprodcode2_name2 = value; }
        }
        // CS Producers
        string csproducer1;

        public string Csproducer1
        {
            get { return csproducer1; }
            set { csproducer1 = value; }
        }

        string csproducer2;

        public string Csproducer2
        {
            get { return csproducer2; }
            set { csproducer2 = value; }
        }

        // CS Weight - single member
        double csprof_weight = 10;

        public double Csprof_weight
        {
            get { return csprof_weight; }
            set { csprof_weight = value; }
        }


        // Geometry

        double h;
        double b;
        double bh;
        double bd;
        double hw1;
        double hi;
        double d;


        double tw;

        double tf;

        double r;
        double r1;
        double r2;
        double r3;
        double d1;


        double w;
        double w1;
        double v;
        double v1;
        double v2;




        // Tubes
        double _D_tube;
        double t_tube;

        // Cross section area
        double _A;
        public double A
        {
            get { return _A; }
            set { _A = value; }
        }
        // Cross section surface area
        double _A_su;

        public double A_su
        {
            get { return _A_su; }
            set { _A_su = value; }
        }


        // Web and flange area

        double _Af;

        public double Af
        {
            get { return _Af; }
            set { _Af = value; }
        }
        double _Aw;

        public double Aw
        {
            get { return _Aw; }
            set { _Aw = value; }
        }

        // tubes
        double _Av;

        public double Av
        {
            get { return _Av; }
            set { _Av = value; }
        }



        // Cross-section shear areas
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

        // Static modulus
        double _Sy;

        public double Sy
        {
            get { return _Sy; }
            set { _Sy = value; }
        }
        double _Sz;

        public double Sz
        {
            get { return _Sz; }
            set { _Sz = value; }
        }

        // Elastic cross-section modulus y-y and z-z
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
        double _Wy_el_min;
        double _Wz_el_min;


        double _Wy1;
        double _Wy2;

        double _Wz1;
        double _Wz2;

        // Plastic cross-section modulus y-y and z-z
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

        // Cross-section modulus for - general for checking formulas
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

        // Sectorial product of area  Staticky vysecovy moment
        double _Sw; // missing formula

        // Vysecova suradnica
        double omega_w; // missing formula





        double e_apostrof;


        // DATA 60 - ........ 


        double ss;
        double ys;

        double _Id;
        double _Ieta;
        double _Weta;
        double ieta_r;
        double _Ixi;
        double _Wxi2;
        double _Wxi13;
        double ixi_r;
        double _Iyz;
        double izf;
        double iz1;
        double ipc;
        double ay;
        double aeta;
        double ip;

        double ia;
        double _Ww1;
        double _Ww2;
        double _Sw3;
        double iw1;
        double k;
        double a1;
        double tg_fi;

        double cy1;
        // It could not to be same as C1y, C2y, C3y - paramaters for lateral buckling
        public double Cy1
        {
            get { return cy1; }
            set { cy1 = value; }
        }
        double ty1;

        public double Ty1
        {
            get { return ty1; }
            set { ty1 = value; }
        }
        double cz1;

        public double Cz1
        {
            get { return cz1; }
            set { cz1 = value; }
        }
        double tz1;

        public double Tz1
        {
            get { return tz1; }
            set { tz1 = value; }
        }

        double cy2;

        public double Cy2
        {
            get { return cy2; }
            set { cy2 = value; }
        }
        double ty2;

        public double Ty2
        {
            get { return ty2; }
            set { ty2 = value; }
        }
        double cz2;

        public double Cz2
        {
            get { return cz2; }
            set { cz2 = value; }
        }
        double tz2;

        public double Tz2
        {
            get { return tz2; }
            set { tz2 = value; }
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

        double y0 = 0;
        double z0 = 0;

        double i0_r;

        // U Section Shear centre
        double ym = 0;

        // L section 
        double aη = 0;
        // Asymetrical L section 

        double ey = 0;
        double ez = 0;

        // Slimdek

        double eel;
        double epl;

        // T Section

        double e;





        #region Variables Cross-section class
        // Cross-section partial classes

        // class for benging
        int class_w_ben_235;
        int class_w_ben_275;
        int class_w_ben_355;
        int class_w_ben_420;
        int class_w_ben_460;
        // class for pressure
        int class_f_pre_235;
        int class_f_pre_275;
        int class_f_pre_355;
        int class_f_pre_420;
        int class_f_pre_460;


        // Whole cross-section class
        // class for benging
        int class_ben_235;
        int class_ben_275;
        int class_ben_355;
        int class_ben_420;
        int class_ben_460;
        // class for pressure
        int class_pre_235;
        int class_pre_275;
        int class_pre_355;
        int class_pre_420;
        int class_pre_460;

        // class of web
        int class_w;

        public int Class_w
        {
            get { return class_w; }
            set { class_w = value; }
        }
        // class of flange
        int class_f;

        public int Class_f
        {
            get { return class_f; }
            set { class_f = value; }
        }
        // class total
        int classall;
        public int Classall
        {
            get { return classall; }
            set { classall = value; }
        }


        #endregion
        #endregion

        #region Variables buckling curves

        string buck_curve_y_420;
        string buck_curve_y_460;
        string buck_curve_z_420;
        string buck_curve_z_460;
        string buck_LT_curve_6322;
        string buck_LT_curve_6323;
        string buck_LT_curve_6324;
        double alfa_y_buck_420;
        double alfa_y_buck_460;
        double alfa_z_buck_420;
        double alfa_z_buck_460;
        double alfa_y_LT_6322;
        double alfa_y_LT_6323;
        double alfa_y_LT_6324;

        // Buckling curve and alfa_y_buck and alfa_z_buck definition (p.58)
        string buck_curve_y;
        string buck_curve_z;
        double alfa_y_buck;
        double alfa_z_buck;

        #endregion

        #endregion

        #region Variables USER

        // Beam properties

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




        // Lateral buckling LT
        // Not same as Cy1 - data of for CS class determination

        double _C1y; // NAD NB3

        public double C1y
        {
            get { return _C1y; }
            set { _C1y = value; }
        }
        double _C2y; // NAD NB3

        public double C2y
        {
            get { return _C2y; }
            set { _C2y = value; }
        }
        double _C3y; // NAD NB3

        public double C3y
        {
            get { return _C3y; }
            set { _C3y = value; }
        }
        double kz_LT; // NAD NB3

        public double Kz_LT
        {
            get { return kz_LT; }
            set { kz_LT = value; }
        }
        double kw_LT; // NAD NB3

        public double Kw_LT
        {
            get { return kw_LT; }
            set { kw_LT = value; }
        }

        // Internal forces in beam cross-section
        double _NEd;
        double _NEd_t;
        double _NEd_p;
        double _Vy_Ed;
        double _Vz_Ed;
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
        double _My_s;
        public double My_s
        {
            get { return _My_s; }
            set { _My_s = value; }
        }
        double _Mz_s;
        public double Mz_s
        {
            get { return _Mz_s; }
            set { _Mz_s = value; }
        }
        // NAD Cczech Republic NB
        // Variables
        double _MEd_T;

        public double MEd_T
        {
            get { return _MEd_T; }
            set { _MEd_T = value; }
        }
        double _VEd_T;

        public double VEd_T
        {
            get { return _VEd_T; }
            set { _VEd_T = value; }
        }
        double e_T;

        public double E_T
        {
            get { return e_T; }
            set { e_T = value; }
        }
        // Table NB.2.1

        double alpha_T; // missing user data definition

        public double Alpha_T
        {
            get { return alpha_T; }
            set { alpha_T = value; }
        }
        double beta_T;

        public double Beta_T
        {
            get { return beta_T; }
            set { beta_T = value; }
        }






        // Table A.2 input data

        double _My_Ed_x; // Missing formula, DATABASE input or user value
        double _Mz_Ed_x; // Missing formula, DATABASE input or user value
        double _deflection_y_x; // Missing formula, DATABASE input or user value
        double _deflection_z_x; // Missing formula, DATABASE input or user value

        public double My_Ed_x
        {
            get { return _My_Ed_x; }
            set { _My_Ed_x = value; }
        }
        public double Mz_Ed_x
        {
            get { return _Mz_Ed_x; }
            set { _Mz_Ed_x = value; }
        }
        public double Deflection_y_x
        {
            get { return _deflection_y_x; }
            set { _deflection_y_x = value; }
        }
        public double Deflection_z_x
        {
            get { return _deflection_z_x; }
            set { _deflection_z_x = value; }
        }




        // Moment diagram for Annex 1 - method 1/ see pictures 1-4

        int moment_diagram_My1 = 1; // missing formula or DATABASE data

        public int Moment_diagram_My1
        {
            get { return moment_diagram_My1; }
            set { moment_diagram_My1 = value; }
        }
        int moment_diagram_Mz1 = 1; // missing formula or DATABASE data

        public int Moment_diagram_Mz1
        {
            get { return moment_diagram_Mz1; }
            set { moment_diagram_Mz1 = value; }
        }

        // Moment diagram for Annex 2 - method 2/ see pictures 1-3

        int moment_diagram_My2 = 1; // missing formula or DATABASE data

        public int Moment_diagram_My2
        {
            get { return moment_diagram_My2; }
            set { moment_diagram_My2 = value; }
        }
        int moment_diagram_Mz2 = 1; // missing formula or DATABASE data

        public int Moment_diagram_Mz2
        {
            get { return moment_diagram_Mz2; }
            set { moment_diagram_Mz2 = value; }
        }


        // Combobox

        int loading_type_y; // missing formula or DATABASE data

        public int Loading_type_y
        {
            get { return loading_type_y; }
            set { loading_type_y = value; }
        }
        int loading_type_z; // missing formula or DATABASE data

        public int Loading_type_z
        {
            get { return loading_type_z; }
            set { loading_type_z = value; }
        }
        int loading_type_LT = 1; // missing formula or DATABASE data

        public int Loading_type_LT
        {
            get { return loading_type_LT; }
            set { loading_type_LT = value; }
        }


        // Torsion NAD CZ Table NB.2.1

        int torsion_koeficientNADCZ;

        public int Torsion_koeficientNADCZ
        {
            get { return torsion_koeficientNADCZ; }
            set { torsion_koeficientNADCZ = value; }
        }


        #endregion

        #region Variables MessageBoxes definition and property method
        // Global Variables for Messageboxes initializing
        bool item3_check;

        public bool Item3_check
        {
            get { return item3_check; }
            set { item3_check = value; }
        }
        bool item4_check;

        public bool Item4_check
        {
            get { return item4_check; }
            set { item4_check = value; }
        }
        bool item5_check;

        public bool Item5_check
        {
            get { return item5_check; }
            set { item5_check = value; }
        }
        bool item6_check;

        public bool Item6_check
        {
            get { return item6_check; }
            set { item6_check = value; }
        }



        #endregion

        #region Variables EN1993_1_1

        #region Variables - material

        // Steel – characteristic yield stress
        double fy; // missing formula or DATABASE data

        public double Fy
        {
            get { return fy; }
            set { fy = value; }
        }
        // Steel – characteristic ultimate stress
        double fu; // missing formula or DATABASE data

        public double Fu
        {
            get { return fu; }
            set { fu = value; }
        }

        #endregion

        #region Variables - cross-section

        double bf1;
        double bf2;
        double tw1;
        double tw2;
        double tf1;
        double tf2;


        // Cross section segment class 
        // Internal parts - Webs
        int class_y1;

        public int Class_y1
        {
            get { return class_y1; }
            set { class_y1 = value; }
        }
        int class_z1;

        public int Class_z1
        {
            get { return class_z1; }
            set { class_z1 = value; }
        }

        //Outstand flanges

        int class_y2;

        public int Class_y2
        {
            get { return class_y2; }
            set { class_y2 = value; }
        }
        int class_z2;

        public int Class_z2
        {
            get { return class_z2; }
            set { class_z2 = value; }
        }

        // Angles

        int class_ang1;

        public int Class_ang1
        {
            get { return class_ang1; }
            set { class_ang1 = value; }
        }


        // Cross section net area
        double _Anet; // missing formula or DATABASE data

        public double Anet
        {
            get { return _Anet; }
            set { _Anet = value; }
        }

        #region Variables - Class 4

        // Cross section effective area
        double _Aeff = 1500; // missing formula or DATABASE data

        public double Aeff
        {
            get { return _Aeff; }
            set { _Aeff = value; }
        }
        // Cross section effective area_net
        double _Aeff_net = 1200; // missing formula or DATABASE data

        public double Aeff_net
        {
            get { return _Aeff_net; }
            set { _Aeff_net = value; }
        }
        // Cross-section shear areas
        double _Ay_v_eff = 900; // missing formula or DATABASE data

        public double Ay_v_eff
        {
            get { return _Ay_v_eff; }
            set { _Ay_v_eff = value; }
        }
        double _Az_v_eff = 550; // missing formula or DATABASE data

        public double Az_v_eff
        {
            get { return _Az_v_eff; }
            set { _Az_v_eff = value; }
        }
        // Effective cross-section modulus y-y and z-z
        double _Wy_eff = 120000; // missing formula or DATABASE data

        public double Wy_eff
        {
            get { return _Wy_eff; }
            set { _Wy_eff = value; }
        }
        double _Wz_eff = 260000; // missing formula or DATABASE data

        public double Wz_eff
        {
            get { return _Wz_eff; }
            set { _Wz_eff = value; }
        }
        double _Wy_eff_min = 120000; // missing formula or DATABASE data

        public double Wy_eff_min
        {
            get { return _Wy_eff_min; }
            set { _Wy_eff_min = value; }
        }
        double _Wz_eff_min = 260000; // missing formula or DATABASE data

        public double Wz_eff_min
        {
            get { return _Wz_eff_min; }
            set { _Wz_eff_min = value; }
        }
        // Effective moment of inertia (Second moment of area) y-y and z-z
        double _Iy_eff = 90000; // missing formula or DATABASE data

        public double Iy_eff
        {
            get { return _Iy_eff; }
            set { _Iy_eff = value; }
        }
        double _Iz_eff = 20000; // missing formula or DATABASE data

        public double Iz_eff
        {
            get { return _Iz_eff; }
            set { _Iz_eff = value; }
        }
        // Radius of inertia y-y and z-z
        double iy_r_eff = 95.56; // missing formula or DATABASE data

        public double Iy_r_eff
        {
            get { return iy_r_eff; }
            set { iy_r_eff = value; }
        }
        double iz_r_eff = 21.86; // missing formula or DATABASE data

        public double Iz_r_eff
        {
            get { return iz_r_eff; }
            set { iz_r_eff = value; }
        }





        #endregion

        #endregion

        #region Variables _N

        // The design tension resistance
        double _Nt_Rd;

        public double Nt_Rd
        {
            get { return _Nt_Rd; }
            set { _Nt_Rd = value; }
        }
        double _Npl_Rd;

        public double Npl_Rd
        {
            get { return _Npl_Rd; }
            set { _Npl_Rd = value; }
        }
        double _Nu_Rd;

        public double Nu_Rd
        {
            get { return _Nu_Rd; }
            set { _Nu_Rd = value; }
        }
        double _Nnet_Rd;

        public double Nnet_Rd
        {
            get { return _Nnet_Rd; }
            set { _Nnet_Rd = value; }
        }
        // The design compression resistances

        double _N_Rk;
        public double N_Rk
        {
            get { return _N_Rk; }
            set { _N_Rk = value; }
        }

        double _Ny_cr;

        public double Ny_cr
        {
            get { return _Ny_cr; }
            set { _Ny_cr = value; }
        }
        double _Nz_cr;

        public double Nz_cr
        {
            get { return _Nz_cr; }
            set { _Nz_cr = value; }
        }
        double _Nc_Rd;

        public double Nc_Rd
        {
            get { return _Nc_Rd; }
            set { _Nc_Rd = value; }
        }


        double _Ny_b_Rd;

        public double Ny_b_Rd
        {
            get { return _Ny_b_Rd; }
            set { _Ny_b_Rd = value; }
        }
        double _Nz_b_Rd;

        public double Nz_b_Rd
        {
            get { return _Nz_b_Rd; }
            set { _Nz_b_Rd = value; }
        }
        double _NT_b_Rd;

        public double NT_b_Rd
        {
            get { return _NT_b_Rd; }
            set { _NT_b_Rd = value; }
        }

        double _N_cr_T;
        public double N_cr_T
        {
            get { return _N_cr_T; }
            set { _N_cr_T = value; }
        }
        double _N_cr_TF;
        public double N_cr_TF
        {
            get { return _N_cr_TF; }
            set { _N_cr_TF = value; }
        }
        double _N_b_Rd;
        public double N_b_Rd
        {
            get { return _N_b_Rd; }
            set { _N_b_Rd = value; }
        }











        #endregion

        #region Variables _T
        double _Tx_Ed;
        double _T_Rd;

        // NAD NB Czech republic

        double _Kt_T;
        double kappa_T;
        double _BEd;
        double _Tt_Ed;
        double _Tw_Ed;







        #endregion

        #region Variables _V
        double _Vy_c_Rd;

        public double Vy_c_Rd
        {
            get { return _Vy_c_Rd; }
            set { _Vy_c_Rd = value; }
        }
        double _Vz_c_Rd;

        public double Vz_c_Rd
        {
            get { return _Vz_c_Rd; }
            set { _Vz_c_Rd = value; }
        }
        #endregion

        #region Variables _M
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
        double _My_V_el_Rd;

        public double My_V_el_Rd
        {
            get { return _My_V_el_Rd; }
            set { _My_V_el_Rd = value; }
        }
        double _Mz_V_el_Rd;

        public double Mz_V_el_Rd
        {
            get { return _Mz_V_el_Rd; }
            set { _Mz_V_el_Rd = value; }
        }
        double _My_V_pl_Rd;

        public double My_V_pl_Rd
        {
            get { return _My_V_pl_Rd; }
            set { _My_V_pl_Rd = value; }
        }
        double _Mz_V_pl_Rd;

        public double Mz_V_pl_Rd
        {
            get { return _Mz_V_pl_Rd; }
            set { _Mz_V_pl_Rd = value; }
        }
        double _My_Rk;



        public double My_Rk
        {
            get { return _My_Rk; }
            set { _My_Rk = value; }
        }
        double _Mz_Rk;
        public double Mz_Rk
        {
            get { return _Mz_Rk; }
            set { _Mz_Rk = value; }
        }
        double _My_N_Rd;

        public double My_N_Rd
        {
            get { return _My_N_Rd; }
            set { _My_N_Rd = value; }
        }
        double _Mz_N_Rd;

        public double Mz_N_Rd
        {
            get { return _Mz_N_Rd; }
            set { _Mz_N_Rd = value; }
        }
        double _My_b_Rd;
        public double My_b_Rd
        {
            get { return _My_b_Rd; }
            set { _My_b_Rd = value; }
        }
        double _Mz_b_Rd;
        public double Mz_b_Rd
        {
            get { return _Mz_b_Rd; }
            set { _Mz_b_Rd = value; }
        }

        double sigma_x_Ed;

        public double Sigma_x_Ed
        {
            get { return sigma_x_Ed; }
            set { sigma_x_Ed = value; }
        }
        double e_Ny = 0; //pomocne zatial
        double e_Nz = 0; //pomocne zatial
        double delta_My_Ed;
        public double Delta_My_Ed
        {
            get { return delta_My_Ed; }
            set { delta_My_Ed = value; }
        }
        double delta_Mz_Ed;
        public double Delta_Mz_Ed
        {
            get { return delta_Mz_Ed; }
            set { delta_Mz_Ed = value; }
        }
        double _My_1a;

        public double My_1a
        {
            get { return _My_1a; }
            set { _My_1a = value; }
        }
        double _My_1b;

        public double My_1b
        {
            get { return _My_1b; }
            set { _My_1b = value; }
        }
        double _Mz_1a;

        public double Mz_1a
        {
            get { return _Mz_1a; }
            set { _Mz_1a = value; }
        }
        double _Mz_1b;

        public double Mz_1b
        {
            get { return _Mz_1b; }
            set { _Mz_1b = value; }
        }

        #endregion

        #region Variables kij
        double kyy;

        public double Kyy
        {
            get { return kyy; }
            set { kyy = value; }
        }
        double kyz;
        public double Kyz
        {
            get { return kyz; }
            set { kyz = value; }
        }
        double kzy;
        public double Kzy
        {
            get { return kzy; }
            set { kzy = value; }
        }
        double kzz;
        public double Kzz
        {
            get { return kzz; }
            set { kzz = value; }
        }
        double _Cmy;
        public double Cmy
        {
            get { return _Cmy; }
            set { _Cmy = value; }
        }
        double _Cmz;
        public double Cmz
        {
            get { return _Cmz; }
            set { _Cmz = value; }
        }
        double _CmLT;
        public double CmLT
        {
            get { return _CmLT; }
            set { _CmLT = value; }
        }



        double _Cmy_0;
        public double Cmy_0
        {
            get { return _Cmy_0; }
            set { Cmy_0 = value; }
        }

        double _Cmz_0;
        public double Cmz_0
        {
            get { return _Cmz_0; }
            set { Cmz_0 = value; }
        }


        double psi_My_1;
        public double Psi_My_1
        {
            get { return psi_My_1; }
            set { psi_My_1 = value; }
        }
        double psi_Mz_1;
        public double Psi_Mz_1
        {
            get { return psi_Mz_1; }
            set { psi_Mz_1 = value; }
        }
        #region Variables Annex A
        // Annex A
        double mu_y;
        public double Mu_y
        {
            get { return mu_y; }
            set { mu_y = value; }
        }
        double mu_z;
        public double Mu_z
        {
            get { return mu_z; }
            set { mu_z = value; }
        }
        double _Cyy;
        public double Cyy
        {
            get { return _Cyy; }
            set { _Cyy = value; }
        }
        double _Cyz;
        public double Cyz
        {
            get { return _Cyz; }
            set { _Cyz = value; }
        }
        double _Czy;
        public double Czy
        {
            get { return _Czy; }
            set { _Czy = value; }
        }
        double _Czz;
        public double Czz
        {
            get { return _Czz; }
            set { _Czz = value; }
        }
        double aLT;
        public double ALT
        {
            get { return aLT; }
            set { aLT = value; }
        }
        double bLT;
        public double BLT
        {
            get { return bLT; }
            set { bLT = value; }
        }
        double cLT;
        public double CLT
        {
            get { return cLT; }
            set { cLT = value; }
        }
        double dLT;
        public double DLT
        {
            get { return dLT; }
            set { dLT = value; }
        }
        double eLT;
        public double ELT
        {
            get { return eLT; }
            set { eLT = value; }
        }


        double epsilon_y;
        public double Epsilon_y
        {
            get { return epsilon_y; }
            set { epsilon_y = value; }
        }













        #endregion
        #endregion

        #region Variables buckling
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
        #endregion

        #region Variables lateral buckling
        double alfa_y_LT;
        double alfa_z_LT;
        double lambda_y_LT_rel;
        double lambda_z_LT_rel;
        double fi_y_LT;
        double fi_z_LT;
        double chi_y_LT;

        public double Chi_y_LT
        {
            get { return chi_y_LT; }
            set { chi_y_LT = value; }
        }
        double chi_z_LT;

        public double Chi_z_LT
        {
            get { return chi_z_LT; }
            set { chi_z_LT = value; }
        }

        // missing moment diagrams and formulas Table 6.6 (p.63)
        int kc_diagram;

        public int Kc_diagram
        {
            get { return kc_diagram; }
            set { kc_diagram = value; }
        }



        double kc_y_LT = 0.9;  // missing formula or DATABASE data
        double kc_z_LT = 0.9; // missing formula or DATABASE data

        #endregion

        #region Variables of check ratio
        double ratio_65;

        public double Ratio_65
        {
            get { return ratio_65; }
            set { ratio_65 = value; }
        }
        double ratio_69;

        public double Ratio_69
        {
            get { return ratio_69; }
            set { ratio_69 = value; }
        }
        double ratio_612y;

        public double Ratio_612y
        {
            get { return ratio_612y; }
            set { ratio_612y = value; }
        }
        double ratio_612z;

        public double Ratio_612z
        {
            get { return ratio_612z; }
            set { ratio_612z = value; }
        }

        // Section 6.2.8 - Bending and shear
        double ratio_612y_MV;

        public double Ratio_612y_MV
        {
            get { return ratio_612y_MV; }
            set { ratio_612y_MV = value; }
        }
        double ratio_612z_MV;

        public double Ratio_612z_MV
        {
            get { return ratio_612z_MV; }
            set { ratio_612z_MV = value; }
        }

        //

        double ratio_623;
        public double Ratio_623
        {
            get { return ratio_623; }
            set { ratio_623 = value; }
        }
        // Auxiliary for torsion
        double ratio_a4;





        double ratio_617y;

        public double Ratio_617y
        {
            get { return ratio_617y; }
            set { ratio_617y = value; }
        }
        double ratio_617z;
        public double Ratio_617z
        {
            get { return ratio_617z; }
            set { ratio_617z = value; }
        }
        double ratio_631y;
        public double Ratio_631y
        {
            get { return ratio_631y; }
            set { ratio_631y = value; }
        }
        double ratio_631z;
        public double Ratio_631z
        {
            get { return ratio_631z; }
            set { ratio_631z = value; }
        }
        double ratio_642;
        public double Ratio_642
        {
            get { return ratio_642; }
            set { ratio_642 = value; }
        }
        double ratio_643;
        public double Ratio_643
        {
            get { return ratio_643; }
            set { ratio_643 = value; }
        }

        double ratio_644;
        public double Ratio_644
        {
            get { return ratio_644; }
            set { ratio_644 = value; }
        }




        double ratio_646;
        public double Ratio_646
        {
            get { return ratio_646; }
            set { ratio_646 = value; }
        }

        double ratio_654y;
        public double Ratio_654y
        {
            get { return ratio_654y; }
            set { ratio_654y = value; }
        }
        double ratio_654z;
        public double Ratio_654z
        {
            get { return ratio_654z; }
            set { ratio_654z = value; }
        }



        double ratio_661N;
        public double Ratio_661N
        {
            get { return ratio_661N; }
            set { ratio_661N = value; }
        }

        double ratio_661My;
        public double Ratio_661My
        {
            get { return ratio_661My; }
            set { ratio_661My = value; }
        }

        double ratio_661Mz;
        public double Ratio_661Mz
        {
            get { return ratio_661Mz; }
            set { ratio_661Mz = value; }
        }




        double ratio_661;
        public double Ratio_661
        {
            get { return ratio_661; }
            set { ratio_661 = value; }
        }





        double ratio_662N;
        public double Ratio_662N
        {
            get { return ratio_662N; }
            set { ratio_662N = value; }
        }

        double ratio_662My;
        public double Ratio_662My
        {
            get { return ratio_662My; }
            set { ratio_662My = value; }
        }

        double ratio_662Mz;
        public double Ratio_662Mz
        {
            get { return ratio_662Mz; }
            set { ratio_662Mz = value; }
        }







        double ratio_662;
        public double Ratio_662
        {
            get { return ratio_662; }
            set { ratio_662 = value; }
        }

        // Auxiliary value for "σx.Ed/fyd":

        double ratio_629_max_d;

        public double Ratio_629_max_d
        {
            get { return ratio_629_max_d; }
            set { ratio_629_max_d = value; }
        }


        // Maximum ratio
        double ratio_maxtot;

        public double Ratio_maxtot
        {
            get { return ratio_maxtot; }
            set { ratio_maxtot = value; }
        }







        #endregion


        #endregion

        #region Konstruktor

        //Konstruktor - v nom sa deje nacitavanie z databazy pomocou SQL prikazu
        //v nom sa nacitaju dane hodnoty pre dany typ ocele,ktory je parametrom v konstruktore
        //pripojenie na databazu

        OleDbDataReader dat_reader;
        DatabaseConnection dat_conn;
        
        public EN1993_1_1(string combo1, string combo2,bool item3,bool item4, bool item5, bool item6)
        {

            string sql1;
            string sql2;
            
            sql1 = "Select mat_num, mat_name, mat_prod_code_num, mat_prod_code_name1, mat_prod_code_name2, steel_grade_num, fy1, fu1, fy2, fu2, gamaM0, gamaM1, gamaM2, betaw, eta_shear, E, G, nu_pois, alpha_temp from Steel where mat_name like '" + combo1 + "'";

            #region Database DATA-STEEL variables reader
            dat_conn = DatabaseConnection.getInstance();
            dat_reader = dat_conn.getDBReader(sql1);

            try
            {
                while (dat_reader.Read())
                {
                    // STEEL DATABASE DATA

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
                        mat_prod_code_num = Convert.ToInt16(dat_reader.GetValue(2).ToString());
                    }
                    catch (FormatException) { }
                    try
                    {
                        mat_prod_code_name1 = dat_reader.GetValue(3).ToString();
                    }
                    catch (FormatException) { }
                    try
                    {
                        mat_prod_code_name2 = dat_reader.GetValue(4).ToString();
                    }
                    catch (FormatException) { }
                    try
                    {
                        steel_grade_num = Convert.ToInt32(dat_reader.GetValue(5).ToString());
                    }
                    catch (FormatException) { }
                    try
                    {
                        fy1 = Convert.ToDouble(dat_reader.GetValue(6).ToString());
                    }
                    catch (FormatException) { }
                    try
                    {
                        fu1 = Convert.ToDouble(dat_reader.GetValue(7).ToString());
                    }
                    catch (FormatException) { }
                    try
                    {
                        fy2 = Convert.ToDouble(dat_reader.GetValue(8).ToString());
                    }
                    catch (FormatException) { }
                    try
                    {
                        fu2 = Convert.ToDouble(dat_reader.GetValue(9).ToString());
                    }
                    catch (FormatException) { }
                    try
                    {
                        gamaM0 = Convert.ToDouble(dat_reader.GetValue(10).ToString());
                    }
                    catch (FormatException) { }
                    try
                    {
                        gamaM1 = Convert.ToDouble(dat_reader.GetValue(11).ToString());
                    }
                    catch (FormatException) { }
                    try
                    {
                        gamaM2 = Convert.ToDouble(dat_reader.GetValue(12).ToString());
                    }
                    catch (FormatException) { }
                    try
                    {
                        betaw = Convert.ToDouble(dat_reader.GetValue(13).ToString());
                    }
                    catch (FormatException) { }
                    try
                    {
                        eta_shear = Convert.ToDouble(dat_reader.GetValue(14).ToString());
                    }
                    catch (FormatException) { }
                    try
                    {
                        _E = Convert.ToDouble(dat_reader.GetValue(15).ToString());
                    }
                    catch (FormatException) { }
                    try
                    {
                        _G = Convert.ToDouble(dat_reader.GetValue(16).ToString());
                    }
                    catch (FormatException) { }
                    try
                    {
                        nu_pois = Convert.ToDouble(dat_reader.GetValue(17).ToString());
                    }
                    catch (FormatException) { }
                    try
                    {
                        alpha_temp = Convert.ToDouble(dat_reader.GetValue(18).ToString());
                    }
                    catch (FormatException) { }

                    #endregion

                }
            }
            catch (FormatException) { }


            #endregion

            sql2 = "Select csprof_namenum, csprof_name, csprofshape, csprof_production, csprodcode1_name1, csprodcode1_name2, csprodcode2_name1, csprodcode2_name2, csproducer1, csproducer2, csprof_weight,h, b, bh, bd, tw, tf, r, r1, r2, r3, hi, d, d1, e, e_apostrof, ez, ey, w, w1, v, v1, v2, eel, epl, D_tube, t_tube, A, A_su, Af, Aw, Ay_v, Az_v, Av, Iy, Wy_el, Wy1, Wy2, Wy_pl, iy_r, Sy, Iz, Wz_el, Wz1, Wz2, Wz_pl, iz_r, Sz, IT, Wt, Iw, ss, ys, ym, Id, Ieta, Weta, ieta_r, Ixi, Wxi2, Wxi13,ixi_r, Iyz, omega_w, izf, iz1, ipc, ay, aeta, ip, ia, Ww1, Ww2, Sw3, iw1, k, a1, tg_fi, cy1, ty1, cz1, tz1, cy2, ty2, cz2, tz2 from sections where csprof_name like '" + combo2 + "'";

            #region Database DATA-SECTIONS variables reader
            dat_conn = DatabaseConnection.getInstance();
            dat_reader = dat_conn.getDBReader(sql2);

            try
            {
                while (dat_reader.Read())
                {
                    // CROSS-SECTION DATABASE DATA

                    #region Data list
                    // 0 - 9 
                    try
                    {
                        csprof_namenum = Convert.ToInt16(dat_reader.GetValue(0).ToString());
                    }
                    catch (FormatException) { }
                    try
                    {
                        csprof_name = dat_reader.GetValue(1).ToString();
                    }
                    catch (FormatException) { }
                    try
                    {
                        csprofshape = Convert.ToInt16(dat_reader.GetValue(2).ToString());
                    }
                    catch (FormatException) { }
                    try
                    {
                        csprof_production = Convert.ToInt16(dat_reader.GetValue(3).ToString());
                    }
                    catch (FormatException) { }
                    try
                    {
                        csprodcode1_name1 = dat_reader.GetValue(4).ToString();
                    }
                    catch (FormatException) { }
                    try
                    {
                        csprodcode1_name2 = dat_reader.GetValue(5).ToString();
                    }
                    catch (FormatException) { }
                    try
                    {
                        csprodcode2_name1 = dat_reader.GetValue(6).ToString();
                    }
                    catch (FormatException) { }
                    try
                    {
                        csprodcode2_name2 = dat_reader.GetValue(7).ToString();
                    }
                    catch (FormatException) { }
                    try
                    {
                        csproducer1 = dat_reader.GetValue(8).ToString();
                    }
                    catch (FormatException) { }
                    try
                    {
                        csproducer2 = dat_reader.GetValue(9).ToString();
                    }
                    catch (FormatException) { }

                    // 10-19

                    csprof_weight = Convert.ToDouble(dat_reader.GetValue(10).ToString());
                    try
                    {
                        h = Convert.ToDouble(dat_reader.GetValue(11).ToString());
                    }
                    catch (FormatException) { }
                    try
                    {
                        b = Convert.ToDouble(dat_reader.GetValue(12).ToString());
                    }
                    catch (FormatException) { }
                    try
                    {
                        bh = Convert.ToDouble(dat_reader.GetValue(13).ToString());
                    }
                    catch (FormatException) { }
                    try
                    {
                        bd = Convert.ToDouble(dat_reader.GetValue(14).ToString());
                    }
                    catch (FormatException) { }
                    try
                    {
                        tw = Convert.ToDouble(dat_reader.GetValue(15).ToString());
                    }
                    catch (FormatException) { }
                    try
                    {
                        tf = Convert.ToDouble(dat_reader.GetValue(16).ToString());
                    }
                    catch (FormatException) { }
                    try
                    {
                        r = Convert.ToDouble(dat_reader.GetValue(17).ToString());
                    }
                    catch (FormatException) { }
                    try
                    {
                        r1 = Convert.ToDouble(dat_reader.GetValue(18).ToString());
                    }
                    catch (FormatException) { }
                    try
                    {
                        r2 = Convert.ToDouble(dat_reader.GetValue(19).ToString());
                    }
                    catch (FormatException) { }

                    // 20-29

                    try
                    {
                        r3 = Convert.ToDouble(dat_reader.GetValue(20).ToString());
                    }
                    catch (FormatException) { }
                    try
                    {
                        hi = Convert.ToDouble(dat_reader.GetValue(21).ToString());
                    }
                    catch (FormatException) { }
                    try
                    {
                        d = Convert.ToDouble(dat_reader.GetValue(22).ToString());
                    }
                    catch (FormatException) { }
                    try
                    {
                        d1 = Convert.ToDouble(dat_reader.GetValue(23).ToString());
                    }
                    catch (FormatException) { }
                    try
                    {
                        e = Convert.ToDouble(dat_reader.GetValue(24).ToString());
                    }
                    catch (FormatException) { }
                    try
                    {
                        e_apostrof = Convert.ToDouble(dat_reader.GetValue(25).ToString());
                    }
                    catch (FormatException) { }
                    try
                    {
                        ez = Convert.ToDouble(dat_reader.GetValue(26).ToString());
                    }
                    catch (FormatException) { }
                    try
                    {
                        ey = Convert.ToDouble(dat_reader.GetValue(27).ToString());
                    }
                    catch (FormatException) { }
                    try
                    {
                        w = Convert.ToDouble(dat_reader.GetValue(28).ToString());
                    }
                    catch (FormatException) { }
                    try
                    {
                        w1 = Convert.ToDouble(dat_reader.GetValue(29).ToString());
                    }
                    catch (FormatException) { }

                    // 30-39
                    try
                    {
                        v = Convert.ToDouble(dat_reader.GetValue(30).ToString());
                    }
                    catch (FormatException) { }
                    try
                    {
                        v1 = Convert.ToDouble(dat_reader.GetValue(31).ToString());
                    }
                    catch (FormatException) { }
                    try
                    {
                        v2 = Convert.ToDouble(dat_reader.GetValue(32).ToString());
                    }
                    catch (FormatException) { }
                    try
                    {
                        eel = Convert.ToDouble(dat_reader.GetValue(33).ToString());
                    }
                    catch (FormatException) { }
                    try
                    {
                        epl = Convert.ToDouble(dat_reader.GetValue(34).ToString());
                    }
                    catch (FormatException) { }
                    try
                    {
                        _D_tube = Convert.ToDouble(dat_reader.GetValue(35).ToString());
                    }
                    catch (FormatException) { }
                    try
                    {
                        t_tube = Convert.ToDouble(dat_reader.GetValue(36).ToString());
                    }
                    catch (FormatException) { }
                    try
                    {
                        _A = Convert.ToDouble(dat_reader.GetValue(37).ToString());
                    }
                    catch (FormatException) { }
                    try
                    {
                        _A_su = Convert.ToDouble(dat_reader.GetValue(38).ToString());
                    }
                    catch (FormatException) { }
                    try
                    {
                        _Af = Convert.ToDouble(dat_reader.GetValue(39).ToString());
                    }
                    catch (FormatException) { }

                    // 40-49

                    try
                    {
                        _Aw = Convert.ToDouble(dat_reader.GetValue(40).ToString());
                    }
                    catch (FormatException) { }
                    try
                    {
                        _Ay_v = Convert.ToDouble(dat_reader.GetValue(41).ToString());
                    }
                    catch (FormatException) { }
                    try
                    {
                        _Az_v = Convert.ToDouble(dat_reader.GetValue(42).ToString());
                    }
                    catch (FormatException) { }
                    try
                    {
                        _Av = Convert.ToDouble(dat_reader.GetValue(43).ToString());
                    }
                    catch (FormatException) { }
                    try
                    {
                        _Iy = Convert.ToDouble(dat_reader.GetValue(44).ToString());
                    }
                    catch (FormatException) { }
                    try
                    {
                        _Wy_el = Convert.ToDouble(dat_reader.GetValue(45).ToString());
                    }
                    catch (FormatException) { }
                    try
                    {
                        _Wy1 = Convert.ToDouble(dat_reader.GetValue(46).ToString());
                    }
                    catch (FormatException) { }
                    try
                    {
                        _Wy2 = Convert.ToDouble(dat_reader.GetValue(47).ToString());
                    }
                    catch (FormatException) { }
                    try
                    {
                        _Wy_pl = Convert.ToDouble(dat_reader.GetValue(48).ToString());
                    }
                    catch (FormatException) { }
                    try
                    {
                        iy_r = Convert.ToDouble(dat_reader.GetValue(49).ToString());
                    }
                    catch (FormatException) { }

                    // 50-59

                    try
                    {
                        _Sy = Convert.ToDouble(dat_reader.GetValue(50).ToString());
                    }
                    catch (FormatException) { }
                    try
                    {
                        _Iz = Convert.ToDouble(dat_reader.GetValue(51).ToString());
                    }
                    catch (FormatException) { }
                    try
                    {
                        _Wz_el = Convert.ToDouble(dat_reader.GetValue(52).ToString());
                    }
                    catch (FormatException) { }
                    try
                    {
                        _Wz1 = Convert.ToDouble(dat_reader.GetValue(53).ToString());
                    }
                    catch (FormatException) { }
                    try
                    {
                        _Wz2 = Convert.ToDouble(dat_reader.GetValue(54).ToString());
                    }
                    catch (FormatException) { }
                    try
                    {
                        _Wz_pl = Convert.ToDouble(dat_reader.GetValue(55).ToString());
                    }
                    catch (FormatException) { }
                    try
                    {
                        iz_r = Convert.ToDouble(dat_reader.GetValue(56).ToString());
                    }
                    catch (FormatException) { }
                    try
                    {
                        _Sz = Convert.ToDouble(dat_reader.GetValue(57).ToString());
                    }
                    catch (FormatException) { }

                    try
                    {
                        _IT = Convert.ToDouble(dat_reader.GetValue(58).ToString());
                    }
                    catch (FormatException) { }
                    try
                    {
                        _Wt = Convert.ToDouble(dat_reader.GetValue(59).ToString());
                    }
                    catch (FormatException) { }

                    // 60-69

                    try
                    {
                        _Iw = Convert.ToDouble(dat_reader.GetValue(60).ToString());
                    }
                    catch (FormatException) { }
                    try
                    {
                        ss = Convert.ToDouble(dat_reader.GetValue(61).ToString());
                    }
                    catch (FormatException) { }
                    try
                    {
                        ys = Convert.ToDouble(dat_reader.GetValue(62).ToString());
                    }
                    catch (FormatException) { }
                    try
                    {
                        ym = Convert.ToDouble(dat_reader.GetValue(63).ToString());
                    }
                    catch (FormatException) { }
                    try
                    {
                        _Id = Convert.ToDouble(dat_reader.GetValue(64).ToString());
                    }
                    catch (FormatException) { }
                    try
                    {
                        _Ieta = Convert.ToDouble(dat_reader.GetValue(65).ToString());
                    }
                    catch (FormatException) { }
                    try
                    {
                        _Weta = Convert.ToDouble(dat_reader.GetValue(66).ToString());
                    }
                    catch (FormatException) { }
                    try
                    {
                        ieta_r = Convert.ToDouble(dat_reader.GetValue(67).ToString());
                    }
                    catch (FormatException) { }
                    try
                    {
                        _Ixi = Convert.ToDouble(dat_reader.GetValue(68).ToString());
                    }
                    catch (FormatException) { }
                    try
                    {
                        _Wxi2 = Convert.ToDouble(dat_reader.GetValue(69).ToString());
                    }
                    catch (FormatException) { }

                    // 70-79

                    try
                    {
                        _Wxi13 = Convert.ToDouble(dat_reader.GetValue(70).ToString());
                    }
                    catch (FormatException) { }
                    try
                    {
                        ixi_r = Convert.ToDouble(dat_reader.GetValue(71).ToString());
                    }
                    catch (FormatException) { }
                    try
                    {
                        _Iyz = Convert.ToDouble(dat_reader.GetValue(72).ToString());
                    }
                    catch (FormatException) { }
                    try
                    {
                        omega_w = Convert.ToDouble(dat_reader.GetValue(73).ToString());
                    }
                    catch (FormatException) { }
                    try
                    {
                        izf = Convert.ToDouble(dat_reader.GetValue(74).ToString());
                    }
                    catch (FormatException) { }
                    try
                    {
                        iz1 = Convert.ToDouble(dat_reader.GetValue(75).ToString());
                    }
                    catch (FormatException) { }
                    try
                    {
                        ipc = Convert.ToDouble(dat_reader.GetValue(76).ToString());
                    }
                    catch (FormatException) { }
                    try
                    {
                        ay = Convert.ToDouble(dat_reader.GetValue(77).ToString());
                    }
                    catch (FormatException) { }
                    try
                    {
                        aeta = Convert.ToDouble(dat_reader.GetValue(78).ToString());
                    }
                    catch (FormatException) { }
                    try
                    {
                        ip = Convert.ToDouble(dat_reader.GetValue(79).ToString());
                    }
                    catch (FormatException) { }

                    // 80-89

                    try
                    {
                        ia = Convert.ToDouble(dat_reader.GetValue(80).ToString());
                    }
                    catch (FormatException) { }
                    try
                    {
                        _Ww1 = Convert.ToDouble(dat_reader.GetValue(81).ToString());
                    }
                    catch (FormatException) { }
                    try
                    {
                        _Ww2 = Convert.ToDouble(dat_reader.GetValue(82).ToString());
                    }
                    catch (FormatException) { }
                    try
                    {

                        _Sw3 = Convert.ToDouble(dat_reader.GetValue(83).ToString());
                    }
                    catch (FormatException) { }
                    try
                    {
                        iw1 = Convert.ToDouble(dat_reader.GetValue(84).ToString());
                    }
                    catch (FormatException) { }
                    try
                    {
                        k = Convert.ToDouble(dat_reader.GetValue(85).ToString());
                    }
                    catch (FormatException) { }
                    try
                    {
                        a1 = Convert.ToDouble(dat_reader.GetValue(86).ToString());
                    }
                    catch (FormatException) { }
                    try
                    {
                        tg_fi = Convert.ToDouble(dat_reader.GetValue(87).ToString());
                    }
                    catch (FormatException) { }
                    try
                    {
                        cy1 = Convert.ToDouble(dat_reader.GetValue(88).ToString());
                    }
                    catch (FormatException) { }
                    try
                    {
                        ty1 = Convert.ToDouble(dat_reader.GetValue(89).ToString());
                    }
                    catch (FormatException) { }


                    // 90-99

                    try
                    {
                        cz1 = Convert.ToDouble(dat_reader.GetValue(90).ToString());
                    }
                    catch (FormatException) { }
                    try
                    {
                        tz1 = Convert.ToDouble(dat_reader.GetValue(91).ToString());
                    }
                    catch (FormatException) { }
                    try
                    {
                        cy2 = Convert.ToDouble(dat_reader.GetValue(92).ToString());
                    }
                    catch (FormatException) { }
                    try
                    {
                        ty2 = Convert.ToDouble(dat_reader.GetValue(93).ToString());
                    }
                    catch (FormatException) { }
                    try
                    {
                        cz2 = Convert.ToDouble(dat_reader.GetValue(94).ToString());
                    }
                    catch (FormatException) { }
                    try
                    {
                        tz2 = Convert.ToDouble(dat_reader.GetValue(95).ToString());
                    }
                    catch (FormatException) { }



                    #endregion

                }
            }
            catch (FormatException) { }
            #endregion


            // Data from Form: EN1993-1-1MessageBoxes.cs 
            this.item3_check = item3;
            this.item4_check = item4;
            this.item5_check = item5;
            this.item6_check = item6;
             

        }
       
        #endregion

        public void EN_1993_1_1_Material()
        {
            #region Material
            // fy and fu value for real thickness

            if (tw1 >= 40 || tw2 >= 40 || tf1 >= 40 || tf2 >= 40)
            {
                fy = fy2;
                fu = fu2;
            }
            else
            {
                fy = fy1;
                fu = fu1;
            }


            // Steel – Epsilon constant
            epsilon_fy = Math.Sqrt(235 / fy);
            // Steel – lambda 1 constant
            lambda1_fy = Math.PI * Math.Sqrt(_E / fy);


            #endregion
        }

        #region Section 5.5 Classification of cross sections (p. 40)

        #region Table 5.2: Maximum width-to-thickness ratios for compression parts - Internal compression parts

        public void EN_1993_1_1_55_y1()
        {




            #region // Web in y direction general for many cross sections

            //double cy1; //!!!
            // double ty1; //!!!
            int _CS_BCy1_case = 1; //!!!
            double alpha_BCy1 = 0.5; //!!!

            double sigmay1_1 = -150;
            double sigmay1_2 = 100;
            double psi_BCy1 = sigmay1_2 / sigmay1_1;// !!!
            // loading of cross-section part
            // _CS_BCy1_case == 1 - Part subject to bending
            // _CS_BCy1_case == 2 - Part subject to compression
            // _CS_BCy1_case == 3 - Part subject to bending and compression

            // _CS_BCy1_case == 1 - Part subject to bending
            if (_CS_BCy1_case == 1)
            {
                if (cy1 / ty1 <= 72 * epsilon_fy) class_y1 = 1;
                else if (cy1 / ty1 <= 83 * epsilon_fy) class_y1 = 2;
                else if (cy1 / ty1 <= 124 * epsilon_fy) class_y1 = 3;
                else if (cy1 / ty1 > 124 * epsilon_fy) class_y1 = 4;///    !!!
            }

            else if (_CS_BCy1_case == 2)
            {
                if (cy1 / ty1 <= 33 * epsilon_fy) class_y1 = 1;
                {
                    if (cy1 / ty1 <= 38 * epsilon_fy) class_y1 = 2;
                    {
                        if (cy1 / ty1 <= 42 * epsilon_fy) class_y1 = 3;
                        else class_y1 = 4;
                    }
                }

            }
            else if (_CS_BCy1_case == 3)
            {
                if ((alpha_BCy1 > 0.5 && cy1 / ty1 <= (396 * epsilon_fy) / (13 * alpha_BCy1 - 1)) || (alpha_BCy1 <= 0.5 && cy1 / ty1 <= (36 * epsilon_fy) / (alpha_BCy1))) class_y1 = 1;
                {
                    if ((alpha_BCy1 > 0.5 && cy1 / ty1 <= (456 * epsilon_fy) / (13 * alpha_BCy1 - 1)) || (alpha_BCy1 <= 0.5 && cy1 / ty1 <= (41.5 * epsilon_fy) / (alpha_BCy1))) class_y1 = 2;
                    {
                        if ((psi_BCy1 > -1 && cy1 / ty1 <= (42 * epsilon_fy) / (0.67 * +0.33 * psi_BCy1)) || ((psi_BCy1 <= -1 && cy1 / ty1 <= (62 * epsilon_fy) * (1 - psi_BCy1) * Math.Sqrt(-psi_BCy1)))) class_y1 = 3;
                        else class_y1 = 4;
                    }
                }

            }

            // EN 1993-1-5 Section 4.4 p. 18

            double ro_y1;
            double lambda_p_bar_y1;
            double b_bar_y1 = cy1; // depends on cross-section shape
            double k_sigma_BCy1;

            if (psi_BCy1 == 1) k_sigma_BCy1 = 4;
            else if (1 > psi_BCy1 && psi_BCy1 > 0) k_sigma_BCy1 = 8.2 / (1.05 + psi_BCy1);
            else if (psi_BCy1 == 0) k_sigma_BCy1 = 7.81;
            else if (0 > psi_BCy1 && psi_BCy1 > -1) k_sigma_BCy1 = 7.81 - 6.29 * psi_BCy1 + 9.78 * Math.Pow(psi_BCy1, 2);
            else if (psi_BCy1 == -1) k_sigma_BCy1 = 23.9;
            else if (-1 > psi_BCy1 && psi_BCy1 > -3) k_sigma_BCy1 = 5.98 * Math.Pow((1 - psi_BCy1), 2);
            else k_sigma_BCy1 = 4; /// !!!!!!!!


            lambda_p_bar_y1 = (b_bar_y1 / ty1) / (28.4 * epsilon_fy * Math.Sqrt(k_sigma_BCy1));

            if (lambda_p_bar_y1 <= 0.673) ro_y1 = 1;
            else if ((lambda_p_bar_y1 > 0.673) && ((3 + psi_BCy1) > 0)) ro_y1 = (lambda_p_bar_y1 - 0.055 * (3 + psi_BCy1)) / Math.Pow(lambda_p_bar_y1, 2);
            else ro_y1 = 1; //!!!!!!!!

            if (ro_y1 > 1) ro_y1 = Math.Min(1, ro_y1);

            // EN 1993-1-5 Table 4.1 - p. 20

            double beff_y1;
            double be1_y1;
            double be2_y1;

            if (psi_BCy1 == 1)
            {
                beff_y1 = ro_y1 * b_bar_y1;
                be1_y1 = 0.5 * beff_y1;
                be2_y1 = 0.5 * beff_y1;
            }
            else if (1 > psi_BCy1 && psi_BCy1 >= 0)
            {
                beff_y1 = ro_y1 * b_bar_y1;
                be1_y1 = (2 / (5 - psi_BCy1)) * beff_y1;
                be2_y1 = beff_y1 - be1_y1;
            }
            else if (0 < psi_BCy1)
            {
                beff_y1 = ro_y1 * (b_bar_y1 / (1 - psi_BCy1));
                be1_y1 = 0.4 * beff_y1;
                be2_y1 = 0.6 * beff_y1;
            }








            #endregion

        }
        public void EN_1993_1_1_55_z1()
        {
            #region // Web in z direction (closed sections) SHS RHS and welded sections

            //double cz1; //!!!
            //double tz1; //!!!
            int _CS_BCz1_case = 2; //!!!
            double alpha_BCz1 = 0.5; //!!!


            double sigmaz1_1 = -150;
            double sigmaz1_2 = 100;
            double psi_BCz1 = sigmaz1_2 / sigmaz1_1;
            // loading of cross-section part
            // _CS_BCz1_case == 1 - Part subject to bending
            // _CS_BCz1_case == 2 - Part subject to compression
            // _CS_BCz1_case == 3 - Part subject to bending and compression

            // _CS_BCz1_case == 1 - Part subject to bending
            if (_CS_BCz1_case == 1)
            {
                if (cz1 / tz1 <= 72 * epsilon_fy) class_z1 = 1;
                else if (cz1 / tz1 <= 83 * epsilon_fy) class_z1 = 2;
                else if (cz1 / tz1 <= 124 * epsilon_fy) class_z1 = 3;
                else class_z1 = 4;

            }
            else if (_CS_BCz1_case == 2)
            {
                if (cz1 / tz1 <= 33 * epsilon_fy) class_z1 = 1;
                else if (cz1 / tz1 <= 38 * epsilon_fy) class_z1 = 2;
                else if (cz1 / tz1 <= 42 * epsilon_fy) class_z1 = 3;
                else class_z1 = 4;

            }
            else if (_CS_BCz1_case == 3)
            {
                if ((alpha_BCz1 > 0.5 && cz1 / tz1 <= (396 * epsilon_fy) / (13 * alpha_BCz1 - 1)) || (alpha_BCz1 <= 0.5 && cz1 / tz1 <= (36 * epsilon_fy) / (alpha_BCz1))) class_z1 = 1;
                else if ((alpha_BCz1 > 0.5 && cz1 / tz1 <= (456 * epsilon_fy) / (13 * alpha_BCz1 - 1)) || (alpha_BCz1 <= 0.5 && cz1 / tz1 <= (41.5 * epsilon_fy) / (alpha_BCz1))) class_z1 = 2;
                else if ((psi_BCz1 > -1 && cz1 / tz1 <= (42 * epsilon_fy) / (0.67 * +0.33 * psi_BCz1)) || ((psi_BCz1 <= -1 && cz1 / tz1 <= (62 * epsilon_fy) * (1 - psi_BCz1) * Math.Sqrt(-psi_BCz1)))) class_z1 = 3;
                else class_z1 = 4;

                // EN 1993-1-5 Section 4.4 p. 18

                double ro_z1;
                double lambda_p_bar_z1;
                double b_bar_z1 = cz1; // depends on cross-section shape
                double k_sigma_BCz1;

                if (psi_BCz1 == 1) k_sigma_BCz1 = 4;
                else if (1 > psi_BCz1 && psi_BCz1 > 0) k_sigma_BCz1 = 8.2 / (1.05 + psi_BCz1);
                else if (psi_BCz1 == 0) k_sigma_BCz1 = 7.81;
                else if (0 > psi_BCz1 && psi_BCz1 > -1) k_sigma_BCz1 = 7.81 - 6.29 * psi_BCz1 + 9.78 * Math.Pow(psi_BCz1, 2);
                else if (psi_BCz1 == -1) k_sigma_BCz1 = 23.9;
                else if (-1 > psi_BCz1 && psi_BCz1 > -3) k_sigma_BCz1 = 5.98 * Math.Pow((1 - psi_BCz1), 2);
                else k_sigma_BCz1 = 4; /// !!!!!!!!


                lambda_p_bar_z1 = (b_bar_z1 / ty1) / (28.4 * epsilon_fy * Math.Sqrt(k_sigma_BCz1));

                if (lambda_p_bar_z1 <= 0.673) ro_z1 = 1;
                else if ((lambda_p_bar_z1 > 0.673) && ((3 + psi_BCz1) > 0)) ro_z1 = (lambda_p_bar_z1 - 0.055 * (3 + psi_BCz1)) / Math.Pow(lambda_p_bar_z1, 2);
                else ro_z1 = 1; //!!!!!!!!

                if (ro_z1 > 1) ro_z1 = Math.Min(1, ro_z1);

                // EN 1993-1-5 Table 4.1 - p. 20

                double beff_z1;
                double be1_z1;
                double be2_z1;

                if (psi_BCz1 == 1)
                {
                    beff_z1 = ro_z1 * b_bar_z1;
                    be1_z1 = 0.5 * beff_z1;
                    be2_z1 = 0.5 * beff_z1;
                }
                else if (1 > psi_BCz1 && psi_BCz1 >= 0)
                {
                    beff_z1 = ro_z1 * b_bar_z1;
                    be1_z1 = (2 / (5 - psi_BCz1)) * beff_z1;
                    be2_z1 = beff_z1 - be1_z1;
                }
                else if (0 < psi_BCz1)
                {
                    beff_z1 = ro_z1 * (b_bar_z1 / (1 - psi_BCz1));
                    be1_z1 = 0.4 * beff_z1;
                    be2_z1 = 0.6 * beff_z1;
                }



            }

            #endregion
        }

        #endregion

        #region Table 5.2: Maximum width-to-thickness ratios for compression parts - Outstand flanges

        public void EN_1993_1_1_55_y2()
        {

            #region // Outstanding flanges (y direction) of angles

            //double cy2; //!!!
            //double ty2; //!!!
            int _CS_BCy2_case = 1; //!!!
            double alpha_BCy2 = 1; //!!!

            double psi_BCy2 = 1;// !!!
            // EN 1993-1-5
            double sigmay2_1 = -150;
            double sigmay2_2 = 100;
            double k_sigma_BCy2 = 11; // !!!!!!!

            // case 1
            psi_BCy2 = sigmay2_2 / sigmay2_1;

            if (psi_BCy2 == 1) k_sigma_BCy2 = 0.43;
            else if (psi_BCy2 == 0) k_sigma_BCy2 = 0.57;
            else if (psi_BCy2 == -1) k_sigma_BCy2 = 0.85;
            else if (psi_BCy2 <= 1 && psi_BCy2 >= -3) k_sigma_BCy2 = 0.57 - 0.21 * psi_BCy2 + 0.07 * Math.Pow(psi_BCy2, 2);

            // case 2

            psi_BCy2 = sigmay2_2 / sigmay2_1;

            if (psi_BCy2 == 1) k_sigma_BCy2 = 0.43;
            else if (psi_BCy2 < 1 && psi_BCy2 > 0) k_sigma_BCy2 = 0.578 / (psi_BCy2 + 0.34);
            else if (psi_BCy2 == 0) k_sigma_BCy2 = 1.7;
            else if (psi_BCy2 > 0 && psi_BCy2 > -1) k_sigma_BCy2 = 1.7 - 5 * psi_BCy2 + 17.1 * Math.Pow(psi_BCy2, 2);
            else if (psi_BCy2 == -1) k_sigma_BCy2 = 23.8;



            // loading of cross-section part
            // _CS_BCy2_case == 1 - Part subject to compression
            // _CS_BCy2_case == 31 - Part subject to bending and compression  - Tip in compression
            // _CS_BCy2_case == 32 - Part subject to bending and compression  - Tip in tension

            // _CS_BCy2_case == 1 - Part subject to bending
            if (_CS_BCy2_case == 1)
            {
                if (cy1 / ty2 <= 9 * epsilon_fy) class_y2 = 1;
                else if (cy2 / ty2 <= 10 * epsilon_fy) class_y2 = 2;
                else if (cy2 / ty2 <= 14 * epsilon_fy) class_y2 = 3;
                else class_y2 = 4;

            }
            else if (_CS_BCy2_case == 31)
            {
                if (cy2 / ty2 <= 9 * epsilon_fy / alpha_BCy2) class_y2 = 1;
                else if (cy2 / ty2 <= 10 * epsilon_fy / alpha_BCy2) class_y2 = 2;
                else if (cy2 / ty2 <= 21 * epsilon_fy * Math.Sqrt(k_sigma_BCy2)) class_y2 = 3;
                else class_y2 = 4;

            }
            else if (_CS_BCy2_case == 32)
            {
                if (cy2 / ty2 <= 9 * epsilon_fy / (alpha_BCy2 * Math.Sqrt(alpha_BCy2))) class_y2 = 1;
                else if (cy2 / ty2 <= 10 * epsilon_fy / (alpha_BCy2 * Math.Sqrt(alpha_BCy2))) class_y2 = 2;
                else if (cy2 / ty2 <= 21 * epsilon_fy * Math.Sqrt(k_sigma_BCy2)) class_y2 = 3;
                else class_y2 = 4;

            }



            #endregion
        }
        public void EN_1993_1_1_55_z2()
        {

            #region // Outstanding flanges (z direction) - generally




            //double cz2; //!!!
            //double tz2; //!!!
            int _CS_BCz2_case = 1; //!!!
            double alpha_BCz2 = 0.5; //!!!

            double psi_BCz2 = 1;// !!!
            // EN 1993-1-5
            double sigmaz2_1 = -150;
            double sigmaz2_2 = 100;
            double k_sigma_BCz2 = 11; // !!!!!!!

            // case 1
            psi_BCz2 = sigmaz2_2 / sigmaz2_1;

            if (psi_BCz2 == 1) k_sigma_BCz2 = 0.43;
            else if (psi_BCz2 == 0) k_sigma_BCz2 = 0.57;
            else if (psi_BCz2 == -1) k_sigma_BCz2 = 0.85;
            else if (psi_BCz2 <= 1 && psi_BCz2 >= -3) k_sigma_BCz2 = 0.57 - 0.21 * psi_BCz2 + 0.07 * Math.Pow(psi_BCz2, 2);

            // case 2

            psi_BCz2 = sigmaz2_2 / sigmaz2_1;

            if (psi_BCz2 == 1) k_sigma_BCz2 = 0.43;
            else if (psi_BCz2 < 1 && psi_BCz2 > 0) k_sigma_BCz2 = 0.578 / (psi_BCz2 + 0.34);
            else if (psi_BCz2 == 0) k_sigma_BCz2 = 1.7;
            else if (psi_BCz2 > 0 && psi_BCz2 > -1) k_sigma_BCz2 = 1.7 - 5 * psi_BCz2 + 17.1 * Math.Pow(psi_BCz2, 2);
            else if (psi_BCz2 == -1) k_sigma_BCz2 = 23.8;



            // loading of cross-section part
            // _CS_BCz1_case == 1 - Part subject to compression
            // _CS_BCz1_case == 31 - Part subject to bending and compression  - Tip in compression
            // _CS_BCz1_case == 32 - Part subject to bending and compression  - Tip in tension

            // _CS_BCz1_case == 1 - Part subject to bending
            if (_CS_BCz2_case == 1)
            {
                if (cz1 / tz2 <= 9 * epsilon_fy) class_z2 = 1;
                else if (cz2 / tz2 <= 10 * epsilon_fy) class_z2 = 2;
                else if (cz2 / tz2 <= 14 * epsilon_fy) class_z2 = 3;
                else class_z2 = 4;

            }
            else if (_CS_BCz2_case == 31)
            {
                if (cz2 / tz2 <= 9 * epsilon_fy / alpha_BCz2) class_z2 = 1;
                else if (cz2 / tz2 <= 10 * epsilon_fy / alpha_BCz2) class_z2 = 2;
                else if (cz2 / tz2 <= 21 * epsilon_fy * Math.Sqrt(k_sigma_BCz2)) class_z2 = 3;
                else class_z2 = 4;

            }
            else if (_CS_BCz2_case == 32)
            {
                if (cz2 / tz2 <= 9 * epsilon_fy / (alpha_BCz2 * Math.Sqrt(alpha_BCz2))) class_z2 = 1;
                else if (cz2 / tz2 <= 10 * epsilon_fy / (alpha_BCz2 * Math.Sqrt(alpha_BCz2))) class_z2 = 2;
                else if (cz2 / tz2 <= 21 * epsilon_fy * Math.Sqrt(k_sigma_BCz2)) class_z2 = 3;
                else class_z2 = 4;

            }


            #endregion

        }

        #endregion

        #endregion

        public void EN_1993_1_1_55()
        {



            switch (csprofshape)
            {
                case 0:
                    classall = 1;
                    break;
                case 1:
                    #region csprofshape = 1  (I and H section)
                    this.EN_1993_1_1_55_y1();
                    this.EN_1993_1_1_55_z2();
                    class_w = class_y1;
                    class_f = class_z2;
                    classall = Math.Max(class_w, class_f);
                    #endregion
                    break;
                case 2:
                    #region csprofshape = 2  (U section)
                    this.EN_1993_1_1_55_y1();
                    this.EN_1993_1_1_55_z2();
                    class_w = class_y1;
                    class_f = class_z2;
                    classall = Math.Max(class_w, class_f);
                    #endregion
                    break;
                case 3:
                    #region csprofshape = 3  (LR section)
                    this.EN_1993_1_1_55_y2();
                    this.EN_1993_1_1_55_z2();

                    #region Table 5.2: Maximum width-to-thickness ratios for compression parts - Angles

                    if (csprofshape == 3)
                    {

                        double h_ang = h; // !!!!!!!
                        double b_ang = b; // !!!!!!!
                        double t_ang = tw; // !!!!!!!
                        // int class_ang1;
                        int _CS_BCang_case = 1;



                        if (_CS_BCang_case == 1)
                        {
                            if ((h_ang / t_ang <= 15 * epsilon_fy) || ((b_ang + h_ang) / (2 * t_ang) <= 11.5 * epsilon_fy)) class_ang1 = 3;
                            else class_ang1 = 4;
                        }
                    }

                    #endregion

                    class_w = class_y2;
                    class_f = class_z2;
                    classall = Math.Max(class_w, class_f);
                    classall = Math.Max(classall, class_ang1);

                    #endregion
                    break;
                case 4:
                    #region csprofshape = 4  (LN section)
                    this.EN_1993_1_1_55_y2();
                    this.EN_1993_1_1_55_z2();

                    #region Table 5.2: Maximum width-to-thickness ratios for compression parts - Angles

                    if (csprofshape == 4)
                    {

                        double h_ang = h; // !!!!!!!
                        double b_ang = b; // !!!!!!!
                        double t_ang = tw; // !!!!!!!
                        // int class_ang1;
                        int _CS_BCang_case = 1;



                        if (_CS_BCang_case == 1)
                        {
                            if ((h_ang / t_ang <= 15 * epsilon_fy) || ((b_ang + h_ang) / (2 * t_ang) <= 11.5 * epsilon_fy)) class_ang1 = 3;
                            else class_ang1 = 4;
                        }
                    }

                    #endregion

                    class_w = class_y2;
                    class_f = class_z2;
                    classall = Math.Max(class_w, class_f);
                    classall = Math.Max(classall, class_ang1);

                    #endregion
                    break;
                case 5:
                    #region csprofshape = 5  (CHS section)
                    #region Table 5.2: Maximum width-to-thickness ratios for compression parts - Tubular sections
                    // Section in bending and/or compression
                    if (csprofshape == 5)
                    {

                        if (_D_tube / t_tube <= 50 * Math.Pow(epsilon_fy, 2)) classall = 1;
                        else if (_D_tube / t_tube <= 70 * Math.Pow(epsilon_fy, 2)) classall = 2;
                        else if (_D_tube / t_tube <= 90 * Math.Pow(epsilon_fy, 2)) classall = 3;
                        else
                        {
                            classall = 4; // see EN 1993-1-6
                            MessageBox.Show(" NOTE: For d / t > 90ε2 see EN 1993-1-6. ");
                        }
                    }

                    #endregion
                    #endregion
                    break;
                case 6:
                    #region csprofshape = 6  (SHS section)
                    this.EN_1993_1_1_55_y1();
                    this.EN_1993_1_1_55_z1();
                    class_w = class_y1;
                    class_f = class_z1;
                    classall = Math.Max(class_w, class_f);

                    #endregion
                    break;
                case 7:
                    #region csprofshape = 7  (RHS section)
                    this.EN_1993_1_1_55_y1();
                    this.EN_1993_1_1_55_z1();
                    class_w = class_y1;
                    class_f = class_z1;
                    classall = Math.Max(class_w, class_f);

                    #endregion
                    break;
                case 8:
                    #region csprofshape = 8  (Slimdek - I asymetrical section)
                    this.EN_1993_1_1_55_y1();
                    this.EN_1993_1_1_55_z2();
                    class_w = class_y1;
                    class_f = class_z2;
                    classall = Math.Max(class_w, class_f);

                    #endregion
                    break;
                case 9:
                    #region csprofshape = 9  (T section)
                    this.EN_1993_1_1_55_y2();
                    this.EN_1993_1_1_55_z2();
                    class_w = class_y2;
                    class_f = class_z2;
                    classall = Math.Max(class_w, class_f);

                    #endregion
                    break;

            }





        }

        public void EN_1993_1_1_62()
        {
            // CHAPTER 6 Ultimate limit states (p.47)
            // Section 6.1 General (p. 47)


            // Steel check EN1993-1-1
            #region EN_1993_1_1_62

            #region Auxiliary calculations


            #region USER - Internal forces
            // Axial force - _NEd_t or _NEd_p         

            if (_NEd <= 0)
            {
                _NEd_p = Math.Abs(_NEd);
                _NEd = Math.Abs(_NEd);
                _NEd_t = 0.0000000000000000000000000000000001;
            }
            else if (_NEd > 0)
            {
                _NEd_t = Math.Abs(_NEd);
                _NEd_p = 0.0000000000000000000000000000000001;
            }


            // Negative internal axial force from solution for compression conversion to general possitive value NEd used in check formulas



            // Torsial moment        
            _Tx_Ed = _Mx_Ed;

            // Bending moments - beam nodes - Annex A

            _My_1a = _My_a;
            _My_1b = _My_b;

            _Mz_1a = _Mz_a;
            _Mz_1b = _Mz_b;

            _My_s = _My_s;
            _Mz_s = _Mz_s;



            #endregion

            #region Cross-section - geometry
            bf1 = b;
            bf2 = b;
            tw1 = tw;
            tw2 = tw;
            tf1 = tf;
            tf2 = tf;

            // NcrT and NcrTF variables

            if (cstype == 1)
            {
                if (csprofshape == 1 || csprofshape == 5 || csprofshape == 6 || csprofshape == 7 || csprofshape == 10)
                {
                    y0 = 0;
                    z0 = 0;
                }

                else if (csprofshape == 2)
                {
                    y0 = ym;
                    z0 = 0;
                }
                else if (csprofshape == 3)
                {//Wrong ??
                    y0 = aη;
                    z0 = 0;
                }
                else if (csprofshape == 4)
                {//Wrong
                    y0 = ey - tw / 2;
                    z0 = ez - tf / 2;
                }
                else if (csprofshape == 8)
                {
                    y0 = 0;
                    //Wrong
                    z0 = epl - eel;
                }
                else if (csprofshape == 9)
                {
                    y0 = 0;
                    //Wrong
                    z0 = e;
                }
            }

            else
            {
                // Just until welded cross-section will be defined
                y0 = 0;
                z0 = 0;
            }


            i0_r = Math.Sqrt(((iy_r * iy_r) + (iz_r * iz_r) + (y0 * y0) + (z0 * z0)));

            #endregion

            #region Cross-section - class

            if (class_w >= class_f) classall = class_w;
            else classall = class_f;

            #endregion

            #region Cross-section - class 4

            if (classall <= 2)
            {

                _Aeff = _A;
                _Ay_v_eff = _Ay_v;
                _Az_v_eff = _Az_v;
                _Wy_eff = _Wy_pl;
                _Wz_eff = _Wz_pl;
                _Wy_eff_min = _Wy_pl;
                _Wz_eff_min = _Wz_pl;
                _Iy_eff = _Iy;
                _Iz_eff = _Iz;
                iy_r_eff = iy_r;
                iz_r_eff = iz_r;

            }
            else if (classall == 3)
            {

                _Aeff = _A;
                _Ay_v_eff = _Ay_v;
                _Az_v_eff = _Az_v;
                _Wy_eff = _Wy_el;
                _Wz_eff = _Wz_el;
                _Wy_eff_min = _Wy_el;
                _Wz_eff_min = _Wz_el;
                _Iy_eff = _Iy;
                _Iz_eff = _Iz;
                iy_r_eff = iy_r;
                iz_r_eff = iz_r;

            }



            #endregion

            #region Cross-section - Wi
            // Wi definition for various classes of cross-section (p. 61)
            if (classall <= 2) _Wy = _Wy_pl;
            else if (classall == 3) _Wy = _Wy_el;
            else _Wy = _Wy_eff;  //classall == 4


            if (classall <= 2) _Wz = _Wz_pl;
            else if (classall == 3) _Wz = _Wz_el;
            else _Wz = _Wz_eff; //classall == 4
            #endregion

            #endregion

            #region Section 6.2 Resistance of cross-sections (p. 47)
            // Section 6.2 Resistance of cross-sections (p. 47)

            #region Section 6.2.3 Tension (p. 50)
            // Section 6.2.3 Tension (p. 50)


            // The design plastic resistance of the cross-section (6.6)
            _Npl_Rd = (_A * fy) / gamaM0; // (6.6)
            // The design ultimate resistance of the net cross-section at holes for fasteners
            _Nu_Rd = (0.9 * _Anet * fu) / gamaM2; // (6.7)
            // The design tension resistance Nt,Rd in 6.2.3(1) of the net section at holes for fasteners in category C connections (see EN 1993-1-8, 3.1.1(4))
            _Nnet_Rd = (_Anet * fy) / gamaM0; // (6.8)

            // The design tension resistance
            if ((_Npl_Rd < _Nu_Rd) && (_Npl_Rd < _Nnet_Rd)) _Nt_Rd = _Npl_Rd;

            // using of connections category "C"
            else _Nt_Rd = Math.Min(_Nu_Rd, _Nnet_Rd);



            #endregion

            #region Section 6.2.4 Compression (p. 50)
            // Section 6.2.4 Compression (p. 50)

            // The design resistance of the cross-section for uniform compression

            if (classall <= 3) _Nc_Rd = _A * fy / gamaM0;
            else if (classall == 4) _Nc_Rd = _Aeff * fy / gamaM0; // (6.10 and 6.11)
            #endregion

            #region Section 6.2.5 Bending moment (p. 51)
            // Section 6.2.5 Bending moment (p. 51)
            // The design resistance for bending about one principal axis of a cross-section



            _My_pl_Rd = _Wy_pl * fy / gamaM0;
            _Mz_pl_Rd = _Wz_pl * fy / gamaM0;

            _My_el_Rd = _Wy_el * fy / gamaM0;
            _Mz_el_Rd = _Wz_el * fy / gamaM0;



            if (classall <= 2) _My_c_Rd = _Wy_pl * fy / gamaM0; // (6.13)
            if (classall == 3) _My_c_Rd = _Wy_el_min * fy / gamaM0; // (6.14)
            if (classall == 4) _My_c_Rd = _Wy_eff_min * fy / gamaM0; // (6.15)

            if (classall <= 2) _Mz_c_Rd = _Wz_pl * fy / gamaM0;
            if (classall == 3) _Mz_c_Rd = _Wz_el_min * fy / gamaM0;
            if (classall == 4) _Mz_c_Rd = _Wz_eff_min * fy / gamaM0;

            #endregion

            #region Section 6.2.6 Shear (p. 52)
            // Section 6.2.6 Shear (p. 52)
            // The design shear resistance


            // The design plastic shear resistance - in the absence of torsion
            double _Vy_pl_Rd = _Ay_v * (fy / Math.Sqrt(3)) / gamaM0; // (6.18)
            double _Vz_pl_Rd = _Az_v * (fy / Math.Sqrt(3)) / gamaM0; //(6.18)
            // !!!!!!!!! skontrolovat vypocet Ay a Az v databaze!!!!!!!!!!!!!!!

            // Shear stress
            double tau_y_Ed = (_Vy_Ed * _Sy) / (_Iy * tw1);
            double tau_z_Ed = (_Vz_Ed * _Sz) / (_Iz * tf1);

            double tau_Rd = fy / (Math.Sqrt(3) * gamaM0);

            // The design elastic shear resistance - in the absence of torsion

            double _Vy_el_Rd = (_Iy * tw1 / _Sy) * tau_Rd;
            double _Vz_el_Rd = (_Iz * tw1 / _Sz) * tau_Rd;

            if (classall <= 3)
            {
                _Vy_c_Rd = _Vy_pl_Rd;
                _Vz_c_Rd = _Vz_pl_Rd;
            }
            else
            {
                _Vy_c_Rd = _Vy_el_Rd;
                _Vz_c_Rd = _Vz_el_Rd;
            }
            // !!! Influence of local web buckling is missing !!!




            #endregion

            #region Section 6.2.7 Torsion (p. 53)


            // NAD Czech republic NB p. 94 Table NB 2.1

            // Okrajové podmínky 

            switch (torsion_koeficientNADCZ)
            {
                case 0:
                    alpha_T = 3.1;
                    beta_T = 1;
                    break;
                case 1:
                    alpha_T = 3.7;
                    beta_T = 1.08;
                    break;
                case 2:
                    alpha_T = 8.0;
                    beta_T = 1.25;
                    break;
                case 3:
                    alpha_T = 5.6;
                    beta_T = 1.00;
                    break;
                case 4:
                    alpha_T = 6.9;
                    beta_T = 1.14;
                    break;
                case 5:
                    alpha_T = 2.7;
                    beta_T = 1.11;
                    break;
            }


            // NAD Czech Republic NB (p. 94)



            _Kt_T = _L_teor * Math.Pow(((_G * _IT) / (_E * _Iw)), 0.5);
            kappa_T = 1 / (beta_T + Math.Pow((alpha_T / _Kt_T), 2));

            _BEd = _MEd_T * e_T * (1 - kappa_T);
            // internal St. Venant torsion
            _Tt_Ed = _VEd_T * e_T * kappa_T;
            // internal warping torsion.
            _Tw_Ed = _VEd_T * e_T * (1 - kappa_T);
            _Tx_Ed = _Tt_Ed + _Tw_Ed;



            // Vysecova suradnica
            // Načítať z databazy alebo vypocitat podla tvaru prierezu
            // I and H cross-section

            if (csprofshape == 1 || csprofshape == 8) //Slimdek missing formula!!!
            {
                omega_w = 1 / 4 * b * (h - tf);
                _Sw = 1 / 16 * b * b * tf * (h - tf);
            }
            if (csprofshape == 3 || csprofshape == 4)
            {
                omega_w = 0;// missing formula???
            }
            if (csprofshape == 5)
            {
                omega_w = 2 * Math.PI * Math.Pow((_D_tube - t_tube), 2);
            }
            if (csprofshape == 6 || csprofshape == 7)
            {
                omega_w = 2 * (h - tf) * (b - tw);
            }
            if (csprofshape == 9)
            {
                omega_w = 0;// missing formula???
            }





            // I and H cross-section
            double sigma_x_Ed_w;
            // the direct stresses σw,Ed due to the bimoment BEd
            sigma_x_Ed_w = _BEd / _Iw * omega_w;

            // the shear stresses τt,Ed due to St. Venant torsion Tt,Ed

            // depends on cross-section shape !!!!!!!!!! not finished
            //web
            double tau_Tw_Ed;
            tau_Tw_Ed = _Tt_Ed / _IT * tw;
            //flange
            double tau_Tf_Ed;
            tau_Tf_Ed = _Tt_Ed / _IT * tf;

            //  and shear stresses τw,Ed due to warping torsion Tw,Ed
            //flange
            double tau_Wf_Ed;
            tau_Wf_Ed = (_Tw_Ed * _Sw) / (_Iw * tf);


            // Maximum sigma x
            double sigma_x_Ed_b = sigma_x_Ed;
            double sigma_x_Ed_max;

            sigma_x_Ed_max = sigma_x_Ed_b + sigma_x_Ed_w;


            // shear stress in flange
            double tau_y_Ed_max;
            tau_y_Ed_max = tau_y_Ed + tau_Tf_Ed + tau_Wf_Ed;

            // Shear stress in web
            double tau_z_Ed_max;
            double tau_Ww_Ed = 0;
            tau_z_Ed_max = tau_z_Ed + tau_Tw_Ed + tau_Ww_Ed;

            // Check ratio for Torsial moment

            double ratio_623x;
            double ratio_623y;
            double ratio_623z;

            ratio_623x = sigma_x_Ed_max / (fy / gamaM1);
            ratio_623y = tau_y_Ed_max / (1 / (Math.Sqrt(3)) * (fy / gamaM0));
            ratio_623z = tau_z_Ed_max / (1 / (Math.Sqrt(3)) * (fy / gamaM0));

            double ratio_a1 = Math.Max(ratio_623x, ratio_623y);
            double ratio_a2 = Math.Max(ratio_a1, ratio_623z);




            // Srovnavaci napjeti Hubert - Mises - Hencky
            double ratio_623f;
            double ratio_623w;
            ratio_623f = (Math.Sqrt(Math.Pow(sigma_x_Ed_max, 2) + 3 * tau_y_Ed_max)) / (fy / gamaM0);
            ratio_623w = (Math.Sqrt(Math.Pow(sigma_x_Ed_b, 2) + 3 * tau_z_Ed_max)) / (fy / gamaM0);

            double ratio_a3 = Math.Max(ratio_623f, ratio_623w);

            // Auxiliary ratio

            ratio_a4 = Math.Max(ratio_a2, ratio_a3);



            // Section 6.2.7 Torsion (p. 53)

            double _T_Rd_t = (fy / (Math.Sqrt(3) * gamaM0)) * _Wt; //! missing formula
            double _T_Rd_w = (fy / (Math.Sqrt(3) * gamaM0)) * (_Iw * tf / _Sw); //! wrong or missing formula !! tf ???

            _T_Rd = _T_Rd_t + _T_Rd_w;  // !!! missing formula or DATABASE data
            // The design plastic shear resistance for combined shear force and torsional moment the plastic shear resistance accounting for torsional effects

            double tau_y_T_Ed = tau_Tf_Ed;  // missing formula or DATABASE data
            double tau_z_T_Ed = tau_Tw_Ed;  // missing formula or DATABASE data
            double tau_y_W_Ed = tau_Wf_Ed;   // missing formula or DATABASE data
            double tau_z_W_Ed = tau_Ww_Ed;   // missing formula or DATABASE data

            double _Vy_pl_T_Rd;
            double _Vz_pl_T_Rd;


            if (csprofshape == 1) _Vy_pl_T_Rd = (Math.Sqrt(1 - (tau_y_T_Ed / 1.25 * (fy / Math.Sqrt(3)) * gamaM0))) * _Vy_pl_Rd; // (6.26)
            else if (csprofshape == 2) _Vy_pl_T_Rd = ((Math.Sqrt(1 - (tau_y_T_Ed / 1.25 * (fy / Math.Sqrt(3)) * gamaM0))) - tau_y_W_Ed / (fy / Math.Sqrt(3)) * gamaM0) * _Vy_pl_Rd; // (6.27)
            else if (csprofshape >= 5 && csprofshape <= 7) _Vy_pl_T_Rd = (1 - (tau_y_T_Ed / ((fy / Math.Sqrt(3)) * gamaM0))) * _Vy_pl_Rd; // (6.28)
            else _Vy_pl_T_Rd = _Vy_pl_Rd; // Auxiliary !!!!


            if (csprofshape == 1) _Vz_pl_T_Rd = (Math.Sqrt(1 - (tau_z_T_Ed / 1.25 * (fy / Math.Sqrt(3)) * gamaM0))) * _Vz_pl_Rd; // (6.26)
            else if (csprofshape == 2) _Vz_pl_T_Rd = ((Math.Sqrt(1 - (tau_z_T_Ed / 1.25 * (fy / Math.Sqrt(3)) * gamaM0))) - tau_z_W_Ed / (fy / Math.Sqrt(3)) * gamaM0) * _Vz_pl_Rd; // (6.27)
            else if (csprofshape >= 5 && csprofshape <= 7) _Vz_pl_T_Rd = (1 - (tau_z_T_Ed / ((fy / Math.Sqrt(3)) * gamaM0))) * _Vz_pl_Rd; // (6.28)
            else _Vz_pl_T_Rd = _Vz_pl_Rd; // Auxiliary !!!!



            #endregion

            #region Section 6.2.8 Bending and shear (p. 54)
            // Section 6.2.8 Bending and shear (p. 54)

            double ro_V_y;
            double ro_V_z;


            if (_Vy_Ed <= 0.5 * _Vy_pl_Rd) ro_V_y = 0;
            else ro_V_y = Math.Pow(((2 * _Vy_Ed / _Vy_pl_Rd) - 1), 2);

            if (_Vz_Ed <= 0.5 * _Vz_pl_Rd) ro_V_z = 0;
            else ro_V_z = Math.Pow(((2 * _Vz_Ed / _Vz_pl_Rd) - 1), 2);




            // (4) Bending, shear and torsion
            double ro_V_y_T;
            double ro_V_z_T;

            if (_Vy_Ed <= 0.5 * _Vy_pl_T_Rd) ro_V_y_T = 0;
            if (_Tx_Ed == 0) ro_V_y_T = ro_V_y;
            else ro_V_y_T = Math.Pow(((2 * _Vy_Ed / _Vy_pl_T_Rd) - 1), 2);

            if (_Vz_Ed <= 0.5 * _Vz_pl_T_Rd) ro_V_z_T = 0;
            if (_Tx_Ed == 0) ro_V_z_T = ro_V_z;
            else ro_V_z_T = Math.Pow(((2 * _Vz_Ed / _Vz_pl_T_Rd) - 1), 2);


            // Interaction of shear force, bending moment and axial force - EN 1993-1-5 (Section 7)

            // The reduced design plastic resistance moment for combination of bending and shear

            if (_Tx_Ed == 0)
            {

                if (_Vy_Ed > 0.5 * _Vy_pl_Rd)
                {
                    if (csprofshape == 1) _My_V_pl_Rd = _Wy_pl - (((ro_V_z * Math.Pow(_Az_v, 2)) / (4 * tw1))) * fy / gamaM0; // (6.30) - symmetrical I section with identical flanges
                    else
                    {
                        _My_V_pl_Rd = _My_pl_Rd * (((1 - ro_V_z) * fy) / fy); // Section 6.2.8 (3) see (6.29) - general shapes of sections
                        _My_V_el_Rd = _My_el_Rd * (((1 - ro_V_z) * fy) / fy);
                    }
                }
                if (_Vz_Ed > 0.5 * _Vz_pl_Rd)
                {
                    if (csprofshape == 1) _Mz_V_pl_Rd = _Wz_pl - (((ro_V_y * Math.Pow(_Ay_v, 2)) / (4 * 2 * tf1))) * fy / gamaM0; // (6.30) - symmetrical I section with identical flanges
                    else
                    {
                        _Mz_V_pl_Rd = _Mz_pl_Rd * (((1 - ro_V_y) * fy) / fy); // Section 6.2.8 (3) see (6.29) - general shapes of sections
                        _Mz_V_el_Rd = _Mz_el_Rd * (((1 - ro_V_y) * fy) / fy);
                    }
                }
            }
            

            // The reduced design plastic resistance moment for combination of bending, shear and torsion

            if (_Tx_Ed != 0)
            {

            if (_Vy_Ed > 0.5 * _Vy_pl_T_Rd)
            {
                if (csprofshape == 1) _My_V_pl_Rd = _Wy_pl - (((ro_V_z_T * Math.Pow(_Az_v, 2)) / (4 * tw1))) * fy / gamaM0; // (6.30)- symmetrical I section with identical flanges
                else
                    {
                    _My_V_pl_Rd = _My_pl_Rd * (((1 - ro_V_z_T) * fy) / fy); // Section 6.2.8 (3) see (6.29) - general shapes of sections
                    _My_V_el_Rd = _My_el_Rd * (((1 - ro_V_z_T) * fy) / fy);
                    }
            }
            if (_Vz_Ed > 0.5 * _Vz_pl_T_Rd)
            {
                if (csprofshape == 1) _Mz_V_pl_Rd = _Wz_pl - (((ro_V_y_T * Math.Pow(_Ay_v, 2)) / (4 * 2 * tf1))) * fy / gamaM0; // (6.30)- symmetrical I section with identical flanges
                else
                    {
                    _Mz_V_pl_Rd = _Mz_pl_Rd * (((1 - ro_V_y_T) * fy) / fy); // Section 6.2.8 (3) see (6.29) - general shapes of sections
                    _Mz_V_el_Rd = _Mz_el_Rd * (((1 - ro_V_y_T) * fy) / fy);
                    }
            }
            }
            















            // Basic resistance values !!!

            if (_My_V_pl_Rd <= _My_c_Rd) _My_V_pl_Rd = _My_c_Rd;
            if (_Mz_V_pl_Rd <= _Mz_c_Rd) _Mz_V_pl_Rd = _Mz_c_Rd;

            if (_My_V_el_Rd <= _My_c_Rd) _My_V_el_Rd = _My_c_Rd;
            if (_Mz_V_el_Rd <= _Mz_c_Rd) _Mz_V_el_Rd = _Mz_c_Rd;


            #endregion

            #region Section 6.2.9 Bending and axial force-tension (p. 55)
            // Section 6.2.9 Bending and axial force-tension (p. 55)
            // Section 6.2.9.1 Class 1 and 2 cross-sections (p. 55)



            if (cssolidshape == 101 || cssolidshape == 102) _My_N_Rd = _My_pl_Rd * (1 - Math.Pow((_NEd_t / _Npl_Rd), 2)); // (6.32)

            if (csprofshape == 1 || csprofshape == 2)
                if (_NEd_t <= 0.25 * _Npl_Rd && _NEd_t <= 0.5 * hw1 * tw1 * fy / gamaM0) _My_N_Rd = _My_pl_Rd; // (6.33) and (6.34)

            if (csprofshape == 1 || csprofshape == 2)
                if (_NEd_t <= hw1 * tw1 * fy / gamaM0) _Mz_N_Rd = _Mz_pl_Rd; // (6.35)

            double n_tens;
            double a_tens;

            n_tens = _NEd_t / _Npl_Rd;

            if ((_A - 2 * bf1 * tf1) / _A < 0.5) a_tens = 0.5;
            else a_tens = (_A - 2 * bf1 * tf1) / _A;


            if (csprofshape == 1 || csprofshape == 2)
            {
                _My_N_Rd = _My_pl_Rd * (1 - n_tens) / (1 - 0.5 * a_tens); // (6.36)
                if (_My_N_Rd > _My_pl_Rd) _My_N_Rd = _My_pl_Rd; // (6.36)
            }
            if (n_tens <= a_tens) _Mz_N_Rd = _Mz_pl_Rd; // (6.37)
            else _Mz_N_Rd = _Mz_pl_Rd * (1 - Math.Pow(((n_tens - a_tens) / (1 - a_tens)), 2)); // (6.38)

            /* For cross-sections where fastener holes are not to be accounted for, the following approximations may be used for rectangular structural hollow sections of uniform thickness and for welded box sections with equal flanges and equal webs */

            double aw_tens;
            double af_tens;

            if ((_A - 2 * bf1 * tf1) / _A < 0.5) aw_tens = 0.5;
            else aw_tens = (_A - 2 * bf1 * tf1) / _A;

            if ((_A - 2 * h * tw1) / _A < 0.5) af_tens = 0.5;
            else af_tens = (_A - 2 * h * tw1) / _A;

            if (csprofshape == 6 || csprofshape == 7 || csweldedshape == 204)
            {
                _My_N_Rd = _My_pl_Rd * (1 - n_tens) / (1 - 0.5 * aw_tens); // (6.39)
                if (_My_N_Rd > _My_pl_Rd) _My_N_Rd = _My_pl_Rd; // (6.39)
            }
            if (csprofshape == 6 || csprofshape == 7 || csweldedshape == 204)
            {
                _Mz_N_Rd = _Mz_pl_Rd * (1 - n_tens) / (1 - 0.5 * af_tens); // (6.40)
                if (_Mz_N_Rd > _Mz_pl_Rd) _Mz_N_Rd = _Mz_pl_Rd; // (6.40)
            }


            // Bi-axial bending

            double alfa_tens;
            double beta_tens;

            if (csprofshape == 1 || csweldedshape <= 203) alfa_tens = 2;
            if (csprofshape == 1 || csweldedshape <= 203)
            {
                beta_tens = 5 * n_tens;
                if (beta_tens >= 1) beta_tens = 1;
            }

            if (csprofshape == 5) alfa_tens = 2;
            if (csprofshape == 5) beta_tens = 2;

            if (csprofshape == 6 || csprofshape == 7 || csweldedshape == 204)
            {
                alfa_tens = 1.66 / (1 - 1.13 * Math.Pow(n_tens, 2));
                if (alfa_tens > 6) alfa_tens = 6;
                beta_tens = alfa_tens;
            }
            // Check formulas - see 6.2.5 (6.16) Af.net and Anet influence !!!

            // Cross-sections class 3 see and control 6.2.3, 6.2.4, 6.2.5 for Anet

            if (classall == 3)
            {
                if (_Anet == _A) sigma_x_Ed = _NEd_t / _A + _My_Ed / (_Wy * fy / gamaM0) + _Mz_Ed / (_Wz * fy / gamaM0);
                else sigma_x_Ed = _NEd_t / _Anet + _My_Ed / (_Wy * fy / gamaM0) + _Mz_Ed / (_Wz * fy / gamaM0);
            }
            // Cross-sections class 4
            if (classall == 4)
            {
                if (_Aeff_net == _Aeff) sigma_x_Ed = _NEd / _Aeff + (_My_Ed + _NEd * e_Ny) / (_Wy * fy / gamaM0) + (_Mz_Ed + _NEd * e_Nz) / (_Wz * fy / gamaM0);
                else sigma_x_Ed = _NEd / _Aeff_net + (_My_Ed + _NEd * e_Ny) / (_Wy * fy / gamaM0) + (_Mz_Ed + _NEd * e_Nz) / (_Wz * fy / gamaM0);
            }


            #endregion

            #region Section 6.2.10 Bending, shear and axial force
            // Section 6.2.10 Bending, shear and axial force

            //(2)
            if (_Vz_Ed < 0.5 * _Vz_pl_Rd) 

            {


            }
            // (3)
            else 
            {

                fy = (1 - ro_V_z) * fy;

            }


            #endregion

            #endregion
            #endregion

        }

        public void EN_1993_1_1_631()
        {
            #region Section 6.3 Buckling resistance of members (p. 56)
            // Section 6.3 Buckling resistance of members (p. 56)


            #region Section 6.3.1 (Table 6.1 and Table 6.2)
            // Buckling curve and alfa_y_buck and alfa_z_buck definition (p.58)



            // Table 6.2
            // rolled sections
            if (cstype == 1)
            {
                if (csprofshape == 1 || csprofshape == 8)
                {
                    if (h / b > 1.2)
                    {
                        if (tf <= 40)
                        {
                            if (steel_grade_num <= 4)
                            {
                                buck_curve_y = "a";
                                buck_curve_z = "b";
                            }
                            else
                            {
                                buck_curve_y = "a0";
                                buck_curve_z = "a0";
                            }
                        }
                        else
                        {
                            if (steel_grade_num <= 4)
                            {
                                buck_curve_y = "b";
                                buck_curve_z = "c";
                            }
                            else
                            {
                                buck_curve_y = "a";
                                buck_curve_z = "a";
                            }
                        }
                    }

                    else
                    {

                        {
                            if (tf <= 100)
                            {
                                if (steel_grade_num <= 4)
                                {
                                    buck_curve_y = "b";
                                    buck_curve_z = "c";
                                }
                                else
                                {
                                    buck_curve_y = "a";
                                    buck_curve_z = "a";
                                }
                            }
                            else
                            {
                                if (steel_grade_num <= 4)
                                {
                                    buck_curve_y = "d";
                                    buck_curve_z = "d";
                                }
                                else
                                {
                                    buck_curve_y = "c";
                                    buck_curve_z = "c";
                                }
                            }
                        }
                    }
                }

                else if (csprofshape == 5 || csprofshape == 6 || csprofshape == 7)
                {
                    if (csprof_production == 1)
                    {
                        if (steel_grade_num <= 4)
                        {
                            buck_curve_y = "a";
                            buck_curve_z = "a";
                        }
                        else
                        {
                            buck_curve_y = "a0";
                            buck_curve_z = "a0";
                        }
                    }
                    else if (csprof_production == 2 || csprof_production == 3)
                    {
                        if (steel_grade_num <= 4)
                        {
                            buck_curve_y = "c";
                            buck_curve_z = "c";
                        }
                        else
                        {
                            buck_curve_y = "c";
                            buck_curve_z = "c";
                        }
                    }
                }


                else if (csprofshape == 2 || csprofshape == 9 || cssolidshape == 101 || cssolidshape == 102 || cssolidshape == 103)
                {
                    if (steel_grade_num <= 4)
                    {
                        buck_curve_y = "c";
                        buck_curve_z = "c";
                    }
                    else
                    {
                        buck_curve_y = "c";
                        buck_curve_z = "c";
                    }
                }
                else if (csprofshape == 3 || csprofshape == 4)
                {
                    buck_curve_y = "b";
                    buck_curve_z = "b";
                }
            }

                // welded sections 
            else if (cstype == 2)
            {
                if (csweldedshape != 204)
                {
                    if (tf <= 40)
                    {
                        if (steel_grade_num <= 4)
                        {
                            buck_curve_y = "b";
                            buck_curve_z = "c";
                        }
                        else
                        {
                            buck_curve_y = "b";
                            buck_curve_z = "c";
                        }
                    }
                    else
                    {
                        if (steel_grade_num <= 4)
                        {
                            buck_curve_y = "c";
                            buck_curve_z = "d";
                        }
                        else
                        {
                            buck_curve_y = "c";
                            buck_curve_z = "d";
                        }
                    }
                }
                if (csweldedshape == 204)
                {
                    // weld thickness
                    double weld_aw = 5;

                    if (weld_aw <= 0.5 * tf || b / tf >= 30 || h / tw >= 30)
                    {
                        buck_curve_y = "b";
                        buck_curve_z = "b";
                    }
                    else
                    {
                        buck_curve_y = "c";
                        buck_curve_z = "c";

                    }
                }
            }


            // Table 6.1 - see NAD

            if (buck_curve_y == "a0") alfa_y_buck = 0.13;
            else if (buck_curve_y == "a") alfa_y_buck = 0.21;
            else if (buck_curve_y == "b") alfa_y_buck = 0.34;
            else if (buck_curve_y == "c") alfa_y_buck = 0.49;
            else if (buck_curve_y == "d") alfa_y_buck = 0.76;

            if (buck_curve_z == "a0") alfa_z_buck = 0.13;
            else if (buck_curve_z == "a") alfa_z_buck = 0.21;
            else if (buck_curve_z == "b") alfa_z_buck = 0.34;
            else if (buck_curve_z == "c") alfa_z_buck = 0.49;
            else if (buck_curve_z == "d") alfa_z_buck = 0.76;



            #endregion

            #region Section 6.3.1 Uniform members in compression (p. 56)

            // Section 6.3.1 Uniform members in compression (p. 56)



            //
            _Ny_cr = (Math.PI * Math.PI * _Iy * _E) / Math.Pow(_L_y_buck, 2);
            _Nz_cr = (Math.PI * Math.PI * _Iz * _E) / Math.Pow(_L_z_buck, 2);

            //elastic torsional buckling force (vzper skrutenim)
            _N_cr_T = (1 / Math.Pow(i0_r, 2)) * (_G * _IT + ((Math.Pow(Math.PI, 2)) * _E * _Iw) / Math.Pow(_L_teor, 2)); // lt = ???           

            // elastic torsional-flexural buckling force (priestorovy vzper)
            // kubicka rovnica viz Access Steel  SN001a-CZ-EU (4) - missing formula
            // CUBIC EQUATION CARDANOs FORMULA (existuji i reseni v oboru komplexnich cisel)
            #region Cardan formula
            double a_cardano;
            double b_cardano;
            double c_cardano;
            double d_cardano;

            double y_cardano;
            double p_cardano;
            double q_cardano;

            a_cardano = (Math.Pow(iy_r, 2) + Math.Pow(iz_r, 2)) / Math.Pow(i0_r, 2);
            b_cardano = ((1 / Math.Pow(i0_r, 2)) * (_Ny_cr * z0 * z0 + _Nz_cr * y0 * y0)) - (_Ny_cr + _Nz_cr + _N_cr_T);
            c_cardano = _Ny_cr * _Nz_cr + _Nz_cr * _N_cr_T + _Ny_cr * _N_cr_T;
            d_cardano = -(_Ny_cr * _Nz_cr * _N_cr_T);

            p_cardano = -Math.Pow(a_cardano, 2) / 3 + b_cardano;
            q_cardano = ((2 * Math.Pow(a_cardano, 3)) / 27) - ((a_cardano * b_cardano) / 3) + c_cardano;


            double y1_cardano;
            double y2_cardano; // complex number
            double y3_cardano; // complex number

            y1_cardano = Math.Pow((-q_cardano / 2 + Math.Sqrt(((Math.Pow(q_cardano, 2)) / 4) + ((Math.Pow(q_cardano, 3)) / 27))), 1 / 3) + Math.Pow((-q_cardano / 2 + Math.Sqrt(((Math.Pow(q_cardano, 2)) / 4) + ((Math.Pow(q_cardano, 3)) / 27))), 1 / 3);

            double x1_cardano;
            double x2_cardano; // complex number
            double x3_cardano; // complex number

            x1_cardano = y1_cardano - (b_cardano / (3 * a_cardano));



            #endregion

            double _N_cr_TF_yy;
            double _N_cr_TF_zz;


            // Page 3 (5) in Access Steel SN001a-CZ-EU
            // Cross-section symetrical  y-y

            if (cstype == 1)
            {
                if (csprofshape == 1 || csprofshape == 5 || csprofshape == 6 || csprofshape == 7 || csprofshape == 8 || csprofshape == 10)
                {

                    _N_cr_TF_yy = (Math.Pow(i0_r, 2) / (2 * (Math.Pow(iy_r, 2) + Math.Pow(iz_r, 2)))) * (_Ny_cr + _N_cr_T - Math.Sqrt(((Math.Pow((_Ny_cr + _N_cr_T), 2) - 4 * _Ny_cr * _N_cr_T * ((Math.Pow(iy_r, 2) + Math.Pow(iz_r, 2)) / Math.Pow(i0_r, 2))))));
                    _N_cr_TF = _N_cr_TF_yy;
                }
            }

            // Cross-section symetrical  z-z

            if (cstype == 1)
            {
                if (csprofshape == 2)
                {
                    _N_cr_TF_zz = (Math.Pow(i0_r, 2) / (2 * (Math.Pow(iy_r, 2) + Math.Pow(iz_r, 2)))) * (_Nz_cr + _N_cr_T - Math.Sqrt(((Math.Pow((_Nz_cr + _N_cr_T), 2) - 4 * _Nz_cr * _N_cr_T * ((Math.Pow(iy_r, 2) + Math.Pow(iz_r, 2)) / Math.Pow(i0_r, 2))))));
                    _N_cr_TF = _N_cr_TF_zz;
                }
            }



            // 6.3.1.2
            // Slenderness taken from the elastic critical force

            if (classall <= 3) lambda_y_buck_rel = Math.Sqrt((_A * fy) / _Ny_cr);
            else lambda_y_buck_rel = Math.Sqrt((_Aeff * fy) / _Ny_cr);

            if (classall <= 3) lambda_z_buck_rel = Math.Sqrt((_A * fy) / _Nz_cr);
            else lambda_z_buck_rel = Math.Sqrt((_Aeff * fy) / _Nz_cr);


            // 6.3.1.3 Slenderness for flexural buckling (same as in 6.3.1.2)

            if (classall <= 3) lambda_y_buck_rel = _L_y_buck / (iy_r * lambda1_fy); // (6.50)
            else lambda_y_buck_rel = (_L_y_buck * Math.Sqrt(_Aeff / _A)) / (iy_r * lambda1_fy); // (6.51)

            if (classall <= 3) lambda_z_buck_rel = _L_z_buck / (iz_r * lambda1_fy);
            else lambda_z_buck_rel = (_L_z_buck * Math.Sqrt(_Aeff / _A)) / (iz_r * lambda1_fy);




            fi_y_buck = 0.5 * (1 + alfa_y_buck * (lambda_y_buck_rel - 0.2) + Math.Pow(lambda_y_buck_rel, 2));
            fi_z_buck = 0.5 * (1 + alfa_z_buck * (lambda_z_buck_rel - 0.2) + Math.Pow(lambda_z_buck_rel, 2));

            chi_y_buck = 1 / (fi_y_buck + Math.Sqrt(Math.Pow(fi_y_buck, 2) - Math.Pow(lambda_y_buck_rel, 2))); // (6.49)
            chi_z_buck = 1 / (fi_z_buck + Math.Sqrt(Math.Pow(fi_z_buck, 2) - Math.Pow(lambda_z_buck_rel, 2))); // (6.49)



            if (chi_y_buck > 1) chi_y_buck = 1;
            if (chi_z_buck > 1) chi_z_buck = 1;


            if (classall <= 3) _Ny_b_Rd = (chi_y_buck * _A * fy) / gamaM1; // (6.47)
            else _Ny_b_Rd = (chi_y_buck * _Aeff * fy) / gamaM1; // (6.48)

            if (classall <= 3) _Nz_b_Rd = (chi_z_buck * _A * fy) / gamaM1; // (6.47)
            else _Nz_b_Rd = (chi_z_buck * _Aeff * fy) / gamaM1; // (6.48)

            _N_b_Rd = Math.Min(_Ny_b_Rd, _Nz_b_Rd);

            // 6.3.1.4 _N_b_Rd calculation
            // 6.3.1.4 Slenderness for torsional and torsional-flexural buckling

            double lambda_T_buck_rel;

            if (classall <= 3) lambda_T_buck_rel = Math.Sqrt((_A * fy) / (Math.Min(_N_cr_TF, _N_cr_T)));
            else lambda_T_buck_rel = Math.Sqrt((_Aeff * fy) / (Math.Min(_N_cr_TF, _N_cr_T)));

            // Local variables definition

            double fi_T_buck;
            double alfa_T_buck = alfa_z_buck; // check it!!!


            // Calculation

            fi_T_buck = 0.5 * (1 + alfa_T_buck * (lambda_T_buck_rel - 0.2) + Math.Pow(lambda_T_buck_rel, 2));
            chi_T_buck = 1 / (fi_T_buck + Math.Sqrt(Math.Pow(fi_T_buck, 2) - Math.Pow(lambda_T_buck_rel, 2)));
            if (chi_T_buck > 1) chi_T_buck = 1;
            if (classall <= 3) _NT_b_Rd = (chi_T_buck * _A * fy) / gamaM1; // (6.47)
            else _NT_b_Rd = (chi_T_buck * _Aeff * fy) / gamaM1; // (6.48)

            // Minimum buckling resistance (for flexural buckling) and (for torsional and torsional-flexural buckling)

            _N_b_Rd = Math.Min(_N_b_Rd, _NT_b_Rd);



            #endregion

            #region Section 6.3.2 Uniform members in bending (p. 61)

            // Section 6.3.2 Uniform members in bending (p. 61)








            #endregion

            #endregion
        }

        public void EN_1993_1_1_6322()
        {

            #region Section 6.3.2.2 Lateral torsional buckling curves – General case
            // Section 6.3.2.2 Lateral torsional buckling curves – General case

            #region Section 6.3.2.2 alfa LT definition

            // Sections 6.3.2.2 Table 6.3 and Table 6.4 (p. 61 and 62) curves - alfa_y_LT and alfa_z_LT

            if (csprofshape == 1)
            {
                if (h / b <= 2)
                {
                    buck_LT_curve_6322 = "a";
                    alfa_y_LT = 0.21;
                }
                else
                {
                    buck_LT_curve_6322 = "b";
                    alfa_y_LT = 0.34;
                }
            }
            else if (csweldedshape == 201 || csweldedshape == 202 || csweldedshape == 203)
            {
                if (h / b <= 2)
                {
                    buck_LT_curve_6322 = "c";
                    alfa_y_LT = 0.49;
                }
                else
                {
                    buck_LT_curve_6322 = "d";
                    alfa_y_LT = 0.76;
                }
            }

            else
            {
                buck_LT_curve_6322 = "d";
                alfa_y_LT = 0.76;
            }

            #endregion





            // the elastic critical moment for lateral-torsional buckling

            double _My_cr;
            double _Mz_cr = 100000000; // missing formula or DATABASE data

            // National Annex for Czech Republic - NB.3 

            double mu_cr_y;
            double mu_cr_z;
            double kappa_wt_LT;
            double zeta_g_LT;
            double zeta_j_LT;
            double zg_LT;
            double zj_LT;
            double za_LT = 100; // missing formula or DATABASE data
            double zs_LT = 100; // missing formula or DATABASE data
            double psi_f_y_LT = 1; // missing formula or DATABASE data
            double hf = 100;
            double _Ifc_y = 1000000; // missing formula or DATABASE data
            double _Ift_y = 1100000; // missing formula or DATABASE data



            zg_LT = za_LT - zs_LT;

            // page 95:    zj_LT = -0.5/_Iy* .........

            // page 96 NB3.5
            psi_f_y_LT = (_Ifc_y - _Ift_y) / (_Ifc_y + _Ift_y);
            zj_LT = 0.45 * psi_f_y_LT * hf;



            kappa_wt_LT = (Math.PI / (kw_LT * _L_y_LT)) * Math.Sqrt((_E * _Iw) / (_G * _IT));
            zeta_g_LT = ((Math.PI * zg_LT) / (kz_LT * _L_y_LT)) * Math.Sqrt((_E * _Iz) / (_G * _IT));
            zeta_j_LT = ((Math.PI * zj_LT) / (kz_LT * _L_y_LT)) * Math.Sqrt((_E * _Iz) / (_G * _IT));

            mu_cr_y = _C1y / kz_LT * ((Math.Sqrt(1 + Math.Pow(kappa_wt_LT, 2) + Math.Pow((_C2y * zeta_g_LT - _C3y * zeta_j_LT), 2))) - (_C2y * zeta_g_LT - _C3y * zeta_j_LT));  // NB.3.2
            _My_cr = mu_cr_y * ((Math.PI * Math.Sqrt(_E * _Iz * _G * _IT)) / _L_y_LT);



            lambda_y_LT_rel = Math.Sqrt((_Wy * fy) / _My_cr); // (p.61)
            lambda_z_LT_rel = Math.Sqrt((_Wz * fy) / _Mz_cr); // (p.61)


            fi_y_LT = 0.5 * (1 + alfa_y_LT * (lambda_y_LT_rel - 0.2) + Math.Pow(lambda_y_LT_rel, 2));
            fi_z_LT = 0.5 * (1 + alfa_z_LT * (lambda_z_LT_rel - 0.2) + Math.Pow(lambda_z_LT_rel, 2));



            chi_y_LT = 1 / (fi_y_LT + Math.Sqrt(Math.Pow(fi_y_LT, 2) - Math.Pow(lambda_y_LT_rel, 2))); // (6.56)
            chi_z_LT = 1 / (fi_z_LT + Math.Sqrt(Math.Pow(fi_z_LT, 2) - Math.Pow(lambda_z_LT_rel, 2))); // (6.56)

            if (chi_y_LT > 1) chi_y_LT = 1;
            if (chi_z_LT > 1) chi_z_LT = 1;

            if (csshape == 5) // circullar hollow sections
            {
               
                chi_y_LT = 1;
            }

            // Resistance
            _My_b_Rd = (chi_y_LT * _Wy * fy) / gamaM1; // (6.55)
            _Mz_b_Rd = (chi_z_LT * _Wz * fy) / gamaM1; // (6.55)

            #endregion

        }

        public void EN_1993_1_1_6323()
        {

            #region Section 6.3.2.3 Lateral torsional buckling curves for rolled sections or equivalent welded sections
            // Section 6.3.2.3 Lateral torsional buckling curves for rolled sections or equivalent welded sections

            {


                #region Section 6.3.2.3 alfa LT definition

                // Sections 6.3.2.3 Table 6.3 and Table 6.5 (p. 61 and 62) curves - alfa_y_LT and alfa_z_LT

                if (csprofshape == 1)
                {
                    if (h / b <= 2)
                    {
                        buck_LT_curve_6323 = "b";
                        alfa_y_LT = 0.34;
                    }
                    else
                    {
                        buck_LT_curve_6323 = "c";
                        alfa_y_LT = 0.49;
                    }
                }
                else if (csweldedshape == 201 || csweldedshape == 202 || csweldedshape == 203)
                {
                    if (h / b <= 2)
                    {
                        buck_LT_curve_6323 = "c";
                        alfa_y_LT = 0.49;
                    }
                    else
                    {
                        buck_LT_curve_6323 = "d";
                        alfa_y_LT = 0.76;
                    }
                }



                #endregion

                double beta_y_LT = 0.75;
                double beta_z_LT = 0.75;

                double lambda_y_LT0_rel = 0.4;
                double lambda_z_LT0_rel = 0.4;

                fi_y_LT = 0.5 * (1 + alfa_y_LT * (lambda_y_LT_rel - lambda_y_LT0_rel) + beta_y_LT * Math.Pow(lambda_y_LT_rel, 2));
                fi_z_LT = 0.5 * (1 + alfa_z_LT * (lambda_z_LT_rel - lambda_z_LT0_rel) + beta_z_LT * Math.Pow(lambda_z_LT_rel, 2));

                chi_y_LT = 1 / (fi_y_LT + Math.Sqrt(Math.Pow(fi_y_LT, 2) - beta_y_LT * Math.Pow(lambda_y_LT_rel, 2))); // (6.56)
                chi_z_LT = 1 / (fi_z_LT + Math.Sqrt(Math.Pow(fi_z_LT, 2) - beta_z_LT * Math.Pow(lambda_z_LT_rel, 2))); // (6.56)

                if (chi_y_LT > 1) chi_y_LT = 1;
                if (chi_z_LT > 1) chi_z_LT = 1;

                if (chi_y_LT > 1 / Math.Pow(lambda_y_LT_rel, 2)) chi_y_LT = 1 / Math.Pow(lambda_y_LT_rel, 2);
                if (chi_z_LT > 1 / Math.Pow(lambda_z_LT_rel, 2)) chi_z_LT = 1 / Math.Pow(lambda_z_LT_rel, 2);

                // missing moment diagrams and formulas Table 6.6 (p.63)
                switch (Kc_diagram)
                {
                    case 0:
                        kc_y_LT = 1 / (1.33 - (0.33 * psi_My_1));
                        break;
                    case 1:
                        kc_y_LT = 0.94;
                        break;
                    case 2:
                        kc_y_LT = 0.90;
                        break;
                    case 3:
                        kc_y_LT = 0.91;
                        break;
                    case 4:
                        kc_y_LT = 0.86;
                        break;
                    case 5:
                        kc_y_LT = 0.77;
                        break;
                    case 6:
                        kc_y_LT = 0.82;
                        break;
                }




                double f_y_LT = 1 - 0.5 * (1 - kc_y_LT) * (1 - 2 * Math.Pow((lambda_y_LT_rel - 0.8), 2));
                double f_z_LT = 1 - 0.5 * (1 - kc_z_LT) * (1 - 2 * Math.Pow((lambda_z_LT_rel - 0.8), 2));

                if (f_y_LT > 1) f_y_LT = 1;
                if (f_z_LT > 1) f_z_LT = 1;

                double chi_y_LT_mod = chi_y_LT / f_y_LT;
                double chi_z_LT_mod = chi_z_LT / f_z_LT;

                if (chi_y_LT_mod > 1) chi_y_LT_mod = 1;
                if (chi_z_LT_mod > 1) chi_z_LT_mod = 1;

                // chi_i_LT_mod conversion to default variable chi_i_LT which is used in formula (6.55)

                chi_y_LT = chi_y_LT_mod;
                chi_z_LT = chi_z_LT_mod;

                // Resistance
                _My_b_Rd = (chi_y_LT * _Wy * fy) / gamaM1; // (6.55)
                _Mz_b_Rd = (chi_z_LT * _Wz * fy) / gamaM1; // (6.55)

            }
            #endregion
        }

        public void EN_1993_1_1_6324()
        {
            #region Section 6.3.2.4 alfa LT definition

            // Sections 6.3.2.4 p. 64 curves - alfa_y_LT and alfa_z_LT

            if ((cstype == 2) && (h / tf <= 44 * epsilon_fy))
            {
                buck_LT_curve_6324 = "d";
                alfa_y_LT = 0.76;
            }

            else
            {
                buck_LT_curve_6324 = "c";
                alfa_y_LT = 0.49;
            }

            #endregion

            // missing moment diagrams and formulas Table 6.6 (p.63)
            switch (Kc_diagram)
            {
                case 0:
                    kc_y_LT = 1 / (1.33 - (0.33 * psi_My_1));
                    break;
                case 1:
                    kc_y_LT = 0.94;
                    break;
                case 2:
                    kc_y_LT = 0.90;
                    break;
                case 3:
                    kc_y_LT = 0.91;
                    break;
                case 4:
                    kc_y_LT = 0.86;
                    break;
                case 5:
                    kc_y_LT = 0.77;
                    break;
                case 6:
                    kc_y_LT = 0.82;
                    break;
            }




            // Section 6.3.2.4 (p.63)


            double _L_y_c = _L_y_LT;

            // missing formula for i_fz see Note 1B for class 4 (p. 64)
            double i_fz = 99.99;

            // missing formula
            double lambda_y_c0_rel = 100; // missing formula or DATABASE data

            // missing formula see (6.60) and NAD NA 2.19
            double kfl_y = 1.1;

            double fi_y_LT_fc1;
            double chi_y_LT_fc1;
            double lambda_y_f_rel = (kc_y_LT * _L_y_c) / (i_fz * lambda1_fy);


            fi_y_LT_fc1 = 0.5 * (1 + alfa_y_LT * (lambda_y_f_rel - 0.2) + Math.Pow(lambda_y_f_rel, 2));
            chi_y_LT_fc1 = 1 / (fi_y_LT_fc1 + Math.Sqrt(Math.Pow(fi_y_LT_fc1, 2) - Math.Pow(lambda_y_f_rel, 2))); // (6.56)
            if (chi_y_LT_fc1 > 1) chi_y_LT_fc1 = 1;

            // Formula (6.59) see p. 63
            if (lambda_y_f_rel <= lambda_y_c0_rel * (_My_c_Rd / _My_Ed))
            {
                chi_y_LT = 1;
                chi_y_LT_fc1 = 1;
                _My_b_Rd = _My_c_Rd;
            }
            else
{
                if (csshape == 5) // circullar hollow sections
{
                    kfl_y =1;
                    chi_y_LT_fc1 = 1;
}

                _My_b_Rd = kfl_y * chi_y_LT_fc1 * _My_c_Rd;
}

            if (_My_b_Rd > _My_c_Rd) _My_b_Rd = _My_c_Rd;





            // Missing formulas for z-z ???
            _Mz_b_Rd = _Mz_c_Rd;
        }

        public void EN_1993_1_1_633()
        {
            #region Section 6.3.3 Uniform members in bending and axial compression
            // 6.3.3 Uniform members in bending and axial compression
            // Table 6.7 


            if (classall <= 3) _N_Rk = fy * _A;
            else _N_Rk = fy * _Aeff;
            // NbRd!!!!
            if (chi_y_buck * _N_Rk < _Ny_b_Rd)
                _N_Rk = _Ny_b_Rd / chi_y_buck;
            if (chi_z_buck * _N_Rk < _Nz_b_Rd)
                _N_Rk = _Nz_b_Rd / chi_z_buck;

            if (chi_T_buck * _N_Rk < _NT_b_Rd)
                _N_Rk = _NT_b_Rd / chi_T_buck;




            // Bending moments definition
            _My_Rk = fy * _Wy;
            _Mz_Rk = fy * _Wz;
            // MbRd!!!!!
            if (chi_y_LT * _My_Rk < _My_b_Rd)
                _My_Rk = _My_b_Rd / chi_y_LT;


            delta_My_Ed = e_Ny * _NEd_p;
            delta_Mz_Ed = e_Nz * _NEd_p;


            #endregion
        }

        #region Annex

        public void AnnexA_mDiag1()
        {
            // Table A.2: Equivalent uniform moment factors Cmi,0 (p.78)
            // Moment diagram 1

            // otestovat podmienky
            if (Math.Abs(_My_1a) > Math.Abs(_My_1b)) psi_My_1 = _My_1b / _My_1a;
            else psi_My_1 = _My_1a / _My_1b;

            if (Math.Abs(_Mz_1a) > Math.Abs(_Mz_1b)) psi_Mz_1 = _Mz_1b / _Mz_1a;
            else psi_Mz_1 = _Mz_1a / _Mz_1b;



            _Cmy_0 = 0.79 + 0.21 * psi_My_1 + 0.36 * (psi_My_1 - 0.33) * (Math.Abs(_NEd_p) / _Ny_cr);
            _Cmz_0 = 0.79 + 0.21 * psi_Mz_1 + 0.36 * (psi_Mz_1 - 0.33) * (Math.Abs(_NEd_p) / _Nz_cr);
        }
        public void AnnexA_mDiag2()
        {
            // Moment diagram 2

            _Cmy_0 = 1 + (((Math.PI * Math.PI * _E * _Iy * Math.Abs(_deflection_y_x)) / (_L_y_LT * Math.Abs(_My_Ed_x))) - 1) * (Math.Abs(_NEd_p) / _Ny_cr);
            _Cmz_0 = 1 + (((Math.PI * Math.PI * _E * _Iz * Math.Abs(_deflection_z_x)) / (_L_z_LT * Math.Abs(_Mz_Ed_x))) - 1) * (Math.Abs(_NEd_p) / _Nz_cr);

        }
        public void AnnexA_mDiag3()
        {
            // Moment diagram 3

            _Cmy_0 = 1 - 0.18 * (Math.Abs(_NEd_p) / _Ny_cr);
            _Cmz_0 = 1 - 0.18 * (Math.Abs(_NEd_p) / _Nz_cr);
        }
        public void AnnexA_mDiag4()
        {
            // Moment diagram 4

            _Cmy_0 = 1 + 0.03 * (Math.Abs(_NEd_p) / _Ny_cr);
            _Cmz_0 = 1 + 0.03 * (Math.Abs(_NEd_p) / _Nz_cr);
        }

        public void AnnexA()
        {


            // Annex A [informative] – Method 1: Interaction factors kij for interaction formula in 6.3.3(4)





            mu_y = (1 - Math.Abs(_NEd_p) / _Ny_cr) / (1 - chi_y_buck * Math.Abs(_NEd) / _Ny_cr);
            mu_z = (1 - Math.Abs(_NEd_p) / _Nz_cr) / (1 - chi_z_buck * Math.Abs(_NEd) / _Nz_cr);

            double wy = _Wy_pl / _Wy_el;
            double wz = _Wz_pl / _Wz_el;

            if (wy > 1.5) wy = 1.5;
            if (wz > 1.5) wz = 1.5;

            double npl = Math.Abs(_NEd_p) / (_N_Rk / gamaM1);
            aLT = 1 - _IT / _Iy;
            if (aLT < 0) aLT = 0;

            double lambda_max_buck_rel;
            if (lambda_y_buck_rel < lambda_z_buck_rel) lambda_max_buck_rel = lambda_z_buck_rel;
            else lambda_max_buck_rel = lambda_y_buck_rel;

            double lambda_0_buck_rel = 0.79 + 0.21 * 1 + 0.36 * (1 - 0.33) * (Math.Abs(_NEd_p) / _Ny_cr);




            // Table A.1


            if (classall <= 3) epsilon_y = (Math.Abs(_My_Ed) / Math.Abs(_NEd_p)) * (_A / _Wy_el);
            else epsilon_y = (Math.Abs(_My_Ed) / Math.Abs(_NEd_p)) * (_Aeff / _Wy_eff);




            // Auxiliary calculations
            double epsilon_y_My_N = (Math.Abs(_My_Ed)) / (Math.Abs(_NEd_p));
            double epsilon_y_A_Wy_el = _A / _Wy_el;


            // MessageBox.Show(" Hodnoty: epsilon_y_My_N =" + epsilon_y_My_N.ToString() + " epsilon_y_A_Wy_el = " + epsilon_y_A_Wy_el.ToString() + " NEd.p = " + _NEd_p.ToString());







            if (lambda_0_buck_rel <= 0.2 * Math.Sqrt(_C1y) * Math.Pow((1 - (Math.Abs(_NEd_p) / _Nz_cr)) * (1 - (Math.Abs(_NEd_p) / _N_cr_TF)), 0.25))
            {

                _Cmy = _Cmy_0;
                _Cmz = _Cmz_0;
                _CmLT = 1;
            }
            else
            {
                _Cmy = _Cmy_0 + (1 - _Cmy_0) * ((Math.Sqrt(epsilon_y) * aLT) / (1 + (Math.Sqrt(epsilon_y) * aLT)));
                _Cmz = _Cmz_0;
                _CmLT = Math.Pow(_Cmy, 2) * (aLT / (Math.Sqrt((1 - (Math.Abs(_NEd_p) / _Nz_cr)) * (1 - (Math.Abs(_NEd_p) / _N_cr_TF)))));
                if (_CmLT < 1) _CmLT = 1;
            }
            // Auxiliary terms for plastic cross-sectional properties class 1, class 2

            bLT = 0.5 * aLT * Math.Pow(lambda_0_buck_rel, 2) * (Math.Abs(_My_Ed) / (chi_y_LT * _My_pl_Rd)) * (Math.Abs(_Mz_Ed) / _Mz_pl_Rd);
            cLT = 10 * aLT * (Math.Pow(lambda_0_buck_rel, 2) / (5 + Math.Pow(lambda_z_buck_rel, 4))) * (Math.Abs(_My_Ed) / (_Cmy * chi_y_LT * _My_pl_Rd));
            dLT = 2 * aLT * (Math.Pow(lambda_0_buck_rel, 2) / (0.1 + Math.Pow(lambda_z_buck_rel, 4))) * (Math.Abs(_My_Ed) / (_Cmy * chi_y_LT * _My_pl_Rd)) * (Math.Abs(_Mz_Ed) / (_Cmz * _Mz_pl_Rd));
            eLT = 1.7 * aLT * (Math.Pow(lambda_0_buck_rel, 2) / (0.1 + Math.Pow(lambda_z_buck_rel, 4))) * (Math.Abs(_My_Ed) / (_Cmy * chi_y_LT * _My_pl_Rd));



            _Cyy = 1 + (wy - 1) * ((2 - 1.6 / wy * Math.Pow(_Cmy, 2) * lambda_max_buck_rel - 1.6 / wy * Math.Pow(_Cmy, 2) * Math.Pow(lambda_max_buck_rel, 2)) * npl - bLT);
            if (_Cyy < _Wy_el / _Wy_pl) _Cyy = _Wy_el / _Wy_pl;

            _Cyz = 1 + (wz - 1) * ((2 - (14 / Math.Pow(wz, 5) * Math.Pow(_Cmz, 2) * Math.Pow(lambda_max_buck_rel, 2))) * npl - cLT);
            if (_Cyz < 0.6 * Math.Sqrt(wz / wy) * _Wz_el / _Wz_pl) _Cyz = 0.6 * Math.Sqrt(wz / wy) * _Wz_el / _Wz_pl;

            _Czy = 1 + (wy - 1) * ((2 - (14 / Math.Pow(wy, 5) * Math.Pow(_Cmy, 2) * Math.Pow(lambda_max_buck_rel, 2))) * npl - dLT);
            if (_Czy < 0.6 * Math.Sqrt(wy / wz) * _Wy_el / _Wy_pl) _Czy = 0.6 * Math.Sqrt(wy / wz) * _Wy_el / _Wy_pl;

            _Czz = 1 + (wz - 1) * ((2 - 1.6 / wz * Math.Pow(_Cmz, 2) * lambda_max_buck_rel - 1.6 / wz * Math.Pow(_Cmz, 2) * Math.Pow(lambda_max_buck_rel, 2)) * npl - eLT);
            if (_Czz < _Wz_el / _Wz_pl) _Czz = _Wz_el / _Wz_pl;

            // plastic cross-sectional properties class 1, class 2

            if (classall == 1 || classall == 2)
            {
                kyy = _Cmy * _CmLT * (mu_y / (1 - (Math.Abs(_NEd_p) / _Ny_cr))) * 1 / _Cyy;
                kyz = _Cmz * (mu_y / (1 - (Math.Abs(_NEd_p) / _Nz_cr))) * 1 / _Cyz * 0.6 * Math.Sqrt(wz / wy);
                kzy = _Cmy * _CmLT * (mu_z / (1 - (Math.Abs(_NEd_p) / _Ny_cr))) * 1 / _Czy * 0.6 * Math.Sqrt(wy / wz);
                kzz = _Cmz * (mu_z / (1 - (Math.Abs(_NEd_p) / _Nz_cr))) * 1 / _Czz;
            }
            // elastic cross-sectional properties class 3, class 4

            else // (classall == 3 || classall == 4)
            {
                kyy = _Cmy * _CmLT * (mu_y / (1 - (Math.Abs(_NEd_p) / _Ny_cr)));
                kyz = _Cmz * (mu_y / (1 - (Math.Abs(_NEd_p) / _Nz_cr)));
                kzy = _Cmy * _CmLT * (mu_z / (1 - (Math.Abs(_NEd_p) / _Ny_cr)));
                kzz = _Cmz * (mu_z / (1 - (Math.Abs(_NEd_p) / _Nz_cr)));
            }

        }

        public void AnnexB_mDiag1()
        {
            // Moment diagram 1 (Table B.3, p. 79)
            #region mDiag1

            // otestovat podmienky
            if (Math.Abs(_My_1a) > Math.Abs(_My_1b)) psi_My_1 = _My_1b / _My_1a;
            else psi_My_1 = _My_1a / _My_1b;

            if (Math.Abs(_Mz_1a) > Math.Abs(_Mz_1b)) psi_Mz_1 = _Mz_1b / _Mz_1a;
            else psi_Mz_1 = _Mz_1a / _Mz_1b;

            _Cmy = 0.6 + 0.4 * psi_My_1;
            _Cmz = 0.6 + 0.4 * psi_Mz_1;
            _CmLT = 0.6 + 0.4 * psi_My_1;

            #endregion

        }
        public void AnnexB_mDiag2()
        {
            // Moment diagram 2 (Table B.3, p. 79)

            double alpha_y_s;
            double alpha_z_s;
            double _My_h;
            double _Mz_h;

            if (Math.Abs(_My_a) > Math.Abs(_My_b)) _My_h = _My_a;
            else _My_h = _My_b;
            if (Math.Abs(_My_s) <= Math.Abs(_My_h)) alpha_y_s = _My_s / _My_h;
            else
            {
                if (_My_s / _My_h < 0) alpha_y_s = -1;
                else alpha_y_s = 1;
            }
            if (Math.Abs(_Mz_a) > Math.Abs(_Mz_b)) _Mz_h = _Mz_a;
            else _Mz_h = _Mz_b;
            if (Math.Abs(_Mz_s) <= Math.Abs(_Mz_h)) alpha_z_s = _Mz_s / _Mz_h;
            else
            {
                if (_Mz_s / _Mz_h < 0) alpha_z_s = -1;
                else alpha_z_s = 1;
            }


            #region mDiag2_Cmy

            if (loading_type_y == 1)
            {
                if (alpha_y_s > 0)
                {
                    _Cmy = 0.2 + 0.8 * alpha_y_s;
                    if (_Cmy < 0.4) _Cmy = 0.4;
                }
                else if ((alpha_y_s < 0) && (psi_My_1 >= 0))
                {
                    _Cmy = 0.1 - 0.8 * alpha_y_s;
                    if (_Cmy < 0.4) _Cmy = 0.4;
                }
                else if ((alpha_y_s < 0) && (psi_My_1 < 0))
                {
                    _Cmy = 0.1 * (0.1 - psi_My_1) - 0.8 * alpha_y_s;
                    if (_Cmy < 0.4) _Cmy = 0.4;
                }
            }
            else if (loading_type_y == 2)
            {
                if (alpha_y_s > 0)
                {
                    _Cmy = 0.2 + 0.8 * alpha_y_s;
                    if (_Cmy < 0.4) _Cmy = 0.4;
                }
                else if ((alpha_y_s < 0) && (psi_My_1 >= 0))
                {
                    _Cmy = -0.8 * alpha_y_s;
                    if (_Cmy < 0.4) _Cmy = 0.4;
                }
                else if ((alpha_y_s < 0) && (psi_My_1 < 0))
                {
                    _Cmy = 0.2 * (-psi_My_1) - 0.8 * alpha_y_s;
                    if (_Cmy < 0.4) _Cmy = 0.4;
                }
            }
            #endregion

            #region mDiag2_Cmz

            if (loading_type_z == 1)
            {
                if (alpha_z_s > 0)
                {
                    _Cmz = 0.2 + 0.8 * alpha_z_s;
                    if (_Cmz < 0.4) _Cmz = 0.4;
                }
                else if ((alpha_z_s < 0) && (psi_Mz_1 >= 0))
                {
                    _Cmz = 0.1 - 0.8 * alpha_z_s;
                    if (_Cmz < 0.4) _Cmz = 0.4;
                }
                else if ((alpha_z_s < 0) && (psi_Mz_1 < 0))
                {
                    _Cmz = 0.1 * (0.1 - psi_Mz_1) - 0.8 * alpha_z_s;
                    if (_Cmz < 0.4) _Cmz = 0.4;
                }
            }
            else if (loading_type_z == 2)
            {
                if (alpha_z_s > 0)
                {
                    _Cmz = 0.2 + 0.8 * alpha_z_s;
                    if (_Cmz < 0.4) _Cmz = 0.4;
                }
                else if ((alpha_z_s < 0) && (psi_Mz_1 >= 0))
                {
                    _Cmz = -0.8 * alpha_z_s;
                    if (_Cmz < 0.4) _Cmz = 0.4;
                }
                else if ((alpha_z_s < 0) && (psi_Mz_1 < 0))
                {
                    _Cmz = 0.2 * (-psi_Mz_1) - 0.8 * alpha_z_s;
                    if (_Cmz < 0.4) _Cmz = 0.4;
                }
            }
            #endregion

            #region mDiag2_CmLT

            if (loading_type_y == 1)
            {
                if (alpha_y_s > 0)
                {
                    _CmLT = 0.2 + 0.8 * alpha_y_s;
                    if (_CmLT < 0.4) _Cmy = 0.4;
                }
                else if ((alpha_y_s < 0) && (psi_My_1 >= 0))
                {
                    _CmLT = 0.1 - 0.8 * alpha_y_s;
                    if (_CmLT < 0.4) _Cmy = 0.4;
                }
                else if ((alpha_y_s < 0) && (psi_My_1 < 0))
                {
                    _CmLT = 0.1 * (0.1 - psi_My_1) - 0.8 * alpha_y_s;
                    if (_CmLT < 0.4) _Cmy = 0.4;
                }
            }
            else if (loading_type_y == 2)
            {
                if (alpha_y_s > 0)
                {
                    _CmLT = 0.2 + 0.8 * alpha_y_s;
                    if (_CmLT < 0.4) _Cmy = 0.4;
                }
                else if ((alpha_y_s < 0) && (psi_My_1 >= 0))
                {
                    _CmLT = -0.8 * alpha_y_s;
                    if (_CmLT < 0.4) _Cmy = 0.4;
                }
                else if ((alpha_y_s < 0) && (psi_My_1 < 0))
                {
                    _CmLT = 0.2 * (-psi_My_1) - 0.8 * alpha_y_s;
                    if (_CmLT < 0.4) _Cmy = 0.4;
                }
            }
            #endregion


        }
        public void AnnexB_mDiag3()
        {
            // Moment diagram 3 (Table B.3, p. 79)

            double alpha_y_h;
            double alpha_z_h;
            double _My_h;
            double _Mz_h;

            if (Math.Abs(_My_a) > Math.Abs(_My_b)) _My_h = _My_a;
            else _My_h = _My_b;
            if (Math.Abs(_My_h) <= Math.Abs(_My_s)) alpha_y_h = _My_h / _My_s;
            else
            {
                if (_My_h / _My_s < 0) alpha_y_h = -1;
                else alpha_y_h = 1;
            }




            if (Math.Abs(_Mz_a) > Math.Abs(_Mz_b)) _Mz_h = _Mz_a;
            else _Mz_h = _Mz_b;
            if (Math.Abs(_Mz_h) <= Math.Abs(_Mz_s)) alpha_z_h = _Mz_h / _Mz_s;
            else
            {
                if (_Mz_h / _Mz_s < 0) alpha_z_h = -1;
                else alpha_z_h = 1;
            }



            #region mDiag3_Cmy
            if (loading_type_y == 1)
            {
                if (alpha_y_h > 0)
                {
                    _Cmy = 0.95 + 0.05 * alpha_y_h;
                }
                else if ((alpha_y_h < 0) && (psi_My_1 >= 0))
                {
                    _Cmy = 0.95 - 0.05 * alpha_y_h;
                }
                else if ((alpha_y_h < 0) && (psi_My_1 < 0))
                {
                    _Cmy = 0.95 + 0.05 * alpha_y_h * (1 + 2 * psi_My_1);
                }
            }
            else if (loading_type_y == 2)
            {
                if (alpha_y_h > 0)
                {
                    _Cmy = 0.90 + 0.1 * alpha_y_h;
                }
                else if ((alpha_y_h < 0) && (psi_My_1 >= 0))
                {
                    _Cmy = 0.90 - 0.1 * alpha_y_h;
                }
                else if ((alpha_y_h < 0) && (psi_My_1 < 0))
                {
                    _Cmy = 0.90 + 0.1 * alpha_y_h * (1 + 2 * psi_My_1);
                }
            }
            #endregion

            #region mDiag3_Cmz

            if (loading_type_z == 1)
            {
                if (alpha_z_h > 0)
                {
                    _Cmz = 0.95 + 0.05 * alpha_z_h;
                }
                else if ((alpha_z_h < 0) && (psi_Mz_1 >= 0))
                {
                    _Cmz = 0.95 - 0.05 * alpha_z_h;
                }
                else if ((alpha_z_h < 0) && (psi_Mz_1 < 0))
                {
                    _Cmz = 0.95 + 0.05 * alpha_z_h * (1 + 2 * psi_Mz_1);
                }
            }
            else if (loading_type_z == 2)
            {
                if (alpha_z_h > 0)
                {
                    _Cmz = 0.90 + 0.1 * alpha_z_h;
                }
                else if ((alpha_z_h < 0) && (psi_Mz_1 >= 0))
                {
                    _Cmz = 0.90 - 0.1 * alpha_z_h;
                }
                else if ((alpha_z_h < 0) && (psi_Mz_1 < 0))
                {
                    _Cmz = 0.90 + 0.1 * alpha_z_h * (1 + 2 * psi_Mz_1);
                }
            }
            #endregion

            #region mDiag3_CmLT
            if (loading_type_y == 1)
            {
                if (alpha_y_h > 0)
                {
                    _CmLT = 0.95 + 0.05 * alpha_y_h;
                }
                else if ((alpha_y_h < 0) && (psi_My_1 >= 0))
                {
                    _CmLT = 0.95 - 0.05 * alpha_y_h;
                }
                else if ((alpha_y_h < 0) && (psi_My_1 < 0))
                {
                    _CmLT = 0.95 + 0.05 * alpha_y_h * (1 + 2 * psi_My_1);
                }
            }
            else if (loading_type_y == 2)
            {
                if (alpha_y_h > 0)
                {
                    _CmLT = 0.90 + 0.1 * alpha_y_h;
                }
                else if ((alpha_y_h < 0) && (psi_My_1 >= 0))
                {
                    _CmLT = 0.90 - 0.1 * alpha_y_h;
                }
                else if ((alpha_y_h < 0) && (psi_My_1 < 0))
                {
                    _CmLT = 0.90 + 0.1 * alpha_y_h * (1 + 2 * psi_My_1);
                }
            }
            #endregion


        }

        public void AnnexB()
        {
            // Annex B [informative] – Method 2: Interaction factors kij for interaction formula in 6.3.3(4)
            // Table B.1: Interaction factors kij for members not susceptible to torsional deformations
            // Table B.3

            // Beam loading – 1 - uniform loading; 2 - concentrated load











            // Cross section class = 1 or 2
            if ((csprofshape == 1 || csprofshape == 6 || csprofshape == 7) && (classall == 1 || classall == 2))
            {
                kyy = _Cmy * (1 + (lambda_y_buck_rel - 0.2) * (_NEd_p / (chi_y_buck * _N_Rk / gamaM1)));
                if (kyy > _Cmy * (1 + 0.8 * (_NEd_p / (chi_y_buck * _N_Rk / gamaM1)))) kyy = _Cmy * (1 + 0.8 * (_NEd_p / (chi_y_buck * _N_Rk / gamaM1)));
            }
            if ((csprofshape == 1) && (classall == 1 || classall == 2))
            {
                kzz = _Cmz * (1 + (2 * lambda_z_buck_rel - 0.6) * (_NEd_p / (chi_z_buck * _N_Rk / gamaM1)));
                if (kzz > _Cmz * (1 + 1.4 * (_NEd_p / (chi_z_buck * _N_Rk / gamaM1)))) kzz = _Cmz * (1 + 1.4 * (_NEd_p / (chi_z_buck * _N_Rk / gamaM1)));
            }
            if ((csprofshape == 6 || csprofshape == 7) && (classall == 1 || classall == 2))
            {
                kzz = _Cmz * (1 + (lambda_z_buck_rel - 0.2) * (_NEd_p / (chi_z_buck * _N_Rk / gamaM1)));
                if (kzz > _Cmz * (1 + 0.8 * (_NEd_p / (chi_z_buck * _N_Rk / gamaM1)))) kzz = _Cmz * (1 + 0.8 * (_NEd_p / (chi_z_buck * _N_Rk / gamaM1)));
            }
            if ((csprofshape == 1 || csprofshape == 6 || csprofshape == 7) && (classall == 1 || classall == 2))
            {
                kyz = 0.6 * kzz;
                kzy = 0.6 * kyy;
            }

            // Cross section class = 3 or 4
            if ((csprofshape == 1 || csprofshape == 6 || csprofshape == 7) && (classall == 3 || classall == 4))
            {
                kyy = _Cmy * (1 + 0.6 * lambda_y_buck_rel * (_NEd_p / (chi_y_buck * _N_Rk / gamaM1)));
                if (kyy > _Cmy * (1 + 0.6 * (_NEd_p / (chi_y_buck * _N_Rk / gamaM1)))) kyy = _Cmy * (1 + 0.6 * (_NEd_p / (chi_y_buck * _N_Rk / gamaM1)));

                kzz = _Cmz * (1 + 0.6 * lambda_z_buck_rel * (_NEd_p / (chi_z_buck * _N_Rk / gamaM1)));
                if (kzz > _Cmz * (1 + 0.6 * (_NEd_p / (chi_z_buck * _N_Rk / gamaM1)))) kzz = _Cmz * (1 + 0.6 * (_NEd_p / (chi_z_buck * _N_Rk / gamaM1)));

                kyz = kzz;
                kzy = 0.8 * kyy;
            }

            // Table B.2: Interaction factors kij for members susceptible to torsional deformations
            // Cross section class = 1 or 2
            if ((csprofshape == 1 || csprofshape == 6 || csprofshape == 7) && (classall == 1 || classall == 2))
            {
                kyy = _Cmy * (1 + (lambda_y_buck_rel - 0.2) * (_NEd_p / (chi_y_buck * _N_Rk / gamaM1)));
                if (kyy > _Cmy * (1 + 0.8 * (_NEd_p / (chi_y_buck * _N_Rk / gamaM1)))) kyy = _Cmy * (1 + 0.8 * (_NEd_p / (chi_y_buck * _N_Rk / gamaM1)));
            }
            if ((csprofshape == 1) && (classall == 1 || classall == 2))
            {
                kzz = _Cmz * (1 + (2 * lambda_z_buck_rel - 0.6) * (_NEd_p / (chi_z_buck * _N_Rk / gamaM1)));
                if (kzz > _Cmz * (1 + 1.4 * (_NEd_p / (chi_z_buck * _N_Rk / gamaM1)))) kzz = _Cmz * (1 + 1.4 * (_NEd_p / (chi_z_buck * _N_Rk / gamaM1)));
            }
            if ((csprofshape == 6 || csprofshape == 7) && (classall == 1 || classall == 2))
            {
                kzz = _Cmz * (1 + (lambda_z_buck_rel - 0.2) * (_NEd_p / (chi_z_buck * _N_Rk / gamaM1)));
                if (kzz > _Cmz * (1 + 0.8 * (_NEd_p / (chi_z_buck * _N_Rk / gamaM1)))) kzz = _Cmz * (1 + 0.8 * (_NEd_p / (chi_z_buck * _N_Rk / gamaM1)));
            }
            if ((csprofshape == 1 || csprofshape == 6 || csprofshape == 7) && (classall == 1 || classall == 2))
            {
                kyz = 0.6 * kzz;

                if (lambda_z_buck_rel >= 0.4)
                    kzy = (1 - (0.1 * lambda_z_buck_rel / (_CmLT - 0.25)) * (_NEd_p / (chi_z_buck * _N_Rk / gamaM1)));
                else
                {
                    kzy = 0.6 + lambda_z_buck_rel;
                    if (kzy > 1 - ((0.1 * lambda_z_buck_rel / (_CmLT - 0.25)) * (_NEd_p / (chi_z_buck * _N_Rk / gamaM1))))
                        kzy = 1 - ((0.1 * lambda_z_buck_rel / (_CmLT - 0.25)) * (_NEd_p / (chi_z_buck * _N_Rk / gamaM1)));
                }
            }

            // Cross section class = 3 or 4
            if ((csprofshape == 1 || csprofshape == 6 || csprofshape == 7) && (classall == 3 || classall == 4))
            {
                kyy = _Cmy * (1 + 0.6 * lambda_y_buck_rel * (_NEd_p / (chi_y_buck * _N_Rk / gamaM1)));
                if (kyy > _Cmy * (1 + 0.6 * (_NEd_p / (chi_y_buck * _N_Rk / gamaM1)))) kyy = _Cmy * (1 + 0.6 * (_NEd_p / (chi_y_buck * _N_Rk / gamaM1)));

                kzz = _Cmz * (1 + 0.6 * lambda_z_buck_rel * (_NEd_p / (chi_z_buck * _N_Rk / gamaM1)));
                if (kzz > _Cmz * (1 + 0.6 * (_NEd_p / (chi_z_buck * _N_Rk / gamaM1))))
                    kzz = _Cmz * (1 + 0.6 * (_NEd_p / (chi_z_buck * _N_Rk / gamaM1)));

                kyz = kzz;
                kzy = 1 - ((0.05 * lambda_z_buck_rel / _CmLT - 0.25) * (_NEd_p / (chi_z_buck * _N_Rk / gamaM1)));
                if (kzy < 1 - ((0.05 / _CmLT - 0.25) * (_NEd_p / (chi_z_buck * _N_Rk / gamaM1))))
                    kzy = 1 - ((0.05 / _CmLT - 0.25) * (_NEd_p / (chi_z_buck * _N_Rk / gamaM1)));
            }
            //koniec AnexB

        }

        #endregion

        #region Final Check Ratio
        public void final_Check_Ratio()
        {


            // 6.2.3 Tension 
            ratio_65 = _NEd_t / _Nt_Rd;

            // 6.2.4 Compression 
            ratio_69 = _NEd_p / _Nc_Rd;

            // 6.2.5 Bending moment 
            ratio_612y = _My_Ed / _My_c_Rd;
            ratio_612z = _Mz_Ed / _Mz_c_Rd;

            // 6.2.6 Shear 
            ratio_617y = _Vy_Ed / _Vy_c_Rd;
            ratio_617z = _Vz_Ed / _Vz_c_Rd;

            // 6.2.7 Torsion


            ratio_623 = _Tx_Ed / _T_Rd;
            // Auxiliary calculation
            ratio_623 = Math.Max(ratio_a4, ratio_623);


            // 6.2.8 Bending and shear + torsion ???
            // see 6.2.5 Bending moment (6.12)

            if (classall == 1 || classall == 2)
 {
            Ratio_612y_MV = _My_Ed /_My_V_pl_Rd;
            ratio_612z_MV = _Mz_Ed /_Mz_V_pl_Rd;
            }
            else if (classall == 3)
 {
            Ratio_612y_MV = _My_Ed /_My_V_el_Rd;
            ratio_612z_MV = _Mz_Ed /_Mz_V_el_Rd;
            }
             else if (classall == 4)
 {
            Ratio_612y_MV = _My_Ed /_My_V_el_Rd;// auxiliary
            ratio_612z_MV = _Mz_Ed /_Mz_V_el_Rd;// auxiliary
            }

            
            // 6.2.9 Bending and axial force-tension

            if (classall == 1 || classall == 2)
            {
                ratio_631y = _My_Ed / _My_N_Rd;
                ratio_631z = _Mz_Ed / _Mz_N_Rd;
            }
            else if (classall == 3)

                ratio_642 = sigma_x_Ed / (fy / gamaM0);

            else if (classall == 4)
            {
                ratio_643 = sigma_x_Ed / (fy / gamaM0);

                ratio_644 = (_NEd / (_Aeff * fy / gamaM0) + (_My_Ed + _NEd * e_Ny) / (_Wy_eff_min * fy / gamaM0) + (_Mz_Ed + _NEd * e_Nz) / (_Wz_eff_min * fy / gamaM0)) / 1;

            }
            // Auxiliary calculation !!!

            double ratio_629_max_a = Math.Max(ratio_631y, ratio_631z);
            double ratio_629_max_b = Math.Max(ratio_642, ratio_643);
            double ratio_629_max_c = Math.Max(ratio_644, ratio_629_max_a);
            double ratio_629_max_d = Math.Max(ratio_629_max_b, ratio_629_max_c);

            // 6.3.1 Compression - buckling 
            ratio_646 = _NEd_p / _N_b_Rd;

            // 6.3.2 Bending - lateral buckling 
            ratio_654y = _My_Ed / _My_b_Rd;
            ratio_654z = _Mz_Ed / _Mz_b_Rd;

            // 6.3.3 Uniform members in bending and axial compression


            ratio_661N = _NEd_p / (chi_y_buck * (_N_Rk / gamaM1));
            ratio_661My = kyy * ((_My_Ed + delta_My_Ed) / (chi_y_LT * (_My_Rk / gamaM1)));
            ratio_661Mz = kyz * ((_Mz_Ed + delta_Mz_Ed) / (_Mz_Rk / gamaM1));

            ratio_661 = ratio_661N + ratio_661My + ratio_661Mz; // (6.61)


            ratio_662N = _NEd_p / (chi_z_buck * _N_Rk / gamaM1);
            ratio_662My = kzy * ((_My_Ed + delta_My_Ed) / (chi_y_LT * (_My_Rk / gamaM1)));
            ratio_662Mz = kzz * ((_Mz_Ed + delta_Mz_Ed) / (_Mz_Rk / gamaM1));

            ratio_662 = ratio_662N + ratio_662My + ratio_662Mz; // (6.62)




            // Absolute maximum ratio

            double ratio_maxtot1 = Math.Max(ratio_65, ratio_69);
            double ratio_maxtot2 = Math.Max(ratio_maxtot1, ratio_612y);
            double ratio_maxtot3 = Math.Max(ratio_maxtot2, ratio_612z);
            double ratio_maxtot4 = Math.Max(ratio_maxtot3, ratio_617y);
            double ratio_maxtot5 = Math.Max(ratio_maxtot4, ratio_617z);
            double ratio_maxtot6 = Math.Max(ratio_maxtot5, ratio_623);
            double ratio_maxtot7 = Math.Max(ratio_maxtot6, ratio_629_max_d);
            double ratio_maxtot8 = Math.Max(ratio_maxtot7, ratio_646);
            double ratio_maxtot9 = Math.Max(ratio_maxtot8, ratio_654y);
            double ratio_maxtot10 = Math.Max(ratio_maxtot9, ratio_654z);
            double ratio_maxtot11 = Math.Max(ratio_maxtot10, ratio_661);
            double ratio_maxtot12 = Math.Max(ratio_maxtot11, ratio_662);


            // Ratio of cross section -  Absolute maximum
            ratio_maxtot = ratio_maxtot12;






            #region Results message window
            // Round variables for output window



            int dec_place_num1 = 3;
            int dec_place_num2 = 1;

            // Auxiliary
            psi_My_1 = Math.Round(psi_My_1, dec_place_num1);
            psi_Mz_1 = Math.Round(psi_Mz_1, dec_place_num1);
            Math.Round(_Cmy_0, dec_place_num1);
            Math.Round(_Cmz_0, dec_place_num1);
            Math.Round(epsilon_y, dec_place_num1);
            Math.Round(_Cmy, dec_place_num1);
            Math.Round(_Cmz, dec_place_num1);
            Math.Round(_CmLT, dec_place_num1);
            Math.Round(mu_y, dec_place_num1);
            Math.Round(mu_z, dec_place_num1);
            Math.Round(aLT, dec_place_num1);
            Math.Round(bLT, dec_place_num1);
            Math.Round(cLT, dec_place_num1);
            Math.Round(dLT, dec_place_num1);
            Math.Round(eLT, dec_place_num1);
            Math.Round(_Cyy, dec_place_num1);
            Math.Round(_Cyz, dec_place_num1);
            Math.Round(_Czy, dec_place_num1);
            Math.Round(_Czz, dec_place_num1);
            Math.Round(kyy, dec_place_num1);
            Math.Round(kyz, dec_place_num1);
            Math.Round(kzy, dec_place_num1);
            Math.Round(kzz, dec_place_num1);
            Math.Round(ratio_661N * 100, dec_place_num2);
            Math.Round(ratio_661My * 100, dec_place_num2);
            Math.Round(ratio_661Mz * 100, dec_place_num2);
            Math.Round(ratio_661 * 100, dec_place_num2);
            Math.Round(ratio_662N * 100, dec_place_num2);
            Math.Round(ratio_662 * 100, dec_place_num2);



            // Results Message boxes definition

            if (item3_check == true)
            #region Annex A output and kij factors
            {
                MessageBox.Show((
                    " Auxiliary calculation results: " + "\n"
                    + "\n"
                    + " Annex A - parameters: " + "\n"
                    + "\n"
                    + " ψ My_1 = " + Math.Round(psi_My_1, dec_place_num1).ToString() + "\n"
                    + " ψ Mz_1 = " + Math.Round(psi_Mz_1, dec_place_num1).ToString() + "\n"
                    + " Cmy0 = " + Math.Round(_Cmy_0, dec_place_num1).ToString() + "\n"
                    + " Cmz0 = " + Math.Round(_Cmz_0, dec_place_num1).ToString() + "\n"
                    + " εy = " + Math.Round(epsilon_y, dec_place_num1).ToString() + "\n"
                    + " μy = " + Math.Round(mu_y, dec_place_num1).ToString() + "\n"
                    + " μz = " + Math.Round(mu_z, dec_place_num1).ToString() + "\n"
                    + " aLT = " + Math.Round(aLT, dec_place_num1).ToString() + "\n"
                    + " bLT = " + Math.Round(bLT, dec_place_num1).ToString() + "\n"
                    + " cLT = " + Math.Round(cLT, dec_place_num1).ToString() + "\n"
                    + " dLT = " + Math.Round(dLT, dec_place_num1).ToString() + "\n"
                    + " eLT = " + Math.Round(eLT, dec_place_num1).ToString() + "\n"
                    + " Cyy = " + Math.Round(_Cyy, dec_place_num1).ToString() + "\n"
                    + " Cyz = " + Math.Round(_Cyz, dec_place_num1).ToString() + "\n"
                    + " Czy = " + Math.Round(_Czy, dec_place_num1).ToString() + "\n"
                    + " Czz = " + Math.Round(_Czz, dec_place_num1).ToString() + "\n"




                    + "\n"
                    + " Interaction factors kij according Section 6.3.3: " + "\n"
                    + "\n"
                    + " Cmy = " + Math.Round(_Cmy, dec_place_num1).ToString() + "\n"
                    + " Cmz = " + Math.Round(_Cmz, dec_place_num1).ToString() + "\n"
                    + " CmLT= " + Math.Round(_CmLT, dec_place_num1).ToString() + "\n"
                    + "\n"
                    + " kyy = " + Math.Round(kyy, dec_place_num1).ToString() + "\n"
                    + " kyz = " + Math.Round(kyz, dec_place_num1).ToString() + "\n"
                    + " kzy = " + Math.Round(kzy, dec_place_num1).ToString() + "\n"
                    + " kzz = " + Math.Round(kzz, dec_place_num1).ToString()), " Auxiliary calculation");

            }
            #endregion
            if (item4_check == true)
            #region Resistances
            {
                MessageBox.Show
                        ((
                        " Calculation results - Resistances and Check ratios: " + "\n"
                    + "\n"
                    + " Axial resistance: " + "\n"
                    + "\n"
                    + " Tension " + "\n"
                    + "\n"
                    + " Npl.Rd = " + Math.Round(_Npl_Rd / 1000, dec_place_num2).ToString() + " kN " + "\n"
                    + " Nu.Rd = " + Math.Round(_Nu_Rd / 1000, dec_place_num2).ToString() + " kN " + "\n"
                    + " Nnet.Rd = " + Math.Round(_Nnet_Rd / 1000, dec_place_num2).ToString() + " kN " + "\n"
                    + " Nt.Rd= " + Math.Round(_Nt_Rd / 1000, dec_place_num2).ToString() + " kN " + "\n"
                    + "\n"
                    + " Compression " + "\n"
                    + "\n"
                    + " Ny.cr = " + Math.Round(_Ny_cr / 1000, dec_place_num2).ToString() + " kN " + "\n"
                    + " Nz.cr = " + Math.Round(_Nz_cr / 1000, dec_place_num2).ToString() + " kN " + "\n"
                    + " Nc.Rd = " + Math.Round(_Nc_Rd / 1000, dec_place_num2).ToString() + " kN " + "\n"
                    + " Ncr.TF = " + Math.Round(_N_cr_TF / 1000, dec_place_num2).ToString() + " kN " + "\n"
                    + " Ncr.T = " + Math.Round(_N_cr_T / 1000, dec_place_num2).ToString() + " kN " + "\n"
                    + " Ny.b.Rd = " + Math.Round(_Ny_b_Rd / 1000, dec_place_num2).ToString() + " kN " + "\n"
                    + " Nz.b.Rd = " + Math.Round(_Nz_b_Rd / 1000, dec_place_num2).ToString() + " kN " + "\n"
                    + " NT.b.Rd = " + Math.Round(_NT_b_Rd / 1000, dec_place_num2).ToString() + " kN " + "\n"
                    + " Nb.Rd = " + Math.Round(_N_b_Rd / 1000, dec_place_num2).ToString() + " kN " + "\n"
                    + "\n"
                    + " Shear resistance: " + "\n"
                    + "\n"
                    + " Vy.c.Rd = " + Math.Round(_Vy_c_Rd / 1000, dec_place_num2).ToString() + " kN " + "\n"
                    + " Vz.c.Rd = " + Math.Round(_Vz_c_Rd / 1000, dec_place_num2).ToString() + " kN " + "\n"
                    + "\n"
                    + " Bending resistance: " + "\n"
                    + "\n"
                    + " My.c.Rd = " + Math.Round(_My_c_Rd / 1000000, dec_place_num2).ToString() + " kNm " + "\n"
                    + " Mz.c.Rd = " + Math.Round(_Mz_c_Rd / 1000000, dec_place_num2).ToString() + " kNm " + "\n"
                    + " My.el.Rd = " + Math.Round(_My_el_Rd / 1000000, dec_place_num2).ToString() + " kNm " + "\n"
                    + " Mz.el.Rd = " + Math.Round(_Mz_el_Rd / 1000000, dec_place_num2).ToString() + " kNm " + "\n"
                    + " My.pl.Rd = " + Math.Round(_My_pl_Rd / 1000000, dec_place_num2).ToString() + " kNm " + "\n"
                    + " Mz.pl.Rd = " + Math.Round(_Mz_pl_Rd / 1000000, dec_place_num2).ToString() + " kNm " + "\n"
                    + " My.V.el.Rd = " + Math.Round(_My_V_el_Rd / 1000000, dec_place_num2).ToString() + " kNm " + "\n"
                    + " Mz.V.el.Rd = " + Math.Round(_Mz_V_el_Rd / 1000000, dec_place_num2).ToString() + " kNm " + "\n"
                    + " My.V.pl.Rd = " + Math.Round(_My_V_pl_Rd / 1000000, dec_place_num2).ToString() + " kNm " + "\n"
                    + " Mz.V.pl.Rd = " + Math.Round(_Mz_V_pl_Rd / 1000000, dec_place_num2).ToString() + " kNm " + "\n"
                    + " My.Rk = " + Math.Round(_My_Rk / 1000000, dec_place_num2).ToString() + " kNm " + "\n"
                    + " Mz.Rk = " + Math.Round(_Mz_Rk / 1000000, dec_place_num2).ToString() + " kNm "), "Calculation results ");


            }
            #endregion
            if (item5_check == true)
            #region Check ratios
            {
                MessageBox.Show
                    ((
                    " Check ratios: " + "\n"

                    + "\n"
                        + " Section 6.2.3 Tension - formula (6.5): " + "\n"
                        + " NEd/Nt.Rd = " + Math.Round(ratio_65 * 100, dec_place_num2).ToString() + " % " + "\n"
                        + "\n"
                        + " 6.2.4 Compression - formula (6.9): " + "\n"
                        + " NEd/Nc.Rd = " + Math.Round(ratio_69 * 100, dec_place_num2).ToString() + " % " + "\n"
                        + "\n"
                        + " 6.2.5 Bending moment  - formula (6.12): " + "\n"
                        + " My.Ed/My.c.Rd = " + Math.Round(ratio_612y * 100, dec_place_num2).ToString() + " % " + "\n"
                        + " Mz.Ed/Mz.c.Rd = " + Math.Round(ratio_612z * 100, dec_place_num2).ToString() + " % " + "\n"
                        + "\n"
                        + " 6.2.6 Shear  - formula (6.17): " + "\n"
                        + " Vy.Ed/Vy.c.Rd = " + Math.Round(ratio_617y * 100, dec_place_num2).ToString() + " % " + "\n"
                        + " Vz.Ed/Vz.c.Rd = " + Math.Round(ratio_617z * 100, dec_place_num2).ToString() + " % " + "\n"
                        + "\n"
                        + " 6.2.7 Torsion - formula (6.23): " + "\n"
                        + " Tx.Ed/TRd = " + Math.Round(ratio_623 * 100, dec_place_num2).ToString() + " % " + "\n"
                        + "\n"
                        + " 6.2.8 Bending and shear (torsion): " + "\n"
                        + " My.Ed/My.V.Rd = " + Math.Round(Ratio_612y_MV * 100, dec_place_num2).ToString() + " % " + "\n"
                        + " Mz.Ed/Mz.V.Rd = " + Math.Round(ratio_612z_MV * 100, dec_place_num2).ToString() + " % " + "\n"
                        + "\n"
                        + " 6.2.9 Bending and axial force (6.31)(6.42)for cross-section classes 1 and 2 or (6.43) for class 3 and (6.44) for class 4: " + "\n"
                        + " σx.Ed/fyd = " + Math.Round(ratio_629_max_d * 100, dec_place_num2).ToString() + " % " + "\n"
                        + "\n"
                        + " 6.3.1 Compression - Buckling resistance of members (6.46): " + "\n"
                        + " NEd/Nb.Rd = " + Math.Round(ratio_646 * 100, dec_place_num2).ToString() + " % " + "\n"
                        + "\n"
                        + " 6.3.2 Uniform members in bending (6.54): " + "\n"
                        + " My.Ed/My.b.Rd = " + Math.Round(ratio_654y * 100, dec_place_num2).ToString() + " % " + "\n"
                        + " Mz.Ed/Mz.b.Rd = " + Math.Round(ratio_654z * 100, dec_place_num2).ToString() + " % " + "\n"
                        + "\n"
                        + " 6.3.3 Uniform members in bending and axial compression (6.61) and (6.62): " + "\n"
                        + " NEd/Ny.b.Rd = " + Math.Round(ratio_661N * 100, dec_place_num2).ToString() + " % " + "\n"
                        + " NEd/Nz.b.Rd = " + Math.Round(ratio_662N * 100, dec_place_num2).ToString() + " % " + "\n"
                        + " My.Ed/My.b.Rd = " + Math.Round(ratio_661My * 100, dec_place_num2).ToString() + " % " + "\n"
                        + " Mz.Ed/Mz.Rd = " + Math.Round(ratio_661Mz * 100, dec_place_num2).ToString() + " % " + "\n"
                        + " Ratio for interaction Nc.kyyMy.kyzMz (6.61) = " + Math.Round(ratio_661 * 100, dec_place_num2).ToString() + " % " + "\n"
                        + " Ratio for interaction Nc.kzyMy.kzzMz (6.62) = " + Math.Round(ratio_662 * 100, dec_place_num2).ToString() + " % " + "\n"
                        + "\n"
                        + " Maximum ratio = " + Math.Round(ratio_maxtot * 100, dec_place_num2).ToString() + " % "),"Check ratios");

            }
            #endregion
            #endregion



        }
        #endregion
    }

}