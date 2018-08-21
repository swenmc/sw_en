using MATH;
using System;
using System.Collections.Generic;

namespace CRSC
{
    // THIN-WALLED CLOSED CROSS-SECTION PROPERTIES CALCULATION

    public class CSC : CSO
    {
        private double _A_t;
        private double _S_t;

        public double A_t
        {
            get { return _A_t; }
            set { _A_t = value; }
        }

        public double S_t
        {
            get { return _S_t; }
            set { _S_t = value; }
        }

        // CONSTRUCTOR

        public CSC() { }
        public CSC(List<double> y_suradnice, List<double> z_suradnice, List<double> t_hodnoty)
        {
            int count = y_suradnice.Count;
            this.J_30_31_32_method(count); // Closed Cross-section
        }

        //J.30-32 method
        public void J_30_31_32_method(int count)
        {
            for (int i = 2; i < count; i++)
            {
                A_t += (y_suradnice[i] - y_suradnice[i - 1]) * (z_suradnice[i] + z_suradnice[i - 1]); // J.31
                S_t += (t_hodnoty[i] < 1e-6) ? 0 : (Math.Sqrt(Math.Pow(y_suradnice[i] - y_suradnice[i - 1],2) * Math.Pow(z_suradnice[i] - z_suradnice[i - 1],2)) / t_hodnoty[i]); // J.32
            }

            A_t *= 0.5;
            A_t = Math.Abs(A_t); //?????? Area should be possitive

            I_t = 4 * Math.Pow(A_t, 2) / S_t; // J.30

            if (MathF.Min(t_hodnoty) != 0) // Existuje minimum rozne od nuly
            {
                W_t_el = 2 * A_t * MathF.Min(t_hodnoty); // Pre nenulovu hrubku
            }
            else if (MathF.Max(t_hodnoty) != 0) // Existuje maximum rozne od nuly
            {
                double min_more_than_zero, max;

                max = t_hodnoty[0]; // Set first item
                foreach (double num in t_hodnoty)
                    if (num > max) max = num; // Set new maximum

                min_more_than_zero = max; // Set minimum to maximum
                foreach (double num in t_hodnoty)
                    if (num != 0 && num < min_more_than_zero) min_more_than_zero = num; // Set non zero minimum

                W_t_el = 2 * A_t * min_more_than_zero;
            }
            //else
            //    MessageBox.Show("ERROR. Minimalny prvok v t_hodnoty je nula!!!!.");
        }

        public void CrScDefPoints_EX_10075()
        {
            // Fill example data
            IsShapeSolid = false;
            const int number_rows = 40;

            float fb = 100f;
            float ft = 0.75f;
            float fr_1 = 6f;
            float fr_2 = 3f;

            float fy_1 = 4.5f; // stiffener
            float fy_2 = 20f;
            float fy_3 = 14f;
            float fy_4 = (fb - 2 * fy_2 - 3 * fy_3) / 3.0f;

            float[,] arrPointCoord_temp = new float[number_rows, 3] {
            { 0.5f * fy_3, -0.5f * fb + 0.5f * ft,  0.0f}, //0
            { 0.5f * fy_3 + fy_4, -0.5f * fb + fy_1 - 0.5f * ft,  ft}, //1
            { 1.5f * fy_3 + fy_4, -0.5f * fb + fy_1 - 0.5f * ft,  ft}, //2
            { 1.5f * fy_3 + 2 * fy_4, -0.5f * fb + 0.5f * ft,  ft}, //3
            { 1.5f * fy_3 + 2 * fy_4 + fy_2 - 0.5f * ft - fr_1, -0.5f * fb + 0.5f * ft,  ft}, //4
            { 0.5f * fb - 0.5f * ft, -(1.5f * fy_3 + 2 * fy_4 + fy_2 - 0.5f * ft - fr_1),  ft}, //5
            { 0.5f * fb - 0.5f * ft, - (1.5f * fy_3 + 2 * fy_4), ft}, //6
            { 0.5f * fb - fy_1 + 0.5f * ft, - (1.5f * fy_3 + fy_4), ft}, //7
            { 0.5f * fb - fy_1 + 0.5f * ft, - (0.5f * fy_3 + fy_4), ft},
            { 0.5f * fb - 0.5f * ft, - 0.5f * fy_3,  ft}, //9
            { 0f,  0f,  ft}, //10
            { 0f,  0f,  ft},
            { 0f,  0f,  ft},
            { 0f,  0f,  ft}, //13
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
            {-0f,  0f,  ft}, //27
            { 0f,  0f,  ft}, //28
            {-0f,  0f,  ft}, //29
            { 0f,  0f,  ft},
            { 0f,  0f,  ft}, //31
            { 0f,  0f,  ft}, //32
            { 0f,  0f,  ft},
            { 0f,  0f,  ft},
            { 0f,  0f,  ft}, //35
            { 0f,  0f,  ft}, //36
            {-0f,  0f,  ft}, //37
            { 0f,  0f,  ft}, //38
            {-0f,  0f,  ft}  //39
            };

            // Fill coordinates (symmetry about horizontal y-y axis)

            for (int i = number_rows / 4; i < number_rows / 2; i++)
            {
                arrPointCoord_temp[i, 0] = arrPointCoord_temp[number_rows / 2 - i - 1, 0];
                arrPointCoord_temp[i, 1] = -arrPointCoord_temp[number_rows / 2 - i - 1, 1];
            }

            // Fill coordinates (symmetry about vertical z-z axis)

            for (int i = number_rows / 2; i < number_rows; i++)
            {
                arrPointCoord_temp[i, 0] = -arrPointCoord_temp[number_rows - i - 1, 0];
                arrPointCoord_temp[i, 1] = arrPointCoord_temp[number_rows - i - 1, 1];
            }

            // We need to add last point (coincident to the first one)
            // Create new temporary array number_row + 1

            arrPointCoord = new float[number_rows + 1, 3];

            // Copy items from arrtemp to arrtemp_new
            for (int i = 0; i < number_rows; i++)
            {
                arrPointCoord[i, 0] = arrPointCoord_temp[i, 0];
                arrPointCoord[i, 1] = arrPointCoord_temp[i, 1];
                arrPointCoord[i, 2] = arrPointCoord_temp[i, 2];
            }

            // Add last point (same data as first one)
            arrPointCoord[number_rows, 0] = arrPointCoord_temp[0, 0];
            arrPointCoord[number_rows, 1] = arrPointCoord_temp[0, 1];
            arrPointCoord[number_rows, 2] = ft;
        }
    }
}
