using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MATH;

namespace FEM_CALC_1Din2D
{
    public static class Equations2
    {
        public static double GetLength(double xi, double yi, double xj, double yj)
        {
            return MathF.Sqrt(MathF.Pow2(xj - xi) + MathF.Pow2(yj - yi)); // Theoretical length of member
        }

        public static double GetCosine_x(double xi, double xj, double L)
        {
            return (xj - xi) / L; // lambda_x
        }

        public static double GetCosine_y(double yi, double yj, double L)
        {
            return (yj - yi) / L; // lambda_y
        }

        public static double [,] GetMemberStiffnessMatrix_K(double A, double I, double E, double L, double lambda_x, double lambda_y)
            {
        double kMA = A * E / L * MathF.Pow2(lambda_x) + 12 * E * I / MathF.Pow3(L) * MathF.Pow2(lambda_y);
        double kMB = (A * E / L - 12 * E * I / MathF.Pow3(L)) * lambda_x * lambda_y;
        double kMC = 6 * E * I / MathF.Pow2(L) * lambda_y;
        double kMD = A * E / L * MathF.Pow2(lambda_y) + 12 * E * I / MathF.Pow3(L) * MathF.Pow2(lambda_x);
        double kME = 6 * E * I / MathF.Pow2(L) * lambda_x;
        double kMF = 4 * E * I / L;
        double kMG = 2 * E * I / L;

        double[,] k = new double[6, 6]
        {
            { kMA,  kMB,-kMC,-kMA, -kMB, -kMC},
            { kMB,  kMD, kME,-kMB, -kMD,  kME},
            {-kMC,  kME, kMF, kMC, -kME,  kMG},
            {-kMA, -kMB, kMC, kMA,  kMB,  kMC},
            {-kMB, -kMD,-kME, kMB,  kMD, -kME},
            {-kMC,  kME, kMG, kMC, -kME,  kMF}
        };

        return k;
        }

        public static void GetFunctionUniformDistributedLoad_L(double E, double I, double w, double b, double e, double L, out double FvL, out double FmL, out double FPhiL, out double FDeltaL)
        {
            double wb = w * b;
            double we = w * e;

            // Loading functions for each uniform or distributed load evaluated at distance x = L from left end of member:
            FvL = -wb * (L - b - (L - e)) + -1 / 2 * (we - wb) / (e - b) * (MathF.Pow2(L - b) - MathF.Pow2(L - e)) + (we - wb) * (L - e);
            FmL = -wb / 2 * (MathF.Pow2(L - b) - MathF.Pow2(L - e)) + -1 / 6 * (we - wb) / (e - b) * (MathF.Pow3(L - b) - MathF.Pow3(L - e)) + (we - wb) / 2 * MathF.Pow2(L - e);
            FPhiL = -wb / (6 * E * I) * (MathF.Pow3(L - b) - MathF.Pow3(L - e)) + -1 / (24 * E * I) * (we - wb) / (e - b) * (MathF.Pow4(L - b) - MathF.Pow4(L - e)) + (we - wb) / (6 * E * I) * MathF.Pow3(L - e);
            FDeltaL = -wb / (24 * E * I) * (MathF.Pow4(L - b) - MathF.Pow4(L - e)) + -1 / (120 * E * I) * (we - wb) / (e - b) * (MathF.Pow5(L - b) - MathF.Pow5(L - e)) + (we - wb) / (24 * E * I) * MathF.Pow4(L - e);
        }

