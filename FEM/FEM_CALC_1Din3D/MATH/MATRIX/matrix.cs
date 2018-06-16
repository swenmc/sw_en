/*
 * Author: Syed Mehroz Alam
 * Email: smehrozalam@yahoo.com
 * URL: Programming Home "http://www.geocities.com/smehrozalam/" 
 * Date: 6/20/2004
 * Time: 4:46 PM
 *
 */

using System;

namespace FEM_CALC_1D2
{
	/// <summary>
	/// Classes Contained:
	/// 	Matrix
	/// 	MatrixException
	/// 	MatrixApplication
	/// 	Fraction
	/// 	FractionException
	/// </summary>



	/// Class name: Matrix
	/// Class used: Fraction
	/// Developed by: Syed Mehroz Alam
	/// Email: smehrozalam@yahoo.com
	/// URL: Programming Home "http://www.geocities.com/smehrozalam/"
	/// 
	/// Constructors:
	/// 	( Fraction[,] ):	takes 2D Fractions array	
	/// 	( int[,] ):	takes 2D integer array, convert them to fractions	
	/// 	( double[,] ):	takes 2D double array, convert them to fractions
	/// 	( int Rows, int Cols )	initializes the dimensions, indexers may be used 
	/// 							to set individual elements' values
	/// 
	/// Properties:
	/// 	Rows: read only property to get the no. of rows in the current matrix
	/// 	Cols: read only property to get the no. of columns in the current matrix
	/// 
	/// Indexers:
	/// 	[i,j] = used to set/get elements in the form of a Fraction object
	/// 
	/// Public Methods (Description is given with respective methods' definitions)
	/// 	string ConvertToString(Matrix)
	/// 	Matrix Minor(Matrix, Row,Col)
	/// 	Fraction Determinent(Matrix)
	/// 	MultiplyRow( Row, Fraction )
	/// 	MultiplyRow( Row, integer )
	/// 	MultiplyRow( Row, double )
	/// 	AddRow( TargetRow, SecondRow, Multiple)
	/// 	InterchangeRow( Row1, Row2)
	/// 	Matrix Concatenate(Matrix1, Matrix2)
	/// 	Matrix EchelonForm(Matrix)
	/// 	Matrix ReducedEchelonForm(Matrix)
	/// 	Matrix Inverse(Matrix)
	/// 	Matrix Adjoint(Matrix)
	/// 	Matrix Transpose(Matrix)
	/// 	Matrix Duplicate()
	/// 	Matrix ScalarMatrix( Rows, Cols, K )
	/// 	Matrix IdentityMatrix( Rows, Cols )
	/// 	Matrix UnitMatrix(Rows, Cols)
	/// 	Matrix NullMatrix(Rows, Cols)
	/// 
	/// Private Methods
	/// 	Fraction GetElement(int iRow, int iCol)
	/// 	SetElement(int iRow, int iCol, Fraction value)
	/// 	Negate(Matrix)
	/// 	Add(Matrix1, Matrix2)
	/// 	Multiply(Matrix1, Matrix2)
	/// 	Multiply(Matrix1, Fraction)
	/// 	Multiply(Matrix1, integer)
	/// 
	/// Operators Overloaded Overloaded
	/// 	Unary: - (negate matrix)
	/// 	Binary: 
	/// 		+,- for two matrices
	/// 		* for two matrices or one matrix with integer or fraction or double
	/// 		/ for matrix with integer or fraction or double
	/// </summary>
	public class Matrix
	{
		/// <summary>
		/// Class attributes/members
		/// </summary>
		int m_iRows;
		int m_iCols;
		Fraction[,] m_iElement;
		

		/// <summary>
		/// Constructors
		/// </summary>
		public Matrix(Fraction[,] elements)
		{
			m_iElement=elements;
			m_iRows=elements.GetLength(0);
			m_iCols=elements.GetLength(1);
		}
		
		public Matrix(int[,] elements)
		{
			m_iRows=elements.GetLength(0);
			m_iCols=elements.GetLength(1);;
			m_iElement=new Fraction[m_iRows,m_iCols];
			for(int i=0;i<elements.GetLength(0);i++)
			{
				for(int j=0;j<elements.GetLength(1);j++)
				{
					this[i,j]=new Fraction( elements[i,j] );
				}
			}
		}

        public Matrix(float[,] elements)
        {
            m_iRows = elements.GetLength(0);
            m_iCols = elements.GetLength(1); ;
            m_iElement = new Fraction[m_iRows, m_iCols];
            for (int i = 0; i < elements.GetLength(0); i++)
            {
                for (int j = 0; j < elements.GetLength(1); j++)
                {
                    this[i, j] = Fraction.ConvertToFraction(elements[i, j]);
                }
            }
        }

		public Matrix(double[,] elements)
		{
			m_iRows=elements.GetLength(0);
			m_iCols=elements.GetLength(1);;
			m_iElement=new Fraction[m_iRows,m_iCols];
			for(int i=0;i<elements.GetLength(0);i++)
			{
				for(int j=0;j<elements.GetLength(1);j++)
				{
					this[i,j]=Fraction.ConvertToFraction( elements[i,j] );
				}
			}
		}
		
		public Matrix(int iRows, int iCols)
		{
			m_iRows=iRows;
			m_iCols=iCols;
			m_iElement=new Fraction[iRows,iCols];
		}

