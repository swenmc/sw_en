using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.OleDb;
using DATABASE;
using BaseClasses;

namespace MATERIAL
{
    // Default concrete material class
    public class CMat_02_00:CMat
    {
        // Default - concrete
        // General material properties

        #region Variables from CONCRETE-SHEET
        // Material properties
        int mat_num;
        string mat_name;
        int mat_prod_code_num;
        string mat_prod_code_name1;
        string mat_prod_code_name2;
        int concrete_grade_num;

        // Concrete – characteristic strength
        double fck;

        public double Fck
        {
            get { return fck; }
            set { fck = value; }
        }

        double fck_cube;

        public double Fck_cube
        {
            get { return fck_cube; }
            set { fck_cube = value; }
        }

        double fctm;

        public double Fctm
        {
            get { return fctm; }
            set { fctm = value; }
        }

        double fcm;

        public double Fcm
        {
            get { return fcm; }
            set { fcm = value; }
        }

        double fctk0_05;

        public double Fctk0_05
        {
            get { return fctk0_05; }
            set { fctk0_05 = value; }
        }

        double fctk0_95;

        public double Fctk0_95
        {
            get { return fctk0_95; }
            set { fctk0_95 = value; }
        }

        double epsilon_c1;
        double epsilon_cu1;
        double epsilon_c2;
        double epsilon_cu2;
        double epsilon_c3;
        double epsilon_cu3;
        double d_n;
        // partial factor
        double d_gamaMc;
        public double D_gamaMc
        {
            get { return d_gamaMc; }
            set { d_gamaMc = value; }
        }

        // Concrete – Youngs modulus
        double d_Ecm;

        public double D_Ecm
        {
            get { return d_Ecm; }
            set { d_Ecm = value; }
        }
        double d_Ec;

        public double D_Ec
        {
            get { return d_Ec; }
            set { d_Ec = value; }
        }
        // Steel – Shear modulus
        double d_G;

        public double D_G
        {
            get { return d_G; }
            set { d_G = value; }
        }
        // Concrete – Poisson constant
        double d_nu_pois;

        public double D_nu_pois
        {
            get { return d_nu_pois; }
            set { d_nu_pois = value; }
        }

        //

        double d_alpha_temp;

        public double D_alpha_temp
        {
            get { return d_alpha_temp; }
            set { d_alpha_temp = value; }
        }

        #endregion




        #region Constructor
        OleDbDataReader dat_reader;
        DatabaseConnection dat_conn;

        public CMat_02_00()
        {
        }

        public CMat_02_00(string combo1)
        {
            m_sMatType = 2;

            string sql1;

            sql1 = "Select mat_num, mat_name, mat_prod_code_num, mat_prod_code_name1, mat_prod_code_name2, concrete_grade_num, fck, fck_cube, fctm, fcm, fctk0_05, fctk0_95, epsilon_c1, epsilon_cu1, epsilon_c2, epsilon_cu2, epsilon_c3, epsilon_cu3, n, gamaMc, Ecm, Ec, G, nu_pois, alpha_temp from Concrete where mat_name like '" + combo1 + "'";

            // Database DATA-CONCRETE variables reader
            dat_conn = DatabaseConnection.getInstance();
            dat_reader = dat_conn.getDBReader(sql1);

            try
            {
                while (dat_reader.Read())
                {
                    // CONCRETE DATABASE DATA

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
                        concrete_grade_num = Convert.ToInt32(dat_reader.GetValue(5).ToString());
                    }
                    catch (FormatException) { }
                    try
                    {
                        fck = Convert.ToDouble(dat_reader.GetValue(6).ToString());
                    }
                    catch (FormatException) { }
                    try
                    {
                        fck_cube = Convert.ToDouble(dat_reader.GetValue(7).ToString());
                    }
                    catch (FormatException) { }
                    try
                    {
                        fctm = Convert.ToDouble(dat_reader.GetValue(8).ToString());
                    }
                    catch (FormatException) { }
                    try
                    {
                        fcm = Convert.ToDouble(dat_reader.GetValue(9).ToString());
                    }
                    catch (FormatException) { }
                    try
                    {
                        fctk0_05 = Convert.ToDouble(dat_reader.GetValue(10).ToString());
                    }
                    catch (FormatException) { }
                    try
                    {
                        fctk0_95 = Convert.ToDouble(dat_reader.GetValue(11).ToString());
                    }
                    catch (FormatException) { }
                    try
                    {
                        epsilon_c1 = Convert.ToDouble(dat_reader.GetValue(12).ToString());
                    }
                    catch (FormatException) { }
                    try
                    {
                        epsilon_cu1 = Convert.ToDouble(dat_reader.GetValue(13).ToString());
                    }
                    catch (FormatException) { }
                    try
                    {
                        epsilon_c2 = Convert.ToDouble(dat_reader.GetValue(14).ToString());
                    }
                    catch (FormatException) { }
                    try
                    {
                        epsilon_cu2 = Convert.ToDouble(dat_reader.GetValue(15).ToString());
                    }
                    catch (FormatException) { }
                    try
                    {
                        epsilon_c3 = Convert.ToDouble(dat_reader.GetValue(16).ToString());
                    }
                    catch (FormatException) { }
                    try
                    {
                        epsilon_cu3 = Convert.ToDouble(dat_reader.GetValue(17).ToString());
                    }
                    catch (FormatException) { }
                    try
                    {
                        d_n = Convert.ToDouble(dat_reader.GetValue(18).ToString());
                    }
                    catch (FormatException) { }
                    try
                    {
                        d_gamaMc = Convert.ToDouble(dat_reader.GetValue(19).ToString());
                    }
                    catch (FormatException) { }
                    try
                    {
                        d_Ecm = Convert.ToDouble(dat_reader.GetValue(20).ToString());
                    }
                    catch (FormatException) { }
                    try
                    {
                        d_Ec = Convert.ToDouble(dat_reader.GetValue(21).ToString());
                    }
                    catch (FormatException) { }
                    try
                    {
                        d_G = Convert.ToDouble(dat_reader.GetValue(22).ToString());
                    }
                    catch (FormatException) { }
                    try
                    {
                        d_nu_pois = Convert.ToDouble(dat_reader.GetValue(23).ToString());
                    }
                    catch (FormatException) { }
                    try
                    {
                        d_alpha_temp = Convert.ToDouble(dat_reader.GetValue(24).ToString());
                    }
                    catch (FormatException) { }

                    #endregion

                }
            }
            catch (FormatException) { }

        }


        #endregion











    }
}
