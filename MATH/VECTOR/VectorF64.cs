using System;
using System.Collections.Generic;
using System.Text;

namespace MATH
{
    // Multi-dimensional vector with 64-bit floating-point components:

    public class VectorF64
    {
        private double[] components;




        public int Dimensions()
        {
            if (null == this.components)
            {
                return (0);
            }

            return (this.components.Length);
        }





        public VectorF64()
        {
            this.components = null;
        }




        public VectorF64(params double[] paramValues)
        {
            this.components = null;

            if (null == paramValues)
            {
                return;
            }

            int dimensions = paramValues.Length;

            this.components = new double[dimensions];

            for (int i = 0; i < dimensions; i++)
            {
                this.components[i] = paramValues[i];
            }
        }




        public VectorF64(VectorF64 other)
        {
            this.components = null;

            if (null == other)
            {
                return;
            }

            if (null == other.components)
            {
                return;
            }

            int dimensions = other.Dimensions();

            this.components = new double[dimensions];

            for (int i = 0; i < dimensions; i++)
            {
                this.components[i] = other.components[i];
            }
        }




        public double this[int index]
        {
            get
            {
                if (null == this.components)
                {
                    return (0.0);
                }

                if
                (
                       (index >= 0)
                    && (index < this.components.Length)
                )
                {
                    return (this.components[index]);
                }

                return (0.0);
            }

            set
            {
                if (null == this.components)
                {
                    return;
                }

                if
                (
                       (index >= 0)
                    && (index < this.components.Length)
                )
                {
                    this.components[index] = value;
                }
            }
        }




        public void Write(int precision)
        {
            if (null == this.components)
            {
                System.Console.Write(String.Empty + '(' + ' ' + ')');
                return;
            }

            int dimensions = this.Dimensions();
            if (0 == dimensions)
            {
                System.Console.Write(String.Empty + '(' + ' ' + ')');
                return;
            }

            // Determine the largest component width in characters
            // so that we can make all components an equal width.
            int largestComponentWidth = 1;
            for (int i = 0; i < dimensions; i++)
            {
                // { index [,minwidth] [:typeCode[precision]] }
                //      (minwidth<0) means left-justify
                String text = String.Format(String.Empty + '{' + '0' + ':'
                    + 'g' + precision + '}', this[i]);
                if (text.Length > largestComponentWidth)
                {
                    largestComponentWidth = text.Length;
                }
            }

            System.Console.Write('(');
            for (int i = 0; i < dimensions; i++)
            {
                System.Console.Write(' ');
                String text =
                    String.Format(String.Empty + '{' + '0' + ','
                    + largestComponentWidth + ':' + 'g' + precision + '}',
                    this[i]);
                System.Console.Write(text);
                if ((i + 1) < dimensions)
                {
                    System.Console.Write(',');
                }
                else
                {
                    System.Console.Write(' ');
                }
            }
            System.Console.Write(')');
        }




        public void WriteLine(int precision)
        {
            this.Write(precision);
            System.Console.WriteLine();
        }




        public void WriteLine()
        {
            const int defaultPrecision = 8;
            this.Write(defaultPrecision);
            System.Console.WriteLine();
        }




        public static VectorF64 Zero(int dimensions)
        {
            VectorF64 zero = new VectorF64();

            zero.components = new double[dimensions];

            for (int i = 0; i < dimensions; i++)
            {
                zero[i] = 0.0;
            }

            return (zero);
        }




        public static VectorF64 operator +(VectorF64 a, VectorF64 b)
        {
            if ((null == a) || (null == b))
            {
                return (new VectorF64()); // Vector not specified.
            }

            if ((null == a.components) || (null == b.components))
            {
                return (new VectorF64()); // Vector is empty.
            }

            if (a.Dimensions() != b.Dimensions())
            {
                return (new VectorF64()); // Vectors not the same size.
            }

            int dimensions = a.Dimensions();

            VectorF64 result = VectorF64.Zero(dimensions);

            for (int i = 0; i < dimensions; i++)
            {
                result[i] = a[i] + b[i];
            }

            return (result);
        }




