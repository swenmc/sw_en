using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MATH.ARRAY;

namespace AAC
{
    class AAC_Database_Data
    {
        private float[] Reinforcement_Strength_array = new float[] { 250.0f, 500.0f, 500.0f }; // MPa
        private float[] Reinforcement_d_array = new float[] { 5.0f, 5.5f, 6.0f, 8.0f, 10.0f, 12.0f, 14.0f, 16.0f, 18.0f, 20.0f }; // mm

        // Autoclaved Aered Concrete
        // 1000 md, eps c, eps s, kx, kz, 1000 omega S235, 1000 omega S500 - 48 rows
        private double[,] AAC_prop_array  = new double [,]
          {
                    {   1.512 , 0.25 , 10.00  , 0.024 ,  0.992 ,  1.524 ,   1.524 },
                    {   5.858 , 0.50 , 10.00  , 0.048 ,  0.984 ,  5.952 ,   5.952 },
                    {   12.78 , 0.75 , 10.00  , 0.070 ,  0.977 ,  13.08 ,   13.08 },
                    {   22.04 , 1.00 , 10.00  , 0.091 ,  0.970 ,  22.73 ,   22.73 },
                    {   33.44 , 1.25 , 10.00  , 0.111 ,  0.963 ,  34.72 ,   34.72 },
                    {   46.79 , 1.50 , 10.00  , 0.130 ,  0.957 ,  48.91 ,   48.91 },
                    {   61.92 , 1.75 , 10.00  , 0.149 ,  0.950 ,  65.16 ,   65.16 },
                    {   78.70 , 2.00 , 10.00  , 0.167 ,  0.944 ,  83.33 ,   83.33 },
                    {   95.72 , 2.25 , 10.00  , 0.184 ,  0.938 , 102.0  ,  102.0  },
                    {  111.7  , 2.50 , 10.00  , 0.200 ,  0.931 , 120.0  ,  120.0  },
                    {  126.8  , 2.75 , 10.00  , 0.216 ,  0.924 , 137.3  ,  137.3  },
                    {  141.0  , 3.00 , 10.00  , 0.231 ,  0.917 , 153.8  ,  153.8  },
                    {  143.5  , 3.00 ,  9.75  , 0.235 ,  0.915 , 156.9  ,  156.9  },
                    {  146.1  , 3.00 ,  9.50  , 0.240 ,  0.913 , 160.0  ,  160.0  },
                    {  148.8  , 3.00 ,  9.25  , 0.245 ,  0.912 , 163.3  ,  163.3  },
                    {  151.6  , 3.00 ,  9.00  , 0.250 ,  0.910 , 166.7  ,  166.7  },
                    {  154.5  , 3.00 ,  8.75  , 0.255 ,  0.908 , 170.2  ,  170.2  },
                    {  157.5  , 3.00 ,  8.50  , 0.261 ,  0.906 , 173.9  ,  173.9  },
                    {  160.7  , 3.00 ,  8.25  , 0.267 ,  0.904 , 177.8  ,  177.8  },
                    {  163.9  , 3.00 ,  8.00  , 0.273 ,  0.902 , 181.8  ,  181.8  },
                    {  167.3  , 3.00 ,  7.75  , 0.279 ,  0.899 , 186.0  ,  186.0  },
                    {  170.8  , 3.00 ,  7.50  , 0.286 ,  0.897 , 190.5  ,  190.5  },
                    {  174.5  , 3.00 ,  7.25  , 0.293 ,  0.894 , 195.1  ,  195.1  },
                    {  178.3  , 3.00 ,  7.00  , 0.300 ,  0.892 , 200.0  ,  200.0  },
                    {  182.3  , 3.00 ,  6.75  , 0.308 ,  0.889 , 205.1  ,  205.1  },
                    {  186.5  , 3.00 ,  6.50  , 0.316 ,  0.886 , 210.5  ,  210.5  },
                    {  190.9  , 3.00 ,  6.25  , 0.324 ,  0.883 , 216.2  ,  216.2  },
                    {  195.5  , 3.00 ,  6.00  , 0.333 ,  0.880 , 222.2  ,  222.2  },
                    {  200.3  , 3.00 ,  5.75  , 0.343 ,  0.876 , 228.6  ,  228.6  },
                    {  205.3  , 3.00 ,  5.50  , 0.353 ,  0.873 , 235.3  ,  235.3  },
                    {  210.6  , 3.00 ,  5.25  , 0.364 ,  0.869 , 242.4  ,  242.4  },
                    {  216.1  , 3.00 ,  5.00  , 0.375 ,  0.865 , 250.0  ,  250.0  },
                    {  222.0  , 3.00 ,  4.75  , 0.387 ,  0.860 , 258.1  ,  258.1  },
                    {  228.1  , 3.00 ,  4.50  , 0.400 ,  0.856 , 266.7  ,  266.7  },
                    {  234.6  , 3.00 ,  4.25  , 0.414 ,  0.851 , 275.9  ,  275.9  },
                    {  241.5  , 3.00 ,  4.00  , 0.429 ,  0.845 , 285.7  ,  285.7  },
                    {  248.7  , 3.00 ,  3.75  , 0.444 ,  0.840 , 296.3  ,  296.3  },
                    {  256.4  , 3.00 ,  3.50  , 0.462 ,  0.833 , 307.7  ,  307.7  },
                    {  264.5  , 3.00 ,  3.25  , 0.480 ,  0.827 , 320.0  ,  320.0  },
                    {  273.1  , 3.00 ,  3.00  , 0.500 ,  0.819 , 333.3  ,  333.3  },
                    {  282.3  , 3.00 ,  2.75  , 0.522 ,  0.812 , 347.8  ,  347.8  },
                    {  292.0  , 3.00 ,  2.50  , 0.545 ,  0.803 , 363.6  ,  363.6  },
                    {  302.3  , 3.00 ,  2.25  , 0.571 ,  0.794 , 381.0  ,  381.0  },
                    {  313.3  , 3.00 ,  2.00  , 0.600 ,  0.783 , 400.0  ,  434.8  },
                    {  325.0  , 3.00 ,  1.75  , 0.632 ,  0.772 , 421.1  ,  523.0  },
                    {  337.4  , 3.00 ,  1.50  , 0.667 ,  0.759 , 444.4  ,  644.1  },
                    {  350.6  , 3.00 ,  1.25  , 0.706 ,  0.745 , 470.6  ,  818.4  },
                    {  364.6  , 3.00 ,  1.00  , 0.750 ,  0.729 , 510.9  , 1087.0  }
            };