        public void PrintMatrix()
        {

            for (int i = 0; i < m_iRows; i++)
            {
                for (int j = 0; j < m_iCols; j++)
                {
                    Console.WriteLine(m_iElement[i, j].ToString() + "/t");
                }

                Console.WriteLine("/n");
            }
        }
	
		/// <summary>
		/// Properites
		/// </summary>
		public int Rows
		{
			get	{ return m_iRows;	}
		}	
	
		public int Cols
		{
			get	{ return m_iCols;	}
		}	
		
		/// <summary>
		/// Indexer
		/// </summary>
		public Fraction this[int iRow, int iCol]		// matrix's index starts at 0,0
		{
			get	{	return GetElement(iRow,iCol);	}
			set	{	SetElement(iRow,iCol,value);	}
		}

		/// <summary>
		/// Internal functions for getting/setting values
		/// </summary>
		private Fraction GetElement(int iRow, int iCol)
		{
				if ( iRow<0 || iRow>Rows-1 || iCol<0 || iCol>Cols-1 )
					throw new MatrixException("Invalid index specified");
				return m_iElement[iRow,iCol];
		}
	
		private void SetElement(int iRow, int iCol, Fraction value)
		{
				if ( iRow<0 || iRow>Rows-1 || iCol<0 || iCol>Cols-1 )
					throw new MatrixException("Invalid index specified");
				m_iElement[iRow,iCol]=value.Duplicate();
		}

		
		/// <summary>
		/// The function returns the current Matrix object as a string
		/// </summary>
		public string ConvertToString()
		{
			return (Matrix.ConvertToString(this));
		}
		
		/// <summary>
		/// The function takes a Matrix object and returns it as a string
		/// </summary>
		public static string ConvertToString(Matrix matrix)
		{
			string str="";
			for (int i=0;i<matrix.Rows;i++)
			{
				for (int j=0;j<matrix.Cols;j++)
					str+=matrix[i,j].ConvertToString()+"\t";
				str+="\n";
			}
			return str;
		}
		
		/// <summary>
		/// The function return the Minor of element[Row,Col] of a Matrix object 
		/// </summary>
		public static Matrix Minor(Matrix matrix, int iRow, int iCol)
		{
			Matrix minor=new Matrix(matrix.Rows-1, matrix.Cols-1);
			int m=0,n=0;
			for (int i=0;i<matrix.Rows;i++)
			{
				if (i==iRow)
					continue;
				n=0;
				for (int j=0;j<matrix.Cols;j++)
				{
					if (j==iCol) 
						continue;
					minor[m,n]=matrix[i,j];
					n++;	
				}
				m++;
			}
			return minor;
		}
			
			
		
		/// <summary>
		/// The function returns the determinent of a Matrix object as Fraction
		/// </summary>
		public static Fraction Determinent(Matrix matrix)
		{
			Fraction det=new Fraction(0);
			if (matrix.Rows!=matrix.Cols)
				throw new MatrixException("Determinent of a non-square matrix doesn't exist");
			if (matrix.Rows==1)
				return matrix[0,0];
			for (int j=0;j<matrix.Cols;j++)
				det+=(matrix[0,j]*Determinent(Matrix.Minor(matrix, 0,j))*(int)System.Math.Pow(-1,0+j));
			return det;
		}
	
		/// <summary>
		/// The function returns the determinent of the current Matrix object as Fraction
		/// </summary>
		public Fraction Determinent()
		{
			return Determinent(this);
		}
		
		/// <summary>
		/// The function multiplies the given row of the current matrix object by a Fraction 
		/// </summary>
		public void MultiplyRow(int iRow, Fraction frac)
		{
			for (int j=0;j<this.Cols;j++)
			{
				this[iRow,j]*=frac;
				Fraction.ReduceFraction(this[iRow,j]);
			}
		}

		/// <summary>
		/// The function multiplies the given row of the current matrix object by an integer
		/// </summary>
		public void MultiplyRow(int iRow, int iNo)
		{
			this.MultiplyRow(iRow, new Fraction(iNo));
		}

		/// <summary>
		/// The function multiplies the given row of the current matrix object by a double
		/// </summary>
		public void MultiplyRow(int iRow, double dbl)
		{
			this.MultiplyRow(iRow, Fraction.ConvertToFraction(dbl));
		}
		
		/// <summary>
		/// The function adds two rows for current matrix object
		/// It performs the following calculation:
		/// iTargetRow = iTargetRow + iMultiple*iSecondRow
		/// </summary>
		public void AddRow(int iTargetRow, int iSecondRow, Fraction iMultiple)
		{
			for (int j=0;j<this.Cols;j++)
				this[iTargetRow,j]+=(this[iSecondRow,j]*iMultiple);
		}

		/// <summary>
		/// The function interchanges two rows of the current matrix object
		/// </summary>
		public void InterchangeRow(int iRow1, int iRow2)
		{
			for (int j=0;j<this.Cols;j++)
			{
				Fraction temp=this[iRow1,j];
				this[iRow1,j]=this[iRow2,j];
				this[iRow2,j]=temp;
			}
		}
		
		/// <summary>
		/// The function concatenates the two given matrices column-wise
		/// </summary>
		public static Matrix Concatenate(Matrix matrix1, Matrix matrix2)
		{
			if (matrix1.Rows!=matrix2.Rows)
				throw new MatrixException("Concatenation not possible");
			Matrix matrix=new Matrix(matrix1.Rows, matrix1.Cols+matrix2.Cols );
			for (int i=0;i<matrix.Rows;i++)
				for (int j=0;j<matrix.Cols;j++)
				{
					if ( j<matrix1.Cols )
						matrix[i,j]=matrix1[i,j];
					else 
						matrix[i,j]=matrix2[i,j-matrix1.Cols];
				}	
			return matrix;
		}

