using System;

namespace CRSC
{
    class CSGClass
    {
        #region   Variables
        #region   Variables Variables - general


        int cs_shape;

        bool symmetry_yy;
        bool symmetry_zz;

        bool symmetry_uu;
        bool symmetry_vv;

        #endregion

        #region   Variables EC9

        double b;
        double h;
        double t1;
        double t2;


        double t;

        double c; // stiffener length

        double b_axial;
        double hf;
        double e;
        double b2;
        double _Iy;
        double _Iz;
        double _Iw;

        double X;
        double Y;

        double ro;
        double alpha;
        double beta;
        double delta;

        double lambda_0;
        double lambda_u;
        double lambda_y;
        double lambda_z;

        double lambda_t;
        double s;

        double ri;
        double r;

        double distance_L_yy;
        
                
              


        // ratios of dimensions - calculations of limits

        double ratio_hIb;
        double ratio_bIh;
        double ratio_t2It1;
        double ratio_cIb;

        double ratio_riIt;
        double ratio_hIt;

        double ratio_bIt;
        double ratio_bIt2;
        double ratio_rIt; // ro
        double ratio_16zIt; // ro
        

        double ratio_lambda_uIlambda_0;
        double ratio_lambda_yIlambda_t;
        double ratio_lambda_zIlambda_0;
        
        #endregion  
        #endregion

        // Constructor

        public CSGClass ()
    {

        







    }






        // GENERAL CROSS-SECTION PROPERTIES CALCULATION




        // 0 - GENERAL CROSS-SECTION SHAPES



        // EC2- CONCRETE CROSS-SECTIONS
        public void CS_EC2 ()
        {



        }


        // EC3 - STEEL CROSS-SECTIONS

        public void CS_EC3()
        {



        }


        // EC4 - COMPOSITE STELL AND CONCRETE CROSS-SECTIONS
        public void CS_EC4()
        {



        }


        // EC5 - TIMBER CROSS-SECTIONS
        public void CS_EC5()
        {



        }


        // EC6 - MASONRY ???




        // EC7 - SOIL ???




