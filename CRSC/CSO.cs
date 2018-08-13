using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Media;
using System.Windows.Forms;
using MATH;
using BaseClasses;

namespace CRSC
{
    // THIN-WALLED OPENED CROSS-SECTION PROPERTIES CALCULATION
    // Opened cross section characteristic calculation (closed cell are not allowed)

    public class CSO : CCrSc_TW
    {
        // CONSTRUCTOR

        public CSO() { }
        public CSO(List<double> y_suradnice, List<double> z_suradnice, List<double> t_hodnoty)
        {
            CalculateSectionProperties(y_suradnice, z_suradnice, t_hodnoty);
        }

        public void CrScDefPoints_EX_01()
        {
            // Fill example data
            IsShapeSolid = true;
            const int number_rows = 9;

            arrPointCoord = new float[number_rows, 3] {
            {-8.0f,  17.0f,  0.0f},
            {-6.0f,  20.0f,  1.0f},
            { 6.0f,  20.0f,  1.0f},
            { 8.0f,  17.0f,  1.0f},
            { 6.0f,  20.0f,  0.0f},
            { 0.0f,  20.0f,  0.0f},
            { 0.0f,   0.0f,  0.8f},
            { 6.0f,   0.0f,  0.0f},
            {-6.0f,   0.0f,  1.0f}
            };
        }

        public void CrScDefPoints_EX_02()
        {
            // Fill example data - Duragal C300x90x8.0

            const int number_rows = 6;

            float fh = 300f;
            float fb = 90f;
            float ft = 8f;
            float fr_1 = 8f;

            arrPointCoord = new float[number_rows, 3] {
            { fb - 0.5f * ft,  -0.5f * fh + 0.5f * ft,  0.0f}, //0
            { 0.5f * ft + fr_1,  -0.5f * fh + 0.5f * ft,  ft}, //1
            { 0, -0.5f * fh + 0.5f * ft  + fr_1,  ft}, //2
            { 0, 0,  ft}, //3
            { 0, 0,  ft}, //4
            { 0, 0,  ft}, //5
            };

            // Fill coordinates (symmetry about horizontal y-y axis)

            for (int i = number_rows / 2; i < number_rows; i++)
            {
                arrPointCoord[i, 0] = arrPointCoord[number_rows - i - 1, 0];
                arrPointCoord[i, 1] = -arrPointCoord[number_rows - i - 1, 1];
            }
        }