		/// <summary>
		/// The function returns the Echelon form of a given matrix
		/// </summary>
		public static Matrix EchelonForm(Matrix matrix)
		{
			try
			{
				Matrix EchelonMatrix=matrix.Duplicate();
				for (int i=0;i<matrix.Rows;i++)
				{	
					if (EchelonMatrix[i,i]==0)	// if diagonal entry is zero, 
						for (int j=i+1;j<EchelonMatrix.Rows;j++)
							if (EchelonMatrix[j,i]!=0)	 //check if some below entry is non-zero
								EchelonMatrix.InterchangeRow(i,j);	// then interchange the two rows
					if (EchelonMatrix[i,i]==0)	// if not found any non-zero diagonal entry
						continue;	// increment i;
					if ( EchelonMatrix[i,i]!=1)	// if diagonal entry is not 1 , 	
						for (int j=i+1;j<EchelonMatrix.Rows;j++)
							if (EchelonMatrix[j,i]==1)	 //check if some below entry is 1
								EchelonMatrix.InterchangeRow(i,j);	// then interchange the two rows
					EchelonMatrix.MultiplyRow(i, Fraction.Inverse(EchelonMatrix[i,i]));
					for (int j=i+1;j<EchelonMatrix.Rows;j++)
						EchelonMatrix.AddRow( j, i, -EchelonMatrix[j,i]);
				}
				return EchelonMatrix;
			}
			catch(Exception)
			{
				throw new MatrixException("Matrix can not be reduced to Echelon form");
			}
		}

		/// <summary>
		/// The function returns the Echelon form of the current matrix
		/// </summary>
		public Matrix EchelonForm()
		{
			return EchelonForm(this);
		}

		/// <summary>
		/// The function returns the reduced echelon form of a given matrix
		/// </summary>
		public static Matrix ReducedEchelonForm(Matrix matrix)
		{
			try
			{
				Matrix ReducedEchelonMatrix=matrix.Duplicate();
				for (int i=0;i<matrix.Rows;i++)
				{	
					if (ReducedEchelonMatrix[i,i]==0)	// if diagonal entry is zero, 
						for (int j=i+1;j<ReducedEchelonMatrix.Rows;j++)
							if (ReducedEchelonMatrix[j,i]!=0)	 //check if some below entry is non-zero
								ReducedEchelonMatrix.InterchangeRow(i,j);	// then interchange the two rows
					if (ReducedEchelonMatrix[i,i]==0)	// if not found any non-zero diagonal entry
						continue;	// increment i;
					if ( ReducedEchelonMatrix[i,i]!=1)	// if diagonal entry is not 1 , 	
						for (int j=i+1;j<ReducedEchelonMatrix.Rows;j++)
							if ( ReducedEchelonMatrix[j,i]==1 )	 //check if some below entry is 1
								ReducedEchelonMatrix.InterchangeRow(i,j);	// then interchange the two rows
					ReducedEchelonMatrix.MultiplyRow(i, Fraction.Inverse(ReducedEchelonMatrix[i,i]));
					for (int j=i+1;j<ReducedEchelonMatrix.Rows;j++)
						ReducedEchelonMatrix.AddRow( j, i, -ReducedEchelonMatrix[j,i]);
					for (int j=i-1;j>=0;j--)
						ReducedEchelonMatrix.AddRow( j, i, -ReducedEchelonMatrix[j,i]);
				}
				return ReducedEchelonMatrix;
			}
			catch(Exception)
			{
				throw new MatrixException("Matrix can not be reduced to Echelon form");
			}
		}

		/// <summary>
		/// The function returns the reduced echelon form of the current matrix
		/// </summary>
		public Matrix ReducedEchelonForm()
		{
			return ReducedEchelonForm(this);
		}

		/// <summary>
		/// The function returns the inverse of a given matrix
		/// </summary>
		public static Matrix Inverse(Matrix matrix)
		{
			if ( Determinent(matrix)==0 )
				throw new MatrixException("Inverse of a singular matrix is not possible");
			return ( Adjoint(matrix)/Determinent(matrix) );
		}
		
		/// <summary>
		/// The function returns the inverse of the current matrix
		/// </summary>
		public Matrix Inverse()
		{
			return Inverse(this);
		}

		/// <summary>
		/// The function returns the adjoint of a given matrix
		/// </summary>
		public static Matrix Adjoint(Matrix matrix)
		{
			if (matrix.Rows!=matrix.Cols)
				throw new MatrixException("Adjoint of a non-square matrix does not exists");
			Matrix AdjointMatrix=new Matrix(matrix.Rows, matrix.Cols);
			for (int i=0;i<matrix.Rows;i++)
				for (int j=0;j<matrix.Cols;j++)
					AdjointMatrix[i,j]=Math.Pow(-1,i+j)*Determinent(Minor(matrix, i,j));
			AdjointMatrix=Transpose(AdjointMatrix);
			return AdjointMatrix;
		}
		
		/// <summary>
		/// The function returns the adjoint of the current matrix
		/// </summary>
		public Matrix Adjoint()
		{
			return Adjoint(this);
		}
		
