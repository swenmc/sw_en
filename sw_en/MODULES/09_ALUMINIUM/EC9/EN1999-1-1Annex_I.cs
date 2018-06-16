using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CENEX
{
    class EN1999_1_1Annex_I
    {



        #region Variables


        #region Variables - global

        // Material

        // MATERIAL MAIN TYPE

        double mat_kind; // 0 - user, 2 - concrete, 3 - steel, 5 - timber, 6 - masonry, 9 - aluminium, 10 - glass, 11 - soil

        public double Mat_kind
        {
            get { return mat_kind; }
            set { mat_kind = value; }
        }

        // MATERIAL SUBTYPE
        // 001 - user
        // 201 - general concrete
        // 301 - general steel, 101 - stainless                 
        // 501 - natural timber            
        // 601 - masonry - bricks
        // 901 - general aluminium 
        // 1001 - general glass
        // 1101 - general soil

        double mat_kind_sub;

        public double Mat_kind_sub
        {
            get { return mat_kind_sub; }
            set { mat_kind_sub = value; }
        }
        

        double f; // yield stress of material - generally

        double fy; // yield stress of steel

        public double Fy
        {
            get { return fy; }
            set { fy = value; }
        }
        double fo; // yield stress of aluminium

        public double Fo
        {
            get { return fo; }
            set { fo = value; }
        }

        double _E;
        double _G;

        public double G
        {
            get { return _G; }
            set { _G = value; }
        }
        double mat_nu;
        // cross-section

        double t1;
        double t2;
        double _D; // ??????????????????? (I.12)


        double _Wy_el;
        double _Wz_el;

       double _Wy;
       double _Wz;

        double _Iy;
        double _Iz;
        double _It;
        double _Iw;

        double iy_r;
        double iz_r;

        // cross-section factors for Lateral-torsial buckling calculation  - exactly for lambda_y_LT

        double x_I5;

        public double X_I5
        {
            get { return x_I5; }
            set { x_I5 = value; }
        }
        double y_I5;

        public double Y_I5
        {
            get { return y_I5; }
            set { y_I5 = value; }
        }

        // shape of cross-section
        // local axis symmetry
        bool symmetry_yy;
        bool symmetry_zz;
        //principal axis symmetry
        bool symmetry_uu;
        bool symmetry_vv;

        // loading moments My

        bool load_I1_My_end;
        bool load_I2_My_tra;

        // beam scheme

        bool member_simple; // simple beam
        bool member_cantilever; // cantilever

        // loading 

        bool member_load_F; // discrete force
        bool member_load_LF; // uniform load along beam

        // general


        bool cs_const; // constant CS
        bool loading_CS; // true if beam is loaded in CS - Shear centre of CS

        // moment ratio

        double psi_My;
        double psi_Mz;

        // restrain of beam
        double ky_LT;
        double kz_LT;
        double kw_LT;


        double _C1y;
        double _C2y;
        double _C3y;
        // length

        double _L_y_LT;




        double psi_f; // Tab I.1




        // the elastic critical moment for lateral-torsional buckling

        double _My_cr;
        double _Mz_cr;

        

        double mu_cr_y;
        double mu_cr_z;
       
        double kappa_wt_LT;
        double zeta_g_LT;
        double zeta_j_LT;
        double zg_LT;
        double zj_LT;

        double lambda_y_LT;
        double lambda_z_LT;
        double lambda_y_LT_rel;
        double lambda_z_LT_rel;

        // Variables from main part of EN 199-1-1

        double alpha_y;
        double alpha_z;

        // Variables user 

        double _My_max;
        double _My_025;
        double _My_050;
        double _My_075;



        #endregion









        #endregion

        

        #region Constructor
        public EN1999_1_1Annex_I()
        {











        }
        #endregion

        #region Auxiliary calculation
        public void EN_1999_1_1_Annex_I_AC()
        
        {

            if (mat_kind == 3) f = fy; // steel
            if (mat_kind == 9) f = fo; // aluminium

        }




         #endregion
        #region Annex I - Informative
        #region Annex I - Factors ky, kz, kw, C1, C2, C3

        double _C10y;
        double _C11y;

        public void EN_1999_1_1_Annex_I_tabI1()
        {

            // 2D array
            double [,] EN_1999_1_1_Tab_I1 = new double [9*4+1, 6+1]

        
        {
        {0.000, 1.000, 2.000, 3.000, 4.000, 5.000, 6.000} , // Row ID 00
        // M diagram Nr. 1
        {1.00, 1.000, 1.000, 1.000, 1.000, 1.000, 1.000} , // Row ID 01
        {0.71, 1.016, 1.100, 1.025, 1.025, 1.000, 1.000} , // Row ID 02
        {0.72, 1.016, 1.100, 1.025, 1.025, 1.000, 1.000} , // Row ID 03
        {0.50, 1.000, 1.127, 1.019, 1.019, 1.019, 1.019} , // Row ID 04
        // M diagram Nr. 2
        {1.00, 1.139, 1.141, 1.000, 1.000, 1.000, 1.000} , // Row ID 05
        {0.71, 1.210, 1.313, 1.050, 1.050, 1.000, 1.000} , // Row ID 06
        {0.72, 1.109, 1.201, 1.000, 1.000, 1.000, 1.000} , // Row ID 07
        {0.50, 1.139, 1.285, 1.017, 1.017, 1.017, 1.017} , // Row ID 08
        // M diagram Nr. 3
        {1.00, 1.312, 1.320, 1.150, 1.000, 1.000, 1.000} , // Row ID 09
        {0.71, 1.480, 1.616, 1.160, 1.160, 1.000, 1.000} , // Row ID 10
        {0.72, 1.213, 1.317, 1.000, 1.000, 1.000, 1.000} , // Row ID 11
        {0.50, 1.310, 1.482, 1.150, 1.000, 1.000, 1.000} , // Row ID 12
        // M diagram Nr. 4
        {1.00, 1.522, 1.551, 1.290, 1.000, 1.000, 1.000} , // Row ID 13
        {0.71, 1.853, 2.059, 1.600, 1.260, 1.000, 1.000} , // Row ID 14
        {0.72, 1.329, 1.467, 1.000, 1.000, 1.000, 1.000} , // Row ID 15
        {0.50, 1.516, 1.730, 1.350, 1.000, 1.000, 1.000} , // Row ID 16
        // M diagram Nr. 5
        {1.00, 1.770, 1.847, 1.470, 1.000, 1.000, 1.000} , // Row ID 17
        {0.71, 2.331, 2.683, 2.000, 1.420, 1.000, 1.000} , // Row ID 18
        {0.72, 1.453, 1.592, 1.000, 1.000, 1.000, 1.000} , // Row ID 19
        {0.50, 1.753, 2.027, 1.500, 1.000, 1.000, 1.000} , // Row ID 20
        // M diagram Nr. 6
        {1.00, 2.047, 2.207, 1.650, 1.000, 0.850, 0.850} , // Row ID 21
        {0.71, 2.827, 3.322, 2.400, 1.550, 0.850, -0.300} , // Row ID 22
        {0.72, 1.582, 1.748, 1.380, 0.850, 0.700, 0.200} , // Row ID 23
        {0.50, 2.004, 2.341, 1.750, 1.000, 0.650, -0.250} , // Row ID 24
        // M diagram Nr. 7
        {1.00, 2.331, 2.591, 1.850, 1.000, 1.3-1.2*psi_f, -0.700} , // Row ID 25
        {0.71, 3.078, 3.399, 2.700, 1.450, 1-1.2*psi_f, -1.150} , // Row ID 26
        {0.72, 1.711, 1.897, 1.450, 0.780, 0.9 - 0.75*psi_f, -0.530} , // Row ID 27
        {0.50, 2.230, 2.579, 2.000, 0.950, 0.750 - psi_f, -0.850} , // Row ID 28
        // M diagram Nr. 8
        {1.00, 2.547, 2.852, 2.000, 1.000, 0.55-psi_f, -1.450} , // Row ID 29
        {0.71, 2.592, 2.770, 2.000, 0.850, 0.230- 0.9*psi_f, -1.550} , // Row ID 30
        {0.72, 1.829, 2.027, 1.550, 0.700, 0.68 - psi_f, -1.070} , // Row ID 31
        {0.50, 2.352, 2.606, 2.000, 0.850, 0.35 - psi_f, -1.450} , // Row ID 32
        // M diagram Nr. 9
        {1.00, 2.555, 2.733, 2.000, - psi_f, - psi_f, -2.000} , // Row ID 33
        {0.71, 1.921, 2.103, 1.550, 0.380,-0.580,-1.550} , // Row ID 34
        {0.72, 1.921, 2.103, 1.550, 0.580,-0.380,-1.550} , // Row ID 35
        {0.50, 2.223, 2.390, 1.880, 0.125-0.7*psi_f,-0.125-0.7*psi_f,-1.880} , // Row ID 36
        
        };
      
        }

        public void EN_1999_1_1_Annex_I_tabI2()
        {

            // 2D array
            double[,] EN_1999_1_1_Tab_I2 = new double[3 * 4 + 2 * 3 + 1, 3 + 2 + 3 + 3]

        
        {
        {0.000, 1.000, 2.000, 3.000, 4.000, 5.000, 6.000, 7.000, 8.000, 9.000, 10.000} , // Row ID 00
        // M diagram Nr. 1
        {1.000, 1.000, 1.000, 1.127, 1.132, 0.330, 0.459, 0.500, 0.930, 0.525, 0.380} , // Row ID 01
        {1.000, 1.000, 0.500, 1.128, 1.231, 0.330, 0.391, 0.500, 0.930, 0.806, 0.380} , // Row ID 02
        {1.000, 0.500, 1.000, 0.947, 0.997, 0.250, 0.407, 0.400, 0.840, 0.478, 0.440} , // Row ID 03
        {1.000, 0.500, 0.500, 0.947, 0.970, 0.250, 0.310, 0.400, 0.840, 0.674, 0.440} , // Row ID 04
        // M diagram Nr. 2
        {1.000, 1.000, 1.000, 1.348, 1.363, 0.520, 0.553, 0.420, 1.000, 0.411, 0.310} , // Row ID 05
        {1.000, 1.000, 0.500, 1.349, 1.452, 0.520, 0.580, 0.420, 1.000, 0.666, 0.310} , // Row ID 06
        {1.000, 0.500, 1.000, 1.030, 1.087, 0.400, 0.449, 0.420, 0.800, 0.338, 0.310} , // Row ID 07
        {1.000, 0.500, 0.500, 1.031, 1.067, 0.400, 0.437, 0.420, 0.800, 0.516, 0.310} , // Row ID 08
        // M diagram Nr. 3
        {1.000, 1.000, 1.000, 1.038, 1.040, 0.330, 0.431, 0.390, 0.930, 0.562, 0.390} , // Row ID 09
        {1.000, 1.000, 0.500, 1.039, 1.148, 0.330, 0.292, 0.390, 0.930, 0.878, 0.390} , // Row ID 10
        {1.000, 0.500, 1.000, 0.922, 0.960, 0.280, 0.404, 0.300, 0.880, 0.539, 0.500} , // Row ID 11
        {1.000, 0.500, 0.500, 0.922, 0.945, 0.280, 0.237, 0.300, 0.880, 0.772, 0.500} , // Row ID 12


        // M diagram Nr. 4
        {0.500, 1.000, 1.000, 2.576, 2.608, 1.000, 1.562, 0.150, 1.000, -0.859, -1.990} , // Row ID 13
        {0.500, 0.500, 1.000, 1.490, 1.515, 0.560, 0.900, 0.080, 0.610, -0.516, -1.200} , // Row ID 14
        {0.500, 0.500, 0.500, 1.494, 1.746, 0.560, 0.825, 0.080, 0.610, 0.002712, -1.200} , // Row ID 15
     
        // M diagram Nr. 5
        {0.500, 1.000, 1.000, 1.683, 1.726, 1.200, 1.388, 0.070, 1.150, -0.716, -1.350} , // Row ID 16
        {0.500, 0.500, 1.000, 0.936, 0.955, 0.690, 0.763, 0.030, 0.640, -0.406, -0.760} , // Row ID 17
        {0.500, 0.500, 0.500, 0.937, 1.057, 0.690, 0.843, 0.030, 0.640, -0.0679, -0.760} , // Row ID 18
     
        
        
        };

        }

        public void EN_1999_1_1_Annex_I_tabI3()
        {

            // 2D array
            double[,] EN_1999_1_1_Tab_I3 = new double[5 * 5 + 1+1, 8]

        
        {
        {0.000, 1.000, 2.000, 3.000, 4.000, 5.000, 6.000, 7.000} , // Row ID 00
        {0.000, -4.000, -2.000, -1.000, 0.000, 1.000, 2.000, 4.000} , // Row ID 00
        // Values for cantilever
        {4.000, 0.107, 0.156, 0.194, 0.245, 0.316, 0.416, 0.759} , // Row ID 01
        {2.000, 0.123, 0.211, 0.302, 0.463, 0.759, 1.312, 4.024} , // Row ID 02
        {0.000, 0.128, 0.254, 0.478, 1.280, 3.178, 5.590, 10.730} , // Row ID 03
        {-2.000, 0.129, 0.258, 0.508, 1.619, 3.894, 6.500, 11.860} , // Row ID 04
        {-4.000, 0.129, 0.258, 0.511, 1.686, 4.055, 6.740, 12.240} , // Row ID 05

        {4.000, 0.151, 0.202, 0.240, 0.293, 0.367, 0.475, 0.899} , // Row ID 06
        {2.000, 0.195, 0.297, 0.393, 0.560, 0.876, 1.528, 5.360} , // Row ID 07
        {0.000, 0.261, 0.495, 0.844, 1.815, 3.766, 6.170, 11.295} , // Row ID 08
        {-2.000, 0.329, 0.674, 1.174, 2.423, 4.642, 7.235, 12.595} , // Row ID 09
        {-4.000, 0.364, 0.723, 1.235, 2.529, 4.843, 7.540, 13.100} , // Row ID 10

        {4.000, 0.198, 0.257, 0.301, 0.360, 0.445, 0.573, 1.123} , // Row ID 11
        {2.000, 0.268, 0.391, 0.502, 0.691, 1.052, 1.838, 6.345} , // Row ID 12
        {0.000, 0.401, 0.750, 1.243, 2.431, 4.456, 4.456, 11.920} , // Row ID 13
        {-2.000, 0.629, 1.326, 2.115, 3.529, 5.635, 6.840, 13.365} , // Row ID 14
        {-4.000, 0.777, 1.474, 2.264, 3.719, 5.915, 8.115, 13.960} , // Row ID 15
          
        {4.000, 0.335, 0.428, 0.496, 0.588, 0.719, 0.916, 1.795} , // Row ID 16
        {2.000, 0.461, 0.657, 0.829, 1.111, 1.630, 2.698, 7.815} , // Row ID 17
        {0.000, 0.725, 1.321, 2.079, 3.611, 5.845, 8.270, 13.285} , // Row ID 18
        {-2.000, 1.398, 3.003, 4.258, 5.865, 7.845, 10.100, 15.040} , // Row ID 19
        {-4.000, 2.119, 3.584, 4.760, 6.360, 8.385, 10.715, 15.825} , // Row ID 20
     
        {4.000, 0.845, 1.069, 1.230, 1.443, 1.739, 2.168, 3.866} , // Row ID 16
        {2.000, 1.159, 1.614, 1.992, 2.569, 3.498, 5.035, 10.345} , // Row ID 17
        {0.000, 1.801, 3.019, 4.231, 6.100, 8.495, 11.060, 16.165} , // Row ID 18
        {-2.000, 3.375, 6.225, 8.035, 9.950, 11.975, 14.110, 18.680} , // Row ID 19
        {-4.000, 5.530, 8.130, 9.660, 11.375, 13.285, 15.365, 19.925} , // Row ID 20
       
            
        };

        }

        public void EN_1999_1_1_Annex_I_tabI4()
        {

            // 2D array
            double[,] EN_1999_1_1_Tab_I4 = new double[5 * 5 + 1 + 1, 8]

        
        {
        {0.000, 1.000, 2.000, 3.000, 4.000, 5.000, 6.000, 7.000} , // Row ID 00
        {0.000, -4.000, -2.000, -1.000, 0.000, 1.000, 2.000, 4.000} , // Row ID 00
        // Values for cantilever
        {4.000, 0.113, 0.173, 0.225, 0.304, 0.431, 0.643, 1.718} , // Row ID 01
        {2.000, 0.126, 0.225, 0.340, 0.583, 1.165, 2.718, 13.270} , // Row ID 02
        {0.000, 0.132, 0.263, 0.516, 2.054, 6.945, 12.925, 25.320} , // Row ID 03
        {-2.000, 0.134, 0.268, 0.537, 3.463, 10.490, 17.260, 30.365} , // Row ID 04
        {-4.000, 0.134, 0.270, 0.541, 4.273, 12.715, 20.135, 34.005} , // Row ID 05

        {4.000, 0.213, 0.290, 0.352, 0.443, 0.586, 0.823, 2.046} , // Row ID 06
        {2.000, 0.273, 0.421, 0.570, 0.854, 1.505, 3.229, 14.365} , // Row ID 07
        {0.000, 0.371, 0.718, 1.287, 3.332, 8.210, 14.125, 26.440} , // Row ID 08
        {-2.000, 0.518, 1.217, 2.418, 6.010, 12.165, 18.685, 31.610} , // Row ID 09
        {-4.000, 0.654, 1.494, 2.950, 7.460, 14.570, 21.675, 35.320} , // Row ID 10

        {4.000, 0.336, 0.441, 0.522, 0.636, 0.806, 1.080, 2.483} , // Row ID 11
        {2.000, 0.449, 0.663, 0.865, 1.224, 1.977, 3.873, 15.575} , // Row ID 12
        {0.000, 0.664, 1.263, 2.172, 4.762, 9.715, 15.530, 27.735} , // Row ID 13
        {-2.000, 1.109, 2.731, 4.810, 8.695, 14.250, 20.425, 33.075} , // Row ID 14
        {-4.000, 1.623, 3.558, 6.025, 10.635, 16.880, 23.555, 36.875} , // Row ID 15
          
        {4.000, 0.646, 0.829, 0.965, 1.152, 1.421, 1.839, 3.865} , // Row ID 16
        {2.000, 0.885, 1.268, 1.611, 2.185, 3.282, 5.700, 18.040} , // Row ID 17
        {0.000, 1.383, 2.550, 4.103, 7.505, 12.770, 18.570, 30.570} , // Row ID 18
        {-2.000, 2.724, 6.460, 9.620, 13.735, 18.755, 24.365, 36.365} , // Row ID 19
        {-4.000, 4.678, 8.635, 11.960, 16.445, 21.880, 27.850, 40.400} , // Row ID 20
     
        {4.000, 1.710, 2.168, 2.500, 2.944, 3.565, 4.478, 8.260} , // Row ID 16
        {2.000, 2.344, 3.279, 4.066, 5.285, 7.295, 10.745, 23.150} , // Row ID 17
        {0.000, 3.651, 6.210, 8.845, 13.070, 18.630, 24.625, 36.645} , // Row ID 18
        {-2.000, 7.010, 13.555, 17.850, 22.460, 27.375, 32.575, 43.690} , // Row ID 19
        {-4.000, 12.270, 18.705, 22.590, 26.980, 31.840, 37.090, 48.390} , // Row ID 20
       
            
        };

        }












        





        public void EN_1999_1_1_Annex_I_factors ()
        {


            #region I.1.2 Obecny vztah pro nosniky konstantniho prurezu symetricke k jedne z hlavnich os

            double za_LT = 100; // missing formula or DATABASE data
            double zs_LT = 100; // missing formula or DATABASE data
            double psi_f_y_LT = 1; // missing formula or DATABASE data
            double hf = 100;
            double _Ifc_z = 1000000; // moment setrvačnosti tlačené pásnice k ose nejmenší tuhosti průřezu
            double _Ift_z = 1100000; // moment setrvačnosti tažené pásnice k ose nejmenší tuhosti průřezu



            zg_LT = za_LT - zs_LT;

            // page 95:    zj_LT = -0.5/_Iy* (y*y + z*z) z dA.

            // page 159
            double hs = 10; // missing formula or DATABASE data
            double c = 10; // missing formula or DATABASE data
            psi_f_y_LT = (_Ifc_z - _Ift_z) / (_Ifc_z + _Ift_z);
            zj_LT = 0.45 * psi_f_y_LT * hs * (1 + (c / 2 * hf)); // (I.4)



            kappa_wt_LT = (Math.PI / (kw_LT * _L_y_LT)) * Math.Sqrt((_E * _Iw) / (_G * _It)); // (I.2)
            zeta_g_LT = ((Math.PI * zg_LT) / (kz_LT * _L_y_LT)) * Math.Sqrt((_E * _Iz) / (_G * _It));
            zeta_j_LT = ((Math.PI * zj_LT) / (kz_LT * _L_y_LT)) * Math.Sqrt((_E * _Iz) / (_G * _It));

            #endregion

            
            // Section I.1.2 (6)
            if (load_I1_My_end == true && kz_LT == 1)
                _C1y = Math.Pow((0.310 + 0.428 * psi_My + 0.262 * psi_My * psi_My), 0.5);

            // Table I.1

            if (load_I1_My_end == true && member_simple == true && ky_LT ==1 && kw_LT == 1)
            {
                if (kappa_wt_LT <= _C11y) _C1y = _C10y + (_C11y - _C10y);
                else if (kappa_wt_LT == 0) _C1y = _C10y;
                else if (kappa_wt_LT >= 1) _C1y = _C11y;
            }

            // Table I.2
            if (load_I2_My_tra == true && member_simple == true)
            {
                if (kappa_wt_LT <= _C11y) _C1y = _C10y + (_C11y - _C10y);
                else if (kappa_wt_LT == 0) _C1y = _C10y;
                else if (kappa_wt_LT >= 1) _C1y = _C11y;
            }


            // Section I.1.3

            // I.1.3 (4)

            if (ky_LT == 1 && kz_LT == 1 && 0.5 <= kw_LT && kw_LT <= 1)

                _C1y = Math.Min((1.7 * Math.Abs(_My_max)) / Math.Sqrt((Math.Pow(_My_025, 2) + Math.Pow(_My_050, 2) + Math.Pow(_My_075, 2))),2.5); // (I.9)

            // I.1.3 (1)

           
            
            if (cs_const == true && loading_CS == true && (symmetry_yy == true || symmetry_uu == true))

            {
                zj_LT = 0;
                mu_cr_y = _C1y / kz_LT * (Math.Sqrt(1 + Math.Pow(kappa_wt_LT, 2) + Math.Pow(_C2y * zeta_g_LT, 2)) - _C2y * zeta_g_LT); // (I.7)
            }

            // I.1.3 (2)

            if (cs_const == true && loading_CS == true && (symmetry_yy == true || symmetry_uu == true) && load_I1_My_end == true)
            {
                _C2y = 0;
                zg_LT = 0;
                zj_LT = 0;
                mu_cr_y = _C1y / kz_LT * (Math.Sqrt(1 + Math.Pow(kappa_wt_LT, 2))); // (I.8)
            }
            // I.1.3 (3)

            if (cs_const == true && loading_CS == true && (symmetry_yy == true || symmetry_uu == true) && load_I1_My_end == true && kappa_wt_LT == 0)
            {
                _C2y = 0;
                zg_LT = 0;
                zj_LT = 0;
                kappa_wt_LT = 0;
               mu_cr_y = _C1y / kz_LT;
            }

            // Section I.1.4 - Cantilevers





        }
        #endregion
        #region Annex I - Critical moment
        public void EN_1999_1_1_Annex_I()
        {


            #region I.1 Elastic critical moment for lateral torsional buckling and slenderness / Pruzny kriticky moment a stihlost
            #region I.1.1 Principy

            // (1) Pružný kritický moment pro klopení nosníků s konstantním symetrickým průřezem se stejnými pásnicemi,
            //     za běžných podmínek uložení na obou koncích, který je namáhán rovnoměrným momentem v rovině procházející
            //     středem smyku, se stanoví jako:

            // (2) Bezne podminky ulozeni na obou koncich

            if (
            ky_LT == 1 && // podepření proti posunutí vzhledem v rovině zatížení, volné natočení v této rovině
            kz_LT == 1 && // vodorovnému posunutí, volné natočení v rovině
            kw_LT == 1&& // podepření proti uložení natočení vzhledem k podélné ose, deplanace umožněna
            symmetry_yy == true) // symetrie prurezu
                {

            _G = _E / (2 *(1 + mat_nu));

            _My_cr = ((Math.PI * _E * _Iz) / Math.Pow (_L_y_LT,2)) * Math.Sqrt((Math.Pow(_L_y_LT, 2) / Math.Pow(Math.PI, 2)) * ((_G * _It) / (_E * _Iz)) + _Iw / _Iz); // (I.1)
                }

            #endregion

            #region I.1.2 Obecny vztah pro nosniky konstantniho prurezu symetricke k jedne z hlavnich os

                       
            mu_cr_y = _C1y / kz_LT * ((Math.Sqrt(1 + Math.Pow(kappa_wt_LT, 2) + Math.Pow((_C2y * zeta_g_LT - _C3y * zeta_j_LT), 2))) - (_C2y * zeta_g_LT - _C3y * zeta_j_LT));  // (I.3)
            mu_cr_z = 0.1; //  // just auxiliary not used for calculation,  axis y-y is main,   _Iy > _Iz - it is usual 
       
            

            // (1) V případě nosníku konstantního průřezu, který je symetrický k ose nejmenší tuhosti, je při ohybu v rovině
            //     větší tuhosti dán pružný kritický moment pro klopení obecným vztahem:
            _My_cr = mu_cr_y * ((Math.PI * Math.Sqrt(_E * _Iz * _G * _It)) / _L_y_LT); // (I.2)
            _Mz_cr = 1; // just auxiliary not used for calculation,  axis y-y is main,   _Iy > _Iz - it is usual 


             #endregion

            #region I.2 Slenderness for lateral-torsial buckling / Stihlosti pro klopeni

            // Steel NAD CZ

            lambda_y_LT_rel = Math.Sqrt((_Wy * fy) / _My_cr); // (p.61)
            lambda_z_LT_rel = Math.Sqrt((_Wz * fy) / _Mz_cr); // (p.61)

            // Aluminium Annex I

            // (1)
            lambda_y_LT_rel = Math.Sqrt((alpha_y * _Wy_el * fo) / _My_cr); // (I.10)
            // alpha_y - Table 6.4 in EN 1999-1-1

            //(2) For I (H) and U section in EN 1999-1-1 Annex I Table I.5 

            

            // _L_y_LT = _L_z_buck
            lambda_y_LT_rel = (X_I5 * _L_y_LT / iz_r) / Math.Pow(1 + Y_I5 * ((_L_y_LT / iz_r) / (_D / t2)), 1 / 4); // (I.12) !!!!???? what is D  ????




            lambda_y_LT_rel = lambda_y_LT * 1 / Math.PI * Math.Sqrt( alpha_y * fo / _E); // (I.11)


             #endregion

            #endregion












        }
        #endregion
        #endregion



























    }
}