        public void CrScDefPoints_EX_2710095()
        {
            // Fill example data
            IsShapeSolid = true;
            const int number_rows = 28;

            float fh = 270f;
            float fb = 70f;
            float ft = 0.95f;
            float fr_1 = 4f;
            float fr_2 = 8f;
            float fr_3 = 5f;

            float fy_1 = 10f; // flange edge stiffener
            float fy_2 = 25f;
            //float fy_3 = 20f;
            //float fy_4 = 25f;

            float fy_5 = 10f; // web stiffener

            float fz_1 = 20f; // flange edge stiffener
            //float fz_2 = 50f;
            float fz_3 = 20f; // web stiffener
            float fz_4 = 15f; // web stiffener
            float fz_5 = 20f; // web stiffener
            float fz_6 = 60f;

            arrPointCoord = new float[number_rows, 3] {
            { fb - fy_1 - ft,  -0.5f * fh + fz_1 - 0.5f * ft,  0.0f}, //0
            { fb - ft - fr_1,  -0.5f * fh + fz_1 - 0.5f * ft,  ft}, //1
            { fb - ft, -0.5f * fh + fz_1 - 0.5f * ft - fr_3,  ft}, //2
            { fb - ft, -0.5f * fh + 0.5f * ft + fr_1,  ft}, //3
            { fb - ft - fr_1,  -0.5f * fh + 0.5f * ft,  ft}, //4
            { fb - 0.5f * ft - fy_2, -0.5f * fh + 0.5f * ft,  ft}, //5
            { 0.5f * (fb - ft), -0.5f * fh + 0.5f * ft + fr_1,  ft}, //6
            { fy_2 - 0.5f * ft, -0.5f * fh + 0.5f * ft,  ft},
            { fr_2 + 0.5f * ft, -0.5f * fh + 0.5f * ft,  ft},
            { 0f,  -0.5f * fh + 0.5f * ft + fr_2,  ft}, //9
            { 0f,  - 0.5f * fz_6 - fz_5 - fz_4 - fz_3,  ft}, //10
            { fy_5 - ft,  - 0.5f * fz_6 - fz_5 - fz_4,  ft},
            { fy_5 - ft,  - 0.5f * fz_6 - fz_5,  ft},
            { 0f,  - 0.5f * fz_6,  ft}, //13
            { 0f,  0f,  ft}, //14
            { 0f,  0f,  ft},
            { 0f,  0f,  ft},
            { 0f,  0f,  ft}, //17
            { 0f,  0f,  ft}, //18
            { 0f,  0f,  ft},
            { 0f,  0f,  ft},
            { 0f,  0f,  ft}, //21
            { 0f,  0f,  ft}, //22
            { 0f,  0f,  ft},
            { 0f,  0f,  ft},
            { 0f,  0f,  ft}, //25
            { 0f,  0f,  ft}, //26
            {-0f,  0f,  ft}  //27
            };

            // Fill coordinates (symmetry about horizontal y-y axis)

            for (int i = number_rows / 2; i < number_rows; i++)
            {
                arrPointCoord[i, 0] = arrPointCoord[number_rows - i - 1, 0];
                arrPointCoord[i, 1] = -arrPointCoord[number_rows - i - 1, 1];
            }
        }

        public void CrScDefPoints_EX_2710115()
        {
            // Fill example data
            IsShapeSolid = true;
            const int number_rows = 28;

            float fh = 270f;
            float fb = 70f;
            float ft = 1.15f;
            float fr_1 = 4f;
            float fr_2 = 8f;
            float fr_3 = 5f;

            float fy_1 = 10f; // flange edge stiffener
            float fy_2 = 25f;
            //float fy_3 = 20f;
            //float fy_4 = 25f;

            float fy_5 = 10f; // web stiffener

            float fz_1 = 20f; // flange edge stiffener
            //float fz_2 = 50f;
            float fz_3 = 20f; // web stiffener
            float fz_4 = 15f; // web stiffener
            float fz_5 = 20f; // web stiffener
            float fz_6 = 60f;

            arrPointCoord = new float[number_rows, 3] {
            { fb - fy_1 - ft,  -0.5f * fh + fz_1 - 0.5f * ft,  0.0f}, //0
            { fb - ft - fr_1,  -0.5f * fh + fz_1 - 0.5f * ft,  ft}, //1
            { fb - ft, -0.5f * fh + fz_1 - 0.5f * ft - fr_3,  ft}, //2
            { fb - ft, -0.5f * fh + 0.5f * ft + fr_1,  ft}, //3
            { fb - ft - fr_1,  -0.5f * fh + 0.5f * ft,  ft}, //4
            { fb - 0.5f * ft - fy_2, -0.5f * fh + 0.5f * ft,  ft}, //5
            { 0.5f * (fb - ft), -0.5f * fh + 0.5f * ft + fr_1,  ft}, //6
            { fy_2 - 0.5f * ft, -0.5f * fh + 0.5f * ft,  ft},
            { fr_2 + 0.5f * ft, -0.5f * fh + 0.5f * ft,  ft},
            { 0f,  -0.5f * fh + 0.5f * ft + fr_2,  ft}, //9
            { 0f,  - 0.5f * fz_6 - fz_5 - fz_4 - fz_3,  ft}, //10
            { fy_5 - ft,  - 0.5f * fz_6 - fz_5 - fz_4,  ft},
            { fy_5 - ft,  - 0.5f * fz_6 - fz_5,  ft},
            { 0f,  - 0.5f * fz_6,  ft}, //13
            { 0f,  0f,  ft}, //14
            { 0f,  0f,  ft},
            { 0f,  0f,  ft},
            { 0f,  0f,  ft}, //17
            { 0f,  0f,  ft}, //18
            { 0f,  0f,  ft},
            { 0f,  0f,  ft},
            { 0f,  0f,  ft}, //21
            { 0f,  0f,  ft}, //22
            { 0f,  0f,  ft},
            { 0f,  0f,  ft},
            { 0f,  0f,  ft}, //25
            { 0f,  0f,  ft}, //26
            {-0f,  0f,  ft}  //27
            };

            // Fill coordinates (symmetry about horizontal y-y axis)

            for (int i = number_rows / 2; i < number_rows; i++)
            {
                arrPointCoord[i, 0] = arrPointCoord[number_rows - i - 1, 0];
                arrPointCoord[i, 1] = -arrPointCoord[number_rows - i - 1, 1];
            }
        }