        public static void GetFunctionUniformDistributedLoad_x(double E, double I, double w, double b, double e, double x, out double Fvx, out double Fmx, out double FPhix, out double FDeltax)
        {
            double wb = w * b;
            double we = w * e;

            // Loading functions for each uniform or distributed load evaluated at distance = x from left end of member:

            if (x >= e)
            {
                Fvx = -wb * (x - b - (x - e)) + -1 / 2 * (we - wb) / (e - b) * (MathF.Pow2(x - b) - MathF.Pow2(x - e)) + (we - wb) * (x - e);
                Fmx = -wb / 2 * (MathF.Pow2(x - b) - MathF.Pow2(x - e)) + -1 / 6 * (we - wb) / (e - b) * (MathF.Pow3(x - b) - MathF.Pow3(x - e)) + (we - wb) / 2 * MathF.Pow2(x - e);
                FPhix = -wb / (6 * E * I) * (MathF.Pow3(x - b) - MathF.Pow3(x - e)) + -1 / (24 * E * I) * (we - wb) / (e - b) * (MathF.Pow4(x - b) - MathF.Pow4(x - e)) + (we - wb) / (6 * E * I) * MathF.Pow3(x - e);
                FDeltax = -wb / (24 * E * I) * (MathF.Pow4(x - b) - MathF.Pow4(x - e)) + -1 / (120 * E * I) * (we - wb) / (e - b) * (MathF.Pow5(x - b) - MathF.Pow5(x - e)) + (we - wb) / (24 * E * I) * MathF.Pow4(x - e);
            }
            else if (x >= b)
            {
                Fvx = -wb * (x - b) + -1 / 2 * (we - wb) / (e - b) * MathF.Pow2(x - b);
                Fmx = -wb / 2 * MathF.Pow2(x - b) + -1 / 6 * (we - wb) / (e - b) * MathF.Pow3(x - b) - MathF.Pow3(x - e);
                FPhix = -wb / (6 * E * I) * MathF.Pow3(x - b) + -1 / (24 * E * I) * (we - wb) / (e - b) * MathF.Pow4(x - b);
                FDeltax = -wb / (24 * E * I) * MathF.Pow4(x - b) + -1 / (120 * E * I) * (we - wb) / (e - b) * MathF.Pow5(x - b);
            }
            else
            {
                Fvx = 0;
                Fmx = 0;
                FPhix = 0;
                FDeltax = 0;
            }
        }

        public static void GetFunctionPointForceLoad_L(double E, double I, double P, double a, double L, out double FvL, out double FmL, out double FPhiL, out double FDeltaL)
        {
            // For Point Loads:
            // Loading functions for each point load evaluated at distance x = L from left end of member:
            FvL = -P;
            FmL = -P * (L - a);
            FPhiL = -P * MathF.Pow2(L - a) / (2 * E * I);
            FDeltaL = P * MathF.Pow3(L - a) / (6 * E * I);
        }

        public static void GetFunctionPointForceLoad_x(double E, double I, double P, double a, double x, out double Fvx, out double Fmx, out double FPhix, out double FDeltax)
        {
            // Loading functions for each point load evaluated at distance = x from left end of member:

            if (x > a)
            {
                Fvx = -P;
                Fmx = -P * (x - a);
                FPhix = -P * MathF.Pow2(x - a) / (2 * E * I);
                FDeltax = P * MathF.Pow3(x - a) / (6 * E * I);
            }
            else
            {
                Fvx = 0;
                Fmx = 0;
                FPhix = 0;
                FDeltax = 0;
            }
        }

        public static void GetFunctionPointMomentLoad_L(double E, double I, double M, double c, double L, out double FvL, out double FmL, out double FPhiL, out double FDeltaL)
        {
            // For Applied Moments:
            // Loading functions for each applied moment evaluated at distance x = L from left end of member:
            FvL = 0;
            FmL = -M;
            FPhiL = -M * (L - c) / (E * I);
            FDeltaL = M * MathF.Pow2(L - c) / (2 * E * I);
        }

        public static void GetFunctionPointMomentLoad_x(double E, double I, double M, double c, double x, out double Fvx, out double Fmx, out double FPhix, out double FDeltax)
        {
            // Loading functions for each applied moment evaluated at distance = x from left end of member:

            if (x > c)
            {
                Fvx = 0;
                Fmx = -M;
                FPhix = -M * (x - c) / (E * I);
                FDeltax = M * MathF.Pow2(x - c) / (2 * E * I);
            }
            else
            {
                Fvx = 0;
                Fmx = 0;
                FPhix = 0;
                FDeltax = 0;
            }
        }
    }
}
