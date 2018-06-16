using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FEM_CALC_1Din3D
{
    public class Constants
    {
        // Number of Node Degress of freedom in 3D (DOF)
        // Basic size of vectors 6x1 and matrices 6x6
        public const int i3D_DOFNo = 6;

        //  DOF Array Constants

        public int UX = 0;
        public int UY = 1;
        public int UZ = 2;
        public int RX = 3;
        public int RY = 4;
        public int RZ = 5;
        public int DEPL = 6;

        //  Nodal Load Array Constants

        public int FX = 0;
        public int FY = 1;
        public int FZ = 2;
        public int MX = 3;
        public int MY = 4;
        public int MZ = 5;
        public int BM = 6;

        // Internal Forces

        public int NX = 0;
        public int VY = 1;
        public int VZ = 2;
        //public int MX = 3;
        //public int MZ = 5;
        //public int BM = 6;

       public Constants() {}
 



    }
}