        // Methods for calculations...

        public override void CalculateSectionProperties()
        {
            CalculateSectionProperties(this.y_suradnice, this.z_suradnice, this.t_hodnoty);
        }

        public void CalculateSectionProperties(List<double> y_suradnice, List<double> z_suradnice, List<double> t_hodnoty)
        {
            int count = y_suradnice.Count;

            this.y_suradnice = y_suradnice;
            this.z_suradnice = z_suradnice;
            this.t_hodnoty = t_hodnoty;

            // Calculate basic dimensions of cross-section
            J_Calc_Dimensions();

            A_g = this.A_method(count);

            // TODO Elastic and Plastic Shear Areas
            // See https://www.dlubal.com/-/media/976FCA1B8CEC499A9C03DE4F06E4E32F.ashx
            // page 94, 95, 103, 105
            A_vy = this.A_vy_method(count);
            A_vz = this.A_vz_method(count);
            this.Sy0_Sz0_method(count);
            this.Iy0_Iz0_method(count);
            this.omega0i = new double[count];
            this.omega = new double[count];
            this.d_omega_s = new double[count];
            this.J_12_13_14_method();
            this.J_15_method(count);
            this.J_16_method(count);
            this.J_17_18_19_method(count);
            this.J_20_21_method();
            this.J_22_method(count);
            this.J_23_method(count);
            this.J_24_25_26_method();
            this.J_27_J_28_method(count);
            this.J_W_el();
            this.W_pl_temporary();
            this.Calc_Radius_of_Gyration();
            this.Calc_Beta_y_method(count);
            this.Calc_Beta_z_method(count);
            t_min = this.t_min_method();
            t_max = this.t_max_method();
        }
        //(J.5) method
        public double dAi_method(int i)
        {
            double dAi = t_hodnoty[i] * Math.Sqrt(Math.Pow(y_suradnice[i] - y_suradnice[i - 1], 2)
                            + Math.Pow(z_suradnice[i] - z_suradnice[i - 1], 2));
            return dAi;
        }
        //(J.6) method
        public double A_method(int count)
        {
            double sum = 0;
            for (int i = 1; i < count; i++)
            {
                sum += dAi_method(i);
            }
            return sum;
        }
        // Shear Area Y
        //(sum of all parts paralel to y-Axis and)
        public double A_vy_method(int count)
        {
            double sum = 0;
            for (int i = 1; i < count; i++)
            {
                sum += dAi_method(i);
            }
            double d_A_vy = sum / 2;
            return d_A_vy;
        }
        // Shear Area Z
        //(sum of all parts paralel to z-Axis and)
        public double A_vz_method(int count)
        {
            double sum = 0;
            for (int i = 1; i < count; i++)
            {
                sum += dAi_method(i);
            }
            double d_A_vz = sum / 2;
            return d_A_vz;
        }
        //(J.7) and (J.9) method
        public void Sy0_Sz0_method(int count)
        {
            S_z0 = 0;
            S_y0 = 0;
            double dAi = 0;
            for (int i = 1; i < count; i++)
            {
                dAi = dAi_method(i) / 2;
                S_y0 += (z_suradnice[i] + z_suradnice[i - 1]) * dAi;
                S_z0 += (y_suradnice[i] + y_suradnice[i - 1]) * dAi;
            }

            D_z_gc = S_y0 / A_g;
            D_y_gc = S_z0 / A_g;
        }
        //(J.8) and (J.10) , (J.11) method
        public void Iy0_Iz0_method(int count)
        {
            I_y0 = 0;
            I_z0 = 0;
            I_yz0 = 0;
            double dAi = 0;
            for (int i = 1; i < count; i++)
            {
                dAi = dAi_method(i);
                I_y0 += (Math.Pow(z_suradnice[i], 2) + Math.Pow(z_suradnice[i - 1], 2) + z_suradnice[i] * z_suradnice[i - 1])
                        * (dAi / 3);
                I_z0 += (Math.Pow(y_suradnice[i], 2) + Math.Pow(y_suradnice[i - 1], 2) + y_suradnice[i] * y_suradnice[i - 1])
                        * (dAi / 3);
                I_yz0 += (2 * y_suradnice[i - 1] * z_suradnice[i - 1] + 2 * y_suradnice[i] * z_suradnice[i]
                        + y_suradnice[i - 1] * z_suradnice[i] + y_suradnice[i] * z_suradnice[i - 1]) * dAi / 6;
            }

            I_y = I_y0 - A_g * Math.Pow(D_z_gc, 2);
            I_z = I_z0 - A_g * Math.Pow(D_y_gc, 2);
            I_yz = I_yz0 - (S_y0 * S_z0 / A_g);
        }
        //J.12,J.13,J.14 method
        public void J_12_13_14_method()
        {
            if ((I_z - I_y) != 0)
                Alpha_rad = Math.Atan(2 * I_yz / (I_z - I_y)) / 2;
            else Alpha_rad = 0;
            double temp = Math.Sqrt(Math.Pow(I_z - I_y, 2) + 4 * Math.Pow(I_yz, 2));
            this.I_epsilon = 0.5 * (I_y + I_z + temp);
            this.I_mikro = 0.5 * (I_y + I_z - temp);
        }
        //J.15 method
        public void J_15_method(int count)
        {
            omega0i[0] = 0;
            omega[0] = 0;
            for (int i = 1; i < count; i++)
            {
                omega0i[i] = y_suradnice[i - 1] * z_suradnice[i] - y_suradnice[i] * z_suradnice[i - 1];
                omega[i] = omega[i - 1] + omega0i[i];
            }
        }
        //J.16 method
        public void J_16_method(int count)
        {
            _Iomega = 0;
            for (int i = 1; i < count; i++)
            {
                _Iomega += (omega[i - 1] + omega[i]) * dAi_method(i) / 2;
            }
            _omega_mean = _Iomega / A_g;
        }
        //J.17,J18,J19 method 
        public void J_17_18_19_method(int count)
        {
            _Iy_omega0 = 0;
            _Iz_omega0 = 0;
            _Iomega_omega0 = 0;

            for (int i = 1; i < count; i++)
            {
                double dAi = dAi_method(i);
                _Iy_omega0 += (2 * y_suradnice[i - 1] * omega[i - 1] +
                               2 * y_suradnice[i] * omega[i] +
                               y_suradnice[i - 1] * omega[i] +
                               y_suradnice[i] * omega[i - 1]) * dAi / 6;
                _Iz_omega0 += (2 * omega[i - 1] * z_suradnice[i - 1] +
                               2 * omega[i] * z_suradnice[i] +
                               omega[i - 1] * z_suradnice[i] +
                               omega[i] * z_suradnice[i - 1]) * dAi / 6;
                _Iomega_omega0 += (Math.Pow(omega[i], 2) + Math.Pow(omega[i - 1], 2) + omega[i] * omega[i - 1]) * dAi / 3;
            }
            _Iy_omega = _Iy_omega0 - (S_z0 * _Iomega / A_g);
            _Iz_omega = _Iz_omega0 - (S_y0 * _Iomega / A_g);
            _Iomega_omega = _Iomega_omega0 - (Math.Pow(_Iomega, 2) / A_g);
        }
        //J.20 and J.21 method
        public void J_20_21_method()
        {
            try
            {
                double temp = I_y * I_z - Math.Pow(I_yz, 2);
                D_y_sc = (_Iz_omega * I_z - _Iy_omega * I_yz) / temp;
                D_z_sc = (-_Iy_omega * I_y - _Iz_omega * I_yz) / temp;
                I_w = _Iomega_omega + D_z_sc * _Iy_omega - D_y_sc * _Iz_omega;
            }
            catch (DivideByZeroException) { MessageBox.Show("ERROR. Divide by zero, J.20 method."); }
        }
        //J.22 method
        public void J_22_method(int count)
        {
            I_t = 0;
            for (int i = 1; i < count; i++)
            {
                I_t += dAi_method(i) * Math.Pow(t_hodnoty[i], 2) / 3;

                // For rectangle see https://www.colorado.edu/engineering/CAS/courses.d/Structures.d/IAST.Lect08.d/IAST.Lect08.pdf
                // Page 8-6
            }

            if (MathF.Min(t_hodnoty) != 0) // Existuje minimum rozne od nuly
                W_t_el = I_t / MathF.Min(t_hodnoty); // Pre nenulovu hrubku
            else if (MathF.Max(t_hodnoty) != 0) // Existuje maximum rozne od nuly
            {
                double min_more_than_zero, max;

                max = t_hodnoty[0]; // Set first item
                foreach (double num in t_hodnoty)
                    if (num > max) max = num; // Set new maximum

                min_more_than_zero = max; // Set minimum to maximum
                foreach (double num in t_hodnoty)
                    if (num != 0 && num < min_more_than_zero) min_more_than_zero = num; // Set non zero minimum

                W_t_el = I_t / min_more_than_zero;
            }
            else
                MessageBox.Show("ERROR. Minimum value of thickness is zero in the array of thickness values!");
        }
        //J.23 method   ????? nerozumiem vzorcu...je potrebne upresnit
        public void J_23_method(int count)
        {
            d_omega_s[0] = 0;
            for (int i = 1; i < count; i++)
            {
                d_omega_s[i] = omega[i] - _omega_mean + D_z_sc * (y_suradnice[i] - D_y_gc) - D_y_sc * (z_suradnice[i] - D_z_gc);
            }
        }
        //J.24,J.25,J.26 method
        public void J_24_25_26_method()
        {
            omega_max = MathF.Max(d_omega_s);
            W_w = I_w / omega_max;
            D_y_s = D_y_sc - D_y_gc;
            D_z_s = D_z_sc - D_z_gc;
            _Ip = I_y + I_z + A_g * (Math.Pow(D_y_s, 2) + Math.Pow(D_z_s, 2));

        }
        //J.29 method
        public void J_29_method(int num)
        {
            d_y_ci = (y_suradnice[num] + y_suradnice[num - 1]) / 2 - D_y_gc;
            d_z_ci = (z_suradnice[num] + z_suradnice[num - 1]) / 2 - D_z_gc;
        }
        //J.27,J.28 method
        //This method uses J.29 method to count actual d_y_ci and d_z_ci numbers
        public void J_27_J_28_method(int count)
        {
            double zj_temp = 0, yj_temp = 0, dAi;
            for (int i = 1; i < count; i++)
            {
                this.J_29_method(i);
                dAi = this.dAi_method(i);
                zj_temp += (Math.Pow(d_z_ci, 3) + d_z_ci * (Math.Pow(z_suradnice[i] - z_suradnice[i - 1], 2) / 4 + Math.Pow(d_y_ci, 2) +
                        Math.Pow(y_suradnice[i] - y_suradnice[i - 1], 2) / 12) + d_y_ci * (y_suradnice[i] - y_suradnice[i - 1]) *
                        (z_suradnice[i] - z_suradnice[i - 1]) / 6) * dAi;
                yj_temp += (Math.Pow(d_y_ci, 3) + d_y_ci * (Math.Pow(y_suradnice[i] - y_suradnice[i - 1], 2) / 4 + Math.Pow(d_z_ci, 2) +
                        Math.Pow(z_suradnice[i] - z_suradnice[i - 1], 2) / 12) + d_z_ci * (z_suradnice[i] - z_suradnice[i - 1]) *
                        (y_suradnice[i] - y_suradnice[i - 1]) / 6) * dAi;
            }
            d_z_j = D_z_s - (0.5 / I_y) * zj_temp;
            d_y_j = D_y_s - (0.5 / I_z) * yj_temp;
        }
        // Calculate dimensions
        public void J_Calc_Dimensions()
        {
            y_min = y_suradnice[0];
            y_max = y_suradnice[0];
            z_min = z_suradnice[0];
            z_max = z_suradnice[0];

            foreach (double num in y_suradnice)
            {
                if (num > y_max) y_max = num; // Set new maximum
                if (num < y_min) y_min = num; // Set new minimum
            }

            foreach (double num in z_suradnice)
            {
                if (num > z_max) z_max = num; // Set new maximum
                if (num < z_min) z_min = num; // Set new minimum
            }

            b = Math.Abs(y_max - y_min);
            h = Math.Abs(z_max - z_min);

        }
        // Calculate elastic cross-section moduli
        public void J_W_el()
        {
            W_y_el_1 = I_y / (z_max - D_z_gc);
            W_y_el_2 = I_y / (D_z_gc - z_min);

            W_z_el_1 = I_z / (y_max - D_y_gc);
            W_z_el_2 = I_z / (D_y_gc - y_min);

            // Minimum absolute value
            W_y_el = MathF.Min(Math.Abs(W_y_el_1), Math.Abs(W_y_el_2));
            W_z_el = MathF.Min(Math.Abs(W_z_el_1), Math.Abs(W_z_el_2));
        }
        public void W_pl_temporary()
        {
            // TODO - Wpl je urceny sucinom tlacenej plochy prierezu * vzdialenost k neutralnej osi + tahana plocha prierezu * vzdialnost taziska tejto plochy a neutralnej osi
            // neutralna osa rozdeluje prierez na polovicu
            // Wpl je vzdy vacsi nez W_el, niekedy je pre design pruta limitovany zhora hodnotou 1.5 * W_el

            // See for example https://github.com/robbievanleeuwen/section-properties
            // https://www.dlubal.com/-/media/976FCA1B8CEC499A9C03DE4F06E4E32F.ashx
            // page 102 - Plastic Section Moduli Zy / Zz / Zu / Zv

            // W_pl = AC * dC + AT * dT

            float fpl_tempoerary = 1.05f;

            W_y_pl = fpl_tempoerary * W_y_el;
            W_z_pl = fpl_tempoerary * W_y_pl;
        }
        // Radius of gyration
        public void Calc_Radius_of_Gyration()
        {
            i_y_rg = MathF.Sqrt(I_y / A_g);
            i_z_rg = MathF.Sqrt(I_z / A_g);
            i_yz_rg = MathF.Sqrt(I_yz / A_g);
            i_epsilon_rg = MathF.Sqrt(I_epsilon / A_g);
            i_mikro_rg = MathF.Sqrt(I_mikro / A_g);

            i_p_rg = MathF.Sqrt(I_p / A_g);
            //i_p_M_rg_= MathF.Sqrt(I_p_M / A_g);
            //i_Omega_M = MathF.Sqrt(I_Omega_M / A_g);
        }
        // Calculate Monosymmetry section constant Beta y
        public void Calc_Beta_y_method(int count)
        {
            double Beta_y_temp = 0, dAi;
            for (int i = 1; i < count; i++)
            {
                this.J_29_method(i);
                dAi = this.dAi_method(i);

                //Beta_y_temp += (Math.Pow((y_suradnice[i] - d_y_gc) - (y_suradnice[i - 1] - d_y_gc), 2) * ((z_suradnice[i] - d_z_gc) - (z_suradnice[i - 1] - d_z_gc)) + Math.Pow((z_suradnice[i] - d_z_gc) - (z_suradnice[i - 1] - d_z_gc), 3)) * dAi;
                Beta_y_temp += (Math.Pow(d_y_ci, 2) * (d_z_ci) + Math.Pow(d_z_ci, 3)) * dAi;
            }

            Beta_y = (1 / I_y) * Beta_y_temp - 2 * D_z_s;
        }
        // Calculate Monosymmetry section constant Beta z
        public void Calc_Beta_z_method(int count)
        {
            double Beta_z_temp = 0, dAi;
            for (int i = 1; i < count; i++)
            {
                this.J_29_method(i);
                dAi = this.dAi_method(i);

                // Beta_z_temp += (Math.Pow((z_suradnice[i] - d_z_gc) - (z_suradnice[i - 1] - d_z_gc), 2) * ((y_suradnice[i] - d_y_gc) - (y_suradnice[i - 1] - d_y_gc)) + Math.Pow((y_suradnice[i] - d_y_gc) - (y_suradnice[i - 1] - d_y_gc), 3)) * dAi;
                Beta_z_temp += (Math.Pow(d_z_ci, 2) * (d_y_ci) + Math.Pow(d_y_ci, 3)) * dAi;
            }

            Beta_z = (1 / I_z) * Beta_z_temp - 2 * D_y_s;

            //Table E1
            // Pokusny vypocet - c s vystuhami na koncoch
            double a, b, c;

            a = 270 - 0.95;
            b = 70 - 0.95;
            c = 20;
            double t = 0.95;

            double x_ = b * b / (a + 2 * b);
            double x_o = x_ + ((3 * b * b) / (6 * b + a));
            double beta_w = (1 / 12) * t * x_ * a * a * a + t * x_ * x_ * x_ * a;
            double beta_f = (0.5 * t * (Math.Pow(b + x_, 4) - Math.Pow(x_, 4))) + (0.25 * a * a * t * (Math.Pow(b + x_, 2) + x_ * x_));
            double beta_L = 0;
            double m = (a * a * b * b * t) / I_y * (0.25 + c / (2 * b) - (2 * c * c * c) / (3 * a * a * b));
            double beta_yps = ((beta_w + beta_f + beta_L) / I_y) - 2 * x_o;
        }
        // Calculate t_min
        public double t_min_method()
        {
            if (MathF.Min(t_hodnoty) != 0)
                return MathF.Min(t_hodnoty);
            else if (MathF.Max(t_hodnoty) != 0) // Existuje maximum rozne od nuly
            {
                double min_more_than_zero, max;

                max = t_hodnoty[0]; // Set first item
                foreach (double num in t_hodnoty)
                    if (num > max) max = num; // Set new maximum

                min_more_than_zero = max; // Set minimum to maximum
                foreach (double num in t_hodnoty)
                    if (num != 0 && num < min_more_than_zero) min_more_than_zero = num; // Set non zero minimum

                return min_more_than_zero;
            }
            else
            {
                // Error
                return Double.NaN;
            }
        }
        // Calculate t_max
        public double t_max_method()
        {
            return MathF.Max(t_hodnoty);
        }
    }
}