        public double[] AAC_value_array_for_1000md = new double[6];

        // Constructor
        public AAC_Database_Data()
        {
        }

        public void Get_Database_Data(int iRein_StrengthClass,
                                 int iRein_d_long_upper,
                                 int iRein_d_long_lower,
                                 int iRein_d_trans,
                                 out float fyk,
                                 out float d_long_upper,
                                 out float d_long_lower,
                                 out float d_trans)
        {
            fyk = Reinforcement_Strength_array[iRein_StrengthClass] * 1.0e+6f; // Pa
            d_long_upper = Reinforcement_d_array[iRein_d_long_upper] / 1000.0f; // m
            d_long_lower = Reinforcement_d_array[iRein_d_long_lower] / 1000.0f; // m
            d_trans = Reinforcement_d_array[iRein_d_trans] / 1000.0f; // m
        }

        public AAC_Database_Data(double value_1000md)
        {
            // Eps_c;
            // Eps_s;
            // kx;
            // kz;
            // value_1000omegaS235;
            // value_1000omegaS500;

            GetAAC_values_for_1000md(value_1000md);
        }

        // Get AAC values for 1000 md
        public void GetAAC_values_for_1000md(double value_1000md)
        {
            for (int i = 0; i < 6; i++)
            {
                AAC_value_array_for_1000md[i] = ArrayF.GetLinearInterpolationValuePositive(value_1000md, Get_1D_Array(0), Get_1D_Array(i + 1));
            }
        }

        // Get Array of one value
        public double [] Get_1D_Array(int column_index)
        {
            int n = 48;

            double[] array = new double[n];

            for (int i = 0; i < n; i++)
            {
                array[i] = AAC_prop_array[i, column_index];
            }

            return array;
        }
    }
}