		/// <summary>
		/// The function returns the transpose of a given matrix
		/// </summary>
		public static Matrix Transpose(Matrix matrix)
		{
			Matrix TransposeMatrix=new Matrix(matrix.Cols, matrix.Rows);
			for (int i=0;i<TransposeMatrix.Rows;i++)
				for (int j=0;j<TransposeMatrix.Cols;j++)
					TransposeMatrix[i,j]=matrix[j,i];
			return TransposeMatrix;
		}
		
		/// <summary>
		/// The function returns the transpose of the current matrix
		/// </summary>
		public Matrix Transpose()
		{
			return Transpose(this);
		}
		
		/// <summary>
		/// The function duplicates the current Matrix object
		/// </summary>
		public Matrix Duplicate()
		{
			Matrix matrix=new Matrix(Rows,Cols);
			for (int i=0;i<Rows;i++)
				for (int j=0;j<Cols;j++)
					matrix[i,j]=this[i,j];
			return matrix;
		}
	
		/// <summary>
		/// The function returns a Scalar Matrix of dimension ( Row x Col ) and scalar K
		/// </summary>
		public static Matrix ScalarMatrix(int iRows, int iCols, int K)
		{
			Fraction zero=new Fraction(0);
			Fraction scalar=new Fraction(K);
			Matrix matrix=new Matrix(iRows,iCols);
			for (int i=0;i<iRows;i++)
				for (int j=0;j<iCols;j++)
				{
					if (i==j)
						matrix[i,j]=scalar;
					else
						matrix[i,j]=zero;
				}
			return matrix;
		}

		/// <summary>
		/// The function returns an identity matrix of dimensions ( Row x Col )
		/// </summary>
		public static Matrix IdentityMatrix(int iRows, int iCols)
		{
			return ScalarMatrix(iRows, iCols, 1);
		}

		/// <summary>
		/// The function returns a Unit Matrix of dimension ( Row x Col )
		/// </summary>
		public static Matrix UnitMatrix(int iRows, int iCols)
{
			Fraction temp=new Fraction(1);
			Matrix matrix=new Matrix(iRows,iCols);
			for (int i=0;i<iRows;i++)
				for (int j=0;j<iCols;j++)
					matrix[i,j]=temp;
			return matrix;
		}
		
		/// <summary>
		/// The function returns a Null Matrix of dimension ( Row x Col )
		/// </summary>
		public static Matrix NullMatrix(int iRows, int iCols)
		{
			Fraction temp=new Fraction(0);
			Matrix matrix=new Matrix(iRows,iCols);
			for (int i=0;i<iRows;i++)
				for (int j=0;j<iCols;j++)
					matrix[i,j]=temp;
			return matrix;
		}
		
		/// <summary>
		/// Operators for the Matrix object
		/// includes -(unary), and binary opertors such as +,-,*,/
		/// </summary>
		public static Matrix operator -(Matrix matrix)
		{	return Matrix.Negate(matrix);	}
		
		public static Matrix operator +(Matrix matrix1, Matrix matrix2)
		{	return Matrix.Add(matrix1, matrix2);	}
		
		public static Matrix operator -(Matrix matrix1, Matrix matrix2)
		{	return Matrix.Add(matrix1, -matrix2);	}
		
		public static Matrix operator *(Matrix matrix1, Matrix matrix2)
		{	return Matrix.Multiply(matrix1, matrix2);	}
	
		public static Matrix operator *(Matrix matrix1, int iNo)
		{	return Matrix.Multiply(matrix1, iNo);	}
	
		public static Matrix operator *(Matrix matrix1, double dbl)
		{	return Matrix.Multiply(matrix1, Fraction.ConvertToFraction(dbl));	}
	
		public static Matrix operator *(Matrix matrix1, Fraction frac)
		{	return Matrix.Multiply(matrix1, frac);	}
	
		public static Matrix operator *(int iNo, Matrix matrix1)
		{	return Matrix.Multiply(matrix1, iNo);	}
	
		public static Matrix operator *(double dbl, Matrix matrix1)
		{	return Matrix.Multiply(matrix1, Fraction.ConvertToFraction(dbl));	}

		public static Matrix operator *(Fraction frac, Matrix matrix1)
		{	return Matrix.Multiply(matrix1, frac);	}
	
		public static Matrix operator /(Matrix matrix1, int iNo)
		{	return Matrix.Multiply(matrix1, Fraction.Inverse(new Fraction(iNo)));	}
		
		public static Matrix operator /(Matrix matrix1, double dbl)
		{	return Matrix.Multiply(matrix1, Fraction.Inverse(Fraction.ConvertToFraction(dbl)));	}
		
		public static Matrix operator /(Matrix matrix1, Fraction frac)
		{	return Matrix.Multiply(matrix1, Fraction.Inverse(frac));	}
	
		
		/// <summary>
		/// Internal Fucntions for the above operators
		/// </summary>
		private static Matrix Negate(Matrix matrix)
		{
			return Matrix.Multiply(matrix,-1);
		}
		
		private static Matrix Add(Matrix matrix1, Matrix matrix2)
		{
			if (matrix1.Rows!=matrix2.Rows || matrix1.Cols!=matrix2.Cols)
				throw new MatrixException("Operation not possible");
			Matrix result=new Matrix(matrix1.Rows, matrix1.Cols);
			for (int i=0;i<result.Rows;i++)
				for (int j=0;j<result.Cols;j++)
					result[i,j]=matrix1[i,j]+matrix2[i,j];
			return result;
		}
		
