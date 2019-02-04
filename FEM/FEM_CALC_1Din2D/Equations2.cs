using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
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

        // USA

        // Member stifness matrix in GCS
        //000 000
        public static void GetMemberStiffnessMatrix_K_Items_000_000_GCS(double A, double I, double E, double L, double lambda_x, double lambda_y, out double kMA, out double kMB, out double kMC, out double kMD, out double kME, out double kMF, out double kMG)
        {
            kMA = A * E / L * MathF.Pow2(lambda_x) + 12 * E * I / MathF.Pow3(L) * MathF.Pow2(lambda_y);
            kMB = (A * E / L - 12 * E * I / MathF.Pow3(L)) * lambda_x * lambda_y;
            kMC = 6 * E * I / MathF.Pow2(L) * lambda_y;
            kMD = A * E / L * MathF.Pow2(lambda_y) + 12 * E * I / MathF.Pow3(L) * MathF.Pow2(lambda_x);
            kME = 6 * E * I / MathF.Pow2(L) * lambda_x;
            kMF = 4 * E * I / L;
            kMG = 2 * E * I / L;
        }
        public static double [,] GetMemberStiffnessMatrixArray_K_000_000_GCS(double A, double I, double E, double L, double lambda_x, double lambda_y)
        {
            double kMA;
            double kMB;
            double kMC;
            double kMD;
            double kME;
            double kMF;
            double kMG;

            GetMemberStiffnessMatrix_K_Items_000_000_GCS(A, I, E, L, lambda_x, lambda_y, out kMA, out kMB, out kMC, out kMD, out kME, out kMF, out kMG);

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
        public static MatrixF64 GetMemberStiffnessMatrix_K_000_000_GCS(double A, double I, double E, double L, double lambda_x, double lambda_y)
        {
            double kMA;
            double kMB;
            double kMC;
            double kMD;
            double kME;
            double kMF;
            double kMG;

            GetMemberStiffnessMatrix_K_Items_000_000_GCS(A, I, E, L, lambda_x, lambda_y, out kMA, out kMB, out kMC, out kMD, out kME, out kMF, out kMG);

            double[] entries = new double[36]{
                 kMA,  kMB, -kMC, -kMA, -kMB, -kMC,
                 kMB,  kMD,  kME, -kMB, -kMD,  kME,
                -kMC,  kME,  kMF,  kMC, -kME,  kMG,
                -kMA, -kMB,  kMC,  kMA,  kMB,  kMC,
                -kMB, -kMD, -kME,  kMB,  kMD, -kME,
                -kMC,  kME,  kMG,  kMC, -kME,  kMF};

            MatrixF64 k_member = new MatrixF64(6, 6);
            k_member.Entries = entries;

            return k_member;
        }

        //000 00_
        public static void GetMemberStiffnessMatrix_K_Items_000_00__GCS(double A, double I, double E, double L, double lambda_x, double lambda_y, out double kMA, out double kMB, out double kMC, out double kMD, out double kME, out double kMF)
        {
            kMA = A * E / L * MathF.Pow2(lambda_x) + 3 * E * I / MathF.Pow3(L) * MathF.Pow2(lambda_y);
            kMB = (A * E / L - 3 * E * I / MathF.Pow3(L)) * lambda_x * lambda_y;
            kMC = 3 * E * I / MathF.Pow2(L) * lambda_y;
            kMD = A * E / L * MathF.Pow2(lambda_y) + 3 * E * I / MathF.Pow3(L) * MathF.Pow2(lambda_x);
            kME = 3 * E * I / MathF.Pow2(L) * lambda_x;
            kMF = 0;
        }
        public static double[,] GetMemberStiffnessMatrixArray_K_000_00__GCS(double A, double I, double E, double L, double lambda_x, double lambda_y)
        {
            double kMA;
            double kMB;
            double kMC;
            double kMD;
            double kME;
            double kMF;

            GetMemberStiffnessMatrix_K_Items_000_00__GCS(A, I, E, L, lambda_x, lambda_y, out kMA, out kMB, out kMC, out kMD, out kME, out kMF);

            double[,] k = new double[6, 6]
            {
                { kMA, kMB,-kMC,-kMA,-kMB, kMF},
                { kMB, kMD, kME,-kMB,-kMD, kMF},
                {-kMC, kME, kMF, kMC,-kME, kMF},
                {-kMA,-kMB, kMC, kMA, kMB, kMF},
                {-kMB,-kMD,-kME, kMB, kMD, kMF},
                { kMF, kMF, kMF, kMF, kMF, kMF}
            };

            return k;
        }
        public static MatrixF64 GetMemberStiffnessMatrix_K_000_00__GCS(double A, double I, double E, double L, double lambda_x, double lambda_y)
        {
            double kMA;
            double kMB;
            double kMC;
            double kMD;
            double kME;
            double kMF;

            GetMemberStiffnessMatrix_K_Items_000_00__GCS(A, I, E, L, lambda_x, lambda_y, out kMA, out kMB, out kMC, out kMD, out kME, out kMF);

            double[] entries = new double[36]{
                 kMA, kMB,-kMC,-kMA,-kMB, kMF,
                 kMB, kMD, kME,-kMB,-kMD, kMF,
                -kMC, kME, kMF, kMC,-kME, kMF,
                -kMA,-kMB, kMC, kMA, kMB, kMF,
                -kMB,-kMD,-kME, kMB, kMD, kMF,
                 kMF, kMF, kMF, kMF, kMF, kMF};

            MatrixF64 k_member = new MatrixF64(6, 6);
            k_member.Entries = entries;

            return k_member;
        }

        //00_ 000
        public static void GetMemberStiffnessMatrix_K_Items_00__000_GCS(double A, double I, double E, double L, double lambda_x, double lambda_y, out double kMA, out double kMB, out double kMC, out double kMD, out double kME, out double kMF)
        {
            kMA = A * E / L * MathF.Pow2(lambda_x) + 3 * E * I / MathF.Pow3(L) * MathF.Pow2(lambda_y);
            kMB = (A * E / L - 3 * E * I / MathF.Pow3(L)) * lambda_x * lambda_y;
            kMC = 3 * E * I / MathF.Pow2(L) * lambda_y;
            kMD = A * E / L * MathF.Pow2(lambda_y) + 3 * E * I / MathF.Pow3(L) * MathF.Pow2(lambda_x);
            kME = 3 * E * I / MathF.Pow2(L) * lambda_x;
            kMF = 0;
        }
        public static double[,] GetMemberStiffnessMatrixArray_K_00__000_GCS(double A, double I, double E, double L, double lambda_x, double lambda_y)
        {
            double kMA;
            double kMB;
            double kMC;
            double kMD;
            double kME;
            double kMF;

            GetMemberStiffnessMatrix_K_Items_00__000_GCS(A, I, E, L, lambda_x, lambda_y, out kMA, out kMB, out kMC, out kMD, out kME, out kMF);

            double[,] k = new double[6, 6]
            {
                { kMA, kMB, kMF,-kMA,-kMB, kMC},
                { kMB, kMD, kMF,-kMB,-kMD,-kME},
                { kMF, kMF, kMF, kMF, kMF, kMF},
                {-kMA,-kMB, kMF, kMA, kMB,-kMC},
                {-kMB,-kMD, kMF, kMB, kMD, kME},
                { kMC,-kME, kMF,-kMC, kME, kMF}
            };

            return k;
        }
        public static MatrixF64 GetMemberStiffnessMatrix_K_00__000_GCS(double A, double I, double E, double L, double lambda_x, double lambda_y)
        {
            double kMA;
            double kMB;
            double kMC;
            double kMD;
            double kME;
            double kMF;

            GetMemberStiffnessMatrix_K_Items_00__000_GCS(A, I, E, L, lambda_x, lambda_y, out kMA, out kMB, out kMC, out kMD, out kME, out kMF);

            double[] entries = new double[36]{
                 kMA, kMB, kMF,-kMA,-kMB, kMC,
                 kMB, kMD, kMF,-kMB,-kMD,-kME,
                 kMF, kMF, kMF, kMF, kMF, kMF,
                -kMA,-kMB, kMF, kMA, kMB,-kMC,
                -kMB,-kMD, kMF, kMB, kMD, kME,
                 kMC,-kME, kMF,-kMC, kME, kMF};

            MatrixF64 k_member = new MatrixF64(6, 6);
            k_member.Entries = entries;

            return k_member;
        }

        //00_ 00_
        public static void GetMemberStiffnessMatrix_K_Items_00__00__GCS(double A, double E, double L, double lambda_x, double lambda_y, out double kMA, out double kMB, out double kMC, out double kMD)
        {
            kMA = A * E / L * MathF.Pow2(lambda_x);
            kMB = A * E / L * lambda_x * lambda_y;
            kMC = A * E / L * MathF.Pow2(lambda_y);
            kMD = 0;
        }
        public static double[,] GetMemberStiffnessMatrixArray_K_00__00__GCS(double A, double E, double L, double lambda_x, double lambda_y)
        {
            double kMA;
            double kMB;
            double kMC;
            double kMD;

            GetMemberStiffnessMatrix_K_Items_00__00__GCS(A, E, L, lambda_x, lambda_y, out kMA, out kMB, out kMC, out kMD);

            double[,] k = new double[6, 6]
            {
                { kMA, kMB, kMD,-kMA,-kMB, kMD},
                { kMB, kMC, kMD,-kMB,-kMC, kMD},
                { kMD, kMD, kMD, kMD, kMD, kMD},
                {-kMA,-kMB, kMD, kMA, kMB, kMD},
                {-kMB,-kMC, kMD, kMB, kMC, kMD},
                { kMD, kMD, kMD, kMD, kMD, kMD}
            };

            return k;
        }
        public static MatrixF64 GetMemberStiffnessMatrix_K_00__00__GCS(double A, double I, double E, double L, double lambda_x, double lambda_y)
        {
            double kMA;
            double kMB;
            double kMC;
            double kMD;

            GetMemberStiffnessMatrix_K_Items_00__00__GCS(A, E, L, lambda_x, lambda_y, out kMA, out kMB, out kMC, out kMD);

            double[] entries = new double[36]{
                 kMA, kMB, kMD,-kMA,-kMB, kMD,
                 kMB, kMC, kMD,-kMB,-kMC, kMD,
                 kMD, kMD, kMD, kMD, kMD, kMD,
                -kMA,-kMB, kMD, kMA, kMB, kMD,
                -kMB,-kMC, kMD, kMB, kMC, kMD,
                 kMD, kMD, kMD, kMD, kMD, kMD};

            MatrixF64 k_member = new MatrixF64(6, 6);
            k_member.Entries = entries;

            return k_member;
        }






        public static double[,] GetMemberTransformationMatrixArray_T(double lambda_x, double lambda_y)
        {
            double[,] T = new double[6, 6]
            {
            { lambda_x, lambda_y,   0,          0,        0,   0},
            {-lambda_y, lambda_x,   0,          0,        0,   0},
            {        0,        0,   1,          0,        0,   0},
            {        0,        0,   0,   lambda_x, lambda_y,   0},
            {        0,        0,   0,  -lambda_y, lambda_x,   0},
            {        0,        0,   0,          0,        0,   1}
            };

            return T;
        }

        public static MatrixF64 GetMemberTransformationMatrix_T(double lambda_x, double lambda_y)
        {
            double[] entries = new double[36]{
                 lambda_x, lambda_y,   0,          0,        0,   0,
                -lambda_y, lambda_x,   0,          0,        0,   0,
                        0,        0,   1,          0,        0,   0,
                        0,        0,   0,   lambda_x, lambda_y,   0,
                        0,        0,   0,  -lambda_y, lambda_x,   0,
                        0,        0,   0,          0,        0,   1};

            MatrixF64 T = new MatrixF64(6, 6);
            T.Entries = entries;

            return T;
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
            double wb = w; // load value at start postion "b"
            double we = w; // load value at end position "e"

            // b - distance of load start from member start point
            // e - distance of load end from member start point

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




        // Cesky zdroj

        // Member coordinate matrix in LCS
        public static void GetMemberStiffnessMatrix_K_Items_000_000_LCS(double A, double I, double E, double L, out double kMA, out double kMB, out double kMC, out double kMD, out double kME, out double kMF)
        {
            kMA = A * E / L;
            kMB = 12 * E * I / MathF.Pow3(L);
            kMC = 6 * E * I / MathF.Pow2(L);
            kMD = 4 * E * I / L;
            kME = 2 * E * I / L;
            kMF = 0;
        }
        public static void GetMemberStiffnessMatrix_K_Items_000_00__LCS(double A, double I, double E, double L, out double kMA, out double kMB, out double kMC, out double kMD, out double kME)
        {
            kMA = A * E / L;
            kMB = 3 * E * I / MathF.Pow3(L);
            kMC = 3 * E * I / MathF.Pow2(L);
            kMD = 3 * E * I / L;
            kME = 0;
        }
        public static void GetMemberStiffnessMatrix_K_Items_00__000_LCS(double A, double I, double E, double L, out double kMA, out double kMB, out double kMC, out double kMD, out double kME)
        {
            kMA = A * E / L;
            kMB = 3 * E * I / MathF.Pow3(L);
            kMC = 3 * E * I / MathF.Pow2(L);
            kMD = 3 * E * I / L;
            kME = 0;
        }
        public static void GetMemberStiffnessMatrix_K_Items_00__00__LCS(double A, double E, double L, out double kMA, out double kMB)
        {
            kMA = A * E / L;
            kMB = 0;
        }

        public static MatrixF64 GetMemberStiffnessMatrix_K_000_000_LCS(double A, double I, double E, double L)
        {
            double kMA;
            double kMB;
            double kMC;
            double kMD;
            double kME;
            double kMF;

            GetMemberStiffnessMatrix_K_Items_000_000_LCS(A, I, E, L, out kMA, out kMB, out kMC, out kMD, out kME, out kMF);

            double[] entries = new double[36]{
                 kMA,  kMF,  kMF, -kMA,  kMF,  kMF,
                 kMF,  kMB, -kMC,  kMF, -kMB, -kMC,
                 kMF, -kMC,  kMD,  kMF,  kMC,  kME,
                -kMA,  kMF,  kMF,  kMA,  kMF,  kMF,
                 kMF, -kMB,  kMC,  kMF,  kMB,  kMC,
                 kMF, -kMC,  kME,  kMF,  kMC,  kMD};

            MatrixF64 k_member = new MatrixF64(6, 6);
            k_member.Entries = entries;

            return k_member;
        }
        public static MatrixF64 GetMemberStiffnessMatrix_K_000_00__LCS(double A, double I, double E, double L)
        {
            double kMA;
            double kMB;
            double kMC;
            double kMD;
            double kME;

            GetMemberStiffnessMatrix_K_Items_000_00__LCS(A, I, E, L, out kMA, out kMB, out kMC, out kMD, out kME);

            double[] entries = new double[36]{
                 kMA,  kME,  kME, -kMA,  kME,  kME,
                 kME,  kMB, -kMC,  kME, -kMB,  kME,
                 kME, -kMC,  kMD,  kME,  kMC,  kME,
                -kMA,  kME,  kME,  kMA,  kME,  kME,
                 kME, -kMB,  kMC,  kME,  kMB,  kME,
                 kME,  kME,  kME,  kME,  kME,  kME};

            MatrixF64 k_member = new MatrixF64(6, 6);
            k_member.Entries = entries;

            return k_member;
        }
        public static MatrixF64 GetMemberStiffnessMatrix_K_00__000_LCS(double A, double I, double E, double L)
        {
            double kMA;
            double kMB;
            double kMC;
            double kMD;
            double kME;

            GetMemberStiffnessMatrix_K_Items_00__000_LCS(A, I, E, L, out kMA, out kMB, out kMC, out kMD, out kME);

            double[] entries = new double[36]{
                 kMA,  kME,  kME, -kMA,  kME,  kME,
                 kME,  kMB,  kME,  kME, -kMB, -kMC,
                 kME,  kME,  kME,  kME,  kME,  kME,
                -kMA,  kME,  kME,  kMA,  kME,  kME,
                 kME, -kMB,  kME,  kME,  kMB,  kMC,
                 kME, -kMC,  kME,  kME,  kMC,  kMD};

            MatrixF64 k_member = new MatrixF64(6, 6);
            k_member.Entries = entries;

            return k_member;
        }
        public static MatrixF64 GetMemberStiffnessMatrix_K_00__00__LCS(double A, double E, double L)
        {
            double kMA;
            double kMB;

            GetMemberStiffnessMatrix_K_Items_00__00__LCS(A, E, L, out kMA, out kMB);

            double[] entries = new double[36]{
                 kMA,  kMB,  kMB, -kMA,  kMB,  kMB,
                 kMB,  kMB,  kMB,  kMB,  kMB,  kMB,
                 kMB,  kMB,  kMB,  kMB,  kMB,  kMB,
                -kMA,  kMB,  kMB,  kMA,  kMB,  kMB,
                 kMB,  kMB,  kMB,  kMB,  kMB,  kMB,
                 kMB,  kMB,  kMB,  kMB,  kMB,  kMB};

            MatrixF64 k_member = new MatrixF64(6, 6);
            k_member.Entries = entries;

            return k_member;
        }
    }
}