        public static VectorF64 operator -(VectorF64 a, VectorF64 b)
        {
            if ((null == a) || (null == b))
            {
                return (new VectorF64()); // Vector not specified.
            }

            if ((null == a.components) || (null == b.components))
            {
                return (new VectorF64()); // Vector is empty.
            }

            if (a.Dimensions() != b.Dimensions())
            {
                return (new VectorF64()); // Vectors not the same size.
            }

            int dimensions = a.Dimensions();

            VectorF64 result = VectorF64.Zero(dimensions);

            for (int i = 0; i < dimensions; i++)
            {
                result[i] = a[i] - b[i];
            }

            return (result);
        }




        public static VectorF64 operator -(VectorF64 a)
        {
            if (null == a)
            {
                return (new VectorF64()); // Vector not specified.
            }

            if (null == a.components)
            {
                return (new VectorF64()); // Vector is empty.
            }

            int dimensions = a.Dimensions();

            VectorF64 result = VectorF64.Zero(dimensions);

            for (int i = 0; i < dimensions; i++)
            {
                result[i] = (-(a[i]));
            }

            return (result);
        }




        public static VectorF64 operator *(double scale, VectorF64 a)
        {
            if (null == a)
            {
                return (new VectorF64()); // Vector not specified.
            }

            if (null == a.components)
            {
                return (new VectorF64()); // Vector is empty.
            }

            int dimensions = a.Dimensions();

            VectorF64 result = VectorF64.Zero(dimensions);

            for (int i = 0; i < dimensions; i++)
            {
                result[i] = scale * a[i];
            }

            return (result);
        }




        public static VectorF64 operator *(VectorF64 a, double scale)
        {
            if (null == a)
            {
                return (new VectorF64()); // Vector not specified.
            }

            if (null == a.components)
            {
                return (new VectorF64()); // Vector is empty.
            }

            int dimensions = a.Dimensions();

            VectorF64 result = VectorF64.Zero(dimensions);

            for (int i = 0; i < dimensions; i++)
            {
                result[i] = scale * a[i];
            }

            return (result);
        }




        public static VectorF64 BasisVector(int dimensions, int componentIndex)
        {
            if (dimensions < 0)
            {
                // Invalid number of dimensions specified.
                return (new VectorF64());
            }

            VectorF64 basisVector = VectorF64.Zero(dimensions);

            if ((componentIndex >= 0) && (componentIndex < dimensions))
            {
                basisVector[componentIndex] = 1.0;
            }

            return (basisVector);
        }




        public double Length()
        {
            if (null == this.components)
            {
                return (0.0); // Vector empty.
            }

            int dimensions = this.Dimensions();

            double sumOfSquares = 0.0;
            for (int i = 0; i < dimensions; i++)
            {
                sumOfSquares += (this[i] * this[i]);
            }

            double length = Math.Sqrt(sumOfSquares);

            return (length);
        }




        public static double Length(VectorF64 a)
        {
            if (null == a)
            {
                return (0.0); // Vector not specified.
            }

            if (null == a.components)
            {
                return (0.0); // Vector is empty.
            }

            return (a.Length());
        }




        public VectorF64 Normalize()
        {
            if (null == this.components)
            {
                return (new VectorF64()); // Vector is empty.
            }

            int dimensions = this.Dimensions();

            double length = this.Length();

            if (0.0 == length)
            {
                // Length is zero.  Cannot normalize.
                return (VectorF64.Zero(dimensions));
            }

            VectorF64 result = VectorF64.Zero(dimensions);

            double factor = (1.0 / length);
            for (int i = 0; i < dimensions; i++)
            {
                result[i] *= factor;
            }

            return (result);
        }




        public static VectorF64 Normalize(VectorF64 v)
        {
            if (null == v)
            {
                return (new VectorF64()); // Vector not specified.
            }

            if (null == v.components)
            {
                return (new VectorF64()); // Vector is empty.
            }

            return (v.Normalize());
        }




        public static double Dot(VectorF64 a, VectorF64 b)
        {
            if ((null == a) || (null == b))
            {
                return (0.0); // Vector not specified.
            }

            if ((null == a.components) || (null == b.components))
            {
                return (0.0); // Vector is empty.
            }

            if (a.Dimensions() != b.Dimensions())
            {
                return (0.0); // Vectors not the same size.
            }

            int dimensions = a.Dimensions();

            double dotProduct = 0.0;
            for (int i = 0; i < dimensions; i++)
            {
                dotProduct += (a[i] * b[i]);
            }

            return (dotProduct);
        }