		public static Matrix Multiply(Matrix matrix1, Matrix matrix2)
		{
			if ( matrix1.Cols!=matrix2.Rows )
				throw new MatrixException("Operation not possible");
			Matrix result=Matrix.NullMatrix(matrix1.Rows,matrix2.Cols);
			for (int i=0;i<result.Rows;i++)
				for (int j=0;j<result.Cols;j++)
					for (int k=0;k<matrix1.Cols;k++)
						result[i,j]+=matrix1[i,k]*matrix2[k,j];
			return result;						
		}
		
		private static Matrix Multiply(Matrix matrix, int iNo)
		{
			Matrix result=new Matrix(matrix.Rows,matrix.Cols);
			for (int i=0;i<matrix.Rows;i++)
				for (int j=0;j<matrix.Cols;j++)
					result[i,j]=matrix[i,j]*iNo;
			return result;
		}
		
		private static Matrix Multiply(Matrix matrix, Fraction frac)
		{
			Matrix result=new Matrix(matrix.Rows,matrix.Cols);
			for (int i=0;i<matrix.Rows;i++)
				for (int j=0;j<matrix.Cols;j++)
					result[i,j]=matrix[i,j]*frac;
			return result;
		}
	
	}	//end class Matrix
	
	/// <summary>
	/// Exception class for Matrix class, derived from System.Exception
	/// </summary>
	public class MatrixException : Exception
	{
		public MatrixException() : base()
		{}
	
		public MatrixException(string Message) : base(Message)
		{}
		
		public MatrixException(string Message, Exception InnerException) : base(Message, InnerException)
		{}
	}	// end class MatrixException

	/// <summary>
	/// class name: Application
	/// The class demonstrates the Matrix class
	/// </summary>
	public class MatrixApplication
	{
		public static void Main1()
		{
			try
			{
				Console.WriteLine("Press 'm' for Matrix demo, 'e' for Equation Solver demo");
				if ( Console.ReadLine().ToLower()=="e" )
					EquationSolver();
				else					
					MatrixDemo();
				Console.WriteLine("\n\nPress any key to exit");
			}	//end try
			catch (MatrixException exp)
			{
				Console.WriteLine("\nInternal Matrix Error: " + exp.Message);
			}
			catch (FractionException exp)
			{
				Console.WriteLine("\nInternal Fraction Error: " + exp.Message);
			}
			catch (Exception exp)
			{
				Console.WriteLine("\nSystem Error: " + exp.Message);
			}
			Console.ReadLine();
		}
		
		public static void MatrixDemo()
		{
			Console.WriteLine("Enter no. of Rows: ");
			int iRows=Convert.ToInt32(Console.ReadLine());
			Console.WriteLine("Enter no. of Cols: ");
			int iCols=Convert.ToInt32(Console.ReadLine());
			
			Matrix m1=new Matrix(iRows,iCols);
			Matrix m2=new Matrix(iRows,iCols);
			System.Random rnd=new System.Random();
			for ( int i=0;i<iRows;i++)
			{
				for ( int j=0;j<iCols;j++)
				{
					m1[i,j]=new Fraction( Convert.ToString( (rnd.Next()%10)-5 ) );
//					m1[i,j]=new Fraction(Console.ReadLine());
					m2[i,j]=new Fraction( Convert.ToString( (rnd.Next()%5.5)-2 ) );
				}
			}
			Console.WriteLine("\n\nLet the two matrices be:");
			Console.WriteLine( "\nm1\n"+ m1.ConvertToString() );
			Console.WriteLine( "\nm2\n"+ m2.ConvertToString() );
			Console.ReadLine();
			Console.WriteLine("\nm1+m2=\n"+ (m1+m2).ConvertToString());
			Console.WriteLine("\nm1*m2=\n");
			Console.WriteLine( (m1*m2).ConvertToString());
			Console.ReadLine();
			Console.WriteLine( "\nEchelon Form for m1:\n"+ m1.EchelonForm().ConvertToString() );
			Console.WriteLine( "\nReduced Echelon Form for m1:\n"+ m1.ReducedEchelonForm().ConvertToString() );
			Console.ReadLine();
			Console.WriteLine("\nDeterminent of m1="+ m1.Determinent().ConvertToString());
			Console.WriteLine("\nTranspose m1:\n"+ m1.Transpose().ConvertToString());
			Console.WriteLine("\nInverse of m1:\n "+ m1.Inverse().ConvertToString() );
		}	//end MatrixDemo()
		
		public static void EquationSolver()
		{
			Console.Write("Enter no. of variables: ");
			int iVarNo=Convert.ToInt32( Console.ReadLine() );
			string str;
			Matrix mat=new Matrix(iVarNo,iVarNo+1);
			Console.Write("\nEnter value of coefficents and constants (can be an integer(e.g 3), a floating point no(e.g. 12.25), or a fraction(e.g. 15/19)");
			int i,cnt;
			for (i=0;i<iVarNo+1;i++)
			{
				cnt=1;
				for (int k=0;k<i;k++)
				{
					Console.Write("\n"+"Equation "+(k+1)+": ");
					for (int nn=0;nn<iVarNo+1;nn++)
					{
						if (nn>0 && nn<iVarNo)
							Console.Write(" + ");
						if (nn<iVarNo)
							Console.Write("X"+(nn+1)+"*("+mat[k,nn].ConvertToString()+")");
						else 
							Console.Write(" = "+mat[k,nn].ConvertToString()  );
					}	
				}
				Console.Write("\n\n");
				if (i==iVarNo) continue;
				for(int j=0;j<iVarNo+1;j++,cnt++)
				{
					if (j>0 && j<iVarNo)
						Console.Write("+");
					if (j<iVarNo)
						Console.Write("X"+cnt+"*");
					else
						Console.Write("=");
					str=Console.ReadLine();
					if ( str.Length==0 )
						str="0";
					mat[i,j]=new Fraction(str);
				}
			}
			Console.WriteLine( "\naugmented matrix:\n"+ mat.ConvertToString() );
			Matrix ech=mat.EchelonForm();
			Console.WriteLine("\nechelon form:\n"+ech.ConvertToString());
			ech=mat.ReducedEchelonForm();
			for (i=0;i<iVarNo;i++)
				if ( ech[i,i]==0 )		// if not unique solution
				{
					Console.WriteLine("The given system of Equation does not produce any unique solution");
					return;
				}
			Console.Write("\nSolution for the given system:\n");
			for (i=0;i<iVarNo;i++)
					Console.Write("X"+(i+1)+"="+ech[i,iVarNo].ConvertToString()+"\t");
		}	//end EquationSolver()
	}	// end class MatrixApplication

