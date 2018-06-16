using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace M_BASE
{
    // Definiton of transverse load at member types
    public enum ETrLoadType1
    {
        eTrLoadType_N,   // No transversal loading - only End Moments
        eTrLoadType_C,   // Concentrated loading
        eTrLoadType_U,   // Uniform loading
        eTrLoadType_CU,  // Concentrated and uniform loading (no triangular load)
        eTrLoadType_GE   // General loading
    }



    public class CBase
    {
        // Presunut este vyssie

        // Mathematical functions
        public float mult(params float[] a)
        {
            float ftemp = 1f;
            for (int i = 0; i < a.Length; i++)
                ftemp *= a[i];

            return ftemp;
        }

        public float sqr(float a)
        {
            return (float)Math.Pow(a, 2);
        }

    }



















 
}