        public static VectorF64 Cross(params VectorF64[] parameterArray)
        {
            if (null == parameterArray)
            {
                return (new VectorF64());
            }

            int totalVectors = parameterArray.Length;

            // The number of dimensions of the space must be one larger
            // than the number of specified vectors.
            int dimensions = (1 + totalVectors);

            if (dimensions < 2)
            {
                // The cross product does not exist for space with
                // fewer than two dimensions.
                return (new VectorF64());
            }

            // All specified vectors must exist and must have a number
            // of components equal to the number of dimensions of the space.
            for (int k = 0; k < totalVectors; k++)
            {
                if (null == parameterArray[k])
                {
                    // Vector missing.
                    return (new VectorF64());
                }
                else if (null == parameterArray[k].components)
                {
                    // Vector is empty.
                    return (new VectorF64());
                }
                else if (parameterArray[k].components.Length != dimensions)
                {
                    // Vector has the wrong number of components.
                    return (new VectorF64());
                }
            }

            // Form a (d*d) matrix with the (d-1) supplied vectors
            // forming the final (d-1) rows of the matrix.
            MatrixF64 matrix = MatrixF64.Zero(dimensions, dimensions);

            for (int i = 1; i < dimensions; i++)
            {
                VectorF64 rowVector = parameterArray[i - 1];
                for (int j = 0; j < dimensions; j++)
                {
                    matrix[i, j] = rowVector[j];
                }
            }

            VectorF64 result = VectorF64.Zero(dimensions);

            // For each component of the result vector, set the first
            // row of the matrix to a basis vector, compute the matrix
            // determinant, and use the resulting value as the 
            // component of the result vector.  This is inefficient,
            // due to the determinant being computed (d) times.
            // Clearly it would be better to have explicit formulas
            // for 2-dimensional, 3-dimensional, and 4-dimensional
            // cross-products.
            for (int i = 0; i < dimensions; i++)
            {
                // Set the first row of the matrix to be the basis
                // vector for the coordinate axis with index (i).
                for (int j = 0; j < dimensions; j++)
                {
                    matrix[0, j] = 0.0;
                }
                matrix[0, i] = 1.0;

                // Compute the determinant of the modified matrix.
                double determinant = matrix.Determinant();

                // Set component (i) of the result to the determinant
                // that was computed.
                result[i] = determinant;
            }

            return (result);
        }




        public static bool Parallel(VectorF64 a, VectorF64 b)
        {
            // Smallest normalized float : 1.1754943e-38
            // Smallest normalized double: 2.2250738585072020e-308  
            double nonZeroThreshold = 1.0e-38; // conservative for double
            // double: (52+1)-bit mantissa; log10(2^53)=15.95 decimal digits
            double fractionalDifferenceThreshold = 1.0e-14; // conservative

            if ((null == a) || (null == b))
            {
                return (false); // Vector is not specified.
            }

            if ((null == a.components) || (null == b.components))
            {
                return (false); // Vector is empty.
            }

            if (a.Dimensions() != b.Dimensions())
            {
                return (false); // Vectors not the same size.
            }

            double lengthA = a.Length();
            if (lengthA <= nonZeroThreshold)
            {
                return (false);
            }

            double lengthB = b.Length();
            if (lengthB <= nonZeroThreshold)
            {
                return (false);
            }

            double oneImpliesParallel =
                Math.Abs(VectorF64.Dot(a, b)) / (lengthA * lengthB);

            double absoluteDifferenceFromOne =
                Math.Abs(oneImpliesParallel - 1.0);

            if (absoluteDifferenceFromOne <= fractionalDifferenceThreshold)
            {
                return (true);
            }

            return (false);
        }