	/// <summary>
	/// Class name: Fraction
	/// Developed by: Syed Mehroz Alam
	/// Email: smehrozalam@yahoo.com
	/// 
	/// Properties:
	/// 	Numerator: Set/Get value for Numerator
	/// 	Denominator:  Set/Get value for Numerator
	/// 	Value:  Set an integer value for the fraction
	/// 
	/// Constructors:
	/// 	no arguments:	initializes fraction as 0/1
	/// 	(Numerator, Denominator): initializes fraction with the given numerator and denominator values
	/// 	(integer):	initializes fraction with the given integer value
	/// 	(double):	initializes fraction with the given double value
	/// 	(string):	initializes fraction with the given string value
	/// 				the string can be an in the form of and integer, double or fraction.
	/// 				e.g it can be like "123" or "123.321" or "123/456"
	/// 
	/// Public Methods (Description is given with respective methods' definitions)
	/// 	string ConvertToString(Fraction)
	/// 	Fraction ConvertToFraction(string)
	/// 	Fraction ConvertToFraction(double)
	/// 	double ConvertToDouble(Fraction)
	/// 	Fraction Duplicate()
	/// 	Fraction Inverse(integer)
	/// 	Fraction Inverse(Fraction)
	/// 	ReduceFraction(Fraction)
	/// 	Equals(object)
	/// 	GetHashCode()
	/// 
	///	Private Methods (Description is given with respective methods' definitions)
	/// 	Initialize(Numerator, Denominator)
	/// 	Fraction Negate(Fraction)
	/// 	Fraction Add(Fraction1, Fraction2)
	/// 	int[] Factors(integer)
	/// 
	/// Operators Overloaded (overloaded for Fractions, Integers and Doubles)
	/// 	Unary: -
	/// 	Binary: +,-,*,/ 
	/// 	Relational and Logical Operators: ==,!=,<,>,<=,>=
	/// </summary>
	public class Fraction
	{
		/// <summary>
		/// Class attributes/members
		/// </summary>
		int m_iNumerator;
		int m_iDenominator;
		
		/// <summary>
		/// Constructors
		/// </summary>
		public Fraction()
		{
			Initialize(0,1);
		}
	
		public Fraction(int iWholeNumber)
		{
			Initialize(iWholeNumber, 1);
		}
	
		public Fraction(double dDecimalValue)
		{
			Fraction temp=ConvertToFraction(dDecimalValue);
			Initialize(temp.Numerator, temp.Denominator);
		}
		
		public Fraction(string strValue)
		{
			Fraction temp=ConvertToFraction(strValue);
			Initialize(temp.Numerator, temp.Denominator);
		}
		
		public Fraction(int iNumerator, int iDenominator)
		{
			Initialize(iNumerator, iDenominator);
		}
		
		/// <summary>
		/// Internal function for constructors
		/// </summary>
		private void Initialize(int iNumerator, int iDenominator)
		{
			Numerator=iNumerator;
			Denominator=iDenominator;
			ReduceFraction(this);
		}
	
		/// <summary>
		/// Properites
		/// </summary>
		public int Denominator
		{
			get
			{	return m_iDenominator;	}
			set
			{
				if (value!=0)
					m_iDenominator=value;
				else
					throw new FractionException("Denominator cannot be assigned a ZERO Value");
			}
		}
	
		public int Numerator
		{
			get	
			{	return m_iNumerator;	}
			set
			{	m_iNumerator=value;	}
		}
	
		public int Value
		{
			set
			{	m_iNumerator=value;
				m_iDenominator=1;	}
		}
	
		
		/// <summary>
		/// The function takes a Fraction object and returns its value as double
		/// </summary>
		public static double ConvertToDouble(Fraction frac)
		{
			return ( (double)frac.Numerator/frac.Denominator );
		}

		/// <summary>
		/// The function returns the current Fraction object as double
		/// </summary>
		public double ConvertToDouble()
		{
			return ( (double)this.Numerator/this.Denominator );
		}

		/// <summary>
		/// The function takes a Fraction object and returns it as a string
		/// </summary>
		public static string ConvertToString (Fraction frac1)
		{
			string str;
			if ( frac1.Denominator==1 )
				str=frac1.Numerator.ToString();
			else
				str=frac1.Numerator + "/" + frac1.Denominator;
			return str;
		}
	
		/// <summary>
		/// The function returns the current Fraction object as a string
		/// </summary>
		public string ConvertToString()
		{
			return Fraction.ConvertToString(this);
		}

