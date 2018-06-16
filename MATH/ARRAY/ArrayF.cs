using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MATH.ARRAY
{
    // Array operations
    public static class ArrayF
    {

        public static float[,] fGetSum(float[,] mat1, float[] mat2)
        {
            if ((int)Math.Sqrt(mat1.Length) == mat2.Length)
            {
                float[,] newMatrix = new float[(int)Math.Sqrt(mat1.Length), (int)Math.Sqrt(mat2.Length)];

                for (int x = 0; x < (int)Math.Sqrt(mat1.Length); x++)
                    for (int y = 0; y < mat2.Length; y++)
                        newMatrix[x, y] = mat1[x, y] + mat2[x];

                return newMatrix;
            }
            else
            {
                //Error - exception
                return null;
            }
        }




    }
}