        public static bool Perpendicular(VectorF64 a, VectorF64 b)
        {
            // Smallest normalized float : 1.1754943e-38
            // Smallest normalized double: 2.2250738585072020e-308  
            double nonZeroThreshold = 1.0e-38; // conservative for double
            // double: (52+1)-bit mantissa; log10(2^53)=15.95 decimal digits
            double fractionalDifferenceThreshold = 1.0e-14; // conservative

            if ((null == a) || (null == b))
            {
                return (false); // Vector is not specified.
            }

            if ((null == a.components) || (null == b.components))
            {
                return (false); // Vector is empty.
            }

            if (a.Dimensions() != b.Dimensions())
            {
                return (false); // Vectors not the same size.
            }

            double lengthA = a.Length();
            if (lengthA <= nonZeroThreshold)
            {
                return (false);
            }

            double lengthB = b.Length();
            if (lengthB <= nonZeroThreshold)
            {
                return (false);
            }

            double zeroImpliesPerpendicular =
                Math.Abs(VectorF64.Dot(a, b)) / (lengthA * lengthB);

            double absoluteDifferenceFromZero =
                Math.Abs(zeroImpliesPerpendicular);

            if (absoluteDifferenceFromZero <= fractionalDifferenceThreshold)
            {
                return (true);
            }

            return (false);
        }




