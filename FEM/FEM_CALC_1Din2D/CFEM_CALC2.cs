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

            double A = 1; // Area - cross-section
            double E = 1; // Elasticity modulus  material
            double I = 1; // Moment of inertia - cross-section

            double w = 1;
            double e = 1;
            double x = 0.02;
            double b = 1;

            double P = 1;
            double a = 1;

            double M = 1;
            double c = 1;

        }
    }
}