		/// <summary>
		/// The function takes an string as an argument and returns its corresponding reduced fraction
		/// the string can be an in the form of and integer, double or fraction.
		/// e.g it can be like "123" or "123.321" or "123/456"
		/// </summary>
		public static Fraction ConvertToFraction(string strValue)
		{
			int i;
			for (i=0;i<strValue.Length;i++)
				if (strValue[i]=='/')
					break;
			
			if (i==strValue.Length)		// if string is not in the form of a fraction
				// then it is double or integer
				return ( ConvertToFraction( Convert.ToDouble(strValue) ) );
			
			// else string is in the form of Numerator/Denominator
			int iNumerator=Convert.ToInt32(strValue.Substring(0,i));
			int iDenominator=Convert.ToInt32(strValue.Substring(i+1));
			return new Fraction(iNumerator, iDenominator);
		}
		
		
		/// <summary>
		/// The function takes a floating point number as an argument 
		/// and returns its corresponding reduced fraction
		/// </summary>
		public static Fraction ConvertToFraction(double dValue)
		{
			try
			{
				Fraction frac;
				if (dValue>2147483647 && dValue<1.0/2147483647)
					throw new FractionException("Conversion not possible");
				if (dValue%1==0)	// if whole number
				{
					frac=new Fraction( (int) dValue );
				}
				else
				{
					double dTemp=dValue;
					int iMultiple=1;
					string strTemp=dValue.ToString();
					int i=0;
					while ( strTemp[i]!='.' )
						i++;
					int iDigitsAfterDecimal=strTemp.Length-i-1;
					while ( dTemp*10<2147483647 && iMultiple*10<2147483647 && iDigitsAfterDecimal>0  )
					{
						dTemp*=10;
						iMultiple*=10;
						iDigitsAfterDecimal--;
					}
					frac=new Fraction( (int)Math.Round(dTemp) , iMultiple );
				}
				return frac;
			}
			catch(Exception)
			{
				throw new FractionException("Conversion not possible");
			}
		}

		/// <summary>
		/// The function replicates current Fraction object
		/// </summary>
		public Fraction Duplicate()
		{
			Fraction frac=new Fraction();
			frac.Numerator=Numerator;
			frac.Denominator=Denominator;
			return frac;
		}

		/// <summary>
		/// The function returns the inverse of a Fraction object
		/// </summary>
		public static Fraction Inverse(Fraction frac1)
		{
			if (frac1.Numerator==0)
				throw new FractionException("Operation not possible (Denominator cannot be assigned a ZERO Value)");
	
			int iNumerator=frac1.Denominator;
			int iDenominator=frac1.Numerator;
			return ( new Fraction(iNumerator, iDenominator));
		}	
	

		/// <summary>
		/// Operators for the Fraction object
		/// includes -(unary), and binary opertors such as +,-,*,/
		/// also includes relational and logical operators such as ==,!=,<,>,<=,>=
		/// </summary>
		public static Fraction operator -(Fraction frac1)
		{	return ( Negate(frac1) );	}
	                              
		public static Fraction operator +(Fraction frac1, Fraction frac2)
		{	return ( Add(frac1 , frac2) );	}
	
		public static Fraction operator +(int iNo, Fraction frac1)
		{	return ( Add(frac1 , new Fraction(iNo) ) );	}
	
		public static Fraction operator +(Fraction frac1, int iNo)
		{	return ( Add(frac1 , new Fraction(iNo) ) );	}

		public static Fraction operator +(double dbl, Fraction frac1)
		{	return ( Add(frac1 , Fraction.ConvertToFraction(dbl) ) );	}
	
		public static Fraction operator +(Fraction frac1, double dbl)
		{	return ( Add(frac1 , Fraction.ConvertToFraction(dbl) ) );	}
	
		public static Fraction operator -(Fraction frac1, Fraction frac2)
		{	return ( Add(frac1 , -frac2) );	}
	
		public static Fraction operator -(int iNo, Fraction frac1)
		{	return ( Add(-frac1 , new Fraction(iNo) ) );	}
	
		public static Fraction operator -(Fraction frac1, int iNo)
		{	return ( Add(frac1 , -(new Fraction(iNo)) ) );	}

		public static Fraction operator -(double dbl, Fraction frac1)
		{	return ( Add(-frac1 , Fraction.ConvertToFraction(dbl) ) );	}
	
		public static Fraction operator -(Fraction frac1, double dbl)
		{	return ( Add(frac1 , -Fraction.ConvertToFraction(dbl) ) );	}
	
		public static Fraction operator *(Fraction frac1, Fraction frac2)
		{	return ( Multiply(frac1 , frac2) );	}
	
		public static Fraction operator *(int iNo, Fraction frac1)
		{	return ( Multiply(frac1 , new Fraction(iNo) ) );	}
	
		public static Fraction operator *(Fraction frac1, int iNo)
		{	return ( Multiply(frac1 , new Fraction(iNo) ) );	}
	
		public static Fraction operator *(double dbl, Fraction frac1)
		{	return ( Multiply(frac1 , Fraction.ConvertToFraction(dbl) ) );	}
	
		public static Fraction operator *(Fraction frac1, double dbl)
		{	return ( Multiply(frac1 , Fraction.ConvertToFraction(dbl) ) );	}
	
		public static Fraction operator /(Fraction frac1, Fraction frac2)
		{	return ( Multiply( frac1 , Inverse(frac2) ) );	}
	
