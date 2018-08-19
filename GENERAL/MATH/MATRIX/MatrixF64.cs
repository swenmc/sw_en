using System;
using System.Collections.Generic;
using System.Text;

// Matrix with 64-bit floating-point entries:
namespace MATH
{
    public class MatrixF64
    {
        private int totalRows;

        /*public int TotalRows
        {
            get { return totalRows; }
            set { totalRows = value; }
        }*/


        private int totalColumns;

        /*public int TotalColumns
        {
            get { return totalColumns; }
            set { totalColumns = value; }
        }*/



        private double[] entries;

        public double[] Entries
        {
            get { return entries; }
            set { entries = value; }
        }



        // matrix[ row, column ] = entries[ (totalColumns * row) + column ]




        public int Columns()
        {
            return (this.totalColumns);
        }




        public int Rows()
        {
            return (this.totalRows);
        }


        public MatrixF64()
        {
            this.totalRows = 0;
            this.totalColumns = 0;
            this.entries = null;
        }




        public MatrixF64(int rows, int columns, params double[] paramValues)
        {
            this.totalRows = 0;
            this.totalColumns = 0;
            this.entries = null;

            if (rows <= 0)
            {
                return; // Specified an invalid row count.
            }
            if (columns <= 0)
            {
                return; // Specified an invalid column count.
            }

            int totalEntries = (rows * columns);

            this.totalRows = rows;
            this.totalColumns = columns;
            this.entries = new double[totalEntries];

            for (int k = 0; k < totalEntries; k++)
            {
                this.entries[k] = 0.0;
            }

            if (null == paramValues)
            {
                return; // No entries specified.
            }

            int totalParamValues = paramValues.Length;
            if (totalParamValues != totalEntries)
            {
                // The number of specified entries does not match the
                // total number of matrix entries.
                return;
            }

            for (int k = 0; k < totalParamValues; k++)
            {
                this.entries[k] = paramValues[k];
            }
        }




        public MatrixF64(MatrixF64 other)
        {
            this.totalRows = 0;
            this.totalColumns = 0;
            this.entries = null;

            if (null == other)
            {
                return;
            }

            if
            (
                   (null == other.entries)
                || (0 == other.totalRows)
                || (0 == other.totalColumns)
            )
            {
                return;
            }

            int totalEntries = (other.totalRows * other.totalColumns);

            if (other.entries.Length != totalEntries)
            {
                return;
            }

            this.totalRows = other.totalRows;
            this.totalColumns = other.totalColumns;
            this.entries = new double[totalEntries];

            for (int k = 0; k < totalEntries; k++)
            {
                this.entries[k] = other.entries[k];
            }
        }




        public double this[int row, int column]
        {
            get
            {
                if
                (
                       (this.totalRows > 0)
                    && (this.totalColumns > 0)
                    && (null != this.entries)
                    && (row >= 0)
                    && (row < this.totalRows)
                    && (column >= 0)
                    && (column < this.totalColumns)
                )
                {
                    int k = (this.totalColumns * row) + column;
                    if ((k >= 0) && (k < this.entries.Length))
                    {
                        return (this.entries[k]);
                    }
                }

                return (0.0);
            }

            set
            {
                if
                (
                       (this.totalRows > 0)
                    && (this.totalColumns > 0)
                    && (null != this.entries)
                    && (row >= 0)
                    && (row < this.totalRows)
                    && (column >= 0)
                    && (column < this.totalColumns)
                )
                {
                    int k = (this.totalColumns * row) + column;
                    if ((k >= 0) && (k < this.entries.Length))
                    {
                        this.entries[k] = value;
                    }
                }
            }
        }