        public static void Test()
        {
            System.Console.WriteLine(String.Empty.PadRight(78, '#'));
            System.Console.WriteLine(Environment.NewLine);
            System.Console.WriteLine("Test of VectorF64");
            System.Console.WriteLine(Environment.NewLine);
            System.Console.WriteLine(String.Empty.PadRight(78, '#'));
            System.Console.WriteLine(Environment.NewLine);




            System.Console.WriteLine(Environment.NewLine);
            System.Console.WriteLine("Test of vector initialization:");
            System.Console.WriteLine(Environment.NewLine);

            // A 3-dimensional vector with 64-bit floating-point components:
            VectorF64 v3 = new VectorF64(0.0, 1.0, 2.0);
            v3.WriteLine(); // ( 0, 1, 2 )
            System.Console.WriteLine(Environment.NewLine);

            // A 4-dimensional vector with 64-bit floating-point components:
            VectorF64 v4 = new VectorF64(0.0, 1.0, 2.0, 3.0);
            v4.WriteLine(); // ( 0, 1, 2, 3 )
            System.Console.WriteLine(Environment.NewLine);



            System.Console.WriteLine(Environment.NewLine);
            System.Console.WriteLine("Test of zero vector:");
            System.Console.WriteLine(Environment.NewLine);

            // An 8-dimensional vector with all 8 64-bit floating-point
            // components set to zero:
            VectorF64 z = VectorF64.Zero(8);
            z.WriteLine(); // ( 0, 0, 0, 0, 0, 0, 0, 0 )
            System.Console.WriteLine(Environment.NewLine);




            // Examples of vector addition, subtraction, and scaling:

            System.Console.WriteLine(Environment.NewLine);
            System.Console.WriteLine("Test of vector addition, subtraction, and scaling:");
            System.Console.WriteLine(Environment.NewLine);

            VectorF64 a = new VectorF64(0.0, 1.0, 2.0, 3.0);
            a.WriteLine(); // ( 0, 1, 2, 3 )
            System.Console.WriteLine(Environment.NewLine);

            VectorF64 b = new VectorF64(3.0, 2.0, 1.0, 0.0);
            b.WriteLine(); // ( 3, 2, 1, 0 )
            System.Console.WriteLine(Environment.NewLine);

            VectorF64 c = new VectorF64();
            c.WriteLine(); // ( )
            System.Console.WriteLine(Environment.NewLine);

            c = a + b;
            c.WriteLine(); // ( 3, 3, 3, 3 )
            System.Console.WriteLine(Environment.NewLine);

            c = a - b;
            c.WriteLine(); // ( -3, -1,  1,  3 )
            System.Console.WriteLine(Environment.NewLine);

            c = -b;
            c.WriteLine(); // ( -3, -2, -1,  0 )
            System.Console.WriteLine(Environment.NewLine);

            c = 3.0 * a;
            c.WriteLine(); // ( 0, 3, 6, 9 )
            System.Console.WriteLine(Environment.NewLine);



            // The 4 basis vectors of 4-dimensional space, 
            // each with 4 64-bit floating-point components:

            System.Console.WriteLine(Environment.NewLine);
            System.Console.WriteLine("Test of unit basis vectors:");
            System.Console.WriteLine(Environment.NewLine);

            VectorF64 b0 = VectorF64.BasisVector(4, 0);
            b0.WriteLine(); // ( 1, 0, 0, 0 )
            System.Console.WriteLine(Environment.NewLine);

            VectorF64 b1 = VectorF64.BasisVector(4, 1);
            b1.WriteLine(); // ( 0, 1, 0, 0 )
            System.Console.WriteLine(Environment.NewLine);

            VectorF64 b2 = VectorF64.BasisVector(4, 2);
            b2.WriteLine(); // ( 0, 0, 1, 0 )
            System.Console.WriteLine(Environment.NewLine);

            VectorF64 b3 = VectorF64.BasisVector(4, 3);
            b3.WriteLine(); // ( 0, 0, 0, 1 )
            System.Console.WriteLine(Environment.NewLine);




            // The following two vectors are equivalent:

            // A 4-dimensional vector with 64-bit floating-point components:
            VectorF64 va = new VectorF64(0.1, 1.1, 2.2, 3.3);
            va.WriteLine(); // ( 0.1, 1.1, 2.2, 3.3 )
            System.Console.WriteLine(Environment.NewLine);

            // A 4-dimensional vector formed by scaling the 4 independent
            // basis vectors for 4-dimensional space:
            VectorF64 vb =
                  0.1 * VectorF64.BasisVector(4, 0)
                + 1.1 * VectorF64.BasisVector(4, 1)
                + 2.2 * VectorF64.BasisVector(4, 2)
                + 3.3 * VectorF64.BasisVector(4, 3);
            vb.WriteLine(); // ( 0.1, 1.1, 2.2, 3.3 )
            System.Console.WriteLine(Environment.NewLine);





            // Example of vector length:

            System.Console.WriteLine(Environment.NewLine);
            System.Console.WriteLine("Test of vector length:");
            System.Console.WriteLine(Environment.NewLine);

            // A 6-dimensional vector representing a point (p):
            VectorF64 p = new VectorF64(0.0, 1.0, 2.0, 3.0, 4.0, 5.0);
            p.WriteLine(); // ( 0, 1, 2, 3, 4, 5 )
            System.Console.WriteLine(Environment.NewLine);

            // A 6-dimensional vector representing a point (q):
            VectorF64 q = new VectorF64(-5.0, 4.0, -3.0, 2.0, -1.0, 0.0);
            q.WriteLine(); // ( -5,  4, -3,  2, -1,  0 )
            System.Console.WriteLine(Environment.NewLine);

            // A 6-dimensional vector representing the displacement
            // from point (p) to point (q):
            VectorF64 r = q - p;
            r.WriteLine(); // ( -5,  3, -5, -1, -5, -5 )
            System.Console.WriteLine(Environment.NewLine);

            // The distance between point (p) and point (q)
            // in 6-dimensional space:
            double distance = r.Length();
            System.Console.WriteLine(distance); // 10.4880884817015
            System.Console.WriteLine(Environment.NewLine);





            // Example of testing for parallel vectors:

            System.Console.WriteLine(Environment.NewLine);
            System.Console.WriteLine("Test of parallel vectors:");
            System.Console.WriteLine(Environment.NewLine);

            // A 6-dimensional vector:
            VectorF64 vf = new VectorF64(0.0, 1.0, 2.0, 3.0, 4.0, 5.0);
            vf.WriteLine(); // ( 0, 1, 2, 3, 4, 5 )
            System.Console.WriteLine(Environment.NewLine);

            // A 6-dimensional vector:
            VectorF64 vg = new VectorF64(0.0, -2.0, -4.0, -6.0, -8.0, -10.0);
            vg.WriteLine(); // (   0,  -2,  -4,  -6,  -8, -10 )
            System.Console.WriteLine(Environment.NewLine);

            // Determine if the specified vectors are parallel 
            // (or ""anti-parallel""):
            bool parallel = VectorF64.Parallel(vf, vg);
            System.Console.WriteLine(parallel); // True
            System.Console.WriteLine(Environment.NewLine);

            // Add a non-negligible displacement to a component of a vector:
            vf[0] += 1.0e-5;
            vf.WriteLine(); // ( 1E-05,     1,     2,     3,     4,     5 )
            System.Console.WriteLine(Environment.NewLine);

            // Determine if the specified vectors are parallel 
            // (or ""anti-parallel""):
            parallel = VectorF64.Parallel(vf, vg);
            System.Console.WriteLine(parallel); // False
            System.Console.WriteLine(Environment.NewLine);



            // Example of testing for perpendicular vectors:

            System.Console.WriteLine(Environment.NewLine);
            System.Console.WriteLine("Test of perpendicular vectors:");
            System.Console.WriteLine(Environment.NewLine);

            // A 6-dimensional vector:
            VectorF64 vf2 = new VectorF64(0.0, 1.0, 2.0, 0.0, 4.0, 5.0);
            vf2.WriteLine(); // ( 0, 1, 2, 0, 4, 5 )
            System.Console.WriteLine(Environment.NewLine);

            // A 6-dimensional vector:
            VectorF64 vg2 = new VectorF64(10.0, 0.0, 0.0, -5.0, 0.0, 0.0);
            vg2.WriteLine(); // ( 10,  0,  0, -5,  0,  0 )
            System.Console.WriteLine(Environment.NewLine);

            // Determine if the specified vectors are perpendicular
            bool perpendicular = VectorF64.Perpendicular(vf2, vg2);
            System.Console.WriteLine(perpendicular); // True
            System.Console.WriteLine(Environment.NewLine);

            // Add a non-negligible displacement to a component of a vector:
            vf2[0] += 1.0e-13;
            vf2.WriteLine(); // ( 1E-13,    1,    2,    0,    4,    5 )
            System.Console.WriteLine(Environment.NewLine);

            // Determine if the specified vectors are perpendicular
            perpendicular = VectorF64.Perpendicular(vf2, vg2);
            System.Console.WriteLine(perpendicular); // False
            System.Console.WriteLine(Environment.NewLine);



            // Examples of cross products:

            System.Console.WriteLine(Environment.NewLine);
            System.Console.WriteLine("Test of cross products:");
            System.Console.WriteLine(Environment.NewLine);

            // Three-dimensional example:

            VectorF64 vcp1x3a = new VectorF64(1.0, 2.0, 3.0);
            VectorF64 vcp1x3b = new VectorF64(3.0, 1.0, 2.0);

            VectorF64 vcp1x3 = VectorF64.Cross(vcp1x3a, vcp1x3b);
            vcp1x3.WriteLine(); // (  1,  7, -5 )
            System.Console.WriteLine(Environment.NewLine);

            // Verify that the result is perpendicular to the original
            // vectors:
            System.Console.WriteLine(VectorF64.Perpendicular(vcp1x3, vcp1x3a));
            System.Console.WriteLine(VectorF64.Perpendicular(vcp1x3, vcp1x3b));
            // True, True

            // Four-dimensional example:

            VectorF64 vcp1x4a = new VectorF64(1.0, 2.0, 3.0, 4.0);
            VectorF64 vcp1x4b = new VectorF64(4.0, 1.0, 2.0, 3.0);
            VectorF64 vcp1x4c = new VectorF64(3.0, 4.0, 1.0, 2.0);

            VectorF64 vcp1x4 = VectorF64.Cross(vcp1x4a, vcp1x4b, vcp1x4c);
            vcp1x4.WriteLine(); // (   4,   4,  44, -36 )
            System.Console.WriteLine(Environment.NewLine);

            // Verify that the result is perpendicular to the original
            // vectors:
            System.Console.WriteLine(VectorF64.Perpendicular(vcp1x4, vcp1x4a));
            System.Console.WriteLine(VectorF64.Perpendicular(vcp1x4, vcp1x4b));
            System.Console.WriteLine(VectorF64.Perpendicular(vcp1x4, vcp1x4c));
            // True, True, True

            System.Console.WriteLine(Environment.NewLine);
            System.Console.WriteLine(String.Empty.PadRight(78, '#'));
            System.Console.WriteLine(Environment.NewLine);
        }





    }
}
