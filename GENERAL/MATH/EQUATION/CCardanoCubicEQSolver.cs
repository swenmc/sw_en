using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MATH
{
    public class CCardanoCubicEQSolver
    {

        public struct TKomplex
        {
            public double real;
            public double imag;
        };

        TKomplex x1, x2, x3;
        double a1, a, b, c, d;

        // Real roots
        public double x1_real, x2_real, x3_real;

        // Minimum real positive root
        public double x_min_positive;

        // Minimum real root
        public double x_min;
        // Minimum real root
        public double x_max;

        public CCardanoCubicEQSolver()
        {

        }

        public CCardanoCubicEQSolver(double a1_input, double b_input, double c_input, double d_input)
        {
            x1 = new TKomplex();
            x2 = new TKomplex();
            x3 = new TKomplex();
            x1.real = 0;
            x1.imag = 0;
            x2.real = 0;
            x2.imag = 0;
            x3.real = 0;
            x3.imag = 0;

            a1 = a1_input;
            b = b_input;
            c = c_input;
            d = d_input;

            // Calculate and set results
            SetResults();
        }

        public double Xroot(double a, double x)
        {
            double i = 1;
            if (a < 0)
                i = -1;
            return (i * Math.Exp(Math.Log(a * i) / x));
        }

        public int Calc_Cardano()  // solve cubic equation according to cardano
        {
            double p, q, u, v;
            double r, alpha;
            int res;
            res = 0;
            if (a1 != 0)
            {
                a = b / a1;
                b = c / a1;
                c = d / a1;

                p = -(a * a / 3.0) + b;
                q = (2.0 / 27.0 * a * a * a) - (a * b / 3.0) + c;
                d = q * q / 4.0 + p * p * p / 27.0;
                if (Math.Abs(d) < Math.Pow(10.0, -11.0))
                    d = 0;
                // 3 cases D > 0, D == 0 and D < 0
                if (d > 1e-20)
                {
                    u = Xroot(-q / 2.0 + Math.Sqrt(d), 3.0);
                    v = Xroot(-q / 2.0 - Math.Sqrt(d), 3.0);
                    x1.real = u + v - a / 3.0;
                    x2.real = -(u + v) / 2.0 - a / 3.0;
                    x2.imag = Math.Sqrt(3.0) / 2.0 * (u - v);
                    x3.real = x2.real;
                    x3.imag = -x2.imag;
                    res = 1;
                }
                if (Math.Abs(d) <= 1e-20)
                {
                    u = Xroot(-q / 2.0, 3.0);
                    v = Xroot(-q / 2.0, 3.0);
                    x1.real = u + v - a / 3.0;
                    x2.real = -(u + v) / 2.0 - a / 3.0;
                    res = 2;
                }
                if (d < -1e-20)
                {
                    r = Math.Sqrt(-p * p * p / 27.0);
                    alpha = Math.Atan(Math.Sqrt(-d) / -q * 2.0);
                    if (q > 0)                         // if q > 0 the angle becomes 2 * PI - alpha
                        alpha = 2.0 * Math.PI - alpha;

                    x1.real = Xroot(r, 3.0) * (Math.Cos((6.0 * Math.PI - alpha) / 3.0) + Math.Cos(alpha / 3.0)) - a / 3.0;
                    x2.real = Xroot(r, 3.0) * (Math.Cos((2.0 * Math.PI + alpha) / 3.0) + Math.Cos((4.0 * Math.PI - alpha) / 3.0)) - a / 3.0;
                    x3.real = Xroot(r, 3.0) * (Math.Cos((4.0 * Math.PI + alpha) / 3.0) + Math.Cos((2.0 * Math.PI - alpha) / 3.0)) - a / 3.0;
                    res = 3;
                }
            }
            else
                res = 0;
            return res;
        }

        public void SetResults()
        {
            if ((a1 != 0) && (b != 0) && (c != 0) && (d != 0))
            {
                switch (Calc_Cardano())
                {
                    case 0:
                        {
                            // No root
                            x1_real = Double.NaN;
                            x2_real = Double.NaN;
                            x3_real = Double.NaN;

                            x_min = x_max = x_min_positive = Double.NaN;
                            break;
                        }
                    case 1:
                        {
                            // One real root
                            x1_real = x1.real;
                            x2_real = Double.NaN;
                            x3_real = Double.NaN;

                            x_min = x_max = x1.real;

                            x_min_positive = x1_real > 0 ? x1_real : Double.NaN;
                            break;
                        }
                    case 2:
                        {
                            // Two real roots
                            x1_real = x1.real;
                            x2_real = x2.real;
                            x3_real = Double.NaN;

                            x_min = Math.Min(x1.real, x2.real);
                            x_max = Math.Max(x1.real, x2.real);

                            x_min_positive = Math.Min(x1_real > 0 ? x1_real : Double.PositiveInfinity, x2_real > 0 ? x2_real : Double.PositiveInfinity);
                            break;
                        }
                    case 3:
                        {
                            // Three real roots
                            x1_real = x1.real;
                            x2_real = x2.real;
                            x3_real = x3.real;

                            x_min = Math.Min(x1.real, Math.Min(x2.real, x3.real));
                            x_max = Math.Max(x1.real, Math.Max(x2.real, x3.real));

                            x_min_positive = Math.Min(x1_real > 0 ? x1_real : Double.PositiveInfinity, Math.Min(x2_real > 0 ? x2_real : Double.PositiveInfinity, x3_real > 0 ? x3_real : Double.PositiveInfinity));
                            break;
                        }
                }
            }
        }
    }
}