        // EC9 - ALUMINIUM
        public void CS_EC9()
        {

            // Table I.5
            #region 901
            if (cs_shape == 901)
            {
                // 901 - 1 - I symmetrical
                // Symmetry
                symmetry_yy = true;
                symmetry_zz = true;

                _Iw = hf * hf * _Iz / 4; // Picture J.2 (1)

                // LT factors
                
                if(1.5 <= ratio_hIb && ratio_hIb <= 4.5 && 1 <= ratio_t2It1 && ratio_t2It1 <= 2)
            {

                X = 0.9 - 0.03 * ratio_hIb + 0.04 * ratio_t2It1;
                Y = 0.05 - 0.01 * Math.Sqrt((ratio_t2It1 - 1) * ratio_hIb);
            }
            }

            #endregion
            #region 902
            else if (cs_shape == 902)
            {
                // 902 - 2 - I symmetrical - with stiffened flanges
                // Symmetry
                symmetry_yy = true;
                symmetry_zz = true;

                _Iw = (hf * hf * _Iy / 4) + (c * c * b_axial * b_axial * t / 6) * (3 * hf + 2 * c); // Picture J.2 (8)

                // LT factors
                
                if(1.5 <= ratio_hIb && ratio_hIb <= 4.5 && 0 <= ratio_cIb && ratio_cIb <= 0.5)
            {

                X = 0.94 - (0.03 - 0.07 * ratio_cIb) * ratio_hIb - 0.3 * ratio_cIb;
                Y = 0.05 - 0.06 * c / h;
            }
            }

            #endregion
            #region 903
            else if (cs_shape == 903)
            {
                // 903 - 3 - U symmetrical
                // Symmetry
                symmetry_yy = true;
                symmetry_zz = false;

                e = 3 * b2 * t2 / (hf * t1 + (6 * b_axial * t2));
                _Iw = (hf * hf * Math.Pow(b_axial, 3) * t2 / 12) * ((2 * hf * t1) + (3 * b_axial * t2)) / (hf * t1 + 6 * b_axial * t2); // Picture J.2 (3)
                // LT factors
               
                if(1.5 <= ratio_hIb && ratio_hIb <= 4.5 && 1 <= ratio_t2It1 && ratio_t2It1 <= 2)
            {

                X = 0.95 - 0.03 * ratio_hIb + 0.06 * ratio_t2It1;
                Y = 0.07 - 0.014 * Math.Sqrt((ratio_t2It1 - 1) * ratio_hIb);
            }
            }

            #endregion
            #region 904
            else if (cs_shape == 904)
            {
                // 904 - 4 - U symmetrical - with stiffened flanges
                // Symmetry
                symmetry_yy = true;
                symmetry_zz = false;

                e = (hf*hf*b_axial*b_axial*t/_Iy) * (1/4 + (c/2*b_axial) - (2* Math.Pow(c,3)/(3*hf*hf*b_axial))); // Picture J.2 (4)
                _Iw = b_axial*b_axial*t /6 * ((4*Math.Pow(c,3)) + 6*hf*c*c + 3*hf*hf*c + hf*hf*b_axial) - e*e*_Iy;
                // LT factors
                
                if(1.5 <= ratio_hIb && ratio_hIb <= 4.5 && 0 <= ratio_cIb && ratio_cIb <= 0.5)
            {

                X = 1.01 - (0.03 - 0.06 * c / b) * h / b - 0.3 * c / b;
                Y = 0.07 - 0.1 * c / h;
            }
            }

            #endregion
            // Table I.8
            #region 905
            else if (cs_shape == 905)
            {
                // 905 - 1 - L symmetrical
                // Symmetry
                symmetry_yy = false;
                symmetry_zz = false;
                symmetry_uu = true;
                symmetry_vv = false;

                _Iw = (Math.Pow(hf, 3) * Math.Pow(t1, 3) + Math.Pow(b_axial, 3) * Math.Pow(t2, 3)) / 36; // Picture J.2 (1)
                // LT factors
                ro = ratio_rIt;
                
                if(ro <=5)
            {

                lambda_0 = 5 * ratio_bIt - 0.6 * Math.Pow(ro, 1.5) * Math.Pow(ratio_bIt, 0.5);
                s = ratio_lambda_uIlambda_0;
                X = 0.6;
            }
            }

            #endregion
            #region 906
            else if (cs_shape == 906)
            {
                // 906 - 2 - L symmetrical with extension
                // Symmetry
                symmetry_yy = false;
                symmetry_zz = false;
                symmetry_uu = true;
                symmetry_vv = false;

                _Iw = (Math.Pow(hf, 3) * Math.Pow(t1, 3) + Math.Pow(b_axial, 3) * Math.Pow(t2, 3)) / 36; // Picture J.2 (1)
                // LT factors
                ro = ratio_rIt;
                
                if(ro <=5 && 1 <= delta && delta <= 2.5)
            {

                lambda_0 = 5 * ratio_bIt - 0.6 * Math.Pow(ro, 1.5) * Math.Pow(ratio_bIt, 0.5) - ((delta - 1) * (2 * Math.Pow(delta - 1, 2) - 1.5 * ro));
                s = ratio_lambda_uIlambda_0;
                X = 0.6;
            }
            }

            #endregion
            #region 907
            else if (cs_shape == 907)
            {
                // 907 - 3 - L symmetrical with extension / no radius
                // Symmetry
                symmetry_yy = false;
                symmetry_zz = false;
                symmetry_uu = true;
                symmetry_vv = false;
                _Iw = (Math.Pow(hf, 3) * Math.Pow(t1, 3) + Math.Pow(b_axial, 3) * Math.Pow(t2, 3)) / 36; // Picture J.2 (1)
                // LT factors
                ro = ratio_16zIt;
                            
                if(ratio_bIt == 20 && ratio_riIt ==2 && delta == 3 && beta == 4)
            {

                lambda_0 = 66;
                s = ratio_lambda_uIlambda_0;
                X = 0.61;
            }
            }

            #endregion
            #region 908
            else if (cs_shape == 908)
            {
                // 908 - 4 - L asymmetrical without extensions
                // Symmetry
                symmetry_yy = false;
                symmetry_zz = false;
                symmetry_uu = false;
                symmetry_vv = false;

                _Iw = (Math.Pow(hf, 3) * Math.Pow(t1, 3) + Math.Pow(b_axial, 3) * Math.Pow(t2, 3)) / 36; // Picture J.2 (1)
                // LT factors
                ro = ratio_rIt;
                
                if(ro <= 5 && 0.5 <= ratio_bIh && ratio_bIh <= 1)
            {
               
                lambda_0 = ratio_hIt * (4.2 + 0.8 * Math.Pow(ratio_bIh,2))- (0.6 * Math.Pow(ro,1.5) * Math.Pow(ratio_hIt, 0.5));
                s = (1 + 6 * Math.Pow((1- ratio_bIh),2))*ratio_lambda_uIlambda_0;
                X = 0.6 - 0.4 * Math.Pow((1- ratio_bIh),2);
            }
            }

            #endregion
            #region 909
            else if (cs_shape == 909)
            {
                // 909 - 5 - L asymmetrical with extensions
                // Symmetry
                symmetry_yy = false;
                symmetry_zz = false;
                symmetry_uu = false;
                symmetry_vv = false;

                _Iw = (Math.Pow(hf, 3) * Math.Pow(t1, 3) + Math.Pow(b_axial, 3) * Math.Pow(t2, 3)) / 36; // Picture J.2 (1)
                // LT factors
                ro = ratio_rIt;
               
                if(ro <= 5  && 0.5 <= ratio_bIh && ratio_bIh <= 1 && 1 <= delta && delta <= 2.5)
            {

                lambda_0 = ratio_hIt * (4.2 + 0.8 * Math.Pow(ratio_bIh, 2)) - (0.6 * Math.Pow(ro, 1.5) * Math.Pow(ratio_hIt, 0.5)) + (1.5 * ro * (delta - 1)) - 2* Math.Pow((delta - 1),3);
                s = (1 + 6 * Math.Pow((1 - ratio_bIh), 2)) * ratio_lambda_uIlambda_0;
                X = 0.6 - 0.4 * Math.Pow((1 - ratio_bIh), 2);
            }
            }

            #endregion
            #region 910
            else if (cs_shape == 910)
            {
                // 910 - 6 - L asymmetrical with extensions
                // Symmetry
                symmetry_yy = false;
                symmetry_zz = false;
                symmetry_uu = false;
                symmetry_vv = false;

                _Iw = (Math.Pow(hf, 3) * Math.Pow(t1, 3) + Math.Pow(b_axial, 3) * Math.Pow(t2, 3)) / 36; // Picture J.2 (1)
                // LT factors
                ro = ratio_16zIt;

                if(ratio_hIt == 20 && ratio_bIt == 15 && ratio_riIt == 2 && delta == 3 && beta == 4)
            {

                lambda_0 = 57;
                s = 1.4 * ratio_lambda_uIlambda_0;
                X = 0.6;
            }
            }

            #endregion
            #region 911
            else if (cs_shape == 911)
            {
                // 911 - 7 - X symmetrical
                // Symmetry
                symmetry_yy = true;
                symmetry_zz = true;
                symmetry_uu = true;
                symmetry_vv = true;

                _Iw = 0;
                // LT factors
                ro = ratio_rIt;
                
                if(ro <= 3.5)
            {

                lambda_0 = 5.1 * ratio_bIt - Math.Pow(ro,1.5) * Math.Pow(ratio_bIt, 0.5);
                
                X = 1;
            }
            }
            #endregion
            #region 912
            else if (cs_shape == 912)
            {
                // 912 - 8 - double L without extensions
                // Symmetry
                symmetry_yy = false;
                symmetry_zz = true;
                symmetry_uu = false;
                symmetry_vv = true;
                // LT factors

                ro = ratio_rIt;
                if (distance_L_yy <= 2*t && ro <= 5 && 0.5 <= ratio_hIb && ratio_hIb <= 2)
            {
                
                lambda_0 = 5.1 * ratio_bIt - Math.Pow(ro, 1.5) * Math.Pow(ratio_bIt, 0.5);
                s = ratio_lambda_zIlambda_0;
                X = 1.1 - 0.3* ratio_hIb;
            }
            }
            #endregion
            #region 913
            else if (cs_shape == 913)
            {
                // 913 - 9 - double L with extensions
                // Symmetry
                symmetry_yy = false;
                symmetry_zz = true;
                symmetry_uu = false;
                symmetry_vv = true;
                // LT factors

                ro = ratio_rIt;
                if (distance_L_yy <= 2 * t && ro <= 5 && 0.5 <= ratio_hIb && ratio_hIb <= 2 && 1 <= delta && delta <= 2.5)
                {

                    lambda_0 = (4.4 + 1.1 * Math.Pow(ratio_bIh,2)) * ratio_bIt - 0.7 * Math.Pow(ro, 1.5) * Math.Pow(ratio_bIt, 0.5) + (1.5 * ro * (delta - 1)) - 2 * Math.Pow(delta -1,3);
                    s = ratio_lambda_zIlambda_0;
                    X = 1.1 - 0.3 * ratio_hIb;
                }
            }
            #endregion
            #region 914
            else if (cs_shape == 914)
            {
                // 914 - 10 - double L with extensions
                // Symmetry
                symmetry_yy = false;
                symmetry_zz = true;
                symmetry_uu = false;
                symmetry_vv = true;
                // LT factors
                ro = ratio_16zIt;
                if (distance_L_yy <= 2 * t && ratio_bIt == 20 && ratio_riIt == 2 && delta == 3 && beta == 4)
                {

                    lambda_0 = 70;
                    s = ratio_lambda_zIlambda_0;
                    X = 0.83;
                }
            }
            #endregion
            #region 915
            else if (cs_shape == 915)
            {
                // 915 - 11 - double L with extensions
                // Symmetry
                symmetry_yy = false;
                symmetry_zz = true;
                symmetry_uu = false;
                symmetry_vv = true;
                // LT factors
                ro = ratio_rIt;
                if (distance_L_yy <= 2 * t && ratio_hIt == 20 && ratio_bIt == 15 && ratio_riIt == 2 && delta == 3 && beta == 4)
                {

                    lambda_0 = 60;
                    s = ratio_lambda_zIlambda_0;
                    X = 0.76;
                }
            }
            #endregion
            #region 916
            else if (cs_shape == 916)
            {
                // 916 - 12 - double L with extensions
                // Symmetry
                symmetry_yy = false;
                symmetry_zz = true;
                symmetry_uu = false;
                symmetry_vv = true;
                // LT factors
                ro = ratio_rIt;
                if (distance_L_yy <= 2 * t && ratio_hIt == 20 && ratio_bIt == 15 && ratio_riIt == 2 && delta == 3 && beta == 4)
                {

                    lambda_0 = 63;
                    s = ratio_lambda_zIlambda_0;
                    X = 0.89;
                }
            }
            #endregion
            #region 917
            else if (cs_shape == 917)
            {
                // 917 - 13 - T symmetrical without extensions
                // Symmetry
                symmetry_yy = false;
                symmetry_zz = true;
                symmetry_uu = false;
                symmetry_vv = true;

                _Iw = (Math.Pow(hf, 3) * Math.Pow(t1, 3))/36 + 2*(Math.Pow(b_axial/2, 3) * Math.Pow(t2, 3)) / 36; // Picture J.2 (2)
                // LT factors
                ro = ratio_rIt;
                if (ro <= 3.5 && 0.5 <= ratio_hIb && ratio_hIb <= 2)
                {

                    lambda_0 = (1.4 + 1.5 * ratio_bIh + 1.1 * ratio_hIb) * ratio_hIt - (Math.Pow(ro, 1.5) * Math.Pow(ratio_hIt, 0.5));
                    s = ratio_lambda_zIlambda_0;
                    X = 1.3 - 0.8 * ratio_hIb + 0.2 * Math.Pow(ratio_hIb, 2);
                }
            }
            #endregion
            #region 918
            else if (cs_shape == 918)
            {
                // 918 - 14 - T symmetrical with extensions
                // Symmetry
                symmetry_yy = false;
                symmetry_zz = true;
                symmetry_uu = false;
                symmetry_vv = true;

                _Iw = (Math.Pow(hf, 3) * Math.Pow(t1, 3)) / 36 + 2 * (Math.Pow(b_axial / 2, 3) * Math.Pow(t2, 3)) / 36; // Picture J.2 (2)
                // LT factors
                ro = ratio_rIt; // ???????????????????????????????????? 
                if (ratio_hIt == 25 && ratio_bIh == 1.2 && ratio_riIt == 0.5)
                {

                    lambda_0 = 0.65;
                    s = ratio_lambda_zIlambda_0;
                    X = 0.78;
                }
            }
            #endregion
            #region 919
            else if (cs_shape == 919)
            {
                // 919 - 15 - U symmetrical without extensions
                // Symmetry
                symmetry_yy = true;
                symmetry_zz = false;
                symmetry_uu = true;
                symmetry_vv = false;

                e = 3 * b2 * t2 / (hf * t1 + (6 * b_axial * t2));
                _Iw = (hf * hf * Math.Pow(b_axial, 3) * t2 / 12) * ((2 * hf * t1) + (3 * b_axial * t2)) / (hf * t1 + 6 * b_axial * t2); // Picture J.2 (3)
                // LT factors
                ro = ratio_rIt;
                if (1 <= ratio_hIb && ratio_hIb <= 3 && 1 <= ratio_t2It1 && ratio_t2It1 <= 2)
                {

                    lambda_0 = ratio_bIt2 * (7+1.5 * ratio_hIb * ratio_t2It1);
                    s = ratio_lambda_yIlambda_t;
                    X = 0.38 * ratio_hIb - (0.04 * Math.Pow(ratio_hIb, 2));
                    Y = 0.14 - 0.02 * ratio_hIb - 0.02 * ratio_t2It1;
                }
            }        
            #endregion
            #region 920
            else if (cs_shape == 920)
            {
                // 920 - 16 - U symmetrical with longitudinal stiffeners (c)
                // Symmetry
                symmetry_yy = true;
                symmetry_zz = false;
                symmetry_uu = true;
                symmetry_vv = false;

                e = (hf * hf * b_axial * b_axial * t / _Iy) * (1 / 4 + (c / 2 * b_axial) - (2 * Math.Pow(c, 3) / (3 * hf * hf * b_axial))); // Picture J.2 (4)
                _Iw = b_axial * b_axial * t / 6 * ((4 * Math.Pow(c, 3)) + 6 * hf * c * c + 3 * hf * hf * c + hf * hf * b_axial) - e * e * _Iy;
                // LT factors
                ro = ratio_rIt;
                if (1 <= ratio_hIb && ratio_hIb <= 3 && ratio_cIb <= 0.4)
                {

                    lambda_0 = ratio_bIt * (7 + 1.5 * ratio_hIb + 5* ratio_cIb);
                    s = ratio_lambda_yIlambda_t;
                    X = 0.38 * ratio_hIb - (0.04 * Math.Pow(ratio_hIb, 2)) - 0.25*ratio_cIb;
                    Y = 0.12 - 0.02 * ratio_hIb + ((0.6*Math.Pow(ratio_cIb,2))/(ratio_hIb -0.5));
                }
            }
            #endregion
            #region 921
            else if (cs_shape == 921)
            {
                // 921 - 17 - U symmetrical with longitudinal upper stiffeners (c)
                // Symmetry
                symmetry_yy = true;
                symmetry_zz = false;
                symmetry_uu = true;
                symmetry_vv = false;

                e = (hf * hf * b_axial * b_axial * t / _Iy) * (1 / 4 + (c / 2 * b_axial) - (2 * Math.Pow(c, 3) / (3 * hf * hf * b_axial))); // Picture J.2 (6)
                _Iw = b_axial * b_axial * t / 6 * ((4 * Math.Pow(c, 3)) - 6 * hf * c * c + 3 * hf * hf * c + hf * hf * b_axial) - e * e * _Iy;
                // LT factors
                ro = ratio_rIt;
                if (1 <= ratio_hIb && ratio_hIb <= 3 && ratio_cIb <= 0.4)
                {

                    lambda_0 = ratio_bIt * (7 + 1.5 * ratio_hIb + 5 * ratio_cIb);
                    s = ratio_lambda_yIlambda_t;
                    X = 0.38 * ratio_hIb - (0.04 * Math.Pow(ratio_hIb, 2));
                    Y = 0.12 - 0.02 * ratio_hIb - ((0.05 * ratio_cIb) / (ratio_hIb - 0.5));
                }
            }
            #endregion
            #region 922
            else if (cs_shape == 922)
            {
                // 922 - 18 - U symmetrical with extensions 16z/t
                // Symmetry
                symmetry_yy = true;
                symmetry_zz = false;
                symmetry_uu = true;
                symmetry_vv = false;

                e = 3 * b2 * t2 / (hf * t1 + (6 * b_axial * t2));
                _Iw = (hf * hf * Math.Pow(b_axial, 3) * t2 / 12) * ((2 * hf * t1) + (3 * b_axial * t2)) / (hf * t1 + 6 * b_axial * t2); // Picture J.2 (3)
                // LT factors
                ro = ratio_16zIt;
                if (ratio_hIt ==32 && ratio_bIh ==0.5 && ratio_riIt == 2)
                {

                    lambda_0 = 126;
                    s = ratio_lambda_yIlambda_t;
                    X = 0.59;
                    Y = 0.104;
                }
            }
            #endregion














            // NEXT STEP IS TO REPAIR conditions for  901-911 + Table / picture J.2 - e and Iw calculation

































         }






























    }
}
