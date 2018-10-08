using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MATH
{
    //trieda je verejna staticka co znamena ze objekt tejto triedy sa nevytvara,ale pouziva sa podobne ako trieda Math
    public static class MathF
    {
        //----------------------------------------------------------------------------------------------------------------------------
        // Minimum and Maximum
        //----------------------------------------------------------------------------------------------------------------------------
        #region MinMax
        // Tieto metody potrebuju lubovolny pocet argumentov a z tychto argumentov vratia minimalny alebo maximalny prvok
        // int
        // tymto metodam by bolo potrebne najprv inicializovat pole a potom im ho odovzdat ako argument
        // metody sluzia pre najdenie minimalneho(maximalneho prvku v zozname Int)
        public static int Min(List<int> data)
        {
            int min;
            min = data[0];
            foreach (int num in data)
                if (num < min) min = num;
            return min;
        }
        public static int Max(List<int> data)
        {
            int max;
            max = data[0];
            foreach (int num in data)
                if (num > max) max = num;
            return max;
        }

        // float
        public static float Min(params float[] data)
        {
            float min;
            min = data[0];
            foreach (float num in data)
                if (num < min) min = num;
            return min;
        }
        public static float Max(params float[] data)
        {
            float max;
            max = data[0];
            foreach (float num in data)
                if (num > max) max = num;
            return max;
        }

        // double
        public static double Min(params double[] data)
        {
            double min;
            min = data[0];
            foreach (double num in data)
                if (num < min) min = num;
            return min;
        }
        public static double Max(params double[] data)
        {
            double max;
            max = data[0];
            foreach (double num in data)
                if (num > max) max = num;
            return max;
        }
        public static double Min(List<double> data)
        {
            double min;
            min = data[0];
            foreach (double num in data)
                if (num < min) min = num;
            return min;
        }
        public static double Max(List<double> data)
        {
            double max;
            max = data[0];
            foreach (double num in data)
                if (num > max) max = num;
            return max;
        }
               #endregion
        //----------------------------------------------------------------------------------------------------------------------------
        // Equality of real numbers / Rovnost realnych cisel (float a double)
        //----------------------------------------------------------------------------------------------------------------------------
        #region Equal
        //overloaded methods for equation of real numbers (method d_equal)
        public static bool d_equal(double a, double b, double limit) 
        {
            if (limit<0) limit = Math.Abs(limit);
            if ((a - limit) < b && (a + limit) > b) return true;
            else return false;
        }
        public static bool d_equal(double a, float b, double limit)
        {
            if (limit < 0) limit = Math.Abs(limit);
            if ((a - limit) < b && (a + limit) > b) return true;
            else return false;
        }
        public static bool d_equal(float a, double b, double limit)
        {
            if (limit < 0) limit = Math.Abs(limit);
            if ((a - limit) < b && (a + limit) > b) return true;
            else return false;
        }
        public static bool d_equal(float a, float b, double limit)
        {
            if (limit < 0) limit = Math.Abs(limit);
            if ((a - limit) < b && (a + limit) > b) return true;
            else return false;
        }
        public static bool d_equal(double a, int b, double limit)
        {
            if (limit < 0) limit = Math.Abs(limit);
            if ((a - limit) < b && (a + limit) > b) return true;
            else return false;
        }
        public static bool d_equal(float a, int b, double limit)
        {
            if (limit < 0) limit = Math.Abs(limit);
            if ((a - limit) < b && (a + limit) > b) return true;
            else return false;
        }
        public static bool d_equal(double a, double b)
        {
            double limit;
            if (Math.Abs(b) > 0.0001)
                limit = 0.001 * Math.Abs(b);
            else limit = 0.00001;

            if ((a - limit) < b && (a + limit) > b) return true;
            else return false;
        }
        public static bool d_equal(double a, float b)
        {
            double limit;
            if (Math.Abs(b) > 0.0001)
                limit = 0.001 * Math.Abs(b);
            else limit = 0.00001;

            if ((a - limit) < b && (a + limit) > b) return true;
            else return false;
        }
        public static bool d_equal(float a, double b)
        {
            double limit;
            if (Math.Abs(b) > 0.0001)
                limit = 0.001 * Math.Abs(b);
            else limit = 0.00001;

            if ((a - limit) < b && (a + limit) > b) return true;
            else return false;
        }
        public static bool d_equal(float a, float b)
        {
            double limit;
            if (Math.Abs(b) > 0.0001)
                limit = 0.001 * Math.Abs(b);
            else limit = 0.00001;

            if ((a - limit) < b && (a + limit) > b) return true;
            else return false;
        }
        public static bool d_equal(double a, int b)
        {
            double limit;
            if (Math.Abs(b) > 0.0001)
                limit = 0.001 * Math.Abs(b);
            else limit = 0.00001;

            if ((a - limit) < b && (a + limit) > b) return true;
            else return false;
        }
        public static bool d_equal(float a, int b)
        {
            double limit;
            if (Math.Abs(b) > 0.0001)
                limit = 0.001 * Math.Abs(b);
            else limit = 0.00001;

            if ((a - limit) < b && (a + limit) > b) return true;
            else return false;
        }
        #endregion
        //----------------------------------------------------------------------------------------------------------------------------
        // Power / Mocniny
        //----------------------------------------------------------------------------------------------------------------------------
        #region Power
        public static float Pow2(float fx) { return fx * fx; }
        public static float Pow3(float fx) { return fx * fx * fx; }
        public static float Pow4(float fx) { float ftemp = fx * fx; return ftemp * ftemp; }
        public static float Pow5(float fx) { float ftemp = fx * fx; return ftemp * ftemp * fx; }
        public static float Pow6(float fx) { float ftemp = fx * fx * fx; return ftemp * ftemp; }
        public static float Pow7(float fx) { float ftemp = fx * fx * fx; return ftemp * ftemp * fx; }
        public static float Pow8(float fx) { float ftemp = fx * fx * fx * fx; return ftemp * ftemp; }
        public static float Pow9(float fx) { float ftemp = fx * fx * fx * fx; return ftemp * ftemp * fx; }

        public static double Pow2(double x) { return x * x; }
        public static double Pow3(double x) { return x * x * x; }
        public static double Pow4(double x) { double temp = x * x; return temp * temp; }
        public static double Pow5(double x) { double temp = x * x; return temp * temp * x; }
        public static double Pow6(double x) { double temp = x * x * x; return temp * temp; }
        public static double Pow7(double x) { double temp = x * x * x; return temp * temp * x; }
        public static double Pow8(double x) { double temp = x * x * x * x; return temp * temp; }
        public static double Pow9(double x) { double temp = x * x * x * x; return temp * temp * x; }

        // Prirodzeny mocnitel / kladny int
        public static float PowN(float fx, short iexp)
        {
            float pow = fx;
            if (iexp > 1)
            {
                for (int i = 0; i < iexp; i++)
                    pow *= fx;
                return pow;
            }
            else if (iexp == 1)
                return fx;
            else
                return 1f;
        }

        public static double PowN(double x, short iexp)
        {
            double pow = x;
            if (iexp > 1)
            {
                for (int i = 0; i < iexp; i++)
                    pow *= x;
                return pow;
            }
            else if (iexp == 1)
                return x;
            else
                return 1;
        }

        public static float Pow_1_3(float fx) { return (float)Math.Pow(fx, 1 / 3f); }
        public static float Pow_1_4(float fx) { return (float)Math.Pow(fx, 0.25f); }
        public static float Pow_1_5(float fx) { return (float)Math.Pow(fx, 1 / 5f); }
        public static float Pow_1_6(float fx) { return (float)Math.Pow(fx, 1 / 6f); }

        public static float Pow_2_3(float fx) { return (float)Math.Pow(fx, 2 / 3f); }
        public static float Pow_2_5(float fx) { return (float)Math.Pow(fx, 2 / 5f); }
        public static float Pow_3_2(float fx) { return (float)Math.Pow(fx, 1.5f); }
        public static float Pow_5_2(float fx) { return (float)Math.Pow(fx, 5 / 2f); }

        public static double Pow_1_3(double x) { return (double)Math.Pow(x, 1 / 3f); }
        public static double Pow_1_4(double x) { return (double)Math.Pow(x, 0.25f); }
        public static double Pow_1_5(double x) { return (double)Math.Pow(x, 1 / 5f); }
        public static double Pow_1_6(double x) { return (double)Math.Pow(x, 1 / 6f); }

        public static double Pow_2_3(double x) { return (double)Math.Pow(x, 2 / 3f); }
        public static double Pow_2_5(double x) { return (double)Math.Pow(x, 2 / 5f); }
        public static double Pow_3_2(double x) { return (double)Math.Pow(x, 1.5f); }
        public static double Pow_5_2(double x) { return (double)Math.Pow(x, 5 / 2f); }

        #endregion
        //----------------------------------------------------------------------------------------------------------------------------
        // Root / Odmocnina / Square Root / Druha odmocnina
        //----------------------------------------------------------------------------------------------------------------------------
        #region Root / Square Root
        /*
        public static short isqrt(short num)
        {
            short op = num;
            short res = 0;
            short one = 1 << 14; // The second-to-top bit is set: 1L<<30 for long

            // "one" starts at the highest power of four <= the argument.
            while (one > op)
                one >>= 2;

            while (one != 0)
            {
                if (op >= res + one)
                {
                    op -= res + one;
                    res = (res >> 1) + one;
                }
                else
                    res >>= 1;
                one >>= 2;
            }
            return res;
        }
        */


        /*
        public static float fastsqrt(float z)  
        {

        union
        {
                int tmp;
                float f;
        } u;
        u.f = z;
        u.tmp -= 1<<23; // Remove last bit so 1.0 gives 1.0
        // tmp is now an approximation to logbase2(z) 
        u.tmp >>= 1; // divide by 2
        u.tmp += 1<<29; // add 64 to exponent: (e+127)/2 =(e/2)+63,
        // that represents (e/2)-64 but want e/2
        return u.f;
        }
        */

        /*
        // Reciprocal of the square root
        public static float invSqrt(float x)
        {
        float xhalf = 0.5f*x;
        union
        {
                float x;
                int i;
        } u;
        u.x = x;
        u.i = 0x5f3759df - (u.i >> 1);
        x = u.x * (1.5f - xhalf * u.x * u.x);
        return x;
         }
        */



        /*
        public unsigned short sqrt(unsigned long a){
unsigned long rem = 0;
unsigned long root = 0;
for (int i = 0; i < 16; i++){
	root <<= 1;
	rem = ((rem << 2) + (a >> 30);
	a <<= 2;
	root ++;
	if(root <= rem){
		rem -= root;
		root ++;
	}
	else
		root --;
}
return (unsigned short)(root >> 1);
}
*/


        public static double Sqrt(double value)
{
     //assert(value >= 1);

    if (value > 0)
    {
        double lo = 1.0;
        double hi = value;

        while (hi - lo > 0.00001)
        {
            double mid = lo + (hi - lo) / 2;
            //std::cout << lo << "," << hi << "," << mid << std::endl;
            if (mid * mid - value > 0.00001)    //this is the predictors we are using 
            {
                hi = mid;
            }
            else
            {
                lo = mid;
            }

        }

        return lo;
    }
    else
        return 1;
 }

        /*
 public static float Sqrt(float m)
 {
     float i = 0;
     float x1, x2;
     while ((i * i) <= m)
         i += 0.1;
     x1 = i;
     for (int j = 0; j < 10; j++)
     {
         x2 = m;
         x2 /= x1;
         x2 += x1;
         x2 /= 2;
         x1 = x2;
     }
     return x2;
 }
*/

        public static float Sqrt(float num)
        {
            if (num >= 0)
            {
                float x = num;
                int i;
                for (i = 0; i < 20; i++)
                {
                    x = (((x * x) + num) / (2 * x));
                }
                return x;
            }
            else
                return 1f; // Exception
        }


        /*
        public static float Sqrt(float value)
        {
            if (value > 0f)
            {
                float lo = 1.0f;
                float hi = value;

                while (hi - lo > 0.00001f)
                {
                    float mid = lo + (hi - lo) / 2;
                    if (mid * mid - value > 0.00001f)    //this is the predictors we are using 
                    {
                        hi = mid;
                    }
                    else
                    {
                        lo = mid;
                    }

                }

                return lo;
            }
            else
                return 1f;
        }

        */
        #endregion
        //----------------------------------------------------------------------------------------------------------------------------
        // Constants / Konstatny 
        //----------------------------------------------------------------------------------------------------------------------------
        #region Constants
        // Napier's constant, or Euler's number, base of Natural logarithm
        // Represents the natural logarithmic base, specified by the constant, e.
        public const float   fE = 2.7182818f;                                  // 7
        public const double  dE = 2.718281828459045;                           // 15-16
        public const decimal mE = 2.718281828459045235360287471352662497m;     // 28-29
        // Pi, Archimedes' constant or Ludolph's number
        // Represents the ratio of the circumference of a circle to its diameter, specified by the constant, π.
        public const float   fPI = 3.1415926f;                                 // 7
        public const double  dPI = 3.14159265358979;                           // 15-16
        public const decimal mPI = 3.14159265358979323846264338327950288m;     // 28-29
        // Feigenbaum constant / Feigenbaumova konstanta delta δ
        public const decimal mFeigenDelta = 4.66920160910299067185320382m;
        // Feigenbaum constant / Feigenbaumova konstanta alfa α
        public const decimal mFeigenAlpha = 2.502907875095892822283902873218m;
        // Kaprekarova konstanta
        public const int iKaprekar = 6174;
        // Apéry's constant ζ(3)
        public const decimal Zeta3 = 1.202056903159594285399738161511449990764986292m;
        //Pythagoras' constant, square root of 2 (√2)
        public const float    fSqrt2 = 1.4142135f;
        public const double   dSqrt2 = 1.414213562373095;
        public const decimal  mSqrt2 = 1.41421356237309504880168872420969807m;
        // Theodorus' constant, square root of 3 (√3)
        public const float    fSqrt3 = 1.7320508f;
        public const double   dSqrt3 = 1.732050807568877;
        public const decimal  mSqrt3 = 1.73205080756887729352744634150587236m;
        // Square root of 5 (√5}
        public const float    fSqrt5 = 2.236067f;
        public const double   dSqrt5 = 2.236067977499789;
        public const decimal  mSqrt5 = 2.23606797749978969640917366873127623m;	
        #endregion

        //----------------------------------------------------------------------------------------------------------------------------
        // Logarithm / Logaritmus
        //----------------------------------------------------------------------------------------------------------------------------
        #region Logarithm
        public static float Ln(float fx)
        {
            return (float)Math.Log(fx, (float)Math.E);
        }
        public static float Log10(float fx)
        {
            return (float)Math.Log10(fx);
        }
        public static float Log(float fx, float fbase)
        {
            return (float)Math.Log(fx, fbase);
        }
        #endregion

        //----------------------------------------------------------------------------------------------------------------------------
        // Integral
        //----------------------------------------------------------------------------------------------------------------------------


        /*
public double RombergIntegration(Function y, double a, double b, int n) 
{ 
  int i, j, m = 0; 
  double h, trap; 
  double[,] I = new double[n, n]; 
  h = b - a; 
  I[0, 0] = (y(a) + y(b)) * (h / 2); 
  if (n > 15) n = 15; 
  do 
  { 
  m++; 
  h = h / 2; 
  trap = (y(a) + y(b)) / 2; 
  for (i = 1; i < Math.Pow(2, m); i++) trap += y(a + h * i); 
  I[0, m] = trap * h; 
  for (i = 1; i < m + 1; i++) 
  { 
  j = m - i; 
  I[i, j] = (Math.Pow(4, i) * I[i - 1, j + 1] - I[i - 1, j]) / (Math.Pow(4, i) - 1); 
  } 
  } while (m < n); 
  return I[n, 0]; 
} 

*/


    }
}

