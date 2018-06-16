using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MATH;

namespace CENEX
{
    public partial class EN1993_1_8Form_PIN : Form
    {
        double d_Sigma_h_Ed;
        double d_E;
        double d_F_Ed;
        double d_F_Ed_ser;
        // the diameter of the pin;
        double d_d;         // outside diameter
        double d_d_in;      // inside diameter
        double d_d_0;
        
        // the lower of the design strengths of the pin and the connected part;
        double d_f_y;
        double d_f_u;
        double d_f_h_Ed;



        double d_F_v_Rd;
        double d_F_b_Rd;
        double d_F_b_Rd_ser;
        double d_M_Rd;
        double d_M_Rd_ser;

        int mat1_id;
        int mat2_id;
        // the yield strength of the pin;
        double d_f_yp;
        // the ultimate tensile strength of the pin
        double d_f_up;
        double d_gamma_M0;
        double d_gamma_M1;
        double d_gamma_M2;

        double d_gamma_M6_ser;
        // the cross-sectional area of a pin.
        double d_A;
        // Elastic modulus
        double d_W_el;
        // Moment of inertia (second moment of area)
        double d_Iy;
        // the thickness of the connected part;
        double d_t_11;
        double d_t_12;
        double d_t_21;
        double d_t_22;
        double d_t_23;


        

        double d_F_v_Ed;
        double d_F_b_Ed;
        double d_F_b_Ed_ser;
        double d_M_Ed;
        double d_M_Ed_ser;

        double d_t_c;

        double d_t_min;
        double d_t_max;
        
        double d_t_1_min;
        double d_t_1_max;

        double d_t_2_min;
        double d_t_2_max;

        double d_t_1;
        double d_t_2;
        
        bool b_index_REPLACE;
        bool b_index_SOLID;
        bool b_index_PLATES32;
        bool b_index_PLATES21;



        // Schemes


        // Scheme 1
        double d_a_p1;
        double d_c_p1;

        // Scheme 2
        double d_t_p2;
        double d_d0_p2;

        double d_03d0_p2;
        double d_075d0_p2;
        double d_1d0_p2;
        double d_13d0_p2;
        double d_16d0_p2;
        double d_25d0_p2;


        // Check ratios
        double d_ratio_1;
        double d_ratio_2;
        double d_ratio_3;
        double d_ratio_4;
        double d_ratio_5;
        double d_ratio_6;
        double d_ratio_7;

        double d_ratio_max;

        // kN to N
        // KILO
        int i_ratio_kilo = 1000;
        // MPa to Pa
        // MEGA
        double d_ratio_mega = 0.000001;
        // mm to m
        double d_ratio_mili = 0.001;
        // Percent
        double d_ratio_percent = 0.01;


        public EN1993_1_8Form_PIN()
        {
            InitializeComponent();
            // Load steel grades into comboboxes
            for (int i = 0; i < 15; i++)
            {
                this.comboBox_Steel_PLATE.Items.Add(steel_grades[i]);            
            }
            for (int i = 0; i < steel_grades.Length; i++)
            {
                this.comboBox_Steel_PIN.Items.Add(steel_grades[i]); 
            }





            // Set default values in dialog
           this.Set_data_default();
        }