        public void WriteLine(int precision)
        {
            if
            (
                   (this.totalRows <= 0)
                || (this.totalColumns <= 0)
                || (null == this.entries)
            )
            {
                System.Console.WriteLine(String.Empty + '[' + ' ' + ']');
                return;
            }

            // Determine the largest entry width in characters
            // so that we can make all columns an equal width.
            int largestEntryWidth = 1;
            for (int i = 0; i < this.totalRows; i++)
            {
                for (int j = 0; j < this.totalColumns; j++)
                {
                    // { index [,minwidth] [:typeCode[precision]] }
                    //      (minwidth<0) means left-justify
                    String text = String.Format(String.Empty
                        + '{' + '0' + ':' + 'g' + precision + '}', this[i, j]);
                    if (text.Length > largestEntryWidth)
                    {
                        largestEntryWidth = text.Length;
                    }
                }
            }

            // Print each row of the matrix.
            for (int i = 0; i < this.totalRows; i++)
            {
                System.Console.Write('[');
                for (int j = 0; j < this.totalColumns; j++)
                {
                    System.Console.Write(' ');
                    String text = String.Format(String.Empty
                        + '{' + '0' + ',' + largestEntryWidth + ':' + 'g'
                        + precision + '}', this[i, j]);
                    System.Console.Write(text);
                    if ((j + 1) < this.totalColumns)
                    {
                        System.Console.Write(',');
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




        public void WriteLine()
        {
            const int defaultPrecision = 8;
            this.WriteLine(defaultPrecision);
        }




        public MatrixF64 SnapToNearestInteger(double tolerance)
        {
            // example tolerance: 1.0e-5

            MatrixF64 result = new MatrixF64(this);

            for (int i = 0; i < result.totalRows; i++)
            {
                for (int j = 0; j < result.totalColumns; j++)
                {
                    double doubleValue = result[i, j];
                    // double only has 53 bits for mantissa, so we'll
                    // conservatively limit our snap attempts to values
                    // less than 2^50 = 1125899906842624.
                    long maxSnapValue = 1125899906842624;
                    if (Math.Abs(doubleValue) <= maxSnapValue)
                    {
                        long longValue = (long)(doubleValue + 0.5);
                        if (doubleValue < (-0.5))
                        {
                            // The interval [-(N).499,-(N-1).501] should be
                            // shifted so that truncation produces -(N).
                            longValue = (long)(doubleValue - 0.5);
                        }
                        double absoluteDifference =
                            Math.Abs(doubleValue - (double)longValue);
                        if (absoluteDifference < tolerance)
                        {
                            result[i, j] = (double)longValue;
                        }
                    }
                }
            }
            return (result);
        }




        public static bool EqualsWithinTolerance
            (
            MatrixF64 a,
            MatrixF64 b,
            double tolerance
            )
        {
            if ((null == a) || (null == b))
            {
                return (false);
            }

            if
            (
                   (null == a.entries)
                || (null == b.entries)
                || (a.totalRows != b.totalRows)
                || (a.totalColumns != b.totalColumns)
            )
            {
                return (false);
            }

            for (int i = 0; i < a.totalRows; i++)
            {
                for (int j = 0; j < a.totalColumns; j++)
                {
                    double absoluteDifference =
                        Math.Abs(a[i, j] - b[i, j]);
                    if (absoluteDifference > tolerance)
                    {
                        return (false);
                    }
                }
            }
            return (true);
        }




        public static MatrixF64 Zero(int rows, int columns)
        {
            MatrixF64 zero = new MatrixF64(rows, columns);
            // The constructor above sets all entries to zero.
            return (zero);
        }




        public static MatrixF64 operator +(MatrixF64 a, MatrixF64 b)
        {
            if ((null == a) || (null == b))
            {
                return (null);
            }

            MatrixF64 result = MatrixF64.Zero(a.totalRows, a.totalColumns);

            if
            (
                   (null == a.entries)
                || (null == b.entries)
                || (a.totalRows != b.totalRows)
                || (a.totalColumns != b.totalColumns)
            )
            {
                return (result);
            }

            for (int i = 0; i < a.totalRows; i++)
            {
                for (int j = 0; j < a.totalColumns; j++)
                {
                    result[i, j] = a[i, j] + b[i, j];
                }
            }

            return (result);
        }




        public static MatrixF64 operator -(MatrixF64 a, MatrixF64 b)
        {
            if ((null == a) || (null == b))
            {
                return (null);
            }

            MatrixF64 result = MatrixF64.Zero(a.totalRows, a.totalColumns);

            if
            (
                   (null == a.entries)
                || (null == b.entries)
                || (a.totalRows != b.totalRows)
                || (a.totalColumns != b.totalColumns)
            )
            {
                return (result);
            }

            for (int i = 0; i < a.totalRows; i++)
            {
                for (int j = 0; j < a.totalColumns; j++)
                {
                    result[i, j] = a[i, j] - b[i, j];
                }
            }

            return (result);
        }




        public static MatrixF64 operator -(MatrixF64 a)
        {
            if (null == a)
            {
                return (null);
            }

            MatrixF64 result = MatrixF64.Zero(a.totalRows, a.totalColumns);

            if (null == a.entries)
            {
                return (result);
            }

            for (int i = 0; i < a.totalRows; i++)
            {
                for (int j = 0; j < a.totalColumns; j++)
                {
                    result[i, j] = (-(a[i, j]));
                }
            }

            return (result);
        }




        public static MatrixF64 operator *(double scale, MatrixF64 a)
        {
            if (null == a)
            {
                return (null);
            }

            MatrixF64 result = MatrixF64.Zero(a.totalRows, a.totalColumns);

            if (null == a.entries)
            {
                return (result);
            }

            for (int i = 0; i < a.totalRows; i++)
            {
                for (int j = 0; j < a.totalColumns; j++)
                {
                    result[i, j] = scale * a[i, j];
                }
            }

            return (result);
        }




        public static MatrixF64 operator *(MatrixF64 a, double scale)
        {
            if (null == a)
            {
                return (null);
            }

            MatrixF64 result = MatrixF64.Zero(a.totalRows, a.totalColumns);

            if (null == a.entries)
            {
                return (result);
            }

            for (int i = 0; i < a.totalRows; i++)
            {
                for (int j = 0; j < a.totalColumns; j++)
                {
                    result[i, j] = scale * a[i, j];
                }
            }

            return (result);
        }




        public static MatrixF64 operator *(MatrixF64 a, MatrixF64 b)
        {
            if ((null == a) || (null == b))
            {
                return (null);
            }

            MatrixF64 result = MatrixF64.Zero(a.totalRows, b.totalColumns);

            // The number of columns of the first matrix must be equal
            // to the number of rows of the second matrix.
            if
            (
                   (null == a.entries)
                || (null == b.entries)
                || (a.totalColumns != b.totalRows)
            )
            {
                return (result);
            }

            // Entry (i,j) of the result is equal to the dot product
            // of row (i) of (a) and column (j) of (b).
            for (int i = 0; i < a.totalRows; i++)
            {
                for (int j = 0; j < b.totalColumns; j++)
                {
                    double dotProduct = 0.0;
                    for (int k = 0; k < a.totalColumns; k++)
                    {
                        dotProduct += (a[i, k] * b[k, j]);
                    }
                    result[i, j] = dotProduct;
                }
            }

            return (result);
        }




        public static MatrixF64 Identity(int rows)
        {
            MatrixF64 identity = MatrixF64.Zero(rows, rows);
            for (int i = 0; i < rows; i++)
            {
                identity[i, i] = 1.0;
            }
            return (identity);
        }




        public static void FindLUFactorsOfARowPermutedVersionOfAnOriginalMatrix
            (
            MatrixF64 originalMatrix,
            ref MatrixF64 LUFactorsMatrix,
            ref int[] rowPermutationOfOriginalMatrix,
            ref bool rowExchangeParityIsOdd
            )
        {
            // ''LU factoring'' involves factoring a square matrix (M) in to a 
            // lower-triangular matrix (L), and an upper-triangular matrix (U),
            // such that (L)*(U) = (M).
            //
            // However, the specified matrix (originalMatrix) might not be 
            // optimal for direct LU factoring, due to the locations of extreme
            // values in the matrix.  Therefore, this function instead implicitly
            // factors a row-permuted version of originalMatrix, where the
            // permutation of rows is determined by the values in the specified
            // matrix.  The specific row permutation selected by this function is
            // returned in an array of row indices (rowPermutationOfOriginalMatrix). 
            // The resulting (L) and (U) factors are merged in to a single 
            // output matrix (LUFactorsMatrix).  Therefore:
            //
            // (L part of LUFactorsMatrix) * (U part of LUFactorsMatrix)
            //   = (originalMatrix with rows permuted according to 
            //           rowPermutationOfOriginalMatrix).
            //
            // Although factoring a row-permuted version of originalMatrix
            // makes it more difficult to interpret the results of this function,
            // the resulting LU factoring is likely to be more accurate.  In a 
            // sense, this function indicates (via rowPermutationOfOriginalMatrix)
            // how to permute the rows of originalMatrix to be able to produce 
            // the most accurate LU factoring directly, and this function produces
            // that factoring (via the LUFactorsMatrix output).
            //
            // When using the (L) and (U) matrices (merged in to LUFactorsMatrix)
            // to solve a system of equations, it is necessary to permute the rows
            // of the solution vector of the system of equations according to 
            // rowPermutation.
            //
            // The output matrix (LUFactorsMatrix) is a combination of
            // the (L) and (U) matrices:
            //
            // To form the (L) matrix from (LUFactorsMatrix):
            //      assume (0) for entries above the diagonal, 
            //      assume (1) for entries on    the diagonal,
            //      and use (LUFactorsMatrix) entries below the diagonal.
            //
            // To form the (U) matrix from (LUFactorsMatrix):
            //      assume (0) for entries below the diagonal, 
            //      and use (LUFactorsMatrix) entries on and above the diagonal.
            //
            // The output array (rowPermutationOfOriginalMatrix) indicates how the
            // rows of the original matrix have implicitly (not actually) been 
            // permuted.
            //
            // The output boolean (rowExchangeParityIsOdd) indicates if the parity
            // of the permutation of rows is odd.
            //
            // This implementation is partly based on pseudo-code appearing in 
            // ''Introduction to Algorithms'' (Cormen; Lieserson; Rivest; 
            // 24th printing, 2000; MIT Press; ISBN 0-262-03141-8).
            // See the section named ''Overview of LUP decomposition'', in 
            // chapter 31 (''Matrix Operations'').  The implementation here follows
            // the ''LUP-Decomposition'' pseudo-code appearing in a section named 
            // ''Computing an LUP decomposition''.
            //
            // This implementation also uses ideas found in the implementation of
            // ''ludcmp'' in the book ''Numerical recipes in C : The art of 
            // scientific computing'' (Press; Teukolsky; Vetterling; Flannery; 
            // second edition; 1996 reprinting; Cambridge University Press).  
            // The overall loop structure here resembles that of ''ludcmp''.  
            // The idea of determining and caching row scale factors in advance of
            // finding pivots is used here.  The method in ''ludcmp'' to avoid zero 
            // pivot values has been modified here to handle excessively-small
            // pivot values, too.  HOWEVER, the row permutation indices produced by
            // the following implementation is a true permutation of 
            // {0,1,...,(totalRows-1)}, whereas ''ludcmp'' (and ''lubksb'') 
            // interpret their ''permutation indices'' as swaps to perform 
            // (roughly, for each (i), swap b[ i ] and b[ indx[i] ]).  So, the 
            // following implementation of LU-factoring is not directly compatible 
            // with ''ludcmp'' or ''lubksb''.  Also, the following procedure
            // does not destroy the original matrix (unlike ''ludcmp'').

            double singularMatrixIfMaxRowElementIsLessThanThis = (1.0e-19);
            double forcePivotsToHaveAtLeastThisAbsoluteValue = (1.0e-19);

            LUFactorsMatrix = null;
            rowPermutationOfOriginalMatrix = null;
            rowExchangeParityIsOdd = false;

            if (null == originalMatrix)
            {
                return; // No matrix specified.
            }

            if
            (
                   (originalMatrix.totalRows <= 0)
                || (originalMatrix.totalColumns <= 0)
                || (null == originalMatrix.entries)
            )
            {
                return; // Matrix is empty.
            }

            if (originalMatrix.totalRows != originalMatrix.totalColumns)
            {
                return; // Matrix is not square.
            }


            // Duplicate the original matrix
            LUFactorsMatrix = new MatrixF64(originalMatrix);


            // (Lines 2-3 of LUP-Decomposition)
            // Initialize the row permutation array to the identity 
            // permutation.
            rowPermutationOfOriginalMatrix = new int[LUFactorsMatrix.totalRows];
            for (int i = 0; i < LUFactorsMatrix.totalRows; i++)
            {
                rowPermutationOfOriginalMatrix[i] = i;
            }


            // For each row, determine the largest absolute value of
            // the row elements, and use the reciprocal as the scale for
            // that row.
            VectorF64 rowScales = VectorF64.Zero(LUFactorsMatrix.totalRows);
            for (int i = 0; i < LUFactorsMatrix.totalRows; i++)
            {
                double largestElementAbsoluteValueInRow = 0.0;
                for (int j = 0; j < LUFactorsMatrix.totalColumns; j++)
                {
                    double absoluteElementValue =
                        Math.Abs(LUFactorsMatrix[i, j]);

                    if (absoluteElementValue >
                        largestElementAbsoluteValueInRow)
                    {
                        largestElementAbsoluteValueInRow =
                            absoluteElementValue;
                    }
                }
                if (largestElementAbsoluteValueInRow <
                    singularMatrixIfMaxRowElementIsLessThanThis)
                {
                    return; // Matrix is singular
                }
                rowScales[i] = (1.0 / largestElementAbsoluteValueInRow);
            }


            // (Lines 4-18 of LUP-Decomposition)
            // Go through the columns of the matrix.
            for (int j = 0; j < LUFactorsMatrix.totalColumns; j++)
            {
                // (Lines 17-18 of LUP-Decomposition)
                // (or equation (2.3.12) of NR)
                for (int i = 0; i < j; i++)
                {
                    double sum = LUFactorsMatrix[i, j];
                    for (int k = 0; k < i; k++)
                    {
                        sum -= (LUFactorsMatrix[i, k] * LUFactorsMatrix[k, j]);
                    }
                    LUFactorsMatrix[i, j] = sum;
                }

                // Go through all remaining rows and find the best pivot.
                double largestScaledSum = 0.0;
                int rowIndexOfLargestScaledSum = j;
                for (int i = j; i < LUFactorsMatrix.totalRows; i++)
                {
                    // (equation (2.3.13) of NR)
                    double sum = LUFactorsMatrix[i, j];
                    for (int k = 0; k < j; k++)
                    {
                        sum -= (LUFactorsMatrix[i, k] * LUFactorsMatrix[k, j]);
                    }
                    LUFactorsMatrix[i, j] = sum;

                    double scaledSum = rowScales[i] * Math.Abs(sum);
                    if (scaledSum >= largestScaledSum)
                    {
                        largestScaledSum = scaledSum;
                        rowIndexOfLargestScaledSum = i;
                    }
                }

                // If indeed we found a better pivot, then exchange rows.
                if (j != rowIndexOfLargestScaledSum)
                {
                    // (Line 12 of LUP-Decomposition)
                    // Exchange the row permutation indices
                    int tempRowIndex = rowPermutationOfOriginalMatrix[j];
                    rowPermutationOfOriginalMatrix[j] =
                        rowPermutationOfOriginalMatrix[rowIndexOfLargestScaledSum];
                    rowPermutationOfOriginalMatrix[rowIndexOfLargestScaledSum] =
                        tempRowIndex;

                    // (Lines 13-14 of LUP-Decomposition)
                    // Exchange the elements of the rows
                    for (int k = 0; k < LUFactorsMatrix.totalColumns; k++)
                    {
                        double temp = LUFactorsMatrix[rowIndexOfLargestScaledSum, k];
                        LUFactorsMatrix[rowIndexOfLargestScaledSum, k] =
                            LUFactorsMatrix[j, k];
                        LUFactorsMatrix[j, k] = temp;
                    }

                    // Exchange the row scale factors
                    double scaleFactor =
                        rowScales[rowIndexOfLargestScaledSum];
                    rowScales[rowIndexOfLargestScaledSum] = rowScales[j];
                    rowScales[j] = scaleFactor;

                    // Invert the overall row exchange parity
                    rowExchangeParityIsOdd = (!(rowExchangeParityIsOdd));
                }

                // Force the pivot element to have at least a certain 
                // absolute value.
                if (Math.Abs(LUFactorsMatrix[j, j]) <
                    forcePivotsToHaveAtLeastThisAbsoluteValue)
                {
                    if (LUFactorsMatrix[j, j] < 0.0)
                    {
                        LUFactorsMatrix[j, j] =
                            (-(forcePivotsToHaveAtLeastThisAbsoluteValue));
                    }
                    else
                    {
                        LUFactorsMatrix[j, j] =
                            forcePivotsToHaveAtLeastThisAbsoluteValue;
                    }
                }

                // If not the final column, then divide all column elements 
                // below the diagonal by the pivot element, matrixLU[j,j].
                // (Lines 15-16 of LUP-Decomposition)
                if (j != (LUFactorsMatrix.totalColumns - 1))
                {
                    double reciprocalOfPivot = (1.0 / LUFactorsMatrix[j, j]);
                    for (int i = (j + 1); i < LUFactorsMatrix.totalRows; i++)
                    {
                        LUFactorsMatrix[i, j] *= reciprocalOfPivot;
                    }
                }
            }
        }




        public static void LUBacksubstitution
            (
            MatrixF64 LUFactorsMatrix,
            int[] rowPermutationOfOriginalMatrix,
            VectorF64 givenProductVector,
            ref VectorF64 solutionVector
            )
        {
            // This implementation is based on pseudo-code appearing in
            // ''Introduction to Algorithms'' (Cormen; Lieserson; Rivest;
            // 24th printing, 2000; MIT Press; ISBN 0-262-03141-8).
            // See the section named ''Overview of LUP decomposition'', in
            // chapter 31 (''Matrix Operations'').  The implementation here
            // follows the ''LUP-Solve'' pseudo-code appearing in a
            // section named ''forward and back substitution''.

            solutionVector = null;

            if (null == LUFactorsMatrix)
            {
                return; // No matrix specified.
            }

            if (null == rowPermutationOfOriginalMatrix)
            {
                return; // No permutation specified.
            }

            if (null == givenProductVector)
            {
                return; // No product vector specified.
            }

            if
            (
                   (null == LUFactorsMatrix.entries)
                || (LUFactorsMatrix.totalRows <= 0)
                || (LUFactorsMatrix.totalColumns <= 0)
            )
            {
                return; // Matrix is empty.
            }

            if (LUFactorsMatrix.totalRows !=
                LUFactorsMatrix.totalColumns)
            {
                return; // Matrix is not square.
            }

            if (givenProductVector.Dimensions() !=
                LUFactorsMatrix.totalRows)
            {
                return; // Product vector size not equal to matrix row count.
            }


            // Copy the product vector in to the result.
            solutionVector = new VectorF64(givenProductVector);


            // (See LUP-Solve, lines 2-3)
            for (int i = 0; i < LUFactorsMatrix.totalRows; i++)
            {
                double sum =
                    givenProductVector[rowPermutationOfOriginalMatrix[i]];
                for (int j = 0; j < i; j++)
                {
                    sum -= (LUFactorsMatrix[i, j] * solutionVector[j]);
                }
                solutionVector[i] = sum;
            }


            // (See LUP-Solve, lines 4-5)
            for (int i = (LUFactorsMatrix.totalRows - 1); i >= 0; i--)
            {
                double sum = solutionVector[i];
                for
                (
                    int j = (i + 1);
                    j < LUFactorsMatrix.totalColumns;
                    j++
                )
                {
                    sum -= (LUFactorsMatrix[i, j] * solutionVector[j]);
                }
                double diagonalElement = LUFactorsMatrix[i, i];
                if (diagonalElement != 0.0)
                {
                    solutionVector[i] = (sum / diagonalElement);
                }
            }
        }




        private double Determinant1x1()
        {
            if
            (
                   (this.totalRows != 1)
                || (this.totalColumns != 1)
                || (null == this.entries)
            )
            {
                return (0.0);
            }

            // The determinant of a 1x1 matrix is simply
            // the value of the single element.
            return (this[0, 0]);
        }




        private double Determinant2x2()
        {
            if
            (
                   (this.totalRows != 2)
                || (this.totalColumns != 2)
                || (null == this.entries)
            )
            {
                return (0.0);
            }

            return
            (
            +this[0, 0] * this[1, 1]

            - this[0, 1] * this[1, 0]
            );
        }




        private double Determinant3x3()
        {
            if
            (
                   (this.totalRows != 3)
                || (this.totalColumns != 3)
                || (null == this.entries)
            )
            {
                return (0.0);
            }

            return
            (
            +this[0, 0] * this[1, 1] * this[2, 2]
            + this[0, 1] * this[1, 2] * this[2, 0]
            + this[0, 2] * this[1, 0] * this[2, 1]

            - this[0, 0] * this[1, 2] * this[2, 1]
            - this[0, 1] * this[1, 0] * this[2, 2]
            - this[0, 2] * this[1, 1] * this[2, 0]
            );
        }




        private double Determinant4x4()
        {
            if
            (
                   (this.totalRows != 4)
                || (this.totalColumns != 4)
                || (null == this.entries)
            )
            {
                return (0.0);
            }

            return
            (
            +this[0, 0] * this[1, 1] * this[2, 2] * this[3, 3]
            + this[0, 0] * this[1, 2] * this[2, 3] * this[3, 1]
            + this[0, 0] * this[1, 3] * this[2, 1] * this[3, 2]

            + this[0, 1] * this[1, 0] * this[2, 3] * this[3, 2]
            + this[0, 1] * this[1, 2] * this[2, 0] * this[3, 3]
            + this[0, 1] * this[1, 3] * this[2, 2] * this[3, 0]

            + this[0, 2] * this[1, 0] * this[2, 1] * this[3, 3]
            + this[0, 2] * this[1, 1] * this[2, 3] * this[3, 0]
            + this[0, 2] * this[1, 3] * this[2, 0] * this[3, 1]

            + this[0, 3] * this[1, 0] * this[2, 2] * this[3, 1]
            + this[0, 3] * this[1, 1] * this[2, 0] * this[3, 2]
            + this[0, 3] * this[1, 2] * this[2, 1] * this[3, 0]

            - this[0, 0] * this[1, 1] * this[2, 3] * this[3, 2]
            - this[0, 0] * this[1, 2] * this[2, 1] * this[3, 3]
            - this[0, 0] * this[1, 3] * this[2, 2] * this[3, 1]

            - this[0, 1] * this[1, 0] * this[2, 2] * this[3, 3]
            - this[0, 1] * this[1, 2] * this[2, 3] * this[3, 0]
            - this[0, 1] * this[1, 3] * this[2, 0] * this[3, 2]

            - this[0, 2] * this[1, 0] * this[2, 3] * this[3, 1]
            - this[0, 2] * this[1, 1] * this[2, 0] * this[3, 3]
            - this[0, 2] * this[1, 3] * this[2, 1] * this[3, 0]

            - this[0, 3] * this[1, 0] * this[2, 1] * this[3, 2]
            - this[0, 3] * this[1, 1] * this[2, 2] * this[3, 0]
            - this[0, 3] * this[1, 2] * this[2, 0] * this[3, 1]
            );
        }




        public double Determinant()
        {
            if
            (
                   (this.totalRows <= 0)
                || (this.totalColumns <= 0)
                || (null == this.entries)
            )
            {
                return (0.0); // Matrix is empty.
            }

            if (this.totalRows != this.totalColumns)
            {
                // Matrix is not square
                return (0.0);
            }

            // Simple cases
            if (1 == this.totalRows) { return (this.Determinant1x1()); }
            if (2 == this.totalRows) { return (this.Determinant2x2()); }
            if (3 == this.totalRows) { return (this.Determinant3x3()); }
            if (4 == this.totalRows) { return (this.Determinant4x4()); }

            int[] rowPermutationOfOriginalMatrix = null;
            bool rowExchangeParityIsOdd = false;
            MatrixF64 originalMatrix = new MatrixF64(this);
            MatrixF64 LUFactorsMatrix = null;

            MatrixF64.FindLUFactorsOfARowPermutedVersionOfAnOriginalMatrix
                (
                originalMatrix,
                ref LUFactorsMatrix,
                ref rowPermutationOfOriginalMatrix,
                ref rowExchangeParityIsOdd
                );

            if ((null == LUFactorsMatrix)
                || (null == rowPermutationOfOriginalMatrix))
            {
                return (0.0);
            }

            double determinant = 1.0;
            if (true == rowExchangeParityIsOdd)
            {
                determinant = (-1.0);
            }
            for (int j = 0; j < LUFactorsMatrix.totalColumns; j++)
            {
                determinant *= LUFactorsMatrix[j, j];
            }
            return (determinant);
        }




        private MatrixF64 Inverse1x1()
        {
            if
            (
                   (this.totalRows != 1)
                || (this.totalColumns != 1)
                || (null == this.entries)
            )
            {
                return (MatrixF64.Zero(1, 1)); // Matrix is empty.
            }

            if (0.0 == this[0, 0])
            {
                return (MatrixF64.Zero(1, 1)); // Matrix has no inverse.
            }

            MatrixF64 inverse = MatrixF64.Zero(1, 1);

            // The inverse of a 1x1 matrix is simply the
            // reciprocal of the single entry.
            inverse[0, 0] = (1.0 / this[0, 0]);

            return (inverse);
        }




        private MatrixF64 Inverse2x2()
        {
            if
            (
                   (this.totalRows != 2)
                || (this.totalColumns != 2)
                || (null == this.entries)
            )
            {
                return (MatrixF64.Zero(2, 2)); // Matrix is empty.
            }

            double determinant = this.Determinant2x2();
            if (0.0 == determinant)
            {
                return (MatrixF64.Zero(2, 2)); // Matrix has no inverse.
            }

            MatrixF64 inverse = MatrixF64.Zero(2, 2);


            inverse[0, 0] = this[1, 1];
            inverse[0, 1] = (-(this[0, 1]));
            inverse[1, 0] = (-(this[1, 0]));
            inverse[1, 1] = this[0, 0];


            double factor = (1.0 / determinant);
            for (int i = 0; i < inverse.totalRows; i++)
            {
                for (int j = 0; j < inverse.totalColumns; j++)
                {
                    inverse[i, j] *= factor;
                }
            }

            return (inverse);
        }




        private MatrixF64 Inverse3x3()
        {
            if
            (
                   (this.totalRows != 3)
                || (this.totalColumns != 3)
                || (null == this.entries)
            )
            {
                return (MatrixF64.Zero(3, 3)); // Matrix is empty.
            }

            double determinant = this.Determinant3x3();
            if (0.0 == determinant)
            {
                return (MatrixF64.Zero(3, 3)); // Matrix has no inverse.
            }

            MatrixF64 inverse = MatrixF64.Zero(3, 3);


            inverse[0, 0] =
                  this[1, 1] * this[2, 2]
                - this[1, 2] * this[2, 1];
            inverse[0, 1] =
                  this[0, 2] * this[2, 1]
                - this[0, 1] * this[2, 2];
            inverse[0, 2] =
                  this[0, 1] * this[1, 2]
                - this[0, 2] * this[1, 1];

            inverse[1, 0] =
                  this[1, 2] * this[2, 0]
                - this[1, 0] * this[2, 2];
            inverse[1, 1] =
                  this[0, 0] * this[2, 2]
                - this[0, 2] * this[2, 0];
            inverse[1, 2] =
                  this[0, 2] * this[1, 0]
                - this[0, 0] * this[1, 2];

            inverse[2, 0] =
                  this[1, 0] * this[2, 1]
                - this[1, 1] * this[2, 0];
            inverse[2, 1] =
                  this[0, 1] * this[2, 0]
                - this[0, 0] * this[2, 1];
            inverse[2, 2] =
                  this[0, 0] * this[1, 1]
                - this[0, 1] * this[1, 0];


            double factor = (1.0 / determinant);
            for (int i = 0; i < inverse.totalRows; i++)
            {
                for (int j = 0; j < inverse.totalColumns; j++)
                {
                    inverse[i, j] *= factor;
                }
            }

            return (inverse);
        }




        private MatrixF64 Inverse4x4()
        {
            if
            (
                   (this.totalRows != 4)
                || (this.totalColumns != 4)
                || (null == this.entries)
            )
            {
                return (MatrixF64.Zero(4, 4)); // Matrix is empty.
            }

            double determinant = this.Determinant4x4();
            if (0.0 == determinant)
            {
                return (MatrixF64.Zero(4, 4)); // Matrix has no inverse.
            }

            MatrixF64 inverse = MatrixF64.Zero(4, 4);


            inverse[0, 0] =
            +this[1, 1] * this[2, 2] * this[3, 3]
            + this[1, 2] * this[2, 3] * this[3, 1]
            + this[1, 3] * this[2, 1] * this[3, 2]
            - this[1, 1] * this[2, 3] * this[3, 2]
            - this[1, 2] * this[2, 1] * this[3, 3]
            - this[1, 3] * this[2, 2] * this[3, 1];

            inverse[0, 1] =
            +this[0, 1] * this[2, 3] * this[3, 2]
            + this[0, 2] * this[2, 1] * this[3, 3]
            + this[0, 3] * this[2, 2] * this[3, 1]
            - this[0, 1] * this[2, 2] * this[3, 3]
            - this[0, 2] * this[2, 3] * this[3, 1]
            - this[0, 3] * this[2, 1] * this[3, 2];

            inverse[0, 2] =
            +this[0, 1] * this[1, 2] * this[3, 3]
            + this[0, 2] * this[1, 3] * this[3, 1]
            + this[0, 3] * this[1, 1] * this[3, 2]
            - this[0, 1] * this[1, 3] * this[3, 2]
            - this[0, 2] * this[1, 1] * this[3, 3]
            - this[0, 3] * this[1, 2] * this[3, 1];

            inverse[0, 3] =
            +this[0, 1] * this[1, 3] * this[2, 2]
            + this[0, 2] * this[1, 1] * this[2, 3]
            + this[0, 3] * this[1, 2] * this[2, 1]
            - this[0, 1] * this[1, 2] * this[2, 3]
            - this[0, 2] * this[1, 3] * this[2, 1]
            - this[0, 3] * this[1, 1] * this[2, 2];


            inverse[1, 0] =
            +this[1, 0] * this[2, 3] * this[3, 2]
            + this[1, 2] * this[2, 0] * this[3, 3]
            + this[1, 3] * this[2, 2] * this[3, 0]
            - this[1, 0] * this[2, 2] * this[3, 3]
            - this[1, 2] * this[2, 3] * this[3, 0]
            - this[1, 3] * this[2, 0] * this[3, 2];

            inverse[1, 1] =
            +this[0, 0] * this[2, 2] * this[3, 3]
            + this[0, 2] * this[2, 3] * this[3, 0]
            + this[0, 3] * this[2, 0] * this[3, 2]
            - this[0, 0] * this[2, 3] * this[3, 2]
            - this[0, 2] * this[2, 0] * this[3, 3]
            - this[0, 3] * this[2, 2] * this[3, 0];

            inverse[1, 2] =
            +this[0, 0] * this[1, 3] * this[3, 2]
            + this[0, 2] * this[1, 0] * this[3, 3]
            + this[0, 3] * this[1, 2] * this[3, 0]
            - this[0, 0] * this[1, 2] * this[3, 3]
            - this[0, 2] * this[1, 3] * this[3, 0]
            - this[0, 3] * this[1, 0] * this[3, 2];

            inverse[1, 3] =
            +this[0, 0] * this[1, 2] * this[2, 3]
            + this[0, 2] * this[1, 3] * this[2, 0]
            + this[0, 3] * this[1, 0] * this[2, 2]
            - this[0, 0] * this[1, 3] * this[2, 2]
            - this[0, 2] * this[1, 0] * this[2, 3]
            - this[0, 3] * this[1, 2] * this[2, 0];


            inverse[2, 0] =
            +this[1, 0] * this[2, 1] * this[3, 3]
            + this[1, 1] * this[2, 3] * this[3, 0]
            + this[1, 3] * this[2, 0] * this[3, 1]
            - this[1, 0] * this[2, 3] * this[3, 1]
            - this[1, 1] * this[2, 0] * this[3, 3]
            - this[1, 3] * this[2, 1] * this[3, 0];

            inverse[2, 1] =
            +this[0, 0] * this[2, 3] * this[3, 1]
            + this[0, 1] * this[2, 0] * this[3, 3]
            + this[0, 3] * this[2, 1] * this[3, 0]
            - this[0, 0] * this[2, 1] * this[3, 3]
            - this[0, 1] * this[2, 3] * this[3, 0]
            - this[0, 3] * this[2, 0] * this[3, 1];

            inverse[2, 2] =
            +this[0, 0] * this[1, 1] * this[3, 3]
            + this[0, 1] * this[1, 3] * this[3, 0]
            + this[0, 3] * this[1, 0] * this[3, 1]
            - this[0, 0] * this[1, 3] * this[3, 1]
            - this[0, 1] * this[1, 0] * this[3, 3]
            - this[0, 3] * this[1, 1] * this[3, 0];

            inverse[2, 3] =
            +this[0, 0] * this[1, 3] * this[2, 1]
            + this[0, 1] * this[1, 0] * this[2, 3]
            + this[0, 3] * this[1, 1] * this[2, 0]
            - this[0, 0] * this[1, 1] * this[2, 3]
            - this[0, 1] * this[1, 3] * this[2, 0]
            - this[0, 3] * this[1, 0] * this[2, 1];


            inverse[3, 0] =
            +this[1, 0] * this[2, 2] * this[3, 1]
            + this[1, 1] * this[2, 0] * this[3, 2]
            + this[1, 2] * this[2, 1] * this[3, 0]
            - this[1, 0] * this[2, 1] * this[3, 2]
            - this[1, 1] * this[2, 2] * this[3, 0]
            - this[1, 2] * this[2, 0] * this[3, 1];

            inverse[3, 1] =
            +this[0, 0] * this[2, 1] * this[3, 2]
            + this[0, 1] * this[2, 2] * this[3, 0]
            + this[0, 2] * this[2, 0] * this[3, 1]
            - this[0, 0] * this[2, 2] * this[3, 1]
            - this[0, 1] * this[2, 0] * this[3, 2]
            - this[0, 2] * this[2, 1] * this[3, 0];

            inverse[3, 2] =
            +this[0, 0] * this[1, 2] * this[3, 1]
            + this[0, 1] * this[1, 0] * this[3, 2]
            + this[0, 2] * this[1, 1] * this[3, 0]
            - this[0, 0] * this[1, 1] * this[3, 2]
            - this[0, 1] * this[1, 2] * this[3, 0]
            - this[0, 2] * this[1, 0] * this[3, 1];

            inverse[3, 3] =
            +this[0, 0] * this[1, 1] * this[2, 2]
            + this[0, 1] * this[1, 2] * this[2, 0]
            + this[0, 2] * this[1, 0] * this[2, 1]
            - this[0, 0] * this[1, 2] * this[2, 1]
            - this[0, 1] * this[1, 0] * this[2, 2]
            - this[0, 2] * this[1, 1] * this[2, 0];


            double factor = (1.0 / determinant);
            for (int i = 0; i < inverse.totalRows; i++)
            {
                for (int j = 0; j < inverse.totalColumns; j++)
                {
                    inverse[i, j] *= factor;
                }
            }

            return (inverse);
        }




        public MatrixF64 Inverse()
        {
            if
            (
                   (this.totalRows <= 0)
                || (this.totalColumns <= 0)
                || (null == this.entries)
            )
            {
                return (MatrixF64.Zero(1, 1)); // Matrix is empty.
            }

            if (this.totalRows != this.totalColumns)
            {
                // Matrix is not square
                return (MatrixF64.Zero(1, 1));
            }

            // Simple cases
            if (1 == this.totalRows) { return (this.Inverse1x1()); }
            if (2 == this.totalRows) { return (this.Inverse2x2()); }
            if (3 == this.totalRows) { return (this.Inverse3x3()); }
            if (4 == this.totalRows) { return (this.Inverse4x4()); }


            int[] rowPermutationOfOriginalMatrix = null;
            bool rowExchangeParityIsOdd = false;
            MatrixF64 originalMatrix = new MatrixF64(this);
            MatrixF64 LUFactorsMatrix = null;

            MatrixF64.FindLUFactorsOfARowPermutedVersionOfAnOriginalMatrix
                (
                originalMatrix,
                ref LUFactorsMatrix,
                ref rowPermutationOfOriginalMatrix,
                ref rowExchangeParityIsOdd
                );

            if ((null == LUFactorsMatrix)
                || (null == rowPermutationOfOriginalMatrix))
            {
                return (MatrixF64.Zero(this.totalRows, this.totalColumns));
            }


            VectorF64 columnVector = VectorF64.Zero(totalRows);
            VectorF64 solutionVector = VectorF64.Zero(totalRows);
            MatrixF64 inverse =
                MatrixF64.Zero(this.totalRows, this.totalColumns);
            for (int j = 0; j < inverse.totalColumns; j++)
            {
                for (int i = 0; i < inverse.totalRows; i++)
                {
                    columnVector[i] = 0.0;
                }
                columnVector[j] = 1.0;

                MatrixF64.LUBacksubstitution
                    (
                    LUFactorsMatrix,
                    rowPermutationOfOriginalMatrix,
                    columnVector,
                    ref solutionVector
                    );
                if (null != solutionVector)
                {
                    for (int i = 0; i < inverse.totalRows; i++)
                    {
                        inverse[i, j] = solutionVector[i];
                    }
                }
            }

            return (inverse);
        }




        public static VectorF64 operator *(MatrixF64 m, VectorF64 v)
        {
            if (null == m)
            {
                return (null); // Matrix is not specified.
            }

            if
            (
                   (null == m.entries)
                || (m.totalRows <= 0)
                || (m.totalColumns <= 0)
            )
            {
                return (null); // Matrix is empty.
            }

            if (null == v)
            {
                // Vector is not specified.
                return (VectorF64.Zero(m.totalRows));
            }

            if (v.Dimensions() != m.totalColumns)
            {
                // The number of columns of the matrix must be equal
                // to the number of rows of the vector.
                return (VectorF64.Zero(m.totalRows));
            }


            // The result vector has the same number of rows as the matrix.
            VectorF64 result = VectorF64.Zero(m.totalRows);


            // Component (i) of the result vector is equal to the
            // dot product of row (i) of the matrix with the vector.
            for (int i = 0; i < m.totalRows; i++)
            {
                double dotProduct = 0.0;
                for (int j = 0; j < m.totalColumns; j++)
                {
                    dotProduct += (m[i, j] * v[j]);
                }
                result[i] = dotProduct;
            }

            return (result);
        }




        public static MatrixF64 FormHomogeneousTranslationMatrix(VectorF64 v)
        {
            // This method assumes the specified vector is a homogeneous
            // vector, where the final component is not relevant to the
            // translation.  Thus, we form a square homogeneous matrix 
            // with a total number of rows equal to the number of 
            // components of the specified vector.  We form an identity
            // matrix of the required size, and copy all but the final
            // component of the specified vector to the final column of
            // the matrix.
            if (null == v)
            {
                return (MatrixF64.Zero(1, 1)); // Vector is not specified.
            }

            if (v.Dimensions() <= 0)
            {
                return (MatrixF64.Zero(1, 1)); // Vector is empty.
            }

            MatrixF64 result =
                MatrixF64.Identity(v.Dimensions());

            // Add all but the final component of the specified vector 
            // to the final column of the result matrix.  The final 
            // entry of the final column of the matrix will remain one (1).
            for (int i = 0; i < (result.totalRows - 1); i++)
            {
                result[i, (result.totalColumns - 1)] = v[i];
            }

            return (result);
        }




        public MatrixF64 TranslateByAHomogeneousVector(VectorF64 v)
        {
            // This function assumes a square matrix that represents a
            // homogeneous transformation (with a final row that is all
            // zero except for a final entry of one), and the specification
            // of a homogeneous vector (with a final component of one).
            // All but the final component of the vector will contribute
            // to the final column vector of the matrix.
            if (null == v)
            {
                return (MatrixF64.Zero(1, 1)); // Vector is not specified.
            }

            if
            (
                   (this.totalRows <= 0)
                || (this.totalColumns <= 0)
                || (null == this.entries)
            )
            {
                return (MatrixF64.Zero(1, 1)); // Matrix is empty.
            }

            if (v.Dimensions() != this.totalRows)
            {
                // The vector size is not equal to the matrix total rows.
                return (MatrixF64.Zero(this.totalRows, this.totalColumns));
            }

            // Add all but the final component of the specified vector 
            // to the final column of this matrix.  The final entry
            // of the final column of the matrix will remain one (1).
            MatrixF64 result = new MatrixF64(this);

            for (int i = 0; i < (result.totalRows - 1); i++)
            {
                result[i, (result.totalColumns - 1)] += v[i];
            }

            return (result);
        }




        public MatrixF64 SetTranslationToAHomogeneousVector(VectorF64 v)
        {
            // This function assumes a square matrix that represents a
            // homogeneous transformation (with a final row that is all
            // zero except for a final entry of one), and the specification
            // of a homogeneous vector (with a final component of one).
            // All but the final component of the vector will contribute
            // to the final column vector of the matrix.
            if (null == v)
            {
                return (MatrixF64.Zero(1, 1)); // Vector is not specified.
            }

            if
            (
                   (this.totalRows <= 0)
                || (this.totalColumns <= 0)
                || (null == this.entries)
            )
            {
                return (MatrixF64.Zero(1, 1)); // Matrix is empty.
            }

            if (v.Dimensions() != this.totalRows)
            {
                // The vector size is not equal to the matrix total rows.
                return (MatrixF64.Zero(this.totalRows, this.totalColumns));
            }

            // Add all but the final component of the specified vector 
            // to the final column of this matrix.  The final entry
            // of the final column of the matrix will remain one (1).
            MatrixF64 result = new MatrixF64(this);

            for (int i = 0; i < (result.totalRows - 1); i++)
            {
                result[i, (result.totalColumns - 1)] = v[i];
            }

            return (result);
        }




        public static MatrixF64 Rab(int rows, int a, int b, double angle)
        {
            if (rows <= 0)
            {
                // Invalid row count specified.
                return (MatrixF64.Zero(1, 1));
            }

            MatrixF64 result = MatrixF64.Identity(rows);

            // If (a==b) (i.e., rotate coordinate (a) to (a)), then
            // there is no rotation to perform.

            if ((a != b) && (a >= 0) && (a < rows) && (b >= 0) && (b < rows))
            {
                double sine = Math.Sin(angle);
                double cosine = Math.Cos(angle);

                // Override m[a,a] = cosine;  m[a,b] = -sine;
                //          m[b,a] =   sine;  m[b,b] = cosine;
                result[a, a] = cosine;
                result[b, b] = cosine;
                result[a, b] = (-(sine));
                result[b, a] = sine;
            }

            return (result);
        }




        public static MatrixF64 Rabk(int rows, int a, int b, int k)
        {
            double angle = (Math.PI / 2.0) * (double)k;
            return (MatrixF64.Rab(rows, a, b, angle));
        }




        public static void Test()
        {
            System.Console.WriteLine(String.Empty.PadRight(78, '#'));
            System.Console.WriteLine(Environment.NewLine);
            System.Console.WriteLine("Test of MatrixF64");
            System.Console.WriteLine(Environment.NewLine);
            System.Console.WriteLine(String.Empty.PadRight(78, '#'));
            System.Console.WriteLine(Environment.NewLine);




            System.Console.WriteLine(Environment.NewLine);
            System.Console.WriteLine("Test of matrix initialization:");
            System.Console.WriteLine(Environment.NewLine);

            // A 5x3 matrix (5 rows x 3 columns) with 
            // 64-bit floating-point entries:
            MatrixF64 m =
                new MatrixF64
                (
                5, 3,
                 0.0, 1.0, 2.0, // row 0
                 3.0, 4.0, 5.0, // row 1 
                 6.0, 7.0, 8.0, // row 2
                 9.0, 10.0, 11.0, // row 3
                12.0, 13.0, 14.0  // row 4
                );
            m.WriteLine();
            System.Console.WriteLine(Environment.NewLine);
            // [  0,  1,  2 ]  // row 0
            // [  3,  4,  5 ]  // row 1
            // [  6,  7,  8 ]  // row 2
            // [  9, 10, 11 ]  // row 3
            // [ 12, 13, 14 ]  // row 4


            // An 8x2 matrix (8 rows x 2 columns) with all 16
            // 64-bit floating-point entries set to zero:
            MatrixF64 z = MatrixF64.Zero(8, 2);
            z.WriteLine();
            System.Console.WriteLine(Environment.NewLine);
            // [ 0, 0 ]  // row 0
            // [ 0, 0 ]  // row 1
            // [ 0, 0 ]  // row 2
            // [ 0, 0 ]  // row 3
            // [ 0, 0 ]  // row 4
            // [ 0, 0 ]  // row 5
            // [ 0, 0 ]  // row 6
            // [ 0, 0 ]  // row 7



            // Examples of matrix addition, subtraction, and multiplication:

            System.Console.WriteLine(Environment.NewLine);
            System.Console.WriteLine("Test of matrix addition, subtraction, and multiplication:");
            System.Console.WriteLine(Environment.NewLine);

            MatrixF64 a =
                new MatrixF64
                (
                3, 5,
                 0.0, 1.0, 2.0, 3.0, 4.0, // row 0
                 5.0, 6.0, 7.0, 8.0, 9.0, // row 1
                10.0, 11.0, 12.0, 13.0, 14.0  // row 2
                );

            MatrixF64 b =
                new MatrixF64
                (
                3, 5,
                 4.0, -3.0, 2.0, -1.0, 0.0, // row 0
                -9.0, 8.0, -7.0, 6.0, -5.0, // row 1
                14.0, -13.0, 12.0, -11.0, 10.0  // row 2
                );

            MatrixF64 c =
                new MatrixF64
                (
                5, 3,
                 0.0, 1.0, 2.0, // row 0
                 3.0, 4.0, 5.0, // row 1 
                 6.0, 7.0, 8.0, // row 2
                 9.0, 10.0, 11.0, // row 3
                12.0, 13.0, 14.0  // row 4
                );

            MatrixF64 result = new MatrixF64();

            result = a + b;
            result.WriteLine();
            System.Console.WriteLine(Environment.NewLine);
            // [  4, -2,  4,  2,  4 ]
            // [ -4, 14,  0, 14,  4 ]
            // [ 24, -2, 24,  2, 24 ]

            result = a - b;
            result.WriteLine();
            System.Console.WriteLine(Environment.NewLine);
            // [ -4,  4,  0,  4,  4 ]
            // [ 14, -2, 14,  2, 14 ]
            // [ -4, 24,  0, 24,  4 ]

            result = -a;
            result.WriteLine();
            System.Console.WriteLine(Environment.NewLine);
            // [   0,  -1,  -2,  -3,  -4 ]
            // [  -5,  -6,  -7,  -8,  -9 ]
            // [ -10, -11, -12, -13, -14 ]

            result = 3.0 * a;
            result.WriteLine();
            System.Console.WriteLine(Environment.NewLine);
            // [  0,  3,  6,  9, 12 ]
            // [ 15, 18, 21, 24, 27 ]
            // [ 30, 33, 36, 39, 42 ]

            result = a * c; // (3x5) * (5x3) = (3x3)
            result.WriteLine();
            System.Console.WriteLine(Environment.NewLine);
            // [  90, 100, 110 ]
            // [ 240, 275, 310 ]
            // [ 390, 450, 510 ]

            result = c * a; // (5x3) * (3x5) = (5x5)
            result.WriteLine();
            System.Console.WriteLine(Environment.NewLine);
            // [  25,  28,  31,  34,  37 ]
            // [  70,  82,  94, 106, 118 ]
            // [ 115, 136, 157, 178, 199 ]
            // [ 160, 190, 220, 250, 280 ]
            // [ 205, 244, 283, 322, 361 ]




            // Examples of multiplying by an identity matrix:

            System.Console.WriteLine(Environment.NewLine);
            System.Console.WriteLine("Test of multiplying by an identity matrix:");
            System.Console.WriteLine(Environment.NewLine);

            MatrixF64 identity3x3 = MatrixF64.Identity(3);
            identity3x3.WriteLine();
            System.Console.WriteLine(Environment.NewLine);
            // [ 1, 0, 0 ]
            // [ 0, 1, 0 ]
            // [ 0, 0, 1 ]

            MatrixF64 s3x3 =
                new MatrixF64
                (
                3, 3,
                0.0, 1.0, 2.0, // row 0
                3.0, 4.0, 5.0, // row 1
                6.0, 7.0, 8.0  // row 2
                );

            result = s3x3 * identity3x3;
            result.WriteLine();
            System.Console.WriteLine(Environment.NewLine);
            // [ 0, 1, 2 ]
            // [ 3, 4, 5 ]
            // [ 6, 7, 8 ]
            // (i.e., the same as s3x3)

            result = identity3x3 * s3x3;
            result.WriteLine();
            System.Console.WriteLine(Environment.NewLine);
            // [ 0, 1, 2 ]
            // [ 3, 4, 5 ]
            // [ 6, 7, 8 ]
            // (i.e., the same as s3x3)




            // Example of LU factoring and back-substitution:

            System.Console.WriteLine(Environment.NewLine);
            System.Console.WriteLine("Test of LU factoring and back-substitution:");
            System.Console.WriteLine(Environment.NewLine);

            MatrixF64 a3x3 =
                new MatrixF64
                (
                    3, 3,
                    0.0, 1.0, 2.0, // row 0
                    3.0, 4.0, 5.0, // row 1
                    6.0, 7.0, 8.0  // row 2
                );

            int[] rowPermutationOfOriginalMatrix = null;
            bool rowInterchangeParityIsOdd = false;
            MatrixF64 a3x3LU = null;
            MatrixF64.FindLUFactorsOfARowPermutedVersionOfAnOriginalMatrix
            (
                a3x3,
                ref a3x3LU,
                ref rowPermutationOfOriginalMatrix,
                ref rowInterchangeParityIsOdd
            );

            a3x3LU.WriteLine();
            System.Console.WriteLine(Environment.NewLine);
            // [     6,     7,     8 ]
            // [     0,     1,     2 ]
            // [   0.5,   0.5, 1e-19 ]
            // The matrix above is a combination of the L and U
            // matrices. The L matrix is 0 above the diagonal,
            // 1 on the diagonal, and the shown values below the
            // diagonal.  The U matrix is 0 below the diagonal,
            // and the shown values on and above the diagonal.
            // This LU factoring is of a ROW-PERMUTED version
            // of the original matrix.

            // The following L and U matrices are manually formed
            // by inspecting the LU factoring matrix above.
            MatrixF64 L3x3 =
                new MatrixF64
                (
                    3, 3,
                    1.0, 0.0, 0.0, // row 0
                    0.0, 1.0, 0.0, // row 1
                    0.5, 0.5, 1.0  // row 2
                );

            MatrixF64 U3x3 =
                new MatrixF64
                (
                    3, 3,
                    6.0, 7.0, 8.0, // row 0
                    0.0, 1.0, 2.0, // row 1
                    0.0, 0.0, 0.0  // row 2
                );

            // The product (L)*(U) produces a ROW-PERMUTED version
            // of the original matrix:
            result = L3x3 * U3x3;
            result.WriteLine(4);
            System.Console.WriteLine(Environment.NewLine);
            // [ 6, 7, 8 ]
            // [ 0, 1, 2 ]
            // [ 3, 4, 5 ]

            VectorF64 b3x1 = new VectorF64(1.0, 2.0, 3.0);
            VectorF64 x3x1 = null;

            MatrixF64.LUBacksubstitution
            (
                a3x3LU,
                rowPermutationOfOriginalMatrix,
                b3x1,
                ref x3x1
            );

            // Solution vector
            x3x1.WriteLine();
            System.Console.WriteLine(Environment.NewLine);
            // ( -0.66666667,           1,           0 )
            // Both the given b3x1 and this x3x1 are relative to the
            // original matrix, a3x3.  The a3x3LU for a ROW-PERMUTED
            // version of a3x3, but LUBacksubstitution accepts a
            // non-permuted product vector (e.g., b3x1) and produces
            // a non-permuted solution vector (e.g., x3x1).

            // Check solution by multiplying the original matrix a3x3
            // by the solution vector x3x1, to get a product vector.
            VectorF64 checkb3x1 = a3x3 * x3x1;
            checkb3x1.WriteLine();
            System.Console.WriteLine(Environment.NewLine);
            // ( 1, 2, 3 )
            // This matches the b3x1 vector we specified above, so
            // the solution is valid.




            // Examples of matrix determinants:

            System.Console.WriteLine(Environment.NewLine);
            System.Console.WriteLine("Test of matrix determinants:");
            System.Console.WriteLine(Environment.NewLine);

            MatrixF64 d2x2 =
                new MatrixF64
                (
                2, 2,
                 2.0, 5.0, // row 0
                -4.0, -7.0  // row 1
                );
            double detd2x2 = d2x2.Determinant();
            System.Console.WriteLine(detd2x2);
            System.Console.WriteLine(Environment.NewLine);
            // detd2x2 = 6

            MatrixF64 d3x3 =
                new MatrixF64
                (
                3, 3,
                 2.0, 5.0, 7.0, // row 0
                -4.0, -1.0, 6.0, // row 1
                 9.0, 8.0, 3.0  // row 2
                );
            double detd3x3 = d3x3.Determinant();
            System.Console.WriteLine(detd3x3);
            System.Console.WriteLine(Environment.NewLine);
            // detd3x3 = 67

            MatrixF64 d4x4 =
                new MatrixF64
                (
                4, 4,
                  7.0, -5.0, 2.0, 4.0, // row 0
                  3.0, 2.0, 6.0, 3.0, // row 1
                 -9.0, 8.0, -3.0, 2.0, // row 2
                  5.0, 3.0, 2.0, 5.0  // row 3
                );
            double detd4x4 = d4x4.Determinant();
            System.Console.WriteLine(detd4x4);
            System.Console.WriteLine(Environment.NewLine);
            // detd4x4 = 1457





            // Examples of matrix inverses:

            System.Console.WriteLine(Environment.NewLine);
            System.Console.WriteLine("Test of matrix inverses:");
            System.Console.WriteLine(Environment.NewLine);

            MatrixF64 m2x2 =
                new MatrixF64
                (
                2, 2,
                 2.0, 5.0, // row 0
                -4.0, -7.0  // row 1
                );
            MatrixF64 inversem2x2 = m2x2.Inverse();
            inversem2x2.WriteLine();
            System.Console.WriteLine(Environment.NewLine);
            // [  -1.1666667, -0.83333333 ]
            // [  0.66666667,  0.33333333 ]

            MatrixF64 prodinvm2x2andm2x2 = inversem2x2 * m2x2;
            prodinvm2x2andm2x2.WriteLine();
            System.Console.WriteLine(Environment.NewLine);
            // [             1, 8.8817842e-16 ]
            // [             0,             1 ]
            // This product is very close to a 
            // 2x2 identity matrix, as it should be.

            MatrixF64 prodm2x2andinvm2x2 = m2x2 * inversem2x2;
            prodm2x2andinvm2x2.WriteLine();
            System.Console.WriteLine(Environment.NewLine);
            // [ 1, 0 ]
            // [ 0, 1 ]
            // This product is the 2x2 identity matrix,
            // as it should be.


            MatrixF64 m3x3 =
                new MatrixF64
                (
                3, 3,
                 2.0, 5.0, 7.0, // row 0
                -4.0, -1.0, 6.0, // row 1
                 9.0, 8.0, 3.0  // row 2
                );
            MatrixF64 inversem3x3 = m3x3.Inverse();
            inversem3x3.WriteLine();
            System.Console.WriteLine(Environment.NewLine);
            // [ -0.76119403,   0.6119403,  0.55223881 ]
            // [  0.98507463, -0.85074627, -0.59701493 ]
            // [ -0.34328358,  0.43283582,  0.26865672 ]

            MatrixF64 prodinvm3x3andm3x3 = inversem3x3 * m3x3;
            prodinvm3x3andm3x3.WriteLine();
            System.Console.WriteLine(Environment.NewLine);
            // [             1,             0, 4.4408921e-16 ]
            // [             0,             1,             0 ]
            // [             0,             0,             1 ]
            // This product is very close to a 3x3 identity matrix,
            // as it should be.

            MatrixF64 prodm3x3andinvm3x3 = m3x3 * inversem3x3;
            prodm3x3andinvm3x3.WriteLine();
            System.Console.WriteLine(Environment.NewLine);
            // [              1, -4.4408921e-16,  4.4408921e-16 ]
            // [              0,              1,  -2.220446e-16 ]
            // [ -4.4408921e-16,              0,              1 ]
            // This product is very close to a 3x3 identity matrix,
            // as it should be.




            // Examples of a product of a matrix and a vector:

            System.Console.WriteLine(Environment.NewLine);
            System.Console.WriteLine("Test of a product of a matrix and a vector:");
            System.Console.WriteLine(Environment.NewLine);

            MatrixF64 t3x3 =
                new MatrixF64
                (
                3, 3,
                 2.0, 5.0, 7.0, // row 0
                -4.0, -1.0, 6.0, // row 1
                 9.0, 8.0, 3.0  // row 2
                );

            VectorF64 p3x1 = new VectorF64(1.0, 2.0, 3.0);

            VectorF64 q3x1 = t3x3 * p3x1;
            q3x1.WriteLine();
            System.Console.WriteLine(Environment.NewLine);
            // ( 33, 12, 34 )


            // The multiplicative product of the inverse 
            // of the matrix and the result vector computed
            // above should produce the original vector.
            MatrixF64 inverset3x3 = t3x3.Inverse();

            VectorF64 prodinvt3x3andq3x1 = inverset3x3 * q3x1;
            prodinvt3x3andq3x1.WriteLine();
            System.Console.WriteLine(Environment.NewLine);
            // ( 1, 2, 3 )
            // This vector is equal to the original vector,
            // as expected.




            // Example of using a homogeneous matrix to 
            // translate a homogeneous vector:

            System.Console.WriteLine(Environment.NewLine);
            System.Console.WriteLine("Test of using a homogeneous matrix to translate a homogeneous vector:");
            System.Console.WriteLine(Environment.NewLine);

            // Forming a homogeneous matrix that represents
            // a translation:

            VectorF64 homogeneousTranslationVector4x1 =
                new VectorF64(10.0, 20.0, 30.0, 1.0);

            MatrixF64 translation4x4 =
                MatrixF64.FormHomogeneousTranslationMatrix
                    (homogeneousTranslationVector4x1);

            translation4x4.WriteLine();
            System.Console.WriteLine(Environment.NewLine);
            // [  1,  0,  0, 10 ]
            // [  0,  1,  0, 20 ]
            // [  0,  0,  1, 30 ]
            // [  0,  0,  0,  1 ]


            // Using the homogeneous matrix to translate
            // a homogeneous vector:

            VectorF64 homogeneousVector =
                new VectorF64(5.0, 6.0, 7.0, 1.0);

            VectorF64 translatedVector =
                translation4x4 * homogeneousVector;

            translatedVector.WriteLine();
            System.Console.WriteLine(Environment.NewLine);
            // ( 15, 26, 37,  1 )
            // The relevant vector components (5,6,7) 
            // were translated by (10,20,30) by the
            // homogeneous translation matrix.







            // Example of using a homogeneous matrix
            // to rotate a homogeneous vector:

            System.Console.WriteLine(Environment.NewLine);
            System.Console.WriteLine("Test of using a homogeneous matrix to rotate a homogeneous vector:");
            System.Console.WriteLine(Environment.NewLine);

            // Forming a homogeneous rotation matrix that
            // rotates a positive component along coordinate
            // axis 0 to a positive component along 
            // cooridnate axis 1, in 3-dimensional space.
            // (The homogeneous matrix must be (3+1)=4
            // rows by (3+1)=4 columns.)

            MatrixF64 rotation4x4 =
                MatrixF64.Rab(4, 0, 1, (0.5 * Math.PI));

            rotation4x4.WriteLine(3);
            System.Console.WriteLine(Environment.NewLine);
            // [ 6.12e-17,       -1,        0,        0 ]
            // [        1, 6.12e-17,        0,        0 ]
            // [        0,        0,        1,        0 ]
            // [        0,        0,        0,        1 ]
            // (The entries '6.12e-17' are negligible.)
            // The upper-left 3x3 part of the matrix represents
            // a counterclockwise rotation of a quarter-turn
            // such that a positive component along axis 0
            // would be rotated to a positive component along
            // axis 1, in 3-dimensional space.

            // Using the homogeneous rotation matrix to 
            // rotate a homogeneous vector:

            VectorF64 homogeneousVector2 =
                new VectorF64(5.0, 6.0, 7.0, 1.0);

            VectorF64 rotatedVector =
                rotation4x4 * homogeneousVector2;

            rotatedVector.WriteLine();
            System.Console.WriteLine(Environment.NewLine);
            // ( -6,  5,  7,  1 )
            // The relevant vector components (5,6,7) were 
            // rotated to (-6,5,7), which is consistent with
            // a counterclockwise quarter-turn rotation 
            // with a rotation plane parallel to axes 0 and 1.



            System.Console.WriteLine(Environment.NewLine);
            System.Console.WriteLine(String.Empty.PadRight(78, '#'));
            System.Console.WriteLine(Environment.NewLine);
        }

    }
}
