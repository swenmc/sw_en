using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MATH
{
    public class CMatrix
    {
        public int m_iRows;

        public int IRows
        {
            get { return m_iRows; }
            set { m_iRows = value; }
        }


        public int m_iColumns;

        public int IColumns
        {
            get { return m_iColumns; }
            set { m_iColumns = value; }
        }

        public float [,] m_fArrMembers;

        public CMatrix[,] m_fArrMembersABxCD;

        //-----------------------------------------------------------------------------------------------
        public CMatrix() {}

        public CMatrix(int iRows, int iCol)
        {
            m_iRows = iRows;
            m_iColumns = iCol;

            m_fArrMembers = new float [m_iRows, m_iColumns];
        }

        //-----------------------------------------------------------------------------------------------
        public CMatrix(int iRowsCols) // square matrix 
        {
            m_iRows = m_iColumns = iRowsCols;

            m_fArrMembers = new float[m_iRows, m_iColumns];
        }

        //-----------------------------------------------------------------------------------------------
        public CMatrix (CMatrix m11, CMatrix m12, CMatrix m21, CMatrix m22)
        {
            m_fArrMembersABxCD = new CMatrix [2,2];
            m_fArrMembersABxCD[0, 0] = m11;
            m_fArrMembersABxCD[0, 1] = m12;
            m_fArrMembersABxCD[1, 0] = m21;
            m_fArrMembersABxCD[1, 1] = m22;
        }





















        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Determinant
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public static float fGetDeterminant(float[,] a, int n)
        {
            int i, j, k;
            float det = 0;
            for (i = 0; i < n - 1; i++)
            {
                for (j = i + 1; j < n; j++)
                {
                    det = a[j, i] / a[i, i];
                    for (k = i; k < n; k++)
                        a[j, k] = a[j, k] - det * a[i, k]; // HERE
                }
            }
            det = 1;
            for (i = 0; i < n; i++)
                det = det * a[i, i];
            return det;
       
        }



        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Sum of Matrices
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////// 

        /*
        public static CMatrix operator +(CMatrix mat1, CMatrix mat2)
        {
            CMatrix newMatrix = new CMatrix();

            for (int x = 0; x < m_iRows(); x++)
                for (int y = 0; y < m_iColumns; y++)
                    newMatrix[x, y] = mat1[x, y] + mat2[x, y];

            return newMatrix;
        }
        */

        // Sum of square array with equal size

        public float [,] fGetSum(float [,] mat1 , float [,] mat2)
        {
            if ((int)Math.Sqrt(mat1.Length) == (int)Math.Sqrt(mat2.Length))
            {
                float[,] newMatrix = new float[(int)Math.Sqrt(mat1.Length), (int)Math.Sqrt(mat2.Length)];

                for (int x = 0; x < (int)Math.Sqrt(mat1.Length); x++)
                    for (int y = 0; y < (int)Math.Sqrt(mat2.Length); y++)
                    newMatrix[x, y] = mat1[x, y] + mat2[x, y];

            return newMatrix;
            }
            else
            {
             //Error - exception
            return null;
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Inverse Matrix
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
       
        // DotNetMatrix: Simple Matrix Library for .NET
        // http://www.codeproject.com/script/Articles/ViewDownloads.aspx?aid=5835

        // http://forum.builder.cz/read.php?13,3314639
        
        
        
        
        /*
        http://www.izzycode.com/cpp/c-matrix-inverse-function.html
         */

              /* This function calculates the inverse of a square matrix
             
              C++ source 
              
              matrix_inverse(double *Min, double *Mout, int actualsize)
             
              Min : Pointer to Input square Double Matrix
              Mout : Pointer to Output (empty) memory space with size of Min
              actualsize : The number of rows/columns
             
              Notes:
               - the matrix must be invertible
               - there's no pivoting of rows or columns, hence,
                     accuracy might not be adequate for your needs.
             
              Code is rewritten from c++ template code Mike Dinolfo
             */

        /*       
               void GetInverse(double* Min, double* Mout, int actualsize)
               {

                   // Loop variables 
                   int i, j, k;
                   // Sum variables 
                   double sum, x;

                   //  Copy the input matrix to output matrix
                   for (i = 0; i < actualsize * actualsize; i++) { Mout[i] = Min[i]; }

                   // Add small value to diagonal if diagonal is zero
                   for (i = 0; i < actualsize; i++)
                   {
                       j = i * actualsize + i;
                       if ((Mout[j] < 1e-12) && (Mout[j] > -1e-12)) { Mout[j] = 1e-12; }
                   }

                   // Matrix size must be larger than one 
                   if (actualsize <= 1) return;

                   for (i = 1; i < actualsize; i++)
                   {
                       Mout[i] /= Mout[0]; // normalize row 0 
                   }

                   for (i = 1; i < actualsize; i++)
                   {
                       for (j = i; j < actualsize; j++)
                       { // do a column of L 
                           sum = 0.0;
                           for (k = 0; k < i; k++)
                           {
                               sum += Mout[j * actualsize + k] * Mout[k * actualsize + i];
                           }
                           Mout[j * actualsize + i] -= sum;
                       }
                       if (i == actualsize - 1) continue;
                       for (j = i + 1; j < actualsize; j++)
                       {  // do a row of U 
                           sum = 0.0;
                           for (k = 0; k < i; k++)
                           {
                               sum += Mout[i * actualsize + k] * Mout[k * actualsize + j];
                           }
                           Mout[i * actualsize + j] = (Mout[i * actualsize + j] - sum) / Mout[i * actualsize + i];
                       }
                   }
                   for (i = 0; i < actualsize; i++)  // invert L 
                   {
                       for (j = i; j < actualsize; j++)
                       {
                           x = 1.0;
                           if (i != j)
                           {
                               x = 0.0;
                               for (k = i; k < j; k++)
                               {
                                   x -= Mout[j * actualsize + k] * Mout[k * actualsize + i];
                               }
                           }
                           Mout[j * actualsize + i] = x / Mout[j * actualsize + j];
                       }
                   }
                   for (i = 0; i < actualsize; i++) // invert U 
                   {
                       for (j = i; j < actualsize; j++)
                       {
                           if (i == j) continue;
                           sum = 0.0;
                           for (k = i; k < j; k++)
                           {
                               sum += Mout[k * actualsize + j] * ((i == k) ? 1.0 : Mout[i * actualsize + k]);
                           }
                           Mout[i * actualsize + j] = -sum;
                       }
                   }
                   for (i = 0; i < actualsize; i++) // final inversion 
                   {
                       for (j = 0; j < actualsize; j++)
                       {
                           sum = 0.0;
                           for (k = ((i > j) ? i : j); k < actualsize; k++)
                           {
                               sum += ((j == k) ? 1.0 : Mout[j * actualsize + k]) * Mout[k * actualsize + i];
                           }
                           Mout[j * actualsize + i] = sum;
                       }
                   }
               }



       /// http://chi3x10.wordpress.com/2008/05/28/calculate-matrix-inversion-in-c/

       // matrix inversioon
       // the result is put in Y
       void MatrixInversion(float **A, int order, float **Y)
       {
           // get the determinant of a
           double det = 1.0/CalcDeterminant(A,order);

           // memory allocation
           float *temp = new float[(order-1)*(order-1)];
           float **minor = new float*[order-1];
           for(int i=0;i<order-1;i++)
               minor[i] = temp+(i*(order-1));

           for(int j=0;j<order;j++)
           {
               for(int i=0;i<order;i++)
               {
                   // get the co-factor (matrix) of A(j,i)
                   GetMinor(A,minor,j,i,order);
                   Y[i][j] = det*CalcDeterminant(minor,order-1);
                   if( (i+j)%2 == 1)
                       Y[i][j] = -Y[i][j];
               }
           }

           // release memory
           //delete [] minor[0];
           delete [] temp;
           delete [] minor;
       }

       // calculate the cofactor of element (row,col)
       int GetMinor(float **src, float **dest, int row, int col, int order)
       {
           // indicate which col and row is being copied to dest
           int colCount=0,rowCount=0;

           for(int i = 0; i < order; i++ )
           {
               if( i != row )
               {
                   colCount = 0;
                   for(int j = 0; j < order; j++ )
                   {
                       // when j is not the element
                       if( j != col )
                       {
                           dest[rowCount][colCount] = src[i][j];
                           colCount++;
                       }
                   }
                   rowCount++;
               }
           }

           return 1;
       }

       // Calculate the determinant recursively.
       double CalcDeterminant( float **mat, int order)
       {
           // order must be >= 0
           // stop the recursion when matrix is a single element
           if( order == 1 )
               return mat[0][0];

           // the determinant value
           float det = 0;

           // allocate the cofactor matrix
           float **minor;
           minor = new float*[order-1];
           for(int i=0;i<order-1;i++)
               minor[i] = new float[order-1];

           for(int i = 0; i < order; i++ )
           {
               // get minor of element (0,i)
               GetMinor( mat, minor, 0, i , order);
               // the recusion is here!

               det += (i%2==1?-1.0:1.0) * mat[0][i] * CalcDeterminant(minor,order-1);
               //det += pow( -1.0, i ) * mat[0][i] * CalcDeterminant( minor,order-1 );
           }

           // release memory
           for(int i=0;i<order-1;i++)
               delete [] minor[i];
           delete [] minor;

           return det;
       }

       */


        // Transformation matrix of Member rotation

        public float[,] fTransMatrix(float x_ba, float y_ba, float z_ba, float l, float angle, float[] fCoord_CA)
        {
            // Podla prezentacie ppt ODM str. 25 

            // Output - 3x3 matrix
            /*
             a1  b1  c1
             a2  b2  c2
             a3  b3  c3
             */

            // Local x-Axis
            // Direction cosine

            float fa1 = x_ba / l;
            float fb1 = y_ba / l;
            float fc1 = z_ba / l;

            // Local y-Axis

            // Bod c urcuje treti bod roviny, lezi na lokalnej osi z0 a urcuje teda natocenie pruta
            float x_ca = fCoord_CA[0];
            float y_ca = fCoord_CA[1];
            float z_ca = fCoord_CA[2];

            // Equation of plane
            // obecna rovnica roviny A*(x-xa) + B*(y-ya) + C*(z-za) = 0;
            float A = y_ca * z_ba - y_ba * z_ca;
            float B = x_ba * z_ca - x_ca * z_ba;
            float C = x_ca * y_ba - x_ba * y_ca;

            float d = (float)Math.Sqrt(Math.Pow(A, 2f) + Math.Pow(B, 2f) + Math.Pow(C, 2f));

            // Direction cosine
            float fa2 = A / d;
            float fb2 = B / d;
            float fc2 = C / d;

            // Local z-Axis

            // Direction cosine
            float fa3 = fb1 * fc2 - fb2 * fc1;
            float fb3 = fa2 * fc1 - fa1 * fc2;
            float fc3 = fa1 * fb2 - fa2 * fb1;

            //CMatrix fM = new CMatrix(3, 3);

            return new float[3, 3]
        {
        {fa1, fb1, fc1 },
        {fa2, fb2, fc2 },
        {fa3, fb3 ,fc3 }
        };

        }

        // Dokoncit
        public CMatrix GetInverse(CMatrix fM, double Mout, int actualsize)
        {
            CMatrix fInvMatrix = new CMatrix(3, 3);

            return fInvMatrix;
        }





        // Rotation matrix
        public float[,] fRotationMatrix(float x, float y, float z, float angle)
        {
            return new float[3, 3]
            {
                {1f + (1f-(float)Math.Cos(angle))*(x*x-1f)	                ,-z*(float)Math.Sin(angle)+(1f-(float)Math.Cos(angle))*x*y	, y*(float)Math.Sin(angle)+(1f-(float)Math.Cos(angle))*x*z},
                {z*(float)Math.Sin(angle)+(1f-(float)Math.Cos(angle))*x*y   ,            1f + (1f-(float)Math.Cos(angle))*(y*y-1f)	    ,-x*(float)Math.Sin(angle)+(1f-(float)Math.Cos(angle))*y*z},
                {-y*(float)Math.Sin(angle)+(1f-(float)Math.Cos(angle))*x*z  ,   x*(float)Math.Sin(angle)+(1f-(float)Math.Cos(angle))*y*z,                1f + (1f-(float)Math.Cos(angle))*(z*z-1f)}
            };
        }


        // There's still the problem of performing the actual rotation about your arbitrary axis.  Luckily, this can also be done with a matrix.  Again, I'm not going to derive it, I'm going to spoon feed it to you.  You can thank me later.
        // Where c = cos (theta), s = sin (theta), t = 1-cos (theta), and <X,Y,Z> is the unit vector representing the arbitrary axis

        //Left Handed
        public float[,] fRotationMatrixLeftHand_34(int iMatDim, float X, float Y, float Z, float theta)
        {
            float c = (float)Math.Cos(theta);
            float s = (float)Math.Sin(theta);
            float t = 1f - (float)Math.Cos(theta);

            if (iMatDim == 3)
            {
                return new float[3, 3]
             {
                {t*X*X  +   c,    t*X*Y + s*Z,   t*X*Z - s*Y},
                {t*X*Y  - s*Z,    t*Y*Y +   c,   t*Y*Z + s*X},
                {t*X*Z  + s*Y,    t*Y*Z - s*X,   t*Z*Z +   c}
             };
            }
            else if (iMatDim == 4)
            {
                return new float[4, 4]
             {
                {t*X*X  +   c,    t*X*Y + s*Z,   t*X*Z - s*Y,  0f},
                {t*X*Y  - s*Z,    t*Y*Y +   c,   t*Y*Z + s*X,  0f},
                {t*X*Z  + s*Y,    t*Y*Z - s*X,   t*Z*Z +   c,  0f},
                {          0f,             0f,            0f,  1f}
             };
            }
            else // Exception
            {
                return new float[2, 2]
                { 
                { 0f, 0f },
                { 0f, 0f }
                };
            }
        }

        // Right Handed
        public float[,] fRotationMatrixRightHand_34(int iMatDim, float X, float Y, float Z, float theta)
        {
            float c = (float)Math.Cos(theta);
            float s = (float)Math.Sin(theta);
            float t = 1f - (float)Math.Cos(theta);

            if (iMatDim == 3)
            {
                return new float[3, 3]
             {
                {t*X*X +   c,    t*X*Y - s*Z,   t*X*Y + s*Y},
                {t*X*Y + s*Z,    t*Y*Y +   c,   t*Y*Z - s*X},
                {t*X*Z - s*Y,    t*Y*Z + s*X,   t*Z*Z +   c}
             };
            }
            else if (iMatDim == 4)
            {
                return new float[4, 4]
             {
                {t*X*X +   c,    t*X*Y - s*Z,   t*X*Y + s*Y,  0f},
                {t*X*Y + s*Z,    t*Y*Y +   c,   t*Y*Z - s*X,  0f},
                {t*X*Z - s*Y,    t*Y*Z + s*X,   t*Z*Z +   c,  0f},
                {         0f,             0f,            0f,  1f}
             };
            }
            else // Exception
            {
                return new float[2, 2]
                { 
                { 0f, 0f },
                { 0f, 0f }
                };
            }

        }

        /*

http://inside.mines.edu/~gmurray/ArbitraryAxisRotation/



http://ycouriel.blogspot.com/2010/08/c-rotation-matrix.html

Rotation matrices are used in 3D graphics to rotate vectors.
I use homogeneous coordinates, so the matrices are 4x4. If you want 3x3, just remove the last column and last row.
I wrote several functions:
Rotation around X axis
Rotation around Y axis
Rotation around Z axis
Rotation around all axes
Rotation around any given axis
Rotation from normal vector to normal vector
Apparently the 5th function is enough, because for example "Rotation around X axis" can be replace by rotation around (1,0,0), and "Rotation around all axes" is merely the product of 3 matrices. BUT, one should use the specific function he or she needs, because it is more efficient.

*/






















         /*
        public static Matrix4 GetRotationMatrixX(double angle)
        {
            if (angle == 0.0)
            {
                return Matrix4.I;
            }
            float sin = (float)Math.Sin(angle);
            float cos = (float)Math.Cos(angle);
            return new Matrix4(new float[4, 4] {
        { 1.0f, 0.0f, 0.0f, 0.0f }, 
        { 0.0f,  cos, -sin, 0.0f }, 
        { 0.0f,  sin,  cos, 0.0f }, 
        { 0.0f, 0.0f, 0.0f, 1.0f } });
        }

        public static Matrix4 GetRotationMatrixY(double angle)
        {
            if (angle == 0.0)
            {
                return Matrix4.I;
            }
            float sin = (float)Math.Sin(angle);
            float cos = (float)Math.Cos(angle);
            return new Matrix4(new float[4, 4] {
        {  cos, 0.0f,  sin, 0.0f }, 
        { 0.0f, 1.0f, 0.0f, 0.0f }, 
        { -sin, 0.0f,  cos, 0.0f }, 
        { 0.0f, 0.0f, 0.0f, 1.0f } });
        }

        public static Matrix4 GetRotationMatrixZ(double angle)
        {
            if (angle == 0.0)
            {
                return Matrix4.I;
            }
            float sin = (float)Math.Sin(angle);
            float cos = (float)Math.Cos(angle);
            return new Matrix4(new float[4, 4] {
        {  cos, -sin, 0.0f, 0.0f }, 
        {  sin, cos,  0.0f, 0.0f }, 
        { 0.0f, 0.0f, 1.0f, 0.0f }, 
        { 0.0f, 0.0f, 0.0f, 1.0f } });
        }

        public static Matrix4 GetRotationMatrix(double ax, double ay, double az)
        {
            Matrix4 my = null;
            Matrix4 mz = null;
            Matrix4 result = null;
            if (ax != 0.0)
            {
                result = GetRotationMatrixX(ax);
            }
            if (ay != 0.0)
            {
                my = GetRotationMatrixY(ay);
            }
            if (az != 0.0)
            {
                mz = GetRotationMatrixZ(az);
            }
            if (my != null)
            {
                if (result != null)
                {
                    result *= my;
                }
                else
                {
                    result = my;
                }
            }
            if (mz != null)
            {
                if (result != null)
                {
                    result *= mz;
                }
                else
                {
                    result = mz;
                }
            }
            if (result != null)
            {
                return result;
            }
            else
            {
                return Matrix4.I;
            }
        }

        public static Matrix4 GetRotationMatrix(Vector3 axis, double angle)
        {
            if (angle == 0.0)
            {
                return Matrix4.I;
            }

            float x = axis.x;
            float y = axis.y;
            float z = axis.z;
            float sin = (float)Math.Sin(angle);
            float cos = (float)Math.Cos(angle);
            float xx = x * x;
            float yy = y * y;
            float zz = z * z;
            float xy = x * y;
            float xz = x * z;
            float yz = y * z;

            float[,] matrix = new float[4, 4];

            matrix[0, 0] = xx + (1 - xx) * cos;
            matrix[1, 0] = xy * (1 - cos) + z * sin;
            matrix[2, 0] = xz * (1 - cos) - y * sin;
            matrix[3, 0] = 0.0f;

            matrix[0, 1] = xy * (1 - cos) - z * sin;
            matrix[1, 1] = yy + (1 - yy) * cos;
            matrix[2, 1] = yz * (1 - cos) + x * sin;
            matrix[3, 1] = 0.0f;

            matrix[0, 2] = xz * (1 - cos) + y * sin;
            matrix[1, 2] = yz * (1 - cos) - x * sin;
            matrix[2, 2] = zz + (1 - zz) * cos;
            matrix[3, 2] = 0.0f;

            matrix[3, 0] = 0.0f;
            matrix[3, 1] = 0.0f;
            matrix[3, 2] = 0.0f;
            matrix[3, 3] = 1.0f;

            return new Matrix4(matrix);
        }

        /// <param name="source">Should be normalized</param>
        /// <param name="destination">Should be normalized</param>
        public static Matrix4 GetRotationMatrix(Vector3 source, Vector3 destination)
        {
            Vector3 rotaxis = Vector3.CrossProduct(source, destination);
            if (rotaxis != Vector3.Zero)
            {
                rotaxis.Normalize();
                float cos = source.DotProduct(destination);
                double angle = Math.Acos(cos);
                return GetRotationMatrix(rotaxis, angle);
            }
            else
            {
                return Matrix4.I;
            }
        }

        */




























        // Display / Print matrix

        public string Print2DMatrix(float[,] fM, int iRows, int iColumns)
        {
            string sOutput = null;
            for (int i = 0; i < iRows; i++)
            {
                for (int j = 0; j < iColumns; j++)
                {
                    sOutput += fM[i, j].ToString();
                    sOutput += "\t"; // New Tab between columns
                }
                sOutput += "\n"; // New row
            }

            return sOutput;
        }

        public string Print2DMatrix()
        {
            string sOutput = null;
            for (int i = 0; i < m_iRows; i++)
            {
                for (int j = 0; j < m_iColumns; j++)
                {
                    sOutput += m_fArrMembers[i, j].ToString();
                    sOutput += "\t"; // New Tab between columns
                }
                sOutput += "\n"; // New row
            }

            return sOutput;
        }

        public string Print2DMatrix(float[,] fM, int iSize)
        {
            string sOutput = null;
            for (int i = 0; i < iSize; i++)
            {
                for (int j = 0; j < iSize; j++)
                {
                    sOutput += fM[i,j].ToString();
                    sOutput += "\t"; // New Tab between columns
                }
                sOutput += "\n"; // New row
            }

            return sOutput;
        }




        public void Print2DMatrix(float[,] fM, int iRows, int iColumns, int precision)
        {

            //precision = 0;

            // Determine the largest entry width in characters
            // so that we can make all columns an equal width.
            int largestEntryWidth = 1;
            for (int i = 0; i < iRows; i++)
            {
                for (int j = 0; j < iColumns; j++)
                {
                    String text = String.Format(String.Empty
                        + '{' + '0' + ':' + 'g' + precision + '}', fM[i, j]);
                    if (text.Length > largestEntryWidth)
                    {
                        largestEntryWidth = text.Length;
                    }
                }
            }

            
            // Print each row of the matrix.
            for (int i = 0; i < iRows; i++)
            {
                System.Console.Write('[');
                for (int j = 0; j < iColumns; j++)
                {
                    System.Console.Write(' ');
                    String text = String.Format(String.Empty
                        + '{' + '0' + ',' + largestEntryWidth + ':' + 'g'
                        + precision + '}', fM[i, j]);
                    System.Console.Write(text);
                    if ((j + 1) < iColumns)
                    {
                        System.Console.Write(';');
                    }
                    else
                    {
                        System.Console.Write(' ');
                    }
                }
                System.Console.WriteLine(']');
            }

            System.Console.WriteLine(' '); // Empty line
        }

        public void Print2DMatrixFormated()
        {
            Print2DMatrix(m_fArrMembers, m_iRows,m_iColumns, 5);
        }

        public void Print2DMatrixFormated_ABxCD(CMatrix[,] fM_ABxCD,  int precision)
        {
            // Determine the largest entry width in characters
            // so that we can make all columns an equal width.
            int largestEntryWidth = 1;

            for (int l = 0; l < Math.Sqrt(fM_ABxCD.Length); l++) // SubMatrix rows
            {
                for (int m = 0; m < Math.Sqrt(fM_ABxCD.Length); m++) // SubMatrix columns
                {
                    for (int i = 0; i < fM_ABxCD[l, m].m_iRows; i++)
                    {
                        for (int j = 0; j < fM_ABxCD[l, m].m_iColumns; j++)
                        {
                            String text = String.Format(String.Empty
                                + '{' + '0' + ':' + 'g' + precision + '}', fM_ABxCD[l, m].m_fArrMembers[i, j]);
                            if (text.Length > largestEntryWidth)
                            {
                                largestEntryWidth = text.Length;
                            }
                        }
                    }
                }
            }


            // Print each row of the matrix.
            for (int i = 0; i < 2 * fM_ABxCD[0,0].m_iRows; i++)
            {
                System.Console.Write('[');
                for (int j = 0; j <  2 * fM_ABxCD[0,0].m_iColumns; j++)
                {
                    System.Console.Write(' ');

                    String text;

                    if (i < fM_ABxCD[0, 0].m_iRows && j < fM_ABxCD[0, 0].IColumns) // k11
                        text = String.Format(String.Empty
                        + '{' + '0' + ',' + largestEntryWidth + ':' + 'g'
                        + precision + '}', fM_ABxCD[0,0].m_fArrMembers[i, j]);
                    else if(i < fM_ABxCD[0, 0].m_iRows && j >= fM_ABxCD[0, 0].IColumns) // k12
                        text = String.Format(String.Empty
                        + '{' + '0' + ',' + largestEntryWidth + ':' + 'g'
                        + precision + '}', fM_ABxCD[0,1].m_fArrMembers[i, j-fM_ABxCD[0,1].IColumns]);
                    else if(i >= fM_ABxCD[0, 0].m_iRows && j < fM_ABxCD[0, 0].IColumns) // k21
                        text = String.Format(String.Empty
                        + '{' + '0' + ',' + largestEntryWidth + ':' + 'g'
                        + precision + '}', fM_ABxCD[1, 0].m_fArrMembers[i- fM_ABxCD[1,0].IRows, j]);
                    else                                                               // k22
                        text = String.Format(String.Empty
                        + '{' + '0' + ',' + largestEntryWidth + ':' + 'g'
                        + precision + '}', fM_ABxCD[1, 1].m_fArrMembers[i - fM_ABxCD[1, 1].IRows, j - fM_ABxCD[1, 1].IColumns]);

                    System.Console.Write(text);

                    // Separator
                    if ((j + 1) <  2 * fM_ABxCD[0,0].m_iColumns)
                    {
                        System.Console.Write(';');
                    }
                    else
                    {
                        System.Console.Write(' ');
                    }
                }
                System.Console.WriteLine(']');
            }

            System.Console.WriteLine(' '); // Empty line
        }

        public void Print2DMatrixFormated_ABxCD(CMatrix[,] fM_ABxCD)
        {
            Print2DMatrixFormated_ABxCD(fM_ABxCD, 5);
        }

        public string Print2DMatrix(float[,][,] fM, int iSize1, int iSize2)
        {
            // iSize1 = 2
            // iSize2 = 6

            string sOutput = null;
            
            for (int a = 0; a < iSize1; a++) // (k11 + k12) and (k21 and k22)
            {
                for (int i = 0; i < iSize2; i++) // i = 6 - rows
                {
                    // k11
                    for (int j = 0; j < iSize2; j++)
                    {
                        sOutput += fM[a, 0][i, j].ToString();
                        sOutput += "\t"; // New Tab between columns
                    }

                    // k12
                    for (int k = 0; k < iSize2; k++)
                    {
                        sOutput += fM[a, 1][i, k].ToString();
                        sOutput += "\t"; // New Tab between columns
                    }

                    sOutput += "\n"; // New row
                }
            }
            return sOutput;
        }


















    }





    /*

    http://ycouriel.blogspot.com/2010/08/c-matrix-and-vector-representation.html

    In C# there are built in representations of 2D points and 2x2 matrices.
    This is good enough for image processing, but I needed more than that for 3D graphics.

    I wrote classes for:
    3D vector
    3x3 matrix
    4x4 matrix
    Any size matrix
    All the first three objects can be represented by "Any size matrix", but it is much more efficient to use the specific class you need.






    */






















    class Vector3
    {
        public static Vector3 Zero = NewZero();
        public static Vector3 One = NewOne();

        public float x;
        public float y;
        public float z;

        public Vector3()
        {
            x = 0.0f;
            y = 0.0f;
            z = 0.0f;
        }

        public Vector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public Vector3(float xyz)
        {
            this.x = xyz;
            this.y = xyz;
            this.z = xyz;
        }

        public Vector3(Vector3 v)
        {
            this.x = v.x;
            this.y = v.y;
            this.z = v.z;
        }

        public static Vector3 NewZero()
        {
            return new Vector3(0.0f);
        }

        public static Vector3 NewOne()
        {
            return new Vector3(1.0f);
        }

        public float DotProduct(Vector3 other)
        {
            return x * other.x + y * other.y + z * other.z;
        }

        public static Vector3 operator +(Vector3 v1, Vector3 v2)
        {
            return new Vector3(v1.x + v2.x, v1.y + v2.y, v1.z + v2.z);
        }

        public static Vector3 operator -(Vector3 v1, Vector3 v2)
        {
            return new Vector3(v1.x - v2.x, v1.y - v2.y, v1.z - v2.z);
        }

        public static Vector3 operator -(Vector3 v)
        {
            return new Vector3(-v.x, -v.y, -v.z);
        }

        public static Vector3 operator *(Vector3 v, float scalar)
        {
            return new Vector3(v.x * scalar, v.y * scalar, v.z * scalar);
        }

        public static Vector3 operator /(Vector3 v, float scalar)
        {
            return new Vector3(v.x / scalar, v.y / scalar, v.z / scalar);
        }

        public static bool operator ==(Vector3 v1, Vector3 v2)
        {
            return v1.x == v2.x && v1.y == v2.y && v1.z == v2.z;
        }

        public static bool operator !=(Vector3 v1, Vector3 v2)
        {
            return v1.x != v2.x || v1.y != v2.y || v1.z != v2.z;
        }

        public static Vector3 CrossProduct(Vector3 a, Vector3 b)
        {
            return new Vector3(a.y * b.z - a.z * b.y, a.z * b.x - a.x * b.z, a.x * b.y - a.y * b.x);
        }

        public Vector3 Add(Vector3 v)
        {
            x += v.x;
            y += v.y;
            z += v.z;
            return this;
        }

        public float DistanceTo(Vector3 v)
        {
            float dx = this.x - v.x;
            float dy = this.y - v.y;
            float dz = this.z - v.z;
            return (float)Math.Sqrt(dx * dx + dy * dy + dz * dz);
        }

        public float Size()
        {
            return DistanceTo(Vector3.Zero);
        }

        public Vector3 Normalize()
        {
            float size = Size();
            this.x /= size;
            this.y /= size;
            this.z /= size;
            return this;
        }

        public Vector3 Clone()
        {
            return new Vector3(this);
        }

        public override string ToString()
        {
            return "(" + x + ", " + y + ", " + z + ")";
        }
    }






    class Matrix
    {
        public float[,] matrix;
        public int rows;
        public int cols;

        public Matrix(int rows, int cols)
        {
            this.matrix = new float[rows, cols];
            this.rows = rows;
            this.cols = cols;
        }

        public Matrix(float[,] matrix)
        {
            this.matrix = matrix;
            this.rows = matrix.GetLength(0);
            this.cols = matrix.GetLength(1);
        }

        protected static float[,] Multiply(Matrix matrix, float scalar)
        {
            int rows = matrix.rows;
            int cols = matrix.cols;
            float[,] m1 = matrix.matrix;
            float[,] m2 = new float[rows, cols];
            for (int i = 0; i < rows; ++i)
            {
                for (int j = 0; j < cols; ++j)
                {
                    m2[i, j] = m1[i, j] * scalar;
                }
            }
            return m2;
        }

        protected static float[,] Multiply(Matrix matrix1, Matrix matrix2)
        {
            int m1rows = matrix1.rows;
            int m1cols = matrix1.cols;
            int m2rows = matrix2.rows;
            int m2cols = matrix2.cols;
            if (m1cols != m2rows)
            {
                throw new ArgumentException();
            }
            float[,] m1 = matrix1.matrix;
            float[,] m2 = matrix2.matrix;
            float[,] m3 = new float[m1rows, m2cols];
            for (int i = 0; i < m1rows; ++i)
            {
                for (int j = 0; j < m2cols; ++j)
                {
                    float sum = 0;
                    for (int it = 0; it < m1cols; ++it)
                    {
                        sum += m1[i, it] * m2[it, j];
                    }
                    m3[i, j] = sum;
                }
            }
            return m3;
        }

        public static Matrix operator *(Matrix m, float scalar)
        {
            return new Matrix(Multiply(m, scalar));
        }

        public static Matrix operator *(Matrix m1, Matrix m2)
        {
            return new Matrix(Multiply(m1, m2));
        }

        public override string ToString()
        {
            string res = "";
            for (int i = 0; i < rows; ++i)
            {
                if (i > 0)
                {
                    res += "|";
                }
                for (int j = 0; j < cols; ++j)
                {
                    if (j > 0)
                    {
                        res += ",";
                    }
                    res += matrix[i, j];
                }
            }
            return "(" + res + ")";
        }
    }

    class Matrix3 : Matrix
    {
        public Matrix3()
            : base(3, 3)
        {
        }

        public Matrix3(float[,] matrix)
            : base(matrix)
        {
            if (rows != 3 || cols != 3)
            {
                throw new ArgumentException();
            }
        }

        public static Matrix3 I()
        {
            return new Matrix3(new float[,] { 
        { 1.0f, 0.0f, 0.0f }, 
        { 0.0f, 1.0f, 0.0f }, 
        { 0.0f, 0.0f, 1.0f } });
        }

        public static Vector3 operator *(Matrix3 matrix3, Vector3 v)
        {
            float[,] m = matrix3.matrix;
            return new Vector3(
                m[0, 0] * v.x + m[0, 1] * v.y + m[0, 2] * v.z,
                m[1, 0] * v.x + m[1, 1] * v.y + m[1, 2] * v.z,
                m[2, 0] * v.x + m[2, 1] * v.y + m[2, 2] * v.z);
        }

        public static Matrix3 operator *(Matrix3 mat1, Matrix3 mat2)
        {
            float[,] m1 = mat1.matrix;
            float[,] m2 = mat2.matrix;
            float[,] m3 = new float[3, 3];
            m3[0, 0] = m1[0, 0] * m2[0, 0] + m1[0, 1] * m2[1, 0] + m1[0, 2] * m2[2, 0];
            m3[0, 1] = m1[0, 0] * m2[0, 1] + m1[0, 1] * m2[1, 1] + m1[0, 2] * m2[2, 1];
            m3[0, 2] = m1[0, 0] * m2[0, 2] + m1[0, 1] * m2[1, 2] + m1[0, 2] * m2[2, 2];
            m3[1, 0] = m1[1, 0] * m2[0, 0] + m1[1, 1] * m2[1, 0] + m1[1, 2] * m2[2, 0];
            m3[1, 1] = m1[1, 0] * m2[0, 1] + m1[1, 1] * m2[1, 1] + m1[1, 2] * m2[2, 1];
            m3[1, 2] = m1[1, 0] * m2[0, 2] + m1[1, 1] * m2[1, 2] + m1[1, 2] * m2[2, 2];
            m3[2, 0] = m1[2, 0] * m2[0, 0] + m1[2, 1] * m2[1, 0] + m1[2, 2] * m2[2, 0];
            m3[2, 1] = m1[2, 0] * m2[0, 1] + m1[2, 1] * m2[1, 1] + m1[2, 2] * m2[2, 1];
            m3[2, 2] = m1[2, 0] * m2[0, 2] + m1[2, 1] * m2[1, 2] + m1[2, 2] * m2[2, 2];
            return new Matrix3(m3);
        }

        public static Matrix3 operator *(Matrix3 m, float scalar)
        {
            return new Matrix3(Multiply(m, scalar));
        }
    }

    class Matrix4 : Matrix
    {
        public static Matrix4 I = NewI();

        public Matrix4()
            : base(4, 4)
        {
        }

        public Matrix4(float[,] matrix)
            : base(matrix)
        {
            if (rows != 4 || cols != 4)
            {
                throw new ArgumentException();
            }
        }

        public static Matrix4 NewI()
        {
            return new Matrix4(new float[,] { 
        { 1.0f, 0.0f, 0.0f, 0.0f }, 
        { 0.0f, 1.0f, 0.0f, 0.0f }, 
        { 0.0f, 0.0f, 1.0f, 0.0f },
        { 0.0f, 0.0f, 0.0f, 1.0f } });
        }

        public static Vector3 operator *(Matrix4 matrix4, Vector3 v)
        {
            float[,] m = matrix4.matrix;
            float w = m[3, 0] * v.x + m[3, 1] * v.y + m[3, 2] * v.z + m[3, 3];
            return new Vector3(
                (m[0, 0] * v.x + m[0, 1] * v.y + m[0, 2] * v.z + m[0, 3]) / w,
                (m[1, 0] * v.x + m[1, 1] * v.y + m[1, 2] * v.z + m[1, 3]) / w,
                (m[2, 0] * v.x + m[2, 1] * v.y + m[2, 2] * v.z + m[2, 3]) / w
                );
        }

        public static Matrix4 operator *(Matrix4 mat1, Matrix4 mat2)
        {
            float[,] m1 = mat1.matrix;
            float[,] m2 = mat2.matrix;
            float[,] m3 = new float[4, 4];
            m3[0, 0] = m1[0, 0] * m2[0, 0] + m1[0, 1] * m2[1, 0] + m1[0, 2] * m2[2, 0] + m1[0, 3] * m2[3, 0];
            m3[0, 1] = m1[0, 0] * m2[0, 1] + m1[0, 1] * m2[1, 1] + m1[0, 2] * m2[2, 1] + m1[0, 3] * m2[3, 1];
            m3[0, 2] = m1[0, 0] * m2[0, 2] + m1[0, 1] * m2[1, 2] + m1[0, 2] * m2[2, 2] + m1[0, 3] * m2[3, 2];
            m3[0, 3] = m1[0, 0] * m2[0, 3] + m1[0, 1] * m2[1, 3] + m1[0, 2] * m2[2, 3] + m1[0, 3] * m2[3, 3];
            m3[1, 0] = m1[1, 0] * m2[0, 0] + m1[1, 1] * m2[1, 0] + m1[1, 2] * m2[2, 0] + m1[1, 3] * m2[3, 0];
            m3[1, 1] = m1[1, 0] * m2[0, 1] + m1[1, 1] * m2[1, 1] + m1[1, 2] * m2[2, 1] + m1[1, 3] * m2[3, 1];
            m3[1, 2] = m1[1, 0] * m2[0, 2] + m1[1, 1] * m2[1, 2] + m1[1, 2] * m2[2, 2] + m1[1, 3] * m2[3, 2];
            m3[1, 3] = m1[1, 0] * m2[0, 3] + m1[1, 1] * m2[1, 3] + m1[1, 2] * m2[2, 3] + m1[1, 3] * m2[3, 3];
            m3[2, 0] = m1[2, 0] * m2[0, 0] + m1[2, 1] * m2[1, 0] + m1[2, 2] * m2[2, 0] + m1[2, 3] * m2[3, 0];
            m3[2, 1] = m1[2, 0] * m2[0, 1] + m1[2, 1] * m2[1, 1] + m1[2, 2] * m2[2, 1] + m1[2, 3] * m2[3, 1];
            m3[2, 2] = m1[2, 0] * m2[0, 2] + m1[2, 1] * m2[1, 2] + m1[2, 2] * m2[2, 2] + m1[2, 3] * m2[3, 2];
            m3[2, 3] = m1[2, 0] * m2[0, 3] + m1[2, 1] * m2[1, 3] + m1[2, 2] * m2[2, 3] + m1[2, 3] * m2[3, 3];
            m3[3, 0] = m1[3, 0] * m2[0, 0] + m1[3, 1] * m2[1, 0] + m1[3, 2] * m2[2, 0] + m1[3, 3] * m2[3, 0];
            m3[3, 1] = m1[3, 0] * m2[0, 1] + m1[3, 1] * m2[1, 1] + m1[3, 2] * m2[2, 1] + m1[3, 3] * m2[3, 1];
            m3[3, 2] = m1[3, 0] * m2[0, 2] + m1[3, 1] * m2[1, 2] + m1[3, 2] * m2[2, 2] + m1[3, 3] * m2[3, 2];
            m3[3, 3] = m1[3, 0] * m2[0, 3] + m1[3, 1] * m2[1, 3] + m1[3, 2] * m2[2, 3] + m1[3, 3] * m2[3, 3];
            return new Matrix4(m3);
        }

        public static Matrix4 operator *(Matrix4 m, float scalar)
        {
            return new Matrix4(Multiply(m, scalar));
        }
    }
}