 // Array of Steel properties


string [] steel_grades  = {
"S 235",       
"S 275",
"S 355",
"S 450",
"S 275 N/NL",
"S 355 N/NL",
"S 420 N/NL",
"S 460 N/NL",
"S 275 M/ML",
"S 355 M/ML",
"S 420 M/ML",
"S 460 M/ML",
"S 235 W",
"S 355 W",
"S 460 Q/QL/QL1",
//16
"Steel for pins",
//17
"C35E        12040.6  1.1181",
"C35           12040.9  1.0501",
//19
"C55E        12060.1  1.1203",
"C55           12060.6  1.0535",
"C56E2       12060.9  1.1219",
//22
"C60E        12061.1  1.1221",
"C60           12061.6  1.0601",
"C60           12061.9  1.0601",
//25
"16MnCr5    14220.4  1.7131 I.31",
"36Mn7       14240.3  1.5069",
"36Mn7       14240.6  1.5069",
"375iCR     14341.7 - Trinec",
//29
"51CrV4 (50CrV4)    15260.6 1.8159 I.59",
"S690QL (StE690V) 16224   1.8928 I.28",
"30CrNiMo8v           16435   1.658      ",
"34CrNiMo6            16343   1.6582 I.82"
    };

double[,] steel_properties = {
{235,	360,	215,	360,	1.00,	1.00,	1.25,	0.80,	1.2,	210000,	80769,	0.3,	1.20E-05},
{275,	430,	255,	410,	1.00,	1.00,	1.25,	0.85,	1.2,	210000,	80769,	0.3,	1.20E-05},
{355,	510,	335,	470,	1.00,	1.00,	1.25,	0.90,	1.2,	210000,	80769,	0.3,	1.20E-05},
{440,	550,	410,	550,	1.00,	1.00,	1.25,	1.00,	1.2,	210000,	80769,	0.3,	1.20E-05},
{275,	390,	255,	370,	1.00,	1.00,	1.25,	0.85,	1.2,	210000,	80769,	0.3,	1.20E-05},
{355,	490,	335,	470,	1.00,	1.00,	1.25,	0.90,	1.2,	210000,	80769,	0.3,	1.20E-05},
{420,	520,    390,    520,	1.00,	1.00,	1.25,	1.00,	1.2,	210000,	80769,	0.3,    1.20E-05},
{460,	540,    430,    540,	1.00,	1.00,	1.25,	1.00,	1.2,	210000,	80769,	0.3,	1.20E-05},
{375,	370,	255,    360,	1.00,	1.00,	1.25,	0.85,	1.2,	210000,	80769,	0.3,	1.20E-05},
{355,	470,	335,	450,	1.00,	1.00,	1.25,	0.90,	1.2,	210000,	80769,	0.3,	1.20E-05},
{420,	520,	390,	500,	1.00,	1.00,	1.25,	1.00,	1.2,	210000,	80769,	0.3,	1.20E-05},
{460,	540,	430,	530,	1.00,	1.00,	1.25,	1.00,	1.2,	210000,	80769,	0.3,	1.20E-05},
{235,	360,	215,	340,	1.00,	1.00,	1.25,	0.80,	1.2,	210000,	80769,	0.3,	1.20E-05},
{355,	510,	335,	490,	1.00,	1.00,	1.25,	0.90,	1.2,	210000,	80769,	0.3,	1.20E-05},
{460,	570,	440,	550,	1.00,	1.00,	1.25,	1.00,	1.2,	210000,	80769,	0.3,	1.20E-05},
//16
{   1,     1,    1,      1,     1,      1,        1,    1,      1,      100000, 79999,    1,          1},
// High strength steel  - pin
//17
{295,	510,	295,	510,	1.00,	1.00,	1.25,	1.00,	1.2,	210000,	80769,	0.3,	1.20E-05},
{255,	490,	255,	490,	1.00,	1.00,	1.25,	1.00,	1.2,	210000,	80769,	0.3,	1.20E-05},
//19
{345,	600,	345,	600,	1.00,	1.00,	1.25,	1.00,	1.2,	210000,	80769,	0.3,	1.20E-05},
{365,	660,	365,	660,	1.00,	1.00,	1.25,	1.00,	1.2,	210000,	80769,	0.3,	1.20E-05},
{345,	640,	345,	640,	1.00,	1.00,	1.25,	1.00,	1.2,	210000,	80769,	0.3,	1.20E-05},
//22
{380,	660,	380,	660,	1.00,	1.00,	1.25,	1.00,	1.2,	210000,	80769,	0.3,	1.20E-05},
{355,	670,	355,	670,	1.00,	1.00,	1.25,	1.00,	1.2,	210000,	80769,	0.3,	1.20E-05},
{335,	660,	335,	660,	1.00,	1.00,	1.25,	1.00,	1.2,	210000,	80769,	0.3,	1.20E-05},
// 14XXX.X
//25
{590,	785,	590,	785,	1.00,	1.00,	1.25,	1.00,	1.2,	210000,	80769,	0.3,	1.20E-05},
{470,	740,	470,	740,	1.00,	1.00,	1.25,	1.00,	1.2,	210000,	80769,	0.3,	1.20E-05},
{470,	690,	470,	690,	1.00,	1.00,	1.25,	1.00,	1.2,	210000,	80769,	0.3,	1.20E-05},
{715,	930,	715,	930,	1.00,	1.00,	1.25,	1.00,	1.2,	210000,	80769,	0.3,	1.20E-05},
// 29
{590,	785,	590,	785,	1.00,	1.00,	1.25,	1.00,	1.2,	210000,	80769,	0.3,	1.20E-05},
{500,	740,	500,	740,	1.00,	1.00,	1.25,	1.00,	1.2,	210000,	80769,	0.3,	1.20E-05},
{900,	1000,	900,	1000,	1.00,	1.00,	1.25,	1.00,	1.2,	205000,	80769,	0.3,	1.20E-05},
{590,	735,	590,	735,	1.00,	1.00,	1.25,	1.00,	1.2,	210000,	80769,	0.3,	1.20E-05}                          
};

public void Set_data_default ()
{
    b_index_REPLACE = false;
    b_index_SOLID = true;
    b_index_PLATES32 = true;
    b_index_PLATES21 = false;

    d_d_textB.Text = Convert.ToString(100);
    d_d0_textB.Text = Convert.ToString(101);
    d_din_textB.Text = Convert.ToString(30);

    d_t11_textB.Text = Convert.ToString(50);
    d_t12_textB.Text = Convert.ToString(50);

    d_t21_textB.Text = Convert.ToString(25);
    d_t22_textB.Text = Convert.ToString(50);
    d_t23_textB.Text = Convert.ToString(25);

    d_tc_textB.Text = Convert.ToString(3);

    d_FEd_textB.Text = Convert.ToString(2500);
    d_FEd_ser_textB.Text = Convert.ToString(1700);

    comboBox_Steel_PLATE.Text = "S 355";
    comboBox_Steel_PLATE.SelectedIndex = 2; // !!! Row number !!!

    comboBox_Steel_PIN.Text = "30CrNiMo8v           16435   1.658      ";
    comboBox_Steel_PIN.SelectedIndex = 30; // !!! Row number !!!

}
// Metoda načítava údaje o oceli z poľa
public void Load_data_Steel()
{
    // Material Plates
    mat1_id = comboBox_Steel_PLATE.SelectedIndex;
    d_f_y = steel_properties[mat1_id, 0];
    d_f_u = steel_properties[mat1_id, 1];

    d_gamma_M0 = steel_properties[mat1_id, 4];
    d_gamma_M1 = steel_properties[mat1_id, 5];
    d_gamma_M2 = steel_properties[mat1_id, 6];
    d_gamma_M6_ser = 1.0; // !!! constant

    // Material Pin
    mat2_id = comboBox_Steel_PIN.SelectedIndex;
    d_f_yp = steel_properties[mat2_id, 0];
    d_f_up = steel_properties[mat2_id, 1];
    // Minimum yield strength  ( pin and plates)
    d_f_y = Math.Min(d_f_y, d_f_yp);
    // Young modulus
    d_E = steel_properties[mat2_id, 9]; // for pin ???
}
// Metoda - Set Steel data in textboxes after selection in comboboxes

public void Set_Steel_Data_Text()
{

    d_dfy_textB.Text = d_f_y.ToString();
    d_dfu_textB.Text = d_f_u.ToString();

    d_dfyp_textB.Text = d_f_yp.ToString();
    d_dfup_textB.Text = d_f_up.ToString();
}




// Metoda - Load data from textboxes
// Tato metoda nacita udaje z textboxov a skonvertuje na cislo
public void Load_data()
{

    if (b_index_SOLID == true)
    {
        d_din_textB.Text = "0";
        d_din_textB.ReadOnly = true;
    }
    if (b_index_SOLID == false)
        d_din_textB.ReadOnly = false;
    

// Load data from textboxes
    try
    {
        d_d = Convert.ToDouble(d_d_textB.Text.ToString());
    }
    catch
    {
        MessageBox.Show("FORMAT ERROR", "Wrong numerical format! Enter real number, please.");
    }

    try
    {
        d_d_0 = Convert.ToDouble(d_d0_textB.Text.ToString());
    }
    catch
    {
        MessageBox.Show("FORMAT ERROR", "Wrong numerical format! Enter real number, please.");
    }
    try
    {
        d_d_in = Convert.ToDouble(d_din_textB.Text.ToString());
    }
    catch
    {
        MessageBox.Show("FORMAT ERROR", "Wrong numerical format! Enter real number, please.");
    }

    try
    {
        d_t_11 = Convert.ToDouble(d_t11_textB.Text.ToString());
    }
    catch
    {
        MessageBox.Show("FORMAT ERROR", "Wrong numerical format! Enter real number, please.");
    }
    try
    {
        d_t_12 = Convert.ToDouble(d_t12_textB.Text.ToString());
    }
    catch
    {
        MessageBox.Show("FORMAT ERROR", "Wrong numerical format! Enter real number, please.");
    }
    try
    {
        d_t_21 = Convert.ToDouble(d_t21_textB.Text.ToString());
    }
    catch
    {
        MessageBox.Show("FORMAT ERROR", "Wrong numerical format! Enter real number, please.");
    }
    try
    {
        d_t_22 = Convert.ToDouble(d_t22_textB.Text.ToString());
    }
    catch
    {
        MessageBox.Show("FORMAT ERROR", "Wrong numerical format! Enter real number, please.");
    }
    try
    {
        d_t_23 = Convert.ToDouble(d_t23_textB.Text.ToString());
    }
    catch
    {
        MessageBox.Show("FORMAT ERROR", "Wrong numerical format! Enter real number, please.");
    }
    try
    {
        d_t_c = Convert.ToDouble(d_tc_textB.Text.ToString());
    }
    catch
    {
        MessageBox.Show("FORMAT ERROR", "Wrong numerical format! Enter real number, please.");
    }
    try
    {
        d_F_Ed = Convert.ToDouble(d_FEd_textB.Text.ToString());
    }
    catch
    {
        MessageBox.Show("FORMAT ERROR", "Wrong numerical format! Enter real number, please.");
    }
    try
    {
        d_F_Ed_ser = Convert.ToDouble(d_FEd_ser_textB.Text.ToString());
    }
    catch
    {
        MessageBox.Show("FORMAT ERROR", "Wrong numerical format! Enter real number, please.");
    }
}

// Metoda meni jednotky načítaných dát na SI sústavu
public void Convert_data_units()
{
    // Dimensions

    d_d *= d_ratio_mili;
    d_d_0 *= d_ratio_mili;
    d_d_in *= d_ratio_mili;
    d_t_11 *= d_ratio_mili;
    d_t_12 *= d_ratio_mili;
    d_t_21 *= d_ratio_mili;
    d_t_22 *= d_ratio_mili;
    d_t_23 *= d_ratio_mili;
    d_t_c *= d_ratio_mili;

    // Calculation
 if (b_index_PLATES32 == true) // 3+ 2 plates
{
    d_t_min = MathF.Min(d_t_11, d_t_12, d_t_21, d_t_22, d_t_23);
    d_t_max = MathF.Max(d_t_11, d_t_12, d_t_21, d_t_22, d_t_23);

    d_t_1_min = MathF.Min(d_t_11, d_t_12);
    d_t_1_max = MathF.Max(d_t_11, d_t_12);

    d_t_2_min = MathF.Min(d_t_21, d_t_22, d_t_23);
    d_t_2_max = MathF.Min(d_t_21, d_t_22, d_t_23);

    d_t_1 = d_t_11 + d_t_12;
    d_t_2 = d_t_21 + d_t_22 + d_t_23;
}
else // 2+1 plates
 {
     d_t_min = MathF.Min(d_t_11, d_t_21, d_t_22);
     d_t_max = MathF.Max(d_t_11, d_t_21, d_t_22);

     d_t_1_min = MathF.Min(d_t_11);
     d_t_1_max = MathF.Max(d_t_11);

     d_t_2_min = MathF.Min(d_t_21, d_t_22);
     d_t_2_max = MathF.Min(d_t_21, d_t_22);

     d_t_1 = d_t_11;
     d_t_2 = d_t_21 + d_t_22;
 }


    // Solved properties
    // Pin area
    d_A = Math.PI * (Math.Pow(d_d / 2, 2) - Math.Pow(d_d_in / 2, 2));
    // Pin Elastic Modulus
    d_W_el = (Math.PI * (Math.Pow(d_d, 3) / 32)) - (Math.PI * (Math.Pow(d_d_in, 3) / 32));
    d_Iy = (Math.PI * (Math.Pow(d_d, 4) / 64)) - (Math.PI * (Math.Pow(d_d_in, 4) / 64));

    // Conversion
    // Loaded data unit conversion

    // Design Force
    d_F_Ed *= i_ratio_kilo;
    d_F_Ed_ser *= i_ratio_kilo;

    // Steel strength - conversion
    d_f_y /= d_ratio_mega;
    d_f_u /= d_ratio_mega;
    d_f_yp /= d_ratio_mega;
    d_f_up /= d_ratio_mega;
    d_E /= d_ratio_mega;
}

public void Control_Message_SI_Units()
{
    MessageBox.Show((

"d = " + d_d.ToString() + " m " + " \n" +
"d0 = " + d_d_0.ToString() + " m " + " \n" +
"din = " + d_d_in.ToString() + " m " + " \n" +
"t11 = " + d_t_11.ToString() + " m " + " \n" +
"t12 = " + d_t_12.ToString() + " m " + " \n" +
"t21 = " + d_t_21.ToString() + " m " + " \n" +
"t22 = " + d_t_22.ToString() + " m " + " \n" +
"t23 = " + d_t_23.ToString() + " m " + " \n" +
"tc = " + d_t_c.ToString() + " m " + " \n" +

"tmin = " + d_t_min.ToString() + " m " + " \n" +
"tmax = " + d_t_max.ToString() + " m " + " \n" +
"t1min = " + d_t_1_min.ToString() + " m " + " \n" +
"t1max = " + d_t_1_max.ToString() + " m " + " \n" +
"t2min = " + d_t_2_min.ToString() + " m " + " \n" +
"t2max = " + d_t_2_max.ToString() + " m " + " \n" +
"t1 = " + d_t_1.ToString() + " m " + " \n" +
"t2 = " + d_t_2.ToString() + " m " + " \n" +

"A =" + Math.Round(d_A, 6).ToString() + " m2 " + " \n" +
"Wel =" + Math.Round(d_W_el, 6).ToString() + " m3 " + " \n" +
"FEd =" + d_F_Ed.ToString() + " N " + " \n" +
"FEd,ser =" + d_F_Ed_ser.ToString() + " N " + " \n" +
"fy =" + d_f_y.ToString() + " Pa " + " \n" +
"fu =" + d_f_u.ToString() + " Pa " + " \n" +
"fyp =" + d_f_yp.ToString() + " Pa " + " \n" +
"fup =" + d_f_up.ToString() + " Pa " + " \n" +
"E =" + d_E.ToString() + " Pa "
       )
       ,
        // Message Header
       "SI Units control");
}

public void Control_Geometry()
{
    if (d_d_0 <= d_d)
    {
        MessageBox.Show("Small hole diameter for pin!", " Hole diameter control");
        d_ratio_max_textB.Text = " ERROR! ";
    }
    if (d_d_in > 0.4 * d_d)
    {
        double d_d_in_max = Math.Round(0.4 * (d_d / d_ratio_mili), 1);

        MessageBox.Show(("Too large inside diameter of pin! \nSet it smaller than 40 % of pin diameter, please.\n" +
            "din max = " + d_d_in_max.ToString() + " mm")
            , " Inside diameter control");

        d_ratio_max_textB.Text = " ERROR! ";
    };
}

// Main method
public void EN1993_1_8_Main()
{
    // Shear force in one cut = Total Force / number of cuts ( 5 plates or 3 plates)
if(b_index_PLATES32 ==true)
{
    d_F_v_Ed = d_F_Ed / 4; // 3+2
}
else
{
    d_F_v_Ed = d_F_Ed / 2; // 2+1
}


    // Sum of all plates in one dirrection
    // ULS
    d_F_b_Ed = d_F_Ed;
    // Figure 3.11: Bending moment in a pin
    // ULS
    // M - krajny = FEd/2 * (a/2 + c + b/4)
    // M - stredny  = FEd/4 (a + 2c + b/2)
    if (b_index_PLATES32 == true) // 3+2
    {
        double d_M_Ed_1 = d_Calc_M_Ed(d_t_21, d_t_11, d_t_c, 0.5 * d_F_Ed);
        double d_M_Ed_2 = d_Calc_M_Ed(d_t_11, d_t_22, d_t_c, 0.5 * d_F_Ed);
        d_M_Ed = MathF.Min(d_M_Ed_1, d_M_Ed_2);
    }
    else
    {
        double d_M_Ed_1 = d_Calc_M_Ed(d_t_21, d_t_11, d_t_c, d_F_Ed); // 2+1
        double d_M_Ed_2 = (d_F_Ed / 2) * (d_t_21 / 2 + d_t_c + d_t_11 / 4);
        double d_M_Ed_3 = (d_F_Ed / 4) * (d_t_21 + 2 * d_t_c + d_t_11 / 2);
        d_M_Ed = MathF.Min(d_M_Ed_1, d_M_Ed_2, d_M_Ed_3);
    }



    if (b_index_REPLACE == true)
    {
        // (3) If the pin is intended to be replaceable, in addition to the provisions given in 3.13.1 to 3.13.2, the contact bearing stress should satisfy
        // (3.15)
        d_Sigma_h_Ed = 0.591 * Math.Sqrt((d_E * d_F_Ed_ser * (d_d_0 - d_d)) / (Math.Pow(d_d, 2) * Math.Min(d_t_1, d_t_2))); // All plates
        // (3.16)
        d_f_h_Ed = 2.5 * d_f_y / d_gamma_M6_ser;
        // Bearing resistance of the plate and the pin
        // SLS
        d_F_b_Ed_ser = d_F_Ed_ser; // All plates
        // Bending resistance of the pin
        // SLS
        double d_M_Ed_1_ser = d_Calc_M_Ed(d_t_21, d_t_11, d_t_c, 0.5 * d_F_Ed_ser);
        double d_M_Ed_2_ser = d_Calc_M_Ed(d_t_12, d_t_22, d_t_c, 0.5 * d_F_Ed_ser);
        d_M_Ed_ser = MathF.Min(d_M_Ed_1_ser, d_M_Ed_2_ser);
    }
    // Table 3.10 Design criteria for pin connections
    // Shear resistance of the pin
    d_F_v_Rd = 0.6 * d_A * (d_f_up / d_gamma_M2);
    // Bearing resistance of the plate and the pin
    d_F_b_Rd = 1.5 * Math.Min(d_t_1, d_t_2) * d_d * (d_f_y / d_gamma_M0);
    // Bending resistance of the pin
    d_M_Rd = 1.5 * d_W_el * (d_f_yp / d_gamma_M0);

    if (b_index_REPLACE == true)
    {
        // Bearing resistance of the plate and the pin
        // If the pin is intended to be replaceable this requirement should also be satisfied.
        d_F_b_Rd_ser = 0.6 * Math.Min(d_t_1, d_t_2) * d_d * (d_f_y / d_gamma_M6_ser);
        // Bending resistance of the pin
        // If the pin is intended to be replaceable this requirement should also be satisfied.
        d_M_Rd_ser = 0.8 * d_W_el * (d_f_yp / d_gamma_M6_ser);
    }
    // Check ratios
    if (b_index_REPLACE == true)
    {
        // Bearing resistance of the plate and the pin
        // If the pin is intended to be replaceable this requirement should also be satisfied.
        d_ratio_3 = d_F_b_Ed_ser / d_F_b_Rd_ser;
        // Bending resistance of the pin
        // If the pin is intended to be replaceable this requirement should also be satisfied.
        d_ratio_5 = d_M_Ed_ser / d_M_Rd_ser;
        // Local state of stress (3.14)
        d_ratio_7 = d_Sigma_h_Ed / d_f_h_Ed;
    }
  else 
      {
      d_ratio_3 = 0.0;
      d_ratio_5 = 0.0;
      d_ratio_7 = 0.0;
      }

    // Table 3.10: Design criteria for pin connections
    // Shear resistance of the pin
    d_ratio_1 = d_F_v_Ed / d_F_v_Rd;
    // Bearing resistance of the plate and the pin
    d_ratio_2 = d_F_b_Ed / d_F_b_Rd;
    // Bending resistance of the pin
    d_ratio_4 = d_M_Ed / d_M_Rd;
    // Combined shear and bending resistance of the pin
    d_ratio_6 = (Math.Pow(d_M_Ed / d_M_Rd, 2) + Math.Pow(d_F_v_Ed / d_F_v_Rd, 2)) / 1;

    // Maximum ratio
    d_ratio_max = MathF.Max(
        d_ratio_1,
        d_ratio_2,
        d_ratio_3,
        d_ratio_4,
        d_ratio_5,
        d_ratio_6,
        d_ratio_7
        );
        
    // Table 3.9: Geometrical requirements for pin ended members
    // (1) Type A: Given thickness t
    d_a_p1 = ((0.5 * d_F_Ed * d_gamma_M0) / (2 * d_t_1_min * d_f_y)) + ((2 * d_d_0) / 3);
    d_c_p1 = ((0.5 * d_F_Ed * d_gamma_M0) / (2 * d_t_1_min * d_f_y)) + (d_d_0 / 3);
    // (2) Type B: Given geometry
    d_t_p2 = 0.7 * Math.Sqrt((0.5 * d_F_Ed * d_gamma_M0) / d_f_y); // 1 plate
    d_d0_p2 = 2.5 * d_t_1_min; // for min required t (t11)

    d_03d0_p2 = 0.30 * d_d_0;
    d_075d0_p2 = 0.75 * d_d_0;
    d_1d0_p2 = 1.00 * d_d_0;
    d_13d0_p2 = 1.3 * d_d_0;
    d_16d0_p2 = 1.6 * d_d_0;
    d_25d0_p2 = 2.5 * d_d_0;

}


// Auxiliary method for Main
double d_Calc_M_Ed(double d_a, double d_b, double d_c, double d_F)
{
    // Figure 3.11: Bending moment in a pin
    double d_M = d_F / 8 * (d_b + 4 * d_c + 2 * d_a);
    return d_M;
}
// Metoda - Nastaví vypocitane hodnoty v textboxoch
public void Set_data()
{
    // Prevod na vystupne jednotky

    d_A /= Math.Pow(d_ratio_mili,2);
    d_W_el /= Math.Pow(d_ratio_mili,3);

    d_f_y *= d_ratio_mega;
    d_f_u *= d_ratio_mega;

    d_f_yp *= d_ratio_mega;
    d_f_up *= d_ratio_mega;

    d_a_p1 /= d_ratio_mili;
    d_c_p1 /= d_ratio_mili;

    d_t_p2 /= d_ratio_mili;
    d_d0_p2 /= d_ratio_mili;

    d_03d0_p2 /= d_ratio_mili;
    d_075d0_p2 /= d_ratio_mili;
    d_1d0_p2 /= d_ratio_mili;
    d_13d0_p2 /= d_ratio_mili;
    d_16d0_p2 /= d_ratio_mili;
    d_25d0_p2 /= d_ratio_mili;

    d_ratio_1 /= d_ratio_percent;
    d_ratio_2 /= d_ratio_percent;
    d_ratio_3 /= d_ratio_percent;
    d_ratio_4 /= d_ratio_percent;
    d_ratio_5 /= d_ratio_percent;
    d_ratio_6 /= d_ratio_percent;
    d_ratio_7 /= d_ratio_percent;

    d_ratio_max /= d_ratio_percent;


    // Nastavia sa načítané a vypocitane hodnoty (skonvetovane z double na string)

    int decimal_pos1 = 1;

    d_A_textB.Text = Math.Round(d_A,decimal_pos1).ToString();
    d_Wel_textB.Text = Math.Round(d_W_el,decimal_pos1).ToString();

    d_dfy_textB.Text = d_f_y.ToString();
    d_dfu_textB.Text = d_f_u.ToString();
    d_dfyp_textB.Text = d_f_yp.ToString();
    d_dfup_textB.Text = d_f_up.ToString();

    d_a_p1_textB.Text = Math.Round(d_a_p1,decimal_pos1).ToString();
    d_c_p1_textB.Text = Math.Round(d_c_p1,decimal_pos1).ToString();

    d_t_p2_textB.Text = Math.Round(d_t_p2,decimal_pos1).ToString();
    d_d0_p2_textB.Text = Math.Round(d_d0_p2,decimal_pos1).ToString();

    d_03d0_p2_textB.Text = Math.Round(d_03d0_p2,decimal_pos1).ToString();
    d_075d0_p2_textB.Text = Math.Round(d_075d0_p2,decimal_pos1).ToString();
    d_1d0_p2_textB.Text = Math.Round(d_1d0_p2,decimal_pos1).ToString();
    d_13d0_p2_textB.Text = Math.Round(d_13d0_p2,decimal_pos1).ToString();
    d_16d0_p2_textB.Text = Math.Round(d_16d0_p2,decimal_pos1).ToString();
    d_25d0_p2_textB.Text = Math.Round(d_25d0_p2, decimal_pos1).ToString();

    int decimal_pos2 = 1;

    d_ratio_1_textB.Text = Math.Round(d_ratio_1, decimal_pos2).ToString();
    d_ratio_2_textB.Text = Math.Round(d_ratio_2, decimal_pos2).ToString();
    d_ratio_3_textB.Text = Math.Round(d_ratio_3, decimal_pos2).ToString();
    d_ratio_4_textB.Text = Math.Round(d_ratio_4, decimal_pos2).ToString();
    d_ratio_5_textB.Text = Math.Round(d_ratio_5, decimal_pos2).ToString();
    d_ratio_6_textB.Text = Math.Round(d_ratio_6, decimal_pos2).ToString();
    d_ratio_7_textB.Text = Math.Round(d_ratio_7, decimal_pos2).ToString();

    d_ratio_max_textB.Text = Math.Round(d_ratio_max, decimal_pos2).ToString();
}

// Calculation control and results control
public void Control_Calc_Error()
{
    if ((d_d_0 <= d_d) || (d_d_in > 0.4 * d_d))
    {
        d_ratio_1_textB.Text = " ERROR! ";
        d_ratio_2_textB.Text = " ERROR! ";
        d_ratio_3_textB.Text = " ERROR! ";
        d_ratio_4_textB.Text = " ERROR! ";
        d_ratio_5_textB.Text = " ERROR! ";
        d_ratio_6_textB.Text = " ERROR! ";
        d_ratio_7_textB.Text = " ERROR! ";
        d_ratio_max_textB.Text = " ERROR! ";
    }


}


// Metoda ktora sa spusti po stlaceni tlacidla calculate
private void Calculate_Click_1(object sender, EventArgs e)
{
    // Načítanie dat
    this.Load_data();
    // Načítanie dát pre oceľ
    this.Load_data_Steel();
    // Uprava jednotek na SI
    this.Convert_data_units();
    // Control messsage - SI UNITS
    // this.Control_Message_SI_Units();         ////////// JUST TEMPORARY CONTROL
    // Control pin joint geometry
    this.Control_Geometry();
    // Vypocet
    this.EN1993_1_8_Main();
    // Zapísanie výsledkov do READONLY textboxov
    this.Set_data();
    // Control calculation and abort results if any wrong data occured
    this.Control_Calc_Error();
}
// Cancel dialog
private void button1_Click(object sender, EventArgs e)
{
    this.DialogResult = DialogResult.Cancel;
}
private void b_index_REPLACE_checkbox_CheckedChanged(object sender, EventArgs e)
{
    b_index_REPLACE = b_index_REPLACE_checkbox.Checked;
}
private void b_index_SOLID_checkbox_CheckedChanged(object sender, EventArgs e)
{
    b_index_SOLID = b_index_SOLID_checkbox.Checked;
    if (b_index_SOLID == true) d_din_textB.ReadOnly = true;
    if (b_index_SOLID == false) d_din_textB.ReadOnly = false;

    if (b_index_SOLID == true)
    {
        if (b_index_PLATES32 == true)
        {
            this.pictureBox3.Image = global::CENEX.Properties.Resources.pin_32_s;
        }
        else
        {
            this.pictureBox3.Image = global::CENEX.Properties.Resources.pin_21_s;
        }
    }
    else
    {
        if (b_index_PLATES32 == true)
        {
            this.pictureBox3.Image = global::CENEX.Properties.Resources.pin_32_h;
        }
        else
        {
            this.pictureBox3.Image = global::CENEX.Properties.Resources.pin_21_h;
        }
    }

}
private void b_plates32_radioB_CheckedChanged(object sender, EventArgs e)
{
    b_index_PLATES32 = b_plates32_radioB.Checked;
    if (b_index_PLATES32 == true)
    {
        b_plates21_radioB.Checked = false;
        d_t12_textB.ReadOnly = false;
        d_t23_textB.ReadOnly = false;
        if (b_index_SOLID == true)
        {
            this.pictureBox3.Image = global::CENEX.Properties.Resources.pin_32_s;
        }
        else
        {
            this.pictureBox3.Image = global::CENEX.Properties.Resources.pin_32_h;
        }
    }
    else
    {
        b_plates21_radioB.Checked = true;
        d_t12_textB.Text = "0";
        d_t23_textB.Text = "0";
        d_t12_textB.ReadOnly = true;
        d_t23_textB.ReadOnly = true;
        if (b_index_SOLID == true)
        {

            this.pictureBox3.Image = global::CENEX.Properties.Resources.pin_21_s;
        }
        else
        {
            this.pictureBox3.Image = global::CENEX.Properties.Resources.pin_21_h;
        }
    }
}
private void b_plates21_radioB_CheckedChanged(object sender, EventArgs e)
{
    b_index_PLATES21 = b_plates21_radioB.Checked;
    if (b_index_PLATES21 == true)
    {
        b_plates32_radioB.Checked = false;
        d_t12_textB.Text = "0";
        d_t23_textB.Text = "0";
        d_t12_textB.ReadOnly = true;
        d_t23_textB.ReadOnly = true;
        if (b_index_SOLID == true)
        {

            this.pictureBox3.Image = global::CENEX.Properties.Resources.pin_21_s;
        }
        else
        {
            this.pictureBox3.Image = global::CENEX.Properties.Resources.pin_21_h;
        }

    }

    else
    {
        b_plates32_radioB.Checked = true;
        d_t12_textB.ReadOnly = false;
        d_t23_textB.ReadOnly = false;
        if (b_index_SOLID == true)
        {
            this.pictureBox3.Image = global::CENEX.Properties.Resources.pin_32_s;
        }
        else
        {
            this.pictureBox3.Image = global::CENEX.Properties.Resources.pin_32_h;
        }
    }

}

private void comboBox_Steel_PLATE_SelectedIndexChanged(object sender, EventArgs e)
{
    mat1_id = comboBox_Steel_PLATE.SelectedIndex;
    d_f_y = steel_properties[mat1_id, 0];
    d_f_u = steel_properties[mat1_id, 1];
    d_dfy_textB.Text = d_f_y.ToString();
    d_dfu_textB.Text = d_f_u.ToString();
}

private void comboBox_Steel_PIN_SelectedIndexChanged(object sender, EventArgs e)
{
    mat2_id = comboBox_Steel_PIN.SelectedIndex;
    d_f_yp = steel_properties[mat2_id, 0];
    d_f_up = steel_properties[mat2_id, 1];
    d_dfyp_textB.Text = d_f_yp.ToString();
    d_dfup_textB.Text = d_f_up.ToString();
}





    }
}
