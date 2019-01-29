using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MATH;

namespace FEM_CALC_1Din2D
{
    public class CFEM_CALC2
    {

        // TEMPORARY, PRIPRAVA ROVNIC Z XLS


        public CFEM_CALC2()
        {
            // Member Start and end point coordinates - point i and j coordinates
            double xi = 0;
            double yi = 0;
            double xj = 1;
            double yj = 0;

            double L = MathF.Sqrt(MathF.Pow2(xj - xi) + MathF.Pow2(yj - yi));

            double A = 1; // Area - cross-section
            double E = 1; // Elasticity modulus  material
            double I = 1; // Moment of inertia - cross-section

            double lambda_x = (xj - xi) / L;
            double lambda_y = (yj - yi) / L;

            double kMA = A * E / L * MathF.Pow2(lambda_x) + 12 * E * I / MathF.Pow3(L) * MathF.Pow2(lambda_y);
            double kMB = (A * E / L - 12 * E * I / MathF.Pow3(L)) * lambda_x * lambda_y;
            double kMC = 6 * E * I / MathF.Pow2(L) * lambda_y;
            double kMD = A * E / L * MathF.Pow2(lambda_y) + 12 * E * I / MathF.Pow3(L) * MathF.Pow2(lambda_x);
            double kME = 6 * E * I / MathF.Pow2(L) * lambda_x;
            double kMF = 4 * E * I / L;
            double kMG = 2 * E * I / L;

            double w = 1;
            double e = 1;
            double x = 0.02;
            double b = 1;

            double wb = w * b;
            double we = w * e;

            // Loading functions for each uniform or distributed load evaluated at distance x = L from left end of member:
            double FvL = -wb * (L - b - (L - e)) + -1 / 2 * (we - wb) / (e - b) * (MathF.Pow2(L - b) - MathF.Pow2(L - e)) + (we - wb) * (L - e);
            double FmL = -wb / 2 * (MathF.Pow2(L - b) - MathF.Pow2(L - e)) + -1 / 6 * (we - wb) / (e - b) * (MathF.Pow3(L - b) - MathF.Pow3(L - e)) + (we - wb) / 2 * MathF.Pow2(L - e);
            double FqL = -wb / (6 * E * I) * (MathF.Pow3(L - b) - MathF.Pow3(L - e)) + -1 / (24 * E * I) * (we - wb) / (e - b) * (MathF.Pow4(L - b) - MathF.Pow4(L - e)) + (we - wb) / (6 * E * I) * MathF.Pow3(L - e);
            double FDL = -wb / (24 * E * I) * (MathF.Pow4(L - b) - MathF.Pow4(L - e)) + -1 / (120 * E * I) * (we - wb) / (e - b) * (MathF.Pow5(L - b) - MathF.Pow5(L - e)) + (we - wb) / (24 * E * I) * MathF.Pow4(L - e);

             // Loading functions for each uniform or distributed load evaluated at distance = x from left end of member:
            double Fvx;
            double Fmx;
            double Fqx;
            double FDx;

            if (x >= e)
            {
                Fvx = -wb * (x - b - (x - e)) + -1 / 2 * (we - wb) / (e - b) * (MathF.Pow2(x - b) - MathF.Pow2(x - e)) + (we - wb) * (x - e);
                Fmx = -wb / 2 * (MathF.Pow2(x - b) - MathF.Pow2(x - e)) + -1 / 6 * (we - wb) / (e - b) * (MathF.Pow3(x - b) - MathF.Pow3(x - e)) + (we - wb) / 2 * MathF.Pow2(x - e);
                Fqx = -wb / (6 * E * I) * (MathF.Pow3(x - b) - MathF.Pow3(x - e)) + -1 / (24 * E * I) * (we - wb) / (e - b) * (MathF.Pow4(x - b) - MathF.Pow4(x - e)) + (we - wb) / (6 * E * I) * MathF.Pow3(x - e);
                FDx = -wb / (24 * E * I) * (MathF.Pow4(x - b) - MathF.Pow4(x - e)) + -1 / (120 * E * I) * (we - wb) / (e - b) * (MathF.Pow5(x - b) - MathF.Pow5(x - e)) + (we - wb) / (24 * E * I) * MathF.Pow4(x - e);
            }
            else if (x >= b)
            {
                Fvx = -wb * (x - b) + -1 / 2 * (we - wb) / (e - b) * MathF.Pow2(x - b);
                Fmx = -wb / 2 * MathF.Pow2(x - b) + -1 / 6 * (we - wb) / (e - b) * MathF.Pow3(x - b) - MathF.Pow3(x - e);
                Fqx = -wb / (6 * E * I) * MathF.Pow3(x - b) + -1 / (24 * E * I) * (we - wb) / (e - b) * MathF.Pow4(x - b);
                FDx = -wb / (24 * E * I) * MathF.Pow4(x - b) + -1 / (120 * E * I) * (we - wb) / (e - b) * MathF.Pow5(x - b);
            }
            else
            {
                Fvx = 0;
                Fmx = 0;
                Fqx = 0;
                FDx = 0;
            }

            // For Point Loads:

            double P = 1;
            double a = 1;

            // Loading functions for each point load evaluated at distance x = L from left end of member:
            FvL = -P;
            FmL = -P * (L - a);
            FqL = -P * MathF.Pow2(L - a) / (2 * E * I);
            FDL = P * MathF.Pow3(L - a) / (6 * E * I);

            // Loading functions for each point load evaluated at distance = x from left end of member:

            if (x > a)
            {
                Fvx = -P;
                Fmx = -P * (x - a);
                Fqx = -P * MathF.Pow2(x - a) / (2 * E * I);
                FDx = P * MathF.Pow3(x - a) / (6 * E * I);
            }
            else
            {
                Fvx = 0;
                Fmx = 0;
                Fqx = 0;
                FDx = 0;
            }

            // For Applied Moments:

            double M = 1;
            double c = 1;

            // Loading functions for each applied moment evaluated at distance x = L from left end of member:
            FvL = 0;
            FmL = -M;
            FqL = -M * (L - c) / (E * I);
            FDL = M * MathF.Pow2(L - c) / (2 * E * I);

            // Loading functions for each applied moment evaluated at distance = x from left end of member:

            if (x > c)
            {
                Fvx = 0;
                Fmx = -M;
                Fqx = -M * (x - c) / (E * I);
                FDx = M * MathF.Pow2(x - c) / (2 * E * I);
            }
            else
            {
                Fvx = 0;
                Fmx = 0;
                Fqx = 0;
                FDx = 0;
            }
        }
    }
}