		public static Fraction operator /(int iNo, Fraction frac1)
		{	return ( Multiply( Inverse(frac1) , new Fraction(iNo) ) );	}
	
		public static Fraction operator /(Fraction frac1, int iNo)
		{	return ( Multiply( frac1 , Inverse(new Fraction(iNo)) ) );	}
	
		public static Fraction operator /(double dbl, Fraction frac1)
		{	return ( Multiply( Inverse(frac1) , Fraction.ConvertToFraction(dbl) ) );	}
	
		public static Fraction operator /(Fraction frac1, double dbl)
		{	return ( Multiply( frac1 , Fraction.Inverse( Fraction.ConvertToFraction(dbl) ) ) );	}

		public static bool operator ==(Fraction frac1, Fraction frac2)
		{	return frac1.Equals(frac2);		}

		public static bool operator !=(Fraction frac1, Fraction frac2)
		{	return ( !frac1.Equals(frac2) );	}

		public static bool operator ==(Fraction frac1, int iNo)
		{	return frac1.Equals( new Fraction(iNo));	}

		public static bool operator !=(Fraction frac1, int iNo)
		{	return ( !frac1.Equals( new Fraction(iNo)) );	}
		
		public static bool operator ==(Fraction frac1, double dbl)
		{	return frac1.Equals( new Fraction(dbl));	}

		public static bool operator !=(Fraction frac1, double dbl)
		{	return ( !frac1.Equals( new Fraction(dbl)) );	}
		
		public static bool operator<(Fraction frac1, Fraction frac2)
		{	return frac1.Numerator * frac2.Denominator < frac2.Numerator * frac1.Denominator;	}

		public static bool operator>(Fraction frac1, Fraction frac2)
		{	return frac1.Numerator * frac2.Denominator > frac2.Numerator * frac1.Denominator;	}

		public static bool operator<=(Fraction frac1, Fraction frac2)
		{	return frac1.Numerator * frac2.Denominator <= frac2.Numerator * frac1.Denominator;	}
		
		public static bool operator>=(Fraction frac1, Fraction frac2)
		{	return frac1.Numerator * frac2.Denominator >= frac2.Numerator * frac1.Denominator;	}
		
		/// <summary>
		/// checks whether two fractions are equal
		/// </summary>
		public override bool Equals(object obj)
		{
			Fraction frac=(Fraction)obj;
			return ( Numerator==frac.Numerator && Denominator==frac.Denominator);
		}
		
		/// <summary>
		/// returns a hash code for this fraction
		/// </summary>
   		public override int GetHashCode()
   		{
			return ( Convert.ToInt32((Numerator ^ Denominator) & 0xFFFFFFFF) ) ;
		}

		/// <summary>
		/// internal function for negation
		/// </summary>
		private static Fraction Negate(Fraction frac1)
		{
			int iNumerator=-frac1.Numerator;
			int iDenominator=frac1.Denominator;
			return ( new Fraction(iNumerator, iDenominator) );

		}	

		/// <summary>
		/// internal functions for binary operations
		/// </summary>
		private static Fraction Add(Fraction frac1, Fraction frac2)
		{
			int iNumerator=frac1.Numerator*frac2.Denominator + frac2.Numerator*frac1.Denominator;
			int iDenominator=frac1.Denominator*frac2.Denominator;
			return ( new Fraction(iNumerator, iDenominator) );
		}
	
		private static Fraction Multiply(Fraction frac1, Fraction frac2)
		{
			int iNumerator=frac1.Numerator*frac2.Numerator;
			int iDenominator=frac1.Denominator*frac2.Denominator;
			return ( new Fraction(iNumerator, iDenominator) );
		}

		/// <summary>
		/// The function returns GCD of two numbers (used for reducing a Fraction)
		/// </summary>
		private static int GCD(int iNo1, int iNo2)
		{
			// take absolute values
			if (iNo1 < 0) iNo1 = -iNo1;
			if (iNo2 < 0) iNo2 = -iNo2;
			
			do
			{
				if (iNo1 < iNo2)
				{
					int tmp = iNo1;  // swap the two operands
					iNo1 = iNo2;
					iNo2 = tmp;
				}
				iNo1 = iNo1 % iNo2;
			} while (iNo1 != 0);
			return iNo2;
		}
	
		/// <summary>
		/// The function reduces(simplifies) a Fraction object by dividing both its numerator 
		/// and denominator by their GCD
		/// </summary>
		public static void ReduceFraction(Fraction frac)
		{
			try
			{
				if (frac.Numerator==0)
				{
					frac.Denominator=1;
					return;
				}
				
				int iGCD=GCD(frac.Numerator, frac.Denominator);
				frac.Numerator/=iGCD;
				frac.Denominator/=iGCD;
				
				if ( frac.Denominator<0 )	// if -ve sign in denominator
				{
					//pass -ve sign to numerator
					frac.Numerator*=-1;
					frac.Denominator*=-1;	
				}
			} // end try
			catch(Exception exp)
			{
				throw new FractionException("Cannot reduce Fraction: " + exp.Message);
			}
		}
			
	}	//end class Fraction


	/// <summary>
	/// Exception class for Fraction, derived from System.Exception
	/// </summary>
	public class FractionException : Exception
	{
		public FractionException() : base()
		{}
	
		public FractionException(string Message) : base(Message)
		{}
		
		public FractionException(string Message, Exception InnerException) : base(Message, InnerException)
		{}
	}	//end class FractionException

}	//end namespace Mehroz
